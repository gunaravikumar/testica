using System;
using System.Threading;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using OpenQA.Selenium;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.eHR;

namespace Selenium.Scripts.Tests
{
    class POP : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public MpacLogin mpaclogin { get; set; }
        String User1 = "User1_" + new Random().Next(1, 10000);

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public POP(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ei = new ExamImporter();
            mpaclogin = new MpacLogin();
            wpfobject = new WpfObjects();
        }

        /// <summary>
        /// iCA-12843
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_101481(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                String UploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String PatientList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientList");
                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String XMLFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "XMLFilePath");
                String NodeValue = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NodeValue");
                String NodePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NodePath");
                String[] StudyPath = UploadFilePath.Split('=');
                String[] PatientNames = PatientList.Split(':');
                String[] AccessionNumbers = AccessionIDList.Split(':');

                //Step-1: Launch PACS Gateway and upload a study
                BasePage.RunBatchFile(Config.batchfilepath, StudyPath[0] + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                //Send the study to dicom devices from MergePacs management page
                login.DriverGoTo(login.mpacstudyurl);
                MpacLogin mplogin = new MpacLogin();
                MPHomePage homepage = mplogin.Loginpacs(Config.pacsadmin, Config.pacspassword);
                Tool tools = (Tool)homepage.NavigateTopMenu("Tools");
                tools.NavigateToSendStudy();
                tools.SearchStudy("Patient Name", PatientNames[0], 0);
                tools.MpacSelectStudy("Accession", AccessionNumbers[0]);
                tools.SendStudy(1);
                mpaclogin.LogoutPacs();
                ExecutedSteps++;

                Thread.Sleep(20000);
                //Step-2: Click on Active transfer in PACS Gateway - Marked NA since POP is installed on client machines
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-3: Verify the transfer status - Marked NA since POP is installed on client machines
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-4: Click on Transger History Tab in Pacs Gateway - Marked NA since POP is installed on client machines
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-5: Verify the study status - Marked NA since POP is installed on client machines
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-6: Verify the study in HP
                login.DriverGoTo(login.hpurl);
                HPLogin hplogin = new HPLogin();
                HPHomePage hphome = hplogin.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");

                //Click Search Archive
                workflow.HPSearchStudy("Accessionno", AccessionNumbers[0]);

                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && (BasePage.BrowserVersion.ToLower().Equals("8") || BasePage.BrowserVersion.ToLower().Equals("9")))
                    BasePage.Driver.FindElement(By.CssSelector("input[name='accessionNumber']")).Clear();
                else
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"input[name='accessionNumber']\").value=''");

                PageLoadWait.WaitForHPPageLoad(20);

                //Validate Study is present in Holding Pen 
                if (workflow.HPCheckStudy(AccessionNumbers[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study is not uploaded in Holding Pen");
                }

                //Step-7: Update Config file
                // Change the LicenseUsageLoggingInterval from 60 minutes to 1 minute (refer to Pre-Condition 9)
                //ChangeNodeValue(XMLFilePath, NodePath, NodeValue);
                ChangeAttributeValue(XMLFilePath, NodePath, "associationTimeout", NodeValue);
                ExecutedSteps++;
                //Step-8: Launch PACS Gateway and upload a study
                BasePage.RunBatchFile(Config.batchfilepath, StudyPath[1] + " " + Config.dicomsendpath + " " + Config.StudyPacs);
                //Send the study to dicom devices from MergePacs management page
                login.DriverGoTo(login.mpacstudyurl);
                homepage = mplogin.Loginpacs(Config.pacsadmin, Config.pacspassword);
                tools = (Tool)homepage.NavigateTopMenu("Tools");
                tools.NavigateToSendStudy();
                tools.SearchStudy("Patient Name", PatientNames[1], 0);
                tools.MpacSelectStudy("Accession", AccessionNumbers[1]);
                tools.SendStudy(1);
                mpaclogin.LogoutPacs();
                ExecutedSteps++;

                Thread.Sleep(20000);
                //Step-9: Click on Active transfer in PACS Gateway - Marked NA since POP is installed on client machines
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-10: Verify the transfer status - Marked NA since POP is installed on client machines
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-11: Click on Transger History Tab in Pacs Gateway - Marked NA since POP is installed on client machines
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-12: Verify the study status - Marked NA since POP is installed on client machines
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-13: Verify the study in HP
                login.DriverGoTo(login.hpurl);
                hplogin = new HPLogin();
                hphome = hplogin.LoginHPen(hpUserName, hpPassword);
                workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");

                //Click Search Archive
                workflow.HPSearchStudy("Accessionno", AccessionNumbers[1]);

                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && (BasePage.BrowserVersion.ToLower().Equals("8") || BasePage.BrowserVersion.ToLower().Equals("9")))
                    BasePage.Driver.FindElement(By.CssSelector("input[name='accessionNumber']")).Clear();
                else
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"input[name='accessionNumber']\").value=''");

                PageLoadWait.WaitForHPPageLoad(20);

                //Validate Study is present in Holding Pen 
                if (workflow.HPCheckStudy(AccessionNumbers[1]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Study is not uploaded in Holding Pen");
                }


                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout 
                hplogin.LogoutHPen();

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

                //Return Result
                return result;
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
