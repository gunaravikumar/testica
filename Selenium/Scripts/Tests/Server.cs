using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.MergeServiceTool;
using System.IO;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Pages.iConnect;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using Selenium.Scripts.Pages.iConnect;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Remote;
using System.ServiceProcess;
using Dicom.Network;
using System.Xml;

namespace Selenium.Scripts.Tests
{
class Server : BasePage
{
    public Login login { get; set; }
    public string filepath { get; set; }
    public Imager imager = new Imager();
    DirectoryInfo CurrentDir = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());

    public Server(String classname)
    {
        login = new Login();
        login.DriverGoTo(login.url);
        filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
    }

        public TestCaseResult Test_163385(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            ServiceTool servicetool = new ServiceTool();
            WpfObjects wpfobject = new WpfObjects();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string[] PatientIDSplit = Patientid.Split('|');
            String PatientID1 = PatientIDSplit[0];
            string PatientID2 = PatientIDSplit[1];
            string PatientID3 = PatientIDSplit[2];
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string[] PatientDescSplit = ImageCount.Split('|');
            String PatientDesc1 = PatientDescSplit[0];
            string PatientDesc2 = PatientDescSplit[1];
            string PatientDesc3 = PatientDescSplit[2];
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step1 :: View an exam that has a series that has more than 15 images of modality CT. (ex. Colin Paul Threed) in Universal viewer.  
                //Step2 :: Select the MPR view option from the smart view drop down.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool step1and2 = z3dvp.searchandopenstudyin3D(PatientID1, PatientDesc1, BluRingZ3DViewerPage.MPR);
                if (step1and2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                z3dvp.CloseViewer();
                //Step3 :: View an exam that has a series that has more than 15 images of modality PET. (ex. Petsy Threed) in Universal viewer.
                //Step4 :: Select the 3D 4:1 layout option from the smart view drop down.
                bool step3and4 = z3dvp.searchandopenstudyin3D(PatientID2, PatientDesc2, BluRingZ3DViewerPage.Three_3d_4);
                if (step3and4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                z3dvp.CloseViewer();

                //Step5 :: View an exam that has a series that has more than 15 images of modality MR. (Ex. Headneck6 Phantom) in universal viewer.
                //Step6 :: Select the 3D 6:1 layout option from the smart view drop down.
                bool step5and6 = z3dvp.searchandopenstudyin3D(PatientID3, PatientDesc3, BluRingZ3DViewerPage.Three_3d_6 , field: "acc");
                if (step5and6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
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
                z3dvp.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163387(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            ServiceTool servicetool = new ServiceTool();
            WpfObjects wpfobject = new WpfObjects();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String Drsyspath = TestDataRequirements.Split('|')[0];
            String Z3DGate = TestDataRequirements.Split('|')[1];
            String NunberofZ3Dgate = TestDataRequirements.Split('|')[2];
            String IBMAppGateManager = TestDataRequirements.Split('|')[3];
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step1 :: Set Z3DGateCanStartOnDemand = 0 in the AppGateServer section in INI file.::C:\drs\sys\data\drsys_ica.ini 
                //string filepath = @"C:\drs\sys\data\drsys_ica.ini";
                var xmlcontent = File.ReadAllLines(Drsyspath);
                bool status1 = false;
                for (int i = 0; i < xmlcontent.Length; i++)
                {
                    if (xmlcontent[i].Contains(Z3DGate))
                    {
                        xmlcontent[i] = Z3DGate + "         = 0";
                        File.WriteAllLines(Drsyspath, xmlcontent);
                        status1 = true;
                        break;
                    }
                }
                if(status1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step2 :: Set NumberOfZ3DGateInstance = 1 in the AppGateServer section in INI file
                xmlcontent = File.ReadAllLines(Drsyspath);
                bool status2 = false;
                for (int i = 0; i < xmlcontent.Length; i++)
                {
                    if (xmlcontent[i].Contains(NunberofZ3Dgate))
                    {
                        xmlcontent[i] = NunberofZ3Dgate + "        = 1";
                        File.WriteAllLines(Drsyspath, xmlcontent);
                        status2 = true;
                        break;
                    }
                }
                if (status2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step3 :: Restart Merge appgate manager service.
                ServiceController serviceController = new ServiceController(IBMAppGateManager);
                try
                {
                    if ((serviceController.Status.Equals(ServiceControllerStatus.Running)) || (serviceController.Status.Equals(ServiceControllerStatus.StartPending)))
                    {
                        serviceController.Stop();
                    }
                    serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                    serviceController.Start();
                    serviceController.WaitForStatus(ServiceControllerStatus.Running);
                    result.steps[++ExecutedSteps].status = "Pass";
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Error while restarting Services : " + e);
                    result.steps[++ExecutedSteps].status = "Fail";
                }

                //Step4 :: Log in to iCA and navigate to studies tab.
                //Step5 :: Search and load a 3D supported study in universal viewer.
                //Step6 :: Select a 3D supported series and Select the MPR view option from the smart view drop down.
                Driver.Quit();
                BasePage.MultiDriver.Add(login.InvokeBrowser("chrome"));
                login.SetDriver(BasePage.MultiDriver[0]);
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                PageLoadWait.WaitForFrameLoad(10);
                bool step456 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
                if (step456)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step7 :: Log in to iCA in another browser and navigate to studies tab.
                BasePage.MultiDriver.Add(login.InvokeBrowser("firefox"));
                login.SetDriver(BasePage.MultiDriver[1]);
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                bool PatientID = studies.PatientID().Displayed;
                if (PatientID)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step8 :: Search and load a 3D supported study in universal viewer.
                //login.Navigate("Studies");
                //PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                String FieldName = z3dvp.GetFieldName("patient");
                login.SearchStudy("patient", Patientid);
                PageLoadWait.WaitForLoadingMessage(30);
                PageLoadWait.WaitForFrameLoad(10);
                login.SelectStudy(FieldName, Patientid);
                PageLoadWait.WaitForFrameLoad(5);
                var viewer = BluRingViewer.LaunchBluRingViewer(fieldname: FieldName, value: Patientid);
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                bool thumbnailselction = z3dvp.selectthumbnail(ImageCount, 0, "");
                if (thumbnailselction)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step9 :: Select a 3D supported series and Select the MPR view option from the smart view drop down.
                bool Layout  = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR);
                if (!Layout)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step10 :: Set Z3DGateCanStartOnDemand = 1 in the AppGateServer section in INI file
                //filepath = @"C:\drs\sys\data\drsys_ica.ini";
                xmlcontent = File.ReadAllLines(Drsyspath);
                bool status3 = false;
                for (int i = 0; i < xmlcontent.Length; i++)
                {
                    if (xmlcontent[i].Contains(Z3DGate))
                    {
                        xmlcontent[i] = Z3DGate + "        = 1";
                        File.WriteAllLines(Drsyspath, xmlcontent);
                        status3 = true;
                        break;
                    }
                }
                if (status3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step11 :: Close all browsers.
                closeallbrowser();
                result.steps[++ExecutedSteps].status = "Pass";
                //Step12 :: Restart Merge appgate manager service.
                serviceController = new ServiceController(IBMAppGateManager);
                try
                {
                    if ((serviceController.Status.Equals(ServiceControllerStatus.Running)) || (serviceController.Status.Equals(ServiceControllerStatus.StartPending)))
                    {
                        serviceController.Stop();
                    }
                    serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                    serviceController.Start();
                    serviceController.WaitForStatus(ServiceControllerStatus.Running);
                    result.steps[++ExecutedSteps].status = "Pass";
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Error while restarting Services : " + e);
                    result.steps[++ExecutedSteps].status = "Fail";
                }
                //Step13 :: Log in to iCA and navigate to studies tab.
                //Step14 :: Search and load a 3D supported study in universal viewer.
                //Step15 :: Select a 3D supported series and Select the MPR view option from the smart view drop down.
                BasePage.MultiDriver.Add(login.InvokeBrowser("chrome"));
                login.SetDriver(BasePage.MultiDriver[2]);
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Step13to15 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
                if (Step13to15)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step16 :: Log in to iCA in another browser and navigate to studies tab.
                //Step17 :: Search and load a 3D supported study in universal viewer.
                //Step18 :: Select a 3D supported series and Select the MPR view option from the smart view drop down.
                BasePage.MultiDriver.Add(login.InvokeBrowser("firefox"));
                login.SetDriver(BasePage.MultiDriver[3]);
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Step16to18 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
                if (Step16to18)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                closeallbrowser();
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
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
                login.Logout();
            }
        }

        public TestCaseResult Test_163386(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ImageCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            //string filepaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string filepaths = Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + @"\163386\AG^MR AG20";
            string EAIP = Config.DestEAsIp;
            string Property90 = "Jpeg90";//
            string Property70 = "Jpeg70";//
            string PropertyREL = "REL";//
            string Property51 = "Jpeg51";
            string Property57 = "Jpeg57";
            string Property91 = "Jpeg91";//
            string LittleEndian = "LittleEndian";//
            string Implicit = "ImplicitLittleEndian";//
            string Baseline = "JpegBaseline";//
            string location = @"C:\WebAccess\WebAccess\Config\DicomMessagingServices.xml";
            string DS = Config.DestEAsIp;
            string DSAETitle = "ECM_ARC_84";
            string[] str = { Property90, Property70, PropertyREL, Property51, Property57, LittleEndian, Implicit, Property91, Baseline };
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            String adminUserName = Config.adminUserName;
            String adminPassword = Config.adminPassword;
            try
            {
                //Pre-Condition
                z3dvp.AddNodeInXmlfile(location, Property57, "1.2.840.10008.1.2.4.57");
                z3dvp.AddNodeInXmlfile(location, Property51, "1.2.840.10008.1.2.4.51");
                //AddAttribute(location, "ServiceList/service[@name='StoreSCP']/presentationContext[@abstractSyntax='MR']/transferSyntax" , "name" , "Jpeg57");
                z3dvp.AddAttributeInsideNode(location, "ServiceList/service[@name='StoreSCP']/presentationContext[@abstractSyntax='MR']", "transferSyntax", Property51);
                z3dvp.AddAttributeInsideNode(location, "ServiceList/service[@name='StoreSCP']/presentationContext[@abstractSyntax='MR']", "transferSyntax", Property57);
                //Step 1 :: Launch iCA application and log in as any user
                z3dvp.DeleteEAStudy(EAIP, Patientid);
                z3dvp.Commentxmlnodeline(location, str);
                z3dvp.UnCommentxmlnodeline(location, Implicit);
                z3dvp.ChangePropertyandRestartEA("1.2.840.10008.1.2");
                //5.Upload Study
                z3dvp.UploadEAStudy(filepaths, DS, DSAETitle);
                //new Login().DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                bool step123 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
                if (step123)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 4 ::Click on the close button from the Global toolbar.
                z3dvp.CloseViewer();
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                {
                    login.Logout();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    result.steps[++ExecutedSteps].status = "Fail";
                //Step 5and6 :: From studies tab and select the study with following transfer syntax - Explicit VR Little Endian Uncompressed - 1.2.840.10008.1.2.1 and load it in universal viewer
                //Pre-Condition
                z3dvp.DeleteEAStudy(EAIP, Patientid);
                z3dvp.Commentxmlnodeline(location, str);
                z3dvp.UnCommentxmlnodeline(location, LittleEndian);
                z3dvp.ChangePropertyandRestartEA("1.2.840.10008.1.2.1");
                //5.Upload Study
                z3dvp.UploadEAStudy(filepaths, DS, DSAETitle);
               // new Login().DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                bool step5and6 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
                if (step5and6)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 7 ::Click on the close button from the Global toolbar. 
                z3dvp.CloseViewer();
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                {
                    login.Logout();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    result.steps[++ExecutedSteps].status = "Fail";
                //Step 8 :: From studies tab and select the study with following transfer syntax - Lossy JPEG 8 Bit JPEG Baseline (Process 1) Compression - 1.2.840.10008.1.2.4.50 and load it in universal viewer
                //Pre-Condition
                z3dvp.DeleteEAStudy(EAIP, Patientid);
                z3dvp.Commentxmlnodeline(location, str);
                z3dvp.UnCommentxmlnodeline(location, Baseline);
                z3dvp.ChangePropertyandRestartEA("1.2.840.10008.1.2.4.50");
                //5.Upload Study
                z3dvp.UploadEAStudy(filepaths, DS, DSAETitle);
                //new Login().DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                bool step8and9 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
                if (step8and9)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 10 :: Click on the close button from the Global toolbar.
                z3dvp.CloseViewer();
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                {
                    login.Logout();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    result.steps[++ExecutedSteps].status = "Fail";
                //Step 11 :: From studies tab and select the study with following transfer syntax - Lossy JPEG 12 Bit JPEG Baseline (Process 4) Compression - 1.2.840.10008.1.2.4.51 and load it in universal viewer.
                z3dvp.DeleteEAStudy(EAIP, Patientid);
                z3dvp.Commentxmlnodeline(location, str);
                z3dvp.UnCommentxmlnodeline(location, Property51);
                z3dvp.ChangePropertyandRestartEA("1.2.840.10008.1.2.4.51");
                //5.Upload Study
                z3dvp.UploadEAStudy(filepaths, DS, DSAETitle);
               // new Login().DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                bool step11and12 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
                if (step11and12)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 13 :: Click on the close button from the Global toolbar.
                z3dvp.CloseViewer();
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                {
                    login.Logout();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    result.steps[++ExecutedSteps].status = "Fail";
                //Step 14:: From studies tab and select the study with following transfer syntax - Lossless, non-hierarchical, JPEG coding process 14 Compression - 1.2.840.10008.1.2.4.57 and load it in universal viewer
                z3dvp.DeleteEAStudy(EAIP, Patientid);
                z3dvp.Commentxmlnodeline(location, str);
                z3dvp.UnCommentxmlnodeline(location, Property57);
                z3dvp.ChangePropertyandRestartEA("1.2.840.10008.1.2.4.57");
                //5.Upload Study
                z3dvp.UploadEAStudy(filepaths, DS, DSAETitle);
                //new Login().DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                bool step14and15 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
                if (step14and15)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 16 :: Click on the close button from the Global toolbar.
                z3dvp.CloseViewer();
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                {
                    login.Logout();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    result.steps[++ExecutedSteps].status = "Fail";
                //Step 17 ::From studies tab and select the study with following transfer syntax - Lossless, non-hierarchical, first-order prediction, JPEG coding process 14 (selection value 1) Compression - 1.2.840.10008.1.2.4.70 and load it in viewer 
                z3dvp.DeleteEAStudy(EAIP, Patientid);
                z3dvp.Commentxmlnodeline(location, str);
                z3dvp.UnCommentxmlnodeline(location, Property70);
                z3dvp.ChangePropertyandRestartEA("1.2.840.10008.1.2.4.70");
                //5.Upload Study
                z3dvp.UploadEAStudy(filepaths, DS, DSAETitle);
                //new Login().DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                bool step17and18 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
                if (step17and18)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 19 :: Click on the close button from the Global toolbar.
                z3dvp.CloseViewer();
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                {
                    login.Logout();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    result.steps[++ExecutedSteps].status = "Fail";
                //Step 20 :: From studies tab and select the study with following transfer syntax - JPEG 2000 Lossless Image Compression - 1.2.840.10008.1.2.4.90 and load it in universal viewer
                z3dvp.DeleteEAStudy(EAIP, Patientid);
                z3dvp.Commentxmlnodeline(location, str);
                z3dvp.UnCommentxmlnodeline(location, Property90);
                z3dvp.ChangePropertyandRestartEA("1.2.840.10008.1.2.4.90");
                //5.Upload Study
                z3dvp.UploadEAStudy(filepaths, DS, DSAETitle);
                //new Login().DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                bool step20and21 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
                if (step20and21)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 22 :: Click on the close button from the Global toolbar.
                z3dvp.CloseViewer();
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                {
                    login.Logout();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    result.steps[++ExecutedSteps].status = "Fail";
                //Step 23 :: From studies tab and select the study with following transfer syntax - JPEG 2000 Lossy Image Compression - 1.2.840.10008.1.2.4.91 and load it in universal viewer
                z3dvp.DeleteEAStudy(EAIP, Patientid);
                z3dvp.Commentxmlnodeline(location, str);
                z3dvp.UnCommentxmlnodeline(location, Property91);
                z3dvp.ChangePropertyandRestartEA("1.2.840.10008.1.2.4.91");
                //5.Upload Study
                z3dvp.UploadEAStudy(filepaths, DS, DSAETitle);
               // new Login().DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                bool step23and24 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
                if (step23and24)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 25 :: Close the Z3D session and navigate to studies tab
                z3dvp.CloseViewer();
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                {
                    login.Logout();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    result.steps[++ExecutedSteps].status = "Fail";
                //Step 26 :: Navigate to studies tab and select the study with following transfer syntax - RLE Run Length Encoding (Lossless) - 1.2.840.10008.1.2.5 and load it in viewer
                z3dvp.DeleteEAStudy(EAIP, Patientid);
                z3dvp.Commentxmlnodeline(location, str);
                z3dvp.UnCommentxmlnodeline(location, PropertyREL);
                z3dvp.ChangePropertyandRestartEA("1.2.840.10008.1.2.5");
                //5.Upload Study
                z3dvp.UploadEAStudy(filepaths, DS, DSAETitle);
               // new Login().DriverGoTo(url);
                login.LoginIConnect(adminUserName, adminPassword);
                bool step26and27 = z3dvp.searchandopenstudyin3D(Patientid, ImageCount, BluRingZ3DViewerPage.MPR);
                if (step23and24)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 28 :: Click on the close button from the Global toolbar. 
                z3dvp.CloseViewer();
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                LogoutBtn = login.LogoutBtn();
                if (LogoutBtn.Displayed)
                {
                    login.Logout();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    result.steps[++ExecutedSteps].status = "Fail";


                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
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

            }
        }
    }
}
