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
using TestStack.White.UIItems.WindowItems;
//using Selenium.FederatedRadiologyQuery;

namespace Selenium.Scripts.Tests
{
    class AdministrationSecurity
    {
        public Login login { get; set; }
        public string filepath { get; set; }

        Studies studies = new Studies();
        ServiceTool servicetool = new ServiceTool();
        WpfObjects wpfobject = new WpfObjects();

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public AdministrationSecurity(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        /// <summary>
        /// Security - 7.0 Secured Communication
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_66140(String testid, String teststeps, int stepcount)
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

                //Step 2 - In the client machine open a web browser and as URL type:http://www.service-repository.com/client/start                   
                ExecutedSteps++;

                //Step 3 - In the WSDL edit box enter the URL of the service you want to call 
                //FederatedRadiologyQueryASMX request = new FederatedRadiologyQueryASMX();
                ExecutedSteps++;

                //Step 4 - Select any of the available operations
                //request.SeriesQuery(new FederatedRadiologyQueryRequest().PatientIDList);
                ExecutedSteps++;

                //Step 5 - Type in the PatientID field a value; for ex. '1'
                ExecutedSteps++;

                //Step 6 - Close the web browser
                ExecutedSteps++;

                //Step 7 -  configuration to use 'unsecured communication'
                ExecutedSteps++;

                //Step 8 - Repeat the same steps you executed using 'secured communication'.
                ExecutedSteps++;

                //Step 9 - In the WSDL edit box enter the URL of the service you want to call
                ExecutedSteps++;

                //Step 10 - Select any of the available operations. 
                ExecutedSteps++;

                //Step 11 - Type in the PatientID field a value; for ex. '1'
                ExecutedSteps++;

                //Step 12 - Close the web browser
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
