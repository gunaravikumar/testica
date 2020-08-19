using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Reusable.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Dicom.Network;

namespace Selenium.Scripts.Tests
{
    class ThumbnailSplittingandOrdering
    {

        public Login login { get; set; }
        public string filepath { get; set; }
		public BluRingViewer viewer { get; set; }

		public string EA_91 = null;
        public string EA_131 = null;
        public string PACS_A7 = null;
        public string EA_77 = null;
        public string[] datasource = null;

		/// <summary>
		/// Constructor - Test Suite
		/// </summary>
		public ThumbnailSplittingandOrdering(String classname)
        {
            login = new Login();
			viewer = new BluRingViewer();
			filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            EA_91 = login.GetHostName(Config.EA91);
            EA_77 = login.GetHostName(Config.EA77);
            EA_131 = login.GetHostName(Config.EA1);
            PACS_A7 = login.GetHostName("10.5.38.28");
            datasource = new string[] { EA_131, EA_77, EA_91, PACS_A7 };
		}


        /// <summary> 
        /// This Test Case is verification of "Image Split Thumbnails"
        /// </summary>

        public TestCaseResult Test_161064(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;

                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNoList.Split(':');

                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String[] PatientID = PatientIDList.Split(':');

                BluRingViewer viewer = new BluRingViewer();
                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();


                //Step-1
                //Pre-conditions are completed
                String TestDomain1 = BasePage.GetUniqueDomainID("TestDomain1_135122_");
                String Role1 = BasePage.GetUniqueRole("Role1_135122_");

                Taskbar bar = new Taskbar();
                bar.Hide();
                //1 and 4
                st.LaunchServiceTool();
                st.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(2);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);

                string[] SeriesMod = { "CT", "MR", "NM", "PT", "RF" };
                var comboBox_mod = wpfobject.GetComboBox("ComboBox_Modality");
                int count = comboBox_mod.Items.Count;
                for (int i = 0; i < count; i++)
                {
                    comboBox_mod.Select(i);
                    if ((SeriesMod.Contains(comboBox_mod.SelectedItemText)))
                    {
                        if (wpfobject.IsRadioBtnSelected("RB_ThumbnailSplittingSeries"))
                        {
                            Logger.Instance.InfoLog("Modality " + comboBox_mod.SelectedItemText + "Series already selected");
                        }
                        else
                        {
                            wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingSeries");
                            Logger.Instance.InfoLog("Modality " + comboBox_mod.SelectedItemText + "Series selected now");
                        }
                    }
                    else
                    {
                        if (wpfobject.IsRadioBtnSelected("RB_ThumbnailSplittingName"))
                        {
                            Logger.Instance.InfoLog("Modality " + comboBox_mod.SelectedItemText + "Image already selected");
                        }
                        else
                        {
                            wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingName");
                            Logger.Instance.InfoLog("Modality " + comboBox_mod.SelectedItemText + "Image selected now");
                        }
                    }
                    wpfobject.WaitTillLoad();
                }

                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
				///Commented as it is not applicable for 7.0 Universal viewer
                //Thumbnail Captions
                //st.NavigateToTab("Viewer");
                //wpfobject.GetTabWpf(1).SelectTabPage(3);
                //wpfobject.ClickButton("Modify", 1);
                //var comboBoxDicomCaption = wpfobject.GetComboBox("ComboBox_DefaultThumbnailCaption");
                //comboBoxDicomCaption.Select("{S%SeriesNum%}{- %ImageNum%}");
                //wpfobject.WaitTillLoad();
                //wpfobject.ClickButton("Apply", 1);
                //wpfobject.WaitTillLoad();
                st.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();
                bar.Show();
                //2 and 3
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(TestDomain1, Role1, datasources: datasource);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(10);
                domain.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(30);
                login.Logout();
                ExecutedSteps++;

                //step-2-
                //Login to WebAccess client (you can use admin1/admin1 user)
                login.DriverGoTo(login.url);
                login.LoginIConnect(TestDomain1, TestDomain1);
                ExecutedSteps++;

