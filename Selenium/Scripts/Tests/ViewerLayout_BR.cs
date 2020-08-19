using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Reusable.Generic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using Selenium.Scripts.Pages.MPAC;
using System.Collections;

namespace Selenium.Scripts.Tests
{
    class ViewerLayout_BR : BasePage
    {
        public Login login { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public String eiWinName { get; set; }
        public ServiceTool servicetool { get; set; }
        public WpfObjects wpfobject { get; set; }
        public Studies studies { get; set; }
        public BasePage basepage { get; set; }
        public BluRingViewer bluringviewer { get; set; }
        public string filepath { get; set; }
        public string EA_91 = "VMSSA-5-38-91";
        public string EA_131 = "VMSSA-4-38-131";
        public string PACS_A7 = "PA-A7-WS8";
        public string EA_96 = "VMSSA-5-38-96";

        DomainManagement domainmanagement;
        RoleManagement rolemanagement;


        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        public ViewerLayout_BR(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            mpaclogin = new MpacLogin();
            wpfobject = new WpfObjects();
            basepage = new BasePage();
            servicetool = new ServiceTool();
            bluringviewer = new BluRingViewer();
        }

        /// <summary> 
        /// This Test Case is Verification of Viewer Layout Scope "Modality Default layout setting"
        /// </summary>
        public TestCaseResult Test_164678(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables            
            TestCaseResult result = new TestCaseResult(stepcount);
            RoleManagement rolemanagement = null;
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String DescriptionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
            String[] Description = DescriptionList.Split(':');
            String modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
            String count = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "count");
            var viewportcount = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "ViewportCount")).Split(':');

            //List of Accession Modality and its Thumbnail count
            String AccessionNo_Mod_Count = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Accession_Mod_Count");
            String[] AccessionNo_Mod_Count_List = AccessionNo_Mod_Count.Split(':');
            IList<String> Accession1 = new List<String>();
            IList<String> Modality1 = new List<String>();
            IList<String> Count1 = new List<String>();
            foreach (String s in AccessionNo_Mod_Count_List)
            {
                String[] List = s.Split('=');
                Accession1.Add(List[0]);
                Modality1.Add(List[1]);
                Count1.Add(List[2]);
            }

            //List of PatientID Modality and its Thumbnail count
            String PatientID_Mod_Count = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Patient_Mod_Count");
            String[] PatientID_Mod_Count_List = PatientID_Mod_Count.Split(':');
            IList<String> PatientID2 = new List<String>();
            IList<String> Modality2 = new List<String>();
            IList<String> Count2 = new List<String>();
            foreach (String s in PatientID_Mod_Count_List)
            {
                String[] List = s.Split('=');
                PatientID2.Add(List[0]);
                Modality2.Add(List[1]);
                Count2.Add(List[2]);
            }

            try
            {
                //Step-1                
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Step-2 -- Launcing CR studies - Mode - Auto                
                //Case-1 -- Sinle series -- 1X1
                bool[] statusflag = new bool[5];
                Studies study = (Studies)login.Navigate("Studies");
                study.SearchStudy(AccessionNo: Accession1[0], Modality: Modality1[0], Datasource: EA_131);
                study.SelectStudy("Accession", Accession1[0]);
                var brviewer = BluRingViewer.LaunchBluRingViewer();
                if ((BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == Int32.Parse(Count1[0])) && (brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[0])))
                {
                    statusflag[0] = true;
                }
                result.steps[++ExecutedSteps].SetPath(testid + "_2_1_1x1", ExecutedSteps + 1);
                bool status2_1 = brviewer.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                brviewer.CloseBluRingViewer();

                //Case-2 - Study with 2 series -- 1X2
                study.SearchStudy(AccessionNo: Accession1[1], Modality: Modality1[1], Datasource: EA_131);
                study.SelectStudy("Accession", Accession1[1]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                if ((BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == Int32.Parse(Count1[1])) && ((brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[1]))))
                {
                    statusflag[1] = true;
                }
                result.steps[ExecutedSteps].SetPath(testid + "_2_2_1x2", ExecutedSteps + 1);
                bool status2_2 = brviewer.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                brviewer.CloseBluRingViewer();

                //Case-3 Study with 3 series -- 1X3
                study.SearchStudy(AccessionNo: Accession1[2], Modality: Modality1[2], Datasource: EA_131);
                study.SelectStudy("Accession", Accession1[2]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                if ((BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == Int32.Parse(Count1[2])) && ((brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[2]))))
                {
                    statusflag[2] = true;
                }
                result.steps[ExecutedSteps].SetPath(testid + "_2_2_1x3", ExecutedSteps + 1);
                bool status2_3 = brviewer.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                brviewer.CloseBluRingViewer();

                //Case-4 - Study with 4 series -- 2X2
                study.SearchStudy(Description: Description[0], Modality: Modality2[0], Datasource: EA_131);
                study.SelectStudy("Description", Description[0]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                if ((BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == 4) &&
                    (brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[3])))
                {
                    statusflag[3] = true;
                }
                result.steps[ExecutedSteps].SetPath(testid + "_2_4_2x2", ExecutedSteps + 1);
                bool status2_4 = brviewer.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                brviewer.CloseBluRingViewer();

                //Case-5 - Study with 5 series -- 2X3
                study.SearchStudy(AccessionNo: Accession1[3], Modality: Modality1[3], Datasource: EA_131);
                study.SelectStudy("Accession", Accession1[3]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                if ((BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == Int32.Parse(Count1[3])) && (brviewer.GetViewPortCount(1) - 1 == Int32.Parse(viewportcount[4])))
                {
                    statusflag[4] = true;
                }
                result.steps[ExecutedSteps].SetPath(testid + "_2_5_2x3", ExecutedSteps + 1);
                bool status2_5 = brviewer.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                brviewer.CloseBluRingViewer();
                if (!statusflag.Contains(false) && status2_1 && status2_2 && status2_3 && status2_4 && status2_5)
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

                //Step-3 -- Change to PNG Format
                PageLoadWait.WaitForPageLoad(20);
                var userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.SelectRadioBtn("ImageFormatRadioButtonList", "PNG (lossless)");
                PageLoadWait.WaitForPageLoad(10);
                userpref.CloseUserPreferences();
                ExecutedSteps++;

                //Setp-4 - CT Study - 2X2
                study.SearchStudy(patientID: PatientID2[1], Modality: Modality2[1], Datasource: EA_91);
                study.SelectStudy("Patient ID", PatientID2[1]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status4 = study.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                if (status4 && BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == Int32.Parse(Count2[1]) &&
                    brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[5]))
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
                brviewer.CloseBluRingViewer();

                //Step-5 --  DR Modality - Auto                
                study.SearchStudy(AccessionNo: Accession1[4], Modality: Modality1[4], Datasource: EA_91);
                study.SelectStudy("Accession", Accession1[4]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                bool flag5 = false;
                if (BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == Int32.Parse(Count1[4]) && brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[6]))
                {
                    flag5 = true;
                }
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status5 = study.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                if (flag5 && status5)
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
                brviewer.CloseBluRingViewer();

                //Step-6 -- DX Modality - Auto                 
                study.SearchStudy(AccessionNo: Accession1[5], Modality: Modality1[5], Datasource: EA_131);
                study.SelectStudy("Accession", Accession1[5]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                bool flag6 = false;
                if (BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == Int32.Parse(Count1[5]) && brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[7]))
                {
                    flag6 = true;
                }
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status6 = brviewer.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                if (flag6 && status6)
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
                brviewer.CloseBluRingViewer();

                //Step-7 -- MG Modality - 1X2
                study.SearchStudy(patientID: PatientID2[2], Modality: Modality2[2], Datasource: EA_131);
                study.SelectStudy("Patient ID", PatientID2[2]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status7 = brviewer.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                if (status7 && BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == 8 &&
                    brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[8]))
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
                brviewer.CloseBluRingViewer();

                //Step-8 -- Study MR Modality - 2X2                
                study.SearchStudy(patientID: PatientID2[3], Modality: Modality2[3], Datasource: EA_91);
                study.SelectStudy("Patient ID", PatientID2[3]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status8 = study.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                if (status8 && BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == Int32.Parse(Count2[3]) &&
                    brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[9]))
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
                brviewer.CloseBluRingViewer();


                //Step-9 - NM Modality - 2X2                
                study.SearchStudy(AccessionNo: Accession1[6], Modality: Modality1[6], Datasource: EA_91);
                study.SelectStudy("Accession", Accession1[6]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status9 = study.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                if (status9 && BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == Int32.Parse(Count1[6]) &&
                    brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[10]))
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
                brviewer.CloseBluRingViewer();

                //Step-10 -- OT Modality - 1X2                
                study.SearchStudy(patientID: PatientID2[4], Modality: Modality2[4], Datasource: EA_131);
                study.SelectStudy("Patient ID", PatientID2[4]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status10 = study.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                if (status10 && BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == Int32.Parse(Count2[4]) &&
                    brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[11]))
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
                brviewer.CloseBluRingViewer();

                //Step-11 - PT Modality - 2X2
                study.SearchStudy(AccessionNo: Accession1[7], Modality: Modality1[7], Datasource: EA_91);
                study.SelectStudy("Accession", Accession1[7]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status11 = study.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                if (status11 && BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == Int32.Parse(Count1[7]) &&
                    brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[12]))
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
                brviewer.CloseBluRingViewer();

                //Step-12 -RF Modality - 2X2            
                study.SearchStudy(AccessionNo: Accession1[8], Modality: Modality1[8], Datasource: EA_131);
                study.SelectStudy("Accession", Accession1[8]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status12 = study.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                if (status12 && BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == Int32.Parse(Count1[8]) &&
                    brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[13]))
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
                brviewer.CloseBluRingViewer();

                //Step-13 - RG Modality - Auto               
                study.SearchStudy(patientID: PatientID2[5], Modality: Modality2[5], Datasource: EA_91);
                study.SelectStudy("Patient ID", PatientID2[5]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                bool flag13 = false;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status13 = study.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                if (status13 && BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == Int32.Parse(Count2[5]) &&
                    brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[14]))
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
                brviewer.CloseBluRingViewer();

                //Step-14 - SC Modality -- 1X2
                study.SearchStudy(patientID: PatientID2[6], Modality: Modality2[6], Datasource: EA_91);
                study.SelectStudy("Patient ID", PatientID2[6]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status14 = study.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                if (status14 && BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == Int32.Parse(Count2[6]) &&
                    brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[15]))
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
                brviewer.CloseBluRingViewer();

                //Step-15 - Load various SC-mf Colour studies.
                study.SearchStudy(patientID: PatientID2[7], Modality: Modality2[7], Datasource: EA_131);
                study.SelectStudy("Patient ID", PatientID2[7]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status15 = study.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                if (status15 && BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == Int32.Parse(Count2[7]) &&
                    brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[16]))
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
                brviewer.CloseBluRingViewer();

                //Step-16 - Load US studies Single and Multiframe - Auto        
                study.SearchStudy(patientID: PatientID2[8], Modality: Modality2[8], Datasource: EA_131);
                study.SelectStudy("Patient ID", PatientID2[8]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                bool flag16_1 = false;
                result.steps[++ExecutedSteps].SetPath(testid + "_16_1_SingleFrame", ExecutedSteps + 1);
                bool status16_1 = study.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                if (status16_1 && BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == Int32.Parse(Count2[8]) &&
                    brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[17]))
                {
                    flag16_1 = true;
                }
                brviewer.CloseBluRingViewer();

                //Case-2 - Multi frame - Auto
                study.SearchStudy(patientID: PatientID2[9], Modality: Modality2[9], Datasource: EA_131);
                study.SelectStudy("Patient ID", PatientID2[9]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                bool flag16_2 = false;
                result.steps[ExecutedSteps].SetPath(testid + "_16_2_MultiFrame", ExecutedSteps + 1);
                bool status16_2 = study.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                if (status16_2 && BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == Int32.Parse(Count2[9]) &&
                brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[18]))
                {
                    flag16_2 = true;
                }
                brviewer.CloseBluRingViewer();
                if (flag16_1 && flag16_2)
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

                //Step-17 -  VL Study -2X2
                study.SearchStudy(patientID:"60162", Modality: modality, Datasource: EA_131);
                study.SelectStudy("Patient ID", "60162");
                brviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status17 = study.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                if (status17 && BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == Int32.Parse(count) &&
                    brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[19]))
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
                brviewer.CloseBluRingViewer();

