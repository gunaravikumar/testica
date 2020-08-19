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
using Selenium.Scripts.Pages.MergeServiceTool;

namespace Selenium.Scripts.Tests
{
    class WebUploader
    {
        public Login login { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public HPLogin hplogin { get; set; }
        public Configure configure { get; set; }
        public HPHomePage hphomepage { get; set; }
        public ExamImporter ei { get; set; }
        public string filepath { get; set; }
        public DomainManagement domain { get; set; }

        public WebUploader(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            domain = new DomainManagement();
            //mpaclogin = new MpacLogin();
            //hplogin = new HPLogin();
            //configure = new Configure();
            //hphomepage = new HPHomePage();
            //ei = new ExamImporter();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        /// <summary>
        /// Web Uploader initial setup
        /// </summary>
        public TestCaseResult Test_27545(String testid, String teststeps, int stepcount)
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
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                //
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

                
                //step 1
                login.LoginIConnect(username, password);
                login.Navigate("DomainManagement");
                domain.SelectDomain(DomainName);
                domain.ClickEditDomain();
                if (!domain.CheckJavaExamImporter())
                {
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
                //step 2
                login.LoginIConnect("ph", "ph");
                if (!domain.CheckJavaExamImporterUserPreferences())
                {
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

                //step 3
                login.DriverGoTo(login.url);

                //Send  mutiple HL7 orders - step 4
                ExecutedSteps++;
                

                //Step-5
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

                //**************Reset the QuerID tag changed*********************
                login.DriverGoTo(login.hpurl);
                hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                configure = (Configure)hphomepage.Navigate("Configure");

                //Navigate to Properties browser tab
                configure.NavigateToTab("properties");

                //Update QueryID tag with default tag
                configure.UpdateQueryIDTags("removealladd", DefaultTag);

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

    }
}
