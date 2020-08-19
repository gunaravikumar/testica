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
using OpenQA.Selenium.Remote;

namespace Selenium.Scripts.Tests
{
    class DicomSOP : BasePage
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
        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public DicomSOP(String classname)
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
        /// Test 161498 - Specific study issues in ICA viewer
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161498(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            IList<string> childnode = null;
            DomainManagement domainmanagement = null;
            Studies studies = null;
            StudyViewer viewer = new StudyViewer();            
            string DicomMessagingServices = @"C:\WebAccess\WebAccess\Config\DicomMessagingServices.xml";
            string nodepath = "/presentationContext[@abstractSyntax='MR']";
            BluRingViewer bluViewer = new BluRingViewer();
            TestCompleteAction action = new TestCompleteAction();
            try
            {
                //PreConditions
                //Run Service Tool and goto Viewer tab and subtab protocols, select modality MR and enable video mode
                int ImgCount1 = 0;
                int ImgCount2 = 0;
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Viewer");
                wpfobject.GetTabWpf(1).SelectTabPage(2);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                var comboBox_mod = wpfobject.GetComboBox("ComboBox_Modality");
                comboBox_mod.Select("MR");
                wpfobject.WaitTillLoad();
                IUIItem[] Radio = WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("RadioButton"));
                Radio[5].Click();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();

                //Edit WebAccess\Config\DicomMessagingServices.xml, find the section in StoreSCP for MR, and comment out all transfer syntax except Little Endian (forcing the datasource to send uncompressed data)
                childnode = basepage.ReadChildNodes(DicomMessagingServices, nodepath).Split('>').Select(node => node + ">").ToList();
                childnode.Remove(">");
                childnode.Remove("<transferSyntax name=\"LittleEndian\" />");
                basepage.InsertNode(DicomMessagingServices, nodepath, "<transferSyntax name=\"LittleEndian\" />", true);

                Thread.Sleep(60000);

                servicetool.LaunchServiceTool();
                servicetool.RestartIISandWindowsServices();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();

                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                Dictionary<Object, String> createDomain = domainmanagement.CreateDomainAttr();
                domainmanagement.CreateDomain(createDomain);
                string DomainAdminUsername = createDomain[DomainManagement.DomainAttr.UserID];
                string DomainAdminPassword = createDomain[DomainManagement.DomainAttr.Password];
                login.Logout();
                //Step 1: Login to ICA
                login.LoginIConnect(DomainAdminUsername, DomainAdminPassword);
                UserPreferences userpreferences = new UserPreferences();
                userpreferences.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                userpreferences.ModalityDropdown().SelectByText("MR");
                userpreferences.ThumbnailSplittingSeriesRadioBtn().Click();
                userpreferences.ViewingScopeSeriesRadioBtn().Click();
                userpreferences.CloseUserPreferences();
                ExecutedSteps++;

                //Step 2: Search the study that has Last name 666111
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: "666111");
                if (studies.CheckStudy("Patient ID", "FormalTest_06-666111"))
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

                //Step 3: Load the study
                studies.SelectStudy("Patient ID", "FormalTest_06-666111");                
                bluViewer = BluRingViewer.LaunchBluRingViewer();
                if (bluViewer.ThumbnailLoadedIndicator(0).Count == 1)
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
                //Step 4: Scroll to Image Number 106
                IWebElement ele = bluViewer.GetElement(BasePage.SelectorType.CssSelector, bluViewer.Activeviewport);
                ImgCount1 = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(1) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                for (int i = 0; i < 2; i++)
                {
                    action.MouseScroll(ele, "down", "1");
                    Thread.Sleep(5000);
                }
                BluRingViewer.WaitforViewports(300);
                ImgCount2 = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(1) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                if (ImgCount2 == ImgCount1 + 2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("step4_ImgCount1 " + ImgCount1);
                    Logger.Instance.InfoLog("step4_ImgCount2 " + ImgCount2);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 5: Verify rest of the images
                ImgCount1 = ImgCount2;
                action.MouseScroll(ele, "down", "1");
                Thread.Sleep(5000);
                BluRingViewer.WaitforViewports(300);
                ImgCount2 = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(1) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                if (ImgCount2 == ImgCount1 + 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    Logger.Instance.InfoLog("step5_ImgCount1 " + ImgCount1);
                    Logger.Instance.InfoLog("step5_ImgCount2 " + ImgCount2);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 6: close the study viewer
                bluViewer.CloseBluRingViewer();
                ExecutedSteps++;


                //Step 7: Load a large CT study which has many images in a series
                studies.SearchStudy(AccessionNo: "B6369075", Datasource: login.GetHostName(Config.PACS2));
                studies.SelectStudy("Accession", "B6369075");
                bluViewer = BluRingViewer.LaunchBluRingViewer();
                if (bluViewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_studypanel + ":nth-of-type(1)")))
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

                //Step 8: Scroll the images of the series
                bluViewer.SetViewPort(1, 1);
                ele = bluViewer.GetElement(BasePage.SelectorType.CssSelector, bluViewer.Activeviewport);
                ImgCount1 = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(2) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                for (int i = 0; i < 2; i++)
                {
                    action.MouseScroll(ele, "down", "1");
                    Thread.Sleep(5000);
                }
                BluRingViewer.WaitforViewports(300);
                ImgCount2 = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(2) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                bool step8_1 = ImgCount2 == ImgCount1 + 2;
                Logger.Instance.InfoLog("step8_1_ImgCount1 " + ImgCount1);
                Logger.Instance.InfoLog("step8_1_ImgCount2 " + ImgCount2);

                ImgCount1 = ImgCount2;
                for (int i = 0; i < 3; i++)
                {
                    action.MouseScroll(ele, "down", "1");
                    Thread.Sleep(5000);
                }
                BluRingViewer.WaitforViewports(300);
                ImgCount2 = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(2) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                bool step8_2 = ImgCount2 == ImgCount1 + 3;
                Logger.Instance.InfoLog("step8_2_ImgCount1 " + ImgCount1);
                Logger.Instance.InfoLog("step8_2_ImgCount2 " + ImgCount2);

                ImgCount1 = ImgCount2;
                for (int i = 0; i < 5; i++)
                {
                    action.MouseScroll(ele, "down", "1");
                    Thread.Sleep(5000);
                }
                BluRingViewer.WaitforViewports(300);
                ImgCount2 = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(2) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                bool step8_3 = ImgCount2 == ImgCount1 + 5;
                Logger.Instance.InfoLog("step8_3_ImgCount1 " + ImgCount1);
                Logger.Instance.InfoLog("step8_3_ImgCount2 " + ImgCount2);

                if (step8_1 && step8_2 && step8_3)
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

                //Step 9: Ensure scrolling the images in all the series of the loaded study
                bluViewer.SetViewPort(2, 1);
                ele = bluViewer.GetElement(BasePage.SelectorType.CssSelector, bluViewer.Activeviewport);
                ImgCount1 = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(3) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                for (int i = 0; i < 2; i++)
                {
                    action.MouseScroll(ele, "down", "1");
                    Thread.Sleep(5000);
                }
                BluRingViewer.WaitforViewports(300);
                ImgCount2 = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(3) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                bool step9_1 = ImgCount2 == ImgCount1 + 2;
                Logger.Instance.InfoLog("step9_1_ImgCount1 " + ImgCount1);
                Logger.Instance.InfoLog("step9_1_ImgCount2 " + ImgCount2);

                ImgCount1 = ImgCount2;
                for (int i = 0; i < 3; i++)
                {
                    action.MouseScroll(ele, "down", "1");
                    Thread.Sleep(5000);
                }
                BluRingViewer.WaitforViewports(300);
                ImgCount2 = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(3) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                bool step9_2 = ImgCount2 == ImgCount1 + 3;
                Logger.Instance.InfoLog("step9_2_ImgCount1 " + ImgCount1);
                Logger.Instance.InfoLog("step9_2_ImgCount2 " + ImgCount2);

                ImgCount1 = ImgCount2;
                for (int i = 0; i < 10; i++)
                {
                    action.MouseScroll(ele, "down", "1");
                    Thread.Sleep(5000);
                }
                BluRingViewer.WaitforViewports(300);
                ImgCount2 = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(3) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                bool step9_3 = ImgCount2 == ImgCount1 + 10;
                Logger.Instance.InfoLog("step9_3_ImgCount1 " + ImgCount1);
                Logger.Instance.InfoLog("step9_3_ImgCount2 " + ImgCount2);