                //Step-18 - XA Modality - Auto                
                study.SearchStudy(AccessionNo: Accession1[9], Modality: Modality1[9], Datasource: EA_91);
                study.SelectStudy("Accession", Accession1[9]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                bool flag18 = false;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status18 = study.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                if (BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == Int32.Parse(Count1[9]) && brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[20]))
                    flag18 = true;
                if (flag18 && status18)
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
                brviewer.CloseBluRingViewer();

                //Step-19 - ES - Auto
                study.SearchStudy(patientID: PatientID2[10], Modality: Modality2[10], Datasource: EA_131);
                study.SelectStudy("Patient ID", PatientID2[10]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                bool flag19 = false;
                if (BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == 1
                    && brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[21]))
                    flag19 = true;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status19 = study.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                if (status19 && flag19)
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
                brviewer.CloseBluRingViewer();

                //Step-20 - IO Study - Auto                
                study.SearchStudy(patientID: PatientID2[11], Modality: Modality2[11], Datasource: EA_91);
                study.SelectStudy("Patient ID", PatientID2[11]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                bool flag20 = false;
                if (BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == Int32.Parse(Count2[11]) && brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[22]))
                    flag20 = true;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status20 = study.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                if (status20 && flag20)
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
                brviewer.CloseBluRingViewer();

                //Step-21 - Load data with KO (Key Objects) series. -- /The series of MR, KO and PR modality should be
                //loaded and displayed into the viewer.
                study.SearchStudy(AccessionNo: Accession1[10], Datasource: EA_131);
                study.SelectStudy("Accession", Accession1[10]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status21 = study.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                if (status21 && brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[23]))
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
                brviewer.CloseBluRingViewer();

                //Step-22 -- RT Study                
                study.SearchStudy(patientID: PatientID2[12], Modality: Modality2[12], Datasource: EA_91);
                study.SelectStudy("Patient ID", PatientID2[12]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status22 = brviewer.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                Logger.Instance.InfoLog("Step-22--" + brviewer.GetViewPortCount(1) + "--" + Int32.Parse(viewportcount[24]));
                if (status22 && brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[24]))
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
                brviewer.CloseBluRingViewer();

                //Step-23 - OP Study                
                study.SearchStudy(patientID: PatientID2[13], Modality: Modality2[13], Datasource: EA_131);
                study.SelectStudy("Patient ID", PatientID2[13]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status23 = study.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel(), pixelTolerance:5);
                Logger.Instance.InfoLog("Step-23--" + brviewer.GetViewPortCount(1) + "--" + Int32.Parse(viewportcount[25]));
                Logger.Instance.InfoLog("Step-23--" + BluRingViewer.NumberOfThumbnailsInStudyPanel(1) + "--" + Int32.Parse(Count2[13]));
                if (status23 && brviewer.GetViewPortCount(1) == 6 &&
                    BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == Int32.Parse(Count2[13]))
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
                brviewer.CloseBluRingViewer();

                //Step-24 - Study with Audio series - 2X2                
                study.SearchStudy(AccessionNo: Accession1[11], Datasource: EA_131);
                study.SelectStudy("Accession", Accession1[11]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status24 = study.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                Logger.Instance.InfoLog("Step-24--" + brviewer.GetViewPortCount(1) + "--" + Int32.Parse(viewportcount[26]));
                Logger.Instance.InfoLog("Step-24--" + BluRingViewer.NumberOfThumbnailsInStudyPanel(1) + "--" + Int32.Parse(Count1[11]));
                if (status24 && brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[26]) &&
                    BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == Int32.Parse(Count1[11]))
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

