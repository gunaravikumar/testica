using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Selenium.Scripts.Pages;
using Selenium.Scripts;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Reusable.Generic;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Application = TestStack.White.Application;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using System.Drawing;
using Selenium.Scripts.Pages.MergeServiceTool;
using OpenQA.Selenium.Remote;

namespace Selenium.Scripts.Tests
{
     
    class Scaling 
    {
        public Scaling(String classname)
        {
            login = new Login();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            string browsertype = Config.BrowserType;
            SetFontSize();
        }


        public Login login { get; set; }
        public string filepath { get; set; }
        public WpfObjects wpfobject;
        public string browsertype = null;

        public string thumbnailFontLarge = "12.13px";
        public string thumbnailFontMedium = "10.26px";
        public string thumbnailFontSmall = "8.4px";

        public string UsersettingSmall = "Small";
        public string UsersettingLarge = "Large";
        public string UsersettingMedium = "Medium";

        //StudyDate at study panel
        // Date, time, info
        public string[] studyInformationAtStudyPanelLarge = null;
        public string[] studyInformationAtStudyPanelMedium = null;
        public string[] studyInformationAtStudyPanelSmall = null;

        //Patinetname, DOB, ID, ExamLable, Show-Hide Lable,exitviewer
        public string[] GlobalToolBarItemFontSizeLarge = null;
        public string[] GlobalToolBarItemFontSizeMedium = null;
        public string[] GlobalToolBarItemFontSizeSmall = null;

        //ExamListOperationContainer
        //ModalityLable, Sitelable, SortByLable, ModalityDropDown,SiteDropDown, SortByDropDown
        public string ExamListOperationContainerFontLarge = null;
        public string ExamListOperationContainerFontMedium = null;
        public string ExamListOperationContainerFontSmall = null;

        //ExamListLable
        public string ExamListLableLarge = null;
        public string ExamListLableMedium = null;
        public string ExamListLableSmall = null;

        //  //ListContainer- Recent study list
        // Date,time,modality,contrast, site
        public string[] listContainerFontLarge = null;
        public string[] listContainerFontMedium = null;
        public string[] listContainerFontSmall = null;

        public string RecentStudyThumbnailFontsLarge = null;
        public string RecentStudyThumbnailFontsMedium = null;
        public string RecentStudyThumbnailFontsSmall = null;

        public string UserSettingsListFontLarge = null;
        public string UserSettingsListFontMedium = null;
        public string UserSettingsListFontSmall = null;

        string sample = null;

        public void SetFontSize()
        {
            if (Config.BrowserType.ToLower() == "chrome" || Config.BrowserType.ToLower().Contains("firefox") )
            {
                thumbnailFontLarge = "11.55px";
                thumbnailFontMedium = "11px";
                thumbnailFontSmall = "10.45px";

                //StudyDate at study panel
                // Date, time, info
                studyInformationAtStudyPanelLarge = new string[] { "16.8px", "16.8px", "12.6px" };
                studyInformationAtStudyPanelMedium = new string[] { "16px", "16px", "12px" };
                studyInformationAtStudyPanelSmall = new string[] { "15.2px", "15.2px", "11.4px" };

                //Patinetname, DOB, ID, ExamLable, Show-Hide Lable,exitviewer
                GlobalToolBarItemFontSizeLarge = new string[] { "18.9px", "12.6px", "12.6px", "9.8px", "9.8px", "9.8px" };
                GlobalToolBarItemFontSizeMedium = new string[] { "18px", "12px", "12px", "9.33px", "9.33px", "9.33px" };
                GlobalToolBarItemFontSizeSmall = new string[] { "17.1px", "11.4px", "11.4px", "8.86px", "8.86px", "8.86px" };

                //ExamListOperationContainer
                //ModalityLable, Sitelable, SortByLable, ModalityDropDown,SiteDropDown, SortByDropDown
                ExamListOperationContainerFontLarge = "12.6px";
                ExamListOperationContainerFontMedium = "12px";
                ExamListOperationContainerFontSmall = "11.4px";

                //ExamListLable
                ExamListLableLarge = "16.8px";
                ExamListLableMedium = "16px";
                ExamListLableSmall = "15.2px";

                //  //ListContainer- Recent study list
                // Date,time,modality,contrast, site
                listContainerFontLarge = new string[] { "16.8px", "12.6px", "13.65px", "12.6px", "12.6px" };
                listContainerFontMedium = new string[] { "16px", "12px", "13px", "12px", "12px" };
                listContainerFontSmall = new string[] { "15.2px", "11.4px", "12.35px", "11.4px", "11.4px" };

                RecentStudyThumbnailFontsLarge = "11.55px";
                RecentStudyThumbnailFontsMedium = "11px";
                RecentStudyThumbnailFontsSmall = "10.45px";

                UserSettingsListFontLarge = "11.55px";
                UserSettingsListFontMedium = "11px";
                UserSettingsListFontSmall = "10.45px";
            }
            else if(Config.BrowserType.ToLower() == "ie" || Config.BrowserType.ToLower() == "explore")
            {

                thumbnailFontLarge = "11.55px";
                thumbnailFontMedium = "11px";
                thumbnailFontSmall = "10.45px";

                //StudyDate at study panel
                // Date, time, info
                studyInformationAtStudyPanelLarge = new string[] { "16.8px", "16.8px", "12.6px" };
                studyInformationAtStudyPanelMedium = new string[] { "16px", "16px", "12px" };
                studyInformationAtStudyPanelSmall = new string[] { "15.2px", "15.2px", "11.4px" };

                //Patinetname, DOB, ID, ExamLable, Show-Hide Lable,exitviewer
                GlobalToolBarItemFontSizeLarge = new string[] { "18.9px", "12.6px", "12.6px", "9.8px", "9.8px", "9.8px" };
                GlobalToolBarItemFontSizeMedium = new string[] { "18px", "12px", "12px", "9.33px", "9.33px", "9.33px" };
                GlobalToolBarItemFontSizeSmall = new string[] { "17.1px", "11.4px", "11.4px", "8.86px", "8.86px", "8.86px" };


                //ExamListOperationContainer
                //ModalityLable, Sitelable, SortByLable, ModalityDropDown,SiteDropDown, SortByDropDown
                ExamListOperationContainerFontLarge = "12.6px";
                ExamListOperationContainerFontMedium = "12px";
                ExamListOperationContainerFontSmall = "11.4px";

                //ExamListLable
                ExamListLableLarge = "16.8px";
                ExamListLableMedium = "16px";
                ExamListLableSmall = "15.2px";

                //  //ListContainer- Recent study list
                // Date,time,modality,contrast, site
                listContainerFontLarge = new string[] { "16.8px", "12.6px", "13.65px", "12.6px", "12.6px" };
                listContainerFontMedium = new string[] { "16px", "12px", "13px", "12px", "12px" };
                listContainerFontSmall = new string[] { "15.2px", "11.4px", "12.35px", "11.4px", "11.4px" };

                RecentStudyThumbnailFontsLarge = "11.55px";
                RecentStudyThumbnailFontsMedium = "11px";
                RecentStudyThumbnailFontsSmall = "10.45px";

                UserSettingsListFontLarge = "11.55px";
                UserSettingsListFontMedium = "11px";
                UserSettingsListFontSmall = "10.45px";
            }
        }

        #region Chrome Browser

        #endregion


        /// <summary> 
        /// 136726 - This Test Case is  Verify that font size changes by selecting User Control Tools in the Viewer page
        /// </summary>
        ///
        public TestCaseResult Test_161551(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                string Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                string Datasource = login.GetHostName(Config.EA91);

                // Step 1
                //Launch the BluRing application with a client browser (http//<BR IP>/WebAccess/) and hit enter  
                //The BluRing application login page should be displayed
                login.DriverGoTo(login.url);
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 2
                //Login to WebAccess site with any privileged user.
                //Should be able to login to the BluRing application.
                login.LoginIConnect(adminusername, adminpassword);
                result.steps[++ExecutedSteps].status = "Pass";

                // Step 3
                //Click on Studies tab
                //List of Studies should be displayed.
                Studies study = (Studies)login.Navigate("Studies");

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

                // Step 4
                //Search and select a study, and click onÂ BluRing ViewÂ button
                //Selected study should be opened in BluRing Viewer
                //Accession: DSQ00000135
                study.SearchStudy(AccessionNo: Accession, Datasource: Datasource);
                PageLoadWait.WaitForLoadingMessage(20);
                PageLoadWait.WaitForSearchLoad();
                study.SelectStudy("Accession", Accession);
                BluRingViewer bluringviewer = BluRingViewer.LaunchBluRingViewer();
                // COmplete laoding and gold image comaprision and add datasource in search study 
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForFrameLoad(30);

                //PreCondition for Step 6,7,8
                // Take the height and Width of All view port for verfication
                string[] AllViewPortHeight = bluringviewer.AllViewPorts().Select(avp => avp.GetCssValue("height")).ToArray();
                string[] AllViewPortWidth = bluringviewer.AllViewPorts().Select(avp => avp.GetCssValue("width")).ToArray();


                //Step 5
                //Search for User Settings at the top right corner in the Viewer page
                //User Settings should be available at the top right corner with User Control tools as UI - LARGE , UI - MEDIUM and UI-SMALL
                if ((bluringviewer.UserSettings("displayed", UsersettingLarge) && bluringviewer.UserSettings("displayed", UsersettingMedium) && bluringviewer.UserSettings("displayed", UsersettingSmall)))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 6
                //Click on UI-LARGE user control tool under User Settings
                //User settings options is disappeared when user selects the UI-Large user control tool under the User Settings.
                bool selected = bluringviewer.UserSettings("select", UsersettingLarge);
                bool UserSettingsLarge = (selected && bluringviewer.UserSettings("checked", UsersettingLarge));

                // Take the height and Width of All view port for verfication
                string[] AllViewPortHeightAfterSelectLarge = bluringviewer.AllViewPorts().Select(avp => avp.GetCssValue("height")).ToArray();
                string[] AllViewPortWidthAfterSelectLarge = bluringviewer.AllViewPorts().Select(avp => avp.GetCssValue("width")).ToArray();

                bool viewportDimension = (AllViewPortHeightAfterSelectLarge.SequenceEqual(AllViewPortHeight) && AllViewPortWidthAfterSelectLarge.SequenceEqual(AllViewPortWidth));
                if (!viewportDimension) Logger.Instance.ErrorLog("View port not remain same after changing the user setting to UI-Large");

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool stepstatus6 = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());

                bool ViewPort = (stepstatus6 && viewportDimension);

                string PatinetNameFontSize = bluringviewer.PatinetName().GetCssValue("font-size");
                IList<IWebElement> PatientDemoDetailList = bluringviewer.PatientDemoDetailList();
                //  bool patdemo = PatientDemoDetailList.All(pd => string.Equals(GlobalToolBarItemFontSizeLarge[1], pd.GetCssValue("font-size")));
                bool PatientDemoDetails = true;
                foreach (IWebElement EachPatinetDemoDetails in PatientDemoDetailList)
                    if (EachPatinetDemoDetails.GetCssValue("font-size") != GlobalToolBarItemFontSizeLarge[1])
                    {
                        PatientDemoDetails = false;
                        break;
                    }
                ////string //ExamIconLable = bluringviewer.ExamTextLable().GetCssValue("font-size");
                ////string // ShowHideIconlable = bluringviewer.ShowHideToolName().GetCssValue("font-size");

                bool patinetnameFontsVerfiy = (PatinetNameFontSize == GlobalToolBarItemFontSizeLarge[0]);
                //bool ExamIconLableVerfiy = (ExamIconLable == GlobalToolBarItemFontSizeLarge[3]);
                //bool ShowHideIconlableVerfiy = (ShowHideIconlable == GlobalToolBarItemFontSizeLarge[4]);
                //bool ExitIconlableVerfiy = (ExitIconlable == GlobalToolBarItemFontSizeLarge[5]);

               // bool globalToolBars = patinetnameFontsVerfiy && PatientDemoDetails && ExitIconlableVerfiy;

