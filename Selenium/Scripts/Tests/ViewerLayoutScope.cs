using System;
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


namespace Selenium.Scripts.Tests
{
    class ViewerLayoutScope
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public string EA_91 = "VMSSA-5-38-91";
        public string EA_131 = "VMSSA-4-38-131";
        public string PACS_A7 = "PA-A7-WS8";


        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        public ViewerLayoutScope(String classname)
        {
            login = new Login();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }


        /// <summary> 
        /// This Test Case is Verification of Viewer Layout Scope "Initial Setup"
        /// </summary>

        public TestCaseResult Test_28034(String testid, String teststeps, int stepcount)
        {
            //-Initial Setup--
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;

            try
            {
                //Step-1
                //"Test Data-  Viewer Layout Ref (screenshot) is in Attachment.  Pre-conditions-
                //1. Datasource- at least one istore online, one DICOM and one AMICAS.  
                //For data source of type - AMICAS, use HALOPACS (ID & Host is HALOPACS, IP 3.0.5.132, AETitle- HPACSAE, Port-10
                //2. Client Browser to be used in this execution should be consistent to the latest version of Functional Product Specification for iConnect Access."

                //Added EA-77, EA-91, EA-131 and PA-A7-WS8

                ExecutedSteps++;

                //Step-2
                //Pre-condition  Configure requisition viewer in ICA Service Tool->Enable Features->General sub-tab,
                // then in the Domain page put a check mark in Enable Requisition Report check box. 
                //Send test data with requisition (AAXFFP from jarjar) to data source.

                //Enabled check box
                ExecutedSteps++;

                //Step-3
                //Pre-condition  Enable study attachment in ICA Service Tool -> Enable Features -> Study Attachment tab. 
                //Select to 'Store attachments with original study'.

                //Done
                ExecutedSteps++;

                //Step-4
                //Pre-condition:At domain level enable-report viewing requisition report attachment viewing & uploading

                //Enabled check box
                ExecutedSteps++;

                //Step-5
                //Pre-condition Configure all review tools to be available tools.
                //Default settings for layout have not been changed. Log in as Administrator/Administrator.

                //Configured all tools
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
                //login.Logout();

                //Return Result
                return result;
            }
        }


        /// <summary> 
        /// This Test Case is Verification of Viewer Layout Scope "Modality Default layout setting"
        /// </summary>

        public TestCaseResult Test_28035(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;

            String DescriptionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
            String[] Description = DescriptionList.Split(':');

            String modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
            String count = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "count");

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

                //Initia Set up completed

                ExecutedSteps++;

                //Login -
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Step-2
                //Test Data: JPEG image(i.e.lossy)not applicable for HTML5 Viewer in iCA 4.0 on client side
                // User has the options-  choose the Legacy study viewer or the new HTML5 study viewer to load various CR studies.

                //The selected study is loaded and displayed into the viewer; it should auto-fill all viewers 
                //in the series layout with the first available series. Since the default layout for 
                //CR modality is set to 'auto', the studies should be loaded with the smallest layout 
                //that can fit as many of the series available as possible.
                //If the loaded study has 1 series, layout should be 1x1. 
                //If the loaded study has 2 series, layout should be 1x2. 
                //If the loaded study has 3 series, layout should be 1x3. 
                //If the loaded study has 4 series, layout should be 2x2. 
                //If the loaded study has 5 or more series, layout should be 2x3.
                //By default, the image shall be displayed on the client as JPEG image (i.e. lossy). 
                //A JPEG indicator is displayed in the study bar.

                bool flag2 = true;

                //Accession1=Modality1=Count1
                //KHIS080107130=CR=1:
                Studies study = (Studies)login.Navigate("Studies");

                //--Pre condition-- enable connection tool.
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB selected successfully");

                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();

