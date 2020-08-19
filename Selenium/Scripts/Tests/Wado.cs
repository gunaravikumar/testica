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
using System.Data;
using Selenium.Scripts.Pages.MergeServiceTool;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TableItems;

namespace Selenium.Scripts.Tests
{
    class Wado 
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public HPHomePage hphomepage { get; set; }
        public HPLogin hplogin { get; set; }
        public ServiceTool servicetool { get; set; }
        public WADOClient wadoclient;
        public BasePage BasePage;
        public WpfObjects wpfobject;

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>

        public Wado(String classname)
        {
            BasePage = new BasePage();
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ei = new ExamImporter();
            mpaclogin = new MpacLogin();
            wpfobject = new WpfObjects();
            wadoclient = new WADOClient();
            
        }

        public TestCaseResult Test_66132(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            ServiceTool servicetool = new ServiceTool();
            DomainManagement domainmanagement = null;
            TestCaseResult result;
            Login login = new Login();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            string WadoExePath = @"C:\WebAccess\WebAccess\bin\WadoWSTestClient.exe";
            string WadoExeConfig = @"C:\WebAccess\WebAccess\bin\WadoWSTestClient.exe.config";
            string OutputPath = @"C:\Windows\Temp\WadoWS";
            string TestDataDirectory = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "WadoXMLPath");
            try
            {

                //Step 1: Install latest build and using the service tool license the application. Add a data datasource that has the following studies Abdomen, MR Horton, Mackie IISRESET                      
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                ExecutedSteps++;
                String currentDirectory = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));

                // Step 2: In the service tool select the WadoWS tab and select all the datasource from the list. Select the Enable KO and Enable PR boxes. Apply and IISRESET
                servicetool.WadoWSSetup();
                ExecutedSteps++;

                // Step 3: Login to ICA. - in the Domain management page move the datasources to the connected side. Save
                login.LoginIConnect(Username,Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.ConnectAllDataSources();
                domainmanagement.ClickSaveEditDomain();
                login.Logout();
                ExecutedSteps++;

                // Step 4: From \\Divot\webaccess\wadows copy all of the files into a directory in the client machine used for testing. Example = C:\wadows 
                ExecutedSteps++;
                // Step 5: Edit the WadoWSTestClient.exe.config.xml. change the IP to the ICA server under test. Example: Save file
                BasePage.ChangeAttributeValue(WadoExeConfig, "/client/endpoint", "address", "http://localhost/WebAccess/WadoWS/WadoWS.svc");
                ExecutedSteps++;

                //Step 6: Open the WadoWSTestClient.exe program and Add the following : The default service path is loaded from the config file, if not then enter: Service Path ="http://"servername"/WebAccess/WadoWS/WadoWS.svc" Select the Requester type = Dicom Input File Path = C:\wadows\DicomRequest_AbdomenMR.xml Output Path = C:\WADO\Temp
                wpfobject.InvokeApplication(WadoExePath, 0);
                wpfobject.GetMainWindow("WADOClient");
                wpfobject.FocusWindow();
                wpfobject.SelectTabFromTabItems("Render/Dicom Request");
                wpfobject.ClickRadioButton(wadoclient.DicomRadioButton);

                wpfobject.ClickButton("button1", 0);
                wpfobject.SetText("File name:", string.Concat(TestDataDirectory, "\\66132 Step 06.xml"), 1);
                wpfobject.ClickButton("Open", 1);

                wpfobject.ClearText(wadoclient.OutputPathFile);
                wpfobject.SetText(wadoclient.OutputPathFile,OutputPath);
                Directory.CreateDirectory(OutputPath);
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
        }

        public TestCaseResult Test_66133(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            ServiceTool servicetool = new ServiceTool();
            TestCaseResult result;
            Login login = new Login();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            string WadoExePath = @"C:\WebAccess\WebAccess\bin\WadoWSTestClient.exe";
            string OutputPath = @"C:\Windows\Temp\WadoWS";
            string TestDataDirectory = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "WadoXMLPath");
            try
            {
                // Step 1: Click on the"Send Request"button.
                wpfobject.ClickButton(wadoclient.SendRequestButton);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow("WadoWS Response");
                wpfobject.FocusWindow();
                string status = WpfObjects._mainWindow.Get(SearchCriteria.ByAutomationId("lbStatusMsg")).Name;
                wadoclient.CloseWadoTestClient();
                if (status.EndsWith("Type:Success", StringComparison.OrdinalIgnoreCase))
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
                // Step 2: On the WadoWS Response window, Click on the File Path link.
                result.steps[++ExecutedSteps].status = "Not Automated";
                // Step 3: In the WadoWS Client click on NewRequest and load "DicomRequest_Horton.xml" Request Type = Dicom
                wadoclient.CloseWadoTestClient();
                wpfobject.InvokeApplication(WadoExePath, 0);
                wpfobject.GetMainWindow("WADOClient");
                wpfobject.FocusWindow();
                wpfobject.SelectTabFromTabItems("Render/Dicom Request");
                wpfobject.ClickRadioButton(wadoclient.DicomRadioButton);

                wpfobject.ClickButton("button1", 0);
                wpfobject.SetText("File name:", string.Concat(TestDataDirectory, "\\66133 Step 03.xml"), 1);
                wpfobject.ClickButton("Open", 1);

                wpfobject.ClearText(wadoclient.OutputPathFile);
                wpfobject.SetText(wadoclient.OutputPathFile, OutputPath);
                ExecutedSteps++;

                // Step 4
                // 	Click on the"Send Request"button.
                wpfobject.ClickButton(wadoclient.SendRequestButton);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow("WadoWS Response");
                wpfobject.FocusWindow();
                status = WpfObjects._mainWindow.Get(SearchCriteria.ByAutomationId("lbStatusMsg")).Name;
                wadoclient.CloseWadoTestClient();
                if (status.EndsWith("Type:Success", StringComparison.OrdinalIgnoreCase))
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
                // Step 5
                // Go to the c:\Wado\Temp and confirm the study was retrieved. Click on the File Path link.	
                result.steps[++ExecutedSteps].status = "Not Automated";

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

        public TestCaseResult Test_66134(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables 
            ServiceTool servicetool = new ServiceTool();
            TestCaseResult result;
            Login login = new Login();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            string WadoExePath = @"C:\WebAccess\WebAccess\bin\WadoWSTestClient.exe";
            string OutputPath = @"C:\Windows\Temp\WadoWS";
            string TestDataDirectory = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "WadoXMLPath");
            string[] altercolumn = null;
            try
            {
                // Step 1: In the WadoWS Client - Request Type = Render - Click on New Request and load "RenderRequest_Horton.xml" This file will retrieve one image and render it in both jpeg and gif formats with different size and W/L image/gif  500 500 1500 1600 1 80< patient 0.0, 0.0, 1.0, 1.0 - image/jpeg 300 300 1800 2000 1 80 patient 0.0, 0.0, 1.0, 1.0
                // Step 2: // Click on the"Send Request"button.
                wadoclient.CloseWadoTestClient();
                if(wadoclient.RenderRequest(WadoExePath, string.Concat(TestDataDirectory, "\\66134 Step 01.xml"), OutputPath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                IList<string> ResponsePath = wadoclient.GetFilePathFromWadoWSResponse();
                if(ResponsePath.Count == 2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                // Step 3:  Click on the File path link for each of the jpeg and gif files
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 4: In the WadoWS Client click on New Request and load "RenderRequest_Horton_KO.xml" Request Type = Render
                //Step 5: Click on the"Send Request"button.
                wadoclient.CloseWadoTestClient();
                ExecutedSteps++;
                if (wadoclient.RenderRequest(WadoExePath, string.Concat(TestDataDirectory, "\\66134 Step 04.xml"), OutputPath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6: Click on the File path link
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 7:  	In the WadoWS Client click on New Request and load "RenderRequest_GifPR+annotate.xml" Request Type = Render
                //Step 8: Click on the"Send Request"button.
                wadoclient.CloseWadoTestClient();
                ExecutedSteps++;
                if (wadoclient.RenderRequest(WadoExePath, string.Concat(TestDataDirectory, "\\66134 Step 07.xml"), OutputPath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9: Click on the File path link 
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 10: In the WadoWS Client click on New Request and load "RenderRequest_jpegKO.xml" - Request Type = Render
                //Step 11: Click on the"Send Request"button.
                wadoclient.CloseWadoTestClient();
                ExecutedSteps++;
                if (wadoclient.RenderRequest(WadoExePath, string.Concat(TestDataDirectory, "\\66134 Step 10.xml"), OutputPath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 12: Click on the File path link
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 13: In the WadoWS Client click on New Request and load "RenderRequest_pngKO.xml" - Request Type = Render
                //Step 14: Click on the"Send Request"button.
                wadoclient.CloseWadoTestClient();
                ExecutedSteps++;
                if (wadoclient.RenderRequest(WadoExePath, string.Concat(TestDataDirectory, "\\66134 Step 13.xml"), OutputPath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 15: Click on the File path link
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 16: In the WadoWS Client click on New Request and load "RenderRequest_pngKO.xml" Request Type = Render click on the small plus sign on the left hand side,then select DocumentRequest, the window displayes the details of the file loaded. Change the Rows = 700 and the Columns = 800 Window\Level = 1200 Window\Width = 1000 Frame number = 1 Click on the Send request button
                wadoclient.CloseWadoTestClient();
                altercolumn = new string[] { "Rows=700","Columns=800","WindowLevel=1200","WindowWidth=1000","FrameNumber=1"};
                if (wadoclient.RenderRequest(WadoExePath, string.Concat(TestDataDirectory, "\\66134 Step 13.xml"), OutputPath,true,altercolumn))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 17: Click on the File path link
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 18: Change the size to 50% Change the Rows = 350 and the Columns = 400 and click on the Send Request button
                wadoclient.CloseWadoTestClient();
                altercolumn = new string[] { "ImageQuality=50", "Rows=350", "Columns=400" };
                if (wadoclient.RenderRequest(WadoExePath, string.Concat(TestDataDirectory, "\\66134 Step 13.xml"), OutputPath, true, altercolumn))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 19: Click on the File path link
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step 20: Change the the Region to 0.0, 0.0, 0.5, 0.5 and click on the Send Request button 
                wadoclient.CloseWadoTestClient();
                altercolumn = new string[] { "Region_Xmin_Ymin_Xmax_Ymax=0.0, 0.0, 0.5, 0.5" };
                if (wadoclient.RenderRequest(WadoExePath, string.Concat(TestDataDirectory, "\\66134 Step 13.xml"), OutputPath, true, altercolumn))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 21: Click on the File path link
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 22: Change the the Region to 0.0, 0.0, 0.75, 0.75 and click on the Send Request button
                wadoclient.CloseWadoTestClient();
                altercolumn = new string[] { "Region_Xmin_Ymin_Xmax_Ymax=0.0, 0.0, 0.75, 0.75" };
                if (wadoclient.RenderRequest(WadoExePath, string.Concat(TestDataDirectory, "\\66134 Step 13.xml"), OutputPath, true, altercolumn))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 23: Click on the File path link
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 24: In the WadoWS Client click on New Request and load "RenderRequest_Appl_pdf-BarkleyW.xml" Request Type = Render
                //Step 25: Click on the"Send Request"button.
                wadoclient.CloseWadoTestClient();
                ExecutedSteps++;
                if (wadoclient.RenderRequest(WadoExePath, string.Concat(TestDataDirectory, "\\66134 Step 24.xml"), OutputPath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 26: Click on the File path link
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 27: In the WadoWS Client click on New Request and load "RenderRequest_SR-text_xml.xml" Request Type = Render
                //Step 28: Click on the"Send Request"button.
                wadoclient.CloseWadoTestClient();
                ExecutedSteps++;
                if (wadoclient.RenderRequest(WadoExePath, string.Concat(TestDataDirectory, "\\66134 Step 27.xml"), OutputPath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 29: Click on the File path link
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 30: In the WadoWS Client click on New Request and load "RenderRequest_SR-text_html.xml" Request Type = Render
                //Step 31: Click on the"Send Request"button.
                wadoclient.CloseWadoTestClient();
                ExecutedSteps++;
                if (wadoclient.RenderRequest(WadoExePath, string.Concat(TestDataDirectory, "\\66134 Step 30.xml"), OutputPath))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 32: Click on the File path link
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step 33: In the Widows Client edit the Content Type List by adding application/PDF1. Click on the Send Request button
                wadoclient.CloseWadoTestClient();
                altercolumn = new string[] { "ContentTypeList=application/pdf1" };
                if (!wadoclient.RenderRequest(WadoExePath, string.Concat(TestDataDirectory, "\\66134 Step 30.xml"), OutputPath, true, altercolumn))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 34: In the Widows Client edit the Document UID by adding an extra character at the end of the string. Click on the Send Request button
                wadoclient.CloseWadoTestClient();
                altercolumn = new string[] { "DocumentUID=123454" };
                if (!wadoclient.RenderRequest(WadoExePath, string.Concat(TestDataDirectory, "\\66134 Step 30.xml"), OutputPath, true, altercolumn))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                wadoclient.CloseWadoTestClient();
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


        public TestCaseResult Test_66135(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            Maintenance maintenance = null;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            try
            {
                //Step 1: Login to ICA. Select Maintenance tab.
                String Username = Config.adminUserName;
                String Password = Config.adminPassword;
                login.LoginIConnect(Username, Password);
                ExecutedSteps++;
                //Step 2: Select Audit tab and look for Data Export and Dicom Instances Transferred.
                maintenance = (Maintenance)login.Navigate("Maintenance");
                maintenance.Navigate("Audit");
                DataTable AuditLog = BasePage.CollectRecordsInAllPages(maintenance.Tbl_EvemtsTable(), maintenance.TableHeader(), maintenance.TableRow(), maintenance.TableColumn());
                IList<string> AuditEvent = BasePage.GetColumnValues(AuditLog, "Audit Event");
                if(AuditEvent.Contains("DICOM Instances Accessed") && AuditEvent.Contains("Export"))
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
