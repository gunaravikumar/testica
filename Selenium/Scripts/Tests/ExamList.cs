using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using System.Globalization;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MergeServiceTool;
using Dicom;
using Dicom.Network;
using System.Windows.Forms;

namespace Selenium.Scripts.Tests
{
    class ExamList : BasePage
    {

        public Login login { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public HPLogin hplogin { get; set; }
        public Configure configure { get; set; }
        public HPHomePage hphomepage { get; set; }
        public ExamImporter ei { get; set; }
        public string filepath { get; set; }
        WpfObjects wpfobject;
        public BluRingViewer bluringviewer { get; set; }
        public ServiceTool servicetool { get; set; }

        String BluringViewer_MappingFilePath = Config.BluringViewer_Mappingfilepath;

        public ExamList(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            configure = new Configure();
            hphomepage = new HPHomePage();
            ei = new ExamImporter();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }


        /// <summary>
        /// Exam List: Filter:  Modality and Sort By
        /// </summary>
        public TestCaseResult Test_161617(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String patientID = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID"));
                String name = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name"));
                String dob = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOB"));
                String gender = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "Gender"));
                String ipid = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPID"));
                String studyDate = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorDate"));
                String modality = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorModality"));
                String studydesc = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDesc"));
                String datasource = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList"));

                //Step 1 - Login to WebAccess site with any privileged user.
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                result.steps[++ExecutedSteps].StepPass();

                //Step 2 - 1.Navigate to Study tab - Search for a patient which has many related studies that contains many series and images. 
                studies.SearchStudy(AccessionNo: accession, Datasource: datasource);
                studies.SelectStudy("Accession", accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].StepPass();

                //Step 3 - Verify that the patient related studies and the current primary study (in the Study Panel) are displayed in the Results list as cards with study details.
                IList<IWebElement> prior_3 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                int expPriorCount = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount"));

                if (bluringviewer.IsAllPriorsDisplayed() && prior_3.Count == expPriorCount)
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

                //Step 4 - Verify that the patient related studies in the Exam List is sorted based on the default Sort By option Date -Newest.
                if (bluringviewer.IsStudiesSortedByDate())
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

                //Step 5 - From the Exam List, click on the Sort By drop-down and select "Date - Oldest". Verify the results list.
                bluringviewer.OpenSortDorpdown();
                bluringviewer.SelectValue_SortDropdown("Date - Oldest");

                if (bluringviewer.IsStudiesSortedByDate(true))
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

