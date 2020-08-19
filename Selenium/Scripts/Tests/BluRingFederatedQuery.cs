using System;
using System.Data;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MPAC;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using System.Diagnostics;
using Selenium.Scripts.Pages.eHR;
using Dicom.Network;
using Selenium.Scripts.Pages.MergeServiceTool;
using OpenQA.Selenium;

namespace Selenium.Scripts.Tests
{
    class BluRingFederatedQuery : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public HPHomePage hphomepage { get; set; }
        public HPLogin hplogin { get; set; }
        public EHR ehr { get; set; }
        public ServiceTool servicetool { get; set; }

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public BluRingFederatedQuery(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ei = new ExamImporter();
            mpaclogin = new MpacLogin();
            wpfobject = new WpfObjects();
            ehr = new EHR();
            servicetool = new ServiceTool();
        }

        /// <summary>
        /// 161366 - Federated_Query - FederatedStudy loading
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161366(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            DomainManagement domainmanagement = null;
            Studies studies = new Studies();
            BluRingViewer bluringviewer = new BluRingViewer();
            string DicomPath = string.Empty;
            string[] PatientID = null;
            string[] Accession = null;
            string[] Expected = null;
            string[] Actual = null;
            int resultcount = 0;
            try
            {
                PatientID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split('=');
                Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Accession")).Split('=');
                DicomPath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath"));
                string[] DS1Accession = Accession[0].Split(':');
                string[] DS2Accession = Accession[1].Split(':');
                //PreCondition
                var client = new DicomClient();
                for (int i = 1; i <= 6; i++)
                {
                    client.AddRequest(new DicomCStoreRequest(string.Concat(Config.TestDataPath + DicomPath, "\\DS1\\662420", i)));
                    client.Send(Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);
                }
                for (int i = 1; i <= 6; i++)
                {
                    client.AddRequest(new DicomCStoreRequest(string.Concat(Config.TestDataPath + DicomPath, "\\DS2\\662420", i)));
                    client.Send(Config.DestinationPACS, 104, false, "SCU", Config.DestinationPACSAETitle);
                }
                //Step-1: Login to iConnect Access as Administrator and ensure all 2 remote servers are enabled for query
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.EditDomainButton().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                domainmanagement.ConnectAllDataSources();
                domainmanagement.ClickChooseColumns();
                domainmanagement.SelectColumns(new string[] { "Accession" });
                domainmanagement.OKButton_ChooseColumns().Click();
                domainmanagement.ClickSaveEditDomain();
                ExecutedSteps++;

                //Step 2: Go to studylist
                studies = (Studies)login.Navigate("Studies");
                if (login.IsTabSelected("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 3: Check the following studies from main server to ensure they are federated from both server
                resultcount = 0;
                studies.RDM_MouseHover(Config.rdm1);
                studies.RDM_MouseHover(Config.rdm2);
                studies.SearchStudy(patientID: PatientID[0], DatasourceList: new string[] { Config.rdm1 + "." + login.GetHostName(Config.DestEAsIp), Config.rdm2 + "." + login.GetHostName(Config.DestinationPACS) });
                studies.ChooseColumns(new string[] { "Accession" });
                Actual = BasePage.GetColumnValues("Accession");
                Expected = DS1Accession[0].Split(',').Concat(DS2Accession[0].Split(',')).ToArray();
                if (Expected.All(sid => Actual.Contains(sid)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Accession's Listed from All Datasources for ALM ANNA");
                }
                studies.RDM_MouseHover(Config.rdm1);
                studies.RDM_MouseHover(Config.rdm2);
                studies.SearchStudy(patientID: PatientID[1], DatasourceList: new string[] { Config.rdm1 + "." + login.GetHostName(Config.DestEAsIp), Config.rdm2 + "." + login.GetHostName(Config.DestinationPACS) });
                studies.ChooseColumns(new string[] { "Accession" });
                Actual = BasePage.GetColumnValues("Accession");
                Expected = DS1Accession[1].Split(',').Concat(DS2Accession[1].Split(',')).ToArray();
                if (Expected.All(sid => Actual.Contains(sid)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Accession's Listed from All Datasources for GAMAGE, MARY");
                }
                studies.RDM_MouseHover(Config.rdm1);
                studies.RDM_MouseHover(Config.rdm2);
                studies.SearchStudy(patientID: PatientID[2], DatasourceList: new string[] { Config.rdm1 + "." + login.GetHostName(Config.DestEAsIp), Config.rdm2 + "." + login.GetHostName(Config.DestinationPACS) });
                studies.ChooseColumns(new string[] { "Accession" });
                Actual = BasePage.GetColumnValues("Accession");
                Expected = DS1Accession[2].Split(',').Concat(DS2Accession[2].Split(',')).ToArray();
                if (Expected.All(sid => Actual.Contains(sid)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Accession's Listed from All Datasources for Chest, Chester");
                }
                if (resultcount == 3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 4: Login iConnect Access using remote server 1 to ensure the following studies are federated from different datasources are correct:
                login.Logout();
                login.DriverGoTo("http://" + Config.RDMIP + "/Webaccess");
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.EditDomainButton().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                domainmanagement.ConnectAllDataSources();
                domainmanagement.ClickChooseColumns();
                domainmanagement.SelectColumns(new string[] { "Accession" });
                domainmanagement.OKButton_ChooseColumns().Click();
                domainmanagement.ClickSaveEditDomain();
                studies = (Studies)login.Navigate("Studies");
                resultcount = 0;
                studies.SearchStudy(patientID: PatientID[0], Datasource: login.GetHostName(Config.DestEAsIp));
                studies.ChooseColumns(new string[] { "Accession" });
                Actual = BasePage.GetColumnValues("Accession");
                Expected = DS1Accession[0].Split(',');
                if (Expected.All(sid => Actual.Contains(sid)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Accession's Listed from All Datasources for ALM ANNA");
                }
                studies.SearchStudy(patientID: PatientID[1], Datasource: login.GetHostName(Config.DestEAsIp));
                studies.ChooseColumns(new string[] { "Accession" });
                Actual = BasePage.GetColumnValues("Accession");
                Expected = DS1Accession[1].Split(',');
                if (Expected.All(sid => Actual.Contains(sid)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Accession's Listed from All Datasources for GAMAGE, MARY");
                }
                studies.SearchStudy(patientID: PatientID[2], Datasource: login.GetHostName(Config.DestEAsIp));
                studies.ChooseColumns(new string[] { "Accession" });
                Actual = BasePage.GetColumnValues("Accession");
                Expected = DS1Accession[2].Split(',');
                if (Expected.All(sid => Actual.Contains(sid)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Accession's Listed from All Datasources for Chest, Chester");
                }
                if (resultcount == 3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 5: Load study GAMAGE, MARY and switch between priors
                studies.SearchStudy(patientID: PatientID[1], Datasource: login.GetHostName(Config.DestEAsIp));
                studies.ChooseColumns(new string[] { "Accession" });
                Expected = DS1Accession[1].Split(',');
                studies.SelectStudy("Accession", Expected[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                bluringviewer.OpenPriors(accession: Expected[1]);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 2, viewport: 1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (bluringviewer.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[1]))
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
                //Step 6: Login iConnect Access using remote server 2 to ensure the following studies are federated from different datasources are correct:
                login.Logout();
                login.DriverGoTo("http://" + Config.RDMIP2 + "/Webaccess");
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.EditDomainButton().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                domainmanagement.ConnectAllDataSources();
                domainmanagement.ClickChooseColumns();
                domainmanagement.SelectColumns(new string[] { "Accession" });
                domainmanagement.OKButton_ChooseColumns().Click();
                domainmanagement.ClickSaveEditDomain();
                studies = (Studies)login.Navigate("Studies");
                resultcount = 0;
                studies.SearchStudy(patientID: PatientID[0], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.ChooseColumns(new string[] { "Accession" });
                Actual = BasePage.GetColumnValues("Accession");
                Expected = DS2Accession[0].Split(',');
                if (Expected.All(sid => Actual.Contains(sid)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Accession's Listed from All Datasources for ALM ANNA");
                }
                studies.SearchStudy(patientID: PatientID[1], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.ChooseColumns(new string[] { "Accession" });
                Actual = BasePage.GetColumnValues("Accession");
                Expected = DS2Accession[1].Split(',');
                if (Expected.All(sid => Actual.Contains(sid)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Accession's Listed from All Datasources for GAMAGE, MARY");
                }
                studies.SearchStudy(patientID: PatientID[2], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.ChooseColumns(new string[] { "Accession" });
                Actual = BasePage.GetColumnValues("Accession");
                Expected = DS2Accession[2].Split(',');
                if (Expected.All(sid => Actual.Contains(sid)))
                {
                    resultcount++;
                    Logger.Instance.InfoLog("Accession's Listed from All Datasources for Chest, Chester");
                }
                if (resultcount == 3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7: Load study GAMAGE, MARY and switch between priors
                studies.SearchStudy(patientID: PatientID[1], Datasource: login.GetHostName(Config.DestinationPACS));
                studies.ChooseColumns(new string[] { "Accession" });
                Expected = DS2Accession[1].Split(',');
                studies.SelectStudy("Accession", Expected[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                bluringviewer.OpenPriors(accession: Expected[1]);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 2, viewport: 1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (bluringviewer.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[1]))
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
                //Step 8: Logout and back on from iConnect Access via main server
                login.Logout();
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                if (login.IsTabSelected("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9: Load study ALM, ANNA
                studies.RDM_MouseHover(Config.rdm1);
                studies.RDM_MouseHover(Config.rdm2);
                studies.SearchStudy(patientID: PatientID[0], DatasourceList: new string[] { Config.rdm1 + "." + login.GetHostName(Config.DestEAsIp), Config.rdm2 + "." + login.GetHostName(Config.DestinationPACS) });
                Expected = DS1Accession[0].Split(',').Concat(DS2Accession[0].Split(',')).ToArray();
                studies.SelectStudy("Accession", Expected[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                ExecutedSteps++;

                //Step 10: Select prior study from the selector
                //Step 11: Load the prior study
                bluringviewer.OpenPriors(accession: Expected[3]);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 2, viewport: 1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (bluringviewer.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[1]))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 12: Load study GAMAGE, MARY
                bluringviewer.CloseBluRingViewer();
                Expected = DS1Accession[1].Split(',').Concat(DS2Accession[1].Split(',')).ToArray();
                studies.RDM_MouseHover(Config.rdm1);
                studies.RDM_MouseHover(Config.rdm2);
                studies.SearchStudy(patientID: PatientID[1], DatasourceList: new string[] { Config.rdm1 + "." + login.GetHostName(Config.DestEAsIp), Config.rdm2 + "." + login.GetHostName(Config.DestinationPACS) });
                studies.ChooseColumns(new string[] { "Accession" });
                studies.SelectStudy("Accession", Expected[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                ExecutedSteps++;

                //Step 13: Select prior study from the selector
                //Step 14: Load the prior study
                bluringviewer.OpenPriors(accession: Expected[3]);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 2, viewport: 1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (bluringviewer.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[1]))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 15: Load the study Chest, Chester
                bluringviewer.CloseBluRingViewer();
                Expected = DS1Accession[2].Split(',').Concat(DS2Accession[2].Split(',')).ToArray();
                studies.RDM_MouseHover(Config.rdm1);
                studies.RDM_MouseHover(Config.rdm2);
                studies.SearchStudy(patientID: PatientID[2], DatasourceList: new string[] { Config.rdm1 + "." + login.GetHostName(Config.DestEAsIp), Config.rdm2 + "." + login.GetHostName(Config.DestinationPACS) });
                studies.ChooseColumns(new string[] { "Accession" });
                studies.SelectStudy("Accession", Expected[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                ExecutedSteps++;

                //Step 16: Select prior study from the selector
                //Step 17: Load the prior study
                bluringviewer.OpenPriors(accession: Expected[3]);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 2, viewport: 1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (bluringviewer.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[1]))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 18: Create a user that can access to all data sources in domain without any filter role defined
                bluringviewer.CloseBluRingViewer();
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                domainmanagement.CreateDomain(createDomain[DomainManagement.DomainAttr.DomainName], createDomain[DomainManagement.DomainAttr.RoleName], datasources: new string[] { Config.rdm1, Config.rdm2 });
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                domainmanagement.ModalityDropDown().SelectByText("CR");
                domainmanagement.LayoutDropDown().SelectByText("1x2");
                domainmanagement.ClickSaveDomain();
                if (domainmanagement.IsDomainExist(createDomain[DomainManagement.DomainAttr.DomainName]))
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

                //Step 19: Login as newly created user and go to studylist
                login.Logout();
                login.LoginIConnect(createDomain[DomainManagement.DomainAttr.DomainName].Replace(" ", "_"), createDomain[DomainManagement.DomainAttr.DomainName]);
                studies = (Studies)login.Navigate("Studies");
                if (login.IsTabSelected("Studies"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 20: Load the study that exists from all data sources
                studies.RDM_MouseHover(Config.rdm1);
                studies.RDM_MouseHover(Config.rdm2);
                studies.SearchStudy(patientID: PatientID[1], DatasourceList: new string[] { Config.rdm1 + "." + login.GetHostName(Config.DestEAsIp), Config.rdm2 + "." + login.GetHostName(Config.DestinationPACS) });
                studies.SelectStudy("Patient ID", PatientID[1]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                ExecutedSteps++;

                //Step 21: Load the study from individual data source
                bluringviewer.CloseBluRingViewer();
                studies.RDM_MouseHover(Config.rdm2);
                studies.SearchStudy(patientID: PatientID[2], Datasource: Config.rdm2 + "." + login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Patient ID", PatientID[2]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                ExecutedSteps++;
                //Step 23:
                ExecutedSteps++;
                //Step 24:
                ExecutedSteps++;
                //Step 25:
                bluringviewer.CloseBluRingViewer();
                studies.SearchStudy(patientID: PatientID[0], Datasource: Config.rdm2 + "." + login.GetHostName(Config.DestinationPACS));
                studies.SelectStudy("Patient ID", PatientID[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_allViewportes)).Count() == 2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

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
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                login.DriverGoTo(login.url);
            }
        }

        /// <summary>
        /// 161368 - 4.0 Saving Report
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161368(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            Studies studies = new Studies(); ;
            BluRingViewer bluringviewer = new BluRingViewer();
            DomainManagement domainmanagement = new DomainManagement();
            string Accession = string.Empty;
            string PatientID = string.Empty;
            string DicomPath = string.Empty;
            string DS2 = string.Empty;
            string DS2AETitle = string.Empty;
            int DS2Port = 0;
            try
            {
                Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Accession");
                PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                DicomPath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath"));
                //PreCondition
                DS2 = Config.DestinationPACS;
                DS2AETitle = Config.DestinationPACSAETitle;
                DS2Port = 104;
                var client = new DicomClient();
                client.AddRequest(new DicomCStoreRequest(string.Concat(Config.TestDataPath + DicomPath, "\\DS1\\66245")));
                client.Send(DS2, DS2Port, false, "SCU", DS2AETitle);
                //Step 1: Send an HL7 Report for Cardiac, MR to DS2
                client.AddRequest(new DicomCStoreRequest(string.Concat(Config.TestDataPath + DicomPath, "\\DS2\\66245SR")));
                client.Send(DS2, DS2Port, false, "SCU", DS2AETitle);
                ExecutedSteps++;
                //Step 2: Load the study Cardiac, MR and check the report
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.EditDomainButton().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                domainmanagement.SetCheckBoxInEditDomain("reportview", 0);
                domainmanagement.ClickSaveEditDomain();
                studies = (Studies)login.Navigate("Studies");
                studies.RDM_MouseHover(Config.rdm1);
                studies.RDM_MouseHover(Config.rdm2);
                studies.SearchStudy(AccessionNo: Accession, DatasourceList: new string[] { Config.rdm1 + "." + login.GetHostName(Config.DestEAsIp), Config.rdm2 + "." + login.GetHostName(Config.DestinationPACS) });
                studies.SelectStudy("Accession", Accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                bluringviewer.OpenReport_BR(0, "SR");
                var report_data1 = bluringviewer.FetchReportData_BR(0);
                if (report_data1 != null && string.Equals(report_data1["MRN:"], PatientID))
                {
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
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
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
                //Close browser
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                //Return Result
                return result;
            }
        }

        /// <summary>
        /// 161370 - 8.0 Study List
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161370(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            BluRingViewer bluringviewer = new BluRingViewer();
            DomainManagement domainmanagement = new DomainManagement();
            string[] PatientName = null;
            string[] FilePath = null;
            string[] PatientID = null;
            string[] StudyDate = null;
            string[] StudyID = null;
            string[] columnvalue = null;
            string url = string.Empty;
            string[] FullPath = null;
            Studies studies = null;
            bool PIDExists = false;
            bool StudyDateExists = false;
            string[] name = null;
            string DS1 = string.Empty;
            string DS1AETitle = string.Empty;
            string DS2 = string.Empty;
            string DS2AETitle = string.Empty;
            string DS3 = string.Empty;
            string DS3AETitle = string.Empty;
            int DS1Port = 0;
            int DS2Port = 0;
            int DS3Port = 0;
            string DS3SeriesUID = string.Empty;
            try
            {
                PatientName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName")).Split('=');
                FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('=');
                PatientID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split('=');
                StudyDate = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate")).Split('=');
                StudyID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID")).Split('=');
                DS1 = Config.DestEAsIp;
                DS1AETitle = Config.DestEAsAETitle;
                DS1Port = 12000;
                DS2 = Config.DestinationPACS2;
                DS2AETitle = Config.DestinationPACS2AETitle;
                DS2Port = 104;
                DS3 = Config.DestinationPACS;
                DS3AETitle = Config.DestinationPACSAETitle;
                DS3Port = 104;
                var client = new DicomClient();
                FullPath = Directory.GetFiles(Config.TestDataPath + FilePath[0], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }
                FullPath = Directory.GetFiles(Config.TestDataPath + FilePath[1], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS2, DS2Port, false, "SCU", DS2AETitle);
                }
                FullPath = Directory.GetFiles(Config.TestDataPath + FilePath[2], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS3, DS3Port, false, "SCU", DS3AETitle);
                }
                //BasePage.RunBatchFile(Config.batchfilepath, Config.TestDataPath + FilePath[1] + " " + Config.dicomsendpath + " " + DS2);
                //BasePage.RunBatchFile(Config.batchfilepath, Config.TestDataPath + FilePath[2] + " " + Config.dicomsendpath + " " + DS3);
                //FullPath = Directory.GetFiles(Config.TestDataPath + FilePath[2], "*.*", SearchOption.AllDirectories);
                DS3SeriesUID = "2.16.840.1.113662.4.4162725393.937919156.244211897988156469";

                //Step 1: From the Study list, clear all search fields, set the Study Performed to All Dates, and Data Source to All. Click Search.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: "*", DatasourceList: new string[] { login.GetHostName(DS1), login.GetHostName(DS2), login.GetHostName(DS3) });
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                Dictionary<int, string[]> Result = BasePage.GetSearchResults();
                if (Result.Count > 0)
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
                //Step 2: Enter Nuclear, Gladys in the First and Last name fields and click Search.
                name = PatientName[0].Split(',');
                studies.SearchStudy(LastName: name[0], FirstName: name[1], DatasourceList: new string[] { login.GetHostName(DS1), login.GetHostName(DS2), login.GetHostName(DS3) });
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                Result = BasePage.GetSearchResults();
                columnvalue = BasePage.GetColumnValues("Patient ID");
                PIDExists = columnvalue.All(pid => string.Equals(pid, PatientID[0]));
                columnvalue = BasePage.GetColumnValues("Study Date");
                StudyDateExists = columnvalue.All(sd => sd.Contains(StudyDate[0]));
                if (PIDExists && StudyDateExists)
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
                //Step 3: Enter RAM A/L POHUMAL in the Patient Name field and click Search.
                name = PatientName[1].Split(',');
                studies.SearchStudy(LastName: name[0], FirstName: name[1], DatasourceList: new string[] { login.GetHostName(DS1), login.GetHostName(DS2), login.GetHostName(DS3) });
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                Result = BasePage.GetSearchResults();
                columnvalue = BasePage.GetColumnValues("Patient ID");
                PIDExists = columnvalue.All(pid => string.Equals(pid, PatientID[1]));
                columnvalue = BasePage.GetColumnValues("Study Date");
                StudyDateExists = columnvalue.All(sd => sd.Contains(StudyDate[1]));
                if (Result.Count == 1 && PIDExists && StudyDateExists)
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
                //Step 4: Enter Oesophagus, Barium in the Patient Name field and click Search.
                name = PatientName[2].Split(',');
                studies.SearchStudy(LastName: name[0], FirstName: name[1], DatasourceList: new string[] { login.GetHostName(DS1), login.GetHostName(DS2), login.GetHostName(DS3) });
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                Result = BasePage.GetSearchResults();
                columnvalue = BasePage.GetColumnValues("Patient ID");
                PIDExists = columnvalue.All(pid => string.Equals(pid, PatientID[2]));
                columnvalue = BasePage.GetColumnValues("Study Date");
                StudyDateExists = columnvalue.All(sd => sd.Contains(StudyDate[2]));
                if (Result.Count == 1 && PIDExists && StudyDateExists)
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
                //Step 5: Clear the Patient Name field, enter 200106120952 in the Study ID field and click Search.
                studies.SearchStudy(studyID: StudyID[0], DatasourceList: new string[] { login.GetHostName(DS1), login.GetHostName(DS2), login.GetHostName(DS3) });
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                Result = BasePage.GetSearchResults();
                columnvalue = BasePage.GetColumnValues("Patient ID");
                PIDExists = columnvalue.All(pid => string.Equals(pid, PatientID[1]));
                columnvalue = BasePage.GetColumnValues("Study Date");
                StudyDateExists = columnvalue.All(sd => sd.Contains(StudyDate[1]));
                if (Result.Count == 1 && PIDExists && StudyDateExists)
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
                //Step 6: Enter 4380 in the Study ID field and click Search.
                studies.SearchStudy(studyID: StudyID[1], DatasourceList: new string[] { login.GetHostName(DS1), login.GetHostName(DS2), login.GetHostName(DS3) });
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                Result = BasePage.GetSearchResults();
                columnvalue = BasePage.GetColumnValues("Patient ID");
                PIDExists = columnvalue.All(pid => string.Equals(pid, PatientID[3]));
                columnvalue = BasePage.GetColumnValues("Study Date");
                StudyDateExists = columnvalue.All(sd => sd.Contains(StudyDate[3]));
                if (Result.Count == 1 && PIDExists && StudyDateExists)
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
                //Step 7: Enter 3001 in the Study ID field and click Search.
                studies.SearchStudy(studyID: StudyID[2], DatasourceList: new string[] { login.GetHostName(DS1), login.GetHostName(DS2), login.GetHostName(DS3) });
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                Result = BasePage.GetSearchResults();
                columnvalue = BasePage.GetColumnValues("Patient ID");
                PIDExists = columnvalue.All(pid => string.Equals(pid, PatientID[4]));
                columnvalue = BasePage.GetColumnValues("Study Date");
                StudyDateExists = columnvalue.All(sd => sd.Contains(StudyDate[4]));
                if (Result.Count == 1 && PIDExists && StudyDateExists)
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
                //Step 8: Clear the Study ID field and set the Data Source to . Click Search.
                studies.SearchStudy(Datasource: "All");
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                name = PatientName[3].Split(',');
                studies.SearchStudy(LastName: name[0], FirstName: name[1], DatasourceList: new string[] { login.GetHostName(DS1) });
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                studies.ChooseColumns(new string[] { "Data Source" });
                columnvalue = BasePage.GetColumnValues("Data Source");
                if (columnvalue.All(ds => ds.Contains(login.GetHostName(DS1))))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9: Set the Data Source to . Click Search.
                studies.SearchStudy(Datasource: "All");
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                name = PatientName[3].Split(',');
                studies.SearchStudy(LastName: name[0], FirstName: name[1], DatasourceList: new string[] { login.GetHostName(DS2) });
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                studies.ChooseColumns(new string[] { "Data Source" });
                columnvalue = BasePage.GetColumnValues("Data Source");
                if (columnvalue.All(ds => ds.Contains(login.GetHostName(DS2))))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 10: Set the Data Source to . Click Search.
                studies.SearchStudy(Datasource: "All");
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                name = PatientName[3].Split(',');
                studies.SearchStudy(LastName: name[0], FirstName: name[1], DatasourceList: new string[] { login.GetHostName(DS3) });
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                studies.ChooseColumns(new string[] { "Data Source" });
                columnvalue = BasePage.GetColumnValues("Data Source");
                if (columnvalue.All(ds => ds.Contains(login.GetHostName(DS3))))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 11: Load the study Cardiac MR
                name = PatientName[3].Split(',');
                studies.SearchStudy(LastName: name[0], FirstName: name[1], DatasourceList: new string[] { login.GetHostName(DS3) });
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                studies.ChooseColumns(new string[] { "Last Name" });
                studies.SelectStudy("Last Name", name[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (bluringviewer.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[0]))
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
                //Step 12: Clear any search criteria and click Search to list all patients.
                bluringviewer.CloseBluRingViewer();
                studies.SearchStudy(Datasource: "All");
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                Result = BasePage.GetSearchResults();
                if (Result.Count > 0)
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
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
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
                //Close browser
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                //Return Result
                return result;
            }
        }

        /// <summary>
        /// 161371 - 9.0 Loading
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161371(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            BluRingViewer bluringviewer = new BluRingViewer();
            DomainManagement domainmanagement = new DomainManagement();
            IList<IWebElement> thumbnails = null;
            string[] PatientName = null;
            string[] FilePath = null;
            string[] PatientID = null;
            string[] StudyDate = null;
            string[] StudyID = null;
            IList<IWebElement> reportlist = null;
            string[] columnvalue = null;
            string url = string.Empty;
            string[] FullPath = null;
            Studies studies = null;
            string[] name = null;
            string[] Accession = null;
            string[] Modality = null;
            string DS1 = string.Empty;
            string DS1AETitle = string.Empty;
            string DS2 = string.Empty;
            string DS2AETitle = string.Empty;
            string DS3 = string.Empty;
            string DS3AETitle = string.Empty;
            string desc1 = string.Empty;
            string desc2 = string.Empty;
            int DS1Port = 0;
            int DS2Port = 0;
            int DS3Port = 0;
            try
            {
                PatientName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName")).Split('=');
                FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('=');
                PatientID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split('=');
                StudyDate = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate")).Split('=');
                StudyID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID")).Split('=');
                Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Accession")).Split('=');
                Modality = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Modality")).Split('=');
                DS1 = Config.DestEAsIp;
                DS1AETitle = Config.DestEAsAETitle;
                DS1Port = 12000;
                DS2 = Config.DestinationPACS2;
                DS2AETitle = Config.DestinationPACS2AETitle;
                DS2Port = 104;
                DS3 = Config.DestinationPACS;
                DS3AETitle = Config.DestinationPACSAETitle;
                DS3Port = 104;
                var client = new DicomClient();
                for (int i = 0; i <= 1; i++)
                {
                    FullPath = Directory.GetFiles(Config.TestDataPath + FilePath[i], "*.*", SearchOption.AllDirectories);
                    foreach (string DicomPath in FullPath)
                    {
                        client.AddRequest(new DicomCStoreRequest(DicomPath));
                        client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                    }
                }

                for (int i = 2; i <= 3; i++)
                {
                    FullPath = Directory.GetFiles(Config.TestDataPath + FilePath[i], "*.*", SearchOption.AllDirectories);
                    foreach (string DicomPath in FullPath)
                    {
                        client.AddRequest(new DicomCStoreRequest(DicomPath));
                        client.Send(DS2, DS2Port, false, "SCU", DS2AETitle);
                    }
                }
                for (int i = 4; i <= 5; i++)
                {
                    FullPath = Directory.GetFiles(Config.TestDataPath + FilePath[i], "*.*", SearchOption.AllDirectories);
                    foreach (string DicomPath in FullPath)
                    {
                        client.AddRequest(new DicomCStoreRequest(DicomPath));
                        client.Send(DS3, DS3Port, false, "SCU", DS3AETitle);
                    }
                }

                //Step 1: Log in to iConnect Access as the test user created in the pre-conditions.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                domainmanagement.CreateDomain(createDomain[DomainManagement.DomainAttr.DomainName], createDomain[DomainManagement.DomainAttr.RoleName], datasources: new string[] { login.GetHostName(DS1), login.GetHostName(DS2), login.GetHostName(DS3) });
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                domainmanagement.VisibleAllStudySearchField();
                domainmanagement.SetCheckBoxInEditDomain("reportview", 0);
                domainmanagement.SetViewerTypeInNewDomain();
                foreach (string modality in Modality)
                {
                    domainmanagement.ModalityDropDown().SelectByText(modality);
                    domainmanagement.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                }
                domainmanagement.ClickSaveDomain();
                login.Logout();
                login.LoginIConnect(createDomain[DomainManagement.DomainAttr.DomainName].Replace(" ", "_"), createDomain[DomainManagement.DomainAttr.DomainName]);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(Datasource: "All");
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                result.steps[++ExecutedSteps].status = "Pass";
                //Step 2: Load the study Nuclear Gladys, 1-2 Phase Bone.
                name = PatientName[0].Split(',');
                studies.SearchStudy(LastName: name[0], FirstName: name[1], DatasourceList: new string[] { login.GetHostName(DS1), login.GetHostName(DS2), login.GetHostName(DS3) });
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SelectStudy("Study ID", StudyID[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                if (thumbnails.Count == 1)
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
                //Step 3: Observe the Exam List.
                IList<string> ACCPriors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors)).Select(pr => bluringviewer.GetAccession(pr.FindElement(By.CssSelector(BluRingViewer.AccessionNumberInExamList)))).ToArray();
                if (Accession.All(acc => ACCPriors.Contains(acc)))
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
                //Step 4: Set the view to 4 series, select the study 3. Phase Bone and load both series into a second Viewer window.
                bluringviewer.OpenPriors(accession: Accession[0]);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 2, viewport: 1);
                BluRingViewer.WaitforThumbnails();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (bluringviewer.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[1]))
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

                //Step 5: Select the study Ganzk&#-3;rper and load the series into a second Viewer window.
                bluringviewer.CloseBluRingViewer();
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SelectStudy("Study ID", StudyID[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                //viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                bluringviewer.OpenPriors(accession: Accession[1]);
                PageLoadWait.WaitForBluRingViewportToLoad(panel: 2, viewport: 1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                if (bluringviewer.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[1]))
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
                //Step 6: Load the study RAM A/L POHUMAL, 200106120952.
                bluringviewer.CloseBluRingViewer();
                name = PatientName[1].Split(',');
                studies.SearchStudy(LastName: name[0], FirstName: name[1], DatasourceList: new string[] { login.GetHostName(DS1), login.GetHostName(DS2), login.GetHostName(DS3) });
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                studies.ChooseColumns(new string[] { "Last Name" });
                studies.SelectStudy("Last Name", name[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                if (thumbnails.Count == 1)
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
                //Step 7: Load the study Chest CT, 22127099722281.
                bluringviewer.CloseBluRingViewer();
                name = PatientName[2].Split(',');
                studies.RDM_MouseHover(Config.rdm1);
                studies.RDM_MouseHover(Config.rdm2);
                studies.SearchStudy(LastName: name[0], FirstName: name[1], DatasourceList: new string[] { login.GetHostName(DS1), login.GetHostName(DS2), login.GetHostName(DS3) });
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                studies.ChooseColumns(new string[] { "Last Name" });
                studies.SelectStudy("Last Name", name[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                if (thumbnails.Count == 2)
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
                //Step 8: Load the study Cardiac MR, 4380.
                bluringviewer.CloseBluRingViewer();
                name = PatientName[3].Split(',');
                studies.SearchStudy(LastName: name[0], FirstName: name[1], DatasourceList: new string[] { login.GetHostName(DS1), login.GetHostName(DS2), login.GetHostName(DS3) });
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                studies.ChooseColumns(new string[] { "Last Name" });
                studies.SelectStudy("Last Name", name[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                if (thumbnails.Count == 4)
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
                //Step 9: Load the study Rhoden Kirchen.
                bluringviewer.CloseBluRingViewer();
                name = PatientName[4].Split(',');
                studies.SearchStudy(LastName: name[0], FirstName: name[1], DatasourceList: new string[] { login.GetHostName(DS1), login.GetHostName(DS2), login.GetHostName(DS3) });
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                studies.ChooseColumns(new string[] { "Last Name" });
                studies.SelectStudy("Last Name", name[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                if (thumbnails.Count == 1)
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
                //Step 10: Load the study Oesophagus Barium.
                bluringviewer.CloseBluRingViewer();
                name = PatientName[5].Split(',');
                studies.SearchStudy(LastName: name[0], FirstName: name[1], DatasourceList: new string[] { login.GetHostName(DS1), login.GetHostName(DS2), login.GetHostName(DS3) });
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                studies.ChooseColumns(new string[] { "Last Name" });
                studies.SelectStudy("Last Name", name[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                if (thumbnails.Count == 3)
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
                //Step 11: Load the study LUND TAMMY J, A1035510.
                bluringviewer.CloseBluRingViewer();
                name = PatientName[6].Split(',');
                studies.SearchStudy(LastName: name[0], FirstName: name[1], DatasourceList: new string[] { login.GetHostName(DS1), login.GetHostName(DS2), login.GetHostName(DS3) });
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                studies.ChooseColumns(new string[] { "Last Name" });
                studies.SelectStudy("Last Name", name[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                if (thumbnails.Count == 4)
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

                //Step 12: "Load the study SpecialChars~!@#$%, &*()_+-->{}[]|/?'"";<>,.3001."
                bluringviewer.CloseBluRingViewer();
                studies.SearchStudy(studyID: StudyID[3], DatasourceList: new string[] { login.GetHostName(DS1), login.GetHostName(DS2), login.GetHostName(DS3) });
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SelectStudy("Study ID", StudyID[3]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                if (thumbnails.Count == 1)
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
                //Step 13: Load the study Abdomen CT, 24813.
                bluringviewer.CloseBluRingViewer();
                name = PatientName[7].Split(',');
                studies.SearchStudy(LastName: name[0], FirstName: name[1], DatasourceList: new string[] { login.GetHostName(DS1), login.GetHostName(DS2), login.GetHostName(DS3) });
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                studies.ChooseColumns(new string[] { "Last Name" });
                studies.SelectStudy("Last Name", name[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                if (thumbnails.Count == 2)
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
                //Step 14: Click on the Report bar to expand the Report section.

                bluringviewer.OpenReport_BR(0, "SR");
                var report_data1 = bluringviewer.FetchReportData_BR(0);
                desc1 = report_data1["Description"];
                if (report_data1 != null && !string.IsNullOrWhiteSpace(desc1))
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

                //Step 15: Verify the number of Reports displayed.

                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                reportlist = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.ReportTabList_div));
                if (reportlist.Count == 2)
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

                //Step 16: Try to view each report.

                bluringviewer.SelectReport_BR(0, 1);
                var report_data2 = bluringviewer.FetchReportData_BR(0);
                desc2 = report_data2["Description"];
                if (!string.IsNullOrWhiteSpace(desc2) && desc1 != desc2)
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

                //Step 17: Load the study 23811.Anon29, 2431
                bluringviewer.CloseBluRingViewer();
                name = PatientName[8].Split(',');
                studies.SearchStudy(LastName: name[0], FirstName: name[1], DatasourceList: new string[] { login.GetHostName(DS1), login.GetHostName(DS2), login.GetHostName(DS3) });
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                studies.ChooseColumns(new string[] { "Last Name" });
                studies.SelectStudy("Last Name", name[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                if (thumbnails.Count == 1)
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

                //Step 18: Load the study Portable Betty, 65842
                bluringviewer.CloseBluRingViewer();
                name = PatientName[9].Split(',');
                studies.SearchStudy(LastName: name[0], FirstName: name[1], DatasourceList: new string[] { login.GetHostName(DS1), login.GetHostName(DS2), login.GetHostName(DS3) });
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                studies.ChooseColumns(new string[] { "Last Name" });
                studies.SelectStudy("Last Name", name[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                if (thumbnails.Count == 1)
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
                bluringviewer.CloseBluRingViewer();
                login.Logout();
                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
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
                //Close browser
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                //Return Result
                return result;
            }
            finally
            {
                /*servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                servicetool.EnableMergeEMPI();
                servicetool.CloseServiceTool();*/
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
