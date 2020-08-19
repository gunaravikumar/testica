using System;
using System.Globalization;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
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
using ComboBox = TestStack.White.UIItems.ListBoxItems.ComboBox;
using Selenium.Scripts.Pages.eHR;

namespace Selenium.Scripts.Tests
{
    class ExternalApplicationPriorsTrans : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public EHR ehr { get; set; }

        Studies studies = new Studies();
        ServiceTool servicetool = new ServiceTool();
        StudyViewer viewer;

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public ExternalApplicationPriorsTrans(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            wpfobject = new WpfObjects();
        }

        /// <summary>
        /// Priors Transfer
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27593(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            BasePage basepage = new BasePage();

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //Fetch required Test data                

                //Step 1 - Pre-conditions
                ExecutedSteps++;

                //Step 2 - Pre-conditions 
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                Thread.Sleep(10000);
                wpfobject.SelectTabFromTabItems(ServiceTool.Encryption.Name.Encryption_tab);
                Thread.Sleep(10000);
                servicetool.Enc_ServiceTab().Focus();
                servicetool.Enc_ServiceTab().Click();
                servicetool.EnterServiceEntry(Key: "Patient.Query", Assembly: "OpenContent.Platform.Data.dll", 
                                              Class: "OpenContent.Data.Patient.Query.Services.EmergePatientQuery");
                servicetool.EnterServiceParameters("configFile", "string", "Config\\EmergeServicesConfiguration.xml");
                ExecutedSteps++;

                //Step 3 - In the Enable Features -- EMPI Select and enable the EMPI flag.
                servicetool.EnableMergeEMPI();
                servicetool.RestartService();
                servicetool.wpfobject.WaitTillLoad();
                servicetool.CloseConfigTool();
                servicetool.RestartIIS();
                ExecutedSteps++;

                //Step 4 - Setup TestEHR application
                ExecutedSteps++;

                //Step 5 - From test server, run the Test EHR application
                ehr.LaunchEHR();
                ehr.SetCommonParameters();
                wpfobject.GetMainWindow("Test WebAccess EHR");
                wpfobject.SelectTabFromTabItems("Transfer Priors");
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 6 - Fill out the page with the following attributes from datasource DS1:
                wpfobject.SetText(EHR.TransferPriors.ID.LastName, "John");
                wpfobject.SetText(EHR.TransferPriors.ID.FirstName, "Doe");
                wpfobject.SelectCheckBox(EHR.TransferPriors.ID.PatientIDCB);
                wpfobject.SetText(EHR.TransferPriors.ID.PatientID, "4455");
                wpfobject.SetText(EHR.TransferPriors.ID.DestinationID, "");
                wpfobject.ClickButton(EHR.TransferPriors.ID.StartBtn);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 7 - Use the same patient but this time fill out the Assigning Authority box: TOH
                wpfobject.SetText(EHR.TransferPriors.ID.LastName, "John");
                wpfobject.SetText(EHR.TransferPriors.ID.FirstName, "Doe");
                wpfobject.SelectCheckBox(EHR.TransferPriors.ID.PatientIDCB);
                wpfobject.SetText(EHR.TransferPriors.ID.PatientID, "4455");
                wpfobject.SetText(EHR.TransferPriors.ID.DestinationID, "");
                wpfobject.ClickRadioButtonById(EHR.TransferPriors.ID.AssigningAuthorityRadioBtn);
                wpfobject.SetText(EHR.TransferPriors.ID.AssigningAuthorityTxtBox, "TOH");
                wpfobject.ClickButton(EHR.TransferPriors.ID.StartBtn);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 8 - Go to destination server, verify these priors studies if these are all received
                //TODO
                ExecutedSteps++;

                //Step 9 - Fill out the page with the following attributes
                wpfobject.SetText(EHR.TransferPriors.ID.LastName, "John");
                wpfobject.SetText(EHR.TransferPriors.ID.FirstName, "Doe");
                wpfobject.SelectCheckBox(EHR.TransferPriors.ID.PatientIDCB);
                wpfobject.SetText(EHR.TransferPriors.ID.PatientID, "999");
                wpfobject.SetText(EHR.TransferPriors.ID.DestinationID, "");
                wpfobject.ClickRadioButtonById(EHR.TransferPriors.ID.AssigningAuthorityRadioBtn);
                wpfobject.SetText(EHR.TransferPriors.ID.AssigningAuthorityTxtBox, "NYH");
                wpfobject.ClickButton(EHR.TransferPriors.ID.StartBtn);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 10 - Go to destination server, verify these priors studies if these are all received.
                //TODO
                ExecutedSteps++;

                //Step 11 - Fill out the page with the following attributes:
                wpfobject.SetText(EHR.TransferPriors.ID.LastName, "John");
                wpfobject.SetText(EHR.TransferPriors.ID.FirstName, "Doe");
                wpfobject.SelectCheckBox(EHR.TransferPriors.ID.PatientIDCB);
                wpfobject.SetText(EHR.TransferPriors.ID.PatientID, "999");
                wpfobject.SetText(EHR.TransferPriors.ID.DestinationID, "");
                wpfobject.ClickRadioButtonById(EHR.TransferPriors.ID.AssigningAuthorityRadioBtn);
                wpfobject.SetText(EHR.TransferPriors.ID.AssigningAuthorityTxtBox, "NYH");
                wpfobject.ClickButton(EHR.TransferPriors.ID.StartBtn);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Setp 12 - Go to destination server, verify these priors studies if these are all received.
                //TODO
                ExecutedSteps++;

                //Step 13 - Fill out the page with the following attributes:
                wpfobject.SetText(EHR.TransferPriors.ID.LastName, "");
                wpfobject.SetText(EHR.TransferPriors.ID.FirstName, "");
                wpfobject.SelectCheckBox(EHR.TransferPriors.ID.PatientIDCB);
                wpfobject.SetText(EHR.TransferPriors.ID.PatientID, "1261234");
                wpfobject.SetText(EHR.TransferPriors.ID.DestinationID, "");
                wpfobject.ClickRadioButtonById(EHR.TransferPriors.ID.AssigningAuthorityRadioBtn);
                wpfobject.SetText(EHR.TransferPriors.ID.AssigningAuthorityTxtBox, "TOH");
                wpfobject.ClickButton(EHR.TransferPriors.ID.StartBtn);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 14 - Go to destination server, verify these priors studies if these are all received.
                //TODO
                ExecutedSteps++;

                //Step 15 - Fill out the page with the following attributes:
                wpfobject.SetText(EHR.TransferPriors.ID.FirstName, "Natalie");
                wpfobject.SetText(EHR.TransferPriors.ID.LastName, "Gardner");                
                wpfobject.SelectCheckBox(EHR.TransferPriors.ID.PatientIDCB);
                wpfobject.SetText(EHR.TransferPriors.ID.PatientID, "371941132");
                wpfobject.SetText(EHR.TransferPriors.ID.DestinationID, "");
                wpfobject.ClickRadioButtonById(EHR.TransferPriors.ID.IssuerOfPIDRadioBtn);
                wpfobject.SetText(EHR.TransferPriors.ID.IssuerOfPIDTxtBox, "TOH_D");
                wpfobject.ClickButton(EHR.TransferPriors.ID.StartBtn);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 16 - Go to destination server, verify these priors studies if these are all received.
                //TODO
                ExecutedSteps++;

                //Step 17 - Fill out the page with the following attributes
                wpfobject.SetText(EHR.TransferPriors.ID.FirstName, "Sites");
                wpfobject.SetText(EHR.TransferPriors.ID.LastName, "Two");                
                wpfobject.SelectCheckBox(EHR.TransferPriors.ID.PatientIDCB);
                wpfobject.SetText(EHR.TransferPriors.ID.PatientID, "57134534");
                wpfobject.SetText(EHR.TransferPriors.ID.DestinationID, "");
                wpfobject.ClickRadioButtonById(EHR.TransferPriors.ID.AssigningAuthorityRadioBtn);
                wpfobject.SetText(EHR.TransferPriors.ID.IssuerOfPIDTxtBox, "ABC");
                wpfobject.ClickButton(EHR.TransferPriors.ID.StartBtn);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 18 - Fill out the page with the following attributes
                wpfobject.SetText(EHR.TransferPriors.ID.FirstName, "Sites");
                wpfobject.SetText(EHR.TransferPriors.ID.LastName, "Two");
                wpfobject.SelectCheckBox(EHR.TransferPriors.ID.PatientIDCB);
                wpfobject.SetText(EHR.TransferPriors.ID.PatientID, "57134534");
                wpfobject.SetText(EHR.TransferPriors.ID.DestinationID, "");
                wpfobject.ClickRadioButtonById(EHR.TransferPriors.ID.IssuerOfPIDRadioBtn);
                wpfobject.SetText(EHR.TransferPriors.ID.IssuerOfPIDTxtBox, "ABC");
                wpfobject.ClickRadioButtonById(EHR.TransferPriors.ID.AssigningAuthorityRadioBtn);
                wpfobject.SetText(EHR.TransferPriors.ID.AssigningAuthorityTxtBox, "");
                wpfobject.ClickButton(EHR.TransferPriors.ID.StartBtn);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 19 - Fill out the page with the following attributes
                wpfobject.SetText(EHR.TransferPriors.ID.FirstName, "Sites");
                wpfobject.SetText(EHR.TransferPriors.ID.LastName, "Two");
                wpfobject.SelectCheckBox(EHR.TransferPriors.ID.PatientIDCB);
                wpfobject.SetText(EHR.TransferPriors.ID.PatientID, "000000");
                wpfobject.SetText(EHR.TransferPriors.ID.DestinationID, "");
                wpfobject.ClickRadioButtonById(EHR.TransferPriors.ID.AssigningAuthorityRadioBtn);
                wpfobject.SetText(EHR.TransferPriors.ID.IssuerOfPIDTxtBox, "TOH");
                wpfobject.ClickButton(EHR.TransferPriors.ID.StartBtn);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 20 - Fill out the page with the following attributes
                wpfobject.SetText(EHR.TransferPriors.ID.FirstName, "Sites");
                wpfobject.SetText(EHR.TransferPriors.ID.LastName, "Two");
                wpfobject.SelectCheckBox(EHR.TransferPriors.ID.PatientIDCB);
                wpfobject.SetText(EHR.TransferPriors.ID.PatientID, "57134534");
                wpfobject.SetText(EHR.TransferPriors.ID.DestinationID, "");
                wpfobject.ClickRadioButtonById(EHR.TransferPriors.ID.AssigningAuthorityRadioBtn);
                wpfobject.SetText(EHR.TransferPriors.ID.IssuerOfPIDTxtBox, "TOH");
                wpfobject.ClickButton(EHR.TransferPriors.ID.StartBtn);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 21 - Go to destination server, verify these priors studies if these are all received.
                //TODO
                ExecutedSteps++;

                //Step 22 - Fill out the page with the following attributes
                wpfobject.SetText(EHR.TransferPriors.ID.FirstName, "Kirk");
                wpfobject.SetText(EHR.TransferPriors.ID.LastName, "Hammet");                
                wpfobject.SetText(EHR.TransferPriors.ID.DestinationID, "");
                wpfobject.ClickRadioButtonById(EHR.TransferPriors.ID.AssigningAuthorityRadioBtn);
                wpfobject.SetText(EHR.TransferPriors.ID.IssuerOfPIDTxtBox, "NYH");
                wpfobject.ClickButton(EHR.TransferPriors.ID.StartBtn);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 23 - Go to destination server, verify these priors studies if these are all received.
                //TODO
                ExecutedSteps++;

                //Step 24 - Fill out the page with the following attributes
                wpfobject.SetText(EHR.TransferPriors.ID.FirstName, "Doe");
                wpfobject.SetText(EHR.TransferPriors.ID.LastName, "John");
                wpfobject.SetText(EHR.TransferPriors.ID.DestinationID, "999");
                wpfobject.ClickRadioButtonById(EHR.TransferPriors.ID.AssigningAuthorityRadioBtn);
                wpfobject.SetText(EHR.TransferPriors.ID.IssuerOfPIDTxtBox, "NYH");
                wpfobject.ClickButton(EHR.TransferPriors.ID.StartBtn);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 25 - Go to destination server, verify these priors studies if these are all received.
                //TODO
                ExecutedSteps++;

                //Step 26 - Fill out the page with the following attributes
                wpfobject.SetText(EHR.TransferPriors.ID.FirstName, "Doe");
                wpfobject.SetText(EHR.TransferPriors.ID.LastName, "John");
                wpfobject.SetText(EHR.TransferPriors.ID.DestinationID, "999");
                wpfobject.ClickRadioButtonById(EHR.TransferPriors.ID.AssigningAuthorityRadioBtn);
                wpfobject.SetText(EHR.TransferPriors.ID.IssuerOfPIDTxtBox, "NYH");
                wpfobject.ClickButton(EHR.TransferPriors.ID.StartBtn);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 27 - Fill out the page with the following attributes
                wpfobject.SetText(EHR.TransferPriors.ID.FirstName, "Cliff");
                wpfobject.SetText(EHR.TransferPriors.ID.LastName, "Burton");
                wpfobject.SetText(EHR.TransferPriors.ID.DestinationID, "999");
                wpfobject.ClickRadioButtonById(EHR.TransferPriors.ID.AssigningAuthorityRadioBtn);
                wpfobject.SetText(EHR.TransferPriors.ID.IssuerOfPIDTxtBox, "TOH");
                wpfobject.SetText(EHR.TransferPriors.ID.StudyUIDTxtBox, "1.2.542.357159.2.1.1.196208194321621.156");
                wpfobject.ClickButton(EHR.TransferPriors.ID.StartBtn);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 28 - Go to destination server, verify these priors studies if these are all received.
                //TODO
                ExecutedSteps++;

                //Step 29 - Patient.Identity and Patient.Query not exist
                //TODO
                ExecutedSteps++;

                //Step 30 - At Test EHR's Transfer Prior's page, fill out the page with the following attributes
                wpfobject.SetText(EHR.TransferPriors.ID.FirstName, "Sites");
                wpfobject.SetText(EHR.TransferPriors.ID.LastName, "Three");
                wpfobject.SetText(EHR.TransferPriors.ID.DestinationID, "64148921");
                wpfobject.ClickRadioButtonById(EHR.TransferPriors.ID.AssigningAuthorityRadioBtn);
                wpfobject.SetText(EHR.TransferPriors.ID.IssuerOfPIDTxtBox, "TOH");                
                wpfobject.ClickButton(EHR.TransferPriors.ID.StartBtn);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 31 - Go to destination server, verify these priors studies if these are all received.
                //TODO
                ExecutedSteps++;

                //Step 32 - At Test EHR's Transfer Prior's page, fill out the page with the following attributes
                wpfobject.SetText(EHR.TransferPriors.ID.FirstName, "Sites");
                wpfobject.SetText(EHR.TransferPriors.ID.LastName, "Three");
                wpfobject.SetText(EHR.TransferPriors.ID.DestinationID, "");
                wpfobject.ClickRadioButtonById(EHR.TransferPriors.ID.AssigningAuthorityRadioBtn);
                wpfobject.SetText(EHR.TransferPriors.ID.IssuerOfPIDTxtBox, "");
                wpfobject.ClickButton(EHR.TransferPriors.ID.StartBtn);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 33 - Go to destination server, verify these priors studies if these are all received.
                //TODO
                ExecutedSteps++;

                //Step 34 - At Test EHR's Transfer Prior's page, fill out the page with the following attributes
                wpfobject.SetText(EHR.TransferPriors.ID.FirstName, "Sites");
                wpfobject.SetText(EHR.TransferPriors.ID.LastName, "Three");
                wpfobject.SetText(EHR.TransferPriors.ID.DestinationID, "");
                wpfobject.ClickRadioButtonById(EHR.TransferPriors.ID.AssigningAuthorityRadioBtn);
                wpfobject.SetText(EHR.TransferPriors.ID.IssuerOfPIDTxtBox, "");
                wpfobject.ClickButton(EHR.TransferPriors.ID.StartBtn);
                wpfobject.WaitTillLoad();
                ExecutedSteps++;

                //Step 35 - Go to destination server, verify these priors studies if these are all received.
                //TODO
                ExecutedSteps++;

                //Step 36 - Pre-condition
                ExecutedSteps++;

                //Step 37 - Encryption Service List box, select Patient.Query and select Delete button
                ExecutedSteps++;

                //Step 38 - Select Add button and enter the followings
                servicetool.LaunchServiceTool();
                wpfobject.GetMainWindow(ServiceTool.ConfigTool_Name);
                wpfobject.WaitTillLoad();
                Thread.Sleep(10000);
                wpfobject.SelectTabFromTabItems(ServiceTool.Encryption.Name.Encryption_tab);
                Thread.Sleep(10000);
                servicetool.Enc_ServiceTab().Focus();
                servicetool.Enc_ServiceTab().Click();
                servicetool.EnterServiceEntry(Key: "Patient.Identity", Assembly: "OpenContent.Domain.Data.dll",
                                              Class: "OpenContent.Data.Patient.Query.Services.IHEPatientIdentitiesQuery");
                servicetool.EnterServiceParameters("configFile", "string", "Config\\PIXConfiguration.xml");              
                servicetool.RestartService();
                servicetool.wpfobject.WaitTillLoad();
                servicetool.CloseConfigTool();
                servicetool.RestartIIS();
                ExecutedSteps++;

                //Step 39
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
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }
    }
}
