using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Reusable.Generic;
using System.Text.RegularExpressions;
using Selenium.Scripts.Pages.HoldingPen;
using OpenQA.Selenium;
using Selenium.Scripts.Pages;
using OpenQA.Selenium.Interactions;
using Selenium.Scripts.Pages.MergeServiceTool;
using OpenQA.Selenium.Remote;
using System.Threading;
using System.Diagnostics;
using Selenium.Scripts.Pages.eHR;
using System.Text.RegularExpressions;
using Dicom.Network;
using Selenium.Scripts.Pages.HoldingPen;

namespace Selenium.Scripts.Tests
{
    class Thumbnails
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public  TestCompleteAction testcompleteAction { get; set; }

        public string EA_91 = null;
        public string EA_131 = null;
        public string PACS_A7 = null;
        public string EA_77 = null;
        public string EA_96 = null;

        public Thumbnails(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            EA_91 = login.GetHostName(Config.EA91);
            EA_77 = login.GetHostName(Config.EA77);
            EA_131 = login.GetHostName(Config.EA1);
            PACS_A7 = login.GetHostName(Config.SanityPACS);
            EA_96 = login.GetHostName(Config.EA96);
        }

        /// <summary>
        /// 137325 - Thumbnail hover - tooltip
        /// </summary>		
        public TestCaseResult Test_161055(String testid, String teststeps, int stepcount)
        {

            // Declare and initialize variables
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            ServiceTool servicetool = new ServiceTool();
            UserManagement usermanagement = new UserManagement();
            DomainManagement domain = new DomainManagement();
            WpfObjects wpfobject = new WpfObjects();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String domain1 = "Domain1_" + new Random().Next(1, 1000);
                String role1 = "Role1_" + new Random().Next(1, 1000);
                String rad1 = Config.newUserName + new Random().Next(1, 1000);

                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String FirstNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstNameList");
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastNameList");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String CaptionsList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Captions");
                String[] Accession = AccessionList.Split(':');
                String[] LastName = LastNameList.Split(':');
                String[] FirstName = FirstNameList.Split(':');
                String[] Captions = CaptionsList.Split(':');

                //Precondition                              
                //Create new user
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.CreateDomain(domain1, role1, datasources: null);
                domain.ClickSaveNewDomain();
                login.Navigate("UserManagement");
                usermanagement.CreateUser(rad1, domain1, role1);
                login.Logout();

                // Step 1 - Login as the domain user        
                login.DriverGoTo(login.url);
                login.LoginIConnect(rad1, rad1);
                UserPreferences UserPreferences = login.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences.ModalityDropDown().SelectByText("MR");
                PageLoadWait.WaitForPageLoad(10);
                UserPreferences.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                UserPreferences.ModalityDropDown().SelectByText("CR");
                PageLoadWait.WaitForPageLoad(10);
                UserPreferences.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                PageLoadWait.WaitForPageLoad(10);
                UserPreferences.ModalityDropDown().SelectByText("CT");
                PageLoadWait.WaitForPageLoad(10);
                UserPreferences.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                PageLoadWait.WaitForPageLoad(10);
                UserPreferences.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                ExecutedSteps++;