                //Step-25 (Audio file not working in FF browser in windows server 2008)
                BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.AvailableReports)).Click();
                BasePage.Driver.SwitchTo().Frame(Driver.FindElement(By.CssSelector("iframe#reportIframe")));
                Boolean IsAudioPlaying = (Boolean)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function AudioStatus(){var audio = document.querySelector('#Audio_Display_Div>audio');return audio.ended;}return AudioStatus();");
                if(!IsAudioPlaying)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                brviewer.CloseBluRingViewer();

                //Step-26-- Update CR - 1x2 in Role Management 
                //Made as Not Automated since this steps needs to be run only in fresh build
                DataBaseUtil db = new DataBaseUtil("sqlserver");
                db.ConnectSQLServerDB();
                db.ReadTable("delete from PythonUserPref where UserID = 'Administrator'");
                ServiceTool st = new ServiceTool();
                st.RestartIISUsingexe();
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                rolemanagement = login.Navigate<RoleManagement>();
                rolemanagement.SelectRole("SuperRole");
                rolemanagement.EditRoleBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                bool rolesel = rolemanagement.UseDomainSetting().Selected;
                if (rolesel)
                    rolemanagement.UseDomainSetting().Click();
                rolemanagement.ModalityDropDown().SelectByText("CR");
                rolemanagement.LayoutDropDown().SelectByText("1x2");
                rolemanagement.ClickSaveEditRole();
                ExecutedSteps++;
                //result.steps[++ExecutedSteps].status = "Not Automted";

                //Step-27
                login.Navigate("Studies");
                study.SearchStudy(AccessionNo: Accession1[3], Modality: Modality1[3], Datasource: EA_131);
                study.SelectStudy("Accession", Accession1[3]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                int viewportnumber = brviewer.GetViewPortCount(1);               
                if(viewportnumber == 2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                brviewer.CloseBluRingViewer();
                // result.steps[++ExecutedSteps].status = "Not Automted";


                //Step-28 - Update User preference - 2X2 Layout
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                userpref = login.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.ModalityDropDown().SelectByText("CR");
                PageLoadWait.WaitForPageLoad(10);
                userpref.LayoutDropDown().SelectByText("2x2");
                PageLoadWait.WaitForFrameLoad(10);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;


                //Step-29 - CR Modality - 2X2 Layout
                study.SearchStudy(AccessionNo: Accession1[12], Modality: Modality1[12], Datasource: EA_131);
                study.SelectStudy("Accession", Accession1[12]);
                brviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status27 = study.CompareImage(result.steps[ExecutedSteps], brviewer.studyPanel());
                if (status27 && brviewer.GetViewPortCount(1) == Int32.Parse(viewportcount[27]) &&
                    BluRingViewer.NumberOfThumbnailsInStudyPanel(1) == 7)
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

                //Step-30
                brviewer.CloseBluRingViewer();
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
            finally
            {
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                login.Navigate("Studies");
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.SelectRadioBtn("ImageFormatRadioButtonList", "JPEG (lossy)");
                PageLoadWait.WaitForPageLoad(20);
                userpref.ModalityDropDown().SelectByText("CR");
                PageLoadWait.WaitForPageLoad(10);
                userpref.LayoutDropDown().SelectByText("auto");
                PageLoadWait.WaitForFrameLoad(10);
                userpref.CloseUserPreferences();
                login.Logout();
            }
        }

        /// <summary> 
        /// Viewer Layout - Change layout for study panel
        /// </summary>
        public TestCaseResult Test_161690(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables            
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String DescriptionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
            String[] Description = DescriptionList.Split(':');
            String modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
            String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
            String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
            String StudyDate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate");
            String StudyTime = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyTime");
            String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo");
            String[] PatientID = PatientIDList.Split(':');

            try
            {
                //Step-1: Login iCA and load a CT modality study that has multiple series with priors .
                login.LoginIConnect(username, password);
                //Search study
                studies = login.Navigate<Studies>();
                studies.SearchStudy(LastName: "*", Description: Description[0]);
                studies.SelectStudy("Patient ID", PatientID[5]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad(viewport: 4);
                //1- Counting viewports
                bool step1_1 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 4;
                IWebElement LayoutIcon = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutIcon), WaitTypes.Visible);
                LayoutIcon.Click();
                //2 - Getting 
                string step1_2 = GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutGridWrapper + ">div").Text;
                LayoutIcon.Click();
				Thread.Sleep(2000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step1_3 = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer());
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step1_4 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 2, 1);
                if (step1_1 && step1_2 == "2x2" && step1_3 && step1_4)
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

                //Step-2: In Study panel toolbar, Hover the mouse pointer over the layout selection tool
                string step2 = GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_ToolbarLayoutWrapper).GetAttribute("title");
                if (step2 == "Layout")
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

                //Step-3 & 4: Click on 'Layout selection tool' & Select 2x1 layout from layout selection grid.
                bluringviewer.ChangeViewerLayout("2x1", viewport: 2);
                ExecutedSteps++;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step3_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer());
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step3_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 2, 1);
                bool step3_3 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 2;
                if (step3_1 && step3_2 && step3_3)
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

                //Step-5: Select 2x2 layout from selection grid .
                bluringviewer.ChangeViewerLayout("2x2", viewport: 4);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step4_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer());
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step4_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 2, 1);
                bool step4_3 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 4;
                if (step4_1 && step4_2 && step4_3)
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

                //Step-6: Drag and drop (series 2,5,3,7) from thumbnail in 2x2 layout .
                IList<IWebElement> ThumbnailList = bluringviewer.ThumbnailIndicator(0);
                int ThumbnailCount = ThumbnailList.Count;
                // Drag the 1st thumbnail to the first viewport
                //GetElement(SelectorType.CssSelector, bluringviewer.SetViewPort(1, 1)).Click();
                bluringviewer.DropAndDropThumbnails(thumbnailnumber: 3, viewport: 1, studyPanelNumber: 1, UseDragDrop: true);
                bluringviewer.DropAndDropThumbnails(thumbnailnumber: 6, viewport: 2, studyPanelNumber: 1, UseDragDrop: true);
                bluringviewer.DropAndDropThumbnails(thumbnailnumber: 4, viewport: 3, studyPanelNumber: 1, UseDragDrop: true);
                bluringviewer.DropAndDropThumbnails(thumbnailnumber: 8, viewport: 4, studyPanelNumber: 1, UseDragDrop: true);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step5_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer());
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step5_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 2, 1);
                if (step5_1 && step5_2)
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

                //Step-7: Select 2x4 layout from selection grid.
                bluringviewer.ChangeViewerLayout("2x4", viewport: 8);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step6_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer());
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step6_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 2, 1);
                bool step6_3 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 8;
                if (step6_1 && step6_2 && step6_3)
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

                //Step-8: From Exam list load related prior study with series (0,1,2,3,4,5,6,7,8,9)
                bluringviewer.OpenPriors(StudyDate: StudyDate, StudyTime: StudyTime);
                //bluringviewer.OpenPriors(2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step7_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step7_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.viewportContainer), 2, 1);
                if (step7_1 && step7_2)
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

                //bluringviewer.NavigateToIntegratorFrame("bluring");

                //Step-9: Select 2x3 layout from selection grid.
                bluringviewer.ChangeViewerLayout("2x3", 2, 2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step8_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step8_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.viewportContainer), 2, 1);
                bool step8_3 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(2) + BluRingViewer.div_ViewportPanels)).Count == 6;
                if (step8_1 && step8_2 && step8_3)
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

                //Step-10: Drag the( 1&2 series) thumbnail in the primary(P1) study panel into the (3rd& 4th) view port of the secondary study panel which is in 2x3 layout .

                //GetElement(SelectorType.CssSelector, bluringviewer.SetViewPort(1, 1)).Click();
                bluringviewer.DropAndDropForeignThumbnails(1, 2, 2, 3);
                bluringviewer.DropAndDropForeignThumbnails(1, 3, 2, 4);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step9_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step9_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.viewportContainer), 2, 1);
                if (step9_1 && step9_2)
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


                //Step-11: From secondary study panel change the 4x4 layout and verify changed layout for foreign series in secondary study panel.
                bluringviewer.ChangeViewerLayout("4x4", 2, 9);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step10_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step10_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.viewportContainer), 2, 1);
                bool step10_3 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(2) + BluRingViewer.div_ViewportPanels)).Count == 16;
                if (step10_1 && step10_2 && step10_3)
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

                //Step-12: Drag the( 3&4 series) thumbnail in the primary study panel into the (11& 12th) view port of the secondary study panel which is in 4x4layout .
                //GetElement(SelectorType.CssSelector, bluringviewer.SetViewPort(1, 1)).Click();
                bluringviewer.DropAndDropForeignThumbnails(1, 4, 2, 11);
                bluringviewer.DropAndDropForeignThumbnails(1, 5, 2, 12);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step11_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step11_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.viewportContainer), 2, 1);
                if (step11_1 && step11_2)
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

                //Step-13: From secondary study panel change the 5x5 layout and verify changed layout for foreign series in secondary study panel.
                bluringviewer.ChangeViewerLayout("5x5", 2, 15);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step12_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step12_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.viewportContainer), 2, 1);
                bool step12_3 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(2) + BluRingViewer.div_ViewportPanels)).Count == 25;
                if (step12_1 && step12_2 && step12_3)
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

                //Step-14: From secondary study panel change the 2x1 layout and verify changed layout series in secondary study panel.
                bluringviewer.ChangeViewerLayout("2x1", 2, 2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step13_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step13_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.viewportContainer), 2, 1);
                bool step13_3 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(2) + BluRingViewer.div_ViewportPanels)).Count == 2;
                if (step13_1 && step13_2 && step13_3)
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

                //Step-15: Drag and drop any series from thumbnail in secondary panel and drop in any of the view port.
                //GetElement(SelectorType.CssSelector, bluringviewer.SetViewPort(1, 1)).Click();
                bluringviewer.DropAndDropForeignThumbnails(1, 1, 2, 1);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step14_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step14_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.viewportContainer), 2, 1);
                if (step14_1 && step14_2)
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

                //Step-16: Select any view port and double click on any series from thumbnail.
                GetElement(SelectorType.CssSelector, bluringviewer.SetViewPort(0, 2)).Click();
                ThumbnailList = bluringviewer.ThumbnailIndicator(1);
                IWebElement viewport = ThumbnailList[2];
                DoubleClick(viewport);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step15_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step15_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.viewportContainer), 2, 1);
                if (step15_1 && step15_2)
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

                //Step-17: Close the viewer from studies tab load a MR modality study that has multiple series study..
                bluringviewer.CloseBluRingViewer();
                studies.SearchStudy(patientID: PatientID[0]);
                studies.SelectStudy("Patient ID", PatientID[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad();
                bool step16_1 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 4;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step16_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer());
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step16_3 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 2, 1);
                if (step16_1 && step16_2 && step16_3)
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

                //Step-18: Click on 1x1,1x2,1x3. layout from layout selection grid and verify the active series too.
                //1x1
                bluringviewer.ChangeViewerLayout("1x1", 1, 1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step18_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_thumbnailcontainer), 1);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step18_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.viewportContainer), 2);
                bool step18_3 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 1;
                //1x2
                bluringviewer.ChangeViewerLayout("1x2", 1, 2);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                bool step18_4 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_thumbnailcontainer), 3);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 4);
                bool step18_5 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.viewportContainer), 4);
                bool step18_6 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 2;
                //1x3
                bluringviewer.ChangeViewerLayout("1x3", 1, 3);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 5);
                bool step18_7 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_thumbnailcontainer), 5);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 6);
                bool step18_8 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.viewportContainer), 6);
                bool step18_9 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 3;

                if (step18_1 && step18_2 && step18_3 && step18_4 && step18_5 && step18_6 && step18_7 && step18_8 && step18_9)
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

                //string[] LayoutList = new string[] { "1x1", "1x2", "1x3", "1x4", "1x5", "1x6", "2x1", "2x2", "2x3", "2x4", "2x5", "2x6", "3x1", "3x2", "3x3", "3x4", "3x5", "3x6", "4x1", "4x2", "4x3", "4x4", "4x5", "4x6", "5x1", "5x2", "5x3", "5x4", "5x5", "5x6", "6x1", "6x2", "6x3", "6x4", "6x5", "6x6" };
                //bool[] step17_thumbnail = new bool[LayoutList.Length];
                //bool[] step17_viewer = new bool[LayoutList.Length];
                //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                //int counter = 0;
                //int thumbnailcount = 0;
                //for (int i = 0; i < LayoutList.Length; i++)
                //{
                //    int number = Convert.ToInt32(LayoutList[i].Split('x')[0]) * Convert.ToInt32(LayoutList[i].Split('x')[1]);
                //    if (number >= 8) { thumbnailcount = 8; } else { thumbnailcount = number; }
                //    bluringviewer.ChangeViewerLayout(LayoutList[i], 1, thumbnailcount);
                //    step17_thumbnail[i] = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer(), counter + 1);
                //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, counter + 1);
                //    if (i == LayoutList.Length - 1)
                //        step17_viewer[i] = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), counter + 2, 1);
                //    else
                //        step17_viewer[i] = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), counter + 2);
                //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, counter + 2);
                //    Logger.Instance.InfoLog("Layout " + LayoutList[i] + " Thumbnail Value: " + step17_thumbnail[i]);
                //    Logger.Instance.InfoLog("Layout " + LayoutList[i] + " Viewport Value: " + step17_viewer[i]);
                //    counter = counter + 2;
                //}
                //if (ValidateBoolArray(step17_thumbnail) && ValidateBoolArray(step17_viewer))
                //{
                //    result.steps[ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}


                //Step-19: Close the viewer and from study list load a MG modality study that has multiple series . .
                bluringviewer.CloseBluRingViewer();
                studies.SearchStudy(patientID: PatientID[1]);
                studies.SelectStudy("Patient ID", PatientID[1]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad();
                bool step19_1 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 2;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step19_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer());
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step19_3 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 2, 1);
                if (step19_1 && step19_2 && step19_3)
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

                //Step-20: Click on 3x1,3x2,3x3. layout from layout selection grid and verify the active series too.
                //1x1
                bluringviewer.ChangeViewerLayout("3x1", 1, 3);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step20_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_thumbnailcontainer), 1);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step20_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.viewportContainer), 2);
                bool step20_3 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 3;
                //1x2
                bluringviewer.ChangeViewerLayout("3x2", 1, 4);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                bool step20_4 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_thumbnailcontainer), 3);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 4);
                bool step20_5 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.viewportContainer), 4);
                bool step20_6 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 6;
                //1x3
                bluringviewer.ChangeViewerLayout("3x3", 1, 4);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 5);
                bool step20_7 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_thumbnailcontainer), 5);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 6);
                bool step20_8 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.viewportContainer), 6);
                bool step20_9 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 9;

                if (step20_1 && step20_2 && step20_3 && step20_4 && step20_5 && step20_6 && step20_7 && step20_8 && step20_9)
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
                //bool[] step19_thumbnail = new bool[LayoutList.Length];
                //bool[] step19_viewer = new bool[LayoutList.Length];
                //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                //counter = 0;
                //for (int i = 0; i < LayoutList.Length; i++)
                //{
                //    int number = Convert.ToInt32(LayoutList[i].Split('x')[0]) * Convert.ToInt32(LayoutList[i].Split('x')[1]);
                //    if (number >= 4) { thumbnailcount = 4; } else { thumbnailcount = number; }
                //    bluringviewer.ChangeViewerLayout(LayoutList[i], 1, thumbnailcount);
                //    step19_thumbnail[i] = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer(), counter + 1);
                //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, counter + 1);
                //    if (i == LayoutList.Length - 1)
                //        step19_viewer[i] = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), counter + 2, 1);
                //    else
                //        step19_viewer[i] = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), counter + 2);
                //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, counter + 2);
                //    Logger.Instance.InfoLog("Layout " + LayoutList[i] + " Thumbnail Value: " + step19_thumbnail[i]);
                //    Logger.Instance.InfoLog("Layout " + LayoutList[i] + " Viewport Value: " + step19_viewer[i]);
                //    counter = counter + 2;

                //}
                //if (ValidateBoolArray(step19_thumbnail) && ValidateBoolArray(step19_viewer))
                //{
                //    result.steps[ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                //Step-21: Hover on layout option button and verify layout highlighted in grid.
                LayoutIcon = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutIcon), WaitTypes.Visible);
                LayoutIcon.Click(); //Open dialog
                PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutGridWrapper), WaitTypes.Visible);
                IList<IWebElement> layoutValues = Driver.FindElements(By.CssSelector(BluRingViewer.td_LayoutGridCells));
                String layoutString = layoutValues.Last().GetAttribute("id");
                LayoutIcon.Click(); //Dismiss dialog
                Thread.Sleep(2000);
                if (layoutString.Equals("3x3"))
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

                //Step-22: Close the viewer and from studies tab load a PT modality study that has multiple series .
                bluringviewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: AccessionNo);
                studies.SelectStudy("Accession", AccessionNo);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad();
                bool step21_1 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 4;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step21_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer());
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step21_3 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 2, 1);
                if (step21_1 && step21_2 && step21_3)
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

                //Step-23: Click on 4x1,4x2,4x3. layout from layout selection grid and verify the active series too.
                //1x1
                bluringviewer.ChangeViewerLayout("4x1", 1, 2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step23_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_thumbnailcontainer), 1);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step23_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.viewportContainer), 2);
                bool step23_3 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 4;
                //1x2
                bluringviewer.ChangeViewerLayout("4x2", 1, 2);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                bool step23_4 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_thumbnailcontainer), 3);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 4);
                bool step23_5 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.viewportContainer), 4);
                bool step23_6 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 8;
                //1x3
                bluringviewer.ChangeViewerLayout("4x3", 1, 2);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 5);
                bool step23_7 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_thumbnailcontainer), 5);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 6);
                bool step23_8 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.viewportContainer), 6);
                bool step23_9 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 12;

                if (step23_1 && step23_2 && step23_3 && step23_4 && step23_5 && step23_6 && step23_7 && step23_8 && step23_9)
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
                //bool[] step22_thumbnail = new bool[LayoutList.Length];
                //bool[] step22_viewer = new bool[LayoutList.Length];
                //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                //counter = 0;
                //for (int i = 0; i < LayoutList.Length; i++)
                //{
                //    int number = Convert.ToInt32(LayoutList[i].Split('x')[0]) * Convert.ToInt32(LayoutList[i].Split('x')[1]);
                //    if (number >= 2) { thumbnailcount = 2; } else { thumbnailcount = number; }
                //    bluringviewer.ChangeViewerLayout(LayoutList[i], 1, thumbnailcount);
                //    step22_thumbnail[i] = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer(), counter + 1);
                //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, counter + 1);
                //    if (i == LayoutList.Length - 1)
                //        step22_viewer[i] = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), counter + 2, 1);
                //    else
                //        step22_viewer[i] = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), counter + 2);
                //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, counter + 2);
                //    Logger.Instance.InfoLog("Layout " + LayoutList[i] + " Thumbnail Value: " + step22_thumbnail[i]);
                //    Logger.Instance.InfoLog("Layout " + LayoutList[i] + " Viewport Value: " + step22_viewer[i]);
                //    counter = counter + 2;
                //}
                //if (ValidateBoolArray(step22_thumbnail) && ValidateBoolArray(step22_viewer))
                //{
                //    result.steps[ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                //Step-24: Select any series from 4x1 layout and apply several tool operation (eg:Pan,zoom,w/l).
                bluringviewer.ChangeViewerLayout("4x1", 1, 2);
                bluringviewer.SelectViewerTool(BluRingTools.Window_Level);
                bluringviewer.ApplyTool_WindowWidth();
                bluringviewer.SelectViewerTool(BluRingTools.Invert);
                bluringviewer.SelectViewerTool(BluRingTools.Pan);
                bluringviewer.ApplyTool_Pan();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step24_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer());
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step24_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 2, 1);
                if (step24_1 && step24_2)
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

                //Step-25: Select 4x2 layout from layout selection grid.
                bluringviewer.ChangeViewerLayout("4x2", 1, 2);
                bool step25_1 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 8;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step25_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer());
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step25_3 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 2, 1);
                if (step25_1 && step25_2 && step25_3)
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

                //Step-26: Verify the tool operations for the series are preserved after changing the layout
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step26 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step26)
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

                //Step-27: Close the viewer and from studs tab load a NM modality study that has multiple series . .
                bluringviewer.CloseBluRingViewer();
                studies.SearchStudy(Description: Description[4]);
                studies.SelectStudy("Description", Description[4]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad();
                bool step26_1 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 4;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step26_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer());
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step26_3 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 2, 1);
                if (step26_1 && step26_2 && step26_3)
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

                //Step-28: Click on 5x1,5x2,3x3. layout from layout selection grid and verify the active series too.
                //1x1
                bluringviewer.ChangeViewerLayout("5x1", 1, 3);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step28_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_thumbnailcontainer), 1);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step28_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.viewportContainer), 2);
                bool step28_3 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 5;
                //1x2
                bluringviewer.ChangeViewerLayout("5x2", 1, 3);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                bool step28_4 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_thumbnailcontainer), 3);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 4);
                bool step28_5 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.viewportContainer), 4);
                bool step28_6 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 10;
                //1x3
                bluringviewer.ChangeViewerLayout("5x3", 1, 3);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 5);
                bool step28_7 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_thumbnailcontainer), 5);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 6);
                bool step28_8 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.viewportContainer), 6);
                bool step28_9 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 15;

                if (step28_1 && step28_2 && step28_3 && step28_4 && step28_5 && step28_6 && step28_7 && step28_8 && step28_9)
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
                //bool[] step27_thumbnail = new bool[LayoutList.Length];
                //bool[] step27_viewer = new bool[LayoutList.Length];
                //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                //counter = 0;
                //for (int i = 0; i < LayoutList.Length; i++)
                //{
                //    int number = Convert.ToInt32(LayoutList[i].Split('x')[0]) * Convert.ToInt32(LayoutList[i].Split('x')[1]);
                //    if (number >= 5) { thumbnailcount = 5; } else { thumbnailcount = number; }
                //    bluringviewer.ChangeViewerLayout(LayoutList[i], 1, thumbnailcount);
                //    step27_thumbnail[i] = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer(), counter + 1);
                //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, counter + 1);
                //    if (i == LayoutList.Length - 1)
                //        step27_viewer[i] = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), counter + 2, 1);
                //    else
                //        step27_viewer[i] = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), counter + 2);
                //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, counter + 2);
                //    Logger.Instance.InfoLog("Layout " + LayoutList[i] + " Thumbnail Value: " + step27_thumbnail[i]);
                //    Logger.Instance.InfoLog("Layout " + LayoutList[i] + " Viewport Value: " + step27_viewer[i]);
                //    counter = counter + 2;
                //}
                //if (ValidateBoolArray(step27_thumbnail) && ValidateBoolArray(step27_viewer))
                //{
                //    result.steps[ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                //Step-29: Close the viewer and load a RF modality study that has multiple series ..
                bluringviewer.CloseBluRingViewer();
                studies.SearchStudy(patientID: PatientID[3]);
                studies.SelectStudy("Description", Description[1]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad();
                bool step29_1 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 4;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step29_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer());
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step29_3 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 2, 1);
                if (step29_1 && step29_2 && step29_3)
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

                //Step-30: Click on 6x1,6x2,6x3. layout from layout selection grid and verify the active series .
                //1x1
                bluringviewer.ChangeViewerLayout("6x1", 1, 6);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step30_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_thumbnailcontainer), 1);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step30_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.viewportContainer), 2);
                bool step30_3 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 6;
                //1x2
                bluringviewer.ChangeViewerLayout("6x2", 1, 8);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                bool step30_4 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_thumbnailcontainer), 3);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 4);
                bool step30_5 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.viewportContainer), 4);
                bool step30_6 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 12;
                //1x3
                bluringviewer.ChangeViewerLayout("6x3", 1, 8);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 5);
                bool step30_7 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_thumbnailcontainer), 5);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 6);
                bool step30_8 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.viewportContainer), 6);
                bool step30_9 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 18;

                if (step30_1 && step30_2 && step30_3 && step30_4 && step30_5 && step30_6 && step30_7 && step30_8 && step30_9)
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
                //bool[] step29_thumbnail = new bool[LayoutList.Length];
                //bool[] step29_viewer = new bool[LayoutList.Length];
                //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                //counter = 0;
                //for (int i = 0; i < LayoutList.Length; i++)
                //{
                //    int number = Convert.ToInt32(LayoutList[i].Split('x')[0]) * Convert.ToInt32(LayoutList[i].Split('x')[1]);
                //    if (number >= 8) { thumbnailcount = 8; } else { thumbnailcount = number; }
                //    bluringviewer.ChangeViewerLayout(LayoutList[i], 1, thumbnailcount);
                //    step29_thumbnail[i] = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer(), counter + 1);
                //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, counter + 1);
                //    if (i == LayoutList.Length - 1)
                //        step29_viewer[i] = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), counter + 2, 1);
                //    else
                //        step29_viewer[i] = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), counter + 2);
                //    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, counter + 2);
                //    Logger.Instance.InfoLog("Layout " + LayoutList[i] + " Thumbnail Value: " + step29_thumbnail[i]);
                //    Logger.Instance.InfoLog("Layout " + LayoutList[i] + " Viewport Value: " + step29_viewer[i]);
                //    counter = counter + 2;
                //}
                //if (ValidateBoolArray(step29_thumbnail) && ValidateBoolArray(step29_viewer))
                //{
                //    result.steps[ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                //Step-31: Close the viewer and Load a CR modality study that has multiple series .
                bluringviewer.CloseBluRingViewer();
                studies.SearchStudy(patientID: PatientID[4]);
                studies.SelectStudy("Description", Description[2]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step31_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer());
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step31_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer(), 2, 1);
                if (step31_1 && step31_2)
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

                //Close the viewer
                bluringviewer.CloseBluRingViewer();

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
                
            }
        }

        /// <summary> 
        /// Viewer Layout - Change layout for Multiple Panels
        /// </summary>
        public TestCaseResult Test_161691(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables            
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String DescriptionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
            String[] Description = DescriptionList.Split(':');
            String modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
            String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
            String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
            String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String StudyDateList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate");
            String StudyTimeList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyTime");
            String AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo");
            String[] PatientID = PatientIDList.Split(':');
            String[] StudyDate = StudyDateList.Split(':');
            String[] StudyTime = StudyTimeList.Split('=');

            try
            {
                //Step-1: Login to ICA as Administrator.
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Step-2: In Studies tab,Search and select the study which has series with priors.
                studies = login.Navigate<Studies>();
                studies.SearchStudy(LastName: "*", Description: Description[0]);
                studies.SelectStudy("Patient ID", PatientID[5]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad(viewport: 4);
                IWebElement LayoutIcon = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutIcon), WaitTypes.Visible);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step2)
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

                //Step-3: Verify the study loaded into the modality default layout
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step3_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                bool step3_2 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 4;
                if (step3_1 && step3_2)
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

                //Step-4: In Study panel toolbar, Hover the mouse pointer over the layout selection tool
                string step4 = GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_ToolbarLayoutWrapper).GetAttribute("title");
                if (step4 == "Layout")
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

                //Step-5: Click on 'Layout selection tool'
                LayoutIcon.Click();
                IWebElement Grid = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutGridWrapper), WaitTypes.Visible);
                if (Grid.Displayed)
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
                LayoutIcon.Click();
                Thread.Sleep(2000);

                //Step-6: Choose any layout from the matrix ranging from 1x1 to 6x6
                bluringviewer.ChangeViewerLayout("2x1", viewport: 2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step5_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                bool step5_2 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 2;
                if (step5_1 && step5_2)
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

                //Step-7: Select 2x3 layout from series layout selection grid.
                bluringviewer.ChangeViewerLayout("2x3", viewport: 6);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step6_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                bool step6_2 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 6;
                if (step6_1 && step6_2)
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

                //Step-8: From Exam list load related prior study .
                bluringviewer.OpenPriors(StudyDate: StudyDate[0], StudyTime: StudyTime[0]);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step7_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step7_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.viewportContainer), 2, 1);
                if (step7_1 && step7_2)
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

                //Step-9: Drag the 2nd series from thumbnail of the primary study panel into the 1 st view port of the secondary study panel .
                IList<IWebElement> ThumbnailList = bluringviewer.ThumbnailIndicator(0);
                int ThumbnailCount = ThumbnailList.Count;
                bluringviewer.DropAndDropForeignThumbnails(1, 2, 2, 1);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step8_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step8_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.viewportContainer), 2, 1);
                if (step8_1 && step8_2)
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

                //Step-10: Select a study layout button and change the layout to 3x3 for secondary study panel.
                bluringviewer.ChangeViewerLayout("3x3", 2, 4);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step9_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step9_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.viewportContainer), 2, 1);
                bool step9_3 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(2) + BluRingViewer.div_ViewportPanels)).Count == 9;
                if (step9_1 && step9_2 && step9_3)
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

                //Step-11: In order to load study in an empty series viewer (if any) or replace the series that is currently displayed within another, by these two methods and check:
                //1. select the destination series viewer, then double click on the thumbnail for the series you want to display for particular study panel.
                GetElement(SelectorType.CssSelector, bluringviewer.SetViewPort(4, 2)).Click();
                ThumbnailList = bluringviewer.ThumbnailIndicator(1);
                IWebElement thumbnail = ThumbnailList[2];
                DoubleClick(thumbnail);
                //2. drag and drop the thumbnail for the series you want to display into the desired series viewer for the particular study panel.
                bluringviewer.DropAndDropThumbnails(1, 6, 2, UseDragDrop: true);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step10_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step10_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.viewportContainer), 2, 1);
                if (step10_1 && step10_2)
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

                //Step-12: Select any series in the study panel and apply tool actions.
                GetElement(SelectorType.CssSelector, bluringviewer.SetViewPort(0, 2)).Click();
                bluringviewer.SelectViewerTool(BluRingTools.Window_Level, 2);
                bluringviewer.ApplyTool_WindowWidth();
                bluringviewer.SelectViewerTool(BluRingTools.Invert, 2);
                bluringviewer.SelectViewerTool(BluRingTools.Pan, 2);
                bluringviewer.ApplyTool_Pan();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step11_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step11_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.viewportContainer), 2, 1);
                if (step11_1 && step11_2)
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

                //Step-13: Load any priors from the exam list.
                bluringviewer.OpenPriors(StudyDate: StudyDate[1], StudyTime: StudyTime[1]);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step12_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(3) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step12_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(3) + BluRingViewer.viewportContainer), 2, 1);
                if (step12_1 && step12_2)
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

                //Step-14: Drag the 2nd series from thumbnail of the secondary study panel into the 1 st view port of the third study panel .
                bluringviewer.DropAndDropForeignThumbnails(2, 2, 3, 1);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step13_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(3) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step13_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(3) + BluRingViewer.viewportContainer), 2, 1);
                if (step13_1 && step13_2)
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

                //Step-15: Select a study layout button and change the layout to 4x4 for third study panel.
                bluringviewer.ChangeViewerLayout("4x4", 3, 9);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step14_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(3) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step14_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(3) + BluRingViewer.viewportContainer), 2, 1);
                bool step14_3 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(3) + BluRingViewer.div_ViewportPanels)).Count == 16;
                if (step14_1 && step14_2 && step14_3)
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

                //Step-16: In order to load study in an empty series viewer (if any) or replace the series that is currently displayed within another, by these two methods and check:
                //1. select the destination series viewer, then double click on the thumbnail for the series you want to display for particular study panel.
                GetElement(SelectorType.CssSelector, bluringviewer.SetViewPort(9, 3)).Click();
                ThumbnailList = bluringviewer.ThumbnailIndicator(2);
                thumbnail = ThumbnailList[2];
                DoubleClick(thumbnail);
                //2. drag and drop the thumbnail for the series you want to display into the desired series viewer for the particular study panel.
                bluringviewer.DropAndDropThumbnails(1, 11, 3, UseDragDrop: true);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step15_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(3) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step15_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(3) + BluRingViewer.viewportContainer), 2, 1);
                if (step15_1 && step15_2)
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

                //Step-17: Select any series in the study panel and apply tool actions.
                GetElement(SelectorType.CssSelector, bluringviewer.SetViewPort(0, 3)).Click();
                bluringviewer.SelectViewerTool(BluRingTools.Window_Level, 3);
                bluringviewer.ApplyTool_WindowWidth();
                bluringviewer.SelectViewerTool(BluRingTools.Invert, 3);
                bluringviewer.SelectViewerTool(BluRingTools.Pan, 3);
                bluringviewer.ApplyTool_Pan();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step16_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(3) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step16_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(3) + BluRingViewer.viewportContainer), 2, 1);
                if (step16_1 && step16_2)
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

                //Step-18: Load any priors from the exam list.
                bluringviewer.OpenPriors(StudyDate: StudyDate[2], StudyTime: StudyTime[2]);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step17_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(4) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step17_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(4) + BluRingViewer.viewportContainer), 2, 1);
                if (step17_1 && step17_2)
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

                //Step-19:Drag the any series from thumbnail of the third study panel into the 1 st view port of the fourth study panel .
                bluringviewer.DropAndDropForeignThumbnails(3, 2, 4, 1);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step18_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(4) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step18_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(4) + BluRingViewer.viewportContainer), 2, 1);
                if (step18_1 && step18_1)
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

                //Step-20: Select a study layout button and change the layout to 5x5 for fourth study panel.
                bluringviewer.ChangeViewerLayout("5x5", 4, 6);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step19_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(4) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step19_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(4) + BluRingViewer.viewportContainer), 2, 1);
                bool step19_3 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(4) + BluRingViewer.div_ViewportPanels)).Count == 25;
                if (step19_1 && step19_2 && step19_3)
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

                //Step-21: In order to load study in an empty series viewer (if any) or replace the series that is currently displayed within another, by these two methods and check:
                //1. select the destination series viewer, then double click on the thumbnail for the series you want to display for particular study panel.
                GetElement(SelectorType.CssSelector, bluringviewer.SetViewPort(6, 4)).Click();
                ThumbnailList = bluringviewer.ThumbnailIndicator(3);
                thumbnail = ThumbnailList[2];
                DoubleClick(thumbnail);
                //2. drag and drop the thumbnail for the series you want to display into the desired series viewer for the particular study panel.
                bluringviewer.DropAndDropThumbnails(1, 8, 4, UseDragDrop: true);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step20_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(4) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step20_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(4) + BluRingViewer.viewportContainer), 2, 1);
                if (step20_1 && step20_2)
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

                //Step-22: Select any series in the study panel and apply tool actions.
                GetElement(SelectorType.CssSelector, bluringviewer.SetViewPort(0, 4)).Click();
                bluringviewer.SelectViewerTool(BluRingTools.Window_Level, 4);
                bluringviewer.ApplyTool_WindowWidth();
                bluringviewer.SelectViewerTool(BluRingTools.Invert, 4);
                bluringviewer.SelectViewerTool(BluRingTools.Pan, 4);
                bluringviewer.ApplyTool_Pan();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step21_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(4) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step21_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(4) + BluRingViewer.viewportContainer), 2, 1);
                if (step21_1 && step21_2)
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

                //Step-23: Load any priors from the exam list.
                bluringviewer.OpenPriors(StudyDate: StudyDate[3], StudyTime: StudyTime[3]);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step22_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(5) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step22_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(5) + BluRingViewer.viewportContainer), 2, 1);
                if (step22_1 && step22_2)
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

                //Step-24: Drag the any series from thumbnail in the fourth study panel into the 1 st view port of the fifth study panel .
                bluringviewer.DropAndDropForeignThumbnails(4, 2, 5, 1);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step23_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(5) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step23_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(5) + BluRingViewer.viewportContainer), 2, 1);
                if (step23_1 && step23_2)
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

                //Step-25: Select a study layout button and change the layout to 6x6 for fifth study panel.
                bluringviewer.ChangeViewerLayout("6x6", 5, 9);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step24_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(5) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step24_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(5) + BluRingViewer.viewportContainer), 2, 1);
                bool step24_3 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(5) + BluRingViewer.div_ViewportPanels)).Count == 36;
                if (step24_1 && step24_2 && step24_3)
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

                //Step-26: In order to load study in an empty series viewer (if any) or replace the series that is currently displayed within another, by these two methods and check:
                //1. select the destination series viewer, then double click on the thumbnail for the series you want to display for particular study panel.
                GetElement(SelectorType.CssSelector, bluringviewer.SetViewPort(9, 5)).Click();
                ThumbnailList = bluringviewer.ThumbnailIndicator(4);
                thumbnail = ThumbnailList[2];
                DoubleClick(thumbnail);
                //2. drag and drop the thumbnail for the series you want to display into the desired series viewer for the particular study panel.
                bluringviewer.DropAndDropThumbnails(1, 11, 5, UseDragDrop: true);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step25_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(5) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step25_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(5) + BluRingViewer.viewportContainer), 2, 1);
                if (step25_1 && step25_2)
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

                //Step-27: Select any series in the study panel and apply tool actions.
                GetElement(SelectorType.CssSelector, bluringviewer.SetViewPort(0, 5)).Click();
                bluringviewer.SelectViewerTool(BluRingTools.Window_Level, 5);
                bluringviewer.ApplyTool_WindowWidth();
                bluringviewer.SelectViewerTool(BluRingTools.Invert, 5);
                bluringviewer.SelectViewerTool(BluRingTools.Pan, 5);
                bluringviewer.ApplyTool_Pan();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step26_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(5) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step26_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(5) + BluRingViewer.viewportContainer), 2, 1);
                if (step26_1 && step26_2)
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

                //Step-28: Check for initial layout displayed in all study panel.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step27_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.viewportContainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step27_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.viewportContainer), 2);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 3);
                bool step27_3 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(3) + BluRingViewer.viewportContainer), 3);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 4);
                bool step27_4 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(4) + BluRingViewer.viewportContainer), 4);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 5);
                bool step27_5 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(5) + BluRingViewer.viewportContainer), 5, 1);
                if (step27_1 && step27_2 && step27_3 && step27_4 && step27_5)
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

                //Step-29: Close the fifth study panel.
                bluringviewer.CloseStudypanel(5);
                if (!IsElementVisible(By.CssSelector(BluRingViewer.div_Panel(5))))
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

                //Step-30: Close all study panel fourth,third,secondary study panel one by one.
                bluringviewer.CloseStudypanel(4);
                bool step29_1 = IsElementVisible(By.CssSelector(BluRingViewer.div_Panel(4)));
                bluringviewer.CloseStudypanel(3);
                bool step29_2 = IsElementVisible(By.CssSelector(BluRingViewer.div_Panel(3)));
                bluringviewer.CloseStudypanel(2);
                bool step29_3 = IsElementVisible(By.CssSelector(BluRingViewer.div_Panel(2)));
                if (!step29_1 && !step29_2 && !step29_3)
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

                //Close the viewer
                bluringviewer.CloseBluRingViewer();
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

            }
        }

        /// <summary> 
        /// Viewer Layout - Auto-layout
        /// </summary>
        public TestCaseResult Test_141521(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables            
            TestCaseResult result = new TestCaseResult(stepcount);
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            UserManagement usermanagement;
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String DescriptionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
            String[] Description = DescriptionList.Split(':');
            String modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
            String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
            String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
            String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String AccessionNo = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo");
            String[] PatientID = PatientIDList.Split(':');
            String StudyDateList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate");
            String StudyTimeList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyTime");
            String[] StudyDate = StudyDateList.Split(':');
            String[] StudyTime = StudyTimeList.Split('=');

            try
            {
                //Precondition - Creating Domain as we will change all settings in this domain and use Domain Admin user settings instead of Administrator - To avoid other scripts getting affected
                login.LoginIConnect(username, password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                //Domain variables
                var domainattr = domainmanagement.CreateDomainAttr();
                foreach (var item in domainattr)
                {
                    Logger.Instance.InfoLog(item.Key + " : " + item.Value);
                }

                //Domain
                String Testdomain = domainattr[DomainManagement.DomainAttr.DomainName];
                String TestdomainAdmin = domainattr[DomainManagement.DomainAttr.UserID];
                String TestdomainAdmin_Pwd = domainattr[DomainManagement.DomainAttr.Password];
                String Testuser1 = "141521_User1" + new Random().Next(1, 1000);

                domainmanagement.CreateDomain(domainattr);

                domainmanagement.SearchDomain(Testdomain);
                domainmanagement.SelectDomain(Testdomain);
                domainmanagement.EditDomainButton().Click();
                Driver.SwitchTo().DefaultContent();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForPageLoad(10);
                domainmanagement.VisibleAllStudySearchField();
                //Setting Auto for each modality
                foreach (IWebElement option in domainmanagement.ModalityDropDown().Options)
                {
                    domainmanagement.AddPreset(option.Text, String.Empty, String.Empty, String.Empty, "auto");
                    domainmanagement.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                }

                PageLoadWait.WaitForFrameLoad(10);
                domainmanagement.SaveDomainButtoninEditPage().Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Create User
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(Testuser1, Testdomain, domainattr[DomainManagement.DomainAttr.RoleName]);
                login.Logout();

                //Step-1: Login to ICA as Administrator.
                login.LoginIConnect(TestdomainAdmin, TestdomainAdmin_Pwd);
                ExecutedSteps++;

                //Step-2: In the Studies tab,search for the multi series prior study . Load the studies in the iCA Enterprise viewer.
                studies = login.Navigate<Studies>();
                studies.SearchStudy(LastName: "*", Description: Description[0]);
                studies.SelectStudy("Patient ID", PatientID[5]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad(viewport: 4);
                IWebElement LayoutIcon = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutIcon), WaitTypes.Visible);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step2)
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

                //Step-3: Load study only have three series
                bluringviewer.CloseBluRingViewer();
                studies.SearchStudy(LastName: "*", Description: Description[0]);
                studies.SelectStudy("Patient ID", PatientID[5]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step3_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                bool step3_2 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 3;
                if (step3_1 && step3_2)
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

                //Step-4: Check for study panel dimension.
                //LayoutIcon.Click();
                IList<IWebElement> viewport = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_allViewportes));
                //Portrait validation
                bool[] step4 = new bool[viewport.Count];
                int count = 0;
                foreach (var item in viewport)
                {
                    step4[count] = (item.Size.Height > item.Size.Width);
                }
                if (ValidateBoolArray(step4))
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

                //Step-5: Change the layout to 3x1 from study layout grid.
                bluringviewer.ChangeViewerLayout("3x1");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step5_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                bool step5_2 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 3;
                if (step5_1 && step5_2)
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

                //Step-6: Check for study panel dimension.
                viewport = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_allViewportes));
                //Landscape validation
                bool[] step6 = new bool[viewport.Count];
                count = 0;
                foreach (var item in viewport)
                {
                    step6[count] = (item.Size.Width > item.Size.Height);
                }
                if (ValidateBoolArray(step6))
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

                //Step-7: Close the study viewer.
                bluringviewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Step-8: Load study only have four series
                studies.SearchStudy(LastName: "*", Description: Description[0]);
                studies.SelectStudy("Patient ID", PatientID[5]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step8_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                bool step8_2 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 4;
                if (step8_1 && step8_2)
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

                //Step-9: Change the layout to 1x3 from study layout grid.
                bluringviewer.ChangeViewerLayout("1x3", viewport: 4);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step9_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                bool step9_2 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 3;
                if (step9_1 && step9_2)
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

                //Step-10: Check for study panel dimension.
                //Portrait validation
                bool[] step10 = new bool[viewport.Count];
                count = 0;
                foreach (var item in viewport)
                {
                    step10[count] = (item.Size.Height > item.Size.Width);
                }
                if (ValidateBoolArray(step10))
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

                //Step-11: Close the study viewer.
                bluringviewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Step-12: Load study which has 6 series .
                studies.SearchStudy(LastName: "*", Description: Description[0]);
                studies.SelectStudy("Patient ID", PatientID[5]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step12_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                bool step12_2 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 6;
                if (step12_1 && step12_2)
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

                //Step-13: Check for study panel dimension.
                //Landscape validation
                bool[] step13 = new bool[viewport.Count];
                count = 0;
                foreach (var item in viewport)
                {
                    step13[count] = (item.Size.Height > item.Size.Width);
                }
                if (ValidateBoolArray(step13))
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


                //Step-14: Change the layout to 3x2 from study layout grid.
                bluringviewer.ChangeViewerLayout("3x2", viewport: 4);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step14_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                bool step14_2 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 6;
                if (step14_1 && step14_2)
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

                //Step-15: Check for study panel dimension.
                //Portrait validation
                bool[] step15 = new bool[viewport.Count];
                count = 0;
                foreach (var item in viewport)
                {
                    step15[count] = (item.Size.Height > item.Size.Width);
                }
                if (ValidateBoolArray(step15))
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

                //Step-16: Close the study viewer.
                bluringviewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Step-17: Load related study , which contains only one series.
                studies.SearchStudy(LastName: "*", Description: Description[0]);
                studies.SelectStudy("Patient ID", PatientID[5]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step17_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                bool step17_2 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 1;
                if (step17_1 && step17_2)
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

                //Step-18: Single click the related study with two series. under Exam List Panel to open up the prior study in the viewer.
                bluringviewer.OpenPriors(StudyDate: "04-Mar-2010", StudyTime: "3:00:03 PM");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step18_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step18_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.viewportContainer), 2, 1);
                bool step18_3 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(2) + BluRingViewer.div_ViewportPanels)).Count == 2;
                if (step18_1 && step18_2 && step18_3)
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

                //Step-19: Close the study viewer.
                bluringviewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Step-20: Load related study, which contains only two series.
                studies.SearchStudy(LastName: "*", Description: Description[0]);
                studies.SelectStudy("Patient ID", PatientID[5]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step20_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                bool step20_2 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 2;
                if (step20_1 && step20_2)
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

                //Step-21: Single click the related study with nine series. under Exam List Panel to open up the prior study in the viewer.
                bluringviewer.OpenPriors(StudyDate: "04-Mar-2010", StudyTime: "3:00:03 PM");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step21_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step21_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.viewportContainer), 2, 1);
                bool step21_3 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(2) + BluRingViewer.div_ViewportPanels)).Count == 9;
                if (step21_1 && step21_2 && step21_3)
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

                //Step-22: Close the study viewer.
                bluringviewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Step-23: Load study with related study, which contains only six series.
                studies.SearchStudy(LastName: "*", Description: Description[0]);
                studies.SelectStudy("Patient ID", PatientID[5]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step23_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                bool step23_2 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 6;
                if (step23_1 && step23_2)
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

                //Step-24: Single click the related study with thirty six series. under Exam List Panel to open up the prior study in the viewer.
                bluringviewer.OpenPriors(StudyDate: "04-Mar-2010", StudyTime: "3:00:03 PM");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step24_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.div_thumbnailcontainer));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step24_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.viewportContainer), 2, 1);
                bool step24_3 = Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(2) + BluRingViewer.div_ViewportPanels)).Count == 9;
                if (step24_1 && step24_2 && step24_3)
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

                //Step-25: Close the study viewer.
                bluringviewer.CloseBluRingViewer();

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

            }
        }

        /// <summary> 
        /// Viewer Layout - Change layout for Multiple Panels
        /// </summary>
        public TestCaseResult Test_161692(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables            
            TestCaseResult result = new TestCaseResult(stepcount);
            UserPreferences userpreferences = new UserPreferences();
            UserManagement usermanagement;

            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String DescriptionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
            String[] Description = DescriptionList.Split(':');
            String modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
            String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
            String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
            String DataSourceList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSource");
            String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String StudyDateList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate");
            String StudyTimeList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyTime");
            String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo");
            String[] PatientID = PatientIDList.Split(':');
            String[] DataSource = DataSourceList.Split(':');
            String[] AcessionNo = AccessionNoList.Split(':');
            String[] StudyDate = StudyDateList.Split(':');
            String[] StudyTime = StudyTimeList.Split('=');

            try
            {

                //Precondition - Creating Domain as we will change all settings in this domain and use Domain Admin user settings instead of Administrator - To avoid other scripts getting affected
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                //Domain variables
                var domainattr = domainmanagement.CreateDomainAttr();
                foreach (var item in domainattr)
                {
                    Logger.Instance.InfoLog(item.Key + " : " + item.Value);
                }

                //Domain
                String Testdomain = domainattr[DomainManagement.DomainAttr.DomainName];
                String TestdomainAdmin = domainattr[DomainManagement.DomainAttr.UserID];
                String TestdomainAdmin_Pwd = domainattr[DomainManagement.DomainAttr.Password];
                String TestRole = domainattr[DomainManagement.DomainAttr.RoleName];
                String Testuser1 = "161692_User1" + new Random().Next(1, 1000);

                domainmanagement.CreateDomain(domainattr);

                domainmanagement.SearchDomain(Testdomain);
                domainmanagement.SelectDomain(Testdomain);
                domainmanagement.EditDomainButton().Click();

                Driver.SwitchTo().DefaultContent();
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForPageLoad(10);
                domainmanagement.VisibleAllStudySearchField();

                //Step-1: Login to ICA as Administrator and setup.
                domainmanagement.SetCheckBoxInEditDomain("universalviewer", 0);

                foreach (IWebElement option in domainmanagement.ModalityDropDown().Options)
                {
                    domainmanagement.ModalityDropDown().SelectByText(option.Text);
                    domainmanagement.LayoutDropDown().SelectByText("auto");
                    domainmanagement.SetThumbnailSpiltting("Series");
                }

                PageLoadWait.WaitForPageLoad(20);
                domainmanagement.ClickSaveNewDomain();

                PageLoadWait.WaitForPageLoad(20);
                rolemanagement = login.Navigate<RoleManagement>();

                rolemanagement.ShowRolesFromDomainDropDown();
                rolemanagement.SelectDomainfromDropDown(Testdomain);
                rolemanagement.SelectRole(TestRole);
                PageLoadWait.WaitForPageLoad(20);
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckbox(rolemanagement.StudySearchFieldUseDomainSetting_CB());
                rolemanagement.ClickSaveRole();
                PageLoadWait.WaitForPageLoad(20);

                //Create User
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(Testuser1, Testdomain, domainattr[DomainManagement.DomainAttr.RoleName]);
                login.Logout();
                login.LoginIConnect(TestdomainAdmin, TestdomainAdmin_Pwd);

                //Set user preferences
                userpreferences.OpenUserPreferences();
                userpreferences.SwitchToUserPrefFrame();

                foreach (IWebElement option in userpreferences.ModalityDropDown().Options)
                {
                    userpreferences.ModalityDropdown().SelectByText(option.Text);
                    userpreferences.LayoutDropDown().SelectByText("auto");
                    userpreferences.SetThumbnailSplitting("Series");
                }
                userpreferences.CloseUserPreferences();
                ExecutedSteps++;

                //Step-2:  In Studies tab, Load study that only has 3 series in to the uv
                studies = login.Navigate<Studies>();
                studies.SearchStudy(AccessionNo: AcessionNo[4], Datasource: DataSource[4]);
                studies.SelectStudy("Accession", AcessionNo[4]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();

                PageLoadWait.WaitForBluRingViewportToLoad(viewport: 3);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);

                //Step-3: Check Study panel dimension.
                IWebElement LayoutIcon = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutIcon), WaitTypes.Visible);
                LayoutIcon.Click();

                // make sure the viewpport is 1x3 layout with 3 panels
                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 3
                    && GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("1x3"))
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
                LayoutIcon.Click();

                //Step-4: Change the layout to 3x1 from study layout grid.
                bluringviewer.ChangeViewerLayout("3x1", 1, 3);

                LayoutIcon.Click();
                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 3
                    && GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("3x1"))
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
                LayoutIcon.Click();

                //Step-5: Check for study panel dimension
                LayoutIcon.Click();
                if (GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("3x1"))
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
                LayoutIcon.Click();

                //Step-6: Close the Study Viewer
                bluringviewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Step-7:  Load study only have four series in to universal viewer
                studies.SearchStudy(patientID: PatientID[0], Datasource: DataSource[0]);
                studies.SelectStudy("Patient ID", PatientID[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad(viewport: 4);

                LayoutIcon = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutIcon), WaitTypes.Visible);
                LayoutIcon.Click();

                // make sure the viewpport is 2x2 layout with 4 panels
                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 4
                    && GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("2x2"))
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
                LayoutIcon.Click();

                //Step-8:  Change the layout to 1x3 from study layout grid
                bluringviewer.ChangeViewerLayout("1x3", 1, 3);
                LayoutIcon.Click();

                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 3
                    && GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("1x3"))
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
                LayoutIcon.Click();

                //Step-9:  Check for study panel dimension
                LayoutIcon = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutIcon), WaitTypes.Visible);
                LayoutIcon.Click();

                // make sure the viewpport is 1x3 layout with 3 panels
                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 3
                    && GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("1x3"))
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
                LayoutIcon.Click();

                //Step-10: Close the Study Viewer
                bluringviewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Step-11:  Load study only have 6 series in to universal viewer
                studies.SearchStudy(AccessionNo: AcessionNo[1], Datasource: DataSource[1]);
                studies.SelectStudy("Accession", AcessionNo[1]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad(viewport: 6);
                result.steps[++ExecutedSteps].status = "Pass";

                //Step-12:  Check for study panel dimension
                LayoutIcon = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutIcon), WaitTypes.Visible);
                LayoutIcon.Click();

                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 6
                    && GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("2x3"))
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
                LayoutIcon.Click();

                //Step-13:  Change the layout to 3x2 from study layout grid
                bluringviewer.ChangeViewerLayout("3x2", 1, 6);
                LayoutIcon.Click();

                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 6
                    && GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("3x2"))
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
                LayoutIcon.Click();

                //Step-14:  Check for study panel dimension
                LayoutIcon = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutIcon), WaitTypes.Visible);
                LayoutIcon.Click();

                // make sure the viewpport is 3x2 layout with 6 panels
                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 6
                    && GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("3x2"))
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
                LayoutIcon.Click();

                //Step-15: Close the Study Viewer
                bluringviewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Step-16:  Load study with realted study, which contains only one series in universal viewer.
                studies.SearchStudy(AccessionNo: AcessionNo[2], Datasource: DataSource[2]);
                studies.SelectStudy("Accession", AcessionNo[2]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad(viewport: 1);
                LayoutIcon = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutIcon), WaitTypes.Visible);
                LayoutIcon.Click();

                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 1
                    && GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("1x1"))
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
                LayoutIcon.Click();

                //Step-17:  Load the prior study with one series from the Exam List Panel and verify if Panel two is also just 1x1 
                bluringviewer.OpenPriors(0, "click");
                IWebElement LayoutIcon2 = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_Panel(2) + BluRingViewer.div_LayoutIcon), WaitTypes.Visible);
                LayoutIcon2.Click();

                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(2) + BluRingViewer.div_ViewportPanels)).Count == 1
                    && GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("1x1"))
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
                LayoutIcon2.Click();

                //Step-18:  Load study with realted study, which contains only two series
                bluringviewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: AcessionNo[3], Datasource: DataSource[3]);
                studies.SelectStudy("Accession", AcessionNo[3]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad(viewport: 2);
                LayoutIcon = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutIcon), WaitTypes.Visible);
                LayoutIcon.Click();

                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 2
                    && GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("1x2"))
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
                LayoutIcon.Click();

                //Step-19:  Load the prior study with two series from the Exam List Panel and verify if Panel two is also just 1x2
                bluringviewer.OpenPriors(0, "click");
                LayoutIcon2 = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_Panel(2) + BluRingViewer.div_LayoutIcon), WaitTypes.Visible);
                LayoutIcon2.Click();

                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(2) + BluRingViewer.div_ViewportPanels)).Count == 2
                    && GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("1x2"))
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
                LayoutIcon2.Click();

                //Step-20: Close the Study Viewer
                bluringviewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Step-21:  Load study with realted study which contains only three series
                studies.SearchStudy(AccessionNo: AcessionNo[4], Datasource: DataSource[4]);
                studies.SelectStudy("Accession", AcessionNo[4]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad(viewport: 3);

                LayoutIcon = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutIcon), WaitTypes.Visible);
                LayoutIcon.Click();

                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 3
                    && GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("1x3"))
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
                LayoutIcon.Click();

                //Step-22:  Load the prior study with three series from the Exam List Panel and verify if Panel two is also just 1x3
                bluringviewer.OpenPriors(0, "click");
                LayoutIcon2 = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_Panel(2) + BluRingViewer.div_LayoutIcon), WaitTypes.Visible);
                LayoutIcon2.Click();

                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(2) + BluRingViewer.div_ViewportPanels)).Count == 3
                    && GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("1x3"))
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
                LayoutIcon2.Click();

                //Step-23: Close the Study Viewer
                bluringviewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Step-24:  Load study with realted study which contains only four series
                studies.SearchStudy(patientID: PatientID[0], Datasource: DataSource[0]);
                studies.SelectStudy("Patient ID", PatientID[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad(viewport: 4);

                LayoutIcon = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutIcon), WaitTypes.Visible);
                LayoutIcon.Click();

                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 4
                    && GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("2x2"))
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
                LayoutIcon.Click();

                //Step-25:  Load the prior study with four series from the Exam List Panel and verify if Panel two is also just 2x2
                bluringviewer.OpenPriors(0, "click");
                LayoutIcon2 = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_Panel(2) + BluRingViewer.div_LayoutIcon), WaitTypes.Visible);
                LayoutIcon2.Click();

                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(2) + BluRingViewer.div_ViewportPanels)).Count == 4
                    && GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("2x2"))
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
                LayoutIcon2.Click();

                //Step-26: Close the Study Viewer
                bluringviewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Step-27:  Load study with realted study which contains more than six series
                studies.SearchStudy(AccessionNo: AcessionNo[5], Datasource: DataSource[5]);
                studies.SelectStudy("Accession", AcessionNo[5]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad(viewport: 6);

                LayoutIcon = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutIcon), WaitTypes.Visible);
                LayoutIcon.Click();

                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels)).Count == 6
                    && GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("2x3"))
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
                LayoutIcon.Click();

                //Step-28:  Load the prior study with more than six series from the Exam List Panel and verify if Panel two is also just 2x3
                bluringviewer.OpenPriors(0, "click");
                LayoutIcon2 = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_Panel(2) + BluRingViewer.div_LayoutIcon), WaitTypes.Visible);
                LayoutIcon2.Click();

                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(2) + BluRingViewer.div_ViewportPanels)).Count == 6
                    && GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("2x3"))
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
                LayoutIcon2.Click();

                //Step-29:  Load another prior study with more than six series from the Exam List Panel
                bluringviewer.OpenPriors(0, "click");
                IWebElement LayoutIcon3 = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_Panel(3) + BluRingViewer.div_LayoutIcon), WaitTypes.Visible);
                LayoutIcon3.Click();

                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(3) + BluRingViewer.div_ViewportPanels)).Count == 6
                    && GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(3) + BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("2x3"))
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
                LayoutIcon3.Click();

                //Step-30: Close the Study Viewer
                bluringviewer.CloseBluRingViewer();
                ExecutedSteps++;

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

            }
        }

       
        /// <summary> 
        /// Unselected layout is shown as selected in layout grid.
        /// </summary>
        public TestCaseResult Test_167657(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables            
            TestCaseResult result = new TestCaseResult(stepcount);

            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo");
            try
            {

                //Step-1:  Unselected layout is shown as selected in layout grid.
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                studies = login.Navigate<Studies>();
                studies.SearchStudy(AccessionNo: Accession, Datasource: GetHostName(Config.EA96));
                studies.SelectStudy("Accession", Accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                IWebElement LayoutIcon = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_LayoutIcon), WaitTypes.Visible);
                LayoutIcon.Click();
                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_ViewportPanels)).Count == 4 && bluringviewer.verfiyLayoutHighlighted(2, 2))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }


                //Step-2: In Study panel toolbar, Hover the mouse pointer here and there over the layout without selecting any option.
                IWebElement LayoutGrid = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_LayoutGridWrapper), WaitTypes.Visible);
                bluringviewer.HouseOverToLayout(4, 4);
                if (bluringviewer.verfiyLayoutHighlighted(4, 4))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 3 - Click on the mouse pointer outside the viewport.
                bluringviewer.ClickOnViewPort(1,1);
                LayoutGrid = BasePage.FindElementByCss(BluRingViewer.div_LayoutGridWrapper);
                if (!LayoutGrid.Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }


                //Step 4 - Click on the layout option and verify the selected layout..
                LayoutIcon.Click();
                // make sure the viewpport is 1x3 layout with 3 panels
                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_ViewportPanels)).Count == 4
                    && GetElement(SelectorType.CssSelector, BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("2x2") && bluringviewer.verfiyLayoutHighlighted(2, 2))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                LayoutIcon.Click();

                //Step-5: Click on the layout option and change the layout to 6x6
                bluringviewer.ChangeViewerLayout("6x6", 1, 12);
                LayoutIcon.Click();
                BluRingViewer.WaitforViewports();
                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_ViewportPanels)).Count == 36
                    && GetElement(SelectorType.CssSelector, BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("6x6") && bluringviewer.verfiyLayoutHighlighted(6, 6))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                LayoutIcon.Click(); //Close


                //Step-6: In Study panel toolbar, Hover the mouse pointer here and there over the layout without selecting any option. eg:say as 4x4)
                LayoutIcon.Click();
                LayoutGrid = PageLoadWait.WaitForElement(By.CssSelector(BluRingViewer.div_LayoutGridWrapper), WaitTypes.Visible);
                bluringviewer.HouseOverToLayout(4, 4);
                if (bluringviewer.verfiyLayoutHighlighted(4, 4))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 7 - Click on the mouse pointer outside the view port.
                new TestCompleteAction().MoveToElement(BasePage.FindElementByCss(bluringviewer.Activeviewport)).Click().Perform();
                LayoutGrid = BasePage.FindElementByCss(BluRingViewer.div_LayoutGridWrapper);
                if (!LayoutGrid.Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }


                //Step 8 - Click on the layout option and verify the selected layout.
                LayoutIcon.Click();
                // make sure the viewpport is 1x3 layout with 3 panels
                if (Driver.FindElements(By.CssSelector(BluRingViewer.div_ViewportPanels)).Count == 36
                    && GetElement(SelectorType.CssSelector, BluRingViewer.div_LayoutGridWrapper + ">div").Text.Equals("6x6"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                bluringviewer.CloseBluRingViewer();
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

    }
}
