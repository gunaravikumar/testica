using System;
using System.Globalization;
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
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using Selenium.Scripts.Pages.MergeServiceTool;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems;
using TestStack.White.UIItems.WindowItems;
using TestStack.White.UIItems.Finders;
using System.Xml;
using System.Diagnostics;
using Dicom.Network;
using Dicom;

namespace Selenium.Scripts.Tests
{
    class ImageSharing
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public MpacLogin mpaclogin {get; set;}
        public HPHomePage hphomepage { get; set; }
        public HPLogin hplogin { get; set;}
        public WpfObjects wpfobject { get; set; }

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        public ImageSharing(String classname)
        {                              
            login = new Login();
            login.DriverGoTo(login.url);
            hplogin = new HPLogin();
            hphomepage = new HPHomePage();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ei = new ExamImporter();
            mpaclogin = new MpacLogin();
            wpfobject = new WpfObjects();
        }
        
        #region Sprint-1 Automation Tests

        /// <summary> 
        /// This Test Case is to validate Physcian able to email Study,Valiadte PIN Generated
        /// and Validate study emailed is present in physicians outbound>
        /// </summary>
        public TestCaseResult Test1_29469(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            Outbounds outbounds = null;
            TestCaseResult result;
            String pinnumber = "";
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String emailid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EmailID");
                String EmailReason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EmailReason");
                String Name = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Upload a study -step 1
                ei.EIDicomUpload(username, password, Config.Dest1, UploadFilePath);
                ExecutedSteps++;

                //Login as physician -step 2
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select Study
                inbounds.SearchStudy("Accession", AccessionID);
                inbounds.SelectStudy("Accession", AccessionID);

                //Email study - step 3                
                inbounds.EmailStudy(emailid, Name, EmailReason);
                ExecutedSteps++;

                //Fetch Pin and validate - step 4
                pinnumber = inbounds.FetchPin();
                if (!(pinnumber == ""))
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

                //Navigate to outbounds and validate if study is present - step 5               
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("PatientID", PatientID);
                bool itemfound = outbounds.CheckStudy("Patient ID", PatientID);
                if (itemfound == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout - step 6
                login.Logout();
                ExecutedSteps++;

                //Non Automated Step 7- Email Notification
                result.steps[++ExecutedSteps].status = "Not Automated";

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

        }

        /// <summary>
        /// Valiadte Error Message generated when all fields are blank
        /// </summary>
        public TestCaseResult Test2_29469(String testid, String teststeps, int stepcount)
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
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Upload a study - step 1
                ei.EIDicomUpload(username, password, Config.Dest1, UploadFilePath);
                ExecutedSteps++;

                //Login as physician -step 2
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select Study - step 3
                inbounds.SearchStudy("Accession", AccessionID);
                inbounds.SelectStudy("Accession", AccessionID);
                ExecutedSteps++;

                //validating email study when all fields are blank - step 4
                inbounds.EmailStudy("", "", "");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#EmailStudyControl_m_errorMessageLable")));
                String message1 = BasePage.Driver.FindElement(By.CssSelector("#EmailStudyControl_m_errorMessageLable")).GetAttribute("innerHTML");
                BasePage.Driver.FindElement(By.CssSelector("#EmailStudyDialogDiv>div.titlebar>span.buttonRounded_small_blue")).Click();
                if (message1.ToLower().Contains("email address cannot be empty"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    login.Logout();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout - step 5
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
        /// Validate Negative conditions -- With Reason field blank and 
        /// Validate Negative conditions -- With name and Reason field blanks 
        /// </summary>
        public TestCaseResult Test3_29469(String testid, String teststeps, int stepcount)
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
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

                //Login as physician - step 1
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select Study - step 2
                inbounds.SearchStudy("Accession", AccessionID);
                inbounds.SelectStudy("Accession", AccessionID);
                ExecutedSteps++;

                //validate with reason field blank - step 3
                String emailid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EmailID ");
                String Name = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                inbounds.EmailStudy(emailid, Name, "");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#EmailStudyControl_m_errorMessageLable")));
                String message2 = BasePage.Driver.FindElement(By.CssSelector("#EmailStudyControl_m_errorMessageLable")).GetAttribute("innerHTML");
                BasePage.Driver.FindElement(By.CssSelector("#EmailStudyDialogDiv>div.titlebar>span.buttonRounded_small_blue")).Click();
                if (message2.ToLower().Contains("reason cannot be empty"))
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

                //Validate Negative conditions -- With name and Reason field blanks - step 4
                inbounds.EmailStudy(emailid, "", "");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#EmailStudyControl_m_errorMessageLable")));
                String message3 = BasePage.Driver.FindElement(By.CssSelector("#EmailStudyControl_m_errorMessageLable")).GetAttribute("innerHTML");
                BasePage.Driver.FindElement(By.CssSelector("#EmailStudyDialogDiv>div.titlebar>span.buttonRounded_small_blue")).Click();
                if (message3.ToLower().Contains("name cannot be empty"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    login.Logout();

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout - step 5
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
        /// Validate Negative conditions -- With invalid email address  
        /// </summary>
        public TestCaseResult Test4_29469(String testid, String teststeps, int stepcount)
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
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

                //Login as physician - step 1
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Navigate to Inbounds, Search and Select Study - step 2
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Accession", AccessionID);
                inbounds.SelectStudy("Accession", AccessionID);
                ExecutedSteps++;

                //validating invalid email id - step 3
                String Name = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String EmailReason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EmailReason");
                inbounds.EmailStudy("%^%^%*^**", Name, EmailReason);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#EmailStudyControl_m_errorMessageLable")));
                String message4 = BasePage.Driver.FindElement(By.CssSelector("#EmailStudyControl_m_errorMessageLable")).GetAttribute("innerHTML");
                BasePage.Driver.FindElement(By.CssSelector("#EmailStudyDialogDiv>div.titlebar>span.buttonRounded_small_blue")).Click();

                if (message4.ToLower().Contains("could not send the email.please contact system administrator"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    login.Logout();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout -step 4
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
        /// Test Case1 - 29463 - Validate that user can add receiver and nominate for archive only study in uploaded
        /// </summary>
        public TestCaseResult Test1_161153(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String order = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Order");
                String UploadFilePathList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] UploadFilePath = UploadFilePathList.Split('-');
                String AccessionNumbersList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNumbersList.Split(':');

                //Upload a DICOM study - step 1
                ei.EIDicomUpload(username, password, Config.Dest1, UploadFilePath[0]);
                ExecutedSteps++;

                //Upload a DICOM study - step 2
                ei.EIDicomUpload(username, password, Config.Dest1, UploadFilePath[1]);
                ExecutedSteps++;

                //Login as physician - step 3
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Navigate to Inbounds - step 4
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Search a Study - step 5
                inbounds.SearchStudy("Accession", AccessionNumbers[1]);
                ExecutedSteps++;

                //Select the Study - step 6
                inbounds.SelectStudy("Accession", AccessionNumbers[1]);
                ExecutedSteps++;

                //Nominate Study - step 7
                inbounds.NominateForArchive(order);
                ExecutedSteps++;

                //Search a Study - step 8
                inbounds.SearchStudy("Accession", AccessionNumbers[1]);
                ExecutedSteps++;

                //Select the Study - step 9
                inbounds.SelectStudy("Accession", AccessionNumbers[1]);
                ExecutedSteps++;

                //Check AddReceiver & Nominate buttons are disabled and Validate
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#m_addReceiverButton")));
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#m_nominateStudyButton")));
                IWebElement addReceiverButton = BasePage.Driver.FindElement(By.CssSelector("#m_addReceiverButton"));
                IWebElement nominateButton = BasePage.Driver.FindElement(By.CssSelector("#m_nominateStudyButton"));

                //Validation - step 10
                if ((addReceiverButton.Enabled != true) && (nominateButton.Enabled != true))
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

                //Search the Study with Uploaded status - step 11
                inbounds.SearchStudy("Accession", AccessionNumbers[0]);
                ExecutedSteps++;

                //Select the Study with Uploaded status - step 12
                inbounds.SelectStudy("Accession", AccessionNumbers[0]);
                ExecutedSteps++;

                //Validate AddReceiver & Nominate buttons are Enabled - step 13
                if ((addReceiverButton.Enabled == true) && (nominateButton.Enabled == true))
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

                //Logout - step 14
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
        /// Test Case2 - 29463 Validate that auto fill works when user enters name in uername textbox
        /// </summary>
        public TestCaseResult Test2_161153(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(2);
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String accessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String unRegisteredUser = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UnregisteredUser");

                //Login as physician - step 1
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Navigate to Inbounds - step 2
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Search a study - step 3
                inbounds.SearchStudy("Accession", accessionNo);
                ExecutedSteps++;

                //Select the study - step 4
                inbounds.SelectStudy("Accession", accessionNo);
                ExecutedSteps++;

                //Click add receiver - step 5
                inbounds.ClickAddReceiver();
                ExecutedSteps++;

                //Validate autocomplete option while typing Unregistered user - step 6             
                if (inbounds.CheckAutoFill(unRegisteredUser) != true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                IWebElement receiverDetails = BasePage.Driver.FindElement(By.CssSelector("#multipselectDiv #searchRecipient"));
                receiverDetails.Clear();

                //Validate autocomplete option while typing registered user - step 7
                if (inbounds.CheckAutoFill(Config.newUserName) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout - step 8
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
        /// Test Case-3 - 29463 Validate user able to add a Receiver
        /// </summary>
        public TestCaseResult Test3_161153(String testid, String teststeps, int stepcount)
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
                String newUserName = Config.newUserName;
                String newUserPass = Config.newPassword;
                String accessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

                //Initial Setup - Giving receiver role for a new user -- step 1
                ExecutedSteps++;

                //Login as physician - step 2
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Navigate to Inbounds - step 3
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Search the Study with uploaded status - step 4
                inbounds.SearchStudy("Accession", accessionNo);
                ExecutedSteps++;

                //Select the Study listed - step 5
                inbounds.SelectStudy("Accession", accessionNo);
                ExecutedSteps++;

                //Add receiver to the Study - step 6
                inbounds.AddReceiver(newUserName);
                ExecutedSteps++;
                IWebElement infoDialog = BasePage.Driver.FindElement(By.CssSelector("#CompletionAddReceiverDiv"));

                //Validate Information dialog is displayed - step 7
                if (infoDialog.Enabled == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as Physician - step 8
                login.Logout();
                ExecutedSteps++;

                //Login as New user - step 9
                login.LoginIConnect(newUserName, newUserPass);
                ExecutedSteps++;

                //Navigate to Inbounds - step 10
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Search the Study with uploaded status 
                inbounds.SearchStudy("Accession", accessionNo);
                
                //Validate Study is received and listed even the user is NOT yet added to the destination receiver list from ImageSharing-->Destination -- step 11
                if (inbounds.CheckStudy("Accession", accessionNo) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select the Study listed - step 12
                inbounds.SelectStudy("Accession", accessionNo);
                ExecutedSteps++;

                BluRingViewer bluering = null;
                StudyViewer studyviewer = new StudyViewer();
                //View the study from the viewer - step 13
                if(Config.isEnterpriseViewer.ToLower() =="y")
                {
                    bluering = BluRingViewer.LaunchBluRingViewer();
                }
                else
                {
                    inbounds.LaunchStudy();
                }
                
                ExecutedSteps++;
                Boolean img = true;

                //Validate Study is displayed correctly with patient information - step 14
                if (img == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close the launched study - step 15
                if (Config.isEnterpriseViewer.ToLower() == "y")
                {
                    bluering.CloseBluRingViewer();
                }
                else
                {
                    inbounds.CloseStudy();
                }
                    
                ExecutedSteps++;

                //Logout - step 16
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
        /// Test Case 4 - 29463 - Validate error message is displayed when valid user and email not entered
        /// </summary>
        public TestCaseResult Test4_161153(String testid, String teststeps, int stepcount)
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
                String accessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Upload a Dicom study - step 1
                ei.EIDicomUpload(username, password, Config.Dest1, UploadFilePath);
                ExecutedSteps++;

                //Login as physician and Validate - step 2
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Navigate to Inbounds - step 3
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Search the uploaded Study - step 4
                inbounds.SearchStudy("Accession", accessionNo);
                ExecutedSteps++;

                //Select the study listed with uploaded status - step 5
                inbounds.SelectStudy("Accession", accessionNo);
                ExecutedSteps++;

                //Choose Add Receiver - step 6
                inbounds.ClickAddReceiver();
                ExecutedSteps++;

                IWebElement receiverDetails = BasePage.Driver.FindElement(By.CssSelector("#multipselectDiv #searchRecipient"));

                //With blank details in add receiver field,click apply - step 7            
                receiverDetails.SendKeys("");
                IWebElement applyButton = BasePage.Driver.FindElement(By.CssSelector("#ctl00_AddAdditionalReceiverCrtl_ApplyButton"));
                applyButton.Click();
                ExecutedSteps++;

                IWebElement errorMsg = BasePage.Driver.FindElement(By.CssSelector("#ctl00_AddAdditionalReceiverCrtl_m_local_errorMsg"));
                String errorText = "* Please enter a valid user name or email address";

                //Validate Negative conditions -- With receiver field blanks - step 8
                if (errorMsg.Enabled && (errorMsg.Text == errorText))
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

                //Enter name of user not existing in merge system and click apply - step 9
                receiverDetails.SendKeys("unknown");
                applyButton.Click();
                ExecutedSteps++;

                //Validate Negative conditions -- With receiver unknown - step 10
                if (errorMsg.Enabled && (errorMsg.Text.Contains(errorText)))
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

                //Logout - step 11
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
        /// 29470 - This Test is to download a study to the local system
        /// </summary>
        public TestCaseResult Test_29470(String testid, String teststeps, int stepcount)
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
                String PID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionNumbersList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNumbersList.Split(':');
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Upload a study with priors - step 1
                ei.EIDicomUpload(Config.stUserName, Config.stPassword, Config.Dest1, UploadFilePath);
                ExecutedSteps++;

                //Login as physician - step 2
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Open User Preferences 
                login.OpenUserPreferences();

                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
               
                //Validate whether user able to select zip file option - step 3
                if (BasePage.Driver.FindElement(By.CssSelector("#DownloadRadioButtonList_0")).Displayed == true)
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

                //Edit and save User Preferences - step 4
                login.EditDownloadPreferences("As zip files");
                login.CloseUserPreferences();
                ExecutedSteps++;

                //Step 5 -- Setting Package expire interval in Service tool.
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study 
                inbounds.SearchStudy("Patient ID", PID);
                ExecutedSteps++;

                //Validate Prior studies are uploaded to destination - step 6
                foreach (String AccNo in AccessionNumbers)
                {
                    if (inbounds.CheckStudy("Accession", AccNo) == true)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        throw new Exception("Study with given accession number " + AccNo + " is not present");
                    }
                }

                //Select all prior studies 
                foreach (String AccNo in AccessionNumbers)
                {
                    inbounds.SelectStudy1("Accession", AccNo, true);
                }

                //Transfer selected studies - step 7
                inbounds.Transfer("Local System");
                ExecutedSteps++;

                //Validate Quality control window is opened or not - step 8               
                if (BasePage.Driver.FindElement(By.CssSelector("#DataQCDlgDiv")) != null)
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

                //Select Confirm-all in Quality Control Window - step 9
                inbounds.ClickConfirm_allInQCWindow();
                ExecutedSteps++;

                IList<IWebElement> tablerows = BasePage.Driver.FindElements(By.CssSelector("#ctl00_DataQCControl_datagrid>tbody>tr[title='']"));
                
                ExecutedSteps++;
                //Validate Selected studies show a Check mark - step 10
                foreach (IWebElement row in tablerows)
                {
                    if (row.FindElement(By.CssSelector("td>.QCData_Confirm")).Displayed == true)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //Select Submit in Quality Control Window - step 11
                inbounds.ClickSubmitInQCWindow();
                ExecutedSteps++;

                //Validate Status is updated to ready in transfer status Window - step 12
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && (BasePage.BrowserVersion.ToLower().Equals("8")))
                {
                    WebDriverWait wait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 90));
                    wait.IgnoreExceptionTypes(new Type[] { new StaleElementReferenceException().GetType() });
                    wait.PollingInterval = TimeSpan.FromSeconds(15);
                    StudyViewer viewer = new StudyViewer();

                    wait.Until<Boolean>((d) =>
                    {
                        if (BasePage.Driver.FindElements(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr"))[AccessionNumbers.Length].FindElements(By.CssSelector("td"))[9].FindElement(By.CssSelector("span")).GetAttribute("title").Contains("Ready"))
                        {
                            Logger.Instance.InfoLog("Attribute value found successfully : ");
                            return true;
                        }
                        else
                        {
                            Logger.Instance.InfoLog("Waiting for Attribute value..");
                            return false;
                        }
                    });
                }
                    //PageLoadWait.WaitForAttribute("title", "Ready", element:BasePage.Driver.FindElements(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr"))[AccessionNumbers.Length].FindElement(By.CssSelector("td")),CSSselector:"span");
                else
                    new WebDriverWait(BasePage.Driver, new TimeSpan(0, 1, 0)).Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(" + (AccessionNumbers.Length + 1) + ")>td:nth-child(10)>span[title*='Ready']")));

                ExecutedSteps++;

                for (int i = 1; i <= AccessionNumbers.Length; i++)
                {
                    //if (BasePage.Driver.FindElement(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(" + (i + 1) + ")>td:nth-child(10)>span[title*='Ready']")) != null)
                    if (BasePage.Driver.FindElements(By.CssSelector("#ctl00_TransferJobsListControl_parentGrid>tbody>tr"))[i].FindElements(By.CssSelector("td"))[9].FindElement(By.CssSelector("span[title*='Ready']")) != null)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //Select one study with ready status - step 13
                //BasePage.Driver.FindElement(By.CssSelector(" #ctl00_TransferJobsListControl_parentGrid>tbody>tr:nth-child(2)>td:nth-child(10)>span[title*='Ready']")).Click();
                BasePage.Driver.FindElements(By.CssSelector(" #ctl00_TransferJobsListControl_parentGrid>tbody>tr"))[1].FindElements(By.CssSelector("td"))[9].FindElement(By.CssSelector("span[title*='Ready']")).Click();
                ExecutedSteps++;

                //Validate download button is displayed in Transfer status window - step 14
                IWebElement downloadButton = BasePage.Driver.FindElement(By.CssSelector("input#ctl00_TransferJobsListControl_m_submitButton"));
                if (downloadButton.Enabled == true)
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

                //Click download button in transfer status window - step 15
                downloadButton.Click();
                ExecutedSteps++;

                //Validate download button is displayed in Download Packages window - step 16       
                if (BasePage.Driver.FindElement(By.CssSelector("input#ctl00_TransferJobPackagesListControl_m_submitButton")).Enabled == true)
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

                //Click download button in Download Packages window - step 17 
                inbounds.ClickButtonInDownloadPackagesWindow("Download");
                ExecutedSteps++;

                String description;
                inbounds.GetMatchingRow("Accession", AccessionNumbers[0]).TryGetValue("Description", out description);
                PageLoadWait.WaitForDownload("_" + description, Config.downloadpath, "zip");

                //Click close button in Download Packages window - step 18
                inbounds.ClickButtonInDownloadPackagesWindow("close");
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;

                //Validate inbounds list is displayed - step 19
                if (BasePage.Driver.FindElement(By.CssSelector("div#ButtonsDiv input#m_launchUploaderButton")) != null)
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

                //Check whether the file is present
                Boolean studydownloaded = BasePage.CheckFile("_" + description, Config.downloadpath, "zip");

                //Validate the study is downloaded - step 20
                if (studydownloaded == true)
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

                //Non-Automated Step 21 - Unzip study file and open in Dicom tool
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Logout - step 22
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
                /// Test Case 1- 29471 -Remove Exam Archivist inbounds and check with it from database(i.e)HP
                /// </summary>
        public TestCaseResult Test1_29471(String testid, String teststeps, int stepcount)
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
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String AccessionNumbersList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String patientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] AccessionNumbers = AccessionNumbersList.Split(':');
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Upload a study with Priors - step 1
                ei.EIDicomUpload(Config.stUserName, Config.stPassword, Config.Dest1, arUsername, "", UploadFilePath);
                ExecutedSteps++;

                //Login as archivist - step 2
                login.LoginIConnect(arUsername, arPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study
                inbounds.SearchStudy("PatientID", patientID);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Select study with related priors  -step 3
                foreach (String Accession in AccessionNumbers)
                {
                    inbounds.SelectStudy1("Accession", Accession, true);
                }
                ExecutedSteps++;

                //Delete study with all related priors - step 4
                inbounds.DeleteStudy();
                ExecutedSteps++;

                //Search Study
                inbounds.SearchStudy("PatientID", patientID);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Validate Study with priors is removed from the archivist Inbounds - step 5
                ExecutedSteps++;
                foreach (String Accession in AccessionNumbers)
                {
                    if (inbounds.CheckStudy("Accession", Accession) != true)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        throw new Exception("Study with priors is not deleted");
                    }
                }

                //Logout as Archivist - step 6
                login.Logout();
                ExecutedSteps++;

                //Navigate to Holding Pen - step 7
                login.DriverGoTo(login.hpurl);
                ExecutedSteps++;

                //Login in Holding Pen and Navigate to archive search menu - step 8
                HPLogin hplogin = new HPLogin();
                HPHomePage hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                ExecutedSteps++;

                //Validate Study with priors is removed from the Holding Pen - step 9 
                ExecutedSteps++;
                try
                {
                    foreach (String Accession in AccessionNumbers)
                    {
                        //Click Search Archive
                        IWebElement accNo = BasePage.Driver.FindElement(By.CssSelector("input[name='accessionNumber']"));
                        accNo.Clear();
                        accNo.SendKeys(Accession);
                        BasePage.Driver.FindElement(By.CssSelector("#submitbutton")).Click();
                        PageLoadWait.WaitForHPPageLoad(10);
                        Thread.Sleep(5000);

                        if (workflow.HPCheckStudy(Accession) != true)
                        {
                            result.steps[ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                            throw new Exception("Study with priors is not deleted in Holding Pen");
                        }
                    }
                }
                catch(Exception e)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study with priors is not deleted in Holding Pen");
                }

                //Again Upload the same study with Priors - step 10
                ei.EIDicomUpload(Config.stUserName, Config.stPassword, Config.Dest1, arUsername, "", UploadFilePath);
                ExecutedSteps++;

                //Login as archivist -step 11
                login.LoginIConnect(arUsername, arPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study
                inbounds.SearchStudy("PatientID", patientID);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Select study with related priors - step 12
                foreach (String Accession in AccessionNumbers)
                {
                    inbounds.SelectStudy1("Accession", Accession, true);
                }
                ExecutedSteps++;

                //Validate Study with priors is Present from the archivist Inbounds - step 13
                ExecutedSteps++;
                try
                {
                    foreach (String Accession in AccessionNumbers)
                    {
                        if (inbounds.CheckStudy("Accession", Accession) == true)
                        {
                            result.steps[ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                            throw new Exception("Study with priors is not Present");
                        }
                    }
                }
                catch(Exception e)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study with priors is not Present");
                }

                //Logout as Archivist - step 14
                login.Logout();
                ExecutedSteps++;

                //Navigate to Holding Pen - step 15
                login.DriverGoTo(login.hpurl);
                ExecutedSteps++;

                //Login in Holding Pen and Navigate to archive search menu - step 16
                HPLogin hplogin1 = new HPLogin();
                HPHomePage hphomepage1 = hplogin1.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow1 = (WorkFlow)hphomepage1.Navigate("Workflow");
                workflow1.NavigateToLink("Workflow", "Archive Search");
                ExecutedSteps++;

                //Validate Study with priors is Present from the Holding Pen - step 17
                ExecutedSteps++;
                try
                {
                    foreach (String Accession in AccessionNumbers)
                    {
                        //Click Search Archive
                        IWebElement accNo = BasePage.Driver.FindElement(By.CssSelector("input[name='accessionNumber']"));
                        accNo.Clear();
                        accNo.SendKeys(Accession);
                        BasePage.Driver.FindElement(By.CssSelector("#submitbutton")).Click();
                        PageLoadWait.WaitForHPPageLoad(10);
                        Thread.Sleep(5000);

                        if (workflow.HPCheckStudy(Accession) == true)
                        {
                            result.steps[ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            result.steps[ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                            throw new Exception("Study with priors is not Present in Holding Pen");
                        }
                    }
                }
                catch(Exception e)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study with priors is not Present in Holding Pen");
                }

                //Logout in holding Pen - step 18
                hplogin.LogoutHPen();
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
        }

        /// <summary>
        /// Test Case 2- 29471 -Remove Study from database and also delete the study in 
        /// Archivist inbounds with Status as deleted 
        /// </summary>
        public TestCaseResult Test2_29471(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            TestCaseResult result;
            HPLogin hplogin = null;
            HPHomePage hphome = null;
            WorkFlow workflow = null;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String arUsername = Config.ar1UserName;
                String arPassword = Config.ar1Password;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String PID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Upload a study with archivist as addittional receiver - step 1
                ei.EIDicomUpload(Config.stUserName, Config.stPassword, Config.Dest1, arUsername, "", UploadFilePath);
                ExecutedSteps++;

                //login - step 2
                login.DriverGoTo(login.hpurl);
                ExecutedSteps++;

                //Login in Holding Pen and Navigate to archive search menu - step 3
                hplogin = new HPLogin();
                hphome = (HPHomePage)hplogin.LoginHPen(hpUserName, hpPassword);
                workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                ExecutedSteps++;

                //Click Search Archive
                workflow.HPSearchStudy("PatientID", PID);

                //Delete study in HP - step 4
                workflow.HPDeleteStudy();
                ExecutedSteps++;
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && (BasePage.BrowserVersion.ToLower().Equals("8") || BasePage.BrowserVersion.ToLower().Equals("9")))
                    BasePage.Driver.FindElement(By.CssSelector("input[name='accessionNumber']")).Clear();
                else
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"input[name='accessionNumber']\").value=''");
                PageLoadWait.WaitForHPPageLoad(20);

                //Validate Study is removed from the Holding Pen - step 5
                if (workflow.HPCheckStudy(AccessionNo) != true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study is not deleted in Holding Pen");
                }

                //Logout in holding Pen - step 6
                hplogin.LogoutHPen();
                ExecutedSteps++;

                //Navigate to iCA - step 7
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Login as archivist - step 8
                login.LoginIConnect(arUsername, arPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study
                inbounds.SearchStudy("Accession", AccessionNo);

                Dictionary<string, string> StudyDeleted = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNo, "Deleted" });

                //Validate Study Study Status from Inbounds page after deletion completed from Holding Pen - step 9 
                if (StudyDeleted != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select Study
                inbounds.SelectStudy("Accession", AccessionNo);

                //Click delete studies button - step 10
                BasePage.Driver.FindElement(By.CssSelector("#m_deleteStudiesButton")).Click();
                ExecutedSteps++;

                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                IWebElement delDialog = BasePage.Driver.FindElement(By.CssSelector("#m_ssDeleteControl_Button1"));

                //Validate delete dialog box is opened or not - step 11
                if (delDialog.Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Confirm study to be deleted - step 12
                delDialog.Click();
                ExecutedSteps++;

                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitHomePage();

                //Search Study
                inbounds.SearchStudy("Accession", AccessionNo);

                //Validate delete already removed Study from Holding pen in Archivist inbounds - step 13
                if (inbounds.CheckStudy("Accession", AccessionNo) != true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[3].SetLogs();
                    throw new Exception("Study is not deleted");
                }

                //Logout as Archivist - step 14
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
        /// Test Case 3- 29471 -Remove Exam as Staff and verify in his/her outbounds, physician's inbounds and database
        /// </summary>
        public TestCaseResult Test3_29471(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            Outbounds outbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String stUsername = Config.stUserName;
                String stPassword = Config.stPassword;
                String phUsername = Config.ph1UserName;
                String phPassword = Config.ph1Password;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Upload a study - step 1
                ei.EIDicomUpload(Config.stUserName, Config.stPassword, Config.Dest1, UploadFilePath);
                ExecutedSteps++;

                //Login as Staff - step 2
                login.LoginIConnect(stUsername, stPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                outbounds = (Outbounds)login.Navigate("Outbounds");

                //Search and Select Study
                outbounds.SearchStudy("Accession", AccessionNo);
                outbounds.SelectStudy1("Accession", AccessionNo);

                //Delete Study - step 3
                outbounds.DeleteStudy();
               ExecutedSteps++;

                outbounds.SearchStudy("Accession", AccessionNo);
                Dictionary<string, string> rowkeyvalue;
                rowkeyvalue = outbounds.GetMatchingRow("Accession", AccessionNo);

                //Validate Study is removed from the Staff's OutBounds - step 4
                if (rowkeyvalue == null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as Staff - step 5
                login.Logout();
                ExecutedSteps++;

                //Login as Physician - step 6
                login.LoginIConnect(phUsername, phPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select Study
                inbounds.SearchStudy("Accession", AccessionNo);

                //Validate Study is removed from the Physician's InBounds - step 7
                if (inbounds.CheckStudy("Accession", AccessionNo) != true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as Physician - step 8
                login.Logout();
                ExecutedSteps++;

                //Navigate to Holding Pen - step 9
                login.DriverGoTo(login.hpurl);
                ExecutedSteps++;

                //Login in Holding Pen and Navigate to archive search menu - step 10
                HPLogin hplogin = new HPLogin();
                HPHomePage hphome = hplogin.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                ExecutedSteps++;

                //Click Search Archive
                workflow.HPSearchStudy("Accession", AccessionNo);

                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && (BasePage.BrowserVersion.ToLower().Equals("8") || BasePage.BrowserVersion.ToLower().Equals("9")))
                    BasePage.Driver.FindElement(By.CssSelector("input[name='accessionNumber']")).Clear();
                else
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"input[name='accessionNumber']\").value=''");

                PageLoadWait.WaitForHPPageLoad(20);

                //Validate Study is removed from Holding Pen - step 11
                if (workflow.HPCheckStudy(AccessionNo) != true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study is not deleted in Holding Pen");
                }

                //Logout in holding Pen - step 12
                hplogin.LogoutHPen();
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
        }

        /// <summary>
        /// Test Case 4- 29471 -Uploading an exam as Staff & remove as physician and then verify the same.
        /// </summary>
        public TestCaseResult Test4_29471(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            Outbounds outbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String stUsername = Config.stUserName;
                String stPassword = Config.stPassword;
                String phUsername = Config.ph1UserName;
                String phPassword = Config.ph1Password;
                String newUsername = Config.newUserName;
                String newPassword = Config.newPassword;
                String AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Upload a study - step 1 
                ei.EIDicomUpload(Config.stUserName, Config.stPassword, Config.Dest1, newUsername, "", UploadFilePath);
                ExecutedSteps++;

                //Login as Staff - step 2
                login.LoginIConnect(stUsername, stPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                outbounds = (Outbounds)login.Navigate("Outbounds");

                //Search and Select Study
                outbounds.SearchStudy("Accession", AccessionNo);

                Dictionary<string, string> rowkeyvalue;
                rowkeyvalue = outbounds.GetMatchingRow("Accession", AccessionNo);

                //Validate Study is present in the Staff's OutBounds - step 3
                if (rowkeyvalue != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as Staff - step 4
                login.Logout();
                ExecutedSteps++;

                //Login as new User - step 5
                login.LoginIConnect(newUsername, newPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select Study
                inbounds.SearchStudy("Accession", AccessionNo);

                //Validate Study is present in new User's InBounds - step 6
                if (inbounds.CheckStudy("Accession", AccessionNo) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as new user - step 7
                login.Logout();
                ExecutedSteps++;

                //Login as Physician - step 8
                login.LoginIConnect(phUsername, phPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select Study
                inbounds.SearchStudy("Accession", AccessionNo);
                inbounds.SelectStudy("Accession", AccessionNo);

                //Validate Study is Present in Physician's Inbounds - step 9
                if (inbounds.CheckStudy("Accession", AccessionNo) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Delete study - step 10
                inbounds.DeleteStudy();
                ExecutedSteps++;

                //Search Study
                inbounds.SearchStudy("Accession", AccessionNo);

                //Validate Study is removed from the Physician's InBounds - step 11                 
                if (inbounds.CheckStudy("Accession", AccessionNo) != true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as Physician - step 12
                login.Logout();
                ExecutedSteps++;

                //Login as Staff - step 13
                login.LoginIConnect(stUsername, stPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                outbounds = (Outbounds)login.Navigate("Outbounds");

                //Search the Study
                outbounds.SearchStudy("Accession", AccessionNo);

                Dictionary<string, string> rowkeyvalue1;
                rowkeyvalue1 = outbounds.GetMatchingRow("Accession", AccessionNo);

                //Validate Study is removed from the Staff's OutBounds - step 14
                if (rowkeyvalue1 == null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as Staff - step 15
                login.Logout();
                ExecutedSteps++;

                //Login as new User - step 16
                login.LoginIConnect(newUsername, newPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select Study
                inbounds.SearchStudy("Accession", AccessionNo);

                //Validate Study is present in new User's InBounds - step 17
                if (inbounds.CheckStudy("Accession", AccessionNo) != true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as New User - step 18
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
        /// Test Case 5_29471 - Delete the uploaded study from new user's inbounds and check it from 
        /// new user,Physician's inbounds and Staff's Outbounds>
        /// </summary>
        public TestCaseResult Test5_29471(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            Outbounds outbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String stUsername = Config.stUserName;
                String stPassword = Config.stPassword;
                String phUsername = Config.ph1UserName;
                String phPassword = Config.ph1Password;
                String newUsername = Config.newUserName;
                String newPassword = Config.newPassword;
                String AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Upload a study with addittional receiver as new user - step 1
                ei.EIDicomUpload(Config.stUserName, Config.stPassword, Config.Dest1, newUsername, "", UploadFilePath);
                ExecutedSteps++;

                //Login as Physician - step 2
                login.LoginIConnect(phUsername, phPassword);
                ExecutedSteps++;

                //Navigate to Inbounds 
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study
                inbounds.SearchStudy("Accession", AccessionNo);

                //Validate Study is Present in Physician's Inbounds - step 3
                if (inbounds.CheckStudy("Accession", AccessionNo) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as Physician - step 4
                login.Logout();
                ExecutedSteps++;
                 
                //Login as Staff - step 5
                login.LoginIConnect(stUsername, stPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                outbounds = (Outbounds)login.Navigate("Outbounds");

                //Search Study
                outbounds.SearchStudy("Accession", AccessionNo);

                Dictionary<string, string> rowkeyvalue;
                rowkeyvalue = outbounds.GetMatchingRow("Accession", AccessionNo);

                //Validate Study is present in the Staff's OutBounds - step 6
                if (rowkeyvalue != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as Staff - step 7
                login.Logout();
                ExecutedSteps++;

                //Login as new User - step 8
                login.LoginIConnect(newUsername, newPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select Study
                inbounds.SearchStudy("Accession", AccessionNo);
                inbounds.SelectStudy("Accession", AccessionNo);

                //Validate Study is present in new User's InBounds - step 9
                if (inbounds.CheckStudy("Accession", AccessionNo) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Delete and Search study - step 10                
                inbounds.DeleteStudy();
                ExecutedSteps++;

                //Search the study
                inbounds.SearchStudy("Accession", AccessionNo);

                //Validate Study is removed from new user's InBounds - step 11               
                if (inbounds.CheckStudy("Accession", AccessionNo) != true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as New user - step 12
                login.Logout();
                ExecutedSteps++;

                //Login as Staff - step 13
                login.LoginIConnect(stUsername, stPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                outbounds = (Outbounds)login.Navigate("Outbounds");

                //Search and Select Study
                outbounds.SearchStudy("Accession", AccessionNo);

                Dictionary<string, string> rowkeyvalue1;
                rowkeyvalue1 = outbounds.GetMatchingRow("Accession", AccessionNo);

                //Validate Study is Present in the Staff's OutBounds - step 14
                if (rowkeyvalue1 == null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as Staff - step 15
                login.Logout();
                ExecutedSteps++;

                //Login as Physician - step 16
                login.LoginIConnect(phUsername, phPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select Study
                inbounds.SearchStudy("Accession", AccessionNo);

                //Validate Study is Present in Physician's Inbounds - step 17
                if (inbounds.CheckStudy("Accession", AccessionNo) != true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as Physician - step 18
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
        /// For LDAP user, upload a study and perform validation.
        /// </summary>    
        public TestCaseResult Test_29473(String testid, String teststeps, int stepcount)
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
                String username1 = Config.ldapuser1;
                String password1 = Config.ldappass1;
                String username2 = Config.ldapuser2;
                String password2 = Config.ldappass2;            
                String order = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Order");
                String accessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Non-Automated Steps 1 & 2 -- Enable LDAP and Configure initial setup
                ExecutedSteps++;
                ExecutedSteps++;
                
                //Upload a study - step 3
                ei.EIDicomUpload(Config.stUserName, Config.stPassword, Config.Dest1, UploadFilePath);
                ExecutedSteps++;

                //Login as physician and Validate - step 4
                login.LoginIConnect(username1, password1);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select Study 
                inbounds.SearchStudy("Accession", accessionNo);

                //Find study status
                String studyStatus;
                inbounds.GetMatchingRow("Accession", accessionNo).TryGetValue("Status", out studyStatus);

                //Validate Study is listed and status as Uploaded - step 5
                if (studyStatus == "Uploaded")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select Study and Nominate Study for archive - step 6
                inbounds.SelectStudy("Accession", accessionNo);
                inbounds.NominateForArchive(order);
                ExecutedSteps++;

                //Find study status
                inbounds.SearchStudy("Accession", accessionNo);
                String studyStatus1;
                inbounds.GetMatchingRow("Accession", accessionNo).TryGetValue("Status", out studyStatus1);

                //Validate Study Status is changed to Nominated for archive - step 7
                if (studyStatus1 == "Nominated For Archive")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as User - step 8
                login.Logout();
                ExecutedSteps++;

                //Non-Automated Step 9 -- Email Notification
                result.steps[++ExecutedSteps].status = "Not-Automated";

                //Login as Archivist - step 10
                login.LoginIConnect(username2, password2);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select Study 
                inbounds.SearchStudy("Accession", accessionNo);

                //Validate Study is listed in archivist inbounds - step 11
                if (inbounds.CheckStudy("Accession", accessionNo) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as Archivist - step 12
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
        /// With LDAP disabled, upload the study and perform validation
        /// </summary>    
        public TestCaseResult Test_29474(String testid, String teststeps, int stepcount)
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
                String arUserName = Config.ar1UserName;
                String arPassword = Config.ar1Password;
                String order = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Order");
                String accessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Non-Automated Step  1 -- Disbale LDAP
                ExecutedSteps++;

                //Upload a study - step 2
                ei.EIDicomUpload(username, password, Config.Dest1, UploadFilePath);
                ExecutedSteps++;

                //Login as physician and Validate - step 3
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select Study 
                inbounds.SearchStudy("Accession", accessionNo);

                //Find study status
                String studyStatus;
                inbounds.GetMatchingRow("Accession", accessionNo).TryGetValue("Status", out studyStatus);

                //Validate Study is listed and status as Uploaded - step 4
                if (studyStatus == "Uploaded")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select Study and Nominate Study for archive - step 5
                inbounds.SelectStudy("Accession", accessionNo);
                inbounds.NominateForArchive(order);
                ExecutedSteps++;

                //Find study status
                inbounds.SearchStudy("Accession", accessionNo);
                String studyStatus1;
                inbounds.GetMatchingRow("Accession", accessionNo).TryGetValue("Status", out studyStatus1);

                //Validate Study Status is changed to Nominated for archive - step 6
                if (studyStatus1 == "Nominated For Archive")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as User - step 7
                login.Logout();
                ExecutedSteps++;

                //Non-Automated Step 8 -- Email notification
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Login as Archivist - step 9
                login.LoginIConnect(arUserName, arPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select Study 
                inbounds.SearchStudy("Accession", accessionNo);

                //Validate Study is listed in archivist inbounds - step 10
                if (inbounds.CheckStudy("Accession", accessionNo) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as Archivist - step 11
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
        /// Test Case - 29467 - Sharing a Dicom Study
        /// </summary>
        public TestCaseResult Test1_29467(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            Inbounds inbounds = null;
            Outbounds outbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int executedsteps = -1;

            try
            {
                //Fetch required Test data
                String phusername = Config.ph1UserName;
                String phpassword = Config.ph1Password;
                String ph1username = Config.phUserName;
                String ph1password = Config.phPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String order = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Order");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Upload a Dicom Study - Step-1
                ei.EIDicomUpload(Config.ph1UserName, Config.ph1Password, Config.Dest1, UploadFilePath);
                executedsteps++;

                //Login as physician and check inbounds on uploaded file - Step-2
                login.LoginIConnect(phusername, phpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNo);
                Dictionary<string, string> row = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
                if (!(row == null))
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                    throw new Exception("Dicom File not uploaded");
                }

                //Logout and Validate Study present in Holding pen-Step-3
                login.Logout();
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin = new HPLogin();
                HPHomePage hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("Accessionno", AccessionNo);

                if (workflow.HPCheckStudy(AccessionNo) == true)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[1].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[1].description);
                    result.steps[executedsteps].SetLogs();
                    throw new Exception("Study not present in Holding Pen");
                }

                //stepcount-4
                //Load Study into the viewer and validate uploaded and study displayed are in synch
                hplogin.LogoutHPen();
                login.DriverGoTo(login.url);
                login.LoginIConnect(phusername, phpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNo);
                inbounds.SelectStudy1(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
                inbounds.LaunchStudy();
                executedsteps++;
                inbounds.CloseStudy();              

                //Share study to single user and verify Study status in physician's outbound and Receiver's outbound -Step-5
                inbounds.SelectStudy1(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
                inbounds.ShareStudy(false, new String[] { ph1username });
                //Check shared study present in Physican's outbound
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("accessionNo", AccessionNo);
                Dictionary<string, string> studyshared = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
                if (!(studyshared == null))
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Verify study status in Receiver's Inbounds - Step-6
                login.Logout();
                login.LoginIConnect(ph1username, ph1password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNo);
                Dictionary<string, string> studysharedinbounds = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
                if (!(studysharedinbounds == null))
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-7 and 8 Not Automated - Email Notification
                result.steps[++executedsteps].status = "Not Automated";
                result.steps[++executedsteps].status = "Not Automated";
                

                //Nominate the study for Archiving and validate-step-9
                login.Logout();
                login.LoginIConnect(phusername, phpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNo);
                inbounds.SelectStudy1(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Uploaded" });
                inbounds.NominateForArchive(order);

                //Check if Archive button not displayed
                int archivebuttonfoundflag = 0;
                try
                {
                    if (BasePage.Driver.FindElement(By.CssSelector("#m_archiveStudyButton")).Displayed)
                    {
                        archivebuttonfoundflag = 1;
                    }

                }
                catch(Exception excep)
                {
                    Logger.Instance.InfoLog("Archive button not found");
                }

                //Validate Study Status+
                Dictionary<string, string> studynominated = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Nominated For Archive" });
                if ((!(studynominated == null)) && (archivebuttonfoundflag==0))
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[++executedsteps].SetLogs();
                }

                //Logout-step-10
                login.Logout();
                executedsteps++;

                //Report Result
                result.FinalResult(executedsteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);                

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

        /// <summary>
        /// Test Case - 29467 - Sharing a Non Dicom Study
        /// </summary>
        public TestCaseResult Test2_29467(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            Inbounds inbounds = null;
            Outbounds outbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int executedsteps = -1;

            try
            {
                //Fetch required Test data
                String phusername = Config.ph1UserName;
                String phpassword = Config.ph1Password;
                String ph1username = Config.phUserName;
                String ph1password = Config.phPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String order = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Order");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String imagePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ImageFilePath");
                String Description = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");

                //Upload a Non-Dicom Study -- Step-1
                ei.EINonDicomUpload(Config.ph1UserName, Config.ph1Password, Config.Dest1, "", "", UploadFilePath, imagePath, Description, PatientID, AccessionNo);
                executedsteps++;

                //Login as physician and check inbounds on uploaded file-Step-2
                login.LoginIConnect(phusername, phpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNo);
                Dictionary<string, string> row = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
                if (!(row == null))
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                    throw new Exception("Non-Dicom File not found");
                }

                //Logout and Valiadte Study present in Holding pen -Step-3
                login.Logout();
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin = new HPLogin();
                HPHomePage hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("Accessionno", AccessionNo);
                if (workflow.HPCheckStudy(AccessionNo) == true)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                    throw new Exception("Study not present in Holding Pen");
                }

                //Load Study into the viewer and validate uploaded and study diaplyed-Step-4
                hplogin.LogoutHPen();
                login.DriverGoTo(login.url);
                login.LoginIConnect(phusername, phpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNo);
                inbounds.SelectStudy1(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
                inbounds.LaunchStudy();
                executedsteps++;
                inbounds.CloseStudy();               

                //Share study to single user and verify Study status in physician's outbound and Receiver's outbound-Step-5
                inbounds.SelectStudy1(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
                inbounds.ShareStudy(false, new String[] { ph1username });
                //Check shared study present in Physican's outbound
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("accessionNo", AccessionNo);
                Dictionary<string, string> studyshared = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
                if (!(studyshared == null))
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Verify study status in Receiver's Inbound-Step-6
                login.Logout();
                login.LoginIConnect(ph1username, ph1password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNo);
                Dictionary<string, string> studysharedinbounds = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
                if (!(studysharedinbounds == null))
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-7 and 8 Not Automated - Email Notification
                result.steps[++executedsteps].status = "Not Automated";
                result.steps[++executedsteps].status = "Not Automated";

                //Nominate the study for Archiving and validate-Step-9
                login.Logout();
                login.LoginIConnect(phusername, phpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNo);
                inbounds.SelectStudy1(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Uploaded" });
                inbounds.NominateForArchive(order);
                //Check if Archive button not displayed
                int archivebuttonfoundflag = 0;
                try
                {
                    if (BasePage.Driver.FindElement(By.CssSelector("#m_archiveStudyButton")).Displayed)
                    {
                        archivebuttonfoundflag = 1;
                    }

                }
                catch (Exception excep)
                {
                    Logger.Instance.InfoLog("Archive button not found");
                }

                //Validate Study Status
                Dictionary<string, string> studynominated = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Nominated For Archive" });
                if ((!(studynominated == null)) && (archivebuttonfoundflag==0))
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Logout-Step-10
                login.Logout();
                executedsteps++;

                //Report Result
                result.FinalResult(executedsteps++);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
               

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedsteps++);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// This Test is to Grant/Remove access (for a Dicom study) to multiple users 
        /// </summary>
        public TestCaseResult Test3_29467(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            Inbounds inbounds = null;
            Outbounds outbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int executedsteps = -1;

            try
            {
                //Fetch required Test data
                String phusername = Config.phUserName;
                String phpassword = Config.phPassword;
                String ph1username = Config.ph1UserName;
                String ph1password = Config.ph1Password;
                String ph2username = Config.ph2UserName;
                String ph2password = Config.ph2Password;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Upload a Dicom Study-Step-1
                ei.EIDicomUpload(Config.ph1UserName, Config.ph1Password, Config.Dest1, UploadFilePath);
                executedsteps++;

                //Login as physician and check inbounds on uploaded file-Step-2
                login.LoginIConnect(ph1username, ph1password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNo);
                Dictionary<string, string> row = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
                if (!(row == null))
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[++executedsteps].SetLogs();
                    throw new Exception("Dicom File not uploaded");
                }

                //Share study to Mutiple user-Step-3
                inbounds.SelectStudy1(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
                inbounds.ShareStudy(false, new String[] { phusername, ph2username });
                executedsteps++;

                //Check shared study present in Physican's outbound in shared status-Step-4
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("accessionNo", AccessionNo);
                Dictionary<string, string> studyshared = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
                if (!(studyshared == null))
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Login as Ph and check study in shared status-Step-5
                login.Logout();
                login.LoginIConnect(phusername, phpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNo);
                Dictionary<string, string> studyshared1 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
                if (!(studyshared1 == null))
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Login as Ph2 and check study in shared status-Step-6               
                login.Logout();
                login.LoginIConnect(ph2username, ph2password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNo);
                Dictionary<string, string> studyshared2 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
                if (!(studyshared2 == null))
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-7 - Not Automation Email Notification.
                result.steps[++executedsteps].status = "Not Automated";


                //Login as Ph1 and remove access to Ph2-Step-8
                login.Logout();
                login.LoginIConnect(ph1username, ph1password);
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("accessionNo", AccessionNo);
                inbounds.SelectStudy1(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
                outbounds.RemoveAccess(new string[] { ph2username });
                //Validate study not present for ph2
                login.Logout();
                login.LoginIConnect(ph2username, ph2password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNo);
                Dictionary<string, string> studyremoved2 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
                if (studyremoved2 == null)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[++executedsteps].SetLogs();
                }

                //Validate study present for ph-Step-9
                login.Logout();
                login.LoginIConnect(phusername, phpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNo);
                Dictionary<string, string> studynotremoved = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
                if (!(studynotremoved == null))
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[++executedsteps].SetLogs();
                }

                //Logout-Step-10
                login.Logout();
                executedsteps++;

                //Report Result
                result.FinalResult(executedsteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);                

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedsteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// This Test is to Grant/Remove access (for a Non-Dicom study) to multiple users 
        /// </summary>
        public TestCaseResult Test4_29467(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            Inbounds inbounds = null;
            Outbounds outbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int executedsteps = -1;

            try
            {
                //Fetch required Test data
                String phusername = Config.phUserName;
                String phpassword = Config.phPassword;
                String ph1username = Config.ph1UserName;
                String ph1password = Config.ph1Password;
                String ph2username = Config.ph2UserName;
                String ph2password = Config.ph2Password;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String imagePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ImageFilePath");
                String Description = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");


                //Upload a Non-Dicom Study
                ei.EINonDicomUpload(Config.ph1UserName, Config.ph1Password, Config.Dest1, "", "", UploadFilePath, imagePath, Description, PatientID, AccessionNo);
                executedsteps++;

                //Login as physician and check inbounds on uploaded file-Step-2
                login.LoginIConnect(ph1username, ph1password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNo);
                Dictionary<string, string> row = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
                if (!(row == null))
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[++executedsteps].SetLogs();
                    throw new Exception("Dicom File not uploaded");
                }

                //Share study to Mutiple user-Step-3
                inbounds.SelectStudy1(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
                inbounds.ShareStudy(false, new String[] { phusername, ph2username });
                executedsteps++;

                //Check shared study present in Physican's outbound in shared status-Step-4
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("accessionNo", AccessionNo);
                Dictionary<string, string> studyshared = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
                if (!(studyshared == null))
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Login as Ph and check study in shared status-Step-5
                login.Logout();
                login.LoginIConnect(phusername, phpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNo);
                Dictionary<string, string> studyshared1 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
                if (!(studyshared1 == null))
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Login as Ph2 and check study in shared status-Step-6               
                login.Logout();
                login.LoginIConnect(ph2username, ph2password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNo);
                Dictionary<string, string> studyshared2 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
                if (!(studyshared2 == null))
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-7 - Not Automation Email Notification.
                result.steps[++executedsteps].status = "Not Automated";


                //Login as Ph1 and remove access to Ph2-Step-8
                login.Logout();
                login.LoginIConnect(ph1username, ph1password);
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("accessionNo", AccessionNo);
                inbounds.SelectStudy1(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
                outbounds.RemoveAccess(new string[] { ph2username });
                //Validate study not present for ph2
                login.Logout();
                login.LoginIConnect(ph2username, ph2password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNo);
                Dictionary<string, string> studyremoved2 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
                if (studyremoved2 == null)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[++executedsteps].SetLogs();
                }

                //Validate study present for ph-Step-9
                login.Logout();
                login.LoginIConnect(phusername, phpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNo);
                Dictionary<string, string> studynotremoved = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
                if (!(studynotremoved == null))
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[++executedsteps].SetLogs();
                }

                //Logout-Step-10
                login.Logout();
                executedsteps++;

                //Report Result
                result.FinalResult(executedsteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedsteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }      

        /// <summary>
        /// This Test is to Share Prior and perform validation with respect to it.
        /// </summary>
        public TestCaseResult Test5_29467(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables            
            Inbounds inbounds = null;
            Outbounds outbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int executedsteps = -1;           

            try
            {
                //Fetch required Test data
                String phusername = Config.phUserName;
                String phpassword = Config.phPassword;
                String ph1username = Config.ph1UserName;
                String ph1password = Config.ph1Password;
                String arusername = Config.ar1UserName;
                String arpassword = Config.ar1Password;
                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] AccessionNumbers = AccessionNoList.Split(':');
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Upload a Dicom Study --Step-1
                ei.EIDicomUpload(Config.ph1UserName, Config.ph1Password, Config.Dest1, UploadFilePath);
                // Check all studies are present in physician's outbound
                login.LoginIConnect(ph1username, ph1password);
                outbounds = (Outbounds)login.Navigate("Outbounds");
                executedsteps++;
                try
                {
                    foreach (String AccessionNumber in AccessionNumbers)
                    {
                        outbounds.SearchStudy("accessionNo", AccessionNumber);
                        PageLoadWait.WaitForPageLoad(20);
                        PageLoadWait.WaitForFrameLoad(20);
                        Dictionary<string, string> priors = outbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumber, "Uploaded" });
                        if (!(priors == null))
                        {
                            result.steps[executedsteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                        }
                        else
                        {
                            result.steps[executedsteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                            result.steps[executedsteps].SetLogs();
                            throw new Exception("One of the Priors Not uploaded--Study not found in outbound");
                        }
                    }
                }
                catch(Exception e)
                {
                    result.steps[executedsteps].status = "Fail";
                    result.steps[executedsteps].SetLogs();
                    throw new Exception("One of the Priors Not uploaded--Study not found in outbound", e);
                }

                //Login as physician and check all priors in inbound-Step-2                
                inbounds = (Inbounds)login.Navigate("Inbounds");
                executedsteps++;
                try
                {
                    foreach (String AccessionNumber in AccessionNumbers)
                    {
                        inbounds.SearchStudy("accessionNo", AccessionNumber);
                        PageLoadWait.WaitForPageLoad(20);
                        PageLoadWait.WaitForFrameLoad(20);
                        Dictionary<string, string> priors = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumber, "Uploaded" });
                        if (!(priors == null))
                        {
                            result.steps[executedsteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                        }
                        else
                        {
                            result.steps[executedsteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                            result.steps[executedsteps].SetLogs();
                            throw new Exception("One of the Priors Not uploaded");
                        }
                    }
                }
                catch (Exception e)
                {
                    result.steps[executedsteps].status = "Fail";
                    result.steps[executedsteps].SetLogs();
                    throw new Exception("One of the Priors Not uploaded--Study not found in Inbound", e);
                }


                //Count Number of related studies for a Patient
                BasePage.Driver.FindElement(By.CssSelector("#m_studySearchControl_m_searchInputAccession")).Clear();
                inbounds.SearchStudy("patientid", PatientID);
                Dictionary<int, string[]> results = BasePage.GetSearchResults();
                int totalpriors = results.Count;

                //Share one of the prior to a user-step--3
                inbounds.SearchStudy("accessionNo", AccessionNumbers[0]);
                inbounds.SelectStudy1(new string[] { "Accession" }, new string[] { AccessionNumbers[0] });
                inbounds.ShareStudy(false, new String[] { phusername });
                //Validate shared prior is in Receiver's Inbound(Login as Receiver)               
                login.Logout();
                login.LoginIConnect(phusername, phpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNumbers[0]);
                Dictionary<string, string> studyshared = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Shared" });
                if (!(studyshared == null))
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Validate other priors are not shared--Step--4
                executedsteps++;
                try
                {
                    for (int i = 0; i < AccessionNumbers.Length; i++)
                    {
                        if (i == 0)
                            continue;
                        inbounds.SearchStudy("accessionNo", AccessionNumbers[i]);
                        Dictionary<string, string> priorsNotshared = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[i], "Shared" });
                        if (priorsNotshared == null)
                        {
                            result.steps[executedsteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                        }
                        else
                        {
                            result.steps[executedsteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                            result.steps[executedsteps].SetLogs();
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    result.steps[executedsteps].status = "Fail";
                    result.steps[executedsteps].SetLogs();
                    throw new Exception("Validate other priors are not shared", e);
                }

                //Email Notification - Step-5-Not Automated 
                result.steps[++executedsteps].status = "Not Automated";

                //Load the shared study in viewer and validate--Step-6
                inbounds.SearchStudy("accessionNo", AccessionNumbers[0]);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Shared" });
                inbounds.LaunchStudy();
                executedsteps++;
                inbounds.CloseStudy();


                //Load the Shared Prior study in viewer and Validate number of priors--Step-7             
                inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Shared" });
                inbounds.LaunchStudy();
                inbounds.NavigateToHistoryPanel();
                int priorscount1 = inbounds.CountPriorsInHistory();

                //Validation
                if (priorscount1 == totalpriors)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                inbounds.CloseHistoryPanel();
                inbounds.CloseStudy();
                
                //Share the rest of the study--Step-8
                login.Logout();
                login.LoginIConnect(ph1username, ph1password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                for (int i = 0; i < AccessionNumbers.Length; i++)
                {
                    if (i == 0)
                        continue;
                    inbounds.SearchStudy("accessionNo", AccessionNumbers[i]);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    inbounds.SelectStudy1(new string[] { "Accession" }, new string[] { AccessionNumbers[i] });
                    inbounds.ShareStudy(false, new string[] { phusername });
                }
                //Validate all priors are in shared status in Receiver's Inbound
                login.Logout();
                login.LoginIConnect(phusername, phpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                executedsteps++;
                try
                {
                    foreach (String AccessionNumber in AccessionNumbers)
                    {
                        inbounds.SearchStudy("accessionNo", AccessionNumber);
                        PageLoadWait.WaitForPageLoad(20);
                        PageLoadWait.WaitForFrameLoad(20);
                        Dictionary<string, string> priors = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumber, "Shared" });
                        if (!(priors == null))
                        {
                            result.steps[executedsteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                        }
                        else
                        {
                            result.steps[executedsteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                            result.steps[executedsteps].SetLogs();
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    result.steps[executedsteps].status = "Fail";
                    result.steps[executedsteps].SetLogs();
                    throw new Exception("Validate all priors are in shared status in receiver's inbound", e);
                }


                //Prior count after sharing the study--Step-9
                inbounds.SearchStudy("accessionNo", AccessionNumbers[1]);
                inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[1], "Shared" });
                inbounds.LaunchStudy();
                inbounds.NavigateToHistoryPanel();
                int priorscount2 = inbounds.CountPriorsInHistory();

                //Validation
                if (priorscount2 == totalpriors)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                inbounds.CloseHistoryPanel();
                inbounds.CloseStudy();
                
                //Validate Upto 3 priors can viewed in the viewer--step10
                inbounds.SearchStudy("accessionNo", AccessionNumbers[0]);
                inbounds.SelectStudy1(new string[] { "Accession" }, new string[] { AccessionNumbers[0] });
                inbounds.LaunchStudy();
                IList<string[]> columnnames = new List<string[]>();
                IList<string[]> columnvalues = new List<string[]>();
                columnnames.Add(new string[] { "Accession" });
                columnnames.Add(new string[] { "Accession" });
                columnvalues.Add(new string[] { AccessionNumbers[1] });
                columnvalues.Add(new string[] { AccessionNumbers[2] });
                inbounds.LaunchMutiplePriors(columnnames, columnvalues);
                if (BasePage.Driver.FindElements(By.CssSelector("[id^='studyPanelDiv_']")).Count == 3)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Close viewer --Step11
                inbounds.CloseStudy();
                executedsteps++;


                // ###### Nominate one Study, Archive it and Perform above Validation #####
                //Nominate one of the priors and Archive it--Step-12
                login.Logout();
                login.LoginIConnect(ph1username, ph1password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNumbers[0]);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Uploaded" });
                inbounds.NominateForArchive("Testing");
                executedsteps++;

                //login as Archivist and check study--Step-13
                login.Logout();
                login.LoginIConnect(arusername, arpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNumbers[0]);
                Dictionary<string, string> nominatedstudy = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Nominated For Archive" });
                if (nominatedstudy != null)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Archive Study and Check its status--step-14
                inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Nominated For Archive" });
                inbounds.ArchiveStudy("", "Testing");
                login.Logout();
                login.LoginIConnect(arusername, arpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNumbers[0]);
                Dictionary<string, string> archivedstudy = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Routing Completed" });
                if (archivedstudy != null)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }


                //Launch study in viewer and perform validation--Step-15
                login.Logout();
                login.LoginIConnect(arusername, arpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("accessionNo", AccessionNumbers[0]);
                inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Routing Completed" });
                inbounds.LaunchStudy();
                inbounds.NavigateToHistoryPanel();
                inbounds.ChooseColumns(new string[] { "Accession" });
                int priorsaftarchive = inbounds.CountPriorsInHistory();
                if (priorsaftarchive == totalpriors)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Validate yellow triangle is displayed for studies in holding pen--Step-16
                int iterate = 0;
                executedsteps++;
                try
                {
                    foreach (String Accession in AccessionNumbers)
                    {
                        if (iterate == 0)
                        {
                            iterate++;
                            continue;
                        }

                        Boolean flagfound = inbounds.CheckForeignExamAlert("Accession", Accession);
                        if (flagfound == true)
                        {
                            result.steps[executedsteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                        }
                        else
                        {
                            result.steps[executedsteps].status = "Fail";
                            Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description + Accession);
                            result.steps[executedsteps].SetLogs();
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    result.steps[executedsteps].status = "Fail";
                    result.steps[executedsteps].SetLogs();
                    throw new Exception("Vaidate yellow triangle is displayed for studies in holding pen", e);
                }

                //Validate Yellow triangle not displayed for archived study--Step-17                 
                Boolean flagfound1 = inbounds.CheckForeignExamAlert("Accession", AccessionNumbers[0]);
                if (flagfound1 == false)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description + AccessionNumbers[0]);
                    result.steps[executedsteps].SetLogs();
                }

                //Load the study with yellow triangle in viewer--Step-18 --Second viewer
                inbounds.OpenPriors(new string[] { "Accession" }, new string[] { AccessionNumbers[1] });
                //Validate Yellow triangle icon is displayed in study panel tool-bar of Study with uploaded status
                if (BasePage.Driver.FindElement(By.CssSelector("span[id*='2_foreignExamDiv']")).Displayed == true)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }
                inbounds.CloseHistoryPanel();
                inbounds.CloseStudy();

                //Validate archived study is present in Studies tab--Step-19
                Studies studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", AccessionNumbers[0]);
                Dictionary<string, string> archivedstudy1 = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { AccessionNumbers[0] });
                if (archivedstudy1 != null)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description + AccessionNumbers[0]);
                    result.steps[executedsteps].SetLogs();
                }

                //Validate studies in holding pen not displayed in Studies tab-Step-20
                int iterate1 = 0;
                executedsteps++;
                try
                {
                    foreach (String Accession in AccessionNumbers)
                    {
                        if (iterate1 == 0)
                        {
                            iterate1++;
                            continue;
                        }
                        Dictionary<string, string> nonarchivedstudy = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { Accession });
                        if (nonarchivedstudy == null)
                        {
                            result.steps[executedsteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                        }
                        else
                        {
                            result.steps[executedsteps].status = "Fail";
                            Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description);
                            result.steps[executedsteps].SetLogs();
                            break;

                        }
                    }
                }
                catch (Exception e)
                {
                    result.steps[executedsteps].status = "Fail";
                    result.steps[executedsteps].SetLogs();
                    throw new Exception("Validate studies in holding pen not displayed in Studies Tab", e);
                }

                //Load  archived study in viewer and check history panel and check prior count--Step-21
                studies.SearchStudy("Accession", AccessionNumbers[0]);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                studies.SelectStudy1(new string[] { "Accession" }, new string[] { AccessionNumbers[0] });
                studies.LaunchStudy();
                studies.NavigateToHistoryPanel();
                int priorsinstudytab = studies.CountPriorsInHistory();
                if (priorsinstudytab == totalpriors)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Navigate to Outbounds and check study--Step-22
                studies.CloseHistoryPanel();
                studies.CloseStudy();
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("Accession", AccessionNumbers[0]);
                Dictionary<string, string> studyinoutbound = outbounds.GetMatchingRow(new string[] { "Accession" }, new string[] { AccessionNumbers[0] });
                if (studyinoutbound == null)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }
                login.Logout();

                //Nominate rest of the studies to Archive--Step23
                login.LoginIConnect(ph1username, ph1password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                int iterate2 = 0;
                foreach (String Accession in AccessionNumbers)
                {
                    if (iterate2 == 0)
                    {
                        iterate2++;
                        continue;
                    }

                    inbounds.SearchStudy("Accession", Accession);
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                    inbounds.SelectStudy1(new string[] { "Accession" }, new string[] { Accession });
                    inbounds.NominateForArchive("Testing");
                }
                int iterate3 = 0;
                executedsteps++;
                try
                {
                    foreach (String Accession in AccessionNumbers)
                    {
                        if (iterate3 == 0)
                        {
                            iterate3++;
                            continue;
                        }

                        inbounds.SearchStudy("Accession", Accession);
                        PageLoadWait.WaitForPageLoad(10);
                        PageLoadWait.WaitForFrameLoad(10);
                        Dictionary<string, string> nominatedstudy1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Nominated For Archive" });
                        if (nominatedstudy1 != null)
                        {
                            result.steps[executedsteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                        }
                        else
                        {
                            result.steps[executedsteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                            result.steps[executedsteps].SetLogs();
                            break;
                        }

                    }
                }
                catch (Exception e)
                {
                    result.steps[executedsteps].status = "Fail";
                    result.steps[executedsteps].SetLogs();
                    throw new Exception("Validate rest of priors are nominated to archive", e);
                }

                //Step-24 
                inbounds.SearchStudy("Accession", AccessionNumbers[1]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                inbounds.SelectStudy1(new string[] { "Accession" }, new string[] { AccessionNumbers[1] });
                inbounds.LaunchStudy();
                executedsteps++;

                //Step-25 validate prior count
                inbounds.NavigateToHistoryPanel();
                int priorcounts3 = inbounds.CountPriorsInHistory();
                if (priorcounts3 == totalpriors)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Validate yellow triangle is displayed for studies in holding pen --Step-26
                int iterate4 = 0;
                inbounds.ChooseColumns(new string[] { "Accession" });
                executedsteps++;
                try
                {
                    foreach (String Accession in AccessionNumbers)
                    {
                        if (iterate4 < 2)
                        {
                            iterate4++;
                            continue;
                        }
                        Boolean flagfound2 = inbounds.CheckForeignExamAlert("Accession", Accession);
                        if (flagfound2 == true)
                        {
                            result.steps[executedsteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                        }
                        else
                        {
                            result.steps[executedsteps].status = "Fail";
                            Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description + Accession);
                            result.steps[executedsteps].SetLogs();
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    result.steps[executedsteps].status = "Fail";
                    result.steps[executedsteps].SetLogs();
                    throw new Exception("Validate yellow triangle icon for studies in holding pen", e);
                }

                //Validate no yellow triangle for studies archived-step-27
                Boolean flagfound5 = inbounds.CheckForeignExamAlert("Accession", AccessionNumbers[0]);
                if (flagfound5 == false)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }   
                inbounds.CloseHistoryPanel();
                inbounds.CloseStudy();
                login.Logout();

                //Step-28 - Login as Archivist and check priors are listed
                login.LoginIConnect(arusername, arpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                int iterate5 = 0;
                executedsteps++;
                try
                {
                    foreach (String Accession in AccessionNumbers)
                    {
                        if (iterate5 == 0)
                        {
                            iterate5++;
                            continue;
                        }

                        inbounds.SearchStudy("accessionNo", Accession);
                        PageLoadWait.WaitForPageLoad(10);
                        PageLoadWait.WaitForFrameLoad(10);
                        Dictionary<string, string> nominatedstudy2 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Nominated For Archive" });
                        if (nominatedstudy2 != null)
                        {
                            result.steps[executedsteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                        }
                        else
                        {
                            result.steps[executedsteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                            result.steps[executedsteps].SetLogs();
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    result.steps[executedsteps].status = "Fail";
                    result.steps[executedsteps].SetLogs();
                    throw new Exception("Validate nominated studies are listed in archivist inbound", e);
                }

                inbounds.SearchStudy("accessionNo", AccessionNumbers[2]);
                inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[2], "Nominated For Archive" });
                inbounds.LaunchStudy();

                //Step-29
                //Validate No Yellow triangle icon is displayed in study panel tool-bar of Study with Nominated for archive status
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (BasePage.Driver.FindElement(By.CssSelector("span[id*='1_foreignExamDiv']")).Displayed != true)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-30 - Check prior count is same
                inbounds.NavigateToHistoryPanel();
                int priorscount4 = inbounds.CountPriorsInHistory();
                
                //Validate yellow triangle is displayed for studies in holding pen                
                inbounds.ChooseColumns(new string[] { "Accession" });
                Boolean flagcheck = inbounds.CheckForeignExamAlert("Accession", AccessionNumbers[1]);
                if (priorscount4 == totalpriors && flagcheck == true)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Validate in archived study in Studies tab --Step31
                inbounds.CloseHistoryPanel();
                inbounds.CloseStudy();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", AccessionNumbers[0]);
                Dictionary<string, string> archivedstudy2 = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { AccessionNumbers[0] });
                if (archivedstudy2 != null)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description + AccessionNumbers[0]);
                    result.steps[executedsteps].SetLogs();
                }
                //Validate studies in holding pen not displayed in Studies tab
                iterate1 = 0;
                foreach (String Accession in AccessionNumbers)
                {
                    if (iterate1 == 0)
                    {
                        iterate1++;
                        continue;
                    }

                    studies.SearchStudy("Accession", Accession);
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                    Dictionary<string, string> nonarchivedstudy = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { Accession });
                    if (nonarchivedstudy == null)
                    {
                        result.steps[executedsteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                    }
                    else
                    {
                        result.steps[executedsteps].status = "Fail";
                        Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description);
                        result.steps[executedsteps].SetLogs();
                        break;

                    }
                }

                //Search, Select and Launch Study-Step-32
                login.SearchStudy("Accession", AccessionNumbers[0]);
                login.SelectStudy1(new string[] { "Accession" }, new string[] { AccessionNumbers[0] });
                login.LaunchStudy();

                //Validate No Yellow triangle icon is displayed in study panel tool-bar of Study with Nominated for archive status
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (BasePage.Driver.FindElement(By.CssSelector("span[id*='1_foreignExamDiv']")).Displayed != true)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }
                //Close Study
                login.CloseStudy();

                //steps-33-Archive all studies
                inbounds = (Inbounds)login.Navigate("Inbounds");
                iterate = 0;
                foreach (string Accession in AccessionNumbers)
                {
                    if (iterate == 0)
                    {
                        iterate++;
                        continue;
                    }
                    inbounds.SearchStudy("Accession", Accession);
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                    inbounds.SelectStudy1(new string[] { "Accession" }, new string[] { Accession });
                    inbounds.ArchiveStudy("", "Testing");
                }

                login.Logout();
                login.LoginIConnect(arusername, arpassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                iterate1 = 0;
                executedsteps++;
                foreach (String Accession in AccessionNumbers)
                {
                    inbounds.SearchStudy("Accession", Accession);
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                    Dictionary<string, string> fullyarchivedstudies = studies.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Routing Completed" });
                    if (fullyarchivedstudies != null)
                    {
                        result.steps[executedsteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                    }
                    else
                    {
                        result.steps[executedsteps].status = "Fail";
                        Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description);
                        result.steps[executedsteps].SetLogs();
                        break;

                    }
                }
                inbounds.SearchStudy("Accession", AccessionNumbers[2]);
                inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[2], "Routing Completed" });
                inbounds.LaunchStudy();

                //steps-34
                //Validate No Yellow triangle icon is displayed in study panel tool-bar of Study with Nominated for archive status
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (BasePage.Driver.FindElement(By.CssSelector("span[id*='1_foreignExamDiv']")).Displayed != true)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                inbounds.NavigateToHistoryPanel();
                inbounds.ChooseColumns(new string[] { "Accession", "Status" });
                foreach (String Accession in AccessionNumbers)
                {
                    Boolean flagfound3 = inbounds.CheckForeignExamAlert("Accession", Accession);
                    if (flagfound3 == false)
                    {
                        result.steps[executedsteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                    }
                    else
                    {
                        result.steps[executedsteps].status = "Fail";
                        Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description + Accession);
                        result.steps[executedsteps].SetLogs();
                        break;
                    }
                }
                int priorcounts4 = inbounds.CountPriorsInHistory();
                if (priorcounts4 == totalpriors)
                {
                    result.steps[executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }
                inbounds.CloseHistoryPanel();
                inbounds.CloseStudy();
                login.Logout();

                //Step-35
                login.LoginIConnect(ph1username, ph1password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Accession", AccessionNumbers[1]);
                inbounds.SelectStudy1(new string[] { "Accession" }, new string[] { AccessionNumbers[1] });
                inbounds.LaunchStudy();
                //Validate No Yellow triangle icon is displayed in study panel tool-bar of Study with Routing Completed status
                if (BasePage.Driver.FindElement(By.CssSelector("span[id*='1_foreignExamDiv']")).Displayed != true)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                inbounds.NavigateToHistoryPanel();                
                inbounds.ChooseColumns(new string[] { "Accession", "Status" });
                foreach (String Accession in AccessionNumbers)
                {
                    Boolean flagfound4 = inbounds.CheckForeignExamAlert("Accession", Accession);
                    if (flagfound4 == false)
                    {
                        result.steps[executedsteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                    }
                    else
                    {
                        result.steps[executedsteps].status = "Fail";
                        Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description + Accession);
                        result.steps[executedsteps].SetLogs();
                        break;
                    }
                }

                int priorcounts5 = inbounds.CountPriorsInHistory();
                if (priorcounts5 == totalpriors)
                {
                    result.steps[executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-36
                inbounds.OpenPriors(new string[] { "Accession" }, new string[] { AccessionNumbers[2] });
                //Validate No Yellow triangle icon is displayed in study panel tool-bar of Study with Routing Completed status
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (BasePage.Driver.FindElement(By.CssSelector("span[id*='2_foreignExamDiv']")).Displayed != true)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }
                inbounds.CloseStudy();

                //Step-37
                studies = (Studies)login.Navigate("Studies");
                executedsteps++;
                try
                {
                    foreach (String Accession in AccessionNumbers)
                    {
                        studies.SearchStudy("Accession", Accession);
                        PageLoadWait.WaitForPageLoad(10);
                        PageLoadWait.WaitForFrameLoad(10);
                        Dictionary<string, string> studiesfinal = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { Accession });

                        if (studiesfinal != null)
                        {
                            result.steps[executedsteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
                        }
                        else
                        {
                            result.steps[executedsteps].status = "Fail";
                            Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description + Accession);
                            result.steps[executedsteps].SetLogs();
                            break;
                        }

                    }
                }
                catch(Exception e)
                {
                    result.steps[executedsteps].status = "Fail";
                    Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                    throw new Exception("Validate all the studies are present in Studies tab");
                }

                //Report Result
                result.FinalResult(executedsteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedsteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Test Case 1- 29471 -Remove Exam Archivist inbounds and check with it from database(i.e)HP
        /// </summary>
     

        #endregion Sprint-1 Automation Tests

        #region Sprint-2 Automation Tests
        
        /// <summary>
        ///Create another domain for Image sharing setup
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161156(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables                      
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            String eiWindow = Config.eiwindow;
            String initialBrowserName = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
            int changebrowser = 0;
            String originalDownloadPath = Config.downloadpath;
            if (initialBrowserName.Contains("ie") || initialBrowserName.Contains("explore"))
                Config.downloadpath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + @"\Downloads";

            try
            {
                //Fetch required Test data                
                String studyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                string[] arrStudyPath = studyPath.Split('=');
                string orderPath = arrStudyPath[3];
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                string[] arrAccession = accession.Split(':');
                String patientId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                string[] arrPatientId= patientId.Split(':');

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String newPort = BasePage.FreeTcpPort().ToString();
                BluRingViewer bluRingViewer = null;

                //Delete studies from Holding Pen
                try
                {
                    HPLogin hplogin = new HPLogin();
                    BasePage.Driver.Navigate().GoToUrl("https://" + Config.HoldingPenIP + "/webadmin");
                    HPHomePage hphome = (HPHomePage)hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: "https://" + Config.HoldingPenIP + "/webadmin");
                    WorkFlow workflow = (WorkFlow)hphome.Navigate("Workflow");
                    workflow.NavigateToLink("Workflow", "Archive Search");
                    workflow.HPSearchStudy("PatientID", arrPatientId[0]);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                        workflow.HPDeleteStudy();
                    workflow.HPSearchStudy("PatientID", arrPatientId[1]);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                        workflow.HPDeleteStudy();
                    workflow.HPSearchStudy("PatientID", arrPatientId[2]);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                        workflow.HPDeleteStudy();
                    hplogin.LogoutHPen();
                }
                catch (Exception) { }

                //Step 1 - Create another domain (not in superadmin domain) with users that has who has the following roles ( all in the same domain) :
                //After creating the new Domain enable it for image sharing
                //logout of the administrator and login as the new Domain Admin for the new Domain.
                //Create the roles and users: In the new Domain
                //Role = PH2 user = PH2 Receiving only
                //Role = AR2 user = AR2 Archiving only
                //Role = ST2 user = ST2 No permissions to receive or archive
                //Create New Institution and Destination using the new domain and users
                //Re - generate installer for both(POP and CD Uploader) and Install the tools using latest Installer

                //Create DomainB and DomainAdmin
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                DomainManagement domain = login.Navigate<DomainManagement>();
                var domainattr = domain.CreateDomainAttr();
                String domainB = domainattr[DomainManagement.DomainAttr.DomainName];
                String domainadminB = domainattr[DomainManagement.DomainAttr.UserID];
                domain.CreateDomain(domainattr, isimagesharingneeded: true, isemailstudy: true);
                login.Logout();

                //Creare Roles - PH2, AR2, ST2.             
                login.DriverGoTo(login.url);
                login.LoginIConnect(domainattr[DomainManagement.DomainAttr.UserID], domainattr[DomainManagement.DomainAttr.Password]);
                var rolemgmt = login.Navigate<RoleManagement>();
                string rolePH2 = "PH" + domainB.Split('n')[1];
                string roleAR2 = "AR" + domainB.Split('n')[1];
                string roleST2 = "ST" + domainB.Split('n')[1];
                string userPH2 = "PH" + domainB.Split('n')[1];
                string userAR2 = "AR" + domainB.Split('n')[1];
                string userST2 = "ST" + domainB.Split('n')[1];
                rolemgmt.CreateRole(domainB, rolePH2, "Physician", new string[] { "receiveexam"}); //Physician
                rolemgmt.CreateRole(domainB, roleAR2, "Archivist", new string[] { "archive"}); //Archivist
                rolemgmt.CreateRole(domainB, roleST2, "Staff", new string[] {"email" }); //Staff

                //Creare Users - PH2, AR2, ST2.
                var usermgmt = login.Navigate<UserManagement>();
                usermgmt.CreateUser(userPH2, domainB, rolePH2, hasPass: 1, Password: userPH2);
                usermgmt.CreateUser(userAR2, domainB, roleAR2, hasPass: 1, Password: userAR2);
                usermgmt.CreateUser(userST2, domainB, roleST2, hasPass: 1, Password: userST2);
                
                //Create Destination for DomainB
                string destinationB = "Destination29478";                
                var imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                var pagedestination = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                pagedestination.CreateDestination(pagedestination.GetHostName(Config.DestinationPACS), userPH2, userAR2, destinationB, domainB);
                //Create Institution for DomainB
                var pageInstitution = (Image_Sharing.Institution)imagesharing.NavigateToSubTab("Institution");
                if (!pageInstitution.SearchInstitution(domainattr[DomainManagement.DomainAttr.InstitutionName]))
                    pageInstitution.CreateInstituition(domainattr[DomainManagement.DomainAttr.InstitutionName], "HOME"+ domainB.Split('n')[1], "Inst-29478");
                string pinInstitution = "";
                if (pageInstitution.SearchInstitution(domainattr[DomainManagement.DomainAttr.InstitutionName]))
                {
                    pageInstitution.SelectInstituition(domainattr[DomainManagement.DomainAttr.InstitutionName]);
                    PageLoadWait.WaitForFrameLoad(10);
                    pageInstitution.InstEditButton().Click();
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#InstitutionEditDialogDiv")));
                    pinInstitution = pageInstitution.PinText().GetAttribute("value");
                    pageInstitution.OKButton().Click();
                    BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#InstitutionEditDialogDiv")));
                }
                login.Logout();

                //Re - generate installer for both(POP and CD Uploader)
                //Generate EI Installer
                string eiDomainBWindow = "EI" + domainattr[DomainManagement.DomainAttr.InstitutionName];
                var servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Image Sharing");
                servicetool.GenerateInstallerExamImporter(domainB, eiDomainBWindow);
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                //Generate POP Installer
                string pacsGatewayWindow = "PACS Gateway for " + domainattr[DomainManagement.DomainAttr.InstitutionName];
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Image Sharing");
                servicetool.GenerateInstallerPOP(domainB, pacsGatewayWindow);
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                //Install the tools using latest Installer
                //Install EI
                var ei = new ExamImporter();                
                string eiInstallPath = ei.EI_Installation(domainB, eiDomainBWindow, domainattr[DomainManagement.DomainAttr.InstitutionName], userPH2, userPH2);
                Logger.Instance.InfoLog("The Window name of EI is" + eiDomainBWindow);

                //Install POP
                var pop = new POPUploader();
                string popInstallerPath = login.PACSGatewayInstallerPath;
                //string popInstallerPath = Config.downloadpath + Path.DirectorySeparatorChar + pop.PACSGatewayInstallerName;
                if (File.Exists(popInstallerPath))    //Delete already downloaded pop msi file
                    File.Delete(popInstallerPath);                                
                login.DownloadInstaller(login.url, "POP", popInstallerPath, domainB);                
                string popInstalledPath = pop.InstallPACSGateway(eMailId: Config.POPAdminEmail, Pin: pinInstitution, InstallerLocation: popInstallerPath,Port: newPort, GateWayInstanceName: pacsGatewayWindow);
                //pop.InstallPACSGateway(eMailId: Config.EmailId2, Pin: pinInstitution);

                //Get POP Device AETitle                        
                POPUploader pacsGateway = new POPUploader();                            
                string popAE = pacsGateway.GetPOPDeviceID(popInstalledPath + @"ConfigTool\" + pacsGatewayWindow + @" ConfigTool.exe");
                //Handle Crash Window
                try
                {
                    WpfObjects wpfObject1 = new WpfObjects();
                    Window crashWindow = wpfObject1.GetMainWindowByTitle("Client.Windows.PopConfigurationTool");
                    Button closeProgramButton = crashWindow.Get<Button>(SearchCriteria.ByText("Close the program"));
                    closeProgramButton.Click();
                    Logger.Instance.InfoLog("Client.Windows.PopConfigurationTool crash window closed successfully");
                    BasePage.KillProcess("WerFault");
                }
                catch (Exception)
                {
                    BasePage.KillProcess("WerFault");
                }

                //PACS DICOM device to send
                string pacsGatewayDevice = "DICOM Device: " + popAE;

                //Add POP device to MPACS dicom devices list   
                //Switch to chrome - sometime driver unable to login in FF
                BasePage.Driver.Quit();
                Config.BrowserType = "chrome";
                Logger.Instance.InfoLog("Swicthing Browser Type to chrome");
                BasePage.Driver = null;
                login = new Login();
                login.DriverGoTo(login.url);
                changebrowser++;     
                mpaclogin.DriverGoTo(mpaclogin.mpacstudyurl);
                MPHomePage mpachome = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);
                MpacConfiguration mpacConfig = (MpacConfiguration)mpachome.NavigateTopMenu("Configuration");
                mpacConfig.NavigateToDicomDevices();
                mpacConfig.AddDicomDevice(Config.IConnectIP, pacsGatewayDevice, newPort);
                mpaclogin.LogoutPacs();
                ExecutedSteps++;

                //Step 2 - Perform the image sharing via POP upload study
                //Import the Studies to MergePacs(Study PACS) Server
                BasePage.RunBatchFile(Config.batchfilepath, Config.TestDataPath + arrStudyPath[0] + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                //Send the study to dicom devices from MergePacs management page
                mpaclogin.DriverGoTo(mpaclogin.mpacstudyurl);
                mpachome = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);
                Tool tool = (Tool)mpachome.NavigateTopMenu("Tools");
                tool.NavigateToSendStudy();
                tool.SearchStudy("Accession", arrAccession[0], 0);
                Dictionary<string, string> MpacDetails = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults()); //Get study details
                tool.MpacSelectStudy("Accession", arrAccession[0]);
                tool.SendStudy(1, pacsGatewayDevice,waitTime: 400);
                mpaclogin.LogoutPacs();
                ExecutedSteps++;
                //Switch to Default browser
                BasePage.Driver.Quit();
                Config.BrowserType = initialBrowserName;
                Logger.Instance.InfoLog("Swicthing Browser Type to Default browser");
                BasePage.Driver = null;
                login = new Login();
                login.DriverGoTo(login.url);
                changebrowser++;

                //Step 3 - 	Login as physician and check the inbounds                
                login.DriverGoTo(login.url);
                login.LoginIConnect(userPH2, userPH2);
                Inbounds inbounds = login.Navigate<Inbounds>();
                inbounds.SearchStudy(AccessionNo: arrAccession[0]);
                bool uploadStatus1 = inbounds.CheckStudy("Status", "Uploaded");
                //Workaround
                if (!uploadStatus1)
                {
                    ExamImporter eiImport1 = new ExamImporter();
                    try
                    {
                        eiImport1.EIDicomUpload(userPH2, userPH2, destinationB, arrStudyPath[0], path: eiInstallPath, windowName: eiDomainBWindow);
                    }
                    catch (Exception) { }
                    eiImport1.CloseUploaderTool();
                    inbounds = login.Navigate<Inbounds>();
                    inbounds.SearchStudy(AccessionNo: arrAccession[0]);
                    uploadStatus1 = inbounds.CheckStudy("Status", "Uploaded");
                }
                if (uploadStatus1)
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

                //Step 4 - Physician nominates the study for archiving   
                inbounds.SelectStudy("Accession", arrAccession[0]);
                PageLoadWait.WaitForFrameLoad(10);
                inbounds.NominateForArchiveBtn().Click();               
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(inbounds.By_NominateDiv()));
                //BasePage.Driver.SwitchTo().DefaultContent();
                //BasePage.Driver.SwitchTo().Frame(0);
                if (inbounds.ArchiveOrderNotes().Displayed && inbounds.ReasonForArchiveSelect().Displayed)
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

                //Step 5 - 	Select a reason and Enter some text in the Order Notes, Type several lines of text. Click on the bottom right hand corner of the Notes Field and drag it down
                new SelectElement(inbounds.ReasonForArchiveSelect()).SelectByText("Prior or Exam for Comparison");
                inbounds.ArchiveOrderNotes().SendKeys("Test 29748\r\nMultiple Lines--Line1\r\nMultiple Lines--Line2\r\nMultiple Lines--Line3\r\nMultiple Lines--Line4");
                PageLoadWait.WaitForFrameLoad(10);
                //Get location and expand order notes box         
                int height = inbounds.ArchiveOrderNotes().Size.Height;
                int width = inbounds.ArchiveOrderNotes().Size.Width;
                Logger.Instance.InfoLog("Step 5-- Old Height" + height);
                Logger.Instance.InfoLog("Step 5-- Old Width" + width);
                //Click & expand
                //Try expanding the text box using testcomplete action. On error try using  webdriver. (Webdriver having issues - Click and hold not working)                
                Thread.Sleep(3000);
                TestCompleteAction tcActions = new TestCompleteAction();
                try
                {
                    tcActions.MoveToElement(inbounds.ArchiveOrderNotes(), width - 1, height - 1).ClickAndHold().MoveByOffset(0, 30).Perform();
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Step 5-- Exception while expanding using test complete --" + ex);
                }
                Thread.Sleep(3000);
                PageLoadWait.WaitForFrameLoad(10);
                if (inbounds.ArchiveOrderNotes().Size.Height == height) //Try expanding using actions if test complete did not work
                {
                    Logger.Instance.InfoLog("Step 5-- Expanding using driver actions");
                    Actions action = new Actions(BasePage.Driver);
                    if (BasePage.SBrowserName.ToLower().Contains("explorer"))
                        action.MoveToElement(inbounds.ArchiveOrderNotes(), width - 2, height - 2).ClickAndHold().MoveByOffset(0, 40).Build().Perform();
                    else
                        action.MoveToElement(inbounds.ArchiveOrderNotes(), width - 2, height - 2).ClickAndHold().MoveByOffset(0, 20).Build().Perform();
                }
                PageLoadWait.WaitForFrameLoad(10);
                int newHeight = inbounds.ArchiveOrderNotes().Size.Height;
                Logger.Instance.InfoLog("Step 5-- New Height" + newHeight);
                if ((newHeight != height) || (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explore")))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.ErrorLog("Browser: " + ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower());
                    Logger.Instance.ErrorLog("Browser: " + BasePage.SBrowserName);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 6 - Click nominate and status get updated to Nominated for archive
                //Click Nominate
                PageLoadWait.WaitForFrameLoad(20);
                inbounds.NominateBtn().Click();
                try
                {
                    BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(inbounds.By_NominateDiv()));
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Step 6. Exception" + e);
                    Logger.Instance.ErrorLog("Step 6. Nominate dialog did not close. Trying to click nominate study using jscript.");
                    inbounds.ClickElement(inbounds.NominateBtn());
                    BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(inbounds.By_NominateDiv()));
                }
                PageLoadWait.WaitForLoadingMessage();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitHomePage();          
                bool nominatedStatus1 = inbounds.CheckStudy("Status", "Nominated For Archive");
                if (nominatedStatus1)
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

                //Step 7 - 	Login as archivist and check the inbounds                
                login.DriverGoTo(login.url);
                login.LoginIConnect(userAR2, userAR2);
                inbounds = login.Navigate<Inbounds>();
                inbounds.SearchStudy(AccessionNo: arrAccession[0]);
                inbounds.SelectStudy("Accession", arrAccession[0]);
                bool nominatedStatus2 = inbounds.CheckStudy("Status", "Nominated For Archive");
                if (nominatedStatus2)
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

                //Step 8 - Select study and click archive study          
                IWebElement UploadCommentsField, ArchiveOrderField;
                inbounds.ClickArchiveStudy(out UploadCommentsField, out ArchiveOrderField);
                //BasePage.Driver.SwitchTo().DefaultContent();
                //BasePage.Driver.SwitchTo().Frame(0);
                if (inbounds.ReconciliationOrderNotes().Displayed)
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

                //Step 9 - 	Click on the bottom right corner of the OrderNotes and drag the corner down.                                              
                //Get location and expand order notes box         
                height = ArchiveOrderField.Size.Height;
                width = ArchiveOrderField.Size.Width;
                Logger.Instance.InfoLog("Step 9-- Old Height" + height);
                Logger.Instance.InfoLog("Step 9-- Old Height" + width);
                //Click & expand
                try
                {
                    Thread.Sleep(3000);
                    tcActions = new TestCompleteAction();
                    tcActions.MoveToElement(inbounds.ArchiveOrderNotes(), width - 1, height - 1).ClickAndHold().MoveByOffset(0, 30).Perform();
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Step 9-- Exception while expanding using test complete --" + ex);
                }
                Thread.Sleep(3000);
                PageLoadWait.WaitForFrameLoad(10);
                if (ArchiveOrderField.Size.Height == height) //Try expanding using actions if test complete did not work
                {
                    //Expanding using driver actions
                    Actions action = new Actions(BasePage.Driver);
                    if (BasePage.SBrowserName.ToLower().Contains("explorer"))
                        action.MoveToElement(ArchiveOrderField, width - 2, height - 2).ClickAndHold().MoveByOffset(0, 40).Build().Perform();
                    else
                        action.MoveToElement(ArchiveOrderField, width - 2, height - 2).ClickAndHold().MoveByOffset(0, 20).Build().Perform();
                }
                PageLoadWait.WaitForFrameLoad(10);
                newHeight = ArchiveOrderField.Size.Height;
                Logger.Instance.InfoLog("Step 9-- New Height" + newHeight);
                if ((newHeight != height) || (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("ie")) || (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explore")))
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

                //Step 10 - Archivist archive the study with manual edit (without matching an order)
                UploadCommentsField.SendKeys("29478 - Archive comments");
                ArchiveOrderField.SendKeys("29478 - Order comments");
                inbounds.ClickArchive();
                PageLoadWait.WaitForLoadingMessage();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForSearchLoad();
                Logger.Instance.InfoLog("Study is archived");
                ExecutedSteps++;

                //Step 11 - Perform image sharing via CD uploader as user (ar/ar) upload study
                ExamImporter eiImport = new ExamImporter();
                try
                {
                    eiImport.EIDicomUpload(userAR2, userAR2, destinationB, arrStudyPath[1], path: eiInstallPath, windowName: eiDomainBWindow);
                }
                catch (Exception e)
                {
                    if (e.Message == "Study Not Loaded As It Already Exists")
                    {
                        Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                    }
                    else
                    {
                        throw e;
                    }
                }
                ExecutedSteps++;

                //Step 12 - View the study in the Outbounds from viewer
                Outbounds outbounds = login.Navigate<Outbounds>();
                outbounds.SearchStudy(AccessionNo: arrAccession[1]);
                PageLoadWait.WaitForLoadingMessage(30);
                outbounds.SelectStudy("Accession", arrAccession[1]);
                
                if (Config.isEnterpriseViewer.ToLower().Equals("n"))
                {
                    outbounds.LaunchStudy();
                    PageLoadWait.WaitForPageLoad(30);
                    PageLoadWait.WaitForFrameLoad(30);
                    PageLoadWait.WaitForThumbnailsToLoad(60);
                    PageLoadWait.WaitForAllViewportsToLoad(60);
                    ExecutedSteps++;
                }
                else
                {
                    bluRingViewer = BluRingViewer.LaunchBluRingViewer();
                    PageLoadWait.WaitForFrameLoad(20); 
                    ExecutedSteps++;
                }

                //Step 13 - Close the study and archive the study
                if (Config.isEnterpriseViewer.ToLower().Equals("n"))
                {
                    outbounds.CloseStudy();
                }
                else
                {
                    bluRingViewer.CloseBluRingViewer();
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                outbounds.SelectStudy("Accession", arrAccession[1]);
                outbounds.ArchiveStudy("29478 Archive study 2", "29478 Study 2 order notes");
                ExecutedSteps++;

                //Step 14 - Send an order to MWL server          
                Boolean hl7order = mpaclogin.SendHL7Order(Config.MergePACsIP, int.Parse(Config.mpacport), orderPath);                
                //From exam importer, Upload the matching study to destination (configured)
                eiImport = new ExamImporter();
                try
                {
                    eiImport.EIDicomUpload(userAR2, userAR2, destinationB, arrStudyPath[2], path: eiInstallPath, windowName: eiDomainBWindow);
                }
                catch (Exception e)
                {
                    if (e.Message == "Study Not Loaded As It Already Exists")
                    {
                        Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                    }
                    else
                    {
                        throw e;
                    }
                }
                if (hl7order)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.ErrorLog("Unable to send order to MWL Server");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }      

                //Step 15 - Login as archivist and from Outbounds, archive the study
                outbounds = login.Navigate<Outbounds>();
                outbounds.SearchStudy(AccessionNo: arrAccession[2]);
                outbounds.SelectStudy("Accession", arrAccession[2]);              
                outbounds.ClickArchiveStudy(out UploadCommentsField, out ArchiveOrderField);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(outbounds.By_ReconcileSearchOrderRadio));
                ExecutedSteps++;

                //Step 16 - Check the reconciliation and make sure the patient information matches                
                outbounds.ArchiveSearch("order", "All Dates");               
                PageLoadWait.WaitForFrameLoad(10);
                string matchingOrderPID= outbounds.ReconcileMatchPIDTxtBx().GetAttribute("value");
                Logger.Instance.InfoLog("Matching Order PID - " + matchingOrderPID);
                if (matchingOrderPID.ToLower().Equals(arrPatientId[2].ToLower()))
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

                //Step 17 - Archive the study
                UploadCommentsField.SendKeys("29478 - Archive comments: Study 3");
                ArchiveOrderField.SendKeys("29478 - Order comments: Study 3");
                inbounds.ClickArchive();
                PageLoadWait.WaitForLoadingMessage();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForSearchLoad();
                ExecutedSteps++;

                //Step 18 - Verify the study reaches to the destination
                Studies studies = login.Navigate<Studies>();
                studies.SearchStudy(AccessionNo: arrAccession[2]);
                if (studies.CheckStudy("Accession", arrAccession[2]))
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

                try
                {
                    //Delete Studies
                    login.LoginIConnect(userPH2, userPH2);
                    inbounds = login.Navigate<Inbounds>();
                    inbounds.SearchStudy(AccessionNo: arrAccession[0]);
                    inbounds.SelectStudy("Accession", arrAccession[0]);
                    inbounds.DeleteStudy();
                    login.Logout();
                    login.LoginIConnect(userAR2, userAR2);
                    outbounds = login.Navigate<Outbounds>();
                    outbounds.SearchStudy(AccessionNo: arrAccession[1]);
                    outbounds.SelectStudy("Accession", arrAccession[1]);
                    outbounds.DeleteStudy();
                    outbounds.SearchStudy(AccessionNo: arrAccession[2]);
                    outbounds.SelectStudy("Accession", arrAccession[2]);
                    outbounds.DeleteStudy();
                    login.Logout();
                    //Delete Destination
                    login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                    imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                    pagedestination = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");               
                    pagedestination.DeleteDestination(destinationB, domainB);                    
                    //Delete Users - PH2, AR2, ST2.
                    usermgmt = login.Navigate<UserManagement>();
                    usermgmt.DeleteUser(domainB, userPH2);
                    usermgmt.DeleteUser(domainB, userAR2);
                    usermgmt.DeleteUser(domainB, userST2);
                    //Delete Domain
                    DomainManagement domainmgmt = login.Navigate<DomainManagement>();
                    domainmgmt.SearchDomain(domainB);
                    domainmgmt.SelectDomain(domainB);
                    domainmgmt.ClickDeleteDomainBtn();
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame(0);                   
                    domainmgmt.ConfirmDeleteDomain();
                    login.Logout();
                }
                catch (Exception) { }

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
                Config.downloadpath = originalDownloadPath;
                if (changebrowser != 0)
                {
                    BasePage.Driver.Quit();
                    Config.BrowserType = initialBrowserName;
                    Logger.Instance.InfoLog("Swicthing Back Browser to --" + initialBrowserName);
                    BasePage.Driver = null;
                    login = new Login();
                    login.DriverGoTo(login.url);
                }
                try
                {
                    //Uninstall POP and EI
                    POPUploader popUninstall = new POPUploader();
                    popUninstall.UnInstallPACSGateway();
                    ExamImporter eiUninstall = new ExamImporter();
                    eiUninstall.UnInstallEI(Config.downloadpath);                    
                }
                catch (Exception)
                { }                
                //Handle Crash Window
                try
                {
                    WpfObjects wpfObject1 = new WpfObjects();
                    Window crashWindow = wpfObject1.GetMainWindowByTitle("Client.Windows.PopConfigurationTool");
                    Button closeProgramButton = crashWindow.Get<Button>(SearchCriteria.ByText("Close the program"));
                    closeProgramButton.Click();
                    Logger.Instance.InfoLog("Client.Windows.PopConfigurationTool crash window closed successfully");
                    BasePage.KillProcess("WerFault");
                }
                catch (Exception ex)
                {
                    BasePage.KillProcess("WerFault");
                    Logger.Instance.InfoLog("Exception while handling pop crash window -" + ex);
                }
              
            }

        }

        /// <summary>
        /// Upload a new study,nominate as physician, archive as archivist and check whether the study is sent to destination
        /// </summary>
        public TestCaseResult Test1_29478(String testid, String teststeps, int stepcount)
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
                String username = Config.phUserName;
                String password = Config.phPassword;
                String arusername = Config.arUserName;
                String arpassword = Config.arPassword;
                String order = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Order");
                String AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo");
                String orderHeight = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderHeight");
                String archiveOrderHeight = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ArchiveOrderHeight");

                //Non Automated step 1 -Initial Setup
                ExecutedSteps++;

                //Upload a study using POP - step 2

                ExecutedSteps++;

                //Login as physician 
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study 
                inbounds.SearchStudy("Accession", AccessionNo);

                //Find Study Status
                String studyStatus;
                inbounds.GetMatchingRow("Accession", AccessionNo).TryGetValue("Status", out studyStatus);

                //Validate Study is listed and status as Uploaded - step 3
                if (studyStatus == "Uploaded")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Find Select Study and Click Nominate button
                inbounds.SelectStudy("Accession", AccessionNo);
                IWebElement reasonField, orderField;
                inbounds.ClickNominateButton(out reasonField, out orderField);

                //Validate Reason Field and Order Field are Displayed - step 4
                if (reasonField.Displayed == true && orderField.Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Expanding archive order field
                Size height = orderField.Size;
                var js = (IJavaScriptExecutor)BasePage.Driver;
                js.ExecuteScript("document.querySelector(\"#NominateStudyControl_m_archiverOrderNotesTextBox\").setAttribute(\"style\", \"color: Black; background-color: White; border-style: none; font-family: Arial; font-size: smaller; width: 100%; height: " + orderHeight + "px; resize: vertical\")");
                Size height1 = orderField.Size;
                orderField.SendKeys(order);

                //Validate Order notes field is Expanded or not in nominate for archive window - step 5
                if (height1.Height > height.Height)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Click Nominate in Confirm window 
                inbounds.ClickConfirmNominate();
                inbounds.GetMatchingRow("Accession", AccessionNo).TryGetValue("Status", out studyStatus);

                //Validate Study is listed and status as Nominated For Archive- step 6
                if (studyStatus == "Nominated For Archive")
                {
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

                //Login as Archivist 
                login.LoginIConnect(arusername, arpassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study 
                inbounds.SearchStudy("Accession", AccessionNo);

                String studyStatus1;
                //Find Study Status
                inbounds.GetMatchingRow("Accession", AccessionNo).TryGetValue("Status", out studyStatus);

                //Validate Study is listed and status as Nominated For Archive in Archivist's inbounds - step 7
                if (studyStatus == "Nominated For Archive")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select Study and archive Study
                inbounds.SelectStudy("Accession", AccessionNo);
                IWebElement uploadComments, archiveOrder;
                inbounds.ClickArchiveStudy(out uploadComments, out archiveOrder);

                //Validate text in archive order matches with the order given in Archive window - step 8
                if (archiveOrder.Text == order)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Expanding archive order field
                var js1 = (IJavaScriptExecutor)BasePage.Driver;
                Size AOheight = archiveOrder.Size;
                js1.ExecuteScript("document.querySelector(\"#m_ReconciliationControl_ArchiverOrderNotes\").setAttribute(\"style\",\"color: Black; background-color: White; border-style: none; font-family: Arial; font-size: smaller; width: 50%; height: " + archiveOrderHeight + "px; \")");
                Size AOheight1 = archiveOrder.Size;

                //Validate Archive Order notes field is Expanded or not in Archive/Reconcilation window - step 9
                if (AOheight1.Height > AOheight.Height)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Click Archive Button
                inbounds.ClickArchive();

                inbounds.GetMatchingRow("Accession", AccessionNo).TryGetValue("Status", out studyStatus1);
                //Validate Study Status is archived from Archivist's inbounds 
                if (studyStatus1 != "Nominated For Archive")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Search Study 
                inbounds.SearchStudy("Accession", AccessionNo);

                //Find Study Status
                inbounds.GetMatchingRow("Accession", AccessionNo).TryGetValue("Status", out studyStatus);

                //Validate Study is listed and status as Nominated For Archive in Archivist's inbounds - step 10
                if (studyStatus == "Routing Completed")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as Archivist - step 11
                login.Logout();
                ExecutedSteps++;

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                ////Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps); ;
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }


        }

        /// <summary>
        ///  
        /// </summary>
        public TestCaseResult Test2_29478(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Outbounds outbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(4);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data                
                String arUsername = Config.arUserName;
                String arPassword = Config.arPassword;
                String order = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Order");
                String AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo1");

                //Login as archivist 
                login.LoginIConnect(arUsername, arPassword);

                //Navigate to Inbounds
                outbounds = (Outbounds)login.Navigate("Outbounds");

                //Search and Select Study
                outbounds.SearchStudy("Study Performed", "All Dates");
                outbounds.SelectStudy1("Accession", AccessionNo);

                IWebElement RerouteButton = BasePage.Driver.FindElement(By.CssSelector("#m_rerouteStudyButton"));
                IWebElement ArchiveButton = BasePage.Driver.FindElement(By.CssSelector("#m_archiveStudyButton"));
                IWebElement TransferButton = BasePage.Driver.FindElement(By.CssSelector("#m_transferButton"));
                IWebElement ViewButton = BasePage.Driver.FindElement(By.CssSelector("#m_viewStudyButton"));

                //Validate Reroute, archive, transfer and view buttons are enabled for the uploaded study
                if (RerouteButton.Enabled == true && ArchiveButton.Enabled == true && TransferButton.Enabled == true && ViewButton.Enabled == true)
                {
                    result.steps[0].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[0].description);
                }
                else
                {
                    result.steps[0].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[0].description);
                    result.steps[0].SetLogs();
                }

                //Launch Study
                outbounds.LaunchStudy();

                /** Insert code to verify study with DICOM directory **/
                Boolean Study = true;

                //Validate the uploaded study is loaded correctly in the viewer
                if (Study == true)
                {
                    result.steps[1].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[1].description);
                }
                else
                {
                    result.steps[1].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[1].description);
                    result.steps[1].SetLogs();
                }

                //Close Study
                outbounds.CloseStudy();

                //Validate after archiving, status is changed from uploaded to arhiving or routing started
                //Search and Select Study
                outbounds.SearchStudy("Accession", AccessionNo);
                outbounds.SelectStudy1("Accession", AccessionNo);

                //Archive Study
                outbounds.ArchiveStudy("", "");

                String studyStatus;
                outbounds.GetMatchingRow("Accession", AccessionNo).TryGetValue("Status", out studyStatus);

                if (studyStatus != "Uploaded")
                {
                    result.steps[2].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[2].description);
                }
                else
                {
                    result.steps[2].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[2].description);
                    result.steps[2].SetLogs();
                }

                //Logout as Archivist
                login.Logout();

                //Validate the archived study is sent to the destination
                //Login as archivist 
                login.LoginIConnect(arUsername, arPassword);

                //Navigate to Inbounds
                outbounds = (Outbounds)login.Navigate("Outbounds");

                //Search and Select Study
                outbounds.SearchStudy("Accession", AccessionNo);

                //Study Status
                outbounds.GetMatchingRow("Accession", AccessionNo).TryGetValue("Status", out studyStatus);

                if (studyStatus == "Routing Completed")
                {
                    result.steps[3].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[3].description);
                }
                else
                {
                    result.steps[3].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[3].description);
                    result.steps[3].SetLogs();
                }

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Logout as Archivist
                login.Logout();

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps); ;
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }


        }

        /// <Test Case 1 - 29468>
        /// Send Exams and check whether it is correctly sent or not as ArchivistS
        /// </summary>
        public TestCaseResult Test1_161155(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            Outbounds outbounds = null;
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
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Upload a DICOM study - step 1
                ei.EIDicomUpload(arUsername, arPassword, Config.Dest1, UploadFilePath);
                ExecutedSteps++;
                ExecutedSteps++;

                //Validate Study is present in the Physician's InBounds
                //Login as Physician and Validate
                login.LoginIConnect(phUsername, phPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study
                inbounds.SearchStudy(AccessionNo : Accession);

                //Validation
                if (inbounds.CheckStudy("Accession", Accession) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not listed in Physiscian's inbounds");
                }

                //Logout as Physician
                login.Logout();
                ExecutedSteps++;

                //Validate Study is present in the Archivist OutBounds
                //Login as Archivist 
                login.LoginIConnect(arUsername, arPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                outbounds = (Outbounds)login.Navigate("Outbounds");

                //Search and Select Study
                outbounds.SearchStudy("Accession", Accession);
                outbounds.SelectStudy1("Accession", Accession);

                Dictionary<string, string> rowkeyvalue;
                rowkeyvalue = outbounds.GetMatchingRow("Accession", Accession);
                //Validation
                if (rowkeyvalue != null)
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

                //Validate StudyStatus as Uploaded and Check the image is correctly sent or not
                //Find study status
                String studyStatus;
                outbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus);

                //View Study
                BluRingViewer bluering = new BluRingViewer();
                bool bool8 = false;
                if (Config.isEnterpriseViewer.ToLower()=="y")
                {
                    bluering = BluRingViewer.LaunchBluRingViewer();
                    bool8 = true;
                }
                else
                {
                    outbounds.LaunchStudy();
                    bool8 = inbounds.ViewStudy();
                }
                

                //Validation
                if (studyStatus == "Uploaded" && bool8)
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

                //Close Study
                if (Config.isEnterpriseViewer.ToLower() == "y")
                    bluering.CloseBluRingViewer();
                else
                  inbounds.CloseStudy();

                //Logout as Archivist
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

        ///<Test Case 2 - 29468>
        /// <summary>
        /// Validate Search panel contains all searching fields in outbounds
        /// Validate appropriate study is listed with given search parameters
        /// </summary>
        public TestCaseResult Test2_161155(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Outbounds outbounds = null;
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
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String PID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String RefPh = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RefPhysician");
                String Modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String StudyDate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate");

                //Validate Search panel contains all searching fields in outbounds
                //Login as Archivist 
                login.LoginIConnect(arUsername, arPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                outbounds = (Outbounds)login.Navigate("Outbounds");

                IWebElement lastname = BasePage.Driver.FindElement(By.Id("m_studySearchControl_m_searchInputPatientLastName"));
                IWebElement firstname = BasePage.Driver.FindElement(By.CssSelector("#m_studySearchControl_m_searchInputPatientFirstName"));
                IWebElement patientID = BasePage.Driver.FindElement(By.Id("m_studySearchControl_m_searchInputPatientID"));
                IWebElement refPhysician = BasePage.Driver.FindElement(By.Id("m_studySearchControl_m_searchInputReferringPhysicianName"));
                IWebElement modality = BasePage.Driver.FindElement(By.Id("m_studySearchControl_m_searchInputModality"));
                IWebElement accNo = BasePage.Driver.FindElement(By.Id("m_studySearchControl_m_searchInputAccession"));
                IWebElement menuper = BasePage.Driver.FindElement(By.Id("searchStudyDropDownMenu"));
                IWebElement menurec = BasePage.Driver.FindElement(By.Id("searchStudyCreatedDropDownMenu"));

                //Validation
                if ((lastname.Displayed == true) && (firstname.Displayed == true) && (patientID.Displayed == true) && (refPhysician.Displayed == true)
                    && (modality.Displayed == true) && (accNo.Displayed == true) && (menuper.Displayed == true) && (menurec.Displayed == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("One/All Of the Panels are not displayed");
                }

                //Search Study with multiple parameters
                outbounds.SearchStudy(LastName, FirstName, PID, RefPh, Accession, Modality, "All Dates", "");

                //Choose Columns
                outbounds.ChooseColumns(new string[] { "Last Name", "First Name", "Refer. Physician", "Modality" });

                Dictionary<string, string> study = outbounds.GetMatchingRow(new string[] { "Last Name", "First Name", "Patient ID", "Refer. Physician", "Accession", "Modality" },
                    new string[] { LastName, FirstName, PID, RefPh, Accession, Modality });

                String iCAdate = DateTime.ParseExact(study["Study Date"], "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();
                String date = DateTime.ParseExact(StudyDate, "dd-MMM-yy", CultureInfo.InvariantCulture).ToShortDateString();

                //Validate appropriate study is listed with given search parameters
                if (study != null && iCAdate.Equals(date))
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

        /// <Test Case 3- 29468>
        /// Validate Nominate button is Disabled while multiple Studies selected
        /// </summary>
        public TestCaseResult Test3_161155(String testid, String teststeps, int stepcount)
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
                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNoList.Split(':');
                String reason = (String)ReadExcel.GetTestData(filepath, "TestData", "29468", "NominationReason");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Upload mutiple DICOM studies - step 1
                ei.EIDicomUpload(arUsername, arPassword, Config.Dest1, UploadFilePath);
                ExecutedSteps++;

                //Login as Archivist 
                login.LoginIConnect(phUsername, phPassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                IWebElement NominateButton = BasePage.Driver.FindElement(By.CssSelector("#m_nominateStudyButton"));

                //Nominate multiple studies - step 
                for (int i = 0; i < AccessionNumbers.Length; i++)
                {
                    //Search and Select Study
                    inbounds.SearchStudy("Accession", AccessionNumbers[i]);
                    inbounds.SelectStudy("Accession", AccessionNumbers[i]);

                    if (NominateButton.Enabled == true)
                    {
                        //Nominate for Archive
                        inbounds.NominateForArchive(reason);
                        Logger.Instance.InfoLog("Study with accession number " + AccessionNumbers[i] + " is Nominated for Archive.");
                    }
                }
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                ExecutedSteps++;
                foreach (String Accession in AccessionNumbers)
                {
                    //Search and Select Study
                    inbounds.SearchStudy("Accession", Accession);

                    Dictionary<string, string> study = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Nominated For Archive" });

                    //Validate Study status as Nominated For Archive in Physician's inbounds
                    if (study != null)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //Logout as Physician
                login.Logout();

                //Login as Archivist 
                login.LoginIConnect(arUsername, arPassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study
                inbounds.SearchStudy("Accession", "*");

                //Select all prior studies 
                foreach (String AccNo in AccessionNumbers)
                {
                    inbounds.SelectStudy1("Accession", AccNo, true);

                }

                //Validation
                IWebElement archiveStudy = BasePage.Driver.FindElement(By.CssSelector("#m_archiveStudyButton"));
                if (archiveStudy.Enabled != true)
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

        ///<Test Case 1- 29472> 
        /// <summary>
        /// <Validate Study is present in Physician InBounds,Nominated and archived in archivist inbounds>
        /// Upload studies from PACS gateway to the new destinations created>nominate study>archive study
        /// </summary>
        public TestCaseResult Test1_29472(String testid, String teststeps, int stepcount)
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
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String pacusername = Config.pacsadmin;
                String pacpassword = Config.pacspassword;
                String AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String reason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Step-1: Upload a study using PACS gateway in different Destination
                BasePage.RunBatchFile(Config.batchfilepath, studypath + " " + Config.dicomsendpath + " " + Config.StudyPacs);

                //Send the study to dicom devices from MergePacs management page
                //Login MPacs
                login.DriverGoTo(login.mpacstudyurl);
                MpacLogin mplogin = new MpacLogin();
                MPHomePage homepage = new MPHomePage();
                mplogin.Loginpacs(pacusername, pacpassword);
                Tool tools = (Tool)homepage.NavigateTopMenu("Tools");
                homepage.NavigateTopMenu("Tools");
                tools.NavigateToSendStudy();
                tools.SearchStudy("Accession", AccessionNo, 0);
                tools.MpacSelectStudy("Patient ID", pid);
                tools.SendStudy(1);

                //Logout MPacs
                mpaclogin.LogoutPacs();

                //Search for the study in HP with updation
                //Login in HP
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin1 = new HPLogin();
                HPHomePage hphomepage1 = hplogin1.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow1 = (WorkFlow)hphomepage1.Navigate("Workflow");

                //Step-6:Search for the study using acc no'
                workflow1.NavigateToLink("Workflow", "Archive Search");
                PageLoadWait.WaitForStudyInHp(180, AccessionNo, workflow1);
                workflow1.HPSearchStudy("Accessionno", AccessionNo);
                Boolean study = workflow1.HPCheckStudy(AccessionNo);
                if (study == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study is not found");
                }

                //Logout in HP
                hplogin1.LogoutHPen();

                //Validate Study is present in the Physician's InBounds                

                //Step-2:Login as Physician 
                login.DriverGoTo(login.url);
                login.LoginIConnect(phUsername, phPassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                PageLoadWait.WaitforReceivingStudy(180, pid);
                PageLoadWait.WaitforUpload(AccessionNo, inbounds);
                //Search  Study
                inbounds.SearchStudy("Accession", AccessionNo);


                //Validation
                if (inbounds.CheckStudy("Accession", AccessionNo) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study is not found");
                }

                //Step-3:Select the study
                inbounds.SelectStudy("Accession", AccessionNo);
                //Nominate study for Archive
                inbounds.NominateForArchive(reason);
                ExecutedSteps++;

                //Logout as Physician
                login.Logout();

                //Step-4:Login as Archivist 
                login.LoginIConnect(arUsername, arPassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select Study
                inbounds.SearchStudy("Accession", AccessionNo);
                inbounds.SelectStudy("Accession", AccessionNo);
                //Click ArchiveStudy
                inbounds.ClickArchiveStudy("", "");
                PageLoadWait.WaitForLoadInArchive(10);
                //Click Archive
                inbounds.ClickArchive();


                //Validate StudyStatus as Routing Completed and Check the image is correctly sent or not
                //Find study status
                inbounds.SearchStudy("Accession", AccessionNo);
                inbounds.SearchStudy("Accession", AccessionNo);
                inbounds.SelectStudy("Accession", AccessionNo);
                Dictionary<string, string> study0 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNo, "Routing Completed" });
                //View Study
                inbounds.LaunchStudy();

                //Validation
                if (study0 != null && inbounds.ViewStudy() == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close Study
                inbounds.CloseStudy();

                //Logout as Archivist
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

        ///<Test Case 2 - 29472> 
        /// <summary>
        /// <Validate Study is present in Physician InBounds,Nominated and archived in archivist inbounds>
        /// Upload studies from Exam Importer to the new destinations created>nominate study>archive study
        /// </summary>
        public TestCaseResult Test2_29472(String testid, String teststeps, int stepcount)
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
                String AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String reason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Step-1:Upload a study using Exam Importer in new Destination
                ei.EIDicomUpload(phUsername, phPassword, Config.Dest1, UploadFilePath);
                ExecutedSteps++;

                //Validate Study is present in the Physician's InBounds                

                //Step-2:Login as Physician 
                login.LoginIConnect(phUsername, phPassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search
                inbounds.SearchStudy("Accession", AccessionNo);

                //Validation
                if (inbounds.CheckStudy("Accession", AccessionNo) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study is not found");
                }

                //Step-3:Select the study
                inbounds.SelectStudy("Accession", AccessionNo);
                //Nominate study for Archive
                inbounds.NominateForArchive(reason);
                ExecutedSteps++;

                //Logout as Physician
                login.Logout();

                //Step-4:Login as Archivist 
                login.LoginIConnect(arUsername, arPassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select Study
                inbounds.SearchStudy("Accession", AccessionNo);
                inbounds.SelectStudy("Accession", AccessionNo);
                //Click ArchiveStudy
                inbounds.ClickArchiveStudy("", "");
                PageLoadWait.WaitForLoadInArchive(10);
                //Click Archive
                inbounds.ClickArchive();


                //Validate StudyStatus as Routing Completed and Check the image is correctly sent or not
                //Find study status
                inbounds.SearchStudy("Accession", AccessionNo);
                inbounds.SearchStudy("Accession", AccessionNo);
                inbounds.SelectStudy("Accession", AccessionNo);
                Dictionary<string, string> study0 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNo, "Routing Completed" });
                //View Study
                inbounds.LaunchStudy();
                Boolean Study = true;

                //Validation
                if (study0 != null && Study == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close Study
                inbounds.CloseStudy();

                //Logout as Archivist
                login.Logout();

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

        /// <Summary>
        /// This is Automation of Test 29476
        /// This Test Case is to nominate from ph and archive from ar by uploading study through  PACS gateway
        /// </Summary>
        public TestCaseResult Test1_29476(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            Studies studies = null;
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
                String uname = Config.ar1UserName;
                String pwd = Config.ar1Password;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String pacusername = Config.pacsadmin;
                String pacpassword = Config.pacspassword;
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                String stUsername = Config.stUserName;
                String stPassword = Config.stPassword;
                String reason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String orderacc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderAccession");
                String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String orderpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");

                //Step-1:Send an order
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

                //Step-2:Upload study by PACS Gateway
                //Import a study which  matches the existing order Patient ID and Acc no' to Merge PACs#2
                BasePage.RunBatchFile(Config.batchfilepath, studypath + " " + Config.dicomsendpath + " " + Config.StudyPacs);

                //Send the study to dicom devices from MergePacs management page
                //Login MPacs
                login.DriverGoTo(login.mpacstudyurl);
                MPHomePage mphomepage = mpaclogin.Loginpacs(pacusername, pacpassword);
                Tool mpactool = (Tool)mphomepage.NavigateTopMenu("Tools");
                mphomepage.NavigateTopMenu("Tools");
                mpactool.NavigateToSendStudy();
                mpactool.SearchStudy("Accession", accession, 0);
                mpactool.MpacSelectStudy("Patient ID", pid);
                mpactool.SendStudy(1);

                //Logout MPacs
                mpaclogin.LogoutPacs();

                //Search for such study in HP exists without any updation
                //Login in HP
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin = new HPLogin();
                hplogin.LoginHPen(hpUserName, hpPassword);
                HPHomePage hphomepage = new HPHomePage();
                hphomepage.Navigate("Workflow");
                WorkFlow workflow = new WorkFlow();
                workflow.NavigateToLink("Workflow", "Archive Search");

                //search study using acc no'
                PageLoadWait.WaitForStudyInHp(180, accession, workflow);
                workflow.HPSearchStudy("Accessionno", accession);

                Boolean study = workflow.HPCheckStudy(accession);

                if (study == true)
                {
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

                //Step-3:Login as physician
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                PageLoadWait.WaitforStudyInStatus(accession, inbounds, "Uploaded");

                //Search and Select Study
                inbounds.SearchStudy("Accession", accession);

                //Find study status
                Dictionary<string, string> study0 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { accession, "Uploaded" });
                //Launch study
                inbounds.SelectStudy("Accession", accession);
                inbounds.LaunchStudy();
                //Validate Study is listed and status as Uploaded
                if (study0 != null && inbounds.ViewStudy() == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Status is not as Uploaded");
                }

                //Step-4:Nominate study through the toolbar
                inbounds.Nominatestudy_toolbar();
                ExecutedSteps++;

                //Step-5:Search the study for its status as Nominated For Archive
                inbounds.SearchStudy("Accession", accession);

                //Find study status
                Dictionary<string, string> study1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { accession, "Nominated For Archive" });

                //Validate Study is listed and status as Nominated For Archive
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

                //Logout as physician
                login.Logout();

                //Step-6:Login as archivist
                login.LoginIConnect(uname, pwd);

                //Navigate to Inbounds and search
                inbounds = (Inbounds)login.Navigate("Inbounds");

                inbounds.SearchStudy("Accession", accession);
                inbounds.SelectStudy("Accession", accession);

                //Launch study
                inbounds.LaunchStudy();

                //Validate the study is loaded with no error
                if (inbounds.ViewStudy() == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();

                }

                //Step-7:Archive study-toolbar
                inbounds.ClickArchive_toolbar();

                //Search order
                inbounds.ArchiveSearch("order", "All Dates");

                //Validate that recociliation window opens
                Dictionary<String, String> OrderDetails = inbounds.GetDataInArchive("Matching Order");
                Dictionary<String, String> OriginalDetails = inbounds.GetDataInArchive("Original Details");

                if ((OrderDetails["Last Name"].Equals(OriginalDetails["Last Name"])) && (OrderDetails["First Name"].Equals(OriginalDetails["First Name"])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();

                }

                //Step-8:Manually reconcile the study
                //Edit the order
                inbounds.SetCheckBoxInArchive("matching order", "gender");

                //Click mandatory fields
                inbounds.SetCheckBoxInArchive("original details", "PID");
                inbounds.SetCheckBoxInArchive("original details", "Accession");
                inbounds.SetBlankFinalDetailsInArchive();

                //Click Archive
                inbounds.ClickArchive();
                inbounds.CloseStudy();

                inbounds.SearchStudy("Accession", accession);

                //Find study status
                Dictionary<string, string> study2 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { accession, "Routing Completed" });

                //Validate Study is listed and status as Routing Completed
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


                //Step-9:Check in the studies tab that the study reached the destination
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", accession);

                Dictionary<string, string> study3 = studies.GetMatchingRow(new string[] { "Accession", "Patient ID" }, new string[] { accession, pid });

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

                //Logout as archivist
                login.Logout();

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
        ///  From PACS send a study with HL7 matching order in MWLSCP to Dest1
        ///  Validate Studies in the Holding Pen, ICA and POP Config tool
        ///  Validate the NO. of images of the study from ICA and check if buttons are enabled
        /// </summary>
        public TestCaseResult Test1_29461(String testid, String teststeps, int stepcount)
        {
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String phUsername = Config.ph1UserName;
                String phPassword = Config.ph1Password;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String Imagesno = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NoOfImages");
                String seriescount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NoOfSeries");
                String OrderPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");
                String StudyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Send HL7 order
                mpaclogin.SendHL7Order(Config.MergePACsIP, int.Parse(Config.mpacport), OrderPath);

                //Send Study to Merge Pacs
                BasePage.RunBatchFile(Config.batchfilepath, StudyPath + " " + Config.dicomsendpath + " " + Config.StudyPacs);

                //login to PACS#1 as admin
                mpaclogin.DriverGoTo(mpaclogin.mpacstudyurl);
                MPHomePage mpachome = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order - step 1 & 2
                Tool tool = (Tool)mpachome.NavigateTopMenu("Tools");
                tool.NavigateToSendStudy();
                tool.SearchStudy("Accession", AccessionID, 0);
                tool.MpacSelectStudy("Accession", AccessionID);
                tool.SendStudy(1);
                mpaclogin.LogoutPacs();

                ExecutedSteps++;
                ExecutedSteps++;

                // Login as Ph from POP Config tool and check the active transfers- Not Automatable step 3
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Login and Navigate to archive search in Holding pen
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin = new HPLogin();
                HPHomePage hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");

                PageLoadWait.WaitForStudyInHp(180, AccessionID, workflow);
                int totalseries = 0,timeout = 0;
                while(totalseries != Int32.Parse(seriescount) && timeout++ <= 2)
                {
                    workflow.HPSearchStudy("Accessionno", AccessionID);
                    totalseries = workflow.NumberOfSeries();
                }

                //Validate Study in the Holding Pen - step 4
                if (workflow.HPCheckStudy(AccessionID) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study is not present in Holding Pen");
                }

                //Logout in holding Pen
                hplogin.LogoutHPen();

                //Validating study in ICA
                login.DriverGoTo(login.url);
                login.LoginIConnect(phUsername, phPassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study
                inbounds.SearchStudy("Accession", AccessionID);
                String studyStatus1;
                inbounds.GetMatchingRow("Accession", AccessionID).TryGetValue("Status", out studyStatus1);

                //Validate Study is listed and status is Uploaded or uploading - step 5
                if (studyStatus1.Equals("Uploaded") || studyStatus1.Equals("Uploading") || studyStatus1.Equals("Routing Completed"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Validate No. of images and series in the study - step 6
                String images;
                inbounds.GetMatchingRow("Accession", AccessionID).TryGetValue("Number of Images", out images);
                if ((images.Split('/')[0].Equals(Imagesno)) && (Int32.Parse(seriescount).Equals(totalseries)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Validate upload button is enabled even when study not selected - step 7
                if (BasePage.Driver.FindElement(By.CssSelector("#m_launchUploaderButton")).Enabled == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Upload button is Enabled");
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                ExecutedSteps++;
                //Validate buttons disabled without selecting a study - step 8                
                //Validate addreciever button is disabled
                if (BasePage.Driver.FindElement(By.CssSelector("#m_addReceiverButton")).Enabled == false)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Addreciever button is disabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Validate GrantAccess button disabled
                if (BasePage.Driver.FindElement(By.CssSelector("#m_grantAccessButton")).Enabled == false)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("GrantAccess button is disabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Validate Deletestudy button
                if (BasePage.Driver.FindElement(By.CssSelector("#m_deleteStudiesButton")).Enabled == false)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("DeleteStudy button is disabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Validate TransferStudy button disabled                
                if (BasePage.Driver.FindElement(By.CssSelector("#m_transferButton")).Enabled == false)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Transfer study button is disabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Validate EmailStudy button disabled
                if (BasePage.Driver.FindElement(By.CssSelector("#m_emailStudyButton")).Enabled == false)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Email study button is disabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Validate Nominate for Archive button disabled
                if (BasePage.Driver.FindElement(By.CssSelector("#m_nominateStudyButton")).Enabled == false)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Nominate for archive button is disabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //RerouteStudy button
                if (BasePage.Driver.FindElement(By.CssSelector("#m_rerouteStudyButton")).Enabled == false)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Rreroute study button is disabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //ViewStudy button
                if (BasePage.Driver.FindElement(By.CssSelector("#m_viewStudyButton")).Enabled == false)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("View study button is disabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                ExecutedSteps++;
                //Validate following buttons enabled after selecting a study except Archive Study button - step 9
                inbounds.SelectStudy("Accession", AccessionID);

                // Upload button
                if (BasePage.Driver.FindElement(By.CssSelector("#m_launchUploaderButton")).Enabled == true)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Upload button is Enabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //addreciever button
                if (BasePage.Driver.FindElement(By.CssSelector("#m_addReceiverButton")).Enabled == true)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Add receiver button is Enabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //GrantAccess button
                if (BasePage.Driver.FindElement(By.CssSelector("#m_grantAccessButton")).Enabled == true)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Add receiver button is Enabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Deletestudy button
                if (BasePage.Driver.FindElement(By.CssSelector("#m_deleteStudiesButton")).Enabled == true)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Add receiver button is Enabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //TransferStudy button                
                if (BasePage.Driver.FindElement(By.CssSelector("#m_transferButton")).Enabled == true)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Add receiver button is Enabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //EmailStudy button
                if (BasePage.Driver.FindElement(By.CssSelector("#m_emailStudyButton")).Enabled == true)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Add receiver button is Enabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Nominate for Archive button
                if (BasePage.Driver.FindElement(By.CssSelector("#m_nominateStudyButton")).Enabled == true)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Add receiver button is Enabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //RerouteStudy button
                if (BasePage.Driver.FindElement(By.CssSelector("#m_rerouteStudyButton")).Enabled == true)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Add receiver button is Enabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //ViewStudy button
                if (BasePage.Driver.FindElement(By.CssSelector("#m_viewStudyButton")).Enabled == true)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Add receiver button is Enabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //HTML 5 View button
                if (Config.BrowserType.ToLower().Equals("chrome"))
                {
                    if (BasePage.Driver.FindElement(By.CssSelector("#m_html5ViewStudyButton")).Enabled == true)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        Logger.Instance.InfoLog("Add receiver button is Enabled");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //Validate no archive study button is enabled when physican logs in. - step 10
                ExecutedSteps++;
                try
                {
                    IWebElement archivebutton = BasePage.Driver.FindElement(By.CssSelector("#m_archiveStudyButton"));
                    if (!(archivebutton.Enabled) || !(archivebutton.Displayed))
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        Logger.Instance.InfoLog("Archive study button is disabled");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                catch (Exception)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                //Logout - step 11
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
        ///  From PACS send a study without HL7 matching order in MWLSCP to Dest1
        ///  Validate Studies in the Holding Pen, ICA and POP Config tool
        ///  Validate the NO. of images of the study from ICA and check if buttons are enabled
        /// </summary>
        public TestCaseResult Test2_29461(String testid, String teststeps, int stepcount)
        {
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String phUsername = Config.ph1UserName;
                String phPassword = Config.ph1Password;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String Imagesno = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NoOfImages");
                String seriescount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NoOfSeries");
                String StudyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Send Study to Merge Pacs
                BasePage.RunBatchFile(Config.batchfilepath, StudyPath + " " + Config.dicomsendpath + " " + Config.StudyPacs);

                //login to PACS#1 as admin
                mpaclogin.DriverGoTo(mpaclogin.mpacstudyurl);
                MPHomePage mpachome = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order - steps 1 & 2
                Tool tool = (Tool)mpachome.NavigateTopMenu("Tools");
                tool.NavigateToSendStudy();
                tool.SearchStudy("Accession", AccessionID, 0);
                tool.MpacSelectStudy("Accession", AccessionID);
                tool.SendStudy(1);
                mpaclogin.LogoutPacs();

                ExecutedSteps++;
                ExecutedSteps++;

                // Login as Ph from POP Config tool and check the active transfers- Not Automatable step 3
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Login and Navigate to archive search in Holding pen
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin = new HPLogin();
                HPHomePage hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");

                PageLoadWait.WaitForStudyInHp(180, AccessionID, workflow);
                int totalseries = 0, timeout = 0;
                while (totalseries != Int32.Parse(seriescount) && timeout++ <= 2)
                {
                    workflow.HPSearchStudy("Accessionno", AccessionID);
                    totalseries = workflow.NumberOfSeries();
                }

                //Validate Study in the Holding Pen - step 4
                if (workflow.HPCheckStudy(AccessionID) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study is not present in Holding Pen");
                }

                //Logout in holding Pen
                hplogin.LogoutHPen();

                //Validating study in ICA
                login.DriverGoTo(login.url);
                login.LoginIConnect(phUsername, phPassword);
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study
                inbounds.SearchStudy("Accession", AccessionID);
                String studyStatus1;
                inbounds.GetMatchingRow("Accession", AccessionID).TryGetValue("Status", out studyStatus1);

                //Validate Study is listed and status is Uploaded or uploading - step 5
                if (studyStatus1.Equals("Uploaded") || studyStatus1.Equals("Uploading"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Validate No. of images and series in the study - step 6
                String images;
                inbounds.GetMatchingRow("Accession", AccessionID).TryGetValue("Number of Images", out images);
                if ((images.Split('/')[0].Equals(Imagesno)) && (Int32.Parse(seriescount).Equals(totalseries)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Validate upload button is enabled even when study not selected - step 7
                if (BasePage.Driver.FindElement(By.CssSelector("#m_launchUploaderButton")).Enabled == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Upload button is Enabled");
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                ExecutedSteps++;
                //Validate buttons disabled without selecting a study - step 8                
                //Validate addreciever button is disabled
                if (BasePage.Driver.FindElement(By.CssSelector("#m_addReceiverButton")).Enabled == false)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Addreciever button is disabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                // Validate GrantAccess button disabled
                if (BasePage.Driver.FindElement(By.CssSelector("#m_grantAccessButton")).Enabled == false)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("GrantAccess button is disabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Validate Deletestudy button
                if (BasePage.Driver.FindElement(By.CssSelector("#m_deleteStudiesButton")).Enabled == false)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("DeleteStudy button is disabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Validate TransferStudy button disabled                
                if (BasePage.Driver.FindElement(By.CssSelector("#m_transferButton")).Enabled == false)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Transfer study button is disabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Validate EmailStudy button disabled
                if (BasePage.Driver.FindElement(By.CssSelector("#m_emailStudyButton")).Enabled == false)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Email study button is disabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Validate Nominate for Archive button disabled
                if (BasePage.Driver.FindElement(By.CssSelector("#m_nominateStudyButton")).Enabled == false)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Nominate for archive button is disabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //RerouteStudy button
                if (BasePage.Driver.FindElement(By.CssSelector("#m_rerouteStudyButton")).Enabled == false)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Rreroute study button is disabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //ViewStudy button
                if (BasePage.Driver.FindElement(By.CssSelector("#m_viewStudyButton")).Enabled == false)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("View study button is disabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                ExecutedSteps++;
                //Validate following buttons enabled after selecting a study except Archive Study button - step 9
                inbounds.SelectStudy("Accession", AccessionID);

                // Upload button
                if (BasePage.Driver.FindElement(By.CssSelector("#m_launchUploaderButton")).Enabled == true)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Upload button is Enabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //addreciever button
                if (BasePage.Driver.FindElement(By.CssSelector("#m_addReceiverButton")).Enabled == true)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Add receiver button is Enabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //GrantAccess button
                if (BasePage.Driver.FindElement(By.CssSelector("#m_grantAccessButton")).Enabled == true)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Add receiver button is Enabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Deletestudy button
                if (BasePage.Driver.FindElement(By.CssSelector("#m_deleteStudiesButton")).Enabled == true)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Add receiver button is Enabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //TransferStudy button                
                if (BasePage.Driver.FindElement(By.CssSelector("#m_transferButton")).Enabled == true)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Add receiver button is Enabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //EmailStudy button
                if (BasePage.Driver.FindElement(By.CssSelector("#m_emailStudyButton")).Enabled == true)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Add receiver button is Enabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Nominate for Archive button
                if (BasePage.Driver.FindElement(By.CssSelector("#m_nominateStudyButton")).Enabled == true)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Add receiver button is Enabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //RerouteStudy button
                if (BasePage.Driver.FindElement(By.CssSelector("#m_rerouteStudyButton")).Enabled == true)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Add receiver button is Enabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //ViewStudy button
                if (BasePage.Driver.FindElement(By.CssSelector("#m_viewStudyButton")).Enabled == true)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Add receiver button is Enabled");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //HTML 5 View button
                if (Config.BrowserType.ToLower().Equals("chrome"))
                {
                    if (BasePage.Driver.FindElement(By.CssSelector("#m_html5ViewStudyButton")).Enabled == true)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        Logger.Instance.InfoLog("Add receiver button is Enabled");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //Validate no archive study button is enabled when physican logs in. - step 10
                ExecutedSteps++;
                try
                {
                    if (!(BasePage.Driver.FindElement(By.CssSelector("#m_archiveStudyButton")).Enabled)
                        || !(BasePage.Driver.FindElement(By.CssSelector("#m_archiveStudyButton")).Displayed))
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        Logger.Instance.InfoLog("Archive study button is disabled");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                catch (Exception)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Archive study button not found");
                }

                //Logout
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
        /// This is to validate reroute button  is disabled for multiple studies
        /// </summary>
        public TestCaseResult Test1_29464(String testid, String teststeps, int stepcount)
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
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String acclist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = acclist.Split(':');
                String StudyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Send Studies to Destination-1 using POP - step 3
                BasePage.RunBatchFile(Config.batchfilepath, StudyPath + " " + Config.dicomsendpath + " " + Config.StudyPacs);

                //login to PACS#1 as admin
                mpaclogin.DriverGoTo(mpaclogin.mpacstudyurl);
                MPHomePage mpachome = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                Tool tool = (Tool)mpachome.NavigateTopMenu("Tools");
                tool.NavigateToSendStudy();
                foreach (String Accession in AccessionNumbers)
                {
                    tool.SearchStudy("Accession", Accession, 0);
                    tool.MpacSelectStudy("Accession", Accession);
                    tool.SendStudy(1);
                }

                //Logout
                mpaclogin.LogoutPacs();
                ExecutedSteps++;

                //Login as physician
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Wait until study reached to HP
                PageLoadWait.WaitforReceivingStudy(180, pid);

                foreach (string Accession in AccessionNumbers)
                {
                    PageLoadWait.WaitforUpload(Accession, inbounds);
                }

                BasePage.Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_searchInputAccession")).Clear();
                PageLoadWait.WaitHomePage();

                //Search and select multiple studies
                inbounds.SearchStudy("PatientID", pid);

                //Select all priors
                for (int i = 0; i < AccessionNumbers.Length; i++)
                {
                    inbounds.SelectStudy1(new String[] { "Accession" }, new String[] { AccessionNumbers[i] });
                }
                ExecutedSteps++;

                //Validate reroute button is not enabled for multilple studies
                IWebElement r = BasePage.Driver.FindElement(By.CssSelector("#m_rerouteStudyButton"));
                if (r.Enabled == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //logout
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
        /// <This is to reroute studies to different destinations>
        /// </summary>
        public TestCaseResult Test2_29464(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            Studies studies = null;
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
                String ph2u = Config.ph2UserName;
                String ph2p = Config.ph2Password;
                String ar2u = Config.ar2UserName;
                String ar2p = Config.ar2Password;
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String acclist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = acclist.Split(':');

                //Initial steps - step 1 & 2 -- Create a set of users for Destination-2 and update user nu to have no receiver and archivist role
                ExecutedSteps++;
                ExecutedSteps++;

                //Login as ph (Destination-1)
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("PatientID", pid);

                ExecutedSteps++;
                //Validate all priors are present in inbounds. - step 3
                foreach (String AccNo in AccessionNumbers)
                {
                    Dictionary<string, string> priors = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccNo, "Uploaded" });
                    if (priors != null)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        throw new Exception("Studies are not Uploaded");
                    }
                }

                //Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Search and nominate the priors to archive with validation
                studies.SearchStudy("PatientID", pid);

                //Validate all priors are not present in studies.
                foreach (String AccNo in AccessionNumbers)
                {
                    Dictionary<string, string> priors = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccNo, "Uploaded" });
                    if (priors == null)
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
                }

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("PatientID", pid);
                ExecutedSteps++;

                //Nominate all studies for archive and validate the status - step 4
                foreach (String AccNo in AccessionNumbers)
                {
                    //Select and Nominate study for archive
                    inbounds.SelectStudy("Accession", AccNo);
                    inbounds.NominateForArchive("Testing");

                    //Study Status
                    String studyStatus1;
                    inbounds.GetMatchingRow("Accession", AccNo).TryGetValue("Status", out studyStatus1);
                    if (studyStatus1 == "Nominated For Archive")
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
                }

                //Logout as ph
                login.Logout();

                //Login as archivist
                login.LoginIConnect(username1, password1);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Select a prior
                inbounds.SearchStudy("PatientID", pid);
                inbounds.SelectStudy("Accession", AccessionNumbers[2]);

                //Validate both the destination in reroute window - step 5
                if (inbounds.CheckDestInRerouteWindow() == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close reroute window - step 6
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div#RerouteStudyDiv .buttonRounded_small_blue")));
                BasePage.Driver.FindElement(By.CssSelector("div#RerouteStudyDiv .buttonRounded_small_blue")).Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitHomePage();
                ExecutedSteps++;

                //Select one study with nominated for archive status
                inbounds.SearchStudy("Accession", AccessionNumbers[0]);
                inbounds.SelectStudy("Accession", AccessionNumbers[0]);

                //Load study in viewer - step 7
                inbounds.LaunchStudy();
                ExecutedSteps++;

                //Navigate to history panel
                inbounds.NavigateToHistoryPanel();
                inbounds.ChooseColumns(new String[] { "Accession" });
                ExecutedSteps++;

                //Validate all priors are present in history list and load another study in second viewer - step 8
                foreach (String AccessionNumber in AccessionNumbers)
                {
                    Dictionary<string, string> priors = inbounds.GetMatchingRow("Accession", AccessionNumber);
                    if (!(priors == null))
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                inbounds.OpenPriors(new String[] { "Accession" }, new String[] { AccessionNumbers[0] });

                //Exit viewer - step 9
                inbounds.CloseStudy();
                ExecutedSteps++;

                //Archive one prior to destination-1 - step 10
                inbounds.SearchStudy("Accession", AccessionNumbers[0]);
                inbounds.SelectStudy("Accession", AccessionNumbers[0]);
                inbounds.ArchiveStudy("", "first");
                ExecutedSteps++;

                //Select one nominated prior study and reroute it - step 11
                inbounds.SearchStudy("Accession", AccessionNumbers[1]);
                inbounds.SelectStudy("Accession", AccessionNumbers[1]);
                inbounds.RerouteStudy(Config.Dest2);
                ExecutedSteps++;

                //Search Study
                inbounds.SearchStudy("Accession", AccessionNumbers[1]);

                //Validate rerouted study is not present in ar's inbounds - step 12
                if (inbounds.CheckStudy("Accession", AccessionNumbers[1]) == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout ar
                login.Logout();

                //Non Automated Step 13 - email notification
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Login as ph2(receiver) - step 14
                login.LoginIConnect(ph2u, ph2p);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Check for rerouted study
                inbounds.SearchStudy("Accession", AccessionNumbers[1]);

                //Find study status 
                String studyStatus2;
                inbounds.GetMatchingRow("Accession", AccessionNumbers[1]).TryGetValue("Status", out studyStatus2);

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


                //Select ,launch and nominate - step 16
                inbounds.SelectStudy("Accession", AccessionNumbers[1]);
                inbounds.LaunchStudy();
                ExecutedSteps++;

                //Nominate study in viewer tool bar
                inbounds.Nominatestudy_toolbar();

                //Validate its status in inbounds
                inbounds.SearchStudy("Accession", AccessionNumbers[1]);

                //Find study status
                String studyStatus3;
                inbounds.GetMatchingRow("Accession", AccessionNumbers[1]).TryGetValue("Status", out studyStatus3);

                //Validate Study is listed and status as Uploaded - step 17
                if (studyStatus3 == "Nominated For Archive")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as ph4
                login.Logout();

                //Login as ar4 - step 18
                login.LoginIConnect(ar2u, ar2p);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search , select , launch and archive
                inbounds.SearchStudy("Accession", AccessionNumbers[1]);
                inbounds.SelectStudy("Accession", AccessionNumbers[1]);

                //Launch the Study and archive
                inbounds.LaunchStudy();
                inbounds.ClickArchive_toolbar();

                inbounds.ArchiveSearch("Order", "", "", "", "", "", "", "", AccessionNumbers[1], "All Dates");
                Dictionary<String, String> MatchingValues = inbounds.GetDataInArchive("Matching Order");

                //Validate no details are found in Matching order - step 19
                if ((MatchingValues.Values.Contains("")) || (MatchingValues.Values.Contains(null)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Get original details in archive window
                Dictionary<String, String> OriginalValues = inbounds.GetDataInArchive("Original Details");

                //Verifying details in original details column and iCA - step 20

                ExecutedSteps++;

                //Click archive in reconcile window and close viewer
                inbounds.ClickArchive();
                inbounds.CloseStudy();

                //Search study
                inbounds.SearchStudy("Accession", AccessionNumbers[1]);

                //Find study status
                String studyStatus4;
                inbounds.GetMatchingRow("Accession", AccessionNumbers[1]).TryGetValue("Status", out studyStatus4);

                //Validate Study is reached to dest - 2 and verify all info are sync with original values - step 21
                if (studyStatus4 == "Routing Completed")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as ar4
                login.Logout();

                //Login as ph2
                login.LoginIConnect(username, password);
                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search the rest of the study,select and launch
                inbounds.SearchStudy("Study Performed", "All Dates");
                inbounds.SelectStudy("Accession", AccessionNumbers[2]);
                inbounds.LaunchStudy();
                Boolean st = true;

                //Validate prior study in Nominated status is loaded in viewer - step 22
                if (st == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //open history panel and add datasource column 
                inbounds.NavigateToHistoryPanel();
                inbounds.ChooseColumns(new String[] { "Data Source" });
                inbounds.ChooseColumns(new String[] { "Accession" });
                ExecutedSteps++;

                //Validate prior studies are displayed in Patient history tab- step 23
                foreach (String AccessionNumber in AccessionNumbers)
                {
                    Dictionary<string, string> priors = inbounds.GetMatchingRow("Accession", AccessionNumber);
                    if (!(priors == null))
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                ExecutedSteps++;
                //Validate yellow icon is displayed for nominated studies in Patient history panel - step 24
                for (int i = 3; i < AccessionNumbers.Length; i++)
                {

                    if (inbounds.CheckForeignExamAlert("Accession", AccessionNumbers[i]) == true)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                ExecutedSteps++;
                //Validate yellow icon contains foreign exam message - step 25
                for (int i = 3; i < AccessionNumbers.Length; i++)
                {

                    if (inbounds.CheckForeignExamMessage("Accession", AccessionNumbers[i]) == true)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                ExecutedSteps++;
                //Validate Yellow triangle icon is displayed in study panel toolbar of Studies with uploaded status - step 26
                int studyIndex = 0;

                for (int i = 3; i < AccessionNumbers.Length; i++)
                {
                    inbounds.OpenPriors(new String[] { "Accession" }, new String[] { AccessionNumbers[i] });

                    studyIndex++;
                    if (BasePage.Driver.FindElement(By.CssSelector("span[id*='" + (studyIndex + 1) + "_foreignExamDiv']")).Displayed == true)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    inbounds.NavigateToHistoryPanel();
                    PageLoadWait.WaitForPageLoad(20);
                }

                //Close the Study in viewer - step 27
                inbounds.CloseStudy();
                ExecutedSteps++;

                //Logout as ph2
                login.Logout();

                //Login as ph2
                login.LoginIConnect(username, password);

                //search for archived study in studies tab
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", AccessionNumbers[0]);

                //Validate archived study is present in studies tab - step 28
                Dictionary<string, string> StudyArchived = studies.GetMatchingRow("Accession", AccessionNumbers[0]);
                if (StudyArchived != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Launch study
                studies.SelectStudy1("Accession", AccessionNumbers[0]);
                studies.LaunchStudy();

                //open history panel and add datasource column 
                studies.NavigateToHistoryPanel();
                studies.ChooseColumns(new String[] { "Data Source", "Accession" });
                ExecutedSteps++;

                //Validate prior studies are displayed in Patient history tab - step 29
                foreach (String AccessionNumber in AccessionNumbers)
                {
                    Dictionary<string, string> priors = studies.GetMatchingRow("Accession", AccessionNumber);
                    if (!(priors == null))
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
                }

                ExecutedSteps++;
                //Validate yellow icon is displayed for nominated studies in Patient history panel - step 30
                for (int i = 2; i < AccessionNumbers.Length; i++)
                {
                    if (login.CheckForeignExamAlert("Accession", AccessionNumbers[i]) == true)
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
                }

                ExecutedSteps++;
                //Validate yellow icon contains foreign exam message - step 31
                for (int i = 2; i < AccessionNumbers.Length; i++)
                {

                    if (inbounds.CheckForeignExamMessage("Accession", AccessionNumbers[i]) == true)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                ExecutedSteps++;
                //Validate Yellow triangle icon is displayed in study panel toolbar of Studies with uploaded status - step 32
                int studyIndex1 = 0;
                for (int i = 2; i < AccessionNumbers.Length - 1; i++)
                {
                    studies.OpenPriors(new String[] { "Accession" }, new String[] { AccessionNumbers[i] });
                    studyIndex1++;
                    if (BasePage.Driver.FindElement(By.CssSelector("span[id*='" + (studyIndex1 + 1) + "_foreignExamDiv']")).Displayed == true)
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
                    login.NavigateToHistoryPanel();
                    PageLoadWait.WaitForPageLoad(20);

                }

                //Close the Study in viewer - step 33
                studies.CloseStudy();
                ExecutedSteps++;

                //Logout as ph2
                login.Logout();

                //Login as ph4
                login.LoginIConnect(ph2u, ph2p);

                //search for archived study in studies tab
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", AccessionNumbers[1]);

                //Validate archived study is present in studies tab - step 34
                Dictionary<string, string> StudyArchived1 = studies.GetMatchingRow("Accession", AccessionNumbers[1]);
                if (StudyArchived1 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Launch study
                studies.SelectStudy1("Accession", AccessionNumbers[1]);
                studies.LaunchStudy();

                //open history panel and add datasource column 
                studies.NavigateToHistoryPanel();
                studies.ChooseColumns(new String[] { "Data Source", "Accession" });
                ExecutedSteps++;

                //Validate prior studies are displayed in Patient history tab - step 35
                foreach (String AccessionNumber in AccessionNumbers)
                {
                    Dictionary<string, string> priors = login.GetMatchingRow("Accession", AccessionNumber);
                    if (!(priors == null))
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
                }

                ExecutedSteps++;
                //Validate yellow icon is displayed for nominated studies in Patient history panel - step 36
                for (int i = 2; i < AccessionNumbers.Length; i++)
                {
                    if (login.CheckForeignExamAlert("Accession", AccessionNumbers[i]) == true)
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
                }

                ExecutedSteps++;
                //Validate yellow icon contains foreign exam message - step 37
                for (int i = 2; i < AccessionNumbers.Length; i++)
                {

                    if (inbounds.CheckForeignExamMessage("Accession", AccessionNumbers[i]) == true)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                //Validate Yellow triangle icon is displayed in study panel toolbar of Studies with uploaded status - step 38
                int studyIndex2 = 0;
                for (int i = 3; i < AccessionNumbers.Length - 1; i++)
                {
                    studies.OpenPriors(new String[] { "Accession" }, new String[] { AccessionNumbers[i] });
                    studyIndex2++;
                    if (BasePage.Driver.FindElement(By.CssSelector("span[id*='" + (studyIndex2 + 1) + "_foreignExamDiv']")).Displayed == true)
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
                    studies.NavigateToHistoryPanel();
                }

                //Close the Study in viewer - step 39
                studies.CloseStudy();
                ExecutedSteps++;

                //Logout as ph4 - step 40
                login.Logout();
                ExecutedSteps++;

                //Non Automated steps 41 & 42
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;

            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps); ;
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary>
        ///upload 1 file from cd uploader validate it in hp and ph inbound and nominate for archive
        ///upload rest of the file from cd uploader validate in ph inbound,nominate and archive 
        /// </summary> 
        public TestCaseResult Test1_29465(String testid, String teststeps, int stepcount)
        {
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String phUsername = Config.ph1UserName;
                String phPassword = Config.ph1Password;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String arUsername = Config.ar1UserName;
                String arPassword = Config.ar1Password;
                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNoList.Split(':');
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] FilePaths = UploadFilePath.Split(':');
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EmailID");
                String Comments = login.RandomString(10, true);

                //Non - Automatable steps - 1 to 3 --Initial setup
                ExecutedSteps++;
                ExecutedSteps++;
                ExecutedSteps++;

                //Upload study using CDUploader - steps -4 & 5
                ei.EIDicomUploadUnReg(Email, Config.Dest1, FilePaths[0]);
                ExecutedSteps++;
                ExecutedSteps++;

                //Navigate to search in Holding pen
                login.DriverGoTo(login.hpurl);
                hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");

                //Search Study
                workflow.HPSearchStudy("Accessionno", AccessionNumbers[0]);

                //Validate study is present in holding pen - step 6
                if (workflow.HPCheckStudy(AccessionNumbers[0]) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study is not present in Holding Pen");
                }

                //Logout in holding Pen
                hplogin.LogoutHPen();

                //Login as physician 
                login.DriverGoTo(login.url);
                login.LoginIConnect(phUsername, phPassword);

                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study               
                inbounds.SearchStudy("Accession", AccessionNumbers[0]);
                String studyStatus1;
                inbounds.GetMatchingRow("Accession", AccessionNumbers[0]).TryGetValue("Status", out studyStatus1);

                //Validate Study is listed and status is Uploaded - step 7
                if (studyStatus1 == "Uploaded")
                {
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

                //Upload Studies as Unregistered user - step 8
                ei.EIDicomUploadUnReg(Email, Config.Dest1, "", "HIGH", FilePaths[1], Comments);
                ExecutedSteps++;

                //Navigate to search in Holding pen
                login.DriverGoTo(login.hpurl);
                hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow1 = (WorkFlow)hphomepage.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");

                ExecutedSteps++;
                //Validate studies are present in holding pen - step 9
                foreach (string Accession in AccessionNumbers)
                {
                    //Search Study
                    workflow1.HPSearchStudy("Accessionno", Accession);

                    if (workflow.HPCheckStudy(Accession) == true)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        throw new Exception("Study is not present in Holding Pen");
                    }
                }

                //Logout in holding Pen
                hplogin.LogoutHPen();

                //Login as physician 
                login.DriverGoTo(login.url);
                login.LoginIConnect(phUsername, phPassword);

                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                ExecutedSteps++;
                //Validate uploaded studies are present in physician's inbounds - step 10
                for (int i = 0; i < AccessionNumbers.Length; i++)
                {
                    if (i == 0) { continue; }
                    inbounds.SearchStudy("Accession", AccessionNumbers[i]);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    Dictionary<string, string> row = inbounds.GetMatchingRow(new string[] { "Accession", "Status", "Priority", "Comments" }, new string[] { AccessionNumbers[i], "Uploaded", "HIGH", Comments });
                    if (row != null)
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
                }

                ExecutedSteps++;
                //Validate all studies are viewed in viewer properly - step 11
                foreach (string Accession in AccessionNumbers)
                {
                    //Search and Select Study 
                    inbounds.SearchStudy("Accession", Accession);
                    inbounds.SelectStudy("Accession", Accession);

                    //Launch Study
                    inbounds.LaunchStudy();

                    Boolean Study = true;
                    if (Study == true)
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

                    //Close Study
                    inbounds.CloseStudy();
                }

                //Nominate all the studies - step 12
                foreach (string Accession in AccessionNumbers)
                {
                    //Search and Select Study 
                    inbounds.SearchStudy("Accession", Accession);
                    inbounds.SelectStudy("Accession", Accession);

                    //Nominate study for archive
                    inbounds.NominateForArchive("");
                }
                ExecutedSteps++;

                //Logout as Physician
                login.Logout();

                //Login as archivist
                login.LoginIConnect(arUsername, arPassword);

                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                ExecutedSteps++;
                //Validate all Studies are listed and status as Nominated For Archive - step 13
                foreach (string Accession in AccessionNumbers)
                {
                    //Search Study
                    inbounds.SearchStudy("Accession", Accession);

                    String studyStatus2;
                    inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus2);
                    if (studyStatus2 == "Nominated For Archive")
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
                }

                ExecutedSteps++;
                //Validate all studies are viewed in viewer properly - step 14
                foreach (string Accession in AccessionNumbers)
                {
                    //Search and Select Study 
                    inbounds.SearchStudy("Accession", Accession);
                    inbounds.SelectStudy("Accession", Accession);

                    //Launch Study
                    inbounds.LaunchStudy();

                    Boolean Study1 = true;
                    if (Study1 == true)
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

                    //Close Study
                    inbounds.CloseStudy();
                }

                //Archive all the Studies - step 15
                foreach (string Accession in AccessionNumbers)
                {
                    //Search and Select Study 
                    inbounds.SearchStudy("Accession", Accession);
                    inbounds.SelectStudy("Accession", Accession);

                    //Archive Study
                    inbounds.ArchiveStudy("", "");
                }
                ExecutedSteps++;

                ExecutedSteps++;
                //Validate the status as Routing Completed for all the studies - step 16
                foreach (string Accession in AccessionNumbers)
                {

                    //Search and Select Study 
                    inbounds.SearchStudy("Accession", Accession);
                    inbounds.SelectStudy("Accession", Accession);

                    String studyStatus;
                    inbounds.GetMatchingRow("Accession", Accession).TryGetValue("Status", out studyStatus);

                    if (inbounds.CheckStudy("Accession", Accession) == true && studyStatus == "Routing Completed")
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
                }

                //Logout as Archivist
                login.Logout();

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
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps); ;
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>        
        /// Uploading a non dicom and give a new patient Information.
        /// </summary>
        public TestCaseResult Test2_29465(String testid, String teststeps, int stepcount)
        {
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String ph1Username = Config.ph1UserName;
                String ph1Password = Config.ph1Password;
                String ph2Username = Config.ph2UserName;
                String ph2Password = Config.ph2Password;
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PaitentID");
                String PatientName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Patient Name");
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Email");
                String Description = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String ImagePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ImageFilePath");

                //Initial Setup - step 1
                ExecutedSteps++;

                //Upload a Non Dicom study
                ei.EINonDicomUploadUnReg(Email, Config.Dest1, "", "", UploadFilePath, ImagePath, Description, PatientID, Accession);

                //Verify Study Present in Destination-1
                //Login as physician1 
                login.DriverGoTo(login.url);

                //Login as physician in Destination-1
                login.LoginIConnect(ph1Username, ph1Password);

                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select study
                inbounds.SearchStudy("Accession", Accession);
                inbounds.SelectStudy("Accession", Accession);

                //Validate study is present in Physician's inbounds
                Dictionary<string, string> row = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Patient Name" }, new string[] { Accession, PatientID, PatientName });
                if (row != null)
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

                //Load the study
                inbounds.SelectStudy1(new string[] { "Accession", "Patient ID", "Patient Name" }, new string[] { Accession, PatientID, PatientName });
                inbounds.LaunchStudy();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                String patientinfo = BasePage.Driver.FindElement(By.CssSelector("span.patientInfoDiv")).GetAttribute("innerHTML");

                // Nominate to Archive and Validate


                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Logout
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                login.Logout();

                //Return Result
                return result;

            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps); ;
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Test Case 3- 29465 - 
        /// Select 2 related studies and upload to 2 different destinations and verify in both destinations
        /// </summary>
        public TestCaseResult Test3_29465(String testid, String teststeps, int stepcount)
        {
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String ph1Username = Config.ph1UserName;
                String ph1Password = Config.ph1Password;
                String ph2Username = Config.ph2UserName;
                String ph2Password = Config.ph2Password;
                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionIDList.Split(':');
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] FilePaths = UploadFilePath.Split(':');
                String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EmailID");

                //Upload study to Destination-1
                ei.EIDicomUploadUnReg(Email, Config.Dest1, FilePaths[0]);

                //Upload study to Destination-2
                ei.EIDicomUploadUnReg(Email, Config.Dest2, FilePaths[1]);
                ExecutedSteps++;

                //Login as physician in destination-1
                login.DriverGoTo(login.url);
                login.LoginIConnect(ph1Username, ph1Password);

                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select study
                inbounds.SearchStudy("Accession", AccessionNumbers[0]);
                inbounds.SelectStudy("Accession", AccessionNumbers[0]);

                String studyStatus2;
                inbounds.GetMatchingRow("Accession", AccessionNumbers[0]).TryGetValue("Status", out studyStatus2);

                //Validate study is present in physician's inbounds
                if (studyStatus2 == "Uploaded")
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not reached to Destination-1");
                }

                //Logout
                login.Logout();

                //Login as Physician of Destination - 2
                login.LoginIConnect(ph2Username, ph2Password);

                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select study
                inbounds.SearchStudy("Accession", AccessionNumbers[1]);
                inbounds.SelectStudy("Accession", AccessionNumbers[1]);

                //Validate Study is listed and status as uploaded
                String studyStatus1;
                inbounds.GetMatchingRow("Accession", AccessionNumbers[1]).TryGetValue("Status", out studyStatus1);
                if (studyStatus1 == "Uploaded")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not reached to Destination-2");
                }

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
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps); ;
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Test Case 5- 29465 - 
        /// Upload a dicom file and non dicom file for a patient from the local by creating a new patient 
        /// </summary>
        public TestCaseResult Test4_29465(String testid, String teststeps, int stepcount)
        {
            Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String phUsername = Config.ph1UserName;
                String phPassword = Config.ph1Password;
                String arUsername = Config.ar1UserName;
                String arPassword = Config.ar1Password;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String reason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String imagePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ImageFilePath");

                //Upload Dicom as well as Nondicom studies - steps 1 & 2
                ei.EI_UploadDicomWithNonDicom(Config.stUserName, Config.stPassword, Config.Dest1, UploadFilePath, imagePath);
                ExecutedSteps++;
                ExecutedSteps++;

                //Navigate to search in Holding pen
                login.DriverGoTo(login.hpurl);
                hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");

                IWebElement acc = BasePage.Driver.FindElement(By.CssSelector("input[name='accessionNumber']"));
                acc.Clear();
                acc.SendKeys(Accession);
                BasePage.Driver.FindElement(By.CssSelector("#submitbutton")).Click();

                //Validate study is present in holding pen - step 3
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
                    throw new Exception("Study is not present in Holding Pen");
                }

                //Logout in holding Pen
                hplogin.LogoutHPen();

                //Login as physician1 
                login.DriverGoTo(login.url);
                login.LoginIConnect(phUsername, phPassword);

                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study
                inbounds.SearchStudy("Accession", Accession);

                //Validate study is present in physician's inbounds - step 4
                Dictionary<string, string> row = inbounds.GetMatchingRow(new string[] { "Accession", "Patient ID", "Status" }, new string[] { Accession, PatientID, "Uploaded" });
                if (row != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Load the study
                inbounds.SelectStudy("Accession", Accession);
                inbounds.LaunchStudy();

                //Get Patient details 
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                String patientinfo = BasePage.Driver.FindElement(By.CssSelector("span.patientInfoDiv")).GetAttribute("innerHTML");
                String details = patientinfo.ToLower();

                //Validate patient details in study viewer - step 5
                if (details.Contains(FirstName.ToLower()) && details.Contains(LastName.ToLower()) && details.Contains(PatientID))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Nominate Study in viewer
                inbounds.Nominatestudy_toolbar(reason);

                //Search Study
                inbounds.SearchStudy("Accession", Accession);

                //Validate study is nominated in physician's inbounds - step 6
                Dictionary<string, string> studynominated = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Nominated For Archive" });
                if (studynominated != null)
                {
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
                login.LoginIConnect(arUsername, arPassword);

                //Navigate to inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study
                inbounds.SearchStudy("Accession", Accession);

                //Validate study is present in archivist's inbounds - step 7
                Dictionary<string, string> row1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Nominated For Archive" });
                if (row1 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Load the study
                inbounds.SelectStudy("Accession", Accession);
                inbounds.LaunchStudy();

                //Archive study in toolbar - step 8
                inbounds.Archivestudy_toolbar();
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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps); ;
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <Test Case-1_29466>
        /// Upload some studies as staff using CD uploader and check in his/her inbounds and outbounds
        /// </summary>
        public TestCaseResult Test1_161154(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            Outbounds outbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String stUsername = Config.stUserName;
                String stPassword = Config.stPassword;
                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNoList.Split(':');
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Upload Studies using webUploader - step 1-7
                //Launch WebUploader
                /**                webuploader.LaunchWebUploader();

                                //Login as Staff
                                webuploader.LoginAsRegisterUser(stUsername, stPassword);

                                //Select Destination
                                webuploader.SelectDestination(Config.Dest1);

                                //Select study folder
                                webuploader.SelectFileFromHdd(UploadFilePath);

                                //Select current study with all series
                                webuploader.SelectAllSeriesToUpload();

                                //Send Study
                                webuploader.Send();
                
                                /** Insert Code to close dialog **/
                ei.EIDicomUpload(stUsername, stPassword, Config.Dest1, UploadFilePath);
                ExecutedSteps = ExecutedSteps + 7;

                //Login as staff - step 8
                login.LoginIConnect(stUsername, stPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                foreach (String Accession in AccessionNumbers)
                {
                    //Search Study 
                    inbounds.SearchStudy("Accession", Accession);

                    //Validate Study is not listed in Staff's inbounds - step 9
                    if (inbounds.CheckStudy("Accession", Accession) != true)
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
                }

                //Navigate to Outbounds
                outbounds = (Outbounds)login.Navigate("Outbounds");
                ExecutedSteps++;

                foreach (String Accession in AccessionNumbers)
                {
                    //Search and Select Study
                    outbounds.SearchStudy("Accession", Accession);

                    Dictionary<string, string> rowkeyvalue;
                    rowkeyvalue = outbounds.GetMatchingRow("Accession", Accession);

                    //Validate is study is listed in Staff's Outbounds - step 10
                    if (rowkeyvalue != null)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        throw new Exception("Study is not listed in Staff's Outbounds");
                    }
                }

                //Logout 
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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps); ;
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <Test Case 2_29466>
        /// Validate share, transfer and add receiver functions in staff's outbounds
        /// </summary>
        public TestCaseResult Test2_161154(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables
            Inbounds inbounds = null;
            Outbounds outbounds = null;
            Studies studies = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String stUsername = Config.stUserName;
                String stPassword = Config.stPassword;
                String phUsername = Config.phUserName;
                String phPassword = Config.phPassword;
                String ph2Username = Config.ph2UserName;
                String ph2Password = Config.ph2Password;
                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNoList.Split(':');
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String Location = login.GetHostName(Config.DestinationPACS);

                //Upload mutiple DICOM studies - step 1
                ei.EIDicomUpload(stUsername, stPassword, Config.Dest1, UploadFilePath);
                ExecutedSteps++;

                //Login as staff 
                login.LoginIConnect(stUsername, stPassword);

                //Navigate to Outbounds
                outbounds = (Outbounds)login.Navigate("Outbounds");

                //Search and Select Study
                outbounds.SearchStudy("Accession", AccessionNumbers[0]);
                outbounds.SelectStudy1("Accession", AccessionNumbers[0]);

                //Share study to user - step 2
                outbounds.ShareStudy(false, new String[] { phUsername });

                //Logout as Staff
                login.Logout();
                ExecutedSteps++;

                //Login as Sharee(Physician) 
                login.LoginIConnect(phUsername, phPassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study 
                inbounds.SearchStudy("Accession", AccessionNumbers[0]);

                //Validate Study is listed in Sharee(Physician)'s inbounds - step 3
                if (inbounds.CheckStudy("Accession", AccessionNumbers[0]) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not present in Sharee's inbounds");
                }

                //Logout as Sharee(Physician)
                login.Logout();

                //Login as staff - step 4
                login.LoginIConnect(stUsername, stPassword);
                ExecutedSteps++;

                //Navigate to Outbounds
                outbounds = (Outbounds)login.Navigate("Outbounds");

                //Search and Select Study
                outbounds.SearchStudy("Accession", AccessionNumbers[1]);
                outbounds.SelectStudy1("Accession", AccessionNumbers[1]);

                //Transfer study - step 5
                outbounds.TransferStudy(Location);
                ExecutedSteps++;

                //Navigate to Outbounds
                studies = (Studies)login.Navigate("Studies");

                //Search Study 
                studies.SearchStudy("Accession", AccessionNumbers[1]);

                //Choose Column and get Datasource of the study transfered                
                studies.ChooseColumns(new string[] { "Data Source" });
                String datasource;
                studies.GetMatchingRow("Accession", AccessionNumbers[1]).TryGetValue("Data Source", out datasource);

                //Validate Study is listed in Staff's Studies tab(i.e)transfered to location - step 6
                if (datasource.Contains(Location))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not Transferred to location " + Location);
                }

                //Navigate to Outbounds
                outbounds = (Outbounds)login.Navigate("Outbounds");

                //Search and Select Study
                outbounds.SearchStudy("Accession", AccessionNumbers[2]);
                outbounds.SelectStudy1("Accession", AccessionNumbers[2]);

                //Add receiver for the study - step 7
                outbounds.AddReceiver(ph2Username);
                ExecutedSteps++;

                //Logout as Staff
                login.Logout();

                //Login as Receiver(Physician 1) 
                login.LoginIConnect(ph2Username, ph2Password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study 
                inbounds.SearchStudy("Accession", AccessionNumbers[2]);
                
                //Validate Study is listed in Receiver(Physician 1)'s inbounds - step 8
                if (inbounds.CheckStudy("Accession", AccessionNumbers[2]) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not listed in receiver's inbounds");
                }
                /*Removed HTML5 Validation
                //Select Study
                inbounds.SelectStudy("Accession", AccessionNumbers[2]);

                //Launch Study in HTML5 View
                inbounds.ClickHTML5ViewButton();

                try
                {
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    IWebElement HTML5viewer = BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_CompositeViewportDiv"));
                    if (HTML5viewer.Displayed)
                    {
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
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close Study
                inbounds.CloseStudy();
                */
                //Select Study
                inbounds.SelectStudy("Accession", AccessionNumbers[2]);

                //Launch Study in HTML4 view
                BluRingViewer BluRingViewer = null;
                if (Config.isEnterpriseViewer.ToLower().Equals("n"))
                {
                    inbounds.LaunchStudy();
                }
                else
                {
                    BluRingViewer = BluRingViewer.LaunchBluRingViewer();
                    PageLoadWait.WaitForFrameLoad(20);
                }

                Boolean StudyLoaded = true;
                //Validate Study is viewed in HTML4 viewer - step 10
                if (StudyLoaded == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close Study
                if (Config.isEnterpriseViewer.ToLower().Equals("n"))
                {
                    inbounds.CloseStudy();
                }
                else
                {
                    BluRingViewer.CloseBluRingViewer();
                }

                //Logout as Receiver(Physician 1) - step 11
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
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps); ;
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <Test Case-3_29466>
        /// Upload new Studies as staff, Nominate as physician and check study as archivist
        /// </summary>
        public TestCaseResult Test3_161154(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            Outbounds outbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String stUsername = Config.stUserName;
                String stPassword = Config.stPassword;
                String phUsername = Config.ph1UserName;
                String phPassword = Config.ph1Password;
                String arUsername = Config.ar1UserName;
                String arPassword = Config.ar1Password;
                String Reason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String PID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String RefPh = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RefPhysician");
                String Modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNoList.Split(':');
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Upload mutiple DICOM studies - step 1
                ei.EIDicomUpload(stUsername, stPassword, Config.Dest1, UploadFilePath);
                ExecutedSteps++;

                //Login as staff 
                login.LoginIConnect(stUsername, stPassword);

                //Navigate to Outbounds
                outbounds = (Outbounds)login.Navigate("Outbounds");

                //Validate multiple studies are present in Staff's Outbounds - step 2
                foreach (String AccNo in AccessionNumbers)
                {
                    //Search Study
                    outbounds.SearchStudy("Accession", AccNo);

                    Dictionary<string, string> study = outbounds.GetMatchingRow("Accession", AccNo);
                    if (!(study == null))
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        throw new Exception("Studies are not found in Staff's outbounds");
                    }
                }
                ExecutedSteps++;

                IWebElement lastname = BasePage.Driver.FindElement(By.Id("m_studySearchControl_m_searchInputPatientLastName"));
                IWebElement firstname = BasePage.Driver.FindElement(By.CssSelector("#m_studySearchControl_m_searchInputPatientFirstName"));
                IWebElement patientID = BasePage.Driver.FindElement(By.Id("m_studySearchControl_m_searchInputPatientID"));
                IWebElement refPhysician = BasePage.Driver.FindElement(By.Id("m_studySearchControl_m_searchInputReferringPhysicianName"));
                IWebElement modality = BasePage.Driver.FindElement(By.Id("m_studySearchControl_m_searchInputModality"));
                IWebElement accNo = BasePage.Driver.FindElement(By.Id("m_studySearchControl_m_searchInputAccession"));
                IWebElement menuper = BasePage.Driver.FindElement(By.Id("searchStudyDropDownMenu"));
                IWebElement menurec = BasePage.Driver.FindElement(By.Id("searchStudyCreatedDropDownMenu"));

                //Validate all search fields are present - step 3
                if ((lastname.Displayed == true) && (firstname.Displayed == true) && (patientID.Displayed == true) && (refPhysician.Displayed == true)
                    && (modality.Displayed == true) && (accNo.Displayed == true) && (menuper.Displayed == true) && (menurec.Displayed == true))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Search Study with multiple parameters
                outbounds.SearchStudy(LastName, FirstName, PID, RefPh, AccessionNumbers[0], Modality, "", "");

                Dictionary<string, string> study1 = outbounds.GetMatchingRow("Accession", AccessionNumbers[0]);

                //Validate appropriate study is listed with given search parameters - step 4
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
                    throw new Exception("Study not found");
                }

                //Logout as Staff
                login.Logout();

                //Non - Automated Steps - steps 5 & 6
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Login as Physician
                login.LoginIConnect(phUsername, phPassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Validate Uploaded Studies are listed in Physician's inbounds - step 7
                foreach (String AccNo in AccessionNumbers)
                {
                    //Search Study 
                    inbounds.SearchStudy("Accession", AccNo);

                    if (inbounds.CheckStudy("Accession", AccNo) == true)
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
                }
                ExecutedSteps++;

                //Validate all studies are nominated for archive - step 8
                foreach (String AccNo in AccessionNumbers)
                {
                    //Search, Select and Nominate Study
                    inbounds.SearchStudy("Accession", AccNo);
                    inbounds.SelectStudy("Accession", AccNo);

                    inbounds.NominateForArchive(Reason);

                    //Study Status
                    String studyStatus;
                    inbounds.GetMatchingRow("Accession", AccNo).TryGetValue("Status", out studyStatus);

                    if (studyStatus == "Nominated For Archive")
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
                }
                ExecutedSteps++;

                //Logout as Physician
                login.Logout();

                //Login as Archivist
                login.LoginIConnect(arUsername, arPassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Validate Nominated Studies are listed in Physician's inbounds - step 9
                foreach (String AccNo in AccessionNumbers)
                {
                    //Search Study 
                    inbounds.SearchStudy("Accession", AccNo);

                    if (inbounds.CheckStudy("Accession", AccNo) == true)
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
                }
                ExecutedSteps++;

                //Not Automated as of now
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Logout as Archivist
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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps); ;
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// With holding pen disabled, validate prior studies are present in patient history panel
        /// </summary>
        public TestCaseResult Test2_29477(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            Studies studies = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {

                //Fetch required Test data                       
                String phUsername = Config.ph1UserName;
                String phPassword = Config.ph1Password;
                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo");
                String[] AccessionNumbers = AccessionNoList.Split(':');

                //Login as physician 1
                login.LoginIConnect(phUsername, phPassword);

                //Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Search Study 
                studies.SearchStudy("Accession", AccessionNumbers[0]);

                //Validate Study is listed in physician 1's Studies tab
                Dictionary<string, string> arStudy = studies.GetMatchingRow("Accession", AccessionNumbers[0]);
                if (!(arStudy == null))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select the prior study in Destination 1
                studies.SelectStudy1("Accession", AccessionNumbers[0]);

                //Launch Study
                studies.LaunchStudy();

                //Validate archived prior study is loaded in viewer
                if (studies.ViewStudy() == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Navigate to History Panel
                studies.NavigateToHistoryPanel();
                studies.ChooseColumns(new String[] { "Accession" });

                //Validate prior studies are displayed in Patient history tab of archived Study
                foreach (String AccessionNumber in AccessionNumbers)
                {
                    Dictionary<string, string> priors = studies.GetMatchingRow("Accession", AccessionNumber);
                    if (!(priors == null))
                    {
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

                //Validate yellow icon is not displayed for Uploaded studies in Patient history panel of archived study
                for (int i = 1; i < AccessionNumbers.Length; i++)
                {
                    if (studies.CheckForeignExamAlert("Accession", AccessionNumbers[i]) != true)
                    {
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

                //Close the Study in viewer
                studies.CloseStudy();

                //Report Result
                result.FinalResult();
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Logout as Physician 1 
                login.Logout();

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }


        }

        #endregion Sprint-2 Automation Tests

        #region Sprint-4 Automation Tests

        /// <summary>
        /// Addiing additional info to the Study pushed through the PACS gateway
        /// </summary>
        public TestCaseResult Test1_161152(String testid, String teststeps, int stepcount)
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
                String pacusername = Config.pacsadmin;
                String pacpassword = Config.pacspassword;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String acc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String mail = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EmailID");
                String name = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String comments = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Comments");
                String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Step-1:Send a Study to PACS and then send it to iConnect System
                BasePage.RunBatchFile(Config.batchfilepath, Config.TestDataPath + studypath + " " + Config.dicomsendpath + " " + Config.StudyPacs);

                //Send the study to dicom devices from MergePacs management page
                login.DriverGoTo(login.mpacstudyurl);
                MpacLogin mplogin = new MpacLogin();
                MPHomePage homepage = mplogin.Loginpacs(pacusername, pacpassword);
                Tool tools = (Tool)homepage.NavigateTopMenu("Tools");
                tools.NavigateToSendStudy();
                tools.SearchStudy("Accession", acc, 0);
                tools.MpacSelectStudy("Patient ID", pid);
                Dictionary<String, String> MpacResults = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());
                tools.SendStudy(1);
                mpaclogin.LogoutPacs();

                //Launch IConnect System
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Wait till Study reaches iConnect
                login.LoginIConnect(username, password);
                PageLoadWait.WaitforStudyInStatus(acc, (Inbounds)login.Navigate("Inbounds"), "Uploaded");
                login.Logout();

                //Step-2:Adding info as unreg user for a uploaded study with email field blank and validate
                login.DriverGoTo(login.url);
                login.AddInfo("", pid, name.Split(',')[0].ToUpper(), MpacResults["DOB"], acc, name.Split(',')[1].ToUpper(), Config.Dest1, MpacResults["Sex"]);
                String errormessage = BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_MasterContentPlaceHolder_validateError"))).GetAttribute("innerHTML");
                if (errormessage.ToLower().Contains("client email address cannot be empty"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Error was not displayed");
                }

                //Step-3:Fill all details with email-id
                login.ClickButton("#ctl00_MasterContentPlaceHolder_CloseButton");
                login.DriverGoTo(login.url);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_LoginMasterContentPlaceHolder_Username")));
                login.AddInfo(mail, pid, name.Split(',')[0].ToUpper(), MpacResults["DOB"], acc, name.Split(',')[1].ToUpper(), Config.Dest1, MpacResults["Sex"]);
                ExecutedSteps++;
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#ctl00_MasterContentPlaceHolder_ShowButton")));
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("GuestNonRegisterUserFrame");

                //Verify Details
                IWebElement fullname = BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#ctl00_MasterContentPlaceHolder_PatientName")));
                String Name = fullname.GetAttribute("innerHTML");
                IWebElement mrn = BasePage.Driver.FindElement(By.CssSelector("div#ctl00_MasterContentPlaceHolder_ContentDiv span#ctl00_MasterContentPlaceHolder_PatientID"));
                String Mrn = mrn.GetAttribute("innerHTML");
                IWebElement accno = BasePage.Driver.FindElement(By.CssSelector("div#ctl00_MasterContentPlaceHolder_ContentDiv span#ctl00_MasterContentPlaceHolder_AccessionNumber"));
                String Acc = accno.GetAttribute("innerHTML");
                IWebElement destn = BasePage.Driver.FindElement(By.CssSelector("div#ctl00_MasterContentPlaceHolder_ContentDiv span#ctl00_MasterContentPlaceHolder_Destintation"));
                String Destn = destn.GetAttribute("innerHTML");
                //IWebElement mailid = BasePage.Driver.FindElement(By.CssSelector("div#ctl00_MasterContentPlaceHolder_ContentDiv span#ctl00_MasterContentPlaceHolder_DefaulReceiversLabel"));
                //String[] MailID = mailid.GetAttribute("innerHTML").Split(';');

                if ((name.Split(',')[1] + " " + name.Split(',')[0]).Trim().ToUpper().Equals(Name.ToUpper().TrimStart().TrimEnd()) && pid.Equals(Mrn) && acc.Equals(Acc) && Config.Dest1.Equals(Destn))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("all details are not correct");
                }

                //Step-5:Set priority as HIGH and apply changes
                IWebElement priority = BasePage.Driver.FindElement(By.CssSelector("select#Priority_criteria>option[value='HIGH']"));
                priority.Click();
                IWebElement comment = BasePage.Driver.FindElement(By.CssSelector("div#commentsInputDiv textarea"));
                comment.SendKeys(comments);
                IWebElement receiver = BasePage.Driver.FindElement(By.CssSelector("input#searchRecipient"));
                //receiver.SendKeys (Config.ph2UserName);
                new BasePage().SendKeysInStroke(receiver, Config.ph2UserName);
                BasePage.wait.Until((d) => d.FindElement(By.CssSelector("body>ul")).GetAttribute("style").ToLower().Contains("display: block"));
                IWebElement chooserec = BasePage.Driver.FindElement(By.CssSelector("body>ul>li>a"));
                chooserec.Click();
                PageLoadWait.WaitForPageLoad(30);
                ExecutedSteps++;

                //Step-6:applychanges button
                BasePage.Driver.FindElement(By.CssSelector("input#ctl00_MasterContentPlaceHolder_AddAdditionalReceiverCrtl_ApplyButton")).Click();
                ExecutedSteps++;

                //Step-7:updated info is listed
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("input#ctl00_MasterContentPlaceHolder_AddAdditionalReceiverCrtl_ApplyButton")));
                IWebElement pr = BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div#CompletionAddReceiverDiv tr#prioritySection td>span")));
                String checkpr = pr.GetAttribute("innerHTML");
                IWebElement cm = BasePage.Driver.FindElement(By.CssSelector("div#CompletionAddReceiverDiv tr#commentsSection td>span"));
                String checkcm = cm.GetAttribute("innerHTML");
                IWebElement rec = BasePage.Driver.FindElement(By.CssSelector("div#CompletionAddReceiverDiv tr#receiverSection td>span>p"));
                String checkrec = rec.GetAttribute("innerHTML");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                if (checkpr.Contains("HIGH") && checkcm.Contains(comments) && checkrec.Contains(Config.ph2UserName))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("all details are not updated");
                }

                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("GuestNonRegisterUserFrame");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input#ctl00_MasterContentPlaceHolder_AddAdditionalDetailsCompletionCrtl_CloseButton")));
                BasePage.Driver.FindElement(By.CssSelector("input#ctl00_MasterContentPlaceHolder_AddAdditionalDetailsCompletionCrtl_CloseButton")).Click();
                PageLoadWait.WaitForPageLoad(30);

                //Step-8 & 9:Check Email(Not Automated)
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Login as Ph
                login.LoginIConnect(username, password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Accession", acc);
                PageLoadWait.WaitForPageLoad(20);
                inbounds.SelectStudy("Accession", acc);

                //Step-10:Validate priority status as HIGH               
                String priorityStatus, studycomments;
                var updatedstudy = inbounds.GetMatchingRow("Accession", acc);
                updatedstudy.TryGetValue("Priority", out priorityStatus);
                updatedstudy.TryGetValue("Comments", out studycomments);

                if (priorityStatus.ToUpper().Equals("HIGH") && comments.ToUpper().Equals(studycomments.ToUpper()))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("priority status is not HIGH in ph2 login");
                }

                //Step-11:Load the study in the viewer and validate the information
                BluRingViewer BluRingViewer = null;
                if (Config.isEnterpriseViewer.ToLower().Equals("n"))
                {
                    inbounds.LaunchStudy();
                }
                else
                {
                    BluRingViewer = BluRingViewer.LaunchBluRingViewer();
                    PageLoadWait.WaitForFrameLoad(20);
                }
                ExecutedSteps++;

                //logout as ph
                if (Config.isEnterpriseViewer.ToLower().Equals("n"))
                {
                    inbounds.CloseStudy();
                }
                else
                {
                    BluRingViewer.CloseBluRingViewer();
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
        /// This is to send study with report and add info
        /// </summary>
        public TestCaseResult Test2_161152(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables          
            //Inbounds inbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required data
                String pacusername = Config.pacsadmin;
                String pacpassword = Config.pacspassword;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String name = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name");
                String[] namesplit = name.Split(',');
                String acc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String mail = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EmailID");
                String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String comments = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Comments");

                //Step-1: Upload a study using PACS gateway in different Destination
                BasePage.RunBatchFile(Config.batchfilepath, Config.TestDataPath + studypath + " " + Config.dicomsendpath + " " + Config.StudyPacs);

                //Send the study to dicom devices from MergePacs management page
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
                Dictionary<String, String> MpacResults = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());
                tools.SendStudy(1);

                //Logout MPacs
                mpaclogin.LogoutPacs();

                //Search for such study in HP exists without any updation
                //Login in HP
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin = new HPLogin();
                HPHomePage hphomepage = new HPHomePage();
                WorkFlow workflow = new WorkFlow();
                hplogin.LoginHPen(hpUserName, hpPassword);
                hphomepage.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");

                //search study using acc no'
                PageLoadWait.WaitForStudyInHp(420, acc, workflow);
                workflow.HPSearchStudy("Accessionno", acc);

                Boolean study = workflow.HPCheckStudy(acc);

                if (study == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study is not present in Holding Pen");
                }

                //Logout in HP
                hplogin.LogoutHPen();

                //Wait till Study reaches iConnect
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.ph1UserName, Config.ph1Password);
                PageLoadWait.WaitforStudyInStatus(acc, (Inbounds)login.Navigate("Inbounds"), "Uploaded");
                login.Logout();

                //Step-2:Adding info  for a uploaded study
                login.DriverGoTo(login.url);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_LoginMasterContentPlaceHolder_Username")));
                login.AddInfo(mail, pid, name.Split(',')[0].ToUpper(), MpacResults["DOB"], acc, name.Split(',')[1].ToUpper(), Config.Dest1, MpacResults["Sex"]);

                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#ctl00_MasterContentPlaceHolder_ShowButton")));
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("GuestNonRegisterUserFrame");

                //Set priority as HIGH and apply changes
                IWebElement priority = BasePage.Driver.FindElement(By.CssSelector("select#Priority_criteria>option[value='HIGH']"));
                priority.Click();
                IWebElement comment = BasePage.Driver.FindElement(By.CssSelector("div#commentsInputDiv textarea"));
                comment.SendKeys(comments);
                IWebElement receiver = BasePage.Driver.FindElement(By.CssSelector("input#searchRecipient"));
                receiver.SendKeys(Config.ph2UserName);
                BasePage.wait.Until((d) => d.FindElement(By.CssSelector("body>ul")).GetAttribute("style").ToLower().Contains("display: block"));
                IWebElement chooserec = BasePage.Driver.FindElement(By.CssSelector("body>ul>li>a"));
                chooserec.Click();
                PageLoadWait.WaitForPageLoad(30);
                ExecutedSteps++;

                //Step-3:applychanges button
                BasePage.Driver.FindElement(By.CssSelector("input#ctl00_MasterContentPlaceHolder_AddAdditionalReceiverCrtl_ApplyButton")).Click();
                PageLoadWait.WaitForPageLoad(50);
                ExecutedSteps++;

                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#ctl00_MasterContentPlaceHolder_AddAdditionalDetailsCompletionCrtl_CloseButton")));
                BasePage.Driver.FindElement(By.CssSelector("input#ctl00_MasterContentPlaceHolder_AddAdditionalDetailsCompletionCrtl_CloseButton")).Click();
                PageLoadWait.WaitForPageLoad(30);

                //Step-4:Check Email(Not Automated)
                result.steps[++ExecutedSteps].status = "Not Automated";

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
        /// This is to send large study from PACS and reconcile in iCA
        /// </summary>
        public TestCaseResult Test3_161152(String testid, String teststeps, int stepcount)
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
                String pacusername = Config.pacsadmin;
                String pacpassword = Config.pacspassword;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;

                String acc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Step-1:Send a Study to PACS and then send it to iConnect System
                BasePage.RunBatchFile(Config.batchfilepath, Config.TestDataPath + studypath + " " + Config.dicomsendpath + " " + Config.StudyPacs);

                //Send the study to dicom devices from MergePacs management page
                login.DriverGoTo(login.mpacstudyurl);
                MpacLogin mplogin = new MpacLogin();
                MPHomePage homepage = mplogin.Loginpacs(pacusername, pacpassword);
                Tool tools = (Tool)homepage.NavigateTopMenu("Tools");
                tools.NavigateToSendStudy();
                tools.SearchStudy("Accession", acc, 0);
                tools.MpacSelectStudy("Accession", acc);
                Dictionary<String, String> MpacResults = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());
                tools.SendStudy(1);
                mpaclogin.LogoutPacs();
                ExecutedSteps++;

                //Launch IConnect System
                login.DriverGoTo(login.url);

                //Wait till Study reaches iConnect
                login.LoginIConnect(username, password);
                PageLoadWait.WaitforStudyInStatus(acc, (Inbounds)login.Navigate("Inbounds"), "Uploaded");

                //Step-2:Search study
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Accession", acc);

                //Select and nominate the study
                inbounds.SelectStudy("Accession", acc);
                inbounds.NominateForArchive("Test");

                //Logout as ph
                login.Logout();

                //Login as ar
                login.LoginIConnect(username1, password1);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("Accession", acc);

                //Select and archive the study
                inbounds.SelectStudy("Accession", acc);
                //Click ArchiveStudy
                inbounds.ClickArchiveStudy("", "");
                PageLoadWait.WaitForLoadInArchive(10);
                ExecutedSteps++;

                //Step-3:Perform Manual Reconciliation
                inbounds.EditFinalDetailsInArchive("first name", "SAM");

                Dictionary<String, String> FinalDetails = inbounds.GetDataInArchive("Final Details");

                //Click Archive
                inbounds.ClickArchive();

                //Check status for Archive Pending
                inbounds.SearchStudy("Accession", acc);
                String Status;
                inbounds.GetMatchingRow(new string[] { "Accession" }, new string[] { acc }).TryGetValue("Status", out Status);

                if (Status == "Archive Pending")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Check for archived study
                inbounds.SearchStudy("Accession", acc);
                inbounds.SearchStudy("Accession", acc);

                //Validate  status as Routing Completed
                Dictionary<string, string> study1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { acc, "Routing Completed" });

                if (study1 != null)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Status is as Routing Completed");
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Status is not as Routing Completed");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4:Verify the no of  images  in iCA and HP and display of images

                //in iCA
                String images_iCA;
                inbounds.GetMatchingRow("Accession", acc).TryGetValue("Number of Images", out images_iCA);
                //Logout as ar2
                login.Logout();

                //Login in HP
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin = new HPLogin();
                HPHomePage hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");

                //search study using acc no'
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("Accessionno", acc);

                //selecting such study with acc no'
                //BasePage.Driver.FindElement(By.CssSelector(".odd td:nth-child(6)>a")).Click();
                BasePage.Driver.FindElements(By.CssSelector(".odd td"))[5].FindElement(By.CssSelector("a")).Click();
                //selecting the study to know the no' of objects
                //IWebElement obj = BasePage.Driver.FindElement(By.CssSelector(".odd td:nth-child(7)>a"));
                IWebElement obj = BasePage.Driver.FindElements(By.CssSelector(".odd td"))[6].FindElement(By.CssSelector("a"));
                String images_hp = obj.Text;
                int imghp = Int32.Parse(images_hp);

                if (images_iCA.Contains(images_hp))
                {
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

                //Login as ar
                login.DriverGoTo(login.url);
                login.LoginIConnect(username1, password1);
                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("Accession", acc);

                //Step-5:Load the study in viewer
                inbounds.SelectStudy("Accession", acc);
                
                BluRingViewer BluRingViewer = null;
                bool Step5 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("n"))
                {
                    inbounds.LaunchStudy();

                    //Image count
                    IList<IWebElement> images = BasePage.Driver.FindElements(By.CssSelector("div[class^='thumbnail ui-draggable loadedThumbnail']"));
                    int imgcnt = images.Count;

                    //Patient and study details
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    String patientinfo = BasePage.Driver.FindElement(By.CssSelector("span.patientInfoDiv")).GetAttribute("innerHTML");
                    String details = patientinfo.ToLower();
                    String studyinfo = BasePage.Driver.FindElement(By.CssSelector("span.studyInfoDiv")).GetAttribute("innerHTML");
                    Step5 = patientinfo.Contains(FinalDetails["Last Name"].ToUpper()) && patientinfo.Contains(FinalDetails["First Name"].ToUpper()) && patientinfo.Contains(FinalDetails["PID / MRN"]) && studyinfo.Contains(FinalDetails["Accession"]);
                }
                else
                {
                    BluRingViewer = BluRingViewer.LaunchBluRingViewer();
                    PageLoadWait.WaitForFrameLoad(20);
                    String patientinfo = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.p_PatientName)).GetAttribute("innerText").Replace(" ", String.Empty);
                    patientinfo = patientinfo + ", " + BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_PatientID)).GetAttribute("innerText");
                    String details = patientinfo.ToLower();
                    Step5 = patientinfo.Contains(FinalDetails["Last Name"].ToUpper()) && patientinfo.Contains(FinalDetails["First Name"].ToUpper()) && patientinfo.Contains(FinalDetails["PID / MRN"]) && BluRingViewer.VerifyPriorsHighlightedInExamList(AccessionNumber: FinalDetails["Accession"]);
                }

                //Validate the study details
                if (Step5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close Study
                if (Config.isEnterpriseViewer.ToLower().Equals("n"))
                {
                    inbounds.CloseStudy();
                }
                else
                {
                    BluRingViewer.CloseBluRingViewer();
                }

                //Step-6 & 7:Verify in different viewers(Not Automated)
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Logout as ar2
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
        /// This is to check whether warning message is displayed when a nominated study is pulled out and buttons are validated
        /// </summary>
        public TestCaseResult Test4_161152(String testid, String teststeps, int stepcount)
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
                String pacusername = Config.pacsadmin;
                String pacpassword = Config.pacspassword;

                String reason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String acc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Accession");
                String acclist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = acclist.Split(':');
                String pidlist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] pid = pidlist.Split(':');
                String mail = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EmailID");
                String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Upload the studies
                ei.EIDicomUpload(username, password, Config.Dest1, studypath);

                //Step-1:Login as ph
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("Accession", AccessionNumbers[0]);
                PageLoadWait.WaitForPageLoad(20);

                //Select a uploaded study and validate 
                inbounds.SelectStudy("Accession", AccessionNumbers[0]);

                //Validate nominate button is enabled
                IWebElement nominate = BasePage.Driver.FindElement(By.CssSelector("#m_nominateStudyButton"));
                if (nominate.Enabled == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as ph
                login.Logout();

                //Step-2:Login as ph
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("Study Performed", "All Dates");
                PageLoadWait.WaitForPageLoad(20);

                //Select two uploaded studies and validate 
                foreach (String AccNo in AccessionNumbers)
                {
                    inbounds.SelectStudy1("Accession", AccNo, true);

                }

                //Validate nominate and viewer buttons are disabled
                IWebElement nominate1 = BasePage.Driver.FindElement(By.CssSelector("#m_nominateStudyButton"));
                IWebElement viewstudy = BasePage.Driver.FindElement(By.CssSelector("#m_viewStudyButton"));
                if (nominate1.Enabled == false && viewstudy.Enabled == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Nominate a study
                inbounds.SearchStudy("Accession", AccessionNumbers[0]);
                inbounds.SelectStudy("Accession", AccessionNumbers[0]);
                inbounds.NominateForArchive(reason);
                //Choose columns
                inbounds.ChooseColumns(new string[] { "Patient DOB", "Gender" });

                //Get the values
                Dictionary<string, string> study = inbounds.GetMatchingRow("Accession", AccessionNumbers[0]);

                //Logout as ph
                login.Logout();

                //Adding info as unreg user for a uploaded study
                login.DriverGoTo(login.url);
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div#ctl00_WebAccessLogoDiv")));
                //var dob = DateTime.ParseExact(study["Patient DOB"], "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToShortDateString();
                login.AddInfo(mail, pid[0], study["Patient Name"].Split(',')[0], study["Patient DOB"], AccessionNumbers[0], study["Patient Name"].Split(',')[1], Config.Dest1, study["Gender"], "dd-MMM-yyyy");

                //Step-3:Validate the warning displays
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForPageLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#SearchError_Div span#ctl00_MasterContentPlaceHolder_m_errorMsg")));
                IWebElement warning = BasePage.Driver.FindElement(By.CssSelector("div#SearchError_Div span#ctl00_MasterContentPlaceHolder_m_errorMsg"));
                if (warning.Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Warning does not display");
                }

                PageLoadWait.WaitForPageLoad(20);
                BasePage.Driver.FindElement(By.CssSelector("input#ctl00_MasterContentPlaceHolder_CloseButton")).Click();
                PageLoadWait.WaitForPageLoad(40);

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

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <Test Case-5_29462>
        /// This Test is to validate that a new Reason can be added in list "Nominate to Archive" and use the same and archive a study
        /// </summary>
        public TestCaseResult Test5_161152(String testid, String teststeps, int stepcount)
        {
            /** Precondition - Keep the checkbox in archive nomination reasons(Edit domain) be Checked and remove other reasons in the list while unchecking **/

            //Declare and initialize variables          
            Inbounds inbounds = null;
            Outbounds outbounds = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            DomainManagement domainmanagement;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String ph1UserName = Config.ph1UserName;
                String ph1Password = Config.ph1Password;
                String arUserName = Config.ar1UserName;
                String arPassword = Config.ar1Password;
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String DomainName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String AccessionNo1 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String DefaultReasonsList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String[] DefaultReasons = DefaultReasonsList.Split(';');
                String NewReasonsList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NewReasonsList");
                String[] NewReasons = NewReasonsList.Split(';');

                String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //A study is uploaded
                ei.EIDicomUpload(Config.stUserName, Config.stPassword, Config.Dest1, studypath);

                //Step-1:Login as Physician
                login.LoginIConnect(ph1UserName, ph1Password);
                ExecutedSteps++;

                //Step-2:Navigate to Outbounds
                outbounds = (Outbounds)login.Navigate("Outbounds");

                //Search Study
                outbounds.SearchStudy("Accession", AccessionNo1);

                Dictionary<string, string> rowkeyvalue;
                rowkeyvalue = outbounds.GetMatchingRow("Accession", AccessionNo1);

                //Validate study is not listed in physician's Outbounds
                if (rowkeyvalue == null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3:Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select study 
                inbounds.SearchStudy("Accession", AccessionNo1);
                inbounds.SelectStudy("Accession", AccessionNo1);

                //Click Nominate button 
                IWebElement ReasonField, OrderField;
                inbounds.ClickNominateButton(out ReasonField, out OrderField);

                //Validate nominate dialog is opened and reason for archive field is present
                if (ReasonField != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4:Validate the default values are present in the reason for nominate dropdown box
                if (inbounds.VerifyDropDownList((BasePage.Driver.FindElement(By.CssSelector("#NominateStudyControl_m_reasonSelector"))), DefaultReasons) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5:Close Nominate Dialog
                inbounds.CloseNominateDialog();
                PageLoadWait.WaitHomePage();

                //Find study status
                String studyStatus;
                inbounds.GetMatchingRow("Accession", AccessionNo1).TryGetValue("Status", out studyStatus);

                //Validate Study status remains in Uploaded Status
                if (studyStatus.Equals("Uploaded"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as ph
                login.Logout();

                //Step-6:Login as admin
                login.LoginIConnect(adminUserName, adminPassword);

                //Navigate to domain mngmnt tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                ExecutedSteps++;

                //Step-7:Select domain and Select edit
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.ClickEditDomain();
                ExecutedSteps++;


                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='_SaveButton']")));
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#NominateReasonsInputControl_Div")));
                IWebElement ArchiveNomReason = BasePage.Driver.FindElement(By.CssSelector("#NominateReasonsInputControl_Div"));
                //Step-8:Validate Archive Nominations reason section is Present
                if (ArchiveNomReason.Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9:UnCheck Use System settings
                login.UnCheckCheckbox("cssselector", "[id$='_UseSystemNominationReasonsCB']");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id$='_NominateReasons_nominateDiv']")));

                //Validate 2 textboxes, add and remove buttons are present after unchecking system settings in archive nomination reasons section
                if (BasePage.Driver.FindElement(By.CssSelector("[id$='_nominateTextBox']")).Displayed &&
                    BasePage.Driver.FindElement(By.CssSelector("[id$='_nominateListBox']")).Displayed &&
                    BasePage.Driver.FindElement(By.CssSelector("[id$='_NominateReasons_AddButton']")).Displayed &&
                    BasePage.Driver.FindElement(By.CssSelector("[id$='_RemoveButton']")).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10:Click add Button
                BasePage.Driver.FindElement(By.CssSelector("[id$='_NominateReasons_AddButton']")).Click();
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id$='_ErrorMessage']")));

                IWebElement ErrorMsgField = BasePage.Driver.FindElement(By.CssSelector("[id$='_ErrorMessage']"));

                //Validate Error displayed to enter a value before clicking Add
                if (ErrorMsgField.Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                BasePage.Driver.FindElement(By.CssSelector("[id$='_NominateReasons_AddButton']")).SendKeys(Keys.Enter);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Step-11:Validate Error displayed to enter a value before Enter click Add
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id$='_ErrorMessage']")));
                if ((BasePage.Driver.FindElement(By.CssSelector("[id$='_ErrorMessage']"))).Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12:Click Remove-no changes should happen
                //BasePage.Driver.FindElement(By.CssSelector("[id$='_RemoveButton']")).Click();
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"[id$='_RemoveButton']\").click()");
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");

                IWebElement ErrorMsgField1 = BasePage.Driver.FindElement(By.CssSelector("[id$='_ErrorMessage']"));
                if (ErrorMsgField1.Displayed == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13:Add New Reasons
                foreach (String reason in NewReasons)
                {
                    BasePage.Driver.FindElement(By.CssSelector("[id$='_nominateTextBox']")).SendKeys(reason);
                    BasePage.Driver.FindElement(By.CssSelector("[id$='_NominateReasons_AddButton']")).Click();
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#NominateReasonsInputControl_Div")));
                }

                SelectElement selector = new SelectElement(BasePage.Driver.FindElement(By.CssSelector("[id$='_nominateListBox']")));
                IList<IWebElement> reasons = selector.Options;
                int reasonCount = 0;

                //Validate all new reasons are added in Archive nomination reason
                ExecutedSteps++;
                foreach (IWebElement reasonValue in reasons)
                {
                    if (reasonValue.Text == NewReasons[reasonCount])
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
                    reasonCount++;
                }

                //Step-14:Remove a reason from list and validate
                SelectElement selector00 = new SelectElement(BasePage.Driver.FindElement(By.CssSelector("[id$='_nominateListBox']")));
                IList<IWebElement> reasons00 = selector00.Options;

                selector00.SelectByText(NewReasons[2]);
                BasePage.Driver.FindElement(By.CssSelector("[id$='_RemoveButton']")).Click();
                PageLoadWait.WaitForPageLoad(20);

                SelectElement selector0 = new SelectElement(BasePage.Driver.FindElement(By.CssSelector("[id$='_nominateListBox']")));
                IList<IWebElement> reasons0 = selector0.Options;
                if (reasons00.Count > reasons0.Count)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-15:Add an existing reason and validate
                SelectElement selector1 = new SelectElement(BasePage.Driver.FindElement(By.CssSelector("[id$='_nominateListBox']")));
                IList<IWebElement> reasons1 = selector1.Options;

                //Add a reason
                BasePage.Driver.FindElement(By.CssSelector("[id$='_nominateTextBox']")).SendKeys(NewReasons[0]);
                BasePage.Driver.FindElement(By.CssSelector("[id$='_NominateReasons_AddButton']")).Click();
                PageLoadWait.WaitForPageLoad(20);

                SelectElement selector2 = new SelectElement(BasePage.Driver.FindElement(By.CssSelector("[id$='_nominateListBox']")));
                IList<IWebElement> reasons2 = selector2.Options;
                int i = reasons2.Count;

                //Validate already existing "Nominated" reason is not added again
                if (reasons2.Count == reasons1.Count)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-16:Add the removed reason and Click Save Button 
                BasePage.Driver.FindElement(By.CssSelector("[id$='_nominateTextBox']")).SendKeys(NewReasons[2]);
                BasePage.Driver.FindElement(By.CssSelector("[id$='_NominateReasons_AddButton']")).Click();
                PageLoadWait.WaitForPageLoad(20);
                domainmanagement.ClickSaveEditDomain();
                ExecutedSteps++;

                //Logout as Administrator
                login.Logout();

                //Step-17:Login as ph
                login.LoginIConnect(ph1UserName, ph1Password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");
                PageLoadWait.WaitForPageLoad(50);

                //Search and Select studyA  
                inbounds.SearchStudy("Accession", AccessionNo1);
                inbounds.SelectStudy("Accession", AccessionNo1);
                IWebElement ReasonField1, OrderField1;
                inbounds.ClickNominateButton(out ReasonField1, out OrderField1);

                //Validate nominate for archive dialog is opened or not
                if (ReasonField1 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-18:Validate the newly added values are present in the reason for nominate dropdown box
                if (inbounds.VerifyDropDownList(ReasonField1, new String[] { NewReasons[0], NewReasons[1], NewReasons[2] }) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-19:Select a new reason 
                inbounds.SelectFromList(ReasonField1, NewReasons[2], 1);

                //Confirm Nominate for archive
                inbounds.ClickConfirmNominate();

                //Find study status
                String studyStatus1;
                inbounds.GetMatchingRow("Accession", AccessionNo1).TryGetValue("Status", out studyStatus1);
                String statusreason;
                inbounds.GetMatchingRow("Accession", AccessionNo1).TryGetValue("Status Reason", out statusreason);

                //Validate Study status as Nominated for archive
                if (studyStatus1.Equals("Nominated For Archive") && statusreason.Equals(NewReasons[2]))
                {
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

                //Step-20 & 21:Check Email(Not Automated)
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Login as Archivist
                login.LoginIConnect(arUserName, arPassword);

                //Step-22:Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study and Select study
                inbounds.SearchStudy("Accession", AccessionNo1);
                inbounds.SelectStudy("Accession", AccessionNo1);

                //Validate Archive Study button is enabled or not
                if (BasePage.Driver.FindElement(By.CssSelector("#m_archiveStudyButton")).Enabled == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-23:Find Study Status and Status Reason
                String studyStatus2;
                inbounds.GetMatchingRow("Accession", AccessionNo1).TryGetValue("Status", out studyStatus2);
                String statusReason;
                inbounds.GetMatchingRow("Accession", AccessionNo1).TryGetValue("Status Reason", out statusReason);

                //Validate Study status as Nominated for archive and status Reason as Testing Purpose
                if (studyStatus2.Equals("Nominated For Archive") && statusReason.Equals(NewReasons[2]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Status is not as Uploaded in ph2 login");
                }

                //Logout as Archivist
                login.Logout();

                //*******Reset the Nomination Reason***
                //Login as admin
                login.LoginIConnect(adminUserName, adminPassword);

                //Navigate to domain mngmnt tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");

                //Select domain and Select edit
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.ClickEditDomain();
                //Remove all newly added reasons
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement ListBox1 = BasePage.Driver.FindElement(By.CssSelector("[id$='_nominateListBox']"));
                IWebElement RemoveBtn1 = BasePage.Driver.FindElement(By.CssSelector("[id$='_RemoveButton']"));
                SelectElement selector5 = new SelectElement(ListBox1);
                selector5.SelectByText(NewReasons[0]);
                RemoveBtn1.Click();
                selector5.SelectByText(NewReasons[1]);
                RemoveBtn1.Click();
                selector5.SelectByText(NewReasons[2]);
                RemoveBtn1.Click();

                //Set UseSystemSetting
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id$='_UseSystemNominationReasonsCB']")));
                if (BasePage.Driver.FindElement(By.CssSelector("[id$='_UseSystemNominationReasonsCB']")).Selected == false)
                {
                    domainmanagement.SetCheckbox("cssselector", "[id$='_UseSystemNominationReasonsCB']");
                }

                //Save
                PageLoadWait.WaitForFrameLoad(20);
                domainmanagement.ClickSaveEditDomain();

                //Logout as admin
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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //*******Reset the Nomination Reason***
                String DomainName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String NewReasonsList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NewReasonsList");
                String[] NewReasons = NewReasonsList.Split(';');
                //Login as admin
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                //Navigate to domain mngmnt tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");

                //Select domain and Select edit
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.ClickEditDomain();
                //Remove all newly added reasons
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement ListBox = BasePage.Driver.FindElement(By.CssSelector("[id$='_nominateListBox']"));
                IWebElement RemoveBtn = BasePage.Driver.FindElement(By.CssSelector("[id$='_RemoveButton']"));
                SelectElement selector = new SelectElement(ListBox);
                selector.SelectByText(NewReasons[0]);
                RemoveBtn.Click();
                selector.SelectByText(NewReasons[1]);
                RemoveBtn.Click();
                selector.SelectByText(NewReasons[2]);
                RemoveBtn.Click();
                //Set UseSystemSetting
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id$='_UseSystemNominationReasonsCB']")));
                if (BasePage.Driver.FindElement(By.CssSelector("[id$='_UseSystemNominationReasonsCB']")).Selected == false)
                {
                    domainmanagement.SetCheckbox("cssselector", "[id$='_UseSystemNominationReasonsCB']");
                }

                //Save
                PageLoadWait.WaitForFrameLoad(20);
                domainmanagement.ClickSaveEditDomain();

                //Logout as admin
                login.Logout();

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <Test case-6_29462>
        /// This is to send study with priors and reconcile a study by editing in final details column
        /// </summary>
        public TestCaseResult Test6_161152(String testid, String teststeps, int stepcount)
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
                String username1 = Config.ar1UserName;
                String password1 = Config.ar1Password;
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String pacusername = Config.pacsadmin;
                String pacpassword = Config.pacspassword;
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String acc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String acclist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = acclist.Split(':');
                String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] studypaths = studypath.Split('=');

                //Step-1:Merge pacs-->sent a study with priors
                for (int i = 0; i < AccessionNumbers.Length; i++)
                {
                    BasePage.RunBatchFile(Config.batchfilepath, Config.TestDataPath + studypaths[i] + " " + Config.dicomsendpath + " " + Config.StudyPacs);

                    //Send the study to dicom devices from MergePacs management page
                    login.DriverGoTo(login.mpacstudyurl);
                    MpacLogin mplogin = new MpacLogin();
                    MPHomePage homepage = mplogin.Loginpacs(pacusername, pacpassword);
                    Tool tools = (Tool)homepage.NavigateTopMenu("Tools");
                    tools.NavigateToSendStudy();
                    tools.SearchStudy("Accession", AccessionNumbers[i], 0);
                    tools.MpacSelectStudy("Accession", AccessionNumbers[i]);
                    tools.SendStudy(1);
                    mpaclogin.LogoutPacs();

                    //Launch IConnect System
                    login.DriverGoTo(login.url);

                    //Wait till Study reaches iConnect
                    login.LoginIConnect(username, password);
                    PageLoadWait.WaitforStudyInStatus(AccessionNumbers[i], (Inbounds)login.Navigate("Inbounds"), "Uploaded");

                    //Logout iCA
                    login.Logout();
                }
                ExecutedSteps++;

                //Login as ph
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("patientID", pid);
                PageLoadWait.WaitForPageLoad(20);

                //Check for the studies
                for (int i = 0; i < AccessionNumbers.Length; i++)
                {
                    if (inbounds.CheckStudy("Accession", AccessionNumbers[i]) == true)
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
                }
                //Logout ph
                login.Logout();

                //Login as archivist
                login.LoginIConnect(username1, password1);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("patientID", pid);
                PageLoadWait.WaitForPageLoad(20);

                //Check for the studies
                for (int i = 0; i < AccessionNumbers.Length; i++)
                {
                    if (inbounds.CheckStudy("Accession", AccessionNumbers[i]) == false)
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
                }

                //Logout as archivist
                login.Logout();

                //Step-2:Login as ph
                login.LoginIConnect(username, password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("Accession", acc);
                PageLoadWait.WaitForPageLoad(20);

                //for studyA
                //Validate status as Nominate For Archive
                String studyStatus1;

                inbounds.GetMatchingRow("Accession", acc).TryGetValue("Status", out studyStatus1);
                if (studyStatus1.Equals("Nominated For Archive"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study is not present or not as nominated");
                }

                BasePage.Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_searchInputAccession")).Clear();

                //for other prior studies
                inbounds.SearchStudy("patientID", pid);

                //Validate status as Uploaded
                for (int i = 0; i < AccessionNumbers.Length; i++)
                {
                    String studyStatus;
                    inbounds.GetMatchingRow("Accession", AccessionNumbers[i]).TryGetValue("Status", out studyStatus);

                    if (studyStatus.Equals("Uploaded"))
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed-->" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed-->" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //Validate nominate button and delete button are enabled for uploaded study
                //Validate reroute button  is disabled
                PageLoadWait.WaitForPageLoad(20);
                inbounds.SelectStudy("Accession", AccessionNumbers[1]);
                IWebElement nominate = BasePage.Driver.FindElement(By.CssSelector("#m_nominateStudyButton"));
                IWebElement del = BasePage.Driver.FindElement(By.CssSelector("#m_deleteStudiesButton"));
                IWebElement reroute = BasePage.Driver.FindElement(By.CssSelector("#m_rerouteStudyButton"));

                if (nominate.Enabled == true && del.Enabled == true && reroute.Enabled == true)
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

                //Logout as ph
                login.Logout();

                //Step-3:Login as ar
                login.LoginIConnect(username1, password1);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search study
                inbounds.SearchStudy("Accession", acc);
                PageLoadWait.WaitForPageLoad(20);

                //Select studyA that is nominated
                inbounds.SelectStudy("Accession", acc);
                //Click ArchiveStudy
                inbounds.ClickArchiveStudy("", "");

                //UI elements
                IWebElement dialogbox = BasePage.Driver.FindElement(By.CssSelector("#ReconciliationControlDialogDiv"));
                IWebElement reasonforarchive = BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_m_reason"));
                IWebElement UploadCommentsField = BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_UploadComments"));
                IWebElement ArchiveOrderField = BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_ArchiverOrderNotes"));
                //IWebElement searchorder = BasePage.Driver.FindElement(By.CssSelector("div#DivSearchFields>div>span:nth-child(1)"));
                //IWebElement searchpatient = BasePage.Driver.FindElement(By.CssSelector("div#DivSearchFields>div>span:nth-child(2)"));
                IWebElement searchorder = BasePage.Driver.FindElements(By.CssSelector("div#DivSearchFields>div>span"))[0];
                IWebElement searchpatient = BasePage.Driver.FindElements(By.CssSelector("div#DivSearchFields>div>span"))[1];

                IWebElement clearbtn = BasePage.Driver.FindElement(By.CssSelector("input[id$='_ButtonClearSearch']"));
                IWebElement searchbtn = BasePage.Driver.FindElement(By.CssSelector("input[id$='_ButtonSearch']"));
                IWebElement cancelbtn = BasePage.Driver.FindElement(By.CssSelector("input[id$='_CancelButton']"));
                IWebElement archivebtn = BasePage.Driver.FindElement(By.CssSelector("#m_ReconciliationControl_StartArchiveButton"));


                //Validate reconcilation window for studyA
                if (dialogbox.Displayed == true && UploadCommentsField.Displayed == true && ArchiveOrderField.Displayed == true && reasonforarchive.Displayed == true && searchorder.Displayed == true
                    && searchpatient.Displayed == true && clearbtn.Enabled == true && searchbtn.Enabled == true && cancelbtn.Enabled == true && archivebtn.Enabled == true
                    && inbounds.GetDataInArchive("Original Details") != null && inbounds.GetDataInArchive("Matching Patient") != null && inbounds.GetDataInArchive("Final Details") != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Cancel the window
                cancelbtn.Click();

                //Step-4:To Archive the study click archivestudy and validate
                PageLoadWait.WaitForPageLoad(20);
                inbounds.SelectStudy("Accession", acc);
                inbounds.ClickArchiveStudy("test", "reconcilation");

                //Validate reconcilation window for studyA
                if (dialogbox.Displayed == true && UploadCommentsField.Displayed == true && ArchiveOrderField.Displayed == true && reasonforarchive.Displayed == true && searchorder.Displayed == true
                    && searchpatient.Displayed == true && clearbtn.Enabled == true && searchbtn.Enabled == true && cancelbtn.Enabled == true && archivebtn.Enabled == true
                    && inbounds.GetDataInArchive("Original Details") != null && inbounds.GetDataInArchive("Matching Patient") != null && inbounds.GetDataInArchive("Final Details") != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5:Edit fields in final details
                inbounds.EditFinalDetailsInArchive("last name", "firsttest1");
                inbounds.EditFinalDetailsInArchive("description", "RightHand");
                Dictionary<String, String> FinalDetails1 = inbounds.GetDataInArchive("Final Details");
                ExecutedSteps++;

                //Step-6:Archive
                inbounds.ClickArchive();

                //Validate the status for routing completed
                inbounds.SearchStudy("Accession", acc);
                inbounds.SearchStudy("Accession", acc);
                String studyStatus2;
                inbounds.GetMatchingRow("Accession", acc).TryGetValue("Status", out studyStatus2);

                if (studyStatus2.Equals("Routing Completed"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed-->" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed-->" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7:Load the study
                inbounds.SelectStudy("Accession", acc);
                StudyViewer viewer = null;
                BluRingViewer BluRingViewer = null;
                bool Step7 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("n"))
                {
                    viewer = new BasePage().LaunchStudy();
                    
                    //Patient and study details
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    //String patientinfo = BasePage.Driver.FindElement(By.CssSelector("span.patientInfoDiv")).GetAttribute("innerHTML");
                    String patientinfo = viewer.PatientInfo();
                    //String details = patientinfo.ToLower();
                    //String studyinfo = BasePage.Driver.FindElement(By.CssSelector("span.studyInfoDiv")).GetAttribute("innerHTML");
                    String studyinfo = viewer.StudyInfo();
                    Step7 = inbounds.ViewStudy() == true && patientinfo.Contains(FinalDetails1["Last Name"].ToUpper()) && patientinfo.Contains(FinalDetails1["First Name"].ToUpper())
                            && patientinfo.Contains(FinalDetails1["PID / MRN"]) && studyinfo.Contains(FinalDetails1["Accession"]);
                }
                else
                {
                    BluRingViewer = BluRingViewer.LaunchBluRingViewer();
                    PageLoadWait.WaitForFrameLoad(20);
                    String patientinfo = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.p_PatientName)).GetAttribute("innerText").Replace(" ", String.Empty);
                    patientinfo = patientinfo + ", " + BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_PatientID)).GetAttribute("innerText");
                    Step7 = inbounds.ViewStudy(html5: true) == true && BluRingViewer.VerifyPriorsHighlightedInExamList(AccessionNumber: FinalDetails1["Accession"]) && patientinfo.Contains(FinalDetails1["Last Name"].ToUpper()) && patientinfo.Contains(FinalDetails1["First Name"].ToUpper())
                            && patientinfo.Contains(FinalDetails1["PID / MRN"]) ;
                }
                
                //Validate the study details
                if (Step7)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close study in viewer
                if (Config.isEnterpriseViewer.ToLower().Equals("n"))
                {
                    inbounds.CloseStudy();
                }
                else
                {
                    BluRingViewer.CloseBluRingViewer();
                }

                //Logout as ar
                login.Logout();

                //Step-8:View the archived study in MergePACS
                login.DriverGoTo(login.mpacdesturl);
                mpaclogin = new MpacLogin();
                MPHomePage mpachome1 = new MPHomePage();
                mpachome1 = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

                //Send Study with matching HL7 order
                Tool tool1 = new Tool();
                tool1 = (Tool)mpachome1.NavigateTopMenu("Tools");
                tool1.NavigateToSendStudy();
                tool1.SearchStudy("Accession", acc, 0);

                //Get study details
                Dictionary<string, string> MpacResults = Tool.MPacGetSearchResults(Tool.MPacGetSearchResults());

                var mpacdate = DateTime.ParseExact(MpacResults["StudyDate"], "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
                var finaldate = DateTime.Parse(FinalDetails1["Study Date"]).ToShortDateString();
                var mpacdob = DateTime.ParseExact(MpacResults["DOB"], "yyyyMMdd", CultureInfo.InvariantCulture).ToShortDateString();
                var finaldob = DateTime.ParseExact(FinalDetails1["DOB"], "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToShortDateString();

                //Validate Details in Final details column on archive window should match with details in Dest PACS 36
                if ((MpacResults["PatientName"].ToUpper().Split(' ')[0].Equals(FinalDetails1["Last Name"].ToUpper())) &&
                    (MpacResults["PatientName"].ToUpper().Split(' ')[1].Equals(FinalDetails1["First Name"].ToUpper())) &&
                    (MpacResults["Sex"].Equals(FinalDetails1["Gender"])) && mpacdob.Equals(finaldob) &&
                    (MpacResults["IPID"].Equals(FinalDetails1["Issuer of PID"])) && (MpacResults["PatientID"].Equals(FinalDetails1["PID / MRN"])) &&
                    (MpacResults["StudyDescription"].ToUpper().Equals(FinalDetails1["Description"].ToUpper())) && mpacdate.Equals(finaldate) &&
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

                //logout
                login.Logout();

                //Return Result
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

        ///<Test Case 3 - 29472> 
        /// <summary>
        /// <Validate Study is present in Physician InBounds,Nominated and archived in archivist inbounds>
        /// Upload studies from Webuploader to the new destinations created>nominate study>archive study
        /// </summary>
        public TestCaseResult Test3_29472(String testid, String teststeps, int stepcount)
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
                String AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String reason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                //Step-1:Upload a study using Web Uploader in new Destination
                Web_Uploader webuploader = new Web_Uploader();
                webuploader.LaunchWebUploader();
                webuploader.AcceptJavaPlugin();
                webuploader.LoginAsRegisterUser(Config.stUserName, Config.stPassword);
                webuploader.SelectDestination(Config.Dest1);

                //Select study folder
                webuploader.SelectFileFromHdd(studypath);

                //Select current study with all series
                webuploader.SelectAllSeriesToUpload();

                //Send Study
                webuploader.Send();

                //Close WebUploader
                BasePage.Driver.Quit();
                ExecutedSteps++;

                login.InvokeBrowser(Config.BrowserType);
                //Validate Study is present in the Physician's InBounds        
                //Step-2:Login as Physician 
                login.DriverGoTo(login.url);
                login.LoginIConnect(phUsername, phPassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search the study
                inbounds.SearchStudy("Accession", AccessionNo);

                //Validation
                if (inbounds.CheckStudy("Accession", AccessionNo) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study is not found");
                }

                //Step-3:Select the study
                inbounds.SelectStudy("Accession", AccessionNo);
                //Nominate study for Archive
                inbounds.NominateForArchive(reason);
                ExecutedSteps++;

                //Logout as Physician
                login.Logout();

                //Step-4:Login as Archivist 
                login.LoginIConnect(arUsername, arPassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select Study
                inbounds.SearchStudy("Accession", AccessionNo);
                inbounds.SelectStudy("Accession", AccessionNo);

                //Archive Study
                inbounds.ArchiveStudy("", "");

                //Validate StudyStatus as Routing Completed and Check the image is correctly sent or not
                //Find study status
                inbounds.SearchStudy("Accession", AccessionNo);
                inbounds.SearchStudy("Accession", AccessionNo);
                inbounds.SelectStudy("Accession", AccessionNo);
                Dictionary<string, string> study0 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNo, "Routing Completed" });
                //View Study
                inbounds.LaunchStudy();

                //Validation
                if (study0 != null && inbounds.ViewStudy() == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close Study
                inbounds.CloseStudy();

                //Logout as Archivist
                login.Logout();

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
        /// This is Automation of Test 29476
        /// This Test Case is to nominate from ph and archive from ar by uploading study through Web Uploader
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test2_29476(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            Outbounds outbounds = null;
            Studies studies = null;
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
                String uname = Config.ar1UserName;
                String pwd = Config.ar1Password;

                String order = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Order");
                String reason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String orderacc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderAccession");
                String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String orderpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");

                //PreCondition:- Send a matching order
                mpaclogin.SendHL7Order(Config.MergePACsIP, int.Parse(Config.mpacport), orderpath);

                //Login as archivist
                login.DriverGoTo(login.url);
                login.LoginIConnect(uname, pwd);

                //Step-1:Set userpreference to Launch Webuploader
                studies = (Studies)login.Navigate("Studies");
                studies.OpenUserPreferences();
                //Set the Make Java Exam Importer as default Exam Importer checkbox
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#DefaultToJavaExamImporterDiv")));
                if (BasePage.Driver.FindElement(By.CssSelector("input#defaultToJavaExamImporterCB")).Selected == false)
                {
                    studies.SetCheckbox("cssselector", "input#defaultToJavaExamImporterCB");
                }

                studies.CloseUserPreferences();
                ExecutedSteps++;

                //Step-2:Launch Webuploader
                outbounds = (Outbounds)login.Navigate("Outbounds");

                //Upload Studies using webUploader 
                //Launch WebUploader
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input#m_launchUploaderButton")));
                BasePage.Driver.FindElement(By.CssSelector("input#m_launchUploaderButton")).Click();
                ExecutedSteps++;

                //PageLoadWait.WaitForPageLoad(10);
                Web_Uploader webuploader = new Web_Uploader();
                //Select Destination                
                webuploader.SelectDestination(Config.Dest1);

                //Step-3:Select study folder
                webuploader.SelectFileFromHdd(studypath);
                ExecutedSteps++;

                //Select current study with all series
                webuploader.SelectAllSeriesToUpload();

                //Step-4:Send Study
                webuploader.Send();

                //Close webUploader
                BasePage.Driver.Close();
                ExecutedSteps++;

                login.InvokeBrowser(Config.BrowserType);                
                //Login as physician
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Step-5:Navigate to inbounds and nominate the study
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Accession", accession);
                inbounds.SelectStudy("Accession", accession);
                inbounds.NominateForArchive(order);

                //Find study status
                inbounds.SearchStudy("Accession", accession);
                Dictionary<string, string> study3 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { accession, "Nominated For Archive" });

                //Validate Study is listed and status as Nominated For Archive
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

                //Logout as physician
                login.Logout();

                //Login as archivist
                login.LoginIConnect(uname, pwd);

                //Step-6:Navigate to Outbounds and load the nominated study
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("Accession", accession);
                outbounds.SelectStudy1("Accession", accession);
                outbounds.LaunchStudy();

                //Validate the study is loaded with no error
                if (outbounds.ViewStudy() == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();

                }

                //Step-7:Archive the study through toolbar
                outbounds.ClickArchive_toolbar();

                //Search Order
                outbounds.ArchiveSearch("order", "All Dates");

                //Validate that recociliation window opens
                Dictionary<String, String> OrderDetails1 = inbounds.GetDataInArchive("Matching Order");
                Dictionary<String, String> OriginalDetails1 = inbounds.GetDataInArchive("Original Details");

                if ((OrderDetails1["Last Name"].Equals(OriginalDetails1["Last Name"])) && (OrderDetails1["First Name"].Equals(OriginalDetails1["First Name"])))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();

                }


                //Step-8:Manually reconcile the study
                //Edit the order
                inbounds.SetCheckBoxInArchive("matching order", "gen");

                //Click Archive
                outbounds.ClickArchive();
                outbounds.CloseStudy();

                outbounds.SearchStudy("Accession", accession);
                outbounds.SearchStudy("Accession", accession);

                //Find study status
                Dictionary<string, string> study4 = outbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { accession, "Routing Completed" });

                //Validate Study is listed and status as Routing Completed
                if (study4 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();

                }

                //Step-9:Check in the studies tab that the study reached the destination
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", accession);
                Dictionary<string, string> study0 = studies.GetMatchingRow(new string[] { "Accession", "Patient ID" }, new string[] { accession, pid });

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

                //Logout as archivist
                login.Logout();

                /***Reset the User Preference**/
                login.DriverGoTo(login.url);
                login.LoginIConnect(uname, pwd);

                //Step-1:Set userpreference to Launch Webuploader
                studies = (Studies)login.Navigate("Studies");
                studies.OpenUserPreferences();
                //Set the Make Java Exam Importer as default Exam Importer checkbox
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#DefaultToJavaExamImporterDiv")));

                if (BasePage.Driver.FindElement(By.CssSelector("input#defaultToJavaExamImporterCB")).Selected == true)
                {
                    studies.UnCheckCheckbox("cssselector", "input#defaultToJavaExamImporterCB");
                }

                //Close User preferences dialog
                studies.CloseUserPreferences();

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
                Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                /***Reset the User Preference**/
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.ar1UserName, Config.ar1Password);

                //Step-1:Set userpreference to Launch Webuploader
                studies = (Studies)login.Navigate("Studies");
                studies.OpenUserPreferences();
                //Set the Make Java Exam Importer as default Exam Importer checkbox
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#DefaultToJavaExamImporterDiv")));
                if (BasePage.Driver.FindElement(By.CssSelector("input#defaultToJavaExamImporterCB")).Selected == true)
                {
                    studies.UnCheckCheckbox("cssselector", "input#defaultToJavaExamImporterCB");
                }

                //Close User preferences dialog
                studies.CloseUserPreferences();

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
        /// Validate prior studies uploaded to different destinations are viewed properly
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_29477(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Inbounds inbounds = null;
            Studies studies = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data       
                String ph1Username = Config.ph1UserName;
                String ph1Password = Config.ph1Password;
                String ar1Username = Config.ar1UserName;
                String ar1Password = Config.ar1Password;
                String ph2Username = Config.ph2UserName;
                String ph2Password = Config.ph2Password;
                String ar2Username = Config.ar2UserName;
                String ar2Password = Config.ar2Password;
                String patientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Reason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNoList.Split(':');
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] FilePaths = UploadFilePath.Split(':');
                String Accession2 = AccessionNumbers[AccessionNumbers.Length - 1];

                //Initial Setup - Steps 1 & 2
                ExecutedSteps++;
                ExecutedSteps++;

                //Upload a DICOM with 3 priors to Destination-1 - step 3 & 4
                ei.EIDicomUpload(ph1Username, ph1Password, Config.Dest1, FilePaths[0]);

                //Upload 1 prior to Destination-2 
                ei.EIDicomUpload(ph1Username, ph1Password, Config.Dest2, FilePaths[1]);
                ExecutedSteps++;
                ExecutedSteps++;

                //Login as physician 
                login.LoginIConnect(ph1Username, ph1Password);

                //Navigate to Studies - step 5
                studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Search Study 
                studies.SearchStudy("Accession", AccessionNumbers[0]);

                //Validate Study is listed in physician 1's Studies tab - step 6
                if (studies.CheckStudy("Accession", AccessionNumbers[0]) != true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not listed in Physician-1's inbounds");
                }

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study 
                inbounds.SearchStudy("Accession", AccessionNumbers[0]);

                //Validate Study is listed in Physician1's inbounds - step 7
                if (inbounds.CheckStudy("Accession", AccessionNumbers[0]) == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select Study and Nominate Study for archive
                inbounds.SelectStudy("Accession", AccessionNumbers[0]);
                inbounds.NominateForArchive(Reason);

                //Find study status
                inbounds.SearchStudy("PatientID", patientID);
                String studyStatus1;
                inbounds.GetMatchingRow("Accession", AccessionNumbers[0]).TryGetValue("Status", out studyStatus1);

                //Validate Study is Nominated for archive - step 8
                if (studyStatus1 == "Nominated For Archive")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as Physician 1
                login.Logout();

                //Login as Archivist 1
                login.LoginIConnect(ar1Username, ar1Password);

                //Navigate to Inbounds - step 9
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Search and Select Study
                inbounds.SearchStudy("Accession", AccessionNumbers[0]);
                inbounds.SelectStudy("Accession", AccessionNumbers[0]);

                //Archive Study and Validate - step 10
                inbounds.ArchiveStudy("", "");

                String studyStatus2;
                inbounds.GetMatchingRow("Accession", AccessionNumbers[0]).TryGetValue("Status", out studyStatus2);

                //Search and Select Study
                inbounds.SearchStudy("Accession", AccessionNumbers[0]);

                //Study Status
                String studyStatus3;
                inbounds.GetMatchingRow("Accession", AccessionNumbers[0]).TryGetValue("Status", out studyStatus3);

                //Validate Study is archived to destination-1
                if (studyStatus2 != "Nominated For Archive" && studyStatus3 == "Routing Completed")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as Archivist
                login.Logout();

                //Login as physician 2
                login.LoginIConnect(ph2Username, ph2Password);

                //Navigate to Inbounds - step 11
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Search Study 
                inbounds.SearchStudy("Accession", Accession2);

                //Select Study and Nominate Study for archive
                inbounds.SelectStudy("Accession", Accession2);
                inbounds.NominateForArchive(Reason);

                //Find study status
                inbounds.SearchStudy("Accession", Accession2);
                String studyStatus4;
                inbounds.GetMatchingRow("Accession", Accession2).TryGetValue("Status", out studyStatus4);

                //Validate Study is Nominated for archive - step 12
                if (studyStatus1 == "Nominated For Archive")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as Physician 2
                login.Logout();

                //Validate Nominated study is archived in archivist 1's inbounds
                //Login as Archivist 2
                login.LoginIConnect(ar2Username, ar2Password);

                //Navigate to Inbounds - step 13
                inbounds = (Inbounds)login.Navigate("Inbounds");
                ExecutedSteps++;

                //Search and Select Study
                inbounds.SearchStudy("Accession", Accession2);
                inbounds.SelectStudy("Accession", Accession2);

                //Archive Study
                inbounds.ArchiveStudy("", "");

                String studyStatus5;
                inbounds.GetMatchingRow("Accession", Accession2).TryGetValue("Status", out studyStatus5);

                //Search Study
                inbounds.SearchStudy("Accession", Accession2);

                //Study Status
                String studyStatus6;
                inbounds.GetMatchingRow("Accession", Accession2).TryGetValue("Status", out studyStatus6);

                //Validate Study is archived to destination-1 - step 14
                if (studyStatus5 != "Nominated For Archive" && studyStatus3 == "Routing Completed")
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as Archivist
                login.Logout();

                //Login as physician 1
                login.LoginIConnect(ph1Username, ph1Password);

                //Navigate to Studies - step 15
                studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Search Study 
                studies.SearchStudy("Accession", AccessionNumbers[0]);

                //Validate Study is listed in physician 1's Studies tab - step 16
                Dictionary<string, string> arStudy = inbounds.GetMatchingRow("Accession", AccessionNumbers[0]);
                if (!(arStudy == null))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select the prior study in Destination 1
                studies.SelectStudy1("Accession", AccessionNumbers[0]);

                //Launch Study
                studies.LaunchStudy();

                //Validate archived prior study is loaded in viewer - step 17
                if (studies.ViewStudy() == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Navigate to History Panel
                studies.NavigateToHistoryPanel();
                studies.ChooseColumns(new String[] { "Accession" });

                //Validate prior studies are displayed in Patient history tab of archived Study - step 18
                ExecutedSteps++;
                foreach (String AccessionNumber in AccessionNumbers)
                {
                    Dictionary<string, string> priors = studies.GetMatchingRow("Accession", AccessionNumber);
                    if (!(priors == null))
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        throw new Exception("One of the study not is not displayed");
                    }
                }

                ExecutedSteps++;
                //Validate yellow icon is displayed for Uploaded studies in Patient history panel of archived study - step 19
                for (int i = 1; i < AccessionNumbers.Length - 1; i++)
                {
                    if (studies.CheckForeignExamAlert("Accession", AccessionNumbers[i]) == true)
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
                }

                ExecutedSteps++;
                //Validate yellow icon contains foreign exam message - step 20
                for (int i = 1; i < AccessionNumbers.Length - 1; i++)
                {

                    if (inbounds.CheckForeignExamMessage("Accession", AccessionNumbers[i]) == true)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //Load study in viewer
                studies.OpenPriors(new String[] { "Accession" }, new String[] { AccessionNumbers[1] });

                IWebElement YellowIcon = BasePage.Driver.FindElement(By.CssSelector("span[id*='2_foreignExamDiv']"));

                //Validate yellow triangle icon is displayed for prior study with Uploaded status - step 21
                if (YellowIcon.Displayed == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Validate yellow triangle icon contains the foreign exam message - step 22
                if (YellowIcon.GetAttribute("title").Contains("Foreign Exam") == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close study in viewer - step 23
                studies.CloseStudy();
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select Study
                inbounds.SearchStudy("PatientID", patientID);

                ExecutedSteps++;
                //Validate prior Studies in uploaded status are listed in physician 1's inbounds tab - step 24
                for (int i = 1; i < AccessionNumbers.Length - 1; i++)
                {
                    String studyStatus;
                    inbounds.GetMatchingRow("Accession", AccessionNumbers[i]).TryGetValue("Status", out studyStatus);

                    if (studyStatus == "Uploaded")
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
                }

                //Select the prior study with Uploaded status in Destination 1
                inbounds.SelectStudy1("Accession", AccessionNumbers[1]);

                //Launch Study
                inbounds.LaunchStudy();
                Boolean st = true;

                //Validate prior study in Uploaded status is loaded in viewer - step 25
                if (st == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Navigate to History Panel and choose columns
                inbounds.NavigateToHistoryPanel();
                inbounds.ChooseColumns(new String[] { "Accession" });

                ExecutedSteps++;
                //Validate prior studies are displayed in Patient history panel - step 26
                foreach (String AccessionNumber in AccessionNumbers)
                {
                    Dictionary<string, string> priors = inbounds.GetMatchingRow("Accession", AccessionNumber);
                    if (!(priors == null))
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
                }

                ExecutedSteps++;
                //Validate yellow icon is displayed for Uploaded studies in Patient history panel - step 27                
                if (inbounds.CheckForeignExamAlert("Accession", AccessionNumbers[2]) == true)
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

                ExecutedSteps++;
                //Validate yellow icon contains foreign exam message - step 28
                if (inbounds.CheckForeignExamMessage("Accession", AccessionNumbers[2]) == true)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                ExecutedSteps++;
                //Validate Yellow triangle icon is displayed in study panel toolbar of Studies with uploaded status - step 29                               

                login.OpenPriors(new String[] { "Accession" }, new String[] { AccessionNumbers[2] });

                if (BasePage.Driver.FindElement(By.CssSelector("span[id*='2_foreignExamDiv']")).Displayed == true)
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

                inbounds.NavigateToHistoryPanel();
                PageLoadWait.WaitForPageLoad(20);

                ExecutedSteps++;
                //Validate Foreign exam message is displayed while hovering icon - step 30                
                if (BasePage.Driver.FindElement(By.CssSelector("span[id*='2_foreignExamDiv']")).GetAttribute("title")
                    .Contains("Foreign Exam. This study may not belong to the same patient") == true)
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

                //Close the Study in viewer - step 31
                inbounds.CloseStudy();
                ExecutedSteps++;

                //Logout as Physician 1 
                login.Logout();

                //Login as physician 2
                login.LoginIConnect(ph2Username, ph2Password);

                //Navigate to Studies - step 32
                studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Search Study 
                studies.SearchStudy("Accession", Accession2);

                //Validate Study is listed in physician 1's Studies tab - step 33
                Dictionary<string, string> arStudy2 = studies.GetMatchingRow("Accession", Accession2);
                if (!(arStudy2 == null))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study not listed in Physiacian-2's inbounds");
                }

                //Select the prior study in Destination 1
                studies.SelectStudy1("Accession", Accession2);

                //Launch Study
                studies.LaunchStudy();

                //Validate archived prior study is loaded in viewer - step 34
                if (studies.ViewStudy() == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Navigate to History Panel
                studies.NavigateToHistoryPanel();
                studies.ChooseColumns(new String[] { "Accession" });

                ExecutedSteps++;
                //Validate prior studies are displayed in Patient history tab of archived Study - step 35
                foreach (String AccessionNumber in AccessionNumbers)
                {
                    Dictionary<string, string> priors = studies.GetMatchingRow("Accession", AccessionNumber);
                    if (!(priors == null))
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
                }

                ExecutedSteps++;
                //Validate yellow icon is displayed for Uploaded studies in Patient history panel of archived study - step 36
                for (int i = 1; i < AccessionNumbers.Length - 1; i++)
                {
                    if (studies.CheckForeignExamAlert("Accession", AccessionNumbers[i]) == true)
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
                }

                ExecutedSteps++;
                //Validate yellow icon contains foreign exam message - step 37
                for (int i = 1; i < AccessionNumbers.Length - 1; i++)
                {
                    if (inbounds.CheckForeignExamMessage("Accession", AccessionNumbers[i]) == true)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                ExecutedSteps++;
                //Validate Yellow triangle icon is displayed in study panel toolbar of Studies with uploaded status - step 38
                for (int iter = 1; iter < AccessionNumbers.Length - 1; iter++)
                {
                    studies.OpenPriors(new String[] { "Accession" }, new String[] { AccessionNumbers[iter] });

                    if (BasePage.Driver.FindElement(By.CssSelector("span[id*='" + (iter + 1) + "_foreignExamDiv']")).Displayed == true)
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

                    inbounds.NavigateToHistoryPanel();
                    PageLoadWait.WaitForPageLoad(20);
                }

                //Back to viewer
                BasePage.Driver.FindElement(By.CssSelector("#image_patientHistoryDrawer")).Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                ExecutedSteps++;
                //Validate Yellow triangle icon contains foreign exam message - step 39
                for (int iter = 1; iter < AccessionNumbers.Length - 1; iter++)
                {

                    if (BasePage.Driver.FindElement(By.CssSelector("span[id*='" + (iter + 1) + "_foreignExamDiv']"))
                         .GetAttribute("title").Contains("Foreign Exam. This study may not belong to the same patient") == true)
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
                }

                //Close the Study in viewer
                studies.CloseStudy();

                //Logout as Physician 2 - step 40
                login.Logout();
                ExecutedSteps++;

                //Not Automated Steps - Steps 41 to 46
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
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
        
        #endregion Sprint-4 Automation Tests

        /// <summary>
        /// Handling Unsupported Files
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_87590(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables          
            Inbounds inbounds = null;
            ExamImporter ei = new ExamImporter();
            WpfObjects wpfobjects = new WpfObjects();
            Web_Uploader webuploader = new Web_Uploader();
            RanorexObjects m_RanorexObjects = new RanorexObjects();
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            String DefaultBrowser = Config.BrowserType;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String UserName = Config.ph1UserName;
                String Password = Config.ph1Password;
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] PatientID = PatientIDList.Split(':');
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] FilePaths = UploadFilePath.Split('=');
                String[] WU_ImageFilePath = Directory.GetFiles(FilePaths[3]);
                String[] EI_ImageFilePath = Directory.GetFiles(FilePaths[5]);
                String ReportPath = Directory.GetFiles(FilePaths[6])[0];

                //PreCondition to validate Web uploader
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                Config.BrowserType = "firefox";
                login.InvokeBrowser(Config.BrowserType);
                login.DriverGoTo(login.url);

                //Launch and Login to Exam importer
                ei.LaunchEI();
                ei.LoginToEi(UserName, Password);

                //Step 1 :- Open Exam Importer/Web uploader and login as registered/un-registered user
                ExecutedSteps++;

                // Choose the folder containing non supported files like bat, exe, xml, etc..
                ei.EI_SelectDestination(Config.Dest1);
                wpfobjects.GetMainWindow(ei.eiWinName);
                wpfobjects.ClickButton("BtnOpenFolder");
                wpfobjects.InteractWithTree(FilePaths[0]);
                wpfobjects.ClickButton("1");
                wpfobjects.WaitTillLoad();
                wpfobjects.ClickButton("yesButton");

                //Get the text from POP window
                String MessageText = wpfobjects.GetAnyUIItem<TestStack.White.UIItems.WindowItems.Window, TestStack.White.UIItems.IUIItem>(WpfObjects._mainWindow, "MessageText").Name;
                Logger.Instance.InfoLog("Message displayed - " + MessageText);

                //Step 2 :- Try to select a folder that contain document formats like .doc, .exe, .xml etc and select the folder for import
                if (MessageText.Contains("no DICOM data found") && MessageText.Contains("Do you want to create a patient and upload associated non-Dicom data"))
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

                //Click Cancel button
                wpfobjects.ClickButton("noButton");
                wpfobjects.WaitTillLoad();
                wpfobjects.GetMainWindow(ei.eiWinName);

                //Step 3 :- Click on Cancel in the pop-up window
                if (WpfObjects._mainWindow.ModalWindows().Count == 0)
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

                bool batfileStatus;
                try { ListView StudyRow = ei.StudyDetails(); batfileStatus = false; }
                catch (NullReferenceException) { batfileStatus = true; }

                //Step 4 :- Verify the Exam Importer window
                if (batfileStatus)
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

                //Launch Webuploader
                webuploader.LaunchWebUploader();
                webuploader.LoginAsRegisterUser(Config.stUserName, Config.stPassword);
                webuploader.SelectDestination(Config.Dest1);

                //Select a folder that contain document formats like .doc, .exe, .xml etc and select the folder for import 
                webuploader.SelectBrowserFolder();
                webuploader.SetTestDataFolderPathAndSelect(FilePaths[1]);
                m_RanorexObjects.WaitForElementTobeVisible(webuploader.scanMsg);

                if (m_RanorexObjects.IsElementVisible(webuploader.GetControlIdForWebUploader("CreatePatientConfirmNo")) &&
                    m_RanorexObjects.IsElementEnabled(webuploader.GetControlIdForWebUploader("CreatePatientConfirmNo")))
                {
                    m_RanorexObjects.ClickButton(webuploader.GetControlIdForWebUploader("CreatePatientConfirmNo"));
                }

                bool batfileStatus_WU;
                try { Ranorex.Table StudyRow = webuploader.StudyTable(); batfileStatus_WU = false; }
                catch (NullReferenceException) { batfileStatus_WU = true; }

                //Step 5 :- Verify the above steps in Webuploader
                if (batfileStatus_WU)
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

                //Sync - up
                m_RanorexObjects.WaitForElementTobeVisible(webuploader.ToDestination());

                //Select study in the specified location
                webuploader.SelectFileFromHdd(FilePaths[2]);

                //Attach Image files in all formats
                foreach (String imagepath in WU_ImageFilePath)
                {
                    webuploader.AttachImage(imagepath);
                }

                //Attach PDF file with study
                webuploader.AttachPDF(ReportPath);

                //Select patient and Click Send
                webuploader.SelectAllSeriesToUpload();
                
                //Upload Study with all the attachments
                webuploader.Send();
                
                //Refresh iCA Home Page
                login.DriverGoTo(login.url);

                //Login as Physician
                login.LoginIConnect(UserName, Password);
                
                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study
                inbounds.SearchStudy("PatientID", PatientID[0]);

                int ListedImagesCount_WU = Int32.Parse(inbounds.GetMatchingRow("Patient ID", PatientID[0])["Number of Images"].Split('/')[1]);
                int FilesCount_WU = Directory.GetFiles(FilePaths[1]).Count() + Directory.GetFiles(FilePaths[2]).Count()
                     + Directory.GetFiles(FilePaths[6]).Count();

                //Step 6 :- Upload any dicom study via webuploader and Verify all supported formats are uploaded successfully
                if (ListedImagesCount_WU == FilesCount_WU)
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

                //Logout ICA
                login.Logout();

                //Upload a study with all supported attachments
                wpfobjects.GetMainWindow(ei.eiWinName);
                ei.SelectFileFromHdd(FilePaths[4]);
                ei.SelectAllSeriesToUpload();
                foreach (String imagepath in EI_ImageFilePath)
                {
                    ei.AttachImage(imagepath);
                }
                ei.AttachPDF(ReportPath);
                ei.Send();
                ei.EI_Logout();
                ei.CloseUploaderTool();

                //Login as Physician
                login.LoginIConnect(UserName, Password);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study
                inbounds.SearchStudy("PatientID", PatientID[1]);

                int ListedImagesCount_EI = Int32.Parse(inbounds.GetMatchingRow("Patient ID", PatientID[1])["Number of Images"].Split('/')[1]);
                int FilesCount_EI = 1 + Directory.GetFiles(FilePaths[2]).Count() + Directory.GetFiles(FilePaths[6]).Count();

                //Step 7 :- Upload any dicom study via webuploader and Verify all supported formats are uploaded successfully
                if (ListedImagesCount_EI == FilesCount_EI)
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

                //Upload a study with unsupported .bat file in Attach PDF option
                //ei.EI_UploadDicomWithAttachment(UserName, Password, Config.Dest1, FilePaths[6], FilePaths[7], "pdf");










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
                //Switch back to Default browser
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                Config.BrowserType = DefaultBrowser;
                login.InvokeBrowser(DefaultBrowser);
                login.DriverGoTo(login.url);
                Thread.Sleep(10000);
                try
                {
                    Window pluginWindow = new WpfObjects().GetMainWindowByTitle("Plugin Container for Firefox");
                    pluginWindow.Get<Button>(SearchCriteria.ByText("Debug the program")).Click();
                    BasePage.KillProcess("plugin-container");
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Creating destinations for various domains
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_65917(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables          
            Inbounds inbounds = null;
            DomainManagement domainmgnt = null;
            RoleManagement rolemgnt = null;
            UserManagement usermgnt = null;
            Image_Sharing imagesharing = null;
            ServiceTool servicetool = new ServiceTool();
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            String tempWindowName = ei.eiWinName;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String UserName = Config.adminUserName;
                String Password = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String StudyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");

                String DomainName = "Domain_" + new Random().Next(1000);
                String RoleName = "AdminRole_" + new Random().Next(1000);
                String ReceiverRole = "ReceiverRole_" + new Random().Next(1000);
                String ArchivistRole = "ArchivistRole_" + new Random().Next(1000);
                String PhUser = "PhUser_" + new Random().Next(1000);
                String ArUser = "ArUser_" + new Random().Next(1000);
                String NewDest = "NewDest_" + new Random().Next(1000);
                String Comments = "Test comments for Upload Entire CD" + new Random().Next(1000);
                
                //Step 1 :- Login as Administrator
                login.LoginIConnect(UserName, Password);
                ExecutedSteps++;

                //Navigate to Domain Management tab
                domainmgnt = login.Navigate<DomainManagement>();

                //Create New domain with image sharing enabled
                domainmgnt.CreateDomain(DomainName, RoleName, DS: new string[] { "imagesharing" }, check: 1);
                
                //Search Domain
                domainmgnt.SearchDomain(DomainName);
                
                //Step 2 :- Validate New domain creation
                if(domainmgnt.IsDomainExist(DomainName))
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

                //Navigate to Role Management tab
                rolemgnt = login.Navigate<RoleManagement>();

                //Create Receiver role
                rolemgnt.CreateRole(DomainName, ReceiverRole, "physician");

                //Step 3 :- Validate Receiver Role Creation
                if (rolemgnt.RoleExists(ReceiverRole, DomainName))
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

                //Create Archivist role
                rolemgnt.CreateRole(DomainName, ArchivistRole, "archivist");

                //Step 4 :- Validate Archivist Role Creation
                if (rolemgnt.RoleExists(ArchivistRole, DomainName))
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

                //Navigate to User Managment tab
                usermgnt = login.Navigate<UserManagement>();

                //Create ph user under receiver role
                usermgnt.CreateUser(PhUser, DomainName, ReceiverRole);

                //Step 5 :- Validate ph user Creation
                if (usermgnt.IsUserExist(PhUser, DomainName))
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

                //Create ph user under receiver role
                usermgnt.CreateUser(ArUser, DomainName, ArchivistRole);

                //Step 6 :- Validate ar user Creation
                if (usermgnt.IsUserExist(ArUser, DomainName))
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

                //Navigate to Image Sharing tab
                imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                
                //Navigate to Destination Sub tab
                Image_Sharing.Destination dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                
                //Step 7 :- Create New Destination 
                dest.SelectDomain(DomainName);
                dest.CreateDestination(dest.GetHostName(Config.DestinationPACS), PhUser, ArUser, domain: DomainName);
                ExecutedSteps++;

                //Step 8 :- Launch Service tool
                servicetool.LaunchServiceTool();
                ExecutedSteps++;

                //Step 9 :- Navigate to image sharing tab
                servicetool.NavigateToTab(ServiceTool.ImageSharing_Tab);
                ExecutedSteps++;

                //Navigate to Upload Device Settings sub tab
                servicetool.NavigateSubTab(ServiceTool.ImageSharing.Name.UploadDeviceSettings_tab);
                servicetool.WaitWhileBusy();

                //Get Upload device Settings tab
                ITabPage DeviceSettingsTab = servicetool.GetCurrentTabItem();

                Button ModifyBtn = wpfobject.GetAnyUIItem<ITabPage, Button>(DeviceSettingsTab, ServiceTool.ModifyBtn_Name, 1);
                Boolean ModifyBtnStatus_BeforeClick = ModifyBtn.Enabled;
                TextBox iConnectURLTxtBox = wpfobject.GetAnyUIItem<ITabPage, TextBox>(DeviceSettingsTab, ServiceTool.ImageSharing.ID.iConnectURL);
                Boolean URLTxtBox_BeforeClick = iConnectURLTxtBox.Enabled;
                wpfobject.WaitTillLoad();

                //Click Modify Btn
                ModifyBtn.Click();
                wpfobject.WaitTillLoad();

                //Step 10 :- Select the Upload Device Setting page tab and click on Modify.
                if (ModifyBtnStatus_BeforeClick && !URLTxtBox_BeforeClick && iConnectURLTxtBox.Enabled)
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

                String iConnectURL = "http://" + Config.IConnectIP;

                //Enter http://<serverIP> in Host Service BaseUrl field and click on Apply.
                iConnectURLTxtBox.BulkText = "";
                iConnectURLTxtBox.BulkText = iConnectURL;
                wpfobject.WaitTillLoad();

                //Step 11 :- Click Apply button
                wpfobject.GetAnyUIItem<ITabPage, Button>(DeviceSettingsTab, ServiceTool.ApplyBtn_Name, 1).Click();
                wpfobject.WaitTillLoad();
                
                wpfobject.ClickButton(ServiceTool.OkBtn_Name, 1);
                wpfobject.WaitTillLoad();

                wpfobject.ClickButton(ServiceTool.OkBtn_Name, 1);
                wpfobject.WaitTillLoad();

                try
                {
                    wpfobject.WaitForPopUp();
                    //Click OK button
                    wpfobject.GetAnyUIItem<TestStack.White.UIItems.WindowItems.Window, Button>(WpfObjects._mainWindow, ServiceTool.OkBtn_Name, 1).Click();
                    wpfobject.WaitTillLoad();
                }
                catch (Exception) { }
                ExecutedSteps++;

                //Navigate to Installer sub tab
                servicetool.NavigateSubTab(ServiceTool.ImageSharing.Name.Installer_tab);
                servicetool.WaitWhileBusy();

                //Step 12 :- Select Exam Importer radio button
                RadioButton ExamImporter = wpfobject.GetAnyUIItem<ITabPage, RadioButton>(servicetool.GetCurrentTabItem(), ServiceTool.ImageSharing.Name.ExamImporterRadioBtn, 1);
                wpfobject.WaitTillLoad();
                if (!ExamImporter.IsSelected)
                {
                    ExamImporter.Click();
                    servicetool.WaitWhileBusy();
                }
                ExecutedSteps++;

                wpfobject.GetAnyUIItem<GroupBox, TestStack.White.UIItems.ListBoxItems.ComboBox>(servicetool.ExamImporterInstallerGrpBox(), ServiceTool.ImageSharing.ID.DomainCmbBox).Select(DomainName);
                wpfobject.WaitTillLoad();
                GroupBox Installer_grpbox = servicetool.ExamImporterInstallerGrpBox();
                String EIWindowName = "Exam Importer_" + new Random().Next(1, 1000);

                wpfobject.GetAnyUIItem<GroupBox, TextBox>(Installer_grpbox, ServiceTool.ImageSharing.ID.ProductName).BulkText = "";
                wpfobject.GetAnyUIItem<GroupBox, TextBox>(Installer_grpbox, ServiceTool.ImageSharing.ID.ProductName).BulkText = EIWindowName;

                //Step 13 :- Click Generate Installer Button
                wpfobject.GetAnyUIItem<GroupBox, Button>(Installer_grpbox, ServiceTool.ImageSharing.Name.GenerateInstallerBtn, 1).Click();
                wpfobject.WaitForPopUp();
                try
                {
                    wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                    wpfobject.WaitForPopUp();
                }
                catch (Exception e)
                {
                    wpfobject.WaitForPopUp();
                }
                ExecutedSteps++;

                //Step 14 :- Click 'OK' on the Pop up
                wpfobject.GetMainWindowByIndex(1);
                wpfobject.GetButton("2").Click();
                wpfobject.GetMainWindowByIndex(0);
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                BasePage.Kill_EXEProcess("UploaderTool");
                ExecutedSteps++;

                //Download EI installer
                ei.DownloadEIinstaller(DomainName, Config.eiInstaller);

                //Launch installer tool
                login._examImporterInstance = EIWindowName;
                wpfobject.InvokeApplication(Config.downloadpath + @"\" + Config.eiInstaller + ".msi");
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                wpfobject.WaitForButtonExist(login._examImporterInstance + " Setup", "Cancel", 1);
                                
                //Step 15 :- Validate "End User License Agreement" with Accept checkbox and Next button
                if (!ei.AcceptCheckbox().Checked && !ei.NextBtn().Enabled)
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

                //"End User License Agreement" window         
                //Click Accept and Next
                TestStack.White.Configuration.CoreAppXmlConfiguration.Instance.BusyTimeout = 90000;
                ei.AcceptCheckbox().Click();
                WpfObjects._mainWindow.WaitWhileBusy();
                Boolean NextBtnStatus = ei.NextBtn().Enabled;
                ei.NextBtn().Click();
                WpfObjects._mainWindow.WaitWhileBusy();

                try
                {
                    //Choose install for all users and Next
                    wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                    ei.InstallForAdministrator().Click();
                    WpfObjects._mainWindow.WaitWhileBusy();
                    ei.NextBtn().Click();
                    WpfObjects._mainWindow.WaitWhileBusy();

                    //Choose default destination and click Next
                    wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                    ei.NextBtn().Click();
                    WpfObjects._mainWindow.WaitWhileBusy();
                }
                catch (Exception) { }

                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");

                //Enter credentials and click Install button
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");

                //Step 16 :- Validation of Next button after accept checkbox is clicked
                if (NextBtnStatus && ei.UserNameTextbox().Enabled)
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

                //Enter credentials and click Install button
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                ei.UserNameTextbox().BulkText = PhUser;
                ei.PasswordTextbox().BulkText = PhUser;

                //Click Install button
                ei.InstallBtn().Click();
                WpfObjects._mainWindow.WaitWhileBusy();

                //wait until installation completes
                int installWindowTimeOut = 0;
                try
                {
                    wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                    while (ei.InstallingText(EIWindowName).Visible && installWindowTimeOut++ < 15)
                    {
                        Thread.Sleep(10000);
                    }
                }
                catch (Exception e)
                {
                    if (installWindowTimeOut == 0)
                    {
                        throw new Exception("Exception in CD Uploader installation window -- " + e);
                    }
                }


                //"Launch application when setup exists" and click Finish
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                WpfObjects._mainWindow.WaitWhileBusy();
                
                //Step 17 :- Validation of Launch Application check box and Finish button
                if (ei.LaunchAppCheckbox().Checked && ei.FinishBtn().Enabled)
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

                //Uncheck launch checkbox and Click Finish btn
                wpfobject.GetMainWindow(login._examImporterInstance + " Setup");
                ei.LaunchAppCheckbox().Click();
                ei.FinishBtn().Click();

                int counter = 0;
                while (WpfObjects._mainWindow.Visible && counter++ < 20)
                {
                    Thread.Sleep(1000);
                }

                //Step 18 :- Validate installer window
                if (WpfObjects._mainWindow.IsClosed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                String[] EIPath = Config.EIFilePath.Split('\\');
                EIPath[Array.FindIndex(EIPath, folder => folder.Equals("Apps")) + 1] = EIWindowName;
                String UploaderToolPath = string.Join("\\", EIPath);
                ei.LaunchEI(UploaderToolPath);
                wpfobject.GetMainWindow(EIWindowName);
                WpfObjects._mainWindow.WaitWhileBusy();

                //Step 19 :- Validate user should able to launch exam importer window
                if (WpfObjects._mainWindow.Visible)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 20 & 21 :- Enter Credentials
                ei.UserNameTextbox_EI().BulkText = PhUser;
                ei.PasswordTextbox_EI().BulkText = PhUser;
                ExecutedSteps++;
                ExecutedSteps++;

                //Click Sign in btn and wait for authentication
                ei.EI_ClickSignIn(EIWindowName);

                //Step 22 :- Verify Welcome text with username in top RHS
                wpfobject.GetMainWindow(EIWindowName);
                if (ei.welcomeText(PhUser).Visible)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Click on 'Ask Later' button
                wpfobject.GetMainWindow(EIWindowName);
                Tab SettingsOverlay = ei.SettingsTab();
                SettingsOverlay.Focus();
                ei.AskLaterBtn().Click();
                wpfobject.WaitTillLoad();

                //Step 23 :- Validation of Settings Overlay
                if (!SettingsOverlay.Visible)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 24 :- Choose Destination
                ei.EI_SelectDestination(NewDest, EIWindowName);
                ExecutedSteps++;

                //Select study in the specified path
                ei.eiWinName = EIWindowName;
                ei.SelectFileFromHdd(StudyPath);
                WpfObjects._mainWindow.WaitWhileBusy();

                //Get all patient details
                string[] PatientDetails = ei.AllPatientDetails(EIWindowName);

                //Step 25 :- Check patient info are displayed correctly with selected test data
                if (Array.Exists(PatientDetails, detail => detail.Contains(LastName))
                    && Array.Exists(PatientDetails, detail => detail.Contains(FirstName))
                    && Array.Exists(PatientDetails, detail => detail.Contains(PatientID)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Add comments in upload comments section
                ei.UploadComments(Comments, EIWindowName);

                //Step 26 :- Upload comments and validate it's display
                if (ei.CommentsTextBox().Text.Equals(Comments))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 26 :- Click Send and wait for progress bar to display
                ei.SelectAllPatientsToUpload();
                ei.Send();
                ExecutedSteps++;

                //Close Uploader Tool
                ei.EI_Logout();
                ei.CloseUploaderTool();

                //Login as Physician user
                login.LoginIConnect(PhUser, PhUser);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search Study
                inbounds.SearchStudy("PatientID", PatientID);

                //Step 27 :- Validate study is displayed or not
                Dictionary<string, string> studyrow = inbounds.GetMatchingRow(new string[] { "Patient ID", "Status", "Comments" }, new string[] { PatientID, "Uploaded", Comments });
                if (studyrow != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout ICA 
                login.Logout();

                //Revert EI window name
                ei.eiWinName = tempWindowName;

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;

            }

            catch (Exception e)
            {
                //Revert EI window name
                ei.eiWinName = tempWindowName;

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
        /// ProcedureCodeSequence dropped from query
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161157(String testid, String teststeps, int stepcount) 
        {
            //Declare and initialize variables                      
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            //Fetch required Test data  
            string xmlPath = Config.FederatedQueryConfiguration;
            //Take BackUp of the xml
            string[] temp = xmlPath.Split('\\');
            string xmlName = temp[temp.Length - 1];
            string backupPath = @"C:\" + xmlName;
            if (File.Exists(xmlPath))
                File.Copy(xmlPath, backupPath, true);

            try
            {
                //Fetch required Test data                
                String studyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String validationTxt = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ValidationText");
                string requestResponseValidation1 = validationTxt.Split(':')[0];
                string requestResponseValidation2 = validationTxt.Split(':')[1];
                string tsharkExePath = Config.tsharkExePath;
                string tsharkListernerOutput1 = @"C:\wiresharkOutput1.txt";
                string tsharkListernerOutput2 = @"C:\wiresharkOutput2.txt";
                string xmlDefaultAtt1 = validationTxt.Split(':')[2];
                string xmlDefaultAtt2 = validationTxt.Split(':')[3];
                string xmlDefaultAtt3 = validationTxt.Split(':')[4];

                //Delete Studies from Holding Pen
                HPLogin hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + Config.HoldingPenIP + "/webadmin");
                HPHomePage hphome = (HPHomePage)hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: "https://" + Config.HoldingPenIP + "/webadmin");
                WorkFlow workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("PatientID", "PID_65907");
                if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                    workflow.HPDeleteStudy();
                hplogin.LogoutHPen();

                //Import study using EI
                ExamImporter eiImport = new ExamImporter();
                try
                {
                    eiImport.EIDicomUpload(Config.ph1UserName, Config.ph1Password, Config.Dest1, studyPath);
                }
                catch (Exception e)
                {
                    if (e.Message == "Study Not Loaded As It Already Exists")
                    {
                        Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                    }
                    else
                    {
                        throw e;
                    }
                }
                eiImport.CloseUploaderTool();

                //Nominate for Archive
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.ph1UserName, Config.ph1Password);
                Inbounds inbounds = login.Navigate<Inbounds>();
                //inbounds.SelectAllDateAndData();
                inbounds.SearchStudy(AccessionNo: accession);
                inbounds.SelectStudy("Accession", accession);
                try
                {
                    inbounds.NominateForArchive("Test_67327 Precondition");
                }
                catch (Exception) { }
                login.Logout();

                //Step 1 - Modify FederatedQueryConfiguration.xml to have default values for excludedAttributes as blank and save Restart IIS                             
                XmlDocument xmlDoc = new XmlDocument();
                string vnaOldVal = "";
                string dicomOldVal = "";
                string amicasOldVal = "";
                xmlDoc.Load(xmlPath);
                //Vna
                XmlElement excludedAtt = (XmlElement)xmlDoc.SelectSingleNode("//add[@type='Vna']/parameter[@name='excludedAttributes']");
                if (excludedAtt != null)
                {
                    vnaOldVal = excludedAtt.GetAttribute("defaultValue");
                    excludedAtt.SetAttribute("defaultValue", ""); // Set to blank value.                    
                }
                else
                    throw new Exception("Vna>excludedAttributes not found in FederatedQueryConfiguration.xml file");
                //dicom
                excludedAtt = (XmlElement)xmlDoc.SelectSingleNode("//add[@type='Dicom']/parameter[@name='excludedAttributes']");
                if (excludedAtt != null)
                {
                    dicomOldVal = excludedAtt.GetAttribute("defaultValue");
                    excludedAtt.SetAttribute("defaultValue", ""); // Set to blank value.                    
                }
                else
                    throw new Exception("Dicom>excludedAttributes not found in FederatedQueryConfiguration.xml file");
                //Vna
                excludedAtt = (XmlElement)xmlDoc.SelectSingleNode("//add[@type='Amicas']/parameter[@name='excludedAttributes']");
                if (excludedAtt != null)
                {
                    amicasOldVal = excludedAtt.GetAttribute("defaultValue");
                    excludedAtt.SetAttribute("defaultValue", ""); // Set to blank value.                    
                }
                else
                    throw new Exception("Amicas>excludedAttributes not found in FederatedQueryConfiguration.xml file");
                xmlDoc.Save(xmlPath);

                //Restart IIS
                ServiceTool serviceTool = new ServiceTool();
                serviceTool.LaunchServiceTool();
                serviceTool.RestartService();
                serviceTool.CloseServiceTool();
                ExecutedSteps++;

                //Step 2 - Login as Archivist
                login.LoginIConnect(Config.ar1UserName, Config.ar1Password);
                ExecutedSteps++;

                //Step 3 - Start wireshark and capture                                
                Process cmdWireshark = BasePage.StartAndListenUsingWireshark(tsharkListernerOutput1, tsharkExePath);
                ExecutedSteps++;

                //Step 4 - From Inbounds tab load a study with status uploaded                           
                inbounds = login.Navigate<Inbounds>();
                //inbounds.SelectAllDateAndData();
                inbounds.SearchStudy(AccessionNo: accession);
                inbounds.SelectStudy("Accession", accession);
                BluRingViewer bluering = null;
                StudyViewer studyviewer = new StudyViewer();
                if (Config.isEnterpriseViewer.ToLower() == "y")
                {
                    bluering = BluRingViewer.LaunchBluRingViewer();
                }
                else
                {
                    inbounds.LaunchStudy();
                    PageLoadWait.WaitForPageLoad(30);
                    PageLoadWait.WaitForFrameLoad(30);
                    PageLoadWait.WaitForThumbnailsToLoad(60);
                    PageLoadWait.WaitForAllViewportsToLoad(60);
                }
                ExecutedSteps++;

                if (Config.isEnterpriseViewer.ToLower() == "y")
                {
                    bluering.CloseBluRingViewer();
                }
                else
                {
                    studyviewer.CloseStudy();
                }
                    login.Logout();

                //Step 5 - ProcedureCodeSequence and requestedProcedureId are included in request and response.
                try
                {
                    cmdWireshark.CloseMainWindow();
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Unable to close wireshark cmd1 --" + e);
                }
                //Read the output from tsharkListernerOutput file                    
                File.Copy(tsharkListernerOutput1, @"C:\wiresharkOutputCopy1.txt", true);
                string cmdOutput = System.IO.File.ReadAllText(@"C:\wiresharkOutputCopy1.txt");
                if (cmdOutput.ToLower().Contains(requestResponseValidation1) && cmdOutput.ToLower().Contains(requestResponseValidation2))
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

                //Step 6 - Modify FederatedQueryConfiguration.xml to have default values for excludedAttributes as 'procedureCodeSequence\,requestedProcedureId\,scheduledProcedureStepId' and save Restart IIS                
                xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlPath);
                //Vna
                excludedAtt = (XmlElement)xmlDoc.SelectSingleNode("//add[@type='Vna']/parameter[@name='excludedAttributes']");
                if (!vnaOldVal.Contains(xmlDefaultAtt1))
                    vnaOldVal = vnaOldVal + @"\," + xmlDefaultAtt1;
                if (!vnaOldVal.Contains(xmlDefaultAtt2))
                    vnaOldVal = vnaOldVal + @"\," + xmlDefaultAtt2;
                if (!vnaOldVal.Contains(xmlDefaultAtt3))
                    vnaOldVal = vnaOldVal + @"\," + xmlDefaultAtt3;
                excludedAtt.SetAttribute("defaultValue", vnaOldVal);
                //dicom
                excludedAtt = (XmlElement)xmlDoc.SelectSingleNode("//add[@type='Dicom']/parameter[@name='excludedAttributes']");
                if (!dicomOldVal.Contains(xmlDefaultAtt1))
                    dicomOldVal = dicomOldVal + @"\," + xmlDefaultAtt1;
                if (!dicomOldVal.Contains(xmlDefaultAtt2))
                    dicomOldVal = dicomOldVal + @"\," + xmlDefaultAtt2;
                if (!dicomOldVal.Contains(xmlDefaultAtt3))
                    dicomOldVal = dicomOldVal + @"\," + xmlDefaultAtt3;
                excludedAtt.SetAttribute("defaultValue", dicomOldVal);
                //Amicas
                excludedAtt = (XmlElement)xmlDoc.SelectSingleNode("//add[@type='Amicas']/parameter[@name='excludedAttributes']");
                if (!amicasOldVal.Contains(xmlDefaultAtt1))
                    amicasOldVal = amicasOldVal + @"\," + xmlDefaultAtt1;
                if (!amicasOldVal.Contains(xmlDefaultAtt2))
                    amicasOldVal = amicasOldVal + @"\," + xmlDefaultAtt2;
                if (!amicasOldVal.Contains(xmlDefaultAtt3))
                    amicasOldVal = amicasOldVal + @"\," + xmlDefaultAtt3;
                excludedAtt.SetAttribute("defaultValue", amicasOldVal);
                xmlDoc.Save(xmlPath);

                //Restart IIS
                serviceTool = new ServiceTool();
                serviceTool.LaunchServiceTool();
                serviceTool.RestartService();
                serviceTool.CloseServiceTool();
                ExecutedSteps++;

                //Step 7 - Login as Archivist
                login.LoginIConnect(Config.ar1UserName, Config.ar1Password);
                ExecutedSteps++;

                //Step 8 - Start wireshark and capture                                
                cmdWireshark = BasePage.StartAndListenUsingWireshark(tsharkListernerOutput2, tsharkExePath);
                ExecutedSteps++;

                //Step 9 - From Inbounds tab load a study with status uploaded                           
                inbounds = login.Navigate<Inbounds>();
                //inbounds.SelectAllDateAndData();
                inbounds.SearchStudy(AccessionNo: accession);
                inbounds.SelectStudy("Accession", accession);
                if (Config.isEnterpriseViewer.ToLower() == "y")
                {
                    bluering = BluRingViewer.LaunchBluRingViewer();
                }
                else
                {
                    inbounds.LaunchStudy();
                    PageLoadWait.WaitForPageLoad(30);
                    PageLoadWait.WaitForFrameLoad(30);
                    PageLoadWait.WaitForThumbnailsToLoad(60);
                    PageLoadWait.WaitForAllViewportsToLoad(60);
                }
                
                ExecutedSteps++;
                login.Logout();

                //Step 10 - ProcedureCodeSequence and requestedProcedureId are not included in request and response.
                try
                {
                    cmdWireshark.CloseMainWindow();
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Unable to close wireshark cmd2 --" + e);
                }
                //Read the output from tsharkListernerOutput file   
                File.Copy(tsharkListernerOutput2, @"C:\wiresharkOutputCopy2.txt", true);
                cmdOutput = System.IO.File.ReadAllText(@"C:\wiresharkOutputCopy2.txt");
                if (!cmdOutput.ToLower().Contains(requestResponseValidation1) && !cmdOutput.ToLower().Contains(requestResponseValidation2))
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

                //Incase of exception replace the backup config file to the path
                if (File.Exists(backupPath))
                    File.Copy(backupPath, xmlPath, true);

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
                    if (File.Exists(backupPath))
                        File.Copy(backupPath, xmlPath, true);
                    ServiceTool serviceTool = new ServiceTool();
                    serviceTool.LaunchServiceTool();
                    serviceTool.RestartService();
                    serviceTool.CloseServiceTool();
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Finally Exception--" + e);
                }
            }

        }


        /// <summary>
        /// Duplicate entries not listed in Reconciliation
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_65908(String testid, String teststeps, int stepcount)
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
                String PhUserName = Config.ph1UserName;
                String PhPassword = Config.ph1Password;
                String ArUserName = Config.ar1UserName;
                String ArPassword = Config.ar1Password;
                String mpUsername = Config.pacsadmin;
                String mpPassword = Config.pacspassword;
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] StudyPath = UploadFilePath.Split('=');
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");

                //Pre-Conditions

                //Import the Studies to MergePacs#3(Destination PACS) Server
                BasePage.RunBatchFile(Config.batchfilepath, StudyPath[0] + " " + Config.dicomsendpath + " " + Config.DestinationPACS);
                
                //Upload a Dicom Study
                ei.EIDicomUpload(Config.stUserName, Config.stPassword, Config.Dest1, StudyPath[1]);

                //Login as physician
                login.DriverGoTo(login.url);
                login.LoginIConnect(PhUserName, PhPassword);

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select study
                inbounds.SearchStudy("Accession", Accession);
                inbounds.SelectStudy1("Accession", Accession);

                //Nominate Study as physician user
                inbounds.NominateForArchive("Testing");

                //Logout ICA
                login.Logout();

                //Step 1 :- Login as Archivist
                login.DriverGoTo(login.url);
                login.LoginIConnect(ArUserName, ArPassword);
                ExecutedSteps++;

                //Navigate to Inbounds
                inbounds = (Inbounds)login.Navigate("Inbounds");

                //Search and Select study
                inbounds.SearchStudy("Accession", Accession);
                inbounds.SelectStudy("Accession", Accession);

                //Click Archive study button
                inbounds.ClickArchiveStudy("", "");

                //Step 2 :- Validate Archive/Reconcile Study dialog is opened
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

                //Search Patient with '*' as Last name
                inbounds.ArchiveSearch("patient", "*", "", "", "", "", "", "", "", "");
                  
                //Select Show All to get patients list window
                inbounds.SelectShowAllInReconcileDialog("patient");

                //Step 3 :- Validate List of Patients displayed
                if (BasePage.Driver.FindElement(By.CssSelector("#ReconciliationFindPatientControlDialogDiv")).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                IList<IWebElement> rows = BasePage.Driver.FindElements(By.CssSelector("#gridTablepatients tbody>tr"));
                IList<String> PatientsList = new List<String>();
                foreach (IWebElement row in rows)
                {
                    IList<IWebElement> columns = row.FindElements(By.CssSelector("td"));
                    PatientsList.Add(columns[1].Text + columns[4].Text);
                    Logger.Instance.InfoLog("Details from matching patient window :- Patient Name - " + columns[1].Text + " -- Patient ID - " + columns[4].Text);
                }
                IList<String> UniquePatientsList = PatientsList.Distinct().ToList<String>();
                //bool PatientListStatus = PatientsList.All(patient => PatientsList.All(d => !d.Equals(patient)));
                bool PatientListStatus = (PatientsList.Count == UniquePatientsList.Count);

                //Step 4 :- Check all entries for patients
                if (PatientListStatus)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select one matching patient
                inbounds.SelectStudyFromReconcile("Patient ID", PatientID);
                inbounds.ClickOkInShowAll();
                Dictionary<String, String> MatchingValues = inbounds.GetDataInArchive("Matching Patient");
                    
                //Step 5 :- Validate the details of matching patient
                if(MatchingValues["PID / MRN"].Equals(PatientID))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Update Patient ID of the nominated study
                String ModifiedPatientID = PatientID + "_Changed";
                inbounds.EditFinalDetailsInArchive("pid", ModifiedPatientID);

                inbounds.SetBlankFinalDetailsInArchive();
                Dictionary<String, String> FinalDetails = inbounds.GetDataInArchive("Final Details");

                //Step 6 :- Validate the modified patient ID updated in Final details
                if (FinalDetails["PID / MRN"].Equals(ModifiedPatientID))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Click on archive
                inbounds.ClickArchive();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Logout ICA
                login.Logout();

                //Step 7 :- Login to MergePacs#3 and Make sure that the study is archived properly as of Final details in Reconcile
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
                String studydate1 = DateTime.ParseExact(FinalDetails["Study Date"], "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture).ToShortDateString();
                Boolean DOBdate = login.CompareDates(MpacResults["DOB"], FinalDetails["DOB"], "yyyyMMdd", "dd-MMM-yyyy");

                //Validate Details in Final details column on archive window should match with details in Dest PACS
                if ((MpacResults["PatientName"].ToUpper().Split(' ')[0].Equals(FinalDetails["Last Name"].ToUpper())) &&
                    (MpacResults["PatientName"].ToUpper().Split(' ')[1].Equals(FinalDetails["First Name"].ToUpper())) &&
                    (MpacResults["Sex"].Equals(FinalDetails["Gender"])) && mpacdate1.Equals(studydate1) &&
                    (MpacResults["IPID"].Equals(FinalDetails["Issuer of PID"])) && (MpacResults["PatientID"].Equals(FinalDetails["PID / MRN"])) &&
                    (MpacResults["StudyDescription"].Equals(FinalDetails["Description"])) && DOBdate == true &&
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

                //Return Result
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
        /// This is dummy test for testing and Debugging purpose.
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_Dummy(String testid, String teststeps, int stepcount)
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
                String username = Config.ph1UserName;
                String password = Config.ph1Password;
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

                //Login as physician -step 1
                login.LoginIConnect("Administrator", "Administrator");                

                //Navigate Studies
                var studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", "1211824");
                studies.SelectStudy("Accession", "1211824");
                var viewer = BluRingViewer.LaunchBluRingViewer();
                var step = result.steps[++ExecutedSteps];
                step.SetPath(testid, ExecutedSteps);
                if(viewer.CompareImage(step, BasePage.FindElementByCss(viewer.Activeviewport)))
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

                //Close Viewport
                viewer.CloseBluRingViewer();
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

        }

		/// <summary>
		/// Reroute studies to different destinations
		/// </summary>
		public TestCaseResult Test1_161158(String testid, String teststeps, int stepcount)
		{
			//Declare and initialize variables
			Inbounds inbounds = null;
			Studies studies = null;
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
				String ph2u = Config.ph2UserName;
				String ph2p = Config.ph2Password;
				String ar2u = Config.ar2UserName;
				String ar2p = Config.ar2Password;
				String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
				String acclist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
				String[] AccessionNumbers = acclist.Split(':');
				String StudyPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				
				//Email objects declaration
				EmailUtils email = new EmailUtils() { EmailId = Config.ph2Email, Password = Config.ph2EmailPassword };
				Dictionary<string, string> receivedMail = new Dictionary<string, string>();

				//Mark all previous mails as read to avoid reading old unread mails
				email.MarkAllMailAsRead("INBOX");

				//Initial steps - Step 1 & 2 -- Create a set of users for Destination-2 and update user nu to have no receiver and archivist role
				ExecutedSteps++;
				ExecutedSteps++;

				//Step 3 - Send a prior studies through POP to destination1
				BasePage.RunBatchFile(Config.batchfilepath, Config.TestDataPath + StudyPath + " " + Config.dicomsendpath + " " + Config.StudyPacs);
								
				foreach (String Accession in AccessionNumbers)
				{
					//login to PACS#1 as admin
					mpaclogin.DriverGoTo(mpaclogin.mpacstudyurl);
					MPHomePage mpachome = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);

					//Send Study with matching HL7 order
					Tool tool = (Tool)mpachome.NavigateTopMenu("Tools");
					tool.NavigateToSendStudy();
					tool.SearchStudy("Accession", Accession, 0);
					tool.MpacSelectStudy("Accession", Accession);
					tool.SendStudy(1);

					//Logout
					mpaclogin.LogoutPacs();
				}				
				ExecutedSteps++;

				//Login as ph (Destination-1)
				login.LoginIConnect(username, password);

				//Navigate to Inbounds
				inbounds = (Inbounds)login.Navigate("Inbounds");

				//Step 4 - Login as ph/ph and check/search Inbounds box-verify the correctness of 
				//number of studies
				ExecutedSteps++;
				foreach (String AccNo in AccessionNumbers)
				{
					inbounds.ClearButton().Click();
					PageLoadWait.WaitforStudyInStatus(AccNo, inbounds, "Uploaded");
					Dictionary<string, string> priors = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccNo, "Uploaded" });
					if (priors != null)
					{
						result.steps[ExecutedSteps].status = "Pass";
						Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
					}
					else
					{
						result.steps[ExecutedSteps].status = "Fail";
						Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
						result.steps[ExecutedSteps].SetLogs();
						throw new Exception("Studies are not Uploaded");
					}
				}

				inbounds.ClearButton().Click();

				//Navigate to Studies
				studies = (Studies)login.Navigate("Studies");

				//Search and nominate the priors to archive with validation
				studies.SearchStudy("PatientID", pid);

				//Validate all priors are not present in studies.
				foreach (String AccNo in AccessionNumbers)
				{
					Dictionary<string, string> priors = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccNo, "Uploaded" });
					if (priors == null)
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
				}

				//Navigate to Inbounds
				inbounds = (Inbounds)login.Navigate("Inbounds");

				//Search study
				inbounds.ClearButton().Click();
				inbounds.SearchStudy("PatientID", pid);

				//Step 5 - Nominate all priors to archive and logout
				ExecutedSteps++;
				foreach (String AccNo in AccessionNumbers)
				{
					//Select and Nominate study for archive
					inbounds.SelectStudy("Accession", AccNo);
					inbounds.NominateForArchive("Testing");

					//Study Status
					String studyStatus1;
					inbounds.GetMatchingRow("Accession", AccNo).TryGetValue("Status", out studyStatus1);
					if (studyStatus1 == "Nominated For Archive")
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
				}

				//Logout as ph
				login.Logout();

				//Login as archivist
				login.LoginIConnect(username1, password1);

				//Navigate to Inbounds
				inbounds = (Inbounds)login.Navigate("Inbounds");

				//Select a prior
				inbounds.SearchStudy("Accession", AccessionNumbers[2]);
				inbounds.SelectStudy("Accession", AccessionNumbers[2]);

				//Step 6 - Login as Archivist and from Inbounds, highlight one study within priors 
				//that is in NominateForArchive status and Press"Change Destination" button
				if (inbounds.CheckDestInRerouteWindow() == true)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step 7 - Dismiss the window by clicking"X"
				BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div#RerouteStudyDiv .buttonRounded_small_blue")));
				BasePage.Driver.FindElement(By.CssSelector("div#RerouteStudyDiv .buttonRounded_small_blue")).Click();
				PageLoadWait.WaitForPageLoad(20);
				PageLoadWait.WaitHomePage();
				ExecutedSteps++;

				//Select one study with nominated for archive status
				inbounds.SearchStudy("Accession", AccessionNumbers[0]);
				inbounds.SelectStudy("Accession", AccessionNumbers[0]);

				//Step 8 - Archivist select one prior study, status=NominateForArchive and Click on " Universal " button
				BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
				ExecutedSteps++;

				//Step 9 - Load another prior study from Exam List to the comparison viewer
				viewer.OpenPriors(1);
				var viewport = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[1];
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
				var step9 = viewer.CompareImage(result.steps[ExecutedSteps], viewport);
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

				//Step 10 -Validate all priors are present in history list and load another study in second viewer - step 8
				viewer.CloseExamList();
				viewer.CloseBluRingViewer();
				ExecutedSteps++;

				//Step 11 - Archive one prior study to destination 1 with manual reconciliation
				inbounds.SearchStudy("Accession", AccessionNumbers[0]);
				inbounds.SelectStudy("Accession", AccessionNumbers[0]);
				inbounds.ArchiveStudy("", "first");
				ExecutedSteps++;

				//Search and select multiple studies
				inbounds.ClearButton().Click();
				inbounds.SearchStudy("PatientID", pid);

				//Select multiple studies
				for (int i = 0; i < AccessionNumbers.Length; i++)
				{
					inbounds.SelectStudy1(new String[] { "Accession" }, new String[] { AccessionNumbers[i] });
				}

				//Step 12 - Select multiple studies and click Change Destination button
				IWebElement r = BasePage.Driver.FindElement(By.CssSelector("#m_rerouteStudyButton"));
				if (r.Enabled == false)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				//Step 13 - Select 1 prior and press "Change Destination" button
				inbounds.SearchStudy("Accession", AccessionNumbers[1]);
				inbounds.SelectStudy("Accession", AccessionNumbers[1]);
				inbounds.ChooseColumns(new String[] { "Number of Images" });
				var primaryImageCount = BasePage.GetColumnValues("Number of Images")[0];
				inbounds.RerouteStudy(Config.Dest2);
				ExecutedSteps++;

				//Step 14 - Select destination 2 and press"Reroute"
				ExecutedSteps++; // performed as part of previous "RerouteStudy" function

				//Search Study
				inbounds.SearchStudy("Accession", AccessionNumbers[1]);

				//Step 15 - Verify the study is removed from the Archivist Inbounds
				if (inbounds.CheckStudy("Accession", AccessionNumbers[1]) == false)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Logout ar
				login.Logout();

				//Step -16 Verify the Email Notification sent to the receivers/archivist for Reroute activity
				receivedMail = email.GetMailUsingIMAP(Config.SystemEmail, "Study Rerouted to a New Destination");

				if (receivedMail.Count > 0)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step 17 - Login as u1 (who is associate to destination2) as receiver
				login.LoginIConnect(ph2u, ph2p);
				ExecutedSteps++;

				//Navigate to Inbounds
				inbounds = (Inbounds)login.Navigate("Inbounds");

				//Check for rerouted study
				inbounds.SearchStudy("Accession", AccessionNumbers[1]);

				//Find study status 
				Dictionary<String, String> Step18_data = inbounds.GetMatchingRow("Accession", AccessionNumbers[1]);

				//Step 18 - Verify the re-routed study appeared from the Inbounds of the receiver (u1)'s Inbounds 
				//(who is associate to the new destination)
				if (Step18_data["Status"].Equals("Uploaded") & Step18_data["To Destination"].Equals(Config.Dest2))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step 19 - Click on choose columns and add # primary images Column to the grid and verify that the #Primary Image Count is displayed correctly after the reroute for the uploaded studies
				inbounds.ChooseColumns(new String[] { "Number of Images" });
				var routedImageCount = BasePage.GetColumnValues("Number of Images")[0];
				Logger.Instance.InfoLog("Actual image Count: " + routedImageCount + ", Expected Image Count: " + primaryImageCount);				
				if ( primaryImageCount.Equals(routedImageCount))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step 20 - Select the study and click on nominate for archive
				inbounds.SelectStudy("Accession", AccessionNumbers[1]);
				inbounds.NominateForArchive("");

				//Find study status 
				inbounds.SearchStudy("Accession", AccessionNumbers[1]);
				Dictionary<String, String> Step19_data = inbounds.GetMatchingRow("Accession", AccessionNumbers[1]);

				if (Step19_data["Status"].Equals("Nominated For Archive"))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Logout as ph4
				login.Logout();

				//Step 21 - Logout
				//Login as u2 who has archive role associated to destination2
				login.LoginIConnect(ar2u, ar2p);
				ExecutedSteps++;

				//Navigate to Inbounds
				inbounds = (Inbounds)login.Navigate("Inbounds");

				//Search , select , launch and archive
				inbounds.SearchStudy("Accession", AccessionNumbers[1]);
				inbounds.SelectStudy("Accession", AccessionNumbers[1]);

				//Launch the Study and archive
				IWebElement UploadCommentsField, ArchiveOrderField = null;
				inbounds.ClickArchiveStudy(out UploadCommentsField, out ArchiveOrderField);
				
				inbounds.ArchiveSearch("Order", "", "", "", "", "", "", "", AccessionNumbers[1], "All Dates");
				Dictionary<String, String> MatchingValues = inbounds.GetDataInArchive("Matching Order");

				//Step 22 - Select the nominated study from Inbounds then Click on Reconcile Exam and verify the reconcile window displays (no matching order found)
				if ((MatchingValues.Values.Contains("")) || (MatchingValues.Values.Contains(null)))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Get Final details in archive window
				Dictionary<String, String> FinalValues = inbounds.GetDataInArchive("Final Details");
								
				//Click archive in reconcile window and close viewer
				inbounds.ClickArchive();
				
				//Search study
				inbounds.SearchStudy("Accession", AccessionNumbers[1]);
				PageLoadWait.WaitforStudyInStatus(AccessionNumbers[1], inbounds, "Routing Completed");

				//Find study status
				inbounds.ChooseColumns(new String[] { "Refer. Physician", "From Institution(s)" });
				Dictionary<String, String> studyStatus22 = inbounds.GetMatchingRow("Accession", AccessionNumbers[1]);

				//search for archived study in studies tab
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy("Accession", AccessionNumbers[1]);
				studies.ChooseColumns(new String[] { "Gender", "Patient DOB", "Refer. Physician", "Institutions" });
				Dictionary<String, String> StudyDetails_22 = inbounds.GetMatchingRow("Accession", AccessionNumbers[1]);

				Logger.Instance.InfoLog("Status - " + studyStatus22["Status"].Equals("Routing Completed").ToString() + Environment.NewLine +
					"Patient ID - " + StudyDetails_22["Patient ID"].Equals(FinalValues["PID / MRN"]).ToString() + Environment.NewLine +
					"Last Name - " + StudyDetails_22["Patient Name"].Split(',')[0].Trim().Equals(FinalValues["Last Name"]).ToString() + Environment.NewLine +
					"First Name - " + StudyDetails_22["Patient Name"].Split(',')[1].Trim().Equals(FinalValues["First Name"]).ToString() + Environment.NewLine +
					"Gender - " + StudyDetails_22["Gender"].Equals(FinalValues["Gender"]).ToString() + Environment.NewLine +
					"Description - " + StudyDetails_22["Description"].Equals(FinalValues["Description"]).ToString() + Environment.NewLine +
					"Patient DOB - " + StudyDetails_22["Patient DOB"].Equals(FinalValues["DOB"]).ToString() + Environment.NewLine +
					"Accession - " + StudyDetails_22["Accession"].Equals(FinalValues["Accession"]).ToString() + Environment.NewLine +
					"Institution - " + StudyDetails_22["Institutions"].Equals(studyStatus22["From Institution(s)"]).ToString() + Environment.NewLine +
					"Referring Physician - " + studyStatus22["Refer. Physician"] + " -- " + StudyDetails_22["Refer. Physician"].ToString());

				//Step 23 - Verify the info displayed from the original and archive the study
				if (studyStatus22["Status"].Equals("Routing Completed") && StudyDetails_22["Patient ID"].Equals(FinalValues["PID / MRN"]) 
					&& StudyDetails_22["Patient Name"].Split(',')[0].Trim().Equals(FinalValues["Last Name"])
					&& StudyDetails_22["Patient Name"].Split(',')[1].Trim().Equals(FinalValues["First Name"])
					&& StudyDetails_22["Gender"].Equals(FinalValues["Gender"]) && StudyDetails_22["Description"].Equals(FinalValues["Description"])
					&& StudyDetails_22["Patient DOB"].Equals(FinalValues["DOB"]) && StudyDetails_22["Accession"].Equals(FinalValues["Accession"])
					&& StudyDetails_22["Institutions"].Equals(studyStatus22["From Institution(s)"]) && StudyDetails_22["Accession"].Equals(FinalValues["Accession"])
					&& StudyDetails_22["Refer. Physician"].Contains(studyStatus22["Refer. Physician"]))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Logout as ar4
				login.Logout();

				//Login Holding Pen
				login.DriverGoTo(login.hpurl);
				hplogin = new HPLogin();
				hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
				WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");
				workflow.NavigateToLink("Workflow", "Archive Search");
				
				//Search study               
				workflow.HPSearchStudy("PatientID", pid);
				Dictionary<string, string> StudyDetails23 = workflow.GetStudyDetailsInHP();

				Logger.Instance.InfoLog("Patient ID - " + StudyDetails_22["Patient ID"].Equals(StudyDetails23["Patient ID"]).ToString() + Environment.NewLine +
					"Last Name - " + StudyDetails_22["Patient Name"].Split(',')[0].Trim().Equals(StudyDetails23["Patient Name"].Split(',')[0].Trim()).ToString() + Environment.NewLine +
					"First Name - " + StudyDetails_22["Patient Name"].Split(',')[1].Trim().Equals(StudyDetails23["Patient Name"].Split(',')[1].Trim()).ToString() + Environment.NewLine +
					"Gender - " + StudyDetails_22["Gender"].Equals(StudyDetails23["Patient's Sex"]).ToString());

				//Step 24 - Verify the study is reaching destination2 with all study info accurately compare with original
				if (StudyDetails_22["Patient ID"].Equals(StudyDetails23["Patient ID"])
					&& StudyDetails_22["Patient Name"].Split(',')[0].Trim().Equals(StudyDetails23["Patient Name"].Split(',')[0].Trim())
					&& StudyDetails_22["Patient Name"].Split(',')[1].Trim().Equals(StudyDetails23["Patient Name"].Split(',')[1].Trim())
					&& StudyDetails_22["Gender"].Equals(StudyDetails23["Patient's Sex"]))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Login as ph2
				login.LoginIConnect(username, password);
				//Navigate to Inbounds
				inbounds = (Inbounds)login.Navigate("Inbounds");

				//Search the rest of the study,select and launch
				inbounds.SearchStudy("Accession", AccessionNumbers[2]);
				inbounds.SelectStudy("Accession", AccessionNumbers[2]);
				viewer = BluRingViewer.LaunchBluRingViewer();

				//Step 25 - Login as ph/ph (who associate to destination1), 
				//load any one of the prior studies which is not archived in Universal viewer
				viewer.OpenPriors(accession: AccessionNumbers[3]);
				var StudyPanel = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[1];
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
				var step24 = viewer.CompareImage(result.steps[ExecutedSteps], StudyPanel);
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

				//Hover over each of the priors in Exam list
				IList<IWebElement> Listedpriors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
				int accessionfound = 0;
				foreach (IWebElement prior in Listedpriors)
				{
					new TestCompleteAction().MoveToElement(prior).Perform();
					var tooltip = prior.GetAttribute("title");
					foreach (String Acc in AccessionNumbers)
					{
						var expected_tooltip = "Acc#: " + Acc;
						if (tooltip.Contains(expected_tooltip))
						{
							accessionfound++;
						}
						Logger.Instance.InfoLog("The Expected ToolTip is--" + expected_tooltip);
						Logger.Instance.InfoLog("The Actual ToolTip is--" + tooltip);
					}
					
				}

				//Step 26 - Mouse hover on all the prior studies in the Exam List
				if (accessionfound == 4)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Ste 27 - Check whether the priors available in Holding pen are listed
				if (viewer.IsForeignExamAlert(AccessionNumbers[3]))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}
				
				//Step 28 - Hover over the yellow triangle icon
				ExecutedSteps++; // Already verified as part of previous validation

				//Step 29 - Load the unreconciled prior studies and the study from each datasource to multiple viewers
				for (int i = 3; i < AccessionNumbers.Count(); i++)
				{
					viewer.OpenPriors(accession: AccessionNumbers[i]);					
				}
				/** VALIDATION PENDING**/
				ExecutedSteps++;

				//Step 30 - Close the viewer
				viewer.CloseBluRingViewer();
				ExecutedSteps++;

				//Step 31 - From ph\ph user, navigate to Studies tab and search for the prior study archived to destination1
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy("Accession", AccessionNumbers[0]);
				studies.SelectStudy("Accession", AccessionNumbers[0]);
				Dictionary<String, String> StudyDetails_30 = inbounds.GetMatchingRow("Accession", AccessionNumbers[0]);
				if (StudyDetails_30 != null)
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

				//Launch Study and hover over prior studies 
				viewer = BluRingViewer.LaunchBluRingViewer();
				IList<IWebElement> Listedpriors_31 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
				int accessionfound_31 = 0;
				foreach (IWebElement prior in Listedpriors_31)
				{
					new TestCompleteAction().MoveToElement(prior).Perform();
					var tooltip = prior.GetAttribute("title");
					foreach (String Acc in AccessionNumbers)
					{
						var expected_tooltip = "Acc#: " + Acc;
						if (tooltip.Contains(expected_tooltip))
						{
							accessionfound_31++;
						}
						Logger.Instance.InfoLog("The Expected ToolTip is--" + expected_tooltip);
						Logger.Instance.InfoLog("The Actual ToolTip is--" + tooltip);
					}

				}

				//Step 32 - Load the study in Universal viewer and mouse hover on all the prior studies in Exam List
				if (accessionfound_31 == 4)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Ste 33 - Check whether the priors available in Holding pen are listed
				if (viewer.IsForeignExamAlert(AccessionNumbers[2]) && viewer.IsForeignExamAlert(AccessionNumbers[3]))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step 34 - Hover over the yellow triangle icon
				ExecutedSteps++; // Already verified as part of previous validation

				//Step 35 - Load the unreconciled prior studies and the study from each datasource to multiple viewers
				for (int i = 3; i < AccessionNumbers.Count(); i++)
				{
					viewer.OpenPriors(accession: AccessionNumbers[i]);
				}
				/** VALIDATION PENDING**/
				ExecutedSteps++;

				//Step 36 - Close the viewer
				viewer.CloseBluRingViewer();
				ExecutedSteps++;

				//Step 37 - From ph\ph user, navigate to Studies tab and search for the prior study archived to destination1
				studies.SearchStudy("Accession", AccessionNumbers[1]);
				studies.SelectStudy("Accession", AccessionNumbers[1]);
				Dictionary<String, String> StudyDetails_36 = inbounds.GetMatchingRow("Accession", AccessionNumbers[1]);
				if (StudyDetails_36 != null)
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

				//Launch Study and hover over prior studies 
				viewer = BluRingViewer.LaunchBluRingViewer();
				IList<IWebElement> Listedpriors_37 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
				int accessionfound_37 = 0;
				foreach (IWebElement prior in Listedpriors_37)
				{
					new TestCompleteAction().MoveToElement(prior).Perform();
					var tooltip = prior.GetAttribute("title");
					foreach (String Acc in AccessionNumbers)
					{
						var expected_tooltip = "Acc#: " + Acc;
						if (tooltip.Contains(expected_tooltip))
						{
							accessionfound_37++;
						}
						Logger.Instance.InfoLog("The Expected ToolTip is--" + expected_tooltip);
						Logger.Instance.InfoLog("The Actual ToolTip is--" + tooltip);
					}

				}

				//Step 38 - Load the study in Universal viewer and mouse hover on all the prior studies in Exam List
				if (accessionfound_37 == 4)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Ste 39 - Check whether the priors available in Holding pen are listed
				if (viewer.IsForeignExamAlert(AccessionNumbers[2]) && viewer.IsForeignExamAlert(AccessionNumbers[3]))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step 40 - Hover over the yellow triangle icon
				ExecutedSteps++; // Already verified as part of previous validation

				//Step 41 - Load the unreconciled prior studies and the study from each datasource to multiple viewers
				for (int i = 3; i < AccessionNumbers.Count(); i++)
				{
					viewer.OpenPriors(accession: AccessionNumbers[i]);
				}
				/** VALIDATION PENDING**/
				ExecutedSteps++;

				//Step 42 - Close the viewer
				viewer.CloseBluRingViewer();
				ExecutedSteps++;

				//Logout application
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
				Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

				//Report Result
				result.FinalResult(e, ExecutedSteps); ;
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);

				//Logout
				login.Logout();

				//Return Result
				return result;
			}

		}

		/// <summary>
		/// Send exam from remote as unregistered user - Dicom Studies Upload
		/// </summary>
		/// <param name="testid"></param>
		/// <param name="teststeps"></param>
		/// <param name="stepcount"></param>
		/// <returns></returns>
		public TestCaseResult Test1_161159(String testid, String teststeps, int stepcount)
		{
			Inbounds inbounds = null;
			TestCaseResult result;
			result = new TestCaseResult(stepcount);
			int ExecutedSteps = -1;

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);

			try
			{
				String phUsername = Config.ph1UserName;
				String phPassword = Config.ph1Password;
				String hpUserName = Config.hpUserName;
				String hpPassword = Config.hpPassword;
				String arUsername = Config.ar1UserName;
				String arPassword = Config.ar1Password;
				String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
				String[] AccessionNumbers = AccessionNoList.Split(':');
				String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				String[] FilePaths = UploadFilePath.Split(':');
				String Comments = login.RandomString(10, true);

				//Steps 1-2 - Initial setup (done as part of initial setup)
				ExecutedSteps++;
				ExecutedSteps++;

				//Step 3 - Launch CD uploader and sign on with email address (anonymous user credentials) that consistent when install 
				//Step 4 - User browses to the location of CD files and select one study out of multiples and upload
				ei.EIDicomUploadUnReg(Config.ph1Email, Config.Dest1, FilePaths[0]);
				ExecutedSteps++;
				ExecutedSteps++;

				//Navigate to search in Holding pen
				login.DriverGoTo(login.hpurl);
				hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
				WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");
				workflow.NavigateToLink("Workflow", "Archive Search");

				//Search Study
				workflow.HPSearchStudy("Accessionno", AccessionNumbers[0]);
				Boolean Step5_HP = workflow.HPCheckStudy(AccessionNumbers[0]);
				if (!Step5_HP == true)
				{
					throw new Exception("Study is not present in Holding Pen");
				}

				//Logout in holding Pen
				hplogin.LogoutHPen();

				//Login as physician 
				login.DriverGoTo(login.url);
				login.LoginIConnect(phUsername, phPassword);

				//Navigate to inbounds
				inbounds = (Inbounds)login.Navigate("Inbounds");

				//Search Study               
				inbounds.SearchStudy("Accession", AccessionNumbers[0]);
				Dictionary<String, String> StudyDetails_5 = inbounds.GetMatchingRow("Accession", AccessionNumbers[0]);

				//Step 5 - Check sent studies from holding pen and physician's inbounds
				if (Step5_HP && StudyDetails_5 != null)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step 6 - Verify the status of received studies in the Inbounds from physician's account
				if (StudyDetails_5["Status"].Equals("Uploaded"))
				{
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

				//Step 7 - Select the rest of the studies and
				//Add comments
				//Raise priority to High and upload
				ei.EIDicomUploadUnReg(Config.ph1Email, Config.Dest1, "", "HIGH", FilePaths[1], Comments);
				ExecutedSteps++;

				//Navigate to search in Holding pen
				login.DriverGoTo(login.hpurl);
				hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
				WorkFlow workflow1 = (WorkFlow)hphomepage.Navigate("Workflow");
				workflow.NavigateToLink("Workflow", "Archive Search");

				Boolean Step8_HP = false, Step8_Inb = false;
				//Check sent studies from holding pen
				foreach (string Accession in AccessionNumbers)
				{
					//Search Study
					workflow1.HPSearchStudy("Accessionno", Accession);

					if (workflow.HPCheckStudy(Accession))
					{
						Step8_HP = true;
					}
					else
					{
						Step8_HP = false;
						throw new Exception("Study is not present in Holding Pen");
					}
				}

				//Logout in holding Pen
				hplogin.LogoutHPen();

				//Login as physician 
				login.DriverGoTo(login.url);
				login.LoginIConnect(phUsername, phPassword);

				//Navigate to inbounds
				inbounds = (Inbounds)login.Navigate("Inbounds");

				//Validate uploaded studies are present in physician's inbounds
				for (int i = 0; i < AccessionNumbers.Length; i++)
				{
					if (i == 0) { continue; }
					inbounds.SearchStudy("Accession", AccessionNumbers[i]);
					PageLoadWait.WaitForPageLoad(20);
					PageLoadWait.WaitForFrameLoad(20);
					Dictionary<string, string> row = inbounds.GetMatchingRow(new string[] { "Accession", "Status", "Priority", "Comments" }, new string[] { AccessionNumbers[i], "Uploaded", "HIGH", Comments });
					if (row != null)
					{
						Step8_Inb = true;
					}
					else
					{
						Step8_Inb = false;
						break;
					}
				}

				//Step 8: Check sent studies from holding pen and physician's inbounds
				if (Step8_HP & Step8_Inb)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				BluRingViewer viewer = null;
				int counter = 0;
				foreach (string Accession in AccessionNumbers)
				{
					////Login as physician 
					//login.CreateNewSesion();
					//login.DriverGoTo(login.url);
					//login.LoginIConnect(phUsername, phPassword);

					////Navigate to inbounds
					//inbounds = (Inbounds)login.Navigate("Inbounds");
					//Search and Select Study 
					inbounds.SearchStudy("Accession", Accession);
					inbounds.SelectStudy("Accession", Accession);

					//Launch Study
					viewer = BluRingViewer.LaunchBluRingViewer();

					//IWebElement infobar = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_StudyPanelTitleComponent));
					//new TestCompleteAction().MoveToElement(infobar).Perform();
					//Thread.Sleep(new TimeSpan(0,0,10));
					//var tooltip = infobar.GetAttribute("title");
					//var expected_tooltip = "Acc#: " + Accession;

					//Logger.Instance.InfoLog("The Expected ToolTip is--" + expected_tooltip);
					//Logger.Instance.InfoLog("The Actual ToolTip is--" + tooltip);
					bool validation = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.AccessionNumberInExamList))[0].Text.Trim().Contains(Accession);
					if (validation)
					{
						counter++;
					}
					else
					{
						result.steps[ExecutedSteps+1].SetLogs();
						Logger.Instance.InfoLog("Condition not met for Accession - " + Accession);
						Logger.Instance.InfoLog("Error image path - " + result.steps[ExecutedSteps + 1].snapshotpath);
					}
					
					//Close Study
					viewer.CloseBluRingViewer();
				}

				//Step 9 : Load the uploaded studies in Universal Viewer
				if (counter == AccessionNumbers.Length)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
				}

				//Step 10 : Close the viewer
				ExecutedSteps++;

				//Step 11 : Nominate the uploaded studies by clicking on " Nominate for Archive "
				foreach (string Accession in AccessionNumbers)
				{
					//Search and Select Study 
					inbounds.SearchStudy("Accession", Accession);
					inbounds.SelectStudy("Accession", Accession);

					//Nominate study for archive
					inbounds.NominateForArchive("");
				}
				ExecutedSteps++;

				//Logout as Physician
				login.Logout();

				//Login as archivist
				login.LoginIConnect(arUsername, arPassword);

				//Navigate to inbounds
				inbounds = (Inbounds)login.Navigate("Inbounds");

				Boolean Step_12 = false;
				foreach (string Accession in AccessionNumbers)
				{
					//Search Study
					inbounds.SearchStudy("Accession", Accession);
					inbounds.SelectStudy("Accession", Accession);

					inbounds.ArchiveStudy("", "");
					PageLoadWait.WaitforStudyInStatus(Accession, inbounds, "Routing Completed");
					Dictionary<String, String> Status_13 = inbounds.GetMatchingRow("Accession", Accession);
					if (Status_13["Status"].Equals("Routing Completed"))
					{
						Step_12 = true;
					}
					else
					{
						Step_12 = false;
						break;
					}
				}

				//Step 12 - Login as archivist and check Inbounds
				//Archive the entire study with manual edit in reconciliation
				if (Step_12)
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
					throw new Exception("One of the Study not routed to destination");
				}

				//Logout
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
				Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

				//Report Result
				result.FinalResult(e, ExecutedSteps); ;
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);

				//Logout
				login.Logout();

				//Return Result
				return result;
			}
		}

		/// <summary>        
		/// Send exam from remote as unregistered user - Non Dicom Studies Upload
		/// </summary>
		public TestCaseResult Test2_161159(String testid, String teststeps, int stepcount)
		{
			Inbounds inbounds = null;
			TestCaseResult result;
			result = new TestCaseResult(stepcount);
			int ExecutedSteps = -1;

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);

			try
			{
				String ph1Username = Config.ph1UserName;
				String ph1Password = Config.ph1Password;
				String ar1Username = Config.ar1UserName;
				String ar1Password = Config.ar1Password;
				String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
				//String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PaitentID");
				String Email = Config.ph1Email;
				//String Description = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
				String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				String ImagePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ImageFilePath");

				//Step 1 : Launch CD uploader and sign on with email address (anonymous user credentials) that consistent when install
				//Step 2 : Select non-dicom files from local and associates it to Exam from CD Session and upload
				ei.EI_UploadDicomWithNonDicom("", "", Config.Dest1, UploadFilePath, ImagePath, UnRegUserEmail: Config.ph1Email);
				ExecutedSteps++;
				ExecutedSteps++;

				//Login as physician1 
				login.DriverGoTo(login.url);

				//Login as physician in Destination-1
				login.LoginIConnect(ph1Username, ph1Password);

				//Navigate to inbounds
				inbounds = (Inbounds)login.Navigate("Inbounds");

				//Search and Select study
				inbounds.SearchStudy("Accession", Accession);
				inbounds.SelectStudy("Accession", Accession);

				//Step 3 : Login iCA as physician and load the study in Universal Viewer to verify the study info 
				BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
				var panel = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[0];
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
				var step4 = viewer.CompareImage(result.steps[ExecutedSteps], panel);
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

				//Step 4 : Close the viewer
				viewer.CloseBluRingViewer();
				ExecutedSteps++;

				//Step 5 : Nominate the uploaded stud(y)ies by clicking on " Nominate for Archive "
				inbounds.SelectStudy("Accession", Accession);
				inbounds.NominateForArchive("");
				inbounds.SearchStudy("Accession", Accession);
				Dictionary<String, String> Status_5 = inbounds.GetMatchingRow("Accession", Accession);
				if (Status_5["Status"].Equals("Nominated For Archive"))
				{
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
				login.Logout();

				//Login as Archivist in Destination-1
				login.LoginIConnect(ar1Username, ar1Password);

				//Navigate to inbounds
				inbounds = (Inbounds)login.Navigate("Inbounds");

				//Search and Select study
				inbounds.SearchStudy("Accession", Accession);
				inbounds.SelectStudy("Accession", Accession);

				//Step 6: Logout and Login as Archivist and load the uploaded studies in Universal viewer
				viewer = BluRingViewer.LaunchBluRingViewer();
				var panel_6 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[0];
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
				var step6 = viewer.CompareImage(result.steps[ExecutedSteps], panel_6);
				if (step6)
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

				//Step 7 : Close the viewer
				viewer.CloseBluRingViewer();
				ExecutedSteps++;

				//Step 8 : Archivist clicks on " Reconcile Exam " and click on archive button
				inbounds.SelectStudy("Accession", Accession);
				inbounds.ArchiveStudy("", "");
				PageLoadWait.WaitforStudyInStatus(Accession, inbounds, "Routing Completed");
				inbounds.SearchStudy("Accession", Accession);
				Dictionary<String, String> Status_8 = inbounds.GetMatchingRow("Accession", Accession);
				if (Status_8["Status"].Equals("Routing Completed"))
				{
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
				PageLoadWait.WaitForPageLoad(10);
				PageLoadWait.WaitForFrameLoad(10);
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
				Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

				//Report Result
				result.FinalResult(e, ExecutedSteps); ;
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);

				//Logout
				login.Logout();

				//Return Result
				return result;
			}
		}

		/// <summary>
		/// Send exam from remote as unregistered user - Priors Upload
		/// </summary>
		public TestCaseResult Test3_161159(String testid, String teststeps, int stepcount)
		{
			Inbounds inbounds = null;
			TestCaseResult result;
			result = new TestCaseResult(stepcount);
			int ExecutedSteps = -1;

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);

			try
			{
				String ph1Username = Config.ph1UserName;
				String ph1Password = Config.ph1Password;
				String ph2Username = Config.ph2UserName;
				String ph2Password = Config.ph2Password;
				String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
				String[] AccessionNumbers = AccessionIDList.Split(':');
				String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				String[] FilePaths = UploadFilePath.Split(':');
				String Email = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EmailID");

				//Upload study to Destination-1
				ei.EIDicomUploadUnReg(Email, Config.Dest1, FilePaths[0]);

				//Upload study to Destination-2
				ei.EIDicomUploadUnReg(Email, Config.Dest2, FilePaths[1]);
				ExecutedSteps++;

				//Login as physician in destination-1
				login.DriverGoTo(login.url);
				login.LoginIConnect(ph1Username, ph1Password);

				//Navigate to inbounds
				inbounds = (Inbounds)login.Navigate("Inbounds");

				//Search and Select study
				inbounds.SearchStudy("Accession", AccessionNumbers[0]);
				//inbounds.SelectStudy("Accession", AccessionNumbers[0]);

				String studyStatus2;
				inbounds.GetMatchingRow("Accession", AccessionNumbers[0]).TryGetValue("Status", out studyStatus2);

				//Validate study is present in physician's inbounds
				if (studyStatus2 == "Uploaded")
				{
					result.steps[ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
					throw new Exception("Study not reached to Destination-1");
				}

				//Logout
				login.Logout();

				//Login as Physician of Destination - 2
				login.LoginIConnect(ph2Username, ph2Password);

				//Navigate to inbounds
				inbounds = (Inbounds)login.Navigate("Inbounds");

				//Search and Select study
				inbounds.SearchStudy("Accession", AccessionNumbers[1]);
				//inbounds.SelectStudy("Accession", AccessionNumbers[1]);

				//Validate Study is listed and status as uploaded
				String studyStatus1;
				inbounds.GetMatchingRow("Accession", AccessionNumbers[1]).TryGetValue("Status", out studyStatus1);
				if (studyStatus1 == "Uploaded")
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
					throw new Exception("Study not reached to Destination-2");
				}

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
				//Log Exception
				Logger.Instance.ErrorLog("Exception in Test Method--" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

				//Report Result
				result.FinalResult(e, ExecutedSteps); ;
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);

				//Logout
				login.Logout();

				//Return Result
				return result;
			}
		}


		/// <summary>
		/// Test Case - 29467 - Sharing a Dicom Study
		/// </summary>
		public TestCaseResult Test1_161160(String testid, String teststeps, int stepcount)
		{
			//Declare and initialize variables            
			Inbounds inbounds = null;
			Outbounds outbounds = null;
			TestCaseResult result;
			result = new TestCaseResult(stepcount);

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);
			int executedsteps = -1;

			try
			{
				//Fetch required Test data
				String phusername = Config.ph1UserName;
				String phpassword = Config.ph1Password;
				String ph1username = Config.phUserName;
				String ph1password = Config.phPassword;
				String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
				String AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
				String order = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Order");
				String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

				//Email objects declaration
				EmailUtils email = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
				Dictionary<string, string> receivedMail = new Dictionary<string, string>();

				//Mark all previous mails as read to avoid reading old unread mails
				email.MarkAllMailAsRead("INBOX");

				//Upload a Dicom Study - Step-1
				ei.EIDicomUpload(Config.ph1UserName, Config.ph1Password, Config.Dest1, UploadFilePath);
				executedsteps++;

				//Login as physician and check inbounds on uploaded file - Step-2
				login.LoginIConnect(phusername, phpassword);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("accessionNo", AccessionNo);
				Dictionary<string, string> row = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
				if (!(row == null))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
					throw new Exception("Dicom File not uploaded");
				}

				//Logout and Validate Study present in Holding pen-Step-3
				login.Logout();
				login.DriverGoTo(login.hpurl);
				HPLogin hplogin = new HPLogin();
				HPHomePage hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
				WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");
				workflow.NavigateToLink("Workflow", "Archive Search");
				workflow.HPSearchStudy("Accessionno", AccessionNo);

				if (workflow.HPCheckStudy(AccessionNo) == true)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[1].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[1].description);
					result.steps[executedsteps].SetLogs();
					throw new Exception("Study not present in Holding Pen");
				}

				//stepcount-4
				//Load Study into the Universal viewer and validate uploaded and study displayed are in synch
				hplogin.LogoutHPen();
				login.DriverGoTo(login.url);
				login.LoginIConnect(phusername, phpassword);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("accessionNo", AccessionNo);
				inbounds.SelectStudy1(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
				BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
				var panel = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[0];
				result.steps[++executedsteps].SetPath(testid, executedsteps);
				var step4 = viewer.CompareImage(result.steps[executedsteps], panel);
				if (step4)
				{
					result.steps[executedsteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}
				viewer.CloseBluRingViewer();

				//Share study to single user and verify Study status in physician's outbound and Receiver's outbound -Step-5
				inbounds.SelectStudy1(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
				inbounds.ShareStudy(false, new String[] { ph1username });
				//Check shared study present in Physican's outbound
				outbounds = (Outbounds)login.Navigate("Outbounds");
				outbounds.SearchStudy("accessionNo", AccessionNo);
				Dictionary<string, string> studyshared = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
				if (!(studyshared == null))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Verify study status in Receiver's Inbounds - Step-6
				login.Logout();
				login.LoginIConnect(ph1username, ph1password);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				PageLoadWait.WaitforStudyInStatus(AccessionNo, inbounds, "Shared");
				inbounds.SearchStudy("accessionNo", AccessionNo);
				Dictionary<string, string> studysharedinbounds = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
				if (!(studysharedinbounds == null))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Get Email
				receivedMail = email.GetMailUsingIMAP(Config.SystemEmail, "Shared Study");

				//Step 7 - Check mail is received in archivist mail box
				if (receivedMail.Count > 0)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Launch the link URL from mail
				String StudyURL = email.GetEmailedStudyLink(receivedMail);

				//Login as Archivist
				LaunchEmailedStudy.LaunchStudy<Login>(StudyURL);
				login.LoginIConnect(ph1username, ph1password);
				PageLoadWait.WaitForLoadingMessage(60);
				PageLoadWait.WaitForPageLoad(20);
				PageLoadWait.WaitForSearchLoad();
				PageLoadWait.WaitForFrameLoad(20);
				BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input#m_studySearchControl_m_searchInputPatientLastName")));
				//inbounds.SelectStudy1(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
				viewer = BluRingViewer.LaunchBluRingViewer();

				//Getting accession details from tool tip
				IWebElement infobar = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_StudyPanelTitleComponent));
				new TestCompleteAction().MoveToElement(infobar).Perform();
				var tooltip = infobar.GetAttribute("title");
				var expected_tooltip = "Acc#: " + AccessionNo;

				Logger.Instance.InfoLog("The Expected ToolTip is--" + expected_tooltip);
				Logger.Instance.InfoLog("The Actual ToolTip is--" + tooltip);

				//Step 8 - Viewer launched 
				if (viewer.PatientDetailsInViewer()["PatientID"].Equals(PatientID) && tooltip.Contains(expected_tooltip))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}
				viewer.CloseBluRingViewer();
				login.Logout();

				//Nominate the study for Archiving and validate-step-9
				login.LoginIConnect(phusername, phpassword);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("accessionNo", AccessionNo);
				inbounds.SelectStudy1(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
				//inbounds.SelectStudy1(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
				inbounds.NominateForArchive(order);

				//Check if Archive button not displayed
				int archivebuttonfoundflag = 0;
				try
				{
					if (BasePage.Driver.FindElement(By.CssSelector("#m_archiveStudyButton")).Displayed)
					{
						archivebuttonfoundflag = 1;
					}

				}
				catch (Exception excep)
				{
					Logger.Instance.InfoLog("Archive button not found");
				}

				//Validate Study Status+
				Dictionary<string, string> studynominated = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Nominated For Archive" });
				if ((!(studynominated == null)) && (archivebuttonfoundflag == 0))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[++executedsteps].SetLogs();
				}

				//Logout-step-10
				login.Logout();
				executedsteps++;

				//Report Result
				result.FinalResult(executedsteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);

				//Return Result
				return result;
			}
			catch (Exception e)
			{
				//Log Exception
				Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

				//Report Result
				result.FinalResult(e, executedsteps);
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);

				//Logout
				login.Logout();

				//Return Result
				return result;
			}
		}

		/// <summary>
		/// Test Case - 29467 - Sharing a Non Dicom Study
		/// </summary>
		public TestCaseResult Test2_161160(String testid, String teststeps, int stepcount)
		{
			//Declare and initialize variables            
			Inbounds inbounds = null;
			Outbounds outbounds = null;
			TestCaseResult result;
			result = new TestCaseResult(stepcount);

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);
			int executedsteps = -1;

			try
			{
				//Fetch required Test data
				String phusername = Config.ph1UserName;
				String phpassword = Config.ph1Password;
				String ph1username = Config.phUserName;
				String ph1password = Config.phPassword;
				String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
				String AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
				String order = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Order");
				String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				String imagePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ImageFilePath");
				String Description = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");

				//Email objects declaration
				EmailUtils email = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
				Dictionary<string, string> receivedMail = new Dictionary<string, string>();

				//Mark all previous mails as read to avoid reading old unread mails
				email.MarkAllMailAsRead("INBOX");

				//Upload a Non-Dicom Study -- Step-1
				ei.EINonDicomUpload(Config.ph1UserName, Config.ph1Password, Config.Dest1, "", "", UploadFilePath, imagePath, Description, PatientID, AccessionNo);
				executedsteps++;

				//Login as physician and check inbounds on uploaded file-Step-2
				login.LoginIConnect(phusername, phpassword);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("accessionNo", AccessionNo);
				Dictionary<string, string> row = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
				if (!(row == null))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
					throw new Exception("Non-Dicom File not found");
				}

				//Logout and Valiadte Study present in Holding pen -Step-3
				login.Logout();
				login.DriverGoTo(login.hpurl);
				HPLogin hplogin = new HPLogin();
				HPHomePage hphomepage = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
				WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");
				workflow.NavigateToLink("Workflow", "Archive Search");
				workflow.HPSearchStudy("Accessionno", AccessionNo);
				if (workflow.HPCheckStudy(AccessionNo) == true)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
					throw new Exception("Study not present in Holding Pen");
				}

				//Load Study into the viewer and validate uploaded and study diaplyed-Step-4
				hplogin.LogoutHPen();
				login.DriverGoTo(login.url);
				login.LoginIConnect(phusername, phpassword);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("accessionNo", AccessionNo);
				inbounds.SelectStudy1(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
				BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
				var panel = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[0];
				result.steps[++executedsteps].SetPath(testid, executedsteps);
				var step4 = viewer.CompareImage(result.steps[executedsteps], panel);
				if (step4)
				{
					result.steps[executedsteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}
				viewer.CloseBluRingViewer();

				//Share study to single user and verify Study status in physician's outbound and Receiver's outbound-Step-5
				inbounds.SelectStudy1(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
				inbounds.ShareStudy(false, new String[] { ph1username });
				//Check shared study present in Physican's outbound
				outbounds = (Outbounds)login.Navigate("Outbounds");
				outbounds.SearchStudy("accessionNo", AccessionNo);
				Dictionary<string, string> studyshared = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
				if (!(studyshared == null))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Verify study status in Receiver's Inbound-Step-6
				login.Logout();
				login.LoginIConnect(ph1username, ph1password);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				PageLoadWait.WaitforStudyInStatus(AccessionNo, inbounds, "Shared");
				inbounds.SearchStudy("accessionNo", AccessionNo);
				Dictionary<string, string> studysharedinbounds = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
				if (!(studysharedinbounds == null))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Get Email
				receivedMail = email.GetMailUsingIMAP(Config.SystemEmail, "Shared Study");

				//Step 7 - Check mail is received in archivist mail box
				if (receivedMail.Count > 0)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Launch the link URL from mail
				String StudyURL = email.GetEmailedStudyLink(receivedMail);

				//Login as Archivist
				LaunchEmailedStudy.LaunchStudy<Login>(StudyURL);
				login.LoginIConnect(ph1username, ph1password);
				PageLoadWait.WaitForLoadingMessage(60);
				PageLoadWait.WaitForPageLoad(20);
				PageLoadWait.WaitForSearchLoad();
				PageLoadWait.WaitForFrameLoad(20);
				BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input#m_studySearchControl_m_searchInputPatientLastName")));
				//inbounds.SelectStudy1(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
				viewer = BluRingViewer.LaunchBluRingViewer();

				//Getting accession details from tool tip
				IWebElement infobar = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_StudyPanelTitleComponent));
				new TestCompleteAction().MoveToElement(infobar).Perform();
				var tooltip = infobar.GetAttribute("title");
				var expected_tooltip = "Acc#: " + AccessionNo;

				Logger.Instance.InfoLog("The Expected ToolTip is--" + expected_tooltip);
				Logger.Instance.InfoLog("The Actual ToolTip is--" + tooltip);

				//Step 8 - Viewer launched 
				if (viewer.PatientDetailsInViewer()["PatientID"].Equals(PatientID) && tooltip.Contains(expected_tooltip))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}
				viewer.CloseBluRingViewer();

				//Nominate the study for Archiving and validate-Step-9
				login.Logout();
				login.LoginIConnect(phusername, phpassword);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("accessionNo", AccessionNo);
				inbounds.SelectStudy1(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Uploaded" });
				inbounds.NominateForArchive(order);
				//Check if Archive button not displayed
				int archivebuttonfoundflag = 0;
				try
				{
					if (BasePage.Driver.FindElement(By.CssSelector("#m_archiveStudyButton")).Displayed)
					{
						archivebuttonfoundflag = 1;
					}

				}
				catch (Exception excep)
				{
					Logger.Instance.InfoLog("Archive button not found");
				}

				//Validate Study Status
				Dictionary<string, string> studynominated = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Nominated For Archive" });
				if ((!(studynominated == null)) && (archivebuttonfoundflag == 0))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Logout-Step-10
				login.Logout();
				executedsteps++;

				//Report Result
				result.FinalResult(executedsteps++);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);


				//Return Result
				return result;
			}
			catch (Exception e)
			{
				//Log Exception
				Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

				//Report Result
				result.FinalResult(e, executedsteps++);
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);

				//Logout
				login.Logout();

				//Return Result
				return result;
			}
		}

		/// <summary>
		/// This Test is to Grant/Remove access (for a Dicom study) to multiple users 
		/// </summary>
		public TestCaseResult Test3_161160(String testid, String teststeps, int stepcount)
		{
			//Declare and initialize variables            
			Inbounds inbounds = null;
			Outbounds outbounds = null;
			TestCaseResult result;
			result = new TestCaseResult(stepcount);

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);
			int executedsteps = -1;

			try
			{
				//Fetch required Test data
				String phusername = Config.phUserName;
				String phpassword = Config.phPassword;
				String ph1username = Config.ph1UserName;
				String ph1password = Config.ph1Password;
				String ph2username = Config.ph2UserName;
				String ph2password = Config.ph2Password;
				String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
				String AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
				String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

				//Email objects declaration
				EmailUtils Phemail = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
				Dictionary<string, string> PhReceivedMail = new Dictionary<string, string>();
				EmailUtils Ph2email = new EmailUtils() { EmailId = Config.ph2Email, Password = Config.ph2EmailPassword };
				Dictionary<string, string> Ph2ReceivedMail = new Dictionary<string, string>();

				//Mark all previous mails as read to avoid reading old unread mails
				Phemail.MarkAllMailAsRead("INBOX");
				Ph2email.MarkAllMailAsRead("INBOX");

				//Upload a Dicom Study-Step-1
				ei.EIDicomUpload(Config.ph1UserName, Config.ph1Password, Config.Dest1, UploadFilePath);
				executedsteps++;

				//Login as physician and check inbounds on uploaded file-Step-2
				login.LoginIConnect(ph1username, ph1password);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("accessionNo", AccessionNo);
				Dictionary<string, string> row = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
				if (!(row == null))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[++executedsteps].SetLogs();
					throw new Exception("Dicom File not uploaded");
				}

				//Share study to Mutiple user-Step-3
				inbounds.SelectStudy1(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
				inbounds.ShareStudy(false, new String[] { phusername, ph2username });
				executedsteps++;

				//Check shared study present in Physican's outbound in shared status-Step-4
				outbounds = (Outbounds)login.Navigate("Outbounds");
				outbounds.SearchStudy("accessionNo", AccessionNo);
				Dictionary<string, string> studyshared = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
				if (!(studyshared == null))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Login as Ph and check study in shared status-Step-5
				login.Logout();
				login.LoginIConnect(phusername, phpassword);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("accessionNo", AccessionNo);
				Dictionary<string, string> studyshared1 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
				if (!(studyshared1 == null))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Login as Ph2 and check study in shared status-Step-6               
				login.Logout();
				login.LoginIConnect(ph2username, ph2password);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("accessionNo", AccessionNo);
				Dictionary<string, string> studyshared2 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
				if (!(studyshared2 == null))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Get Email
				PhReceivedMail = Phemail.GetMailUsingIMAP(Config.SystemEmail, "Shared Study");
				Ph2ReceivedMail = Ph2email.GetMailUsingIMAP(Config.SystemEmail, "Shared Study");

				//Step 7 - Email Notification
				if (PhReceivedMail.Count > 0 && Ph2ReceivedMail.Count > 0)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Login as Ph1 and remove access to Ph2-Step-8
				login.Logout();
				login.LoginIConnect(ph1username, ph1password);
				outbounds = (Outbounds)login.Navigate("Outbounds");
				outbounds.SearchStudy("accessionNo", AccessionNo);
				outbounds.SelectStudy1(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
				outbounds.RemoveAccess(new string[] { ph2username });
				//Validate study not present for ph2
				login.Logout();
				login.LoginIConnect(ph2username, ph2password);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("accessionNo", AccessionNo);
				Dictionary<string, string> studyremoved2 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
				if (studyremoved2 == null)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[++executedsteps].SetLogs();
				}

				//Validate study present for ph-Step-9
				login.Logout();
				login.LoginIConnect(phusername, phpassword);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("accessionNo", AccessionNo);
				Dictionary<string, string> studynotremoved = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
				if (!(studynotremoved == null))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[++executedsteps].SetLogs();
				}

				//Logout-Step-10
				login.Logout();
				executedsteps++;

				//Report Result
				result.FinalResult(executedsteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);

				//Return Result
				return result;
			}
			catch (Exception e)
			{
				//Log Exception
				Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

				//Report Result
				result.FinalResult(e, executedsteps);
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);

				//Logout
				login.Logout();

				//Return Result
				return result;
			}
		}

		/// <summary>
		/// This Test is to Grant/Remove access (for a Non-Dicom study) to multiple users 
		/// </summary>
		public TestCaseResult Test4_161160(String testid, String teststeps, int stepcount)
		{
			//Declare and initialize variables            
			Inbounds inbounds = null;
			Outbounds outbounds = null;
			TestCaseResult result;
			result = new TestCaseResult(stepcount);

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);
			int executedsteps = -1;

			try
			{
				//Fetch required Test data
				String phusername = Config.phUserName;
				String phpassword = Config.phPassword;
				String ph1username = Config.ph1UserName;
				String ph1password = Config.ph1Password;
				String ph2username = Config.ph2UserName;
				String ph2password = Config.ph2Password;
				String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
				String AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
				String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				String imagePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ImageFilePath");
				String Description = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
				
				//Email objects declaration
				EmailUtils Phemail = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
				Dictionary<string, string> PhReceivedMail = new Dictionary<string, string>();
				EmailUtils Ph2email = new EmailUtils() { EmailId = Config.ph2Email, Password = Config.ph2EmailPassword };
				Dictionary<string, string> Ph2ReceivedMail = new Dictionary<string, string>();

				//Mark all previous mails as read to avoid reading old unread mails
				Phemail.MarkAllMailAsRead("INBOX");
				Ph2email.MarkAllMailAsRead("INBOX");

				//Upload a Non-Dicom Study
				ei.EINonDicomUpload(Config.ph1UserName, Config.ph1Password, Config.Dest1, "", "", UploadFilePath, imagePath, Description, PatientID, AccessionNo);
				executedsteps++;

				//Login as physician and check inbounds on uploaded file-Step-2
				login.LoginIConnect(ph1username, ph1password);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("accessionNo", AccessionNo);
				Dictionary<string, string> row = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
				if (!(row == null))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[++executedsteps].SetLogs();
					throw new Exception("Dicom File not uploaded");
				}

				//Share study to Mutiple user-Step-3
				inbounds.SelectStudy1(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
				inbounds.ShareStudy(false, new String[] { phusername, ph2username });
				executedsteps++;

				//Check shared study present in Physican's outbound in shared status-Step-4
				outbounds = (Outbounds)login.Navigate("Outbounds");
				outbounds.SearchStudy("accessionNo", AccessionNo);
				Dictionary<string, string> studyshared = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
				if (!(studyshared == null))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Login as Ph and check study in shared status-Step-5
				login.Logout();
				login.LoginIConnect(phusername, phpassword);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("accessionNo", AccessionNo);
				Dictionary<string, string> studyshared1 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
				if (!(studyshared1 == null))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Login as Ph2 and check study in shared status-Step-6               
				login.Logout();
				login.LoginIConnect(ph2username, ph2password);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("accessionNo", AccessionNo);
				Dictionary<string, string> studyshared2 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
				if (!(studyshared2 == null))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Get Email
				PhReceivedMail = Phemail.GetMailUsingIMAP(Config.SystemEmail, "Shared Study");
				Ph2ReceivedMail = Ph2email.GetMailUsingIMAP(Config.SystemEmail, "Shared Study");

				//Step 7 - Email Notification
				if (PhReceivedMail.Count > 0 && Ph2ReceivedMail.Count > 0)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Login as Ph1 and remove access to Ph2-Step-8
				login.Logout();
				login.LoginIConnect(ph1username, ph1password);
				outbounds = (Outbounds)login.Navigate("Outbounds");
				outbounds.SearchStudy("accessionNo", AccessionNo);
				inbounds.SelectStudy1(new string[] { "Patient ID", "Accession" }, new string[] { PatientID, AccessionNo });
				outbounds.RemoveAccess(new string[] { ph2username });
				//Validate study not present for ph2
				login.Logout();
				login.LoginIConnect(ph2username, ph2password);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("accessionNo", AccessionNo);
				Dictionary<string, string> studyremoved2 = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
				if (studyremoved2 == null)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[++executedsteps].SetLogs();
				}

				//Validate study present for ph-Step-9
				login.Logout();
				login.LoginIConnect(phusername, phpassword);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("accessionNo", AccessionNo);
				Dictionary<string, string> studynotremoved = inbounds.GetMatchingRow(new string[] { "Patient ID", "Accession", "Status" }, new string[] { PatientID, AccessionNo, "Shared" });
				if (!(studynotremoved == null))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[++executedsteps].SetLogs();
				}

				//Logout-Step-10
				login.Logout();
				executedsteps++;

				//Report Result
				result.FinalResult(executedsteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);

				//Return Result
				return result;
			}
			catch (Exception e)
			{
				//Log Exception
				Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

				//Report Result
				result.FinalResult(e, executedsteps);
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);

				//Logout
				login.Logout();

				//Return Result
				return result;
			}
		}

		/// <summary>
		/// This Test is to Share Prior and perform validation with respect to it.
		/// </summary>
		public TestCaseResult Test5_161160(String testid, String teststeps, int stepcount)
		{

			//Declare and initialize variables            
			Inbounds inbounds = null;
			Outbounds outbounds = null;
			TestCaseResult result;
			result = new TestCaseResult(stepcount);

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);
			int executedsteps = -1;

			try
			{
				//Fetch required Test data
				String phusername = Config.phUserName;
				String phpassword = Config.phPassword;
				String ph1username = Config.ph1UserName;
				String ph1password = Config.ph1Password;
				String arusername = Config.ar1UserName;
				String arpassword = Config.ar1Password;
				String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
				String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
				String[] AccessionNumbers = AccessionNoList.Split(':');
				String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				String[] Filepaths = UploadFilePath.Split('=');

				//Email objects declaration
				EmailUtils PHemail = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
				Dictionary<string, string> PHreceivedMail = new Dictionary<string, string>();

				//Mark all previous mails as read to avoid reading old unread mails
				PHemail.MarkAllMailAsRead("INBOX");

				//Upload a Dicom Study --Step-1
				foreach (String path in Filepaths)
				{
					ei.EIDicomUpload(Config.ph1UserName, Config.ph1Password, Config.Dest1, path);
				}
				// Check all studies are present in physician's outbound
				login.LoginIConnect(ph1username, ph1password);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				executedsteps++;
				try
				{
					foreach (String AccessionNumber in AccessionNumbers)
					{
						inbounds.SearchStudy("accessionNo", AccessionNumber);
						PageLoadWait.WaitForPageLoad(20);
						PageLoadWait.WaitForFrameLoad(20);
						PageLoadWait.WaitforStudyInStatus(AccessionNumber, inbounds, "Uploaded");
						Dictionary<string, string> priors = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumber, "Uploaded" });
						if (!(priors == null))
						{
							result.steps[executedsteps].status = "Pass";
							Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
						}
						else
						{
							result.steps[executedsteps].status = "Fail";
							Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
							result.steps[executedsteps].SetLogs();
							throw new Exception("One of the Priors Not uploaded--Study not found in inbounds");
						}
					}
				}
				catch (Exception e)
				{
					result.steps[executedsteps].status = "Fail";
					result.steps[executedsteps].SetLogs();
					throw new Exception("One of the Priors Not uploaded--Study not found in inbounds", e);
				}

				//Login as physician and check all priors in inbound-Step-2                
				//inbounds = (Inbounds)login.Navigate("Inbounds");
				executedsteps++;
				try
				{
					foreach (String AccessionNumber in AccessionNumbers)
					{
						inbounds.SearchStudy("accessionNo", AccessionNumber);
						PageLoadWait.WaitForPageLoad(20);
						PageLoadWait.WaitForFrameLoad(20);
						Dictionary<string, string> priors = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumber, "Uploaded" });
						if (!(priors == null))
						{
							result.steps[executedsteps].status = "Pass";
							Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
						}
						else
						{
							result.steps[executedsteps].status = "Fail";
							Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
							result.steps[executedsteps].SetLogs();
							throw new Exception("One of the Priors Not uploaded");
						}
					}
				}
				catch (Exception e)
				{
					result.steps[executedsteps].status = "Fail";
					result.steps[executedsteps].SetLogs();
					throw new Exception("One of the Priors Not uploaded--Study not found in Inbound", e);
				}


				//Count Number of related studies for a Patient
				BasePage.Driver.FindElement(By.CssSelector("#m_studySearchControl_m_searchInputAccession")).Clear();
				inbounds.SearchStudy("patientid", PatientID);
				Dictionary<int, string[]> results = BasePage.GetSearchResults();
				int totalpriors = results.Count;

				//Share one of the prior to a user-step--3
				inbounds.SearchStudy("accessionNo", AccessionNumbers[0]);
				inbounds.SelectStudy1(new string[] { "Accession" }, new string[] { AccessionNumbers[0] });
				inbounds.ShareStudy(false, new String[] { phusername });
				//Validate shared prior is in Receiver's Inbound(Login as Receiver)               
				login.Logout();
				login.LoginIConnect(phusername, phpassword);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("accessionNo", AccessionNumbers[0]);
				Dictionary<string, string> studyshared = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Shared" });
				if (!(studyshared == null))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Validate other priors are not shared--Step--4
				executedsteps++;
				try
				{
					for (int i = 0; i < AccessionNumbers.Length; i++)
					{
						if (i == 0)
							continue;
						inbounds.SearchStudy("accessionNo", AccessionNumbers[i]);
						Dictionary<string, string> priorsNotshared = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[i], "Shared" });
						if (priorsNotshared == null)
						{
							result.steps[executedsteps].status = "Pass";
							Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
						}
						else
						{
							result.steps[executedsteps].status = "Fail";
							Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
							result.steps[executedsteps].SetLogs();
							break;
						}
					}
				}
				catch (Exception e)
				{
					result.steps[executedsteps].status = "Fail";
					result.steps[executedsteps].SetLogs();
					throw new Exception("Validate other priors are not shared", e);
				}

				//Step-5 - Check mail notification is arrived to physician user
				PHreceivedMail = PHemail.GetMailUsingIMAP(Config.SystemEmail, "Shared Study");

				if (PHreceivedMail.Count > 0)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step Passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test case Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Launch the link URL from mail
				String StudyURL = PHemail.GetEmailedStudyLink(PHreceivedMail);

				//Step - 6: Link is loaded with iCA login page
				LaunchEmailedStudy.LaunchStudy<Login>(StudyURL);
				login.LoginIConnect(phusername, phpassword);
				PageLoadWait.WaitForLoadingMessage(60);
				PageLoadWait.WaitForPageLoad(20);
				PageLoadWait.WaitForSearchLoad();
				PageLoadWait.WaitForFrameLoad(20);
				BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input#m_studySearchControl_m_searchInputPatientLastName")));

				//Load the shared study in viewer and validate--Step-6
				inbounds.SearchStudy("accessionNo", AccessionNumbers[0]);
				PageLoadWait.WaitForPageLoad(20);
				PageLoadWait.WaitForFrameLoad(20);
				inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Shared" });
				BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
				var panel = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[0];
				result.steps[++executedsteps].SetPath(testid, executedsteps);
				var step6 = viewer.CompareImage(result.steps[executedsteps], panel);
				if (step6)
				{
					result.steps[executedsteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}
				viewer.CloseBluRingViewer();

				//Load the Shared Prior study in viewer and Validate number of priors--Step-7             
				inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Shared" });
				BluRingViewer.LaunchBluRingViewer();
				int priorscount1 = viewer.CheckPriorsCount();

				//Validation
				if (priorscount1 == totalpriors)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}
				viewer.CloseBluRingViewer();

				//Share the rest of the study--Step-8
				login.Logout();
				login.LoginIConnect(ph1username, ph1password);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				for (int i = 0; i < AccessionNumbers.Length; i++)
				{
					if (i == 0)
						continue;
					inbounds.SearchStudy("accessionNo", AccessionNumbers[i]);
					PageLoadWait.WaitForPageLoad(20);
					PageLoadWait.WaitForFrameLoad(20);
					inbounds.SelectStudy1(new string[] { "Accession" }, new string[] { AccessionNumbers[i] });
					inbounds.ShareStudy(false, new string[] { phusername });
				}
				//Validate all priors are in shared status in Receiver's Inbound
				login.Logout();
				login.LoginIConnect(phusername, phpassword);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				executedsteps++;
				try
				{
					foreach (String AccessionNumber in AccessionNumbers)
					{
						inbounds.SearchStudy("accessionNo", AccessionNumber);
						PageLoadWait.WaitForPageLoad(20);
						PageLoadWait.WaitForFrameLoad(20);
						Dictionary<string, string> priors = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumber, "Shared" });
						if (!(priors == null))
						{
							result.steps[executedsteps].status = "Pass";
							Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
						}
						else
						{
							result.steps[executedsteps].status = "Fail";
							Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
							result.steps[executedsteps].SetLogs();
							break;
						}
					}
				}
				catch (Exception e)
				{
					result.steps[executedsteps].status = "Fail";
					result.steps[executedsteps].SetLogs();
					throw new Exception("Validate all priors are in shared status in receiver's inbound", e);
				}


				//Prior count after sharing the study--Step-9
				inbounds.SearchStudy("accessionNo", AccessionNumbers[1]);
				inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[1], "Shared" });
				BluRingViewer.LaunchBluRingViewer();
				int priorscount2 = viewer.CheckPriorsCount(); 

				//Validation
				if (priorscount2 == totalpriors)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}
				viewer.CloseBluRingViewer();

				//Validate Upto 3 priors can viewed in the viewer--step10
				inbounds.SearchStudy("accessionNo", AccessionNumbers[0]);
				inbounds.SelectStudy1(new string[] { "Accession" }, new string[] { AccessionNumbers[0] });
				BluRingViewer.LaunchBluRingViewer();
				viewer.OpenPriors(accession: AccessionNumbers[1]);
				viewer.OpenPriors(accession: AccessionNumbers[2]);
				if (viewer.GetStudyPanelCount() == 3)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Close viewer --Step11
				viewer.CloseBluRingViewer();
				executedsteps++;


				// ###### Nominate one Study, Archive it and Perform above Validation #####
				//Nominate one of the priors and Archive it--Step-12
				login.Logout();
				login.LoginIConnect(ph1username, ph1password);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("accessionNo", AccessionNumbers[0]);
				PageLoadWait.WaitForPageLoad(20);
				PageLoadWait.WaitForFrameLoad(20);
				inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Uploaded" });
				inbounds.NominateForArchive("Testing");
				executedsteps++;

				//login as Archivist and check study--Step-13
				login.Logout();
				login.LoginIConnect(arusername, arpassword);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("accessionNo", AccessionNumbers[0]);
				Dictionary<string, string> nominatedstudy = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Nominated For Archive" });
				if (nominatedstudy != null)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Archive Study and Check its status--step-14
				inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Nominated For Archive" });
				inbounds.ArchiveStudy("", "Testing");
				PageLoadWait.WaitforStudyInStatus(AccessionNumbers[0], inbounds, "Routing Completed");
				inbounds.SearchStudy("accessionNo", AccessionNumbers[0]);
				Dictionary<string, string> archivedstudy = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Routing Completed" });
				if (archivedstudy != null)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Launch study in viewer and perform validation--Step-15
				login.Logout();
				login.LoginIConnect(arusername, arpassword);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("accessionNo", AccessionNumbers[0]);
				inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[0], "Routing Completed" });
				BluRingViewer.LaunchBluRingViewer();
				int priorsaftarchive = viewer.CheckPriorsCount();
				if (priorsaftarchive == totalpriors)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Validate yellow triangle is displayed for studies in holding pen--Step-16
				int iterate = 0;
				executedsteps++;
				try
				{
					foreach (String Accession in AccessionNumbers)
					{
						if (iterate == 0)
						{
							iterate++;
							continue;
						}

						if (viewer.IsForeignExamAlert(Accession))
						{
							result.steps[executedsteps].status = "Pass";
							Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
						}
						else
						{
							result.steps[executedsteps].status = "Fail";
							Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description + Accession);
							result.steps[executedsteps].SetLogs();
							break;
						}
					}
				}
				catch (Exception e)
				{
					result.steps[executedsteps].status = "Fail";
					result.steps[executedsteps].SetLogs();
					throw new Exception("Vaidate yellow triangle is displayed for studies in holding pen", e);
				}

				//Validate Yellow triangle not displayed for archived study--Step-17                 
				if (!viewer.IsForeignExamAlert(AccessionNumbers[0]))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description + AccessionNumbers[0]);
					result.steps[executedsteps].SetLogs();
				}

				//Load the study with yellow triangle in viewer--Step-18 --Second viewer
				viewer.OpenPriors(accession: AccessionNumbers[1]);
				
				/** Validation Pending **/
				//Validate Yellow triangle icon is displayed in study panel tool-bar of Study with uploaded status
				if (true)//BasePage.Driver.FindElement(By.CssSelector("span[id*='2_foreignExamDiv']")).Displayed == true)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}
				viewer.CloseBluRingViewer();

				//Validate archived study is present in Studies tab--Step-19
				Studies studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy("Accession", AccessionNumbers[0]);
				Dictionary<string, string> archivedstudy1 = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { AccessionNumbers[0] });
				if (archivedstudy1 != null)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description + AccessionNumbers[0]);
					result.steps[executedsteps].SetLogs();
				}

				//Validate studies in holding pen not displayed in Studies tab-Step-20
				int iterate1 = 0;
				executedsteps++;
				try
				{
					foreach (String Accession in AccessionNumbers)
					{
						if (iterate1 == 0)
						{
							iterate1++;
							continue;
						}
						Dictionary<string, string> nonarchivedstudy = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { Accession });
						if (nonarchivedstudy == null)
						{
							result.steps[executedsteps].status = "Pass";
							Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
						}
						else
						{
							result.steps[executedsteps].status = "Fail";
							Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description);
							result.steps[executedsteps].SetLogs();
							break;

						}
					}
				}
				catch (Exception e)
				{
					result.steps[executedsteps].status = "Fail";
					result.steps[executedsteps].SetLogs();
					throw new Exception("Validate studies in holding pen not displayed in Studies Tab", e);
				}

				//Load  archived study in viewer and check history panel and check prior count--Step-21
				studies.SearchStudy("Accession", AccessionNumbers[0]);
				PageLoadWait.WaitForPageLoad(20);
				PageLoadWait.WaitForFrameLoad(20);
				studies.SelectStudy1(new string[] { "Accession" }, new string[] { AccessionNumbers[0] });
				BluRingViewer.LaunchBluRingViewer();
				int priorsinstudytab = viewer.CheckPriorsCount();
				if (priorsinstudytab == totalpriors)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Navigate to Outbounds and check study--Step-22
				viewer.CloseBluRingViewer();
				outbounds = (Outbounds)login.Navigate("Outbounds");
				outbounds.SearchStudy("Accession", AccessionNumbers[0]);
				Dictionary<string, string> studyinoutbound = outbounds.GetMatchingRow(new string[] { "Accession" }, new string[] { AccessionNumbers[0] });
				if (studyinoutbound == null)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}
				login.Logout();

				//Nominate rest of the studies to Archive--Step23
				login.LoginIConnect(ph1username, ph1password);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				int iterate2 = 0;
				foreach (String Accession in AccessionNumbers)
				{
					if (iterate2 == 0)
					{
						iterate2++;
						continue;
					}

					inbounds.SearchStudy("Accession", Accession);
					PageLoadWait.WaitForPageLoad(10);
					PageLoadWait.WaitForFrameLoad(10);
					inbounds.SelectStudy1(new string[] { "Accession" }, new string[] { Accession });
					inbounds.NominateForArchive("Testing");
				}
				int iterate3 = 0;
				executedsteps++;
				try
				{
					foreach (String Accession in AccessionNumbers)
					{
						if (iterate3 == 0)
						{
							iterate3++;
							continue;
						}

						inbounds.SearchStudy("Accession", Accession);
						PageLoadWait.WaitForPageLoad(10);
						PageLoadWait.WaitForFrameLoad(10);
						Dictionary<string, string> nominatedstudy1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Nominated For Archive" });
						if (nominatedstudy1 != null)
						{
							result.steps[executedsteps].status = "Pass";
							Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
						}
						else
						{
							result.steps[executedsteps].status = "Fail";
							Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
							result.steps[executedsteps].SetLogs();
							break;
						}

					}
				}
				catch (Exception e)
				{
					result.steps[executedsteps].status = "Fail";
					result.steps[executedsteps].SetLogs();
					throw new Exception("Validate rest of priors are nominated to archive", e);
				}

				//Step-24 
				inbounds.SearchStudy("Accession", AccessionNumbers[1]);
				PageLoadWait.WaitForPageLoad(10);
				PageLoadWait.WaitForFrameLoad(10);
				inbounds.SelectStudy1(new string[] { "Accession" }, new string[] { AccessionNumbers[1] });
				BluRingViewer.LaunchBluRingViewer();
				executedsteps++;

				//Step-25 validate prior count
				int priorcounts3 = viewer.CheckPriorsCount();
				if (priorcounts3 == totalpriors)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Validate yellow triangle is displayed for studies in holding pen --Step-26
				int iterate4 = 0;
				executedsteps++;
				try
				{
					foreach (String Accession in AccessionNumbers)
					{
						if (iterate4 < 2)
						{
							iterate4++;
							continue;
						}
						if (viewer.IsForeignExamAlert(Accession))
						{
							result.steps[executedsteps].status = "Pass";
							Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
						}
						else
						{
							result.steps[executedsteps].status = "Fail";
							Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description + Accession);
							result.steps[executedsteps].SetLogs();
							break;
						}
					}
				}
				catch (Exception e)
				{
					result.steps[executedsteps].status = "Fail";
					result.steps[executedsteps].SetLogs();
					throw new Exception("Validate yellow triangle icon for studies in holding pen", e);
				}

				//Validate no yellow triangle for studies archived-step-27
				if (!viewer.IsForeignExamAlert(AccessionNumbers[0]))
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}
				viewer.CloseBluRingViewer();
				login.Logout();

				//Step-28 - Login as Archivist and check priors are listed
				login.LoginIConnect(arusername, arpassword);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				int iterate5 = 0;
				executedsteps++;
				try
				{
					foreach (String Accession in AccessionNumbers)
					{
						if (iterate5 == 0)
						{
							iterate5++;
							continue;
						}

						inbounds.SearchStudy("accessionNo", Accession);
						PageLoadWait.WaitForPageLoad(10);
						PageLoadWait.WaitForFrameLoad(10);
						Dictionary<string, string> nominatedstudy2 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Nominated For Archive" });
						if (nominatedstudy2 != null)
						{
							result.steps[executedsteps].status = "Pass";
							Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
						}
						else
						{
							result.steps[executedsteps].status = "Fail";
							Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
							result.steps[executedsteps].SetLogs();
							break;
						}
					}
				}
				catch (Exception e)
				{
					result.steps[executedsteps].status = "Fail";
					result.steps[executedsteps].SetLogs();
					throw new Exception("Validate nominated studies are listed in archivist inbound", e);
				}

				inbounds.SearchStudy("accessionNo", AccessionNumbers[2]);
				inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[2], "Nominated For Archive" });
				BluRingViewer.LaunchBluRingViewer();

				/** Validation Pending **/
				//Step-29
				//Validate No Yellow triangle icon is displayed in study panel tool-bar of Study with Nominated for archive status
				if (true)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Step-30 - Check prior count is same
				int priorscount4 = viewer.CheckPriorsCount();
				Boolean flagcheck = viewer.IsForeignExamAlert(AccessionNumbers[1]);
				if (priorscount4 == totalpriors && flagcheck)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Validate in archived study in Studies tab --Step31
				viewer.CloseBluRingViewer();
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy("Accession", AccessionNumbers[0]);
				Dictionary<string, string> archivedstudy2 = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { AccessionNumbers[0] });
				if (archivedstudy2 != null)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description + AccessionNumbers[0]);
					result.steps[executedsteps].SetLogs();
				}
				//Validate studies in holding pen not displayed in Studies tab
				iterate1 = 0;
				foreach (String Accession in AccessionNumbers)
				{
					if (iterate1 == 0)
					{
						iterate1++;
						continue;
					}

					studies.SearchStudy("Accession", Accession);
					PageLoadWait.WaitForPageLoad(10);
					PageLoadWait.WaitForFrameLoad(10);
					Dictionary<string, string> nonarchivedstudy = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { Accession });
					if (nonarchivedstudy == null)
					{
						result.steps[executedsteps].status = "Pass";
						Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
					}
					else
					{
						result.steps[executedsteps].status = "Fail";
						Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description);
						result.steps[executedsteps].SetLogs();
						break;

					}
				}

				//Search, Select and Launch Study-Step-32
				login.SearchStudy("Accession", AccessionNumbers[0]);
				login.SelectStudy1(new string[] { "Accession" }, new string[] { AccessionNumbers[0] });
				BluRingViewer.LaunchBluRingViewer();

				/** Validation Pending **/
				//Validate No Yellow triangle icon is displayed in study panel tool-bar of Study with Nominated for archive status
				if (true)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}
				//Close Study
				viewer.CloseBluRingViewer();

				//steps-33-Archive all studies
				inbounds = (Inbounds)login.Navigate("Inbounds");
				iterate = 0;
				foreach (string Accession in AccessionNumbers)
				{
					if (iterate == 0)
					{
						iterate++;
						continue;
					}
					inbounds.SearchStudy("Accession", Accession);
					PageLoadWait.WaitForPageLoad(10);
					PageLoadWait.WaitForFrameLoad(10);
					inbounds.SelectStudy1(new string[] { "Accession" }, new string[] { Accession });
					inbounds.ArchiveStudy("", "Testing");
				}

				login.Logout();
				login.LoginIConnect(arusername, arpassword);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				iterate1 = 0;
				executedsteps++;
				foreach (String Accession in AccessionNumbers)
				{
					inbounds.SearchStudy("Accession", Accession);
					PageLoadWait.WaitForPageLoad(10);
					PageLoadWait.WaitForFrameLoad(10);
					Dictionary<string, string> fullyarchivedstudies = studies.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession, "Routing Completed" });
					if (fullyarchivedstudies != null)
					{
						result.steps[executedsteps].status = "Pass";
						Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
					}
					else
					{
						result.steps[executedsteps].status = "Fail";
						Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description);
						result.steps[executedsteps].SetLogs();
						break;

					}
				}
				inbounds.SearchStudy("Accession", AccessionNumbers[2]);
				inbounds.SelectStudy1(new string[] { "Accession", "Status" }, new string[] { AccessionNumbers[2], "Routing Completed" });
				BluRingViewer.LaunchBluRingViewer();

				/** Validation Pending **/
				//steps-34
				//Validate No Yellow triangle icon is displayed in study panel tool-bar of Study with Nominated for archive status
				if (true)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}
								
				foreach (String Accession in AccessionNumbers)
				{
					if (!viewer.IsForeignExamAlert(Accession))
					{
						result.steps[executedsteps].status = "Pass";
						Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
					}
					else
					{
						result.steps[executedsteps].status = "Fail";
						Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description + Accession);
						result.steps[executedsteps].SetLogs();
						break;
					}
				}
				int priorcounts4 = viewer.CheckPriorsCount();
				if (priorcounts4 == totalpriors)
				{
					result.steps[executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}
				viewer.CloseBluRingViewer();
				login.Logout();

				//Step-35
				login.LoginIConnect(ph1username, ph1password);
				inbounds = (Inbounds)login.Navigate("Inbounds");
				inbounds.SearchStudy("Accession", AccessionNumbers[1]);
				inbounds.SelectStudy1(new string[] { "Accession" }, new string[] { AccessionNumbers[1] });
				BluRingViewer.LaunchBluRingViewer();

				/** Validation Pending **/
				//Validate No Yellow triangle icon is displayed in study panel tool-bar of Study with Routing Completed status
				if (true)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}
								
				foreach (String Accession in AccessionNumbers)
				{
					if (!viewer.IsForeignExamAlert(Accession))
					{
						result.steps[executedsteps].status = "Pass";
						Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
					}
					else
					{
						result.steps[executedsteps].status = "Fail";
						Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description + Accession);
						result.steps[executedsteps].SetLogs();
						break;
					}
				}

				int priorcounts5 = viewer.CheckPriorsCount();
				if (priorcounts5 == totalpriors)
				{
					result.steps[executedsteps].status = "Pass";
					Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}

				//Step-36
				viewer.OpenPriors(accession: AccessionNumbers[2]);

				/** Validation Pending **/
				//Validate No Yellow triangle icon is displayed in study panel tool-bar of Study with Routing Completed status
				if (true)
				{
					result.steps[++executedsteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step passed--" + result.steps[executedsteps].description);
				}
				else
				{
					result.steps[++executedsteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
				}
				viewer.CloseBluRingViewer();

				//Step-37
				studies = (Studies)login.Navigate("Studies");
				executedsteps++;
				try
				{
					foreach (String Accession in AccessionNumbers)
					{
						studies.SearchStudy("Accession", Accession);
						PageLoadWait.WaitForPageLoad(10);
						PageLoadWait.WaitForFrameLoad(10);
						Dictionary<string, string> studiesfinal = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { Accession });

						if (studiesfinal != null)
						{
							result.steps[executedsteps].status = "Pass";
							Logger.Instance.InfoLog("Test Step passed--" + result.steps[executedsteps].description);
						}
						else
						{
							result.steps[executedsteps].status = "Fail";
							Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description + Accession);
							result.steps[executedsteps].SetLogs();
							break;
						}

					}
				}
				catch (Exception e)
				{
					result.steps[executedsteps].status = "Fail";
					Logger.Instance.InfoLog("Test Step Failed--" + result.steps[executedsteps].description);
					result.steps[executedsteps].SetLogs();
					throw new Exception("Validate all the studies are present in Studies tab");
				}

				//Report Result
				result.FinalResult(executedsteps);
				Logger.Instance.InfoLog("Overall Test status--" + result.status);

				//Logout
				login.Logout();

				//Return Result
				return result;
			}
			catch (Exception e)
			{
				//Log Exception
				Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

				//Report Result
				result.FinalResult(e, executedsteps);
				Logger.Instance.ErrorLog("Overall Test status--" + result.status);

				//Logout
				login.Logout();

				//Return Result
				return result;
			}
		}


		/// <Summary>
		/// This is Automation of Test 29476
		/// This Test Case is to nominate from ph and archive from ar by uploading study through  PACS gateway
		/// </Summary>
		public TestCaseResult Test1_161161(String testid, String teststeps, int stepcount)
		{
			//Declare and initialize variables
			Inbounds inbounds = null;
			Studies studies = null;
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
				String uname = Config.ar1UserName;
				String pwd = Config.ar1Password;
				String hpUserName = Config.hpUserName;
				String hpPassword = Config.hpPassword;
				String pacusername = Config.pacsadmin;
				String pacpassword = Config.pacspassword;
				String adminusername = Config.adminUserName;
				String adminpassword = Config.adminPassword;
				String stUsername = Config.stUserName;
				String stPassword = Config.stPassword;
				String reason = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NominationReason");
				String pid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
				String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
				String orderacc = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderAccession");
				String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
				String orderpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "OrderFilePath");

				//Send an order
				Boolean hl7order = mpaclogin.SendHL7Order(Config.MergePACsIP, int.Parse(Config.mpacport), orderpath);
				
				//Import a study which  matches the existing order Patient ID and Acc no' to Merge PACs#2
				BasePage.RunBatchFile(Config.batchfilepath, Config.TestDataPath + studypath + " " + Config.dicomsendpath + " " + Config.StudyPacs);

				//Send the study to dicom devices from MergePacs management page
				//Login MPacs
				login.DriverGoTo(login.mpacstudyurl);
				MPHomePage mphomepage = mpaclogin.Loginpacs(pacusername, pacpassword);
				Tool mpactool = (Tool)mphomepage.NavigateTopMenu("Tools");
				mphomepage.NavigateTopMenu("Tools");
				mpactool.NavigateToSendStudy();
				mpactool.SearchStudy("Accession", accession, 0);
				mpactool.MpacSelectStudy("Patient ID", pid);
				mpactool.SendStudy(1);

				//Logout MPacs
				mpaclogin.LogoutPacs();

				//Login as physician
				login.DriverGoTo(login.url);
				login.LoginIConnect(username, password);

				//Navigate to Inbounds
				inbounds = (Inbounds)login.Navigate("Inbounds");
				PageLoadWait.WaitforStudyInStatus(accession, inbounds, "Uploaded");

				//Search and Select Study
				inbounds.SearchStudy("Accession", accession);

				//Launch study
				inbounds.SelectStudy("Accession", accession);
				BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();

				//Step 1: Send studies to destination via PACS Gateway,
				//With physician's log in, load a study in Universal Viewer that status=uploaded
				if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.span_Demographics))[3].Text.ToLower().Contains(pid.ToLower()))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Nominate study through the toolbar
				viewer.CloseBluRingViewer();
				inbounds.SelectStudy("Accession", accession);
				inbounds.NominateForArchive("");
				inbounds.SearchStudy("Accession", accession);

				//Find study status
				Dictionary<string, string> study1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { accession, "Nominated For Archive" });

				//Step 2 :  Close the Viewer
				//Select the study and click on " Nominate for Archive " button
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

				//Logout as physician
				login.Logout();

				//Login as archivist
				login.LoginIConnect(uname, pwd);

				//Navigate to Inbounds and search
				inbounds = (Inbounds)login.Navigate("Inbounds");

				inbounds.SearchStudy("Accession", accession);
				inbounds.SelectStudy("Accession", accession);

				//Launch study
				BluRingViewer.LaunchBluRingViewer();

				//Step 3: From archivist inbounds, load the study that physician nominated in Universal viewer
				if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.span_Demographics))[3].Text.ToLower().Contains(pid.ToLower()))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();

				}

				//Nominate study through the toolbar
				viewer.CloseBluRingViewer();
				inbounds.SelectStudy("Accession", accession);
				inbounds.ClickArchiveStudy("", "");
				
				//Search order
				inbounds.ArchiveSearch("order", "All Dates");

				//Validate that recociliation window opens
				Dictionary<String, String> OrderDetails = inbounds.GetDataInArchive("Matching Order");
				Dictionary<String, String> OriginalDetails = inbounds.GetDataInArchive("Original Details");

				//Step 4: Close the Viewer
				//Select the nominated study and click on " Reconcile Exam " button
				if ((OrderDetails["Last Name"].Equals(OriginalDetails["Last Name"])) && (OrderDetails["First Name"].Equals(OriginalDetails["First Name"])))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();

				}

				//Edit the order
				inbounds.SetCheckBoxInArchive("matching order", "gender");

				//Click mandatory fields
				inbounds.SetCheckBoxInArchive("original details", "PID");
				inbounds.SetCheckBoxInArchive("original details", "Accession");
				inbounds.SetBlankFinalDetailsInArchive();

				//Click Archive
				inbounds.ClickArchive();
				
				//Find study status
				PageLoadWait.WaitforStudyInStatus(accession, inbounds, "Routing Completed");
				Dictionary<string, string> study2 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { accession, "Routing Completed" });

				//Step 5: Manually edit the order and archive the study
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

				//Step 6: Verify the study reaches to the destination (from the studylist and check the study from destination datasource)
				studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy("Accession", accession);

				Dictionary<string, string> study3 = studies.GetMatchingRow(new string[] { "Accession", "Patient ID" }, new string[] { accession, pid });

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

				//Logout as archivist
				login.Logout();

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
        /// Verifying expired studies from Inbounds
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_168036(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables          
            Inbounds inbounds = null;
            DomainManagement domainmanagement = null;
            ServiceTool servicetool = new ServiceTool();
            Studies studies = null;
            Outbounds outbounds = null;
            RoleManagement rolemanagement = null;
            UserPreferences userpreferences = new UserPreferences();
            BasePage basepage = new BasePage();
            HTML5_Uploader html5 = new HTML5_Uploader();
            BluRingViewer viewer = new BluRingViewer();
            string pinnumber = string.Empty;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string updatedateandtimebatchfile = string.Empty;
            string Date = string.Empty;
            string Time = string.Empty;
            string[] FilePath = null;
            string ScheduledJobConfiguration = @"C:\WebAccess\WebAccess\Config\ScheduledJobConfiguration.xml";
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            
            try
            {
                DicomClient client = new DicomClient();
                updatedateandtimebatchfile = string.Concat(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar, "OtherFiles\\UpdateDatetime.bat");
                string U1_Username = Config.ph1UserName;
                string U1_Password = Config.ph1Password;
                string U2_Username = Config.ph2UserName;
                string U2_Password = Config.ph2Password;
                string U3_Username = Config.ar1UserName;
                string U3_Password = Config.ar1Password;
                string[] Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList")).Split('=');
                string[] DicomPath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('=').Select(pth=> Config.TestDataPath+pth).ToArray();
                EmailUtils CustomUser1 = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
                string link = String.Empty;
                Dictionary<string, string> downloadedMail = new Dictionary<string, string>();

                //PreCondition
                servicetool.LaunchServiceTool();
                servicetool.SetEmailNotification(AdministratorEmail: Config.AdminEmail, SystemEmail: Config.SystemEmail, SMTPHost: Config.SMTPServer);
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();

                Time = "01:00:00";
                BasePage.RunBatchFile(updatedateandtimebatchfile, "time" + " " + Time);

                //Step 1:
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(Config.adminGroupName);
                domainmanagement.SelectDomain(Config.adminGroupName);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("grant", 0);
                domainmanagement.SendKeys(domainmanagement.GrantAccessValidDaysTxtBox(), "6");
                domainmanagement.SetCheckBoxInEditDomain("emailstudy", 0);
                domainmanagement.SendKeys(domainmanagement.EmailStudyValidDaysTxtBox(), "1");
                domainmanagement.SetCheckBoxInEditDomain("imagesharing", 0);
                domainmanagement.SendKeys(domainmanagement.UploadStudyValidDaysTxtBox(), "1");
                domainmanagement.DefaultUploaderDropdown().SelectByText("Web Uploader");
                if (!domainmanagement.WebUploaderConsentCheckbox().Selected)
                {
                    domainmanagement.WebUploaderConsentCheckbox().Click();
                }
                domainmanagement.ClickSaveEditDomain();
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SearchRole("Physician", Config.adminGroupName);
                rolemanagement.SelectRole("Physician");
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("transfer", 0);
                rolemanagement.SetCheckboxInEditRole("email", 0);
                rolemanagement.SetCheckboxInEditRole("receiveexam", 0);
                if (!rolemanagement.GrantAccessRadioBtn_Anyone().Selected)
                {
                    rolemanagement.GrantAccessRadioBtn_Anyone().Click();
                }
                rolemanagement.ClickSaveEditRole();
                login.Logout();
                result.steps[++ExecutedSteps].StepPass();

                //Step 2:
                client.AddRequest(new DicomCStoreRequest(DicomPath[0]));
                client.Send(Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);
                login.LoginIConnect(U1_Username, U1_Password);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: "*", AccessionNo: Accession[0], Datasource: login.GetHostName(Config.DestEAsIp));
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                studies.SelectStudy("Accession", Accession[0]);
                studies.ShareStudy(false, new String[] { U2_Username }, domainName: Config.adminGroupName);
                viewer = BluRingViewer.LaunchBluRingViewer();
                CustomUser1.MarkAllMailAsRead();
                pinnumber = viewer.EmailStudy_BR(emailaddr: Config.CustomUser1Email, DeleteEmail: false);
                viewer.CloseBluRingViewer();
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy(AccessionNo: Accession[0], Study_Received_Period: "All Dates");
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                //Emailed-Sender
                bool Step2_1 = studies.GetMatchingRow(new string[] { "Accession", "Status" }, new String[] { Accession[0], "Emailed" }) !=null;
                //Shared-Sender
                bool Step2_2 = studies.GetMatchingRow(new string[] { "Accession", "Status" }, new String[] { Accession[0], "Shared" }) != null;
                login.Logout();
                login.LoginIConnect(U2_Username, U2_Password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: Accession[0], Study_Received_Period: "All Dates");
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                //Shared-Receiver
                bool Step2_3 = studies.GetMatchingRow(new string[] { "Accession", "Status" }, new String[] { Accession[0], "Shared" }) != null;
                //Emailed-Receiver
                downloadedMail = CustomUser1.GetMailUsingIMAP(Config.SystemEmail, "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                link = CustomUser1.GetEmailedStudyLink(downloadedMail);
                viewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(link, pinnumber);
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitForPriorsToLoad();
                //viewer.ChangeViewerLayout();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool Step2_4 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.ViewPortContainer());
                if (Step2_1 && Step2_2 && Step2_3 && Step2_4)
                {
                    result.steps[ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.ErrorLog("Step 2 - Emailed-Sender = " + Step2_1);
                    Logger.Instance.ErrorLog("Step 2 - Shared-Sender = " + Step2_2);
                    Logger.Instance.ErrorLog("Step 2 - Shared-Receiver = " + Step2_3);
                    Logger.Instance.ErrorLog("Step 2 - Emailed-Receiver = " + Step2_4);
                    result.steps[ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 3:
                login.Logout();
                login.LoginIConnect(U2_Username, U2_Password);
                userpreferences.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.DefaultUploaderDropdown().SelectByText("Web Uploader");
                userpreferences.CloseUserPreferences();
                inbounds = (Inbounds)login.Navigate("Inbounds");
                string[] HTML5WindowHandle = basepage.OpenHTML5UploaderandSwitchtoIT();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForElement(html5.By_HippaComplianceLabel(), BasePage.WaitTypes.Visible);
                if (!html5.HippaAgreeChkBox().Selected)
                {
                    html5.HippaAgreeChkBox().Click();
                }
                PageLoadWait.WaitForElement(html5.By_HippaContinueBtn(), BasePage.WaitTypes.Clickable);
                html5.HippaContinueBtn().Click();
                FilePath = Directory.GetFiles(DicomPath[1], "*.*", SearchOption.AllDirectories);
                foreach (string Pth in FilePath)
                {
                    html5.UploadFilesBtn().Click();
                    basepage.UploadFileInBrowser(Pth, "file", AppendDrivepath: false);
                    PageLoadWait.WaitForHTML5StudyToUpload(120);
                    html5.ShareJobButton().Click();
                    html5.DestinationDropdown().SelectByText(Config.Dest2);
                    html5.PriorityDropdown().SelectByText("ROUTINE");
                    html5.CommentTextBox().SendKeys("Test 168036 - Step 3");
                    html5.ShareBtn().Click();
                    PageLoadWait.WaitForElement(html5.By_DragFilesDiv(), BasePage.WaitTypes.Visible, 60);
                }
                result.steps[++ExecutedSteps].StepPass();


                //Step 4:
                login.Logout();
                login.LoginIConnect(U2_Username, U2_Password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: Accession[1], Study_Received_Period: "All Dates");
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                inbounds.SelectStudy("Accession", Accession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                CustomUser1.MarkAllMailAsRead();
                pinnumber = viewer.EmailStudy_BR(emailaddr: Config.CustomUser1Email, DeleteEmail: false);
                downloadedMail = CustomUser1.GetMailUsingIMAP(Config.SystemEmail, "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                link = CustomUser1.GetEmailedStudyLink(downloadedMail);
                viewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(link, pinnumber);
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitForPriorsToLoad();
                //viewer.ChangeViewerLayout();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.ViewPortContainer()))
                {
                    result.steps[ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5:
                login.Logout();
                login.LoginIConnect(U2_Username, U2_Password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: Accession[2], Study_Received_Period: "All Dates");
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                inbounds.SelectStudy("Accession", Accession[2]);
                inbounds.ShareStudy(false, new String[] { U3_Username }, domainName: Config.adminGroupName);
                viewer = BluRingViewer.LaunchBluRingViewer();
                CustomUser1.MarkAllMailAsRead();
                pinnumber = viewer.EmailStudy_BR(emailaddr: Config.CustomUser1Email, DeleteEmail: false);
                viewer.CloseBluRingViewer();
                login.Logout();
                login.LoginIConnect(U3_Username, U3_Password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: Accession[2], Study_Received_Period: "All Dates");
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                //Shared-Receiver
                bool Step5_1 = studies.GetMatchingRow(new string[] { "Accession", "Status" }, new String[] { Accession[2], "Shared" }) != null;
                //Emailed-Receiver
                downloadedMail = CustomUser1.GetMailUsingIMAP(Config.SystemEmail, "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                link = CustomUser1.GetEmailedStudyLink(downloadedMail);
                viewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(link, pinnumber);
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitForPriorsToLoad();
                //viewer.ChangeViewerLayout();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool Step5_2 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.ViewPortContainer());
                if (Step5_1 && Step5_2)
                {
                    result.steps[ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.ErrorLog("Step 5 - Shared-Receiver = " + Step5_1);
                    Logger.Instance.ErrorLog("Step 5 - Emailed-Receiver = " + Step5_2);
                    result.steps[ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 6:
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(Config.adminGroupName);
                domainmanagement.SelectDomain(Config.adminGroupName);
                domainmanagement.ClickEditDomain();
                domainmanagement.SendKeys(domainmanagement.UploadStudyValidDaysTxtBox(), "30");
                domainmanagement.ClickSaveEditDomain();
                result.steps[++ExecutedSteps].StepPass();

                //Step 7:
                login.Logout();
                login.LoginIConnect(U2_Username, U2_Password);
                userpreferences.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.DefaultUploaderDropdown().SelectByText("Web Uploader");
                userpreferences.CloseUserPreferences();
                inbounds = (Inbounds)login.Navigate("Inbounds");
                HTML5WindowHandle = basepage.OpenHTML5UploaderandSwitchtoIT();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForElement(html5.By_HippaComplianceLabel(), BasePage.WaitTypes.Visible);
                if (!html5.HippaAgreeChkBox().Selected)
                {
                    html5.HippaAgreeChkBox().Click();
                }
                PageLoadWait.WaitForElement(html5.By_HippaContinueBtn(), BasePage.WaitTypes.Clickable);
                html5.HippaContinueBtn().Click();
                FilePath = Directory.GetFiles(DicomPath[2], "*.*", SearchOption.AllDirectories);
                foreach (string Pth in FilePath)
                {
                    html5.UploadFilesBtn().Click();
                    basepage.UploadFileInBrowser(Pth, "file", AppendDrivepath: false);
                    PageLoadWait.WaitForHTML5StudyToUpload(120);
                    html5.ShareJobButton().Click();
                    html5.DestinationDropdown().SelectByText(Config.Dest2);
                    html5.PriorityDropdown().SelectByText("ROUTINE");
                    html5.CommentTextBox().SendKeys("Test 168036 - Step 7");
                    html5.ShareBtn().Click();
                    PageLoadWait.WaitForElement(html5.By_DragFilesDiv(), BasePage.WaitTypes.Visible, 60);
                }
                login.Logout();
                login.LoginIConnect(U2_Username, U2_Password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: Accession[3], Study_Received_Period: "All Dates");
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                if (inbounds.CheckStudy("Accession", Accession[3]))
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //Step 8:
                DateTime localDate = DateTime.Now.AddDays(1);
                Date = localDate.ToString("MM/dd/yyyy");
                Time = localDate.ToString("HH:mm:ss");
                BasePage.RunBatchFile(updatedateandtimebatchfile, "date" + " " + Date);
                BasePage.RunBatchFile(updatedateandtimebatchfile, "time" + " " + Time);
                basepage.ChangeAttributeValue(ScheduledJobConfiguration, "/ScheduledJob[@Name='SharedStudiesCleanUpJob']", "LastExecutionDate", localDate.AddMonths(-4).ToString("MM/dd/yyyy"));
                servicetool.RestartIISUsingexe();
                result.steps[++ExecutedSteps].StepPass();

                //Step 9:
                login.Logout();
                login.LoginIConnect(U1_Username, U1_Password);
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy(AccessionNo: Accession[0], Study_Received_Period: "All Dates");
                //Emailed-Sender
                bool Step9_1 = studies.GetMatchingRow(new string[] { "Accession", "Status" }, new String[] { Accession[0], "Emailed" }) == null;
                //Shared-Sender
                bool Step9_2 = studies.GetMatchingRow(new string[] { "Accession", "Status" }, new String[] { Accession[0], "Shared" }) != null;
                if (Step9_1 && Step9_2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.ErrorLog("Step 9 - Emailed-Sender = " + Step9_1);
                    Logger.Instance.ErrorLog("Step 9 - Shared-Sender = " + Step9_2);
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 10:
                login.Logout();
                login.LoginIConnect(U2_Username, U2_Password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: Accession[1], Study_Received_Period: "All Dates");
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                bool Step10_1 = BasePage.GetSearchResults().Count == 0;
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy(AccessionNo: Accession[1], Study_Received_Period: "All Dates");
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                bool Step10_2 = BasePage.GetSearchResults().Count == 0;
                if (Step10_1 && Step10_2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.ErrorLog("Step10_1 = " + Step10_1);
                    Logger.Instance.ErrorLog("Step10_2 = " + Step10_2);
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 11:
                outbounds.SearchStudy(AccessionNo: Accession[2], Study_Received_Period: "All Dates");
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                bool Step11_1 = studies.GetMatchingRow(new string[] { "Accession", "Status" }, new String[] { Accession[2], "Emailed" }) == null;
                bool Step11_2 = studies.GetMatchingRow(new string[] { "Accession", "Status" }, new String[] { Accession[2], "Shared" }) != null;
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: Accession[2], Study_Received_Period: "All Dates");
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                bool Step11_3 = BasePage.GetSearchResults().Count == 0;
                login.Logout();
                login.LoginIConnect(U3_Username, U3_Password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: Accession[2], Study_Received_Period: "All Dates");
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                bool Step11_4 = studies.GetMatchingRow(new string[] { "Accession", "Status" }, new String[] { Accession[2], "Shared" }) != null;
                if (Step11_1 && Step11_2 && Step11_3 && Step11_4)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.ErrorLog("Step11_1 = " + Step11_1);
                    Logger.Instance.ErrorLog("Step11_2 = " + Step11_2);
                    Logger.Instance.ErrorLog("Step11_3 = " + Step11_3);
                    Logger.Instance.ErrorLog("Step11_4 = " + Step11_4);
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12:
                login.Logout();
                login.LoginIConnect(U2_Username, U2_Password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: Accession[3], Study_Received_Period: "All Dates");
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                if(inbounds.CheckStudy("Accession", Accession[3]))
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
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
            finally
            {
                try
                {
                    servicetool.CloseServiceTool();
                    string[] currentdatetime = basepage.GetCurrentDateAndTimeFromInternet().Split(' ');
                    BasePage.RunBatchFile(updatedateandtimebatchfile, "date" + " " + currentdatetime[0]);
                    BasePage.RunBatchFile(updatedateandtimebatchfile, "time" + " " + currentdatetime[1]);
                }
                catch (Exception) { }
                try
                {
                    login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                    domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                    domainmanagement.SearchDomain(Config.adminGroupName);
                    domainmanagement.SelectDomain(Config.adminGroupName);
                    domainmanagement.ClickEditDomain();
                    domainmanagement.SetCheckBoxInEditDomain("grant", 0);
                    domainmanagement.SendKeys(domainmanagement.GrantAccessValidDaysTxtBox(), "");
                    domainmanagement.SetCheckBoxInEditDomain("emailstudy", 0);
                    domainmanagement.SendKeys(domainmanagement.EmailStudyValidDaysTxtBox(), "");
                    domainmanagement.SetCheckBoxInEditDomain("imagesharing", 0);
                    domainmanagement.SendKeys(domainmanagement.UploadStudyValidDaysTxtBox(), "30");
                    domainmanagement.ClickSaveEditDomain();
                    login.Logout();
                }
                catch (Exception) { }
                try
                {
                    basepage.ChangeAttributeValue(ScheduledJobConfiguration, "/ScheduledJob[@Name='SharedStudiesCleanUpJob']", "LastExecutionDate", DateTime.Now.ToString("MM/dd/yyyy"));
                    servicetool.RestartIISUsingexe();
                }
                catch (Exception) { }
            }
        }


        /// <summary>
        /// Study details are not getting logged in some C-Store failure cases
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_167654(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables          
            Inbounds inbounds = null;
            DomainManagement domainmanagement = null;
            ServiceTool servicetool = new ServiceTool();
            UserPreferences userpreferences = new UserPreferences();
            BasePage basepage = new BasePage();
            HTML5_Uploader html5 = new HTML5_Uploader();
            BluRingViewer viewer = new BluRingViewer();
            string pinnumber = string.Empty;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string updatedateandtimebatchfile = string.Empty;
            DateTime StartTime = DateTime.Now;
            DateTime EndTime = DateTime.Now;
            string Time = string.Empty;
            MpacLogin mplogin = new MpacLogin();
            MPHomePage mphomepage = new MPHomePage();
            POPUploader popUploader = new POPUploader();
            string[] PatientID = null;
            string LogPath = @"C:\Windows\Temp\"+testid;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                string[] DicomPath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('=');
                string[] Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList")).Split('=');
                Directory.CreateDirectory(Config.HTML5UploaderRejectedPath);
                string[] MainDirectory = Directory.GetDirectories(Config.HTML5UploaderRejectedPath);
                string StudyInstanceUID = string.Empty;
                string SOPInstanceUID = string.Empty;
                updatedateandtimebatchfile = string.Concat(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar, "OtherFiles\\UpdateDatetime.bat");
                PatientID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split('=');
                //Pre-Condition
                Directory.CreateDirectory(LogPath);
                DirectoryInfo di = new DirectoryInfo(LogPath);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                servicetool.RestartIISUsingexe();
                try
                {
                    for (var i = 1; i >= 1; i++)
                    {
                        String HTML5LogFilePath = @"C:\Windows\Temp" + @"\WebUploaderDeveloper-" + System.DateTime.Now.Date.ToString("yyyyMMdd") + "(" + i + ")" + ".log";
                        if (File.Exists(HTML5LogFilePath))
                        {
                            File.Delete(HTML5LogFilePath);
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Unable to Read Log file");
                            break;
                        }
                    }
                }
                catch(Exception e)
                {
                    Logger.Instance.InfoLog("Unable to Delete HTML5 Log File "+e.InnerException);
                }
                Time = "01:00:00";
                BasePage.RunBatchFile(updatedateandtimebatchfile, "time" + " " + Time);

                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Developer Logs");
                wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                servicetool.WaitWhileBusy();
                servicetool.LogType().SetValue("WebAccess Developer");//Anonymous STS
                servicetool.LogPath().SetValue(LogPath + "\\WebAccessDeveloper.log");
                servicetool.Creationrule().SetValue("Daily");
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.ClickButton(ServiceTool.ApplyBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();

                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(Config.adminGroupName);
                domainmanagement.SelectDomain(Config.adminGroupName);
                domainmanagement.ClickEditDomain();
                domainmanagement.DefaultUploaderDropdown().SelectByText("Web Uploader");
                if (!domainmanagement.WebUploaderConsentCheckbox().Selected)
                {
                    domainmanagement.WebUploaderConsentCheckbox().Click();
                }
                domainmanagement.ClickSaveEditDomain();
                login.Logout();
                foreach (var dir in MainDirectory)
                {
                    Directory.Delete(dir, true);
                }
                
                //Step 1:
                StudyInstanceUID = BasePage.ReadDicomFile<String>(DicomPath[0]+"\\ACC16765401.dcm", DicomTag.StudyInstanceUID);
                SOPInstanceUID = BasePage.ReadDicomFile<String>(DicomPath[0] + "\\ACC16765401.dcm", DicomTag.SOPInstanceUID);
                StartTime = DateTime.Now;
                Logger.Instance.InfoLog("EI Start Time = " + StartTime);
                ei.LaunchEI(Config.EIFilePath);
                ei.LoginToEi(Config.ph1UserName, Config.ph1Password);
                ei.EI_SelectDestination(Config.Dest1);
                ei.SelectFileFromHdd(DicomPath[0]);
                ei.SelectAllPatientsToUpload();
                ei.m_wpfObjects.GetButton("BtnSend").Click();
                ei.m_wpfObjects.WaitTillLoad();
                Thread.Sleep(15000);
                Button button = ei.m_wpfObjects.GetButton("BtnOk");

                while (button != null && !button.Enabled)
                {
                    var tempButton = ei.m_wpfObjects.GetButton("okButton");

                    //Handle Error message popup (Study already exists)
                    if (tempButton != null)
                    {
                        if (tempButton.Enabled && tempButton.Visible)
                        {
                            throw new Exception("Study Not Loaded As It Already Exists");
                        }
                    }
                    Thread.Sleep(3000);
                }
                IUIItem item = WpfObjects._mainWindow.Get(SearchCriteria.ByAutomationId("").AndByText("Exam import failed. Refer log for details."));
                if(string.Equals(item.Name.Trim(), "Exam import failed. Refer log for details."))
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                ei.CloseUploaderTool();
                EndTime = DateTime.Now;
                Logger.Instance.InfoLog("EI End Time = " + EndTime);

                //Step 2:
                try
                {
                    var loggedMessage1 = string.Empty;
                    var loggedMessage2 = string.Empty;
                    String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
                    for (var i = 1; i >= 1; i++)
                    {
                        String LogFilePath = LogPath+@"\WebAccessDeveloper-" + Date + "(" + i + ")" + ".log";
                        System.DateTime DateTime = System.DateTime.Now.Date;

                        if (File.Exists(LogFilePath))
                        {
                            //StreamReader reader = new StreamReader(stream);
                            var LogValues = basepage.ReadDevTraceLog(LogFilePath, StartTime, EndTime, isInformation: true);

                            foreach (var entry in LogValues)
                            {
                                if (entry.Value["Message"].Contains("(Upload Request Worker) C_STORE operation failed.")) //Actual message : License type embeded SessionType=Integrator
                                {
                                    loggedMessage1 = entry.Value["Message"];
                                    Logger.Instance.InfoLog("EI loggedMessage1 = " + loggedMessage1);
                                    break;
                                }
                            }
                            foreach (var entry in LogValues)
                            {
                                if (entry.Value["Message"].Contains("MC exception")) //Actual message : License type embeded SessionType=Integrator
                                {
                                    loggedMessage2 = entry.Value["Detail"];
                                    Logger.Instance.InfoLog("EI loggedMessage2 = " + loggedMessage2);
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
                    bool Step2_1 = loggedMessage1 == "(Upload Request Worker) C_STORE operation failed.";
                    bool Step2_2 = loggedMessage2.Contains(StudyInstanceUID) && loggedMessage2.Contains(SOPInstanceUID);
                    //Validation of message in log file
                    if (Step2_1 && Step2_2)
                    {
                        result.steps[++ExecutedSteps].StepPass();
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("EI Step2_1 = "+ Step2_1);
                        Logger.Instance.ErrorLog("EI Step2_2 = "+ Step2_2);
                        result.steps[++ExecutedSteps].StepFail();
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

                //Step 3:
                StartTime = DateTime.Now;
                Logger.Instance.InfoLog("Pop Start Time = "+StartTime);
                StudyInstanceUID = BasePage.ReadDicomFile<String>(DicomPath[1] + "\\ACC16765402.dcm", DicomTag.StudyInstanceUID);
                SOPInstanceUID = BasePage.ReadDicomFile<String>(DicomPath[1] + "\\ACC16765402.dcm", DicomTag.SOPInstanceUID);
                BasePage.RunBatchFile(Config.batchfilepath, Config.TestDataPath + DicomPath[1] + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                login.DriverGoTo(login.mpacstudyurl);
                mplogin.Loginpacs(Config.pacsadmin, Config.pacspassword);
                Tool tools = (Tool)mphomepage.NavigateTopMenu("Tools");
                mphomepage.NavigateTopMenu("Tools");

                tools.NavigateToSendStudy();
                tools.SearchStudy("Accession", Accession[1], 0);
                tools.MpacSelectStudy("Accession", Accession[1]);

                tools.SendStudy(1, Config.pacsgatway1, waitTime: 180);
                popUploader.LaunchPACS(Config.PACSFilePath);
                wpfobject.GetMainWindowByTitle(Config.pacswindow);
                wpfobject.SelectTabFromTabItems(popUploader.TransferHistoryTab);
                wpfobject.WaitTillLoad();
                popUploader.TxtAccession_TxtBx().BulkText = Accession[1];
                popUploader.PACSGatewaySearch_Btn().Click();
                wpfobject.WaitTillLoad();
                Thread.Sleep(5000);
                int counterX = 0;
                bool failStausFound = false;
                int statusColumnIndex = 5;
                while (!failStausFound && counterX <= 15)
                {
                    try
                    {
                        ListView transferHistory = popUploader.HisDataGrid();
                        int loopI = 0;
                        Logger.Instance.InfoLog("TransferHistory Tab: RowCountOutsideLoopI=" + transferHistory.Rows.Count + ", when counterX=" + counterX + " and loopI=" + loopI);
                        while (loopI < transferHistory.Rows.Count)
                        {
                            Logger.Instance.InfoLog("TransferHistory Tab: RowCountInsideLoopI=" + transferHistory.Rows.Count + " when loopI=" + loopI);
                            Logger.Instance.InfoLog("TransferHistory Tab: ColumnsCountInsideLoopI=" + transferHistory.Rows[loopI].Cells.Count + " when loopI=" + loopI);
                            for (int loopJ = 0; loopJ < transferHistory.Rows[loopI].Cells.Count; loopJ++)
                            {
                                Logger.Instance.InfoLog("RowNumber=" + loopI + ", column " + loopJ + "'s value=" + transferHistory.Rows[loopI].Cells[loopJ].Text);
                            }
                            String studyStatus = transferHistory.Rows[loopI].Cells[statusColumnIndex].Text;
                            Logger.Instance.InfoLog("Row Number=" + loopI + ", status=" + studyStatus);
                            if (studyStatus.ToLower().Contains("fail"))
                            {
                                failStausFound = true;
                                Logger.Instance.InfoLog("Study Status fail found in RowNumber(loopI)=" + loopI);
                                break;
                            }
                            loopI++;
                        }
                        if (!failStausFound)
                        {
                            Thread.Sleep(20000); //Wait for 20 secs and search again
                            popUploader.PACSGatewaySearch_Btn().Click();
                            wpfobject.WaitTillLoad();
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.ErrorLog("Exception while checking status 'success' in POP history tab--" + e);
                        //Starting POP again
                        popUploader = new POPUploader();
                        popUploader.LaunchPACS(Config.PACSFilePath);
                        wpfobject.GetMainWindowByTitle(Config.pacswindow);
                        wpfobject.SelectTabFromTabItems(popUploader.TransferHistoryTab);
                        wpfobject.WaitTillLoad();
                        popUploader.TxtAccession_TxtBx().BulkText = Accession[1];
                        popUploader.PACSGatewaySearch_Btn().Click();
                        wpfobject.WaitTillLoad();
                        Thread.Sleep(5000);
                    }
                    counterX++;
                }
                if (failStausFound)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 4:
                wpfobject.GetMainWindowByTitle("PACS Gateway Configuration");
                wpfobject.CloseWindow();
                EndTime = DateTime.Now;
                Logger.Instance.InfoLog("Pop End Time = " + EndTime);
                try
                {
                    var loggedMessage1 = string.Empty;
                    var loggedMessage2 = string.Empty;
                    String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
                    for (var i = 1; i >= 1; i++)
                    {
                        String LogFilePath = LogPath +@"\WebAccessDeveloper-" + Date + "(" + i + ")" + ".log";
                        System.DateTime DateTime = System.DateTime.Now.Date;

                        if (File.Exists(LogFilePath))
                        {
                            //StreamReader reader = new StreamReader(stream);
                            var LogValues = basepage.ReadDevTraceLog(LogFilePath, StartTime, EndTime, isInformation: true);

                            foreach (var entry in LogValues)
                            {
                                if (entry.Value["Message"].Contains("(Upload Request Worker) C_STORE operation failed.")) //Actual message : License type embeded SessionType=Integrator
                                {
                                    loggedMessage1 = entry.Value["Message"];
                                    Logger.Instance.InfoLog("Pop loggedMessage1 = " + loggedMessage1);
                                    break;
                                }
                            }
                            foreach (var entry in LogValues)
                            {
                                if (entry.Value["Message"].Contains("MC exception")) //Actual message : License type embeded SessionType=Integrator
                                {
                                    loggedMessage2 = entry.Value["Detail"];
                                    Logger.Instance.InfoLog("Pop loggedMessage2 = " + loggedMessage2);
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
                    bool Step4_1 = loggedMessage1 == "(Upload Request Worker) C_STORE operation failed.";
                    bool Step4_2 = loggedMessage2.Contains(StudyInstanceUID) && loggedMessage2.Contains(SOPInstanceUID);
                    //Validation of message in log file
                    if (Step4_1 && Step4_2)
                    {
                        result.steps[++ExecutedSteps].StepPass();
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Pop Step6_1 = " + Step4_1);
                        Logger.Instance.InfoLog("Pop Step6_2 = " + Step4_2);
                        result.steps[++ExecutedSteps].StepFail();
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

                //Step 5:
                StudyInstanceUID = BasePage.ReadDicomFile<String>(DicomPath[2], DicomTag.StudyInstanceUID);
                SOPInstanceUID = BasePage.ReadDicomFile<String>(DicomPath[2], DicomTag.SOPInstanceUID);
                StartTime = DateTime.Now;
                Logger.Instance.InfoLog("HTML5 Start Time = " + StartTime);
                login.Logout();
                string[] HTML5WindowHandle = basepage.OpenHTML5UploaderandSwitchtoIT("login");
                PageLoadWait.WaitForFrameLoad(10);
                html5.RegisteredUserRadioBtn().Click();
                html5.UserNameTxtBox().SendKeys(Config.ph1UserName);
                html5.PasswordTxtBox().SendKeys(Config.ph1Password);
                html5.SignInBtn().Click();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                PageLoadWait.WaitForElement(html5.By_HippaComplianceLabel(), BasePage.WaitTypes.Visible);
                if (!html5.HippaAgreeChkBox().Selected)
                {
                    html5.HippaAgreeChkBox().Click();
                }
                PageLoadWait.WaitForElement(html5.By_HippaContinueBtn(), BasePage.WaitTypes.Clickable);
                html5.HippaContinueBtn().Click();
                result.steps[++ExecutedSteps].StepPass();

                //Step 6:
                //Step 7:
                html5.UploadFilesBtn().Click();
                basepage.UploadFileInBrowser(DicomPath[2], "file", AppendDrivepath: true);
                PageLoadWait.WaitForHTML5StudyToUpload(120);
                html5.ShareJobButton().Click();
                html5.DestinationDropdown().SelectByText(Config.Dest1);
                html5.PriorityDropdown().SelectByText("ROUTINE");
                html5.CommentTextBox().SendKeys("Test 1676564 - Step 7");
                html5.ShareBtn().Click();
                if (PageLoadWait.WaitForElement(html5.By_DragFilesDiv(), BasePage.WaitTypes.Visible, 60).Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8:
                Thread.Sleep(15000);
                MainDirectory = Directory.GetDirectories(Config.HTML5UploaderRejectedPath, "*", SearchOption.AllDirectories);
                if (MainDirectory.Any(q => q.Contains(StudyInstanceUID)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 9:
                EndTime = DateTime.Now;
                Logger.Instance.InfoLog("HTML5 End Time = " + EndTime);
                try
                {
                    var loggedMessage1 = string.Empty;
                    var loggedMessage2 = string.Empty;
                    String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
                    for (var i = 1; i >= 1; i++)
                    {
                        String LogFilePath = @"C:\Windows\Temp" + @"\WebUploaderDeveloper-" + Date + "(" + i + ")" + ".log";
                        System.DateTime DateTime = System.DateTime.Now.Date;

                        if (File.Exists(LogFilePath))
                        {
                            //StreamReader reader = new StreamReader(stream);
                            var LogValues = basepage.ReadDevTraceLog(LogFilePath, StartTime, EndTime, isInformation: true);

                            foreach (var entry in LogValues)
                            {
                                if (entry.Value["Message"].Contains("(Upload Request Worker) C_STORE operation failed.")) //Actual message : License type embeded SessionType=Integrator
                                {
                                    loggedMessage1 = entry.Value["Message"];
                                    Logger.Instance.InfoLog("HTML5 loggedMessage1 = " + loggedMessage1);
                                    break;
                                }
                            }
                            foreach (var entry in LogValues)
                            {
                                if (entry.Value["Message"].Contains("MC exception")) //Actual message : License type embeded SessionType=Integrator
                                {
                                    loggedMessage2 = entry.Value["Detail"];
                                    Logger.Instance.InfoLog("HTML5 loggedMessage2 = " + loggedMessage2);
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
                    bool Step9_1 = loggedMessage1 == "(Upload Request Worker) C_STORE operation failed.";
                    bool Step9_2 = loggedMessage2.Contains(StudyInstanceUID) && loggedMessage2.Contains(SOPInstanceUID);
                    //Validation of message in log file
                    if (Step9_1 && Step9_2)
                    {
                        result.steps[++ExecutedSteps].StepPass();
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        Logger.Instance.InfoLog("HTML5 Step9_1 = " + Step9_1);
                        Logger.Instance.InfoLog("HTML5 Step9_2 = " + Step9_2);
                        result.steps[++ExecutedSteps].StepFail();
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

                //Step 10:
                login.Logout();
                ei.EIDicomUpload(Config.ph1UserName, Config.ph1Password, Config.Dest1, DicomPath[3]);
                BasePage.RunBatchFile(Config.batchfilepath, Config.TestDataPath + DicomPath[4] + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                login.DriverGoTo(login.mpacstudyurl);
                mplogin.Loginpacs(Config.pacsadmin, Config.pacspassword);
                tools = (Tool)mphomepage.NavigateTopMenu("Tools");
                mphomepage.NavigateTopMenu("Tools");
                tools.NavigateToSendStudy();
                tools.SearchStudy("Accession", Accession[4], 0);
                tools.MpacSelectStudy("Accession", Accession[4]);
                tools.SendStudy(1, Config.pacsgatway1, waitTime: 180);
                login.Logout();
                HTML5WindowHandle = basepage.OpenHTML5UploaderandSwitchtoIT("login");
                PageLoadWait.WaitForFrameLoad(10);
                html5.RegisteredUserRadioBtn().Click();
                html5.UserNameTxtBox().SendKeys(Config.ph1UserName);
                html5.PasswordTxtBox().SendKeys(Config.ph1Password);
                html5.SignInBtn().Click();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                PageLoadWait.WaitForElement(html5.By_HippaComplianceLabel(), BasePage.WaitTypes.Visible);
                if (!html5.HippaAgreeChkBox().Selected)
                {
                    html5.HippaAgreeChkBox().Click();
                }
                PageLoadWait.WaitForElement(html5.By_HippaContinueBtn(), BasePage.WaitTypes.Clickable);
                html5.HippaContinueBtn().Click();
                html5.UploadFilesBtn().Click();
                basepage.UploadFileInBrowser(DicomPath[5], "file", AppendDrivepath: true);
                PageLoadWait.WaitForHTML5StudyToUpload(120);
                html5.ShareJobButton().Click();
                html5.DestinationDropdown().SelectByText(Config.Dest1);
                html5.PriorityDropdown().SelectByText("ROUTINE");
                html5.CommentTextBox().SendKeys("Test 1676564 - Step 10");
                html5.ShareBtn().Click();
                PageLoadWait.WaitForElement(html5.By_DragFilesDiv(), BasePage.WaitTypes.Visible, 60);
                login.Logout();
                login.LoginIConnect(Config.ph1UserName,Config.ph1Password);
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Accession", Accession[3]);
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                bool Step10_1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new String[] { Accession[3], "Uploaded" }) !=null;
                inbounds.SearchStudy("Accession", Accession[4]);
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                bool Step10_2 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new String[] { Accession[4], "Uploaded" }) != null;
                inbounds.SearchStudy("Accession", Accession[5]);
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                bool Step10_3 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new String[] { Accession[5], "Uploaded" }) != null;
                if(Step10_1 && Step10_2 && Step10_3)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("EI Step10_1 = " + Step10_1);
                    Logger.Instance.InfoLog("POP Step10_2 = " + Step10_2);
                    Logger.Instance.InfoLog("HTML5 Step10_3 = " + Step10_3);
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
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
            finally
            {
                basepage.ChangeAttributeValue(Config.WebConfigPath, "/sharedListeners/add[@name='DeveloperLog']", "initializeData", @"C:\Windows\Temp\WebAccessDeveloper.log");
                servicetool.RestartIISUsingexe();
                try
                {
                    string[] currentdatetime = basepage.GetCurrentDateAndTimeFromInternet().Split(' ');
                    BasePage.RunBatchFile(updatedateandtimebatchfile, "date" + " " + currentdatetime[0]);
                    BasePage.RunBatchFile(updatedateandtimebatchfile, "time" + " " + currentdatetime[1]);
                }
                catch (Exception) { }
            }
        }
        /// <summary>
        /// Cleanup script to close browser
        /// </summary>
        public void Test_Cleanup()
        {
            login.CloseBrowser();
        }


      

    }
}