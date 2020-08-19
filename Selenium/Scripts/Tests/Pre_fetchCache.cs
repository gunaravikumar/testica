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
using Selenium.Scripts.Pages.MergeServiceTool;
using TestStack.White.Configuration;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;
using Application = TestStack.White.Application;
using System.Diagnostics;
using System.Xml.Serialization;
using OpenQA.Selenium.Remote;
using Selenium.Scripts.Pages.eHR;
using Dicom;
using Selenium.Scripts.Reusable.Generic;
using Dicom.Network;

namespace Selenium.Scripts.Tests
{
    class Pre_fetchCache:BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }       
        public ServiceTool servicetool { get; set; }
		public NetStat netstat { get; set; }
        BasePage basepage;
        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public Pre_fetchCache(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";       
            wpfobject = new WpfObjects();    
            servicetool = new ServiceTool();
            basepage = new BasePage();
        }
        public string remoteserverip = Config.IConnectIP2;

        /// <summary> 
        /// Pre-fetch Cache - Settings
        /// </summary>
        public TestCaseResult Test_66143(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            int executedSteps = -1;
            ServiceTool servicetool = new ServiceTool();
            var netstat = new NetStat();
            String cachepath = String.Empty;
            String iconnectHostName = String.Empty;
            String datasourceip = String.Empty;
            String datasourceHostName = String.Empty;
            String iconnectAETitle = String.Empty;

            try
            {
                //Setup Test Step Description
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                //Get Test Data                
                iconnectHostName = login.GetHostName(Config.IConnectIP);
                datasourceHostName = login.GetHostName(Config.DestEAsIp);
                cachepath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "CachePath");
                iconnectAETitle = login.GetHostName(Config.IConnectIP);
                cachepath = cachepath + Path.DirectorySeparatorChar + iconnectAETitle;
                String PortList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Ports");
                String dicomPort = PortList.Split(':')[0];
                String iCAPort = PortList.Split(':')[1];

                try
                {
                    //Delete existing cache study folders
                    if (Directory.Exists(cachepath))
                    {
                        BasePage.DeleteAllFileFolder(cachepath);        //Delete all files and folders inside cache Study folder
                        //Directory.Delete(cachepath, true);      //Delete Cache Study folder itself
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog(ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.InnerException);
                }

                #region No_Autonaion_Code_Commented
                // Step - 1 - Verify default settings are configured in the Pre-Fetch Cache in the dicom tab.
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService);
                servicetool.ClickModifyButton();

                bool IsDefaultValueSet = true;

                //Step-1: Verify Default values for Pre-fetch cache Service options
                CheckBox enablePreFetchCheckbox = servicetool.EnablePreFetchCheckbox();
                if (enablePreFetchCheckbox.Checked == false)
                {
                    IsDefaultValueSet = false;
                }

                //Local cache
                RadioButton localCacheRadioButton = servicetool.LocalCacheRadioButton();
                if (!(localCacheRadioButton.Enabled && localCacheRadioButton.IsSelected))
                {
                    IsDefaultValueSet = false;
                }

                //Enable Q/R SCU
                CheckBox enableQueryRetrieveSCU = servicetool.EnableQueryRetrieveCheckbox();
                if (enableQueryRetrieveSCU.Checked == false)
                {
                    IsDefaultValueSet = false;
                }

                //Polling time
                //TextBox pollingTime = servicetool.PollingInterval_txt();
                //if (!pollingTime.Enabled || pollingTime.Text != "10" || pollingTime.Text != "5")
                //{
                //    IsDefaultValueSet = false;
                //}

                ////Retrieve Time range
                //TextBox retrieveTimeRange = servicetool.RetrieveTimeRange_txt();
                //if (!retrieveTimeRange.Enabled || retrieveTimeRange.Text != "30" || retrieveTimeRange.Text != "60")
                //{
                //    IsDefaultValueSet = false;
                //}

                ////QC Completed Time
                //TextBox CompletedTime = servicetool.QC_CompletedTime_txt();
                //if (!CompletedTime.Enabled || CompletedTime.Text != "0")
                //{
                //    IsDefaultValueSet = false;
                //}

                ////AE title
                //TextBox AEtitle = servicetool.SCP_AEtitle_txt();
                //if (!AEtitle.Enabled || AEtitle.Text != @"PF_{LOCALHOST_UPPERCASE}" || AEtitle.Text != @"PF_" + Config.IConnectIP)
                //{
                //    IsDefaultValueSet = false;
                //}

                ////SCP Port
                //TextBox SCPport = servicetool.SCPport_txt();
                //if (!SCPport.Enabled || SCPport.Text == dicomPort)
                //{
                //    IsDefaultValueSet = false;
                //    Logger.Instance.ErrorLog("SCP Port is not '" + dicomPort + "' by default instead it shown as " + SCPport.Text);
                //}

                ////Cleanup Threshold
                //TextBox cleanupThreshold = servicetool.CleanupThreshold_txt();
                //if (!cleanupThreshold.Enabled || cleanupThreshold.Text != "24" || cleanupThreshold.Text != "60")
                //{
                //    IsDefaultValueSet = false;
                //    Logger.Instance.ErrorLog(@"Cleanup Threshold is not 24(hours) or Environment steup value 60  by default instead it is in " + cleanupThreshold.Text + "(hours)");
                //}

                ////clean-up intreval
                //TextBox cleanupInterval = servicetool.CleanupInterval_txt();
                //if (!(cleanupInterval.Enabled || cleanupInterval.Text != "10" || cleanupInterval.Text != "60"))
                //{
                //    IsDefaultValueSet = false;
                //    Logger.Instance.ErrorLog(@"Cleanup Interval is not 10(minutes) or Environment steup value 60 by default instead it is in " + cleanupInterval.Text + "(minutes)");
                //}

                ////Cleanup High Water Mark
                //TextBox cleanupHighWatermark = servicetool.CleanupHighWatermark_txt();
                //if (!cleanupHighWatermark.Enabled || cleanupHighWatermark.Text != "80")
                //{
                //    IsDefaultValueSet = false;
                //    Logger.Instance.ErrorLog(@"Cleanup High Water Mark Level is not 80(%) by default instead it is in " + cleanupHighWatermark.Text + "(%)");
                //}

                ////Cleanup Low Water Mark
                //TextBox cleanupLowWatermark = servicetool.CleanupLowWatermark_txt();
                //if (!cleanupLowWatermark.Enabled || cleanupLowWatermark.Text != "50")
                //{
                //    IsDefaultValueSet = false;
                //    Logger.Instance.ErrorLog(@"Cleanup Low Water Mark Level is not 50(%) by default instead it is in " + cleanupLowWatermark.Text + "(%)");
                //}


                //// Local Web Service Port
                //TextBox localPort = servicetool.LocalPort_txt();
                //if (!localPort.Enabled || localPort.Text != iCAPort)
                //{
                //    result.steps[executedSteps].statuslist.Add("Fail");
                //    Logger.Instance.ErrorLog("Local Web Service Port is not '" + iCAPort + "' by default instead it shown as " + localPort.Text);
                //}
                //else
                //{
                //    result.steps[executedSteps].statuslist.Add("Pass");
                //}

                ////Remote cache
                //RadioButton RemoteCacheRadioButton = servicetool.RemoteCacheRadioButton();
                //if (RemoteCacheRadioButton.Enabled && RemoteCacheRadioButton.IsSelected)
                //{
                //    result.steps[executedSteps].statuslist.Add("Fail");
                //    Logger.Instance.ErrorLog("Remote Cache Service Radio button is selected by default");
                //}
                //else
                //{
                //    result.steps[executedSteps].statuslist.Add("Pass");
                //}

                if ( IsDefaultValueSet && !result.steps[++executedSteps].statuslist.Contains("Fail"))
                {
                    result.steps[executedSteps].status = "Pass";   //Step-1
                    Logger.Instance.InfoLog("Default settings are configured for Pre-fetch Cache Service");
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Default settings are not configured for Pre-fetch Cache Service");
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                #endregion No_Autonaion_Code_Commented

                //result.steps[++executedSteps].status = "No Automation"; //Step 1

                //Step-2: Change Pre-fetch Cache Service to local
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService);
                servicetool.ClickModifyButton();
                servicetool.EnablePrefetchCache(cachetype: "Local", AEtitle: iconnectAETitle, pollingtime: 5, timerange: 60, cleanupthreshold: 1);
                servicetool.RestartService();
                result.steps[++executedSteps].StepPass();   //Step-2

                //Step-3 - Add ICA Server as dicom device in DICOM Archive with port 4446
                result.steps[++executedSteps].StepPass();    //Step-3

                //Step-4 - Add Dicom Data source and Enable Prefetch cache - Datasource
                if (!servicetool.IsDataSourceExists(datasourceHostName))
                {
                    servicetool.AddEADatasource(Config.DestEAsIp, Config.DestEAsAETitle, "");
                }
                servicetool.EnableCacheForDataSource(datasourceHostName, true);
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                result.steps[++executedSteps].StepPass();    //Step-4

                //Step-5 - Verify iConnect Access Image Pre-fetch Service is running
                bool PreFetchService = wpfobject.ServiceStatus("iConnect Access Image Pre-fetch Service", "Running");
                if (PreFetchService)
                {
                    result.steps[++executedSteps].status = "Pass";   //Step-5
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                    throw new Exception("iCA Pre-fetch Service is not running");
                }

                //Step-6_1: Verify iCA Port is open and Listening
                bool boolPortFound = false;
                result.steps[++executedSteps].StepPass();
                List<NetStat.Port> portList = netstat.GetNetStatPorts();
                for (int i = 0; i < portList.Count && !boolPortFound; i++)
                {
                    if (portList[i].port_number == iCAPort)
                    {
                        boolPortFound = true;
                        if (portList[i].state != "LISTENING")
                        {
                            result.steps[executedSteps].statuslist.Add("Fail");
                            Logger.Instance.ErrorLog("iCA port '" + iCAPort + "' is open but not listening. Actual State: " + portList[i].state);
                            result.steps[executedSteps].SetLogs();
                        }
                    }
                }
                if (boolPortFound)
                {
                    result.steps[executedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("iCA port '" + iCAPort + "' is open and listening");
                }
                else
                {
                    result.steps[executedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("iCA port " + iCAPort + " is not open");
                    result.steps[executedSteps].SetLogs();
                }

                //Step-6_2: Verify dicom Port is open and Listening
                boolPortFound = false;
                portList = netstat.GetNetStatPorts();
                for (int i = 0; i < portList.Count && !boolPortFound; i++)
                {
                    if (portList[i].port_number == dicomPort)
                    {
                        boolPortFound = true;
                        if (portList[i].state != "LISTENING")
                        {
                            result.steps[executedSteps].statuslist.Add("Fail");
                            Logger.Instance.ErrorLog("Dicom port '" + dicomPort + "' is open but not listening. Actual State: " + portList[i].state);
                            result.steps[executedSteps].SetLogs();
                        }
                    }
                }
                if (boolPortFound)
                {
                    result.steps[executedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Dicom port '" + dicomPort + "' is open and listening");
                }
                else
                {
                    result.steps[executedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Dicom port " + dicomPort + " is not open");
                    result.steps[executedSteps].SetLogs();
                }

                if (result.steps[executedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[executedSteps].status = "Fail";
                }
                else
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }

                //Step-7 -  Disable Prefetch cache Service
                servicetool.LaunchServiceTool();
                servicetool.DisablePrefetchCache();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                result.steps[++executedSteps].StepPass();  //Step-7

                //Step-8 - Verify iConnect Access Image Pre-fetch Service is running
                Thread.Sleep(5000);     //Wait for Service to restart
                PreFetchService = wpfobject.ServiceStatus("iConnect Access Image Pre-fetch Service", "Running");
                if (PreFetchService)
                {
                    result.steps[++executedSteps].status = "Pass";   //Step-8
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                    throw new Exception("iCA Pre-fetch Service is not running");
                }

                //Step-9_1: Verify iCA Port is open and Listening
                boolPortFound = false;
                result.steps[++executedSteps].StepPass();
                Thread.Sleep(8000); //Wait for port to close
                portList = netstat.GetNetStatPorts();
                for (int i = 0; i < portList.Count && !boolPortFound; i++)
                {
                    if (portList[i].port_number == iCAPort)
                    {
                        boolPortFound = true;
                        if (portList[i].state == "LISTENING")
                        {
                            result.steps[executedSteps].statuslist.Add("Fail");
                            Logger.Instance.ErrorLog("iCA port '" + iCAPort + "' is open and listening after Pre-fetch Cache service is disabled");
                            result.steps[executedSteps].SetLogs();
                        }
                    }
                }
                if (!boolPortFound)
                {
                    result.steps[executedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("iCA port '" + iCAPort + "' is not listed as open and listening");
                }
                else
                {
                    result.steps[executedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("iCA port " + iCAPort + " is open after Pre-fetch cache service is disabled");
                    result.steps[executedSteps].SetLogs();
                }


                //Step-9_2: Verify dicom Port is open and Listening
                boolPortFound = false;
                portList = netstat.GetNetStatPorts();
                for (int i = 0; i < portList.Count && !boolPortFound; i++)
                {
                    if (portList[i].port_number == dicomPort)
                    {
                        boolPortFound = true;
                        if (portList[i].state == "LISTENING")
                        {
                            result.steps[executedSteps].statuslist.Add("Fail");
                            Logger.Instance.ErrorLog("Dicom port '" + dicomPort + "' is open and listening after Pre-fetch Cache service is disabled");
                            result.steps[executedSteps].SetLogs();
                        }
                    }
                }
                if (!boolPortFound)
                {
                    result.steps[executedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Dicom port '" + dicomPort + "' is not listed as open and listening");
                }
                else
                {
                    result.steps[executedSteps].statuslist.Add("Pass");
                    Logger.Instance.ErrorLog("Dicom port " + dicomPort + " is open after Pre-fetch cache service is disabled");
                    result.steps[executedSteps].SetLogs();
                }

                if (result.steps[executedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[executedSteps].status = "Fail";
                }
                else
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }

                //Step-10 -  Enable Prefetch cache - Datasource
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService);
                servicetool.ClickModifyButton();
                servicetool.EnablePrefetchCache(cachetype: "Local");
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                result.steps[++executedSteps].StepPass();   //Step-10

                //Step-11 - Verify iConnect Access Image Pre-fetch Service is running
                PreFetchService = wpfobject.ServiceStatus("iConnect Access Image Pre-fetch Service", "Running");
                if (PreFetchService)
                {
                    result.steps[++executedSteps].status = "Pass";   //Step-11
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                    throw new Exception("iCA Pre-fetch Service is not running");
                }

                //Step-12_1: Verify iCA Port is open and Listening
                boolPortFound = false;
                result.steps[++executedSteps].StepPass();
                portList = netstat.GetNetStatPorts();
                for (int i = 0; i < portList.Count && !boolPortFound; i++)
                {
                    if (portList[i].port_number == iCAPort)
                    {
                        boolPortFound = true;
                        if (portList[i].state != "LISTENING")
                        {
                            result.steps[executedSteps].statuslist.Add("Fail");
                            Logger.Instance.ErrorLog("iCA port '" + iCAPort + "' is open but not listening. Actual State: " + portList[i].state);
                            result.steps[executedSteps].SetLogs();
                        }
                    }
                }
                if (boolPortFound)
                {
                    result.steps[executedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("iCA port '" + iCAPort + "' is open and listening");
                }
                else
                {
                    result.steps[executedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("iCA port " + iCAPort + " is not open");
                    result.steps[executedSteps].SetLogs();
                }

                //Step-12_2: Verify dicom Port is open and Listening
                boolPortFound = false;
                portList = netstat.GetNetStatPorts();
                for (int i = 0; i < portList.Count && !boolPortFound; i++)
                {
                    if (portList[i].port_number == dicomPort)
                    {
                        boolPortFound = true;
                        if (portList[i].state != "LISTENING")
                        {
                            result.steps[executedSteps].statuslist.Add("Fail");
                            Logger.Instance.ErrorLog("Dicom port '" + dicomPort + "' is open but not listening. Actual State: " + portList[i].state);
                            result.steps[executedSteps].SetLogs();
                        }
                    }
                }
                if (boolPortFound)
                {
                    result.steps[executedSteps].statuslist.Add("Pass");
                    Logger.Instance.InfoLog("Dicom port '" + dicomPort + "' is open and listening");
                }
                else
                {
                    result.steps[executedSteps].statuslist.Add("Fail");
                    Logger.Instance.ErrorLog("Dicom port " + dicomPort + " is not open");
                    result.steps[executedSteps].SetLogs();
                }

                if (result.steps[executedSteps].statuslist.Any<String>(status => status == "Fail"))
                {
                    result.steps[executedSteps].status = "Fail";
                }
                else
                {
                    result.steps[executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }

                //Close Serivce Manager
                result.steps[++executedSteps].StepPass();  //Step-13

                //Step-14 Verify pre-fetch Cache folder is empty
                bool isCacheFolderEmpty = BasePage.IsDirectoryEmpty(cachepath);
                if (isCacheFolderEmpty)
                {
                    result.steps[++executedSteps].status = "Pass";   //Step-14
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Report Result
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary>
        /// 5.0 Cache disable\enable
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_66146(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables     
            TestCaseResult result;
            result = new TestCaseResult(stepcount);


            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int Executedsteps = -1;

            try
            {

                //Step-1:Open the ICA service tool and select the Datasource tab
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                servicetool.NavigateToConfigToolDataSourceTab();
                Thread.Sleep(1500);
                Executedsteps++;


                //Step-2:Click on the ADD button to open the new datasource screen and select dicom from the TYPE dropdown.
                WpfObjects._mainWindow.Get<Button>(SearchCriteria.ByText("Add")).Click();
                Thread.Sleep(1500);
                wpfobject.GetMainWindowByTitle("Create a data source");
                Thread.Sleep(1500);
                servicetool.SetDataSourceType("2");
                Executedsteps++;

                //Step-3:Go to the Dicom tab and select the Enable Pre-Fetch Cache square.
                servicetool.NavigateToDicomTab();
                ITabPage DicomTab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
                CheckBox PrefetchCache = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(DicomTab, "Enable Pre-fetch Cache", 1);
                //CheckBox PrefetchCache = wpfobject.GetCheckBox("Enable Pre-fetch Cache");
                PrefetchCache.Checked = true;
                if (PrefetchCache.Checked)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-4:Unselect the  Instance Query Support square Instance Query Support
                CheckBox InstanceQuerySupport = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(DicomTab, "Instance Query Support", 1);
                InstanceQuerySupport.Checked = false;
                CheckBox PrefetchCache_4 = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(DicomTab, "Enable Pre-fetch Cache", 1);
                if (!(PrefetchCache_4.Enabled))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-5:Reselect the Instance Query Support square
                InstanceQuerySupport.Checked = true;
                CheckBox PrefetchCache_5 = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(DicomTab, "Enable Pre-fetch Cache", 1);
                if (PrefetchCache_5.Enabled)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }


                //Step-6:Select the Generic Tab and from the Type dropdown select the VNA option then go to the Dicom tab.
                servicetool.NavigateToDataSourceGenericTab();
                servicetool.SetDataSourceType("VNA", byIndex: 0, byoption: 1);
                servicetool.NavigateToDicomTab();
                DicomTab = WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab;
                CheckBox PrefetchCache_6 = wpfobject.GetAnyUIItem<ITabPage, CheckBox>(DicomTab, "Enable Pre-fetch Cache", 1);
                if (!(PrefetchCache_6.Enabled))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                servicetool.CloseConfigTool();

                //Report Result
                result.FinalResult(Executedsteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);


                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, Executedsteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);


                //Return Result
                return result;
            }
        }

        /// <summary> 
        /// 2.0 Local Cache Service
        /// </summary>
        public TestCaseResult Test_164717(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            int executedSteps = -1;
            var servicetool = new ServiceTool();
            String cachepath = String.Empty;
            String datasource = String.Empty;
            String datasourceip = String.Empty;
            String studypaths = String.Empty;
            List<String> PatientIDs = new List<String>();
            List<String> Accessions = new List<String>();
            List<String> SopUID = new List<String>();
            List<String> Seriesuid = new List<String>();
            List<String> StudyUID = new List<String>();
            List<String> Cachedstudypath = new List<String>();
            List<String> Cachedstudyname = new List<String>();
            List<String> Demographicsxmlpath = new List<String>();

            try
            {
                //Setup Test Step Description
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                BluRingViewer viewer = new BluRingViewer();
                StudyViewer OldViewer = new StudyViewer();

                //Get Test Data   
                String AETitle = login.PrefetchAETitle;
                datasource = login.GetHostName(Config.DestEAsIp);
                studypaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] studypath = studypaths.Split('=');
                cachepath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "CachePath");
                datasourceip = Config.DestEAsIp;
                cachepath = cachepath + Path.DirectorySeparatorChar + AETitle;
                //String AETitle = "PF_" + login.GetHostName(Config.IConnectIP);
                for (int i = 0; i <= 4; i++)
                {
                    String PID = BasePage.ReadDicomFile<String>(studypath[i], DicomTag.PatientID);
                    PatientIDs.Add(PID);
                    String Acc = BasePage.ReadDicomFile<String>(studypath[i], DicomTag.AccessionNumber);
                    Accessions.Add(Acc);
                    String sopuid = BasePage.ReadDicomFile<String>(studypath[i], DicomTag.SOPInstanceUID);
                    SopUID.Add(sopuid);
                    String studyuid = BasePage.ReadDicomFile<String>(studypath[i], DicomTag.StudyInstanceUID);
                    StudyUID.Add(studyuid);
                    String seriesuid = BasePage.ReadDicomFile<String>(studypath[i], DicomTag.SeriesInstanceUID);
                    Seriesuid.Add(seriesuid);
                    String cachestudypath = cachepath + Path.DirectorySeparatorChar + PID + "-" + studyuid + Path.DirectorySeparatorChar + seriesuid;
                    Cachedstudypath.Add(cachestudypath);
                    String cachedstudyname = sopuid + "." + "dcm";
                    Cachedstudyname.Add(cachedstudyname);
                    String demographicsxmlpath = cachepath + Path.DirectorySeparatorChar + PID + "-" + studyuid + Path.DirectorySeparatorChar + "demographics.xml";
                    Logger.Instance.InfoLog("Demographics XML path :- " + i + " -- " + demographicsxmlpath);
                    Demographicsxmlpath.Add(demographicsxmlpath);
                }

                //Preconditions
                try
                {
                    String Lastnames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                    String[] LastNameList = Lastnames.Split(':');
                    var hplogin = new HPLogin();
                    BasePage.Driver.Navigate().GoToUrl("https://" + datasourceip + "/webadmin");
                    var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                    var workflow = (WorkFlow)hphome.Navigate("Workflow");
                    workflow.NavigateToLink("Workflow", "Archive Search");
                    foreach (String lastnme in LastNameList)
                    {
                        workflow.HPSearchStudy("Lastname", lastnme);
                        if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                            workflow.HPDeleteStudy();
                    }
                    hplogin.LogoutHPen();
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Error while Delete study from EA" + ex.Message);
                }

                //Enable Prefetch Cache
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                servicetool.NavigateSubTab("Pre-fetch Cache Service");
                servicetool.ClickModifyButton();
                servicetool.EnablePrefetchCache(cachetype: "Local", pollingtime: 5, timerange: 60, cleanupthreshold: 60, AEtitle: AETitle);
                servicetool.RestartService();

                //Enable Prefetch cache - Datasource
                servicetool.EnableCacheForDataSource(datasource);
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                //Delete existing study folders
                BasePage.DeleteAllFileFolder(cachepath);
                //Updating the Dicom files
                String patient = BasePage.ReadDicomFile<String>(studypath[0], DicomTag.PatientName).Replace("^", " ");
                String lastname_2 = patient.Split(' ')[0];
                String firstname_2 = patient.Split(' ')[1];
                var file1 = BasePage.WriteDicomFile(studypath[0], new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime }, new String[] { DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 20, 0)).ToString("HHmmss")) }, "File1");
                String patient1 = BasePage.ReadDicomFile<String>(studypath[1], DicomTag.PatientName).Replace("^", " ");
                String lastname_1 = patient1.Split(' ')[0];
                String firstname_1 = patient1.Split(' ')[1];
                var file2 = BasePage.WriteDicomFile(studypath[1], new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime }, new String[] { DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 20, 0)).ToString("HHmmss")) }, "File2");

                //Step-1 - Using the pummel tool send 2 studies to an archive that has been enabled for caching, for example ENIGMA an ISTORE configured as a dicom archive.
                var client = new DicomClient();
                client.AddRequest(new DicomCStoreRequest(file1));
                client.Send(datasourceip, 12000, false, "SCU",  Config.DestEAsAETitle );
                client.AddRequest(new DicomCStoreRequest(file2));
                client.Send(datasourceip, 12000, false, "SCU", Config.DestEAsAETitle);
                executedSteps++;

                //Step-2  -In the iCoonect server search for these studies in the DICOM archive.               
                BasePage.Driver.Navigate().GoToUrl(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var studies = login.Navigate<Studies>();
                studies.SearchStudy( AccessionNo : Accessions[0], Datasource: GetHostName(Config.DestEAsIp));
                studies.SelectStudy("Accession", Accessions[0]);
                executedSteps++;

                String studytime;
                string AM_PM;
                bool Step3 = false;
                //Step-3 - Load one study and take note of the Patient name and Patient ID.            
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                     viewer = BluRingViewer.LaunchBluRingViewer();
                    studytime = BasePage.FindElementByCss(BluRingViewer.div_studypaneltime).Text.Split(' ')[0];
                    Step3 = (BasePage.FindElementByCss(BluRingViewer.p_PatientName).Text.Split(',')[0].Equals(lastname_2) && BasePage.FindElementByCss(BluRingViewer.p_PatientName).Text.Split(',')[1].Trim().Equals(firstname_2));

                    }
                else
                {
                    OldViewer= studies.LaunchStudy();
                    Dictionary<String, String> detail = OldViewer.StudyDetailsInViewer();
                    studytime = OldViewer.StudyInfo().Split(',')[1].Split(' ')[2];
                    AM_PM = OldViewer.StudyInfo().Split(',')[1].Split(' ')[3];
                    Step3 = (OldViewer.PatientDetailsInViewer()["LastName"].Equals(lastname_2) && OldViewer.PatientDetailsInViewer()["FirstName"].Equals(firstname_2));
                }

                if (Step3)
                {
                    result.steps[++executedSteps].StepPass();
                }
                else
                {
                    result.steps[++executedSteps].StepFail();
                }


                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                    viewer.CloseBluRingViewer();
                else
                    viewer.CloseStudy();
                
                login.Logout();

                //Step-4 - Open the File Explorer on the iConnect server,  go to the Dive:\cache\PF_Server after 5 min the two studies sent in STEP 
                //Cache2.1 will be listed, confirm the demographics by opening the demographics.xml file located in a subdirectory in the cached study.
                var cachewait = new DefaultWait<IWebDriver>(BasePage.Driver);
                cachewait.Timeout = new TimeSpan(0, 10, 0);
                cachewait.IgnoreExceptionTypes(new Type[] { new FileNotFoundException().GetType() });
                cachewait.Until<Boolean>((d) =>
                {
                    var isFileFound = BasePage.CheckFile(Cachedstudyname[0], Cachedstudypath[0], "dcm");
                    if (isFileFound)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                cachewait.Until<Boolean>((d) =>
                {
                    var isFileFound = BasePage.CheckFile(Cachedstudyname[1], Cachedstudypath[1], "dcm");
                    if (isFileFound)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                var patientname1 = ReadXML.ReadAttribute(Demographicsxmlpath[0], "Study", "name");
                var lastname1 = patientname1.Split('^')[0].Replace(" ", "");
                var firstname1 = patientname1.Split('^')[1].Replace(" ", "");
                var patientname2 = ReadXML.ReadAttribute(Demographicsxmlpath[1], "Study", "name");
                var lastname2 = patientname2.Split('^')[0].Replace(" ", "");
                var firstname2 = patientname2.Split('^')[1].Replace(" ", "");
                if (lastname1.Equals(lastname_2) && firstname1.Equals(firstname_2) &&
                    lastname2.Equals(lastname_1) && firstname2.Equals(firstname_1))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-5-Send another 3 studies to the DICOM archive   
                String patient3 = BasePage.ReadDicomFile<String>(studypath[2], DicomTag.PatientName).Replace("^", " ");
                String lastname_3 = patient3.Split(' ')[0];
                String firstname_3 = patient3.Split(' ')[1];
                var file3 = BasePage.WriteDicomFile(studypath[2], new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime }, new String[] { DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 20, 0)).ToString("HHmmss")) }, "File3");
                String patient4 = BasePage.ReadDicomFile<String>(studypath[3], DicomTag.PatientName).Replace("^", " ");
                String lastname_4 = patient4.Split(' ')[0];
                String firstname_4 = patient4.Split(' ')[1];
                var file4 = BasePage.WriteDicomFile(studypath[3], new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime }, new String[] { DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 20, 0)).ToString("HHmmss")) }, "File4");
                String patient5 = BasePage.ReadDicomFile<String>(studypath[4], DicomTag.PatientName).Replace("^", " ");
                String lastname_5 = patient5.Split(' ')[0];
                String firstname_5 = patient5.Split(' ')[1];
                var file5 = BasePage.WriteDicomFile(studypath[4], new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime }, new String[] { DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 20, 0)).ToString("HHmmss")) }, "File5");

                client = new DicomClient();
                client.AddRequest(new DicomCStoreRequest(file3));
                client.Send(datasourceip, 12000, false, "SCU", Config.DestEAsAETitle);
                client.AddRequest(new DicomCStoreRequest(file4));
                client.Send(datasourceip, 12000, false, "SCU", Config.DestEAsAETitle);
                client.AddRequest(new DicomCStoreRequest(file5));
                client.Send(datasourceip, 12000, false, "SCU", Config.DestEAsAETitle);
                executedSteps++;

                //Step-6-Open the File Explorer on the iConnect server,  go to the Drive:\cache\PF_Server after 5 min the 3 studies sent in STEP  
                //Cache2.5 will be listed, confirm the demographics by opening the demographics.xml file.
                cachewait = new DefaultWait<IWebDriver>(BasePage.Driver);
                cachewait.Timeout = new TimeSpan(0, 30, 0);
                cachewait.IgnoreExceptionTypes(new Type[] { new FileNotFoundException().GetType() });

                cachewait.Until<Boolean>((d) =>
                {
                    var isFileFound = BasePage.CheckFile(Cachedstudyname[2], Cachedstudypath[2], "dcm");
                    if (isFileFound)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                cachewait.Until<Boolean>((d) =>
                {
                    var isFileFound = BasePage.CheckFile(Cachedstudyname[3], Cachedstudypath[3], "dcm");
                    if (isFileFound)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                cachewait.Until<Boolean>((d) =>
                {
                    var isFileFound = BasePage.CheckFile(Cachedstudyname[4], Cachedstudypath[4], "dcm");
                    if (isFileFound)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                var patientname3 = ReadXML.ReadAttribute(Demographicsxmlpath[2], "Study", "name");
                var lastname3 = patientname3.Split('^')[0].Replace(" ", "");
                var firstname3 = patientname3.Split('^')[1].Replace(" ", "");
                var patientname4 = ReadXML.ReadAttribute(Demographicsxmlpath[3], "Study", "name");
                var lastname4 = patientname4.Split('^')[0].Replace(" ", "");
                var firstname4 = patientname4.Split('^')[1].Replace(" ", "");
                var patientname5 = ReadXML.ReadAttribute(Demographicsxmlpath[4], "Study", "name");
                var lastname5 = patientname5.Split('^')[0].Replace(" ", "");
                var firstname5 = patientname5.Split('^')[1].Replace(" ", "");
                if (lastname3.Equals(lastname_3) && firstname3.Equals(firstname_3) &&
                    lastname4.Equals(lastname_4) && firstname4.Equals(firstname_4) &&
                    lastname5.Equals(lastname_5) && firstname5.Equals(firstname_5))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-7-Confirm the time in the demographics.xml should be consistent with the time they were sent. Example:""cm_20130425.114940^^^^""the file was sent at 11:49:40 AM"               
                var time = ReadXML.ReadAttribute(Demographicsxmlpath[0], "Study", "time");
                string convertedtime;
                if (studytime.Contains("AM") && studytime.Contains("12"))
                {
                    convertedtime = studytime.Replace("12", "00");
                }
                else
                {
                    convertedtime = studytime;
                }
                if (time.Replace(".", "").Contains(convertedtime.Replace(":", "")))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Report Result
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                String Lastnames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String[] LastNameList = Lastnames.Split(':');
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + datasourceip + "/webadmin");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                foreach (String lastnme in LastNameList)
                {
                    workflow.HPSearchStudy("Lastname", lastnme);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                        workflow.HPDeleteStudy();
                }
                hplogin.LogoutHPen();
            }

        }

        /// <summary>
        /// 6.0 Cache Studies preferred
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164720(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables     
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int Executedsteps = -1;
            try
            {
                //Get Test Data
                String AETitle = login.PrefetchAETitle;
                String datasource = login.GetHostName(Config.DestEAsIp);
                String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String cachepath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "CachePath");
                String datasourceip = Config.DestEAsIp;
                cachepath = cachepath + Path.DirectorySeparatorChar + AETitle;
                String PID = BasePage.ReadDicomFile<String>(studypath, DicomTag.PatientID);
                String studyinstanceuid = BasePage.ReadDicomFile<String>(studypath, DicomTag.StudyInstanceUID);
                String seriesuid = BasePage.ReadDicomFile<String>(studypath, DicomTag.SeriesInstanceUID);
                String sopuid = BasePage.ReadDicomFile<String>(studypath, DicomTag.SOPInstanceUID);
                String cachedstudypath = cachepath + Path.DirectorySeparatorChar + PID + "-" + studyinstanceuid + Path.DirectorySeparatorChar + seriesuid;
                String cachedstudyname = sopuid + "." + "dcm";
                String demographicsxmlpath = cachepath + Path.DirectorySeparatorChar + PID + "-" + studyinstanceuid + Path.DirectorySeparatorChar + "demographics.xml";
                String accession = BasePage.ReadDicomFile<String>(studypath, DicomTag.AccessionNumber);
                String PrimaryCachePath = @"C:\Windows\Temp\WebAccessP10FilesCache\" + login.GetHostName(Config.IConnectIP);

                //Preconditions
                //Enable Prefetch Cache
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                servicetool.NavigateSubTab("Pre-fetch Cache Service");
                servicetool.ClickModifyButton();
                servicetool.EnablePrefetchCache(cachetype: "Local", pollingtime: 2, timerange: 60, cleanupthreshold: 50, AEtitle: AETitle);
                servicetool.RestartService();

                //Enable Prefetch cache - Datasource
                servicetool.EnableCacheForDataSource(datasource);
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                //Clear cache Drive
                BasePage.DeleteAllFileFolder(cachepath);

                // Update Dicom study date and study time                
                String patient = BasePage.ReadDicomFile<String>(studypath, DicomTag.PatientName).Replace("^", " ");
                String lastname_2 = patient.Split(' ')[0];
                String firstname_2 = patient.Split(' ')[1];
                var file1 = BasePage.WriteDicomFile(studypath, new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime }, new String[] { DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 30, 0)).ToString("HHmmss")) }, "File1");

                //Send Dicom study to datasource              
                var client = new DicomClient();
                client.AddRequest(new DicomCStoreRequest(file1));
                client.Send(datasourceip, 12000, false, "SCU", Config.DestEAsAETitle);

                //Wait till study present in cache
                var cachewait = new DefaultWait<IWebDriver>(BasePage.Driver);
                cachewait.Timeout = new TimeSpan(0, 10, 0);
                cachewait.IgnoreExceptionTypes(new Type[] { new FileNotFoundException().GetType() });
                cachewait.Until<Boolean>((d) =>
                {
                    var isFileFound = BasePage.CheckFile(cachedstudyname, cachedstudypath, "dcm");
                    if (isFileFound)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });

                //Step-1:Close all ICA browsers.
                Executedsteps++;

                //Step-2:Login into ICA and search for the Patient Name viewed above.
                BasePage.Driver.Navigate().GoToUrl(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var studies = login.Navigate<Studies>();
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy("Accession", accession);
                Executedsteps++;

                var BlueRingviewer = new BluRingViewer();
                var StudyViewer = new StudyViewer();
                bool step3 = false;
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                {
                    BlueRingviewer = BluRingViewer.LaunchBluRingViewer();
                    step3 = (BasePage.FindElementByCss(BluRingViewer.p_PatientName).Text.Split(',')[0].Equals(lastname_2) && BasePage.FindElementByCss(BluRingViewer.p_PatientName).Text.Split(',')[1].Trim().Equals(firstname_2));

                }
                else
                {
                    StudyViewer = studies.LaunchStudy();
                    step3 = (StudyViewer.PatientDetailsInViewer()["LastName"].Equals(lastname_2) && StudyViewer.PatientDetailsInViewer()["FirstName"].Equals(firstname_2));
                }
                if (step3)
                {
                    result.steps[++Executedsteps].StepPass();
                }
                else
                {
                    result.steps[++Executedsteps].StepFail();
                }
                if (Config.isEnterpriseViewer.ToLower().Equals("y"))
                    BlueRingviewer.CloseBluRingViewer();
                else
                    StudyViewer.CloseStudy();

                login.Logout();

                //Step-4:Using windows explorer open the folder Drive:\temp\WebAccessP10FilesCache\ICA_server
                bool IsFileExist = BasePage.CheckFile(cachedstudyname, PrimaryCachePath, "dcm");
                if (!IsFileExist)
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }

                //Step-5:Open the window  Task Manager and locate the  two W3WP.exe tasks, right click and select"End Process".
                BasePage.Kill_EXEProcess("w3wp");
                Executedsteps++;

                //Step-6:Go to the Drive:\temp\WebAccessP10FilesCache\server name and delete all of the entries, Keep this window open
                BasePage.DeleteAllFileFolder(PrimaryCachePath);
                Executedsteps++;

                //Step-7:Go to folder Drive:\Cache\PF_ICA_ServerName and select one study folder.  In the selected  folder edit demographics.xml file and view the Patient Name and UID.  Keep this window open.
                var Patientname = ReadXML.ReadAttribute(demographicsxmlpath, "Study", "name");
                var lastname = Patientname.Split('^')[0].Replace(" ", "");
                var firstname = Patientname.Split('^')[1].Replace(" ", "");
                if (lastname.Equals(lastname_2) && firstname.Equals(firstname_2))
                {
                    result.steps[++Executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[Executedsteps].description);
                }
                else
                {
                    result.steps[++Executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[Executedsteps].description);
                    result.steps[Executedsteps].SetLogs();
                }


                //Report Result
                result.FinalResult(Executedsteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(Executedsteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                return result;
            }
            finally
            {
                BasePage basepage = new BasePage();
                basepage.RestartIISUsingexe();
                String lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String datasourceip = login.GetHostName(Config.DestEAsIp);
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + datasourceip + "/webadmin");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                workflow.HPSearchStudy("Lastname", lastname);
                if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                    workflow.HPDeleteStudy();
                hplogin.LogoutHPen();

            }
        }

        /// <summary> 
        /// 3.0 Remote Cache Server
        /// </summary>
        public TestCaseResult Test_164718(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            int executedSteps = -1;
            var servicetool = new ServiceTool();
            String cachepath = String.Empty;
            String datasource = String.Empty;
            String datasourceip = String.Empty;
            String studypaths = String.Empty;
            List<String> PatientIDs = new List<String>();
            List<String> Accessions = new List<String>();
            List<String> SopUID = new List<String>();
            List<String> Seriesuid = new List<String>();
            List<String> StudyUID = new List<String>();
            List<String> Cachedstudypath = new List<String>();
            List<String> Cachedstudyname = new List<String>();
            List<String> Demographicsxmlpath = new List<String>();
            string remoteserverip = Config.IConnectIP2;
            string networkpath = @"\\" + remoteserverip;
            string Remotehostname = "PF_" + new BasePage().GetHostName(remoteserverip).Split('-')[0] + "-" + new BasePage().GetHostName(remoteserverip).Split('-')[1];

            try
            {
                //Setup Test Step Description
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                //Get Test Data                
                datasource = login.GetHostName(Config.DestEAsIp);
                studypaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] studypath = studypaths.Split('=');
                cachepath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "CachePath");
                datasourceip = Config.DestEAsIp;
                cachepath = cachepath + Path.DirectorySeparatorChar + Remotehostname;

                for (int i = 0; i <= 2; i++)
                {
                    String PID = BasePage.ReadDicomFile<String>(studypath[i], DicomTag.PatientID);
                    PatientIDs.Add(PID);
                    String Acc = BasePage.ReadDicomFile<String>(studypath[i], DicomTag.AccessionNumber);
                    Accessions.Add(Acc);
                    String sopuid = BasePage.ReadDicomFile<String>(studypath[i], DicomTag.SOPInstanceUID);
                    SopUID.Add(sopuid);
                    String studyuid = BasePage.ReadDicomFile<String>(studypath[i], DicomTag.StudyInstanceUID);
                    StudyUID.Add(studyuid);
                    String seriesuid = BasePage.ReadDicomFile<String>(studypath[i], DicomTag.SeriesInstanceUID);
                    Seriesuid.Add(seriesuid);
                    String cachestudypath = cachepath + Path.DirectorySeparatorChar + PID + "-" + studyuid + Path.DirectorySeparatorChar + seriesuid;
                    Cachedstudypath.Add(cachestudypath);
                    String cachedstudyname = sopuid + "." + "dcm";
                    Cachedstudyname.Add(cachedstudyname);
                    String demographicsxmlpath = cachepath + Path.DirectorySeparatorChar + PID + "-" + studyuid + Path.DirectorySeparatorChar + "demographics.xml";
                    Demographicsxmlpath.Add(demographicsxmlpath);
                }

				//Delete existing study folders		
				
				//startInfo.Arguments = "use \\" + remoteserverip +" PQAte$t123-"+ new BasePage().GetHostName(remoteserverip).ToLower() + " /user:" + Config.adminUserName;
                Process proc = new Process();
                proc.StartInfo.FileName = "net.exe";
                proc.StartInfo.Arguments = @"use \\" + remoteserverip +  " PQAte$t123-" + new BasePage().GetHostName(remoteserverip).ToLower()+ " /user:Administrator" ;
                proc.Start();
                proc.WaitForExit(20000);
				if (!proc.HasExited) { proc.CloseMainWindow(); }


				BasePage.DeleteAllFileFolder(networkpath + @"\" + cachepath);

                //Step-1 - Setup a second ICA server with the same Build -and configure the Pre-Fetch Cache by enabling it and changing the polling interval to 5 min. -Configure the same Dicom archive and enable the Pre-fetch in the dicom tab.
                executedSteps++;

                //Step-2:on the remote server, share the folder Drive:\Cache\PF_SERVERNAME to everyone with read permission (e.g. shared folder is \\servername\PF_SERVERNAME) 
                //In the first ICAserver edit the Pre - fetch tab and unselect the Local CacheService and enable the Remote Cache service. Changing the default setting
                //Host = IP of the Remote ICA server
                //Port = 8771
                //Shared Folder = \\servername\PF_SERVERNAME                
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                servicetool.NavigateSubTab("Pre-fetch Cache Service");
                servicetool.ClickModifyButton();
                servicetool.EnablePrefetchCache(cachetype: "Remote", host: remoteserverip, remoteport: 8771, folderpath: networkpath + @"\Cache\" + Remotehostname);
                servicetool.RestartService();

                //Enable Prefetch cache - Datasource
                servicetool.EnableCacheForDataSource(datasource);
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                executedSteps++;

                //Step-3 - Using the pummel tool send 2 studies to an archive that has been enabled for Caching
                //Updating the Dicom files
                String patient = BasePage.ReadDicomFile<String>(studypath[0], DicomTag.PatientName).Replace("^", " ");
                String lastname_2 = patient.Split(' ')[0];
                String firstname_2 = patient.Split(' ')[1];
                var file1 = BasePage.WriteDicomFile(studypath[0], new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime }, new String[] { DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 20, 0)).ToString("HHmmss")) }, "File1");
                String patient1 = BasePage.ReadDicomFile<String>(studypath[1], DicomTag.PatientName).Replace("^", " ");
                String lastname_1 = patient1.Split(' ')[0];
                String firstname_1 = patient1.Split(' ')[1];
                var file2 = BasePage.WriteDicomFile(studypath[1], new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime }, new String[] { DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 20, 0)).ToString("HHmmss")) }, "File2");

                var client = new DicomClient();
                client.AddRequest(new DicomCStoreRequest(file1));
                client.Send(datasourceip, 12000, false, "SCU", Config.DestEAsAETitle);
                client.AddRequest(new DicomCStoreRequest(file2));
                client.Send(datasourceip, 12000, false, "SCU", Config.DestEAsAETitle);
                executedSteps++;


                //Step-4 On the Remote system open Drive-\cache\PF_Servername and wait for the studies to be listed. 
                //This might take up to a minute more than the Polling Interval set. 
                //Open the Demographics.xml file to validate the Patient name and Patient ID of the study sent.
                var cachewait = new DefaultWait<IWebDriver>(BasePage.Driver);
                cachewait.Timeout = new TimeSpan(0, 10, 0);
                cachewait.IgnoreExceptionTypes(new Type[] { new FileNotFoundException().GetType() });
                cachewait.Until<Boolean>((d) =>
                {
                    var isFileFound = BasePage.CheckFile(Cachedstudyname[0], networkpath + @"\" + Cachedstudypath[0], "dcm");
                    if (isFileFound)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                cachewait.Until<Boolean>((d) =>
                {
                    var isFileFound = BasePage.CheckFile(Cachedstudyname[1], networkpath + @"\" + Cachedstudypath[1], "dcm");
                    if (isFileFound)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                string TempLocation1 = string.Concat(Directory.GetCurrentDirectory(), Path.DirectorySeparatorChar, "Study145_1");
                if (!Directory.Exists(TempLocation1)) Directory.CreateDirectory(TempLocation1);
                string xmlpath1 = TempLocation1 + Path.DirectorySeparatorChar + Demographicsxmlpath[0].Split('\\').Last();
                File.Copy(networkpath + @"\" + Demographicsxmlpath[0], xmlpath1, true);
                var patientname1 = ReadXML.ReadAttribute(xmlpath1, "Study", "name");
                var lastname1 = patientname1.Split('^')[0].Replace(" ", "");
                var firstname1 = patientname1.Split('^')[1].Replace(" ", "");

                string TempLocation2 = string.Concat(Directory.GetCurrentDirectory(), Path.DirectorySeparatorChar, "Study145_2");
                if (!Directory.Exists(TempLocation2)) Directory.CreateDirectory(TempLocation2);
                string xmlpath2 = TempLocation2 + Path.DirectorySeparatorChar + Demographicsxmlpath[1].Split('\\').Last();
                File.Copy(networkpath + @"\" + Demographicsxmlpath[1], xmlpath2, true);
                var patientname2 = ReadXML.ReadAttribute(xmlpath2, "Study", "name");
                var lastname2 = patientname2.Split('^')[0].Replace(" ", "");
                var firstname2 = patientname2.Split('^')[1].Replace(" ", "");
                if (lastname1.ToLower().Equals(lastname_2.ToLower()) && firstname1.Equals(firstname_2) &&
                    lastname2.Equals(lastname_1) && firstname2.Equals(firstname_1))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Step-5-Login to ICA in the First server and do a search for the studies with the Study Performed - 2 hours or a setting that will list studies recently loaded.
                BasePage.Driver.Navigate().GoToUrl(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var studies = login.Navigate<Studies>();
                studies.SearchStudy(LastName: "*", Study_Performed_Period: "Last 2 Hours");
                executedSteps++;

                //Step-6-Open a study in the viewer
                studies.SelectStudy("Accession", Accessions[0]);
                bool step6 = false;
                Dictionary<string, string> patientDetails;
                if (Config.isEnterpriseViewer.ToLower().Equals("n"))
                {
                    var viewer = studies.LaunchStudy();
                    patientDetails = viewer.PatientDetailsInViewer();
                    step6 = patientDetails["LastName"].ToLower().Equals(lastname_2.ToLower()) && patientDetails["FirstName"].Equals(firstname_2);
                    viewer.CloseStudy();
                }
                else
                {
                    BluRingViewer viewer = BluRingViewer.LaunchBluRingViewer();
                    PageLoadWait.WaitForFrameLoad(20);
                    patientDetails = viewer.PatientDetailsInViewer();
                    step6 = patientDetails["LastName"].ToLower().Equals(lastname_2.ToLower()) && patientDetails["FirstName"].Equals(firstname_2);
                    viewer.CloseBluRingViewer();
                }
                if (step6)
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }
                login.Logout();

                //Step-7:Using a third party tool send a study to the Store SCP using the Server name  AE title and port 4446 
                //For example sendimage program or TomoVision Dicom manager -Configure-AE Title - ICA SERVER (in capitals)-Port - 4446-IP - ICA server -Check the cache folder Drive-\cache\PF_ICAserver
                String patient3 = BasePage.ReadDicomFile<String>(studypath[2], DicomTag.PatientName).Replace("^", " ");
                String lastname_3 = patient3.Split(' ')[0];
                String firstname_3 = patient3.Split(' ')[1];
                var file3 = BasePage.WriteDicomFile(studypath[2], new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime }, new String[] { DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 20, 0)).ToString("HHmmss")) }, "File3");
                client = new DicomClient();
                client.AddRequest(new DicomCStoreRequest(file3));
                client.Send(datasourceip, 12000, false, "SCU", Config.DestEAsAETitle);
                cachewait = new DefaultWait<IWebDriver>(BasePage.Driver);
                cachewait.Timeout = new TimeSpan(0, 10, 0);
                cachewait.IgnoreExceptionTypes(new Type[] { new FileNotFoundException().GetType() });

                cachewait.Until<Boolean>((d) =>
                {
                    var isFileFound = BasePage.CheckFile(Cachedstudyname[2], networkpath + @"\" + Cachedstudypath[2], "dcm");
                    if (isFileFound)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                string TempLocation3 = string.Concat(Directory.GetCurrentDirectory(), Path.DirectorySeparatorChar, "Study145_3");
                if (!Directory.Exists(TempLocation3)) Directory.CreateDirectory(TempLocation3);
                string xmlpath3 = TempLocation3 + Path.DirectorySeparatorChar + Demographicsxmlpath[2].Split('\\').Last();
                File.Copy(networkpath + @"\" + Demographicsxmlpath[2], xmlpath3, true);
                var patientname3 = ReadXML.ReadAttribute(xmlpath3, "Study", "name");
                var lastname3 = patientname3.Split('^')[0].Replace(" ", "");
                var firstname3 = patientname3.Split('^')[1].Replace(" ", "");

                if (lastname3.ToLower().Equals(lastname_3.ToLower()) && firstname3.Equals(firstname_3))
                {
                    result.steps[++executedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedSteps].description);
                }
                else
                {
                    result.steps[++executedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[executedSteps].description);
                    result.steps[executedSteps].SetLogs();
                }

                //Report Result
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                String Lastnames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String[] LastNameList = Lastnames.Split(':');
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + datasourceip + "/webadmin");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                foreach (String lastnme in LastNameList)
                {
                    workflow.HPSearchStudy("Lastname", lastnme);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                        workflow.HPDeleteStudy();
                }
                hplogin.LogoutHPen();
            }

        }


        /// <summary> 
        /// Pre fetch cache for local and UNC path
        /// </summary>
        public TestCaseResult Test_164723(String testid, String teststeps, int stepcount)
        {
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            //Setup Test Step Description
            result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            servicetool = new ServiceTool();
            Studies studies = null;
            StudyViewer studyviewer = null;
            BluRingViewer bluRingViewer = null;
            string Cachebase = string.Empty;
            string[] cachebase = null;
            string AETitle = string.Empty;
            string[] AccessionID = null;
            string Remotehostname = login.GetHostName(remoteserverip);
            string updatedateandtimebatchfile = string.Concat(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar, "OtherFiles\\UpdateDatetime.bat");
            Stopwatch stopwatch = new Stopwatch();
            TimeSpan timeout = new TimeSpan(0, 2, 0);
            TimeSpan timeout1 = new TimeSpan(0, 10, 0);
            string BrowserType = Config.BrowserType;
            try
            {
                //PreCondition
                AETitle = login.PrefetchAETitle;
                string[] studypath = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath")).Split('=');
                AccessionID = Convert.ToString(ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList")).Split('=');
                Cachebase = @"C:\Cache=\\" + Remotehostname + @"\Cache";
                cachebase = Cachebase.Split('=');
                // Delete the Study if already exists
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                Config.BrowserType = "Chrome";
                login = new Login();
                login.DriverGoTo(login.url);
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + Config.DestEAsIp + "/webadmin");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                foreach (String acc in AccessionID)
                {
                    workflow.HPSearchStudy("Accessionno", acc);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                        workflow.HPDeleteStudy();
                }
                hplogin.LogoutHPen();
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                Config.BrowserType = BrowserType;
                login = new Login();
                login.DriverGoTo(login.url);

                //Set Date and Time
                string time = "01:00:00";
                BasePage.RunBatchFile(updatedateandtimebatchfile, "time" + " " + time);

                //Step 1: From iCA server, Launch ica service tool
                servicetool.LaunchServiceTool();
                servicetool.EnableCacheForDataSource(login.GetHostName(Config.DestEAsIp));
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step 2: Enable Prefetch cache option from Enable Features ? Pre-Fetch Cach Service tab
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                ExecutedSteps++;

                //Step 3: Add the following fields for local machine cache base path on Prefetch cache settings
                servicetool.NavigateSubTab("Pre-fetch Cache Service");
                servicetool.ClickModifyButton();
                servicetool.EnablePrefetchCache(pollingtime: 1, timerange: 120, AEtitle: AETitle, cachebase: cachebase[0], cleanupthreshold: 1, cleanupinterval: 10, cleanuphighwatermark: "80", cleanuplowwatermark: "1");
                ExecutedSteps++;

                //Step 4: Restart IIS and windows services
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                ExecutedSteps++;

                //Step 5: Send multiple studies to the server with today's date as study date and study time will be within 2 hrs based on current time.
                string patient1 = BasePage.ReadDicomFile<String>(studypath[0], DicomTag.PatientName).Replace("^", " ");
                string studyinstanceuid1 = BasePage.ReadDicomFile<String>(studypath[0], DicomTag.StudyInstanceUID);
                string seriesuid1 = BasePage.ReadDicomFile<String>(studypath[0], DicomTag.SeriesInstanceUID);
                string sopuid1 = BasePage.ReadDicomFile<String>(studypath[0], DicomTag.SOPInstanceUID);
                string PID1 = BasePage.ReadDicomFile<String>(studypath[0], DicomTag.PatientID);
                string lastname_1 = patient1.Split(' ')[0];
                string firstname_1 = patient1.Split(' ')[1];
                string file1 = BasePage.WriteDicomFile(studypath[0], new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime }, new String[] { DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 30, 0)).ToString("HHmmss")) }, "File1");
                string patient2 = BasePage.ReadDicomFile<String>(studypath[1], DicomTag.PatientName).Replace("^", " ");
                string studyinstanceuid2 = BasePage.ReadDicomFile<String>(studypath[1], DicomTag.StudyInstanceUID);
                string seriesuid2 = BasePage.ReadDicomFile<String>(studypath[1], DicomTag.SeriesInstanceUID);
                string sopuid2 = BasePage.ReadDicomFile<String>(studypath[1], DicomTag.SOPInstanceUID);
                string PID2 = BasePage.ReadDicomFile<String>(studypath[1], DicomTag.PatientID);
                string lastname_2 = patient2.Split(' ')[0];
                string firstname_2 = patient2.Split(' ')[1];
                string file2 = BasePage.WriteDicomFile(studypath[1], new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime }, new String[] { DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 30, 0)).ToString("HHmmss")) }, "File2");
                var client = new DicomClient();
                client.AddRequest(new DicomCStoreRequest(file1));
                client.Send(Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);
                client.AddRequest(new DicomCStoreRequest(file2));
                client.Send(Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);
                ExecutedSteps++;
                //Step 6: Check studies are moved to the respective cache base location
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout)) { /*Stay Idle for 2 Miniutes*/ }
                stopwatch.Stop();
                stopwatch.Reset();
                string cachedstudyname = sopuid1;
                string cachedstudypath = cachebase[0] + Path.DirectorySeparatorChar + AETitle + Path.DirectorySeparatorChar + PID1 + "-" + studyinstanceuid1 + Path.DirectorySeparatorChar + seriesuid1 + Path.DirectorySeparatorChar;
                bool FileFound1 = false;
                try
                {
                    FileFound1 = Directory.EnumerateFiles(cachedstudypath, sopuid1 + "*").Any();
                }
                catch (Exception e) { Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException); }
                cachedstudyname = sopuid2;
                cachedstudypath = cachebase[0] + Path.DirectorySeparatorChar + AETitle + Path.DirectorySeparatorChar + PID2 + "-" + studyinstanceuid2 + Path.DirectorySeparatorChar + seriesuid2 + Path.DirectorySeparatorChar;
                bool FileFound2 = false;
                try
                {
                    FileFound2 = Directory.EnumerateFiles(cachedstudypath, sopuid2 + "*").Any();
                }
                catch (Exception e) { Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException); }
                if (FileFound1 && FileFound2)
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
                //Step 7: Login to iCA and load the cached study
                //Step 8: Click on the history tab and ensure that it loads without any problem/delay
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionID[0]);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.SelectStudy("Accession", AccessionID[0]);
                bluRingViewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                bool step7_1 = bluRingViewer.PatientDetailsInViewer()["LastName"].Equals(lastname_1, StringComparison.CurrentCultureIgnoreCase) && bluRingViewer.PatientDetailsInViewer()["FirstName"].Equals(firstname_1, StringComparison.CurrentCultureIgnoreCase);
                bool step8_1 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors)).Count == 1;
                bluRingViewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: AccessionID[1]);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.SelectStudy("Accession", AccessionID[1]);
                bluRingViewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                bool step7_2 = bluRingViewer.PatientDetailsInViewer()["LastName"].Equals(lastname_2, StringComparison.CurrentCultureIgnoreCase) && bluRingViewer.PatientDetailsInViewer()["FirstName"].Equals(firstname_2, StringComparison.CurrentCultureIgnoreCase);
                bool step8_2 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors)).Count == 1;
                bluRingViewer.CloseBluRingViewer();
                if (step7_1 && step7_2)
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

                if (step8_1 && step8_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 9: Close the iCA webaccess
                login.Logout();
                ExecutedSteps++;
                //Step 10: Check for studies in <c:\cache\PF_TestServerName> cache deleted as per the clean upthreshold and high water mark settings
                DateTime localDate = DateTime.Now.AddHours(2);
                time = localDate.ToString("hh:mm:ss");
                BasePage.RunBatchFile(updatedateandtimebatchfile, "time" + " " + time);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout1)) { /*Stay Idle for 10 Miniutes*/ }
                stopwatch.Stop();
                stopwatch.Reset();
                cachedstudypath = cachebase[0] + Path.DirectorySeparatorChar + AETitle + Path.DirectorySeparatorChar + PID1 + "-" + studyinstanceuid1 + Path.DirectorySeparatorChar + seriesuid1 + Path.DirectorySeparatorChar;
                FileFound1 = Directory.Exists(cachedstudypath);
                cachedstudypath = cachebase[0] + Path.DirectorySeparatorChar + AETitle + Path.DirectorySeparatorChar + PID2 + "-" + studyinstanceuid2 + Path.DirectorySeparatorChar + seriesuid2 + Path.DirectorySeparatorChar;
                FileFound2 = Directory.Exists(cachedstudypath);
                if (!FileFound1 && !FileFound2)
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
                /*string[] currentdatetime = basepage.GetCurrentDateAndTimeFromInternet().Split(' ');
                BasePage.RunBatchFile(updatedateandtimebatchfile, "date" + " " + currentdatetime[0]);
                BasePage.RunBatchFile(updatedateandtimebatchfile, "time" + " " + currentdatetime[1]);*/
                //Step 11: Navigate to Pre-fetch Cache Service tab and add the following fields for UNC (shared network location)cache base path
                BasePage.RunBatchFile(string.Concat(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar, "OtherFiles\\Prefetch.bat"), null);
                servicetool.RestartIISUsingexe();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                servicetool.NavigateSubTab("Pre-fetch Cache Service");
                servicetool.ClickModifyButton();
                servicetool.EnablePrefetchCache(pollingtime: 1, timerange: 120, AEtitle: AETitle, cachebase: cachebase[1], cleanupthreshold: 1, cleanupinterval: 10, cleanuphighwatermark: "80", cleanuplowwatermark: "1");
                ExecutedSteps++;
                //Step 12: Restart IIS and windows services
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                ExecutedSteps++;
                //Step 13: Send multiple studies to the server with today's date as study date and study time will be within 2 hrs based on current time.
                patient1 = BasePage.ReadDicomFile<String>(studypath[2], DicomTag.PatientName).Replace("^", " ");
                studyinstanceuid1 = BasePage.ReadDicomFile<String>(studypath[2], DicomTag.StudyInstanceUID);
                seriesuid1 = BasePage.ReadDicomFile<String>(studypath[2], DicomTag.SeriesInstanceUID);
                sopuid1 = BasePage.ReadDicomFile<String>(studypath[2], DicomTag.SOPInstanceUID);
                PID1 = BasePage.ReadDicomFile<String>(studypath[2], DicomTag.PatientID);
                lastname_1 = patient1.Split(' ')[0];
                firstname_1 = patient1.Split(' ')[1];
                file1 = BasePage.WriteDicomFile(studypath[2], new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime }, new String[] { DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 30, 0)).ToString("HHmmss")) }, "File1");
                patient2 = BasePage.ReadDicomFile<String>(studypath[3], DicomTag.PatientName).Replace("^", " ");
                studyinstanceuid2 = BasePage.ReadDicomFile<String>(studypath[3], DicomTag.StudyInstanceUID);
                seriesuid2 = BasePage.ReadDicomFile<String>(studypath[3], DicomTag.SeriesInstanceUID);
                sopuid2 = BasePage.ReadDicomFile<String>(studypath[3], DicomTag.SOPInstanceUID);
                PID2 = BasePage.ReadDicomFile<String>(studypath[3], DicomTag.PatientID);
                lastname_2 = patient2.Split(' ')[0];
                firstname_2 = patient2.Split(' ')[1];
                file2 = BasePage.WriteDicomFile(studypath[3], new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime }, new String[] { DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 30, 0)).ToString("HHmmss")) }, "File2");
                client.AddRequest(new DicomCStoreRequest(file1));
                client.Send(Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);
                client.AddRequest(new DicomCStoreRequest(file2));
                client.Send(Config.DestEAsIp, 12000, false, "SCU", Config.DestEAsAETitle);
                ExecutedSteps++;
                //Step 14: Check studies are moved to the respective cache base location
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout)) { /*Stay Idle for 2 Miniutes*/ }
                stopwatch.Stop();
                stopwatch.Reset();
                cachedstudyname = sopuid1;
                cachedstudypath = cachebase[1] + Path.DirectorySeparatorChar + AETitle + Path.DirectorySeparatorChar + PID1 + "-" + studyinstanceuid1 + Path.DirectorySeparatorChar + seriesuid1 + Path.DirectorySeparatorChar;
                FileFound1 = false;
                try
                {
                    FileFound1 = Directory.EnumerateFiles(cachedstudypath, sopuid1 + "*").Any();
                }
                catch (Exception e) { Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException); }
                cachedstudyname = sopuid2;
                cachedstudypath = cachebase[1] + Path.DirectorySeparatorChar + AETitle + Path.DirectorySeparatorChar + PID2 + "-" + studyinstanceuid2 + Path.DirectorySeparatorChar + seriesuid2 + Path.DirectorySeparatorChar;
                FileFound2 = false;
                try
                {
                    FileFound2 = Directory.EnumerateFiles(cachedstudypath, sopuid2 + "*").Any();
                }
                catch (Exception e) { Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException); }
                if (FileFound1 && FileFound2)
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
                //Step 15: Login to iCA and load the cached study
                //Step 16: Click on the history tab and ensure that it loads without any problem/delay
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionID[2]);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.SelectStudy("Accession", AccessionID[2]);
                bluRingViewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                bool step15_1 = bluRingViewer.PatientDetailsInViewer()["LastName"].Equals(lastname_1, StringComparison.CurrentCultureIgnoreCase) && bluRingViewer.PatientDetailsInViewer()["FirstName"].Equals(firstname_1, StringComparison.CurrentCultureIgnoreCase);
                bool step16_1 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors)).Count == 1;
                bluRingViewer.CloseBluRingViewer();
                studies.SearchStudy(AccessionNo: AccessionID[3]);
                PageLoadWait.WaitForLoadingMessage(30);
                studies.SelectStudy("Accession", AccessionID[3]);
                bluRingViewer = BluRingViewer.LaunchBluRingViewer();
                PageLoadWait.WaitForFrameLoad(20);
                bool step15_2 = bluRingViewer.PatientDetailsInViewer()["LastName"].Equals(lastname_2, StringComparison.CurrentCultureIgnoreCase) && bluRingViewer.PatientDetailsInViewer()["FirstName"].Equals(firstname_2, StringComparison.CurrentCultureIgnoreCase);
                bool step16_2 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors)).Count == 1;
                bluRingViewer.CloseBluRingViewer();
                if (step15_1 && step15_2)
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
                if (step16_1 && step16_2)
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
                
                //Step 17: Close the iCA webaccess
                
                login.Logout();
                ExecutedSteps++;
                //Step 18: Check for studies in ( \\<remotemachineHostname>\<SharedFoldername>)cache deleted as per the clean upthreshold and high water mark settings.
                localDate = DateTime.Now.AddHours(2);
                time = localDate.ToString("hh:mm:ss");
                BasePage.RunBatchFile(updatedateandtimebatchfile, "time" + " " + time);
                stopwatch.Start();
                while (!(stopwatch.Elapsed >= timeout1)) { /*Stay Idle for 10 Miniutes*/ }
                stopwatch.Stop();
                stopwatch.Reset();
                cachedstudypath = cachebase[1] + Path.DirectorySeparatorChar + AETitle + Path.DirectorySeparatorChar + PID1 + "-" + studyinstanceuid1 + Path.DirectorySeparatorChar + seriesuid1 + Path.DirectorySeparatorChar;
                FileFound1 = Directory.Exists(cachedstudypath);
                cachedstudypath = cachebase[1] + Path.DirectorySeparatorChar + AETitle + Path.DirectorySeparatorChar + PID2 + "-" + studyinstanceuid2 + Path.DirectorySeparatorChar + seriesuid2 + Path.DirectorySeparatorChar;
                FileFound2 = Directory.Exists(cachedstudypath);
                if (!FileFound1 && !FileFound2)
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
                try
                {
                    servicetool.LaunchServiceTool();
                    servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                    servicetool.NavigateSubTab("Pre-fetch Cache Service");
                    servicetool.ClickModifyButton();
                    servicetool.EnablePrefetchCache(pollingtime: 5, timerange: 120, AEtitle: AETitle, cachebase: cachebase[0], cleanupthreshold: 1, cleanupinterval: 8, cleanuphighwatermark: "80", cleanuplowwatermark: "1");
                    servicetool.RestartIISandWindowsServices();
                    servicetool.CloseServiceTool();
                }
                catch (Exception) { }
                try
                {
                    BasePage.Driver.Quit();
                    BasePage.Driver = null;
                    Config.BrowserType = "Chrome";
                    login = new Login();
                    login.DriverGoTo(login.url);
                    PageLoadWait.WaitForPageLoad(60);
                    var hplogin = new HPLogin();
                    BasePage.Driver.Navigate().GoToUrl("https://" + Config.DestEAsIp + "/webadmin");
                    var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, destea: true);
                    var workflow = (WorkFlow)hphome.Navigate("Workflow");
                    workflow.NavigateToLink("Workflow", "Archive Search");
                    foreach (String acc in AccessionID)
                    {
                        workflow.HPSearchStudy("Accessionno", acc);
                        if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                            workflow.HPDeleteStudy();
                    }
                    hplogin.LogoutHPen();
                    login.DriverGoTo(login.url);
                }
                catch (Exception) { }
                try
                {
                    servicetool.CloseServiceTool();
                    string[] currentdatetime = basepage.GetCurrentDateAndTimeFromInternet().Split(' ');
                    BasePage.RunBatchFile(updatedateandtimebatchfile, "date" + " " + currentdatetime[0]);
                    BasePage.RunBatchFile(updatedateandtimebatchfile, "time" + " " + currentdatetime[1]);
                }
                catch (Exception) { }
                try
                {
                    BasePage.Driver.Quit();
                    BasePage.Driver = null;
                    Config.BrowserType = BrowserType;
                    login = new Login();
                    login.DriverGoTo(login.url);
                }
                catch (Exception) { }
            }
        }

        /// <summary> 
        /// Uncompressed Cache - Multiple Storage Volume Support
        /// </summary>
        public TestCaseResult Test_164591(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            ServiceTool servicetool = new ServiceTool();
            var netstat = new NetStat();
            String datasourceHostName = String.Empty;
            String iconnectAETitle = String.Empty;
            String sourceDirectory = String.Empty;
            String backupDirectory = String.Empty;

            try
            {
                //Setup Test Step Description
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                //Get Test Data                             
                datasourceHostName = login.GetHostName(Config.EA91);
                String cachepath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "CachePath");
                String cacheVolumSettings = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "CacheVolumeSettings");
                String messages = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Messages");
                var cachePathList = cachepath.Split('=');
                var cacheVolumeSettingsList = cacheVolumSettings.Split(':');
                var messageList = messages.Split(':');
                sourceDirectory = cachePathList[3];
                backupDirectory = cachePathList[4];

                // Precondition.
                File.Copy(sourceDirectory, backupDirectory, true);
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService);
                servicetool.ClickModifyButton();

                // Enabling the Prefetch Checkbox
                CheckBox enablePreFetchCheckbox = servicetool.EnablePreFetchCheckbox();
                if (!enablePreFetchCheckbox.Checked)
                {
                    enablePreFetchCheckbox.Click();
                }

                //Enabling the Local cache 
                RadioButton localCacheRadioButton = servicetool.LocalCacheRadioButton();
                if (!localCacheRadioButton.IsSelected)
                {
                    localCacheRadioButton.Click();
                }

                // Apply changes and restart Service tool
                servicetool.ClickApplyButtonFromTab();
                servicetool.RestartService();

                // Enable Pre-Fetch cache flag in data source                
                if (!servicetool.IsDataSourceExists(datasourceHostName))
                {
                    servicetool.AddEADatasource(Config.DestEAsIp, Config.DestEAsAETitle, "");
                }
                servicetool.EnableCacheForDataSource(datasourceHostName, true);
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                //Check the System Service is running "iConnect Access Image Pre-fetch Service"
                bool PreFetchService = wpfobject.ServiceStatus("iConnect Access Image Pre-fetch Service", "Running");
                if (PreFetchService)
                    Logger.Instance.InfoLog("Prefectch catch service is running");
                else
                    Logger.Instance.InfoLog("Prefetch catch sevice  is not running");

                // Step 1 - Login to iCA 7.0 server and launch Service Tool	
                servicetool.LaunchServiceTool();
                if (wpfobject.CheckWindowExists(ServiceTool.ConfigTool_Name))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 2  - Navigate to Enable Feature tab> Pre-fetch cache service	
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService);
                if (wpfobject.VerifyElementExist("Modify", 1))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 3 -  Select the checkbox "Enable Pre-fetch Cache Service"
                servicetool.ClickModifyButton();
                enablePreFetchCheckbox = servicetool.EnablePreFetchCheckbox();
                if (!enablePreFetchCheckbox.Checked)
                {
                    enablePreFetchCheckbox.Click();
                    wpfobject.WaitTillLoad();
                }
                if (enablePreFetchCheckbox.Checked)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 4 - Go to "Cache store SCP Settings" tab	
                //Local cache            
                wpfobject.GetUIItem<ITabPage, RadioButton>(servicetool.GetCurrentTabItem(), ServiceTool.EnableFeatures.Name.LocalCacheService, 1, "0").Click();
                Tab PreFTab = wpfobject.GetUIItem<ITabPage, Tab>(WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab);
                PreFTab.SelectTabPage(ServiceTool.EnableFeatures.Name.CacheStoreSCPSettings);
                wpfobject.WaitTillLoad();
                ITabPage cacheStore = servicetool.GetCurrentTabItem().Get<Tab>(SearchCriteria.All).SelectedTab;
                var step4 = wpfobject.VerifyElementExist(ServiceTool.EnableFeatures.Name.CacheVolumeSetting, 1);
                if (step4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 5 - Cache Volume settings have one default volume added (eg: C:\Cache with volumeid=1)               
                GroupBox cacheGroup = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), ServiceTool.EnableFeatures.Name.CacheVolumeSetting, 1);
                ListView cacheListview = wpfobject.GetAnyUIItem<GroupBox, ListView>(cacheGroup, "", 1);
                bool step5_1 = cacheListview.Rows.Count == 1;
                bool step5_2 = false;
                foreach (ListViewRow row in cacheListview.Rows)
                {
                    if (row.Cells[0].Text.Equals("1"))
                    {
                        if (!row.Cells[1].Text.Equals(cachePathList[0]))
                        {
                            step5_2 = true;
                            break;
                        }
                    }
                    Logger.Instance.InfoLog("The Expected volume id is 1 and the actual volume id is "+ row.Cells[0]);
                    Logger.Instance.InfoLog("The Expected volume Path is " + cachePathList[0] + " and the actual volume id is " + row.Cells[1]);
                }
                if (step5_1 && step5_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The step5_1 is "+ step5_1 + " and the step5_2 is " + step5_2);                    
                }
                    
                // Step 6 - Check All the settings in Cache Volume setting section	
                Button addButton = wpfobject.GetButton(ServiceTool.EnableFeatures.Name.AddButton, 1);
                Button editButton = wpfobject.GetButton(ServiceTool.EnableFeatures.Name.EditButton, 1);
                Button deleteButton = wpfobject.GetButton(ServiceTool.EnableFeatures.Name.DeleteButton, 1);
                Button submitButton = wpfobject.GetButton(ServiceTool.EnableFeatures.Name.SubmitButton, 1);
                ITabPage cacheTab = servicetool.GetCurrentTabItem().Get<Tab>(SearchCriteria.All).SelectedTab;
                wpfobject.WaitTillLoad();
                IUIItem[] TextBoxList = cacheTab.GetMultiple(SearchCriteria.ByClassName("TextBox"));
                if (addButton.Enabled && !(editButton.Enabled && deleteButton.Enabled
                    && submitButton.Enabled && TextBoxList[6].Enabled && TextBoxList[7].Enabled
                    && TextBoxList[8].Enabled && TextBoxList[9].Enabled))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 7 - Click "Add" button in Cache Volume Setting section	
                wpfobject.ClickButton(ServiceTool.EnableFeatures.Name.AddButton, 1);
                wpfobject.WaitTillLoad();
                TextBoxList = cacheTab.GetMultiple(SearchCriteria.ByClassName("TextBox"));
                if (submitButton.Enabled && TextBoxList[6].Enabled && TextBoxList[7].Enabled
                    && TextBoxList[8].Enabled && TextBoxList[9].Enabled)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 8 & 9 - Add details for below fields
                //1) Cache volume storage path by clicking on button "..." > Select path(to cache folder)
                //2) Cleanup High Water Mark(%)= 80
                //3) Cleanup Low water Mark(%)= 50
                var cleanupWaterMark = cacheVolumeSettingsList[0].Split(',');
                TextBoxList[7].SetValue(cachePathList[1]);
                TextBoxList[8].SetValue(cleanupWaterMark[0]);
                TextBoxList[9].SetValue(cleanupWaterMark[1]);
                wpfobject.ClickButton(ServiceTool.EnableFeatures.Name.SubmitButton, 1);
                cacheListview = wpfobject.GetAnyUIItem<GroupBox, ListView>(cacheGroup, "", 1);
                servicetool.ApplyEnableFeatures();
                servicetool.RestartService();
                if (cacheListview.Rows.Count.Equals(2))
                    result.steps[ExecutedSteps += 2].StepPass();
                else
                    result.steps[ExecutedSteps += 2].StepFail();

                // Step 10 - Open file explorer and navigate to c:\webaccess\WindowsService\ImagePrefetch\Config\PrefetchStoreSCPServerConfiguration.xml	
                //ReadXML read = new ReadXML();
                var xml = ReadXML.ReadDataXML(cachePathList[3], "StoreScpServer");
                var cache = xml["CacheVolumes"].Trim();
                var cachevolumenode = cache.Split('>');
                if (cachevolumenode[1].Equals("<CacheVolume volumeId=\"2\" cacheBase=\"" + cachePathList[1] + "\" cacheCleanupHighWaterMark=\"" + cleanupWaterMark[0] + "\" cacheCleanupLowWaterMark=\"" + cleanupWaterMark[1] + "\" /"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 11 & 12 - Click "Add" button in Cache Volume Setting section
                //Add details for below fields
                //1) Cache volume storage path by clicking on button "..." > Select path(to cache folder)
                //2) Cleanup High Water Mark(%)= 60
                //3) Cleanup Low water Mark(%)= 30
                servicetool.ModifyEnableFeatures();
                var cleanupWaterMark1 = cacheVolumeSettingsList[1].Split(',');
                wpfobject.ClickButton(ServiceTool.EnableFeatures.Name.AddButton, 1);
                TextBoxList = cacheTab.GetMultiple(SearchCriteria.ByClassName("TextBox"));
                TextBoxList[7].SetValue(cachePathList[2]);
                TextBoxList[8].SetValue(cleanupWaterMark1[0]);
                TextBoxList[9].SetValue(cleanupWaterMark1[1]);
                wpfobject.ClickButton(ServiceTool.EnableFeatures.Name.SubmitButton, 1);
                wpfobject.WaitTillLoad();
                cacheListview = wpfobject.GetAnyUIItem<GroupBox, ListView>(cacheGroup, "", 1);
                servicetool.ApplyEnableFeatures();
                servicetool.RestartService();
                if (cacheListview.Rows.Count.Equals(3))
                    result.steps[ExecutedSteps += 2].StepPass();
                else
                    result.steps[ExecutedSteps += 2].StepFail();

                // Step 13 - Open file explorer and navigate to c:\webaccess\WindowsService\ImagePrefetch\Config\PrefetchStoreSCPServerConfiguration.xml
                xml = ReadXML.ReadDataXML(cachePathList[3], "StoreScpServer");
                cache = xml["CacheVolumes"].Trim();
                cachevolumenode = cache.Split('>');
                if (cachevolumenode[0].Equals("<CacheVolume volumeId=\"1\" cacheBase=\"c:\\Cache\" cacheCleanupHighWaterMark=\"80\" cacheCleanupLowWaterMark=\"50\" /") &&
                    cachevolumenode[1].Equals("<CacheVolume volumeId=\"2\" cacheBase=\"" + cachePathList[1] + "\" cacheCleanupHighWaterMark=\"" + cleanupWaterMark[0] + "\" cacheCleanupLowWaterMark=\"" + cleanupWaterMark[1] + "\" /") &&
                    cachevolumenode[2].Equals("<CacheVolume volumeId=\"3\" cacheBase=\"" + cachePathList[2] + "\" cacheCleanupHighWaterMark=\"" + cleanupWaterMark1[0] + "\" cacheCleanupLowWaterMark=\"" + cleanupWaterMark1[1] + "\" /"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The cachevolume of First in xml is " + cachevolumenode[0]);
                    Logger.Instance.InfoLog("The cachevolume of second in xml is " + cachevolumenode[1]);
                    Logger.Instance.InfoLog("The cachevolume of thrid in xml is " + cachevolumenode[2]);
                }
                    
                // Step 14 - 1) Highlight the last record in cache volume settings
                //eg: C:\Cache\NewCache2
                //2) Click "Edit" button and update values for High water mark / low water mark values and click "submit" button
                //3) Apply and reset IIS and window services
                servicetool.ModifyEnableFeatures();
                cacheListview = wpfobject.GetAnyUIItem<GroupBox, ListView>(cacheGroup, "", 1);
                cacheListview.Select("Volume Base", cachePathList[2]);
                var cleanupWaterMark2 = cacheVolumeSettingsList[2].Split(',');
                wpfobject.ClickButton(ServiceTool.EnableFeatures.Name.EditButton, 1);
                TextBoxList = cacheTab.GetMultiple(SearchCriteria.ByClassName("TextBox"));
                var step14 = TextBoxList[6].Enabled && TextBoxList[7].Enabled && TextBoxList[8].Enabled && TextBoxList[9].Enabled;
                TextBoxList[8].SetValue(cleanupWaterMark2[0]);
                TextBoxList[9].SetValue(cleanupWaterMark2[1]);
                wpfobject.ClickButton(ServiceTool.EnableFeatures.Name.SubmitButton, 1);
                wpfobject.WaitTillLoad();
                servicetool.ApplyEnableFeatures();
                servicetool.RestartService();
                if (step14)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 15 - 1) Open file explorer and navigate to c:\webaccess\WindowsService\ImagePrefetch\Config\PrefetchStoreSCPServerConfiguration.xml
                //2) Check for updated values
                xml = ReadXML.ReadDataXML(cachePathList[3], "StoreScpServer");
                cache = xml["CacheVolumes"].Trim();
                cachevolumenode = cache.Split('>');
                if (cachevolumenode[2].Equals("<CacheVolume volumeId=\"3\" cacheBase=\"" + cachePathList[2] + "\" cacheCleanupHighWaterMark=\"" + cleanupWaterMark2[0] + "\" cacheCleanupLowWaterMark=\"" + cleanupWaterMark2[1] + "\" /"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 16 - 1) Highlight the last record in cache volume settings
                //eg: E\Cache\NewCache2
                //2) Click "Delete" button
                servicetool.ModifyEnableFeatures();
                cacheListview = wpfobject.GetAnyUIItem<GroupBox, ListView>(cacheGroup, "", 1);
                cacheListview.Select("Volume Base", cachePathList[2]);
                int rowCountBefore = cacheListview.Rows.Count;
                wpfobject.ClickButton(ServiceTool.EnableFeatures.Name.DeleteButton, 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByIndex(1);
                var warningMessage = wpfobject.GetTextfromElement("65535", messageList[0]);
                if (warningMessage.Equals(messageList[0]))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 17 - Click "No" button	
                wpfobject.ClickButton("No", 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByTitle(ServiceTool.ConfigTool_Name);
                cacheListview = wpfobject.GetAnyUIItem<GroupBox, ListView>(cacheGroup, "", 1);
                if (cacheListview.Rows.Count == rowCountBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 18 - 1) Highlight the last record in cache volume settings
                //eg: E:\Cache\NewCache2
                //2) Click "Delete" button
                //3) Click "Yes" to warning 
                bool step18_1 = false;
                cacheListview = wpfobject.GetAnyUIItem<GroupBox, ListView>(cacheGroup, "", 1);
                cacheListview.Select("Volume Base", cachePathList[2]);
                wpfobject.ClickButton(ServiceTool.EnableFeatures.Name.DeleteButton, 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByIndex(1);
                wpfobject.ClickButton("Yes", 1);
                wpfobject.WaitTillLoad();
                cacheListview = wpfobject.GetAnyUIItem<GroupBox, ListView>(cacheGroup, "", 1);
                step18_1 = cacheListview.Rows.Count == rowCountBefore - 1;
                wpfobject.GetMainWindowByTitle(ServiceTool.ConfigTool_Name);
                servicetool.ApplyEnableFeatures();
                servicetool.RestartService();
                xml = ReadXML.ReadDataXML(cachePathList[3], "StoreScpServer");
                cache = xml["CacheVolumes"].Trim();
                bool step18_2 = !cache.Contains("<CacheVolume volumeId=\"3\" cacheBase=\"" + cachePathList[2] + "\" cacheCleanupHighWaterMark=\"" + cleanupWaterMark2[0] + "\" cacheCleanupLowWaterMark=\"" + cleanupWaterMark2[1] + "\" /");
                if (step18_1 && step18_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 19 - Click "Add" button in Cache Volume Setting section
                //Add details for below fields
                //1) Cache volume storage path by clicking on button "..." > Select already existing path eg;
                //(D:\Cache\NewCache1)
                //2) Cleanup High Water Mark(%)= 60
                //3) Cleanup Low water Mark(%)= 30
                //4) Submit
                wpfobject.GetMainWindowByTitle(ServiceTool.ConfigTool_Name);
                servicetool.ModifyEnableFeatures();
                wpfobject.ClickButton("Add", 1);
                wpfobject.WaitTillLoad();
                TextBoxList = cacheTab.GetMultiple(SearchCriteria.ByClassName("TextBox"));
                TextBoxList[7].SetValue(cachePathList[1]);
                TextBoxList[8].SetValue(cleanupWaterMark2[0]);
                TextBoxList[9].SetValue(cleanupWaterMark2[1]);
                wpfobject.ClickButton("Submit", 1);
                wpfobject.WaitTillLoad();
                wpfobject.GetMainWindowByIndex(1);
                var Message = wpfobject.GetTextfromElement("65535", messageList[1]);
                wpfobject.ClickButton("OK", 1);
                if (Message.Equals(messageList[1]))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 20 - 1) Scroll down in service tool to > Auto Decompress settings
                //2) Check for the 2 checkboxes and their default value
                //3) Check Compressed Study Cleanup threashold input area is disabled 
                wpfobject.GetMainWindowByTitle(ServiceTool.ConfigTool_Name);
                var isEncapsultedDataChecked = wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfEncapsulatedData, 1);
                var isLossyDataChecked = wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfLossyData, 1);
                TextBox compressedThreashold = wpfobject.GetTextbox(ServiceTool.EnableFeatures.ID.CompressedStudyCleanupThreshold, 0);
                if (!isEncapsultedDataChecked && !isLossyDataChecked && !compressedThreashold.Enabled)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The state of lossyDataChecked is " + isLossyDataChecked);
                    Logger.Instance.InfoLog("The state of EncapsultedDataChecked is " + isEncapsultedDataChecked);
                    Logger.Instance.InfoLog("The Enabled state of compressedThreashold is " + compressedThreashold.Enabled);
                }                    

                // Step 21 - Check both the check boxes for auto decompress settings and verify Compressed Study Cleanup threashold input area is enabled
                wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfEncapsulatedData, 1);
                wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfLossyData, 1);
                isEncapsultedDataChecked = wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfEncapsulatedData, 1);
                isLossyDataChecked = wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfLossyData, 1);
                compressedThreashold = wpfobject.GetTextbox(ServiceTool.EnableFeatures.ID.CompressedStudyCleanupThreshold, 0);
                if (isEncapsultedDataChecked && isLossyDataChecked && compressedThreashold.Enabled)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                // Step 22 - Give some input value for compressed Study Cleanup threshold and click Apply button
                //Restart IIS
                compressedThreashold.SetValue(cacheVolumeSettingsList[3]);
                servicetool.ApplyEnableFeatures();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                servicetool.RestartIIS();
                ExecutedSteps++;

                // Step 23 - Open file explorer and navigate to c:\webaccess\WindowsService\ImagePrefetch\Config\PrefetchStoreSCPServerConfiguration.xml
                var studyCleanupThreshold = ReadXML.ReadAttribute(cachePathList[3], "AutoDecompress", "CompressedStudyCleanupThreshold");
                if (studyCleanupThreshold.Equals(cacheVolumeSettingsList[3]))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Report Result
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
                //Logout
                login.Logout();
                //Return Result
                return result;
            }
            finally
            {
                File.Copy(backupDirectory, sourceDirectory, true);
            }
        }

        /// <summary>
        ///  Studies cached as per configured storage settings
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164598(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            String datasourceip = Config.EA91;

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);

                String studypaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String cachepath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "CachePath");
                cachepath = cachepath + Path.DirectorySeparatorChar + "PF_" + login.GetHostName(Config.IConnectIP);
                String AETitle = "PF_" + login.GetHostName(Config.IConnectIP);
                String datasource = login.GetHostName(Config.EA91);
                String PID = BasePage.ReadDicomFile<String>(studypaths, DicomTag.PatientID);
                String studyuid = BasePage.ReadDicomFile<String>(studypaths, DicomTag.StudyInstanceUID);
                String seriesuid = BasePage.ReadDicomFile<String>(studypaths, DicomTag.SeriesInstanceUID);
                String sopuid = BasePage.ReadDicomFile<String>(studypaths, DicomTag.SOPInstanceUID);
                String cachestudypath = cachepath + Path.DirectorySeparatorChar + PID + "-" + studyuid + Path.DirectorySeparatorChar + seriesuid;
                String cachedstudyname = sopuid + "." + "dcm";

                try
                {
                    String Lastnames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                    var hplogin = new HPLogin();
                    BasePage.Driver.Navigate().GoToUrl("https://" + datasourceip + "/webadmin");
                    var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                    var workflow = (WorkFlow)hphome.Navigate("Workflow");
                    workflow.NavigateToLink("Workflow", "Archive Search");
                    workflow.HPSearchStudy("Lastname", Lastnames);
                    workflow.HPDeleteStudy();
                    hplogin.LogoutHPen();
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Error while Delete study from EA" + ex.Message);
                }

                //Pre-conditions for EnableFeaturesTab.
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService);
                servicetool.ClickModifyButton();
                CheckBox EnablePrefetch = wpfobject.GetUIItem<ITabPage, CheckBox>(servicetool.GetCurrentTabItem(), "Enable Pre-fetch Cache Service", 1, "0");
                if (EnablePrefetch.Checked == false)
                {
                    EnablePrefetch.Click();
                }
                RadioButton localCacheRadioButton = servicetool.LocalCacheRadioButton();
                if (!(localCacheRadioButton.Enabled && localCacheRadioButton.IsSelected))
                {
                    localCacheRadioButton.Click();
                }
                servicetool.EnablePrefetchCache(cachetype: "Local", pollingtime: 2, AEtitle: AETitle);
                servicetool.WaitWhileBusy();
                servicetool.RestartService();

                //Pre-condition for DataSourceTab.
                if (!servicetool.IsDataSourceExists(login.GetHostName(Config.EA91)))
                {
                    servicetool.AddEADatasource(Config.EA91, Config.EA91AETitle);
                }
                servicetool.EnableCacheForDataSource(login.GetHostName(Config.EA91), true);
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                try
                {
                    bool PreFetchService = wpfobject.ServiceStatus("iConnect Access Image Pre-fetch Service", "Running");
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Error while Verifying iConnect Access Image Pre-fetch Service is Running or Not " + ex.Message);
                }

                //Delete existing study folders
                BasePage.DeleteAllFileFolder(cachepath);

                //step1 Run Preconditions.
                ExecutedSteps++;

                //step2  Launch service tool and Observe for the checkbox "Allow Auto Decompression of encapsulated data with lossless transfer syntaxes".
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService);
                servicetool.ClickModifyButton();
                Tab PreFTab_1 = wpfobject.GetUIItem<ITabPage, Tab>(WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab);
                PreFTab_1.SelectTabPage("Cache Store SCP Settings");
                bool isEncapsultedDataChecked = wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfEncapsulatedData, 1);
                if (!isEncapsultedDataChecked)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step3 Enable the checkbox "Allow Auto Decompression of encapsulated data with lossless transfer syntaxes"
                if (!isEncapsultedDataChecked)
                    wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfEncapsulatedData, 1);
                bool isEncapsultedDataChecked_1 = wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfEncapsulatedData, 1);
                if (isEncapsultedDataChecked_1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                servicetool.ClickApplyButtonFromTab();
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                //step4 Upload a study to the data source (BR_EA) that has prefetch configured and node added on EA
                //Updating the Dicom files
                var file1 = BasePage.WriteDicomFile(studypaths, new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime }, new String[] { DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 20, 0)).ToString("HHmmss")) });
                var client = new DicomClient();
                client.AddRequest(new DicomCStoreRequest(file1));
                client.Send(datasourceip, 12000, false, "SCU", Config.EA91AETitle);
                ExecutedSteps++;

                //step5 Wait for the time specified in polling interval (eg 2 mins)
                //step6 Check the all available configured storage volume settings
                //step7 Check the folder name
                var cachewait = new DefaultWait<IWebDriver>(BasePage.Driver);
                cachewait.Timeout = new TimeSpan(0, 10, 0);
                cachewait.IgnoreExceptionTypes(new Type[] { new FileNotFoundException().GetType() });
                bool filefound = false;
                cachewait.Until<Boolean>((d) =>
                {

                    var isFileFound = BasePage.CheckFile(cachedstudyname, cachestudypath, "dcm");
                    if (isFileFound)
                    {
                        filefound = true;
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                });
                if (filefound)
                    result.steps[ExecutedSteps += 3].StepPass();
                else
                    result.steps[ExecutedSteps += 3].StepFail();


                //step8 Check the database for uncompressed job record
                DataBaseUtil db = new DataBaseUtil("sqlserver", "IRWSDB", InstanceName: "WEBACCESS");
                db.ConnectSQLServerDB();
                IList<String> rows = db.ExecuteQuery(" Select Detail from Job  where JobTypeUid = 11 and status = 7 order by CreationDate desc");
                bool Step8 = rows[0].Contains(studyuid);
                if (Step8)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step9 Check if the uncompressed job record has been added to database
                rows = db.ExecuteQuery("Select VolumeID from UncompressStudy");
                bool step9 = rows[0].Equals("1");
                if (step9)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step10 Check the cached storage folder for corresponding VolumeID 
                bool step10 = Directory.Exists(cachepath);
                if (step10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Return Result.
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
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService);
                servicetool.ClickModifyButton();
                Tab PreFTab_1 = wpfobject.GetUIItem<ITabPage, Tab>(WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab);
                PreFTab_1.SelectTabPage("Cache Store SCP Settings");
                if (wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfEncapsulatedData, 1))
                    wpfobject.UnSelectCheckBox(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfEncapsulatedData, 1);
                servicetool.ClickApplyButtonFromTab();
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                String Lastnames = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String[] LastNameList = Lastnames.Split(':');
                var hplogin = new HPLogin();
                BasePage.Driver.Navigate().GoToUrl("https://" + datasourceip + "/webadmin");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                foreach (String lastnme in LastNameList)
                {
                    workflow.HPSearchStudy("Lastname", lastnme);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                        workflow.HPDeleteStudy();
                }
                hplogin.LogoutHPen();
            }
        }


        /// <summary>
        ///  Auto decompression of data for Lossy/Lossless transfer syntax
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164656(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables         
            TestCaseResult result = null;
            int ExecutedSteps = -1;
            String datasourceip = Config.EA91;
            List<String> PatientIDs = new List<String>();

            try
            {
                result = new TestCaseResult(stepcount);
                result.SetTestStepDescription(teststeps);
                String studypaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String[] studypathslist = studypaths.Split('=');
                String cachepath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "CachePath");
                cachepath = cachepath + Path.DirectorySeparatorChar + "PF_" + login.GetHostName(Config.IConnectIP);
                String AETitle = "PF_" + login.GetHostName(Config.IConnectIP);
                String datasource = login.GetHostName(Config.EA91);
                List<String> Accessions = new List<String>();
                List<String> SopUID = new List<String>();
                List<String> Seriesuid = new List<String>();
                List<String> StudyUID = new List<String>();
                List<String> Cachedstudypath = new List<String>();
                List<String> Cachedstudyname = new List<String>();
                List<String> Demographicsxmlpath = new List<String>();

                for (int i = 0; i <= 2; i++)
                {
                    String PID = BasePage.ReadDicomFile<String>(studypathslist[i], DicomTag.PatientID);
                    PatientIDs.Add(PID);
                    String Acc = BasePage.ReadDicomFile<String>(studypathslist[i], DicomTag.AccessionNumber);
                    Accessions.Add(Acc);
                    String sopuid = BasePage.ReadDicomFile<String>(studypathslist[i], DicomTag.SOPInstanceUID);
                    SopUID.Add(sopuid);
                    String studyuid = BasePage.ReadDicomFile<String>(studypathslist[i], DicomTag.StudyInstanceUID);
                    StudyUID.Add(studyuid);
                    String seriesuid = BasePage.ReadDicomFile<String>(studypathslist[i], DicomTag.SeriesInstanceUID);
                    Seriesuid.Add(seriesuid);
                    String cachestudypath = cachepath + Path.DirectorySeparatorChar + PID + "-" + studyuid + Path.DirectorySeparatorChar + seriesuid;
                    Cachedstudypath.Add(cachestudypath);
                    String cachedstudyname = sopuid + "." + "dcm";
                    Cachedstudyname.Add(cachedstudyname);
                }

                //Pre-conditions for EnableFeaturesTab.
                BasePage.DeleteAllFileFolder(cachepath);
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService);
                servicetool.ClickModifyButton();
                CheckBox EnablePrefetch = wpfobject.GetUIItem<ITabPage, CheckBox>(servicetool.GetCurrentTabItem(), "Enable Pre-fetch Cache Service", 1, "0");
                if (EnablePrefetch.Checked == false)
                {
                    EnablePrefetch.Click();
                }
                RadioButton localCacheRadioButton = servicetool.LocalCacheRadioButton();
                if (!(localCacheRadioButton.Enabled && localCacheRadioButton.IsSelected))
                {
                    localCacheRadioButton.Click();
                }
                servicetool.EnablePrefetchCache(cachetype: "Local", pollingtime: 2, AEtitle: AETitle);
                servicetool.WaitWhileBusy();
                servicetool.RestartService();

                //Pre-condition for DataSourceTab.
                if (!servicetool.IsDataSourceExists(login.GetHostName(Config.EA91)))
                {
                    servicetool.AddEADatasource(Config.EA91, Config.EA91AETitle, "");
                }
                servicetool.EnableCacheForDataSource(login.GetHostName(Config.EA91), true);
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                try
                {
                    bool PreFetchService = wpfobject.ServiceStatus("iConnect Access Image Pre-fetch Service", "Running");
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Error while Verifying iConnect Access Image Pre-fetch Service is Running or Not " + ex.Message);
                }

                //step1 - Run Preconditions
                ExecutedSteps++;

                //step2 - 1) Launch service tool
                //2)Navigate to Enable Feature tab > Pre - fetch cache service
                //3) Scroll down in service tool to > Auto Decompress settings
                //4) Check the checkbox "Allow Auto Decompression of encapsulated data with lossless transfer syntaxes"
                //5) Uncheck "Allow Auto decompression for lossy data."
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService);
                servicetool.ClickModifyButton();
                Tab PreFTab = wpfobject.GetUIItem<ITabPage, Tab>(WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab);
                PreFTab.SelectTabPage("Cache Store SCP Settings");
                GroupBox autoDecompressGroudp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), "Auto Decompress Settings", 1);
                bool isEncapsultedDataChecked = wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfEncapsulatedData, 1);
                if (!isEncapsultedDataChecked)
                    wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfEncapsulatedData, 1);
                bool isLossyDataChecked = wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfLossyData, 1);
                if (isLossyDataChecked)
                    wpfobject.UnSelectCheckBox(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfLossyData, 1);
                isEncapsultedDataChecked = wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfEncapsulatedData, 1);
                isLossyDataChecked = wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfLossyData, 1);
                if (isEncapsultedDataChecked && !isLossyDataChecked)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                servicetool.ClickApplyButtonFromTab();
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                //step3 - Upload a study(Dicom with lossless transfer syntax) to the data source (BR_EA) that has prefetch configured and node added on EA	
                var file1 = BasePage.WriteDicomFile(studypathslist[0], new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime }, new String[] { DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 20, 0)).ToString("HHmmss")) });
                var client = new DicomClient();
                client.AddRequest(new DicomCStoreRequest(file1));
                client.Send(datasourceip, 12000, false, "SCU", Config.EA91AETitle);
                ExecutedSteps++;

                //step4 - Wait for the time specified in polling interval (eg 2 mins)	
                var cachewait = new DefaultWait<IWebDriver>(BasePage.Driver);
                cachewait.Timeout = new TimeSpan(0, 10, 0);
                cachewait.IgnoreExceptionTypes(new Type[] { new FileNotFoundException().GetType() });
                bool FileFound = false;
                cachewait.Until<Boolean>((d) =>
                {
                    var isFileFound = BasePage.CheckFile(Cachedstudyname[0], Cachedstudypath[0], "dcm");
                    if (isFileFound)
                    {
                        FileFound = true;
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                });
                if (FileFound)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step5 - Check the database for uncompressed job record
                //1) Login to SQL management studio
                //2) Create a new query: "Select * from Job where JobTypeUid=11 and status=7"
                //3) Check the "Detail" column
                Thread.Sleep(15000);
                DataBaseUtil db = new DataBaseUtil("sqlserver", "IRWSDB", InstanceName: "WEBACCESS");
                db.ConnectSQLServerDB();
                var rows = db.ExecuteQuery("Select Detail from Job  where JobTypeUid = 11 and status = 7 order by CreationDate desc");
                bool step5 = rows[0].Contains(StudyUID[0]);
                if (step5)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step6 - 	Check the volumeID for the uncompressed job record to see the storage location
                //1) Login to SQL management studio
                //2) Create a query "Select * from UncompressStudy"
                //3) Note the VolumeID eg = 1                
                rows = db.ExecuteQuery("Select VolumeID from UncompressStudy");
                bool step6 = rows[0].Equals("1");
                if (step6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step7 - Check the cached storage folder for corresponding VolumeID (egVolumeID=1 is for C\:Cache)	
                var step7 = Directory.Exists(cachepath + "\\" + PatientIDs[0] + "-" + StudyUID[0]);
                if (step7)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step8 - Check the uncompressed series folder (appended with -u)
                //eg: 1.2.840.113697.1.2.1530125917.3633541310.9988.1530125917.13.1 - u
                Cachedstudypath[0] = Cachedstudypath[0] + "-u";
                Thread.Sleep(30000);
                var isFileFound_1 = BasePage.CheckFile(Cachedstudyname[0], Cachedstudypath[0], "dcm");
                var isFileFound_2 = BasePage.CheckFile("seriesinfo", Cachedstudypath[0], "xml");
                if (isFileFound_1 && isFileFound_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The dcm file found is " + isFileFound_1 + " The xml File found is " + isFileFound_2);
                }

                // Step 9 - 1) Launch service tool
                //2)Navigate to Enable Feature tab > Pre - fetch cache service
                //3) Scroll down in service tool to > Auto Decompress settings
                //4) Check the checkbox "Allow Auto Decompression of encapsulated data with lossless transfer syntaxes"
                //5) Check "Allow Auto decompression for lossy data."
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService);
                servicetool.ClickModifyButton();
                PreFTab = wpfobject.GetUIItem<ITabPage, Tab>(WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab);
                PreFTab.SelectTabPage(ServiceTool.EnableFeatures.Name.CacheStoreSCPSettings);
                autoDecompressGroudp = wpfobject.GetAnyUIItem<ITabPage, GroupBox>(servicetool.GetCurrentTabItem(), "Auto Decompress Settings", 1);
                isEncapsultedDataChecked = wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfEncapsulatedData, 1);
                if (!isEncapsultedDataChecked)
                    wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfEncapsulatedData, 1);
                isLossyDataChecked = wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfLossyData, 1);
                if (!isLossyDataChecked)
                    wpfobject.SelectCheckBox(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfLossyData, 1);
                isEncapsultedDataChecked = wpfobject.IsCheckBoxSelected("Allow Auto Decompression of encapsulated data with lossless transfer syntaxes", 1);
                isLossyDataChecked = wpfobject.IsCheckBoxSelected("Allow Auto Decompression for lossy data", 1);
                if (isEncapsultedDataChecked && isLossyDataChecked)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                servicetool.ClickApplyButtonFromTab();
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                //step10 - Upload a study(Dicom with lossy transfer syntax) to the data source (BR_EA) that has prefetch configured and node added on EA
                var file2 = BasePage.WriteDicomFile(studypathslist[1], new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime }, new String[] { DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 20, 0)).ToString("HHmmss")) });
                client = new DicomClient();
                client.AddRequest(new DicomCStoreRequest(file2));
                client.Send(datasourceip, 12000, false, "SCU", Config.EA91AETitle);
                ExecutedSteps++;

                //step11 - Wait for the time specified in polling interval (eg 2 mins)	
                cachewait.Timeout = new TimeSpan(0, 10, 0);
                cachewait.IgnoreExceptionTypes(new Type[] { new FileNotFoundException().GetType() });
                bool FileFound_1 = false;
                cachewait.Until<Boolean>((d) =>
                {

                    var isFileFound = BasePage.CheckFile(Cachedstudyname[1], Cachedstudypath[1], "dcm");
                    if (isFileFound)
                    {
                        FileFound_1 = true;
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                });
                if (FileFound_1)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step12 - Check the DB tables (Job and uncompressStudy) and note the VolumeID	                
                db.ConnectSQLServerDB();
                IList<String> rows_2 = db.ExecuteQuery("Select Detail from Job  where JobTypeUid = 11 and status = 7 order by CreationDate desc");
                bool step12_1 = rows_2[0].Contains(StudyUID[1]);
                db.ConnectSQLServerDB();
                IList<String> rows_3 = db.ExecuteQuery("Select VolumeID from UncompressStudy");
                bool step12_2 = rows_3[0].Equals("1");
                if (step12_1 && step12_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step13 - Check the cached storage folder for corresponding VolumeID (egVolumeID=1 is for C\:Cache)	
                bool step13 = Directory.Exists(cachepath + "\\" + PatientIDs[1] + "-" + StudyUID[1]);
                if (step13)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step14 - Check the uncompressed series folder (appended with -u)
                //eg: 1.2.840.113697.1.2.1530123881.3633541310.9988.1530123881.7.1 - u
                Cachedstudypath[1] = Cachedstudypath[1] + "-u";
                Thread.Sleep(60000);                
                bool isFileFound_3 = BasePage.CheckFile(Cachedstudyname[1], Cachedstudypath[1], "dcm");
                bool isFileFound_4 = BasePage.CheckFile("seriesinfo", Cachedstudypath[1], "xml");
                if (isFileFound_3 && isFileFound_4)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The dcm file found is " + isFileFound_3 + " The xml File found is " + isFileFound_4);
                }

                //step15 - 1) Launch service tool
                //2)Navigate to Enable Feature tab > Pre - fetch cache service
                //3) Scroll down in service tool to > Auto Decompress settings
                //4) UnCheck the checkbox "Allow Auto Decompression of encapsulated data with lossless transfer syntaxes"
                //5) UnCheck "Allow Auto decompression for lossy data."
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab(ServiceTool.EnableFeatures_Tab);
                servicetool.NavigateSubTab(ServiceTool.EnableFeatures.Name.PrefetchCacheService);
                servicetool.ClickModifyButton();
                PreFTab = wpfobject.GetUIItem<ITabPage, Tab>(WpfObjects._mainWindow.Get<Tab>(SearchCriteria.All).SelectedTab);
                PreFTab.SelectTabPage(ServiceTool.EnableFeatures.Name.CacheStoreSCPSettings);
                isLossyDataChecked = wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfLossyData, 1);
                if (isLossyDataChecked)
                    wpfobject.UnSelectCheckBox(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfLossyData, 1);
                isEncapsultedDataChecked = wpfobject.IsCheckBoxSelected(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfEncapsulatedData, 1);
                if (isEncapsultedDataChecked)
                    wpfobject.UnSelectCheckBox(ServiceTool.EnableFeatures.Name.AllowAutoDecompressionOfEncapsulatedData, 1);
                isEncapsultedDataChecked = wpfobject.IsCheckBoxSelected("Allow Auto Decompression of encapsulated data with lossless transfer syntaxes", 1);
                isLossyDataChecked = wpfobject.IsCheckBoxSelected("Allow Auto Decompression for lossy data", 1);
                if (!isEncapsultedDataChecked && !isLossyDataChecked)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                servicetool.ClickApplyButtonFromTab();
                servicetool.RestartService();
                servicetool.CloseServiceTool();

                //step16 - Upload a study(Dicom with lossless transfer syntax) to the data source (BR_EA) that has prefetch configured and node added on EA
                var file3 = BasePage.WriteDicomFile(studypathslist[2], new DicomTag[] { DicomTag.StudyDate, DicomTag.StudyTime }, new String[] { DateTime.Now.ToString("yyyyMMdd"), (DateTime.Now.Subtract(new TimeSpan(0, 20, 0)).ToString("HHmmss")) });
                client = new DicomClient();
                client.AddRequest(new DicomCStoreRequest(file3));
                client.Send(datasourceip, 12000, false, "SCU", Config.EA91AETitle);
                ExecutedSteps++;

                //step17 -Wait for the time specified in polling interval (eg 2 mins)
                cachewait.Timeout = new TimeSpan(0, 10, 0);
                cachewait.IgnoreExceptionTypes(new Type[] { new FileNotFoundException().GetType() });
                bool FileFound_2 = false;
                cachewait.Until<Boolean>((d) =>
                {

                    var isFileFound = BasePage.CheckFile(Cachedstudyname[2], Cachedstudypath[2], "dcm");
                    if (isFileFound)
                    {
                        FileFound_2 = true;
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                });
                if (FileFound_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step18 - Check the cached storage folder for corresponding VolumeID (egVolumeID=1 is for C\:Cache)	
                var step18 = Directory.Exists(cachepath + "\\" + PatientIDs[2] + "-" + StudyUID[2]);
                if (step18)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step19 - Check the folder
                //eg: 1.2.840.113697.1.2.1530125917.3633541310.9988.1530125917.13.1                
                Thread.Sleep(60000);
                var isFileFound_5 = BasePage.CheckFile(Cachedstudyname[2], Cachedstudypath[2], "dcm");
                var isFileFound_6 = BasePage.CheckFile("seriesinfo", Cachedstudypath[2], "xml");
                if (isFileFound_5 && isFileFound_6)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    Logger.Instance.InfoLog("The dcm file found is " + isFileFound_5 + " The xml File found is " + isFileFound_6);
                }

                //Return Result.
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
                BasePage.Driver.Navigate().GoToUrl("https://" + datasourceip + "/webadmin");
                var hphome = hplogin.LoginHPen(Config.hpUserName, Config.hpPassword);
                var workflow = (WorkFlow)hphome.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                foreach (String PatientID in PatientIDs)
                {
                    workflow.HPSearchStudy("PatientID", PatientID);
                    if (BasePage.Driver.FindElements(By.CssSelector("#tabrow>tbody tr")).Count > 0)
                        workflow.HPDeleteStudy();
                }
                hplogin.LogoutHPen();
            }
        }
    }
}
