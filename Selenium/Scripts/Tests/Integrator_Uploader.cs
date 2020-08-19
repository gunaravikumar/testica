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
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using System.Data;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems;
using TestStack.White.UIItems.TableItems;
using System.Xml;
using Dicom;
using Dicom.Network;
using System.Diagnostics;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Pages.eHR;

namespace Selenium.Scripts.Tests
{
    class Integrator_Uploader
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public HPHomePage hphomepage { get; set; }
        public HPLogin hplogin { get; set; }
        public ServiceTool servicetool { get; set; }
        public BasePage basepage { get; set; }
        public WpfObjects wpfobject { get; set; }
        public EHR ehr { get; set; }
        public BluRingViewer bluringviewer { get; set; }

        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public Integrator_Uploader(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ei = new ExamImporter();
            mpaclogin = new MpacLogin();
            basepage = new BasePage();
            servicetool = new ServiceTool();
            wpfobject = new WpfObjects();
        }

        /// <summary>
        /// Test 161498 - View Images using an EMR link with Holding Pen added as Datasource
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161341(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            Inbounds inbounds = null;
            ehr = new EHR();
            String URL = "http://" + Config.IConnectIP + "/webaccess";
            try
            {
                string stUserName = Config.stUserName;
                string phUserName = Config.ph1UserName;
                string arUserName = Config.ar1UserName;
                string stPassword = Config.stPassword;
                string phPassword = Config.ph1Password;
                string arPassword = Config.ar1Password;
                string destination = Config.Dest1;
                String[] UploadFilePath = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('=');
                String[] Accession = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList")).Split(':');
                String StudyUID = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyUID"));
                //PreConditions:
                basepage.EnableBypass();
                //Upload 3 Studies to HP using EI
                ei.EIDicomUpload(stUserName, stPassword, destination, UploadFilePath[0]);
                //Nominate 2 of them
                login.LoginIConnect(phUserName, phPassword);
                inbounds = login.Navigate<Inbounds>();
                inbounds.SelectAllInboundData();
                inbounds.SearchStudy("Accession", Accession[0]);
                inbounds.SelectStudy("Accession", Accession[0]);
                inbounds.NominateForArchive("Nominate1");
                inbounds.SearchStudy("Accession", Accession[1]);
                inbounds.SelectStudy("Accession", Accession[1]);
                inbounds.NominateForArchive("Nominate1");
                login.Logout();
                //Login as the Archivist and Archive one study [S1]
                login.LoginIConnect(arUserName, arPassword);
                inbounds = login.Navigate<Inbounds>();
                inbounds.SelectAllInboundData();
                inbounds.SearchStudy("lastname", "*");
                inbounds.SelectStudy("Accession", Accession[0]);
                inbounds.ArchiveStudy("Precondition Route", "");
                login.Logout();
                Thread.Sleep(20000); //Wait for routing to complete

                //Integrator Precondtitions
                TestFixtures.UpdateFeatureFixture("bluring", value: "true:BluRing");
                TestFixtures.UpdateFeatureFixture("usersharing", value: "Always enabled", restart: true);

                //Step-1:Make sure the three studies are loaded in the holdingpen and are processed acording to the preconditions. The Study S3 is in the holding pen and has not been nominated or archived. 
                login.LoginIConnect(phUserName, phPassword);
                inbounds = login.Navigate<Inbounds>();
                inbounds.SelectAllInboundData();
                inbounds.SearchStudy("Accession", Accession[0]);
                Dictionary<string, string> study1_1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession[0], "Routing Completed" });
                inbounds.SearchStudy("Accession", Accession[1]);
                Dictionary<string, string> study1_2 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession[1], "Nominated For Archive" });
                inbounds.SearchStudy("Accession", Accession[2]);
                Dictionary<string, string> study1_3 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { Accession[2], "Uploaded" });
                if (study1_1 != null && study1_2 != null && study1_3 != null)
                {
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

                //Step-2: Launch the TestEHR program and change the setting defined below
                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Image Load");
                wpfobject.WaitTillLoad();
                ehr.SetCommonParameters(address: URL);
                ehr.SetSearchKeys_Study(StudyUID, "Study_UID");
                String url_2 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                Logger.Instance.InfoLog("Generated URL- Step-2: " + url_2);
                login.CreateNewSesion();
                ehr.NavigateToIntegratorURL(url_2);
                login.NavigateToIntegratorFrame(viewer: "bluring");
                if (basepage.GetElement(BasePage.SelectorType.CssSelector, EHR.css_ErrorSpan).Text.Contains("No studies that match the search criteria could be found"))
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

                //Step-3: Close the browser and go back to the TestEHR and change to Include Holding Pen = True
                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Image Load");
                wpfobject.WaitTillLoad();
                ehr.SetCommonParameters(address: URL);
                ehr.SetSelectorOptions(IncludeHoldingPen: "True");
                ehr.SetSearchKeys_Study(StudyUID, "Study_UID");
                String url_3 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                Logger.Instance.InfoLog("Generated URL- Step-3: " + url_3);
                login.CreateNewSesion();
                ehr.NavigateToIntegratorURL(url_3);
                bluringviewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "bluring");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                var step3 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step3)
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

                //Step-4: Close the browser and go back to the TestEHR program and change the Include Holding Pen = False
                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Image Load");
                wpfobject.WaitTillLoad();
                ehr.SetCommonParameters(address: URL);
                ehr.SetSearchKeys_Study(StudyUID, "Study_UID");
                String url_4 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                Logger.Instance.InfoLog("Generated URL- Step-4: " + url_4);
                login.CreateNewSesion();
                ehr.NavigateToIntegratorURL(url_4);
                login.NavigateToIntegratorFrame(viewer: "bluring");
                if (basepage.GetElement(BasePage.SelectorType.CssSelector, EHR.css_ErrorSpan).Text.Contains("No studies that match the search criteria could be found"))
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
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
            finally
            {
                basepage.CreateNewSesion();
                login.DriverGoTo(login.url);
            }
        }
    }
}
