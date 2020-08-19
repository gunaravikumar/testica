using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Reusable.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TestStack.White.InputDevices;
using TestStack.White.WindowsAPI;

namespace Selenium.Scripts.Tests
{
    class Settings3DViewer : BasePage
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public Imager imager = new Imager();
        DirectoryInfo CurrentDir = new DirectoryInfo(System.IO.Directory.GetCurrentDirectory());
        public Settings3DViewer(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }

        public TestCaseResult Test_163389(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            BluRingViewer bluview = new BluRingViewer();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string imagethumb = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string requirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] values = requirements.Split('_');

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, imagethumb);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study");
                    throw new Exception("Failed to open study");
                }

                //step 02
                bluview.UserSettings("select", "3D Settings");
                PageLoadWait.WaitForElementToDisplay(brz3dvp.overlaypane());
                try
                {
                    wait.Until(ExpectedConditions.TextToBePresentInElement(brz3dvp.overlaypane(), "Settings"));
                    Logger.Instance.InfoLog("Setting text in found");
                    wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(By.CssSelector(Locators.CssSelector.SettingsValues)));
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Failed to wait for Setting Dialog" + ex.ToString());
                }
                PageLoadWait.WaitForFrameLoad(10);
                if (brz3dvp.overlaypane().Displayed)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Settings panel not found");

                //step 03
                IWebElement closebtn = Driver.FindElement(By.CssSelector(Locators.CssSelector.OverLayPane + " " + Locators.CssSelector.CloseSelectedToolBox));
                closebtn.Click();
                PageLoadWait.WaitForFrameLoad(10);
                int counter = 0;
                try
                {
                    if (!brz3dvp.overlaypane().Displayed)
                        counter++;
                }
                catch (Exception over)
                {
                    counter++;
                }
                if (counter > 0)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("3D Settings not closed");

                //step 04
                bluview.ClickOnUSerSettings();
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.activetool + " " + Locators.CssSelector.dropdownactive)));
                IList<IWebElement> listelements = Driver.FindElements(By.CssSelector(Locators.CssSelector.activetool + " li"));
                Logger.Instance.InfoLog("Shift button pressed");
                foreach (IWebElement element in listelements)
                {
                    if (element.Text.ToLower().Contains("3d settings"))
                    {
                        new Actions(Driver).KeyDown(Keys.Shift).Click(element).KeyUp(Keys.Shift).Perform();
                        break;
                    }
                }
                PageLoadWait.WaitForFrameLoad(5);
                if (brz3dvp.overlaypane().Displayed)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Settings panel not available");

                //step 05 & 06
                IList<IWebElement> contentlabels = Driver.FindElements(By.CssSelector(Locators.CssSelector.labelcontent));
                int inc = 0;
                foreach (IWebElement element in contentlabels)
                {
                    if (element.Text.ToLower().Trim().Contains("advanced") && element.Enabled)
                    {
                        element.Click();
                        inc++;
                    }
                }
                if (inc == 1)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    throw new Exception("Advanced tab not available");

                //step 07
                PageLoadWait.WaitForFrameLoad(5);
                int itr = 0;
                IList<IWebElement> list2 = Driver.FindElements(By.CssSelector(Locators.CssSelector.SettingsValues));
                foreach (IWebElement element in list2)
                {
                    IWebElement value1 = element.FindElement(By.CssSelector(Locators.CssSelector.leftcontent));
                    if (value1.Text.ToLower().Contains("display frame per second"))
                    {
                        IWebElement rightele = element.FindElement(By.CssSelector(Locators.CssSelector.CheckBox));
                        if (rightele.Displayed)
                            itr++;
                    }
                    else if (value1.Text.ToLower().Contains("log level"))
                    {
                        IWebElement rightele = element.FindElement(By.CssSelector(Locators.CssSelector.matlist));
                        IWebElement infooption = null;
                        if (rightele.Displayed)
                        {
                            int flag = 0;
                            rightele.Click();
                            try
                            {
                                IList<String> dropdownvalues = new List<String>();

                                String Csselement = Locators.CssSelector.Warning + " " + Locators.CssSelector.DropDown3DBox;
                                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Csselement)));
                                IList<IWebElement> dropdownelements = Driver.FindElements(By.CssSelector(Locators.CssSelector.Layoutlist));
                                foreach (IWebElement dropdown in dropdownelements)
                                {
                                    dropdownvalues.Add(dropdown.Text.Trim().ToLower());
                                    if (dropdown.Text.Trim().ToLower().Contains("info"))
                                    {
                                        infooption = dropdown;
                                    }
                                }
                                foreach (String ddval in dropdownvalues)
                                {
                                    foreach (String sval in values)
                                    {
                                        if (sval.Equals(ddval))
                                        {
                                            flag++;
                                        }
                                    }
                                }
                                if (flag == 8)
                                {
                                    infooption.Click();
                                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Csselement)));
                                    itr++;
                                }
                            }
                            catch (Exception a)
                            {
                                Logger.Instance.ErrorLog("log level not found");
                            }
                        }
                    }
                }
                if (itr == 2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 08
                IList<IWebElement> buttons = Driver.FindElements(By.CssSelector(Locators.CssSelector.ConfirmButton));
                foreach (IWebElement button in buttons)
                {
                    if (button.Text.ToLower().Contains("cancel"))
                    {
                        button.Click();
                        break;
                    }
                }
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.OverLayPane)));
                try
                {
                    if (!brz3dvp.overlaypane().Displayed)
                        result.steps[++ExecutedSteps].StepPass();
                    else
                        result.steps[++ExecutedSteps].StepFail();
                }
                catch (Exception o)
                {
                    Logger.Instance.InfoLog("Setting overlay panel unavailable");
                    result.steps[++ExecutedSteps].StepPass();
                }

                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
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
                brz3dvp.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163391(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            BluRingViewer bluview = new BluRingViewer();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string imagethumb = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string requirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, imagethumb, layout: BluRingZ3DViewerPage.CurvedMPR);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study in Curved MPR Layout");
                    throw new Exception("Failed to open study in Curved MPR Layout");
                }

                //step 02
                Result = brz3dvp.change3dsettings("Flip", check: false);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 03
                int counter = 0;
                IWebElement Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                Result = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual);
                if (Result)
                {
                    new Actions(Driver).SendKeys("x").Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    int blucolorbefore3 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 31, 0, 0, 255);
                    new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 4).Click().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 10, (Navigation1.Size.Height / 4) + 20).Click().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 20, (Navigation1.Size.Height / 4)).Click().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    int blucolorafter3 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 32, 0, 0, 255);
                    if (blucolorafter3 > blucolorbefore3)
                    {
                        String locationvaluebefore3 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                        brz3dvp.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, ScrollDirection: "down", scrolllevel: 20, Thickness: "n");
                        PageLoadWait.WaitForFrameLoad(10);
                        String locationvalueafter3 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                        if (locationvalueafter3 != locationvaluebefore3)
                        {
                            brz3dvp.select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(10);
                            String locationvalue3 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                            if (locationvalue3.Equals(requirements))
                            {
                                bool check3 = brz3dvp.checkflipstatus(BluRingZ3DViewerPage._3DPathNavigation, false);
                                if (check3)
                                    result.steps[++ExecutedSteps].StepPass();
                                else
                                    result.steps[++ExecutedSteps].StepFail();
                            }
                        }
                        else
                            result.steps[++ExecutedSteps].StepFail();
                    }
                    else
                        throw new Exception("Failed to draw path in navigation1");
                }
                else
                    throw new Exception("Failed to select curved drawing tool");

                //step 04
                Result = brz3dvp.change3dsettings("Flip");
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 05
                int counter1 = 0;
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                Result = brz3dvp.select3DTools(Z3DTools.Curve_Drawing_Tool_1_Manual);
                if (Result)
                {
                    int blucolorbefore5 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 51, 0, 0, 255);
                    new Actions(Driver).MoveToElement(Navigation1, Navigation1.Size.Width / 2, Navigation1.Size.Height / 4).Click().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 10, (Navigation1.Size.Height / 4) + 20).Click().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    new Actions(Driver).MoveToElement(Navigation1, (Navigation1.Size.Width / 2) + 20, (Navigation1.Size.Height / 4)).Click().Build().Perform();
                    PageLoadWait.WaitForFrameLoad(10);
                    int blucolorafter5 = brz3dvp.LevelOfSelectedColor(Navigation1, testid, ExecutedSteps + 52, 0, 0, 255);
                    if (blucolorafter5 > blucolorbefore5)
                    {
                        String locationvaluebefore5 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                        brz3dvp.ScrollInView(BluRingZ3DViewerPage._3DPathNavigation, ScrollDirection: "down", scrolllevel: 20, Thickness: "n");
                        PageLoadWait.WaitForFrameLoad(10);
                        String locationvalueafter5 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                        if (locationvalueafter5 != locationvaluebefore5)
                        {
                            brz3dvp.select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(10);
                            String locationvalue5 = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage._3DPathNavigation);
                            if (locationvalue5.Equals(requirements))
                            {
                                bool check3 = brz3dvp.checkflipstatus(BluRingZ3DViewerPage._3DPathNavigation, true);
                                if (check3)
                                    result.steps[++ExecutedSteps].StepPass();
                                else
                                    result.steps[++ExecutedSteps].StepFail();
                            }
                        }
                        else
                            result.steps[++ExecutedSteps].StepFail();
                    }
                    else
                        throw new Exception("Failed to draw path in navigation1");
                }
                else
                    throw new Exception("Failed to select curved drawing tool");

                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
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
                brz3dvp.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163390(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            BluRingViewer bluview = new BluRingViewer();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string imagethumb = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string requirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, imagethumb);
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study");
                    throw new Exception("Failed to open study");
                }

                //step 02 - 04
                Result = brz3dvp.advanced3dsettings("Log Level", "all");
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    Logger.Instance.ErrorLog("Failed to set log level in advanced 3D Settings");
                    throw new Exception("Failed to set log level in advanced 3D Settings");
                }

                //step 05
                Result = brz3dvp.advanced3dsettings("Display Frame Per Second");
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    Logger.Instance.ErrorLog("Failed to check display frame per second from advanced 3D Settings");
                    throw new Exception("Failed to check display frame per second from advanced 3D Settings");
                }

                //setp 06
                new Actions(Driver).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                bool result1 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Scrolling_Tool, 20, 20, 50);
                Logger.Instance.InfoLog("The result of scroll tool application over Navigation 1 in MPR Layout is : " + result1.ToString());
                PageLoadWait.WaitForFrameLoad(5);
                bool result2 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Window_Level, 20, 20, 50);
                Logger.Instance.InfoLog("The result of window level tool application over Navigation 1 in MPR Layout is : " + result2.ToString());
                PageLoadWait.WaitForFrameLoad(5);
                bool result3 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Rotate_Tool_1_Click_Center, 20, 20, 50);
                Logger.Instance.InfoLog("The result of rotate tool application over Navigation 1 in MPR Layout is : " + result3.ToString());
                PageLoadWait.WaitForFrameLoad(5);
                bool result4 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Pan, 20, 20, 50);
                Logger.Instance.InfoLog("The result of pan tool application over Navigation 1 in MPR Layout is : " + result4.ToString());
                PageLoadWait.WaitForFrameLoad(5);
                IList<IWebElement> fpslist = Driver.FindElements(By.CssSelector(Locators.CssSelector.fps));
                if (result1 && result2 && result3 && result4 && fpslist[0].Text.Equals(requirements))
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 07
                int counter = 0;
                String[] LayoutMode = { BluRingZ3DViewerPage.MPR, BluRingZ3DViewerPage.Three_3d_4, BluRingZ3DViewerPage.Three_3d_6, BluRingZ3DViewerPage.CurvedMPR, BluRingZ3DViewerPage.CalciumScoring };
                foreach (String layout in LayoutMode)
                {
                    brz3dvp.select3dlayout(layout);
                    PageLoadWait.WaitForFrameLoad(5);
                    brz3dvp.select3DTools(Z3DTools.Reset);
                    PageLoadWait.WaitForFrameLoad(5);
                    IWebElement LayoutSelector = Driver.FindElement(By.CssSelector(Locators.CssSelector.StudyViewTitleBar + " " + Locators.CssSelector.layoutvalue));
                    if (LayoutSelector.GetAttribute("innerText").Contains(BluRingZ3DViewerPage.CalciumScoring))
                    {
                        brz3dvp.checkerrormsg("y");
                        Thread.Sleep(3000);
                        PageLoadWait.WaitForFrameLoad(5);
                        brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                        PageLoadWait.WaitForFrameLoad(5);
                        bool calres1 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.CalciumScoring, Z3DTools.Scrolling_Tool, 20, 20, 50);
                        Logger.Instance.InfoLog("The result of scroll tool application over calcium scoring is : " + calres1.ToString());
                        PageLoadWait.WaitForFrameLoad(5);
                        bool calres2 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.CalciumScoring, Z3DTools.Window_Level, 20, 20, 50);
                        Logger.Instance.InfoLog("The result of window level tool application over calcium scoring is : " + calres2.ToString());
                        PageLoadWait.WaitForFrameLoad(5);
                        IList<IWebElement> calfpslist = Driver.FindElements(By.CssSelector(Locators.CssSelector.fps));
                        if (calres1 && calres2 && calfpslist[0].Text.Equals(requirements))
                        {
                            Logger.Instance.InfoLog("Calcium Scoring Viewmode verified");
                            counter++;
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Verification failed in Calcium Scoring Viewmode");
                            break;
                        }
                    }
                    else if (LayoutSelector.GetAttribute("innerText").Contains(BluRingZ3DViewerPage.CurvedMPR))
                    {
                        int itr = 0;
                        bool curveres1 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Scrolling_Tool, 20, 20, 50);
                        Logger.Instance.InfoLog("The result of scroll tool application over navigation 1 in curved mpr layout is : " + curveres1.ToString());
                        PageLoadWait.WaitForFrameLoad(5);
                        bool curveres2 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationtwo, Z3DTools.Window_Level, 20, 20, 50);
                        Logger.Instance.InfoLog("The result of window level tool application over navigation 2 in curved mpr layout is : " + curveres2.ToString());
                        PageLoadWait.WaitForFrameLoad(5);
                        bool curveres3 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationthree, Z3DTools.Rotate_Tool_1_Click_Center, 20, 20, 50);
                        Logger.Instance.InfoLog("The result of rotate tool application over navigation 3 in curved mpr layout is : " + curveres3.ToString());
                        PageLoadWait.WaitForFrameLoad(5);
                        brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage._3DPathNavigation, Z3DTools.Window_Level, 20, 20, 50, movement: "positive");
                        Logger.Instance.InfoLog("window level tool applied in 3D Path Navigation control");
                        PageLoadWait.WaitForFrameLoad(5);
                        brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.MPRPathNavigation, Z3DTools.Window_Level, 20, 20, 50, movement: "positive");
                        Logger.Instance.InfoLog("window level tool applied in MPR Path Navigation control");
                        PageLoadWait.WaitForFrameLoad(5);
                        brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.CurvedMPR, Z3DTools.Window_Level, 20, 20, 50, movement: "positive");
                        Logger.Instance.InfoLog("window level tool applied in CurvedMPR Navigation control");
                        PageLoadWait.WaitForFrameLoad(5);
                        IList<IWebElement> curvefpslist = Driver.FindElements(By.CssSelector(Locators.CssSelector.fps));
                        foreach (IWebElement fps in curvefpslist)
                        {
                            if (fps.Text.Equals(requirements))
                            {
                                itr++;
                            }
                        }
                        if (curveres1 && curveres2 && curveres3 && itr == 6)
                        {
                            Logger.Instance.InfoLog("Curved MPR Viewmode verified");
                            counter++;
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Verification failed in Curved MPR Viewmode");
                            break;
                        }
                    }
                    else if (LayoutSelector.GetAttribute("innerText").Contains(BluRingZ3DViewerPage.Three_3d_6))
                    {
                        int itr = 0;
                        bool res1 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Scrolling_Tool, 20, 20, 50);
                        Logger.Instance.InfoLog("The result of scroll tool application over navigation 1 in 3D 6:1 mpr layout is : " + res1.ToString());
                        PageLoadWait.WaitForFrameLoad(5);
                        bool res2 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationtwo, Z3DTools.Window_Level, 20, 20, 50);
                        Logger.Instance.InfoLog("The result of window level tool application over navigation 2 in 3D 6:1 mpr layout is : " + res2.ToString());
                        PageLoadWait.WaitForFrameLoad(5);
                        bool res3 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationthree, Z3DTools.Rotate_Tool_1_Click_Center, 20, 20, 50);
                        Logger.Instance.InfoLog("The result of rotate tool application over navigation 3 in 3D 6:1 mpr layout is : " + res3.ToString());
                        PageLoadWait.WaitForFrameLoad(5);
                        bool res4 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.ResultPanel, Z3DTools.Pan, 20, 20, 50);
                        Logger.Instance.InfoLog("The result of pan tool application over result panel in 3D 6:1 mpr layout is : " + res4.ToString());
                        PageLoadWait.WaitForFrameLoad(5);
                        bool res5 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D1, Z3DTools.Rotate_Tool_1_Click_Center, 20, 20, 50);
                        Logger.Instance.InfoLog("The result of rotate tool application over navigation 3D1 in 3D 6:1 mpr layout is : " + res5.ToString());
                        PageLoadWait.WaitForFrameLoad(5);
                        bool res6 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D2, Z3DTools.Rotate_Tool_1_Click_Center, 20, 20, 50);
                        Logger.Instance.InfoLog("The result of rotate tool application over navigation 3D2 in 3D 6:1 mpr layout is : " + res6.ToString());
                        PageLoadWait.WaitForFrameLoad(5);
                        IList<IWebElement> threedfpslist = Driver.FindElements(By.CssSelector(Locators.CssSelector.fps));
                        foreach (IWebElement fps in threedfpslist)
                        {
                            if (fps.Text.Equals(requirements))
                            {
                                Logger.Instance.InfoLog("MPR 6:1 Viewmode verified");
                                itr++;
                            }
                        }
                        if (res1 && res2 && res4 && res5 && res6 && res3 && itr == 6)
                        {
                            int inc = 0;
                            brz3dvp.ChangeViewMode();
                            PageLoadWait.WaitForFrameLoad(5);
                            brz3dvp.select3DTools(Z3DTools.Reset);
                            PageLoadWait.WaitForFrameLoad(5);
                            bool res8 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D1, Z3DTools.Rotate_Tool_1_Click_Center, 20, 20, 50);
                            Logger.Instance.InfoLog("The result of rotate tool application over navigation 3D1 in 3D 6:1 layout is : " + res8.ToString());
                            PageLoadWait.WaitForFrameLoad(5);
                            bool res9 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D2, Z3DTools.Rotate_Tool_1_Click_Center, 20, 20, 50);
                            Logger.Instance.InfoLog("The result of rotate tool application over navigation 3D2 in 3D 6:1 layout is : " + res9.ToString());
                            PageLoadWait.WaitForFrameLoad(5);
                            IList<IWebElement> threedfpslist1 = Driver.FindElements(By.CssSelector(Locators.CssSelector.fps));
                            foreach (IWebElement fps in threedfpslist1)
                            {
                                if (fps.Text.Equals(requirements))
                                {
                                    inc++;
                                }
                            }
                            if (res8 && res9 && inc == 6)
                            {
                                Logger.Instance.InfoLog("3D 6:1 Viewmode verified");
                                counter++;
                            }
                            else
                            {
                                Logger.Instance.ErrorLog("Verification failed in 3D 6:1 Viewmode");
                                break;
                            }
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Verification failed in 6:1 MPR Viewmode");
                            break;
                        }
                    }
                    else if (LayoutSelector.GetAttribute("innerText").Contains(BluRingZ3DViewerPage.MPR))
                    {
                        int itr = 0;
                        bool res1 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationone, Z3DTools.Scrolling_Tool, 20, 20, 50);
                        Logger.Instance.InfoLog("The result of scroll tool application over navigation 1 in mpr layout is : " + res1.ToString());
                        PageLoadWait.WaitForFrameLoad(5);
                        bool res2 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationtwo, Z3DTools.Window_Level, 20, 20, 50);
                        Logger.Instance.InfoLog("The result of window level tool application over navigation 2 in mpr layout is : " + res2.ToString());
                        PageLoadWait.WaitForFrameLoad(5);
                        bool res3 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigationthree, Z3DTools.Rotate_Tool_1_Click_Center, 20, 20, 50);
                        Logger.Instance.InfoLog("The result of rotate tool application over navigation 3 in mpr layout is : " + res3.ToString());
                        PageLoadWait.WaitForFrameLoad(5);
                        bool res4 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.ResultPanel, Z3DTools.Pan, 20, 20, 50);
                        Logger.Instance.InfoLog("The result of pan tool application over result control in mpr layout is : " + res4.ToString());
                        PageLoadWait.WaitForFrameLoad(5);
                        IList<IWebElement> mprfpslist = Driver.FindElements(By.CssSelector(Locators.CssSelector.fps));
                        foreach (IWebElement fps in mprfpslist)
                        {
                            if (fps.Text.Equals(requirements))
                            {
                                itr++;
                            }
                        }
                        if (itr == 4 && res1 && res2 && res3 && res4)
                        {
                            Logger.Instance.InfoLog("MPR Viewmode verified");
                            counter++;
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Verification failed in MPR Viewmode");
                            break;
                        }
                    }
                    else
                    {
                        int itr = 0;
                        bool res1 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D1, Z3DTools.Scrolling_Tool, 20, 20, 50);
                        Logger.Instance.InfoLog("The result of scroll tool application over navigation 3D1 in 3D 4:1 layout is : " + res1.ToString());
                        PageLoadWait.WaitForFrameLoad(5);
                        bool res2 = brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D1, Z3DTools.Rotate_Tool_1_Click_Center, 20, 20, 50);
                        Logger.Instance.InfoLog("The result of rotate tool application over navigation 3D1 in 3D 4:1 layout is : " + res2.ToString());
                        PageLoadWait.WaitForFrameLoad(5);
                        brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D1, Z3DTools.Window_Level, 20, 20, 50);
                        Logger.Instance.InfoLog("Window level tool is applied over 3D1 Control in 3D 4:1 Layout");
                        PageLoadWait.WaitForFrameLoad(5);
                        brz3dvp.ApplyToolsonViewPort(BluRingZ3DViewerPage.Navigation3D1, Z3DTools.Interactive_Zoom, 20, 20, 50);
                        Logger.Instance.InfoLog("Zoom tool is applied over 3D1 Control in 3D 4:1 Layout");
                        PageLoadWait.WaitForFrameLoad(5);
                        IList<IWebElement> threed4fpslist = Driver.FindElements(By.CssSelector(Locators.CssSelector.fps));
                        foreach (IWebElement fps in threed4fpslist)
                        {
                            if (fps.Text.Equals(requirements))
                            {
                                itr++;
                            }
                        }
                        if (res1 && res2 && itr == 4)
                        {
                            Logger.Instance.InfoLog("3D 4:1 Viewmode verified");
                            counter++;
                        }
                        else
                        {
                            Logger.Instance.ErrorLog("Verification failed in 3D 4:1 Viewmode");
                            break;
                        }
                    }
                }
                if (counter == 5)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
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
                brz3dvp.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163393(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            BluRingViewer bluview = new BluRingViewer();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string imagethumb = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string requirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] values = requirements.Split('|');

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //STEP 01 :: Log in to iCA and navigate to studies tab.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].StepPass();
                Logger.Instance.InfoLog("-->Completed Step : 01<--");

                //STEP 02 :: Search and load a 3D supported Lossy compressed series in universal viewer.
                //STEP 03 :: From the Universal viewer , Select a 3D supported series.
                //STEP 04 :: Select the MPR view option from the smart view drop down.
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, imagethumb);
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Steps : 02,03,04<--");

                //STEP 05 :: Click on the 3D Settings option from the user settings under global toolbar and move the MPR final quality and 3D final quality sliders to 100%. Click on Save button. 
                IWebElement SmartViewValue = Driver.FindElement(By.CssSelector(Locators.CssSelector.SmartViewSelectedValue));
                bool MPRFinalQuality = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRFinalQuality, 100);
                bool FinalQuality3D = brz3dvp.change3dsettings(BluRingZ3DViewerPage.FinalQuality3D, 100);
                if (!MPRFinalQuality || !FinalQuality3D)
                    throw new Exception("Cannot be set 3D Settings");
                //IWebElement Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                //IWebElement Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                //IWebElement Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                //IWebElement ResultPanel = brz3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                //String step5 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation1);
                //String step5_1 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation2);
                //String step5_2 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation3);
                //String step5_3 = brz3dvp.GetCenterBottomAnnotationLocationValue(ResultPanel);
                //  if (step5.Equals("Lossy Compressed") && step5_1.Equals("Lossy Compressed") && step5_2.Equals("Lossy Compressed") && step5_3.Equals("Lossy Compressed"))
                List<string> result5 = brz3dvp.GetAttributes_Result(Locators.CssSelector.CenterBottomAnnotationValue, null, null, 0);
                if (result5[0].Equals("Lossy Compressed") && result5[1].Equals("Lossy Compressed") && result5[2].Equals("Lossy Compressed") && result5[2].Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 05<--");

                //STEP 06 :: Select the 3D view option from the smart view drop down.
                brz3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                //IWebElement Navigation3D1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                //String step6 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation1);
                //String step6_1 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation2);
                //String step6_2 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation3);
                //String step6_3 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation3D1);
                //if (step6.Equals("Lossy Compressed") && step6_1.Equals("Lossy Compressed") && step6_2.Equals("Lossy Compressed") && step6_3.Equals("Lossy Compressed"))
                    List<string> result6 = brz3dvp.GetAttributes_Result(Locators.CssSelector.CenterBottomAnnotationValue, null, null, 0);
                if (result6[0].Equals("Lossy Compressed") && result6[1].Equals("Lossy Compressed") && result6[2].Equals("Lossy Compressed") && result6[3].Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 06<--");

                //STEP 07 :: Select the Sixup viewing mode from the smart view drop down and verify the Controls.
                brz3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                //IWebElement Navigation3D2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                //String step7 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation1);
                //String step7_1 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation2);
                //String step7_2 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation3);
                //String step7_3 = brz3dvp.GetCenterBottomAnnotationLocationValue(ResultPanel);
                //String step7_4 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation3D1);
                //String step7_5 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation3D2);
                //   if (step7.Equals("Lossy Compressed") && step7_1.Equals("Lossy Compressed") && step7_2.Equals("Lossy Compressed") && step7_3.Equals("Lossy Compressed") && step7_4.Equals("Lossy Compressed") && step7_5.Equals("Lossy Compressed"))
                List<string> result7 = brz3dvp.GetAttributes_Result(Locators.CssSelector.CenterBottomAnnotationValue, null, null, 0);
                if (result7[0].Equals("Lossy Compressed") && result7[1].Equals("Lossy Compressed") && result7[2].Equals("Lossy Compressed") && result7[3].Equals("Lossy Compressed") && result7[4].Equals("Lossy Compressed") && result7[5].Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 07<--");

                //STEP 08 :: Select the Curved MPR viewing mode from the smart view drop down and verify the Controls.
                brz3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                //IWebElement MPRPathNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                //IWebElement _3DPathNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                //IWebElement CurvedMPR = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                //String step8 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation1);
                //String step8_1 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation2);
                //String step8_2 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation3);
                //String step8_3 = brz3dvp.GetCenterBottomAnnotationLocationValue(MPRPathNavigation);
                //String step8_4 = brz3dvp.GetCenterBottomAnnotationLocationValue(_3DPathNavigation);
                //String step8_5 = brz3dvp.GetCenterBottomAnnotationLocationValue(CurvedMPR);
                //if (step8.Equals("Lossy Compressed") && step8_1.Equals("Lossy Compressed") && step8_2.Equals("Lossy Compressed") && step8_3.Equals("Lossy Compressed") && step8_4.Equals("Lossy Compressed") && step8_5.Equals("Lossy Compressed"))
                List<string> result8 = brz3dvp.GetAttributes_Result(Locators.CssSelector.CenterBottomAnnotationValue, null, null, 0);
                if (result8[0].Equals("Lossy Compressed") && result8[1].Equals("Lossy Compressed") && result8[2].Equals("Lossy Compressed") && result8[3].Equals("Lossy Compressed") && result8[4].Equals("Lossy Compressed") && result8[5].Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 08<--");

                //STEP 09 :: Select the Calcium scoring mode from the smart view drop down and verify the Control.
                brz3dvp.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                bool HandleWarningMsg = brz3dvp.checkerrormsg("y");
                if (!HandleWarningMsg)
                    Logger.Instance.ErrorLog("Calcium Scoring Warning message not found (or) cannot be handled.");

                IWebElement CalciumScoring = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                String step9 = brz3dvp.GetCenterBottomAnnotationLocationValue(CalciumScoring);
                if (step9.Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 09<--");

                //STEP 10 :: Click on the 3D Settings option from the user settings under global toolbar and move the MPR final quality and 3D final quality sliders lesser 100%. ( Range   1% to 99%). Click on Save button.
                MPRFinalQuality = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRFinalQuality, 90);
                FinalQuality3D = brz3dvp.change3dsettings(BluRingZ3DViewerPage.FinalQuality3D, 90);
                if (!MPRFinalQuality || !FinalQuality3D)
                    throw new Exception("Cannot be set 3D Settings");
                brz3dvp.select3dlayout(BluRingZ3DViewerPage.MPR);
                //Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                //Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                //Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                //ResultPanel = brz3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                //String step10 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation1);
                //String step10_1 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation2);
                //String step10_2 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation3);
                //String step10_3 = brz3dvp.GetCenterBottomAnnotationLocationValue(ResultPanel);
                //     if (step10.Equals("Lossy Compressed") && step10_1.Equals("Lossy Compressed") && step10_2.Equals("Lossy Compressed") && step10_3.Equals("Lossy Compressed"))
                List<string> result10 = brz3dvp.GetAttributes_Result(Locators.CssSelector.CenterBottomAnnotationValue, null, null, 0);
                if (result10[0].Equals("Lossy Compressed") && result10[1].Equals("Lossy Compressed") && result10[2].Equals("Lossy Compressed") && result10[3].Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 10<--");

                //STEP 11 :: Repeat steps 6-9.
                string[] layoutname = new string[] { BluRingZ3DViewerPage.Three_3d_4, BluRingZ3DViewerPage.Three_3d_6, BluRingZ3DViewerPage.CurvedMPR , BluRingZ3DViewerPage.CalciumScoring };
                
                int z = 0;
                for(int i=0;i<layoutname.Length;i++)
                {
                    bool layout = brz3dvp.select3dlayout(layoutname[i]);
                    if(layoutname[i]== BluRingZ3DViewerPage.CalciumScoring) brz3dvp.checkerrormsg("y");
                    if (layout)
                    {
                        List<string> result11 = brz3dvp.GetAttributes_Result(Locators.CssSelector.CenterBottomAnnotationValue, null, null, 0);
                        int m = 0;
                        if(result11.Count>0)
                        {
                            for(int j=0; j< result11.Count;j++)
                            {
                                if (result11[j] == "Lossy Compressed")
                                    m++;
                                else
                                {
                                    Logger.Instance.ErrorLog("Lossy complressed not displayed in  " + layoutname[i] +" " + result11[j]);
                                }
                            }
                        }
                        if (result11.Count == m) z++;

                    }
                }
                if(layoutname.Length==z)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                /*
                Dictionary<string, List<String>> map = new Dictionary<string, List<String>>();
                map.Add(BluRingZ3DViewerPage.Three_3d_4, new List<string>() { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.Navigation3D1 });
                map.Add(BluRingZ3DViewerPage.Three_3d_6, new List<string>() { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Navigation3D2 });
                map.Add(BluRingZ3DViewerPage.CurvedMPR, new List<string>() { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.MPRPathNavigation, BluRingZ3DViewerPage._3DPathNavigation, BluRingZ3DViewerPage.CurvedMPR });
                map.Add(BluRingZ3DViewerPage.CalciumScoring, new List<string>() { BluRingZ3DViewerPage.CalciumScoring });

                bool ResultValue = true;
                int Count = 0;
               foreach (KeyValuePair<string, List<string>> kvp in map)
                {
                   
                    bool layout = brz3dvp.select3dlayout(kvp.Key);
                    PageLoadWait.WaitForFrameLoad(5);
                    if (SmartViewValue.GetAttribute("innerText").Equals(kvp.Key))
                    {
                        Logger.Instance.InfoLog(kvp.Key + " Viewer displayed successfully.");
                        if (kvp.Key.Equals(BluRingZ3DViewerPage.CalciumScoring))
                        {
                            HandleWarningMsg = brz3dvp.checkerrormsg("y");
                            if (!HandleWarningMsg)
                                Logger.Instance.ErrorLog("Calcium Scoring Warning message not found (or) cannot be handled.");
                        }
                        foreach (string value in kvp.Value)
                        {
                            try
                            {
                                if (!brz3dvp.GetCenterBottomAnnotationLocationValue(brz3dvp.controlelement(value)).Equals("Lossy Compressed"))
                                {
                                    ResultValue = false;
                                    Logger.Instance.ErrorLog(kvp.Key + " Viewer not contain 'Lossy Compression' annotation in " + value + " control.");
                                    break;
                                }
                            }
                            catch (Exception e)
                            {
                                ResultValue = false;
                                Logger.Instance.ErrorLog(kvp.Key + " Viewer not contain 'Lossy Compression' annotation in " + value + " control : " + e.Message);
                                break;
                            }
                            Logger.Instance.InfoLog(kvp.Key + " Viewer contain 'Lossy Compression' annotation in " + value + " control");
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
                Logger.Instance.InfoLog("-->Completed Step : 11<--");
                */


                //STEP 12 :: Select the 2D option from the smart view drop down.
                IWebElement webElement = GetElement("cssselector", Locators.CssSelector.ViewerButton3D);
                ClickElement(webElement);
                Thread.Sleep(5000);
                IList<IWebElement> weli = brz3dvp.layoutlist();
                foreach (IWebElement we in weli)
                {
                    if (we.Text.Equals(BluRingZ3DViewerPage.Two_2D))
                    {
                        ClickElement(we);
                        break;
                    }
                }
                PageLoadWait.WaitForFrameLoad(10);
                if (SmartViewValue.GetAttribute("innerText").Equals(BluRingZ3DViewerPage.Two_2D))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 12<--");

                //STEP 13 :: Click on the close button from the Global toolbar.
                brz3dvp.CloseViewer();
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement SelectedTab = Driver.FindElement(By.CssSelector(Locators.CssSelector.SelectedTab));
                if (SelectedTab.GetAttribute("title").Equals("Studies"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 13<--");

                //STEP 14 :: Search for a study that has a No lossy compressed series
                //STEP 15 :: Load the study in universal viewer.
                //STEP 16 :: Select a 3D supported series and Select the MPR view option from the smart view drop down.
                Result = brz3dvp.searchandopenstudyin3D(values[0], values[1]);
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Steps : 14,15,16<--");

                //STEP 17 :: Click on the 3D Settings option from the user settings under global toolbar and move the MPR final quality and 3D final quality sliders to 100%. Click on Save button. 
                SmartViewValue = Driver.FindElement(By.CssSelector(Locators.CssSelector.SmartViewSelectedValue));
                MPRFinalQuality = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRFinalQuality, 100);
                FinalQuality3D = brz3dvp.change3dsettings(BluRingZ3DViewerPage.FinalQuality3D, 100);
                if (!MPRFinalQuality || !FinalQuality3D)
                    throw new Exception("Cannot be set 3D Settings");
                brz3dvp.select3dlayout(BluRingZ3DViewerPage.MPR);
                //Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                //Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                //Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                //ResultPanel = brz3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                //String step17 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation1);
                //String step17_1 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation2);
                //String step17_2 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation3);
                //String step17_3 = brz3dvp.GetCenterBottomAnnotationLocationValue(ResultPanel);
                // if (!step17.Equals("Lossy Compressed") && !step17_1.Equals("Lossy Compressed") && !step17_2.Equals("Lossy Compressed") && !step17_3.Equals("Lossy Compressed"))
                List<string> result17 = brz3dvp.GetAttributes_Result(Locators.CssSelector.CenterBottomAnnotationValue, null, null, 0);
                if (!result17[0].Equals("Lossy Compressed") && !result17[1].Equals("Lossy Compressed") && !result17[2].Equals("Lossy Compressed") && !result17[3].Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 17<--");

                //STEP 18 :: Select the 3D view option from the smart view drop down.
                brz3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                //Navigation3D1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                //String step18 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation1);
                //String step18_1 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation2);
                //String step18_2 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation3);
                //String step18_3 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation3D1);
                List<string> result18 = brz3dvp.GetAttributes_Result(Locators.CssSelector.CenterBottomAnnotationValue, null, null, 0);
                if (!result18[0].Equals("Lossy Compressed") && !result18[1].Equals("Lossy Compressed") && !result18[2].Equals("Lossy Compressed") && !result18[3].Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 18<--");

                //STEP 19 :: Select the Sixup viewing mode from the smart view drop down and verify the Controls.
                brz3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                //Navigation3D2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2);
                //String step19 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation1);
                //String step19_1 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation2);
                //String step19_2 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation3);
                //String step19_3 = brz3dvp.GetCenterBottomAnnotationLocationValue(ResultPanel);
                //String step19_4 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation3D1);
                //String step19_5 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation3D2);
                List<string> result19 = brz3dvp.GetAttributes_Result(Locators.CssSelector.CenterBottomAnnotationValue, null, null, 0);
                if (!result19[0].Equals("Lossy Compressed") && !result19[1].Equals("Lossy Compressed") && !result19[2].Equals("Lossy Compressed") && !result19[3].Equals("Lossy Compressed") && !result19[4].Equals("Lossy Compressed") && !result19[5].Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 19<--");

                //STEP 20 :: Select the Curved MPR viewing mode from the smart view drop down and verify the Controls.
                brz3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                //MPRPathNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation);
                //_3DPathNavigation = brz3dvp.controlelement(BluRingZ3DViewerPage._3DPathNavigation);
                //CurvedMPR = brz3dvp.controlelement(BluRingZ3DViewerPage.CurvedMPR);
                //String step20 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation1);
                //String step20_1 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation2);
                //String step20_2 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation3);
                //String step20_3 = brz3dvp.GetCenterBottomAnnotationLocationValue(MPRPathNavigation);
                //String step20_4 = brz3dvp.GetCenterBottomAnnotationLocationValue(_3DPathNavigation);
                //String step20_5 = brz3dvp.GetCenterBottomAnnotationLocationValue(CurvedMPR);
                List<string> result20 = brz3dvp.GetAttributes_Result(Locators.CssSelector.CenterBottomAnnotationValue, null, null, 0);
                if (!result20[0].Equals("Lossy Compressed") && !result20[1].Equals("Lossy Compressed") && !result20[2].Equals("Lossy Compressed") && !result20[3].Equals("Lossy Compressed") && !result20[4].Equals("Lossy Compressed") && !result20[5].Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 20<--");

                //STEP 21 :: Select the Calcium scoring mode from the smart view drop down and verify the Control.
                brz3dvp.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                HandleWarningMsg = brz3dvp.checkerrormsg("y");
                if (!HandleWarningMsg)
                    Logger.Instance.ErrorLog("Calcium Scoring Warning message not found (or) cannot be handled.");

                CalciumScoring = brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring);
                String step21 = brz3dvp.GetCenterBottomAnnotationLocationValue(CalciumScoring);
                if (!step21.Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 21<--");

                //STEP 22 :: Click on the 3D Settings option from the user settings under global toolbar and move the MPR final quality and 3D final quality sliders lesser 100%. ( Range   1% to 99%). Click on Save button.
                MPRFinalQuality = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRFinalQuality, 90);
                FinalQuality3D = brz3dvp.change3dsettings(BluRingZ3DViewerPage.FinalQuality3D, 90);
                if (!MPRFinalQuality || !FinalQuality3D)
                    throw new Exception("Cannot be set 3D Settings");
                brz3dvp.select3dlayout(BluRingZ3DViewerPage.MPR);
                //Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                //Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                //Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                //ResultPanel = brz3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                //String step22 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation1);
                //String step22_1 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation2);
                //String step22_2 = brz3dvp.GetCenterBottomAnnotationLocationValue(Navigation3);
                //String step22_3 = brz3dvp.GetCenterBottomAnnotationLocationValue(ResultPanel);
                List<string> result22 = brz3dvp.GetAttributes_Result(Locators.CssSelector.CenterBottomAnnotationValue, null, null, 0);
                if (result22[0].Equals("Lossy Compressed") && result22[1].Equals("Lossy Compressed") && result22[2].Equals("Lossy Compressed") && result22[3].Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 22<--");

                //STEP 23 :: Repeat steps 6-9.
                int k = 0;
                for (int i = 0; i < layoutname.Length; i++)
                {
                    bool layout = brz3dvp.select3dlayout(layoutname[i]);
                    if (layoutname[i] == BluRingZ3DViewerPage.CalciumScoring) brz3dvp.checkerrormsg("y");
                    if (layout)
                    {
                        List<string> result23 = brz3dvp.GetAttributes_Result(Locators.CssSelector.CenterBottomAnnotationValue, null, null, 0);
                        int m = 0;
                        if (result23.Count > 0)
                        {
                            for (int j = 0; j < result23.Count; j++)
                            {
                                if (result23[j] == "Lossy Compressed") m++;
                                else
                                {
                                    Logger.Instance.ErrorLog("Lossy complressed not displayed in  " + layoutname[i] + " " + result23[j] + " displaying wrong values ");
                                }
                            }
                        }
                        if (result23.Count == m)
                            k++;

                    }
                }
                if (layoutname.Length == k)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                /*     Dictionary<string, List<String>> map = new Dictionary<string, List<String>>();
                     bool      ResultValue = true;
               int       Count = 0;
                     foreach (KeyValuePair<string, List<string>> kvp in map)
                     {
                         brz3dvp.select3dlayout(kvp.Key);
                         PageLoadWait.WaitForFrameLoad(5);
                         if (SmartViewValue.GetAttribute("innerText").Equals(kvp.Key))
                         {
                             Logger.Instance.InfoLog(kvp.Key + " Viewer displayed successfully.");
                             if (kvp.Key.Equals(BluRingZ3DViewerPage.CalciumScoring))
                             {
                                 HandleWarningMsg = brz3dvp.checkerrormsg("y");
                                 if (!HandleWarningMsg)
                                     Logger.Instance.ErrorLog("Calcium Scoring Warning message not found (or) cannot be handled.");
                             }
                             foreach (string value in kvp.Value)
                             {
                                 try
                                 {
                                     if (!brz3dvp.GetCenterBottomAnnotationLocationValue(brz3dvp.controlelement(value)).Equals("Lossy Compressed"))
                                     {
                                         ResultValue = false;
                                         Logger.Instance.ErrorLog(kvp.Key + " Viewer not contain 'Lossy Compression' annotation in " + value + " control.");
                                         break;
                                     }
                                 }
                                 catch (Exception e)
                                 {
                                     ResultValue = false;
                                     Logger.Instance.ErrorLog(kvp.Key + " Viewer not contain 'Lossy Compression' annotation in " + value + " control : " + e.Message);
                                     break;
                                 }
                                 Logger.Instance.InfoLog(kvp.Key + " Viewer contain 'Lossy Compression' annotation in " + value + " control");
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
                     }*/
                Logger.Instance.InfoLog("-->Completed Step : 23<--");

                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
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
                brz3dvp.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163392(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            BluRingViewer bluview = new BluRingViewer();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string imagethumb = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string requirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] values = requirements.Split('|');

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                // Precondition
                login.ChangeAttributeValue(@"C:\drs\wwwroot\DRPACS\Z3D\custom.config", "appSettings", "LogLevel", "2", true);
                //STEP 01 :: From the Universal viewer , Select a 3D supported series and Select the MPR view option from the smart view drop down.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool step1 = brz3dvp.searchandopenstudyin3D("1TLJY1YL", imagethumb, BluRingZ3DViewerPage.MPR, field: "acc", ChangeSettings: "No");
                if (step1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *1 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 2 :: Hold Shift key and Click on the 3D settings options from the User settings under Global tool bar .
                bluview.ClickOnUSerSettings();
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Locators.CssSelector.activetool + " " + Locators.CssSelector.dropdownactive)));
                IList<IWebElement> listelements = Driver.FindElements(By.CssSelector(Locators.CssSelector.activetool + " li"));
                Logger.Instance.InfoLog("Shift button pressed");
                foreach (IWebElement element in listelements)
                {
                    if (element.Text.ToLower().Contains("3d settings"))
                    {
                        new Actions(Driver).MoveToElement(element).Build().Perform();
                        Thread.Sleep(3000);
                        new Actions(Driver).KeyDown(Keys.Shift).Click(element).KeyUp(Keys.Shift).Perform();
                        break;
                    }
                }
                IList<IWebElement> contentlabels = Driver.FindElements(By.CssSelector(Locators.CssSelector.labelcontent));
                int inc = 0;
                foreach (IWebElement element in contentlabels)
                {
                    if (element.Text.ToLower().Trim().Contains("advanced") && element.Enabled)
                    {
                        inc++;
                    }
                }
                if (inc == 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *2 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                    throw new Exception("Advanced tab not available");
                }
                //Step 3 :: Navigate to the Advanced tab and verify the log level options.
                contentlabels = Driver.FindElements(By.CssSelector(Locators.CssSelector.labelcontent));
                foreach (IWebElement element in contentlabels)
                {
                    if (element.Text.ToLower().Trim().Contains("advanced") && element.Enabled)
                    {
                        element.Click();
                    }
                }
                Thread.Sleep(5000);
                IList<IWebElement> list2 = Driver.FindElements(By.CssSelector(Locators.CssSelector.SettingsValues));
                int flag = 0;
                list2[1].FindElement(By.CssSelector(Locators.CssSelector.matlist)).Click();
                String Csselement = Locators.CssSelector.Warning + " " + Locators.CssSelector.DropDown3DBox;
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(Csselement)));
                IList<IWebElement> dropdownelements = Driver.FindElements(By.CssSelector(Locators.CssSelector.Layoutlist));
                foreach (IWebElement ddval in dropdownelements)
                {
                    foreach (String sval in values)
                    {
                        if (sval.Equals(ddval.Text.Trim()))
                        {
                            flag++;
                        }
                    }
                }

                if (flag == 8)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *3 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 4 :: Remote to the Z3D server.
                new Actions(Driver).MoveToElement(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone)).Click().Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                IList<IWebElement> buttons = Driver.FindElements(By.CssSelector(Locators.CssSelector.ConfirmButton));
                foreach (IWebElement button in buttons)
                {
                    if (button.Text.ToLower().Contains("cancel"))
                    {
                        ClickElement(button);
                        //button.Click();
                        break;
                    }
                }
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(Locators.CssSelector.OverLayPane)));
                brz3dvp.CloseViewer();
                bool step4 = brz3dvp.searchandopenstudyin3D("1TLJY1YL", imagethumb, BluRingZ3DViewerPage.MPR, field: "acc", ChangeSettings: "No");
                if (step4)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *4 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step 5 :: Open and verify the Z3DIntegratedICA CleintViewer.log file from the below location:C:\DRS\LOGS\Users\Administrator
                brz3dvp.select3DTools(Z3DTools.Window_Level);
                DateTime today = DateTime.Today;
                string ServerDateFormat = (Convert.ToDateTime(today)).ToString("MM/dd/yy").ToString();
                string AdminPath = @"C:\drs\LOGS\Users\Administrator\Z3d Integrated ICA ClientViewer.log";
                string[] readText = File.ReadAllLines(AdminPath);
                int step5 = 0;
                for (int i = 0; i < readText.Length; i++)
                {
                    if (readText[i].Contains(ServerDateFormat))
                    {
                        if (readText[i].Contains("userAgent:") || readText[i].Contains(" Modality volumes:") || readText[i].Contains("OPENING LOG:") || readText[i].Contains("user clicked 'wl'") || readText[i].Contains("CLOSING LOG:") || readText[i].Contains("Duration"))
                        {
                            step5++;
                        }

                    }
                }
                if (step5 >= 6)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *5 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }

                //Step 6 :: From the Advanced settings, set the Log level to TRACE / DEBUG/ INFO and verify the log "Z3DIntegratedICACleintViewer.log"
                bool Trace = brz3dvp.advanced3dsettings("Log Level", "trace");
                brz3dvp.select3DTools(Z3DTools.Window_Level);
                brz3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                readText = File.ReadAllLines(AdminPath);
                int step6 = 0;
                for (int i = 0; i < readText.Length; i++)
                {
                    if (readText[i].Contains(ServerDateFormat))
                    {
                        if (readText[i].Contains("user clicked 'wl'") || readText[i].Contains("user clicked 'zoom'"))
                        {
                            step6++;
                        }

                    }
                }
                if (step6 >= 3 && Trace)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *6 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step7 :: From the Advanced settings, set the Log level to ERROR/ FATAL and verify the log "Z3DIntegratedICA CleintViewer.log"
                bool Error = brz3dvp.advanced3dsettings("Log Level", "error");
                IWebElement navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                bool drapdrpthumbnail = brz3dvp.DragandDropThumbnail("S4", "MR", "23", navigation1);
                Logger.Instance.InfoLog("Thumbnail drap and drop status : " + drapdrpthumbnail);
                PageLoadWait.WaitForFrameLoad(10);
                //Verification :: "Slices are not sufficiently parallel" message shows up.
                brz3dvp.checkerrormsg("y");
                DirectoryInfo directoryInfo = new DirectoryInfo(@"C:\windows\temp\");
                var webaccessDeveloperLog = directoryInfo.GetFiles("WebAccessDeveloper*.log", SearchOption.AllDirectories).OrderBy(t => t.LastWriteTime).ToList();
                String pathname = webaccessDeveloperLog[webaccessDeveloperLog.Count - 1].FullName;
                FileStream logFileStream = new FileStream(pathname, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader logFileReader = new StreamReader(logFileStream);
                int Step7 = 0;
                while (!logFileReader.EndOfStream)
                {
                    string line = logFileReader.ReadToEnd();
                    if (line.Contains("Slices are not sufficiently parallel"))
                    {
                        Step7++;
                    }
                }
                logFileReader.Close();
                logFileStream.Close();
                if (Step7 >= 1 && Error)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *7 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step8 :: From the Advanced settings, set the Log level to OFF and verify the log "Z3DIntegratedICA CleintViewer.log".
                string[] ReadAllLines_B = File.ReadAllLines(AdminPath);
                bool Off = brz3dvp.advanced3dsettings("Log Level", "off");
                brz3dvp.select3DTools(Z3DTools.Pan);
                string[] ReadAllLines_A = File.ReadAllLines(AdminPath);
                if (ReadAllLines_B.Length.Equals(ReadAllLines_A.Length) && Off)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *8 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                brz3dvp.CloseViewer();
                login.Logout();
                Driver.Quit();
                //Step9 :: From iCA server, Stop the 3D server (merge app gate manager)
                ServiceController serviceController = new ServiceController("IBM AppGate Manager");
                try
                {
                    if ((serviceController.Status.Equals(ServiceControllerStatus.Running)) || (serviceController.Status.Equals(ServiceControllerStatus.StartPending)))
                    {
                        serviceController.Stop();
                    }
                    serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *9 Passed--" + result.steps[ExecutedSteps].description);
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Error while restarting Services : " + e);
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                }
                //Step10 :: From the Universal viewer , Load a study in 3D viewer.
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool step11 = brz3dvp.searchandopenstudyin3D("1TLJY1YL", imagethumb, BluRingZ3DViewerPage.MPR, field: "acc",ChangeSettings: "No");
                if (!step11)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *10 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                //Step11 :: From iCA server, Open and verify the WebAccessDeveloper file from the below location:C:\windows\temp
                directoryInfo = new DirectoryInfo(@"C:\windows\temp\");
                webaccessDeveloperLog = directoryInfo.GetFiles("WebAccessDeveloper*.log", SearchOption.AllDirectories).OrderBy(t => t.LastWriteTime).ToList();
                pathname = webaccessDeveloperLog[webaccessDeveloperLog.Count - 1].FullName;
                logFileStream = new FileStream(pathname, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                logFileReader = new StreamReader(logFileStream);
                int Step12 = 0;
                while (!logFileReader.EndOfStream)
                {
                    string line = logFileReader.ReadToEnd();
                    if (line.Contains("An error occured while establishing a session with the 3D server"))
                    {
                        Step12++;
                    }
                }
                logFileReader.Close();
                logFileStream.Close();
                if (Step12 >= 1)
                {
                    result.steps[++ExecutedSteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step *11 Passed--" + result.steps[ExecutedSteps].description);
                }
                else
                {
                    result.steps[++ExecutedSteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test case Failed--" + result.steps[ExecutedSteps].description);
                    result.steps[ExecutedSteps].SetLogs();
                }
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
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
                ServiceController serviceController = new ServiceController("IBM AppGate Manager");
                try
                {
                    if ((serviceController.Status.Equals(ServiceControllerStatus.Stopped)) || (serviceController.Status.Equals(ServiceControllerStatus.StopPending)))
                    {
                        Thread.Sleep(2000);
                        serviceController.Start();
                    }
                    serviceController.WaitForStatus(ServiceControllerStatus.Running);
                }
                catch (Exception ex)
                {
                    Logger.Instance.ErrorLog("Test_163392 Error in starting stoped IBM AppGate Manager : " + ex.ToString());
                }
                brz3dvp.CloseViewer();
                login.Logout();
            }
        }

        public TestCaseResult Test_163394(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            BluRingViewer bluview = new BluRingViewer();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string imagethumb = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string requirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String[] values = requirements.Split('|');

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;

            try
            {
                //STEP 01 :: Login in iCA as Administrator
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                result.steps[++ExecutedSteps].StepPass();
                Logger.Instance.InfoLog("-->Completed Step : 01<--");

                //STEP 02 :: Search for a study that has a Lossy compressed series.
                //STEP 03 :: Load the study in universal viewer.
                //STEP 04 :: From the Universal viewer , Select a 3D supported series and Select the MPR view option from the smart view drop down.
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, imagethumb, ChangeSettings: "No");
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Steps : 02,03,04<--");

                //STEP 05 :: Click on the 3D Settings button from the user settings under the Global toolbar options and move the MPR and 3D interactive quality sliders to 100%. Click on Save button.
                DirectoryInfo di = new DirectoryInfo(@"C:\drs\TEMP\Img3D"); // To Check png & jpg images availability
                DateTime StartTime = DateTime.Now;
                IWebElement SmartViewValue = Driver.FindElement(By.CssSelector(Locators.CssSelector.SmartViewSelectedValue));
                bool MPRInteractiveQuality = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 100);
                bool InteractiveQuality3D = brz3dvp.change3dsettings(BluRingZ3DViewerPage.InteractiveQuality3D, 100);
                if (!MPRInteractiveQuality || !InteractiveQuality3D)
                    throw new Exception("Cannot be set 3D Settings");
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                Logger.Instance.InfoLog("-->Completed Step : 05<--");

                //STEP 06 :: Select the scroll tool from the 3D tool box.
                bool Scroll = brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                PageLoadWait.WaitForFrameLoad(10);
                if (Scroll)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                Logger.Instance.InfoLog("-->Completed Step : 06<--");

                //STEP 07 :: Scroll through the image in MPR controls.
                IWebElement Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                IWebElement Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                IWebElement Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                IWebElement ResultPanel = brz3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                string BeforeDragLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Logger.Instance.InfoLog("Before Drag " + BluRingZ3DViewerPage.Navigationone + " Location value : " + BeforeDragLocVal);
                List<string> MPRviewportValue = new List<string>() { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.ResultPanel };
                new Actions(Driver).SendKeys(Navigation1, "x");
                String[] step7 = brz3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigationone, Navigation1.Size.Width / 4, (Navigation1.Size.Height / 4), Navigation1.Size.Width / 4, Navigation1.Size.Height / 2, MPRviewportValue);
                string AfterDragLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Logger.Instance.InfoLog("After Drag " + BluRingZ3DViewerPage.Navigationone + " Location value : " + AfterDragLocVal);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                if (!BeforeDragLocVal.Equals(AfterDragLocVal) && step7[0].Equals("Lossy Compressed") && step7[1].Equals("Lossy Compressed") && step7[2].Equals("Lossy Compressed") && step7[3].Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 07<--");

                //STEP 08 :: Scroll through the Images in all other controls from all the views -3D viewing mode -Six up viewing mode -Curved MPR viewing mode -Calcium Scoring mode.
                Dictionary<string, List<String>> map = new Dictionary<string, List<String>>();
                map.Add(BluRingZ3DViewerPage.Three_3d_4, new List<string>() { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.Navigation3D1 });
                map.Add(BluRingZ3DViewerPage.Three_3d_6, new List<string>() { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Navigation3D2 });
                map.Add(BluRingZ3DViewerPage.CurvedMPR, new List<string>() { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.MPRPathNavigation, BluRingZ3DViewerPage._3DPathNavigation, BluRingZ3DViewerPage.CurvedMPR });
                map.Add(BluRingZ3DViewerPage.CalciumScoring, new List<string>() { BluRingZ3DViewerPage.CalciumScoring });

                bool ResultValue = true;
                int Count = 0;
                foreach (KeyValuePair<string, List<string>> kvp in map)
                {
                    BeforeDragLocVal = null; AfterDragLocVal = null;
                    brz3dvp.select3dlayout(kvp.Key);
                    PageLoadWait.WaitForFrameLoad(10);
                    SmartViewValue = Driver.FindElement(By.CssSelector(Locators.CssSelector.SmartViewSelectedValue));
                    if (SmartViewValue.GetAttribute("innerText").Equals(kvp.Key))
                    {
                        Logger.Instance.InfoLog(kvp.Key + " Viewer displayed successfully.");
                        if (kvp.Key.Equals(BluRingZ3DViewerPage.CalciumScoring))
                        {
                            PageLoadWait.WaitForFrameLoad(10);
                            brz3dvp.checkerrormsg("y");
                            Thread.Sleep(10000);
                            PageLoadWait.WaitForFrameLoad(5);
                            brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                            brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                            //IWebElement closebttn = Driver.FindElement(By.CssSelector(Locators.CssSelector.dialogclose));
                            //closebttn.Click();
                            PageLoadWait.WaitForFrameLoad(5);
                        }
                        brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                        PageLoadWait.WaitForFrameLoad(10);
                        int a = 0;  // Selection tool should apply only on first element in 'map' list
                        string ControlLocVal = null;
                        IWebElement ControlElement = null;
                        String[] ViewportBottomValue = null;
                        try
                        {
                            if (kvp.Key.Equals(BluRingZ3DViewerPage.Three_3d_4))
                            {
                                ControlElement = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                                ControlLocVal = BluRingZ3DViewerPage.Navigation3D1;
                            }
                            else
                            {
                                ControlElement = brz3dvp.controlelement(kvp.Value[0]);
                                ControlLocVal = kvp.Value[0];
                            }
                            BeforeDragLocVal = brz3dvp.GetTopleftAnnotationLocationValue(ControlLocVal);
                            Logger.Instance.InfoLog("Before Drag " + ControlLocVal + " Location value : " + BeforeDragLocVal);
                            if (kvp.Key.Equals(BluRingZ3DViewerPage.CalciumScoring))
                            {
                                ViewportBottomValue = brz3dvp.CheckLossyInteraction(ControlLocVal, ControlElement.Size.Width / 4, (ControlElement.Size.Height / 2), ControlElement.Size.Width / 4, ControlElement.Size.Height / 4, kvp.Value);
                            }
                            else
                            {
                                ViewportBottomValue = brz3dvp.CheckLossyInteraction(ControlLocVal, ControlElement.Size.Width / 4, (ControlElement.Size.Height / 4), ControlElement.Size.Width / 4, ControlElement.Size.Height / 2, kvp.Value);
                            }
                            for (int i = 0; i < ViewportBottomValue.Length; i++)
                            {
                                if (!ViewportBottomValue[i].Equals("Lossy Compressed"))
                                {
                                    ResultValue = false;
                                    Logger.Instance.ErrorLog("'Lossy Compression' annotation is not displayed during the interaction.");
                                    break;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Instance.ErrorLog("'Lossy Compression' annotation is not displayed during the interaction. " + e.Message);
                            break;
                        }
                        if (!ResultValue)
                            break;
                        AfterDragLocVal = brz3dvp.GetTopleftAnnotationLocationValue(ControlLocVal);
                        Logger.Instance.InfoLog("After Drag " + ControlLocVal + " Location value : " + AfterDragLocVal);
                        brz3dvp.select3DTools(Z3DTools.Reset);
                        PageLoadWait.WaitForFrameLoad(5);
                        if (!BeforeDragLocVal.Equals(AfterDragLocVal))
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
                Logger.Instance.InfoLog("-->Completed Step : 08<--");

                //STEP 09 ::  From the iCA server side, Go to the below location and verify .C \drs\TEMP\Img3D
                DateTime EndTime = DateTime.Now;
                int png = 0;
                foreach (FileInfo flInfo in di.GetFiles("*.png"))
                {
                    DateTime dateToCheck = flInfo.CreationTime;
                    if (dateToCheck >= StartTime && dateToCheck < EndTime)
                    {
                        png++;
                    }
                }
                if (!png.Equals(0))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 09<--");

                //STEP 10 :: Click on the 3D Settings button from the user settings under the Global toolbar options and move the MPR and 3D interactive quality sliders lesser 100%. ( Range = 1% to 99%).  Click on Save button. Note: If the slider is between 1 % to 99 %, JPEG will be used
                StartTime = DateTime.Now;
                MPRInteractiveQuality = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 90);
                InteractiveQuality3D = brz3dvp.change3dsettings(BluRingZ3DViewerPage.InteractiveQuality3D, 90);
                if (!MPRInteractiveQuality || !InteractiveQuality3D)
                    throw new Exception("Cannot be set 3D Settings");
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                Logger.Instance.InfoLog("-->Completed Step : 10<--");

                //STEP 11 :: Repeat steps 6-8.
                Dictionary<string, List<String>> map1 = new Dictionary<string, List<String>>();
                map1.Add(BluRingZ3DViewerPage.MPR, new List<string>() { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.ResultPanel });
                map1.Add(BluRingZ3DViewerPage.Three_3d_4, new List<string>() { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.Navigation3D1 });
                map1.Add(BluRingZ3DViewerPage.Three_3d_6, new List<string>() { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.ResultPanel, BluRingZ3DViewerPage.Navigation3D1, BluRingZ3DViewerPage.Navigation3D2 });
                map1.Add(BluRingZ3DViewerPage.CurvedMPR, new List<string>() { BluRingZ3DViewerPage.Navigationone, BluRingZ3DViewerPage.Navigationtwo, BluRingZ3DViewerPage.Navigationthree, BluRingZ3DViewerPage.MPRPathNavigation, BluRingZ3DViewerPage._3DPathNavigation, BluRingZ3DViewerPage.CurvedMPR });
                map1.Add(BluRingZ3DViewerPage.CalciumScoring, new List<string>() { BluRingZ3DViewerPage.CalciumScoring });

                ResultValue = true;
                Count = 0;
                foreach (KeyValuePair<string, List<string>> kvp in map1)
                {
                    BeforeDragLocVal = null; AfterDragLocVal = null;
                    brz3dvp.select3dlayout(kvp.Key);
                    PageLoadWait.WaitForFrameLoad(10);
                    SmartViewValue = Driver.FindElement(By.CssSelector(Locators.CssSelector.SmartViewSelectedValue));
                    if (SmartViewValue.GetAttribute("innerText").Equals(kvp.Key))
                    {
                        Logger.Instance.InfoLog(kvp.Key + " Viewer displayed successfully.");
                        if (kvp.Key.Equals(BluRingZ3DViewerPage.CalciumScoring))
                        {
                            brz3dvp.checkerrormsg("y");
                            PageLoadWait.WaitForFrameLoad(5);
                            brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                            //IWebElement closebttn = Driver.FindElement(By.CssSelector(Locators.CssSelector.dialogclose));
                            //closebttn.Click();
                            PageLoadWait.WaitForFrameLoad(5);
                        }
                        brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                        PageLoadWait.WaitForFrameLoad(10);
                        int a = 0;  // Selection tool should apply only on first element in 'map' list
                        string ControlLocVal = null;
                        IWebElement ControlElement = null;
                        String[] ViewportBottomValue = null;
                        try
                        {
                            if (kvp.Key.Equals(BluRingZ3DViewerPage.Three_3d_4))
                            {
                                ControlElement = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                                ControlLocVal = BluRingZ3DViewerPage.Navigation3D1;
                            }
                            else
                            {
                                ControlElement = brz3dvp.controlelement(kvp.Value[0]);
                                ControlLocVal = kvp.Value[0];
                            }
                            BeforeDragLocVal = brz3dvp.GetTopleftAnnotationLocationValue(ControlLocVal);
                            Logger.Instance.InfoLog("Before Drag " + ControlLocVal + " Location value : " + BeforeDragLocVal);
                            if (kvp.Key.Equals(BluRingZ3DViewerPage.CalciumScoring))
                            {
                                ViewportBottomValue = brz3dvp.CheckLossyInteraction(ControlLocVal, ControlElement.Size.Width / 4, (ControlElement.Size.Height / 2), ControlElement.Size.Width / 4, ControlElement.Size.Height / 4, kvp.Value);
                            }
                            else
                            {
                                ViewportBottomValue = brz3dvp.CheckLossyInteraction(ControlLocVal, ControlElement.Size.Width / 4, (ControlElement.Size.Height / 4), ControlElement.Size.Width / 4, ControlElement.Size.Height / 2, kvp.Value);
                            }
                                for (int i = 0; i < ViewportBottomValue.Length; i++)
                            {
                                if (!ViewportBottomValue[i].Equals("Lossy Compressed"))
                                {
                                    ResultValue = false;
                                    Logger.Instance.ErrorLog("'Lossy Compression' annotation is not displayed during the interaction.");
                                    break;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Instance.ErrorLog("'Lossy Compression' annotation is not displayed during the interaction. " + e.Message);
                            break;
                        }
                        if (!ResultValue)
                            break;
                        AfterDragLocVal = brz3dvp.GetTopleftAnnotationLocationValue(ControlLocVal);
                        Logger.Instance.InfoLog("After Drag " + ControlLocVal + " Location value : " + AfterDragLocVal);
                        brz3dvp.select3DTools(Z3DTools.Reset);
                        PageLoadWait.WaitForFrameLoad(5);
                        if (!BeforeDragLocVal.Equals(AfterDragLocVal))
                            Count++;
                    }
                }

                if (map1.Count.Equals(Count))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 11<--");

                //STEP 12 :: From the iCA server side, Go to the below location and verify. C \drs\TEMP\Img3D
                EndTime = DateTime.Now;
                int jpg = 0;
                foreach (FileInfo flInfo in di.GetFiles("*.jpg"))
                {
                    DateTime dateToCheck = flInfo.CreationTime;
                    if (dateToCheck >= StartTime && dateToCheck < EndTime)
                    {
                        jpg++;
                    }
                }
                if (!jpg.Equals(0))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 12<--");

                //STEP 13 :: Select the 2D option from the smart view drop down.
                IWebElement viewer3dbutton = Driver.FindElement(By.CssSelector(Locators.CssSelector.ViewerButton3D));
                ClickElement(viewer3dbutton);
                Thread.Sleep(5000);
                PageLoadWait.WaitForElementToDisplay(brz3dvp.DropDownBox3D());
                IList<IWebElement> weli = brz3dvp.layoutlist();
                foreach (IWebElement we in weli)
                {
                    if (we.Text.Equals(BluRingZ3DViewerPage.Two_2D))
                    {
                        ClickElement(we);
                        break;
                    }
                }
                PageLoadWait.WaitForFrameLoad(10);
                SmartViewValue = Driver.FindElement(By.CssSelector(Locators.CssSelector.SmartViewSelectedValue));
                if (SmartViewValue.GetAttribute("innerText").Equals(BluRingZ3DViewerPage.Two_2D))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 13<--");

                //STEP 14 :: Click on the close button from the Global toolbar.
                brz3dvp.CloseViewer();
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                IWebElement SelectedTab = Driver.FindElement(By.CssSelector(Locators.CssSelector.SelectedTab));
                if (SelectedTab.GetAttribute("title").Equals("Studies"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 14<--");

                //STEP 15 :: Search for a study that has a No lossy compressed series
                //STEP 16 :: Load the study in universal viewer.
                //STEP 17 :: Select a 3D supported series and Select the MPR view option from the smart view drop down.
                Result = brz3dvp.searchandopenstudyin3D(values[0], values[1], ChangeSettings: "No");
                if (Result)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Steps : 15,16,17<--");

                //STEP 18 :: Click on the 3D Settings button from the user settings under the Global toolbar options and move the MPR/3D interactive quality sliders to 100%.
                StartTime = DateTime.Now;
                //SmartViewValue = Driver.FindElement(By.CssSelector(Locators.CssSelector.SmartViewSelectedValue));
                MPRInteractiveQuality = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 100);
                InteractiveQuality3D = brz3dvp.change3dsettings(BluRingZ3DViewerPage.InteractiveQuality3D, 100);
                if (!MPRInteractiveQuality || !InteractiveQuality3D)
                    throw new Exception("Cannot be set 3D Settings");
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                Logger.Instance.InfoLog("-->Completed Step : 18<--");

                //STEP 19 :: Select the scroll tool from the 3D tool box.
                Scroll = brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                PageLoadWait.WaitForFrameLoad(10);
                if (Scroll)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();
                Logger.Instance.InfoLog("-->Completed Step : 19<--");

                //STEP 20 :: Scroll through the image in MPR controls.
                BeforeDragLocVal = null; AfterDragLocVal = null;
                Navigation1 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone);
                Navigation2 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo);
                Navigation3 = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree);
                ResultPanel = brz3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel);
                BeforeDragLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Logger.Instance.InfoLog("Before Drag " + BluRingZ3DViewerPage.Navigationone + " Location value : " + BeforeDragLocVal);
                String[] step20 = brz3dvp.CheckLossyInteraction(BluRingZ3DViewerPage.Navigationone, Navigation1.Size.Width / 4, (Navigation1.Size.Height / 4), Navigation1.Size.Width / 4, Navigation1.Size.Height / 2, MPRviewportValue);
                AfterDragLocVal = brz3dvp.GetTopleftAnnotationLocationValue(BluRingZ3DViewerPage.Navigationone);
                Logger.Instance.InfoLog("After Drag " + BluRingZ3DViewerPage.Navigationone + " Location value : " + AfterDragLocVal);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                if (!BeforeDragLocVal.Equals(AfterDragLocVal) && !step20[0].Equals("Lossy Compressed") && !step20[1].Equals("Lossy Compressed") && !step20[2].Equals("Lossy Compressed") && !step20[3].Equals("Lossy Compressed"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 20<--");

                //STEP 21 :: Scroll through the Images in all other controls from all the views -3D viewing mode -Six up viewing mode -Curved MPR viewing mode -Calcium Scoring mode.
                ResultValue = true;
                Count = 0;
                foreach (KeyValuePair<string, List<string>> kvp in map)
                {
                    BeforeDragLocVal = null; AfterDragLocVal = null;
                    brz3dvp.select3dlayout(kvp.Key);
                    PageLoadWait.WaitForFrameLoad(10);
                    SmartViewValue = Driver.FindElement(By.CssSelector(Locators.CssSelector.SmartViewSelectedValue));
                    if (SmartViewValue.GetAttribute("innerText").Equals(kvp.Key))
                    {
                        Logger.Instance.InfoLog(kvp.Key + " Viewer displayed successfully.");
                        if (kvp.Key.Equals(BluRingZ3DViewerPage.CalciumScoring))
                        {
                            brz3dvp.checkerrormsg("y");
                            Thread.Sleep(10000);
                            PageLoadWait.WaitForFrameLoad(5);
                            brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                            brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                            //IWebElement closebttn = Driver.FindElement(By.CssSelector(Locators.CssSelector.dialogclose));
                            //closebttn.Click();
                            PageLoadWait.WaitForFrameLoad(5);
                        }
                        brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                        PageLoadWait.WaitForFrameLoad(10);
                        int a = 0;  // Selection tool should apply only on first element in 'map' list
                        string ControlLocVal = null;
                        IWebElement ControlElement = null;
                        String[] ViewportBottomValue = null;
                        try
                        {
                            if (kvp.Key.Equals(BluRingZ3DViewerPage.Three_3d_4))
                            {
                                ControlElement = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                                ControlLocVal = BluRingZ3DViewerPage.Navigation3D1;
                            }
                            else
                            {
                                ControlElement = brz3dvp.controlelement(kvp.Value[0]);
                                ControlLocVal = kvp.Value[0];
                            }
                            BeforeDragLocVal = brz3dvp.GetTopleftAnnotationLocationValue(ControlLocVal);
                            Logger.Instance.InfoLog("Before Drag " + ControlLocVal + " Location value : " + BeforeDragLocVal);
                            if (kvp.Key.Equals(BluRingZ3DViewerPage.CalciumScoring))
                            {
                                ViewportBottomValue = brz3dvp.CheckLossyInteraction(ControlLocVal, ControlElement.Size.Width / 4, (ControlElement.Size.Height / 2), ControlElement.Size.Width / 4, ControlElement.Size.Height / 4, kvp.Value);
                            }
                            else
                            {
                                ViewportBottomValue = brz3dvp.CheckLossyInteraction(ControlLocVal, ControlElement.Size.Width / 4, (ControlElement.Size.Height / 4), ControlElement.Size.Width / 4, ControlElement.Size.Height / 2, kvp.Value);
                            }
                                for (int i = 0; i < ViewportBottomValue.Length; i++)
                            {
                                if (ViewportBottomValue[i].Equals("Lossy Compressed"))
                                {
                                    ResultValue = false;
                                    Logger.Instance.ErrorLog("'Lossy Compression' annotation is displayed during the interaction.");
                                    break;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Instance.ErrorLog("'Lossy Compression' annotation is displayed during the interaction. " + e.Message);
                            break;
                        }
                        if (!ResultValue)
                            break;
                        AfterDragLocVal = brz3dvp.GetTopleftAnnotationLocationValue(ControlLocVal);
                        Logger.Instance.InfoLog("After Drag " + ControlLocVal + " Location value : " + AfterDragLocVal);
                        brz3dvp.select3DTools(Z3DTools.Reset);
                        PageLoadWait.WaitForFrameLoad(5);
                        if (!BeforeDragLocVal.Equals(AfterDragLocVal))
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
                Logger.Instance.InfoLog("-->Completed Step : 21<--");

                //STEP 22 ::  From the iCA server side, Go to the below location and verify .C \drs\TEMP\Img3D
                EndTime = DateTime.Now;
                png = 0;
                foreach (FileInfo flInfo in di.GetFiles("*.png"))
                {
                    DateTime dateToCheck = flInfo.CreationTime;
                    if (dateToCheck >= StartTime && dateToCheck < EndTime)
                    {
                        png++;
                    }
                }
                if (!png.Equals(0))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 22<--");

                //STEP 23 :: Click on the 3D Settings button from the user settings under the Global toolbar options and move the MPR and 3D interactive quality sliders lesser 100%. ( Range = 1% to 99%). Click on Save button.Note: If the slider is between 1 % to 99 %, JPEG will be used
                StartTime = DateTime.Now;
                MPRInteractiveQuality = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 90);
                InteractiveQuality3D = brz3dvp.change3dsettings(BluRingZ3DViewerPage.InteractiveQuality3D, 90);
                if (!MPRInteractiveQuality || !InteractiveQuality3D)
                    throw new Exception("Cannot be set 3D Settings");
                else
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                Logger.Instance.InfoLog("-->Completed Step : 23<--");

                //STEP 24 :: Repeat steps 6-8.
                ResultValue = true;
                Count = 0;
                foreach (KeyValuePair<string, List<string>> kvp in map1)
                {
                    BeforeDragLocVal = null; AfterDragLocVal = null;
                    brz3dvp.select3dlayout(kvp.Key);
                    PageLoadWait.WaitForFrameLoad(5);
                    SmartViewValue = Driver.FindElement(By.CssSelector(Locators.CssSelector.SmartViewSelectedValue));
                    if (SmartViewValue.Text.Equals(kvp.Key))
                    {
                        Logger.Instance.InfoLog(kvp.Key + " Viewer displayed successfully.");
                        if (kvp.Key.Equals(BluRingZ3DViewerPage.CalciumScoring))
                        {
                            brz3dvp.checkerrormsg("y");
                            Thread.Sleep(10000);
                            PageLoadWait.WaitForFrameLoad(5);
                            brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                            //IWebElement closebttn = Driver.FindElement(By.CssSelector(Locators.CssSelector.dialogclose));
                            //closebttn.Click();
                            PageLoadWait.WaitForFrameLoad(5);
                        }
                        brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                        PageLoadWait.WaitForFrameLoad(10);
                        string ControlLocVal = null;
                        IWebElement ControlElement = null;
                        String[] ViewportBottomValue = null;
                        try
                        {
                            if (kvp.Key.Equals(BluRingZ3DViewerPage.Three_3d_4))
                            {
                                ControlElement = brz3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1);
                                ControlLocVal = BluRingZ3DViewerPage.Navigation3D1;
                            }
                            else
                            {
                                ControlElement = brz3dvp.controlelement(kvp.Value[0]);
                                ControlLocVal = kvp.Value[0];
                            }
                            BeforeDragLocVal = brz3dvp.GetTopleftAnnotationLocationValue(ControlLocVal);
                            Logger.Instance.InfoLog("Before Drag " + ControlLocVal + " Location value : " + BeforeDragLocVal);
                            if (kvp.Key.Equals(BluRingZ3DViewerPage.CalciumScoring))
                            {
                                ViewportBottomValue = brz3dvp.CheckLossyInteraction(ControlLocVal, ControlElement.Size.Width / 4, (ControlElement.Size.Height / 2), ControlElement.Size.Width / 4, ControlElement.Size.Height / 4, kvp.Value);
                            }
                            else
                            {
                                ViewportBottomValue = brz3dvp.CheckLossyInteraction(ControlLocVal, ControlElement.Size.Width / 4, (ControlElement.Size.Height / 4), ControlElement.Size.Width / 4, ControlElement.Size.Height / 2, kvp.Value);
                            }
                            int itr = 0;
                            for (int i = 0; i < ViewportBottomValue.Length; i++)
                            {
                                if (kvp.Key.Equals(BluRingZ3DViewerPage.Three_3d_4))
                                {
                                    if (!ViewportBottomValue[3].Equals("Lossy Compressed"))
                                    {
                                        ResultValue = false;
                                        Logger.Instance.ErrorLog("'Lossy Compression' annotation is not displayed during the interaction.");
                                        break;
                                    }
                                }
                                else if (kvp.Key.Equals(BluRingZ3DViewerPage.Three_3d_6) || kvp.Key.Equals(BluRingZ3DViewerPage.CurvedMPR))
                                {
                                    if (ViewportBottomValue[i].Equals("Lossy Compressed"))
                                    {
                                        itr++;
                                        Logger.Instance.ErrorLog("'Lossy Compression' annotation is displayed during the interaction.");
                                    }
                                }
                                else if (kvp.Key.Equals(BluRingZ3DViewerPage.MPR))
                                {
                                    if (ViewportBottomValue[i].Equals("Lossy Compressed"))
                                    {
                                        itr++;
                                        Logger.Instance.ErrorLog("'Lossy Compression' annotation is displayed during the interaction.");
                                    }
                                }
                                else if (!ViewportBottomValue[i].Equals("Lossy Compressed"))
                                {
                                    ResultValue = false;
                                    Logger.Instance.ErrorLog("'Lossy Compression' annotation is not displayed during the interaction.");
                                    break;
                                }
                            }
                            if (kvp.Key.Equals(BluRingZ3DViewerPage.Three_3d_6))
                            {
                                if (itr == 4)
                                    ResultValue = true;
                            }
                            if(kvp.Key.Equals(BluRingZ3DViewerPage.CurvedMPR))
                            {
                                if (itr > 0)
                                    ResultValue = true;
                            }
                            if (kvp.Key.Equals(BluRingZ3DViewerPage.MPR))
                            {
                                if(itr == 2)
                                    ResultValue = true;
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Instance.ErrorLog("'Lossy Compression' annotation is not displayed during the interaction. " + e.Message);
                            break;
                        }
                        if (!ResultValue)
                            break;
                        AfterDragLocVal = brz3dvp.GetTopleftAnnotationLocationValue(ControlLocVal);
                        Logger.Instance.InfoLog("After Drag " + ControlLocVal + " Location value : " + AfterDragLocVal);
                        brz3dvp.select3DTools(Z3DTools.Reset);
                        PageLoadWait.WaitForFrameLoad(5);
                        if (!BeforeDragLocVal.Equals(AfterDragLocVal))
                            Count++;
                    }
                }
                Logger.Instance.InfoLog("Count value : " + Count + " map1 count is : " + map1.Count);
                if (map1.Count.Equals(Count))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 24<--");

                //STEP 25 :: From the iCA server side, Go to the below location and verify. C \drs\TEMP\Img3D
                EndTime = DateTime.Now;
                jpg = 0;
                foreach (FileInfo flInfo in di.GetFiles("*.jpg"))
                {
                    DateTime dateToCheck = flInfo.CreationTime;
                    if (dateToCheck >= StartTime && dateToCheck < EndTime)
                    {
                        jpg++;
                    }
                }
                if (!jpg.Equals(0))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 25<--");

                //STEP 26 :: Select the 2D option from the smart view drop down.
                viewer3dbutton = Driver.FindElement(By.CssSelector(Locators.CssSelector.ViewerButton3D));
                ClickElement(viewer3dbutton);
                Thread.Sleep(5000);
                PageLoadWait.WaitForElementToDisplay(brz3dvp.DropDownBox3D());
                weli = brz3dvp.layoutlist();
                foreach (IWebElement we in weli)
                {
                    if (we.Text.Equals(BluRingZ3DViewerPage.Two_2D))
                    {
                        ClickElement(we);
                        break;
                    }
                }
                PageLoadWait.WaitForFrameLoad(10);
                SmartViewValue = Driver.FindElement(By.CssSelector(Locators.CssSelector.SmartViewSelectedValue));
                if (SmartViewValue.GetAttribute("innerText").Equals(BluRingZ3DViewerPage.Two_2D))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 26<--");

                //STEP 27 :: Click on the close button from the Global toolbar.
                brz3dvp.CloseViewer();
                PageLoadWait.WaitForFrameLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
                SelectedTab = Driver.FindElement(By.CssSelector(Locators.CssSelector.SelectedTab));
                if (SelectedTab.GetAttribute("title").Equals("Studies"))
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }
                Logger.Instance.InfoLog("-->Completed Step : 27<--");

                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
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
                login.Logout();
            }
        }

        public TestCaseResult Test_163388(string testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            Studies studies = new Studies();
            BluRingZ3DViewerPage brz3dvp = new BluRingZ3DViewerPage();
            BluRingViewer bluview = new BluRingViewer();
            string Patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            string imagethumb = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "ThumbnailDescription");
            string requirements = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "TestDataRequirements");
            String configlocation = requirements.Split('|')[0];
            String[] tag = { requirements.Split('|')[1], requirements.Split('|')[2], requirements.Split('|')[3] },
            attribute = { requirements.Split('|')[4], requirements.Split('|')[5], requirements.Split('|')[6] },
            value = { requirements.Split('|')[7], requirements.Split('|')[8], requirements.Split('|')[9] };

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            int ExecutedSteps = -1;
            try
            {
                //step 01
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                bool Result = brz3dvp.searchandopenstudyin3D(Patientid, imagethumb, ChangeSettings: "No");
                if (Result)
                    result.steps[++ExecutedSteps].StepPass();
                else
                {
                    Logger.Instance.ErrorLog("Failed while launching study");
                    throw new Exception("Failed to open study");
                }

                //step 02
                int counter = 0;
                var doc = new XmlDocument();
                doc.Load(configlocation);
                XmlNodeList nodelist1 = doc.GetElementsByTagName(tag[0]);
                for (int i = 0; i < nodelist1.Count; i++)
                {
                    if (nodelist1[i].Attributes[attribute[0]].Value.Equals(value[0]))
                    {
                        counter++;
                    }
                }
                XmlNodeList nodelist2 = doc.GetElementsByTagName(tag[1]);
                for (int i = 0; i < nodelist2.Count; i++)
                {
                    if (nodelist2[i].Attributes[attribute[1]].Value.Equals(value[1]) || nodelist2[i].Attributes[attribute[2]].Value.Equals(value[2]))
                    {
                        counter++;
                    }
                }
                XmlNodeList nodelist3 = doc.GetElementsByTagName(tag[2]);
                for (int i = 0; i < nodelist3.Count; i++)
                {
                    if (nodelist3[i].Attributes[attribute[1]].Value.Equals(value[1]) || nodelist3[i].Attributes[attribute[2]].Value.Equals(value[2]))
                    {
                        counter++;
                    }
                }
                if (counter == 5)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 03
                bool Result1 = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 1);
                bool Result2 = brz3dvp.change3dsettings(BluRingZ3DViewerPage.InteractiveQuality3D, 1);
                if (Result1 && Result2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    throw new Exception("Changing MPR and 3D Interactive Quality Settings to 1 failed");

                //step 04
                new Actions(Driver).SendKeys("x").Build().Perform();
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Window_Level);
                PageLoadWait.WaitForFrameLoad(5);
                bool Step4_1 = brz3dvp.interactioncheck(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationone), testid, ExecutedSteps + 37, 132, 130, 132);
                Logger.Instance.InfoLog("The result of Window_Level tool application over navigation 1 in mpr layout is : " + Step4_1.ToString());
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                PageLoadWait.WaitForFrameLoad(5);
                bool Step4_2 = brz3dvp.interactioncheck(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), testid, ExecutedSteps + 38, 132, 130, 132);
                Logger.Instance.InfoLog("The result of zoom tool application over navigation 2 in mpr layout is : " + Step4_2.ToString());
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Pan);
                PageLoadWait.WaitForFrameLoad(5);
                bool Step4_3 = brz3dvp.interactioncheck(brz3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel), testid, ExecutedSteps + 44, 132, 130, 132);
                String annotationvalue = brz3dvp.GetCenterBottomAnnotationLocationValue(brz3dvp.controlelement(BluRingZ3DViewerPage.ResultPanel));
                Logger.Instance.InfoLog("The result of window levl tool application over Result panel in mpr layout is : " + Step4_3.ToString());
                if (Step4_1 && Step4_2 && Step4_3&& annotationvalue != "Lossy Compressed")
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //STep 05 
                bool step5 = brz3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_4);
                if (step5)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 06 & 07
                bool Result3 = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRInteractiveQuality, 100);
                bool Result4 = brz3dvp.change3dsettings(BluRingZ3DViewerPage.InteractiveQuality3D, 100);
                if (Result3 && Result4)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    throw new Exception("Changing 3D and MPR interactive quality Settings to 100 failed");

                //step 08
                brz3dvp.select3DTools(Z3DTools.Window_Level);
                PageLoadWait.WaitForFrameLoad(5);
                bool Step8_1 = brz3dvp.interactioncheck(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 29, 107, 40, 66);
                Logger.Instance.InfoLog("The result of scroll tool application over navigation3D1 in 3D 4:1 layout is : " + Step8_1.ToString());
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Interactive_Zoom, BluRingZ3DViewerPage.Navigation3D1);
                PageLoadWait.WaitForFrameLoad(5);
                bool Step8_2 = brz3dvp.interactioncheck(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 30, 107, 40, 66);
                Logger.Instance.InfoLog("The result of zoom tool application over navigation3D1 in 3D 4:1 layout is : " + Step8_2.ToString());
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Pan, BluRingZ3DViewerPage.Navigation3D1);
                PageLoadWait.WaitForFrameLoad(5);
                bool Step8_3 = brz3dvp.interactioncheck(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 31, 107, 40, 66);
                Logger.Instance.InfoLog("The result of rotate tool application over navigation3D1 in 3D 4:1 layout is : " + Step8_3.ToString());
                if (Step8_1 == false && Step8_2 == false && Step8_3 == false)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step09
                bool step9 = brz3dvp.select3dlayout(BluRingZ3DViewerPage.Three_3d_6);
                if (step9)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 10 & 11
                bool Result5 = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRFinalQuality, 1);
                bool Result6 = brz3dvp.change3dsettings(BluRingZ3DViewerPage.FinalQuality3D, 1);
                if (Result5 && Result6)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    throw new Exception("Changing 3D and MPR Final quality Settings to 1 failed");

                //step 12
                int loop = 0;
                brz3dvp.ChangeViewMode();
                PageLoadWait.WaitForFrameLoad(5);
                IList<IWebElement> elements1 = brz3dvp.controlImage();
                foreach (IWebElement element1 in elements1)
                {
                    String annotationvalue1 = brz3dvp.GetCenterBottomAnnotationLocationValue(element1);
                    if (annotationvalue1.Equals("Lossy Compressed"))
                    {
                        loop++;
                    }
                }
                if (loop == elements1.Count)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();


                //step 13
                brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                bool Step13_1 = brz3dvp.interactioncheck(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D1), testid, ExecutedSteps + 1, 132, 130, 132,false);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                Logger.Instance.InfoLog("The result of scroll tool over calcium scoring is : " + Step13_1.ToString());
                brz3dvp.select3DTools(Z3DTools.Window_Level);
                PageLoadWait.WaitForFrameLoad(5);
                bool Step13_2 = brz3dvp.interactioncheck(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigation3D2), testid, ExecutedSteps + 2, 255, 255, 255,false);
                Logger.Instance.InfoLog("The result of window level tool over calcium scoring is : " + Step13_2.ToString());
                if (Step13_1 && Step13_2)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //Step14
                bool step14 = brz3dvp.select3dlayout(BluRingZ3DViewerPage.CurvedMPR);
                int loop2 = 0;
                IList<IWebElement> elements2 = brz3dvp.controlImage();
                foreach (IWebElement element2 in elements2)
                {
                    String annotationvalue2 = brz3dvp.GetCenterBottomAnnotationLocationValue(element2);
                    if (annotationvalue2.Equals("Lossy Compressed"))
                    {
                        loop2++;
                    }
                }
                if (step14 && loop2 == elements2.Count)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 15 & 16
                bool Result7 = brz3dvp.change3dsettings(BluRingZ3DViewerPage.MPRFinalQuality, 100);
                bool Result8 = brz3dvp.change3dsettings(BluRingZ3DViewerPage.FinalQuality3D, 100);
                if (Result5 && Result6)
                {
                    result.steps[++ExecutedSteps].StepPass();
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                    throw new Exception("Changing 3D and MPR Final quality Settings to 100 failed");

                //step 17
                int loop3 = 0;
                elements2 = brz3dvp.controlImage();
                foreach (IWebElement element2 in elements2)
                {
                    String annotationvalue2 = brz3dvp.GetCenterBottomAnnotationLocationValue(element2);
                    if (!annotationvalue2.Equals("Lossy Compressed"))
                    {
                        loop3++;
                    }
                }
                if (loop3 == elements2.Count)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 18
                brz3dvp.select3DTools(Z3DTools.Interactive_Zoom);
                PageLoadWait.WaitForFrameLoad(5);
                bool Step18_1 = brz3dvp.interactioncheck(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationtwo), testid, ExecutedSteps + 10, 132, 130, 132, false);
                Logger.Instance.InfoLog("The result of zoom tool application over navigation 2 in curved mpr layout is : " + Step18_1.ToString());
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Pan);
                PageLoadWait.WaitForFrameLoad(5);
                bool Step18_2 = brz3dvp.interactioncheck(brz3dvp.controlelement(BluRingZ3DViewerPage.Navigationthree), testid, ExecutedSteps + 11, 132, 130, 132, false);
                Logger.Instance.InfoLog("The result of rotate tool application over navigation 3 in curved mpr layout is : " + Step18_2.ToString());
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Window_Level);
                PageLoadWait.WaitForFrameLoad(5);
                bool Step18_3 = brz3dvp.interactioncheck(brz3dvp.controlelement(BluRingZ3DViewerPage.MPRPathNavigation), testid, ExecutedSteps + 12, 255, 255, 255, false);
                Logger.Instance.InfoLog("The result of window levl tool application over MPR Path Navigation in curved mpr layout is : " + Step18_3.ToString());
                if (Step18_1 == false && Step18_2 == false && Step18_3 == false)
                    result.steps[++ExecutedSteps].StepPass();
                else
                    result.steps[++ExecutedSteps].StepFail();

                //step 19
                bool Step19 = brz3dvp.select3dlayout(BluRingZ3DViewerPage.CalciumScoring);
                if (Step19)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();
                }

                //Step20
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                brz3dvp.Handle3dToolsDialogs(BluRingZ3DViewerPage.CalciumScoringDialog, "Close");
                brz3dvp.select3DTools(Z3DTools.Scrolling_Tool);
                PageLoadWait.WaitForFrameLoad(5);
                bool Step20_1 = brz3dvp.interactioncheck(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 3, 132, 130, 132, false);
                PageLoadWait.WaitForFrameLoad(5);
                brz3dvp.select3DTools(Z3DTools.Reset);
                PageLoadWait.WaitForFrameLoad(5);
                Logger.Instance.InfoLog("The result of scroll tool over calcium scoring is : " + Step20_1.ToString());
                brz3dvp.select3DTools(Z3DTools.Window_Level);
                PageLoadWait.WaitForFrameLoad(5);
                bool Step20_2 = brz3dvp.interactioncheck(brz3dvp.controlelement(BluRingZ3DViewerPage.CalciumScoring), testid, ExecutedSteps + 4, 255, 255, 255, false);
                Logger.Instance.InfoLog("The result of window level tool over calcium scoring is : " + Step20_2.ToString());
                PageLoadWait.WaitForFrameLoad(5);
                if (Step20_1 == false && Step20_2 == false)
                {
                    result.steps[++ExecutedSteps].StepPass();
                }
                else
                {
                    result.steps[++ExecutedSteps].StepFail();                

                }
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);
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
                brz3dvp.CloseViewer();
                login.Logout();
            }
        }
    }
}
