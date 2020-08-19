using Selenium.Scripts.Pages;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace Selenium.Scripts.Reusable.Generic
{
    public class TestCompleteConnect
    {

        /// Fields
        static TestComplete.ITestCompleteCOMManager TestExecuteManager = null;
        static TestComplete.ItcIntegration IntegrationObject = null;
        public string RemoteServer { get; set; }
        public int SessionID { get; set; }
        public string batchfilePath = string.Empty;
        public string projectPath = string.Empty;
        public string projectName = "TestProject1";
        public string unitName = "ICAActionsCommandLine";
        BasePage bp = new BasePage();

        public TestCompleteConnect()
        {
            if (bp.browserName.ToLower().Contains("remote"))
            {
                batchfilePath = @"C:\RemoteActions\RunTestExecute.bat";
                projectPath = @"C:\RemoteActions\TestCompleteComponents\TestCompleteVSIntegration\Project.pjs";
            }
            else
            {
                batchfilePath = System.IO.Directory.GetCurrentDirectory() + @"\OtherFiles\RunTestExecute.bat";
                projectPath = @"C:\TestCompleteVSIntegration\Project.pjs";
            }
            RemoteServer = Config.node;
        }
        /// Instance Methods
        /// <summary>
        /// Launch Test Execute
        /// </summary>
        public void Opentestcomplete()
        {
            const string TEProgID = "TestExecute.TestExecuteApplication.12";
            dynamic TestExecuteObject = null;

            // Obtains access to TestExecute
            try
            {
                TestExecuteObject = Marshal.GetActiveObject(TEProgID);
            }
            catch
            {
                try
                {
                    var typeTestExecute = Type.GetTypeFromProgID(TEProgID, true);
                    if (typeTestExecute != null)
                        TestExecuteObject = Activator.CreateInstance(Type.GetTypeFromProgID(TEProgID));
                    else
                        throw new Exception("Test Execute not opened, pls check license usgae");
                }
                catch (Exception ex)
                {
                    Logger.Instance.InfoLog("testexecute did not open" + ex.Message);
                }
            }
            Thread.Sleep(2000);

            try
            {
                if (TestExecuteObject == null) return;
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                Logger.Instance.InfoLog("An exception occurred: " + ex.Message);
            }

            // Obtain the ITestCompleteCOMManager object
            TestExecuteManager = (TestComplete.ITestCompleteCOMManager)TestExecuteObject;
            IntegrationObject = TestExecuteManager.Integration;

            // Loads the project suite
            var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) +
            "\\TestCompleteComponents\\TestCompleteVSIntegration\\Project.pjs";
            path = path.Substring(6);
            IntegrationObject.OpenProjectSuite(path);
            Thread.Sleep(2000);

            // Checks whether the project suite was opened
            if (!IntegrationObject.IsProjectSuiteOpened())
            {
                Logger.Instance.InfoLog("Could not open the project suite.");
                TestExecuteManager.Quit();
                Marshal.ReleaseComObject(IntegrationObject);
                Marshal.ReleaseComObject(TestExecuteManager);
                return;
            }
        }

        /// <summary>
        /// Invoke Test Complete Methods
        /// </summary>
        /// <param name="routineName"></param>
        /// <param name="Params"></param>
        public void TCActions(string routineName, string[] Params = null)
        {
            bp = new BasePage();
            if (bp.browserName.ToLower().Contains("remote"))
            {
                string parameters = "";
                if (Params != null)
                    parameters = " " + String.Join(" ", from arg in Params select (arg.Contains("=") ? "\"" + arg + "\"" : arg));

                //String strTCCommand = " -i " + SessionID + @" -c -f " + batchfilePath + " " + projectPath + " " + projectName + " " + unitName + " " + routineName + parameters;
                //String strTCCommand = " -i " + SessionID + " " + batchfilePath + " " + projectPath + " " + projectName + " " + unitName + " " + routineName + parameters;
                string FileName = DriverScript.TestRunner.testid + DateTime.Now.ToString("MMMddyyyyHHmmss") + ".bat";
                string TCBatchPath = @"\\"+Config.node+ @"\C$\Windows\Temp\"+ FileName;
                string TCBatchExecutionCommand = " -i " + SessionID + " " + @"C:\Windows\Temp\" + FileName;
                bp.CreateFile(TCBatchPath, batchfilePath + " " + projectPath + " " + projectName + " " + unitName + " " + routineName + parameters, Config.node, Config.WindowsUserName, "PQAte$t123-" + new Login().GetHostName(Config.node).ToLowerInvariant());
                //bp.ExecuteRemoteCommand(RemoteServer, Config.WindowsUserName, Config.WindowsPassword, strTCCommand);
                bp.ExecuteRemoteCommand(RemoteServer, Config.WindowsUserName, Config.WindowsPassword, TCBatchExecutionCommand);
                Thread.Sleep(2000);
            }
            else
            {
                object RoutineResult = null;
                try
                {
                    // Runs the test
                    if (Params == null)
                    {
                        IntegrationObject.RunRoutine(projectName, "ICAActionsXpath", routineName);
                    }
                    else
                    {
                        IntegrationObject.RunRoutineEx(projectName, "ICAActionsXpath", routineName, Params);
                    }


                    // Waits until testing is over
                    int runCounter = 0;
                    while (IntegrationObject.IsRunning() && runCounter < 70)
                    {
                        Thread.Sleep(10000);
                        runCounter++;
                    }

                        // Check the results
                        RoutineResult = IntegrationObject.RoutineResult;
                    if (RoutineResult != null)
                        Logger.Instance.InfoLog("Script routine returned " + RoutineResult.ToString());
                    else
                        Logger.Instance.InfoLog("Script routine did not return any result");

                    switch (IntegrationObject.GetLastResultDescription().Status)
                    {
                        case TestComplete.TC_LOG_STATUS.lsOk:
                            Logger.Instance.InfoLog("The test run finished successfully.");
                            break;
                        case TestComplete.TC_LOG_STATUS.lsWarning:
                            Logger.Instance.InfoLog("Warning messages were posted to the test log.");
                            break;
                        case TestComplete.TC_LOG_STATUS.lsError:
                            Logger.Instance.InfoLog("Error messages were posted to the test log.");
                            break;
                    }

                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    Logger.Instance.InfoLog("An exception occurred: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Invoke Test Complete Methods //Overload with returning results
        /// </summary>
        /// <param name="routineName"></param>
        /// <param name="Params"></param>
        /// /// <param name="getResultFrom"></param>
        public bool TCActions(string routineName, string[] Params, string getResultFrom)
        {
            object RoutineResult = null;
            bool returnResult = false;
            bool logResult = false;
            bp = new BasePage();
            if (bp.browserName.ToLower().Contains("remote"))
            {
                string parameters = "";
                if (Params != null)
                    parameters = " " + String.Join(" ", from arg in Params select (arg.Contains("=") ? "\"" + arg + "\"" : arg));

                String strTCCommand = " -i " + SessionID + @" - c -f " + batchfilePath + " " + projectPath + " " + projectName + " " + unitName + " " + routineName + parameters;
                returnResult = bp.ExecuteRemoteCommand(RemoteServer, Config.WindowsUserName, Config.WindowsPassword, strTCCommand);
                //Thread.Sleep(2000);
                return returnResult;
            }
            else
            {
                try
                {
                    // Runs the test
                    if (Params == null)
                    {
                        IntegrationObject.RunRoutine(projectName, "ICAActionsXpath", routineName);
                    }
                    else
                    {
                        IntegrationObject.RunRoutineEx(projectName, "ICAActionsXpath", routineName, Params);
                    }


                    // Waits until testing is over
                    while (IntegrationObject.IsRunning())

                        // Check the results
                        RoutineResult = IntegrationObject.RoutineResult;

                    if (RoutineResult != null)
                        Logger.Instance.InfoLog("Script routine returned " + RoutineResult.ToString());
                    else
                        Logger.Instance.InfoLog("Script routine did not return any result");

                    switch (IntegrationObject.GetLastResultDescription().Status)
                    {
                        case TestComplete.TC_LOG_STATUS.lsOk:
                            Logger.Instance.InfoLog("The test run finished successfully.");
                            logResult = true;
                            break;
                        case TestComplete.TC_LOG_STATUS.lsWarning:
                            Logger.Instance.InfoLog("Warning messages were posted to the test log.");
                            break;
                        case TestComplete.TC_LOG_STATUS.lsError:
                            Logger.Instance.InfoLog("Error messages were posted to the test log.");
                            break;
                    }
                    if (getResultFrom.ToLower().Equals("routine"))
                        returnResult = Convert.ToBoolean(RoutineResult);
                    else
                        returnResult = logResult;

                    return returnResult;
                }
                catch (System.Runtime.InteropServices.COMException ex)
                {
                    Logger.Instance.InfoLog("An exception occurred: " + ex.Message);
                    return returnResult;
                }
            }
        }

        /// <summary>
        /// Close Test Execute
        /// </summary>
        public void Closetestcomplete()
        {
            //bp = new BasePage();
            //if (!bp.browserName.ToLower().Contains("remote"))
            //{
                TestExecuteManager.Quit();
                
                Marshal.ReleaseComObject(IntegrationObject);
                Marshal.ReleaseComObject(TestExecuteManager);
               
                Thread.Sleep(5000);
                try
                {
                    KillProcessByName("TestExecute");
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Exception in closing the process TestExecute.EXE due to " + ex.Message);
                }
                try
                {
                    BasePage.KillProcessByPartialName("TestCompleteService");
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Exception in closing the process TestCompleteService*.EXE due to " + ex.Message);
                }
            //}
        }

        /// <summary>
        /// To kill the process
        /// </summary>
        /// <param name="processName"></param>
        public void KillProcessByName(string processName)
        {
            try
            {
                foreach (Process process in Process.GetProcessesByName(processName))
                {
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in closing the process " + processName + " due to " + ex.Message);
            }
        }

    }
}
