using System;
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
using Dicom.Network;
using Dicom;

namespace Selenium.Scripts.Tests
{
    class DICOMSOPClassSupport : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public ServiceTool servicetool { get; set; }
        public WpfObjects wpfobject { get; set; }
        public Studies studies { get; set; }
        public BasePage basepage { get; set; }
        public BluRingViewer bluringviewer { get; set; }
        DirectoryInfo CurrentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
        string FolderPath = "";

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public DICOMSOPClassSupport(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            FolderPath = Config.downloadpath;//CurrentDir.Parent.Parent.FullName + "\\Downloads\\";
            mpaclogin = new MpacLogin();
            wpfobject = new WpfObjects();
            basepage = new BasePage();
            servicetool = new ServiceTool();
            bluringviewer = new BluRingViewer();
        }

        /// <summary>
        /// DICOMSOP Class Support - Support viewing and C-MOVE for private sop class (FUJI)
        /// </summary>
        public TestCaseResult Test_161693(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            Taskbar taskbar = null;
            DomainManagement domain;
            string[] FullPath = null;
            string DS1 = string.Empty;
            string DS1AETitle = string.Empty;
            string PatientID = string.Empty;
            int DS1Port = 0;
            String WebaccessconfigurationPath = @"C:\WebAccess\WebAccess\Config\WebAccessConfiguration.xml";
            String DicomMessagingServicesPath = @"C:\WebAccess\WebAccess\Config\DicomMessagingServices.xml";
            String MERGECOMFilePath = @"C:\WebAccess\WebAccess\Config\MergeCom3\MERGECOM.SRV";
            String BackupFolderPath = @"C:\WebAccess\WebAccess\Config\Backup";
            String NodePath1 = "/Configuration/ImageSopClasses";
            String NodePath2 = "/ServiceList";
            String CRNodeXpath = "//abstractSyntax[@name='CR']";
            String RTImageNodeXpath = "//sopClass[@usualModalities='RTIMAGE']";

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String MergeComFileLocationList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "MergeComFileLocation");
                String[] MergeComFileLocation = MergeComFileLocationList.Split('=');
                String URL = "http://" + Config.IConnectIP + "/webaccess";
                String[] FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('='); //D:\142151\S000001 = D:\142151\S000002
                DS1 = Config.EA91;
                DS1AETitle = Config.EA91AETitle;
                DS1Port = 12000;

                //Preconditions
                //Creating Backup folder
                Directory.CreateDirectory(BackupFolderPath);

                //Step-1: Latest build of iCA 7.0 installed on the server
                ExecutedSteps++;

                //Step-2: Send attached data \\datasets\anonymized_data\Bluring\PQA\Image Review\DICOM SOP Class support\TC-144865 to EA
                var client = new DicomClient();
                FullPath = Directory.GetFiles(FilePath[0], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }
                ExecutedSteps++;

                //Step-3: Login to iCA 7.0 as administrator
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Step-4: Navigate to studies tab and Search for Patient ID=6559464
                studies = login.Navigate<Studies>();
                studies.SearchStudy(LastName: "*", patientID: PatientID);
                studies.SelectStudy("Patient ID", PatientID);
                ExecutedSteps++;

                //Step-5: Launch study in Enterprise viewer
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
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
                bluringviewer.CloseBluRingViewer();
                login.Logout();

                //Step-6: Login to iCA 7.0 server and backup below 3 files.
                //1. Webaccessconfiguration.xml (Navigate to webaccess/webaccess/config/webaccessconfiguration.xml)
                //2. DicomMessagingServices(Navigate to webaccess / webaccess / config / DicomMessagingServices)
                //3. MERGECOM.SRV(navigate to WebAccess\Config\MergeCom3\MERGECOM.SRV)
                File.Copy(WebaccessconfigurationPath, BackupFolderPath + Path.DirectorySeparatorChar + Path.GetFileName(WebaccessconfigurationPath), true);
                File.Copy(DicomMessagingServicesPath, BackupFolderPath + Path.DirectorySeparatorChar + Path.GetFileName(DicomMessagingServicesPath), true);
                File.Copy(MERGECOMFilePath, BackupFolderPath + Path.DirectorySeparatorChar + Path.GetFileName(MERGECOMFilePath), true);
                ExecutedSteps++;

                //Step-7: 1. Navigate to webaccess/webaccess/config/webaccessconfiguration.xml and open webaccessconfiguration file 
                //2. Update this file finding the last sopClass entry, "RTIMAGE", in the<ImageSopClasses> section and by adding new SOP class UID as below
                //<sopClass uid="1.2.392.200036.9125.1.1.2" class="FUJI Private CR Storage" usualModalities="CR" />
                //3. save
                basepage.InsertNode(WebaccessconfigurationPath, NodePath1, "<sopClass uid=\"1.2.392.200036.9125.1.1.2\" class=\"FUJI Private CR Storage\" usualModalities=\"CR\" />", false, false, RTImageNodeXpath);
                //sopClass[@usualModalities='RTIMAGE']
                ExecutedSteps++;

