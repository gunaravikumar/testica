using System;
using System.Globalization;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
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
using Application = TestStack.White.Application;
using Window = TestStack.White.UIItems.WindowItems.Window;
using TestStack.White.Factory;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.TabItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;
using TestStack.White.UIItems;

namespace Selenium.Scripts.Tests
{
    class HistoryPanel
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public HPHomePage hphomepage { get; set; }
        public HPLogin hplogin { get; set; }
        public MpacLogin mpaclogin { get; set; }

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        public HistoryPanel(String classname)
        {                              
            login = new Login();
            login.DriverGoTo(login.url);
            hplogin = new HPLogin();
            hphomepage = new HPHomePage();
            mpaclogin = new MpacLogin();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ei = new ExamImporter();
        }
                    
        /// <summary>
        /// Study Panel - Viewer Layout
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test1_27931(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Studies studies = null;
            TestCaseResult result;
            StudyViewer viewer = null;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String UserName = Config.adminUserName;
                String Password = Config.adminPassword;
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String SeriesCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NoOfSeries");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String EA91 = login.GetHostName(Config.EA91);

                //Step 1 :- Login as Administrator
                login.LoginIConnect(UserName, Password);
                ExecutedSteps++;

                //Navigate to Inbounds
                studies = (Studies)login.Navigate("Studies");

                //Search, Select Study and Launch study
                studies.SearchStudy(AccessionNo: AccessionID, Datasource:  EA91);
                studies.SelectStudy("Accession", AccessionID);
                viewer = StudyViewer.LaunchStudy();

                //Step 2 :- Launch study to verify viewer layout and tools in toolbar
                if (viewer.SeriesViewer_2X2(1).Displayed && !viewer.SeriesViewer_2X3(1).Displayed && studies.GetReviewToolsFromviewer().Count != 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                String PatientInfo = viewer.PatientInfoTab();

                //Step 3 :- Check patient details  in study viewer
                if(PatientInfo.Split(',')[0] == LastName && PatientInfo.Split(',')[1] == FirstName
                    && PatientInfo.Split(',')[2] == PatientID)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Get Patient history tab status
                Boolean patienthistorytab = !viewer.PatientHistoryDrawer().GetAttribute("style").Contains("right");

                //Step 4 :- Verify all series are listed in thumbnails and  patient history panel is hided.
                if (Int32.Parse(SeriesCount) == viewer.NumberOfThumbnails() && patienthistorytab &&
                    BasePage.Driver.FindElement(By.CssSelector("div#m_patientHistory_drawer")).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Drag and drop a series in viewport
                Actions action = new Actions(BasePage.Driver);
                action.DragAndDrop(viewer.Thumbnails()[0], viewer.SeriesViewer_2X2(1)).Build().Perform();

                String ThumbnailSeriesUID = viewer.GetInnerAttribute(viewer.Thumbnails()[0], "src", '&', "seriesUID");
                String ViewerSeriesUID = viewer.GetInnerAttribute(viewer.SeriesViewer_2X2(), "src", '&', "seriesUID");

                //Step 5 :- Validate Dragged thumbnail is loaded to the viewport and thumbnail is highlighted correctly 
                if (ThumbnailSeriesUID.Equals(ViewerSeriesUID) &&
                    !viewer.Thumbnails()[0].GetCssValue("border-top-color").Equals("transparent"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }                          
                    
                //Get detail of Viewport
                String EllipseseriesUID_1 = viewer.GetInnerAttribute(viewer.SeriesViewer_2X2(), "src", '&', "seriesUID");

                //Select Ellipse Tool
                viewer.SelectToolInToolBar("DrawEllipse");

                //Draw Ellipse                 
                viewer.JoinCoordinatesInStudyViewer(viewer.SeriesViewer_2X2(), 111, 45, 111, 60);

                //Get detail of Viewport
                String EllipseseriesUID_2 = viewer.GetInnerAttribute(viewer.SeriesViewer_2X2(), "src", '&', "seriesUID");

                //Validate Images 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool EllipseImage = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_2X2());

                //Step 6 :- Draw Ellipse and Validate
                if (EllipseImage && EllipseseriesUID_1.Equals(EllipseseriesUID_2))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.SelectToolInToolBar("Reset");
                
                //Get detail of Viewport
                String ZoomseriesUID_1 = viewer.GetInnerAttribute(viewer.SeriesViewer_2X2(1), "src", '&', "seriesUID");

                //Perform Zoom
                viewer.SelectToolInToolBar("Zoom");
                viewer.DragMovement(viewer.SeriesViewer_2X2(1));

                //Get detail of Viewport
                PageLoadWait.WaitForFrameLoad(15);
                String ZoomseriesUID_2 = viewer.GetInnerAttribute(viewer.SeriesViewer_2X2(), "src", '&', "seriesUID");

                //Validate Images 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool ZoomImage = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_2X2());
                
                //Step 7 :- Perform Zoom
                if (ZoomImage && ZoomseriesUID_1.Equals(ZoomseriesUID_2))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.SelectToolInToolBar("Reset");

               
                
                //Retreiving viewport details in default layout
                String[] SeriesUID_2x2 = viewer.GetSeriesUID(viewer.SeriesViewPorts());
                int emptyPorts_2x2 = viewer.GetEmptyViewports().Count;

                //Change Layout to 2x3 viewer
                viewer.SelectToolInToolBar("SeriesViewer2x3");

                //Retreiving viewport details in 2x3 viewer layout
                String[] SeriesUID_2x3 = viewer.GetSeriesUID(viewer.SeriesViewPorts());
                int emptyPorts_2x3 = viewer.GetEmptyViewports().Count;

                int CountIndex = 0;
                Boolean seriesUIDMatch = Array.Exists(SeriesUID_2x2, SeriesUID => SeriesUID.Equals(SeriesUID_2x3[CountIndex++]));

                Boolean diffemptyPorts;
                if((emptyPorts_2x2 == 0 && emptyPorts_2x3 == 0) || emptyPorts_2x2 + 2 == emptyPorts_2x3 
                    || (Int32.Parse(SeriesCount) >= 4 && emptyPorts_2x3 <= 2))
                {
                    diffemptyPorts = true;
                }
                else
                {
                    diffemptyPorts = false;
                }

                //Step 8 :- Change Viewer layout and check the images
                if (seriesUIDMatch && viewer.SeriesViewPorts().Count == 6 && diffemptyPorts)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Click any one empty view port to activate it
                IList<IWebElement> EmptyPorts = viewer.GetEmptyViewports();
                IWebElement Viewport;
                if (EmptyPorts.Count != 0)
                {
                    Viewport = EmptyPorts[0];
                }
                else
                {
                    Viewport = viewer.SeriesViewPorts()[0];
                }
                Viewport.Click();

                //Double click any one thumbnail to open in active viewport
                //action.DoubleClick(viewer.Thumbnails()[0]).Build().Perform();
                viewer.DoubleClick(viewer.Thumbnails()[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Step 9 :- Validate selected series is opened in the active viewport
                if (viewer.GetInnerAttribute(Viewport, "src", '&', "seriesUID")
                    == viewer.GetInnerAttribute(viewer.Thumbnails()[0], "src", '&', "seriesUID"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Get Empty viewports
                IWebElement DragViewport;
                EmptyPorts = viewer.GetEmptyViewports();

                //Drag and drop a series in empty viewport
                if (EmptyPorts.Count != 0)
                {
                    DragViewport = EmptyPorts[0];                    
                }
                else
                {
                    DragViewport = viewer.SeriesViewPorts()[0];   
                }
                action.DragAndDrop(viewer.Thumbnails()[1], DragViewport).Build().Perform();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Step 10 :- Validate dragged series is opened in the empty/Second viewport
                if (viewer.GetInnerAttribute(DragViewport, "src", '&', "seriesUID")
                    == viewer.GetInnerAttribute(viewer.Thumbnails()[1], "src", '&', "seriesUID"))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Set a view port to be active
                viewer.SeriesViewPorts()[0].Click();
                PageLoadWait.WaitForPageLoad(10);
                String seriesUID_active = viewer.GetInnerAttribute(viewer.SeriesViewPorts()[0], "src", '&', "seriesUID");
                
                //Get index of active thumbnail
                String[] AllSeriesUID = viewer.GetSeriesUID(viewer.Thumbnails());
                int ActivethumbIndex = Array.FindIndex(viewer.GetSeriesUID(viewer.Thumbnails()), s => s.Equals(seriesUID_active));
                int NextSeriesthumbIndex = 0;
                if (ActivethumbIndex == (Int32.Parse(SeriesCount)-1))
                {
                    NextSeriesthumbIndex = (ActivethumbIndex + 1) - Int32.Parse(SeriesCount);
                }
                else
                {
                    NextSeriesthumbIndex = ActivethumbIndex + 1;
                }

                //Click Next series using "Next series" tool btn
                viewer.SelectToolInToolBar(IEnum.ViewerTools.NextSeries);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                String seriesUID_next = viewer.GetInnerAttribute(viewer.SeriesViewPorts()[0], "src", '&', "seriesUID");

                //Click Previous Series "Previous series" tool button
                viewer.SelectToolInToolBar(IEnum.ViewerTools.PreviousSeries);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                String seriesUID_previous = viewer.GetInnerAttribute(viewer.SeriesViewPorts()[0], "src", '&', "seriesUID");

                //Step 11 :- Use Next Series/Previous Series tool buttons to scroll to the next/previous series.
                if (seriesUID_next.Equals(AllSeriesUID[NextSeriesthumbIndex])
                    && seriesUID_previous.Equals(seriesUID_active))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                String seriesUID_active_1 = viewer.GetInnerAttribute(viewer.ActiveThumbnail(), "src", '&', "seriesUID");

                //Get index of active thumbnail
                String[] AllSeriesUID_1 = viewer.GetSeriesUID(viewer.Thumbnails());
                int ActivethumbIndex_1 = Array.FindIndex(viewer.GetSeriesUID(viewer.Thumbnails()), s => s.Equals(seriesUID_active_1));
                int NextSeriesthumbIndex_1 = 0;
                if (ActivethumbIndex == (Int32.Parse(SeriesCount) - 1))
                {
                    NextSeriesthumbIndex_1 = (ActivethumbIndex_1 + 1) - Int32.Parse(SeriesCount);
                }
                else
                {
                    NextSeriesthumbIndex_1 = ActivethumbIndex_1 + 1;
                }

                //Click Next series using Keyboard right button
                action.SendKeys(viewer.SeriesViewPorts()[0], Keys.Right).Build().Perform();
                PageLoadWait.WaitForAllViewportsToLoad(30);
                Thread.Sleep(2000);
                String seriesUID_next1 = viewer.GetInnerAttribute(viewer.SeriesViewPorts()[0], "src", '&', "seriesUID");

                //Click Previous Series Keyboard left button
                action.SendKeys(viewer.SeriesViewPorts()[0], Keys.Left).Build().Perform();
                PageLoadWait.WaitForAllViewportsToLoad(30);
                Thread.Sleep(2000);
                String seriesUID_previous1 = viewer.GetInnerAttribute(viewer.SeriesViewPorts()[0], "src", '&', "seriesUID");

                //Step 12 :- Use Keyboard right/left buttons to scroll to the next/previous series.
                if (seriesUID_next1.Equals(AllSeriesUID_1[NextSeriesthumbIndex_1])
                    && seriesUID_previous1.Equals(seriesUID_active_1))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 13 :- Validate tooltip dropdown for all grouped tools in review Toolbar
                int counter = 0;
                ExecutedSteps++;
                foreach (IWebElement toolgroup in viewer.GroupedReviewTools())
                {
                    studies.JSMouseHover(toolgroup);
                    IList<IWebElement> dropdowntools = viewer.DropdownReviewTools();
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(dropdowntools[counter]));

                    if(dropdowntools[counter].Displayed)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        Logger.Instance.InfoLog("Tooltip dropdown for tool group '" + counter + "' is not displayed.");
                        break;
                    }
                    counter++;
                }

                //Get detail of Viewport
                String PanseriesUID_1 = viewer.GetInnerAttribute(viewer.SeriesViewer_2X2(1), "src", '&', "seriesUID");

                //Perform Pan 
                viewer.SelectToolInToolBar("Pan");
                viewer.DragMovement(viewer.SeriesViewer_2X2());

                //Get detail of Viewport
                String PanseriesUID_2 = viewer.GetInnerAttribute(viewer.SeriesViewer_2X2(1), "src", '&', "seriesUID");

                //Validate Images 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool PanImage = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_2X2());

                //Step 14 :- Apply Pan tool
                if (PanImage && PanseriesUID_1.Equals(PanseriesUID_2))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.SelectToolInToolBar("Reset");

                //Get detail of Viewport
                String InvertseriesUID_1 = viewer.GetInnerAttribute(viewer.SeriesViewPorts()[0], "src", '&', "seriesUID");

                //Select one viewport be active
                viewer.SeriesViewPorts()[0].Click();

                //Apply Invert tool
                viewer.SelectToolInToolBar("Invert");
                viewer.SeriesViewPorts()[0].Click();
                PageLoadWait.WaitForAllViewportsToLoad(10);

                //Get detail of Viewport
                String InvertseriesUID_2 = viewer.GetInnerAttribute(viewer.SeriesViewPorts()[0], "src", '&', "seriesUID");

                //Validate Images 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool InvertImage = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewPorts()[0]);

                //Step 15 :- Apply Invert tool and Validate
                if (InvertImage && InvertseriesUID_1.Equals(InvertseriesUID_2))
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.SelectToolInToolBar("Reset");
               
                
                //Get Tools count before window resizing
                int toolscount_1 = viewer.GetReviewToolsFromviewer().Count;

                //Get height of tool bar and width of thumbnail container 
                int ToolsBox_max_height1, ContainerWidth1, toolscount_2, ToolsBox_min_height, ContainerWidth2, ThumbnailsWidth;
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                {
                    ToolsBox_max_height1 = Int32.Parse(viewer.GetInnerAttribute(viewer.ReviewtoolBar(), "style", ';', "HEIGHT", ":").Replace("px", "").Trim());
                    ContainerWidth1 = Int32.Parse(viewer.GetInnerAttribute(viewer.ThumbnailContainer(), "style", ';', "WIDTH", ":").Replace("px", "").Trim());

                    //Resize window size
                    BasePage.Driver.Manage().Window.Size = new Size(700, 700);

                    //Get Tools count after window resizing
                    toolscount_2 = viewer.GetReviewToolsFromviewer().Count;
                    //IWebElement ToolsBox_min = BasePage.Driver.FindElement(By.CssSelector("#reviewToolbar:not([style*='height: 38px;'])"));
                    ToolsBox_min_height = Int32.Parse(viewer.GetInnerAttribute(viewer.ReviewtoolBar(), "style", ';', "HEIGHT", ":").Replace("px", "").Trim());

                    //Get width of thumbnail container and all thumbnails
                    ContainerWidth2 = Int32.Parse(viewer.GetInnerAttribute(viewer.ThumbnailContainer(), "style", ';', "WIDTH", ":").Replace("px", "").Trim());
                    ThumbnailsWidth = Int32.Parse(viewer.GetInnerAttribute(viewer.ThumbnailsDiv(), "style", ';', "WIDTH", ":").Replace("px", "").Trim());
                }
                
                else
                {
                    ToolsBox_max_height1 = Int32.Parse(viewer.GetInnerAttribute(viewer.ReviewtoolBar(), "style", ';', "height", ":").Replace("px", "").Trim());
                    ContainerWidth1 = Int32.Parse(viewer.GetInnerAttribute(viewer.ThumbnailContainer(), "style", ';', "width", ":").Replace("px", "").Trim());

                    //Resize window size
                    BasePage.Driver.Manage().Window.Size = new Size(700, 700);

                    //Get Tools count after window resizing
                    toolscount_2 = viewer.GetReviewToolsFromviewer().Count;
                    //IWebElement ToolsBox_min = BasePage.Driver.FindElement(By.CssSelector("#reviewToolbar:not([style*='height: 38px;'])"));
                    ToolsBox_min_height = Int32.Parse(viewer.GetInnerAttribute(viewer.ReviewtoolBar(), "style", ';', "height", ":").Replace("px", "").Trim());

                    //Get width of thumbnail container and all thumbnails
                    ContainerWidth2 = Int32.Parse(viewer.GetInnerAttribute(viewer.ThumbnailContainer(), "style", ';', "width", ":").Replace("px", "").Trim());
                    ThumbnailsWidth = Int32.Parse(viewer.GetInnerAttribute(viewer.ThumbnailsDiv(), "style", ';', "width", ":").Replace("px", "").Trim());

                }

                //To check thumbnails scrollbar is present if thumbnails container size is higher than browser window 
                Boolean ScrollbarStatus = true;
                if (ContainerWidth2 < ThumbnailsWidth)
                {
                    if (!viewer.ThumbnailScrollBar().Displayed)
                    {
                        ScrollbarStatus = false;
                    }
                }

                //Step 16 :- Resize browser window size and verify auto fit of review tools row, thumbnail scrollbar
                if (toolscount_1 == toolscount_2 && ToolsBox_max_height1 < ToolsBox_min_height && ScrollbarStatus &&
                    viewer.SeriesViewPorts().Count == 6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Maximize window size
                BasePage.Driver.Manage().Window.Maximize();

                //Get Tools count after window resizing
                int toolscount_3 = viewer.GetReviewToolsFromviewer().Count;

                  int ToolsBox_max_height2, ContainerWidth3;
                  if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                  {
                      //IWebElement ToolsBox_max = BasePage.Driver.FindElement(By.CssSelector("#reviewToolbar[style*='height: 38px;']"));
                      ToolsBox_max_height2 = Int32.Parse(viewer.GetInnerAttribute(viewer.ReviewtoolBar(), "style", ';', "HEIGHT", ":").Replace("px", "").Trim());
                      //Get width of thumbnail container 
                      ContainerWidth3 = Int32.Parse(viewer.GetInnerAttribute(viewer.ThumbnailContainer(), "style", ';', "WIDTH", ":").Replace("px", "").Trim());
                  }
                  else
                  {
                      //IWebElement ToolsBox_max = BasePage.Driver.FindElement(By.CssSelector("#reviewToolbar[style*='height: 38px;']"));
                      ToolsBox_max_height2 = Int32.Parse(viewer.GetInnerAttribute(viewer.ReviewtoolBar(), "style", ';', "height", ":").Replace("px", "").Trim());
                      //Get width of thumbnail container 
                      ContainerWidth3 = Int32.Parse(viewer.GetInnerAttribute(viewer.ThumbnailContainer(), "style", ';', "width", ":").Replace("px", "").Trim());
                  }
                //Step 17 :- Maximize browser size and Verify Tools , Thumbnail, images, viewport resize and all restored as previous.
                if (toolscount_1 == toolscount_2 && ToolsBox_max_height1 == ToolsBox_max_height2 && ContainerWidth1 == ContainerWidth3
                    && viewer.SeriesViewPorts().Count == 6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test Step Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close Study and logout iCA
                studies.CloseStudy();
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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

                if (ExecutedSteps >= 13) { BasePage.Driver.Manage().Window.Maximize(); }

                //Close Study and logout iCA
                studies.CloseStudy();
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary>
        /// Study Panel - Report Viewer
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test2_27931(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Studies studies = null;
            StudyViewer viewer = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String UserName = Config.adminUserName;
                String Password = Config.adminPassword;
                String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
                String FirstName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "FirstName");
                String SeriesCount = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "NoOfSeries");
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String EA1 = login.GetHostName(Config.EA1);

                //Step 1 :- Upload study to HALOPACS datasource
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Login as Administrator
                login.LoginIConnect(UserName, Password);

                //Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Search, Select and Launch study
                studies.SearchStudy(AccessionNo: AccessionID, Datasource: EA1);
                studies.SelectStudy("Accession", AccessionID);
                viewer = StudyViewer.LaunchStudy();

                //wait for report icon to be clickable
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.TitlebarReportIcon()));

                //Step 2 :- Validate study opened with default layout and report button should be available 
                if (viewer.TitlebarReportIcon().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Get width of study viewer container
                int ContainerWidth_1;
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                {
                    ContainerWidth_1 = Int32.Parse(viewer.GetInnerAttribute(viewer.ViewerContainer(), "style", ';', "WIDTH", ":").Replace("px", "").Trim());
                }
                else
                {
                    ContainerWidth_1 = Int32.Parse(viewer.GetInnerAttribute(viewer.ViewerContainer(), "style", ';', "width", ":").Replace("px", "").Trim());
                }
                

                //Click report icon
                viewer.TitlebarReportIcon().Click();
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id$='studyPanel_1_reportViewerContainer']")));

                //Get width of study viewer container
                
                int ContainerWidth_2;
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                {
                    ContainerWidth_2 = Int32.Parse(viewer.GetInnerAttribute(viewer.ViewerContainer(), "style", ';', "WIDTH", ":").Replace("px", "").Trim());
                }
                else
                {
                    ContainerWidth_2 = Int32.Parse(viewer.GetInnerAttribute(viewer.ViewerContainer(), "style", ';', "width", ":").Replace("px", "").Trim());
                }
                
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);

                //Take Screenshot
                Boolean ViewerStatus = studies.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer()); 

                //Step 3 :- Validate report list,full screen report viewer icons and study panel with images are displayed properly
                if (viewer.ReportFullScreenIcon().Displayed && viewer.ViewerReportListButton().Displayed
                    && ContainerWidth_1 > ContainerWidth_2 && ViewerStatus)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select report list button
                viewer.ViewerReportListButton().Click();

                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id$='studyPanel_1_m_reportViewer_reportListContainer']")));
                Dictionary<int, string[]> ReportListDetails = viewer.StudyViewerListResults("StudyPanel", "report", 1);
                
                //Step 4 :- Check report list is displayed with available reports
                if (viewer.ReportListContainer().Displayed && ReportListDetails.Count != 0)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Get column names and row details of first report in the report list
                String[] reportColumnNames = viewer.StudyViewerListColumnNames("StudyPanel", "report", 1);
                String[] reportColumnValues = BasePage.GetColumnValues(ReportListDetails, "Title", reportColumnNames);
                Dictionary<string, string> FirstreportDetails = viewer.StudyViewerListMatchingRow("Title", reportColumnValues[0], "StudyPanel", "report");

                //Select the first report in report list
                viewer.SelectItemInStudyViewerList("Title", reportColumnValues[0], "StudyPanel", "report");

                //Get Report details
                viewer.SwitchToReportFrame("studypanel");
                
                String css = "#ViewerDisplay object";

                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                {
                    css = "#ViewerDisplay iframe";
                }

                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(css)));
                Dictionary<string, string> reportDetails = viewer.ReportDetails("studypanel");

