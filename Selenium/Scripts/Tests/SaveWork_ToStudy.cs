using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using TestComplete;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using Accord.Imaging;
using IronOcr;
using Accord.Imaging.Filters;
using System.Runtime.InteropServices;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Pages.HoldingPen;


namespace Selenium.Scripts.Tests
{
    class SaveWork_ToStudy:BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public DomainManagement domain { get; set; }
        public StudyViewer studyviewer { get; set; }
        public Cursor Cursor { get; private set; }


        public SaveWork_ToStudy(String classname)
        {

            login = new Login();
            login.DriverGoTo(login.url);
            domain = new DomainManagement();

            studyviewer = new StudyViewer();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }
        public TestCaseResult Test_163536(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            //  ICAZ3DViewerPage z3dvp = new ICAZ3DViewerPage();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            BluRingViewer Two_d_viewer = new BluRingViewer();
            WpfObjects wpfobject = new WpfObjects();
            BasePage basepage = new BasePage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            String Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string testdatades = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] ssplit = testdatades.Split('|');
            string sthumbpattern = ssplit[0];
            string sWindowlevel_value = ssplit[1];
            string sstudyid = ssplit[2];
            string sSeriesdes = ssplit[3];
          //  string Accession = ssplit[4];
            String username = Config.adminUserName;
            String password = Config.adminPassword;

            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {

                z3dvp.Deletefiles(testcasefolder);
             //   string EASERver = "Z3DEaServer";
                string tempbrowser = Config.BrowserType;
                //For EA Open Purpose
                Config.BrowserType = "internet explorer";
                BasePage.Driver.Close();
                BasePage.Driver.Quit();
                BasePage.Driver = null;
                String hpUserName = Config.hpUserName;
                String hpPassword = Config.hpPassword;
                string tempholdingpen = Config.HoldingPenIP;

                Config.HoldingPenIP = BluRingZ3DViewerPage.Z3DEaServer;
                //z3dvp.DeletePriorsInEA(Config.HoldingPenIP, Patientid , Accession);
                z3dvp.DeletePriorsFromEA(Config.HoldingPenIP, Patientid, sstudyid, sSeriesdes);
                Driver.Close();
                Driver.Quit();
                Config.BrowserType = tempbrowser;
                Config.HoldingPenIP = tempholdingpen;
                //Precondition ends here 
                login.LoginIConnect(username, password);

                //Precontion for this test case to delete  the Manipulation saved img from Ea server 
                //step 1 In ICA, log in as 'Administrator' and navigate to studies tab.
                //step 2  Search and select the study in universal viewer with below criteria : 
                //step 3 Select a 3D supported series and Select the MPR view option from the smart view drop down.
                bool res = z3dvp.searchandopenstudyin3D(Patientid, thumbnailcaption);
                if (res == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 4 Click on the save button in the viewport top bar from any one of the navigation control.
                bool bflag4 = false;
                int sLastThumbName = 0;
                sLastThumbName = z3dvp.ThumbNailOperation(BluRingZ3DViewerPage.Navigationone);
                if(sLastThumbName>0)
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
                //Step 5 Click on the close button from the Global toolbar.
                //IWebElement IExit5 = z3dvp.ExitIcon();
                //if (IExit5.Displayed)
                //{
                //    IExit5.Click();
                //    PageLoadWait.WaitForPageLoad(5);
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                //Step 5  Launch the same study again in the universal viewer and verify the saved images.
                //step 5 new Select the 2D view option from the smart view drop down.
                //PageLoadWait.WaitForFrameLoad(10);
                //String FieldName = z3dvp.GetFieldName("patient");
                //login.SearchStudy(FieldName, Patientid);
                //PageLoadWait.WaitForLoadingMessage(30);
                //login.SelectStudy(FieldName, Patientid);
                //PageLoadWait.WaitForFrameLoad(5);
                //var viewer = BluRingViewer.LaunchBluRingViewer(fieldname: FieldName, value: thumbnailcaption);
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Two_2D, "n");
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                int SSavedThum =z3dvp.VerfiySavedThumbNail(sLastThumbName);
                if (SSavedThum > sLastThumbName)
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

                //Step 6 Load the saved images in the active viewport .
                if (SSavedThum > sLastThumbName)
                {
                    string sthumbu2dload = sthumbpattern.Remove(1, 2).Insert(1, SSavedThum.ToString());
                    Thread.Sleep(1000);
                    sthumbu2dload = sthumbu2dload.Remove(9, 5);
                    Thread.Sleep(500);
                    bool thumbnailselction = z3dvp.selectthumbnail(sthumbu2dload);
                    if (thumbnailselction == false)
                    {
                        sthumbu2dload = sthumbpattern.Remove(1, 2).Insert(1, SSavedThum.ToString());
                        thumbnailselction = z3dvp.selectthumbnail(sthumbu2dload);
                    }
                    if (thumbnailselction==false)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                    }
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 7 Apply the Window level and draw measurements in 
                PageLoadWait.WaitForPageLoad(20);
                IList<IWebElement> TwoDPanel4 = z3dvp.Viewpot2D();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                string sBeforewindowvlaue8 = null;
                string sAfterwindowvlaue8 = null;
                bool btool3 = Two_d_viewer.SelectViewerTool(BluRingTools.Window_Level);
                bool bflag3 = false;
                if (btool3)
                {
                    string Beforeocrtext8 = z3dvp.ReadPatientDetailsUsingTesseract(TwoDPanel4[0], 4, 900, 1124, 1400, 1400);
                    if (Beforeocrtext8.Length > 0)
                    {
                        int ilength = Beforeocrtext8.Length;
                        int iindex = Beforeocrtext8.IndexOf("WL:");
                        sBeforewindowvlaue8 = Beforeocrtext8.Substring(iindex + 3, ilength - (iindex + 3));

                    }
                    z3dvp.Performdragdrop(TwoDPanel4[0], 100, 100, TwoDPanel4[0].Size.Width / 2, TwoDPanel4[0].Size.Height / 2);
                    z3dvp.Performdragdrop(TwoDPanel4[0], 100, 100, TwoDPanel4[0].Size.Width / 2, TwoDPanel4[0].Size.Height / 2);
                    Thread.Sleep(5000);
                    string Afterocrtext8 = z3dvp.ReadPatientDetailsUsingTesseract(TwoDPanel4[0], 4, 900, 1124, 1400, 1400);
                    if (Afterocrtext8.Length > 0)
                    {
                        int ilength = Afterocrtext8.Length;
                        int iindex = Afterocrtext8.IndexOf("WL:");
                        sAfterwindowvlaue8 = Afterocrtext8.Substring(iindex + 3, ilength - (iindex + 3));
                    }
                    string[] ssubslitAFter = sAfterwindowvlaue8.Split(new string[] { "<br>", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
                    string[] ssubslitBefoe = sBeforewindowvlaue8.Split(new string[] { "<br>", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
                    if (ssubslitAFter[0].Trim() != ssubslitBefoe[0].Trim())
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        bflag3 = true;
                    }
                }
                if(bflag3==false)
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step 8 Select the 3D supported series and load the series in 3D viewer.
                //Precondtion 
                IWebElement IExit9 = z3dvp.ExitIcon();
                if (IExit9.Displayed)
                {
                    IExit9.Click();
                    PageLoadWait.WaitForPageLoad(5);
                }
                //  z3dvp.SelectStudy_FromSelectedRows("patient", Patientid, thumbnailcaption);
                PageLoadWait.WaitForFrameLoad(10);
                String FieldName9 = z3dvp.GetFieldName("patient");
                login.SearchStudy(FieldName9, Patientid);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy(FieldName9, Patientid);
                PageLoadWait.WaitForFrameLoad(5);
                var viewer9 = BluRingViewer.LaunchBluRingViewer(fieldname: FieldName9, value: thumbnailcaption);
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();

                bool thumbnailselction9 = z3dvp.selectthumbnail(thumbnailcaption);
                bool IMpr = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR);
                 thumbnailselction9 = z3dvp.selectthumbnail(thumbnailcaption);
                if (!thumbnailselction9 && IMpr==false )
                {
                    throw new Exception();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                //Step 9  Modify the image volume in MPR navigation control 1 by applying the following tools:1.Scroll,2.Magnify,3.Rotate,4.Window level
                //Scroll
                bool btscrooltool = z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                IWebElement Inavigationone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((Inavigationone.Location.X + 100), (Inavigationone.Location.Y / 2 + 200));
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(500);
                //Scroll.
                List<string> Beforescroll10 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane,null,null,2);
                new Actions(Driver).SendKeys("x").Build().Perform();
                Thread.Sleep(1000);
                for (int i = 0; i < 20; i++)
                {
                    BasePage.mouse_event(0x0800, 0, 0, -5, 0);
                    Thread.Sleep(1000);
                }
                Thread.Sleep(5000);
                List<string> Afterscroll10 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);

                //Magnify
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                z3dvp.DragandDropelement(BluRingZ3DViewerPage.Navigationone, scrollvalue:false);
                Thread.Sleep(1000);
                List<string> AfterMagnify10 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);

                //Rotate
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                new Actions(Driver).DragAndDropToOffset(Inavigationone, Inavigationone.Location.X + 150, Inavigationone.Location.Y + 100).Build().Perform();
                Thread.Sleep(2000);
                List<string> AfterRotate10 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                
                //Window
                List<string> BeforeWindowlevel10 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                z3dvp.select3DTools(Z3DTools.Window_Level);
                Thread.Sleep(1000);
                z3dvp.Performdragdrop(Inavigationone, 10, 20,40,50);
                Thread.Sleep(3000);
                List<string> AfterWindowlevel10 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                if(AfterWindowlevel10[0].Trim()== sWindowlevel_value)
                {
                    z3dvp.Performdragdrop(Inavigationone, 10, 20, 40, 50);
                    AfterWindowlevel10 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                }
                if (Beforescroll10[0]!=Afterscroll10[0]  && Afterscroll10[0]!=AfterMagnify10[0]  && AfterMagnify10[0]!=AfterRotate10[0] && BeforeWindowlevel10[0]!=AfterWindowlevel10[0])
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
                //step 10 Click on the save button in the viewport top bar from the navigation control 1.
                int sLastThumbName11 = z3dvp.ThumbNailOperation(BluRingZ3DViewerPage.Navigationone);
                if (sLastThumbName11 > 0)
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

                //step 11  Select the 2D view option from the smart view drop down. 
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Two_2D, "n");
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                int SSavedThum13 = z3dvp.VerfiySavedThumbNail(sLastThumbName11);
                if (SSavedThum13 > sLastThumbName11)
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
                //Step 12 Load the saved images in the active viewport .
                String goldimage14 = "";
                if (SSavedThum13 > sLastThumbName11)
                {
                    string sthumbu2dload = sthumbpattern.Remove(1, 2).Insert(1, SSavedThum13.ToString());
                    sthumbu2dload = sthumbu2dload.Remove(9, 5);
                    bool thumbnailselction = z3dvp.selectthumbnail(sthumbu2dload);
                    //once again trying some times 100% coming ,some times it is not coming 
                    if (thumbnailselction == false)
                    {
                        sthumbu2dload = sthumbpattern.Remove(1, 2).Insert(1, SSavedThum13.ToString());
                        thumbnailselction = z3dvp.selectthumbnail(sthumbu2dload);
                    }
                    if (!thumbnailselction)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                        IList<IWebElement> TwoDPanel14 = z3dvp.Viewpot2D();
                        goldimage14 = result.steps[ExecutedSteps].goldimagepath;
                        DownloadImageFile(TwoDPanel14[0], goldimage14);
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                    }
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //STep 13 Compare the Screenshot of the 3D viewer with the saved image in the Active viewport
                IList<IWebElement> TwoDPanel15 = z3dvp.Viewpot2D();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                string   tempimage15 = result.steps[ExecutedSteps].goldimagepath;
                DownloadImageFile(TwoDPanel15[0], tempimage15);
                if (CompareImage(goldimage14, tempimage15,200))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 14  Again launch the study in 3D, Repeat steps 10-15 in other MPR controls.
                bool bflag16_1 = false;
                bool thumbnailselction16 = z3dvp.selectthumbnail(thumbnailcaption);
                bool IMpr16 = z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR);
                thumbnailselction16 = z3dvp.selectthumbnail(thumbnailcaption);
                if (thumbnailselction16== true  && IMpr16 == true  )
                {
                    bflag16_1 = true; 
                    
                }
                else
                {
                    throw new Exception();

                }
                //Modify the image volume in MPR navigation control 1 by applying the following tools://1.Scroll,2.Magnify,3.Rotate,.Window level
                //Scroll
                 z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                IWebElement Inavigationtwo = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((Inavigationtwo.Location.X + 100), (Inavigationtwo.Location.Y / 2 + 200));
                Thread.Sleep(500);
                //Scroll.
                List<string> Beforescroll16 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                //  new Actions(Driver).SendKeys("x").Build().Perform();
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationtwo);
                Thread.Sleep(500);
                for (int i = 0; i < 20; i++)
                {
                    BasePage.mouse_event(0x0800, 0, 0, -5, 0);
                    Thread.Sleep(1000);
                }
                Thread.Sleep(5000);
                List<string> Afterscroll16 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);

                //Magnify
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                Actions ActMag16 = new Actions(Driver);
                //ActMag16.MoveToElement(Inavigationtwo).Click().DragAndDropToOffset(Inavigationtwo, Inavigationtwo.Size.Width + 5, Inavigationtwo.Size.Height + 10).Build().Perform();
                z3dvp.DragandDropelement(BluRingZ3DViewerPage.Navigationtwo, scrollvalue:false);
                Thread.Sleep(2000);
                ActMag16.Release().Build().Perform();
                List<string> AfterMagnify16 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);

                //Rotate
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                if (Config.BrowserType.ToLower() == "firefox" || Config.BrowserType.ToLower() == "mozilla")
                {
                    //new Actions(Driver).DragAndDropToOffset(Inavigationtwo, Inavigationtwo.Location.X/2-10 + 100, Inavigationtwo.Location.Y/2-10 + 100).Build().Perform();
                    //Thread.Sleep(2000);
                    //new Actions(Driver).DragAndDropToOffset(Inavigationtwo, Inavigationtwo.Location.X/2-10 + 100, Inavigationtwo.Location.Y/2-10 + 100).Build().Perform();
                    new Actions(Driver).MoveToElement(Inavigationtwo, Inavigationtwo.Size.Width / 2, Inavigationtwo.Size.Height / 2).ClickAndHold()
                 .MoveToElement(Inavigationtwo, Inavigationtwo.Size.Width / 2, Inavigationtwo.Size.Height / 2 + 100)
                 .Release().Build().Perform();
                    Thread.Sleep(1000);
                    new Actions(Driver).MoveToElement(Inavigationtwo, Inavigationtwo.Size.Width / 2, Inavigationtwo.Size.Height / 2).ClickAndHold()
                 .MoveToElement(Inavigationtwo, Inavigationtwo.Size.Width / 2, Inavigationtwo.Size.Height / 2 + 100)
                 .Release().Build().Perform();
                }
                else
                {
                    //   new Actions(Driver).SendKeys("x").Build().Perform();
                    new Actions(Driver).DragAndDropToOffset(Inavigationtwo, Inavigationtwo.Location.X + 150, Inavigationtwo.Location.Y + 200).Build().Perform();
                    Thread.Sleep(2000);
                    new Actions(Driver).DragAndDropToOffset(Inavigationtwo, Inavigationtwo.Location.X + 150, Inavigationtwo.Location.Y + 200).Build().Perform();
                }
                Thread.Sleep(2000);
             //   new Actions(Driver).SendKeys("x").Build().Perform();
                List<string> AfterRotate16 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                //Window
                List<string> BeforeWindowlevel16 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                z3dvp.select3DTools(Z3DTools.Window_Level);
                //     z3dvp.DragandDropelement(BluRingZ3DViewerPage.Navigationtwo);
                //    new Actions(Driver).MoveToElement(Inavigationtwo, Inavigationtwo.Size.Width - 10, Inavigationtwo.Size.Height - 5).ClickAndHold().DragAndDropToOffset(Inavigationtwo, Inavigationtwo.Size.Width - 10, Inavigationtwo.Size.Height - 200).Release().Build().Perform();

                z3dvp.Performdragdrop(Inavigationtwo, 10, 20,40,50);
                Thread.Sleep(1000);
                List<string> AfterWindowlevel16 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                if (AfterWindowlevel16[1].Trim()== sWindowlevel_value)
                {
                    z3dvp.Performdragdrop(Inavigationtwo, 10, 20, 40, 50);
                    AfterWindowlevel16 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                    Thread.Sleep(1000);
                }
                Thread.Sleep(2000);
               
                //step 16 -second  Click on the save button in the viewport top bar from the navigation control 1.
                int sLastThumbName16_2 = z3dvp.ThumbNailOperation(BluRingZ3DViewerPage.Navigationtwo);

                //Step 16 -third Click on the close button from the Global toolbar.
                bool bflag16_3 = false;
                IWebElement IExit16 = z3dvp.ExitIcon();
                if (IExit16.Displayed)
                {
                    IExit16.Click();
                    Thread.Sleep(1000);
                    bflag16_3 = true;
                }
                //step 16 -four Launch the same study again in the universal viewer and verify the saved images.
                PageLoadWait.WaitForFrameLoad(10);
                String FieldName16 = z3dvp.GetFieldName("patient");
                login.SearchStudy(FieldName16, Patientid);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy(FieldName16, Patientid);
                PageLoadWait.WaitForFrameLoad(5);
                var viewer16 = BluRingViewer.LaunchBluRingViewer(fieldname: FieldName16, value: thumbnailcaption);
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                int SSavedThum16 = z3dvp.VerfiySavedThumbNail(sLastThumbName16_2);
                bool bflag16_5 = false;
                //step 16 - five  Load the saved images in the active viewport .
                string goldimage16_5 = "";
                if (SSavedThum16 > sLastThumbName16_2)
                {
                    string sthumbu2dload = sthumbpattern.Remove(1, 2).Insert(1, SSavedThum16.ToString());

                    sthumbu2dload = sthumbu2dload.Remove(9, 5);
                    bool thumbnailselction = z3dvp.selectthumbnail(sthumbu2dload);
                    //once again trying some times 100% coming ,some times it is not coming 
                    if(thumbnailselction==false)
                    {
                        sthumbu2dload = sthumbpattern.Remove(1, 2).Insert(1, SSavedThum16.ToString());
                        thumbnailselction = z3dvp.selectthumbnail(sthumbu2dload);
                    }
                    if (!thumbnailselction)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 2, 1);
                        IList<IWebElement> TwoDPanel16_5 = z3dvp.Viewpot2D();
                        goldimage16_5 = result.steps[ExecutedSteps].goldimagepath;
                        DownloadImageFile(TwoDPanel16_5[0], goldimage16_5);
                        bflag16_5 = true;

                    }
                }
                //Step 16- six  -Compare the Screenshot of the 3D viewer with the saved image in the Active viewport
                bool bflag16_6 = false;
                IList<IWebElement> TwoDPanel16 = z3dvp.Viewpot2D();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                string tempimage16_6 = result.steps[ExecutedSteps].goldimagepath;
                DownloadImageFile(TwoDPanel16[0], tempimage16_6);
                if (CompareImage(goldimage16_5, tempimage16_6, 500))
                {
                    bflag16_6 = true;
                }
               
                if (bflag16_1==true && Beforescroll16[1] != Afterscroll16[1] && Afterscroll16[1] != AfterMagnify16[1] && AfterMagnify16[1] != AfterRotate16[1] && BeforeWindowlevel16[1] != AfterWindowlevel16[1]
                    && (sLastThumbName16_2 > 0) && bflag16_3==true  && (SSavedThum16 > sLastThumbName16_2) && bflag16_6 == true && bflag16_6==true)
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

                //Step 15 From six up viewing mode, Repeat steps 10-15 in MPR controls.
                //Step 1 
                bool bflag17_1 = false;
                bool thumbnailselction17 = z3dvp.selectthumbnail(thumbnailcaption);
                bool IMpr17 = z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                thumbnailselction17 = z3dvp.selectthumbnail(thumbnailcaption);
                if (!thumbnailselction17 && IMpr17 == false)
                {
                    throw new Exception();
                }
                else
                {
                    bflag17_1 = true;
                }
                //step 17 step 2 
                //Modify the image volume in MPR navigation control 1 by applying the following tools://1.Scroll,2.Magnify,3.Rotate,.Window level
                //Scroll
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                IWebElement Inavigation17_two = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((Inavigation17_two.Location.X + 100), (Inavigation17_two.Location.Y / 2 + 200));
                Thread.Sleep(500);
                //Scroll.
                List<string> Beforescroll17 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
             //   new Actions(Driver).SendKeys("x").Build().Perform();Thread.Sleep(1000);
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(500);
                for (int i = 0; i < 20; i++)
                {
                    BasePage.mouse_event(0x0800, 0, 0, -10, 0);
                    Thread.Sleep(1000);
                }
                Thread.Sleep(5000);
                List<string> Afterscroll17 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);

                //Magnify
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                Actions ActMag17 = new Actions(Driver);
                //  ActMag17.MoveToElement(Inavigation17_two).Click().DragAndDropToOffset(Inavigation17_two, Inavigation17_two.Size.Width + 5, Inavigation17_two.Size.Height + 10).Build().Perform();
                z3dvp.DragandDropelement(BluRingZ3DViewerPage.Navigationone, scrollvalue :false);
                Thread.Sleep(2000);
                ActMag17.Release().Build().Perform();
                List<string> AfterMagnify17 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);

                //Rotate
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                new Actions(Driver).DragAndDropToOffset(Inavigation17_two, Inavigation17_two.Location.X + 150, Inavigation17_two.Location.Y + 100).Build().Perform();
                Thread.Sleep(2000);
                List<string> AfterRotate17 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                //Window
                List<string> BeforeWindowlevel17 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                z3dvp.select3DTools(Z3DTools.Window_Level);
                z3dvp.Performdragdrop(Inavigation17_two, 10, 20,40,50);
                Thread.Sleep(2000);
                List<string> AfterWindowlevel17 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                if(AfterWindowlevel17[0].Trim()== sWindowlevel_value)
                {
                    z3dvp.Performdragdrop(Inavigation17_two, 10, 20, 60, 50);
                    Thread.Sleep(1000);
                    AfterWindowlevel17 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                }
                //step 17 -second  Click on the save button in the viewport top bar from the navigation control 1.
                int sLastThumbName17_2 = z3dvp.ThumbNailOperation(BluRingZ3DViewerPage.Navigationone);

                //Step 17 -third Click on the close button from the Global toolbar.
                bool bflag17_3 = false;
                IWebElement IExit17 = z3dvp.ExitIcon();
                if (IExit17.Displayed)
                {
                    IExit17.Click();
                    Thread.Sleep(1000);
                    bflag17_3 = true;
                }


                //step 17 -four Launch the same study again in the universal viewer and verify the saved images.
                PageLoadWait.WaitForFrameLoad(10);
                String FieldName17 = z3dvp.GetFieldName("patient");
                login.SearchStudy(FieldName17, Patientid);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy(FieldName17, Patientid);
                PageLoadWait.WaitForFrameLoad(5);
                var viewer17 = BluRingViewer.LaunchBluRingViewer(fieldname: FieldName17, value: thumbnailcaption);
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                int SSavedThum17 = z3dvp.VerfiySavedThumbNail(sLastThumbName17_2);
                bool bflag17_5 = false;
                //step 16 - five  Load the saved images in the active viewport .
                string goldimage17_5 = "";
                if (SSavedThum17 > sLastThumbName17_2)
                {
                    string sthumbu2dload = sthumbpattern.Remove(1, 2).Insert(1, SSavedThum17.ToString());

                    sthumbu2dload = sthumbu2dload.Remove(9, 5);
                    bool thumbnailselction = z3dvp.selectthumbnail(sthumbu2dload);
                    //once again trying some times 100% coming ,some times it is not coming 
                    if (thumbnailselction == false)
                    {
                        sthumbu2dload = sthumbpattern.Remove(1, 2).Insert(1, SSavedThum17.ToString());
                        thumbnailselction = z3dvp.selectthumbnail(sthumbu2dload);
                    }
                    if (!thumbnailselction)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        bflag17_5 = true;
                        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                        IList<IWebElement> TwoDPanel17_5 = z3dvp.Viewpot2D();
                        goldimage17_5 = result.steps[ExecutedSteps].goldimagepath;
                        DownloadImageFile(TwoDPanel17_5[0], goldimage17_5);

                    }
                }

                //Step 17- six  -Compare the Screenshot of the 3D viewer with the saved image in the Active viewport
                bool bflag17_6 = false;
                IList<IWebElement> TwoDPanel17 = z3dvp.Viewpot2D();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                string tempimage17_6 = result.steps[ExecutedSteps].goldimagepath;
                DownloadImageFile(TwoDPanel17[0], tempimage17_6);
                if (CompareImage(goldimage17_5, tempimage17_6, 500))
                {
                    bflag17_6 = true;
                }
               
                if (bflag17_1== true && Beforescroll17[0] != Afterscroll17[0] && Afterscroll17[0] != AfterMagnify17[0] && AfterMagnify17[0] != AfterRotate17[0] && BeforeWindowlevel17[0] != AfterWindowlevel17[0]
                    && (sLastThumbName17_2 > 0) && bflag17_3 == true && (SSavedThum17 > sLastThumbName17_2) && bflag17_6 == true )
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
                //step 16 Load the series in Curved MPR view of 3D viewer.
               
                bool thumbnailselction18 = z3dvp.selectthumbnail(thumbnailcaption);
                bool IMpr18 = z3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                thumbnailselction18 = z3dvp.selectthumbnail(thumbnailcaption);
                IWebElement INavigationone18 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                String SCursormode = INavigationone18.GetCssValue("cursor");
                if (thumbnailselction18 && IMpr18 == true && SCursormode.Contains(BluRingZ3DViewerPage.CurvedToolManualCursor))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    
                }
                else
                {
                    throw new Exception();

                }

                //Step 17 Create a path in navigation controls by adding the points.
                IWebElement Inavigationone19 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(50);
                Actions IAct19 = new Actions(Driver);
                IAct19.MoveToElement(Inavigationone19, Inavigationone19.Size.Width / 2 - 5, Inavigationone19.Location.Y - 10).Click().Build().Perform();
                Thread.Sleep(3000);
                IAct19.MoveToElement(Inavigationone19, Inavigationone19.Size.Width / 2 - 5, Inavigationone19.Location.Y - 50).Click().Build().Perform();
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Thread.Sleep(1000);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), pixelTolerance: 200))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 18  Modify the image volume in MPR path navigation control by applying the following tools: 1.Scroll 2.Magnify 3.Rotate 4.Window level
                //Modify the image volume in MPR navigation control 1 by applying the following tools://1.Scroll,2.Magnify,3.Rotate,.Window level
                //Scroll
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                IWebElement Inavigationone20 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((Inavigationone20.Location.X + 100), (Inavigationone20.Location.Y / 2 + 200));
                Thread.Sleep(500);
                //Scroll.
                z3dvp.SelectControl(BluRingZ3DViewerPage.Navigationone);
                List<string> Beforescroll2 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
              
                Thread.Sleep(500);
                for (int i = 0; i < 20; i++)
                {
                    BasePage.mouse_event(0x0800, 0, 0, -5, 0);
                    Thread.Sleep(1000);
                }
                Thread.Sleep(5000);
                List<string> Afterscroll20 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);

                //Magnify
                z3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                z3dvp.DragandDropelement(BluRingZ3DViewerPage.Navigationone, scrollvalue: false);
                Thread.Sleep(2000);
                List<string> AfterMagnify20 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);


                //Rotate
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                new Actions(Driver).DragAndDropToOffset(Inavigationone20, Inavigationone20.Location.X + 150, Inavigationone20.Location.Y + 100).Build().Perform();
                Thread.Sleep(2000);
                List<string> AfterRotate20 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);

                //Window
                List<string> BeforeWindowlevel20 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                z3dvp.select3DTools(Z3DTools.Window_Level);
                z3dvp.Performdragdrop(Inavigationone20, 10, 20);
                Thread.Sleep(2000);
                List<string> AfterWindowlevel20 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                if (AfterWindowlevel20[0].Trim() == sWindowlevel_value)
                {
                    z3dvp.Performdragdrop(Inavigationone20, 10, 20);
                    Thread.Sleep(2000);
                    AfterWindowlevel20 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                }
                if(Beforescroll2[0]!= Afterscroll20[0] && Afterscroll20[0]!= AfterMagnify20[0] && AfterMagnify20[0]!= AfterRotate20[0] && AfterRotate20[0] != AfterWindowlevel20[0])
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

                //Step 19     Click on the save button in the viewport top bar from the MPR path navigation control.
                int sLastThumbName21 = z3dvp.ThumbNailOperation(BluRingZ3DViewerPage.Navigationone);
                if(sLastThumbName21>0)
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

                ////Step 20 Select the 2D view option from the smart view drop down.
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Two_2D, "n");
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                int SSavedThum24 = z3dvp.VerfiySavedThumbNail(sLastThumbName21);
                if (SSavedThum24 > sLastThumbName21)
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
                //step 21 -   Load the saved images in the active viewport .
                string goldimage24 = "";
                string sthumbu2dload24 = sthumbpattern.Remove(1, 2).Insert(1, SSavedThum24.ToString());
                sthumbu2dload24 = sthumbu2dload24.Remove(9, 5);
                bool thumbnailselction24 = z3dvp.selectthumbnail(sthumbu2dload24);
                //once again trying some times 100% coming ,some times it is not coming 
                if (thumbnailselction24 == false)
                {
                sthumbu2dload24 = sthumbpattern.Remove(1, 2).Insert(1, SSavedThum24.ToString());
                    thumbnailselction24 = z3dvp.selectthumbnail(sthumbu2dload24);
                }
                if (!thumbnailselction24)
                {
                    throw new Exception();
                }
                else
                {
                    result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    IList<IWebElement> TwoDPanel24 = z3dvp.Viewpot2D();
                    goldimage24 = result.steps[ExecutedSteps].goldimagepath;
                    DownloadImageFile(TwoDPanel24[0], goldimage24);

                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                //step 22 Compare the Screenshot of the Z3D session with the saved image in the ICA thumbnail bar.
                IList<IWebElement> TwoDPanel25 = z3dvp.Viewpot2D();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                string tempimage25 = result.steps[ExecutedSteps].goldimagepath;
                DownloadImageFile(TwoDPanel25[0], tempimage25);
                if (CompareImage(goldimage24, tempimage25, 500))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
               
                //step 23 Select the 3D supported series and select curved MPR view from the smart 3D view.
                bool thumbnailselction26 = z3dvp.selectthumbnail(thumbnailcaption);
                bool IMpr26 = z3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                thumbnailselction26 = z3dvp.selectthumbnail(thumbnailcaption);
                IWebElement INavigationone26 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                string SCursormode26 = INavigationone26.GetCssValue("cursor");
                if (thumbnailselction26 && IMpr26 == true && SCursormode26.Contains(BluRingZ3DViewerPage.CurvedToolManualCursor))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {

                    throw new Exception();
                }

                //Step 24 Create a path in navigation controls by adding the points.
                IWebElement Inavigationone27 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
             //   new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(50);
                Actions IAct27 = new Actions(Driver);
                IAct27.MoveToElement(Inavigationone27, Inavigationone27.Size.Width / 2 - 5, Inavigationone27.Location.Y - 10).Click().Build().Perform();
                Thread.Sleep(3000);
                IAct27.MoveToElement(Inavigationone27, Inavigationone27.Size.Width / 2 - 5, Inavigationone27.Location.Y - 50).Click().Build().Perform();
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Thread.Sleep(1000);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone),pixelTolerance:200))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 25 Modify the image volume in Curved MPR control by applying the following tools:1.Rotate 2.Window level
                //Rotate
                List<string> BeforeRotate28 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                IWebElement ICurvedMprControl28 = z3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                if (Config.BrowserType.ToLower() == "firefox" || Config.BrowserType.ToLower() == "mozilla")
                {
                    new Actions(Driver).MoveToElement(ICurvedMprControl28, ICurvedMprControl28.Size.Width / 2, ICurvedMprControl28.Size.Height / 2).ClickAndHold()
                 .MoveToElement(ICurvedMprControl28, ICurvedMprControl28.Size.Width / 2, ICurvedMprControl28.Size.Height / 2 + 100)
                 .Release()
                 .Build()
                 .Perform();
                }
                else
                {
                    new Actions(Driver).DragAndDropToOffset(ICurvedMprControl28, ICurvedMprControl28.Location.X + 150, ICurvedMprControl28.Location.Y + 200).Build().Perform();
                }
                    Thread.Sleep(2000);
                List<string> AfterRotate28 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);

                //Window
                List<string> BeforeWindowlevel28 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                z3dvp.select3DTools(Z3DTools.Window_Level);
                Thread.Sleep(2000);
                z3dvp.Performdragdrop(ICurvedMprControl28, 10, 20,40,50);
                Thread.Sleep(2000);
                
                List<string> AfterWindowlevel28 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                if(AfterWindowlevel28[5].Trim()== sWindowlevel_value)
                {
                    z3dvp.Performdragdrop(ICurvedMprControl28, 10, 20, 40, 50);
                    Thread.Sleep(2000);
                    AfterWindowlevel28 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);

                }
                if(BeforeWindowlevel28[5]!= AfterWindowlevel28[5])
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

                //Step 26 Click on the save button in the viewport top bar from the Curved MPR control.
                int sLastThumbName29 = z3dvp.ThumbNailOperation(BluRingZ3DViewerPage.CurvedMPR);
                if (sLastThumbName29 > 0)
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
                //Step 30 Select the 3D supported series and select Calcium scoring view from the smart 3D view. 

                z3dvp.select3dlayout(BluRingZ3DViewerPage.Two_2D, "n");
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();

                int SSavedThum31 = z3dvp.VerfiySavedThumbNail(sLastThumbName29);
                if (SSavedThum31 > sLastThumbName29)
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

                //Step 28 Load the saved images in the active viewport .
                string goldimage32 = "";
                string sthumbu2dload32 = sthumbpattern.Remove(1, 2).Insert(1, SSavedThum31.ToString());
                sthumbu2dload32 = sthumbu2dload32.Remove(9, 5);
                bool thumbnailselction32 = z3dvp.selectthumbnail(sthumbu2dload32);
                //once again trying some times 100% coming ,some times it is not coming 
                if (thumbnailselction32 == false)
                {
                    sthumbu2dload32 = sthumbpattern.Remove(1, 2).Insert(1, SSavedThum31.ToString());
                    thumbnailselction32 = z3dvp.selectthumbnail(sthumbu2dload32);
                }
                if (!thumbnailselction32)
                {
                    throw new Exception();
                }
                else
                {
                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    IList<IWebElement> TwoDPanel32 = z3dvp.Viewpot2D();
                    goldimage32 = result.steps[ExecutedSteps].goldimagepath;
                    DownloadImageFile(TwoDPanel32[0], goldimage32);

                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }

                //step 29 Compare the Screenshot of the Z3D session with the saved image in the ICA thumbnail bar.
                IList<IWebElement> TwoDPanel33 = z3dvp.Viewpot2D();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                string tempimage33 = result.steps[ExecutedSteps].goldimagepath;
                DownloadImageFile(TwoDPanel33[0], tempimage33);
                if (CompareImage(goldimage32, tempimage33, 500))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
             
                //step 30  Select the 3D supported series and select Calcium scoring view from the smart 3D view.  
                bool thumbnailselction34 = z3dvp.selectthumbnail(thumbnailcaption);
                bool IMpr34 = z3dvp.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                z3dvp.checkerrormsg("y");
             
                thumbnailselction34 = z3dvp.selectthumbnail(thumbnailcaption);
                if (!thumbnailselction34 && IMpr34 == false)
                {
                    throw new Exception();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                }

                //Step 31 Mark the calcium regions (LM,RCA,LAD,CX).
                IWebElement iCalcium35 = z3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
              Actions  Markcalc = new Actions(Driver);
                Markcalc.MoveToElement(iCalcium35, iCalcium35.Size.Width / 2, iCalcium35.Size.Height / 2 + 10).ClickAndHold()
                    .MoveToElement(iCalcium35, iCalcium35.Size.Width / 2 - 100, iCalcium35.Size.Height / 2 + 200)
                    .MoveToElement(iCalcium35, iCalcium35.Size.Width / 2 + 100, iCalcium35.Size.Height / 2 + 200)
                    .MoveToElement(iCalcium35, iCalcium35.Size.Width / 2, iCalcium35.Size.Height / 2 + 200).Release().Build().Perform();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                IWebElement Iclose35 = z3dvp.CloseSelectedToolBox();
                if (Iclose35.Displayed == true)
                {
                    Iclose35.Click();
                    Thread.Sleep(1000);
                    
                }
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Thread.Sleep(1000);
                if (CompareImage(result.steps[ExecutedSteps], iCalcium35))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //step 32 Click on the save button in the viewport top bar from the Calcium scoring control.

                int sLastThumbName36 = z3dvp.ThumbNailOperation(BluRingZ3DViewerPage.CalciumScoring);
                if (sLastThumbName36 > 0)
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

                //Step 33 Select the 2D view option from the smart view drop down.

                z3dvp.select3dlayout(BluRingZ3DViewerPage.Two_2D, "n");
                PageLoadWait.WaitForFrameLoad(10);
                SwitchToDefault();
                SwitchToUserHomeFrame();
                int SSavedThum38 = z3dvp.VerfiySavedThumbNail(sLastThumbName36);
                if (SSavedThum38 > sLastThumbName36)
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

                //Step 34 Load the saved images in the active viewport .
                string goldimage39 = "";
                string sthumbu2dload39 = sthumbpattern.Remove(1, 2).Insert(1, SSavedThum38.ToString());
                sthumbu2dload39 = sthumbu2dload39.Remove(9, 5);
                bool thumbnailselction39 = z3dvp.selectthumbnail(sthumbu2dload39);
                //once again trying some times 100% coming ,some times it is not coming 
                if (thumbnailselction39 == false)
                {
                    sthumbu2dload39 = sthumbpattern.Remove(1, 2).Insert(1, SSavedThum38.ToString());
                    thumbnailselction39 = z3dvp.selectthumbnail(sthumbu2dload39);
                }
                if (!thumbnailselction39)
                {
                    throw new Exception();
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);

                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                    IList<IWebElement> TwoDPanel39 = z3dvp.Viewpot2D();
                    goldimage39 = result.steps[ExecutedSteps].goldimagepath;
                    DownloadImageFile(TwoDPanel39[0], goldimage39);

                   
                }
                //Step 35 Compare the Screenshot of the Z3D session with the saved image in the ICA thumbnail bar.
                IList<IWebElement> TwoDPanel40 = z3dvp.Viewpot2D();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                string tempimage40 = result.steps[ExecutedSteps].goldimagepath;
                DownloadImageFile(TwoDPanel40[0], tempimage40);
                if (CompareImage(goldimage39, tempimage40, 200))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
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

                return result;
            }
            finally
            {
                z3dvp.CloseViewer();
                login.Logout();

            }
        }

        public TestCaseResult Test_163537(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            //Set up Validation Steps
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 -  	iCA is logged in as Administrator.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;

                //step:2 and step:3  -Series is loaded in the 3D viewer in 3D 4:1 viewing mode
                Boolean step3 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, layout: BluRingZ3DViewerPage.Three_3d_4);
                if (step3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //step:4 - Modify the image volume in 3D control by applying the following tools:
                //1.Scroll 2.Magnify 3.Rotate 4.Window level preset 5.Sculp 6.Tissue selection
                Boolean step4_1 = Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D1, Z3DTools.Scrolling_Tool, 50, 50, 100);
                PageLoadWait.WaitForPageLoad(5);
                Z3dViewerPage.select3DTools(Z3DTools.Reset, ControlName: BluRingZ3DViewerPage.Navigation3D1);
                IWebElement Navigation3D = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                String goldimage1 = result.steps[ExecutedSteps].goldimagepath;
                String testimage1 = result.steps[ExecutedSteps].testimagepath;
                DownloadImageFile(Navigation3D, goldimage1);
                Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D1, Z3DTools.Interactive_Zoom, 50, 50, 100);
                DownloadImageFile(Navigation3D, testimage1);
                Boolean step4_2 = !CompareImage(goldimage1, testimage1);
                PageLoadWait.WaitForPageLoad(5);
                Z3dViewerPage.select3DTools(Z3DTools.Reset, ControlName: BluRingZ3DViewerPage.Navigation3D1);
                Boolean step4_3 = Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D1, Z3DTools.Rotate_Tool_1_Click_Center, 50, 50, 100);
                Z3dViewerPage.select3DTools(Z3DTools.Reset, ControlName: BluRingZ3DViewerPage.Navigation3D1);
                PageLoadWait.WaitForPageLoad(5);
                int step4_before = Z3dViewerPage.LevelOfSelectedColor(Navigation3D, testid, ExecutedSteps, 255, 255, 255, 2);
                Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D1, Z3DTools.Window_Level, 50, 50, 200, movement: "positive");
                int step4_after = Z3dViewerPage.LevelOfSelectedColor(Navigation3D, testid, ExecutedSteps, 255, 255, 255, 2);
                Boolean step4_4 = false;
                if(step4_before != step4_after)
                    step4_4 = true;
                Z3dViewerPage.select3DTools(Z3DTools.Reset, ControlName: BluRingZ3DViewerPage.Navigation3D1);
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 6);
                goldimage1 = result.steps[ExecutedSteps].goldimagepath;
                testimage1 = result.steps[ExecutedSteps].testimagepath;
                DownloadImageFile(Navigation3D, goldimage1);
                Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D1, Z3DTools.Sculpt_Tool_for_3D_1_Polygon, 50, 50, 100, testid, ExecutedSteps + 3);
                DownloadImageFile(Navigation3D, testimage1);
                Boolean step4_5 = !CompareImage(goldimage1, testimage1);
                PageLoadWait.WaitForPageLoad(5);
                Z3dViewerPage.select3DTools(Z3DTools.Reset, ControlName: BluRingZ3DViewerPage.Navigation3D1);
                PageLoadWait.WaitForPageLoad(5);
                Z3dViewerPage.select3DTools(Z3DTools.Reset, ControlName: BluRingZ3DViewerPage.Navigation3D1);
                Boolean step4_6 = Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D1, Z3DTools.Selection_Tool, 20, 20, 0, testid, ExecutedSteps + 2115, movement: "positive");
                PageLoadWait.WaitForPageLoad(5);
                IList<IWebElement> BeforeThumbCount = Z3dViewerPage.ThumbNailList();
                IWebElement Navigation3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                int step4_color = Z3dViewerPage.LevelOfSelectedColor(Navigation3D1, testid, ExecutedSteps, 0, 0, 255, 2);
                if (step4_1 && step4_2 && step4_3 && step4_4 && step4_5 && step4_6)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:5 - Click on the save button in the viewport top bar from the 3D1 control.
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigation3D1, "Save image and annotations to the exam");
                IWebElement step5 = Z3dViewerPage.BusyCursor();
                if(step5.Enabled)
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

                //step:7 - Study is loaded in the universal viewer without any errors and the saved images are listed in the thumbnail section.
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D);
                PageLoadWait.WaitForFrameLoad(10);
                IList<IWebElement> AfterThumbCount = Z3dViewerPage.ThumbNailList();
                if(BeforeThumbCount.Count < AfterThumbCount.Count)
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

                //step:8 - Saved images are loaded in the viewport without any errors.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                Boolean step8 = Z3dViewerPage.selectthumbnail("Saved 3D Image");
                if(step8)
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

                //step:9 - Compare the Screenshot of the 3D 4:1 view with the saved image in the ICA thumbnail bar.
                IList<IWebElement> Viewport2D = Z3dViewerPage.Viewpot2D();
                int step9_color = Z3dViewerPage.LevelOfSelectedColor(Viewport2D[0], testid, ExecutedSteps, 0, 0, 255, 2);
                int step9 = step4_color - step9_color;
                if(step9 < 250 && step9 > -250)
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

                //step:10 - Again launch the Z3D session , Navigate to Six up viewing modes and repeat steps 4-9 in 3D 1 and 3D 2 controls.
                Boolean step10 = false;
                Z3dViewerPage.selectthumbnail(ThumbnailDescription);
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6, "y");
                Boolean step10_1 = Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D2, Z3DTools.Scrolling_Tool, 50, 50, 100);
                Z3dViewerPage.select3DTools(Z3DTools.Reset, ControlName: BluRingZ3DViewerPage.Navigation3D2);
                Navigation3D = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                goldimage1 = result.steps[ExecutedSteps].goldimagepath;
                testimage1 = result.steps[ExecutedSteps].testimagepath;
                DownloadImageFile(Navigation3D, goldimage1);
                Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D1, Z3DTools.Interactive_Zoom, 50, 50, 100);
                DownloadImageFile(Navigation3D, testimage1);
                Boolean step10_2 = !CompareImage(goldimage1, testimage1);
                Z3dViewerPage.select3DTools(Z3DTools.Reset, ControlName: BluRingZ3DViewerPage.Navigation3D2);
                Boolean step10_3 = Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D2, Z3DTools.Rotate_Tool_1_Click_Center, 50, 50, 100);
                Z3dViewerPage.select3DTools(Z3DTools.Reset, ControlName: BluRingZ3DViewerPage.Navigation3D2);
                int step10_before = Z3dViewerPage.LevelOfSelectedColor(Navigation3D, testid, ExecutedSteps, 255, 255, 255, 2);
                Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D2, Z3DTools.Window_Level, 50, 50, 200, movement: "positive");
                PageLoadWait.WaitForPageLoad(5);
                int step10_after = Z3dViewerPage.LevelOfSelectedColor(Navigation3D, testid, ExecutedSteps, 255, 255, 255, 2);
                Boolean step10_4 = false;
                if (step10_before != step10_after)
                    step10_4 = true;
                Z3dViewerPage.select3DTools(Z3DTools.Reset, ControlName: BluRingZ3DViewerPage.Navigation3D2);
                Z3dViewerPage.select3DTools(Z3DTools.Reset, ControlName: BluRingZ3DViewerPage.Navigation3D2);
                Boolean step10_6 = Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D2, Z3DTools.Selection_Tool, 20, 20, 0, testid, ExecutedSteps + 2115, movement: "positive");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 6);
                goldimage1 = result.steps[ExecutedSteps].goldimagepath;
                testimage1 = result.steps[ExecutedSteps].testimagepath;
                DownloadImageFile(Navigation3D, goldimage1);
                Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D2, Z3DTools.Sculpt_Tool_for_3D_1_Polygon, 50, 50, 100, testid, ExecutedSteps + 3);
                DownloadImageFile(Navigation3D, testimage1);
                Boolean step10_5 = !CompareImage(goldimage1, testimage1);
                BeforeThumbCount = Z3dViewerPage.ThumbNailList();
                IWebElement Navigation3D2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                int step10_color1 = Z3dViewerPage.LevelOfSelectedColor(Navigation3D2, testid, ExecutedSteps, 0, 0, 255, 2);
                if (step10_1 && step10_2 && step10_3 && step10_4 && step10_5 && step10_6)
                    step10 = true;
                else
                    step10 = false;

                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigation3D1, "Save image and annotations to the exam");
                IWebElement step10_7 = Z3dViewerPage.BusyCursor();
                if (step10_7.Enabled)
                    step10 = true;
                else
                    step10 = false;

                //Z3dViewerPage.CloseViewer();
                //PageLoadWait.WaitForFrameLoad(5);
                //Driver.SwitchTo().DefaultContent(); Driver.SwitchTo().Frame("UserHomeFrame");
                //if (Driver.FindElement(By.CssSelector("#TabText0")).Displayed)
                //    step10 = true;
                //else
                //    step10 = false;
                //login.SelectStudy("Patient ID", PatientID);
                //PageLoadWait.WaitForFrameLoad(5);
                //var viewer = BluRingViewer.LaunchBluRingViewer();
                //PageLoadWait.WaitForFrameLoad(10);
                //SwitchToDefault();
                //SwitchToUserHomeFrame();
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D);
                PageLoadWait.WaitForFrameLoad(10);
                AfterThumbCount = Z3dViewerPage.ThumbNailList();
                if (BeforeThumbCount.Count < AfterThumbCount.Count)
                    step10 = true;
                else
                    step10 = false;

                SwitchToDefault();
                SwitchToUserHomeFrame();
                Boolean step10_8 = Z3dViewerPage.selectthumbnail("Saved 3D Image", AfterThumbCount.Count - 1);
                if (step10_8)
                    step10 = true;
                else
                    step10 = false;

                Viewport2D = Z3dViewerPage.Viewpot2D();
                int step10_color2 = Z3dViewerPage.LevelOfSelectedColor(Viewport2D[0], testid, ExecutedSteps, 0, 0, 255, 2);
                int step10_9 = step10_color1 - step10_color2;
                if (step9 < 250 && step9 > -250)
                    step10 = true;
                else
                    step10 = false;
                if (step10)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:11 -  	Select the Curved MPR viewing mode from the smart view drop down
                Z3dViewerPage.CloseViewer();
                PageLoadWait.WaitForPageLoad(10);
                Boolean step11 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, layout: BluRingZ3DViewerPage.CurvedMPR);
                if (step11)
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

                //step:12 - Create a path in navigation controls by adding the points.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                String goldimage = result.steps[ExecutedSteps].goldimagepath;
                String testimage = result.steps[ExecutedSteps].testimagepath;
                IWebElement ViewerContainer = Z3dViewerPage.ViewerContainer();
                DownloadImageFile(ViewerContainer, goldimage);
                int BluColorBeforePoint = Z3dViewerPage.selectedcolorcheck(goldimage, 0, 0, 255, 1);
                IWebElement navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Actions act = new Actions(Driver);
                Z3dViewerPage.MoveAndClick(navigation1, navigation1.Size.Width / 2 + 40, navigation1.Size.Height / 4);
                PageLoadWait.WaitForPageLoad(10);
                Z3dViewerPage.MoveAndClick(navigation1, navigation1.Size.Width / 2 + 80, navigation1.Size.Height / 4);
                PageLoadWait.WaitForPageLoad(10);
                Z3dViewerPage.MoveAndClick(navigation1, navigation1.Size.Width / 2 + 120, navigation1.Size.Height / 4);
                PageLoadWait.WaitForPageLoad(10);
                DownloadImageFile(ViewerContainer, testimage);
                int BluColorAfterPoint1 = Z3dViewerPage.selectedcolorcheck(testimage, 0, 0, 255, 1);
                if (BluColorAfterPoint1 > BluColorBeforePoint)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:13 - Modify the image volume in 3D path navigation control by applying the following tools:
                //1.Scroll 2.Magnify 3.Rotate 4.Window level
                Boolean step13_1 = Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage._3DPathNavigation, Z3DTools.Scrolling_Tool, 150, 150, 100);
                Navigation3D = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                goldimage1 = result.steps[ExecutedSteps].goldimagepath;
                testimage1 = result.steps[ExecutedSteps].testimagepath;
                DownloadImageFile(Navigation3D, goldimage1);
                Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage._3DPathNavigation, Z3DTools.Interactive_Zoom, 50, 50, 100);
                DownloadImageFile(Navigation3D, testimage1);
                Boolean step13_2 = !CompareImage(goldimage1, testimage1);
                IWebElement NavigationElement = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                String step13_before = NavigationElement.FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).GetAttribute("innerHTML");
                Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage._3DPathNavigation, Z3DTools.Rotate_Tool_1_Click_Center, 150, 150, -500);
                String step13_after = NavigationElement.FindElement(By.CssSelector(Locators.CssSelector.AnnotationCentreTop)).GetAttribute("innerHTML");
                Boolean step13_3 = false;
                if (step13_before != step13_after)
                    step13_3 = true;
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 6);
                goldimage = result.steps[ExecutedSteps].goldimagepath;
                testimage = result.steps[ExecutedSteps].testimagepath;
                IWebElement Navigation3DPath = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                DownloadImageFile(Navigation3DPath, testimage);
                Z3dViewerPage.ApplyToolsonViewPort(BluRingZ3DViewerPage._3DPathNavigation, Z3DTools.Window_Level, 150, 150, 200, movement: "positive");
                DownloadImageFile(Navigation3DPath, goldimage);
                Boolean step13_4 = !CompareImage(goldimage, testimage);
                PageLoadWait.WaitForPageLoad(10);
                IWebElement CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                int step13_colors = Z3dViewerPage.LevelOfSelectedColor(CurvedMPR, testid, ExecutedSteps, 255, 255, 255, 2);
                if (step13_1 && step13_2 && step13_3 && step13_4)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:14 - Click on the save button in the viewport top bar from the 3D1 control.
                BeforeThumbCount = Z3dViewerPage.ThumbNailList();
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.CurvedMPR, "Save image and annotations to the exam");
                IWebElement step14 = Z3dViewerPage.BusyCursor();
                if (step14.Enabled)
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

                //step:16 - Study is loaded in the universal viewer without any errors and the saved images are listed in the thumbnail section.
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D);
                PageLoadWait.WaitForFrameLoad(10);
                AfterThumbCount = Z3dViewerPage.ThumbNailList();
                if (BeforeThumbCount.Count < AfterThumbCount.Count)
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

                //step:17 - Saved images are loaded in the viewport without any errors.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                Boolean step17 = Z3dViewerPage.selectthumbnail("Saved 3D Image", AfterThumbCount.Count - 1);
                if (step17)
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

                //step:18 - Compare the Screenshot of the 3D 4:1 view with the saved image in the ICA thumbnail bar.
                Viewport2D = Z3dViewerPage.Viewpot2D();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport2D[0]))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                Z3dViewerPage.CloseViewer();
                login.Logout();
                try
                {
                    Z3dViewerPage.DeletePriorsInEA("10.9.37.82", PatientID, TestDataRequirements);
                }
                catch(Exception ex)
                {
                    Logger.Instance.ErrorLog("Error in deleting priors "+ ex.ToString());
                }
            }
        }

        public TestCaseResult Test_163538(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            //Set up Validation Steps
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 -  	iCA is logged in as Administrator.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;

                //step:2 and step:3  -Series is loaded in the 3D viewer in 3D 4:1 viewing mode
                Boolean step3 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, layout: BluRingZ3DViewerPage.MPR);
                if (step3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:4 - Select the Measurement tool from the 3D tool box.
                Boolean step4 = Z3dViewerPage.select3DTools(Z3DTools.Line_Measurement);
                if (step4)
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

                //step:5 -  	Create a measurement on navigation control 1
                IWebElement Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                int step5_Before = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps, 255, 255, 0, 2);
                if (browserName.ToLower().Contains("internet") || browserName.ToLower().Contains("ie"))
                    new TestCompleteAction().PerformDraganddrop(Navigation1, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4, Navigation1.Size.Width / 3, Navigation1.Size.Height / 3);
                else
                    Z3dViewerPage.Performdragdrop(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 2, Navigation1.Size.Width / 3, Navigation1.Size.Height / 3);
                int step5_After = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps, 255, 255, 0, 2);
                if(step5_Before < step5_After)
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

                //step:6 - Click on the save button in the viewport top bar from the navigation control 1.
                IList<IWebElement> BeforeThumbCount = Z3dViewerPage.ThumbNailList();
                int step6_color = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps, 255, 255, 255, 2);
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationone, "Save image and annotations to the exam");
                IWebElement step6 = Z3dViewerPage.BusyCursor();
                if (step6.Enabled)
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


                //step:8 - Study is loaded in the universal viewer without any errors and the saved images are listed in the thumbnail section.
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D);
                PageLoadWait.WaitForFrameLoad(10);
                IList<IWebElement> AfterThumbCount = Z3dViewerPage.ThumbNailList();
                if (BeforeThumbCount.Count < AfterThumbCount.Count)
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

                //step:9 - Saved images are loaded in the viewport without any errors.
                SwitchToDefault();
                SwitchToUserHomeFrame();
                Boolean step8 = Z3dViewerPage.selectthumbnail("Saved 3D Image");
                if (step8)
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

                //step:10 - Compare the Screenshot of the 3D 4:1 view with the saved image in the ICA thumbnail bar.
                IList<IWebElement> Viewport2D = Z3dViewerPage.Viewpot2D();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport2D[0]))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:11 - Repeat steps 4-10 in all the MPR navigation controls and MPR result control.
                //1.Six up view 2.Curved MPR view
                Z3dViewerPage.selectthumbnail(ThumbnailDescription);
                PageLoadWait.WaitForPageLoad(10);
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6, "y");
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);

                Boolean step11_1 = Z3dViewerPage.select3DTools(Z3DTools.Line_Measurement);

                Boolean step11_19 = false;
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                int step11_19_Before = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps, 255, 255, 0, 2);
                if (browserName.ToLower().Contains("internet") || browserName.ToLower().Contains("ie"))
                    new TestCompleteAction().PerformDraganddrop(Navigation1, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4, Navigation1.Size.Width / 3, Navigation1.Size.Height / 3);
                else
                    Z3dViewerPage.Performdragdrop(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 2, Navigation1.Size.Width / 3, Navigation1.Size.Height / 3);
                int step11_19_After = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps, 255, 255, 0, 2);
                if (step11_19_Before < step11_19_After)
                    step11_19 = true;
                else
                    step11_19 = false;

                Boolean step11_2 = false;
                BeforeThumbCount = Z3dViewerPage.ThumbNailList();
                int step11_2_color = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps, 255, 255, 255, 2);
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationone, "Save image and annotations to the exam");
                IWebElement step11_2_cursor = Z3dViewerPage.BusyCursor();
                if (step11_2_cursor.Enabled)
                    step11_2 = true;
                else
                    step11_2 = false;


                Boolean step11_4 = false;
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D);
                PageLoadWait.WaitForFrameLoad(10);
                AfterThumbCount = Z3dViewerPage.ThumbNailList();
                if (BeforeThumbCount.Count < AfterThumbCount.Count)
                    step11_4 = true;
                else
                    step11_4 = false;

                SwitchToDefault();
                SwitchToUserHomeFrame();
                Boolean step11_5 = Z3dViewerPage.selectthumbnail("Saved 3D Image");

                Boolean step11_6 = false;
                Viewport2D = Z3dViewerPage.Viewpot2D();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Viewport2D[0]))
                    step11_6 = true;
                else
                    step11_6 = false;

                Z3dViewerPage.selectthumbnail(ThumbnailDescription);
                PageLoadWait.WaitForPageLoad(10);
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6, "y");
                IWebElement ResultControl = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);

                Boolean step11_7 = Z3dViewerPage.select3DTools(Z3DTools.Line_Measurement);

                Boolean step11_20 = false;
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                int step11_20_Before = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps, 255, 255, 0, 2);
                if (browserName.ToLower().Contains("internet") || browserName.ToLower().Contains("ie"))
                    new TestCompleteAction().PerformDraganddrop(Navigation1, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4, Navigation1.Size.Width / 3, Navigation1.Size.Height / 3);
                else
                    Z3dViewerPage.Performdragdrop(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 2, Navigation1.Size.Width / 3, Navigation1.Size.Height / 3);
                int step11_20_After = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps, 255, 255, 0, 2);
                if (step11_20_Before < step11_20_After)
                    step11_20 = true;
                else
                    step11_20 = false;

                Boolean step11_8 = false;
                BeforeThumbCount = Z3dViewerPage.ThumbNailList();
                int step11_8_color = Z3dViewerPage.LevelOfSelectedColor(ResultControl, testid, ExecutedSteps, 255, 255, 255, 2);
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.ResultPanel, "Save image and annotations to the exam");
                IWebElement step11_8_cursor = Z3dViewerPage.BusyCursor();
                if (step11_8_cursor.Enabled)
                    step11_8 = true;
                else
                    step11_8 = false;


                Boolean step11_10 = false;
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D);
                PageLoadWait.WaitForFrameLoad(10);
                AfterThumbCount = Z3dViewerPage.ThumbNailList();
                if (BeforeThumbCount.Count < AfterThumbCount.Count)
                    step11_10 = true;
                else
                    step11_10 = false;

                SwitchToDefault();
                SwitchToUserHomeFrame();
                Boolean step11_11 = Z3dViewerPage.selectthumbnail("Saved 3D Image");

                Boolean step11_12 = false;
                Viewport2D = Z3dViewerPage.Viewpot2D();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2);
                if (CompareImage(result.steps[ExecutedSteps], Viewport2D[0]))
                    step11_12 = true;
                else
                    step11_12 = false;

                Z3dViewerPage.selectthumbnail(ThumbnailDescription);
                PageLoadWait.WaitForPageLoad(10);
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.CurvedMPR, "y");
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);

                Boolean step11_13 = Z3dViewerPage.select3DTools(Z3DTools.Line_Measurement);

                Boolean step11_21 = false;
                Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                int step11_21_Before = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps, 255, 255, 0, 2);
                if (browserName.ToLower().Contains("internet") || browserName.ToLower().Contains("ie"))
                    new TestCompleteAction().PerformDraganddrop(Navigation1, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4, Navigation1.Size.Width / 3, Navigation1.Size.Height / 3);
                else
                    Z3dViewerPage.Performdragdrop(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 2, Navigation1.Size.Width / 3, Navigation1.Size.Height / 3);
                int step11_21_After = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps, 255, 255, 0, 2);
                if (step11_21_Before < step11_21_After)
                    step11_21 = true;
                else
                    step11_21 = false;

                Boolean step11_14 = false;
                BeforeThumbCount = Z3dViewerPage.ThumbNailList();
                int step11_14_color = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps, 255, 255, 255, 2);
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationone, "Save image and annotations to the exam");
                IWebElement step11_14_cursor = Z3dViewerPage.BusyCursor();
                if (step11_14_cursor.Enabled)
                    step11_14 = true;
                else
                    step11_14 = false;


                Boolean step11_16 = false;
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D);
                PageLoadWait.WaitForFrameLoad(10);
                AfterThumbCount = Z3dViewerPage.ThumbNailList();
                if (BeforeThumbCount.Count < AfterThumbCount.Count)
                    step11_16 = true;
                else
                    step11_16 = false;

                SwitchToDefault();
                SwitchToUserHomeFrame();
                Boolean step11_17 = Z3dViewerPage.selectthumbnail("Saved Presentation State");

                Boolean step11_18 = false;
                Viewport2D = Z3dViewerPage.Viewpot2D();
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3);
                if (CompareImage(result.steps[ExecutedSteps], Viewport2D[0]))
                    step11_18 = true;
                else
                    step11_18 = false;

                if (step11_1 && step11_2 && step11_4 && step11_5 && step11_6 && step11_7 && step11_8 && step11_10 && step11_12 && step11_13 && step11_14 && step11_16 && step11_17 && step11_18 && step11_19 && step11_20 && step11_21)
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

                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                return result;
            }

            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                Z3dViewerPage.CloseViewer();
                login.Logout();              
                try
                {
                    Z3dViewerPage.DeletePriorsInEA("10.9.37.82", PatientID, TestDataRequirements);
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Error in deleting priors " + ex.ToString());
                }
            }
        }

        public TestCaseResult Test_163539(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            //Set up Validation Steps
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1 -  	iCA is logged in as Administrator.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                ExecutedSteps++;

                //step:2 and step:3  -Series is loaded in the 3D viewer in 3D 4:1 viewing mode
                Boolean step3 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, layout: BluRingZ3DViewerPage.Three_3d_4);
                if (step3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //step:4 - Select the Measurement tool from the 3D tool box.
                Boolean step4 = Z3dViewerPage.select3DTools(Z3DTools.Line_Measurement);
                if (step4)
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

                //step:5 -  	Create a measurement on navigation control 1
                IWebElement Navigation3D1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                int step5_Before = Z3dViewerPage.LevelOfSelectedColor(Navigation3D1, testid, ExecutedSteps, 255, 255, 0, 2);
                if (browserName.ToLower().Contains("internet") || browserName.ToLower().Contains("ie"))
                    new TestCompleteAction().PerformDraganddrop(Navigation3D1, (Navigation3D1.Size.Width / 4) * 3, (Navigation3D1.Size.Height / 4), (Navigation3D1.Size.Width / 4), (Navigation3D1.Size.Height / 4));
                else
                Z3dViewerPage.Performdragdrop(Navigation3D1, (Navigation3D1.Size.Width / 4) * 3, (Navigation3D1.Size.Height / 4), (Navigation3D1.Size.Width / 4), (Navigation3D1.Size.Height / 4));
                int step5_After = Z3dViewerPage.LevelOfSelectedColor(Navigation3D1, testid, ExecutedSteps, 255, 255, 0, 2);
                if (step5_Before < step5_After)
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

                //step:6 - Click on the save button in the viewport top bar from the navigation control 1.
                IList<IWebElement> BeforeThumbCount = Z3dViewerPage.ThumbNailList();
                int step6_color = Z3dViewerPage.LevelOfSelectedColor(Navigation3D1, testid, ExecutedSteps, 255, 255, 0, 2);
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigation3D1, "Save image and annotations to the exam");
                IWebElement step6 = Z3dViewerPage.BusyCursor();
                if (step6.Enabled)
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
                PageLoadWait.WaitForFrameLoad(10);

                //step:8 - Study is loaded in the universal viewer without any errors and the saved images are listed in the thumbnail section.
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D);
                PageLoadWait.WaitForFrameLoad(10);
                IList<IWebElement> AfterThumbCount = Z3dViewerPage.ThumbNailList();
                SwitchToDefault();
                SwitchToUserHomeFrame();
                Boolean step8 = Z3dViewerPage.selectthumbnail("Saved 3D Image");
                if (BeforeThumbCount.Count < AfterThumbCount.Count && step8)
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

                //step:9 - Compare the Screenshot of the 3D 4:1 view with the saved image in the ICA thumbnail bar.
                IList<IWebElement> Viewport2D = Z3dViewerPage.Viewpot2D();
                int step9_color = Z3dViewerPage.LevelOfSelectedColor(Viewport2D[0], testid, ExecutedSteps, 0, 255, 0, 2);
                //if (step9_color != step6_color)
                if(step9_color == 0)
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

                //login.SelectStudy("Patient ID", PatientID);
                //PageLoadWait.WaitForFrameLoad(5);
                //var viewer = BluRingViewer.LaunchBluRingViewer();
                //PageLoadWait.WaitForFrameLoad(10);
                //SwitchToDefault();
                //SwitchToUserHomeFrame();

                //step:10 - Relaunch Z3D and Navigate to Six up viewing mode.
                Z3dViewerPage.selectthumbnail(ThumbnailDescription);
                PageLoadWait.WaitForPageLoad(10);
                Boolean step10 =  Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Three_3d_6, "y");
                if (step10)
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

                //step:11 - Repeat steps 4-10 in all the MPR navigation controls and MPR result control.
                //1.Six up view
                IWebElement Navigation3D2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D2);

                Boolean step11_1 = Z3dViewerPage.select3DTools(Z3DTools.Line_Measurement);

                Boolean step11_7 = false;
                Navigation3D2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                int step11_7__Before = Z3dViewerPage.LevelOfSelectedColor(Navigation3D2, testid, ExecutedSteps, 255, 255, 0, 2);
                if (browserName.ToLower().Contains("internet") || browserName.ToLower().Contains("ie"))
                    new TestCompleteAction().PerformDraganddrop(Navigation3D2, (Navigation3D2.Size.Width / 5) * 4, (Navigation3D2.Size.Height / 5) * 3, (Navigation3D2.Size.Width / 3) * 2, (Navigation3D2.Size.Height / 5) * 3);
                else
                    Z3dViewerPage.Performdragdrop(Navigation3D2, (Navigation3D2.Size.Width / 5) * 4, (Navigation3D2.Size.Height / 5) * 3, (Navigation3D2.Size.Width / 3) * 2, (Navigation3D2.Size.Height / 5) * 3);
                int step11_7__After = Z3dViewerPage.LevelOfSelectedColor(Navigation3D2, testid, ExecutedSteps, 255, 255, 0, 2);
                if (step11_7__Before < step11_7__After)
                    step11_7 = true;
                else
                    step11_7 = false;

                Boolean step11_2 = false;
                BeforeThumbCount = Z3dViewerPage.ThumbNailList();
                int step11_2_color = Z3dViewerPage.LevelOfSelectedColor(Navigation3D2, testid, ExecutedSteps, 255, 255, 255, 2);
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigation3D2, "Save image and annotations to the exam");
                IWebElement step11_2_cursor = Z3dViewerPage.BusyCursor();
                if (step11_2_cursor.Enabled)
                    step11_2 = true;
                else
                    step11_2 = false;

                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.select3dlayout(BluRingZ3DViewerPage.Two_2D);
                PageLoadWait.WaitForFrameLoad(10);

                Boolean step11_4 = false;
                AfterThumbCount = Z3dViewerPage.ThumbNailList();
                if (BeforeThumbCount.Count < AfterThumbCount.Count)
                    step11_4 = true;
                else
                    step11_4 = false;

                SwitchToDefault();
                SwitchToUserHomeFrame();
                Boolean step11_5 = Z3dViewerPage.selectthumbnail("Saved 3D Image");

                Boolean step11_6 = false;
                Viewport2D = Z3dViewerPage.Viewpot2D();
                int step11_6_color = Z3dViewerPage.LevelOfSelectedColor(Viewport2D[0], testid, ExecutedSteps, 0, 255, 0, 2);
                if (step11_6_color == 0)
                    step11_6 = true;
                else
                    step11_6 = false;

                if (step11_1 && step11_2 && step11_4 && step11_5 && step11_6 && step11_7)
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

                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                Z3dViewerPage.CloseViewer();
                login.Logout();              
                try
                {
                    Z3dViewerPage.DeletePriorsInEA("10.9.37.82", PatientID, TestDataRequirements);
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Error in deleting priors " + ex.ToString());
                }
            }
        }

        public TestCaseResult Test_163540(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            ServiceTool servicetool = new ServiceTool();
            WpfObjects wpfobject = new WpfObjects();
            WorkFlow Workflow = new WorkFlow();
            DomainManagement domainmanagement = new DomainManagement();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String PatientID1 = PatientID.Split('|')[0];
            String PatientID2 = PatientID.Split('|')[1];
            String PatientID3 = PatientID.Split('|')[2];
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            String Desc1 = ThumbnailDescription.Split('|')[0];
            String Desc2 = ThumbnailDescription.Split('|')[1];
            String Desc3 = ThumbnailDescription.Split('|')[2];
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String accession1 = TestDataRequirements.Split('|')[0];
            String accession2 = TestDataRequirements.Split('|')[1];
            String accession3 = TestDataRequirements.Split('|')[2];

            //Set up Validation Steps
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                Z3dViewerPage.DeletePriorsInEA(Config.DestEAsIp, PatientID1, accession1);
                Z3dViewerPage.DeletePriorsInEA(Config.DestEAsIp, PatientID2, accession2);
                Z3dViewerPage.DeletePriorsInEA(Config.DestEAsIp, PatientID3, accession3);

                //Step1 :: Launch ICA Service tool, Navigate to Enable Features tab->General tab -> ensure that the 'Enable Saving GSPS' option is enabled by default.
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures(); // Navigate to Enable features tab in Service tool
                wpfobject.WaitTillLoad();
                servicetool.ClickModifyButton();
                servicetool.EnableSavingGSPS();
                servicetool.ClickApplyButtonFromTab();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                result.steps[++ExecutedSteps].status = "Pass";

                //Step2 ::Login to ICA as Administrator and navigate to Domain management page, disable the 'Enable Saving GSPS' option. 
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.Navigate("DomainManagement");
                PageLoadWait.WaitForFrameLoad(10);
                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("savegsps", 1);
                bool Checkbox = domainmanagement.VerifyCheckBoxInEditDomain("savegsps");
                domainmanagement.SaveButton().Click();
                if (!Checkbox)
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
                PageLoadWait.WaitForFrameLoad(10);
                login.Logout();
                //Step3 :: From the ICA Service tool, Enable Features tab->General tab -> disable the 'Enable Saving GSPS' and Restart IIS
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures(); // Navigate to Enable features tab in Service tool
                wpfobject.WaitTillLoad();
                servicetool.ClickModifyButton();
                servicetool.DisableSavingGSPS();
                servicetool.ClickApplyButtonFromTab();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                result.steps[++ExecutedSteps].status = "Pass";

                //Step4 :: Login to ICA as Administrator and Load a CT study having a series more than 15 images in universal viewer.
                //Step5 :: Select the MPR view option from the smart view drop down.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step4and5 = Z3dViewerPage.searchandopenstudyin3D(PatientID1, Desc1, layout: BluRingZ3DViewerPage.MPR);
                if (step4and5)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step6 :: Verify that the save button in the viewport top bar from all the available controls.
                bool Navogation1 = Z3dViewerPage.VerifyOptionsfromViewport(BluRingZ3DViewerPage.Navigationone, "Save image and annotations to the exam");
                bool Navigation2 = Z3dViewerPage.VerifyOptionsfromViewport(BluRingZ3DViewerPage.Navigationtwo, "Save image and annotations to the exam");
                bool Navigation3 = Z3dViewerPage.VerifyOptionsfromViewport(BluRingZ3DViewerPage.Navigationthree, "Save image and annotations to the exam");
                bool Result = Z3dViewerPage.VerifyOptionsfromViewport(BluRingZ3DViewerPage.ResultPanel, "Save image and annotations to the exam");
                if (!Navogation1 && !Navigation2 && !Navigation3 && !Result)
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
                Z3dViewerPage.CloseViewer();
                login.Logout();
                //Step7 :: launch ICA Service tool, Navigate to Enable Features tab->General tab -> enable the 'Enable Saving GSPS' and Restart IIS.
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures(); // Navigate to Enable features tab in Service tool
                wpfobject.WaitTillLoad();
                servicetool.ClickModifyButton();
                servicetool.EnableSavingGSPS();
                servicetool.ClickApplyButtonFromTab();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                result.steps[++ExecutedSteps].status = "Pass";
                //Step8 :: Login to ICA as Administrator and Load a CT study having a series more than 15 images in universal viewer.
                //Step9 :: Select the MPR view option from the smart view drop down.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step8and9 = Z3dViewerPage.searchandopenstudyin3D(PatientID1, Desc1, layout: BluRingZ3DViewerPage.MPR);
                if (step8and9)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step10 :: Verify that the save button in the viewport top bar from all the available controls.
                Navogation1 = Z3dViewerPage.VerifyOptionsfromViewport(BluRingZ3DViewerPage.Navigationone, "Save image and annotations to the exam");
                Navigation2 = Z3dViewerPage.VerifyOptionsfromViewport(BluRingZ3DViewerPage.Navigationtwo, "Save image and annotations to the exam");
                Navigation3 = Z3dViewerPage.VerifyOptionsfromViewport(BluRingZ3DViewerPage.Navigationthree, "Save image and annotations to the exam");
                Result = Z3dViewerPage.VerifyOptionsfromViewport(BluRingZ3DViewerPage.ResultPanel, "Save image and annotations to the exam");
                if (!Navogation1 && !Navigation2 && !Navigation3 && !Result)
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
                Z3dViewerPage.CloseViewer();
                //Step11 :: Navigate to domain management page, enabled the 'Enable Saving GSPS' option.
                login.Navigate("DomainManagement");
                PageLoadWait.WaitForFrameLoad(10);
                domainmanagement.SearchDomain("SuperAdminGroup");
                domainmanagement.SelectDomain("SuperAdminGroup");
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("savegsps", 0);
                Checkbox = domainmanagement.VerifyCheckBoxInEditDomain("savegsps");
                domainmanagement.SaveButton().Click();
                if (Checkbox)
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
                //Step12 :: Login to ICA as Administrator and Load a CT study having a series more than 15 images in universal viewer.
                //Step13 :: Select the MPR view option from the smart view drop down.
                Boolean step12and13 = Z3dViewerPage.searchandopenstudyin3D(PatientID1, Desc1, layout: BluRingZ3DViewerPage.MPR);
                if (step12and13)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step14 :: Verify that the save button in the viewport top bar from all the available controls.
                Navogation1 = Z3dViewerPage.VerifyOptionsfromViewport(BluRingZ3DViewerPage.Navigationone, "Save image and annotations to the exam");
                Navigation2 = Z3dViewerPage.VerifyOptionsfromViewport(BluRingZ3DViewerPage.Navigationtwo, "Save image and annotations to the exam");
                Navigation3 = Z3dViewerPage.VerifyOptionsfromViewport(BluRingZ3DViewerPage.Navigationthree, "Save image and annotations to the exam");
                Result = Z3dViewerPage.VerifyOptionsfromViewport(BluRingZ3DViewerPage.ResultPanel, "Save image and annotations to the exam");
                if (Navogation1 && Navigation2 && Navigation3 && Result)
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
                //Step15 :: Click on the save button in the viewport top bar from any one of the navigation control.
                IList<IWebElement> BeforeThumbCount = Z3dViewerPage.ThumbNailList();
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.ResultPanel, "Save image and annotations to the exam");
                IWebElement step15 = Z3dViewerPage.BusyCursor();
                if (step15.Enabled)
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
                //Step16 & 17 :: Click on the close button from the Global toolbar.
                //IList<IWebElement> viewer3dbutton = Driver.FindElements(By.CssSelector(Locators.CssSelector.ViewerButton3D));
                //ClickElement(viewer3dbutton[0]);
                //try
                //{
                //    PageLoadWait.WaitForElementToDisplay(Driver.FindElement(By.CssSelector(Locators.CssSelector.DropDown3DBox)));
                //}
                //catch (Exception ex)
                //{
                //    Logger.Instance.ErrorLog("Viewer 3D button is not clicked with ClickElement method");
                //    Logger.Instance.ErrorLog("Exception in WaitForElementToDisplay is " + ex.ToString());
                //    viewer3dbutton[0].Click();
                //    PageLoadWait.WaitForElementToDisplay(Driver.FindElement(By.CssSelector(Locators.CssSelector.DropDown3DBox)));
                //}
                //IList<IWebElement> weli = Driver.FindElements(By.CssSelector(Locators.CssSelector.Layoutlist));
                //foreach (IWebElement we in weli)
                //{
                //    String str = we.Text;
                //    if (str.Equals("2D"))
                //    {
                //        ClickElement(we);
                //        break;
                //    }
                //}
                Z3dViewerPage.select3dlayout("2D");
                PageLoadWait.waitforprocessingspinner(10);
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForThumbnailsToLoad(10);
                Thread.Sleep(10000);
                IList<IWebElement> AfterThumbCount = Z3dViewerPage.ThumbNailList();
                Z3dViewerPage.selectthumbnail("S4- 1 1 CT");
                Thread.Sleep(10000);
                bool res16 = Z3dViewerPage.checkerrormsg();
                Logger.Instance.InfoLog("163540 Step 16 17 log"+ res16.ToString()+ " AfterThumbCount.Count : " + AfterThumbCount.Count + " BeforeThumbCount.Count : " + BeforeThumbCount.Count);
                if (!res16 && (AfterThumbCount.Count > BeforeThumbCount.Count))
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 18
                Z3dViewerPage.CloseViewer();
                PageLoadWait.WaitForFrameLoad(5);
                Driver.SwitchTo().DefaultContent(); Driver.SwitchTo().Frame("UserHomeFrame");
                if (Driver.FindElement(By.CssSelector("#TabText0")).Displayed)
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
                ////Step17 :: Launch the same study again in the universal viewer and verify the saved images.
                //string field = "patient";
                //login.Navigate("Studies");
                //PageLoadWait.WaitForFrameLoad(10);
                //login.ClearFields();
                //PageLoadWait.WaitForFrameLoad(10);
                //string FieldName = Z3dViewerPage.GetFieldName(field);
                //login.SearchStudy(field, PatientID1);
                //PageLoadWait.WaitForLoadingMessage(30);
                //login.SelectStudy(FieldName, PatientID1);
                //PageLoadWait.WaitForFrameLoad(5);
                //BluRingViewer.LaunchBluRingViewer(fieldname: FieldName, value: PatientID1);
                //PageLoadWait.WaitForFrameLoad(10);
                ////IList<IWebElement>AfterThumbCount = Z3dViewerPage.ThumbNailList();
                //if (AfterThumbCount.Count > BeforeThumbCount.Count)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}
                ////Step18 :: Load the saved images in the active viewport...
                //SwitchToDefault();
                //SwitchToUserHomeFrame();
                //Boolean step18 = Z3dViewerPage.selectthumbnail("Saved 3D Image", AfterThumbCount.Count - 1);
                //if (step18)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}
                ////Step19 :: Click on the close button from the Global toolbar.
                //Z3dViewerPage.CloseViewer();
                //PageLoadWait.WaitForFrameLoad(5);
                //Driver.SwitchTo().DefaultContent(); Driver.SwitchTo().Frame("UserHomeFrame");
                //if (Driver.FindElement(By.CssSelector("#TabText0")).Displayed)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}
                //Step19 :: 1. Search Load a CT study In the Z3D view and apply measurements.
                // 2.Save images by clicking on the save button with measurements applied from Z3D.
                // 3.Close the study and relaunch the same study.
                Boolean step20 = Z3dViewerPage.searchandopenstudyin3D(PatientID1, Desc1, layout: BluRingZ3DViewerPage.MPR);
                IWebElement ResultPanel = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                bool LineMeasureMent = Z3dViewerPage.select3DTools(Z3DTools.Line_Measurement);
                Z3dViewerPage.Performdragdrop(ResultPanel, ResultPanel.Size.Width / 2 + 50, ResultPanel.Size.Height / 2 , ResultPanel.Size.Width / 2, ResultPanel.Size.Height / 2);
                //new Actions(Driver).MoveToElement(ResultPanel, ResultPanel.Size.Width / 2, ResultPanel.Size.Height / 2).ClickAndHold().
                //    MoveToElement(ResultPanel , ResultPanel.Size.Width / 2 + 50, ResultPanel.Size.Height / 2).Release().Build().Perform();
                BeforeThumbCount = Z3dViewerPage.ThumbNailList();
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.ResultPanel, "Save image and annotations to the exam");
                Z3dViewerPage.CloseViewer();
                //Close and relaunch the same study....
                string field = "patient";
                login.Navigate("Studies");
                PageLoadWait.WaitForFrameLoad(10);
                login.ClearFields();
                PageLoadWait.WaitForFrameLoad(10);
                string FieldName = Z3dViewerPage.GetFieldName(field);
                login.SearchStudy(field, PatientID1);
                PageLoadWait.WaitForLoadingMessage(30);
                login.SelectStudy(FieldName, PatientID1);
                PageLoadWait.WaitForFrameLoad(5);
                BluRingViewer.LaunchBluRingViewer(fieldname: FieldName, value: PatientID1);
                PageLoadWait.WaitForFrameLoad(10);
                AfterThumbCount = Z3dViewerPage.ThumbNailList();
                if (AfterThumbCount.Count > BeforeThumbCount.Count && step20 && LineMeasureMent)
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
                //Step20 :: Click on the close button from the Global toolbar.
                Z3dViewerPage.CloseViewer();
                PageLoadWait.WaitForFrameLoad(5);
                Driver.SwitchTo().DefaultContent(); Driver.SwitchTo().Frame("UserHomeFrame");
                if (Driver.FindElement(By.CssSelector("#TabText0")).Displayed)
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
                //Step21 :: Load a 3D supported PT study in universal viewer and Select the MPR view from the smart view drop down.
                Boolean step22 = Z3dViewerPage.searchandopenstudyin3D(PatientID2, Desc2, layout: BluRingZ3DViewerPage.MPR);
                if (step22)
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
                //Step22 :: Click on the save button in the viewport top bar from any one of the navigation control.
                BeforeThumbCount = Z3dViewerPage.ThumbNailList();
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.ResultPanel, "Save image and annotations to the exam");
                IWebElement step23 = Z3dViewerPage.BusyCursor();
                if (step23.Enabled)
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

                //step 23,24
                //viewer3dbutton = Driver.FindElements(By.CssSelector(Locators.CssSelector.ViewerButton3D));
                //ClickElement(viewer3dbutton[0]);
                //try
                //{
                //    PageLoadWait.WaitForElementToDisplay(Driver.FindElement(By.CssSelector(Locators.CssSelector.DropDown3DBox)));
                //}
                //catch (Exception ex)
                //{
                //    Logger.Instance.ErrorLog("Viewer 3D button is not clicked with ClickElement method");
                //    Logger.Instance.ErrorLog("Exception in WaitForElementToDisplay is " + ex.ToString());
                //    viewer3dbutton[0].Click();
                //    PageLoadWait.WaitForElementToDisplay(Driver.FindElement(By.CssSelector(Locators.CssSelector.DropDown3DBox)));
                //}
                //weli = Driver.FindElements(By.CssSelector(Locators.CssSelector.Layoutlist));
                //foreach (IWebElement we in weli)
                //{
                //    String str = we.Text;
                //    if (str.Equals("2D"))
                //    {
                //        ClickElement(we);
                //        break;
                //    }
                //}
                Z3dViewerPage.select3dlayout("2D");
                PageLoadWait.waitforprocessingspinner(10);
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForThumbnailsToLoad(10);
                AfterThumbCount = Z3dViewerPage.ThumbNailList();
                Z3dViewerPage.selectthumbnail("S4- 1 1 CT");
                PageLoadWait.WaitForFrameLoad(2);
                bool res24 = Z3dViewerPage.checkerrormsg();
                if (!res24 && (AfterThumbCount.Count > BeforeThumbCount.Count))
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }
                //Z3dViewerPage.CloseViewer();
                //PageLoadWait.WaitForFrameLoad(5);
                //Driver.SwitchTo().DefaultContent(); Driver.SwitchTo().Frame("UserHomeFrame");
                //if (Driver.FindElement(By.CssSelector("#TabText0")).Displayed)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                ////Step25 :: Launch the same study again in the universal viewer and verify the saved images.
                //field = "patient";
                //login.Navigate("Studies");
                //PageLoadWait.WaitForFrameLoad(10);
                //login.ClearFields();
                //PageLoadWait.WaitForFrameLoad(10);
                //FieldName = Z3dViewerPage.GetFieldName(field);
                //login.SearchStudy(field, PatientID2);
                //PageLoadWait.WaitForLoadingMessage(30);
                //login.SelectStudy(FieldName, PatientID2);
                //PageLoadWait.WaitForFrameLoad(5);
                //BluRingViewer.LaunchBluRingViewer(fieldname: FieldName, value: PatientID2);
                //PageLoadWait.WaitForFrameLoad(10);
                //AfterThumbCount = Z3dViewerPage.ThumbNailList();
                //if (AfterThumbCount.Count > BeforeThumbCount.Count)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}
                ////Step26 :: Load the saved images in the active viewport .
                //SwitchToDefault();
                //SwitchToUserHomeFrame();
                //Boolean step26 = Z3dViewerPage.selectthumbnail("Saved 3D Image", AfterThumbCount.Count - 1);
                //if (step26)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}
                Z3dViewerPage.CloseViewer();
                //Step25 ::1. Load a PT study In the 3D view and apply measurements.
                //2.Save images by clicking on the save buttonwith measurements applied from Z3D
                //3.Close the 3D view.
                Boolean step27 = Z3dViewerPage.searchandopenstudyin3D(PatientID2, Desc2, layout: BluRingZ3DViewerPage.MPR);
                ResultPanel = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                LineMeasureMent = Z3dViewerPage.select3DTools(Z3DTools.Line_Measurement);
                Z3dViewerPage.Performdragdrop(ResultPanel, ResultPanel.Size.Width / 2 + 50, ResultPanel.Size.Height / 2 , ResultPanel.Size.Width / 2, ResultPanel.Size.Height / 2);
                //new Actions(Driver).MoveToElement(ResultPanel, ResultPanel.Size.Width / 2, ResultPanel.Size.Height / 2).ClickAndHold().
                //    MoveToElement(ResultPanel, ResultPanel.Size.Width / 2 + 50, ResultPanel.Size.Height / 2).Release().Build().Perform();
                BeforeThumbCount = Z3dViewerPage.ThumbNailList();
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.ResultPanel, "Save image and annotations to the exam");
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(10000);
                AfterThumbCount = Z3dViewerPage.ThumbNailList();
                //Close and relaunch the same study....
                Z3dViewerPage.CloseViewer();
                if (AfterThumbCount.Count > BeforeThumbCount.Count && step27 && LineMeasureMent)
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
                //Step26 ::1. Load a MR study In the 3D view and apply measurements.
                //2.Save images by clicking on the save button with measurements applied from Z3D
                // 3.Close the 3D view.
                //Boolean step28 = Z3dViewerPage.searchandopenstudyin3D(PatientID3, Desc3, layout: BluRingZ3DViewerPage.MPR);
                bool step28 = Z3dViewerPage.searchandopenstudyin3D(accession3, Desc3, BluRingZ3DViewerPage.MPR, field: "acc");
                ResultPanel = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.ResultPanel);
                LineMeasureMent = Z3dViewerPage.select3DTools(Z3DTools.Line_Measurement);
                Z3dViewerPage.Performdragdrop(ResultPanel, ResultPanel.Size.Width / 2 + 50, ResultPanel.Size.Height / 2 , ResultPanel.Size.Width / 2, ResultPanel.Size.Height / 2);
                //new Actions(Driver).MoveToElement(ResultPanel, ResultPanel.Size.Width / 2, ResultPanel.Size.Height / 2).ClickAndHold().
                //    MoveToElement(ResultPanel, ResultPanel.Size.Width / 2 + 50, ResultPanel.Size.Height / 2).Release().Build().Perform();
                BeforeThumbCount = Z3dViewerPage.ThumbNailList();
                Z3dViewerPage.SelectOptionsfromViewPort(BluRingZ3DViewerPage.ResultPanel, "Save image and annotations to the exam");
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                AfterThumbCount = Z3dViewerPage.ThumbNailList();
                //Close and relaunch the same study....
                if (AfterThumbCount.Count > BeforeThumbCount.Count && step28 && LineMeasureMent)
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
                Z3dViewerPage.CloseViewer();
                login.Logout();
                //Step27 :: Launch ICA Service tool, Navigate to Enable Features tab -> General tab -> disable the 'Enable Saving GSPS' and Restart IIS
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures(); // Navigate to Enable features tab in Service tool
                wpfobject.WaitTillLoad();
                servicetool.ClickModifyButton();
                servicetool.DisableSavingGSPS();
                servicetool.ClickApplyButtonFromTab();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                result.steps[++ExecutedSteps].status = "Pass";
                //Step30 :: Load a CT / PT / MR study having a series more than 15 images and load the series in 3D view. Verify that the save button in all the controls from all viewing modes
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step30 = Z3dViewerPage.searchandopenstudyin3D(PatientID1, Desc1, layout: BluRingZ3DViewerPage.MPR);
                PageLoadWait.WaitForFrameLoad(10);
                Navogation1 = Z3dViewerPage.VerifyOptionsfromViewport(BluRingZ3DViewerPage.Navigationone, "Save image and annotations to the exam");
                Navigation2 = Z3dViewerPage.VerifyOptionsfromViewport(BluRingZ3DViewerPage.Navigationtwo, "Save image and annotations to the exam");
                Navigation3 = Z3dViewerPage.VerifyOptionsfromViewport(BluRingZ3DViewerPage.Navigationthree, "Save image and annotations to the exam");
                Result = Z3dViewerPage.VerifyOptionsfromViewport(BluRingZ3DViewerPage.ResultPanel, "Save image and annotations to the exam");
                if (step30 && !Navogation1 && !Navigation2 && !Navigation3 && !Result)
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
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException + Environment.NewLine + e.ToString());

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Return Result
                return result;
            }
            finally
            {
                Z3dViewerPage.CloseViewer();
                login.Logout();
                servicetool.LaunchServiceTool();
                servicetool.NavigateToEnableFeatures(); 
                wpfobject.WaitTillLoad();
                servicetool.ClickModifyButton();
                servicetool.EnableSavingGSPS();
                servicetool.ClickApplyButtonFromTab();
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton(ServiceTool.YesBtn_Name, 1);
                wpfobject.WaitTillLoad();
                servicetool.RestartIISandWindowsServices();
                servicetool.CloseServiceTool();
                login.Logout();
                //Z3dViewerPage.DeletePriorsInEA("10.9.37.82", PatientID1, "alien");
                //Z3dViewerPage.DeletePriorsInEA("10.9.37.82", PatientID2, "icapetsy");
                //Z3dViewerPage.DeletePriorsInEA("10.9.37.82", "ABC3514923", PatientID3);
            }
        }
    }
}
