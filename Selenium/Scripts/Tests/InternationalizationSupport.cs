using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Pages.MergeServiceTool;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;
using TextBox = TestStack.White.UIItems.TextBox;

namespace Selenium.Scripts.Tests
{
    class InternationalizationSupport : BasePage
    {
        //public Login login { get; set; }
        //public string filepath { get; set; }
        //public DomainManagement domain { get; set; }
        //public BluRingZ3DViewerPage Z3dViewerPage { get; set; }

        public Login login { get; set; }
        public string filepath { get; set; }
        public DomainManagement domain { get; set; }
        public StudyViewer studyviewer { get; set; }
        public Cursor Cursor { get; private set; }
        public ServiceTool servicetool { get; set; }

        public InternationalizationSupport(String classname)
        {
            login = new Login();
            //  BasePage.InitializeControlIdMap();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        public TestCaseResult Test_163330(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            //Set up Validation Steps
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 - Navigate to 3D tab and Click MPR mode from the dropdown. Note: This is new design(could change)
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription);
                if (step1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:2 - From MPR 4:1 viewing mode , Verify the date format displayed in the DICOM image annotations
                Boolean step2_1 = Z3dViewerPage.VerifyDateFormat(BluRingZ3DViewerPage.Navigationone);
                Boolean step2_2 = Z3dViewerPage.VerifyDateFormat(BluRingZ3DViewerPage.Navigationtwo);
                Boolean step2_3 = Z3dViewerPage.VerifyDateFormat(BluRingZ3DViewerPage.Navigationthree);
                Boolean step2_4 = Z3dViewerPage.VerifyDateFormat(BluRingZ3DViewerPage.ResultPanel);
                if(step2_1 && step2_2 && step2_3 && step2_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:3 - From 3D 4-up viewing mode , Verify the date format displayed in the DICOM image annotations
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                Boolean step3_1 = Z3dViewerPage.VerifyDateFormat(BluRingZ3DViewerPage.Navigationone);
                Boolean step3_2 = Z3dViewerPage.VerifyDateFormat(BluRingZ3DViewerPage.Navigationtwo);
                Boolean step3_3 = Z3dViewerPage.VerifyDateFormat(BluRingZ3DViewerPage.Navigationthree);
                Boolean step3_4 = Z3dViewerPage.VerifyDateFormat(BluRingZ3DViewerPage.Navigation3D1);
                if (step3_1 && step3_2 && step3_3 && step3_4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:4 - From 3D SixUp viewing mode , Verify the date format displayed in the DICOM image annotations 
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                Boolean step4_1 = Z3dViewerPage.VerifyDateFormat(BluRingZ3DViewerPage.Navigationone);
                Boolean step4_2 = Z3dViewerPage.VerifyDateFormat(BluRingZ3DViewerPage.Navigationtwo);
                Boolean step4_3 = Z3dViewerPage.VerifyDateFormat(BluRingZ3DViewerPage.Navigationthree);
                Boolean step4_4 = Z3dViewerPage.VerifyDateFormat(BluRingZ3DViewerPage.ResultPanel);
                Boolean step4_5 = Z3dViewerPage.VerifyDateFormat(BluRingZ3DViewerPage.Navigation3D1);
                Boolean step4_6 = Z3dViewerPage.VerifyDateFormat(BluRingZ3DViewerPage.Navigation3D2);
                if (step4_1 && step4_2 && step4_3 && step4_4 && step4_5 && step4_6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:5 - From Curved MPR viewing mode , Verify the date format displayed in the DICOM image 
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                Boolean step5_1 = Z3dViewerPage.VerifyDateFormat(BluRingZ3DViewerPage.Navigationone);
                Boolean step5_2 = Z3dViewerPage.VerifyDateFormat(BluRingZ3DViewerPage.Navigationtwo);
                Boolean step5_3 = Z3dViewerPage.VerifyDateFormat(BluRingZ3DViewerPage.Navigationthree);
                Boolean step5_4 = Z3dViewerPage.VerifyDateFormat(BluRingZ3DViewerPage.CurvedMPR);
                Boolean step5_5 = Z3dViewerPage.VerifyDateFormat(BluRingZ3DViewerPage.MPRPathNavigation);
                Boolean step5_6 = Z3dViewerPage.VerifyDateFormat(BluRingZ3DViewerPage._3DPathNavigation);
                if (step5_1 && step5_2 && step5_3 && step5_4 && step5_5 && step5_6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:6 - From Calcium Scoring view mode , Verify the date format displayed in the DICOM image 
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                Boolean step6 = Z3dViewerPage.VerifyDateFormat(BluRingZ3DViewerPage.CalciumScoring);
                if(step6)
                {
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
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                Z3dViewerPage.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163331(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            WpfObjects wpfobject = new WpfObjects();
            BasePage basepage = new BasePage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            String sAccestionVlues = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string testdatades = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] ssplit = testdatades.Split('|');
            String sAccessionHeading = ssplit[0];
            string sPatientValue = ssplit[1];
            string sPatientthumbnail = ssplit[2];
            String sFilepath = ssplit[3];
            String sFileName = ssplit[4];
            string sServiceName = ssplit[5];
            String username = Config.adminUserName;
            String password = Config.adminPassword;

            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                login.LoginIConnect(username, password);
             //   z3dvp.LoginToShare("ICA-TST22-WS12.products.network.internal", @"C$", "Administrator", "Pa$$word");
                //Step 1   From iCA, Load a study in Z3D viewer. By default MPR viewing mode should dipslay
                //step 2   Select a 3D supported series and Select the MPR view option from the smart view drop down.
                z3dvp.Deletefiles(testcasefolder);
                
                bool res = z3dvp.searchandopenstudyin3D(sAccestionVlues, thumbnailcaption, field: sAccessionHeading);
                if (res == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 3 Verify that MPI value is displayed on the 3D viewports from all the view modes as a dicom Overlay
                List<string> IMPI3 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightTop, null, null, 4);
                if (IMPI3[0].Contains("MPI") && IMPI3[1].Contains("MPI") && IMPI3[2].Contains("MPI") && IMPI3[3].Contains("MPI"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 4 Close the study and from Studies page, Search and load a 3D supported study in universal viewer without tag value Other Patient ID .
                IWebElement IExit4 = z3dvp.ExitIcon();
                if(IExit4.Displayed)
                {
                    z3dvp.CloseViewer();
                   // IExit4.Click();
                    PageLoadWait.WaitForPageLoad(5);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 5 Select a 3D supported series and Select the MPR view option from the smart view drop down.
                //Step 6  Verify that MPI value is not displayed on the 3D viewports from all the view modes as a dicom Overlay. 
                bool res5 = z3dvp.searchandopenstudyin3D(sPatientValue, sPatientthumbnail);
                if (res5 == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //step 7In server side, Open the specific file path c:\drs\sys\data\annot.ini in Z3D server and comment the line “Other patient id” under annotation tokens from annot config file.
                 string[] filePaths = Directory.GetFiles(sFilepath, "*.INI",SearchOption.AllDirectories);
                
                string strfilepath=null;
                for(int i=0; i< filePaths.Length; i++)
                {
                    if(filePaths[i].ToString()== sFilepath+ sFileName)
                    {
                        strfilepath = filePaths[i].ToString();
                        break;
                    }
                }
                string str = @"OtherPatientID = ""MPI: ""+DICOM(0010,1000,LO)";
                string sUpdatestr = @";OtherPatientID = ""MPI: ""+DICOM(0010,1000,LO)";
                if (string.IsNullOrEmpty(strfilepath) ==false)
                {
                    string text = File.ReadAllText(strfilepath);
                    if (string.IsNullOrEmpty(text)==false)
                    {
                        text = text.Replace(str, sUpdatestr);
                        File.WriteAllText(strfilepath, text);
                    }
                }
                //Expected REsults
                bool bflag7 = false;
                string strReadTExt = File.ReadAllText(strfilepath);
                if(string.IsNullOrEmpty(strReadTExt)==false)
                {
                    if(strReadTExt.Contains(sUpdatestr)==true)
                        {
                        bflag7 = true;
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if(bflag7==false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 8 restart the services (IBm App gate)
            bool bStopServices=    wpfobject.StopService(sServiceName);
                bool bStartSErices=wpfobject.StartService(sServiceName);

              if(bStopServices && bStartSErices)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 9 Search and load a 3D supported study in universal viewer with tag value Other Patient ID (Dicom tag: 0010, 1000) 
                //step 10 Select a 3D supported series and Select the MPR view option from the smart view drop down.
                IWebElement IExit9 = z3dvp.ExitIcon();
                if (IExit9.Displayed)
                {
                    z3dvp.CloseViewer();
                    PageLoadWait.WaitForPageLoad(5);
                }
                    bool res9 = z3dvp.searchandopenstudyin3D(sAccestionVlues, thumbnailcaption, field: sAccessionHeading);
                if (res9 == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 11 Verify that MPI value is not displayed on the 3D viewports from all the view modes as a dicom Overlay.
                List<string> IMPI11 = z3dvp.GetAttributes_Result(Locators.CssSelector.AnnotationRightTop, null, null, 4);
                if (IMPI11[0].Contains("MPI")==false && IMPI11[1].Contains("MPI") == false  && IMPI11[2].Contains("MPI") == false  && IMPI11[3].Contains("MPI")==false)
                {
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
               
                return result;
            }
            finally
            {
                try
                {
                    //Revert the services   start here
                    string[] filePaths = Directory.GetFiles(sFilepath, "*.INI", SearchOption.AllDirectories);
                    string strfilepath = null;
                    for (int i = 0; i < filePaths.Length; i++)
                    {
                        if (filePaths[i].ToString() == sFilepath + sFileName)
                        {
                            strfilepath = filePaths[i].ToString();
                            break;
                        }
                    }
                    string sUpdatestr = @"OtherPatientID = ""MPI: ""+DICOM(0010,1000,LO)";
                    string str = @";OtherPatientID = ""MPI: ""+DICOM(0010,1000,LO)";
                    if (string.IsNullOrEmpty(strfilepath) == false)
                    {
                        string text = File.ReadAllText(strfilepath);
                        if (string.IsNullOrEmpty(text) == false)
                        {
                            text = text.Replace(str, sUpdatestr);
                            File.WriteAllText(strfilepath, text);
                        }
                    }

                    bool bStopServices = wpfobject.StopService(sServiceName);
                    bool bStartSErices = wpfobject.StartService(sServiceName);
                }
                catch(Exception e )
                {
                    Logger.Instance.ErrorLog("Service start & Stop Issues  " +e.Message);
                }
                //Revert the services   end here
                z3dvp.CloseViewer();
                login.Logout();
                
            }
        }
        public TestCaseResult Test_163332(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] TestData = TestDataRequirements.Split('|');
            string PatientID2 = TestData[0];
            string ThumbnailDescription2 = TestData[1];
            //Set up Validation Steps
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 -  	Load a Study containing double byte in Universal viewer
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;

                //step:2 - Verify that the Z3D viewer does not supports the study containing double byte.
                Boolean step2 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription);
                if(!step2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[ExecutedSteps].comments = "Related to JIRA ICA-18041";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //step:3 - Load a Study containing Single byte in Universal viewer.
                //step:4 - Study should be launched with out any errors.
                Z3dViewerPage.ExitIcon().Click();
                PageLoadWait.WaitForFrameLoad(5);
                Boolean step4 = Z3dViewerPage.searchandopenstudyin3D(PatientID2, ThumbnailDescription2);
                if (step4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                Z3dViewerPage.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163329(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] TestData = TestDataRequirements.Split('|');
            String culture = TestData[0];
            String LCIDCode = TestData[1];
            String Language = TestData[2];
            String prefix = culture.Split('-')[0];
            String suffix = culture.Split('-')[1];
            //Set up Validation Steps
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            BluRingViewer bluringviewer = new BluRingViewer();
            WpfObjects wpf = new WpfObjects();
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Precondition
                string currentdirectory = Directory.GetCurrentDirectory();
                string ZipPath = Config.zipPath;
                string ExtractPath = Config.extractpath;
                string defaultPath = Config.defaultpath;

                string commonpath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "OtherFiles" + Path.DirectorySeparatorChar;
                Logger.Instance.InfoLog("Localization Setup:  ZipPath = " + ZipPath);
                Logger.Instance.InfoLog("Localization Setup:  ExtractPath = " + ExtractPath);
                Logger.Instance.InfoLog("Localization Setup:  defaultPath = " + defaultPath);

                Directory.SetCurrentDirectory(ExtractPath);
                Logger.Instance.InfoLog("Localization Setup:  Current Directory changed to:" + Directory.GetCurrentDirectory());

                ServiceTool servicetool = new ServiceTool();

                string LocalizationPrepareFile = ExtractPath + Path.DirectorySeparatorChar + "Localization_Prepare.wsf";
                string LocalizationCompleteFile = ExtractPath + Path.DirectorySeparatorChar + "Localization_Complete.wsf";
                string PrepareOutputPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Localization_Prepare.log";
                string CompleteOutputPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Localization_Complete.log";

                string TranslationEXEpath = commonpath + "TranslationTool.exe";
                Logger.Instance.InfoLog("Localization Setup:  TranslationEXEpath = " + TranslationEXEpath);

                String xmlFilePath = @"C:\WebAccess\WebAccess\web.Config";
                String NodePath = "configuration/appSettings/add";
                String FirstAttribute = "key";
                String AttValue = "Application.Culture";
                String SecondAttribute = "value";

                bool localizationPrepare = true;
                bool localizationComplete = true;
                bool webConfigUpdated = true;

                bool UnzipFolder = UnZipSDKFolder(ZipPath, ExtractPath, defaultPath);

                String EIWix = ExtractPath + Path.DirectorySeparatorChar + culture + @"\UploaderTool_Resources\WixLocalization\Language_" + culture + ".wxl";
                String POPWix = ExtractPath + Path.DirectorySeparatorChar + culture + @"\PopConfigurationTool_Resources\WixLocalization\Language_" + culture + ".wxl";
                String EIBoot = ExtractPath + Path.DirectorySeparatorChar + culture + @"\UploaderTool_Resources\BootStrapperLocalization\Theme_" + culture + ".wxl";
                String POPBoot = ExtractPath + Path.DirectorySeparatorChar + culture + @"\PopConfigurationTool_Resources\BootStrapperLocalization\Theme_" + culture + ".wxl";
                String GlobalResourcePath = @"This PC\Local Disk (C:)\WebAccess\LocalizationSDK\" + culture;

                // Run Localiation Prepare
                if (!servicetool.Prepare_CompleteLocalization(culture, LocalizationPrepareFile, PrepareOutputPath))
                {
                    Logger.Instance.InfoLog("Localization Prepare failed for " + culture);
                    localizationPrepare = false;
                }

                // Run Translation tool
                servicetool.Translation(TranslationEXEpath, GlobalResourcePath, prefix, suffix);

                // Update Wix files
                ChangeAttributeValue(EIBoot, "/WixLocalization", "Culture", culture, encoding: true); //Theme.wxl
                ChangeAttributeValue(EIBoot, "/WixLocalization", "Language", LCIDCode, encoding: true);
                ChangeAttributeValue(POPBoot, "/WixLocalization", "Culture", culture, encoding: true);
                ChangeAttributeValue(POPBoot, "/WixLocalization", "Language", LCIDCode, encoding: true);
                ChangeAttributeValue(EIWix, "/WixLocalization", "Culture", culture, encoding: true); //Language.wxl                
                ChangeAttributeValue(POPWix, "/WixLocalization", "Culture", culture, encoding: true);

                // Run Localization Completion tool
                if (!servicetool.Prepare_CompleteLocalization(culture, LocalizationCompleteFile, CompleteOutputPath))
                {
                    Logger.Instance.InfoLog("Localization Complete failed for " + culture);
                    localizationComplete = false;
                }

                // Update web.config
                String ExistingValue = GetAttributeValue(xmlFilePath, NodePath, FirstAttribute, AttValue, SecondAttribute);
                if (!ExistingValue.Contains(culture))
                {
                    SetWebConfigValue(xmlFilePath, AttValue, ExistingValue + "," + culture);
                }
                String NewValue = GetAttributeValue(xmlFilePath, NodePath, FirstAttribute, AttValue, SecondAttribute);

                if (!NewValue.Contains(culture))
                {
                    Logger.Instance.InfoLog("Web.config does not contain " + culture);
                    webConfigUpdated = false;
                }

                Directory.SetCurrentDirectory(currentdirectory);
                Logger.Instance.InfoLog("Switched to default directory");

                //step:1 - In the iCA server, Install the 3D build and verify the installation windows
                Thread.Sleep(15000);
                String Z3DBuildPath = Config.Z3DBuildPath;
                BasePage.LatestZ3DBuild_Path = login.LatestDirectory(Z3DBuildPath);
                BasePage.LatestZ3DBuild_Path = login.LatestDirectory(BasePage.LatestZ3DBuild_Path);
                BasePage.LatestZ3DBuild_Path = login.LatestDirectory(BasePage.LatestZ3DBuild_Path);
                BasePage.LatestZ3DBuild_Path = login.LatestDirectory(BasePage.LatestZ3DBuild_Path);
                String Z3dBuilds = BasePage.LatestZ3DBuild_Path + "\\Installer";
                String Z3DInstaller = "Z3D_ICAinstaller.msi";
                Process proc = Process.Start(Z3dBuilds + "\\" + Z3DInstaller);
                Thread.Sleep(750);
                var MainWindow = wpf.GetMainWindowByTitle("IBM Z3D - iConnect Access Version");
                String msgbox = MainWindow.Title.ToString();
                Boolean step1 = false;
                msgbox = Regex.Replace(msgbox, @"[- :0-9()]", String.Empty);
                foreach (char c in msgbox)
                {
                    int unicode = c;
                    if (unicode > 64 && unicode < 123)
                        step1 = true;
                    else
                    {
                        step1 = false;
                        break;
                    }
                }
                if(step1 && localizationPrepare && localizationComplete && webConfigUpdated)
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

                Z3dViewerPage.CloseZ3DErrorPopUp();

                //step:2 - In the client machine, launch the webaccess URL in the supported browser. make sure the iCA language is set to English
                Thread.Sleep(15000);
                IList<IWebElement> Contents = new List<IWebElement>();
                IWebElement content1 = Driver.FindElement(By.CssSelector(Locators.CssSelector.loginbtn));
                IWebElement content2 = Driver.FindElement(By.CssSelector(Locators.CssSelector.passwordbtn));
                IWebElement content3 = Driver.FindElement(By.CssSelector(Locators.CssSelector.unamelable));
                IWebElement content4 = Driver.FindElement(By.CssSelector(Locators.CssSelector.pwdlable));
                Contents.Add(content1);
                Contents.Add(content2);
                Contents.Add(content3);
                Contents.Add(content4);
                Boolean step2_1 = Z3dViewerPage.CheckLanguageinEnglish(Contents[0]);
                Boolean step2_2 = Z3dViewerPage.CheckLanguageinEnglish(Contents[1]);
                Boolean step2_3 = Z3dViewerPage.CheckLanguageinEnglish(Contents[2]);
                Boolean step2_4 = Z3dViewerPage.CheckLanguageinEnglish(Contents[3]);
                if (step2_1 && step2_2 && step2_3 && step2_4)
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

                //step:3 and 4- Log in to iCA and navigate to studies tab. Search and load a study that has the 3D supported series in the universal viewer
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step3 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription);
                if(step3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:5 - Select the 3D tool box. Place the mouse cursor over the 3D tools
                IWebElement Viewport = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Actions act = new Actions(Driver);
                act.ContextClick(Viewport).Build().Perform();
                PageLoadWait.WaitForPageLoad(5);
                String Element = Locators.CssSelector.GridTilecontains + " " + Locators.CssSelector.ToolWrapper;
                IList<IWebElement> ToolElts = Driver.FindElements(By.CssSelector(Element));
                Boolean step5 = false;
                foreach (IWebElement tool in ToolElts)
                {
                    step5 = Z3dViewerPage.CheckLanguageinEnglish(tool);
                }
                if(step5)
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
                IWebElement Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                ClickElement(Navigation2);
                PageLoadWait.WaitForPageLoad(5);

                //step:6 - Click on the 3D settings from the user settings under the Global tool bar.
                Boolean step6 = false;
                bluringviewer.UserSettings("select", "3D Settings");
                wait.Until(ExpectedConditions.TextToBePresentInElement(Z3dViewerPage.overlaypane(), "Settings"));
                IList<IWebElement> SettingsValues = Driver.FindElements(By.CssSelector(Locators.CssSelector.SettingsValues));
                foreach (IWebElement values in SettingsValues)
                {
                    step6 = Z3dViewerPage.CheckLanguageinEnglish(values);
                }
                if (step6)
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
                ClickElement(Z3dViewerPage.CloseSelectedToolBox());
                PageLoadWait.WaitForPageLoad(5);

                //step:7 - Observe the contents in the control windows from all the viewing modes
                Boolean step7 = false;
                IWebElement viewerbutton3d = Driver.FindElement(By.CssSelector(Locators.CssSelector.ViewerButton3D));
                ClickElement(viewerbutton3d);
                //new Actions(Driver).MoveToElement(Driver.FindElement(By.CssSelector(Locators.CssSelector.ViewerButton3D))).Click().Build().Perform();
                PageLoadWait.WaitForElementToDisplay(Z3dViewerPage.DropDownBox3D());
                IList<IWebElement> layoutlist = Z3dViewerPage.layoutlist();
                foreach (IWebElement list in layoutlist)
                {
                    step7 = Z3dViewerPage.CheckLanguageinEnglish(list);
                }
                if (step7)
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
                ClickElement(Navigation2);

                //step:8 - Log out of iCA and set the language any other than English (E.g. Chinese).
                Z3dViewerPage.CloseViewer();
                login.Logout();
                PageLoadWait.WaitForFrameLoad(10);
                login.PreferredLanguageSelectList().SelectByText(Language);
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement SelectedLanguage = Driver.FindElement(By.CssSelector(Locators.CssSelector.GetCulture));
                if(SelectedLanguage.Text.Contains(Language))
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

                //step:9 - Study is loaded in the universal viewer without any errors
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.Navigate(prefix+"Studies"+suffix);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.SearchStudyfromViewer(prefix+"Patient ID:"+suffix, PatientID);
                PageLoadWait.WaitForFrameLoad(10);
                login.SelectStudy(prefix+"Patient ID"+suffix, PatientID);
                PageLoadWait.WaitForFrameLoad(5);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(10);
                if (viewer.studyPanel().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:10 -  	Informations are displayed in the chinese language
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                IWebElement ViewerContainer = Z3dViewerPage.ViewerContainer();
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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


                //step:11 - Series is loaded in the 3D viewer in MPR 4:1 viewing mode
                Z3dViewerPage.selectthumbnail(ThumbnailDescription);
                PageLoadWait.WaitForFrameLoad(10);
                Boolean step11 = Z3dViewerPage.select3dlayout("MPR", "y");
                if (step11)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:12 - Select the 3D tool box. Place the mouse cursor over the 3D tools
                Viewport = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                act = new Actions(Driver);
                act.ContextClick(Viewport).Build().Perform();
                PageLoadWait.WaitForPageLoad(5);
                Element = Locators.CssSelector.GridTilecontains + " " + Locators.CssSelector.ToolWrapper;
                ToolElts = Driver.FindElements(By.CssSelector(Element));
                Boolean step12 = false;
                foreach (IWebElement tool in ToolElts)
                {
                    step12 = Z3dViewerPage.CheckLanguageinEnglish(tool);
                }
                if (step12)
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
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                ClickElement(Navigation2);
                PageLoadWait.WaitForPageLoad(5);

                //step:13 - Click on the 3D settings from the user settings under the Global tool bar.
                Boolean step13 = false;
                IWebElement Settings3D = Driver.FindElement(By.CssSelector("div[title='"+prefix+"User Settings"+suffix+"']"));
                ClickElement(Settings3D);
                PageLoadWait.WaitForPageLoad(5);
                //bluringviewer.UserSettings("select", "fr3D SettingsFR");
                IList<IWebElement> usersettingList = BasePage.Driver.FindElements(By.CssSelector("div[class*='globalSettingPanel'] ul li"));
                foreach (IWebElement usersetting in usersettingList)
                {
                    if (usersetting.Text.Replace(" ", "").Replace("✔", "").Replace("\r", "").Replace("\n", "").ToLower() == (prefix+"3D Settings"+suffix).Replace(" ", "").ToLower())
                    {
                        ClickElement(usersetting);
                        break;
                    }
                }
                    
                PageLoadWait.WaitForPageLoad(5);
                SettingsValues = Driver.FindElements(By.CssSelector(Locators.CssSelector.SettingsValues));
                foreach (IWebElement values in SettingsValues)
                {
                    step13 = Z3dViewerPage.CheckLanguageinEnglish(values);
                }
                if (step13)
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
                ClickElement(Z3dViewerPage.CloseSelectedToolBox());
                PageLoadWait.WaitForPageLoad(5);

                //step:14 - Observe the contents in the control windows from all the viewing modes
                Boolean step14 = false;
                viewerbutton3d = Driver.FindElement(By.CssSelector(Locators.CssSelector.ViewerButton3D));
                ClickElement(viewerbutton3d);
                //new Actions(Driver).MoveToElement(Driver.FindElement(By.CssSelector(Locators.CssSelector.ViewerButton3D))).Click().Build().Perform();
                PageLoadWait.WaitForElementToDisplay(Z3dViewerPage.DropDownBox3D());
                layoutlist = Z3dViewerPage.layoutlist();
                foreach (IWebElement list in layoutlist)
                {
                    step14 = Z3dViewerPage.CheckLanguageinEnglish(list);
                }
                if (step14)
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
                ClickElement(Navigation2);

                result.FinalResult(ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                Z3dViewerPage.CloseViewer();
                login.Logout();
            }
        }
    }
}