                if (step9_1 && step9_2 && step9_3)
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
                bluViewer.CloseBluRingViewer();

                //Step 10: Load a large MR study which has many images in a series
                studies.SearchStudy(patientID: "PIC 08-12-46", Datasource: login.GetHostName(Config.EA96));
                studies.SelectStudy("Patient ID", "PIC 08-12-46");
                BluRingViewer.LaunchBluRingViewer();
                BluRingViewer.WaitforViewports(300);
                BluRingViewer.WaitforThumbnails(300);
                BluRingViewer.WaitForPriorsToLoad();
                if (bluViewer.IsElementVisibleInUI(By.CssSelector(BluRingViewer.div_studypanel + ":nth-of-type(1)")))
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

                //Step 11: Scroll the images of the series
                bluViewer.SetViewPort(1, 1);
                ele = bluViewer.GetElement(BasePage.SelectorType.CssSelector, bluViewer.Activeviewport);
                ImgCount1 = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(2) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                for (int i = 0; i < 2; i++)
                {
                    action.MouseScroll(ele, "down", "1");
                    Thread.Sleep(5000);
                }
                BluRingViewer.WaitforViewports(300);
                ImgCount2 = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(2) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                bool step11_1 = ImgCount2 == ImgCount1 + 2;
                Logger.Instance.InfoLog("step11_1_ImgCount1 " + ImgCount1);
                Logger.Instance.InfoLog("step11_1_ImgCount2 " + ImgCount2);

                ImgCount1 = ImgCount2;
                for (int i = 0; i < 3; i++)
                {
                    action.MouseScroll(ele, "down", "1");
                    Thread.Sleep(5000);
                }
                BluRingViewer.WaitforViewports(300);
                ImgCount2 = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(2) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                bool step11_2 = ImgCount2 == ImgCount1 + 3;
                Logger.Instance.InfoLog("step11_2_ImgCount1 " + ImgCount1);
                Logger.Instance.InfoLog("step11_2_ImgCount2 " + ImgCount2);

                ImgCount1 = ImgCount2;
                for (int i = 0; i < 10; i++)
                {
                    action.MouseScroll(ele, "down", "1");
                    Thread.Sleep(5000);
                }
                BluRingViewer.WaitforViewports(300);
                ImgCount2 = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(2) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                bool step11_3 = ImgCount2 == ImgCount1 + 10;
                Logger.Instance.InfoLog("step11_3_ImgCount1 " + ImgCount1);
                Logger.Instance.InfoLog("step11_3_ImgCount2 " + ImgCount2);

                if (step11_1 && step11_2 && step11_3)
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

                //Step 12: Ensure scrolling the images in all the series of the loaded study
                bluViewer.SetViewPort(2, 1);
                ele = bluViewer.GetElement(BasePage.SelectorType.CssSelector, bluViewer.Activeviewport);
                ImgCount1 = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(3) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                for (int i = 0; i < 2; i++)
                {
                    action.MouseScroll(ele, "down", "1");
                    Thread.Sleep(5000);
                }
                BluRingViewer.WaitforViewports(300);
                ImgCount2 = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(3) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                bool step12_1 = ImgCount2 == ImgCount1 + 2;
                Logger.Instance.InfoLog("step12_1_ImgCount1 " + ImgCount1);
                Logger.Instance.InfoLog("step12_1_ImgCount2 " + ImgCount2);

                ImgCount1 = ImgCount2;
                for (int i = 0; i < 3; i++)
                {
                    action.MouseScroll(ele, "down", "1");
                    Thread.Sleep(5000);
                }
                BluRingViewer.WaitforViewports(300);
                ImgCount2 = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(3) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                bool step12_2 = ImgCount2 == ImgCount1 + 3;
                Logger.Instance.InfoLog("step12_2_ImgCount1 " + ImgCount1);
                Logger.Instance.InfoLog("step12_2_ImgCount2 " + ImgCount2);

                ImgCount1 = ImgCount2;
                for (int i = 0; i < 10; i++)
                {
                    action.MouseScroll(ele, "down", "1");
                    Thread.Sleep(5000);
                }
                BluRingViewer.WaitforViewports(300);
                ImgCount2 = int.Parse(BasePage.Driver.FindElement(By.CssSelector("div.viewerContainer:nth-of-type(3) " + BluRingViewer.div_StackSlider)).GetAttribute("innerHTML"));
                bool step12_3 = ImgCount2 == ImgCount1 + 10;
                Logger.Instance.InfoLog("step12_3_ImgCount1 " + ImgCount1);
                Logger.Instance.InfoLog("step12_3_ImgCount2 " + ImgCount2);

                if (step12_1 && step12_2 && step12_3)
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
                //Step 13: Close the study viewer
                bluViewer.CloseBluRingViewer();
                ExecutedSteps++;

                login.Logout();
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
                try
                {
                    foreach (string node in childnode)
                    {
                        basepage.InsertNode(DicomMessagingServices, nodepath, node, false);
                    }
                }
                catch (Exception) { }
                try
                {
                    servicetool.LaunchServiceTool();
                    servicetool.NavigateToTab("Viewer");
                    wpfobject.GetTabWpf(1).SelectTabPage(2);
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickButton("Modify", 1);
                    var comboBox_mod = wpfobject.GetComboBox("ComboBox_Modality");
                    comboBox_mod.Select("MR");
                    wpfobject.WaitTillLoad();
                    IUIItem[] Radio = WpfObjects._mainWindow.GetMultiple(SearchCriteria.ByClassName("RadioButton"));
                    Radio[6].Click();
                    wpfobject.WaitTillLoad();
                    wpfobject.ClickButton("Apply", 1);
                    wpfobject.WaitTillLoad();
                    servicetool.RestartIISandWindowsServices();
                    wpfobject.WaitTillLoad();
                    servicetool.CloseServiceTool();
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Test 161495-1 - Photometric Interpretation (PI) - "Application.EnableWebGLRendering key to true in web.config" 
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>

        public TestCaseResult Test_161495_1(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            Studies studies = new Studies();
            BluRingViewer viewer = new BluRingViewer();
            string[] PatientID = null;
            string patientID = string.Empty;
            string[] DicomPath = null;
            string[] FilePath = null;
            DicomClient client = new DicomClient();
            HPLogin hplogin = null;
            HPHomePage hphome = null;
            WorkFlow workflow = null;

            try
            {
                // precondition:
                BasePage basepage = new BasePage();
                login.ChangeNodeValue(Config.FileLocationPath, "/EnableWebGLRendering", "true");
                servicetool.RestartIISUsingexe();


                DicomPath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('=');
                //Step 1 to 4
                //JPEG50
                FilePath = Directory.GetFiles(Config.TestDataPath + DicomPath[0], "*.*", SearchOption.AllDirectories);
                PatientID = FilePath.Select(fp => Path.GetFileNameWithoutExtension(fp)).ToArray();
                foreach (string path in FilePath)
                {
                    client.AddRequest(new DicomCStoreRequest(path));
                    client.Send(Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);
                }
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                for (int i = 0; i < 4; i++)
                {
                    patientID = PatientID[i];
                    studies = (Studies)login.Navigate("Studies");
                    studies.SearchStudy(patientID: patientID, Datasource: login.GetHostName(Config.DestEAsIp));
                    PageLoadWait.WaitForLoadingMessage(60);
                    PageLoadWait.WaitForSearchLoad();
                    studies.SelectStudy("Patient ID", patientID);
                    viewer = BluRingViewer.LaunchBluRingViewer();
                    if (viewer.GetViewPortCount(1) != 1)
                        viewer.ChangeViewerLayout("1x1");
                    BluRingViewer.WaitforViewports();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport)))
                    {
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
                }

                //Step 5: 
                login.Logout();
                BasePage.Driver.Navigate().GoToUrl("https://" + Config.DestEAsIp + "/webadmin");
                hplogin = new HPLogin();
                hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, destea: true);
                workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                foreach (string pid in PatientID)
                {
                    workflow.HPSearchStudy("PatientID", pid);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                        workflow.HPDeleteStudy();
                }
                hplogin.LogoutHPen();
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 6: 
                servicetool.RestartIISUsingexe();
                login.CreateNewSesion();
                result.steps[++ExecutedSteps].status = "Pass";

