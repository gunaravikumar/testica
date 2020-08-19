using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Selenium.Scripts.Pages;
using System.IO;
using Selenium.Scripts.Reusable.Generic;
using Selenium.Scripts.Pages.iConnect;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using Selenium.Scripts.Pages.iConnect;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Scripts.Tests
{
    class Workflow_3D : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public Cursor Cursor { get; private set; }
        public Imager imager = new Imager();
        DirectoryInfo CurrentDir = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
        public Workflow_3D(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }
        public TestCaseResult Test_164844(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage z3dvp = new BluRingZ3DViewerPage();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string thumbnailcaption = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string[] thumbnailSearchvalue = thumbnailcaption.Split('|');
            string TestDataRequirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            string[] searchvalue = TestDataRequirements.Split('|');

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
               
                //STEP 01 :: From ICA server open WebAccessConfiguration.xml search for 'ThreeDViewSopClasses', and confirm SOP Class UIDs are enabled in the file 
                Dictionary<String, String> ThreeDViewSopClasses = new Dictionary<String, String>();
                ThreeDViewSopClasses.Add("1.2.840.10008.5.1.4.1.1.2", "CT");
                ThreeDViewSopClasses.Add("1.2.840.10008.5.1.4.1.1.4", "MR");
                ThreeDViewSopClasses.Add("1.2.840.10008.5.1.4.1.1.128", "PT");
                int a = 0;
                try
                {
                    XDocument xdoc = XDocument.Load(Config.FileLocationPath);
                    foreach (var element in ThreeDViewSopClasses)
                    {
                        string find = "//ThreeDViewSopClasses//sopClass[@uid=" + "'" + element.Key + "'" + "]";
                        string childName = xdoc.XPathSelectElement(find).Attribute("usualModalities").Value;
                        if (childName.Contains(element.Value))
                            a++;
                        else
                        {
                            Logger.Instance.ErrorLog(element.Key + " SOP Class UID not enabled.");
                            break;
                        }
                    }
                }
                catch(Exception e)
                {
                    Logger.Instance.ErrorLog("Error while verify SOP Class UID in server : "+e.Message);
                }
                if (ThreeDViewSopClasses.Count.Equals(a))
                {
                    result.steps[++ExecutedSteps].StepPass("SOP Class UID already enabled & Remaining Pre-conditions defaultly configured in server.");
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 01<--");

                //STEP 02 :: Login iCA as Administrator &  Select a 3D supported series and select MPR option from the smart view drop down.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                bool step2 = z3dvp.searchandopenstudyin3D(Patientid, thumbnailSearchvalue[0]);
                if (step2)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 02<--");

                //STEP 03 :: Select each viewing modes and verify
                Dictionary<string, List<String>> map = new Dictionary<string, List<String>>();
                map.Add(BluRingZ3DViewerPage.MPR, new List<string>() { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.ResultPanel });
                map.Add(BluRingZ3DViewerPage.CurvedMPR, new List<string>() { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.MPRPathNavigation, BluRingZ3DViewerPage._3DPathNavigation, BluRingZ3DViewerPage.CurvedMPR });
                map.Add(BluRingZ3DViewerPage.CalciumScoring, new List<string>() { BluRingZ3DViewerPage.CalciumScoring });
                map.Add(BluRingZ3DViewerPage.Three_3d_4, new List<string>() { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.Navigation3D1 });
                map.Add(BluRingZ3DViewerPage.Three_3d_6, new List<string>() { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Navigation3D2 });              
                bool ResultValue = false;
                int Count = 0;
                foreach (KeyValuePair<string, List<string>> kvp in map)
                {
                    bool layout = z3dvp.select3dlayout(kvp.Key);
                    PageLoadWait.WaitForFrameLoad(5);
                    if (layout)
                    {
                        Logger.Instance.InfoLog(kvp.Key + " Viewer displayed successfully.");
                        foreach (string value in kvp.Value)
                        {
                            try
                            {
                                ResultValue = z3dvp.controlelement(value).Text.Contains(value);
                                if (!ResultValue)
                                {
                                    Logger.Instance.ErrorLog(kvp.Key + " Viewer not contain " + value + " Viewport");
                                    break;
                                }
                            }
                            catch (Exception e)
                            {
                                ResultValue = false;
                                Logger.Instance.ErrorLog(kvp.Key + " Viewer not contain " + value + " Viewport : "+e.Message);
                                break;
                            }
                            Logger.Instance.InfoLog(kvp.Key + " Viewer contain " + value +" Viewport");
                        }
                        if (!ResultValue)
                            break;
                        Count++;
                    }
                }
                
                if (map.Count.Equals(Count))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 03<--");

                //STEP 04 :: Hover the mouse cursor over the MPR/ 3D controls in each modes
                bool MPR = false;
                z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR);
                bool step4_1 =  z3dvp.verifyHoverBarOptions(BluRingZ3DViewerPage.Navigationone, new string[] { BluRingZ3DViewerPage.UndoSegmentation, BluRingZ3DViewerPage.RedoSegmentation, BluRingZ3DViewerPage.SaveIamge , BluRingZ3DViewerPage.SubVolumes , BluRingZ3DViewerPage.Preset, BluRingZ3DViewerPage.RenderType, BluRingZ3DViewerPage.Thickness });
                bool step4_2 = z3dvp.verifyHoverBarOptions(BluRingZ3DViewerPage.Navigationtwo, new string[] { BluRingZ3DViewerPage.UndoSegmentation, BluRingZ3DViewerPage.RedoSegmentation, BluRingZ3DViewerPage.SaveIamge, BluRingZ3DViewerPage.SubVolumes, BluRingZ3DViewerPage.Preset, BluRingZ3DViewerPage.RenderType, BluRingZ3DViewerPage.Thickness });
                bool step4_3 = z3dvp.verifyHoverBarOptions(BluRingZ3DViewerPage.Navigationthree, new string[] { BluRingZ3DViewerPage.UndoSegmentation, BluRingZ3DViewerPage.RedoSegmentation, BluRingZ3DViewerPage.SaveIamge, BluRingZ3DViewerPage.SubVolumes, BluRingZ3DViewerPage.Preset, BluRingZ3DViewerPage.RenderType, BluRingZ3DViewerPage.Thickness });
                bool step4_4 = z3dvp.verifyHoverBarOptions(BluRingZ3DViewerPage.ResultPanel, new string[] { BluRingZ3DViewerPage.UndoSegmentation, BluRingZ3DViewerPage.RedoSegmentation, BluRingZ3DViewerPage.SaveIamge, BluRingZ3DViewerPage.SubVolumes, BluRingZ3DViewerPage.Preset, BluRingZ3DViewerPage.RenderType, BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Thickness });
                if(step4_1&& step4_2&& step4_3&& step4_4)
                {
                    MPR = true; 
                }
                bool Three_3d_4 = false;
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                bool step4_5 = z3dvp.verifyHoverBarOptions(BluRingZ3DViewerPage.Navigationone, new string[] { BluRingZ3DViewerPage.UndoSegmentation, BluRingZ3DViewerPage.RedoSegmentation, BluRingZ3DViewerPage.SaveIamge, BluRingZ3DViewerPage.SubVolumes });
                bool step4_6 = z3dvp.verifyHoverBarOptions(BluRingZ3DViewerPage.Navigationtwo, new string[] { BluRingZ3DViewerPage.UndoSegmentation, BluRingZ3DViewerPage.RedoSegmentation, BluRingZ3DViewerPage.SaveIamge, BluRingZ3DViewerPage.SubVolumes});
                bool step4_7 = z3dvp.verifyHoverBarOptions(BluRingZ3DViewerPage.Navigationthree, new string[] { BluRingZ3DViewerPage.UndoSegmentation, BluRingZ3DViewerPage.RedoSegmentation, BluRingZ3DViewerPage.SaveIamge, BluRingZ3DViewerPage.SubVolumes });
                bool step4_8 = z3dvp.verifyHoverBarOptions(BluRingZ3DViewerPage.Navigation3D1, new string[] { BluRingZ3DViewerPage.UndoSegmentation, BluRingZ3DViewerPage.RedoSegmentation, BluRingZ3DViewerPage.SaveIamge, BluRingZ3DViewerPage.SubVolumes, BluRingZ3DViewerPage.Preset});
                if (step4_5 && step4_6 && step4_7 && step4_8)
                {
                    Three_3d_4 = true;
                }
                bool Three_3d_6 = false;
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                bool step4_9 = z3dvp.verifyHoverBarOptions(BluRingZ3DViewerPage.Navigationone, new string[] { BluRingZ3DViewerPage.UndoSegmentation, BluRingZ3DViewerPage.RedoSegmentation, BluRingZ3DViewerPage.SaveIamge, BluRingZ3DViewerPage.SubVolumes, BluRingZ3DViewerPage.Preset, BluRingZ3DViewerPage.RenderType, BluRingZ3DViewerPage.Thickness });
                bool step4_10 = z3dvp.verifyHoverBarOptions(BluRingZ3DViewerPage.Navigationtwo, new string[] { BluRingZ3DViewerPage.UndoSegmentation, BluRingZ3DViewerPage.RedoSegmentation, BluRingZ3DViewerPage.SaveIamge, BluRingZ3DViewerPage.SubVolumes, BluRingZ3DViewerPage.Preset, BluRingZ3DViewerPage.RenderType, BluRingZ3DViewerPage.Thickness });
                bool step4_11 = z3dvp.verifyHoverBarOptions(BluRingZ3DViewerPage.Navigationthree, new string[] { BluRingZ3DViewerPage.UndoSegmentation, BluRingZ3DViewerPage.RedoSegmentation, BluRingZ3DViewerPage.SaveIamge, BluRingZ3DViewerPage.SubVolumes, BluRingZ3DViewerPage.Preset, BluRingZ3DViewerPage.RenderType, BluRingZ3DViewerPage.Thickness });
                bool step4_12 = z3dvp.verifyHoverBarOptions(BluRingZ3DViewerPage.ResultPanel, new string[] { BluRingZ3DViewerPage.UndoSegmentation, BluRingZ3DViewerPage.RedoSegmentation, BluRingZ3DViewerPage.SaveIamge, BluRingZ3DViewerPage.SubVolumes, BluRingZ3DViewerPage.Preset, BluRingZ3DViewerPage.RenderType, BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Thickness });
                bool step4_13 = z3dvp.verifyHoverBarOptions(BluRingZ3DViewerPage.Navigation3D1, new string[] { BluRingZ3DViewerPage.UndoSegmentation, BluRingZ3DViewerPage.RedoSegmentation, BluRingZ3DViewerPage.SaveIamge, BluRingZ3DViewerPage.SubVolumes, BluRingZ3DViewerPage.Preset, BluRingZ3DViewerPage.Toggle3d });
                bool step4_14 = z3dvp.verifyHoverBarOptions(BluRingZ3DViewerPage.Navigation3D2, new string[] { BluRingZ3DViewerPage.UndoSegmentation, BluRingZ3DViewerPage.RedoSegmentation, BluRingZ3DViewerPage.SaveIamge, BluRingZ3DViewerPage.SubVolumes, BluRingZ3DViewerPage.Preset});
                if (step4_9 && step4_10 && step4_11 && step4_12&& step4_13&& step4_14)
                {
                    Three_3d_6 = true;
                }
                bool CurvedMPR = false;
                z3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                bool step4_15 = z3dvp.verifyHoverBarOptions(BluRingZ3DViewerPage.Navigationone, new string[] {  BluRingZ3DViewerPage.SaveIamge, BluRingZ3DViewerPage.SubVolumes, BluRingZ3DViewerPage.Preset, BluRingZ3DViewerPage.RenderType, BluRingZ3DViewerPage.Thickness });
                bool step4_16 = z3dvp.verifyHoverBarOptions(BluRingZ3DViewerPage.Navigationtwo, new string[] {  BluRingZ3DViewerPage.SaveIamge, BluRingZ3DViewerPage.SubVolumes, BluRingZ3DViewerPage.Preset, BluRingZ3DViewerPage.RenderType, BluRingZ3DViewerPage.Thickness });
                bool step4_17 = z3dvp.verifyHoverBarOptions(BluRingZ3DViewerPage.Navigationthree, new string[] {  BluRingZ3DViewerPage.SaveIamge, BluRingZ3DViewerPage.SubVolumes, BluRingZ3DViewerPage.Preset, BluRingZ3DViewerPage.RenderType, BluRingZ3DViewerPage.Thickness });
                bool step4_18 = z3dvp.verifyHoverBarOptions(BluRingZ3DViewerPage._3DPathNavigation, new string[] { BluRingZ3DViewerPage.SaveIamge, BluRingZ3DViewerPage.SubVolumes, BluRingZ3DViewerPage.Preset});
                bool step4_19 = z3dvp.verifyHoverBarOptions(BluRingZ3DViewerPage.MPRPathNavigation, new string[] {  BluRingZ3DViewerPage.SaveIamge, BluRingZ3DViewerPage.SubVolumes, BluRingZ3DViewerPage.Preset, BluRingZ3DViewerPage.RenderType, BluRingZ3DViewerPage.Thickness });
                bool step4_20 = z3dvp.verifyHoverBarOptions(BluRingZ3DViewerPage.CurvedMPR, new string[] {  BluRingZ3DViewerPage.SaveIamge, BluRingZ3DViewerPage.SubVolumes });
                if (step4_15 && step4_16 && step4_17 && step4_18 && step4_19 &&step4_20)
                {
                    CurvedMPR = true;
                }
                bool CalciumScoring = false;
                z3dvp.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                try { z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close"); }
                catch(Exception ex )
                { Logger.Instance.InfoLog("Error in closing a Calcium Scoring dialog box "+ex.ToString()); }
                bool step4_21 = z3dvp.verifyHoverBarOptions(BluRingZ3DViewerPage.CalciumScoring, new string[] { BluRingZ3DViewerPage.SaveIamge, BluRingZ3DViewerPage.SubVolumes, BluRingZ3DViewerPage.Preset });
               if ( step4_21)
                {
                    CalciumScoring = true;
                }

                if (MPR && Three_3d_4 && Three_3d_6 && CurvedMPR && CalciumScoring)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 04<--");

                //STEP 05 :: Apply all tools and Draw a measurement on images.  Save PR and 3D images
                //MPR viewer mode 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1, 1);
                z3dvp.select3dlayout(BluRingZ3DViewerPage.MPR);
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement Navigation2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement Navigation3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                IWebElement ResultPanel = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);

                //3D 6 : 1 viewer mode - Applying Pan tool
                bool Pan = z3dvp.select3DTools(Z3DTools.Pan);
                if(!Pan)
                {
                    Logger.Instance.ErrorLog("Pan Tool cannot be applied on " + BluRingZ3DViewerPage.MPR+" Viewer.");
                }
                PageLoadWait.WaitForFrameLoad(10);
                string Navigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                string Navigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                string Navigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                string ResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                z3dvp.Performdragdrop(Navigation1, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4, Navigation1.Size.Width / 4, 3 * (Navigation1.Size.Height / 4));
                string AfterDragNavigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                string AfterDragNavigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                string AfterDragNavigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                string AfterDragResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                bool PanTool = false;
                if (Navigation1LocVal != AfterDragNavigation1LocVal && Navigation2LocVal != AfterDragNavigation2LocVal && Navigation3LocVal != AfterDragNavigation3LocVal && ResultPanelLocVal != AfterDragResultPanelLocVal)
                {
                    PanTool = true;
                    Logger.Instance.InfoLog("Pan tool applied successfully on Navigation1 in : " + BluRingZ3DViewerPage.MPR + " Viewer.");
                }
                else
                    Logger.Instance.ErrorLog("Pan tool not performed on Navigation1 in: " + BluRingZ3DViewerPage.MPR + " Viewer.");
                z3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);

                //MPR viewer mode - Applying Zoom tool
                bool Zoom = z3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                if (!Zoom)
                {
                    Logger.Instance.ErrorLog("Zoom Tool cannot be applied on " + BluRingZ3DViewerPage.MPR + " Viewer.");
                }
                PageLoadWait.WaitForFrameLoad(10);
                Navigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Navigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                ResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                z3dvp.Performdragdrop(Navigation1, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4, Navigation1.Size.Width / 4, 3 * (Navigation1.Size.Height / 4));
                AfterDragNavigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                AfterDragNavigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                AfterDragNavigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                AfterDragResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                bool ZoomTool = false;
                if (Navigation1LocVal != AfterDragNavigation1LocVal && Navigation2LocVal != AfterDragNavigation2LocVal && Navigation3LocVal != AfterDragNavigation3LocVal && ResultPanelLocVal != AfterDragResultPanelLocVal)
                {
                    ZoomTool = true;
                    Logger.Instance.InfoLog("Zoom tool applied successfully on Navigation1 in : " + BluRingZ3DViewerPage.MPR + " Viewer.");
                }
                else
                    Logger.Instance.ErrorLog("Zoom tool not performed on Navigation1 in : " + BluRingZ3DViewerPage.MPR + " Viewer.");
                z3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);

                //MPR viewer mode - Applying Rotate tool
                bool Rotate = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center);
                if (!Rotate)
                {
                    Logger.Instance.ErrorLog("Rotate Tool cannot be applied on " + BluRingZ3DViewerPage.MPR + " Viewer.");
                }
                PageLoadWait.WaitForFrameLoad(10);
                Navigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Navigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                ResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                z3dvp.Performdragdrop(Navigation3, Navigation3.Size.Width / 4, Navigation3.Size.Height / 4, Navigation3.Size.Width / 4, 3 * (Navigation3.Size.Height / 4));
                AfterDragNavigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                AfterDragNavigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                AfterDragNavigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                AfterDragResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                bool RotateTool = false;
                if (!Navigation1LocVal.Equals(AfterDragNavigation1LocVal) && !Navigation2LocVal.Equals(AfterDragNavigation2LocVal) && !Navigation3LocVal.Equals(AfterDragNavigation3LocVal) && !ResultPanelLocVal.Equals(AfterDragResultPanelLocVal))
                {
                    RotateTool = true;
                    Logger.Instance.InfoLog("Rotate tool applied successfully on Navigation3 in : " + BluRingZ3DViewerPage.MPR + " Viewer.");
                }
                else
                    Logger.Instance.ErrorLog("Rotate tool not performed on Navigation3 in : " + BluRingZ3DViewerPage.MPR + " Viewer.");
                z3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);

                //3D 6 : 1 viewer mode
                z3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).SendKeys("X").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                // Getting Thumpnail image count before save PR and 3D images
                IList<IWebElement> ThumpnailCount = Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                PageLoadWait.WaitForFrameLoad(5);
                Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navigation2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                ResultPanel = z3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                IWebElement Navigation3D1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                IWebElement Navigation3D2 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);

                //3D 6 : 1 viewer mode - Applying Rotate tool
                Rotate = z3dvp.select3DTools(Z3DTools.Rotate_Tool_1_Click_Center, BluRingZ3DViewerPage.Navigation3D1);
                if (!Rotate)
                {
                    Logger.Instance.ErrorLog("Rotate Tool cannot be applied on " + BluRingZ3DViewerPage.Three_3d_6 + " Viewer.");
                }
                PageLoadWait.WaitForFrameLoad(10);
                Navigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Navigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                ResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                string Navigation3d1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                string Navigation3d2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                z3dvp.Performdragdrop(Navigation3D2, Navigation3D2.Size.Width / 4, Navigation3D2.Size.Height / 4, Navigation3D2.Size.Width / 4, 3 * (Navigation3D2.Size.Height / 4));
                AfterDragNavigation1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                AfterDragNavigation2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationtwo);
                AfterDragNavigation3LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationthree);
                AfterDragResultPanelLocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.ResultPanel);
                string AfterDragNavigation3d1LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D1);
                string AfterDragNavigation3d2LocVal = z3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigation3D2);
                bool RotateTool_3D = false;
                if (Navigation3d1LocVal != AfterDragNavigation3d1LocVal && Navigation3d2LocVal != AfterDragNavigation3d2LocVal)
                {
                    RotateTool_3D = true;
                    Logger.Instance.InfoLog("Rotate tool applied successfully on 3D1 viewport in : " + BluRingZ3DViewerPage.Three_3d_6 + " Viewer.");
                }
                else
                    Logger.Instance.ErrorLog("Rotate tool not performed on 3D1 viewport in : " + BluRingZ3DViewerPage.Three_3d_6 + " Viewer.");
                z3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(10);

                //3D 6 : 1 viewer mode - Applying Sculpt Polygon tool
                bool Sculptpolygon = z3dvp.select3DTools(Z3DTools.Sculpt_Tool_for_3D_1_Polygon, BluRingZ3DViewerPage.Navigation3D1);
                if (!Sculptpolygon)
                {
                    Logger.Instance.ErrorLog("Sculpt Tool cannot be applied on " + BluRingZ3DViewerPage.Three_3d_6 + " Viewer.");
                }
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SculptToolPolygondialog, "Close");
                PageLoadWait.WaitForFrameLoad(5);

                new Actions(Driver).MoveToElement(Navigation3D1, Navigation3D1.Size.Width / 4 - 20, Navigation3D1.Size.Height / 2 - 50).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Navigation3D1, Navigation3D1.Size.Width / 4 - 20, Navigation3D1.Size.Height / 2 + 50).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Navigation3D1, Navigation3D1.Size.Width / 2, Navigation3D1.Size.Height / 2).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Navigation3D1, Navigation3D1.Size.Width / 4 - 20, Navigation3D1.Size.Height / 2 - 50).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);

                //3D 6 : 1 viewer mode - Applying Selection tool
                bool SelectingTool = z3dvp.select3DTools(Z3DTools.Selection_Tool, BluRingZ3DViewerPage.Navigation3D1);
                if (!SelectingTool)
                {
                    Logger.Instance.ErrorLog("Selecting Tool cannot be applied on " + BluRingZ3DViewerPage.Three_3d_6 + " Viewer.");
                }
                PageLoadWait.WaitForFrameLoad(10);
                try
                {
                    z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.SelectionTooldialog, "Close");
                }catch(Exception ex)
                {
                    Logger.Instance.ErrorLog("Error in handleing selection tool box Step5  " + ex.ToString());
                }
                PageLoadWait.WaitForFrameLoad(5);
                new Actions(Driver).MoveToElement(Navigation3D1, Navigation3D1.Size.Width / 4 - 20, Navigation3D1.Size.Height / 2 - 50).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);

                //3D 6 : 1 viewer mode - Applying Line Measurement tool
                bool line = z3dvp.select3DTools(Z3DTools.Line_Measurement);
                if (!line)
                {
                    Logger.Instance.ErrorLog("Line Measurement Tool cannot be applied on " + BluRingZ3DViewerPage.Three_3d_6 + " Viewer.");
                }
                PageLoadWait.WaitForFrameLoad(10);
                z3dvp.Performdragdrop(Navigation1, Navigation1.Size.Width / 4, Navigation1.Size.Height / 4, Navigation1.Size.Width / 4, (Navigation1.Size.Height / 2));

                //Curved MPR viewer mode - Applying Curve Drawing tool
                z3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR); // Defaultly selected curve drawing tool
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement _3DPathNavigation = z3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);


                //Saving PR and 3D images
                bool DrawLine = false;
                try
                {
                    Navigation1 = z3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                    z3dvp.SelectOptionsfromViewPort(BluRingZ3DViewerPage.Navigationone, "Save image and annotations to the exam");
                    //new Actions(Driver).MoveToElement(Navigation1.FindElement(By.CssSelector(Locators.CssSelector.LeftTopPane)),1,1).Build().Perform();
                    //PageLoadWait.WaitForFrameLoad(5);
                    //Navigation1.FindElement(By.CssSelector(Locators.CssSelector.SaveImageExam)).Click();
                    //PageLoadWait.WaitForFrameLoad(120);
                    new Actions(Driver).MoveToElement(_3DPathNavigation).Click().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(5);
                    z3dvp.SelectOptionsfromViewPort(BluRingZ3DViewerPage._3DPathNavigation, "Save image and annotations to the exam");
                    //new Actions(Driver).MoveToElement(_3DPathNavigation.FindElement(By.CssSelector(Locators.CssSelector.LeftTopPane))).Build().Perform();
                    //PageLoadWait.WaitForFrameLoad(5);
                    //_3DPathNavigation.FindElement(By.CssSelector(Locators.CssSelector.SaveImageExam)).Click();           
                    PageLoadWait.WaitForFrameLoad(60);
                    new Actions(Driver).MoveToElement(_3DPathNavigation).Click().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(5);
                    IWebElement ViewerContainer = z3dvp.ViewerContainer();
                    
                    DrawLine = CompareImage(result.steps[ExecutedSteps], ViewerContainer);
                }
                catch(Exception ex)
                {
                    Logger.Instance.ErrorLog("Error while save PR and 3D images : "+ex.Message);
                }

                //Calcium Scoring viewer mode - Applying Calcium Scoring tool
                z3dvp.select3dlayout(BluRingZ3DViewerPage.CalciumScoring); // Defaultly selected Calcium Scoring tool
                PageLoadWait.WaitForFrameLoad(5);
                try
                {
                    z3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                }
                catch { }
                PageLoadWait.WaitForFrameLoad(5);
                IWebElement CalciumScoreImage = z3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                int GreenColorBefore = z3dvp.LevelOfSelectedColor(CalciumScoreImage, testid, ExecutedSteps + 1, 0, 255, 0, 2);
                Actions calciumaction = new Actions(Driver);
                calciumaction.MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2 - 120, CalciumScoreImage.Size.Height * 3/4 ).ClickAndHold()
                    .MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2 - 40, CalciumScoreImage.Size.Height * 3/4 )
                    .MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2 + 40, CalciumScoreImage.Size.Height * 3/4)
                    .MoveToElement(CalciumScoreImage, CalciumScoreImage.Size.Width / 2 - 50, CalciumScoreImage.Size.Height * 3/4 + 60).Release().Build().Perform();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.LoadingIcon)));
                int GreenColorAfter = z3dvp.LevelOfSelectedColor(z3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 1, 0, 255, 0, 2);
                Logger.Instance.InfoLog("Step 5 :" + " PanTool: " + PanTool + " ZoomTool: " + ZoomTool + " RotateTool: " + RotateTool + " RotateTool_3D: " + RotateTool_3D + " DrawLine: " + DrawLine + " GreenColorBefore: " + GreenColorBefore + " GreenColorAfter: " + GreenColorAfter);
                if (PanTool && ZoomTool && RotateTool && RotateTool_3D  && DrawLine && GreenColorBefore != GreenColorAfter)
                {
                    result.steps[ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[ExecutedSteps].StepFail();

                }
                Logger.Instance.InfoLog("-->Completed Step : 05<--");

                //STEP 06 :: Relaunch the same study again and verify the saved PR and 3D images in the viewport of the universal viewer.
                z3dvp.CloseViewer();
                z3dvp.searchandopenstudyin3D(Patientid, thumbnailSearchvalue[0], BluRingZ3DViewerPage.Two_2D);
                // Getting Thumpnail image count after save PR and 3D images
                IList<IWebElement> SavedThumpnail = Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                bool step6 = z3dvp.selectthumbnail("CT", SavedThumpnail.Count-2, thumbnailSearchvalue[3]);
                bool step6_1 = z3dvp.selectthumbnail("CT", SavedThumpnail.Count-1, thumbnailSearchvalue[3]);
                bool step6_2 = z3dvp.selectthumbnail("PR", 0, thumbnailSearchvalue[2]);
                if (step6 && step6_1 && step6_2 && SavedThumpnail.Count.Equals(ThumpnailCount.Count+3))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 06<--");

                //STEP 07 :: Select a PR series & Attempt to launch Z3D view
                bool DisableViewerButton = Driver.FindElement(By.CssSelector(Locators.CssSelector.SmartViewEnabled)).GetAttribute("class").Contains("disabled");
                if (DisableViewerButton)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 07<--");

                //STEP 08 :: Close the study and Logout iCA
                z3dvp.CloseViewer();
                login.Logout();
                bool LoginWindow = Driver.FindElement(By.CssSelector("input[id$='_LoginMasterContentPlaceHolder_Username']")).Displayed;
                if (LoginWindow)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 08<--");

                //STEP 09 :: Login ICA. Select a unsupported series and Attempt to launch Z3D view
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                bool step9 = z3dvp.searchandopenstudyin3D(searchvalue[0], searchvalue[1], BluRingZ3DViewerPage.Three_3d_4, "Accession");
                if (!step9)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 09<--");

                //STEP 10 :: Search and load a 3D supported study in universal viewer that has 3D supported series which doesn't meet 3D criteria.
                z3dvp.CloseViewer();
                bool step10 = z3dvp.searchandopenstudyin3D(Patientid, thumbnailSearchvalue[1], BluRingZ3DViewerPage.Three_3d_4);
                if (!step10)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 10<--");

                //STEP 11 :: Close the study and Logout from ICA
                z3dvp.CloseViewer();
                login.Logout();
                LoginWindow = Driver.FindElement(By.CssSelector("input[id$='_LoginMasterContentPlaceHolder_Username']")).Displayed;
                if (LoginWindow)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 11<--");

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
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            finally
            {
                z3dvp.CloseViewer();
                login.Logout();
                z3dvp.DeletePriorsInEA(Config.DestEAsIp, Patientid, "calcium4");
            }
        }
    }
}
