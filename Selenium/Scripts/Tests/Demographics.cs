using Dicom;
using Dicom.Network;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.eHR;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Reusable.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Selenium.Scripts.Tests
{
    class Demographics
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        ServiceTool servicetool { get; set; }

        public Demographics(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            servicetool = new ServiceTool();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        /// <summary>
        /// Verifying demographic information display in viewer on changes in demographics.xml
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_162298(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            WpfObjects wpfobject = new WpfObjects();
            String EA_91 = login.GetHostName(Config.EA91);
            String sourceLocation = "C:\\WebAccess\\WebAccess\\Config\\Imager\\Demographics.xml";           
            String backupLocation = "C:\\Users\\Administrator\\Desktop\\Demographics.xml";
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");                

                //Step 1 - Login to application and launch the bluring viewer
                login.LoginIConnect(adminUserName, adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: EA_91);
                studies.SelectStudy("Accession", Accession);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                // Step 2 -  
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));               
                if (step2)
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
                login.Logout();

                // Step 3 - Taking back-up copy of Demographics.xml                
                File.Copy(sourceLocation, backupLocation, true);
                Logger.Instance.InfoLog("Back-up of Demographics.xml was taken");
                ExecutedSteps++;

                // step 4 - Launch the demographic file and modify                 
                XmlDocument demographicXml = new XmlDocument();
                demographicXml.Load(sourceLocation);
                XmlNodeList elements = demographicXml.GetElementsByTagName("ActivatorLayer");
                XmlNode node = elements[4].ChildNodes[1];                
                node.InnerText = "attr(0008,0016/*sopClassUid*/@img)='1.2.840.10008.5.1.4.1.1.4' and starts-with(attr(0008,0016/*sopClassUid*/@img),'1.2.840.10008.5.1.4.1.1.4.')";
                Logger.Instance.InfoLog("The demographic file was modified with attr(0008,0016/*sopClassUid*/@img)='1.2.840.10008.5.1.4.1.1.4' and starts-with(attr(0008,0016/*sopClassUid*/@img),'1.2.840.10008.5.1.4.1.1.4.') under the Layer.MR");
                ExecutedSteps++;

                // Step 5 - Save the File and Restart the service tool 
                demographicXml.Save(sourceLocation);
                Logger.Instance.InfoLog("Demographics.xml was saved successfully");
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                // Step 6 - Login as Administrator and Launch the same study in enterprise viewer and verify that demographic information is not displayed on the image in viewport
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: EA_91);
                studies.SelectStudy("Accession", Accession);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));               
                if (step6)
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
                login.Logout();

                // Step 7 - Modify the demographic xml and restart the service tool after saving the xml file
                demographicXml.Load(sourceLocation);
                node = elements[4].ChildNodes[1];
                node.InnerText = "attr(0010,0024/*IssuerOfPatientIDQualifiersSequence*/@img/[*]/0040,0033/*UniversalEntityIDType*/) = 'K'";
                Logger.Instance.InfoLog("The demographic file was modified with attr(0010,0024/*IssuerOfPatientIDQualifiersSequence*/@img/[*]/0040,0033/*UniversalEntityIDType*/) = 'K' under the Layer.MR");
                demographicXml.Save(sourceLocation);
                Logger.Instance.InfoLog("Demographics.xml was saved successfully");
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                // Step 8 - Login as Administrator and Launch the same study in enterprise viewer and verify that demographic information is not displayed on the image in viewport
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: EA_91);
                studies.SelectStudy("Accession", Accession);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step8 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
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
                viewer.CloseBluRingViewer();
                login.Logout();

                // Step 9 - Modify the demographic xml and restart the service tool after saving the xml file
                demographicXml.Load(sourceLocation);
                node = elements[4].ChildNodes[1];
                node.InnerText = "attr(0010,0024/*IssuerOfPatientIDQualifiersSequence*/@img/[*]/0040,0033/*UniversalEntityIDType*/) = 'L'";
                Logger.Instance.InfoLog("The demographic file was modified with attr(0010,0024/*IssuerOfPatientIDQualifiersSequence*/@img/[*]/0040,0033/*UniversalEntityIDType*/) = 'L' under the Layer.MR");
                demographicXml.Save(sourceLocation);
                Logger.Instance.InfoLog("Demographics.xml was saved successfully");
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                // Step 10 - Login as Administrator and Launch the same study in enterprise viewer and verify that demographic information is not displayed on the image in viewport
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: EA_91);
                studies.SelectStudy("Accession", Accession);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step10 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step10)
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
                login.Logout();


                // Step 11 - Replace the modified demographics file with the original file and restart services
                File.Copy(backupLocation, sourceLocation, true);
                Logger.Instance.InfoLog("The Orginal demographics file was replaced");
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                // Step 12 - Login as Administrator and Launch the same study in enterprise viewer and verify that demographic information is not displayed on the image in viewport
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: EA_91);
                studies.SelectStudy("Accession", Accession);
                BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step12 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + " " + BluRingViewer.div_compositeViewer));
                if (step12)
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

                // Close the viewer and logout of the application
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
                File.Copy(backupLocation, sourceLocation, true);
                Logger.Instance.InfoLog("The Orginal demographics file was replaced finally");
                servicetool.InvokeServiceTool();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.CloseServiceTool();
            }
        }

        /// <summary>
        /// Demographics displayed in Global toolbar
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161611(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            string[] FullPath = null;
            string DS1 = string.Empty;
            string DS1AETitle = string.Empty;
            int DS1Port = 0;
            String EA_96 = login.GetHostName(Config.EA96);
            String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String FilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                DS1 = Config.EA96;
                DS1AETitle = Config.EA96AETitle;
                DS1Port = 12000;

                //Step1 - Launch any dicom viewer tool and open any dicom study
                var client = new DicomClient();
                FullPath = Directory.GetFiles(FilePath, "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }

                var path = FilePath + Path.DirectorySeparatorChar + "series 1";
                var file = DicomFile.Open(path);
                var DicomData = file.Dataset;
                String name = DicomData.Get<string>(DicomTag.PatientName);
                String[] PatientName = name.Split('^');
                String patientID = DicomData.Get<string>(DicomTag.PatientID);
                String dob = DicomData.Get<string>(DicomTag.PatientBirthDate);
                String[] birthDate = new String[3];
                birthDate[0] = dob.Substring(0, 4);
                birthDate[1] = dob.Substring(4, 2);
                birthDate[2] = dob.Substring(6, 2);
                DateTime dt = new DateTime(Int32.Parse(birthDate[0]), Int32.Parse(birthDate[1]), Int32.Parse(birthDate[2]));
                dob = String.Format("{0:dd-MMM-yyyy}", dt);
                int age = DateTime.Now.Year - Int32.Parse(birthDate[0]);
                string gender = DicomData.Get<string>(DicomTag.PatientSex);
                ExecutedSteps++;

                //Step2 - From iCA application, login as any user and launch the same study in the 'Universal' viewer
                login.LoginIConnect(adminUserName, adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: EA_96);
                studies.SelectStudy("Accession", Accession);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                ExecutedSteps++;

                //Step3 - Verify the demographic information in the global toolbar              
                String patientNameInGlobal = viewer.GetText("cssselector", BluRingViewer.p_PatientName);
                String dobInGlobal = viewer.GetText("cssselector", BluRingViewer.span_PatientDOB);
                String ageInGlobal = viewer.GetText("cssselector", BluRingViewer.div_PatientAge);
                String genderInGlobal = viewer.GetText("cssselector", BluRingViewer.div_PatientGender).Substring(0, 1);
                String idInGlobal = viewer.GetText("cssselector", BluRingViewer.div_PatientID);
                int patientAge = Int32.Parse(Regex.Replace(ageInGlobal, @"[^0-9]", ""));

                bool step3_1 = patientNameInGlobal.Equals(PatientName[0] + ", " + PatientName[1] + " " + PatientName[2].Substring(0, 1) + ".");
                bool step3_2 = dobInGlobal.Equals(dob);
                bool step3_3 = patientAge == age;
                bool step3_4 = genderInGlobal.Equals(gender);
                bool step3_5 = idInGlobal.Equals(patientID);
                if (step3_1 && step3_2 && step3_3 && step3_4 && step3_5)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("Expected PatientName : " + PatientName[0] + ", " + PatientName[1] + " " + PatientName[2].Substring(0, 1) + "." + "   Actual Patient Name : " + patientNameInGlobal);
                    Logger.Instance.InfoLog("Expected DOB : " + dob + "   Actual DOB : " + dobInGlobal);
                    Logger.Instance.InfoLog("Expected Age : " + age + "   Actual Age : " + ageInGlobal);
                    Logger.Instance.InfoLog("Expected Gender : " + gender + "   Actual Age : " + genderInGlobal);
                    Logger.Instance.InfoLog("Expected Patient ID : " + patientID + "   Actual Age : " + idInGlobal);
                }

                // Close the viewer and logout of the application
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
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + DS1 + "/webadmin");
                if (BasePage.SBrowserName.ToLower().Contains("explorer") && BasePage.Driver.Title.Contains("Certificate"))
                    BasePage.Driver.Navigate().GoToUrl("javascript:document.getElementById('overridelink').click()");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("Accessionno", Accession);
                workflow.HPDeleteStudy();
                hplogin.LogoutHPen();
            }
        }

        /// <summary>
        /// Demographic information displayed in Grant Access, Transfer and Email study window
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161613(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            WpfObjects wpfobject = new WpfObjects();
            String EA_96 = login.GetHostName(Config.EA96);
            var DS1 = Config.EA96;
            var DS1AETitle = Config.EA96AETitle;
            var DS1Port = 12000;
            String PatientID = null;

            UserManagement usermanagement = new UserManagement();
            DomainManagement domain = new DomainManagement();
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String domain1 = "Domain1_" + new Random().Next(1, 1000);
                String role1 = "Role1_" + new Random().Next(1, 1000);
                String rad1 = Config.newUserName + new Random().Next(1, 1000);
                String Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath"));

                //setting up the service tool
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();

                // Ebabling the studysharing, data transfer, and data download features
                servicetool.EnableStudySharing();
                servicetool.EnableDataTransfer();
                servicetool.EnableDataDownloader();
                servicetool.EnableEmailStudy();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();

                // configuring the email notification
                servicetool.NavigateToTab("E-mail Notification");
                servicetool.NavigateSubTab("General");
                servicetool.SetEmailNotification(AdministratorEmail: Config.AdminEmail, SMTPHost: Config.IMAPServer);
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();

                // Restart and close the service tool
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();

                //Create new domain with grant aceess, data transfer and email study
                login.LoginIConnect(adminUserName, adminPassword);
                login.Navigate("DomainManagement");
                domain.CreateDomain(domain1, role1, datasources: null);
                domain.SetCheckBoxInEditDomain("grant", 0);
                domain.SetCheckBoxInEditDomain("datatransfer", 0);
                domain.SetCheckBoxInEditDomain("emailstudy", 0);
                domain.ClickSaveNewDomain();

                // Enabling the grant access in rolemanagement
                var rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown(domain1);
                rolemanagement.SearchRole(role1);
                rolemanagement.SelectRole(role1);
                rolemanagement.ClickEditRole();
                rolemanagement.ClickElement(rolemanagement.GrantAccessRadioBtn_Anyone());
                rolemanagement.ClickSaveEditRole();
                login.Navigate("UserManagement");

                // Create a new user for the above domain
                usermanagement.CreateUser(rad1, domain1, role1);
                login.Logout();

                // Pushing dataset to EA datasource
                var client = new DicomClient();
                var FullPath = Directory.GetFiles(FilePath, "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }

                // Reading data from the dicom 
                var path = FilePath + Path.DirectorySeparatorChar + "series 1";
                var file = DicomFile.Open(path);
                var DicomData = file.Dataset;
                string DcmStudyDate = DicomData.Get<string>(DicomTag.StudyDate);
                string DcmPatientName = DicomData.Get<string>(DicomTag.PatientName);
                string DcmPatientID = DicomData.Get<string>(DicomTag.PatientID);
                string DcmStudyDescription = DicomData.Get<string>(DicomTag.StudyDescription);
                string DcmAccessionNumber = DicomData.Get<string>(DicomTag.AccessionNumber);
                string DcmDateofbirth = DicomData.Get<string>(DicomTag.PatientBirthDate);
                string DCMGender = DicomData.Get<string>(DicomTag.PatientSex);
                string DCMStudytime = DicomData.Get<string>(DicomTag.StudyTime);
                string DCMStudyID = DicomData.Get<string>(DicomTag.StudyID);

                var dcmPatientNameList = DcmPatientName.Split('^');
                String DCMStudyDateTime = domain.dateAndTimeFormat(DcmStudyDate, DCMStudytime);
                String DCMStudyDate = domain.dateFormat(DcmStudyDate);
                String DCMDOB = domain.dateFormat(DcmDateofbirth);

                // Step 1 - Login as U1 user and navigate to Studies tab	
                login.LoginIConnect(rad1, rad1);
                var studies = (Studies)login.Navigate("Studies");
                ExecutedSteps++;

                // Step 2 - Search for any prior study in the Studies tab with some search criteria	
                studies.SearchStudy(AccessionNo: Accession, Datasource: EA_96);
                ExecutedSteps++;

                // Step 3 - Select any study and click on "Grant Access" button	
                studies.SelectStudy("Accession", Accession);
                Thread.Sleep(2000);
                studies.ClickElement(studies.GrantAccessBtn());
                Thread.Sleep(2000);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#UserHomeFrame")));
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(studies.By_StudyTransferDialogDiv()));

                IList<String> grantAccessDemoInfo = new List<String>();
                grantAccessDemoInfo = BasePage.Driver.FindElements(By.CssSelector("#ctl00_StudySharingControl_m_toShareGrid tr:nth-of-type(2) td")).Select<IWebElement, String>(demo => demo.Text).ToList().Where(demo => !String.IsNullOrEmpty(demo)).ToList();
                String[] DcmDemographicList = { dcmPatientNameList[0] + ", " + dcmPatientNameList[1], DcmPatientID,
                                                DCMStudyID, DcmStudyDescription, DcmAccessionNumber, DCMStudyDate };

                var step3 = DcmDemographicList.SequenceEqual(grantAccessDemoInfo);
                if (step3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The Expected study date is :" + DCMStudyDate + " and the Actual study date is :" + grantAccessDemoInfo[5]);
                    Logger.Instance.InfoLog("The Expected studyId is :" + DCMStudyID + " and the Actual Modality is :" + grantAccessDemoInfo[2]);
                    Logger.Instance.InfoLog("The Expected PatientName is :" + dcmPatientNameList[0] + ", " + dcmPatientNameList[1] + " and the Actual PatientName is :" + grantAccessDemoInfo);
                    Logger.Instance.InfoLog("The Expected PatientID is :" + DcmPatientID + " and the Actual PatientID is :" + grantAccessDemoInfo[1]);
                    Logger.Instance.InfoLog("The Expected study Description is :" + DcmStudyDescription + " and the Actual PatientID is :" + grantAccessDemoInfo[3]);
                    Logger.Instance.InfoLog("The Expected Accession Number is :" + DcmAccessionNumber + " and the Actual PatientID is :" + grantAccessDemoInfo[4]);
                }

                // Step 4 - Close the Grant access window, select the same study and click on Transfer button
                studies.ClickElement(studies.CancelBtn_GAWindow());
                Thread.Sleep(2000);
                BasePage.Driver.SwitchTo().Frame("TabContent");
                BasePage.Driver.SwitchTo().Frame("TabContent");
                studies.ClickElement(studies.Btn_StudyPageTransfer());
                Thread.Sleep(2000);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IList<String> transferDemoInfo = new List<String>();
                transferDemoInfo = BasePage.Driver.FindElements(By.CssSelector("#ctl00_StudyTransferControl_transferDataGrid tbody tr:nth-of-type(2) td")).Select<IWebElement, String>(demo => demo.Text).ToList().Where(demo => !String.IsNullOrWhiteSpace(demo)).ToList();
                studies.ClickElement(studies.CancelBtn());
                Thread.Sleep(2000);
                var step4 = DcmDemographicList.SequenceEqual(transferDemoInfo);
                if (step4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The Expected study date is :" + DCMStudyDate + " and the Actual study date is :" + transferDemoInfo[5]);
                    Logger.Instance.InfoLog("The Expected studyId is :" + DCMStudyID + " and the Actual Modality is :" + transferDemoInfo[2]);
                    Logger.Instance.InfoLog("The Expected PatientName is :" + dcmPatientNameList[0] + ", " + dcmPatientNameList[1] + " and the Actual PatientName is :" + transferDemoInfo[0]);
                    Logger.Instance.InfoLog("The Expected PatientID is :" + DcmPatientID + " and the Actual PatientID is :" + transferDemoInfo[1]);
                    Logger.Instance.InfoLog("The Expected study Description is :" + DcmStudyDescription + " and the Actual PatientID is :" + transferDemoInfo[3]);
                    Logger.Instance.InfoLog("The Expected Accession Number is :" + DcmAccessionNumber + " and the Actual PatientID is :" + transferDemoInfo[4]);
                }

                // Step 5 - Launch any study in the Universal viewer and email study to any guest user 
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                Dictionary<string, string> downloadedMail = new Dictionary<string, string>();
                EmailUtils ph1Email = new EmailUtils() { EmailId = Config.CustomUser1Email, Password = Config.CustomUserEmailPassword };
                ph1Email.MarkAllMailAsRead("INBOX");
                var pinnumber = viewer.EmailStudy_BR(Config.CustomUser1Email);
                downloadedMail = ph1Email.GetMailUsingIMAP("no-reply@merge.com", "Emailed Study", MarkAsRead: true, maxWaitTime: 3);
                string emaillink2 = ph1Email.GetEmailedStudyLink(downloadedMail);
                if (downloadedMail.Count > 0 && downloadedMail["Body"].ToLower().Contains(rad1))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                viewer.CloseBluRingViewer();

                // Step 6 - Launch the emailed study from guest usre in any other browser and enter the PIN no
                viewer = LaunchEmailedStudy.LaunchStudy<BluRingViewer>(emaillink2, pinnumber);
                ExecutedSteps++;

                // Step 7 - Verify the demographic information displayed in the viewer	
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool step7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.GetElement(BasePage.SelectorType.CssSelector, viewer.Activeviewport));
                if (step7)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                // step 8 - Verify the information displayed in the thumbnail below the study panel toolbar for the loaded study	
                String[] thumbnailSeriesNumber = { "S1- 5", "S2- 1", "S3- 1", "S5- 1" };
                String[] thumbnailImageNumber = { "5", "5", "5", "5" };
                String[] seriesnumber = viewer.GetStudyPanelThumbnailCaption();
                var imageNumberElement = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_imageFrameNumber));
                String[] imageNumber = new String[4];
                for (int i = 0; i < 4; i++)
                {
                    imageNumber[i] = imageNumberElement[i].GetAttribute("innerHTML");
                }

                var step8_1 = seriesnumber.SequenceEqual(thumbnailSeriesNumber);
                var step8_2 = imageNumber.SequenceEqual(thumbnailImageNumber);
                if (step8_1 && step8_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The Expected Thumbnail Series Numbers is :" + thumbnailSeriesNumber + " and the Actual series number is :" + seriesnumber);
                    Logger.Instance.InfoLog("The Expected Thumbnail Image Numbers is :" + thumbnailImageNumber + " and the Actual Modality is :" + imageNumber);
                }

                // step 9 - Verify the demographic information in the global toolbar	
                var patientName = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.p_PatientNamemedium).GetAttribute("innerHTML");
                var patientID = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_PatientID).GetAttribute("innerHTML");
                var dateofBirth = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.span_PatientDOB).GetAttribute("innerHTML");
                var patientGender = viewer.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_PatientGender).GetAttribute("innerHTML");
                DateTime StudyDOB = Convert.ToDateTime(dateofBirth);
                var step9_1 = (patientName).Contains(dcmPatientNameList[0] + ", " + dcmPatientNameList[1]);
                var step9_2 = DcmPatientID.Equals(PatientID);
                var step9_3 = DCMDOB.Equals(dateofBirth);
                var step9_4 = patientGender.StartsWith(DCMGender);
                if (step9_1 && step9_2 && step9_3 && step9_4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The Expected patientID is :" + DcmPatientID + " and the Actual patientID is :" + patientID);
                    Logger.Instance.InfoLog("The Expected PatientName is :" + dcmPatientNameList[0] + ", " + dcmPatientNameList[1] + " and the Actual PatientName is :" + transferDemoInfo);
                    Logger.Instance.InfoLog("The Expected Date of Birth is :" + DCMDOB + " and the Actual Date of Birth is :" + StudyDOB);
                    Logger.Instance.InfoLog("The Expected Gender is :" + patientGender + " and the Actual Start with  is :" + DCMGender);
                }

                // Step 10 - Verify the information present in the study panel toolbar are displayed properly	
                var studyDate = viewer.AllStudyDateAtStudyPanel();                
                var studyDescription = viewer.AllStudyInfoAtStudyPanel();
                var step10_1 = DCMStudyDateTime.Equals(studyDate[0].GetAttribute("innerHTML"));
                var step10_2 = DcmStudyDescription.Equals(studyDescription[0].GetAttribute("innerHTML"));
                if (step10_1 && step10_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The Expected study date is :" + DCMStudyDateTime + " and the Actual study date is :" + studyDate[0].GetAttribute("innerHTML"));
                    Logger.Instance.InfoLog("The Expected study Description is :" + DcmStudyDescription + " and the Actual study Description is :" + studyDescription[0].GetAttribute("innerHTML"));
                }

                // Close the viewer and logout of the application                
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
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + DS1 + "/webadmin");
                if (BasePage.SBrowserName.ToLower().Contains("explorer") && BasePage.Driver.Title.Contains("Certificate"))
                    BasePage.Driver.Navigate().GoToUrl("javascript:document.getElementById('overridelink').click()");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                Thread.Sleep(4000);
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                Thread.Sleep(3000);
                workflow.NavigateToLink("Workflow", "Archive Search");
                Thread.Sleep(3000);
                workflow.HPSearchStudy("PatientID", PatientID);
                workflow.HPDeleteStudy();
                hplogin.LogoutHPen();
            }
        }

        /// <summary>
        /// Verifying demographic information display in viewer on changes in demographics.xml
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161615(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            WpfObjects wpfobject = new WpfObjects();
            string[] FullPath = null;
            string DS1 = string.Empty;
            string DS1AETitle = string.Empty;
            int DS1Port = 0;
            String EA_96 = login.GetHostName(Config.EA91);
            DomainManagement domain = new DomainManagement();
            UserManagement usermanagement = new UserManagement();
            BluRingViewer viewer = new BluRingViewer();
            String Accession = null;
            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                EHR ehr = new EHR();
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String lastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");

                DS1 = Config.EA91;
                DS1AETitle = Config.EA91AETitle;
                DS1Port = 12000;

                // Send study to EA data source and make a note of demographics
                var client = new DicomClient();
                FullPath = Directory.GetFiles(FilePath, "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }

                var path = FilePath + Path.DirectorySeparatorChar + "series 1";
                var file = DicomFile.Open(path);
                var DicomData = file.Dataset;
                String name = DicomData.Get<string>(DicomTag.PatientName);
                String[] PatientName = name.Split('^');
                String patientID = DicomData.Get<string>(DicomTag.PatientID);
                String dob = DicomData.Get<string>(DicomTag.PatientBirthDate);
                int age = DateTime.Now.Year - Int32.Parse(dob.Substring(0, 4));
                dob = viewer.dateFormat(dob);
                string gender = DicomData.Get<string>(DicomTag.PatientSex);
                string studyID = DicomData.Get<string>(DicomTag.StudyID);
                string accession = DicomData.Get<string>(DicomTag.AccessionNumber);
                string description = DicomData.Get<string>(DicomTag.StudyDescription);
                string modality = DicomData.Get<string>(DicomTag.Modality);
                string studyDate = DicomData.Get<string>(DicomTag.StudyDate);
                string studyTime = DicomData.Get<string>(DicomTag.StudyTime);
                string referingPhysician = DicomData.Get<string>(DicomTag.ReferringPhysicianName);
                string studyDateAndTime = viewer.dateAndTimeFormat(studyDate, studyTime);

                //Precondition
                TestFixtures.UpdateFeatureFixture("bluring", value: "true:BluRing");
                TestFixtures.UpdateFeatureFixture("usersharing", value: "Always enabled");
                TestFixtures.UpdateFeatureFixture("allowshowselector", value: "True");
                TestFixtures.UpdateFeatureFixture("allowshowselectorsearch", value: "True");
                servicetool.LaunchServiceTool();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();

                // Domain, Role, User creation
                String TestDomain = "DemoDomain_145_" + new Random().Next(1, 1000);
                String Role = "DemoRole_145_" + new Random().Next(1, 1000);
                String DomainAdmin = "DemoDomainAdmin_145_" + new Random().Next(1, 1000);
                String user1 = "DemoUser_145_" + new Random().Next(1, 1000);

                login.LoginIConnect(adminUserName, adminPassword);
                domain.CreateDomain(TestDomain, TestDomain, TestDomain, DomainAdmin, null, DomainAdmin, DomainAdmin, DomainAdmin, Role, Role);
                login.Navigate("UserManagement");
                usermanagement.CreateUser(user1, TestDomain, Role);
                login.Logout();

                //step1 Launch iCA application, login as any valid user [U1] from Domain1 and navigate to User Preferences page
                login.LoginIConnect(user1, user1);
                ExecutedSteps++;

                //step2 Search for any study using last/first name in Studies tab and note down the demographic information displayed in the Studylist
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: EA_96);
                String[] demographicsInStudyList = studies.GetRowValuesInStudyList(1).ToArray();
                referingPhysician = referingPhysician.Replace("^", string.Empty);
                referingPhysician = referingPhysician + ",";
                String patientNameInStudyList = PatientName[0] + ", " + PatientName[1] + " " + PatientName[2];
                String[] demographicsInDicom = new String[] { studyDateAndTime, modality, patientNameInStudyList, patientID, description, accession, referingPhysician, "4" };
                bool step2_1 = demographicsInDicom.Count() == demographicsInStudyList.Count();
                bool step2_2 = true;
                for (int i = 0; i < demographicsInDicom.Count(); i++)
                {
                    if (!demographicsInDicom[i].Equals(demographicsInStudyList[i]))
                    {
                        step2_2 = false;
                        Logger.Instance.InfoLog("Mismatched : Expected Value : " + demographicsInDicom[i] + "    Actual Value : " + demographicsInStudyList[i]);
                    }
                    else
                        Logger.Instance.InfoLog("Matched : Expected Value : " + demographicsInDicom[i] + "    Actual Value : " + demographicsInStudyList[i]);
                }
                if (step2_1 && step2_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step3 -  From Studies tab, launch the same study in Universal viewer
                studies.SelectStudy1("Accession", Accession);
                BluRingViewer.LaunchBluRingViewer();
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_allViewportes)).Count() == 4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Get the demographics information 
                String patientNameInStandAlone = viewer.GetText("cssselector", BluRingViewer.p_PatientName);
                String dobInStandAlone = viewer.GetText("cssselector", BluRingViewer.span_PatientDOB);
                String ageInStandAlone = viewer.GetText("cssselector", BluRingViewer.div_PatientAge);
                String genderInStandAlone = viewer.GetText("cssselector", BluRingViewer.div_PatientGender).Substring(0, 1);
                String idInStandAlone = viewer.GetText("cssselector", BluRingViewer.div_PatientID);
                int patientAgeInStandAlone = Int32.Parse(Regex.Replace(ageInStandAlone, @"[^0-9]", ""));
                viewer.CloseBrowser();

                //Step4, 5 & 6- Launch TestEHR application, navigate to Imageload tab
                ehr.LaunchEHR();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Image Load");
                wpfobject.WaitTillLoad();
                ehr.SetCommonParameters("http://" + Config.IConnectIP + "/webaccess", domain: TestDomain, role: Role, user: user1, usersharing: "True");
                ehr.SetSelectorOptions(showSelector: "True", selectorsearch: "True");
                ehr.SetSearchKeys_Patient("lastname", lastName);
                ehr.SetSearchKeys_Study(Accession);
                String url = ehr.clickCmdLine("ImageLoad");
                ehr.CloseEHR();
                login.CreateNewSesion();
                viewer.NavigateToBluringIntegratorURL(url);
                viewer = (BluRingViewer)login.NavigateToIntegratorFrame(viewer: "bluring");
                ExecutedSteps += 2;
                if (viewer.GetElement(BasePage.SelectorType.CssSelector, EHR.searchResult).Text.Trim().Equals("1 Patient"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step7 - 
                String[] demographicsInEHRSearchReult = ehr.GetSearchResult(2).ToArray();
                demographicsInDicom = new String[] { "TESTER", PatientName[0], PatientName[1], patientID, dob, gender, studyDateAndTime };
                bool step7_1 = demographicsInDicom.Count() == demographicsInEHRSearchReult.Count();
                bool step7_2 = true;
                for (int i = 0; i < demographicsInDicom.Count(); i++)
                {
                    if (!demographicsInDicom[i].Equals(demographicsInEHRSearchReult[i]))
                    {
                        step7_2 = false;
                        Logger.Instance.InfoLog("Mismatched - Expected Value : " + demographicsInDicom[i] + "    Actual Value : " + demographicsInEHRSearchReult[i]);
                    }
                    else
                        Logger.Instance.InfoLog("Matched  - Expected Value : " + demographicsInDicom[i] + "    Actual Value : " + demographicsInEHRSearchReult[i]);
                }
                if (step7_1 && step7_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step8 - Select any study from the Show selector page and launch the study in Universal viewer
                login.Click("id", "ctl00_ctl05_parentGrid_check_0_3");
                viewer = BluRingViewer.LaunchBluRingViewer(mode: "Integrator", showselector: true);
                if (BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_allViewportes)).Count() == 4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step9 - 
                String patientNameInIntegrator = viewer.GetText("cssselector", BluRingViewer.p_PatientName);
                String dobInIntegrator = viewer.GetText("cssselector", BluRingViewer.span_PatientDOB);
                String ageInIntegrator = viewer.GetText("cssselector", BluRingViewer.div_PatientAge);
                String genderInIntegrator = viewer.GetText("cssselector", BluRingViewer.div_PatientGender).Substring(0, 1);
                String idInIntegrator = viewer.GetText("cssselector", BluRingViewer.div_PatientID);
                int patientAgeInIntegrator = Int32.Parse(Regex.Replace(ageInIntegrator, @"[^0-9]", ""));

                demographicsInDicom = new String[] { PatientName[0] + ", " + PatientName[1] + " " + PatientName[2].Substring(0, 1) + ".", dob, age.ToString(), gender, patientID };
                String[] demographicsInStandAlone = new String[] { patientNameInStandAlone, dobInStandAlone, patientAgeInStandAlone.ToString(), genderInStandAlone, idInStandAlone };
                String[] demographicsInIntegrator = new String[] { patientNameInIntegrator, dobInIntegrator, patientAgeInIntegrator.ToString(), genderInIntegrator, idInIntegrator };
                bool step9_1 = demographicsInStandAlone.SequenceEqual(demographicsInDicom);
                bool step9_2 = demographicsInIntegrator.SequenceEqual(demographicsInDicom);
                if (step9_1 && step9_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

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
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + DS1 + "/webadmin");
                if (BasePage.SBrowserName.ToLower().Contains("explorer") && BasePage.Driver.Title.Contains("Certificate"))
                    BasePage.Driver.Navigate().GoToUrl("javascript:document.getElementById('overridelink').click()");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("Accessionno", Accession);
                workflow.HPDeleteStudy();
                hplogin.LogoutHPen();
            }
        }

        /// <summary>
        /// Demographics displayed on Study List
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161610(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            WpfObjects wpfobject = new WpfObjects();
            String EA_96 = login.GetHostName(Config.EA96);
            var DS1 = Config.EA96;
            var DS1AETitle = Config.EA96AETitle;
            var DS1Port = 12000;
            String Accession = null;
            String PatientID = null;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String User1 = "U1" + new Random().Next(1000);
                Accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String FilePath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath"));

                //setting up the service tool
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                wpfobject.WaitTillLoad();

                //enabling the studysharing , datatransfer and data donwload features
                servicetool.EnableStudySharing();
                servicetool.EnableDataTransfer();
                servicetool.EnableDataDownloader();
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                wpfobject.ClickOkPopUp();
                wpfobject.WaitTillLoad();

                // navigate to datasource tab
                servicetool.NavigateToConfigToolDataSourceTab();
                wpfobject.WaitTillLoad();

                // Select the datasource and enable the institution check box
                servicetool.SelectDataSource(EA_96);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.DetailsBtn_Name, 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                servicetool.wpfobject.WaitTillLoad();
                wpfobject.SelectTabFromTabItems(ServiceTool.DataSource.Name.Dicom_Tab);
                Thread.Sleep(3000);
                wpfobject.SelectCheckBox("Perform Series Level Query For Institution", 1);
                wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                wpfobject.ClickButton(ServiceTool.DataSource.ID.OkBtn);
                wpfobject.WaitTillLoad();
                Thread.Sleep(3000);
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();

                // Restarting the service tool
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();

                // Create a new user
                login.LoginIConnect(adminUserName, adminPassword);
                var usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(User1, Config.adminGroupName, Config.adminRoleName);
                var domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(Config.adminGroupName);
                domainmanagement.SelectDomain(Config.adminGroupName);
                login.ClickElement(domainmanagement.EditDomainButton());
                domainmanagement.SetCheckBoxInEditDomain("grant", 0);
                domainmanagement.ClickSaveDomain();
                var rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown(Config.adminGroupName);
                rolemanagement.SearchRole(Config.adminRoleName);
                rolemanagement.SelectRole(Config.adminRoleName);
                rolemanagement.ClickEditRole();
                rolemanagement.ClickElement(rolemanagement.GrantAccessRadioBtn_Anyone());
                rolemanagement.ClickSaveEditRole();
                login.Logout();

                // Step 1 - From any dicom viewer tool, launch any dicom study
                // Pushing dataset to EA datasource
                var client = new DicomClient();
                var FullPath = Directory.GetFiles(FilePath, "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }
                var path = FilePath + Path.DirectorySeparatorChar + "series 1";
                var file = DicomFile.Open(path);
                var DicomData = file.Dataset;
                string DcmStudyDate = DicomData.Get<string>(DicomTag.StudyDate);
                string DcmModality = DicomData.Get<string>(DicomTag.Modality);
                string DcmPatientName = DicomData.Get<string>(DicomTag.PatientName);
                string DcmPatientID = DicomData.Get<string>(DicomTag.PatientID);
                string DcmStudyDescription = DicomData.Get<string>(DicomTag.StudyDescription);
                string DcmAccessionNumber = DicomData.Get<string>(DicomTag.AccessionNumber);
                string DcmReferringPhysicianName = DicomData.Get<string>(DicomTag.ReferringPhysicianName);
                string DcmDateofbirth = DicomData.Get<string>(DicomTag.PatientBirthDate);
                string DCMGender = DicomData.Get<string>(DicomTag.PatientSex);
                string DCMStudyID = DicomData.Get<string>(DicomTag.StudyID);
                string DCMInstitution = DicomData.Get<string>(DicomTag.InstitutionName);
                string DCMIssuerOfPatientID = DicomData.Get<string>(DicomTag.IssuerOfPatientID);
                string DCMStudyUID = DicomData.Get<string>(DicomTag.StudyInstanceUID);
                string DCMBodypart = DicomData.Get<string>(DicomTag.BodyPartExamined);
                string DCMStudytime = DicomData.Get<string>(DicomTag.StudyTime);
                ExecutedSteps++;

                //Step 2 - Login to application and search the study
                login.LoginIConnect(adminUserName, adminPassword);
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accession, Datasource: EA_96);
                ExecutedSteps++;

                // Step 3 - Add all the columns in the study list by clicking on "Choose Columns" in the bottom left corner of the study list
                studies.ClickChooseColumns();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(studies.selectColumnsDialog()));
                studies.ClickElement(studies.AddAllLink());
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(studies.selectColumnsDialog()));
                if (studies.AvailableElements().Count == 0)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                studies.ClickElement(studies.OKButton_ChooseColumns());


                // Step 4 - Compare the information present in the study list with that of the study launched in the dicom viewer                            
                IList<String> studyDetails = studies.GetRowValuesInStudyList(1);
                var dcmPatientNameList = DcmPatientName.Split('^');
                var dcmPatientName = dcmPatientNameList[0] + ", " + dcmPatientNameList[1] + " " + dcmPatientNameList[2];
                var dcmphysicianNameList = DcmReferringPhysicianName.Split('^');
                var dcmReferingPhysicianName = dcmphysicianNameList[0] + ",";
                String DCMStudyDateTime = studies.dateAndTimeFormat(DcmStudyDate, DCMStudytime);
                String DCMDateofBirth = studies.dateFormat(DcmDateofbirth);
                String[] dcmData =      { DCMStudyDateTime, DcmModality, dcmPatientName, DcmPatientID, DcmStudyDescription,
                                        DcmAccessionNumber, dcmReferingPhysicianName, DCMDateofBirth, DCMGender, DCMStudyID,
                                        DCMInstitution, DCMIssuerOfPatientID, DCMStudyUID, DCMBodypart, dcmPatientNameList[0],
                                        dcmPatientNameList[1], dcmPatientNameList[2]};

                String[] studyListData = { studyDetails[0], studyDetails[1], studyDetails[2].Trim(), studyDetails[3], studyDetails[4], studyDetails[5],
                                        studyDetails[6].Trim(), studyDetails[11], studyDetails[12], studyDetails[13], studyDetails[15], studyDetails[16],
                                        studyDetails[17], studyDetails[18], studyDetails[8], studyDetails[9], studyDetails[10]};
                var step4 = dcmData.SequenceEqual(studyListData);

                if (step4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The Expected study date is :" + DCMStudyDateTime + " and the Actual study date is :" + studyDetails[0]);
                    Logger.Instance.InfoLog("The Expected Modality is :" + DcmModality + " and the Actual Modality is :" + studyDetails[1]);
                    Logger.Instance.InfoLog("The Expected PatientName is :" + dcmPatientName + " and the Actual PatientName is :" + studyDetails[2].Trim());
                    Logger.Instance.InfoLog("The Expected PatientID is :" + DcmPatientID + " and the Actual PatientID is :" + studyDetails[3]);
                    Logger.Instance.InfoLog("The Expected StudyDescription is :" + DcmStudyDescription + " and the Actual StudyDescription is :" + studyDetails[4]);
                    Logger.Instance.InfoLog("The Expected AccessionNumber is :" + DcmAccessionNumber + " and the Actual AccessionNumber is :" + studyDetails[5]);
                    Logger.Instance.InfoLog("The Expected refering Physician is :" + dcmReferingPhysicianName + " and the Actual refering Physician is :" + studyDetails[6].Trim());
                    Logger.Instance.InfoLog("The Expected DOB date is :" + DCMDateofBirth + " and the Actual DOB date is :" + studyDetails[11]);
                    Logger.Instance.InfoLog("The Expected Gender is :" + DCMGender + " and the Actual Gender is :" + studyDetails[12]);
                    Logger.Instance.InfoLog("The Expected study ID is :" + DCMStudyID + " and the Actual study ID is :" + studyDetails[14]);
                    Logger.Instance.InfoLog("The Expected Instution is :" + DCMInstitution + " and the Actual Instution is :" + studyDetails[15]);
                    Logger.Instance.InfoLog("The Expected Issuer of patient ID is :" + DCMIssuerOfPatientID + " and the Actual Issuer of patient ID is :" + studyDetails[16]);
                    Logger.Instance.InfoLog("The Expected study UID is :" + DCMStudyUID + " and the Actual study UID is :" + studyDetails[17]);
                    Logger.Instance.InfoLog("The Expected studybodypart is :" + DCMBodypart + " and the Actual studybodypart is :" + studyDetails[18]);
                    Logger.Instance.InfoLog("The Expected lastname is :" + dcmPatientNameList[0] + " and the Actual lastname is :" + studyDetails[8]);
                    Logger.Instance.InfoLog("The Expected firstname is :" + dcmPatientNameList[1] + " and the Actual firstname is :" + studyDetails[9]);
                    Logger.Instance.InfoLog("The Expected Middlename is :" + dcmPatientNameList[2] + " and the Actual Middlename is :" + studyDetails[10]);
                }

                // Step 5 - Make sure studies are available in the Inbounds and Outbounds tab of any user and verify demographic information in study list of Inbounds and Outbounds tab
                studies.SelectStudy("Accession", Accession);
                studies.GrantAccessToUsers(Config.adminGroupName, User1);
                var outbound = (Outbounds)login.Navigate("Outbounds");
                outbound.SearchStudy(AccessionNo: Accession, Study_Performed_Period: "All Dates", Study_Received_Period: "All Dates");
                outbound.ClickElement(outbound.GetElement(BasePage.SelectorType.CssSelector, Outbounds.divChooseColumns));
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Outbounds.divSelectColumnDialog)));
                outbound.ClickElement(outbound.GetElement(BasePage.SelectorType.CssSelector, Outbounds.divAddAllLink));
                outbound.ClickElement(studies.OKButton_ChooseColumns());
                studyDetails = BasePage.Driver.FindElements(By.CssSelector(Outbounds.divSearchResultsTable + " tr[id='1'] td")).Select<IWebElement, String>(demo => demo.Text).ToList().Where(deomo => !String.IsNullOrWhiteSpace(deomo)).ToList();

                //Logout as Admin
                login.Logout();

                // Login as User
                login.LoginIConnect(User1, User1);
                var inbounds = (Inbounds)login.Navigate("Inbounds");
                inbounds.SearchStudy(AccessionNo: Accession, Study_Performed_Period: "All Dates", Study_Received_Period: "All Dates");
                PageLoadWait.WaitForPageLoad(20);
                inbounds.ClickChooseColumns();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Inbounds.divSelectColumnsDialog)));
                inbounds.ClickElement(inbounds.GetElement(BasePage.SelectorType.CssSelector, Inbounds.divAddAllLink));
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Inbounds.divSelectColumnsDialog)));
                inbounds.ClickElement(studies.OKButton_ChooseColumns());
                IList<String> InboundstudyDetails = BasePage.Driver.FindElements(By.CssSelector(Inbounds.divSearchResultsTable + " tr[id='1'] td")).Select<IWebElement, String>(demo => demo.Text).ToList().Where(deomo => !String.IsNullOrWhiteSpace(deomo)).ToList();
                //}
                String[] dcmData2 =      { DCMStudyDateTime, DcmModality, dcmPatientName, DcmPatientID, DcmStudyDescription,
                                        DcmAccessionNumber, dcmphysicianNameList[0], DCMDateofBirth, DCMGender, DCMStudyID,
                                        DCMInstitution, DCMIssuerOfPatientID, dcmPatientNameList[0],
                                        dcmPatientNameList[1]};

                String[] outboundData = {studyDetails[4], studyDetails[5], studyDetails[0].Trim(), studyDetails[1], studyDetails[7], studyDetails[8],
                                           studyDetails[9].Trim(), studyDetails[12], studyDetails[6], studyDetails[13], studyDetails[14], studyDetails[15], studyDetails[10], studyDetails[11]};

                String[] inboundData = {InboundstudyDetails[8], InboundstudyDetails[9], InboundstudyDetails[4].Trim(), InboundstudyDetails[3], InboundstudyDetails[6],
                                        InboundstudyDetails[10], InboundstudyDetails[11].Trim(), InboundstudyDetails[5], InboundstudyDetails[7], InboundstudyDetails[14],
                                        InboundstudyDetails[1], InboundstudyDetails[15], InboundstudyDetails[12], InboundstudyDetails[13]};

                var step5_1 = dcmData2.SequenceEqual(outboundData);
                var step5_2 = dcmData2.SequenceEqual(inboundData);
                if (step5_1 && step5_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The Expected study date is :" + DCMStudyDateTime + " and the Actual study date in outbounds is :" + studyDetails[4]);
                    Logger.Instance.InfoLog("The Expected Modality is :" + DcmModality + " and the Actual Modality is :" + studyDetails[5]);
                    Logger.Instance.InfoLog("The Expected PatientName is :" + dcmPatientName + " and the Actual PatientName is :" + studyDetails[0].Trim());
                    Logger.Instance.InfoLog("The Expected PatientID is :" + DcmPatientID + " and the Actual PatientID is :" + studyDetails[1]);
                    Logger.Instance.InfoLog("The Expected StudyDescription is :" + DcmStudyDescription + " and the Actual StudyDescription is :" + studyDetails[7]);
                    Logger.Instance.InfoLog("The Expected AccessionNumber is :" + DcmAccessionNumber + " and the Actual AccessionNumber is :" + studyDetails[8]);
                    Logger.Instance.InfoLog("The Expected refering Physician is :" + dcmphysicianNameList[0] + " and the Actual refering Physician is :" + studyDetails[9].Trim());
                    Logger.Instance.InfoLog("The Expected DOB date is :" + DCMDateofBirth + " and the Actual DOB date is :" + studyDetails[12]);
                    Logger.Instance.InfoLog("The Expected Gender is :" + DCMGender + " and the Actual Gender is :" + studyDetails[6]);
                    Logger.Instance.InfoLog("The Expected study ID is :" + DCMStudyID + " and the Actual study ID is :" + studyDetails[13]);
                    Logger.Instance.InfoLog("The Expected Instution is :" + DCMInstitution + " and the Actual Instution is :" + studyDetails[14]);
                    Logger.Instance.InfoLog("The Expected Issuer of patient ID is :" + DCMIssuerOfPatientID + " and the Actual Issuer of patient ID is :" + studyDetails[15]);
                    Logger.Instance.InfoLog("The Expected Lastname is :" + dcmPatientNameList[0] + " and the Actual Lastname is :" + studyDetails[10]);
                    Logger.Instance.InfoLog("The Expected Firstname is :" + dcmPatientNameList[1] + " and the Actual Firstname is :" + studyDetails[11]);

                    Logger.Instance.InfoLog("The Expected study date is :" + DCMStudyDateTime + " and the Actual study date in Inbounds is :" + InboundstudyDetails[8]);
                    Logger.Instance.InfoLog("The Expected Modality is :" + DcmModality + " and the  Actual Modality is :" + InboundstudyDetails[9]);
                    Logger.Instance.InfoLog("The Expected PatientName is :" + dcmPatientName + " and the Actual PatientName is :" + InboundstudyDetails[4].Trim());
                    Logger.Instance.InfoLog("The Expected PatientID is :" + DcmPatientID + " and the Actual PatientID is :" + InboundstudyDetails[3]);
                    Logger.Instance.InfoLog("The Expected StudyDescription is :" + DcmStudyDescription + " and the Actual StudyDescription is :" + InboundstudyDetails[6]);
                    Logger.Instance.InfoLog("The Expected AccessionNumber is :" + DcmAccessionNumber + " and the Actual AccessionNumber is :" + InboundstudyDetails[10]);
                    Logger.Instance.InfoLog("The Expected refering Physician is :" + dcmphysicianNameList[0] + " and the Actual refering Physician is :" + InboundstudyDetails[11].Trim());
                    Logger.Instance.InfoLog("The Expected DOB date is :" + DCMDateofBirth + " and the Actual DOB date is :" + InboundstudyDetails[5]);
                    Logger.Instance.InfoLog("The Expected Gender is :" + DCMGender + " and the Actual Gender is :" + InboundstudyDetails[7]);
                    Logger.Instance.InfoLog("The Expected study ID is :" + DCMStudyID + " and the Actual study ID is :" + InboundstudyDetails[14]);
                    Logger.Instance.InfoLog("The Expected Instution is :" + DCMInstitution + " and the Actual Instution is :" + InboundstudyDetails[1]);
                    Logger.Instance.InfoLog("The Expected Issuer of patient ID is :" + DCMIssuerOfPatientID + " and the Actual Issuer of patient ID is :" + InboundstudyDetails[15]);
                    Logger.Instance.InfoLog("The Expected Issuer of Lastname is :" + dcmPatientNameList[0] + " and the Actual Lastname is :" + InboundstudyDetails[12]);
                    Logger.Instance.InfoLog("The Expected Issuer of First name is :" + dcmPatientNameList[1] + " and the Actual Firstname is :" + InboundstudyDetails[13]);
                }

                // Close the viewer and logout of the application                
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
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + DS1 + "/webadmin");
                if (BasePage.SBrowserName.ToLower().Contains("explorer") && BasePage.Driver.Title.Contains("Certificate"))
                    BasePage.Driver.Navigate().GoToUrl("javascript:document.getElementById('overridelink').click()");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("PatientID", PatientID);
                workflow.HPDeleteStudy();
                hplogin.LogoutHPen();

                servicetool.LaunchServiceTool();
                servicetool.NavigateToConfigToolDataSourceTab();
                wpfobject.WaitTillLoad();
                // Select the datasource and enable the institution check box
                servicetool.SelectDataSource(EA_96);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.DetailsBtn_Name, 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                servicetool.wpfobject.WaitTillLoad();
                wpfobject.SelectTabFromTabItems(ServiceTool.DataSource.Name.Dicom_Tab);
                Thread.Sleep(3000);
                wpfobject.UnSelectCheckBox("Perform Series Level Query For Institution", 1);
                wpfobject.GetMainWindowByTitle(ServiceTool.DataSource.Name.EditDataSource_Window);
                wpfobject.ClickButton(ServiceTool.DataSource.ID.OkBtn);
                wpfobject.WaitTillLoad();
                Thread.Sleep(3000);
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                // Restarting the service tool
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();
            }
        }

        /// <summary>
        /// Demographics displayed in Reports
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161614(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            ServiceTool servicetool = new ServiceTool();
            WpfObjects wpfobject = new WpfObjects();
            string[] FullPath = null;
            string DS1 = string.Empty;
            string DS1AETitle = string.Empty;
            int DS1Port = 0;
            String EA_96 = login.GetHostName(Config.EA96);
            String[] AccessionNo = null;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AccessionList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String PatientIdList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientIDList");
                String FilePaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                AccessionNo = AccessionList.Split(':');
                String[] PatientId = PatientIdList.Split(':');
                String[] FilePath = FilePaths.Split(':');

                DS1 = Config.EA96;
                DS1AETitle = Config.EA96AETitle;
                DS1Port = 12000;

                // Send Cardio Report study to EA data source and make a note of demographics  
                var client = new DicomClient();
                FullPath = Directory.GetFiles(FilePath[1], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }

                // Send Structured Report study to EA data source and make a note of demographics  
                FullPath = Directory.GetFiles(FilePath[2], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }

                // Send HL7 Report study to EA data source and make a note of demographics  
                FullPath = Directory.GetFiles(FilePath[3], "*.*", SearchOption.AllDirectories);
                foreach (string DicomPath in FullPath)
                {
                    client.AddRequest(new DicomCStoreRequest(DicomPath));
                    client.Send(DS1, DS1Port, false, "SCU", DS1AETitle);
                }

                //Pre-condition
                //1. From iCA service tool, Navigate to Enable Features tab -> Reports subtab and enable Structured, Audio and Encapsulated PDF option
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.Report);
                wpfobject.WaitTillLoad();
                servicetool.ModifyEnableFeatures();
                if (!wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.StructuredReports, 1))
                    wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.StructuredReports, 1);
                if (!wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.AudioReports, 1))
                    wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.AudioReports, 1);
                servicetool.ApplyEnableFeatures();
                wpfobject.WaitTillLoad();
                servicetool.EnableReports();
                //2.From iCA service tool, Navigate to Enable Features tab->General subtab and enable PDF Report option
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.General);
                servicetool.ModifyEnableFeatures();
                servicetool.EnablePDFReport();
                servicetool.ApplyEnableFeatures();
                wpfobject.GetMainWindowByIndex(1);
                wpfobject.ClickButton("Yes", 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();
                //3.From Domain and Role Management -> Enable report
                login.LoginIConnect(adminUserName, adminPassword);
                login.NavigateToDomainManagementTab();
                var domainmanagement = new DomainManagement();
                domainmanagement.SearchDomain(Config.adminGroupName);
                domainmanagement.SelectDomain(Config.adminGroupName);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("pdfreport", 0);
                domainmanagement.SaveButton().Click();
                login.Navigate("Role Management");
                var rolemanagement = new RoleManagement();
                rolemanagement.SearchRole(Config.adminRoleName, Config.adminGroupName);
                rolemanagement.SelectRole(Config.adminRoleName);
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("pdfreport", 0);
                rolemanagement.SaveBtn().Click();
                login.Logout();

                //Step1 Login to iCA as any valid user for which report feature is enabled
                login.LoginIConnect(adminUserName, adminPassword);
                ExecutedSteps++;

                //Step2 Search for a study that has multiple cardio reports in the Studies tab and launch it in the 'Universal' viewer
                var studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionNo[0], Datasource: EA_96);
                studies.SelectStudy("Accession", AccessionNo[0]);
                var viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //step3 Click on the Report icon from the Exam list for the loaded study
                viewer.OpenReport_BR(1, "PDF", accession: AccessionNo[0]);
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                viewer.NavigateToReportFrame(reporttype: "PDF");
                if (BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.PDFContainer_div)).Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step4 Verify the demographic information displayed in the report matches with the demographic information displayed on the Universal viewer and the original study                

                //Collecing patient data from Cardio Report
                viewer.SelectReport_BR(0, 0, "PDF");
                var report_data1 = viewer.FetchCardioReportData_BR(1);

                // Collecting patient information from Universal viewer
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                String patientNameInGlobal = viewer.GetText("cssselector", BluRingViewer.p_PatientName);
                String dobInGlobal = viewer.GetText("cssselector", BluRingViewer.span_PatientDOB);
                String genderInGlobal = viewer.GetText("cssselector", BluRingViewer.div_PatientGender).Substring(0, 1);
                String idInGlobal = viewer.GetText("cssselector", BluRingViewer.div_PatientID);

                //Collecting Patient information from DICOM file
                var path = FilePath[1] + Path.DirectorySeparatorChar + "1I00003.dcm";
                var file = DicomFile.Open(path);
                var DicomData = file.Dataset;
                String name = DicomData.Get<string>(DicomTag.PatientName);
                String[] PatientNameDCM = name.Split('^');
                String patientIDDCM = DicomData.Get<string>(DicomTag.PatientID);
                String dobDCM = DicomData.Get<string>(DicomTag.PatientBirthDate);
                string genderDCM = DicomData.Get<string>(DicomTag.PatientSex);

                bool patientid = string.Equals(report_data1["MRN"], patientIDDCM) && String.Equals(idInGlobal, patientIDDCM);
                bool patient = string.Equals(report_data1["Patient"].ToLower(), PatientNameDCM[1] + " " + PatientNameDCM[0].ToLower()) &&
                    string.Equals(patientNameInGlobal, PatientNameDCM[0] + ", " + PatientNameDCM[1]);
                var patientdobreport = Convert.ToDateTime(report_data1["DOB"].Trim());
                var patientdobdcm = Convert.ToDateTime(viewer.dateFormat(dobDCM));
                var patientdobglobal = Convert.ToDateTime(dobInGlobal);
                bool patientdob = DateTime.Equals(patientdobreport, patientdobdcm) && DateTime.Equals(patientdobglobal, patientdobdcm);
                bool patientgender = string.Equals(report_data1["Gender"], genderDCM) && string.Equals(genderInGlobal, genderDCM);
                if (patientid && patient && patientdob && patientgender)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("PatientName from DCM: " + PatientNameDCM[0] + ", " + PatientNameDCM[1] + " Patient Name from UV: "
                        + patientNameInGlobal + " Patient Name from report: " + report_data1["Patient"]);
                    Logger.Instance.InfoLog("PatientID from DCM: " + patientIDDCM + " Patient Id from UV: " + idInGlobal +
                        " Patient Id from report:" + report_data1["MRN"]);
                    Logger.Instance.InfoLog("Patient DOB from DCM: " + patientdobdcm + " Patient DOB from UV: " + dobInGlobal +
                        " Patient DOB from report:" + report_data1["DOB"]);
                    Logger.Instance.InfoLog("Patient Gender from DCM: " + genderDCM + " Patient Gender from UV: " + genderInGlobal +
                        " Patient Gender from report:" + report_data1["Gender:"]);
                }

                //Step5 Search and load the study that has SR reports in the Universal viewer.
                viewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: AccessionNo[1], Datasource: EA_96);
                studies.SelectStudy("Accession", AccessionNo[1]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step6 Click on the Report icon from the Exam list for the loaded study
                viewer.OpenReport_BR(0, "SR");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                viewer.NavigateToReportFrame(reporttype: "SR");
                if (BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.pdfreport_continer)).Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step7 Verify the demographic information displayed in the report matches with the demographic information displayed on the Universal viewer and the original study
                //Collecting data from report
                report_data1 = viewer.FetchReportData_BR(0);

                // Collecting patient information from Universal viewer
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                patientNameInGlobal = viewer.GetText("cssselector", BluRingViewer.p_PatientName);
                dobInGlobal = viewer.GetText("cssselector", BluRingViewer.span_PatientDOB);
                genderInGlobal = viewer.GetText("cssselector", BluRingViewer.div_PatientGender).Substring(0, 1);
                idInGlobal = viewer.GetText("cssselector", BluRingViewer.div_PatientID);

                //Collecting Patient information from DICOM file
                path = FilePath[2] + Path.DirectorySeparatorChar + "1I00026.dcm";
                file = DicomFile.Open(path);
                DicomData = file.Dataset;
                name = DicomData.Get<string>(DicomTag.PatientName);
                PatientNameDCM = name.Split('^');
                patientIDDCM = DicomData.Get<string>(DicomTag.PatientID);
                dobDCM = DicomData.Get<string>(DicomTag.PatientBirthDate);
                genderDCM = DicomData.Get<string>(DicomTag.PatientSex);

                patientid = string.Equals(report_data1["MRN:"], patientIDDCM) && string.Equals(idInGlobal, patientIDDCM);
                bool patientfirstname = string.Equals(report_data1["First Name:"], PatientNameDCM[1]);
                bool patientlastname = string.Equals(report_data1["Last Name:"], PatientNameDCM[0]);
                patient = string.Equals(patientNameInGlobal, PatientNameDCM[0] + ", " + PatientNameDCM[1] + " " + PatientNameDCM[2] + ".");
                patientdobreport = Convert.ToDateTime(report_data1["Date of Birth:"]);
                patientdobdcm = Convert.ToDateTime(viewer.dateFormat(dobDCM));
                patientdobglobal = Convert.ToDateTime(dobInGlobal);
                patientdob = string.Equals(patientdobreport, patientdobdcm) && string.Equals(patientdobglobal, patientdobdcm);
                patientgender = string.Equals(report_data1["Gender:"], genderDCM) && string.Equals(genderInGlobal, genderDCM);

                if (patientid && patientfirstname && patientlastname && patient && patientdob && patientgender)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("PatientName from DCM: " + PatientNameDCM[0] + ", " + PatientNameDCM[1] + " " + PatientNameDCM[2] + "." +
                        " Patient Name from UV: " + patientNameInGlobal + " Patient Name from report: " + report_data1["First Name:"] + " " + report_data1["Last Name:"]);
                    Logger.Instance.InfoLog("PatientID from DCM: " + patientIDDCM + " Patient Id from UV: " + idInGlobal +
                        " Patient Id from report:" + report_data1["MRN:"]);
                    Logger.Instance.InfoLog("Patient DOB from DCM: " + patientdobdcm + " Patient DOB from UV: " + dobInGlobal +
                        " Patient DOB from report:" + report_data1["Date of Birth:"]);
                    Logger.Instance.InfoLog("Patient Gender from DCM: " + genderDCM + " Patient Gender from UV: " + genderInGlobal +
                        " Patient Gender from report:" + report_data1["Gender:"]);
                }

                //Step8 In the Studies tab, search for the study with HL7 Report attached in the MPACS
                viewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: AccessionNo[2], Datasource: EA_96);
                studies.SelectStudy("Accession", AccessionNo[2]);
                viewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                ExecutedSteps++;

                //Step9 Click on the 'Report' icon and verify the HL7 report for the study should be opened to the right of the exam list as a panel overlaying the images.
                viewer.OpenReport_BR(0, "SR");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                viewer.NavigateToReportFrame(reporttype: "SR");
                if (BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.pdfreport_continer)).Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step10 Verify the demographic information displayed in the report matches with the demographic information displayed on the Universal viewer and the original study
                //Collecting data from the report
                report_data1 = viewer.FetchReportData_BR(0);

                // Collecting patient information from Universal viewer
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                patientNameInGlobal = viewer.GetText("cssselector", BluRingViewer.p_PatientName);
                dobInGlobal = viewer.GetText("cssselector", BluRingViewer.span_PatientDOB);
                genderInGlobal = viewer.GetText("cssselector", BluRingViewer.div_PatientGender).Substring(0, 1);
                idInGlobal = viewer.GetText("cssselector", BluRingViewer.div_PatientID);

                //Collecting Patient information from DICOM file
                path = FilePath[3] + Path.DirectorySeparatorChar + "F000002.dcm";
                file = DicomFile.Open(path);
                DicomData = file.Dataset;
                name = DicomData.Get<string>(DicomTag.PatientName);
                PatientNameDCM = name.Split('^');
                patientIDDCM = DicomData.Get<string>(DicomTag.PatientID);
                dobDCM = DicomData.Get<string>(DicomTag.PatientBirthDate);
                genderDCM = DicomData.Get<string>(DicomTag.PatientSex);

                patientid = string.Equals(report_data1["MRN:"], patientIDDCM) && string.Equals(idInGlobal, patientIDDCM);
                patientfirstname = string.Equals(report_data1["First Name:"], PatientNameDCM[1]);
                patientlastname = string.Equals(report_data1["Last Name:"], PatientNameDCM[0]);
                patient = string.Equals(patientNameInGlobal, PatientNameDCM[0] + ", " + PatientNameDCM[1] + " " + PatientNameDCM[2].Substring(0, 1) + ".");
                patientdobreport = Convert.ToDateTime(report_data1["Date of Birth:"]);
                patientdobdcm = Convert.ToDateTime(viewer.dateFormat(dobDCM));
                patientdobglobal = Convert.ToDateTime(dobInGlobal);
                patientdob = string.Equals(patientdobreport, patientdobdcm) && string.Equals(patientdobglobal, patientdobdcm);
                patientgender = string.Equals(report_data1["Gender:"], genderDCM) && string.Equals(genderInGlobal, genderDCM);

                if (patientid && patientfirstname && patientlastname && patient && patientdob && patientgender)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("PatientName from DCM: " + PatientNameDCM[0] + ", " + PatientNameDCM[1] + " " + PatientNameDCM[2].Substring(0, 1) + "." +
                        " Patient Name from UV: " + patientNameInGlobal + " Patient Name from report: " + report_data1["First Name:"] + " " + report_data1["Last Name:"]);
                    Logger.Instance.InfoLog("PatientID from DCM: " + patientIDDCM + " Patient Id from UV: " + idInGlobal +
                        " Patient Id from report:" + report_data1["MRN:"]);
                    Logger.Instance.InfoLog("Patient DOB from DCM: " + patientdobdcm + " Patient DOB from UV: " + dobInGlobal +
                        " Patient DOB from report:" + report_data1["Date of Birth:"]);
                    Logger.Instance.InfoLog("Patient Gender from DCM: " + genderDCM + " Patient Gender from UV: " + genderInGlobal +
                        " Patient Gender from report:" + report_data1["Gender:"]);
                }

                //Closing Universal viewer and logout.
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
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + DS1 + "/webadmin");
                if (BasePage.SBrowserName.ToLower().Contains("explorer") && BasePage.Driver.Title.Contains("Certificate"))
                    BasePage.Driver.Navigate().GoToUrl("javascript:document.getElementById('overridelink').click()");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                for (int i = 0; i < AccessionNo.Count(); i++)
                {
                    workflow.HPSearchStudy("Accessionno", AccessionNo[i]);
                    workflow.HPDeleteStudy();
                }
                hplogin.LogoutHPen();
            }

        }
    }
}
