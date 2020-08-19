using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using System.Xml;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Pages;
using System.Diagnostics;
using OpenQA.Selenium.Remote;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;
using Selenium.MergeReportService;
using System.Globalization;
using System.Threading;

namespace Selenium.Scripts.DriverScript
{
    class TestRunner
    {
        public static String ServerIP = "";
        public static String isImageSharing = "";
        public static String isXDS = "";
        public static String isRDM = "";
        public static String isHTTPS = "";

        public static String VPName = "";
        public static String TestMethod = "";
        public static String Parameters;
        public static String testid = "";
        private static string SessionID = "";
        private static MergeReportServiceSoapClient consumer = new MergeReportService.MergeReportServiceSoapClient();
        //private static MergeReportServiceSoapClient consumer = null;
        private static int ExecutionCount = 0;
        private static int TestSet = 0;
        private static string Logfilepath = Directory.GetCurrentDirectory()+Path.DirectorySeparatorChar+"WebServiceLogs";//@"D:\7.1 UV\Functional_Automation\Scripts\Selenium";
        private static bool SetBrowserName, SetTimeZone, SetEnvironment, SetServerNetworkDomain, 
            SetProject, SetBuild, SetServerName, SetClientNetworkDomain, SetClientName, SetAdditionalServerName,SetTestSet , SetTestResult, 
            SetTestCaseDetails, SetTestCaseExecutionDetails, SetTestStepDetails, SetTestStepExecutionDetails, SetTestSetExecutionStatus, Stop;

