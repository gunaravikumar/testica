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
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using Selenium.Scripts.Pages.MergeServiceTool;
using TestStack.White.Configuration;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;
using Application = TestStack.White.Application;
using System.Diagnostics;
using System.Xml.Serialization;
using OpenQA.Selenium.Remote;
using Selenium.Scripts.Pages.eHR;

namespace Selenium.Scripts.Tests
{
    class Localizer : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public EHR ehr { get; set; }
        public Web_Uploader webuploader { get; set; }
        public RanorexObjects rnxobject { get; set; }
        DirectoryInfo CurrentDir = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
        string FolderPath = "";
        String User1 = "User1_" + new Random().Next(1, 10000);

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public Localizer(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            FolderPath = Config.downloadpath;//CurrentDir.Parent.Parent.FullName + "\\Downloads\\";
            ei = new ExamImporter();
            mpaclogin = new MpacLogin();
            wpfobject = new WpfObjects();
            rnxobject = new RanorexObjects();
            webuploader = new Web_Uploader();
            ehr = new EHR();
        }

        /// <summary>
        /// Localizer - Initial Setups and Pre-Conditions
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27997(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            UserManagement usermanagement;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;

                //Step-1: Client browser should be as per FPS - Taken care during automation run
                ExecutedSteps++;

                //Step-2: ICA should be installed with sufficient datasets - Part of Environment setup
                ExecutedSteps++;

                //Step-3: Install Dicom toolbox - Marked NA since installing third party application
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-4: Configuring data sources - Part of environment setup
                ExecutedSteps++;

                //Step-5: Create a standard user
                login.LoginIConnect(username, password);
                //Create User
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(User1, DomainName, RoleName);
                login.Logout();
                ExecutedSteps++;

                //Step-6: Date/Time formats are in the US English culture is assumed - Instruction, not an actual step
                ExecutedSteps++;


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
        }

