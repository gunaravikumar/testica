using System;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Selenium.Scripts.Pages.iConnect;
using OpenQA.Selenium;
using Selenium.Scripts.Pages.HoldingPen;
using OpenQA.Selenium.Interactions;
using System.Threading;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Dicom.Network;
using TestStack.White.UIItems;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.Finders;
using System.Windows.Automation;
using TestStack.White.UIItems.WindowItems;

namespace Selenium.Scripts.Tests
{
    class Error_Handling : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public BasePage basepage { get; set; }

        Studies studies = new Studies();
        ServiceTool servicetool = new ServiceTool();

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public Error_Handling(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            wpfobject = new WpfObjects();
        }

        /// <summary>
        /// Error Handling - Viewer error when image type is not supported
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_160984(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            BasePage basepage = new BasePage();
            //StudyViewer studyViewer;
            BluRingViewer studyViewer;
            XmlNode node1 = null;
            XmlNode node2 = null;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                String adminDomain = Config.adminGroupName;
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String DataSource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Datasource");
                String SOPClassUIDs = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "SOPClass UID");
                String[] SOPClassUID = SOPClassUIDs.Split('=');

                //PreCondition: Ensure MG type is not supported by removing from your iCA Server
                node1 = basepage.RemoveNode(Config.FileLocationPath, "ImageSopClasses", "sopClass", "uid", SOPClassUID[0]);
                node2 = basepage.RemoveNode(Config.FileLocationPath, "ImageSopClasses", "sopClass", "uid", SOPClassUID[1]);
                servicetool.RestartIISUsingexe();

                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.ModalityDropDown().SelectByText(Modality);
                userpref.LayoutDropDown().SelectByText("1x3");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);

                if (node1 != null && node2 != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";  //Step-1
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.Logout();

                //Step-2: Login to iCA webconsole
                login.LoginIConnect(adminusername, adminpassword);
                ExecutedSteps++;    //Step-2

                //Step-3
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy(AccessionNo: Accession, patientID: PatientID, Modality: Modality, Datasource: DataSource);
                studies.ChooseColumns(new string[] { "Accession" });
                studies.SelectStudy("Accession", Accession);
                //studyViewer = studies.LaunchStudy();
                studyViewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(15);
                //PageLoadWait.WaitForThumbnailsToLoad(15);
                //PageLoadWait.WaitForAllViewportsToLoad(15);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step3 = false;
                //step3 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.SeriesViewer_1X1());
                step3 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_viewportNo(1)));
                if (step3)
                {
                    result.steps[ExecutedSteps].status = "Pass";  //Step-3
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
            finally
            {
                try
                {
                    if (node1 != null)
                        basepage.InsertNodeBefore(Config.FileLocationPath, "ImageSopClasses", basepage.GetXMLAsString(node1));

                    if (node2 != null)
                        basepage.InsertNodeBefore(Config.FileLocationPath, "ImageSopClasses", basepage.GetXMLAsString(node2));
                    servicetool.RestartIISUsingexe();
                }
                catch (Exception e)
                {
                    //Log Finally Exception
                    Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }
            }
        }
        
        /// <summary>
        /// Error Handling - Viewer error when image type is not supported
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_160982(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            BasePage basepage = new BasePage();
            BluRingViewer studyViewer;
            MultiDriver = new List<IWebDriver>();
            HPHomePage hphomepage;

            bool IsStudyImported = false;
            String strEAIPAddress = Config.DestEAsIp;
            String strDatasource = basepage.GetHostName(strEAIPAddress);
            String hpUserName = Config.hpUserName;
            String hpPassword = Config.hpPassword;

            String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
            String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                
                String Modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String DataSource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Datasource");
                String uploadFilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                string StudyPath = Config.TestDataPath + uploadFilePath;
                //string StudyPath = Config.TestSuitePath + Path.DirectorySeparatorChar + uploadFilePath;   
                string[] dcmFilePath = Directory.GetFiles(StudyPath, "*.dcm");
                            
                
                //Send a Study to EA
                var client = new DicomClient();
                client.AddRequest(new DicomCStoreRequest(dcmFilePath.FirstOrDefault()));
                client.Send(strEAIPAddress, 12000, false, "SCU", Config.DestEAsAETitle);
                ExecutedSteps++;    //Step-1

                //Step-2: Login to iCA
                BasePage.MultiDriver.Add(BasePage.Driver);
                login.LoginIConnect(adminusername, adminpassword);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.ModalityDropDown().SelectByText(Modality);
                userpref.LayoutDropDown().SelectByText("2x2");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;    //Step-2

                //Step-3: Search the study
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy(AccessionNo: Accession, patientID: PatientID, Modality: Modality, Datasource: strDatasource);
                studies.ChooseColumns(new string[] { "Accession" });
                studies.SelectStudy("Accession", Accession);
                Dictionary<string, string> StudyDisplayed = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { Accession });
                if (StudyDisplayed != null)
                {
                    IsStudyImported = true;
                    result.steps[++ExecutedSteps].status = "Pass";  //Step-3
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4: Open study in viewer
                //studyViewer = studies.LaunchStudy();
                studyViewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(10);
                if (studies.ViewStudy())
                {
                    result.steps[++ExecutedSteps].status = "Pass";  //Step-4
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5: Draw annotation and Save the Annotation
                IList<IWebElement> ThumbnailList = studyViewer.ThumbnailIndicator(0);
                int ThumbnailCountBeforeSave = ThumbnailList.Count;
                
                studyViewer.SelectViewerTool(BluRingTools.Line_Measurement);
                BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(BluRingViewer.div_toolboxContainer)));
                studyViewer.ApplyTool_LineMeasurement();

                result.steps[++ExecutedSteps].SetPath(testid + "_Line_Applied", ExecutedSteps + 1);
                bool status5 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_viewportNo(1)));

                studyViewer.SavePresentationState(BluRingTools.Save_Annotated_Image);

                //This time the GSPS are saved and a thumbnail for the new PR series is displayed 
                //in the Series Thumbnail area.

                string thumbnailCaption = GetElement(SelectorType.CssSelector, BluRingViewer.div_thumbnails).FindElement(By.CssSelector(BluRingViewer.div_thumbnailCaption)).Text;
                Logger.Instance.InfoLog("thumbnailCaption--" + thumbnailCaption);

                if (status5 && studyViewer.ThumbnailIndicator(0).Count == (ThumbnailCountBeforeSave + 1) &&
                    thumbnailCaption.Contains("PR"))
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

                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Step-6: Login to EA and delete referenced image and leave PR
                BasePage.MultiDriver.Add(login.InvokeBrowser(Config.BrowserType));
                login.SetDriver(MultiDriver[1]);

                login.DriverGoTo("https://" + strEAIPAddress + "/webadmin");
                HPLogin hplogin = new HPLogin();
                hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");

                //search study using acc no'
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("Accessionno", Accession);
                IWebElement SearchResultTable = BasePage.Driver.FindElement(By.CssSelector("#tabrow > tbody"));
                IWebElement AccNoLink = SearchResultTable.FindElement(By.LinkText(Accession.ToString().Trim().ToUpper()));
                AccNoLink.Click();

                IWebElement SeriesResultTable = BasePage.Driver.FindElement(By.CssSelector("#results > tbody"));
                IWebElement ModalityLink = SeriesResultTable.FindElement(By.LinkText(Modality.ToString().Trim().ToUpper()));
                ModalityLink.Click();

                //Delete reference images
                workflow.HPDeleteStudy();
                ExecutedSteps++;    //Step-6

                //logout from EA
                hplogin.LogoutHPen();
                MultiDriver[1].Quit();

                //Step-7: In iCA, open the study in viewer
                login.SetDriver(MultiDriver[0]);
                Thread.Sleep(1000 * 60 * 2);        //Due to caching, Wait 2-3 minutes after deleting the referenced image and before loading the study in the viewer
                studies.SelectStudy("Accession", Accession);
                //studyViewer = studies.LaunchStudy();
                studyViewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(15);
                //PageLoadWait.WaitForThumbnailsToLoad(15);
                //PageLoadWait.WaitForAllViewportsToLoad(15);

                result.steps[++ExecutedSteps].SetPath(testid + "_CT_Image", ExecutedSteps + 1);
                bool CTViewportStatus = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_viewportNo(2)));

                result.steps[ExecutedSteps].SetPath(testid + "_PR_Image", ExecutedSteps + 1);
                bool PRViewportStatus = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_viewportNo(1)));

                if (CTViewportStatus && PRViewportStatus)
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

                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);

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
                try {
                    login.ResetDriver();
                    BasePage.Driver.Quit();
                    BasePage.Driver = null;
                    if (IsStudyImported) {
                        //Login to EA and delete Imported Study
                        new Login().DriverGoTo("https://" + strEAIPAddress + "/webadmin");
                        HPLogin hplogin = new HPLogin();
                        hphomepage = hplogin.LoginHPen(hpUserName, hpPassword);
                        WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");

                        //search study using acc no'
                        workflow.NavigateToLink("Workflow", "Archive Search");
                        workflow.HPSearchStudy("PatientID", PatientID);
                        IWebElement SearchResultTable = BasePage.Driver.FindElement(By.CssSelector("#tabrow > tbody"));
                        IWebElement PIDLink = SearchResultTable.FindElement(By.LinkText(PatientID.ToString().Trim().ToUpper()));
                        if (PIDLink.Displayed)
                        {
                            workflow.HPDeleteStudy(); //Delete reference images
                        }
                        //logout from EA
                        hplogin.LogoutHPen();
                    }
                }
                catch (Exception Err)
                {
                    Logger.Instance.ErrorLog("Error in Finally Block: " + Err.Message);
                }

                if (BasePage.Driver != null)
                {
                    BasePage.Driver.Quit();
                    BasePage.Driver = null;
                }
                new Login().DriverGoTo(url);
            }
        }

        /// <summary>
        /// Error Handling - Viewer error when instance query is not supported
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_160983(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            BasePage basepage = new BasePage();
            BluRingViewer studyViewer;
            DomainManagement domainmanagement;
            XmlNode MGNode = null;
            String strDataSource = "";

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                String adminDomain = Config.adminGroupName;
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                strDataSource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Datasource");
                
                //PreCondition-1: Uncheck Instance Query Support checkbox for EA Datasource
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.ChangeInstanceQuerySupportForDataSource(strDataSource, false);
                servicetool.CloseConfigTool();

                //PreCondition-2: Ensure MG type is not supported by removing from your iCA Server
                MGNode = basepage.RemoveNode(Config.DicomMessagingServiceXMLPath, "service[@name=\"StoreSCP\"]", "presentationContext", "abstractSyntax", Modality);
                servicetool.RestartIISUsingexe();

                //PreCondition-3: Thumbnail Splitting for MG Modality in Domain Management
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);

                //Navigate to DomainManagement Tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");

                //Click Edit in DomainManagement Tab
                domainmanagement.SearchDomain(adminDomain);
                domainmanagement.SelectDomain(adminDomain);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                domainmanagement.ModalityDropDown().SelectByText(Modality);
                IWebElement SeriesRadioButton = BasePage.Driver.FindElement(By.CssSelector("input[id$='viewingProtocolsControl_ThumbSplitRadioButtons_1']"));
                SeriesRadioButton.Click();
                domainmanagement.ClickSaveEditDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);

                //PreCondition-4: Thumbnail Splitting for MG Modality in User Preference
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.ModalityDropDown().SelectByText(Modality);
                userpref.LayoutDropDown().SelectByText("1x3");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);

                userpref.SetThumbnailSplittingAtUserLevel(Modality, "series");
                if (MGNode != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";  //Step-1
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                login.Logout();

                //Step-2: Login to iCA webconsole
                login.LoginIConnect(adminusername, adminpassword);
                ExecutedSteps++;    //Step-2

                //Step-3
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy(AccessionNo: Accession, patientID: PatientID, Modality: Modality, Datasource: strDataSource);
                studies.ChooseColumns(new string[] { "Accession" });
                studies.SelectStudy("Accession", Accession);
                //studyViewer = studies.LaunchStudy();
                studyViewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(15);
                //PageLoadWait.WaitForThumbnailsToLoad(15);
                //PageLoadWait.WaitForAllViewportsToLoad(15);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step3 = false;
                step3 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_viewportNo(1)));
                if (step3)
                {
                    result.steps[ExecutedSteps].status = "Pass";  //Step-3
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
            finally
            {
                try
                {

                    if (MGNode != null)
                        basepage.InsertNodeBefore(Config.DicomMessagingServiceXMLPath, "service[@name=\"StoreSCP\"]", basepage.GetXMLAsString(MGNode));

                    servicetool.LaunchServiceTool();
                    wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                    servicetool.ChangeInstanceQuerySupportForDataSource(strDataSource, true);
                    servicetool.CloseConfigTool();
                    servicetool.RestartIISUsingexe();

                    String adminUserName = Config.adminUserName;
                    String adminPassword = Config.adminPassword;
                    String adminDomain = Config.adminGroupName;
                    login.LoginIConnect(adminUserName, adminPassword);
                    domainmanagement = (DomainManagement)login.Navigate("DomainManagement");

                    //Click Edit in DomainManagement Tab
                    domainmanagement.SearchDomain(adminDomain);
                    domainmanagement.SelectDomain(adminDomain);
                    domainmanagement.ClickEditDomain();
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(30);
                    domainmanagement.ModalityDropDown().SelectByText("MG");
                    IWebElement ImageRadioButton = BasePage.Driver.FindElement(By.CssSelector("input[id$='viewingProtocolsControl_ThumbSplitRadioButtons_2']"));
                    ImageRadioButton.Click();
                    domainmanagement.ClickSaveEditDomain();
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(30);
                    PageLoadWait.WaitForLoadingMessage(120);
                    UserPreferences userpref = new UserPreferences();
                    userpref.SetThumbnailSplittingAtUserLevel("MG", "image");
                    login.Logout();
                }
                catch (Exception e)
                {
                    //Log Finally Exception
                    Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }
            }
        }
        
        /// <summary>
        /// Error Handling - Viewer error when remote viewer is down (Viewer Balance error)
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_160980(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            BasePage basepage = new BasePage();
            ServiceTool servicetool = new ServiceTool();
            BluRingViewer studyViewer;
            
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            int ViewerServiceBefore = 0, ViewerServiceAfter = 0;
            bool IsIIS_Stopped = false, IsViewerServiceAdded = false;
            String SecondiCAServerIP = Config.RDMIP;
            String FirstiCAServerIP = Config.IConnectIP;
            String SecondiCAServer = basepage.GetHostName(SecondiCAServerIP);
            String FirstiCAServer = basepage.GetHostName(FirstiCAServerIP);
            
            try
            {
                //Fetch required Test data
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String strDatasource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Datasource");
                
                //Get Dicom Data Source
                string strWebAccessCachePath = Config.WebAccessAmicasP10FilesCache;
                string strWebAccessCacheRemotePath = "\\\\" + SecondiCAServer + Path.DirectorySeparatorChar + strWebAccessCachePath.Replace(':', '$') + Path.DirectorySeparatorChar;// + SecondiCAServer;
                string strWebAccessCacheLocalPath = strWebAccessCachePath + Path.DirectorySeparatorChar;    // + FirstiCAServer;
                
                //Step-1 - Add the remote random server- iCA2.
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.Viewer_Tab);
                servicetool.wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab(ServiceTool.Viewer.Name.ViewerService_tab);
                servicetool.wpfobject.WaitTillLoad();
                Window iCAServiceToolWnd = servicetool.wpfobject.GetMainWindowByTitle(ServiceTool.ConfigTool_Name);
                GroupBox ViewerServiceGroupbox = iCAServiceToolWnd.Get<GroupBox>(SearchCriteria.ByText(ServiceTool.Viewer.Name.ViewerService_group));
                Button AddButton = ViewerServiceGroupbox.Get<Button>(SearchCriteria.ByControlType(ControlType.Button).AndByText(ServiceTool.AddBtn_Name));
                ListView dataGrid = ViewerServiceGroupbox.Get<ListView>(SearchCriteria.ByControlType(ControlType.DataGrid).AndByClassName("ListView"));
                ViewerServiceBefore = dataGrid.Rows.Count;

                AddButton.Click();
                servicetool.wpfobject.WaitForPopUp();
                Window AddViewerServiceWindow = servicetool.wpfobject.GetMainWindowByTitle(ServiceTool.AddAdditionalViewer_Wnd);
                TextBox HostNameTextbox = AddViewerServiceWindow.Get<TextBox>(SearchCriteria.ByControlType(ControlType.Edit).AndIndex(1));
                HostNameTextbox.SetValue(SecondiCAServer);

                var ServiceComboBox = AddViewerServiceWindow.Get(SearchCriteria.ByControlType(ControlType.ComboBox));
                Button OkButton = AddViewerServiceWindow.Get<Button>(SearchCriteria.ByControlType(ControlType.Button).AndByText(ServiceTool.OkBtn_Name));
                OkButton.Click();
                servicetool.wpfobject.WaitTillLoad();
                ViewerServiceAfter = dataGrid.Rows.Count;
                if (ViewerServiceAfter > ViewerServiceBefore)
                    IsViewerServiceAdded = true;
                else
                    throw new Exception("Unable to add Sercond iCA Server as Additional Viewer service");

                dataGrid.Select(ViewerServiceBefore);
                CheckBox LoadBalanceCheckbox = iCAServiceToolWnd.Get<CheckBox>(SearchCriteria.ByControlType(ControlType.CheckBox).AndAutomationId("EnableLocalViewCheckBox"));
                if (LoadBalanceCheckbox != null && LoadBalanceCheckbox.Enabled)
                {
                    if (LoadBalanceCheckbox.Checked)
                    {
                        LoadBalanceCheckbox.Checked = false;
                        servicetool.HandlePopup("Confirm to delete", ServiceTool.OkBtn_Name);
                    }
                }
                else
                    throw new Exception("Unable to UnCheck 'Enable local viewer for load balancing' checkbox");

                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                if (IsViewerServiceAdded)
                {
                    result.steps[++ExecutedSteps].status = "Pass";  //Step-1
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Ensure WebAccessAmicasP10FilesCache folder is empty
                FileUtils.DeleteFilesInRemoteFolder(strWebAccessCacheRemotePath);

                System.IO.DirectoryInfo iCA1CacheFolder = new DirectoryInfo(strWebAccessCachePath);
                foreach (FileInfo file in iCA1CacheFolder.GetFiles())
                {
                    file.Delete();
                }

                //Step-2: Login to iCA and open a study in viewer
                login.LoginIConnect(adminusername, adminpassword);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.ModalityDropDown().SelectByText(Modality);
                userpref.LayoutDropDown().SelectByText("2x2");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);

                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy(AccessionNo: Accession, patientID: PatientID, Modality: Modality, Datasource: strDatasource);
                studies.ChooseColumns(new string[] { "Accession" });
                studies.SelectStudy("Accession", Accession);
                //studyViewer = studies.LaunchStudy();
                studyViewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(10);
                if (studies.ViewStudy())
                {
                    result.steps[++ExecutedSteps].status = "Pass";  //Step-2
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                Dictionary<string, string> StudyDisplayed = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { Accession });
                if (StudyDisplayed != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";  //Step-3
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4_1: Verify on iCA1 Server WebAccessAmicasP10FilesCache Folder is empty
                bool isICA1CacheFolderEmpty = BasePage.IsDirectoryEmpty(strWebAccessCacheLocalPath);
                if (isICA1CacheFolderEmpty)
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog(strWebAccessCachePath + " folder is empty");
                }
                else
                {
                    result.steps[++ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog(strWebAccessCachePath + " folder is not empty, some images are cached");
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4_2: Verify on iCA2 Server, images are cached in WebAccessAmicasP10FilesCache Folder
                List<string> iCA2CacheFiles = FileUtils.GetFileNameFromRemoteFolder(strWebAccessCacheRemotePath);
                if (iCA2CacheFiles.Count > 0)
                {
                    result.steps[ExecutedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Images are cached in " + strWebAccessCacheRemotePath + " folder");
                }
                else
                {
                    result.steps[ExecutedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Images are not cached in " + strWebAccessCacheRemotePath + " folder");
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].status = "Fail";
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Pass";        //Step-4
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                //Step-5: Restart IIS Service on iCA2 Server
                IsIIS_Stopped = servicetool.ResetRemoteIISUsingexe(SecondiCAServer, "STOP");
                if (IsIIS_Stopped)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("IIS service stopped on iCA server - " + SecondiCAServer);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("IIS service not stopped on iCA server - " + SecondiCAServer);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6: In iCA, open the study in viewer
                studies.SelectStudy("Accession", Accession);
                //studyViewer = studies.LaunchStudy();
                studyViewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(15);
                //PageLoadWait.WaitForThumbnailsToLoad(15);
                //PageLoadWait.WaitForAllViewportsToLoad(15);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool ViewportStatus = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_viewportNo(1)));
                if (ViewportStatus)
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

                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);

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
                if (IsIIS_Stopped)
                {
                    servicetool.ResetRemoteIISUsingexe(SecondiCAServer, "START");
                }
                try
                {
                    if (IsViewerServiceAdded)
                    {
                        servicetool.LaunchServiceTool();
                        servicetool.NavigateToTab(ServiceTool.Viewer_Tab);
                        servicetool.wpfobject.WaitTillLoad();
                        servicetool.NavigateSubTab(ServiceTool.Viewer.Name.ViewerService_tab);
                        servicetool.wpfobject.WaitTillLoad();
                        Window iCAServiceToolWnd = servicetool.wpfobject.GetMainWindowByTitle(ServiceTool.ConfigTool_Name);
                        GroupBox ViewerServiceGroupbox = iCAServiceToolWnd.Get<GroupBox>(SearchCriteria.ByText(ServiceTool.Viewer.Name.ViewerService_group));
                        Button DeleteButton = ViewerServiceGroupbox.Get<Button>(SearchCriteria.ByControlType(ControlType.Button).AndByText("Delete"));
                        ListView dataGrid = ViewerServiceGroupbox.Get<ListView>(SearchCriteria.ByControlType(ControlType.DataGrid).AndByClassName("ListView"));
                        if (DeleteButton != null && !DeleteButton.Enabled)
                        {
                            CheckBox LoadBalanceCheckbox = iCAServiceToolWnd.Get<CheckBox>(SearchCriteria.ByControlType(ControlType.CheckBox).AndAutomationId("EnableLocalViewCheckBox"));
                            if (!LoadBalanceCheckbox.Checked)
                            {
                                LoadBalanceCheckbox.Checked = true;
                            }
                        }
                        dataGrid.Select(ViewerServiceBefore);
                        DeleteButton.Click();
                        servicetool.HandlePopup("Confirm to delete", ServiceTool.YesBtn_Name);
                        wpfobject.WaitTillLoad();
                        servicetool.RestartService();
                        servicetool.CloseServiceTool();
                        Logger.Instance.InfoLog("Added Viewer Service removed successfully");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Error while removing added viewer service list. Error: " + ex.Message);
                }
            }
        }
        
        /// <summary>
        /// Error Handling - Viewer error when image rendering timeout is too short
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_160985(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            BasePage basepage = new BasePage();
            BluRingViewer studyViewer;
            
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                String adminDomain = Config.adminGroupName;
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String strDataSource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Datasource");

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(Config.ImagerConfiguration);
                XmlElement propertyData = (XmlElement)xmlDoc.SelectSingleNode("//property[@key='Common.CaptureTimeout']");
                if (propertyData != null)
                {
                    propertyData.SetAttribute("value", "1000"); // Set to new value.
                }
                else
                    throw new Exception("Rendering timeout property not found in ImagerConfiguration.xml file");

                xmlDoc.Save(Config.ImagerConfiguration);
                servicetool.RestartIISUsingexe();
                ExecutedSteps++;    //Step-1

                //Step-2: Login to iCA webconsole
                login.LoginIConnect(adminusername, adminpassword);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.ModalityDropDown().SelectByText(Modality);
                userpref.LayoutDropDown().SelectByText("2x2");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;    //Step-2

                //Step-3
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy(AccessionNo: Accession, patientID: PatientID, Modality: Modality, Datasource: strDataSource);
                Dictionary<string, string> StudyDisplayed = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { Accession });
                if (StudyDisplayed != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";  //Step-3
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4: Select a Study and load it in a Viewer
                studies.ChooseColumns(new string[] { "Accession" });
                studies.SelectStudy("Accession", Accession);
                //studyViewer = studies.LaunchStudy();
                studyViewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(15);
                //PageLoadWait.WaitForThumbnailsToLoad(15);
                //PageLoadWait.WaitForAllViewportsToLoad(15);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step4 = false;
                step4 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_viewportNo(1)));
                if (step4)
                {
                    result.steps[ExecutedSteps].status = "Pass";  //Step-4
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);

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
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(Config.ImagerConfiguration);
                    XmlElement propertyData = (XmlElement)xmlDoc.SelectSingleNode("//property[@key='Common.CaptureTimeout']");
                    if (propertyData != null)
                    {
                        propertyData.SetAttribute("value", "30000"); // Set to new value.
                    }
                    else
                        throw new Exception("Rendering timeout property not found in ImagerConfiguration.xml file");
                    xmlDoc.Save(Config.ImagerConfiguration);
                    servicetool.RestartIISUsingexe();
                }
                catch (Exception ex)
                {
                    //Log Finally Exception
                    Logger.Instance.ErrorLog(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.InnerException);
                }
            }
        }

        /// <summary>
        /// Error Handling - Viewer error when wado url is invalid
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_160979(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            BasePage basepage = new BasePage();
            BluRingViewer studyViewer;
            String iCAServerIP = Config.IConnectIP;
            String iCAServerName = basepage.GetHostName(iCAServerIP);
            String strDataSource = "";

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                String adminDomain = Config.adminGroupName;
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                strDataSource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Datasource");

                new Taskbar().Hide();

                //Step-1: Precondition - EA datasource configured.
                servicetool.LaunchServiceTool();
                if (!servicetool.IsDataSourceExists(strDataSource))
                {
                    throw new Exception(strDataSource + " EA Datasource is not configured");
                }
                wpfobject.WaitTillLoad();
                servicetool.CloseConfigTool();
                ExecutedSteps++;    //Step-1

                //Step-2: Login to iCA webconsole
                login.LoginIConnect(adminusername, adminpassword);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.ModalityDropDown().SelectByText(Modality);
                userpref.LayoutDropDown().SelectByText("2x2");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;    //Step-2

                //Step-3: Search the Study and it should displayed in StudyList
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                //studies.SearchStudy(AccessionNo: Accession, patientID: PatientID, Modality: Modality, Datasource: strDataSource);
                studies.SearchStudy(AccessionNo: Accession, patientID: PatientID, Datasource: strDataSource);
                Dictionary<string, string> StudyDisplayed = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { Accession });
                if (StudyDisplayed != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";  //Step-3
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4: Select a study and open in Viewer
                studies.ChooseColumns(new string[] { "Accession" });
                studies.SelectStudy("Accession", Accession);
                //studyViewer = studies.LaunchStudy();
                studyViewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(10);
                //if (studies.ViewStudy())
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";  //Step-4
                //    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}
                ExecutedSteps++;
                studyViewer.CloseBluRingViewer();
                PageLoadWait.WaitForPageLoad(20);
                
                //Logout 
                login.Logout();

                //Step-5: Configure incorrect value to WADO url
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.ChangeWADOHostNameForDataSource(strDataSource, iCAServerName + "_ERR");
                servicetool.CloseConfigTool();
                servicetool.RestartIISUsingexe();
                ExecutedSteps++;        //Step-5

                //Step-6: Login to Url and open the study in viewer
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                studies = login.Navigate<Studies>();
                //studies.SearchStudy(AccessionNo: Accession, patientID: PatientID, Modality: Modality, Datasource: strDataSource);
                studies.SearchStudy(AccessionNo: Accession, patientID: PatientID, Datasource: strDataSource);
                studies.SelectStudy("Accession", Accession);
                //studyViewer = studies.LaunchStudy();
                studyViewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(15);
                //PageLoadWait.WaitForThumbnailsToLoad(15);
                //PageLoadWait.WaitForAllViewportsToLoad(15);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step6 = false;
                step6 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_viewportNo(1)));
                if (step6)
                {
                    result.steps[ExecutedSteps].status = "Pass";  //Step-6
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
            finally
            {
                try
                {
                    servicetool.LaunchServiceTool();
                    wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                    servicetool.ChangeWADOHostNameForDataSource(strDataSource, iCAServerName);
                    servicetool.CloseConfigTool();
                    servicetool.RestartIISUsingexe();
                }
                catch (Exception e)
                {
                    //Log Finally Exception
                    Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }
                new Taskbar().Show();
            }
        }

        /// <summary>
        /// Error Handling - Viewer error when datasource server is down - Optional
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_160986(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            BasePage basepage = new BasePage();
            BluRingViewer studyViewer;
            
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                String adminDomain = Config.adminGroupName;
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String strDataSource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Datasource");

                //Step-1: Precondition - EA datasource configured.
                servicetool.LaunchServiceTool();
                if (!servicetool.IsDataSourceExists(strDataSource))
                {
                    //Add Destination 1 - Pacs
                    servicetool.AddPacsDatasource(Config.SanityPACS, Config.SanityPACSAETitle, "", Config.pacsadmin, Config.pacspassword);
                }

                wpfobject.WaitTillLoad();
                servicetool.CloseConfigTool();
                ExecutedSteps++;    //Step-1

                //Step-2: Login to iCA webconsole
                login.LoginIConnect(adminusername, adminpassword);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.ModalityDropDown().SelectByText(Modality);
                userpref.LayoutDropDown().SelectByText("2x2");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;    //Step-2

                //Step-3: Search the Study and it should displayed in StudyList
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy(AccessionNo: Accession, patientID: PatientID, Modality: Modality, Datasource: strDataSource);
                Dictionary<string, string> StudyDisplayed = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { Accession });
                if (StudyDisplayed != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";  //Step-3
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4:
                String StopCommand = "";
                String[] arrAmicasProcess = new String[] { "AmicasWatchService", "AmicasWebService", "AmicasService", "AmicasMessagingService" };
                String strkillCommand = @"/IM PROCESS_NAME.exe /T /F";
                for (int item = 0; item < arrAmicasProcess.Length; item++)
                {
                    StopCommand = strkillCommand.Replace("PROCESS_NAME", arrAmicasProcess[item]);
                    KillRemoteProcess(Config.SanityPACS, Config.pacsadmin, Config.pacspassword, StopCommand);
                }
                ExecutedSteps++;
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                //Step-5: Select a study and open in Viewer
                studies.SelectStudy("Accession", Accession);
                //studyViewer = studies.LaunchStudy();
                studyViewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(15);
                //PageLoadWait.WaitForThumbnailsToLoad(15);
                //PageLoadWait.WaitForAllViewportsToLoad(15);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step5 = false;
                step5 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_viewportNo(1)));
                if (step5)
                {
                    result.steps[ExecutedSteps].status = "Pass";  //Step-5
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);

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
                String StartCommand = "";
                String[] arrAmicasServices = new String[] { "AmicasWatchServiceJ", "AmicasWebServiceJ", "AmicasServiceJ", "AmicasMessaging" };
                String strkillCommand = @"net start SERVICE_NAME /T /F";
                for (int item = 0; item < arrAmicasServices.Length; item++)
                {
                    StartCommand = strkillCommand.Replace("SERVICE_NAME", arrAmicasServices[item]);
                    ExecuteRemoteCommand(Config.SanityPACS, Config.pacsadmin, Config.pacspassword, StartCommand);
                }
                Logger.Instance.InfoLog("All 4 PACS Services started successfully");
            }
        }
        
        /// <summary>
        /// Error Handling - Viewer error when remote device is removed from EA server
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_160981(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            HPHomePage homepage;
            BluRingViewer studyViewer;
            BasePage basepage = new BasePage();
            Configure configure = new Configure();
            
            MultiDriver = new List<IWebDriver>();
            bool IsRemoteDeviceRemoved = false;
            String iCAServerIP = Config.IConnectIP;
            String ICAServerName = basepage.GetHostName(iCAServerIP);
            String hpUserName = Config.hpUserName;
            String hpPassword = Config.hpPassword;
            String strEAIPAddress = Config.DestEAsIp;
            String strDataSource = basepage.GetHostName(strEAIPAddress);
            String RemoteDeviceConfigPath = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "ServerConfigFiles" + Path.DirectorySeparatorChar + @"RemoteDevice.xml";

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String DataSource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Datasource");

                new Taskbar().Hide();

                //Enable Instance Query Support for the EA Datasource
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.ChangeInstanceQuerySupportForDataSource(strDataSource, false);
                servicetool.CloseConfigTool();
                servicetool.RestartIISUsingexe();
                ExecutedSteps++;    //Step-1

                //Step-2: Login to iCA webconsole
                BasePage.MultiDriver.Add(BasePage.Driver);
                login.DriverGoTo(login.url);
                login.LoginIConnect(adminusername, adminpassword);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.ModalityDropDown().SelectByText(Modality);
                userpref.LayoutDropDown().SelectByText("2x2");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;    //Step-2

                //Step-3
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                //studies.SearchStudy(AccessionNo: Accession, patientID: PatientID, Modality: Modality, Datasource: DataSource);
                studies.SearchStudy(AccessionNo: Accession, patientID: PatientID, Datasource: DataSource);
                Dictionary<string, string> StudyDisplayed = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { Accession });
                if (StudyDisplayed != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";  //Step-3
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4: Login to EA and delete remote device for iCA Server
                /*
                if (!NodeExist(xmlFilePath: RemoteDeviceConfigPath, NodePath: "RemoteDeviceList/RemoteDevice[@name='" + ICAServerName + "']"))
                {
                    AddNodeInRemoteDeviceConfigFile(ICAHostName: ICAServerName, IPAddress: iCAServerIP);
                    Logger.Instance.InfoLog("New RemoteDevice Node '" + ICAServerName + "' added to " + RemoteDeviceConfigPath);
                    Thread.Sleep(500);
                }

                if (!NodeExist(xmlFilePath: RemoteDeviceConfigPath, NodePath: "RemoteDeviceList/RemoteDevice[@name='" + ICAServerName + "']"))
                    throw new Exception("No RemoteDevice node found with the device name '" + ICAServerName + "' in the configuration file - RemoteDevice.xml");
                */
                BasePage.MultiDriver.Add(login.InvokeBrowser(Config.BrowserType));
                login.SetDriver(MultiDriver[1]);

                login.DriverGoTo("https://" + strEAIPAddress + "/webadmin");
                HPLogin hplogin = new HPLogin();
                homepage = hplogin.LoginHPen(hpUserName, hpPassword);
                PageLoadWait.WaitForHPPageLoad(20);
                configure = (Configure)homepage.Navigate("Configure");
                configure.NavigateToTab("remotedevices");
                IsRemoteDeviceRemoved = configure.DeleteRemoteDevice(ICAServerName);
                PageLoadWait.WaitForHPPageLoad(20);
                if (IsRemoteDeviceRemoved)
                {
                    result.steps[++ExecutedSteps].status = "Pass";  //Step-4
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //logout from EA
                hplogin.LogoutHPen();
                MultiDriver[1].Quit();

                //Step-5: In iCA, open the study in viewer
                login.SetDriver(MultiDriver[0]);
                studies = login.Navigate<Studies>();
                studies.ChooseColumns(new string[] { "Accession" });
                studies.SelectStudy("Accession", Accession);
                //studyViewer = studies.LaunchStudy();
                studyViewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(15);
                //PageLoadWait.WaitForThumbnailsToLoad(15);
                //PageLoadWait.WaitForAllViewportsToLoad(15);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step5 = false;
                step5 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_viewportNo(1)));
                if (step5)
                {
                    result.steps[ExecutedSteps].status = "Pass";  //Step-5
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                studyViewer.CloseBluRingViewer();
                PageLoadWait.WaitForPageLoad(20);

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
                if (IsRemoteDeviceRemoved)
                {
                    login.DriverGoTo("https://" + strEAIPAddress + "/webadmin");
                    HPLogin hplogin = new HPLogin();
                    homepage = hplogin.LoginHPen(hpUserName, hpPassword);
                    PageLoadWait.WaitForHPPageLoad(20);
                    configure = (Configure)homepage.Navigate("Configure");
                    configure.NavigateToTab("remotedevices");
                    configure.AddRemoteDevice(ICAServerName, RemoteDeviceConfigPath);
                    hplogin.LogoutHPen();
                    login.DriverGoTo(login.url);
                }
                new Taskbar().Show();
            }
        }

        /// <summary>
        /// Error Handling - Viewer error when database is down - OPTIONAL
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_160987(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);

            BasePage basepage = new BasePage();
            BluRingViewer studyViewer;
            String SQLIP = "";
            bool IsDatabaseDisconnected = false;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data
                String adminusername = Config.adminUserName;
                String adminpassword = Config.adminPassword;
                String adminDomain = Config.adminGroupName;
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String Modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String strDataSource = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Datasource");
                SQLIP = Config.IConnectIP; //(String)ReadExcel.GetTestData(filepath, "TestData", testid, "SQLIP");

                //Step-1: Precondition - EA datasource configured.
                ExecutedSteps++;    //Step-1

                //Step-2: Login to iCA webconsole
                login.LoginIConnect(adminusername, adminpassword);
                UserPreferences userpref = new UserPreferences();
                userpref.OpenUserPreferences();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
                PageLoadWait.WaitForPageLoad(20);
                userpref.ModalityDropDown().SelectByText(Modality);
                userpref.LayoutDropDown().SelectByText("2x2");
                PageLoadWait.WaitForPageLoad(20);
                userpref.CloseUserPreferences();
                PageLoadWait.WaitForPageLoad(20);
                ExecutedSteps++;    //Step-2

                //Step-3: Search the Study and it should displayed in StudyList
                studies = login.Navigate<Studies>();
                studies.SelectAllDateAndData();
                studies.SearchStudy(AccessionNo: Accession, patientID: PatientID, Modality: Modality, Datasource: strDataSource);
                Dictionary<string, string> StudyDisplayed = studies.GetMatchingRow(new string[] { "Accession" }, new string[] { Accession });
                if (StudyDisplayed != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";  //Step-3
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4: Disconnect database service.
                var dbutil = new DataBaseUtil("sqlserver", DBName: "IRWSDB", DataSourceIP: SQLIP);
                dbutil.SetOffline();
                IsDatabaseDisconnected = true;
                ExecutedSteps++;
                Logger.Instance.InfoLog("Database Set to Offline successfully");

                //Step-5: Select a study and open in Viewer
                studies.SelectStudy("Accession", Accession);
                //studyViewer = studies.LaunchStudy();
                studyViewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(15);
                //PageLoadWait.WaitForThumbnailsToLoad(15);
                //PageLoadWait.WaitForAllViewportsToLoad(15);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step5 = false;
                step5 = studies.CompareImage(result.steps[ExecutedSteps], studyViewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_viewportNo(1)));
                if (step5)
                {
                    result.steps[ExecutedSteps].status = "Pass";  //Step-5
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                studyViewer.CloseStudy();
                PageLoadWait.WaitForPageLoad(20);

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
                if (IsDatabaseDisconnected)
                {
                    try
                    {
                        String strkillCommand = @"net stop MSSQL$WEBACCESS";
                        ExecuteRemoteCommand(SQLIP, Config.WindowsUserName, Config.WindowsPassword, strkillCommand);
                        Thread.Sleep(2000);
                        String strStartCommand = @"net start MSSQL$WEBACCESS";
                        ExecuteRemoteCommand(SQLIP, Config.WindowsUserName, Config.WindowsPassword, strStartCommand);

                        var dbutil = new DataBaseUtil("sqlserver", DBName: "master", DataSourceIP: SQLIP);
                        dbutil.SetOnline();
                        Thread.Sleep(10000);
                        Logger.Instance.InfoLog("Database Set to Online successfully");
                    }
                    catch (Exception err)
                    {
                        Logger.Instance.ErrorLog("Unable to set database as ONLINE. Error:" + err.InnerException);
                    }
                }
            }
        }
    }
}