                //Step 5 :- Select a report in the list and check it's correctness
                if (FirstreportDetails["Date"].Equals(reportDetails["Report Date"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select report list button
                PageLoadWait.WaitForFrameLoad(10);
                viewer.ViewerReportListButton().Click();

                //Select the second report in report list
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id$='studyPanel_1_m_reportViewer_reportListContainer']")));
                Dictionary<string, string> SecondreportDetails = viewer.StudyViewerListMatchingRow("Title", reportColumnValues[0], "StudyPanel", "report");
                viewer.SelectItemInStudyViewerList("Title", reportColumnValues[1], "StudyPanel", "report");

                //Get Report details
                viewer.SwitchToReportFrame("studypanel");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(css)));
                Dictionary<string, string> reportDetails_2 = viewer.ReportDetails("studypanel");

                //Step 6 :- Select a report in the list and check it's correctness
                if (SecondreportDetails["Date"].Equals(reportDetails_2["Report Date"]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 7 :- Select next report in the list one by one and check it's correctness on viewer
                int counter = 0;
                ExecutedSteps++;
                foreach (String columnname in reportColumnNames)
                {
                    if(counter < 2)
                    {
                        continue;
                    }
                    //Select report list button
                    PageLoadWait.WaitForFrameLoad(10);
                    viewer.ViewerReportListButton().Click();

                    //Select the next report in report list
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id$='studyPanel_1_m_reportViewer_reportListContainer']")));
                    Dictionary<string, string> NextreportDetails = viewer.StudyViewerListMatchingRow("Title", reportColumnValues[0], "StudyPanel", "report");
                    viewer.SelectItemInStudyViewerList("Title", reportColumnValues[counter], "StudyPanel", "report");
                    
                    //Get Report details
                    viewer.SwitchToReportFrame("studypanel");
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(css)));
                    Dictionary<string, string> reportDetails_next = viewer.ReportDetails("studypanel");