                //Step-8: 1. Navigate to webaccess/webaccess/config/DicomMessagingServices.xml
                //2. Add and update the custom abstract Syntax name and the SOP class UID before the "CR" entry as below :
                //<abstractSyntax name = "FUJI_CR" uid="1.2.392.200036.9125.1.1.2" />
                //3. Go further down in the file and add the presentation context transfer syntax as below:
                //<presentationContext abstractSyntax = "FUJI_CR" >
                //< transferSyntax name="Jpeg90" />
                //<transferSyntax name = "Jpeg70" />
                //< transferSyntax name="RLE" />
                //<transferSyntax name = "LittleEndian" />
                //< transferSyntax name="ImplicitLittleEndian" />
                //<transferSyntax name = "Jpeg91" />
                //< transferSyntax name="JpegBaseline" />
                //</presentationContext>
                basepage.InsertNode(DicomMessagingServicesPath, NodePath2, "<abstractSyntax name = \"FUJI_CR\" uid=\"1.2.392.200036.9125.1.1.2\" />", false, true, CRNodeXpath);
                basepage.InsertNode(DicomMessagingServicesPath, NodePath2 + "/service[1]", "<presentationContext abstractSyntax = \"FUJI_CR\" > </presentationContext>", false);
                basepage.InsertNode(DicomMessagingServicesPath, NodePath2 + "/service/presentationContext[1]", "<transferSyntax name=\"Jpeg90\" />", false);
                basepage.InsertNode(DicomMessagingServicesPath, NodePath2 + "/service/presentationContext[1]", "<transferSyntax name=\"Jpeg70\" />", false, false);
                basepage.InsertNode(DicomMessagingServicesPath, NodePath2 + "/service/presentationContext[1]", "<transferSyntax name=\"RLE\" />", false, false);
                basepage.InsertNode(DicomMessagingServicesPath, NodePath2 + "/service/presentationContext[1]", "<transferSyntax name=\"LittleEndian\" />", false, false);
                basepage.InsertNode(DicomMessagingServicesPath, NodePath2 + "/service/presentationContext[1]", "<transferSyntax name=\"ImplicitLittleEndian\" />", false, false);
                basepage.InsertNode(DicomMessagingServicesPath, NodePath2 + "/service/presentationContext[1]", "<transferSyntax name=\"Jpeg91\" />", false, false);
                basepage.InsertNode(DicomMessagingServicesPath, NodePath2 + "/service/presentationContext[1]", "<transferSyntax name=\"JpegBaseline\" />", false, false);

                ExecutedSteps++;

                //Step-9: 1. Navigate to WebAccess\Config\MergeCom3\MERGECOM.SRV and make given changes
                //Checking if build MergeCom file is not changed according to what we had modified during scripting
                if (FileUtils.CompareTextFiles(MERGECOMFilePath, MergeComFileLocation[0]))
                {
                    //If same , replace updated file at location
                    File.Copy(MergeComFileLocation[1], MERGECOMFilePath, true);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Test case failed since MergeCom file changed in new build, please update and check in at SVN location as per latest");
                }

                //Step-10: 1. Launch Service Tool
                //2. Click Restart IIS and window services
                servicetool.LaunchServiceTool();
                Thread.Sleep(10000);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                servicetool.CloseConfigTool();
                ExecutedSteps++;

                //Step-11: Login to iCA 7.0 as administrator
                login.LoginIConnect(username, password);
                ExecutedSteps++;

                //Step-12: Navigate to studies tab and Search for Patient ID=6559464
                studies = login.Navigate<Studies>();
                studies.SearchStudy(LastName: "*", patientID: PatientID);
                studies.SelectStudy("Patient ID", PatientID);
                ExecutedSteps++;

                //Step-13: Launch study in Enterprise viewer
                bluringviewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool step13 = bluringviewer.CompareImage(result.steps[ExecutedSteps], bluringviewer.ViewPortContainer());

                if (step13)
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
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);
                login.Logout();

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                //Reverting the original files taken as backup
                File.Copy(BackupFolderPath + Path.DirectorySeparatorChar + Path.GetFileName(WebaccessconfigurationPath), WebaccessconfigurationPath, true);
                File.Copy(BackupFolderPath + Path.DirectorySeparatorChar + Path.GetFileName(DicomMessagingServicesPath), DicomMessagingServicesPath, true);
                File.Copy(BackupFolderPath + Path.DirectorySeparatorChar + Path.GetFileName(MERGECOMFilePath), MERGECOMFilePath, true);
                RestartIISUsingexe();

                try
                {
                    //Deleting uploaded study
                    var hplogin = new HPLogin();
                    //BasePage.Driver.Navigate().GoToUrl("https://" + DS1 + "/webadmin");
                    var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: "https://" + DS1 + "/webadmin");
                    var workflow = (WorkFlow)hphome.Navigate("Workflow");
                    workflow.NavigateToLink("Workflow", "Archive Search");
                    workflow.HPSearchStudy("PatientID", PatientID);
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
        /// Cleanup script to close browser
        /// </summary>
        public void Test_Cleanup()
        {
            login.CloseBrowser();
        }
    }
}