                //step-3
                //change the thumbnail splitting for CR to be Image in User Preference. and load study
                Studies study = (Studies)login.Navigate("Studies");
                study.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                domain.ModalityDropDown().SelectByText("CR");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                PageLoadWait.WaitForPageLoad(20);
                study.CloseUserPreferences();
                //AccessionNo: 29822041 //Last Name: Woodall
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_91);
                PageLoadWait.WaitForPageLoad(20);
                study.SelectStudy("Accession", AccessionNumbers[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();

                IList<IWebElement> Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                if (Thumbnail_list.Count == 5 &&
                    Thumbnail_list[0].Displayed &&
                    Thumbnail_list[1].Displayed &&
                    Thumbnail_list[2].Displayed &&
                    Thumbnail_list[3].Displayed &&
                    Thumbnail_list[4].Displayed)
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
                //From the Exam List, click on the thumbnail icon of the current study to display the exam list thumbnails.
                var ExamlistThumbnailIcon = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_thumbnailIcon);
                ExamlistThumbnailIcon.Click();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ExamList_thumbnails))));
                IList<IWebElement> ExamList_Thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));

                //Exam List Thumbnails
                if (ExamList_Thumbnails.Count == 5 &&
                    ExamList_Thumbnails[0].Displayed &&
                    ExamList_Thumbnails[1].Displayed &&
                    ExamList_Thumbnails[2].Displayed &&
                    ExamList_Thumbnails[3].Displayed &&
                    ExamList_Thumbnails[4].Displayed)
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
                //Step-5
                //Double click on each of the thumbnails in the study panel
                viewer.SetViewPort(0, 1);
                IWebElement Viewport = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                Viewport.Click();
                Thread.Sleep(2000);
                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                var action = new Actions(BasePage.Driver);

                action.DoubleClick(Thumbnail_list[2]).Build().Perform();
                Thread.Sleep(5000);
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();

                //The active study viewport should be updated to display the selected thumbnails
                result.steps[++ExecutedSteps].SetPath(testid + "_5_1", ExecutedSteps + 1);
                bool status5_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

				action = new Actions(BasePage.Driver);
				action.DoubleClick(Thumbnail_list[4]).Build().Perform();
                Thread.Sleep(5000);
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();

                result.steps[ExecutedSteps].SetPath(testid + "_5_2", ExecutedSteps + 1);
                bool status5_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status5_1 && status5_2)
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

                //step-6
                //Visually check the thumbnail captionZ: "S"SeriesNumber "-"ImageNumber
                IList<IWebElement> ThumbnailPercentImagesViewed = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailcontainer + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
                IList<IWebElement> Image_FrameNumber = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailcontainer + " " + BluRingViewer.div_imageFrameNumber));
                IList<IWebElement> Thumbnail_Caption = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailcontainer + " " + BluRingViewer.div_thumbnailCaption));
				IList<IWebElement> Thumbnail_Modality = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailcontainer + " " + BluRingViewer.div_thumbnailModality));
				bool step_6 = true;
                for (int i = 0; i < 5; i++)
                {
                    if (ThumbnailPercentImagesViewed[i].Text.Equals("100%") && Image_FrameNumber[i].Text.Equals("1") &&
                        Thumbnail_Caption[i].Text.Equals("S1- " + (i + 1)) && !Thumbnail_Modality[i].Text.Equals(""))
                        step_6 = true;
                    else
                    {
                        step_6 = false;
                        break;
                    }
                }
                if (step_6)
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
                viewer.CloseBluRingViewer();

                //Step-7
                //BEEEA027P0F886P (48 images- XA Modality)
                //patientID : 9cd74b9bp16328p
                study.SearchStudy(patientID: PatientID[0], Datasource: PACS_A7);
                study.SelectStudy("Patient ID", PatientID[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();

                //1Default thumbnail (SeriesNumber +"-"+ ImageNumber) =>"S1-300"
                //2. If the thumbnail has corresponding image in the Viewport, Top left has percentage viewed, and blank otherwise.
                //3. Top right has "1" (only one image available). For image-splitting thumbnails, this value will always be 
                //4. The caption is displayed as overlay on the thumbnail image; 

                String[] caption_text = { "S1- 300", "S1- 301", "S1- 302", "S1- 303", "S1- 304", "S1- 305", 
                                          "S1- 306", "S1- 307", "S1- 308", "S1- 309", "S1- 310", "S1- 311",  //s1---12
                                          "S2- 300", "S2- 301", "S2- 302", "S2- 303",  //s2---4
                                          "S5- 300", "S5- 301", "S5- 302", "S5- 303",  //s3---4
                                          "S9- 1", "S9- 2", "S9- 3", "S9- 4", "S9- 5", "S9- 6", "S9- 7", "S9- 8", "S9- 9", "S9- 10",  //s9---10
                                          "S11- 1", "S11- 2", "S11- 3", "S11- 4", "S11- 5", "S11- 6", "S11- 7", "S11- 8", "S11- 9", "S11- 10",  //s11---10
                                          "S12- 1", "S12- 2", "S12- 3", "S12- 4", "S12- 5", "S12- 6", "S12- 7", "S12- 8"  //s12---8
                                        };

                ThumbnailPercentImagesViewed = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailPercentImagesViewed));
                Image_FrameNumber = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_imageFrameNumber));
                Thumbnail_Caption = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailCaption));
                bool step_7 = true;
                for (int i = 0; i < 48; i++)
                {
                    if (Image_FrameNumber[i].GetAttribute("innerText").Equals("1") &&
                        Thumbnail_Caption[i].GetAttribute("innerText").Equals(caption_text[i]))
                        step_7 = true;
                    else
                    {
                        step_7 = false;
                        break;
                    }

                    if (i < 6) //visible viewport
                    {
                        if (ThumbnailPercentImagesViewed[i].Text.Equals("100%"))
                            step_7 = true;
                        else
                        {
                            step_7 = false;
                            break;
                        }
                    }
                }
                if (step_7 && ThumbnailPercentImagesViewed.Count == 6 &&
                    Thumbnail_Caption.Count == 48)
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

                //step-8
                //In Exam List, click on the thumbnail icon of the current study 
                //Each image should be displayed in its own thumbnail; should be 48 thumbnails.

                ExamlistThumbnailIcon = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_thumbnailIcon);
                ExamlistThumbnailIcon.Click();
                BluRingViewer.WaitforThumbnails(300);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ExamList_thumbnails))));
                ExamList_Thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));
                //there should be 48 thumbnails
                if (ExamList_Thumbnails.Count == 48 &&
                    ExamList_Thumbnails[0].Displayed && ExamList_Thumbnails[1].Displayed &&
                    ExamList_Thumbnails[2].Displayed && ExamList_Thumbnails[3].Displayed &&
                    ExamList_Thumbnails[4].Displayed && ExamList_Thumbnails[5].Displayed &&
                    ExamList_Thumbnails[6].Displayed && ExamList_Thumbnails[7].Displayed &&
                    ExamList_Thumbnails[8].Displayed)
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

                //Step-9
                //Drag and drop various thumbnail(from the Exam List Thumbnail preview area) into a viewport
                ExamList_Thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));
                viewer.SetViewPort(5, 1);

                Viewport = BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport));
                Viewport.Click();
                Thread.Sleep(2000);
                Actions action9 = new Actions(BasePage.Driver);

                action9.DoubleClick(ExamList_Thumbnails[7]).Build().Perform();
                Thread.Sleep(5000);
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();

                //viewport should update to display the image that is represented by the selected thumbnail. 
                //The viewport and thumbnail is highlighted (thick blue border) and % viewed should be 100%

                IList<IWebElement> Thumbnail_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailcontainer + " " + BluRingViewer.div_ThumbnailOuter));
                ThumbnailPercentImagesViewed = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailPercentImagesViewed));
                viewer.SetViewPort(5, 1);
				if (BasePage.Driver.FindElement(By.CssSelector(viewer.Activeviewport)).GetAttribute("class").Contains("activeViewportDiv") &&
					BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 8) &&
                    ThumbnailPercentImagesViewed[7].Text.Equals("100%"))
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
                viewer.CloseBluRingViewer();

                //Logout
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
        /// Service Tool, Domain, User Pref - thumbnail-split configuration
        /// </summary>

        public TestCaseResult Test_161066(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            DomainManagement domain = new DomainManagement();
            UserPreferences userpref = new UserPreferences();
            Studies study = new Studies();
            int ExecutedSteps = -1;
            ServiceTool st = new ServiceTool();
            WpfObjects wpfobject = new WpfObjects();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;

                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNoList.Split(':');

                //Pre-condition
                st.LaunchServiceTool();
                st.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(2);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                st.SelectDropdown("ComboBox_Modality", "CR");
                wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingName");
                wpfobject.ClickRadioButton("Image", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();

                //Step-1
                //Login to WebAccess instance as Administrator.
                //Set TestDomain1 with the thumbnail splitting set for Series for CR modality. Save Domain.

                //Pre-conditions are completed
                String TestDomain1 = BasePage.GetUniqueDomainID("TestDomain_135124_");
                String Role1 = BasePage.GetUniqueRole("Role_135124_");

                String ThumbnailDomain = BasePage.GetUniqueDomainID("ThumbnailDomain_135124_");
                String ThumbnailRole1 = BasePage.GetUniqueRole("ThumbnailRole1_135124_");

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(TestDomain1, Role1, datasources: datasource);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(10);
                domain.ModalityDropDown().SelectByText("CR");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                PageLoadWait.WaitForPageLoad(30);
                domain.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(30);
                login.Logout();
                ExecutedSteps++;

                //step-2-
                //Login to WebAccess as admin1 user. (this user is in TestDomain1)
                login.DriverGoTo(login.url);
                login.LoginIConnect(TestDomain1, TestDomain1);
                ExecutedSteps++;

                //step-3
                //Go to the User Preferences panel for the current admin1 user.
                //Ensure that in the 'Default Settings per Modality' panel, the Thumbnail Splitting setting to 'Series' for CR modality.

                study = (Studies)login.Navigate("Studies");

                study.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                domain.ModalityDropDown().SelectByText("CR");
                //User Preferences have thumbnail splitting for CR as "Series".
                if (domain.SelectedValueOfRadioBtn("ThumbSplitRadioButtons").ToLower().Equals("series"))
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
				userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);


                //Step-4
                //Find the study for "PORTABLE, BETTY" and load the study.
                //Portable Betty  Acc:10717882
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumbers[0]);
                BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforThumbnails(100);
                BluRingViewer.WaitforViewports();
				//Thumbnails are ordered based on series_number and Image_number, so 
				//The image# 82632000 should be the first thumbnail, the image# 82633000 should be the second thumbnail.

                IList<IWebElement> Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
				IList<IWebElement> Thumbnail_Caption = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailcontainer + " " + BluRingViewer.div_thumbnailCaption));

				//result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
				//bool status4 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

				if (Thumbnail_Caption[0].Text == "S0- 82632000" && Thumbnail_Caption[1].Text == "S0- 82633000" &&
					Thumbnail_list.Count == 2)
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
                viewer.CloseBluRingViewer();

                //Step-5
                //Find and load study "Burton Cliff", patient ID ~ 852654.
                //AC8526354 (BURTON, CLIFF) (852654)
                study.SearchStudy(AccessionNo: AccessionNumbers[1], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumbers[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();

                //there will be one thumbnail for each series displayed in the study thumbnail bar.
                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                if (Thumbnail_list.Count == 1)
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
                viewer.CloseBluRingViewer();
                login.Logout();
                //step-6
                //Login as Administrator and create ThumbnailDomain.
                //Add the data source, ThumbnailRole (role), ThumbnailUser (admin user) for this domain and role.

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(ThumbnailDomain, ThumbnailDomain, datasources: datasource);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(10);
                domain.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(30);
                domain.SearchDomain(ThumbnailDomain);
                if (domain.DomainExists(ThumbnailDomain))
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
                //Specifically, check that for CR- layout=auto, ThumbnailSplitting =Image.
                //ThumbnailUser preferences are the same as the Service tool's CR default- layout~auto, ThumbnailSplitting~Image.

                domain.SearchDomain(ThumbnailDomain);
                domain.SelectDomain(ThumbnailDomain);
                domain.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(30);
                domain.ModalityDropDown().SelectByText("CR");
                PageLoadWait.WaitForPageLoad(30);
                if (domain.LayoutDropDown().SelectedOption.Text.Equals("auto") &&
                   domain.SelectedValueOfRadioBtn("ThumbSplitRadioButtons").ToLower().Equals("image"))
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
                domain.ClickCloseEditDomain();
                login.Logout();
                //step-8
                //Login to WebAccess with user ThumbnailUser/ThumbnailUser 
                //Load study Burton Cliff, patient ID ~ 852654.

                login.DriverGoTo(login.url);
                login.LoginIConnect(ThumbnailDomain, ThumbnailDomain);
                study = (Studies)login.Navigate("Studies");
                //AC8526354 (BURTON, CLIFF) (852654)
                study.SearchStudy(AccessionNo: AccessionNumbers[1], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumbers[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();

                //there will be 1 thumbnail for each PR series & 1 thumbnail for each CR image displayed(4 thumbnails in this case).
                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                if (Thumbnail_list.Count == 4)
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
                viewer.CloseBluRingViewer();

                //step-9
                //Modify the user preferences of ThumbnailUser to set Thumbnail splitting of CR to Series.
                //Load study Burton Cliff, patient ID ~ 852654. Verify the thumbnails.
                //Verify the thumbnail splitting preferences in the ThumbnailDomain - ensure that CR is still Image thumbnail split.

                userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                domain.ModalityDropDown().SelectByText("CR");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                //AC8526354 (BURTON, CLIFF) (852654)
                study.SearchStudy(AccessionNo: AccessionNumbers[1], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumbers[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                //there will be one thumbnail for each series displayed in the study thumbnail bar.
                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                int count = Thumbnail_list.Count;

                viewer.CloseBluRingViewer();
                domain = (DomainManagement)login.Navigate("DomainManagement");

                domain.ModalityDropDown().SelectByText("CR");
                if (count == 1 &&
                    domain.SelectedValueOfRadioBtn("ThumbSplitRadioButtons").Equals("Image"))
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

                //step-10
                //"Revert user preferences of ThumbnailUser to set Thumbnail splitting of CR back to Image.
                //Modify ThumbnailDomain to set Thumbnail splitting of CR to Series. Save domain.
                //Verify that ThumbnailUser's preferences of Thumbnail splitting of CR is set to Image.
                //Load study Burton Cliff, patient ID ~ 852654. Verify the thumbnails."

                domain.ModalityDropDown().SelectByText("CR");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                domain.ClickSaveEditDomain();
                domain.ClickCloseEditDomain();
                study = (Studies)login.Navigate("Studies");
                userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                domain.ModalityDropDown().SelectByText("CR");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);

                //AC8526354 (BURTON, CLIFF) (852654)
                study.SearchStudy(AccessionNo: AccessionNumbers[1], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumbers[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();

                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                count = Thumbnail_list.Count;

                //"1. ThumbnailUser's preferences has CR thumbnail splitting back to Image.
                //2. ThumbnailDomain's preferenes has CR thumbnail splitting to Series.
                //3. Verified that Thumbnail User's preferences for CR is still Image.
                //4. The Burton, Cliff study is loaded into the viewer.
                //Since the thumbnail splitting for this user is set to Images, there will be one thumbnail for each PR series
                //and one thumbnail for each CR image displayed in the study thumbnail bar (4 thumbnails in this case).
                //Note- After the User preferences has been modified,the Domain's preferences do not affect the user's preference anymore 
                //(User preferences only take from the Domain's preference initially).
                //Also, User preferences is the final preference to be used during study display."
                viewer.CloseBluRingViewer();
                userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                domain.ModalityDropDown().SelectByText("CR");
                bool userpreferencesetting = domain.SelectedValueOfRadioBtn("ThumbSplitRadioButtons").Equals("Image");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.ModalityDropDown().SelectByText("CR");
                bool domainsetting = domain.SelectedValueOfRadioBtn("ThumbSplitRadioButtons").Equals("Series");
                if (count == 4 && userpreferencesetting &&
                    domainsetting)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.ErrorLog("count--" + count);
                    Logger.Instance.ErrorLog("userpreferencesetting--" + userpreferencesetting);
                    Logger.Instance.ErrorLog("domainsetting--" + domainsetting);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-11
                //Revert user preferences of ThumbnailDomain to set Thumbnail splitting of CR back to Image.
                //Go to Service Tool > Viewer > Protocols and set CR's Thumbnail Splitting to Auto, then reset IIS.
                //After the web application restarts, login as ThumbnailUser. 
                //Verify Thumbnail Splitting for CR in ThumbnailDomain and in the ThumbnailUser preferences."
                domain.ModalityDropDown().SelectByText("CR");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                domain.ClickSaveEditDomain();
                domain.ClickCloseEditDomain();
                login.Logout();

                st.LaunchServiceTool();
                st.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(2);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                st.SelectDropdown("ComboBox_Modality", "CR");
                wpfobject.ClickRadioButton("Auto", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();

                //1. ThumbnailDomain's preferenes has CR thumbnail splitting to Image.
                //After IIS reset, log into the application as ThumbnailUser- 
                //ThumbnailDomain AND ThumbnailUser has thumbnail splitting for CR as Image.

                login.DriverGoTo(login.url);
                login.LoginIConnect(ThumbnailDomain, ThumbnailDomain);
                study = (Studies)login.Navigate("Studies");
                userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                domain.ModalityDropDown().SelectByText("CR");
                bool userpreferencesetting_11 = domain.SelectedValueOfRadioBtn("ThumbSplitRadioButtons").Equals("Image");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CancelPreferenceBtn().Click();
                PageLoadWait.WaitForPageLoad(20);
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.ModalityDropDown().SelectByText("CR");
                bool domainsetting_11 = domain.SelectedValueOfRadioBtn("ThumbSplitRadioButtons").Equals("Image");
                if (userpreferencesetting_11 && domainsetting_11)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.ErrorLog("userpreferencesetting_11--" + userpreferencesetting_11);
                    Logger.Instance.ErrorLog("domainsetting_11--" + domainsetting_11);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.Logout();

                //step-12
                //Reset all domain configuration back to default-
                //Login to Administrator-Set TestDomain1 & ThumbnailDomain with the thumbnail splitting set for Image for CR modality.
                //Then logout of Administrator account.

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                domain = (DomainManagement)login.Navigate("DomainManagement");

                domain.SearchDomain(TestDomain1);
                domain.SelectDomain(TestDomain1);
                domain.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(30);
                domain.ModalityDropDown().SelectByText("CR");
                PageLoadWait.WaitForPageLoad(30);
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                domain.ClickSaveDomain();

                domain.SearchDomain(ThumbnailDomain);
                domain.SelectDomain(ThumbnailDomain);
                domain.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(30);
                domain.ModalityDropDown().SelectByText("CR");
                PageLoadWait.WaitForPageLoad(30);
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                domain.ClickSaveDomain();
                login.Logout();
                ExecutedSteps++;

                //step-13
                //Reset all user preferences back to default-
                //Login as admin1, go to User Preference & in 'Default Settings Per Modality' section change the Thumbnail Splitting from Series to Image, for CR modality. Save the preference.
                //Login as ThumbnailUser, go to User Preference and in 'Default Settings Per Modality' ensure the Thumbnail Splitting is set to Image, for CR modality. 
                //Go to Service Tool > Viewer > Protocols and set CR's Thumbnail Splitting to Image, then reset IIS.
                login.DriverGoTo(login.url);
                login.LoginIConnect(TestDomain1, TestDomain1);
                study = (Studies)login.Navigate("Studies");
                userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                domain.ModalityDropDown().SelectByText("CR");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                login.Logout();

                login.DriverGoTo(login.url);
                login.LoginIConnect(ThumbnailDomain, ThumbnailDomain);
                study = (Studies)login.Navigate("Studies");
                userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                domain.ModalityDropDown().SelectByText("CR");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                login.Logout();

                st.LaunchServiceTool();
                st.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(2);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                st.SelectDropdown("ComboBox_Modality", "CR");
                wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingName");
                wpfobject.ClickRadioButton("Image", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
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
                st.LaunchServiceTool();
                st.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(2);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                st.SelectDropdown("ComboBox_Modality", "CR");
                wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingName");
                wpfobject.ClickRadioButton("Image", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
            }
        }


        /// <summary> 
        /// Auto Thumbnail Spliting
        /// </summary>

        public TestCaseResult Test_161067(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;

                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNoList.Split(':');

                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String[] PatientID = PatientIDList.Split(':');

                //Step-1
                //Pre-conditions are completed
                ExecutedSteps++;

                //step-2
                //Go to the BlueRing machine and open the Service tool > Viewer > Protocols page. 
                //Verify the following default settings for the modalities listsed-
                //Modal Layout Thumb ---Viewer ----Exam
                //CT---- 2x2-- series---series -----Off
                //XA ----Auto--- image-- series -----Off
                //MG ---1x2----- image---series------Off
                //US----Auto--- Image----series------Off 
                //Leave the other settings to default

                string[] Modality = { "CT", "XA", "MG", "US" };
                string[] Layout = { "2x2", "auto", "1x2", "auto" };
                string[] Thumbnail = { "Series", "Image", "Image", "Image" };
                string[] ViewerScope = { "Series", "Series", "Series", "Series" };
                string[] ExamMode = { "Off", "Off", "Off", "Off" };

                String TestDomain1 = BasePage.GetUniqueDomainID("TestDomain1_135126_");
                String Role1 = BasePage.GetUniqueRole("Role1_135126_");

                Studies study = new Studies();
                UserPreferences userprf = new UserPreferences();
                DomainManagement domain = new DomainManagement();
                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                BluRingViewer viewer = new BluRingViewer();

                st.LaunchServiceTool();
                st.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(2);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);

                for (int i = 0; i < 4; i++)
                {
                    st.SelectDropdown("ComboBox_Modality", Modality[i]);
                    st.SelectDropdown("ComboBox_Layout", Layout[i]);
                    if (Thumbnail[i].Equals("Series"))
                        wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingSeries");
                    else
                        wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingName");

                    wpfobject.ClickRadioButtonById("RB_ViewingScope" + ViewerScope[i]);
                    wpfobject.ClickRadioButtonById("RB_ExamMode" + ExamMode[i]);

                    wpfobject.WaitTillLoad();
                }
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                st.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(TestDomain1, Role1, datasources: datasource);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(10);
                domain.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(30);
                login.Logout();
                ExecutedSteps++;

                //step-3
                //Login to admin1/admin1 user account and navigate to the Domain Management tab (ensure the default setting), 

                login.DriverGoTo(login.url);
                login.LoginIConnect(TestDomain1, TestDomain1);
                domain = (DomainManagement)login.Navigate("DomainManagement");

                Boolean res3 = true;
                String ThumbnailSplit_id = "ThumbSplitRadioButtons";
                String ViewingScope_id = "ScopeRadioButtons";
                String ExamMode_id = "ExamModeRadioButtons";

                for (int i = 0; i < 4; i++)
                {
                    domain.ModalityDropDown().SelectByText(Modality[i]);

                    if (domain.LayoutDropDown().SelectedOption.Text.Equals(Layout[i], StringComparison.CurrentCultureIgnoreCase)
                        && domain.SelectedValueOfRadioBtn(ThumbnailSplit_id).Equals(Thumbnail[i], StringComparison.CurrentCultureIgnoreCase)
                        && domain.SelectedValueOfRadioBtn(ViewingScope_id).Equals(ViewerScope[i], StringComparison.CurrentCultureIgnoreCase)
                        && domain.SelectedValueOfRadioBtn(ExamMode_id).Equals(ExamMode[i], StringComparison.CurrentCultureIgnoreCase))
                    {
                        Logger.Instance.InfoLog("Modality: " + Modality[i] + "=> Layout: " + Layout[i] + "  Thumbnail : " + Thumbnail[i] +
                             "Viewing_Scope: " + ViewerScope[i] + " ExamMode: " + ExamMode[i] + "  -Verified Successfully");
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("Modality: " + Modality[i] + "=> Layout: " + Layout[i] + "  Thumbnail : " + Thumbnail[i] +
                            "Viewing_Scope: " + ViewerScope[i] + " ExamMode: " + ExamMode[i] + "  -Verified failed");
                        res3 = false;
                        break;
                    }
                }
                if (res3)
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
                //domain.ClickCloseEditDomain();

                //Step-4
                //Open the User Preferences for admin1 & verify/change the protocol setting to the same as described in the previous step
                Boolean res4 = true;
                study = (Studies)login.Navigate("Studies");
                userprf = login.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");

                for (int i = 0; i < 4; i++)
                {
                    domain.ModalityDropDown().SelectByText(Modality[i]);

                    if (domain.LayoutDropDown().SelectedOption.Text.Equals(Layout[i], StringComparison.CurrentCultureIgnoreCase)
                        && domain.SelectedValueOfRadioBtn(ThumbnailSplit_id).Equals(Thumbnail[i], StringComparison.CurrentCultureIgnoreCase)
                        && domain.SelectedValueOfRadioBtn(ViewingScope_id).Equals(ViewerScope[i], StringComparison.CurrentCultureIgnoreCase)
                        && domain.SelectedValueOfRadioBtn(ExamMode_id).Equals(ExamMode[i], StringComparison.CurrentCultureIgnoreCase))
                    {
                        Logger.Instance.InfoLog("Modality: " + Modality[i] + "=> Layout: " + Layout[i] + "  Thumbnail : " + Thumbnail[i] +
                             "Viewing_Scope: " + ViewerScope[i] + " ExamMode: " + ExamMode[i] + "  -Verified Successfully");
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("Modality: " + Modality[i] + "=> Layout: " + Layout[i] + "  Thumbnail : " + Thumbnail[i] +
                            "Viewing_Scope: " + ViewerScope[i] + " ExamMode: " + ExamMode[i] + "  -Verified failed");
                        res4 = false;
                        break;
                    }
                }
                if (res4)
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
				userprf.CloseUserPreferences();

                //step-5
                //Load CT Study, PatientName~8114BB, ( PID~16E2AC) Accession ~7f7ed13ae
                //8 series 3, 38, 38, 80, 28, 28, 38, 80
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: PACS_A7);
                study.SelectStudy("Accession", AccessionNumbers[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();
                String[] Imagecount = { "3", "38", "38", "80", "28", "28", "38", "80" };
                IList<IWebElement> Thumbnail_ImageNumber = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_imageFrameNumber));
                bool res5 = true;
                for (int i = 0; i < Imagecount.Length; i++)
                {
                    if (Thumbnail_ImageNumber[i].Text.Contains(Imagecount[i]))
                    {
                        Logger.Instance.InfoLog("Thumbnail Caption " + (i + 1) + ":" + Thumbnail_ImageNumber[i].Text + "=> Image count: " + Imagecount[i] + "  -Verified successfully");
                    }
                    else
                    {
                        res5 = false;
                        Logger.Instance.ErrorLog("Thumbnail Caption " + (i + 1) + ":" + Thumbnail_ImageNumber[i].Text + "=> Image count: " + Imagecount[i] + "  - not matching -Verified failed");
                        break;
                    }
                }

                if (Thumbnail_ImageNumber.Count == 8 && res5)
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
                viewer.CloseBluRingViewer();

                //step-6
                //Load XA Study- PatientName~XA-Bargus, U  2 Multiframe series-
                //Series#1, Image#2, 114 Frames
                //Series#1, Image#7, 132 Frames //PID_12514
                study.SearchStudy(patientID: PatientID[0], Datasource: PACS_A7);
                study.SelectStudy("Patient ID", PatientID[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();
                //2 thumbnails are displayed for the two Multi-frame images-
                //Series#1, Image#2, 114 Frames //Series#1, Image#7, 132 Frames

                IList<IWebElement> Image_FrameNumber = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_imageFrameNumber));
                IList<IWebElement> Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                IList<IWebElement> Thumbnail_Caption = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailCaption));

                if (Thumbnail_list.Count == 2 &&
                    Thumbnail_Caption[0].Text.Contains("S1- 2") &&
                    Thumbnail_Caption[1].Text.Contains("S1- 7") &&
                    Image_FrameNumber[0].Text.Equals("114") &&
                    Image_FrameNumber[1].Text.Equals("132"))
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
                viewer.CloseBluRingViewer();

                //step-7
                //Load the US study - PatientName~YSJ-US100 (PID = ysj-222100)
                //16 single image (in series instance ..249)
                //30 Multi-frame images (17 to 47) (in series instance ..249)
                //1 single image (in series instance ..248)

                //--- Series instance pending***
                study.SearchStudy(patientID: PatientID[1], Datasource: PACS_A7);
                study.SelectStudy("Patient ID", PatientID[1]);
                viewer = BluRingViewer.LaunchBluRingViewer(ThumbnailTimeout: 900);
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();

                Image_FrameNumber = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_imageFrameNumber));

                bool res7_1 = true;
                bool res7_2 = true;
                bool res7_3 = true;
                for (int i = 0; i < 16; i++)
                {
                    if (Image_FrameNumber[i].Text.Equals(""))
                    {
                        BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ThumbnailNextArrowButton)).Click();
                    }
                    if (Image_FrameNumber[i].Text.Equals("1"))
                    {
                        Logger.Instance.InfoLog("Image_FrameNumber " + (i + 1) + ":" + Image_FrameNumber[i].Text);
                    }
                    else
                    {
                        res7_1 = false;
                        Logger.Instance.ErrorLog("Image_FrameNumber " + (i + 1) + ":" + Image_FrameNumber[i].Text);
                        break;
                    }
                }

                for (int i = 16; i < 46; i++)
                {
                    if (Image_FrameNumber[i].Text.Equals(""))
                    {
                        BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ThumbnailNextArrowButton)).Click();
                    }
                    if (!Image_FrameNumber[i].Text.Equals("1"))
                    {
                        Logger.Instance.InfoLog("Image_FrameNumber " + (i + 1) + ":" + Image_FrameNumber[i].Text);
                    }
                    else
                    {
                        res7_2 = false;
                        Logger.Instance.ErrorLog("Image_FrameNumber " + (i + 1) + ":" + Image_FrameNumber[i].Text);
                        break;
                    }
                }
                if (Image_FrameNumber[46].Text.Equals(""))
                {
                    BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ThumbnailNextArrowButton)).Click();
                }
                if (Image_FrameNumber[46].Text.Equals("1"))
                {
                    Logger.Instance.InfoLog("Image_FrameNumber " + (46 + 1) + ":" + Image_FrameNumber[46].Text);
                }
                else
                {
                    res7_3 = false;
                    Logger.Instance.ErrorLog("Image_FrameNumber " + (46 + 1) + ":" + Image_FrameNumber[46].Text);
                }
                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                if (res7_1 && res7_2 && res7_3 && Thumbnail_list.Count == 47)
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
                viewer.CloseBluRingViewer();

                //step-8
                //Load MG Study- PatientName~BAL-MG10 (ID~ALMG100010) Accession = 2205587  
                //10 images in two series 14626 (5 images), 14627 (5 images)

                study.SearchStudy(AccessionNo: AccessionNumbers[1], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumbers[1]);
                viewer = BluRingViewer.LaunchBluRingViewer(ThumbnailTimeout: 600);

                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));

                //In Image-split mode, the viewer displays 10 thumbnails, one for each image.
                if (Thumbnail_list.Count == 10)
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
                viewer.CloseBluRingViewer();

                //step-9
                //Login to WebAccess as admin1/admin1 and navigate to the TestDomain1 Domain and 
                //change the Thumbnail Splitting setting for Modality CT, XA , MG and US to AUTO, the Viewing scope ~ Series , leave the other settings to default.

                domain = (DomainManagement)login.Navigate("DomainManagement");
                string[] Mod = { "CT", "US", "XA", "MG" };
                for (int i = 0; i < Mod.Length; i++)
                {
                    domain.ModalityDropDown().SelectByText(Mod[i]);
                    domain.SelectRadioBtn("ThumbSplitRadioButtons", "Auto");
                    domain.SelectRadioBtn("ScopeRadioButtons", "Series");
                }
                domain.ClickSaveEditDomain(); //Save option not working--***
                PageLoadWait.WaitForPageLoad(30);
                domain.ClickCloseEditDomain();
                ExecutedSteps++;

                //Step-10
                //Set User Preferences for admin1 to have the same setting as the previous step- 
                //ensure/change the Thumbnail Splitting setting for Modality CT, XA , MG and US to AUTO, the Viewing scope ~ Series , leave the other settings to default.

                string[] Modality_10 = { "CT", "XA", "MG", "US" };
                string[] Layout_10 = { "2x2", "auto", "1x2", "auto" };
                string[] Thumbnail_10 = { "Auto", "Auto", "Auto", "Auto" }; //**
                string[] ViewerScope_10 = { "Series", "Series", "Series", "Series" };
                string[] ExamMode_10 = { "Off", "Off", "Off", "Off" };

                Boolean res10 = true;
                study = (Studies)login.Navigate("Studies");
                userprf = login.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");

                for (int i = 0; i < 4; i++)
                {
                    domain.ModalityDropDown().SelectByText(Modality_10[i]);

                    if (domain.LayoutDropDown().SelectedOption.Text.Equals(Layout_10[i], StringComparison.CurrentCultureIgnoreCase)
                        && domain.SelectedValueOfRadioBtn(ThumbnailSplit_id).Equals(Thumbnail_10[i], StringComparison.CurrentCultureIgnoreCase)
                        && domain.SelectedValueOfRadioBtn(ViewingScope_id).Equals(ViewerScope_10[i], StringComparison.CurrentCultureIgnoreCase)
                        && domain.SelectedValueOfRadioBtn(ExamMode_id).Equals(ExamMode_10[i], StringComparison.CurrentCultureIgnoreCase))
                    {
                        Logger.Instance.InfoLog("Modality: " + Modality_10[i] + "=> Layout: " + Layout_10[i] + "  Thumbnail : " + Thumbnail_10[i] +
                             "Viewing_Scope: " + ViewerScope_10[i] + " ExamMode: " + ExamMode_10[i] + "  -Verified Successfully");
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("Modality: " + Modality_10[i] + "=> Layout: " + Layout_10[i] + "  Thumbnail : " + Thumbnail_10[i] +
                            "Viewing_Scope: " + ViewerScope_10[i] + " ExamMode: " + ExamMode_10[i] + "  -Verified failed Changing values");
						//res10 = false;
						//break;						
						domain.SelectRadioBtn("ThumbSplitRadioButtons", Thumbnail_10[i]);
						PageLoadWait.WaitForPageLoad(20);
					}
                }

                if (res10)
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
                userprf.CloseUserPreferences();

				///fix for application session timeout
				login.Logout();
				login.CreateNewSesion();
				login.DriverGoTo(login.url);
				login.LoginIConnect(TestDomain1, TestDomain1);
				study = (Studies)login.Navigate("Studies");

				//step-11
				//Load CT Study, PatientName~8114BB, ( PID= 16E2AC). Review the thumbnails in BlueRing viewer.
				//ASSession : 7f7ed13ae)
				study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumbers[0]);
                viewer = BluRingViewer.LaunchBluRingViewer(ThumbnailTimeout: 900);
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();

                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));

                if (Thumbnail_list.Count == 8)
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
                viewer.CloseBluRingViewer();

				//step-12
				//Load the US Study- YSJ-US100 (ysj-222100) eleven-teen  
				study.SearchStudy(patientID: PatientID[1], Datasource: PACS_A7);
                study.SelectStudy("Patient ID", PatientID[1]);
                viewer = BluRingViewer.LaunchBluRingViewer(ThumbnailTimeout: 900);
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();
                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));

                //Therefore, 32 thumbnails are displayed- first thumbnail should be series#1 with all single-frame images (16 images within this thumbnail). 2nd .. 31st thumbnails should be multi-frame images. Last thumbnail should be single-framed image (with different series instance than the first series)

                if (Thumbnail_list.Count == 32)
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
                viewer.CloseBluRingViewer();

                //step-13
                //Load the XA study- patientName~ XA-Bargus //PID_12514

                study.SearchStudy(patientID: PatientID[0], Datasource: PACS_A7);
                study.SelectStudy("Patient ID", PatientID[0]);
                viewer = BluRingViewer.LaunchBluRingViewer(ThumbnailTimeout: 600);
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();
                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));

                //Therefore, the viewer displays two thumbnails- one for each multi-frame image in separate series.
                if (Thumbnail_list.Count == 2)
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

                viewer.CloseBluRingViewer();
                //step-14
                //Load MG Study- BAL-MG10 (ALMG100010) Accession =2205587 

                study.SearchStudy(AccessionNo: AccessionNumbers[1], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumbers[1]);
                viewer = BluRingViewer.LaunchBluRingViewer(ThumbnailTimeout: 600);
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();
                //Thumbnail auto-split mode is enabled; the two series 14626, 14627 are put together to display 2 thumbnails one for each series. Each series has 5 image.

                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                Thumbnail_Caption = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailCaption));
                Image_FrameNumber = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_imageFrameNumber));

                if (Thumbnail_list.Count == 2 &&
                    Thumbnail_Caption[0].Text.Equals("S14626- 1") &&
                    Thumbnail_Caption[1].Text.Equals("S14627- 1") &&
                    Image_FrameNumber[0].Text.Equals("5") &&
                    Image_FrameNumber[1].Text.Equals("5"))
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
                viewer.CloseBluRingViewer();

                //step-15
                //Reset configuration back to default-
                //change the Thumbnail Splitting setting for Modality US, XA , MG to Image and CT to Series.

                study.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                domain.ModalityDropDown().SelectByText("US");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                PageLoadWait.WaitForPageLoad(20);
                domain.ModalityDropDown().SelectByText("XA");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                PageLoadWait.WaitForPageLoad(20);
                domain.ModalityDropDown().SelectByText("MG");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                PageLoadWait.WaitForPageLoad(20);
                domain.ModalityDropDown().SelectByText("CT");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                PageLoadWait.WaitForPageLoad(20);
                study.CloseUserPreferences();

                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.ModalityDropDown().SelectByText("US");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                PageLoadWait.WaitForPageLoad(20);
                domain.ModalityDropDown().SelectByText("XA");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                PageLoadWait.WaitForPageLoad(20);
                domain.ModalityDropDown().SelectByText("MG");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                PageLoadWait.WaitForPageLoad(20);
                domain.ModalityDropDown().SelectByText("CT");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                PageLoadWait.WaitForPageLoad(20);
                domain.ClickSaveEditDomain();
                domain.ClickCloseEditDomain();
                ExecutedSteps++;

                //---End of script---
                //Logout
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
        /// Thumbnail splitting in Exam List and Study Panel
        /// </summary>

        public TestCaseResult Test_161063(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;

                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNoList.Split(':');
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String[] datasource_135128 = { EA_91, EA_77 };

                //Pre-conditions
                String TestDomain1 = BasePage.GetUniqueDomainID("TestDomain1_135128_");
                String Role1 = BasePage.GetUniqueRole("Role1_135128_");
                String PhysicianRole = BasePage.GetUniqueRole("PhysicianRole_135128_");
                String rad1 = BasePage.GetUniqueUserId("rad1_135128_");

                DomainManagement domain = new DomainManagement();
                RoleManagement role = new RoleManagement();
                UserManagement user = new UserManagement();
                Studies study = new Studies();
                UserPreferences userpref = new UserPreferences();
                BluRingViewer viewer = new BluRingViewer();

                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                Taskbar bar = new Taskbar();
                bar.Hide();
                st.LaunchServiceTool();
                st.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(2);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);

                string[] SeriesMod = { "CT", "MR", "NM", "PT", "RF" };

                var comboBox_mod = wpfobject.GetComboBox("ComboBox_Modality");

                int count = comboBox_mod.Items.Count;
                for (int i = 0; i < count; i++)
                {
                    comboBox_mod.Select(i);
                    if ((SeriesMod.Contains(comboBox_mod.SelectedItemText)))
                    {
                        if (wpfobject.IsRadioBtnSelected("RB_ThumbnailSplittingSeries"))
                        {
                            Logger.Instance.InfoLog("Modality " + comboBox_mod.SelectedItemText + "Series already selected");
                        }
                        else
                        {
                            wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingSeries");
                            Logger.Instance.InfoLog("Modality " + comboBox_mod.SelectedItemText + "Series selected now");
                        }
                    }
                    else
                    {
                        if (wpfobject.IsRadioBtnSelected("RB_ThumbnailSplittingName"))
                        {
                            Logger.Instance.InfoLog("Modality " + comboBox_mod.SelectedItemText + "Image already selected");
                        }
                        else
                        {
                            wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingName");
                            Logger.Instance.InfoLog("Modality " + comboBox_mod.SelectedItemText + "Image selected now");
                        }
                    }
                    wpfobject.WaitTillLoad();
                }

                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
				///Commented as it is not applicable for Universal viewer
				////Thumbnail Captions
				//st.NavigateToTab("Viewer");
				//wpfobject.GetTabWpf(1).SelectTabPage(3);
				//wpfobject.ClickButton("Modify", 1);
				//var comboBoxDicomCaption = wpfobject.GetComboBox("ComboBox_DefaultThumbnailCaption");
				//comboBoxDicomCaption.Select("{S%SeriesNum%}{- %ImageNum%}");
				//wpfobject.WaitTillLoad();
				//wpfobject.ClickButton("Apply", 1);
				//wpfobject.WaitTillLoad();
				st.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(TestDomain1, Role1, datasources: datasource_135128);
                if (domain.OverlayCheckbox().Selected == false)
                    domain.OverlayCheckbox().Click();
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
                //Step-1
                //Login to iCA with a reading physician role (use rad1/rad1 if possible)
                login.DriverGoTo(login.url);
                login.LoginIConnect(rad1, rad1);
                ExecutedSteps++;

                //step-2
                //Open User Preferences and set Thumbnail Splitting ~ Image for MR, CT and CR
                study = (Studies)login.Navigate("Studies");
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                String[] Modality = { "MR", "CT", "CR" };
                for (int i = 0; i < 3; i++)
                {
                    domain.ModalityDropDown().SelectByText(Modality[i]);
                    domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                    PageLoadWait.WaitForPageLoad(20);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                userpref.BluringViewerRadioBtn().Click();
                userpref.CloseUserPreferences();
                ExecutedSteps++;

                //step-3
                //Load the MR study- PatientName= "MICKEY, MOUSE", Accession No="ACC01" in the BlueRing viewer.
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: datasource_135128[0]);
                study.SelectStudy("Accession", AccessionNumbers[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();

                //There should be 100 thumbnails. Each image will have one thumbnail (MR thumbnail is now image-split)
                //The Exam List should have 2 other prior studies- CT (19-Aug-2004) and CR (24-Feb-2000).

                IList<IWebElement> Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                try
                {
                    var cachewait = new DefaultWait<IWebDriver>(BasePage.Driver);
                    cachewait.Timeout = new TimeSpan(0, 1, 0);
                    cachewait.Until<Boolean>((d) =>
                    {
                        BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ThumbnailNextArrowButtonEnabled)).Click();
                        Thread.Sleep(3000);
                        Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                        if (Thumbnail_list.Count == 100)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    });
                }
                catch(Exception ex)
                {
                    Logger.Instance.ErrorLog("Error while Loading the thumbnail. Not all 100 thumbnails are get loaded."+ ex.Message);
                }

                IList<IWebElement> PriorsList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList));
                IList<IWebElement> ExamListPanelDate = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListPanelDate));
                IList<IWebElement> ExamListPanelModality = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorModality));

                if (Thumbnail_list.Count == 100 &&
                    ExamListPanelDate.Count == 3 &&
                    PriorsList.Count == 3 &&
                    ExamListPanelDate[0].Text.Equals("19-Aug-2004") &&
                    ExamListPanelDate[1].Text.Equals("24-Feb-2000") &&
                    ExamListPanelDate[2].Text.Equals("04-Feb-1995") &&
                    ExamListPanelModality[0].Text.Equals("CT") &&
                    ExamListPanelModality[1].Text.Equals("CR") &&
                    ExamListPanelModality[2].Text.Equals("MR"))
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

                //step-4
                //Click on the thumbnail icon for study ACC01 (MR - 04-Feb-1995) in the Exam List.
                IList<IWebElement> ExamlistThumbnailIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailIcon));
                ExamlistThumbnailIcon[2].Click();
                BluRingViewer.WaitforThumbnails();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ExamList_thumbnails))));

                IList<IWebElement> ExamList_Thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));
                try
                {
                    var Thumbnailwait = new DefaultWait<IWebDriver>(BasePage.Driver);
                    Thumbnailwait.Timeout = new TimeSpan(0, 1, 0);
                    Thumbnailwait.Until<Boolean>((d) =>
                    {
                        ExamList_Thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));
                        if (ExamList_Thumbnails.Count == 100)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Error while Loading the thumbnail. Not all 100 thumbnails are get loaded in exam list thumbnail preview." + ex.Message);
                }

                //There should be 100 thumbnails in the Exam list & there should be a scroll bar to traverse
                ExamList_Thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));
                IList<IWebElement> ExamlistScrollbar = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_Scrollbar_ExamList_thumbnails));
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();

                if (ExamList_Thumbnails.Count == 100 &&
                    ExamlistScrollbar[0].Displayed)
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

                //step-5
                //In the Exam List, click on the prior CT study (Acc No- ACC02, 19-Aug-2004) to open it in another study panel.
                viewer.OpenPriors(0);
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();

                //The CT Study should be loaded. There should be 191 thumbnails in the thumbnail bar.

                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(2) " + BluRingViewer.div_thumbnails));
				Logger.Instance.InfoLog("thumbnailCount- Expceted: 191, Actual: " + Thumbnail_list.Count);
				if (Thumbnail_list.Count == 191)
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
                //Click on the thumbnail icon for study ACC02 (CT - 19-Aug-2004) in the Exam List.
                ExamlistThumbnailIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailIcon));
                ExamlistThumbnailIcon[0].Click();
                BluRingViewer.WaitforThumbnails();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ExamList_thumbnails))));

                //There should be 191 thumbnails in the Exam list thumbnail drop-down, should be a scroll bar to traverse and see all the thumbnails.

                ExamList_Thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));
                try
                {
                    var Thumbnailwait = new DefaultWait<IWebDriver>(BasePage.Driver);
                    Thumbnailwait.Timeout = new TimeSpan(0, 1, 0);
                    Thumbnailwait.Until<Boolean>((d) =>
                    {
                        ExamList_Thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));
                        if (ExamList_Thumbnails.Count == 191)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Error while Loading the thumbnail. Not all 191 thumbnails are get loaded in exam list thumbnail preview." + ex.Message);
                }

                ExamList_Thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));
                ExamlistScrollbar = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_Scrollbar_ExamList_thumbnails));

                if (ExamList_Thumbnails.Count == 191 &&
                    ExamlistScrollbar[0].Displayed)
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
                //In the Exam List, click on the prior CR study (Acc No- ACC03, 24-Feb-2000) 
                viewer.OpenPriors(1);
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();

                //The CR Study should be loaded another study panel to the right of the CT study panel.
                //There should be 7 thumbnails in the thumbnail bar.
                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(3) " + BluRingViewer.div_thumbnails));
                if (Thumbnail_list.Count == 7)
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

                //step-8
                //Click on the thumbnail icon for study ACC03 (CR - 24-Feb-2000) in the Exam List.
                ExamlistThumbnailIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailIcon));
                ExamlistThumbnailIcon[1].Click();
                BluRingViewer.WaitforThumbnails();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ExamList_thumbnails))));

                //There should be 7 thumbnails in the Exam list thumbnail drop-down.

                ExamList_Thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));

                if (ExamList_Thumbnails.Count == 7)
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
                viewer.CloseBluRingViewer();

                //step-9
                //Open User Preferences and set Thumbnail Splitting ~ Series for MR and CT Modality
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                String[] Modality_9 = { "MR", "CT" };
                for (int i = 0; i < 2; i++)
                {
                    domain.ModalityDropDown().SelectByText(Modality_9[i]);
                    domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                    PageLoadWait.WaitForPageLoad(20);
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                ExecutedSteps++;

                //step-10
                //Load the MR study- PatientName~"MICKEY, MOUSE", Accession No~"ACC01" in the BlueRing viewer.
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: datasource_135128[0]);
                study.SelectStudy("Accession", AccessionNumbers[0]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();

                //Study should be loaded in the viewer. There should be 2 thumbnails in the thumbnail bar. 
                //Each series will have one thumbnail (MR thumbnails are now series-split)
                //The Exam List should have 2 other prior studies- CT (19-Aug-2004) and CR (24-Feb-2000).

                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                PriorsList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList));
                ExamListPanelDate = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_examListPanelDate));
                ExamListPanelModality = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorModality));

                if (Thumbnail_list.Count == 2 &&
                    ExamListPanelDate.Count == 3 &&
                    PriorsList.Count == 3 &&
                    ExamListPanelDate[0].Text.Equals("19-Aug-2004") &&
                    ExamListPanelDate[1].Text.Equals("24-Feb-2000") &&
                    ExamListPanelDate[2].Text.Equals("04-Feb-1995") &&
                    ExamListPanelModality[0].Text.Equals("CT") &&
                    ExamListPanelModality[1].Text.Equals("CR") &&
                    ExamListPanelModality[2].Text.Equals("MR"))
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

                //step-11
                //Click on the thumbnail icon for study ACC01 (MR - 04-Feb-1995) in the Exam List.

                ExamlistThumbnailIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailIcon));
                ExamlistThumbnailIcon[2].Click();
                BluRingViewer.WaitforThumbnails();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ExamList_thumbnails))));

                //There should be 2 thumbnails in the Exam list thumbnail drop-down (thumbnails should be series-split).

                ExamList_Thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));
                ExamlistScrollbar = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_Scrollbar_ExamList_thumbnails));
                if (ExamList_Thumbnails.Count == 2 &&
                    ExamlistScrollbar.Count == 0 )
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

                //step-12
                //In the Exam List, click on the prior CT study (Acc No- ACC02, 19-Aug-2004) to open it 
                viewer.OpenPriors(0);
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();
                //The CT Study should be loaded in another study panel to the right of the MR study panel. There should be one thumbnail in the thumbnail bar. 

                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(2) " + BluRingViewer.div_thumbnails));
                if (Thumbnail_list.Count == 1)
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

                //step-13
                //Click on the thumbnail icon for study ACC02 (CT - 19-Aug-2004) in the Exam List.

                ExamlistThumbnailIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailIcon));
                ExamlistThumbnailIcon[0].Click();
                BluRingViewer.WaitforThumbnails();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ExamList_thumbnails))));

                //There should be 1 thumbnail in the Exam list thumbnail drop-down.

                ExamList_Thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));
                if (ExamList_Thumbnails.Count == 1)
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

                //step-14
                //In the Exam List, click on the prior CR study (Acc No- ACC03, 24-Feb-2000) to open it in another study panel.
                viewer.OpenPriors(1);
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();
                //The CR Study should be loaded in another study panel to the right of the CT study panel.
                //There should be 7 thumbnails in the thumbnail bar. (Thumbnails is still image-split)

                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(3) " + BluRingViewer.div_thumbnails));
                if (Thumbnail_list.Count == 7)
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

                //step-15
                //Click on the thumbnail icon for study ACC03 (CR - 24-Feb-2000) in the Exam List.

                ExamlistThumbnailIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailIcon));
                ExamlistThumbnailIcon[1].Click();
                BluRingViewer.WaitforThumbnails();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ExamList_thumbnails))));

                //There should be 7 thumbnails in the Exam list thumbnail drop-down.

                ExamList_Thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));

                if (ExamList_Thumbnails.Count == 7)
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
                viewer.CloseBluRingViewer();

				///fix for application session timeout
				login.Logout();
				login.CreateNewSesion();
				login.DriverGoTo(login.url);
				login.LoginIConnect(rad1, rad1);
				study = (Studies)login.Navigate("Studies");

				//Step-16
				//Open User Preferences and set Thumbnail Splitting ~ Auto for US and CT Modality
				userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                String[] Modality_16 = { "US", "CT" };
                for (int i = 0; i < 2; i++)
                {
                    domain.ModalityDropDown().SelectByText(Modality_16[i]);
                    domain.SelectRadioBtn("ThumbSplitRadioButtons", "Auto");
                    PageLoadWait.WaitForPageLoad(20);
                }
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                ExecutedSteps++;

                //step-17
                //Load the US study- PatName="0EAC02C5P07FA6P", AccNo ="394efe9ad" in the BlueRing viewer.
                //Note down the image count on all prior studies

                study.SearchStudy(AccessionNo: AccessionNumbers[1], LastName: LastName, Datasource: EA_91);
                study.SelectStudy("Accession", AccessionNumbers[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();

                int count_17_1 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(1) " + BluRingViewer.div_thumbnails)).Count;

                //Study should be loaded properly in the BlueRing viewer's study panel- 
                if (BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_studypanel)).Displayed)
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

                //step-18
                //Click on the thumbnail icon for US study (10-May-2000) in the Exam List. //42

                ExamlistThumbnailIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailIcon));
                ExamlistThumbnailIcon[1].Click();
                BluRingViewer.WaitforThumbnails();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ExamList_thumbnails))));

                ExamList_Thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));

                //The Exam List Thumbnail panel should display below the study label with-
                //- each multi-frame image will have one thumbnail all non-multiframe
                // (as well as single frame) images will be grouped together and represented by a single thumbnail.

                if (ExamList_Thumbnails.Count == 42 &&
                    count_17_1 == ExamList_Thumbnails.Count)
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

                //step-19
                //In the Exam List, click on the prior CT study (04-Aug-1993 - 5-22-34 PM) to open it in another study panel.

                viewer.OpenPriors(2);
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitforViewports();
                int count_17_2 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel + ":nth-child(2) " + BluRingViewer.div_thumbnails)).Count;

                //The CT Study should be loaded in another study panel to the right of the US study panel. 
                //There should be one thumbnail in the thumbnail bar. 
                //(CT thumbnails are now auto-split - each multi-frame image will have one thumbnail and all non-multi-frame 

                if (count_17_2 == 1)
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

                //Step-20
                //Click on the thumbnail icon for CT study (04-Aug-1993 - 5-22-34 PM) in the Exam List.

                ExamlistThumbnailIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailIcon));
                ExamlistThumbnailIcon[2].Click();
                BluRingViewer.WaitforThumbnails();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ExamList_thumbnails))));

                //The Exam List Thumbnail panel should display below the study label with one thumbnail.

                ExamList_Thumbnails = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ExamList_thumbnails));

                if (ExamList_Thumbnails.Count == 1)
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
                viewer.CloseBluRingViewer();

                //step-21
                //Reset the Thumbnail splitting back to default for the user rad1-
                //Open User Preferences & set Thumbnail Splitting ~ Series for MR, CT,  and Thumbnail Splitting= Image for CR, US

                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                String[] Modality_21 = { "MR", "CT" };
                for (int i = 0; i < 2; i++)
                {
                    domain.ModalityDropDown().SelectByText(Modality_21[i]);
                    domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                    PageLoadWait.WaitForPageLoad(20);
                }

                String[] Modality_21_2 = { "CR", "US" };
                for (int i = 0; i < 2; i++)
                {
                    domain.ModalityDropDown().SelectByText(Modality_21_2[i]);
                    domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                    PageLoadWait.WaitForPageLoad(20);
                }

                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                ExecutedSteps++;

                //Logout
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
        /// Multiple series with mix of image-split, series-split thumbnails and ordering
        /// </summary>

        public TestCaseResult Test_161065(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;

                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNoList.Split(':');

                //Pre-conditon
                String TestDomain1 = BasePage.GetUniqueDomainID("TestDomain1_135123_");
                String Role1 = BasePage.GetUniqueRole("Role1_135123_");

                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                Taskbar bar = new Taskbar();
                bar.Hide();

                //1 and 4
                st.LaunchServiceTool();
                st.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(2);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);

                string[] SeriesMod = { "CT", "MR", "NM", "PT", "RF" };

                var comboBox_mod = wpfobject.GetComboBox("ComboBox_Modality");

                int count = comboBox_mod.Items.Count;
                for (int i = 0; i < count; i++)
                {
                    comboBox_mod.Select(i);
                    if ((SeriesMod.Contains(comboBox_mod.SelectedItemText)))
                    {
                        if (wpfobject.IsRadioBtnSelected("RB_ThumbnailSplittingSeries"))
                        {
                            Logger.Instance.InfoLog("Modality " + comboBox_mod.SelectedItemText + "Series already selected");
                        }
                        else
                        {
                            wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingSeries");
                            Logger.Instance.InfoLog("Modality " + comboBox_mod.SelectedItemText + "Series selected now");
                        }
                    }
                    else
                    {
                        if (wpfobject.IsRadioBtnSelected("RB_ThumbnailSplittingName"))
                        {
                            Logger.Instance.InfoLog("Modality " + comboBox_mod.SelectedItemText + "Image already selected");
                        }
                        else
                        {
                            wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingName");
                            Logger.Instance.InfoLog("Modality " + comboBox_mod.SelectedItemText + "Image selected now");
                        }
                    }
                    wpfobject.WaitTillLoad();
                }

                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
				///Commented as it is not applicable for Universal viewer
				////Thumbnail Captions
				//st.NavigateToTab("Viewer");
    //            wpfobject.GetTabWpf(1).SelectTabPage(3);
    //            wpfobject.ClickButton("Modify", 1);
    //            var comboBoxDicomCaption = wpfobject.GetComboBox("ComboBox_DefaultThumbnailCaption");
    //            comboBoxDicomCaption.Select("{S%SeriesNum%}{- %ImageNum%}");
    //            wpfobject.WaitTillLoad();
    //            wpfobject.ClickButton("Apply", 1);
    //            wpfobject.WaitTillLoad();
                st.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();
                bar.Show();

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.CreateDomain(TestDomain1, Role1, datasources: datasource);
                if (domain.OverlayCheckbox().Selected == false)
                    domain.OverlayCheckbox().Click();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(10);
                domain.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(30);
                login.Logout();
               
                //step-1-
                //Login to WebAccess client (you can use admin1/admin1 user)
                login.DriverGoTo(login.url);
                login.LoginIConnect(TestDomain1, TestDomain1);
                ExecutedSteps++;

                //step-2
                //"Load a data which has series of modality like CR/CT/MR, PR, KO or OT in the same study. 
                //12 MR series with more than 1 image per series
                //3 KO series //? PR series  //Acc: 9066875

                Studies study = (Studies)login.Navigate("Studies");
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumbers[0]);
                BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
                IList<IWebElement> Thumbnail_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ThumbnailOuter));
                IList<IWebElement> Thumbnail_Caption = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailCaption));

                String[] caption_text_MR = { "S0", "S1", "S2", "S3", "S4", "S5", "S6", "S7", "S8", "S9", "S10", "S400" };

                Boolean res2 = true;
                if (Thumbnail_Outer.Count == 18)
                {
                    for (int i = 0; i < 3; i++) //KO
                    {
                        String title = Thumbnail_Outer[i].GetAttribute("title");
                        if (title.Contains("Modality:KO"))
                            Logger.Instance.InfoLog("Thumbnail Title " + title + " -Verified successfully");
                        else
                        {
                            res2 = false;
                            Logger.Instance.ErrorLog("Thumbnail Title " + title + "  is not correct -Verified failed");
                            break;
                        }
                    }
                    for (int i = 3; i < 6; i++) //PR
                    {
                        String title = Thumbnail_Outer[i].GetAttribute("title");
                        if (title.Contains("Modality:PR"))
                            Logger.Instance.InfoLog("Thumbnail Title " + title + " -Verified successfully");
                        else
                        {
                            res2 = false;
                            Logger.Instance.ErrorLog("Thumbnail Title " + title + "  is not correct -Verified failed");
                            break;
                        }
                    }

                    for (int i = 6; i < 18; i++) //MR
                    {
                        String title = Thumbnail_Outer[i].GetAttribute("title");
                        String Series = Thumbnail_Caption[i].GetAttribute("innerText");
                        if (title.Contains("Modality:MR") && Series.Equals(caption_text_MR[i - 6]))
                        {
                            Logger.Instance.InfoLog("Thumbnail Title " + title + " -Verified successfully");
                            Logger.Instance.InfoLog("Thumbnail Series " + Series + " -Verified successfully");
                        }
                        else
                        {
                            res2 = false;
                            Logger.Instance.ErrorLog("Thumbnail Title " + title + "  is not correct -Verified failed");
                            Logger.Instance.ErrorLog("Thumbnail Series " + Series + "  is not correct -Verified failed");
                            break;
                        }
                    }
                }
                else
                {
                    res2 = false;
                     Logger.Instance.ErrorLog("Thumbnail Count  " + Thumbnail_Outer.Count + "  is not correct -Verified failed");
                }

                //The first 3 thumbnails are representing the KO series Next, the PR series is displayed (optional)
                //next MR series (12 in total). Ensure the MR series are ordered by series number (increasing order).
                //The two SR reports are displayed when the report icon is clicked.

                var reportIcon = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_priorsreportIcon));
                reportIcon.Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(BluRingViewer.div_Reports)));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_Reports))[0]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_Reports))[1]));
                //BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_Reports))[2]));
                var reports = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_Reports));

                if (res2 && reports[0].Displayed &&
                    reports[1].Displayed)
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
                viewer.CloseBluRingViewer();

                //step-3
                //Load a second data (mix of modalities series)Eg: Test, Arsone, patient ID = X010025
                //6 CT series with single and multiple images / 4 PT series /? PR series
                // Acc= X01002507082010
                study.SearchStudy(AccessionNo: AccessionNumbers[1]);
                study.SelectStudy("Accession", AccessionNumbers[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();

                //a. The first three thumbnails are representing all images in the PR series (PR series cannot be configured for image-scope thumbnails).
                //b. The 4th, 5th, 6th, 7th, 8th and 9th thumbnails are representing the images of the CT series;  the sort order of thumbnails should be by increasing series number.
                //c. The 10th, 11th, 12th and 13th thumbnails are representing the images of the PT series; the sort order of thumbnails should be by increasing series number.

                //** PR images Not available---
                String[] caption_text_CT = { "S1- 1", "S1- 1", "S2", "S2", "S999- 1", "S999- 1" };
                String[] caption_text_PT = { "S3- 14", "S3- 201", "S4- 14", "S4- 201" };

                Thumbnail_Outer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_ThumbnailOuter));
                Thumbnail_Caption = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailCaption));
                Boolean res3 = true;

                for (int i = 0; i < 6; i++)  //CT
                {
                    String title = Thumbnail_Outer[i].GetAttribute("title");
                    String Series = Thumbnail_Caption[i].GetAttribute("innerText");
                    if (title.Contains("Modality:CT") && Series.Equals(caption_text_CT[i]))
                    {
                        Logger.Instance.InfoLog("Thumbnail Caption " + (i + 1) + ":" + title + " -Verified successfully");
                        Logger.Instance.InfoLog("Thumbnail Series " + (i + 1) + ":" + Series + " -Verified successfully");
                    }
                    else
                    {
                        res3 = false;
                        Logger.Instance.ErrorLog("Thumbnail Caption " + (i + 1) + ":" + title + " is not correct -Verified failed");
                        Logger.Instance.ErrorLog("Thumbnail Series " + (i + 1) + ":" + Series + " is not correct -Verified failed");
                        break;
                    }
                }

                for (int i = 6; i < 10; i++) //PT
                {
                    String title = Thumbnail_Outer[i].GetAttribute("title");
                    String Series = Thumbnail_Caption[i].GetAttribute("innerText");
                    if (title.Contains("Modality:PT") && Series.Equals(caption_text_PT[i - 6]))
                    {
                        Logger.Instance.InfoLog("Thumbnail Caption " + (i + 1) + ":" + title + " -Verified successfully");
                        Logger.Instance.InfoLog("Thumbnail Series " + (i + 1) + ":" + Series + " -Verified successfully");
                    }
                    else
                    {
                        res3 = false;
                        Logger.Instance.ErrorLog("Thumbnail Caption " + (i + 1) + ":" + title + " is not correct -Verified failed");
                        Logger.Instance.ErrorLog("Thumbnail Series " + (i + 1) + ":" + Series + " is not correct -Verified failed");
                        break;
                    }
                }

                if (res3 && Thumbnail_Outer.Count == 10)
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
		/// Thumbnail - Cardio ordering - modality sorting
		/// </summary>
		public TestCaseResult Test_161061(String testid, String teststeps, int stepcount)
		{
			//Declare and initialize variables            
			TestCaseResult result;
			result = new TestCaseResult(stepcount);
			int ExecutedSteps = -1;

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);

			try
			{
				String username = Config.adminUserName;
				String password = Config.adminPassword;

				String AccessionNumber = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");				

				BluRingViewer viewer = new BluRingViewer();
				ServiceTool st = new ServiceTool();
				WpfObjects wpfobject = new WpfObjects();


				//Step-1
				//Pre-conditions are completed
				String TestDomain1 = BasePage.GetUniqueDomainID("TestDomain1_161061_");
				String Role1 = BasePage.GetUniqueRole("Role1_161061_");
				login.DriverGoTo(login.url);
				login.LoginIConnect(username, password);
				DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
				domain.CreateDomain(TestDomain1, Role1, datasources: datasource);
				PageLoadWait.WaitForPageLoad(30);
				PageLoadWait.WaitForFrameLoad(10);
				domain.ClickSaveDomain();
				PageLoadWait.WaitForPageLoad(30);
				login.Logout();
				login.DriverGoTo(login.url);
				login.LoginIConnect(TestDomain1, TestDomain1);
				ExecutedSteps++;

				//step-2-
				//Load a data which has series of modality like CR/CT/MR, PR, KO or OT in the same study into the Universal viewer. E.g. Schmidt James has MR, PR and KO series:						
				Studies study = (Studies)login.Navigate("Studies");				
				study.SearchStudy(AccessionNo: AccessionNumber, Datasource: EA_131);
				PageLoadWait.WaitForPageLoad(20);
				study.SelectStudy("Accession", AccessionNumber);
				viewer = BluRingViewer.LaunchBluRingViewer();

				IList<IWebElement> Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
				if (Thumbnail_list.Count == 18)
				{
					int thumbnailCount = 0;
					result.steps[++ExecutedSteps].AddPassStatusList("Total Thumbnail count is 18");																
					IList<IWebElement> Thumbnail_Modality = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailcontainer + " " + BluRingViewer.div_thumbnailModality));
					for (thumbnailCount = 0; thumbnailCount < 3; thumbnailCount++)
					{
						if (!Thumbnail_Modality[thumbnailCount].Text.Equals("KO"))
						{
							result.steps[ExecutedSteps].AddFailStatusList("KO modality not available in thumbanil no: " + thumbnailCount);
						}
					}
					for (thumbnailCount = 3; thumbnailCount < 6; thumbnailCount++)
					{
						if (!Thumbnail_Modality[thumbnailCount].Text.Equals("PR"))
						{
							result.steps[ExecutedSteps].AddFailStatusList("PR modality not available in thumbanil no: " + thumbnailCount);
						}
					}										
					for (thumbnailCount = 6; thumbnailCount < 18; thumbnailCount++)
					{
						if (Thumbnail_Modality[thumbnailCount].Text.Equals(""))
						{
							BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ThumbnailNextArrowButton)).Click();
						}
						if (!Thumbnail_Modality[thumbnailCount].Text.Equals("MR"))
						{
							result.steps[ExecutedSteps].AddFailStatusList("MR modality not available in thumbanil no: " + thumbnailCount);
						}
					}
				}
				else
				{
					result.steps[++ExecutedSteps].AddFailStatusList("Total Thumbnail count is: " + Thumbnail_list.Count + ", Actual: 18");
				}

				if (result.steps[ExecutedSteps].statuslist.Any(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}
				viewer.CloseBluRingViewer();

				//Logout
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
        /// Verify "Cardio Order" Check box state
        /// </summary>
        public TestCaseResult Test_161069(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;

                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                Taskbar bar = new Taskbar();

                //Step 1
                st.LaunchServiceTool();
                st.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(2);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                st.SelectDropdown("ComboBox_Modality", "CR");
                result.steps[++ExecutedSteps].StepPass();

                //Step 2
                wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingSeries");
                var Cardiology_CheckBox = wpfobject.GetCheckBox(0);
                Thread.Sleep(3000);
                if (!Cardiology_CheckBox.Enabled)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 3
                wpfobject.ClickRadioButton("Auto", 1);
                Cardiology_CheckBox = wpfobject.GetCheckBox(0);
                Thread.Sleep(3000);
                if (!Cardiology_CheckBox.Enabled)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 4
                wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingName");
                Cardiology_CheckBox = wpfobject.GetCheckBox(0);
                Thread.Sleep(3000);
                if (Cardiology_CheckBox.Enabled)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                st.CloseServiceTool();


                //Step 5              
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                result.steps[++ExecutedSteps].StepPass();

                //Step 6
                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                result.steps[++ExecutedSteps].StepPass();

                //Step 7
                domain.SearchDomain(Config.adminGroupName);
                domain.SelectDomain(Config.adminGroupName);
                domain.EditDomainButton().Click();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                domain.ModalityDropDown().SelectByText("CR");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                Thread.Sleep(2000);
                if (!domain.CardioOrderCheckBox().Enabled)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 8
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Auto");
                Thread.Sleep(2000);
                if (!domain.CardioOrderCheckBox().Enabled)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }


                //Step 9
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                Thread.Sleep(2000);
                if (domain.CardioOrderCheckBox().Enabled)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                domain.EditDomainCloseBtn().Click();

                //Step 10
                UserPreferences userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.ModalityDropDown().SelectByText("CR");
                PageLoadWait.WaitForPageLoad(20);
                userpreferences.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                PageLoadWait.WaitForPageLoad(20);
                if (!userpreferences.CardioOrderCheckBox().Enabled)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 11
                userpreferences.SelectRadioBtn("ThumbSplitRadioButtons", "Auto");
                PageLoadWait.WaitForPageLoad(20);
                if (!userpreferences.CardioOrderCheckBox().Enabled)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 12
                userpreferences.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                PageLoadWait.WaitForPageLoad(20);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(userpreferences.CardioOrderCheckBox()));
                if (userpreferences.CardioOrderCheckBox().Enabled)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                userpreferences.CloseUserPreferences();
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
		/// Thumbnail - Cardio ordering - series sorting
		/// </summary>
		public TestCaseResult Test_161059(String testid, String teststeps, int stepcount)
		{
			//Declare and initialize variables            
			TestCaseResult result;
			result = new TestCaseResult(stepcount);
			int ExecutedSteps = -1;

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);

			try
			{
				String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");

				//Step 1- Pre-conditions are completed
				String TestDomain1 = BasePage.GetUniqueDomainID("TestDomain1_161059_");
				String Role1 = BasePage.GetUniqueRole("Role1_161059_");
				login.DriverGoTo(login.url);
				login.LoginIConnect(Config.adminUserName, Config.adminPassword);
				DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
				domain.CreateDomain(TestDomain1, Role1, datasources: datasource);
				PageLoadWait.WaitForPageLoad(30);
				PageLoadWait.WaitForFrameLoad(10);
				domain.ClickSaveDomain();
				PageLoadWait.WaitForPageLoad(30);
				login.Logout();
				login.DriverGoTo(login.url);
				login.LoginIConnect(TestDomain1, TestDomain1);
				ExecutedSteps++;

				//step 2- Go to Domain Management > view current domain (i.e., TestDomain1). Ensure that CT thumbnail splitting is "Series" and "Cardio Order" is unchecked.
				domain = (DomainManagement)login.Navigate("DomainManagement");
				//domain.SearchDomain(Config.adminGroupName);
				//domain.SelectDomain(Config.adminGroupName);
				//domain.EditDomainButton().Click();
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
				domain.ModalityDropDown().SelectByText("CT");
				if (domain.SelectedValueOfRadioBtn("ThumbSplitRadioButtons").Equals("Series") && !domain.CardioOrderCheckBox().Selected)
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}
				//domain.EditDomainCloseBtn().Click();

				//Step 3- Go to User Preferences and ensure that CT thumbnail splitting is "Series" and "Cardio Order" is unchecked.
				UserPreferences userpreferences = new UserPreferences();
				userpreferences.OpenUserPreferences();
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
				userpreferences.ModalityDropDown().SelectByText("CT");
				if (userpreferences.SelectedValueOfRadioBtn("ThumbSplitRadioButtons").Equals("Series") && !userpreferences.CardioOrderCheckBox().Selected)
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}
				userpreferences.CloseUserPreferences();

				//Step 4- 	Go to the Studies tab, search and load study "CT, MULTIPLESTUDY" in Universal viewer // PID161059
				Studies study = (Studies)login.Navigate("Studies");
				study.SearchStudy(patientID: PatientID, Datasource: EA_91);
				PageLoadWait.WaitForPageLoad(20);
				study.SelectStudy("Patient ID", PatientID);
				viewer = BluRingViewer.LaunchBluRingViewer();

				//Verify that the order of thumbnails are as follows:
				//796 - Series "2" Image "2" - series date time(29 - Jul - 08, NULL)
				//795 - Series "2" Image "1" - series date time(29 - Jun - 08, NULL)
				//782 - Series "3" Image "4" - series date time(29 - Jun - 06 11:47:50 PM)
				//781 - Series "3" Image "5" - series date time(29 - Jun - 06 10:47:50 PM)
				//798 - Series "3" Image "6" - series date time(NULL 11:47:50 PM)
				//799 - Series "3" Image "7" - series date time(NULL 08:47:50 PM)
				//797 - Series "3" Image "3" - series date time(NULL NULL)
				//This is because the sort order is: series number > series date > series time > series instance uid.

				String[] CaptionText = { "S2- 2", "S2- 1", "S3- 4", "S3- 5", "S3- 6", "S3- 7", "S3- 3" };
				bool ThumbnailSortedInSeries = true;
				var ActualCaption = new List<String>();
				IList<IWebElement> Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
				if (Thumbnail_list.Count == 7)
				{
					int thumbnailCount = 0;
					result.steps[++ExecutedSteps].AddPassStatusList("Total Thumbnail count is 7");
					IList<IWebElement> Thumbnail_Caption = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailcontainer + " " + BluRingViewer.div_thumbnailCaption));
					IList<IWebElement> Thumbnail_Modality = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailcontainer + " " + BluRingViewer.div_thumbnailModality));

					for (thumbnailCount = 0; thumbnailCount < 7; thumbnailCount++)
					{
						if (Thumbnail_Modality[thumbnailCount].Text.Equals(""))
						{
							BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_ThumbnailNextArrowButton)).Click();
						}

						if (!Thumbnail_Caption[thumbnailCount].GetAttribute("innerText").Equals(CaptionText[thumbnailCount]) || !Thumbnail_Modality[thumbnailCount].Text.Equals("CT"))
						{
							ThumbnailSortedInSeries = false;
						}
						ActualCaption.Add(Thumbnail_Caption[thumbnailCount].GetAttribute("innerText"));
					}
					if (ThumbnailSortedInSeries)
					{
						result.steps[ExecutedSteps].AddPassStatusList("Thumbnails displayed in serires order. Expected: " + string.Join(", ", CaptionText) +"Actual: " + string.Join(",", ActualCaption));
					}
					else
					{
						result.steps[ExecutedSteps].AddFailStatusList("Thumbnails not displayed in serires order. Expected: " + string.Join(",", CaptionText) + "Actual: " + string.Join(",", ActualCaption));
					}
				}
				else
				{
					result.steps[++ExecutedSteps].AddFailStatusList("Total Thumbnail count is: " + Thumbnail_list.Count + ", Expected: 7");
				}
				if (result.steps[ExecutedSteps].statuslist.Any(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}
				viewer.CloseBluRingViewer();

				//Logout
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
		/// Thumbnail - Cardio ordering - image sorting
		/// </summary>
		public TestCaseResult Test_161060(String testid, String teststeps, int stepcount)
		{
			//Declare and initialize variables            
			TestCaseResult result;
			result = new TestCaseResult(stepcount);
			int ExecutedSteps = -1;

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);

			try
			{
				String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
				String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");

				//Step 1- Pre-conditions are completed
				String TestDomain1 = BasePage.GetUniqueDomainID("TestDomain1_161060_");
				String Role1 = BasePage.GetUniqueRole("Role1_161060_");
				login.DriverGoTo(login.url);
				login.LoginIConnect(Config.adminUserName, Config.adminPassword);
				DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
				domain.CreateDomain(TestDomain1, Role1, datasources: datasource);
				PageLoadWait.WaitForPageLoad(30);
				PageLoadWait.WaitForFrameLoad(10);
				domain.ClickSaveDomain();
				PageLoadWait.WaitForPageLoad(30);
				login.Logout();
				login.DriverGoTo(login.url);
				login.LoginIConnect(TestDomain1, TestDomain1);
				ExecutedSteps++;

				//step 2- Go to Domain Management > view current domain (i.e., TestDomain1). Ensure that CT thumbnail splitting is "Series" and "Cardio Order" is unchecked.
				domain = (DomainManagement)login.Navigate("DomainManagement");				
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
				domain.ModalityDropDown().SelectByText("XA");
				if (domain.SelectedValueOfRadioBtn("ThumbSplitRadioButtons").Equals("Image") && !domain.CardioOrderCheckBox().Selected)
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}				

				//Step 3- Go to User Preferences and ensure that CT thumbnail splitting is "Series" and "Cardio Order" is unchecked.
				UserPreferences userpreferences = new UserPreferences();
				userpreferences.OpenUserPreferences();
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
				userpreferences.ModalityDropDown().SelectByText("XA");
				if (userpreferences.SelectedValueOfRadioBtn("ThumbSplitRadioButtons").Equals("Image") && !userpreferences.CardioOrderCheckBox().Selected)
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}
				userpreferences.CloseUserPreferences();

				//Step 4- 	Go to the Studies tab, search and load study "ANOTHERBIG, STUDY" into the Universal viewer
				Studies study = (Studies)login.Navigate("Studies");
				study.SearchStudy(LastName: LastName, Datasource: EA_131);
				PageLoadWait.WaitForPageLoad(20);
				study.SelectStudy("Patient ID", PatientID);
				viewer = BluRingViewer.LaunchBluRingViewer();

				//Verify that the order of thumbnails are as follows:				
				//Sort order is series# > image# > sop_instance_uid

				//String[] CaptionText = { "S1- 6", "S1- 1", "S1- 2", "S1- 2", "S1- 2", "S2- 1", "S1- 2", "S2- 1", "S2- 1", "S2- 1" };
				//String[] FrameNumber = { "88", "128", "39", "90", "1", "1", "101", "90", "90" };

				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
				if (viewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer()))
				{
					result.steps[ExecutedSteps].StepPass();					
				}
				else
				{
					result.steps[ExecutedSteps].StepFail();
				}
				viewer.CloseBluRingViewer();

				//step 5- Go to Domain Management > view current domain (i.e., TestDomain1). Ensure that CT thumbnail splitting is "Series" and check "Cardio Order" (enable this feature). Save Domain.
				domain = (DomainManagement)login.Navigate("DomainManagement");
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
				domain.ModalityDropDown().SelectByText("XA");
				BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.CardioOrderCheckBox()));
				domain.CardioOrderCheckBox().Click();
				if (domain.SelectedValueOfRadioBtn("ThumbSplitRadioButtons").Equals("Image") && domain.CardioOrderCheckBox().Selected)
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}
				domain.ClickSaveEditDomain();

				//Step 6- Go to User Preferences and ensure that CT thumbnail splitting is "Series" and "Cardio Order" is unchecked.
				userpreferences = new UserPreferences();
				userpreferences.OpenUserPreferences();
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
				userpreferences.ModalityDropDown().SelectByText("XA");
				BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(userpreferences.CardioOrderCheckBox()));
				userpreferences.CardioOrderCheckBox().Click();
				if (userpreferences.SelectedValueOfRadioBtn("ThumbSplitRadioButtons").Equals("Image") && userpreferences.CardioOrderCheckBox().Selected)
				{
					result.steps[++ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[++ExecutedSteps].StepFail();
				}
				userpreferences.CloseUserPreferences();

				//Step 7- 	Go to the Studies tab, search and load study "ANOTHERBIG, STUDY" into the Universal viewer
				study = (Studies)login.Navigate("Studies");
				study.SearchStudy(LastName: LastName, Datasource: EA_131);
				PageLoadWait.WaitForPageLoad(20);
				study.SelectStudy("Patient ID", PatientID);
				viewer = BluRingViewer.LaunchBluRingViewer();

				//Verify that the order of thumbnails are as follows:				
				//Sort order is series# > image# > biplaneA > biplane B > Primary > Secondary > more#frames > less#frames > sop_instance_uid
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
				if (viewer.CompareImage(result.steps[ExecutedSteps], BluRingViewer.StudyPanelThumbnailContainer()))
				{
					result.steps[ExecutedSteps].StepPass();
				}
				else
				{					
					result.steps[ExecutedSteps].StepFail();
				}
				viewer.CloseBluRingViewer();

				//Reset the cardio order:
				//Step 8- Go to Domain Management > view current domain(i.e., TestDomain1). Ensure that XA thumbnail splitting is "Image" and "Cardio Order" is unchecked.Save Domain.
				domain = (DomainManagement)login.Navigate("DomainManagement");
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent");
				domain.ModalityDropDown().SelectByText("XA");
				BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(domain.CardioOrderCheckBox()));
				domain.CardioOrderCheckBox().Click();
				if (domain.SelectedValueOfRadioBtn("ThumbSplitRadioButtons").Equals("Image") && !domain.CardioOrderCheckBox().Selected)
				{
					result.steps[++ExecutedSteps].AddPassStatusList();
				}
				else
				{
					result.steps[++ExecutedSteps].AddFailStatusList();
				}
				domain.ClickSaveEditDomain();

				//Go to User Preferences and ensure that XA thumbnail splitting is "Image" and uncheck "Cardio Order"(disable it).Save preferences
				userpreferences = new UserPreferences();
				userpreferences.OpenUserPreferences();
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
				userpreferences.ModalityDropDown().SelectByText("XA");
				BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(userpreferences.CardioOrderCheckBox()));
				userpreferences.CardioOrderCheckBox().Click();
				if (userpreferences.SelectedValueOfRadioBtn("ThumbSplitRadioButtons").Equals("Image") && !userpreferences.CardioOrderCheckBox().Selected)
				{
					result.steps[ExecutedSteps].AddPassStatusList();
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList();
				}
				userpreferences.CloseUserPreferences();
				if (result.steps[ExecutedSteps].statuslist.Any(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Logout
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
		/// Workflow - open series from thumbnail bar and EXAM LIST thumbnail preview area (CR)
		/// </summary>
		public TestCaseResult Test_164728(String testid, String teststeps, int stepcount)
		{
			//Declare and initialize variables            
			TestCaseResult result;
			result = new TestCaseResult(stepcount);
			int ExecutedSteps = -1;

			//Set up Validation Steps
			result.SetTestStepDescription(teststeps);

			try
			{
				String AccessionNumberList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList"); // 891112
				String[] AccessionNumber = AccessionNumberList.Split(':');
				String warningmessage = @"You have reached the maximum number of study viewer panels. Close a study viewer panel to open the selected study in a new panel.";

				//Pre-conditions are completed
				BasePage.SetVMResolution("1980", "1080");
				String TestDomain1 = BasePage.GetUniqueDomainID("TestDomain1_164728_");
				String Role1 = BasePage.GetUniqueRole("Role1_164728_");
				login.DriverGoTo(login.url);
				login.LoginIConnect(Config.adminUserName, Config.adminPassword);
				DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
				domain.CreateDomain(TestDomain1, Role1, datasources: datasource);
				PageLoadWait.WaitForPageLoad(30);
				PageLoadWait.WaitForFrameLoad(10);
				domain.ClickSaveDomain();
				PageLoadWait.WaitForPageLoad(30);
				login.Logout();
				login.DriverGoTo(login.url);
				login.LoginIConnect(TestDomain1, TestDomain1);
				UserPreferences userpreferences = new UserPreferences();
				userpreferences.OpenUserPreferences();
				BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
				userpreferences.ModalityDropDown().SelectByText("CR");
				userpreferences.ThumbnailSplittingImageRadioBtn().Click();
				userpreferences.LayoutDropdown().SelectByText("1x2");
				userpreferences.CloseUserPreferences();

				//Step 1- Search and load a CR study that has related study in universal viewer(e.g. Chest, Chester, CR studies) 
				Studies study = (Studies)login.Navigate("Studies");
				study.SearchStudy(AccessionNo: AccessionNumber[0], Datasource: EA_131);
				PageLoadWait.WaitForPageLoad(20);
				study.SelectStudy("Accession", AccessionNumber[0]);
				viewer = BluRingViewer.LaunchBluRingViewer();

				//Images of the selected study displays in the Universal viewer in 1x2 layout as PRIMARY study; thumbnail captions in 4 corners displayed match each series displayed in the viewport:
				//top left shows modality: CR
				//top right shows number of images in the series: 1
				//bottom left show Series#-Image#: S1-2
				//bottom right shows percentage of viewed images: 100 %

				String[] SeriesCaptionText = { "S1- 2", "S1- 2", "S1- 1", "S1- 1" };
				IList<IWebElement> Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
				bool isViewportLoaded = false;
				//IList<IWebElement> ThumbnailPercentImagesViewed, Image_FrameNumber, Thumbnail_Caption, Thumbnail_Modality;
				IList<string> Thumbnail_Modality = new List<string>(); IList<string> ThumbnailPercentImagesViewed = new List<string>(); IList<string> Thumbnail_Caption = new List<string>(); IList<string> Image_FrameNumber = new List<string>();				
				for (int count = 0;count < Thumbnail_list.Count; count++)
				{
					Thumbnail_Modality.Add(Thumbnail_list[count].FindElement(By.CssSelector(BluRingViewer.div_thumbnailModality)).Text);					
					Image_FrameNumber.Add(Thumbnail_list[count].FindElement(By.CssSelector(BluRingViewer.div_imageFrameNumber)).Text);
					Thumbnail_Caption.Add(Thumbnail_list[count].FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption)).Text);
					if (count < 2)
						ThumbnailPercentImagesViewed.Add(Thumbnail_list[count].FindElement(By.CssSelector(BluRingViewer.div_thumbnailPercentImagesViewed)).Text);
					else
						ThumbnailPercentImagesViewed.Add("");
				}

				if (Thumbnail_list.Count == 4)
				{
					int thumbnailCount = 0;
					result.steps[++ExecutedSteps].AddPassStatusList("Total Thumbnail count is 4");
					//ThumbnailPercentImagesViewed = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailcontainer + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
					//Image_FrameNumber = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailcontainer + " " + BluRingViewer.div_imageFrameNumber));
					//Thumbnail_Caption = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailcontainer + " " + BluRingViewer.div_thumbnailCaption));
					//Thumbnail_Modality = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailcontainer + " " + BluRingViewer.div_thumbnailModality));

					for (thumbnailCount = 0; thumbnailCount < 2; thumbnailCount++)
					{
						if (!Thumbnail_Caption[thumbnailCount].Equals(SeriesCaptionText[thumbnailCount]) || !Thumbnail_Modality[thumbnailCount].Equals("CR")
							|| !Image_FrameNumber[thumbnailCount].Equals("1") || !ThumbnailPercentImagesViewed[thumbnailCount].Equals("100%"))
						{
							result.steps[ExecutedSteps].AddFailStatusList("Thumbnails captionn is incorrect for thumbnail: " + thumbnailCount);
						}
						else
						{
							result.steps[ExecutedSteps].AddPassStatusList("Thumbnails captions displayed correctly for thumbnail: " + thumbnailCount);
						}
					}					
					result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
					isViewportLoaded = viewer.CompareImage(result.steps[ExecutedSteps],
								viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer), RGBTolerance: 70);
				}
				else
				{
					result.steps[++ExecutedSteps].AddFailStatusList("Total Thumbnail count is: " + Thumbnail_list.Count + ", Expected: 4");
				}

				if (result.steps[ExecutedSteps].statuslist.Any(status => status == "Fail") || !isViewportLoaded)
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step2 -  Open preview area from each study listed EXAM LIST
				IList<IWebElement> ExamlistThumbnailIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnailIcon));
				int accCount = 0;				
				ExecutedSteps++;
				foreach (IWebElement prior in BasePage.FindElementsByCss(BluRingViewer.div_priors))
				{
					viewer.OpenExamListThumbnailPreview(accession: AccessionNumber[accCount]);
					for (int thumbnail = 1; thumbnail <= BasePage.FindElementsByCss(BluRingViewer.div_ExamList_thumbnails).Count; thumbnail++)
					{
						if (thumbnail == 1 && accCount == 0)
						{
							if (!BluRingViewer.VerifyThumbnailsInExamList(thumbnail, "Active"))
							{
								result.steps[ExecutedSteps].AddFailStatusList();
							}
						}
						else if (accCount == 0 && thumbnail <= 2)
						{
							if (!BluRingViewer.VerifyThumbnailsInExamList(thumbnail, "Visible"))
							{
								result.steps[ExecutedSteps].AddFailStatusList();
							}
						}
						else if (accCount != 0 || (accCount == 0 && thumbnail > 2))
						{
							if (!BluRingViewer.VerifyThumbnailsInExamList(thumbnail, "No Border"))
								result.steps[ExecutedSteps].AddFailStatusList();
						}
					}
					accCount++;
				}
				viewer.OpenExamListThumbnailPreview(accession: AccessionNumber[0]);
				IList<string> CaptionInThumbnailPreview = new List<string>();
				foreach (IWebElement thumbnails in BasePage.FindElementsByCss(BluRingViewer.div_ExamList_thumbnails))
					CaptionInThumbnailPreview.Add(thumbnails.FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption)).Text);				
				if (!CaptionInThumbnailPreview.SequenceEqual(Thumbnail_Caption))
				{
					result.steps[ExecutedSteps].AddFailStatusList();
				}				
				if (result.steps[ExecutedSteps].statuslist.Any(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 3- Apply 2D tools, ww/wl, flip, zoom, pan on both images
				viewer.ClickOnViewPort(1, 1);
				viewer.SelectViewerTool(BluRingTools.Window_Level);
				viewer.ApplyTool_WindowWidth();
				viewer.SelectViewerTool(BluRingTools.Pan);
				viewer.ApplyTool_Pan();
				viewer.SelectViewerTool(BluRingTools.Interactive_Zoom);
				viewer.ApplyTool_Zoom();
				viewer.SelectViewerTool(BluRingTools.Flip_Horizontal);				
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				bool Step3_1 = viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(viewer.Activeviewport), RGBTolerance: 70);				

				
				viewer.ClickOnViewPort(1, 2);
				viewer.SelectViewerTool(BluRingTools.Window_Level, viewport: 2);
				viewer.ApplyTool_WindowWidth();
				viewer.SelectViewerTool(BluRingTools.Pan, viewport: 2);
				viewer.ApplyTool_Pan();
				viewer.SelectViewerTool(BluRingTools.Interactive_Zoom, viewport: 2);
				viewer.ApplyTool_Zoom();
				viewer.SelectViewerTool(BluRingTools.Flip_Horizontal, viewport: 2);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				bool Step3_2 = viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(viewer.Activeviewport), RGBTolerance: 70);

				if (Step3_1 && Step3_2)
				{
					result.steps[ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[ExecutedSteps].StepFail();
				}

				//Step 4- Ensure the thumbnail preview area is displayed under a related study, load the related study in COMPARISON study panel
				viewer.OpenPriors(accession: AccessionNumber[1]);
				ExecutedSteps++;
				if (!viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_studypanel + ":nth-of-type(2)")))
				{
					result.steps[ExecutedSteps].AddFailStatusList(logMessage:"Comparison study panel not displayed");
				}
				viewer.OpenExamListThumbnailPreview(accession: AccessionNumber[1]);
				for (int thumbnail = 1; thumbnail <= BasePage.FindElementsByCss(BluRingViewer.div_ExamList_thumbnails).Count; thumbnail++)
				{
					if (thumbnail == 1)
					{
						if (!BluRingViewer.VerifyThumbnailsInExamList(thumbnail, "Active"))
						{
							result.steps[ExecutedSteps].AddFailStatusList();
						}
					}
					else if (!BluRingViewer.VerifyThumbnailsInExamList(thumbnail, "Visible"))
					{
						result.steps[ExecutedSteps].AddFailStatusList();
					}
				}

				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 5- 	From the Primary study panel thumbnail bar, double click on a different thumbnail (PA image in the 2nd thumbnail) to load it into the left viewport, then drag&drop the other thumbnail (LAT image in the 1st thumbnail) into the right viewport
				viewer.ClickOnViewPort(1, 1);
				viewer.ClickOnThumbnailsInStudyPanel(1, 2, doubleclick: true,isTestcompleteAction: true);
				PageLoadWait.WaitForLoadInViewport(10, BasePage.FindElementByCss(viewer.GetViewportCss(1, 0)));
				//result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				//bool Step5_1 = viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(viewer.SetViewPort1(1, 1)), RGBTolerance: 70);
				if (viewer.VerifyViewPortIsActive(1, 1) && BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 2))// && Step5_1)
				{
					result.steps[++ExecutedSteps].AddPassStatusList(logMessage: "2nd thumbnail loaded in 1st viewport in studypanel 1 using double click");
				}
				else
				{
					result.steps[++ExecutedSteps].AddFailStatusList(logMessage: "2nd thumbnail not loaded in 1st viewport in studypanel 1 using double click");
				}

				viewer.DropAndDropThumbnails(thumbnailnumber: 1, viewport: 2, studyPanelNumber: 1, UseDragDrop: true);
				PageLoadWait.WaitForLoadInViewport(10, BasePage.FindElementByCss(viewer.GetViewportCss(1, 1)));
				//result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				//bool Step5_2 = viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(viewer.SetViewPort1(1, 2)), RGBTolerance: 70);
				if (viewer.VerifyViewPortIsActive(1, 2) && BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 1))// && Step5_2)
				{
					result.steps[ExecutedSteps].AddPassStatusList(logMessage: "1st thumbnail loaded in 2nd viewport in studypanel 1 using drag and drop");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList(logMessage: "1st thumbnail not loaded in 2nd viewport in studypanel 1 using drag and drop");
				}

				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 6- From the COMPARISON study panel thumbnail bar, repeat above step double click and drag&drop different thumbnails to different viewport within the related study.
				viewer.ClickOnViewPort(2, 1);
				viewer.ClickOnThumbnailsInStudyPanel(2, 2, doubleclick: true, isTestcompleteAction: true);
				PageLoadWait.WaitForLoadInViewport(10, BasePage.FindElementByCss(viewer.GetViewportCss(2, 0)));
				//result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				//bool Step6_1 = viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(viewer.SetViewPort1(2, 1)), RGBTolerance: 70);
				if (viewer.VerifyViewPortIsActive(2, 1) && BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(2, 2))// && Step6_1)
				{
					result.steps[++ExecutedSteps].AddPassStatusList(logMessage: "2nd thumbnail loaded in 1st viewport in studypanel 2 using double click");
				}
				else
				{
					result.steps[++ExecutedSteps].AddFailStatusList(logMessage: "2nd thumbnail not loaded in 1st viewport in studypanel 2 using double click");
				}

				viewer.DropAndDropThumbnails(thumbnailnumber: 1, viewport: 2, studyPanelNumber: 2, UseDragDrop: true);
				PageLoadWait.WaitForLoadInViewport(10, BasePage.FindElementByCss(viewer.GetViewportCss(2, 0)));

				
				//result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				//bool Step6_2 = viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(viewer.SetViewPort1(2, 2)), RGBTolerance: 70);
				if (viewer.VerifyViewPortIsActive(2, 2) && BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(2, 1))// && Step6_2)
				{
					result.steps[ExecutedSteps].AddPassStatusList(logMessage: "1st thumbnail loaded in 2nd viewport in studypanel 2 using drag and drop");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList(logMessage: "1st thumbnail not loaded in 2nd viewport in studypanel 2 using drag and drop");
				}

				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 7- Apply pan, zoom, ww/wl and measurement in the PRIMARY study panel on an image (e.g. PA image);
				viewer.ClickOnViewPort(1, 1);
				viewer.SelectViewerTool(BluRingTools.Window_Level);
				viewer.ApplyTool_WindowWidth();
				viewer.SelectViewerTool(BluRingTools.Pan);
				viewer.ApplyTool_Pan();
				viewer.SelectViewerTool(BluRingTools.Interactive_Zoom);
				viewer.ApplyTool_Zoom();
				viewer.SelectViewerTool(BluRingTools.Line_Measurement);
				var attributes = viewer.GetElementAttributes(viewer.Activeviewport);
				viewer.ApplyTool_LineMeasurement(attributes["width"] / 3, attributes["height"] / 3, attributes["width"] / 4, attributes["height"] / 4);
				viewer.SelectViewerTool(BluRingTools.Window_Level);
				PageLoadWait.WaitForLoadInViewport(10, BasePage.FindElementByCss(viewer.GetViewportCss(1, 0)));
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				if (viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(viewer.Activeviewport), RGBTolerance: 70))
				//if (viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(viewer.Activeviewport)))
				{
					result.steps[ExecutedSteps].AddPassStatusList(logMessage: "Apply pan, zoom, ww/wl and measurement in the PRIMARY study panel on an image");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList(logMessage: "Apply pan, zoom, ww/wl and measurement in the PRIMARY study panel on an image");
				}

				//repeat the same to apply measurement on an image (e.g. PA image) in the COMPARISON study panel.
				viewer.ClickOnViewPort(2, 1);
				viewer.SelectViewerTool(BluRingTools.Window_Level, panel: 2);
				viewer.ApplyTool_WindowWidth();
				viewer.SelectViewerTool(BluRingTools.Pan, panel: 2);
				viewer.ApplyTool_Pan();
				viewer.SelectViewerTool(BluRingTools.Interactive_Zoom, panel: 2);
				viewer.ApplyTool_Zoom();
				viewer.SelectViewerTool(BluRingTools.Line_Measurement, panel: 2);
				attributes = viewer.GetElementAttributes(viewer.Activeviewport);
				viewer.ApplyTool_LineMeasurement(attributes["width"] / 3, attributes["height"] / 3, attributes["width"] / 4, attributes["height"] / 4);
				viewer.SelectViewerTool(BluRingTools.Window_Level, panel: 2);
				PageLoadWait.WaitForLoadInViewport(10, BasePage.FindElementByCss(viewer.GetViewportCss(2, 0)));
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				if (viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(viewer.Activeviewport), RGBTolerance: 70)) 				
				{
					result.steps[ExecutedSteps].AddPassStatusList(logMessage: "Apply pan, zoom, ww/wl and measurement in the PRIMARY study panel on an image");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList(logMessage: "Apply pan, zoom, ww/wl and measurement in the PRIMARY study panel on an image");
				}

				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 8- In the Primary study panel, change layout to 2x2; From EXAM LIST thumbnail preview area, double click on a thumbnail to load an image (e.g. PA image) 
				//from the related study to the bottom left viewport of Primary study panel
				viewer.ChangeViewerLayout("2x2", 1);
				if (viewer.GetViewPortCount(panelnumber: 1) == 4)
				{
					result.steps[++ExecutedSteps].AddPassStatusList(logMessage: "PRIMARY viewport is set to 2x2 layout");
				}
				else
				{
					result.steps[++ExecutedSteps].AddFailStatusList(logMessage: "PRIMARY viewport is set to 2x2 layout");
				}
				//viewer.OpenExamListThumbnailPreview(accession: AccessionNumber[1]);
				viewer.ClickOnViewPort(1, 3);
				viewer.Doubleclick("cssselector", viewer.GetExamListThumbnailCss(viewer.GetPriorNumber(AccessionNumber[1]), 1), useTestComplete: true);
				PageLoadWait.WaitForLoadInViewport(10, BasePage.FindElementByCss(viewer.GetViewportCss(1, 2)));
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
				bool step8 = viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(viewer.Activeviewport));
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail") || !step8)
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 9- From EXAM LIST thumbnail preview area of the related study, drag&drop a thumbnail to load an image (e.g. LAT image) 
				//from the related study to the bottom right viewport of Primary study panel								
				viewer.DropAndDropThumbnails(thumbnailnumber: 2, viewport: 4, studyPanelNumber: 1, ExamList: true, UseDragDrop: true);
				PageLoadWait.WaitForLoadInViewport(10, BasePage.FindElementByCss(viewer.GetViewportCss(1, 3)));
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
				if (viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(viewer.Activeviewport), RGBTolerance: 70)) 
				{
					result.steps[ExecutedSteps].StepPass();
				}
				else
				{
					result.steps[ExecutedSteps].StepFail();
				}

				//Step 10- Close the COMPARISON study panel. Apply zoom, ww/wl and measurement on an image in the COMPARISON series viewport
				viewer.CloseStudypanel(2);
				if (viewer.GetStudyPanelCount() == 1)
				{
					result.steps[++ExecutedSteps].AddPassStatusList(logMessage: "Only 1 study panel displays in entire image display area");
				}
				else
				{
					result.steps[++ExecutedSteps].AddFailStatusList(logMessage: "Only 1 study panel displays in entire image display area");
				}
				viewer.ClickOnViewPort(1, 3);
				viewer.SelectViewerTool(BluRingTools.Window_Level, viewport: 3);
				viewer.ApplyTool_WindowWidth();				
				viewer.SelectViewerTool(BluRingTools.Interactive_Zoom, viewport: 3);
				viewer.ApplyTool_Zoom();
				viewer.SelectViewerTool(BluRingTools.Line_Measurement, viewport: 3);
				attributes = viewer.GetElementAttributes(viewer.Activeviewport);
				viewer.ApplyTool_LineMeasurement(attributes["width"] / 3, attributes["height"] / 3, attributes["width"] / 4, attributes["height"] / 4);
				viewer.SelectViewerTool(BluRingTools.Window_Level, viewport: 3);
				PageLoadWait.WaitForLoadInViewport(10, BasePage.FindElementByCss(viewer.GetViewportCss(1, 2)));
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
				if (viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(viewer.Activeviewport), RGBTolerance: 70))
				{
					result.steps[ExecutedSteps].AddPassStatusList(logMessage: "Apply pan, zoom, ww/wl and measurement in the PRIMARY study panel on an image");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList(logMessage: "Apply pan, zoom, ww/wl and measurement in the PRIMARY study panel on an image");
				}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 11- Double click on an image in PRIMARY viewport to show 1x1 layout, then double click on the image to switch back to original layout 2x2.
				viewer.Doubleclick("cssselector", viewer.GetViewportCss(1, 0), useTestComplete: true);
				if (viewer.GetViewPortCount(panelnumber: 1) == 1)
				{
					result.steps[++ExecutedSteps].AddPassStatusList(logMessage: "PRIMARY viewport is set to 1x1 layout");
				}
				else
				{
					result.steps[++ExecutedSteps].AddFailStatusList(logMessage: "PRIMARY viewport is not set to 1x1 layout");
				}
				viewer.Doubleclick("cssselector", viewer.GetViewportCss(1, 0), useTestComplete: true);
				PageLoadWait.WaitForAllViewportsToLoad(20, 1);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
				if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel(studyPanelIndex: 1), RGBTolerance: 70))
				{
					result.steps[ExecutedSteps].AddPassStatusList(logMessage: " Study panel shows 2 primary series at top row and 2 comparison series at bottom row.");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList(logMessage: "Study panel didn't shows 2 primary series at top row and 2 comparison series at bottom row.");
				}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 12- Double click on an image in COMPARISON viewport to show it in 1x1 layout, then double click it to switch back to original layout 2x2.
				viewer.Doubleclick("cssselector", viewer.GetViewportCss(1, 3), useTestComplete: true);
				if (viewer.GetViewPortCount(panelnumber: 1) == 1)
				{
					result.steps[++ExecutedSteps].AddPassStatusList(logMessage: "COMPARISON viewport is set to 1x1 layout");
				}
				else
				{
					result.steps[++ExecutedSteps].AddFailStatusList(logMessage: "COMPARISON viewport is not set to 1x1 layout");
				}
				viewer.Doubleclick("cssselector", viewer.GetViewportCss(1, 3), useTestComplete: true);
				PageLoadWait.WaitForAllViewportsToLoad(20, 1);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
				if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel(studyPanelIndex: 1), RGBTolerance: 70))
				{
					result.steps[ExecutedSteps].AddPassStatusList(logMessage: " Study panel shows 2 primary series at top row and 2 comparison series at bottom row.");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList(logMessage: "Study panel didn't shows 2 primary series at top row and 2 comparison series at bottom row.");
				}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 13- Select image viewport of the 2nd thumbnail series from thumbnail bar, change layouts to 1x1 from the Layout button
				viewer.ClickOnViewPort(1, 1);
				viewer.ChangeViewerLayout("1x1", 1);
				if (viewer.GetViewPortCount(panelnumber: 1) == 1)
				{
					result.steps[++ExecutedSteps].AddPassStatusList(logMessage: "PRIMARY viewport is set to 1x1 layout");
				}
				else
				{
					result.steps[++ExecutedSteps].AddFailStatusList(logMessage: "PRIMARY viewport is set to 1x1 layout");
				}
				//result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
				//if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel(studyPanelIndex: 1), RGBTolerance: 70))
				if(BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 2))
				{
					result.steps[ExecutedSteps].AddPassStatusList(logMessage: "Image from the 2nd thumbnail is displayed in 1x1 layout");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList(logMessage: "Image from the 2nd thumbnail is displayed in 1x1 layout");
				}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 14- Double click on the 2nd thumbnail in the thumbnail bar, then change layout to 2x2 using Layout button.
				viewer.Doubleclick("cssselector", viewer.GetStudyPanelThumbnailCss(2, 1), useTestComplete: true);
				viewer.ChangeViewerLayout("2x2", 1);
				viewer.OpenExamListThumbnailPreview(accession: AccessionNumber[0]);
				ExecutedSteps++;
				for (int thumbnail = 1; thumbnail <= BasePage.FindElementsByCss(BluRingViewer.div_studyPanelThumbnailImages).Count; thumbnail++)
				{
					if (thumbnail == 1)
					{
						if (!BluRingViewer.VerifyThumbnailsInExamList(thumbnail, "No Border"))
						{
							result.steps[ExecutedSteps].AddFailStatusList("Thumbnail no: 1 is displayed in 2x2 viewport");
						}
					}
					else if (thumbnail == 2)
					{
						if (!BluRingViewer.VerifyThumbnailsInExamList(thumbnail, "Active"))
						{
							result.steps[ExecutedSteps].AddFailStatusList("Thumbnail no: 2 is not active in 2x2 viewport");
						}
					}
					else if (!BluRingViewer.VerifyThumbnailsInExamList(thumbnail, "Visible"))
					{
						result.steps[ExecutedSteps].AddFailStatusList("Thumbnail no: " + thumbnail  + "not displayed in 2x2 viewport");
					}					
				}							
				if (viewer.VerifyPriorsHighlightedInExamList(AccessionNumber: AccessionNumber[0]))
				{
					result.steps[ExecutedSteps].AddPassStatusList(logMessage: "The PRIMARY study is selected in the EXAM PANEL");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList(logMessage: "The PRIMARY study is selected in the EXAM PANEL");
				}				
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 15- Change layout to 1x1. Load the primary study in the 2nd study panel
				viewer.ChangeViewerLayout("1x1", 1);
				if (viewer.GetViewPortCount(panelnumber: 1) == 1)
				{
					result.steps[++ExecutedSteps].AddPassStatusList(logMessage: "PRIMARY viewport is set to 1x1 layout");
				}
				else
				{
					result.steps[++ExecutedSteps].AddFailStatusList(logMessage: "PRIMARY viewport is not set to 1x1 layout");
				}
				viewer.OpenPriors(accession: AccessionNumber[0]);
				PageLoadWait.WaitForAllViewportsToLoad(20, 2);
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
				if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel(studyPanelIndex: 2), RGBTolerance: 70))
				{
					result.steps[ExecutedSteps].AddPassStatusList(logMessage: "	PRIMARY images are displayed in the 2nd study panel");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList(logMessage: "	PRIMARY images are not displayed in the 2nd study panel");
				}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 16- Open a related study in the 3rd study panel
				viewer.OpenPriors(accession: AccessionNumber[1]);
				PageLoadWait.WaitForAllViewportsToLoad(20, 3);
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
				if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel(studyPanelIndex: 3), RGBTolerance: 70))
				{
					result.steps[ExecutedSteps].AddPassStatusList(logMessage: "COMPARISON images are displayed in the 3rd study panel");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList(logMessage: "COMPARISON images are displayed in the 3rd study panel");
				}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 17- Open the primary study in the 4th study panel, then open the related study in the 5th study panel.
				viewer.OpenPriors(accession: AccessionNumber[0]);
				PageLoadWait.WaitForAllViewportsToLoad(20, 4);
				viewer.OpenPriors(accession: AccessionNumber[1]);
				PageLoadWait.WaitForAllViewportsToLoad(20, 5);
				result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel(studyPanelIndex: 4), RGBTolerance: 70))
				{
					result.steps[ExecutedSteps].AddPassStatusList(logMessage: "	PRIMARY Images are displayed in the 4th study panel");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList(logMessage: "	PRIMARY Images are not displayed in the 4th study panel");
				}
				result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel(studyPanelIndex: 5), RGBTolerance: 70))
				{
					result.steps[ExecutedSteps].AddPassStatusList(logMessage: "COMPARISON images are displayed in the 5th study panel");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList(logMessage: "COMPARISON images are displayed in the 5th study panel");
				}				
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 18- Attempt the load a study into the 6th study panel.
				viewer.OpenPriors(accession: AccessionNumber[0]);
				var popup = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_MaxStudyPanelPopup));
				bool IsPopup = popup.Displayed;
				bool IsErrorMessag = popup.FindElement(By.CssSelector(BluRingViewer.div_MaxStudyPanelErrorMessage)).
				   GetAttribute("innerHTML").Trim().Equals(warningmessage);
				if (IsPopup && IsErrorMessag)
				{
					result.steps[++ExecutedSteps].AddPassStatusList("Flag Value - IsPopup" + IsPopup + "Flag Value--IsErrorMessag" + IsErrorMessag);
					popup.FindElement(By.CssSelector("button")).Click();
					Thread.Sleep(3000);
				}
				else
				{
					result.steps[++ExecutedSteps].AddFailStatusList("Flag Value - IsPopup " + IsPopup + "Flag Value--IsErrorMessag " + IsErrorMessag);
				}
				if (viewer.GetStudyPanelCount() == 5)
				{
					result.steps[ExecutedSteps].AddPassStatusList("Total study Panel opened: " + viewer.GetStudyPanelCount());
				}
				else
				{
					result.steps[ExecutedSteps].AddPassStatusList("Total study Panel opened: " + viewer.GetStudyPanelCount());
				}

				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail(); 
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 19 - For the 1st Study Panel display series using mixed methods of double clicking and drag&drop a thumbnail from EXAM LIST:
				//a) Primary study 2nd thumbnail --> 1st viewport
				//viewer.OpenExamListThumbnailPreview(accession: AccessionNumber[0]);
				viewer.ClickOnViewPort(1, 1);
				viewer.ChangeViewerLayout("2x2", 1);
				PageLoadWait.WaitForThumbnailsToLoad(60);
				viewer.DropAndDropThumbnails(thumbnailnumber: 2, viewport: 1, studyPanelNumber: 1, ExamList: true, UseDragDrop: true);
				PageLoadWait.WaitForAllViewportsToLoad(60, 1);
				ExecutedSteps++;
				if (!viewer.VerifyViewPortIsActive(1, 1) && !BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 2))
				{
					result.steps[ExecutedSteps].AddFailStatusList("Primary study 2nd thumbnail --> 1st viewport");
				}

				//b) Primary study 1st thumbnail --> 2nd viewport
				viewer.ClickOnViewPort(1, 2);
				viewer.Doubleclick("cssselector", viewer.GetExamListThumbnailCss(viewer.GetPriorNumber(AccessionNumber[0]), 1), useTestComplete: true);
				PageLoadWait.WaitForAllViewportsToLoad(60, 1);
				if (!viewer.VerifyViewPortIsActive(1, 2) && !BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 1))
				{
					result.steps[ExecutedSteps].AddFailStatusList("Primary study 1st thumbnail --> 2nd viewport");
				}

				//c) related study 2nd thumbnail --> 3rd viewport
				viewer.OpenExamListThumbnailPreview(accession: AccessionNumber[1]);
				PageLoadWait.WaitForThumbnailsToLoad(60);
				viewer.ClickOnViewPort(1, 3);
				viewer.Doubleclick("cssselector", viewer.GetExamListThumbnailCss(viewer.GetPriorNumber(AccessionNumber[1]), 2), useTestComplete: true);
				PageLoadWait.WaitForAllViewportsToLoad(60, 1);
				if (!BluRingViewer.VerifyThumbnailsInExamList(thumbnail: 2, type: "Active"))
				{
					result.steps[ExecutedSteps].AddFailStatusList("Related study 2nd thumbnail --> 3rd viewport");
				}

				//d) related study 1st thumbnail --> 4th viewport
				viewer.ClickOnViewPort(1, 4);
				viewer.DropAndDropThumbnails(thumbnailnumber: 1, viewport: 4, studyPanelNumber: 1, ExamList: true, UseDragDrop: true);
				PageLoadWait.WaitForAllViewportsToLoad(60, 1);
				if (!BluRingViewer.VerifyThumbnailsInExamList(thumbnail: 1, type: "Active"))
				{
					result.steps[ExecutedSteps].AddFailStatusList("Related study 1st thumbnail --> 4th viewport");
				}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}


				//Step 20- For the 2nd study panel, change layout to 1x1, double click on the 2nd thumbnail in its thumbnail bar.
				viewer.ChangeViewerLayout("1x1", 2);
				viewer.ClickOnViewPort(2, 1);
				if (viewer.GetViewPortCount(panelnumber: 2) == 1)
				{
					result.steps[++ExecutedSteps].AddPassStatusList(logMessage: "2nd study panel, change layout to 1x1");
				}
				else
				{
					result.steps[++ExecutedSteps].AddFailStatusList(logMessage: "2nd study panel, change layout to 1x1");
				}
				viewer.Doubleclick("cssselector", viewer.GetStudyPanelThumbnailCss(2, 2), useTestComplete: true);
				PageLoadWait.WaitForAllViewportsToLoad(20, 2);
				//result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
				//if (viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(viewer.Activeviewport), RGBTolerance: 70))
				if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(2, 2))
				{
					result.steps[ExecutedSteps].AddPassStatusList(logMessage: "PRIMARY is displayed in the viewport.");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList(logMessage: "PRIMARY is displayed in the viewport.");
				}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 21- For the 3rd study panel, change layout to 1x1, double click on the 2nd thumbnail in its thumbnail bar.
				viewer.ChangeViewerLayout("1x1", 3);
				viewer.ClickOnViewPort(3, 1);
				if (viewer.GetViewPortCount(panelnumber: 3) == 1)
				{
					result.steps[++ExecutedSteps].AddPassStatusList(logMessage: "3rd study panel, change layout to 1x1");
				}
				else
				{
					result.steps[++ExecutedSteps].AddFailStatusList(logMessage: "3rd study panel, change layout to 1x1");
				}
				viewer.Doubleclick("cssselector", viewer.GetStudyPanelThumbnailCss(2, 3), useTestComplete: true);
				PageLoadWait.WaitForAllViewportsToLoad(20, 3);
				//result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
				//if (viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(viewer.Activeviewport), RGBTolerance: 70))
				if (BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(3, 2))
				{
					result.steps[ExecutedSteps].AddPassStatusList(logMessage: "COMPARISON is displayed in the viewport.");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList(logMessage: "COMPARISON is displayed in the viewport.");
				}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}

				//Step 22- For the 4th study panel, change layout to 1x1, drag&drop the 1st thumbnail of the primary study to viewport; 
				viewer.ChangeViewerLayout("1x1", 4);
				viewer.ClickOnViewPort(4, 1);
				if (viewer.GetViewPortCount(panelnumber: 4) == 1)
				{
					result.steps[++ExecutedSteps].AddPassStatusList(logMessage: "4th study panel, change layout to 1x1");
				}
				else
				{
					result.steps[++ExecutedSteps].AddFailStatusList(logMessage: "4th study panel, change layout to 1x1");
				}
				viewer.DropAndDropThumbnails(thumbnailnumber: 1, viewport: 1, studyPanelNumber: 4, UseDragDrop: true);
				PageLoadWait.WaitForAllViewportsToLoad(20, 4);
				if (!BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(4, 1))
				{
					result.steps[ExecutedSteps].AddFailStatusList("1st thumbnail is not displayed in 1x1 layout in the 4th study panel for the primary study");
				}
				//result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
				//if (viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(viewer.Activeviewport), RGBTolerance: 70))
				//{
				//	result.steps[ExecutedSteps].AddPassStatusList(logMessage: "PRIMARY is displayed in the viewport.");
				//}
				//else
				//{
				//	result.steps[ExecutedSteps].AddFailStatusList(logMessage: "PRIMARY is not displayed in the viewport.");
				//}				

				//In the 5th study panel repeat drag&drop 1st thumbnail for the COMPARISON study
				viewer.ChangeViewerLayout("1x1", 5);
				viewer.ClickOnViewPort(5, 1);
				if (viewer.GetViewPortCount(panelnumber: 5) == 1)
				{
					result.steps[ExecutedSteps].AddPassStatusList(logMessage: "5th study panel, change layout to 1x1");
				}
				else
				{
					result.steps[ExecutedSteps].AddFailStatusList(logMessage: "5th study panel, change layout to 1x1");
				}
				viewer.DropAndDropThumbnails(thumbnailnumber: 1, viewport: 1, studyPanelNumber: 5, UseDragDrop: true);
				PageLoadWait.WaitForAllViewportsToLoad(20, 5);
				if (!BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(5, 1))
				{
					result.steps[ExecutedSteps].AddFailStatusList("1st thumbnail is not displayed in 1x1 layout in the 5th study panel for the primary study");
				}
				//result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
				//if (viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(viewer.Activeviewport), RGBTolerance: 70))
				//{
				//	result.steps[ExecutedSteps].AddPassStatusList(logMessage: "COMPARISON is displayed in the viewport.");
				//}
				//else
				//{
				//	result.steps[ExecutedSteps].AddFailStatusList(logMessage: "COMPARISON is not displayed in the viewport.");
				//}
				if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
				{
					result.steps[ExecutedSteps].StepFail();
				}
				else
				{
					result.steps[ExecutedSteps].StepPass();
				}				

				//Logout
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
				BasePage.SetVMResolution("1280", "1024");
			}
		}		

        /// <summary> 
        /// Verify Measurement tools on US modality when Thumbnail Splitting = Image
        /// </summary>
        public TestCaseResult Test_164773(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                string[] FullPath;
                string PatinetName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                string[] PatinetNameList = PatinetName.Split('@');

                string FileUploadpath = (string)ReadExcel.GetTestData(filepath, "TestData", testid, "FileUploadPath");
                FileUploadpath = Config.TestDataPath + FileUploadpath;

                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                Taskbar bar = new Taskbar();

                //Pre-Condition - Send Study to the Temp EA. 
                var client = new DicomClient();
                string[] folderList = Directory.GetDirectories(FileUploadpath);
                foreach (string folderName in folderList)
                {
                    FullPath = Directory.GetFiles(folderName, "*.*", SearchOption.AllDirectories);
                    foreach (string path in FullPath)
                    {
                        client.AddRequest(new DicomCStoreRequest(path));
                        client.Send(Config.EA96, 12000, false, "SCU", Config.EA96AETitle);
                    }
                }

                //Step 1
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                result.steps[++ExecutedSteps].StepPass();

                //Step 2
                Studies studieSearch = (Studies)login.Navigate("Studies");
                studieSearch.SearchStudy(LastName: PatinetNameList[0], Datasource: login.GetHostName(Config.EA91));
                studieSearch.SelectStudy("Patient Name", PatinetNameList[0] + ", ");
                BluRingViewer Viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //Step 3
                Viewer.ClickOnViewPort(1, 1);
                Viewer.OpenViewerToolsPOPUp();

                String text = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewportToolbox + " "
                    + BluRingViewer.div_toolWrapper + "[title='" + BluRingViewer.GetToolName(BluRingTools.Line_Measurement) + "']")).GetAttribute("class");
                if (text.Contains("tool-disabled"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 4
                Viewer.CloseBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //Step 5 
                studieSearch.SearchStudy(LastName: PatinetNameList[1]);
                studieSearch.SelectStudy("Patient Name", PatinetNameList[1] + ", ");
                Viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //Step 6
                Viewer.ClickOnViewPort(1, 1);
                Viewer.OpenViewerToolsPOPUp();
                text = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewportToolbox + " "
                  + BluRingViewer.div_toolWrapper + "[title='" + BluRingViewer.GetToolName(BluRingTools.Line_Measurement) + "']")).GetAttribute("class");
                Viewer.SelectViewerTool(BluRingTools.Line_Measurement, isOpenToolsPOPup: false);
                Viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_6 = Viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(Viewer.Activeviewport));
                if (step_6 && !text.Contains("tool-disabled"))
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //Step 7
                Viewer.CloseBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //Step 8
                studieSearch.SearchStudy(LastName: PatinetNameList[2], Modality: "US");
                studieSearch.SelectStudy("Patient Name", PatinetNameList[2] + ", ");
                Viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                //Step 9
                viewer.ChangeViewerLayout("3x3", 1, 6);
                Thread.Sleep(5000);
                var action = new Actions(BasePage.Driver);
                action.MoveToElement(BasePage.FindElementByCss(Viewer.SetViewPort1(1, 7))).DoubleClick().Build().Perform();
                Thread.Sleep(5000);
                //Viewer.ClickOnViewPort(1, 7);
                //viewer.DoubleClick(BasePage.FindElementByCss(viewer.SetViewPort1(1,6)));
                Viewer.OpenViewerToolsPOPUp();
                text = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_viewportToolbox + " "
                 + BluRingViewer.div_toolWrapper + "[title='" + BluRingViewer.GetToolName(BluRingTools.Line_Measurement) + "']")).GetAttribute("class");
                if (text.Contains("tool-disabled"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Viewer.SelectViewerTool(BluRingTools.Window_Level, isOpenToolsPOPup: false, panel: 1, viewport: 7);

                //Step 10
                Viewer.ChangeViewerLayout("5x5");
                BluRingViewer.WaitforViewports();
                Viewer.ClickOnViewPort(1, 21);

                var action2 = new Actions(BasePage.Driver);
                action2.MoveToElement(BasePage.FindElementByCss(Viewer.SetViewPort1(1, 21))).DoubleClick().Build().Perform();
                Thread.Sleep(5000);
                Viewer.OpenViewerToolsPOPUp();
                Thread.Sleep(5000);
                text = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_viewportToolbox + " "
                + BluRingViewer.div_toolWrapper + "[title='" + BluRingViewer.GetToolName(BluRingTools.Line_Measurement) + "']"))[1].GetAttribute("class");
				Viewer.SelectViewerTool(BluRingTools.Line_Measurement, isOpenToolsPOPup: false, panel: 1, viewport: 21);
                Viewer.SetViewPort1(1, 21);
                Viewer.ApplyTool_LineMeasurement();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step_9 = Viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(Viewer.SetViewPort1(1, 21)), isCaptureScreen: true);
                if (step_9 && !text.Contains("tool-disabled"))
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                Viewer.CloseBluRingViewer();
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
        /// Workflow - open series from thumbnail bar and EXAM LIST thumbnail preview area (MR, CT)
        /// </summary>
        public TestCaseResult Test_164742(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                string PatinetName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                string[] PatinetNameList = PatinetName.Split('@');
                string patinetId = "00-001-002";
                string Description = "pelvis";
                String warningmessage = @"You have reached the maximum number of study viewer panels. Close a study viewer panel to open the selected study in a new panel.";

                string FileUploadpath = (string)ReadExcel.GetTestData(filepath, "TestData", testid, "FileUploadPath");
                FileUploadpath = Config.TestDataPath + FileUploadpath;
                string[] FullPath;
                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                Taskbar bar = new Taskbar();


                FileUploadpath = @"T:\ThumbnailSplitting\20050829_CT";
                //Pre-Condition - Send Study to the Temp EA. 
                var client = new DicomClient();
                string[] folderList = Directory.GetDirectories(FileUploadpath);
                foreach (string folderName in folderList)
                {
                    FullPath = Directory.GetFiles(folderName, "*.*", SearchOption.AllDirectories);
                    foreach (string path in FullPath)
                    {
                        client.AddRequest(new DicomCStoreRequest(path));
                        client.Send(Config.EA96, 12000, false, "SCU", Config.EA96AETitle);
                    }
                }

                //Step 1
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                Studies studieSearch = (Studies)login.Navigate("Studies");
                studieSearch.SearchStudy(patientID: patinetId, Description: Description, Datasource: login.GetHostName(Config.EA96));
                studieSearch.SelectStudy("Patient ID", patinetId);
                BluRingViewer Viewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].StepPass();

                IList<string> Modality = new List<string>(); IList<string> PerViewed = new List<string>(); IList<string> Caption = new List<string>(); IList<string> TotalImages = new List<string>();
                foreach (IWebElement thumbnails in BasePage.FindElementsByCss(BluRingViewer.div_thumbnails))
                {
                    Modality.Add(thumbnails.FindElement(By.CssSelector(BluRingViewer.div_thumbnailModality)).Text);
                    PerViewed.Add(thumbnails.FindElement(By.CssSelector(BluRingViewer.div_thumbnailPercentImagesViewed)).Text);
                    TotalImages.Add(thumbnails.FindElement(By.CssSelector(BluRingViewer.div_imageFrameNumber)).Text);
                    Caption.Add(thumbnails.FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption)).Text);
                }

                //Step 2
                result.steps[++ExecutedSteps].AddPassStatusList();
                int priorCount = 0;
                foreach (IWebElement prior in BasePage.FindElementsByCss(BluRingViewer.div_priors))
                {
                    Viewer.OpenExamListThumbnailPreview(priorCount);
                    for (int thumbnail = 1; thumbnail <= BasePage.FindElementsByCss(BluRingViewer.div_ExamList_thumbnails).Count; thumbnail++)
                    {
                        if (thumbnail == 1 && priorCount == 0)
                        {
                            if (!BluRingViewer.VerifyThumbnailsInExamList(thumbnail, "Active"))
                            {
                                result.steps[ExecutedSteps].AddFailStatusList();
                            }
                        }
                        else if (priorCount == 0)
                        {
                            if (!BluRingViewer.VerifyThumbnailsInExamList(thumbnail, "Visible"))
                            {
                                result.steps[ExecutedSteps].AddFailStatusList();
                            }
                        }
                        if (priorCount != 0)
                        {
                            if (!BluRingViewer.VerifyThumbnailsInExamList(1, "No Border"))
                                result.steps[ExecutedSteps].AddFailStatusList();
                        }
                    }
                    priorCount++;
                }

                Viewer.OpenExamListThumbnailPreview(0);
                IList<string> CaptionInThumbnailPreview = new List<string>();
                foreach (IWebElement thumbnails in BasePage.FindElementsByCss(BluRingViewer.div_ExamList_thumbnails))
                    CaptionInThumbnailPreview.Add(thumbnails.FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption)).Text);
                if (!CaptionInThumbnailPreview.SequenceEqual(Caption))
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                    result.steps[ExecutedSteps].StepFail();
                else
                    result.steps[ExecutedSteps].StepPass();


                //stepcount 3
                Viewer.ClickOnThumbnailsInStudyPanel(1, 2, true, true);
                if (Viewer.VerifyViewPortIsActive(1, 1) && BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 2))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 4
                IWebElement tempThumbnail = BasePage.FindElementsByCss(BluRingViewer.div_studyPanelThumbnailImages)[1];
                IWebElement percentViewedElement = tempThumbnail.FindElement(By.CssSelector(BluRingViewer.div_thumbnailPercentImagesViewed));
                int TotalImage = int.Parse(tempThumbnail.FindElement(By.CssSelector(BluRingViewer.div_imageFrameNumber)).Text);
                var viewport = BasePage.FindElementByCss(Viewer.Activeviewport);
                TestCompleteAction Action = new TestCompleteAction();
                Action.MouseScroll(viewport, "down", "18").Perform();
                bool step4 = Viewer.VerifyThumbnailPercentImagesViewed(percentViewedElement, TotalImage, 19);
                Action = new TestCompleteAction();
                Action.MouseScroll(viewport, "down", "58").Perform();
                BasePage.wait.Until<Boolean>((d) =>
                {
                    if (Viewer.VerifyThumbnailPercentImagesViewed(percentViewedElement, TotalImage, 75))
                        return true;
                    else
                    {
                        Action.MouseScroll(viewport, "down", "").Perform();
                        return false;
                    }
                });

                if (step4 && Viewer.VerifyThumbnailPercentImagesViewed(percentViewedElement, TotalImage, 75))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 5
                Viewer.DropAndDropThumbnails(thumbnailnumber: 3, viewport: 1, studyPanelNumber: 1, UseDragDrop: true);
                if (Viewer.VerifyViewPortIsActive(1, 1) && BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(1, 3))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }


                //Step 6
                Viewer.SelectViewerTool(BluRingTools.Window_Level);
                Viewer.ApplyTool_WindowWidth();
                Viewer.SelectViewerTool(BluRingTools.Pan);
                Viewer.ApplyTool_Pan();
                Viewer.SelectViewerTool(BluRingTools.Interactive_Zoom);
                Viewer.ApplyTool_Zoom();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool Step_6 = Viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(Viewer.Activeviewport));
                if (Step_6)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }


                //Step 7
                Viewer.PlayCINE(1, 1);
                Thread.Sleep(3000);
                Viewer.PauseCINE(1, 1);
                string ThumbnailPercentage1 = BasePage.FindElementsByCss(BluRingViewer.div_studyPanelThumbnailImages)[2].FindElement(By.CssSelector(BluRingViewer.div_thumbnailPercentImagesViewed)).Text;
                Viewer.PlayCINE(1, 1);
                Thread.Sleep(3000);
                Viewer.PauseCINE(1, 1);
                string ThumbnailPercentage2 = BasePage.FindElementsByCss(BluRingViewer.div_studyPanelThumbnailImages)[2].FindElement(By.CssSelector(BluRingViewer.div_thumbnailPercentImagesViewed)).Text;
                WebDriverWait tempwait = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 10, 0));
                tempwait.Until<Boolean>((d) =>
                {
                    Viewer.PlayCINE(1, 1);
                    Thread.Sleep(20000);
                    Viewer.PauseCINE(1, 1);
                    if (BasePage.FindElementsByCss(BluRingViewer.div_studyPanelThumbnailImages)[2].FindElement(By.CssSelector(BluRingViewer.div_thumbnailPercentImagesViewed)).Text == "100%")
                        return true;
                    else
                        return false;
                });

                if ((ThumbnailPercentage1 != ThumbnailPercentage2) && BasePage.FindElementsByCss(BluRingViewer.div_studyPanelThumbnailImages)[2].FindElement(By.CssSelector(BluRingViewer.div_thumbnailPercentImagesViewed)).Text == "100%")
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }


                //Step 8
                if (!Viewer.IsCINEPlaying())
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }


                //Step 9
                Viewer.OpenPriors(1);
                bool Step9_1 = Viewer.IsElementVisible(By.CssSelector(BluRingViewer.div_studypanel + ":nth-of-type(2)"));
                Viewer.OpenExamListThumbnailPreview(1);
                result.steps[++ExecutedSteps].AddPassStatusList();
                for (int thumbnail = 1; thumbnail <= BasePage.FindElementsByCss(BluRingViewer.div_ExamList_thumbnails).Count; thumbnail++)
                {
                    if (thumbnail == 1)
                    {
                        if (!BluRingViewer.VerifyThumbnailsInExamList(thumbnail, "Active"))
                        {
                            result.steps[ExecutedSteps].AddFailStatusList();
                        }
                    }
                    else if (thumbnail <= 4)
                    {
                        if (!BluRingViewer.VerifyThumbnailsInExamList(thumbnail, "Visible"))
                        {
                            result.steps[ExecutedSteps].AddFailStatusList();
                        }
                    }
                    else if (!BluRingViewer.VerifyThumbnailsInExamList(thumbnail, "No Border"))
                        result.steps[ExecutedSteps].AddFailStatusList();
                }

                if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Step 10
                result.steps[++ExecutedSteps].AddPassStatusList();
                Viewer.ClickOnThumbnailsInStudyPanel(2, 2, true, true);
                if (!Viewer.VerifyViewPortIsActive(2, 1) || !BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(2, 2))
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool Step10_1 = Viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(Viewer.SetViewPort1(2, 1)));

                Viewer.DropAndDropThumbnails(thumbnailnumber: 3, viewport: 1, studyPanelNumber: 2, UseDragDrop: true);
                if (!Viewer.VerifyViewPortIsActive(2, 1) || !BluRingViewer.VerifyThumbnailsInStudyPanelIsActive(2, 3))
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 2);
                bool Step10_2 = Viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(Viewer.SetViewPort1(2, 1)));

                if (result.steps[ExecutedSteps].statuslist.Any<string>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Step 11
                Viewer.SetViewPort1(1, 1);
                Viewer.ClickOnViewPort(1, 1);
                Viewer.SelectViewerTool(BluRingTools.Reset);
                Viewer.SelectViewerTool(BluRingTools.Window_Level);
                Viewer.ApplyTool_WindowWidth();
                Viewer.SelectViewerTool(BluRingTools.Pan);
                Viewer.ApplyTool_Pan();
                Viewer.SelectViewerTool(BluRingTools.Interactive_Zoom);
                Viewer.ApplyTool_Zoom();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool Step11_1 = Viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(Viewer.Activeviewport));

                Viewer.SetViewPort1(2, 1);
                Viewer.ClickOnViewPort(2, 1);
                Viewer.SelectViewerTool(BluRingTools.Reset, 2, 1);
                Viewer.SelectViewerTool(BluRingTools.Window_Level, 2, 1);
                Viewer.ApplyTool_WindowWidth();
                Viewer.SelectViewerTool(BluRingTools.Pan, 2, 1);
                Viewer.ApplyTool_Pan();
                Viewer.SelectViewerTool(BluRingTools.Interactive_Zoom, 2, 1);
                Viewer.ApplyTool_Zoom();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 2);
                bool Step11_2 = Viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(Viewer.Activeviewport));

                if (Step11_1 && Step11_2)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //Step 12
                Viewer.ChangeViewerLayout("3x2", 1);
                Viewer.OpenExamListThumbnailPreview(1);
                Viewer.SetViewPort1(1, 3);
                Viewer.ClickOnViewPort(1, 3);
                Viewer.Doubleclick("cssselector", Viewer.GetExamListThumbnailCss(2, 1), useTestComplete: true);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool Step12_1 = Viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(Viewer.Activeviewport));
                if (Step12_1)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //Step 13
                Viewer.SetViewPort1(1, 4);
                Viewer.ClickOnViewPort(1, 4);
                Viewer.DropAndDropThumbnails(thumbnailnumber: 2, viewport: 4, studyPanelNumber: 1, ExamList: true, UseDragDrop: true);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool Step13_1 = Viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(Viewer.Activeviewport));
                if (Step13_1)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //Step 14
                Viewer.OpenExamListThumbnailPreview(2);
                Viewer.SetViewPort1(1, 5);
                Viewer.ClickOnViewPort(1, 5);
                Viewer.Doubleclick("cssselector", Viewer.GetExamListThumbnailCss(3, 1), useTestComplete: true);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool Step14_1 = Viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(Viewer.Activeviewport));

                Viewer.SetViewPort1(1, 6);
                Viewer.ClickOnViewPort(1, 6);
                Viewer.DropAndDropThumbnails(thumbnailnumber: 2, viewport: 6, studyPanelNumber: 1, ExamList: true, UseDragDrop: true);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 2);
                bool Step14_2 = Viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(Viewer.Activeviewport));
                if (Step14_1 && Step14_2)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //Step 15
                for (int i = Viewer.GetStudyPanelCount(); i > 1; i--)
                    Viewer.CloseStudypanel(i);
                bool step15_1 = Viewer.GetStudyPanelCount() == 1;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step15_2 = Viewer.CompareImage(result.steps[ExecutedSteps], Viewer.StudyPanelList.ToArray()[0]);

                Viewer.SetViewPort1(1, 3);
                Viewer.ClickOnViewPort(1, 3);
                Viewer.SelectViewerTool(BluRingTools.Window_Level, 1, 3);
                Viewer.ApplyTool_WindowWidth();
                Viewer.SelectViewerTool(BluRingTools.Pan, 1, 3);
                Viewer.ApplyTool_Pan();
                Viewer.SelectViewerTool(BluRingTools.Interactive_Zoom, 1, 3);
                Viewer.ApplyTool_Zoom();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 2);
                bool step15_3 = Viewer.CompareImage(result.steps[ExecutedSteps], Viewer.StudyPanelList.ToArray()[0]);
                if (step15_1 && step15_2 && step15_3)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //Step 16
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step16_1 = Viewer.CompareImage(result.steps[ExecutedSteps], Viewer.StudyPanelList.ToArray()[0]);
                if (step16_1)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //Step 17
                //Viewer.ClickOnViewPort(1, 1);
                viewer.Doubleclick("cssselector", viewer.GetViewportCss(1, 0), useTestComplete: true);
                PageLoadWait.WaitForAllViewportsToLoad(20, 1);
                //var action = new Actions(BasePage.Driver);
                //action.DoubleClick(Viewer.ClickOnViewPort(1, 1)).Build().Perform();
                bool step17_1 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels + " .viewerContainerComponent.shown")).Count == 1;

                Thread.Sleep(3000);
                viewer.Doubleclick("cssselector", viewer.GetViewportCss(1, 0), useTestComplete: true);
                PageLoadWait.WaitForAllViewportsToLoad(20, 1);
                //new Actions(BasePage.Driver).DoubleClick(Viewer.ClickOnViewPort(1, 1)).Build().Perform();
                bool step17_2 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels + " .viewerContainerComponent.shown")).Count == 6;
                if (step17_2 != true)
                {
                    new Actions(BasePage.Driver).DoubleClick(Viewer.ClickOnViewPort(1, 1)).Build().Perform();
                    step17_2 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels + " .viewerContainerComponent.shown")).Count == 6;
                }

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step17_3 = Viewer.CompareImage(result.steps[ExecutedSteps], Viewer.StudyPanelList.ToArray()[0]);
                if (step17_1 && step17_2 && step17_3)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //Step 18
                viewer.Doubleclick("cssselector", viewer.GetViewportCss(1, 2), useTestComplete: true);
                PageLoadWait.WaitForAllViewportsToLoad(20, 1);
                //Viewer.ClickOnViewPort(1, 3);
                //action = new Actions(BasePage.Driver);
                //action.DoubleClick(Viewer.ClickOnViewPort(1, 3)).Build().Perform();
                bool step18_1 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels + " .viewerContainerComponent.shown")).Count == 1;

                Thread.Sleep(3000);
                viewer.Doubleclick("cssselector", viewer.GetViewportCss(1, 2), useTestComplete: true);
                //new Actions(BasePage.Driver).DoubleClick(Viewer.ClickOnViewPort(1, 3)).Build().Perform();
                bool step18_2 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels + " .viewerContainerComponent.shown")).Count == 6;
                if (step18_2 != true)
                {
                    new Actions(BasePage.Driver).DoubleClick(Viewer.ClickOnViewPort(1, 3)).Build().Perform();
                    step18_2 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_Panel(1) + BluRingViewer.div_ViewportPanels + " .viewerContainerComponent.shown")).Count == 1;
                }

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step18_3 = Viewer.CompareImage(result.steps[ExecutedSteps], Viewer.StudyPanelList.ToArray()[0]);
                if (step18_1 && step18_2 && step18_3)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }


                //Step 19
                Viewer.ClickOnViewPort(1, 1);
                Viewer.ChangeViewerLayout("1x1");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step19_1 = Viewer.CompareImage(result.steps[ExecutedSteps], BasePage.FindElementByCss(Viewer.Activeviewport));
                if (step19_1)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }


                //Step20
                Viewer.ChangeViewerLayout("2x2");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step20_1 = Viewer.CompareImage(result.steps[ExecutedSteps], Viewer.StudyPanelList.ToArray()[0]);
                if (step20_1)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //step 21
                Viewer.ChangeViewerLayout("3x2");
                Viewer.OpenPriors(0);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step21_1 = Viewer.CompareImage(result.steps[ExecutedSteps], Viewer.StudyPanelList.ToArray()[1]);
                if (step21_1)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //Step22
                Viewer.OpenPriors(1);
                Viewer.OpenPriors(1);
                Viewer.OpenPriors(2);
                int studycountBefore = Viewer.GetStudyPanelCount();
                if (studycountBefore == 5)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 23
                Viewer.OpenPriors(2);
                var popup = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_MaxStudyPanelPopup));
                bool IsPopup = popup.Displayed;
                bool IsErrorMessag = popup.FindElement(By.CssSelector(BluRingViewer.div_MaxStudyPanelErrorMessage)).
                   GetAttribute("innerHTML").Trim().Equals(warningmessage);
                if (IsPopup && IsErrorMessag)
                {
                    popup.FindElement(By.CssSelector("button")).Click();
                    Thread.Sleep(3000);
                }
                int studycountAfter = Viewer.GetStudyPanelCount();
                if (studycountAfter == studycountBefore && IsPopup && IsErrorMessag)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 24
                Viewer.OpenExamListThumbnailPreview(1);
                Viewer.ClickOnViewPort(1, 3);
                Viewer.Doubleclick("cssselector", Viewer.GetExamListThumbnailCss(1, 1), useTestComplete: true);
                Viewer.DropAndDropThumbnails(thumbnailnumber: 2, viewport: 4, studyPanelNumber: 1, ExamList: true, UseDragDrop: true);

                Viewer.OpenExamListThumbnailPreview(2);
                Viewer.ClickOnViewPort(1, 5);
                Viewer.Doubleclick("cssselector", Viewer.GetExamListThumbnailCss(1, 1), useTestComplete: true);
                Viewer.DropAndDropThumbnails(thumbnailnumber: 2, viewport: 6, studyPanelNumber: 1, ExamList: true, UseDragDrop: true);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step24_1 = Viewer.CompareImage(result.steps[ExecutedSteps], Viewer.StudyPanelList.ToArray()[0]);
                if (step24_1)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //Step 25
                Viewer.ChangeViewerLayout("1x1", 2);
                Viewer.ClickOnViewPort(1, 1);
                Viewer.ClickOnThumbnailsInStudyPanel(3, 1, true, true);
                Viewer.ClickOnViewPort(3, 1);
                Viewer.ClickOnThumbnailsInStudyPanel(1, 1, true, true);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step25_1 = Viewer.CompareImage(result.steps[ExecutedSteps], Viewer.StudyPanelList.ToArray()[0]);

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 2);
                bool step25_2 = Viewer.CompareImage(result.steps[ExecutedSteps], Viewer.StudyPanelList.ToArray()[2]);
                if (step25_1 && step25_2)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //step 26
                Viewer.DropAndDropForeignThumbnails(sourceStudyPanelNumber: 3, sourceThumbnailNumber: 1, destinationStudyPanelNumber: 1, destinationViewport: 1, UseDragDrop: true);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step26_1 = Viewer.CompareImage(result.steps[ExecutedSteps], Viewer.StudyPanelList.ToArray()[0]);

                Viewer.DropAndDropForeignThumbnails(sourceStudyPanelNumber: 1, sourceThumbnailNumber: 1, destinationStudyPanelNumber: 3, destinationViewport: 1, UseDragDrop: true);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step26_2 = Viewer.CompareImage(result.steps[ExecutedSteps], Viewer.StudyPanelList.ToArray()[1]);

                if (step26_1 && step26_2)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }


                //Step 27.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step27_1 = Viewer.CompareImage(result.steps[ExecutedSteps], Viewer.StudyPanelList.ToArray()[0]);

                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 2);
                bool step27_2 = Viewer.CompareImage(result.steps[ExecutedSteps], Viewer.StudyPanelList.ToArray()[2]);
                if (step27_1 && step27_2)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }


                Viewer.CloseBluRingViewer();
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


    }

}