        [STAThreadAttribute]
        public static void Main(String[] args)
        {
            //Thread.Sleep(30000);
            int count = 0;
            Dictionary<String, TimeSpan> TotalTestRunTime = new Dictionary<string, TimeSpan>();
            Dictionary<String, IList<Object>> OverallResults = new Dictionary<String, IList<Object>>();

            //Set Environment parameters for Server
            AssignInputParams(args);

            //Setup Config parameter based on iput config file
            try { SetupConfig(args[1]); } catch (Exception e) { Logger.Instance.InfoLog("Exception in setting up config values-" + e); }
           

            //To Import the testresult in DB
            if (Config.ImportReport.ToLower().Equals("y"))
            {
                SessionID = consumer.Start(Config.adminUserName, Logfilepath);
                Console.WriteLine("New SessionID:"+ SessionID+" is created");
            }

            BasePage.SetVMResolution("1280", "1024");
            //Set VM resolution while running in batch
            if (Config.BatchMode.ToLower().Equals("y"))
            {
                BasePage.SetVMResolution("1280", "1024");
                BasePage.MapTestDataDrive();
            }

            //Run Tests
            if (!args.Any(a => a.Equals("-vp")))
            {

                //Take the Test Classes (Suites or modules) from Spreadsheet and set the count
                String DataWorkbookPath = Config.BatchMode.ToLower().Equals("y") ? Config.ConfigFilePath : Config.TestSuitePath;
                String[,] data = ReadExcel.ReadData((DataWorkbookPath + Path.DirectorySeparatorChar + "ExecutionList.xls"), "ExecutionList");
                String[,] arrClasNames = new String[data.GetUpperBound(0), 2];
                for (int i = 1; i < (data.GetUpperBound(0) + 1); i++)
                {
                    arrClasNames[(i - 1), 0] = data[i, 2];
                    arrClasNames[(i - 1), 1] = data[i, 1];
                }


                //Iterate through Test Classes to execute their Test method
                while (count < (arrClasNames.GetUpperBound(0) + 1))
                {
                    //Changes - Need to add an if condition based on Suite selected
                    if ((arrClasNames[count, 1]).ToLower() == "y")
                    {
                        //Execution of Suite starts here
                        try
                        {
                            VPName = arrClasNames[count, 0];
                            TestSet = 0;
                            try { BasePage.Driver.Quit(); BasePage.Driver = null; } catch (Exception) { }                            
                            IList<Object> result = ExecuteTest(arrClasNames[count, 0],xmlpath:args[1]);
                            OverallResults.Add(arrClasNames[count, 0], result);
                            TotalTestRunTime.Add(arrClasNames[count, 0], (TimeSpan)result[2]);
                            if (!String.IsNullOrEmpty(SessionID))
                            {
                                try
                                {
                                    SetTestSetExecutionStatus = consumer.SetTestSetExecutionStatus(SessionID);
                                    Logger.Instance.InfoLog("SetTestSetExecutionStatus:" + SetTestSetExecutionStatus);
                                }
                                catch(Exception e)
                                {
                                    Logger.Instance.ErrorLog("Exception in setting execution status--" + e.Message + e.StackTrace);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Instance.ErrorLog("Error in executing the Suite==" + arrClasNames[count, 0]);
                            Logger.Instance.ErrorLog("Exception--" + e.Message + e.StackTrace);
                            Logger.Instance.ErrorLog("Inner Exception--" + e.InnerException);
                            //Logger.Instance.InfoLog("Started Execution of suite==" + arrClasNames[count + 1, 0]);
                        }

                    }
                    count++;
                }                
            }
            else
            {
                for (int i = 0; i < args.Length; i += 2)
                {
                    args[i] = args[i].ToLower();
                    switch (args[i])
                    {
                        case "vp":
                        case "-vp":
                            VPName = args[i + 1];
                            break;

                        case "testcase":
                        case "-testcase":
                            TestMethod = args[i + 1];
                            break;

                        case "param":
                        case "-param":
                            Parameters = args[i + 1];
                            break;

                    }
                }
                try
                {
                    TestSet = 0;
                    try { BasePage.Driver.Quit(); BasePage.Driver = null; } catch (Exception) { }
                    IList<Object> result = ExecuteTest(VPName, true, xmlpath: args[1]);                    
                    OverallResults.Add(VPName, result);
                    TotalTestRunTime.Add(VPName, (TimeSpan)result[2]);
                    if (!String.IsNullOrEmpty(SessionID))
                    {
                        try
                        {
                            SetTestSetExecutionStatus = consumer.SetTestSetExecutionStatus(SessionID);
                            Logger.Instance.InfoLog("SetTestSetExecutionStatus:" + SetTestSetExecutionStatus);
                        }
                        catch (Exception e)
                        {
                            Logger.Instance.ErrorLog("Exception in setting execution status--" + e.Message + e.StackTrace);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Error in executing the Suite==" + VPName + " Test case: " + TestMethod);
                    Logger.Instance.ErrorLog("Exception--" + e.Message + e.StackTrace);
                    Logger.Instance.ErrorLog("Inner Exception--" + e.InnerException);
                    Logger.Instance.InfoLog("Started Execution of suite==" + VPName + " Test case: " + TestMethod);
                }
            }
            if (!String.IsNullOrEmpty(SessionID))
            {
                try
                {                    
                    Stop = consumer.Stop(SessionID);                    
                    Logger.Instance.InfoLog("Stop:" + Stop);
                }
                catch(Exception e)
                {
                    Logger.Instance.ErrorLog("Exception in setting execution status--" + e.Message + e.StackTrace);
                }
            }

            //Setup the overall Report
            String[] reportparams = null;
            try
            {
                reportparams = GetReportParam(TotalTestRunTime);
                CreateOverrallXML(reportparams, OverallResults, args[1]);
            }
            catch (Exception e) { Logger.Instance.ErrorLog("Exception in Creating Overall report xml - " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException); }

           
            //Email Execution Summary
            if (Config.BatchMode.ToLower().Equals("y"))
            {
                try
                {
                    //Send Overall Report
                    SendOverallReport(reportparams, OverallResults);
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Exception in sending Overall report status as Email - " +
  e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                }
            }

            //Re-Run Failed Tests
            if (Config.RerunMode.ToLower().Equals("y") && Config.BatchMode.ToLower().Equals("y"))
            {                
                //Environment Preconditions
                BasePage.SetVMResolution("1280", "1024");
                BasePage.MapTestDataDrive();

                int reportfolderflag = 0;
                Dictionary<String, TimeSpan> Rerun_TotalTestRunTime = new Dictionary<string, TimeSpan>();
                Dictionary<String, IList<Object>> Rerun_OverallResults = new Dictionary<String, IList<Object>>();
                TimeSpan totaltime = new TimeSpan(0, 00, 00, 00);
                foreach (String module in OverallResults.Keys)
                {
                    if (module.ToLower().Equals("environmentsetup")) { continue; }
                    String[,] data1 = ReadExcel.ReadData((Config.ConfigFilePath + Path.DirectorySeparatorChar + "ExecutionList" + ".xls"), module);
                    String[,] methodnames = new String[data1.GetUpperBound(0), 2];

                    foreach (String testID in ((Dictionary<String, TestCaseResult>)OverallResults[module][0]).Keys)
                    {
                        //Get data from module
                        TestCaseResult result = ((Dictionary<String, TestCaseResult>)OverallResults[module][0])[testID];
                        if (result.status.ToLower().Equals("fail"))
                        {
                            //Creates Execution report folder if any cases failed in main execution
                            if (reportfolderflag == 0)
                            {
                                ExecutionCount = 0;
                                if (Config.ImportReport.ToLower().Equals("y"))
                                {
                                    SessionID = consumer.Start(Config.adminUserName, Logfilepath);
                                    Logger.Instance.InfoLog("New SessionID:" + SessionID + " is created");
                                }

                                //Quit Driver after main execution
                                BasePage.Driver.Quit();
                                BasePage.Driver = null;

                                //Create New report folder path
                                Config.reportpath = Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TestResult_" + DateTime.Now.ToString("MMMM_dd_yyyy_HH_mm_ss");
                                Directory.CreateDirectory(Config.reportpath);
                                Config.detailedreportpath = Config.reportpath + Path.DirectorySeparatorChar + "DetailedReport";
                                Directory.CreateDirectory(Config.detailedreportpath);
                                Config.screenshotpath = Config.detailedreportpath + Path.DirectorySeparatorChar + "Screenshot";
                                Directory.CreateDirectory(Config.screenshotpath);

                                //Report Templates Copy
                                Directory.CreateDirectory(Config.reportpath + "\\ReportTemplates");
                                File.Copy(Directory.GetCurrentDirectory() + "\\OtherFiles\\ReportTemplates\\DetailedReport.xsl", Config.reportpath + "\\ReportTemplates\\DetailedReport.xsl", true);
                                File.Copy(Directory.GetCurrentDirectory() + "\\OtherFiles\\ReportTemplates\\logo_merge_healthcare.png", Config.reportpath + "\\ReportTemplates\\logo_merge_healthcare.png", true);
                                File.Copy(Directory.GetCurrentDirectory() + "\\OtherFiles\\ReportTemplates\\OverallReport.xsl", Config.reportpath + "\\ReportTemplates\\OverallReport.xsl", true);

                                //increment report folder flag to avoid repetitive folder creation
                                reportfolderflag++;
                                ExecutionCount++;
                            }

                            for (int i = 1; i < data1.GetUpperBound(0) + 1; i++)
                            {
                                if (data1[i, 2].ToLower().Equals(testID.ToLower()))
                                {
                                    methodnames[(i - 1), 0] = data1[i, 5];
                                    methodnames[(i - 1), 1] = "y";
                                }
                                else if (data1[i, 2].ToLower().Equals(testID.ToLower()))
                                {
                                    methodnames[(i - 1), 0] = data1[i, 5];
                                    methodnames[(i - 1), 1] = "n";
                                }
                            }
                        }
                    }
                    if (reportfolderflag >= 1)
                    {
                        try
                        {
                            TestSet = 0;
                            IList<Object> Rerun_result = ExecuteTest(module, TestmethodsList: methodnames, xmlpath: args[1]);                            
                            Rerun_OverallResults.Add(module, Rerun_result);
                            Rerun_TotalTestRunTime.Add(module, (TimeSpan)Rerun_result[2]);
                            if (!String.IsNullOrEmpty(SessionID))
                            {
                                try
                                {
                                    SetTestSetExecutionStatus = consumer.SetTestSetExecutionStatus(SessionID);
                                    Logger.Instance.InfoLog("SetTestSetExecutionStatus:" + SetTestSetExecutionStatus);
                                }
                                catch (Exception e)
                                {
                                    Logger.Instance.ErrorLog("Exception in setting execution status--" + e.Message + e.StackTrace);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Instance.ErrorLog("Error in executing the Suite==" + module);// + " Test case: " + TestMethod);
                            Logger.Instance.ErrorLog("Exception--" + e.Message + e.StackTrace);
                            Logger.Instance.ErrorLog("Inner Exception--" + e.InnerException);
                        }
                    }
                }
                if (!String.IsNullOrEmpty(SessionID))
                {
                    try
                    {
                        Stop = consumer.Stop(SessionID);                        
                        Logger.Instance.InfoLog("Stop:" + Stop);
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.ErrorLog("Exception in setting execution status--" + e.Message + e.StackTrace);
                    }
                }

                //Create the overall Rerun Report
                String[] Rerun_reportparams = null;
                if (reportfolderflag >= 1)
                {
                    try
                    {
                        Rerun_reportparams = GetReportParam(Rerun_TotalTestRunTime);
                        CreateOverrallXML(Rerun_reportparams, Rerun_OverallResults, args[1]);
                    }
                    catch (Exception e) { Logger.Instance.ErrorLog("Exception in Creating Overall report xml - " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException); }

                    //Send Overall Rerun Report in mail to recipients
                    try
                    {
                        SendOverallReport(Rerun_reportparams, Rerun_OverallResults);
                    }
                    catch (Exception e) { Logger.Instance.ErrorLog("Exception in sending Overall report status as Email - " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException); }
                }
            }

            //Quit Driver object after main/rerun run
            try
            {
                BasePage.Driver.Quit();
                BasePage.Driver = null;
            }
            catch (Exception) { }
        }

        /// <summary>
        /// This  method executes each Test Method in the Test Class.
        /// </summary>
        /// <param name="classname"></param>
        /// <returns></returns>
        public static IList<Object> ExecuteTest(String classname, bool ExecuteSpecificTest = false, String[,] TestmethodsList = null,String xmlpath=null)
        {
            //Declare variable
            MethodInfo[] methods;
            Assembly assembly;
            Type type;
            object obj;
            Dictionary<String, TestCaseResult> SuiteResults;
            Dictionary<String, String> Duration;
            Dictionary<String, String> TestName;
            Dictionary<String, String> TestData;
            TimeSpan totaltime;
            IList<Object> suiteresult_timespan = new List<Object>();

            //Setup Results and Duration object
            SuiteResults = new Dictionary<string, TestCaseResult>();
            Duration = new Dictionary<string, string>();
            TestName = new Dictionary<string, string>();
            TestData = new Dictionary<string, string>();
            totaltime = new TimeSpan(0, 00, 00, 00);
            string elapsedTotalTime = "";
            Object[] TestSuiteDetails = new Object[3];

            //Setup logfile for the suite
            Config.logfilepath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + classname;
            Directory.CreateDirectory(Config.logfilepath);
            Logger.Instance.Initialize(Config.logfilepath);

            //Load the Class and Get all the methods
            assembly = Assembly.Load("Selenium");
            type = assembly.GetType("Selenium.Scripts.Tests." + classname);
            obj = Activator.CreateInstance(type, new object[] { classname });
            methods = type.GetMethods();
            int iterator = 0;

            //Get all Test Methods and its ExecutionFlag from driver spredasheet
            String[,] testmethods;
            if (ExecuteSpecificTest)
            {
                testmethods = new string[,] { { TestMethod, "y" } };
            }
            else if (classname.Equals("EnvironmentSetup"))
            {
                testmethods = GetEnvironmentSetupMethodsList(classname);
            }
            else if (TestmethodsList != null)
            {
                testmethods = TestmethodsList;
            }
            else
            {
                testmethods = GetMethodList(classname);
            }
            try
            {    
                // Iterate through all Test methods in the class            
                MethodInfo method = null;
                for (iterator = 0; iterator <= testmethods.GetUpperBound(0); iterator++)
                {
                    if (ExecuteSpecificTest)
                    {
                        method = GetTestMethod(TestMethod, methods);
                    }
                    else
                    {
                        method = GetTestMethod(testmethods[iterator, 0], methods);
                    }
                    if (method != null && testmethods[iterator, 1].ToLower() == "y")
                    {
                        //String testid = "";
                        String testdescription = "";
                        Stopwatch stopwatch = null;
                        String teststeps = "";
                        int stepscount = 0;
                        String starttime = "";

                        try
                        {
                            Logger.Instance.InfoLog("============= Started Execution of Test Method==" + method.Name + " ===================== ");

                            //Get validation steps for the Test method and Test ID
                            teststeps = GetTestSteps(method.Name, classname, out testid, out testdescription);
                            String testdata = GetTestData(classname, testid);
                            TestData.Add(testid, testdata);
                            stepscount = teststeps.Split('=')[0].Split(':').Length;

                            //Execute the Test Method
                            stopwatch = new Stopwatch();
                            stopwatch.Start();
                            //setup videocapture
                            try
                            {
                                if (Config.videoCapture != null && Config.videoCapture.ToLowerInvariant().Equals("y"))
                                {
                                    string VideoFilePath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + classname + Path.DirectorySeparatorChar + method.Name + "_"+string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".mp4";
                                    VideoCaptureUtil.StartVideoCapture(VideoFilePath);
                                }
                            }

                            catch (Exception ex) { Logger.Instance.ErrorLog("Error in Strating video capture. check VLC installed "+ ex.ToString()); }
                            starttime = String.Format("{0:MMMM-dd-yyyy HH:mm:ss}", DateTime.Now);
                            TestCaseResult result;
                            if (String.IsNullOrEmpty(Parameters))
                                result = (TestCaseResult)method.Invoke(obj, new object[] { testid, teststeps, stepscount });
                            else
                                result = (TestCaseResult)method.Invoke(obj, new object[] { testid, teststeps, stepscount, Parameters });
                            stopwatch.Stop();

                            try
                            {
                                if (Config.videoCapture != null && Config.videoCapture.ToLowerInvariant().Equals("y"))
                                {
                                    VideoCaptureUtil.StopVideoCapture();
                                }
                            }catch(Exception ex)

                            { Logger.Instance.ErrorLog("Error in Starting video capture "); }
                            //Take Total time and other result objects
                            totaltime = totaltime + stopwatch.Elapsed;
                            Duration.Add(testid, (starttime + "=" + ((stopwatch.Elapsed.Days) + ":" + (stopwatch.Elapsed.Hours) + ":" + (stopwatch.Elapsed.Minutes) + ":" + (stopwatch.Elapsed.Seconds))));
                            SuiteResults.Add(testid, result);
                            TestName.Add(testid, testdescription);
                            TestSuiteDetails = new Object[5] { SuiteResults, Duration, TestName, TestData, totaltime };

                            if (!String.IsNullOrEmpty(SessionID))
                            {
                                //Import details to the DB
                                if (ExecutionCount == 0)
                                {
                                    ImportEnvironmentDetails();
                                    ExecutionCount++;
                                }
                                if(TestSet==0)
                                {
                                    if (!String.IsNullOrEmpty(SessionID))
                                    {
                                        try
                                        {                                            
											try
											{
												SetTestSet = consumer.SetTestSet(SessionID, classname, Config.ExecutionType);
												Logger.Instance.InfoLog("SetTestSet:" + SetTestSet);
											}
											catch (Exception exception)
											{
												Logger.Instance.ErrorLog("Exception occured in Importing Test set details - " + exception.Message + exception.StackTrace);
												consumer = new MergeReportServiceSoapClient();
												SetTestSet = consumer.SetTestSet(SessionID, classname, Config.ExecutionType);
												Logger.Instance.InfoLog("SetTestSet:" + SetTestSet);
											}

											SetTestResult = consumer.SetTestResult(SessionID);
                                            Logger.Instance.InfoLog("SetTestResult:" + SetTestResult);
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.Instance.ErrorLog("Exception in setting execution status--" + e.Message + e.StackTrace);
                                        }
                                    
                                    }
                                    TestSet++;
                                }
                                if (!String.IsNullOrEmpty(SessionID))
                                {
                                    try
                                    {
										try
										{
											SetTestCaseDetails = consumer.SetTestCaseDetails(SessionID, testid, TestName[testid]);
											Logger.Instance.InfoLog("SetTestCaseDetails:" + SetTestCaseDetails);
										}
										catch (Exception exception)
										{
											Logger.Instance.ErrorLog("Exception occured in Importing Test case details - " + exception.Message + exception.StackTrace);
											consumer = new MergeReportServiceSoapClient();
											SetTestCaseDetails = consumer.SetTestCaseDetails(SessionID, testid, TestName[testid]);
											Logger.Instance.InfoLog("SetTestCaseDetails:" + SetTestCaseDetails);
										}
									}
                                    catch (Exception e)
                                    {
                                        Logger.Instance.ErrorLog("Exception in setting execution status--" + e.Message + e.StackTrace);
                                    }
                                }
                                ImportReport(testid, result, new Object[] { classname, Duration, TestName, TestData, totaltime });
                            }
                                                       
                        }
                        catch (Exception e)
                        {
                            Logger.Instance.ErrorLog("============= Execution Terminated Due to Error ==" + method.Name + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + "Inner Exception is==" + e.InnerException + "=====================");

                            //Set up objects for reporting
                            TestCaseResult testresult = new TestCaseResult(stepscount);

                            //Test Case Reult object
                            try
                            {
                                testresult.SetTestStepDescription(teststeps);
                                testresult.steps[0].SetLogs(e);
                                testresult.FinalResult(e, -1);
                                testresult.status = "Fail";
                                SuiteResults.Add(testid, testresult);
                            }
                            catch (Exception excep)
                            {
                                testresult.status = "Fail";
                                SuiteResults.Add(testid, testresult);
                            }
                            //Video capture stop
                            try
                            {
                                if (Config.videoCapture != null && Config.videoCapture.ToLowerInvariant().Equals("y"))
                                {
                                    VideoCaptureUtil.StopVideoCapture();
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                            //Duration object
                            stopwatch.Stop();
                            totaltime = totaltime + stopwatch.Elapsed;
                            Duration.Add(testid, (starttime + "=" + ((stopwatch.Elapsed.Days) + ":" + (stopwatch.Elapsed.Hours) + ":" + (stopwatch.Elapsed.Minutes) + ":" + (stopwatch.Elapsed.Seconds))));

                            //Testname
                            TestName.Add(testid, testdescription);

                            //All requied objects in an array
                            TestSuiteDetails = new Object[5] { SuiteResults, Duration, TestName, TestData, totaltime };

                            //Import details to the DB
                            if (!String.IsNullOrEmpty(SessionID))
                            {
                                if (ExecutionCount == 0)
                                {
                                    ImportEnvironmentDetails();
                                    ExecutionCount++;
                                }
                                if (TestSet == 0)
                                {
                                    if (!String.IsNullOrEmpty(SessionID))
                                    {
                                        try
                                        {
											try
											{
												SetTestSet = consumer.SetTestSet(SessionID, classname, Config.ExecutionType);
												Logger.Instance.InfoLog("SetTestSet:" + SetTestSet);												
											}
											catch (Exception exception)
											{
												Logger.Instance.ErrorLog("Exception occured in Importing Test case details - " + exception.Message + exception.StackTrace);
												consumer = new MergeReportService.MergeReportServiceSoapClient();
												SetTestSet = consumer.SetTestSet(SessionID, classname, Config.ExecutionType);
												Logger.Instance.InfoLog("SetTestSet:" + SetTestSet);
											}                                            

                                            SetTestResult = consumer.SetTestResult(SessionID);
                                            Logger.Instance.InfoLog("SetTestResult:" + SetTestResult);
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.Instance.ErrorLog("Exception in setting execution status--" + ex.Message + ex.StackTrace);
                                        }
                                    }
                                    TestSet++;
                                }
                                if (!String.IsNullOrEmpty(SessionID))
                                {
                                    try
                                    {
										try
										{
											SetTestCaseDetails = consumer.SetTestCaseDetails(SessionID, testid, TestName[testid]);
											Logger.Instance.InfoLog("SetTestCaseDetails:" + SetTestCaseDetails);
										}
										catch (Exception exception)
										{
											Logger.Instance.ErrorLog("Exception occured in Importing Test case details - " + exception.Message + exception.StackTrace);
											consumer = new MergeReportServiceSoapClient();
											SetTestCaseDetails = consumer.SetTestCaseDetails(SessionID, testid, TestName[testid]);
											Logger.Instance.InfoLog("SetTestCaseDetails:" + SetTestCaseDetails);
										}
									}
                                    catch (Exception ex)
                                    {
                                        Logger.Instance.ErrorLog("Exception in setting execution status--" + ex.Message + ex.StackTrace);
                                    }
                                }
                                ImportReport(testid, testresult, new Object[] { classname, Duration, TestName, TestData, totaltime });
                            }
                        }
                        //if (Config.ImportReport.ToLower().Equals("y"))
                        //{
                        //    Logger.Instance.InfoLog("SetBrowserName:" + SetBrowserName);
                        //    Logger.Instance.InfoLog("SetTimeZone:" + SetTimeZone);
                        //    Logger.Instance.InfoLog("SetEnvironment:" + SetEnvironment);
                        //    Logger.Instance.InfoLog("SetServerNetworkDomain:" + SetServerNetworkDomain);
                        //    Logger.Instance.InfoLog("SetProject:" + SetProject);
                        //    Logger.Instance.InfoLog("SetBuild:" + SetBuild);
                        //    Logger.Instance.InfoLog("SetServerName:" + SetServerName);
                        //    Logger.Instance.InfoLog("SetClientNetworkDomain:" + SetClientNetworkDomain);
                        //    Logger.Instance.InfoLog("SetClientName:" + SetClientName);
                        //    Logger.Instance.InfoLog("SetAdditionalServerName:" + SetAdditionalServerName);
                        //}
                    }                                  
                }

                //Call Reporting Component - Suite Level
                CreateSuiteXML(TestSuiteDetails, classname);

                //This suite result has all the TestCasResult objects in the Test case
                suiteresult_timespan.Add(SuiteResults);

                //This is the start time for the suite
                suiteresult_timespan.Add(Duration.First().Value);

                //This is total test run for the suite
                suiteresult_timespan.Add(totaltime);

                if (Config.BatchMode.ToLower().Equals("y"))
                {
                    try
                    {
                        //Send Module Execution details as Email
                        SendModuleReport(classname, SuiteResults, Duration, totaltime);
                    }
                    catch (Exception e) { Logger.Instance.ErrorLog("Exception in sending Module report status as Email - " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException); }
                }

                return suiteresult_timespan;
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("============= Execution of Test Suite Terminated due to==" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + "Inner Exception is===" + e.InnerException + " ===================== ");
                BasePage.Driver.Quit();
                suiteresult_timespan.Add(SuiteResults);
                suiteresult_timespan.Add(Duration.First().Value);
                suiteresult_timespan.Add(totaltime);
                return suiteresult_timespan;
            }
        }

        /// <summary>
        /// This method is to set up the config data
        /// </summary>
        /// <param name="args"></param>
        public static void SetupConfig(string xmlpath)
        {
            //Setup required config Data
            Dictionary<string, string> browserconfig = ReadXML.ReadDataXML(xmlpath, "/ConfigData/BrowserType");
            Dictionary<string, string> batchrunconfig = ReadXML.ReadDataXML(xmlpath, "/ConfigData/BatchRunDetails");
            Dictionary<string, string> ControllerDetailsConfig = ReadXML.ReadDataXML(xmlpath, "/ConfigData/BatchRunDetails/ControllerDetails");
            Dictionary<string, string> networkconfig = ReadXML.ReadDataXML(xmlpath, "/ConfigData/BatchRunDetails/NetworkDomain");
            Dictionary<string, string> EnvironmentDetailsConfig = ReadXML.ReadDataXML(xmlpath, "/ConfigData/EnvironmentDetails");
            Dictionary<string, string> applicationconfig = ReadXML.ReadDataXML(xmlpath, "/ConfigData/IPAddress");
            Dictionary<string, string> pathconfig = ReadXML.ReadDataXML(xmlpath, "/ConfigData/FilePath");
            Dictionary<string, string> userconfig = ReadXML.ReadDataXML(xmlpath, "/ConfigData/UserInfo");
            Dictionary<string, string> XDSHTTPSConfig = ReadXML.ReadDataXML(xmlpath, "/ConfigData/XDS-Config/HTTPS");
            Dictionary<string, string> XDSHTTPConfig = ReadXML.ReadDataXML(xmlpath, "/ConfigData/XDS-Config/HTTP");
            Dictionary<string, string> puttyconfig = ReadXML.ReadDataXML(xmlpath, "/ConfigData/PuttyInfo");
            Dictionary<string, string> comparemode = ReadXML.ReadDataXML(xmlpath, "/ConfigData/CompareMode");
            Dictionary<string, string> AETitleconfig = ReadXML.ReadDataXML(xmlpath, "/ConfigData/AETitle");
            Dictionary<string, string> BackupPathconfig = ReadXML.ReadDataXML(xmlpath, "/ConfigData/BackupConfigFilePath");
            Dictionary<string, string> XDSDataSources = ReadXML.ReadDataXML(xmlpath, "/ConfigData/XDS-DataSources");
            Dictionary<string, string> RDMDatasources = ReadXML.ReadDataXML(xmlpath, "/ConfigData/RDM-Datasources");
            Dictionary<string, string> VNADatasources = ReadXML.ReadDataXML(xmlpath, "/ConfigData/VNA-Datasources");
            Dictionary<string, string> IConnectDB = ReadXML.ReadDataXML(xmlpath, "/ConfigData/DBType");
            Dictionary<string, string> AdditionalServerIP = ReadXML.ReadDataXML(xmlpath, "/ConfigData/AdditionalServers/IPAddress");
            Dictionary<string, string> AdditionalServerVersion = ReadXML.ReadDataXML(xmlpath, "/ConfigData/AdditionalServers/Version");
            Dictionary<string, string> Gridconfig = ReadXML.ReadDataXML(xmlpath, "/ConfigData/Grid");
            Dictionary<string, string> LocaleInfo = ReadXML.ReadDataXML(xmlpath, "/ConfigData/Language");
            Dictionary<string, string> iCA_MappingFilePathconfig = ReadXML.ReadDataXML(xmlpath, "/Config/FilePath");
            Dictionary<string, string> BluringViewer_MappingFilePathconfig = ReadXML.ReadDataXML(xmlpath, "/Config/FilePath");
            Dictionary<string, string> EmailInfo = ReadXML.ReadDataXML(xmlpath, "/ConfigData/EmailInfo");
            Dictionary<string, string> SMTPInfo = ReadXML.ReadDataXML(xmlpath, "/ConfigData/EmailInfo/SMTP");
            Dictionary<string, string> IMAPInfo = ReadXML.ReadDataXML(xmlpath, "/ConfigData/EmailInfo/IMAP");
            Dictionary<string, string> EmailSuperAdmin = ReadXML.ReadDataXML(xmlpath, "/ConfigData/EmailInfo/SuperAdmin");
            Dictionary<string, string> EmailPhUser = ReadXML.ReadDataXML(xmlpath, "/ConfigData/EmailInfo/PhUser");
            Dictionary<string, string> EmailArUser = ReadXML.ReadDataXML(xmlpath, "/ConfigData/EmailInfo/ArUser");
            Dictionary<string, string> EmailStUser = ReadXML.ReadDataXML(xmlpath, "/ConfigData/EmailInfo/StUser");
            Dictionary<string, string> timeouts = ReadXML.ReadDataXML(xmlpath, "/ConfigData/Timeouts");
            Dictionary<string, string> EmailPOPAdmin = ReadXML.ReadDataXML(xmlpath, "/ConfigData/EmailInfo/POPAdmin");
            Dictionary<string, string> EmailCustomUsers = ReadXML.ReadDataXML(xmlpath, "/ConfigData/EmailInfo/Custom");
            Dictionary<string, string> extrenalcomponenets = ReadXML.ReadDataXML(xmlpath, "/ConfigData/ExternalComponents");
            Dictionary<string, string> EmailConfig4 = ReadXML.ReadDataXML(xmlpath, "/ConfigData/EmailConfig4");
            Dictionary<string, string> WindowsInfo = ReadXML.ReadDataXML(xmlpath, "/ConfigData/WindowsCredentials");
            Dictionary<string, string> ViewerType = ReadXML.ReadDataXML(xmlpath, "/ConfigData/ViewerType");
            Dictionary<string, string> ExternalApps = ReadXML.ReadDataXML(xmlpath, "/ConfigData/ExternalApplications");           


            //After Config file updates
            Dictionary<string, string> PACSDatasourcesIP = ReadXML.ReadDataXML(xmlpath, "/ConfigData/AdditionalServers/IPAddress/PACSDataSources");
            Dictionary<string, string> EADatasourcesIP = ReadXML.ReadDataXML(xmlpath, "/ConfigData/AdditionalServers/IPAddress/EADataSources");
            Dictionary<string, string> XDSEADatasourcesIP = ReadXML.ReadDataXML(xmlpath, "/ConfigData/AdditionalServers/IPAddress/XDSEADataSources");
            Dictionary<string, string> RDMDatasourcesIP = ReadXML.ReadDataXML(xmlpath, "/ConfigData/AdditionalServers/IPAddress/RDMDataSources");
            Dictionary<string, string> PACS_AETitleconfig = ReadXML.ReadDataXML(xmlpath, "/ConfigData/AdditionalServers/AETitle/PACSDataSources");
            Dictionary<string, string> EA_AETitleconfig = ReadXML.ReadDataXML(xmlpath, "/ConfigData/AdditionalServers/AETitle/EADataSources");
            Dictionary<string, string> XDAEA_AETitleconfig = ReadXML.ReadDataXML(xmlpath, "/ConfigData/AdditionalServers/AETitle/XDSEADataSources");
            Dictionary<string, string> DatasourceNames = ReadXML.ReadDataXML(xmlpath, "/ConfigData/AdditionalServers/DataSourceNames");
            Dictionary<string, string> XDSEA_DatasourceNames = ReadXML.ReadDataXML(xmlpath, "/ConfigData/AdditionalServers/DataSourceNames/XDSEADataSources");
            Dictionary<string, string> MergePortDatasourcesIP = ReadXML.ReadDataXML(xmlpath, "/ConfigData/AdditionalServers/IPAddress/MergePortDatasources");
            Dictionary<string, string> LoadBalancerIP = ReadXML.ReadDataXML(xmlpath, "/ConfigData/AdditionalServers/IPAddress/LoadBalancer");
            Dictionary<string, string> VideoLog = ReadXML.ReadDataXML(xmlpath, "/ConfigData/VideoLog");

            //Set up the global variables
            //Batch Run Details
            Config.BatchMode = GetConfigValues(batchrunconfig, "BatchMode");
            //Config.HTTPSmode = GetConfigValues(batchrunconfig, "HTTPSmode");
            Config.RerunMode = GetConfigValues(batchrunconfig, "RerunMode");
            Config.ExecutionType = GetConfigValues(batchrunconfig, "ExecutionType");
            Config.ImportReport= GetConfigValues(batchrunconfig, "ImportReport");
            Config.Theme = GetConfigValues(batchrunconfig, "Theme");

            //Network Details
            Config.NetUsername = GetConfigValues(networkconfig,"UserName");
            Config.NetPassword = GetConfigValues(networkconfig,"Password");

            //Screen Resolution
            //Config.X_Coordinate = GetConfigValues(EnvironmentDetailsConfig,"Screen_X_Coordinate");
            //Config.Y_Coordinate = GetConfigValues(EnvironmentDetailsConfig,"Screen_Y_Coordinate");

            //Controller details
            Config.ControllerName = GetConfigValues(ControllerDetailsConfig,"HostName");
            Config.ControllerUserName = GetConfigValues(ControllerDetailsConfig,"UserName");
            Config.ControllerPassword = GetConfigValues(ControllerDetailsConfig,"Password");

            //Controller details
            Config.SetImageSharing = !String.IsNullOrEmpty(isImageSharing) ? isImageSharing : GetConfigValues(EnvironmentDetailsConfig,"ImageSharing");
            Config.SetXDS = !String.IsNullOrEmpty(isXDS) ? isXDS : GetConfigValues(EnvironmentDetailsConfig,"XDS");
            Config.SetRDM = !String.IsNullOrEmpty(isRDM) ? isRDM : GetConfigValues(EnvironmentDetailsConfig,"RDM");
            Config.HTTPSmode = !String.IsNullOrEmpty(isHTTPS) ? isHTTPS : GetConfigValues(EnvironmentDetailsConfig,"HTTPSmode");

            //External Components
            Config.isTestCompleteActions = GetConfigValues(extrenalcomponenets,"IsTestCompleteActions");
            try
            {
                Config.videoCapture = VideoLog["IsVideoCapture"];
            }
            catch(Exception ex) { }
            //IP Addresses
            Config.IConnectIP = !String.IsNullOrEmpty(ServerIP) ? ServerIP : GetConfigValues(applicationconfig,"iConnect");
            Config.mpacport = GetConfigValues(applicationconfig,"mpacport");
            Config.CdUploaderServer = "";
            Config.Eiclient1 = GetConfigValues(applicationconfig,"EiClient1");
            Config.Eiclient2 = GetConfigValues(applicationconfig,"EiClient2");
            Config.Popclient1 = GetConfigValues(applicationconfig,"PopClient1");
            Config.Popclient2 = GetConfigValues(applicationconfig,"PopClient2");
            Config.node = GetConfigValues(applicationconfig,"node");
            //Config.RDMIP =applicationconfig["RDM"];
            Config.remotedbinstance = GetConfigValues(applicationconfig,"remotedbinstance");


            //Additional Servers - IP address
            Config.IConnectIP2 = GetConfigValues(AdditionalServerIP,"iConnectServer2");
            Config.HoldingPenIP = GetConfigValues(AdditionalServerIP,"HoldingPen");
            Config.MergePACsIP = GetConfigValues(AdditionalServerIP,"MWLPACS");
            Config.DestinationPACS = GetConfigValues(PACSDatasourcesIP,"DestinationPACS");
            Config.DestinationPACS2 = GetConfigValues(PACSDatasourcesIP,"DestinationPACS2");
            Config.PACS2 = GetConfigValues(PACSDatasourcesIP,"DataSource2-PACS2");
            Config.SanityPACS = GetConfigValues(PACSDatasourcesIP,"DataSource1-PACS1");
            Config.StudyPacs = GetConfigValues(AdditionalServerIP,"SourcePACS");
            Config.DestEAsIp = GetConfigValues(EADatasourcesIP,"DestinationEA");
            Config.EA1 = GetConfigValues(EADatasourcesIP,"DataSource3-EA131");
            Config.EA91 = GetConfigValues(EADatasourcesIP,"DataSource4-EA91");
            Config.EA77 = GetConfigValues(EADatasourcesIP,"DataSource5-EA77");
            Config.EA96 = GetConfigValues(EADatasourcesIP,"DataSource6-EA96");
            //Config.EA7 = GetConfigValues(EADatasourcesIP,"DataMaskingEA");
            Config.XDS_EA1_IP = GetConfigValues(XDSEADatasourcesIP,"XDS-EA1");
            Config.XDS_EA2_IP = GetConfigValues(XDSEADatasourcesIP,"XDS-EA2");
            Config.RDMIP = GetConfigValues(RDMDatasourcesIP,"RDM");
            Config.RDMIP2 = GetConfigValues(RDMDatasourcesIP,"RDM2");
            Config.MergeportIP = GetConfigValues(MergePortDatasourcesIP,"MP");
            
            Config.LB_BigIP = GetConfigValues(LoadBalancerIP,"LB_BigIP");
            Config.LB_VIP = GetConfigValues(LoadBalancerIP,"LB_VIP");
            Config.LB_ICA1IP = GetConfigValues(LoadBalancerIP,"LB_ICA1IP");
            Config.LB_ICA2IP = GetConfigValues(LoadBalancerIP,"LB_ICA2IP");
            Config.LB_HP1IP = GetConfigValues(LoadBalancerIP,"LB_HP1IP");
            Config.LB_HP2IP = GetConfigValues(LoadBalancerIP,"LB_HP2IP");
            Config.LB_SQLDBIP = GetConfigValues(LoadBalancerIP,"LB_SQLDBIP");
            Config.LB_Dest1IP = GetConfigValues(LoadBalancerIP,"LB_Dest1IP");
            Config.LB_Dest2IP = GetConfigValues(LoadBalancerIP,"LB_Dest2IP");
            Config.LB_MWLPacsIP = GetConfigValues(LoadBalancerIP,"LB_MWLPacsIP");
            Config.LB_InstallerURL = GetConfigValues(LoadBalancerIP, "LB_InstallerURL");
            Config.LB_SQLDBName = GetConfigValues(LoadBalancerIP, "LB_SQLDBName");
            Config.HighAvilabilitySetUp = GetConfigValues(LoadBalancerIP, "LB_HighAvailabilitySetup");
            Config.FullUI_InstalltionMode = GetConfigValues(LoadBalancerIP, "LB_HighAvailabilitySetup");


            //Additional servers - Names
           Config.AETitleDestEA = GetConfigValues(EA_AETitleconfig, "DestinationEA");
            Config.XDS = GetConfigValues(DatasourceNames,"XDSDataSource");
            Config.XDS_EA1 = GetConfigValues(XDSEA_DatasourceNames,"XDS-EA1");
            Config.XDS_EA2 = GetConfigValues(XDSEA_DatasourceNames,"XDS-EA2");

            //Additional Servers - Version
            Config.HoldingPenVersion = GetConfigValues(AdditionalServerVersion,"HoldingPen");
            Config.MergePACSVersion = GetConfigValues(AdditionalServerVersion,"MWLPACS");
            Config.DestinationPACSVersion = GetConfigValues(AdditionalServerVersion,"DestinationPACS");
            Config.DestinationPACS2Version = GetConfigValues(AdditionalServerVersion,"DestinationPACS2");
            Config.PACS2Version = GetConfigValues(AdditionalServerVersion,"DataSource2-PACS2");
            Config.SanityPACSVersion = GetConfigValues(AdditionalServerVersion,"DataSource1-PACS1");
            Config.StudyPacsVersion = GetConfigValues(AdditionalServerVersion,"SourcePACS");
            Config.DestEAVersion = GetConfigValues(AdditionalServerVersion,"DestinationEA");
            Config.EA1Version = GetConfigValues(AdditionalServerVersion,"DataSource3-EA131");
            Config.EA91Version = GetConfigValues(AdditionalServerVersion,"DataSource4-EA91");
            Config.EA77Version = GetConfigValues(AdditionalServerVersion,"DataSource5-EA77");
            Config.XDS_EA1Version = GetConfigValues(AdditionalServerVersion,"XDS-EA1");
            Config.XDS_EA2Version = GetConfigValues(AdditionalServerVersion,"XDS-EA2");
            Config.XDSVersion = GetConfigValues(AdditionalServerVersion,"XDS");
            Config.RDMVersion = GetConfigValues(AdditionalServerVersion,"RDM");


            //DB Type
            Config.IConnect_dbversion = GetConfigValues(IConnectDB,"iConnectDBVersion");

            //XDS DataSource Name
            Config.xds1 = GetConfigValues(XDSDataSources,"XDS1");
            Config.xds2 = GetConfigValues(XDSDataSources,"XDS2");
            Config.xds3 = GetConfigValues(XDSDataSources,"XDS3");

            //RDM Datasource
            Config.rdm = GetConfigValues(RDMDatasources,"RDM");
            Config.rdm1 = GetConfigValues(RDMDatasources,"RDM1");
            Config.rdm2 = GetConfigValues(RDMDatasources,"RDM2");
            Config.rdm4 = GetConfigValues(RDMDatasources,"RDM4");

            //VNA Datasource
            //	Config.vna61 = VNADatasources["VNA"];


			//PACS Gateway
			Config.pacsgatway1 = GetConfigValues(applicationconfig,"pacsgateway1");
            Config.pacsgatway2 = GetConfigValues(applicationconfig,"pacsgateway2");


            //Set the IPID
            //PACS Gateway
            Config.ipid1 = GetConfigValues(applicationconfig,"IPID1");
            Config.ipid2 = GetConfigValues(applicationconfig,"IPID2");

            //Putty Details
            Config.puttypath = GetConfigValues(puttyconfig,"Path");
            Config.puttyuser = GetConfigValues(puttyconfig,"User");
            Config.puttypassword = GetConfigValues(puttyconfig,"password");
            Config.rootPwd = GetConfigValues(puttyconfig,"rootpass");

            //Windows Credential
            Config.WindowsUserName = GetConfigValues(WindowsInfo,"UserName");
            Config.WindowsPassword = GetConfigValues(WindowsInfo,"Password");
            Config.WindowsDomain = GetConfigValues(WindowsInfo,"Domain");

            //Browser Type
            Config.BrowserType = GetConfigValues(browserconfig,"Name");

            //Folder Path
            Config.ConfigFilePath = xmlpath.Replace("\\" + xmlpath.Split('\\').LastOrDefault(), String.Empty);
            Config.TestSuitePath = GetConfigValues(pathconfig,"TestSuite");
            Config.TestDataPath = GetConfigValues(pathconfig,"TestDataPath");
            Config.EI_TestDataPath = GetConfigValues(pathconfig,"EITestDataPath"); 
            Config.BuildPath = GetConfigValues(pathconfig,"BuildPath"); 
            Config.EIFilePath = GetConfigValues(pathconfig,"EIFilePath");
            Config.EIFilePath2 = GetConfigValues(pathconfig,"EIFilePath2");
            Config.LdapTenetEIFilePath = GetConfigValues(pathconfig,"LdapTenetEIFilePath");
            try { Config.Z3DBuildPath = GetConfigValues(pathconfig, "Z3DBuildPath"); }
            catch { Config.Z3DBuildPath = @"C:\Z3DBuilds"; }
            Config.reportpath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TestResult_" + DateTime.Now.ToString("MMMM_dd_yyyy_HH_mm_ss");
            System.IO.Directory.CreateDirectory(Config.reportpath);
            Config.detailedreportpath = Config.reportpath + Path.DirectorySeparatorChar + "DetailedReport";
            System.IO.Directory.CreateDirectory(Config.detailedreportpath);
            Config.screenshotpath = Config.detailedreportpath + Path.DirectorySeparatorChar + "Screenshot";
            System.IO.Directory.CreateDirectory(Config.screenshotpath);

            //Report Templates Copy
            Directory.CreateDirectory(Config.reportpath + "\\ReportTemplates");
            File.Copy(Directory.GetCurrentDirectory() + "\\OtherFiles\\ReportTemplates\\DetailedReport.xsl", Config.reportpath + "\\ReportTemplates\\DetailedReport.xsl", true);
            File.Copy(Directory.GetCurrentDirectory() + "\\OtherFiles\\ReportTemplates\\logo_merge_healthcare.png", Config.reportpath + "\\ReportTemplates\\logo_merge_healthcare.png", true);
            File.Copy(Directory.GetCurrentDirectory() + "\\OtherFiles\\ReportTemplates\\OverallReport.xsl", Config.reportpath + "\\ReportTemplates\\OverallReport.xsl", true);

            Config.dicomsendpath = GetConfigValues(pathconfig,"DicomSendPath");
            Config.batchfilepath = GetConfigValues(pathconfig,"BatchFilePath");
            Config.inputparameterpath = xmlpath;
            Config.licensefilepath = GetConfigValues(pathconfig,"LicenseFilePath");
            Config.DSManagerFilePath = GetConfigValues(pathconfig,"DSManagerFilePath");
            Config.ResourceConfigFilePath = GetConfigValues(pathconfig,"ResourceConfigFilePath");
            Config.XDSConfigFilePath = GetConfigValues(pathconfig,"XDSConfigFilePath");
            Config.FileLocationPath = GetConfigValues(pathconfig,"FileLocationPath");
            Config.OriginalLicensePath = GetConfigValues(pathconfig,"OriginalLicensePath");
            Config.User4LicensePath = GetConfigValues(pathconfig,"User4LicensePath");
            Config.BackupLicensePath = GetConfigValues(pathconfig,"BackupLicensePath");
            Config.WebConfigPath = GetConfigValues(pathconfig,"WebConfigPath");
            Config.EmailNotificationWebConfigPath = GetConfigValues(pathconfig,"EmailNotificationWebConfigPath");
            Config.TransferStoreScpServerConfigPath = GetConfigValues(pathconfig,"TransferStoreScpServerConfigPath");
            Config.SystemConfigurationXMLPath = GetConfigValues(pathconfig,"SystemConfigurationXMLPath");
            Config.ServiceFactoryConfigPath = GetConfigValues(pathconfig,"ServiceFactoryConfigPath");
            Config.PrevReleaseFilePath = GetConfigValues(pathconfig,"PrevReleaseFilePath");
            Config.CurrReleaseFilePath = GetConfigValues(pathconfig,"CurrReleaseFilePath");
            Config.iCAInstalledPath = GetConfigValues(pathconfig,"iCAInstalledPath");
            Config.ServiceToolInstalledPath = GetConfigValues(pathconfig,"ServiceToolInstalledPath");
            Config.PrevBuildConfigToolPath = GetConfigValues(pathconfig,"PrevBuildConfigToolPath");
            Config.PrevBuildWebaccessInstallerPath = GetConfigValues(pathconfig,"PrevBuildWebaccessInstallerPath");
            Config.CurrBuildConfigToolPath = GetConfigValues(pathconfig,"CurrBuildConfigToolPath");
            Config.CurrBuildWebaccessInstallerPath = GetConfigValues(pathconfig,"CurrBuildWebaccessInstallerPath");
            Config.ImageTransferExeConfigPath = GetConfigValues(pathconfig,"ImageTransferExeConfigPath");
            Config.ExternalApplicationConfiguration = GetConfigValues(pathconfig,"ExternalApplicationConfiguration");
            Config.DSAServerManagerConfiguration = GetConfigValues(pathconfig,"DSAServerManagerConfiguration");
            Config.Part10Import = GetConfigValues(pathconfig,"Part10Import");
            Config.ica_Mappingfilepath = GetConfigValues(pathconfig,"iCA_MappingFilePath");
            Config.BluringViewer_Mappingfilepath = GetConfigValues(pathconfig,"BluringViewer_Mappingfilepath");
            Config.ImagerConfiguration = GetConfigValues(pathconfig,"ImagerConfiguration");
            Config.HTML5UploaderAcceptedPath = GetConfigValues(pathconfig,"HTML5UploaderAcceptedPath");
            Config.HTML5UploaderRejectedPath = GetConfigValues(pathconfig,"HTML5UploaderRejectedPath");
            Config.HTML5UploaderTempAcceptedPath = GetConfigValues(pathconfig,"HTML5UploaderTemproryAcceptedPath");
            Config.HTML5UploaderTempRejectedPath = GetConfigValues(pathconfig,"HTML5UploaderTemproryRejectedPath");
            Config.DicomMessagingServiceXMLPath = GetConfigValues(pathconfig,"DicomMessagingServiceXMLPath");
            Config.WebAccessP10FilesCachePath = GetConfigValues(pathconfig,"WebAccessP10FilesCachePath");
            Config.WebAccessAmicasP10FilesCache = GetConfigValues(pathconfig,"WebAccessAmicasP10FilesCache");
            Config.FederatedQueryConfiguration = GetConfigValues(pathconfig,"FederatedQueryConfiguration");
            Config.zipPath = GetConfigValues(pathconfig,"ZipPath");
            Config.extractpath = GetConfigValues(pathconfig,"ExtractPath");
            Config.defaultpath = GetConfigValues(pathconfig,"defaultPath");
            Config.ServiceTool_MappingFilePath = GetConfigValues(pathconfig,"ServiceTool_MappingFilePath");
            //dont delet it 
            //  Config.chunkfilesPath = pathconfig["Chunkfilepath"];
            //  Config.DemoclientPath = pathconfig["DemoclientPath"];

            //XDS HTTP Mode Values
            Config.HTTP_ID = GetConfigValues(XDSHTTPConfig,"Id");
            Config.HTTP_Endpoint = GetConfigValues(XDSHTTPConfig,"Endpoint");
            Config.HTTP_AddressEndPoint = GetConfigValues(XDSHTTPConfig,"AddressEndpoint");
            Config.HTTP_Identifier = GetConfigValues(XDSHTTPConfig,"Identifier");
            Config.HTTP_AddressURL = GetConfigValues(XDSHTTPConfig,"AddressURL");

            //XDS HTTPs Mode Values
            Config.HTTPS_ID = GetConfigValues(XDSHTTPSConfig,"Id");
            Config.HTTPS_Endpoint = GetConfigValues(XDSHTTPSConfig,"Endpoint");
            Config.HTTPS_AddressEndPoint = GetConfigValues(XDSHTTPSConfig,"AddressEndpoint");
            Config.HTTPS_Identifier = GetConfigValues(XDSHTTPSConfig,"Identifier");
            Config.HTTPS_AddressURL = GetConfigValues(XDSHTTPSConfig,"AddressURL");

            //User Details - Iconnect Users
            Config.phUserName = GetConfigValues(userconfig,"phUserName");
            Config.phPassword = GetConfigValues(userconfig,"phPassword");
            Config.ph1UserName = GetConfigValues(userconfig,"ph1UserName");
            Config.ph1Password = GetConfigValues(userconfig,"ph1Password");
            Config.ph2UserName = GetConfigValues(userconfig,"ph2UserName");
            Config.ph2Password = GetConfigValues(userconfig,"ph2Password");
            Config.LdapPHUser = GetConfigValues(userconfig,"LdapPHUser");
            Config.LdapSTUser = "st";//userconfig["LdapSTUser"];
            Config.LdapARUser = "ar"; //userconfig["LdapARUser"];
            Config.arUserName = GetConfigValues(userconfig,"arUserName");
            Config.arPassword = GetConfigValues(userconfig,"arPassword");
            Config.ar1UserName = GetConfigValues(userconfig,"ar1UserName");
            Config.ar1Password = GetConfigValues(userconfig,"ar1Password");
            Config.ar2UserName = GetConfigValues(userconfig,"ar2UserName");
            Config.ar2Password = GetConfigValues(userconfig,"ar2Password");
            Config.stUserName = GetConfigValues(userconfig,"stUserName");
            Config.stPassword = GetConfigValues(userconfig,"stPassword");
            Config.st1UserName = GetConfigValues(userconfig,"st1UserName");
            Config.st1Password = GetConfigValues(userconfig,"st1Password");
            Config.newUserName = GetConfigValues(userconfig,"newUserName");
            Config.newPassword = GetConfigValues(userconfig,"newPassword");
            Config.adminUserName = GetConfigValues(userconfig,"adminusername");
            Config.adminPassword = GetConfigValues(userconfig,"adminpassword");
            Config.adminGroupName = GetConfigValues(userconfig,"domainname");
            Config.adminRoleName = GetConfigValues(userconfig,"rolename");
            //Config.nuUserName = userconfig["nuUserName"];
            //Config.nuPassword = userconfig["nuPassword"];

            //LDAP Users
            Config.ldapuser1 = GetConfigValues(userconfig,"Ldapuser1");
            Config.ldappass1 = GetConfigValues(userconfig,"LdapPassword1");
            Config.ldapuser2 = GetConfigValues(userconfig,"Ldapuser2");
            Config.ldappass2 = GetConfigValues(userconfig,"LdapPassword2");
            Config.LdapAdminUserName = GetConfigValues(userconfig,"LdapAdminUserName");
            Config.LdapAdminPassword = GetConfigValues(userconfig,"LdapAdminPassword");
            Config.LdapUserPassword = GetConfigValues(userconfig,"LdapUserPassword");
            Config.MarketDomain1 = GetConfigValues(userconfig,"MarketDomain1");
            Config.LdapSuperAdmin = GetConfigValues(userconfig,"LdapSuperAdmin");
            Config.LdapDomainAdmin = GetConfigValues(userconfig,"LdapDomainAdmin");

            //Holding pen users
            Config.hpUserName = GetConfigValues(userconfig,"hpusername");
            Config.hpPassword = GetConfigValues(userconfig,"hppassword");

            //Mpacs Uesrs       
            Config.pacsadmin = GetConfigValues(userconfig,"pacsadmin");
            Config.pacspassword = GetConfigValues(userconfig,"pacspassword");

            //MergePacs Users
            Config.mergepacsuser = GetConfigValues(userconfig,"mergepacsuser");
            Config.mergepacspassword = GetConfigValues(userconfig,"mergepacspassword");

            //Others
            Config.buildversion = GetConfigValues(userconfig,"buildversion");
            Config.buildnumber = GetConfigValues(userconfig,"buildnumber");
            Config.Dest1 = GetConfigValues(userconfig,"Destination1");
            Config.Dest2 = GetConfigValues(userconfig,"Destination2");
            Config.Dest3 = GetConfigValues(userconfig,"Destination3");
            Config.Inst1 = GetConfigValues(userconfig,"Institution1");
            Config.Inst2 = GetConfigValues(userconfig,"Institution2");
            Config.eiwindow = GetConfigValues(userconfig,"eiwindow");
            Config.eiwindow2 = GetConfigValues(userconfig,"eiwindow2");
            Config.pacswindow = "PACS Gateway Configuration";
            //Config.pacswindow2 = new BasePage().PacsGatewayInstance2;
            Config.eiwindowLdapTenet = GetConfigValues(userconfig,"eiwindowLdapTenet");
            Config.eiInstaller = GetConfigValues(userconfig,"eiInstallerName");
            Config.eiInstaller1 = GetConfigValues(userconfig,"eiInstaller");
            Config.emailid = GetConfigValues(userconfig,"emailid");
            Config.prevbuildversion = GetConfigValues(userconfig,"prevbuildversion");
            Config.currbuildversion = GetConfigValues(userconfig,"currbuildversion");

            //Wireshark Installation Path
            Config.tsharkExePath = @"""C:\Program Files (x86)\Wireshark\tshark.exe""";

            //Set the compare mode
            Config.compareimages = GetConfigValues(comparemode,"imagecompare");
            Config.webconfig = @"C:\WebAccess\WebAccess\Web.config";
            Config.Licensepath = @"C:\WebAccess\WebAccess\Config\License.xml";

            //Load AETitle
            Config.HoldingPenAETitle = GetConfigValues(AETitleconfig,"HoldingPen");
            Config.DestEAsAETitle = GetConfigValues(AETitleconfig,"DestEA");
            Config.DestinationPACSAETitle = GetConfigValues(AETitleconfig,"DestPACS");
            Config.IStore1AETitle = GetConfigValues(AETitleconfig,"IStore1");
            Config.EA1AETitle = GetConfigValues(AETitleconfig,"EA1");
            Config.EA77AETitle = GetConfigValues(AETitleconfig,"EA77");
            Config.EA91AETitle = GetConfigValues(AETitleconfig,"EA91");
			Config.EA96AETitle = GetConfigValues(AETitleconfig,"EA96");
            //.EA7AETitle = GetConfigValues(AETitleconfig,"DataMaskingEA");
            //Config.EA7AETitle = GetConfigValues(AETitleconfig,"DataMaskingEA");
            Config.SanityPACSAETitle = GetConfigValues(AETitleconfig,"SanityPACS");
            Config.PACS2AETitle = GetConfigValues(AETitleconfig,"PACS2");
            Config.ICCAEAAETitle = GetConfigValues(AETitleconfig,"ICCAEAAETitle");

            //Load AETitle from PACSDataSources
            Config.DestinationPACS2AETitle = GetConfigValues(PACS_AETitleconfig,"DestinationPACS2");

            // Grid params
            Config.Clientsys1 = GetConfigValues(Gridconfig,"Clientsys1");
            Config.Clientsys2 = GetConfigValues(Gridconfig,"Clientsys2");
            Config.Clientsys3 = GetConfigValues(Gridconfig,"Clientsys3");
            Config.Clientsys4 = GetConfigValues(Gridconfig,"Clientsys4");

            //Locale
            Config.Locale = GetConfigValues(LocaleInfo,"Locale");

            //Timeouts
            try
            {
                Config.minTimeout = int.Parse(GetConfigValues(timeouts, "Min"));
                Config.medTimeout = int.Parse(GetConfigValues(timeouts, "Med"));
                Config.maxTimeout = int.Parse(GetConfigValues(timeouts, "Max"));
                Config.ms_minTimeout = Config.minTimeout * 1000;
                Config.ms_medTimeout = Config.medTimeout * 1000;
                Config.ms_maxTimeout = Config.maxTimeout * 1000;
            }
            catch (Exception) { }


            //Email config
            try
            {
                Config.POPMailHostname = GetConfigValues(EmailConfig4, "SMTPServer");
                Config.POPMailPort = int.Parse(GetConfigValues(EmailConfig4, "SMTPport"));
                Config.POPMailUseSSL = bool.Parse(GetConfigValues(EmailConfig4, "UseSSL"));
                Config.FileDownloadLocation = GetConfigValues(EmailConfig4, "FileDownloadLocation");
                Config.POP3_Enable = GetConfigValues(EmailConfig4, "POP3_Enable");
                Config.Email_Password = GetConfigValues(EmailConfig4, "EmailPassword");
            }
            catch (Exception) { }          


            //Email configuration
            Config.EmailRecipients = GetConfigValues(EmailInfo,"EmailRecipients");

            //IMAP Configuration         
            Config.IMAPServer = GetConfigValues(IMAPInfo,"Server");
            Config.IMAPport = GetConfigValues(IMAPInfo,"port");
            try
            {
                Config.SSLConnection = bool.Parse(GetConfigValues(IMAPInfo, "SSL"));
            }
            catch (Exception) { }            
            Config.InboxPath = GetConfigValues(IMAPInfo,"InboxPath");
            //SMTP Configuration
            Config.AdminEmail = GetConfigValues(SMTPInfo,"AdminEmail");
            Config.AdminEmailPassword = GetConfigValues(SMTPInfo,"AdminEmailPassword");
            Config.SystemEmail = GetConfigValues(SMTPInfo,"SystemEmail");
            Config.SMTPServer = GetConfigValues(SMTPInfo,"Server");
            Config.SMTPServerIP = GetConfigValues(SMTPInfo,"ServerIP");
            Config.SMTPport = GetConfigValues(SMTPInfo,"port");
            Config.OutboxPath = GetConfigValues(SMTPInfo,"OutboxPath");
            //email Ids
            Config.superAdminEmail = GetConfigValues(EmailSuperAdmin,"superAdminEmail");
            Config.superAdminEmailPassword = GetConfigValues(EmailSuperAdmin,"superAdminEmailPassword");
            Config.ph1Email = GetConfigValues(EmailPhUser,"ph1Email");
            Config.ph1EmailPassword = GetConfigValues(EmailPhUser,"ph1EmailPassword");
            Config.ph2Email = GetConfigValues(EmailPhUser,"ph2Email");
            Config.ph2EmailPassword = GetConfigValues(EmailPhUser,"ph2EmailPassword");
            Config.ar1Email = GetConfigValues(EmailArUser,"ar1Email");
            Config.ar1EmailPassword = GetConfigValues(EmailArUser,"ar1EmailPassword");
            Config.ar2Email = GetConfigValues(EmailArUser,"ar2Email");
            Config.ar2EmailPassword = GetConfigValues(EmailArUser,"ar2EmailPassword");
            Config.stEmail = GetConfigValues(EmailStUser,"stEmail");
            Config.stEmailPassword = GetConfigValues(EmailStUser,"stEmailPassword");
            Config.st1Email = GetConfigValues(EmailStUser,"st1Email");
            Config.st1EmailPassword = GetConfigValues(EmailStUser,"st1EmailPassword");
            Config.POPAdminEmail = GetConfigValues(EmailPOPAdmin,"popAdminEmail");
            Config.POPAdminEmailPassword = GetConfigValues(EmailPOPAdmin,"popAdminEmailPassword");

            //Custom Users email
            Config.CustomUser1Email = GetConfigValues(EmailCustomUsers,"User1");
            Config.CustomUser2Email = GetConfigValues(EmailCustomUsers,"User2");
            Config.CustomUser3Email = GetConfigValues(EmailCustomUsers,"User3");
            Config.CustomUserEmailPassword = GetConfigValues(EmailCustomUsers,"Password");  


            //Viewer Type
            Config.isEnterpriseViewer = GetConfigValues(ViewerType,"IsEnterpriseViewer");

            //External Applications  
            Config.HaloId = "HALO";
            Config.HaloName = "Halo Application";
            Config.HaloIp = "10.9.37.112";
            Config.HaloPort = "80";
            Config.HaloUser = "dicom";
            Config.HaloPass = "Pacs@Merge11";
            try
            {
                //Config.RadSuiteId = ExternalApps["RadSuiteId"];
                //Config.RadsuiteName = ExternalApps["RadsuiteName"];
                //Config.RadSuiteIp = ExternalApps["RadSuiteIp"];
                //Config.RadSuitePort = ExternalApps["RadSuitePort"];
                //Config.RadSuiteUser = ExternalApps["RadSuiteUser"];
                //Config.RadSuitePass = ExternalApps["RadSuitePass"];
                //Config.HaloId = ExternalApps["HaloId"];
                //Config.HaloName = ExternalApps["HaloName"];
                //Config.HaloIp = ExternalApps["HaloIp"];
                //Config.HaloPort = ExternalApps["HaloPort"];
                //Config.HaloUser = ExternalApps["HaloUser"];
                //Config.HaloPass = ExternalApps["HaloPass"];
            }
            catch (Exception)
            {
                Config.RadSuiteId = "RADSUITE";
                Config.RadsuiteName = "RADSUITE using URLLaunch";
                Config.RadSuiteIp = "10.9.39.246";
                Config.RadSuitePort = "80";
                Config.RadSuiteUser = "anonymous";
                Config.RadSuitePass = "anonymous";
                Config.HaloId = "HALO";
                Config.HaloName = "Halo Application";
                Config.HaloIp = "10.9.37.112";
                Config.HaloPort = "80";
                Config.HaloUser = "dicom";
                Config.HaloPass = "Pacs@Merge11";
                //Config.VericisId = ExternalApps["VericisId"];
                //Config.VericisName = ExternalApps["VericisName"];
                //Config.VericisIp = ExternalApps["VericisIp"];
                //Config.VericisPort = ExternalApps["VericisPort"];
                //Config.VericisUser = ExternalApps["VericisUser"];
                //Config.VericisPass = ExternalApps["VericisPass"];
                //Config.VericisEAIp = ExternalApps["VericisEAIp"];
            }
        }

        /// <summary>
        /// This method will get the list of methods in the spreadsheet and it's execution flag.
        /// </summary>
        /// <param name="classname"></param>
        /// <returns></returns>
        public static string[,] GetMethodList(String classname)
        {

            //Take the Test Classes from Spreadsheet and set the count
            String WorkbookPath = Config.BatchMode.ToLower().Equals("y") ? Config.ConfigFilePath : Config.TestSuitePath;
            String WorkbookName = Config.BatchMode.ToLower().Equals("y") ? "ExecutionList" : classname;
            String[,] methods = ReadExcel.ReadData((WorkbookPath + Path.DirectorySeparatorChar + WorkbookName + ".xls"), classname);
            String[,] methodnames = new String[methods.GetUpperBound(0), 2];
            for (int i = 1; i < (methods.GetUpperBound(0) + 1); i++)
            {
                methodnames[(i - 1), 0] = methods[i, 5];
                methodnames[(i - 1), 1] = methods[i, 1];
            }

            return methodnames;

        }

        /// <summary>
        /// This method checks if a method exists in the Class file and if not it will return null
        /// </summary>
        /// <param name="methodname"></param>
        /// <param name="methodlistinclass"></param>
        /// <returns></returns>
        public static MethodInfo GetTestMethod(String methodname, MethodInfo[] methodlistinclass)
        {
            foreach (MethodInfo method in methodlistinclass)
            {
                if (method.Name == methodname)
                {
                    return method;
                }
            }
            return null;
        }

        /// <summary>
        /// Retreives the Test Step description and expected result for a Test
        /// </summary>
        /// <param name="methodname"></param>
        /// <param name="classname"></param>
        /// <param name="testid"></param>
        /// <param name="testdescription"></param>
        /// <returns></returns>
        public static String GetTestSteps(String methodname, String classname, out String testid, out String testdescription)
        {

            //Get TestId
            testid = "";
            testdescription = "";
            String WorkbookPath = Config.BatchMode.ToLower().Equals("y") ? Config.ConfigFilePath : Config.TestSuitePath;
            String WorkbookName = Config.BatchMode.ToLower().Equals("y") ? "ExecutionList" : classname;
            String[,] data1 = ReadExcel.ReadData((WorkbookPath + Path.DirectorySeparatorChar + WorkbookName + ".xls"), classname);
            for (int i = 1; i < data1.GetUpperBound(0) + 1; i++)
            {
                if (data1[i, 5].ToLower().Equals(methodname.ToLower()))
                {
                    testid = data1[i, 2];
                    testdescription = data1[i, 4];
                    break;
                }
            }

            //Get list of Test Steps and Expected Results
            String[,] data = ReadExcel.ReadData((Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls"), "TestSteps");
            String[] teststeps = new String[data.GetUpperBound(0)];
            String[] excpectedreults = new String[data.GetUpperBound(0)];
            String[] actualresults = new String[data.GetUpperBound(0)];
            

            int count = 0;
            for (int i = 1; i < (data.GetUpperBound(0) + 1); i++)
            {
                if (data[i, 0].ToLower().Equals(testid.ToLower()))
                {
                    teststeps[count] = data[i, 2];
                    excpectedreults[count] = data[i, 3];
                    try
                    {
                        actualresults[count] = data[i, 4];
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.ErrorLog("ActualResult Column not available in the TestData Sheet");
                        actualresults[count] = "Step working properly as expected";
                    }
                    count++;
                }
            }

            //Resize Test Step array based on actual stepcount
            Array.Resize<String>(ref teststeps, count);
            String teststepconcat = "";
            foreach (String teststep in teststeps)
            {
                if (teststepconcat == "") { teststepconcat = teststep; }
                else { teststepconcat = teststepconcat + ":" + teststep; }
            }

            //Resize Test Step array based on actual stepcount
            Array.Resize<String>(ref excpectedreults, count);
            String excpectedreultsconcat = "";
            foreach (String excpectedreult in excpectedreults)
            {
                if (excpectedreultsconcat == "") { excpectedreultsconcat = excpectedreult; }
                else { excpectedreultsconcat = excpectedreultsconcat + ":" + excpectedreult; }
            }

            //Resize Test Step array based on actual_result stepcount
            Array.Resize<String>(ref actualresults, count);
            String actualresultconcat = "";
            foreach (String actualresult in actualresults)
            {
                if (actualresultconcat == "") { actualresultconcat = actualresult; }
                else { actualresultconcat = actualresultconcat + ":" + actualresult; }
            }

            return teststepconcat + "=" + excpectedreultsconcat + "=" + actualresultconcat;
        }

        /// <summary>
        /// Gets the input parameters required for summary report
        /// </summary>
        /// <param name="suiteruntime"></param>
        /// <returns></returns>
        public static String[] GetReportParam(Dictionary<String, TimeSpan> suiteruntime)
        {
            TimeSpan totaltime;
            totaltime = new TimeSpan(00, 00, 00, 00);

            //Get the OS name and version          
            String os = Environment.OSVersion.VersionString;

            //Get Machine Name
            String machinename = Environment.MachineName;

            //Get Total Execution Time
            foreach (TimeSpan suitetime in suiteruntime.Values)
            {

                totaltime = suitetime + totaltime;

            }
            String elapsedTotalTime = String.Format("{0:00}:{1:00}:{2:00}:{3:00}",
                                totaltime.Days, totaltime.Hours, totaltime.Minutes, totaltime.Seconds);

            return new String[] { os, machinename, elapsedTotalTime.ToString() };
        }

        /// <summary>
        /// Creates the suite level report for each module.
        /// </summary>
        /// <param name="testsuite"></param>
        /// <param name="suitename"></param>
        public static void CreateSuiteXML(Object[] testsuite, String suitename)
        {
            Dictionary<String, TestCaseResult> result = (Dictionary<String, TestCaseResult>)testsuite[0];
            Dictionary<String, String> duration = (Dictionary<String, String>)testsuite[1];
            Dictionary<String, String> testname = (Dictionary<String, String>)testsuite[2];
            Dictionary<String, String> testdata = (Dictionary<String, String>)testsuite[3];
            TimeSpan totalsuitetime = (TimeSpan)testsuite[4];
            String reportpath = Config.detailedreportpath + Path.DirectorySeparatorChar + suitename + ".xml";

            if (!File.Exists(reportpath))
            {
                using (XmlWriter writer = XmlWriter.Create(reportpath))
                {
                    writer.WriteProcessingInstruction("xml", "version='1.0' encoding='ISO-8859-1'");
                    writer.WriteProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\"../ReportTemplates/DetailedReport.xsl\"");
                    writer.WriteStartElement("XMLStorage");
                    writer.WriteStartElement("Modules");
                    writer.WriteElementString("Version", Config.buildversion + "." + Config.buildnumber);

                    foreach (String testid in testname.Keys)
                    {
                        writer.WriteStartElement("TestCase");
                        writer.WriteAttributeString("ID", testid);
                        writer.WriteAttributeString("Name", testname[testid]);
                        writer.WriteAttributeString("StartTime", duration[testid].Split('=')[0]);
                        TestCaseResult actualresult = result[testid];
                        writer.WriteElementString("Status", actualresult.status);
                        writer.WriteStartElement("TestData");
                        writer.WriteAttributeString("ID", "1");
                        writer.WriteAttributeString("Duration", duration[testid].Split('=')[1]);
                        writer.WriteElementString("Data", testdata[testid]);

                        writer.WriteStartElement("Steps");
                        foreach (TestStep step in actualresult.steps)
                        {
                            writer.WriteStartElement("Step");
                            writer.WriteElementString("StepsSummary", step.description);
                            writer.WriteElementString("ExpectedResult", step.expectedresult);
                            if (step.status.ToLower() == "fail" && (step.actualresult == "" || step.actualresult == string.Empty || step.actualresult == null))
                                writer.WriteElementString("ActualResult", "Step did not work properly as expected");
                            else if(step.status.ToLower() == "pass" || (step.status.ToLower() == "fail" && (step.actualresult != "" && step.actualresult != string.Empty && step.actualresult != null)))
                                writer.WriteElementString("ActualResult", step.actualresult);
                            else if (step.status.ToLower() == "pass" && (step.actualresult == "" || step.actualresult == string.Empty || step.actualresult == null))
                                writer.WriteElementString("ActualResult", "Step worked properly as expected");
                            else
                                writer.WriteElementString("ActualResult", "");
                            writer.WriteElementString("Result", step.status.ToUpper());
                            writer.WriteElementString("Comments", step.comments);
                            writer.WriteElementString("Screenshot", step.snapshotpath);
                            writer.WriteElementString("GoldImage", step.goldimagepath);
                            writer.WriteElementString("TestImage", step.testimagepath);
                            writer.WriteElementString("DiffImage", step.diffimagepath);
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            else
            {
                XDocument document = XDocument.Load(reportpath);
                XElement modulesElement = document.Element("XMLStorage").Element("Modules");

                foreach (String testid in testname.Keys)
                {
                    TestCaseResult actualresult = result[testid];
                    XElement lastnode = modulesElement.Elements("TestCase").LastOrDefault();
                    lastnode.AddAfterSelf(
                        new XElement("TestCase",
                        new XAttribute("ID", testid),
                        new XAttribute("Name", testname[testid]),
                        new XAttribute("StartTime", duration[testid].Split('=')[0]),
                        new XElement("Status", actualresult.status),
                        new XElement("TestData",
                        new XAttribute("ID", "1"),
                        new XAttribute("Duration", duration[testid].Split('=')[1]),
                        new XElement("Data", testdata[testid]))));
                    document.Save(reportpath);

                    XElement testcaseElement = modulesElement.Elements("TestCase").Where(x => x.Attribute("ID").Value.Equals(testid)).FirstOrDefault();
                    XElement DataElement = testcaseElement.Descendants("Data").LastOrDefault();
                    foreach (TestStep step in actualresult.steps)
                    {
                        if (step.status.ToLower().Equals("fail"))
                        {
                            DataElement.AddAfterSelf(
                                new XElement("Steps",
                                new XElement("Step",
                                new XElement("StepsSummary", step.description),
                                new XElement("ExpectedResult", step.expectedresult),
                                new XElement("ActualResult", "Step did not work properly as expected"),
                                new XElement("Result", step.status.ToUpper()),
                                new XElement("Comments", step.comments),
                                new XElement("Screenshot", step.snapshotpath),
                                new XElement("GoldImage", step.goldimagepath),
                                new XElement("TestImage", step.testimagepath),
                                new XElement("DiffImage", step.diffimagepath))));
                            document.Save(reportpath);
                        }
                        else
                        {
                            DataElement.AddAfterSelf(
                                new XElement("Steps",
                                new XElement("Step",
                                new XElement("StepsSummary", step.description),
                                new XElement("ExpectedResult", step.expectedresult),
                                new XElement("ActualResult", step.actualresult),
                                new XElement("Result", step.status.ToUpper()),
                                new XElement("Comments", step.comments),
                                new XElement("Screenshot", step.snapshotpath),
                                new XElement("GoldImage", step.goldimagepath),
                                new XElement("TestImage", step.testimagepath),
                                new XElement("DiffImage", step.diffimagepath))));
                            document.Save(reportpath);
                        }
                    }
                }
            }

            //Add attribute to the xml
            XmlDocument doc = new XmlDocument();
            doc.Load(Config.detailedreportpath + Path.DirectorySeparatorChar + suitename + ".xml");
            XmlAttribute attr = doc.CreateAttribute("name");
            attr.Value = "view";

            XmlNodeList elements = doc.GetElementsByTagName("Screenshot");
            foreach (XmlNode element in elements)
            {
                element.Attributes.SetNamedItem(attr);
            }

            XmlNodeList goldimage = doc.GetElementsByTagName("GoldImage");
            foreach (XmlNode element in goldimage)
            {
                element.Attributes.SetNamedItem(attr);
            }

            XmlNodeList testImage = doc.GetElementsByTagName("TestImage");
            foreach (XmlNode element in testImage)
            {
                element.Attributes.SetNamedItem(attr);
            }

            XmlNodeList diffImage = doc.GetElementsByTagName("DiffImage");
            foreach (XmlNode element in diffImage)
            {
                element.Attributes.SetNamedItem(attr);
            }

            doc.Save(Config.detailedreportpath + Path.DirectorySeparatorChar + suitename + ".xml");
        }

        /// <summary>
        /// To create the overall summary report
        /// </summary>
        /// <param name="reportparams"></param>
        /// <param name="overallresult"></param>
        public static void CreateOverrallXML(String[] reportparams, Dictionary<String, IList<Object>> overallresult, String xmlpath)
        {
            String filepath = Config.reportpath + Path.DirectorySeparatorChar + "SummaryReport.xml";
            using (XmlWriter writer = XmlWriter.Create(filepath))
            {
                writer.WriteProcessingInstruction("xml", "version='1.0' encoding='ISO-8859-1'");
                writer.WriteProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\"ReportTemplates/OverallReport.xsl\"");
                writer.WriteStartElement("XMLStorage");
                writer.WriteStartElement("Modules");

                foreach (String module in overallresult.Keys)
                {
                    writer.WriteStartElement("Module");
                    writer.WriteAttributeString("Name", module);
                    writer.WriteAttributeString("StartTime", ((String)overallresult[module][1]).Split('=')[0]);
                    writer.WriteAttributeString("Duration", ((TimeSpan)overallresult[module][2]).Days + ":" + ((TimeSpan)overallresult[module][2]).Hours + ":" + ((TimeSpan)overallresult[module][2]).Minutes + ":" + ((TimeSpan)overallresult[module][2]).Seconds);

                    //Get Total Test cases
                    int totaltestcases = ((Dictionary<String, TestCaseResult>)overallresult[module][0]).Count;
                    writer.WriteElementString("TotalTestCases", totaltestcases.ToString());

                    //Get Total Passed Test cases
                    int totalpassed = 0;
                    foreach (TestCaseResult result in ((Dictionary<String, TestCaseResult>)overallresult[module][0]).Values)
                    {
                        if (result.status == "Pass")
                        {
                            totalpassed++;
                        }
                    }
                    writer.WriteElementString("Pass", totalpassed.ToString());

                    //Get Total Failed Test Cases
                    int totalfailed = 0;
                    foreach (TestCaseResult result in ((Dictionary<String, TestCaseResult>)overallresult[module][0]).Values)
                    {
                        if (result.status == "Fail")
                        {
                            totalfailed++;
                        }
                    }
                    writer.WriteElementString("Fail", totalfailed.ToString());

                    //Get not executed count
                    int notexecuted = totaltestcases - (totalpassed + totalfailed);
                    writer.WriteElementString("NotExecuted", notexecuted.ToString());

                    //Get the module or suite status
                    String modulestatus = null;
                    if (totalfailed > 0) { modulestatus = "Fail"; } else { modulestatus = "Pass"; }
                    writer.WriteElementString("Result", modulestatus);

                    //Get the detailed report link                    
                    writer.WriteElementString("DetailedView", "DetailedReport/" + module + ".xml");

                    //close module tag
                    writer.WriteEndElement();
                }

                //close modules tag
                writer.WriteEndElement();

                //Get added datasource list
                //IList<String> AddedDatasources = GetAdditionalServers();
                Dictionary<String, IList<String>> AdditionalServerDetails = GetAdditionalServerDetails();

                //Get additional server details from Config file
                //Dictionary<string, string> AdditionalServerIP = ReadXML.ReadDataXML(xmlpath, "/ConfigData/AdditionalServers/IPAddress");
                //Dictionary<string, string> AdditionalServerVersion = ReadXML.ReadDataXML(xmlpath, "/ConfigData/AdditionalServers/Version");

                //Start additional servers tag
                writer.WriteStartElement("AdditionalServers");

                //Including additional Server details
                if (AdditionalServerDetails != null)
                {
                    foreach (String Key in AdditionalServerDetails.Keys)
                    {
                        //bool IfMWLorSourcePACS = (Key.Equals("SourcePACS") || Key.Equals("MWLPACS"));
                        //if (AddedDatasources.Any(datasource => datasource.Equals(AdditionalServerIP[Key]))
                        //    || (AddedDatasources.Any(datasource => datasource.Equals(AdditionalServerIP["HoldingPen"]) && IfMWLorSourcePACS)))
                        //{
                        //Start additional server tag
                        writer.WriteStartElement("AdditionalServer");

                        writer.WriteAttributeString("Type", Key);
                        writer.WriteAttributeString("MachineIP", AdditionalServerDetails[Key][0]);
                        Logger.Instance.InfoLog(AdditionalServerDetails[Key][0] + "===" + Key);
                        writer.WriteAttributeString("MachineName", AdditionalServerDetails[Key][1]);
                        writer.WriteAttributeString("Version", AdditionalServerDetails[Key][2]);
                        //close additional server tag
                        writer.WriteEndElement();
                        //}
                    }
                }

                //close additional servers tag
                writer.WriteEndElement();

                //Start summary tag
                writer.WriteStartElement("Summary");
                writer.WriteElementString("ExecutionTime", ((String)overallresult.First().Value[1]).Split('=')[0]);
                writer.WriteElementString("BuildVersion", Config.buildversion + "." + Config.buildnumber);
                writer.WriteElementString("ServerName", Config.IConnectIP);
                writer.WriteElementString("ClientName", reportparams[1]);
                writer.WriteElementString("OS", reportparams[0]);
                String browsername = new System.Globalization.CultureInfo("en-US").TextInfo.ToTitleCase(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName);
                writer.WriteElementString("BrowserType", browsername + " (" + ((RemoteWebDriver)BasePage.Driver).Capabilities.Version + ")");
                writer.WriteElementString("TotalExecutionTime", reportparams[2]);
                writer.WriteEndElement();

                //close xmlstorage tag
                writer.WriteEndElement();

                //close document
                writer.WriteEndDocument();
            }

            //Add attribute to the xml
            XmlDocument doc = new XmlDocument();
            doc.Load(filepath);
            XmlNodeList elements = doc.GetElementsByTagName("DetailedView");
            XmlAttribute attr = doc.CreateAttribute("name");
            attr.Value = "view";
            foreach (XmlNode element in elements)
            {
                element.Attributes.SetNamedItem(attr);
            }
            doc.Save(filepath);

        }

        /// <summary>
        /// Gets the Test data used for a Test Case
        /// </summary>
        /// <param name="classname"></param>
        /// <param name="testid"></param>
        /// <returns></returns>
        public static String GetTestData(String classname, String testid)
        {
            String testdata = null;
            String[,] data = ReadExcel.ReadData((Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls"), "TestData");

            //Get appropriate Accession Number
            String AccessionID = (String)ReadExcel.GetTestData(data, testid, "AccessionID");
            String AccessionList = (String)ReadExcel.GetTestData(data, testid, "AccessionIDList");
            String Acc = "Accession-" + AccessionID + (String.IsNullOrEmpty(AccessionList) ? "" : "-" + AccessionList);

            //Get the corresponding Patient Id
            String PatientId = (String)ReadExcel.GetTestData(data, testid, "PatientID");
            String Patient = "PatientId-" + PatientId;
            testdata = Acc + " " + Patient;

            return testdata;
        }

        public static IList<String> GetAdditionalServers()
        {
            try
            {
                IList<String> DataSourceList = new List<String>();
                StreamReader sr = null;

                //DataSource Configuration XML path'
                String DataSourceXmlPath = @"\\" + Config.IConnectIP + @"\c$\WebAccess\WebAccess\Config\DataSource\DataSourceManagerConfiguration.xml";

                if (File.Exists(DataSourceXmlPath))
                {
                    // Create an XmlDocument
                    XmlDocument xmlDocument = new XmlDocument();

                    // Load the XML file in to the document
                    if (!Dns.GetHostName().ToUpper().Equals(new BasePage().GetHostName(Config.IConnectIP)))
                    {
                        String newpath = System.IO.Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + DataSourceXmlPath.Split('\\').LastOrDefault();
                        //BasePage.CopyFileFromAnotherMachine(Config.IConnectIP, DataSourceXmlPath, newpath);
                        DataSourceXmlPath = newpath;
                    }

                    sr = new StreamReader(DataSourceXmlPath);
                    xmlDocument.Load(sr);

                    //Get Parent Node
                    XmlNodeList Nodes = xmlDocument.SelectNodes("/dataSources/add/parameters/address");

                    foreach (XmlNode node in Nodes)
                    {
                        DataSourceList.Add(node.InnerXml);
                    }
                    sr.Close();
                    sr.Dispose();

                    return DataSourceList;
                }
                else
                {
                    return DataSourceList;
                }
            }
            catch (Exception e) { return null; }

        }

        /// <summary>
        /// To Send Module Execution details 
        /// </summary>
        /// <param name="classname"></param>
        /// <param name="SuiteResults"></param>
        /// <param name="Duration"></param>
        /// <param name="totaltime"></param>
        private static void SendModuleReport(String classname, Dictionary<String, TestCaseResult> SuiteResults, Dictionary<String, String> Duration, TimeSpan totaltime)
        {
            IList<String> DataSourceList = new List<String>();
            StreamReader sr = null;

            //DataSource Configuration XML path'
            String DataSourceXmlPath = @"\\" + Config.IConnectIP + @"\c$\WebAccess\WebAccess\Config\DataSource\DataSourceManagerConfiguration.xml";

            // Create an XmlDocument
            XmlDocument xmlDocument = new XmlDocument();

            sr = new StreamReader(DataSourceXmlPath);
            xmlDocument.Load(sr);

            String path = Directory.GetCurrentDirectory() + @"\ExecutableFiles\SuiteResult.txt";
            String EmailDetailsPath = Directory.GetCurrentDirectory() + @"\ExecutableFiles\EmailDetails.txt";
            String BlatExePath = Directory.GetCurrentDirectory() + @"\ExecutableFiles\blat.exe";
            String EmailSubject = "\"" + Config.IConnectIP + ": Execution for VP - " + classname + " Completed..\"";
            //String EmailRecipients = File.ReadAllText(EmailDetailsPath);

            if (!File.Exists(path))
            {
                FileStream file = File.Create(path);
                file.Close();
            }

            File.WriteAllText(path, String.Empty);
            TextWriter tw = new StreamWriter(path);
            tw.Flush();
            tw.WriteLine("Total No of Test cases :-\t" + SuiteResults.Keys.Count);
            Logger.Instance.InfoLog("Total No of Test cases: -\t" + SuiteResults.Keys.Count);
            IEnumerable<TestCaseResult> passCount = SuiteResults.Values.Where(value => value.status.ToLower().Equals("pass"));
            tw.WriteLine("Passed Cases :-\t" + passCount.Count());
            Logger.Instance.InfoLog("Passed Cases :-\t" + passCount.Count() + Environment.NewLine);
            tw.WriteLine("Failed Cases :-\t" + (SuiteResults.Keys.Count - passCount.Count()) + Environment.NewLine);
            Logger.Instance.InfoLog("Failed Cases :-\t" + (SuiteResults.Keys.Count - passCount.Count()) + Environment.NewLine);
            tw.WriteLine("S.NO\tTESTCASE ID\tSTATUS\tDURATION");
            tw.WriteLine("------\t---------------\t---------\t------------");

            int counter = 0;
            foreach (String TestID in SuiteResults.Keys)
            {
                tw.WriteLine(++counter + "\t" + TestID + "\t" + SuiteResults[TestID].status + "\t" + Duration[TestID].Split('=').LastOrDefault());
                Logger.Instance.InfoLog(counter + "\t" + TestID + "\t" + SuiteResults[TestID].status + "\t" + Duration[TestID].Split('=').LastOrDefault() + Environment.NewLine);
            }

            tw.WriteLine(Environment.NewLine + "Total Duration :-\t" + totaltime.ToString().Split('.').FirstOrDefault());
            Logger.Instance.InfoLog("Total Duration :-\t" + totaltime.ToString().Split('.').FirstOrDefault());
            tw.Close();

            string EmailBody;
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, System.Text.Encoding.UTF8))
            {
                EmailBody = streamReader.ReadToEnd();
            }
            EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);


        }

        /// <summary>
        /// To Send the Overall Execution details
        /// </summary>
        /// <param name="reportparams"></param>
        /// <param name="overallresult"></param>
        private static void SendOverallReport(String[] reportparams, Dictionary<String, IList<Object>> OverallResults)
        {

            String path = Directory.GetCurrentDirectory() + @"\ExecutableFiles\OverallResult.txt";
            String EmailDetailsPath = Directory.GetCurrentDirectory() + @"\ExecutableFiles\EmailDetails.txt";
            String BlatExePath = Directory.GetCurrentDirectory() + @"\ExecutableFiles\blat.exe";
            String EmailSubject = "\"" + Config.IConnectIP + ": Automation Script Execution - Completed\"";
            //String EmailRecipients = File.ReadAllText(EmailDetailsPath);

            if (!File.Exists(path))
            {
                FileStream file = File.Create(path);
                file.Close();
            }

            File.WriteAllText(path, String.Empty);
            TextWriter tw = new StreamWriter(path);
            tw.Flush();

            tw.WriteLine("Hi," + Environment.NewLine);
            if (reportparams[1].Equals("ICA-A2-WS8") && (OverallResults.ContainsKey("Environment Setup")
                || OverallResults.ContainsKey("Sanity")) && OverallResults.Keys.Count <= 2)
            {
                tw.WriteLine("Find below the Sanity Execution Status." + Environment.NewLine);
                EmailDetailsPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "SanityEmailDetails.txt";
            }
            else
            {
                tw.WriteLine("Find below the Regression Execution Status." + Environment.NewLine);
            }


            tw.WriteLine("Machine Name :\t" + reportparams[1] + Environment.NewLine);

            tw.WriteLine("Execution Details" + Environment.NewLine + "---------------------" + Environment.NewLine);
            tw.WriteLine("Total No of Modules :-\t" + OverallResults.Keys.Count + Environment.NewLine);
            tw.WriteLine("Total Execution time :\t" + reportparams[2] + Environment.NewLine);

            tw.WriteLine("S.NO\tMODULE NAME\tTOTAL CASES\tPASSED CASES\tFAILED CASES\tDURATION");
            Logger.Instance.InfoLog("S.NO\tMODULE NAME\tTOTAL CASES\tPASSED CASES\tFAILED CASES\tDURATION");
            tw.WriteLine("------\t-----------------\t--------------\t---------------\t--------------\t-----------");
            Logger.Instance.InfoLog("------\t-----------------\t--------------\t---------------\t--------------\t-------------");
            int Counter = 0;
            foreach (String module in OverallResults.Keys)
            {
                String Duration = ((TimeSpan)OverallResults[module][2]).Days + ":" + ((TimeSpan)OverallResults[module][2]).Hours + ":" + ((TimeSpan)OverallResults[module][2]).Minutes + ":" + ((TimeSpan)OverallResults[module][2]).Seconds;

                //Get Test cases count
                int totaltestcases = ((Dictionary<String, TestCaseResult>)OverallResults[module][0]).Count;
                int PassedCases = ((Dictionary<String, TestCaseResult>)OverallResults[module][0]).Values.Where(value => value.status.Equals("Pass")).Count();
                int FailedCases = ((Dictionary<String, TestCaseResult>)OverallResults[module][0]).Values.Where(value => value.status.Equals("Fail")).Count();

                tw.WriteLine(++Counter + "\t" + module + "\t" + totaltestcases + "\t" + PassedCases + "\t" + FailedCases + "\t" + Duration + Environment.NewLine);
                Logger.Instance.InfoLog(Counter + "\t" + module + "\t" + totaltestcases + "\t" + PassedCases + "\t" + FailedCases + "\t" + Duration + Environment.NewLine);
            }

            tw.WriteLine("Regards,");
            tw.WriteLine("Automation Team." + Environment.NewLine);
            tw.WriteLine("Note: Detailed report analysis will be sent shortly.");

            tw.Close();

            string EmailBody;
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, System.Text.Encoding.UTF8))
            {
                EmailBody = streamReader.ReadToEnd();
            }
            EmailUtils.SendSMTPEmail(Config.EmailRecipients, EmailSubject, EmailBody, HostIP: Config.SMTPMailServerIP);
        }

        /// <summary>
        /// This method will get the list of methods in the spreadsheet and it's execution flag.
        /// </summary>
        /// <param name="classname"></param>
        /// <returns></returns>
        public static string[,] GetEnvironmentSetupMethodsList(String classname)
        {

            //Take the Test Classes from Spreadsheet and set the count
            String WorkbookPath = Config.BatchMode.ToLower().Equals("y") ? Config.ConfigFilePath : Config.TestSuitePath;
            String WorkbookName = Config.BatchMode.ToLower().Equals("y") ? "ExecutionList" : classname;
            String[,] methods = ReadExcel.ReadData((WorkbookPath + Path.DirectorySeparatorChar + WorkbookName + ".xls"), classname);
            String[,] methodnames = new String[methods.GetUpperBound(0), 2];

            String Setup_flag = Config.SetImageSharing.ToLower().Equals("y") ? "is" : "nis";

            for (int i = 1; i < (methods.GetUpperBound(0) + 1); i++)
            {
                bool SetupType_flag = (methods[i, 6].ToLower().Equals(Setup_flag) || methods[i, 6].ToLower().Equals("all"))
                     && methods[i, 1].ToLower().StartsWith("y");

                bool XDS_flag = (Config.SetXDS.ToLower().Equals("pix") || Config.SetXDS.ToLower().Equals("sad")) && methods[i, 1].ToLower().StartsWith("y")
                    ? methods[i, 7].ToLower().StartsWith("y") : false;

                bool RDM_flag = Config.SetRDM.ToLower().Equals("y") && methods[i, 1].ToLower().StartsWith("y")
                    ? methods[i, 8].ToLower().StartsWith("y") : false;

                bool HighAvail_flag = (Config.HighAvilabilitySetUp.ToLower().Equals("y") && methods[i, 6].ToLower().Equals("ha"))
                    ? methods[i, 1].ToLower().StartsWith("y") : false;

                methodnames[(i - 1), 0] = methods[i, 5];
                if (SetupType_flag || XDS_flag || RDM_flag || HighAvail_flag)
                {
                    methodnames[(i - 1), 1] = "y";
                }
                else
                {
                    methodnames[(i - 1), 1] = "n";
                }
            }

            return methodnames;

        }

        /// <summary>
        /// This function is to assign input parameters taken from command inputs to the temp variables 
        /// In Setup Config method these temp values will be updated in global Config variables
        /// </summary>
        /// <param name="inputargs"></param>
        public static void AssignInputParams(String[] inputargs)
        {
            if (inputargs.Length > 2)
            {
                for (int i = 0; i < inputargs.Length; i += 2)
                {
                    inputargs[i] = inputargs[i].ToLower();
                    switch (inputargs[i])
                    {
                        case "-ip":
                            ServerIP = inputargs[i + 1];
                            break;

                        case "-is":
                            isImageSharing = inputargs[i + 1];
                            break;

                        case "-xds":
                            isXDS = inputargs[i + 1];
                            break;

                        case "-rdm":
                            isRDM = inputargs[i + 1];
                            break;

                        case "-https":
                            isHTTPS = inputargs[i + 1];
                            break;

                        case "-upgradepath":
                            Config.UpgradePath = inputargs[i + 1];
                            break;

                        case "-setupconfigforupgrade":
                            Config.IsConfigForUpgrade = inputargs[i + 1];
                            break;

                        case "-upgradecomparisonlevel":
                            Config.UpgradeComparisonLevel = inputargs[i + 1];
                            break;

                        case "-downloadfreshinstaller":
                            Config.DownloadFreshInstaller = inputargs[i + 1];
                            break;

                        case "-dbusername":
                            Config.DbUserName = inputargs[i + 1];
                            break;

                        case "-dbpassword":
                            Config.DbPassword = inputargs[i + 1];
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// This method is to get the values for the specified node from the xml by passing as key 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfigValues(Dictionary<string, string> obj, string key)
        {
            string Configvalue = "";
            try
            {
                Configvalue = obj[key];
                Console.WriteLine("Config value for the key: " + key + " is: " + Configvalue);

            }
            catch (Exception e)
            {
                Console.WriteLine("Config value for the key: " + key + " is not found due to- " + e.InnerException + Environment.NewLine + e.StackTrace + Environment.NewLine + e.Message);
            }
            return Configvalue;
        }

        /// <summary>
        /// This method is to 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static string GetNetworkDomain(string ip)
        {
            string domain = "";
            if(ip.StartsWith("10.4"))
            {
                domain = "tor-vctr.products.network.internal";
            }
            else if(ip.StartsWith("10.5"))
            {
                domain = "neptune.products.network.internal";
            }
            else
            {
                domain = "sdg-vctr.products.network.internal";
            }

            return domain;
        }

        /// <summary>
        /// This method is to get the additional server details for each type of datasources
        /// </summary>
        /// <returns></returns>
        public static Dictionary<String, IList<String>> GetAdditionalServerDetails()
        {
            try
            {
                Dictionary<String, IList<String>> AdditionalServerDetails = new Dictionary<String, IList<String>>();

                //Get all additional server details from Config file 
                Dictionary<string, string> AddittionalServers_IP = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/IPAddress");
                Dictionary<string, string> AddittionalServers_DataSourceNames = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/AETitle");
                Dictionary<string, string> AddittionalServers_Version = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/Version");

                //Get PACS data source details from Config file
                Dictionary<string, string> PACS_DataSources_IP = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/IPAddress/PACSDataSources");

                //Get EA data source details from Config file
                Dictionary<string, string> EA_DataSources_IP = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/IPAddress/EADataSources");

                //Get XDS related EA data source details from Config file
                Dictionary<string, string> XDSEA_DataSources_IP = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/IPAddress/XDSEADataSources");

                //Get RDM data source details from Config file
                Dictionary<string, string> RDM_DataSources_IP = ReadXML.ReadDataXML(Config.inputparameterpath, "/ConfigData/AdditionalServers/IPAddress/RDMDataSources");

                //Getting Holding Pen, MWL & Source PACS Details Details               
                if (!String.IsNullOrEmpty(AddittionalServers_IP["HoldingPen"]))
                {
                    String HPhostname = new BasePage().GetHostName(AddittionalServers_IP["HoldingPen"]);
                    AdditionalServerDetails.Add("HoldingPen", new List<String>(new String[] { AddittionalServers_IP["HoldingPen"], HPhostname, AddittionalServers_Version["HoldingPen"] }));

                    String MWLhostname = new BasePage().GetHostName(AddittionalServers_IP["MWLPACS"]);
                    AdditionalServerDetails.Add("MWLPACS", new List<String>(new String[] { AddittionalServers_IP["MWLPACS"], MWLhostname, AddittionalServers_Version["MWLPACS"] }));

                    String Sourcehostname = new BasePage().GetHostName(AddittionalServers_IP["SourcePACS"]);
                    AdditionalServerDetails.Add("SourcePACS", new List<String>(new String[] { AddittionalServers_IP["SourcePACS"], Sourcehostname, AddittionalServers_Version["SourcePACS"] }));
                }

                //Adding EA data sources
                foreach (String EAIP in EA_DataSources_IP.Keys)
                {
                    if (!String.IsNullOrEmpty(EA_DataSources_IP[EAIP]))
                    {
                        String hostname = new BasePage().GetHostName(EA_DataSources_IP[EAIP]);
                        AdditionalServerDetails.Add(EAIP, new List<String>(new String[] { EA_DataSources_IP[EAIP], hostname, AddittionalServers_Version[EAIP] }));
                    }
                }

                //Adding PACS data sources
                foreach (String PACSIP in PACS_DataSources_IP.Keys)
                {
                    if (!String.IsNullOrEmpty(PACS_DataSources_IP[PACSIP]))
                    {
                        String hostname = new BasePage().GetHostName(PACS_DataSources_IP[PACSIP]);
                        AdditionalServerDetails.Add(PACSIP, new List<String>(new String[] { PACS_DataSources_IP[PACSIP], hostname, AddittionalServers_Version[PACSIP] }));
                    }
                }

                if (!String.IsNullOrEmpty(AddittionalServers_DataSourceNames["XDSDataSource"]))
                {
                    AdditionalServerDetails.Add("XDS", new List<String>(new String[] { "", AddittionalServers_DataSourceNames["XDSDataSource"], "" }));
                }

                //Adding XDS related EA data sources
                foreach (String XDSEAIP in XDSEA_DataSources_IP.Keys)
                {
                    if (!String.IsNullOrEmpty(XDSEA_DataSources_IP[XDSEAIP]))
                    {
                        String hostname = new BasePage().GetHostName(XDSEA_DataSources_IP[XDSEAIP]);
                        AdditionalServerDetails.Add(XDSEAIP, new List<String>(new String[] { XDSEA_DataSources_IP[XDSEAIP], hostname, AddittionalServers_Version[XDSEAIP] }));
                    }
                }

                //Adding RDM data sources
                foreach (String RDMIP in RDM_DataSources_IP.Keys)
                {
                    if (!String.IsNullOrEmpty(RDM_DataSources_IP[RDMIP]))
                    {
                        String hostname = new BasePage().GetHostName(RDM_DataSources_IP[RDMIP]);
                        AdditionalServerDetails.Add(RDMIP, new List<String>(new String[] { RDM_DataSources_IP[RDMIP], hostname, AddittionalServers_Version[RDMIP] }));
                    }
                }
                return AdditionalServerDetails;
            }
            catch (Exception e) { return null; }

        }

        /// <summary>
        /// This method is to import the environment details to the DB
        /// </summary>
        public static void ImportEnvironmentDetails()
        {
            string netUsername = Config.NetUsername;
            string netpwd = Config.NetPassword;
            string servernetwork = GetNetworkDomain(Config.IConnectIP);
            string clientnetwork = GetNetworkDomain(Config.Clientsys1);
            String browsername = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName;
            String browserversion = ((RemoteWebDriver)BasePage.Driver).Capabilities.Version;
            IList<string> additionalservers = new List<String>();
            try
            {
                Logger.Instance.InfoLog("Importing Environment details to the DB");
                Logger.Instance.InfoLog("Created SessionID is :" + SessionID);
                SetBrowserName = consumer.SetBrowserName(SessionID, browsername, browserversion);
                Logger.Instance.InfoLog("SetBrowserName:" + SetBrowserName);

                Dictionary<String, IList<String>> AdditionalServerDetails = GetAdditionalServerDetails();
                TimeZone curTimeZone = TimeZone.CurrentTimeZone;
                SetTimeZone = consumer.SetTimeZone(SessionID, curTimeZone.StandardName.ToString());
                Logger.Instance.InfoLog("SetTimeZone:" + SetTimeZone);

                SetServerNetworkDomain = consumer.SetNetworkDomain(SessionID, servernetwork, netUsername, netpwd);
                Logger.Instance.InfoLog("SetServerNetworkDomain:" + SetServerNetworkDomain);

                SetProject = consumer.SetProject(SessionID, "ICA_Test");
                Logger.Instance.InfoLog("SetProject:" + SetProject);

                SetBuild = consumer.SetBuild(SessionID, Config.buildversion + "." + Config.buildnumber);
                Logger.Instance.InfoLog("SetBuild:" + SetBuild);

                SetServerName = consumer.SetServerName(SessionID, new BasePage().GetHostName(Config.IConnectIP));
                Logger.Instance.InfoLog("SetServerName:" + SetServerName);

                SetClientNetworkDomain = consumer.SetNetworkDomain(SessionID, clientnetwork, netUsername, netpwd);
                Logger.Instance.InfoLog("SetClientNetworkDomain:" + SetClientNetworkDomain);

                SetClientName = consumer.SetClientName(SessionID, Environment.MachineName);
                Logger.Instance.InfoLog("SetClientName:" + SetClientName);
                                
                if (AdditionalServerDetails.Count > 0)
                {
                    foreach (IList<string> Key in AdditionalServerDetails.Values)
                    {
                        string AdditionalServernetwork = GetNetworkDomain(Key.First());
                        consumer.SetNetworkDomain(SessionID, AdditionalServernetwork, netUsername, netpwd);
                        Logger.Instance.InfoLog("SetClientNetworkDomain:" + SetClientNetworkDomain+"for the IP:"+ Key.First());
                        additionalservers.Add(Key[1]);
                        Logger.Instance.InfoLog("SetAdditionalServerName:" + SetAdditionalServerName + " for the machine IP: " + Key.First() + "and hostname: " + Key[1]);
                    }
                }
                string servers = string.Join(",", additionalservers.Select(x => x.ToString()).ToArray());
                SetAdditionalServerName = consumer.SetAdditionalServerName(SessionID, servers);//ToDO
                Logger.Instance.InfoLog("SetAdditionalServerNames are imported");

                SetEnvironment = consumer.SetEnvironment(SessionID);
                Logger.Instance.InfoLog("SetEnvironment:" + SetEnvironment);

            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Error in importing environment details" + e);
                Logger.Instance.ErrorLog("Exception--" + e.Message + e.StackTrace);
                Logger.Instance.ErrorLog("Inner Exception--" + e.InnerException);
            }
        }

        /// <summary>
        /// This method is to import the testcase run result to the DB
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="result"></param>
        /// <param name="testobject"></param>
        public static void ImportReport(string testid = null, TestCaseResult result = null, Object[] testobject = null)/*,string comment=null,string screenshot=null,string testimage=null,string goldimage=null,string diffimage=null)*/
        {

            //string SessionID = CurrentSessionID;
            TestCaseResult Testresult = result;
            string testsetname = testobject[0].ToString();
            Dictionary<String, String> duration = (Dictionary<String, String>)testobject[1];
            Dictionary<String, String> testname = (Dictionary<String, String>)testobject[2];
            Dictionary<String, String> testdata = (Dictionary<String, String>)testobject[3];

            try
            {
                Logger.Instance.InfoLog("Start of Importing result of a testcase");
                Logger.Instance.InfoLog("SessionID:" + SessionID);

                SetTestCaseExecutionDetails =consumer.SetTestCaseExecutionDetails(SessionID, Testresult.status, duration[testid].Split('=')[1], 1, testdata[testid]);
                Logger.Instance.InfoLog("SetTestCaseExecutionDetails:" + SetTestCaseExecutionDetails);

                int iterate = 0;
                foreach (TestStep step in Testresult.steps)
                {
                    SetTestStepDetails=consumer.SetTestStepDetails(SessionID, step.description, step.expectedresult, iterate + 1);                   
                    SetTestStepExecutionDetails =consumer.SetTestStepExecutionDetails(SessionID, step.status, step.comments, step.snapshotpath, step.testimagepath, step.goldimagepath, step.diffimagepath);                    
                    iterate++;
                }
                Logger.Instance.InfoLog("SetTestStepDetails:" + SetTestStepDetails);
                Logger.Instance.InfoLog("SetTestStepExecutionDetails:" + SetTestStepExecutionDetails);
                Logger.Instance.InfoLog("End of Importing result of a testcase");
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Error in importing report details" + e);
                Logger.Instance.ErrorLog("Exception--" + e.Message + e.StackTrace);
                Logger.Instance.ErrorLog("Inner Exception--" + e.InnerException);
                Logger.Instance.ErrorLog("Target site--"+ e.TargetSite);
            }

        }


    }
}