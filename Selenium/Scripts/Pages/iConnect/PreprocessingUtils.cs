using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.MergeServiceTool;
using System.Runtime.Serialization;
using System.Threading;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.InputDevices;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Keys = OpenQA.Selenium.Keys;
using Application = TestStack.White.Application;
using Window = TestStack.White.UIItems.WindowItems.Window;
using TestStack.White.WindowsAPI;
using System.IO;
using Selenium.Scripts.Pages;
using System.Data;
using System.Xml;
using ComboBox = TestStack.White.UIItems.ListBoxItems.ComboBox;
using Dicom.Network;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Data.SqlClient;
using Button = TestStack.White.UIItems.Button;
using CheckBox = TestStack.White.UIItems.CheckBox;
using ListView = TestStack.White.UIItems.ListView;
using RadioButton = TestStack.White.UIItems.RadioButton;
using TextBox = TestStack.White.UIItems.TextBox;
using Panel = TestStack.White.UIItems.Panel;
using TestStack.White.Configuration;
using System.Runtime.InteropServices;
using Selenium.Scripts.Pages.iConnect;


using System.Management;



namespace Selenium.Scripts.Pages.iConnect
{
    public class PreprocessingUtils : BasePage
    {
        public new  WpfObjects wpfobject;
        public NetStat netstart;
        public ServiceTool servicetool;
        public static String PreProcessingServiceName  = "PreprocessingService";
        public static string ImagePrefetchService = "ImagePrefetchService";
        public static String PreProcessingServiceDescription = "TBA";
        public static string PrefetchStoreXML = @"C:\WebAccess\WindowsService\ImagePrefetch\Config\PrefetchStoreScpServerConfiguration.xml";
        public static string PreprocessingConfigXML   = @"C:\WebAccess\WindowsService\Preprocessing\Config\PreprocessingConfiguration.xml";
        public static string PreprocessingConfigExe = @"C:\WebAccess\WindowsService\Preprocessing\bin\Preprocessing.exe.config";
        public static string logFilePath
        {
            get
            {
                DirectoryInfo di = new DirectoryInfo("C:\\Windows\\Temp");
                var file = di.GetFiles("WebAccessPreprocessingServiceDeveloper*")
                .OrderByDescending<FileInfo, DateTime>(fileinfo => fileinfo.LastWriteTime).
                First();
                return @"C:\Windows\Temp"+Path.DirectorySeparatorChar+file.Name;
            }
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public PreprocessingUtils()
        {

            netstart = new NetStat();
            servicetool = new ServiceTool();
            wpfobject = new WpfObjects();
        }

        /// <summary>
        /// This method will Restart the given windows service by its name
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns>bool - True is restart is successed, else false</returns>
        public bool RestartService(string serviceName)
        {

            ServiceController serviceController = new ServiceController(serviceName);
            try
            {
                if ((serviceController.Status.Equals(ServiceControllerStatus.Running)) || (serviceController.Status.Equals(ServiceControllerStatus.StartPending)))
                {
                    serviceController.Stop();
                }
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                serviceController.Start();
                serviceController.WaitForStatus(ServiceControllerStatus.Running);
            }
            catch
            {
                throw new Exception("Error while Restart the service "+ serviceName);
            }

            ServiceController sc = new ServiceController(serviceName);

            if (sc.Status.ToString() == "Running")
                return true;
            else
            {
                Logger.Instance.ErrorLog("Error Occured while restart the service "+ serviceName);
                return false;
            }

        }

        /// <summary>
        /// This method get the job row count from the DB
        /// </summary>
        /// <param name=""> </param>
        /// <returns></returns>
        public int GetJobcount(String jobType = "9")
        {
            //Get the Job table Count before Prefetch
            Thread.Sleep(2000);
            DataBaseUtil dbJob = new DataBaseUtil("sqlserver", "IRWSDB", InstanceName: "WEBACCESS");
            dbJob.ConnectSQLServerDB();
            string jobQuery = "select * from Job where JobTypeUid='"+jobType+"'";
            IList<String> jobBefore = dbJob.ExecuteQuery(jobQuery);
            Logger.Instance.InfoLog("Total Job with jobType "+jobType+" is " + jobBefore.Count);
            return jobBefore.Count;
        }

        /// <summary>
        /// This method get the job row count from the DB
        /// </summary>
        /// <param name=""> </param>
        /// <returns></returns>
        public IList<string> GetJob(String jobType = "9")
        {
            //Get the Job table Count before Prefetch
            DataBaseUtil db = new DataBaseUtil("sqlserver", "IRWSDB", InstanceName: "WEBACCESS");
            db.ConnectSQLServerDB();
            string jobQuery = "select status from Job where JobTypeUid='" + jobType + "'";
            IList<string> jobBefore = db.ExecuteQuery(jobQuery);
            Logger.Instance.InfoLog("Total Job with jobType " + jobType + " before prefetch is " + jobBefore.Count);
            return jobBefore;
        }

        /// <summary>
        /// This method get the job row count from the DB
        /// </summary>
        /// <param name=""> </param>
        /// <returns></returns>
        public IList<string> GetJobUID(String jobType = "9" )
        {
            //Get the Job table Count before Prefetch
            DataBaseUtil db = new DataBaseUtil("sqlserver", "IRWSDB", InstanceName: "WEBACCESS");
            db.ConnectSQLServerDB();
            string jobQuery = "select JobUid from Job where JobTypeUid='" + jobType + "' ";
            IList<string> jobUID = db.ExecuteQuery(jobQuery);
            return jobUID;
        }

        /// <summary>
        /// This method get the jobactions row count from the DB based in action name
        /// </summary>
        /// <param name="actionName"> Action Name </param>
        /// <returns></returns>
        public int GetJobActionCountFromDB(string actionName)
        {
            DataBaseUtil db = new DataBaseUtil("sqlserver", "IRWSDB", InstanceName: "WEBACCESS");
            db.ConnectSQLServerDB();
            string jobAction = null;
            if (actionName != null) 
                jobAction = "select * from JobAction where ActionName like '%" + actionName + "%'";
            else
                jobAction = "select * from JobAction";
            IList<String> jobActionBefore = db.ExecuteQuery(jobAction);
            Logger.Instance.InfoLog("Total Job action before prefetch is " + jobActionBefore.Count);
            return jobActionBefore.Count;
        }

        /// <summary>
        /// This method get the jobactions status from the DB based in action table
        /// </summary>
        /// <param name="actionName"> Action Name </param>
        /// <returns></returns>
        public string GetJobActionStatusFromDB(string jobUID, string actionName = null)
        {
            DataBaseUtil db = new DataBaseUtil("sqlserver", "IRWSDB", InstanceName: "WEBACCESS");
            db.ConnectSQLServerDB();
            string jobAction = null;
            Thread.Sleep(3000);
            if (actionName != null)
                jobAction = "select ActionStatus from JobAction where JobUid='" + jobUID + "' and ActionName like '%" + actionName + "%'";
            else
                jobAction = "select ActionStatus from JobAction where JobUid='"+jobUID+"' and ActionName like '%PriorStudyDigest%'";
            IList<String> jobActionBefore = db.ExecuteQuery(jobAction);
            Logger.Instance.InfoLog("Total Job action before prefetch is " + jobActionBefore.Count);
            return jobActionBefore[0];
        }

        /// <summary>
        /// This method get the 
        /// </summary>
        /// <param name="PF_Node"> PF node AE title</param>
        /// <returns></returns>
        public string GetJobStatusFromDB( string jobid )
        {
            DataBaseUtil db = new DataBaseUtil("sqlserver", "IRWSDB", InstanceName: "WEBACCESS");
            db.ConnectSQLServerDB();
            string status = "select Status from Job where JobUid = '" + jobid + "'";
            IList<String> jobStatus = db.ExecuteQuery(status);
            Logger.Instance.InfoLog("Job Status for the job Id"+ jobid + " is " + jobStatus[0]);
            return jobStatus[0];
        }    

        /// <summary>
        /// Sending Study from EA to Prefetch
        /// </summary>
        /// <returns></returns>
        public void PushStudyToPrefetch( string PF_Node )
        {
            WorkFlow EAPortal = new WorkFlow();
           string  currentHandler = BasePage.Driver.CurrentWindowHandle;

           EAPortal.HPSendStudy();

            // switch to new window
            foreach (string handle in BasePage.Driver.WindowHandles)
                if (!handle.Equals(currentHandler))
                    BasePage.Driver.SwitchTo().Window(handle);

            var NodeSelect = BasePage.Driver.FindElement(By.CssSelector("select[name = 'destinations']"));
            var selectElement = new SelectElement(NodeSelect);

            //select by value
            selectElement.SelectByValue(PF_Node); Thread.Sleep(2000);
            BasePage.Driver.FindElement(By.CssSelector("input[value = 'Send To Selected Remotes']")).Click(); Thread.Sleep(2000);

            BasePage.Driver.FindElement(By.CssSelector(".messagegreenlarge"));
            BasePage.Driver.FindElement(By.CssSelector("input[value = 'Close Window']")).Click();

            BasePage.Driver.SwitchTo().Window(currentHandler);

        }

        /// <summary>
        /// Create a new Dicom Study
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="Modality"></param>
        /// <param name="NewFolderLocationToSave"></param>
        /// <returns></returns>
        new public String CreateNewDicomStudy(String filename = "", string Modality = "CR", string NewFolderLocationToSave = null)
        {
            return base.CreateNewDicomStudy(filename, Modality, NewFolderLocationToSave);
        }        

        /// <summary>
        /// Pushing Dicom Study to EA
        /// </summary>
        public void PushStudytoEA(string studypath, string host, int port, bool useTls, string callingAe, string calledAe)
        {
            var client = new DicomClient();
            client.AddRequest(new DicomCStoreRequest(studypath));
            client.Send(host, port, useTls, callingAe , calledAe);
        }     

        /// <summary>
        /// This method returns Service Status
        /// </summary>
        /// <param name="servicename">Name of of the Windos Service</param>
        /// <returns>Current Status of the Service</returns>
        public String GetServiceStatus(String servicename)
        {
            ServiceController servicecontroller = new ServiceController(servicename);

            switch (servicecontroller.Status)
            {
                case ServiceControllerStatus.Running:
                    return "Running";
                case ServiceControllerStatus.Stopped:
                    return "Stopped";
                case ServiceControllerStatus.Paused:
                    return "Paused";
                case ServiceControllerStatus.StopPending:
                    return "Stopping";
                case ServiceControllerStatus.StartPending:
                    return "Starting";
                default:
                    return "Status Changing";
            }            

        }

        /// <summary>
        /// This method returns Service Controller object
        /// </summary>
        /// <param name="servicename">Name of of the Windos Service</param>
        /// <returns>Service Controller object for the Service</returns>
        public ServiceController GetServiceObject(String servicename)
        {
            ServiceController servicecontroller = new ServiceController(servicename);
            return servicecontroller;

        }

        /// <summary>
        /// This will return the Description of this Service
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public string GetServiceDescription(string serviceName)
        {
            using (ManagementObject service = new ManagementObject(new ManagementPath(string.Format("Win32_Service.Name='{0}'", serviceName))))
            {
                Logger.Instance.InfoLog("The Service Description is --" + service["Description"].ToString());
                return service["Description"].ToString();
            }
        }

        /// <summary>
        /// This method is to Stop the Service.
        /// </summary>
        /// <param name="servicename"></param>
        public void StopService(String servicename)
        {
            var preproceService  = this.GetServiceObject(servicename);
            preproceService.Stop();
        }

        /// <summary>
        /// This method is to Start the Service.
        /// </summary>
        /// <param name="servicename"></param>
        public void StartService(String servicename)
        {
            var preproceService = this.GetServiceObject(servicename);
            preproceService.Start();
        }

    }
}
