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

namespace Selenium.Scripts.Tests
{
    class EnhancedThumbnail
    {

        public Login login { get; set; }
        public string filepath { get; set; }


        String seriesDomain = "ThumbnailsSeriesDomain" + new Random().Next(1000);
        String seriesRole = "ThumbnailsSeriesRole" + new Random().Next(1000);
        String ImageDomain = "ThumbnailsImagesDomain" + new Random().Next(1000);
        String ImageRole = "ThumbnailsImagesRole" + new Random().Next(1000);
        String roleName = "User_" + new Random().Next(1000);
        String userId = "User1" + new Random().Next(1000);
        public string EA_91 = "VMSSA-5-38-91";
        public string EA_131 = "VMSSA-4-38-131";
        public string PACS_A7 = "PA-A7-WS8";

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        public EnhancedThumbnail(String classname)
        {
            login = new Login();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        

        /// <summary> 
        /// This Test Case is Initial Setups for Enhanced thumbnail VP
        /// </summary>

        public TestCaseResult Test_27898(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Pre-Conditions- Step-1
                //1. No changes to the thumbnail splitting rule for any modality have been made at the domain level (system default domain).
                //2. If there were changes at the domain level, bring them to the default settings. The default settings are-    
                //Thumbnail Splitting -  'Series' for CT, MR, NM, PT, RF, VL,RTIMAGE.   
                //Thumbnail Splitting -  'Image' for all the other modalities.  
                //3. Thumbnail caption overlay is set to true (checked) by default in domain settings, 
                //i.e. we display the thumbnail caption overlaid on image vs. underneath thumbnail image. 
                //If 'Display Thumbnail Caption As Overlay' setting has been modified, 
                //changed it back to true (put a check mark in the check box), save the changes.  
                //4. Client Browser to be used in this execution should be consistent to the latest version of Functional Product Specification for iConnect Access.

                String username = Config.adminUserName;
                String password = Config.adminPassword;

                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                Taskbar bar = new Taskbar();
                bar.Hide();

                st.LaunchServiceTool();
                st.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(2);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);

                string[] SeriesMod = { "CT", "MR", "NM", "PT", "RF", "VL", "RTIMAGE" };

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
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();

                // precondition -2
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain("SuperAdminGroup");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();
                //Select Overlay checkbox
                if (!domain.OverlayCheckbox().Selected)
                {
                    domain.OverlayCheckbox().Click();
                    Logger.Instance.InfoLog("Overlay Checkbox is selected successfully");
                }
                else
                    Logger.Instance.InfoLog("Overlay Checkbox already selected");

                domain.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(30);
                login.Logout();
                ExecutedSteps++;

                //------------End of script---
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

                //Return Result
                return result;
            }
        }


        /// <summary> 
        /// This Test Case is verification of Thumbnail in "Image-level"
        /// </summary>

        public TestCaseResult Test_27899(String testid, String teststeps, int stepcount)
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

                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String[] LastNames = LastNameList.Split(':');

                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNoList.Split(':');