                //Step 6 - From the Exam List, click on the Sort By drop-down and select Modality Type. Verify the results list is alphabetically ordered based on list 
                //         of modalities in the Exam list cards. (Note that the modalities on the Exam card lists primary modality before non-primary modality. e.g., CR, CT, MR before KO, PR).
                bluringviewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: accession, Datasource: datasource);
                studies.SelectStudy("Accession", accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                bluringviewer.OpenSortDorpdown();
                bluringviewer.SelectValue_SortDropdown("Modality Type");

                IList<IWebElement> prior_6 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                bool isModalitySorted6 = true;
                String expectedModalityString6 = (String)ReadExcel.GetTestData(filepath, "TestData", testid + "_6", "PriorModality");
                int expPriorCount6 = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_6", "PriorCount"));
                String[] expectedModalityList6 = expectedModalityString6.Split(':');

                //Determine if the modalities are displayed and sorted.
                int priorCount = 0;
                IList<String> modalitiesRetrieved6 = new List<String>();

                while (priorCount <= prior_6.Count - 1)
                {
                    String currentModalityFound;
                    var priors = BasePage.FindElementsByCss(BluRingViewer.div_priors);
                    var container = BasePage.FindElementByCss(BluRingViewer.div_ContainerPriors);
                    if (this.IsInBrowserViewport(priors[priorCount]) == false)
                    {
                        new TestCompleteAction().MouseScroll(container, "down", "4").Perform();
                        if (this.IsInBrowserViewport(priors[priorCount]) == true)
                        {
                            currentModalityFound = priors[priorCount].Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[1];
                            modalitiesRetrieved6.Add(currentModalityFound);

                            priorCount++;
                            continue;
                        }
                        else
                        {
                            Logger.Instance.ErrorLog((priorCount + 1) + "the Prior Not Displayed");
                            break;
                        }
                    }
                    else
                    {
                        currentModalityFound = priors[priorCount].Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[1];
                        modalitiesRetrieved6.Add(currentModalityFound);

                        priorCount++;
                        continue;
                    }
                }

                for (int index = 0; index < prior_6.Count; index++)
                {
                    if (!modalitiesRetrieved6[index].Equals(expectedModalityList6[index]))
                    {
                        isModalitySorted6 = false;
                    }
                }

                if (isModalitySorted6 && prior_6.Count == expPriorCount6)
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

                //Sep 7 - From the Exam List, click on the Modality drop-down list and select one modality from the list(e.g, CT)
                //Selcet CT for modality
                bluringviewer.OpenModalityFilter();
                bluringviewer.SelectModalityValue("CT");
                bluringviewer.CloseModalityFilter();

                IList<IWebElement> prior_7 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                String expectedExamCardCount7 = (String)ReadExcel.GetTestData(filepath, "TestData", testid + "_7", "ExamCardCount");
                String outOfTextRetrieved7 = Driver.FindElement(By.CssSelector(BluRingViewer.examCardStudyCountText)).GetAttribute("innerText");
                String expectedModality7 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_7", "PriorModality"));

                String moalityRetrieved7 = prior_7[0].Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[1];

                if (outOfTextRetrieved7.Trim().Equals(expectedExamCardCount7.Trim())
                    && moalityRetrieved7.Equals(expectedModality7)
                    && prior_7.Count == 1)
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

                //Step 8 - From the Exam List, click on the Modality drop-down list and select a few modalities from the list (e.g, CT, MR, PR)	
                bluringviewer.OpenModalityFilter();
                bluringviewer.SelectModalityValue("CT");
                bluringviewer.SelectModalityValue("DX");
                bluringviewer.CloseModalityFilter();

                IList<IWebElement> prior_8 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                String expectedExamCardCount8 = (String)ReadExcel.GetTestData(filepath, "TestData", testid + "_8", "ExamCardCount");
                String outOfTextRetrieved8 = Driver.FindElement(By.CssSelector(BluRingViewer.examCardStudyCountText)).GetAttribute("innerText");
                String expectedModalityString8 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_8", "PriorModality"));
                String[] expectedModalityList8 = expectedModalityString8.Split(':');

                bool isModalitySorted8 = true;

                IList<String> modalitiesRetrieved8 = new List<String>();
                modalitiesRetrieved8.Add(prior_8[0].Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[1]);
                modalitiesRetrieved8.Add(prior_8[1].Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[1]);
                modalitiesRetrieved8.Add(prior_8[2].Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[1]);

                for (int index = 0; index < prior_8.Count; index++)
                {
                    if (!modalitiesRetrieved8[index].Equals(expectedModalityList8[index]))
                    {
                        isModalitySorted8 = false;
                    }
                }

                if (isModalitySorted8
                    && outOfTextRetrieved8.Trim().Equals(expectedExamCardCount8.Trim())
                    && prior_8.Count == 3)
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

                //Step 9 - From the Exam List, click on the Modality drop-down list, click Clear All.
                bluringviewer.OpenModalityFilter();
                GetElement(SelectorType.CssSelector, BluRingViewer.Modality_Clear_All).Click();
                bluringviewer.CloseModalityFilter();

                IList<IWebElement> prior_9 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                String expectedExamCardCount9 = (String)ReadExcel.GetTestData(filepath, "TestData", testid + "_9", "ExamCardCount");
                String outOfTextRetrieved9 = Driver.FindElement(By.CssSelector(BluRingViewer.examCardStudyCountText)).GetAttribute("innerText");

                if (prior_9.Count == 0
                    && outOfTextRetrieved9.Trim().Equals(expectedExamCardCount9.Trim()))
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

                //Step 10 - From the Exam List, click on the Modality drop-down list, select All.	
                bluringviewer.OpenModalityFilter();
                bluringviewer.SelectModalityValue("All");
                bluringviewer.CloseModalityFilter();

                String expectedExamCardCount10 = (String)ReadExcel.GetTestData(filepath, "TestData", testid + "_10", "ExamCardCount");
                String outOfTextRetrieved10 = Driver.FindElement(By.CssSelector(BluRingViewer.examCardStudyCountText)).GetAttribute("innerText");
                int expPriorCount10 = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_10", "PriorCount"));
                IList<IWebElement> prior_10 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));

                if (bluringviewer.IsAllPriorsDisplayed()
                    && outOfTextRetrieved10.Trim().Equals(expectedExamCardCount10.Trim())
                    && prior_10.Count == expPriorCount10)
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

                //Step 11 - From the modality - filtered Exam List, click on the Thumbnail Preview button of one of the studies in the list. 
                GetElement(SelectorType.CssSelector, BluRingViewer.div_thumbnailpreviewIconActiveStudy).Click();
                result.steps[++ExecutedSteps].StepPass();

                //Step 12 - From the modality - filtered Exam List, click on one of the study card of the studies in the list.
                //          Verify that clicking on a study card opens the study in a new Study Panel,
                //          for a filtered results list.
                int intialCount = bluringviewer.GetStudyPanelCount();
                GetElement(SelectorType.CssSelector, BluRingViewer.div_activeExamPanel).Click();
                PageLoadWait.WaitForFrameLoad(20);

                if (bluringviewer.GetStudyPanelCount() == intialCount + 1)
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

                //Logout Application
                bluringviewer.CloseBluRingViewer();
                login.Logout();

                // Result
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
        ///Verify the 'Back to List' icon shall close the viewer window to take user back to Study List page/window
        /// </summary>
        public TestCaseResult Test_161619(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                int expPriorCount = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount"));
                String modality6 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorModality");
                String institution10 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorSite");
                BluRingViewer viewer = null;

                //Step 1, 2 - Launch and Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;
                ExecutedSteps++;

                //Step 3 - Navigate to Search                
                var studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step 4 - Load a study with multiple series that contain multiple images.Ensure the scope is set to Image.                            
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy("Accession", accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step-5 - Click Back to List
                viewer.CloseBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                if (viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.btn_bluringviewer) != null)
                {
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
        ///Verify the Exam list shall close by clicking "X" icon on the top right corner of the "Exam List" panel
        /// </summary>
        public TestCaseResult Test_161620(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                int expPriorCount = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount"));
                String modality6 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorModality");
                String institution10 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorSite");
                BluRingViewer viewer = null;

                //Step 1, 2 - Launch and Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;
                ExecutedSteps++;

                //Step 3 - Load a study with multiple series that contain multiple images.Ensure the scope is set to Image.
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy("Accession", accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step-4 - ExamList Open by default
                var priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                var priorcount = priors.Count;
                bool isPriorDisplayed = false;
                foreach (IWebElement prior in priors)
                {
                    if (prior.Displayed) { isPriorDisplayed = true; }
                    else { isPriorDisplayed = false; break; }
                }
                if (isPriorDisplayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5
                if (priorcount == expPriorCount)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6 - Close X icon and close Exam List
                var closebtn = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_closeExamList));
                new TestCompleteAction().MoveToElement(closebtn).Perform();
                var step6_1 = closebtn.GetAttribute("title").Equals("Close");
                var step6 = viewer.CloseExamList();
                if (step6 && step6_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-7 - Open Exam List Panel
                var step7 = viewer.OpenExamList();
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

                //Step-8 - Open prior
                viewer.OpenPriors(1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 2);
                var viewport = viewer.Activeviewport;
                var step8 = viewer.CompareImage(result.steps[ExecutedSteps],
                    viewer.GetElement(BasePage.SelectorType.CssSelector, viewport));
                if (step8)
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

                //step-9 - Close Exam List
                var step9 = viewer.CloseExamList();
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
        ///Without animation Effect in Exam List: Verify the Exam list shall show/hide instantly without any animation effect when user click on the EXAMS icon in the Global Toolbar
        /// </summary>
        public TestCaseResult Test_161624(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                int expPriorCount = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount"));
                BluRingViewer viewer = null;

                //Step 1, 2 - Launch and Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;
                ExecutedSteps++;

                //Step 3 - Select Studies Tab
                var studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step-4 - Load the study in viewer
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy("Accession", accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step-5 - ExamList Open by default
                var priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                var priorcount = priors.Count;
                bool isPriorDisplayed = false;
                foreach (IWebElement prior in priors)
                {
                    if (prior.Displayed) { isPriorDisplayed = true; }
                    else { isPriorDisplayed = false; break; }
                }
                if (isPriorDisplayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6
                if (priorcount == expPriorCount)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7 - Click on Exam in Global Toolbar section                
                var isPiorDisplayed1 = viewer.OpenExamList();
                priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                isPriorDisplayed = false;
                foreach (IWebElement prior in priors)
                {
                    if (prior.Displayed) { isPriorDisplayed = true; }
                    else { isPriorDisplayed = false; break; }
                }
                if (!isPriorDisplayed && !isPiorDisplayed1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8 - Verify the size of the view port is increased.              
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                var viewport = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[0];
                var step8 = viewer.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step8)
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

                //step-9 - Click on Exam List and esnure Exam List opens
                viewer.OpenExamList();
                priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                isPriorDisplayed = false;
                foreach (IWebElement prior in priors)
                {
                    if (prior.Displayed) { isPriorDisplayed = true; }
                    else { isPriorDisplayed = false; break; }
                }
                if (isPriorDisplayed)
                {
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
        /// Verify when the user double clicks or single click on the patient study in Exam list only one study panel shall get open
        /// </summary>
        public TestCaseResult Test_161626(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                int expPriorCount = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount"));
                BluRingViewer viewer = null;

                //Step 1, 2 - Launch and Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;
                ExecutedSteps++;

                //Step 3 - Load the study in viewer
                var studies = (Studies)login.Navigate("Studies");                
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy("Accession", accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step-4 - ExamList Open by default
                var priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                var priorcount = priors.Count;
                bool isPriorDisplayed = false;
                foreach (IWebElement prior in priors)
                {
                    if (prior.Displayed) { isPriorDisplayed = true; }
                    else { isPriorDisplayed = false; break; }
                }
                if (isPriorDisplayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5
                if(viewer.CloseExamList())
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6
                if (viewer.OpenExamList())
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-7
                int panelcount6 = viewer.GetStudyPanelCount();
                if (priorcount == expPriorCount)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8 - Single Click on priors and open one of the prior                
                viewer.OpenPriors(1, "click");
                int panelcount7 = viewer.GetStudyPanelCount();
                if ((panelcount7 == (panelcount6 + 1)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-9 - Ensure right prior is opened
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 2);
                var viewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                var step8 = viewer.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step8)
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


                //Step-10 Single Click on priors  and open one of the prior   
                viewer.OpenPriors(2, "dblclick");               
                int panelcount10 = viewer.GetStudyPanelCount();
                if (panelcount10 == (panelcount7 + 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-11 - Verify image
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 3);
                viewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                var step11 = viewer.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step11)
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

                //Step-12 - This is covered in above steps itself
                if (result.steps[ExecutedSteps].status.ToLower().Equals("pass"))
                {
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
        /// Verify when the user double clicks or single click on the patient study in Exam list only one study panel shall get open
        /// </summary>
        public TestCaseResult Test_161628(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                int expPriorCount = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount"));
                BluRingViewer viewer = null;

                //Step 1, 2 - Launch and Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;
                ExecutedSteps++;

                //Step 3 - Select Studies Tab
                var studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step-4 - Load the study in viewer
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy("Accession", accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step-5 - ExamList Open by default
                var priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                var priorcount = priors.Count;
                bool isPriorDisplayed = false;
                foreach (IWebElement prior in priors)
                {
                    if (prior.Displayed) { isPriorDisplayed = true; }
                    else { isPriorDisplayed = false; break; }
                }
                if (isPriorDisplayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6 - Close Exam List Panel
                var step6 = viewer.CloseExamList();
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


                //Step-7 - Open Exam List panel              
                var step7 = viewer.OpenExamList();
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


                //Step-8- Verify studies are listed  
                int panelcount8 = viewer.GetStudyPanelCount();
                priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                priorcount = priors.Count;
                if (priorcount == expPriorCount)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-9 - open a prior by double click
                viewer.OpenPriors(0, "dblclick");
                int panelcount9 = viewer.GetStudyPanelCount();
                if (panelcount9 == (panelcount8 + 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step - 10, 11 - Verify the image in the viewer               
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 2);
                var viewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                var step11 = viewer.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step11)
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

                //Step-11 - Same as step-10
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

                //Step-12 Single Click on priors and open one of the prior              
                viewer.OpenPriors(2, "click");
                int panelcount12 = viewer.GetStudyPanelCount();
                if (panelcount12 == (panelcount9 + 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13 - Verify image
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 3);
                viewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                var step13 = viewer.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step13)
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
        /// Verify the Exam List filters option has no white spaces on top and bottom in Exam list panel under Results List
        /// </summary>
        public TestCaseResult Test_161618(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                int expPriorCount = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount"));
                String modality6 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorModality");
                String institution10 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorSite");
                BluRingViewer viewer = null;

                //Step 1, 2 - Launch and Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;
                ExecutedSteps++;

                //Step 3 - Navigate to Search                
                var studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step 4 - Load a study with multiple series that contain multiple images.Ensure the scope is set to Image.                            
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy("Accession", accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step-5 -- Check for White Space in Modality drop down
                var modalityfilter = new SelectElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.select_priormodality));
                if (modalityfilter.Options.All<IWebElement>(element =>
                {
                    if (String.IsNullOrEmpty(element.GetAttribute("innerHTML").Replace(" ", "")))
                        return false;
                    else
                        return true;
                }))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-6 -- Check for White Space in Site filter drop down
                var sitefilter = new SelectElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.select_priorsite));
                if (sitefilter.Options.All<IWebElement>(element =>
                {
                    if (String.IsNullOrEmpty(element.GetAttribute("innerHTML").Replace(" ", "")))
                        return false;
                    else
                        return true;
                }))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step7 -- Check for White Space in Sort by drop down
                var sort = new SelectElement(viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.select_priorsort));
                if (sort.Options.All<IWebElement>(element =>
                {
                    if (String.IsNullOrEmpty(element.GetAttribute("innerHTML").Replace(" ", "")))
                        return false;
                    else
                        return true;
                }))
                {
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
        /// Verify the Exam List Panel shall cap the amount of total studies open to 5
        /// </summary>
        public TestCaseResult Test_161622(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String patientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                int expPriorCount = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount"));
                String modality6 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorModality");
                String institution10 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorSite");
                BluRingViewer viewer = null;
                String warningmessage = @"You have reached the maximum number of study viewer panels. Close a study viewer panel to open the selected study in a new panel.";

                //Precondition --  Setup Correct resolution
                BasePage.SetVMResolution("1980", "1080");


                //Step 1, 2 - Launch and Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;
                ExecutedSteps++;

                //Step 3 - Navigate to Search                
                var studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step 4 - Load a study in Bluring viewer.(study panel -1)                        
                studies.SearchStudy("Patient ID", patientID);
                studies.SelectStudy1(new string[] { "Patient ID", "Modality" }, new string[] { patientID, "PR,OP" });
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                int panelcount4 = viewer.GetStudyPanelCount();
                ExecutedSteps++;

                //Step-5 - Open Priors (study panel -2) 
                viewer.OpenPriors(1);                
                int panelcount5 = viewer.GetStudyPanelCount();
                var step5 = result.steps[++ExecutedSteps];
                step5.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 2);
                var study5 = viewer.CompareImage(step5, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (study5 && (panelcount5 == panelcount4 + 1))
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


                //Step-6 - Open Priors (study panel -3)                
                viewer.OpenPriors(2);              
                int panelcount6 = viewer.GetStudyPanelCount();
                var step6 = result.steps[++ExecutedSteps];
                step6.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 3);
                var study6 = viewer.CompareImage(step6, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (study6 && (panelcount6 == panelcount5 + 1))
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

                //Step-7 - Open Prior (study panel -4 and Study panel-5)                
                viewer.OpenPriors(3);
                viewer.OpenPriors(4);             
                int panelcount7 = viewer.GetStudyPanelCount();
                var step7 = result.steps[++ExecutedSteps];
                step7.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 5);
                var study7 = viewer.CompareImage(step7, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (study7 && (panelcount7 == panelcount6 + 2))
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


                //Step-8 - Open Prior (try to open 6th studypanel)             
                viewer.OpenPriors(5);
                var popup = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_MaxStudyPanelPopup));
                bool IsPopup = popup.Displayed;
                bool IsErrorMessag = popup.FindElement(By.CssSelector(BluRingViewer.div_MaxStudyPanelErrorMessage)).
                   GetAttribute("innerHTML").Trim().Equals(warningmessage);
                if (IsPopup && IsErrorMessag)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Flag Value - IsPopup" + IsPopup + "Flag Value--IsErrorMessag" + IsErrorMessag);
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9
                popup.FindElement(By.CssSelector("button")).Click();             
                try
                {
                    BasePage.wait.Until<Boolean>(d => d.FindElement(By.CssSelector(BluRingViewer.div_MaxStudyPanelPopup)).Displayed == false);
                    popup = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_MaxStudyPanelPopup));
                    IsPopup = popup.Displayed;
                }
                catch (Exception) { IsPopup = false; }
                if (!IsPopup)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Flag Value - IsPopup" + IsPopup + "Flag Value--IsErrorMessag" + IsErrorMessag);
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10               
                viewer.CloseStudypanel(1);
                int panelcount10_1 = viewer.GetStudyPanelCount();
                viewer.OpenPriors(5);
                int panelcount10_2 = viewer.GetStudyPanelCount();
                var step10 = result.steps[++ExecutedSteps];
                step10.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 5);
                var study10 = viewer.CompareImage(step10, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (study10 && (panelcount10_2 == panelcount10_1 + 1))
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
                viewer.CloseStudypanel(1);
                viewer.CloseStudypanel(1);
                viewer.CloseBluRingViewer();
                login.Logout();
                ExecutedSteps++;

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
                BasePage.SetVMResolution("1280", "1024");
            }

        }

        /// <summary>
        /// Verify the Date shall be displayed correctly in Exam List Panel and also in Study Panel
        /// </summary>
        public TestCaseResult Test_161621(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                int expPriorCount = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount"));
                String modality6 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorModality");
                String institution10 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorSite");
                BluRingViewer viewer = null;

                //Step 1, 2 - Launch and Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;
                ExecutedSteps++;

                //Step 3 - Navigate to Search                
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy("Accession", accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                int panelcount4 = viewer.GetStudyPanelCount();
                ExecutedSteps++;

                //Step-4 - Verify Date format in Exam List Panel
                var prior = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_priors);
                var priordate = prior.FindElement(By.CssSelector(BluRingViewer.div_priorDate)).GetAttribute("innerHTML");
                var date4 = DateTime.ParseExact(priordate, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
                if (date4 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5 - Verify Date format in Study Panel
                var studypaneldate = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypaneldate).GetAttribute("innerHTML");
                var date5 = DateTime.ParseExact(studypaneldate.Split(' ')[0], "dd-MMM-yyyy", CultureInfo.InvariantCulture);
                if (date5 != null)
                {
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
        /// Test 136908 - Results List - hovering and highlight
        /// </summary>
        public TestCaseResult Test_161629(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                int expPriorCount = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount"));
                String modality6 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorModality");
                String institution10 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorSite");
                BluRingViewer viewer = null;

                //Step 1 - Launch and Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step-2 - Select studies tab    
                var studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step 3 - Navigate to Search              
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy("Accession", accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step-4 - Verify that study panel and priors Loaded
                var step4 = result.steps[++ExecutedSteps];
                var viewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                step4.SetPath(testid, ExecutedSteps);
                var result4 = viewer.CompareImage(step4, viewport);
                var priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                var priorcount = priors.Count;
                bool isPriorDisplayed = false;
                foreach (IWebElement prior in priors)
                {
                    if (prior.Displayed) { isPriorDisplayed = true; }
                    else { isPriorDisplayed = false; break; }
                }
                if (isPriorDisplayed && result4)
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

                //Step-5 - Verify selected prior is in color
                var step5 = priors[0].GetCssValue("border-color").Equals("rgb(255, 255, 255)");
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

                //Step6 - Verify Cursor Change.
                var priorthumbnail = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsThumbnail))[0];
                BasePage.SetCursorPos(0, 0);
                viewer.JSMouseHover(priorthumbnail);
                var step6 = priorthumbnail.GetCssValue("cursor").Equals("pointer");
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

                //setp-7 - Verify reportIcon cursor type
                var reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                BasePage.SetCursorPos(0, 0);
                viewer.JSMouseHover(reportIcon);
                var step7 = reportIcon.GetCssValue("cursor").Equals("pointer");
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

                //Step-8 - Not Automated - Not clear
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-9 - Validate result counter UI
                var resultbar = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_resultsList);
                var step9_1 = resultbar.FindElement(By.CssSelector("span")).Size.Width == 10 ? true : false;
                var step9_2 = resultbar.FindElement(By.CssSelector("span")).GetCssValue("font-weight").Equals("normal");
                var step9_3 = resultbar.FindElement(By.CssSelector("span")).GetCssValue("color").Equals("rgba(158, 158, 158, 1)");
                if (step9_1 && step9_2 && step9_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Stp-10 - Validate line height
                var step10 = priors[0].FindElement(By.CssSelector(BluRingViewer.div_priorDate)).GetCssValue("line-height").Equals("16");
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
        /// Test 136455 - changes in Exam List Panel,Toolbox and Emaill
        /// </summary>
        public TestCaseResult Test_161623(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                IList<string> Accession = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID")).Split(',');
                string PriorsWithMoremodalities = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath"));
                string studyuid = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "Study UID"));

                //Precondition - Add DataSource - 
                var configtool = new ServiceTool();
                configtool.LaunchServiceTool();
                configtool.AddEADatasource("10.4.38.96", "ECM_ARC_96", "", dataSourceName:"VMSSA-4-38-96");                
                configtool.CloseServiceTool();
                configtool.RestartIIS();
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domaintab = login.Navigate<DomainManagement>();
                domaintab.SearchDomain("SuperAdminGroup");
                domaintab.SelectDomain("SuperAdminGroup");
                domaintab.ClickEditDomain();
                domaintab.ConnectAllDataSources();
                domaintab.ClickSaveEditDomain();
                login.Logout();

                //Step-1
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //step-2
                var studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", Accession[0]);
                studies.ChooseColumns(new string[] {"Study UID"});
                studies.SelectStudy("Study UID", studyuid);                
                var viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //Step-3
                viewer.OpenModalityFilter();
                IList<IWebElement> modalityOptionsText = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text));
                IWebElement Modality_Clear_all = BasePage.FindElementByCss(BluRingViewer.div_modalityFilterPopup).
                    FindElement(By.CssSelector(BluRingViewer.Modality_Clear_All));                
                bool step3 = studies.IsVerticalScrollBarPresent( BasePage.Driver.FindElement(By.CssSelector
                    (BluRingViewer.ModalityFliterPopUpWithScrollBar)));
                if (step3 && modalityOptionsText.Any<IWebElement>(ALL => ALL.Text.Equals("All")) && Modality_Clear_all.Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                
                //Step 4
                Modality_Clear_all = BasePage.FindElementByCss(BluRingViewer.div_modalityFilterPopup).
                    FindElement(By.CssSelector(BluRingViewer.Modality_Clear_All));
                Modality_Clear_all.Click();
                IList<IWebElement> ModalityOptionElemnet = BasePage.FindElementsByCss(BluRingViewer.modality_options_ele);
                bool ModalityClear_aLL = ModalityOptionElemnet.All<IWebElement>(option => option.GetAttribute("aria-selected").
                ToString().ToLower().Equals("false"));
                IWebElement Priors_result = BasePage.FindElementByCss(BluRingViewer.div_resultsList);
                string expected_result = "0 out of 16";
                bool Result_list = Priors_result.Text.Equals(expected_result);
                IList<IWebElement> Prior_list = BasePage.FindElementsByCss(BluRingViewer.div_priors);
                bool Prior_List = Prior_list.Count == 0 ? true : false;
                if (ModalityClear_aLL && Result_list && Prior_List)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail(string.Format("ModalityClear_aLL = {0}. Result_list: {1}. Prior_List: {2}", 
                    ModalityClear_aLL, Result_list, Prior_List), false);
                }
                viewer.CloseModalityFilter();

                //Step 5
                viewer.OpenModalityFilter();
                Modality_Clear_all = BasePage.FindElementByCss(BluRingViewer.div_modalityFilterPopup).
                    FindElement(By.CssSelector(BluRingViewer.Modality_Clear_All));
                Modality_Clear_all.Click();
                ++ExecutedSteps;
                ModalityOptionElemnet = BasePage.FindElementsByCss(BluRingViewer.modality_options_ele);
                ModalityOptionElemnet[0].Click();
                if (ModalityOptionElemnet[0].GetAttribute("aria-selected").ToString().ToLower().Equals("false"))
                    ModalityOptionElemnet[0].Click();
                foreach (IWebElement Options in ModalityOptionElemnet)
                {
                    if ((Options.GetAttribute("aria-selected").ToString().ToLower().Equals("true") && Options.
                        FindElement(By.CssSelector(BluRingViewer.modality_options_text)).Text == "All"))
                    {
                        result.steps[ExecutedSteps].AddPassStatusList();
                    }
                    else if ((Options.GetAttribute("aria-selected").ToString().ToLower().Equals("false") && Options.
                        FindElement(By.CssSelector(BluRingViewer.modality_options_text)).Text != "All"))
                    {
                        result.steps[ExecutedSteps].AddPassStatusList();
                    }
                    else
                        result.steps[ExecutedSteps].AddFailStatusList();
                }
                IList<IWebElement> Priors_Modality = BasePage.FindElementsByCss(BluRingViewer.div_RSmodality);
                Priors_result = BasePage.FindElementByCss(BluRingViewer.div_resultsList);
                expected_result = "16 out of 16";
                if(Priors_result.Text.Equals(expected_result))
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                viewer.CloseModalityFilter();
                viewer.CloseBluRingViewer();

                //Step 6
                studies.ChooseColumns(new string[] { "Study UID" });
                studies.SelectStudy("Study UID", studyuid);
                viewer = BluRingViewer.LaunchBluRingViewer();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IList<IWebElement> ModalityDropDown = BasePage.FindElementsByCss(BluRingViewer.div_multiSelect_Modality);
                new TestCompleteAction().MoveToElement(ModalityDropDown[0]).Perform();
                bool ModalityToolTip = ModalityDropDown[0].GetAttribute("title") == "All";
                ExecutedSteps++;
                if (ModalityToolTip == false)
                    result.steps[ExecutedSteps].comments = "Default, the selected filter is ALL and not displayed" +
                        " on Hovering over the Modality drop-down box ";
                 new TestCompleteAction().MoveToElement(ModalityDropDown[1]).Perform();
                bool SortByToolTip = ModalityDropDown[1].GetAttribute("title") == "Date - Newest";
                if (SortByToolTip == false)
                    result.steps[ExecutedSteps].comments = "Default, the Date - Newest";
                if (SortByToolTip && ModalityToolTip)
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail(string.Format("Modality ToolTip :{0}. , SortByTooltip: {1}.", ModalityToolTip, 
                        SortByToolTip));              

                //Step 7
                viewer.OpenModalityFilter();
                Modality_Clear_all = BasePage.FindElementByCss(BluRingViewer.div_modalityFilterPopup).
                    FindElement(By.CssSelector(BluRingViewer.Modality_Clear_All));
                Modality_Clear_all.Click();
                foreach (IWebElement DropDown in BasePage.FindElementsByCss(BluRingViewer.modality_options_text))
                {
                    if (DropDown.Text.Equals("CR") || DropDown.Text.Equals("MR"))
                        DropDown.Click();
                }
                viewer.CloseModalityFilter();
                Priors_Modality = BasePage.FindElementsByCss(BluRingViewer.div_RSmodality);
                Priors_result = BasePage.FindElementByCss(BluRingViewer.div_resultsList);
                expected_result = "10 out of 16";
                bool Result_list_Step10 = Priors_result.Text.Equals(expected_result);
                IList<string> modalityText = new List<string>();
                foreach( IWebElement modality in Priors_Modality)
                {
                    modalityText.Add(modality.GetAttribute("innerHTML"));
                }       
                if (modalityText.All<string>(X =>(( X.Contains("CR") || X.Contains("MR")) )) && Result_list_Step10)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 8
                new TestCompleteAction().MoveToElement(ModalityDropDown[0]).Perform();
                ModalityToolTip = ModalityDropDown[0].GetAttribute("title") == "CR,MR";
                if (ModalityToolTip == true)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 9
                new TestCompleteAction().MoveToElement(ModalityDropDown[1]).Perform();
                SortByToolTip = ModalityDropDown[1].GetAttribute("title") == "Date - Newest";
                if (SortByToolTip == true)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }                

                //Logout and Return Result
                viewer.CloseModalityFilter();
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

            finally
            {
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domaintab = login.Navigate<DomainManagement>();
                domaintab.SearchDomain("SuperAdminGroup");
                domaintab.SelectDomain("SuperAdminGroup");
                domaintab.ClickEditDomain();
                domaintab.DisConnectDataSource("VMSSA-4-38-96");
                domaintab.ClickSaveEditDomain();
                login.Logout();

                var configtool = new ServiceTool();
                configtool.LaunchServiceTool();
                configtool.NavigateToConfigToolDataSourceTab();
                configtool.DeleteDataSource(0, "VMSSA-4-38-96");                
                configtool.CloseServiceTool();
                configtool.RestartIIS();
            }
        }

        /// <summary>
        ///Verify the Exam list shall show/hide instantly without any animation effect when user click on the EXAMS icon in the Global Toolbar
        /// </summary>
        public TestCaseResult Test_161625(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String Prior_count = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] Accession = AccessionList.Split(':');

                //Step 1
                //Launch the BlueRing application with a client browser                 
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Step-2
                //Login to WebAccess site with any privileged user.
                login.LoginIConnect(adminUserName, adminPassword);
                if (login.IsTabPresent("Studies") && login.IsTabPresent("Patients") && login.IsTabPresent("Domain Management"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 3
                //Select Studies tab.
                Studies studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step 4
                //Search and select study and click "BluRing View" button
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA91));
                studies.SelectStudy("Accession", Accession[0]);
                BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 5
                //Verify the Exam List column should opened by default.
                var step5 = viewer.IsExamListVisible();
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

                //Step-6
                //Verify the patient related studies are displayed under Results List.
                IList<IWebElement> Exam_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList));
                if (Exam_list[0].Displayed && Exam_list[1].Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7                
                IList<IWebElement> Study_Panel = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                int before_click = Study_Panel.Count;
                viewer.OpenPriors(1);
                Study_Panel = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                int after_click = Study_Panel.Count;
                if (before_click == (after_click - 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //----validation-- for -st 9--
                Study_Panel = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                int panel1_w = Study_Panel[0].Size.Width;
                int panel2_w = Study_Panel[1].Size.Width;

                //Step-8
                //Click on 'EXAMS' icon on the Global Toolbar 
                viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ExamsIconButton).Click();
                var isExamLitOpen = viewer.IsExamListVisible();
                if (!isExamLitOpen)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9
                //Verify the size of all viewports should get increased.
                Study_Panel = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                int after_Expand_panel1_w = Study_Panel[0].Size.Width;
                int after_Expand_panel2_w = Study_Panel[1].Size.Width;

                //Size of all viewports are get increased.
                if (panel1_w < after_Expand_panel1_w &&
                    panel2_w < after_Expand_panel2_w
                    )
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10
                //Click on 'EXAMS' icon on the Global Toolbar and
                viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ExamsIconButton).Click();
                isExamLitOpen = viewer.IsExamListVisible();
                if (isExamLitOpen)
                {
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
                //Verify the size of all viewports should get decreased.
                Study_Panel = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                if (panel1_w == Study_Panel[0].Size.Width &&
                    panel2_w == Study_Panel[1].Size.Width)

                {
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
        /// Verify the two study panels shall not get open when user double clicks on patient study in Exam List
        /// </summary>
        public TestCaseResult Test_136820(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String Prior_count = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] PatientID = PatientIDList.Split(':');

                //Step 1
                //Launch the BlueRing application with a client browser 
                login.DriverGoTo(login.url);

                //The BlueRing application login page is displayed
                if (login.UserIdTxtBox().Displayed &&
                    login.PasswordTxtBox().Displayed &&
                    login.LoginBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-2
                //Login to WebAccess site with any privileged user.
                login.LoginIConnect(adminUserName, adminPassword);

                //Able to login to the web application; Able to see Studies, Patients, Domain Management tab
                if (login.IsTabPresent("Studies") && login.IsTabPresent("Patients") && login.IsTabPresent("Domain Management"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 3
                //Select Studies tab.
                Studies studies = (Studies)login.Navigate("Studies");
                //A Studies page is displayed.
                ExecutedSteps++;

                //Step 4
                //Search and select multiple studies that contain huge number of related studies 
                //with same patient IDs and click on BluRing View button
                //PatientID -- Mickey^Mouse
                studies.SearchStudy(patientID: PatientID[0], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Patient ID", PatientID[0]);
                BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 5
                //Verify the Exam List column should opened by default.
                //Exam List column is opened by default.
                var priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                var priorcount = priors.Count;
                if (priorcount == (Int32.Parse(Prior_count))) //Count Excel --147
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6
                //Verify the patient related studies are displayed under Results List.
                IList<IWebElement> Exam_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList));
                //The patient related studies are displayed under Results List section.
                if (Exam_list[0].Displayed && Exam_list[1].Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7
                //Single click on the study info area under Results list  
                IList<IWebElement> Study_Panel = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                int before_click = Study_Panel.Count;
                Exam_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList));
                Exam_list[0].Click();
                PageLoadWait.WaitForFrameLoad(20);
                int after_click = Study_Panel.Count;

                //The study is opened in a new study panel 
                if (before_click == (after_click - 1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //----validation-- for -st 9--
                Study_Panel = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                int panel1_w = Study_Panel[0].Size.Width;
                int panel1_h = Study_Panel[0].Size.Height;
                int panel2_w = Study_Panel[1].Size.Width;
                int panel2_h = Study_Panel[1].Size.Height;

                //Step-8
                //Click on 'EXAMS' icon on the Global Toolbar 
                viewer.CloseExamList();

                //Exam list is disappeared instantly w/o any animation 
                Exam_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList));
                if (Exam_list[0].Displayed == false &&
                    Exam_list[1].Displayed == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9
                //Verify the size of all viewports should get increased.
                Study_Panel = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                int after_Expand_panel1_w = Study_Panel[0].Size.Width;
                int after_Expand_panel1_h = Study_Panel[0].Size.Height;
                int after_Expand_panel2_w = Study_Panel[1].Size.Width;
                int after_Expand_panel2_h = Study_Panel[1].Size.Height;

                //Size of all viewports are get increased.
                if (panel1_w < after_Expand_panel1_w &&
                    panel2_w < after_Expand_panel2_w &&
                    panel1_h < after_Expand_panel1_h &&
                    panel2_h < after_Expand_panel2_h)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10
                //Click on 'EXAMS' icon on the Global Toolbar and
                viewer.OpenExamList();

                //Exam list is appeared instantly.
                Exam_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList));
                if (Exam_list[0].Displayed == true &&
                    Exam_list[1].Displayed == true)
                {
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
                //Verify the size of all viewports should get decreased.
                Study_Panel = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudyPanel));
                if (panel1_w == Study_Panel[0].Size.Width &&
                    panel2_w == Study_Panel[1].Size.Width &&
                    panel1_h == Study_Panel[0].Size.Height &&
                    panel2_h == Study_Panel[1].Size.Height)
                {
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
        /// Verify the Scroll bar shall shown beside the related study list when user loads the related studies list is longer than the exam list panel
        /// </summary>
        public TestCaseResult Test_161635(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                string studyuid = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "Study UID"));

                //Step-1                                
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Step-2                
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step-3                
                Studies studies = (Studies)login.Navigate("Studies");                
                studies.SearchStudy("Accession", accession);
                studies.ChooseColumns(new string[] { "Study UID" });
                studies.SelectStudy("Study UID", studyuid);
                BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step-4
                var container = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ContainerPriors);
                var step4 = viewer.IsVerticalScrollBarPresent(container);
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

                //Step-5
                var lisopcontainer = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ContainerList);
                var step5 = viewer.IsVerticalScrollBarPresent(lisopcontainer);
                if (!step5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-6 - Resize browser
                int width = BasePage.Driver.Manage().Window.Size.Width;
                int height = BasePage.Driver.Manage().Window.Size.Height;
                BasePage.Driver.Manage().Window.Size = new System.Drawing.Size(width, (height - 100));
                BasePage.wait.Until<Boolean>(driver => driver.Manage().Window.Size.Height == (height - 100));
                BasePage.Driver.Manage().Window.Position = new System.Drawing.Point(0, 0);
                container = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ContainerPriors);
                var step7 = viewer.IsVerticalScrollBarPresent(container);
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

                //Step-7               
                if(viewer.IsAllPriorsDisplayed())
                {
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
                BasePage.Driver.Manage().Window.Maximize();
                BasePage.wait.Until<Boolean>(driver => driver.Manage().Window.Size.Height == height);
                container = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ContainerPriors);
                var step8 = viewer.IsVerticalScrollBarPresent(container);
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

                //Step-9
                new TestCompleteAction().MouseScroll(container, "up", "10").Perform();
                new TestCompleteAction().MouseScroll(container, "up", "10").Perform();
                new TestCompleteAction().MouseScroll(container, "up", "15").Perform();
                if (viewer.IsAllPriorsDisplayed())
                {
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
                BasePage.Driver.Manage().Window.Maximize();
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

            finally
            {
                BasePage.Driver.Manage().Window.Maximize();
            }
        }

        /// <summary>
        /// Test 137742 - Verify the Scroll bar shall not be displayed when user the loads multiple studies that contain minimum(four or five related studies)number of related studies with same patient IDs is in the exam list panel
        /// </summary>
        public TestCaseResult Test_161636(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

                //Step-1                                
                login.DriverGoTo(login.url);
                ExecutedSteps++;

                //Step-2                
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step-3                
                Studies studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step-4
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy("Accession", accession);
                BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step-5
                var container = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ContainerPriors);
                var step5 = viewer.IsVerticalScrollBarPresent(container);
                if (!step5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6 - Resize browser
                int width = BasePage.Driver.Manage().Window.Size.Width;
                int height = BasePage.Driver.Manage().Window.Size.Height;
                int screenhieght = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
                BasePage.Driver.Manage().Window.Size = new System.Drawing.Size(width, ((screenhieght / 3) + 200));
                BasePage.wait.Until<Boolean>(driver => driver.Manage().Window.Size.Height == ((screenhieght / 3) + 200));
                Thread.Sleep(1000);
                BasePage.Driver.Manage().Window.Position = new System.Drawing.Point(0, 0);
                container = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ContainerPriors);
                var step6 = viewer.IsVerticalScrollBarPresent(container);
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

                //Step-7
                var step7 = viewer.IsAllPriorsDisplayed();
                if(step7)
                {
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
                BasePage.Driver.Manage().Window.Maximize();
                BasePage.wait.Until<Boolean>(driver => driver.Manage().Window.Size.Height == height);
                container = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ContainerPriors);
                var step8 = viewer.IsVerticalScrollBarPresent(container);
                if (!step8)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-9
                new TestCompleteAction().MouseScroll(container, "up", "10").Perform();
                new TestCompleteAction().MouseScroll(container, "up", "10").Perform();
                new TestCompleteAction().MouseScroll(container, "up", "15").Perform();
                var step9 = viewer.IsAllPriorsDisplayed();
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

                //Logout Application
                BasePage.Driver.Manage().Window.Maximize();
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

            finally
            {
                BasePage.Driver.Manage().Window.Maximize();
            }

        }

        /// <summary>
        /// Verify the scrollbar is displaying to the exam list when clicking on exam list thumbnail icon.
        /// </summary>
        public TestCaseResult Test_161639(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String datasourcelist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList");
                var arrAccession = accession.Split(':');
                var arrdatasource = datasourcelist.Split(':');

                //Step-1 & 2                
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;
                ExecutedSteps++;

                //Step-3                
                Studies studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: arrAccession[0], Datasource: arrdatasource[0]);
                studies.SelectStudy("Accession", arrAccession[0]);
                BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step-4
                var step4 = viewer.IsExamListVisible();
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

                //Step-5
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


                //Step-6
                IList<IWebElement> priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsBlock));
                viewer.ClickExamListThumbnailIcon(priors[0]);
                var step7_1 = viewer.IsVerticalScrollBarPresent(BasePage.Driver.
                FindElement(By.CssSelector(BluRingViewer.div_thumbnailContainerExamList)));
                new Actions(BasePage.Driver).Click(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_closeStudy)));
                PageLoadWait.WaitForLoadingMessage();
                viewer.CloseBluRingViewer();
                if (step7_1)
                {
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
        /// Test 138517 - Verify that Exam List and Exam List text(date,modality,contrast ans site) displayed brighter
        /// </summary>
        public TestCaseResult Test_161638(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                var arrAccession = accession.Split(':');

                //Step-1 & 2                
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;
                ExecutedSteps++;

                //Step-3                
                Studies studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", arrAccession[0]);
                studies.SelectStudy("Accession", arrAccession[0]);
                BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step-4
                var examlist = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ContainerPriors);
                var step4 = result.steps[++ExecutedSteps];
                step4.SetPath(testid, ExecutedSteps);
                var isExamListImageCorrect = viewer.CompareImage(step4, examlist);
                if (isExamListImageCorrect)
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
        /// Test 139108 - Verify that Stickman should not be displayed in Bluring viewer
        /// </summary>
        public TestCaseResult Test_161651(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                var arrAccession = accession.Split(':');

                //Step-1               
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step-2               
                Studies studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //step-3
                studies.SearchStudy("Accession", arrAccession[0]);
                studies.SelectStudy("Accession", arrAccession[0]);
                BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step-4
                var examlist = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_examListPanel);
                var step4 = result.steps[++ExecutedSteps];
                /*step4.SetPath(testid, ExecutedSteps);
                var isExamListImageCorrect = viewer.CompareImage(step4, examlist);*/
                bool isExamListImageCorrect = examlist.Location.Y <= 75 ? true : false;
                if (isExamListImageCorrect)
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

                //step-5
                viewer.CloseBluRingViewer();
                studies.SearchStudy("Accession", arrAccession[1]);
                studies.SelectStudy("Accession", arrAccession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                examlist = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_examListPanel);
                var step5 = result.steps[++ExecutedSteps];
                /*step5.SetPath(testid, ExecutedSteps);
                isExamListImageCorrect = viewer.CompareImage(step5, examlist);*/
                isExamListImageCorrect = examlist.Location.Y <= 75 ? true : false;
                if (isExamListImageCorrect)
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
        /// In Domain Management, leave the default configuration for Query Related Study Parameter: Patient ID + Patient Full Name
        /// </summary>
        public TestCaseResult Test_138736(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            String errormessage = "At least Patient ID, Patient Full Name or Patient Last Name is required as query related study parameters.";

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String domainname = "SuperAdminGroup";

                //step-1 - Check only Patient DOB  
                login.LoginIConnect(adminUserName, adminPassword);
                var domaintab = login.Navigate<DomainManagement>();
                domaintab.SetupStudyQueryParameters(domainname, new QueryParamters[] { QueryParamters.PatientDOB });
                String errormesag1 = domaintab.ErrorMessage();
                domaintab.ClickCloseEditDomain();
                if (errormesag1.Trim().Equals(errormessage))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-2 - Check only IPID
                domaintab.SetupStudyQueryParameters(domainname, new QueryParamters[] { QueryParamters.IPID });
                String errormesag2 = domaintab.ErrorMessage();
                domaintab.ClickCloseEditDomain();
                if (errormesag1.Trim().Equals(errormessage))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step-3 - Check only IPID and DOB
                domaintab.SetupStudyQueryParameters(domainname, new QueryParamters[] { QueryParamters.PatientDOB, QueryParamters.PatientDOB });
                String errormesag3 = domaintab.ErrorMessage();
                domaintab.ClickCloseEditDomain();
                if (errormesag3.Trim().Equals(errormessage))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4 - Uncheck every field                
                domaintab.SetupStudyQueryParameters(domainname, new QueryParamters[] { });
                String errormesag4 = domaintab.ErrorMessage();
                domaintab.ClickCloseEditDomain();
                if (errormesag4.Trim().Equals(errormessage))
                {
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
        ///  Test 137316 - Priors with Reports -Verify that studies containing multiple priors with reports can be launched in BR Viewer 
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161634(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try

            {
                String accessionlist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String datasourcelist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList");
                String descriptionlist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
                var arrAccession = accessionlist.Split(':');
                var arrdatasource = datasourcelist.Split(':');
                var arrDescription = descriptionlist.Split(':');
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;

                //Step-1 and 2
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;
                ExecutedSteps++;

                //Step-3
                var studies = login.Navigate<Studies>();
                studies.SearchStudy(AccessionNo: arrAccession[0], Datasource: arrdatasource[0], Description: arrDescription[0]);
                studies.SelectStudy("Accession", arrAccession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step-4 - lanuch Reports and perform validation
                var prior = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors))[0];
                var date = prior.FindElement(By.CssSelector(BluRingViewer.div_examListPanelDate)).GetAttribute("innerHTML") + " " +
                    prior.FindElement(By.CssSelector(BluRingViewer.div_priorTime)).GetAttribute("innerHTML");
                viewer.OpenReport_BR(0);
                var report_data1 = viewer.FetchReportData_BR(0);
                PageLoadWait.WaitForFrameLoad(10);
                var reportcount = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.ReportTabList_div)).Count;
                viewer.SelectReport_BR(0, 1);
                var report_data2 = viewer.FetchReportData_BR(0);
                viewer.CloseReport_BR(0);
                if (reportcount == 2 && report_data1 != null && report_data2 != null && report_data1["Exam Date:"].Equals(date))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5 - Validate exam date
                viewer.OpenReport_BR(1);
                viewer.SelectReport_BR(1, 1);
                PageLoadWait.WaitForFrameLoad(1);
                var prior2 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors))[1];
                var date2 = prior2.FindElement(By.CssSelector(BluRingViewer.div_examListPanelDate)).GetAttribute("innerHTML") + " " +
                    prior2.FindElement(By.CssSelector(BluRingViewer.div_priorTime)).GetAttribute("innerHTML");
                viewer.NavigateToReportFrame();
                var report_data_2 = viewer.FetchReportData_BR(1);
                viewer.CloseReport_BR(1);
                if (report_data_2["Exam Date:"].Contains(date2))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6 - Validate reports launched for each prior
                PageLoadWait.WaitForFrameLoad(1);
                viewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: arrAccession[1], Datasource: arrdatasource[0], Description: arrDescription[1]);
                studies.SelectStudy("Accession", arrAccession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                int priorcount6 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors)).Count;
                ExecutedSteps++;
                for (int iterate = 0; iterate < priorcount6; iterate++)
                {
                    viewer.OpenReport_BR(iterate);
                    var report_data6 = viewer.FetchReportData_BR(iterate);
                    if (report_data6 != null)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                        Logger.Instance.ErrorLog("Report Data not available for prior" + priorcount6);
                        result.steps[ExecutedSteps].SetLogs();
                        break;
                    }

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
        ///  BLU-542 Prior Relevancy/Exam List 
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161630(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                String patientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String priorscount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                var arrPriors = priorscount.Split(':');
                var arrAccession = accession.Split(':');
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                bool isPriorsDisplayed = false;
                String domainname = "SuperAdminGroup";

                //Precondiion
                login.LoginIConnect(adminUserName, adminPassword);
                var domain = login.Navigate<DomainManagement>();
                domain.SetupStudyQueryParameters(domainname, new QueryParamters[] { QueryParamters.FullName, QueryParamters.PatientID });

                //Step-1                
                var studies = login.Navigate<Studies>();
                studies.SearchStudy(patientID: patientID);
                studies.SelectStudy("Accession", arrAccession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                int priorscount_actual = viewer.CheckPriorsCount();
                var priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                foreach (var prior in priors)
                {
                    //Neeed to add logic here
                    if (prior.Displayed)
                        isPriorsDisplayed = true;
                    else { isPriorsDisplayed = false; break; }

                }
                Logger.Instance.InfoLog("isPriorsDisplayed:" + isPriorsDisplayed);
                Logger.Instance.InfoLog("priorscount actual:" + priorscount_actual);
                var step1 = result.steps[++ExecutedSteps];
                step1.SetPath(testid, ExecutedSteps);
                var isImageComapre1 = viewer.CompareImage(step1, BasePage.FindElementByCss(viewer.Activeviewport));                
                if ((Int32.Parse(arrPriors[0]) == priorscount_actual) && isPriorsDisplayed && isImageComapre1)
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

                //Step-2    
                viewer.OpenPriors(1);
                viewer.SetViewPort(0, 2);
                var step2 = result.steps[++ExecutedSteps];
                step2.SetPath(testid, ExecutedSteps);
                var isImageComapre2 = viewer.CompareImage(step2, BasePage.FindElementByCss(viewer.Activeviewport));
                if(isImageComapre2)
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
                viewer.OpenPriors(2);
                viewer.SetViewPort(0, 3);
                var step3 = result.steps[++ExecutedSteps];
                step3.SetPath(testid, ExecutedSteps);
                var isImageComapre3 = viewer.CompareImage(step3, BasePage.FindElementsByCss(BluRingViewer.div_StudyPanel)[2]);
                if (isImageComapre3)
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
                viewer.SetViewPort(0, 3);
                viewer.SelectViewerTool(BluRingTools.Flip_Horizontal, 3);
                var step4 = result.steps[++ExecutedSteps];
                step4.SetPath(testid, ExecutedSteps);
                var isCompare4 = viewer.CompareImage(step4, BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport)));
                if (isCompare4)
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
                viewer.CloseStudypanel(1);
                viewer.CloseStudypanel(1);
                viewer.CloseBluRingViewer();              
                ExecutedSteps++;

                //Step-6
                login.Navigate<DomainManagement>();
                domain.SetupStudyQueryParameters(domainname, new QueryParamters[] { QueryParamters.PatientID });
                ExecutedSteps++;

                //Step-7
                studies = login.Navigate<Studies>();
                studies.SearchStudy(patientID: patientID);
                studies.SelectStudy("Patient ID", patientID);
                viewer = BluRingViewer.LaunchBluRingViewer();
                var step7 = result.steps[++ExecutedSteps];
                step7.SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(step7, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport)))
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
                priorscount_actual = viewer.CheckPriorsCount();
                viewer.OpenPriors(0);
                int priorcount8 = viewer.CheckPriorsCount();
                var step8 = result.steps[++ExecutedSteps];
                step8.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 2);
                var isStudyCompare8 = viewer.CompareImage(step8, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isStudyCompare8 && priorcount8 == (Int32.Parse(arrPriors[1])))
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
                viewer.OpenPriors(1);
                var step9 = result.steps[++ExecutedSteps];
                step9.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 3);
                var isStudyCompare9 = viewer.CompareImage(step9, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isStudyCompare9)
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
                viewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Logout Application  
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
        /// Test Precondition - Setting up Domain, User and Sharing studies.
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_Precondition(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                String patientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String priorscount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String datasourcelist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList");
                var arrDataSources = datasourcelist.Split(':');
                var ds1 = arrDataSources[0];
                var ds2 = arrDataSources[1] + "=" + login.GetHostName(Config.DestinationPACS);
                var arrPriors = priorscount.Split(':');
                var arrAccession = accession.Split('=');
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String domainname = String.Empty;
                String role1 = BasePage.GetUniqueRole("R1");
                String role2 = BasePage.GetUniqueRole("R2");
                String user1 = BasePage.GetUniqueUserId("U1");
                String user2 = BasePage.GetUniqueUserId("U2");
                String destination = "Destination" + new Random().Next(1, 100);
                String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyPath");
                String imagescount_shared = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Images");
                String stuydate_shared = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorDate");
                String modality_shared = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorModality");


                //Step-1, 2 and 3
                //create domain, role users
                login.LoginIConnect(adminUserName, adminPassword);
                var domain = login.Navigate<DomainManagement>();
                var domainattr = domain.CreateDomainAttr();
                domainname = domainattr[DomainManagement.DomainAttr.DomainName];
                domain.CreateDomain(domainattr, isgrantaccessneeded: true, isimagesharingneeded: true);
                domain.SetupStudyQueryParameters(domainname, new QueryParamters[] { QueryParamters.LastName });
                var rolemgmt = login.Navigate<RoleManagement>();
                rolemgmt.CreateRole(domainname, role1, "", ds1);
                rolemgmt.CreateRole(domainname, role2, "both", ds2);
                var usertab = login.Navigate<UserManagement>();
                usertab.CreateUser(user1, role1);
                usertab.CreateUser(user2, role2);

                //Create Destination for DomainB
                var imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                var pagedestination = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                //pagedestination.CreateDestination(login.GetHostName(Config.DestinationPACS), user2, user2, destination, domainname);
                pagedestination.AddDestination(domainname, destination, login.GetHostName(Config.DestinationPACS), user2, user2);
                var instTab = (Image_Sharing.Institution)imagesharing.NavigateToSubTab("Institution");

                //Update Test Data Sheet
                ReadExcel.UpdateTestData(filepath, "TestData", testid, "DomainName", domainname);
                ReadExcel.UpdateTestData(filepath, "TestData", testid, "User1", user1);
                ReadExcel.UpdateTestData(filepath, "TestData", testid, "User2", user2);

                //Generate Installer
                var servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Image Sharing");
                servicetool.GenerateInstallerExamImporter(domainname, "EI" + domainname);
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                //Install EI and upload study
                var ei = new ExamImporter();
                ei.EI_Installation(domainname, "EI" + domainname, Config.Inst1, user2, user2);
                Logger.Instance.InfoLog("The Window name of EI is" + "EI" + domainname);
                String eipath = "C:\\Users\\Administrator\\AppData\\Local\\Apps" + "\\EI" + domainname + "\\bin" + "\\UploaderTool.exe";
                var cduploader = new ExamImporter();
                cduploader.eiWinName = "EI" + domainattr[DomainManagement.DomainAttr.DomainName];
                cduploader.EIDicomUpload(user2, user2, destination, studypath, path: eipath);
                ExecutedSteps++;
                ExecutedSteps++;
                ExecutedSteps++;

                //Step-4
                login.Logout();
                login.LoginIConnect(user1, user1);
                var userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                var studies = login.Navigate<Studies>();
                String accession5 = arrAccession[1].Split(':')[0];
                String datasource5 = arrAccession[1].Split(':')[1];
                studies.SearchStudy(AccessionNo: accession5);
                studies.SelectStudy("Accession", accession5);
                studies.ShareStudy(false, new string[] { user2 }, domainName: domainname);
                var outbounds = login.Navigate<Outbounds>();
                outbounds.SearchStudy(AccessionNo: accession5);
                outbounds.ChooseColumns(new string[] { "Modality", "# Images" });
                var study_outbounds = outbounds.GetMatchingRow(new string[] { "Accession", "Study Date", "# Images", "Status", "Modality" }, new string[] { accession5, stuydate_shared, imagescount_shared, "Shared", modality_shared });
                login.Logout();
                login.LoginIConnect(user2, user2);
                userpreferences.OpenUserPreferences();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                var inbounds = login.Navigate<Inbounds>();
                outbounds.SearchStudy(AccessionNo: accession5);
                outbounds.ChooseColumns(new string[] { "Modality", "# Images" });
                var study_inbounds = inbounds.GetMatchingRow(new string[] { "Accession", "Study Date", "# Images", "Status", "Modality" }, new string[] { accession5, stuydate_shared, imagescount_shared, "Shared", modality_shared });
                if (study_outbounds != null && study_inbounds != null)
                {
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
        /// Test 139003 - 1-Viewing priors with Query Related Study Parameters Applied - Patient Last Name
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161643(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            String domainame = String.Empty;

            try
            {
                var patientID = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split(':');
                String priorscount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String accession2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                var datasourcelist = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList")).Split(':');
                domainame = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String priorcount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String user1 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "User1");
                String user2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "User2");
                var user1Priors = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "U1PriorsList")).Split(':');
                var user2Priors = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "U2PriorsList")).Split(':');
                String sharedstudy = accession.Split('=')[1].Split(':')[0];
                String uploadedstudy = accession.Split('=')[0].Split(':')[0];
                var arrPriorscount = priorcount.Split(':');
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);


                //Step-1 to 5
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domaintab = login.Navigate<DomainManagement>();
                domaintab.SetupStudyQueryParameters(domainame, new QueryParamters[] { QueryParamters.LastName });
                ExecutedSteps++;
                ExecutedSteps++;
                ExecutedSteps++;
                ExecutedSteps++;
                ExecutedSteps++;

                //Step-6
                login.Logout();
                login.LoginIConnect(user1, user1);
                var outbounds = login.Navigate<Outbounds>();
                outbounds.SearchStudy("Accession", sharedstudy);
                var viewer = BluRingViewer.LaunchBluRingViewer("OutBounds", "Accession", sharedstudy);
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[0]) && viewer.CheckAccession_ExamList(user1Priors))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7
                login.Logout();
                login.LoginIConnect(user2, user2);
                var inbounds = login.Navigate<Inbounds>();
                inbounds.SearchStudy("Accession", sharedstudy);
                var study7 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { sharedstudy, "Shared" });
                if (study7 != null)
                {
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
                viewer = BluRingViewer.LaunchBluRingViewer("Inbounds", "Accession", sharedstudy);
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors) 
                    && viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9
                viewer.OpenPriors(accession:accession2);
                viewer.SetViewPort(0, 2);
                var step9 = result.steps[++ExecutedSteps];
                step9.SetPath(testid, ExecutedSteps);
                var isImageCompare9 = viewer.CompareImage(step9, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare9)
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

                //Step-10 -- Opening Study uploaded in Holding pen
                viewer.OpenPriors(accession: uploadedstudy);
                var step10 = result.steps[++ExecutedSteps];
                step10.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 3);
                var isImageCompare10 = viewer.CompareImage(step10, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare10)
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
                var studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", accession2);
                studies.SelectStudy("Accession", accession2);
                BluRingViewer.LaunchBluRingViewer();
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors) &&
                    viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12 -- Opening the Shared Study from exam list
                viewer.OpenPriors(accession: sharedstudy);
                var step12 = result.steps[++ExecutedSteps];
                step12.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 2);
                var isImageCompare12 = viewer.CompareImage(step12, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare12)
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

                //Step-13 -- Open the study uploaded in Holding pen
                viewer.OpenPriors(accession: uploadedstudy);
                var step13 = result.steps[++ExecutedSteps];
                step13.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 3);
                var isImageCompare13 = viewer.CompareImage(step13, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare13)
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
                outbounds = login.Navigate<Outbounds>();
                outbounds.SearchStudy("Accession", uploadedstudy);
                viewer = BluRingViewer.LaunchBluRingViewer("OutBounds", "Accession", uploadedstudy);
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors)
                    && viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-15 -- Loading shared prior
                viewer.OpenPriors(accession: sharedstudy);
                viewer.SetViewPort(0, 2);
                var step15 = result.steps[++ExecutedSteps];
                step15.SetPath(testid, ExecutedSteps);
                var isImageCompare15 = viewer.CompareImage(step15, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare15)
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


                //Step-16 -- Open a prior that belongs to U2
                viewer.OpenPriors(accession: accession2);
                viewer.SetViewPort(0, 3);
                var step16 = result.steps[++ExecutedSteps];
                step16.SetPath(testid, ExecutedSteps);
                var isImageCompare16 = viewer.CompareImage(step16, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare16)
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
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domaintab = login.Navigate<DomainManagement>();
                domaintab.SetupStudyQueryParameters(domainame, new QueryParamters[] { QueryParamters.PatientID, QueryParamters.FullName });
                login.Logout();
            }

        }

        /// <summary>
        /// Test 139004 2-Viewing priors with Query Related Study Parameters Applied - Patient Full Name
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161644(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            String domainame = String.Empty;
            int ExecutedSteps = -1;

            try
            {
                var patientID = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split(':');
                String priorscount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String accession2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                var datasourcelist = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList")).Split(':');
                domainame = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String priorcount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String priordate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorDate");
                String priormodality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorModality");
                String user1 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "User1");
                String user2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "User2");
                String sharedstudy = accession.Split('=')[1].Split(':')[0];
                String uploadedstudy = accession.Split('=')[0].Split(':')[0];
                var arrPriorscount = priorcount.Split(':');
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                var user1Priors = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "U1PriorsList")).Split(':');
                var user2Priors = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "U2PriorsList")).Split(':');


                //Step-1
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domaintab = login.Navigate<DomainManagement>();
                domaintab.SetupStudyQueryParameters(domainame, new QueryParamters[] { QueryParamters.FullName });
                ExecutedSteps++;

                //Step-2
                login.Logout();
                login.LoginIConnect(user2, user2);
                var inbounds = login.Navigate<Inbounds>();
                inbounds.SearchStudy("Accession", sharedstudy);
                var study7_1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { sharedstudy, "Shared" });
                inbounds.SearchStudy("Accession", uploadedstudy);
                var study7_2 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { uploadedstudy, "Uploaded" });
                if (study7_1 != null && study7_2 != null)
                {
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
                inbounds.SearchStudy("Accession", sharedstudy);
                var viewer = BluRingViewer.LaunchBluRingViewer("Inbounds", "Accession", sharedstudy);
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors)
                    && viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4 -- Open a prior from U2 Datasource
                viewer.OpenPriors(accession: accession2);
                viewer.SetViewPort(0, 2);
                var step4 = result.steps[++ExecutedSteps];
                step4.SetPath(testid, ExecutedSteps);
                var isImageCompare4 = viewer.CompareImage(step4, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare4)
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

                //Step-5 -- Opening Study uploaded in Holding pen
                viewer.OpenPriors(accession: uploadedstudy);
                var step5 = result.steps[++ExecutedSteps];
                step5.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 3);
                var isImageCompare5 = viewer.CompareImage(step5, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare5)
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
                var studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", accession2);
                studies.SelectStudy("Accession", accession2);
                BluRingViewer.LaunchBluRingViewer();
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors) &&
                    viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7 -- Opening the Shared Study from exam list
                viewer.OpenPriors(accession: sharedstudy);
                var step7 = result.steps[++ExecutedSteps];
                step7.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 2);
                var isImageCompare7 = viewer.CompareImage(step7, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare7)
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

                //Step-8 -- Open the study uploaded in Holding pen
                viewer.OpenPriors(accession: uploadedstudy);
                var step8 = result.steps[++ExecutedSteps];
                step8.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 3);
                var isImageCompare8 = viewer.CompareImage(step8, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare8)
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
                var outbounds = login.Navigate<Outbounds>();
                outbounds.SearchStudy("Accession", uploadedstudy);
                viewer = BluRingViewer.LaunchBluRingViewer("OutBounds", "Accession", uploadedstudy);
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors) &&
                    viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10 -- Loading shared prior
                viewer.OpenPriors(accession: sharedstudy);
                viewer.SetViewPort(0, 2);
                var step10 = result.steps[++ExecutedSteps];
                step10.SetPath(testid, ExecutedSteps);
                var isImageCompare10 = viewer.CompareImage(step10, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare10)
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


                //Step-11 -- Open a prior that belongs to U2
                viewer.OpenPriors(accession: accession2);
                viewer.SetViewPort(0, 3);
                var step11 = result.steps[++ExecutedSteps];
                step11.SetPath(testid, ExecutedSteps);
                var isImageCompare11 = viewer.CompareImage(step11, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare11)
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
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domaintab = login.Navigate<DomainManagement>();
                domaintab.SetupStudyQueryParameters(domainame, new QueryParamters[] { QueryParamters.PatientID, QueryParamters.FullName });
                login.Logout();
            }

        }

        /// <summary>
        /// Test 139006 3- Viewing priors with Query Related Study Parameters Applied - Patient ID
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161645(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            String domainame = String.Empty;
            int ExecutedSteps = -1;

            try
            {
                var patientID = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split(':');
                String priorscount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String accession2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                var datasourcelist = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList")).Split(':');
                domainame = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String priorcount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String user1 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "User1");
                String user2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "User2");
                String sharedstudy = accession.Split('=')[1].Split(':')[0];
                String uploadedstudy = accession.Split('=')[0].Split(':')[0];
                var arrPriorscount = priorcount.Split(':');
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                var user1Priors = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "U1PriorsList")).Split(':');
                var user2Priors = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "U2PriorsList")).Split(':');


                //Step-1
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domaintab = login.Navigate<DomainManagement>();
                domaintab.SetupStudyQueryParameters(domainame, new QueryParamters[] { QueryParamters.PatientID });
                ExecutedSteps++;

                //Step-2 -- Performed as part of precondition
                ExecutedSteps++;

                //Step-3
                login.Logout();
                login.LoginIConnect(user1, user1);
                var outbounds = login.Navigate<Outbounds>();
                outbounds.SearchStudy("Accession", sharedstudy);
                var viewer = BluRingViewer.LaunchBluRingViewer("OutBounds", "Accession", sharedstudy);
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[0]) && viewer.CheckAccession_ExamList(user1Priors))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4
                login.Logout();
                login.LoginIConnect(user2, user2);
                var inbounds = login.Navigate<Inbounds>();
                inbounds.SearchStudy("Accession", sharedstudy);
                var study4_1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { sharedstudy, "Shared" });
                inbounds.SearchStudy("Accession", uploadedstudy);
                var study4_2 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { uploadedstudy, "Uploaded" });
                if (study4_1 != null && study4_2 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5 
                inbounds.SearchStudy("Accession", sharedstudy);
                viewer = BluRingViewer.LaunchBluRingViewer("Inbounds", "Accession", sharedstudy);
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors) &&
                    viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6 -- Open a prior from U2 Datasource
                viewer.OpenPriors(accession: accession2);
                viewer.SetViewPort(0, 2);
                var step6 = result.steps[++ExecutedSteps];
                step6.SetPath(testid, ExecutedSteps);
                var isImageCompare6 = viewer.CompareImage(step6, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare6)
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

                //Step-7 -- Opening Study uploaded in Holding pen
                viewer.OpenPriors(accession: uploadedstudy);
                var step7 = result.steps[++ExecutedSteps];
                step7.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 3);
                var isImageCompare7 = viewer.CompareImage(step7, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare7)
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
                var studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", accession2);
                studies.SelectStudy("Accession", accession2);
                BluRingViewer.LaunchBluRingViewer();
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors) &&
                    viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9 -- Opening the Shared Study from exam list
                viewer.OpenPriors(accession: sharedstudy);
                var step9 = result.steps[++ExecutedSteps];
                step9.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 2);
                var isImageCompare9 = viewer.CompareImage(step9, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare9)
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

                //Step-10 -- Open the study uploaded in Holding pen
                viewer.OpenPriors(accession: uploadedstudy);
                var step10 = result.steps[++ExecutedSteps];
                step10.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 3);
                var isImageCompare10 = viewer.CompareImage(step10, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare10)
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
                outbounds = login.Navigate<Outbounds>();
                outbounds.SearchStudy("Accession", uploadedstudy);
                viewer = BluRingViewer.LaunchBluRingViewer("OutBounds", "Accession", uploadedstudy);
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors) &&
                    viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12 -- Loading shared prior
                viewer.OpenPriors(accession: sharedstudy);
                viewer.SetViewPort(0, 2);
                var step12 = result.steps[++ExecutedSteps];
                step12.SetPath(testid, ExecutedSteps);
                var isImageCompare12 = viewer.CompareImage(step12, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare12)
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


                //Step-13 -- Open a prior that belongs to U2
                viewer.OpenPriors(accession: accession2);
                viewer.SetViewPort(0, 3);
                var step13 = result.steps[++ExecutedSteps];
                step13.SetPath(testid, ExecutedSteps);
                var isImageCompare13 = viewer.CompareImage(step13, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare13)
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
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domaintab = login.Navigate<DomainManagement>();
                domaintab.SetupStudyQueryParameters(domainame, new QueryParamters[] { QueryParamters.PatientID, QueryParamters.FullName });
                login.Logout();
            }



        }

        /// <summary>
        /// Test 139007 - 4-Viewing priors with Query Related Study Parameters Applied - Patient ID and Last Name
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161646(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            String domainame = String.Empty;
            int ExecutedSteps = -1;

            try
            {
                var patientID = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split(':');
                String priorscount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String accession2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                var datasourcelist = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList")).Split(':');
                domainame = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String priorcount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String priordate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorDate");
                String priormodality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorModality");
                String user1 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "User1");
                String user2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "User2");
                var user1Priors = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "U1PriorsList")).Split(':');
                var user2Priors = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "U2PriorsList")).Split(':');
                String sharedstudy = accession.Split('=')[1].Split(':')[0];
                String uploadedstudy = accession.Split('=')[0].Split(':')[0];
                var arrPriorscount = priorcount.Split(':');
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);


                //Step-1
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domaintab = login.Navigate<DomainManagement>();
                domaintab.SetupStudyQueryParameters(domainame, new QueryParamters[] { QueryParamters.PatientID, QueryParamters.LastName });
                ExecutedSteps++;

                //Step-2 -- Performed as part of precondition
                ExecutedSteps++;

                //Step-3
                login.Logout();
                login.LoginIConnect(user1, user1);
                var outbounds = login.Navigate<Outbounds>();
                outbounds.SearchStudy("Accession", sharedstudy);
                var viewer = BluRingViewer.LaunchBluRingViewer("OutBounds", "Accession", sharedstudy);
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[0]) && viewer.CheckAccession_ExamList(user1Priors))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4
                login.Logout();
                login.LoginIConnect(user2, user2);
                var inbounds = login.Navigate<Inbounds>();
                inbounds.SearchStudy("Accession", sharedstudy);
                var study4_1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { sharedstudy, "Shared" });
                inbounds.SearchStudy("Accession", uploadedstudy);
                var study4_2 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { uploadedstudy, "Uploaded" });
                if (study4_1 != null && study4_2 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5
                inbounds.SearchStudy("Accession", sharedstudy);
                viewer = BluRingViewer.LaunchBluRingViewer("Inbounds", "Accession", sharedstudy);
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors) &&
                    viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6 -- Open a prior from U2 Datasource
                viewer.OpenPriors(accession: accession2);
                viewer.SetViewPort(0, 2);
                var step6 = result.steps[++ExecutedSteps];
                step6.SetPath(testid, ExecutedSteps);
                var isImageCompare6 = viewer.CompareImage(step6, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare6)
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

                //Step-7 -- Opening Study uploaded in Holding pen
                viewer.OpenPriors(accession: uploadedstudy);
                var step7 = result.steps[++ExecutedSteps];
                step7.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 2);
                var isImageCompare7 = viewer.CompareImage(step7, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare7)
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
                var studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", accession2);
                studies.SelectStudy("Accession", accession2);
                BluRingViewer.LaunchBluRingViewer();
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors) &&
                    viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9 -- Opening the Shared Study from exam list
                viewer.OpenPriors(accession: sharedstudy);
                var step9 = result.steps[++ExecutedSteps];
                step9.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 2);
                var isImageCompare9 = viewer.CompareImage(step9, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare9)
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

                //Step-10 -- Open the study uploaded in Holding pen
                viewer.OpenPriors(accession: uploadedstudy);
                var step10 = result.steps[++ExecutedSteps];
                step10.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 3);
                var isImageCompare10 = viewer.CompareImage(step10, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare10)
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
                outbounds = login.Navigate<Outbounds>();
                outbounds.SearchStudy("Accession", uploadedstudy);
                viewer = BluRingViewer.LaunchBluRingViewer("OutBounds", "Accession", uploadedstudy);
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors) &&
                    viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12 -- Loading shared prior
                viewer.OpenPriors(accession: sharedstudy);
                viewer.SetViewPort(0, 2);
                var step12 = result.steps[++ExecutedSteps];
                step12.SetPath(testid, ExecutedSteps);
                var isImageCompare12 = viewer.CompareImage(step12, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare12)
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


                //Step-13 -- Open a prior that belongs to U2
                viewer.OpenPriors(accession: accession2);
                viewer.SetViewPort(0, 3);
                var step13 = result.steps[++ExecutedSteps];
                step13.SetPath(testid, ExecutedSteps);
                var isImageCompare13 = viewer.CompareImage(step13, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare13)
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
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domaintab = login.Navigate<DomainManagement>();
                domaintab.SetupStudyQueryParameters(domainame, new QueryParamters[] { QueryParamters.PatientID, QueryParamters.FullName });
                login.Logout();
            }

        }

        /// <summary>
        /// Test 139008 5-Viewing priors with Query Related Study Parameters Applied - Patient ID and FULL Name 
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161647(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            String domainame = String.Empty;
            int ExecutedSteps = -1;

            try
            {
                var patientID = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split(':');
                String priorscount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String accession2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                var datasourcelist = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList")).Split(':');
                domainame = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String priorcount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String user1 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "User1");
                String user2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "User2");
                var user1Priors = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "U1PriorsList")).Split(':');
                var user2Priors = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "U2PriorsList")).Split(':');
                String sharedstudy = accession.Split('=')[1].Split(':')[0];
                String uploadedstudy = accession.Split('=')[0].Split(':')[0];
                var arrPriorscount = priorcount.Split(':');
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);


                //Step-1
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domaintab = login.Navigate<DomainManagement>();
                domaintab.SetupStudyQueryParameters(domainame, new QueryParamters[] { QueryParamters.FullName, QueryParamters.PatientID });
                ExecutedSteps++;

                //Step-2
                login.Logout();
                login.LoginIConnect(user2, user2);
                var inbounds = login.Navigate<Inbounds>();
                inbounds.SearchStudy("Accession", sharedstudy);
                var study7_1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { sharedstudy, "Shared" });
                inbounds.SearchStudy("Accession", uploadedstudy);
                var study7_2 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { uploadedstudy, "Uploaded" });
                if (study7_1 != null && study7_2 != null)
                {
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
                inbounds.SearchStudy("Accession", sharedstudy);
                var viewer = BluRingViewer.LaunchBluRingViewer("Inbounds", "Accession", sharedstudy);
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors) &&
                    viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4 -- Open a prior from U2 Datasource
                viewer.OpenPriors(accession: accession2);
                viewer.SetViewPort(0, 2);
                var step4 = result.steps[++ExecutedSteps];
                step4.SetPath(testid, ExecutedSteps);
                var isImageCompare4 = viewer.CompareImage(step4, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare4)
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

                //Step-5 -- Opening Study uploaded in Holding pen
                viewer.OpenPriors(accession: uploadedstudy);
                var step5 = result.steps[++ExecutedSteps];
                step5.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 3);
                var isImageCompare5 = viewer.CompareImage(step5, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare5)
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
                var studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", accession2);
                studies.SelectStudy("Accession", accession2);
                BluRingViewer.LaunchBluRingViewer();
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors) &&
                    viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7 -- Opening the Shared Study from exam list
                viewer.OpenPriors(accession: sharedstudy);
                var step7 = result.steps[++ExecutedSteps];
                step7.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 2);
                var isImageCompare7 = viewer.CompareImage(step7, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare7)
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

                //Step-8 -- Open the study uploaded in Holding pen
                viewer.OpenPriors(accession: uploadedstudy);
                var step8 = result.steps[++ExecutedSteps];
                step8.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 3);
                var isImageCompare8 = viewer.CompareImage(step8, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare8)
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
                var outbounds = login.Navigate<Outbounds>();
                outbounds.SearchStudy("Accession", uploadedstudy);
                viewer = BluRingViewer.LaunchBluRingViewer("OutBounds", "Accession", uploadedstudy);
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors) &&
                    viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10 -- Loading shared prior
                viewer.OpenPriors(accession: sharedstudy);
                viewer.SetViewPort(0, 2);
                var step10 = result.steps[++ExecutedSteps];
                step10.SetPath(testid, ExecutedSteps);
                var isImageCompare10 = viewer.CompareImage(step10, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare10)
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


                //Step-11 -- Open a prior that belongs to U2
                viewer.OpenPriors(accession: accession2);
                viewer.SetViewPort(0, 3);
                var step11 = result.steps[++ExecutedSteps];
                step11.SetPath(testid, ExecutedSteps);
                var isImageCompare11 = viewer.CompareImage(step11, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare11)
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
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domaintab = login.Navigate<DomainManagement>();
                domaintab.SetupStudyQueryParameters(domainame, new QueryParamters[] { QueryParamters.PatientID, QueryParamters.FullName });
                login.Logout();
            }

        }

        /// <summary>
        /// Test 139009 6-Viewing priors with Query Related Study Parameters Applied - Patient ID and DOB  
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161648(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            String domainame = String.Empty;
            int ExecutedSteps = -1;

            try
            {
                var patientID = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split(':');
                String priorscount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String accession2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                var datasourcelist = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList")).Split(':');
                domainame = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String priorcount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String user1 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "User1");
                String user2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "User2");
                String sharedstudy = accession.Split('=')[1].Split(':')[0];
                String uploadedstudy = accession.Split('=')[0].Split(':')[0];
                var user1Priors = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "U1PriorsList")).Split(':');
                var user2Priors = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "U2PriorsList")).Split(':');
                var arrPriorscount = priorcount.Split(':');
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);


                //Step-1
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domaintab = login.Navigate<DomainManagement>();
                domaintab.SetupStudyQueryParameters(domainame, new QueryParamters[] { QueryParamters.PatientDOB, QueryParamters.PatientID });
                ExecutedSteps++;

                //Step-2
                login.Logout();
                login.LoginIConnect(user2, user2);
                var inbounds = login.Navigate<Inbounds>();
                inbounds.SearchStudy("Accession", sharedstudy);
                var study7_1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { sharedstudy, "Shared" });
                inbounds.SearchStudy("Accession", uploadedstudy);
                var study7_2 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { uploadedstudy, "Uploaded" });
                if (study7_1 != null && study7_2 != null)
                {
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
                inbounds.SearchStudy("Accession", sharedstudy);
                var viewer = BluRingViewer.LaunchBluRingViewer("Inbounds", "Accession", sharedstudy);
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors)
                    && viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4 -- Open a prior from U2 Datasource
                viewer.OpenPriors(accession: accession2);
                viewer.SetViewPort(0, 2);
                var step4 = result.steps[++ExecutedSteps];
                step4.SetPath(testid, ExecutedSteps);
                var isImageCompare4 = viewer.CompareImage(step4, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare4)
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

                //Step-5 -- Opening Study uploaded in Holding pen
                viewer.OpenPriors(accession: uploadedstudy);
                var step5 = result.steps[++ExecutedSteps];
                step5.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 3);
                var isImageCompare5 = viewer.CompareImage(step5, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare5)
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
                var studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", accession2);
                studies.SelectStudy("Accession", accession2);
                BluRingViewer.LaunchBluRingViewer();
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors) &&
                    viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7 -- Opening the Shared Study from exam list
                viewer.OpenPriors(accession: sharedstudy);
                var step7 = result.steps[++ExecutedSteps];
                step7.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 2);
                var isImageCompare7 = viewer.CompareImage(step7, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare7)
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

                //Step-8 -- Open the study uploaded in Holding pen
                viewer.OpenPriors(accession: uploadedstudy);
                var step8 = result.steps[++ExecutedSteps];
                step8.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 3);
                var isImageCompare8 = viewer.CompareImage(step8, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare8)
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
                var outbounds = login.Navigate<Outbounds>();
                outbounds.SearchStudy("Accession", uploadedstudy);
                viewer = BluRingViewer.LaunchBluRingViewer("OutBounds", "Accession", uploadedstudy);
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors) &&
                    viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10 -- Loading shared prior
                viewer.OpenPriors(accession: sharedstudy);
                viewer.SetViewPort(0, 2);
                var step10 = result.steps[++ExecutedSteps];
                step10.SetPath(testid, ExecutedSteps);
                var isImageCompare10 = viewer.CompareImage(step10, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare10)
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


                //Step-11 -- Open a prior that belongs to U2
                viewer.OpenPriors(accession: accession2);
                viewer.SetViewPort(0, 3);
                var step11 = result.steps[++ExecutedSteps];
                step11.SetPath(testid, ExecutedSteps);
                var isImageCompare11 = viewer.CompareImage(step11, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare11)
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
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domaintab = login.Navigate<DomainManagement>();
                domaintab.SetupStudyQueryParameters(domainame, new QueryParamters[] { QueryParamters.PatientID, QueryParamters.FullName });
                login.Logout();
            }

        }

        /// <summary>
        /// Test 139010 7-Viewing priors with Query Related Study Parameters Applied - Patient ID and Issuer of Patient ID (IPID)  
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161649(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            String domainame = String.Empty;
            int ExecutedSteps = -1;

            try
            {
                var patientID = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split(':');
                String priorscount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String accession2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                var datasourcelist = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList")).Split(':');
                domainame = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String priorcount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String user1 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "User1");
                String user2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "User2");
                var user1Priors = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "U1PriorsList")).Split(':');
                var user2Priors = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "U2PriorsList")).Split(':');
                String sharedstudy = accession.Split('=')[1].Split(':')[0];
                String uploadedstudy = accession.Split('=')[0].Split(':')[0];
                var arrPriorscount = priorcount.Split(':');
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);


                //Step-1
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domaintab = login.Navigate<DomainManagement>();
                domaintab.SetupStudyQueryParameters(domainame, new QueryParamters[] { QueryParamters.PatientID, QueryParamters.IPID });
                ExecutedSteps++;

                //Step-2
                login.Logout();
                login.LoginIConnect(user2, user2);
                var inbounds = login.Navigate<Inbounds>();
                inbounds.SearchStudy("Accession", sharedstudy);
                var study7_1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { sharedstudy, "Shared" });
                inbounds.SearchStudy("Accession", uploadedstudy);
                var study7_2 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { uploadedstudy, "Uploaded" });
                if (study7_1 != null && study7_2 != null)
                {
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
                inbounds.SearchStudy("Accession", sharedstudy);
                var viewer = BluRingViewer.LaunchBluRingViewer("Inbounds", "Accession", sharedstudy);
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors) &&
                    viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4 -- Open a prior from U2 Datasource
                viewer.OpenPriors(accession: accession2);
                viewer.SetViewPort(0, 2);
                var step4 = result.steps[++ExecutedSteps];
                step4.SetPath(testid, ExecutedSteps);
                var isImageCompare4 = viewer.CompareImage(step4, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare4)
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

                //Step-5 -- Opening Study uploaded in Holding pen
                viewer.OpenPriors(accession: uploadedstudy);
                var step5 = result.steps[++ExecutedSteps];
                step5.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 3);
                var isImageCompare5 = viewer.CompareImage(step5, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare5)
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
                var studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", accession2);
                studies.SelectStudy("Accession", accession2);
                BluRingViewer.LaunchBluRingViewer();
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors) &&
                    viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7 -- Opening the Shared Study from exam list
                viewer.OpenPriors(accession: sharedstudy);
                var step7 = result.steps[++ExecutedSteps];
                step7.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 2);
                var isImageCompare7 = viewer.CompareImage(step7, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare7)
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

                //Step-8 -- Open the study uploaded in Holding pen
                viewer.OpenPriors(accession: uploadedstudy);
                var step8 = result.steps[++ExecutedSteps];
                step8.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 3);
                var isImageCompare8 = viewer.CompareImage(step8, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare8)
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
                var outbounds = login.Navigate<Outbounds>();
                outbounds.SearchStudy("Accession", uploadedstudy);
                viewer = BluRingViewer.LaunchBluRingViewer("OutBounds", "Accession", uploadedstudy);
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors) &&
                    viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10 -- Loading shared prior
                viewer.OpenPriors(accession: sharedstudy);
                viewer.SetViewPort(0, 2);
                var step10 = result.steps[++ExecutedSteps];
                step10.SetPath(testid, ExecutedSteps);
                var isImageCompare10 = viewer.CompareImage(step10, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare10)
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


                //Step-11 -- Open a prior that belongs to U2
                viewer.OpenPriors(accession: accession2);
                viewer.SetViewPort(0, 3);
                var step11 = result.steps[++ExecutedSteps];
                step11.SetPath(testid, ExecutedSteps);
                var isImageCompare11 = viewer.CompareImage(step11, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare11)
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
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domaintab = login.Navigate<DomainManagement>();
                domaintab.SetupStudyQueryParameters(domainame, new QueryParamters[] { QueryParamters.PatientID, QueryParamters.FullName });
                login.Logout();
            }

        }

        /// <summary>
        /// Test 139011 8-Viewing priors with Query Related Study Parameters Applied - Patient Last Name and Patient DOB 
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161650(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            String domainame = String.Empty;

            try
            {
                var patientID = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split(':');
                String priorscount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String accession2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                var datasourcelist = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList")).Split(':');
                domainame = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String priorcount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String user1 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "User1");
                String user2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "User2");
                var user1Priors = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "U1PriorsList")).Split(':');
                var user2Priors = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "U2PriorsList")).Split(':');
                String sharedstudy = accession.Split('=')[1].Split(':')[0];
                String uploadedstudy = accession.Split('=')[0].Split(':')[0];
                var arrPriorscount = priorcount.Split(':');
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);


                //Step-1
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domaintab = login.Navigate<DomainManagement>();
                domaintab.SetupStudyQueryParameters(domainame, new QueryParamters[] { QueryParamters.PatientDOB, QueryParamters.LastName });
                ExecutedSteps++;

                //Step-2
                login.Logout();
                login.LoginIConnect(user2, user2);
                var inbounds = login.Navigate<Inbounds>();
                inbounds.SearchStudy("Accession", sharedstudy);
                var study7_1 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { sharedstudy, "Shared" });
                inbounds.SearchStudy("Accession", uploadedstudy);
                var study7_2 = inbounds.GetMatchingRow(new string[] { "Accession", "Status" }, new string[] { uploadedstudy, "Uploaded" });
                if (study7_1 != null && study7_2 != null)
                {
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
                inbounds.SearchStudy("Accession", sharedstudy);
                var viewer = BluRingViewer.LaunchBluRingViewer("Inbounds", "Accession", sharedstudy);
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors) 
                    && viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4 -- Open a prior from U2 Datasource
                viewer.OpenPriors(accession: accession2);
                viewer.SetViewPort(0, 2);
                var step4 = result.steps[++ExecutedSteps];
                step4.SetPath(testid, ExecutedSteps);
                var isImageCompare4 = viewer.CompareImage(step4, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare4)
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

                //Step-5 -- Opening Study uploaded in Holding pen
                viewer.OpenPriors(accession: uploadedstudy);
                var step5 = result.steps[++ExecutedSteps];
                step5.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 3);
                var isImageCompare5 = viewer.CompareImage(step5, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare5)
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
                var studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", accession2);
                studies.SelectStudy("Accession", accession2);
                BluRingViewer.LaunchBluRingViewer();
                if (viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1]) && viewer.CheckAccession_ExamList(user2Priors)
                    && viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7 -- Opening the Shared Study from exam list
                viewer.OpenPriors(accession: sharedstudy);
                var step7 = result.steps[++ExecutedSteps];
                step7.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 2);
                var isImageCompare7 = viewer.CompareImage(step7, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare7)
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

                //Step-8 -- Open the study uploaded in Holding pen
                viewer.OpenPriors(accession: uploadedstudy);
                var step8 = result.steps[++ExecutedSteps];
                step8.SetPath(testid, ExecutedSteps);
                viewer.SetViewPort(0, 3);
                var isImageCompare8 = viewer.CompareImage(step8, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare8)
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
                var outbounds = login.Navigate<Outbounds>();
                outbounds.SearchStudy("Accession", uploadedstudy);
                viewer = BluRingViewer.LaunchBluRingViewer("OutBounds", "Accession", uploadedstudy);
                if ((viewer.CheckPriorsCount() == Int32.Parse(arrPriorscount[1])) && viewer.CheckAccession_ExamList(user2Priors)
                    && viewer.IsForeignExamAlert(uploadedstudy))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10 -- Loading shared prior
                viewer.OpenPriors(accession: sharedstudy);
                viewer.SetViewPort(0, 2);
                var step10 = result.steps[++ExecutedSteps];
                step10.SetPath(testid, ExecutedSteps);
                var isImageCompare10 = viewer.CompareImage(step10, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (isImageCompare10)
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


                //Step-11 -- Open a prior that belongs to U2
                viewer.OpenPriors(accession: accession2);
                viewer.SetViewPort(0, 3);
                var step11 = result.steps[++ExecutedSteps];
                step11.SetPath(testid, ExecutedSteps);
                var isImageCompare11 = viewer.CompareImage(step11, viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                viewer.CloseBluRingViewer();
                if (isImageCompare11)
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
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domaintab = login.Navigate<DomainManagement>();
                domaintab.SetupStudyQueryParameters(domainame, new QueryParamters[] { QueryParamters.PatientDOB, QueryParamters.LastName });
            }


        }

        /// <summary>
        /// Load Study Priors 2- Query Related Study Parameters - Viewing Priors with Combination of various queries
        /// </summary>
        public TestCaseResult Test_161640(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] arrAccession = accession.Split(':');
                String expPriorCounts = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String[] arrExpPriorCounts = expPriorCounts.Split(':');


                //Objects
                BluRingViewer viewer = null;
                DomainManagement domainmanagement = null;
                Studies studies = null;
                IWebElement viewport;

                //Step 1 - Navigate to Domain Manage ment > Edit > checkmark Query Related Study Parameter: Patient Full Name
                login.LoginIConnect(adminUserName, adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SetupStudyQueryParameters(Config.adminGroupName, new QueryParamters[] { QueryParamters.FullName });
                ExecutedSteps++;

                //Step 2 - Search and Load a study in BR Viewer, 
                //Study is load on to BR Viewer and image displays in the viewport
                //Exam list Panel displays priors from 2 data souces
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", arrAccession[0]);
                studies.SelectStudy("Accession", arrAccession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                ExecutedSteps++;

                //Step 3 - Current study and prior study from all available data
                //source listed: which only patient Full Name match.
                int actualPriorCount = viewer.CheckPriorsCount();
                if (actualPriorCount == Int32.Parse(arrExpPriorCounts[0]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 3 - Click on the Prior from the Exam List Panel. Selected Prior study loads to the second study viewer Port.
                viewer.OpenPriors(accession: arrAccession[3]);
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                viewport = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[1];
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps);
                var step4 = viewer.CompareImage(result.steps[ExecutedSteps], viewport);
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

                //Step 4 - Close one of the prior
                viewer.CloseStudypanel(1);
                ExecutedSteps++;

                //Step 5 - Close viewer
                viewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Step 6 - Navigate to Domain Management > Edit >
                //configure / checkmark Query Related Study Parameter:Patient Last Name only
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SetupStudyQueryParameters(Config.adminGroupName, new QueryParamters[] { QueryParamters.LastName });
                ExecutedSteps++;

                //Step 7 - Search and Load a study in BR Viewr, which has prior study on at least two different data source. 
                //Prior count according to Last name only
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", arrAccession[0]);
                studies.SelectStudy("Accession", arrAccession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                actualPriorCount = viewer.CheckPriorsCount();
                if (actualPriorCount == Int32.Parse(arrExpPriorCounts[1]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8 - Click one of prior studies from the Exam List Panel. Selected Prior is loaded to the second study viewer port Thumbnail and image display correct study.
                viewer.OpenPriors(accession: arrAccession[2]);
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                viewport = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[1];
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                var step8 = viewer.CompareImage(result.steps[ExecutedSteps], viewport);
                if (step8)
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

                //Step 9 - Close viewer
                viewer.CloseBluRingViewer();
                ExecutedSteps++;

                //Step 10 - In Domain Management, checkmark Query Related Study Parameter: Patient Last Name only do not save domain
                //Patient's last Name is selected, No error message displayed
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(Config.adminGroupName);
                domainmanagement.SelectDomain(Config.adminGroupName);
                domainmanagement.ClickEditDomain();
                domainmanagement.ScrollIntoView(domainmanagement.PatientFullnameCheckbox());
                domainmanagement.SetCheckbox(domainmanagement.PatientLastnameCheckbox(), true);
                PageLoadWait.WaitForFrameLoad(60);
                ExecutedSteps++;

                //Step 11 - select Parameter: Patient Full Name only , Patient's Last name gets unselected
                domainmanagement.SetCheckbox(domainmanagement.PatientFullnameCheckbox(), true);
                PageLoadWait.WaitForFrameLoad(60);
                if (!domainmanagement.PatientLastnameCheckbox().Selected)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12 - In Domain Management, checkmark Query Related Study Parameter: Patient ID and last name
                //Launch study and see if the priors are listed accordingly
                domainmanagement.SetCheckbox(domainmanagement.PatientLastnameCheckbox(), true);
                domainmanagement.SetCheckbox(domainmanagement.PatientIDCheckbox(), true);
                domainmanagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", arrAccession[0]);
                studies.SelectStudy("Accession", arrAccession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                actualPriorCount = viewer.CheckPriorsCount();
                ExecutedSteps++;
                viewer.OpenPriors(accession: arrAccession[2]);
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                viewport = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[1];
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps);
                var step12 = viewer.CompareImage(result.steps[ExecutedSteps], viewport);
                if ((actualPriorCount == Int32.Parse(arrExpPriorCounts[2])) && step12)
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
                viewer.CloseBluRingViewer();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);

                //Step 13 - In Domain Management, checkmark Query Related Study Parameter: Patient ID and patient DOB
                //Launch study and see if the priors are listed accordingly
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SetupStudyQueryParameters(Config.adminGroupName, new
                    QueryParamters[] { QueryParamters.PatientID, QueryParamters.PatientDOB });
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", arrAccession[0]);
                studies.SelectStudy("Accession", arrAccession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                actualPriorCount = viewer.CheckPriorsCount();
                ExecutedSteps++;
                viewer.OpenPriors(accession: arrAccession[2]);
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                viewport = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[1];
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps);
                var step13 = viewer.CompareImage(result.steps[ExecutedSteps], viewport);
                if ((actualPriorCount == Int32.Parse(arrExpPriorCounts[3])) && step13)
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
                viewer.CloseBluRingViewer();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);

                //Step 14 - In Domain Management, checkmark Query Related Study Parameter: Patient ID and IPID
                //Launch study and see if the priors are listed accordingly
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SetupStudyQueryParameters(Config.adminGroupName, 
                    new QueryParamters[] { QueryParamters.PatientID, QueryParamters.PatientDOB });
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", arrAccession[0]);
                studies.SelectStudy("Accession", arrAccession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                actualPriorCount = viewer.CheckPriorsCount();
                ExecutedSteps++;
                viewer.OpenPriors(accession: arrAccession[2]);
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                viewport = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[1];
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps);
                var step14 = viewer.CompareImage(result.steps[ExecutedSteps], viewport);
                if ((actualPriorCount == Int32.Parse(arrExpPriorCounts[4])) && step14)
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
                viewer.CloseBluRingViewer();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);

                //Step 15 - In Domain Management, checkmark Query Related Study Parameter: Patient DOB and Full Name
                //Launch study and see if the priors are listed accordingly
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SetupStudyQueryParameters(Config.adminGroupName, new QueryParamters[] 
                { QueryParamters.PatientDOB, QueryParamters.FullName });
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", arrAccession[3]);
                studies.SelectStudy("Accession", arrAccession[3]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                actualPriorCount = viewer.CheckPriorsCount();
                ExecutedSteps++;
                viewer.OpenPriors(accession: arrAccession[0]);
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                viewport = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[1];
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps);
                var step15 = viewer.CompareImage(result.steps[ExecutedSteps], viewport);
                if ((actualPriorCount == Int32.Parse(arrExpPriorCounts[5])) && step15)
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
                viewer.CloseBluRingViewer();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);

                //Step 16 - In Domain Management, checkmark Query Related Study Parameter: Full Name and IPID
                //Launch study and see if the priors are listed accordingly
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SetupStudyQueryParameters(Config.adminGroupName,
                    new QueryParamters[] { QueryParamters.IPID, QueryParamters.FullName });
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", arrAccession[1]);
                studies.SelectStudy("Accession", arrAccession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                actualPriorCount = viewer.CheckPriorsCount();
                ExecutedSteps++;
                viewer.OpenPriors(accession: arrAccession[3]);
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                viewport = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[1];
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps);
                var step16 = viewer.CompareImage(result.steps[ExecutedSteps], viewport);
                if ((actualPriorCount == Int32.Parse(arrExpPriorCounts[6])) && step16)
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
                viewer.CloseBluRingViewer();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);

                //Step 17 - In Domain Management, checkmark Query Related Study Parameter: DOB and LastName
                //Launch study and see if the priors are listed accordingly
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SetupStudyQueryParameters(Config.adminGroupName, new QueryParamters[] { QueryParamters.PatientDOB, QueryParamters.LastName });
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", arrAccession[2]);
                studies.SelectStudy("Accession", arrAccession[2]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                actualPriorCount = viewer.CheckPriorsCount();
                ExecutedSteps++;
                viewer.OpenPriors(accession: arrAccession[3]);
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                viewport = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[1];
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps);
                var step17 = viewer.CompareImage(result.steps[ExecutedSteps], viewport);
                if ((actualPriorCount == Int32.Parse(arrExpPriorCounts[7])) && step17)
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
                viewer.CloseBluRingViewer();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);

                //Step 18 - In Domain Management, checkmark Query Related Study Parameter: IPID and LastName
                //Launch study and see if the priors are listed accordingly
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SetupStudyQueryParameters(Config.adminGroupName, new QueryParamters[] 
                { QueryParamters.IPID, QueryParamters.LastName });
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", arrAccession[2]);
                studies.SelectStudy("Accession", arrAccession[2]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                actualPriorCount = viewer.CheckPriorsCount();
                ExecutedSteps++;
                viewer.OpenPriors(accession: arrAccession[0]);
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                viewport = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[1];
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps);
                var step18 = viewer.CompareImage(result.steps[ExecutedSteps], viewport);
                if ((actualPriorCount == Int32.Parse(arrExpPriorCounts[8])) && step18)
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
                viewer.CloseBluRingViewer();

                //Step-19
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SetupStudyQueryParameters(Config.adminGroupName, new QueryParamters[]
               { QueryParamters.IPID, QueryParamters.PatientDOB });
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                if (domainmanagement.ErrorMessage() == "At least Patient ID, Patient Full Name or Patient Last Name is required as query related study parameters.")
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                domainmanagement.EditDomainCloseBtn().Click();

                //Step-20
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SetupStudyQueryParameters(Config.adminGroupName, new QueryParamters[]
               { });
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
                if (domainmanagement.ErrorMessage() == "At least Patient ID, Patient Full Name or Patient Last Name is required as query related study parameters.")
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                domainmanagement.EditDomainCloseBtn().Click();

                //Logout Application
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
                try
                {
                    login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                    DomainManagement domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                    domainmanagement.SetupStudyQueryParameters(Config.adminGroupName, new QueryParamters[] { QueryParamters.FullName, QueryParamters.PatientID });
                    login.Logout();
                }
                catch (Exception e)
                {
                    //Log Exception
                    Logger.Instance.ErrorLog("Error while setting default Query Parameters in finally block");
                    Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }

            }
        }

        /// <summary>
        ///  Test 137309 - Viewing shared priors from Inbound and Outbound page 
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161631(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            RoleManagement rolemanagement = null;
            UserManagement usermanagement = null;
            UserPreferences userpreferences = null;
            string[] PatientID = null;
            string[] Accession = null;
            int[] expPriorCount = null;
            Studies studies = null;
            BluRingViewer viewer = null;
            Inbounds inbounds = null;
            Outbounds outbounds = null;
            string Status = string.Empty;
            string DicomPath = string.Empty;
            string[] FullPath = null;
            try
            {
                PatientID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split('=');
                Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList")).Split('=');
                expPriorCount = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount")).Split('=').Select(epc => Convert.ToInt32(epc)).ToArray();

                //Step 1: Login in iCA as Administrator
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;

                //Step 2: Navigate to Role Management > New Role and Create a role Role1 with only 1 datasource DS1 connected and grant access to anyone
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                string Role1 = BasePage.GetUniqueRole();
                rolemanagement.CreateRole(Config.adminGroupName, Role1, Role1, checkboxes: new string[] { "datadownload", "datatransfer" }, isGrantAccessAnyone: true, datasourcelist: login.GetHostName(Config.EA91));
                if (rolemanagement.RoleExists(Role1, Config.adminGroupName))
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

                //Step 3: Create a role Role2 with only 1 datasource DS2 connected and grant access to anyone
                string Role2 = BasePage.GetUniqueRole();
                rolemanagement.CreateRole(Config.adminGroupName, Role2, Role2, checkboxes: new string[] { "datadownload", "datatransfer" }, isGrantAccessAnyone: true, datasourcelist: login.GetHostName(Config.SanityPACS));
                if (rolemanagement.RoleExists(Role2, Config.adminGroupName))
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

                //Step 4: Navigate to User management > New user and Create a user R1 in Role1
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                string User1 = BasePage.GetUniqueUserId();
                usermanagement.CreateUser(User1, domainName: Config.adminGroupName, roleName: Role1, hasEmail: 1, emailId: Config.emailid);
                if (usermanagement.IsUserExist(User1, Config.adminGroupName))
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

                //Step 5: Create a user R2 in Role2
                string User2 = BasePage.GetUniqueUserId();
                usermanagement.CreateUser(User2, domainName: Config.adminGroupName, roleName: Role2, hasEmail: 1, emailId: Config.emailid);
                if (usermanagement.IsUserExist(User2, Config.adminGroupName))
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

                //Step 6: logout. login in iCA as R1
                login.Logout();
                login.LoginIConnect(User1, User1);
                userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                ExecutedSteps++;

                //Step 7: from studies tab search for a study having priors in DS1 and none in DS2
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(20);
                studies.SearchStudy(patientID: PatientID[0]);
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                studies.ChooseColumns(new string[] { "Patient ID", "Accession" });
                if (studies.CheckStudy("Accession", Accession[0]))
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

                //Step 8: Select a study and Grant access to R2
                studies.SelectStudy("Accession", Accession[0]);
                studies.ShareStudy(false, new string[] { User2 });
                ExecutedSteps++;

                //Step 9: Check outbounds of R1
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("Accession", Accession[0]);
                outbounds.GetMatchingRow("Accession", Accession[0]).TryGetValue("Status", out Status);
                if (string.Equals(Status, "Shared"))
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

                //Step 10: Load the study in BR viewer
                outbounds.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer("OutBounds", "Accession", Accession[0]);
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 11: Verify that all Priors are listed under Exam List Panel
                var priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                int priorcount = priors.Count;
                if (priorcount == expPriorCount[0])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 12: Load a prior from the Exam List panel
                viewer.OpenPriors(1);
                var viewport = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[1];
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewport))
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

                //Step 13: logout. login in iCA as R2
                login.Logout();
                login.LoginIConnect(User2, User2);
                userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                ExecutedSteps++;

                //Step 14: Check inbounds of R2
                Status = string.Empty;
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Accession", Accession[0]);
                inbounds.GetMatchingRow("Accession", Accession[0]).TryGetValue("Status", out Status);
                if (string.Equals(Status, "Shared"))
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

                //Step 15: Load the study in BR viewer
                inbounds.SelectStudy("Accession", Accession[0]);
                viewer = BluRingViewer.LaunchBluRingViewer("Inbounds", "Accession", Accession[0]);
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 16: Verify if Priors are listed under Exam List Panel
                priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                priorcount = priors.Count;
                if (priorcount == expPriorCount[1])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 17: from studies tab search for a study having no priors in DS2 but a prior present in DS1
                viewer.CloseBluRingViewer();
                studies = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(20);
                studies.SearchStudy(patientID: PatientID[1]);
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                studies.ChooseColumns(new string[] { "Patient ID", "Accession" });
                if (studies.CheckStudy("Accession", Accession[1]))
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

                //Step 18: Select the study and Grant access to R1
                studies.SelectStudy("Accession", Accession[1]);
                studies.ShareStudy(false, new string[] { User1 });
                ExecutedSteps++;

                //Step 19: Check outbounds of R2
                Status = string.Empty;
                outbounds = (Outbounds)login.Navigate("Outbounds");
                outbounds.SearchStudy("Accession", Accession[1]);
                outbounds.GetMatchingRow("Accession", Accession[1]).TryGetValue("Status", out Status);
                if (string.Equals(Status, "Shared"))
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

                //Step 20: Load the study in BR viewer
                outbounds.SelectStudy("Accession", Accession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer("Outbounds", "Accession", Accession[1]);
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 21: Verify that no priors are listed under Exam List Panel
                priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                priorcount = priors.Count;
                if (priorcount == expPriorCount[1])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 22: logout. login in iCA as R1
                login.Logout();
                login.LoginIConnect(User1, User1);
                ExecutedSteps++;

                //Step 23: Check inbounds of R1
                Status = string.Empty;
                inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy("Accession", Accession[1]);
                inbounds.GetMatchingRow("Accession", Accession[1]).TryGetValue("Status", out Status);
                if (string.Equals(Status, "Shared"))
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

                //Step 24: Load the study in BR viewer
                inbounds.SelectStudy("Accession", Accession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer("Inbounds", "Accession", Accession[1]);
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step 25: Verify that Priors are listed Under Exam List Panel
                priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                priorcount = priors.Count;
                if (priorcount == expPriorCount[0])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 26: Load a prior from the Exam List panel
                viewer.OpenPriors(1);
                viewport = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[1];
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewport))
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
        ///  Test 137312 - Viewing priors from holding pen 
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161632(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            RoleManagement rolemanagement = null;
            UserManagement usermanagement = null;
            UserPreferences userpreferences = null;
            string Accession = string.Empty;
            int[] expPriorCount = null;
            Studies studies = null;
            BluRingViewer viewer = null;
            string Status = string.Empty;
            string Dest_1 = "Dest-" + new Random().Next(10, 99);
            string StudyPath = string.Empty;
            string UploadFilePath = string.Empty;
            string Datasource = string.Empty;
            string eiWindow = string.Concat("EI_137312_", System.DateTime.Now.ToString("MMddHHmm"));
            string[] FullPath = null;
            try
            {
                Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID"));
                StudyPath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "StudyPath"));
                UploadFilePath = Config.TestDataPath + (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                expPriorCount = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount")).Split('=').Select(epc => Convert.ToInt32(epc)).ToArray();
                Datasource = login.GetHostName(Config.DestEAsIp);
                string staff = BasePage.GetUniqueUserId("St_");
                //PreCondition
                BasePage.Kill_EXEProcess("UploaderTool");
                var client = new DicomClient();
                FullPath = Directory.GetFiles(UploadFilePath, "*.*", SearchOption.AllDirectories);
                foreach (string path in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(path));
                    client.Send(Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);
                }
                //Step 1: Login in iCA as Administrator
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(staff, "SuperAdminGroup", "Staff");
                ExecutedSteps++;
                //Step 2: Create role Role3 with Receiving and Archiving permission and connected to only 1 datasource
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                string Role1 = BasePage.GetUniqueRole();
                rolemanagement.CreateRole(Config.adminGroupName, Role1, Role1, checkboxes: new string[] { "datadownload", "datatransfer", "receiveexam", "archive" }, isGrantAccessAnyone: true, datasourcelist: Datasource);
                if (rolemanagement.RoleExists(Role1, Config.adminGroupName))
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
                //Step 3: Create user Role3 with Role3
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                string User1 = BasePage.GetUniqueUserId();
                usermanagement.CreateUser(User1, domainName: Config.adminGroupName, roleName: Role1, hasEmail: 1, emailId: Config.emailid);
                if (usermanagement.IsUserExist(User1, Config.adminGroupName))
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
                //Step 4: Create destination D1 with receiver and archiver as R3
                Image_Sharing imagesharing = (Image_Sharing)login.Navigate("Image Sharing");
                Image_Sharing.Destination dest = (Image_Sharing.Destination)imagesharing.NavigateToSubTab("Destination");
                //dest.EditDestination(Config.adminGroupName, Dest_1, User1, User1);
                dest.AddDestination(Config.adminGroupName, Dest_1, login.GetHostName(Config.DestEAsIp), User1, User1);
                if (dest.SearchDestination(Config.adminGroupName, Dest_1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Test Step-" + ExecutedSteps);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 5: logout. from service tool generate installer for Exam importer
                login.Logout();
                Taskbar taskbar = new Taskbar();
                taskbar.Hide();
                ServiceTool servicetool = new ServiceTool();
                servicetool.LaunchServiceTool();
                servicetool.GenerateInstallerAllDomain(Config.adminGroupName, eiWindow);
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                taskbar.Show();
                ExecutedSteps++;
                //Step 6: in browser navigate to iCA homepage
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                login = new Login();
                login.DriverGoTo(login.url);
                ExecutedSteps++;
                //Step 7: Click on Install button under Upload study from CD
                //Step 8: run the installer
                //Step 9: User(st) checks the"I accept.."checkbox and clicks Next
                //Step 10: User(U1) Enters iCA credentials (ph/ph) and Selects Registered user then clicks on Install
                //Step 11: User(ph) unchecks the Launch application when setup exits and clicks on Finish
                ei.eiWinName = eiWindow;
                string EIPath = ei.EI_Installation(Config.adminGroupName, eiWindow, Config.Inst1, Config.ph1UserName, Config.ph1Password);
                ExecutedSteps++;
                ExecutedSteps++;
                ExecutedSteps++;
                ExecutedSteps++;
                ExecutedSteps++;
                //Step 12: The user launches the Exam Importer desktop shortcut titled"Send Exam to *^<^*INSTITUTION_NAME*^>^*"on their system.
                //Step 13: Login in Exam importer as R2
                //Step 14: R2 clicks on Don't ask me again button
                //Step 15: User selects path where study is located from Studies From local folder (select study such that it has a prior in DS2)
                //Step 16: User requests Exam Importer to upload the entire CD by clicking on Send
                //Step 17: When progress bar shows 100% complete a pop-up is displayed
                //Step 18: User clicks on OK
                ei.EIDicomUpload(staff, staff, Dest_1, StudyPath, 1, EIPath);
                ExecutedSteps++;
                ExecutedSteps++;
                ExecutedSteps++;
                ExecutedSteps++;
                ExecutedSteps++;
                ExecutedSteps++;
                ExecutedSteps++;
                //Step 19: The user launches browser and accesses the Merge iCA system (http://*^<^*serverip*^>^*/webaccess)
                //Step 20: The user provides valid credentials for the iCA system for the registered physician, R2/R2 and clicks on the Login button
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.ph1UserName, Config.ph1Password);
                userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                ExecutedSteps++;
                ExecutedSteps++;
                //Step 21: From studies tab search for the study whose prior user has uploaded
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession);
                PageLoadWait.WaitForLoadingMessage(60);
                PageLoadWait.WaitForSearchLoad();
                studies.ChooseColumns(new string[] { "Accession" });
                if (studies.CheckStudy("Accession", Accession))
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
                //Step 22: Load the study in BR viewer
                studies.SelectStudy("Accession", Accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;
                //Step 23: load prior study that was uploaded from Exam Importer from Exam list Panel
                var priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                int priorcount = priors.Count;
                if (priorcount == expPriorCount[0])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 24: Load a prior from the Exam List panel
                viewer.OpenPriors(0);
                IWebElement viewport = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel))[1];
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(result.steps[ExecutedSteps], viewport))
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
        ///  Test 138780 - Grant Access to Study with all the priors and verify they are listed on Inbound and Outbound
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161641(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            RoleManagement rolemanagement = null;
            UserManagement usermanagement = null;
            UserPreferences userpreferences = null;
            string[] PatientID = null;
            string[] Accession = null;
            int[] expPriorCount = null;
            Studies studies = null;
            BluRingViewer viewer = null;
            Outbounds outbounds = null;
            Inbounds inbounds = null;
            String domainname = "SuperAdminGroup";

            try
            {
                PatientID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split('=');
                Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList")).Split('=');
                expPriorCount = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount")).Split('=').Select(epc => Convert.ToInt32(epc)).ToArray();

                //Precondition -- Enable Grant Access
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domaintab = login.Navigate<DomainManagement>();
                domaintab.SearchDomain(domainname);
                domaintab.SelectDomain(domainname);
                domaintab.ClickEditDomain();
                domaintab.SetCheckbox(domaintab.GetElement(BasePage.SelectorType.CssSelector, "[id$= '_GrantAccessEnabledCB']"));
                domaintab.ClickSaveEditDomain();
                rolemanagement = login.Navigate<RoleManagement>();
                rolemanagement.SelectDomainfromDropDown(domainname);
                rolemanagement.SearchRole("SuperRole");
                rolemanagement.SelectRole("SuperRole");
                rolemanagement.ClickEditRole();
                rolemanagement.SetRadioButton("cssselector", "[id$='GrantAccessRadioButtonList_2']");
                rolemanagement.ClickSaveEditRole();

                //Step 1: Login in iCA as Administrator  
                string Role1 = BasePage.GetUniqueRole();
                rolemanagement.CreateRole(Config.adminGroupName, Role1, Role1, checkboxes: new string[] { "datadownload", "datatransfer" }, isGrantAccessAnyone: true, datasourcelist: login.GetHostName(Config.EA1));
                if (rolemanagement.RoleExists(Role1, Config.adminGroupName))
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


                //Step 2: Navigate to User management > New user and Create a user R1 in Role1
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                string User1 = BasePage.GetUniqueUserId("User1");
                usermanagement.CreateUser(User1, domainName: Config.adminGroupName, roleName: Role1, hasEmail: 1, emailId: Config.emailid);
                login.Logout();
                login.LoginIConnect(User1, User1);
                userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.BluringViewerRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                ExecutedSteps++;

                //Step-3 - Duplicate **Needs to be removed from Test Case**
                ExecutedSteps++;

                //Step-4
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = login.Navigate<Studies>();
                studies.SearchStudy(AccessionNo: Accession[0], patientID: PatientID[0]);
                studies.SelectStudy("Accession", Accession[0]);
                studies.GrantAccessBtn().Click();
                bool isPriorsDisplayed = false;
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(studies.By_StudyTransferDialogDiv()));
                var rows = BasePage.wait.Until<IList<IWebElement>>(d =>
                {
                    var priorrows = studies.PriorList_GAwinddow();
                    if (priorrows.Count > 4)
                    {
                        return priorrows;
                    }
                    else
                    {
                        return null;
                    }
                });
                foreach (IWebElement row in rows)
                {
                    string accession = row.FindElement(By.CssSelector("td:nth-of-type(5) span")).GetAttribute("innerHTML");
                    if (!Accession.Contains(accession))
                    {
                        isPriorsDisplayed = false;
                        break;
                    }
                    else
                    {
                        isPriorsDisplayed = true;
                    }

                }
                BasePage.Driver.FindElement(studies.By_CloseTransferBtn()).Click();
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(studies.By_StudyTransferDialogDiv()));
                if (isPriorsDisplayed)
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

                //Ste-5 to 8
                studies.ShareStudy(true, new string[] { User1 });
                ExecutedSteps++;
                ExecutedSteps++;
                ExecutedSteps++;
                ExecutedSteps++;

                //Step-9
                outbounds = login.Navigate<Outbounds>();
                outbounds.SearchStudy("PatientID", PatientID[0]);
                ExecutedSteps++;
                foreach (String accession in Accession)
                {
                    if (studies.GetMatchingRow("Accession", accession) != null)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        Logger.Instance.ErrorLog("Study with Accession not found-" + accession);
                        result.steps[ExecutedSteps].SetLogs();
                        break;
                    }
                }

                //Step-10
                viewer = BluRingViewer.LaunchBluRingViewer("Outbounds", "Accession", Accession[0]);
                var step10 = result.steps[++ExecutedSteps];
                step10.SetPath(testid, ExecutedSteps);
                var viewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                if (viewer.CompareImage(step10, viewport))
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


                //Step-11
                if (viewer.CheckPriorsCount() == expPriorCount[0])
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

                //Step-12 
                login.Logout();
                login.LoginIConnect(User1, User1);
                ExecutedSteps++;

                //Step-13
                inbounds = login.Navigate<Inbounds>();
                inbounds.SearchStudy("PatientID", PatientID[0]);
                inbounds.SelectStudy("Accession", Accession[0]);
                ExecutedSteps++;
                foreach (String accession in Accession)
                {
                    if (inbounds.GetMatchingRow("Accession", accession) != null)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        break;
                    }
                }

                //Step-14                
                viewer = BluRingViewer.LaunchBluRingViewer("Outbounds", "Accession", Accession[0]);
                var step14 = result.steps[++ExecutedSteps];
                step14.SetPath(testid, ExecutedSteps);
                viewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                if (viewer.CompareImage(step14, viewport))
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
                if (viewer.CheckPriorsCount() == expPriorCount[0])
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

                //Logout Application                 
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
        /// Test 138781 - Study in Exam List Panel of emailed priors-receipient is able to launch emailed priors
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161642(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                var patientID = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split(':');
                String priorscount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String accession2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                var datasourcelist = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList")).Split(':');
                String domainame = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String priorcount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount");
                String user1 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "User1");
                String user2 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "User2");
                String sharedstudy = accession.Split('=')[1].Split(':')[0];
                String uploadedstudy = accession.Split('=')[0].Split(':')[0];
                var arrPriorscount = priorcount.Split(':');
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                //Step-1
                //Precondition - Refer Test_Precondition.

                //Step-2
                login.LoginIConnect(user2, user2);
                var studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", accession2);
                var viewer = BluRingViewer.LaunchBluRingViewer("OutBounds", "Accession", accession2);
                String pinnumber2 = viewer.EmailStudy_BR();
                if (pinnumber2 != null && (!String.IsNullOrWhiteSpace(pinnumber2)))
                {
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
                String emaillink2 = Pop3EmailUtil.GetEmailedStudyLink(Config.emailid, Config.Email_Password, "", "Emailed Study");
                viewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(emaillink2, pinnumber2);
                var viewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                var step3 = result.steps[++ExecutedSteps];
                if (viewer.CompareImage(step3, viewport))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4
                if (viewer.CheckPriorsCount() == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5 (1 study already shared in precondition)
                login.Logout();
                login.LoginIConnect(user1, user1);
                studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", "");
                studies.SelectStudy("Accession", "");
                studies.ShareStudy(false, new string[] { user2 }, domainName: domainame);
                ExecutedSteps++;

                //Step-6 Email shared Prior
                var outbounds = login.Navigate<Outbounds>();
                outbounds.SearchStudy("Accession", sharedstudy);
                outbounds.SelectStudy("Accession", sharedstudy);
                outbounds.EmailStudy(Config.emailid, "Test", "Test");
                var pinnumber6 = outbounds.FetchPin();
                String emaillink6 = Pop3EmailUtil.GetEmailedStudyLink(Config.emailid, Config.Email_Password, "", "Emailed Study");
                ExecutedSteps++;

                //Step-7 
                viewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(emaillink6, pinnumber6);
                viewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                var step7 = result.steps[++ExecutedSteps];
                if (viewer.CompareImage(step7, viewport))
                {
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
                if (viewer.CheckPriorsCount() == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9
                login.Logout();
                login.LoginIConnect(user2, user2);
                studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", "");
                studies.SelectStudy("Accession", "");
                studies.EmailStudy(Config.emailid, "Test", "Test");
                var pinnumber9 = studies.FetchPin();
                var emaillink9 = Pop3EmailUtil.GetEmailedStudyLink(Config.emailid, Config.Email_Password, "", "Emailed Study");
                ExecutedSteps++;

                //Step-10
                viewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(emaillink9, pinnumber9);
                viewport = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                var step10 = result.steps[++ExecutedSteps];
                if (viewer.CompareImage(step10, viewport))
                {
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
                if (viewer.CheckPriorsCount() == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //sep-12 - Upload 2 priors and do the same    
                String eipath = "C:\\Users\\Administrator\\AppData\\Local\\Apps" + "\\EI" + domainame + "\\bin" + "\\UploaderTool.exe";
                var cduploader = new ExamImporter();
                cduploader.eiWinName = "EI" + domainame;
                cduploader.EIDicomUpload(user2, user2, "", "", path: eipath);
                login.Logout();
                login.LoginIConnect(user2, user2);
                var inbounds = login.Navigate<Inbounds>();
                inbounds.SearchStudy("Accession", "");
                inbounds.SelectStudy("Accession", "");
                inbounds.EmailStudy(Config.emailid, "Test", "Test");
                var pinnumber12 = inbounds.FetchPin();
                var emaillink12 = Pop3EmailUtil.GetEmailedStudyLink(Config.emailid, Config.Email_Password, "", "Emailed Study");
                viewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(emaillink12, pinnumber12);
                if (viewer.CheckPriorsCount() == 1)
                {
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
                login.Navigate<Outbounds>();
                outbounds.SearchStudy("Accession", "");
                outbounds.SelectStudy("Accession", "");
                outbounds.EmailStudy(Config.emailid, "Test", "Test");
                var pinnumber13 = outbounds.FetchPin();
                var emaillink13 = Pop3EmailUtil.GetEmailedStudyLink(Config.emailid, Config.Email_Password, "", "Emailed Study");
                viewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(emaillink13, pinnumber13);
                if (viewer.CheckPriorsCount() == 1)
                {
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
        /// Test 141987 - Acc#: label in Study card tool tip
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161654(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            String domainame = String.Empty;
            int ExecutedSteps = -1;

            try
            {
                var patientID = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID"));
                var name = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name"));
                var dob = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOB"));
                var gender = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "Gender"));
                var ipid = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPID"));
                var studyadte = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorDate"));
                var modality = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorModality"));
                var studydesc = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDesc"));
                var datasource = "PA-A7-WS8";
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                //Step-1
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var studies = login.Navigate<Studies>();
                ExecutedSteps++;

                //Step-2 -- Performed as part of precondition
                studies.SearchStudy(patientID: patientID, Datasource: datasource);
                studies.SelectStudy("Patient ID", patientID);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step-3
                var prior = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors))[0];
                new TestCompleteAction().MoveToElement(prior).Perform();
                var tooltip = prior.GetAttribute("title");
                var expected_tooltip = name + ", " + Environment.NewLine +
                                       "Birthdate: " + dob + "  Sex: " + gender + Environment.NewLine +
                                       "MRN: " + patientID + Environment.NewLine +
                                       "IPID: " + ipid + Environment.NewLine +
                                       Environment.NewLine +
                                       studyadte + Environment.NewLine +
                                       studydesc + Environment.NewLine +
                                       modality + Environment.NewLine +
                                       "Acc#: " + Environment.NewLine +
                                       "DataSource: " + datasource +
                                       Environment.NewLine;
                Logger.Instance.InfoLog("The Expected ToolTip is--" + expected_tooltip);
                Logger.Instance.InfoLog("The Actual ToolTip is--" + tooltip);
                if (tooltip.Equals(expected_tooltip))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4
                var acc_ele = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.AccessionNumberInExamList));
                var acc_text = acc_ele.FindElement(By.CssSelector("label")).GetAttribute("innerHTML");
                if (acc_text.Equals("Acc:"))
                {
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
        /// Test Study Data result set occupy the space of the scroll bar
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161655(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            String domainame = String.Empty;
            int ExecutedSteps = -1;

            try
            {
                var patientID = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID"));
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);


                //Step-1
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var studies = login.Navigate<Studies>();
                ExecutedSteps++;

                //Step-2 -- Performed as part of precondition
                studies.SearchStudy("PatientID", patientID);
                studies.SelectStudy("Patient ID", patientID);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step-3
                var container = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_ContainerPriors);
                var step3 = viewer.IsVerticalScrollBarPresent(container);
                if (!step3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4
                var prior = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_relatedStudyComponent));
                if (prior.Size.Width == 320)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("Prior Size is =" + prior.Size.Width);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5
                var thumbnailiconContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_thumbnailpreviewIconActiveStudy));
                var thumbnailIcon = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_thumbnailIcon));
                var reporticonContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_priorsreportIcon));
                var reportIcon = reporticonContainer.FindElement(By.CssSelector("div"));

                var isThumbnailIconCentered = (thumbnailiconContainer.GetCssValue("padding-top").Equals
                    (thumbnailiconContainer.GetCssValue("padding-bottom"))) &&                  
                    (thumbnailiconContainer.GetCssValue("padding-left").Equals
                    (thumbnailiconContainer.GetCssValue("padding-right"))) &&                   
                    thumbnailIcon.GetCssValue("margin-top").Equals
                    (thumbnailIcon.GetCssValue("margin-bottom")) &&
                    thumbnailIcon.GetCssValue("margin-left").Equals
                    (thumbnailIcon.GetCssValue("margin-right"));
               
                var isReportIconCentered = reporticonContainer.GetCssValue("padding-top").Equals
                  (reporticonContainer.GetCssValue("padding-bottom")) &&
                  reporticonContainer.GetCssValue("padding-left").Equals
                  (reporticonContainer.GetCssValue("padding-right")) &&
                  reportIcon.GetCssValue("margin-top").Equals
                  (reportIcon.GetCssValue("margin-bottom")) &&
                  reportIcon.GetCssValue("margin-left").Equals
                  (reportIcon.GetCssValue("margin-right"));

                if (isThumbnailIconCentered && isReportIconCentered)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Value for thumbnail is");
                    Logger.Instance.InfoLog("padding-top is =" + thumbnailiconContainer.GetCssValue("padding-top"));
                    Logger.Instance.InfoLog("padding-bottom is =" + thumbnailiconContainer.GetCssValue("padding-bottom"));
                    Logger.Instance.InfoLog("padding-left is =" + thumbnailiconContainer.GetCssValue("padding-left"));
                    Logger.Instance.InfoLog("padding-right is =" + thumbnailiconContainer.GetCssValue("padding-right"));
                    Logger.Instance.InfoLog("margin-top is =" + thumbnailIcon.GetCssValue("margin-top"));
                    Logger.Instance.InfoLog("margin-bottom is =" + thumbnailIcon.GetCssValue("margin-bottom"));
                    Logger.Instance.InfoLog("margin-left is =" + thumbnailIcon.GetCssValue("margin-left"));
                    Logger.Instance.InfoLog("margin-right is =" + thumbnailIcon.GetCssValue("margin-right"));
                    Logger.Instance.InfoLog("Value for Report is");
                    Logger.Instance.InfoLog("padding-top is =" + reporticonContainer.GetCssValue("padding-top"));
                    Logger.Instance.InfoLog("padding-bottom is =" + reporticonContainer.GetCssValue("padding-bottom"));
                    Logger.Instance.InfoLog("padding-left is =" + reporticonContainer.GetCssValue("padding-left"));
                    Logger.Instance.InfoLog("padding-right is =" + reporticonContainer.GetCssValue("padding-right"));
                    Logger.Instance.InfoLog("margin-top is =" + reportIcon.GetCssValue("margin-top"));
                    Logger.Instance.InfoLog("margin-bottom is =" + reportIcon.GetCssValue("margin-bottom"));
                    Logger.Instance.InfoLog("margin-left is =" + reportIcon.GetCssValue("margin-left"));
                    Logger.Instance.InfoLog("margin-right is =" + reportIcon.GetCssValue("margin-right"));
                }
                else
                {
                    Logger.Instance.ErrorLog("Value for thumbnail is");
                    Logger.Instance.ErrorLog("padding-top is =" + thumbnailiconContainer.GetCssValue("padding-top"));
                    Logger.Instance.ErrorLog("padding-bottom is =" + thumbnailiconContainer.GetCssValue("padding-bottom"));
                    Logger.Instance.ErrorLog("padding-left is =" + thumbnailiconContainer.GetCssValue("padding-left"));
                    Logger.Instance.ErrorLog("padding-right is =" + thumbnailiconContainer.GetCssValue("padding-right"));
                    Logger.Instance.ErrorLog("margin-top is =" + thumbnailIcon.GetCssValue("margin-top"));
                    Logger.Instance.ErrorLog("margin-bottom is =" + thumbnailIcon.GetCssValue("margin-bottom"));
                    Logger.Instance.ErrorLog("margin-left is =" + thumbnailIcon.GetCssValue("margin-left"));
                    Logger.Instance.ErrorLog("margin-right is =" + thumbnailIcon.GetCssValue("margin-right"));
                    Logger.Instance.ErrorLog("Value for Report is");
                    Logger.Instance.ErrorLog("padding-top is =" + reporticonContainer.GetCssValue("padding-top"));
                    Logger.Instance.ErrorLog("padding-bottom is =" + reporticonContainer.GetCssValue("padding-bottom"));
                    Logger.Instance.ErrorLog("padding-left is =" + reporticonContainer.GetCssValue("padding-left"));
                    Logger.Instance.ErrorLog("padding-right is =" + reporticonContainer.GetCssValue("padding-right"));
                    Logger.Instance.ErrorLog("margin-top is =" + reportIcon.GetCssValue("margin-top"));
                    Logger.Instance.ErrorLog("margin-bottom is =" + reportIcon.GetCssValue("margin-bottom"));
                    Logger.Instance.ErrorLog("margin-left is =" + reportIcon.GetCssValue("margin-left"));
                    Logger.Instance.ErrorLog("margin-right is =" + reportIcon.GetCssValue("margin-right"));
                    result.steps[++ExecutedSteps].status = "Fail";
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
        /// Exam List Filter: Modality filter configuration
        /// </summary>
        public TestCaseResult Test_161652(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            servicetool = new ServiceTool();
            int ExecutedSteps = -1;
            String saveExistingNonPrimaryImageModalities = GetNodeValue(Config.FileLocationPath, "/Configuration/ImageViewer/NonPrimaryImageModalities");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String patientID = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID"));
                String name = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name"));
                String dob = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOB"));
                String gender = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "Gender"));
                String ipid = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPID"));
                String studyDate = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorDate"));
                String modality = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorModality"));
                String studydesc = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDesc"));
                String datasource = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList"));

                //Step - 1 Check the WebAccessConfiguration.xml file to see what the "non primary modalities" are.
                String nonPrimaryModality = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "NonPrimaryModalities"));
                if (saveExistingNonPrimaryImageModalities.Equals(nonPrimaryModality))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step - 2 Login to WebAccess site with any privileged user.	
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                result.steps[++ExecutedSteps].StepPass();

                //Step - 3 Search and load a patient with many priors into the Universal viewer(if you are using the recommended data set, find the patient "PET 2, 15 PRIORS" and load one of the studies).
                studies.SearchStudy(AccessionNo: accession, Datasource: datasource);
                studies.SelectStudy("Accession", accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);

                if (bluringviewer.IsAllPriorsDisplayed())
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step - 4 Verify that ALL is selected by default in the Modality drop - down filter, and all other modalities are not selected.
                bluringviewer.OpenModalityFilter();
                String modalities4 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_4", "Modalities"));
                String[] modalitiesList4 = modalities4.Split(',');

                bool modalityIsSelected = false;
                foreach (String modality4 in modalitiesList4)
                {
                    if (bluringviewer.IsModalitySelected(modality4))
                    {
                        modalityIsSelected = true;
                        break;
                    }
                }

                if (bluringviewer.IsModalitySelected("All") && !modalityIsSelected)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step - 5 Verify that all modalities in the Exam List card details(each study's modalities) are represented in the Modality drop-down filter list.	
                String expectedModalityString5 = (String)ReadExcel.GetTestData(filepath, "TestData", testid + "_5", "PriorModality");
                String[] expectedModalityList5 = expectedModalityString5.Split(':');
                IList<String> modalitiesRetrieve5 = new List<String>();
                bool isModalityfound = true;

                var modalityDropdown = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_ele));

                foreach (IWebElement option in modalityDropdown)
                {
                    modalitiesRetrieve5.Add(option.FindElement(By.CssSelector(BluRingViewer.modality_options_text)).GetAttribute("innerHTML"));
                }

                foreach (String modalityCurrent in expectedModalityList5)
                {
                    if (!modalitiesRetrieve5.Contains(modalityCurrent))
                    {
                        isModalityfound = false;
                    }
                }

                if (isModalityfound)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step - 6 From the Exam List's Modality filter drop-down, deselect ALL, and select half of the primary modalities. 
                //       Make note of the modalities you've selected and make note of the number of studies remaining in the Exam List.
                bluringviewer.CloseModalityFilter();

                String modalities6 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_6", "Modalities"));
                String[] modalitiesList6 = modalities6.Split(',');

                bluringviewer.OpenModalityFilter();
                foreach (String modality6 in modalitiesList6)
                {
                    bluringviewer.SelectModalityValue(modality6);
                }
                bluringviewer.CloseModalityFilter();

                IList<IWebElement> prior_6 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                String expectedExamCardCount6 = (String)ReadExcel.GetTestData(filepath, "TestData", testid + "_6", "ExamCardCount");
                String outOfTextRetrieved6 = Driver.FindElement(By.CssSelector(BluRingViewer.examCardStudyCountText)).GetAttribute("innerText");
                String expectedModalityString6 = (String)ReadExcel.GetTestData(filepath, "TestData", testid + "_6", "PriorModality");
                String[] expectedModalityList6 = expectedModalityString6.Split(':');
                int expPriorCount6 = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_6", "PriorCount"));
                bool isModalityfound6 = true;
                IList<String> modalityRetrieved6 = new List<String>();

                // Get list of priors that are displayed and save their modalities
                int priorIndex = 0;
                while (priorIndex <= prior_6.Count - 1)
                {
                    String currentModalityFound;
                    currentModalityFound = prior_6[priorIndex].Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[1];
                    modalityRetrieved6.Add(currentModalityFound);
                    priorIndex++;
                }

                // Verify that the modalites that are displayed in the priors are those that are expected.
                foreach (String modalityCurrent in expectedModalityList6)
                {
                    if (!modalityRetrieved6.Contains(modalityCurrent))
                    {
                        isModalityfound6 = false;
                    }
                }

                // Verify count is as expected.
                if (outOfTextRetrieved6.Trim().Equals(expectedExamCardCount6.Trim())
                    && isModalityfound6
                    && prior_6.Count == expPriorCount6)
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

                //Step - 7 Go to the WebAccess web server and modify C:\WebAccess\WebAccess\Config\WebAccessConfiguration.xml.  
                //         If you used the recommended data set, remove PR from the<NonPrimaryImageModalities> list.Save the file and reset IIS.
                String updatedNonPrimaryModality = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "UpdatedNonPrimaryModalities"));
                ChangeNodeValue(Config.FileLocationPath, "/Configuration/ImageViewer/NonPrimaryImageModalities", updatedNonPrimaryModality);

                bluringviewer.CloseBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                login.Logout();
                PageLoadWait.WaitForFrameLoad(20);
                servicetool.LaunchServiceTool();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseConfigTool();
                result.steps[++ExecutedSteps].StepPass();

                //Step - 8 Log back into WebAccess site with the previous privileged user.
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                result.steps[++ExecutedSteps].StepPass();

                //Step - 9 Search and load the same patient from previous step with many priors into the Universal viewer.
                //         If you are using the recommended data set, find the patient "PET 2, 15 PRIORS" and load one of the studies into the Universal viewer. 
                studies.SearchStudy(AccessionNo: accession, Datasource: datasource);
                studies.SelectStudy("Accession", accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);

                IList<IWebElement> prior_9 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                String expectedExamCardCount9 = (String)ReadExcel.GetTestData(filepath, "TestData", testid + "_9", "ExamCardCount");
                int expPriorCount9 = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_9", "PriorCount"));

                String outOfTextRetrieved9 = Driver.FindElement(By.CssSelector(BluRingViewer.examCardStudyCountText)).GetAttribute("innerText");

                if (outOfTextRetrieved9.Trim().Equals(expectedExamCardCount9.Trim()) &&
                    prior_9.Count == expPriorCount9 &&
                    (bluringviewer.IsAllPriorsDisplayed()))
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

                //Step - 10 From the Exam List, verify that the modality displayed in the Modality drop - down filter should not contain the modalities from 
                //       the priors that are in the<NonPrimaryImageModalities> list in the WebAccessConfiguration.xml file.  For the recommended data set "PET 2, 15 PRIORS"(with PR modality 
                //       excluded from the<NonPrimaryImageModalities> list), verify that the Modality filter drop-down list contains All modalities with PR modality also. 
                String expectedModalityString10 = (String)ReadExcel.GetTestData(filepath, "TestData", testid + "_10", "PriorModality");
                String[] expectedModalityList10 = expectedModalityString10.Split(':');
                IList<String> modalitiesRetrieve10 = new List<String>();
                isModalityfound = true;

                bluringviewer.OpenModalityFilter();
                modalityDropdown = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_ele));

                foreach (IWebElement option in modalityDropdown)
                {
                    modalitiesRetrieve10.Add(option.FindElement(By.CssSelector(BluRingViewer.modality_options_text)).GetAttribute("innerHTML"));
                }

                foreach (String modalityCurrent in expectedModalityList10)
                {
                    if (!modalitiesRetrieve10.Contains(modalityCurrent))
                    {
                        isModalityfound = false;
                    }
                }

                //Ensure that "KO" is not included in the Modality list
                if (isModalityfound &&
                    !modalitiesRetrieve10.Contains(updatedNonPrimaryModality))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step - 11 From the Exam List's Modality filter drop-down, select only one of the modality on the list.
                bluringviewer.CloseModalityFilter();
                bluringviewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: accession, Datasource: datasource);
                studies.SelectStudy("Accession", accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);

                String testModality11 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_11", "TestModalities"));

                bluringviewer.OpenModalityFilter();
                bluringviewer.SelectModalityValue(testModality11);
                bluringviewer.CloseModalityFilter();

                IList<IWebElement> prior_11 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                String expectedExamCardCount11 = (String)ReadExcel.GetTestData(filepath, "TestData", testid + "_11", "ExamCardCount");
                String outOfTextRetrieved11 = Driver.FindElement(By.CssSelector(BluRingViewer.examCardStudyCountText)).GetAttribute("innerText");
                String expectedModality11 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_11", "PriorModality"));
                String[] expectedModalityList11 = expectedModality11.Split(':');
                int expPriorCount11 = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_11", "PriorCount"));

                IList<String> modalitiesRetrieved11 = new List<String>();
                bool unexpectedModalityFound11 = false;

                int index11 = 0;
                while (index11 <= prior_11.Count - 1)
                {
                    String currentModalityFound;
                    var container = BasePage.FindElementByCss(BluRingViewer.div_ContainerPriors);
                    if (this.IsInBrowserViewport(prior_11[index11]) == false)
                    {
                        new TestCompleteAction().MouseScroll(container, "down", "4").Perform();
                        if (this.IsInBrowserViewport(prior_11[index11]) == true)
                        {
                            currentModalityFound = prior_11[index11].Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[1];
                            modalitiesRetrieved11.Add(currentModalityFound);

                            index11++;
                            continue;
                        }
                        else
                        {
                            Logger.Instance.ErrorLog((index11 + 1) + "the Prior Not Displayed");
                            break;
                        }
                    }
                    else
                    {
                        currentModalityFound = prior_11[index11].Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[1];
                        modalitiesRetrieved11.Add(currentModalityFound);

                        index11++;
                        continue;
                    }
                }

                for (int index = 0; index < prior_11.Count; index++)
                {
                    if (!expectedModalityList11.Contains(modalitiesRetrieved11[index]))
                    {
                        unexpectedModalityFound11 = true;
                        Logger.Instance.ErrorLog("Unexpected Modailty found");
                    }
                }

                if (outOfTextRetrieved11.Trim().Equals(expectedExamCardCount11.Trim())
                    && !unexpectedModalityFound11
                    && prior_11.Count == expPriorCount11)
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


                //Step - 12 From the Exam List's Modality filter drop-down, select another modality on the list. 
                //          Make note of the modality you have selected and ensure that the Exam list now only contains the studies with that modality filtered. 
                //          Ensure the studies counter is updated accordingly. For the recommended data set "PET 2, 15 PRIORS", select only MR modality from the Modality filter list.  
                String testModalities12 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_12", "TestModalities"));
                String[] testModalitiesList12 = testModalities12.Split(',');

                bluringviewer.OpenModalityFilter();
                bluringviewer.SelectModalityValue(testModalitiesList12[0], unselect: true);
                bluringviewer.SelectModalityValue(testModalitiesList12[1]);
                bluringviewer.CloseModalityFilter();

                IList<IWebElement> prior_12 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                String expectedExamCardCount12 = (String)ReadExcel.GetTestData(filepath, "TestData", testid + "_12", "ExamCardCount");
                String outOfTextRetrieved12 = Driver.FindElement(By.CssSelector(BluRingViewer.examCardStudyCountText)).GetAttribute("innerText");
                String expectedModality12 = ((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_12", "PriorModality"));
                int expPriorCount12 = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_12", "PriorCount"));
                String modalityRetrieved12 = prior_12[0].Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[1];


                if (outOfTextRetrieved12.Trim().Equals(expectedExamCardCount12.Trim())
                    && modalityRetrieved12.Equals(expectedModality12)
                    && prior_12.Count == expPriorCount12)
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

                //Step - 13 From the Exam List's Modality filter drop-down, select ALL. 
                //          Ensure that the Exam list now contains all the prior studies for the patient, even the non-primary modality studies. 
                //          Ensure the studies counter is updated accordingly. Ensure that the other options in the Modality filter list is not selected. 
                bluringviewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: accession, Datasource: datasource);
                studies.SelectStudy("Accession", accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);

                bluringviewer.OpenModalityFilter();
                bluringviewer.SelectModalityValue("All");
                bluringviewer.CloseModalityFilter();

                IList<IWebElement> prior_13 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                String expectedExamCardCount13 = (String)ReadExcel.GetTestData(filepath, "TestData", testid + "_13", "ExamCardCount");
                int expPriorCount13 = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_13", "PriorCount"));
                String outOfTextRetrieved13 = Driver.FindElement(By.CssSelector(BluRingViewer.examCardStudyCountText)).GetAttribute("innerText");

                if (outOfTextRetrieved13.Trim().Equals(expectedExamCardCount13.Trim()) &&
                    prior_13.Count == expPriorCount13 &&
                    bluringviewer.IsAllPriorsDisplayed())
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

                //Step - 14 Go to the WebAccess web server and modify C:\WebAccess\WebAccess\Config\WebAccessConfiguration.xml: add the modalities you have removed from the previous step.
                //       If you used the recommended data set, add back the PR modality to the < NonPrimaryImageModalities > list.Save the file and reset IIS.
                ChangeNodeValue(Config.FileLocationPath, "/Configuration/ImageViewer/NonPrimaryImageModalities", saveExistingNonPrimaryImageModalities);

                bluringviewer.CloseBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                login.Logout();
                PageLoadWait.WaitForFrameLoad(20);
                servicetool.LaunchServiceTool();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseConfigTool();
                result.steps[++ExecutedSteps].StepPass();

                //Step - 15 Log back into WebAccess site with the previous privileged user.
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                studies = (Studies)login.Navigate("Studies");
                result.steps[++ExecutedSteps].StepPass();

                //Step - 16 Search and load the same patient from previous step with many priors into the Universal viewer.If you are using the recommended data set, find the patient "PET 2, 15 PRIORS" and load one of the studies into the Universal viewer.	
                studies.SearchStudy(AccessionNo: accession, Datasource: datasource);
                studies.SelectStudy("Accession", accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);

                IList<IWebElement> prior_16 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                String expectedExamCardCount16 = (String)ReadExcel.GetTestData(filepath, "TestData", testid + "_16", "ExamCardCount");
                int expPriorCount16 = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", testid + "_16", "PriorCount"));
                String outOfTextRetrieved16 = Driver.FindElement(By.CssSelector(BluRingViewer.examCardStudyCountText)).GetAttribute("innerText");

                if (outOfTextRetrieved16.Trim().Equals(expectedExamCardCount16.Trim()) &&
                    prior_16.Count == expPriorCount16 &&
                    bluringviewer.IsAllPriorsDisplayed())
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

                //Step - 17 Verify that ALL is selected by default in the Modality drop - down filter contains all the primary modalities included in the priors list 
                //          and Ensure that the Non Primary modality should not be displayed in the Modality drop - down
                bluringviewer.OpenModalityFilter();

                String expectedModalityString17 = (String)ReadExcel.GetTestData(filepath, "TestData", testid + "_17", "PriorModality");
                String[] expectedModalityList17 = expectedModalityString17.Split(':');
                String[] nonPrimaryModalityList = saveExistingNonPrimaryImageModalities.Split(',');
                var displayedModalityList = new List<String>();
                var modalityDropdown17 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_ele));

                //Verify All is selected by default
                bool isAllSelected = bluringviewer.IsModalitySelected("All");

                // Confirm Non-primary modality are not displayed in the modailty drop down and get list of displayed modalities
                bool nonPrimaryModalityFound = false;
                foreach (IWebElement option in modalityDropdown17)
                {
                    String modalityCurrent = option.FindElement(By.CssSelector(BluRingViewer.modality_options_text)).GetAttribute("innerHTML");
                    displayedModalityList.Add(modalityCurrent);

                    if (nonPrimaryModalityList.Contains(modalityCurrent))
                    {
                        nonPrimaryModalityFound = true;
                    }
                }

                // Confirm all the primary modailites are included in the priors list
                bool allPrimaryModalityIncluded = true;
                foreach (String modality17 in expectedModalityList17)
                {
                    if (!displayedModalityList.Contains(modality17))
                    {
                        allPrimaryModalityIncluded = false;
                        break;
                    }
                }

                if (isAllSelected &&
                    !nonPrimaryModalityFound &&
                    allPrimaryModalityIncluded)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                bluringviewer.CloseModalityFilter();

                //Logout Application
                bluringviewer.CloseBluRingViewer();
                login.Logout();

                // Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Reset the NonPrimaryImageModalities
                ChangeNodeValue(Config.FileLocationPath, "/Configuration/ImageViewer/NonPrimaryImageModalities", saveExistingNonPrimaryImageModalities);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }


        /// <summary>
        /// Test 141965 - Opacity for Report Icon in the Study Data result set
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161653(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            String domainame = String.Empty;
            int ExecutedSteps = -1;

            try
            {
                var patientID = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID"));
                var accession = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID"));
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                //Step-1
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var studies = login.Navigate<Studies>();
                ExecutedSteps++;

                //Step-2
                studies.SearchStudy("PatientID", patientID);
                studies.SelectStudy("Patient ID", patientID);
                ExecutedSteps++;

                //Step-3
                var viewer = BluRingViewer.LaunchBluRingViewer();
                var step3 = result.steps[++ExecutedSteps];
                var viewport3 = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                var thumbnails3 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails))[0];
                step3.SetPath(testid, ExecutedSteps, 0);
                var isViewportLoaded = viewer.CompareImage(step3, viewport3, 0);
                step3.SetPath(testid, ExecutedSteps, 1);
                var isThumbnailLoaded = viewer.CompareImage(step3, thumbnails3, 1, 1);
                if (isViewportLoaded && isThumbnailLoaded)
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

                //Step-4 Opacity 0.2 - report Icon
                var reportIcon4 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                var icon = reportIcon4.FindElement(By.CssSelector("div"));
                if (icon.GetCssValue("opacity").Equals("0.2"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-5 Opacity 0.6 - report Icon
                viewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: accession);
                studies.SelectStudy("Accession", accession);
                viewer = BluRingViewer.LaunchBluRingViewer();
                var reportIcon5 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon))[0];
                icon = reportIcon5.FindElement(By.CssSelector("div"));
                if (icon.GetCssValue("opacity").Equals("0.6"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6 Exam List Border - Selected Study -- White
                var prior = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors))[0];
                bool isBorderCorrect = false;
                viewer.OpenPriors(0);  
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer"))
                    isBorderCorrect = prior.GetCssValue("border-color").ToUpper().Contains("#FFFFFF");
                else
                    isBorderCorrect = prior.GetCssValue("border").Contains("rgb(255, 255, 255)");

                if (isBorderCorrect)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog(prior.GetCssValue("border"));
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.InfoLog(prior.GetCssValue("border"));
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.ErrorLog("-->Actual Color Value--" + prior.GetCssValue("border"));
                    Logger.Instance.ErrorLog("-->Expected color Value--" + "rgb(90, 170, 255)");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7 Exam List Border - Selected Study
                prior = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors))[0];
                isBorderCorrect = false;                
                new TestCompleteAction().MoveToElement(reportIcon5).Perform();
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer"))
                    isBorderCorrect = prior.GetCssValue("border-color").Contains("#5aaaff");
                else
                    isBorderCorrect = prior.GetCssValue("border").Contains("rgb(90, 170, 255)");

                if (isBorderCorrect)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog(prior.GetCssValue("border"));
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.InfoLog(prior.GetCssValue("border"));
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.ErrorLog("-->Actual Color Value--" + prior.GetCssValue("border"));
                    Logger.Instance.ErrorLog("-->Expected color Value--" + "rgb(90, 170, 255)");
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
        /// Test 142048 - Closing the Exam list filters dropdown by using Tab Key and clicking the mouse outside of the filter dropdown area
        /// </summary>
        public TestCaseResult Test_161656(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                int expPriorCount = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount"));
                var modalityList = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorModality")).Split(':');
                String institution10 = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorSite");
                BluRingViewer viewer = null;
                String cssSelector = BluRingViewer.modality_options_ele + " " + BluRingViewer.modality_options_text;
                String studydate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate");

                //Step 1 Launch and Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                //Step 2 - Load study                        
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy1(new string[] { "Study Date", "Accession" }, new string[] { studydate, accession });
                viewer = BluRingViewer.LaunchBluRingViewer();
                var step3 = result.steps[++ExecutedSteps];
                var viewport3 = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                var thumbnails3 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails))[0];
                step3.SetPath(testid, ExecutedSteps, 0);
                var isViewportLoaded = viewer.CompareImage(step3, viewport3);
                step3.SetPath(testid, ExecutedSteps, 1);
                var isThumbnailLoaded = viewer.CompareImage(step3, thumbnails3);
                if (isViewportLoaded && isThumbnailLoaded)
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
                if (viewer.CheckPriorsCount() == expPriorCount)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.ErrorLog("The expected prior count---" + expPriorCount);
                    Logger.Instance.ErrorLog("The actual prior count---" + viewer.CheckPriorsCount());
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-4
                viewer.OpenModalityFilter();
                var options = BasePage.Driver.FindElements(By.CssSelector(cssSelector)).Select<IWebElement, String>
                    (item => item.GetAttribute("innerHTML")).ToList<String>();
                if (viewer.ValidateModalityFiltered(options))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }

                //Stpe-5 Press Tab key and close it
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));
                ExecutedSteps++;

                //Step-6 - Click Sort By dropdown                
                viewer.OpenSortDorpdown();
                var popup = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_SortPopUp);
                var items = popup.FindElements(By.CssSelector(BluRingViewer.div_SortItems)).
                    Select(item => item.FindElement(By.CssSelector("span")).GetAttribute("innerHTML")).ToList<String>();
                if (items.Contains("Date - Newest") && items.Contains("Date - Oldest") && items.Contains("Modality Type"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7 - Press Tab and close Sort Filter
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                new Actions(BasePage.Driver).Click(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport)).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_SortPopUp)));
                ExecutedSteps++;

                //Step-8 - Open Modality Filter
                viewer.OpenModalityFilter();
                options = BasePage.Driver.FindElements(By.CssSelector(cssSelector)).Select<IWebElement, String>
                    (item => item.GetAttribute("innerHTML")).ToList<String>();

                if (viewer.ValidateModalityFiltered(options))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }

                //Step-9 - Click Outside and close Modality Filter
                new Actions(BasePage.Driver).Click(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport)).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));
                ExecutedSteps++;

                //Step-10 - Open sort Doropdown
                viewer.OpenSortDorpdown();
                popup = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_SortPopUp);
                items = popup.FindElements(By.CssSelector(BluRingViewer.div_SortItems)).
                    Select(item => item.FindElement(By.CssSelector("span")).GetAttribute("innerHTML")).ToList<String>();
                if (items.Contains("Date - Newest") && items.Contains("Date - Oldest") && items.Contains("Modality Type") && viewer.IsStudiesSortedByDate())
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-11 - Click Outside and Close Sort Dropdown
                new Actions(BasePage.Driver).Click(viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport)).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_SortPopUp)));
                ExecutedSteps++;

                //Step-12
                viewer.OpenModalityFilter();
                options = BasePage.Driver.FindElements(By.CssSelector(cssSelector)).Select<IWebElement, String>
                    (item => item.GetAttribute("innerHTML")).ToList<String>();
                if (viewer.ValidateModalityFiltered(options))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }

                //Step-13 - All check box checked, Remaining UnChecked
                var modalities = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_ele));
                var isAllCheckBoxSleected = false;
                var isOtherCheckBoxselected = true;
                foreach (var modality in modalities)
                {
                    if (modality.FindElement(By.CssSelector(BluRingViewer.modality_options_text)).GetAttribute("innerHTML").Equals("All"))
                    {
                        if (modality.GetAttribute("aria-selected").Equals("true"))
                            isAllCheckBoxSleected = true;
                    }
                    else
                    {
                        if (modality.GetAttribute("aria-selected").Equals("false"))
                        {
                            isOtherCheckBoxselected = true;
                        }
                        else
                        {
                            isOtherCheckBoxselected = false;
                            break;

                        }

                    }

                }
                if (isAllCheckBoxSleected && isOtherCheckBoxselected)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }

                //Step-14-Verify all check box get un selected
                foreach (var modality in modalityList)
                {
                    viewer.SelectModalityValue(modality);
                }
                modalities = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_ele));
                isAllCheckBoxSleected = false;
                foreach (var modality in modalities)
                {
                    if (modality.FindElement(By.CssSelector(BluRingViewer.modality_options_text)).GetAttribute("innerHTML").Equals("All"))
                    {
                        if (modality.GetAttribute("aria-selected").Equals("true"))
                            isAllCheckBoxSleected = true;
                        else
                            isAllCheckBoxSleected = false;
                    }
                }
                if (!isAllCheckBoxSleected)
                {
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
                viewer.UnSelectAllModalities(modalityList);
                viewer.SelectModalityValue(modalityList[0]);
                var mlist = new List<String>();
                mlist.Add(modalityList[0]);
                if (viewer.ValidateModalityFiltered(mlist))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }


                //Step-16                
                modalities = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_ele));
                isAllCheckBoxSleected = false;
                foreach (var modality in modalities)
                {
                    if (modality.FindElement(By.CssSelector(BluRingViewer.modality_options_text)).GetAttribute("innerHTML").Equals("All"))
                    {
                        if (modality.GetAttribute("aria-selected").Equals("true"))
                            isAllCheckBoxSleected = true;
                        else
                            isAllCheckBoxSleected = false;
                    }
                }
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab).Build().Perform();
                if (!isAllCheckBoxSleected)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

				//Step 17
				viewer.OpenModalityFilter();
				GetElement(SelectorType.CssSelector, BluRingViewer.Modality_Clear_All).Click();
				modalities = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_ele));
				if (modalities.All(mod => mod.GetAttribute("aria-selected").Equals("false")))
				{
					result.steps[++ExecutedSteps].status = "Pass";
					Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
				}
				else
				{
					result.steps[++ExecutedSteps].status = "Fail";
					Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
					result.steps[ExecutedSteps].SetLogs();
				}

				//Step 18
				viewer.SelectModalityValue("All");
				isAllCheckBoxSleected = false;				
				foreach (var modality in modalities)
				{
					if (modality.FindElement(By.CssSelector(BluRingViewer.modality_options_text)).GetAttribute("innerHTML").Equals("All"))
					{
						if (modality.GetAttribute("aria-selected").Equals("true"))
							isAllCheckBoxSleected = true;						
						else
							isAllCheckBoxSleected = false;
						break;
					}
				}
				new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab).Build().Perform();
				if (isAllCheckBoxSleected)
				{
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
        /// Test 142250 - Empty series viewports shall resize to fit the new panel size when closing the other study panels in the viewer
        /// </summary>
        public TestCaseResult Test_161657(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

                //Precondition --  Setup Correct resolution
                BasePage.SetVMResolution("1980", "1080");

                //Step 1 Launch and Login as Administrator 
                login.LoginIConnect(adminUserName, adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;


                //Step 2 - Load study                        
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy("Accession", accession);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                var step2 = result.steps[++ExecutedSteps];
                var viewport2 = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport);
                step2.SetPath(testid, ExecutedSteps);
                var isViewportLoaded = viewer.CompareImage(step2, viewport2);
                if (isViewportLoaded)
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

                //Step-3 Open priors
                viewer.OpenPriors(0);
                viewer.OpenPriors(1);
                viewer.OpenPriors(2);
                var studyPanelCount = viewer.GetStudyPanelCount();
                viewer.CloseExamList();
                if (studyPanelCount == 4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4
                var viewportAttr2_4 = viewer.GetElementAttributes(viewer.SetViewPort(2, 1));
                var viewportAttr3_4 = viewer.GetElementAttributes(viewer.SetViewPort(3, 1));
                viewer.CloseStudypanel(4);
                viewer.CloseStudypanel(3);
                viewer.CloseStudypanel(2);
                studyPanelCount = viewer.GetStudyPanelCount();
                var viewportCount = viewer.GetViewPortCount(1);
                if (studyPanelCount == 1 && viewportCount == 4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5
                var viewportAttr2_5 = viewer.GetElementAttributes(viewer.SetViewPort(2, 1));
                var viewportAttr3_5 = viewer.GetElementAttributes(viewer.SetViewPort(3, 1));
                var studyPanelAttr = viewer.GetElementAttributes(BluRingViewer.div_studypanel);
                if (viewportAttr2_5["width"] > viewportAttr2_4["width"] &&
                    viewportAttr3_5["width"] > viewportAttr3_4["width"])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6
                viewer.DropAndDropThumbnails(1, 3, 1, UseDragDrop: true);
                Thread.Sleep(5000);
                var viewport6 = viewer.GetElement(BasePage.SelectorType.CssSelector,
                    viewer.SetViewPort(2, 1));
                var step6 = result.steps[++ExecutedSteps];
                step6.SetPath(testid, ExecutedSteps);
                if (viewer.CompareImage(step6, viewport6))
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
                int width = BasePage.Driver.Manage().Window.Size.Width;
                int height = BasePage.Driver.Manage().Window.Size.Height;
                BasePage.Driver.Manage().Window.Size = new System.Drawing.Size(width - 100, (height - 100));
                BasePage.wait.Until<Boolean>(driver => driver.Manage().Window.Size.Height == (height - 100));
                BasePage.wait.Until<Boolean>(driver => driver.Manage().Window.Size.Width == (width - 100));
                BasePage.Driver.Manage().Window.Position = new System.Drawing.Point(0, 0);

                var viewportAttr2_7 = viewer.GetElementAttributes(viewer.SetViewPort(2, 1));
                var viewportAttr3_7 = viewer.GetElementAttributes(viewer.SetViewPort(3, 1));
                BasePage.Driver.Manage().Window.Maximize();
                if (viewportAttr2_7["width"] < viewportAttr2_5["width"] && viewportAttr3_7["width"] < viewportAttr3_5["width"]
                    && viewportAttr2_7["height"] < viewportAttr2_5["height"] && viewportAttr3_7["height"] < viewportAttr3_5["height"])
                {
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
                viewer.OpenExamList();
                viewer.OpenPriors(0);
                viewer.CloseExamList();
                if (viewer.GetStudyPanelCount() == 2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9 - No Automation - since will not be able to precisly verify 
                //that empty viewport re-sizing when other viewports are loading.
                var viewportAttr2_9 = viewer.GetElementAttributes(viewer.SetViewPort(2, 1));
                var viewportAttr3_9 = viewer.GetElementAttributes(viewer.SetViewPort(3, 1));
                if(viewportAttr2_9["width"] < viewportAttr2_5["width"] && viewportAttr3_9["width"] < viewportAttr3_5["width"])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }                

                //Step-10 
                var viewportAttr2_10 = viewer.GetElementAttributes(viewer.SetViewPort(2, 1));
                var viewportAttr3_10 = viewer.GetElementAttributes(viewer.SetViewPort(3, 1));
                var studyPanelAttr10 = viewer.GetElementAttributes(BluRingViewer.div_studypanel);
                if (viewportAttr3_10["width"] < viewportAttr3_5["width"])
                    
                {
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
                Thread.Sleep(2000);
                viewer.CloseStudypanel(2);
                Thread.Sleep(2000);
                viewer.OpenExamList();
                Thread.Sleep(2000);
                var priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));               
                priors[2].Click();
                Thread.Sleep(5000);
                viewer.CloseExamList();
                Thread.Sleep(2000);
                var viewportAttr2_11 = viewer.GetElementAttributes(viewer.SetViewPort(1, 1));
                if (viewportAttr2_11["width"] == viewportAttr3_10["width"])
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-12     
                viewer.OpenExamList();
                viewer.OpenPriors(1);
                viewer.CloseExamList();
                viewer.CloseStudypanel(2);
                var viewportAttr3_12 = viewer.GetElementAttributes(viewer.SetViewPort(3, 1));
                var viewport12 = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(3, 1));
                var step12 = result.steps[++ExecutedSteps];
                step12.SetPath(testid, ExecutedSteps);
                if (viewportAttr3_12["width"] == viewportAttr3_10["width"] && viewer.CompareImage(step12, viewport12))
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
                viewer.CloseBluRingViewer();
                ExecutedSteps++;                

                //Step-14
                studies.SelectStudy("Accession", accession);
                var viewer15 = BluRingViewer.LaunchBluRingViewer();
                var step15 = result.steps[++ExecutedSteps];
                var viewport15 = viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.SetViewPort(0, 1));
                step15.SetPath(testid, ExecutedSteps);
                var isViewportLoaded15 = viewer.CompareImage(step15, viewport15);
                if (isViewportLoaded15)
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

                //step-15
                viewer.OpenPriors(0);
                viewer.OpenPriors(1);
                viewer.OpenPriors(2);
                var studyPanelCount16 = viewer.GetStudyPanelCount();
                if (studyPanelCount16 == 4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-16        
                viewer.CloseStudypanel(2);
                viewer.CloseStudypanel(2);
                viewer.CloseStudypanel(2);
                studyPanelCount = viewer.GetStudyPanelCount();
                var viewportCount17 = viewer.GetViewPortCount(1);
                if (studyPanelCount == 1 && viewportCount17 == 4)
                {
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

            finally
            {
                BasePage.SetVMResolution("1280", "1024");
            }
        }

        /// <summary>
        /// Test 145245  - UI changes in Exam List Panel,Toolbox and Email
        /// </summary>
        public TestCaseResult Test_161659(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                string PID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                string[] sortbyOptionValues = { "Date - Newest", "Date - Oldest", "Modality Type" };

                //Precondition 
                BasePage.SetVMResolution("1280", "1024");

                //Step 1 Launch ICA Application
                login.DriverGoTo(login.url);
                result.steps[++ExecutedSteps].StepPass();

                //Step 2
                login.LoginIConnect(adminUserName, adminPassword);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                var studies = (Studies)login.Navigate("Studies");
                login.Navigate("Patients");
                login.Navigate("DomainManagement");
                result.steps[++ExecutedSteps].StepPass();

                //Step 3
                studies = (Studies)login.Navigate("Studies");
                result.steps[++ExecutedSteps].StepPass();

                //Step 4
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy("Accession", accession);
                var blueringViewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //Step 6 -Precondition
                IWebElement Activeviewport = blueringViewer.ClickOnViewPort(1, 1);
                blueringViewer.OpenViewerToolsPOPUp();
                new Actions(BasePage.Driver).Click(Activeviewport).Build().Perform();
                string widthBeforeRSChange = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_toolboxContainer)).GetCssValue("width");
                blueringViewer.CloseBluRingViewer();
                BasePage.SetVMResolution("1440", "710");

                //step 5 and 6
                result.steps[++ExecutedSteps].StepPass();
                studies.SelectStudy("Accession", accession);
                blueringViewer = BluRingViewer.LaunchBluRingViewer();
                blueringViewer.ClickOnViewPort(1, 1);
                blueringViewer.OpenViewerToolsPOPUp();
                string widthAfterRSChange = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_toolboxContainer)).GetCssValue("width");
                if (widthBeforeRSChange != widthAfterRSChange && widthBeforeRSChange == "177px")
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail(string.Format("widthBeforeRSChange: {0}. ", widthBeforeRSChange), false);
                blueringViewer.CloseBluRingViewer();

                //Step 7
                BasePage.SetVMResolution("1280", "1024");
                studies.SelectStudy("Accession", accession);
                blueringViewer = BluRingViewer.LaunchBluRingViewer();
                blueringViewer.OpenReport_BR(0);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IWebElement ReportContainer = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.ReportContainer_div));
                blueringViewer.NavigateToReportFrame(reporttype: "SR");
                var report_data1 = blueringViewer.FetchReportData_BR(0);
                if (string.Equals(report_data1["MRN:"], PID))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

                //step 8 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_8 = studies.CompareImage(result.steps[ExecutedSteps], blueringViewer.studyPanel());
                if (step_8)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                blueringViewer.CloseReport_BR(0);

                //Step 9
                blueringViewer.OpenModalityFilter();
                IWebElement ModalityDropDownWindow = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title));
                if (ModalityDropDownWindow.Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                blueringViewer.CloseModalityFilter();


                //Step 10
                int i = 0, found = 0;
                blueringViewer.OpenSortDorpdown();
                IList<IWebElement> SortByvaluesinWeb = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudySortOptionValues));
                foreach (IWebElement sortvalues in SortByvaluesinWeb)
                {
                    if (sortvalues.Text == sortbyOptionValues[i])
                        found++;
                    else
                        Logger.Instance.ErrorLog("Unable to find the Sort option value" + sortbyOptionValues[i]);
                    i++;
                }

                if (found == sortbyOptionValues.Length)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                blueringViewer.CloseModalityFilter();

                //Step 11.
                IWebElement ModalityDropDown = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_multiSelect_Modality)).ToArray()[0];
                if (ModalityDropDown.GetCssValue("font-size").StartsWith("11"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                blueringViewer.CloseModalityFilter();

                //Step 12.
                IWebElement SortByDrop = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_StudySort));
                if (SortByDrop.GetCssValue("font-size").StartsWith("11"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                blueringViewer.CloseModalityFilter();


                //Step 13
                string viewportfontsize = BasePage.Driver.FindElement(By.CssSelector(blueringViewer.SetViewPort1())).GetCssValue("font-size");
                string ViewportFontLine = BasePage.Driver.FindElement(By.CssSelector(blueringViewer.SetViewPort1())).GetCssValue("line-height");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_13 = studies.CompareImage(result.steps[ExecutedSteps], blueringViewer.ClickOnViewPort(1, 1));
                if (step_13 && viewportfontsize.StartsWith("11") && ViewportFontLine.StartsWith("13"))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail(string.Format("viewportfontsize: {0}. , ViewportFontLine: {1}. , imagecompare: {2}. ", viewportfontsize, ViewportFontLine, step_13), false);

                //Step 14
                result.steps[++ExecutedSteps].status = "NO AUTOMATION";

                //Step 15
                IWebElement EmailStudyButton = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_emailstudy));
                blueringViewer.ClickElement(EmailStudyButton);

                IWebElement send_button = BasePage.Driver.FindElements(By.CssSelector("button[type='submit']")).ToList<IWebElement>().Find(element =>
                element.GetAttribute("innerHTML").Equals("Send"));

                IWebElement Cancel_button = BasePage.Driver.FindElements(By.CssSelector("button[type='submit']")).ToList<IWebElement>().Find(element =>
                element.GetAttribute("innerHTML").Equals("Cancel"));
                if (send_button.GetCssValue("border-left-style") == "solid" && Cancel_button.GetCssValue("border-left-style") == "solid")
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

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
                string x_cor = Screen.PrimaryScreen.Bounds.Width.ToString();
                string Y_cor = Screen.PrimaryScreen.Bounds.Height.ToString();
                if (x_cor != Config.X_Coordinate && Y_cor != Config.Y_Coordinate)
                    BasePage.SetVMResolution(Config.X_Coordinate, Config.Y_Coordinate);
            }

        }

        /// <summary>
        /// Test 145271  - Non primary modalities (SR and DOC) shall display in exam card's Tool Tip
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161660(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            String domainame = String.Empty;
            int ExecutedSteps = -1;
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                var Accession = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID")).Split(',');
                var modality = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorModality")).Split(':');
                string SR_DOC_path = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath"));

                ServiceTool serviceTool = new ServiceTool();
                serviceTool.LaunchServiceTool();
                serviceTool.SetEnableFeaturesGeneral();
                serviceTool.wpfobject.ClickButton(ServiceTool.ModifyBtn_Name, 1);
                serviceTool.EnablePDFReport();                
                serviceTool.ApplyEnableFeatures();
                serviceTool.wpfobject.ClickOkPopUp();
                serviceTool.RestartService();
                serviceTool.CloseServiceTool();
                serviceTool.EnableAllReports();

                //Pre-condition - Send Study WITH SR,DOC
                string[] studyPath = Directory.GetFiles(SR_DOC_path, "*.*", SearchOption.AllDirectories);
                foreach (string Path in studyPath)
                    BasePage.PushStudy(Path, Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);

                //Step-1
                login.DriverGoTo(login.url);
                result.steps[++ExecutedSteps].StepPass();

                //step2
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //step-3
                var studies = login.Navigate<Studies>();
                ++ExecutedSteps;

                //Step-4 -
                studies.SearchStudy("Accession", Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                result.steps[++ExecutedSteps].StepPass();

                //Step-5
                var viewer = BluRingViewer.LaunchBluRingViewer();
                ++ExecutedSteps;

                //Step-6
                IWebElement prior = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors))[0];
                new TestCompleteAction().MoveToElement(prior).Perform();
                Thread.Sleep(3000);
                string tooltip = prior.GetAttribute("title");                
                if (tooltip.Contains("SR"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7
                viewer.CloseBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //Step-8
                studies.SearchStudy("Accession", Accession[1]);
                studies.SelectStudy("Accession", Accession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ++ExecutedSteps;

                //Step-9
                IWebElement prior_2 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors))[0];
                new TestCompleteAction().MoveToElement(prior_2).Perform();
                Thread.Sleep(3000);
                tooltip = prior_2.GetAttribute("title");                
                if (tooltip.Contains("DOC"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10 
                viewer.CloseBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //Step-11
                studies.SearchStudy(AccessionNo: Accession[2], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", Accession[2]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ++ExecutedSteps;

                //Step-12
                prior_2 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors))[0];
                new TestCompleteAction().MoveToElement(prior_2).Perform();
                new TestCompleteAction().MoveToElement(prior_2).Perform();
                Thread.Sleep(3000);
                tooltip = prior_2.GetAttribute("title");               
                if (tooltip.Contains("SR") && tooltip.Contains("DOC"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-13
                viewer.CloseBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //Step-14
                studies.SearchStudy("Accession", Accession[3]);
                studies.SelectStudy("Accession", Accession[3]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                ++ExecutedSteps;

                //Step-15
                IWebElement prior_4 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors))[0];
                new TestCompleteAction().MoveToElement(prior_4).Perform();
                new TestCompleteAction().MoveToElement(prior_4).Perform();
                Thread.Sleep(3000);
                tooltip = prior_4.GetAttribute("title");
                if (!tooltip.Contains("SR") && !tooltip.Contains("DOC"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
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
        /// Test 145274  - Non Primary modalities shall not display in Modality filter drop down
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161662(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                IList<string> Accession = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID")).Split(',');
                IList<string> Non_PrimaryModalityList = new List<string>() { "MG", "KO", "PR", "DOC", "SR" };

                //Step-1
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //step-2
                var studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", Accession[0]);
                studies.SelectStudy("Accession", Accession[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //step-3
                ++ExecutedSteps;
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                viewer.OpenModalityFilter();
                IList<IWebElement> modalityOptionsText = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text));
                foreach (IWebElement modalityOptionValues in modalityOptionsText)
                {
                    if (Non_PrimaryModalityList.Any<string>(MOD => MOD.Equals(modalityOptionValues.Text)))
                        result.steps[ExecutedSteps].AddFailStatusList();
                    else
                        result.steps[ExecutedSteps].AddPassStatusList();
                }

                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab).
                    Build().Perform();
                viewer.CloseBluRingViewer();
                studies.SearchStudy("Accession", Accession[1]);
                studies.SelectStudy("Accession", Accession[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                viewer.OpenModalityFilter();
                modalityOptionsText = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text));
                foreach (IWebElement modalityOptionValues in modalityOptionsText)
                {
                    if (Non_PrimaryModalityList.Any<string>(MOD => MOD.Equals(modalityOptionValues.Text)))
                        result.steps[ExecutedSteps].AddFailStatusList();
                    else
                        result.steps[ExecutedSteps].AddPassStatusList();
                }

                if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();

                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab).
                    Build().Perform();
                viewer.CloseBluRingViewer();
                //Logout Application 
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
        /// Test 145243  - Data source name in Exam card's tool tip
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161658(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                string patientID = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID"));
                string name = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name"));
                string dob = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOB"));
                string gender = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "Gender"));
                string ipid = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPID"));
                string studyadte = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorDate"));
                string modality = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorModality"));
                string studydesc = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDesc"));
                string datasource = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList"));
                string Accession = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID"));

                //Step-1
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //step-2
                var studies = login.Navigate<Studies>();
                studies.SearchStudy(AccessionNo: Accession, Datasource: datasource);
                result.steps[++ExecutedSteps].StepPass();

                //Step 3
                studies.SelectStudy("Accession", Accession);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //step-4
                var prior = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors))[0];
                new TestCompleteAction().MoveToElement(prior).Perform();
                var tooltip = prior.GetAttribute("title");
                var expected_tooltip = name + Environment.NewLine +
                                       "Birthdate: " + dob + "  Sex: " + gender + Environment.NewLine +
                                       "MRN: " + patientID + Environment.NewLine +
                                       "IPID: " + ipid + Environment.NewLine +
                                       Environment.NewLine +
                                       studyadte + Environment.NewLine +
                                       studydesc + Environment.NewLine +
                                       modality + Environment.NewLine +
                                       "Acc#: " + Accession + Environment.NewLine +
                                       "DataSource: " + datasource + Environment.NewLine;
                Logger.Instance.InfoLog("The Expected ToolTip is--" + expected_tooltip);
                Logger.Instance.InfoLog("The Actual ToolTip is--" + tooltip);
                if (tooltip.Equals(expected_tooltip))
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
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
        /// Verify the patient prior studies shall display when the user filter the prior studies by using Modality,Site and Sort By dropdown in the Exam List
        /// </summary>
        public TestCaseResult Test_161616(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String patientID = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID"));
                String name = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name"));
                String dob = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOB"));
                String gender = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "Gender"));
                String ipid = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPID"));
                String priorDate = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorDate"));
                String modality = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorModality"));
                String studydesc = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDesc"));
                String datasource = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DataSourceList"));
                String studyDate = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDate"));


                //Step 1 - Login to WebAccess site with any privileged user.
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                result.steps[++ExecutedSteps].StepPass();

                //Step 2 -From the Studies tab, search for a study with multiple priors and different modalities. Load the study into the Universal Viewer.               
                studies.SearchStudy(AccessionNo: accession, Datasource: datasource);
                studies.SelectStudy("Accession", accession);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].StepPass();

                //Step 3 - Verify that the current and related studies are listed in Exam List.
                IList<IWebElement> prior_3 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
                int expPriorCount = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorCount"));

                if (bluringviewer.IsAllPriorsDisplayed() && prior_3.Count == expPriorCount)
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

                //Step 4 - Verify that the labels "EXAM LIST", "History", "Modality", "Sort By" exist.
                bool isExamListHistoryModalitySortByExist = ValidateLocalization(BluringViewer_MappingFilePath, "PatientHistory", path: "en-US", viewer: "bluring");
                if (isExamListHistoryModalitySortByExist)
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

                //Step 5 -Verify that the study card detail contains. 
                //Date Time
                //Modality / Modalities
                //Study Description
                //Accession Number
                ++ExecutedSteps;
                String teststepdata5 = testid + "_5";

                String accession5 = (String)ReadExcel.GetTestData(filepath, "TestData", teststepdata5, "AccessionID");
                String priorDate5 = ((String)ReadExcel.GetTestData(filepath, "TestData", teststepdata5, "PriorDate"));
                String modality5 = ((String)ReadExcel.GetTestData(filepath, "TestData", teststepdata5, "PriorModality"));
                String studydesc5 = ((String)ReadExcel.GetTestData(filepath, "TestData", teststepdata5, "StudyDesc"));
                String studyDate5 = ((String)ReadExcel.GetTestData(filepath, "TestData", teststepdata5, "StudyDate"));
                int testPriorIndex5 = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", teststepdata5, "TestPriorIndex"));

                String examCardDateRetrieved = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors))[testPriorIndex5].GetAttribute("innerText");

                String examCardExpected = studyDate5 + modality5 + studydesc5 + "Acc: " + accession5;

                if (examCardDateRetrieved.Replace("\r", "").Replace("\n", "").Equals(examCardExpected))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.ErrorLog("ExamCardExpected--->" + examCardExpected);
                    Logger.Instance.ErrorLog("ExamCardRetrieved--->" + examCardDateRetrieved);

                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("ExamCardExpected--->" + examCardExpected);
                    Logger.Instance.ErrorLog("ExamCardRetrieved--->" + examCardDateRetrieved);

                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6 - Mouse-hover over a study card detail.
                ++ExecutedSteps;
                try
                {

                    String teststepdata6 = testid + "_6";

                    String accession6 = (String)ReadExcel.GetTestData(filepath, "TestData", teststepdata6, "AccessionID");
                    String patientID6 = ((String)ReadExcel.GetTestData(filepath, "TestData", teststepdata6, "PatientID"));
                    String name6 = ((String)ReadExcel.GetTestData(filepath, "TestData", teststepdata6, "Name"));
                    String dob6 = ((String)ReadExcel.GetTestData(filepath, "TestData", teststepdata6, "DOB"));
                    String gender6 = ((String)ReadExcel.GetTestData(filepath, "TestData", teststepdata6, "Gender"));
                    String ipid6 = ((String)ReadExcel.GetTestData(filepath, "TestData", teststepdata6, "IPID"));
                    String priorDate6 = ((String)ReadExcel.GetTestData(filepath, "TestData", teststepdata6, "PriorDate"));
                    String modality6 = ((String)ReadExcel.GetTestData(filepath, "TestData", teststepdata6, "PriorModality"));
                    String studydesc6 = ((String)ReadExcel.GetTestData(filepath, "TestData", teststepdata6, "StudyDesc"));
                    String datasource6 = ((String)ReadExcel.GetTestData(filepath, "TestData", teststepdata6, "DataSourceList"));
                    String studyDate6 = ((String)ReadExcel.GetTestData(filepath, "TestData", teststepdata6, "StudyDate"));
                    int testPriorIndex = Int32.Parse((String)ReadExcel.GetTestData(filepath, "TestData", teststepdata6, "TestPriorIndex"));

                    var prior6 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors))[testPriorIndex];
                    bluringviewer.HoverElement(prior6);
                    var tooltip6 = prior6.GetAttribute("title");

                    string expectedtooltip6 = name6 + ", " + Environment.NewLine + "Birthdate: " + dob6 + " Sex: " + gender6 + Environment.NewLine + "MRN: " +
                                               patientID6 + Environment.NewLine + "IPID: " + ipid6 + Environment.NewLine + Environment.NewLine + studyDate6 + Environment.NewLine + studydesc6 +
                                               Environment.NewLine + modality6 + Environment.NewLine + "Acc#: " + accession6 + Environment.NewLine + Environment.NewLine + "DataSource: " + datasource6 +
                                               Environment.NewLine;

                    if (expectedtooltip6.Equals(tooltip6))
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
                catch (Exception e)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 7 -Click on the (x) Exit icon to close the Viewer, go back to the study list and search and select a study with no Accession number and click Universal button
                ++ExecutedSteps;

                bluringviewer.CloseBluRingViewer();
                String teststepdata7 = testid + "_2";
                String lastName7 = ((String)ReadExcel.GetTestData(filepath, "TestData", teststepdata7, "LastName"));
                String patientID7 = ((String)ReadExcel.GetTestData(filepath, "TestData", teststepdata7, "PatientID"));

                studies.SearchStudy(LastName: lastName7);
                studies.SelectStudy("Patient ID", patientID7);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[ExecutedSteps].status = "Pass";

                //Step 8 - Verify the label ACC#: should be displayed study card tooltip.
                try
                {
                    ++ExecutedSteps;

                    var prior8 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors))[0];
                    bluringviewer.HoverElement(prior8);
                    var tooltip8 = prior8.GetAttribute("title");

                    if (tooltip8.Contains("Acc#: "))
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
                catch (Exception e)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 9 - Click on the (x) Exit icon to close the Viewer, go back to the study list and search and select a study with no IPID value and click "Universal " button
                ++ExecutedSteps;

                bluringviewer.CloseBluRingViewer();
                String teststepdata8 = testid + "_3";
                String lastName8 = ((String)ReadExcel.GetTestData(filepath, "TestData", teststepdata8, "LastName"));
                String firstName8 = ((String)ReadExcel.GetTestData(filepath, "TestData", teststepdata8, "FirstName"));

                studies.SearchStudy(LastName: lastName8);
                studies.SelectStudy("Patient Name", lastName8 + ", " + firstName8 + " ");
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[ExecutedSteps].status = "Pass";

                //Step 10 - Verify the label "IPID" should be displayed when the user mouse - hover over a study card detail.
                try
                {
                    ++ExecutedSteps;

                    var prior8 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors))[0];
                    bluringviewer.HoverElement(prior8);
                    var tooltip8 = prior8.GetAttribute("title");

                    if (tooltip8.Contains("IPID: "))
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
                catch (Exception e)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Logout Application
                bluringviewer.CloseBluRingViewer();
                login.Logout();

                // Result
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
        /// Test 145273  - IPID: label in Study card tool tip for the study which has no IPID value
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161661(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                string patientID = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID"));
                string name = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "Name"));
                string dob = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "DOB"));
                string gender = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "Gender"));
                //string ipid = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPID"));
                string studyadte = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorDate"));
                string modality = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "PriorModality"));
                string studydesc = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyDesc"));
                string datasource = login.GetHostName(Config.EA77);
                string Accession = ((String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID"));

                //Step-1
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].StepPass();

                //step-2
                var studies = login.Navigate<Studies>();
                studies.SearchStudy(AccessionNo: Accession, Datasource: datasource);
                studies.SelectStudy("Accession", Accession);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //step-3
                var prior = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors))[0];
                new TestCompleteAction().MoveToElement(prior).Perform();
                var tooltip = prior.GetAttribute("title");
                var expected_tooltip = name + Environment.NewLine +
                                       "Birthdate: " + dob + "  Sex: " + gender + Environment.NewLine +
                                       "MRN: " + patientID + Environment.NewLine +
                                       "IPID: " + Environment.NewLine +
                                       Environment.NewLine +
                                       studyadte + Environment.NewLine +
                                       studydesc + Environment.NewLine +
                                       modality + Environment.NewLine +
                                       "Acc#: " + Accession + Environment.NewLine +
                                       "DataSource: " + datasource + Environment.NewLine;
                Logger.Instance.InfoLog("The Expected ToolTip is--" + expected_tooltip);
                Logger.Instance.InfoLog("The Actual ToolTip is--" + tooltip);
                if (tooltip.Equals(expected_tooltip))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step-4
                result.steps[++ExecutedSteps].StepPass();

                viewer.CloseBluRingViewer();
                //Logout Application 
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