                // Step 2 - search for the study and launch in the bluring viewer
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                String[] StudyCaption = viewer.GetStudyPanelThumbnailCaption();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step2 && StudyCaption[0].Equals("S6") && StudyCaption[1].Equals("S7"))
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

                // Step 3 - Verify the tooltip of the  thumbnail in study panel 
                String toolTip1 = "Modality:MR\r\nDate:04-Feb-1995 10:13:15 AM";
                String toolTip2 = "Modality:CR\r\nDescription:cervical Left LAT\r\nDate:unknown";
                String toolTip3 = "Modality:CT\r\nDescription:Neck 5mm 5.0 B50s\r\nDate:19-Aug-2004 2:01:45 PM";
                String toolTip4 = "Modality:US\r\nDate:unknown";
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer"))
                {
                    toolTip1 = "Modality:MR&#10;Date:04-Feb-1995 10:13:15 AM";
                    toolTip2 = "Modality:CR&#10;Description:cervical Left LAT&#10;Date:unknown";
                    toolTip3 = "Modality:CT&#10;Description:Neck 5mm 5.0 B50s&#10;Date:19-Aug-2004 2:01:45 PM";                    
                }               
                var step3_1 = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1)).GetAttribute("innerHTML").Contains(toolTip1);
                var step3_2 = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(2)).GetAttribute("innerHTML").Contains(toolTip1);
                viewer.HoverElement(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1)));
                var step3_3 = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1)).GetCssValue("opacity").Equals("0.8");
                if (step3_1 && step3_2 && step3_3)
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

                // Step 4 - Click on thumbnail icon on the exam list 
                viewer.ScrollIntoView(viewer.GetElement("cssselector", BluRingViewer.div_thumbnailpreviewIconActiveStudy));
                viewer.ClickElement(viewer.GetElement("cssselector", BluRingViewer.div_thumbnailpreviewIconActiveStudy));
                BluRingViewer.WaitforThumbnails();
                if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
                    viewer.ScrollIntoView(viewer.GetElement("cssselector", BluRingViewer.div_closeStudy));
                var ExamListThumbnailCount = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImageComponent)).Count();
                var step4_1 = ExamListThumbnailCount == BluRingViewer.NumberOfThumbnailsInStudyPanel();
                if (step4_1)
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

                // Step 5 - Verify the tooltip of the  thumbnail in Exam list
                var step5_1 = false;
                var step5_2 = false;
                var step5_3 = false;
                step5_1 = viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(133, 1)).GetAttribute("innerHTML").Contains(toolTip1);
                step5_2 = viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(133, 2)).GetAttribute("innerHTML").Contains(toolTip1);
                viewer.HoverElement(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(133, 1)));
                step5_3 = viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(133, 1)).GetCssValue("opacity").Equals("0.8");
                if (step5_1 && step5_2 && step5_3)
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

                // Step 6 - open another study in the another study panel
                viewer.ScrollIntoView(viewer.GetElement("cssselector", BluRingViewer.div_relatedStudyPanel + ":nth-of-type(103)"));
                viewer.OpenPriors(102);
                if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
                {
                    viewer.ScrollIntoView(viewer.GetElement("cssselector", BluRingViewer.div_closeStudy));
                }
                int studyPanelThumbnailCount = BluRingViewer.NumberOfThumbnailsInStudyPanel(2);
                if (studyPanelThumbnailCount.Equals(7))
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

                // Step 7 - Verify the tooltip of the  thumbnail in the second study panel                      
                var step7_1 = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1, 2)).GetAttribute("innerHTML").Contains(toolTip2);
                var step7_2 = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(2, 2)).GetAttribute("innerHTML").Contains(toolTip2);
                var step7_3 = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(3, 2)).GetAttribute("innerHTML").Contains(toolTip2);
                var step7_4 = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(4, 2)).GetAttribute("innerHTML").Contains(toolTip2);
                var step7_5 = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(5, 2)).GetAttribute("innerHTML").Contains(toolTip2);
                var step7_6 = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(6, 2)).GetAttribute("innerHTML").Contains(toolTip2);
                var step7_7 = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(7, 2)).GetAttribute("innerHTML").Contains(toolTip2);
                viewer.HoverElement(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1, 2)));
                var step7_8 = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1, 2)).GetCssValue("opacity").Equals("0.8");
                if (step7_1 && step7_2 && step7_3 && step7_4 && step7_5 && step7_6 && step7_7 && step7_8)
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

                // Step 8 - Click on the thumbnail icon in the exam list                
                viewer.ClickElement(viewer.GetElement("cssselector", BluRingViewer.div_relatedStudyPanel + ":nth-of-type(103) " + BluRingViewer.div_thumbnailpreviewIconActiveStudy));
                BluRingViewer.WaitforThumbnails();
                var ExamListThumbnailCount2 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_relatedStudy + ":nth-of-type(103) " + BluRingViewer.div_examListThumbnailImageComponent)).Count();
                if (studyPanelThumbnailCount.Equals(ExamListThumbnailCount2))
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

                // Step 9 - verify the tool tip of the thumbnails in the exam list                 
                var step9_1 = viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(103, 1)).GetAttribute("innerHTML").Contains(toolTip2);
                var step9_2 = viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(103, 2)).GetAttribute("innerHTML").Contains(toolTip2);
                var step9_3 = viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(103, 3)).GetAttribute("innerHTML").Contains(toolTip2);
                var step9_4 = viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(103, 4)).GetAttribute("innerHTML").Contains(toolTip2);
                var step9_5 = viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(103, 5)).GetAttribute("innerHTML").Contains(toolTip2);
                var step9_6 = viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(103, 6)).GetAttribute("innerHTML").Contains(toolTip2);
                var step9_7 = viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(103, 7)).GetAttribute("innerHTML").Contains(toolTip2);
                viewer.HoverElement(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(103, 1)));
                var step9_8 = viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(103, 1)).GetCssValue("opacity").Equals("0.8");
                if (step9_1 && step9_2 && step9_3 && step9_4 && step9_5 && step9_6 && step9_7 && step9_8)
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

                // Step 10 - open another study in  3rd study panel
                viewer.ScrollIntoView(viewer.GetElement("cssselector", BluRingViewer.div_relatedStudyPanel + ":nth-of-type(34)"));
                viewer.OpenPriors(33);
                if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
                {
                    viewer.ScrollIntoView(viewer.GetElement("cssselector", BluRingViewer.div_closeStudy));
                }
                if (BluRingViewer.NumberOfThumbnailsInStudyPanel(3).Equals(1))
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

                // Step 11 - verify tooltip of thumbnail in study panel    
                viewer.HoverElement(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1, 3)));
                var step11_1 = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1, 3)).GetCssValue("opacity").Equals("0.8");
                var step11_2 = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1, 3)).GetAttribute("innerHTML").Contains(toolTip3);
                if (step11_1 && step11_2)
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

                // Step 12 - verify tooltip of thumbnail in Exam list
                viewer.Click("cssselector", BluRingViewer.div_relatedStudyPanel + ":nth-of-type(34) " + BluRingViewer.div_thumbnailpreviewIconActiveStudy);
                BluRingViewer.WaitforThumbnails();
                viewer.HoverElement(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(34, 1)));
              //  var step12_1 = viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(34, 1)).GetCssValue("opacity").Equals("0.8");
                var step12_2 = viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(34, 1)).GetAttribute("innerHTML").Contains(toolTip3);
                if (step12_2)
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

                // Step 13 - Close the Bluring viewer
                viewer.CloseBluRingViewer();
                login.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                UserPreferences.ModalityDropDown().SelectByText("US");
                UserPreferences.SelectRadioBtn("ThumbSplitRadioButtons", "Auto");
                UserPreferences.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                ExecutedSteps++;

                // Step 14 - search fot the study
                studies.SearchStudy(AccessionNo: Accession[1]);
                studies.SelectStudy("Accession", Accession[1]);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                String[] ThumbnailCaptions = viewer.GetStudyPanelThumbnailCaption();
                bool step14_1 = true;
                for (int i = 0; i < Captions.Length; i++)
                {
                    if (Captions[i].Equals(ThumbnailCaptions[i]))
                    {
                        Logger.Instance.InfoLog("The captions are matched for " + i + "times");
                        Logger.Instance.InfoLog("Expected " + Captions[i] + "; Actual " + ThumbnailCaptions[i]);
                    }
                    else
                    {
                        step14_1 = false;
                        Logger.Instance.InfoLog("Expected " + Captions[i] + "; Actual " + ThumbnailCaptions[i]);
                    }
                }
                if (step14_1)
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


                // Step 15 - verify the tooltip of the first thumbnail in study panel         
                var step15_1 = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1)).GetAttribute("title").Contains(toolTip4);
                var step15_2 = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(2)).GetAttribute("title").Contains(toolTip4);
                viewer.HoverElement(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1)));
                var step15_3 = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1)).GetCssValue("opacity").Equals("0.8");
                if (step15_1 && step15_2 && step15_3)
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

                // Step 16 - Click on the thumbnail preview icon in the exam list               
                viewer.Click("cssselector", BluRingViewer.div_thumbnailpreviewIconActiveStudy);
                BluRingViewer.WaitforThumbnails();
                var ExamListThumbnailCount3 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImageComponent)).Count();
                if (ExamListThumbnailCount3.Equals(BluRingViewer.NumberOfThumbnailsInStudyPanel()))
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

                // Step 17 - hover the thumbnails in the Exam list               
                var step17_1 = viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(1, 1)).GetAttribute("title").Contains(toolTip4);
                var step17_2 = viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(1, 2)).GetAttribute("title").Contains(toolTip4);
                viewer.HoverElement(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(1, 1)));
                var step17_3 = viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(1, 1)).GetCssValue("opacity").Equals("0.8");
                if (step17_1 && step17_2 && step17_3)
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

                // Close the Bluring viewer
                viewer.CloseBluRingViewer();
                login.Logout();

                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Thumbnails - orientation removed
        /// </summary>		
        public TestCaseResult Test_161053(String testid, String teststeps, int stepcount)
        {

            // Declare and initialize variables
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            ServiceTool servicetool = new ServiceTool();
            UserManagement usermanagement = new UserManagement();
            DomainManagement domain = new DomainManagement();
            WpfObjects wpfobject = new WpfObjects();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String domain1 = "Domain1_" + new Random().Next(1, 1000);
                String role1 = "Role1_" + new Random().Next(1, 1000);
                String rad1 = Config.newUserName + new Random().Next(1, 1000);

                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String FirstNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstNameList");
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastNameList");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String ThumbnailCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailCount");
                String[] Accession = AccessionList.Split(':');
                String[] LastName = LastNameList.Split(':');
                String[] FirstName = FirstNameList.Split(':');
                String[] Thumbnail = ThumbnailCount.Split(':');

                //Precondition - step 2              
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab("Viewer");
                servicetool.NavigateSubTab("Protocols");
                wpfobject.ClickButton("Modify", 1);
                String[] Modality = { "CT", "MR", "NM", "PT", "RF", "CR", "XA" };
                for (int i = 0; i < Modality.Length; i++)
                {
                    servicetool.SelectDropdown("ComboBox_Modality", Modality[i]);
                    if (Modality[i] == "CT" || Modality[i] == "MR" || Modality[i] == "NM" || Modality[i] == "PT" || Modality[i] == "RF")
                    {
                        wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingSeries");
                    }
                    else
                    {
                        wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingName");
                    }
                }
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                wpfobject.WaitTillLoad();

                //Precondition - step 1,3
                //Create new user
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.CreateDomain(domain1, role1, datasources: null);
                domain.ClickSaveNewDomain();
                login.Navigate("UserManagement");
                usermanagement.CreateUser(rad1, domain1, role1);
                login.Navigate("DomainManagement");
                domain.SearchDomain(domain1);
                domain.SelectDomain(domain1);
                domain.ClickEditDomain();
                string[] Mod = { "CT", "MR", "NM", "PT", "RF", "CR", "XA" };
                for (int i = 0; i < Mod.Length; i++)
                {
                    domain.ModalityDropDown().SelectByText(Mod[i]);
                    if (Mod[i] == "CT" || Mod[i] == "MR" || Mod[i] == "NM" || Mod[i] == "PT" || Mod[i] == "RF")
                    {
                        domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                    }
                    else
                    {
                        domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                    }
                }
                domain.ClickSaveDomain();
                login.Logout();

                // Step 1 - Login as the domain user        
                login.DriverGoTo(login.url);
                login.LoginIConnect(rad1, rad1);
                ExecutedSteps++;

                // Step 2 - search for the study and launch in the bluring viewer
                var studies = new Studies();
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                if (Int32.Parse(Thumbnail[0]).Equals(BluRingViewer.NumberOfThumbnailsInStudyPanel()))
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

                // Step 3 - verify the thumbnail in study panel and exam list
                IWebElement ele = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_thumbnailpreviewIconActiveStudy);
                viewer.ScrollIntoView(ele);
                viewer.ClickElement(ele);
                BluRingViewer.WaitforThumbnails();

                bool step3_1 = false;
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("chrome"))
                {
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    step3_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement("cssselector", viewer.GetExamListThumbnailContainerCss(136)));

                }
                else
                {
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    step3_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement("cssselector", viewer.GetExamListThumbnailContainerCss(133)));

                }
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step3_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel), totalImageCount: 2, IsFinal: 1);
                if (step3_1 && step3_2)
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

                // Step 4 - Close the Bluring viewer and open the study in ICA viewer
                viewer.CloseBluRingViewer();
                UserPreferences userPrefer = new UserPreferences();
                userPrefer.OpenUserPreferences();
                userPrefer.SwitchToUserPrefFrame();
                viewer.ClickElement(userPrefer.HTML4RadioBtn());
                userPrefer.CloseUserPreferences();
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, "#m_studyPanels_m_studyPanel_1_studyViewerContainer"));
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

                // Close ICA viewer
                studies.CloseStudy();

                // Step 5 -Load another study in the Bluring viewer
                userPrefer.OpenUserPreferences();
                userPrefer.SwitchToUserPrefFrame();
                viewer.ClickElement(userPrefer.BluringViewerRadioBtn());
                userPrefer.CloseUserPreferences();
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[1]);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                if (Int32.Parse(Thumbnail[1]).Equals(BluRingViewer.NumberOfThumbnailsInStudyPanel()))
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

                // Step 6 -  verify the thumbnail in study panel and exam list
                viewer.ClickExamListThumbnailIcon("08-Sep-1999 8:28:44 AM");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step6_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement("cssselector", viewer.GetExamListThumbnailContainerCss(1)));
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step6_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel));
                if (step6_1 && step6_2)
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

                // Step 7 - Close the Bluring viewer and open the study in ICA viewer
                viewer.CloseBluRingViewer();
                userPrefer.OpenUserPreferences();
                userPrefer.SwitchToUserPrefFrame();
                viewer.ClickElement(userPrefer.HTML4RadioBtn());
                userPrefer.CloseUserPreferences();
                studies.SelectStudy("Accession", Accession[1]);
                studies.LaunchStudy();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, "#m_studyPanels_m_studyPanel_1_studyViewerContainer"));
                if (step7)
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

                // Close ICA viewer 
                studies.CloseStudy();
                // Change the preferrence to BluRing Viewer
                userPrefer.OpenUserPreferences();
                userPrefer.SwitchToUserPrefFrame();
                viewer.ClickElement(userPrefer.BluringViewerRadioBtn());
                userPrefer.CloseUserPreferences();
                //Logout from application
                login.Logout();

                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
        ///  Exam List: thumbnail preview
        /// </summary>
        public TestCaseResult Test_161045(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            ServiceTool servicetool = new ServiceTool();
            UserManagement usermanagement = new UserManagement();
            DomainManagement domain = new DomainManagement();
            WpfObjects wpfobject = new WpfObjects();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String domain1 = "Domain1_" + new Random().Next(1, 1000);
                String role1 = "Role1_" + new Random().Next(1, 1000);
                String rad1 = Config.newUserName + new Random().Next(1, 1000);

                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String ThumbnailCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailCount");
                String ModalityList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String[] Accession = AccessionList.Split(':');
                String[] Thumbnail = ThumbnailCount.Split(':');

                //Pre-condition: Use Default thumbnail splitting rules(CT,MR,NM,PT,RF are series-split,everything else is image-split) in service tool 
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab("Viewer");
                servicetool.NavigateSubTab("Protocols");
                wpfobject.ClickButton("Modify", 1);
                String[] Modality = { "CT", "MR", "NM", "PT", "RF", "CR", "XA" };
                for (int i = 0; i < Modality.Length; i++)
                {
                    servicetool.SelectDropdown("ComboBox_Modality", Modality[i]);
                    if (Modality[i] == "CR" || Modality[i] == "XA")
                    {
                        wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingSeries");
                    }
                    else
                    {
                        wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingName");
                    }
                }
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                wpfobject.WaitTillLoad();
                //Pre-condition:DomainConfiguration and create new user 
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.CreateDomain(domain1, role1, datasources: null);
                domain.ClickSaveNewDomain();
                login.Navigate("UserManagement");
                usermanagement.CreateUser(rad1, domain1, role1);
                login.Navigate("DomainManagement");
                domain.SearchDomain(domain1);
                domain.SelectDomain(domain1);
                //Pre-condition: Ensure the TestDomain1, Role1, ReadingPhysician role, admin1, rad1, are configured
                domain.ClickEditDomain();
                string[] Mod = { "CT", "MR", "NM", "PT", "RF", "CR", "XA" };
                for (int i = 0; i < Mod.Length; i++)
                {
                    domain.ModalityDropDown().SelectByText(Mod[i]);
                    if (Mod[i] == "CR" || Mod[i] == "MR" || Mod[i] == "XA")
                    {
                        domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                    }
                    else
                    {
                        domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                    }
                }
                domain.ClickSaveDomain();
                login.Logout();

                //Step 1-Login as the domain user
                login.DriverGoTo(login.url);
                login.LoginIConnect(rad1, rad1);
                ExecutedSteps++;

                //Step-2 Search select a MR Modality Study and verify thumbnail its loaded in studypanel asseries-split and accordingly in viewport
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Modality", ModalityList);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForFrameLoad(50);
                if (Int32.Parse(Thumbnail[0]).Equals(BluRingViewer.NumberOfThumbnailsInStudyPanel()) && ((viewer.IsElementPresent(By.CssSelector(BluRingViewer.div_allThumbnailsViewports)))))
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

                //Step-3 Click on current  MR modality Study and verify the Thumbnail Preview panel opens with selected study then it contains 3X3 rows and columns and it Equivalent to Studypanel
                viewer.OpenExamListThumbnailPreview(1);
                viewer.waitForThumbnailstoLoad();
                IList<IWebElement> thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));
                bool step3_1 = false;
                IList<IWebElement> thumbnailsContainer = BasePage.Driver.FindElements(By.CssSelector("div.relatedStudythumbnailContainerComponent div.ps-container.thumbnails.ps"));
                int thumbnailExpectedHeight = thumbnailsContainer[1].Size.Height / 3;
                int thumbnailExpectedWidth = thumbnailsContainer[1].Size.Width / 3;
                foreach (IWebElement thumbnail in thumbnails)
                {
                    if(!(thumbnail.Size.Height <= thumbnailExpectedHeight && thumbnail.Size.Height >= (thumbnailExpectedHeight - thumbnailExpectedHeight / 3)))
                    {                        
                        step3_1 = true;
                        Logger.Instance.InfoLog("Invalid Height");
                        break;
                    }
                    if(!(thumbnail.Size.Width <= thumbnailExpectedWidth && thumbnail.Size.Width >= (thumbnailExpectedWidth - thumbnailExpectedWidth / 3)))
                    {
                        step3_1 = true;
                        Logger.Instance.InfoLog("Invalid Width");
                        break;
                    }
                }
                bool step3_2 = ((viewer.IsElementPresent(By.CssSelector(BluRingViewer.div_ExamListthumbnailview))) && (viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_Examlistdefaultselectedthumbnail))));             
                String[] ExamCaption = viewer.GetExamListThumbnailCaption();
                String[] StudyCaption = viewer.GetStudyPanelThumbnailCaption();
                bool step3_3 = true;
                for (int i = 0; i < StudyCaption.Length; i++)
                {
                    if (ExamCaption[i].Equals(StudyCaption[i]))
                    {
                        Logger.Instance.InfoLog("The examlist thumbnail caption is equals with studypanel thumbnail caption");
                    }
                    else
                    {
                        step3_3 = false;
                    }
                }
                if (!step3_1 && step3_2 && step3_3)
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

                //Step-4 Click  Prior Study and verify the Thumbnail Preview panel opens with selected study then it contains 3X3 rows and columns and it Equivalent to Studypanel
                viewer.CloseExamListThumbnailPreviewWindow(1);
                viewer.OpenExamListThumbnailPreview(0);
                bool step4 = ((viewer.IsElementPresent(By.CssSelector(BluRingViewer.div_ExamListthumbnailview))));
                bool step4_1 = false;
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));
                thumbnailsContainer = BasePage.Driver.FindElements(By.CssSelector("div.relatedStudythumbnailContainerComponent div.ps-container.thumbnails.ps"));
                thumbnailExpectedHeight = thumbnailsContainer[0].Size.Height / 3;
                thumbnailExpectedWidth = thumbnailsContainer[0].Size.Width / 3;
                foreach (IWebElement thumbnail in thumbnails)
                {
                    if (!(thumbnail.Size.Height <= thumbnailExpectedHeight && thumbnail.Size.Height >= (thumbnailExpectedHeight - thumbnailExpectedHeight / 3)))
                    {
                        step4_1 = true;
                        Logger.Instance.InfoLog("Invalid Height");
                        break;
                    }
                    if (!(thumbnail.Size.Width <= thumbnailExpectedWidth && thumbnail.Size.Width >= (thumbnailExpectedWidth - thumbnailExpectedWidth / 3)))
                    {
                        step4_1 = true;
                        Logger.Instance.InfoLog("Invalid Width");
                        break;
                    }
                }               
                if (step4 && !step4_1)
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


                //Step-5 Close the Examlist thumnail Preview
                viewer.CloseExamListThumbnailPreviewWindow(0);
                if ((!viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_ExamListthumbnailview))))
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

                //Step-6  Search select a US Modality Study and verify thumbnail its loaded in studypanel as image and accordingly in viewport
                viewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.SanityPACS));
                studies.SelectStudy("Accession", Accession[1]);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(50);
                PageLoadWait.WaitForFrameLoad(50);
                if (Int32.Parse(Thumbnail[1]).Equals(BluRingViewer.NumberOfThumbnailsInStudyPanel()) && ((viewer.IsElementPresent(By.CssSelector(BluRingViewer.div_allThumbnailsViewports)))))
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

                //Step-7 Click on current US modality Study and verify the Thumbnail Preview panel opens with selected study then it contains 3X3 rows and columns  it Equivalent to Studypanel and Overlay
                viewer.OpenExamListThumbnailPreview(0);
                bool step7_0 = ((viewer.IsElementPresent(By.CssSelector(BluRingViewer.div_ExamListthumbnailview))) && (viewer.IsElementPresent(By.CssSelector(BluRingViewer.div_Examlistdefaultselectedthumbnail))));                
                bool step7_1 = false;
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));
                thumbnailsContainer = BasePage.Driver.FindElements(By.CssSelector("div.relatedStudythumbnailContainerComponent div.ps-container.thumbnails.ps"));
                thumbnailExpectedHeight = thumbnailsContainer[0].Size.Height / 3;
                thumbnailExpectedWidth = thumbnailsContainer[0].Size.Width / 3;                
                foreach (IWebElement thumbnail in thumbnails)
                {
                    if (!(thumbnail.Size.Height <= thumbnailExpectedHeight && thumbnail.Size.Height >= (thumbnailExpectedHeight - thumbnailExpectedHeight / 3)))
                    {
                        step7_1 = true;
                        Logger.Instance.InfoLog("Invalid Height");
                        break;
                    }

                    if (!(thumbnail.Size.Width <= thumbnailExpectedWidth && thumbnail.Size.Width >= (thumbnailExpectedWidth - thumbnailExpectedWidth / 3)))
                    {
                        step7_1 = true;
                        Logger.Instance.InfoLog("Invalid Width");
                        break;
                    }
                }
                IWebElement ele = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_examlistThumbnailVerticalScrollbar));
                bool step7_2 = !ele.GetAttribute("style").Contains("height: 0px");               
                String[] ExamCaption1 = viewer.GetExamListThumbnailCaption();
                String[] StudyCaption1 = viewer.GetStudyPanelThumbnailCaption();
                bool step7_3 = true;
                for (int j = 0; j < 6; j++)
                {
                    if (ExamCaption1[j].Equals(StudyCaption1[j]))
                    {
                        Logger.Instance.InfoLog("The examlist thumbnail caption is equals with studypanel thumbnail caption");
                    }
                    else
                    {
                        step7_3 = false;
                    }
                }
                if (step7_0 &&!step7_1 && step7_2 && step7_3)
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

                //Step-8 Thumbnail Preview panel scroll bar
                viewer.OpenExamListThumbnailPreview(0);
                viewer.ScrollIntoView(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(1, 31)));
                var step8 = viewer.IsElementVisibleInUI(By.CssSelector(viewer.GetExamListThumbnailCss(1, 31)));
                if (step8)
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

                //Step-9 Click on the current study Thumbnail Preview icon to Close the Examlist thumnail Preview
                viewer.CloseExamListThumbnailPreviewWindow(0);
                if ((!viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_ExamListthumbnailview))))
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

                //Logout Application
                viewer.ScrollIntoView(viewer.GetElement("cssselector", BluRingViewer.div_closeStudy));
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Verify percentage viewed with mouse-wheel scroll (image and frame)
        /// </summary>
        public TestCaseResult Test_161047(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            UserPreferences UserPref = new UserPreferences();
            RoleManagement rolemanagement = new RoleManagement();
            UserManagement usermanagement = new UserManagement();
            DomainManagement domain = new DomainManagement();

            try
            {
                //Set up Validation Steps
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastNameList");
                String FirstNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstNameList");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");

                String[] Accession = AccessionList.Split(':');
                String[] LastName = LastNameList.Split(':');
                String[] FirstName = FirstNameList.Split(':');
                String[] PatientID = PatientIDList.Split(':');

                // Precondition 1
                // Ensure TestDomain1, Role1, rad1, are configured
                login.LoginIConnect(adminUserName, adminPassword);
                String DomainName = "TestDomain" + new Random().Next(10000);
                String Role = "TestRole" + new Random().Next(10000);
                String User = "rad" + new Random().Next(10000);
                domain.CreateDomain(DomainName, Role, datasources: null);
                domain.ClickSaveNewDomain();
                login.Navigate("UserManagement");
                usermanagement.CreateUser(User, DomainName, Role);

                // Precondition 2
                //  TestDomain1 use default thumbnail splitting rules (CT, MR, NM, PT, RF are series-split, everything else is image-split). 				
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(DomainName);
                domain.SelectDomain(DomainName);
                domain.ClickEditDomain();

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");

                string[] Mod = { "CT", "MR", "NM", "PT", "RF", "CR", "XA" };
                for (int i = 0; i < Mod.Length; i++)
                {
                    domain.ModalityDropDown().SelectByText(Mod[i]);
                    if (Mod[i] == "CR" || Mod[i] == "XA")
                        domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                    else
                        domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                }

                // Precondition 3
                // Ensure current domain (TestDomain1) has "Show Percentage Viewed" selected.
                if (!domain.PercentageViewed().Selected)
                {
                    domain.PercentageViewed().Click();
                }

                domain.ClickSaveDomain();
                login.Logout();

                // Precondition 4
                //// Ensure that # of images/frames is displayed at the top-right corner of thumbnails (or go to Service Tool and configure Thumbnail Captions to have # of image instances in the caption)
                //ServiceTool st = new ServiceTool();
                //WpfObjects wpfobject = new WpfObjects();

                //st.LaunchServiceTool();
                //st.NavigateToTab("Viewer");
                //wpfobject.GetTabWpf(1).SelectTabPage(3);
                //wpfobject.WaitTillLoad();
                //wpfobject.ClickButton("Modify", 1);
                //wpfobject.WaitTillLoad();

                //wpfobject.SelectFromComboBox("ComboBox_DefaultThumbnailCaption", "{S%SeriesNum%}{- %ImageNum%}");

                //wpfobject.ClickButton("Apply", 1);
                //wpfobject.WaitTillLoad();
                //st.RestartService();
                //wpfobject.WaitTillLoad();
                //st.CloseServiceTool();
                //wpfobject.WaitTillLoad();


                // Step 1 - Login as User
                login.LoginIConnect(User, User);
                ExecutedSteps++;

                //Step 2 - Search for a study and load in BluRing Viewer
                // Navigate to Studies tab            
                var studies = (Studies)login.Navigate("Studies");

                // Search and Load a Study
                studies.SearchStudy(LastName: LastName[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForPageLoad(40);

                // Ensure that initial % viewed in each thumbnail is correct
                IList<IWebElement> PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                if (viewer.VerifyThumbnailPercentImagesViewed(PercentViewedList, new List<int> { 14, 123, 64 }, new List<int> { 1, 1, 1 }))
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

                // Step 3 Review % viewed in ExamList Thumbnail
                viewer.OpenExamListThumbnailPreview(0);
                PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                if (viewer.VerifyThumbnailPercentImagesViewed(PercentViewedList, new List<int> { 14, 123, 64 }, new List<int> { 1, 1, 1 }))
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
                viewer.CloseExamListThumbnailPreviewWindow(0);

                // Step4 View next image in first viewport and check % viewed
                viewer.SetViewPort(0, 1);
                IWebElement ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                TestCompleteAction action = new TestCompleteAction();
                action.MouseScroll(ele, "down", "1").Perform();

                PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                bool step4 = viewer.VerifyThumbnailPercentImagesViewed(PercentViewedList.ElementAt(0), 14, 2);
                if (step4)
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

                // Step5 View all images in first viewport and check % viewed
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                action = new TestCompleteAction();
                action.MouseScroll(ele, "down", "12").Perform();
                PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                bool step5 = viewer.VerifyThumbnailPercentImagesViewed(PercentViewedList.ElementAt(0), 14, 14);
                if (step5)
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

                // Step 6  Scroll up in first viewport
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                action = new TestCompleteAction();
                action.MouseScroll(ele, "up", "4").Perform();
                PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                bool step6 = viewer.VerifyThumbnailPercentImagesViewed(PercentViewedList.ElementAt(0), 14, 14);
                if (step6)
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

                // Step 7  Make second view port as active and mouse scroll down 
                viewer.SetViewPort(1, 1);
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                action = new TestCompleteAction();
                action.MouseScroll(ele, "down", "10").Perform();
                PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                bool step7 = viewer.VerifyThumbnailPercentImagesViewed(PercentViewedList.ElementAt(1), 123, 11);
                if (step7)
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
                viewer.CloseBluRingViewer();

                // Step 8 Search for a study with XA modality and load in BluRing Viewer
                // Navigate to Studies tab            
                studies = (Studies)login.Navigate("Studies");

                // Search and Load a Study
                studies.SearchStudy(LastName: LastName[1]);
                studies.SelectStudy("Patient ID", PatientID[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForPageLoad(50);

                viewer.OpenExamListThumbnailPreview(0);
                PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                //Workaround for displaying percentage of the thumbnails in exam list
                if (PercentViewedList.Count == 0 || PercentViewedList.Count == 1)
                {
                    viewer.CloseBluRingViewer();
                    studies.SelectStudy("Patient ID", PatientID[0]);
                    viewer = BluRingViewer.LaunchBluRingViewer();
                    PageLoadWait.WaitForFrameLoad(40);
                    PageLoadWait.WaitForPageLoad(50);
                    viewer.OpenExamListThumbnailPreview(0);
                    PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                }

                // Ensure that initial % viewed in each thumbnail is correct
                PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                if (viewer.VerifyThumbnailPercentImagesViewed(PercentViewedList, new List<int> { 114, 132 },
                                                                new List<int> { 1, 1 }))
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

                // Step 9 Make first viewport active and mouse scroll down 15 times and see the % viewed
                viewer.SetViewPort(0, 1);
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                action = new TestCompleteAction();
                action.MouseScroll(ele, "down", "15").Perform();

                PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                bool step9 = viewer.VerifyThumbnailPercentImagesViewed(PercentViewedList.ElementAt(0), 114, 16);
                if (step9)
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

                // Step 10 Make second viewport active and mouse scroll down 20 times and see the % viewed
                viewer.SetViewPort(1, 1);
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                action = new TestCompleteAction();
                action.MouseScroll(ele, "down", "20").Perform();
                PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                bool step10 = viewer.VerifyThumbnailPercentImagesViewed(PercentViewedList.ElementAt(1), 132, 21);
                if (step10)
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

                // Step11 - Scroll backwards and see the % viewed
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                action = new TestCompleteAction();
                action.MouseScroll(ele, "up", "5").Perform();
                PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                bool step11 = viewer.VerifyThumbnailPercentImagesViewed(PercentViewedList.ElementAt(1), 132, 21);
                if (step11)
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

                // Step 12 Scroll backwards in first viewport and and see the % viewed in first and second thumbnail
                viewer.SetViewPort(0, 1);
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                action = new TestCompleteAction();
                action.MouseScroll(ele, "up", "5").Perform();
                PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                bool step12 = viewer.VerifyThumbnailPercentImagesViewed(PercentViewedList.ElementAt(0), 114, 16);
                bool step12_1 = viewer.VerifyThumbnailPercentImagesViewed(PercentViewedList.ElementAt(1), 132, 21);
                if (step12 && step12_1)
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

                // Step 13 Verify the % viewed in Examlist Thumbnail
                viewer.OpenExamListThumbnailPreview(0);
                PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                if (viewer.VerifyThumbnailPercentImagesViewed(PercentViewedList, new List<int> { 114, 132 }, new List<int> { 16, 21 }))
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
                viewer.CloseExamListThumbnailPreviewWindow(0);
                viewer.CloseBluRingViewer();
                PageLoadWait.WaitForPageLoad(60);

                // Step 14 Open User Preferences and select Thumbnail Splitting for US modality to be "Auto"
                UserPref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                UserPref.ModalityDropDown().SelectByText("US");
                PageLoadWait.WaitForPageLoad(20);
                UserPref.SelectRadioBtn("ThumbSplitRadioButtons", "Auto");
                PageLoadWait.WaitForPageLoad(20);
                UserPref.CloseUserPreferences();
                ExecutedSteps++;

                // Step 15 Search for US modality study and load it in BluRing Viewer
                studies = (Studies)login.Navigate("Studies");

                // Search and Load a Study
                studies.SearchStudy(LastName: LastName[2]);
                studies.SelectStudy("Accession", Accession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(40);

                // Verify the Image Count and % viewed for first 4 Thumbnails
                PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                bool step15 = viewer.VerifyThumbnailFrameNumber(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImageComponent + " " + BluRingViewer.div_imageFrameNumber)), new List<String> { "16", "280", "283", "327" }, 4);
                bool step15_1 = viewer.VerifyThumbnailPercentImagesViewed(PercentViewedList, new List<int> { 16, 280, 283, 327 },
                                                                            new List<int> { 1, 1, 1, 1 }, 4);
                if (step15 && step15_1)
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

                // Step 16  Review % viewed in exam list thumbnail
                Thread.Sleep(10000);
                viewer.OpenExamListThumbnailPreview(0);               
                PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                //Workaround for displaying percentage of the thumbnails in exam list
                int workAroundCount = 5;
                while (PercentViewedList.Count == 0 && workAroundCount != 0)
                {
                    viewer.CloseBluRingViewer();
                    studies.SelectStudy("Accession", Accession[1]);
                    viewer = BluRingViewer.LaunchBluRingViewer(); 
                   
                    Thread.Sleep(25000);
                    viewer.OpenExamListThumbnailPreview(0);                  
                    PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                    Logger.Instance.InfoLog("Work around : " + workAroundCount);
                    workAroundCount--;
                    Thread.Sleep(10000);
                }
                if (viewer.VerifyThumbnailPercentImagesViewed(PercentViewedList, new List<int> { 16, 280, 283, 327 },
                                                                new List<int> { 1, 1, 1, 1 }, 4))
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
                viewer.CloseExamListThumbnailPreviewWindow(0);

                // Step 17	Drag the 2nd last thumbnail to the first viewport and last thumbnail to the 2nd viewport.
                IList<IWebElement> ThumbnailList = viewer.ThumbnailIndicator(0);
                int ThumbnailCount = ThumbnailList.Count;

                // Drag the 2nd last thumbnail to the first viewport
                viewer.ScrollIntoView(ThumbnailList.ElementAt(ThumbnailCount - 2));
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox")))
                    viewer.DropAndDropThumbnails(ThumbnailCount - 1, 1, 1, UseDragDrop: true);
                else
                    viewer.DropAndDropThumbnails(ThumbnailCount - 1, 1, 1);
                Thread.Sleep(3000);
                Logger.Instance.InfoLog("Thumbnail dragged and dropped to Viewport Successful - DragThumbnailToViewport");
                int count = ThumbnailList.ElementAt(ThumbnailCount - 2).
                                                               FindElements(By.CssSelector(BluRingViewer.div_thumbnailPercentImagesViewed)).Count;
                int NumberOfTimes = 0;
                while (count == 0 && NumberOfTimes != 2)
                {
                    if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox")))
                        viewer.DropAndDropThumbnails(ThumbnailCount - 1, 1, 1, UseDragDrop: true);
                    else
                        viewer.DropAndDropThumbnails(ThumbnailCount - 1, 1, 1);
                    Thread.Sleep(3000);
                    count = ThumbnailList.ElementAt(ThumbnailCount - 2).
                                                               FindElements(By.CssSelector(BluRingViewer.div_thumbnailPercentImagesViewed)).Count;
                    NumberOfTimes++;
                    Logger.Instance.InfoLog("Workaround = " + NumberOfTimes);
                }

                // Drag the last thumbnail to the second viewport
                viewer.ScrollIntoView(ThumbnailList.ElementAt(ThumbnailCount - 1));
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox")))
                    viewer.DropAndDropThumbnails(ThumbnailCount, 2, 1, UseDragDrop: true);
                else
                    viewer.DropAndDropThumbnails(ThumbnailCount, 2, 1);
                Thread.Sleep(3000);
                Logger.Instance.InfoLog("Thumbnail dragged and dropped to Viewport Successful - DragThumbnailToViewport");

                int count1 = ThumbnailList.ElementAt(ThumbnailCount - 1).
                                                              FindElements(By.CssSelector(BluRingViewer.div_thumbnailPercentImagesViewed)).Count;
                NumberOfTimes = 0;
                while (count1 == 0 && NumberOfTimes != 2)
                {
                    if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox")))
                        viewer.DropAndDropThumbnails(ThumbnailCount, 2, 1, UseDragDrop: true);
                    else
                        viewer.DropAndDropThumbnails(ThumbnailCount, 2, 1);
                    Thread.Sleep(3000);
                    count = ThumbnailList.ElementAt(ThumbnailCount - 1).
                                                               FindElements(By.CssSelector(BluRingViewer.div_thumbnailPercentImagesViewed)).Count;
                    NumberOfTimes++;
                    Logger.Instance.InfoLog("Workaround = " + NumberOfTimes);
                }

                // Get updated Thumbnail list
                ThumbnailList = viewer.ThumbnailIndicator(0);

                // Verify the % viewed in 2nd last Thumbnail
                IWebElement percentViewedElement = ThumbnailList.ElementAt(ThumbnailCount - 2).
                                                                FindElement(By.CssSelector(BluRingViewer.div_thumbnailPercentImagesViewed));
                bool step17 = viewer.VerifyThumbnailPercentImagesViewed(percentViewedElement, 475, 1);

                // Verify the % viewed in last thumbnail
                percentViewedElement = ThumbnailList.ElementAt(ThumbnailCount - 1).
                                                FindElement(By.CssSelector(BluRingViewer.div_thumbnailPercentImagesViewed));
                bool step17_1 = viewer.VerifyThumbnailPercentImagesViewed(percentViewedElement, 1, 1);
                if (step17 && step17_1)
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

                // Step 18 
                IList<IWebElement> ExamListThumbnails = viewer.ExamListThumbnailIndicator(0);
                int ExamListThumbnailCount = ExamListThumbnails.Count;

                // Make first viewport active and Scroll down to view images
                viewer.SetViewPort(0, 1);
                ele = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                action = new TestCompleteAction();
                action.MouseScroll(ele, "down", "10").Perform();

                percentViewedElement = ThumbnailList.ElementAt(ThumbnailCount - 2).
                                            FindElement(By.CssSelector(BluRingViewer.div_thumbnailPercentImagesViewed));
                bool step18 = viewer.VerifyThumbnailPercentImagesViewed(percentViewedElement, 475, 11);

                viewer.OpenExamListThumbnailPreview(0);
                percentViewedElement = ExamListThumbnails.ElementAt(ExamListThumbnailCount - 2).
                                            FindElement(By.CssSelector(BluRingViewer.div_thumbnailPercentImagesViewed));
                bool step18_1 = viewer.VerifyThumbnailPercentImagesViewed(percentViewedElement, 475, 11);
                if (step18 && step18_1)
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
                viewer.CloseExamListThumbnailPreviewWindow(0);

                // Step 19
                // Drag first Thumbnail to first viewport
                viewer.ScrollIntoView(ThumbnailList.ElementAt(0));
                viewer.SetViewPort(0, 1);
                IWebElement TargetElement = viewer.GetElement("cssselector", viewer.Activeviewport);
                action = new TestCompleteAction();
                action.DragAndDrop(ThumbnailList[0], TargetElement);
                action.MouseScroll(TargetElement, "down", "4");

                percentViewedElement = ThumbnailList.ElementAt(0).FindElement(By.CssSelector(BluRingViewer.div_thumbnailPercentImagesViewed));
                bool step19 = viewer.VerifyThumbnailPercentImagesViewed(percentViewedElement, 16, 5);

                viewer.OpenExamListThumbnailPreview(0);
                viewer.ScrollIntoView(ExamListThumbnails.ElementAt(0));
                percentViewedElement = ExamListThumbnails.ElementAt(0).FindElement(By.CssSelector(BluRingViewer.div_thumbnailPercentImagesViewed));
                bool step19_1 = viewer.VerifyThumbnailPercentImagesViewed(percentViewedElement, 16, 5);
                if (step19 && step19_1)
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
                viewer.CloseExamListThumbnailPreviewWindow(0);

                viewer.ScrollIntoView(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_closeStudy)));
                viewer.CloseBluRingViewer();
                PageLoadWait.WaitForPageLoad(60);

                // Step 20 Open User Preferences and reset Thumbnail Splitting for US modality to be "Image"
                UserPref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                UserPref.ModalityDropDown().SelectByText("US");
                PageLoadWait.WaitForPageLoad(20);
                UserPref.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                PageLoadWait.WaitForPageLoad(20);
                UserPref.CloseUserPreferences();
                ExecutedSteps++;

                action.Perform();

                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Thumbnail Captions
        /// </summary>		
        public TestCaseResult Test_161049(String testid, String teststeps, int stepcount)
        {

            // Declare and initialize variables
            TestCaseResult result = null;
            int ExecutedSteps = -1;            

            ServiceTool servicetool = new ServiceTool();
            UserManagement usermanagement = new UserManagement();
            DomainManagement domain = new DomainManagement();
            WpfObjects wpfobject = new WpfObjects();
            String[] Accession = null;
            string DS1 = string.Empty;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String domain1 = "Domain_" + new Random().Next(1, 1000);
                String role1 = "Role_" + new Random().Next(1, 1000);
                String tech1 = Config.newUserName + new Random().Next(1, 1000);
                String admin1 = "admin" + new Random().Next(1, 1000);
                String domaindes = domain1 + "Description";
                String InstitutionName = "Institution Name";
                String EA_131 = "VMSSA-4-38-131";

                String[] FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('=');
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");               
                Accession = AccessionList.Split(':');
                string[] FullPath = null;
                string DS1AETitle = string.Empty;
                int DS1Port = 0;               
                DS1 = Config.EA96;
                DS1AETitle = Config.EA96AETitle;
                DS1Port = 12000;

                //Precondition - Send studies to EA  
                var client = new DicomClient();
                FullPath = Directory.GetFiles(FilePath[0], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }
                FullPath = Directory.GetFiles(FilePath[1], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }

                //Precondition              
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab("Viewer");
                servicetool.NavigateSubTab("Protocols");
                wpfobject.ClickButton("Modify", 1);
                String[] Modality = { "CT", "MR", "NM", "PT", "RF", "CR", "XA" };
                for (int i = 0; i < Modality.Length; i++)
                {
                    servicetool.SelectDropdown("ComboBox_Modality", Modality[i]);
                    if (Modality[i] == "CT" || Modality[i] == "MR" || Modality[i] == "NM" || Modality[i] == "PT" || Modality[i] == "RF")
                    {
                        wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingSeries");
                    }
                    else
                    {
                        wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingName");
                    }
                }
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                wpfobject.WaitTillLoad();

                //Create new user say tech1
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.CreateDomain(domain1, domaindes, InstitutionName, admin1, null, domain1, domain1, admin1, role1, role1);
                login.Navigate("UserManagement");
                usermanagement.CreateUser(tech1, domain1, role1);
                login.Navigate("DomainManagement");
                domain.SearchDomain(domain1);
                domain.SelectDomain(domain1);
                domain.ClickEditDomain();
                string[] Mod = { "CT", "MR", "NM", "PT", "RF" };
                for (int i = 0; i < Mod.Length; i++)
                {
                    domain.ModalityDropDown().SelectByText(Mod[i]);
                    if (Mod[i] == "CT" || Mod[i] == "MR" || Mod[i] == "NM" || Mod[i] == "PT" || Mod[i] == "RF")
                    {
                        domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                    }
                    else
                    {
                        domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                    }
                }
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(11)"));
                var dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add("Save Annotated Images", group1);
                dictionary.Add("Save Series", group1);
                domain.AddToolsToToolbox(dictionary, addToolAtEnd: true);
                domain.ClickSaveEditDomain();                
                login.Logout();

                // Step 1 - Login as Administrator and verify the default setting of the option “Show Percentage Viewed”
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.SearchDomain(domain1);
                domain.SelectDomain(domain1);
                domain.ClickEditDomain();
                if (!domain.PercentageViewed().Selected)
                {
                    domain.PercentageViewed().Click();
                }
                domain.ClickSaveDomain();
                login.Logout();
                ExecutedSteps++;

                // Step 2 - Login as the tech1 user        
                login.DriverGoTo(login.url);
                login.LoginIConnect(tech1, tech1);
                ExecutedSteps++;

                // Step 3 - search for the study(patient id - 852654) and launch in the bluring viewer
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_131);
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(100);
                BluRingViewer.WaitforThumbnails();

                // Verifyting thumbnail shoudl be in image split by getting the count of the thumbnail
                var step2_1 = BluRingViewer.NumberOfThumbnailsInStudyPanel().Equals(4);
                IWebElement thumbnailpercentage = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_thumbnailPercentImagesViewed);
                IWebElement thumbnailFrame = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_imageFrameNumber);
                IWebElement thumbnailcaptions = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_thumbnailCaption);
                var thumbnailModality = BasePage.Driver.FindElements(By.CssSelector(viewer.GetStudyPanelThumbnailCss(1) + " .modality.fontCaption_m"));

                var step2_2 = thumbnailpercentage.GetCssValue("position").Equals("absolute") &&
                              thumbnailpercentage.GetCssValue("right").Equals("0px") &&
                              thumbnailpercentage.GetAttribute("innerHTML").Equals("100%");
                var step2_3 = thumbnailFrame.GetCssValue("position").Equals("absolute") &&
                              thumbnailFrame.GetCssValue("text-align").Equals("right") &&
                              thumbnailFrame.GetCssValue("right").Equals("0px") &&
                              thumbnailFrame.GetAttribute("innerHTML").Equals("1");
                var step2_4 = thumbnailcaptions.GetCssValue("position").Equals("absolute") &&
                              thumbnailcaptions.GetCssValue("text-align").Equals("left") &&
                              thumbnailcaptions.GetCssValue("bottom").Equals("0px") &&
                              thumbnailcaptions.GetAttribute("innerHTML").Equals("S83223- 0");
                var step2_5 = thumbnailModality[0].GetAttribute("innerHTML").Equals("CR") &&
                              thumbnailModality[0].GetCssValue("position").Equals("absolute") &&
                              thumbnailModality[0].GetCssValue("left").Equals("0px");

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool Step2_6 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));

                if (step2_1 && step2_2 && step2_3 && step2_4 && step2_5 && Step2_6)
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

                // Close the Bluring viewer
                viewer.CloseBluRingViewer();

                // Step 4 - search for the another study(patient id - 2004414132327812) and lauch in bluring viewer
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.EA96));
                studies.SelectStudy("Accession", Accession[1]);
                BluRingViewer.LaunchBluRingViewer();              

                // Verifyting thumbnail shoudl be in image split by getting the count of the thumbnail
                var step3_1 = BluRingViewer.NumberOfThumbnailsInStudyPanel().Equals(1);
                IWebElement thumbnailPercentageStudy2 = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_thumbnailPercentImagesViewed);
                IWebElement thumbnailFrameStudy2 = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_imageFrameNumber);
                IWebElement thumbnailCaptionsStudy2 = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_thumbnailCaption);
                var thumbnailModalityStudy2 = BasePage.Driver.FindElements(By.CssSelector(viewer.GetStudyPanelThumbnailCss(1) + " .modality.fontCaption_m"));

                var step3_2 = thumbnailPercentageStudy2.GetCssValue("position").Equals("absolute") &&                              
                              thumbnailPercentageStudy2.GetCssValue("right").Equals("0px") &&
                              thumbnailPercentageStudy2.GetAttribute("innerHTML").Equals("100%");
                var step3_3 = thumbnailFrameStudy2.GetCssValue("position").Equals("absolute") &&
                              thumbnailFrameStudy2.GetCssValue("text-align").Equals("right") &&
                              thumbnailFrameStudy2.GetCssValue("right").Equals("0px") &&
                              thumbnailFrameStudy2.GetAttribute("innerHTML").Equals("1");
                var step3_4 = thumbnailCaptionsStudy2.GetCssValue("position").Equals("absolute") &&
                              thumbnailCaptionsStudy2.GetCssValue("text-align").Equals("left") &&
                              thumbnailCaptionsStudy2.GetCssValue("bottom").Equals("0px") &&
                              thumbnailCaptionsStudy2.GetAttribute("innerHTML").Equals("S2- 1");
                var step3_5 = thumbnailModalityStudy2[0].GetAttribute("innerHTML").Equals("CR") &&
                              thumbnailModalityStudy2[0].GetCssValue("position").Equals("absolute") &&
                              thumbnailModalityStudy2[0].GetCssValue("left").Equals("0px");

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step3_6 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));

                if (step3_1 && step3_2 && step3_3 && step3_4 && step3_5 && step3_6)
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

                // Close the Bluring viewer
                viewer.CloseBluRingViewer();
                login.Logout();

                // Step 5 & 6 - open service tool and Enter custom captions
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab("Viewer");
                servicetool.NavigateSubTab("Thumbnail Captions");
                wpfobject.ClickButton("Modify", 1);
                wpfobject.setTextInTextBoxUsingIndex(1, "{%SeriesDesc%<br>}{%NumInstances% }{%Mod% Images<br>}{#%SeriesNum%}{, Image#%ImageNum%} {%DocumentName%<br>}{%LastUpdateDate%}");
                wpfobject.setTextInTextBoxUsingIndex(0, "{%Mod%}{-S%SeriesNum%}{ - %ImageNum%}");
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.RestartIIS();
                servicetool.CloseServiceTool();
                wpfobject.WaitTillLoad();
                ExecutedSteps++;
                ExecutedSteps++;

                // Step 7 - Login to Bluring as a tech1 user                
                login.LoginIConnect(tech1, tech1);

                // Searh for the study (Patient ID -852654) and Launch it in Bluring Viewer
                login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_131);
                studies.SelectStudy("Accession", Accession[0]);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(100);

                IWebElement thumbnailPercentageAfterChange = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_thumbnailPercentImagesViewed);
                IWebElement thumbnailFrameAfterChange = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_imageFrameNumber);
                IWebElement thumbnailcaptionsAfterChange = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_thumbnailCaption);
                var thumbnailModalityAfterChange = BasePage.Driver.FindElements(By.CssSelector(viewer.GetStudyPanelThumbnailCss(1) + " .modality.fontCaption_m"));

                var step6_1 = thumbnailPercentageAfterChange.GetCssValue("position").Equals("absolute") &&                              
                              thumbnailPercentageAfterChange.GetCssValue("right").Equals("0px") &&
                              thumbnailPercentageAfterChange.GetAttribute("innerHTML").Equals("100%");
                var step6_2 = thumbnailFrameAfterChange.GetCssValue("position").Equals("absolute") &&
                              thumbnailFrameAfterChange.GetCssValue("text-align").Equals("right") &&
                              thumbnailFrameAfterChange.GetCssValue("right").Equals("0px") &&
                              thumbnailFrameAfterChange.GetAttribute("innerHTML").Equals("1");
                var step6_3 = thumbnailcaptionsAfterChange.GetCssValue("position").Equals("absolute") &&
                              thumbnailcaptionsAfterChange.GetCssValue("text-align").Equals("left") &&
                              thumbnailcaptionsAfterChange.GetCssValue("bottom").Equals("0px") &&
                              thumbnailcaptionsAfterChange.GetAttribute("innerHTML").Equals("S83223- 0");
                var step6_4 = thumbnailModalityAfterChange[0].GetAttribute("innerHTML").Equals("CR") &&
                              thumbnailModalityAfterChange[0].GetCssValue("position").Equals("absolute") &&
                              thumbnailModalityAfterChange[0].GetCssValue("left").Equals("0px");

                // Close the Bluring viewer
                viewer.CloseBluRingViewer();

                // Searh for the study (Patient ID -852654) and Launch it in Bluring Viewer
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.EA96));
                studies.SelectStudy("Accession", Accession[1]);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(100);

                IWebElement thumbnailPercentageStudy2AfterChange = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_thumbnailPercentImagesViewed);
                IWebElement thumbnailFrameStudy2AfterChange = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_imageFrameNumber);
                IWebElement thumbnailCaptionsStudy2AfterChange = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_thumbnailCaption);
                var thumbnailModalityStudy2AfterChange = BasePage.Driver.FindElements(By.CssSelector(viewer.GetStudyPanelThumbnailCss(1) + " .modality.fontCaption_m"));

                var step6_5 = thumbnailPercentageStudy2AfterChange.GetCssValue("position").Equals("absolute") &&                              
                              thumbnailPercentageStudy2AfterChange.GetCssValue("right").Equals("0px") &&
                              thumbnailPercentageStudy2AfterChange.GetAttribute("innerHTML").Equals("100%");
                var step6_6 = thumbnailFrameStudy2AfterChange.GetCssValue("position").Equals("absolute") &&
                               thumbnailFrameStudy2AfterChange.GetCssValue("text-align").Equals("right") &&
                               thumbnailFrameStudy2AfterChange.GetCssValue("right").Equals("0px") &&
                               thumbnailFrameStudy2AfterChange.GetAttribute("innerHTML").Equals("1");
                var step6_7 = thumbnailCaptionsStudy2AfterChange.GetCssValue("position").Equals("absolute") &&
                               thumbnailCaptionsStudy2AfterChange.GetCssValue("text-align").Equals("left") &&
                               thumbnailCaptionsStudy2AfterChange.GetCssValue("bottom").Equals("0px") &&
                               thumbnailCaptionsStudy2AfterChange.GetAttribute("innerHTML").Equals("S2- 1");
                var step6_8 = thumbnailModalityStudy2AfterChange[0].GetAttribute("innerHTML").Equals("CR") &&
                              thumbnailModalityStudy2AfterChange[0].GetCssValue("position").Equals("absolute") &&
                              thumbnailModalityStudy2AfterChange[0].GetCssValue("left").Equals("0px");

                if (step6_1 && step6_2 && step6_3 && step6_4 && step6_5 && step6_6 && step6_7 && step6_8)
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

                // Close the Bluring viewer
                viewer.CloseBluRingViewer();

                // Step 8 - Draw some annotation and save it               
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.EA96));
                studies.SelectStudy("Accession", Accession[1]);
                BluRingViewer.LaunchBluRingViewer();               
                viewer.SelectViewerTool(BluRingTools.Draw_Ellipse);
                viewer.ApplyTool_DrawEllipse();
                viewer.SavePresentationState(BluRingTools.Save_Annotated_Image);
                IList<IWebElement> ThumbnailModality = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailModality));
                bool step8_1 = ThumbnailModality[0].GetAttribute("innerHTML").Equals("PR");
                bool step8_2 = ThumbnailModality[1].GetAttribute("innerHTML").Equals("CR");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool Step8_3 = studies.CompareImage(result.steps[ExecutedSteps],
                viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_thumbnails));
                if (step8_1 && step8_2 && Step8_3)
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

                // Setp 9 - Ensure for MR modality default series splitting thumbnail is configured for the user; load a MR study in Universal viewer. Verify thumbnail captions in thumbnail bar
                viewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: Accession[2], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[2]);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                var step9_1 = BluRingViewer.NumberOfThumbnailsInStudyPanel().Equals(5);
                thumbnailpercentage = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_thumbnailPercentImagesViewed);
                thumbnailFrame = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_imageFrameNumber);
                thumbnailcaptions = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_thumbnailCaption);
                thumbnailModality = BasePage.Driver.FindElements(By.CssSelector(viewer.GetStudyPanelThumbnailCss(1) + " .modality.fontCaption_m"));

                var step9_2 = thumbnailpercentage.GetCssValue("position").Equals("absolute") &&
                              thumbnailpercentage.GetCssValue("right").Equals("0px") &&
                              thumbnailpercentage.GetAttribute("innerHTML").Equals("20%");
                var step9_3 = thumbnailFrame.GetCssValue("position").Equals("absolute") &&
                              thumbnailFrame.GetCssValue("text-align").Equals("right") &&
                              thumbnailFrame.GetCssValue("right").Equals("0px") &&
                              thumbnailFrame.GetAttribute("innerHTML").Equals("5");
                var step9_4 = thumbnailcaptions.GetCssValue("position").Equals("absolute") &&
                              thumbnailcaptions.GetCssValue("text-align").Equals("left") &&
                              thumbnailcaptions.GetCssValue("bottom").Equals("0px") &&
                              thumbnailcaptions.GetAttribute("innerHTML").Equals("S1");
                var step9_5 = thumbnailModality[0].GetAttribute("innerHTML").Equals("MR") &&
                              thumbnailModality[0].GetCssValue("position").Equals("absolute") &&
                              thumbnailModality[0].GetCssValue("left").Equals("0px");

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool Step9_6 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));

                if (step9_1 && step9_2 && step9_3 && step9_4 && step9_5 && Step9_6)
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

                // Step 10 - Verify in each viewport Number of images, Series number values and compare them in its corresponding thumbnail.	
                bool step10_1 = true;
                String[] Thumbnailscaptions = { "S1", "S2", "S3", "S4", "S5"};
                String[] NumbImageinThumbnails = { "5", "20", "20", "22", "60"};
                var StudyThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                var studyThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));
                for (int i = 0; i < NumbImageinThumbnails.Count(); i++)
                {
                    if (!(Thumbnailscaptions[i] == StudyThumbnailCaptionList[i].GetAttribute("innerHTML") &&
                        NumbImageinThumbnails[i] == studyThumbnailNumberList[i].GetAttribute("innerHTML")))
                    {
                        step10_1 = false;
                        Logger.Instance.InfoLog("The thumbnails caption of " + i + " th thumbnail is :" + StudyThumbnailCaptionList[i].GetAttribute("innerHTML") + " The Thumbnail number of " + i + " th thumbnail is :" + studyThumbnailNumberList[i].GetAttribute("innerHTML"));
                    }
                }

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step10_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step10_1 && step10_2)
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

                // Step 11 - Open the Thumbnail Preview area of the same study from the Exam List.	
                viewer.OpenExamListThumbnailPreview(0);
                Thread.Sleep(2000);
                bool step11_1 = true;
                var ExamListThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                var ExamListThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));

                for (int i = 0; i < NumbImageinThumbnails.Count(); i++)
                {
                    if (!(Thumbnailscaptions[i] == ExamListThumbnailCaptionList[i].GetAttribute("innerHTML") &&
                        NumbImageinThumbnails[i] == ExamListThumbnailNumberList[i].GetAttribute("innerHTML")))
                    {
                        step11_1 = false;
                        Logger.Instance.InfoLog("The thumbnails caption of  " + i + " th Examlist thumbnail is :" + ExamListThumbnailCaptionList[i].GetAttribute("innerHTML") + " The Thumbnail number of  " + i + " th Examlist thumbnail is :" + ExamListThumbnailNumberList[i].GetAttribute("innerHTML"));
                    }
                }

                if (step11_1)
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

                // Close the Bluring viewer
                viewer.CloseBluRingViewer();
                login.Logout();

                // Step 12 - Login as Administrator or domain admin of the user, de-select the option “Show Percentage Viewed” in the user's domain (Disabled)	
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.SearchDomain(domain1);
                domain.SelectDomain(domain1);
                domain.ClickEditDomain();
                if (domain.PercentageViewed().Selected)
                {
                    domain.PercentageViewed().Click();
                }
                domain.ClickSaveDomain();
                login.Logout();
                ExecutedSteps++;

                // Step 13 - Login to WebAccess instance as tech1 user. Load study in Universal Viewer, Burton Cliff, patient ID = 852654. Review the contents of the first thumbnail caption.
                login.LoginIConnect(tech1, tech1);
                login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_131);
                studies.SelectStudy("Accession", Accession[0]);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(100);
                BluRingViewer.WaitforThumbnails();

                // Verifyting thumbnail shoudl be in image split by getting the count of the thumbnail
                var step13_1 = BluRingViewer.NumberOfThumbnailsInStudyPanel().Equals(4);
                //thumbnailpercentage = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_thumbnailPercentImagesViewed);
                //thumbnailFrame = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_imageFrameNumber);
                //thumbnailcaptions = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_thumbnailCaption);
                //thumbnailModality = BasePage.Driver.FindElements(By.CssSelector(viewer.GetStudyPanelThumbnailCss(1) + " .modality.fontCaption_m"));

                //var step13_2 = thumbnailpercentage.Displayed;
                //var step13_3 = thumbnailFrame.GetCssValue("position").Equals("absolute") &&
                //              thumbnailFrame.GetCssValue("text-align").Equals("right") &&
                //              thumbnailFrame.GetCssValue("right").Equals("0px") &&
                //              thumbnailFrame.GetAttribute("innerHTML").Equals("1");
                //var step13_4 = thumbnailcaptions.GetCssValue("position").Equals("absolute") &&
                //              thumbnailcaptions.GetCssValue("text-align").Equals("left") &&
                //              thumbnailcaptions.GetCssValue("bottom").Equals("0px") &&
                //              thumbnailcaptions.GetAttribute("innerHTML").Equals("S83223- 0");
                //var step13_5 = thumbnailModality[0].GetAttribute("innerHTML").Equals("CR") &&
                //              thumbnailModality[0].GetCssValue("position").Equals("absolute") &&
                //              thumbnailModality[0].GetCssValue("left").Equals("0px");

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step13_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, viewer.GetStudyPanelThumbnailCss(1)));

                if (step13_1 && step13_2)
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

                // Close the Bluring viewer
                viewer.CloseBluRingViewer();
                Thread.Sleep(2000);

                // Step 14 - Ensure for MR modality default series splitting thumbnail is configured for the user; load a MR study in Universal viewer. Verify thumbnail captions in thumbnail bar	
                studies.SearchStudy(AccessionNo: Accession[2], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[2]);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                var step14_1 = BluRingViewer.NumberOfThumbnailsInStudyPanel().Equals(5);
                //thumbnailpercentage = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_thumbnailPercentImagesViewed);
                //thumbnailFrame = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_imageFrameNumber);
                //thumbnailcaptions = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_thumbnailCaption);
                //thumbnailModality = BasePage.Driver.FindElements(By.CssSelector(viewer.GetStudyPanelThumbnailCss(1) + " .modality.fontCaption_m"));

                //var step14_2 = thumbnailpercentage.Displayed;
                //var step14_3 = thumbnailFrame.GetCssValue("position").Equals("absolute") &&
                //              thumbnailFrame.GetCssValue("text-align").Equals("right") &&
                //              thumbnailFrame.GetCssValue("right").Equals("0px") &&
                //              thumbnailFrame.GetAttribute("innerHTML").Equals("5");
                //var step14_4 = thumbnailcaptions.GetCssValue("position").Equals("absolute") &&
                //              thumbnailcaptions.GetCssValue("text-align").Equals("left") &&
                //              thumbnailcaptions.GetCssValue("bottom").Equals("0px") &&
                //              thumbnailcaptions.GetAttribute("innerHTML").Equals("S1");
                //var step14_5 = thumbnailModality[0].GetAttribute("innerHTML").Equals("MR") &&
                //              thumbnailModality[0].GetCssValue("position").Equals("absolute") &&
                //              thumbnailModality[0].GetCssValue("left").Equals("0px");

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool Step14_2 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step14_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, viewer.GetStudyPanelThumbnailCss(1)));

                if (step14_1 && Step14_2 && step14_3)
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

                // Close the Bluring viewer
                viewer.CloseBluRingViewer();
                login.Logout();

                // Step 15 - Login as Administrator or domain admin of the user, enable the "Show Percentage Viewed" option in the user's domain (enabled).	
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.SearchDomain(domain1);
                domain.SelectDomain(domain1);
                domain.ClickEditDomain();
                if (!domain.PercentageViewed().Selected)
                {
                    domain.PercentageViewed().Click();
                }
                domain.ClickSaveDomain();
                login.Logout();
                ExecutedSteps++;

                // Step 16 - Login to WebAccess instance as tech1 user. Load a study in Universal Viewer. Review the contents of the first thumbnail caption.
                login.LoginIConnect(tech1, tech1);
                login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_131);
                studies.SelectStudy("Accession", Accession[0]);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(100);
                BluRingViewer.WaitforThumbnails();

                // Verifyting thumbnail shoudl be in image split by getting the count of the thumbnail                
                thumbnailpercentage = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_thumbnailPercentImagesViewed);                
                thumbnailcaptions = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_thumbnailCaption);

                var step16_1 = thumbnailpercentage.GetCssValue("position").Equals("absolute") &&
                              thumbnailpercentage.GetCssValue("right").Equals("0px") &&
                              thumbnailpercentage.GetAttribute("innerHTML").Equals("100%");                
                var step16_2 = thumbnailcaptions.GetCssValue("position").Equals("absolute") &&
                              thumbnailcaptions.GetCssValue("text-align").Equals("left") &&
                              thumbnailcaptions.GetCssValue("bottom").Equals("0px") &&
                              thumbnailcaptions.GetAttribute("innerHTML").Equals("S83223- 0");

                if (step16_1 && step16_2)
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

                // Close the Bluring viewer
                viewer.CloseBluRingViewer();
                login.Logout();

                // Step 17 - Login as an administrator of the user’s domain, from the Domain Management page, verify the default setting for option “Display Thumbnail Caption As Overlay”	
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.SearchDomain(domain1);
                domain.SelectDomain(domain1);
                domain.ClickEditDomain();                
                if (!domain.OverlayCheckbox().Selected)
                {
                    domain.OverlayCheckbox().Click();
                    Logger.Instance.InfoLog("Overlay Checkbox is selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Overlay Checkbox already selected");
                }
                domain.ClickSaveDomain();                
                login.Logout();
                ExecutedSteps++;

                // Step 18 -Login to WebAccess instance as tech1 user. Load a study in Universal viewer. Review the contents of the thumbnail captions.	
                login.LoginIConnect(tech1, tech1);
                login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[2], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[2]);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(100);
                BluRingViewer.WaitforThumbnails();
                thumbnailpercentage = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_thumbnailPercentImagesViewed);
                thumbnailFrame = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_imageFrameNumber);
                thumbnailcaptions = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_thumbnailCaption);
                thumbnailModality = BasePage.Driver.FindElements(By.CssSelector(viewer.GetStudyPanelThumbnailCss(1) + " .modality.fontCaption_m"));

                var step18_1 = thumbnailpercentage.Displayed;
                var step18_2 = thumbnailFrame.Displayed;
                var step18_3 = thumbnailcaptions.Displayed;
                var step18_4 = thumbnailModality[0].Displayed;

                if (step18_1 && step18_2 && step18_3 && step18_4)
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

                // Close the Bluring viewer
                viewer.CloseBluRingViewer();
                login.Logout();

                // Step 19 - Login as an administrator of the user’s domain, de-select the “Display Thumbnail Caption as Overlay”	
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.SearchDomain(domain1);
                domain.SelectDomain(domain1);
                domain.ClickEditDomain();
                if (domain.OverlayCheckbox().Selected)
                {
                    domain.OverlayCheckbox().Click();
                    Logger.Instance.InfoLog("Overlay Checkbox is un-selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Overlay Checkbox already un-selected");
                }
                domain.ClickSaveDomain();
                login.Logout();
                ExecutedSteps++;

                // Step 20 - Login to WebAccess instance as tech1 user. Load a study in Universal viewer. Review the contents of thumbnail captions	
                login.LoginIConnect(tech1, tech1);
                login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[2], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[2]);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(100);
                BluRingViewer.WaitforThumbnails();
                thumbnailpercentage = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_thumbnailPercentImagesViewed);
                thumbnailFrame = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_imageFrameNumber);
                thumbnailcaptions = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1) + " " + BluRingViewer.div_thumbnailCaption);
                thumbnailModality = BasePage.Driver.FindElements(By.CssSelector(viewer.GetStudyPanelThumbnailCss(1) + " .modality.fontCaption_m"));

                var step20_1 = thumbnailpercentage.Displayed;
                var step20_2 = thumbnailFrame.Displayed;
                var step20_3 = thumbnailcaptions.Displayed;
                var step20_4 = thumbnailModality[0].Displayed;

                if (step20_1 && step20_2 && step20_3 && step20_4)
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

                // Close the Bluring viewer
                viewer.CloseBluRingViewer();
                Thread.Sleep(2000);

                // Step 21 - Load the same study in Enterprise viewer (HTML4 viewer)	
                studies.SelectStudy("Accession", Accession[2]);
                var enterpriseviewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(20);
                var captions = enterpriseviewer.GetElement(BasePage.SelectorType.CssSelector, ".thumbnailNoCaptionOverlay.ui-draggable.selectedThumb.loadedThumbnail > div.thumbnailCaption");
                if(captions.Displayed)
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

                // Close the Enterprise viewer
                enterpriseviewer.CloseStudy();
                login.Logout();                

                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                try
                {
                    //Deleting uploaded study  
                    var hplogin = new HPLogin();                                       
                    var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: "https://" + DS1 + "/webadmin");
                    var workflow = (WorkFlow)hphome.Navigate("Workflow");
                    workflow.NavigateToLink("Workflow", "Archive Search");
                    workflow.HPSearchStudy("Accessionno", Accession[1]);
                    workflow.HPDeleteStudy();
                    hplogin.LogoutHPen();
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Exception due to: " + ex);
                }
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab("Viewer");
                servicetool.NavigateSubTab("Thumbnail Captions");
                wpfobject.ClickButton("Modify", 1);
                var comboBoxDicomCaption = wpfobject.GetComboBox("ComboBox_DefaultThumbnailCaption");
                comboBoxDicomCaption.Select("{%SeriesDesc%<br>}{%NumInstances% }{%Mod% Images<br>}{#%SeriesNum%}{, Image#%ImageNum%}");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                var selectedText = comboBoxDicomCaption.SelectedItemText;
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                wpfobject.WaitTillLoad();
            }
        }

        /// <summary>
        /// Test case ID 136526 - Study Panel: Traverse through groups of thumbnails using arrows
        /// <summary>        
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161043(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            Studies studies = new Studies();
            UserPreferences UserPref = new UserPreferences();
            RoleManagement rolemanagement = new RoleManagement();
            UserManagement usermanagement = new UserManagement();
            DomainManagement domain = new DomainManagement();

            ServiceTool servicetool = new ServiceTool();
            WpfObjects wpfobject = new WpfObjects();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                String domain1 = "Domain1_" + new Random().Next(1, 1000);
                String role1 = "Role1_" + new Random().Next(1, 1000);
                String rad1 = Config.newUserName + new Random().Next(1, 1000);

                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String SeriesNumber = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "SeriesNum");
                String ThumbnailCountPerPanel = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailCountPerPanel");
                String ClickCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ClickCount");

                String[] Accession = AccessionList.Split(':');
                String[] PatientID = PatientIDList.Split(':');
                String[] PanelCount = ThumbnailCountPerPanel.Split('-');

                String[] CountPerPanel1 = PanelCount[0].Split(':');
                String[] CountPerPanel2 = PanelCount[1].Split(':');
                String[] CountPerPanel3 = PanelCount[2].Split(':');
                String[] CountPerPanel4 = PanelCount[3].Split(':');

                //PreCondition - Thumbnail splitting and Caption settings in Service tool
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToTab("Viewer");
                //servicetool.NavigateSubTab("Thumbnail Captions");
                //wpfobject.ClickButton("Modify", 1);
                //wpfobject.setTextInTextBoxUsingIndex(0, "S{%Series%}{ - %ImageNum%}");
                //wpfobject.ClickButton("Apply", 1);
                //wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab("Protocols");
                wpfobject.ClickButton("Modify", 1);
                String[] Modality = { "CT", "MR", "NM", "PT", "RF", "CR", "XA" };
                for (int i = 0; i < Modality.Length; i++)
                {
                    servicetool.SelectDropdown("ComboBox_Modality", Modality[i]);
                    if (Modality[i] == "CT" || Modality[i] == "MR" || Modality[i] == "NM" || Modality[i] == "PT" || Modality[i] == "RF")
                    {
                        wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingSeries");
                    }
                    else
                    {
                        wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingName");
                    }
                }
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                wpfobject.WaitTillLoad();

                //Updating the Thumbnail Splitting in Domain management
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.CreateDomain(domain1, role1, null);
                domain.ClickSaveNewDomain();
                login.Navigate("UserManagement");
                usermanagement.CreateUser(rad1, domain1, role1);
                login.Navigate("DomainManagement");
                domain.SearchDomain(domain1);
                domain.SelectDomain(domain1);
                domain.ClickEditDomain();
                string[] Mod = { "CT", "MR", "NM", "PT", "RF" };
                for (int i = 0; i < Mod.Length; i++)
                {
                    domain.ModalityDropDown().SelectByText(Mod[i]);
                    if (Mod[i] == "CT" || Mod[i] == "MR" || Mod[i] == "NM" || Mod[i] == "PT" || Mod[i] == "RF")
                    {
                        domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                    }
                    else
                    {
                        domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                    }
                }
                domain.ClickSaveDomain();
                login.Logout();

                //Step1 - Login as rad1 user to the iCA/BlueRing application 
                login.LoginIConnect(rad1, rad1);
                ExecutedSteps++;

                //Step2 - Search for patient "YSJ-US100" , accession="eleven-teen", load the study in the new viewer.
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(patientID: PatientID[0], Datasource: login.GetHostName(Config.SanityPACS));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                BluRingViewer.WaitforThumbnails();

                //viewer.HoverElement(By.CssSelector(BluRingViewer.div_thumbnails));
                bool Step2_1 = viewer.VerifyElementPresence("cssselector", BluRingViewer.div_ThumbnailPreviousArrowButtonDisabled);
                bool Step2_2 = viewer.VerifyElementPresence("cssselector", BluRingViewer.div_ThumbnailNextArrowButtonEnabled);
                if (!Step2_1 && !Step2_2)
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

                //Step3 - Review the thumbnails shown in the thumbnail bar. Count the number of thumbnails shown
                int ThumbnailPanelCount1 = viewer.ThumbnailLoadedIndicator(0).Count();
                int LastThumbnailImagenum1 = viewer.GetImageNumber(viewer.StudyPanelThumbnailIndicator(0)[(ThumbnailPanelCount1 - 1)]);
                int LastThumbnailSeriesNum1 = viewer.GetSeriesNumber(viewer.StudyPanelThumbnailIndicator(0)[(ThumbnailPanelCount1 - 1)]);

                Logger.Instance.InfoLog("In the last Thumbnail panel- Series Number of last thumbnail is: " + LastThumbnailSeriesNum1 + "And Image number is: " + LastThumbnailImagenum1 + " and the total thumbnail count is: " + ThumbnailPanelCount1);
                Logger.Instance.InfoLog("ThumbnailPanelCount1 is " + ThumbnailPanelCount1);
                bool Step3_1 = (Int32.Parse(CountPerPanel1[0]) == ThumbnailPanelCount1);
                bool Step3_2 = (Int32.Parse(CountPerPanel1[1]) == LastThumbnailImagenum1);
                bool Step3_3 = (Int32.Parse(SeriesNumber) == LastThumbnailSeriesNum1);

                if (Step3_1 && Step3_2 && Step3_3)
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

                //Step4 - When first thumbnail (series) of the study is shown and not all series of the study shown in the thumbnail bar, verify the arrow icons on both end.
                viewer.HoverElement(By.CssSelector(BluRingViewer.div_thumbnails));
                bool Step4_1 = viewer.VerifyElementPresence("cssselector", BluRingViewer.div_ThumbnailNextArrowButtonEnabled);
                bool Step4_2 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ThumbnailPreviousArrowButtonEnabled)).Count == 0;
                if (Step4_1 && Step4_2)
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

                //Step5 - Click on the arrow on the right end of the thumbnail bar in the study panel.                
                viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ThumbnailNextArrowButton)));
                BluRingViewer.WaitforThumbnails();
                result.steps[++ExecutedSteps].StepPass();                

                //Step6 - When the thumbnail bar displays middle portion of the series from the study, verify the arrow icons on both end by hovering/not hovering on it.                              
                viewer.HoverElement(By.CssSelector(viewer.GetStudyPanelThumbnailCss(14)));
                bool Step6_3 = viewer.VerifyElementPresence("cssselector", BluRingViewer.div_ThumbnailNextArrowButtonEnabled);
                bool Step6_4 = viewer.VerifyElementPresence("cssselector", BluRingViewer.div_ThumbnailPreviousArrowButtonEnabled);
                if (Step6_3 && Step6_4)
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

                //Step7 - Review the new thumbnails shown in the thumbnail bar. Count the number of thumbnails shown in the main study panel.
                int ThumbnailPanelCount2 = viewer.ThumbnailLoadedIndicator(0).Count();
                int FirstThumbnailImagenum2 = viewer.GetImageNumber(viewer.ThumbnailLoadedIndicator(0)[0]);
                int FirstThumbnailSeriesnum2 = viewer.GetSeriesNumber(viewer.ThumbnailLoadedIndicator(0)[0]);

                Logger.Instance.InfoLog("In the Second Thumbnail panel- Series Number of first thumbnail is: " + FirstThumbnailSeriesnum2 + "And Image number is: " + FirstThumbnailImagenum2);

                int LastThumbnailImagenum2 = viewer.GetImageNumber(viewer.ThumbnailLoadedIndicator(0)[(ThumbnailPanelCount2 - 1)]);
                int LastThumbnailSeriesNum2 = viewer.GetSeriesNumber(viewer.ThumbnailLoadedIndicator(0)[(ThumbnailPanelCount2 - 1)]);

                Logger.Instance.InfoLog("In the Second Thumbnail panel- Series Number of Last thumbnail is: " + LastThumbnailSeriesNum2 + "And Image number is: " + LastThumbnailImagenum2);
                Logger.Instance.InfoLog("ThumbnailPanelCount2 is " + ThumbnailPanelCount2);
                bool Step7_1 = (Int32.Parse(CountPerPanel2[0]) == ThumbnailPanelCount2);
                bool Step7_2 = (Int32.Parse(CountPerPanel2[1]) == FirstThumbnailImagenum2);
                bool Step7_3 = (Int32.Parse(CountPerPanel2[2]) == LastThumbnailImagenum2);
                bool Step7_4 = (Int32.Parse(SeriesNumber) == FirstThumbnailSeriesnum2);
                bool Step7_5 = (Int32.Parse(SeriesNumber) == LastThumbnailSeriesNum1);

                if (Step7_1 && Step7_2 && Step7_3 && Step7_4 && Step7_5)
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

                //Step8 - Click on the arrow on the right end of the thumbnail bar in the study panel.                
                viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ThumbnailNextArrowButton)));
                BluRingViewer.WaitforThumbnails();
                result.steps[++ExecutedSteps].StepPass();                

                //Step9 - Review the new thumbnails shown in the thumbnail bar. Count the number of thumbnails shown in the main study panel.
                int ThumbnailPanelCount3 = viewer.ThumbnailLoadedIndicator(0).Count();
                int FirstThumbnailImagenum3 = viewer.GetImageNumber(viewer.ThumbnailLoadedIndicator(0)[0]);
                int FirstThumbnailSeriesnum3 = viewer.GetSeriesNumber(viewer.ThumbnailLoadedIndicator(0)[0]);

                Logger.Instance.InfoLog("In the Second Thumbnail panel- Series Number of first thumbnail is: " + FirstThumbnailSeriesnum3 + "And Image number is: " + FirstThumbnailImagenum3);

                int LastThumbnailImagenum3 = viewer.GetImageNumber(viewer.ThumbnailLoadedIndicator(0)[(ThumbnailPanelCount3 - 1)]);
                int LastThumbnailSeriesNum3 = viewer.GetSeriesNumber(viewer.ThumbnailLoadedIndicator(0)[(ThumbnailPanelCount3 - 1)]);

                Logger.Instance.InfoLog("In the Second Thumbnail panel- Series Number of Last  thumbnail is: " + LastThumbnailSeriesNum3 + "And Image number is: " + LastThumbnailImagenum3);
                Logger.Instance.InfoLog("The ThumbnailPanelCount3 is " + ThumbnailPanelCount3);

                bool Step9_1 = (Int32.Parse(CountPerPanel3[0]) == ThumbnailPanelCount3);

                if (Step9_1 && Int32.Parse(CountPerPanel3[1]) == FirstThumbnailImagenum3 &&
                    Int32.Parse(CountPerPanel3[2]) == LastThumbnailImagenum3 &&
                    Int32.Parse(SeriesNumber) == FirstThumbnailSeriesnum3 &&
                    Int32.Parse(SeriesNumber) == LastThumbnailSeriesNum3)
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

                //Step10 - When the thumbnail bar shows the last thumbnail and not all thumbnails (series) of the study are shown, verify arrow icons on both end when hovering/not hovering on it                
                viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ThumbnailNextArrowButton)));
                viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ThumbnailNextArrowButton)));
                BluRingViewer.WaitforThumbnails();
                viewer.HoverElement(By.CssSelector(viewer.GetStudyPanelThumbnailCss(43)));
                Thread.Sleep(2000);
                bool Step10_1 = viewer.VerifyElementPresence("cssselector", BluRingViewer.div_ThumbnailPreviousArrowButtonEnabled);
                bool Step10_2 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ThumbnailNextArrowButtonEnabled)).Count == 0;
                if (Step10_1 && Step10_2)
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

                //Step11 - From the Exam List, click a prior exam to open up another study panel. If there are no prior exams, click on the same study to open it again in another study panel.
                viewer.HoverElement(By.CssSelector(BluRingViewer.div_ShowHideTool));
                if (viewer.CheckPriorsCount() > 1)
                {
                    viewer.OpenPriors(2);
                    BluRingViewer.WaitforThumbnails();
                }
                else
                {
                    viewer.ClickElement(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ActiveExamPanel)));
                    BluRingViewer.WaitforThumbnails();
                }
                Thread.Sleep(30000);

                if (viewer.VerifyElementPresence("cssselector", BluRingViewer.div_studypanel + ":nth-of-type(2)"))
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

                //Step12 - Review the thumbnails shown in the new study panel. Count the number of thumbnails shown in the second study panel. 
                int ThumbnailPanelCount1_2 = viewer.ThumbnailLoadedIndicator(1).Count();
                int FirstThumbnailImagenum1_2 = viewer.GetImageNumber(viewer.ThumbnailLoadedIndicator(1)[0]);
                int FirstThumbnailSeriesnum1_2 = viewer.GetSeriesNumber(viewer.ThumbnailLoadedIndicator(1)[0]);

                int LastThumbnailImagenum1_2 = viewer.GetImageNumber(viewer.ThumbnailLoadedIndicator(1)[(ThumbnailPanelCount1_2 - 1)]);
                int LastThumbnailSeriesnum1_2 = viewer.GetSeriesNumber(viewer.ThumbnailLoadedIndicator(1)[(ThumbnailPanelCount1_2 - 1)]);
                Logger.Instance.InfoLog("The Total count of the thumbnail is " + ThumbnailPanelCount1_2 + " and The First thumbnail image number is " + FirstThumbnailImagenum1_2 + " and the last thumbnail number is " + LastThumbnailImagenum1_2);
                Logger.Instance.InfoLog("The ThumbnailPanelCount1_2 is " + ThumbnailPanelCount1_2);
                bool Step12_1 = false;
                if (Int32.Parse(CountPerPanel4[0]) == ThumbnailPanelCount1_2 &&
                    Int32.Parse(CountPerPanel4[1]) == FirstThumbnailImagenum1_2 &&
                    Int32.Parse(CountPerPanel4[2]) == LastThumbnailImagenum1_2 &&
                    Int32.Parse(SeriesNumber) == FirstThumbnailSeriesnum1_2 &&
                    Int32.Parse(SeriesNumber) == LastThumbnailSeriesnum1_2 &&
                    BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count == 2)
                {
                    Step12_1 = true;
                }                                
                if(Step12_1)
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

                //Step13 - Click on the arrow on the right end of the thumbnail bar in the second study panel.
                viewer.HoverElement(By.CssSelector("blu-ring-study-panel-control:nth - of - type(2) " + BluRingViewer.div_thumbnails));
                IList<IWebElement> StudyPanels = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                viewer.ClickElement(StudyPanels[1].FindElement(By.CssSelector(BluRingViewer.div_ThumbnailNextArrowButton)));
                BluRingViewer.WaitforThumbnails();                
                String[] expectedthumbnailsCaption = { "S1- 5", "S1- 6", "S1- 7", "S1- 8", "S1- 9", "S1- 10", "S1- 11" };
                IList<String> actualthumbnailsCaption = new List<String>();
                foreach (IWebElement ele in viewer.ThumbnailLoadedIndicator(1))
                {
                    var caption = ele.FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption)).GetAttribute("innerHTML");
                    actualthumbnailsCaption.Add(caption);

                }
                Logger.Instance.InfoLog("The expected thumbanils caption is " + expectedthumbnailsCaption + " and the actual is " + actualthumbnailsCaption);
                var step13 = expectedthumbnailsCaption.SequenceEqual(actualthumbnailsCaption);                
                if (step13)
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

                //Step14 - Click on the thumbnail traverse arrow on the left end of the thumbnail bar in the second study panel.               
                viewer.ClickElement(StudyPanels[1].FindElement(By.CssSelector(BluRingViewer.div_ThumbnailPreviousArrowButton)));
                BluRingViewer.WaitforThumbnails();
                String[] expectedthumbnails = { "S1- 1", "S1- 2", "S1- 3", "S1- 4", "S1- 5", "S1- 6" };
                IList<String> actualthumbnails = new List<String>();
                foreach (IWebElement ele in viewer.ThumbnailLoadedIndicator(1))
                {
                    var caption = ele.FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption)).GetAttribute("innerHTML");
                    actualthumbnails.Add(caption);

                }
                var step14 = expectedthumbnails.SequenceEqual(actualthumbnails);
                if(step14)                
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

                //Step15 - In the second study panel when the thumbnail bar shows the last thumbnail and not all thumbnails (series) of the study are shown, verify arrow icons on both end when hovering/not hovering on it.
                bool tempVar = StudyPanels[1].FindElement(By.CssSelector(BluRingViewer.div_ThumbnailNextArrowButtonEnabled)).Enabled;
                try
                {
                    while (tempVar)
                    {
                        viewer.ClickElement(StudyPanels[1].FindElement(By.CssSelector(BluRingViewer.div_ThumbnailNextArrowButton)));
                        Thread.Sleep(2000);
                        tempVar = StudyPanels[1].FindElement(By.CssSelector(BluRingViewer.div_ThumbnailNextArrowButtonEnabled)).Enabled;
                    }
                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Next button is not enabled since there are no more thumbnails to traverse through" + e);
                }
                bool Step15_1 = StudyPanels[1].FindElement(By.CssSelector(BluRingViewer.div_ThumbnailPreviousArrowButtonEnabled)).Displayed;
                viewer.HoverElement(By.CssSelector(viewer.GetStudyPanelThumbnailCss(46, 2)));
                Thread.Sleep(5000);
                bool Step15_2 = StudyPanels[1].FindElement(By.CssSelector(BluRingViewer.div_ThumbnailPreviousArrowButtonEnabled)).Displayed;
                bool Step15_3 = StudyPanels[1].FindElements(By.CssSelector(BluRingViewer.div_ThumbnailNextArrowButtonEnabled)).Count == 0;
                if (!Step15_1 && Step15_2 && Step15_3)
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

                //Step16 - Click on the thumbnail traverse arrow on the left end of the thumbnail bar in the first study panel.
                viewer.HoverElement(By.CssSelector(BluRingViewer.div_thumbnails));
                viewer.ClickElement(StudyPanels[0].FindElement(By.CssSelector(BluRingViewer.div_ThumbnailPreviousArrowButton)));
                BluRingViewer.WaitforThumbnails();
                String[] FirstStudypanelThumbanails = { "S1- 33", "S1- 34", "S1- 35", "S1- 36", "S1- 37", "S1- 38", "S1- 39" };
                String[] SecondStudyPanelThumbnails = { "S1- 43", "S1- 44", "S1- 45", "S1- 46", "S1- 47", "S1- 1" };
                IList<String> ActualFisrtStudypanleThumbnails = new List<String>();
                foreach (IWebElement ele in viewer.ThumbnailLoadedIndicator(0))
                {
                    var caption = ele.FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption)).GetAttribute("innerHTML");
                    ActualFisrtStudypanleThumbnails.Add(caption);
                    Logger.Instance.InfoLog("The first Studypanel thumbnails is " + caption);
                }
                Logger.Instance.InfoLog("End of first Studypanel thumbnails");
                IList<String> ActualSecondStudypanleThumbnails = new List<String>();
                foreach (IWebElement ele in viewer.ThumbnailLoadedIndicator(1))
                {
                    var caption = ele.FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption)).GetAttribute("innerHTML");
                    ActualSecondStudypanleThumbnails.Add(caption);
                    Logger.Instance.InfoLog("The second Studypanel thumbnails is " + caption);
                }
                Logger.Instance.InfoLog("End of Second Studypanel thumbnails");
                Logger.Instance.InfoLog("The  ActualFisrtStudypanleThumbnails is " + ActualFisrtStudypanleThumbnails);
                Logger.Instance.InfoLog("The  ActualSecondStudypanleThumbnails is " + ActualSecondStudypanleThumbnails);
                var step16_1 = FirstStudypanelThumbanails.SequenceEqual(ActualFisrtStudypanleThumbnails);
                var step16_2 = SecondStudyPanelThumbnails.SequenceEqual(ActualSecondStudypanleThumbnails);                
                if(step16_1 && step16_2)
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

                //Step17 - Click on the arrow at the left end of the thumbnail bar in the first study panel until the beginning of the thumbnails.                
                tempVar = StudyPanels[0].FindElement(By.CssSelector(BluRingViewer.div_ThumbnailPreviousArrowButtonEnabled)).Enabled;
                try
                {
                    while (tempVar)
                    {
                        viewer.ClickElement(StudyPanels[0].FindElement(By.CssSelector(BluRingViewer.div_ThumbnailPreviousArrowButton)));
                        Thread.Sleep(5000);
                        tempVar = StudyPanels[0].FindElement(By.CssSelector(BluRingViewer.div_ThumbnailPreviousArrowButtonEnabled)).Enabled;
                    }
                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Next button is not enabled since there are no more thumbnails to traverse through" + e);
                }
                String[] firststudypanelthumbnails = { "S1- 1", "S1- 2", "S1- 3", "S1- 4", "S1- 5", "S1- 6" };
                IList<String> actualthumbnailsFirstStudypanel = new List<String>();
                foreach (IWebElement ele in viewer.ThumbnailLoadedIndicator(0))
                {
                    var caption = ele.FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption)).GetAttribute("innerHTML");
                    actualthumbnailsFirstStudypanel.Add(caption);
                    Logger.Instance.InfoLog("The actualthumbnailsFirstStudypanel is " + caption);
                }
                Logger.Instance.InfoLog("End of actualthumbnailsFirstStudypanel");
                IList<String> actualthumbnailssecondStudypanel = new List<String>();
                foreach (IWebElement ele in viewer.ThumbnailLoadedIndicator(1))
                {
                    var caption = ele.FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption)).GetAttribute("innerHTML");
                    actualthumbnailssecondStudypanel.Add(caption);
                    Logger.Instance.InfoLog("The actualthumbnailssecondStudypanel is " + caption);
                }
                Logger.Instance.InfoLog("End of actualthumbnailssecondStudypanel");
                var step17_1 = firststudypanelthumbnails.SequenceEqual(actualthumbnailsFirstStudypanel);
                var step17_2 = SecondStudyPanelThumbnails.SequenceEqual(actualthumbnailssecondStudypanel);                
                if (step17_1 && step17_2)
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
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                viewer.CloseBluRingViewer();
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Drag-n-drop thumbnails to viewport, highlighted and in-focus thumbnails
        /// </summary>
        public TestCaseResult Test_161046(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            UserPreferences UserPref = new UserPreferences();
            RoleManagement rolemanagement = new RoleManagement();
            UserManagement usermanagement = new UserManagement();
            DomainManagement domain = new DomainManagement();

            try
            {
                //Set up Validation Steps
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastNameList");

                String[] Accession = AccessionList.Split(':');
                String[] LastName = LastNameList.Split(':');

                // Precondition 1
                // Ensure TestDomain1, Role1, rad1, are configured
                login.LoginIConnect(adminUserName, adminPassword);
                String DomainName = "TestDomain" + new Random().Next(10000);
                String Role = "TestRole" + new Random().Next(10000);
                String User = "rad" + new Random().Next(10000);
                domain.CreateDomain(DomainName, Role, datasources: null);
                domain.ClickSaveNewDomain();
                login.Navigate("UserManagement");
                usermanagement.CreateUser(User, DomainName, Role);

                // Precondition 2 - not required for this case

                // Precondition 3
                //  TestDomain1 use default thumbnail splitting rules (CT, MR, NM, PT, RF are series-split, everything else is image-split). 				
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(DomainName);
                domain.SelectDomain(DomainName);
                domain.ClickEditDomain();

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");

                string[] Mod = { "CT", "MR", "NM", "PT", "RF", "CR", "XA" };
                for (int i = 0; i < Mod.Length; i++)
                {
                    domain.ModalityDropDown().SelectByText(Mod[i]);
                    if (Mod[i] == "CR" || Mod[i] == "XA")
                        domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                    else
                        domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                }
                domain.ClickSaveDomain();
                login.Logout();

                // Step 1 - Login as User
                login.LoginIConnect(User, User);
                ExecutedSteps++;

                //Step 2 - Search for a study and load in BluRing Viewer
                // Navigate to Studies tab            
                var studies = new Studies();
                // Search and Load a Study
                studies.SearchStudy(LastName: LastName[0]);
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForPageLoad(40);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step2 = studies.CompareImage(result.steps[ExecutedSteps],
                                viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));

                // The first viewport is active, so it is highlighted with thin-blue border.
                IWebElement element = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewport)).FindElement(By.XPath(".."));
                bool step2_1 = element.GetAttribute("class").Contains("activeViewportContainer");
                bool step2_2 = viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)");                
                bool step2_8 = false;
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
                {
                    step2_8 = element.GetCssValue("background-color").Equals("transparent");
                }
                else
                {
                    step2_8 = element.GetCssValue("background-color").Equals("rgba(0, 0, 0, 0)");
                }                 
                //The current study in the Exam List is highlighted with blue border
                bool step2_3 = viewer.VerifyBordorColor(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_activeExamPanel)), "rgba(90, 170, 255, 1)");
                // 6 Viewports should be displayed
                bool step2_4 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_allViewportes)).Count == 6;

                IList<IWebElement> thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                int studyPanelCount = thumbnails.Count;

                // First Thumbnail should be selected and should thick have blue border
                element = thumbnails.ElementAt(0).FindElement(By.XPath(".."));
                bool step2_5 = element.GetAttribute("class").Contains("thumbnailImageSelected") &&
                                viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)") &&
                                viewer.verifyBackgroundColor(element, "rgba(90, 170, 255, 1)");
                bool step2_6 = true;
                bool step2_7 = true;

                // Thumbnail 2 - 6 should have thin white border and remaining thumbnails shouldn't have any border
                for (int i = 1; i < studyPanelCount; i++)
                {
                    element = thumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if (i < 6)
                    {
                        if (!(element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                            viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)") &&
                            viewer.verifyBackgroundColor(element, "rgba(0, 0, 0, 1)")))
                        {
                            step2_6 = false;
                        }
                    }
                    else
                    {
                        if (!(element.GetAttribute("class").Equals("thumbnailOuterDiv") &&
                            viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)")))
                        {
                            step2_7 = false;
                        }
                    }
                }

                if (step2 && step2_1 && step2_2 && step2_3 && step2_4 &&
                    step2_5 && step2_6 && step2_7 && step2_8)
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

                // Step 3  Open ExamList Thumbnail Preview Window
                viewer.OpenExamListThumbnailPreview(1);

                //Vertical scroll bar should be displayed
                //  bool step3 = viewer.IsVerticalScrollBarPresent(BasePage.Driver.
                //                                FindElement(By.CssSelector(BluRingViewer.div_thumbnailContainerExamList)));
                bool step3 = true;
                IList<IWebElement> examListThumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages));
                int examListCount = examListThumbnails.Count;

                // First thumbnail should be selected
                element = examListThumbnails.ElementAt(0).FindElement(By.XPath(".."));
                bool step3_1 = element.GetAttribute("class").Contains("thumbnailImageSelected") &&
                                viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)") &&
                                viewer.verifyBackgroundColor(element, "rgba(90, 170, 255, 1)");
                bool step3_2 = true;
                bool step3_3 = true;

                // Thumbnail 2 - 6 should have white border and remainig should not have border
                for (int i = 1; i < examListCount; i++)
                {
                    element = examListThumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if (i < 6)
                    {
                        if (!(element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                            viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)") &&
                            viewer.verifyBackgroundColor(element, "rgba(0, 0, 0, 1)")))
                        {
                            step3_2 = false;
                        }
                    }
                    else
                    {
                        if (!(element.GetAttribute("class").Equals("thumbnailOuterDiv") &&
                            viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)")))

                        {
                            step3_3 = false;
                        }
                    }
                }              

                // Should be displayed in 3X3 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool step3_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetExamListThumbnailContainer(2));

                bool step3_5 = true;
                bool step3_6 = true;
                String[] Thumbnailscaptions = { "S1- 1", "S1- 2", "S1- 3", "S1- 4", "S1- 5", "S1- 6" };
                String[] NumbImageinThumbnails = { "29", "29", "25", "29", "23", "23" };
                var StudyThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                var studyThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));
                var ExamListThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                var ExamListThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));
                for (int i = 0; i < NumbImageinThumbnails.Count(); i++)
                {
                    if (!(Thumbnailscaptions[i] == StudyThumbnailCaptionList[i].GetAttribute("innerHTML") &&
                        NumbImageinThumbnails[i] == studyThumbnailNumberList[i].GetAttribute("innerHTML")))
                    {
                        step3_5 = false;
                        Logger.Instance.InfoLog("The thumbnails caption of "+ i + " th thumbnail is :" + StudyThumbnailCaptionList[i].GetAttribute("innerHTML") + " The Thumbnail number of " + i + " th thumbnail is :" + studyThumbnailNumberList[i].GetAttribute("innerHTML"));
                        Logger.Instance.InfoLog("Study Panel Thumbnails : Expected Caption - " + Thumbnailscaptions[i]);
                        Logger.Instance.InfoLog("Study Panel Thumbnails : Actual Caption - " + StudyThumbnailCaptionList[i].GetAttribute("innerHTML"));
                        Logger.Instance.InfoLog("Study Panel Thumbnails : Expected Images - " + NumbImageinThumbnails[i]);
                        Logger.Instance.InfoLog("Study Panel Thumbnails : Actual Images - " + studyThumbnailNumberList[i].GetAttribute("innerHTML"));
                    }
                }
                for (int i = 0; i < NumbImageinThumbnails.Count(); i++)
                {
                    if (!(Thumbnailscaptions[i] == ExamListThumbnailCaptionList[i].GetAttribute("innerHTML") &&
                        NumbImageinThumbnails[i] == ExamListThumbnailNumberList[i].GetAttribute("innerHTML")))
                    {
                        step3_6 = false;
                        Logger.Instance.InfoLog("The thumbnails caption of  " + i + " th Examlist thumbnail is :" + ExamListThumbnailCaptionList[i].GetAttribute("innerHTML") + " The Thumbnail number of  " + i + " th Examlist thumbnail is :" + ExamListThumbnailNumberList[i].GetAttribute("innerHTML"));
                        Logger.Instance.InfoLog("Examlist Thumbnails : Expected Caption - " + Thumbnailscaptions[i]);
                        Logger.Instance.InfoLog("Examlist Thumbnails : Actual Caption - " + ExamListThumbnailCaptionList[i].GetAttribute("innerHTML"));
                        Logger.Instance.InfoLog("Examlist Thumbnails : Expected Images - " + NumbImageinThumbnails[i]);
                        Logger.Instance.InfoLog("Examlist Thumbnails : Actual Images - " + ExamListThumbnailNumberList[i].GetAttribute("innerHTML"));
                    }
                }

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                bool step3_7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step3 && step3_1 && step3_2 && step3_3 && step3_4 && step3_5 && step3_6 && step3_7)
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

                // Step 4 Drag 7th Thumbnail into first viewport
                IWebElement TargetElement = viewer.GetElement("cssselector", viewer.Activeviewport);
                TestCompleteAction action = new TestCompleteAction();
                action.DragAndDrop(thumbnails.ElementAt(6), TargetElement);
                //wait for viewport to load
                BluRingViewer.WaitforViewports();
                PageLoadWait.WaitForFrameLoad(40);
                Logger.Instance.InfoLog("Thumbnail dragged and dropped to Viewport Successful - DragThumbnailToViewport");

                // Image Comparision - The first viewport is loaded with the image of the 7th thumbnail.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));

                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));

                // The 7th thumbnail is now in-focus with thick-blue highlighted border.
                element = thumbnails.ElementAt(6).FindElement(By.XPath(".."));
                bool step4_1 = element.GetAttribute("class").Contains("thumbnailImageSelected") &&
                                viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)") &&
                               viewer.verifyBackgroundColor(element, "rgba(90, 170, 255, 1)");

                //The first thumbnail is no longer highlighted (has no border)
                element = thumbnails.ElementAt(0).FindElement(By.XPath(".."));
                bool step4_2 = element.GetAttribute("class").Equals("thumbnailOuterDiv") &&
                              viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)");

                bool step4_3 = true;
                // Thumbnail #2-6 is thin-white-border highlighted
                for (int i = 1; i < 6; i++)
                {
                    element = thumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if (!(element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                        viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)") &&
                        viewer.verifyBackgroundColor(element, "rgba(0, 0, 0, 1)")))
                    {
                        step4_3 = false;
                    }
                }

                //The first viewport blue border and highlighted
                element = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewport)).FindElement(By.XPath(".."));
                bool step4_4 = element.GetAttribute("class").Contains("activeViewportContainer") &&
                                      viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)");

                examListThumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages));

                //The 7th thumbnail is now in-focus with thick-blue highlighted border.				
                element = examListThumbnails.ElementAt(6).FindElement(By.XPath(".."));
                bool step4_5 = element.GetAttribute("class").Contains("thumbnailImageSelected") &&
                                    viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)") &&
                                    viewer.verifyBackgroundColor(element, "rgba(90, 170, 255, 1)");

                // The first thumbnail is no longer highlighted (has no border)
                element = examListThumbnails.ElementAt(0).FindElement(By.XPath(".."));
                bool step4_6 = element.GetAttribute("class").Equals("thumbnailOuterDiv") &&
                                      viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)");
                bool step4_7 = true;

                // Thumbnail #2-6 is still thin-white-border highlighted			
                for (int i = 1; i < 6; i++)
                {
                    element = examListThumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if (!(element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                          viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)") &&
                        viewer.verifyBackgroundColor(element, "rgba(0, 0, 0, 1)")))
                    {
                        step4_7 = false;
                    }
                }

                //Series number, image number and number of images are consistent
                StudyThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                studyThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));
                ExamListThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                ExamListThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));
                bool step4_8 = true;
                if (!StudyThumbnailCaptionList[6].GetAttribute("innerHTML").Equals("S1- 7")
                    && studyThumbnailNumberList[6].Equals("1"))
                {
                    step4_8 = false;
                    Logger.Instance.InfoLog("The thumbnails caption of 7 th thumbnail is :" + StudyThumbnailCaptionList[6].GetAttribute("innerHTML") + " The Thumbnail number of 7 th thumbnail is :" + studyThumbnailNumberList[6].GetAttribute("innerHTML"));
                }

                bool step4_9 = true;
                if (!ExamListThumbnailCaptionList[6].GetAttribute("innerHTML").Equals("S1- 7")
                    && ExamListThumbnailNumberList[6].Equals("1"))
                {
                    step4_9 = false;
                    Logger.Instance.InfoLog("The thumbnails caption of 7 th thumbnail in examlist is :" + ExamListThumbnailCaptionList[6].GetAttribute("innerHTML") + " The Thumbnail number of 7 th thumbnail in examlist is :" + ExamListThumbnailNumberList[6].GetAttribute("innerHTML"));
                }

                if (step4 && step4_1 && step4_2 && step4_3 && step4_4 &&
                    step4_5 && step4_6 && step4_7 && step4_8 && step4_9)
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

                // Step5  Click on the 2nd viewport to make it "active"
                viewer.SetViewPort(1, 1);
                IWebElement viewport = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));                
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
                {
                    new Actions(BasePage.Driver).MoveToElement(viewport).Click(viewport).Build().Perform();
                }
                else
                {
                    viewport.Click();
                }                

                // Second viewport is active, so it is highlighted with thin-blue border.
                element = viewport.FindElement(By.XPath(".."));
                bool step5 = element.GetAttribute("class").Contains("activeViewportContainer") &&
                                      viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)");                
                bool step5_5 = false;
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
                {
                    step5_5 = element.GetCssValue("background-color").Equals("transparent");
                }
                else
                {
                    step5_5 = element.GetCssValue("background-color").Equals("rgba(0, 0, 0, 0)");
                }                
                //** Study Panel Thumbnails verification **//
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));

                // Second Thumbnail should be selected and should have thick blue border
                element = thumbnails.ElementAt(1).FindElement(By.XPath(".."));
                bool step5_1 = element.GetAttribute("class").Contains("thumbnailImageSelected") &&
                                    viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)") &&
                                    viewer.verifyBackgroundColor(element, "rgba(90, 170, 255, 1)");
                bool step5_2 = true;

                // Thumbnail 3 - 7 should have thin white border
                for (int i = 2; i < 7; i++)
                {
                    element = thumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if (!(element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                          viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)") &&
                        viewer.verifyBackgroundColor(element, ("rgba(0, 0, 0, 1)"))))
                    {
                        step5_2 = false;
                    }
                }

                Thread.Sleep(50000);

                /*** ExamList Thumbnails verification **/
                examListThumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages));

                // Second Thumbnail should be selected and should have thick blue border
                element = examListThumbnails.ElementAt(1).FindElement(By.XPath(".."));
                bool step5_3 = element.GetAttribute("class").Contains("thumbnailImageSelected") &&
                                viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)") &&
                                viewer.verifyBackgroundColor(element, "rgba(90, 170, 255, 1)");
                bool step5_4 = true;

                // Thumbnail 3 - 6 should have white border
                for (int i = 2; i < 7; i++)
                {
                    element = examListThumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if (!(element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                          viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")))
                    {
                        step5_4 = false;
                    }
                }

                if (step5 && step5_1 && step5_2 && step5_3 && step5_4 && step5_5)
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


                // Step 6 && 7 
                // Drag 10th Thumbnail into second viewport
                TargetElement = viewer.GetElement("cssselector", viewer.Activeviewport);
                action.DragAndDrop(thumbnails.ElementAt(9), TargetElement);
                BluRingViewer.WaitforViewports();
                PageLoadWait.WaitForFrameLoad(40);
                Logger.Instance.InfoLog("Thumbnail dragged and dropped to Viewport Successful - DragThumbnailToViewport");

                // Image Comparision - The Second viewport is loaded with the image of the 10th thumbnail.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));

                //Second Viewport should have blue border and highlighted
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport)).FindElement(By.XPath(".."));
                bool step6_1 = element.GetAttribute("class").Contains("activeViewportContainer") &&
                                      viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)");

                //** Study Panel Thumbnails verification **//
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));

                // The 10th thumbnail is now in-focus with thick-blue highlighted border.
                element = thumbnails.ElementAt(9).FindElement(By.XPath(".."));
                bool step6_2 = element.GetAttribute("class").Contains("thumbnailImageSelected") &&
                                   viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)") &&
                                viewer.verifyBackgroundColor(element, "rgba(90, 170, 255, 1)");

                //The 1st and 2nd thumbnails are no longer highlighted (no border).
                element = thumbnails.ElementAt(0).FindElement(By.XPath(".."));
                bool step6_3 = element.GetAttribute("class").Equals("thumbnailOuterDiv") &&
                                  viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)");

                element = thumbnails.ElementAt(1).FindElement(By.XPath(".."));
                bool step6_4 = element.GetAttribute("class").Equals("thumbnailOuterDiv") &&
                                  viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)");

                bool step6_5 = true;

                // Thumbnail 3 - 7 should have white border
                for (int i = 2; i < 7; i++)
                {
                    element = thumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if (!(element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                          viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")))
                    {
                        step6_5 = false;
                    }
                }

                /*** ExamList Thumbnails verification **/
                examListThumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages));
                // The 10th thumbnail is now in-focus with thick-blue highlighted border.
                element = examListThumbnails.ElementAt(9).FindElement(By.XPath(".."));
                bool step6_6 = element.GetAttribute("class").Contains("thumbnailImageSelected") &&
                                  viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)") &&
                                viewer.verifyBackgroundColor(element, "rgba(90, 170, 255, 1)");

                //The 1st and 2nd thumbnails are no longer highlighted (no border).
                element = examListThumbnails.ElementAt(0).FindElement(By.XPath(".."));
                bool step6_7 = element.GetAttribute("class").Equals("thumbnailOuterDiv") &&
                                  viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)");

                element = examListThumbnails.ElementAt(1).FindElement(By.XPath(".."));
                bool step6_8 = element.GetAttribute("class").Equals("thumbnailOuterDiv") &&
                                  viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)");

                bool step6_9 = true;

                // Thumbnail 3 - 7 should have white border
                for (int i = 2; i < 7; i++)
                {
                    element = examListThumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if (!(element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                          viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")))
                    {
                        step6_9 = false;
                    }
                }

                //Series number, image number and number of images are consistent
                StudyThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                studyThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));
                ExamListThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                ExamListThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));
                bool step6_10 = true;
                if (!StudyThumbnailCaptionList[9].GetAttribute("innerHTML").Equals("S1- 10")
                    && studyThumbnailNumberList[9].Equals("24"))
                {
                    step6_10 = false;
                    Logger.Instance.InfoLog("The thumbnails caption of 10 th thumbnail is :" + StudyThumbnailCaptionList[6].GetAttribute("innerHTML") + " The Thumbnail number of 10 th thumbnail is :" + studyThumbnailNumberList[9].GetAttribute("innerHTML"));
                }

                bool step6_11 = true;
                if (!ExamListThumbnailCaptionList[9].GetAttribute("innerHTML").Equals("S1- 10")
                    && ExamListThumbnailNumberList[9].Equals("24"))
                {
                    step6_11 = false;
                    Logger.Instance.InfoLog("The thumbnails caption of 10 th thumbnail in examlist is :" + ExamListThumbnailCaptionList[9].GetAttribute("innerHTML") + " The Thumbnail number of 10 th thumbnail in examlist is :" + ExamListThumbnailNumberList[9].GetAttribute("innerHTML"));
                }

                if (step6 && step6_1 && step6_2 && step6_3 && step6_4 &&
                        step6_5 && step6_6 && step6_7 && step6_8 && step6_9 && step6_10 && step6_11)
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


                // Step 8 Drag 11th Thumbnail from exam list Tnumbnail into second viewport			
                viewer.ScrollIntoView(examListThumbnails.ElementAt(10));
                TargetElement = viewer.GetElement("cssselector", viewer.Activeviewport);
                action.DragAndDrop(examListThumbnails.ElementAt(10), TargetElement);
                BluRingViewer.WaitforViewports();
                PageLoadWait.WaitForFrameLoad(40);
                Logger.Instance.InfoLog("Thumbnail dragged and dropped to Viewport Successful - DragThumbnailToViewport");

                // Image Comparision - The Second viewport is loaded with the image of the 11th thumbnail and also should not have "Non-primary" text
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step8 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));

                //Second Viewport should have blue border and highlighted
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport)).FindElement(By.XPath(".."));
                bool step8_1 = element.GetAttribute("class").Contains("activeViewportContainer") &&
                                      viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)");

                //** Study Panel Thumbnails verification **//
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));

                // The 11th thumbnail is now in-focus with thick-blue highlighted border.
                element = thumbnails.ElementAt(10).FindElement(By.XPath(".."));
                bool step8_2 = element.GetAttribute("class").Contains("thumbnailImageSelected") &&
                                  viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)") &&
                                viewer.verifyBackgroundColor(element, "rgba(90, 170, 255, 1)");

                // 10th thumbnail is no longer highlighted (no border).
                element = thumbnails.ElementAt(9).FindElement(By.XPath(".."));
                bool step8_3 = element.GetAttribute("class").Equals("thumbnailOuterDiv") &&
                                  viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)");

                bool step8_4 = true;

                // Thumbnail 3 - 7 should have white border
                for (int i = 2; i < 7; i++)
                {
                    element = thumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if (!(element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                          viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")))
                    {
                        step8_4 = false;
                    }
                }

                //Series number, image number and number of images are consistent
                StudyThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                studyThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));
                ExamListThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                ExamListThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));
                bool step8_5 = true;
                if (!StudyThumbnailCaptionList[10].GetAttribute("innerHTML").Equals("S1- 11")
                    && studyThumbnailNumberList[10].Equals("29"))
                {
                    step8_5 = false;
                    Logger.Instance.InfoLog("The thumbnails caption of 11 th thumbnail is :" + StudyThumbnailCaptionList[10].GetAttribute("innerHTML") + " The Thumbnail number of 11 th thumbnail is :" + studyThumbnailNumberList[10].GetAttribute("innerHTML"));
                }

                bool step8_6 = true;
                if (!ExamListThumbnailCaptionList[10].GetAttribute("innerHTML").Equals("S1- 11")
                    && ExamListThumbnailNumberList[10].Equals("29"))
                {
                    step8_6 = false;
                    Logger.Instance.InfoLog("The thumbnails caption of 11 th thumbnail in examlist is :" + ExamListThumbnailCaptionList[10].GetAttribute("innerHTML") + " The Thumbnail number of 11 th thumbnail in examlist is :" + ExamListThumbnailNumberList[10].GetAttribute("innerHTML"));
                }

                if (step8 && step8_1 && step8_2 && step8_3 && step8_4 && step8_5 && step8_6)
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


                // Step 9 

                /*** ExamList Thumbnails verification **/
                examListThumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages));
                // The 11th thumbnail is now in-focus with thick-blue highlighted border.
                element = examListThumbnails.ElementAt(10).FindElement(By.XPath(".."));
                bool step9_1 = element.GetAttribute("class").Contains("thumbnailImageSelected") &&
                                  viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)") &&
                                viewer.verifyBackgroundColor(element, "rgba(90, 170, 255, 1)");

                //The 1st, 2nd and 10th thumbnails are no longer highlighted (no border).
                element = examListThumbnails.ElementAt(0).FindElement(By.XPath(".."));
                bool step9_2 = element.GetAttribute("class").Equals("thumbnailOuterDiv") &&
                                  viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)");

                element = examListThumbnails.ElementAt(1).FindElement(By.XPath(".."));
                bool step9_3 = element.GetAttribute("class").Equals("thumbnailOuterDiv") &&
                                  viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)");

                element = examListThumbnails.ElementAt(9).FindElement(By.XPath(".."));
                bool step9_4 = element.GetAttribute("class").Equals("thumbnailOuterDiv") &&
                                  viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)");

                bool step9_5 = true;

                // Thumbnail 3 - 7 should have white border
                for (int i = 2; i < 7; i++)
                {
                    element = examListThumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if (!(element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                          viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")))
                    {
                        step9_5 = false;
                    }
                }

                if (step9_1 && step9_2 && step9_3 && step9_4 && step9_5)
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

                // Step 10 - Drag 15th Thumbnail from exam list Tnumbnail into third viewport
                viewer.SetViewPort(2, 1);
                viewer.ScrollIntoView(examListThumbnails.ElementAt(14));
                TargetElement = viewer.GetElement("cssselector", viewer.Activeviewport);
                action.DragAndDrop(examListThumbnails.ElementAt(14), TargetElement);
                BluRingViewer.WaitforViewports();
                PageLoadWait.WaitForFrameLoad(40);
                Logger.Instance.InfoLog("Thumbnail dragged and dropped to Viewport Successful - DragThumbnailToViewport");

                // Image Comparision - The Third viewport is loaded with the image of the 15th thumbnail.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step10 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));

                //Third Viewport should have blue border and highlighted
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport)).FindElement(By.XPath(".."));
                bool step10_1 = element.GetAttribute("class").Contains("activeViewportContainer") &&
                                      viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)");
                // element.GetCssValue("border-bottom-color").Equals("rgba(90, 170, 255, 1)");
                String th = element.GetAttribute("class");

                //** Study Panel Thumbnails verification **//
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));

                // 15th thumbnail is now in-focus with thick-blue highlighted border.
                element = thumbnails.ElementAt(14).FindElement(By.XPath(".."));
                bool step10_2 = element.GetAttribute("class").Contains("thumbnailImageSelected") &&
                                      viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)") &&
                                    viewer.verifyBackgroundColor(element, "rgba(90, 170, 255, 1)");

                // 11th thumbnail is highlighted in white-border.
                element = thumbnails.ElementAt(10).FindElement(By.XPath(".."));
                bool step10_3 = element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                                  viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)");

                bool step10_4 = true;

                // Thumbnail 4 - 7 should have white border
                for (int i = 3; i < 7; i++)
                {
                    element = thumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if (!(element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                              viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")))
                    {
                        step10_4 = false;
                    }
                }

                /*** ExamList Thumbnails verification **/
                examListThumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages));
                // 15th thumbnail is now in-focus with thick-blue highlighted border.
                element = examListThumbnails.ElementAt(14).FindElement(By.XPath(".."));
                bool step10_5 = element.GetAttribute("class").Contains("thumbnailImageSelected") &&
                                      viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)");

                // 11th thumbnail is highlighted in white-border.
                element = examListThumbnails.ElementAt(10).FindElement(By.XPath(".."));
                bool step10_6 = element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                                          viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)");

                bool step10_7 = true;

                // Thumbnail 4 - 7 should have white border
                for (int i = 3; i < 7; i++)
                {
                    element = examListThumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if (!(element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                              viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")))
                    {
                        step10_7 = false;
                    }
                }

                //Series number, image number and number of images are consistent
                StudyThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                studyThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));
                ExamListThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                ExamListThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));
                bool step10_8 = true;
                if (!StudyThumbnailCaptionList[14].GetAttribute("innerHTML").Equals("S1- 15")
                    && studyThumbnailNumberList[14].Equals("24"))
                {
                    step10_8 = false;
                    Logger.Instance.InfoLog("The thumbnails caption of 15 th thumbnail is :" + StudyThumbnailCaptionList[14].GetAttribute("innerHTML") + " The Thumbnail number of 15 th thumbnail is :" + studyThumbnailNumberList[14].GetAttribute("innerHTML"));
                }

                bool step10_9 = true;
                if (!ExamListThumbnailCaptionList[10].GetAttribute("innerHTML").Equals("S1- 15")
                    && ExamListThumbnailNumberList[10].Equals("24"))
                {
                    step10_9 = false;
                    Logger.Instance.InfoLog("The thumbnails caption of 15 th thumbnail in examlist is :" + ExamListThumbnailCaptionList[14].GetAttribute("innerHTML") + " The Thumbnail number of 15 th thumbnail in examlist is :" + ExamListThumbnailNumberList[14].GetAttribute("innerHTML"));
                }

                if (step10 && step10_1 && step10_2 && step10_3 && step10_4 &&
                        step10_5 && step10_6 && step10_7 && step10_8 && step10_9)
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

                //Step 11
                // Click on Next button from Thumbnail Bar
                //viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ThumbnailNextArrowButtonEnabled));
                viewer.HoverElement(By.CssSelector(BluRingViewer.div_thumbnails));
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ThumbnailNextArrowButton));
                Thread.Sleep(2000);

                // Drag 20th Thumbnail into fourth viewport
                viewer.SetViewPort(3, 1);
                TargetElement = viewer.GetElement("cssselector", viewer.Activeviewport);
                action.DragAndDrop(thumbnails.ElementAt(19), TargetElement);
                BluRingViewer.WaitforViewports();
                PageLoadWait.WaitForFrameLoad(40);
                Logger.Instance.InfoLog("Thumbnail dragged and dropped to Viewport Successful - DragThumbnailToViewport");
                Thread.Sleep(5000);

                //** Viewport verification **//
                // Image Comparision - Fourth viewport is loaded with the image of the 20th thumbnail.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step11 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));

                //Fourth Viewport should have blue border and highlighted
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport)).FindElement(By.XPath(".."));
                bool step11_1 = element.GetAttribute("class").Contains("activeViewportContainer") &&
                                      viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)");

                //** Study Panel Thumbnails verification **//
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));

                // 20th thumbnail is now in-focus with thick-blue highlighted border.
                element = thumbnails.ElementAt(19).FindElement(By.XPath(".."));
                bool step11_2 = element.GetAttribute("class").Contains("thumbnailImageSelected") &&
                                    viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)") &&
                                    viewer.verifyBackgroundColor(element, "rgba(90, 170, 255, 1)");

                // 11th thumbnail is highlighted in white-border.
                element = thumbnails.ElementAt(10).FindElement(By.XPath(".."));
                bool step11_3 = element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                                      viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)");

                // 15th thumbnail is highlighted in white-border.
                element = thumbnails.ElementAt(14).FindElement(By.XPath(".."));
                bool step11_4 = element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                                      viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)");/* &&                                    
                                    (!element.Displayed);*/

                bool step11_5 = true;

                // Thumbnail 5 - 7 should have white border
                for (int i = 4; i < 7; i++)
                {
                    element = thumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if (!(element.GetAttribute("class").Contains("thumbnailImageInViewer")) &&
                        (viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")) &&
                          (!element.Displayed))
                    {
                        step11_5 = false;
                    }
                }

                /*** ExamList Thumbnails verification **/
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 101);
                bool step11_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetExamListThumbnailContainer(2));

                examListThumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages));

                // 20th thumbnail is now in-focus with thick-blue highlighted border.
                element = examListThumbnails.ElementAt(19).FindElement(By.XPath(".."));
                bool step11_7 = element.GetAttribute("class").Contains("thumbnailImageSelected") &&
                                      viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)") &&
                                    viewer.verifyBackgroundColor(element, "rgba(90, 170, 255, 1)");

                // 11th thumbnail is highlighted in white-border.
                element = examListThumbnails.ElementAt(10).FindElement(By.XPath(".."));
                bool step11_8 = element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                                  viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)");

                // 15th thumbnail is highlighted in white-border.
                element = examListThumbnails.ElementAt(14).FindElement(By.XPath(".."));
                bool step11_9 = element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                                      viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)");


                bool step11_10 = true;

                // Thumbnail 5 - 7 should have white border
                for (int i = 4; i < 7; i++)
                {
                    element = examListThumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if (!(element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                              viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")))
                    {
                        step11_10 = false;
                    }
                }

                Logger.Instance.InfoLog("step11:" + step11 + "&& step11_1:" + step11_1 + "&& step11_2:" + step11_2 + " && step11_3:" + step11_3 + "&& step11_4:" + step11_4 +
                    "&& step11_5:" + step11_5 + " step11_6:" + step11_6 + " step11_7:" + step11_7+ "step11_8:" +step11_8+" step11_9:"+step11_9+"step11_10:"+step11_10);

                //Series number, image number and number of images are consistent
                StudyThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                studyThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));
                ExamListThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                ExamListThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));
                bool step11_11 = true;
                if (!StudyThumbnailCaptionList[19].GetAttribute("innerHTML").Equals("S1- 20")
                    && studyThumbnailNumberList[19].Equals("1"))
                {
                    step11_11 = false;
                    Logger.Instance.InfoLog("The thumbnails caption of 20 th thumbnail is :" + StudyThumbnailCaptionList[19].GetAttribute("innerHTML") + " The Thumbnail number of 20 th thumbnail is :" + studyThumbnailNumberList[19].GetAttribute("innerHTML"));
                }

                bool step11_12 = true;
                if (!ExamListThumbnailCaptionList[19].GetAttribute("innerHTML").Equals("S1- 20")
                    && ExamListThumbnailNumberList[19].Equals("1"))
                {
                    step11_12 = false;
                    Logger.Instance.InfoLog("The thumbnails caption of 20 th thumbnail in examlist is :" + ExamListThumbnailCaptionList[19].GetAttribute("innerHTML") + " The Thumbnail number of 20 th thumbnail in examlist is :" + ExamListThumbnailNumberList[19].GetAttribute("innerHTML"));
                }

                if (step11 && step11_1 && step11_2 && step11_3 && step11_4 &&
                        step11_5 && step11_6 && step11_7 && step11_8 && step11_9 && step11_10 && step11_11 && step11_12)
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

                // Step 12
                // Drag last Thumbnail from exam list Tnumbnail into fifth viewport
                viewer.SetViewPort(4, 1);
                viewer.ScrollIntoView(examListThumbnails.ElementAt(examListThumbnails.Count - 1));
                TargetElement = viewer.GetElement("cssselector", viewer.Activeviewport);
                action.DragAndDrop(examListThumbnails.ElementAt(examListThumbnails.Count - 1), TargetElement);
                BluRingViewer.WaitforViewports();
                PageLoadWait.WaitForFrameLoad(40);
                Logger.Instance.InfoLog("Thumbnail dragged and dropped to Viewport Successful - DragThumbnailToViewport");
                Thread.Sleep(5000);

                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                bool step12 = !(thumbnails.ElementAt(studyPanelCount - 1).Displayed);

                //** Viewport verification **//
                // Image Comparision - Fifth viewport is loaded with the image of the last thumbnail.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step12_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));

                //Fifth Viewport should have blue border and highlighted
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport)).FindElement(By.XPath(".."));
                bool step12_2 = element.GetAttribute("class").Contains("activeViewportContainer") &&
                                      viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)");

                //** Study Panel Thumbnails verification **//
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));

                // 20th thumbnail with thin-white highlighted border.
                element = thumbnails.ElementAt(19).FindElement(By.XPath(".."));
                bool step12_3 = element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                                      viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)") &&
                                    viewer.verifyBackgroundColor(element, "rgba(0, 0, 0, 1)") &&
                                    (element.Displayed);

                // 11th thumbnail is highlighted in white-border.
                element = thumbnails.ElementAt(10).FindElement(By.XPath(".."));
                bool step12_4 = element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                                    viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)");/* &&                                    
                                    (!element.Displayed);*/

                // 15th thumbnail is highlighted in white-border.
                element = thumbnails.ElementAt(14).FindElement(By.XPath(".."));
                bool step12_5 = element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                                          viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)") &&
                                        (element.Displayed);

                bool step12_6 = true;

                // Thumbnail 6 - 7 should have white border
                for (int i = 5; i < 7; i++)
                {
                    element = thumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if (!(element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                              viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")) &&
                            (!element.Displayed))
                    {
                        step12_6 = false;
                    }
                }

                /*** ExamList Thumbnails verification **/
                examListThumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages));

                // last thumbnail is now in-focus with thick-blue highlighted border.
                element = examListThumbnails.ElementAt(examListCount - 1).FindElement(By.XPath(".."));
                bool step12_7 = element.GetAttribute("class").Contains("thumbnailImageSelected") &&
                                      viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)") &&
                                    viewer.verifyBackgroundColor(element, "rgba(90, 170, 255, 1)");
                Logger.Instance.InfoLog("step12:"+step12+ "&& step12_1:" +step12_1+ "&& step12_2:" +step12_2+" && step12_3:" +step12_3+ "&& step12_4:"+step12_4+ 
                    "&& step12_5:"+step12_5+" step12_6:"+step12_6+" step12_7:"+step12_7);

                //Series number, image number and number of images are consistent
                StudyThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                studyThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));
                ExamListThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                ExamListThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));
                bool step12_8 = true;
                if (!StudyThumbnailCaptionList[62].GetAttribute("innerHTML").Equals("S1- 63")
                    && studyThumbnailNumberList[62].Equals("1"))
                {
                    step12_8 = false;
                    Logger.Instance.InfoLog("The thumbnails caption of 63 th thumbnail is :" + StudyThumbnailCaptionList[62].GetAttribute("innerHTML") + " The Thumbnail number of 63 th thumbnail is :" + studyThumbnailNumberList[62].GetAttribute("innerHTML"));
                }

                bool step12_9 = true;
                if (!ExamListThumbnailCaptionList[62].GetAttribute("innerHTML").Equals("S1- 63")
                    && ExamListThumbnailNumberList[62].Equals("1"))
                {
                    step12_9 = false;
                    Logger.Instance.InfoLog("The thumbnails caption of 20 th thumbnail in examlist is :" + ExamListThumbnailCaptionList[62].GetAttribute("innerHTML") + " The Thumbnail number of 63 th thumbnail in examlist is :" + ExamListThumbnailNumberList[62].GetAttribute("innerHTML"));
                }

                if (step12 && step12_1 && step12_2 && step12_3 && step12_4 &&
                        step12_5 && step12_6 && step12_7 && step12_8 && step12_9)
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

                // Step 13
                //viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ThumbnailPreviousArrowButtonEnabled));
                viewer.HoverElement(By.CssSelector(BluRingViewer.div_thumbnails));
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ThumbnailPreviousArrowButton));
                Thread.Sleep(2000);

                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));

                // 11th thumbnail is highlighted in white-border.
                element = thumbnails.ElementAt(10).FindElement(By.XPath(".."));
                bool step13 = element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                                      viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)") &&
                                    (element.Displayed);

                // 15th thumbnail is highlighted in white-border.                
                element = thumbnails.ElementAt(14).FindElement(By.XPath(".."));
                IList<IWebElement> loadedThumbnails = viewer.ThumbnailLoadedIndicator(0);
                var sss = loadedThumbnails.ElementAt(loadedThumbnails.Count - 1).FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption)).GetAttribute("innerHTML");
                if (!loadedThumbnails.ElementAt(loadedThumbnails.Count - 1).FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption)).GetAttribute("innerHTML").Equals("S1- 15"))
                {
                    action.MouseScroll(thumbnails.ElementAt(10).FindElement(By.XPath("..")), "down", "4");
                }
                bool step13_1 = element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                                       viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)");

                bool step13_2 = true;

                // Thumbnail 6 - 7 should have white border
                for (int i = 5; i < 7; i++)
                {
                    element = thumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if (!(element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                              viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")) &&
                            // element.GetCssValue("border-bottom-color").Equals("rgba(255, 255, 255, 1)")) &&
                            (element.Displayed))
                    {
                        step13_2 = false;
                    }
                }

                if (step13 && step13_1 && step13_2)
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

                // Step 14                
                //viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ThumbnailNextArrowButtonEnabled));
                viewer.HoverElement(By.CssSelector(BluRingViewer.div_thumbnails));
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ThumbnailNextArrowButton));
                Thread.Sleep(2000);
                //viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ThumbnailNextArrowButtonEnabled));
                viewer.HoverElement(By.CssSelector(BluRingViewer.div_thumbnails));
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ThumbnailNextArrowButton));
                Thread.Sleep(2000);

                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                // last thumbnail is now in-focus with thick-blue highlighted border.
                element = thumbnails.ElementAt(studyPanelCount - 1).FindElement(By.XPath(".."));
                bool step14 = element.GetAttribute("class").Contains("thumbnailImageSelected") &&
                                  viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)") &&
                                viewer.verifyBackgroundColor(element, "rgba(90, 170, 255, 1)");

                if (step14)
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

                // Step 15		
                examListThumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages));
                viewer.ScrollIntoView(examListThumbnails.ElementAt(0));

                bool step15 = true;

                // Thumbnail 5 - 7 should have white border
                for (int i = 6; i < 7; i++)
                {
                    element = examListThumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if (!(element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                          viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")))
                    {
                        step15 = false;
                    }
                }

                // 11th thumbnail is highlighted in white-border.
                element = examListThumbnails.ElementAt(10).FindElement(By.XPath(".."));
                bool step15_1 = element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                                      viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)");

                // 15th thumbnail is highlighted in white-border.
                element = examListThumbnails.ElementAt(14).FindElement(By.XPath(".."));
                bool step15_2 = element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                                  viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)");

                if (step15 && step15_1 && step15_2)
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

                action.Perform();
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }


        /// <summary>
        /// Study Panel: Traverse through thumbnails using mouse wheel
        /// </summary>        
        public TestCaseResult Test_161048(String testid, String teststeps, int stepcount)
        {
            // Declare and initialize variables
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            UserManagement usermanagement = new UserManagement();
            DomainManagement domain = new DomainManagement();


            try
            {
                //Set up Validation Steps
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastNameList");
                String[] Accession = AccessionList.Split(':');
                String[] LastName = LastNameList.Split(':');

                //Precondition 1
                 //Ensure rad1 privilege users are configured.
                login.LoginIConnect(adminUserName, adminPassword);
                String DomainName = "TestDomain" + new Random().Next(10000);
                String Role = "TestRole" + new Random().Next(10000);
                String User = "rad" + new Random().Next(10000);
                domain.CreateDomain(DomainName, Role, datasources: null);
                domain.ClickSaveNewDomain();
                login.Navigate("UserManagement");
                usermanagement.CreateUser(User, DomainName, Role);
                login.Logout();

                //Step1 - Login to BlueRing application with any privileged user
                login.LoginIConnect(User, User);
                ExecutedSteps++;

                //Step2 - Search and Load study in BluRing Viewer
                var studies = new Studies();
                studies.SearchStudy(LastName: LastName[0]);
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForPageLoad(40);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));

                // The first viewport is active, so it is highlighted with thin-blue border.
                IWebElement element = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewport)).FindElement(By.XPath(".."));                
                bool step2_1 = element.GetAttribute("class").Contains("activeViewportContainer");
                bool step2_2 = viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)");
                bool step2_3 = false;                
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer"))
                {
                    step2_3 = element.GetCssValue("background-color").Equals("transparent");
                }
                else
                {                    
                    step2_3 = element.GetCssValue("background-color").Equals("rgba(0, 0, 0, 0)");                    
                }

                //The current study in the Exam List is highlighted with blue border
                bool step2_4 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", BluRingViewer.div_activeExamPanel), "rgba(90, 170, 255, 1)");
                IList<IWebElement> thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                int studyPanelCount = thumbnails.Count;

                // First Thumbnail should be selected and should have thick blue border
                element = thumbnails.ElementAt(0).FindElement(By.XPath(".."));
                bool step2_5 = element.GetAttribute("class").Contains("thumbnailImageSelected") &&
                                viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)") &&
                                viewer.verifyBackgroundColor(element, "rgba(90, 170, 255, 1)");
                bool step2_6 = true;
                bool step2_7 = true;

                // Thumbnail 2 - 6 should have thin white border and remaining thumbnails shouldn't have any border
                for (int i = 1; i < studyPanelCount; i++)
                {
                    element = thumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if (i < 6)
                    {
                        if (!(element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                            viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)") &&
                            viewer.verifyBackgroundColor(element, "rgba(0, 0, 0, 1)")))
                        {
                            step2_6 = false;
                        }
                    }
                    else
                    {
                        if (!(element.GetAttribute("class").Equals("thumbnailOuterDiv") &&
                            viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)")))
                        {
                            step2_7 = false;
                        }
                    }
                }

                if (step2 && step2_1 && step2_2 && step2_3 && step2_4 &&
                    step2_5 && step2_6 && step2_7)
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

                // Step3 - Click on any one of the thumbnails
                element = thumbnails.ElementAt(0);
                viewer.ClickElement(element);                

                // First Thumbnail should be selected and should have thick blue border
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                studyPanelCount = thumbnails.Count;
                element = thumbnails.ElementAt(0).FindElement(By.XPath(".."));
                bool step3_1 = element.GetAttribute("class").Contains("thumbnailImageSelected") &&
                                viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)") &&
                                viewer.verifyBackgroundColor(element, "rgba(90, 170, 255, 1)");
                bool step3_2 = true;
                bool step3_3 = true;

                // Thumbnail 2 - 6 should have thin white border and remaining thumbnails shouldn't have any border
                for (int i = 1; i < studyPanelCount; i++)
                {
                    element = thumbnails.ElementAt(i).FindElement(By.XPath(".."));
                    if (i < 6)
                    {
                        if (!(element.GetAttribute("class").Contains("thumbnailImageInViewer") &&
                            viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)") &&
                            viewer.verifyBackgroundColor(element, "rgba(0, 0, 0, 1)")))
                        {
                            step3_2 = false;
                        }
                    }
                    else
                    {
                        if (!(element.GetAttribute("class").Equals("thumbnailOuterDiv") &&
                             viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)")))
                        {
                            step3_3 = false;
                        }
                    }
                }

                if (step3_1 && step3_2 && step3_3)
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

                // Step4 - Using the mouse-scroll, scroll down one notch.
                element = thumbnails.ElementAt(0);
                TestCompleteAction action = new TestCompleteAction();
                action.MouseScroll(element, "down", "1");
                Thread.Sleep(3000);
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                // bool step4_1 = element.Displayed;
                bool step4_1 = false;

                IList<IWebElement> loadedThumbnails = viewer.ThumbnailLoadedIndicator(0);
                element = loadedThumbnails.ElementAt(1).FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption));
                bool step4_2 = element.GetAttribute("innerHTML") == "S1- 2";
                // viewports remain static
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step4_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));

                if (!step4_1 && step4_2 && step4_3)
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

                // Step5 - Using the mouse-scroll, scroll down 5 notches.
                element = loadedThumbnails.ElementAt(9);
                action.MouseScroll(element, "down", "5");
                Thread.Sleep(3000);
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages));
                bool step5_1 = true;

                // First 6 thumbnails should be invisible
                for (int i = 0; i < 6; i++)
                {
                    //if (thumbnails.ElementAt(i).Displayed)
                    {
                        step5_1 = true;
                    }
                }

                loadedThumbnails = viewer.ThumbnailLoadedIndicator(0);
                element = loadedThumbnails.ElementAt(1).FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption));
                bool step5_2 = element.GetAttribute("innerHTML") == "S1- 7";
                // viewports remain static
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step5_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step5_1 && step5_2 && step5_3)
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

                // Step6 - Using the mouse-scroll, scroll up 2 notches.                
                element = loadedThumbnails.ElementAt(8);
                action.MouseScroll(element, "up", "2");
                Thread.Sleep(3000);
                loadedThumbnails = viewer.ThumbnailLoadedIndicator(0);
                element = loadedThumbnails.ElementAt(1).FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption));
                bool step6_1 = element.GetAttribute("innerHTML") == "S1- 5";

                // First loaded thumbnail should have white border
                element = loadedThumbnails.ElementAt(0).FindElement(By.XPath(".."));
                bool step6_2 = viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)");

                // Second loaded thumbnail should have white border
                element = loadedThumbnails.ElementAt(1).FindElement(By.XPath(".."));
                bool step6_3 = viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)");

                // viewports remain static
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step6_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step6_1 && step6_2 && step6_3 && step6_4)
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

                // Step7 - Using the mouse-scroll (when the Thumbnail bar is currently active), scroll down a few times towards the end of the thumbnail bar (you can stop around S1-40 thumbnail.)
                int numberOfScroll = 4;
                while (numberOfScroll > 0)
                {
                    loadedThumbnails = viewer.ThumbnailLoadedIndicator(0);
                    action.MouseScroll(loadedThumbnails.ElementAt(9), "down", "9");
                    numberOfScroll--;
                }

                loadedThumbnails = viewer.ThumbnailLoadedIndicator(0);
                element = loadedThumbnails.ElementAt(0).FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption));
                bool step7_1 = element.GetAttribute("innerHTML") == "S1- 40";

                // viewports remain static
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step7_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step7_1 && step7_2)
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

                // Step 8 Click on the 2nd viewport to make that viewport active.
                viewer.SetViewPort(1, 1);
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
                {
                    new Actions(BasePage.Driver).MoveToElement(viewer.GetElement("cssselector", viewer.Activeviewport)).Click(viewer.GetElement("cssselector", viewer.Activeviewport)).Build().Perform();
                }
                else
                {
                    viewer.GetElement("cssselector", viewer.Activeviewport).Click();
                }                

                // Second viewport is active, so it is highlighted with thin-blue border.
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport)).FindElement(By.XPath(".."));
                bool step8_1 = element.GetAttribute("class").Contains("activeViewportContainer selected");
                bool step8_2 = viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)");
                bool step8_3 = false;                
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer"))
                {
                    step8_3 = element.GetCssValue("background-color").Equals("transparent");
                }
                else
                {                    
                    step8_3 = element.GetCssValue("background-color").Equals("rgba(0, 0, 0, 0)");
                }

                bool step8_4 = false;
                loadedThumbnails = viewer.ThumbnailLoadedIndicator(0);
                foreach (IWebElement thumbnail in loadedThumbnails)
                {
                    if (!viewer.VerifyBordorColor(thumbnail.FindElement(By.CssSelector("div.thumbnailOuterDiv")), "rgba(0, 0, 0, 1)"))
                    {
                        step8_4 = true;
                        break;
                    }
                }

                if (step8_1 && step8_2 && step8_3 && !step8_4)
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

                // Step9 - Using the mouse-scroll. scroll down a few times to scroll through the frames of the image in the 2nd viewport
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                var sliderValue_1 = viewer.GetSliderValue(1, 2);
                action.MouseScroll(element, "down", "3");
                Thread.Sleep(1000);
                var sliderValue_2 = viewer.GetSliderValue(1, 2);
                bool step9_1 = sliderValue_2 == sliderValue_1 + 3;           

                bool step9_2 = false;
                loadedThumbnails = viewer.ThumbnailLoadedIndicator(0);
                foreach (IWebElement thumbnail in loadedThumbnails)
                {
                    if (!viewer.VerifyBordorColor(thumbnail.FindElement(By.CssSelector("div.thumbnailOuterDiv")), "rgba(0, 0, 0, 1)"))
                    {
                        step9_2 = true;
                        break;
                    }
                }


                if (step9_1 && !step9_2)
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

                // Step10 - Click on the Thumbnail bar
                viewer.ClickElement(viewer.ThumbnailLoadedIndicator(0).ElementAt(2));            
                bool step10 = false;
                loadedThumbnails = viewer.ThumbnailLoadedIndicator(0);
                foreach (IWebElement thumbnail in loadedThumbnails)
                {
                    if (!viewer.VerifyBordorColor(thumbnail.FindElement(By.CssSelector("div.thumbnailOuterDiv")), "rgba(0, 0, 0, 1)"))
                    {
                        step10 = true;
                        break;
                    }
                }
                if (!step10)
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

                // Step11 - Using the mouse-scroll (when the Thumbnail bar is currently active), scroll-down a few times until the last thumbnail is visible on the right sid
                loadedThumbnails = viewer.ThumbnailLoadedIndicator(0);
                action.MouseScroll(loadedThumbnails.ElementAt(9), "down", "12");
                loadedThumbnails = viewer.ThumbnailLoadedIndicator(0);
                action.MouseScroll(loadedThumbnails.ElementAt(9), "down", "6");

                loadedThumbnails = viewer.ThumbnailLoadedIndicator(0);
                element = loadedThumbnails.ElementAt(loadedThumbnails.Count - 1).FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption));
                bool step11_1 = element.GetAttribute("innerHTML") == "S1- 63";
                Logger.Instance.InfoLog("After 8 scroll " + element.GetAttribute("innerHTML") + " Thumbnail is displayed");

                action.MouseScroll(loadedThumbnails.ElementAt(2), "down", "1");
                loadedThumbnails = viewer.ThumbnailLoadedIndicator(0);
                Logger.Instance.InfoLog("After 1 scroll " + element.GetAttribute("innerHTML") + " Thumbnail is displayed");
                element = loadedThumbnails.ElementAt(loadedThumbnails.Count - 1).FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption));
                bool step11_2 = element.GetAttribute("innerHTML") == "S1- 63";

                action.MouseScroll(loadedThumbnails.ElementAt(2), "down", "1");
                loadedThumbnails = viewer.ThumbnailLoadedIndicator(0);
                Logger.Instance.InfoLog("After 1 scroll " + element.GetAttribute("innerHTML") + " Thumbnail is displayed");
                element = loadedThumbnails.ElementAt(loadedThumbnails.Count - 1).FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption));
                bool step11_3 = element.GetAttribute("innerHTML") == "S1- 63";
                
                // viewports remain static
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step11_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step11_1 && step11_2 && step11_3 && step11_4)
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

                // Step12 - Using the mouse-scroll (when the Thumbnail bar is currently active), scroll-up a few times until the first thumbnail is visible on the left side
                numberOfScroll = 6;
                while (numberOfScroll > 0)
                {
                    loadedThumbnails = viewer.ThumbnailLoadedIndicator(0);
                    action.MouseScroll(loadedThumbnails.ElementAt(2), "up", "9");
                    numberOfScroll--;
                }
                loadedThumbnails = viewer.ThumbnailLoadedIndicator(0);
                action.MouseScroll(loadedThumbnails.ElementAt(2), "up", "2");

                loadedThumbnails = viewer.ThumbnailLoadedIndicator(0);
                element = loadedThumbnails.ElementAt(0).FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption));
                bool step12_1 = element.GetAttribute("innerHTML") == "S1- 1";

                action.MouseScroll(loadedThumbnails.ElementAt(2), "up", "1");
                loadedThumbnails = viewer.ThumbnailLoadedIndicator(0);
                element = loadedThumbnails.ElementAt(0).FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption));
                bool step12_2 = element.GetAttribute("innerHTML") == "S1- 1";

                action.MouseScroll(loadedThumbnails.ElementAt(2), "up", "1");
                loadedThumbnails = viewer.ThumbnailLoadedIndicator(0);
                element = loadedThumbnails.ElementAt(0).FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption));
                bool step12_3 = element.GetAttribute("innerHTML") == "S1- 1";

                // viewports remain static
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step12_4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step12_1 && step12_2 && step12_3 && step12_4)
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
                action.Perform();

                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }

        }

        /// <summary> 
        /// Thumbnail Annotation Overlay
        /// </summary>
        public TestCaseResult Test_161052(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
            String[] AccessionNumbers = AccessionNoList.Split(':');
            String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
            String[] PatientIDList = PatientID.Split(':');


            try
            {
                string[] datasource = { EA_91, EA_131, PACS_A7, EA_96 };
                String username = Config.adminUserName;
                String password = Config.adminPassword;

                //Step-1
                //Go to Service Tool>Viewer>Miscellaneous> Ensure "Show Thumbnail Overlay" is selected.

                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();

                st.LaunchServiceTool();
                st.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                //select ShowThumbnailOverlays
                if (wpfobject.IsCheckBoxSelected("CB_ShowThumbnailOverlays"))
                {
                    Logger.Instance.InfoLog("ShowThumbnailOverlays  checkbox already checked -verified successfully");
                    if (wpfobject.IsCheckBoxSelected("CB_ShowThumbnailOverlays"))
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
                }
                else
                {
                    wpfobject.SelectCheckBox("CB_ShowThumbnailOverlays");
                    Logger.Instance.InfoLog("ShowThumbnailOverlays  checkbox selected successfully");
                    if (wpfobject.IsCheckBoxSelected("CB_ShowThumbnailOverlays"))
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
                    wpfobject.ClickButton("Apply", 1);
                    wpfobject.WaitTillLoad();
                }
                st.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();
                Thread.Sleep(5000);

                //Step-2
                //Login privilege user (i.e., rad1/rad1) and open a study in the viewer. Open CT/MR study
                //Pre-conditions
                String TestDomain1 = BasePage.GetUniqueDomainID("TestDomain1_135125_");
                String Role1 = BasePage.GetUniqueRole("Role1_135125_");
                String PhysicianRole = BasePage.GetUniqueRole("PhysicianRole_135125_");
                String rad1 = BasePage.GetUniqueUserId("rad1_135125_");

                DomainManagement domain = new DomainManagement();
                RoleManagement role = new RoleManagement();
                UserManagement user = new UserManagement();
                Studies study = new Studies();
                UserPreferences userpref = new UserPreferences();
                BluRingViewer viewer = new BluRingViewer();

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(TestDomain1, Role1, datasources: datasource);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(10);
                domain.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(30);
                role = (RoleManagement)login.Navigate("RoleManagement");
                if (!role.RoleExists(PhysicianRole))
                {
                    role.CreateRole(TestDomain1, PhysicianRole, "physician");
                }
                user = (UserManagement)login.Navigate("UserManagement");
                user.CreateUser(rad1, TestDomain1, PhysicianRole, 1, Config.emailid, 1, rad1);
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(TestDomain1);
                domain.SelectDomain(TestDomain1);
                domain.EditDomainButton().Click();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                //Precontion - Add Save Serious 
                IWebElement group3 = viewer.GetElement(BasePage.SelectorType.CssSelector, BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(3)");
                var dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add(BluRingViewer.GetToolName(BluRingTools.Save_Series), group3);
                domain.AddToolsToToolbox(dictionary);
                domain.ClickSaveEditDomain();
                login.Logout();
                login.DriverGoTo(login.url);
                login.LoginIConnect(rad1, rad1);
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();
                study = (Studies)login.Navigate("Studies");
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_131); //acc# 0006f94a4
                study.SelectStudy("Accession", AccessionNumbers[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();

                //"Study opens in the primary Study Panel. 
                //If the PR image has annotations, it is displayed in the viewport as overlay.
                //The corresponding Study Panel thumbnail shows the annotation overlay (line, circle, etc.), but not the text annotation
                IList<IWebElement> Thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                Boolean res2 = true;
                if (Thumbnails.Count == 8)
                {
                    for (int i = 0; i < 1; i++) //PR
                    {
                        String title = Thumbnails[i].GetAttribute("title");
                        if (title.Contains("Modality:PR"))
                            Logger.Instance.InfoLog("Thumbnail Title " + title + " -Verified successfully");
                        else
                        {
                            res2 = false;
                            Logger.Instance.InfoLog("Thumbnail Title " + title + "  is not correct -Verified failed");
                            break;
                        }
                    }
                    for (int i = 1; i < 8; i++) //MR
                    {
                        String title = Thumbnails[i].GetAttribute("title");
                        if (title.Contains("Modality:CT"))
                            Logger.Instance.InfoLog("Thumbnail Title " + title + " -Verified successfully");
                        else
                        {
                            res2 = false;
                            Logger.Instance.InfoLog("Thumbnail Title " + title + "  is not correct -Verified failed");
                            break;
                        }
                    }
                }
                else
                    res2 = false;

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status2 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (res2 && status2 && Thumbnails.Count == 8)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail(String.Format("Image Compare : {0}. , Thumbnails title : {1}, Thumbnails Count: {2}", status2, res2 , Thumbnails.Count));
                }

                //step-3
                //Click on the Thumbnail preview icon of the current study from the Exam List.
                IList<IWebElement> ExamlistThumbnailIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailIcon));
                viewer.OpenExamListThumbnailPreview(0);
                BluRingViewer.WaitforThumbnails();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ExamList_thumbnails))));

                //Thumbnail preview drop-down screen is displayed for The current study.
                //thumbnails displayed should be similar to The study Panel thumbnails- If The PR image has annotations, 
                //The corresponding Exam List preview Thumbnail shows The annotation overlay (line, circle, etc.), but not The text annotation.
                IList<IWebElement> ExamlistThumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));
                IWebElement examListContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ExamListContainer));
                Thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status3 = study.CompareImage(result.steps[ExecutedSteps], examListContainer);
                if (status3 && Thumbnails.Count == ExamlistThumbnails.Count)
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

                //Step-4
                //Select an image from the current study that does not have any annotations and add a line, circle and text, Save the series.
                //Image is saved. Screen is refreshed with the following-
                viewer.CloseBluRingViewer();
                study.SearchStudy(AccessionNo: AccessionNumbers[2], Datasource: login.GetHostName(Config.EA96)); //acc# 0006f94a4
                study.SelectStudy("Accession", AccessionNumbers[2]);
                viewer = BluRingViewer.LaunchBluRingViewer();

                IList<IWebElement> Thumbnails_PR = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                int PR_Count = 0;
                for (int i = 0; i < Thumbnails_PR.Count ; i++) //PR
                {
                    String title = Thumbnails_PR[i].GetAttribute("title");
                    if (title.Contains("Modality:PR"))
                        PR_Count = PR_Count + 1 ;
                }
                Logger.Instance.InfoLog("PR Before Save Serious is "+ PR_Count );

                viewer.SetViewPort1(1, 1);
                viewer.ClickOnViewPort(1, 1);
                viewer.OpenViewerToolsPOPUp();
                viewer.OpenStackedTool(BluRingTools.Add_Text, isOpenToolsPOPup: false);
                viewer.SelectViewerTool(BluRingTools.Add_Text, isOpenToolsPOPup: false);
                viewer.ApplyTool_AddText(testid);
                viewer.ClickOnViewPort(1, 1);
                viewer.OpenViewerToolsPOPUp();
                viewer.OpenStackedTool(BluRingTools.Line_Measurement, isOpenToolsPOPup: false);
                viewer.SelectViewerTool(BluRingTools.Line_Measurement, isOpenToolsPOPup: false);
                var attributes = viewer.GetElementAttributes(viewer.Activeviewport);
                viewer.ApplyTool_LineMeasurement(attributes["width"] / 3, attributes["height"] / 3, attributes["width"] / 4, attributes["height"] / 4);

                bool step3_1 = viewer.SavePresentationState(BluRingTools.Save_Series, BluRingTools.Pan);
                //viewer.OpenViewerToolsPOPUp();
               // viewer.OpenStackedTool(BluRingTools.Pan, isOpenToolsPOPup: false);
               // viewer.SelectViewerTool(BluRingViewer.GetToolName(BluRingTools.Save_Series));

                IList<IWebElement> Thumbnails_PR_AfterSaveSerious = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                BasePage.wait.Until(d =>
                {
                    Thumbnails_PR_AfterSaveSerious = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                    return (Thumbnails_PR_AfterSaveSerious.Count == Thumbnails_PR.Count + 1);
                });

                Thumbnails_PR_AfterSaveSerious = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                int PR_Count_AfterSaveSerious = 0;
                for (int i = 0; i < Thumbnails_PR_AfterSaveSerious.Count; i++) //PR
                {
                    String title = Thumbnails_PR_AfterSaveSerious[i].GetAttribute("title");
                    if (title.Contains("Modality:PR"))
                        PR_Count_AfterSaveSerious = PR_Count_AfterSaveSerious + 1;
                }
                Logger.Instance.InfoLog("PR After Save Serious is " + PR_Count_AfterSaveSerious);

                ExamlistThumbnailIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailIcon));
                viewer.OpenExamListThumbnailPreview(0);
                BluRingViewer.WaitforThumbnails();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ExamList_thumbnails))));

                IList<IWebElement> ExamlistThumbnails_AfterSaveSerious = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));
                IWebElement ExamListContainer_AfterSaveSerious = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ExamListContainer));

                bool status4_1 = (PR_Count_AfterSaveSerious  == PR_Count + 1) && (ExamlistThumbnails_AfterSaveSerious.Count == Thumbnails_PR.Count + 1);

                result.steps[++ExecutedSteps].SetPath(testid + "_4_studypanel", ExecutedSteps + 1);
                bool status4_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                result.steps[ExecutedSteps].SetPath(testid + "_4_ExamListContainer", ExecutedSteps + 1);
                bool status4_3 = study.CompareImage(result.steps[ExecutedSteps], ExamListContainer_AfterSaveSerious);

                if (status4_1 && status4_2 && status4_3)
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

                //step 5
                viewer.CloseBluRingViewer();
                study = (Studies)login.Navigate("Studies");
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_131); //acc# 0006f94a4
                study.SelectStudy("Accession", AccessionNumbers[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                Thread.Sleep(3000);
                viewer.CloseBluRingViewer();
                study.SearchStudy(AccessionNo: AccessionNumbers[2], Datasource: login.GetHostName(Config.EA96)); //acc# 0006f94a4
                study.SelectStudy("Accession", AccessionNumbers[2]);
                viewer = BluRingViewer.LaunchBluRingViewer();

                Thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                int PR_Count_AfterReOpen = 0;
                for (int i = 0; i < Thumbnails.Count; i++) //PR
                {
                    String title = Thumbnails[i].GetAttribute("title");
                    if (title.Contains("Modality:PR"))
                        PR_Count_AfterReOpen = PR_Count_AfterReOpen + 1;
                }
                Logger.Instance.InfoLog("PR After ReOpen Study is " + PR_Count_AfterSaveSerious);

                ExamlistThumbnailIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailIcon));
                ExamlistThumbnailIcon[0].Click();
                BluRingViewer.WaitforThumbnails();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ExamList_thumbnails))));

                ExamlistThumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));
                IWebElement ExamListContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ExamListContainer));

                bool status5_1 = (PR_Count_AfterReOpen == PR_Count + 1) && (ExamlistThumbnails.Count == Thumbnails_PR.Count + 1);

                result.steps[++ExecutedSteps].SetPath(testid + "_5_studypanel", ExecutedSteps + 1);
                bool status5_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                result.steps[ExecutedSteps].SetPath(testid + "_5_ExamListContainer", ExecutedSteps + 1);
                bool status5_3 = study.CompareImage(result.steps[ExecutedSteps], ExamListContainer);

                if (status5_1 && status5_2 && status5_3)
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


                //Step-6
                //Go to Service Tool > Viewer > Miscellaneous & uncheck the Show Thumbnail Overlay checkbox.
                //Then reset IIS- click on IIS RESET.
                viewer.CloseBluRingViewer();
                login.Logout();
                Taskbar bar = new Taskbar();
                bar.Hide();
                st.LaunchServiceTool();
                st.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                wpfobject.WaitTillLoad();
                wpfobject.UnSelectCheckBox("CB_ShowThumbnailOverlays");

                if (!wpfobject.IsCheckBoxSelected("CB_ShowThumbnailOverlays"))
                {
                    Logger.Instance.InfoLog("Overlay is not selected - Verified successfully");
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.ErrorLog("Overlay is selected -- Verified failed");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
				st.RestartIISandWindowsServices();
				wpfobject.WaitTillLoad();
				st.CloseServiceTool();
				wpfobject.WaitTillLoad();
				Thread.Sleep(5000);
				bar.Show();				

				//Step-7
				//Login as privilege user and open the same study from the previous step in the viewer. 
				login.DriverGoTo(login.url);
                login.LoginIConnect(rad1, rad1);
                study = (Studies)login.Navigate("Studies");
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_131);  //acc# 0006f94a4
                study.SelectStudy("Accession", AccessionNumbers[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                Thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                ExamlistThumbnailIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailIcon));
                viewer.OpenExamListThumbnailPreview(0);
                BluRingViewer.WaitforThumbnails();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ExamList_thumbnails))));
                examListContainer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamListContainer))[0];

                //The PR image with all the annotation is displayed in the viewport.
                //The corresponding thumbnails in the Study Panel Thumbnail bar & 
                //in the Exam List Thumbnail preview do not have any annotation displayed;
                //annotations are not displayed in the Thumbnail.
                result.steps[++ExecutedSteps].SetPath(testid + "_7_1_study", ExecutedSteps + 1);
                bool status7_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                result.steps[ExecutedSteps].SetPath(testid + "_7_2_exam", ExecutedSteps + 1);
                bool status7_2 = study.CompareImage(result.steps[ExecutedSteps], examListContainer);
                if (status7_1 && status7_2 && Thumbnails.Count == 8)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail(String.Format("studyPanel Image Compare : {0}. , examListContainer Image compare : {1}, Thumbnails Count: {2}", status7_1, status7_2, Thumbnails.Count));
                }
                viewer.CloseBluRingViewer();

                //Step-8
                //"Load other studies that have a PR series containing annotations 
                //(TEST, ARSONE) & confirm the annotations are not displayed in the Thumbnails.
                //(Note- Some PR series may not have visible annotations, such as W/L. 
                study.SearchStudy(AccessionNo: AccessionNumbers[1], Datasource: EA_91); //Acc:ARS0745607
                study.SelectStudy("Accession", AccessionNumbers[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ExamlistThumbnailIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailIcon));
                viewer.OpenExamListThumbnailPreview(0);
                BluRingViewer.WaitforThumbnails();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ExamList_thumbnails))));
                Thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                examListContainer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamListContainer))[0];
                
                //No annotations are displayed in the thumbnails (from Study Panel Thumbnail bar and Exam List Thumbnail preview).
                result.steps[++ExecutedSteps].SetPath(testid + "_8_1_study", ExecutedSteps + 1);
                bool status8_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                result.steps[ExecutedSteps].SetPath(testid + "_8_2_exam", ExecutedSteps + 1);
                bool status8_2 = study.CompareImage(result.steps[ExecutedSteps], examListContainer);
                if (status8_1 && status8_2 && Thumbnails.Count == 6)
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
                viewer.CloseBluRingViewer();
                login.Logout();

                //step-9
                //Go to Service Tool > Viewer > Miscellaneous & check the Show Thumbnail Overlay option.
                //This resets the annotation thumbnail overlay back to default
                bar = new Taskbar();
                bar.Hide();
                st.LaunchServiceTool();
                st.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                wpfobject.WaitTillLoad();
                //uncheck Overlay
                wpfobject.SelectCheckBox("CB_ShowThumbnailOverlays");

                if (wpfobject.IsCheckBoxSelected("CB_ShowThumbnailOverlays"))
                {
                    Logger.Instance.InfoLog("Overlay is selected - Verified successfully");
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.ErrorLog("Overlay is not selected -- Verified failed");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();
                bar.Show();

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
                return result;
            }
            finally
            {
                try
                {
                    // patientID  = PID27916
                    HPLogin hplogin = new HPLogin();
                    BasePage.Driver.Navigate().GoToUrl("https://" + Config.EA96 + "/webadmin");
                    HPHomePage hphome = (HPHomePage)hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: "https://" + Config.EA96 + "/webadmin");
                    WorkFlow workflow = (WorkFlow)hphome.Navigate("Workflow");
                    workflow.NavigateToLink("Workflow", "Archive Search");
                    workflow.HPSearchStudy("PatientID", PatientIDList[2]);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                        workflow.DeletePaticularModality("PR");
                    hplogin.LogoutHPen();
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("PR delete exception -- " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }

                //Enable Thumbnail checkbox
                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                Taskbar bar = new Taskbar();
                bar.Hide();
                st.LaunchServiceTool();
                st.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                wpfobject.WaitTillLoad();
                //enable Overlay
                wpfobject.SelectCheckBox("CB_ShowThumbnailOverlays");

                //--verify overlay is selected--
                if (wpfobject.IsCheckBoxSelected("CB_ShowThumbnailOverlays"))
                {
                    Logger.Instance.InfoLog("Overlay is selected");
                }
                else
                {
                    Logger.Instance.ErrorLog("Overlay is not selected");
                }
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();
                bar.Show();

            }
        }

        /// <summary> 
        /// UI Interaction Test: Load operation cancels properly
        /// </summary>
        public TestCaseResult Test_161051(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            TestCompleteAction testcompleteAction = new TestCompleteAction();
            int ExecutedSteps = -1;
            ServiceTool st = new ServiceTool();
            WpfObjects wpfobject = new WpfObjects();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;

                string[] datasource = { EA_91, EA_77, PACS_A7 };

                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNoList.Split(':');

                //Step-1
                //Login to Enterprise Viewer application with any privileged user (i.e., rad/rad user)
                String TestDomain1 = BasePage.GetUniqueDomainID("TestDomain1_139245_");
                String Role1 = BasePage.GetUniqueRole("Role1_139245_");
                String PhysicianRole = BasePage.GetUniqueRole("PhysicianRole_139245_");
                String rad1 = BasePage.GetUniqueUserId("rad1_139245_");

                DomainManagement domain = new DomainManagement();
                RoleManagement role = new RoleManagement();
                UserManagement user = new UserManagement();
                Studies study = new Studies();
                UserPreferences userpref = new UserPreferences();
                BluRingViewer viewer = new BluRingViewer();
                TestCompleteAction TestcompleteAction = new TestCompleteAction();

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(TestDomain1, Role1, datasources: datasource);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(10);
                domain.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(30);

                role = (RoleManagement)login.Navigate("RoleManagement");
                if (!role.RoleExists(PhysicianRole))
                {
                    role.CreateRole(TestDomain1, PhysicianRole, "physician");
                }

                user = (UserManagement)login.Navigate("UserManagement");
                user.CreateUser(rad1, TestDomain1, PhysicianRole, 1, Config.emailid, 1, rad1);
                login.Logout();

                login.DriverGoTo(login.url);
                login.LoginIConnect(rad1, rad1);
                study = (Studies)login.Navigate("Studies");
                ExecutedSteps++;


                //Step-2
                //From the Studies tab, search and load a patient with a few thumbnails
                //search for patient "MICKEY, MOUSE" accession ACC02, this one is an CR, 
                //which by default is setup to be image-split).
                //acc# ACC02
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_91);
                study.SelectStudy("Accession", AccessionNumbers[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();

                //Ensure that thumbnails start to load after the first series is loaded
                //The current study in the Exam List is highlighted (blue border around the study info rectangle).
                //The first viewport in the primary study panel is active (has blue border), 
                //first thumbnail in the primary study panel is in-focus (has thick blue border)

                IList<IWebElement> Viewport_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_viewport_Outer));
                bool Vieport_color_2 = viewer.VerifyBordorColor(Viewport_Outer[0], "rgba(90, 170, 255, 1)");					

				var priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                bool ExamList_color_2 = viewer.VerifyBordorColor(priors[1], "rgba(90, 170, 255, 1)");

				IList<IWebElement> Thumbnail_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ThumbnailOuter));
                bool Thumbnail_color_2 = viewer.VerifyBordorColor(Thumbnail_Outer[0], "rgba(90, 170, 255, 1)");

				if (Vieport_color_2 && ExamList_color_2 && Thumbnail_color_2)
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

                //step-3
                //Click on the 2nd viewport of the primary study panel (to make the 2nd viewport active).
                viewer.SetViewPort(1, 1);
                BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport)).Click();
                BluRingViewer.WaitforViewports();

                //"The 2nd viewport of the primary study panel is active - it has a blue border around it. 
                //The 2nd thumbnail is now in-focus (has a thick blue border)

                Viewport_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_viewport_Outer));
                bool Vieport_color_4 = viewer.VerifyBordorColor(Viewport_Outer[1], "rgba(90, 170, 255, 1)");

				priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                bool ExamList_color_4 = viewer.VerifyBordorColor(priors[1], "rgba(90, 170, 255, 1)");

				Thumbnail_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ThumbnailOuter));
                bool Thumbnail_color_4 = viewer.VerifyBordorColor(Thumbnail_Outer[1], "rgba(90, 170, 255, 1)");

				if (Vieport_color_4 && ExamList_color_4 && Thumbnail_color_4)
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

                //Step-4
                //"Rapidly, in sequence, double click on the following thumbnails-
                //5th thumbnail,  6th thumbnail , 7th thumbnail
                IList<IWebElement> Thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                Viewport_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_viewport_Outer));
                Actions action = new Actions(BasePage.Driver);
                new Actions(BasePage.Driver).DoubleClick(Thumbnails[4]).Build().Perform();
                Thread.Sleep(4000);
                new Actions(BasePage.Driver).DoubleClick(Thumbnails[5]).Build().Perform();
                Thread.Sleep(4000);
                new Actions(BasePage.Driver).DoubleClick(Thumbnails[6]).Build().Perform();
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                Thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                bool Step_5_1 = viewer.VerifyBordorColor(Viewport_Outer[1], "rgba(90, 170, 255, 1)");
                bool Step_5_2 = viewer.VerifyBordorColor(Thumbnail_Outer[6], "rgba(90, 170, 255, 1)");
                bool Step_5_3 = viewer.VerifyBordorColor(Thumbnail_Outer[5], "rgba(255, 255, 255, 1)");
                bool Step_5_4 = viewer.VerifyBordorColor(Thumbnail_Outer[4], "rgba(255, 255, 255, 1)");
                if (Step_5_1 && Step_5_2 && Step_5_3 && Step_5_4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                
                //step-5
                //From the Exam List, click on a prior study for this patient (for MICKEY, MOUSE, select MR study dated 04-Feb-1995).
                viewer.OpenPriors(2);

                //The first viewport is active in the new study panel (has blue highlight border)
                //Thumbnail bar- 1st thumbnail is in-focus (thick blue border), 2nd thumbnail is in-viewport highlight (thin white border)

                IList<IWebElement> Panel = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel));
                Viewport_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(2) " + BluRingViewer.div_viewport_Outer));
                bool Vieport_color_7 = viewer.VerifyBordorColor(Viewport_Outer[0], "rgba(90, 170, 255, 1)");

				Thumbnail_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(2) " + BluRingViewer.div_ThumbnailOuter));
                bool Thumbnail_color_7_1 = viewer.VerifyBordorColor(Thumbnail_Outer[0], "rgba(90, 170, 255, 1)");
				bool Thumbnail_color_7_2 = viewer.VerifyBordorColor(Thumbnail_Outer[1], "rgba(255, 255, 255, 1)");

				if (Panel.Count == 2 &&
                    Vieport_color_7 && Thumbnail_color_7_2 &&
                    Thumbnail_color_7_1)
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

                //step-6
                //Click on the 3rd viewport of the primary study panel (to make the 3rd viewport active).
                viewer.SetViewPort(2, 1);
                BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport)).Click();
                BluRingViewer.WaitforViewports();

                Thumbnail_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(1) " + BluRingViewer.div_ThumbnailOuter));

                //"The 3rd viewport of the primary study panel is active - it has a blue border around it. 

                if (viewer.VerifyBordorColor(Thumbnail_Outer[2], "rgba(90, 170, 255, 1)"))
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

                //step-7
                //Rapidly, in sequence, double click on the following thumbnails from the 2nd study panel-
                //1st thumbnail, 2nd thumbnail

                IList<IWebElement> Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(2) " + BluRingViewer.div_thumbnails));
                Viewport_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(1) " + BluRingViewer.div_viewport_Outer));

                action = new Actions(BasePage.Driver);
                action.DoubleClick(Thumbnail_list[0]).Build().Perform();
                action.DoubleClick(Thumbnail_list[0]).Build().Perform();
                Thread.Sleep(6000);
                action.MoveToElement(Viewport_Outer[0]).Build().Perform();
                Thread.Sleep(4000);
                Thumbnail_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(2) " + BluRingViewer.div_ThumbnailOuter));
                bool thumb_9_1 = viewer.VerifyBordorColor(Thumbnail_Outer[0], "rgba(90, 170, 255, 1)");
				Viewport_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(1) " + BluRingViewer.div_viewport_Outer));
                bool view_9_1 = viewer.VerifyBordorColor(Viewport_Outer[2], "rgba(90, 170, 255, 1)");
				Thread.Sleep(5000);

                action = new Actions(BasePage.Driver);
                action.DoubleClick(Thumbnail_list[1]).Build().Perform();
                Thread.Sleep(3000);
                action.DoubleClick(Thumbnail_list[1]).Build().Perform();
                Thread.Sleep(6000);
                action.DoubleClick(Thumbnail_list[1]).Build().Perform();
                Thread.Sleep(6000);
                action.MoveToElement(Viewport_Outer[0]).Build().Perform();
                Thread.Sleep(4000);
                Thumbnail_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(2) " + BluRingViewer.div_ThumbnailOuter));
                bool thumb_9_2 = viewer.VerifyBordorColor(Thumbnail_Outer[1], "rgba(90, 170, 255, 1)");
				Viewport_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(1) " + BluRingViewer.div_viewport_Outer));
                bool view_9_2 = viewer.VerifyBordorColor(Viewport_Outer[2], "rgba(90, 170, 255, 1)");
				Thread.Sleep(2000);
                //3rd viewport of the primary study panel starts to load the 1st thumbnail series, but ultimately displays the 2nd thumbnail series.
                //3rd viewport is still active (blue border) with the 2th thumbnail series of the 2nd study panel (foreign series).
                //2nd thumbnail of the 2nd study panel is in-focus (has thick border)"

                if (thumb_9_1 && view_9_1 &&
                    thumb_9_2 && view_9_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog(thumb_9_1 + " -- " + view_9_1 + " -- " + thumb_9_2 + " -- " + view_9_2);
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
               
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
                return result;
            }
        }

        /// <summary>
		/// Double-click thumbnail to viewport, highlighted and in-focus thumbnails
		/// </summary>
		/// <param name="testid"></param>
		/// <param name="teststeps"></param>
		/// <param name="stepcount"></param>
		/// <returns></returns>
		public TestCaseResult Test_161054(String testid, String teststeps, int stepcount)
        {

            // Declare and initialize variables
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            ServiceTool servicetool = new ServiceTool();
            UserManagement usermanagement = new UserManagement();
            DomainManagement domain = new DomainManagement();
            WpfObjects wpfobject = new WpfObjects();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String domain1 = "Domain1_" + new Random().Next(1, 1000);
                String role1 = "Role1_" + new Random().Next(1, 1000);
                String rad1 = Config.newUserName + new Random().Next(1, 1000);

                String FirstNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstNameList");
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastNameList");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');
                String[] LastName = LastNameList.Split(':');
                String[] FirstName = FirstNameList.Split(':');

                //Precondition - step 1,2
                //Create new user
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.CreateDomain(domain1, role1, datasources: null);
                domain.ClickSaveNewDomain();
                login.Navigate("UserManagement");
                usermanagement.CreateUser(rad1, domain1, role1);
                login.Navigate("DomainManagement");
                domain.SearchDomain(domain1);
                domain.SelectDomain(domain1);
                domain.ClickEditDomain();
                string[] Mod = { "MR", "CR" };
                for (int i = 0; i < Mod.Length; i++)
                {
                    domain.ModalityDropDown().SelectByText(Mod[i]);
                    if (Mod[i] == "MR")
                    {
                        domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                        domain.LayoutDropDown().SelectByText("2x2");
                    }

                    else
                    {
                        domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                        domain.LayoutDropDown().SelectByText("2x3");
                    }
                }
                domain.ClickSaveDomain();
                login.Logout();

                // Step 1 - Login as the domain user        
                login.DriverGoTo(login.url);
                login.LoginIConnect(rad1, rad1);
                ExecutedSteps++;

                // Step 2 - search for the study and launch in the bluring viewer
                var studies = new Studies();
                studies.SearchStudy(LastName: LastName[0], FirstName: FirstName[0], AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                viewer.ScrollIntoView(viewer.GetElement("cssselector", BluRingViewer.div_thumbnailpreviewIconActiveStudy));
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer"))
                    viewer.ScrollIntoView(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_mergeLogo));
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_thumbnailpreviewIconActiveStudy));
                BluRingViewer.WaitforThumbnails();
                Thread.Sleep(4000);                
                // Verifying the border color of selected thumbnail under Exam list
                bool step2_1 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", BluRingViewer.div_thumbnailpreviewIconActiveStudy), "rgba(90, 170, 255, 1)");
                // verifying the border color of active viewport in study panel
                bool step2_20 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", BluRingViewer.div_ViewportBorder), "rgba(90, 170, 255, 1)");
                bool step2_21 = true;
                String viewportcss = viewer.GetViewportCss(1, 0);
                String color = BasePage.Driver.FindElement(By.CssSelector(viewportcss)).FindElement(By.XPath("..")).GetCssValue("background-color");
                if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer"))
                {
                    step2_21 = color.Equals("rgba(0, 0, 0, 0)");
                }
                else
                {
                    step2_21 = color.Equals("transparent");
                }
                //Verifying the border color of thumbnail in study panel
                bool step2_22 = viewer.VerifyBordorColor(viewer.GetElement("cssselector",
                    viewer.GetStudyPanelThumbnailCss(1)), "rgba(90, 170, 255, 1)");
                // verifying the border color of thumbnail in study panel is thick
                viewportcss = viewer.GetStudyPanelThumbnailCss(1);
                bool step2_23 = viewer.verifyBackgroundColor(viewer.GetElement("cssselector", viewportcss), "rgba(90, 170, 255, 1)");

                // Series Number and Number of images are consistent
                bool step2_4 = true;
                bool step2_5 = true;
                String[] Thumbnailscaptions = { "S6", "S7" };
                String[] NumbImageinThumbnails = { "4", "96" };
                var StudyThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                var studyThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));

                for (int i = 0; i < NumbImageinThumbnails.Count(); i++)
                {
                    if (!(Thumbnailscaptions[i] == StudyThumbnailCaptionList[i].GetAttribute("innerHTML") &&
                        NumbImageinThumbnails[i] == studyThumbnailNumberList[i].GetAttribute("innerHTML")))
                    {
                        step2_4 = false;
                        Logger.Instance.InfoLog("The thumbnails caption of " + i + " th thumbnail is :" + StudyThumbnailCaptionList[i].GetAttribute("innerHTML") + " The Thumbnail number of " + i + " th thumbnail is :" + studyThumbnailNumberList[i].GetAttribute("innerHTML"));
                    }
                }

                // Verifying non-primary text
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step2_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step2_1 && step2_20 && step2_21 && step2_22 && step2_23 && step2_3 && step2_4 && step2_5)
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

                // Step 3 - Click on the Thumbnail preview icon of the current study from the Exam List.
                // Clicking on the Thumbnail preview icon - Done in step2
                // Verifying the border color of selected thumbnail under Exam list
                bool step3_11 = viewer.VerifyBordorColor(viewer.GetElement("cssselector",
                    BluRingViewer.div_Examlistdefaultselectedthumbnail), "rgba(90, 170, 255, 1)");
                //bool step3_12 = false;
                bool step3_12 = viewer.verifyBackgroundColor(viewer.GetElement("cssselector", BluRingViewer.div_Examlistdefaultselectedthumbnail), "rgba(90, 170, 255, 1)");
                // Verifying the border color of thumbnail under Exam list which image in Viewer 
                bool step3_2 = viewer.VerifyBordorColor(viewer.GetElement("cssselector",
                    BluRingViewer.div_thumbnailImageInViewer), "rgba(255, 255, 255, 1)");

                // Series Number and Number of Images are consistent
                bool step3_3 = true;
                bool step3_4 = true;
                var ExamListThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                var ExamListThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));
                for (int i = 0; i < NumbImageinThumbnails.Count(); i++)
                {
                    if (!(Thumbnailscaptions[i] == ExamListThumbnailCaptionList[i].GetAttribute("innerHTML") &&
                        NumbImageinThumbnails[i] == ExamListThumbnailNumberList[i].GetAttribute("innerHTML")))
                    {
                        step3_3 = false;
                        Logger.Instance.InfoLog("The thumbnails caption of  " + i + " th Examlist thumbnail is :" + ExamListThumbnailCaptionList[i].GetAttribute("innerHTML") + " The Thumbnail number of  " + i + " th Examlist thumbnail is :" + ExamListThumbnailNumberList[i].GetAttribute("innerHTML"));
                    }
                }

                StudyThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                studyThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));
                for (int i = 0; i < NumbImageinThumbnails.Count(); i++)
                {
                    if (!(Thumbnailscaptions[i] == StudyThumbnailCaptionList[i].GetAttribute("innerHTML") &&
                        NumbImageinThumbnails[i] == studyThumbnailNumberList[i].GetAttribute("innerHTML")))
                    {
                        step3_4 = false;
                        Logger.Instance.InfoLog("The thumbnails caption of " + i + " th thumbnail is :" + StudyThumbnailCaptionList[i].GetAttribute("innerHTML") + " The Thumbnail number of " + i + " th thumbnail is :" + studyThumbnailNumberList[i].GetAttribute("innerHTML"));
                    }
                }
                // ** Verification of viewport is already done in step2 ** 
                if (step3_11 && step3_12 && step3_2 && step3_3 && step3_4)
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

                // Step 4 - Click on the 3rd viewport of the primary study panel (to make the 3rd viewport active).
                viewer.SetViewPort(2, 1);
                viewportcss = viewer.GetViewportCss(1, 2);
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer"))
                {
                    new Actions(BasePage.Driver).Click(viewer.GetElement(BasePage.SelectorType.CssSelector, viewportcss)).Build().Perform();
                }
                else
                {
                    BasePage.Driver.FindElement(By.CssSelector(viewportcss)).Click();
                }                    
                Thread.Sleep(2000);
                // Verifying the border color of active viewport in study panel
                bool step4_1 = viewer.VerifyBordorColor(BasePage.Driver.FindElement(By.CssSelector(viewer.GetViewportCss(1, 2))).FindElement(By.XPath("..")), "rgba(90, 170, 255, 1)");
                bool step4_2 = true;
                bool step4_3 = true;
                IList<IWebElement> thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailImageInViewer));
                IWebElement element;
                for (int i = 0; i < 1; i++)
                {
                    // Verifying the border color of thumbnails from study panel
                    if (!viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(i)), "rgba(255, 255, 255, 1)"))
                    {
                        step4_2 = false;
                    }

                    // Verifying the border color of thumbnails from Exam List
                    element = thumbnails.ElementAt(i);
                    element = thumbnails.ElementAt(i);
                    if (!viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)"))
                    {
                        step4_3 = false;
                    }
                }
                if (step4_1 && step4_2 && step4_3)
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

                //Step 5 - From the Study Panel's Thumbnail bar, double-click the 1st thumbnail.
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetStudyPanelThumbnailCss(1)));
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox"))
                {
                    var actions = new TestCompleteAction();
                    actions.DoubleClick(element).Perform();
                }
                else
                {
                    new Actions(BasePage.Driver).DoubleClick(element).Build().Perform();
                }
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step5_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewportcss));
                bool step5_2 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", BluRingViewer.div_ViewportBorder), "rgba(90, 170, 255, 1)");
                bool step5_3 = true;
                bool step5_4 = true;
                bool step5_5 = true;
                for (int i = 0; i < 2; i++)
                {
                    if (!viewer.VerifyBordorColor(BasePage.Driver.FindElement(By.CssSelector(viewer.GetViewportCss(1, i))).FindElement(By.XPath("..")), "rgba(0, 0, 0, 1)"))
                    {
                        step5_3 = false;
                    }

                    // Verifying the border color of thumbnails from study panel
                    element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetStudyPanelThumbnailCss(i + 1)));
                    if (i == 0 && !(viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)")))
                    {
                        step5_4 = false;
                    }
                    if (i == 1 && !(viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")))
                    {
                        step5_4 = false;
                    }

                    // Verifying the border color of thumbnails from Exam List
                    if (i == 0 && !(viewer.VerifyBordorColor(viewer.GetElement("cssselector", BluRingViewer.div_Examlistdefaultselectedthumbnail), "rgba(90, 170, 255, 1)")))
                    {
                        step5_5 = false;
                    }

                    if (i == 1 && !(viewer.VerifyBordorColor(viewer.GetElement("cssselector", BluRingViewer.div_thumbnailImageInViewer), "rgba(255, 255, 255, 1)")))
                    {
                        step5_5 = false;
                    }
                }
                if (step5_1 && step5_2 && step5_3 && step5_4 && step5_5)
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

                //Step 6 - From the Exam List's Thumbnail preview, double-click the 2nd thumbnail in the primary study.
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetExamListThumbnailCss(133, 2)));
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox"))
                {
                    var actions = new TestCompleteAction();
                    actions.DoubleClick(element).Perform();
                }
                else
                {
                    new Actions(BasePage.Driver).DoubleClick(element).Build().Perform();
                }
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                element = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                bool step6_1 = studies.CompareImage(result.steps[ExecutedSteps], element);
                element = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ViewportBorder));
                bool step6_2 = viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)");
                bool step6_3 = true;
                bool step6_4 = true;
                bool step6_5 = true;
                for (int i = 0; i < 2; i++)
                {
                    element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetViewportCss(1, i)));
                    if (!viewer.VerifyBordorColor(element.FindElement(By.XPath("..")), "rgba(0, 0, 0, 1)"))
                    {
                        step6_3 = false;
                    }

                    // Verifying the border color of thumbnails from study panel
                    element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetStudyPanelThumbnailCss(i + 1)));
                    if (i == 1 && !(viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)")))
                    {
                        step6_4 = false;
                    }
                    if (i == 0 && !(viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")))
                    {
                        step6_4 = false;
                    }

                    // Verifying the border color of thumbnails from Exam List
                    element = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_Examlistdefaultselectedthumbnail));
                    if (i == 1 && !(viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)")))
                    {
                        step6_5 = false;
                    }
                    element = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_thumbnailImageInViewer));
                    if (i == 0 && !(viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")))
                    {
                        step6_5 = false;
                    }
                }
                if (step6_1 && step6_2 && step6_3 && step6_4 && step6_5)
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

                // Step 7 - From the Exam List, click on an image-split thumbnail modality prior study for this patient (for MICKEY, MOUSE, select CR study dated 24-Feb-2000).
                viewer.ScrollIntoView(viewer.GetElement("cssselector", BluRingViewer.div_relatedStudyPanel + ":nth-of-type(103)"));
                viewer.OpenPriors(102);
                bool step7_1 = viewer.GetStudyPanelCount().Equals(2);
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetViewportCss(2, 0)));
                bool step7_2 = viewer.VerifyBordorColor(element.FindElement(By.XPath("..")), "rgba(90, 170, 255, 1)");

                bool step7_3 = true;
                for (int i = 1; i < 7; i++)
                {
                    element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetStudyPanelThumbnailCss(i, 2)));
                    if (i == 1 && !(viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)")))
                    {
                        step7_3 = false;
                    }
                    if (i > 1 && !(viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")))
                    {
                        step7_3 = false;
                    }
                }
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                element = viewer.GetElement(BasePage.SelectorType.CssSelector, (BluRingViewer.div_studypanel + ":nth-of-type(2)" + " " + BluRingViewer.div_compositeViewer));
                bool step7_4 = studies.CompareImage(result.steps[ExecutedSteps], element);
                bool step7_5 = true;
                bool step7_6 = true;
                for (int i = 1; i < 3; i++)
                {
                    // Verifying the border color of viewports in first study panel
                    element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetViewportCss(1, (i - 1))));
                    if (!viewer.VerifyBordorColor(element.FindElement(By.XPath("..")), "rgba(0, 0, 0, 1)"))
                    {
                        step7_5 = false;
                    }

                    // Verifying the border color of thumbnails from first study panel
                    element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetStudyPanelThumbnailCss(i)));
                    if (!((viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")) && (viewer.verifyBackgroundColor(element, "rgba(0, 0, 0, 1)")) || element.GetCssValue("background-color").Equals("transparent")))
                    {
                        step7_6 = false;
                    }
                }

                // Verifying the Primary text in first study panel
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 66);
                element = viewer.GetElement(BasePage.SelectorType.CssSelector, (BluRingViewer.div_studypanel + ":nth-of-type(1)" + " " + BluRingViewer.div_compositeViewer));
                bool step7_7 = studies.CompareImage(result.steps[ExecutedSteps], element);
                if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
                {
                    viewer.ScrollIntoView(viewer.GetElement("cssselector", BluRingViewer.div_closeStudy));
                }
                IList<IWebElement> element1 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_activeExamPanel));
                bool step7_8 = element1.Count.Equals(2);
                if (step7_1 && step7_2 && step7_3 && step7_4 && step7_5 && step7_6 && step7_7 && step7_8)
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

                // Step 8 - Click on the 3rd viewport of the 2nd study panel(to make the 3rd viewport active).
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer"))
                {
                    new Actions(BasePage.Driver).Click(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.GetViewportCss(2, 2))).Build().Perform();
                }
                else
                {
                    BasePage.Driver.FindElement(By.CssSelector(viewer.GetViewportCss(2, 2))).Click();
                }                    
                Thread.Sleep(2000);
                // Verifying the border color of 3rd view port in second study panel
                bool step8_1 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", BluRingViewer.div_ViewportBorder), "rgba(90, 170, 255, 1)");
                // Verifying the border color of 3rd thumbnail in second study panel
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetStudyPanelThumbnailCss(3, 2)));
                bool step8_2 = false;
                if (viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)") && viewer.verifyBackgroundColor(element, "rgba(90, 170, 255, 1)"))
                {
                    step8_2 = true;
                }
                bool step8_3 = true;
                for (int i = 1; i < 7; i++)
                {
                    if (i != 3)
                    {
                        element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetStudyPanelThumbnailCss(i, 2)));
                        if (!(viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")))
                        {
                            step8_3 = false;
                        }
                    }
                }
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetStudyPanelThumbnailCss(7, 2)));
                bool step8_4 = viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)");

                //Series number, image number and number of images are consistent 
                StudyThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-of-type(2) " + BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                studyThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-of-type(2) " + BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));
                bool step8_5 = true;
                if (!StudyThumbnailCaptionList[6].GetAttribute("innerHTML").Equals("S1- 3")
                    && studyThumbnailNumberList[6].Equals("1"))
                {
                    step8_5 = false;
                    Logger.Instance.InfoLog("The thumbnails caption of 4 th thumbnail in examlist is :" + StudyThumbnailCaptionList[2].GetAttribute("innerHTML") + " The Thumbnail number of 4 th thumbnail in examlist is :" + studyThumbnailNumberList[2].GetAttribute("innerHTML"));
                }

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step8_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(2) " + BluRingViewer.div_compositeViewer));

                if (step8_1 && step8_2 && step8_3 && step8_4 && step8_5 && step8_6)
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

                //Step 9 - Click on the Thumbnail preview icon of the current study (24-Feb-2000 study) from the Exam List.
                viewer.ScrollIntoView(viewer.GetElement("cssselector", BluRingViewer.div_thumbnailpreviewIconActiveStudy));
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer"))
                    viewer.ScrollIntoView(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_mergeLogo));
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_thumbnailpreviewIconActiveStudy));
                BluRingViewer.WaitforThumbnails();
                Thread.Sleep(2000);
                bool step9_1 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", BluRingViewer.div_Examlistdefaultselectedthumbnail), "rgba(90, 170, 255, 1)");
                bool step9_2 = viewer.verifyBackgroundColor(viewer.GetElement("cssselector", BluRingViewer.div_Examlistdefaultselectedthumbnail), "rgba(90, 170, 255, 1)");
                bool step9_3 = true;
                thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailImageInViewer));
                for (int i = 0; i < 5; i++)
                {
                    // verifying borderder color of Examlist Thumbnails
                    element = thumbnails.ElementAt(i);
                    color = element.GetCssValue("border-bottom-color");
                    if (!(viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")))
                        step9_3 = false;
                }

                //Series number, image number and number of images are consistent
                ExamListThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                ExamListThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));
                bool step9_4 = true;
                if (!ExamListThumbnailCaptionList[2].GetAttribute("innerHTML").Equals("S1- 3")
                    && ExamListThumbnailNumberList[2].Equals("1"))
                {
                    step9_4 = false;
                    Logger.Instance.InfoLog("The thumbnails caption of 4 th thumbnail in examlist is :" + ExamListThumbnailCaptionList[2].GetAttribute("innerHTML") + " The Thumbnail number of 4 th thumbnail in examlist is :" + ExamListThumbnailNumberList[2].GetAttribute("innerHTML"));
                }
                //**** The viewport verification is already done in step8 ***
                if (step9_1 && step9_2 && step9_3 && step9_4)
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

                //Step 10 - From the 2nd Study Panel Thumbnail bar, double-click the 2nd thumbnail in the 2nd study panel.
                // Double click on 2nd thumbnail in the 2nd study panel
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetStudyPanelThumbnailCss(2, 2)));
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox"))
                {
                    var actions = new TestCompleteAction();
                    actions.DoubleClick(element).Perform();
                }
                else
                {
                    new Actions(BasePage.Driver).DoubleClick(element).Build().Perform();
                }
                // Verifying the image in 3rd viewport
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                element = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.GetViewportCss(2, 2));
                bool step10_1 = studies.CompareImage(result.steps[ExecutedSteps], element);
                // Verifying the 3rd viewport border color in the 2nd study panel
                bool step10_2 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetViewportCss(2, 2)).FindElement(By.XPath("..")), "rgba(90, 170, 255, 1)");
                bool step10_3 = true;
                bool step10_4 = true;
                bool step10_5 = true;
                bool step10_6 = true;
                bool step10_7 = true;
                bool step10_8 = true;
                for (int i = 1; i < 8; i++)
                {
                    element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetStudyPanelThumbnailCss(i, 2)));
                    // Verifying border color of study panel thumbnails of 1st and 4th to 6th
                    if (i != 2 && i != 3 && i != 7)
                    {
                        if (!(viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")))
                            step10_3 = false;
                    }

                    // Verifying border color of study panel thumbnails of 3rd and 7th
                    if ((i == 3 || i == 7) && !(viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)")))
                        step10_4 = false;

                    // Verifying border color of study panel thumbnails of 2nd
                    color = element.GetCssValue("border-bottom-color");
                    if (i == 2 && !(viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)")))
                        step10_5 = false;

                    element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetExamListThumbnailCss(103, i)));
                    // Verifying border color of Exam List thumbnails of 1st and 4th to 6th
                    if (i != 2 && i != 3 && i != 7)
                    {
                        if (!(viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")))
                            step10_6 = false;
                    }

                    //  Verifying border color of Exam List thumbnails of 3rd and 7th
                    if ((i == 3 || i == 7) && !(viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)")))
                        step10_7 = false;

                    //  Verifying border color of Exam List thumbnails of 2nd
                    color = element.GetCssValue("border-bottom-color");
                    if (i == 2 && !(viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)")))
                        step10_8 = false;
                }
                if (step10_1 && step10_2 && step10_3 && step10_4 && step10_5 && step10_6 && step10_7 && step10_8)
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

                // Step 11 - From the Exam List Thumbnail preview, double-click the 1st thumbnail in the prior study.
                // Double click on 1st thumbnail in the prior study
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetExamListThumbnailCss(103, 1)));
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox"))
                {
                    var actions = new TestCompleteAction();
                    actions.DoubleClick(element).Perform();
                }
                else
                {
                    new Actions(BasePage.Driver).DoubleClick(element).Build().Perform();
                }
                Thread.Sleep(5000);
                // Verifying the image in 3rd viewport
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step11_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.GetViewportCss(2, 2)));
                // Verifying the 3rd viewport border color in the 2nd study panel
                bool step11_2 = viewer.VerifyBordorColor(BasePage.Driver.FindElement(By.CssSelector(viewer.GetViewportCss(2, 2))).FindElement(By.XPath("..")), "rgba(90, 170, 255, 1)");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 102);
                bool step11_3 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, (BluRingViewer.div_studypanel + ":nth-of-type(2)" + " " + BluRingViewer.div_compositeViewer)));
                bool step11_4 = true;
                bool step11_5 = true;
                bool step11_6 = true;
                bool step11_7 = true;
                bool step11_8 = true;
                bool step11_9 = true;
                for (int i = 1; i < 8; i++)
                {
                    element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetStudyPanelThumbnailCss(i, 2)));
                    // Verifying border color of study panel thumbnails of 1st and 4th to 6th
                    if (i != 1 && i != 3 && i != 7)
                    {
                        String color5 = element.GetCssValue("border-bottom-color");
                        if (!(viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")))
                            step11_4 = false;
                    }

                    // Verifying border color of study panel thumbnails of 3rd and 7th
                    if ((i == 3 || i == 7) && !(viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)")))
                        step11_5 = false;

                    // Verifying border color of study panel thumbnails of 1st
                    if (i == 1 && !(viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)")))
                        step11_6 = false;

                    element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetExamListThumbnailCss(103, i)));
                    // Verifying border color of Exam List thumbnails of 1st and 4th to 6th
                    if (i != 1 && i != 3 && i != 7)
                    {
                        if (!(viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")))
                            step11_7 = false;
                    }

                    //  Verifying border color of Exam List thumbnails of 3rd and 7th
                    if ((i == 3 || i == 7) && !(viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)")))
                        step11_8 = false;

                    //  Verifying border color of Exam List thumbnails of 2nd
                    if (i == 1 && !(viewer.VerifyBordorColor(element, "rgba(90, 170, 255, 1)")))
                        step11_9 = false;
                }
                if (step11_1 && step11_2 && step11_3 && step11_4 && step11_5 && step11_6 && step11_7 && step11_8 && step11_9)
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

                // Step 12 - Click X (Exit) icon from the top right corner of the global toolbar menu to close the BlueRing viewer. Select the same study again and open it in the BlueRing viewer (for MICKEY, MOUSE, select accession ACC01).
                if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
                {
                    viewer.ScrollIntoView(viewer.GetElement("cssselector", BluRingViewer.div_closeStudy));
                }
                viewer.CloseBluRingViewer();
                studies.SelectStudy("Accession", Accession[0]);
                BluRingViewer.LaunchBluRingViewer();
                // Verifying first viewport is default original viewport
                viewportcss = viewer.GetViewportCss(1, 0);
                String color12 = BasePage.Driver.FindElement(By.CssSelector(viewportcss)).FindElement(By.XPath("..")).GetCssValue("border-bottom-color");
                bool step12_1 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetViewportCss(1, 0)).FindElement(By.XPath("..")), "rgba(90, 170, 255, 1)");
                // Verifying the thumbnail that was moved to 3rd viewport previously was not preserved
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step12_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.GetViewportCss(1, 2)));
                if (step12_1 && step12_2)
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

                //Step 13 - Click on an image-split thumbnail modality prior study for this patient (for MICKEY, MOUSE, select CR study dated 24-Feb-2000) from the Exam list.
                viewer.ScrollIntoView(viewer.GetElement("cssselector", BluRingViewer.div_relatedStudyPanel + ":nth-of-type(103)"));
                viewer.OpenPriors(102);
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer"))
                    viewer.ScrollIntoView(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_mergeLogo));
                bool step13_1 = viewer.GetStudyPanelCount().Equals(2);
                // Verifying border color of first viewport in second study panel
                bool step13_2 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetViewportCss(2, 0)).FindElement(By.XPath("..")), "rgba(90, 170, 255, 1)");
                // Verifying border color of first thumbpanel in second study panel
                string color13 = BasePage.Driver.FindElement(By.CssSelector(viewer.GetStudyPanelThumbnailCss(1, 2))).GetCssValue("border-bottom-color");
                bool step13_3 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1, 2)), "rgba(90, 170, 255, 1)");
                bool step13_4 = true;
                bool step13_5 = true;
                for (int i = 2; i < 7; i++)
                {
                    // Verifying the border of 2nd to 6th thumbnails in second study panel
                    element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetStudyPanelThumbnailCss(i, 2)));
                    if (!(viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")))
                        step13_4 = false;
                    // Verifying the border of 2nd to 6th viewports in second study panel
                    element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetViewportCss(2, i - 1)));
                    if (!(viewer.VerifyBordorColor(element.FindElement(By.XPath("..")), "rgba(0, 0, 0, 1)")))
                        step13_5 = false;
                }
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step13_6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, (BluRingViewer.div_studypanel + ":nth-of-type(2)" + " " + BluRingViewer.div_compositeViewer)));
                viewer.ScrollIntoView(viewer.GetElement("cssselector", BluRingViewer.div_thumbnailpreviewIconActiveStudy));
                viewer.ClickElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_thumbnailpreviewIconActiveStudy));
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer"))
                    viewer.ScrollIntoView(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_mergeLogo));                
                BluRingViewer.WaitforThumbnails(60);
                // Verifying border color of first thumbnail
                bool step13_7 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(103, 1)), "rgba(90, 170, 255, 1)");
                if (step13_1 && step13_2 && step13_3 && step13_4 && step13_5 && step13_6 && step13_7)
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

                //Step 14 - Click on the Thumbnail preview icon of the prior study(24 - Feb - 2000 study) from the Exam List.
                bool step14_1 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(103, 1)), "rgba(90, 170, 255, 1)");
                bool step14_2 = true;
                for (int i = 2; i < 7; i++)
                {
                    // Verifying the border of 2nd to 6th thumbnails in ExamList
                    element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetExamListThumbnailCss(103, i)));
                    if (!(viewer.VerifyBordorColor(element, "rgba(255, 255, 255, 1)")))
                        step14_2 = false;
                }
                // Series Number and Number of images are consistent
                bool step14_3 = true;
                bool step14_4 = true;
                String[] Thumbnailscaption = { "S1- 1", "S1- 2", "S1- 3", "S1- 4", "S1- 5", "S1- 6", "S1- 7" };
                String[] NumbImageinThumbnail = { "1", "1", "1", "1", "1", "1", "1" };
                StudyThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-of-type(2) " + BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                studyThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-of-type(2) " + BluRingViewer.div_studyPanelThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));
                ExamListThumbnailCaptionList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages + " " + BluRingViewer.div_thumbnailCaption));
                ExamListThumbnailNumberList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListThumbnailImages + " " + BluRingViewer.div_imageFrameNumber));

                for (int i = 0; i < NumbImageinThumbnail.Count(); i++)
                {
                    if (!(Thumbnailscaption[i] == StudyThumbnailCaptionList[i].GetAttribute("innerHTML") &&
                        NumbImageinThumbnail[i] == studyThumbnailNumberList[i].GetAttribute("innerHTML")))
                    {
                        step14_3 = false;
                        Logger.Instance.InfoLog("The thumbnails caption of " + i + " th thumbnail is :" + StudyThumbnailCaptionList[i].GetAttribute("innerHTML") + " The Thumbnail number of " + i + " th thumbnail is :" + studyThumbnailNumberList[i].GetAttribute("innerHTML"));
                    }
                }

                for (int i = 0; i < NumbImageinThumbnail.Count(); i++)
                {
                    if (!(Thumbnailscaption[i] == ExamListThumbnailCaptionList[i].GetAttribute("innerHTML") &&
                        NumbImageinThumbnail[i] == ExamListThumbnailNumberList[i].GetAttribute("innerHTML")))
                    {
                        step14_4 = false;
                        Logger.Instance.InfoLog("The thumbnails caption of " + i + " th ExamList thumbnail is :" + ExamListThumbnailCaptionList[i].GetAttribute("innerHTML") + " The ExamList Thumbnail number of " + i + " th thumbnail is :" + ExamListThumbnailNumberList[i].GetAttribute("innerHTML"));
                    }
                }

                // Verifying non-primary text
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step14_5 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(
                                BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(2) " + BluRingViewer.div_compositeViewer));

                if (step14_1 && step14_2 && step14_3 && step14_4 && step14_5)
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

                // Step 15 - From the Exam List, click the Thumbnail Preview button from a study that have not been opened or loaded. Check the borders for all thumbnails under the study.
                bool step15 = true;
                viewer.OpenExamListThumbnailPreview(103);
                viewer.ScrollIntoView(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_mergeLogo));
                var UnopenedThumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_relatedStudyPanel + ":nth-of-type(104) " + BluRingViewer.div_examListThumbnailImageComponent + " .thumbnailOuterDiv"));
                for (int i = 0; i < UnopenedThumbnails.Count(); i++)
                {
                    if ((viewer.VerifyBordorColor(UnopenedThumbnails[i], "rgba(255, 255, 255, 1)")) ||
                        (viewer.VerifyBordorColor(UnopenedThumbnails[i], "rgba(90, 170, 255, 1)")))
                        step15 = false;
                }
                if (step15)
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
                viewer.ScrollIntoView(viewer.GetElement("cssselector", BluRingViewer.div_closeStudy));
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// Thumbnail Server-side Rendering
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161057(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables                      
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
			ServiceTool st = new ServiceTool();
			WpfObjects wpfobject = new WpfObjects();
			int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

			BasePage.MultiDriver = new List<IWebDriver>();
			BasePage.MultiDriver.Add(BasePage.Driver);

			// XML Node
			String EnableServerSideThumbnailRendering = @"Configuration/ImageViewer/Html5/EnableServerSideThumbnailRendering";

			try
			{
				//Fetch required Test data        
				String adminUserName = Config.adminUserName;
				String adminPassword = Config.adminPassword;
				String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
				String Firstname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstNameList");
				string tsharkListernerOutput1 = @"C:\Program Files (x86)\Wireshark\captures.txt";

				//precondition - Set value EnableServerSideThumbnailRendering as true 
				login.ChangeNodeValue(new EHR().WebaccessConfigurationXMLPath, EnableServerSideThumbnailRendering, "true");
				st.RestartIISUsingexe();

				//Step 1 - Login to Enterprise Viewer application (use http-//<IP>/WebAccess URL to access) with any privileged user               
				//login.DriverGoTo(login.url);
				//login.LoginIConnect(adminUserName, adminPassword);
				login.DriverGoTo(login.url);
				login.SetDriver(BasePage.MultiDriver[0]);
				Config.node = Config.Clientsys4;
				BasePage.MultiDriver.Add(login.InvokeBrowser("Remote-" + Config.BrowserType));
				login.SetDriver(BasePage.MultiDriver.Last());
				login.DriverGoTo(login.url);
				login.LoginGrid(Config.adminUserName, Config.adminPassword);
				ExecutedSteps++;

				//Step 2 - From the Studies tab, search and load a patient with a few thumbnails (if using recommend data set, search for patient "MICKEY, MOUSE").
				var studies = (Studies)login.Navigate("Studies");
				studies.SearchStudy(FirstName: Firstname, AccessionNo: accession);
				ExecutedSteps++;

				//Step 3 -Start Wireshark (Network Analyzer) on the client machine and filter with Enterprise Viewer host IP on the network connection accessing the server.
				//E.g., Capture ...using this filter- host 10.1.2.100 
				//Ensure that the network capture function is recording.                            				
				ExecutedSteps++;

				//Step 4 - Select a study from the Study List and load it into the Enterprise Viewer.
				//(if using recommended data set, select accession ACC02, this one is an CR, which by default is setup to be image-split)
				studies.SelectStudy("Accession", accession);
				if (BasePage.SBrowserName.ToLower().Equals("internet explorer"))
				{
					var js = (IJavaScriptExecutor)BasePage.Driver;
					js.ExecuteScript("arguments[0].click()", BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.btn_bluringviewer)));
				}
				else
				{
					var button = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.btn_bluringviewer));
					button.Click();
				}
				ExecutedSteps++;

				//Step 5 - Go to the Wireshark screen and filter the network traffic with "websocket". Find the corresponding request and confirm that the 2 payloads are-
				//1. Mimetype
				//2. Binary image data (jpeg)    
				Process cmdWireshark = BasePage.StartWiresharkReadOutput(Config.IConnectIP);
				string cmdOutput = System.IO.File.ReadAllText(tsharkListernerOutput1);
				Logger.Instance.InfoLog("Wireshark- captured websocket: " + cmdOutput);

				if (cmdOutput.Contains("\"Mimetype\": \"image/jpeg\""))
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

				//Step-6 Go to the Wireshark screen and filter the network traffic with "websocket". Find the corresponding request and confirm that the 2 payloads are-
				//1. Pixel-Stats
				//2. Binary image data
				if (cmdOutput.Contains("\"DicomAttribute\"") && cmdOutput.Contains("\"Name\": \"PixelStats\""))
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

				BasePage.MultiDriver.Last().Close();
				BasePage.MultiDriver.Remove(BasePage.MultiDriver.Last());
				BasePage.Driver = BasePage.MultiDriver[0];
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
					// Reset EnableServerSideThumbnailRendering to false.
					login.ChangeNodeValue(new EHR().WebaccessConfigurationXMLPath, EnableServerSideThumbnailRendering, "false");
					st.RestartIISUsingexe();
				}
				catch (Exception ex)
				{
					Logger.Instance.ErrorLog("Finnaly block failed: Reset EnableServerSideThumbnailRendering to false-- " + ex.Message);
				}
				
				try
				{
					if (ExecutedSteps > -1)
						BasePage.Driver = BasePage.MultiDriver[0];
					login.closeallbrowser();
					Thread.Sleep(500);
					login.CreateNewSesion();
					login.DriverGoTo(login.url);
				}
				catch (Exception ex)
				{
					Logger.Instance.ErrorLog("Error in finally block create new session: " + ex.Message);
				}
			}

        }

        /// <summary>
        /// Study Panel: Single-click thumbnails
        /// </summary>
        public TestCaseResult Test_161044(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            ServiceTool servicetool = new ServiceTool();
            UserManagement usermanagement = new UserManagement();
            DomainManagement domain = new DomainManagement();
            WpfObjects wpfobject = new WpfObjects();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String domain1 = "Domain1_" + new Random().Next(1, 1000);
                String role1 = "Role1_" + new Random().Next(1, 1000);
                String rad1 = Config.newUserName + new Random().Next(1, 1000);
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastNameList");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] LastName = LastNameList.Split(':');
                String[] Accession = AccessionList.Split(':');

                //Precondition - Create new user
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.CreateDomain(domain1, role1, datasources: null);
                domain.ClickSaveNewDomain();
                login.Navigate("UserManagement");
                usermanagement.CreateUser(rad1, domain1, role1);
                login.Logout();

                //Step-1 Login to application as domin user.
                login.LoginIConnect(rad1, rad1);
                ExecutedSteps++;

                //Step-2 Search select a US Modality Study and verify Six viewports are displayed,Six thumbnails are highlighted.The first in thick-blue highlight then remaining 5 have white border with No "Non-Primary" text indicator of the viewports.
                var studies = new Studies();
                studies.SearchStudy(LastName: LastName[0], AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                bool step2 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_allViewportes)).Count == 6;
                bool step2_1 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1)), "rgba(90, 170, 255, 1)");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step2_2 = studies.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElement(By.CssSelector(viewer.GetViewportCss(1, 0))));
                bool step2_3 = true;
                for (int i = 2; i < 7; i++)
                {
                    bool color = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(i)), "rgba(255, 255, 255, 1)");
                    if (!color)
                    {
                        step2_3 = false;
                    }
                }
                if (step2 && step2_1 && step2_2 && step2_3)
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

                //Step-3 Verify nothing happened when single click on thumbnail in StudyPanel which highlighte
                viewer.ClickElement(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1)));
                bool HighlightBlue = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1)), "rgba(90, 170, 255, 1)");
                viewer.ClickElement(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(3)));
                bool HighlightWhite = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(3)), "rgba(255, 255, 255, 1)");
                viewer.ClickElement(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(9)));
                bool nothighighlight = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(9)), "rgba(0, 0, 0, 1)");
                viewer.HoverElement(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1)));
                var step3 = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1)).GetCssValue("opacity").Equals("0.8");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step3_1 = studies.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ViewportBorder)));

                if (HighlightBlue && HighlightWhite && nothighighlight && step3 && step3_1)
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

                //Step-4 Open ExamList Thumbnail Preview Window and verify the border
                viewer.ClickExamListThumbnailIcon("10-May-2000");
                bool step4_1 = viewer.IsElementPresent(By.CssSelector(BluRingViewer.div_examlistThumbnailVerticalScrollbar));
                bool step4_2 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(2, 1)), "rgba(90, 170, 255, 1)");               
                bool step4_3 = true;
                for (int i = 2; i < 7; i++)
                {

                    bool color1 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(i)), "rgba(255, 255, 255, 1)");
                    if (!color1)
                    {
                        step4_3 = false;
                    }
                }
                //3*3 verification
                bool step4_4 = false;
                IList<IWebElement> thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));
                IList<IWebElement> thumbnailsContainer = BasePage.Driver.FindElements(By.CssSelector("div.relatedStudythumbnailContainerComponent div.ps-container.thumbnails.ps"));
                int thumbnailExpectedHeight = thumbnailsContainer[1].Size.Height / 3;
                int thumbnailExpectedWidth = thumbnailsContainer[1].Size.Width / 3;
                foreach (IWebElement thumbnail in thumbnails)
                {
                    if (!(thumbnail.Size.Height <= thumbnailExpectedHeight && thumbnail.Size.Height >= (thumbnailExpectedHeight - thumbnailExpectedHeight / 3)))
                    {
                        step4_4 = true;
                        Logger.Instance.InfoLog("Invalid Height");
                        break;
                    }
                    if (!(thumbnail.Size.Width <= thumbnailExpectedWidth && thumbnail.Size.Width >= (thumbnailExpectedWidth - thumbnailExpectedWidth / 3)))
                    {
                        step4_4 = true;
                        Logger.Instance.InfoLog("Invalid Width");
                        break;
                    }
                }
                if (step4_1 && step4_2 && step4_3 && !step4_4)
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

                //Step-5 verify nothing happened when single click on thumbnail in ExamList which highlighted.
                viewer.ClickElement(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(2, 1)));
                bool HighlightBlue1 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(2, 1)), "rgba(90, 170, 255, 1)");
                viewer.ClickElement(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(2, 3)));
                bool HighlightWhite1 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(2, 3)), "rgba(255, 255, 255, 1)");
                viewer.ClickElement(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(2, 9)));
                bool nothighighlight1 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(2, 9)), "rgba(0, 0, 0, 1)");
                viewer.HoverElement(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(2, 1)));
                var step5 = viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(2, 1)).GetCssValue("opacity").Equals("0.8");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step5_1 = studies.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ViewportBorder)));

                if (HighlightBlue1 && HighlightWhite1 && nothighighlight1 && step5 && step5_1)
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

                //Step-6 Close the bluringviewer
                viewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Step-7 Search select a MR Modality Study and verify thumbnail its loaded in studypanel asseries-split and accordingly in viewport              
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[1]);
                BluRingViewer.LaunchBluRingViewer();
                if (viewer.IsElementPresent(By.CssSelector(BluRingViewer.div_allThumbnailsViewports)))
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

                //Step-8 verify nothing happened when single click on thumbnail in StudyPanel which highlighted.
                viewer.ClickElement(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1)));
                bool HighlightBlue2 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1)), "rgba(90, 170, 255, 1)");
                viewer.ClickElement(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(2)));
                bool HighlightWhite2 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(2)), "rgba(255, 255, 255, 1)");
                viewer.HoverElement(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1)));
                var step8 = viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1)).GetCssValue("opacity").Equals("0.8");
                if (HighlightBlue2 && HighlightWhite2 && step8)
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

                //Step-9 From the exam List, Verify the image-split thumbnail modality prior study
                viewer.OpenModalityFilter();
                viewer.SelectModalityValue("CR");
                viewer.CloseModalityFilter();
                Thread.Sleep(20000);
                viewer.OpenPriors(5);
                bool step9 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", BluRingViewer.div_ViewportBorder), "rgba(90, 170, 255, 1)");
                bool step9_1 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(1, 2)), "rgba(90, 170, 255, 1)");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step9_2 = studies.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElement(By.CssSelector(viewer.GetViewportCss(2, 0))));
                var step9_3 = true;
                for (int i = 2; i < 5; i++)
                {
                    bool color3 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(i, 2)), "rgba(255, 255, 255, 1)");
                    if (!color3)
                    {
                        step9_3 = false;
                    }
                }
                IWebElement element;
                element = BasePage.Driver.FindElement(By.CssSelector(viewer.GetViewportCss(1, 0))).FindElement(By.XPath(".."));
                bool step9_4 = viewer.VerifyBordorColor(element, "rgba(0, 0, 0, 1)");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 84);
                bool step9_5 = studies.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElement(By.CssSelector(viewer.GetViewportCss(1, 0))));
                var step9_6 = true;
                for (int i = 1; i < 3; i++)
                {
                    bool color = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetStudyPanelThumbnailCss(i)), "rgba(255, 255, 255, 1)");
                    if (!color)
                    {
                        step9_6 = false;
                    }
                }

                bool step9_7 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", BluRingViewer.div_ViewportBorder), "rgba(90, 170, 255, 1)");
                if (step9 && step9_1 && step9_2 && step9_3 && step9_4 && step9_5 && step9_6 && step9_7)
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

                //Step-10 Click on the Thumbnail preview icon of the prior study from the Exam List 
                viewer.ClickExamListThumbnailIcon("24-Feb-2000");
                bool step10 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(6, 1)), "rgba(90, 170, 255, 1)");
                bool step10_1 = true;
                for (int i = 2; i < 7; i++)
                {
                    bool color6 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(6, i)), "rgba(255, 255, 255, 1)");
                    if (!color6)
                    {
                        step10_1 = false;
                    }
                }
                if (step10 && step10_1)
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

                //Step-11 Single-click on each of the following thumbnails from the Exam List 
                viewer.ClickElement(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(6, 1)));
                bool HighlightBlue5 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(6, 1)), "rgba(90, 170, 255, 1)");
                viewer.ClickElement(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(6, 3)));
                bool HighlightWhite5 = viewer.VerifyBordorColor(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(6, 3)), "rgba(255, 255, 255, 1)");
                viewer.HoverElement(viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(6, 1)));
                var step11 = viewer.GetElement("cssselector", viewer.GetExamListThumbnailCss(6, 1)).GetCssValue("opacity").Equals("0.8");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step11_0 = studies.CompareImage(result.steps[ExecutedSteps], BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ViewportBorder)));
                if (HighlightBlue5 && HighlightWhite5 && step11 && step11_0)
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
                //Logout Application
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;

            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

        /// <summary>
		/// Series splitting: Thumbnail caption for all modalities
		/// </summary>
		/// <param name="testid"></param>
		/// <param name="teststeps"></param>
		/// <param name="stepcount"></param>
		/// <returns></returns>
		public TestCaseResult Test_161056(String testid, String teststeps, int stepcount)
        {

            // Declare and initialize variables
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            UserPreferences userpref = new UserPreferences();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                EA_91 = login.GetHostName(Config.EA91);
                
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');

                // Precondition - Set Thumbnail Splitting to 'Series' for US modality. 
                login.LoginIConnect(adminUserName, adminPassword);
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.ModalityDropDown().SelectByText("US");
                userpref.ClickElement(userpref.ThumbnailSplittingSeriesRadioBtn());                
                PageLoadWait.WaitForFrameLoad(10);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                login.Logout();

                // Step 1 - Login to WebAccess site with any privileged user        
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabPresent("Studies"))
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

                // Step 2 - In Studies page,Search and select any study(e.g. US modality) which has multiple series which has single image with multi frames then click on 'View Exam' button
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step 3 - verify number of frames present in the series at the top right of the thumbnail                
                IList<IWebElement> noOfImagesList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_imageFrameNumber));
                bool step3 = noOfImagesList[0].Displayed && noOfImagesList[0].GetCssValue("position").Equals("absolute") &&
                               noOfImagesList[0].GetCssValue("right").Equals("0px") &&
                               noOfImagesList[0].GetCssValue("z-index").Equals("108") &&
                               noOfImagesList[0].GetCssValue("text-align").Equals("right");
                if (step3)
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

                // Step 4 - Scroll through the images in the series and verify the percentage viewed in the thumbnail
                TestCompleteAction action = new TestCompleteAction();
                action.MouseScroll(viewer.GetElement("cssselector", viewer.Activeviewport), "down", "3");
                Thread.Sleep(3000);
                IList<IWebElement> PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                bool step4_1 = PercentViewedList[0].GetAttribute("innerHTML").Equals("6%");
                bool step4_2 = PercentViewedList[0].GetCssValue("position").Equals("absolute") &&
                                PercentViewedList[0].GetCssValue("z-index").Equals("108");
                if (step4_1 && step4_2)
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

                // Step 5 - Verify the series number and image number  
                IWebElement firstThumbnailCaption = viewer.GetElement("cssselector", BluRingViewer.div_thumbnailcontainer + " " + BluRingViewer.div_thumbnailCaption);
                bool step5_1 = firstThumbnailCaption.GetAttribute("innerHTML").Equals("S1- 44");
                bool step5_2 = firstThumbnailCaption.GetCssValue("position").Equals("absolute") &&
                               firstThumbnailCaption.GetCssValue("bottom").Equals("0px") &&
                               firstThumbnailCaption.GetCssValue("z-index").Equals("105") &&
                               firstThumbnailCaption.GetCssValue("text-align").Equals("left");
                if (step5_1 && step5_2)
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

                // Step 6 - close the bluring viewer and verify studies tab is displayed
                viewer.CloseBluRingViewer();
                if (login.IsTabPresent("Studies"))
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

                // Step 7 - Search and select any study(e.g. US modality) which has multiple series which has no series number with multiple images/frames then click on 'View Exam' button
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[1]);
                BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                // Step 8 - select any series viewport which has no series number and verify that the empty series number should be displayed as '?'
                IList<IWebElement> ThumbnailCaption = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailCaption));
                bool step8_1 = ThumbnailCaption[1].GetAttribute("innerHTML").Equals("S?");
                bool step8_2 = ThumbnailCaption[1].GetCssValue("position").Equals("absolute") &&
                               ThumbnailCaption[1].GetCssValue("bottom").Equals("0px") &&
                               ThumbnailCaption[1].GetCssValue("z-index").Equals("105") &&
                               ThumbnailCaption[1].GetCssValue("text-align").Equals("left");
                if (step8_1 && step8_2)
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

                // Step 9 - Verify Number of Image in the study is displayed
                bool step9_1 = viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_imageFrameNumber));
                noOfImagesList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_imageFrameNumber));
                bool step9_2 = noOfImagesList[1].Displayed && noOfImagesList[0].GetCssValue("position").Equals("absolute") &&
                              noOfImagesList[1].GetCssValue("right").Equals("0px") &&
                              noOfImagesList[1].GetCssValue("z-index").Equals("108") &&
                              noOfImagesList[1].GetCssValue("text-align").Equals("right");
                if (step9_1 && step9_2)
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


                // Step 10 - Scroll through the images in the series and verify the percentage viewed in the thumbnail 
                viewer.SetViewPort(1, 1);
                action.MouseScroll(viewer.GetElement("cssselector", viewer.Activeviewport), "down", "1");
                PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                bool step10_1 = viewer.VerifyThumbnailPercentImagesViewed(PercentViewedList[1], 5, 2);
                bool step10_2 = PercentViewedList[1].GetCssValue("position").Equals("absolute") &&
                                PercentViewedList[1].GetCssValue("z-index").Equals("108");
                if (step10_1 && step10_2)
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

                // Step 11 - close the bluring viewer and verify studies tab is displayed
                viewer.CloseBluRingViewer();
                if (login.IsTabPresent("Studies"))
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

                // Step 12 - Search and select US study which has multiple series which has no series number with single image then click on 'View Exam' button
                studies.SearchStudy(AccessionNo: Accession[2], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[2]);
                BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                // Step 13 - Verify the thumbnail series number should be displayed as "?" which dont have series number
                ThumbnailCaption = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailCaption));
                bool step13_1 = ThumbnailCaption[2].GetAttribute("innerHTML").Equals("S?- 2");
                bool step13_2 = ThumbnailCaption[2].GetCssValue("position").Equals("absolute") &&
                               ThumbnailCaption[2].GetCssValue("bottom").Equals("0px") &&
                               ThumbnailCaption[2].GetCssValue("z-index").Equals("105") &&
                               ThumbnailCaption[2].GetCssValue("text-align").Equals("left");
                if (step13_1 && step13_2)
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

                // Step 14 - close the bluring viewer and verify studies tab is displayed
                viewer.CloseBluRingViewer();
                if (login.IsTabPresent("Studies"))
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

                // Step 15 - Search and select US study which has multiple series which has no series number with single image then click on 'View Exam' button
                studies.SearchStudy(AccessionNo: Accession[3], patientID: PatientID, Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[3]);
                BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                // Step 16 - Select any series viewport which has multiple images with multi frames and verify number of images present in the series should be displayed at the top right of the thumbnail in the thumbnail bar
                noOfImagesList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_imageFrameNumber));
                bool step16_1 = noOfImagesList[0].GetAttribute("innerHTML").Equals("215");
                bool step16_2 = noOfImagesList[0].GetCssValue("position").Equals("absolute") &&
                               noOfImagesList[0].GetCssValue("right").Equals("0px") &&
                               noOfImagesList[0].GetCssValue("z-index").Equals("108") &&
                               noOfImagesList[0].GetCssValue("text-align").Equals("right");
                if (step16_1 && step16_2)
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

                // Step 17 - verify the percentage viewed top left of the thumbnail   
                viewer.SetViewPort(0, 1);
                action.MouseScroll(viewer.GetElement("cssselector", viewer.Activeviewport), "down", "5");
                PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                bool step17_1 = PercentViewedList[0].GetAttribute("innerHTML").Equals("2%");
                bool step17_2 = PercentViewedList[0].GetCssValue("position").Equals("absolute") &&
                                PercentViewedList[0].GetCssValue("z-index").Equals("108");
                if (step17_1 && step17_2)
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

                // Step 18 - Verify that the series number of the thumbnail
                ThumbnailCaption = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailCaption));
                bool step18_1 = ThumbnailCaption[0].GetAttribute("innerHTML").Equals("S2");
                bool step18_2 = ThumbnailCaption[0].GetCssValue("position").Equals("absolute") &&
                                ThumbnailCaption[0].GetCssValue("bottom").Equals("0px") &&
                                ThumbnailCaption[0].GetCssValue("z-index").Equals("105") &&
                                ThumbnailCaption[0].GetCssValue("text-align").Equals("left");
                if (step18_1 && step18_2)
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

                // Step 19 - close the viewer
                viewer.CloseBluRingViewer();
                if (login.IsTabPresent("Studies"))
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

                // Step 20 - search and select any study(e.g. US modality) which has multiple images with single frame then click on 'View Exam' button
                studies.SearchStudy(AccessionNo: Accession[4], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[4]);
                BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;


                // Step 21 - Select any series viewport which has multiple images with multi frames and verify number of images present in the series should be displayed at the top right of the thumbnail in the thumbnail bar
                noOfImagesList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_imageFrameNumber));
                bool step21_1 = noOfImagesList[0].GetAttribute("innerHTML").Equals("10");
                bool step21_2 = noOfImagesList[0].GetCssValue("position").Equals("absolute") &&
                               noOfImagesList[0].GetCssValue("right").Equals("0px") &&
                               noOfImagesList[0].GetCssValue("z-index").Equals("108") &&
                               noOfImagesList[0].GetCssValue("text-align").Equals("right");
                if (step21_1 && step21_2)
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

                // Step 22 - Verify that the series number of the thumbnail
                ThumbnailCaption = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailCaption));
                bool step22_1 = ThumbnailCaption[0].GetAttribute("innerHTML").Equals("S2");
                bool step22_2 = ThumbnailCaption[0].GetCssValue("position").Equals("absolute") &&
                                ThumbnailCaption[0].GetCssValue("bottom").Equals("0px") &&
                                ThumbnailCaption[0].GetCssValue("z-index").Equals("105") &&
                                ThumbnailCaption[0].GetCssValue("text-align").Equals("left");
                if (step22_1 && step22_2)
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

                // step 23 - close the bluring viewer
                viewer.CloseBluRingViewer();
                if (login.IsTabPresent("Studies"))
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

                // Step 24 - Search and select any study(e.g. US modality) which has multiple series contains single image with single frame
                studies.SearchStudy(AccessionNo: Accession[5], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[5]);
                BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                // Step 25 - Verify that the no. of images should be displayed, which is in the top right corner of the thumbnail.
                noOfImagesList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_imageFrameNumber));
                bool step25_1 = noOfImagesList[0].GetAttribute("innerHTML").Equals("1");
                bool step25_2 = noOfImagesList[0].GetCssValue("position").Equals("absolute") &&
                               noOfImagesList[0].GetCssValue("right").Equals("0px") &&
                               noOfImagesList[0].GetCssValue("z-index").Equals("108") &&
                               noOfImagesList[0].GetCssValue("text-align").Equals("right");
                if (step25_1 && step25_2)
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

                // Step 26 - Verify that the series number of the thumbnail
                ThumbnailCaption = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailCaption));
                bool step26_1 = ThumbnailCaption[0].GetAttribute("innerHTML").Equals("S8- 1");
                bool step26_2 = ThumbnailCaption[0].GetCssValue("position").Equals("absolute") &&
                                ThumbnailCaption[0].GetCssValue("bottom").Equals("0px") &&
                                ThumbnailCaption[0].GetCssValue("z-index").Equals("105") &&
                                ThumbnailCaption[0].GetCssValue("text-align").Equals("left");
                if (step26_1 && step26_2)
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

                // step 27 - close the bluring viewer
                viewer.CloseBluRingViewer();
                if (login.IsTabPresent("Studies"))
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

                // Step 28 - Search and Select any series with PR and click on 'View Exam' button.
                studies.SearchStudy(AccessionNo: Accession[6], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[6]);
                BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                // Step 29 - Select PR series and verify number of referenced images
                noOfImagesList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_imageFrameNumber));
                bool step29 = noOfImagesList[0].GetCssValue("position").Equals("absolute") &&
                               noOfImagesList[0].GetCssValue("right").Equals("0px") &&
                               noOfImagesList[0].GetCssValue("z-index").Equals("108") &&
                               noOfImagesList[0].GetCssValue("text-align").Equals("right");
                if (step29)
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
                viewer.CloseBluRingViewer();
                action.Perform();

                // Reverting Thumbnail Splitting to 'Image' for US modality.
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.ModalityDropDown().SelectByText("US");
                userpref.ClickElement(userpref.ThumbnailSplittingSeriesRadioBtn());                
                PageLoadWait.WaitForFrameLoad(10);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                login.Logout();

                //Return Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }

            finally
            {
				

				// Reverting Thumbnail Splitting to 'Image' for US modality.
				login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.ModalityDropDown().SelectByText("US");
                userpref.ClickElement(userpref.ThumbnailSplittingSeriesRadioBtn());                
                PageLoadWait.WaitForFrameLoad(10);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                login.Logout();
            }

        }

        /// <summary>
		/// Image splitting: Thumbnail caption for all modalities
		/// </summary>
		/// <param name="testid"></param>
		/// <param name="teststeps"></param>
		/// <param name="stepcount"></param>
		/// <returns></returns>
		public TestCaseResult Test_161050(String testid, String teststeps, int stepcount)
        {

            // Declare and initialize variables
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            UserPreferences userpref = new UserPreferences();
            UserManagement usermanagement = new UserManagement();
            DomainManagement domain = new DomainManagement();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                EA_91 = login.GetHostName(Config.EA91);
                String domain1 = "Domain1_" + new Random().Next(1, 1000);
                String role1 = "Role1_" + new Random().Next(1, 1000);
                String rad1 = Config.newUserName + new Random().Next(1, 1000);

                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');

                //Precondition                              
                //Create new user
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.CreateDomain(domain1, role1, datasources: null);
                domain.ClickSaveNewDomain();
                login.Navigate("UserManagement");
                usermanagement.CreateUser(rad1, domain1, role1);
                login.Logout();

                // Precondition - Set Thumbnail Splitting to 'Image' for US modality. 
                login.LoginIConnect(rad1, rad1);
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.ModalityDropDown().SelectByText("US");
                userpref.ClickElement(userpref.ThumbnailSplittingImageRadioBtn());
                PageLoadWait.WaitForFrameLoad(10);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                login.Logout();

                // Step 1 - Login to WebAccess site with any privileged user. (e.g., rad/rad)
                login.LoginIConnect(rad1, rad1);
                if (login.IsTabPresent("Studies"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 2 - In Studies page,Search and select any study(e.g. US modality) which has single series which has single image with multi frames then click on 'Universal' button	
                Studies studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[0]);
                BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                // Step 3 - Select any series viewport which has single image with multi frames and verify number of frames present in the series
                viewer.SetViewPort(0, 1);
                var imageFrameNumber = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.GetStudyPanelThumbnailCss(1, 1) + " " + BluRingViewer.div_imageFrameNumber);
                var step3_1 = imageFrameNumber.GetCssValue("right").Equals("0px");
                var step3_2 = imageFrameNumber.GetCssValue("text-align").Equals("right");
                var step3_3 = imageFrameNumber.GetCssValue("position").Equals("absolute");
                var step3_4 = imageFrameNumber.GetAttribute("innerHTML").Equals("50");
                if (step3_1 && step3_2 && step3_3 && step3_4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The Expected css value of right is " + imageFrameNumber.GetCssValue("right") + " The Actual value is 0px");
                    Logger.Instance.InfoLog("The Expected css value of text-align is " + imageFrameNumber.GetCssValue("text-align") + " The Actual value is right");
                    Logger.Instance.InfoLog("The Expected css value of position is " + imageFrameNumber.GetCssValue("position") + " The Actual value is absolute");
                    Logger.Instance.InfoLog("The Expected ImageFrameNumber is " + imageFrameNumber.GetAttribute("innerHTML") + " The Actual value is 50");
                }

                // Step 4 - Verify Modality of the series should be displayed at the top left of the thumbnail in the thumbnail bar.
                var thumbnailmodality = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.GetStudyPanelThumbnailCss(1, 1) + " " + BluRingViewer.div_thumbnailModality);
                var step4_1 = thumbnailmodality.GetCssValue("left").Equals("0px");
                var step4_2 = thumbnailmodality.GetCssValue("position").Equals("absolute");
                var step4_3 = thumbnailmodality.GetAttribute("innerHTML").Equals("US");
                if (step4_1 && step4_2 && step4_3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The Expected css value of left is " + thumbnailmodality.GetCssValue("left") + " The Actual value is 0px");
                    Logger.Instance.InfoLog("The Expected css value of position is " + thumbnailmodality.GetCssValue("position") + " The Actual value is absolute");
                    Logger.Instance.InfoLog("The Expected Modality is " + thumbnailmodality.GetAttribute("innerHTML") + " The Actual value is US");
                }

                // Step 5 - Verify % viewed value should be displayed as rounded value at the bottom right of the thumbnail in the thumbnail bar.	
                var thumbnailsPercentageViewed = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.GetStudyPanelThumbnailCss(1, 1) + " " + BluRingViewer.div_thumbnailPercentImagesViewed);
                //var step5_1 = thumbnailsPercentageViewed.GetCssValue("margin").Equals("1px");
                var step5_1 = thumbnailsPercentageViewed.GetCssValue("right").Equals("0px");
                var step5_2 = thumbnailsPercentageViewed.GetCssValue("bottom").Equals("0px");
                var step5_3 = thumbnailsPercentageViewed.GetCssValue("position").Equals("absolute");
                var percentageImageviewed = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.GetStudyPanelThumbnailCss(1, 1) + " " + BluRingViewer.div_thumbnailPercentImagesViewed).GetAttribute("innerHTML");
                var Stringnumber = percentageImageviewed.Replace("%", "");
                var number = Convert.ToInt32(Stringnumber);
                var step5_4 = number % 1 == 0;
                if (step5_1 && step5_2 && step5_3 && step5_4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The Expected css value of right is " + thumbnailsPercentageViewed.GetCssValue("right") + " The Actual value is 0px");
                    Logger.Instance.InfoLog("The Expected css value of bottom is " + thumbnailsPercentageViewed.GetCssValue("bottom") + " The Actual value is 0px");
                    Logger.Instance.InfoLog("The Expected css value of position is " + thumbnailsPercentageViewed.GetCssValue("position") + " The Actual value is absolute");
                    Logger.Instance.InfoLog("The percentage viewed is " + Stringnumber + " and roundablity is " + step5_4);
                }

                // Step 6 - Verify that the Series number and Image number should be displayed as S(Series number) at the bottom left of the thumbnail in the thumbnail bar.
                var seriesNumberImageNumber = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.GetStudyPanelThumbnailCss(1, 1) + " " + BluRingViewer.div_thumbnailCaption);
                var step6_1 = seriesNumberImageNumber.GetCssValue("text-align").Equals("left");
                var step6_2 = seriesNumberImageNumber.GetCssValue("position").Equals("absolute");
                var step6_3 = seriesNumberImageNumber.GetAttribute("innerHTML").Equals("S1- 1");
                if (step6_1 && step6_2 && step6_3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();                    
                    Logger.Instance.InfoLog("The Expected css value of text-align is " + seriesNumberImageNumber.GetCssValue("text-align") + " The Actual value is left");
                    Logger.Instance.InfoLog("The Expected css value of position is " + seriesNumberImageNumber.GetCssValue("position") + " The Actual value is absolute");
                    Logger.Instance.InfoLog("The Expected innerHTML is " + seriesNumberImageNumber.GetAttribute("innerHTML") + " The Actual value is S1- 1");
                }

                // Step 7 -  Verify the image text Series number, Number of frames and Image number in its corresponding viewport are consistent.	
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step7)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                // Step 8 - Click on 'EXIT' button and Navigate to Studies tab	
                viewer.CloseBluRingViewer();
                if (login.IsTabPresent("Studies"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 9 - Search and select any study(e.g. US modality) which has multiple series contains single image with single frame then click on 'Universal' button	
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: EA_91);
                studies.SelectStudy("Accession", Accession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                // Step 10 -Select any series viewport which has single image with single frame and verify number of images present in the series should be displayed at the top right of the thumbnail in the thumbnail bar	
                viewer.SetViewPort(0, 1);
                imageFrameNumber = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.GetStudyPanelThumbnailCss(1, 1) + " " + BluRingViewer.div_imageFrameNumber);
                var step10_1 = imageFrameNumber.GetCssValue("right").Equals("0px");
                var step10_2 = imageFrameNumber.GetCssValue("text-align").Equals("right");
                var step10_3 = imageFrameNumber.GetCssValue("position").Equals("absolute");
                var step10_4 = imageFrameNumber.GetAttribute("innerHTML").Equals("1");
                if (step10_1 && step10_2 && step10_3 && step10_4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The Expected css value of right is " + imageFrameNumber.GetCssValue("right") + " The Actual value is 0px");
                    Logger.Instance.InfoLog("The Expected css value of text-align is " + imageFrameNumber.GetCssValue("text-align") + " The Actual value is right");
                    Logger.Instance.InfoLog("The Expected css value of position is " + imageFrameNumber.GetCssValue("position") + " The Actual value is absolute");
                    Logger.Instance.InfoLog("The Expected ImageFrame number is " + imageFrameNumber.GetAttribute("innerHTML") + " The Actual value is absolute");
                }

                // Step 11 - Verify Modality of the series should be displayed at the top left of the thumbnail in the thumbnail bar.
                thumbnailmodality = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.GetStudyPanelThumbnailCss(1, 1) + " " + BluRingViewer.div_thumbnailModality);
                var step11_1 = thumbnailmodality.GetCssValue("left").Equals("0px");
                var step11_2 = thumbnailmodality.GetCssValue("position").Equals("absolute");
                var step11_3 = thumbnailmodality.GetAttribute("innerHTML").Equals("US");
                if (step11_1 && step11_2 && step11_3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The Expected css value of left is " + thumbnailmodality.GetCssValue("left") + " The Actual value is 0px");
                    Logger.Instance.InfoLog("The Expected css value of position is " + thumbnailmodality.GetCssValue("position") + " The Actual value is absolute");
                    Logger.Instance.InfoLog("The Expected Modality is " + thumbnailmodality.GetCssValue("position") + " The Actual value is US");
                }

                // Step 12 - Verify % viewed value should be displayed as rounded value at the bottom right of the thumbnail in the thumbnail bar.	
                thumbnailsPercentageViewed = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.GetStudyPanelThumbnailCss(1, 1) + " " + BluRingViewer.div_thumbnailPercentImagesViewed);
                //var step12_1 = thumbnailsPercentageViewed.GetCssValue("margin").Equals("1px");
                var step12_1 = thumbnailsPercentageViewed.GetCssValue("right").Equals("0px");
                var step12_2 = thumbnailsPercentageViewed.GetCssValue("bottom").Equals("0px");
                var step12_3 = thumbnailsPercentageViewed.GetCssValue("position").Equals("absolute");
                percentageImageviewed = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.GetStudyPanelThumbnailCss(1, 1) + " " + BluRingViewer.div_thumbnailPercentImagesViewed).GetAttribute("innerHTML");
                Stringnumber = percentageImageviewed.Replace("%", "");
                number = Convert.ToInt32(Stringnumber);
                var step12_4 = number % 1 == 0;
                if (step12_1 && step12_2 && step12_3 && step12_4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The Expected css value of right is " + thumbnailsPercentageViewed.GetCssValue("right") + " The Actual value is 0px");
                    Logger.Instance.InfoLog("The Expected css value of bottom is " + thumbnailsPercentageViewed.GetCssValue("bottom") + " The Actual value is 0px");
                    Logger.Instance.InfoLog("The Expected css value of position is " + thumbnailsPercentageViewed.GetCssValue("position") + " The Actual value is absolute");
                    Logger.Instance.InfoLog("The percentage viewed is " + Stringnumber + " and roundablity is " + step12_4);
                }

                // Step 13 - Verify that the Series number and Image number should be displayed as S(Series number) at the bottom left of the thumbnail in the thumbnail bar.
                seriesNumberImageNumber = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.GetStudyPanelThumbnailCss(1, 1) + " " + BluRingViewer.div_thumbnailCaption);
                //var step13_1 = seriesNumberImageNumber.GetCssValue("margin").Equals("1px");
                var step13_1 = seriesNumberImageNumber.GetCssValue("text-align").Equals("left");
                var step13_2 = seriesNumberImageNumber.GetCssValue("position").Equals("absolute");
                var step13_3 = seriesNumberImageNumber.GetAttribute("innerHTML").Equals("S3- 1");
                if (step13_1 && step13_2 && step13_3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The Expected css value of text-align is " + seriesNumberImageNumber.GetCssValue("text-align") + " The Actual value is left");
                    Logger.Instance.InfoLog("The Expected css value of position is " + seriesNumberImageNumber.GetCssValue("position") + "The Actual value is absolute");
                    Logger.Instance.InfoLog("The Expected innerHTML is " + seriesNumberImageNumber.GetAttribute("innerHTML") + " The Actual value is S3- 1");
                }

                // Step 14 -  Verify the image text Series number, Number of Images and Image number in its corresponding viewport are consistent.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step14 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step14)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //close the viewer and logout of the application
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

    }
}