using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Selenium.Scripts.Pages.eHR;
using Microsoft.Win32;
using Dicom;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.HoldingPen;
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using OpenQA.Selenium;
using System.Threading;
using OpenQA.Selenium.Interactions;
using System.Windows.Forms;
using System.Drawing.Imaging;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Scripts.Tests
{
    class BluRing3DView : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public DomainManagement domain { get; set; }
        public RoleManagement role { get; set; }
        public UserManagement user { get; set; }
        public StudyViewer studyviewer { get; set; }
        public ServiceTool servicetool { get; set; }
        public EHR ehr { get; set; }
        public HPLogin hplogin { get; set; }
        public Web_Uploader webuploader { get; set; }
        public ExamImporter ei { get; set; }
        public HPHomePage hphomepage { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public WpfObjects wpfobject { get; set; }
        public Viewer viewer { get; set; }
        public object MouseSimulator { get; private set; }
        public BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
        public TestCompleteAction tca = new TestCompleteAction();
        public BluRing3DView(String classname)
        {
            login = new Login();
            BasePage.InitializeControlIdMap();
            login.DriverGoTo(login.url);
            domain = new DomainManagement();
            role = new RoleManagement();
            user = new UserManagement();
            studyviewer = new StudyViewer();
            viewer = new Viewer();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            hplogin = new HPLogin();
            ei = new ExamImporter();
            webuploader = new Web_Uploader();
            hphomepage = new HPHomePage();
            mpaclogin = new MpacLogin();
            wpfobject = new WpfObjects();
            servicetool = new ServiceTool();
            ehr = new EHR();

        }
        public TestCaseResult Test_153823(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            ServiceTool servicetool = new ServiceTool();
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            string licensefilepath = Config.licensefilepath;
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                //Fetch required Test data
                String adminUserName = Config.adminUserName;
                String adminPassword = Config.adminPassword;
                String AdminDomain = Config.adminGroupName;
                String DefaultRole = Config.adminRoleName;
                String objpatid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String objthumbimg = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailImage");

                //step 01
                login.LoginIConnect(adminUserName, adminPassword);
                bool res = brz3dvp.searchandopenstudyin3D(objpatid , objthumbimg);
                if(!res)
                    result.steps[++ExecutedSteps].status = "Fail";
                else
                    result.steps[++ExecutedSteps].status = "Pass";
                brz3dvp.select3DTools(Z3DTools.Reset);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.ToolBoxComponent)));
                res = brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.ToolBoxComponent)));
                if (res)
                {
                    IWebElement se = brz3dvp.controlelement("Navigation 1");
                    new Actions(Driver).MoveToElement(se,se.Size.Width-5,se.Size.Height-5).Click().Build().Perform();
                    tca.MouseScroll(brz3dvp.controlelement("Navigation 1"),"down","200");
                }
                else
                    result.steps[++ExecutedSteps].status = "Pass";

                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
        }

    }
}