        /// <summary>
        /// Localizer - 1.0 Localizer Line(s) - Series viewer/Image Layout
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27998(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            Studies studies = null;
            StudyViewer StudyVw;
            Viewer viewer = new Viewer();
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                String PatientName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");

                String[] PatName = PatientName.Split(':');
                String[] AccessionID = AccessionIDList.Split(':');

                //Step-1: Initial setup
                ExecutedSteps++;

                //Step-2: Load Study: Tumor, Left Forearm. Set the view to 2 series
                login.LoginIConnect(User1, User1);
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.ChooseColumns(new string[] { "Patient Name", "Accession" });
                studies.SearchStudy(LastName: PatName[0].Trim().Split(',')[0], AccessionNo: AccessionID[0]);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.SelectStudy("Accession", AccessionID[0]);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);
                //Set series view to 2 series
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step2 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step2)
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

                //Step-3: Load related series that have different scan plane into the two viewers.
                StudyVw.DragThumbnailToViewport(3, StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                StudyVw.DragThumbnailToViewport(4, StudyVw.SeriesViewer_1X2().GetAttribute("id"));
                Thread.Sleep(3000);
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step3 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step3)
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

                //Step-4: Select one of the series and toggle on the localizer line.
                StudyVw.DragThumbnailToViewport(1, StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                StudyVw.DragThumbnailToViewport(2, StudyVw.SeriesViewer_1X2().GetAttribute("id"));
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                //Select localizer line
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                //Big thread.Sleep for 1st time 
                Thread.Sleep(10000); 
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                String LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step4 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag == "true" && step4)
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

                //Step-5: Scroll through the selected series on the Reference viewer.
                StudyVw.DragThumbnailToViewport(1, StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                StudyVw.DragThumbnailToViewport(2, StudyVw.SeriesViewer_1X2().GetAttribute("id"));
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                //Scroll 
                for (int i = 0; i < 3; i++)
                {
                    StudyVw.ClickDownArrowbutton(1, 1);
                }
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step5 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step5)
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
                //Step-6: Select the series that the localizer line is displayed on and scroll 
                StudyVw.Click("id", StudyVw.SeriesViewer_1X2().GetAttribute("id"));
                //Scroll 
                for (int i = 0; i < 3; i++)
                {
                    StudyVw.ClickDownArrowbutton(1, 2);
                }
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X1(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step6 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag == "true" && step6)
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

                //Step-7: Missing
                //Step-8: Change Image layout to 1x2
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x2);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step8 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step8)
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
                //Step-9: Change Image layout to 2x1
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x1);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step9 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step9)
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

                //Step-10: Change Image layout to 2x2
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step10 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step10)
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

                //Step-11: Change Image layout to 3x3
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout3x3);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step11 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step11)
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

                //Step-12: Change Image layout to 4x4
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout4x4);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step12 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step12)
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

                //Step-13: Change Image layout back to 1x1
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x1);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step13 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step13)
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

                //Step-14: Toggle Off Localizer
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X1(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step14 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag == "false" && step14)
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

                //Step-15: Select the series that the localizer line was displayed on and toggle on the localizer line.
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step15 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag == "true" && step15)
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

                //Step-16: Scroll through the selected series.
                //Scroll 
                for (int i = 0; i < 3; i++)
                {
                    StudyVw.ClickDownArrowbutton(1, 1);
                }
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step16 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step16)
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

                //Step-17: Set the view to 4 series.
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step17 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step17)
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

                //Step-18: Previously displayed 2 series viewers and localizer mode (ON) are kept
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn");
                if (LocalizerFlag == "true")
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

                //Step-19: Load related series to viewport
                StudyVw.DragThumbnailToViewport(5, StudyVw.SeriesViewer_2X2().GetAttribute("id"));
                Thread.Sleep(3000);
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step19 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step19)
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

                //Step-20: Click on a diff series - Click 1x2
                StudyVw.Click("id", StudyVw.SeriesViewer_1X2().GetAttribute("id"));
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step20 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step20)
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

                //Step-21: Scroll through the series
                //Scroll 
                for (int i = 0; i < 3; i++)
                {
                    StudyVw.ClickDownArrowbutton(1, 2);
                }
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step21 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step21)
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

                //Step-22: Change layout to 1x2
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x2);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step22 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step22)
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
                //Step-23: Change Image layout to 2x1
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x1);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step23 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step23)
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

                //Step-24: Change Image layout to 2x2
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step24 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step24)
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

                //Step-25: Change Image layout to 3x3
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout3x3);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step25 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step25)
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

                //Step-26: Change Image layout to 4x4
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout4x4);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step26 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step26)
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

                //Step-27: Change Image layout back to 1x1
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x1);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step27)
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
                //Step-28: Toggle Off Localizer
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X1(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step28 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag == "false" && step28)
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

                //Step-29: Select the series that the localizer line was displayed on and toggle on the localizer line.
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step29 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag == "true" && step29)
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

                //Step-30: Scroll through the selected series.
                //Scroll 
                for (int i = 0; i < 3; i++)
                {
                    StudyVw.ClickDownArrowbutton(1, 1);
                }
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step30 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step30)
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

                //Step-31: Set the view to 6 series.
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step31 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step31)
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

                //Step-32: Previously displayed 4 series viewers and localizer mode (ON) are kept
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn");
                String LocalizerFlag2 = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_2X1(), "src", '&', "ToggleLocalizerOn");
                String LocalizerFlag3 = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_2X2(), "src", '&', "ToggleLocalizerOn");
                if (LocalizerFlag == "true" && LocalizerFlag2 == "true" && LocalizerFlag3 == "true")
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

                //Step-33: Load related series to viewport
                StudyVw.DragThumbnailToViewport(2, StudyVw.SeriesViewer_2X3().GetAttribute("id"));
                Thread.Sleep(3000);
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step33 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step33)
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

                //Step-34: Click on a diff series - Click 1x2
                StudyVw.Click("id", StudyVw.SeriesViewer_1X2().GetAttribute("id"));
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step34 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step34)
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

                //Step-35: Scroll through the series
                //Scroll 
                for (int i = 0; i < 3; i++)
                {
                    StudyVw.ClickDownArrowbutton(1, 2);
                }
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step35 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step35)
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

                //Step-36: Change layout to 1x2
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x2);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step36 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step36)
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
                //Step-37: Change Image layout to 2x1
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x1);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step37 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step37)
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

                //Step-38: Change Image layout to 2x2
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step38 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step38)
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

                //Step-39: Change Image layout to 3x3
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout3x3);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step39 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step39)
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

                //Step-40: Change Image layout to 4x4
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout4x4);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step40 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step40)
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

                //Step-41: Change Image layout back to 1x1
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x1);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step41 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step41)
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

                //Step-42: Toggle Off Localizer
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X1(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step42 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag == "false" && step42)
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

                //Step-43: Select the series that the localizer line was displayed on and toggle on the localizer line.
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step43 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag == "true" && step43)
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

                //Step-44: Scroll through the selected series.
                //Scroll 
                for (int i = 0; i < 3; i++)
                {
                    StudyVw.ClickDownArrowbutton(1, 1);
                }
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step44 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step44)
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
        }

        /// <summary>
        /// Localizer - Review tools
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27999(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            Taskbar taskbar = null;
            Studies studies = null;
            StudyViewer StudyVw;
            Viewer viewer = new Viewer();
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                String PatientName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                String StudyID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");
                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String Description = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");

                String[] PatName = PatientName.Split(':');
                String[] AccessionID = AccessionIDList.Split(':');
                String[] StudyIDList = StudyID.Split(':');

                //Step-1: Load Study: Tumor, Left Forearm. Set the view to 2 series
                login.LoginIConnect(username, password);
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.ChooseColumns(new string[] { "Patient Name", "Accession" });
                studies.SearchStudy(LastName: PatName[0].Trim().Split(',')[0], AccessionNo: AccessionID[0]);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.SelectStudy("Accession", AccessionID[0]);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);
                //Set series view to 2 series
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                Thread.Sleep(3000);
                //Load related series that have different scan plane into the two viewers.
                StudyVw.DragThumbnailToViewport(3, StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                StudyVw.DragThumbnailToViewport(4, StudyVw.SeriesViewer_1X2().GetAttribute("id"));
                Thread.Sleep(3000);
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);
                //Turn ON Localizer line
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step1 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step1)
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

                //Step-2: Apply pan tool where localizer is displayed - On 2nd viewport
                //IWebElement element = BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"));
                StudyVw.ApplyPan(StudyVw.SeriesViewer_1X2());
                Thread.Sleep(3000);
                String LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X1(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step2 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag == "true" && step2)
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

                //Step-3: Apply zoom tool where localizer is displayed - On 1st viewport
                StudyVw.ApplyZoom(StudyVw.SeriesViewer_1X1());
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step3 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag == "true" && step3)
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

                //Step-4: Apply the rotate clockwise and rotate counter clockwise tools
                StudyVw.Click("id", StudyVw.SeriesViewer_1X2().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X1(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step4_1 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.RotateCounterclockwise);
                Thread.Sleep(3000);
                String LocalizerFlag2 = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X1(), "src", '&', "ToggleLocalizerOn");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step4_2 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel(), 2, 1);
                if (LocalizerFlag == "true" && LocalizerFlag2 =="true" && step4_1 && step4_2)
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

                //Step-5: Apply the horizontal flip and vertical flip tools
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.FlipHorizontal);
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step5_1 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.FlipVertical);
                Thread.Sleep(3000);
                LocalizerFlag2 = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step5_2 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel(), 2, 1);
                if (LocalizerFlag == "true" && LocalizerFlag2 == "true" && step5_1 && step5_2)
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

                //Step-6: Click Reset
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step6 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag == "true" && step6)
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

                //Step-7: Apply WW/WL 
                StudyVw.ApplyWindowLevel(StudyVw.SeriesViewer_1X2());
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X1(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step7 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag == "true" && step7)
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

                //Step-8: Add text on an image that has no localizer line displayed
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.AddText);
                var action = new Actions(Driver);
                action.MoveToElement(StudyVw.SeriesViewer_1X2(), StudyVw.SeriesViewer_1X2().Size.Width / 4, StudyVw.SeriesViewer_1X2().Size.Height / 4).Click().Build().Perform();
                bool Step8_TextBox = PageLoadWait.WaitForElement(StudyVw.StudyPanelTextbox(1, 2), WaitTypes.Visible).Displayed;
                //StudyVw.DrawTextAnnotation(StudyVw.SeriesViewer_1X2(), StudyVw.SeriesViewer_1X2().Size.Width / 4, StudyVw.SeriesViewer_1X2().Size.Height / 4, StudyVw.StudyPanelTextbox(1, 2), "Text1");
                //m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_inputBox
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X1(), "src", '&', "ToggleLocalizerOn");
                if (LocalizerFlag == "true" && Step8_TextBox)
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

                //Step-9: With the add text tool selected click on an image that has localizer line displayed
                action = new Actions(Driver);
                action.MoveToElement(StudyVw.SeriesViewer_1X1(), StudyVw.SeriesViewer_1X1().Size.Width / 4, StudyVw.SeriesViewer_1X1().Size.Height / 4).Click().Build().Perform();
                bool TextBoxVisibility = PageLoadWait.WaitForElement(StudyVw.StudyPanelTextbox(1, 1), WaitTypes.Visible).Displayed;
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn");
                if (TextBoxVisibility && LocalizerFlag == "true")
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

                //Step-10: Type in any text, hit Enter key.
                Driver.FindElement(StudyVw.StudyPanelTextbox(1, 1)).SendKeys("Text1");
                Driver.FindElement(StudyVw.StudyPanelTextbox(1, 1)).SendKeys(Keys.Enter);
                Thread.Sleep(3000);
                LocalizerFlag2 = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step10 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag2 == "true" && step10)
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

                //Step-11: add text tool selected click on an image that has localizer line displayed
                action = new Actions(Driver);
                action.MoveToElement(StudyVw.SeriesViewer_1X2(), StudyVw.SeriesViewer_1X2().Size.Width / 4, StudyVw.SeriesViewer_1X2().Size.Height / 4).Click().Build().Perform();
                TextBoxVisibility = PageLoadWait.WaitForElement(StudyVw.StudyPanelTextbox(1, 2), WaitTypes.Visible).Displayed;
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X1(), "src", '&', "ToggleLocalizerOn");
                if (TextBoxVisibility && LocalizerFlag == "true")
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

                //Step-12: Type in any text, hit Esc key.
                Driver.FindElement(StudyVw.StudyPanelTextbox(1, 2)).SendKeys("Text2");
                Driver.FindElement(StudyVw.StudyPanelTextbox(1, 2)).SendKeys(Keys.Escape);
                Thread.Sleep(3000);
                LocalizerFlag2 = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X1(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step12 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag2 == "true" && step12)
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

                //Step-13: Click the Printable View link
                string[] WindowHandles = viewer.OpenPrintViewandSwitchtoIT();
                IWebElement viewport = BasePage.Driver.FindElement(By.Id("SeriesViewersDiv"));
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step13 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step13)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step number# " + ExecutedSteps + " Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step number# " + ExecutedSteps + " Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.ClosePrintView(WindowHandles[1], WindowHandles[0]);

                //Step-14: Print the images and compare the print out to what is displayed. - Not Automated
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-15: Load a study with series from the same acquisition angle
                //Set series view to 4 series
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                Thread.Sleep(3000);
                StudyVw.DragThumbnailToViewport(1, StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                StudyVw.DragThumbnailToViewport(2, StudyVw.SeriesViewer_1X2().GetAttribute("id"));
                Thread.Sleep(3000);
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step15 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step15)
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

                //Step-16: Load a series with the same acquisition angle into viewers and toggle on the localizer line
                StudyVw.DragThumbnailToViewport(3, StudyVw.SeriesViewer_2X1().GetAttribute("id"));
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step16 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag == "true" && step16)
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
                StudyVw.CloseStudy();

                //Step-17: Load a study with multiple series that are of the same body part from different angles
                studies.ClearFields();
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(LastName: PatName[1].Trim().Split(',')[0], studyID: StudyIDList[0]);
                PageLoadWait.WaitForLoadingMessage(60);
                studies.SelectStudy("Study ID", StudyIDList[0]);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step17 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step17)
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

                //Step-18: From Patient History drawer, load related series from the other study
                studies.NavigateToHistoryPanel();
                studies.OpenPriors(new string[] { "Study Description" }, new string[] { Description });
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step18 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.StudyPanelContainer());
                if (step18)
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

                //Step-19: Selectprimary study and enable localizer line
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step19 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag == "true" && step19)
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

                //Step-20: Select viewport with no image 
                //Change layout to 2x3 for a blank viewport number 6
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                Thread.Sleep(3000);
                StudyVw.Click("id", StudyVw.SeriesViewer_2X3().GetAttribute("id"));
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X1(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step20 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag == "false" && step20)
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

                //Step-21: Select a viewport that has image loaded
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step21 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag == "true" && step21)
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

                //Step-22: Disable the localizer line.
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step22 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag == "false" && step22)
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
                StudyVw.CloseStudy();

                //Step-23: Load a MR or CT (example: Bony, Rose - 33648)
                studies.ClearFields();
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(LastName: PatName[1].Trim().Split(',')[0], studyID: StudyIDList[0]);
                studies.SelectStudy("Study ID", StudyIDList[0]);
                studies.LaunchStudy();
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);
                // Load one of the related series into the second viewer window
                studies.NavigateToHistoryPanel();
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.OpenPriors(new string[] { "Study ID" }, new string[] { StudyIDList[1] });
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step23 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.StudyPanelContainer());
                if (step23)
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

                //Step-24: Select the top left view port (Series 1) of the first viewer window and enable the localizer line.
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn");
                try{    LocalizerFlag2 = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X1(2), "src", '&', "ToggleLocalizerOn"); }
                catch (Exception ex) { LocalizerFlag2 = null; }
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step24 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.StudyPanelContainer());
                if (LocalizerFlag == "true" && LocalizerFlag2 == null && step24)
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

                //Step-25: Select the top left view port (Series 1) of the second viewer window and enable the localizer line.
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1(2).GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_2X2(2), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step25 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.StudyPanelContainer());
                if (LocalizerFlag == "true" && step25)
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


                //Step-26: Scroll 2nd viewer
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1(2).GetAttribute("id"));
                //Scroll down x 6
                for (int i = 0; i < 6; i++)
                {
                    StudyVw.ClickDownArrowbutton(1, 1, 2);
                }
                //Scroll Up x 3
                for (int i = 0; i < 3; i++)
                {
                    StudyVw.ClickUpArrowbutton(1, 1, 2);
                }
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step26 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.StudyPanelContainer());
                if (step26)
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

                //Step-27: Load the last one of the related series into the third viewer window 
                studies.NavigateToHistoryPanel();
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.OpenPriors(new string[] { "Study ID" }, new string[] { StudyIDList[2] });
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);
                Thread.Sleep(5000);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(StudyVw.SeriesViewer_1X1(3)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(StudyVw.Thumbnails()[0]));

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step27 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.StudyPanelContainer());
                if (step27)
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

                //Step-28: Select an empty viewport
                StudyVw.Click("id", StudyVw.SeriesViewer_2X1(3).GetAttribute("id"));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(StudyVw.SeriesViewer_1X1(3)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(StudyVw.Thumbnails()[0]));
                Thread.Sleep(3000);
                try { LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X1(3), "src", '&', "ToggleLocalizerOn"); }
                catch (Exception ex) { LocalizerFlag = null; }
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step28 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.StudyPanelContainer());
                if (LocalizerFlag == null && step28)
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
        }

        /// <summary>
        /// Localizer - 2.0 Scope: Series/Image
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_28000(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            Taskbar taskbar = null;
            Studies studies = null;
            StudyViewer StudyVw;
            Viewer viewer = new Viewer();
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            UserManagement usermanagement;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String DomainName = Config.adminGroupName;
                String RoleName = Config.adminRoleName;
                String PatientName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String StudyID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");
                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String Description = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
                String PatientDOB = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientDOB");
                String DSList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Datasource");
                String EA131 = login.GetHostName(Config.EA1);

                String[] PatName = PatientName.Split(':');
                String[] AccessionID = AccessionIDList.Split(':');
                String[] StudyIDList = StudyID.Split(':');
                String[] Datasources = DSList.Split(':');

                //Step-1: Complete precondition steps
                ExecutedSteps++;

                //Step-2: Define loading layout of MR and CT to 2 series viewer in Domain Management
                login.LoginIConnect(username, password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                //To Do
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);
                domainmanagement.ClickEditDomain();
                domainmanagement.AddPreset("MR", "", "", "", "1x2");
                domainmanagement.AddPreset("CT", "", "", "", "1x2");
                domainmanagement.ClickSaveEditDomain();
                ExecutedSteps++;
                //Step-3: Load a MR or CT study with multiple series that has the same Frame UID and different scan plane.
                studies = (Studies)login.Navigate("Studies");
                studies.ClearFields();
                studies.ChooseColumns(new string[] { "Patient Name", "Accession" });
                studies.SearchStudy(LastName: PatName[0].Trim().Split(',')[0], AccessionNo: AccessionID[0]);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.SelectStudy("Accession", AccessionID[0]);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);
                //1x2 should appear
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step3 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step3)
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

                //Step-4: Change Scope to image. Select top left viewer, turn on localizer line.
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageScope);
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                Thread.Sleep(3000);
                String LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step4 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag == "true" && step4)
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

                //Step-5: Change to 4 series viewer.
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step5 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag == "true" && step5)
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

                //Step-6: Load other 2 series to the bottom viewer
                //Since study already has series loaded in 2 viewers, no operation performed
                ExecutedSteps++;

                //Step-7: Scroll through images each series.
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                //Scroll 
                for (int i = 0; i < 3; i++)
                {
                    StudyVw.ClickDownArrowbutton(1, 1);
                }
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step7 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step7)
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

                //Step-8: Change to 6 series viewer.
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                Thread.Sleep(3000);
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step8 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag == "true" && step8)
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

                //Step-9: Load more series into the viewer
                StudyVw.DragThumbnailToViewport(3, StudyVw.SeriesViewer_2X2().GetAttribute("id"));
                StudyVw.DragThumbnailToViewport(4, StudyVw.SeriesViewer_2X3().GetAttribute("id"));
                Thread.Sleep(3000);
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step9 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step9)
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

                //Step-10: Change image layout for each viewer
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x2);
                Thread.Sleep(3000);
                //Viewport 2
                StudyVw.Click("id", StudyVw.SeriesViewer_1X2().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x1);
                Thread.Sleep(3000);
                //Viewport 3
                StudyVw.Click("id", StudyVw.SeriesViewer_1X3().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                Thread.Sleep(3000);
                //Viewport 4
                StudyVw.Click("id", StudyVw.SeriesViewer_2X1().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout3x3);
                Thread.Sleep(3000);
                //Viewport 5
                StudyVw.Click("id", StudyVw.SeriesViewer_2X2().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout4x4);
                Thread.Sleep(3000);
                //Viewport 6
                StudyVw.Click("id", StudyVw.SeriesViewer_2X3().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x1);
                Thread.Sleep(3000);
                //WL on viewer 1
                StudyVw.ApplyWindowLevel(StudyVw.SeriesViewer_1X1());
                //Zoom on Viewer 2
                StudyVw.ApplyZoom(StudyVw.SeriesViewer_1X2());
                //Pan on Viewer 3
                StudyVw.ApplyPan(StudyVw.SeriesViewer_1X3());
                //Rotate on Viewer 4
                StudyVw.Click("id", StudyVw.SeriesViewer_2X1().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                //Invert on Viewer 5
                StudyVw.Click("id", StudyVw.SeriesViewer_2X2().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step10 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step10)
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

                //Step-11: Select a series with/without localizer displayed, apply measurement and text annotation.
                StudyVw.DrawLineMeasurement(StudyVw.SeriesViewer_1X1(), StudyVw.SeriesViewer_1X1().Size.Width/4, StudyVw.SeriesViewer_1X1().Size.Height/4);
                StudyVw.DrawTextAnnotation(StudyVw.SeriesViewer_1X1(), StudyVw.SeriesViewer_1X1().Size.Width / 4, StudyVw.SeriesViewer_1X1().Size.Height / 4, StudyVw.StudyPanelTextbox(1, 1), "Text1");
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step11 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step11)
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

                //Step-12: Select a series that has measurement and annotation added. Select Save Annotated Images.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-13: Switch to series scoping.
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.SeriesScope);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step13 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step13)
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

                studies.CloseStudy();

                //Step-14: Load a MR or CT study - Same study reloaded as it satisfies required scenario
                studies.ClearFields();
                studies.ChooseColumns(new string[] { "Patient Name", "Accession" });
                studies.SearchStudy(LastName: PatName[0].Split(',')[0].Trim(), AccessionNo: AccessionID[0]);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.SelectStudy("Accession", AccessionID[0]);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step14 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step14)
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

                //Step-15: Scroll to middle image in each series. Select a reference viewer and then turn on localizer line.
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                //Scroll 
                for (int i = 0; i < 6; i++)
                {
                    StudyVw.ClickDownArrowbutton(1, 1);
                }
                StudyVw.Click("id", StudyVw.SeriesViewer_1X2().GetAttribute("id"));
                //Scroll 
                for (int i = 0; i < 6; i++)
                {
                    StudyVw.ClickDownArrowbutton(1, 2);
                }
                StudyVw.Click("id", StudyVw.SeriesViewer_2X1().GetAttribute("id"));
                ////Scroll 
                //for (int i = 0; i < 6; i++)
                //{
                //    StudyVw.ClickDownArrowbutton(2, 1);
                //}
                //StudyVw.Click("id", StudyVw.SeriesViewer_2X2().GetAttribute("id"));
                ////Scroll 
                //for (int i = 0; i < 6; i++)
                //{
                //    StudyVw.ClickDownArrowbutton(2, 2);
                //}
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step15 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag=="true" && step15)
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

                //Step-16: Change layout on each viewer. Apply different tools on each viewers
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x2);
                //Viewport 2
                StudyVw.Click("id", StudyVw.SeriesViewer_1X2().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x1);
                ////Viewport 3
                //StudyVw.Click("id", StudyVw.SeriesViewer_2X1().GetAttribute("id"));
                //StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout3x3);
                ////Viewport 4
                //StudyVw.Click("id", StudyVw.SeriesViewer_2X2().GetAttribute("id"));
                //StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout4x4);
                Thread.Sleep(3000);
                //WL on viewer 1
                StudyVw.ApplyWindowLevel(StudyVw.SeriesViewer_1X1());
                //Zoom on Viewer 2
                StudyVw.ApplyZoom(StudyVw.SeriesViewer_1X2());
                ////Rotate on Viewer 3
                //StudyVw.Click("id", StudyVw.SeriesViewer_2X1().GetAttribute("id"));
                //StudyVw.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                ////Invert on Viewer 4
                //StudyVw.Click("id", StudyVw.SeriesViewer_2X2().GetAttribute("id"));
                //StudyVw.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step16 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step16)
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

                //Step-17: Change the Scope to image. Apply different tools on each viewers.
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageScope);
                //WL on viewer 1
                StudyVw.ApplyWindowLevel(StudyVw.SeriesViewer_1X1());
                //Zoom on Viewer 2
                StudyVw.Click("id", StudyVw.SeriesViewer_1X2().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageScope);
                StudyVw.ApplyZoom(StudyVw.SeriesViewer_1X2());
                ////Rotate on Viewer 3
                //StudyVw.Click("id", StudyVw.SeriesViewer_2X1().GetAttribute("id"));
                //StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageScope);
                //StudyVw.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise);
                ////Invert on Viewer 4
                //StudyVw.Click("id", StudyVw.SeriesViewer_2X2().GetAttribute("id"));
                //StudyVw.SelectToolInToolBar(IEnum.ViewerTools.ImageScope);
                //StudyVw.SelectToolInToolBar(IEnum.ViewerTools.Invert);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step17 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step17)
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

                //Step-18: Scroll through images in reference viewer.
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                //Scroll 
                for (int i = 0; i < 3; i++)
                {
                    StudyVw.ClickDownArrowbutton(1, 1);
                }
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step18 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step18)
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

                studies.CloseStudy();

                //Step-19: Load the study"Different, Bodypart"(ID=12345)
                studies.ClearFields();
                studies.ChooseColumns(new string[] { "Patient Name", "Accession" });
                studies.SearchStudy(LastName: PatName[1], AccessionNo: AccessionID[1], Datasource: EA131);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.SelectStudy("Accession", AccessionID[1]);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step19 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step19)
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

                //Step-20: Change layout to 2 series and get the second series of the different body part loaded into 2nd viewer (series 3 and 5) and display the study into 1x2 layout
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                StudyVw.DragThumbnailToViewport(1, StudyVw.SeriesViewer_1X2().GetAttribute("id"));
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step20 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step20)
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

                //Step-21: Check localizer line tool
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-22: Select the localizer button
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step22 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step22)
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

                studies.CloseStudy();

                //Step-23: Load study Abdomen CT (patient ID 1205937) into the viewer
                studies.ClearFields();
                studies.ChooseColumns(new string[] { "Patient Name", "Patient ID" });
                studies.SearchStudy(LastName: PatName[2], patientID: PatientID);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.SelectStudy("Patient ID", PatientID);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step23 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (step23)
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

                //Step-24: Toggle the localizer line on.
                StudyVw.Click("id", StudyVw.SeriesViewer_1X2().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X1(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step24 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.studyPanel());
                if (LocalizerFlag=="true" && step24)
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

                //Step-25-28: These steps deal with exact measurement, hence marked NA
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                studies.CloseStudy();

                //Step-29: Configure a AMICAS data source - Configured as part of environment setup
                ExecutedSteps++;

                //Step-30: Load Smith patient ID=AM-0098
                studies.ClearFields();
                studies.ChooseColumns(new string[] { "Patient Name", "Accession" });
                studies.SearchStudy(LastName: PatName[3], AccessionNo: AccessionID[2], Datasource: DSList);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.SelectStudy("Accession", AccessionID[2]);
                StudyVw = studies.LaunchStudy();
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);
                //Load 2 related studies
                studies.NavigateToHistoryPanel();
                studies.ChooseColumns(new string[] { "Accession" });
                studies.OpenPriors(new string[] { "Accession" }, new string[] { AccessionID[3] });
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);

                studies.NavigateToHistoryPanel();
                studies.ChooseColumns(new string[] { "Accession" });
                studies.OpenPriors(new string[] { "Accession" }, new string[] { AccessionID[4] });
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);

                //Load series 2 and 3 in viewport for all panels
                //Panel 1
                StudyVw.DragThumbnailToViewport(2, StudyVw.SeriesViewer_1X1().GetAttribute("id"));
                StudyVw.DragThumbnailToViewport(3, StudyVw.SeriesViewer_1X2().GetAttribute("id"));
                //Panel 2
                StudyVw.DragThumbnailToViewport(2, StudyVw.SeriesViewer_1X1(2).GetAttribute("id"), 2);
                StudyVw.DragThumbnailToViewport(3, StudyVw.SeriesViewer_1X2(2).GetAttribute("id"), 2);
                //Panel 3
                StudyVw.DragThumbnailToViewport(2, StudyVw.SeriesViewer_1X1(3).GetAttribute("id"), 3);
                StudyVw.DragThumbnailToViewport(3, StudyVw.SeriesViewer_1X2(3).GetAttribute("id"), 3);
                PageLoadWait.WaitForThumbnailsToLoad(80);
                PageLoadWait.WaitForPageLoad(80);
                PageLoadWait.WaitForFrameLoad(80);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step30 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.StudyPanelContainer());
                if (step30)
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

                //Step-31: Select a viewport in the primary viewer and turn on the Localizer Line
                StudyVw.Click("id", StudyVw.SeriesViewer_1X2().GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                //Scroll 
                for (int i = 0; i < 3; i++)
                {
                    StudyVw.ClickDownArrowbutton(1, 2);
                }
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X1(), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step31 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.StudyPanelContainer());
                if (LocalizerFlag == "true" && step31)
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

                //Step-32: Repeat the previous step on the seconday, respectively on the third viewer.
                //Scroll Viewer 2
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1(2).GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                for (int i = 0; i < 3; i++)
                {
                    StudyVw.ClickDownArrowbutton(1, 1, 2);
                }
                Thread.Sleep(3000);
                LocalizerFlag = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(2), "src", '&', "ToggleLocalizerOn");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                bool step32_1 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.StudyPanelContainer());
                //Scroll Viewer 3
                StudyVw.Click("id", StudyVw.SeriesViewer_1X1(3).GetAttribute("id"));
                StudyVw.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                for (int i = 0; i < 3; i++)
                {
                    StudyVw.ClickDownArrowbutton(1, 1, 3);
                }
                Thread.Sleep(3000);
                String LocalizerFlag2 = StudyVw.GetInnerAttribute(StudyVw.SeriesViewer_1X2(3), "src", '&', "ToggleLocalizerOn");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                bool step32_2 = studies.CompareImage(result.steps[ExecutedSteps], StudyVw.StudyPanelContainer(), 2, 1);
                //Verification
                if (LocalizerFlag == "true" && LocalizerFlag2 == "true" && step32_1 && step32_2)
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
        }

        /// <summary>
        /// Localizer - Initial Setups and Pre-Conditions
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_168669(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = null;
            BluRingViewer bluringviewer = null;
            BasePage basepage = new BasePage();
            string LocalizerLineAngleToleranceBeforeUpdate = "";
            String imageConfigXML = @"C:\WebAccess\WebAccess\Config\Imager\ImagerConfiguration.xml";

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String accessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                
                //Step-1: Precondition : Edit ImageConfiguration.xml file and make sure this parameter is set to value 30.0.                
                LocalizerLineAngleToleranceBeforeUpdate = basepage.GetAttributeValue(imageConfigXML, "/ImagerConfiguration/properties/property[@key='LocalizerLineAngleToleranceForParallelPlanes']", "value");
                basepage.ChangeAttributeValue(imageConfigXML, "/ImagerConfiguration/properties/property[@key='LocalizerLineAngleToleranceForParallelPlanes']", "value", "30.0");
                basepage.RestartIISUsingexe();
                string LocalizerLineAngleToleranceAfterUpdate1 = basepage.GetAttributeValue(imageConfigXML, "/ImagerConfiguration/properties/property[@key='LocalizerLineAngleToleranceForParallelPlanes']", "value");
                if (LocalizerLineAngleToleranceAfterUpdate1 == "30.0")
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step-2: Load the study mentioned in precondition.
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: accessionID, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", accessionID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                bluringviewer.ChangeViewerLayout("2x2", 1);
                BluRingViewer.WaitforViewports();

                //click on view port 3 and Toggle ON the localizer button.
                bluringviewer.ClickOnViewPort(panelnumber: 1, viewportnumber: 3);
                BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_LocalizerLinesIcon)).Click();
                result.steps[++ExecutedSteps].StepPass();

                //Step-3: scroll through viewport 3 and go to image #53
                IWebElement ele = bluringviewer.GetElement(BasePage.SelectorType.CssSelector, bluringviewer.Activeviewport);
                new TestCompleteAction().MouseScroll(ele, "down", "54").Perform();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool Step3 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.studyPanel(studyPanelIndex: 1));
                if (Step3)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //Step-4: Edit ImageConfiguration.xml file and make sure this parameter is set to value 1.0.
                basepage.ChangeAttributeValue(imageConfigXML, "/ImagerConfiguration/properties/property[@key='LocalizerLineAngleToleranceForParallelPlanes']", "value", "1.0");
                basepage.RestartIISUsingexe();
                string LocalizerLineAngleToleranceAfterUpdate2 = basepage.GetAttributeValue(imageConfigXML, "/ImagerConfiguration/properties/property[@key='LocalizerLineAngleToleranceForParallelPlanes']", "value");
                if (LocalizerLineAngleToleranceAfterUpdate2 == "1.0")
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step-5: Repeat step 2and 3
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: accessionID, Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", accessionID);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                bluringviewer.ChangeViewerLayout("2x2", 1);
                BluRingViewer.WaitforViewports();
                bluringviewer.ClickOnViewPort(panelnumber: 1, viewportnumber: 3);
                BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_LocalizerLinesIcon)).Click();
                ele = bluringviewer.GetElement(BasePage.SelectorType.CssSelector, bluringviewer.Activeviewport);
                new TestCompleteAction().MouseScroll(ele, "down", "54").Perform();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool Step5 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.studyPanel(studyPanelIndex: 1));
                if (Step5)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
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
                if (LocalizerLineAngleToleranceBeforeUpdate != "")
                {
                    basepage.ChangeAttributeValue(imageConfigXML, "/ImagerConfiguration/properties/property[@key='LocalizerLineAngleToleranceForParallelPlanes']", "value", LocalizerLineAngleToleranceBeforeUpdate);
                    basepage.RestartIISUsingexe();
                }
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