                study.SearchStudy(AccessionNo: Accession1[0], Modality: Modality1[0], Datasource: EA_131);
                study.SelectStudy("Accession", Accession1[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                if (!(viewer.Thumbnails().Count == Int32.Parse(Count1[0])) && !(viewer.SeriesViewPorts().Count == 1))
                {
                    flag2 = false;
                }

                result.steps[++ExecutedSteps].SetPath(testid + "_2_1_1x1", ExecutedSteps + 1);
                bool status2_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();


                //--2
                //Accession1=Modality1=Count1
                //3002108387=CR=2
                study.SearchStudy(AccessionNo: Accession1[1], Modality: Modality1[1], Datasource: EA_131);
                study.SelectStudy("Accession", Accession1[1]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                if (!(viewer.Thumbnails().Count == Int32.Parse(Count1[1])) && !(viewer.SeriesViewPorts().Count == 2))
                {
                    flag2 = false;
                }

                result.steps[ExecutedSteps].SetPath(testid + "_2_2_1x2", ExecutedSteps + 1);
                bool status2_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //--3

                //Accession1=Modality1=Count1
                //11594551=CR=3:
                study.SearchStudy(AccessionNo: Accession1[2], Modality: Modality1[2], Datasource: EA_131);
                study.SelectStudy("Accession", Accession1[2]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                if (!(viewer.Thumbnails().Count == Int32.Parse(Count1[2])) && !(viewer.SeriesViewPorts().Count == 3))
                {
                    flag2 = false;
                }

                result.steps[ExecutedSteps].SetPath(testid + "_2_3_1x3", ExecutedSteps + 1);
                bool status2_3 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //--4
                //*

                //PatientID2=Modality2=Count2
                //NA=CR=4:
                //Description FOOT-LT

                study.SearchStudy(Description: Description[0], Modality: Modality2[0], Datasource: EA_131);
                study.SelectStudy("Description", Description[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                if (!(viewer.Thumbnails().Count == Int32.Parse(Count2[0])) && !(viewer.SeriesViewPorts().Count == 4))
                {
                    flag2 = false;
                }

                result.steps[ExecutedSteps].SetPath(testid + "_2_4_2x2", ExecutedSteps + 1);
                bool status2_4 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //--5
                //*
                //Accession1=Modality1=Count1
                //TW31747884=CR=5:
                study.SearchStudy(AccessionNo: Accession1[3], Modality: Modality1[3], Datasource: EA_131);
                study.SelectStudy("Accession", Accession1[3]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                if (!(viewer.Thumbnails().Count == Int32.Parse(Count1[3])) && !(viewer.SeriesViewPorts().Count == 6))
                {
                    flag2 = false;
                }

                result.steps[ExecutedSteps].SetPath(testid + "_2_5_2x3", ExecutedSteps + 1);
                bool status2_5 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();


                if (flag2 && status2_1 && status2_2 && status2_3 && status2_4 && status2_5)
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

                //Step-3
                //Go to Options -> User Preferences and change the image format to PNG. Save the changes.

                PageLoadWait.WaitForPageLoad(20);
                userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.SelectRadioBtn("ImageFormatRadioButtonList", "PNG (lossless)");
                PageLoadWait.WaitForPageLoad(20);
                
                //disable conn test tool----
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == true)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB de-selected successfully");

                }
                PageLoadWait.WaitForPageLoad(20);
                //-- Click OK and close btn--
                userpref.CloseUserPreferences();
                ExecutedSteps++;


                //Step-4
                //Load various CT studies.
                //PatientID2=Modality2=Count2
                //2CT2=CT=2:

                study.SearchStudy(patientID: PatientID2[1], Modality: Modality2[1], Datasource: EA_91);
                study.SelectStudy("Patient ID", PatientID2[1]);

                StudyViewer.LaunchStudy();
                Thread.Sleep(5000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //The studies should be loaded in 2x2, regardless of how many series the study has.
                //The image should be displayed PNG image (i.e. lossless)

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status4 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status4 && viewer.Thumbnails().Count == Int32.Parse(Count2[1]) &&
                    viewer.SeriesViewPorts().Count == 4)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-5
                //Load various DR studies.
                //Accession1=Modality1=Count1
                //SMS000023=DR=NA:
                study.SearchStudy(AccessionNo: Accession1[4], Modality: Modality1[4], Datasource: EA_91);
                study.SelectStudy("Accession", Accession1[4]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[viewer.Thumbnails().Count-1]));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Same expected result as for CR modality.
                bool flag5 = false;

                if (viewer.Thumbnails().Count == 1)
                {
                    if (viewer.SeriesViewPorts().Count == 1)
                        flag5 = true;
                }
                else if (viewer.Thumbnails().Count == 2)
                {
                    if (viewer.SeriesViewPorts().Count == 2)
                        flag5 = true;
                }
                else if (viewer.Thumbnails().Count == 3)
                {
                    if (viewer.SeriesViewPorts().Count == 3)
                        flag5 = true;
                }
                else if (viewer.Thumbnails().Count == 4)
                {
                    if (viewer.SeriesViewPorts().Count == 4)
                        flag5 = true;
                }
                else if (viewer.Thumbnails().Count >= 5)
                {
                    if (viewer.SeriesViewPorts().Count == 6)
                        flag5 = true;
                }
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status5 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-6
                //Load various DX studies.
                //Accession1=Modality1=Count1
                //4070=DX=NA:
                study.SearchStudy(AccessionNo: Accession1[5], Modality: Modality1[5], Datasource: EA_131);
                study.SelectStudy("Accession", Accession1[5]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Same expected result as for CR modality.
                bool flag6 = false;

                if (viewer.Thumbnails().Count == 1)
                {
                    if (viewer.SeriesViewPorts().Count == 1)
                        flag6 = true;
                }
                else if (viewer.Thumbnails().Count == 2)
                {
                    if (viewer.SeriesViewPorts().Count == 2)
                        flag6 = true;
                }
                else if (viewer.Thumbnails().Count == 3)
                {
                    if (viewer.SeriesViewPorts().Count == 3)
                        flag6 = true;
                }
                else if (viewer.Thumbnails().Count == 4)
                {
                    if (viewer.SeriesViewPorts().Count == 4)
                        flag6 = true;
                }
                else if (viewer.Thumbnails().Count >= 5)
                {
                    if (viewer.SeriesViewPorts().Count == 6)
                        flag6 = true;
                }

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status6 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-7
                //Load various MG studies (MG Pres and MG Proc). 
                //An example of MR Pres is Mammot Jenna available in \\panda\data_storage\MammoRealData6. 
                //TBD MG Proc data.
                //** Have to get exact study

                //PatientID2=Modality2=Count2
                //129335=MG=8:

                study.SearchStudy(patientID: PatientID2[2], Modality: Modality2[2], Datasource: EA_131);
                study.SelectStudy("Patient ID", PatientID2[2]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[1]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[2]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[3]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[viewer.Thumbnails().Count - 1]));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Bydefault the studies should be loaded in 1x2, regardless of how many series the study has.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status7 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status7 && viewer.Thumbnails().Count == Int32.Parse(Count2[2]) &&
                    viewer.SeriesViewPorts().Count == 2)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-8
                //Load various MR studies.
                //PatientID2=Modality2=Count2
                //007=MR=1:

                study.SearchStudy(patientID: PatientID2[3], Modality: Modality2[3], Datasource: EA_91);
                study.SelectStudy("Patient ID", PatientID2[3]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[viewer.Thumbnails().Count - 1]));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Since the default layout for MR modality is set to '2x2', the studies should be loaded in 2x2, regardless of how many series the study has.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status8 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status8 && viewer.Thumbnails().Count == Int32.Parse(Count2[3]) &&
                    viewer.SeriesViewPorts().Count == 4)
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

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-9
                //Load various NM studies.
                //Accession1=Modality1=Count1
                //PIKR0001=NM=1:
                study.SearchStudy(AccessionNo: Accession1[6], Modality: Modality1[6], Datasource: EA_91);
                study.SelectStudy("Accession", Accession1[6]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[viewer.Thumbnails().Count - 1]));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Since the default layout for NM modality is set to '2x2', the studies should be loaded in 2x2, regardless of how many series the study has.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status9 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status9 && viewer.Thumbnails().Count == Int32.Parse(Count1[6]) &&
                    viewer.SeriesViewPorts().Count == 4)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-10
                //Load various OT studies.
                //PatientID2=Modality2=Count2
                //6509=OT=5:

                study.SearchStudy(patientID: PatientID2[4], Modality: Modality2[4], Datasource: EA_131);
                study.SelectStudy("Patient ID", PatientID2[4]);
                StudyViewer.LaunchStudy();
                Thread.Sleep(5000);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[viewer.Thumbnails().Count - 1]));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                

                //Since the default layout for OT modality is set to '1x2', the studies should be loaded in 1x2, regardless of how many series the study has.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status10 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status10 && viewer.Thumbnails().Count == Int32.Parse(Count2[4]) &&
                    viewer.SeriesViewPorts().Count == 2)
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

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-11
                //Load various PT studies.
                //Accession1=Modality1=Count1
                //537103=PT=1:
                study.SearchStudy(AccessionNo: Accession1[7], Modality: Modality1[7], Datasource: EA_91);
                study.SelectStudy("Accession", Accession1[7]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[viewer.Thumbnails().Count - 1]));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Same expected result as for CT modality.
                //default layout set to '2x2', the studies should be loaded in 2x2, regardless of how many series the study has

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status11 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status11 && viewer.Thumbnails().Count == Int32.Parse(Count1[7]) &&
                    viewer.SeriesViewPorts().Count == 4)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-12
                //Load various RF studies.
                //Accession1=Modality1=Count1
                //QLBBB140=RF=2:
                //QLBBB140=RF=1: --
                study.SearchStudy(AccessionNo: Accession1[8], Modality: Modality1[8], Datasource: EA_131);
                study.SelectStudy("Accession", Accession1[8]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[viewer.Thumbnails().Count - 1]));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Same expected result as for CT modality.
                //default layout '2x2'

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status12 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status12 && viewer.Thumbnails().Count == Int32.Parse(Count1[8]) &&
                    viewer.SeriesViewPorts().Count == 4)
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

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-13
                //Load various RG studies.
                //PatientID2=Modality2=Count2
                //1545=RG=NA:

                study.SearchStudy(patientID: PatientID2[5], Modality: Modality2[5], Datasource: EA_91);
                study.SelectStudy("Patient ID", PatientID2[5]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[viewer.Thumbnails().Count - 1]));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Same expected result as for CR modality.

                bool flag13 = false;

                if (viewer.Thumbnails().Count == 1)
                {
                    if (viewer.SeriesViewPorts().Count == 1)
                        flag13 = true;
                }
                else if (viewer.Thumbnails().Count == 2)
                {
                    if (viewer.SeriesViewPorts().Count == 2)
                        flag13 = true;
                }
                else if (viewer.Thumbnails().Count == 3)
                {
                    if (viewer.SeriesViewPorts().Count == 3)
                        flag13 = true;
                }
                else if (viewer.Thumbnails().Count == 4)
                {
                    if (viewer.SeriesViewPorts().Count == 4)
                        flag13 = true;
                }
                else if (viewer.Thumbnails().Count >= 5)
                {
                    if (viewer.SeriesViewPorts().Count == 6)
                        flag13 = true;
                }
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status13 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (flag13 && status13)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-14
                //Load various SC studies.
                //PatientID2=Modality2=Count2
                //15_20100914.174041=SC=1:

                study.SearchStudy(patientID: PatientID2[6], Modality: Modality2[6], Datasource: EA_91);
                study.SelectStudy("Patient ID", PatientID2[6]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[viewer.Thumbnails().Count - 1]));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Same expected result as for MG modality.
                //default layout is set to '1x2'

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status14 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status14 && viewer.Thumbnails().Count == Int32.Parse(Count2[6]) &&
                    viewer.SeriesViewPorts().Count == 2)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-15
                //Test Data -  1.2.840.10008.5.1.4.1.1.7.4 
                //Load various SC-mf Colour studies.  Multi-frame True Color Secondary Capture Image Storage

                //PatientID2=Modality2=Count2
                //Patient ID=SC=1:

                study.SearchStudy(patientID: PatientID2[7], Modality: Modality2[7], Datasource: EA_131);
                study.SelectStudy("Patient ID", PatientID2[7]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[viewer.Thumbnails().Count - 1]));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Same expected result as for MG modality.
                //default layout for MG modality is set to '1x2'

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status15 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status15 && viewer.Thumbnails().Count == Int32.Parse(Count2[7]) &&
                    viewer.SeriesViewPorts().Count == 2)
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

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-16

                //Load various US studies (single frame and multi frame).
                //Single frame

                //PatientID2=Modality2=Count2
                //M01210120=US=NA:

                study.SearchStudy(patientID: PatientID2[8], Modality: Modality2[8], Datasource: EA_131);
                study.SelectStudy("Patient ID", PatientID2[8]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[viewer.Thumbnails().Count - 1]));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Same expected result as for CR modality.
                bool flag16_1 = false;
                if (viewer.Thumbnails().Count == 1)
                {
                    if (viewer.SeriesViewPorts().Count == 1)
                        flag16_1 = true;
                }
                else if (viewer.Thumbnails().Count == 2)
                {
                    if (viewer.SeriesViewPorts().Count == 2)
                        flag16_1 = true;
                }
                else if (viewer.Thumbnails().Count == 3)
                {
                    if (viewer.SeriesViewPorts().Count == 3)
                        flag16_1 = true;
                }
                else if (viewer.Thumbnails().Count == 4)
                {
                    if (viewer.SeriesViewPorts().Count == 4)
                        flag16_1 = true;
                }
                else if (viewer.Thumbnails().Count >= 5)
                {
                    if (viewer.SeriesViewPorts().Count == 6)
                        flag16_1 = true;
                }

                result.steps[++ExecutedSteps].SetPath(testid + "_16_1_SingleFrame", ExecutedSteps + 1);
                bool status16_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Multi frame

                //PatientID2=Modality2=Count2
                //469FBF:US:NA

                study.SearchStudy(patientID: PatientID2[9], Modality: Modality2[9], Datasource: EA_131);
                study.SelectStudy("Patient ID", PatientID2[9]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[viewer.Thumbnails().Count - 1]));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Same expected result as for CR modality.
                bool flag16_2 = false;

                if (viewer.Thumbnails().Count == 1)
                {
                    if (viewer.SeriesViewPorts().Count == 1)
                        flag16_2 = true;
                }
                else if (viewer.Thumbnails().Count == 2)
                {
                    if (viewer.SeriesViewPorts().Count == 2)
                        flag16_2 = true;
                }
                else if (viewer.Thumbnails().Count == 3)
                {
                    if (viewer.SeriesViewPorts().Count == 3)
                        flag16_2 = true;
                }
                else if (viewer.Thumbnails().Count == 4)
                {
                    if (viewer.SeriesViewPorts().Count == 4)
                        flag16_2 = true;
                }
                else if (viewer.Thumbnails().Count >= 5)
                {
                    if (viewer.SeriesViewPorts().Count == 6)
                        flag16_2 = true;
                }
                result.steps[ExecutedSteps].SetPath(testid + "_16_2_MultiFrame", ExecutedSteps + 1);
                bool status16_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (flag16_1 && status16_1 && flag16_2 && status16_2)
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

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                study.CloseStudy();

                //Step-17
                //Load various VL studies.
                //Des :STANDARD_VL_MICROSCOPIC ,
                study.SearchStudy(Description: Description[1], Modality: modality, Datasource: EA_131);
                study.SelectStudy("Description", Description[1]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[viewer.Thumbnails().Count - 1]));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Same expected result as for CT modality.(2x2)

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status17 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status17 && viewer.Thumbnails().Count == Int32.Parse(count) &&
                    viewer.SeriesViewPorts().Count == 4)
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

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();


                //Step-18
                //Load various XA studies
                //Accession1=Modality1=Count1
                //9673973=XA=NA:
                study.SearchStudy(AccessionNo: Accession1[9], Modality: Modality1[9], Datasource: EA_91);
                study.SelectStudy("Accession", Accession1[9]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(20);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[1]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[2]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[3]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[4]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[5]));
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(30);

                //Same expected result as for CR modality.
                bool flag18 = false;
                if (viewer.Thumbnails().Count == 1)
                {
                    if (viewer.SeriesViewPorts().Count == 1)
                        flag18 = true;
                }
                else if (viewer.Thumbnails().Count == 2)
                {
                    if (viewer.SeriesViewPorts().Count == 2)
                        flag18 = true;
                }
                else if (viewer.Thumbnails().Count == 3)
                {
                    if (viewer.SeriesViewPorts().Count == 3)
                        flag18 = true;
                }
                else if (viewer.Thumbnails().Count == 4)
                {
                    if (viewer.SeriesViewPorts().Count == 4)
                        flag18 = true;
                }
                else if (viewer.Thumbnails().Count >= 5)
                {
                    if (viewer.SeriesViewPorts().Count == 6)
                        flag18 = true;
                }

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status18 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();


                //Step19
                //Load various ES studies.  VL Endoscopic Image (Endoman.Eliza EE12345)
                //PatientID2=Modality2=Count2
                //28537=ES=NA:

                study.SearchStudy(patientID: PatientID2[10], Modality: Modality2[10], Datasource: EA_131);
                study.SelectStudy("Patient ID", PatientID2[10]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Same expected result as for CR modality.


                bool flag19 = false;
                if (viewer.Thumbnails().Count == 1)
                {
                    if (viewer.SeriesViewPorts().Count == 1)
                        flag19 = true;
                }
                else if (viewer.Thumbnails().Count == 2)
                {
                    if (viewer.SeriesViewPorts().Count == 2)
                        flag19 = true;
                }
                else if (viewer.Thumbnails().Count == 3)
                {
                    if (viewer.SeriesViewPorts().Count == 3)
                        flag19 = true;
                }
                else if (viewer.Thumbnails().Count == 4)
                {
                    if (viewer.SeriesViewPorts().Count == 4)
                        flag19 = true;
                }
                else if (viewer.Thumbnails().Count >= 5)
                {
                    if (viewer.SeriesViewPorts().Count == 6)
                        flag19 = true;
                }
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status19 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

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

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-20
                //Load various IO studies. (Clark,Lola- Forenza) Dental images
                //PatientID2=Modality2=Count2
                //3731-2=IO=NA:

                study.SearchStudy(patientID: PatientID2[11], Modality: Modality2[11], Datasource: EA_91);
                study.SelectStudy("Patient ID", PatientID2[11]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Same expected result as for CR modality.
                bool flag20 = false;

                if (viewer.Thumbnails().Count == 1)
                {
                    if (viewer.SeriesViewPorts().Count == 1)
                        flag20 = true;
                }
                else if (viewer.Thumbnails().Count == 2)
                {
                    if (viewer.SeriesViewPorts().Count == 2)
                        flag20 = true;
                }
                else if (viewer.Thumbnails().Count == 3)
                {
                    if (viewer.SeriesViewPorts().Count == 3)
                        flag20 = true;
                }
                else if (viewer.Thumbnails().Count == 4)
                {
                    if (viewer.SeriesViewPorts().Count == 4)
                        flag20 = true;
                }
                else if (viewer.Thumbnails().Count >= 5)
                {
                    if (viewer.SeriesViewPorts().Count == 6)
                        flag20 = true;
                }

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status20 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

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

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();


                //Step-21
                //Load data with KO (Key Objects) series. Such a data would be Schmidt James 
                //available in Forenza (it has MR, PR, KO and SR modality series).
                //Accession1=Modality1=Count1
                //9066875=KO=NA: (* removed Modality: Modality1[10],)

                study.SearchStudy(AccessionNo: Accession1[10], Datasource: PACS_A7);
                study.SelectStudy("Accession", Accession1[10]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //The series of MR, KO and PR modality should be loaded and displayed into the viewer.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status21 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                //First time error came relaunching the study---**
                if (status21 == false)
                {
                    Logger.Instance.InfoLog("First time error came relaunching the study-28035_21");
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    study.CloseStudy();
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    study.SelectStudy("Accession", Accession1[10]);
                    study.LaunchStudy();
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    PageLoadWait.WaitForAllViewportsToLoad(30);
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[8]));
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[9]));
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[10]));
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    PageLoadWait.WaitForAllViewportsToLoad(30);

                    result.steps[ExecutedSteps].SetPath(testid + "_21_1", ExecutedSteps + 1);
                    status21 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                }

                if (status21 && viewer.SeriesViewPorts().Count == 4)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();



                //Step-22
                //Load data with RT series. Such a data would be Wiley James 
                //available in \\aviance\hdrv\tdr\RTSamples\RTImage481.1 to import it in one of your data sources.

                //PatientID2=Modality2=Count2  93-0022
                //93-0022=RTImage=NA:

                study.SearchStudy(patientID: PatientID2[12], Modality: Modality2[12], Datasource: EA_91);
                study.SelectStudy("Patient ID", PatientID2[12]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //The RT modality series should be loaded and displayed into the viewer.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status22 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status22 && viewer.SeriesViewPorts().Count == 1)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-23
                //Load various OP studies. Ex. 'Eyescan Bestof' available in Forenza.
                //PatientID2=Modality2=Count2
                //PV-Case1=OP=NA:

                study.SearchStudy(patientID: PatientID2[13], Modality: Modality2[13], Datasource: EA_131);
                study.SelectStudy("Patient ID", PatientID2[13]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //The OP (ophthalmology) modality series should be loaded and displayed into the viewer.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status23 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status23 && viewer.SeriesViewPorts().Count == 6 &&
                    viewer.Thumbnails().Count == 5)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-24

                //Load data with AU (audio) series. Ex. 'Szutan' dataset available in Forenza. 
                //This data has 2 studies- one with 1 CR series and one with 2 CR, 2 PR and 1 AU series. 
                //Load the study with the AU series.

                //Accession1=Modality1=Count1
                //00000CT050004054=NA=1:

                study.SearchStudy(AccessionNo: Accession1[11], Datasource: EA_131);
                study.SelectStudy("Accession", Accession1[11]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //The study is loaded into the viewer in a 2x2 layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status24 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status24 && viewer.SeriesViewPorts().Count == 4 &&
                    viewer.Thumbnails().Count == Int32.Parse(Count1[11]))
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

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Step-25
                //Open the patient history drawer -> select Report tab -> double click on the report of AU type
                //Audio file not working in FF browser in windows server 2008

                if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("chrome"))
                {
                    result.steps[++ExecutedSteps].status = "Not Automated";
                }
                else
                {
                    viewer.NavigateToHistoryPanel();
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    viewer.Study(1).Click();

                    //The report is played by the media player (you need to make sure media player - 
                    //Windows Media Player,  Apple QuickTime or RealPlayer -  is installed in you machine)
                    Thread.Sleep(3000);
                    viewer.SwitchToReportFrame("patienthistory");
                    double duration = (double)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function AudioDuration(){var audio = document.querySelector('#Audio_Display_Div>audio');return audio.duration;}return AudioDuration();");
                    Boolean IsAudioPlaying = (Boolean)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function AudioStatus(){var audio = document.querySelector('#Audio_Display_Div>audio');return audio.ended;}return AudioStatus();");

                    double timer = 0;
                    while (duration > timer++)
                    {
                        Thread.Sleep(1000);
                    }
                    //Get audio status
                    Boolean IsAudioEnded = (Boolean)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function AudioStatus(){var audio = document.querySelector('#Audio_Display_Div>audio');return audio.ended;}return AudioStatus();");
                    IWebElement AudioDiv = BasePage.Driver.FindElement(By.Id("Audio_Display_Div"));

                    if (!IsAudioPlaying && IsAudioEnded && AudioDiv.Displayed)
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
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                }
                study.CloseStudy();

                //Step-26

                //Go to Options -> User Preferences and change the layout for CR modality to be 2x2. 
                //Save the changes.  Load a CR study with multiple series.

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                login.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);

                userpref = new UserPreferences();
                userpref.ModalityDropDown().SelectByText("CR");
                PageLoadWait.WaitForPageLoad(10);
                userpref.LayoutDropDown().SelectByText("2x2");
                PageLoadWait.WaitForFrameLoad(10);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);

                //PatientID2=Modality2=Count2
                //123=CR=5:
                study.SearchStudy(patientID: PatientID2[14], Modality: Modality2[14], Datasource: EA_131);
                study.SelectStudy("Patient ID", PatientID2[14]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(5000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //The selected study is loaded and displayed into the viewer. Since the user configured 
                //the series layout for CR modality to be 2x2, the study should be loaded in 2x2 layout, 
                //no matter how many series are in the study.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status26 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status26 && viewer.SeriesViewPorts().Count == 4 &&
                    viewer.Thumbnails().Count == Int32.Parse(Count2[14]))
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //step-27
                //load another CR modality data with 6 series for example (Alm Anna from Forenza).
                // LastName: "ALM"
                //Accession1=Modality1=Count1
                //AA123=CR=7:

                study.SearchStudy(AccessionNo: Accession1[12], Modality: Modality1[12], Datasource: EA_131);
                study.SelectStudy("Accession", Accession1[12]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[5]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[6]));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(30);

                //Changing the series layout from default by the user changes the layout 
                //for all studies loaded with the same modality going forward.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status27 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status27 && viewer.SeriesViewPorts().Count == 4 &&
                    viewer.Thumbnails().Count == Int32.Parse(Count1[12]))
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Step-28
                //Close the study. and logout

                study.CloseStudy();

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
        /// This Test Case is Verification of Viewer Layout Scope "Full Screen Mode"
        /// </summary>

        public TestCaseResult Test_28036(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;

            String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo");

            try
            {
                //Step-1
                //Initia Set up completed
                ExecutedSteps++;

                //Step-2

                //Full screen mode is defined as removing the thumbnail panel and the tool bar. 
                //The icon is located under the reset icon. Once the full screen mode is active 
                //you have to select show menus to see the tool bar. 

                //Load a dataset with multiple studies .

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                Studies study = (Studies)login.Navigate("Studies");
                
                //--Pre condition-- disable connection tool.
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == true)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB un-selected successfully");

                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();

                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                study = (Studies)login.Navigate("Studies");

                //Accession: U-ID179490
                study.SearchStudy(AccessionNo: AccessionID, Datasource: EA_91);
                study.SelectStudy("Accession", AccessionID);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[1]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[3]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[4]));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Study images display as default setting.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status2 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                int thumbnail = 5;
                int Viewport = 4;

                if (status2 && viewer.Thumbnails().Count == thumbnail &&
                    viewer.SeriesViewPorts().Count == Viewport)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Step-3
                //Load a second study to viewer.
                viewer.NavigateToHistoryPanel();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                viewer.DoubleClick(viewer.Study(1));
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60, 1);
                PageLoadWait.WaitForAllViewportsToLoad(60, 2);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[1]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[3]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[4]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails(2)[1]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails(2)[3]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails(2)[4]));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Two study panels displayed with images.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status3 = study.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (status3 && viewer.studyPanel(1).Displayed &&
                    viewer.studyPanel(2).Displayed)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Step-4
                //Click the full screen tool. Enable Full Screen

                viewer.SelectToolInToolBar(IEnum.ViewerTools.FullScreen);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(5000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //The toolbar, thumbnail bar, and menu section are hidden.
                //The study panel expand to fill the available space.
                //Title bar display the patient information- patient ID, patient Name, etc.
                //*//Primary study panel without close button. Report button is available on

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status4 = study.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (status4 &&
                    viewer.ReviewtoolBar().Displayed == false &&
                    viewer.ThumbnailContainer(1).Displayed == false &&
                    viewer.ThumbnailContainer(2).Displayed == false &&
                    viewer.DropMenu().Displayed == false &&
                    viewer.StudyInfoElement(1).Displayed == true &&
                    viewer.StudyInfoElement(2).Displayed == true &&
                    viewer.PatientInfoElement(1).Displayed == true &&
                    viewer.PatientInfoElement(2).Displayed == true &&
                    viewer.GetReviewTool("Close").Displayed == false)
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

                //Step-5
                //Click"Show Menus"tab.
                viewer.MenusBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(5000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Toolbar, thumbnail bar expend to show on the top of the study panels. 
                //And align with the study panel. Show Menus tab change to"Hide Menus"

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status5 = study.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (status5 &&
                    viewer.ReviewtoolBar().Displayed == true &&
                    viewer.ThumbnailContainer(1).Displayed == true &&
                    viewer.ThumbnailContainer(2).Displayed == true)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Step-6
                //Select one tool by click on it.


                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                Thread.Sleep(5000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //The toolbar, thumbnail bar, and menu section are hidden again.  
                //The Patient History Drawer tab and Modality Toolbar should be visible.

                //**##Modality Toolbar should be visible.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status6 = study.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (status6 &&
                    viewer.ReviewtoolBar().Displayed == false &&
                    viewer.ThumbnailContainer(1).Displayed == false &&
                    viewer.ThumbnailContainer(2).Displayed == false &&
                    viewer.DropMenu().Displayed == false)
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

                //Step-7
                //Click Patient History to expand the patient information panel.

                viewer.NavigateToHistoryPanel();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Patient information Panel expands in front of the study panels.
                if (BasePage.Driver.FindElement(By.CssSelector("#patientHistoryDemographics")).Displayed &&
                    BasePage.Driver.FindElement(By.CssSelector("#patientHistoryTable")).Displayed &&
                    BasePage.Driver.FindElement(By.CssSelector("#m_patientHistory_documentViewerContainer")).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8
                //Double click one study from study list.

                viewer.DoubleClick(viewer.Study(2));
                Thread.Sleep(8000);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60, 1);
                PageLoadWait.WaitForAllViewportsToLoad(60, 2);
                PageLoadWait.WaitForAllViewportsToLoad(60, 3);

                //Images load to third study panel. The toolbar, thumbnail bar, and menu section are hidden again
                //The Patient History Drawer tab and Modality Toolbar should be visible.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status7 = study.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (status7 &&
                    viewer.ReviewtoolBar().Displayed == false &&
                    viewer.ThumbnailContainer(1).Displayed == false &&
                    viewer.ThumbnailContainer(2).Displayed == false &&
                    viewer.DropMenu().Displayed == false)
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

                //Step-9
                //Click "Menus"tab.(to show the menus)

                viewer.MenusBtn().Click();

                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(viewer.By_ReviewtoolBar()));
                
                //Toolbar,thumbnail bar expend to show on the top of the study panels & align with the study panel.             

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status9 = study.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (status9 &&
                    viewer.ReviewtoolBar().Displayed == true &&
                    viewer.ThumbnailContainer(1).Displayed == true &&
                    viewer.ThumbnailContainer(2).Displayed == true &&
                    viewer.ThumbnailContainer(3).Displayed == true)
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

                //Step-10
                //Click "Menus" tab. (to hide)

                viewer.MenusBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //The toolbar, thumbnail bar, and menu section are hidden.
                //"Menus"tab display as same name(design changed-show menus& hide menus)
                //The Patient History Drawer tab and Modality Toolbar should be visible.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status10 = study.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (status10 &&
                    viewer.ReviewtoolBar().Displayed == false &&
                    viewer.ThumbnailContainer(1).Displayed == false &&
                    viewer.ThumbnailContainer(2).Displayed == false &&
                    viewer.ThumbnailContainer(3).Displayed == false &&
                    viewer.DropMenu().Displayed == false)
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

                //Step-11
                //Click "Menus" tab.(to show the menus)

                viewer.MenusBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(viewer.By_ReviewtoolBar()));

                //Toolbar, thumbnail bar expend to show on the top of the study panels. 
                //And align with the study panel.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status11 = study.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (status11 &&
                    viewer.ReviewtoolBar().Displayed == true &&
                    viewer.ThumbnailContainer(1).Displayed == true &&
                    viewer.ThumbnailContainer(2).Displayed == true &&
                    viewer.ThumbnailContainer(3).Displayed == true &&
                    viewer.DropMenu().Displayed == true)
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

                //Step-12
                //Click the full screen tool. Disable full screen mode

                viewer.SelectToolInToolBar(IEnum.ViewerTools.FullScreen);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(60, 1);
                PageLoadWait.WaitForAllViewportsToLoad(60, 2);
                PageLoadWait.WaitForAllViewportsToLoad(60, 3);

                //The tools bar, thumbnail bar, and menu section are displayed. 
                //Study Panel resize to  fit into available space under the tool bar, thumbnail bar, and menu sections.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status12 = study.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (status12 &&
                    viewer.ReviewtoolBar().Displayed == true &&
                    viewer.ThumbnailContainer(1).Displayed == true &&
                    viewer.ThumbnailContainer(2).Displayed == true &&
                    viewer.ThumbnailContainer(3).Displayed == true &&
                    viewer.DropMenu().Displayed == true)
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

                //Step-13
                // Select one image and double click on it.

                IWebElement element = viewer.SeriesViewer_1X1();

                element.Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                Actions action = new Actions(BasePage.Driver);
                action.DoubleClick(element).Build().Perform();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20, 1);
                PageLoadWait.WaitForAllViewportsToLoad(20, 2);
                PageLoadWait.WaitForAllViewportsToLoad(20, 3);

                //This enters 1-up mode (the toolbar, thumbnail bar and tabs will continue to be visible),
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status13 = study.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (status13 &&
                    viewer.ReviewtoolBar().Displayed == true &&
                    viewer.ThumbnailContainer(1).Displayed == true &&
                    viewer.ThumbnailContainer(2).Displayed == true &&
                    viewer.ThumbnailContainer(3).Displayed == true &&
                    viewer.DropMenu().Displayed == true)
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

                //Step-14
                //Select the Image layout icon

                IWebElement Download = viewer.GetReviewTool("Download Document");
                viewer.JSMouseHover(Download);

                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                String[] Imagelayout = { "Image Layout 2x2", "Image Layout 4x4", "Image Layout 3x3", "Image Layout 2x1", "Image Layout 1x2", "Image Layout 1x1" };
                Boolean flag14 = true;
                foreach (String s in Imagelayout)
                {
                    IWebElement ele = viewer.GetReviewToolImage(s);
                    if (ele.GetAttribute("class").Contains("disableOnCine") == false)
                    {
                        flag14 = false;
                        break;
                    }
                }

                //The image layout icons are grayed out, not functional.

                if (flag14)
                {
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
                //Click Patient History to expand the patient information panel.

                viewer.NavigateToHistoryPanel();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Patient information Panel expands in front of the study panels.

                if (viewer.PatientHistoryDrawer().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Step-16
                //Close the study. and logout

                study.CloseStudy();
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
        /// This Test Case is Verification of Viewer Layout Scope "Auto-layout"
        /// </summary>

        public TestCaseResult Test_28037(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;

            String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo");
            String[] AccessionNumber = AccessionNoList.Split(':');

            String DescriptionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
            String[] description = DescriptionList.Split(':');

            String DateList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DateList");
            String[] date = DateList.Split('=');

            try
            {
                //Step-1
                //Initia Set up completed
                ExecutedSteps++;

                //Step-2
                //Log in as System or domain administrator.  Go to the Domain Management tab.

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");

                //Domain Results list all active domains.
                ExecutedSteps++;

                //Step-3
                //Select One domain for auto layout testing and double click it.
                //(Create new domain- name: AutoDomain_111)

                string[] Datasource = { "AUTO-SSA-001", "VMSSA-4-38-131", "VMSSA-5-38-91" };
                String AutoDomain = "AutoDomain" + new Random().Next(1000);
                String AutoRole = "AutoRole" + new Random().Next(1000);

                ////Precondition-creating AutoDomain_123 damain
                domain.CreateDomain(AutoDomain, AutoRole, datasources: Datasource);
                PageLoadWait.WaitForPageLoad(30);
                domain.VisibleAllStudySearchField();
                PageLoadWait.WaitForPageLoad(30);
                domain.ClickSaveDomain();

                PageLoadWait.WaitForPageLoad(30);
                domain.SelectDomain(AutoDomain);
                domain.ClickEditDomain();

                //Edit Domain page displayed.
                ExecutedSteps++;

                //Step-4
                //In Default Settings Per Modality section, for each modality, select"auto"in Layout option.
                //Ensure the thumbnail split is set to- Series  Save the changes.

                SelectElement modality = domain.ModalityDropDown();
                SelectElement layOut = domain.LayoutDropDown();


                for (int i = 0; i < modality.Options.Count; i++)
                {
                    modality.SelectByIndex(i);
                    PageLoadWait.WaitForPageLoad(10);
                    layOut.SelectByText("auto");
                    PageLoadWait.WaitForPageLoad(10);
                    domain.SelectRadioBtn("ThumbSplitRadioButton", "Series");
                    PageLoadWait.WaitForPageLoad(10);
                }

                domain.ClickSaveDomain();
                //Domain Results page displayed.
                ExecutedSteps++;
                login.Logout();

                //precondition- login and logout--AutoDomain
                login.DriverGoTo(login.url);
                login.LoginIConnect(AutoDomain, AutoDomain);
                domain = (DomainManagement)login.Navigate("DomainManagement");
                Studies study = (Studies)login.Navigate("Studies");
                //Accession=3453451
                study.SearchStudy(AccessionNo: AccessionNumber[0], Datasource: EA_91);
                PageLoadWait.WaitForPageLoad(20);
                login.Logout();

                //Step-5
                //Log in as a user from the Domain which set up modality is auto-layout.

                login.DriverGoTo(login.url);
                login.LoginIConnect(AutoDomain, AutoDomain);
                ExecutedSteps++;

                //Step-6
                //Load study only have a single series
                domain = (DomainManagement)login.Navigate("DomainManagement");
                study = (Studies)login.Navigate("Studies");

                //Accession=3453451
                study.SearchStudy(AccessionNo: AccessionNumber[0], Datasource: EA_91);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(25);
                study.SelectStudy("Accession", AccessionNumber[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //study loads into 1x1 series layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status6 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());


                if (status6 && viewer.Thumbnails().Count == 1 && viewer.SeriesViewPorts().Count == 1)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-7
                //Load study only have  two series
                //Description:32000
                //Date:28-Feb-1998 9:58:00 AM

                study.SearchStudy(Description: description[0], Datasource: EA_131);
                study.SelectStudy("Study Date", date[0]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //study loads into 1x2 series layout.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status7 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status7 && viewer.Thumbnails().Count == 2 && viewer.SeriesViewPorts().Count == 2)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-8
                //Load study only have  three series
                //Accession:11643936
                study.SearchStudy(AccessionNo: AccessionNumber[1], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumber[1]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //study loads into 1x3 series layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status8 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status8 && viewer.Thumbnails().Count == 3 && viewer.SeriesViewPorts().Count == 3)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-9
                //Load study only have four series

                //Description:Standard Screening - Combo

                study.SearchStudy(Description: description[1], Datasource: EA_131);
                study.SelectStudy("Description", description[1]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //study loads into 2x2 series layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status9 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status9 && viewer.Thumbnails().Count == 4 && viewer.SeriesViewPorts().Count == 4)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-10
                //Load study only have five series

                //Accession:U-ID179490

                study.SearchStudy(AccessionNo: AccessionNumber[2], Datasource: EA_91);
                study.SelectStudy("Accession", AccessionNumber[2]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //study loads into 2x3 series layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status10 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status10 && viewer.Thumbnails().Count == 5 && viewer.SeriesViewPorts().Count == 6)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();


                //Step-11
                //Load study only have six series
                //Description:Mammo Digital Screen Bilat W CAD
                //Date:27-May-2009 8:11:11 AM

                study.SearchStudy(Description: description[2], Datasource: EA_131);
                study.SelectStudy("Study Date", date[1]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //study loads into 2x3 series layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status11 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status11 && viewer.Thumbnails().Count == 6 && viewer.SeriesViewPorts().Count == 6)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-12
                //Load study only have more than six series

                //Accession:3016941.1

                study.SearchStudy(AccessionNo: AccessionNumber[3], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumber[3]);
                viewer = StudyViewer.LaunchStudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[1]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[5]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[6]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[7]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[8]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[9]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[10]));
                

                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);



                //study loads into 2x3 series layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status12 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status12 && viewer.Thumbnails().Count > 6 && viewer.SeriesViewPorts().Count == 6)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-13
                //Load study with related study, which contains only one series.

                //Accession:KHIS080105590
                study.SearchStudy(AccessionNo: AccessionNumber[4], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumber[4]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Study loaded to primary study panel as auto-layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status13 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status13 && viewer.Thumbnails().Count == 1 && viewer.SeriesViewPorts().Count == 1)
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

                //Step-14
                //Test Data-   Patient Information Panel > History Panel   
                //Open Patient Information Panel, Load the related study with one series.

                viewer.NavigateToHistoryPanel();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Date:30-Jul-1995 6:03:00 AM

                viewer.OpenPriors(new String[] { "Study Date" }, new String[] { date[2] });
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30, 1);
                PageLoadWait.WaitForAllViewportsToLoad(30, 2);

                //Study loaded to second study panel with 1x1 series layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status14 = study.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (status14 && viewer.Thumbnails(2).Count == 1 && viewer.SeriesViewPorts(2).Count == 1)
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

                //Step-15
                //Close the study viewer.
                study.CloseStudy();
                //Study viewer closes.
                ExecutedSteps++;

                //Step-16
                //Test Data-Load study with related study, which contains only two series.
                //Description:32000
                //Date:28-Feb-1998 9:58:00 AM

                //Accession:8f8941fa0


                study.SearchStudy(AccessionNo: AccessionNumber[6], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumber[6]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Study loaded to primary study panel as auto-layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status16 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status16 && viewer.Thumbnails().Count == 2 && viewer.SeriesViewPorts().Count == 2)
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

                //Step-17
                //Test Data- Patient Information Panel > History Panel   
                //Open Patient Information Panel, Load the related study with two series.

                viewer.NavigateToHistoryPanel();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Date:10-Feb-2003 4:11:29 PM

                viewer.OpenPriors(new String[] { "Study Date" }, new String[] { date[3] });
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30, 1);
                PageLoadWait.WaitForAllViewportsToLoad(30, 2);

                //Study loaded to second study panel with 1x2 series layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status17 = study.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (status17 && viewer.Thumbnails(2).Count == 2 && viewer.SeriesViewPorts(2).Count == 2)
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


                //Step-18
                //Close the study viewer.
                study.CloseStudy();
                //Study viewer closes.
                ExecutedSteps++;

                //Step-19
                //Load study with related study, which contains only three series.

                //Accession:11643936
                study.SearchStudy(AccessionNo: AccessionNumber[5], Datasource: EA_131);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                study.SelectStudy("Accession", AccessionNumber[5]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Study loaded to primary study panel as auto-layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status19 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status19 && viewer.Thumbnails().Count == 3 && viewer.SeriesViewPorts().Count == 3)
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

                //Step-20
                //Test Data-   Patient Information Panel > History Panel   
                //Open Patient Information Panel, Load the related study with three series.

                viewer.NavigateToHistoryPanel();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Date:05-Aug-2008 10:41:46 AM

                viewer.OpenPriors(new String[] { "Study Date" }, new String[] { date[4] });
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30, 1);
                PageLoadWait.WaitForAllViewportsToLoad(30, 2);

                //Study loaded to second study panel with 1x3 series layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status20 = study.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (status20 && viewer.Thumbnails(2).Count == 3 && viewer.SeriesViewPorts(2).Count == 3)
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

                //Step-21
                //Close the study viewer.
                study.CloseStudy();
                //Study viewer closes.
                ExecutedSteps++;

                //Step-22
                //Load study with related study, which contains only four series.

                //Description:Standard Screening - Combo
                //Date:09-Jul-2010 10:51:55 AM

                //Accession:05208632

                study.SearchStudy(AccessionNo: AccessionNumber[7], Datasource: EA_131);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                study.SelectStudy("Accession", AccessionNumber[7]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Study loaded to primary study panel as auto-layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status22 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status22 && viewer.Thumbnails().Count == 4 && viewer.SeriesViewPorts().Count == 4)
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

                //Step-23
                //Test Data-   Patient Information Panel > History Panel   
                //Open Patient Information Panel, Load the related study with three series.

                viewer.NavigateToHistoryPanel();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Date:26-May-2009 10:09:27 AM
                //Date:19-Nov-2012 3:32:55 PM

                viewer.OpenPriors(new String[] { "Study Date" }, new String[] { date[5] });
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30, 1);
                PageLoadWait.WaitForAllViewportsToLoad(30, 2);

                //Study loaded to second study panel with 2x2 series layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status23 = study.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (status23 && viewer.Thumbnails(2).Count == 4 && viewer.SeriesViewPorts(2).Count == 4)
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

                //Step-24
                //Close the study viewer.
                study.CloseStudy();
                //Study viewer closes.
                ExecutedSteps++;

                //Step-25

                //Load study with related study, which contains more than six series.

                //Description:ABDOMEN W/&W/O CNTRST - UMR
                //Date : 15-Jun-2006 12:44:23 PM

                //Accession:SE0000168
                ExecutedSteps = 23;
                study.SearchStudy(AccessionNo: AccessionNumber[8], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumber[8]);

                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[1]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[2]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[3]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[4]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[6]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[7]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[8]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[10]));

                //Study loaded to primary study panel as auto-layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status25 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status25 && viewer.Thumbnails().Count >= 6 && viewer.SeriesViewPorts().Count == 6)
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

                //Step-26

                //Test Data-   Patient Information Panel > History Panel   
                //Open Patient Information Panel, Load the related study with more than six series.

                viewer.NavigateToHistoryPanel();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Date:09-Jun-2003 2:09:49 PM

                viewer.OpenPriors(new String[] { "Study Date" }, new String[] { date[6] });
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30, 1);
                PageLoadWait.WaitForAllViewportsToLoad(30, 2);

                //Study loaded to second study panel with 2x3 series layout.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status26 = study.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (status26 && viewer.Thumbnails(2).Count >= 6 && viewer.SeriesViewPorts(2).Count == 6)
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

                //Step-27
                //Test Data-   Patient Information Panel > History Panel   
                //Open Patient Information Panel, Load the related study with more than six series.

                viewer.NavigateToHistoryPanel();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                
                //Date:09-Jun-2003 1:31:34 PM
                viewer.OpenPriors(new String[] { "Study Date" }, new String[] { date[7] });
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30, 1);
                PageLoadWait.WaitForAllViewportsToLoad(30, 2);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails(3)[2]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails(3)[3]));
                PageLoadWait.WaitForAllViewportsToLoad(30, 3);
                PageLoadWait.WaitForAllViewportsToLoad(30, 3);
                //Study loaded to third study panel with 2x3 series layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status27 = study.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (status27 && viewer.Thumbnails(3).Count >= 6 && viewer.SeriesViewPorts(3).Count == 6)
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

                //Step-28
                //Close the study viewer.
                study.CloseStudy();
                //Study viewer closes.
                ExecutedSteps++;

                //logout
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
        /// This Test Case is Verification of Viewer Layout Scope "Image Layout"
        /// </summary>

        public TestCaseResult Test_28038(String testid, String teststeps, int stepcount)
        {
            //-Initial Setup--
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;

            String Patient_ID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");


            try
            {
                //Step-1
                //Initial Setups are completed
                ExecutedSteps++;

                //Step-2

                //Login and Load a study with multiple series that contain multiple images.
                //Ensure the scope is set to Image. Double click one of the series and select one series view port

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                Studies study = (Studies)login.Navigate("Studies");
                //--Pre condition--
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                if (BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Selected == false)
                {
                    BasePage.Driver.FindElement(By.CssSelector("#ConnTestToolCB")).Click();
                    Logger.Instance.InfoLog("Connection tool CB selected successfully");

                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();

                //Patient_ID=454-54-5454
                study.SearchStudy(patientID: Patient_ID, Datasource: EA_131);
                study.SelectStudy("Patient ID", Patient_ID);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(9000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageScope);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                Actions action = new Actions(BasePage.Driver);
                action.DoubleClick(viewer.SeriesViewer_1X1()).Build().Perform();

                Thread.Sleep(9000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(viewer.BySeriesViewer_XxY(1, 1)));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status2 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                //Series displayed in one viewport , layout 1 series.

                if (status2 && viewer.Thumbnails().Count == 4)//&& viewer.SeriesViewPorts().Count == 1
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

                //Step-3
                //Select each available image layout.

                IWebElement ImageScope = viewer.GetReviewTool("Image Scope");
                viewer.JSMouseHover(ImageScope);

                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //Image layout is disabled after double clicking to put the display in a 1x1 series layout.

                String[] Imagelayout = { "Image Layout 2x2", "Image Layout 4x4", "Image Layout 3x3", "Image Layout 2x1", "Image Layout 1x2", "Image Layout 1x1" };
                Boolean disable3 = true;
                foreach (String s in Imagelayout)
                {
                    IWebElement ele = viewer.GetReviewToolImage(s);
                    if (ele.GetAttribute("class").Contains("disableOnCine") == false)
                    {
                        disable3 = false;
                        break;
                    }
                }
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                action = new Actions(BasePage.Driver);
                action.DoubleClick(viewer.SeriesViewer_1X1()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                Thread.Sleep(9000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                ImageScope = viewer.GetReviewTool("Image Scope");
                viewer.JSMouseHover(ImageScope);

                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                Boolean enable3 = true;
                foreach (String s in Imagelayout)
                {
                    IWebElement ele = viewer.GetReviewToolImage(s);
                    if (ele.GetAttribute("class").Contains("disableOnCine") == true)
                    {
                        enable3 = false;
                        break;
                    }
                }

                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                Boolean layout3 = true;
                foreach (String s in Imagelayout)
                {
                    String[] Format = s.Split(' ');
                    String temp1 = Regex.Replace(s, @"\s+", "");
                    viewer.SelectToolInToolBar(temp1);
                    PageLoadWait.WaitForPageLoad(15);
                    PageLoadWait.WaitForFrameLoad(15);

                    if (viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "layoutFormat").Equals(Format[2]) == false)
                    {
                        layout3 = false;
                        break;
                    }
                }

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status3 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status3 && disable3 && enable3 && layout3)
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

                //Step-4
                //Select Layout button, change image  layout to 1x2, in One viewport

                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x2);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Series displayed in 1x2 image layout.  Top left viewport is same as previous.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status4 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status4 && viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "layoutFormat").Equals("1x2"))
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

                //Step-5
                //Double click on another series thumbnail in the Thumbnail bar

                action = new Actions(BasePage.Driver);
                action.DoubleClick(viewer.Thumbnails()[1]).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //The images from the selected series are displayed in a 1x2 layout. In the same viewport selected

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status5 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status5 && viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "layoutFormat").Equals("1x2"))
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

                //Step-6
                //Select Layout button, change image layout to 3x3 in the same viewport.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout3x3);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
               
                //Series displayed in 3x3 image layout. Top left image from previous layout is displayed in one of the viewports

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status6 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status6 && viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "layoutFormat").Equals("3x3"))
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

                //Step-7
                //Double click any of the images in the 3x3 layout in the selected viewports

                action = new Actions(BasePage.Driver);
                action.DoubleClick(viewer.SeriesViewer_1X1()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_1X1()));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                PageLoadWait.WaitForAttributeInViewport(1, 1, "src", "1x1");

                //Image in the viewport is displayed in 1x1 Image layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status7 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status7 && viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "layoutFormat").Equals("1x1"))
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

                //Step-8
                //Double click on viewport.
                action = new Actions(BasePage.Driver);
                action.DoubleClick(viewer.SeriesViewer_1X1()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_1X1()));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_1X2()));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_2X1()));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_2X2()));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);


                PageLoadWait.WaitForAttributeInViewport(1, 1, "src", "3x3");

                //The images are displayed in a 3x3 Image layout.  
                //The same images that were previously displayed in the 3x3 layout are visible.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status8 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status8 && viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "layoutFormat").Equals("3x3"))
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

                //Step-9
                //Change image layout to 1x1

                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x1);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForAttributeInViewport(1, 1, "src", "1x1");

                //Image in the viewport is displayed in 1x1 layout.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status9 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status9 && viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "layoutFormat").Equals("1x1"))
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


                //Step-10
                //Change image layout to 3x3
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout3x3);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForAttributeInViewport(1, 1, "src", "3x3");

                //Series displayed in 3x3 image layout
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status10 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status10 && viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "layoutFormat").Equals("3x3"))
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

                //Step-11
                //Scroll slider to last position so that there is at least 1 empty image viewport.

                IWebElement source = viewer.ViewportScrollHandle(1, 1);
                IWebElement target = viewer.DownArrowBtn(1, 1);

                action = new Actions(BasePage.Driver);
                action.DragAndDrop(source, target).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Images/empty viewports displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status11 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status11 && viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "layoutFormat").Equals("3x3"))
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

                //Step-12
                //Use Window level tool on empty image viewport.

                IWebElement viewport1 = viewer.SeriesViewer_1X1();

                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                int h = viewport1.Size.Height;
                int w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, 2 * w / 3 + 30, 2 * h / 3 + 15, w - 30, h - 15);

                //Nothing changes

                result.steps[++ExecutedSteps].SetPath(testid + "_12_WL", ExecutedSteps + 1);
                bool status12 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status12 && viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "layoutFormat").Equals("3x3"))
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

                //Step-13
                //Use Zoom tool on empty image viewport

                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                viewer.DragandDropImage(viewport1, 2 * w / 3 + 30, 2 * h / 3 + 15, w - 30, h - 15);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                //Nothing changes.

                result.steps[++ExecutedSteps].SetPath(testid + "_13_Zoom", ExecutedSteps + 1);
                bool status13 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status13 && viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "layoutFormat").Equals("3x3"))
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

                //Step-14
                //Use Pan tool on empty image viewport

                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                viewer.DragandDropImage(viewport1, 2 * w / 3 + 30, 2 * h / 3 + 15, w - 30, h - 15);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Nothing changes.

                result.steps[++ExecutedSteps].SetPath(testid + "_14_Pan", ExecutedSteps + 1);
                bool status14 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status14 && viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "layoutFormat").Equals("3x3"))
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

                //Step-15
                //Use each measurement tool on empty image viewport.

                //Draw Line
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LineMeasurement);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                action = new Actions(BasePage.Driver);
                action.MoveToElement(viewport1, 2 * w / 3 + 30, h / 3 + 15).Click().Build().Perform();
                Thread.Sleep(3000);
                action.MoveToElement(viewport1, 2 * w / 3 + w / 6, h / 3 + h / 4).Click().Build().Perform();
                Thread.Sleep(2000);

                if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                {
                    action.Release(viewport1).Build().Perform();
                    Thread.Sleep(2000);
                }

                //HorizontalPlumbLine
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AllinOneTool);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.HorizontalPlumbLine);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                action.MoveToElement(viewport1, 2 * w / 3 + w / 6, 2 * h / 3 + 15).Click().Build().Perform();
                Thread.Sleep(2000);
                action.MoveToElement(viewport1, 2 * w / 3 + w / 6, 2 * h / 3 + 15).Click().Build().Perform();
                Thread.Sleep(2000);

                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                {
                    action.Release(viewport1).Build().Perform();
                    Thread.Sleep(2000);
                }

                //VerticalPlumbLine
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AllinOneTool);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.VerticalPlumbLine);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                action.MoveToElement(viewport1, 2 * w / 3 + w / 4, h / 3 + h / 6).Click().Build().Perform();
                Thread.Sleep(2000);
                action.MoveToElement(viewport1, 2 * w / 3 + w / 4, h / 3 + h / 6).Click().Build().Perform();
                Thread.Sleep(3000);

                if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                {
                    action.Release(viewport1).Build().Perform();
                    Thread.Sleep(2000);
                }

                //AngleMeasurement
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AllinOneTool);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.AngleMeasurement);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                action.MoveToElement(viewport1, 2 * w / 3 + w / 12, 2 * h / 3 + h / 12).Click().Build().Perform();
                Thread.Sleep(2000);
                action.MoveToElement(viewport1, 2 * w / 3 + w / 6, 2 * h / 3 + h / 6).Click().Build().Perform();
                Thread.Sleep(3000);
                action.MoveToElement(viewport1, 2 * w / 3 + w / 4, 2 * h / 3 + h / 12).Click().Build().Perform();
                Thread.Sleep(4000);

                if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                {
                    action.Release(viewport1).Build().Perform();
                    Thread.Sleep(2000);
                }

                //JointLineMeasurement
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AllinOneTool);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.JointLineMeasurement);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                action.MoveToElement(viewport1, 2 * w / 3 + w / 12, h / 3 + h / 4).Click().Build().Perform();
                Thread.Sleep(2000);
                action.MoveToElement(viewport1, 2 * w / 3 + w / 6, h / 3 + h / 6).Click().Build().Perform();
                Thread.Sleep(3000);
                action.MoveToElement(viewport1, 2 * w / 3 + w / 5, h / 3 + h / 4).Click().Build().Perform();
                Thread.Sleep(4000);

                if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("firefox"))
                {
                    action.Release(viewport1).Build().Perform();
                    Thread.Sleep(2000);
                }

                //Nothing changes.

                result.steps[++ExecutedSteps].SetPath(testid + "_15_Measurement", ExecutedSteps + 1);
                bool status15 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status15)
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

                //Step-16
                //Double click on the empty image viewport
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AllinOneTool);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(10);

                action = new Actions(BasePage.Driver);
                action.DoubleClick(viewer.SeriesViewer_1X1()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Nothing changes.

                result.steps[++ExecutedSteps].SetPath(testid + "_16_DoubleClick", ExecutedSteps + 1);
                bool status16 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status16)
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

                //Step-17
                //Close the study. and logout
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                login.CloseStudy();
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
        /// This Test Case is Verification of Viewer Layout Scope "Viewing Scope - Series or Image"
        /// </summary>

        public TestCaseResult Test_66685(String testid, String teststeps, int stepcount)
        {
            //-Initial Setup--
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;

            String Patient_IDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String[] Patient_ID = Patient_IDList.Split(':');
            String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionNo");
            String[] Accession = AccessionList.Split(':');

            try
            {
                //Step-1
                //Initial Setups create role--
                //1. From Service Tool >Viewer Tab>Protocols Tab, set Viewing Scope to 'Image' for MR modality, 
                //Apply, Restart IIS and Windows Services.


                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                Taskbar bar = new Taskbar();
                bar.Hide();
                //Pre-condition set manually, Open and close service tool first time
                try
                {
                    st.LaunchServiceTool();
                    st.NavigateToTab("Linked Scrolling");
                    wpfobject.WaitTillLoad();
                    st.RestartService();
                    wpfobject.WaitTillLoad();
                    st.CloseServiceTool();
                    wpfobject.WaitTillLoad();
                }
                catch (Exception e) { st.CloseServiceTool(); }

                st.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                st.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(2);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                var comboBox_mod = wpfobject.GetComboBox("ComboBox_Modality");
                comboBox_mod.Select("MR");
                wpfobject.WaitTillLoad();
                wpfobject.ClickRadioButtonById("RB_ViewingScopeImage");
                wpfobject.WaitTillLoad();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();
                bar.Show();

                //2. From WebAccess Domain Management Page of the domain under testing, set Viewing Scope to 'Image' for MR  
                //3. From WebAccess Role Management Pages of the domain, ensure 'Use Domain Settings' selected.

                String userId = "user_" + new Random().Next(1000); ;
                String DefaultDomain = "SuperAdminGroup";
                String DefaultRole = "SuperRole";

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SelectDomain(DefaultDomain);
                domain.ClickEditDomain();
                domain.ModalityDropDown().SelectByText("MR");
                domain.SelectRadioBtn("ScopeRadioButtons", "Image");
                domain.ClickSaveDomain();

                UserManagement usermgt = (UserManagement)login.Navigate("UserManagement");

                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermgt.DomainDropDown().SelectByText(DefaultDomain);
                usermgt.NewUsrBtn().Click();
                PageLoadWait.WaitForPageLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_sharedNewUserControl_Button1")));

                usermgt.UserIdTxtBox().SendKeys(userId);
                usermgt.LastNameTxtBox().SendKeys(userId);
                usermgt.FirstNameTxtBox().SendKeys(userId);
                usermgt.PasswordTxtBox().SendKeys(userId);
                usermgt.ConfirmPwdTxtBox().SendKeys(userId);
                usermgt.RoleDropDown().SelectByText(DefaultRole);
                PageLoadWait.WaitForPageLoad(10);
                usermgt.CreateBtn().Click();
                PageLoadWait.WaitForFrameLoad(20);

                RoleManagement role = (RoleManagement)login.Navigate("RoleManagement");

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                role.ShowRolesFromDomainDropDown().SelectByText(DefaultDomain);
                role.SelectRole(DefaultRole);
                role.ClickEditRole();

                PageLoadWait.WaitForFrameLoad(15);

                if (role.DefaultSettingPerModalityUseDomainSetting_CB().Selected == false)
                    role.DefaultSettingPerModalityUseDomainSetting_CB().Click();

                role.ClickSaveEditRole();
                login.Logout();
                ExecutedSteps++;

                //Step-2
                //Login ICA as the user with this role.  and Open a MR study.
                login.DriverGoTo(login.url);
                login.LoginIConnect(userId, userId);
                Studies study = (Studies)login.Navigate("Studies");
                //Accession:385de1da5
                study.SearchStudy(AccessionNo: Accession[0], Modality: "MR", Datasource: EA_131);
                study.SelectStudy("Accession", Accession[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();

                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Image scope is selected by default in the tool bar.
                //** have to verify color

                if (viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("toggleItem_On") == true &&
                    viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3
                //Apply W/L on the first image in a series with multiple images.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                IWebElement viewport1 = viewer.SeriesViewer_1X1();
                int h = viewport1.Size.Height;
                int w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //W/L should only be applied on the selected image, It is not applied on other images.

                result.steps[++ExecutedSteps].SetPath(testid + "_3_1_WL_Applied", ExecutedSteps +1);
                bool status3_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_3_2_WL_NextImage_NR", ExecutedSteps + 1);
                bool status3_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status3_1 && status3_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("2"))
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

                //Step-4
                //Scroll to next image, apply Zoom on the image.
                viewer.ClickDownArrowbutton(1, 1);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //Zoom should only be applied on the selected image.

                result.steps[++ExecutedSteps].SetPath(testid + "_4_1_Zoom_Applied", ExecutedSteps + 1);
                bool status4_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_4_2_Zoom_NextImage_NR", ExecutedSteps + 1);
                bool status4_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status4_1 && status4_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("4"))
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

                //Step-5
                //Scroll to next image, apply Pan on the image.
                viewer.ClickDownArrowbutton(1, 1);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //Pan should only be applied on the selected image.

                result.steps[++ExecutedSteps].SetPath(testid + "_5_1_Pan_Applied", ExecutedSteps + 1);
                bool status5_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                result.steps[ExecutedSteps].SetPath(testid + "_5_2_Pan_NextImage_NR", ExecutedSteps + 1);
                bool status5_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status5_1 && status5_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("6"))
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

                //Step-6
                //Scroll to next image, apply Rotate on the image.
                viewer.ClickDownArrowbutton(1, 1);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Pan should only be applied on the selected image.

                result.steps[++ExecutedSteps].SetPath(testid + "_6_1_Rotate_Applied", ExecutedSteps + 1);
                bool status6_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_6_2_Rotate_NextImage_NR", ExecutedSteps + 1);
                bool status6_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status6_1 && status6_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("8"))
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

                //Step-7
                //Scroll to next image, apply Flip on the image.
                viewer.ClickDownArrowbutton(1, 1);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.FlipHorizontal);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Flip should only be applied on the selected image.

                result.steps[++ExecutedSteps].SetPath(testid + "_7_1_Flip_Applied", ExecutedSteps + 1);
                bool status7_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_7_2_Flip_NextImage_NR", ExecutedSteps + 1);
                bool status7_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status7_1 && status7_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("10"))
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

                //Step-8
                //Scroll to next image, apply Grayscale inversion on the image.
                viewer.ClickDownArrowbutton(1, 1);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Grayscale inversion should only be applied on the selected image.

                result.steps[++ExecutedSteps].SetPath(testid + "_8_1_Invert_Applied", ExecutedSteps + 1);
                bool status8_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_8_2_Invert_NextImage_NR", ExecutedSteps + 1);
                bool status8_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status8_1 && status8_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("12"))
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


                //Step-9
                //Scroll to next image, apply Reset on the image.

                viewer.ClickDownArrowbutton(1, 1);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 2);

                result.steps[++ExecutedSteps].SetPath(testid + "_9_1_Pan_Applied_1", ExecutedSteps + 1);
                bool status9_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 2);

                result.steps[ExecutedSteps].SetPath(testid + "_9_1_Pan_Applied_2", ExecutedSteps + 1);
                bool status9_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //Apply Reset
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                result.steps[ExecutedSteps].SetPath(testid + "_9_3_Reset_Applied_R", ExecutedSteps + 1);
                bool status9_3 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickUpArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_9_4_PreviousImage_NR", ExecutedSteps + 1);
                bool status9_4 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status9_1 && status9_2 && status9_3 && status9_4 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("13"))
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

                //Step-10
                //1. From Role Management Page set Viewing Scope to 'Series' for MR  
                //2. Login ICA as the user with this role  
                //3. Open a MR study
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();
                login.Logout();

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                role = (RoleManagement)login.Navigate("RoleManagement");

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                role.ShowRolesFromDomainDropDown().SelectByText(DefaultDomain);
                role.SelectRole(DefaultRole);
                role.ClickEditRole();

                PageLoadWait.WaitForFrameLoad(15);

                if (role.DefaultSettingPerModalityUseDomainSetting_CB().Selected == true)
                    role.DefaultSettingPerModalityUseDomainSetting_CB().Click();

                PageLoadWait.WaitForPageLoad(15);
                role.ModalityDropDown().SelectByText("MR");
                PageLoadWait.WaitForPageLoad(15);
                role.SelectRadioBtn("ScopeRadioButtons", "Series");
                PageLoadWait.WaitForPageLoad(15);
                role.ClickSaveEditRole();
                PageLoadWait.WaitForPageLoad(15);
                login.Logout();


                login.DriverGoTo(login.url);
                login.LoginIConnect(userId, userId);

                study = (Studies)login.Navigate("Studies");
                //Accession:385de1da5  
                study.SearchStudy(AccessionNo: Accession[0], Modality: "MR", Datasource: EA_131);
                study.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();

                //Series scope is selected by default in the tool bar.

                //** have to verify color

                if (viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == true &&
                    viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("toggleItem_On") == false)
                {
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
                //Apply W/L on an image in a series with multiple images. Scroll to next image.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //W/L is applied on all images in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_11_1_WL_Applied", ExecutedSteps + 1);
                bool status11_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_11_2_WL_NextImage_R", ExecutedSteps + 1);
                bool status11_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status11_1 && status11_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("2"))
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

                //Step-12
                //1. Return to Service tool and now set the Viewing Scope to 'Series' for MR  
                //2. From Domain Management page is set Viewing Scope to 'Image' for MR 
                //3. From Role Management Page is set Viewing Scope to 'Series' for MR
                //4. Login ICA as the user with this role  
                //5. Open an MR study
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();
                login.Logout();

                st = new ServiceTool();
                wpfobject = new WpfObjects();
                bar = new Taskbar();
                bar.Hide();

                st.LaunchServiceTool();
                st.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(2);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                comboBox_mod = wpfobject.GetComboBox("ComboBox_Modality");
                comboBox_mod.Select("MR");
                wpfobject.WaitTillLoad();
                wpfobject.ClickRadioButtonById("RB_ViewingScopeSeries");
                wpfobject.WaitTillLoad();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();

                bar.Show();


                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SelectDomain(DefaultDomain);
                domain.ClickEditDomain();
                domain.ModalityDropDown().SelectByText("MR");
                domain.SelectRadioBtn("ScopeRadioButtons", "Image");
                domain.ClickSaveDomain();

                role = (RoleManagement)login.Navigate("RoleManagement");

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                role.ShowRolesFromDomainDropDown().SelectByText(DefaultDomain);
                role.SelectRole(DefaultRole);
                role.ClickEditRole();

                PageLoadWait.WaitForFrameLoad(15);

                if (role.DefaultSettingPerModalityUseDomainSetting_CB().Selected == true)
                    role.DefaultSettingPerModalityUseDomainSetting_CB().Click();

                PageLoadWait.WaitForPageLoad(15);
                role.ModalityDropDown().SelectByText("MR");
                PageLoadWait.WaitForPageLoad(15);
                role.SelectRadioBtn("ScopeRadioButtons", "Series");
                PageLoadWait.WaitForPageLoad(15);
                role.ClickSaveEditRole();
                login.Logout();


                login.DriverGoTo(login.url);
                login.LoginIConnect(userId, userId);
                study = (Studies)login.Navigate("Studies");
                //Accession:385de1da5 
                study.SearchStudy(AccessionNo: Accession[0], Modality: "MR", Datasource: EA_131);
                study.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();

                //Series scope is selected by default in the tool bar.

                //** have to verify color

                if (viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == true &&
                    viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("toggleItem_On") == false)
                {
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
                //Apply W/L on an image in a series with multiple images. Scroll to next image.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();

                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);


                //W/L is applied on all images in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_13_1_WL_Applied", ExecutedSteps + 1);
                bool status13_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_13_2_WL_NextImage_R", ExecutedSteps + 1);
                bool status13_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status13_1 && status13_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("2"))
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

                //Step-14
                //1. From iCA Domain Management page set Viewing Scope to 'Image' for MR  
                //2. From Role Management page ensure option 'Use Domain Setting'  
                //3. Login ICA as the user with this role  4. Open a MR study
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();
                login.Logout();
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SelectDomain(DefaultDomain);
                domain.ClickEditDomain();
                domain.ModalityDropDown().SelectByText("MR");
                domain.SelectRadioBtn("ScopeRadioButtons", "Image");
                domain.ClickSaveDomain();

                role = (RoleManagement)login.Navigate("RoleManagement");

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                role.ShowRolesFromDomainDropDown().SelectByText(DefaultDomain);
                role.SelectRole(DefaultRole);
                role.ClickEditRole();

                PageLoadWait.WaitForFrameLoad(15);

                if (role.DefaultSettingPerModalityUseDomainSetting_CB().Selected == false)
                    role.DefaultSettingPerModalityUseDomainSetting_CB().Click();

                role.ClickSaveEditRole();
                login.Logout();


                login.DriverGoTo(login.url);
                login.LoginIConnect(userId, userId);
                study = (Studies)login.Navigate("Studies");
                //Accession:385de1da5  Accession[0]

                study.SearchStudy(AccessionNo: Accession[0], Modality: "MR", Datasource: EA_131);
                study.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();

                //Image scope is selected in the tool bar.

                //** have to verify color

                if (viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("toggleItem_On") == true &&
                    viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == false)
                {
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
                //Apply W/L on an image in a series with multiple images.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);


                //W/L should only be applied on the selected image.

                result.steps[++ExecutedSteps].SetPath(testid + "_15_1_WL_Applied", ExecutedSteps + 1);
                bool status15_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_15_2_WL_NextImage_NR", ExecutedSteps + 1);
                bool status15_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status15_1 && status15_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("2"))
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

                //Step-16
                //1. From iCA Domain Management page, set Viewing Scope to 'Series' for MR  
                //2. From Role Management page ensure option 'Use Domain Setting' selected 
                //3. Login ICA as the user with this role  4. Open a MR study

                study.CloseStudy();
                login.Logout();

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SelectDomain(DefaultDomain);
                domain.ClickEditDomain();
                domain.ModalityDropDown().SelectByText("MR");
                domain.SelectRadioBtn("ScopeRadioButtons", "Series");
                domain.ClickSaveDomain();

                role = (RoleManagement)login.Navigate("RoleManagement");

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                role.ShowRolesFromDomainDropDown().SelectByText(DefaultDomain);
                role.SelectRole(DefaultRole);
                role.ClickEditRole();

                PageLoadWait.WaitForFrameLoad(15);

                if (role.DefaultSettingPerModalityUseDomainSetting_CB().Selected == false)
                    role.DefaultSettingPerModalityUseDomainSetting_CB().Click();

                role.ClickSaveEditRole();
                login.Logout();

                login.DriverGoTo(login.url);
                login.LoginIConnect(userId, userId);
                study = (Studies)login.Navigate("Studies");

                //Accession:385de1da5  
                study.SearchStudy(AccessionNo: Accession[0], Modality: "MR", Datasource: EA_131);
                study.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();

                //Series scope is selected by default in the tool bar.

                //** have to verify color

                if (viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == true &&
                    viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("toggleItem_On") == false)
                {
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
                //Apply W/L on an image in a series with multiple images. Scroll to next image.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);


                //W/L is applied on all images in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_17_1_WL_Applied", ExecutedSteps + 1);
                bool status17_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_17_2_WL_NextImage_R", ExecutedSteps + 1);
                bool status17_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status17_1 && status17_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("2"))
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

                //Step-18 
                //Apply the following tools on different image one at time-  * Window/Level  * Zoom 
                //* Pan  * Rotate  * Flip   * Grayscale inversion  * Reset

                //Apply Window/Level (18)
                viewer.ClickDownArrowbutton(1, 1);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //These tools are applied on all images in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_18_1_WL_Applied", ExecutedSteps + 1);
                bool status18_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_18_2_WL_NextImage_R", ExecutedSteps + 1);
                bool status18_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status18_1 && status18_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("4"))
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

                //Step-19
                //apply Zoom 
                viewer.ClickDownArrowbutton(1, 1);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //These tools are applied on all images in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_19_1_Zoom_Applied", ExecutedSteps + 1);
                bool status19_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_19_2_Zoom_NextImage_R", ExecutedSteps + 1);
                bool status19_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status19_1 && status19_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("6"))
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



                //Step-20
                //apply Rotate
                viewer.ClickDownArrowbutton(1, 1);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //These tools are applied on all images in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_20_1_Rotate_Applied", ExecutedSteps + 1);
                bool status20_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_20_2_Rotate_NextImage_R", ExecutedSteps + 1);
                bool status20_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status20_1 && status20_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("8"))
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

                //Step-21
                //apply Flip

                viewer.ClickDownArrowbutton(1, 1);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.FlipHorizontal);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //These tools are applied on all images in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_21_1_Flip_Applied", ExecutedSteps + 1);
                bool status21_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_21_2_Flip_NextImage_R", ExecutedSteps + 1);
                bool status21_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status21_1 && status21_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("10"))
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

                //Step-22
                //apply Grayscale inversion
                viewer.ClickDownArrowbutton(1, 1);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //These tools are applied on all images in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_22_1_Invert_Applied", ExecutedSteps + 1);
                bool status22_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_22_2_Invert_NextImage_R", ExecutedSteps + 1);
                bool status22_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status22_1 && status22_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("12"))
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

                //Step-23
                //apply Pan
                viewer.ClickDownArrowbutton(1, 1);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 2 * w / 4, h / 4);

                //These tools are applied on all images in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_23_1_Pan_Applied", ExecutedSteps + 1);
                bool status23_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_23_2_Pan_NextImage_R", ExecutedSteps + 1);
                bool status23_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status23_1 && status23_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("14"))
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

                //Step-24
                //apply Reset.

                viewer.ClickDownArrowbutton(1, 1);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 2);

                result.steps[++ExecutedSteps].SetPath(testid + "_24_1_Pan_Applied", ExecutedSteps + 1);
                bool status24_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //Apply Reset
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                result.steps[ExecutedSteps].SetPath(testid + "_24_2_Reset_Applied", ExecutedSteps + 1);
                bool status24_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickUpArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_24_4_PreviousImage_R", ExecutedSteps + 1);
                bool status24_3 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //These tools are applied on all images in selected series.

                if (status24_1 && status24_2 && status24_3 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("14"))
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

                //Step-25

                //1. Set Viewing Scope in Domain Management page to 'Series' for MR  
                //2. Role Management page Viewing Scope to 'Image' for MR  
                //3. Login ICA as the user with this role  4. Open a MR study.

                study.CloseStudy();
                login.Logout();

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SelectDomain(DefaultDomain);
                domain.ClickEditDomain();
                domain.ModalityDropDown().SelectByText("MR");
                domain.SelectRadioBtn("ScopeRadioButtons", "Series");
                domain.ClickSaveDomain();

                role = (RoleManagement)login.Navigate("RoleManagement");

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                role.ShowRolesFromDomainDropDown().SelectByText(DefaultDomain);
                role.SelectRole(DefaultRole);
                role.ClickEditRole();

                PageLoadWait.WaitForFrameLoad(15);

                if (role.DefaultSettingPerModalityUseDomainSetting_CB().Selected == true)
                    role.DefaultSettingPerModalityUseDomainSetting_CB().Click();

                PageLoadWait.WaitForPageLoad(15);
                role.ModalityDropDown().SelectByText("MR");
                PageLoadWait.WaitForPageLoad(15);
                role.SelectRadioBtn("ScopeRadioButtons", "Image");
                PageLoadWait.WaitForPageLoad(15);
                role.ClickSaveEditRole();
                login.Logout();

                login.DriverGoTo(login.url);
                login.LoginIConnect(userId, userId);
                study = (Studies)login.Navigate("Studies");
                //Accession:385de1da5  
                study.SearchStudy(AccessionNo: Accession[0], Modality: "MR", Datasource: EA_131);
                study.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();

                //Image scope is selected in the tool bar.

                if (viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("toggleItem_On") == true &&
                    viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == false)
                {
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
                //Apply W/L on an image in a series with multiple images.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //W/L  should only be applied on the selected image, W/L is not applied on other images in the same series.

                result.steps[++ExecutedSteps].SetPath(testid + "_26_1_WL_Applied", ExecutedSteps + 1);
                bool status26_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_26_2_WL_NextImage_NR", ExecutedSteps + 1);
                bool status26_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status26_1 && status26_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("2"))
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

                //Step-27
                //(Apply the following tools on different image one at time-  * Window/Level  * Zoom  * Pan  * Rotate  * Flip   * Grayscale inversion  * Reset)
                //Apply Window/Level

                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //A tool is only applied on the selected image in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_27_1_WL_Applied", ExecutedSteps + 1);
                bool status27_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_27_2_WL_NextImage_NR", ExecutedSteps + 1);
                bool status27_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status27_1 && status27_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("3"))
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

                //Step-28
                //apply Zoom 

                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //A tool is only applied on the selected image in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_28_1_Zoom_Applied", ExecutedSteps + 1);
                bool status28_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_28_2_Zoom_NextImage_NR", ExecutedSteps + 1);
                bool status28_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status28_1 && status28_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("4"))
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

                //Step-29
                //apply Pan

                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //A tool is only applied on the selected image in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_29_1_Pan_Applied", ExecutedSteps + 1);
                bool status29_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_29_2_Pan_NextImage_NR", ExecutedSteps + 1);
                bool status29_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status29_1 && status29_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("5"))
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
                //apply Rotate

                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //A tool is only applied on the selected image in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_30_1_Rotate_Applied", ExecutedSteps + 1);
                bool status30_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_30_2_Rotate_NextImage_NR", ExecutedSteps + 1);
                bool status30_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status30_1 && status30_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("6"))
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

                //Step-31
                //apply Flip

                viewer.SelectToolInToolBar(IEnum.ViewerTools.FlipHorizontal);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //A tool is only applied on the selected image in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_31_1_Flip_Applied", ExecutedSteps + 1);
                bool status31_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_31_2_Flip_NextImage_NR", ExecutedSteps + 1);
                bool status31_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status31_1 && status31_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("7"))
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

                //Step-32
                //apply Grayscale inversion

                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //A tool is only applied on the selected image in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_32_1_Invert_Applied", ExecutedSteps + 1);
                bool status32_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_32_2_Invert_NextImage_NR", ExecutedSteps + 1);
                bool status32_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status32_1 && status32_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("8"))
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

                //Step-33
                //apply Reset.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 2);

                result.steps[++ExecutedSteps].SetPath(testid + "_33_1_Pan_Applied_1", ExecutedSteps + 1);
                bool status33_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 2);

                result.steps[ExecutedSteps].SetPath(testid + "_33_2_Pan_Applied_2", ExecutedSteps + 1);
                bool status33_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //Apply Reset
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                result.steps[ExecutedSteps].SetPath(testid + "_33_3_Reset_Applied_R", ExecutedSteps + 1);
                bool status33_3 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickUpArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_33_4_PreviousImage_NR", ExecutedSteps + 1);
                bool status33_4 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //A tool is only applied on the selected image in selected series.

                if (status33_1 && status33_2 && status33_3 && status33_4 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("8"))
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

                //Step-34
                //Open User Preferences and change Viewing Scope to 'Series' for MR, 
                //re-open the same MR or open a MR study.

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);

                userpref.ModalityDropDown().SelectByText("MR");
                userpref.SelectRadioBtn("ScopeRadioButtons", "Series");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);

                study = (Studies)login.Navigate("Studies");
                //Accession:385de1da5  
                study.SearchStudy(AccessionNo: Accession[0], Modality: "MR", Datasource: EA_131);
                study.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();

                //Series scope is selected by default in the tool bar.

                if (viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == true &&
                    viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("toggleItem_On") == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-35
                //Apply W/L on an image in a series with multiple images. Scroll to next image.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //W/L is applied on all images in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_35_1_WL_Applied", ExecutedSteps + 1);
                bool status35_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_35_2_WL_NextImage_R", ExecutedSteps + 1);
                bool status35_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status35_1 && status35_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("2"))
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

                //Step-36
                //1. Set Viewing Scope in Domain Management page to 'Series' for MR
                //2. Role Management page Viewing Scope to 'Series' for MR.
                //3. Login ICA as  the user with this role  
                //4. Open User Preferences and change Viewing Scope to 'Image' for MR
                //5. Open a MR study

                study.CloseStudy();
                login.Logout();

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SelectDomain(DefaultDomain);
                domain.ClickEditDomain();
                domain.ModalityDropDown().SelectByText("MR");
                domain.SelectRadioBtn("ScopeRadioButtons", "Series");
                domain.ClickSaveDomain();

                role = (RoleManagement)login.Navigate("RoleManagement");

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                role.ShowRolesFromDomainDropDown().SelectByText(DefaultDomain);
                role.SelectRole(DefaultRole);
                role.ClickEditRole();

                PageLoadWait.WaitForFrameLoad(15);
                if (role.DefaultSettingPerModalityUseDomainSetting_CB().Selected == true)
                    role.DefaultSettingPerModalityUseDomainSetting_CB().Click();

                PageLoadWait.WaitForPageLoad(15);
                role.ModalityDropDown().SelectByText("MR");
                PageLoadWait.WaitForPageLoad(15);
                role.SelectRadioBtn("ScopeRadioButtons", "Series");
                PageLoadWait.WaitForPageLoad(15);
                role.ClickSaveEditRole();
                login.Logout();

                login.DriverGoTo(login.url);
                login.LoginIConnect(userId, userId);

                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.ModalityDropDown().SelectByText("MR");
                userpref.SelectRadioBtn("ScopeRadioButtons", "Image");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);

                study = (Studies)login.Navigate("Studies");
                //Accession:385de1da5  
                study.SearchStudy(AccessionNo: Accession[0], Modality: "MR", Datasource: EA_131);
                study.SelectStudy("Accession", Accession[0]);
                viewer = StudyViewer.LaunchStudy();

                //Image scope is selected in the tool bar.

                if (viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("toggleItem_On") == true &&
                    viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-37
                //Apply W/L on an image in a series with multiple images.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //W/L should only be applied on the selected image, not applied on other images in the same series.

                result.steps[++ExecutedSteps].SetPath(testid + "_37_1_WL_Applied", ExecutedSteps + 1);
                bool status37_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_37_2_WL_NextImage_NR", ExecutedSteps + 1);
                bool status37_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status37_1 && status37_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("2"))
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

                //Step-38
                //Load a different MR study with multiple series.
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                study = (Studies)login.Navigate("Studies");
                //Accession:U-ID179490 
                study.SearchStudy(AccessionNo: Accession[1], Modality: "MR", Datasource: EA_91);
                study.SelectStudy("Accession", Accession[1]);
                viewer = StudyViewer.LaunchStudy();

                //Image scope is selected in the tool bar.

                if (viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("toggleItem_On") == true &&
                    viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-39
                //Apply W/L on an image in a series with multiple images.  
                //Select a different series with multiple images and then apply W/L on an image.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //W/L  should only be applied on the selected image in selected series, not applied on other images in the same series.

                result.steps[++ExecutedSteps].SetPath(testid + "_39_1_WL_Applied", ExecutedSteps + 1);
                bool status39_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_39_2_WL_NextImage_NR", ExecutedSteps + 1);
                bool status39_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status37_1 && status37_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("2"))
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

                //Step-40
                //(Apply the following tools on different image/series one at time-  * Window/Level  * Zoom  * Pan  * Rotate  * Flip   * Grayscale inversion  * Reset)
                //Apply Window/Level

                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //A tool is only applied on the selected image in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_40_1_WL_Applied", ExecutedSteps + 1);
                bool status40_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_40_2_WL_NextImage_NR", ExecutedSteps + 1);
                bool status40_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status40_1 && status40_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("3"))
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

                //Step-41
                //apply Zoom 

                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //A tool is only applied on the selected image in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_41_1_Zoom_Applied", ExecutedSteps + 1);
                bool status41_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_41_2_Zoom_NextImage_NR", ExecutedSteps + 1);
                bool status41_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status41_1 && status41_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("4"))
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

                //Step-42
                //apply Pan

                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //A tool is only applied on the selected image in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_42_1_Pan_Applied", ExecutedSteps + 1);
                bool status42_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                result.steps[ExecutedSteps].SetPath(testid + "_42_2_Pan_NextImage_NR", ExecutedSteps + 1);
                bool status42_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status42_1 && status42_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("5"))
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

                //Step-43
                //apply Rotate
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //A tool is only applied on the selected image in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_43_1_Rotate_Applied", ExecutedSteps + 1);
                bool status43_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_43_2_Rotate_NextImage_NR", ExecutedSteps + 1);
                bool status43_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status43_1 && status43_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("6"))
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

                //Step-44
                //apply Flip
                viewer.SelectToolInToolBar(IEnum.ViewerTools.FlipHorizontal);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //A tool is only applied on the selected image in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_44_1_Flip_Applied", ExecutedSteps + 1);
                bool status44_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_44_2_Flip_NextImage_NR", ExecutedSteps + 1);
                bool status44_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status44_1 && status44_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("7"))
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

                //Step-45
                //apply Grayscale inversion
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //A tool is only applied on the selected image in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_45_1_Invert_Applied", ExecutedSteps + 1);
                bool status45_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_45_2_Invert_NextImage_NR", ExecutedSteps + 1);
                bool status45_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status45_1 && status45_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("8"))
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

                //Step-46
                //apply Reset.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 2);

                result.steps[++ExecutedSteps].SetPath(testid + "_46_1_Pan_Applied_1", ExecutedSteps + 1);
                bool status46_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;
                viewer.DragandDropImage(viewport1, w / 4, h / 4, w / 2, h / 2);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                result.steps[ExecutedSteps].SetPath(testid + "_46_2_Pan_Applied_2", ExecutedSteps + 1);
                bool status46_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //Apply Reset
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                result.steps[ExecutedSteps].SetPath(testid + "_46_3_Reset_Applied_R", ExecutedSteps + 1);
                bool status46_3 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickUpArrowbutton(1, 1);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                result.steps[ExecutedSteps].SetPath(testid + "_46_4_PreviousImage_NR", ExecutedSteps + 1);
                bool status46_4 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                //A tool is only applied on the selected image in selected series.

                if (status46_1 && status46_2 && status46_3 && status46_4 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("8"))
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

                //Step-47
                //Open User Preferences and change Viewing Scope to 'Image' for CT, open a CT study.

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);

                userpref.ModalityDropDown().SelectByText("CT");
                userpref.SelectRadioBtn("ScopeRadioButtons", "Image");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);

                study = (Studies)login.Navigate("Studies");
                //Patient ID:454-54-5454   
                study.SearchStudy(patientID: Patient_ID[0], Modality: "CT", Datasource: EA_131);
                study.SelectStudy("Patient ID", Patient_ID[0]);
                viewer = StudyViewer.LaunchStudy();

                //Image scope is selected in the tool bar.

                if (viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("toggleItem_On") == true &&
                    viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-48
                //Apply W/L on an image in a series with multiple images.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                IWebElement viewport2 = viewer.SeriesViewer_1X2();
                h = viewport2.Size.Height;
                w = viewport2.Size.Width;

                viewer.DragandDropImage(viewport2, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //W/L  should only be applied on the selected image,not applied on other images in the same series.

                result.steps[++ExecutedSteps].SetPath(testid + "_48_1_WL_Applied", ExecutedSteps + 1);
                bool status48_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                viewer.ClickDownArrowbutton(1, 2);

                result.steps[ExecutedSteps].SetPath(testid + "_48_2_WL_NextImage_NR", ExecutedSteps + 1);
                bool status48_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                if (status48_1 && status48_2 &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("2"))
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

                //Step-49
                //(Apply the following tools on different image/series one at a time-  * Window/Level  * Zoom  * Pan  * Rotate  * Flip   * Grayscale inversion  * Reset).
                //Apply Window/Level

                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport2 = viewer.SeriesViewer_1X2();
                h = viewport2.Size.Height;
                w = viewport2.Size.Width;

                viewer.DragandDropImage(viewport2, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //A tool is only applied on the selected image in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_49_1_WL_Applied", ExecutedSteps + 1);
                bool status49_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                viewer.ClickDownArrowbutton(1, 2);

                result.steps[ExecutedSteps].SetPath(testid + "_49_2_WL_NextImage_NR", ExecutedSteps + 1);
                bool status49_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                if (status49_1 && status49_2 &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("3"))
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

                //Step-50
                //apply Zoom 
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport2 = viewer.SeriesViewer_1X2();
                h = viewport2.Size.Height;
                w = viewport2.Size.Width;

                viewer.DragandDropImage(viewport2, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //A tool is only applied on the selected image in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_50_1_Zoom_Applied", ExecutedSteps + 1);
                bool status50_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                viewer.ClickDownArrowbutton(1, 2);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                result.steps[ExecutedSteps].SetPath(testid + "_50_2_Zoom_NextImage_NR", ExecutedSteps + 1);
                bool status50_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                if (status50_1 && status50_2 &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("4"))
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

                //Step-51
                //apply Pan
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                viewport2 = viewer.SeriesViewer_1X2();
                h = viewport2.Size.Height;
                w = viewport2.Size.Width;

                viewer.DragandDropImage(viewport2, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //A tool is only applied on the selected image in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_51_1_Pan_Applied", ExecutedSteps + 1);
                bool status51_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                viewer.ClickDownArrowbutton(1, 2);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                result.steps[ExecutedSteps].SetPath(testid + "_51_2_Pan_NextImage_NR", ExecutedSteps + 1);
                bool status51_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                if (status51_1 && status51_2 &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("5"))
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

                //Step-52
                //apply Rotate
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //A tool is only applied on the selected image in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_52_1_Rotate_Applied", ExecutedSteps + 1);
                bool status52_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                viewer.ClickDownArrowbutton(1, 2);

                result.steps[ExecutedSteps].SetPath(testid + "_52_2_Rotate_NextImage_NR", ExecutedSteps + 1);
                bool status52_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                if (status52_1 && status52_2 &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("6"))
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

                //Step-53
                //apply Flip
                viewer.SelectToolInToolBar(IEnum.ViewerTools.FlipHorizontal);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //A tool is only applied on the selected image in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_53_1_Flip_Applied", ExecutedSteps + 1);
                bool status53_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                viewer.ClickDownArrowbutton(1, 2);

                result.steps[ExecutedSteps].SetPath(testid + "_53_2_Flip_NextImage_NR", ExecutedSteps + 1);
                bool status53_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                if (status53_1 && status53_2 &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("7"))
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

                //Step-54
                //apply Grayscale inversion
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //A tool is only applied on the selected image in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_54_1_Invert_Applied", ExecutedSteps + 1);
                bool status54_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                viewer.ClickDownArrowbutton(1, 2);

                result.steps[ExecutedSteps].SetPath(testid + "_54_2_Invert_NextImage_NR", ExecutedSteps + 1);
                bool status54_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                if (status54_1 && status54_2 &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("8"))
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

                //Step-55
                //apply Reset.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport2 = viewer.SeriesViewer_1X2();
                h = viewport2.Size.Height;
                w = viewport2.Size.Width;
                viewer.DragandDropImage(viewport2, w / 4, h / 4, w / 2, h / 2);

                result.steps[++ExecutedSteps].SetPath(testid + "_55_1_Pan_Applied_1", ExecutedSteps + 1);
                bool status55_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                viewer.ClickDownArrowbutton(1, 2);

                viewport2 = viewer.SeriesViewer_1X2();
                h = viewport2.Size.Height;
                w = viewport2.Size.Width;
                viewer.DragandDropImage(viewport2, w / 4, h / 4, w / 2, h / 2);

                result.steps[ExecutedSteps].SetPath(testid + "_55_2_Pan_Applied_2", ExecutedSteps + 1);
                bool status55_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                //Apply Reset
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                result.steps[ExecutedSteps].SetPath(testid + "_55_3_Reset_Applied_R", ExecutedSteps + 1);
                bool status55_3 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                viewer.ClickUpArrowbutton(1, 2);

                result.steps[ExecutedSteps].SetPath(testid + "_55_4_PreviousImage_NR", ExecutedSteps + 1);
                bool status55_4 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                //A tool is only applied on the selected image in selected series.

                if (status55_1 && status55_2 && status55_3 && status55_4 &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("8"))
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

                //Step-56
                //Open User Preferences and change Viewing Scope to"Series' for CT, open a CT study.

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);

                userpref.ModalityDropDown().SelectByText("CT");
                userpref.SelectRadioBtn("ScopeRadioButtons", "Series");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);

                study = (Studies)login.Navigate("Studies");
                //Patient ID:454-54-5454   
                study.SearchStudy(patientID: Patient_ID[0], Modality: "CT", Datasource: EA_131);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(40);
                study.SelectStudy("Patient ID", Patient_ID[0]);
                viewer = StudyViewer.LaunchStudy();

                //Series Scope is selected in the tool bar.

                if (viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == true &&
                    viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("toggleItem_On") == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-57
                //(Apply the following tools on different image/series one at a time-  * Window/Level  * Zoom  * Pan  * Rotate  * Flip   * Grayscale inversion  * Reset).
                //Apply Window/Level

                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport2 = viewer.SeriesViewer_1X2();
                h = viewport2.Size.Height;
                w = viewport2.Size.Width;

                viewer.DragandDropImage(viewport2, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //These tools are applied on all images in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_57_1_WL_Applied", ExecutedSteps + 1);
                bool status57_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                viewer.ClickDownArrowbutton(1, 2);

                result.steps[ExecutedSteps].SetPath(testid + "_57_2_WL_NextImage_R", ExecutedSteps + 1);
                bool status57_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                if (status57_1 && status57_2 &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("2"))
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

                //Step-58
                //apply Zoom 
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                viewport2 = viewer.SeriesViewer_1X2();
                h = viewport2.Size.Height;
                w = viewport2.Size.Width;

                viewer.DragandDropImage(viewport2, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //These tools are applied on all images in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_58_1_Zoom_Applied", ExecutedSteps + 1);
                bool status58_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                viewer.ClickDownArrowbutton(1, 2);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                result.steps[ExecutedSteps].SetPath(testid + "_58_2_Zoom_NextImage_R", ExecutedSteps + 1);
                bool status58_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                if (status58_1 && status58_2 &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("3"))
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

                //Step-59
                //apply Rotate
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //These tools are applied on all images in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_59_1_Rotate_Applied", ExecutedSteps + 1);
                bool status59_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                viewer.ClickDownArrowbutton(1, 2);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                result.steps[ExecutedSteps].SetPath(testid + "_59_2_Rotate_NextImage_R", ExecutedSteps + 1);
                bool status59_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                if (status59_1 && status59_2 &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("4"))
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

                //Step-60
                //apply Flip
                viewer.SelectToolInToolBar(IEnum.ViewerTools.FlipHorizontal);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //These tools are applied on all images in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_60_1_Flip_Applied", ExecutedSteps + 1);
                bool status60_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                viewer.ClickDownArrowbutton(1, 2);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                result.steps[ExecutedSteps].SetPath(testid + "_60_2_Flip_NextImage_R", ExecutedSteps + 1);
                bool status60_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                if (status60_1 && status60_2 &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("5"))
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

                //Step-61
                //apply Grayscale inversion
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
            

                //These tools are applied on all images in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_61_1_Invert_Applied", ExecutedSteps + 1);
                bool status61_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                viewer.ClickDownArrowbutton(1, 2);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                result.steps[ExecutedSteps].SetPath(testid + "_61_2_Invert_NextImage_R", ExecutedSteps + 1);
                bool status61_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                if (status61_1 && status61_2 &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("6"))
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

                //Step-62
                //apply Pan
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport2 = viewer.SeriesViewer_1X2();
                h = viewport2.Size.Height;
                w = viewport2.Size.Width;

                viewer.DragandDropImage(viewport2, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //These tools are applied on all images in selected series.

                result.steps[++ExecutedSteps].SetPath(testid + "_62_1_Pan_Applied", ExecutedSteps + 1);
                bool status62_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                viewer.ClickDownArrowbutton(1, 2);

                result.steps[ExecutedSteps].SetPath(testid + "_62_2_Pan_NextImage_R", ExecutedSteps + 1);
                bool status62_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                if (status62_1 && status62_2 &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("7"))
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

                //Step-63
                //apply Reset.
                viewport2 = viewer.SeriesViewer_1X2();
                h = viewport2.Size.Height;
                w = viewport2.Size.Width;
                viewer.DragandDropImage(viewport2, w / 4, h / 4, w / 2, h / 2);

                result.steps[++ExecutedSteps].SetPath(testid + "_63_1_Pan_Applied", ExecutedSteps + 1);
                bool status63_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                //Apply Reset
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                result.steps[ExecutedSteps].SetPath(testid + "_63_2_Reset_Applied_R", ExecutedSteps + 1);
                bool status63_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                viewer.ClickUpArrowbutton(1, 2);

                result.steps[ExecutedSteps].SetPath(testid + "_63_3_PreviousImage_R", ExecutedSteps + 1);
                bool status63_3 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X2());

                //These tools are applied on all images in selected series.

                if (status63_1 && status63_2 && status63_3 &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("6"))
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

                //Step-64
                //Open User Preferences and change Viewing Scope to 'Image' for NM, PT, RF, XA. 
                //Load NM study and apply a tool on an image in a series with multiple images. 
                //(Repeat this step for PT studies, RF studies,  and XA studies.)

                String[] modality = { "NM", "PT", "RF", "XA" };

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                foreach (string s in modality)
                {
                    userpref.ModalityDropDown().SelectByText(s);
                    PageLoadWait.WaitForPageLoad(20);
                    userpref.SelectRadioBtn("ScopeRadioButtons", "Image");
                    PageLoadWait.WaitForPageLoad(20);
                }
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);

                //Accession[2]:3453451 
                study.SearchStudy(AccessionNo: Accession[2], Modality: "NM", Datasource: EA_131);
                study.SelectStudy("Accession", Accession[2]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //Image scope is selected in the tool bar. The tool should only be applied on the selected image.

                result.steps[++ExecutedSteps].SetPath(testid + "_64_1_WL_Applied", ExecutedSteps + 1);
                bool status64_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_64_2_WL_NextImage_NR", ExecutedSteps + 1);
                bool status64_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status64_1 && status64_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("2") &&
                    viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("toggleItem_On") == true &&
                    viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == false)
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

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-65
                //Load PT study & apply a tool on an image in a series with multiple images
                //Accession[3]:537092 

                study.SearchStudy(AccessionNo: Accession[3], Modality: "PT", Datasource: EA_131);
                study.SelectStudy("Accession", Accession[3]);
                viewer = StudyViewer.LaunchStudy();
                
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //Image scope is selected in the tool bar. The tool should only be applied on the selected image.

                result.steps[++ExecutedSteps].SetPath(testid + "_65_1_WL_Applied", ExecutedSteps + 1);
                bool status65_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_65_2_WL_NextImage_NR", ExecutedSteps + 1);
                bool status65_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status65_1 && status65_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("2") &&
                    viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("toggleItem_On") == true &&
                    viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == false)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-66
                //Load RF study & apply a tool on an image in a series with multiple images
                //Accession[4]:25697752 
                study.SearchStudy(AccessionNo: Accession[4], Modality: "RF", Datasource: EA_131);
                study.SelectStudy("Accession", Accession[4]);
                viewer = StudyViewer.LaunchStudy();

                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //Image scope is selected in the tool bar. The tool should only be applied on the selected image.

                result.steps[++ExecutedSteps].SetPath(testid + "_66_1_WL_Applied", ExecutedSteps + 1);
                bool status66_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_66_2_WL_NextImage_NR", ExecutedSteps + 1);
                bool status66_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status66_1 && status66_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("2") &&
                    viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("toggleItem_On") == true &&
                    viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == false)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-67
                //Load XA study & apply a tool on an image in a series with multiple images
                // Patient_ID[1]:PRP000030
                study.SearchStudy(patientID: Patient_ID[1], Modality: "XA", Datasource: EA_131);
                study.SelectStudy("Patient ID", Patient_ID[1]);
                viewer = StudyViewer.LaunchStudy();
                Thread.Sleep(5000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                viewer.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //Image scope is selected in the tool bar. The tool should only be applied on the selected image.

                result.steps[++ExecutedSteps].SetPath(testid + "_67_1_WL_Applied", ExecutedSteps + 1);
                bool status67_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_67_2_WL_NextImage_NR", ExecutedSteps + 1);
                bool status67_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status67_1 && status67_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("2") &&
                    viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("toggleItem_On") == true &&
                    viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == false)
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


                //Step-68
                //Open User Preferences and change Viewing Scope to"Series' for NM, PT, RF, XA, Load NM study 
                //and apply a tool on an image in a series with multiple images. :
                //(Repeat this step for PT studies,; Rfstudies; and XA studies.)

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                foreach (string s in modality)
                {
                    userpref.ModalityDropDown().SelectByText(s);
                    PageLoadWait.WaitForPageLoad(20);
                    userpref.SelectRadioBtn("ScopeRadioButtons", "Series");
                    PageLoadWait.WaitForPageLoad(20);
                }
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                //Accession[2]:3453451 
                study.SearchStudy(AccessionNo: Accession[2], Modality: "NM", Datasource: EA_131);
                study.SelectStudy("Accession", Accession[2]);
                viewer = StudyViewer.LaunchStudy();
               
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //Series Scope is selected in the tool bar. The tool should be applied on all images.

                result.steps[++ExecutedSteps].SetPath(testid + "_68_1_WL_Applied", ExecutedSteps + 1);
                bool status68_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_68_2_WL_NextImage_R", ExecutedSteps + 1);
                bool status68_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status68_1 && status68_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("2") &&
                    viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == true &&
                    viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("toggleItem_On") == false)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-69
                //Load PT study & apply a tool on an image in a series with multiple images
                //Accession[3]:537092 
                study.SearchStudy(AccessionNo: Accession[3], Modality: "PT", Datasource: EA_131);
                study.SelectStudy("Accession", Accession[3]);
                viewer = StudyViewer.LaunchStudy();

                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //Series Scope is selected in the tool bar. The tool should be applied on all images.

                result.steps[++ExecutedSteps].SetPath(testid + "_69_1_WL_Applied", ExecutedSteps + 1);
                bool status69_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_69_2_WL_NextImage_R", ExecutedSteps + 1);
                bool status69_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status69_1 && status69_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("2") &&
                    viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == true &&
                    viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("toggleItem_On") == false)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-70
                //Load RF study & apply a tool on an image in a series with multiple images
                //Accession[4]:25697752 
                study.SearchStudy(AccessionNo: Accession[4], Modality: "RF", Datasource: EA_131);
                study.SelectStudy("Accession", Accession[4]);
                viewer = StudyViewer.LaunchStudy();

                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //Series Scope is selected in the tool bar. The tool should be applied on all images.

                result.steps[++ExecutedSteps].SetPath(testid + "_70_1_WL_Applied", ExecutedSteps + 1);
                bool status70_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_70_2_WL_NextImage_R", ExecutedSteps + 1);
                bool status70_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status70_1 && status70_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("2") &&
                    viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == true &&
                    viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("toggleItem_On") == false)
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Step-71
                //Load XA study & apply a tool on an image in a series with multiple images
                // Patient_ID[1]:PRP000030
                study.SearchStudy(patientID: Patient_ID[1], Modality: "XA", Datasource: EA_131);
                study.SelectStudy("Patient ID", Patient_ID[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewport1 = viewer.SeriesViewer_1X1();
                h = viewport1.Size.Height;
                w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);

                //Series Scope is selected in the tool bar. The tool should be applied on all images.

                result.steps[++ExecutedSteps].SetPath(testid + "_71_1_WL_Applied", ExecutedSteps + 1);
                bool status71_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                viewer.ClickDownArrowbutton(1, 1);

                result.steps[ExecutedSteps].SetPath(testid + "_71_2_WL_NextImage_R", ExecutedSteps + 1);
                bool status71_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status71_1 && status71_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("2") &&
                    viewer.GetReviewToolImage("Series Scope").GetAttribute("class").Contains("toggleItem_On") == true &&
                    viewer.GetReviewToolImage("Image Scope").GetAttribute("class").Contains("toggleItem_On") == false)
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

                //Step-72
                //Close the study. and logout
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();
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
    }
}
