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
using System.ComponentModel;
using System.Data;
using Accord;

namespace Selenium.Scripts.Tests
{
    class CurvedMPR_NC_CurveDrawTool : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public DomainManagement domain { get; set; }
        public StudyViewer studyviewer { get; set; }
        public Cursor Cursor { get; private set; }

        public CurvedMPR_NC_CurveDrawTool(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            domain = new DomainManagement();

            studyviewer = new StudyViewer();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        public TestCaseResult Test_163317(String testid, String teststeps, int stepcount)
        {
            
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            
            String Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string testdatades = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] ssplit = testdatades.Split('|');
            string slocationvalue = ssplit[0];
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            string slocation = ssplit[0];

            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            
            try
            {
                login.LoginIConnect(username, password);
                //Step 1  From the Universal viewer , Select a 3D supported series and Select the Curved MPR option from the smart view drop down.
                z3dvp.Deletefiles(testcasefolder);
                bool res = z3dvp.searchandopenstudyin3D(Patientid, thumbnailcaption, layout: BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    throw new Exception("Unable to open study due to exception ");
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                }
                
                //Step 2 Create a path by adding the points in navigation controls.(Drawing tool manual )
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual);
                IWebElement viewcontainer = z3dvp.ViewerContainer();
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 100), (viewcontainer.Location.Y + 150));
                Thread.Sleep(1000);
                IWebElement INavigationone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);

                //Frist Point 
                new Actions(Driver).MoveToElement(INavigationone, (INavigationone.Size.Width / 2) + 18, (INavigationone.Size.Height / 4) - 50).Click().Build().Perform();
                //z3dvp.MoveAndClick(INavigationone, (INavigationone.Size.Width / 2) + 18, (INavigationone.Size.Height / 4) - 50);
                PageLoadWait.WaitForFrameLoad(10);

                //Second Point 
                Actions act3 = new Actions(Driver);
                //act3.MoveToElement(INavigationone, (INavigationone.Size.Width / 2) + 18, (INavigationone.Size.Height / 2) - 35).Click().Build().Perform();
                z3dvp.MoveAndClick(INavigationone, (INavigationone.Size.Width / 2) + 18, (INavigationone.Size.Height / 2) - 35);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], INavigationone, pixelTolerance: 10))
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

                //step 3 Right click on the navigation control 1, then right click on the Rotate tool and select the Click center option from the drop down.
                bool bclickcner=z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(5);
                if (bclickcner)
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

                //step 4 Left click and drag the mouse on the Blue Prism in the Navigation control 1.
                List<string> result4_before  = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                new Actions(Driver).MoveToElement(INavigationone, (INavigationone.Size.Width / 2) + 18, (INavigationone.Size.Height / 2) - 35)
              .ClickAndHold().DragAndDropToOffset(INavigationone, INavigationone.Size.Width - 10, INavigationone.Size.Height - 30).
               Release().Build().Perform();
                Thread.Sleep(10000);
                List<string> result4 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool bflag4 = false;
                if (result4_before[0] != result4[0] && result4_before[1] != result4[2] && result4_before[3] != result4[3])
                {
                    if (CompareImage(result.steps[ExecutedSteps], INavigationone, pixelTolerance: 10))
                    {
                        bflag4 = true;
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if(bflag4==false)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 5  Select the Reset button from the 3D tool box. 
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                Thread.Sleep(10000);
                List<string> result5 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result5[0] == result5[1] && result5[2] == result5[3] && result5[4] == result5[5] && slocation == result5[5])
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

                //step 6 Right click on the navigation control 1, then right click on the Curved drawing tool and select the Auto vessel option from the drop down
                bool bcAuto=z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                Thread.Sleep(1000);
                if (bcAuto)
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

                //Step 7 Create a path by adding the points on Aorta region in navigation controls.
                IWebElement INavigationone6 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 100), (viewcontainer.Location.Y + 150));
                Thread.Sleep(1000);
                for (int i = 0; i < 44; i++)
                {
                    BasePage.mouse_event(0x0800, 0, 0, 5, 0);
                    Thread.Sleep(1000);
                }
                Thread.Sleep(5000);

                //Frist Point 
                new Actions(Driver).MoveToElement(INavigationone6, (INavigationone6.Size.Width / 2) + 20, (INavigationone6.Size.Height / 4) - 50).Click().Build().Perform();
                //z3dvp.MoveAndClick(INavigationone6, (INavigationone6.Size.Width / 2) + 20, (INavigationone6.Size.Height / 4) - 50);
                PageLoadWait.WaitForFrameLoad(15);
                Thread.Sleep(3000);
                
                //Second Point 
                Actions act7 = new Actions(Driver);
                //act7.MoveToElement(INavigationone6, (INavigationone6.Size.Width / 2) + 20, (INavigationone6.Size.Height / 4) - 30   ).Click().Build().Perform();
                z3dvp.MoveAndClick(INavigationone6, (INavigationone6.Size.Width / 2) + 20, (INavigationone6.Size.Height / 4) - 30);
                Thread.Sleep(3000);
                Boolean checkissue = z3dvp.checkerrormsg();
                if (checkissue)
                    throw new Exception("Failed to find path");
                PageLoadWait.WaitForFrameLoad(15);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], INavigationone6, pixelTolerance: 10))
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

                //step 8 Select the Rotate tool from the 3D tool box.
                bool bclickcner8 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(1000);
                if (bclickcner8)
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

                //step 9 Left click and drag the mouse on the Blue Prism in the Navigation control 1.
                List<string> result9_before = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                new Actions(Driver).MoveToElement(INavigationone6, (INavigationone6.Size.Width / 2) + 20, (INavigationone6.Size.Height / 4) - 30)
                .ClickAndHold().DragAndDropToOffset(INavigationone6, INavigationone6.Size.Width - 10, INavigationone6.Size.Height - 30).
                Release().Build().Perform();
                Thread.Sleep(10000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                List<string> result10_before = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                bool bflag9 = false;
                if (result9_before[0] != result10_before[0] && result9_before[1] != result10_before[1] && result9_before[3] != result10_before[3])
                {
                    if (CompareImage(result.steps[ExecutedSteps], INavigationone, pixelTolerance: 10))
                    {
                        bflag9 = true;
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if(bflag9==false)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 10 Select the Reset button from the 3D tool box.
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(10000);
                List<string> result10 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result10[0] == result10[1] && result10[2] == result10[3] && result10[4] == result10[5] && slocation == result10[5])
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


                //step 11 Right click on the navigation control 1, then right click on the Curved drawing tool and select the Auto colon option from the drop down
                bool bcAuto11 = z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                Thread.Sleep(1000);
                if (bcAuto11)
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

                //step 12  Create a path by adding the points on Colon regions in navigation controls. 
                Thread.Sleep(500);
                IWebElement INavigationone12 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 100), (viewcontainer.Location.Y + 150));
                Thread.Sleep(1000);
                z3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, scrolllevel: 60);
                //for (int i = 0; i < 69; i++)
                //{
                //    BasePage.mouse_event(0x0800, 0, 0, 5, 0);
                //    Thread.Sleep(2000);
                //}
                //Frist Point 
                //new Actions(Driver).MoveToElement(INavigationone12, (INavigationone12.Size.Width / 2) + 10, (INavigationone12.Size.Height / 2) + 90).Click().Build().Perform();
                z3dvp.MoveAndClick(INavigationone12, (INavigationone12.Size.Width / 2) + 10, (INavigationone12.Size.Height / 2) + 90);
                PageLoadWait.WaitForFrameLoad(15);
                Actions act14 = new Actions(Driver);
                //SEcnond Point 
                //act14.MoveToElement(INavigationone12, (INavigationone12.Size.Width / 2) + 10, (INavigationone12.Size.Height / 2) + 115).Click().Build().Perform();
                z3dvp.MoveAndClick(INavigationone12, (INavigationone12.Size.Width / 2) + 10, (INavigationone12.Size.Height / 2) + 105);
                Thread.Sleep(10000);
                checkissue = z3dvp.checkerrormsg();
                if (checkissue)
                    throw new Exception("Failed to find path");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], INavigationone6,pixelTolerance: 10))
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

                //Step 13 Select the Rotate tool from the 3D tool box.
                bool bclickcner13 = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(2000);
                if (bclickcner13)
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

                //Step 14 Left click and drag the mouse on the Blue Prism in the Navigation control 1.
                List<string> result14_before  = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                new Actions(Driver).MoveToElement(INavigationone12, (INavigationone12.Size.Width / 2) + 10, (INavigationone12.Size.Height / 2) + 105)
                .ClickAndHold().DragAndDropToOffset(INavigationone12, INavigationone12.Size.Width - 10, INavigationone12.Size.Height - 30).
                Release().Build().Perform();
                Thread.Sleep(15000);
                bool bflag14 = false;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                List<string> result14_after = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result14_before[0] != result14_after[0] && result14_before[1] != result14_after[1] && result14_before[3] != result14_after[3])
                {
                    if (CompareImage(result.steps[ExecutedSteps], INavigationone, pixelTolerance: 10))
                    {
                        bflag14 = true;
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if(bflag14==false)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 15 Select the Reset button from the 3D tool box.
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(10000);
                List<string> result15 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result15[0] == result15[1] && result15[2] == result15[3] && result15[4] == result15[5] && slocation == result10[5])
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
                login.Logout();
            }
        }

        public TestCaseResult Test_163311(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] TestData = TestDataRequirements.Split('|');
            String ResetLoc = TestData[0];
            String AortaLoc = TestData[1];
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1  - Log in iCA and Navigate to studies tab
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, BluRingZ3DViewerPage.CurvedMPR);
                if (step1)
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
                

                //step:2 - Top of the Aorta should be visible
                //Z3dViewerPage.EnterThickness(BluRingZ3DViewerPage.Navigationone, "1");
                System.Drawing.Point location = Z3dViewerPage.ControllerPoints(BluRingZ3DViewerPage.Navigationone);
                int xcoordinate = location.X;
                int ycoordinate = location.Y;
                Cursor.Position = new System.Drawing.Point(xcoordinate + 48, ycoordinate + 48);
                Thread.Sleep(1000);
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage.Navigationone, AortaLoc, scrolllevel: 34);
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation1))
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

                //step:3 - Add a point at the top of the aorta displayed on navigation control 1
                IWebElement ViewerContainer = Z3dViewerPage.ViewerContainer();
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                IWebElement navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                Z3dViewerPage.MoveAndClick(navigation1, navigation1.Size.Width / 2 + 20, 30);
                PageLoadWait.WaitForFrameLoad(5);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:4 - Add a 2nd point along the aorta displayed on navigation control 1
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Z3dViewerPage.MoveAndClick(navigation1, navigation1.Size.Width / 2 + 20, 60);
                PageLoadWait.WaitForFrameLoad(5);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:5 - Add the 3rd point on the aorta below the 2nd point by clicking on “Navigation 2” image this time
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Z3dViewerPage.MoveClickAndHold(navigation1, (navigation1.Size.Width / 2) + 20, 60);
                //new TestCompleteAction().MoveClickAndHold(navigation1, (navigation1.Size.Width / 2) + 18, 60);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.checkerrormsg();
                IWebElement Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 20, 120);
                PageLoadWait.WaitForFrameLoad(10);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:6 - Add a 4th point between 3rd and 2nd point on navigation control 3.
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.Performdragdrop(Navigation2, (Navigation2.Size.Width / 7) * 4, (Navigation2.Size.Width / 5) * 2, (Navigation2.Size.Width / 8) * 7, (Navigation2.Size.Height / 6));
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                IWebElement Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                Z3dViewerPage.MoveAndClick(Navigation3, (Navigation3.Size.Width / 2) + 22, 90);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:7 - Right click on the images in MPR navigation controls/ MPR path navigation/ Curved MPR control and drag the mouse
                String BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                String AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                IWebElement CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                DownloadImageFile(ViewerContainer, BeforeImagePath, removeCurserFromPage: true);
                //navigation 1
                location = Z3dViewerPage.ControllerPoints(BluRingZ3DViewerPage.Navigationone);
                xcoordinate = location.X;
                ycoordinate = location.Y;
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 50);
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate + 50);
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                //navigation 2
                location = Z3dViewerPage.ControllerPoints(BluRingZ3DViewerPage.Navigationtwo);
                xcoordinate = location.X;
                ycoordinate = location.Y;
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 50);
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate + 50);
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                //navigation 3
                location = Z3dViewerPage.ControllerPoints(BluRingZ3DViewerPage.Navigationthree);
                xcoordinate = location.X;
                ycoordinate = location.Y;
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 50);
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate + 50);
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                //MPR Path Navigation
                location = Z3dViewerPage.ControllerPoints(BluRingZ3DViewerPage.MPRPathNavigation);
                xcoordinate = location.X;
                ycoordinate = location.Y;
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 50);
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate + 150);
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                PageLoadWait.WaitForFrameLoad(5);
                DownloadImageFile(ViewerContainer, AfterImagePath, removeCurserFromPage: true);
                if (!CompareImage(BeforeImagePath, AfterImagePath))
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

                //step 8 = Right click on the image in 3D path navigation and drag the mouse
                IWebElement Path3DNavigation = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                DownloadImageFile(Path3DNavigation, BeforeImagePath);
                location = Z3dViewerPage.ControllerPoints(BluRingZ3DViewerPage._3DPathNavigation);
                xcoordinate = location.X;
                ycoordinate = location.Y;
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 50);
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate + 50);
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                
                PageLoadWait.WaitForFrameLoad(5);
                DownloadImageFile(Path3DNavigation, AfterImagePath);
                if (!CompareImage(BeforeImagePath, AfterImagePath))
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
                
                //step:9 - Drop down list is displayed with the following options: 1) Delete Path 2) Delete Control Point
                Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                if (browserName.ToLower().Contains("mozilla") || browserName.ToLower().Contains("firefox"))
                    Z3dViewerPage.MoveAndClick(Navigation3, (Navigation3.Size.Width / 2) + 22, 90);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.MoveAndClick(Navigation3, (Navigation3.Size.Width / 2) + 22, 90, Rclick: true);
                PageLoadWait.WaitForFrameLoad(2);
                IWebElement CtrlPointdropdown;
                try
                {
                    CtrlPointdropdown = Driver.FindElement(By.CssSelector(Locators.CssSelector.ctrlpointdropdown));
                }catch(NoSuchElementException ex)
                {
                    Logger.Instance.ErrorLog("Error in step9 163311 contex click not done by actions class " + ex.InnerException);
                    new TestCompleteAction().MoveToElement(Navigation3, (Navigation3.Size.Width / 2) + 22, 90).ContextClick();
                    CtrlPointdropdown = Driver.FindElement(By.CssSelector(Locators.CssSelector.ctrlpointdropdown));
                }            
                IList<IWebElement> CtrlPointopt = CtrlPointdropdown.FindElements(By.CssSelector(Locators.CssSelector.ctrlpointoptions));
                if(CtrlPointopt[0].Text == "Delete Path" && CtrlPointopt[1].Text == "Delete Control Point")
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

                //step:10 - Select the 'Delete Control Point' option
                CtrlPointopt[1].Click();
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:11 - Select the scroll tool from the 3D tool box and scroll down on the image displayed in navigation control 3 until more of the bottom part of the aorta is visible.
                Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                //new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.Performdragdrop(Navigation3, (Navigation3.Size.Width / 2) + 20, Navigation3.Size.Width / 2, (Navigation3.Size.Width / 2) + 110, 60);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:12 - Click on the 1st point
                //new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual);
                PageLoadWait.WaitForFrameLoad(5);
                Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage.Navigationthree, AortaLoc, ScrollDirection: "down", scrolllevel: 10);
                Z3dViewerPage.MoveAndClick(Navigation3, Navigation3.Size.Width / 2 + 20, 30);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:13 - Left click on the 2nd point and drag the point slightly to the left of the original position
                Z3dViewerPage.Performdragdrop(Navigation3, Navigation3.Size.Width / 2, 60, (Navigation3.Size.Width / 2) + 20, 60);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:14 - Left click on the 2nd point and drag the point to its original position
                Z3dViewerPage.Performdragdrop(Navigation3, (Navigation3.Size.Width / 2) + 20, 60, Navigation3.Size.Width / 2, 60);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:15 - Add a 4th point on the aorta below the 3rd point by clicking on “Navigation 3” image this time
                Z3dViewerPage.Performdragdrop(Navigation3, Navigation3.Size.Width / 2, 150, Navigation3.Size.Width / 2, 150);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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
                //step:16 - Right click on the path on any of the navigation controls and select the "Delete Path" option from the drop down list.
                Z3dViewerPage.CurvedPathdeletor(Navigation3, (Navigation3.Size.Width / 2) + 20, 120, "Delete Path");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer))
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

                //step:17 -  	Select the Reset button from the Z3D tool box.
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                List<string> step17 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (step17[0] == ResetLoc && step17[1] == ResetLoc && step17[2] == ResetLoc && step17[3] == ResetLoc && step17[4] == ResetLoc && step17[5] == ResetLoc)
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
            }
        }
        public TestCaseResult Test_163316(String testid, String teststeps, int stepcount)
        {
            
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            String Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            String thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string testdatades = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] ssplit = testdatades.Split('|');
            string slocationvalue = ssplit[0];
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            string slocation = ssplit[0];
            String testcasefolder = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "TestData" + Path.DirectorySeparatorChar + testid + Path.DirectorySeparatorChar;
            try
            {
                login.LoginIConnect(username, password);
                //Step 1 & step 2  From the Universal viewer , Select a 3D supported series and Select the Curved MPR option from the smart view drop down.
                z3dvp.Deletefiles(testcasefolder);
                bool res = z3dvp.searchandopenstudyin3D(Patientid, thumbnailcaption, layout: BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    throw new Exception("Unable to open study due to exception ");
                else
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                }
                

                //Step 3 Click on navigation control 1 and scroll up until the top of the colon is visible. 
                String annotationvalbefore = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //new Actions(Driver).SendKeys("X").Build().Perform();
                IWebElement viewcontainer = z3dvp.ViewerContainer();
                Thread.Sleep(1000);
                z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5, BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(1000);
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(1000);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 100), (viewcontainer.Location.Y + 150));
                Thread.Sleep(1000);
                z3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, scrolllevel: 60);
                //if (Config.BrowserType.ToLower() == "chrome")
                //{
                //    for (int i = 0; i < 88; i++)
                //    {
                //        BasePage.mouse_event(0x0800, 0, 0, 5, 0);
                //        Thread.Sleep(1000);
                //    }
                //}
                //else
                //{
                //    //for (int i = 0; i < 83; i++)
                //    //{
                //    //    BasePage.mouse_event(0x0800, 0, 0, 10, 0);
                //    //    Thread.Sleep(1000);
                //    //}
                //    int t = 0;
                //    do
                //    {
                //        BasePage.mouse_event(0x0800, 0, 0, 10, 0);
                //        Thread.Sleep(1000);
                //        t++;
                //        if (t > 100) break;
                //    }
                //    while (z3dvp.checkvalue(Locators.CssSelector.LeftTopPane,2,0,1) <= 66);
                //}
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                String annotationvalafter = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (annotationvalafter != annotationvalbefore && CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), pixelTolerance: 50))
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
                //Step 4   Add a point at the top of the colon displayed on navigation control 1. 
                //Step 5  Add a 2nd point along the colon displayed on navigation control 1. 
                IWebElement INavigationone4 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                //first point 
                if (Config.BrowserType.ToLower() == "chrome")
                    new Actions(Driver).MoveToElement(INavigationone4, (INavigationone4.Size.Width / 2) + 5, (INavigationone4.Size.Height / 2) + 90).Click().Build().Perform();
                else
                    new Actions(Driver).MoveToElement(INavigationone4, (INavigationone4.Size.Width / 2)+8, (INavigationone4.Size.Height - 81)).Click().Build().Perform();
                    //z3dvp.MoveAndClick(INavigationone4, (INavigationone4.Size.Width / 2) + 8, (INavigationone4.Size.Height - 81));
                Thread.Sleep(3000);
                //second point 
                Actions act5 = new Actions(Driver);
                if (Config.BrowserType.ToLower() == "chrome")
                    act5.MoveToElement(INavigationone4, (INavigationone4.Size.Width / 2) + 5, (INavigationone4.Size.Height / 2) + 130).Click().Build().Perform();
                else
                   act5.MoveToElement(INavigationone4, (INavigationone4.Size.Width / 2) + 8, (INavigationone4.Size.Height - 79)).Click().Build().Perform();
                 //   z3dvp.MoveAndClick(INavigationone4, (INavigationone4.Size.Width / 2) + 8, (INavigationone4.Size.Height - 79));
                Thread.Sleep(5000);
                Boolean checkissue = z3dvp.checkerrormsg();
                if (checkissue)
                    throw new Exception("Failed to find path");
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), pixelTolerance: 10))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //STep 6  Add the 3rd point on the colon below the 2nd point by clicking on “Navigation 2” image this time.
                IWebElement iNavigationtwo6 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Thread.Sleep(1000);
                act5 = new Actions(Driver);
                if (Config.BrowserType.ToLower() == "chrome")
                {
                    new Actions(Driver).MoveToElement(iNavigationtwo6).SendKeys("x").Build().Perform();
                    Thread.Sleep(3000);
                    act5.MoveToElement(INavigationone4, (INavigationone4.Size.Width / 2) + 5, (INavigationone4.Size.Height / 2) + 130).ClickAndHold().Build().Perform();
                    Thread.Sleep(3000);
                    act5.Release().Build().Perform();
                    Thread.Sleep(3000);
                    new Actions(Driver).MoveToElement(iNavigationtwo6).SendKeys("x").Build().Perform();
                    Thread.Sleep(3000);
                    Actions act6 = new Actions(Driver);
                   act6.MoveToElement(iNavigationtwo6, (iNavigationtwo6.Size.Width / 2 + 20), (iNavigationtwo6.Size.Height - 50)).Click().Build().Perform();
                  }
                    else
                  {
                    new Actions(Driver).MoveToElement(iNavigationtwo6, (iNavigationtwo6.Size.Width / 2 +25), (iNavigationtwo6.Size.Height - 55)).Click().Build().Perform();
                  //  z3dvp.MoveAndClick(iNavigationtwo6, (iNavigationtwo6.Size.Width / 2 + 25), (iNavigationtwo6.Size.Height - 55));
                  }
                Thread.Sleep(5000);
                bool checkerror = z3dvp.checkerrormsg();
                if(checkerror)
                    throw new Exception("Failed to find path");
                Thread.Sleep(1000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), pixelTolerance: 10))
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

                //Step 7 Add a 4th point on navigation control 3, but this time click a location along the path already created between the 3rd and 2nd points.

                //precondtion starts here 
                new Actions(Driver).SendKeys("X").Build().Perform();
                Thread.Sleep(2000);
                iNavigationtwo6 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Actions step7 = new Actions(Driver);
                if (Config.BrowserType.ToLower() == "chrome")
                    step7.MoveToElement(iNavigationtwo6, (iNavigationtwo6.Size.Width / 2 + 20), (iNavigationtwo6.Size.Height - 50)).ClickAndHold().Build().Perform();
                else
                    new Actions(Driver).MoveToElement(iNavigationtwo6, (iNavigationtwo6.Size.Width / 2 + 25), (iNavigationtwo6.Size.Height - 55)).ClickAndHold().Build().Perform();
                 //   z3dvp.MoveClickAndHold(iNavigationtwo6, (iNavigationtwo6.Size.Width / 2 + 25), (iNavigationtwo6.Size.Height - 55));
                Thread.Sleep(5000);
                step7.Release().Build().Perform();
                Thread.Sleep(5000);
                Accord.Point p11 = z3dvp.GetIntersectionPoints(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), testid, ExecutedSteps, "yellow", "Vertical", 0);
                Accord.Point p1 = z3dvp.GetIntersectionPoints(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), testid, ExecutedSteps + 1, "red", "Horizontal", 1);
                Thread.Sleep(10000);
                Actions act7_a = new Actions(Driver);
                act7_a.MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), (int)p1.X, (int)p1.Y).ClickAndHold()
                    .MoveToElement(z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), (int)p11.X, (int)p11.Y).Release().Build().Perform();
                Thread.Sleep(20000);
                new Actions(Driver).SendKeys("X").Build().Perform();
                //Precondition ends here 

                IWebElement iNavigationthree7 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                Actions act7 = new Actions(Driver);
                if (Config.BrowserType.ToLower() == "chrome")
                    act7.MoveToElement(iNavigationthree7, (iNavigationthree7.Size.Width / 2) - 5, (iNavigationthree7.Size.Height / 4 - 47)).Click().Build().Perform();
                else
                    //    act7.MoveToElement(iNavigationthree7, (iNavigationthree7.Size.Width / 2) - 5, (iNavigationthree7.Size.Height - 84)).Click().Build().Perform();
                    new Actions(Driver).MoveToElement(iNavigationthree7, (iNavigationthree7.Size.Width / 2 + 15), (iNavigationthree7.Size.Height / 4 - 45)).Click().Build().Perform();
                  //  z3dvp.MoveAndClick(iNavigationthree7, (iNavigationthree7.Size.Width / 2 + 15), (iNavigationthree7.Size.Height / 4 - 45));
                Thread.Sleep(5000);
                bool res7 = z3dvp.checkerrormsg();
                if (res7)
                    throw new Exception("Failed to find path");
                //z3dvp.checkerrormsg("y");
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 700), (viewcontainer.Location.Y + 500));
                Thread.Sleep(1000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), pixelTolerance: 50))
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


                //step 8 Select the scroll tool from the 3D tool box and scroll through the path that is displayed on the MPR path navigation control.
                IList<string> result8_before = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.MPRPathNavigation);
                Thread.Sleep(1000);
                z3dvp.SelectControl(BluRingZ3DViewerPage.MPRPathNavigation);
                //this.Cursor = new Cursor(Cursor.Current.Handle);
                //Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 550), (viewcontainer.Location.Y / 2 + 600));
                //Thread.Sleep(500);
                IWebElement Layout = z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                z3dvp.Performdragdrop(Layout, Layout.Size.Width / 2, Layout.Size.Height / 2, Layout.Size.Width / 4, Layout.Size.Height / 4);
                //for (int i = 0; i < 5; i++)
                //{
                //    BasePage.mouse_event(0x0800, 0, 0, -2, 0);
                //    Thread.Sleep(1000);
                //}
                Thread.Sleep(10000);
                bool bflag8 = false;
                
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                IList<string> result8_after = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result8_before[2] != result8_after[2] && result8_before[4] != result8_after[4])
                {
                    // z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, false);
                    new Actions(Driver).SendKeys("D").Build().Perform();
                    Thread.Sleep(500);
                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), pixelTolerance: 50))
                    {
                        bflag8 = true;
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    new Actions(Driver).SendKeys("D").Build().Perform();
                    Thread.Sleep(500);
                }
                if(bflag8==false)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, true);

                //Step 9 Select the scroll tool from the 3D tool box and scroll through the path that is displayed on the 3D path navigation control.
                IList<string> result9_before = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                z3dvp.SelectControl(BluRingZ3DViewerPage._3DPathNavigation);
                IWebElement threeDpath = z3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                //this.Cursor = new Cursor(Cursor.Current.Handle);
                //Cursor.Position = new System.Drawing.Point((threeDpath.Location.X +50), (threeDpath.Location.Y/2+250 ));
                Thread.Sleep(500);
                Layout = z3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                z3dvp.Performdragdrop(Layout, Layout.Size.Width / 2, Layout.Size.Height / 2, Layout.Size.Width / 4, Layout.Size.Height / 4);
                //for (int i = 0; i < 15; i++)
                //{
                //    BasePage.mouse_event(0x0800, 0, 0, -2, 0);
                //    Thread.Sleep(1000);
                //}
                Thread.Sleep(5000);
                bool bflag9 = false;
               
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                IList<string> result9_after = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result9_before[2] != result9_after[2] && result9_before[4] != result9_after[4])
                {
                    //   z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, false);
                    new Actions(Driver).SendKeys("D").Build().Perform();
                    Thread.Sleep(500);
                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation), pixelTolerance: 500))
                    {
                        bflag9 = true;
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    new Actions(Driver).SendKeys("D").Build().Perform();
                    Thread.Sleep(500);
                }
                if(bflag9==false)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
             //   z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, true);
                //step 10 Select the reset button from the 3D tool box.
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                Thread.Sleep(10000);
                List<string> result3 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result3[0] == result3[1] && result3[2] == result3[3] && result3[4] == result3[5] && slocationvalue == (result3[0]))
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

                //Step 11 Right click on the navigation control 1, then right click on the Curved drawing tool and select the Auto vessel option from the drop down
                bool bflag13 = z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5, BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(1000);
                if(bflag13)
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
                //Step 12 Click on navigation control 1 and scroll up until the top of the Aorta is visible. 
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 100), (viewcontainer.Location.Y + 150));
                Thread.Sleep(500);
                z3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, scrolllevel: 34);
                //for (int i = 0; i < 44; i++)
                //{
                //    BasePage.mouse_event(0x0800, 0, 0, 5, 0);
                //    Thread.Sleep(1000);
                //}
               // do
                //{
                //    BasePage.mouse_event(0x0800, 0, 0, 10, 0);
                //    Thread.Sleep(1000);
                //}
                //while (z3dvp.checkvalue(Locators.CssSelector.LeftTopPane, 2, 0, 1) <= 30);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
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
                //step 13 Add a point at the top of the Aorta displayed on navigation control 1.
                IWebElement INavigationone = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                Thread.Sleep(1000);
                new Actions(Driver).MoveToElement(INavigationone, (INavigationone.Size.Width / 2) + 18, (INavigationone.Size.Height / 4) - 50).Click().Build().Perform();
                //    PageLoadWait.WaitForFrameLoad(10);
                //z3dvp.MoveAndClick(INavigationone, (INavigationone.Size.Width / 2) + 18, (INavigationone.Size.Height / 4) - 50);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
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

                //step 14 Add a 2nd point along the Aorta displayed on navigation control 1.
                Actions act16 = new Actions(Driver);
                act16.MoveToElement(INavigationone, (INavigationone.Size.Width / 2) + 18, (INavigationone.Size.Height / 4) - 53).Click().Build().Perform();
              //  z3dvp.MoveAndClick(INavigationone, (INavigationone.Size.Width / 2) + 18, (INavigationone.Size.Height / 4) - 35);
                Thread.Sleep(1000);
                checkissue = z3dvp.checkerrormsg();
                if (checkissue)
                    throw new Exception("Failed to find path");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
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

                //step 15  Add the 3rd point on the Aorta below the 2nd point by clicking on “Navigation 2” image this time. 
                new Actions(Driver).MoveToElement(INavigationone).SendKeys("x").Build().Perform();
                //  PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(3000);
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                Thread.Sleep(2000);
                Actions action = new Actions(Driver);
                action.MoveToElement(INavigationone, (INavigationone.Size.Width / 2) + 18, (INavigationone.Size.Height / 4) - 35).ClickAndHold().Build().Perform();
                //  PageLoadWait.WaitForFrameLoad(10);
            //    z3dvp.MoveClickAndHold(INavigationone, (INavigationone.Size.Width / 2) + 18, (INavigationone.Size.Height / 4) - 35);
                Thread.Sleep(3000);
                action.Release().Build().Perform();
                //   PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(3000);
                //Navigation 2
                new Actions(Driver).MoveToElement(INavigationone).SendKeys("x").Build().Perform();
                // PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(3000);
                z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                Thread.Sleep(2000);
                IWebElement INavigationtwo = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Actions act17 = new Actions(Driver);
                act17.MoveToElement(INavigationtwo, (INavigationtwo.Size.Width / 2) + 18, (INavigationtwo.Size.Height / 4) - 25).Click().Build().Perform();
            //    z3dvp.MoveAndClick(INavigationtwo, (INavigationtwo.Size.Width / 2) + 18, (INavigationtwo.Size.Height / 4) - 25);
                //    PageLoadWait.WaitForFrameLoad(120);
                Thread.Sleep(3000);
                checkerror = z3dvp.checkerrormsg();
                if (checkerror)
                    throw new Exception("Failed to find path");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo)))
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
                //Step 16 Select the scroll tool from the 3D tool box and scroll through the path that is displayed on the MPR path navigation control.
                IList<string> result18 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.MPRPathNavigation);
                Thread.Sleep(1000);
                z3dvp.SelectControl(BluRingZ3DViewerPage.MPRPathNavigation);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 550), (viewcontainer.Location.Y / 2 + 600));
                //   PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(3000);
                Layout = z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                z3dvp.Performdragdrop(Layout, Layout.Size.Width / 2, Layout.Size.Height / 2, Layout.Size.Width / 4, Layout.Size.Height / 4);
                //for (int i = 0; i < 5; i++)
                //{
                //    BasePage.mouse_event(0x0800, 0, 0, -2, 0);
                //    //   PageLoadWait.WaitForFrameLoad(2);
                //    Thread.Sleep(2000);
                //}
                //PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                bool bflag18 = false;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                IList<string> result18_after = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result18[2] != result18_after[2] && result18[4] != result18_after[4])
                {
                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)))
                    { 
                        bflag18 = true;
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if(bflag18==false)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                checkerror = z3dvp.checkerrormsg();
                if (checkerror)
                    throw new Exception("Error encountered while scrolling in MPR Path Control");

                //Step 17 Select the scroll tool from the 3D tool box and scroll through the path that is displayed on the 3D path navigation control.
                IList<string> result19_before = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                z3dvp.SelectControl(BluRingZ3DViewerPage._3DPathNavigation);
                IWebElement threeDpath19 = z3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((threeDpath19.Location.X + 50), (threeDpath19.Location.Y / 2 + 250));
                //   PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(3000);
                //for (int i = 0; i < 5; i++)
                //{
                //    BasePage.mouse_event(0x0800, 0, 0, -2, 0);
                //    // PageLoadWait.WaitForFrameLoad(2);
                //    Thread.Sleep(2000);
                //}
                Layout = z3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                z3dvp.Performdragdrop(Layout, Layout.Size.Width / 2, Layout.Size.Height / 2, Layout.Size.Width / 4, Layout.Size.Height / 4);
                // PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(5000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                bool bflag19 = false;
                IList<string> result19_after = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result19_before[2] != result19_after[2] && result19_before[4] != result19_after[4])
                {
                    //  z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, false);
                    new Actions(Driver).SendKeys("D").Build().Perform();
                    Thread.Sleep(500);
                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation)))
                    {
                        bflag19 = true;
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    new Actions(Driver).SendKeys("D").Build().Perform();
                    Thread.Sleep(500);
                }
                if(bflag19==false)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                checkerror = z3dvp.checkerrormsg();
                if (checkerror)
                    throw new Exception("Error encountered while scrolling in 3d Path Control");
                //     z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, true);
                //    Thread.Sleep(1000);
                
                //step 18  Select the reset button from the 3D tool box.
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                //  PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(2000);
                List<string> result20 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result20[0] == result20[1] && result20[2] == result20[3] && result20[4] == result20[5] && slocationvalue == (result20[0]))
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

                //Step 19 Right click on the navigation control 1, then right click on the Curved drawing tool and select the Manual  option from the drop down
                bool bflag21 =z3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual,BluRingZ3DViewerPage.Navigationone);
                if(bflag21)
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
                ////Step 20  scroll upto aorta  visitble 
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 100), (viewcontainer.Location.Y + 150));
                Thread.Sleep(500);
                z3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, scrolllevel: 34);
                //for (int i = 0; i < 44; i++)
                //{
                //    BasePage.mouse_event(0x0800, 0, 0, 5, 0);
                //    Thread.Sleep(1000);
                //}
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
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

                //step 21 Add a point at the top of the colon displayed on navigation control 1.
                new Actions(Driver).MoveToElement(INavigationone, (INavigationone.Size.Width / 2) + 18, (INavigationone.Size.Height / 4) - 50).Click().Build().Perform();
                Thread.Sleep(1000);
                //  PageLoadWait.WaitForFrameLoad(10);
              //  z3dvp.MoveAndClick(INavigationone, (INavigationone.Size.Width / 2) + 18, (INavigationone.Size.Height / 4) - 50);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
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
                //Step 22 Add a 2nd point along the colon displayed on navigation control 1.
                Actions act24 = new Actions(Driver);
                act24.MoveToElement(INavigationone, (INavigationone.Size.Width / 2) + 18, (INavigationone.Size.Height / 4) - 35).Click().Build().Perform();
                //z3dvp.MoveAndClick(INavigationone, (INavigationone.Size.Width / 2) + 18, (INavigationone.Size.Height / 4) - 35);
                Thread.Sleep(1000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
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

                //Step 23  Add the 3rd point on the colon below the 2nd point by clicking on “Navigation 2” image this time.
                new Actions(Driver).MoveToElement(INavigationone).SendKeys("x").Build().Perform();
                //  PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(3000);
                Actions action25 = new Actions(Driver);
                action25.MoveToElement(INavigationone, (INavigationone.Size.Width / 2) + 18, (INavigationone.Size.Height / 4) - 35).ClickAndHold().Build().Perform();
              //  z3dvp.MoveClickAndHold(INavigationone, (INavigationone.Size.Width / 2) + 18, (INavigationone.Size.Height / 4) - 35);
                //PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(3000);
                action25.Release().Build().Perform();
                //PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(3000);
                //Navigation 2
                new Actions(Driver).MoveToElement(INavigationone).SendKeys("x").Build().Perform();
                //PageLoadWait.WaitForFrameLoad(10);
                Thread.Sleep(3000);
                Actions act25_b = new Actions(Driver);
                act25_b.MoveToElement(INavigationtwo, (INavigationtwo.Size.Width / 2) + 18, (INavigationtwo.Size.Height / 4) - 25).Click().Build().Perform();
                //PageLoadWait.WaitForFrameLoad(10);
           //     z3dvp.MoveAndClick(INavigationtwo, (INavigationtwo.Size.Width / 2) + 18, (INavigationtwo.Size.Height / 4) - 25);
                Thread.Sleep(3000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo)))
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

                //Step 24 Select the scroll tool from the 3D tool box and scroll through the path that is displayed on the MPR path navigation control.
                IList<string> result26_before = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                z3dvp.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.MPRPathNavigation);
                Thread.Sleep(1000);
                z3dvp.SelectControl(BluRingZ3DViewerPage.MPRPathNavigation);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((viewcontainer.Location.X + 550), (viewcontainer.Location.Y / 2 + 600));
                Thread.Sleep(500);
                Layout = z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                z3dvp.Performdragdrop(Layout, Layout.Size.Width / 2, Layout.Size.Height / 2, Layout.Size.Width / 4, Layout.Size.Height / 4);
                //for (int i = 0; i < 5; i++)
                //{
                //    BasePage.mouse_event(0x0800, 0, 0, -2, 0);
                //    Thread.Sleep(1000);
                //}
                Thread.Sleep(5000);
                bool bflag26 = false;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                IList<string> result26_after = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result26_before[2] != result26_after[2] && result26_before[4] != result26_after[4])
                {
                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)))
                    {
                        bflag26 = true;
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                }
                if(bflag26==false)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 25 Select the scroll tool from the 3D tool box and scroll through the path that is displayed on the 3D path navigation control.
                IList<string> result27_before = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                z3dvp.SelectControl(BluRingZ3DViewerPage._3DPathNavigation);
                IWebElement threeDpath27 = z3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point((threeDpath27.Location.X + 50), (threeDpath27.Location.Y / 2 + 250));
                Thread.Sleep(500);
                Layout = z3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                z3dvp.Performdragdrop(Layout, Layout.Size.Width / 2, Layout.Size.Height / 2, Layout.Size.Width / 4, Layout.Size.Height / 4);
                //for (int i = 0; i < 5; i++)
                //{
                //    BasePage.mouse_event(0x0800, 0, 0, -2, 0);
                //    Thread.Sleep(1000);
                //}
                Thread.Sleep(5000);
                bool bflag27 = false;
                
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                IList<string> result27_after = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result27_before[2] != result27_after[2] && result27_before[4] != result27_after[4])
                {
                    //z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, false);
                    //Thread.Sleep(1000);
                    new Actions(Driver).SendKeys("D").Build().Perform();
                    Thread.Sleep(500);
                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation)))
                    {
                        bflag27 = true;
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    new Actions(Driver).SendKeys("D").Build().Perform();
                    Thread.Sleep(500);
                }
                if(bflag27==false)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, true);
                //Thread.Sleep(1000);
               
                //step 26 Select the rotate tool and apply on the image in 3D path navigation control.
                z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Image_Center, BluRingZ3DViewerPage._3DPathNavigation);
                List<String> result28_before  = z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                Actions act28 = new Actions(Driver);
                act28.MoveToElement(threeDpath27, threeDpath27.Size.Width / 2, threeDpath27.Size.Height / 2).ClickAndHold()
                    .MoveToElement(threeDpath27, threeDpath27.Size.Width / 2, threeDpath27.Size.Height / 2 + 100).Release().Build().Perform();
                Thread.Sleep(10000);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                Thread.Sleep(1000);
                bool bflag28 = false;
                List<String> result28= z3dvp.GetAttributes_Result(Locators.CssSelector.CenterTopPane, null, null, 0);
                if (result28[2] != result28_before[2] && result28[4] != result28_before[4])
                {
                    //z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, false);
                    //Thread.Sleep(1000);
                    new Actions(Driver).SendKeys("D").Build().Perform();
                    Thread.Sleep(500);

                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                    {
                    bflag28 = true;
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    new Actions(Driver).SendKeys("D").Build().Perform();
                    Thread.Sleep(500);
                }
                if(bflag28==false)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //z3dvp.change3dsettings(BluRingZ3DViewerPage.DisplayAnnotations, 0, true);
                //Thread.Sleep(1000);

                //Step 27  Right-left click and drag the mouse on the image in 3D path navigation control.
                //new Actions(Driver).MoveToElement(threeDpath27).DragAndDropToOffset(threeDpath27, 100, 25).Build().Perform();
                System.Drawing.Point location = z3dvp.ControllerPoints(BluRingZ3DViewerPage._3DPathNavigation);
                int xcoordinate = location.X;
                int ycoordinate = location.Y;
                Thread.Sleep(5000);
                String step30before1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 50);
                BasePage.mouse_event(0x00000002, 0, 0, 0, 0);
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate + 50);
                PageLoadWait.WaitForFrameLoad(5);
                String step30After1 = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                BasePage.mouse_event(0x00000004, 0, 0, 0, 0);
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(10000);
                bool bflag29 = false;
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation)))
                {

                    result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 2); Thread.Sleep(1000);
                    if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation)))
                    {
                        result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 3); Thread.Sleep(1000);
                        if (CompareImage(result.steps[ExecutedSteps], z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)))
                        {
                            bflag29 = true;
                            result.steps[ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                    }
                }
                if (bflag29 == false)
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 28 Select the reset button from the 3D tool box.
                z3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.MPRPathNavigation);
                Thread.Sleep(10000);
                List<string> result30 = z3dvp.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (result30[0] == result30[1] && result30[2] == result30[3] && result30[4] == result30[5] && slocationvalue == (result30[0]))
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
                //Logout
               // login.Logout();
                //Return Result
                return result;
            }
            finally
            {
                z3dvp.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163312(String testid, String teststeps, int stepcount)
        {


            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
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
                String objthumbimg = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
                String objTestRequirement = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
                String objlocval1 = objTestRequirement.Split('|')[0];
                String objlocval2 = objTestRequirement.Split('|')[1];
                String objlocval3 = objTestRequirement.Split('|')[2];
                IWebElement Navigation1, Navigation2, PathNavigationMPR, PathNavigation3D, CurvedMPRNavigation, Navigation3;

                //step 01
                login.LoginIConnect(adminUserName, adminPassword);
                bool res = brz3dvp.searchandopenstudyin3D(objpatid, objthumbimg, layout: BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    throw new Exception("unable to launch study in 3D Viewer");
                else
                    result.steps[++ExecutedSteps].StepPass();
                
                //step 02
                res = brz3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage._3DPathNavigation,"check",BluRingZ3DViewerPage.Flip);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                {
                    res = brz3dvp.SelectRender_PresetMode(BluRingZ3DViewerPage.MPRPathNavigation, "check", BluRingZ3DViewerPage.Flip);
                    if (!res)
                        result.steps[++ExecutedSteps].StepFail();
                    else
                        result.steps[++ExecutedSteps].StepPass();
                }

                //step 03
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                if(!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 04
                res = brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, objlocval1);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 05
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Navigation1).SendKeys("x").Build().Perform();
                int BlueColorValBefore = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 51, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 60);
                PageLoadWait.WaitForFrameLoad(20);
                int BlueColorValAfter_5 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 52, 0, 0, 255, 2, isMoveCursor: true);
                if (BlueColorValAfter_5 != BlueColorValBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 06
                res = brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, objlocval2, "down");
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                {
                    res = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone).Equals(objlocval2);
                    if (!res)
                        result.steps[++ExecutedSteps].StepFail();
                    else
                        result.steps[++ExecutedSteps].StepPass();
                }

                //step 07
                CurvedMPRNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                String MPRNavigationPathLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                String NavigationPath3DLocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);

                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 12, (Navigation1.Size.Height / 2) - 40);
                PageLoadWait.WaitForFrameLoad(20);
                Boolean checkissue = brz3dvp.checkerrormsg();
                if (checkissue)
                    throw new Exception("Failed to find path");
                int BlueColorValAfter_7 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 72, 0, 0, 255, 2);
                String MPRNavigationPathLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                String NavigationPath3DLocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                bool result1 = MPRNavigationPathLocValAfter.Equals(MPRNavigationPathLocValBefore) && NavigationPath3DLocValAfter.Equals(NavigationPath3DLocValBefore);
                bool result2 = MPRNavigationPathLocValAfter.Equals(NavigationPath3DLocValAfter);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                IWebElement curvedmprelement = brz3dvp.controlelement("Curved MPR");
                if (BlueColorValAfter_7 != BlueColorValAfter_5 && !result1 && result2 && CompareImage(result.steps[ExecutedSteps], curvedmprelement))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 08
                res = brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationtwo, objlocval3, "down");
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                {
                    res = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo).Equals(objlocval3);
                    if (!res)
                        result.steps[++ExecutedSteps].StepFail();
                    else
                        result.steps[++ExecutedSteps].StepPass();
                }

                //step 09
                Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                int BlueColorValBefore_9 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 21, 0, 0, 255, 2);
                //new Actions(Driver).MoveToElement(Navigation2, (Navigation2.Size.Width / 2) - 5, (Navigation2.Size.Height / 2) + 10).Click().Build().Perform();
                brz3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) - 5, (Navigation2.Size.Height / 2) + 10);
                PageLoadWait.WaitForFrameLoad(20);
                int BlueColorValAfter_9 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 22, 0, 0, 255, 2);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                curvedmprelement = brz3dvp.controlelement("Curved MPR");
                String MPRNavigationPathLocValAfter09 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                String NavigationPath3DLocValAfter09 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                result1 = MPRNavigationPathLocValAfter.Equals(MPRNavigationPathLocValAfter09) && NavigationPath3DLocValAfter.Equals(NavigationPath3DLocValAfter09);
                result2 = NavigationPath3DLocValAfter09.Equals(MPRNavigationPathLocValAfter09);
                if (BlueColorValBefore_9 != BlueColorValAfter_9 && !result1 && result2 && CompareImage(result.steps[ExecutedSteps], curvedmprelement))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 10
                Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                int BlueColorValBefore_10 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 21, 0, 0, 255, 2);
                brz3dvp.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2), (Navigation2.Size.Height / 2) - 10);
                PageLoadWait.WaitForFrameLoad(20);
                int BlueColorValAfter_10 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 22, 0, 0, 255, 2);
                if (BlueColorValBefore_10 != BlueColorValAfter_10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 11
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                PathNavigationMPR = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                String PathNavigationMPROreinetationBefore11 = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                System.Drawing.Point location = brz3dvp.ControllerPoints(BluRingZ3DViewerPage.MPRPathNavigation);
                Thread.Sleep(2000);
                int xcoordinate = location.X;
                int ycoordinate = location.Y;
                Cursor.Position = new System.Drawing.Point(xcoordinate + 20 , ycoordinate - 10);
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                PageLoadWait.WaitForFrameLoad(5);
                Cursor.Position = new System.Drawing.Point(xcoordinate + 20, ycoordinate + 50);
                Thread.Sleep(5000);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                Thread.Sleep(2000);
                String PathNavigationMPROreinetationAfter11 = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                bool res11 = PathNavigationMPROreinetationAfter11.Equals(PathNavigationMPROreinetationBefore11);
                if (CompareImage(result.steps[ExecutedSteps],brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR)))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 12
                PathNavigation3D = brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                String PathNavigation3DOreinetationBefore = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String PathNavigationMPROreinetationBefore = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                location = brz3dvp.ControllerPoints(BluRingZ3DViewerPage._3DPathNavigation);
                xcoordinate = location.X;
                ycoordinate = location.Y;
                Cursor.Position = new System.Drawing.Point(xcoordinate - 10, ycoordinate - 10);
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                PageLoadWait.WaitForFrameLoad(5);
                Cursor.Position = new System.Drawing.Point(xcoordinate - 10, ycoordinate + 100);
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                PageLoadWait.WaitForFrameLoad(10);
                String PathNavigation3DOreinetationAfter = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage._3DPathNavigation);
                String PathNavigationMPROreinetationAfter = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                result1 = PathNavigation3DOreinetationAfter.Equals(PathNavigation3DOreinetationBefore);
                result2 = PathNavigationMPROreinetationAfter.Equals(PathNavigationMPROreinetationBefore);
                if (!result1 && !result2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 13
                Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                int BlueColorValBefore_13 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 131, 0, 0, 255, 2);
                brz3dvp.CurvedPathdeletor(Navigation2, Navigation2.Size.Width / 2, (Navigation2.Size.Height / 2) - 10 ,"Delete Control Point", PostTool: Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                PageLoadWait.WaitForFrameLoad(10);
                int BlueColorValAfter_13 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 132, 0, 0, 255, 2);
                if(BlueColorValAfter_13 != BlueColorValBefore_13)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 14
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                Actions action = new Actions(Driver);
                action.MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 12, (Navigation1.Size.Height / 2) - 40).ClickAndHold()
                    .MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 9, (Navigation1.Size.Height / 2) - 40).Build().Perform();
                action.Release().Build().Perform();
                if (CompareImage(result.steps[ExecutedSteps],brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR)))
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();
                }

                //step 15 & 16
                Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                int BlueColorValBefore_15 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 151, 0, 0, 255, 2);
                if(browserName.ToLower().Contains("explorer") || browserName.ToLower().Contains("ie") || browserName.ToLower().Contains("mozilla") || browserName.ToLower().Contains("firefox"))
                    brz3dvp.CurvedPathdeletor(Navigation2, (Navigation2.Size.Width / 2) , (Navigation2.Size.Height / 2), "Delete Control Point", PostTool: Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                else
                    brz3dvp.CurvedPathdeletor(Navigation2, (Navigation2.Size.Width / 2) - 5, (Navigation2.Size.Height / 2) + 10, "Delete Control Point", PostTool: Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                PageLoadWait.WaitForFrameLoad(10);
                int BlueColorValAfter_15 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 152, 0, 0, 255, 2);
                if (BlueColorValAfter_15 != BlueColorValBefore_15)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }

                //step 17
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                String enabledbutton = "", disabledbutton = "";
                String DeleteOptions = Locators.CssSelector.ctrlpointdropdown + " " + Locators.CssSelector.ctrlpointoptions;
                if (Config.BrowserType.ToLower() == "mozilla" || Config.BrowserType.ToLower() == "firefox")
                {
                    Actions act17 = new Actions(Driver);
                    act17.MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 60).Build().Perform();
                    PageLoadWait.WaitForFrameLoad(2);
                    act17.ContextClick().Build().Perform();
                }
                else
                {
                    // new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 60).ContextClick().Build().Perform();
                    brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 60, Rclick: true);
                }
                wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.CssSelector(DeleteOptions)));
                IList<IWebElement> Options = Driver.FindElements(By.CssSelector(DeleteOptions));
                foreach (IWebElement option in Options)
                {
                    String ButtonText = option.Text;
                    if (option.Enabled)
                    {
                        enabledbutton = ButtonText;
                    }
                    else
                    {
                        disabledbutton = ButtonText;
                    }
                }
                if(enabledbutton.Equals("Delete Path") && disabledbutton.Equals("Delete Control Point"))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 18
                new Actions(Driver).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Thread.Sleep(5000);
                Accord.Point redblobvalue = brz3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 181);
                Thread.Sleep(5000);
                Accord.Point yellowblobvalue = brz3dvp.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 182, "yellow", "Vertical", 1);
                Thread.Sleep(5000);
                new Actions(Driver).MoveToElement(Navigation2, Convert.ToInt32(redblobvalue.X), Convert.ToInt32(redblobvalue.Y)).ClickAndHold()
                    .MoveToElement(Navigation2, Convert.ToInt32(yellowblobvalue.X), Convert.ToInt32(yellowblobvalue.Y)).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(5000);
               // brz3dvp.Performdragdrop(Navigation2, (Int32)redblobvalue.X, (Int32)redblobvalue.Y, (Int32)yellowblobvalue.X, (Int32)yellowblobvalue.Y);

                //PageLoadWait.WaitForFrameLoad(10);
                Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                new Actions(Driver).SendKeys("x").Build().Perform();
                res = brz3dvp.select3DTools(Z3DTools.Scrolling_Tool, BluRingZ3DViewerPage.Navigationthree);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                {
                    PageLoadWait.WaitForFrameLoad(10);
                    String LocationvalueBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                    res = brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationthree, scrolllevel : 10, Thickness: "n");
                    String LocationvalueAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                    if(LocationvalueAfter != LocationvalueBefore)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }

                //step 19
                brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5, BluRingZ3DViewerPage.Navigationthree);
                PageLoadWait.WaitForFrameLoad(10);
                Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                String MPRNavigationPathLocValBefore19 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                String NavigationPath3DLocValBefore19 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                int BlueColorValBefore_19 = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 192, 0, 0, 255, 2);
                //Actions act19 = new Actions(Driver);
                //act19.MoveToElement(Navigation3, (Navigation3.Size.Width / 2) + 5, (Navigation3.Size.Height / 2) + 25).Build().Perform();
                //PageLoadWait.WaitForFrameLoad(2);
                //act19.Click().Build().Perform();
                brz3dvp.MoveAndClick(Navigation3, (Navigation3.Size.Width / 2) + 5, (Navigation3.Size.Height / 2) + 25);
                //new Actions(Driver).MoveToElement(Navigation3, (Navigation3.Size.Width / 2) + 5, (Navigation3.Size.Height / 2) + 25).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(20);
                int BlueColorValAfter_19 = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 193, 0, 0, 255, 2);
                String MPRNavigationPathLocValAfter19 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.MPRPathNavigation);
                String NavigationPath3DLocValAfter19 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                result1 = MPRNavigationPathLocValBefore19.Equals(MPRNavigationPathLocValAfter19) && NavigationPath3DLocValBefore19.Equals(NavigationPath3DLocValAfter19);
                result2 = MPRNavigationPathLocValAfter19.Equals(NavigationPath3DLocValAfter19);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                curvedmprelement = brz3dvp.controlelement("Curved MPR");
                if ((BlueColorValBefore_9 != BlueColorValAfter_9) || (!result1 && result2 && CompareImage(result.steps[ExecutedSteps], curvedmprelement)))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 20
                int Nav1BlueColorBefore = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 201, 0, 0, 255, 2);
                int Nav2BlueColorBefore = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 202, 0, 0, 255, 2);
                int Nav3BlueColorBefore = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 203, 0, 0, 255, 2);
                brz3dvp.CurvedPathdeletor(Navigation3, (Navigation3.Size.Width / 2) + 5, (Navigation3.Size.Height / 2) + 25, "Delete Path", PostTool: Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                int Nav1BlueColorAfter = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 204, 0, 0, 255, 2);
                int Nav2BlueColorAfter = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 205, 0, 0, 255, 2);
                int Nav3BlueColorAfter = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 206, 0, 0, 255, 2);
                result1 = Nav1BlueColorAfter.Equals(Nav1BlueColorBefore);
                result2 = Nav2BlueColorBefore.Equals(Nav2BlueColorAfter);
                bool result3 = Nav3BlueColorAfter.Equals(Nav2BlueColorBefore);
                if(!result1 && !result2 && !result3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 21
                String Nav1LocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                String Nav2LocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                String Nav3LocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                String Nav1LocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                String Nav2LocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                String Nav3LocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                result1 = Nav1LocValBefore.Equals(Nav1LocValAfter);
                result2 = Nav2LocValBefore.Equals(Nav2LocValAfter);
                result3 = Nav3LocValBefore.Equals(Nav3LocValAfter);
                if (!result1 && !result2 && !result3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();


                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                brz3dvp.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163314(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            string PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string ThumbnailDescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] TestData = TestDataRequirements.Split('|');
            String ResetLoc = TestData[0];
            String ColonLoc = TestData[1];
            BluRingZ3DViewerPage Z3dViewerPage = new BluRingZ3DViewerPage();
            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //Step:1  - Series is loaded in the 3D viewer in Curved MPR viewing mode
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Boolean step1 = Z3dViewerPage.searchandopenstudyin3D(PatientID, ThumbnailDescription, BluRingZ3DViewerPage.CurvedMPR);
                if (step1)
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
                
                //step:2 - Curve drawing cursor shows up while hovering over the images
                Boolean step2 =  Z3dViewerPage.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                if(step2)
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

                //step:3 - Click on navigation control 1 and scroll up until the top of the colon is visible. 
                Z3dViewerPage.EnterThickness(BluRingZ3DViewerPage.Navigationone, "1");
                IWebElement ViewerContainer = Z3dViewerPage.ViewerContainer();
                IWebElement Navigation1 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                System.Drawing.Point location = Z3dViewerPage.ControllerPoints(BluRingZ3DViewerPage.Navigationone);
                int xcoordinate = location.X;
                int ycoordinate = location.Y;
                Cursor.Position = new System.Drawing.Point(xcoordinate + 48, ycoordinate + 48);
                Thread.Sleep(1000);
                bool res3 = Z3dViewerPage.ScrollInView(BluRingZ3DViewerPage.Navigationone, ColonLoc, scrolllevel: 60);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (res3 && CompareImage(result.steps[ExecutedSteps], Navigation1))
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

                //step:4 - Add a point at the top of the colon displayed on navigation control 1.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                String goldimage = result.steps[ExecutedSteps].goldimagepath;
                String testimage = result.steps[ExecutedSteps].testimagepath;
                new Actions(Driver).MoveToElement(Navigation1).SendKeys("x").Build().Perform();
                DownloadImageFile(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), goldimage);
                int BluColorBeforePoint = Z3dViewerPage.selectedcolorcheck(goldimage, 0, 0, 255, 1);
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2, ((Navigation1.Size.Height / 4) * 3));
                DownloadImageFile(Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone), testimage);
                int BluColorAfterPoint1 = Z3dViewerPage.selectedcolorcheck(testimage, 0, 0, 255, 1);
                BluColorAfterPoint1 = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 41, 0, 0, 255, 2);
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

                //Step5:: Add a 2nd point along the aorta displayed on navigation control 1.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                testimage = result.steps[ExecutedSteps].testimagepath;
                Z3dViewerPage.MoveAndClick(Navigation1, Navigation1.Size.Width / 2, ((Navigation1.Size.Height / 4) * 3) + 30);
                Thread.Sleep(10000);
                Boolean checkissue = Z3dViewerPage.checkerrormsg();
                if (checkissue)
                    throw new Exception("Failed to find path");                
                int BluColorAfterPoint2 = Z3dViewerPage.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 41, 0, 0, 255, 2);
                if (BluColorAfterPoint2 != BluColorAfterPoint1)
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

                //Steps 6::Select the scroll tool from the 3D tool box and click on navigation control 2
                IWebElement Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Navigation1).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                Z3dViewerPage.MoveClickAndHold(Navigation1, Navigation1.Size.Width / 2, ((Navigation1.Size.Height / 4) * 3) + 30);
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.checkerrormsg();
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], Navigation2))
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

                //step:7  -Add a 3rd point along the lower range of the colon displayed on navigation control 2
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                Z3dViewerPage.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 30, ((Navigation2.Size.Height / 4) * 3) + 50);
                PageLoadWait.WaitForFrameLoad(10);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer, pixelTolerance:300))
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

                //step:8 - Try to add a 4th point on navigation control 2, but this time click a location along the path already created between the 3rd and 2nd points.
                Z3dViewerPage.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 40, ((Navigation2.Size.Height / 4) * 3) + 40);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer, pixelTolerance:300))
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

                //step:9  -Click and hold Right+left mouse button. Drag on the image in MPR navigation controls and Curved MPR control.
                List<string> step9_before = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                location = Z3dViewerPage.ControllerPoints(BluRingZ3DViewerPage.Navigationone);
                xcoordinate = location.X;
                ycoordinate = location.Y;
                Cursor.Position = new System.Drawing.Point(xcoordinate + 10, ycoordinate - 10);
                BasePage.mouse_event(0x00000002, 0, 0, 0, 0);
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(3000);
                Cursor.Position = new System.Drawing.Point(xcoordinate + 10, ycoordinate + 30);
                Thread.Sleep(2000);
                BasePage.mouse_event(0x00000004, 0, 0, 0, 0);
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                PageLoadWait.WaitForFrameLoad(5);
                Thread.Sleep(5000);
                List<string> step9_after = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 3);
                if(step9_before[0] != step9_after[0] && step9_before[1] != step9_after[1] && step9_before[3] != step9_after[3] && step9_before[4] != step9_after[4] && step9_before[5] != step9_after[5])
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

                //step:10 - Click and hold Right+left mouse button. Drag on the image in MPR path navigation control
                List<string> step10_before = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                Thread.Sleep(5000);
                location = Z3dViewerPage.ControllerPoints(BluRingZ3DViewerPage.MPRPathNavigation);
                xcoordinate = location.X;
                ycoordinate = location.Y;
                Thread.Sleep(3000);
                for (int i = 0; i < 25; i++)
                {
                    Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 10);
                    BasePage.mouse_event(0x00000002, 0, 0, 0, 0);
                    BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                    Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate + 90);
                    PageLoadWait.WaitForFrameLoad(5);
                    Thread.Sleep(2000);
                    BasePage.mouse_event(0x00000004, 0, 0, 0, 0);
                    BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                    PageLoadWait.WaitForFrameLoad(5);
                }
                List<string> step10_after = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (step10_before[2] != step10_after[2] && step10_before[4] != step10_after[4])
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

                //step:11 - Click and hold Right mouse button. Drag on the image in MPR navigation controls, MPR path navigation and Curved MPR control.
                Thread.Sleep(5000);
                String BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                String AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                IWebElement CurvedMPR = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                //navigation 1
                location = Z3dViewerPage.ControllerPoints(BluRingZ3DViewerPage.Navigationone);
                xcoordinate = location.X;
                ycoordinate = location.Y;
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 50);
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 60);
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                //navigation 2
                location = Z3dViewerPage.ControllerPoints(BluRingZ3DViewerPage.Navigationtwo);
                xcoordinate = location.X;
                ycoordinate = location.Y;
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 50);
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                Thread.Sleep(2000);
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 60);
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                //navigation 3
                location = Z3dViewerPage.ControllerPoints(BluRingZ3DViewerPage.Navigationthree);
                xcoordinate = location.X;
                ycoordinate = location.Y;
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 50);
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                Thread.Sleep(2000);
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 60);
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                Thread.Sleep(5000);
                //MPR Path Navigation
                location = Z3dViewerPage.ControllerPoints(BluRingZ3DViewerPage.MPRPathNavigation);
                xcoordinate = location.X;
                ycoordinate = location.Y;
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 50);
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                Thread.Sleep(3000);
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 60);
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], CurvedMPR, pixelTolerance:300))
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

                //step:12 - Click and hold Right mouse button. Drag on the image in 3D path navigation controls.
                IWebElement Path3DNavigation = Z3dViewerPage.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                BeforeImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_Before.jpg";
                AfterImagePath = Config.downloadpath + "\\" + testid + ExecutedSteps + "_After.jpg";
                DownloadImageFile(Path3DNavigation, BeforeImagePath);
                new Actions(Driver).MoveToElement(Path3DNavigation).Build().Perform();
                Thread.Sleep(2000);
                location = Z3dViewerPage.ControllerPoints(BluRingZ3DViewerPage._3DPathNavigation);
                xcoordinate = location.X;
                ycoordinate = location.Y;
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 50);
                BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                Thread.Sleep(2000);
                Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate + 50);
                BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                PageLoadWait.WaitForFrameLoad(5);
                DownloadImageFile(Path3DNavigation, AfterImagePath);
                if (!CompareImage(BeforeImagePath, AfterImagePath))
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

                //step:13 - Click and hold Right+left mouse button. Drag on the image in 3D path navigation controls
                List<string> step13_before = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                location = Z3dViewerPage.ControllerPoints(BluRingZ3DViewerPage._3DPathNavigation);
                xcoordinate = location.X;
                ycoordinate = location.Y;
                for (int i = 0; i < 25; i++)
                {
                    Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate - 50);
                    BasePage.mouse_event(0x00000002, 0, 0, 0, 0);
                    BasePage.mouse_event(0x00000008, 0, 0, 0, 0);
                    Thread.Sleep(2000);
                    Cursor.Position = new System.Drawing.Point(xcoordinate + 50, ycoordinate + 50);
                    Thread.Sleep(2000);
                    BasePage.mouse_event(0x00000004, 0, 0, 0, 0);
                    BasePage.mouse_event(0x00000010, 0, 0, 0, 0);
                    Thread.Sleep(2000);
                }
                List<string> step13_after = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (step13_before[2] != step13_after[2] && step13_before[4] != step13_after[4])
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

                //step:14 - Right click at the 4th point and select the "Delete Control Point" option
                Z3dViewerPage.CurvedPathdeletor(Navigation2, (Navigation2.Size.Width / 2) + 40, ((Navigation2.Size.Height / 4) * 3) + 40, "Delete Control Point", PostTool: Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer, pixelTolerance:300))
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

                //step:15 - Try to modify the location of the 2nd point slightly to the left
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Thread.Sleep(3000);
                new Actions(Driver).MoveToElement(Navigation2, (Navigation2.Size.Width / 2) + 40, ((Navigation2.Size.Height / 4) * 3) + 35).Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(Navigation2, (Navigation2.Size.Width / 2) + 40, ((Navigation2.Size.Height / 4) * 3) + 35)
                    .ClickAndHold().MoveToElement(Navigation2, (Navigation2.Size.Width / 2) + 45, ((Navigation2.Size.Height / 4) * 3) + 35);
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer, pixelTolerance:300))
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

                //step:16 - Right click on the 3rd point added
                if (browserName.Contains("mozilla") || browserName.Contains("firefox"))
                {
                    Z3dViewerPage.select3DTools(Z3DTools.Scrolling_Tool);
                    Thread.Sleep(1500);
                    Actions act = new Actions(Driver);
                    Z3dViewerPage.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 30, ((Navigation2.Size.Height / 4) * 3) + 50);
                }
                Z3dViewerPage.MoveAndClick(Navigation2, (Navigation2.Size.Width / 2) + 30, ((Navigation2.Size.Height / 4) * 3) + 50, Rclick: true);
                PageLoadWait.WaitForFrameLoad(2);
                IWebElement CtrlPointdropdown = Driver.FindElement(By.CssSelector(Locators.CssSelector.ctrlpointdropdown));
                IList<IWebElement> CtrlPointopt = CtrlPointdropdown.FindElements(By.CssSelector(Locators.CssSelector.ctrlpointoptions));
                if (CtrlPointopt[0].Text == "Delete Path" && CtrlPointopt[1].Text == "Delete Control Point")
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

                //step:17 - Select the 'Delete Control Point' option
                CtrlPointopt[1].Click();
                PageLoadWait.WaitForFrameLoad(5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer, pixelTolerance: 300))
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

                //step:18 - Add a 3rd point towards the end of the colon by clicking on “Navigation 3” image this time.
                Navigation2 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                Thread.Sleep(3000);
                Accord.Point redposition = Z3dViewerPage.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 181, "red");
                PageLoadWait.WaitForFrameLoad(10);
                Accord.Point yellowposition = Z3dViewerPage.GetIntersectionPoints(Navigation2, testid, ExecutedSteps + 182, "yellow", "vertical");
                new Actions(Driver).MoveToElement(Navigation2, (Int32)redposition.X, (Int32)redposition.Y).Build().Perform();
                Thread.Sleep(2000);
                new Actions(Driver).MoveToElement(Navigation2, (Int32)redposition.X, (Int32)redposition.Y).ClickAndHold()
                    .MoveToElement(Navigation2, (Int32)yellowposition.X, (Int32)yellowposition.Y).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Navigation2).SendKeys("x").Build().Perform();
                Thread.Sleep(3000);
                Z3dViewerPage.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                IWebElement Navigation3 = Z3dViewerPage.controlelement(BluRingZ3DViewerPage.Navigationthree);
                Z3dViewerPage.MoveAndClick(Navigation3, Navigation3.Size.Width / 2, Navigation3.Size.Height - 40);
                bool res18 = Z3dViewerPage.checkerrormsg();
                if (res18)
                    throw new Exception("Failed to find path");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer, pixelTolerance: 300))
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

                //step:19 - Right click on the path on any of the navigation controls and select the "Delete Path" option from the drop down list.
                Z3dViewerPage.CurvedPathdeletor(Navigation3, Navigation3.Size.Width / 2, Navigation3.Size.Height - 40, "Delete Path", PostTool: Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                if (CompareImage(result.steps[ExecutedSteps], ViewerContainer, pixelTolerance: 300))
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

                //step:20 -  	Select the Reset button from the Z3D tool box.
                Z3dViewerPage.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                List<string> step17 = Z3dViewerPage.GetAttributes_Result(Locators.CssSelector.LeftTopPane, null, null, 2);
                if (step17[0] == ResetLoc && step17[1] == ResetLoc && step17[2] == ResetLoc && step17[3] == ResetLoc && step17[4] == ResetLoc && step17[5] == ResetLoc)
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
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);
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
            }
        }

        public TestCaseResult Test_163315(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
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
                String objthumbimg = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
                String objTestRequirement = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
                String objlocval1 = objTestRequirement.Split('|')[0];
                String objlocval2 = objTestRequirement.Split('|')[1];
                String objlocval3 = objTestRequirement.Split('|')[2];
                String objlocval4 = objTestRequirement.Split('|')[3];
                String objlocval5 = objTestRequirement.Split('|')[4];
                String objlocval6 = objTestRequirement.Split('|')[5];
                String BaseImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "BaseImages" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(BaseImages);
                String ColorSplitImages = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "Test Result" + Path.DirectorySeparatorChar + "TempImages" + Path.DirectorySeparatorChar + "ColorSplitted" + Path.DirectorySeparatorChar + Config.BrowserType;
                Directory.CreateDirectory(ColorSplitImages);
                IWebElement Navigation1, Navigation2,  Navigation3;
                String baseimagepath, colorsplittedpath;

                //step 01 & 02
                login.LoginIConnect(adminUserName, adminPassword);
                bool res = brz3dvp.searchandopenstudyin3D(objpatid, objthumbimg, layout: BluRingZ3DViewerPage.CurvedMPR);
                if (!res)
                    throw new Exception("unable to launch study in 3D Viewer");
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                
                //step 03
                IWebElement curvedmprelement = brz3dvp.controlelement("Curved MPR");
                String LocationvalueBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, objlocval1, ScrollDirection: "up", scrolllevel: 34, Thickness: "y");
                PageLoadWait.WaitForFrameLoad(5);
                String LocationvalueAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Navigation1).SendKeys("x").Build().Perform();
                int BlueColorValBefore = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 41, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 60).Click().Build().Perform();
                //brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 23, (Navigation1.Size.Height / 4) - 60);
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(Navigation1).Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int BlueColorValAfter_4 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 42, 0, 0, 255, 2, isMoveCursor: true);
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                //new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20).Click().Build().Perform();
                brz3dvp.MoveAndClick(Navigation1, (Navigation1.Size.Width / 2) + 15, (Navigation1.Size.Height / 4) - 20);
                PageLoadWait.WaitForFrameLoad(10);
                int BlueColorValAfter_5 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 1, 0, 0, 255, 2);
                curvedmprelement = brz3dvp.controlelement("Curved MPR");
                if (LocationvalueAfter != LocationvalueBefore && BlueColorValAfter_4 != BlueColorValBefore && BlueColorValAfter_5 != BlueColorValAfter_4 && CompareImage(result.steps[ExecutedSteps], curvedmprelement))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 04
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                String OrientationValueBefore = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationone);
                new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2, 3 * (Navigation1.Size.Height / 4)).ClickAndHold().
                    MoveToElement(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                String OrientationValueAfter = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationone);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                IWebElement viewport = Driver.FindElement(By.CssSelector(Locators.CssSelector.wholepanel));
                if (OrientationValueBefore != OrientationValueAfter && CompareImage(result.steps[ExecutedSteps], viewport))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 05
                String Nav1LocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                String Nav2LocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                String Nav3LocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                brz3dvp.select3DTools(Z3DTools.Reset,BluRingZ3DViewerPage.CurvedMPR);
                Thread.Sleep(20000);
                String Nav1LocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                String Nav2LocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                String Nav3LocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                bool result1 = Nav1LocValBefore.Equals(Nav1LocValAfter);
                bool result2 = Nav3LocValAfter.Equals(Nav2LocValBefore);
                bool result3 = Nav3LocValAfter.Equals(Nav3LocValBefore);
                if (!result1 && !result2 && !result3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 06
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                if(!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 07
                LocationvalueBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationtwo, objlocval2, ScrollDirection: "down");
                PageLoadWait.WaitForFrameLoad(5);
                Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                BlueColorValBefore = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 111, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(5);
                //new Actions(Driver).MoveToElement(Navigation2, (Navigation2.Size.Width / 2) + 40, (Navigation2.Size.Height / 4) - 60).Click().Build().Perform();
                brz3dvp.MoveAndClick(Navigation2, Navigation2.Size.Width / 2 + 25, 60);
                PageLoadWait.WaitForFrameLoad(20);
                BlueColorValAfter_4 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 112, 0, 0, 255, 2, isMoveCursor: true);
                if (BlueColorValAfter_4 != BlueColorValBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 08
                Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                OrientationValueBefore = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationtwo);
                new Actions(Driver).MoveToElement(Navigation2, Navigation2.Size.Width / 2, 3 * (Navigation2.Size.Height / 4)).ClickAndHold()
                .MoveToElement(Navigation2, Navigation2.Size.Width / 2, Navigation2.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                OrientationValueAfter = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationtwo);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                viewport = Driver.FindElement(By.CssSelector(Locators.CssSelector.wholepanel));
                if (OrientationValueBefore != OrientationValueAfter && CompareImage(result.steps[ExecutedSteps], viewport))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 09
                Nav1LocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Nav2LocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                Nav3LocValBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                Thread.Sleep(20000);
                Nav1LocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Nav2LocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                Nav3LocValAfter = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                result1 = Nav1LocValBefore.Equals(Nav1LocValAfter);
                result2 = Nav3LocValAfter.Equals(Nav2LocValBefore);
                result3 = Nav3LocValAfter.Equals(Nav3LocValBefore);
                if (!result1 && !result2 && !result3)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 10
                res = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                if (!res)
                    result.steps[++ExecutedSteps].StepFail();
                else
                    result.steps[++ExecutedSteps].StepPass();

                //step 11
                LocationvalueBefore = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                BlueColorValBefore = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 191, 0, 0, 255, 2);
                PageLoadWait.WaitForFrameLoad(5);
                //new Actions(Driver).MoveToElement(Navigation3, (Navigation3.Size.Width / 4), (Navigation3.Size.Height / 2) - 40).Click().Build().Perform();
                brz3dvp.MoveAndClick(Navigation3, (Navigation3.Size.Width / 4), (Navigation3.Size.Height / 2) - 40);
                PageLoadWait.WaitForFrameLoad(20);
                BlueColorValAfter_4 = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 192, 0, 0, 255, 2, isMoveCursor: true);
                if (BlueColorValAfter_4 != BlueColorValBefore)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 12
                Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                OrientationValueBefore = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationthree);
                new Actions(Driver).MoveToElement(Navigation3, Navigation3.Size.Width / 2, 3 * (Navigation3.Size.Height / 4)).ClickAndHold()
                .MoveToElement(Navigation3, Navigation3.Size.Width / 2, Navigation3.Size.Height / 4).Release().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                OrientationValueAfter = brz3dvp.GetOrientationValue(BluRingZ3DViewerPage.Navigationthree);
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                viewport = Driver.FindElement(By.CssSelector(Locators.CssSelector.wholepanel));
                if (OrientationValueBefore != OrientationValueAfter && CompareImage(result.steps[ExecutedSteps], viewport))
                    result.steps[ExecutedSteps].StepPass();
                else
                    result.steps[ExecutedSteps].StepFail();

                //step 13
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                int Nav1BCValBef13 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 1311, 0, 0, 255, 2);
                Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                int Nav2BCValBef13 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 1312, 0, 0, 255, 2);
                Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                int Nav3BCValBef13 = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 1313, 0, 0, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage.CurvedMPR);
                Thread.Sleep(20000);
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                int Nav1BCValAft13 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 1314, 0, 0, 255, 2);
                Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                int Nav2BCValAft13 = brz3dvp.LevelOfSelectedColor(Navigation2, testid, ExecutedSteps + 1315, 0, 0, 255, 2);
                Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                int Nav3BCValAft13 = brz3dvp.LevelOfSelectedColor(Navigation3, testid, ExecutedSteps + 1316, 0, 0, 255, 2);
                if (Nav1BCValAft13 != Nav1BCValBef13  && Nav2BCValAft13 != Nav2BCValBef13  && Nav3BCValAft13 != Nav3BCValBef13)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                brz3dvp.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163313(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables 
            TestCaseResult result;
            DomainManagement domainmgmt;
            UserManagement usermgmt;
            BasePage basepage = new BasePage();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
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
                String objthumbimg = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
                String objTestRequirement = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
                String objlocval1 = objTestRequirement.Split('|')[0];
                String objlocval2 = objTestRequirement.Split('|')[1];
                //String objlocval3 = objTestRequirement.Split('|')[2];

                //step 01 :: Search and load a 3D supported study in the universal viewer.
                //Steps 2 :: Select a 3D supported series and Select the Curved MPR option from the smart view drop down.
                login.LoginIConnect(adminUserName, adminPassword);
                bool StudyLoad = brz3dvp.searchandopenstudyin3D(objpatid, objthumbimg, layout: BluRingZ3DViewerPage.CurvedMPR);
                if (StudyLoad)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    throw new Exception("unable to launch study in 3D Viewer");
                }
                
                //Steps 3 :: Right click on the navigation control 1, then right click on the Curved drawing tool and select the Auto vessel option from the drop down. 
                IWebElement navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                PageLoadWait.WaitForFrameLoad(10);
                new Actions(Driver).MoveToElement(navigation1).SendKeys("x").Build().Perform();
                bool CurveDrawingAutoVissel = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                if (CurveDrawingAutoVissel)
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
                //Steps 4 :: Click on navigation control 1 and scroll up until the top of the aorta is visible.
                String InitialLocation = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, objlocval1,scrolllevel:34);
                PageLoadWait.WaitForFrameLoad(10);
                String LocationAfterScroll = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                if (LocationAfterScroll == objlocval1 && LocationAfterScroll != InitialLocation)
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
                // Steps 5 :: Add a point at the top of the aorta displayed on navigation control 1.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int ColorValBefore_5 = brz3dvp.LevelOfSelectedColor(navigation1, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                //brz3dvp.MoveAndClick(navigation1, (navigation1.Size.Width / 2) + 18, (navigation1.Size.Height / 4) - 10);
                new Actions(Driver).MoveToElement(navigation1, (navigation1.Size.Width / 2) + 18, (navigation1.Size.Height / 4) - 10).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_5 = brz3dvp.LevelOfSelectedColor(navigation1, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                if (ColorValBefore_5 != ColorValAfter_5)
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
                //Steps 6 :: Add a second point outside of the aorta so that the curve drawing tool fails to find a path.
                brz3dvp.MoveAndClick(navigation1, (navigation1.Size.Width / 2), (navigation1.Size.Height / 2));
                //new Actions(Driver).MoveToElement(navigation1, (navigation1.Size.Width / 2), (navigation1.Size.Height / 2)).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                //Verification::"Failed to find path" error message should displayed..
                bool Navigation1ErrMsg = brz3dvp.checkerrormsg("y");
                if (Navigation1ErrMsg)
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
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                //Steps 7 :: Repeat steps 3-6 for MPR navigation controls 2 and 3.
                //Navigation2
                IWebElement navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                PageLoadWait.WaitForFrameLoad(10);
                bool Nav2CurveDrawingAutoVissel = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                if(Config.BrowserType.ToLower()=="mozilla" || Config.BrowserType.ToLower()=="firefox")
                    new Actions(Driver).MoveToElement(navigation2, (navigation2.Size.Width / 2) + 35, (navigation2.Size.Height / 4) - 10).Click().Build().Perform();
                else
                    brz3dvp.MoveAndClick(navigation2, (navigation2.Size.Width / 2) + 35, (navigation2.Size.Height / 4) - 10);
                
                PageLoadWait.WaitForFrameLoad(10);
                if (Config.BrowserType.ToLower() == "mozilla" || Config.BrowserType.ToLower() == "firefox")
                    new Actions(Driver).MoveToElement(navigation2, (navigation2.Size.Width / 2), (navigation2.Size.Height / 2)).Click().Build().Perform();
                else
                    brz3dvp.MoveAndClick(navigation2, (navigation2.Size.Width / 2), (navigation2.Size.Height / 2));
                
                PageLoadWait.WaitForFrameLoad(10);

                //Verification::"Failed to find path" error message should displayed..
                bool Navigation2ErrMsg = brz3dvp.checkerrormsg("y");
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);

                //Navigation3
                IWebElement navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                PageLoadWait.WaitForFrameLoad(10);
                bool Nav3CurveDrawingAutoVissel = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Vessels5);
                if (Config.BrowserType.ToLower() == "mozilla" || Config.BrowserType.ToLower() == "firefox")
                    new Actions(Driver).MoveToElement(navigation3, (navigation3.Size.Width / 2), (navigation3.Size.Height / 2) + 25).Click().Build().Perform();
                else
                    brz3dvp.MoveAndClick(navigation3, (navigation3.Size.Width / 2), (navigation3.Size.Height / 2) + 25);
                
                PageLoadWait.WaitForFrameLoad(10);
                if (Config.BrowserType.ToLower() == "mozilla" || Config.BrowserType.ToLower() == "firefox")
                    new Actions(Driver).MoveToElement(navigation3, (navigation3.Size.Width / 2), (navigation3.Size.Height / 2)).Click().Build().Perform();
                else
                    brz3dvp.MoveAndClick(navigation3, (navigation3.Size.Width / 2), (navigation3.Size.Height / 2));
                PageLoadWait.WaitForFrameLoad(10);
                
                //Verification::"Failed to find path" error message should displayed..
                bool Navigation3ErrMsg = brz3dvp.checkerrormsg("y");
                PageLoadWait.WaitForFrameLoad(10);
                if (Nav2CurveDrawingAutoVissel && Nav3CurveDrawingAutoVissel && Navigation2ErrMsg && Navigation3ErrMsg)
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
                //Steps 8 :: Select the reset button on the 3D tool box.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int ColorValBefore_8 = brz3dvp.LevelOfSelectedColor(navigation3, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                bool Reset = brz3dvp.select3DTools(Z3DTools.Reset, BluRingZ3DViewerPage._3DPathNavigation);
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_8 = brz3dvp.LevelOfSelectedColor(navigation3, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                if (ColorValBefore_8 != ColorValAfter_8)
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
                //Steps 9::Right click on the navigation control 1, then right click on the Curved drawing tool and select the Auto colon option from the drop down.
                bool CurveDrawingtoolColon = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                if (CurveDrawingtoolColon)
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
                //Steps 10 ::Click on navigation control 1 and scroll up until the top of the Colon is visible.
                InitialLocation = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                bool res10 = brz3dvp.ScrollInView(BluRingZ3DViewerPage.Navigationone, objlocval2, scrolllevel: 60);
                PageLoadWait.WaitForFrameLoad(10);
                if (res10)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                //LocationAfterScroll = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                //if (LocationAfterScroll == objlocval2 && LocationAfterScroll != InitialLocation)
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
                //Steps 11 :: Add a point at the top of the colon displayed on navigation control 1.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int ColorValBefore_11 = brz3dvp.LevelOfSelectedColor(navigation3, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                if(Config.BrowserType.ToLower()=="mozilla" || Config.BrowserType.ToLower()=="firefox")
                    new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2 + 7, navigation1.Size.Height / 2 + 91).Click().Build().Perform();
                else
                    brz3dvp.MoveAndClick(navigation1, navigation1.Size.Width / 2 + 7, navigation1.Size.Height / 2 + 91);
                
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_11 = brz3dvp.LevelOfSelectedColor(navigation3, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                if (ColorValBefore_11 != ColorValAfter_11)
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
                //Steps 12 ::Add a second point outside of the aorta so that the curve drawing tool fails to find a path.
                brz3dvp.MoveAndClick(navigation1, navigation1.Size.Width / 2, navigation1.Size.Height / 2);
                //new Actions(Driver).MoveToElement(navigation1, navigation1.Size.Width / 2, navigation1.Size.Height / 2).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(10);
                bool ColonNavigation1ErrMsg = brz3dvp.checkerrormsg("y");
                if (ColonNavigation1ErrMsg)
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
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);

                //Steps 13 :: Repeat steps 9-12 for MPR navigation controls 2 and 3.
                //Navigation2
                navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                PageLoadWait.WaitForFrameLoad(10);
                bool Nav2CurveDrawingAutoColon = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                if (Config.BrowserType.ToLower() == "mozilla" || Config.BrowserType.ToLower() == "firefox")
                    new Actions(Driver).MoveToElement(navigation2, (navigation2.Size.Width / 2), (navigation2.Size.Height / 2) + 72).Click().Build().Perform();
                else
                brz3dvp.MoveAndClick(navigation2, (navigation2.Size.Width / 2), (navigation2.Size.Height / 2) + 72);
                
                PageLoadWait.WaitForFrameLoad(10);
                if (Config.BrowserType.ToLower() == "mozilla" || Config.BrowserType.ToLower() == "firefox")
                    new Actions(Driver).MoveToElement(navigation2, (navigation2.Size.Width / 2), (navigation2.Size.Height / 2)).Click().Build().Perform();
                else
                brz3dvp.MoveAndClick(navigation2, (navigation2.Size.Width / 2), (navigation2.Size.Height / 2));
                PageLoadWait.WaitForFrameLoad(10);

                //Verification::"Failed to find path" error message should displayed..
                Navigation2ErrMsg = brz3dvp.checkerrormsg("y");
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                
                //Navigation3
                navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                PageLoadWait.WaitForFrameLoad(10);
                bool Nav3CurveDrawingAutoColon = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Auto_2Colon5);
                if (Config.BrowserType.ToLower() == "mozilla" || Config.BrowserType.ToLower() == "firefox")
                    new Actions(Driver).MoveToElement(navigation3, ((navigation3.Size.Width / 2) - 50), (navigation3.Size.Height / 2) - 30).Click().Build().Perform();
                else
                brz3dvp.MoveAndClick(navigation3, ((navigation3.Size.Width / 2) - 50), (navigation3.Size.Height / 2) - 30);
                
                PageLoadWait.WaitForFrameLoad(10);
                if (Config.BrowserType.ToLower() == "mozilla" || Config.BrowserType.ToLower() == "firefox")
                    new Actions(Driver).MoveToElement(navigation3, (navigation3.Size.Width / 2), (navigation3.Size.Height / 2)).Click().Build().Perform();
                else
                brz3dvp.MoveAndClick(navigation3, (navigation3.Size.Width / 2), (navigation3.Size.Height / 2));
                PageLoadWait.WaitForFrameLoad(10);

                //Verification::"Failed to find path" error message should displayed..
                Navigation3ErrMsg = brz3dvp.checkerrormsg("y");
                PageLoadWait.WaitForFrameLoad(10);
                if (Nav2CurveDrawingAutoColon && Nav3CurveDrawingAutoColon && Navigation2ErrMsg && Navigation3ErrMsg)
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
                //Steps 14 :: Select the reset button on the 3D tool box.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                int ColorValBefore_14 = brz3dvp.LevelOfSelectedColor(navigation3, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);
                int ColorValAfter_14 = brz3dvp.LevelOfSelectedColor(navigation3, testid, ExecutedSteps + 4, 0, 0, 255, 2);
                if (ColorValBefore_14 != ColorValAfter_14)
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
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
                return result;
            }
            catch (Exception e)
            {
                //Log exception
                Logger.Instance.ErrorLog(e.StackTrace + e.Message + e.InnerException);

                //Report Result
                result.FinalResult(e, ExecutedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                brz3dvp.CloseViewer();
                login.Logout();
            }
        }
    }
}