                //Verify the Study Info font size which is available above the Thumbnails should get changed to Large in Size
                //Study Info font size which is available above the Thumbnails is changed to Large in Size
                List<IWebElement> AllStudyDateTimeAtStudyPanel = new List<IWebElement>();
                AllStudyDateTimeAtStudyPanel.AddRange(bluringviewer.AllStudyDateAtStudyPanel().ToList());
                bool boolAllStudyDateAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyDateTimeAtStudyPanel).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[0]));

                List<IWebElement> AllStudyTimeAtStudyPanel = new List<IWebElement>();
                AllStudyTimeAtStudyPanel.AddRange(bluringviewer.AllStudyTimeAtStudyPanel().ToList());
                Logger.Instance.InfoLog("StudyTime Font Values Test:" + AllStudyTimeAtStudyPanel.First() + "code:" + studyInformationAtStudyPanelLarge[1]);
                bool boolAllStudyTimeAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanel).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[1]));

                List<IWebElement> AllStudyInfoAtStudyPanel = new List<IWebElement>();
                AllStudyInfoAtStudyPanel.AddRange(bluringviewer.AllStudyInfoAtStudyPanel().ToList());
                Logger.Instance.InfoLog("Study Info Font Values Test:" + AllStudyInfoAtStudyPanel.First() + "code:" + studyInformationAtStudyPanelLarge[2]);
                bool boolAllStudyInfoAtStudyPanel = bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanel).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[2]));

                // Verfiy the Thumbnail fonts in the thumbnail 
                List<IWebElement> thumbnail = new List<IWebElement>();
                thumbnail.AddRange(bluringviewer.ThumbnailPercentImagesViewed().ToList());
                thumbnail.AddRange(bluringviewer.ThumbnailPercentImagesViewed().ToList());
                thumbnail.AddRange(bluringviewer.ThumbnailImageFrameNumber().ToList());

                string[] thumbnailFonts = bluringviewer.getFontSizeofElements(thumbnail);

                bool studyInfoandThumbnail = (thumbnailFonts.All(c1 => c1.Equals(thumbnailFontLarge)) && boolAllStudyDateAtStudypanel && boolAllStudyTimeAtStudypanel && boolAllStudyInfoAtStudyPanel);

                //Verify the all menus(e.g.Exam List Filters, Filters drop down and Result List) font size in the "EXAM LIST" Panel should get changed to Large Size
                //All menus(Exam List Filters,Filters drop down and Result List ) font size in the "EXAM LIST" Panel are changed to Large Size
                // Exam list label;
                bool ExamListLable = (bluringviewer.ExamListLable().GetCssValue("font-size") == ExamListLableLarge);

                //Verfiy the Filter operator for the exam list 
                bluringviewer.OpenModalityFilter();
                string[] filterOperatorsFont = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text)).ToList());
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));

                bluringviewer.OpenSortDorpdown();
                string[] sortdropdown = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudySortOptionValues)).ToList());
                bool examOperators = filterOperatorsFont.All(c1 => c1.Equals(ExamListOperationContainerFontLarge)) && sortdropdown.All(c1 => c1.Equals(ExamListOperationContainerFontLarge));
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
               
                //Verfiy the RecentStudyDate Font
                string[] RecentStudyAllDatesFonts = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDates().ToList());
                Logger.Instance.InfoLog("Study Date Font Values Test:" + RecentStudyAllDatesFonts.First() + "code:" + listContainerFontLarge[0]);
                bool boolRecentStudyAllDatesFonts = RecentStudyAllDatesFonts.All(c1 => c1.Equals(listContainerFontLarge[0]));

                //Verfiy the RecentStudyTimesFont
                List<IWebElement> RecentStudyAllTimes = bluringviewer.RecentStudyAllTimes().ToList();
                string[] RecentStudyAllTimesFonts = bluringviewer.getFontSizeofElements(RecentStudyAllTimes);
                Logger.Instance.InfoLog("Recent study time font Values Test:" + RecentStudyAllTimesFonts.First() + "code:" + listContainerFontLarge[1]);
                bool boolRecentStudyAllTimesFonts = RecentStudyAllTimesFonts.All(c1 => c1.Equals(listContainerFontLarge[1]));

                //Verfiy the RecentStudyModalityFont
                List<IWebElement> RecentStudyAllModality = bluringviewer.RecentStudyAllModality().ToList();
                string[] RecentStudyAllModalityFonts = bluringviewer.getFontSizeofElements(RecentStudyAllModality);
                Logger.Instance.InfoLog("Recent study Modality font Values Test:" + RecentStudyAllModalityFonts.First() +"code:"+ listContainerFontLarge[2]);
                bool boolRecentStudyAllModalityFonts = RecentStudyAllModalityFonts.All(c1 => c1.Equals(listContainerFontLarge[2]));

                //Verfiy the RecentStudyContrastFont
                List<IWebElement> RecentStudyAllDescription = bluringviewer.RecentStudyAllDescription().ToList();
                string[] RecentStudyAllDescriptionFonts = bluringviewer.getFontSizeofElements(RecentStudyAllDescription);
                Logger.Instance.InfoLog("Recent study Description font Values Test:" + RecentStudyAllDescriptionFonts.First() + "code:" + listContainerFontLarge[3]);
                bool boolRecentStudyAllDescriptionFonts = RecentStudyAllDescriptionFonts.All(c1 => c1.Equals(listContainerFontLarge[3]));
                
                //Add site
                //Verfiy the RecentStudySiteFont
                List<IWebElement> RecentStudyAllAccession = bluringviewer.RecentStudyAllAccession().ToList();
                string[] RecentStudyAllAccessionFonts = bluringviewer.getFontSizeofElements(RecentStudyAllAccession);
                Logger.Instance.InfoLog("Recent study Accession font Values Test:" + RecentStudyAllDescriptionFonts.First() + "code:" + listContainerFontLarge[4]);
                bool boolRecentStudyAllAccessionFonts = RecentStudyAllAccessionFonts.All(c1 => c1.Equals(listContainerFontLarge[4]));

                bool examPanel = (ExamListLable && examOperators && boolRecentStudyAllDatesFonts && boolRecentStudyAllTimesFonts && boolRecentStudyAllModalityFonts && boolRecentStudyAllDescriptionFonts && boolRecentStudyAllAccessionFonts);

                //Step 6
                //Click on UI-LARGE user control tool under User Settings
                //	Font size in the viewer page should be changed to Large size and UI-LARGE user control tool is marked as Checked under User Settings but the viewports remain same
                if ( studyInfoandThumbnail && examPanel && UserSettingsLarge && viewportDimension)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.InfoLog("");
                }

                //Step 7
                //Click on UI-MEDIUM user control tool under User Settings
                selected = bluringviewer.UserSettings("select", UsersettingMedium);

                // Take the height and Width of All view port for verfication
                string[] AllViewPortHeightAfterSelectMedium = bluringviewer.AllViewPorts().Select(avp => avp.GetCssValue("height")).ToArray();
                string[] AllViewPortWidthAfterSelectMedium = bluringviewer.AllViewPorts().Select(avp => avp.GetCssValue("width")).ToArray();

                viewportDimension = (AllViewPortHeightAfterSelectMedium.SequenceEqual(AllViewPortHeight) && AllViewPortWidthAfterSelectMedium.SequenceEqual(AllViewPortWidth));
                if (!viewportDimension) Logger.Instance.ErrorLog("View port not remain same after changing the user setting to UI-Medium");

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool stepstatus7 = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());

                ViewPort = viewportDimension && stepstatus7;

                //Verify the Global Toolbar menus (Patient Info,EXAMS, Show/Hide Tool ,EXIT) font size should get changed to Medium size in the viewer page.
                //Global toolbar menus font size is changed to MEDIUM size when the user selects the UI-MEDIUM control tool.
                PatinetNameFontSize = bluringviewer.PatinetName().GetCssValue("font-size");
                Logger.Instance.InfoLog("PatinetNameFontSize Medium font Values Test:" + PatinetNameFontSize + "code:" + GlobalToolBarItemFontSizeMedium[0]);
               // var PatinetNameFont = bluringviewer.PatinetName().GetCssValue("font-size");
                IList<IWebElement> PatientDemoDetailListMedium = bluringviewer.PatientDemoDetailList();
                Logger.Instance.InfoLog("PatientDemoDetailListMedium  font Values Test:" + PatientDemoDetailListMedium.First() + "code:" + GlobalToolBarItemFontSizeMedium[1]);
                PatientDemoDetails = true;
                foreach (IWebElement EachPatinetDemoDetails in PatientDemoDetailListMedium)
                {
                    if (EachPatinetDemoDetails.GetCssValue("font-size") != GlobalToolBarItemFontSizeMedium[1])
                    {
                        PatientDemoDetails = false;
                        break;
                    }
                }

                ////ExamIconLable = bluringviewer.ExamTextLable().GetCssValue("font-size");
                //Logger.Instance.InfoLog("ExamIconLable Medium  font Values Test:" + ExamIconLable + "code:" + GlobalToolBarItemFontSizeMedium[3]);
                //// ShowHideIconlable = bluringviewer.ShowHideToolName().GetCssValue("font-size");
                //Logger.Instance.InfoLog("ShowHideIconlable Medium  font Values Test:" + ShowHideIconlable + "code:" + GlobalToolBarItemFontSizeMedium[4]);

                patinetnameFontsVerfiy = (PatinetNameFontSize == GlobalToolBarItemFontSizeMedium[0]);
                //ExamIconLableVerfiy = (ExamIconLable == GlobalToolBarItemFontSizeMedium[3]);
               // ShowHideIconlableVerfiy = (ShowHideIconlable == GlobalToolBarItemFontSizeMedium[4]);
               // ExitIconlableVerfiy = (ExitIconlable == GlobalToolBarItemFontSizeMedium[5]);

                bool GobalToolBars = (patinetnameFontsVerfiy && PatientDemoDetails );

                //Verify the Study Info font size which is available above the Thumbnails should get changed to Medium in Size
                //Study Info font size which is available above the Thumbnails is h changed to Medium in Size
                List<IWebElement> AllStudyDateTimeAtStudyPanelMedium = new List<IWebElement>();
                AllStudyDateTimeAtStudyPanelMedium.AddRange(bluringviewer.AllStudyDateAtStudyPanel().ToList());
                Logger.Instance.InfoLog("StudyDateTimeAtStudyPanelMedium font Values Test:" + AllStudyDateTimeAtStudyPanelMedium.First() + "code:" + studyInformationAtStudyPanelMedium[0]);
                boolAllStudyDateAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyDateTimeAtStudyPanelMedium).All(c1 => c1.Equals(studyInformationAtStudyPanelMedium[0]));

                List<IWebElement> AllStudyTimeAtStudyPanelMedium = new List<IWebElement>();
                AllStudyTimeAtStudyPanelMedium.AddRange(bluringviewer.AllStudyTimeAtStudyPanel().ToList());
                Logger.Instance.InfoLog("StudyTimeAtStudyPanelMedium font Values Test:" + AllStudyTimeAtStudyPanelMedium.First() + "code:" + studyInformationAtStudyPanelMedium[1]);
                boolAllStudyTimeAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanelMedium).All(c1 => c1.Equals(studyInformationAtStudyPanelMedium[1]));

                List<IWebElement> AllStudyInfoAtStudyPanelMedium = new List<IWebElement>();
                AllStudyInfoAtStudyPanelMedium.AddRange(bluringviewer.AllStudyInfoAtStudyPanel().ToList());
                Logger.Instance.InfoLog("StudyInfoAtStudyPanelMedium font Values Test:" + AllStudyInfoAtStudyPanelMedium.First() + "code:" + studyInformationAtStudyPanelMedium[2]);
                boolAllStudyInfoAtStudyPanel = bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanelMedium).All(c1 => c1.Equals(studyInformationAtStudyPanelMedium[2]));

                // Verfiy the Thumbnail fonts in the thumbnail 
                List<IWebElement> thumbnailMedium = new List<IWebElement>();
                thumbnailMedium.AddRange(bluringviewer.ThumbnailPercentImagesViewed().ToList());
                thumbnailMedium.AddRange(bluringviewer.ThumbnailPercentImagesViewed().ToList());
                thumbnailMedium.AddRange(bluringviewer.ThumbnailImageFrameNumber().ToList());

                thumbnailFonts = bluringviewer.getFontSizeofElements(thumbnailMedium);
                Logger.Instance.InfoLog("thumbnailFonts Medium font Values Test:" + thumbnailFonts.First() + "code:" + thumbnailFontMedium);

                bool StudyInfoAndThumbnail = (thumbnailFonts.All(c1 => c1.Equals(thumbnailFontMedium)) && boolAllStudyDateAtStudypanel && boolAllStudyTimeAtStudypanel && boolAllStudyInfoAtStudyPanel);

                //Verify the all menus(Exam List Filters, Filters drop down and Result List) font size in the "EXAM LIST" Panel should get changed to Medium Size
                //All menus(Exam List Filters,Filters drop down and Result List ) font size in the "EXAM LIST" Panel are changed to Medium Size
                // Exam list label;
                ExamListLable = (bluringviewer.ExamListLable().GetCssValue("font-size") == ExamListLableMedium);

                //Verfiy the Filter operator for the exam list 
                bluringviewer.OpenModalityFilter();
                filterOperatorsFont = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text)).ToList());
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));

                bluringviewer.OpenSortDorpdown();
                sortdropdown = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudySortOptionValues)).ToList());
                examOperators = filterOperatorsFont.All(c1 => c1.Equals(ExamListOperationContainerFontMedium)) && sortdropdown.All(c1 => c1.Equals(ExamListOperationContainerFontMedium));
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();

                //Verfiy the RecentStudyDate Font
                RecentStudyAllDatesFonts = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDates().ToList());
                boolRecentStudyAllDatesFonts = RecentStudyAllDatesFonts.All(c1 => c1.Equals(listContainerFontMedium[0]));

                //Verfiy the RecentStudyTimesFont
                List<IWebElement> RecentStudyAllTimesMedium = bluringviewer.RecentStudyAllTimes().ToList();
                string[] RecentStudyAllTimesFontsMedium = bluringviewer.getFontSizeofElements(RecentStudyAllTimesMedium);
                boolRecentStudyAllTimesFonts = RecentStudyAllTimesFontsMedium.All(c1 => c1.Equals(listContainerFontMedium[1]));

                //Verfiy the RecentStudyModalityFont
                List<IWebElement> RecentStudyAllModalityMedium = bluringviewer.RecentStudyAllModality().ToList();
                string[] RecentStudyAllModalityFontsMedium = bluringviewer.getFontSizeofElements(RecentStudyAllModalityMedium);
                boolRecentStudyAllModalityFonts = RecentStudyAllModalityFontsMedium.All(c1 => c1.Equals(listContainerFontMedium[2]));

                //Verfiy the RecentStudyContrastFont
                List<IWebElement> RecentStudyAllDescriptionMedium = bluringviewer.RecentStudyAllDescription().ToList();
                string[] RecentStudyAllDescriptionFontsMedium = bluringviewer.getFontSizeofElements(RecentStudyAllDescriptionMedium);
                boolRecentStudyAllDescriptionFonts = RecentStudyAllDescriptionFontsMedium.All(c1 => c1.Equals(listContainerFontMedium[3]));

                //Add site
                //Verfiy the RecentStudySiteFont
                List<IWebElement> RecentStudyAllAccessionMedium = bluringviewer.RecentStudyAllAccession().ToList();
                string[] RecentStudyAllAccessionFontsMedium = bluringviewer.getFontSizeofElements(RecentStudyAllAccessionMedium);
                boolRecentStudyAllAccessionFonts = RecentStudyAllAccessionFontsMedium.All(c1 => c1.Equals(listContainerFontMedium[4]));

                bool ExamPanel = (ExamListLable && examOperators && boolRecentStudyAllDatesFonts && boolRecentStudyAllTimesFonts && boolRecentStudyAllModalityFonts && boolRecentStudyAllDescriptionFonts && boolRecentStudyAllAccessionFonts);

                //Step 7
                //Click on UI-Medium user control tool under User Settings
                //	Font size in the viewer page should be changed to Medium size and UI-Medium user control tool is marked as Checked under User Settings but the viewports remain same
                if (GobalToolBars && StudyInfoAndThumbnail && ExamPanel && viewportDimension )
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step 8
                //Click on UI-Small user control tool under User Settings
                selected = bluringviewer.UserSettings("select", UsersettingSmall);

               bool UserSettings = (selected && bluringviewer.UserSettings("checked", UsersettingSmall));

                // Take the height and Width of All view port for verfication
                string[] AllViewPortHeightAfterSelectSmall = bluringviewer.AllViewPorts().Select(avp => avp.GetCssValue("height")).ToArray();
                string[] AllViewPortWidthAfterSelectSmall = bluringviewer.AllViewPorts().Select(avp => avp.GetCssValue("width")).ToArray();

                viewportDimension = (AllViewPortHeightAfterSelectSmall.SequenceEqual(AllViewPortHeight) && AllViewPortWidthAfterSelectSmall.SequenceEqual(AllViewPortWidth));
                if (!viewportDimension) Logger.Instance.ErrorLog("View port not remain same after changing the user setting to UI-Small");

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool stepstatus8 = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());

                //Step 8 Verification
                //Verify the Image size should not changed to SMALL when user selects UI - SMALL control tool from the User Settings.
                //Image size is not changed to SMALL when user selects UI-SMALL control tool from the User Settings.
                bool ViewPortDimension = stepstatus8 && viewportDimension;

                //Verify the Global Toolbar menus (Patient Info,EXAMS, Show/Hide Tool ,EXIT) font size should get changed to small size in the viewer page.
                //Global toolbar menus font size is changed to SMALL size when the user selects the UI-SMALL control tool.
                PatinetNameFontSize = bluringviewer.PatinetName().GetCssValue("font-size");
                IList<IWebElement> PatientDemoDetailListSmall = bluringviewer.PatientDemoDetailList();
                PatientDemoDetails = true;
                foreach (IWebElement EachPatinetDemoDetails in PatientDemoDetailListSmall)
                {
                    if (EachPatinetDemoDetails.GetCssValue("font-size") != GlobalToolBarItemFontSizeSmall[1])
                    {
                        PatientDemoDetails = false;
                        break;
                    }
                }

                ////ExamIconLable = bluringviewer.ExamTextLable().GetCssValue("font-size");
                //// ShowHideIconlable = bluringviewer.ShowHideToolName().GetCssValue("font-size");

                patinetnameFontsVerfiy = (PatinetNameFontSize == GlobalToolBarItemFontSizeSmall[0]);
               // //ExamIconLableVerfiy = (ExamIconLable == GlobalToolBarItemFontSizeSmall[3]);
               //// ShowHideIconlableVerfiy = (ShowHideIconlable == GlobalToolBarItemFontSizeSmall[4]);
               // ExitIconlableVerfiy = (ExitIconlable == GlobalToolBarItemFontSizeSmall[5]);

                bool GlobalToolBar = (patinetnameFontsVerfiy && PatientDemoDetails );

                //Verify the Study Info font size which is available above the Thumbnails should get changed to small in Size
                //Study Info font size which is available above the Thumbnails is h changed to small in Size.
                List<IWebElement> AllStudyDateTimeAtStudyPanelSmall = new List<IWebElement>();
                AllStudyDateTimeAtStudyPanelSmall.AddRange(bluringviewer.AllStudyDateAtStudyPanel().ToList());
                boolAllStudyDateAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyDateTimeAtStudyPanelSmall).All(c1 => c1.Equals(studyInformationAtStudyPanelSmall[0]));

                List<IWebElement> AllStudyTimeAtStudyPanelSmall = new List<IWebElement>();
                AllStudyTimeAtStudyPanelSmall.AddRange(bluringviewer.AllStudyTimeAtStudyPanel().ToList());
                boolAllStudyTimeAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanelSmall).All(c1 => c1.Equals(studyInformationAtStudyPanelSmall[1]));

                List<IWebElement> AllStudyInfoAtStudyPanelSmall = new List<IWebElement>();
                AllStudyInfoAtStudyPanelSmall.AddRange(bluringviewer.AllStudyInfoAtStudyPanel().ToList());
                boolAllStudyInfoAtStudyPanel = bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanelSmall).All(c1 => c1.Equals(studyInformationAtStudyPanelSmall[2]));

                // Verfiy the Thumbnail fonts in the thumbnail 
                //List<IWebElement> thumbnailPercentrViewedFont = bluringviewer.ThumbnailPercentImagesViewed().ToList(); ;
                //List<IWebElement> thumbnailCaptionFont = bluringviewer.ThumbnailPercentImagesViewed().ToList();
                //List<IWebElement> thumbnailimageFrameNumberFont = bluringviewer.ThumbnailImageFrameNumber().ToList();
                List<IWebElement> thumbnailSmall = new List<IWebElement>();
                thumbnailSmall.AddRange(bluringviewer.ThumbnailPercentImagesViewed().ToList());
                thumbnailSmall.AddRange(bluringviewer.ThumbnailPercentImagesViewed().ToList());
                thumbnailSmall.AddRange(bluringviewer.ThumbnailImageFrameNumber().ToList());

                thumbnailFonts = bluringviewer.getFontSizeofElements(thumbnailSmall);

                bool StudyInfoAndthumbnail = (thumbnailFonts.All(c1 => c1.Equals(thumbnailFontSmall)) && boolAllStudyDateAtStudypanel && boolAllStudyTimeAtStudypanel && boolAllStudyInfoAtStudyPanel);


                //Verify the all menus(e.g. Exam List Filters,Filters drop down and Result List ) font size in the "EXAM LIST" Panel should get changed to small Size.
                //All menus(e.g. Exam List Filters,Filters drop down and Result List ) font size in the "EXAM LIST" Panel should get changed to small Size.
                // Exam list label;
                ExamListLable = (bluringviewer.ExamListLable().GetCssValue("font-size") == ExamListLableSmall);

                //Verfiy the Filter operator for the exam list 
                bluringviewer.OpenModalityFilter();
                filterOperatorsFont = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text)).ToList());
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));

                bluringviewer.OpenSortDorpdown();
                sortdropdown = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudySortOptionValues)).ToList());
                examOperators = filterOperatorsFont.All(c1 => c1.Equals(ExamListOperationContainerFontSmall)) && sortdropdown.All(c1 => c1.Equals(ExamListOperationContainerFontSmall));
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();

                //Verfiy the RecentStudyDate Font
                RecentStudyAllDatesFonts = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDates().ToList());
                boolRecentStudyAllDatesFonts = RecentStudyAllDatesFonts.All(c1 => c1.Equals(listContainerFontSmall[0]));

                //Verfiy the RecentStudyTimesFont
                List<IWebElement> RecentStudyAllTimesSmall = bluringviewer.RecentStudyAllTimes().ToList();
                RecentStudyAllTimesFonts = bluringviewer.getFontSizeofElements(RecentStudyAllTimesSmall);
                boolRecentStudyAllTimesFonts = RecentStudyAllTimesFonts.All(c1 => c1.Equals(listContainerFontSmall[1]));

                //Verfiy the RecentStudyModalityFont
                List<IWebElement> RecentStudyAllModalitySmall = bluringviewer.RecentStudyAllModality().ToList();
                RecentStudyAllModalityFonts = bluringviewer.getFontSizeofElements(RecentStudyAllModalitySmall);
                boolRecentStudyAllModalityFonts = RecentStudyAllModalityFonts.All(c1 => c1.Equals(listContainerFontSmall[2]));

                //Verfiy the RecentStudyContrastFont
                List<IWebElement> RecentStudyAllDescriptionSmall = bluringviewer.RecentStudyAllDescription().ToList();
                RecentStudyAllDescriptionFonts = bluringviewer.getFontSizeofElements(RecentStudyAllDescriptionSmall);
                boolRecentStudyAllDescriptionFonts = RecentStudyAllDescriptionFonts.All(c1 => c1.Equals(listContainerFontSmall[3]));

                //Add site
                //Verfiy the RecentStudySiteFont
                List<IWebElement> RecentStudyAllAccessionSmall = bluringviewer.RecentStudyAllAccession().ToList();
                RecentStudyAllAccessionFonts = bluringviewer.getFontSizeofElements(RecentStudyAllAccessionSmall);
                boolRecentStudyAllAccessionFonts = RecentStudyAllAccessionFonts.All(c1 => c1.Equals(listContainerFontSmall[4]));

                bool Exampanel = (ExamListLable && examOperators && boolRecentStudyAllDatesFonts && boolRecentStudyAllTimesFonts && boolRecentStudyAllModalityFonts && boolRecentStudyAllDescriptionFonts && boolRecentStudyAllAccessionFonts);

                //Step 8
                //Click on UI-Medium user control tool under User Settings
                //	Font size in the viewer page should be changed to Medium size and UI-Medium user control tool is marked as Checked under User Settings but the viewports remain same
                if (GlobalToolBar && StudyInfoAndthumbnail && Exampanel && viewportDimension && UserSettings)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                bluringviewer.CloseBluRingViewer();
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);

                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                return result;
                //------------End of script---

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
        /// 161553 -  UI-Scaling - Large
        /// </summary>
        ///
        public TestCaseResult Test_161553(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                string Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                string Datasource = login.GetHostName(Config.EA91);

                // Step 1
                //Launch the BluRing application with a client browser (http//<BR IP>/WebAccess/) and hit enter  
                //The BluRing application login page should be displayed
                login.DriverGoTo(login.url);
                result.steps[++ExecutedSteps].StepPass();

                //Step 2
                //Login to WebAccess site with any privileged user.
                //Should be able to login to the BluRing application.
                login.LoginIConnect(adminusername, adminpassword);
                result.steps[++ExecutedSteps].StepPass();

                // Step 3
                //Click on Studies tab
                //List of Studies should be displayed.
                Studies study = (Studies)login.Navigate("Studies");

                if (login.IsTabSelected("Studies"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                // Step 4
                //Search and select a study, and click onÂ BluRing ViewÂ button //Accession: DSQ00000135
                study.SearchStudy(AccessionNo: Accession, Datasource: Datasource);
                PageLoadWait.WaitForPageLoad(20);
                study.SelectStudy("Accession", Accession);
                BluRingViewer bluringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass(); //Step 4
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);

                bluringviewer.OpenViewerToolsPOPUp();
                bluringviewer.SelectViewerTool(BluRingTools.Line_Measurement, isOpenToolsPOPup: false);
                bluringviewer.ApplyTool_LineMeasurement();

                //Step 8- PreCondition
                // Take the height and Width of All view port for verfication
                string[] AllViewPortHeight = bluringviewer.AllViewPorts().Select(avp => avp.GetCssValue("height")).ToArray();
                string[] AllViewPortWidth = bluringviewer.AllViewPorts().Select(avp => avp.GetCssValue("width")).ToArray();

                //Step 10 - PreCondition
                //Get the Logo height and width before change to Large for verfication
                IList<String> LogoHeightWidth = new List<String>();
                LogoHeightWidth.Add(bluringviewer.MergeLogo().GetCssValue("height"));
                LogoHeightWidth.Add(bluringviewer.MergeLogo().GetCssValue("width"));

                //Step 5
                //Search for User Settings at the top right corner in the Viewer page
                //User Settings should be available at the top right corner with User Control tools as UI - LARGE , UI - MEDIUM and UI-SMALL
                if ((bluringviewer.UserSettings("displayed", UsersettingSmall) && bluringviewer.UserSettings("displayed", UsersettingMedium) && bluringviewer.UserSettings("displayed", UsersettingLarge)))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 6
                //Click on UI-Large user control tool under the User Settings and Verify the User settings options should be disappeared.
                //User settings options is disappeared when user selects the UI-Large user control tool under the User Settings.
                bool selected = bluringviewer.UserSettings("select", UsersettingLarge);
                if (selected && !(bluringviewer.SettingPanel().Displayed))
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("User settings options is disappeared when user selects the UI-Large user control tool under the User Settings.");
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("User settings options is appeared after when user selects the UI-Large user control tool under the User Settings.");
                }

                //Step 7
                //Again,Click on User Settings icon and verify the UI-Large should get selected.
                //UI-Large user control tool is selected.
                if ((bluringviewer.UserSettings("checked", UsersettingLarge)))
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("The user setting 'ui - Large' is checked");
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("The user setting 'ui - Large' is not checked");
                }

                //Step 8 - Verify that the User settings tools(UI-LARGE,UI-MEDIUM UI-SMALL) font size should be Large
                if (!bluringviewer.SettingPanel().Displayed)
                    bluringviewer.ClickOnUSerSettings();
                IList<IWebElement> UserControlToolsList = BasePage.FindElementsByCss(BluRingViewer.div_userControlToolsList);
                if (UserControlToolsList.All(settings => settings.GetCssValue("font-size").Equals( UserSettingsListFontLarge)))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 9 Click on 'User Settings' icon and verify that the User settings options should be disappeared.
                if (bluringviewer.SettingPanel().Displayed)
                    bluringviewer.ClickOnUSerSettings();
                if (!(bluringviewer.SettingPanel().Displayed))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("User Seeting tools Not disappeared after clicking on the User Settings Icon");
                }

                //Step 10
                //Image size is not changed to Large when user selects UI-Large control tool from the User Settings.
                string[] AllViewPortHeightAfterSelectLarge = bluringviewer.AllViewPorts().Select(avp => avp.GetCssValue("height")).ToArray();
                string[] AllViewPortWidthAfterSelectLarge = bluringviewer.AllViewPorts().Select(avp => avp.GetCssValue("width")).ToArray();
                bool viewportDimension = (AllViewPortHeightAfterSelectLarge.SequenceEqual(AllViewPortHeight) && AllViewPortWidthAfterSelectLarge.SequenceEqual(AllViewPortWidth));
                if (!viewportDimension) Logger.Instance.ErrorLog("View port not remain same after changing the user setting to UI-Large");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool stepstatus8 = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if ((stepstatus8 && viewportDimension))
                {
                    result.steps[ExecutedSteps].StepPass();
                   Logger.Instance.InfoLog("Images and View port not remain same after changing the user setting to UI - Large");
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("Images or Image size is changed when user selects UI-Large control tool from the User Settings.");
                }

                string PatinetNameFontSize = bluringviewer.PatinetName().GetCssValue("font-size");
                IList<IWebElement> PatientDemoDetailList = bluringviewer.PatientDemoDetailList();
                bool PatientDemoDetails = true;
                bool patinetnameFontsVerfiy = (PatinetNameFontSize == GlobalToolBarItemFontSizeLarge[0]);

                // Step 11
                //Merge LOGO is not scaling as Large and it remains same.
                //Get the Logo height and width after chnage to Large for verfication
                IList<String> LogoHeightWidthAfterLarge = new List<String>();
                LogoHeightWidthAfterLarge.Add(bluringviewer.MergeLogo().GetCssValue("height"));
                LogoHeightWidthAfterLarge.Add(bluringviewer.MergeLogo().GetCssValue("width"));
                if (LogoHeightWidthAfterLarge.ToArray().SequenceEqual(LogoHeightWidth.ToArray()))
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("Merge LOGO is not scaling as Large.");
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("Merge LOGO is scaling");
                }

                //Step12
                //Verify the Study Info font size which is available above the Thumbnails should get changed to Large in Size
                //Study Info font size which is available above the Thumbnails is changed to Large in Size
                List<IWebElement> AllStudyDateTimeAtStudyPanel = new List<IWebElement>();
                AllStudyDateTimeAtStudyPanel.AddRange(bluringviewer.AllStudyDateAtStudyPanel().ToList());
                bool boolAllStudyDateAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyDateTimeAtStudyPanel).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[0]));

                List<IWebElement> AllStudyTimeAtStudyPanel = new List<IWebElement>();
                AllStudyTimeAtStudyPanel.AddRange(bluringviewer.AllStudyTimeAtStudyPanel().ToList());
                bool boolAllStudyTimeAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanel).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[1]));

                List<IWebElement> AllStudyInfoAtStudyPanel = new List<IWebElement>();
                AllStudyInfoAtStudyPanel.AddRange(bluringviewer.AllStudyInfoAtStudyPanel().ToList());
                bool boolAllStudyInfoAtStudyPanel = bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanel).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[2]));

                if ( boolAllStudyDateAtStudypanel && boolAllStudyTimeAtStudypanel && boolAllStudyInfoAtStudyPanel)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("Study Info font size which is available above the Thumbnails is h changed to Large in Size");
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("Study Info font size which is available above the Thumbnails is not changed to Large in Size");
                }

                //Step 13 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool stepstatus13 = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if ((stepstatus8 && viewportDimension))
                {
                    result.steps[ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("the font size for the patient description/information available in all visible series viewports should get changed to Large in Size.");
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("the font size for the patient description/information available in all visible series viewports not get changed to Large in Size.");
                }

                // Step14 
                //Verify that the measurement units and image position (letters A/P/R etc) should not change to Small in size.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool stepstatus15 = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (stepstatus15)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("Measurement units and image position (letters A/P/R etc) are scaling large.");
                }


                //Step15
                //Verify the all menus(e.g.Exam List Filters, Filters drop down and Result List) font size in the "EXAM LIST" Panel should get changed to Large Size
                bool ExamListLable = (bluringviewer.ExamListLable().GetCssValue("font-size") == ExamListLableLarge);

                //Verfiy the Filter operator for the exam list 
                bluringviewer.OpenModalityFilter();
                string[] filterOperatorsFont = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text)).ToList());
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));
                
               // bluringviewer.CloseModalityFilter();
                bluringviewer.OpenSortDorpdown();
                string[] sortdropdown = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudySortOptionValues)).ToList());
                bool examOperators = filterOperatorsFont.All(c1 => c1.Equals(ExamListOperationContainerFontLarge)) && sortdropdown.All(c1 => c1.Equals(ExamListOperationContainerFontLarge));
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();

                //Verfiy the RecentStudyDate Font
                string[] RecentStudyAllDatesFonts = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDates().ToList());
                bool boolRecentStudyAllDatesFonts = RecentStudyAllDatesFonts.All(c1 => c1.Equals(listContainerFontLarge[0]));

                //Verfiy the RecentStudyTimesFont
                List<IWebElement> RecentStudyAllTimes = bluringviewer.RecentStudyAllTimes().ToList();
                string[] RecentStudyAllTimesFonts = bluringviewer.getFontSizeofElements(RecentStudyAllTimes);
                bool boolRecentStudyAllTimesFonts = RecentStudyAllTimesFonts.All(c1 => c1.Equals(listContainerFontLarge[1]));

                //Verfiy the RecentStudyModalityFont
                List<IWebElement> RecentStudyAllModality = bluringviewer.RecentStudyAllModality().ToList();
                string[] RecentStudyAllModalityFonts = bluringviewer.getFontSizeofElements(RecentStudyAllModality);
                bool boolRecentStudyAllModalityFonts = RecentStudyAllModalityFonts.All(c1 => c1.Equals(listContainerFontLarge[2]));

                //Verfiy the RecentStudyContrastFont
                List<IWebElement> RecentStudyAllDescription = bluringviewer.RecentStudyAllDescription().ToList();
                string[] RecentStudyAllDescriptionFonts = bluringviewer.getFontSizeofElements(RecentStudyAllDescription);
                bool boolRecentStudyAllDescriptionFonts = RecentStudyAllDescriptionFonts.All(c1 => c1.Equals(listContainerFontLarge[3]));

                //Add site
                //Verfiy the RecentStudySiteFont
                List<IWebElement> RecentStudyAllAccession = bluringviewer.RecentStudyAllAccession().ToList();
                string[] RecentStudyAllAccessionFonts = bluringviewer.getFontSizeofElements(RecentStudyAllAccession);
                bool boolRecentStudyAllAccessionFonts = RecentStudyAllAccessionFonts.All(c1 => c1.Equals(listContainerFontLarge[4]));

                if (ExamListLable && examOperators && boolRecentStudyAllDatesFonts && boolRecentStudyAllTimesFonts && boolRecentStudyAllModalityFonts && boolRecentStudyAllDescriptionFonts && boolRecentStudyAllAccessionFonts)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("All menus(e.g.Exam List Filters, Filters drop down and Result List) font size in the \"EXAM LIST\" Panel are not changed to Large Size");
                }

                //step 16
                // Verfiy the Thumbnail fonts in the thumbnail 
                List<IWebElement> thumbnail = new List<IWebElement>();
                thumbnail.AddRange(bluringviewer.ThumbnailPercentImagesViewed().ToList());
                thumbnail.AddRange(bluringviewer.ThumbnailImageFrameNumber().ToList());
                thumbnail.AddRange(bluringviewer.thumbnailCaption().ToList());
                string[] thumbnailFonts = bluringviewer.getFontSizeofElements(thumbnail);
                if( thumbnailFonts.All(c1 => c1.Equals(thumbnailFontLarge)))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 17
                //Select any Thumbnail Preview for the prior study and verify the Series and Image information text font size in thumbnails should be Large in Size
                //Series and Image information text font size in thumbnails is in Large in Size.
                if (bluringviewer.RecentStudythumbnailpreviewIcons().Count == 0)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.ErrorLog("Unable to select the Thumbnail Preview of the Recent study, as there is no recent study");
                }
                else
                {
                    if (Config.BrowserType != "ie")
                        bluringviewer.RecentStudythumbnailpreviewIcons()[0].Click();
                    else
                        ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", bluringviewer.RecentStudythumbnailpreviewIcons()[0]);

                    BluRingViewer.WaitforThumbnails();

                    Thread.Sleep(5000);

                    if (!bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed)
                    {
                        result.steps[++ExecutedSteps].StepFail();
                        Logger.Instance.ErrorLog("Thumbnail preview is not displayed");
                    }
                    else
                    {
                        List<IWebElement> ThumbPreviewCaptions = new List<IWebElement>();
                        ThumbPreviewCaptions.AddRange(bluringviewer.RecentStudyActiveThumbnailCaption().ToList());
                        ThumbPreviewCaptions.AddRange(bluringviewer.RecentStudyActiveThumbnailImageFrameNumber().ToList());
                        ThumbPreviewCaptions.AddRange(bluringviewer.RecentStudyActiveThumbnailModalityText().ToList());
                        string[] RecentStudythumbnailPreviewFontsLarge = bluringviewer.getFontSizeofElements(ThumbPreviewCaptions);
                        bool boolRecentStudythumbnailPreviewFontsLarge = RecentStudythumbnailPreviewFontsLarge.All(c1 => c1.Equals(RecentStudyThumbnailFontsLarge));
                        if (boolRecentStudythumbnailPreviewFontsLarge && bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed)
                        {
                            result.steps[++ExecutedSteps].StepPass();
                            Logger.Instance.InfoLog("Series and Image information text font size in thumbnails are in Large in Size.");
                        }
                        else
                        {
                            result.steps[++ExecutedSteps].StepFail();
                            Logger.Instance.ErrorLog("Series and Image information text font size in thumbnails are not in Large in Size.");
                        }
                    }
                }

                //Step 18
                //click right mouse button to open up the floating toolbox, perform any tools on selected series viewport and verify the UI font size should be in LARGE.
                bluringviewer.OpenViewerToolsPOPUp();
                bluringviewer.SelectViewerTool(BluRingTools.Pan, isOpenToolsPOPup: false);
                bluringviewer.ApplyTool_Pan();

                bool ExamListLableAfterApplyPan = (bluringviewer.ExamListLable().GetCssValue("font-size") == ExamListLableLarge);

                string PatinetNameFontSizeAfterApplyPan = bluringviewer.PatinetName().GetCssValue("font-size");
                bool patinetnameFontsVerfiyAfterApplyPan = (PatinetNameFontSizeAfterApplyPan == GlobalToolBarItemFontSizeLarge[0]);

                IList<IWebElement> PatientDemoDetailListAfterApplyPan = bluringviewer.PatientDemoDetailList();
                bool PatientDemoDetailsAfterApplyPan = true;
                foreach (IWebElement EachPatinetDemoDetails in PatientDemoDetailListAfterApplyPan)
                {
                    if (EachPatinetDemoDetails.GetCssValue("font-size") != GlobalToolBarItemFontSizeLarge[1])
                    {
                        PatientDemoDetailsAfterApplyPan = false;
                        break;
                    }

                }

                //Study Info font size which is available above the Thumbnails is h changed to Large in Size
                List<IWebElement> AllStudyDateAtStudyPanelAfterApplyPan = new List<IWebElement>();
                AllStudyDateAtStudyPanelAfterApplyPan.AddRange(bluringviewer.AllStudyDateAtStudyPanel().ToList());
                bool boolAllStudyDateAtStudypanelAfterApplyPan = bluringviewer.getFontSizeofElements(AllStudyDateAtStudyPanelAfterApplyPan).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[0]));

                List<IWebElement> AllStudyTimeAtStudyPanelAfterApplyPan = new List<IWebElement>();
                AllStudyTimeAtStudyPanelAfterApplyPan.AddRange(bluringviewer.AllStudyTimeAtStudyPanel().ToList());
                bool boolAllStudyTimeAtStudypanelAfterApplyPan = bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanelAfterApplyPan).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[1]));

                List<IWebElement> AllStudyInfoAtStudyPanelAfterApplyPan = new List<IWebElement>();
                AllStudyInfoAtStudyPanelAfterApplyPan.AddRange(bluringviewer.AllStudyInfoAtStudyPanel().ToList());
                bool boolAllStudyInfoAtStudyPaneAfterApplyPan = bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanelAfterApplyPan).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[2]));

                // Verfiy the Thumbnail fonts in the thumbnail 
                List<IWebElement> thumbnailPercentrViewedFontAfterApplyPan = bluringviewer.ThumbnailPercentImagesViewed().ToList(); ;
                List<IWebElement> thumbnailCaptionFontAfterApplyPan = bluringviewer.ThumbnailPercentImagesViewed().ToList();
                List<IWebElement> thumbnailimageFrameNumberFontAfterApplyPan = bluringviewer.ThumbnailImageFrameNumber().ToList();
                List<IWebElement> thumbnailAfterApplyPan = new List<IWebElement>();
                thumbnailAfterApplyPan.AddRange(thumbnailPercentrViewedFontAfterApplyPan);
                thumbnailAfterApplyPan.AddRange(thumbnailCaptionFontAfterApplyPan);
                thumbnailAfterApplyPan.AddRange(thumbnailimageFrameNumberFontAfterApplyPan);

                string[] thumbnailFontsAfterApplyPan = bluringviewer.getFontSizeofElements(thumbnailAfterApplyPan);

                bool StudyInfoVerified = (thumbnailFontsAfterApplyPan.All(c1 => c1.Equals(thumbnailFontLarge)) && boolAllStudyDateAtStudypanelAfterApplyPan && boolAllStudyTimeAtStudypanelAfterApplyPan && boolAllStudyInfoAtStudyPaneAfterApplyPan);

                // Exam list label;
                //Verfiy the Filter operator for the exam list 
                bluringviewer.OpenModalityFilter();
                string[] filterOperatorsFontAfterApplyPan = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text)).ToList());
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));

                bluringviewer.OpenSortDorpdown();
                string[] sortdropdownAfterApplyPan = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudySortOptionValues)).ToList());
                bool examOperatorsAfterApplyPan = filterOperatorsFontAfterApplyPan.All(c1 => c1.Equals(ExamListOperationContainerFontLarge)) && sortdropdownAfterApplyPan.All(c1 => c1.Equals(ExamListOperationContainerFontLarge));
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();

                //Verfiy the RecentStudyDate Font
                string[] RecentStudyAllDatesFontsAfterApplyPan = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDates().ToList());
                bool boolRecentStudyAllDatesFontsAfterApplyPan = RecentStudyAllDatesFontsAfterApplyPan.All(c1 => c1.Equals(listContainerFontLarge[0]));
                //Verfiy the RecentStudyTimesFont
                string[] RecentStudyAllTimesFontsAfterApplyPan = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllTimes().ToList());
                bool boolRecentStudyAllTimesFontsAfterApplyPan = RecentStudyAllTimesFontsAfterApplyPan.All(c1 => c1.Equals(listContainerFontLarge[1]));
                //Verfiy the RecentStudyModalityFont
                string[] RecentStudyAllModalityFontsAfterApplyPan = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllModality().ToList());
                bool boolRecentStudyAllModalityFontsAfterApplyPan = RecentStudyAllModalityFontsAfterApplyPan.All(c1 => c1.Equals(listContainerFontLarge[2]));
                //Verfiy the RecentStudyModalityFont
                string[] RecentStudyAllDescriptionFontsAfterApplyPan = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDescription().ToList());
                bool boolRecentStudyAllDescriptionFontsAfterApplyPan = RecentStudyAllDescriptionFontsAfterApplyPan.All(c1 => c1.Equals(listContainerFontLarge[3]));
                //Add site
                //Verfiy the RecentStudyModalityFont
                string[] RecentStudyAllAccessionFontsAfterApplyPan = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllAccession().ToList());
                bool boolRecentStudyAllAccessionFontsAfterApplyPan = RecentStudyAllAccessionFontsAfterApplyPan.All(c1 => c1.Equals(listContainerFontLarge[4]));

                bool examListpanel = (ExamListLableAfterApplyPan && examOperatorsAfterApplyPan && boolRecentStudyAllDatesFontsAfterApplyPan && boolRecentStudyAllTimesFontsAfterApplyPan && boolRecentStudyAllModalityFontsAfterApplyPan && boolRecentStudyAllDescriptionFontsAfterApplyPan && boolRecentStudyAllAccessionFontsAfterApplyPan);

                bool thumbnailPreviewAfterOpenNewStudy= false;
                if (!bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed)
                {
                    result.steps[ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("Thumbnail preview is not displayed");
                }
                else
                {

                    List<IWebElement> ThumbPreviewCaptionsAfterOpenNewStudy = new List<IWebElement>();
                    ThumbPreviewCaptionsAfterOpenNewStudy.AddRange(bluringviewer.RecentStudyActiveThumbnailCaption().ToList());
                    ThumbPreviewCaptionsAfterOpenNewStudy.AddRange(bluringviewer.RecentStudyActiveThumbnailImageFrameNumber().ToList());

                    string[] RecentStudythumbnailPreviewFontsLargeAfterOpenNewStudy = bluringviewer.getFontSizeofElements(ThumbPreviewCaptionsAfterOpenNewStudy);
                    bool boolRecentStudythumbnailPreviewFontsLargeAfterOpenNewStudy = RecentStudythumbnailPreviewFontsLargeAfterOpenNewStudy.All(c1 => c1.Equals(RecentStudyThumbnailFontsLarge));
                     thumbnailPreviewAfterOpenNewStudy = (boolRecentStudythumbnailPreviewFontsLargeAfterOpenNewStudy && bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed);

                }

                if ( PatientDemoDetailsAfterApplyPan && StudyInfoVerified && examListpanel && thumbnailPreviewAfterOpenNewStudy)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }


                //Step 19
                int StudyPanel = bluringviewer.AllstudyPanel().Count;
                float studyPanelLeft = float.Parse(bluringviewer.AllstudyPanel()[0].GetCssValue("left").Replace("px", ""));

                bluringviewer.RecentStudyAllDates()[0].Click();
                Thread.Sleep(3000);
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();

                Thread.Sleep(3000);
                bool NewstudyPanleOpened = (bluringviewer.AllstudyPanel().Count > StudyPanel);
                bool newStudyPosition = (float.Parse(bluringviewer.AllstudyPanel()[1].GetCssValue("left").Replace("px", "")) > studyPanelLeft);

                if (NewstudyPanleOpened && newStudyPosition)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("The study not opened in a new study panel to the right of the current exam or to the right of last prior study opened.");
                }


                //Step 20
                //Verify the UI font size should be in Large Size.
                //Global Tool bar menus,Study Info,Thumbnails Series and Image information text,Study Info,Exam List Panel menus,Thumbnail Preview font size(Series and Image information text) is in Large n size.
                string PatinetNameFontSizeAfterOpenNewStudy = bluringviewer.PatinetName().GetCssValue("font-size");
                bool patinetnameFontsVerfiyAfterOpenNewStudy = (PatinetNameFontSizeAfterOpenNewStudy == GlobalToolBarItemFontSizeLarge[0]);

                IList<IWebElement> PatientDemoDetailListAfterOpenNewStudy = bluringviewer.PatientDemoDetailList();
                bool PatientDemoDetailsAfterOpenNewStudy = true;
                foreach (IWebElement EachPatinetDemoDetails in PatientDemoDetailListAfterOpenNewStudy)
                {
                    if (EachPatinetDemoDetails.GetCssValue("font-size") != GlobalToolBarItemFontSizeLarge[1])
                    {
                        PatientDemoDetailsAfterOpenNewStudy = false;
                        break;
                    }

                }

                //Study Info font size which is available above the Thumbnails is h changed to Large in Size
                List<IWebElement> AllStudyDateAtStudyPanelAfterOpenNewStudy = new List<IWebElement>();
                AllStudyDateAtStudyPanelAfterOpenNewStudy.AddRange(bluringviewer.AllStudyDateAtStudyPanel().ToList());
                bool boolAllStudyDateAtStudypanelAfterOpenNewStudy = bluringviewer.getFontSizeofElements(AllStudyDateAtStudyPanelAfterOpenNewStudy).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[0]));

                List<IWebElement> AllStudyTimeAtStudyPanelAfterOpenNewStudy = new List<IWebElement>();
                AllStudyTimeAtStudyPanelAfterOpenNewStudy.AddRange(bluringviewer.AllStudyTimeAtStudyPanel().ToList());
                bool boolAllStudyTimeAtStudypanelAfterOpenNewStudy = bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanelAfterOpenNewStudy).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[1]));

                List<IWebElement> AllStudyInfoAtStudyPanelAfterOpenNewStudy = new List<IWebElement>();
                AllStudyInfoAtStudyPanelAfterOpenNewStudy.AddRange(bluringviewer.AllStudyInfoAtStudyPanel().ToList());
                bool boolAllStudyInfoAtStudyPaneAfterOpenNewStudy = bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanelAfterOpenNewStudy).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[2]));

                // Verfiy the Thumbnail fonts in the thumbnail 
                List<IWebElement> thumbnailPercentrViewedFontAfterOpenNewStudy = bluringviewer.ThumbnailPercentImagesViewed().ToList(); ;
                List<IWebElement> thumbnailCaptionFontAfterOpenNewStudy = bluringviewer.ThumbnailPercentImagesViewed().ToList();
                List<IWebElement> thumbnailimageFrameNumberFontAfterOpenNewStudy = bluringviewer.ThumbnailImageFrameNumber().ToList();
                List<IWebElement> thumbnailAfterOpenNewStudy = new List<IWebElement>();
                thumbnailAfterOpenNewStudy.AddRange(thumbnailPercentrViewedFontAfterOpenNewStudy);
                thumbnailAfterOpenNewStudy.AddRange(thumbnailCaptionFontAfterOpenNewStudy);
                thumbnailAfterOpenNewStudy.AddRange(thumbnailimageFrameNumberFontAfterOpenNewStudy);

                string[] thumbnailFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(thumbnailAfterOpenNewStudy);

                StudyInfoVerified = (thumbnailFontsAfterOpenNewStudy.All(c1 => c1.Equals(thumbnailFontLarge)) && boolAllStudyDateAtStudypanelAfterOpenNewStudy && boolAllStudyTimeAtStudypanelAfterOpenNewStudy && boolAllStudyInfoAtStudyPaneAfterOpenNewStudy);

                // Exam list label;
                bool ExamListLableAfterOpenNewStudy = (bluringviewer.ExamListLable().GetCssValue("font-size") == ExamListLableLarge);
                //Verfiy the Filter operator for the exam list 
                bluringviewer.OpenModalityFilter();
                string[] filterOperatorsFontAfterOpenNewStudy = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text)).ToList());
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));

                bluringviewer.OpenSortDorpdown();
                string[] sortdropdownAfterOpenNewStudy = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudySortOptionValues)).ToList());
                bool examOperatorsAfterOpenNewStudy = filterOperatorsFontAfterOpenNewStudy.All(c1 => c1.Equals(ExamListOperationContainerFontLarge)) && sortdropdownAfterOpenNewStudy.All(c1 => c1.Equals(ExamListOperationContainerFontLarge));
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();

                //Verfiy the RecentStudyDate Font
                string[] RecentStudyAllDatesFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDates().ToList());
                bool boolRecentStudyAllDatesFontsAfterOpenNewStudy = RecentStudyAllDatesFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontLarge[0]));
                //Verfiy the RecentStudyTimesFont
                string[] RecentStudyAllTimesFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllTimes().ToList());
                bool boolRecentStudyAllTimesFontsAfterOpenNewStudy = RecentStudyAllTimesFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontLarge[1]));
                //Verfiy the RecentStudyModalityFont
                string[] RecentStudyAllModalityFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllModality().ToList());
                bool boolRecentStudyAllModalityFontsAfterOpenNewStudy = RecentStudyAllModalityFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontLarge[2]));
                //Verfiy the RecentStudyModalityFont
                string[] RecentStudyAllDescriptionFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDescription().ToList());
                bool boolRecentStudyAllDescriptionFontsAfterOpenNewStudy = RecentStudyAllDescriptionFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontLarge[3]));
                //Add site
                //Verfiy the RecentStudyModalityFont
                string[] RecentStudyAllAccessionFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllAccession().ToList());
                bool boolRecentStudyAllAccessionFontsAfterOpenNewStudy = RecentStudyAllAccessionFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontLarge[4]));

                examListpanel = (PatientDemoDetailsAfterOpenNewStudy && ExamListLableAfterOpenNewStudy && examOperatorsAfterOpenNewStudy && boolRecentStudyAllDatesFontsAfterOpenNewStudy && boolRecentStudyAllTimesFontsAfterOpenNewStudy && boolRecentStudyAllModalityFontsAfterOpenNewStudy && boolRecentStudyAllDescriptionFontsAfterOpenNewStudy && boolRecentStudyAllAccessionFontsAfterOpenNewStudy);

                if (!bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed)
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("Thumbnail preview is not displayed");
                }
                else
                {

                    List<IWebElement> ThumbPreviewCaptionsAfterOpenNewStudy = new List<IWebElement>();
                    ThumbPreviewCaptionsAfterOpenNewStudy.AddRange(bluringviewer.RecentStudyActiveThumbnailCaption().ToList());
                    ThumbPreviewCaptionsAfterOpenNewStudy.AddRange(bluringviewer.RecentStudyActiveThumbnailImageFrameNumber().ToList());

                    string[] RecentStudythumbnailPreviewFontsLargeAfterOpenNewStudy = bluringviewer.getFontSizeofElements(ThumbPreviewCaptionsAfterOpenNewStudy);
                    bool boolRecentStudythumbnailPreviewFontsLargeAfterOpenNewStudy = RecentStudythumbnailPreviewFontsLargeAfterOpenNewStudy.All(c1 => c1.Equals(RecentStudyThumbnailFontsLarge));
                    thumbnailPreviewAfterOpenNewStudy = (boolRecentStudythumbnailPreviewFontsLargeAfterOpenNewStudy && bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed);

                    if (PatientDemoDetailsAfterOpenNewStudy && StudyInfoVerified && examListpanel && thumbnailPreviewAfterOpenNewStudy)
                    {
                        result.steps[++ExecutedSteps].StepPass();
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].StepFail();
                    }
                }

                // Step 21
                //Close the Exam List panel either by clicking on the EXAMS icon in the Global Toolbar or clicking on the 'X' on the top right corner of the panel and then verify the Global Toolbar menus (Patient Info,EXAMS, Show/Hide Tool ,EXIT) font size should be in Large Size.
                //Global Tool bar menus(Exam List Filters,Filters drop down and Result List) font size are in small.
                bluringviewer.CloseExamPanel().Click();
                PatinetNameFontSize = bluringviewer.PatinetName().GetCssValue("font-size");
                PatientDemoDetailList = bluringviewer.PatientDemoDetailList();
                //ExamIconLable = bluringviewer.ExamTextLable().GetCssValue("font-size");
                string PatinetNameFontSizeAfterExamPanelClose= bluringviewer.PatinetName().GetCssValue("font-size");
                bool patinetnameFontsVerfiyAfterExamPanelClose= (PatinetNameFontSizeAfterExamPanelClose== GlobalToolBarItemFontSizeLarge[0]);

                IList<IWebElement> PatientDemoDetailListAfterExamPanelClose= bluringviewer.PatientDemoDetailList();
                bool PatientDemoDetailsAfterExamPanelClose= true;
                foreach (IWebElement EachPatinetDemoDetails in PatientDemoDetailListAfterOpenNewStudy)
                {
                    if (EachPatinetDemoDetails.GetCssValue("font-size") != GlobalToolBarItemFontSizeLarge[1])
                    {
                        PatientDemoDetailsAfterExamPanelClose= false;
                        break;
                    }

                }

                //Study Info font size which is available above the Thumbnails is h changed to Large in Size
                List<IWebElement> AllStudyDateAtStudyPanelAfterExamPanelClose= new List<IWebElement>();
                AllStudyDateAtStudyPanelAfterOpenNewStudy.AddRange(bluringviewer.AllStudyDateAtStudyPanel().ToList());
                bool boolAllStudyDateAtStudypanelAfterExamPanelClose= bluringviewer.getFontSizeofElements(AllStudyDateAtStudyPanelAfterOpenNewStudy).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[0]));

                List<IWebElement> AllStudyTimeAtStudyPanelAfterExamPanelClose= new List<IWebElement>();
                AllStudyTimeAtStudyPanelAfterOpenNewStudy.AddRange(bluringviewer.AllStudyTimeAtStudyPanel().ToList());
                bool boolAllStudyTimeAtStudypanelAfterExamPanelClose= bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanelAfterOpenNewStudy).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[1]));

                List<IWebElement> AllStudyInfoAtStudyPanelAfterExamPanelClose= new List<IWebElement>();
                AllStudyInfoAtStudyPanelAfterOpenNewStudy.AddRange(bluringviewer.AllStudyInfoAtStudyPanel().ToList());
                bool boolAllStudyInfoAtStudyPaneAfterExamPanelClose= bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanelAfterOpenNewStudy).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[2]));

                // Verfiy the Thumbnail fonts in the thumbnail 
                List<IWebElement> thumbnailPercentrViewedFontAfterExamPanelClose= bluringviewer.ThumbnailPercentImagesViewed().ToList(); ;
                List<IWebElement> thumbnailCaptionFontAfterExamPanelClose= bluringviewer.ThumbnailPercentImagesViewed().ToList();
                List<IWebElement> thumbnailimageFrameNumberFontAfterExamPanelClose= bluringviewer.ThumbnailImageFrameNumber().ToList();
                List<IWebElement> thumbnailAfterExamPanelClose= new List<IWebElement>();
                thumbnailAfterOpenNewStudy.AddRange(thumbnailPercentrViewedFontAfterOpenNewStudy);
                thumbnailAfterOpenNewStudy.AddRange(thumbnailCaptionFontAfterOpenNewStudy);
                thumbnailAfterOpenNewStudy.AddRange(thumbnailimageFrameNumberFontAfterOpenNewStudy);

                string[] thumbnailFontsAfterExamPanelClose= bluringviewer.getFontSizeofElements(thumbnailAfterOpenNewStudy);

                StudyInfoVerified = (thumbnailFontsAfterOpenNewStudy.All(c1 => c1.Equals(thumbnailFontLarge)) && boolAllStudyDateAtStudypanelAfterExamPanelClose&& boolAllStudyTimeAtStudypanelAfterExamPanelClose&& boolAllStudyInfoAtStudyPaneAfterOpenNewStudy);

                if (StudyInfoVerified && PatientDemoDetailsAfterExamPanelClose && StudyInfoVerified )
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                
                //step 22
                bluringviewer.ExamIcon().Click();
                //bluringviewer.RecentStudyAllDates()[0].Click();
                Thread.Sleep(3000);
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();

                Thread.Sleep(3000);
                 PatinetNameFontSizeAfterOpenNewStudy = bluringviewer.PatinetName().GetCssValue("font-size");
                 patinetnameFontsVerfiyAfterOpenNewStudy = (PatinetNameFontSizeAfterOpenNewStudy == GlobalToolBarItemFontSizeLarge[0]);

                PatientDemoDetailListAfterOpenNewStudy = bluringviewer.PatientDemoDetailList();
                 PatientDemoDetailsAfterOpenNewStudy = true;
                foreach (IWebElement EachPatinetDemoDetails in PatientDemoDetailListAfterOpenNewStudy)
                {
                    if (EachPatinetDemoDetails.GetCssValue("font-size") != GlobalToolBarItemFontSizeLarge[1])
                    {
                        PatientDemoDetailsAfterOpenNewStudy = false;
                        break;
                    }

                }


                //Study Info font size which is available above the Thumbnails is h changed to Large in Size
                 AllStudyDateAtStudyPanelAfterOpenNewStudy = new List<IWebElement>();
                AllStudyDateAtStudyPanelAfterOpenNewStudy.AddRange(bluringviewer.AllStudyDateAtStudyPanel().ToList());
                 boolAllStudyDateAtStudypanelAfterOpenNewStudy = bluringviewer.getFontSizeofElements(AllStudyDateAtStudyPanelAfterOpenNewStudy).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[0]));

                AllStudyTimeAtStudyPanelAfterOpenNewStudy = new List<IWebElement>();
                AllStudyTimeAtStudyPanelAfterOpenNewStudy.AddRange(bluringviewer.AllStudyTimeAtStudyPanel().ToList());
                 boolAllStudyTimeAtStudypanelAfterOpenNewStudy = bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanelAfterOpenNewStudy).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[1]));

                AllStudyInfoAtStudyPanelAfterOpenNewStudy = new List<IWebElement>();
                AllStudyInfoAtStudyPanelAfterOpenNewStudy.AddRange(bluringviewer.AllStudyInfoAtStudyPanel().ToList());
                 boolAllStudyInfoAtStudyPaneAfterOpenNewStudy = bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanelAfterOpenNewStudy).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[2]));

                // Verfiy the Thumbnail fonts in the thumbnail 
                 thumbnailPercentrViewedFontAfterOpenNewStudy = bluringviewer.ThumbnailPercentImagesViewed().ToList(); ;
                thumbnailCaptionFontAfterOpenNewStudy = bluringviewer.ThumbnailPercentImagesViewed().ToList();
                thumbnailimageFrameNumberFontAfterOpenNewStudy = bluringviewer.ThumbnailImageFrameNumber().ToList();
                thumbnailAfterOpenNewStudy = new List<IWebElement>();
                thumbnailAfterOpenNewStudy.AddRange(thumbnailPercentrViewedFontAfterOpenNewStudy);
                thumbnailAfterOpenNewStudy.AddRange(thumbnailCaptionFontAfterOpenNewStudy);
                thumbnailAfterOpenNewStudy.AddRange(thumbnailimageFrameNumberFontAfterOpenNewStudy);

                thumbnailFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(thumbnailAfterOpenNewStudy);

                StudyInfoVerified = (thumbnailFontsAfterOpenNewStudy.All(c1 => c1.Equals(thumbnailFontLarge)) && boolAllStudyDateAtStudypanelAfterOpenNewStudy && boolAllStudyTimeAtStudypanelAfterOpenNewStudy && boolAllStudyInfoAtStudyPaneAfterOpenNewStudy);

                // Exam list label;
                 ExamListLableAfterOpenNewStudy = (bluringviewer.ExamListLable().GetCssValue("font-size") == ExamListLableLarge);
                //Verfiy the Filter operator for the exam list 
                bluringviewer.OpenModalityFilter();
                filterOperatorsFontAfterOpenNewStudy = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text)).ToList());
                 new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));

                bluringviewer.OpenSortDorpdown();
                 sortdropdownAfterOpenNewStudy = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudySortOptionValues)).ToList());
                 examOperatorsAfterOpenNewStudy = filterOperatorsFontAfterOpenNewStudy.All(c1 => c1.Equals(ExamListOperationContainerFontLarge)) && sortdropdownAfterOpenNewStudy.All(c1 => c1.Equals(ExamListOperationContainerFontLarge));
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();

                //Verfiy the RecentStudyDate Font
                RecentStudyAllDatesFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDates().ToList());
                 boolRecentStudyAllDatesFontsAfterOpenNewStudy = RecentStudyAllDatesFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontLarge[0]));
                //Verfiy the RecentStudyTimesFont
                RecentStudyAllTimesFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllTimes().ToList());
                 boolRecentStudyAllTimesFontsAfterOpenNewStudy = RecentStudyAllTimesFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontLarge[1]));
                //Verfiy the RecentStudyModalityFont
                RecentStudyAllModalityFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllModality().ToList());
                 boolRecentStudyAllModalityFontsAfterOpenNewStudy = RecentStudyAllModalityFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontLarge[2]));
                //Verfiy the RecentStudyModalityFont
                 RecentStudyAllDescriptionFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDescription().ToList());
                boolRecentStudyAllDescriptionFontsAfterOpenNewStudy = RecentStudyAllDescriptionFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontLarge[3]));
                //Add site
                //Verfiy the RecentStudyModalityFont
                 RecentStudyAllAccessionFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllAccession().ToList());
                 boolRecentStudyAllAccessionFontsAfterOpenNewStudy = RecentStudyAllAccessionFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontLarge[4]));

                examListpanel = (PatientDemoDetailsAfterOpenNewStudy && ExamListLableAfterOpenNewStudy && examOperatorsAfterOpenNewStudy && boolRecentStudyAllDatesFontsAfterOpenNewStudy && boolRecentStudyAllTimesFontsAfterOpenNewStudy && boolRecentStudyAllModalityFontsAfterOpenNewStudy && boolRecentStudyAllDescriptionFontsAfterOpenNewStudy && boolRecentStudyAllAccessionFontsAfterOpenNewStudy);

                if (!bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed)
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("Thumbnail preview is not displayed");
                }
                else
                {

                    List<IWebElement> ThumbPreviewCaptionsAfterOpenNewStudy = new List<IWebElement>();
                    ThumbPreviewCaptionsAfterOpenNewStudy.AddRange(bluringviewer.RecentStudyActiveThumbnailCaption().ToList());
                    ThumbPreviewCaptionsAfterOpenNewStudy.AddRange(bluringviewer.RecentStudyActiveThumbnailImageFrameNumber().ToList());

                    string[] RecentStudythumbnailPreviewFontsLargeAfterOpenNewStudy = bluringviewer.getFontSizeofElements(ThumbPreviewCaptionsAfterOpenNewStudy);
                    bool boolRecentStudythumbnailPreviewFontsLargeAfterOpenNewStudy = RecentStudythumbnailPreviewFontsLargeAfterOpenNewStudy.All(c1 => c1.Equals(RecentStudyThumbnailFontsLarge));
                    thumbnailPreviewAfterOpenNewStudy = (boolRecentStudythumbnailPreviewFontsLargeAfterOpenNewStudy && bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed);

                    if (PatientDemoDetailsAfterOpenNewStudy && StudyInfoVerified && examListpanel && thumbnailPreviewAfterOpenNewStudy)
                    {
                        result.steps[++ExecutedSteps].StepPass();
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].StepFail();
                    }
                }

                bluringviewer.CloseBluRingViewer();
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);

                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                return result;
                //------------End of script---

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
        /// 161554 - UI-Scaling - MEDIUM
        /// </summary>
        ///
        public TestCaseResult Test_161554(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;

                string Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                string Datasource = login.GetHostName(Config.EA91);

                // Step 1
                login.DriverGoTo(login.url);
                result.steps[++ExecutedSteps].StepPass();

                //Step 2
                login.LoginIConnect(adminusername, adminpassword);
                result.steps[++ExecutedSteps].StepPass();

                // Step 3
                Studies study = (Studies)login.Navigate("Studies");

                if (login.IsTabSelected("Studies"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                // Step 4
                //Search and select a study, and click onÂ BluRing ViewÂ button
                //Selected study should be opened in BluRing Viewer
                study.SearchStudy(AccessionNo: Accession, Datasource: Datasource);
                PageLoadWait.WaitForPageLoad(20);
                study.SelectStudy("Accession", Accession);
                BluRingViewer bluringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();
                

                //Step 12- PreCondition
                // Take the height and Width of All view port for verfication
                string[] AllViewPortHeight = bluringviewer.AllViewPorts().Select(avp => avp.GetCssValue("height")).ToArray();
                string[] AllViewPortWidth = bluringviewer.AllViewPorts().Select(avp => avp.GetCssValue("width")).ToArray();
                bluringviewer.OpenViewerToolsPOPUp();
                bluringviewer.OpenStackedTool(BluRingTools.Line_Measurement, false);
                bluringviewer.SelectViewerTool(BluRingTools.Line_Measurement, isOpenToolsPOPup: false);
                bluringviewer.ApplyTool_LineMeasurement();

                //Step 10 - PreCondition
                //Get the Logo height and width before chnage to Medium for verfication
                IList<String> LogoHeightWidth = new List<String>();
                LogoHeightWidth.Add(bluringviewer.MergeLogo().GetCssValue("height"));
                LogoHeightWidth.Add(bluringviewer.MergeLogo().GetCssValue("width"));

                //Step 5
                //Search for User Settings at the top right corner in the Viewer page
                //User Settings should be available at the top right corner with User Control tools as UI-LARGE , UI-MEDIUM and UI-SMALL
                bool step5 = ((bluringviewer.UserSettings("displayed", UsersettingSmall) && bluringviewer.UserSettings("displayed", UsersettingLarge) && bluringviewer.UserSettings("displayed", UsersettingMedium)));
                result.steps[++ExecutedSteps].StepStatus(step5);

                //Step 6 
                result.steps[++ExecutedSteps].StepStatus(bluringviewer.UserSettings("checked", UsersettingMedium));

                //Step 7
                if (!bluringviewer.SettingPanel().Displayed)
                    bluringviewer.ClickOnUSerSettings();
                IList<IWebElement> UserControlToolsList = BasePage.FindElementsByCss(BluRingViewer.div_userControlToolsList);
                bool step7 = UserControlToolsList.All(settings => settings.GetCssValue("font-size").Equals(UserSettingsListFontMedium));
                result.steps[++ExecutedSteps].StepStatus(step7);
            
                 string PatinetNameFontSize = bluringviewer.PatinetName().GetCssValue("font-size");
                var PatinetNameFont = bluringviewer.PatinetName().GetCssValue("font-size");
                IList<IWebElement> PatientDemoDetailList = bluringviewer.PatientDemoDetailList();
                bool patinetnameFontsVerfiy = (PatinetNameFontSize == GlobalToolBarItemFontSizeMedium[0]);
                bool PatinetDetailsFont = (bluringviewer.getFontSizeofElements(PatientDemoDetailList).All(c1 => c1.Equals(GlobalToolBarItemFontSizeMedium[1])));

                // Step 8
                //Verify the Merge LOGO should remain same
                //Merge LOGO is not scaling as Medium and it remains same.
                IList<String> LogoHeightWidthAfterMedium = new List<String>();
                LogoHeightWidthAfterMedium.Add(bluringviewer.MergeLogo().GetCssValue("height"));
                LogoHeightWidthAfterMedium.Add(bluringviewer.MergeLogo().GetCssValue("width"));

                bool step8 = (LogoHeightWidthAfterMedium.ToArray().SequenceEqual(LogoHeightWidth.ToArray()));
                result.steps[++ExecutedSteps].StepStatus(step8);

                //Step 9
                //Verify the Study Info font size which is available above the Thumbnails should get changed to Medium in Size
                //Study Info font size which is available above the Thumbnails is h changed to Medium in Size
                List<IWebElement> AllStudyDateTimeAtStudyPanel = new List<IWebElement>();
                AllStudyDateTimeAtStudyPanel.AddRange(bluringviewer.AllStudyDateAtStudyPanel().ToList());
                bool boolAllStudyDateAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyDateTimeAtStudyPanel).All(c1 => c1.Equals(studyInformationAtStudyPanelMedium[0]));

                List<IWebElement> AllStudyTimeAtStudyPanel = new List<IWebElement>();
                AllStudyTimeAtStudyPanel.AddRange(bluringviewer.AllStudyTimeAtStudyPanel().ToList());
                bool boolAllStudyTimeAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanel).All(c1 => c1.Equals(studyInformationAtStudyPanelMedium[1]));

                List<IWebElement> AllStudyInfoAtStudyPanel = new List<IWebElement>();
                AllStudyInfoAtStudyPanel.AddRange(bluringviewer.AllStudyInfoAtStudyPanel().ToList());
                bool boolAllStudyInfoAtStudyPanel = bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanel).All(c1 => c1.Equals(studyInformationAtStudyPanelMedium[2]));

                bool step9 = boolAllStudyDateAtStudypanel && boolAllStudyTimeAtStudypanel && boolAllStudyInfoAtStudyPanel;
                result.steps[++ExecutedSteps].StepStatus(step9);

                // Step 10
                //Verify the "Image Text" font size on the series viewports should not changed to Medium in size
                //"Image Text" font size on the series viewports are not changed to Medium in size
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool stepstatus10 = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                result.steps[ExecutedSteps].StepStatus(stepstatus10);

                //Step 11
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool stepstatus11 = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                result.steps[ExecutedSteps].StepStatus(stepstatus11);


                //Step12
                //Verify the all menus(Exam List Filters, Filters drop down and Result List) font size in the "EXAM LIST" Panel should get changed to Medium Size
                //All menus(Exam List Filters,Filters drop down and Result List ) font size in the "EXAM LIST" Panel are changed to Medium Size
                // Exam list label;
                bool ExamListLable = (bluringviewer.ExamListLable().GetCssValue("font-size") == ExamListLableMedium);

                //Verfiy the Filter operator for the exam list 
                bluringviewer.OpenModalityFilter();
                string[] filterOperatorsFont = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text)).ToList());
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));

                bluringviewer.OpenSortDorpdown();
                string[] sortdropdown = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudySortOptionValues)).ToList());
                bool examOperators = filterOperatorsFont.All(c1 => c1.Equals(ExamListOperationContainerFontMedium)) && sortdropdown.All(c1 => c1.Equals(ExamListOperationContainerFontMedium));
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();

                //Verfiy the RecentStudyDate Font
                string[] RecentStudyAllDatesFonts = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDates().ToList());
                bool boolRecentStudyAllDatesFonts = RecentStudyAllDatesFonts.All(c1 => c1.Equals(listContainerFontMedium[0]));

                //Verfiy the RecentStudyTimesFont
                List<IWebElement> RecentStudyAllTimes = bluringviewer.RecentStudyAllTimes().ToList();
                string[] RecentStudyAllTimesFonts = bluringviewer.getFontSizeofElements(RecentStudyAllTimes);
                bool boolRecentStudyAllTimesFonts = RecentStudyAllTimesFonts.All(c1 => c1.Equals(listContainerFontMedium[1]));

                //Verfiy the RecentStudyModalityFont
                List<IWebElement> RecentStudyAllModality = bluringviewer.RecentStudyAllModality().ToList();
                string[] RecentStudyAllModalityFonts = bluringviewer.getFontSizeofElements(RecentStudyAllModality);
                bool boolRecentStudyAllModalityFonts = RecentStudyAllModalityFonts.All(c1 => c1.Equals(listContainerFontMedium[2]));

                //Verfiy the RecentStudyContrastFont
                List<IWebElement> RecentStudyAllDescription = bluringviewer.RecentStudyAllDescription().ToList();
                string[] RecentStudyAllDescriptionFonts = bluringviewer.getFontSizeofElements(RecentStudyAllDescription);
                bool boolRecentStudyAllDescriptionFonts = RecentStudyAllDescriptionFonts.All(c1 => c1.Equals(listContainerFontMedium[3]));

                //Add site
                //Verfiy the RecentStudySiteFont
                List<IWebElement> RecentStudyAllAccession = bluringviewer.RecentStudyAllAccession().ToList();
                string[] RecentStudyAllAccessionFonts = bluringviewer.getFontSizeofElements(RecentStudyAllAccession);
                bool boolRecentStudyAllAccessionFonts = RecentStudyAllAccessionFonts.All(c1 => c1.Equals(listContainerFontMedium[4]));

                bool stepStatus12 = ExamListLable && examOperators && boolRecentStudyAllDatesFonts && boolRecentStudyAllTimesFonts && boolRecentStudyAllModalityFonts && boolRecentStudyAllDescriptionFonts && boolRecentStudyAllAccessionFonts; ;
                result.steps[++ExecutedSteps].StepStatus(stepStatus12);

                //Step 13
                // Verfiy the Thumbnail fonts in the thumbnail 
                List<IWebElement> thumbnail = new List<IWebElement>();
                thumbnail.AddRange(bluringviewer.ThumbnailPercentImagesViewed().ToList());
                thumbnail.AddRange(bluringviewer.ThumbnailImageFrameNumber().ToList());

                string[] thumbnailFonts = bluringviewer.getFontSizeofElements(thumbnail);
                bool StepStatus13 = (thumbnailFonts.All(c1 => c1.Equals(thumbnailFontMedium)));
                result.steps[++ExecutedSteps].StepStatus(StepStatus13);
               

                //Step 14
                //Select any Thumbnail Preview for the prior study and verify the Series and Image information text font size in thumbnails should be Medium in Size
                //Series and Image information text font size in thumbnails are in Medium in Size.
                if (bluringviewer.RecentStudythumbnailpreviewIcons().Count == 0)
                {
                    Logger.Instance.ErrorLog("Unable to select the Thumbnail Preview of the Recent study, as there is no recent study");
                    result.steps[++ExecutedSteps].StepFail();
                }
                else
                {
                    if(Config.BrowserType != "ie")
                        bluringviewer.RecentStudythumbnailpreviewIcons()[0].Click();
                    else
                        ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", bluringviewer.RecentStudythumbnailpreviewIcons()[0]);
                    
                    BluRingViewer.WaitforThumbnails();
                    
                    Thread.Sleep(10000);

                    if (!bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed)
                    {
                        Logger.Instance.ErrorLog("Thumbnail preview is not displayed");
                        result.steps[++ExecutedSteps].StepFail();
                    }
                    else
                    {
                        List<IWebElement> ThumbPreviewCaptions = new List<IWebElement>();
                        ThumbPreviewCaptions.AddRange(bluringviewer.RecentStudyActiveThumbnailCaption().ToList());
                        ThumbPreviewCaptions.AddRange(bluringviewer.RecentStudyActiveThumbnailImageFrameNumber().ToList());

                        string[] RecentStudythumbnailPreviewFontsMedium = bluringviewer.getFontSizeofElements(ThumbPreviewCaptions);
                        bool boolRecentStudythumbnailPreviewFontsMedium = RecentStudythumbnailPreviewFontsMedium.All(c1 => c1.Equals(RecentStudyThumbnailFontsMedium));
                        if (boolRecentStudythumbnailPreviewFontsMedium && bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed)
                        {
                            result.steps[++ExecutedSteps].StepPass();
                            Logger.Instance.InfoLog("Series and Image information text font size in thumbnails are in Medium in Size.");
                        }
                        else
                        {
                            result.steps[++ExecutedSteps].StepFail();
                            Logger.Instance.ErrorLog("Series and Image information text font size in thumbnails are not in Medium in Size.");
                        }
                    }

                }


                //Step15 
                //Select any series viewport,right mouse button click to open up the floating toolbox,perform any tools on selected series viewport and verify the UI font size should be in Medium.
                //UI font size is in Medium when user perform any tools on the selected series viewport.
                bluringviewer.OpenViewerToolsPOPUp();
                bluringviewer.SelectViewerTool(BluRingTools.Pan, isOpenToolsPOPup: false);
                bluringviewer.ApplyTool_Pan();

                // Exam list label;
                bool ExamListLableAfterApplyPan = (bluringviewer.ExamListLable().GetCssValue("font-size") == ExamListLableMedium);

                //Verfiy the Filter operator for the exam list 
                bluringviewer.OpenModalityFilter();
                string[] filterOperatorsFontAfterApplyPan = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text)).ToList());
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));

                bluringviewer.OpenSortDorpdown();
                string[] sortdropdownAfterApplyPan = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudySortOptionValues)).ToList());
                bool examOperatorsAfterApplyPan = filterOperatorsFontAfterApplyPan.All(c1 => c1.Equals(ExamListOperationContainerFontMedium)) && sortdropdownAfterApplyPan.All(c1 => c1.Equals(ExamListOperationContainerFontMedium));
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();

                //Verfiy the RecentStudyDate Font
                string[] RecentStudyAllDatesFontsAfterApplyPan = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDates().ToList());
                bool boolRecentStudyAllDatesFontsAfterApplyPan = RecentStudyAllDatesFontsAfterApplyPan.All(c1 => c1.Equals(listContainerFontMedium[0]));

                //Verfiy the RecentStudyTimesFont
                string[] RecentStudyAllTimesFontsAfterApplyPan = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllTimes().ToList());
                bool boolRecentStudyAllTimesFontsAfterApplyPan = RecentStudyAllTimesFontsAfterApplyPan.All(c1 => c1.Equals(listContainerFontMedium[1]));

                //Verfiy the RecentStudyModalityFont
                string[] RecentStudyAllModalityFontsAfterApplyPan = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllModality().ToList());
                bool boolRecentStudyAllModalityFontsAfterApplyPan = RecentStudyAllModalityFontsAfterApplyPan.All(c1 => c1.Equals(listContainerFontMedium[2]));

                //Verfiy the RecentStudyContrastFont
                string[] RecentStudyAllDescriptionFontsAfterApplyPan = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDescription().ToList());
                bool boolRecentStudyAllDescriptionFontsAfterApplyPan = RecentStudyAllDescriptionFontsAfterApplyPan.All(c1 => c1.Equals(listContainerFontMedium[3]));

                //Verfiy the RecentStudySiteFont
                string[] RecentStudyAllAccessionFontsAfterApplyPan = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllAccession().ToList());
                bool boolRecentStudyAllAccessionFontsAfterApplyPan = RecentStudyAllAccessionFontsAfterApplyPan.All(c1 => c1.Equals(listContainerFontMedium[4]));

                if (ExamListLableAfterApplyPan && examOperatorsAfterApplyPan && boolRecentStudyAllDatesFontsAfterApplyPan && boolRecentStudyAllTimesFontsAfterApplyPan && boolRecentStudyAllModalityFontsAfterApplyPan && boolRecentStudyAllDescriptionFontsAfterApplyPan && boolRecentStudyAllAccessionFontsAfterApplyPan)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog(" --ExamListLableAfterApplyPan=" + ExamListLableAfterApplyPan + " --examOperatorsAfterApplyPan=" + examOperatorsAfterApplyPan + " --boolRecentStudyAllDatesFontsAfterApplyPan=" + boolRecentStudyAllDatesFontsAfterApplyPan + " --boolRecentStudyAllTimesFontsAfterApplyPan=" + boolRecentStudyAllTimesFontsAfterApplyPan + " --boolRecentStudyAllModalityFontsAfterApplyPan=" + boolRecentStudyAllModalityFontsAfterApplyPan + " --boolRecentStudyAllDescriptionFontsAfterApplyPan=" + boolRecentStudyAllDescriptionFontsAfterApplyPan + " --boolRecentStudyAllAccessionFontsAfterApplyPan=" + boolRecentStudyAllAccessionFontsAfterApplyPan);
                }

                //Step16
                //Single click the study info area under Results list to open up the study in the viewer
                //The study opened in a new study panel to the right of the current exam or to the right of last prior study opened.
                //get the Total Active Study Panel 
                int StudyPanel = bluringviewer.AllstudyPanel().Count;
                float studyPanelLeft = float.Parse(bluringviewer.AllstudyPanel()[0].GetCssValue("left").Replace("px", ""));

                bluringviewer.RecentStudyAllDates()[0].Click();
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();

                Thread.Sleep(10000);
                bool NewstudyPanleOpened = (bluringviewer.AllstudyPanel().Count > StudyPanel);
                bool newStudyPosition = (float.Parse(bluringviewer.AllstudyPanel()[1].GetCssValue("left").Replace("px", "")) > studyPanelLeft);

                if (NewstudyPanleOpened && newStudyPosition)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("The study opened in a new study panel to the right of the current exam or to the right of last prior study opened.");
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("The study not opened in a new study panel to the right of the current exam or to the right of last prior study opened.");
                }


                //Step17
                //Verify the UI font size should be in Medium Size
                //Global Tool bar menus,Study Info,Thumbnails Series and Image information text,Study Info,Exam List Panel menus,Thumbnail Preview font size is changed to Medium.
                string PatinetNameFontSizeAfterOpenNewStudy = bluringviewer.PatinetName().GetCssValue("font-size");
                bool patinetnameFontsVerfiyAfterOpenNewStudy = (PatinetNameFontSizeAfterOpenNewStudy == GlobalToolBarItemFontSizeMedium[0]);

                IList<IWebElement> PatientDemoDetailListAfterOpenNewStudy = bluringviewer.PatientDemoDetailList();
                bool PatientDemoDetailsAfterOpenNewStudy = true;
                foreach (IWebElement EachPatinetDemoDetails in PatientDemoDetailListAfterOpenNewStudy)
                {
                    if (EachPatinetDemoDetails.GetCssValue("font-size") != GlobalToolBarItemFontSizeMedium[1])
                    {
                        PatientDemoDetailsAfterOpenNewStudy = false;
                        break;
                    }
                }
                bool globaltoolBars = (PatientDemoDetailsAfterOpenNewStudy && patinetnameFontsVerfiyAfterOpenNewStudy );

                //Study Info font size which is available above the Thumbnails is h changed to Medium in Size
                List<IWebElement> AllStudyDateAtStudyPanelAfterOpenNewStudy = new List<IWebElement>();
                AllStudyDateAtStudyPanelAfterOpenNewStudy.AddRange(bluringviewer.AllStudyDateAtStudyPanel().ToList());
                bool boolAllStudyDateAtStudypanelAfterOpenNewStudy = bluringviewer.getFontSizeofElements(AllStudyDateAtStudyPanelAfterOpenNewStudy).All(c1 => c1.Equals(studyInformationAtStudyPanelMedium[0]));

                List<IWebElement> AllStudyTimeAtStudyPanelAfterOpenNewStudy = new List<IWebElement>();
                AllStudyTimeAtStudyPanelAfterOpenNewStudy.AddRange(bluringviewer.AllStudyTimeAtStudyPanel().ToList());
                bool boolAllStudyTimeAtStudypanelAfterOpenNewStudy = bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanelAfterOpenNewStudy).All(c1 => c1.Equals(studyInformationAtStudyPanelMedium[1]));

                List<IWebElement> AllStudyInfoAtStudyPanelAfterOpenNewStudy = new List<IWebElement>();
                AllStudyInfoAtStudyPanelAfterOpenNewStudy.AddRange(bluringviewer.AllStudyInfoAtStudyPanel().ToList());
                bool boolAllStudyInfoAtStudyPaneAfterOpenNewStudy = bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanelAfterOpenNewStudy).All(c1 => c1.Equals(studyInformationAtStudyPanelMedium[2]));

                // Verfiy the Thumbnail fonts in the thumbnail 
                List<IWebElement> thumbnailPercentrViewedFontAfterOpenNewStudy = bluringviewer.ThumbnailPercentImagesViewed().ToList(); ;
                List<IWebElement> thumbnailCaptionFontAfterOpenNewStudy = bluringviewer.ThumbnailPercentImagesViewed().ToList();
                List<IWebElement> thumbnailimageFrameNumberFontAfterOpenNewStudy = bluringviewer.ThumbnailImageFrameNumber().ToList();
                List<IWebElement> thumbnailAfterOpenNewStudy = new List<IWebElement>();
                thumbnailAfterOpenNewStudy.AddRange(thumbnailPercentrViewedFontAfterOpenNewStudy);
                thumbnailAfterOpenNewStudy.AddRange(thumbnailCaptionFontAfterOpenNewStudy);
                thumbnailAfterOpenNewStudy.AddRange(thumbnailimageFrameNumberFontAfterOpenNewStudy);

                string[] thumbnailFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(thumbnailAfterOpenNewStudy);

                bool StudyInfoVerified = (thumbnailFontsAfterOpenNewStudy.All(c1 => c1.Equals(thumbnailFontMedium)) && boolAllStudyDateAtStudypanelAfterOpenNewStudy && boolAllStudyTimeAtStudypanelAfterOpenNewStudy && boolAllStudyInfoAtStudyPaneAfterOpenNewStudy);

                // Exam list label;
                bool ExamListLableAfterOpenNewStudy = (bluringviewer.ExamListLable().GetCssValue("font-size") == ExamListLableMedium);
                //Verfiy the Filter operator for the exam list 
                bluringviewer.OpenModalityFilter();
                string[] filterOperatorsFontAfterOpenNewStudy = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text)).ToList());
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));

                bluringviewer.OpenSortDorpdown();
                string[] sortdropdownAfterOpenNewStudy = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudySortOptionValues)).ToList());
                bool examOperatorsAfterOpenNewStudy = filterOperatorsFontAfterOpenNewStudy.All(c1 => c1.Equals(ExamListOperationContainerFontMedium)) && sortdropdownAfterOpenNewStudy.All(c1 => c1.Equals(ExamListOperationContainerFontMedium));
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();

                //Verfiy the RecentStudyDate Font
                string[] RecentStudyAllDatesFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDates().ToList());
                bool boolRecentStudyAllDatesFontsAfterOpenNewStudy = RecentStudyAllDatesFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontMedium[0]));
                //Verfiy the RecentStudyTimesFont
                string[] RecentStudyAllTimesFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllTimes().ToList());
                bool boolRecentStudyAllTimesFontsAfterOpenNewStudy = RecentStudyAllTimesFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontMedium[1]));
                //Verfiy the RecentStudyModalityFont
                string[] RecentStudyAllModalityFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllModality().ToList());
                bool boolRecentStudyAllModalityFontsAfterOpenNewStudy = RecentStudyAllModalityFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontMedium[2]));
                //Verfiy the RecentStudyModalityFont
                string[] RecentStudyAllDescriptionFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDescription().ToList());
                bool boolRecentStudyAllDescriptionFontsAfterOpenNewStudy = RecentStudyAllDescriptionFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontMedium[3]));
                //Add site
                //Verfiy the RecentStudyModalityFont
                string[] RecentStudyAllAccessionFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllAccession().ToList());
                bool boolRecentStudyAllAccessionFontsAfterOpenNewStudy = RecentStudyAllAccessionFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontMedium[4]));

                bool examListpanel = (ExamListLableAfterOpenNewStudy && examOperatorsAfterOpenNewStudy && boolRecentStudyAllDatesFontsAfterOpenNewStudy && boolRecentStudyAllTimesFontsAfterOpenNewStudy && boolRecentStudyAllModalityFontsAfterOpenNewStudy && boolRecentStudyAllDescriptionFontsAfterOpenNewStudy && boolRecentStudyAllAccessionFontsAfterOpenNewStudy);

                if (!bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed)
                {
                    Logger.Instance.ErrorLog("Thumbnail preview is not displayed");
                    result.steps[++ExecutedSteps].StepFail();
                }
                else
                {
                    List<IWebElement> ThumbPreviewCaptionsAfterOpenNewStudy = new List<IWebElement>();
                    ThumbPreviewCaptionsAfterOpenNewStudy.AddRange(bluringviewer.RecentStudyActiveThumbnailCaption().ToList());
                    ThumbPreviewCaptionsAfterOpenNewStudy.AddRange(bluringviewer.RecentStudyActiveThumbnailImageFrameNumber().ToList());

                    string[] RecentStudythumbnailPreviewFontsMediumAfterOpenNewStudy = bluringviewer.getFontSizeofElements(ThumbPreviewCaptionsAfterOpenNewStudy);
                    bool boolRecentStudythumbnailPreviewFontsMediumAfterOpenNewStudy = RecentStudythumbnailPreviewFontsMediumAfterOpenNewStudy.All(c1 => c1.Equals(RecentStudyThumbnailFontsMedium));
                    bool thumbnailPreviewAfterOpenNewStudy = (boolRecentStudythumbnailPreviewFontsMediumAfterOpenNewStudy && bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed);

                    if (globaltoolBars && StudyInfoVerified && examListpanel && thumbnailPreviewAfterOpenNewStudy)
                    {
                        result.steps[++ExecutedSteps].StepPass();
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].StepFail();
                    }
                }


                //Step 18
                //Close the Exam List panel either by clicking on the EXAMS icon in the Global Toolbar or clicking on the 'X' on the top right corner of the panel and then verify the Global Toolbar menus (Patient Info,EXAMS, Show/Hide Tool ,EXIT) font size should be in Medium Size.
                //Global Tool bar menus(Exam List Filters,Filters drop down and Result List) font size are in Medium.
                bluringviewer.CloseExamPanel().Click();

                PatinetNameFontSize = bluringviewer.PatinetName().GetCssValue("font-size");
                PatientDemoDetailList = bluringviewer.PatientDemoDetailList();
                //  bool patdemo = PatientDemoDetailList.All(pd => string.Equals(GlobalToolBarItemFontSizeMedium[1], pd.GetCssValue("font-size")));
                bool PatientDemoDetailsAfterExamPanelClose = true;
                foreach (IWebElement EachPatinetDemoDetails in PatientDemoDetailList)
                {
                    if (EachPatinetDemoDetails.GetCssValue("font-size") != GlobalToolBarItemFontSizeMedium[1])
                    {
                        PatientDemoDetailsAfterExamPanelClose = false;
                        break;
                    }

                }
                //ExamIconLable = bluringviewer.ExamTextLable().GetCssValue("font-size");
                // ShowHideIconlable = bluringviewer.ShowHideToolName().GetCssValue("font-size");

                patinetnameFontsVerfiy = (PatinetNameFontSize == GlobalToolBarItemFontSizeMedium[0]);
                //ExamIconLableVerfiy = (ExamIconLable == GlobalToolBarItemFontSizeMedium[3]);
                //ShowHideIconlableVerfiy = (ShowHideIconlable == GlobalToolBarItemFontSizeMedium[4]);

                if (patinetnameFontsVerfiy && PatientDemoDetailsAfterExamPanelClose )
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog("Global toolbar menus font size is changed to SMALL size when the user selects the UI-SMALL control tool.");
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.ErrorLog("Global toolbar menus font size is not changed to SMALL size when the user selects the UI-SMALL control tool.");
                }

                //Step 19
                bluringviewer.ExamIcon().Click();
                bluringviewer.RecentStudyAllDates()[1].Click();
                Thread.Sleep(3000);
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();


                PatinetNameFontSizeAfterOpenNewStudy = bluringviewer.PatinetName().GetCssValue("font-size");
                patinetnameFontsVerfiyAfterOpenNewStudy = (PatinetNameFontSizeAfterOpenNewStudy == GlobalToolBarItemFontSizeMedium[0]);

                PatientDemoDetailListAfterOpenNewStudy = bluringviewer.PatientDemoDetailList();
                PatientDemoDetailsAfterOpenNewStudy = true;
                foreach (IWebElement EachPatinetDemoDetails in PatientDemoDetailListAfterOpenNewStudy)
                {
                    if (EachPatinetDemoDetails.GetCssValue("font-size") != GlobalToolBarItemFontSizeMedium[1])
                    {
                        PatientDemoDetailsAfterOpenNewStudy = false;
                        break;
                    }
                }

                globaltoolBars = (PatientDemoDetailsAfterOpenNewStudy && patinetnameFontsVerfiyAfterOpenNewStudy );

                //Study Info font size which is available above the Thumbnails is h changed to Medium in Size
                AllStudyDateAtStudyPanelAfterOpenNewStudy = new List<IWebElement>();
                AllStudyDateAtStudyPanelAfterOpenNewStudy.AddRange(bluringviewer.AllStudyDateAtStudyPanel().ToList());
                boolAllStudyDateAtStudypanelAfterOpenNewStudy = bluringviewer.getFontSizeofElements(AllStudyDateAtStudyPanelAfterOpenNewStudy).All(c1 => c1.Equals(studyInformationAtStudyPanelMedium[0]));

                AllStudyTimeAtStudyPanelAfterOpenNewStudy = new List<IWebElement>();
                AllStudyTimeAtStudyPanelAfterOpenNewStudy.AddRange(bluringviewer.AllStudyTimeAtStudyPanel().ToList());
                boolAllStudyTimeAtStudypanelAfterOpenNewStudy = bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanelAfterOpenNewStudy).All(c1 => c1.Equals(studyInformationAtStudyPanelMedium[1]));

                AllStudyInfoAtStudyPanelAfterOpenNewStudy = new List<IWebElement>();
                AllStudyInfoAtStudyPanelAfterOpenNewStudy.AddRange(bluringviewer.AllStudyInfoAtStudyPanel().ToList());
                boolAllStudyInfoAtStudyPaneAfterOpenNewStudy = bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanelAfterOpenNewStudy).All(c1 => c1.Equals(studyInformationAtStudyPanelMedium[2]));

                // Verfiy the Thumbnail fonts in the thumbnail 
                thumbnailPercentrViewedFontAfterOpenNewStudy = bluringviewer.ThumbnailPercentImagesViewed().ToList(); ;
                thumbnailCaptionFontAfterOpenNewStudy = bluringviewer.ThumbnailPercentImagesViewed().ToList();
                thumbnailimageFrameNumberFontAfterOpenNewStudy = bluringviewer.ThumbnailImageFrameNumber().ToList();
                thumbnailAfterOpenNewStudy = new List<IWebElement>();
                thumbnailAfterOpenNewStudy.AddRange(thumbnailPercentrViewedFontAfterOpenNewStudy);
                thumbnailAfterOpenNewStudy.AddRange(thumbnailCaptionFontAfterOpenNewStudy);
                thumbnailAfterOpenNewStudy.AddRange(thumbnailimageFrameNumberFontAfterOpenNewStudy);

                thumbnailFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(thumbnailAfterOpenNewStudy);

                StudyInfoVerified = (thumbnailFontsAfterOpenNewStudy.All(c1 => c1.Equals(thumbnailFontMedium)) && boolAllStudyDateAtStudypanelAfterOpenNewStudy && boolAllStudyTimeAtStudypanelAfterOpenNewStudy && boolAllStudyInfoAtStudyPaneAfterOpenNewStudy);

                // Exam list label;
                ExamListLableAfterOpenNewStudy = (bluringviewer.ExamListLable().GetCssValue("font-size") == ExamListLableMedium);
                //Verfiy the Filter operator for the exam list 
                bluringviewer.OpenModalityFilter();
                filterOperatorsFontAfterOpenNewStudy = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text)).ToList());
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));

                bluringviewer.OpenSortDorpdown();
                sortdropdownAfterOpenNewStudy = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudySortOptionValues)).ToList());
                examOperatorsAfterOpenNewStudy = filterOperatorsFontAfterOpenNewStudy.All(c1 => c1.Equals(ExamListOperationContainerFontMedium)) && sortdropdownAfterOpenNewStudy.All(c1 => c1.Equals(ExamListOperationContainerFontMedium));
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();

                //Verfiy the RecentStudyDate Font
                RecentStudyAllDatesFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDates().ToList());
                boolRecentStudyAllDatesFontsAfterOpenNewStudy = RecentStudyAllDatesFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontMedium[0]));
                //Verfiy the RecentStudyTimesFont
                RecentStudyAllTimesFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllTimes().ToList());
                boolRecentStudyAllTimesFontsAfterOpenNewStudy = RecentStudyAllTimesFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontMedium[1]));
                //Verfiy the RecentStudyModalityFont
                RecentStudyAllModalityFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllModality().ToList());
                boolRecentStudyAllModalityFontsAfterOpenNewStudy = RecentStudyAllModalityFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontMedium[2]));
                //Verfiy the RecentStudyModalityFont
                RecentStudyAllDescriptionFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDescription().ToList());
                boolRecentStudyAllDescriptionFontsAfterOpenNewStudy = RecentStudyAllDescriptionFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontMedium[3]));
                //Add site
                //Verfiy the RecentStudyModalityFont
                 RecentStudyAllAccessionFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllAccession().ToList());
                boolRecentStudyAllAccessionFontsAfterOpenNewStudy = RecentStudyAllAccessionFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontMedium[4]));

                examListpanel = (ExamListLableAfterOpenNewStudy && examOperatorsAfterOpenNewStudy && boolRecentStudyAllDatesFontsAfterOpenNewStudy && boolRecentStudyAllTimesFontsAfterOpenNewStudy && boolRecentStudyAllModalityFontsAfterOpenNewStudy && boolRecentStudyAllDescriptionFontsAfterOpenNewStudy && boolRecentStudyAllAccessionFontsAfterOpenNewStudy);

                if (!bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed)
                {
                    Logger.Instance.ErrorLog("Thumbnail preview is not displayed");
                    result.steps[++ExecutedSteps].StepFail();
                }
                else
                {
                    List<IWebElement> ThumbPreviewCaptionsAfterOpenNewStudy = new List<IWebElement>();
                    ThumbPreviewCaptionsAfterOpenNewStudy.AddRange(bluringviewer.RecentStudyActiveThumbnailCaption().ToList());
                    ThumbPreviewCaptionsAfterOpenNewStudy.AddRange(bluringviewer.RecentStudyActiveThumbnailImageFrameNumber().ToList());

                    string[] RecentStudythumbnailPreviewFontsMediumAfterOpenNewStudy = bluringviewer.getFontSizeofElements(ThumbPreviewCaptionsAfterOpenNewStudy);
                    bool boolRecentStudythumbnailPreviewFontsMediumAfterOpenNewStudy = RecentStudythumbnailPreviewFontsMediumAfterOpenNewStudy.All(c1 => c1.Equals(RecentStudyThumbnailFontsMedium));
                    bool thumbnailPreviewAfterOpenNewStudy = (boolRecentStudythumbnailPreviewFontsMediumAfterOpenNewStudy && bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed);

                    if (globaltoolBars && StudyInfoVerified && examListpanel && thumbnailPreviewAfterOpenNewStudy)
                    {
                        result.steps[++ExecutedSteps].StepPass();
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].StepFail();
                    }
                }

                bluringviewer.CloseBluRingViewer();

                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);

                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                return result;
                //------------End of script---

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
        /// 136733 - This Test Case is Verify the UI font-size shall get change to SMALL when the user switching of font-size preset as UI-SMALL under User Settings in viewer
        /// </summary>
        ///
        public TestCaseResult Test_161555(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;

                string Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                string Datasource = login.GetHostName(Config.EA91);

                // Step 1
                //Launch the BluRing application with a client browser (http//<BR IP>/WebAccess/) and hit enter   
                login.DriverGoTo(login.url);
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 2
                //Login to WebAccess site with any privileged user.
                login.LoginIConnect(adminusername, adminpassword);
                result.steps[++ExecutedSteps].status = "Pass";

                // Step 3
                //Click on Studies tab.
                //List of Studies should be displayed.
                Studies study = (Studies)login.Navigate("Studies");

                if (login.IsTabSelected("Studies"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                // Step 4
                //Search and select a study, and click onÂ BluRing ViewÂ button
                study.SearchStudy(AccessionNo: Accession, Datasource: Datasource);
                PageLoadWait.WaitForPageLoad(20);
                study.SelectStudy("Accession", Accession);
                BluRingViewer bluringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForFrameLoad(30);

                //Step 8- PreCondition
                // Take the height and Width of All view port for verfication
                string[] AllViewPortHeight = bluringviewer.AllViewPorts().Select(avp => avp.GetCssValue("height")).ToArray();
                string[] AllViewPortWidth = bluringviewer.AllViewPorts().Select(avp => avp.GetCssValue("width")).ToArray();

                //Step 10 - PreCondition
                //Get the Logo height and width before chnage to small for verfication
                IList<String> LogoHeightWidth = new List<String>();
                LogoHeightWidth.Add(bluringviewer.MergeLogo().GetCssValue("height"));
                LogoHeightWidth.Add(bluringviewer.MergeLogo().GetCssValue("width"));

                //Step 5
                //Search for User Settings at the top right corner in the Viewer page
                //User Settings should be available at the top right corner with User Control tools as UI - LARGE , UI - MEDIUM and UI-SMALL
                if ((bluringviewer.UserSettings("displayed", UsersettingLarge) && bluringviewer.UserSettings("displayed", UsersettingMedium) && bluringviewer.UserSettings("displayed", UsersettingSmall)))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 6
                //Click on UI-SMALL user control tool under the User Settings and Verify the User settings options should be disappeared.
                //User settings options is disappeared when user selects the UI-SMALL user control tool under the User Settings.
                bool selected = bluringviewer.UserSettings("select", UsersettingSmall);

                if (selected && !(bluringviewer.SettingPanel().Displayed))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("User settings options is appeared after when user selects the UI-SMALL user control tool under the User Settings.");
                }

                //Step 7
                //Again,Click on User Settings icon and verify the UI-SMALL should get selected.
                //UI-SMALL user control tool is selected.
                if ((bluringviewer.UserSettings("checked", UsersettingSmall)))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }


                //Step 8 
                if (!bluringviewer.SettingPanel().Displayed)
                    bluringviewer.ClickOnUSerSettings();
                IList<IWebElement> UserControlToolsList = BasePage.FindElementsByCss(BluRingViewer.div_userControlToolsList);
                if (UserControlToolsList.All(settings => settings.GetCssValue("font-size").Equals(UserSettingsListFontSmall)))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 9
                if (bluringviewer.SettingPanel().Displayed)
                    bluringviewer.ClickOnUSerSettings();
                if (!(bluringviewer.SettingPanel().Displayed))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("User Seeting tools Not disappeared after clicking on the User Settings Icon");
                }

                // Step 10
                // Take the height and Width of All view port for verfication
                string[] AllViewPortHeightAfterSelectSmall = bluringviewer.AllViewPorts().Select(avp => avp.GetCssValue("height")).ToArray();
                string[] AllViewPortWidthAfterSelectSmall = bluringviewer.AllViewPorts().Select(avp => avp.GetCssValue("width")).ToArray();

                bool viewportDimension = (AllViewPortHeightAfterSelectSmall.SequenceEqual(AllViewPortHeight) && AllViewPortWidthAfterSelectSmall.SequenceEqual(AllViewPortWidth));
                if (!viewportDimension) Logger.Instance.ErrorLog("View port not remain same after changing the user setting to UI-Small");

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool stepstatus8 = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());

                //Step 10 Verification
                //Verify the Image size should not changed to SMALL when user selects UI - SMALL control tool from the User Settings.
                //Image size is not changed to SMALL when user selects UI-SMALL control tool from the User Settings.
                if ((stepstatus8 && viewportDimension))
                {
                    result.steps[ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("Images and View port not remain same after changing the user setting to UI - Small");
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("Images or Image size is changed when user selects UI-SMALL control tool from the User Settings.");
                }

                //// Step 11
                ////Verify the Global Toolbar menus (Patient Info,EXAMS, Show/Hide Tool ,EXIT) font size should get changed to small size in the viewer page.
                ////Global toolbar menus font size is changed to SMALL size when the user selects the UI-SMALL control tool.
                //string PatinetNameFontSize = bluringviewer.PatinetName().GetCssValue("font-size");
                //IList<IWebElement> PatientDemoDetailList = bluringviewer.PatientDemoDetailList();
                ////  bool patdemo = PatientDemoDetailList.All(pd => string.Equals(GlobalToolBarItemFontSizeSmall[1], pd.GetCssValue("font-size")));
                //bool PatientDemoDetails = true;
                //foreach (IWebElement EachPatinetDemoDetails in PatientDemoDetailList)
                //{
                //    if (EachPatinetDemoDetails.GetCssValue("font-size") != GlobalToolBarItemFontSizeSmall[1])
                //    {
                //        PatientDemoDetails = false;
                //        break;
                //    }
                //}

                //bool patinetnameFontsVerfiy = (PatinetNameFontSize == GlobalToolBarItemFontSizeSmall[0]);
                //if (patinetnameFontsVerfiy && PatientDemoDetails  )
                //{
                //    result.steps[++ExecutedSteps].StepPass();
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].StepFail();

                //}


                // Step 11
                //Verify the Merge LOGO should remain same
                //Merge LOGO is not scaling as small and it remains same.
                IList<String> LogoHeightWidthAfterSmall = new List<String>();
                LogoHeightWidthAfterSmall.Add(bluringviewer.MergeLogo().GetCssValue("height"));
                LogoHeightWidthAfterSmall.Add(bluringviewer.MergeLogo().GetCssValue("width"));
           
                if (LogoHeightWidthAfterSmall.ToArray().SequenceEqual(LogoHeightWidth.ToArray()))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step12
                //Verify the Study Info font size which is available above the Thumbnails should get changed to small in Size
                //Study Info font size which is available above the Thumbnails is h changed to small in Size.
                List<IWebElement> AllStudyDateTimeAtStudyPanel = new List<IWebElement>();
                AllStudyDateTimeAtStudyPanel.AddRange(bluringviewer.AllStudyDateAtStudyPanel().ToList());
                bool boolAllStudyDateAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyDateTimeAtStudyPanel).All(c1 => c1.Equals(studyInformationAtStudyPanelSmall[0]));

                List<IWebElement> AllStudyTimeAtStudyPanel = new List<IWebElement>();
                AllStudyTimeAtStudyPanel.AddRange(bluringviewer.AllStudyTimeAtStudyPanel().ToList());
                bool boolAllStudyTimeAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanel).All(c1 => c1.Equals(studyInformationAtStudyPanelSmall[1]));

                List<IWebElement> AllStudyInfoAtStudyPanel = new List<IWebElement>();
                AllStudyInfoAtStudyPanel.AddRange(bluringviewer.AllStudyInfoAtStudyPanel().ToList());
                bool boolAllStudyInfoAtStudyPanel = bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanel).All(c1 => c1.Equals(studyInformationAtStudyPanelSmall[2]));

                // Verfiy the Thumbnail fonts in the thumbnail 
                //List<IWebElement> thumbnailPercentrViewedFont = bluringviewer.ThumbnailPercentImagesViewed().ToList(); ;
                //List<IWebElement> thumbnailCaptionFont = bluringviewer.ThumbnailPercentImagesViewed().ToList();
                //List<IWebElement> thumbnailimageFrameNumberFont = bluringviewer.ThumbnailImageFrameNumber().ToList();
                List<IWebElement> thumbnail = new List<IWebElement>();
                thumbnail.AddRange(bluringviewer.ThumbnailPercentImagesViewed().ToList());
                thumbnail.AddRange(bluringviewer.ThumbnailPercentImagesViewed().ToList());
                thumbnail.AddRange(bluringviewer.ThumbnailImageFrameNumber().ToList());

                string[] thumbnailFonts = bluringviewer.getFontSizeofElements(thumbnail);

                if (thumbnailFonts.All(c1 => c1.Equals(thumbnailFontSmall)) && boolAllStudyDateAtStudypanel && boolAllStudyTimeAtStudypanel && boolAllStudyInfoAtStudyPanel)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("Study Info font size which is available above the Thumbnails is h changed to small in Size");
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("Study Info font size which is available above the Thumbnails is not changed to small in Size");
                }


                // Step13 , Step 14
                //Verify the "Image Text" font size on the series viewports should not be changed to small in size
                //"Image Text" font size on the series viewports are not changed to small in size
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool stepstatus13 = study.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());

                if (stepstatus13)
                {
                    result.steps[ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("\"Image Text\" font size on the series viewports are not changed to small in size");
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("\"Image Text\" font size on the series viewports are changed to small in size");
                }

                //Step15
                //Verify the all menus(e.g. Exam List Filters,Filters drop down and Result List ) font size in the "EXAM LIST" Panel should get changed to small Size.
                //All menus(e.g. Exam List Filters,Filters drop down and Result List ) font size in the "EXAM LIST" Panel should get changed to small Size.
                // Exam list label;
                bool ExamListLable = (bluringviewer.ExamListLable().GetCssValue("font-size") == ExamListLableSmall);

                //Verfiy the Filter operator for the exam list 
                bluringviewer.OpenModalityFilter();
                string[] filterOperatorsFont = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text)).ToList());
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));

                bluringviewer.OpenSortDorpdown();
                string[] sortdropdown = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudySortOptionValues)).ToList());
                bool examOperators = filterOperatorsFont.All(c1 => c1.Equals(ExamListOperationContainerFontSmall)) && sortdropdown.All(c1 => c1.Equals(ExamListOperationContainerFontSmall));
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();

                //Verfiy the RecentStudyDate Font
                string[] RecentStudyAllDatesFonts = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDates().ToList());
                bool boolRecentStudyAllDatesFonts = RecentStudyAllDatesFonts.All(c1 => c1.Equals(listContainerFontSmall[0]));

                //Verfiy the RecentStudyTimesFont
                List<IWebElement> RecentStudyAllTimes = bluringviewer.RecentStudyAllTimes().ToList();
                string[] RecentStudyAllTimesFonts = bluringviewer.getFontSizeofElements(RecentStudyAllTimes);
                bool boolRecentStudyAllTimesFonts = RecentStudyAllTimesFonts.All(c1 => c1.Equals(listContainerFontSmall[1]));

                //Verfiy the RecentStudyModalityFont
                List<IWebElement> RecentStudyAllModality = bluringviewer.RecentStudyAllModality().ToList();
                string[] RecentStudyAllModalityFonts = bluringviewer.getFontSizeofElements(RecentStudyAllModality);
                bool boolRecentStudyAllModalityFonts = RecentStudyAllModalityFonts.All(c1 => c1.Equals(listContainerFontSmall[2]));

                //Verfiy the RecentStudyContrastFont
                List<IWebElement> RecentStudyAllDescription = bluringviewer.RecentStudyAllDescription().ToList();
                string[] RecentStudyAllDescriptionFonts = bluringviewer.getFontSizeofElements(RecentStudyAllDescription);
                bool boolRecentStudyAllDescriptionFonts = RecentStudyAllDescriptionFonts.All(c1 => c1.Equals(listContainerFontSmall[3]));

                //Add site
                //Verfiy the RecentStudySiteFont
                List<IWebElement> RecentStudyAllAccession = bluringviewer.RecentStudyAllAccession().ToList();
                string[] RecentStudyAllAccessionFonts = bluringviewer.getFontSizeofElements(RecentStudyAllAccession);
                bool boolRecentStudyAllAccessionFonts = RecentStudyAllAccessionFonts.All(c1 => c1.Equals(listContainerFontSmall[4]));

                if (ExamListLable && examOperators && boolRecentStudyAllDatesFonts && boolRecentStudyAllTimesFonts && boolRecentStudyAllModalityFonts && boolRecentStudyAllDescriptionFonts && boolRecentStudyAllAccessionFonts)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 16
                // Verfiy the Thumbnail fonts in the thumbnail 
                List<IWebElement> thumbnails = new List<IWebElement>();
                thumbnails.AddRange(bluringviewer.ThumbnailPercentImagesViewed().ToList());
                thumbnails.AddRange(bluringviewer.ThumbnailImageFrameNumber().ToList());

                string[] thumbnailFontss = bluringviewer.getFontSizeofElements(thumbnail);
                if (thumbnailFontss.All(c1 => c1.Equals(thumbnailFontSmall)))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 17
                //Select any Thumbnail Preview for the prior study and verify the Series and Image information text font size in thumbnails should be Small in Size
                //Series and Image information text font size in thumbnails are in Small in Size.
                if (bluringviewer.RecentStudythumbnailpreviewIcons().Count == 0)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    Logger.Instance.ErrorLog("Unable to select the Thumbnail Preview of the Recent study, as there is no recent study");
                }
                else
                {
                    if (Config.BrowserType != "ie")
                        bluringviewer.RecentStudythumbnailpreviewIcons()[0].Click();
                    else
                        ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", bluringviewer.RecentStudythumbnailpreviewIcons()[0]);

                    BluRingViewer.WaitforThumbnails();

                    Thread.Sleep(10000);

                    if (!bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed)
                    {
                        result.steps[++ExecutedSteps].StepFail();
                    }
                    else
                    {
                        List<IWebElement> ThumbPreviewCaptions = new List<IWebElement>();
                        ThumbPreviewCaptions.AddRange(bluringviewer.RecentStudyActiveThumbnailCaption().ToList());
                        ThumbPreviewCaptions.AddRange(bluringviewer.RecentStudyActiveThumbnailImageFrameNumber().ToList());

                        string[] RecentStudythumbnailPreviewFontsSmall = bluringviewer.getFontSizeofElements(ThumbPreviewCaptions);
                        bool boolRecentStudythumbnailPreviewFontsSmall = RecentStudythumbnailPreviewFontsSmall.All(c1 => c1.Equals(RecentStudyThumbnailFontsSmall));
                        if (boolRecentStudythumbnailPreviewFontsSmall && bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed)
                        {
                            result.steps[++ExecutedSteps].StepPass();
                        }
                        else
                        {
                            result.steps[++ExecutedSteps].StepFail();
                        }
                    }
                }

                //Step 18
                //Select any series viewport, click on right mouse button to open up the floating toolbox, perform any tools on selected series viewport and verify the Exam List Filters, Filters drop down and Result List menus font size should be in small.
                //Verfiy- Exam List Filters,Filters drop down and Result List menus font size is in small when user perform any tools on the selected series viewport.
                bluringviewer.OpenViewerToolsPOPUp();
                bluringviewer.SelectViewerTool(BluRingTools.Pan , isOpenToolsPOPup: false);
                bluringviewer.ApplyTool_Pan();

                // Exam list label;
                bool ExamListLableAfterApplyPan = (bluringviewer.ExamListLable().GetCssValue("font-size") == ExamListLableSmall);

                //Verfiy the Filter operator for the exam list 
                bluringviewer.OpenModalityFilter();
                string[] filterOperatorsFontAfterApplyPan = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text)).ToList());
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));

                bluringviewer.OpenSortDorpdown();
                string[] sortdropdownAfterApplyPan = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudySortOptionValues)).ToList());
                bool examOperatorsAfterApplyPan = filterOperatorsFontAfterApplyPan.All(c1 => c1.Equals(ExamListOperationContainerFontSmall)) && sortdropdownAfterApplyPan.All(c1 => c1.Equals(ExamListOperationContainerFontSmall));
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();

                //Verfiy the RecentStudyDate Font
                string[] RecentStudyAllDatesFontsAfterApplyPan = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDates().ToList());
                bool boolRecentStudyAllDatesFontsAfterApplyPan = RecentStudyAllDatesFontsAfterApplyPan.All(c1 => c1.Equals(listContainerFontSmall[0]));

                //Verfiy the RecentStudyTimesFont
                string[] RecentStudyAllTimesFontsAfterApplyPan = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllTimes().ToList());
                bool boolRecentStudyAllTimesFontsAfterApplyPan = RecentStudyAllTimesFontsAfterApplyPan.All(c1 => c1.Equals(listContainerFontSmall[1]));

                //Verfiy the RecentStudyModalityFont
                string[] RecentStudyAllModalityFontsAfterApplyPan = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllModality().ToList());
                bool boolRecentStudyAllModalityFontsAfterApplyPan = RecentStudyAllModalityFontsAfterApplyPan.All(c1 => c1.Equals(listContainerFontSmall[2]));

                //Verfiy the RecentStudyContrastFont
                string[] RecentStudyAllDescriptionFontsAfterApplyPan = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDescription().ToList());
                bool boolRecentStudyAllDescriptionFontsAfterApplyPan = RecentStudyAllDescriptionFontsAfterApplyPan.All(c1 => c1.Equals(listContainerFontSmall[3]));

                //Verfiy the RecentStudySiteFont
                string[] RecentStudyAllAccessionFontsAfterApplyPan = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllAccession().ToList());
                bool boolRecentStudyAllAccessionFontsAfterApplyPan = RecentStudyAllAccessionFontsAfterApplyPan.All(c1 => c1.Equals(listContainerFontSmall[4]));

                if (ExamListLableAfterApplyPan && examOperatorsAfterApplyPan && boolRecentStudyAllDatesFontsAfterApplyPan && boolRecentStudyAllTimesFontsAfterApplyPan && boolRecentStudyAllModalityFontsAfterApplyPan && boolRecentStudyAllDescriptionFontsAfterApplyPan && boolRecentStudyAllAccessionFontsAfterApplyPan)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }


                //Step19
                //Single click the study info area under Results list to open up the study in the viewer
                //The study opened in a new study panel to the right of the current exam or to the right of last prior study opened.
                //get the Total Active Study Panel 
                int StudyPanel = bluringviewer.AllstudyPanel().Count;
                float studyPanelLeft = float.Parse(bluringviewer.AllstudyPanel()[0].GetCssValue("left").Replace("px", ""));

                bluringviewer.RecentStudyAllDates()[0].Click();
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();

                Thread.Sleep(10000);
                bool NewstudyPanleOpened = (bluringviewer.AllstudyPanel().Count > StudyPanel);
                bool newStudyPosition = (float.Parse(bluringviewer.AllstudyPanel()[1].GetCssValue("left").Replace("px", "")) > studyPanelLeft);

                if (NewstudyPanleOpened && newStudyPosition)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step20
                //Verify the Study Info,Thumbnails Series and Image information text,Study Info,Exam List Panel menus,Thumbnail Preview font size should be in Small Size.
                //Global Tool bar menus,Study Info,Thumbnails Series and Image information text,Study Info,Exam List Panel menus,Thumbnail Preview font size is changed to Small.
                string PatinetNameFontSizeAfterOpenNewStudy = bluringviewer.PatinetName().GetCssValue("font-size");
                bool patinetnameFontsVerfiyAfterOpenNewStudy = (PatinetNameFontSizeAfterOpenNewStudy == GlobalToolBarItemFontSizeSmall[0]);

                IList<IWebElement> PatientDemoDetailListAfterOpenNewStudy = bluringviewer.PatientDemoDetailList();
                //  bool patdemo = PatientDemoDetailList.All(pd => string.Equals(GlobalToolBarItemFontSizeSmall[1], pd.GetCssValue("font-size")));
                bool PatientDemoDetailsAfterOpenNewStudy = true;
                foreach (IWebElement EachPatinetDemoDetails in PatientDemoDetailListAfterOpenNewStudy)
                {
                    if (EachPatinetDemoDetails.GetCssValue("font-size") != GlobalToolBarItemFontSizeSmall[1])
                    {
                        PatientDemoDetailsAfterOpenNewStudy = false;
                        break;
                    }

                }
                //string ExamIconLableAfterOpenNewStudy = bluringviewer.ExamTextLable().GetCssValue("font-size");
                //string ShowHideIconlableAfterOpenNewStudy = bluringviewer.ShowHideToolName().GetCssValue("font-size");
                bool globaltoolBars = (PatientDemoDetailsAfterOpenNewStudy && patinetnameFontsVerfiyAfterOpenNewStudy );

                //Study Info font size which is available above the Thumbnails is h changed to small in Size
                List<IWebElement> AllStudyDateAtStudyPanelAfterOpenNewStudy = new List<IWebElement>();
                AllStudyDateAtStudyPanelAfterOpenNewStudy.AddRange(bluringviewer.AllStudyDateAtStudyPanel().ToList());
                bool boolAllStudyDateAtStudypanelAfterOpenNewStudy = bluringviewer.getFontSizeofElements(AllStudyDateAtStudyPanelAfterOpenNewStudy).All(c1 => c1.Equals(studyInformationAtStudyPanelSmall[0]));

                List<IWebElement> AllStudyTimeAtStudyPanelAfterOpenNewStudy = new List<IWebElement>();
                AllStudyTimeAtStudyPanelAfterOpenNewStudy.AddRange(bluringviewer.AllStudyTimeAtStudyPanel().ToList());
                bool boolAllStudyTimeAtStudypanelAfterOpenNewStudy = bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanelAfterOpenNewStudy).All(c1 => c1.Equals(studyInformationAtStudyPanelSmall[1]));

                List<IWebElement> AllStudyInfoAtStudyPanelAfterOpenNewStudy = new List<IWebElement>();
                AllStudyInfoAtStudyPanelAfterOpenNewStudy.AddRange(bluringviewer.AllStudyInfoAtStudyPanel().ToList());
                bool boolAllStudyInfoAtStudyPaneAfterOpenNewStudy = bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanelAfterOpenNewStudy).All(c1 => c1.Equals(studyInformationAtStudyPanelSmall[2]));

                // Verfiy the Thumbnail fonts in the thumbnail 
                List<IWebElement> thumbnailPercentrViewedFontAfterOpenNewStudy = bluringviewer.ThumbnailPercentImagesViewed().ToList(); ;
                List<IWebElement> thumbnailCaptionFontAfterOpenNewStudy = bluringviewer.ThumbnailPercentImagesViewed().ToList();
                List<IWebElement> thumbnailimageFrameNumberFontAfterOpenNewStudy = bluringviewer.ThumbnailImageFrameNumber().ToList();
                List<IWebElement> thumbnailAfterOpenNewStudy = new List<IWebElement>();
                thumbnailAfterOpenNewStudy.AddRange(thumbnailPercentrViewedFontAfterOpenNewStudy);
                thumbnailAfterOpenNewStudy.AddRange(thumbnailCaptionFontAfterOpenNewStudy);
                thumbnailAfterOpenNewStudy.AddRange(thumbnailimageFrameNumberFontAfterOpenNewStudy);

                string[] thumbnailFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(thumbnailAfterOpenNewStudy);

                bool StudyInfoVerified = (thumbnailFontsAfterOpenNewStudy.All(c1 => c1.Equals(thumbnailFontSmall)) && boolAllStudyDateAtStudypanelAfterOpenNewStudy && boolAllStudyTimeAtStudypanelAfterOpenNewStudy && boolAllStudyInfoAtStudyPaneAfterOpenNewStudy);

                // Exam list label;
                bool ExamListLableAfterOpenNewStudy = (bluringviewer.ExamListLable().GetCssValue("font-size") == ExamListLableSmall);
                //Verfiy the Filter operator for the exam list 
                bluringviewer.OpenModalityFilter();
                string[] filterOperatorsFontAfterOpenNewStudy = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text)).ToList());
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));

                bluringviewer.OpenSortDorpdown();
                string[] sortdropdownAfterOpenNewStudy = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudySortOptionValues)).ToList());
                bool examOperatorsAfterOpenNewStudy = filterOperatorsFontAfterOpenNewStudy.All(c1 => c1.Equals(ExamListOperationContainerFontSmall)) && sortdropdownAfterOpenNewStudy.All(c1 => c1.Equals(ExamListOperationContainerFontSmall));
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();

                //Verfiy the RecentStudyDate Font
                string[] RecentStudyAllDatesFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDates().ToList());
                bool boolRecentStudyAllDatesFontsAfterOpenNewStudy = RecentStudyAllDatesFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontSmall[0]));
                //Verfiy the RecentStudyTimesFont
                string[] RecentStudyAllTimesFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllTimes().ToList());
                bool boolRecentStudyAllTimesFontsAfterOpenNewStudy = RecentStudyAllTimesFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontSmall[1]));
                //Verfiy the RecentStudyModalityFont
                string[] RecentStudyAllModalityFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllModality().ToList());
                bool boolRecentStudyAllModalityFontsAfterOpenNewStudy = RecentStudyAllModalityFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontSmall[2]));
                //Verfiy the RecentStudyModalityFont
                string[] RecentStudyAllDescriptionFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDescription().ToList());
                bool boolRecentStudyAllDescriptionFontsAfterOpenNewStudy = RecentStudyAllDescriptionFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontSmall[3]));
                //Add site
                //Verfiy the RecentStudyModalityFont
                string[] RecentStudyAllAccessionFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllAccession().ToList());
                bool boolRecentStudyAllAccessionFontsAfterOpenNewStudy = RecentStudyAllAccessionFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontSmall[4]));

                bool examListpanel = (ExamListLableAfterOpenNewStudy && examOperatorsAfterOpenNewStudy && boolRecentStudyAllDatesFontsAfterOpenNewStudy && boolRecentStudyAllTimesFontsAfterOpenNewStudy && boolRecentStudyAllModalityFontsAfterOpenNewStudy && boolRecentStudyAllDescriptionFontsAfterOpenNewStudy && boolRecentStudyAllAccessionFontsAfterOpenNewStudy);

                if (!bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed)
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                else
                {
                    List<IWebElement> ThumbPreviewCaptionsAfterOpenNewStudy = new List<IWebElement>();
                    ThumbPreviewCaptionsAfterOpenNewStudy.AddRange(bluringviewer.RecentStudyActiveThumbnailCaption().ToList());
                    ThumbPreviewCaptionsAfterOpenNewStudy.AddRange(bluringviewer.RecentStudyActiveThumbnailImageFrameNumber().ToList());

                    string[] RecentStudythumbnailPreviewFontsSmallAfterOpenNewStudy = bluringviewer.getFontSizeofElements(ThumbPreviewCaptionsAfterOpenNewStudy);
                    bool boolRecentStudythumbnailPreviewFontsSmallAfterOpenNewStudy = RecentStudythumbnailPreviewFontsSmallAfterOpenNewStudy.All(c1 => c1.Equals(RecentStudyThumbnailFontsSmall));
                    bool thumbnailPreviewAfterOpenNewStudy = (boolRecentStudythumbnailPreviewFontsSmallAfterOpenNewStudy && bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed);

                    if (globaltoolBars && StudyInfoVerified && examListpanel && thumbnailPreviewAfterOpenNewStudy)
                    {
                        result.steps[++ExecutedSteps].StepPass();
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].StepFail();
                    }
                }


                // Step 21
                //Close the Exam List panel either by clicking on the EXAMS icon in the Global Toolbar or clicking on the 'X' on the top right corner of the panel and then verify the Global Toolbar menus (Patient Info,EXAMS, Show/Hide Tool ,EXIT) font size should be in small Size.
                //Global Tool bar menus(Patient Info,EXAMS, Show/Hide Tool ,EXIT) font size are in small.
                bluringviewer.CloseExamPanel().Click();

                string PatinetNameFontSize = bluringviewer.PatinetName().GetCssValue("font-size");
                IList<IWebElement> PatientDemoDetailList = bluringviewer.PatientDemoDetailList();
                //  bool patdemo = PatientDemoDetailList.All(pd => string.Equals(GlobalToolBarItemFontSizeSmall[1], pd.GetCssValue("font-size")));
                bool PatientDemoDetailsAfterExamPanelClose = true;
                foreach (IWebElement EachPatinetDemoDetails in PatientDemoDetailList)
                {
                    if (EachPatinetDemoDetails.GetCssValue("font-size") != GlobalToolBarItemFontSizeSmall[1])
                    {
                        PatientDemoDetailsAfterExamPanelClose = false;
                        break;
                    }

                }
                //ExamIconLable = bluringviewer.ExamTextLable().GetCssValue("font-size");
                // ShowHideIconlable = bluringviewer.ShowHideToolName().GetCssValue("font-size");
                bool patinetnameFontsVerfiy = (PatinetNameFontSize == GlobalToolBarItemFontSizeSmall[0]);
                if (patinetnameFontsVerfiy && PatientDemoDetailsAfterExamPanelClose )
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 22
                bluringviewer.ExamIcon().Click();
                bluringviewer.RecentStudyAllDates()[0].Click();
                Thread.Sleep(3000);
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();

                Thread.Sleep(3000);
                //Verify the UI font size should be in Small Size.
                //Global Tool bar menus,Study Info,Thumbnails Series and Image information text,Study Info,Exam List Panel menus,Thumbnail Preview font size(Series and Image information text) is in Small n size.
                PatinetNameFontSizeAfterOpenNewStudy = bluringviewer.PatinetName().GetCssValue("font-size");
                patinetnameFontsVerfiyAfterOpenNewStudy = (PatinetNameFontSizeAfterOpenNewStudy == GlobalToolBarItemFontSizeSmall[0]);

                PatientDemoDetailListAfterOpenNewStudy = bluringviewer.PatientDemoDetailList();
                PatientDemoDetailsAfterOpenNewStudy = true;
                foreach (IWebElement EachPatinetDemoDetails in PatientDemoDetailListAfterOpenNewStudy)
                {
                    if (EachPatinetDemoDetails.GetCssValue("font-size") != GlobalToolBarItemFontSizeSmall[1])
                    {
                        PatientDemoDetailsAfterOpenNewStudy = false;
                        break;
                    }

                }
                globaltoolBars = (PatientDemoDetailsAfterOpenNewStudy && patinetnameFontsVerfiyAfterOpenNewStudy );

                //Study Info font size which is available above the Thumbnails is h changed to Small in Size
                AllStudyDateAtStudyPanelAfterOpenNewStudy = new List<IWebElement>();
                AllStudyDateAtStudyPanelAfterOpenNewStudy.AddRange(bluringviewer.AllStudyDateAtStudyPanel().ToList());
                boolAllStudyDateAtStudypanelAfterOpenNewStudy = bluringviewer.getFontSizeofElements(AllStudyDateAtStudyPanelAfterOpenNewStudy).All(c1 => c1.Equals(studyInformationAtStudyPanelSmall[0]));

                AllStudyTimeAtStudyPanelAfterOpenNewStudy = new List<IWebElement>();
                AllStudyTimeAtStudyPanelAfterOpenNewStudy.AddRange(bluringviewer.AllStudyTimeAtStudyPanel().ToList());
                boolAllStudyTimeAtStudypanelAfterOpenNewStudy = bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanelAfterOpenNewStudy).All(c1 => c1.Equals(studyInformationAtStudyPanelSmall[1]));

                AllStudyInfoAtStudyPanelAfterOpenNewStudy = new List<IWebElement>();
                AllStudyInfoAtStudyPanelAfterOpenNewStudy.AddRange(bluringviewer.AllStudyInfoAtStudyPanel().ToList());
                boolAllStudyInfoAtStudyPaneAfterOpenNewStudy = bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanelAfterOpenNewStudy).All(c1 => c1.Equals(studyInformationAtStudyPanelSmall[2]));

                // Verfiy the Thumbnail fonts in the thumbnail 
                thumbnailPercentrViewedFontAfterOpenNewStudy = bluringviewer.ThumbnailPercentImagesViewed().ToList(); ;
                thumbnailCaptionFontAfterOpenNewStudy = bluringviewer.ThumbnailPercentImagesViewed().ToList();
                thumbnailimageFrameNumberFontAfterOpenNewStudy = bluringviewer.ThumbnailImageFrameNumber().ToList();
                thumbnailAfterOpenNewStudy = new List<IWebElement>();
                thumbnailAfterOpenNewStudy.AddRange(thumbnailPercentrViewedFontAfterOpenNewStudy);
                thumbnailAfterOpenNewStudy.AddRange(thumbnailCaptionFontAfterOpenNewStudy);
                thumbnailAfterOpenNewStudy.AddRange(thumbnailimageFrameNumberFontAfterOpenNewStudy);

                thumbnailFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(thumbnailAfterOpenNewStudy);

                StudyInfoVerified = (thumbnailFontsAfterOpenNewStudy.All(c1 => c1.Equals(thumbnailFontSmall)) && boolAllStudyDateAtStudypanelAfterOpenNewStudy && boolAllStudyTimeAtStudypanelAfterOpenNewStudy && boolAllStudyInfoAtStudyPaneAfterOpenNewStudy);

                // Exam list label;
                ExamListLableAfterOpenNewStudy = (bluringviewer.ExamListLable().GetCssValue("font-size") == ExamListLableSmall);
                //Verfiy the Filter operator for the exam list 
                bluringviewer.OpenModalityFilter();
                filterOperatorsFontAfterOpenNewStudy = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text)).ToList());
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));

                bluringviewer.OpenSortDorpdown();
                sortdropdownAfterOpenNewStudy = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudySortOptionValues)).ToList());
                examOperatorsAfterOpenNewStudy = filterOperatorsFontAfterOpenNewStudy.All(c1 => c1.Equals(ExamListOperationContainerFontSmall)) && sortdropdownAfterOpenNewStudy.All(c1 => c1.Equals(ExamListOperationContainerFontSmall));
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();

                //Verfiy the RecentStudyDate Font
                RecentStudyAllDatesFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDates().ToList());
                boolRecentStudyAllDatesFontsAfterOpenNewStudy = RecentStudyAllDatesFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontSmall[0]));
                //Verfiy the RecentStudyTimesFont
                RecentStudyAllTimesFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllTimes().ToList());
                boolRecentStudyAllTimesFontsAfterOpenNewStudy = RecentStudyAllTimesFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontSmall[1]));
                //Verfiy the RecentStudyModalityFont
                RecentStudyAllModalityFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllModality().ToList());
                boolRecentStudyAllModalityFontsAfterOpenNewStudy = RecentStudyAllModalityFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontSmall[2]));
                //Verfiy the RecentStudyModalityFont
                RecentStudyAllDescriptionFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDescription().ToList());
                boolRecentStudyAllDescriptionFontsAfterOpenNewStudy = RecentStudyAllDescriptionFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontSmall[3]));
                //Add site
                //Verfiy the RecentStudyModalityFont
                RecentStudyAllAccessionFontsAfterOpenNewStudy = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllAccession().ToList());
                boolRecentStudyAllAccessionFontsAfterOpenNewStudy = RecentStudyAllAccessionFontsAfterOpenNewStudy.All(c1 => c1.Equals(listContainerFontSmall[4]));

                examListpanel = (ExamListLableAfterOpenNewStudy && examOperatorsAfterOpenNewStudy && boolRecentStudyAllDatesFontsAfterOpenNewStudy && boolRecentStudyAllTimesFontsAfterOpenNewStudy && boolRecentStudyAllModalityFontsAfterOpenNewStudy && boolRecentStudyAllDescriptionFontsAfterOpenNewStudy && boolRecentStudyAllAccessionFontsAfterOpenNewStudy);

                if (!bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed)
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.ErrorLog("Thumbnail preview is not displayed");
                }
                else
                {

                    List<IWebElement> ThumbPreviewCaptionsAfterOpenNewStudy = new List<IWebElement>();
                    ThumbPreviewCaptionsAfterOpenNewStudy.AddRange(bluringviewer.RecentStudyActiveThumbnailCaption().ToList());
                    ThumbPreviewCaptionsAfterOpenNewStudy.AddRange(bluringviewer.RecentStudyActiveThumbnailImageFrameNumber().ToList());

                    string[] RecentStudythumbnailPreviewFontsSmallAfterOpenNewStudy = bluringviewer.getFontSizeofElements(ThumbPreviewCaptionsAfterOpenNewStudy);
                    bool boolRecentStudythumbnailPreviewFontsSmallAfterOpenNewStudy = RecentStudythumbnailPreviewFontsSmallAfterOpenNewStudy.All(c1 => c1.Equals(RecentStudyThumbnailFontsSmall));
                    bool thumbnailPreviewAfterOpenNewStudy = (boolRecentStudythumbnailPreviewFontsSmallAfterOpenNewStudy && bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed);

                    if (globaltoolBars && StudyInfoVerified && examListpanel && thumbnailPreviewAfterOpenNewStudy)
                    {
                        result.steps[++ExecutedSteps].StepPass();
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].StepFail();
                    }
                }

                bluringviewer.CloseBluRingViewer();
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);

                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                return result;
                //------------End of script---

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

        public TestCaseResult Test_161552(string testid, string teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            int ExecutedSteps = -1;
            
            //Set Up validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;

                string Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                string Datasource = login.GetHostName(Config.EA91);

                // Step 1
                //Launch the BluRing application with a client browser (http//<BR IP>/WebAccess/) and hit enter   
                //The BluRing application login page should be displayed
                login.DriverGoTo(login.url);
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 2
                //Login to WebAccess site with any privileged user.
                //Should be able to login to the BluRing application
                login.LoginIConnect(adminusername, adminpassword);
                result.steps[++ExecutedSteps].status = "Pass";

                // Step 3
                //Click on Studies tab.
                //List of Studies should be displayed.
                Studies study = (Studies)login.Navigate("Studies");

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

                // Step 4
                //Search and select a study, and click onÂ BluRing ViewÂ button
                //Selected study should be opened in BluRing Viewer.
                //Accession: DSQ00000135
                study.SearchStudy(AccessionNo: Accession, Datasource: Datasource);
                PageLoadWait.WaitForLoadingMessage(20);
                PageLoadWait.WaitForSearchLoad();
                study.SelectStudy("Accession", Accession);
                BluRingViewer bluringviewer = BluRingViewer.LaunchBluRingViewer();
                // COmplete laoding and gold image comaprision and add datasource in search study 
                result.steps[++ExecutedSteps].status = "Pass";
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForFrameLoad(30);

                BasePage.Driver.Manage().Window.Maximize();

                //Set the User Setting to UI-Large
                bool selected = bluringviewer.UserSettings("select", UsersettingLarge);
                //Step 5
                //Minimize the Browser in which the Viewer page is opened and check for Font size 
                //Font size in the Viewer page should not be changed
                BasePage.Driver.Manage().Window.Position = new Point(0, 0);

                //Wait for window Minimize fully
                Thread.Sleep(5000);

                string PatinetNameFontSize = bluringviewer.PatinetName().GetCssValue("font-size");
                IList<IWebElement> PatientDemoDetailList = bluringviewer.PatientDemoDetailList();
                bool PatientDemoDetails = true;
                foreach (IWebElement EachPatinetDemoDetails in PatientDemoDetailList)
                    if (EachPatinetDemoDetails.GetCssValue("font-size") != GlobalToolBarItemFontSizeLarge[1])
                    {
                        PatientDemoDetails = false;
                        break;
                    }
                
                bool patinetnameFontsVerfiy = (PatinetNameFontSize == GlobalToolBarItemFontSizeLarge[0]);
                bool globalToolBars = patinetnameFontsVerfiy && PatientDemoDetails ;

                //Verify the Study Info font size which is available above the Thumbnails should get changed to Large in Size
                //Study Info font size which is available above the Thumbnails is changed to Large in Size
                List<IWebElement> AllStudyDateTimeAtStudyPanel = new List<IWebElement>();
                AllStudyDateTimeAtStudyPanel.AddRange(bluringviewer.AllStudyDateAtStudyPanel().ToList());
                bool boolAllStudyDateAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyDateTimeAtStudyPanel).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[0]));

                List<IWebElement> AllStudyTimeAtStudyPanel = new List<IWebElement>();
                AllStudyTimeAtStudyPanel.AddRange(bluringviewer.AllStudyTimeAtStudyPanel().ToList());
                bool boolAllStudyTimeAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanel).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[1]));

                List<IWebElement> AllStudyInfoAtStudyPanel = new List<IWebElement>();
                AllStudyInfoAtStudyPanel.AddRange(bluringviewer.AllStudyInfoAtStudyPanel().ToList());
                bool boolAllStudyInfoAtStudyPanel = bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanel).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[2]));

                // Verfiy the Thumbnail fonts in the thumbnail 
                List<IWebElement> thumbnail = new List<IWebElement>();
                thumbnail.AddRange(bluringviewer.ThumbnailPercentImagesViewed().ToList());
                thumbnail.AddRange(bluringviewer.ThumbnailPercentImagesViewed().ToList());
                thumbnail.AddRange(bluringviewer.ThumbnailImageFrameNumber().ToList());

                string[] thumbnailFonts = bluringviewer.getFontSizeofElements(thumbnail);

                bool studyInfoandThumbnail = (thumbnailFonts.All(c1 => c1.Equals(thumbnailFontLarge)) && boolAllStudyDateAtStudypanel && boolAllStudyTimeAtStudypanel && boolAllStudyInfoAtStudyPanel);

                // Exam list label;
                bool ExamListLable = (bluringviewer.ExamListLable().GetCssValue("font-size") == ExamListLableLarge);

                //Verfiy the Filter operator for the exam list 
                bluringviewer.OpenModalityFilter();
                string[] filterOperatorsFont = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text)).ToList());
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));

                bluringviewer.OpenSortDorpdown();
                string [] sortdropdown = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudySortOptionValues)).ToList());
                bool examOperators = filterOperatorsFont.All(c1 => c1.Equals(ExamListOperationContainerFontLarge)) && sortdropdown.All(c1 => c1.Equals(ExamListOperationContainerFontLarge));
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();

                //Verfiy the RecentStudyDate Font
                string[] RecentStudyAllDatesFonts = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDates().ToList());
                bool boolRecentStudyAllDatesFonts = RecentStudyAllDatesFonts.All(c1 => c1.Equals(listContainerFontLarge[0]));

                //Verfiy the RecentStudyTimesFont
                List<IWebElement> RecentStudyAllTimes = bluringviewer.RecentStudyAllTimes().ToList();
                string[] RecentStudyAllTimesFonts = bluringviewer.getFontSizeofElements(RecentStudyAllTimes);
                bool boolRecentStudyAllTimesFonts = RecentStudyAllTimesFonts.All(c1 => c1.Equals(listContainerFontLarge[1]));

                //Verfiy the RecentStudyModalityFont
                List<IWebElement> RecentStudyAllModality = bluringviewer.RecentStudyAllModality().ToList();
                string[] RecentStudyAllModalityFonts = bluringviewer.getFontSizeofElements(RecentStudyAllModality);
                bool boolRecentStudyAllModalityFonts = RecentStudyAllModalityFonts.All(c1 => c1.Equals(listContainerFontLarge[2]));

                //Verfiy the RecentStudyContrastFont
                List<IWebElement> RecentStudyAllDescription = bluringviewer.RecentStudyAllDescription().ToList();
                string[] RecentStudyAllDescriptionFonts = bluringviewer.getFontSizeofElements(RecentStudyAllDescription);
                bool boolRecentStudyAllDescriptionFonts = RecentStudyAllDescriptionFonts.All(c1 => c1.Equals(listContainerFontLarge[3]));

                //Add site
                //Verfiy the RecentStudySiteFont
                List<IWebElement> RecentStudyAllAccession = bluringviewer.RecentStudyAllAccession().ToList();
                string[] RecentStudyAllAccessionFonts = bluringviewer.getFontSizeofElements(RecentStudyAllAccession);
                bool boolRecentStudyAllAccessionFonts = RecentStudyAllAccessionFonts.All(c1 => c1.Equals(listContainerFontLarge[4]));

                bool examPanel = (ExamListLable && examOperators && boolRecentStudyAllDatesFonts && boolRecentStudyAllTimesFonts && boolRecentStudyAllModalityFonts && boolRecentStudyAllDescriptionFonts && boolRecentStudyAllAccessionFonts);

                //Step 5
                if (globalToolBars && studyInfoandThumbnail && examPanel)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //result.steps[++ExecutedSteps].status = "No Automation";


                //Step 6 
                //Maximize the Browser in which the Viewer page is opened and check for Font size
                //Font size in the Viewer page should not be changed
                BasePage.Driver.Manage().Window.Maximize();

                //Wait for window Maximize fully
                Thread.Sleep(10000);

                PatinetNameFontSize = bluringviewer.PatinetName().GetCssValue("font-size");
                IList<IWebElement> PatientDemoDetailListMax = bluringviewer.PatientDemoDetailList();
                PatientDemoDetails = true;
                foreach (IWebElement EachPatinetDemoDetails in PatientDemoDetailListMax)
                    if (EachPatinetDemoDetails.GetCssValue("font-size") != GlobalToolBarItemFontSizeLarge[1])
                    {
                        PatientDemoDetails = false;
                        break;
                    }
                 patinetnameFontsVerfiy = (PatinetNameFontSize == GlobalToolBarItemFontSizeLarge[0]);
                 globalToolBars = patinetnameFontsVerfiy && PatientDemoDetails ;

                //Verify the Study Info font size which is available above the Thumbnails should get changed to Large in Size
                //Study Info font size which is available above the Thumbnails is changed to Large in Size
                List<IWebElement> AllStudyDateTimeAtStudyPanelMax = new List<IWebElement>();
                AllStudyDateTimeAtStudyPanelMax.AddRange(bluringviewer.AllStudyDateAtStudyPanel().ToList());
                boolAllStudyDateAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyDateTimeAtStudyPanelMax).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[0]));

                List<IWebElement> AllStudyTimeAtStudyPanelMax = new List<IWebElement>();
                AllStudyTimeAtStudyPanelMax.AddRange(bluringviewer.AllStudyTimeAtStudyPanel().ToList());
                boolAllStudyTimeAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanelMax).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[1]));

               List<IWebElement> AllStudyInfoAtStudyPanelMax = new List<IWebElement>();
                AllStudyInfoAtStudyPanelMax.AddRange(bluringviewer.AllStudyInfoAtStudyPanel().ToList());
                boolAllStudyInfoAtStudyPanel = bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanelMax).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[2]));

                // Verfiy the Thumbnail fonts in the thumbnail 
                List<IWebElement> thumbnailMax = new List<IWebElement>();
                thumbnailMax.AddRange(bluringviewer.ThumbnailPercentImagesViewed().ToList());
                thumbnailMax.AddRange(bluringviewer.ThumbnailPercentImagesViewed().ToList());
                thumbnailMax.AddRange(bluringviewer.ThumbnailImageFrameNumber().ToList());

                thumbnailFonts = bluringviewer.getFontSizeofElements(thumbnailMax);

                studyInfoandThumbnail = (thumbnailFonts.All(c1 => c1.Equals(thumbnailFontLarge)) && boolAllStudyDateAtStudypanel && boolAllStudyTimeAtStudypanel && boolAllStudyInfoAtStudyPanel);

                // Exam list label;
                ExamListLable = (bluringviewer.ExamListLable().GetCssValue("font-size") == ExamListLableLarge);

                //Verfiy the Filter operator for the exam list 
                bluringviewer.OpenModalityFilter();
                filterOperatorsFont = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text)).ToList());
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));

                bluringviewer.OpenSortDorpdown();
                sortdropdown = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudySortOptionValues)).ToList());
                examOperators = filterOperatorsFont.All(c1 => c1.Equals(ExamListOperationContainerFontLarge)) && sortdropdown.All(c1 => c1.Equals(ExamListOperationContainerFontLarge));
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();

                //Verfiy the RecentStudyDate Font
                RecentStudyAllDatesFonts = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDates().ToList());
                boolRecentStudyAllDatesFonts = RecentStudyAllDatesFonts.All(c1 => c1.Equals(listContainerFontLarge[0]));

                //Verfiy the RecentStudyTimesFont
                List<IWebElement> RecentStudyAllTimesMax = bluringviewer.RecentStudyAllTimes().ToList();
                RecentStudyAllTimesFonts = bluringviewer.getFontSizeofElements(RecentStudyAllTimesMax);
                boolRecentStudyAllTimesFonts = RecentStudyAllTimesFonts.All(c1 => c1.Equals(listContainerFontLarge[1]));

                //Verfiy the RecentStudyModalityFont
                List<IWebElement> RecentStudyAllModalityMax = bluringviewer.RecentStudyAllModality().ToList();
                RecentStudyAllModalityFonts = bluringviewer.getFontSizeofElements(RecentStudyAllModalityMax);
                boolRecentStudyAllModalityFonts = RecentStudyAllModalityFonts.All(c1 => c1.Equals(listContainerFontLarge[2]));

                //Verfiy the RecentStudyContrastFont
                List<IWebElement> RecentStudyAllDescriptionMax = bluringviewer.RecentStudyAllDescription().ToList();
                RecentStudyAllDescriptionFonts = bluringviewer.getFontSizeofElements(RecentStudyAllDescriptionMax);
                boolRecentStudyAllDescriptionFonts = RecentStudyAllDescriptionFonts.All(c1 => c1.Equals(listContainerFontLarge[3]));

                //Add site
                //Verfiy the RecentStudySiteFont
                List<IWebElement> RecentStudyAllAccessionMax = bluringviewer.RecentStudyAllAccession().ToList();
                RecentStudyAllAccessionFonts = bluringviewer.getFontSizeofElements(RecentStudyAllAccessionMax);
                boolRecentStudyAllAccessionFonts = RecentStudyAllAccessionFonts.All(c1 => c1.Equals(listContainerFontLarge[4]));

                examPanel = (ExamListLable && examOperators && boolRecentStudyAllDatesFonts && boolRecentStudyAllTimesFonts && boolRecentStudyAllModalityFonts && boolRecentStudyAllDescriptionFonts && boolRecentStudyAllAccessionFonts);

                //Step 6 - Maximize 
                if (globalToolBars && studyInfoandThumbnail && examPanel)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }



                //Step 7 
                //Check for Font size in the Viewer page when the broswer window is dragged
                //Font size in the Viewer page should not be changed 

                // We resize the browser which is fianl output of the Dragging.
                BasePage.Driver.Manage().Window.Position = new Point(10, 10);

                //Wait for window To resize fully
                Thread.Sleep(10000);

                BasePage.Driver.Manage().Window.Maximize();
                PatinetNameFontSize = bluringviewer.PatinetName().GetCssValue("font-size");
                IList<IWebElement> PatientDemoDetailListReSize = bluringviewer.PatientDemoDetailList();
                PatientDemoDetails = true;
                foreach (IWebElement EachPatinetDemoDetails in PatientDemoDetailListReSize)
                    if (EachPatinetDemoDetails.GetCssValue("font-size") != GlobalToolBarItemFontSizeLarge[1])
                    {
                        PatientDemoDetails = false;
                        break;
                    }
                
                patinetnameFontsVerfiy = (PatinetNameFontSize == GlobalToolBarItemFontSizeLarge[0]);
                globalToolBars = patinetnameFontsVerfiy && PatientDemoDetails;

                //Verify the Study Info font size which is available above the Thumbnails should get changed to Large in Size
                //Study Info font size which is available above the Thumbnails is changed to Large in Size
                List<IWebElement> AllStudyDateTimeAtStudyPanelReSize = new List<IWebElement>();
                AllStudyDateTimeAtStudyPanelReSize.AddRange(bluringviewer.AllStudyDateAtStudyPanel().ToList());
                boolAllStudyDateAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyDateTimeAtStudyPanelReSize).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[0]));

                List<IWebElement> AllStudyTimeAtStudyPanelReSize = new List<IWebElement>();
                AllStudyTimeAtStudyPanelReSize.AddRange(bluringviewer.AllStudyTimeAtStudyPanel().ToList());
                boolAllStudyTimeAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanelReSize).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[1]));

                List<IWebElement> AllStudyInfoAtStudyPanelReSize= new List<IWebElement>();
                AllStudyInfoAtStudyPanelReSize.AddRange(bluringviewer.AllStudyInfoAtStudyPanel().ToList());
                boolAllStudyInfoAtStudyPanel = bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanelReSize).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[2]));

                // Verfiy the Thumbnail fonts in the thumbnail 
                List<IWebElement> thumbnailReSize = new List<IWebElement>();
                thumbnailReSize.AddRange(bluringviewer.ThumbnailPercentImagesViewed().ToList());
                thumbnailReSize.AddRange(bluringviewer.ThumbnailPercentImagesViewed().ToList());
                thumbnailReSize.AddRange(bluringviewer.ThumbnailImageFrameNumber().ToList());

                thumbnailFonts = bluringviewer.getFontSizeofElements(thumbnailReSize);

                studyInfoandThumbnail = (thumbnailFonts.All(c1 => c1.Equals(thumbnailFontLarge)) && boolAllStudyDateAtStudypanel && boolAllStudyTimeAtStudypanel && boolAllStudyInfoAtStudyPanel);

                // Exam list label;
                ExamListLable = (bluringviewer.ExamListLable().GetCssValue("font-size") == ExamListLableLarge);

                //Verfiy the Filter operator for the exam list 
                bluringviewer.OpenModalityFilter();
                filterOperatorsFont = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text)).ToList());
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));

                bluringviewer.OpenSortDorpdown();
                sortdropdown = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudySortOptionValues)).ToList());
                examOperators = filterOperatorsFont.All(c1 => c1.Equals(ExamListOperationContainerFontLarge)) && sortdropdown.All(c1 => c1.Equals(ExamListOperationContainerFontLarge));
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();

                //Verfiy the RecentStudyDate Font
                RecentStudyAllDatesFonts = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDates().ToList());
                boolRecentStudyAllDatesFonts = RecentStudyAllDatesFonts.All(c1 => c1.Equals(listContainerFontLarge[0]));

                //Verfiy the RecentStudyTimesFont
                List<IWebElement> RecentStudyAllTimesReSize = bluringviewer.RecentStudyAllTimes().ToList();
                RecentStudyAllTimesFonts = bluringviewer.getFontSizeofElements(RecentStudyAllTimesReSize);
                boolRecentStudyAllTimesFonts = RecentStudyAllTimesFonts.All(c1 => c1.Equals(listContainerFontLarge[1]));

                //Verfiy the RecentStudyModalityFont
                List<IWebElement> RecentStudyAllModalityResize = bluringviewer.RecentStudyAllModality().ToList();
                RecentStudyAllModalityFonts = bluringviewer.getFontSizeofElements(RecentStudyAllModalityResize);
                boolRecentStudyAllModalityFonts = RecentStudyAllModalityFonts.All(c1 => c1.Equals(listContainerFontLarge[2]));

                //Verfiy the RecentStudyContrastFont
                List<IWebElement> RecentStudyAllDescriptionReSize = bluringviewer.RecentStudyAllDescription().ToList();
                RecentStudyAllDescriptionFonts = bluringviewer.getFontSizeofElements(RecentStudyAllDescriptionReSize);
                boolRecentStudyAllDescriptionFonts = RecentStudyAllDescriptionFonts.All(c1 => c1.Equals(listContainerFontLarge[3]));

                //Add site
                //Verfiy the RecentStudySiteFont
                List<IWebElement> RecentStudyAllAccessionReSize = bluringviewer.RecentStudyAllAccession().ToList();
                RecentStudyAllAccessionFonts = bluringviewer.getFontSizeofElements(RecentStudyAllAccessionReSize);
                boolRecentStudyAllAccessionFonts = RecentStudyAllAccessionFonts.All(c1 => c1.Equals(listContainerFontLarge[4]));

                examPanel = (ExamListLable && examOperators && boolRecentStudyAllDatesFonts && boolRecentStudyAllTimesFonts && boolRecentStudyAllModalityFonts && boolRecentStudyAllDescriptionFonts && boolRecentStudyAllAccessionFonts);

                //Step 7 - Drag 
                if (globalToolBars && studyInfoandThumbnail && examPanel)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                bluringviewer.CloseBluRingViewer();
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                return result;
                //------------End of script---


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
                BasePage.Driver.Manage().Window.Maximize();
            }
        }

        /// <summary>
        /// Global user control Icons shall display within the Global header container
        /// </summary>
        public TestCaseResult Test_161556(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables               
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            wpfobject = new WpfObjects();
            ServiceTool servicetool = new ServiceTool();
            UserPreferences userPrefer = new UserPreferences();
            String adminUserName = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            string EA131 = login.GetHostName(Config.EA1);

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

                //Preconditon:
                // Enabling Network conncetion Test tool from service tool
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableConnectionTestTool();
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                //Preconditon:
                //Enabling Connection Test Tool In UserPreferences.
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminUserName, adminPassword);
                userPrefer.OpenUserPreferences();
                userPrefer.SwitchToUserPrefFrame();
                if (!userPrefer.EnableConnectionTestTool().Selected)
                {
                    login.ScrollIntoView(login.GetElement(BasePage.SelectorType.CssSelector, "input#ConnTestToolCB"));
                    userPrefer.EnableConnectionTestTool().Click();
                }
                userPrefer.CloseUserPreferences();
                login.Logout();

                // step 1 and 2 - Launch the Application and Login as Administrator
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps += 2;

                // step 3 - click on studies tab
                var studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                // Step 4 - Search a study using Accession, Select a study, and click on Universal button
                studies.SearchStudy(AccessionNo: Accession, Datasource: EA131);
                studies.SelectStudy("Accession", Accession);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                // Step 5 - Search the "User Settings" option at the top right corner in the Viewer page                
                var step5_1 = viewer.UserSettings("displayed", UsersettingLarge);
                var step5_2 = viewer.UserSettings("displayed", UsersettingMedium);
                var step5_3 = viewer.UserSettings("displayed", UsersettingSmall);
                if (step5_1 && step5_2 && step5_3)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                // step 6 - Click on UI-LARGE user control tool under User Settings option
                var step6_1 = viewer.UserSettings("select", UsersettingLarge);
                var step6_2 = viewer.UserSettings("checked", UsersettingLarge);
                if (step6_1 && step6_2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                // Step 7 - Verify that the global user control icons on Global header container
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_globalTitleBar));
                if (step7)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                // step 8 - Click on UI-SMALL user control tool under User Settings
                var step8_1 = viewer.UserSettings("select", UsersettingSmall);
                var step8_2 = viewer.UserSettings("checked", UsersettingSmall);
                if (step8_1 && step8_2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                // step 9 - Verify that the global user control icons on Global header container
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step9 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_globalTitleBar));
                if (step9)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                // Close the viewer and logout
                viewer.CloseBluRingViewer();
                login.Logout();

                //Return Result.
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
                // Reverting the settings in service tool
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.DisableConnectionTestTool();
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
            }
        }


      //  Not FOr regression Run.

        /// <summary> 
        /// Getdata_Large - To Get the font of the Text in the Universal viewer by setting the large in user settings
        /// </summary>
        ///
        public void Getdata_large(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                string Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                string Datasource = login.GetHostName(Config.EA91);

                // Step 1
                //Launch the BluRing application with a client browser (http//<BR IP>/WebAccess/) and hit enter  
                //The BluRing application login page should be displayed
                login.DriverGoTo(login.url);

                //Step 2
                //Login to WebAccess site with any privileged user.
                //Should be able to login to the BluRing application.
                login.LoginIConnect(adminusername, adminpassword);

                Studies study = (Studies)login.Navigate("Studies");

                study.SearchStudy(AccessionNo: "HOM_13ICCA1", Datasource: Datasource);
                PageLoadWait.WaitForPageLoad(20);
                study.SelectStudy("Accession", "HOM_13ICCA1");
                BluRingViewer bluringviewer = BluRingViewer.LaunchBluRingViewer();

                bool selected = bluringviewer.UserSettings("select", UsersettingLarge);
                bluringviewer.UserSettings("checked",UsersettingLarge);

                //Step 8
                if (!bluringviewer.SettingPanel().Displayed)
                    bluringviewer.ClickOnUSerSettings();
                IList<IWebElement> UserControlToolsList = BasePage.FindElementsByCss(BluRingViewer.div_userControlToolsList);
                string LogUserControlToolsList = UserControlToolsList[0].GetCssValue("font-size");
                Logger.Instance.InfoLog("********  UserSettingsListFontLarge : " + UserControlToolsList[0].GetCssValue("font-size"));

                //if (UserControlToolsList.All(settings => settings.GetCssValue("font-size").Equals(UserSettingsListFontLarge)))

                //Verify the Global Toolbar menus (Patient Info,EXAMS, Show/Hide Tool ,EXIT) font size should get changed to Large size in the viewer page.
                //Global toolbar menus font size is changed to Large size when the user selects the UI-LARGE control tool.
                string PatinetNameFontSize = bluringviewer.PatinetName().GetCssValue("font-size");
                IList<IWebElement> PatientDemoDetailList = bluringviewer.PatientDemoDetailList();
                string LogPatientDemoDetailList = PatientDemoDetailList[0].GetCssValue("font-size");
                //string LogExamTextLable = bluringviewer.ExamTextLable().GetCssValue("font-size");
               // string LogShowHideToolName = bluringviewer.ShowHideToolName().GetCssValue("font-size");

                Logger.Instance.InfoLog("********  GlobalToolBarItemFontSizeLarge[0] :  " + PatinetNameFontSize);
                Logger.Instance.InfoLog("********  GlobalToolBarItemFontSizeLarge[1] :  " + PatientDemoDetailList[0].GetCssValue("font-size"));
                Logger.Instance.InfoLog("********  GlobalToolBarItemFontSizeLarge[2] :  " + PatientDemoDetailList[0].GetCssValue("font-size"));
                //Logger.Instance.InfoLog("********  GlobalToolBarItemFontSizeLarge[3] :  " + bluringviewer.ExamTextLable().GetCssValue("font-size"));
                //Logger.Instance.InfoLog("********  GlobalToolBarItemFontSizeLarge[4] :  " + bluringviewer.ShowHideToolName().GetCssValue("font-size"));
                Logger.Instance.InfoLog("********  GlobalToolBarItemFontSizeLarge[3] :  " );
                Logger.Instance.InfoLog("********  GlobalToolBarItemFontSizeLarge[4] :  ");

                //bool PatientDemoDetails = true;
                //foreach (IWebElement EachPatinetDemoDetails in PatientDemoDetailList)
                //{
                //    if (EachPatinetDemoDetails.GetCssValue("font-size") != GlobalToolBarItemFontSizeLarge[1])
                //    {
                //        PatientDemoDetails = false;
                //        break;
                //    }
                //}

                // //string //ExamIconLable = bluringviewer.ExamTextLable().GetCssValue("font-size");
                // //string // ShowHideIconlable = bluringviewer.ShowHideToolName().GetCssValue("font-size");

                //Verify the Study Info font size which is available above the Thumbnails should get changed to Large in Size
                //Study Info font size which is available above the Thumbnails is changed to Large in Size

                List<IWebElement> AllStudyDateTimeAtStudyPanel = new List<IWebElement>();
                AllStudyDateTimeAtStudyPanel.AddRange(bluringviewer.AllStudyDateAtStudyPanel().ToList());
                string LogAllStudyDateTimeAtStudyPanel = bluringviewer.getFontSizeofElements(AllStudyDateTimeAtStudyPanel)[0];
                Logger.Instance.InfoLog("********  studyInformationAtStudyPanelLarge[0]:  " + bluringviewer.getFontSizeofElements(AllStudyDateTimeAtStudyPanel)[0]);

                //bool boolAllStudyDateAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyDateTimeAtStudyPanel).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[0]));

                List<IWebElement> AllStudyTimeAtStudyPanel = new List<IWebElement>();
                AllStudyTimeAtStudyPanel.AddRange(bluringviewer.AllStudyTimeAtStudyPanel().ToList());
                string LogAllStudyTimeAtStudyPanel = bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanel)[0];
                Logger.Instance.InfoLog("********  studyInformationAtStudyPanelLarge[1]:  " + bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanel)[0]);


                //bool boolAllStudyTimeAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanel).All(c1 => c1.Equals(studyInformationAtStudyPanelLarge[1]));

                List<IWebElement> AllStudyInfoAtStudyPanel = new List<IWebElement>();
                AllStudyInfoAtStudyPanel.AddRange(bluringviewer.AllStudyInfoAtStudyPanel().ToList());
                string LogAllStudyInfoAtStudyPanel = bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanel)[0];
                Logger.Instance.InfoLog("********  studyInformationAtStudyPanelLarge[2]:  " + bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanel)[0]);

                string LogExamListlablelarge = bluringviewer.ExamListLable().GetCssValue("font-size");
                Logger.Instance.InfoLog("********  ExamListLableLarge:  " + bluringviewer.ExamListLable().GetCssValue("font-size"));

                //Verfiy the Filter operator for the exam list 
                bluringviewer.OpenModalityFilter();
                string[] filterOperatorsFont = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text)).ToList());
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));

                bluringviewer.OpenSortDorpdown();
                string[] sortdropdown = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudySortOptionValues)).ToList());

                Logger.Instance.InfoLog("********  ExamListLableLarge:  " + bluringviewer.ExamListLable().GetCssValue("font-size"));

                Logger.Instance.InfoLog("********  ExamListOperationContainerFontLarge:  " + filterOperatorsFont[0]);
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();


                //Verfiy the RecentStudyDate Font
                string[] RecentStudyAllDatesFonts = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDates().ToList());
                //bool boolRecentStudyAllDatesFonts = RecentStudyAllDatesFonts.All(c1 => c1.Equals(listContainerFontLarge[0]));
                Logger.Instance.InfoLog("listContainerFontLarge[0]:  " + RecentStudyAllDatesFonts[0]);


                //Verfiy the RecentStudyTimesFont
                List<IWebElement> RecentStudyAllTimes = bluringviewer.RecentStudyAllTimes().ToList();
                string[] RecentStudyAllTimesFonts = bluringviewer.getFontSizeofElements(RecentStudyAllTimes);
                Logger.Instance.InfoLog("********  listContainerFontLarge[1]:  " + RecentStudyAllTimesFonts[0]);

                bool boolRecentStudyAllTimesFonts = RecentStudyAllTimesFonts.All(c1 => c1.Equals(listContainerFontLarge[1]));

                //Verfiy the RecentStudyModalityFont
                List<IWebElement> RecentStudyAllModality = bluringviewer.RecentStudyAllModality().ToList();
                string[] RecentStudyAllModalityFonts = bluringviewer.getFontSizeofElements(RecentStudyAllModality);
                Logger.Instance.InfoLog("********  listContainerFontLarge[2]:  " + RecentStudyAllModalityFonts[0]);
                // bool boolRecentStudyAllModalityFonts = RecentStudyAllModalityFonts.All(c1 => c1.Equals(listContainerFontLarge[2]));

                //Verfiy the RecentStudyContrastFont
                List<IWebElement> RecentStudyAllDescription = bluringviewer.RecentStudyAllDescription().ToList();
                string[] RecentStudyAllDescriptionFonts = bluringviewer.getFontSizeofElements(RecentStudyAllDescription);
                Logger.Instance.InfoLog("********  listContainerFontLarge[3]:  " + RecentStudyAllDescriptionFonts[0]);

                //bool boolRecentStudyAllDescriptionFonts = RecentStudyAllDescriptionFonts.All(c1 => c1.Equals(listContainerFontLarge[3]));

                //Add site
                //Verfiy the RecentStudySiteFont
                List<IWebElement> RecentStudyAllAccession = bluringviewer.RecentStudyAllAccession().ToList();
                string[] RecentStudyAllAccessionFonts = bluringviewer.getFontSizeofElements(RecentStudyAllAccession);
                Logger.Instance.InfoLog("********  listContainerFontLarge[4]:  " + RecentStudyAllAccession[0]);
                //bool boolRecentStudyAllAccessionFonts = RecentStudyAllAccessionFonts.All(c1 => c1.Equals(listContainerFontLarge[4]));


                //step 18
                // Verfiy the Thumbnail fonts in the thumbnail 
                List<IWebElement> thumbnail = new List<IWebElement>();
                thumbnail.AddRange(bluringviewer.ThumbnailPercentImagesViewed().ToList());
                thumbnail.AddRange(bluringviewer.ThumbnailImageFrameNumber().ToList());

                string[] thumbnailFonts = bluringviewer.getFontSizeofElements(thumbnail);
                Logger.Instance.InfoLog("********  thumbnailFontLarge:  " + thumbnailFonts[0]);

                //  if (thumbnailFonts.All(c1 => c1.Equals(thumbnailFontLarge)))

                string[] RecentStudythumbnailPreviewFontsLarge = new string[] { };
                //Step 19
                //Select any Thumbnail Preview for the prior study and verify the Series and Image information text font size in thumbnails should be Large in Size
                //Series and Image information text font size in thumbnails is in Large in Size.
                if (bluringviewer.RecentStudythumbnailpreviewIcons().Count == 0)
                {
                    Logger.Instance.ErrorLog("Unable to select the Thumbnail Preview of the Recent study, as there is no recent study");
                }
                else
                {
                    if (Config.BrowserType != "ie")
                        bluringviewer.RecentStudythumbnailpreviewIcons()[0].Click();
                    else
                        ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", bluringviewer.RecentStudythumbnailpreviewIcons()[0]);

                    BluRingViewer.WaitforThumbnails();


                    Thread.Sleep(5000);

                    if (!bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed)
                    {
                        Logger.Instance.ErrorLog("Thumbnail preview is not displayed");
                    }
                    else
                    {
                        List<IWebElement> ThumbPreviewCaptions = new List<IWebElement>();
                        ThumbPreviewCaptions.AddRange(bluringviewer.RecentStudyActiveThumbnailCaption().ToList());
                        ThumbPreviewCaptions.AddRange(bluringviewer.RecentStudyActiveThumbnailImageFrameNumber().ToList());

                        RecentStudythumbnailPreviewFontsLarge = bluringviewer.getFontSizeofElements(ThumbPreviewCaptions);
                        bool boolRecentStudythumbnailPreviewFontsLarge = RecentStudythumbnailPreviewFontsLarge.All(c1 => c1.Equals(RecentStudyThumbnailFontsLarge));
                        Logger.Instance.InfoLog("********  RecentStudyThumbnailFontsLarge:  " + RecentStudythumbnailPreviewFontsLarge[0]);
                    }
                }


                bluringviewer.CloseBluRingViewer();
                login.Logout();

                Logger.Instance.InfoLog("******** thumbnailFontLarge = " + thumbnailFonts[0]);
                Logger.Instance.InfoLog("******** studyInformationAtStudyPanelLarge = { " + LogAllStudyDateTimeAtStudyPanel + ", " + LogAllStudyTimeAtStudyPanel + ", " + LogAllStudyInfoAtStudyPanel + " }; ");
                //Logger.Instance.InfoLog("******** GlobalToolBarItemFontSizeLarge = { " + PatinetNameFontSize + ", " + LogPatientDemoDetailList + ", " + LogPatientDemoDetailList + ", " + LogExamTextLable + ", " + LogShowHideToolName + ", " + LogExitButtonLable + " }");
                Logger.Instance.InfoLog("******** GlobalToolBarItemFontSizeLarge = { " + PatinetNameFontSize + ", " + LogPatientDemoDetailList + ", " + LogPatientDemoDetailList + ", " + "Blank" + ", " + "Blank" + ", " + " }");
                Logger.Instance.InfoLog("******** ExamListOperationContainerFontLarge = " + filterOperatorsFont[0]);
                Logger.Instance.InfoLog("******** ExamListLableLarge = " + LogExamListlablelarge);
                Logger.Instance.InfoLog("******** listContainerFontLarge = { " + RecentStudyAllDatesFonts[0] + "," + RecentStudyAllTimesFonts[0] + ", " + RecentStudyAllModalityFonts[0] + ", " + RecentStudyAllDescriptionFonts[0] + ", " + RecentStudyAllAccessionFonts[0] + "}");
                Logger.Instance.InfoLog("******** RecentStudyThumbnailFontsLarge = " + RecentStudythumbnailPreviewFontsLarge[0]);
                Logger.Instance.InfoLog("********  UserSettingsListFontLarge = " + LogUserControlToolsList);

                //Report Result

                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                //------------End of script---

            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Logout
                login.Logout();

                //Return Result
            }
        }

        //Not FOr regression Run.
        /// <summary> 
        /// Getdata_Medium - To get the font of the Text in the Universal viewer by setting the medium in user settings
        /// </summary>
        ///
        public void Getdata_Medium(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                string Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                string Datasource = login.GetHostName(Config.EA91);

                // Step 1
                //Launch the BluRing application with a client browser (http//<BR IP>/WebAccess/) and hit enter  
                //The BluRing application login page should be displayed
                login.DriverGoTo(login.url);

                //Step 2
                //Login to WebAccess site with any privileged user.
                //Should be able to login to the BluRing application.
                login.LoginIConnect(adminusername, adminpassword);

                Studies study = (Studies)login.Navigate("Studies");

                study.SearchStudy(AccessionNo: "HOM_13ICCA1", Datasource: Datasource);
                PageLoadWait.WaitForPageLoad(20);
                study.SelectStudy("Accession", "HOM_13ICCA1");
                BluRingViewer bluringviewer = BluRingViewer.LaunchBluRingViewer();

                bool selected = bluringviewer.UserSettings("select", UsersettingMedium);
                bluringviewer.UserSettings("checked", UsersettingMedium);

                //Step 8
                if (!bluringviewer.SettingPanel().Displayed)
                    bluringviewer.ClickOnUSerSettings();
                IList<IWebElement> UserControlToolsList = BasePage.FindElementsByCss(BluRingViewer.div_userControlToolsList);
                string LogUserControlToolsList = UserControlToolsList[0].GetCssValue("font-size");
                Logger.Instance.InfoLog("********  UserSettingsListFontMedium : " + UserControlToolsList[0].GetCssValue("font-size"));

                //if (UserControlToolsList.All(settings => settings.GetCssValue("font-size").Equals(UserSettingsListFontMedium)))

                //Verify the Global Toolbar menus (Patient Info,EXAMS, Show/Hide Tool ,EXIT) font size should get changed to Medium size in the viewer page.
                //Global toolbar menus font size is changed to Medium size when the user selects the UI-Medium control tool.
                string PatinetNameFontSize = bluringviewer.PatinetName().GetCssValue("font-size");
                IList<IWebElement> PatientDemoDetailList = bluringviewer.PatientDemoDetailList();
                string LogPatientDemoDetailList = PatientDemoDetailList[0].GetCssValue("font-size");

                Logger.Instance.InfoLog("********  GlobalToolBarItemFontSizeMedium[0] :  " + PatinetNameFontSize);
                Logger.Instance.InfoLog("********  GlobalToolBarItemFontSizeMedium[1] :  " + PatientDemoDetailList[0].GetCssValue("font-size"));
                Logger.Instance.InfoLog("********  GlobalToolBarItemFontSizeMedium[2] :  " + PatientDemoDetailList[0].GetCssValue("font-size"));
                //Logger.Instance.InfoLog("********  GlobalToolBarItemFontSizeMedium[3] :  " + bluringviewer.ExamTextLable().GetCssValue("font-size"));
                //Logger.Instance.InfoLog("********  GlobalToolBarItemFontSizeMedium[4] :  " + bluringviewer.ShowHideToolName().GetCssValue("font-size"));
                Logger.Instance.InfoLog("********  GlobalToolBarItemFontSizeMedium[3] :  ");
                Logger.Instance.InfoLog("********  GlobalToolBarItemFontSizeMedium[4] :  ");



                //bool PatientDemoDetails = true;
                //foreach (IWebElement EachPatinetDemoDetails in PatientDemoDetailList)
                //{
                //    if (EachPatinetDemoDetails.GetCssValue("font-size") != GlobalToolBarItemFontSizeMedium[1])
                //    {
                //        PatientDemoDetails = false;
                //        break;
                //    }
                //}

                //Verify the Study Info font size which is available above the Thumbnails should get changed to Medium in Size
                //Study Info font size which is available above the Thumbnails is changed to Medium in Size

                List<IWebElement> AllStudyDateTimeAtStudyPanel = new List<IWebElement>();
                AllStudyDateTimeAtStudyPanel.AddRange(bluringviewer.AllStudyDateAtStudyPanel().ToList());
                string LogAllStudyDateTimeAtStudyPanel = bluringviewer.getFontSizeofElements(AllStudyDateTimeAtStudyPanel)[0];
                Logger.Instance.InfoLog("********  studyInformationAtStudyPanelMedium[0]:  " + bluringviewer.getFontSizeofElements(AllStudyDateTimeAtStudyPanel)[0]);

                //bool boolAllStudyDateAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyDateTimeAtStudyPanel).All(c1 => c1.Equals(studyInformationAtStudyPanelMedium[0]));

                List<IWebElement> AllStudyTimeAtStudyPanel = new List<IWebElement>();
                AllStudyTimeAtStudyPanel.AddRange(bluringviewer.AllStudyTimeAtStudyPanel().ToList());
                string LogAllStudyTimeAtStudyPanel = bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanel)[0];
                Logger.Instance.InfoLog("********  studyInformationAtStudyPanelMedium[1]:  " + bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanel)[0]);


                //bool boolAllStudyTimeAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanel).All(c1 => c1.Equals(studyInformationAtStudyPanelMedium[1]));

                List<IWebElement> AllStudyInfoAtStudyPanel = new List<IWebElement>();
                AllStudyInfoAtStudyPanel.AddRange(bluringviewer.AllStudyInfoAtStudyPanel().ToList());
                string LogAllStudyInfoAtStudyPanel = bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanel)[0];
                Logger.Instance.InfoLog("********  studyInformationAtStudyPanelMedium[2]:  " + bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanel)[0]);

                string LogExamListlableMedium = bluringviewer.ExamListLable().GetCssValue("font-size");
                Logger.Instance.InfoLog("********  ExamListLableMedium:  " + bluringviewer.ExamListLable().GetCssValue("font-size"));

                //Verfiy the Filter operator for the exam list 
                bluringviewer.OpenModalityFilter();
                string[] filterOperatorsFont = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text)).ToList());
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));

                bluringviewer.OpenSortDorpdown();
                string[] sortdropdown = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudySortOptionValues)).ToList());

                Logger.Instance.InfoLog("********  ExamListLableMedium:  " + bluringviewer.ExamListLable().GetCssValue("font-size"));

                Logger.Instance.InfoLog("********  ExamListOperationContainerFontMedium:  " + filterOperatorsFont[0]);
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();


                //Verfiy the RecentStudyDate Font
                string[] RecentStudyAllDatesFonts = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDates().ToList());
                //bool boolRecentStudyAllDatesFonts = RecentStudyAllDatesFonts.All(c1 => c1.Equals(listContainerFontMedium[0]));
                Logger.Instance.InfoLog("listContainerFontMedium[0]:  " + RecentStudyAllDatesFonts[0]);


                //Verfiy the RecentStudyTimesFont
                List<IWebElement> RecentStudyAllTimes = bluringviewer.RecentStudyAllTimes().ToList();
                string[] RecentStudyAllTimesFonts = bluringviewer.getFontSizeofElements(RecentStudyAllTimes);
                Logger.Instance.InfoLog("********  listContainerFontMedium[1]:  " + RecentStudyAllTimesFonts[0]);

                bool boolRecentStudyAllTimesFonts = RecentStudyAllTimesFonts.All(c1 => c1.Equals(listContainerFontMedium[1]));

                //Verfiy the RecentStudyModalityFont
                List<IWebElement> RecentStudyAllModality = bluringviewer.RecentStudyAllModality().ToList();
                string[] RecentStudyAllModalityFonts = bluringviewer.getFontSizeofElements(RecentStudyAllModality);
                Logger.Instance.InfoLog("********  listContainerFontMedium[2]:  " + RecentStudyAllModalityFonts[0]);
                // bool boolRecentStudyAllModalityFonts = RecentStudyAllModalityFonts.All(c1 => c1.Equals(listContainerFontMedium[2]));

                //Verfiy the RecentStudyContrastFont
                List<IWebElement> RecentStudyAllDescription = bluringviewer.RecentStudyAllDescription().ToList();
                string[] RecentStudyAllDescriptionFonts = bluringviewer.getFontSizeofElements(RecentStudyAllDescription);
                Logger.Instance.InfoLog("********  listContainerFontMedium[3]:  " + RecentStudyAllDescriptionFonts[0]);

                //bool boolRecentStudyAllDescriptionFonts = RecentStudyAllDescriptionFonts.All(c1 => c1.Equals(listContainerFontMedium[3]));

                //Add site
                //Verfiy the RecentStudySiteFont
                List<IWebElement> RecentStudyAllAccession = bluringviewer.RecentStudyAllAccession().ToList();
                string[] RecentStudyAllAccessionFonts = bluringviewer.getFontSizeofElements(RecentStudyAllAccession);
                Logger.Instance.InfoLog("********  listContainerFontMedium[4]:  " + RecentStudyAllAccession[0]);
                //bool boolRecentStudyAllAccessionFonts = RecentStudyAllAccessionFonts.All(c1 => c1.Equals(listContainerFontMedium[4]));


                //step 18
                // Verfiy the Thumbnail fonts in the thumbnail 
                List<IWebElement> thumbnail = new List<IWebElement>();
                thumbnail.AddRange(bluringviewer.ThumbnailPercentImagesViewed().ToList());
                thumbnail.AddRange(bluringviewer.ThumbnailImageFrameNumber().ToList());

                string[] thumbnailFonts = bluringviewer.getFontSizeofElements(thumbnail);
                Logger.Instance.InfoLog("********  thumbnailFontMedium:  " + thumbnailFonts[0]);

                //  if (thumbnailFonts.All(c1 => c1.Equals(thumbnailFontMedium)))

                string[] RecentStudythumbnailPreviewFontsMedium = new string[] { };
                //Step 19
                //Select any Thumbnail Preview for the prior study and verify the Series and Image information text font size in thumbnails should be Medium in Size
                //Series and Image information text font size in thumbnails is in Medium in Size.
                if (bluringviewer.RecentStudythumbnailpreviewIcons().Count == 0)
                {
                    Logger.Instance.ErrorLog("Unable to select the Thumbnail Preview of the Recent study, as there is no recent study");
                }
                else
                {
                    if (Config.BrowserType != "ie")
                        bluringviewer.RecentStudythumbnailpreviewIcons()[0].Click();
                    else
                        ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", bluringviewer.RecentStudythumbnailpreviewIcons()[0]);

                    BluRingViewer.WaitforThumbnails();

                    Thread.Sleep(5000);

                    if (!bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed)
                    {
                        Logger.Instance.ErrorLog("Thumbnail preview is not displayed");
                    }
                    else
                    {
                        List<IWebElement> ThumbPreviewCaptions = new List<IWebElement>();
                        ThumbPreviewCaptions.AddRange(bluringviewer.RecentStudyActiveThumbnailCaption().ToList());
                        ThumbPreviewCaptions.AddRange(bluringviewer.RecentStudyActiveThumbnailImageFrameNumber().ToList());

                        RecentStudythumbnailPreviewFontsMedium = bluringviewer.getFontSizeofElements(ThumbPreviewCaptions);
                        bool boolRecentStudythumbnailPreviewFontsMedium = RecentStudythumbnailPreviewFontsMedium.All(c1 => c1.Equals(RecentStudyThumbnailFontsMedium));
                        Logger.Instance.InfoLog("********  RecentStudyThumbnailFontsMedium:  " + RecentStudythumbnailPreviewFontsMedium[0]);
                    }
                }


                bluringviewer.CloseBluRingViewer();
                login.Logout();

                Logger.Instance.InfoLog("******** thumbnailFontMedium = " + thumbnailFonts[0]);
                Logger.Instance.InfoLog("******** studyInformationAtStudyPanelMedium = { " + LogAllStudyDateTimeAtStudyPanel + ", " + LogAllStudyTimeAtStudyPanel + ", " + LogAllStudyInfoAtStudyPanel + " }; ");
                //Logger.Instance.InfoLog("******** GlobalToolBarItemFontSizeMedium = { " + PatinetNameFontSize + ", " + LogPatientDemoDetailList + ", " + LogPatientDemoDetailList + ", " + LogExamTextLable + ", " + LogShowHideToolName + ", " + LogExitButtonLable + " }");
                Logger.Instance.InfoLog("******** GlobalToolBarItemFontSizeMedium = { " + PatinetNameFontSize + ", " + LogPatientDemoDetailList + ", " + LogPatientDemoDetailList + ", " + "Blank" + ", " + "Blank" + ", " +  " }");
                Logger.Instance.InfoLog("******** ExamListOperationContainerFontMedium = " + filterOperatorsFont[0]);
                Logger.Instance.InfoLog("******** ExamListLableMedium = " + LogExamListlableMedium);
                Logger.Instance.InfoLog("******** listContainerFontMedium = { " + RecentStudyAllDatesFonts[0] + "," + RecentStudyAllTimesFonts[0] + ", " + RecentStudyAllModalityFonts[0] + ", " + RecentStudyAllDescriptionFonts[0] + ", " + RecentStudyAllAccessionFonts[0] + "}");
                Logger.Instance.InfoLog("******** RecentStudyThumbnailFontsMedium = " + RecentStudythumbnailPreviewFontsMedium[0]);
                Logger.Instance.InfoLog("********  UserSettingsListFontMedium = " + LogUserControlToolsList);

                //Report Result

                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                //------------End of script---

            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Logout
                login.Logout();

                //Return Result
            }
        }

        //Not FOr regression Run.
        /// <summary> 
        /// Getdata_Small - To get the font of the Text in the Universal viewer by setting the Small in user settings
        /// </summary>
        ///
        public void Getdata_Small(string testid, string teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                string Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                string Datasource = login.GetHostName(Config.EA91);

                // Step 1
                //Launch the BluRing application with a client browser (http//<BR IP>/WebAccess/) and hit enter  
                //The BluRing application login page should be displayed
                login.DriverGoTo(login.url);

                //Step 2
                //Login to WebAccess site with any privileged user.
                //Should be able to login to the BluRing application.
                login.LoginIConnect(adminusername, adminpassword);

                Studies study = (Studies)login.Navigate("Studies");

                study.SearchStudy(AccessionNo: "HOM_13ICCA1", Datasource: Datasource);
                PageLoadWait.WaitForPageLoad(20);
                study.SelectStudy("Accession", "HOM_13ICCA1");
                BluRingViewer bluringviewer = BluRingViewer.LaunchBluRingViewer();

                bool selected = bluringviewer.UserSettings("select", UsersettingSmall);
                bluringviewer.UserSettings("checked", UsersettingSmall);

                //Step 8
                if (!bluringviewer.SettingPanel().Displayed)
                    bluringviewer.ClickOnUSerSettings();
                IList<IWebElement> UserControlToolsList = BasePage.FindElementsByCss(BluRingViewer.div_userControlToolsList);
                string LogUserControlToolsList = UserControlToolsList[0].GetCssValue("font-size");
                Logger.Instance.InfoLog("********  UserSettingsListFontSmall : " + UserControlToolsList[0].GetCssValue("font-size"));

                //if (UserControlToolsList.All(settings => settings.GetCssValue("font-size").Equals(UserSettingsListFontSmall)))

                //Verify the Global Toolbar menus (Patient Info,EXAMS, Show/Hide Tool ,EXIT) font size should get changed to Small size in the viewer page.
                //Global toolbar menus font size is changed to Small size when the user selects the UI-Small control tool.
                string PatinetNameFontSize = bluringviewer.PatinetName().GetCssValue("font-size");
                IList<IWebElement> PatientDemoDetailList = bluringviewer.PatientDemoDetailList();
                string LogPatientDemoDetailList = PatientDemoDetailList[0].GetCssValue("font-size");
                //string LogExamTextLable = bluringviewer.ExamTextLable().GetCssValue("font-size");
                //string LogShowHideToolName = bluringviewer.ShowHideToolName().GetCssValue("font-size");

                Logger.Instance.InfoLog("********  GlobalToolBarItemFontSizeSmall[0] :  " + PatinetNameFontSize);
                Logger.Instance.InfoLog("********  GlobalToolBarItemFontSizeSmall[1] :  " + PatientDemoDetailList[0].GetCssValue("font-size"));
                Logger.Instance.InfoLog("********  GlobalToolBarItemFontSizeSmall[2] :  " + PatientDemoDetailList[0].GetCssValue("font-size"));
                //Logger.Instance.InfoLog("********  GlobalToolBarItemFontSizeSmall[3] :  " + bluringviewer.ExamTextLable().GetCssValue("font-size"));
                //Logger.Instance.InfoLog("********  GlobalToolBarItemFontSizeSmall[4] :  " + bluringviewer.ShowHideToolName().GetCssValue("font-size"));
                Logger.Instance.InfoLog("********  GlobalToolBarItemFontSizeSmall[3] :  ");
                Logger.Instance.InfoLog("********  GlobalToolBarItemFontSizeSmall[4] :  ");


                //bool PatientDemoDetails = true;
                //foreach (IWebElement EachPatinetDemoDetails in PatientDemoDetailList)
                //{
                //    if (EachPatinetDemoDetails.GetCssValue("font-size") != GlobalToolBarItemFontSizeSmall[1])
                //    {
                //        PatientDemoDetails = false;
                //        break;
                //    }
                //}

                // //string //ExamIconLable = bluringviewer.ExamTextLable().GetCssValue("font-size");
                // //string // ShowHideIconlable = bluringviewer.ShowHideToolName().GetCssValue("font-size");

                //Verify the Study Info font size which is available above the Thumbnails should get changed to Small in Size
                //Study Info font size which is available above the Thumbnails is changed to Small in Size

                List<IWebElement> AllStudyDateTimeAtStudyPanel = new List<IWebElement>();
                AllStudyDateTimeAtStudyPanel.AddRange(bluringviewer.AllStudyDateAtStudyPanel().ToList());
                string LogAllStudyDateTimeAtStudyPanel = bluringviewer.getFontSizeofElements(AllStudyDateTimeAtStudyPanel)[0];
                Logger.Instance.InfoLog("********  studyInformationAtStudyPanelSmall[0]:  " + bluringviewer.getFontSizeofElements(AllStudyDateTimeAtStudyPanel)[0]);

                //bool boolAllStudyDateAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyDateTimeAtStudyPanel).All(c1 => c1.Equals(studyInformationAtStudyPanelSmall[0]));

                List<IWebElement> AllStudyTimeAtStudyPanel = new List<IWebElement>();
                AllStudyTimeAtStudyPanel.AddRange(bluringviewer.AllStudyTimeAtStudyPanel().ToList());
                string LogAllStudyTimeAtStudyPanel = bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanel)[0];
                Logger.Instance.InfoLog("********  studyInformationAtStudyPanelSmall[1]:  " + bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanel)[0]);


                //bool boolAllStudyTimeAtStudypanel = bluringviewer.getFontSizeofElements(AllStudyTimeAtStudyPanel).All(c1 => c1.Equals(studyInformationAtStudyPanelSmall[1]));

                List<IWebElement> AllStudyInfoAtStudyPanel = new List<IWebElement>();
                AllStudyInfoAtStudyPanel.AddRange(bluringviewer.AllStudyInfoAtStudyPanel().ToList());
                string LogAllStudyInfoAtStudyPanel = bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanel)[0];
                Logger.Instance.InfoLog("********  studyInformationAtStudyPanelSmall[2]:  " + bluringviewer.getFontSizeofElements(AllStudyInfoAtStudyPanel)[0]);

                string LogExamListlableSmall = bluringviewer.ExamListLable().GetCssValue("font-size");
                Logger.Instance.InfoLog("********  ExamListLableSmall:  " + bluringviewer.ExamListLable().GetCssValue("font-size"));

                //Verfiy the Filter operator for the exam list 
                bluringviewer.OpenModalityFilter();
                string[] filterOperatorsFont = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.modality_options_text)).ToList());
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_modalityFilterPopup_Title)));
                bluringviewer.OpenSortDorpdown();
                string[] sortdropdown = bluringviewer.getFontSizeofElements(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_StudySortOptionValues)).ToList());

                Logger.Instance.InfoLog("********  ExamListLableSmall:  " + bluringviewer.ExamListLable().GetCssValue("font-size"));

                Logger.Instance.InfoLog("********  ExamListOperationContainerFontSmall:  " + filterOperatorsFont[0]);
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab.ToString()).Build().Perform();


                //Verfiy the RecentStudyDate Font
                string[] RecentStudyAllDatesFonts = bluringviewer.getFontSizeofElements(bluringviewer.RecentStudyAllDates().ToList());
                //bool boolRecentStudyAllDatesFonts = RecentStudyAllDatesFonts.All(c1 => c1.Equals(listContainerFontSmall[0]));
                Logger.Instance.InfoLog("listContainerFontSmall[0]:  " + RecentStudyAllDatesFonts[0]);


                //Verfiy the RecentStudyTimesFont
                List<IWebElement> RecentStudyAllTimes = bluringviewer.RecentStudyAllTimes().ToList();
                string[] RecentStudyAllTimesFonts = bluringviewer.getFontSizeofElements(RecentStudyAllTimes);
                Logger.Instance.InfoLog("********  listContainerFontSmall[1]:  " + RecentStudyAllTimesFonts[0]);

                bool boolRecentStudyAllTimesFonts = RecentStudyAllTimesFonts.All(c1 => c1.Equals(listContainerFontSmall[1]));

                //Verfiy the RecentStudyModalityFont
                List<IWebElement> RecentStudyAllModality = bluringviewer.RecentStudyAllModality().ToList();
                string[] RecentStudyAllModalityFonts = bluringviewer.getFontSizeofElements(RecentStudyAllModality);
                Logger.Instance.InfoLog("********  listContainerFontSmall[2]:  " + RecentStudyAllModalityFonts[0]);
                // bool boolRecentStudyAllModalityFonts = RecentStudyAllModalityFonts.All(c1 => c1.Equals(listContainerFontSmall[2]));

                //Verfiy the RecentStudyContrastFont
                List<IWebElement> RecentStudyAllDescription = bluringviewer.RecentStudyAllDescription().ToList();
                string[] RecentStudyAllDescriptionFonts = bluringviewer.getFontSizeofElements(RecentStudyAllDescription);
                Logger.Instance.InfoLog("********  listContainerFontSmall[3]:  " + RecentStudyAllDescriptionFonts[0]);

                //bool boolRecentStudyAllDescriptionFonts = RecentStudyAllDescriptionFonts.All(c1 => c1.Equals(listContainerFontSmall[3]));

                //Add site
                //Verfiy the RecentStudySiteFont
                List<IWebElement> RecentStudyAllAccession = bluringviewer.RecentStudyAllAccession().ToList();
                string[] RecentStudyAllAccessionFonts = bluringviewer.getFontSizeofElements(RecentStudyAllAccession);
                Logger.Instance.InfoLog("********  listContainerFontSmall[4]:  " + RecentStudyAllAccession[0]);
                //bool boolRecentStudyAllAccessionFonts = RecentStudyAllAccessionFonts.All(c1 => c1.Equals(listContainerFontSmall[4]));


                //step 18
                // Verfiy the Thumbnail fonts in the thumbnail 
                List<IWebElement> thumbnail = new List<IWebElement>();
                thumbnail.AddRange(bluringviewer.ThumbnailPercentImagesViewed().ToList());
                thumbnail.AddRange(bluringviewer.ThumbnailImageFrameNumber().ToList());

                string[] thumbnailFonts = bluringviewer.getFontSizeofElements(thumbnail);
                Logger.Instance.InfoLog("********  thumbnailFontSmall:  " + thumbnailFonts[0]);

                //  if (thumbnailFonts.All(c1 => c1.Equals(thumbnailFontSmall)))

                string[] RecentStudythumbnailPreviewFontsSmall = new string[] { };
                //Step 19
                //Select any Thumbnail Preview for the prior study and verify the Series and Image information text font size in thumbnails should be Small in Size
                //Series and Image information text font size in thumbnails is in Small in Size.
                if (bluringviewer.RecentStudythumbnailpreviewIcons().Count == 0)
                {
                    Logger.Instance.ErrorLog("Unable to select the Thumbnail Preview of the Recent study, as there is no recent study");
                }
                else
                {
                    if (Config.BrowserType != "ie")
                        bluringviewer.RecentStudythumbnailpreviewIcons()[0].Click();
                    else
                        ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", bluringviewer.RecentStudythumbnailpreviewIcons()[0]);

                    BluRingViewer.WaitforThumbnails();

                    Thread.Sleep(5000);

                    if (!bluringviewer.ActiveRecentStudyThumbnailPreviewContainer().Displayed)
                    {
                        Logger.Instance.ErrorLog("Thumbnail preview is not displayed");
                    }
                    else
                    {
                        List<IWebElement> ThumbPreviewCaptions = new List<IWebElement>();
                        ThumbPreviewCaptions.AddRange(bluringviewer.RecentStudyActiveThumbnailCaption().ToList());
                        ThumbPreviewCaptions.AddRange(bluringviewer.RecentStudyActiveThumbnailImageFrameNumber().ToList());

                        RecentStudythumbnailPreviewFontsSmall = bluringviewer.getFontSizeofElements(ThumbPreviewCaptions);
                        bool boolRecentStudythumbnailPreviewFontsSmall = RecentStudythumbnailPreviewFontsSmall.All(c1 => c1.Equals(RecentStudyThumbnailFontsSmall));
                        Logger.Instance.InfoLog("********  RecentStudyThumbnailFontsSmall:  " + RecentStudythumbnailPreviewFontsSmall[0]);
                    }
                }


                bluringviewer.CloseBluRingViewer();
                login.Logout();

                Logger.Instance.InfoLog("******** thumbnailFontSmall = " + thumbnailFonts[0]);
                Logger.Instance.InfoLog("******** studyInformationAtStudyPanelSmall = { " + LogAllStudyDateTimeAtStudyPanel + ", " + LogAllStudyTimeAtStudyPanel + ", " + LogAllStudyInfoAtStudyPanel + " }; ");
                //Logger.Instance.InfoLog("******** GlobalToolBarItemFontSizeSmall = { " + PatinetNameFontSize + ", " + LogPatientDemoDetailList + ", " + LogPatientDemoDetailList + ", " + LogExamTextLable + ", " + LogShowHideToolName + ", " + LogExitButtonLable + " }");
                Logger.Instance.InfoLog("******** GlobalToolBarItemFontSizeSmall = { " + PatinetNameFontSize + ", " + LogPatientDemoDetailList + ", " + LogPatientDemoDetailList + ", " + "Blank" + ", " + "Blank" + ", " + " }");
                Logger.Instance.InfoLog("******** ExamListOperationContainerFontSmall = " + filterOperatorsFont[0]);
                Logger.Instance.InfoLog("******** ExamListLableSmall = " + LogExamListlableSmall);
                Logger.Instance.InfoLog("******** listContainerFontSmall = { " + RecentStudyAllDatesFonts[0] + "," + RecentStudyAllTimesFonts[0] + ", " + RecentStudyAllModalityFonts[0] + ", " + RecentStudyAllDescriptionFonts[0] + ", " + RecentStudyAllAccessionFonts[0] + "}");
                Logger.Instance.InfoLog("******** RecentStudyThumbnailFontsSmall = " + RecentStudythumbnailPreviewFontsSmall[0]);
                Logger.Instance.InfoLog("********  UserSettingsListFontSmall = " + LogUserControlToolsList);

                //Report Result

                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                //Return Result
                //------------End of script---

            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Logout
                login.Logout();

                //Return Result
            }
        }



    }
}