                    //check it's correctness of report displayed
                    if (NextreportDetails["Date"].Equals(reportDetails_next["Report Date"]))
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        break;
                    }
                }

                //Step 8 :- Tablet testing Step
                result.steps[++ExecutedSteps].status = "Not Automated";
                
                //Maximize report viewer
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ReportFullScreenIcon()));
                viewer.ReportFullScreenIcon().Click();

                //Sync-up 
                PageLoadWait.WaitForPageLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ReportFullScreenIcon()));

                //Get Patient info in study panel title bar
                String PatientInfo = viewer.PatientInfoTab();

                bool STEP_9 = false;

                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                    STEP_9 = viewer.GetInnerAttribute(viewer.ViewerContainer(), "style", ';', "DISPLAY", ":").Equals("none");
                else
                    STEP_9 = viewer.GetInnerAttribute(viewer.ViewerContainer(), "style", ';', "display", ":").Equals("none");

                //Step 9 :- Maximize viewer and validate it's display
                if (PatientInfo.Split(',')[0] == LastName && viewer.ReportFullScreenIcon().Displayed
                    && PatientInfo.Split(',')[1] == FirstName && PatientInfo.Split(',')[2] == PatientID &&
                    STEP_9)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                                
                //Click full screen icon again to restore half screen
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ReportFullScreenIcon()));
                viewer.ReportFullScreenIcon().Click();

                //Sync-up 
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ReportFullScreenIcon()));

                //Get width of study viewer container in half screen of report viewer
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                int ContainerWidth_3;
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                    ContainerWidth_3 = Int32.Parse(viewer.GetInnerAttribute(viewer.ViewerContainer(), "style", ';', "WIDTH", ":").Replace("px", "").Trim());
                else
                    ContainerWidth_3 = Int32.Parse(viewer.GetInnerAttribute(viewer.ViewerContainer(), "style", ';', "width", ":").Replace("px", "").Trim());
                
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);

                //Take Screenshot
                Boolean ViewerStatus_half = studies.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer()); 

                //Step 10 :- Validate report list,full screen report viewer icons and study panel with images are displayed properly
                if (viewer.ReportFullScreenIcon().Displayed && viewer.ViewerReportListButton().Displayed
                    && ContainerWidth_3 == ContainerWidth_2 && ViewerStatus_half)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //wait for report icon to be clickable
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.TitlebarReportIcon()));
                viewer.TitlebarReportIcon().Click();

                //Get width of study viewer container without report viewer
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                int ContainerWidth_4;
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                    ContainerWidth_4 = Int32.Parse(viewer.GetInnerAttribute(viewer.ViewerContainer(), "style", ';', "WIDTH", ":").Replace("px", "").Trim());
                else
                    ContainerWidth_4 = Int32.Parse(viewer.GetInnerAttribute(viewer.ViewerContainer(), "style", ';', "width", ":").Replace("px", "").Trim());
              
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);

                //Take Screenshot
                Boolean ViewerStatus_full = studies.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer()); 

                //Step 11 :- Validate report viewer is closed and images display correctly
                if (!viewer.ReportFullScreenIcon().Displayed && ContainerWidth_4 == ContainerWidth_1 && ViewerStatus_full)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close Study and logout iCA
                studies.CloseStudy();
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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

                if (ExecutedSteps >= 13) { BasePage.Driver.Manage().Window.Maximize(); }

                //Close Study and logout iCA
                studies.CloseStudy();
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary>
        /// Reports
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_28003(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Studies studies = null;
            StudyViewer viewer = null;
            DomainManagement domainmanagement = null;
            UserManagement usermanagement = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String UserName = Config.adminUserName;
                String Password = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionID = AccessionIDList.Split(':');   
                //String datasource = login.GetHostName(Config.SanityPACS);
                String NewUserName = "TestUser" + new Random().Next(1, 1000);
                String DomainName = "TestDomain" + new Random().Next(1, 1000);
                String EA91 = login.GetHostName(Config.EA91);
                String EA1 = login.GetHostName(Config.EA1);

                //Restore window size
                BasePage.Driver.Manage().Window.Size = new Size(1000,1000);

                //Login as Administrator
                login.LoginIConnect(UserName, Password);

                //Navigate to Domain Management tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");                             
                
                //Create new domain
                domainmanagement.CreateDomain(DomainName, DomainName + "Role",datasources: new string[] { EA91,EA1});
                domainmanagement.ClickSaveNewDomain();

                //Select domain
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);

                //Edit the report view option and Save
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("reportview", 0);
                domainmanagement.ClickSaveDomain();

                //Navigate to User Management tab
                usermanagement = (UserManagement)login.Navigate("UserManagement");

                //Create new user for the SuperAdmin domain                
                usermanagement.CreateUser(NewUserName, DomainName, DomainName + "Role");
                Boolean UserStatus = usermanagement.SearchUser(NewUserName, DomainName);               

                //Step 1:- Create new user with report viewing enabled
                if (UserStatus)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Logout as Administrator
                login.Logout();

                //sync up new user
                login.LoginIConnect(NewUserName, NewUserName);
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: AccessionID[1]);
                login.Logout();

                //Login as New User
                login.LoginIConnect(NewUserName, NewUserName);

                //Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Search, Select and Launch study               
                studies.SearchStudy(AccessionNo:AccessionID[1], Datasource: EA91);
                        
                studies.SelectStudy("Accession", AccessionID[1]);
                viewer = StudyViewer.LaunchStudy();

                Boolean TitlebarReportIcon;
                try
                {
                    //Click report icon in titlebar if available
                    if (viewer.TitlebarReportIcon().Displayed && viewer.TitlebarReportIcon().Enabled)
                    {
                        viewer.TitlebarReportIcon().Click();
                        TitlebarReportIcon = true;
                    }
                    TitlebarReportIcon = false;
                }
                catch(NoSuchElementException)
                {
                    TitlebarReportIcon = false;
                }

                //Step 2 :- Verify report icon is not available for study without report
                if (!TitlebarReportIcon)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Navigate to HistoryPanel
                viewer.NavigateToHistoryPanel();

                //Step 3 :- Check report icon is not enabled in Patient history tab
                if (!viewer.TabInHistoryPanel("Report").Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close Study
                studies.CloseStudy();

                //Clear existing search
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                BasePage.Driver.FindElement(By.CssSelector("#m_studySearchControl_m_clearButton")).Click();

                //Search, Select and Launch study               
                studies.SearchStudy(AccessionNo: AccessionID[0], Datasource: EA1);
                studies.SelectStudy("Accession", AccessionID[0]);
                viewer = StudyViewer.LaunchStudy();
                
                //wait for report icon to be clickable
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.TitlebarReportIcon()));

                //Click Report icon
                viewer.TitlebarReportIcon().Click();

                //wait for report viewer to gets loaded
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ReportFullScreenIcon()));

                //Switch to report frame
                viewer.SwitchToReportFrame("studypanel");

                decimal ViewerHeight;
                decimal ReportHeight; 
                decimal ViewerWidth;
                decimal ReportWidth;

                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                {

                    //Get report viewer height and report height
                    //Get report viewer width and report width
                    //ViewerHeight = BasePage.Driver.FindElement(By.CssSelector("div#ViewerContainer_Content")).Size.Height;
                    //ViewerWidth = BasePage.Driver.FindElement(By.CssSelector("div#ViewerContainer_Content")).Size.Height;

                    //BasePage.Driver.SwitchTo().Frame(BasePage.Driver.FindElement(By.CssSelector("div#ViewerContainer_Content iframe")));

                    //ReportHeight = BasePage.Driver.FindElement(By.CssSelector(".radiologist_report")).Size.Height;
                    //ReportWidth = BasePage.Driver.FindElement(By.CssSelector(".radiologist_report")).Size.Width;

                    //Logger.Instance.InfoLog("ViewerHeight: " + ViewerHeight);
                    //Logger.Instance.InfoLog("ReportHeight: " + ReportHeight);
                    //Logger.Instance.InfoLog("ViewerWidth: " + ViewerWidth);
                    //Logger.Instance.InfoLog("ReportWidth: " + ReportWidth);
                    //viewer.SwitchToReportFrame("studypanel");

                    result.steps[++ExecutedSteps].status = "Not Automated";
                }
                else
                {
                    //Get report viewer height and report height
                    ViewerHeight = decimal.Parse(((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function viewerHeight(){var viewer = window.document.querySelector(\"#ContentContainer object\");var viewer_height = window.document.defaultView.getComputedStyle(viewer , null).getPropertyValue('height');return viewer_height ;}return viewerHeight()").ToString().Replace("px", ""));
                    ReportHeight = decimal.Parse(((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function reportHeight(){var html = window.document.querySelector('#ContentContainer object').contentDocument.querySelector('html');var html_height = window.document.defaultView.getComputedStyle(html, null).getPropertyValue('height');return html_height;}return reportHeight()").ToString().Replace("px", ""));

                    //Get report viewer width and report width
                    ViewerWidth = decimal.Parse(((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function viewerHeight(){var viewer = window.document.querySelector(\"#ContentContainer object\");var viewer_height = window.document.defaultView.getComputedStyle(viewer , null).getPropertyValue('width');return viewer_height ;}return viewerHeight()").ToString().Replace("px", ""));
                    ReportWidth = decimal.Parse(((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function reportHeight(){var html = window.document.querySelector('#ContentContainer object').contentDocument.querySelector('html');var html_height = window.document.defaultView.getComputedStyle(html, null).getPropertyValue('width');return html_height;}return reportHeight()").ToString().Replace("px", ""));



                    Boolean ScrollbarStatus = false;
                    if ((ReportHeight > ViewerHeight && ViewerWidth > ReportWidth) ||
                        (ReportHeight <= ViewerHeight && ViewerWidth == ReportWidth))
                    {
                        ScrollbarStatus = true;
                    }

                    //Step 4 :- Verify Scroll bar presence for wrapped report
                    if (ScrollbarStatus)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //Get Scroll bar height in normal window
                int ScrollbarHeight_1, ScrollbarHeight_2;
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                {
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#MainContainer iframe")));
                    ScrollbarHeight_1 = Convert.ToInt32(((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function scrollHeight(){var height=document.querySelector('#ContentContainer iframe').scrollHeight;return height;}return scrollHeight();"));

                    //Maximize window size in maximized window
                    BasePage.Driver.Manage().Window.Maximize();

                    //Get Scroll bar height
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#MainContainer iframe")));
                    ScrollbarHeight_2 = Convert.ToInt32(((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function scrollHeight(){var height=document.querySelector('#ContentContainer iframe').scrollHeight;return height;}return scrollHeight();"));
                }
                else
                {
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#MainContainer object")));
                    ScrollbarHeight_1 = Convert.ToInt32(((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function scrollHeight(){var height=document.querySelector('#ContentContainer object').scrollHeight;return height;}return scrollHeight();"));

                    //Maximize window size in maximized window
                    BasePage.Driver.Manage().Window.Maximize();

                    //Get Scroll bar height
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#MainContainer object")));
                    ScrollbarHeight_2 = Convert.ToInt32(((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function scrollHeight(){var height=document.querySelector('#ContentContainer object').scrollHeight;return height;}return scrollHeight();"));
                }
                
                
                //Step 5 :- Maximize browser to full browser mode and check sidebar expansion with report viewer
                if (ScrollbarHeight_1 < ScrollbarHeight_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Resize window size
                BasePage.Driver.Manage().Window.Size = new Size(1000, 800);

                //Get Scroll bar height
                int ScrollbarHeight_3;
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                {
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#MainContainer iframe")));
                    ScrollbarHeight_3 = Convert.ToInt32(((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function scrollHeight(){var height=document.querySelector('#ContentContainer iframe').scrollHeight;return height;}return scrollHeight();"));
                }
                else
                {
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#MainContainer object")));
                    ScrollbarHeight_3 = Convert.ToInt32(((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function scrollHeight(){var height=document.querySelector('#ContentContainer object').scrollHeight;return height;}return scrollHeight();"));
                }
                
                //Step 6 :- Resize browser window size and check sidebar resize with report viewer
                if (ScrollbarHeight_3 < ScrollbarHeight_2)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Restore window size
                Size newSize = BasePage.Driver.Manage().Window.Size - new Size(15, 15);

                BasePage.Driver.Manage().Window.Size = newSize;
                PageLoadWait.WaitForPageLoad(10);
            
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#MainContainer iframe")));
                else
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#MainContainer object")));
                
                //Step 7 :- Check full browser mode is exited
                String browsername = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                if (browsername.Equals("chrome"))
                {
                    result.steps[++ExecutedSteps].status = "Not Automated";
                }
                else
                {
                    if (BasePage.Driver.Manage().Window.Size.Equals(newSize))
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //Get width of study viewer container in half screen of report viewer
                PageLoadWait.WaitForFrameLoad(10);
                int ContainerWidth_1;
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                    ContainerWidth_1 = Int32.Parse(viewer.GetInnerAttribute(viewer.ReportContainer(), "style", ';', "WIDTH", ":").Replace("px", "").Trim());
                else 
                    ContainerWidth_1 = Int32.Parse(viewer.GetInnerAttribute(viewer.ReportContainer(), "style", ';', "width", ":").Replace("px", "").Trim());


                //Maximize report viewer
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ReportFullScreenIcon()));
                viewer.ReportFullScreenIcon().Click();

                //Sync-up 
                PageLoadWait.WaitForPageLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ReportFullScreenIcon()));

                //Get width of study viewer container in full screen of report viewer
                int ContainerWidth_2;
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                    ContainerWidth_2 = Int32.Parse(viewer.GetInnerAttribute(viewer.ReportContainer(), "style", ';', "WIDTH", ":").Replace("px", "").Trim());
                else
                    ContainerWidth_2 = Int32.Parse(viewer.GetInnerAttribute(viewer.ReportContainer(), "style", ';', "width", ":").Replace("px", "").Trim());

                //Step 8 :- Open report in full screen and verify it's correctness
                if (!viewer.ViewerContainer().Displayed && viewer.ReportFullScreenIcon().Displayed 
                    && ContainerWidth_2 > ContainerWidth_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Get Current working window
                var currentWindow = BasePage.Driver.CurrentWindowHandle;

                //Select Print view
                viewer.SelectToolInToolBar("PrintView");

                var newwindow = "";
                foreach (var window in BasePage.Driver.WindowHandles)
                {
                    BasePage.Driver.SwitchTo().Window(window);
                    Thread.Sleep(2000);
                    String url = BasePage.Driver.Url;
                    if (url.Contains("PrintView"))
                    {
                        newwindow = window;
                        break;
                    }
                }
                                
                //Navigate to print window
                BasePage.Driver.SwitchTo().Window(newwindow);

                //Step 9 :- Validate new window showing print dialog is displayed 
                if (viewer.PrintButton().Enabled)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 10 :- Non Automated step -- Print the report
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Close print window and navigate to main window
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(currentWindow);

                //Syn-up
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.TitlebarReportIcon()));

                //Click Full screen icon to open image view
                viewer.TitlebarReportIcon().Click();

                //Sync-up
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                 
                //Step 11 :- Validate normal window showing viewports without report are displayed
                if (viewer.ViewerContainer().Displayed && !viewer.ReportFullScreenIcon().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close Study and logout iCA
                studies.CloseStudy();
                login.Logout();
                                
                //Login as Administrator
                login.LoginIConnect(UserName, Password);

                //Navigate to Domain Management tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");

                //Search and Select domain
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);

                //Step 12 :- Disable report viewing for Test user domain as Administrator and save
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("reportview", 1);
                domainmanagement.ClickSaveDomain();
                ExecutedSteps++;

                //Logout iCA
                login.Logout();

                //Login as New User
                login.LoginIConnect(NewUserName, NewUserName);

                //Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Search, Select and Launch a study without report
                studies.SearchStudy(AccessionNo:AccessionID[1],Datasource:EA91);
                studies.SelectStudy("Accession", AccessionID[1]);
                viewer = StudyViewer.LaunchStudy();

                Boolean ReportIcon;
                try
                {
                    if(viewer.TitlebarReportIcon().Displayed)
                    {
                        ReportIcon = true;
                    }
                    ReportIcon = false;
                }
                catch(NoSuchElementException)
                {
                    ReportIcon = false;
                }

                //Step 13 :- Check report icon is not displayed for study without report
                if (!ReportIcon)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close Study 
                studies.CloseStudy();

                //Clear existing search
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                BasePage.Driver.FindElement(By.CssSelector("#m_studySearchControl_m_clearButton")).Click();

                //Search, Select and Launch study with report
                studies.SearchStudy(AccessionNo: AccessionID[0], Datasource: EA1);
                studies.SelectStudy("Accession", AccessionID[0]);
                viewer = StudyViewer.LaunchStudy();

                Boolean ReportIcon1;
                try
                {
                    if (viewer.TitlebarReportIcon().Displayed)
                    {
                        ReportIcon1 = true;
                    }
                    ReportIcon1 = false;
                }
                catch (NoSuchElementException)
                {
                    ReportIcon1 = false;
                }

                //Step 14 :- Check report icon is not displayed for study with report
                if (!ReportIcon1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close Study and logout iCA
                studies.CloseStudy();
                login.Logout();

                //Login as Administrator
                login.LoginIConnect(UserName, Password);

                //Navigate to Domain Management tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");

                //Search and Select domain
                domainmanagement.SearchDomain(DomainName);
                domainmanagement.SelectDomain(DomainName);

                //Step 15 :- Re-enable report viewing for Test user domain as Administrator and save
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("reportview", 0);
                domainmanagement.ClickSaveDomain();
                ExecutedSteps++;

                //Navigate to User Management tab
                usermanagement = (UserManagement)login.Navigate("UserManagement");

                //Search and Select new user
                usermanagement.SearchUser(NewUserName, DomainName);
                usermanagement.SelectUser(NewUserName);

                //Step 16 :- Add Access Filter to the user and click save
                usermanagement.ClickEditUser();
                usermanagement.SetAccessFilter("Patient ID", PatientID);
                usermanagement.SaveBtn().Click();
                ExecutedSteps++;

                //Logout iCA
                login.Logout();

                //Login as New User
                login.LoginIConnect(NewUserName, NewUserName);

                //Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Search Study
                studies.SearchStudy(AccessionNo: "");

                //Step 17 :- Check filtered study only present
                if (BasePage.GetSearchResults().Count == 1 &&
                    studies.GetMatchingRow("Accession", AccessionID[0]) != null)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select and Launch study
                studies.SelectStudy("Accession", AccessionID[0]);
                viewer = StudyViewer.LaunchStudy();

                //Step 18 :- Check report icon is displayed in title bar   
                if (viewer.TitlebarReportIcon().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Click report icon
                viewer.TitlebarReportIcon().Click();
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id$='studyPanel_1_reportViewerContainer']")));

                //Get Report details
                String reportMRN = viewer.ReportDetails("studypanel")["MRN"];
                PageLoadWait.WaitForFrameLoad(10);

                //Step 19 :- Validate report for the study is opened in left half of study panel
                if (viewer.ReportContainer().Displayed && reportMRN.Equals(PatientID))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Get width of report viewer
                 int ReportViewerWidth_1;
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                    ReportViewerWidth_1 = Int32.Parse(viewer.GetInnerAttribute(viewer.ReportContainer(), "style", ';', "WIDTH", ":").Replace("px", "").Trim());
                else
                    ReportViewerWidth_1 = Int32.Parse(viewer.GetInnerAttribute(viewer.ReportContainer(), "style", ';', "width", ":").Replace("px", "").Trim());

                //Maximize report viewer
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ReportFullScreenIcon()));
                viewer.ReportFullScreenIcon().Click();

                //Sync-up 
                PageLoadWait.WaitForPageLoad(10);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ReportFullScreenIcon()));

                //Get width of report viewer
                int ReportViewerWidth_2; 
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                    ReportViewerWidth_2 = Int32.Parse(viewer.GetInnerAttribute(viewer.ReportContainer(), "style", ';', "WIDTH", ":").Replace("px", "").Trim());
                else
                    ReportViewerWidth_2 = Int32.Parse(viewer.GetInnerAttribute(viewer.ReportContainer(), "style", ';', "width", ":").Replace("px", "").Trim());

                //Step 20 :- Maximize viewer and validate it's display

                bool Step_20 = false;
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                    Step_20 = viewer.GetInnerAttribute(viewer.ViewerContainer(), "style", ';', "DISPLAY", ":").Equals("none");
                else
                    Step_20 = viewer.GetInnerAttribute(viewer.ViewerContainer(), "style", ';', "display", ":").Equals("none");

                if (Step_20
                    && ReportViewerWidth_2 > ReportViewerWidth_1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                
                //Close Study and logout iCA
                studies.CloseStudy();
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Maximize window size
                BasePage.Driver.Manage().Window.Maximize();

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

                //Maximize window size
                BasePage.Driver.Manage().Window.Maximize();

                //Close Study and logout iCA
                //login.CloseStudy();
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                //Maximize window size
                BasePage.Driver.Manage().Window.Maximize();
            }

        }

        /// <summary>
        /// Report with Audio
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_28004(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Studies studies = null;
            StudyViewer viewer = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String UserName = Config.adminUserName;
                String Password = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String EA77 = login.GetHostName(Config.EA77);

                //Step 1:- Initail Setup - Prepare Study with SR report and audio
                ExecutedSteps++;

                //Login as Administrator
                login.LoginIConnect(UserName, Password);

                //Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Search, Select study               
                studies.SearchStudy(AccessionNo: AccessionID,Datasource:EA77);
                studies.SelectStudy("Accession", AccessionID);

                //Launch study in viewer
                viewer = StudyViewer.LaunchStudy();
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[6]));
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.Thumbnails()[7]));
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForThumbnailsToLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //Take Screenshot
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                Boolean ViewerStatus = studies.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                //Step 2 :- Lauch study and verify it's correctness
                if (ViewerStatus)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //wait for report icon to be clickable
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.TitlebarReportIcon()));

                //Click report icon
                viewer.TitlebarReportIcon().Click();
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div[id$='studyPanel_1_reportViewerContainer']")));

                //Select report list button
                viewer.ViewerReportListButton().Click();
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div[id$='studyPanel_1_m_reportViewer_reportListContainer']")));
                
                //Get Report list details
                Dictionary<int, string[]> ReportListDetails = viewer.StudyViewerListResults("StudyPanel", "report", 1);
                String[] ReportListColumns = viewer.StudyViewerListColumnNames("StudyPanel", "report", 1);
                String[] ReportListColumnValues = BasePage.GetColumnValues(ReportListDetails, "Type", ReportListColumns);

                //Step 3:- Validate list with SR report and audio file is displayed
                if (Array.Exists(ReportListColumnValues, type => type.EndsWith("SR"))
                    && Array.Exists(ReportListColumnValues, type => type.EndsWith("AU")))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Get report type
                string SRreport = "";
                foreach(int key in ReportListDetails.Keys)
                {
                    SRreport = Array.Find(ReportListDetails[key], t => t.EndsWith("SR"));
                    if(SRreport != null)
                    {
                        break;
                    }
                }
                             
                //Get SR report's row details
                Dictionary<string, string> SRreportDetails = viewer.StudyViewerListMatchingRow("Type", SRreport, "StudyPanel", "report");

                //Select report with SR type
                viewer.SelectItemInStudyViewerList("Type", SRreport, "StudyPanel", "report");

                //wait for report list button to be clickable
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ViewerReportListButton()));

                //Get Report details
                Dictionary<string, string> ReportDetails = viewer.ReportDetails("studypanel");

                //Step 4 :- Verify SR report opens in study panel
                if (ReportDetails["MRN"].Equals(PatientID))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Select report list button
                PageLoadWait.WaitForFrameLoad(10);
                viewer.ViewerReportListButton().Click();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id$='studyPanel_1_m_reportViewer_reportListContainer']")));
                
                string AUreport = "";
                foreach(int key in ReportListDetails.Keys)
                {
                    AUreport = Array.Find(ReportListDetails[key], t => t.EndsWith("AU"));
                    if(AUreport != null)
                    {
                        break;
                    }
                }

                String browsername = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                if (browsername.Equals("chrome"))
                {

                    //Select report with SR type
                    viewer.SelectItemInStudyViewerList("Type", AUreport, "StudyPanel", "report");

                    //wait for report list button to be clickable
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ViewerReportListButton()));

                    //Play Audio and get audio duration & status
                    viewer.SwitchToReportFrame("studypanel");
                    double duration = (double)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function AudioDuration(){var audio = document.querySelector('#Audio_Display_Div>audio');audio.play();return audio.duration;}return AudioDuration();");
                    Boolean IsAudioPlaying = (Boolean)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function AudioStatus(){var audio = document.querySelector('#Audio_Display_Div>audio');return audio.ended;}return AudioStatus();");

                    double timer = 0;
                    while (duration > timer++)
                    {
                        Thread.Sleep(1000);
                    }

                    //Get audio status
                    Boolean IsAudioEnded = (Boolean)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function AudioStatus(){var audio = document.querySelector('#Audio_Display_Div>audio');return audio.ended;}return AudioStatus();");

                    //Step 5:- Play Audio and verify it's correctness
                    if (!IsAudioPlaying && IsAudioEnded)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Not Automated";
                }

                //Close Study and logout iCA
                PageLoadWait.WaitForFrameLoad(10);
                studies.CloseStudy();
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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

                //Maximize window
                BasePage.Driver.Manage().Window.Maximize();

                //Close Study and logout iCA
                studies.CloseStudy();
                login.Logout();

                //Return Result
                return result;
            }
            finally
            {
                //Maximize window
                BasePage.Driver.Manage().Window.Maximize();
            }

        }

        /// <summary>
        /// Requisition Form
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_28005(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Studies studies = null;
            StudyViewer viewer = null;
            DomainManagement domainmanagement;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String UserName = Config.adminUserName;
                String Password = Config.adminPassword;                
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String StudyID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");               
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String PACSA7 = login.GetHostName(Config.SanityPACS);
               
               
                /*Enabling Requisition checkbox in DomianManagement*/
                login.DriverGoTo(login.url);
                login.LoginIConnect(UserName, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("requisitionreport", 0);
                domainmanagement.ClickSaveDomain();
                login.Logout();

                //Step-1:
                /*Precondition--> Edit the file WebAccessConfiguration.xml file by uncommenting the requisition category section.*/
                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step-2:Login as Administrator
                login.DriverGoTo(login.url);
                login.LoginIConnect(UserName, Password);

                //Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Search,Select,Launch study
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID: StudyID, Datasource: PACSA7);
                studies.SelectStudy1("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();

                //Verify whether study loads into viewer
                if (viewer.ViewStudy())
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3:Select Requisition button and validate
                PageLoadWait.WaitForFrameLoad(20);
                viewer.RequisitionIcon().Click();
                Thread.Sleep(40000);
                PageLoadWait.WaitForThumbnailsToLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForFrameLoad(180);
                IWebElement leftside = BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_m_requisitionViewer_m_compositeViewer_SeriesViewer_1_1_viewerImg"));
                if (leftside.Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4:Click on Printable view link
                var currentWindow = BasePage.Driver.CurrentWindowHandle;
                viewer.SelectToolInToolBar(StudyViewer.ViewerTools.PrintView, "review");
                var newwindow = BasePage.Driver.WindowHandles.Last();
                //BasePage.Driver.SwitchTo().Window(newwindow);
                //BasePage.Driver.Close();
                //var newwindow1 = BasePage.Driver.WindowHandles.Last();
                int count = 0;
                while (newwindow == currentWindow)
                {
                    if (count > 20)
                    {
                        throw new Exception("Error in Print Window");
                    }

                    foreach (var window in BasePage.Driver.WindowHandles)
                    {
                        BasePage.Driver.SwitchTo().Window(window);
                        if (BasePage.Driver.Url.Contains("OperationClass=imagePrintView"))
                        {
                            newwindow = window;
                            break;
                        }
                    }

                    Thread.Sleep(1000);
                    count++;

                }

                BasePage.Driver.SwitchTo().Window(newwindow);
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div[class='hidePrint']")));
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#PrintButton")));
                IWebElement Image = BasePage.Driver.FindElement(By.CssSelector("#viewerImg_1_1"));

                if (viewer.PrintButton().Enabled && Image.Displayed)
                {
                    Logger.Instance.InfoLog("*****Print Window opened with the image*****");
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);

                }
                else
                {

                    Logger.Instance.InfoLog("******Error in Print WIndow*******");
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();

                }

                BasePage.Driver.Close();
                var existingwindow = BasePage.Driver.WindowHandles.Last();
                BasePage.Driver.SwitchTo().Window(existingwindow);
                BasePage.Driver.Close();
                BasePage.Driver.SwitchTo().Window(currentWindow);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();

                //result.steps[++ExecutedSteps].status = "In Hold";

                viewer.CloseStudy();
                login.Logout();


                //Step-5:Click on Print
                result.steps[++ExecutedSteps].status = "Not Automated";
                //Step-6:Value ORDONNACE is deleted from the tag
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                /*   //Step-7:Login as Administrator
                   login.DriverGoTo(login.url);
                   login.LoginIConnect(UserName, Password);

                   //Navigate to Studies
                   studies = (Studies)login.Navigate("Studies");

                   //Search,Select,Launch study
                   studies.ChooseColumns(new string[] { "Study ID" });
                   studies.SearchStudy("Study ID", StudyID);
                   studies.SelectStudy1("Study ID", StudyID);
                   viewer = StudyViewer.LaunchStudy();

                   //Verify whether study loads into viewer
                   if (studies.ViewStudy())
                   {
                       result.steps[++ExecutedSteps].status = "Pass";
                       Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                   }
                   else
                   {
                       result.steps[++ExecutedSteps].status = "Fail";
                       Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                       result.steps[ExecutedSteps].SetLogs();
                   }

                   //Step-8:Select Requisition button and validate
                   PageLoadWait.WaitForFrameLoad(20);
                   viewer.RequisitionIcon().Click();
                   PageLoadWait.WaitForFrameLoad(30);
                   IWebElement leftside0 = BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_m_requisitionViewer_m_compositeViewer_SeriesViewer_1_1_viewerImg"));
                   if (leftside0.Displayed == false)
                   {
                       result.steps[++ExecutedSteps].status = "Pass";
                       Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                   }
                   else
                   {
                       result.steps[++ExecutedSteps].status = "Fail";
                       Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                       result.steps[ExecutedSteps].SetLogs();
                   }

                   viewer.CloseStudy();
                   login.Logout(); */



                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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
        }

        /// <summary>
        /// Attachment
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_28006(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Studies studies = null;
            StudyViewer viewer = null;
            DomainManagement domainmanagement;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String UserName = Config.adminUserName;
                String Password = Config.adminPassword;
                String phUsername = Config.ph1UserName;
                String phPassword = Config.ph1Password;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] PatientIds = PatientID.Split(':');
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String StudyID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");                
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String FilePaths = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AttachmentPath");
                String[] Filepath = FilePaths.Split('=');
                String Modality = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");                
                String EA131 = login.GetHostName(Config.EA1);
                String PACSA7 = login.GetHostName(Config.SanityPACS);
                WebDriverWait WAIT = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 120000));

                /*Enabling attachment checkboxes in DomianManagement*/
                login.DriverGoTo(login.url);
                login.LoginIConnect(UserName, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("attachment", 0);
                domainmanagement.SetCheckBoxInEditDomain("attachmentupload", 0);
                domainmanagement.SetCheckBoxInEditDomain("requisitionreport", 1);
                domainmanagement.ClickSaveDomain();
                login.Logout();


                //Step-1:Login as TestUser(physician)
                login.LoginIConnect(phUsername, phPassword);
                ExecutedSteps++;

                //Step-2:Load any study which is available in EA data source.
                //Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Search,Select,Launch study
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID: StudyID, Datasource: EA131);
                studies.SelectStudy1("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();

                //Verify whether study loads into viewer
                if (viewer.ViewStudy())
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3:Click on History tab and expand Attachment menu.
                viewer.NavigateToHistoryPanel();
                viewer.NavigateTabInHistoryPanel("Attachment");

                //Validate the display
                bool UploadTitle = viewer.UploadLabel().Displayed;
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("iframeAttachment");
                if (UploadTitle == true && viewer.ChooseFileBtn().Displayed && viewer.AttachmentSaveBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4:Edit the Text field and validate that it cannot be edited
                //viewer.BrowseAttachment().SendKeys("Log");
                if (!(viewer.BrowseAttachment().GetAttribute("type").Equals("text")))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-5:Select Save button and validate the display
                viewer.AttachmentSaveBtn().Click();
                //BasePage.Driver.SwitchTo().DefaultContent();
                //BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                PageLoadWait.WaitForFrameLoad(10);
                if (viewer.ErrorMessage().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step-6 to 14 in IE
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("iframeAttachment");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ChooseFileBtn()));
                String BrowserName = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                if (BrowserName.Equals("internet explorer"))
                {
                    //Step-6 to 8:
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("iframeAttachment");
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ChooseFileBtn()));
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ChooseFileBtn()));
                    viewer.ChooseFileBtn().SendKeys(Filepath[0]);
                    ExecutedSteps++;
                    PageLoadWait.WaitForFrameLoad(10);
                    BasePage.Driver.SwitchTo().Frame("iframeAttachment");
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.AttachmentSaveBtn()));
                    if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                        viewer.AttachmentSaveBtn().Click();
                    else
                        ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"input#m_sendAttachmentButton\").click()");
                    
                    int counter1 = 0;
                    counter1 = 0;
                    PageLoadWait.WaitForFrameLoad(10);
                    if (viewer.AttachmentUploadIcon().Displayed)
                    {
                        while (viewer.AttachmentUploadIcon().Displayed && counter1++ < 10)
                        {
                            Thread.Sleep(60000);
                            PageLoadWait.WaitForFrameLoad(20);

                        }
                    }
                    ExecutedSteps++;
                    ExecutedSteps++;
                    //Step-9 to 11:
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("iframeAttachment");
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ChooseFileBtn()));
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ChooseFileBtn()));
                    viewer.ChooseFileBtn().SendKeys(Filepath[1]);
                    Thread.Sleep(5000);
                    ExecutedSteps++;
                    PageLoadWait.WaitForFrameLoad(10);
                    BasePage.Driver.SwitchTo().Frame("iframeAttachment");
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.AttachmentSaveBtn()));
                    if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                        viewer.AttachmentSaveBtn().Click();
                    else
                        ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"input#m_sendAttachmentButton\").click()");
                    
                    int counter = 0;
                    counter = 0;
                    PageLoadWait.WaitForFrameLoad(10);
                    if (viewer.AttachmentUploadIcon().Displayed)
                    {
                        while (viewer.AttachmentUploadIcon().Displayed && counter++ < 10)
                        {
                            Thread.Sleep(60000);
                            PageLoadWait.WaitForFrameLoad(20);

                        }
                    }
                    ExecutedSteps++;
                    ExecutedSteps++;
                    //Step-12 to 14:
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("iframeAttachment");
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ChooseFileBtn()));
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ChooseFileBtn()));
                    viewer.ChooseFileBtn().SendKeys(Filepath[0]);
                    ExecutedSteps++;
                    PageLoadWait.WaitForFrameLoad(10);
                    BasePage.Driver.SwitchTo().Frame("iframeAttachment");
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.AttachmentSaveBtn()));
                    if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                        viewer.AttachmentSaveBtn().Click();
                    else
                        ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"input#m_sendAttachmentButton\").click()");
                    
                    int counter13 = 0;
                    counter13 = 0;
                    PageLoadWait.WaitForFrameLoad(10);
                    if (viewer.AttachmentUploadIcon().Displayed)
                    {
                        while (viewer.AttachmentUploadIcon().Displayed && counter13++ < 10)
                        {
                            Thread.Sleep(60000);
                            PageLoadWait.WaitForFrameLoad(20);

                        }
                    }
                    ExecutedSteps++;
                    ExecutedSteps++;
                }
                else
                {
                    //Step-6:Select browse button and validate the dialog window displayed(Chrome and Firefox)
                    //Click Choose file button
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("iframeAttachment");
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ChooseFileBtn()));
                    viewer.ChooseFileBtn().Click();


                    //((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#inputAttachment\").click()");

                    //Get the main window
                    Window mainWindow = null;
                    IList<Window> windows = TestStack.White.Desktop.Instance.Windows(); //Get all the windows on desktop
                    String browsername = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                    for (int i = 0; i < windows.Count; i++)
                    {
                        string str = windows[i].Title.ToLower();
                        if (str.Contains(browsername)) //compare which window title is matching to your string
                        {
                            mainWindow = windows[i];
                            Logger.Instance.InfoLog("Window with title " + str + " is set as the working window");
                            break;
                        }
                    }
                    mainWindow.WaitWhileBusy();

                    String uploadWindowName = "";
                    if (mainWindow.Enabled)
                    {
                        if (browsername.Equals("chrome"))
                        {
                            uploadWindowName = "Open";
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else if (browsername.Equals("firefox"))
                        {
                            uploadWindowName = "File Upload";
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }



                    //Step-7:Select any file to upload and validate the file name displayed
                    //Get Upload window
                    Window UploadWindow = mainWindow.ModalWindow(uploadWindowName);
                    UploadWindow.WaitWhileBusy();

                    var editBox = UploadWindow.Get<TestStack.White.UIItems.TextBox>(SearchCriteria.ByText("File name:"));
                    editBox.SetValue(Filepath[0]);
                    UploadWindow.WaitWhileBusy();
                    Thread.Sleep(20000);

                    //Click Open button
                    try
                    {
                        var openBtn = UploadWindow.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("1"));
                        openBtn.Click();
                    }
                    catch (Exception)
                    {
                        var openBtn = UploadWindow.Get<TestStack.White.UIItems.Panel>(SearchCriteria.ByAutomationId("1"));
                        openBtn.Click();
                    }

                    //Sync-up
                    int counter = 0;
                    while (UploadWindow.Visible && counter++ < 10)
                    {
                        Thread.Sleep(1000);
                    }
                    mainWindow.WaitWhileBusy();
                    String filename = Filepath[0].Split('\\')[Filepath[0].Split('\\').Length - 1];
                    String Browsername = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                    if (Browsername.Equals("chrome"))
                    {
                        String file = (String)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function Filename(){var name=document.querySelector('input#inputAttachment').value;return name;}return Filename()");
                        String[] FileName = file.Split('\\');

                        if (filename.Equals(FileName[2]))
                        {
                            Logger.Instance.InfoLog("In Chrome");
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            Logger.Instance.InfoLog("In Chrome");
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }

                    }
                    else if (Browsername.Equals("firefox"))
                    {
                        String file = (String)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function Filename(){var name=document.querySelector('input#inputAttachment').value;return name;}return Filename()");
                        if (filename.Equals(file))
                        {
                            Logger.Instance.InfoLog("In Firefox");
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            Logger.Instance.InfoLog("In Firefox");
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }
                    }
                    else
                    {
                        Logger.Instance.InfoLog("In Firefox");
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }




                //Step-8:Select Save button and validate the display
                // viewer.AttachmentSaveBtn().Click();
                    if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                        viewer.AttachmentSaveBtn().Click();
                    else
                        ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"input#m_sendAttachmentButton\").click()");
                    
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    PageLoadWait.WaitForPageLoad(40);
                    PageLoadWait.WaitForFrameLoad(40);                    
                    WAIT.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("img#imgUpLoadProgress")));
                    Thread.Sleep(60000); 

                    Dictionary<string, string> Filerow = viewer.StudyViewerListMatchingRow("Name", filename, "patienthistory", "attachment");

                    if (Filerow != null)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Step-9:Select browse button and validate the dialog window displayed
                    //Click Choose file button
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("iframeAttachment");
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ChooseFileBtn()));
                    viewer.ChooseFileBtn().Click();

                    //Get the main window
                    Window mainWindow1 = null;
                    IList<Window> windows1 = TestStack.White.Desktop.Instance.Windows(); //Get all the windows on desktop
                    String browsername1 = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                    for (int i = 0; i < windows1.Count; i++)
                    {
                        string str = windows1[i].Title.ToLower();
                        if (str.Contains(browsername1)) //compare which window title is matching to your string
                        {
                            mainWindow1 = windows1[i];
                            Logger.Instance.InfoLog("Window with title " + str + " is set as the working window");
                            break;
                        }
                    }
                    mainWindow1.WaitWhileBusy();

                    String uploadWindowName1 = "";
                    if (mainWindow1.Enabled)
                    {
                        if (browsername1.Equals("chrome"))
                        {
                            uploadWindowName1 = "Open";
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else if (browsername.Equals("firefox"))
                        {
                            uploadWindowName1 = "File Upload";
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }



                    //Step-10:Select any file to upload and validate the file name displayed
                    //Get Upload window
                    Window UploadWindow1 = mainWindow1.ModalWindow(uploadWindowName1);
                    UploadWindow1.WaitWhileBusy();

                    var editBox1 = UploadWindow1.Get<TestStack.White.UIItems.TextBox>(SearchCriteria.ByText("File name:"));
                    editBox1.SetValue(Filepath[1]);
                    UploadWindow1.WaitWhileBusy();
                    Thread.Sleep(20000);

                    //Click Open button
                    try
                    {
                        var openBtn0 = UploadWindow1.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("1"));
                        openBtn0.Click();
                    }
                    catch (Exception)
                    {
                        var openBtn0 = UploadWindow1.Get<TestStack.White.UIItems.Panel>(SearchCriteria.ByAutomationId("1"));
                        openBtn0.Click();
                    }

                    //Sync-up
                    int counter1 = 0;
                    while (UploadWindow1.Visible && counter1++ < 10)
                    {
                        Thread.Sleep(1000);
                    }
                    mainWindow1.WaitWhileBusy();
                    String filename1 = Filepath[1].Split('\\')[Filepath[1].Split('\\').Length - 1];
                    String Browsername1 = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                    if (Browsername1.Equals("chrome"))
                    {
                        String file = (String)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function Filename(){var name=document.querySelector('input#inputAttachment').value;return name;}return Filename()");
                        String[] FileName = file.Split('\\');

                        if (filename1.Equals(FileName[2]))
                        {
                            Logger.Instance.InfoLog("In Chrome");
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            Logger.Instance.InfoLog("In Chrome");
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }

                    }
                    else if (Browsername1.Equals("firefox"))
                    {
                        String file = (String)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function Filename(){var name=document.querySelector('input#inputAttachment').value;return name;}return Filename()");
                        if (filename1.Equals(file))
                        {
                            Logger.Instance.InfoLog("In Firefox");
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            Logger.Instance.InfoLog("In Firefox");
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }




                //Step-11:Select Save button and validate the display
                //viewer.AttachmentSaveBtn().Click();
                    if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                        viewer.AttachmentSaveBtn().Click();
                    else
                        ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"input#m_sendAttachmentButton\").click()");
                    
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    WAIT.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("img#imgUpLoadProgress")));
                    Thread.Sleep(60000); 

                    Dictionary<string, string> Filerow1 = viewer.StudyViewerListMatchingRow("Name", filename1, "patienthistory", "attachment");

                    if (Filerow1 != null)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }

                    //Step-12:Select browse button and validate the dialog window displayed
                    //Click Choose file button
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("iframeAttachment");
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ChooseFileBtn()));
                    viewer.ChooseFileBtn().Click();

                    //Get the main window
                    Window mainWindow2 = null;
                    IList<Window> windows2 = TestStack.White.Desktop.Instance.Windows(); //Get all the windows on desktop
                    String browsername2 = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                    for (int i = 0; i < windows2.Count; i++)
                    {
                        string str = windows2[i].Title.ToLower();
                        if (str.Contains(browsername2)) //compare which window title is matching to your string
                        {
                            mainWindow2 = windows2[i];
                            Logger.Instance.InfoLog("Window with title " + str + " is set as the working window");
                            break;
                        }
                    }
                    mainWindow2.WaitWhileBusy();

                    String uploadWindowName2 = "";
                    if (mainWindow2.Enabled)
                    {
                        if (browsername2.Equals("chrome"))
                        {
                            uploadWindowName2 = "Open";
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else if (browsername2.Equals("firefox"))
                        {
                            uploadWindowName2 = "File Upload";
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }



                    //Step-13:Select Same file to upload and validate the file name displayed
                    //Get Upload window
                    Window UploadWindow2 = mainWindow.ModalWindow(uploadWindowName);
                    UploadWindow2.WaitWhileBusy();

                    var editBox2 = UploadWindow2.Get<TestStack.White.UIItems.TextBox>(SearchCriteria.ByText("File name:"));
                    editBox2.SetValue(Filepath[0]);
                    UploadWindow2.WaitWhileBusy();
                    Thread.Sleep(20000);

                    //Click Open button
                    try
                    {
                        var openBtn1 = UploadWindow2.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("1"));
                        openBtn1.Click();
                    }
                    catch (Exception)
                    {
                        var openBtn1 = UploadWindow2.Get<TestStack.White.UIItems.Panel>(SearchCriteria.ByAutomationId("1"));
                        openBtn1.Click();
                    }

                    //Sync-up
                    int counter2 = 0;
                    while (UploadWindow2.Visible && counter2++ < 10)
                    {
                        Thread.Sleep(1000);
                    }
                    mainWindow2.WaitWhileBusy();
                    String filename2 = Filepath[0].Split('\\')[Filepath[0].Split('\\').Length - 1];
                    String Browsername2 = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                    if (Browsername2.Equals("chrome"))
                    {
                        String file = (String)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function Filename(){var name=document.querySelector('input#inputAttachment').value;return name;}return Filename()");
                        String[] FileName = file.Split('\\');

                        if (filename2.Equals(FileName[2]))
                        {
                            Logger.Instance.InfoLog("In Chrome");
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            Logger.Instance.InfoLog("In Chrome");
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }

                    }
                    else if (Browsername2.Equals("firefox"))
                    {
                        String file = (String)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function Filename(){var name=document.querySelector('input#inputAttachment').value;return name;}return Filename()");
                        if (filename2.Equals(file))
                        {
                            Logger.Instance.InfoLog("In Firefox");
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            Logger.Instance.InfoLog("In Firefox");
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }




                //Step-14:Select Save button and validate the display
                //viewer.AttachmentSaveBtn().Click();
                    if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                        viewer.AttachmentSaveBtn().Click();
                    else
                        ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"input#m_sendAttachmentButton\").click()");
                    
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    WAIT.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("img#imgUpLoadProgress")));
                    Thread.Sleep(60000); 

                    Dictionary<string, string> Filerow2 = viewer.StudyViewerListMatchingRow("Name", filename2, "patienthistory", "attachment");

                    if (Filerow2 != null)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                }

                //Step-15 to 20 (Not Automated)
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                login.Logout();

                //Step-21:Login as Administrator and disable 'Attachment uploading'
                login.LoginIConnect(UserName, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("attachment", 1);
                domainmanagement.SetCheckBoxInEditDomain("attachmentupload", 1);
                domainmanagement.ClickSaveDomain();
                login.Logout();
                ExecutedSteps++;


                //Step-22:Login as Testuser(physician)
                login.LoginIConnect(phUsername, phPassword);
                //Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Search,Select,Launch study
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID: StudyID, Datasource: EA131);
                studies.SelectStudy1("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();

                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (viewer.AttachmentSection().Displayed == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-23:Login as Administrator and re-enable 'attachment uploading'
                login.LoginIConnect(UserName, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("attachment", 0);
                domainmanagement.SetCheckBoxInEditDomain("attachmentupload", 0);
                domainmanagement.ClickSaveDomain();
                login.Logout();

                //Login as Testuser(physician)
                login.LoginIConnect(phUsername, phPassword);
                //Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Search,Select,Launch study
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID: StudyID, Datasource: EA131);
                studies.SelectStudy1("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (viewer.AttachmentSection().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                login.Logout();

                //Step-24:Login to the EA Webadmin.
                login.DriverGoTo(login.GetEAUrl(Config.EA1));
                hplogin.LoginHPen(Config.hpUserName, Config.hpPassword, EA_URL: login.GetEAUrl(Config.EA1));
                WorkFlow workflow = (WorkFlow)hphomepage.Navigate("Workflow");
                workflow.NavigateToLink("Workflow", "Archive Search");
                ExecutedSteps++;

                //Step-25:Search for the studies and verify that the attachment is saved under the study selected.
                workflow.HPSearchStudy("PatientID", PatientIds[0]);
                workflow.HPSearchStudy("Modality", Modality);
                Dictionary<string, string> studyresults = workflow.GetStudyDetailsInHP();

                //CLick on Patient ID to verify the Attachment presence                
                Dictionary<int, string[]> seriesresults = workflow.GetSeriesDetailsInHP(StudyID);
               
                if (studyresults["Study ID"].Equals(StudyID) && seriesresults.Count>1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-26:Close EA Webadmin
                hplogin.LogoutHPen();
                ExecutedSteps++;

                //Step-27:Load any study which is available in PACS data source.
                login.DriverGoTo(login.url);
                login.LoginIConnect(phUsername, phPassword);                
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession",AccessionID);
                studies.SelectStudy1("Accession", AccessionID);
                viewer = StudyViewer.LaunchStudy();               
                if (viewer.ViewStudy())
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-28:Click on History tab and expand Attachment menu.
                viewer.NavigateToHistoryPanel();
                viewer.NavigateTabInHistoryPanel("Attachment");

                //Validate the display
                bool UploadTitle28 = viewer.UploadLabel().Displayed;
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("iframeAttachment");
                if (UploadTitle28 == true && viewer.ChooseFileBtn().Displayed && viewer.AttachmentSaveBtn().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-29 to 31:
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("iframeAttachment");
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ChooseFileBtn()));
                if (BrowserName.Equals("internet explorer"))
                {
                    //Step-29 to 31:
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("iframeAttachment");
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ChooseFileBtn()));
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ChooseFileBtn()));
                    viewer.ChooseFileBtn().SendKeys(Filepath[0]);
                    ExecutedSteps++;
                    PageLoadWait.WaitForFrameLoad(10);
                    BasePage.Driver.SwitchTo().Frame("iframeAttachment");
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.AttachmentSaveBtn()));
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"input#m_sendAttachmentButton\").click()");
                    int counter1 = 0;
                    counter1 = 0;
                    PageLoadWait.WaitForFrameLoad(10);
                    if (viewer.AttachmentUploadIcon().Displayed)
                    {
                        while (viewer.AttachmentUploadIcon().Displayed && counter1++ < 10)
                        {
                            Thread.Sleep(60000);
                            PageLoadWait.WaitForFrameLoad(20);

                        }
                    }
                    ExecutedSteps++;
                    ExecutedSteps++;
                    viewer.CloseHistoryPanel();
                    viewer.CloseStudy();
                    login.Logout();
                }
                else
                {
                    //Step-29:Select browse button and validate the dialog window displayed(Chrome and Firefox)
                    //Click Choose file button
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("iframeAttachment");
                    BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.ChooseFileBtn()));
                    viewer.ChooseFileBtn().Click();

                    //Get the main window
                    Window mainWindow = null;
                    IList<Window> windows = TestStack.White.Desktop.Instance.Windows(); //Get all the windows on desktop
                    String browsername = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                    for (int i = 0; i < windows.Count; i++)
                    {
                        string str = windows[i].Title.ToLower();
                        if (str.Contains(browsername)) //compare which window title is matching to your string
                        {
                            mainWindow = windows[i];
                            Logger.Instance.InfoLog("Window with title " + str + " is set as the working window");
                            break;
                        }
                    }
                    mainWindow.WaitWhileBusy();

                    String uploadWindowName = "";
                    if (mainWindow.Enabled)
                    {
                        if (browsername.Equals("chrome"))
                        {
                            uploadWindowName = "Open";
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else if (browsername.Equals("firefox"))
                        {
                            uploadWindowName = "File Upload";
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }

                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }



                    //Step-30:Select any file to upload and validate the file name displayed
                    //Get Upload window
                    Window UploadWindow = mainWindow.ModalWindow(uploadWindowName);
                    UploadWindow.WaitWhileBusy();

                    var editBox = UploadWindow.Get<TestStack.White.UIItems.TextBox>(SearchCriteria.ByText("File name:"));
                    editBox.SetValue(Filepath[0]);
                    UploadWindow.WaitWhileBusy();
                    Thread.Sleep(20000);

                    //Click Open button
                    try
                    {
                        var openBtn = UploadWindow.Get<TestStack.White.UIItems.Button>(SearchCriteria.ByAutomationId("1"));
                        openBtn.Click();
                    }
                    catch (Exception)
                    {
                        var openBtn = UploadWindow.Get<TestStack.White.UIItems.Panel>(SearchCriteria.ByAutomationId("1"));
                        openBtn.Click();
                    }

                    //Sync-up
                    int counter = 0;
                    while (UploadWindow.Visible && counter++ < 10)
                    {
                        Thread.Sleep(1000);
                    }
                    mainWindow.WaitWhileBusy();
                    String filename = Filepath[0].Split('\\')[Filepath[0].Split('\\').Length - 1];
                    String Browsername = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
                    if (Browsername.Equals("chrome"))
                    {
                        String file = (String)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function Filename(){var name=document.querySelector('input#inputAttachment').value;return name;}return Filename()");
                        String[] FileName = file.Split('\\');

                        if (filename.Equals(FileName[2]))
                        {
                            Logger.Instance.InfoLog("In Chrome");
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            Logger.Instance.InfoLog("In Chrome");
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }

                    }
                    else if (Browsername.Equals("firefox"))
                    {
                        String file = (String)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("function Filename(){var name=document.querySelector('input#inputAttachment').value;return name;}return Filename()");
                        if (filename.Equals(file))
                        {
                            Logger.Instance.InfoLog("In Firefox");
                            result.steps[++ExecutedSteps].status = "Pass";
                            Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                        }
                        else
                        {
                            Logger.Instance.InfoLog("In Firefox");
                            result.steps[++ExecutedSteps].status = "Fail";
                            Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                            result.steps[ExecutedSteps].SetLogs();
                        }
                    }
                    else
                    {
                        Logger.Instance.InfoLog("In Firefox");
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }


                    //Step-31:Select Save button and validate the display
                    // viewer.AttachmentSaveBtn().Click();
                    ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"input#m_sendAttachmentButton\").click()");
                    BasePage.Driver.SwitchTo().DefaultContent();
                    BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                    WAIT.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("img#imgUpLoadProgress")));
                    Thread.Sleep(60000); 

                    Dictionary<string, string> Filerow = viewer.StudyViewerListMatchingRow("Name", filename, "patienthistory", "attachment");

                    if (Filerow != null)
                    {
                        result.steps[++ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[++ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                    }
                    viewer.CloseHistoryPanel();
                    viewer.CloseStudy();
                    login.Logout();

                }

                //Step-32:Logon to MergePACS Management page and navigate to Monitor---Importer tab and verify the status for the study.
                //Synch-up
                Thread.Sleep(300000);
                mpaclogin.DriverGoTo(login.GetMPacUrl(Config.SanityPACS));
                MPHomePage mpachome = mpaclogin.Loginpacs(Config.pacsadmin, Config.pacspassword);                
                Monitors monitors = (Monitors)mpachome.NavigateTopMenu("Monitors");
                monitors.NavigateToImporters();
                IList<String> msgs = monitors.GetStudyStatus();
                bool IsStatusExist = msgs.Any(msg=>msg.Contains("Status:OK"));
                bool IsNewStudymsgExist = msgs.Any(msg=>msg.Contains("New study arrived"));
                bool IsPatientIDExist = msgs.Any(msg=>msg.Contains(PatientIds[1]));
                bool IsAccessionExist = msgs.Any(msg=>msg.Contains(AccessionID));
                bool IsIPIDExist = msgs.Any(msg=>msg.Contains(Config.ipid1));
                bool IsModalityExist = msgs.Any(msg=>msg.Contains(Modality));
                if (IsStatusExist && IsPatientIDExist && IsAccessionExist && IsIPIDExist && IsNewStudymsgExist &&
                    IsModalityExist)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                mpaclogin.LogoutPacs();
                

                //Step-33 to 41:
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
               


                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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
        }

        /// <summary>
        /// Requisition Reports
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test1_27932(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Studies studies = null;
            StudyViewer viewer = null;
            DomainManagement domainmanagement;
            RoleManagement rolemanagement;
            UserManagement usermanagement;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String UserName = Config.adminUserName;
                String Password = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String StudyID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");
                String Title = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Title");
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String newrolename = "RegularRole" + new Random().Next(1, 100);
                String newusername = "U1" + new Random().Next(1, 100);
                String PACSA7 = login.GetHostName(Config.SanityPACS);

                /*Precondition--> In the service tool Enable Features tab click on the Enable Requisition Report flag,
                  Login as Administrator and in the Domain management click on the Enable Requisition Reports box*/

                /*Setting the tools in Requisition Toolbar*/
                //Login as Administrator
                login.DriverGoTo(login.url);
                login.LoginIConnect(UserName, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SetCheckBoxInEditDomain("requisitionreport", 0);
                domainmanagement.SetCheckBoxInEditDomain("attachment", 0);
                domainmanagement.SelectToolbarType("Requisition Toolbar",0);
                IList<String> ToolsAdded = domainmanagement.GetConnectedTools();
                //domainmanagement.RemoveAllToolsFromToolBar();               
                //domainmanagement.AddToolsToToolbarByName(new String[] { "All in One Tool", "Window Level", "Auto Window Level", "Zoom", "Pan", "Rotate Clockwise", "Flip Vertical", "Reset", "Toggle Text", "Print View" });
                //domainmanagement.AddAllToolsFromToolBar();
                domainmanagement.ClickSaveDomain();


                //Step-1: Load a study with multiple requisitions and attachments

                //Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Search,Select,Launch study
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID:StudyID,Datasource: PACSA7);
                Dictionary<string, string> study = studies.GetMatchingRow("Study ID", StudyID);
                studies.SelectStudy1("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();

                //Verify whether Requisition icon is displayed
                PageLoadWait.WaitForFrameLoad(30);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                if (viewer.RequisitionIcon().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-2: Click on the Requisition icon and verify the display
                viewer.RequisitionIcon().Click();
                PageLoadWait.WaitForFrameLoad(20);
                Logger.Instance.InfoLog("Requisition icon is clicked");
                Thread.Sleep(3000);
                PageLoadWait.WaitForFrameLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.By_ReqViewerinPanel()));
                if (viewer.SeriesViewer_1X1().Displayed && viewer.RequisitionViewerInStudyPanel().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                /**Test Data is required**/
                //Step-3:Select Requisition List Button and Double click on the first item listed and validate
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-4:Double click on the second item listed and validate
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-5:Repeat the previous step for all remaining items listed
                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step-6:Click Maximize icon and verify the display
                String PatientInfo = BasePage.Driver.FindElement(By.CssSelector("span#m_studyPanels_m_studyPanel_1_patientInfoDiv"))
                   .GetAttribute("title").Replace(" ", "").Replace("(", ",").Replace(")", "");

                viewer.RequisitionMaxIcon().Click();
                PageLoadWait.WaitForFrameLoad(20);
                Logger.Instance.InfoLog("Requisition Max icon is clicked");
                Thread.Sleep(3000);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.By_ReqViewerinPanel()));
                PageLoadWait.WaitForFrameLoad(20);
                if (viewer.SeriesViewer_1X1().Displayed == false && viewer.RequisitionViewerInStudyPanel().Displayed == true && viewer.RequisitionIcon().Displayed && study["Patient ID"].Equals(PatientInfo.Split(',')[2]) && study["Patient Name"].Contains(PatientInfo.Split(',')[0]) && study["Patient Name"].Contains(PatientInfo.Split(',')[1]))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-7:Click Maximize icon again and verify the display is restored
                viewer.RequisitionMaxIcon().Click();
                Thread.Sleep(3000);
                PageLoadWait.WaitForFrameLoad(20);
                Logger.Instance.InfoLog("Requisition Max icon is clicked again");
                Thread.Sleep(3000);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.By_ReqViewerinPanel()));
                PageLoadWait.WaitForFrameLoad(20);
                if (viewer.SeriesViewer_1X1().Displayed && viewer.RequisitionViewerInStudyPanel().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-8: Click Requisition close button and verify the display of viewport
                viewer.RequisitionCloseIcon().Click();
                PageLoadWait.WaitForFrameLoad(20);
                if (viewer.SeriesViewer_1X1().Displayed && viewer.RequisitionViewerInStudyPanel().Displayed == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-9:Open Requisition in History Panel Drawer and validate the tools displayed
                viewer.NavigateToHistoryPanel();
                viewer.NavigateTabInHistoryPanel("Requisition");
                viewer.SelectItemInStudyViewerList("Title", Title, "PatientHistory", "Requisition");
                PageLoadWait.WaitForFrameLoad(20);
                IList<String> ToolTitles = viewer.GetToolsFromViewer("requisition");
                IList<String> DefaultTitles = new List<String> { "All in One Tool", "Window Level", "Auto Window Level", "Zoom", "Pan", "Rotate Clockwise", "Flip Vertical", "Reset", "Toggle Text", "Print View" };
                Boolean compare = studies.CompareList(ToolTitles, DefaultTitles);
                if (viewer.RequisitionToolBar().Displayed && compare == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }



                //Step-10:Configure Requisition Toolbar in Domain Level and Role level
                studies.CloseHistoryPanel();
                studies.CloseStudy();

                //Navigate to DomainManagement Tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");

                //Enable Requisition Reports box in SuperAdminGroup
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SelectToolbarType("Requisition Toolbar",0);
                domainmanagement.RemoveAllToolsFromToolBar();
                domainmanagement.AddToolsToToolbarByName(new String[] { "All in One Tool", "Invert", "Line Measurement" });
                domainmanagement.ClickSaveDomain();
                //Navigate to Inbounds
                studies = (Studies)login.Navigate("Studies");

                //Search,Select,Launch study
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID: StudyID, Datasource: PACSA7);
                Dictionary<string, string> study1 = studies.GetMatchingRow("Study ID", StudyID);
                studies.SelectStudy1("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();

                viewer.NavigateToHistoryPanel();
                viewer.NavigateTabInHistoryPanel("Requisition");
                viewer.SelectItemInStudyViewerList("Title", Title, "PatientHistory", "Requisition");
                PageLoadWait.WaitForFrameLoad(20);
                IList<String> ToolTitles1 = viewer.GetToolsFromViewer("requisition");
                IList<String> DefaultTitles1 = new List<String> { "All in One Tool", "Invert", "Line Measurement" };
                Boolean compare1 = studies.CompareList(ToolTitles1, DefaultTitles1);
                if (viewer.RequisitionToolBar().Displayed && compare1 == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-11:Create new Role and configure Requisition toolbar
                studies.CloseHistoryPanel();
                studies.CloseStudy();
                //Navigate to RoleManagement tab
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.ClickNewRoleBtn();
                rolemanagement.CreateRole(DefaultDomain, newrolename);
                rolemanagement.SearchRole(newrolename,DefaultDomain);
                rolemanagement.SelectRole(newrolename);
                rolemanagement.ClickEditRole();                
                rolemanagement.SelectToolbarType("Requisition Toolbar",0);
                rolemanagement.SetCheckboxInEditRole("toolbar", 1);
                rolemanagement.RemoveAllToolsFromToolBar();
                rolemanagement.AddToolsToToolbarByName(new String[] { "Pan", "Window Level" });
                rolemanagement.ClickSaveEditRole();
                PageLoadWait.WaitForFrameLoad(30);
                rolemanagement.DomainSelector().SelectByText(DefaultDomain);
                bool role = rolemanagement.RoleExists(newrolename);
                //Navigate to UserManagement tab
                usermanagement = (UserManagement)login.Navigate("UserManagement");
                usermanagement.CreateUser(newusername, DefaultDomain, newrolename);
                bool user = usermanagement.SearchUser(newusername, DefaultDomain);

                if (role == true && user == true)
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

                //Step-12:Login as New user
                login.Logout();
                login.LoginIConnect(newusername, newusername);
                //Navigate to Inbounds
                studies = (Studies)login.Navigate("Studies");

                //Search,Select,Launch study
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID: StudyID, Datasource: PACSA7);
                studies.SelectStudy1("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();

                studies.NavigateToHistoryPanel();
                viewer.NavigateTabInHistoryPanel("Requisition");
                viewer.SelectItemInStudyViewerList("Title", Title, "PatientHistory", "Requisition");
                PageLoadWait.WaitForFrameLoad(20);
                IList<String> ToolTitles2 = viewer.GetToolsFromViewer("requisition");
                IList<String> DefaultTitles2 = new List<String> { "Pan", "Window Level" };
                Boolean compare2 = studies.CompareList(ToolTitles2, DefaultTitles2);
                if (viewer.RequisitionToolBar().Displayed && compare2 == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                studies.CloseHistoryPanel();
                studies.CloseStudy();
                login.Logout();

                //Step-13:Login as Administrator
                login.LoginIConnect(UserName, Password);
                //Navigate to Inbounds
                studies = (Studies)login.Navigate("Studies");

                //Search,Select,Launch study
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID: StudyID, Datasource: PACSA7);
                studies.SelectStudy1("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();

                viewer.NavigateToHistoryPanel();
                viewer.NavigateTabInHistoryPanel("Requisition");
                viewer.SelectItemInStudyViewerList("Title", Title, "PatientHistory", "Requisition");
                PageLoadWait.WaitForFrameLoad(20);
                IList<String> ToolTitles3 = viewer.GetToolsFromViewer("requisition");
                IList<String> DefaultTitles3 = new List<String> { "All in One Tool", "Invert", "Line Measurement" };
                Boolean compare3 = studies.CompareList(ToolTitles3, DefaultTitles3);
                if (viewer.RequisitionToolBar().Displayed && compare3 == true)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-14: Click Attachment Tab in History Panel and validate multiple attachments are listed
                viewer.NavigateTabInHistoryPanel("Attachment");
                IList<IWebElement> rows = BasePage.Driver.FindElements(By.CssSelector("[id$='_attachmentList']>tbody>tr"));

                if (rows.Count > 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-15:Select different attachments and validate the display                
                result.steps[++ExecutedSteps].status = "Not Automated";

                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                login.Logout();


                /*Resetting the tools in Requisition Toolbar*/
                login.LoginIConnect(UserName, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SelectToolbarType("Requisition Toolbar",0);
                domainmanagement.RemoveAllToolsFromToolBar();
                domainmanagement.AddToolsToToolbarByName(new String[] { "All in One Tool", "Window Level", "Auto Window Level", "Zoom", "Pan", "Rotate Clockwise", "Flip Vertical", "Reset", "Toggle Text", "Print View" });
                domainmanagement.ClickSaveDomain();
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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
            finally
            {
                /*Resetting the tools in Requisition Toolbar*/
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SelectToolbarType("Requisition Toolbar",0);
                domainmanagement.RemoveAllToolsFromToolBar();
                domainmanagement.AddToolsToToolbarByName(new String[] { "All in One Tool", "Window Level", "Auto Window Level", "Zoom", "Pan", "Rotate Clockwise", "Flip Vertical", "Reset", "Toggle Text", "Print View" });
                domainmanagement.ClickSaveDomain();
                login.Logout();
            }
        }

        /// <summary>
        /// Validating Tools functionality in Requisition Toolbar
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test2_27932(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Studies studies = null;
            StudyViewer viewer = null;
            DomainManagement domainmanagement;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String UserName = Config.adminUserName;
                String Password = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String StudyID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");
                String Title = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Title");
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String PACSA7 = login.GetHostName(Config.SanityPACS);

                //Setting the tools
                login.DriverGoTo(login.url);
                login.LoginIConnect(UserName, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SelectToolbarType("Requisition Toolbar",0);
                domainmanagement.RemoveAllToolsFromToolBar();
                domainmanagement.AddToolsToToolbarByName(new String[] { "All in One Tool", "Window Level", "Auto Window Level", "Zoom", "Pan", "Rotate Clockwise", "Flip Vertical", "Reset", "Toggle Text", "Invert", "Line Measurement" });
                domainmanagement.ClickSaveDomain();
                login.Logout();

                //Login as Administrator
                login.DriverGoTo(login.url);
                login.LoginIConnect(UserName, Password);

                //Navigate to Inbounds
                studies = (Studies)login.Navigate("Studies");

                //Search,Select,Launch study
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID:StudyID,Datasource: PACSA7);
                Dictionary<string, string> study = studies.GetMatchingRow("Study ID", StudyID);
                studies.SelectStudy1("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();

                //OpenHistoryPanel and Open Requisition Viewer
                viewer.NavigateToHistoryPanel();
                viewer.NavigateTabInHistoryPanel("Requisition");
                viewer.SelectItemInStudyViewerList("Title", Title, "PatientHistory", "Requisition");

                //Perform on each tool with the image(AllInOne,Pan,Zoom,WL,Auto WL,Flip Vertical,Rotate Clockwise,ToggleText,Print,Reset)
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IWebElement Img = BasePage.Driver.FindElement(By.CssSelector("#m_patientHistory_m_requisitionViewer_m_compositeViewer_SeriesViewer_1_1_viewerImg"));

                //Step-1:AllinOne and Reset
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AllinOneTool, "requisition");
                viewer.DragMovement(Img);
                //Validate Images 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                var viewport = BasePage.Driver.FindElement(By.CssSelector("#m_patientHistory_m_requisitionViewer_m_compositeViewer_SeriesViewer_1_1_viewerImg"));
                bool status = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (status)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset, "requisition");



                //Step-2:Pan 
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan, "requisition");
                viewer.DragMovement(Img);
                //Validate Images 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                var viewport0 = BasePage.Driver.FindElement(By.CssSelector("#m_patientHistory_m_requisitionViewer_m_compositeViewer_SeriesViewer_1_1_viewerImg"));
                bool status0 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (status0)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset, "requisition");


                //Step-3:Zoom
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Zoom, "requisition");
                viewer.DragMovement(Img);
                //Validate Images 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                var viewport1 = BasePage.Driver.FindElement(By.CssSelector("#m_patientHistory_m_requisitionViewer_m_compositeViewer_SeriesViewer_1_1_viewerImg"));
                bool status1 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (status1)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset, "requisition");


                //Step-4:WindowLevel
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel, "requisition");
                viewer.DragMovement(Img);
                //Validate Images 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                var viewport2 = BasePage.Driver.FindElement(By.CssSelector("#m_patientHistory_m_requisitionViewer_m_compositeViewer_SeriesViewer_1_1_viewerImg"));
                bool status2 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (status2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset, "requisition");

                //Step-5:FlipVertical
                viewer.SelectToolInToolBar(IEnum.ViewerTools.FlipVertical, "requisition");
                //Validate Images 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                var viewport3 = BasePage.Driver.FindElement(By.CssSelector("#m_patientHistory_m_requisitionViewer_m_compositeViewer_SeriesViewer_1_1_viewerImg"));
                bool status3 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (status3)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset, "requisition");

                //Step-6:RotateClockwise
                viewer.SelectToolInToolBar(IEnum.ViewerTools.RotateClockwise, "requisition");
                //Validate Images 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                var viewport4 = BasePage.Driver.FindElement(By.CssSelector("#m_patientHistory_m_requisitionViewer_m_compositeViewer_SeriesViewer_1_1_viewerImg"));
                bool status4 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (status4)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset, "requisition");

                //Step-7:AutoWindow Level
                viewer.SelectToolInToolBar(IEnum.ViewerTools.AutoWindowLevel, "requisition");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IWebElement image1 = BasePage.Driver.FindElement(By.CssSelector("img[src*='autoWlImage']"));
                //Validate Image 
                if (image1.Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset, "requisition");



                //Step-8:ToggleText
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ToggleText, "requisition");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IWebElement image4 = BasePage.Driver.FindElement(By.CssSelector("img[src*='toggleImageText']"));
                //Validate Image 
                if (image4.Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset, "requisition");
                viewer.SelectItemInStudyViewerList("Title", Title, "PatientHistory", "Requisition");

                //Step-9:Invert
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Invert, "requisition");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IWebElement image0 = BasePage.Driver.FindElement(By.CssSelector("img[src*='invertImages']"));
                //Validate Image 
                if (image0.Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset, "requisition");

                //Step-10:Line Measurement                
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LineMeasurement, "requisition");
                viewer.JoinCoordinatesInStudyViewer(Img);
                //Validate Images 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                PageLoadWait.WaitForFrameLoad(10);
                var viewport8 = BasePage.Driver.FindElement(By.CssSelector("#m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg"));
                bool status8 = studies.CompareImage(result.steps[ExecutedSteps], viewport);
                if (status8)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset, "requisition");
                viewer.SelectItemInStudyViewerList("Title", Title, "PatientHistory", "Requisition");

                //Step-11:Reset
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset, "requisition");
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                IWebElement image5 = BasePage.Driver.FindElement(By.CssSelector("img[src*='resetImage']"));
                //Validate Image 
                if (image5.Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset, "requisition");

                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                login.Logout();

                //Resetting
                login.LoginIConnect(UserName, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                domainmanagement.SelectToolbarType("Requisition Toolbar",0);
                domainmanagement.RemoveAllToolsFromToolBar();
                domainmanagement.AddToolsToToolbarByName(new String[] { "All in One Tool", "Window Level", "Auto Window Level", "Zoom", "Pan", "Rotate Clockwise", "Flip Vertical", "Reset", "Toggle Text", "Print View" });
                domainmanagement.ClickSaveDomain();
                login.Logout();



                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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
                //Resetting
            String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
            login.LoginIConnect(Config.adminUserName, Config.adminPassword);
            domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
            domainmanagement.SearchDomain(DefaultDomain);
            domainmanagement.SelectDomain(DefaultDomain);
            domainmanagement.ClickEditDomain();
            domainmanagement.SelectToolbarType("Requisition Toolbar",0);
            domainmanagement.RemoveAllToolsFromToolBar();
            domainmanagement.AddToolsToToolbarByName(new String[] { "All in One Tool", "Window Level", "Auto Window Level", "Zoom", "Pan", "Rotate Clockwise", "Flip Vertical", "Reset", "Toggle Text", "Print View" });
            domainmanagement.ClickSaveDomain();
            login.Logout();

            }
        }

        /// <summary>
        /// Study Attachments
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27933(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Studies studies = null;
            StudyViewer viewer;
            DomainManagement domainmanagement;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String UserName = Config.adminUserName;
                String Password = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String[] PatientIds = PatientID.Split(':');
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
                String[] Accessions = AccessionID.Split(':');
                String StudyID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "StudyID");
                String Title = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Title");
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String FilePath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AttachmentPath");
                String PACSA7 = login.GetHostName(Config.SanityPACS);

                /*Precondition--> Enable study attachment in ICA Service Tool -*^>^* Enable Features -*^>^* Study Attachment tab. Select to 'Store attachments with original study'*/
                //Setting required features
                login.LoginIConnect(UserName, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForFrameLoad(40);
                domainmanagement.SetCheckBoxInEditDomain("reportview", 0);
                domainmanagement.SetCheckBoxInEditDomain("requisitionreport", 0);
                domainmanagement.ClickSaveDomain();
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(AccessionNo: Accessions[1],Datasource:PACSA7);
                studies.SelectStudy1("Accession", Accessions[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(30);
                int viewports = viewer.SeriesViewPorts(1).Count;
                viewer.CloseStudy();
                login.Logout();

                //Login as Administrator
                login.DriverGoTo(login.url);
                login.LoginIConnect(UserName, Password);

                //Navigate to DomainManagement Tab
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");


                //Step-1:Enable Attachment at DomainLevel(SuperAdminGroup)
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForFrameLoad(40);
                domainmanagement.SetCheckBoxInEditDomain("attachment", 0);
                domainmanagement.SetCheckBoxInEditDomain("attachmentupload", 0);
                domainmanagement.ClickSaveDomain();
                ExecutedSteps++;

                //Step-2:Load a study with multiple studies,reports,requisitions,attachments
                //Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Search,Select,Launch study
                studies.SearchStudy(AccessionNo: Accessions[1], Datasource: PACSA7);
                studies.SelectStudy1("Accession", Accessions[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(10);
                int viewports1 = viewer.SeriesViewPorts(1).Count;
                bool ReportIcon = viewer.TitlebarReportIcon().Displayed;
                viewer.NavigateToHistoryPanel();
                String reportMRN = viewer.ReportDetails("patienthistory")["MRN"];
                PageLoadWait.WaitForFrameLoad(10);
                bool Report = viewer.ReportInHistoryPanel().Displayed;
                bool ReportMrn = reportMRN.Equals(PatientIds[1]);
                viewer.CloseHistoryPanel();
                viewer.CloseStudy();
                studies.ClearButton().Click();
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID:StudyID,Datasource:PACSA7);
                Dictionary<string, string> study = studies.GetMatchingRow("Study ID", StudyID);
                studies.SelectStudy1("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();

                //Verify whether Requisition icon is displayed
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                if (viewports1 == viewports && viewer.RequisitionIcon().Displayed && ReportIcon && Report && ReportMrn)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-3:Open History Panel and validate the display
                viewer.NavigateToHistoryPanel();

                if (viewer.RequisitionIcon().Displayed && viewer.AttachmentTab().Displayed && viewer.ReportTab().Displayed &&
                    viewer.StudylistInHistoryPanel().Count > 0 && viewer.PatientStudyInfo("Name").Displayed && viewer.PatientStudyInfo("ID").Displayed && viewer.PatientStudyInfo("DOB").Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-4:Select another study and validate whether it is highlighted                
                result.steps[++ExecutedSteps].status = "Not Applicable";

             /*   viewer.Study().Click();
                if (viewer.Study().GetAttribute("aria-selected").Contains("true") )
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                } */

                //Step-5:Click Attachment tab and attachment should be listed
                viewer.NavigateTabInHistoryPanel("Attachment");
                if (viewer.ContentList("attachment").Displayed && viewer.ContentHeader("attachment", "date").Displayed && viewer.ContentHeader("attachment", "name").Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-6:Click attachment file
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-7:Upload a attachment
                PageLoadWait.WaitForFrameLoad(20);
                bool attachment = viewer.UploadAttachment(FilePath, 20);
                if (attachment)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-8:View previously saved attachment
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-9:Click Requisition tab
                viewer.NavigateTabInHistoryPanel("Requisition");
                if (viewer.ContentList("requisition").Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-10:Requisition is clicked and validate its display
                viewer.SelectItemInStudyViewerList("Title", Title, "PatientHistory", "Requisition");
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(50);
                PageLoadWait.WaitForFrameLoad(50);
                BasePage.wait.Until<Boolean>((d) =>
                {
                    if (new StudyViewer().GetInnerAttribute(viewer.RequisitionViewerInHistoryPanel(), "style", ';', "cursor", ":").Equals("move"))
                    {
                        Logger.Instance.InfoLog("Requisition viewer loading is completed");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Waiting for Requisition viewer to load");
                        return false;
                    }
                });
                if (viewer.RequisitionViewerInHistoryPanel().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-11:Close HistoryPanel and validate the 'History' Button
                viewer.CloseHistoryPanel();
                Thread.Sleep(10000);
                PageLoadWait.WaitForThumbnailsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.By_ReviewtoolBar()));
                if (viewer.RequisitionViewerInHistoryPanel().Displayed == false && viewer.PatientHistoryDrawer().Displayed && viewer.SeriesViewer_1X1().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                studies.CloseStudy();

                //Step-12 to 20 : Applicable to only Tablet Testing
                result.steps[++ExecutedSteps].status = "Not Applicable";
                result.steps[++ExecutedSteps].status = "Not Applicable";
                result.steps[++ExecutedSteps].status = "Not Applicable";
                result.steps[++ExecutedSteps].status = "Not Applicable";
                result.steps[++ExecutedSteps].status = "Not Applicable";
                result.steps[++ExecutedSteps].status = "Not Applicable";
                result.steps[++ExecutedSteps].status = "Not Applicable";
                result.steps[++ExecutedSteps].status = "Not Applicable";
                result.steps[++ExecutedSteps].status = "Not Applicable";

                //Step-21:Load a study and validate the buttons in viewer
                studies.ClearButton().Click();
                studies.SearchStudy(AccessionNo: Accessions[1], Datasource: PACSA7);
                studies.SelectStudy1("Accession", Accessions[1]);
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(10);
                bool ReportIcon21 = viewer.TitlebarReportIcon().Displayed;
                viewer.CloseStudy();
                studies.ClearButton().Click();
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID:StudyID,Datasource:PACSA7);
                studies.SelectStudy1("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();

                if (viewer.SeriesViewer_1X1().Displayed && viewer.RequisitionIcon().Displayed && ReportIcon21)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-22:Open HistoryPanel and validate
                viewer.NavigateToHistoryPanel();
                if (viewer.ReportTab().Displayed && viewer.AttachmentTab().Displayed && viewer.RequisitionTab().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                viewer.CloseHistoryPanel();
                viewer.CloseStudy();

                //Step-23:Disable  Report Viewing checkbox in Edit Domain Page
                //Navigate to DomainManagement Tab
                PageLoadWait.WaitForFrameLoad(30);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");

                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForFrameLoad(40);
                domainmanagement.SetCheckBoxInEditDomain("reportview", 1);
                domainmanagement.ClickSaveDomain();
                ExecutedSteps++;

                //Step-24:Load a study and validate the buttons in viewer
                studies = (Studies)login.Navigate("Studies");
                studies.ClearButton().Click();
                studies.SearchStudy(AccessionNo: Accessions[1], Datasource: PACSA7);
                studies.SelectStudy1("Accession", Accessions[1]);
                viewer = StudyViewer.LaunchStudy();
                bool ReportIcon24;
                try
                {

                    ReportIcon24 = viewer.TitlebarReportIcon().Displayed;

                }
                catch (NoSuchElementException e)
                {
                    ReportIcon24 = true;
                }
                if (viewer.SeriesViewer_1X1().Displayed && ReportIcon24)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }


                //Step-25:Open HistoryPanel and validate
                viewer.CloseStudy();
                studies.ClearButton().Click();
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID: StudyID,Datasource:PACSA7);
                studies.SelectStudy1("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();
                viewer.NavigateToHistoryPanel();
                bool Reporttab25;
                try
                {
                    Reporttab25 = viewer.ReportTab().Displayed;

                }
                catch (NoSuchElementException e)
                {

                    Reporttab25 = true;
                }
                if (viewer.AttachmentTab().Displayed && viewer.RequisitionTab().Displayed && Reporttab25)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                viewer.CloseHistoryPanel();
                studies.CloseStudy();

                //Step-26:Disable Enable Requisition report checkbox in Edit Domain Page
                //Navigate to DomainManagement Tab
                PageLoadWait.WaitForFrameLoad(30);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");

                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForFrameLoad(40);
                domainmanagement.SetCheckBoxInEditDomain("requisitionreport", 1);
                domainmanagement.ClickSaveDomain();
                ExecutedSteps++;

                //Step-27:Load a study and validate the buttons in viewer
                studies = (Studies)login.Navigate("Studies");
                studies.ClearButton().Click();
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID: StudyID,Datasource:PACSA7);
                studies.SelectStudy1("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();
                bool ReqIcon27;
                try
                {
                    ReqIcon27 = viewer.RequisitionIcon().Displayed;

                }
                catch (NoSuchElementException e)
                {
                    ReqIcon27 = true;
                }
                if (viewer.SeriesViewer_1X1().Displayed && ReqIcon27)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-28:Open HistoryPanel and validate
                viewer.NavigateToHistoryPanel();
                bool Reqtab28;
                try
                {
                    Reqtab28 = viewer.RequisitionTab().Displayed;

                }
                catch (NoSuchElementException e)
                {
                    Reqtab28 = true;
                }
                if (viewer.AttachmentTab().Displayed && Reqtab28)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                viewer.CloseHistoryPanel();
                studies.CloseStudy();

                //Step-29:Disable Enable Attach upload,Enable Attachment checkbox in Edit Domain Page
                //Navigate to DomainManagement Tab
                PageLoadWait.WaitForFrameLoad(30);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");

                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForFrameLoad(40);
                domainmanagement.SetCheckBoxInEditDomain("attachmentupload", 1);
                domainmanagement.SetCheckBoxInEditDomain("attachment", 1);
                domainmanagement.ClickSaveDomain();
                ExecutedSteps++;

                //Step-30:Load a study and validate the buttons in viewer
                studies = (Studies)login.Navigate("Studies");
                studies.ClearButton().Click();
                studies.ChooseColumns(new string[] { "Study ID" });
                studies.SearchStudy(studyID:StudyID,Datasource:PACSA7);
                studies.SelectStudy1("Study ID", StudyID);
                viewer = StudyViewer.LaunchStudy();

                if (viewer.SeriesViewer_1X1().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step-31:Open HistoryPanel and validate
                viewer.NavigateToHistoryPanel();
                if (viewer.StudylistInHistoryPanel().Count > 0 && BasePage.Driver.FindElement(By.CssSelector("div#m_patientHistory_documentViewerContainer")).Displayed == false)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                viewer.CloseHistoryPanel();
                studies.CloseStudy();

                //Setting required features
                login.LoginIConnect(UserName, Password);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForFrameLoad(40);
                domainmanagement.SetCheckBoxInEditDomain("reportview", 0);
                domainmanagement.SetCheckBoxInEditDomain("requisitionreport", 0);
                domainmanagement.SetCheckBoxInEditDomain("attachment", 0);
                domainmanagement.SetCheckBoxInEditDomain("attachmentupload", 0);
                domainmanagement.ClickSaveDomain();
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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
                //Setting required features
                String DefaultDomain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domainmanagement = (DomainManagement)login.Navigate("DomainManagement");
                domainmanagement.SearchDomain(DefaultDomain);
                domainmanagement.SelectDomain(DefaultDomain);
                domainmanagement.ClickEditDomain();
                PageLoadWait.WaitForFrameLoad(40);
                domainmanagement.SetCheckBoxInEditDomain("reportview", 0);
                domainmanagement.SetCheckBoxInEditDomain("requisitionreport", 0);
                domainmanagement.SetCheckBoxInEditDomain("attachment", 0);
                domainmanagement.SetCheckBoxInEditDomain("attachmentupload", 0);
                domainmanagement.ClickSaveDomain();
                login.Logout();
            }

        }
                
        /// <summary>
        /// Study Selection
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_28007(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Studies studies = null;
            StudyViewer viewer = null;
            UserManagement usermanagement = null;
            RoleManagement rolemanagement = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String UserName = Config.adminUserName;
                String Password = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String[] AccessionID = AccessionIDList.Split(':');
                String Description = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
                String AttachmentPath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AttachmentPath");
                String[] AttachmentFilePath = AttachmentPath.Split('=');
                String ReportDate = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ReportDate");
                String EmailID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EmailID");
                String EmailPassword = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "EmailPassword");
                String studypath = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "UploadFilePath");
                String Reports = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ReportFilePath");
                String DomainName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String[] ReportPath = Reports.Split('=');
                String Reason = "Testing_" +  new Random().Next(1, 1000);
                String NewUserName = "TestUser" + new Random().Next(1, 1000);
                //String DestPACS = login.GetHostName(Config.DestinationPACS);
                String DestPACS = "PA-TST5-WS8";

                //Initail Setup - Send report to study with multiple studies
                BasePage.RunBatchFile(Config.batchfilepath, studypath + " " + Config.dicomsendpath + " " + Config.DestinationPACS);
                Boolean Report_1 = login.SendHL7Order(Config.DestinationPACS, int.Parse(Config.mpacport), ReportPath[0]);
                Boolean Report_2 = login.SendHL7Order(Config.DestinationPACS, int.Parse(Config.mpacport), ReportPath[1]);

                //Login as Administrator
                login.LoginIConnect(UserName, Password);

                //PreCondition :- Enable Email Study for SuperRole User
                rolemanagement = (RoleManagement)login.Navigate("RoleManagement");
                rolemanagement.SelectDomainfromDropDown("SuperAdminGroup");
                rolemanagement.SearchRole("SuperRole");
                rolemanagement.SelectRole("SuperRole");
                rolemanagement.ClickEditRole();
                rolemanagement.SetCheckboxInEditRole("email", 0);
                rolemanagement.ClickSaveEditRole();

                //Navigate to Studies tab
                studies = (Studies)login.Navigate("Studies");

                //Step 1:- Validate studies with report are either uploaded or not
                ExecutedSteps++;
                studies.SearchStudy(patientID:PatientID,Datasource: DestPACS);
                foreach (string Accession in AccessionID)
                {
                    String ImageCount = "2";
                    if (Accession.Equals(AccessionID[2])) { ImageCount = "1"; }
                    Dictionary<string, string> studydetails = studies.GetMatchingRow(new string[] { "Accession", "# Images" }, new string[] { Accession, ImageCount });

                    if (studydetails != null)
                    {
                        result.steps[ExecutedSteps].status = "Pass";
                        Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                    }
                    else
                    {
                        result.steps[ExecutedSteps].status = "Fail";
                        Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                        result.steps[ExecutedSteps].SetLogs();
                        throw new Exception("One of the Study/Report is not Uploaded");
                    }
                }

                //Navigate to User Management tab
                usermanagement = (UserManagement)login.Navigate("UserManagement");

                //Create new user for the SuperAdmin domain                
                usermanagement.CreateUser(NewUserName, DomainName, "SuperRole", 1, EmailID, 1, NewUserName);
                Boolean UserStatus = usermanagement.SearchUser(NewUserName, DomainName);

                //Check user is created or not
                if (!UserStatus) { throw new Exception("User not created"); }

                //Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Search, Select study
                studies.SearchStudy(AccessionNo:AccessionID[0],Datasource: DestPACS);
                studies.SelectStudy("Accession", AccessionID[0]);

                //Launch study in viewer
                viewer = StudyViewer.LaunchStudy();

                //Take Screenshot
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                Boolean ViewerStatus = studies.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                //Step 2 :- Launch study and verify it's correctness
                if (ViewerStatus && viewer.ThumbnailContainer().Displayed)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Retreiving viewport details in default layout
                String[] SeriesUID_1 = viewer.GetSeriesUID(viewer.SeriesViewPorts());

                //Navigate to HistoryPanel
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });

                //Get Study details
                Dictionary<String, String> StudyDetails = viewer.GetMatchingRow("Accession", AccessionID[0]);
                String[] ColumnValues = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());
                DateTime Format;

                //Step 3 :- Validate details, format and position of opened study
                if (StudyDetails["Patient ID"].Equals(PatientID) && viewer.PatientHistoryRows()[0].GetAttribute("class").Contains("state-hover") &&
                    StudyDetails["Study Description"].Equals(Description) &&
                    DateTime.TryParseExact(StudyDetails["Study Date"], "dd-MMM-yyyy hh:mm:ss tt", null, DateTimeStyles.None, out Format))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Navigate to Attachment tab
                viewer.NavigateTabInHistoryPanel("Attachment");

                //Get Attachments 
                Dictionary<int, string[]> attachmentsList_1 = viewer.StudyViewerListResults("patienthistory", "attachment", 0);

                //Upload attachment file
                Boolean UploadStatus = viewer.UploadAttachment(AttachmentFilePath[0]);

                //Get Attachments 
                Dictionary<int, string[]> attachmentsList_2 = viewer.StudyViewerListResults("patienthistory", "attachment", 0);
                String[] AttachColumnNames = viewer.StudyViewerListColumnNames("patienthistory", "attachment", 0);
                String[] AttachColumnValues_2 = BasePage.GetColumnValues(attachmentsList_2, "Name", AttachColumnNames);

                //Step 4 :- Upload an attachment file and verify it's correctness
                if (attachmentsList_2.Count == (attachmentsList_1.Count + 1) && UploadStatus)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Navigate to Report tab
                viewer.NavigateTabInHistoryPanel("Report");

                //Get report details
                Dictionary<string, string> Reportdetails_1 = viewer.ReportDetails("patienthistory");

                //Step 5 :- Open sent report in report tab under patient history panel and check opened report is correctly displayed
                if (Reportdetails_1["Report Date"].Contains(ReportDate) && Reportdetails_1["MRN"].Equals(PatientID))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Click report icon in study panel
                PageLoadWait.WaitForFrameLoad(10);
                viewer.TitlebarReportIcon().Click();

                //Get report details
                Dictionary<string, string> Reportdetails_2 = viewer.ReportDetails("studypanel");
                PageLoadWait.WaitForFrameLoad(10);

                //Step 6 :- Open sent report in study panel and validate report viewer is displayed with study viewer
                if (viewer.ViewerContainer().Displayed && viewer.ReportContainer().Displayed &&
                    Reportdetails_2["Report Date"].Contains(ReportDate) && Reportdetails_2["MRN"].Equals(PatientID))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Navigate to HistoryPanel
                PageLoadWait.WaitForFrameLoad(10);
                viewer.NavigateToHistoryPanel();

                //Choose columns
                viewer.ChooseColumns(new String[] { "Accession" });

                //Select second study to open in another study panel
                viewer.OpenPriors(new String[] { "Accession" }, new String[] { ColumnValues[1] });

                //Get report details
                Dictionary<string, string> Reportdetails7_1 = viewer.ReportDetails("historypanel");
                PageLoadWait.WaitForFrameLoad(10);

                //Navigate to StudyPanel
                viewer.CloseHistoryPanel();

                //Get report details
                Dictionary<string, string> Reportdetails7_2 = viewer.ReportDetails("studypanel");
                PageLoadWait.WaitForFrameLoad(10);

                //Step 7 :- Validate report section is updated with reports for the Selected study
                if (Reportdetails7_1["MRN"].Equals(PatientID) && Reportdetails7_2["MRN"].Equals(PatientID)
                    || !viewer.ReportTab().Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Navigate to Attachment tab in left panel
                try { viewer.NavigateToHistoryPanel(); }
                catch (WebDriverTimeoutException) { }
                if (!BasePage.Driver.FindElement(By.CssSelector("#patientHistoryDemographics")).Displayed) { viewer.NavigateToHistoryPanel(); }
                viewer.NavigateTabInHistoryPanel("Attachment");

                //Get Attachments 
                Dictionary<int, string[]> attachmentsList_3 = viewer.StudyViewerListResults("patienthistory", "attachment", 0);
                String[] AttachColumnNames_3 = viewer.StudyViewerListColumnNames("patienthistory", "attachment", 0);
                String[] AttachColumnValues_3 = BasePage.GetColumnValues(attachmentsList_3, "Name", AttachColumnNames_3);

                int CountIndex = 0;
                Boolean AttachmentsMatch = Array.Exists(AttachColumnValues_3, attachment => attachment.Equals(AttachColumnValues_2[CountIndex++]));

                //Step 8 :- Validate attachments of currently opened study is opened
                if (!AttachmentsMatch)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 9 :- Validate thumbnails of newly Selected study
                result.steps[++ExecutedSteps].status = "Not Applicable";

                //Close HistoryPanel
                PageLoadWait.WaitForFrameLoad(10);
                viewer.CloseHistoryPanel();

                //Click report icon
                PageLoadWait.WaitForAllViewportsToLoad(30, 2);
                viewer.TitlebarReportIcon().Click();
                PageLoadWait.WaitForThumbnailsToLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Retreiving viewport details in first panel
                String[] SeriesUID_2 = viewer.GetSeriesUID(viewer.SeriesViewPorts());

                CountIndex = 0;
                Boolean SeriesUIDMatch = Array.Exists(SeriesUID_2, UID => UID.Equals(SeriesUID_1[CountIndex++]));

                //Step 10 :- Open image view and verify the image/series in viewport & disappearance of report viewer
                if (viewer.ViewerContainer().Displayed && !viewer.ReportContainer().Displayed && SeriesUIDMatch)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Keep newly opened study to be active
                viewer.Thumbnails(2)[0].Click();
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Email study              
                studies.EmailStudy(EmailID, EmailID.Split('@')[0], Reason, 1);

                //Step 11 :- Fetch Pin and validate emailed study
                string pinnumber = studies.FetchPin();
                if (!(pinnumber == ""))
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close Study and logout iCA
                studies.CloseStudy();
                login.Logout();

                //Step 12 - 21 :- Not Automated (have to use link from Email sent to the unregistered email ID)
                //totally 10 steps
                for (int i = 0; i < 10 ;i++)
                {
                    result.steps[++ExecutedSteps].status = "Not Automated";
                }

                ////Get all unread mails
                //Dictionary<DateTime, Dictionary<string, string>> MailDetails = OutlookMail.GetMailDetails(EmailID, EmailPassword);

                ////Get all hyperlinks in the selected mail 
                //KeyValuePair<DateTime, Dictionary<string, string>> RecentMail = (from mail in MailDetails
                //                                                                 where mail.Value["Subject"].Contains("Emailed Study")
                //                                                                 select mail).LastOrDefault();
                //IList<String> HyperLinks = login.GetHyperLinkList(RecentMail.Value["Body"]);

                ////Navigate to the url to open emailed study from mail hyperlink
                //String mailURL = HyperLinks.Where(link => link.Contains(Config.IConnectIP) && link.Contains("Guest")).FirstOrDefault();
                //login.DriverGoTo(mailURL);

                ////Enter Pin details
                //BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(login.OkButton()));
                //login.PinNumberTextBox().SendKeys(pinnumber);
                //login.OkButton().Click();
                //PageLoadWait.WaitForPageLoad(20);
                //BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("iframe#GuestHomeFrame")));
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("GuestHomeFrame");

                //Boolean Status_12 = viewer.StudyDetailsInViewer()["Accession"].Equals(ColumnValues[1]);

                ////Navigate to HistoryPanel
                //viewer.NavigateToHistoryPanel();
                //viewer.ChooseColumns(new string[] { "Accession" });

                ////Get Study details
                ////StudyDetails = viewer.GetMatchingRow("Accession", AccessionID);
                //String[] ColumnValues_12 = BasePage.GetColumnValues(BasePage.GetSearchResults(), "Accession", BasePage.GetColumnNames());

                ////Step 12 :- Validate email link 
                //if (Status_12 && ColumnValues_12[0].Equals(ColumnValues[1]))
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                ////Select other study
                //viewer.SelectStudy1("Accession", ColumnValues_12[1]);

                ////Get Attachment List for reference
                //Dictionary<int, string[]> attachmentsList_ref = viewer.StudyViewerListResults("patienthistory", "attachment");
                //String[] AttachColumnNames_ref = viewer.StudyViewerListColumnNames("patienthistory", "attachment", 0);
                //String[] AttachColumnValues_ref = BasePage.GetColumnValues(attachmentsList_ref, "Name", AttachColumnNames_ref);

                ////Select another study in left panel
                //viewer.OpenPriors(new string[] { "Accession" }, new string[] { ColumnValues_12[1] });

                ////Step 13 :- 
                //result.steps[++ExecutedSteps].status = "In Hold";

                ////Set newly opened study panel be active
                //PageLoadWait.WaitForAllViewportsToLoad(30, 2);
                //PageLoadWait.WaitForFrameLoad(10);
                //viewer.Thumbnails(2)[0].Click();

                ////Change Layout to 2x3 viewer
                //viewer.SelectToolInToolBar("SeriesViewer2x3");

                ////Step 14:- Change layout and validate the changes
                //if (viewer.SeriesViewer_2X2(2).Displayed && viewer.SeriesViewer_2X3(2).Displayed)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                ////Click any one empty view port to activate it
                //IList<IWebElement> EmptyPorts = viewer.GetEmptyViewports(2);
                //IWebElement Viewport;
                //if (EmptyPorts.Count != 0)
                //{
                //    Viewport = EmptyPorts[0];
                //}
                //else
                //{
                //    throw new Exception("Empty Viewport not found in newly opened study..");
                //}
                //Viewport.Click();

                ////Double click any one thumbnail to open in active viewport
                //Actions action = new Actions(BasePage.Driver);
                //action.DoubleClick(viewer.Thumbnails(2)[0]).Build().Perform();
                //PageLoadWait.WaitForPageLoad(10);
                //PageLoadWait.WaitForAllViewportsToLoad(30, 2);

                ////Step 15 :- Validate selected series is opened in the active viewport
                //if (viewer.GetInnerAttribute(Viewport, "src", '&', "seriesUID")
                //    == viewer.GetInnerAttribute(viewer.Thumbnails(2)[0], "src", '&', "seriesUID"))
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                ////Navigate to HistoryPanel
                //viewer.NavigateToHistoryPanel();

                ////Navigate to Attachment tab
                //viewer.NavigateTabInHistoryPanel("Attachment");

                ////Get Attachment List
                //Dictionary<int, string[]> attachmentsList_16 = viewer.StudyViewerListResults("patienthistory", "attachment");
                //String[] AttachColumnNames_16 = viewer.StudyViewerListColumnNames("patienthistory", "attachment", 0);
                //String[] AttachColumnValues_16 = BasePage.GetColumnValues(attachmentsList_16, "Name", AttachColumnNames_16);

                ////Compare Attachments
                //Boolean AttachmentsMatch_16 = Array.Exists(AttachColumnValues_16, attachment => attachment.Equals(AttachColumnValues_ref[CountIndex++]));

                ////Step 16 :- Compare attachments of the same study here and before
                //if (AttachmentsMatch_16)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                ////Upload attachment 
                //Boolean UploadStatus_2 = viewer.UploadAttachment(AttachmentFilePath[1], 1);

                ////Select another study
                //viewer.SelectStudy1("Accession", ColumnValues_12[0]);

                ////Get Attachments 
                //Dictionary<int, string[]> attachmentsList_4 = viewer.StudyViewerListResults("patienthistory", "attachment");
                //String[] AttachColumnNames_4 = viewer.StudyViewerListColumnNames("patienthistory", "attachment", 0);
                //String[] AttachColumnValues_4 = BasePage.GetColumnValues(attachmentsList_4, "Name", AttachColumnNames_4);
                //String filename = AttachmentFilePath[1].Split('\\')[AttachmentFilePath[1].Split('\\').Length - 1];

                ////Compare attachments of same study
                //CountIndex = 0;
                //Boolean AttachmentsMatch_2 = Array.Exists(AttachColumnValues_4, attachment => attachment.Equals(AttachColumnValues_3[CountIndex++]));

                ////Step 17 :- Uplaod an attachment file and verify it's correctness
                //if (AttachmentsMatch_2 && !UploadStatus_2)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                ////Wait until Upload operation completes
                //int counter = 0;
                //while (viewer.AttachmentUploadIcon().Displayed && counter++ < 15)
                //{
                //    Thread.Sleep(1000);
                //}

                ////Check currenly uploaded attachment in other study
                //CountIndex = 0;
                //Boolean AttachmentsMatch_3 = Array.Exists(AttachColumnValues_4, attachment => attachment.Equals(filename));

                ////Step 18 :- Validate attachments of currently selected study not contains recently uploaded attachment
                //if (!AttachmentsMatch_3)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                ////ReSelect the previous selected study
                //viewer.SelectStudy1("Accession", ColumnValues_12[1]);

                ////Get Attachments 
                //Dictionary<int, string[]> attachmentsList_5 = viewer.StudyViewerListResults("patienthistory", "attachment");
                //String[] AttachColumnNames_5 = viewer.StudyViewerListColumnNames("patienthistory", "attachment", 0);
                //String[] AttachColumnValues_5 = BasePage.GetColumnValues(attachmentsList_5, "Name", AttachColumnNames_5);

                ////Add recent attachment filname to other study's attachment list
                //String[] Attachments = AttachColumnValues_2;
                //Attachments[Attachments.Length] = filename;

                ////Compare attachments of selected study
                //CountIndex = 0;
                //Boolean AttachmentsMatch_4 = Array.Exists(AttachColumnValues_5, attachment => attachment.Equals(Attachments));

                ////Step 19 :-Validate newly attached file should present in attachments list
                //if (AttachmentsMatch_4)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                ////Step 20 :- Open attachment in the link
                //result.steps[++ExecutedSteps].status = "Not Automated";

                ////Select another study
                //viewer.SelectStudy1("Accession", ColumnValues_12[0]);

                ////Get Attachments 
                //Dictionary<int, string[]> AttachmentResults_other = viewer.StudyViewerListResults("patienthistory", "attachment");
                //String[] AttachmentColumns_other = viewer.StudyViewerListColumnNames("patienthistory", "attachment", 0);
                //String[] AttachmentNames_other = BasePage.GetColumnValues(AttachmentResults_other, "Name", AttachmentColumns_other);

                ////Compare attachments of same study
                //CountIndex = 0;
                //Boolean AttachmentsMatch_other = Array.Exists(AttachmentNames_other, attachment => attachment.Equals(filename));

                ////Step 21 :- Uplaod an attachment file and verify it's correctness
                //if (!AttachmentsMatch_other)
                //{
                //    result.steps[++ExecutedSteps].status = "Pass";
                //    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                //}
                //else
                //{
                //    result.steps[++ExecutedSteps].status = "Fail";
                //    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                //    result.steps[ExecutedSteps].SetLogs();
                //}

                ////Close Study and logout iCA
                //studies.CloseStudy();
                //login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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

                //Close Study and logout iCA
                //studies.CloseStudy();
                login.Logout();

                //Return Result
                return result;
            }

        }
        
        /// <summary>
        /// Multiple History Panel
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_27934(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables
            Studies studies = null;
            StudyViewer viewer = null;
            DomainManagement domainmgmt = null;
            RoleManagement rolemgmt = null;
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String AdminUserName = Config.adminUserName;
                String AdminPassword = Config.adminPassword;
                String PatientID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
                String AccessionIDList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
                String ModalityList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
                String Domain = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
                String[] Modality = ModalityList.Split('=');
                String[] AccessionID = AccessionIDList.Split(':');

                //String[] CTtools = Array.ConvertAll<Object, String>(new Object[] { IEnum.ViewerTools.WindowLevel, IEnum.ViewerTools.AllinOneTool, IEnum.ViewerTools.AngleMeasurement, IEnum.ViewerTools.TransischialMeasurement, IEnum.ViewerTools.JointLineMeasurement, IEnum.ViewerTools.LineMeasurement }, x => x.ToString());
                //String[] MRtools = Array.ConvertAll<Object, String>(new Object[] { IEnum.ViewerTools.Pan, IEnum.ViewerTools.LocalizerLine, IEnum.ViewerTools.Reset }, x => x.ToString());
                //String[] PTtools = Array.ConvertAll<Object, String>(new Object[] { IEnum.ViewerTools.WindowLevel, IEnum.ViewerTools.Pan, IEnum.ViewerTools.Zoom, IEnum.ViewerTools.RotateClockwise, IEnum.ViewerTools.RotateCounterclockwise, 
                                   //IEnum.ViewerTools.AngleMeasurement, IEnum.ViewerTools.TransischialMeasurement, IEnum.ViewerTools.JointLineMeasurement, IEnum.ViewerTools.LineMeasurement, IEnum.ViewerTools.SaveAnnotatedImages, IEnum.ViewerTools.Reset }, x => x.ToString());

                String[] CTtools = new String[] { "Window Level", "All in One Tool" , "Angle Measurement", "Transischial Measurement", "Joint Line Measurement", "Line Measurement" };
                String[] MRtools = new String[] { "Pan", "Localizer Line", "Reset" };
                String[] PTtools = new String[] { "Window Level" , "Pan", "Zoom", "Rotate Clockwise", "Rotate Counterclockwise" , "Angle Measurement", "Transischial Measurement" , "Joint Line Measurement", "Line Measurement", "Save Annotated Images", "Reset" };
                String PACSA7 = login.GetHostName(Config.SanityPACS);
                String EA91 = login.GetHostName(Config.EA91);
                //PreCondition: -Configure tools for different modalities
                //Login as Administrator
                login.LoginIConnect(AdminUserName, AdminPassword);

                //Navigate to Domain Management
                domainmgmt = (DomainManagement)login.Navigate("DomainManagement");

                //Navigate to Edit domain page
                domainmgmt.SearchDomain(Domain);
                domainmgmt.SelectDomain(Domain);
                domainmgmt.ClickEditDomain();

                //Re-enable report,requistion & attachment (is disabled)
                domainmgmt.SetCheckBoxInEditDomain("reportview", 0);
                domainmgmt.SetCheckBoxInEditDomain("requisitionreport", 0);
                domainmgmt.SetCheckBoxInEditDomain("attachment", 0);
                domainmgmt.SetCheckBoxInEditDomain("attachmentupload", 0);

                //Configure tool bar per modality
                domainmgmt.ConfigureModalityToolbar("CT", CTtools);
                domainmgmt.ConfigureModalityToolbar("MR", MRtools);
                domainmgmt.ConfigureModalityToolbar("PT", PTtools);

                //Save changes
                domainmgmt.ClickSaveDomain();

                //Navigate to Role Management tab
                rolemgmt = (RoleManagement)login.Navigate("RoleManagement");

                //Navigate to edit role page
                rolemgmt.SearchRole("SuperRole", Domain);
                rolemgmt.SelectRole("SuperRole");
                rolemgmt.ClickEditRole();

                //Check Use Domain setting checkbox (if selected)
                PageLoadWait.WaitForFrameLoad(5);
                if (!rolemgmt.UseDomainSetting_toolbar().Selected) { rolemgmt.UseDomainSetting_toolbar().Click(); }
                if (!rolemgmt.UseDomainSetting_modality().Selected) { rolemgmt.UseDomainSetting_modality().Click(); }

                //Save changes
                rolemgmt.ClickSaveRole();

                //Logout
                login.Logout();
                ExecutedSteps++;

                //Login as Administrator
                login.LoginIConnect(AdminUserName, AdminPassword);

                //Navigate to Studies
                studies = (Studies)login.Navigate("Studies");

                //Search, Select study
                studies.SearchStudy(patientID: PatientID, Modality: Modality[0],Datasource: PACSA7);
                studies.SelectStudy("Patient ID", PatientID);

                //Launch study in viewer
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Take Screenshot - 1
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                Boolean ViewerStatus2_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());
                                
                //Navigate to HistoryPanel
                viewer.NavigateToHistoryPanel();

                //Select second study to open in another study panel
                viewer.ChooseColumns(new string[] { "# Images"});
                viewer.OpenPriors(new string[] { "Patient ID", "Modality","# Images" }, new string[] { PatientID, Modality[1] ,"82"});
                PageLoadWait.WaitForAllViewportsToLoad(30, 2);

                //Take Screenshot - 2
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                Boolean ViewerStatus2_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer(), 2, 1);
                
                //Step 2 :- Validate viewer with both single and two prior studies in viewer
                if (ViewerStatus2_1 && ViewerStatus2_2 && viewer.studyPanel(2).Displayed)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Click any one empty view port to activate it
                PageLoadWait.WaitForFrameLoad(10);
                viewer.studyPanel(2).Click();
                IList<IWebElement> EmptyPorts = viewer.GetEmptyViewports(2);
                IWebElement Viewport;
                if (EmptyPorts.Count != 0)
                {
                    Viewport = EmptyPorts[0];
                }
                else
                {
                    Viewport = viewer.SeriesViewPorts(2)[0];
                }
                Viewport.Click();

                //Double click any one thumbnail to open in active viewport
                Actions action = new Actions(BasePage.Driver);
                //action.DoubleClick(viewer.Thumbnails(2)[0]).Build().Perform();
                viewer.DoubleClick(viewer.Thumbnails(2)[0]);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Verify Selected series is opened correctly
                Boolean seriesSelected = viewer.GetInnerAttribute(Viewport, "src", '&', "seriesUID")
                    .Equals(viewer.GetInnerAttribute(viewer.Thumbnails(2)[0], "src", '&', "seriesUID"));

                //Get Empty viewports
                IWebElement DragViewport;
                IList<IWebElement> EmptyPorts0 = viewer.GetEmptyViewports(2);

                //Drag and drop a series in empty viewport
                if (EmptyPorts0.Count != 0)
                {
                    DragViewport = EmptyPorts0[0];
                }
                else
                {
                    DragViewport = viewer.SeriesViewPorts(2)[0];
                }
                action.DragAndDrop(viewer.Thumbnails(2)[1], DragViewport).Build().Perform();                
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                Boolean seriesDragged = viewer.GetInnerAttribute(DragViewport, "src", '&', "seriesUID")
                    .Equals(viewer.GetInnerAttribute(viewer.Thumbnails(2)[1], "src", '&', "seriesUID"));

                //Step 3 :- Validate both selected and dragged series is opened in the empty/first viewport
                if (seriesSelected && seriesDragged)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Perform Pan 
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewer.DragMovement(viewer.SeriesViewer_2X2(2));

                //Validate Images 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool PanImage = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_2X2(2));

                //Step 4 :- Select any one tool in review/study tool bar and check it's correctness   
                if (PanImage)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);

                //Close Study panel of second study
                //viewer.StudyPanelCloseBtn().Click();
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", viewer.StudyPanelCloseBtn());

                //Step 5 :- Close another study panel and valiadate
                if (!viewer.studyPanel(2).Displayed)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Get current layout
                int viewportLayout_1 = viewer.SeriesViewPorts().Count;

                //Double Click one viewport and get current layout                
                login.DoubleClick(viewer.SeriesViewPorts()[0]);                
                int viewportLayout_2 = viewer.SeriesViewPorts().Count;
                PageLoadWait.WaitForAllViewportsToLoad(10);

                //Again Double Click one viewport and get current layout
                login.DoubleClick(viewer.SeriesViewPorts()[0]);
                int viewportLayout_3 = viewer.SeriesViewPorts().Count;
                PageLoadWait.WaitForAllViewportsToLoad(10);

                //Step 6 :- Validate study panel layout with any viewport double clicked
                if (viewportLayout_2 == 1 && viewportLayout_1 == viewportLayout_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close Study 
                studies.CloseStudy();

                //Search, Select study
                BasePage.Driver.FindElement(By.CssSelector("input#m_studySearchControl_m_clearButton")).Click();
                studies.SearchStudy(AccessionNo:AccessionID[0],Datasource:EA91);
                studies.SelectStudy("Accession", AccessionID[0]);

                //Launch study in viewer
                viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);


                //Take Screenshot
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                Boolean ViewerStatus7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                //Retreiving study panel details
                String[] ViewportsUID_1 = viewer.GetSeriesUID(viewer.SeriesViewPorts());
                String[] ThumbnailsUID_1 = viewer.GetSeriesUID(viewer.Thumbnails());

                String[] CTtoolsList = viewer.GetStudyToolsinViewer();
                //Boolean IsToolsExist_7 = Array.Exists(CTtoolsList, x => Regex.Replace(x, @"\s+", "").Equals(CTtools));
                Boolean IsToolsExist_7 = CTtools.All(item => CTtoolsList.Contains(item));

                //Step 7 :- Launch study and verify it's correctness
                if (ViewerStatus7 && viewer.studyPanel(1).Displayed && IsToolsExist_7)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Navigate to HistoryPanel
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });

                //Select second study to open in another study panel
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { AccessionID[1] });
                PageLoadWait.WaitForAllViewportsToLoad(30, 2);

                //Get tools in modality tool bar
                viewer.SeriesViewer_1X1(2).Click();
                String[] MRtoolsList = viewer.GetStudyToolsinViewer();
                //Boolean IsToolsExist_8 = Array.Exists(MRtoolsList, x => Regex.Replace(x, @"\s+", "").Equals(MRtools));
                Boolean IsToolsExist_8 = MRtools.All(item => MRtoolsList.Contains(item));

                //Take Screenshot
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                Boolean ViewerStatus8 = studies.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                //Step 8 :- Validate Second study details loaded from history panel
                if (ViewerStatus8 && viewer.studyPanel(2).Displayed && IsToolsExist_8)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Navigate to HistoryPanel
                viewer.NavigateToHistoryPanel();
                
                //Select second study to open in another study panel
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { AccessionID[2] });
                PageLoadWait.WaitForAllViewportsToLoad(30, 3);
                PageLoadWait.WaitForPageLoad(60);

                //Retreiving third study panel details
                viewer.SeriesViewer_1X1(3).Click();
                String[] ViewportsUID_3 = viewer.GetSeriesUID(viewer.SeriesViewPorts(3));
                String[] ThumbnailsUID_3 = viewer.GetSeriesUID(viewer.Thumbnails(3));

                //Get tools in modality tool bar
                String[] PTtoolsList = viewer.GetStudyToolsinViewer();
                //Boolean IsToolsExist_9 = Array.Exists(PTtoolsList, x => Regex.Replace(x, @"\s+", "").Equals(PTtools));
                Boolean IsToolsExist_9 = PTtools.All(item => PTtoolsList.Contains(item));

                //Take Screenshot
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                Boolean ViewerStatus9 = studies.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                //Step 9 :- Validate third study details loaded from history panel
                if (ViewerStatus9 && viewer.studyPanel(3).Displayed && IsToolsExist_9)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Navigate to HistoryPanel
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });

                //Select fourth study to open in another study panel
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { AccessionID[3] });
                PageLoadWait.WaitForAllViewportsToLoad(30, 3);
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);

                //Retreiving third study panel details
                PageLoadWait.WaitForPageLoad(30);
                String[] ViewportsUID_4 = viewer.GetSeriesUID(viewer.SeriesViewPorts(3));
                String[] ThumbnailsUID_4 = viewer.GetSeriesUID(viewer.Thumbnails(3));

                Boolean PanelUIDMatch = Array.Exists(ViewportsUID_3, UID => UID.Equals(ViewportsUID_4))
                    || Array.Exists(ThumbnailsUID_3, UID => UID.Equals(ThumbnailsUID_4));

                //Take Screenshot
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                Boolean PanelStatus = studies.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                //Step 10 :- Validate maximum no of studies loaded in study viewer
                if (!PanelUIDMatch && PanelStatus)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Get current layout
                int viewportLayout_3_1 = viewer.SeriesViewPorts(3).Count;

                //Double Click one viewport and get current layout
                viewer.DoubleClick(viewer.SeriesViewPorts(3)[0]);
                PageLoadWait.WaitForPageLoad(180);
                int viewportLayout_3_2 = viewer.SeriesViewPorts(3).Count;

                //Again Double Click one viewport and get current layout
                PageLoadWait.WaitForPageLoad(180);
                viewer.DoubleClick(viewer.SeriesViewPorts(3)[0]);
                PageLoadWait.WaitForPageLoad(180);
                PageLoadWait.WaitForViewportPanelToLoad(40,viewer.SeriesViewer_1X2(3));
                int viewportLayout_3_3 = viewer.SeriesViewPorts(3).Count;
                PageLoadWait.WaitForPageLoad(60);

                //Step 11 :- Validate third study panel layout with any viewport double clicked
                if (viewportLayout_3_2 == 1 && viewportLayout_3_1 == viewportLayout_3_3)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Perform Pan 
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                viewer.DragMovement(viewer.SeriesViewer_2X2(1));

                //Validate Images 
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                bool PanImage_12 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_2X2(1));

                //Step 12 :- Select any one tool in review/study tool bar and check it's correctness   
                if (PanImage_12)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);

                //Get dimensions
                PageLoadWait.WaitForAllViewportsToLoad(15);

                int ToolsBox_1,Thumbnails_1,StudyPanel_1;
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                {
                    ToolsBox_1 = Int32.Parse(viewer.GetInnerAttribute(viewer.ReviewtoolBar(), "style", ';', "HEIGHT", ":").Replace("px", "").Trim());
                    Thumbnails_1 = Int32.Parse(viewer.GetInnerAttribute(viewer.ThumbnailContainer(), "style", ';', "WIDTH", ":").Replace("px", "").Trim());
                    StudyPanel_1 = Int32.Parse(viewer.GetInnerAttribute(viewer.ViewerContainer(), "style", ';', "WIDTH", ":").Replace("px", "").Trim());
                }
                else
                {
                    ToolsBox_1 = Int32.Parse(viewer.GetInnerAttribute(viewer.ReviewtoolBar(), "style", ';', "height", ":").Replace("px", "").Trim());
                    Thumbnails_1 = Int32.Parse(viewer.GetInnerAttribute(viewer.ThumbnailContainer(), "style", ';', "width", ":").Replace("px", "").Trim());
                    StudyPanel_1 = Int32.Parse(viewer.GetInnerAttribute(viewer.ViewerContainer(), "style", ';', "width", ":").Replace("px", "").Trim());
                }

                //Resize window size
                BasePage.Driver.Manage().Window.Size = new Size(1000, 1000);

                //Get dimensions
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(15);
                PageLoadWait.WaitForViewportPanelToLoad(40,viewer.SeriesViewer_2X1(3));

                int ToolsBox_2, Thumbnails_2, StudyPanel_2;
                 if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                {
                    ToolsBox_2 = Int32.Parse(viewer.GetInnerAttribute(viewer.ReviewtoolBar(), "style", ';', "HEIGHT", ":").Replace("px", "").Trim());
                    Thumbnails_2 = Int32.Parse(viewer.GetInnerAttribute(viewer.ThumbnailContainer(), "style", ';', "WIDTH", ":").Replace("px", "").Trim());
                    StudyPanel_2 = Int32.Parse(viewer.GetInnerAttribute(viewer.ViewerContainer(), "style", ';', "WIDTH", ":").Replace("px", "").Trim());
                }
                else
                {
                    ToolsBox_2 = Int32.Parse(viewer.GetInnerAttribute(viewer.ReviewtoolBar(), "style", ';', "height", ":").Replace("px", "").Trim());
                    Thumbnails_2 = Int32.Parse(viewer.GetInnerAttribute(viewer.ThumbnailContainer(), "style", ';', "width", ":").Replace("px", "").Trim());
                    StudyPanel_2 = Int32.Parse(viewer.GetInnerAttribute(viewer.ViewerContainer(), "style", ';', "width", ":").Replace("px", "").Trim());
                }
                //Take Screenshot
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                Boolean ResizedWindow = studies.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                //Step 13 :- Resize browser window size and verify auto fit for all study panels
                if (ToolsBox_1 <= ToolsBox_2 && Thumbnails_1 > Thumbnails_2 && StudyPanel_1 > StudyPanel_2
                    && ResizedWindow)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog(ToolsBox_1 + "--" + ToolsBox_2 + "--" + Thumbnails_1 + "--" + Thumbnails_2 + "--" + StudyPanel_1 + "--" + StudyPanel_2 +"--"+ ResizedWindow);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close second study panel
                //viewer.StudyPanelCloseBtn().Click();
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", viewer.StudyPanelCloseBtn());
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(60);                
                PageLoadWait.WaitForViewportPanelToLoad(40, viewer.SeriesViewer_2X1(1));
                PageLoadWait.WaitForViewportPanelToLoad(40, viewer.SeriesViewer_2X1(3));
                PageLoadWait.WaitForAllViewportsToLoad(60);

                //Take Screenshot
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                Boolean SecondPanelClosed = studies.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                //Step 14 :- Close Second study panel and check it's correctness
                if (!viewer.studyPanel(2).Displayed && SecondPanelClosed)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Maximize window
                BasePage.Driver.Manage().Window.Maximize();

                //Get dimensions
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(60);               
                PageLoadWait.WaitForViewportPanelToLoad(40, viewer.SeriesViewer_2X1(1));
                PageLoadWait.WaitForViewportPanelToLoad(40, viewer.SeriesViewer_2X1(3));
                PageLoadWait.WaitForAllViewportsToLoad(60);
                int ToolsBox_3, Thumbnails_3, StudyPanel_3;

                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("8"))
                {
                    ToolsBox_3 = Int32.Parse(viewer.GetInnerAttribute(viewer.ReviewtoolBar(), "style", ';', "HEIGHT", ":").Replace("px", "").Trim());
                    Thumbnails_3 = Int32.Parse(viewer.GetInnerAttribute(viewer.ThumbnailContainer(), "style", ';', "WIDTH", ":").Replace("px", "").Trim());
                    StudyPanel_3 = Int32.Parse(viewer.GetInnerAttribute(viewer.ViewerContainer(), "style", ';', "WIDTH", ":").Replace("px", "").Trim());
                }
                else
                {
                    ToolsBox_3 = Int32.Parse(viewer.GetInnerAttribute(viewer.ReviewtoolBar(), "style", ';', "height", ":").Replace("px", "").Trim());
                    Thumbnails_3 = Int32.Parse(viewer.GetInnerAttribute(viewer.ThumbnailContainer(), "style", ';', "width", ":").Replace("px", "").Trim());
                    StudyPanel_3 = Int32.Parse(viewer.GetInnerAttribute(viewer.ViewerContainer(), "style", ';', "width", ":").Replace("px", "").Trim());
                }
                //Take Screenshot
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                Boolean MaximizedWindow = studies.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                //Step 15 :- Maximize window and Verify window's correctness
                if (ToolsBox_3 == ToolsBox_1 && MaximizedWindow)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    Logger.Instance.InfoLog(ToolsBox_3 +"----"+ ToolsBox_1);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Navigate to HistoryPanel
                viewer.NavigateToHistoryPanel();
                viewer.ChooseColumns(new string[] { "Accession" });

                //Select first study to open in another study panel
                viewer.OpenPriors(new string[] { "Accession" }, new string[] { AccessionID[0] });
                PageLoadWait.WaitForAllViewportsToLoad(30, 3);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);

                //Take Screenshot
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                Boolean PanelStatus_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                //Step 16 :- Open any study in history panel and validate the loading of study in third panel
                if (viewer.StudyDetailsInViewer()["Accession"].Equals(AccessionID[0]) && PanelStatus_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close third study panel
                viewer.StudyPanelCloseBtn(3).Click();
                //((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", viewer.StudyPanelCloseBtn(3));
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(40);
                PageLoadWait.WaitForAllViewportsToLoad(10,2);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForViewportPanelToLoad(40,viewer.SeriesViewer_2X1(2));

                //Take Screenshot
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                Boolean ThirdPanelClosed = studies.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                //Step 17 :- Close Third study panel and check it's correctness
                if (!viewer.studyPanel(3).Displayed && ThirdPanelClosed && viewer.studyPanel(2).Displayed
                    && viewer.studyPanel(1).Displayed)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close second study panel
                //viewer.StudyPanelCloseBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(30);
                //((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", viewer.StudyPanelCloseBtn());
                viewer.StudyPanelCloseBtn().Click();
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);

                //Take Screenshot
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                Boolean SecondPanelClosed_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());


                //Step 18 :- Close second study panel and check it's correctness
                if (!viewer.studyPanel(3).Displayed && !viewer.studyPanel(2).Displayed
                    && viewer.studyPanel(1).Displayed && SecondPanelClosed_2)
                {
                    result.steps[ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("Test Step Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Close Study and logout iCA
                studies.CloseStudy();
                login.Logout();

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

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

                //Close Study and logout iCA
                //studies.CloseStudy();
                login.Logout();

                //Return Result
                return result;
            }

        }
                        
        /// <summary>
        /// Cleanup script to close browser
        /// </summary>
        public void Test_Cleanup()
        {
            login.CloseBrowser();
        }

    }
}
