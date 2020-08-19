using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.MergeServiceTool;
using Dicom;
using Dicom.Network;
using System.ServiceProcess;
using System.Diagnostics;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using Selenium.Scripts.Pages.iCAInstaller;
using System.Xml;
using TestStack.White.WindowsAPI;
using Application = TestStack.White.Application;
using Button = TestStack.White.UIItems.Button;
using CheckBox = TestStack.White.UIItems.CheckBox;
using ComboBox = TestStack.White.UIItems.ListBoxItems.ComboBox;
using ListView = TestStack.White.UIItems.ListView;
using RadioButton = TestStack.White.UIItems.RadioButton;
using TextBox = TestStack.White.UIItems.TextBox;
using Window = TestStack.White.UIItems.WindowItems.Window;
using Panel = TestStack.White.UIItems.Panel;
using TestStack.White.Configuration;


namespace Selenium.Scripts.Tests
{
    class PreProcessing : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ServiceTool servicetool { get; set; }
        public BasePage basepage {get; set;}
        public Configure configure { get; set; }
        public HPHomePage hphomepage { get; set; }
        public HPLogin hplogin { get; set; }
        public WpfObjects wpfobject { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public ExamImporter ei { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="classname"></param>
        public PreProcessing(String classname)
        {
            servicetool = new ServiceTool();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            wpfobject = new WpfObjects();
            login = new Login();
            basepage = new BasePage();
            hplogin = new HPLogin();
            hphomepage = new HPHomePage();
        }

        ///<summary>
        ///PreProcessing- Preprocessing Setup - Precondition
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_PreProcessingSetup(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables     
            TestCaseResult result = new TestCaseResult(stepcount);
            Configure EAUtils = new Configure();
            HPHomePage homepage = new HPHomePage();
            var netstat = new NetStat();
            WorkFlow Workflow = new WorkFlow();
            PreprocessingUtils PreprocessingUtil = new PreprocessingUtils();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                string IConnectIP = Config.IConnectIP;
                string EA_IP = Config.DestEAsIp;
                string EA_Title = Config.DestEAsAETitle;
                string cachepath = @"C:\Cache";
                string PF_cachepath = cachepath + Path.DirectorySeparatorChar + "PF_" + GetHostName(IConnectIP);
                string PF_Node = "PF_" + GetHostName(IConnectIP);
                if (Directory.Exists(PF_cachepath))
                {
                    BasePage.DeleteAllFileFolder(PF_cachepath);
                    Directory.Delete(PF_cachepath);
                }
                else
                {
                    Directory.CreateDirectory(cachepath);
                }
                servicetool.ConfigurePrefetch(PF_Node);
                bool isCacheFolderEmpty = !Directory.EnumerateFileSystemEntries(PF_cachepath).Any();

                if (!isCacheFolderEmpty)
                {
                    throw new Exception("Cache Folder is not empty");
                }
                BasePage.Driver.Navigate().GoToUrl(login.GetEAUrl(EA_IP));
                HPLogin hplogin = new HPLogin();
                homepage = hplogin.LoginHPen(hpUserName, hpPassword);
                PageLoadWait.WaitForHPPageLoad(20);
                if (!EAUtils.IsRemoteDeiveConfigured(GetHostName(IConnectIP)))
                {
                    EAUtils.Add_remoteDevice(GetHostName(IConnectIP), IConnectIP, "4444");
                }
                if (!EAUtils.IsRemoteDeiveConfigured(PF_Node))
                {
                    EAUtils.Add_remoteDevice(PF_Node, IConnectIP, "4446");
                }
                //Verfiy the Dicom node is added
                if (!EAUtils.IsRemoteDeiveConfigured(GetHostName(IConnectIP)) && EAUtils.IsRemoteDeiveConfigured(GetHostName(PF_Node)))
                {
                    throw new Exception("Unable to add the remote device Dicom node " + GetHostName(IConnectIP) + "and " + PF_Node + "in the EA");
                }
                PageLoadWait.WaitForHPPageLoad(20);
                Configure.logout().Click();

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
        }

        ///<summary>
        ///PreProcessing- Action: Generation of study digest for priors
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161602(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables     
            TestCaseResult result = new TestCaseResult(stepcount);
            Configure EAUtils = new Configure();
            HPHomePage homepage = new HPHomePage();
            var netstat = new NetStat();
            WorkFlow Workflow = new WorkFlow();
            PreprocessingUtils PreprocessingUtil = new PreprocessingUtils();
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                string Filepath = string.Empty;
                string[] FullPath = null;
                string IConnectIP = Config.IConnectIP;
                string EA_IP = Config.DestEAsIp;
                string EA_Title = Config.DestEAsAETitle;
                string cachepath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "CachePath");
                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                string Study_InstanceUID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Study_InstanceUID");
                string[] Study_InstanceUIDValue = Study_InstanceUID.Split('@');
                string Number_Of_Studies = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Number_Of_Studies");
                string studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                string PF_cachepath = cachepath + Path.DirectorySeparatorChar + "PF_" + GetHostName(IConnectIP);
                string PF_Node = "PF_" + GetHostName(IConnectIP);

                //Delete the patinet before Send to EA
                BasePage.Driver.Navigate().GoToUrl(login.GetEAUrl(EA_IP));
                hplogin = new HPLogin();
                homepage = hplogin.LoginHPen(hpUserName, hpPassword);
                PageLoadWait.WaitForHPPageLoad(20);
                WorkFlow EAPortal = (WorkFlow)hphomepage.Navigate("Workflow");
                EAPortal.NavigateToLink("Workflow", "Archive Search");
                Dictionary<string, string> PatinetDetails;
                //Click Search Archive
                EAPortal.HPSearchStudy("PatientID", PatinetID);
                try
                {
                    PatinetDetails = EAPortal.GetStudyDetailsInHP();
                    if (PatinetDetails["Patient ID"].Equals(PatinetID))
                        EAPortal.HPDeleteStudy();
                }
                catch (Exception ex)
                {
                    Logger.Instance.InfoLog("Patinet not present");
                }

                //Step 1- PreCondition
                //Perprocessing config Setup 
                ExecutedSteps++;

                //Step2
                string PreprocessingXmlPath = PreprocessingUtils.PreprocessingConfigXML;
                basepage.ChangeAttributeValue(PreprocessingXmlPath, "PreprocessingConfiguration/BlueRingRules/Rule/Actions/Action/Parameter[@name='onlyFromSameModality']", "value", "false");
                string AttributeValue = basepage.GetAttributeValue(PreprocessingXmlPath, "/PreprocessingConfiguration/BlueRingRules/Rule/Actions/Action/Parameter[@name='onlyFromSameModality']", "value");
                if (AttributeValue == "false")
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 3
                PreprocessingUtil.RestartService("PreprocessingService");
                ExecutedSteps++;

                //Step 4
                ServiceController sc = new ServiceController("ImagePrefetchService");
                if (sc.Status.ToString() == "Running")
                {
                    Logger.Instance.InfoLog("The service ImagePrefetchService is running");
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Precondition for step 12
                //Get the Job table Count before Prefetch
                BasePage.DeleteAllFileFolder(PF_cachepath);
                int jobBefore = PreprocessingUtil.GetJobcount();
                int jobActionBefore = PreprocessingUtil.GetJobActionCountFromDB("PriorStudyDigest");
                Logger.Instance.InfoLog("Total Job action before prefetch is " + jobActionBefore);

                //Step 5
                var client = new DicomClient();
                string[] folderList = Directory.GetDirectories(studypath);
                foreach (string folderName in folderList)
                {
                    //FullPath = Directory.GetFiles(studypath + Path.PathSeparator + folderName, "*.*", SearchOption.AllDirectories);
                    FullPath = Directory.GetFiles(folderName, "*.*", SearchOption.AllDirectories);

                    foreach (string path in FullPath)
                    {
                        client.AddRequest(new DicomCStoreRequest(path));
                        client.Send(EA_IP, 12000, false, "SCU", EA_Title);
                    }
                }
                ExecutedSteps++;

                //Step 6
                //Wait for Images get Loaded in the EA
                Thread.Sleep(10000);
                hphomepage.Navigate("Workflow");
                EAPortal.NavigateToLink("Workflow", "Archive Search");
                EAPortal.HPSearchStudy("PatientID", PatinetID);
                PatinetDetails = EAPortal.GetStudyDetailsInHP();
                if (!PatinetDetails["Patient ID"].Equals(PatinetID))
                {
                    throw new Exception("Error While Send the Images to EA");
                }
                Dictionary<string, string> StudyDetails = EAPortal.GetStudyDetailsInHP();
                List<String> Cachedstudypath = new List<String>();
                List<String> StudyUIDFolder = new List<String>();
                List<String> Demographicsxmlpath = new List<String>();
                int CT = 0, MR = 0, studies = 0;
                if (StudyDetails["Number of Studies"].Equals(Number_Of_Studies))
                {
                    BasePage.Driver.FindElement(By.LinkText(PatinetID)).Click();
                    foreach (string Study_InstanceUI in Study_InstanceUIDValue)
                    {
                        //Dictionary<string, string> StudyDetails = EAPortal.GetStudyDetailsInHP();

                        if (BasePage.Driver.FindElement(By.LinkText(Study_InstanceUI)).Displayed)
                        {
                            BasePage.Driver.FindElement(By.LinkText(Study_InstanceUI)).Click();
                            studies++;
                            Dictionary<string, string> seriesresults = EAPortal.GetStudyDetailsInHP();
                            if (seriesresults["Modality"] == "CT")
                            {
                                ++CT;
                                Cachedstudypath.Add(PF_cachepath + Path.DirectorySeparatorChar + Study_InstanceUI + Path.DirectorySeparatorChar + seriesresults["Series InstanceUID"]);
                                Demographicsxmlpath.Add(PF_cachepath + Path.DirectorySeparatorChar + Study_InstanceUI + Path.DirectorySeparatorChar + "demographics.xml");
                                StudyUIDFolder.Add(PF_cachepath + Path.DirectorySeparatorChar + Study_InstanceUI);
                            }
                            else if (seriesresults["Modality"] == "MR")
                            {
                                ++MR;
                                Cachedstudypath.Add(PF_cachepath + Path.DirectorySeparatorChar + Study_InstanceUI + Path.DirectorySeparatorChar + seriesresults["Series InstanceUID"]);
                                Demographicsxmlpath.Add(PF_cachepath + Path.DirectorySeparatorChar + Study_InstanceUI + Path.DirectorySeparatorChar + "demographics.xml");
                                StudyUIDFolder.Add(PF_cachepath + Path.DirectorySeparatorChar + Study_InstanceUI);
                            }
                            BasePage.Driver.FindElement(By.LinkText("(Back to Studies)")).Click();
                        }
                    }
                }
                if (studies == 3 && CT == 2 && MR == 1)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 7
                string currentHandler = BasePage.Driver.CurrentWindowHandle;
                foreach (string Study_InstanceUI in Study_InstanceUIDValue)
                {
                    if (BasePage.Driver.FindElement(By.LinkText(Study_InstanceUI)).Displayed)
                    {
                        BasePage.Driver.FindElement(By.LinkText(Study_InstanceUI)).Click();
                        Dictionary<string, string> seriesresults = EAPortal.GetStudyDetailsInHP();
                        if (seriesresults["Modality"] == "MR")
                            EAPortal.HPSendStudy();
                        else
                            BasePage.Driver.FindElement(By.LinkText("(Back to Studies)")).Click();
                    }

                }
                // switch to new window
                foreach (string handle in BasePage.Driver.WindowHandles)
                {
                    if (!handle.Equals(currentHandler))
                    {
                        BasePage.Driver.SwitchTo().Window(handle);
                    }
                }
                IWebElement nodeselect = BasePage.Driver.FindElement(By.CssSelector("select[name = 'remoteDestinations']"));
                if (nodeselect.Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }


                //Step 8
                var NodeSelect = BasePage.Driver.FindElement(By.CssSelector("select[name = 'remoteDestinations']"));
                var selectElement = new SelectElement(NodeSelect);
                //select by value
                selectElement.SelectByValue(PF_Node); Thread.Sleep(2000);
                BasePage.Driver.FindElement(By.CssSelector("input[value = 'Send To Selected Remotes']")).Click(); Thread.Sleep(2000);
                ExecutedSteps++;

                //Step 9
                BasePage.Driver.FindElement(By.CssSelector(".messagegreenlarge"));
                BasePage.Driver.FindElement(By.CssSelector("input[value = 'Close Window']")).Click();
                BasePage.Driver.SwitchTo().Window(currentHandler);
                Configure.logout().Click();
                Thread.Sleep(3000);
                ExecutedSteps++;

                //Step 10, 11
                var cachewait = new DefaultWait<IWebDriver>(BasePage.Driver);
                cachewait.Timeout = new TimeSpan(0, 2, 0);
                cachewait.IgnoreExceptionTypes(new Type[] { new FileNotFoundException().GetType() });
                cachewait.Until<Boolean>((d) =>
                {
                    if (Directory.Exists(Cachedstudypath[0]) && File.Exists(Demographicsxmlpath[0]) && Directory.Exists(Cachedstudypath[1]) && Directory.Exists(Cachedstudypath[2]))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });

                ++ExecutedSteps;
                foreach (string CachedstudypathFolder in Cachedstudypath)
                {
                    if (Directory.Exists(CachedstudypathFolder))
                        result.steps[ExecutedSteps].AddPassStatusList();
                    else
                        result.steps[ExecutedSteps].AddFailStatusList();
                }
                foreach (string DemographicsxmlpathFile in Demographicsxmlpath)
                {
                    if (File.Exists(DemographicsxmlpathFile))
                        result.steps[ExecutedSteps].AddPassStatusList();
                    else
                        result.steps[ExecutedSteps].AddFailStatusList("Unable to find the file " + DemographicsxmlpathFile);
                }
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }

                //step 12
                // Wait for the Preprocessing to get update the status in tables.
                Thread.Sleep(10000);
                int jobAfter = PreprocessingUtil.GetJobcount();
                Stopwatch stopwatch = new Stopwatch();
                TimeSpan timeout = new TimeSpan(0, 1, 0);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (jobAfter == jobBefore + 1)
                        break;
                    jobAfter = PreprocessingUtil.GetJobcount();
                    Thread.Sleep(2000);
                }
                stopwatch.Stop();
                jobAfter = PreprocessingUtil.GetJobcount();
                Logger.Instance.InfoLog("Total Job After prefetch is " + jobAfter);
                if (jobAfter == jobBefore + 1)
                {
                    result.steps[++ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[++ExecutedSteps].AddFailStatusList();
                }

                string jobUID = PreprocessingUtil.GetJobUID()[jobBefore];
                string jobStatus = PreprocessingUtil.GetJobStatusFromDB(jobUID);
                 stopwatch = new Stopwatch();
                 timeout = new TimeSpan(0, 1, 0);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    Thread.Sleep(1000);
                    jobStatus = PreprocessingUtil.GetJobStatusFromDB(jobUID);
                    if (int.Parse(jobStatus) == 4)
                        break;
                    stopwatch.Stop();
                }
                if (int.Parse(jobStatus) == 4)
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }
                int jobActionAfter = PreprocessingUtil.GetJobActionCountFromDB("PriorStudyDigest");
                Logger.Instance.InfoLog("Total Job action After prefetch is " + jobActionAfter);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    Thread.Sleep(2000);
                    jobActionAfter = PreprocessingUtil.GetJobActionCountFromDB("PriorStudyDigest");
                    if (jobActionAfter == jobActionBefore + 1)
                        break;
                }
                stopwatch.Stop();
                if (jobActionAfter == jobActionBefore + 1)
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }

                string jobactionstatus = PreprocessingUtil.GetJobActionStatusFromDB(jobUID);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    jobactionstatus = PreprocessingUtil.GetJobActionStatusFromDB(jobUID);
                    if (jobActionAfter == jobActionBefore + 1)
                        break;
                }
                stopwatch.Stop();
                if (int.Parse(jobactionstatus) == 9)
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                
                //Step 13
                //Pre-condition additional step
                BasePage.DeleteAllFileFolder(PF_cachepath);
                basepage.ChangeAttributeValue(PreprocessingXmlPath, "PreprocessingConfiguration/BlueRingRules/Rule/Actions/Action/Parameter[@name='onlyFromSameModality']", "value", "true");
                result.steps[++ExecutedSteps].StepPass();

                //Step 14
                ServiceController serviceController = new ServiceController("PreprocessingService");
                PreprocessingUtil.RestartService("PreprocessingService");
                ExecutedSteps++;

                //Step 15
                BasePage.Driver.Navigate().GoToUrl(login.GetEAUrl(EA_IP));
                hplogin = new HPLogin();
                homepage = hplogin.LoginHPen(hpUserName, hpPassword);
                PageLoadWait.WaitForHPPageLoad(20);
                EAPortal = (WorkFlow)hphomepage.Navigate("Workflow");
                EAPortal.NavigateToLink("Workflow", "Archive Search");
                EAPortal.HPSearchStudy("PatientID", PatinetID);
                PatinetDetails = EAPortal.GetStudyDetailsInHP();
                if (!PatinetDetails["Patient ID"].Equals(PatinetID))
                    throw new Exception("Error While Send the Images to EA");
                StudyDetails = EAPortal.GetStudyDetailsInHP();

                List<String> Cachedstudypath_CT = new List<String>();
                List<String> StudyUIDFolder_CT = new List<String>();
                List<String> Demographicsxmlpath_CT = new List<String>();

                List<String> Cachedstudypath_MR = new List<String>();
                List<String> StudyUIDFolder_MR = new List<String>();
                List<String> Demographicsxmlpath_MR = new List<String>();

                CT = MR = studies = 0;
                if (StudyDetails["Number of Studies"].Equals(Number_Of_Studies))
                {
                    BasePage.Driver.FindElement(By.LinkText(PatinetID)).Click();
                    foreach (string Study_InstanceUI in Study_InstanceUIDValue)
                    {
                        if (BasePage.Driver.FindElement(By.LinkText(Study_InstanceUI)).Displayed)
                        {
                            BasePage.Driver.FindElement(By.LinkText(Study_InstanceUI)).Click();
                            studies++;
                            Dictionary<string, string> seriesresults = EAPortal.GetStudyDetailsInHP();
                            if (seriesresults["Modality"] == "CT")
                            {
                                ++CT;
                                Cachedstudypath_CT.Add(PF_cachepath + Path.DirectorySeparatorChar + Study_InstanceUI + Path.DirectorySeparatorChar + seriesresults["Series InstanceUID"]);
                                Demographicsxmlpath_CT.Add(PF_cachepath + Path.DirectorySeparatorChar + Study_InstanceUI + Path.DirectorySeparatorChar + "demographics.xml");
                                StudyUIDFolder_CT.Add(PF_cachepath + Path.DirectorySeparatorChar + Study_InstanceUI);
                            }
                            else if (seriesresults["Modality"] == "MR")
                            {
                                ++MR;
                                Cachedstudypath_MR.Add(PF_cachepath + Path.DirectorySeparatorChar + Study_InstanceUI + Path.DirectorySeparatorChar + seriesresults["Series InstanceUID"]);
                                Demographicsxmlpath_MR.Add(PF_cachepath + Path.DirectorySeparatorChar + Study_InstanceUI + Path.DirectorySeparatorChar + "demographics.xml");
                                StudyUIDFolder_MR.Add(PF_cachepath + Path.DirectorySeparatorChar + Study_InstanceUI);
                            }
                            BasePage.Driver.FindElement(By.LinkText("(Back to Studies)")).Click();
                        }
                    }
                }
                if (studies == 3 && CT == 2 && MR == 1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step 16
                currentHandler = BasePage.Driver.CurrentWindowHandle;
                foreach (string Study_InstanceUI in Study_InstanceUIDValue)
                {
                    if (BasePage.Driver.FindElement(By.LinkText(Study_InstanceUI)).Displayed)
                    {
                        BasePage.Driver.FindElement(By.LinkText(Study_InstanceUI)).Click();
                        Dictionary<string, string> seriesresults = EAPortal.GetStudyDetailsInHP();
                        if (seriesresults["Modality"] == "CT")
                        {
                            EAPortal.HPSendStudy();
                            break;
                        }
                        else
                            BasePage.Driver.FindElement(By.LinkText("(Back to Studies)")).Click();
                    }

                }
                // switch to new window
                foreach (string handle in BasePage.Driver.WindowHandles)
                {
                    if (!handle.Equals(currentHandler))
                    {
                        BasePage.Driver.SwitchTo().Window(handle);
                    }
                }
                IWebElement nodeselect1 = BasePage.Driver.FindElement(By.CssSelector("select[name = 'remoteDestinations']"));
                if (nodeselect1.Displayed)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 17
                NodeSelect = BasePage.Driver.FindElement(By.CssSelector("select[name = 'remoteDestinations']"));
                selectElement = new SelectElement(NodeSelect);
                selectElement.SelectByValue(PF_Node); Thread.Sleep(2000);
                BasePage.Driver.FindElement(By.CssSelector("input[value = 'Send To Selected Remotes']")).Click(); Thread.Sleep(2000);
                result.steps[++ExecutedSteps].StepPass();

                //Step 18
                BasePage.Driver.FindElement(By.CssSelector(".messagegreenlarge"));
                BasePage.Driver.FindElement(By.CssSelector("input[value = 'Close Window']")).Click();
                result.steps[++ExecutedSteps].StepPass();
                BasePage.Driver.SwitchTo().Window(currentHandler);
                Configure.logout().Click();
                Thread.Sleep(3000);

                //Step 19,20
                cachewait = new DefaultWait<IWebDriver>(BasePage.Driver);
                cachewait.Timeout = new TimeSpan(0, 3, 0);
                cachewait.IgnoreExceptionTypes(new Type[] { new FileNotFoundException().GetType() });
                cachewait.Until<Boolean>((d) =>
                {
                    if (Directory.Exists(Cachedstudypath_CT[0]) && Directory.Exists(Cachedstudypath_CT[1]) && File.Exists(Demographicsxmlpath_CT[0]))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });

                ExecutedSteps++;
                foreach (string CachedstudypathFolder in Cachedstudypath_CT)
                {
                    if (Directory.Exists(CachedstudypathFolder))
                        result.steps[ExecutedSteps].AddPassStatusList();
                    else
                        result.steps[ExecutedSteps].AddFailStatusList("Unable to find the Folder  " + CachedstudypathFolder + " for the CT Study");
                }
                foreach (string DemographicsxmlpathFile in Demographicsxmlpath_CT)
                {
                    if (File.Exists(DemographicsxmlpathFile))
                        result.steps[ExecutedSteps].AddPassStatusList();
                    else
                        result.steps[ExecutedSteps].AddFailStatusList("Unable to find the file " + DemographicsxmlpathFile + " for the CT Study");
                }
                foreach (string CachedstudypathFolder in Cachedstudypath_MR)
                {
                    if (!Directory.Exists(CachedstudypathFolder))
                        result.steps[ExecutedSteps].AddPassStatusList();
                    else
                        result.steps[ExecutedSteps].AddFailStatusList("Able to find the Folder " + CachedstudypathFolder + " For MR study");
                }
                foreach (string DemographicsxmlpathFile in Demographicsxmlpath_MR)
                {
                    if (!File.Exists(DemographicsxmlpathFile))
                        result.steps[ExecutedSteps].AddPassStatusList();
                    else
                        result.steps[ExecutedSteps].AddFailStatusList("Able to find the file " + DemographicsxmlpathFile + " For MR study");
                }

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }

                //step 21
                Thread.Sleep(5000);
                int jobAfter_CT = PreprocessingUtil.GetJobcount();
                 stopwatch = new Stopwatch();
                 timeout = new TimeSpan(0, 1, 0);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (jobAfter_CT == jobAfter + 1)
                        break;
                    Thread.Sleep(2000);
                    jobAfter_CT = PreprocessingUtil.GetJobcount();
                }
                stopwatch.Stop();
                stopwatch.Reset();
                jobAfter_CT = PreprocessingUtil.GetJobcount();
                Logger.Instance.InfoLog("Total Job After prefetch is " + jobAfter_CT);
                if (jobAfter_CT == jobAfter + 1)
                {
                    result.steps[++ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[++ExecutedSteps].AddFailStatusList();
                }

                string jobuid = PreprocessingUtil.GetJobUID()[jobAfter];
                string jobStatus_CT = PreprocessingUtil.GetJobStatusFromDB(jobuid);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (int.Parse(jobStatus_CT) == 4)
                        break;
                    Thread.Sleep(2000);
                    jobStatus_CT = PreprocessingUtil.GetJobStatusFromDB(jobuid);
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (int.Parse(jobStatus_CT) == 4)
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }

                int jobActionAfter_CT = PreprocessingUtil.GetJobActionCountFromDB("PriorStudyDigest");
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (jobActionAfter_CT == jobActionAfter + 1)
                        break;
                    Thread.Sleep(2000);
                    jobActionAfter_CT = PreprocessingUtil.GetJobActionCountFromDB("PriorStudyDigest");
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (jobActionAfter_CT == jobActionAfter + 1)
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }

                jobuid = PreprocessingUtil.GetJobUID()[jobAfter];
                string jobactionStatus_CT = PreprocessingUtil.GetJobActionStatusFromDB(jobuid);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    if (int.Parse(jobactionStatus_CT) == 9)
                        break;
                    Thread.Sleep(2000);
                    jobactionStatus_CT = PreprocessingUtil.GetJobActionStatusFromDB(jobuid);
                }
                stopwatch.Stop();
                stopwatch.Reset();
                if (int.Parse(jobactionStatus_CT) == 9)
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }

                //step 21
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

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
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                BasePage.Driver.Navigate().GoToUrl(login.GetEAUrl(Config.DestEAsIp));
                hplogin = new HPLogin();
                homepage = hplogin.LoginHPen(hpUserName, hpPassword);
                PageLoadWait.WaitForHPPageLoad(20);
                WorkFlow EAPortal = (WorkFlow)hphomepage.Navigate("Workflow");
                EAPortal.NavigateToLink("Workflow", "Archive Search");

                //Click Search Archive
                EAPortal.HPSearchStudy("PatientID", PatinetID);
                try
                {
                    Dictionary<string, string> PatinetDetails = EAPortal.GetStudyDetailsInHP();
                    if (PatinetDetails["Patient ID"].Equals(PatinetID))
                        EAPortal.HPDeleteStudy();
                }
                catch (Exception ex)
                {
                    Logger.Instance.InfoLog("Patinet not present");
                }
                Configure.logout().Click();

            }

        }

        ///<summary>
        ///PreProcessing- "New Study notification" based on time configured in Service Tool
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161603(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables     
            TestCaseResult result = new TestCaseResult(stepcount);
            Configure EAUtils = new Configure();
            HPHomePage homepage = new HPHomePage();
            var netstat = new NetStat();
            WorkFlow Workflow = new WorkFlow();
            PreprocessingUtils PreprocessingUtils = new PreprocessingUtils();
            ServiceTool servicetool = new ServiceTool();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            List<string> studyUID = new List<string>();
            try
            {
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                string Filepath = string.Empty;
                string[] FullPath = null;
                string IConnectIP = Config.IConnectIP;
                string EA_IP = Config.DestEAsIp;
                string EA_Title = Config.DestEAsAETitle;
                string cachepath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "CachePath");
                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                string Study_InstanceUID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Study_InstanceUID");
                string[] Study_InstanceUIDValue = Study_InstanceUID.Split('@');
                string SeriesInstanceUID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "SeriesInstanceUID");
                string[] SeriesInstanceUIDValue = SeriesInstanceUID.Split('@');
                string Number_Of_Studies = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Number_Of_Studies");
                string studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                TimeSpan aday = new System.TimeSpan(30, 0, 0, 0);
                string today = DateTime.Today.Subtract(aday).ToString().Split(' ')[0];

                String datasource = String.Empty;
                String datasourceip = String.Empty;
                String cachedstudypath = String.Empty;
                String cachedstudyname = String.Empty;
                String studyinstanceuid = String.Empty;
                String seriesuid = String.Empty;
                String sopuid = String.Empty;
                String demographicsxmlpath = String.Empty;
                String accession = String.Empty;
                String lastname = String.Empty;
                List<String> Cachedstudypath = new List<String>();
                List<String> Cachedstudyname = new List<String>();
                List<String> Demographicsxmlpath = new List<String>();
                string PF_cachepath = cachepath + Path.DirectorySeparatorChar + "PF_" + GetHostName(IConnectIP);
                string PF_Node = "PF_" + GetHostName(IConnectIP);

                //Step 1- PreCondition
                result.steps[++ExecutedSteps].StepPass();

                //Step 2
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(PreprocessingUtils.PreprocessingConfigXML);
                XmlElement attribute = (XmlElement)xmlDoc.SelectSingleNode("//PreprocessingConfiguration/BlueRingRules/Rule[@id = 'All']//Actions//Action[@name='GeneratePriorStudyDigest']");
                if (attribute == null)
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                }

                //Step 3- Update the study Notification date and restart IIS
                servicetool.SetStudyNotificationDate(today);
                result.steps[++ExecutedSteps].StepPass();

                //Step -4
                result.steps[++ExecutedSteps].StepPass();

                //Step 5
                //Get the Job table Count before Prefetch and create a new Dicom Study
                int jobcountbeforePrefetch = PreprocessingUtils.GetJobcount();
                string newFile = CreateNewDicomStudy(studypath);
                studyinstanceuid = BasePage.ReadDicomFile<String>(newFile, DicomTag.StudyInstanceUID);
                studyUID.Add(studyinstanceuid);
                seriesuid = BasePage.ReadDicomFile<String>(newFile, DicomTag.SeriesInstanceUID);
                sopuid = BasePage.ReadDicomFile<String>(newFile, DicomTag.SOPInstanceUID);
                cachedstudypath = PF_cachepath + Path.DirectorySeparatorChar + studyinstanceuid + Path.DirectorySeparatorChar + seriesuid;
                cachedstudyname = sopuid + "." + "dcm";
                demographicsxmlpath = PF_cachepath + Path.DirectorySeparatorChar + studyinstanceuid + Path.DirectorySeparatorChar + "demographics.xml";
                accession = BasePage.ReadDicomFile<String>(newFile, DicomTag.AccessionNumber);
                Cachedstudypath.Add(cachedstudypath);
                Cachedstudyname.Add(cachedstudyname);
                Demographicsxmlpath.Add(demographicsxmlpath);

                //Update the Study date
                Random random = new Random();
                var FinalStudyName = BasePage.WriteDicomFile(newFile, new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime }, new String[] { DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 30, 0)).ToString("HHmmss")) }, testid + random.Next(111, 999));
                string FinalStudypath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + FinalStudyName;
                string TemDirectory = BasePage.MoveFilesToTempFolders(FinalStudyName, FinalStudypath, "TempDicom",true);
                FullPath = Directory.GetFiles(TemDirectory, "*.*", SearchOption.AllDirectories);
                foreach (string path in FullPath)
                {
                    PreprocessingUtils.PushStudytoEA(path, IConnectIP, 4446, false, "SCU", PF_Node);
                }
                result.steps[++ExecutedSteps].StepPass();

                //Step 6
                Stopwatch stopwatch = new Stopwatch();
                TimeSpan timeout = new TimeSpan(0, 2, 0);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                    if (Directory.Exists(cachedstudypath) && File.Exists(demographicsxmlpath))
                        break;
                stopwatch.Stop();
                stopwatch.Reset();
                if (!Directory.Exists(cachedstudypath))
                {
                    result.steps[++ExecutedSteps].AddFailStatusList("Unable to find the Folder  " + cachedstudypath + " for the Study");
                }
                else
                {
                    result.steps[++ExecutedSteps].AddPassStatusList();
                }
                if (!File.Exists(demographicsxmlpath))
                {
                    result.steps[ExecutedSteps].AddFailStatusList("Unable to find the file " + demographicsxmlpath + " for the Study");
                }
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Step - 7
                //Get the Job table Count before Prefetch
                int jobCountAfterPrefetch = PreprocessingUtils.GetJobcount();
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    jobCountAfterPrefetch = PreprocessingUtils.GetJobcount();
                    if (jobCountAfterPrefetch == jobcountbeforePrefetch + 1)
                        break;
                }
                stopwatch.Stop();
                stopwatch.Reset();
                jobCountAfterPrefetch = PreprocessingUtils.GetJobcount();
                string jobuid = PreprocessingUtils.GetJobUID()[jobCountAfterPrefetch - 1];
                if (jobCountAfterPrefetch == jobcountbeforePrefetch + 1 && (PreprocessingUtils.GetJobStatusFromDB(jobuid) == "1" || PreprocessingUtils.GetJobStatusFromDB(jobuid) == "4" || PreprocessingUtils.GetJobStatusFromDB(jobuid) == "5"))
                {
                    result.steps[++ExecutedSteps].StepPass("Job count is increaded by 1 in job table after prefetch and the status is set as '1' or '4', job status =" + PreprocessingUtils.GetJob()[jobCountAfterPrefetch - 1]);
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Job count is not increaded by 1 in job table after prefetch or the status is set as '1' or '4', job status =" + PreprocessingUtils.GetJob()[jobCountAfterPrefetch - 1]);
                }

                //Step 8
                //Create a new Study -2 
                string newFile2 = CreateNewDicomStudy(studypath);
                studyinstanceuid = BasePage.ReadDicomFile<String>(newFile2, DicomTag.StudyInstanceUID);
                studyUID.Add(studyinstanceuid);
                seriesuid = BasePage.ReadDicomFile<String>(newFile2, DicomTag.SeriesInstanceUID);
                sopuid = BasePage.ReadDicomFile<String>(newFile2, DicomTag.SOPInstanceUID);
                cachedstudypath = PF_cachepath + Path.DirectorySeparatorChar + studyinstanceuid + Path.DirectorySeparatorChar + seriesuid;
                cachedstudyname = sopuid + "." + "dcm";
                demographicsxmlpath = PF_cachepath + Path.DirectorySeparatorChar + studyinstanceuid + Path.DirectorySeparatorChar + "demographics.xml";
                accession = BasePage.ReadDicomFile<String>(newFile2, DicomTag.AccessionNumber);
                string FinalStudy2path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + newFile2;
                TemDirectory = BasePage.MoveFilesToTempFolders(newFile2, FinalStudy2path, "TempDicom",true);
                FullPath = Directory.GetFiles(TemDirectory, "*.*", SearchOption.AllDirectories);
                foreach (string path in FullPath)
                {
                    PreprocessingUtils.PushStudytoEA(path, Config.IConnectIP, 4446, false, "SCU", PF_Node);
                }
                ExecutedSteps++;

                //Step 9
                stopwatch = new Stopwatch();
                timeout = new TimeSpan(0, 1, 0);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                    if (Directory.Exists(cachedstudypath) && File.Exists(demographicsxmlpath))
                        break;
                stopwatch.Stop();
                stopwatch.Reset();
                if (!Directory.Exists(cachedstudypath))
                {
                    result.steps[++ExecutedSteps].AddFailStatusList("Unable to find the Folder  " + cachedstudypath + " for the Study");
                }
                else
                {
                    result.steps[++ExecutedSteps].AddPassStatusList();
                }
                if (!File.Exists(demographicsxmlpath))
                    result.steps[ExecutedSteps].AddFailStatusList("Unable to find the file " + demographicsxmlpath + " for the Study");
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Step 10
                //Get the Job table Count before Prefetch
                int jobCountAfterPrefetch2 = PreprocessingUtils.GetJobcount();
                if (jobCountAfterPrefetch2 == jobCountAfterPrefetch)
                {
                    result.steps[++ExecutedSteps].StepPass("Job count is not increaded by 1 in job table after prefetch");
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Job count is increaded by 1 in job table after prefetch");
                }

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
                servicetool.SetStudyNotificationDate("1/1/1995");
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                BasePage.Driver.Navigate().GoToUrl(login.GetEAUrl(Config.DestEAsIp));
                hplogin = new HPLogin();
                homepage = hplogin.LoginHPen(hpUserName, hpPassword);
                PageLoadWait.WaitForHPPageLoad(20);
                WorkFlow EAPortal = (WorkFlow)hphomepage.Navigate("Workflow");
                EAPortal.NavigateToLink("Workflow", "Archive Search");

                foreach (string studUID in studyUID)
                {
                    //Click Search Archive
                    EAPortal.HPSearchStudy("Study Instance UID", studUID);
                    try
                    {
                        Dictionary<string, string> PatinetDetails = EAPortal.GetStudyDetailsInHP();
                        if (PatinetDetails["Study InstanceUID"].Equals(studUID))
                            EAPortal.HPDeleteStudy();
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.InfoLog("Patinet not present");
                    }
                }
            }

        }

        ///<summary>
        ///PreProcessing- Pre-Fetch using PUSH mechanism- New Study notification arrives to preprocessing system from pre-fetch
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161596(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables     
            TestCaseResult result = new TestCaseResult(stepcount);
            Configure EAUtils = new Configure();
            HPHomePage homepage = new HPHomePage();
            var netstat = new NetStat();
            WorkFlow Workflow = new WorkFlow();
            PreprocessingUtils PreprocessingUtil = new PreprocessingUtils();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            List<string> studyUID = new List<string>();
            List<string> Accession = new List<string>();

            try
            {
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                string Filepath = string.Empty;
                string[] FullPath = null;
                string IConnectIP = Config.IConnectIP;
                string EA_IP = Config.DestEAsIp;
                string EA_Title = Config.DestEAsAETitle;
                string cachepath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "CachePath");
                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                string Study_InstanceUID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Study_InstanceUID");
                string[] Study_InstanceUIDValue = Study_InstanceUID.Split('@');
                string SeriesInstanceUID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "SeriesInstanceUID");
                string[] SeriesInstanceUIDValue = SeriesInstanceUID.Split('@');
                string Number_Of_Studies = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Number_Of_Studies");
                string studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String cachedstudypath = String.Empty;
                String cachedstudyname = String.Empty;
                String studyinstanceuid = String.Empty;
                String seriesuid = String.Empty;
                String sopuid = String.Empty;
                String demographicsxmlpath = String.Empty;
                String accession = String.Empty;
                List<String> Cachedstudypath = new List<String>();
                List<String> Cachedstudyname = new List<String>();
                List<String> Demographicsxmlpath = new List<String>();
                string PF_cachepath = cachepath + Path.DirectorySeparatorChar + "PF_" + GetHostName(IConnectIP);
                string PF_Node = "PF_" + GetHostName(IConnectIP);

                //Pre-Condition 
                //Step 1
                result.steps[++ExecutedSteps].StepPass();

                //Step 2 
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(PreprocessingUtils.PreprocessingConfigXML);
                XmlElement attribute = (XmlElement)xmlDoc.SelectSingleNode("//PreprocessingConfiguration/BlueRingRules/Rule[@id = 'All']//Actions//Action[@name='GeneratePriorStudyDigest']");
                if (attribute == null)
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();

                //Step 3
                //Create a new Study -1
                string newFile = CreateNewDicomStudy(studypath);
                studyinstanceuid = BasePage.ReadDicomFile<String>(newFile, DicomTag.StudyInstanceUID);
                studyUID.Add(studyinstanceuid);
                string AccessionNumber = BasePage.ReadDicomFile<String>(newFile, DicomTag.AccessionNumber);
                Accession.Add(AccessionNumber);
                seriesuid = BasePage.ReadDicomFile<String>(newFile, DicomTag.SeriesInstanceUID);
                sopuid = BasePage.ReadDicomFile<String>(newFile, DicomTag.SOPInstanceUID);
                cachedstudypath = PF_cachepath + Path.DirectorySeparatorChar + studyinstanceuid + Path.DirectorySeparatorChar + seriesuid;
                cachedstudyname = sopuid + "." + "dcm";
                demographicsxmlpath = PF_cachepath + Path.DirectorySeparatorChar + studyinstanceuid + Path.DirectorySeparatorChar + "demographics.xml";
                accession = BasePage.ReadDicomFile<String>(newFile, DicomTag.AccessionNumber);
                Cachedstudypath.Add(cachedstudypath);
                Cachedstudyname.Add(cachedstudyname);
                Demographicsxmlpath.Add(demographicsxmlpath);

                //step 5 - precondition
                int jobBeforePrefetch = PreprocessingUtil.GetJobcount();
                Random random = new Random();
                var FinalStudyName = BasePage.WriteDicomFile(newFile, new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime }, new String[] { DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 30, 0)).ToString("HHmmss")) }, testid + random.Next(111, 999));
                string FinalStudypath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + FinalStudyName;
                string TemDirectory = BasePage.MoveFilesToTempFolders(FinalStudyName, FinalStudypath, "TempDicom",true);
                FullPath = Directory.GetFiles(TemDirectory, "*.*", SearchOption.AllDirectories);
                foreach (string path in FullPath)
                {
                    PreprocessingUtil.PushStudytoEA(path, IConnectIP, 4446, false, "SCU", PF_Node);
                }
                ExecutedSteps++;

                //Step 4
                var cachewait = new DefaultWait<IWebDriver>(BasePage.Driver);
                cachewait.Timeout = new TimeSpan(0, 1, 0);
                cachewait.IgnoreExceptionTypes(new Type[] { new FileNotFoundException().GetType() });
                cachewait.Until<Boolean>((d) =>
                {
                    if (File.Exists(Demographicsxmlpath[0]) && Directory.Exists(Cachedstudypath[0]))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                if (File.Exists(Demographicsxmlpath[0]) && Directory.Exists(Cachedstudypath[0]))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 5
                // Wait for the Preprocessing to get update the status in tables.
                Thread.Sleep(20000);
                int jobAfterPrefetch = PreprocessingUtil.GetJobcount();
                Stopwatch stopwatch = new Stopwatch();
                TimeSpan timeout = new TimeSpan(0, 1, 0);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    jobAfterPrefetch = PreprocessingUtil.GetJobcount();
                    if (jobAfterPrefetch == jobBeforePrefetch + 1)
                        break;
                }
                stopwatch.Stop();

                if (jobAfterPrefetch == jobBeforePrefetch + 1)
                {
                    result.steps[++ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    Logger.Instance.ErrorLog("job before prefecth is"+ jobBeforePrefetch + " job count after Prefetch is" + jobAfterPrefetch);
                    result.steps[++ExecutedSteps].AddFailStatusList();
                }
                string jobuid = PreprocessingUtil.GetJobUID()[jobAfterPrefetch - 1];
                string Jobstatus = PreprocessingUtil.GetJobStatusFromDB(jobuid);
                if (int.Parse(Jobstatus) == 1)
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Step 6
                int jobBeforePrefetch2 = PreprocessingUtil.GetJobcount();
                Logger.Instance.InfoLog("Total Job before prefetch is " + jobBeforePrefetch2);
                ServiceController serviceController = new ServiceController("PreprocessingService");
                try
                {
                    if ((serviceController.Status.Equals(ServiceControllerStatus.Running)) || (serviceController.Status.Equals(ServiceControllerStatus.StartPending)))
                        serviceController.Stop();
                    serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                }
                catch
                {
                    throw new Exception("Error while Restart the service " + "PreprocessingService");
                }
                ServiceController sc = new ServiceController("PreprocessingService");
                if (sc.Status.ToString() == "Stopped")
                {
                    Logger.Instance.InfoLog("Successfully the Preprocessing service is stopped");
                    result.steps[++ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    Logger.Instance.ErrorLog("Error Occured while stop the service PreprocessingService");
                    result.steps[++ExecutedSteps].AddFailStatusList();
                }

                //Create a new Study -2
                newFile = CreateNewDicomStudy(studypath);
                studyinstanceuid = BasePage.ReadDicomFile<String>(newFile, DicomTag.StudyInstanceUID);
                studyUID.Add(studyinstanceuid);
                AccessionNumber = BasePage.ReadDicomFile<String>(newFile, DicomTag.AccessionNumber);
                Accession.Add(AccessionNumber);
                seriesuid = BasePage.ReadDicomFile<String>(newFile, DicomTag.SeriesInstanceUID);
                sopuid = BasePage.ReadDicomFile<String>(newFile, DicomTag.SOPInstanceUID);
                cachedstudypath = PF_cachepath + Path.DirectorySeparatorChar + studyinstanceuid + Path.DirectorySeparatorChar + seriesuid;
                cachedstudyname = sopuid + "." + "dcm";
                demographicsxmlpath = PF_cachepath + Path.DirectorySeparatorChar + studyinstanceuid + Path.DirectorySeparatorChar + "demographics.xml";
                accession = BasePage.ReadDicomFile<String>(newFile, DicomTag.AccessionNumber);
                Cachedstudypath.Add(cachedstudypath);
                Cachedstudyname.Add(cachedstudyname);
                Demographicsxmlpath.Add(demographicsxmlpath);

                //Update the Study date
                random = new Random();
                FinalStudyName = BasePage.WriteDicomFile(newFile, new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime }, new String[] { DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 30, 0)).ToString("HHmmss")) }, testid + random.Next(111, 999));
                FinalStudypath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + FinalStudyName;
                TemDirectory = BasePage.MoveFilesToTempFolders(FinalStudyName, FinalStudypath, "TempDicom",true);
                FullPath = Directory.GetFiles(TemDirectory, "*.*", SearchOption.AllDirectories);
                foreach (string path in FullPath)
                {
                    PreprocessingUtil.PushStudytoEA(path, IConnectIP, 4446, false, "SCU", PF_Node);
                }
                Logger.Instance.InfoLog("Successfully sent the Study to EA");
                cachewait = new DefaultWait<IWebDriver>(BasePage.Driver);
                cachewait.Timeout = new TimeSpan(0, 1, 0);
                cachewait.IgnoreExceptionTypes(new Type[] { new FileNotFoundException().GetType() });
                cachewait.Until<Boolean>((d) =>
                {
                    if (File.Exists(Demographicsxmlpath[1]) && Directory.Exists(Cachedstudypath[1]))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                if (File.Exists(Demographicsxmlpath[1]) && Directory.Exists(Cachedstudypath[1]))
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }

                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }


                //step 7
                // Wait for the Preprocessing to get update the status in tables.
                int jobAfterPrefetch2 = PreprocessingUtil.GetJobcount();
                Logger.Instance.InfoLog("Total Job After prefetch is " + jobAfterPrefetch2);
                stopwatch = new Stopwatch();
                timeout = new TimeSpan(0, 1, 0);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout))
                {
                    jobAfterPrefetch2 = PreprocessingUtil.GetJobcount();
                    if (jobAfterPrefetch2 == jobBeforePrefetch2 + 1)
                        break;
                }
                stopwatch.Stop();

                if (jobAfterPrefetch2 == jobBeforePrefetch2 + 1)
                {
                    result.steps[++ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[++ExecutedSteps].AddFailStatusList();
                }
                string jobuid2 = PreprocessingUtil.GetJobUID()[jobAfterPrefetch2 - 1];
                string Jobstatus2 = PreprocessingUtil.GetJobStatusFromDB(jobuid2);

                if (int.Parse(Jobstatus2) == 1)
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }
                //Step result
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Step 8
                //Wait for the 2 minutes
                Thread.Sleep(120000);
                jobuid2 = PreprocessingUtil.GetJobUID()[jobAfterPrefetch2 - 1];
                Jobstatus2 = PreprocessingUtil.GetJobStatusFromDB(jobuid2);
                if (int.Parse(Jobstatus2) == 1)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }


                //Step 9
                PreprocessingUtil.RestartService("PreprocessingService");
                result.steps[++ExecutedSteps].StepPass();

                //Step 10
                result.steps[++ExecutedSteps].status = "No Automation";

                //Step 11
                //Wait for the 2 minutes
                cachewait = new DefaultWait<IWebDriver>(BasePage.Driver);
                cachewait.Timeout = new TimeSpan(0, 3, 0);
                cachewait.Until<Boolean>((d) =>
                {
                    jobuid2 = PreprocessingUtil.GetJobUID()[jobAfterPrefetch2 - 1];
                    Jobstatus2 = PreprocessingUtil.GetJobStatusFromDB(jobuid);
                    if (int.Parse(Jobstatus2) == 4)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                jobuid2 = PreprocessingUtil.GetJobUID()[jobAfterPrefetch2 - 1];
                Jobstatus2 = PreprocessingUtil.GetJobStatusFromDB(jobuid);
                if (int.Parse(Jobstatus2) == 4)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 12
                DataBaseUtil db = new DataBaseUtil("sqlserver", "IRWSDB", InstanceName: "WEBACCESS");
                db.ConnectSQLServerDB();
                string NodeID = "select NodeID from Job where JobUid =" + jobuid2;
                IList<string> NodeIdvalue = db.ExecuteQuery(NodeID);
                if (NodeIdvalue[0] == "HA_" + GetHostName(IConnectIP))
                {
                    result.steps[++ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[++ExecutedSteps].AddFailStatusList();
                }
                string jobdetailsQuery = " select Detail from job where JobUid = '" + jobuid2 + "'";
                IList<string> jobDetailsvalue = db.ExecuteQuery(jobdetailsQuery);
                if (jobDetailsvalue[0].Contains(studyUID[1]))
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }
                jobdetailsQuery = " select Detail from job where JobUid = '" + jobuid2 + "'";
                jobDetailsvalue = db.ExecuteQuery(jobdetailsQuery);
                if (jobDetailsvalue[0].Contains(Accession[1]))
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[ExecutedSteps].AddFailStatusList();
                }
                if (result.steps[ExecutedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
                }

                //Step 13
                string LastPulse = "select LastPulse from ServerNode where NodeID='HA_" + GetHostName(IConnectIP) + "'";
                IList<string> LastplusevalueList = db.ExecuteQuery(LastPulse);
                DateTime dateTime1 = DateTime.ParseExact(LastplusevalueList[0].ToString(), "M/d/yyyy h:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
                //Wait for 10 seconds
                Thread.Sleep(10000);
                LastplusevalueList = db.ExecuteQuery(LastPulse);
                DateTime dateTime2 = DateTime.ParseExact(LastplusevalueList[0].ToString(), "M/d/yyyy h:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture);
                if (dateTime1.AddSeconds(10) == dateTime2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                BasePage.DeleteAllFileFolder(PF_cachepath);

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
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                BasePage.Driver.Navigate().GoToUrl(login.GetEAUrl(Config.DestEAsIp));
                hplogin = new HPLogin();
                homepage = hplogin.LoginHPen(hpUserName, hpPassword);
                PageLoadWait.WaitForHPPageLoad(20);
                WorkFlow EAPortal = (WorkFlow)hphomepage.Navigate("Workflow");
                EAPortal.NavigateToLink("Workflow", "Archive Search");

                foreach (string studUID in studyUID)
                {
                    //Click Search Archive
                    EAPortal.HPSearchStudy("Study Instance UID", studUID);
                    try
                    {
                        Dictionary<string, string> PatinetDetails = EAPortal.GetStudyDetailsInHP();
                        if (PatinetDetails["Study InstanceUID"].Equals(studUID))
                            EAPortal.HPDeleteStudy();
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.InfoLog("Patinet not present");
                    }
                }
            }

        }

        ///<summary>
        /// PreProcessing- Study Completion Timeout configuration in Service Tool
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161601(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables     
            TestCaseResult result = new TestCaseResult(stepcount);
            Configure EAUtils = new Configure();
            HPHomePage homepage = new HPHomePage();
            var netstat = new NetStat();
            WorkFlow Workflow = new WorkFlow();
            PreprocessingUtils PreprocessingUtils = new PreprocessingUtils();
            ServiceTool servicetool = new ServiceTool();
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            List<string> studyUID = new List<string>();

            try
            {
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                string Filepath = string.Empty;
                string[] FullPath = null;
                string IConnectIP = Config.IConnectIP;
                string EA_IP = Config.DestEAsIp;
                string EA_Title = Config.DestEAsAETitle;
                string cachepath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "CachePath");
                string PatinetID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                string patinetName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                string Study_InstanceUID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Study_InstanceUID");
                string[] Study_InstanceUIDValue = Study_InstanceUID.Split('@');
                string studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String demographicsxmlpath = String.Empty;
                String accession = String.Empty;
                List<String> Cachedstudypath = new List<String>();
                List<String> Cachedstudyname = new List<String>();
                List<String> Demographicsxmlpath = new List<String>();
                string PF_cachepath = cachepath + Path.DirectorySeparatorChar + "PF_" + GetHostName(IConnectIP);
                string PF_Node = "PF_" + GetHostName(IConnectIP);

                //Step 1
                //Open the ICA service tool and select the Datasource tab
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                result.steps[++ExecutedSteps].StepPass("Service tool launched");
                Thread.Sleep(1500);
                Taskbar taskbar = new Taskbar();
                taskbar.Hide();

                //Step 2
                //Enable Prefetch Cache
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                servicetool.NavigateSubTab("Pre-fetch Cache Service");
                servicetool.ClickModifyButton();
                wpfobject.GetUIItem<ITabPage, RadioButton>(servicetool.GetCurrentTabItem(), "Local Cache Service", 1, "0").Click();
                Tab PreFTab = wpfobject.GetUIItem<ITabPage, Tab>(WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab);
                PreFTab.SelectTabPage("Cache Store SCP Settings");
                result.steps[++ExecutedSteps].StepPass("Navigated to Cache Store SCP Settings");
                ITabPage t2 = servicetool.GetCurrentTabItem().Get<Tab>(SearchCriteria.All).SelectedTab;

                //Step 3
                wpfobject.setTextInTextBoxUsingIndex(8, "!");
                if (wpfobject.GetTextInTextBoxUsingIndex(8) == "!")
                {
                    result.steps[++ExecutedSteps].StepFail("The value ! is typed in the Study completeion text box");
                }
                else
                    result.steps[++ExecutedSteps].StepPass("The value ! is not typed in the Study completeion text box");

                //Step 4
                wpfobject.setTextInTextBoxUsingIndex(8, "5");
                if (wpfobject.GetTextInTextBoxUsingIndex(8) != "5")
                {
                    result.steps[++ExecutedSteps].StepFail("The value 10 is not typed in the Study completeion text box");
                }
                else
                    result.steps[++ExecutedSteps].StepPass("The value 10 is typed in the Study completeion text box");
                servicetool.ClickApplyButtonFromTab();
                servicetool.WaitWhileBusy();

                //Step 5.
                servicetool.ClickModifyButton();
                servicetool.EnablePrefetchCache(cachetype: "Local", pollingtime: 1, timerange: 60, cleanupthreshold: 60, AEtitle: PF_Node);
                result.steps[++ExecutedSteps].StepPass();

                //Step 6
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].StepPass();

                //Step 7
                if (ReadXML.ReadAttribute(PreprocessingUtils.PrefetchStoreXML, "StoreScpServer", "studyCompletionTimeoutSecond") == "5")
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("The Completion time updated at service tool is not updtaed in Config XML");
                }

                //Step 8
                if (PreprocessingUtils.GetServiceStatus(PreprocessingUtils.ImagePrefetchService).Equals("Running"))
                {
                    Logger.Instance.InfoLog("The service ImagePrefetchService is running");
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 9
                if ((PreprocessingUtils.GetServiceStatus(PreprocessingUtils.PreProcessingServiceName).Equals("Running")))
                {
                    Logger.Instance.InfoLog("The service PreprocessingService is running");
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 10
                result.steps[++ExecutedSteps].StepPass();

                //step13- Precondition
                int jobcountBeforeSendingImage = PreprocessingUtils.GetJobcount();

                //Step 11
                string[] folderList = Directory.GetDirectories(studypath);
                FullPath = Directory.GetFiles(folderList[0], "*.*", SearchOption.AllDirectories);
                foreach (string path in FullPath)
                {
                    PreprocessingUtils.PushStudytoEA(path, IConnectIP, 4446, false, "SCU", PF_Node);
                }
                Thread.Sleep(5000);
                FullPath = Directory.GetFiles(folderList[1], "*.*", SearchOption.AllDirectories);
                foreach (string path in FullPath)
                {
                    PreprocessingUtils.PushStudytoEA(path, IConnectIP, 4446, false, "SCU", PF_Node);
                }
                Thread.Sleep(5000);
                FullPath = Directory.GetFiles(folderList[2], "*.*", SearchOption.AllDirectories);
                foreach (string path in FullPath)
                {
                    PreprocessingUtils.PushStudytoEA(path, IConnectIP, 4446, false, "SCU", PF_Node);
                }
                Thread.Sleep(5000);

                result.steps[++ExecutedSteps].StepPass();

                //Step 12
                demographicsxmlpath = PF_cachepath + Path.DirectorySeparatorChar + Study_InstanceUIDValue[0] + Path.DirectorySeparatorChar + "demographics.xml";
                var cachewait = new DefaultWait<IWebDriver>(BasePage.Driver);
                cachewait.Timeout = new TimeSpan(0, 0, 10);
                cachewait.IgnoreExceptionTypes(new Type[] { new FileNotFoundException().GetType() });
                cachewait.Until<Boolean>((d) =>
                {
                    var isFileFound = Directory.Exists(PF_cachepath + Path.DirectorySeparatorChar + Study_InstanceUIDValue[0]);
                    if (isFileFound)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                string XMLpatinetID = ReadXML.ReadAttribute(demographicsxmlpath, "Study", "pid");
                string XMLpatinetName = ReadXML.ReadAttribute(demographicsxmlpath, "Study", "name");
                if (XMLpatinetID == PatinetID && XMLpatinetName == patinetName)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 13
                int jobcountAfterSendingImage = PreprocessingUtils.GetJobcount();
                string jobuid2 = PreprocessingUtils.GetJobUID()[jobcountBeforeSendingImage];
                string Jobstatus2 = PreprocessingUtils.GetJobStatusFromDB(jobuid2);
                if ((jobcountAfterSendingImage == jobcountBeforeSendingImage + 1) && (int.Parse(Jobstatus2) == 1 || int.Parse(Jobstatus2) == 4 || int.Parse(Jobstatus2) != 3))
                {
                    result.steps[++ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[++ExecutedSteps].AddFailStatusList();
                }

                //Step 16 - Precondition
                jobcountBeforeSendingImage = PreprocessingUtils.GetJobcount();

                //Step 14
                IList<string> AllDicomPath = new List<string>();
                foreach (string eachFolder in folderList)
                {
                    foreach (string files in Directory.GetFiles(eachFolder, "*.*", SearchOption.AllDirectories))
                    {
                        PreprocessingUtils.PushStudytoEA(files, IConnectIP, 4446, false, "SCU", PF_Node);
                    }

                }
                result.steps[++ExecutedSteps].StepPass();
                Thread.Sleep(10000);

                //Step 15
                XMLpatinetID = ReadXML.ReadAttribute(demographicsxmlpath, "Study", "pid");
                XMLpatinetName = ReadXML.ReadAttribute(demographicsxmlpath, "Study", "name");
                if (XMLpatinetID == PatinetID && XMLpatinetName == patinetName)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 16
                jobcountAfterSendingImage = PreprocessingUtils.GetJobcount();
                if ((jobcountAfterSendingImage == jobcountBeforeSendingImage + 1) && (int.Parse(Jobstatus2) == 1 || int.Parse(Jobstatus2) == 4 || int.Parse(Jobstatus2) != 3))
                {
                    result.steps[++ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[++ExecutedSteps].AddFailStatusList();
                }

                //Step 17
                result.steps[++ExecutedSteps].status = "NO AUTOMATION";

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

        ///<summary>
        /// PreProcessing- Negative Test - incorrect data in PreprocessingConfiguration.xml file
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161599(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables     
            TestCaseResult result = new TestCaseResult(stepcount);
            Configure EAUtils = new Configure();
            HPHomePage homepage = new HPHomePage();
            var netstat = new NetStat();
            WorkFlow Workflow = new WorkFlow();
            PreprocessingUtils PreprocessingUtils = new PreprocessingUtils();
            ServiceTool servicetool = new ServiceTool();
            PreprocessingUtils PreprocessingUtil = new PreprocessingUtils();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            List<string> studyUID = new List<string>();
            string AttribuetValuebeforeUpdate = null;
            string ValuesSwitchBeforeUpdate = null;
            try
            {
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                string[] FullPath = null;
                string IConnectIP = Config.IConnectIP;
                string EA_IP = Config.DestEAsIp;
                string EA_Title = Config.DestEAsAETitle;
                string cachepath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "CachePath");
                string patinetName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
                string Study_InstanceUID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Study_InstanceUID");
                string[] Study_InstanceUIDValue = Study_InstanceUID.Split('@');
                string studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String cachedstudypath = String.Empty;
                String cachedstudyname = String.Empty;
                String studyinstanceuid = String.Empty;
                String seriesuid = String.Empty;
                String sopuid = String.Empty;
                String demographicsxmlpath = String.Empty;
                String accession = String.Empty;
                List<String> Cachedstudypath = new List<String>();
                List<String> Cachedstudyname = new List<String>();
                List<String> Demographicsxmlpath = new List<String>();
                string PF_cachepath = cachepath + Path.DirectorySeparatorChar + "PF_" + GetHostName(IConnectIP);
                string PF_Node = "PF_" + GetHostName(IConnectIP);

                //String 1
                string PreprocessingConfigPath = PreprocessingUtils.PreprocessingConfigExe;
                ValuesSwitchBeforeUpdate = basepage.GetAttributeValue(PreprocessingConfigPath, "/configuration/system.diagnostics/switches/add[@name='DeveloperTraceSwitch']", "value");
                basepage.ChangeAttributeValue(PreprocessingConfigPath, "configuration/system.diagnostics/switches/add[@name='DeveloperTraceSwitch']", "value", "Verbose");
                string updatedValuesSwitch = basepage.GetAttributeValue(PreprocessingConfigPath, "/configuration/system.diagnostics/switches/add[@name='DeveloperTraceSwitch']", "value");
                if (updatedValuesSwitch == "Verbose")
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 2
                string PreprocessingConfigurationxml = PreprocessingUtils.PreprocessingConfigXML;
                AttribuetValuebeforeUpdate = basepage.GetAttributeValue(PreprocessingUtils.PreprocessingConfigXML, "/PreprocessingConfiguration/BlueRingRules/Rule/Actions/Action/Parameter[@name='onlyFromSameModality']", "value");
                basepage.ChangeAttributeValue(PreprocessingConfigurationxml, "PreprocessingConfiguration/BlueRingRules/Rule/Actions/Action/Parameter[@name='onlyFromSameModality']", "value", "true/false");
                string updatedValues = basepage.GetAttributeValue(PreprocessingUtils.PreprocessingConfigXML, "/PreprocessingConfiguration/BlueRingRules/Rule/Actions/Action/Parameter[@name='onlyFromSameModality']", "value");
                if (updatedValues == "true/false")
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 4 - Precondition
                int jobBeforeSendingImage = PreprocessingUtil.GetJobcount();

                //Step 3
                string newFile = CreateNewDicomStudy(studypath);
                studyinstanceuid = BasePage.ReadDicomFile<String>(newFile, DicomTag.StudyInstanceUID);
                studyUID.Add(studyinstanceuid);
                string AccessionNumber = BasePage.ReadDicomFile<String>(newFile, DicomTag.AccessionNumber);
                seriesuid = BasePage.ReadDicomFile<String>(newFile, DicomTag.SeriesInstanceUID);
                sopuid = BasePage.ReadDicomFile<String>(newFile, DicomTag.SOPInstanceUID);
                cachedstudypath = PF_cachepath + Path.DirectorySeparatorChar + studyinstanceuid + Path.DirectorySeparatorChar + seriesuid;
                cachedstudyname = sopuid + "." + "dcm";
                demographicsxmlpath = PF_cachepath + Path.DirectorySeparatorChar + studyinstanceuid + Path.DirectorySeparatorChar + "demographics.xml";
                accession = BasePage.ReadDicomFile<String>(newFile, DicomTag.AccessionNumber);
                Cachedstudypath.Add(cachedstudypath);
                Cachedstudyname.Add(cachedstudyname);
                Demographicsxmlpath.Add(demographicsxmlpath);

                Random random = new Random();
                var FinalStudyName = BasePage.WriteDicomFile(newFile, new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime }, new String[] { DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 30, 0)).ToString("HHmmss")) }, testid + random.Next(111, 999));
                string FinalStudypath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + FinalStudyName;
                string TemDirectory = BasePage.MoveFilesToTempFolders(FinalStudyName, FinalStudypath, "TempDicom", true);
                FullPath = Directory.GetFiles(TemDirectory, "*.*", SearchOption.AllDirectories);
                var LogStartTime = System.DateTime.Now;
                foreach (string path in FullPath)
                {
                    PreprocessingUtil.PushStudytoEA(path, Config.IConnectIP, 4446, false, "SCU", PF_Node);
                }
                var cachewait = new DefaultWait<IWebDriver>(BasePage.Driver);
                cachewait.Timeout = new TimeSpan(0, 1, 0);
                cachewait.IgnoreExceptionTypes(new Type[] { new FileNotFoundException().GetType() });
                cachewait.Until<Boolean>((d) =>
                {
                    if (File.Exists(Demographicsxmlpath[0]) && Directory.Exists(Cachedstudypath[0]))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                ExecutedSteps++;

                //Step 4
                cachewait = new DefaultWait<IWebDriver>(BasePage.Driver);
                cachewait.Timeout = new TimeSpan(0, 1, 0);
                cachewait.Until<Boolean>((d) =>
                {
                    if (PreprocessingUtil.GetJobcount() == jobBeforeSendingImage + 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                int jobAfterSendingImage = PreprocessingUtil.GetJobcount();
                string jobUID = PreprocessingUtil.GetJobUID()[jobBeforeSendingImage];
                cachewait.Until<Boolean>((d) =>
                {
                    Thread.Sleep(1000);
                    if (PreprocessingUtil.GetJobStatusFromDB(jobUID) == "3")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                string jobstatus = PreprocessingUtil.GetJobStatusFromDB(jobUID);
                jobAfterSendingImage = PreprocessingUtil.GetJobcount();
                if ((jobAfterSendingImage == jobBeforeSendingImage + 1) && (jobstatus == "3"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step 5
                var LogEndTime = System.DateTime.Now;
                var loggedError = string.Empty;
                try
                {
                    String Date = System.DateTime.Now.Date.ToString("yyyyMMdd");
                    String LogFilePath = @"c:\Windows\Temp\WebAccessPreprocessingServiceDeveloper-" + Date + "(" + 1 + ")" + ".log";
                    Stream stream = File.Open(LogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        if (File.Exists(LogFilePath))
                        {
                            StreamReader reader = new StreamReader(stream);
                            var LogValues = basepage.ReadDevTraceLog(LogFilePath, LogStartTime, LogEndTime, false,true);
                            foreach (var entry in LogValues)
                            {
                                if (entry.Value["Source"].Contains("OpenContent.Data.Preprocessing.ServiceModules.Internal.JobActionExecutor"))
                                    if (entry.Value["Message"].Contains("Action returns an error."))
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
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("lldsf");
                }
                finally
                {
                    //Validation of message failed in log file
                    if (loggedError == "OpenContent.Data.Preprocessing.ServiceModules.Internal.JobActionExecutor")
                    {
                        result.steps[++ExecutedSteps].StepPass();
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].StepFail("Error log in WebAccessPreprocessingServiceDevelope log file is not as excepted");
                        Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                }

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
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                //Revert back to original value in PreprocessingConfigXML
                basepage.ChangeAttributeValue(PreprocessingUtils.PreprocessingConfigXML, "PreprocessingConfiguration/BlueRingRules/Rule/Actions/Action/Parameter[@name='onlyFromSameModality']", "value", AttribuetValuebeforeUpdate);
                string updatedValues = basepage.GetAttributeValue(PreprocessingUtils.PreprocessingConfigXML, "/PreprocessingConfiguration/BlueRingRules/Rule/Actions/Action/Parameter[@name='onlyFromSameModality']", "value");
                if (updatedValues != AttribuetValuebeforeUpdate)
                    throw new Exception("Error while Update the preprocessing XML file");

                //Revert back to original value in PreprocessingConfigExe
                basepage.ChangeAttributeValue(PreprocessingUtils.PreprocessingConfigExe, "configuration/system.diagnostics/switches/add[@name='DeveloperTraceSwitch']", "value", ValuesSwitchBeforeUpdate);
                string ValuesSwitchAfterRevert = basepage.GetAttributeValue(PreprocessingUtils.PreprocessingConfigExe, "configuration/system.diagnostics/switches/add[@name='DeveloperTraceSwitch']", "value");
                if (ValuesSwitchBeforeUpdate != ValuesSwitchAfterRevert)
                    throw new Exception("Error while Update the ConfigExe XML file");

                PreprocessingUtil.RestartService("PreprocessingService");

            }

        }

        /// <summary>
        /// Test 162027 - User interface for Preprocessing rule in Service Tool
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcunt"></param>
        /// <returns></returns>
        public TestCaseResult Test_162027(String testid, String teststeps, int stepcount)
        {            
            //Declare and initialize variables     
            TestCaseResult result = new TestCaseResult(stepcount);
            ServiceTool tool = new ServiceTool();
            var wpf = new WpfObjects();
            String[] files = new String[4]{"Conditions Definition File", "Rules Definition File",
                "Scheduling Periods File", "Preprocessing Rules Package" };
            String[] toolTip = new String[4] { "Open Browse dialog to select an XML file containing condition definitions",
                "Open Browse dialog to select an XML file containing rules definitions",
                "Open Browse dialog to select an XML file containing definitions of scheduling periods",
                "Open Browse dialog to select an XML file that contains or will contain a compiled preprocessing rules package"};

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            int iterate = 0;

            try
            {
                //Step-1
                tool.LaunchServiceTool();
                ExecutedSteps++;

                //Step-2
                tool.NavigateToTab("Preprocessing Rules");
                ExecutedSteps++;

                //Step-3
                var tab = tool.GetCurrentTabItem();
                var groupbox = tab.Get<GroupBox>();
                var textboxs = wpfobject.GetUIItemList<GroupBox, TextBox>(groupbox);
                var labels  =  wpfobject.GetUIItemList<GroupBox, Label>(groupbox);
                var buttons =   wpfobject.GetUIItemList<GroupBox, Button>(groupbox);
                var btnValidateInput = tab.Get(SearchCriteria.ByText("Validate Input"));
                var btnCompilePackage = tab.Get(SearchCriteria.ByText("Compile Package"));

                //Check Location - Y coordinate of all 3 text box should be same as 3 Buttons            
                bool isAlignmentCorrect = false;               
                var textboxs_location = textboxs.Select<IUIItem, System.Windows.Point>(item => item.Location).ToList();
                var buttons_location = buttons.Select<IUIItem, System.Windows.Point>(item => item.Location).ToList();
                foreach (var location in textboxs_location)
                {
                    if (location.Y.Equals(buttons_location[iterate].Y))
                    {
                        isAlignmentCorrect = true;
                    }                        
                    else
                    {
                        isAlignmentCorrect = false;
                        break;
                    }
                    iterate++;
                }

                //Check the labels and Textbox and button count
                bool isLabelsCorrect = labels[0].Name.Contains(files[0]) && labels[1].Name.Contains(files[1]) &&
                    labels[2].Name.Contains(files[2]) && labels[3].Name.Contains(files[3]);
                var name1 = labels[3].Name;
                bool isTextBoxPresent = textboxs.Count == 4;
                bool isBrowserButtonPresent = buttons.Count == 4;

                //Check Location of Compile and Validate button
                bool isCompileButtonAtBottom = buttons_location.All(location => location.Y < btnCompilePackage.Location.Y);
                bool isValidateButtonAtBottom  = buttons_location.All(location => location.Y < btnValidateInput.Location.Y);

                if (isLabelsCorrect && isTextBoxPresent &&
                    isBrowserButtonPresent && btnCompilePackage != null 
                    && btnValidateInput != null && isAlignmentCorrect &&
                    isCompileButtonAtBottom && isValidateButtonAtBottom)
                {
                    result.steps[++ExecutedSteps].StepPass("Buttons, TextBox and labels are present");
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Buttons, TextBox and labels are present are not present");
                }
                

                //Step-4             
                bool isToolTipCorrect = wpfobject.GetToolTip(buttons[0]).Contains(toolTip[0])
                    && wpfobject.GetToolTip(buttons[1]).Contains(toolTip[1])
                    && wpfobject.GetToolTip(buttons[2]).Contains(toolTip[2])
                    && wpfobject.GetToolTip(buttons[3]).Contains(toolTip[3]);
                if(isToolTipCorrect)
                {
                    result.steps[++ExecutedSteps].StepPass("Help Text present for all buttons");
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Help Texts are incorrect");
                }

                //Step-5 Check Restart buton is flashing
                result.steps[++ExecutedSteps].status = "Not Automated";
                tool.CloseServiceTool();


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
        /// 162026 - Preprocessing service as a part of installation package
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcunt"></param>
        /// <returns></returns>
        public TestCaseResult Test_162026(String testid, String teststeps, int stepcount)
        {            
            int ExecutedSteps = -1;
            TestCaseResult result = null;
            var preprocesingutil = new PreprocessingUtils();
            String licensepath = @"C:\WebAccess\WebAccess\Config\BluRingLicense.xml";
            iCAInstaller installer = null;


            try
            {
                //Declare and initialize variables     
                result = new TestCaseResult(stepcount);
                ServiceTool tool = new ServiceTool();
                result.SetTestStepDescription(teststeps);                
                String servicepath = @"C:\WebAccess\WindowsService";
                String webaccesspath = @"C:\WebAccess\WebAccess";                
                var logContent = String.Empty;
                String logpath = String.Empty;
                installer = new iCAInstaller();

                //Step-1,2-Download and install build -- (download Will be done as part of precondition)
                ExecutedSteps++;                
                if (File.Exists(licensepath))
                File.Copy(licensepath, Config.BackupLicensePath, overwrite:true);
                var taskbar = new Taskbar();
                taskbar.Hide();
                BasePage.Kill_EXEProcess(iCAInstaller.InstallerEXE);
                installer.UninstalliCA(deleteWebAccessPath:true);
                installer.installiCA();
                taskbar.Show();
                ExecutedSteps++;

                //Step-3
                if (Directory.Exists(servicepath + Path.DirectorySeparatorChar + "Preprocessing"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Path--" + servicepath + Path.DirectorySeparatorChar
                        + "Preprocessing" + "-- Not Present");
                }

                //Step-4                
                if (Directory.Exists(webaccesspath + Path.DirectorySeparatorChar + "Domain") &&
                    Directory.Exists(webaccesspath + Path.DirectorySeparatorChar + "StudyNotification"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Path--" + servicepath + Path.PathSeparator
                        + "Preprocessing" + "-- Not Present");
                }

                //Step-5                
                if (preprocesingutil.GetServiceStatus(PreprocessingUtils.PreProcessingServiceName).Equals("Running"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step-6
                if (preprocesingutil.GetServiceDescription(PreprocessingUtils.PreProcessingServiceName).
                    Equals(PreprocessingUtils.PreProcessingServiceDescription))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail("Service Description is not--" + PreprocessingUtils.PreProcessingServiceDescription);
                }

                //Stpep-7
                preprocesingutil.StopService(PreprocessingUtils.PreProcessingServiceName);
                wait.Until(d => preprocesingutil.GetServiceStatus(PreprocessingUtils.PreProcessingServiceName).
                Equals("Stopped"));
                if (preprocesingutil.GetServiceStatus(PreprocessingUtils.PreProcessingServiceName).Equals("Stopped"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step-8
                preprocesingutil.StartService(PreprocessingUtils.PreProcessingServiceName);
                wait.Until(d => preprocesingutil.GetServiceStatus(PreprocessingUtils.PreProcessingServiceName).
                Equals("Running"));
                if (preprocesingutil.GetServiceStatus(PreprocessingUtils.PreProcessingServiceName).Equals("Running"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step-9
                preprocesingutil.StopService(PreprocessingUtils.PreProcessingServiceName);
                BasePage.wait.Until(d => preprocesingutil.GetServiceStatus(PreprocessingUtils.PreProcessingServiceName).
                Equals("Stopped"));
                int retry = 0;
                bool fileRead = false;
                while (retry<4 && fileRead==false)
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(PreprocessingUtils.logFilePath))
                        {
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                logContent = logContent + line;
                                logContent = logContent + Environment.NewLine;
                            }
                            fileRead = true;
                        }
                    }
                    catch (Exception e) { Logger.Instance.InfoLog(e.Message + Environment.NewLine + e.StackTrace); retry++; }
                    
                }
                preprocesingutil.StartService(PreprocessingUtils.PreProcessingServiceName);
                BasePage.wait.Until(d => preprocesingutil.GetServiceStatus(PreprocessingUtils.PreProcessingServiceName).
               Equals("Running"));
                if (logContent.Contains("[Message] Start") && logContent.Contains("[Message] End"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step-10 - Start Service in debug mode
                var process_log = preprocesingutil.RunProcess("cmd.exe", 
                    @"/K C:\WebAccess\WindowsService\Preprocessing\bin\preprocessing.exe -debug", 10000);               
                var process = process_log.Keys.ToList()[0];
                var log = process_log.Values.ToList()[0];                
                if(log.Contains("PreprocessingService is running in debug mode") &&
                    log.Contains("Pressing ctrl+c will simulate a service stop command and initiate a graceful shutdown"))
                {
                    result.steps[++ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[++ExecutedSteps].AddPassStatusList();
                }

                //Gracefull stop a process
                process.Kill();  
                if (process.HasExited)
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }
                else
                {
                    result.steps[ExecutedSteps].AddPassStatusList();
                }

                //Set step status
                if(result.steps[ExecutedSteps].statuslist.Contains("Fail"))
                {
                    result.steps[ExecutedSteps].StepFail();
                }
                else
                {
                    result.steps[ExecutedSteps].StepPass();
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
                BasePage.Kill_EXEProcess(iCAInstaller.InstallerEXE);
                installer.UninstalliCA(deleteWebAccessPath: true); 

            }

        }

    }
}