                String Patient_id = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] Patient_ids = Patient_id.Split(':');

                String ValidationList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Validation");
                String[] Validation = ValidationList.Split(':');

                Taskbar bar = new Taskbar();
                bar.Hide();

                //Step-1
                //----Precondition completed----
                ExecutedSteps++;

                //Step-2
                //In Merge ICA Service Tool -> Viewer tab-> Protocols sub-tab -> select Modify button 
                //and verify the default thumbnail configuration for each modality.

                //1. The default thumbnail configuration for the image modalities 
                //CR, DX, RG, MG, US, XA and OT (secondary capture) should be image-scope.
                //2. For all other modalities the default configuration shall be series-scope.

                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                Boolean res2 = true;

                st.LaunchServiceTool();
                st.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(2);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);

                string[] SeriesMod = { "CT", "MR", "NM", "PT", "RF", "VL", "RTIMAGE" };
                var comboBox_mod = wpfobject.GetComboBox("ComboBox_Modality");

                int count = comboBox_mod.Items.Count;
                for (int i = 0; i < count; i++)
                {
                    comboBox_mod.Select(i);
                    if ((SeriesMod.Contains(comboBox_mod.SelectedItemText)))
                    {
                        if (wpfobject.IsRadioBtnSelected("RB_ThumbnailSplittingSeries"))
                        {
                            Logger.Instance.InfoLog("Modality " + comboBox_mod.SelectedItemText + " Series selected - Verified successfully");
                        }
                        else
                        {
                            res2 = false;
                            Logger.Instance.ErrorLog("Modality " + comboBox_mod.SelectedItemText + " Series not selected - verified failed");
                        }
                    }
                    else
                    {
                        if (wpfobject.IsRadioBtnSelected("RB_ThumbnailSplittingName"))
                        {
                            Logger.Instance.InfoLog("Modality " + comboBox_mod.SelectedItemText + " Image selected - Verified Successfully");
                        }
                        else
                        {
                            res2 = false;
                            Logger.Instance.ErrorLog("Modality " + comboBox_mod.SelectedItemText + " Image not selected verified failed");
                        }

                    }
                    wpfobject.WaitTillLoad();
                }

                if (res2)
                {
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
                //Configure the Thumbnail Splitting to be 'Image' for all the modalities.  
                //Save the changes.  Restart IIS.

                for (int i = 0; i < count; i++)
                {
                    comboBox_mod.Select(i);
                    wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingName");
                    wpfobject.WaitTillLoad();
                }
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //-Step-4
                //Log in the client machine   
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //--Step-5
                //Test Data: Note: the User Preferences setting may have different setting than the Domain settings.  
                //In this case User Preferences has precedence.  Load one study with one series of modality CR, 
                //which contains more than 1 image.  If using FORENZA as an I-Store Online data source, there is 
                //'Woodall William' dataset, which has one study with one CR series containing 5 images.  
                //If the dataset has a PR series as well, delete it from Forenza, so you load only the CR series.

                Studies study = (Studies)login.Navigate("Studies");
                //AccessionNo: 29822041
                //Last Name: Woodall
                study.SearchStudy(AccessionNo: AccessionNumbers[0], LastName: LastNames[0], Datasource: EA_91);
                PageLoadWait.WaitForPageLoad(20);
                study.SelectStudy("Accession", AccessionNumbers[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                //Each image should be displayed in its own thumbnail; 
                //there should be 5 thumbnails in the thumbnail bar.

                IList<IWebElement> thumbnailAll = BasePage.Driver.FindElements(By.CssSelector(".thumbnailImage"));
                Boolean res5 = true;

                //Validation== 5 Thumbnail count

                for (int i = 0; i < Int32.Parse(Validation[0]); i++)
                {
                    if (thumbnailAll[0].Displayed)
                    {
                        Logger.Instance.InfoLog(" Thumbnail " + (0 + i) + " displayed or total thumbnail count is equal to 5");
                    }
                    else
                    {
                        res5 = false;
                        Logger.Instance.ErrorLog(" Thumbnail not displayed or thumbnail count not equal to 5");
                        break;
                    }
                }

                //-- Image comparision ----
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status5 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                //  verify thumbnail count =5 & displayed 5 thumbnail & image 
                if (res5 && thumbnailAll.Count == Int32.Parse(Validation[0]) && status5)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //-step-6
                //Double click on each of the thumbnails.   

                viewer.SeriesViewer_1X1().Click();
                bool res6 = true;
                var action = new Actions(BasePage.Driver);
                for (int i = 0; i < Int32.Parse(Validation[0]); i++)
                {
                    if (viewer.Thumbnails()[i] != null)
                    {
                        action.DoubleClick(viewer.Thumbnails()[i]).Build().Perform();
                        Thread.Sleep(5000);
                        PageLoadWait.WaitForPageLoad(20);
                        PageLoadWait.WaitForFrameLoad(20);

                        PageLoadWait.WaitForAllViewportsToLoad(30);
                    }

                    String Thumbnail_PsID = viewer.GetInnerAttribute(viewer.Thumbnails()[i], "src", '&', "pseudoSeriesID");
                    String Viewerport_PsID = viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "pseudoSeriesID");

                    //The viewer should update to display the image of the selected thumbnail.
                    if (Thumbnail_PsID.Equals(Viewerport_PsID) &&
                        !viewer.Thumbnails()[i].GetCssValue("border-top-color").Equals("transparent"))
                    {
                        Logger.Instance.InfoLog("Thumbnail " + i + " loaded scccessfuly in viewport");
                    }
                    else
                    {
                        res6 = false;
                        Logger.Instance.ErrorLog("Thumbnail " + i + " Double Clicked not working - verified failed");
                        break;
                    }
                }

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status6 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                // verify double click and last image comparision---
                if (res6 && status6)
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

                //step-7
                //Visually check the thumbnail caption of each thumbnail.   

                IList<IWebElement> thumbnailcaptionAll = BasePage.Driver.FindElements(By.CssSelector("div.thumbnailCaption"));
                Boolean res7 = true;

                //In this case, the thumbnail's caption is- AP CR Images #1, Image#x"
                //--Thumbnail format-- default
                //Validation[1]==> "AP\r\nCR Images\r\n#1, Image#"
                // Unable to change \\ to \ (reading data from excel data)

                Validation[1] = "AP\r\nCR Images\r\n#1, Image#";
                for (int i = 0; i < thumbnailcaptionAll.Count; i++)
                {

                    if (thumbnailcaptionAll[i].Text.Equals(Validation[1] + (i + 1)))
                    {
                        Logger.Instance.InfoLog("Thumbnail Caption " + i + 1 + ":" + thumbnailcaptionAll[i].Text + " -Verified successfully");
                    }
                    else
                    {
                        res7 = false;
                        Logger.Instance.ErrorLog("Thumbnail Caption " + i + 1 + ":" + thumbnailcaptionAll[i].Text + " is not correct -Verified failed");
                        break;
                    }
                }

                if (res7)
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
                //Load one study with one series of modality XA, which contains more than 1 image.  
                //If using FORENZA as an I-Store Online data source, 
                //there is 'Tasse Gingras Mario' dataset, which has one study XA 
                //(has one series with 73 images) and one NM. Load only the XA study.

                //***XA --Study not available, Cindy gave LN:BEEEA027P0F886P, PID:9cd74b9bp16328p
                //Available in PACS
                viewer.CloseStudy();
                study.SearchStudy(LastName: LastNames[1], patientID: Patient_ids[0], Datasource: PACS_A7);
                study.SelectStudy("Patient ID", Patient_ids[0]);
                study.LaunchStudy();

                //Study taking long time to load
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[9]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[10]));

                //Each image should be displayed in its own thumbnail;
                //there should be 9(73) thumnails in the thumbnail bar.

                //-- Validation[2]=48 (thumbnail count)

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status8 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status8 && viewer.Thumbnails().Count == Int32.Parse(Validation[2]))
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


                //--Step-9
                //Drag and drop various thumbnails into the viewer.  
                //The viewer should update to display only the image that is represented by the thumbnail.
                viewer.SeriesViewer_1X1().Click();
                bool res9 = true;
                var action9 = new Actions(BasePage.Driver);
                for (int i = 0; i < 4; i++)
                {

                    if (viewer.Thumbnails()[i] != null)
                    {
                        action9.DoubleClick(viewer.Thumbnails()[i]).Build().Perform();
                        Thread.Sleep(5000);
                        PageLoadWait.WaitForPageLoad(30);
                        PageLoadWait.WaitForFrameLoad(30);
                        PageLoadWait.WaitForFrameLoad(30);
                        PageLoadWait.WaitForAllViewportsToLoad(60);
                        PageLoadWait.WaitForAllViewportsToLoad(60);
                    }

                    String Thumbnail_PsID = viewer.GetInnerAttribute(viewer.Thumbnails()[i], "src", '&', "pseudoSeriesID");
                    String Viewerport_PsID = viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "pseudoSeriesID");

                    //The viewer should update to display the image of the selected thumbnail.
                    if (Thumbnail_PsID.Equals(Viewerport_PsID) &&
                        !viewer.Thumbnails()[i].GetCssValue("border-top-color").Equals("transparent"))
                    {
                        Logger.Instance.InfoLog("Thumbnail " + i + " loaded scccessfuly in viewport");
                    }
                    else
                    {
                        res9 = false;
                        Logger.Instance.ErrorLog("Thumbnail " + i + " Double Clicked not working - verified failed");
                        break;
                    }
                }

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status9 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (res9 && status9)
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

                //------------End of script---
                //--Step-10---
                //Logout - 
                login.Logout();
                ExecutedSteps++;
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

                //Return Result
                return result;
            }
        }

        /// <summary> 
        /// This Test Case is verification of "Review multiple series with mix of image- and series-level thumbnails"
        /// </summary>

        public TestCaseResult Test_27900(String testid, String teststeps, int stepcount)
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

                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String[] LastNames = LastNameList.Split(':');

                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNoList.Split(':');

                String StudyID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");

                String ValidationList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Validation");
                String[] Validation = ValidationList.Split(':');


                Taskbar bar = new Taskbar();
                bar.Hide();


                //Step-1--
                //----Precondition already implemented ----
                ExecutedSteps++;

                //step-2-
                //Log in to application   
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //step-3
                //Load a data which has series of modality like CR/CT/MR, PR, KO or OT in the same study.  
                //E.g. Schmidt James has MR, PR and KO series- 12 MR series with more than 1 image per series  3 KO series  3 PR series
                //(it does not matter how many image the PR series has)  Load this data.

                Studies study = (Studies)login.Navigate("Studies");
                PageLoadWait.WaitForPageLoad(20);
                //Acc: 9066875, Last name:Schmidt
                study.SearchStudy(AccessionNo: AccessionNumbers[0], LastName: LastNames[0], Datasource: PACS_A7);
                PageLoadWait.WaitForPageLoad(20);
                study.SelectStudy("Accession", AccessionNumbers[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                Thread.Sleep(5000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[8]));
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[9]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[10]));
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //The selected data should be loaded into the viewer and the following 
                //thumbnails should appear in the thumbnail bar in this order-   
                //a. The first 3 thumbnails are representing the KO series  
                //b. The 4th, 5th and 6th thumbnail are representing all images in the PR series (PR series cannot be configured for image-scope thumbnails).  
                //c. The 7th, 8th, ... to the last thumbnail are representing the MR series (12 in total)


                //-- Image comparision ----

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status3 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                //First time error came relaunching the study---**
                if (status3 == false)
                {
                    Logger.Instance.InfoLog("First time error came relaunching the study-27900_3");
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                    study.CloseStudy();
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                    study.SelectStudy("Accession", AccessionNumbers[0]);
                    study.LaunchStudy();
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    PageLoadWait.WaitForAllViewportsToLoad(30);
                    result.steps[ExecutedSteps].SetPath(testid + "_3_1", ExecutedSteps + 1);
                    status3 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                }

                Boolean res3 = true;


                if (viewer.Thumbnails().Count == 18)
                {
                    //Validation[0]==>3
                    //Validation[1]==>KO

                    for (int i = 0; i < Int32.Parse(Validation[0]); i++)
                    {
                        IJavaScriptExecutor executor = (IJavaScriptExecutor)BasePage.Driver;
                        var Thumb = executor.ExecuteScript("return document.querySelectorAll('.thumbnailCaption')[" + i + "].innerHTML");

                        if (Thumb.ToString().Contains(Validation[1]))
                        {
                            Logger.Instance.InfoLog("Thumbnail Caption " + (i + 1) + ":" + Thumb.ToString() + " -Verified successfully");
                        }
                        else
                        {
                            res3 = false;
                            Logger.Instance.ErrorLog("Thumbnail Caption " + (i + 1) + ":" + Thumb.ToString() + " is not correct -Verified failed");
                            break;
                        }
                    }
                    //Validation[0]==>3
                    //Validation[2]==>6
                    //Validation[3]==>PR
                    for (int i = Int32.Parse(Validation[0]); i < Int32.Parse(Validation[2]); i++)
                    {
                        IJavaScriptExecutor executor = (IJavaScriptExecutor)BasePage.Driver;
                        var Thumb = executor.ExecuteScript("return document.querySelectorAll('.thumbnailCaption')[" + i + "].innerHTML");

                        if (Thumb.ToString().Contains(Validation[3]))
                        {
                            Logger.Instance.InfoLog("Thumbnail Caption " + (i + 1) + ":" + Thumb.ToString() + " -Verified successfully");
                        }
                        else
                        {
                            res3 = false;
                            Logger.Instance.ErrorLog("Thumbnail Caption " + (i + 1) + ":" + Thumb.ToString() + " is not correct -Verified failed");
                            break;
                        }
                    }

                    //Validation[2]==>6
                    //Validation[4]==>18
                    //Validation[5]==>MR

                    for (int i = Int32.Parse(Validation[2]); i < Int32.Parse(Validation[4]); i++)
                    {
                        IJavaScriptExecutor executor = (IJavaScriptExecutor)BasePage.Driver;
                        var Thumb = executor.ExecuteScript("return document.querySelectorAll('.thumbnailCaption')[" + i + "].innerHTML");

                        if (Thumb.ToString().Contains(Validation[5]))
                        {
                            Logger.Instance.InfoLog("Thumbnail Caption " + (i + 1) + ":" + Thumb.ToString() + " -Verified successfully");
                        }
                        else
                        {
                            res3 = false;
                            Logger.Instance.ErrorLog("Thumbnail Caption " + (i + 1) + ":" + Thumb.ToString() + " is not correct -Verified failed");
                            break;
                        }
                    }
                }
                else if (viewer.Thumbnails().Count == 15)
                {
                    //Validation[10]==>3
                    //Validation[11]==>KO

                    for (int i = 0; i < Int32.Parse(Validation[10]); i++)
                    {
                        IJavaScriptExecutor executor = (IJavaScriptExecutor)BasePage.Driver;
                        var Thumb = executor.ExecuteScript("return document.querySelectorAll('.thumbnailCaption')[" + i + "].innerHTML");

                        if (Thumb.ToString().Contains(Validation[11]))
                        {
                            Logger.Instance.InfoLog("Thumbnail Caption " + (i + 1) + ":" + Thumb.ToString() + " -Verified successfully");
                        }
                        else
                        {
                            res3 = false;
                            Logger.Instance.ErrorLog("Thumbnail Caption " + (i + 1) + ":" + Thumb.ToString() + " is not correct -Verified failed");
                            break;
                        }
                    }

                    //Validation[12]==>15
                    //Validation[13]==>MR

                    for (int i = Int32.Parse(Validation[10]); i < Int32.Parse(Validation[12]); i++)
                    {
                        IJavaScriptExecutor executor = (IJavaScriptExecutor)BasePage.Driver;
                        var Thumb = executor.ExecuteScript("return document.querySelectorAll('.thumbnailCaption')[" + i + "].innerHTML");

                        if (Thumb.ToString().Contains(Validation[13]))
                        {
                            Logger.Instance.InfoLog("Thumbnail Caption " + (i + 1) + ":" + Thumb.ToString() + " -Verified successfully");
                        }
                        else
                        {
                            res3 = false;
                            Logger.Instance.ErrorLog("Thumbnail Caption " + (i + 1) + ":" + Thumb.ToString() + " is not correct -Verified failed");
                            break;
                        }
                    }


                }
                else
                {
                    res3 = false;
                }
               

                if (res3 && status3)
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

                //step-4
                //Load a second data with a mix of modalities series.  
                //Ex. Test Arsone, study ID -2578, available in Forenza.
                //It has- 6 CT series with single and multiple images  4 PT series
                //3 PR series (it does not matter how many images the PR series have) Load the data.

                study.CloseStudy();
                //X01002507082010 , Last Name : Test (Available in PACS)
                study.SearchStudy(LastName: LastNames[1], AccessionNo: AccessionNumbers[1], studyID: StudyID, Datasource: PACS_A7);
                study.SelectStudy("Accession", AccessionNumbers[1]);
                study.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[7]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[8]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[9]));
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);

                //The selected data should be loaded into the viewer & the following thumbnails should appear in the thumbnail bar in this order-   
                //a. The first three thumbnails are representing all images in the PR series (PR series cannot be configured for image-scope thumbnails).
                //b. The 4th, 5th, 6th, 7th, 8th and 9th thumbnails are representing the images of the CT series;  the sort order of thumbnails should be by increasing series number.
                //c. The 10th, 11th, 12th and 13th thumbnails are representing the images of the PT series; the sort order of thumbnails should be by increasing series number.
                //The thumbnail caption is displayed as overlay on the thumbnail image; no image orientation labels or measurements (in the PR thumbnails) are shown on the thumbnail image.

                //** PR images Not available---

                IList<IWebElement> thumbnailcaption_2 = BasePage.Driver.FindElements(By.CssSelector("div.thumbnailCaption"));
                Boolean res4 = true;

                //Validation[6]==>6
                //Validation[7]==>CT

                for (int i = 0; i < Int32.Parse(Validation[6]); i++)
                {
                    IJavaScriptExecutor executor = (IJavaScriptExecutor)BasePage.Driver;
                    var Thumb = executor.ExecuteScript("return document.querySelectorAll('.thumbnailCaption')[" + i + "].innerHTML");

                    if (Thumb.ToString().Contains(Validation[7]))
                    {
                        Logger.Instance.InfoLog("Thumbnail Caption " + (i + 1) + ":" + Thumb.ToString() + " -Verified successfully");
                    }
                    else
                    {
                        res4 = false;
                        Logger.Instance.ErrorLog("Thumbnail Caption " + (i + 1) + ":" + Thumb.ToString() + " is not correct -Verified failed");
                        break;
                    }
                }

                //Validation[8]==>10
                //Validation[9]==>PT

                for (int i = Int32.Parse(Validation[6]); i < Int32.Parse(Validation[8]); i++)
                {
                    IJavaScriptExecutor executor = (IJavaScriptExecutor)BasePage.Driver;
                    var Thumb = executor.ExecuteScript("return document.querySelectorAll('.thumbnailCaption')[" + i + "].innerHTML");

                    if (Thumb.ToString().Contains(Validation[9]))
                    {
                        Logger.Instance.InfoLog("Thumbnail Caption " + (i + 1) + ":" + Thumb.ToString() + " -Verified successfully");
                    }
                    else
                    {
                        res4 = false;
                        Logger.Instance.ErrorLog("Thumbnail Caption " + (i + 1) + ":" + Thumb.ToString() + " is not correct -Verified failed");
                        break;
                    }
                }
                //-- Image comparision ----
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status4 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (res4 && status4)
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

                //------------End of script---
                //Logout - Step-5

                login.Logout();
                ExecutedSteps++;

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

                //Return Result
                return result;
            }
        }


        /// <summary> 
        /// This Test Case is verification of Thumbnail in "Domain configuration thumbnails -1 (verify Series and Image Splitting)" 
        /// </summary>

        public TestCaseResult Test1_27901(String testid, String teststeps, int stepcount)
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

                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String[] LastNames = LastNameList.Split(':');

                String FirstNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String[] FirstNames = FirstNameList.Split(':');

                String DescriptionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
                String[] Descriptions = DescriptionList.Split(':');

                String StudyIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");
                String[] StudyIDs = StudyIDList.Split(':');

                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNoList.Split(':');

                String ValidationList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Validation");
                String[] Validation = ValidationList.Split(':');

                //String datasource = login.GetHostName(Config.EA1);
                //String datasource1 = login.GetHostName(Config.ISTORE1); 
                string[] datasource = { "VMSSA-4-38-131", "AUTO-SSA-001", "VMSSA-5-38-91" };

                //Step-1
                //--Precondition ---

                Taskbar bar = new Taskbar();
                bar.Hide();

                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();

                st.LaunchServiceTool();
                st.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(2);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                wpfobject.WaitTillLoad();

                var comboBox_mod = wpfobject.GetComboBox("ComboBox_Modality");

                int count = comboBox_mod.Items.Count;

                for (int i = 0; i < count; i++)
                {
                    comboBox_mod.Select(i);
                    wpfobject.ClickRadioButtonById("RB_ThumbnailSplittingName");
                    wpfobject.WaitTillLoad();
                }
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();


                ExecutedSteps++;

                //step-2
                //Log in to application   
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //step-3--
                //Edit the Default System Domain.    
                //In the Edit Domain page scroll to 'Default Settings per Modality' panel & 
                //change the Thumbnail Splitting setting to 'Series' for CR modality. Save the Changes.
                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain("SuperAdminGroup");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();
                domain.ModalityDropDown().SelectByText("CR");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                domain.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(30);
                ExecutedSteps++;

                // step-4
                //Test Data- Note-the User Preferences setting may have different setting than the Domain settings.
                //In this case User Preferences has precedence.
                //If using FORENZA as datasource, load CR HI-RES data, the one with study ID -  2.
                //It has- 1 CR series with 2 images (series ID -  1)  1 CR series with 1 image (series ID- 1) 1 PR series with 1 image

                //* PR not available--

                Studies study = (Studies)login.Navigate("Studies");
                //CR  HI-RES, 2
                study.SearchStudy(LastName: LastNames[0], FirstName: FirstNames[0], studyID: StudyIDs[0], Datasource: EA_91);
                study.SelectStudy("Description", Descriptions[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();

                //-- Image comparision ----
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status4 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status4 && viewer.Thumbnails().Count == 1)
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

                study.CloseStudy();

                //-step-5-
                //Load Portable Betty data, the study with Study ID -  41487.   

                login.Navigate("Studies");
                //Portable Betty 41487   10717882
                study.SearchStudy(LastName: LastNames[1], FirstName: FirstNames[1], studyID: StudyIDs[1], AccessionNo: AccessionNumbers[0], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumbers[0]);
                study.LaunchStudy();

                //The selected study is loaded into the viewer.  Since both images are of the same modality,
                //the order in which thumbnails are displayed in the study thumbnail bar should be-  lower image number, 
                //before higher image number.  The image# 82632000 should be the first thumbnail, 
                //the image# 82633000 should be the second thumbnail.

                //-- Image comparision ----
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status5 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status5 && viewer.Thumbnails().Count == 2)
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
                study.CloseStudy();

                //step-6
                //Create two new domains-domain1 name-ThumbnailsSeriesDomain with the thumbnail splitting set for Series for CR modality.
                //domain2 name -ThumbnailsImagesDomain with the thumbnail splitting set for Images for CR modality.
                //Fill in all the required information and connect the same data source to both domains (ex. FORENZA)

                login.Navigate("DomainManagement");

                domain.CreateDomain(seriesDomain, seriesRole,datasources: datasource);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(10);
                domain.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(30);
                domain.SearchDomain(seriesDomain);
                domain.SelectDomain(seriesDomain);
                domain.ClickEditDomain();
                domain.ModalityDropDown().SelectByText("CR");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                domain.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(30);

                domain.CreateDomain(ImageDomain, ImageRole, datasources: datasource);
                PageLoadWait.WaitForPageLoad(30);
                domain.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(30);
                domain.SearchDomain(ImageDomain);
                domain.SelectDomain(ImageDomain);
                domain.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(30);
                domain.ModalityDropDown().SelectByText("CR");
                PageLoadWait.WaitForPageLoad(30);
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                domain.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(30);

                //Two new domains are created each of them with its own thumbnail settings.
                if (domain.SearchDomain(seriesDomain) && domain.SearchDomain(ImageDomain))
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

                //step-7
                //Log in ICA client as the administrator of ThumbnailsSeriesDomain.
                //Load dataset Burton Cliff, study ID -  852654

                login.DriverGoTo(login.url);
                login.LoginIConnect(seriesDomain, seriesDomain);
                login.Navigate("Studies");

                //--Select all data source & all date--
                var js = BasePage.Driver as IJavaScriptExecutor;
                if (js != null)
                {

                    js.ExecuteScript("MultiSelectorMenuObject.dropDownMenuItemClick(1)");
                    PageLoadWait.WaitForPageLoad(20);
                    js.ExecuteScript("StudySearchMenuControl.dropDownMenuItemClick(\'0\');");
                    PageLoadWait.WaitForPageLoad(20);
                }

                //AC8526354 (BURTON, CLIFF) (852654)
                study.SearchStudy(AccessionNo: AccessionNumbers[1], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumbers[1]);
                study.LaunchStudy();

                //--Image comparision------
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status7 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status7 && viewer.Thumbnails().Count == 1)
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

                study.CloseStudy();
                //--step-8
                //Create a new role, 'User',  in the ThumbnailsSeriesDomain.  
                //Create a new user, 'User1', associated to the 'User' role.  
                //Log in as this user, 'User1', and load the same dataset as before.

                RoleManagement role = (RoleManagement)login.Navigate("RoleManagement");
                role.NewRoleBtn().Click();
                //role.CreateRole(seriesDomain, roleName);

                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");

                role.DomainNameDropDown().SelectByText(seriesDomain);
                PageLoadWait.WaitForPageLoad(20);

                role.RoleNameTxt().SendKeys(roleName);
                role.RoleDescriptionTxt().SendKeys(roleName + " Description");
                PageLoadWait.WaitForPageLoad(20);
                role.SaveBtn().Click();
                PageLoadWait.WaitForPageLoad(20);

                UserManagement usermgt = (UserManagement)login.Navigate("UserManagement");


                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                usermgt.Click("cssselector", " #NewUserButton");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("#m_sharedNewUserControl_Button1")));

                usermgt.SetText("cssselector", "#m_sharedNewUserControl_UserInfo_UserID", userId);
                usermgt.SetText("id", "m_sharedNewUserControl_UserInfo_LastName", userId);
                usermgt.SetText("id", "m_sharedNewUserControl_UserInfo_FirstName", userId);
                usermgt.SetText("id", "m_sharedNewUserControl_UserInfo_Password", userId);
                usermgt.SetText("id", "m_sharedNewUserControl_UserInfo_ComparisonPassword", userId);
                usermgt.SelectFromList("id", "m_sharedNewUserControl_ChooseRoleDropDownList", roleName);

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(20);
                usermgt.ClickButton("#m_sharedNewUserControl_Button1");
                PageLoadWait.WaitForFrameLoad(20);


                login.Logout();

                login.DriverGoTo(login.url);
                login.LoginIConnect(userId, userId);
                login.Navigate("Studies");

                var js1 = BasePage.Driver as IJavaScriptExecutor;
                if (js1 != null)
                {

                    js1.ExecuteScript("MultiSelectorMenuObject.dropDownMenuItemClick(1)");
                    PageLoadWait.WaitForPageLoad(20);
                    js1.ExecuteScript("StudySearchMenuControl.dropDownMenuItemClick(\'0\');");
                    PageLoadWait.WaitForPageLoad(20);
                }
                //study.SearchStudy(LastName: LastNames[2], FirstName: FirstNames[2]);
                //AC8526354 (BURTON, CLIFF) (852654)
                study.SearchStudy(AccessionNo: AccessionNumbers[1], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumbers[1]);
                study.LaunchStudy();

                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);

                //--Image comparision------
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status8 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status8 && viewer.Thumbnails().Count == 1)
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


                //--step-9
                //1. Go to Options -> User Preferences and in 'Default Settings Per Modality' section 
                //change the Thumbnail Splitting from Series to Image, for CR modality.
                //Say OK.  2. Load the same data as before.

                study.CloseStudy();
                login.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                //PageLoadWait.WaitForFrameLoad(20);
                domain.ModalityDropDown().SelectByText("CR");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                PageLoadWait.WaitForPageLoad(20);

                //-- Click OK and close btn--
                study.CloseUserPreferences();

                login.Navigate("Studies");
                //study.SearchStudy(LastName: LastNames[2], FirstName: FirstNames[2]);
                //AC8526354 (BURTON, CLIFF) (852654)
                study.SearchStudy(AccessionNo: AccessionNumbers[1], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumbers[1]);
                study.LaunchStudy();

                //--Image comparision------
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status9 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status9 && viewer.Thumbnails().Count == 4)
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
                study.CloseStudy();
                login.Logout();

                //--step-10
                //Log in ICA client as the administrator of ThumbnailsImagesDomain.
                //Load dataset Burton Cliff, study ID -  852654

                login.DriverGoTo(login.url);
                login.LoginIConnect(ImageDomain, ImageDomain);
                login.Navigate("Studies");

                //--Select all data source & all date--
                var js2 = BasePage.Driver as IJavaScriptExecutor;
                if (js2 != null)
                {
                    js2.ExecuteScript("MultiSelectorMenuObject.dropDownMenuItemClick(1)");
                    PageLoadWait.WaitForPageLoad(20);
                    js2.ExecuteScript("StudySearchMenuControl.dropDownMenuItemClick(\'0\');");
                    PageLoadWait.WaitForPageLoad(20);
                }
                //AC8526354 (BURTON, CLIFF) (852654)
                study.SearchStudy(AccessionNo: AccessionNumbers[1], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumbers[1]);
                study.LaunchStudy();


                //The selected study is loaded into the viewer.
                //Since the thumbnail splitting for this domain is set to Images, 
                //there will be one thumbnail for each PR series & 
                //one thumbnail for each CR image displayed in the study thumbnail bar.

                //--Image comparision------

                IList<IWebElement> thumbnailAll = BasePage.Driver.FindElements(By.CssSelector(".thumbnailImage"));
                Boolean res10 = true;

                //Validation[0]=4
                for (int i = 0; i < Int32.Parse(Validation[0]); i++)
                {
                    if (thumbnailAll[i].Displayed)
                    {
                        Logger.Instance.InfoLog(" Thumbnail " + (0 + i) + " displayed -verified successfully");
                    }
                    else
                    {
                        res10 = false;
                        Logger.Instance.ErrorLog(" Thumbnail " + (0 + i) + " not displayed - verified failed");
                        break;
                    }
                }


                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status10 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                //-Verify screenshot and thumbnail count & thumbnail text 
                if (status10 && res10 && thumbnailAll.Count == 4)
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
                study.CloseStudy();

                //Logout Step-11
                login.Logout();
                ExecutedSteps++;

                //--------End of script---
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

                //Return Result
                return result;
            }
        }

        /// <summary> 
        /// This Test Case is verification of Thumbnail in "Domain configuration thumbnails -2 (Verify Caption Pattern  and Overlay)" 
        /// </summary>

        public TestCaseResult Test2_27901(String testid, String teststeps, int stepcount)
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

                String ValidationList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Validation");
                String[] Validation = ValidationList.Split(':');



                //String ImageDomain = "ThumbnailsImagesDomain";


                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();

                Taskbar bar = new Taskbar();
                bar.Hide();

                //step-1
                //In the ICA server open Merge iConnect Access Service Tool & 
                //select Viewer tab -> Miscellaneous sub-tab -> Modify button

                st.LaunchServiceTool();
                st.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                wpfobject.WaitTillLoad();
                wpfobject.SelectCheckBox("CB_EnableHtml5Support");
                wpfobject.ClickButton("Cancel", 1);
                wpfobject.WaitTillLoad();
                //st.CloseServiceTool();

                ExecutedSteps++;

                //step-2
                //Test Data-Attachment-Screenshot for TC Domain configuration thumbnails.docx.
                //Change the Default Thumbnail Caption to something else, 
                //by selecting any item from the drop down list. 
                //See example in the attached screen shot. Save the changes and restart IIS.
                //What this syntax for the thumbnail caption means is-    
                //1. Each field is delimited by % character. The fields that appear in this string are-   
                //*Mod-  two-letter name of modality, e.g. MR, CT,....  
                //*SeriesNum-  Series number  
                //* Image-  'Image' text  
                //*ImageNum-  Image number  
                //*SeriesDesc-  series description    
                //2. The curly braces are used to group static text with a field.
                //The purpose of that being,that if the DICOM tag is missing or 
                //the field doesn't make sense (e.g. ImageNum in a series-scope thumbnail), 
                //the whole text between the curly braces is omitted from the thumbnail caption.
                //3. <br> indicates line breaks in overlay mode but is replaced with "," 
                //in the mode when caption is displayed underneath.

                //st.LaunchServiceTool();
                //st.NavigateToTab("Viewer");

                //Thumbnail Captions
                wpfobject.GetTabWpf(1).SelectTabPage(3);
                wpfobject.ClickButton("Modify", 1);
                var comboBoxDicomCaption = wpfobject.GetComboBox("ComboBox_DefaultThumbnailCaption");
                comboBoxDicomCaption.Select("{%Mod%}{ #%SeriesNum%}{- Image %ImageNum%}{<br>%SeriesDesc%}");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();

                ExecutedSteps++;

                //step-3
                //Log back in ICA client

                login.DriverGoTo(login.url);
                login.LoginIConnect(ImageDomain, ImageDomain);
                ExecutedSteps++;

                //Step-4
                //load various datasets (ex. CR Solo Knee from Forenza).
                //Observed the tumbnails' caption.

                Studies study = (Studies)login.Navigate("Studies");
                //2004414132327828
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_91);
                study.SelectStudy("Accession", AccessionNumbers[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();

                //The caption of the tumbnails should be 'Modality -Series# - <Image> Image# - Series Description'.
                //If loading CR Solo Knee this should read- CR #2 -Image#1  Knee AP  
                //You can verify if this is correct by looking at the data directly in the data source.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status4 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                IJavaScriptExecutor executor1 = (IJavaScriptExecutor)BasePage.Driver;
                var Thumb1 = executor1.ExecuteScript("return document.querySelector('.thumbnailCaption').innerHTML");

                //Validation[0] ="CR #2<br>Knee AP"

                if (status4 && Thumb1.ToString().ToLower().Equals(Validation[0].ToLower()))
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

                //Step-5
                //Draw some measurements on one image and save the annotated image.  
                //Re-load the data.  Visually check the thumbnail caption.

                result.steps[++ExecutedSteps].status = "Not Automated";

                login.Logout();
                //-Step-6
                //Try various thumbnail's captions configurations, 
                //by modifying them in the Merge ICA Service Tool.

                st.LaunchServiceTool();
                st.NavigateToTab("Viewer");
                wpfobject.WaitTillLoad();

                //Thumbnail Captions
                wpfobject.GetTabWpf(1).SelectTabPage(3);
                wpfobject.ClickButton("Modify", 1);
                var comboBoxDicomCaption_1 = wpfobject.GetComboBox("ComboBox_DefaultThumbnailCaption");
                comboBoxDicomCaption_1.Select("{%Mod%}{ #%SeriesNum%}{ Image#%ImageNum%}{<br>%NumInstances% Images}{<br>%SeriesDesc%}");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();

                login.DriverGoTo(login.url);
                login.LoginIConnect(ImageDomain, ImageDomain);
                login.Navigate("Studies");
                //2004414132327828
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_91);
                study.SelectStudy("Accession", AccessionNumbers[0]);
                study.LaunchStudy();

                //The caption of the tumbnails should change accordingly &
                //should be correctly displayed when data is loaded.  
                //The thumbnail caption is displayed as overlay on the thumbnail image; 
                //no image orientation labels or measurements (in the PR thumbnails) are shown on the thumbnail image.

                IJavaScriptExecutor executor2 = (IJavaScriptExecutor)BasePage.Driver;
                var Thumb2 = executor2.ExecuteScript("return document.querySelector('.thumbnailCaption').innerHTML");

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status6 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                //Validation[1] ="CR #2<br>1 Images<br>Knee AP"

                if (status6 && Thumb2.ToString().ToLower().Equals(Validation[1].ToLower()))
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

                login.Logout();

                //Step-7
                //Select to edit the 'ThumbnailsImages Domain' and remove the check mark from 
                //'Display Thumbnail Caption As Overlay' option.  Save the changes.

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(ImageDomain);
                domain.SelectDomain(ImageDomain);
                domain.ClickEditDomain();
                //--remove the check box--
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
                ExecutedSteps++;
                login.Logout();

                //Step-8
                //Load the same datasets as at the above test cases and
                //make note of the position of the thumbnails' caption.

                login.DriverGoTo(login.url);
                login.LoginIConnect(ImageDomain, ImageDomain);
                login.Navigate("Studies");
                //2004414132327828
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_91);
                study.SelectStudy("Accession", AccessionNumbers[0]);
                study.LaunchStudy();

                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);

                //The caption of the tumbnails should remain unchanged, 
                //but the position now should be underneath the thumbnails and not over the thumbnails.  
                //The image orientation labels and measurements (in the PR thumbnails) are shown on the thumbnail image.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status8 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                IJavaScriptExecutor executor3 = (IJavaScriptExecutor)BasePage.Driver;
                var Thumb3 = executor3.ExecuteScript("return document.querySelector('.thumbnailCaption').innerHTML");


                //Validation[2] ="CR #2, 1 Images, Knee AP"
                if (status8 && Thumb3.ToString().ToLower().Equals(Validation[2].ToLower()))
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

                //------------End of script---
                //Logout -- Step-9
                login.Logout();
                ExecutedSteps++;

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

                //Return Result
                return result;

            }
            finally
            {

                //Change CR Thumbnail splitting to Image.

                String username = Config.adminUserName;
                String password = Config.adminPassword;

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain("SuperAdminGroup");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();
                domain.ModalityDropDown().SelectByText("CR");
                domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");
                domain.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(30);

                login.Logout();

                //--service tool Modify default setting as Thumbnail caption--
                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();

                Taskbar bar = new Taskbar();
                bar.Hide();


                st.LaunchServiceTool();
                st.NavigateToTab("Viewer");

                wpfobject.GetTabWpf(1).SelectTabPage(3);
                wpfobject.ClickButton("Modify", 1);
                wpfobject.WaitTillLoad();
                var comboBoxDicomCaption = wpfobject.GetComboBox("ComboBox_DefaultThumbnailCaption");
                comboBoxDicomCaption.Select("{%SeriesDesc%<br>}{%NumInstances% }{%Mod% Images<br>}{#%SeriesNum%}{, Image#%ImageNum%}");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();

                //-- Modify Default settings--
                st.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(2);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);

                string[] SeriesMod = { "CT", "MR", "NM", "PT", "RF", "VL", "RTIMAGE" };

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
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();

                bar.Show();
            }

        }


        /// <summary> 
        /// This Test Case is verification of Thumbnail with respect of "Thumbnail Attributes Overlay" 
        /// </summary>

        public TestCaseResult Test_27902(String testid, String teststeps, int stepcount)
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

                String LastNameList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String[] LastNames = LastNameList.Split(':');

                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNoList.Split(':');

                Taskbar bar = new Taskbar();
                bar.Hide();

                //--step-1
                //Complete Pre-Conditions in Initial Setups test case.
                ExecutedSteps++;

                //Step-2
                //Test Data-the attributes saved on an image are displayed in the thumbnail view as a default. 
                //The attributes can be turned off in the Servicetool.     
                //TAB Viewer->Miscellaneous -- Open the service tool in the tab Viewer->Miscellaneous &
                //make sure the Show Thumbnail Overlay box is checked,
                ServiceTool st = new ServiceTool();

                st.LaunchServiceTool();
                WpfObjects wpfobject = new WpfObjects();

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
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();

                Thread.Sleep(10000);
                //--step-3
                //Login as a Admin
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //step-4
                //Open a study in the viewer.
                Studies study = (Studies)login.Navigate("Studies");

                //--The given study is not having overlay attribute. 
                //-- So Used study (accession --ARS0745607) for testing.
                //ARS0745607 has loading issue So
                //AccessionNumbers[0] =510bc45ad
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: PACS_A7);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                study.SelectStudy("Accession", AccessionNumbers[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Study opens if there is a PR image it is in the first viewport &
                //the attributes are displayed on the thumbnail image.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status4 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                //First time error came relaunching the study---**
                if (status4 == false)
                {
                    Logger.Instance.InfoLog("First time error came relaunching the study-27902_4");
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                    study.CloseStudy();
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                    study.SelectStudy("Accession", AccessionNumbers[0]);
                    study.LaunchStudy();
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    PageLoadWait.WaitForAllViewportsToLoad(30);
                    result.steps[ExecutedSteps].SetPath(testid + "_4_1", ExecutedSteps + 1);
                    status4 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                }

                if (status4 && viewer.Thumbnails().Count == 10 && viewer.SeriesViewPorts().Count == 4)
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
                //Step-5--
                //select an image that does not have any attributes & add a line,circle & text,Save the series.
                //Image is saved,  a new thumbnail is displayed with the attributes 
                //that were added in the previous step also displayed.
                //*-- save option not eabled--
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-6
                //Load another study and then go back and reload the study just saved.
                //*-- save option not eabled--
                result.steps[++ExecutedSteps].status = "Not Automated";

                login.Logout();
                Thread.Sleep(5000);

                //Step-7---
                //Open the service tool  Viewer->Miscellaneous and 
                //uncheck the Show Thumbnail Overlay box,  IISRESET               

                st.LaunchServiceTool();
                st.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                wpfobject.WaitTillLoad();
                //chk ShowThumbnailOverlays
                wpfobject.UnSelectCheckBox("CB_ShowThumbnailOverlays");

                //--verify overlay not selected--
                if (!wpfobject.IsCheckBoxSelected("CB_ShowThumbnailOverlays"))
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
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();

                Thread.Sleep(10000);

                //Step-8
                //Login as an Admin and open the same study from the previous step in the viewer.
                //Precondition--
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                login.Navigate("Studies");
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: PACS_A7);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                login.Logout();

                //--The given study( accession:AC8526354) is not having overlay attribute. 
                //-- So Used study (accession --ARS0745607) for testing.
                //ARS0745607 loading issue So
                //AccessionNumbers[0] =510bc45ad 
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                login.Navigate("Studies");
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: PACS_A7);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                study.SelectStudy("Accession", AccessionNumbers[0]);
                study.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);



                //Study opens the PR image it is in the first viewport & 
                //the attributes are displayed in the viewport.  
                //The attributes are not displayed in the Thumbnail.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status8 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                //First time error came relaunching the study---**
                if (status8 == false)
                {
                    Logger.Instance.InfoLog("First time error came relaunching the study-27902_8");
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                    study.CloseStudy();
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                    study.SelectStudy("Accession", AccessionNumbers[0]);
                    study.LaunchStudy();
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[6]));
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[7]));
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[8]));
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[9]));
                    PageLoadWait.WaitForAllViewportsToLoad(30);
                    result.steps[ExecutedSteps].SetPath(testid + "_8_1", ExecutedSteps + 1);
                    status8 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                }

                if (status8 && viewer.Thumbnails().Count == 10 && viewer.SeriesViewPorts().Count == 4)
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
                //Load other studies that have a PR and 
                //confirm the attributes are not displayed in the Thumbnail
                //access no-"7817811", Last name-aneurysm laoding issue
                //b8edc56a4   Last Name :C9B44A
                login.Navigate("Studies");
                //14779 Avango
                study.SearchStudy(LastName: LastNames[0], AccessionNo: AccessionNumbers[1], Datasource: PACS_A7);
                study.SelectStudy("Accession", AccessionNumbers[1]);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                study.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status9 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status9 && viewer.Thumbnails().Count == 6 && viewer.SeriesViewPorts().Count == 4)
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

                //------End of script---
                //Logout -- Step-10
                login.Logout();
                ExecutedSteps++;
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

                //Return Result
                return result;
            }
            finally
            {
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
        /// This Test Case is verification of Thumbnail with respect of "Auto Thumbnail Spliting" 
        /// </summary>

        public TestCaseResult Test_73807(String testid, String teststeps, int stepcount)
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

                String PatientIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] PatientIDs = PatientIDList.Split(':');

                String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionNumbers = AccessionNoList.Split(':');

                String ValidationList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Validation");
                String[] Validation = ValidationList.Split(':');

                String ImageCountList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ImageCount");
                String[] Imagecount = ImageCountList.Split(':');

                Studies study = new Studies();
                Taskbar bar = new Taskbar();
                bar.Hide();

                //-study already available-- step-1
                ExecutedSteps++;

                //service tool code-- step-2

                //Login to the Service tool and navigate to the Viewer/Protocols page,
                //Verify or set setting as default for the modalities listed below 

                //Modal Layout  Thumb     Viewer    Exam
                // CT -- 2x2  -- series -- series -- Off
                // XA -- Auto -- image  -- series -- Off
                // MG -- 1x2  -- image  -- series -- Off
                // US -- Auto -- Image  -- series -- Off 

                string[] Modality = { "CT", "XA", "MG", "US" };
                string[] Layout = { "2x2", "auto", "1x2", "auto" };
                string[] Thumbnail = { "Series", "Image", "Image", "Image" };
                string[] ViewerScope = { "Series", "Series", "Series", "Series" };
                string[] ExamMode = { "Off", "Off", "Off", "Off" };

                //Leave the other settings to default
                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();

                st.LaunchServiceTool();
                st.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(2);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);

                for (int i = 0; i < 4; i++)
                {
                    st.SelectDropdown("ComboBox_Modality", Modality[i]);
                    st.SelectDropdown("ComboBox_Layout", Layout[i]);
                    //wpfobject.ClickRadioButtonById("vp_" + Thumbnail[i].ToLower() + "ThumbnailRadioButton");
                    //wpfobject.ClickRadioButtonById("vp_" + ViewerScope[i].ToLower() + "ScopeRadioButton");
                    //wpfobject.ClickRadioButtonById("vp_ExamMode" + ExamMode[i] + "RadioButton");
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
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //-Step-3
                //Login to ICA as administrator and navigate to the Domain management for the user logged in &
                //confirm the default setting for the protocols are the same as the settings above.

                //Modal Layout  Thumb     Viewer    Exam
                // CT -- 2x2  -- Series -- series -- Off
                // XA -- auto -- Image  -- series -- Off
                // MG -- 1x2  -- Image  -- series -- Off
                // US -- auto -- Image  -- series -- Off

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain("SuperAdminGroup");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();

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
                //Click Close
                domain.Click("cssselector", "[id$='EditDomainControl_CloseButton']");

                login.Navigate("Studies");

                //--Step-4
                //Open the User Preferences and verify or change the Protocol setting to default setting as 
                //described in step one

                Boolean res4 = true;
                login.OpenUserPreferences();
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

                login.Click("id", "CancelPreferenceUpdateButton");

                //step-5
                //Load CT Study - 8114BB, ( PID- 16E2AC) Accession - 7f7ed13ae, (Location EA-131)
                //8 series =>1-3 images, 2-38 images, 3-38 images, 4-80 images, 5-28 images, 6-28 images,
                //7-38 images  8- 80 images

                login.Navigate("Studies");

                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumbers[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();

                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[6]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[7]));
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);

                Boolean res5 = true;

                //Imagecount = { "3", "38", "38", "80", "28", "28", "38", "80" };
                //Validation[0]=8

                for (int i = 0; i < Int32.Parse(Validation[0]); i++)
                {
                    IJavaScriptExecutor executor = (IJavaScriptExecutor)BasePage.Driver;
                    var Thumb = executor.ExecuteScript("return document.querySelectorAll('.thumbnailCaption')[" + i + "].innerHTML");

                    if (Thumb.ToString().Contains(Imagecount[i]))
                    {
                        Logger.Instance.InfoLog("Thumbnail Caption " + (i + 1) + ":" + Thumb.ToString() + "=> Image count: " + Imagecount[i] + "  -Verified successfully");
                    }
                    else
                    {
                        res5 = false;
                        Logger.Instance.ErrorLog("Thumbnail Caption " + (i + 1) + ":" + Thumb.ToString() + "=> Image count: " + Imagecount[i] + "  - not matching -Verified failed");
                        break;
                    }
                }

                IList<IWebElement> thumbnail_5_1 = BasePage.Driver.FindElements(By.CssSelector(".thumbnailImage"));

                //-- verify 8 thumbnail are displayed-
                if (thumbnail_5_1.Count != Int32.Parse(Validation[0]))
                {
                    res5 = false;
                    Logger.Instance.ErrorLog("Thumbnail count is not equal to 8 - verified failed");
                }

                //-- Image comparision ----
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status5 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (res5 && status5)
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

                study.CloseStudy();

                //step-6
                //XA Study- XA-Bargus, 2 Multiframe series (Location Chrisd701)
                //Images#2 and Images# 7 the video film strip is displayed in each thumbnail
                //-- XA-Bargus PID_12514

                study.SearchStudy(patientID: PatientIDs[0], Datasource: PACS_A7);
                study.SelectStudy("Patient ID", PatientIDs[0]);
                study.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);


                Boolean res6 = true;
                IList<IWebElement> thumbnail_1 = BasePage.Driver.FindElements(By.CssSelector(".thumbnailImage"));

                //Validation[1]=1

                //-- verify 1 thumbnail image is displayed---
                if (thumbnail_1.Count != Int32.Parse(Validation[1]))
                {
                    res6 = false;
                    Logger.Instance.ErrorLog("Thumbnail count is not equal to 1 - verified failed");
                }

                //-- Image comparision ----
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status6 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (res6 && status6)
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
                study.CloseStudy();

                //step-7
                //Load the US study --YSJ-US100 (PID- ysj-222100) eleven-teen Contents- 16 single image
                //29 Multiframe series (18 to 47)    1 single image (PID=525387)

                //-- above study not available--
                study.SearchStudy(patientID: PatientIDs[1], Datasource: PACS_A7);
                study.SelectStudy("Patient ID", PatientIDs[1]);
                study.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[6]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[7]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[8]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[9]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[10]));


                Boolean res7 = true;
                IList<IWebElement> thumbnail_50 = BasePage.Driver.FindElements(By.CssSelector(".thumbnailImage"));

                //Validation[2]=47
                //-- verify 47 thumbnail images are displayed---
                if (thumbnail_50.Count != Int32.Parse(Validation[2]))
                {
                    res7 = false;
                    Logger.Instance.ErrorLog("Thumbnail count is not equal to 47 - verified failed");
                }

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status7 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (res7 && status7)
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
                study.CloseStudy();

                //--Step-8
                //MG Study BAL-MG10 (ALMG100010) Accssion - 2205587,
                //10 images in two series 14626, 14627

                study.SearchStudy(AccessionNo: AccessionNumbers[1], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumbers[1]);
                study.LaunchStudy();

                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[0]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[1]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[2]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[3]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[4]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[5]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[6]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[7]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[8]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[9]));

                Boolean res8 = true;
                IList<IWebElement> thumbnail_10 = BasePage.Driver.FindElements(By.CssSelector(".thumbnailImage"));

                //-- verify 10 thumbnail images are displayed---
                //Validation[3]=10

                if (thumbnail_10.Count != Int32.Parse(Validation[3]))
                {
                    res8 = false;
                    Logger.Instance.ErrorLog("Thumbnail count is not equal to 10 - verified failed");
                }

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status8 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (res8 && status8)
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
                study.CloseStudy();

                //--Step-9
                //Login to ICA as Administrator and navigate to the SuperAdmiGroup Domain & 
                //change the Thumbnail Splitting setting for Modality CT,XA , MG and US to AUTO, 
                //the Viewing scope -  Series , leave the other settings to default/

                //Modal    Thumb     Viewer   
                //CT --  -- Auto  -- Series
                //US --  -- Auto  -- Series  
                //XA --  -- Auto  -- Series    
                //MG --  -- Auto  -- Series 

                login.Navigate("DomainManagement");
                domain.SearchDomain("SuperAdminGroup");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();

                string[] Mod = { "CT", "US", "XA", "MG" };
                for (int i = 0; i < Mod.Length; i++)
                {
                    domain.ModalityDropDown().SelectByText(Mod[i]);
                    domain.SelectRadioBtn("ThumbSplitRadioButtons", "Auto");
                    domain.SelectRadioBtn("ScopeRadioButtons", "Series");
                }
                domain.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(30);
                ExecutedSteps++;

                //step-10
                //Load CT Study (ASSession : 7f7ed13ae)

                login.Navigate("Studies");
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumbers[0]);
                study.LaunchStudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[3]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[4]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[5]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[6]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[7]));

                Boolean res10 = true;
                //Imagecount = { "3", "38", "38", "80", "28", "28", "38", "80" };

                for (int i = 0; i < Imagecount.Length; i++)
                {
                    IJavaScriptExecutor executor = (IJavaScriptExecutor)BasePage.Driver;
                    var Thumb = executor.ExecuteScript("return document.querySelectorAll('.thumbnailCaption')[" + i + "].innerHTML");

                    if (Thumb.ToString().Contains(Imagecount[i]))
                    {
                        Logger.Instance.InfoLog("Thumbnail Caption " + (i + 1) + ":" + Thumb.ToString() + "=> Image count: " + Imagecount[i] + "  -Verified successfully");
                    }
                    else
                    {
                        res10 = false;
                        Logger.Instance.ErrorLog("Thumbnail Caption " + (i + 1) + ":" + Thumb.ToString() + "=> Image count: " + Imagecount[i] + "  - not matching -Verified failed");
                        break;
                    }
                }

                IList<IWebElement> thumbnail_10_1 = BasePage.Driver.FindElements(By.CssSelector(".thumbnailImage"));

                //-- verify 8 thumbnail are displayed-

                //Validation[4]=8

                if (thumbnail_10_1.Count != Int32.Parse(Validation[4]))
                {
                    res10 = false;
                    Logger.Instance.ErrorLog("Thumbnail count is not equal to 8 - verified failed");
                }

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status10 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (res10 && status10)
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
                study.CloseStudy();

                //step-11
                //Load the US Study  YSJ-US100 (ysj-222100) eleven-teen (PID= "525387")
                study.SearchStudy(patientID: PatientIDs[1], Datasource: PACS_A7);
                study.SelectStudy("Patient ID", PatientIDs[1]);
                study.LaunchStudy();
                Thread.Sleep(10000);
              
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(30);

                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[6]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[7]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[8]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[9]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[10]));

                Boolean res11 = true;
                IList<IWebElement> thumbnail_11_1 = BasePage.Driver.FindElements(By.CssSelector(".thumbnailImage"));

                //-- verify 1 thumbnail image is displayed-

                //Validation[5]=32

                if (thumbnail_11_1.Count != Int32.Parse(Validation[5]))
                {
                    res11 = false;
                    Logger.Instance.ErrorLog("Thumbnail count is not equal to 1 - verified failed");
                }

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status11 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (res11 && status11)
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
                study.CloseStudy();

                try
                {
                    //Sync-up IE-9
                    //Frame switching Error occured in this line in IE-9 browser 
                    //so added try catch block for workaround

                    domain = (DomainManagement)login.Navigate("DomainManagement");
                    study = (Studies)login.Navigate("Studies");

                    //step-12
                    //Load the XA study XA-Bargus 9PID_3267)
                    // PID_12514
                    study.SearchStudy(patientID: PatientIDs[0], Datasource: PACS_A7);
                    study.SelectStudy("Patient ID", PatientIDs[0]);
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                    study.LaunchStudy();
                   
                    PageLoadWait.WaitForPageLoad(30);
                    PageLoadWait.WaitForFrameLoad(30);
                    PageLoadWait.WaitForAllViewportsToLoad(40);
                    PageLoadWait.WaitForThumbnailsToLoad(40);
                }
                catch (Exception e)
                {
                    if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("9"))
                    {
                        login.Logout();
                        login.DriverGoTo(login.url);
                        login.LoginIConnect(username, password);

                        domain = (DomainManagement)login.Navigate("DomainManagement");
                        study = (Studies)login.Navigate("Studies");

                        //step-12
                        //Load the XA study XA-Bargus 9PID_3267)
                        // PID_12514
                        study.SearchStudy(patientID: PatientIDs[0], Datasource: PACS_A7);
                        study.SelectStudy("Patient ID", PatientIDs[0]);
                        PageLoadWait.WaitForPageLoad(10);
                        PageLoadWait.WaitForFrameLoad(10);
                        study.LaunchStudy();                        
                        PageLoadWait.WaitForPageLoad(30);
                        PageLoadWait.WaitForFrameLoad(30);
                        PageLoadWait.WaitForAllViewportsToLoad(40);
                        PageLoadWait.WaitForThumbnailsToLoad(40);
                    }
                    else
                        throw new Exception(e.StackTrace);
                }
                

                Boolean res12 = true;
                IList<IWebElement> thumbnail_12_1 = BasePage.Driver.FindElements(By.CssSelector(".thumbnailImage"));

                //-- verify 1 thumbnail image is displayed---

                //Validation[6]=1

                if (thumbnail_12_1.Count != Int32.Parse(Validation[6]))
                {
                    res12 = false;
                    Logger.Instance.ErrorLog("Thumbnail count is not equal to 1 - verified failed");
                }

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status12 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (res12 && status12)
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
                study.CloseStudy();

                //--Step-13
                //MG Study BAL-MG10 (ALMG100010) Accssion - 2205587,
                //10 images in two series 14626, 14627

                study.SearchStudy(AccessionNo: AccessionNumbers[1], Datasource: EA_131);
                study.SelectStudy("Accession", AccessionNumbers[1]);
                study.LaunchStudy();
               
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                //In auto mode the two series 14626, 14627 are put together to display two thumbnails 
                //one for each series. Each series has 5 images

                Boolean res13 = true;
                IList<IWebElement> thumbnail_2 = BasePage.Driver.FindElements(By.CssSelector(".thumbnailImage"));

                //-- verify 2 thumbnail images are displayed---

                //Validation[7]=2

                if (thumbnail_2.Count != Int32.Parse(Validation[7]))
                {
                    res13 = false;
                    Logger.Instance.ErrorLog("Thumbnail count is not equal to 2 - verified failed");
                }

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status13 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (res13 && status13)
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
                study.CloseStudy();

                //------------End of script---
                //Logout - step-14
                login.Logout();
                ExecutedSteps++;
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

                //Return Result
                return result;
            }
            finally
            {


                //Modal    Thumb     Viewer   
                //CT --  -- Series  -- Series
                //US --  -- Image  -- Series  
                //XA --  -- Image  -- Series    
                //MG --  -- Image  -- Series 

                String username = Config.adminUserName;
                String password = Config.adminPassword;

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);
                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain("SuperAdminGroup");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();

                string[] Mod = { "CT", "US", "XA", "MG" };
                for (int i = 0; i < Mod.Length; i++)
                {
                    domain.ModalityDropDown().SelectByText(Mod[i]);
                    if (Mod[i] == "CT")
                        domain.SelectRadioBtn("ThumbSplitRadioButtons", "Series");
                    else
                        domain.SelectRadioBtn("ThumbSplitRadioButtons", "Image");

                    domain.SelectRadioBtn("ScopeRadioButtons", "Series");
                }
                domain.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(30);


            }
        }

        
    }
}




