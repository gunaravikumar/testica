using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.eHR;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Pages.iConnect;
using TestStack.White.UIItems;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.Finders;
using System.Threading;
using System.Data;
using System.Xml;
using OpenQA.Selenium.Interactions;
using Selenium.Scripts.Pages.MPAC;
using ComboBox = TestStack.White.UIItems.ListBoxItems.ComboBox;
using Window = TestStack.White.UIItems.WindowItems.Window;
using Selenium.Scripts.Pages.HoldingPen;
using System.IO;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Net.Mail;
using HtmlAgilityPack;
using Microsoft.XmlDiffPatch;
using OpenQA.Selenium;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Selenium.Scripts.Tests
{
    class HUS : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public EHR ehr { get; set; }
        public WpfObjects wpfobject { get; set; }
        public ServiceTool servicetool { get; set; }
        public UserPreferences userpref { get; set; }

        Studies studies { get; set; }
        public BasePage basepage { get; set; }
        public BluRingViewer bluringviewer { get; set; }
        public StudyViewer studyviewer { get; set; }
        
        public static string WebAccessConfigXML = @"C:\WebAccess\WebAccess\Config\WebAccessConfiguration.xml";
        string defaultSAMLAssesrtion = Directory.GetCurrentDirectory() + "\\TestEHRfiles\\samlAssertion.xml";
        String adminUserName = Config.adminUserName;
        String adminPassword = Config.adminPassword;
        String EntryUUID = null;
        String RepositoryId = null;
        String SAMLName = null;
        string DocumentId = null;
        String SAMLpath = null;
        string tsharkListernerOutput1 = @"C:\Program Files\Wireshark\captures.txt";
        string NewSAMLAssertionName = "NewSAMLAssertionName";
        string username = "administrator";
        string TestDomain = "SuperAdminGroup";
        string DomainAdmin = "administrator";
        string Role = null;
        string URL = "http://" + Config.IConnectIP + "/webaccess";



        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        public HUS(String classname)
        {
            login = new Login();
            BasePage.InitializeControlIdMap();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            wpfobject = new WpfObjects();
            servicetool = new ServiceTool();
            ehr = new EHR();
            basepage = new BasePage();
            userpref = new UserPreferences();
            bluringviewer = new BluRingViewer();
            studies = new Studies();
        }
        

        /// <summary>
        ///  Name of Evidence Token for the form data Interfaces of XDS Document Display is configurable
        /// </summary>
        public TestCaseResult Test_167954(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
               GettestDataForHUS(filepath, testid);
                IList<string> EntryUUIDList = EntryUUID.Split(',');
                IList<string> DocumentIdList = DocumentId.Split(',');
                IList<string> RepositoryIdList = RepositoryId.Split(',');
                
                SAMLpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "SALMpath");

                //Pre-Condition
                //Create a Domain User 
                //createDomainForHUS(testid);

                //Step 1
                string WebAccessConfigXML = HUS.WebAccessConfigXML;
                basepage.ChangeAttributeValue(WebAccessConfigXML, "/IntegratedMode/parameters/parameter[@key='SAMLAssertion']", "value", NewSAMLAssertionName);
                Thread.Sleep(5000);
                string AttributeValue = basepage.GetAttributeValue(WebAccessConfigXML, "/IntegratedMode/parameters/parameter[@key='SAMLAssertion']", "value");
                if (AttributeValue == NewSAMLAssertionName)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                RestartIISUsingexe();

                //Step 2
                SAMLpath = "default";
                ++ExecutedSteps;
                LaunchTestEHRandSetParamterinDocSearch();
                ehr.SearchDocumentInTestEHR(EntryUUID: EntryUUIDList[0], DocumentId: DocumentIdList[0], RepositoryId: RepositoryIdList[0], SAMLpath: SAMLpath);
                string EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                string HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);

                //Create a new Session and Launch URL
                //Naviage to the HTML file created for Post request from TestEHR.
                string[] files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                Process processid = StartWireshark(Config.XDSHTTPS_Registery, Config.XDSRepository, BatFilePath: @"\OtherFiles\XMLtshark.bat");
                login.NavigateToIntegratorURL("file:///" + files[0]);

                //matching Documnet is Displayed
                IntegratorStudies integratorstudies = null;
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                PageLoadWait.WaitForPageLoad(20);
                StudyViewer studyviewer = new StudyViewer();
                PageLoadWait.WaitForWebElement(studyviewer.SeriesViewer_1X1_ByElement, "visible");
                EndWireshark(processid, waitTime: 30);

                // Vefiy the Assesrtion in the Wireshark Log File.
                string WiresharkOutput = System.IO.File.ReadAllText(tsharkListernerOutput1);
                IList<string> SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);
                SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);
                IList<string> ITI43SAMl = GetITI43FromWireSharkOutput(WiresharkOutput);
                if (ITI43SAMl.Count == 0)
                    result.steps[ExecutedSteps].AddPassStatusList("Unable to find the Iti43 request call.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("Able to find the Iti43 request call.");

                if (SAMLAssesrtionFromWireSharkOutput.Count != 0)
                    result.steps[ExecutedSteps].AddFailStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                else
                    result.steps[ExecutedSteps].AddPassStatusList("No SAML Assertion can be found in header of iti-18 and iti-43 requests.");

                //If there is Token check the Token with SAMl Assesrtion
                if (SAMLAssesrtionFromWireSharkOutput.Contains("saml:Assertion"))
                    result.steps[ExecutedSteps].AddFailStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //step 3 - Verify the Evidence Token in the launched request.
                if ((VerfiyThePostXMLAttribute(HTMLFilepath, "SAMLAssertion", EncryptedSAML)) || (VerfiyThePostXMLAttribute(HTMLFilepath, "entryUUID", EntryUUIDList[0])) && (VerfiyThePostXMLAttribute(HTMLFilepath, "repositoryId", RepositoryId)) && (VerfiyThePostXMLAttribute(HTMLFilepath, "documentID", DocumentIdList[0])) && (VerfiyThePostXMLAttribute(HTMLFilepath, "SAMLAssertion", EncryptedSAML)))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                servicetool.LaunchServiceTool();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                servicetool.RestartIISUsingexe();
                launchNewStudyForITI43Reset();

                //Step 4
                //Search and Launch Documnet from TESTEHR.
                LaunchTestEHRandSetParamterinDocSearch();
                ehr.SearchDocumentInTestEHR(EntryUUID: EntryUUIDList[1], DocumentId: DocumentIdList[1], RepositoryId: RepositoryIdList[1], SAMLpath: SAMLpath, SAMLName: NewSAMLAssertionName);
                EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);
              
                //Naviage to the HTML file created for Post request from TestEHR.
                files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                processid = StartWireshark(Config.XDSHTTPS_Registery, Config.XDSRepository, BatFilePath: @"\OtherFiles\XMLtshark.bat"); 
                login.NavigateToIntegratorURL("file:///" + files[0]);

                //matching Documnet is Displayed
                integratorstudies = null;
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                studyviewer = new StudyViewer();
                try
                {
                    PageLoadWait.WaitForWebElement(studyviewer.SeriesViewer_1X1_ByElement, "visible");
                    if (studyviewer.SeriesViewer_1X1().Displayed)
                    {
                        result.steps[++ExecutedSteps].SetPath(testid + "_1_", ExecutedSteps);
                        if (studies.CompareImage(result.steps[ExecutedSteps], studyviewer.SeriesViewer_1X1()))
                            result.steps[ExecutedSteps].AddPassStatusList("Image Comapre passed for the step 3");
                        else
                            result.steps[ExecutedSteps].AddFailStatusList();
                    }
                    else
                        result.steps[ExecutedSteps].AddFailStatusList("Fail : Study is not loaded");
                }catch(Exception ex)
                {
                    result.steps[ExecutedSteps].AddFailStatusList("Fail : Study is not loaded");
                }

                EndWireshark(processid, waitTime: 30);
                
                // Vefiy the Assesrtion in the Wireshark Log File.
                WiresharkOutput = System.IO.File.ReadAllText(tsharkListernerOutput1);
                SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);
                 ITI43SAMl = GetITI43FromWireSharkOutput(WiresharkOutput);
                if(ITI43SAMl.Count == 0)
                    result.steps[ExecutedSteps].AddFailStatusList("Unable to find the Iti43 request call.");
                else
                {
                    //yet to write.
                    if(!CompareSAMLAssertionXML(ITI43SAMl))
                    {
                        result.steps[ExecutedSteps].AddFailStatusList("No SAML Assertion can be found in Iti-43 requests or SAMl not match With the SAML used at TestEHR");
                    }

                }

                if (SAMLAssesrtionFromWireSharkOutput.Count == 0)
                    result.steps[ExecutedSteps].AddFailStatusList("No SAML Assertion can be found in header of iti-18.");
                else
                {
                    result.steps[ExecutedSteps].AddPassStatusList("SAML Assertion can be found in header of iti-18 requests.");
                    if(CompareSAMLAssertionXML(SAMLAssesrtionFromWireSharkOutput, defaultSAMLAssesrtion))
                    {
                        result.steps[ExecutedSteps].AddPassStatusList("SAML Assertion in header of iti-18  requests. match with SAML used in TESTEHR");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList("SAML Assertion in header of iti-18 requests not match with SAML used in TESTEHR");
                    }
                }
               
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Step 5
                //Verfiy the values at the HTML file at Temp folder.
                if ((VerfiyThePostXMLAttribute(HTMLFilepath, NewSAMLAssertionName, EncryptedSAML)) || (VerfiyThePostXMLAttribute(HTMLFilepath, "entryUUID", EntryUUIDList[1])) || (VerfiyThePostXMLAttribute(HTMLFilepath, "documentID", DocumentIdList[1])))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Close browser
                ehr.KillProcessByName("chrome");
                ehr.KillProcessByName("iexplore");
                ehr.KillProcessByName("firefox");
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login.CreateNewSesion();
                
                //Return Result
                return result;
            }
            finally
            {
                ehr.CloseEHR();
                basepage.ChangeAttributeValue(HUS.WebAccessConfigXML, "/IntegratedMode/parameters/parameter[@key='SAMLAssertion']", "value", "SAMLAssertion");
                string AttributeValue = basepage.GetAttributeValue(HUS.WebAccessConfigXML, "/IntegratedMode/parameters/parameter[@key='SAMLAssertion']", "value");
                if (AttributeValue != "SAMLAssertion")
                {
                    throw new Exception("Error while update the Default SALM name in webAccessconfigXML");
                }
                RestartIISUsingexe();

                //Close browser
                ehr.KillProcessByName("chrome");
                ehr.KillProcessByName("iexplore");
                ehr.KillProcessByName("firefox");
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login.CreateNewSesion();
            }
        }

        /// <summary>
        ///  Using the same Evidence Token for the current user session (non-XCA)
        /// </summary>
        public TestCaseResult Test_167998(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                GettestDataForHUS(filepath, testid);
                IList<string> EntryUUIDList = EntryUUID.Split(',');
                IList<string> DocumentIdList = DocumentId.Split(',');
                IList<string> RepositoryIdList = RepositoryId.Split(',');

                BasePage.MultiDriver = new List<IWebDriver>();
                BasePage.MultiDriver.Add(BasePage.Driver);

                //Pre - Condition
                //Create a Domain User
                //createDomainForHUS(testid);
                servicetool.LaunchServiceTool();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                servicetool.RestartIISUsingexe();
                Thread.Sleep(5000);
                servicetool.RestartIISUsingexe();

                //Step 1
                SAMLpath = "default";
                ++ExecutedSteps;
                LaunchTestEHRandSetParamterinDocSearch();
                ehr.SearchDocumentInTestEHR(EntryUUID: EntryUUIDList[0], DocumentId: DocumentIdList[0], RepositoryId: RepositoryIdList[0]);
                string HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);

                if ((VerfiyThePostXMLAttribute(HTMLFilepath, "entryUUID", EntryUUIDList[0])))
                    result.steps[ExecutedSteps].AddPassStatusList();
                else
                    result.steps[ExecutedSteps].AddFailStatusList();

                //Create a new Session and Launch URL
                //Naviage to the HTML file created for Post request from TestEHR.
                string[] files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                Process processid = StartWireshark(Config.XDSHTTPS_Registery, Config.XDSRepository, BatFilePath: @"\OtherFiles\XMLtshark.bat");
                login.NavigateToIntegratorURL("file:///" + files[0]);

                //matching Documnet is Displayed
                IntegratorStudies integratorstudies = null;
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                PageLoadWait.WaitForPageLoad(20);
                StudyViewer studyviewer = new StudyViewer();
                try
                {
                    PageLoadWait.WaitForWebElement(studyviewer.SeriesViewer_1X1_ByElement, "visible");
                    if (studyviewer.SeriesViewer_1X1().Displayed)
                    {
                        result.steps[ExecutedSteps].SetPath(testid + "_1_", ExecutedSteps);
                        if (!studies.CompareImage(result.steps[ExecutedSteps], studyviewer.SeriesViewer_1X1()))
                            result.steps[ExecutedSteps].AddFailStatusList();
                    }
                    else
                        result.steps[ExecutedSteps].AddFailStatusList();
                }
                catch (Exception ex)
                {
                    result.steps[ExecutedSteps].AddFailStatusList(ex.Message);
                }
                EndWireshark(processid, waitTime: 30);


                // Vefiy the Assesrtion in the Wireshark Log File.
                string WiresharkOutput = System.IO.File.ReadAllText(tsharkListernerOutput1);
                IList<string> SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);

                if (SAMLAssesrtionFromWireSharkOutput.Count != 0)
                    result.steps[ExecutedSteps].AddFailStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                else
                    result.steps[ExecutedSteps].AddPassStatusList("No SAML Assertion can be found in header of iti-18 and iti-43 requests.");

                Logger.Instance.InfoLog("HTML file is created in the Temp Folder for the Launched URL");
              

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Step 2
                //Search and Launch Documnet from TESTEHR.
                servicetool.LaunchServiceTool();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                servicetool.RestartIISUsingexe();
                Thread.Sleep(5000);
                servicetool.RestartIISUsingexe();
                launchNewStudyForITI43Reset();

                LaunchTestEHRandSetParamterinDocSearch();
                ehr.SearchDocumentInTestEHR(EntryUUID: EntryUUIDList[1], DocumentId: DocumentIdList[1], RepositoryId: RepositoryIdList[1], SAMLpath: SAMLpath);
                string EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);

                //Verfiy the values at the HTML file at Temp folder.
                if ((VerfiyThePostXMLAttribute(HTMLFilepath, "entryUUID", EntryUUIDList[1])) && (VerfiyThePostXMLAttribute(HTMLFilepath, "SAMLAssertion", EncryptedSAML)))
                    result.steps[++ExecutedSteps].AddPassStatusList();
                else
                    result.steps[++ExecutedSteps].AddFailStatusList();

                //Naviage to the HTML file created for Post request from TestEHR.
                files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                processid = StartWireshark(Config.XDSHTTPS_Registery, Config.XDSRepository, BatFilePath: @"\OtherFiles\XMLtshark.bat");
                login.NavigateToIntegratorURL("file:///" + files[0]);

                //matching Documnet is Displayed
                integratorstudies = null;
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                studyviewer = new StudyViewer();
                try
                {
                    PageLoadWait.WaitForWebElement( studyviewer.DocumentViewportContainer , "visible");
                    if (BasePage.Driver.FindElement(studyviewer.DocumentViewportContainer).Displayed)
                    {
                        result.steps[ExecutedSteps].SetPath(testid + "_1_", ExecutedSteps);
                        if (studies.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElement(studyviewer.DocumentViewportContainer)))
                            result.steps[ExecutedSteps].AddPassStatusList("Image Comapre passed for the step 3");
                        else
                            result.steps[ExecutedSteps].AddFailStatusList();
                    }
                    else
                        result.steps[ExecutedSteps].AddFailStatusList("Fail : Study is not loaded");
                }
                catch (Exception ex)
                {
                    result.steps[ExecutedSteps].AddFailStatusList("Fail : Study is not loaded");
                }

                EndWireshark(processid, waitTime: 30);

                // Vefiy the Assesrtion in the Wireshark Log File.
                WiresharkOutput = System.IO.File.ReadAllText(tsharkListernerOutput1);

                SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);
                IList<string> Iti43RequestCallSAML = GetITI43FromWireSharkOutput(WiresharkOutput);
                if (SAMLAssesrtionFromWireSharkOutput.Count == 0)
                    result.steps[ExecutedSteps].AddFailStatusList("No SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                else
                {
                    result.steps[ExecutedSteps].AddPassStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                    if(CompareSAMLAssertionXML(SAMLAssesrtionFromWireSharkOutput,defaultSAMLAssesrtion))
                    {
                        result.steps[ExecutedSteps].AddPassStatusList("SAML in request match with SAML in TestEHR");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList("SAML in request not match with SAML in TestEHR");
                    }
                }

                SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);
                IList<string> ITI43SAMl = GetITI43FromWireSharkOutput(WiresharkOutput);
                if (ITI43SAMl.Count == 0)
                    result.steps[ExecutedSteps].AddFailStatusList("Unable to find the Iti43 request call.");
                else
                {
                    //yet to write.
                    if (!CompareSAMLAssertionXML(ITI43SAMl))
                    {
                        result.steps[ExecutedSteps].AddFailStatusList("No SAML Assertion can be found in Iti-43 requests or SAMl not match With the SAML used at TestEHR");
                    }

                }

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Step3
                //Search and Launch Documnet from TESTEHR.
                servicetool.LaunchServiceTool();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                RestartIISUsingexe();
                Thread.Sleep(5000);
                RestartIISUsingexe();
                launchNewStudyForITI43Reset();
                //Wait one Minute for the IIS restart.
                Thread.Sleep(100000);

                LaunchTestEHRandSetParamterinDocSearch();
                HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false, DeletetempHTML: false);
                ehr.SearchDocumentInTestEHR(EntryUUID: EntryUUIDList[2], DocumentId: DocumentIdList[2], RepositoryId: RepositoryIdList[2], SAMLpath: Directory.GetCurrentDirectory() + "\\TestEHRfiles\\samlAssertion2.xml");
                EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                Thread.Sleep(4000);
                HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false, DeletetempHTML: false);

                //Verfiy the values at the HTML file at Temp folder.
                if ((VerfiyThePostXMLAttribute(HTMLFilepath, "entryUUID", EntryUUIDList[2])) && (VerfiyThePostXMLAttribute(HTMLFilepath, "SAMLAssertion", EncryptedSAML)))
                    result.steps[++ExecutedSteps].AddPassStatusList();
                else
                    result.steps[++ExecutedSteps].AddFailStatusList();

                //Naviage to the HTML file created for Post request from TestEHR.
                List<String> browserList = new List<String> { "firefox", "ie" };
                for (int count = 0; count < 2; count++)
                {
                    if (Config.BrowserType.ToLower() == browserList[count])
                    {
                        browserList[count] = "chrome";
                        break;
                    }
                }

                BasePage.MultiDriver.Add(login.InvokeBrowser(browserList[0]));
                Driver = BasePage.MultiDriver.Last();
                files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                processid = StartWireshark(Config.XDSHTTPS_Registery, Config.XDSRepository, BatFilePath: @"\OtherFiles\XMLtshark.bat");
                login.NavigateToIntegratorURL("file:///" + files[0]);

                //matching Documnet is Displayed
                integratorstudies = null;
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                studyviewer = new StudyViewer();
                try
                {
                    PageLoadWait.WaitForWebElement(studyviewer.SeriesViewer_1X1_ByElement, "visible");
                    if (studyviewer.SeriesViewer_1X1().Displayed)
                    {
                        result.steps[ExecutedSteps].SetPath(testid + "_1_", ExecutedSteps);
                        if (studies.CompareImage(result.steps[ExecutedSteps], studyviewer.SeriesViewer_1X1()))
                            result.steps[ExecutedSteps].AddPassStatusList("Image Comapre passed for the step 3");
                        else
                            result.steps[ExecutedSteps].AddFailStatusList();
                    }
                    else
                        result.steps[ExecutedSteps].AddFailStatusList("Fail : Study is not loaded");
                }
                catch (Exception ex)
                {
                    result.steps[ExecutedSteps].AddFailStatusList("Fail : Study is not loaded");
                }

                EndWireshark(processid, waitTime: 30);

                // Vefiy the Assesrtion in the Wireshark Log File.
                WiresharkOutput = System.IO.File.ReadAllText(tsharkListernerOutput1);
                SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);

                if (SAMLAssesrtionFromWireSharkOutput.Count == 0)
                    result.steps[ExecutedSteps].AddFailStatusList("No SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                else
                {
                    result.steps[ExecutedSteps].AddPassStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                    if(CompareSAMLAssertionXML(SAMLAssesrtionFromWireSharkOutput, Directory.GetCurrentDirectory() + "\\TestEHRfiles\\samlAssertion2.xml"))
                    {
                        result.steps[ExecutedSteps].AddPassStatusList();
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList("SAML in the ITI-18 Not match with the SAML Passed in TestEHR");
                    }
                }

                SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);
                Iti43RequestCallSAML = GetITI43FromWireSharkOutput(WiresharkOutput);
                if (Iti43RequestCallSAML.Count == 0)
                    result.steps[ExecutedSteps].AddFailStatusList("No SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                else
                {
                    result.steps[ExecutedSteps].AddPassStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                    if (CompareSAMLAssertionXML(Iti43RequestCallSAML, Directory.GetCurrentDirectory() + "\\TestEHRfiles\\samlAssertion2.xml"))
                    {
                        result.steps[ExecutedSteps].AddPassStatusList("SAML in iti-43 request match with SAML in TestEHR");
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList("SAML in iti-43 request not match with SAML in TestEHR");
                    }
                }

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                BasePage.MultiDriver.Last().Close();
                Thread.Sleep(5000);
                BasePage.MultiDriver.Remove(MultiDriver.Last());
                Thread.Sleep(5000);
                Driver = BasePage.MultiDriver.Last();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                ehr.CloseEHR();
                RestartIISUsingexe();

                //Close browser
                //basepage.CloseBrowser();
                ehr.KillProcessByName("chrome");
                ehr.KillProcessByName("iexplore");
                ehr.KillProcessByName("firefox");
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login.CreateNewSesion();
            }
        }

        /// <summary>
        ///  Support URL encryption on the XDS Document Display request (non-XCA)
        /// </summary>
        public TestCaseResult Test_167997(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Encryption Steup - Precondition
                string Encryption_TripleDes_Key = "TripleDES";
                string Encryption_Passpharse_TripleDes_Key = "mergehealthcare";
                string Encryption_Passphares_TripleDes_A_key = "cedaracare";
                Stopwatch stopwatch = new Stopwatch();
                TimeSpan timeout = new TimeSpan(0, 3, 0);
                string encrytionProvider1 = "ID-123";

                //Preconditon 
                //Update the Config values
                basepage.SetWebConfigValue(Config.webconfig, "Application.Integrator.TimeSpan", "2");
                RestartIISUsingexe();

                //Setup the Encrytion Values
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                wpfobject.WaitTillLoad();
                servicetool.wpfobject.ClickButton("Modify", 1);
                wpfobject.WaitTillLoad();
                servicetool.EnablePatient();
                // servicetool.EnableMergeEMPI();
                wpfobject.WaitTillLoad();

                servicetool.NavigateToEncryption();
                servicetool.WaitWhileBusy();
                servicetool.IntegratorUrlTab().Click();
                servicetool.WaitWhileBusy();
                servicetool.ClickModifyFromTab();
                servicetool.WaitWhileBusy();
                if (servicetool.URLEnc_CB().Checked != true)
                {
                    servicetool.URLEnc_CB().Click();
                }
                servicetool.WaitWhileBusy();
                servicetool.NavigateSubTab("Integrator Url");
                ListView integratotURL = wpfobject.GetAnyUIItem<ITabPage, ListView>(servicetool.GetCurrentTabItem(), "ListView", 0);
                if (integratotURL.Rows.Count == 0)
                {
                    servicetool.NavigateToEncryption();
                    servicetool.SetEncryptionEncryptionService();
                    GroupBox group = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), "Encryption Service List", 1);
                    ListView datagrid = wpfobject.GetAnyUIItem<GroupBox, ListView>(group, "ListView");
                    servicetool.NavigateToTab(ServiceTool.Encryption_Tab);
                    wpfobject.WaitTillLoad();
                    servicetool.NavigateSubTab("Key Generator");
                    String[] TripleDESGeneratedKey = servicetool.GenerateEncryptionKeys(Encryption_Passpharse_TripleDes_Key, keysize: "192 bit (Key for TripleDES, AES)");
                    String[] TripleDESAGeneratedKey = servicetool.GenerateEncryptionKeys(Encryption_Passphares_TripleDes_A_key, keysize: "192 bit (Key for TripleDES, AES)");

                    // Create Encrption Service for "Triple DES"
                    servicetool.SetEncryptionEncryptionService();
                    servicetool.WaitWhileBusy();
                    servicetool.EnterServiceEntry(Key: Encryption_TripleDes_Key, Assembly: "OpenContent.Generic.Core.dll", Class: "OpenContent.Core.Security.Services.TripleDES");
                    wpfobject.GetButton("Apply", 1).Click();
                    servicetool.EnterServiceParameters("key", "string", TripleDESGeneratedKey[0]);
                    servicetool.EnterServiceParameters("iv", "string", "");
                    servicetool.EnterServiceParameters("characterset", "string", "Windows-1252");
                    servicetool.EnterServiceParameters("operationMode", "string", "CBC");
                    servicetool.EnterServiceParameters("paddingMode", "string", "Zeros");
                    wpfobject.GetMainWindowByTitle("Service Entry Form");
                    wpfobject.GetButton("OK", 1).Click();

                    wpfobject.WaitTillLoad();
                    wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                    wpfobject.WaitTillLoad();
                    servicetool.NavigateSubTab("Integrator Url");
                    wpfobject.WaitTillLoad();

                    //servicetool.ClickModifyFromTab();
                    wpfobject.WaitTillLoad();
                    wpfobject.SelectCheckBox("URL Encryption Enabled", 1);
                    wpfobject.WaitTillLoad();
                    TextBox ID = wpfobject.GetUIItem<ITabPage, TextBox>(servicetool.GetCurrentTabItem(), 1);
                    ID.BulkText = encrytionProvider1;
                    wpfobject.WaitTillLoad();
                    TextBox ArugumentName = wpfobject.GetUIItem<ITabPage, TextBox>(servicetool.GetCurrentTabItem(), 0);
                    ArugumentName.BulkText = "args";
                    wpfobject.WaitTillLoad();
                    wpfobject.SetText("PART_EditableTextBox", "Cryptographic." + Encryption_TripleDes_Key);
                    wpfobject.ClickButton("Add", 1);
                    wpfobject.WaitTillLoad();

                    ComboBox DefaultEncryptionProvider = wpfobject.GetUIItem<ITabPage, ComboBox>(servicetool.GetCurrentTabItem(), 0);
                    DefaultEncryptionProvider.Enter("ID-123");
                    wpfobject.WaitTillLoad();
                    servicetool.ClickApplyButtonFromTab();
                    wpfobject.WaitTillLoad();
                    try
                    {
                        wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                    }
                    catch (Exception e)
                    { }
                    servicetool.RestartService();
                    servicetool.CloseServiceTool();
                }
                else
                {
                    servicetool.ClickApplyButtonFromTab();
                    wpfobject.WaitTillLoad();
                    try
                    {
                        wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                    }
                    catch (Exception e)
                    { }
                    servicetool.RestartService();
                    servicetool.CloseServiceTool();

                }

                GettestDataForHUS(filepath, testid);
                IList<string> EntryUUIDList = EntryUUID.Split(';');

                BasePage.MultiDriver = new List<IWebDriver>();
                BasePage.MultiDriver.Add(BasePage.Driver);

                //Pre-Condition
                //Create a Domain User 
                createDomainForHUS(testid);

                //Step 1
                LaunchTestEHRandSetParamterinDocSearch();
                wpfobject.SelectCheckBox("encryptEnabledCheckBox");
                ComboBox EhrEncryptionProvider = wpfobject.GetAnyUIItem<ITabPage, ComboBox>(ehr.GetCurrentTabItem(), "encryptionProviderCmb", 0);
                EhrEncryptionProvider.Select(0);
                ehr.SearchDocumentInTestEHR(EntryUUID: EntryUUIDList[0]);
                string HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);
                try
                {
                    if ((VerfiyThePostXMLAttribute(HTMLFilepath, "entryUUID", EntryUUIDList[0])) )
                         result.steps[++ExecutedSteps].AddFailStatusList();
                }
                catch(Exception ex)
                {
                    result.steps[++ExecutedSteps].AddPassStatusList();
                }

                //Create a new Session and Launch URL
                //Naviage to the HTML file created for Post request from TestEHR.
                string[] files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                Process processid = StartWireshark(Config.XDSHTTPS_Registery, Config.XDSRepository, BatFilePath: @"\OtherFiles\XMLtshark.bat");
                login.NavigateToIntegratorURL("file:///" + files[0]);

                //matching Documnet is Displayed
                IntegratorStudies integratorstudies = null;
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                PageLoadWait.WaitForPageLoad(20);
                StudyViewer studyviewer = new StudyViewer();
                try
                {
                    PageLoadWait.WaitForWebElement(studyviewer.SeriesViewer_1X1_ByElement, "visible");
                    if (studyviewer.SeriesViewer_1X1().Displayed)
                    {
                        result.steps[ExecutedSteps].SetPath(testid + "_1_", ExecutedSteps);
                        if (!studies.CompareImage(result.steps[ExecutedSteps], studyviewer.SeriesViewer_1X1()))
                            result.steps[ExecutedSteps].AddFailStatusList();
                    }
                    else
                        result.steps[ExecutedSteps].AddFailStatusList();
                }
                catch (Exception ex)
                {
                    result.steps[ExecutedSteps].AddFailStatusList(ex.Message);
                }
                EndWireshark(processid, waitTime: 30);


                // Vefiy the Assesrtion in the Wireshark Log File.
                string WiresharkOutput = System.IO.File.ReadAllText(tsharkListernerOutput1);
                IList<string> SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);

                if (SAMLAssesrtionFromWireSharkOutput.Count != 0)
                    result.steps[ExecutedSteps].AddFailStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                else
                    result.steps[ExecutedSteps].AddPassStatusList("No SAML Assertion can be found in header of iti-18 and iti-43 requests.");

                if (SAMLAssesrtionFromWireSharkOutput.Contains("saml:Assertion"))
                    result.steps[ExecutedSteps].AddFailStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");

                Logger.Instance.InfoLog("HTML file is created in the Temp Folder for the Launched URL");
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Step 2
                //Search and Launch Documnet from TESTEHR.
                LaunchTestEHRandSetParamterinDocSearch();
                ehr.SetCommonParameters(address: URL, user: adminUserName, domain: "SuperAdminGroup", usersharing: "True", SecurityID: "administrator-administrator", autoendsession: "True", usepostmethod: "check",ExamList: null);
                //ehr.SetCommonParameters(address: URL, user: username, domain: TestDomain, usersharing: "True", SecurityID: username + "-" + username, autoendsession: "True", usepostmethod: "check");
                wpfobject.SelectCheckBox("encryptEnabledCheckBox");
                EhrEncryptionProvider = wpfobject.GetAnyUIItem<ITabPage, ComboBox>(ehr.GetCurrentTabItem(), "encryptionProviderCmb", 0);
                EhrEncryptionProvider.Select(0);
                ehr.SearchDocumentInTestEHR(EntryUUID: EntryUUIDList[0], SAMLpath: "default");
                string EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);
                

                //Verfiy the values at the HTML file at Temp folder.
                try
                {
                    if ((VerfiyThePostXMLAttribute(HTMLFilepath, "entryUUID", EntryUUIDList[0])))
                        result.steps[++ExecutedSteps].AddFailStatusList();
                }
                catch (Exception ex)
                {
                    result.steps[++ExecutedSteps].AddPassStatusList();
                }
                try
                {
                    if ((VerfiyThePostXMLAttribute(HTMLFilepath, "WrongSAMLAssertion", EncryptedSAML)))
                        result.steps[ExecutedSteps].AddFailStatusList();
                }
                catch (Exception ex)
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }

                //Naviage to the HTML file created for Post request from TestEHR.
                files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                processid = StartWireshark(Config.XDSHTTPS_Registery, Config.XDSRepository, BatFilePath: @"\OtherFiles\XMLtshark.bat");
                login.NavigateToIntegratorURL("file:///" + files[0]);

                //matching Documnet is Displayed
                integratorstudies = null;
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                studyviewer = new StudyViewer();
                try
                {
                    PageLoadWait.WaitForWebElement(studyviewer.SeriesViewer_1X1_ByElement, "visible");
                    if (studyviewer.SeriesViewer_1X1().Displayed)
                    {
                        result.steps[ExecutedSteps].SetPath(testid + "_1_", ExecutedSteps);
                        if (studies.CompareImage(result.steps[ExecutedSteps], studyviewer.SeriesViewer_1X1()))
                            result.steps[ExecutedSteps].AddPassStatusList("Image Comapre passed for the step 3");
                        else
                            result.steps[ExecutedSteps].AddFailStatusList();
                    }
                    else
                        result.steps[ExecutedSteps].AddFailStatusList("Fail : Study is not loaded");
                }
                catch (Exception ex)
                {
                    result.steps[ExecutedSteps].AddFailStatusList("Fail : Study is not loaded");
                }

                EndWireshark(processid, waitTime: 30);

                // Vefiy the Assesrtion in the Wireshark Log File.
                WiresharkOutput = System.IO.File.ReadAllText(tsharkListernerOutput1);
                SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);
                
                if (SAMLAssesrtionFromWireSharkOutput.Count == 0)
                    result.steps[ExecutedSteps].AddFailStatusList("No SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                else
                {
                    result.steps[ExecutedSteps].AddPassStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                    if(CompareSAMLAssertionXML(SAMLAssesrtionFromWireSharkOutput, defaultSAMLAssesrtion))
                        result.steps[ExecutedSteps].AddPassStatusList("SAML Assertion in header of iti-18 and iti-43 requests match with SAML in TestEHR");
                    else
                        result.steps[ExecutedSteps].AddPassStatusList("SAML Assertion in header of iti-18 and iti-43 requests not match with SAML in TestEHR");
                }

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Step 3
                Thread.Sleep(150000);
                basepage.CreateNewSesion();
                login.NavigateToIntegratorURL("file:///" + files[0]);
                Thread.Sleep(1000);
                basepage.SwitchTo("id", "IntegratorHomeFrame");
                IWebElement errormessage = BasePage.FindElementByCss("span[id='m_title']");
                if (errormessage.Text == "Error Occurred in operation: The Url has been over due.")
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step4
                //Search and Launch Documnet from TESTEHR.
                LaunchTestEHRandSetParamterinDocSearch();
                ehr.SetCommonParameters(address: URL, user: adminUserName, domain: "SuperAdminGroup", usersharing: "True", SecurityID: "administrator-administrator", autoendsession: "True", usepostmethod: "check", ExamList: null);
                wpfobject.SelectCheckBox("encryptEnabledCheckBox");
                EhrEncryptionProvider = wpfobject.GetAnyUIItem<ITabPage, ComboBox>(ehr.GetCurrentTabItem(), "encryptionProviderCmb", 0);
                EhrEncryptionProvider.Select(0);
                ehr.SearchDocumentInTestEHR(DocumentId: DocumentId , RepositoryId: RepositoryId,  SAMLpath: "default");
                EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false, DeletetempHTML: false);
                

                ////Verfiy the values at the HTML file at Temp folder.
                try
                {
                    if ((VerfiyThePostXMLAttribute(HTMLFilepath, "documnetID", DocumentId)) || (!VerfiyThePostXMLAttribute(HTMLFilepath, "SAMLAssertion", EncryptedSAML)))
                        result.steps[++ExecutedSteps].AddFailStatusList();
                }
                catch (Exception ex)
                {
                    result.steps[++ExecutedSteps].AddPassStatusList();
                }
                try
                {
                    if ((VerfiyThePostXMLAttribute(HTMLFilepath, "repository", RepositoryId)))
                        result.steps[ExecutedSteps].AddFailStatusList();
                }
                catch (Exception ex)
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }
                try
                {
                    if ((VerfiyThePostXMLAttribute(HTMLFilepath, "SAMLAssertion", EncryptedSAML)))
                        result.steps[ExecutedSteps].AddFailStatusList();
                }
                catch (Exception ex)
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }

                
                //Naviage to the HTML file created for Post request from TestEHR.
                files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                processid = StartWireshark(Config.XDSHTTPS_Registery, Config.XDSRepository, BatFilePath: @"\OtherFiles\XMLtshark.bat");
                login.NavigateToIntegratorURL("file:///" + files[0]);

                //matching Documnet is Displayed
                integratorstudies = null;
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                studyviewer = new StudyViewer();
                try
                {
                    PageLoadWait.WaitForWebElement(studyviewer.SeriesViewer_1X1_ByElement, "visible");
                    if (studyviewer.SeriesViewer_1X1().Displayed)
                    {
                        result.steps[ExecutedSteps].SetPath(testid + "_1_", ExecutedSteps);
                        if (studies.CompareImage(result.steps[ExecutedSteps], studyviewer.SeriesViewer_1X1()))
                            result.steps[ExecutedSteps].AddPassStatusList("Image Comapre passed for the step 3");
                        else
                            result.steps[ExecutedSteps].AddFailStatusList();
                    }
                    else
                        result.steps[ExecutedSteps].AddFailStatusList("Fail : Study is not loaded");
                }
                catch (Exception ex)
                {
                    result.steps[ExecutedSteps].AddFailStatusList("Fail : Study is not loaded");
                }

                EndWireshark(processid, waitTime: 30);

                // Vefiy the Assesrtion in the Wireshark Log File.
                WiresharkOutput = System.IO.File.ReadAllText(tsharkListernerOutput1);
                SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);
                if (SAMLAssesrtionFromWireSharkOutput.Count == 0)
                    result.steps[ExecutedSteps].AddFailStatusList("No SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                else
                {
                    result.steps[ExecutedSteps].AddPassStatusList("SAML Assertion can be found in header of iti-18 requests.");
                    if(CompareSAMLAssertionXML(SAMLAssesrtionFromWireSharkOutput, defaultSAMLAssesrtion))
                        result.steps[ExecutedSteps].AddPassStatusList("SAML Assertion in header of iti-18 requests match with the SAML in the TestEHR");
                    else
                        result.steps[ExecutedSteps].AddFailStatusList("SAML Assertion in header of iti-18 requests not match with the SAML in the TestEHR");
                }

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }


                //Step 5
                Thread.Sleep(150000);
                basepage.CreateNewSesion();
                login.NavigateToIntegratorURL("file:///" + files[0]);
                basepage.SwitchTo("id", "IntegratorHomeFrame");
                errormessage =  BasePage.FindElementByCss("span[id='m_title']");
                if(errormessage.Text == "Error Occurred in operation: The Url has been over due.")
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }



                //Step 5
                servicetool.LaunchServiceTool();
                servicetool.RestartService();
                Thread.Sleep(5000);
                servicetool.CloseServiceTool();
                servicetool.RestartIISUsingexe();
                Thread.Sleep(5000);
                servicetool.RestartIISUsingexe();
                launchNewStudyForITI43Reset();

                //Search and Launch Documnet from TESTEHR.
                LaunchTestEHRandSetParamterinDocSearch();
                wpfobject.SelectCheckBox("encryptEnabledCheckBox");
                EhrEncryptionProvider = wpfobject.GetAnyUIItem<ITabPage, ComboBox>(ehr.GetCurrentTabItem(), "encryptionProviderCmb", 0);
                EhrEncryptionProvider.Select(0);
                ehr.SearchDocumentInTestEHR(EntryUUID: EntryUUIDList[1], SAMLpath: "default");
                EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);

                //Verfiy the values at the HTML file at Temp folder.
                if ((VerfiyThePostXMLAttribute(HTMLFilepath, "entryUUID", EntryUUIDList[1])) && (VerfiyThePostXMLAttribute(HTMLFilepath, "SAMLAssertion", EncryptedSAML)))
                    result.steps[++ExecutedSteps].AddPassStatusList();
                else
                    result.steps[++ExecutedSteps].AddFailStatusList();

                //Naviage to the HTML file created for Post request from TestEHR.
                files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                processid = StartWireshark(Config.XDSHTTPS_Registery, Config.XDSRepository, BatFilePath: @"\OtherFiles\XMLtshark.bat");
                login.NavigateToIntegratorURL("file:///" + files[0]);

                //matching Documnet is Displayed
                integratorstudies = null;
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                studyviewer = new StudyViewer();
                try
                {
                    PageLoadWait.WaitForWebElement(studyviewer.SeriesViewer_1X1_ByElement, "visible");
                    if (studyviewer.SeriesViewer_1X1().Displayed)
                    {
                        result.steps[ExecutedSteps].SetPath(testid + "_1_", ExecutedSteps);
                        if (studies.CompareImage(result.steps[ExecutedSteps], studyviewer.SeriesViewer_1X1()))
                            result.steps[ExecutedSteps].AddPassStatusList("Image Comapre passed for the step 3");
                        else
                            result.steps[ExecutedSteps].AddFailStatusList();
                    }
                    else
                        result.steps[ExecutedSteps].AddFailStatusList("Fail : Study is not loaded");
                }
                catch (Exception ex)
                {
                    result.steps[ExecutedSteps].AddFailStatusList("Fail : Study is not loaded");
                }

                EndWireshark(processid, waitTime: 30);

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }


                //Step 6
                // Vefiy the Assesrtion in the Wireshark Log File.
                ++ExecutedSteps;
                WiresharkOutput = System.IO.File.ReadAllText(tsharkListernerOutput1);
                SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);
                IList<string> ITI43SAML = GetITI43FromWireSharkOutput(WiresharkOutput);

                if (SAMLAssesrtionFromWireSharkOutput.Count == 0)
                    result.steps[ExecutedSteps].AddFailStatusList("No SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                else
                {
                    result.steps[ExecutedSteps].AddPassStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                    if (CompareSAMLAssertionXML(SAMLAssesrtionFromWireSharkOutput, defaultSAMLAssesrtion))
                    {
                        result.steps[ExecutedSteps].AddPassStatusList();
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList();
                    }
                }

                if (ITI43SAML.Count == 0)
                    result.steps[ExecutedSteps].AddFailStatusList("No SAML Assertion can be found in header of iti-43 requests.");
                else
                {
                    result.steps[ExecutedSteps].AddPassStatusList("SAML Assertion can be found in header of iti-43  requests.");
                    if (CompareSAMLAssertionXML(ITI43SAML, defaultSAMLAssesrtion))
                    {
                        result.steps[ExecutedSteps].AddPassStatusList();
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList();
                    }
                }

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Close browser
                ehr.KillProcessByName("chrome");
                ehr.KillProcessByName("iexplore");
                ehr.KillProcessByName("firefox");
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login.CreateNewSesion();

                //Return Result
                return result;
            }
            finally
            {
                ehr.CloseEHR();
                basepage.SetWebConfigValue(Config.webconfig, "Application.Integrator.TimeSpan", "0");
                RestartIISUsingexe();

                //Close browser
                ehr.KillProcessByName("chrome");
                ehr.KillProcessByName("iexplore");
                ehr.KillProcessByName("firefox");
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login.CreateNewSesion();
            }
        }

        /// <summary>
        ///  Viewing XDS non-KOS document launched from integration URL with Evidence Token provided (non-XCA)
        /// </summary>
        public TestCaseResult Test_167996(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                GettestDataForHUS(filepath, testid);
                string[] EntryUUIDList = EntryUUID.Split(',');
                string[] DocumentIdList = DocumentId.Split(',');
                string[] RepositoryIdList = RepositoryId.Split(',');
                String SAMLName = (String)ReadExcel.GetTestData(filepath, "TestData", "HUS", "SAMLName");
                //String SAMLpath = (String)ReadExcel.GetTestData(filepath, "TestData", "HUS", "SALMpath");
                SAMLpath = "default";
                //Pre - Condition
                //Create a Domain User
                createDomainForHUS(testid);

                //Step 1
                LaunchTestEHRandSetParamterinDocSearch();
                ehr.SearchDocumentInTestEHR(EntryUUID: EntryUUIDList[0] , MIMEtype: "image/jpeg");
                string EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                string HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);

                //check the entryUUID in the launched URL
                if ((VerfiyThePostXMLAttribute(HTMLFilepath, "entryUUID", EntryUUIDList[0])))
                    result.steps[++ExecutedSteps].AddPassStatusList();
                else
                    result.steps[++ExecutedSteps].AddFailStatusList();

                try
                {
                    if (VerfiyThePostXMLAttribute(HTMLFilepath, "SAMLAssertion", EncryptedSAML))
                        result.steps[ExecutedSteps].AddFailStatusList();
                }
                catch (Exception ex)
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                    Logger.Instance.InfoLog("Unable to find the SAML Assertion");
                }

                //Create a new Session and Launch URL
                //Naviage to the HTML file created for Post request from TestEHR.
                string[] files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                Process processid = StartWireshark(Config.XDSHTTPS_Registery, Config.XDSRepository, BatFilePath: @"\OtherFiles\XMLtshark.bat");
                login.NavigateToIntegratorURL("file:///" + files[0]);

                //matching Documnet is Displayed
                IntegratorStudies integratorstudies = null;
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                PageLoadWait.WaitForPageLoad(20);
                StudyViewer studyviewer = new StudyViewer();
                try
                {
                    PageLoadWait.WaitForWebElement(studyviewer.SeriesViewer_1X1_ByElement, "visible");
                    if (studyviewer.SeriesViewer_1X1().Displayed)
                    {
                        result.steps[ExecutedSteps].SetPath(testid + "_1_", ExecutedSteps);
                        if (!studies.CompareImage(result.steps[ExecutedSteps], studyviewer.SeriesViewer_1X1()))
                            result.steps[ExecutedSteps].AddFailStatusList();
                    }
                    else
                        result.steps[ExecutedSteps].AddFailStatusList();
                }
                catch (Exception ex)
                {
                    result.steps[ExecutedSteps].AddFailStatusList(ex.Message);
                    Logger.Instance.InfoLog("Unable to find the viewport in the study viewer");
                }
                EndWireshark(processid, waitTime: 30);
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Step 2
                // Vefiy the Assesrtion in the Wireshark Log File.
                ++ExecutedSteps;
                string WiresharkOutput = System.IO.File.ReadAllText(tsharkListernerOutput1);
                IList<string> SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);
                IList<string> ITI43SAML =  GetITI43FromWireSharkOutput(WiresharkOutput);

                if (SAMLAssesrtionFromWireSharkOutput.Count != 0 && ITI43SAML.Count !=0 )
                    result.steps[ExecutedSteps].AddFailStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                else
                    result.steps[ExecutedSteps].AddPassStatusList("No SAML Assertion can be found in header of iti-18 and iti-43 requests.");

                if (WiresharkOutput.Contains("saml:Assertion"))
                    result.steps[ExecutedSteps].AddFailStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");

                Logger.Instance.InfoLog("HTML file is created in the Temp Folder for the Launched URL");
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }


                //Step 3
                servicetool.LaunchServiceTool();
                servicetool.RestartService();
                Thread.Sleep(5000);
                servicetool.CloseServiceTool();
                servicetool.RestartIISUsingexe();
                Thread.Sleep(5000);
                servicetool.RestartIISUsingexe();
                launchNewStudyForITI43Reset();

                //Search and Launch Documnet from TESTEHR.
                LaunchTestEHRandSetParamterinDocSearch();
                ehr.SearchDocumentInTestEHR(EntryUUID: EntryUUIDList[0], SAMLpath: SAMLpath, MIMEtype: "image/jpeg");
                EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);
                
                //Verfiy the values at the HTML file at Temp folder.
                if ((VerfiyThePostXMLAttribute(HTMLFilepath, "entryUUID", EntryUUIDList[0])) && (VerfiyThePostXMLAttribute(HTMLFilepath, "SAMLAssertion", EncryptedSAML)))
                    result.steps[++ExecutedSteps].AddPassStatusList();
                else
                    result.steps[++ExecutedSteps].AddFailStatusList();

                //Naviage to the HTML file created for Post request from TestEHR.
                files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                processid = StartWireshark(Config.XDSHTTPS_Registery, Config.XDSRepository, BatFilePath: @"\OtherFiles\XMLtshark.bat");
                login.NavigateToIntegratorURL("file:///" + files[0]);

                //matching Documnet is Displayed
                integratorstudies = null;
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                studyviewer = new StudyViewer();
                try
                {
                    PageLoadWait.WaitForWebElement(studyviewer.SeriesViewer_1X1_ByElement, "visible");
                    if (studyviewer.SeriesViewer_1X1().Displayed)
                    {
                        result.steps[ExecutedSteps].SetPath(testid + "_1_", ExecutedSteps);
                        if (studies.CompareImage(result.steps[ExecutedSteps], studyviewer.SeriesViewer_1X1()))
                            result.steps[ExecutedSteps].AddPassStatusList("Image Comapre passed for the step 3");
                        else
                            result.steps[ExecutedSteps].AddFailStatusList();
                    }
                    else
                        result.steps[ExecutedSteps].AddFailStatusList("Fail : Study is not loaded");
                }
                catch (Exception ex)
                {
                    result.steps[ExecutedSteps].AddFailStatusList("Fail : Study is not loaded");
                }

                EndWireshark(processid, waitTime: 30);

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }


                //Step 4
                // Vefiy the Assesrtion in the Wireshark Log File.
                ++ExecutedSteps;
                WiresharkOutput = System.IO.File.ReadAllText(tsharkListernerOutput1);
                SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);
                ITI43SAML = GetITI43FromWireSharkOutput(WiresharkOutput);

                if (SAMLAssesrtionFromWireSharkOutput.Count == 0)
                    result.steps[ExecutedSteps].AddFailStatusList("No SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                else
                {
                    result.steps[ExecutedSteps].AddPassStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                    if(CompareSAMLAssertionXML(SAMLAssesrtionFromWireSharkOutput, defaultSAMLAssesrtion))
                    {
                        result.steps[ExecutedSteps].AddPassStatusList();
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList();
                    }
                }

                if (ITI43SAML.Count == 0)
                    result.steps[ExecutedSteps].AddFailStatusList("No SAML Assertion can be found in header of iti-43 requests.");
                else
                {
                    result.steps[ExecutedSteps].AddPassStatusList("SAML Assertion can be found in header of iti-43  requests.");
                    if (CompareSAMLAssertionXML(ITI43SAML, defaultSAMLAssesrtion))
                    {
                        result.steps[ExecutedSteps].AddPassStatusList();
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList();
                    }
                }

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }


                //Step 5
                //Search and Launch Documnet from TESTEHR with invaild EntryUUID
                LaunchTestEHRandSetParamterinDocSearch();
                ehr.SearchDocumentInTestEHR(EntryUUID: EntryUUID+"123", SAMLpath: SAMLpath, MIMEtype: "image/jpeg");
                EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);

                //Naviage to the HTML file created for Post request from TestEHR.
                files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                login.NavigateToIntegratorURL("file:///" + files[0]);

                basepage.SwitchTo("id", "IntegratorHomeFrame");
                IWebElement errormessage = BasePage.FindElementByCss("span[id='m_title']");
                if (errormessage.Text.Contains("Error Occurred in operation: No XDS document associated with the Entry UUID"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }


                //Step 6
                servicetool.LaunchServiceTool();
                servicetool.RestartService();
                Thread.Sleep(5000);
                servicetool.CloseServiceTool();
                servicetool.RestartIISUsingexe();
                Thread.Sleep(5000);
                servicetool.RestartIISUsingexe();
                launchNewStudyForITI43Reset();
                //Search and Launch Documnet from TESTEHR with  DocumnetId and  repositryID
                LaunchTestEHRandSetParamterinDocSearch();
                //ehr.SetCommonParameters(address: URL, user: username, domain: TestDomain, usersharing: "True", SecurityID: username + "-" + username, autoendsession: "True", usepostmethod: "check");
                ehr.SearchDocumentInTestEHR(DocumentId: DocumentIdList[0], RepositoryId: RepositoryIdList[0], SAMLpath: SAMLpath, MIMEtype: "image/jpeg");
                EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);
                
                //Verfiy the values at the HTML file at Temp folder.
                if (VerfiyThePostXMLAttribute(HTMLFilepath, "documentID", DocumentIdList[0]) && VerfiyThePostXMLAttribute(HTMLFilepath, "repositoryId", RepositoryIdList[0]) && VerfiyThePostXMLAttribute(HTMLFilepath, "SAMLAssertion", EncryptedSAML))
                    result.steps[++ExecutedSteps].AddPassStatusList();
                else
                    result.steps[++ExecutedSteps].AddFailStatusList();

                //Naviage to the HTML file created for Post request from TestEHR.
                files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                processid = StartWireshark(Config.XDSHTTPS_Registery, Config.XDSRepository, BatFilePath: @"\OtherFiles\XMLtshark.bat");
                login.NavigateToIntegratorURL("file:///" + files[0]);

                //matching Documnet is Displayed
                integratorstudies = null;
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                studyviewer = new StudyViewer();
                try
                {
                    PageLoadWait.WaitForWebElement(studyviewer.SeriesViewer_1X1_ByElement, "visible");
                    if (studyviewer.SeriesViewer_1X1().Displayed)
                    {
                        result.steps[ExecutedSteps].SetPath(testid + "_1_", ExecutedSteps);
                        if (studies.CompareImage(result.steps[ExecutedSteps], studyviewer.SeriesViewer_1X1()))
                            result.steps[ExecutedSteps].AddPassStatusList("Image Comapre passed for the step 3");
                        else
                            result.steps[ExecutedSteps].AddFailStatusList();
                    }
                    else
                        result.steps[ExecutedSteps].AddFailStatusList("Fail : Study is not loaded");
                }
                catch (Exception ex)
                {
                    result.steps[ExecutedSteps].AddFailStatusList("Fail : Study is not loaded");
                }

                EndWireshark(processid, waitTime: 30);
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }



                //Step 7
                // Vefiy the Assesrtion in the Wireshark Log File.
                ++ExecutedSteps;
                WiresharkOutput = System.IO.File.ReadAllText(tsharkListernerOutput1);
                SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);
                if (SAMLAssesrtionFromWireSharkOutput.Count == 0)
                    result.steps[ExecutedSteps].AddFailStatusList("No SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                else
                {
                    result.steps[ExecutedSteps].AddPassStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests." + SAMLAssesrtionFromWireSharkOutput.Count);
                    if(CompareSAMLAssertionXML(SAMLAssesrtionFromWireSharkOutput, defaultSAMLAssesrtion))
                    {
                        result.steps[ExecutedSteps].AddPassStatusList() ;
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList();
                    }
                }

                ITI43SAML = GetITI43FromWireSharkOutput(WiresharkOutput);
                if (ITI43SAML.Count == 0)
                    result.steps[ExecutedSteps].AddFailStatusList("No SAML Assertion can be found in header of iti-43 requests.");
                else
                {
                    result.steps[ExecutedSteps].AddPassStatusList("SAML Assertion can be found in header of iti-43  requests.");
                    if (CompareSAMLAssertionXML(ITI43SAML, defaultSAMLAssesrtion))
                    {
                        result.steps[ExecutedSteps].AddPassStatusList();
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList();
                    }
                }


                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }


                //Step 8
                //Search and Launch Documnet from TESTEHR with invalid DocumnetId and repositryID
                LaunchTestEHRandSetParamterinDocSearch();
                ehr.SearchDocumentInTestEHR(DocumentId: DocumentIdList[0]+"123", RepositoryId: RepositoryIdList[0], SAMLpath: SAMLpath, MIMEtype: "image/jpeg");
                EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);
                

                //Naviage to the HTML file created for Post request from TestEHR.
                files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                login.NavigateToIntegratorURL("file:///" + files[0]);
                basepage.SwitchTo("id", "IntegratorHomeFrame");
                errormessage = BasePage.FindElementByCss("span[id='m_title']");
                if (errormessage.Text.Contains("Error Occurred in operation: No XDS document associated with the document ID :"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 9
                //Search and Launch Documnet from TESTEHR with  DocumnetId and invaild repositryID
                LaunchTestEHRandSetParamterinDocSearch();
                ehr.SearchDocumentInTestEHR(DocumentId: DocumentIdList[0] , RepositoryId: RepositoryIdList[0]+"123", SAMLpath: SAMLpath, MIMEtype: "image/jpeg");
                EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);
                

                //Naviage to the HTML file created for Post request from TestEHR.
                files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                login.NavigateToIntegratorURL("file:///" + files[0]);
                basepage.SwitchTo("id", "IntegratorHomeFrame");
                errormessage = BasePage.FindElementByCss("span[id='m_title']");
                if (errormessage.Text.Contains("Error Occurred in operation: Repository ID is invalid"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 10.
                servicetool.LaunchServiceTool();
                servicetool.RestartService();
                Thread.Sleep(5000);
                servicetool.CloseServiceTool();
                servicetool.RestartIISUsingexe();
                Thread.Sleep(5000);
                servicetool.RestartIISUsingexe();
                launchNewStudyForITI43Reset();

                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Document View");
                wpfobject.WaitTillLoad();
                ehr.SetCommonParameters(address: URL, user: adminUserName, domain: "SuperAdminGroup", usersharing: "True", SecurityID: "administrator-administrator", autoendsession: "True", usepostmethod: "check", ExamList : null);
                //ehr.SetCommonParameters(address: URL, user: username, domain: TestDomain, usersharing: "True", SecurityID: username + "-" + username, autoendsession: "True", usepostmethod: "check");
                //ehr.SearchDocumentInTestEHR(EntryUUID: EntryUUID , MIMEtype: "text/xml");
                ehr.SearchDocumentInTestEHR(EntryUUID: EntryUUIDList[1], DocumentId: DocumentIdList[1], RepositoryId: RepositoryIdList[1], SAMLpath: SAMLpath, MIMEtype: "text/xml");
                EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);

                //check the entryUUID in the launched URL
                if (VerfiyThePostXMLAttribute(HTMLFilepath, "entryUUID", EntryUUIDList[1]) && VerfiyThePostXMLAttribute(HTMLFilepath, "repositoryId", RepositoryIdList[1]) && VerfiyThePostXMLAttribute(HTMLFilepath, "documentID", DocumentIdList[1]) && VerfiyThePostXMLAttribute(HTMLFilepath, "SAMLAssertion", EncryptedSAML))
                     result.steps[++ExecutedSteps].AddPassStatusList();
                else
                    result.steps[++ExecutedSteps].AddFailStatusList();
                

                //Create a new Session and Launch URL
                //Naviage to the HTML file created for Post request from TestEHR.
                files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                processid = StartWireshark(Config.XDSHTTPS_Registery, Config.XDSRepository, BatFilePath: @"\OtherFiles\XMLtshark.bat");
                login.NavigateToIntegratorURL("file:///" + files[0]);

                //matching Documnet is Displayed
                integratorstudies = null;
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                PageLoadWait.WaitForPageLoad(20);
                studyviewer = new StudyViewer();
                try
                {
                    PageLoadWait.WaitForWebElement(studyviewer.DocumentViewportContainer, "visible");
                    if (studyviewer.SeriesViewer_1X1().Displayed)
                    {
                        result.steps[ExecutedSteps].SetPath(testid + "_1_", ExecutedSteps);
                        if (!studies.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElement(studyviewer.DocumentViewportContainer)));
                            result.steps[ExecutedSteps].AddFailStatusList();
                    }
                    else
                        result.steps[ExecutedSteps].AddFailStatusList();
                }
                catch (Exception ex)
                {
                    result.steps[ExecutedSteps].AddFailStatusList(ex.Message);
                    Logger.Instance.InfoLog("Unable to find the viewport in the study viewer");
                }
                EndWireshark(processid, waitTime: 30);
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Step 11
                // Vefiy the Assesrtion in the Wireshark Log File.
                ++ExecutedSteps;
                WiresharkOutput = System.IO.File.ReadAllText(tsharkListernerOutput1);
                SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);
                if (SAMLAssesrtionFromWireSharkOutput.Count == 0)
                    result.steps[ExecutedSteps].AddFailStatusList("No SAML Assertion can be found in header of iti-18 requests.");
                else
                    result.steps[ExecutedSteps].AddPassStatusList("SAML Assertion can be found in header of iti-18 requests.");

                //If there is Token check the Token with SAMl Assesrtion
                if(! CompareSAMLAssertionXML(SAMLAssesrtionFromWireSharkOutput, defaultSAMLAssesrtion) )
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }

                ITI43SAML = GetITI43FromWireSharkOutput(WiresharkOutput);
                if (ITI43SAML.Count == 0)
                    result.steps[ExecutedSteps].AddFailStatusList("No SAML Assertion can be found in header of iti-43 requests.");
                else
                {
                    result.steps[ExecutedSteps].AddPassStatusList("SAML Assertion can be found in header of iti-43  requests.");
                    if (CompareSAMLAssertionXML(ITI43SAML, defaultSAMLAssesrtion))
                    {
                        result.steps[ExecutedSteps].AddPassStatusList();
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList();
                    }
                }

                Logger.Instance.InfoLog("HTML file is created in the Temp Folder for the Launched URL");
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }



                //Step 12
                //Search and Launch Documnet from TESTEHR with invalid DocumnetId and repositryID
                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Document View");
                wpfobject.WaitTillLoad();
                ehr.SetCommonParameters(address: URL, user: adminUserName, domain: "SuperAdminGroup", usersharing: "True", SecurityID: "administrator-administrator", autoendsession: "True", usepostmethod: "check", ExamList: null);
                //ehr.SetCommonParameters(address: URL, user: username, domain: TestDomain, usersharing: "True", SecurityID: username + "-" + username, autoendsession: "True", usepostmethod: "check");
                ehr.SearchDocumentInTestEHR(EntryUUID: EntryUUIDList[1], DocumentId: DocumentIdList[1] + "123", RepositoryId: RepositoryIdList[1], SAMLpath: SAMLpath, MIMEtype: "text/xml");
                EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);
                

                //Naviage to the HTML file created for Post request from TestEHR.
                files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                login.NavigateToIntegratorURL("file:///" + files[0]);
                basepage.SwitchTo("id", "IntegratorHomeFrame");
                errormessage = BasePage.FindElementByCss("span[id='m_title']");
                if (errormessage.Text == "Error Occurred in operation: Entered document ID does not match document ID in query result.")
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 13
                //Search and Launch Documnet from TESTEHR with DocumnetId and invalid repositryID
                LaunchTestEHRandSetParamterinDocSearch();
                ehr.SearchDocumentInTestEHR(EntryUUID: EntryUUIDList[1], DocumentId: DocumentIdList[1] , RepositoryId: RepositoryIdList[1]+"123" , SAMLpath: SAMLpath, MIMEtype: "text/xml");
                EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);

                //Naviage to the HTML file created for Post request from TestEHR.
                files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                login.NavigateToIntegratorURL("file:///" + files[0]);
                basepage.SwitchTo("id", "IntegratorHomeFrame");
                errormessage = BasePage.FindElementByCss("span[id='m_title']");
                if (errormessage.Text == "Error Occurred in operation: Entered Respository ID does not match repository ID in query result.")
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }


                //Step 14
                //Search and Launch Documnet from TESTEHR with DocumnetId and invalid repositryID
                LaunchTestEHRandSetParamterinDocSearch();
                ehr.SearchDocumentInTestEHR(EntryUUID: EntryUUIDList[1]+"123", DocumentId: DocumentIdList[1], RepositoryId: RepositoryIdList[1], SAMLpath: SAMLpath, MIMEtype: "text/xml");
                EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);

                //Naviage to the HTML file created for Post request from TestEHR.
                files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                login.NavigateToIntegratorURL("file:///" + files[0]);
                basepage.SwitchTo("id", "IntegratorHomeFrame");
                errormessage = BasePage.FindElementByCss("span[id='m_title']");
                if (errormessage.Text == "Error Occurred in operation: No XDS document associated with the Entry UUID")
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 15
                //Search and Launch Documnet from TESTEHR with DocumnetId and invalid repositryID
                LaunchTestEHRandSetParamterinDocSearch();
                XmlDocument docTemp = new XmlDocument();
                docTemp.Load(Directory.GetCurrentDirectory() + "\\TestEHRfiles\\samlAssertion.xml");
                docTemp.ToString();
                ehr.SearchDocumentInTestEHR(EntryUUID: EntryUUIDList[1] , SAMLString: docTemp.InnerXml+"12;", MIMEtype: "text/xml");
                EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);
                

                //Naviage to the HTML file created for Post request from TestEHR.
                files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                processid = StartWireshark(Config.XDSHTTPS_Registery, Config.XDSRepository, BatFilePath: @"\OtherFiles\XMLtshark.bat");
                login.NavigateToIntegratorURL("file:///" + files[0]);
                basepage.SwitchTo("id", "IntegratorHomeFrame");
                errormessage = BasePage.FindElementByCss("span[id='m_title']");
                if (errormessage.Text.Contains("Error Occurred in operation"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                EndWireshark(processid, waitTime: 30);

                //Step 16
                var LogStartTime = System.DateTime.Now;
                ++ExecutedSteps;
                WiresharkOutput = System.IO.File.ReadAllText(tsharkListernerOutput1);
                SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);
                if (SAMLAssesrtionFromWireSharkOutput.Count == 0)
                    result.steps[ExecutedSteps].AddPassStatusList("No SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");

                if (WiresharkOutput.Contains("saml:Assertion"))
                    result.steps[ExecutedSteps].AddFailStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");

                Logger.Instance.InfoLog("HTML file is created in the Temp Folder for the Launched URL");
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Step 17
                LaunchTestEHRandSetParamterinDocSearch();
                docTemp = new XmlDocument();
                docTemp.Load(Directory.GetCurrentDirectory() + "\\TestEHRfiles\\samlAssertion.xml");
                docTemp.ToString();
                ehr.SearchDocumentInTestEHR( DocumentId: DocumentIdList[1], RepositoryId: RepositoryIdList[1], SAMLString: docTemp.InnerXml + "12;");
                EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);
                
                //Naviage to the HTML file created for Post request from TestEHR.
                files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                processid = StartWireshark(Config.XDSHTTPS_Registery, Config.XDSRepository, BatFilePath: @"\OtherFiles\XMLtshark.bat");
                login.NavigateToIntegratorURL("file:///" + files[0]);
                basepage.SwitchTo("id", "IntegratorHomeFrame");
                errormessage = BasePage.FindElementByCss("span[id='m_title']");
                if (errormessage.Text.Contains("Error Occurred in operation"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                EndWireshark(processid, waitTime: 30);

                //Step 18
                ++ExecutedSteps;
                WiresharkOutput = System.IO.File.ReadAllText(tsharkListernerOutput1);
                SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);
                if (SAMLAssesrtionFromWireSharkOutput.Count == 0)
                    result.steps[ExecutedSteps].AddPassStatusList("No SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");

                if (WiresharkOutput.Contains("saml:Assertion"))
                    result.steps[ExecutedSteps].AddFailStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");

                Logger.Instance.InfoLog("HTML file is created in the Temp Folder for the Launched URL");
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Step 19
                var LogEndTime = System.DateTime.Now;
                var loggedError = string.Empty;
                //Open C\\Windows\Temp\WebAccessDeveloperxxxxxx(date).log to find the error message. - step 30
                try
                {
                    String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
                    for (var i = 1; i >= 1; i++)
                    {
                        String LogFilePath = @"\\" + Config.IConnectIP + @"\c$\Windows\Temp\WebAccessDeveloper-" + Date + "(" + i + ")" + ".log";
                        Stream stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        System.DateTime DateTime = System.DateTime.Now.Date;

                        if (File.Exists(LogFilePath))
                        {
                            StreamReader reader = new StreamReader(stream);
                            var LogValues = basepage.ReadDevTraceLog(LogFilePath, LogStartTime, LogEndTime);
                            foreach (var entry in LogValues)
                            {
                                if (entry.Value["Source"].Contains("OpenContent.Data.Xds.XUA.SamTokenHeader::OnWriteHeaderContents"))
                                    if (entry.Value["Message"].Contains("Exception caught during writing SAML Assertion header."))
                                        {
                                            loggedError = entry.Value["Message"];
                                            break;
                                        }
                            }
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Unable to Read Log file");
                            break;
                        }
                        if (loggedError == "Exception caught during writing SAML Assertion header.")
                            break;
                    }

                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Error Occured While verfiy the WebAccessDeveloper log");
                }
                finally
                {
                    //Validation of message failed in log file
                    if (loggedError == "Exception caught during writing SAML Assertion header.")
                    {
                        result.steps[++ExecutedSteps].StepPass();
                        Logger.Instance.InfoLog("-->Test Step Passed--Found the Log Error");
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].StepFail("Error log in WebAccessDeveloper log file is not as excepted");
                        Logger.Instance.ErrorLog("-->Test Step Failed--Unable to Error Log");
                    }
                }

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                ehr.CloseEHR();
                RestartIISUsingexe();

                //Close browser
                ehr.KillProcessByName("chrome");
                ehr.KillProcessByName("iexplore");
                ehr.KillProcessByName("firefox");
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login.CreateNewSesion();
            }
        }

        /// <summary>
        ///  Viewing XDS KOS document launched from integration URL with Evidence Token provided (non-XCA)
        /// </summary>
        public TestCaseResult Test_168015(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {

                GettestDataForHUS(filepath, testid);
                String SAMLpath = "default";
                string defaultSAMLAssesrtion = Directory.GetCurrentDirectory() + "\\TestEHRfiles\\samlAssertion.xml";
                IList<string> EntryUUIDList = EntryUUID.Split(',');

                //Pre - Condition
                //Create a Domain User
                //createDomainForHUS(testid);

                //Step 1
                LaunchTestEHRandSetParamterinDocSearch();
                ehr.SearchDocumentInTestEHR(EntryUUID: EntryUUIDList[0], MIMEtype: "application/dicom");
                string EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                string HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);

                //check the entryUUID in the launched URL
                if ((VerfiyThePostXMLAttribute(HTMLFilepath, "entryUUID", EntryUUIDList[0])))
                    result.steps[++ExecutedSteps].AddPassStatusList();
                else
                    result.steps[++ExecutedSteps].AddFailStatusList();

                try
                {
                    if (VerfiyThePostXMLAttribute(HTMLFilepath, "SAMLAssertion", EncryptedSAML))
                        result.steps[ExecutedSteps].AddFailStatusList();
                }
                catch (Exception ex)
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                    Logger.Instance.InfoLog("Unable to find the SAML Assertion in the URL");
                }

                //Create a new Session and Launch URL
                //Naviage to the HTML file created for Post request from TestEHR.
                string[] files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                Process processid = StartWireshark(Config.XDSHTTPS_Registery, Config.XDSRepository, BatFilePath: @"\OtherFiles\XMLtshark.bat");
                login.NavigateToIntegratorURL("file:///" + files[0]);

                //matching Documnet is Displayed
                IntegratorStudies integratorstudies = null;
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                PageLoadWait.WaitForPageLoad(20);
                StudyViewer studyviewer = new StudyViewer();
                try
                {
                    PageLoadWait.WaitForWebElement(studyviewer.SeriesViewer_1X1_ByElement, "visible");
                    if (studyviewer.SeriesViewer_1X1().Displayed)
                    {
                        result.steps[ExecutedSteps].SetPath(testid + "_1_", ExecutedSteps);
                        if (!studies.CompareImage(result.steps[ExecutedSteps], studyviewer.SeriesViewer_1X1()))
                            result.steps[ExecutedSteps].AddFailStatusList();
                    }
                    else
                        result.steps[ExecutedSteps].AddFailStatusList();
                }
                catch (Exception ex)
                {
                    result.steps[ExecutedSteps].AddFailStatusList(ex.Message);
                    Logger.Instance.InfoLog("Unable to find the viewport in the study viewer");
                }
                EndWireshark(processid, waitTime: 30);
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Step 2
                // Vefiy the Assesrtion in the Wireshark Log File.
                ++ExecutedSteps;
                string WiresharkOutput = System.IO.File.ReadAllText(tsharkListernerOutput1);
                IList<string> SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);
                IList<string> BodyFromWireSharkOutput = new List<string>();
                BodyFromWireSharkOutput = GetBodyFromWireSharkOutput(WiresharkOutput); 
                IList<string> ITI43SAML = GetITI43FromWireSharkOutput(WiresharkOutput);

                if ( SAMLAssesrtionFromWireSharkOutput.Count != 0 && ITI43SAML.Count!=0 )
                    result.steps[ExecutedSteps].AddFailStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                else
                    result.steps[ExecutedSteps].AddPassStatusList("No SAML Assertion can be found in header of iti-18 and iti-43 requests.");

                if (WiresharkOutput.Contains("saml:Assertion"))
                    result.steps[ExecutedSteps].AddFailStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");

                if (!BodyFromWireSharkOutput[0].ToString().Contains(EntryUUIDList[0]))
                    result.steps[ExecutedSteps].AddFailStatusList(EntryUUIDList[0] + "is not present in the Request");

                if (ITI43SAML.Count != 0)
                    result.steps[ExecutedSteps].AddFailStatusList("SAML Assertion can be found is not present in the ITI43 Request");

                Logger.Instance.InfoLog("HTML file is created in the Temp Folder for the Launched URL");
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Step 3
                servicetool.LaunchServiceTool();
                servicetool.RestartService();
                Thread.Sleep(5000);
                servicetool.CloseServiceTool();
                servicetool.RestartIISUsingexe();
                Thread.Sleep(5000);
                servicetool.RestartIISUsingexe();
                launchNewStudyForITI43Reset();

                LaunchTestEHRandSetParamterinDocSearch();
                ehr.SearchDocumentInTestEHR(EntryUUID: EntryUUIDList[0], SAMLpath: SAMLpath, MIMEtype: "application/dicom");
                EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);


                //Verfiy the values at the HTML file at Temp folder.
                if ((VerfiyThePostXMLAttribute(HTMLFilepath, "entryUUID", EntryUUIDList[0])) && (VerfiyThePostXMLAttribute(HTMLFilepath, "SAMLAssertion", EncryptedSAML)))
                    result.steps[++ExecutedSteps].AddPassStatusList();
                else
                    result.steps[++ExecutedSteps].AddFailStatusList();

                //Naviage to the HTML file created for Post request from TestEHR.
                files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                processid = StartWireshark(Config.XDSHTTPS_Registery, Config.XDSRepository, BatFilePath: @"\OtherFiles\XMLtshark.bat");
                login.NavigateToIntegratorURL("file:///" + files[0]);

                //matching Documnet is Displayed
                integratorstudies = null;
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                studyviewer = new StudyViewer();
                try
                {
                    PageLoadWait.WaitForWebElement(studyviewer.SeriesViewer_1X1_ByElement, "visible");
                    if (studyviewer.SeriesViewer_1X1().Displayed)
                    {
                        result.steps[ExecutedSteps].SetPath(testid + "_1_", ExecutedSteps);
                        if (studies.CompareImage(result.steps[ExecutedSteps], studyviewer.SeriesViewer_1X1()))
                            result.steps[ExecutedSteps].AddPassStatusList("Image Comapre passed for the step 3");
                        else
                            result.steps[ExecutedSteps].AddFailStatusList();
                    }
                    else
                        result.steps[ExecutedSteps].AddFailStatusList("Fail : Study is not loaded");
                }
                catch (Exception ex)
                {
                    result.steps[ExecutedSteps].AddFailStatusList("Fail : Study is not loaded");
                }

                EndWireshark(processid, waitTime: 30);

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }


                //Step 4
                // Vefiy the Assesrtion in the Wireshark Log File.
                ++ExecutedSteps;
                WiresharkOutput = System.IO.File.ReadAllText(tsharkListernerOutput1);
                SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);
                try
                {
                    BodyFromWireSharkOutput = GetBodyFromWireSharkOutput(WiresharkOutput);
                }
                catch(Exception ex)
                {

                }
                ITI43SAML = GetITI43FromWireSharkOutput(WiresharkOutput);
                if (!WiresharkOutput.Contains(EntryUUIDList[0]))
                    result.steps[ExecutedSteps].AddFailStatusList(EntryUUIDList[0] + "is not present in the Request");

                if (SAMLAssesrtionFromWireSharkOutput.Count == 0)
                    result.steps[ExecutedSteps].AddFailStatusList("No SAML Assertion can be found in header of iti-18  requests.");
                else
                {
                    result.steps[ExecutedSteps].AddPassStatusList("SAML Assertion can be found in header of iti-18  requests.");
                    if (CompareSAMLAssertionXML(SAMLAssesrtionFromWireSharkOutput, defaultSAMLAssesrtion))
                    {
                        result.steps[ExecutedSteps].AddPassStatusList();
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList();
                    }
                }

                if (ITI43SAML.Count == 0)
                    result.steps[ExecutedSteps].AddFailStatusList("No SAML Assertion can be found in header of iti-43 requests.");
                else
                {
                    result.steps[ExecutedSteps].AddPassStatusList("SAML Assertion can be found in header of iti-43  requests.");
                    if (CompareSAMLAssertionXML(ITI43SAML, defaultSAMLAssesrtion))
                    {
                        result.steps[ExecutedSteps].AddPassStatusList();
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList();
                    }
                }

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }


                //Step 5
                servicetool.LaunchServiceTool();
                servicetool.RestartService();
                Thread.Sleep(5000);
                servicetool.CloseServiceTool();
                servicetool.RestartIISUsingexe();
                Thread.Sleep(5000);
                servicetool.RestartIISUsingexe();
                launchNewStudyForITI43Reset();

                LaunchTestEHRandSetParamterinDocSearch();
                ehr.SearchDocumentInTestEHR(DocumentId: DocumentId, RepositoryId: RepositoryId, SAMLpath: SAMLpath, MIMEtype: "application/dicom");
                EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);
                
                //Verfiy the values at the HTML file at Temp folder.
                if (VerfiyThePostXMLAttribute(HTMLFilepath, "documentID", DocumentId) && VerfiyThePostXMLAttribute(HTMLFilepath, "repositoryId", RepositoryId) && VerfiyThePostXMLAttribute(HTMLFilepath, "SAMLAssertion", EncryptedSAML))
                    result.steps[++ExecutedSteps].AddPassStatusList();
                else
                    result.steps[++ExecutedSteps].AddFailStatusList();

                //Naviage to the HTML file created for Post request from TestEHR.
                files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                processid = StartWireshark(Config.XDSHTTPS_Registery, Config.XDSRepository, BatFilePath: @"\OtherFiles\XMLtshark.bat");
                login.NavigateToIntegratorURL("file:///" + files[0]);

                //matching Documnet is Displayed
                integratorstudies = null;
                integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                studyviewer = new StudyViewer();
                try
                {
                    PageLoadWait.WaitForWebElement(studyviewer.SeriesViewer_1X1_ByElement, "visible");
                    if (studyviewer.SeriesViewer_1X1().Displayed)
                    {
                        result.steps[ExecutedSteps].SetPath(testid + "_1_", ExecutedSteps);
                        if (studies.CompareImage(result.steps[ExecutedSteps], studyviewer.studyPanel()))
                            result.steps[ExecutedSteps].AddPassStatusList("Image Comapre passed for the step 3");
                        else
                            result.steps[ExecutedSteps].AddFailStatusList();
                    }
                    else
                        result.steps[ExecutedSteps].AddFailStatusList("Fail : Study is not loaded");
                }
                catch (Exception ex)
                {
                    result.steps[ExecutedSteps].AddFailStatusList("Fail : Study is not loaded");
                }

                EndWireshark(processid, waitTime: 30);
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }


                //Step 6
                // Vefiy the Assesrtion in the Wireshark Log File.
                ++ExecutedSteps;
                WiresharkOutput = System.IO.File.ReadAllText(tsharkListernerOutput1);
                SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);
                try
                {
                    BodyFromWireSharkOutput = GetBodyFromWireSharkOutput(WiresharkOutput);
                }
                catch(Exception ex)
                { }

                ITI43SAML = GetITI43FromWireSharkOutput(WiresharkOutput);

                if (SAMLAssesrtionFromWireSharkOutput.Count == 0)
                    result.steps[ExecutedSteps].AddFailStatusList("No SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                else
                {
                    result.steps[ExecutedSteps].AddPassStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                    if (CompareSAMLAssertionXML(SAMLAssesrtionFromWireSharkOutput, defaultSAMLAssesrtion))
                    {
                        result.steps[ExecutedSteps].AddPassStatusList();
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList();
                    }
                }

                if (ITI43SAML.Count == 0)
                    result.steps[ExecutedSteps].AddFailStatusList("unable to find the iti-43 requests.");
                else
                {
                    result.steps[ExecutedSteps].AddPassStatusList("SAML Assertion can be found in header of iti-43  requests.");
                    if (CompareSAMLAssertionXML(ITI43SAML, defaultSAMLAssesrtion))
                    {
                        result.steps[ExecutedSteps].AddPassStatusList();
                    }
                    else
                    {
                        result.steps[ExecutedSteps].AddFailStatusList();
                    }
                }

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }


                //Step 7
                //Search and Launch Documnet from TESTEHR with DocumnetId and invalid repositryID
                LaunchTestEHRandSetParamterinDocSearch();
                XmlDocument docTemp = new XmlDocument();
                docTemp.Load(Directory.GetCurrentDirectory() + "\\TestEHRfiles\\samlAssertion.xml");
                docTemp.ToString();
                ehr.SearchDocumentInTestEHR(EntryUUID: EntryUUID, SAMLString: docTemp.InnerXml + "12;", MIMEtype: "text/xml");
                EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);
                
                //Naviage to the HTML file created for Post request from TestEHR.
                files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                processid = StartWireshark(Config.XDSHTTPS_Registery, Config.XDSRepository, BatFilePath: @"\OtherFiles\XMLtshark.bat");
                login.NavigateToIntegratorURL("file:///" + files[0]);
                Driver.SwitchTo().Frame("IntegratorHomeFrame");
                IWebElement errormessage = BasePage.FindElementByCss("span[id='m_title']");
                if (errormessage.Text.Contains("Error Occurred in operation"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                EndWireshark(processid, waitTime: 30);

                //Step 8
                ++ExecutedSteps;
                WiresharkOutput = System.IO.File.ReadAllText(tsharkListernerOutput1);
                SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);
                if (SAMLAssesrtionFromWireSharkOutput.Count == 0)
                    result.steps[ExecutedSteps].AddPassStatusList("No SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");

                if (WiresharkOutput.Contains("saml:Assertion"))
                    result.steps[ExecutedSteps].AddFailStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");

                Logger.Instance.InfoLog("HTML file is created in the Temp Folder for the Launched URL");
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Step 9
                LaunchTestEHRandSetParamterinDocSearch();
                docTemp = new XmlDocument();
                docTemp.Load(Directory.GetCurrentDirectory() + "\\TestEHRfiles\\samlAssertion.xml");
                docTemp.ToString();
                ehr.SearchDocumentInTestEHR(DocumentId: DocumentId, RepositoryId: RepositoryId, SAMLString: docTemp.InnerXml + "12;");
                EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);
                

                //Naviage to the HTML file created for Post request from TestEHR.
                files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                processid = StartWireshark(Config.XDSHTTPS_Registery, Config.XDSRepository, BatFilePath: @"\OtherFiles\XMLtshark.bat");
                login.NavigateToIntegratorURL("file:///" + files[0]);
                Driver.SwitchTo().Frame("IntegratorHomeFrame");
                errormessage = BasePage.FindElementByCss("span[id='m_title']");
                if (errormessage.Text.Contains("Error Occurred in operation"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                EndWireshark(processid, waitTime: 30);

                //Step 10
                ++ExecutedSteps;
                WiresharkOutput = System.IO.File.ReadAllText(tsharkListernerOutput1);
                SAMLAssesrtionFromWireSharkOutput = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);
                if (SAMLAssesrtionFromWireSharkOutput.Count == 0)
                    result.steps[ExecutedSteps].AddPassStatusList("No SAML Assertion can be found in header of iti-18 and iti-43 requests.");
                else
                    result.steps[ExecutedSteps].AddFailStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");

                if (WiresharkOutput.Contains("saml:Assertion"))
                    result.steps[ExecutedSteps].AddFailStatusList("SAML Assertion can be found in header of iti-18 and iti-43 requests.");

                Logger.Instance.InfoLog("HTML file is created in the Temp Folder for the Launched URL");
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }


                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                ehr.CloseEHR();
                RestartIISUsingexe();

                //Close browser
                ehr.KillProcessByName("chrome");
                ehr.KillProcessByName("iexplore");
                ehr.KillProcessByName("firefox");
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login.CreateNewSesion();
            }
        }

        /// <summary>
        ///  Verify Input and Output Parameters
        /// </summary>
        public TestCaseResult Test_167980(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                GettestDataForHUS(filepath, testid);
                IList<bool> step2 = new List<bool>();
                IList<bool> step3 = new List<bool>();

                //Step 1 - Covered in the Environment setup
                ExecutedSteps++;

                //Step 2
                string HUSParamterFilePath = Directory.GetCurrentDirectory().ToString() + "\\TestData\\HUS\\HUS-InputParam-Testing" + ".xlsx";
                ExecutedSteps++;
                for (int i = 1; i <= 16; i++)
                {
                    bool status = true;
                    string EntryUUIDTemp = EntryUUID, DocumentIdTemp = DocumentId, RepositoryIdTemp = RepositoryId;
                    int DocumentNode = int.Parse((String)ReadExcel.GetTestData(HUSParamterFilePath, "TestData", i.ToString(), "documentId"));
                    int RepositoryNode = int.Parse((String)ReadExcel.GetTestData(HUSParamterFilePath, "TestData", i.ToString(), "respositoryId"));
                    int SAMLAssertionNode = int.Parse((String)ReadExcel.GetTestData(HUSParamterFilePath, "TestData", i.ToString(), "SAMLAssertion"));
                    int EntryUUIDNode = int.Parse((String)ReadExcel.GetTestData(HUSParamterFilePath, "TestData", i.ToString(), "entryUUID"));

                    if (EntryUUIDNode == 0) EntryUUIDTemp = "";
                    if (DocumentNode == 0) DocumentIdTemp = "";
                    if (RepositoryNode == 0) RepositoryIdTemp = "";
                    if (SAMLAssertionNode == 0) SAMLpath = ""; else SAMLpath = "default";

                    //Delete All  HTML files in the temp Path
                    string[] files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                    foreach (string file in files)
                        File.Delete(file);

                    LaunchTestEHRandSetParamterinDocSearch();
                    ehr.SetCommonParameters(address: URL, user: adminUserName, domain: "SuperAdminGroup", usersharing: "True", SecurityID: "administrator-administrator", autoendsession: "True", usepostmethod: "check", ExamList: null);

                    string cmd_Url = ehr.SearchDocumentInTestEHR(EntryUUID: EntryUUIDTemp, DocumentId: DocumentIdTemp, RepositoryId: RepositoryIdTemp, SAMLpath: SAMLpath);
                    string EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
                    bool WaitforStudyLoad = (EntryUUIDNode != 0 || (RepositoryNode != 0 && DocumentNode != 0));
                    string HTMLPath = ehr.GetPostFilePath();

                    //Create a new Session and Launch URL
                    //login.CreateNewSesion();
                    files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                    Process processid = StartWireshark(Config.XDSHTTPS_Registery, Config.XDSRepository, BatFilePath: @"\OtherFiles\XMLtshark.bat");
                    login.NavigateToIntegratorURL("file:///" + files[0]);
                    if (EntryUUIDNode != 0 || (RepositoryNode != 0 && DocumentNode != 0))
                    {
                        IntegratorStudies integratorstudies = null;
                        integratorstudies = (IntegratorStudies)login.NavigateToIntegratorFrame("Studies");
                        StudyViewer studyviewer = new StudyViewer();
                        if (studyviewer.SeriesViewer_1X1().Displayed)
                        {
                            result.steps[ExecutedSteps].SetPath(testid + "_1_" + i, ExecutedSteps);
                            if (!studies.CompareImage(result.steps[ExecutedSteps], studyviewer.SeriesViewer_1X1()))
                                step2.Add(false); // Fail
                        }
                        else
                            step2.Add(false); // Fail
                    }
                    else
                    {
                        try
                        {
                            basepage.SwitchTo("id", "IntegratorHomeFrame");
                            IWebElement errormessage = BasePage.FindElementByCss("span[id='m_title']");
                            if (errormessage.Text == "Error Occurred in operation: Value cannot be null. Parameter name: Entry UUID or Document ID.")
                            {
                                step2.Add(true); //Fail
                            }
                            else
                            {
                                step2.Add(false); //Fail
                            }
                        }
                        catch (Exception ex)
                        { }
                    }
                    EndWireshark(processid, waitTime: 30);

                    files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
                    if (files.Count() == 1)
                    {
                        Logger.Instance.InfoLog("HTML file is created in the Temp Folder for the Launched URL");

                        if (EntryUUIDNode != 0)
                        {
                            if (!(VerfiyThePostXMLAttribute(files[0], "entryUUID", EntryUUIDTemp)))
                                status = false;
                        }
                        else
                            if ((VerfiyThePostXMLAttribute(files[0], "entryUUID", EntryUUIDTemp)))
                            status = false;

                        if (RepositoryNode != 0)
                        {
                            if (!(VerfiyThePostXMLAttribute(files[0], "repositoryId", RepositoryIdTemp)))
                                status = false;
                        }
                        else
                            if ((VerfiyThePostXMLAttribute(files[0], "repositoryId", RepositoryIdTemp)))
                            status = false;

                        if (DocumentNode != 0)
                        {
                            if (!(VerfiyThePostXMLAttribute(files[0], "documentID", DocumentIdTemp)))
                                status = false;
                        }
                        else
                            if ((VerfiyThePostXMLAttribute(files[0], "documentID", DocumentIdTemp)))
                            status = false;

                        if (SAMLAssertionNode != 0)
                        {
                            if (!(VerfiyThePostXMLAttribute(files[0], "SAMLAssertion", EncryptedSAML)))
                                status = false;
                        }
                        else
                            if ((VerfiyThePostXMLAttribute(files[0], "SAMLAssertion", EncryptedSAML)))
                            status = false;
                    }
                    else
                    {
                        throw new Exception("Unable to Find the Temp HTMl file for the URL loaded");
                    }

                    if (status == false)
                    {
                        Logger.Instance.InfoLog("Verification fasiled at the scenario " + i);
                        step2.Add(false); //Fail
                    }
                    else
                    {
                        step2.Add(true); //Pass
                        Logger.Instance.InfoLog("Verification Passed at the step " + i);
                    }

                    //Step 3
                    // Vefiy the Assesrtion in the Wireshark Log File.
                    string WiresharkOutput = System.IO.File.ReadAllText(tsharkListernerOutput1);
                    IList<string> SamlAssertionList = new List<string>();
                    SamlAssertionList = GetSAMLAssesrtionFromWireSharkOutput(WiresharkOutput);
                    if (SAMLAssertionNode == 1 && (EntryUUIDNode != 0 || (RepositoryNode != 0 && DocumentNode != 0)))
                    {
                        if (!CompareSAMLAssertionXML(SamlAssertionList, defaultSAMLAssesrtion))
                        {
                            Logger.Instance.ErrorLog("SAML assesrtion inserted at the websocekt between ICA and XDS not matches with the SALM Asesstion used at the TestEHR tool ");
                            step3.Add(false);
                            Logger.Instance.InfoLog("Verification fasiled at the scenario " + i);
                        }
                    }
                    else
                    {
                        if (SamlAssertionList.Count != 0)
                        {
                            step3.Add(false); //Fail
                            Logger.Instance.InfoLog("Able to find the SAML Assesrtion at Websocket between ICA and XDS server when the SAML Assesrtion token is not used at TESTEHR tool");
                            Logger.Instance.InfoLog("Verification fasiled at the scenario " + i);
                        }
                        else
                        {
                            step3.Add(true); // true
                            result.steps[ExecutedSteps].AddPassStatusList("Not Able to find the SAML Assesrtion at Websocket between ICA and XDS server when the SAML Assesrtion token is not used at TESTEHR tool");
                        }
                    }

                    IList<string> BodyParkFromWiresharkOutPut = new List<string>();
                    BodyParkFromWiresharkOutPut = GetBodyFromWireSharkOutput(WiresharkOutput);
                    if (EntryUUIDNode != 0 || (RepositoryNode != 0 && DocumentNode != 0))
                    {
                        if (BodyParkFromWiresharkOutPut.Count == 3)
                            step3.Add(true); // true
                        else
                            step3.Add(false); // true
                    }
                    else
                    {
                        if (BodyParkFromWiresharkOutPut.Count == 0)
                            step3.Add(true); // true
                        else
                            step3.Add(false);
                    }

                }

                if (step2.Any<bool>(status => status == false))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                if (step3.Any<bool>(status => status == false))
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();


                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                GC.Collect();
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);
                //Close browser
                ehr.KillProcessByName("chrome");
                ehr.KillProcessByName("iexplore");
                ehr.KillProcessByName("firefox");
                BasePage.Driver.Quit();
                BasePage.Driver = null;

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                ehr.CloseEHR();
            }
        }

        private bool VerfiyThePostXMLAttribute(string path= null, string Attribute = null, string value= null )
        {
            try
            {
                Thread.Sleep(5000);
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                HtmlDocument hDoc = new HtmlDocument();
                hDoc.LoadHtml(doc.OuterXml);

                HtmlNodeNavigator navigator = (HtmlNodeNavigator)hDoc.CreateNavigator();

                //Get value from given xpath
                string xpath = "//input[@name='" + Attribute + "']";
                string val = navigator.SelectSingleNode(xpath).OuterXml;
                if (val.Contains("value=\"" + value + "\""))
                    Logger.Instance.InfoLog(" The Post File contains the value " + value + " for attribute " + Attribute + "Node Vlaue is " + val);
                Thread.Sleep(5000);
                return val.Contains("value=\"" + value + "\"");
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        //private List<string> GetSAMLAssesrtionFromWireSharkOutput( string WiresharkOutput)
        //    {
        //    List<string> comdOutputList = new List<string>();
        //     foreach (string eachsegmnet in WiresharkOutput.Split(new string[] { "<s:Envelope" }, StringSplitOptions.None))
        //        {
        //            if (eachsegmnet.Contains("<s:Header>"))
        //                if (eachsegmnet.Contains("<saml:Assertion")) 
        //                      comdOutputList.Add("<saml:Assertion" + (eachsegmnet.Split(new string[] { "</s:Envelope" }, StringSplitOptions.None)[0]).Split(new string[] { "<saml:Assertion" }, StringSplitOptions.None)[1].Split(new string[] { "</saml:Assertion>" }, StringSplitOptions.None)[0] + "</saml:Assertion>");
        //    }
        //    return comdOutputList;
        //}

        private List<string> GetSAMLAssesrtionFromWireSharkOutput(string WiresharkOutput)
        {
            List<string> comdOutputList = new List<string>();
            foreach (string eachsegmnet in WiresharkOutput.Split(new string[] { " POST /index/services/registry HTTP/1.1" }, StringSplitOptions.None))
            {
                if (eachsegmnet.Contains("<s:Header>"))
                    if (eachsegmnet.Contains("<saml:Assertion"))
                        comdOutputList.Add("<saml:Assertion" + (eachsegmnet.Split(new string[] { "</s:Envelope" }, StringSplitOptions.None)[0]).Split(new string[] { "<saml:Assertion" }, StringSplitOptions.None)[1].Split(new string[] { "</saml:Assertion>" }, StringSplitOptions.None)[0] + "</saml:Assertion>");
            }
            return comdOutputList;
        }

        private List<string> GetITI43FromWireSharkOutput(string WiresharkOutput)
        {
            List<string> comdOutputList = new List<string>();
            foreach (string eachsegmnet in WiresharkOutput.Split(new string[] { "POST /iti43 HTTP/1.1" }, StringSplitOptions.None))
            {
                string segment = eachsegmnet;
                if (segment.Contains("Reassembled TCP (40"))
                {
                    segment = segment.Split(new string[] { "Reassembled TCP (40" }, StringSplitOptions.None)[1];
                    // string testtext = System.IO.File.ReadAllText(@"D:\BluRing\Batch_Execution\Functional_Automation\Scripts\Selenium\bin\Debug\TestFileForiti43.txt");
                    string testtext = segment;
                    string[] lines = testtext.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                    IList<string> textateachLine = new List<string>();
                    StringBuilder fulstring = new StringBuilder();
                    foreach (string eachline in lines)
                    {
                        try
                        {
                            textateachLine.Add(Regex.Split(eachline, "   ")[1]);
                            fulstring.Append((Regex.Split(eachline, "   ")[1]));
                        }
                        catch (Exception ex) { }
                    }
                    //Regex.Split(lines[25],"\t");
                    string finalSAML = "<saml:Assertion" + fulstring.ToString().Split(new string[] { "<saml:Assertion" }, StringSplitOptions.None)[1].Split(new string[] { "</wsse:Security>" }, StringSplitOptions.None)[0];
                    comdOutputList.Add(finalSAML);
                    //if (segment.Contains("POST /iti43 HTTP"))
                    //{
                    //    segment = "<wsse:Security"+segment.Split(new string[] { "<wsse:Se" }, StringSplitOptions.None)[1];
                    //    comdOutputList.Add(segment.Split(new string[] { "</s:Envelope" }, StringSplitOptions.None)[0]);
                    //}
                }
            }
            return comdOutputList;
        }

        private List<string> GetBodyFromWireSharkOutput(string WiresharkOutput)
        {
            List<string> HeaderList = new List<string>();
            foreach (string eachsegmnet in WiresharkOutput.Split(new string[] { "<s:Envelope" }, StringSplitOptions.None))
            {
                if (eachsegmnet.Contains("<s:Body>"))
                    HeaderList.Add("<saml:Body>" + (eachsegmnet.Split(new string[] { "</s:Body>" }, StringSplitOptions.None)[0]).Split(new string[] { "<s:Body>" }, StringSplitOptions.None)[1]);
            }
            return HeaderList;
        }

        private bool CompareSAMLAssertionXML( IList<string> samlAssertionFromWireShark, string samlAssertionXMLPath=null, bool Comparevalue = true)
        {
            bool status = true;
            if (samlAssertionXMLPath == null)
                samlAssertionXMLPath = Directory.GetCurrentDirectory() + "\\TestEHRfiles\\samlAssertion.xml";
            for (int j = 0; j < samlAssertionFromWireShark.Count; j++)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(samlAssertionFromWireShark[j]);
                doc.Save(System.IO.Path.GetTempPath() + "tempXml.xml");
                XmlDiff xmldiff = new XmlDiff(XmlDiffOptions.IgnoreWhitespace | XmlDiffOptions.IgnorePI | XmlDiffOptions.IgnorePI);
                bool XmlDiff = xmldiff.Compare(System.IO.Path.GetTempPath() + "tempXml.xml", samlAssertionXMLPath , false);
                if (XmlDiff == !Comparevalue)
                {
                    status = false;
                    Logger.Instance.ErrorLog("SAML assesrtion inserted at the websocekt between ICA and XDS matches = "+ !Comparevalue + " with the SALM Asesstion used at the TestEHR tool ");
                    break;
                }
            }
            return status;

        }

        private bool CompareSAMLAtITI43(string ITI43SAMl)
        {
            string tempTextFile = System.IO.Path.GetTempPath() + "tempXml.txt";
            string ITI43SamlTextFile = Directory.GetCurrentDirectory() + "\\TestEHRfiles\\ITI43SamlTextFile.txt";
            System.IO.File.WriteAllText(tempTextFile, ITI43SAMl);
            return ComapreFileUsingHash(tempTextFile, ITI43SamlTextFile);
           // return ITI43SAMl.Contains("wsse:Se");


        }

        public bool ComapreFileUsingHash(string Path1, string Path2)
        {
            HashAlgorithm ha = HashAlgorithm.Create();
            FileStream f1 = new FileStream(Path1, FileMode.Open);
            FileStream f2 = new FileStream(Path2, FileMode.Open);
            byte[] hash1 = ha.ComputeHash(f1);
            byte[] hash2 = ha.ComputeHash(f2);
            f1.Close();
            f2.Close();
            string Hash1 = BitConverter.ToString(hash1);
            string Hash2 = BitConverter.ToString(hash2);
            if (string.Equals(Hash1, Hash2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    private void createDomainForHUS(String testid)
        {
            
            login.LoginIConnect(adminUserName, adminPassword);
            DomainManagement domain = new DomainManagement();
            UserManagement userManagement = new UserManagement();
            TestDomain = "TestDomain" + testid + new Random().Next(1, 10000);
            Role = "Role_" + testid + new Random().Next(1, 10000);
            username = "User_" + testid + new Random().Next(1, 10000);
            DomainAdmin = "DomainAdmin_" + testid + new Random().Next(1, 10000);
            domain.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
            login.Navigate("UserManagement");
            userManagement.CreateUser(username, TestDomain, Role);
            login.Logout();

        }

        public void GettestDataForHUS(string filepath , string testid)
        {
            EntryUUID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EntryUUID");
            DocumentId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DocumentId");
            RepositoryId = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RepositoryId");
            SAMLName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "SAMLName");
            SAMLpath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "SALMpath");
        }

        private void LaunchTestEHRandSetParamterinDocSearch()
        {
            ehr.LaunchEHR();
            wpfobject.GetMainWindow("Test WebAccess EHR");
            wpfobject.SelectTabFromTabItems("Document View");
            wpfobject.WaitTillLoad();
            ehr.SetCommonParameters(address: URL, user: username, domain: TestDomain, usersharing: "True", SecurityID: username + "-" + username, autoendsession: "True", usepostmethod: "check", ExamList : null);
        }


        private void launchNewStudyForITI43Reset()
        {
            LaunchTestEHRandSetParamterinDocSearch();
            ehr.SearchDocumentInTestEHR(EntryUUID: "urn:uuid:31efc51e-009c-4b54-9634-cc731c2d136f", SAMLpath: "default");
            string EncryptedSAML = ehr.GetEncryptedSALMAssesrtion();
            string HTMLFilepath = ehr.GetPostFilePath(CreateNewsession: true, waitforStudyToLoad: false);
            string[] files = Directory.GetFiles(System.IO.Path.GetTempPath(), "*.html");
            login.NavigateToIntegratorURL("file:///" + files[0]);
            Thread.Sleep(5000);
            servicetool.LaunchServiceTool();
            servicetool.RestartService();
            servicetool.CloseServiceTool();
            Thread.Sleep(5000);
            servicetool.RestartIISUsingexe();
            Thread.Sleep(10000);
            servicetool.RestartIISUsingexe();
            Thread.Sleep(5000);
        }






    }

}
