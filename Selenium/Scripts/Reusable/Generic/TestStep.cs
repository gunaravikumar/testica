using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Reusable.Generic;
using System.Configuration;
using Selenium.Scripts.Pages;
using System.Drawing;

namespace Selenium.Scripts.Reusable.Generic
{
    public class TestStep
    {  

        public TestStep() 
        {
            this.Status = "No Run";
            this.description = String.Empty;
            this.testimagepath = String.Empty;
            this.goldimagepath = String.Empty;
            this.diffimagepath = String.Empty;
            this.statuslist = new List<String>();
        }

        private String Status;
        public String status
        {
            get
            {
                return this.Status;
            }
            set
            {
                if (value.ToLower().Contains("pass"))
                    this.Status = "PASS";
                else if (value.ToLower().Contains("fail"))
                    this.Status = "FAIL";
                else if (value.ToLower().Contains("skip"))
                    this.Status = "SKIP";
                else if (value.ToLower().Contains("no run"))
                    this.Status = "NO RUN";
                //If user tries to update any status like No Automation, On-Hold, or others
                //it will be set only to Not Automated
                else
                    this.Status = "NOT AUTOMATED";
            }

        }
        public IList<String> statuslist;
        public String description;
        public String expectedresult;
        public String actualresult;
        public String snapshotpath;
        public String comments;
        public String testimagepath;
        public String goldimagepath;
        public String diffimagepath;
        public int stepnumber;


        /// <summary>
        /// This method will update the snapshot field in the Step object
        /// </summary>
        public void SetLogs()
        {
            try
            {
                //Create directory if not created before.
                String screenshotpath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) +
                Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "Screenshot";
                Directory.CreateDirectory(screenshotpath);
                String filename = string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".jpg";

                String filepath = screenshotpath + Path.DirectorySeparatorChar + filename;
                if (Config.BrowserType.Contains("remote") || Config.BrowserType.Contains("cross"))
                {
                   Screenshot driversnapshot = ((ITakesScreenshot)BasePage.Driver).GetScreenshot();
                    driversnapshot.SaveAsFile(filepath, ScreenshotImageFormat.Jpeg);                                   
                }
                else
                {
                   Bitmap windowsnapshot = new ScreenShot().Capture(ScreenShot.enmScreenCaptureMode.Screen);
                    windowsnapshot.Save(filepath, ImageFormat.Jpeg);
                }

                this.snapshotpath = "Screenshot" + Path.DirectorySeparatorChar + filename;   
            }
            catch (Exception e) { }
        }

        /// <summary>
        /// This method will update the comment field in the step object
        /// </summary>
        /// <param name="exception"></param>
        public void SetLogs(Exception exception)
        {
            this.SetLogs();
            this.comments = exception.Message + Environment.NewLine + exception.StackTrace + Environment.NewLine+ exception.InnerException;
           
        }

        /// <summary>
        /// This method will set the test image and gold image path for the step
        /// </summary>
        public void SetPath(String testid, int executedstep, int CompareCount = 0)
        {
            String TestImages = String.Empty;
            String GoldImages = String.Empty;
            String DiffImages = String.Empty;

            //Create directory if not created before.
            if (Config.Theme.ToLower().Equals("grey"))
            {
                TestImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar +
                    "Test Result" + Path.DirectorySeparatorChar + "TestImages" + Path.DirectorySeparatorChar + Config.BrowserType +
                    Path.DirectorySeparatorChar + "Grey";
                Directory.CreateDirectory(TestImages);
                GoldImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar +
                    "Test Result" + Path.DirectorySeparatorChar + "GoldImages" + Path.DirectorySeparatorChar + Config.BrowserType +
                     Path.DirectorySeparatorChar + "Grey";                    
                Directory.CreateDirectory(GoldImages);
                DiffImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar +
                    "Test Result" + Path.DirectorySeparatorChar + "DiffImages" + Path.DirectorySeparatorChar + Config.BrowserType +
                    Path.DirectorySeparatorChar + "Grey";
                Directory.CreateDirectory(DiffImages);
            }
            else
            {
                TestImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TestImages" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(TestImages);
                GoldImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "GoldImages" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(GoldImages);
                DiffImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "DiffImages" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(DiffImages);
            }                

            //Set the path values
            if (CompareCount == 0)
            {   
                this.goldimagepath = GoldImages + Path.DirectorySeparatorChar + testid + "_" + executedstep + ".jpg";
                this.testimagepath = TestImages + Path.DirectorySeparatorChar + testid + "_" + executedstep + ".jpg";
                this.diffimagepath = DiffImages + Path.DirectorySeparatorChar + testid + "_" + executedstep + ".jpg";                
            }
            else
            {
                this.goldimagepath = GoldImages + Path.DirectorySeparatorChar + testid + "_" + executedstep + "-" + CompareCount + ".jpg";
                this.testimagepath = TestImages + Path.DirectorySeparatorChar + testid + "_" + executedstep + "-" + CompareCount + ".jpg";
                this.diffimagepath = DiffImages + Path.DirectorySeparatorChar + testid + "_" + executedstep + "-" + CompareCount + ".jpg";
            }
        }

        /// <summary>
        /// This method will is to Pass the Current Step 
        /// </summary>
        public void StepPass(string logMessage = null)
        {
            this.status = "Pass";
            Logger.Instance.InfoLog("-->Test Step * "+ this.stepnumber +"* -Passed--" + this.description);
            Logger.Instance.InfoLog(logMessage);
            if (logMessage != null)
                this.actualresult = logMessage;
        }

        /// <summary>
        /// This method will is to Fail the Current Step 
        /// </summary>
        public void StepFail(string logMessage = null , bool UpdateErrorLogAtReport = true)
        {
            try
            {
                if (logMessage != null && logMessage != "" && UpdateErrorLogAtReport == true)
                    this.comments = logMessage;
                Logger.Instance.ErrorLog("-->Test Step * " + this.stepnumber + "* -Failed--" + this.description);
                this.status = "Fail";
                this.SetLogs();
                if (logMessage != null)
                    this.actualresult = logMessage;
            }
            catch (Exception e) { }
        }

        /// <summary>
        /// This method will is to Pass the Current Step 
        /// </summary>
        public void StepStatus(bool status)
        {
            if (status)
                StepPass();
            else
                StepFail();
        }

        /// <summary>
        /// This method will add the "pass" the current "Statuslist" to string List varibale 
        /// </summary>
        public void AddPassStatusList(string logMessage = null)
        {
            this.statuslist.Add("Pass");
            Logger.Instance.InfoLog(logMessage);

        }

        /// <summary>
        /// This method will add the "Fail" the current "Statuslist" to string List varibale 
        /// </summary>
        public void AddFailStatusList(string logMessage = null)
        {
            if (logMessage != null && logMessage != "")
            {
                this.comments = this.comments + "Verification failed : " + logMessage + ",";
                Logger.Instance.ErrorLog("-->Test case Verification Failed--" + logMessage);
            }
            this.statuslist.Add("Fail");
            //this.SetLogs();
        }




    }
}
