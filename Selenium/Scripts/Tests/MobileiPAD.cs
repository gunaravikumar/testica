using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using System.IO;

namespace Selenium.Scripts.Tests
{
    class MobileiPAD
    {

        private Login login;
        private String filepath;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public MobileiPAD(String classname)
        {
            this.login = new Login();
            this.filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        /// <summary>
        ///  Pan Tool
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_73544(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int executedsteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

            try
            {
                //Step-1-Login and load study
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var studies  = login.Navigate<Studies>();
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy("Accession", accession);
                var viewer = studies.LaunchStudy();

                //Step-2 - Select Pan tool
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);

                //Step-3 and 4
                viewer.JQDaragAndDrop("", "");
                

                //Return Result
                result.FinalResult(executedsteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                return result;
            }

            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedsteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;

            }
        }

        /// <summary>
        ///  User Guide should include a"not for daignostic use"statement
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27739(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables         
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int executedsteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

            try
            {
                //Step-1-Precondition
                executedsteps++;

                //Step-2 Login into iConnect and 
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domain = login.Navigate<DomainManagement>();             
                

                return result;
            }

            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedsteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;

            }
        }

    }
}