                //JPEG70
                //Step 7:
                FilePath = Directory.GetFiles(Config.TestDataPath + DicomPath[1], "*.*", SearchOption.AllDirectories);
                PatientID = FilePath.Select(fp => Path.GetFileNameWithoutExtension(fp)).ToArray();
                foreach (string path in FilePath)
                {
                    client.AddRequest(new DicomCStoreRequest(path));
                    client.Send(Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);
                }
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 8:
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 9 to 15:
                for (int i = 0; i < 7; i++)
                {
                    patientID = PatientID[i];
                    studies = (Studies)login.Navigate("Studies");
                    studies.SearchStudy(patientID: patientID, Datasource: login.GetHostName(Config.DestEAsIp));
                    PageLoadWait.WaitForLoadingMessage(60);
                    PageLoadWait.WaitForSearchLoad();
                    studies.SelectStudy("Patient ID", patientID);
                    viewer = BluRingViewer.LaunchBluRingViewer();
                    if (viewer.GetViewPortCount(1) != 1)
                        viewer.ChangeViewerLayout("1x1");
                    BluRingViewer.WaitforViewports();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport)))
                    {
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
                }

                //Step 16: 
                login.Logout();
                BasePage.Driver.Navigate().GoToUrl("https://" + Config.DestEAsIp + "/webadmin");
                hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, destea: true);
                workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                foreach (string pid in PatientID)
                {
                    workflow.HPSearchStudy("PatientID", pid);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                        workflow.HPDeleteStudy();
                }
                hplogin.LogoutHPen();
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 17: 
                servicetool.RestartIISUsingexe();
                login.CreateNewSesion();
                result.steps[++ExecutedSteps].status = "Pass";

                //JPEG90
                //Step 18:
                FilePath = Directory.GetFiles(Config.TestDataPath + DicomPath[2], "*.*", SearchOption.AllDirectories);
                PatientID = FilePath.Select(fp => Path.GetFileNameWithoutExtension(fp)).ToArray();
                foreach (string path in FilePath)
                {
                    client.AddRequest(new DicomCStoreRequest(path));
                    client.Send(Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);
                }
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 19:
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 20 to 25:
                for (int i = 0; i < 6; i++)
                {
                    patientID = PatientID[i];
                    studies = (Studies)login.Navigate("Studies");
                    studies.SearchStudy(patientID: patientID, Datasource: login.GetHostName(Config.DestEAsIp));
                    PageLoadWait.WaitForLoadingMessage(60);
                    PageLoadWait.WaitForSearchLoad();
                    studies.SelectStudy("Patient ID", patientID);
                    viewer = BluRingViewer.LaunchBluRingViewer();
                    if (viewer.GetViewPortCount(1) != 1)
                        viewer.ChangeViewerLayout("1x1");
                    BluRingViewer.WaitforViewports();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport)))
                    {
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
                }

                //Step 26: 
                login.Logout();
                BasePage.Driver.Navigate().GoToUrl("https://" + Config.DestEAsIp + "/webadmin");
                hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, destea: true);
                workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                foreach (string pid in PatientID)
                {
                    workflow.HPSearchStudy("PatientID", pid);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                        workflow.HPDeleteStudy();
                }
                hplogin.LogoutHPen();
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 27: 
                servicetool.RestartIISUsingexe();
                login.CreateNewSesion();
                result.steps[++ExecutedSteps].status = "Pass";

                //JPEG91
                //Step 28:
                FilePath = Directory.GetFiles(Config.TestDataPath + DicomPath[3], "*.*", SearchOption.AllDirectories);
                PatientID = FilePath.Select(fp => Path.GetFileNameWithoutExtension(fp)).ToArray();
                foreach (string path in FilePath)
                {
                    client.AddRequest(new DicomCStoreRequest(path));
                    client.Send(Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);
                }
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 29:
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 30 to 34:
                for (int i = 0; i < 5; i++)
                {
                    patientID = PatientID[i];
                    studies = (Studies)login.Navigate("Studies");
                    studies.SearchStudy(patientID: patientID, Datasource: login.GetHostName(Config.DestEAsIp));
                    PageLoadWait.WaitForLoadingMessage(60);
                    PageLoadWait.WaitForSearchLoad();
                    studies.SelectStudy("Patient ID", patientID);
                    viewer = BluRingViewer.LaunchBluRingViewer();
                    if (viewer.GetViewPortCount(1) != 1)
                        viewer.ChangeViewerLayout("1x1");
                    BluRingViewer.WaitforViewports();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport)))
                    {
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
                }

                //Step 35: 
                login.Logout();
                BasePage.Driver.Navigate().GoToUrl("https://" + Config.DestEAsIp + "/webadmin");
                hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, destea: true);
                workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                foreach (string pid in PatientID)
                {
                    workflow.HPSearchStudy("PatientID", pid);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                        workflow.HPDeleteStudy();
                }
                hplogin.LogoutHPen();
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 36: 
                servicetool.RestartIISUsingexe();
                login.CreateNewSesion();
                result.steps[++ExecutedSteps].status = "Pass";

                //RLE 
                //Step 37:
                FilePath = Directory.GetFiles(Config.TestDataPath + DicomPath[4], "*.*", SearchOption.AllDirectories);
                PatientID = FilePath.Select(fp => Path.GetFileNameWithoutExtension(fp)).ToArray();
                foreach (string path in FilePath)
                {
                    client.AddRequest(new DicomCStoreRequest(path));
                    client.Send(Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);
                }
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 38:
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 39 to 44:
                for (int i = 0; i < 6; i++)
                {
                    patientID = PatientID[i];
                    studies = (Studies)login.Navigate("Studies");
                    studies.SearchStudy(patientID: patientID, Datasource: login.GetHostName(Config.DestEAsIp));
                    PageLoadWait.WaitForLoadingMessage(60);
                    PageLoadWait.WaitForSearchLoad();
                    studies.SelectStudy("Patient ID", patientID);
                    viewer = BluRingViewer.LaunchBluRingViewer();
                    if (viewer.GetViewPortCount(1) != 1)
                        viewer.ChangeViewerLayout("1x1");
                    BluRingViewer.WaitforViewports();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport)))
                    {
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
                }

                //Step 45: 
                login.Logout();
                BasePage.Driver.Navigate().GoToUrl("https://" + Config.DestEAsIp + "/webadmin");
                hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, destea: true);
                workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                foreach (string pid in PatientID)
                {
                    workflow.HPSearchStudy("PatientID", pid);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                        workflow.HPDeleteStudy();
                }
                hplogin.LogoutHPen();
                servicetool.RestartIISUsingexe();
                login.CreateNewSesion();
                result.steps[++ExecutedSteps].status = "Pass";

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
        /// Test 161495-2 - Photometric Interpretation (PI) - "Application.EnableWebGLRendering key to false in web.config" 
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>

        public TestCaseResult Test_161495_2(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            Studies studies = new Studies();
            BluRingViewer viewer = new BluRingViewer();
            string[] PatientID = null;
            string patientID = string.Empty;
            string[] DicomPath = null;
            string[] FilePath = null;
            DicomClient client = new DicomClient();
            HPLogin hplogin = null;
            HPHomePage hphome = null;
            WorkFlow workflow = null;

            try
            {
                // precondition:
                BasePage basepage = new BasePage();
                login.ChangeNodeValue(Config.FileLocationPath, "/EnableWebGLRendering", "false");
                servicetool.RestartIISUsingexe();

                DicomPath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('=');
                //Step 1 to 4
                //JPEG50
                FilePath = Directory.GetFiles(Config.TestDataPath + DicomPath[0], "*.*", SearchOption.AllDirectories);
                PatientID = FilePath.Select(fp => Path.GetFileNameWithoutExtension(fp)).ToArray();
                foreach (string path in FilePath)
                {
                    client.AddRequest(new DicomCStoreRequest(path));
                    client.Send(Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);
                }
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                for (int i = 0; i < 4; i++)
                {
                    patientID = PatientID[i];
                    studies = (Studies)login.Navigate("Studies");
                    studies.SearchStudy(patientID: patientID, Datasource: login.GetHostName(Config.DestEAsIp));
                    PageLoadWait.WaitForLoadingMessage(60);
                    PageLoadWait.WaitForSearchLoad();
                    studies.SelectStudy("Patient ID", patientID);
                    viewer = BluRingViewer.LaunchBluRingViewer();
                    if (viewer.GetViewPortCount(1) != 1)
                        viewer.ChangeViewerLayout("1x1");
                    BluRingViewer.WaitforViewports();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport)))
                    {
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
                }

                //Step 5: 
                login.Logout();
                BasePage.Driver.Navigate().GoToUrl("https://" + Config.DestEAsIp + "/webadmin");
                hplogin = new HPLogin();
                hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, destea: true);
                workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                foreach (string pid in PatientID)
                {
                    workflow.HPSearchStudy("PatientID", pid);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                        workflow.HPDeleteStudy();
                }
                hplogin.LogoutHPen();
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 6: 
                servicetool.RestartIISUsingexe();
                login.CreateNewSesion();
                result.steps[++ExecutedSteps].status = "Pass";

                //JPEG70
                //Step 7:
                FilePath = Directory.GetFiles(Config.TestDataPath + DicomPath[1], "*.*", SearchOption.AllDirectories);
                PatientID = FilePath.Select(fp => Path.GetFileNameWithoutExtension(fp)).ToArray();
                foreach (string path in FilePath)
                {
                    client.AddRequest(new DicomCStoreRequest(path));
                    client.Send(Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);
                }
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 8:
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 9 to 15:
                for (int i = 0; i < 7; i++)
                {
                    patientID = PatientID[i];
                    studies = (Studies)login.Navigate("Studies");
                    studies.SearchStudy(patientID: patientID, Datasource: login.GetHostName(Config.DestEAsIp));
                    PageLoadWait.WaitForLoadingMessage(60);
                    PageLoadWait.WaitForSearchLoad();
                    studies.SelectStudy("Patient ID", patientID);
                    viewer = BluRingViewer.LaunchBluRingViewer();
                    if (viewer.GetViewPortCount(1) != 1)
                        viewer.ChangeViewerLayout("1x1");
                    BluRingViewer.WaitforViewports();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport)))
                    {
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
                }

                //Step 16: 
                login.Logout();
                BasePage.Driver.Navigate().GoToUrl("https://" + Config.DestEAsIp + "/webadmin");
                hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, destea: true);
                workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                foreach (string pid in PatientID)
                {
                    workflow.HPSearchStudy("PatientID", pid);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                        workflow.HPDeleteStudy();
                }
                hplogin.LogoutHPen();
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 17: 
                servicetool.RestartIISUsingexe();
                login.CreateNewSesion();
                result.steps[++ExecutedSteps].status = "Pass";

                //JPEG90
                //Step 18:
                FilePath = Directory.GetFiles(Config.TestDataPath + DicomPath[2], "*.*", SearchOption.AllDirectories);
                PatientID = FilePath.Select(fp => Path.GetFileNameWithoutExtension(fp)).ToArray();
                foreach (string path in FilePath)
                {
                    client.AddRequest(new DicomCStoreRequest(path));
                    client.Send(Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);
                }
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 19:
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 20 to 25:
                for (int i = 0; i < 6; i++)
                {
                    patientID = PatientID[i];
                    studies = (Studies)login.Navigate("Studies");
                    studies.SearchStudy(patientID: patientID, Datasource: login.GetHostName(Config.DestEAsIp));
                    PageLoadWait.WaitForLoadingMessage(60);
                    PageLoadWait.WaitForSearchLoad();
                    studies.SelectStudy("Patient ID", patientID);
                    viewer = BluRingViewer.LaunchBluRingViewer();
                    if (viewer.GetViewPortCount(1) != 1)
                        viewer.ChangeViewerLayout("1x1");
                    BluRingViewer.WaitforViewports();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport)))
                    {
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
                }

                //Step 26: 
                login.Logout();
                BasePage.Driver.Navigate().GoToUrl("https://" + Config.DestEAsIp + "/webadmin");
                hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, destea: true);
                workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                foreach (string pid in PatientID)
                {
                    workflow.HPSearchStudy("PatientID", pid);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                        workflow.HPDeleteStudy();
                }
                hplogin.LogoutHPen();
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 27: 
                servicetool.RestartIISUsingexe();
                login.CreateNewSesion();
                result.steps[++ExecutedSteps].status = "Pass";

                //JPEG91
                //Step 28:
                FilePath = Directory.GetFiles(Config.TestDataPath + DicomPath[3], "*.*", SearchOption.AllDirectories);
                PatientID = FilePath.Select(fp => Path.GetFileNameWithoutExtension(fp)).ToArray();
                foreach (string path in FilePath)
                {
                    client.AddRequest(new DicomCStoreRequest(path));
                    client.Send(Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);
                }
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 29:
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 30 to 34:
                for (int i = 0; i < 5; i++)
                {
                    patientID = PatientID[i];
                    studies = (Studies)login.Navigate("Studies");
                    studies.SearchStudy(patientID: patientID, Datasource: login.GetHostName(Config.DestEAsIp));
                    PageLoadWait.WaitForLoadingMessage(60);
                    PageLoadWait.WaitForSearchLoad();
                    studies.SelectStudy("Patient ID", patientID);
                    viewer = BluRingViewer.LaunchBluRingViewer();
                    if (viewer.GetViewPortCount(1) != 1)
                        viewer.ChangeViewerLayout("1x1");
                    BluRingViewer.WaitforViewports();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport)))
                    {
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
                }

                //Step 35: 
                login.Logout();
                BasePage.Driver.Navigate().GoToUrl("https://" + Config.DestEAsIp + "/webadmin");
                hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, destea: true);
                workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                foreach (string pid in PatientID)
                {
                    workflow.HPSearchStudy("PatientID", pid);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                        workflow.HPDeleteStudy();
                }
                hplogin.LogoutHPen();
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 36: 
                servicetool.RestartIISUsingexe();
                login.CreateNewSesion();
                result.steps[++ExecutedSteps].status = "Pass";

                //RLE 
                //Step 37:
                FilePath = Directory.GetFiles(Config.TestDataPath + DicomPath[4], "*.*", SearchOption.AllDirectories);
                PatientID = FilePath.Select(fp => Path.GetFileNameWithoutExtension(fp)).ToArray();
                foreach (string path in FilePath)
                {
                    client.AddRequest(new DicomCStoreRequest(path));
                    client.Send(Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);
                }
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 38:
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].status = "Pass";

                //Step 39 to 44:
                for (int i = 0; i < 6; i++)
                {
                    patientID = PatientID[i];
                    studies = (Studies)login.Navigate("Studies");
                    studies.SearchStudy(patientID: patientID, Datasource: login.GetHostName(Config.DestEAsIp));
                    PageLoadWait.WaitForLoadingMessage(60);
                    PageLoadWait.WaitForSearchLoad();
                    studies.SelectStudy("Patient ID", patientID);
                    viewer = BluRingViewer.LaunchBluRingViewer();
                    if (viewer.GetViewPortCount(1) != 1)
                        viewer.ChangeViewerLayout("1x1");
                    BluRingViewer.WaitforViewports();
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                    if (viewer.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport)))
                    {
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
                }

                //Step 45: 
                login.Logout();
                BasePage.Driver.Navigate().GoToUrl("https://" + Config.DestEAsIp + "/webadmin");
                hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, destea: true);
                workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                foreach (string pid in PatientID)
                {
                    workflow.HPSearchStudy("PatientID", pid);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                        workflow.HPDeleteStudy();
                }
                hplogin.LogoutHPen();
                servicetool.RestartIISUsingexe();
                login.CreateNewSesion();
                result.steps[++ExecutedSteps].status = "Pass";

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
                // precondition:
                BasePage basepage = new BasePage();
                login.ChangeNodeValue(Config.FileLocationPath, "/EnableWebGLRendering", "false");
                servicetool.RestartIISUsingexe();
            }

        }


        /// <summary>
        /// Test 161499 - Studies with Polygonal shutter
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161499(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            Taskbar taskbar;
            DomainManagement domain = null;
            Studies studies = null;
            BluRingViewer bluringviewer;
            TestCompleteAction action = new TestCompleteAction();
            string[] FullPath = null;
            String[] PatientID = null;
            string DS1 = string.Empty;
            string DS1AETitle = string.Empty;
            int DS1Port = 0;

            try
            {
                String[] FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('='); //D:\142151\S000001 = D:\142151\S000002
                String[] Description = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Description")).Split(':');
                PatientID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split('=');
                String LastName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "LastName"));
                DS1 = Config.EA96;
                DS1AETitle = Config.EA96AETitle;
                DS1Port = 12000;

                //PreConditions
                //Enable GSPS in Service tool
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableSavingGSPS();
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                try
                {
                    wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                }
                catch (Exception e)
                { }
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseConfigTool();
                taskbar.Show();

                //Add PR tool to toolbox 
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain("SuperAdminGroup");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(11)"));
                var dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add("Save Annotated Images", group1);
                dictionary.Add("Save Series", group1);
                domain.AddToolsToToolbox(dictionary, addToolAtEnd: true);
                domain.ClickSaveEditDomain();
                login.Logout();

                //Step-1: Import the DX, PR modality studies from the below location :\\10.4.16.130\anonymized_data\Data Sets by VP\iCA\VP_StudViewer\DicomSOP\DX,PR
                //Precondition - Send studies to EA
                var client = new DicomClient();
                String ConcatPath1 = Config.TestDataPath + FilePath[0];
                String ConcatPath2 = Config.TestDataPath + FilePath[1];
                String ConcatPath3 = Config.TestDataPath + FilePath[2];
                String ConcatPath4 = Config.TestDataPath + FilePath[3];
                //Study 1
                FullPath = Directory.GetFiles(ConcatPath1, "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }
                //Study 2
                FullPath = Directory.GetFiles(ConcatPath2, "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }
                //Study 3
                FullPath = Directory.GetFiles(ConcatPath3, "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }
                //Study 4
                FullPath = Directory.GetFiles(ConcatPath4, "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }
                ExecutedSteps++;

                //Step-2: Log in to iCA as valid user and Navigate to studies tab..
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = login.Navigate<Studies>();
                ExecutedSteps++;

                int step3 = ++ExecutedSteps;
                int step4 = ++ExecutedSteps;
                int step5 = ++ExecutedSteps;
                bool[] step3_1 = new bool[4];
                bool[] step4_1 = new bool[4];
                bool[] step5_1 = new bool[4];
                for (int i = 0; i < 4; i++) // for 4 Studies
                {
                    //Step-3: Search and try loading the imported DX , PR studies in the universal viewer .
                    studies.SearchStudy(patientID: PatientID[i], Description: Description[i]);
                    studies.SelectStudy("Patient ID", PatientID[i]);
                    bluringviewer = BluRingViewer.LaunchBluRingViewer();
                    PageLoadWait.WaitForBluRingViewportToLoad(viewport: 2);
                    result.steps[step3].SetPath(testid, step3, i);
                    if (i == 3)
                        step3_1[i] = bluringviewer.CompareImage(result.steps[step3], bluringviewer.ViewPortContainer(), i + 1, 1);
                    else
                        step3_1[i] = bluringviewer.CompareImage(result.steps[step3], bluringviewer.ViewPortContainer(), i + 1);
                    if (step3_1[i])
                    {
                        result.steps[step3].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[step3].description);
                    }
                    else
                    {
                        result.steps[step3].statuslist.Add("Fail");
                        Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[step3].description);
                        result.steps[step3].SetLogs();
                    }

                    //Step-4: Apply few tool operations on the images (W/L , Zoom, Pan etc) from floating toolbox
                    bluringviewer.SelectViewerTool(BluRingTools.Window_Level);
                    bluringviewer.ApplyTool_WindowWidth();
                    bluringviewer.SelectViewerTool(BluRingTools.Interactive_Zoom);
                    bluringviewer.ApplyTool_Zoom();
                    Thread.Sleep(2000);
                    bluringviewer.SelectViewerTool(BluRingTools.Pan);
                    bluringviewer.ApplyTool_Pan();
                    result.steps[step4].SetPath(testid, step4, i);
                    if (i == 3)
                        step4_1[i] = bluringviewer.CompareImage(result.steps[step4], bluringviewer.ViewPortContainer(), i + 1, 1);
                    else
                        step4_1[i] = bluringviewer.CompareImage(result.steps[step4], bluringviewer.ViewPortContainer(), i + 1);
                    if (step4_1[i])
                    {
                        result.steps[step4].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[step4].description);
                    }
                    else
                    {
                        result.steps[step4].statuslist.Add("Fail");
                        Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[step4].description);
                        result.steps[step4].SetLogs();
                    }

                    //Step-5: Select line measurement tool from the floating toolbox and draw a line measurement over the image and click on the save series from the floating toolbox
                    var ele = BasePage.Driver.FindElement(By.CssSelector(bluringviewer.Activeviewport));
                    bluringviewer.SelectViewerTool(BluRingTools.Line_Measurement);
                    bluringviewer.ApplyTool_LineMeasurement(ele.Size.Width / 3, ele.Size.Height / 3, ele.Size.Width / 5, ele.Size.Height / 5);
                    var ThumbnailList = bluringviewer.ThumbnailIndicator(0);
                    int ThumbnailCount = ThumbnailList.Count;
                    bool PRState = bluringviewer.SavePresentationState(BluRingTools.Save_Series);
                    ThumbnailList = bluringviewer.ThumbnailIndicator(0);

                    result.steps[step5].SetPath(testid, step5, i);
                    if (i == 3)
                        step5_1[i] = bluringviewer.CompareImage(result.steps[step5], bluringviewer.ViewPortContainer(), i + 1, 1);
                    else
                        step5_1[i] = bluringviewer.CompareImage(result.steps[step5], bluringviewer.ViewPortContainer(), i + 1);
                    string step5_3 = GetElement(SelectorType.CssSelector, BluRingViewer.div_thumbnails).FindElement(By.CssSelector(BluRingViewer.div_thumbnailModality)).Text;

                    if (ThumbnailList.Count == ThumbnailCount + 1 && PRState && step5_1[i] && step5_3 == "PR")
                    {
                        result.steps[step5].statuslist.Add("Pass");
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[step5].description);
                    }
                    else
                    {
                        result.steps[step5].statuslist.Add("Fail");
                        Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[step5].description);
                        result.steps[step5].SetLogs();
                    } 
                    bluringviewer.CloseBluRingViewer();
                }

                //Step-6: Log out of iCA.
                login.Logout();
                ExecutedSteps++;
                
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
                try
                {
                    //Deleting uploaded study
                    var hplogin = new HPLogin();
                    BasePage.Driver.Navigate().GoToUrl("https://" + DS1 + "/webadmin");
                    var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: "https://" + DS1 + "/webadmin");
                    var workflow = (WorkFlow)hphome.Navigate("Workflow");
                    for (int i = 0; i < 4; i++) // for 4 Studies
                    {
                        workflow.NavigateToLink("Workflow", "Archive Search");
                        workflow.HPSearchStudy("PatientID", PatientID[i]);
                        workflow.HPDeleteStudy();
                    }
                    hplogin.LogoutHPen();

                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Exception due to: " + ex);
                }
            }
        }

        /// <summary>
        /// Test 161500 - DX images are washed out.
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161500(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            Taskbar taskbar;
            DomainManagement domain = null;
            Studies studies = null;
            BluRingViewer bluringviewer;
            TestCompleteAction action = new TestCompleteAction();
            string[] FullPath = null;
            String[] PatientID = null;
            string DS1 = string.Empty;
            string DS1AETitle = string.Empty;
            int DS1Port = 0;

            try
            {
                String[] FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('='); //D:\142151\S000001 = D:\142151\S000002
                String[] Description = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Description")).Split(':');
                PatientID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split(':');
                String LastName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "LastName"));
                DS1 = Config.EA96;
                DS1AETitle = Config.EA96AETitle;
                DS1Port = 12000;

                //PreConditions
                //Enable GSPS in Service tool
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableSavingGSPS();
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                try
                {
                    wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                }
                catch (Exception e)
                { }
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseConfigTool();
                taskbar.Show();

                //Add PR tool to toolbox 
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain("SuperAdminGroup");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");

                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(11)"));
                var dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add("Save Annotated Images", group1);
                dictionary.Add("Save Series", group1);
                domain.AddToolsToToolbox(dictionary, addToolAtEnd: true);
                domain.ClickSaveEditDomain();
                login.Logout();

                //Precondition - Send studies to EA
                var client = new DicomClient();
                String ConcatPath1 = Config.TestDataPath + FilePath[0];
                //2 Studies in 1 folder
                FullPath = Directory.GetFiles(ConcatPath1, "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }

                //Step-1: Log in to iCA with valid credentials.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;

                //Step-2: Navigate to studies tab, Search and load the study in the universal viewer .
                studies = login.Navigate<Studies>();

                studies.SearchStudy(patientID: PatientID[0], Datasource: login.GetHostName(DS1));
                studies.SelectStudy("Patient ID", PatientID[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad(viewport: 2);
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

                //Step-3: Load the PR series in the universal viewer .
                bluringviewer.DropAndDropThumbnails(1, 1, 1, UseDragDrop: true);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step3 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(1) + BluRingViewer.viewportContainer));
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


                //Step-4: Draw few measurements on the PR image and Save the series.
                var ele = BasePage.Driver.FindElement(By.CssSelector(bluringviewer.Activeviewport));
                bluringviewer.SelectViewerTool(BluRingTools.Line_Measurement);
                bluringviewer.ApplyTool_LineMeasurement(ele.Size.Width / 3, ele.Size.Height / 3, ele.Size.Width / 5, ele.Size.Height / 5);
                var ThumbnailList = bluringviewer.ThumbnailIndicator(0);
                int ThumbnailCount = ThumbnailList.Count;
                bool PRState4 = bluringviewer.SavePresentationState(BluRingTools.Save_Series);
                ThumbnailList = bluringviewer.ThumbnailIndicator(0);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step4_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                string step4_2 = GetElement(SelectorType.CssSelector, BluRingViewer.div_thumbnails).FindElement(By.CssSelector(BluRingViewer.div_thumbnailModality)).Text;
                if (ThumbnailList.Count == ThumbnailCount + 1 && PRState4 && step4_1 && step4_2 == "PR")
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

                //Step-5: Close the Viewer and reload the same study again to verify the saved PR series.
                bluringviewer.CloseBluRingViewer();
                studies.SearchStudy(patientID: PatientID[0], Datasource: login.GetHostName(DS1));
                studies.SelectStudy("Patient ID", PatientID[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad(viewport: 2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step5 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step5)
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

                //Step-6: Close the Viewer and Load the below study in the universal viewer .
                bluringviewer.CloseBluRingViewer();
                studies.SearchStudy(patientID: PatientID[1], Datasource: login.GetHostName(DS1));
                studies.SelectStudy("Patient ID", PatientID[1]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad(viewport: 2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step6 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step6)
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

                //Step-7: Apply few tool operations over the image.
                bluringviewer.SelectViewerTool(BluRingTools.Window_Level);
                bluringviewer.ApplyTool_WindowWidth();
                bluringviewer.SelectViewerTool(BluRingTools.Interactive_Zoom);
                bluringviewer.ApplyTool_Zoom();
                Thread.Sleep(2000);
                bluringviewer.SelectViewerTool(BluRingTools.Pan);
                bluringviewer.ApplyTool_Pan();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step7 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step7)
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

                //Step-8: Close the study and log out of iCA.
                bluringviewer.CloseBluRingViewer();
                login.Logout();
                ExecutedSteps++;

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
                try
                {
                    //Deleting uploaded study
                    var hplogin = new HPLogin();
                    BasePage.Driver.Navigate().GoToUrl("https://" + DS1 + "/webadmin");
                    var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: "https://" + DS1 + "/webadmin");
                    var workflow = (WorkFlow)hphome.Navigate("Workflow");
                    for (int i = 0; i < 2; i++) // for 2 Studies
                    {
                        workflow.NavigateToLink("Workflow", "Archive Search");
                        workflow.HPSearchStudy("PatientID", PatientID[i]);
                        workflow.HPDeleteStudy();
                    }
                    hplogin.LogoutHPen();

                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Exception due to: " + ex);
                }
            }
        }

        /// <summary>
        /// Test 161502 - Multi-frame Grayscale Byte Secondary Capture Image Storage - Integrator URL (desktop)
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161502(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            Taskbar taskbar;
            DomainManagement domain = null;
            RoleManagement role = null;
            Studies studies = null;
            BluRingViewer bluringviewer;
            TestCompleteAction action = new TestCompleteAction();
            ehr = new EHR();
            string[] FullPath = null;
            String[] PatientID = null;
            string DS1 = string.Empty;
            string DS1AETitle = string.Empty;
            string ExtractPath = string.Empty;
            var datasource = "PA-A7-WS8"; 
            String IEDownloadPath = @"C:\Users\Administrator\Downloads"; 
            int DS1Port = 0;

            try
            {
                String[] FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('='); //D:\142151\S000001 = D:\142151\S000002
                String[] FName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Fname")).Split(':');
                String[] LName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Lname")).Split(':');
                String[] PatDetails = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName")).Split('=');
                String[] FileName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "DownloadFileName")).Split('=');
                String AccessionList = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Accession"));
                 PatientID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split(':');
                String[] Accession = AccessionList.Split(':');
                DS1 = Config.EA96;
                DS1AETitle = Config.EA96AETitle;
                DS1Port = 12000;
                String URL = "http://" + Config.IConnectIP + "/webaccess";

                //PreConditions
                //Enable GSPS in Service tool
                taskbar = new Taskbar();
                taskbar.Hide();
                servicetool.LaunchServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableSavingGSPS();
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                try
                {
                    wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                }
                catch (Exception e)
                { }
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseConfigTool();
                taskbar.Show();

                //Clear download directory
                try
                {
                    DeleteAllFileFolder(Config.downloadpath);
                    DeleteAllFileFolder(IEDownloadPath);
                }
                catch (Exception ex) { }

                //Integrator Precondtitions
                TestFixtures.UpdateFeatureFixture("bluring", value: "true:BluRing");
                TestFixtures.UpdateFeatureFixture("usersharing", value: "Always enabled");
                TestFixtures.UpdateFeatureFixture("allowshowselector", value: "True");
                TestFixtures.UpdateFeatureFixture("allowshowselectorsearch", value: "True", restart: true);

                //Add PR tool to toolbox 
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domain = login.Navigate<DomainManagement>();
                domain.SearchDomain("SuperAdminGroup");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                domain.SetCheckBoxInEditDomain("savegsps", 0);
                domain.SetCheckBoxInEditDomain("datatransfer", 0);
                domain.SetCheckBoxInEditDomain("datadownload", 0);

                IWebElement group1 = BasePage.Driver.FindElement(By.CssSelector(BasePage.div_toolBoxConfiguration_Groups + ":nth-of-type(11)"));
                var dictionary = new Dictionary<String, IWebElement>();
                dictionary.Add("Save Annotated Images", group1);
                dictionary.Add("Save Series", group1);
                domain.AddToolsToToolbox(dictionary, addToolAtEnd: true);
                domain.ClickSaveEditDomain();

                //Enable Transfer and data download in Role
                role = login.Navigate<RoleManagement>();
                role.SearchRole(Config.adminRoleName, Config.adminGroupName);
                role.SelectRole(Config.adminRoleName);
                role.ClickEditRole();

                //select allow download flag
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                role.SetCheckboxInEditRole("transfer", 0);
                role.SetCheckboxInEditRole("download", 0);
                role.ClickSaveEditRole();
                PageLoadWait.WaitForPageLoad(5);
                login.Logout();

                //Precondition - Send studies to EA
                var client = new DicomClient();
                String ConcatPath1 = Config.TestDataPath + FilePath[0];
                String ConcatPath2 = Config.TestDataPath + FilePath[1];
                //Study 1
                FullPath = Directory.GetFiles(ConcatPath1, "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }
                //Study 2
                FullPath = Directory.GetFiles(ConcatPath2, "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }

                //Step-1: Start the TestEHR program, from Image Load tab set, update details as mentioned
                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Image Load");
                wpfobject.WaitTillLoad();
                ehr.SetCommonParameters(address: URL);
                ehr.SetSelectorOptions(selectoroption:"Study", SearchPriors: "True");
                //ehr.SetSearchKeys_Study(FirstName.Split(':')[0], "First_Name");
                ehr.SetSearchKeys_Study(FName[0], "First_Name");
                ehr.SetSearchKeys_Study(LName[0], "Last_Name");
                ehr.SetSearchKeys_Study(PatientID[0], "Patient_ID");
                ehr.SetSearchKeys_Study(login.GetHostName(DS1), "Datasource");
                String url_1 = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                Logger.Instance.InfoLog("Generated URL- Step-2: " + url_1);
                //Copy/Paste the generated URL in a browser
                login.CreateNewSesion();
                PatientsStudy pstudy = PatientsStudy.LaunchPatientsStudyPage(url_1);
                PageLoadWait.WaitForIntegratorPatientListToLoad();
                var PatientList = pstudy.GetPateintList();
                IList<string> step1 = PatientList["First Name"];

                if (step1.All(p => p.ToLower().Contains(FName[0].ToLower())))
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

                //Step-2 - Load study in the universal viewer with description Sample 8 bit Greyscale Secondary Capture, Open the original DICOM file (.dcm) from the DICOM data source under testing (e.g. EA DICOM), compare the patient and study information on the universal viewer with its original (.dcm) file.
                pstudy.SelectPatinet("Accession", Accession[1]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer(mode: "integrator", showselector: true);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                var step2 = pstudy.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
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

                //Step-3: From universal viewer draw some annotations on the Secondary Capture image
                var ele = BasePage.Driver.FindElement(By.CssSelector(bluringviewer.Activeviewport));
                bluringviewer.SelectViewerTool(BluRingTools.Line_Measurement);
                bluringviewer.ApplyTool_LineMeasurement(ele.Size.Width / 5, ele.Size.Height / 5, ele.Size.Width / 3, ele.Size.Height / 3);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step3 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
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

                //Step-4: Draw some other measurements, e.g. line, angle, rectangle, square&#-3;
                bluringviewer.SelectViewerTool(BluRingTools.Angle_Measurement);
                bluringviewer.ApplyTool_AngleMeasurement(ele.Size.Width / 5, ele.Size.Height / 3, ele.Size.Width / 3, ele.Size.Height / 5);
                bluringviewer.SelectInnerViewerTool(BluRingTools.Draw_Rectangle, BluRingTools.Draw_Ellipse);
                bluringviewer.ApplyTool_DrawRectangle(ele.Size.Width / 5, ele.Size.Height / 2, ele.Size.Width / 3, ele.Size.Height / 3);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step4_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step4_1)
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

                //Step-5: Use calibration tool and do calibration in mm, draw a line measurement.
                bluringviewer.SelectInnerViewerTool(BluRingTools.Calibration_Tool, BluRingTools.Line_Measurement);
                bluringviewer.ApplyTool_Calibration(10, ele.Size.Width / 2, ele.Size.Height / 3, ele.Size.Width / 3, ele.Size.Height / 3);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step5 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step5)
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

                //Step-6: Apply 2D tools e.g. Window Level, zoom, pan, scrolling
                bluringviewer.SelectViewerTool(BluRingTools.Window_Level);
                bluringviewer.ApplyTool_WindowWidth();
                bluringviewer.SelectViewerTool(BluRingTools.Interactive_Zoom);
                bluringviewer.ApplyTool_Zoom();
                Thread.Sleep(2000);
                bluringviewer.SelectViewerTool(BluRingTools.Pan);
                bluringviewer.ApplyTool_Pan();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step6 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step6)
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

                //Step-7: Click to Save Annotated Image
                ele = BasePage.Driver.FindElement(By.CssSelector(bluringviewer.Activeviewport));
                var ThumbnailList = bluringviewer.ThumbnailIndicator(0);
                int ThumbnailCount = ThumbnailList.Count;
                bool PRState7 = bluringviewer.SavePresentationState(BluRingTools.Save_Annotated_Image);
                ThumbnailList = bluringviewer.ThumbnailIndicator(0);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step7_1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                string step7_2 = GetElement(SelectorType.CssSelector, BluRingViewer.div_thumbnails).FindElement(By.CssSelector(BluRingViewer.div_thumbnailModality)).Text;
                if (ThumbnailList.Count == ThumbnailCount + 1 && PRState7 && step7_1 && step7_2 == "PR")
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

                //Step-8: Drag the newly created PR image into viewport
                bluringviewer.DropAndDropThumbnails(1, 1, 1, UseDragDrop: true);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step8 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step8)
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

                //Step-9: From Exam List, load the study into the study panel with study description Sample 4 bit Greyscale Secondary Capture, from the DICOM data source under testing (e.g. EA DICOM)
                bluringviewer.OpenPriors(accession: Accession[0]);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step9 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.viewportContainer));
                if (step9)
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

                //Step-10: Apply measurements, pan, zoom, saving PR on the study panel.
                bluringviewer.SetViewPort(0, 2);
                ele = BasePage.Driver.FindElement(By.CssSelector(bluringviewer.Activeviewport));
                bluringviewer.SelectViewerTool(BluRingTools.Line_Measurement, 2);
                bluringviewer.ApplyTool_LineMeasurement(ele.Size.Width / 3, ele.Size.Height / 3, ele.Size.Width / 5, ele.Size.Height / 5);
                bluringviewer.SelectViewerTool(BluRingTools.Interactive_Zoom, 2);
                bluringviewer.ApplyTool_Zoom();
                Thread.Sleep(2000);
                bluringviewer.SelectViewerTool(BluRingTools.Pan, 2);
                bluringviewer.ApplyTool_Pan();
                ThumbnailList = bluringviewer.ThumbnailIndicator(1);
                ThumbnailCount = ThumbnailList.Count;
                bool PRState10 = bluringviewer.SavePresentationState(BluRingTools.Save_Annotated_Image, panel: 2);
                ThumbnailList = bluringviewer.ThumbnailIndicator(1);
                string step10_1 = GetElement(SelectorType.CssSelector, BluRingViewer.div_thumbnails).FindElement(By.CssSelector(BluRingViewer.div_thumbnailModality)).Text;

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step10_2 = bluringviewer.CompareImage(result.steps[ExecutedSteps], GetElement(SelectorType.CssSelector, BluRingViewer.div_Panel(2) + BluRingViewer.viewportContainer));
                if (ThumbnailList.Count == ThumbnailCount + 1 && PRState10 && step10_2 && step10_1 == "PR")
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

                //Step-11: Launch iCA in another browser then login as any registered user and transfer the study with priors to local after selecting the study
                login.CreateNewSesion();
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = login.Navigate<Studies>();
                //Download file using transfer service
                studies.SearchStudy(LastName: LName[0], FirstName: FName[0], Datasource: login.GetHostName(DS1));
                studies.SelectStudy("Accession", Accession[1]);
                studies.TransferStudy("Local System", SelectallPriors: false, waittime: 180);
                PageLoadWait.WaitForDownload(FileName[0].Split('.')[0], Config.downloadpath, FileName[0].Split('.')[1]);
                Boolean studydownloaded = false;
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                    studydownloaded = BasePage.CheckFile(FileName[0].Split('.')[0], IEDownloadPath, FileName[0].Split('.')[1]);
                else
                    studydownloaded = BasePage.CheckFile(FileName[0].Split('.')[0], Config.downloadpath, FileName[0].Split('.')[1]);
                if (studydownloaded)
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


                //Step-12: Select the same study then click on " Transfer " and transfer the study with priors to other datasource (ex : PACS / EA)
                studies.SearchStudy(LastName: LName[0], FirstName: FName[0], Datasource: login.GetHostName(DS1));
                studies.SelectStudy("Accession", Accession[1]);
                string status = studies.TransferStudy(login.GetHostName(Config.SanityPACS), TimeOut: 900);
                if (string.Equals("succeeded", status, StringComparison.OrdinalIgnoreCase))
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

                //Step-13: Logout from iCA 
                login.Logout();
                ExecutedSteps++;

                //Step-14: Open the DICOM files of the downloaded studies, compare the patient and study information with iCA which was launched via TestEHR
                //Unzip downloaded file
                string ZipPath = String.Empty;
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                    ZipPath = IEDownloadPath + Path.DirectorySeparatorChar + FileName[0];
                else
                    ZipPath = Config.downloadpath + Path.DirectorySeparatorChar + FileName[0];
                ExtractPath = Config.downloadpath;
                string UnzipFolderName = UnZipFolder(ZipPath, ExtractPath);
                Thread.Sleep(5000);
                ExtractPath = Config.downloadpath + Path.DirectorySeparatorChar + UnzipFolderName;

                var file = DicomFile.Open(ExtractPath + Path.DirectorySeparatorChar + "S000003" + Path.DirectorySeparatorChar + FileName[1]);

                var DicomData = file.Dataset;
                string PName = DicomData.Get<string>(DicomTag.PatientName);
                string PID = DicomData.Get<string>(DicomTag.PatientID);
                string PDOB = DicomData.Get<string>(DicomTag.PatientBirthDate);
                string PSex = DicomData.Get<string>(DicomTag.PatientSex);
                string PStudyDate = DicomData.Get<string>(DicomTag.StudyDate);
                string PSOPClassUID = DicomData.Get<string>(DicomTag.SOPClassUID);
                string PAccession = DicomData.Get<string>(DicomTag.AccessionNumber);

                Logger.Instance.InfoLog("Data 1: " + PName + " _ " + PID + " _ " + PDOB + " _ " + PSex + " _ " + PStudyDate + " _ " + PSOPClassUID + " _ " + PAccession + " _ ");

                if (PName.Contains(PatDetails[0]) && PID.Equals(PatDetails[1]) && PDOB.Equals(PatDetails[2]) && PSex.Equals(PatDetails[3]) && PStudyDate.Equals(PatDetails[4]) && PSOPClassUID.Equals(PatDetails[5]) && PAccession.Equals(PatDetails[6]))
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

                //Step-15: Load the transferred study from these data sources that have the transferred study
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = login.Navigate<Studies>();
                studies.SearchStudy(LastName: LName[0], FirstName: FName[0], Datasource: login.GetHostName(Config.SanityPACS));
                studies.SelectStudy("Accession", Accession[1]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                var step15 = pstudy.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step15)
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

                //Step-16: Compare patient information with the same study on original data source that sent.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                var step16 = pstudy.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step16)
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

                bluringviewer.CloseBluRingViewer();
                login.Logout();

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
                try
                {
                    //Deleting uploaded study
                    var hplogin = new HPLogin();
                    BasePage.Driver.Navigate().GoToUrl("https://" + DS1 + "/webadmin");
                    var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: "https://" + DS1 + "/webadmin");
                    var workflow = (WorkFlow)hphome.Navigate("Workflow");
                    workflow.NavigateToLink("Workflow", "Archive Search");
                    workflow.HPSearchStudy("PatientID", PatientID[0]);
                    workflow.HPDeleteStudy();
                    hplogin.LogoutHPen();
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Exception due to: " + ex);
                }
            }
        }

        /// <summary>
        /// Test 164648 - Enhance FindScu network connection drop handling
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164648(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            TestCaseResult result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            Studies studies = null;
            BluRingViewer bluringviewer;
            TestCompleteAction action = new TestCompleteAction();
            String[] PatientID = null;
            var datasource = "PA-A7-WS8";

            try
            {
                String[] FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('='); //D:\142151\S000001 = D:\142151\S000002
                String[] Description = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Description")).Split(':');
                String[] Accession = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "Accession")).Split(':');
                PatientID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID")).Split(':');
                String LastName = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "LastName"));

                //PreConditions: Allow SCU Caching - Enabled by default

                //Step-1: Load a study with multiple series from EA datasource in universal viewer and keep the session idle for 15 minutes
                var LogStartTime = System.DateTime.Now;
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = login.Navigate<Studies>();
                studies.SearchStudy(AccessionNo: Accession[0], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", Accession[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad(viewport: 2);
                //Keep session idle for 15 Mins
                Thread.Sleep(15 * 60 * 1000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step1 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step1)
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

                //Step-2: Close the study, query again with different patient after 15 minutes
                bluringviewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: Accession[1], Datasource: login.GetHostName(Config.EA1));
                studies.SelectStudy("Accession", Accession[1]);
                ExecutedSteps++;

                //Step-3: Load the patient in the universal viewer from the study list
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad(viewport: 2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step3 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
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

                //Step-4: Open developer log
                var LogEndTime = System.DateTime.Now;
                var loggedError = string.Empty;
                String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
                DirectoryInfo taskDirectory = new DirectoryInfo(@"C:\Windows\Temp\");
                FileInfo[] taskFiles = taskDirectory.GetFiles("WebAccessDeveloper-" + Date + "*.log");
                try
                {
                    String LogFilePath = @"C:\Windows\Temp\WebAccessDeveloper-" + Date + "(" + taskFiles.Count() + ")" + ".log";
                    Stream stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    if (File.Exists(LogFilePath))
                    {
                        StreamReader reader = new StreamReader(stream);
                        var LogValues = basepage.ReadDevTraceLog(LogFilePath, LogStartTime, LogEndTime, false);
                        foreach (var entry in LogValues)
                        {
                            if (entry.Value["Source"].Contains("SendFindMessage"))
                                if (entry.Value["Message"].Contains("Connection error, retrying"))
                                {
                                    loggedError = entry.Value["Source"];
                                    break;
                                }
                        }
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("Unable to Read Log file");
                    }

                }
                catch (Exception ex)
                {
                    Logger.Instance.InfoLog("Error while trying to read Developer Log due to: " + ex);
                }
                finally
                {
                    //Validation of message failed in log file
                    if (loggedError.Contains("SendFindMessage"))
                    {
                        result.steps[++ExecutedSteps].StepPass();
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].StepFail("Error log in WebAccessDeveloper log file is not as excepted");
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                }

                //Step-5: Close the Viewer and reload the same study again to verify the saved PR series.
                bluringviewer.CloseBluRingViewer();
                LogStartTime = System.DateTime.Now; 
                studies.SearchStudy(patientID: PatientID[0], Datasource: login.GetHostName(Config.SanityPACS));
                studies.SelectStudy("Patient ID", PatientID[0]);
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad(viewport: 2);
                //Keep session idle for 15 Mins
                Thread.Sleep(5 * 60 * 1000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step5 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step5)
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

                //Step-6: Close the study, query again with different patient after 5 minutes
                bluringviewer.CloseBluRingViewer();
                studies.SearchStudy(patientID: PatientID[1], Datasource: login.GetHostName(Config.SanityPACS));
                studies.SelectStudy("Patient ID", PatientID[1]);
                ExecutedSteps++;

                //Step-7: Load the patient from the study list in the universal viewer
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForBluRingViewportToLoad(viewport: 2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step7 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());
                if (step7)
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

                //Step-8: Open developer log
                LogEndTime = System.DateTime.Now;
                loggedError = string.Empty;
                try
                {
                    String LogFilePath = @"C:\Windows\Temp\WebAccessDeveloper-" + Date + "(" + taskFiles.Count() + ")" + ".log";
                    Stream stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    if (File.Exists(LogFilePath))
                    {
                        StreamReader reader = new StreamReader(stream);
                        var LogValues = basepage.ReadDevTraceLog(LogFilePath, LogStartTime, LogEndTime, false);
                        foreach (var entry in LogValues)
                        {
                            if (entry.Value["Source"].Contains("SendFindMessage"))
                                if (entry.Value["Message"].Contains("Connection error, retrying"))
                                {
                                    loggedError = entry.Value["Source"];
                                    break;
                                }
                        }
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("Unable to Read Log file");
                    }

                }
                catch (Exception ex)
                {
                    Logger.Instance.InfoLog("Error while trying to read Developer Log due to: " + ex);
                }
                finally
                {
                    //Validation of message failed in log file
                    if (loggedError.Contains("SendFindMessage"))
                    {
                        result.steps[++ExecutedSteps].StepPass();
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].StepFail("Error log in WebAccessDeveloper log file is not as excepted");
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                }

                bluringviewer.CloseBluRingViewer();
                login.Logout();

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
        }
    }
}
