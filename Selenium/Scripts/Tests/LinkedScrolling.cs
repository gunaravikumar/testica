using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.eHR;
using Selenium.Scripts.Pages.iConnect;
using Selenium.Scripts.Pages.MergeServiceTool;
using Selenium.Scripts.Reusable.Generic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Selenium.Scripts.Tests
{
    class LinkedScrolling
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public EHR ehr { get; set; }

        public string[] link_5 = { "Link Selected", "Link Selected Offset", "Link All", "Link All Offset", "Unlink" };
        public string[] link_3 = { "Link Selected", "Link Selected Offset", "Unlink" };
        public string rgbavalue = "rgba(255, 160, 0, 1)";
        public string EA_91 = "VMSSA-5-38-91";
        public string EA_131 = "VMSSA-4-38-131";

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        public LinkedScrolling(String classname)
        {
            login = new Login();
            ehr = new EHR();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
        }


        /// <summary> 
        /// This Test Case is Verification of Linked scrolling "Initial Setup"
        /// </summary>

        public TestCaseResult Test_27987(String testid, String teststeps, int stepcount)
        {
            //-Initial Setup--
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;

            try
            {
                //Step-1
                //Test Data -    Linked Scrolling - Dataset worksheet is in the Attachment. 
                //Pre-Test Conditions -   Instruction on basic server configuration           
                //1) Install the test build on the server.          
                //2) License the server, and restart IIS.           
                //3) Configure the archive settings in the configuration            
                //4) Enable Patient tab by running Merge iConnect Access Service Tool -> 
                //Enable Features -> General tab -> selecting check box besides Enable Patient. Apply button & IISRESET
                //5) Enable EMPI by running Merge iConnect Access Service Tool -> Enable Features 
                //-> MPI tab -> selecting radio button besides Attribute based search. Apply button and IISRESET.              
                //Note - the EMPI consists of 3 datasources - \\topkick, \\forenza and \\rebel and Assigning Authorities - TOH&&, NYH&& and CH&&.

                //Pre-condition set manually, Open and close service tool first time
                try
                {
                    ServiceTool st = new ServiceTool();
                    WpfObjects wpfobject = new WpfObjects();
                    Taskbar bar = new Taskbar();
                    bar.Hide();

                    st.LaunchServiceTool();
                    st.NavigateToTab("Linked Scrolling");
                    wpfobject.WaitTillLoad();
                    st.RestartService();
                    wpfobject.WaitTillLoad();
                    st.CloseServiceTool();
                    wpfobject.WaitTillLoad();
                    bar.Show();
                }
                catch (Exception e) { }

                ExecutedSteps++;

                //Step-2
                //Pre-Test Conditions - Instruction on basic client configuration 
                //Client Browser to be used in this execution should be consistent to the latest version 
                //of Functional Product Specification for iConnect Access.
                //IstoreOnLine Datasources - \\topkick, \\forenza, \\rebel \\sylvan, \\notabug and \\optra

                //Pre-condition set manually, Open and close webaccess first time

                try
                {
                    login.DriverGoTo(login.url);
                    login.LoginIConnect(username, password);

                    DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                    login.Logout();
                }
                catch (Exception e) { }

                ExecutedSteps++;

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
                //login.Logout();

                //Return Result
                return result;
            }
        }

        /// <summary> 
        /// This Test Case is Verification of "Linking Series - Anatomic linking"
        /// </summary>

        public TestCaseResult Test_27988(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

            try
            {

                //Step-1
                //Complete steps in Initial Setups test case.
                //Completed
                ExecutedSteps++;

                //Step-2 **##--have to do
                //1.Ensure to select Link All Enabled in the Merge iConnect Access Service Tool-> Linked Scrolling tab & in the Domain Management tab. 
                //2.Ensure the viewer and series layout are added to the menu. Anatomic linking For anatomical linking, 
                //all of the linked series need to be approximately in the same plain. For example axial images can only be anatomically linked with other axial images


                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                Taskbar bar = new Taskbar();
                bar.Hide();

                st.LaunchServiceTool();
                st.NavigateToTab("Linked Scrolling");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);

                if (wpfobject.IsCheckBoxSelected("CB_LinkAll") == false)
                {
                    wpfobject.SelectCheckBox("CB_LinkAll");
                    Logger.Instance.InfoLog("Link All check box is selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Link All check box is already selected");
                }

                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("6");
                wpfobject.WaitTillLoad();
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();

                bar.Show();


                ExecutedSteps++;

                //Step-3
                //Login -
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();

                //Remove Link All check box
                if (domain.LinkAllCheckbox().Selected == true)
                {
                    domain.LinkAllCheckbox().Click();
                    Logger.Instance.InfoLog("Link scrolling Link All Check box is de-selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Link scrolling Link All Check box is already de-selected");
                }

                if (domain.LinkAllCheckbox().Selected == false)
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

                domain.ClickSaveDomain();


                //Step-4
                //Load MR-Series into the viewer
                //Accession:U-ID179490
                Studies study = (Studies)login.Navigate("Studies");
                study.SearchStudy(AccessionNo: AccessionID, Datasource: EA_91);
                study.SelectStudy("Accession", AccessionID);
                StudyViewer viewer = StudyViewer.LaunchStudy();

                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                var action4 = new Actions(BasePage.Driver);

                action4.DoubleClick(viewer.Thumbnails()[0]).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //The MR-Series 2 is loaded successfully into the viewer with 1 series and 1x1 layout.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status4 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status4 && viewer.SeriesViewPorts().Count == 1)
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

                //Step-5
                //Select the Linked Scrolling button from the menu.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkSelected);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //A drop down menu is displayed. Unlink, Link Selected, Link Selected Offset
                //Should not display 'Link All' & 'Link All Offset'

                IWebElement linkselected = viewer.GetReviewTool("Link Selected");
                viewer.JSMouseHover(linkselected);

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                IList<IWebElement> dropdown5 = BasePage.Driver.FindElements(By.CssSelector("li[title='Link Selected'] ul>li"));
                bool flag5 = true;
                IList<String> title = new List<String>();

                foreach (IWebElement dropdowntool in dropdown5)
                {
                    title.Add(dropdowntool.GetAttribute("title"));
                    if (!(dropdowntool.Enabled && dropdowntool.Displayed))
                    {
                        flag5 = false;
                        break;
                    }
                }

                if (flag5 && dropdown5.Count == 3 && title.SequenceEqual(link_3))
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

                viewer.LinkedScrollingCancelBtn().Click();
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);
                viewer.CloseStudy();

                //Step-6- Step-10 Patient tab

                for (int i = 0; i < 5; i++)
                    result.steps[++ExecutedSteps].status = "Not Automated";


                //Patients patient = (Patients)login.Navigate("Patients");
                //ExecutedSteps++;
                //study.SearchStudy("last Name", "John");
                //ExecutedSteps++;
                //study.SelectStudy("Study ID", "1111");
                //ExecutedSteps++;
                //StudyViewer.LaunchStudy();
                //ExecutedSteps++;


                //Step-11
                //Go back to the Domain Management tab, select SuperAdminGroup and select Edit button.

                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();
                ExecutedSteps++;


                //Step-12
                //Select/check box Enable Linked Scrolling Link All   Select Save button
                if (domain.LinkAllCheckbox().Selected == false)
                {
                    domain.LinkAllCheckbox().Click();
                    Logger.Instance.InfoLog("Link scrolling Link All Check box is selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Link scrolling Link All Check box is already selected");
                }
                domain.ClickSaveDomain();
                ExecutedSteps++;

                //Step-13-17 Patient tab
                for (int i = 0; i < 5; i++)
                    result.steps[++ExecutedSteps].status = "Not Automated";

                //Patients patient = (Patients)login.Navigate("Patients");
                //ExecutedSteps++;
                //study.SearchStudy("last Name", "John");
                //ExecutedSteps++;
                //study.SelectStudy("Study ID", "1111");
                //ExecutedSteps++;
                //StudyViewer.LaunchStudy();
                //ExecutedSteps++;


                //Step-18
                //Select Studies tab.
                login.Navigate("Studies");
                ExecutedSteps++;

                //Step-19
                //Load a study with multiple series - Brunschweiler, Rene ID#141496 located at \\optra.
                //Accession: U-ID179490
                study.SearchStudy(AccessionNo: AccessionID, Datasource: EA_91);
                study.SelectStudy("Accession", AccessionID);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status19 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status19 && viewer.SeriesViewPorts().Count == 4 && viewer.Thumbnails().Count == 5)
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


                //Step-20
                //Load MR-Series 2 into the viewer.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                var action20 = new Actions(BasePage.Driver);

                if (viewer.Thumbnails()[0] != null)
                {
                    action20.DoubleClick(viewer.Thumbnails()[0]).Build().Perform();
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                }

                //The MR-Series 2 is loaded successfully into the viewer with 1 series and 1x1 layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status20 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status20 && viewer.SeriesViewPorts().Count == 1)
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

                //Step-21
                //Hover the mouse cursor to the Linked Scrolling button from the menu.
                //a[class^='AnchorClass32 toplevel'] >img

                IWebElement linkSelected = viewer.GetReviewTool("Link Selected");
                viewer.JSMouseHover(linkSelected);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //A drop down menu is displayed. 
                //Link All, Link All Offset, Unlink, Link Selected, Link Selected Offset

                IList<IWebElement> dropdown21 = BasePage.Driver.FindElements(By.CssSelector("li[title='Link Selected'] ul>li"));
                title.Clear();
                bool flag21 = true;
                foreach (IWebElement dropdowntool in dropdown21)
                {
                    title.Add(dropdowntool.GetAttribute("title"));
                    if (!(dropdowntool.Enabled && dropdowntool.Displayed))
                    {
                        flag21 = false;
                        break;
                    }
                }
                if (flag21 && dropdown21.Count == 5 && title.SequenceEqual(link_5))
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

                //Select Reset for remove mouse hover.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);

                //Step-22
                //Select the Link All from the menu.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkAll);
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);

                //Nothing will happen-
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status22 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status22 && viewer.SeriesViewPorts().Count == 1)
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

                //step-23
                //Set the viewer to 2 series.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Two viewers are displayed side by side.
                if (viewer.SeriesViewer_1X1().Displayed &&
                    viewer.SeriesViewer_1X2().Displayed &&
                    viewer.SeriesViewPorts().Count == 2)
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

                //Step-24
                //Load MR-Series 2 on the left and MR-Series 3 on the right window

                var action24 = new Actions(BasePage.Driver);

                action24.DragAndDrop(viewer.Thumbnails()[0], viewer.SeriesViewer_1X1()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                action24.DragAndDrop(viewer.Thumbnails()[1], viewer.SeriesViewer_1X2()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status24 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status24 && viewer.SeriesViewPorts().Count == 2)
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

                //Step-25
                //Select one of the series as a reference viewer (MR-Series 2)   
                //Change series layout to 1x1.
                viewer.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //The enclosed orange window is displayed around MR-Series 2

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status25 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status25 && viewer.SeriesViewPorts().Count == 1)
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



                //Step-26
                //Hover the mouse cursor to the Linked Scrolling button from the menu.

                IWebElement linkAll = viewer.GetReviewTool("Link All");
                viewer.JSMouseHover(linkAll);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);


                //A drop down menu is displayed. 
                //Link All, Link All Offset, Unlink, Link Selected, Link Selected Offset

                IList<IWebElement> dropdown26 = BasePage.Driver.FindElements(By.CssSelector("li[title='Link All'] ul>li"));
                title.Clear();
                bool flag26 = true;
                foreach (IWebElement dropdowntool in dropdown26)
                {
                    title.Add(dropdowntool.GetAttribute("title"));
                    if (!(dropdowntool.Enabled && dropdowntool.Displayed))
                    {
                        flag26 = false;
                        break;
                    }
                }
                if (flag26 && dropdown26.Count == 5 && title.SequenceEqual(link_5))
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

                viewer.GetReviewTool("Reset").Click();


                //Step-27
                //Set series layout to 1X2. Select Link Selected from the list
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkSelected);
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);

                //A window with two black square windows is displayed.
                //(Square 1 represents for the series in the left window and Square 2 represents for the series in the right window)

                if (viewer.LinkSelectTableCheckBoxList().Count == 2 &&
                    viewer.LinkSelectTableCheckBox(1, 1).Displayed &&
                    viewer.LinkSelectTableCheckBox(1, 2).Displayed)
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

                //Step-28
                //Select square 1 and square 2 and then green check mark.

                //
                viewer.SelectLinkedCheckBox(1, 1);
                viewer.SelectLinkedCheckBox(1, 2);
                viewer.LinkedScrollingCheckBtn().Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //MR-Series 2 and MR-Series 3 are linked.An anatomical linking icon is displayed on the right top corner of the image.
                //At this point the MR-Series 3 is referred as a Base viewer

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status28 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status28 &&
                    viewer.LinkScrollingStatusImageList().Count == 2 &&
                    viewer.LinkScrollingStatusImage(1, 1).Displayed &&
                    viewer.LinkScrollingStatusImage(1, 2).Displayed)
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

                //Step-29

                //Scroll to the image 4 in the series MR-Series 2 by using down triangle.
                //(example SliceLocation - -71.255205746101)

                for (int i = 0; i < 3; i++)
                {
                    viewer.ClickDownArrowbutton(1, 1);
                }

                //In the base viewer MR-Series 3, the image 6 is displayed.
                //(example SliceLocation - -71.005206635607)

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status29 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status29 && viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("6"))
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

                //Step-30
                //Continue scrolling to the image 5 in the series MR-Series 2 by using down triangle.
                //(example SliceLocation - -75.15520382913)

                viewer.ClickDownArrowbutton(1, 1);


                //In the base viewer MR-Series 3, the image 8 is displayed.
                //(example SliceLocation - -74.405206572029)

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status30 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status30 && viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("8"))
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

                //Step-31
                //Continue scrolling to the image 6 in the series MR-Series 2 by using down triangle.
                //(example SliceLocation - -79.055209531099)

                viewer.ClickDownArrowbutton(1, 1);

                //In the base viewer MR-Series 3, the image 11 is displayed.
                //(example SliceLocation - -79.505206476661)

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status31 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status31 && viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("11"))
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

                //Step-32
                //Continue scrolling to the image 10 in the series MR-Series 2 by using down triangle.

                for (int i = 0; i < 4; i++)
                {
                    viewer.ClickDownArrowbutton(1, 1);
                }

                //In the base viewer MR-Series 3, the image 20 is displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status32 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status32 && viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("20"))
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



                //Step-33
                //Drag the vertical scrollbar position to the image 16 in MR-Series 2

                //viewer.DragScroll(1, 1, 15, 23);

                IWebElement source = viewer.ViewportScrollHandle(1, 1);
                IWebElement destination = viewer.ViewportScrollBar(1, 1);

                int w = destination.Size.Width;
                int h = destination.Size.Height;

                Actions action33 = new Actions(BasePage.Driver);


                action33.ClickAndHold(source).MoveToElement(destination, w / 2, h * 15 / 23).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);


                //In the base viewer MR-Series 3, the image 34 is displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status33 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status33 && viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("34"))
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

                //Step-34
                //Use the mouse's middle wheel, scroll to the image 21 in the series MR-Series 2.
                //In the base viewer MR-Series 3, the image 45 is displayed.

                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step-35
                //Select MR-Series 3
                viewer.SeriesViewer_1X2().Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //MR-Series 3 becomes a reference viewer (with orange window).
                //MR-Series 2 becomes a base viewer.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status35 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status35 &&
                    viewer.SeriesViewer_1X2().GetAttribute("class").Contains("activeSeriesViewer") &&
                    viewer.SeriesViewer_1X2().GetCssValue("border-top-color").Equals(rgbavalue))
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


                //Step-36
                //Use the up triangle, scroll to the image 21. (example SliceLocation -96.50520615877)

                for (int i = 0; i < 13; i++)
                {
                    viewer.ClickUpArrowbutton(1, 2);
                }

                //In the base viewer MR-Series 2, the image 10 is displayed.
                //(example SliceLocation -94.655209881448)

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status36 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status36 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("10"))
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

                //Step-37
                //Use the up triangle, continue scrolling to the image 18.

                for (int i = 0; i < 3; i++)
                {
                    viewer.ClickUpArrowbutton(1, 2);
                }

                //In the base viewer MR-Series 2, the image 9 is displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status37 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status37 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("9"))
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

                //Step-38
                //Drag the vertical scrollbar position to the image 9 in MR-Series 3

                source = viewer.ViewportScrollHandle(1, 2);
                destination = viewer.ViewportScrollBar(1, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                Actions action38 = new Actions(BasePage.Driver);

                //--9/__ have to give correct value
                action38.ClickAndHold(source).MoveToElement(destination, w / 2, h * 8 / 48).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);


                //In the base viewer MR-Series 2, the image 5 is displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status38 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status38 && viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("5"))
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

                //Step-39
                //Use the mouse's middle wheel, scroll to the image 24 in the series MR-Series 3.

                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step-40
                //Change the layout of the selected viewport to 1x2. 
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x2);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAttributeInViewport(1, 2, "src", "layoutFormat=1x2");

                //Layout 1x2 is displayed in the reference viewer MR-Series 3.
                //Note-The link icon is displayed in the right top corner of the window & is not displayed on the individual images.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status40 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status40 && viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(), "src", '&', "layoutFormat").Equals("1x2"))
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

                //Step-41
                //Select a middle position on the vertical scrollbar in the series MR-Series 3.
                //Example - Series 3 Image 23 and Image 24.

                source = viewer.ViewportScrollHandle(1, 2);
                destination = viewer.ViewportScrollBar(1, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                Actions action41 = new Actions(BasePage.Driver);

                //--23/48__ have to give correct value
                action41.ClickAndHold(source).MoveToElement(destination, w / 2, h * 22 / 48).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                //In the base viewer MR-Series 2, the image 11 is displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status41 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status41 && viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("11"))
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

                //Step-42

                //Select a bottom end point on the vertical scrollbar in the series MR-Series 3.
                //Example - Series 3 Image 47 and 48.

                source = viewer.ViewportScrollHandle(1, 2);
                destination = viewer.ViewportScrollBar(1, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                Actions action42 = new Actions(BasePage.Driver);

                //--23/48__ have to give correct value
                action42.ClickAndHold(source).MoveToElement(destination, w / 2, h * 46 / 48).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //In the base viewer MR-Series 2, the image 22 is displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status42 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                //Series-2 1x1 Image-13 not available.

                if (status42 && viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("21"))
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

                //Step-43
                //Change the layout of the selected viewport to 2x1.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x1);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //Layout 2x1 is displayed in the reference viewer MR-Series 3.
                //Note-The link icon is displayed in the right top corner of the window & is not displayed on the individual images.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status43 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status43 && viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(), "src", '&', "layoutFormat").Equals("2x1"))
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

                //Step-44
                //Select a middle position on the vertical scrollbar in the series MR-Series 3.   
                //Example - Series 3 Image 23 and Image 24.

                source = viewer.ViewportScrollHandle(1, 2);
                destination = viewer.ViewportScrollBar(1, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                Actions action44 = new Actions(BasePage.Driver);

                //--23/48__ have to give correct value
                action44.ClickAndHold(source).MoveToElement(destination, w / 2, h * 22 / 48).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                //In the base viewer MR-Series 2, the image 11 is displayed.


                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status44 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status44 && viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("11"))
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

                //Step-45
                //Select a position 1/4 from the bottom end point on the vertical scrollbar in the series MR-Series 3.
                //Example - Series 3 Image 35 and Image 36.

                source = viewer.ViewportScrollHandle(1, 2);
                destination = viewer.ViewportScrollBar(1, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                Actions action45 = new Actions(BasePage.Driver);

                action45.ClickAndHold(source).MoveToElement(destination, w / 2, h * 34 / 48).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                //In the base viewer MR-Series 2, the image 17 is displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status45 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                //image-13 not available in series-2
                if (status45 && viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("16"))
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

                //Step-46
                //Change the layout of the selected viewport to 2x2.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //Layout 2x2 is displayed in the reference viewer MR-Series 3.
                //Note-The link icon is displayed in the right top corner of the window & is not displayed on the individual images.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status46 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status46 && viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(), "src", '&', "layoutFormat").Equals("2x2"))
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


                //Step-47
                //Use the mouse's middle wheel, scroll to the image 17, Image 18, Image 19 
                //and Image 20 in the series MR-Series 3.


                //In the base viewer MR-Series 2, the image 9 is displayed.

                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-48
                //Use the mouse's middle wheel, scroll to the image 41, Image 42, Image 43 
                //and Image 44 in the series MR-Series 3.

                //In the base viewer MR-Series 2, the image 19 is displayed.

                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-49
                //Change the layout of the selected viewport to 3x3. Scroll through the selected series.

                //Layout 3x3 is displayed in the reference viewer MR-Series 3.
                //Note - The link icon is displayed in the right top corner of the window 
                //and is not displayed on the individual images.

                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-50
                //Use the mouse's middle wheel, scroll to the image 10, Image 11, Image 12,..
                //and Image 18 in the series MR-Series 3.


                //In the base viewer MR-Series 2, the image 6 is displayed.

                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-51
                //Change the layout of the selected viewport to 4x4. Scroll through the selected series.

                //Layout 4x4 is displayed in the reference viewer MR-Series 3.
                //At this point all the image numbers are very hard too see due to the fonts are overlapping.
                //Ensure that the images are responding to scrolling up/down command.

                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-52
                //Change the layout back to 1x1.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x1);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //The image is displayed in the 1x1 layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status52 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status52 && viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(), "src", '&', "layoutFormat").Equals("1x1"))
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

                //Step-53
                //Hover the mouse cursor to the Linked Scrolling button from the menu.

                linkselected = viewer.GetReviewTool("Link Selected");
                viewer.JSMouseHover(linkselected);

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //A drop down menu is displayed.
                //Link All, Link All Offset, Unlink, Link Selected, Link Selected Offset

                IList<IWebElement> dropdown53 = BasePage.Driver.FindElements(By.CssSelector("li[title='Link Selected'] ul>li"));
                title.Clear();
                bool flag53 = true;
                foreach (IWebElement dropdowntool in dropdown53)
                {
                    title.Add(dropdowntool.GetAttribute("title"));
                    if (!(dropdowntool.Enabled && dropdowntool.Displayed))
                    {
                        flag53 = false;
                        break;
                    }
                }
                if (flag53 && dropdown53.Count == 5 && title.SequenceEqual(link_5))
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

                //Step-54
                //Select Unlink to unlink all
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Unlink);
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);
                viewer.switchToUserHomeFrame();



                //The Series are unlinked. Link icon is no longer displayed on the series.
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status54 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status54 && viewer.LinkScrollingStatusImageList().Count == 0)
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

                //Step-55
                //[ICA-10987] 1. From Studies tab load the following study in HTML-4 viewer - Patient Name (TBD) 
                //2. Set series layout to 1x2 and load series #5 on the right and series #6 on the left. 
                //3. Link the series using linked scrolling tool. 
                //4.Use mouse scroll to scroll down rapidly and scroll up to the top in the same speed.

                //sync up for IE-9 browser--
                if (BasePage.SBrowserName.ToLower().Equals("internet explorer") && BasePage.BrowserVersion.ToLower().Equals("91"))
                {
                    study.CloseStudy();
                    domain = (DomainManagement)login.Navigate("DomainManagement");
                    study = (Studies)login.Navigate("Studies");
                    study.SearchStudy(AccessionNo: AccessionID, Datasource: EA_91);
                    study.SelectStudy("Accession", AccessionID);
                    viewer = StudyViewer.LaunchStudy();

                    viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x2);
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                }

                var action55 = new Actions(BasePage.Driver);

                action55.DragAndDrop(viewer.Thumbnails()[3], viewer.SeriesViewer_1X1()).Build().Perform();
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                action55 = new Actions(BasePage.Driver);

                action55.DragAndDrop(viewer.Thumbnails()[4], viewer.SeriesViewer_1X2()).Build().Perform();
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);



                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkSelected);
                viewer.SelectLinkedCheckBox(1, 1);
                viewer.SelectLinkedCheckBox(1, 2);
                viewer.LinkedScrollingCheckBtn().Click();

                source = viewer.ViewportScrollHandle(1, 1);
                destination = viewer.DownArrowBtn(1, 1);

                //destination = viewer.ViewportScrollBar(1, 1);
                //w = viewer.ViewportScrollBar(1, 1).Size.Width;
                //h = viewer.ViewportScrollBar(1, 1).Size.Height;

                var action55_0 = new Actions(BasePage.Driver);

                action55_0 = new Actions(BasePage.Driver);

                //action55_0.ClickAndHold(source).MoveToElement(destination, w / 2, h).Release().Build().Perform();
                action55_0.ClickAndHold(source).MoveToElement(destination).Release().Build().Perform();
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //action55_0.ClickAndHold(source).MoveToElement(destination, w / 2, h).Release().Build().Perform();
                action55_0.ClickAndHold(source).MoveToElement(destination).Release().Build().Perform();
                Thread.Sleep(5000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForAttributeInViewport(1, 1, "imagenum", "19");
                PageLoadWait.WaitForAttributeInViewport(1, 2, "imagenum", "19");

                bool setp_55_1 = viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("19");
                bool setp_55_2 = viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("19");

                source = viewer.ViewportScrollHandle(1, 1);
                //destination = viewer.ViewportScrollBar(1, 1);
                destination = viewer.UpArrowBtn(1, 1);

                var action55_1 = new Actions(BasePage.Driver);

                //action55_1.ClickAndHold(source).MoveToElement(destination, w / 2, 1).Release().Build().Perform();
                action55_1.ClickAndHold(source).MoveToElement(destination).Release().Build().Perform();
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //action55_1.ClickAndHold(source).MoveToElement(destination, w / 2, 1).Release().Build().Perform();
                action55_1.ClickAndHold(source).MoveToElement(destination).Release().Build().Perform();
                Thread.Sleep(5000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForAttributeInViewport(1, 1, "imagenum", "1");
                PageLoadWait.WaitForAttributeInViewport(1, 2, "imagenum", "1");

                //Both viewers should display image #1
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status55 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status55 && setp_55_1 && setp_55_2 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("1") &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("1"))
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
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //Logout  -Step-56
                login.Logout();
                ExecutedSteps++;

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
        /// This Test Case is Verification of "Linking Series - Numerical linking"
        /// </summary>

        public TestCaseResult Test_27989(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables

            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

                //step-1: initial setup
                //Link All in service tool 
                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                Taskbar bar = new Taskbar();
                bar.Hide();

                st.LaunchServiceTool();
                st.NavigateToTab("Linked Scrolling");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);

                if (wpfobject.IsCheckBoxSelected("CB_LinkAll") == false)
                {
                    wpfobject.SelectCheckBox("CB_LinkAll");
                    Logger.Instance.InfoLog("Link All check box is selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Link All check box is already selected");
                }

                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("6");
                wpfobject.WaitTillLoad();
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();

                bar.Show();

                ExecutedSteps++;

                //step-2:
                //Login as system Administrator and Enable Link All domain management page
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Enable Link All
                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();

                // Link All check box
                if (domain.LinkAllCheckbox().Selected == false)
                {
                    domain.LinkAllCheckbox().Click();
                    Logger.Instance.InfoLog("Link scrolling Link All Check box is de-selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Link scrolling Link All Check box is already de-selected");
                }
                domain.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Launch study
                Studies studies = (Studies)login.Navigate("Studies");
                //Accession: U-ID179490
                studies.SearchStudy(AccessionNo: AccessionID, Datasource: EA_91);
                studies.SelectStudy("Accession", AccessionID);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                ExecutedSteps++;


                //

                //Step-3
                //Set the view to 4 series.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                int count = viewer.SeriesViewPorts().Count();

                //4 viewers are displayed side by side.
                if (count == 4)
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

                //step-4: 
                //Load related series (MR-Series 4 and MR-Series 5) into the viewers.
                Actions action = new Actions(BasePage.Driver);

                action.DragAndDrop(viewer.Thumbnails()[2], viewer.SeriesViewer_2X1()).Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(25);
                action.DragAndDrop(viewer.Thumbnails()[3], viewer.SeriesViewer_2X2()).Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(25);

                //The MR-Series 4 is displayed bottom left and the MR-Series 5 is displayed bottom right

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status4 && viewer.SeriesViewPorts().Count == 4)
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


                //Step-5
                //Hover the mouse cursor to the Linked Scrolling button from the menu.

                IWebElement linkselected = viewer.GetReviewTool("Link Selected");
                studies.JSMouseHover(linkselected);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                IList<IWebElement> dropdown = BasePage.Driver.FindElements(By.CssSelector("li[title='Link Selected'] ul>li"));
                IList<String> title = new List<String>();

                //A drop down menu is displayed. Link All, Link All Offset, Unlink, Link Selected, Link Selected Offset

                bool flag = true;

                foreach (IWebElement dropdowntool in dropdown)
                {
                    title.Add(dropdowntool.GetAttribute("title"));
                    if (!(dropdowntool.Enabled && dropdowntool.Displayed))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag && dropdown.Count == 5 && title.SequenceEqual(link_5))
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
                //Click reset to remove mousehover
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);

                //Step-6: Select Link All.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkAll);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //All series are numerically linked to the reference viewer (MR-Series 5).
                //The numerical linking icon is displayed on the series windows.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status6 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status6 && viewer.LinkScrollingStatusImageList().Count == 4)
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


                //Step-7
                //Drag the vertical scrollbar position to the image 9 in MR-Series 5  

                //----------viewer.DragScroll(2, 1, 8, 19);

                IWebElement source = viewer.ViewportScrollHandle(2, 2);
                IWebElement destination = viewer.ViewportScrollBar(2, 2);

                int w = destination.Size.Width;
                int h = destination.Size.Height;
                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 8 / 19).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 8 / 19).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                //In the base viewers- MR-Series 2, MR-Series 3, MR-Series 4, the image 9 is displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status7 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status7 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("9") &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("9") &&
                    viewer.SeriesViewer_2X1().GetAttribute("imagenum").Equals("9"))
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



                //Step-8
                //Use the mouse's middle wheel, scroll to the image 3 in the series MR-Series 5
                //In the base viewers- MR-Series 2, MR-Series 3, MR-Series 4, the image 3 is displayed.

                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-9
                //Change the layout of the selected viewport to 1x2.
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x2);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //Layout 1x2 is displayed in the reference viewer MR-Series 5.
                //Note-The numerical link icon is displayed in the right top corner of the window and is not displayed on the individual images.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status9 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_2X2());

                if (status9 && viewer.GetInnerAttribute(viewer.SeriesViewer_2X2(), "src", '&', "layoutFormat").Equals("1x2"))
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

                //Step-10
                //Select a middle position on the vertical scrollbar in the series MR-Series 5.
                //Example - Series 5 Image 9 and Image 10.

                //--viewer.DragScroll(2, 2, 12, 19);

                source = viewer.ViewportScrollHandle(2, 2);
                destination = viewer.ViewportScrollBar(2, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 8 / 19).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status10 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                //In the base viewers- MR-Series 2, MR-Series 3, MR-Series 4, the image 9 is displayed.

                if (status10 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("9") &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("9"))
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

                //Step-11
                //Select a bottom end point on the vertical scrollbar in the series MR-Series 5.
                //Example - Series 5 Image 18 and 19.

                //viewer.DragScroll(2, 2, 17, 19);

                source = viewer.ViewportScrollHandle(2, 2);
                destination = viewer.ViewportScrollBar(2, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 17 / 19).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                int loop = 0;
                while (loop < 2)
                {
                    if (Int32.Parse(viewer.SeriesViewer_2X2().GetAttribute("imagenum")) == 18)
                        break;
                    else if (Int32.Parse(viewer.SeriesViewer_2X2().GetAttribute("imagenum")) < 18)
                        viewer.ClickDownArrowbutton(2, 2);
                    else
                        viewer.ClickUpArrowbutton(2, 2);

                    loop++;
                }

                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //In the base viewers- MR-Series 2, MR-Series 3, MR-Series 4, the image 18 is displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status11 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                //Serier-2 image-13 not available
                //Series-4 inage-9 not available
                if (status11 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("17") &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("18") &&
                    viewer.SeriesViewer_2X1().GetAttribute("imagenum").Equals("17"))
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

                //Step-12
                //Change the layout of the selected viewport to 2x1.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x1);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status12 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status12 &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_2X2(), "src", '&', "layoutFormat").Equals("2x1"))
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

                //Step-13
                //Select a middle position on the vertical scrollbar in the series MR-Series 5.
                //Example - Series 5 Image 11 and Image 12.

                //viewer.DragScroll(2, 2, 8, 19);

                source = viewer.ViewportScrollHandle(2, 2);
                destination = viewer.ViewportScrollBar(2, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 10 / 19).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);


                //In the base viewers- MR-Series 2, MR-Series 3,  MR-Series 4, the image 11 is displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status13 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                //Serier-2 image-13 not available
                //Series-4 inage-9 not available

                if (status13 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("11") &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("11") &&
                    viewer.SeriesViewer_2X1().GetAttribute("imagenum").Equals("10"))
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

                //step-14:
                //Select a position 1/4 from the bottom end point on the vertical scrollbar in the series MR-Series 5.
                //Example - Series 5 Image 16 and Image 17.


                source = viewer.ViewportScrollHandle(2, 2);
                destination = viewer.ViewportScrollBar(2, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 15 / 19).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //In the base viewers- MR-Series 2, MR-Series 3, MR-Series 4, the image 16 is displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status14 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                //Serier-2 image-13 not available
                //Series-4 inage-9 not available
                if (status14 &&
                     viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("15") &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("16") &&
                    viewer.SeriesViewer_2X1().GetAttribute("imagenum").Equals("15"))
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


                //Step-15 
                //Change the layout of the selected viewport to 2x2.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //Layout 2x2 is displayed in the reference viewer MR-Series 5.
                //Note: The numerical link icon is displayed in the right top corner of the window and is not displayed on the individual images.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status15 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status15 &&
                   viewer.GetInnerAttribute(viewer.SeriesViewer_2X2(), "src", '&', "layoutFormat").Equals("2x2"))
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

                //Step-16
                //Scroll up/down the selected series by using up/down triangle.

                //Scroll up

                viewer.ClickUpArrowbutton(2, 2);

                //At this point all the image numbers are very hard too see due to the fonts are overlapping.
                //Ensure that the images are responding to scrolling up/down command.

                result.steps[++ExecutedSteps].SetPath(testid + "_16_1_up", ExecutedSteps + 1);
                bool status16_1 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                //Scroll down
                viewer.ClickDownArrowbutton(2, 2);

                result.steps[ExecutedSteps].SetPath(testid + "_16_2_down", ExecutedSteps + 1);
                bool status16_2 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status16_1 && status16_2)
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


                //Step-17
                //Scroll up/down the selected series by dragging the vertical scrollbar position.

                //Scroll up
                source = viewer.ViewportScrollHandle(2, 2);
                destination = viewer.ViewportScrollBar(2, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h / 2).Release().Build().Perform();
                Thread.Sleep(5000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                action.ClickAndHold(source).MoveToElement(destination, w / 2, h / 2).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);


                //At this point all the image numbers are very hard too see due to the fonts are overlapping.
                //Ensure that the images are responding to scrolling up/down command.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status17 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status17)
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


                //Step-18
                //Move up/down the selected series by using mouse's middle wheel.
                result.steps[++ExecutedSteps].status = "Not Automated";


                //Step-19
                //Move up/down the selected series by selecting different position on the vertical scrollbar

                //Move down

                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, 3 * h / 4).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //At this point all the image numbers are very hard too see due to the fonts are overlapping.
                //Ensure that the images are responding to scrolling up/down command.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status19 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status19)
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


                //step-20:
                //Change the layout of the selected viewport to 3x3.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout3x3);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //Layout 3x3 is displayed in the reference viewer MR-Series 5.
                //link icon is displayed in the right top corner of the window & is not displayed on the individual images

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status20 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status20 &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_2X2(), "src", '&', "layoutFormat").Equals("3x3"))
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


                //Step-21
                //Move up/down the selected series by using mouse's middle wheel.
                result.steps[++ExecutedSteps].status = "Not Automated";



                //Step-22
                //Change the layout of the selected viewport to 4x4

                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout4x4);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //Layout 4x4 is displayed in the reference viewer MR-Series 5.
                //link icon is displayed in the right top corner of the window and is not displayed on the individual images.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status22 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status22 && viewer.GetInnerAttribute(viewer.SeriesViewer_2X2(), "src", '&', "layoutFormat").Equals("4x4"))
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


                //Step-23:Change the layout back to 1x2.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x2);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Layout 1x2 is displayed in the reference viewer MR-Series 5.
                //link icon is displayed in the right top corner of the window and is not displayed on the individual images.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status23 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (status23 && viewer.GetInnerAttribute(viewer.SeriesViewer_2X2(), "src", '&', "layoutFormat").Equals("1x2"))
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


                //Step-24
                //Change the reference window by selecting a different series window. (example - MR-Series 3)

                viewer.SeriesViewer_1X2().Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //All series are numerically linked to the new reference viewer.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status24 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if (status24 &&
                    viewer.SeriesViewer_1X2().GetAttribute("class").Contains("activeSeriesViewer") &&
                    viewer.SeriesViewer_1X2().GetCssValue("border-top-color").Equals(rgbavalue))
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

                //Step-25 
                //Change the layout to 2x1.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x1);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Layout 2x1 is displayed in the reference viewer MR-Series 3.
                //link icon is displayed in the right top corner of the window and is not displayed on the individual images.


                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status25 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status25 && viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(), "src", '&', "layoutFormat").Equals("2x1"))
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

                //Step-26 
                //Select a middle position on the vertical scrollbar in the series MR-Series 3.
                //Example - Series 3 Image 23 and Image 24.

                source = viewer.ViewportScrollHandle(1, 2);
                destination = viewer.ViewportScrollBar(1, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 22 / 48).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 22 / 48).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //In the base viewers- MR-Series 2, the numerical linked indicator is displayed on top of the image 23.
                //MR-Series 4, and MR-Series 5, the out of range linked indicator is displayed on top of the image 19.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status26 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                //Serier-2 image-13 not available
                //Series-4 inage-9 not available

                if (status26 &&
                     viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("22") &&
                     viewer.SeriesViewer_2X1().GetAttribute("imagenum").Equals("18") &&
                     viewer.SeriesViewer_2X2().GetAttribute("imagenum").Equals("19"))
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

                //step-27
                //Select a bottom end point on the vertical scrollbar in the series MR-Series 3.
                //Example - Series 3 Image 47 and 48.


                source = viewer.ViewportScrollHandle(1, 2);
                destination = viewer.ViewportScrollBar(1, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 42 / 48).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);


                //In the base viewers- MR-Series 2, the out of range linked indicator is displayed on top of the image 23.
                //MR-Series 4 and  MR-Series 5, the out of range linked indicator is displayed on top of the image 19.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status27 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                //Serier-2 image-13 not available
                //Series-4 inage-9 not available
                //failing**
                if (status27 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("22") &&
                     viewer.SeriesViewer_2X1().GetAttribute("imagenum").Equals("18") &&
                     viewer.SeriesViewer_2X2().GetAttribute("imagenum").Equals("19"))
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


                //Step-28
                //Select Linked Scrolling button and then Link Selected

                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkSelected);
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);

                //Four small windows with link icons are displayed.

                if (viewer.LinkSelectTableCheckBoxList().Count == 4)
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


                //Step-29
                //Select square 1 and square 3 and then green check mark.
                viewer.SelectLinkedCheckBox(1, 1);
                viewer.SelectLinkedCheckBox(2, 1);
                viewer.LinkedScrollingCheckBtn().Click();

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status29 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                //MR-Series 2 and MR-Series 4 are unlinked.Link icon is no longer displayed on the right top corner of the series 2 & 4.
                //MR-Series 3 and MR-Series 5 are linked.   A link icon is displayed on the right top corner of the series.

                if (status29 && viewer.LinkScrollingStatusImageList().Count == 2)
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

                //step-30
                //Select a middle position on the vertical scrollbar in the series MR-Series 3.
                //Example - Series 3 Image 23 and Image 24.

                source = viewer.ViewportScrollHandle(1, 2);
                destination = viewer.ViewportScrollBar(1, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 21 / 48).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //In the base viewers- MR-Series 2, MR-Series 4 do not change.
                //MR-Series 5, the out of range link indicator is displayed on top of the image 19.


                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status30 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status30 && viewer.LinkScrollingStatusImageList().Count == 2)
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

                //Step-31
                //Select a bottom end point on the vertical scrollbar in the series MR-Series 3.
                //Example - Series 3 Image 47 and 48.

                source = viewer.ViewportScrollHandle(1, 2);
                destination = viewer.ViewportScrollBar(1, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 42 / 48).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //In the base viewers - MR-Series 2, MR-Series 4 do not change.
                //MR-Series 5, the out of range link indicator is displayed on top of the image 19.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status31 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status31 && viewer.LinkScrollingStatusImageList().Count == 2)
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


                //Step-32
                //Select Linked Scrolling button and then Link Selected

                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkSelected);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //A window with four black square windows is displayed. Square 1 and Square 3 show empty window
                // and Square 2 and Square 4 show link icon

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status32 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status32 && viewer.LinkSelectTableCheckBoxList().Count == 4)
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

                //Step-33
                //Select one of the previous unlined Series and select green check mark.
                //(example - MR-Series 2 - window 1)


                viewer.SelectLinkedCheckBox(1, 1);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                viewer.LinkedScrollingCheckBtn().Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //The Series is linked with the reference window. The link icon is displayed on the series.
                //MR-Series 2, MR-Series 3 & MR-Series 5 are linked with each other. MR-Series 4 is unlinked.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status33 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status33 && viewer.LinkScrollingStatusImageList().Count == 3)
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

                //Step-34
                //Select a middle position on the vertical scrollbar in the series MR-Series 3.
                //Example - Series 3 Image 23 and Image 24.

                source = viewer.ViewportScrollHandle(1, 2);
                destination = viewer.ViewportScrollBar(1, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 22 / 48).Release().Build().Perform();
                Thread.Sleep(5000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 22 / 48).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //In the base viewers - MR-Series 2, the image 23 is displayed, MR-Series 4 does not change.
                //MR-Series 5, the out of range link indicator is displayed on top of the image 19.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status34 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status34 && viewer.LinkScrollingStatusImageList().Count == 3)
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


                //Step-35
                //Set the view to 6 series.
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);

                //Six viewers are displayed side by side.

                if (viewer.SeriesViewPorts().Count() == 6)
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


                //Step-36
                //Load the related series MR-Series 6 into the viewers.

                //--Series -6 loaded-- action not needed.

                Thread.Sleep(3000);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x2);
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForPageLoad(20);
                source = viewer.ViewportScrollHandle(1, 2);
                destination = viewer.ViewportScrollBar(1, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 22 / 48).Release().Build().Perform();
                Thread.Sleep(5000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 22 / 48).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                viewer.ClickUpArrowbutton(1, 2);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                viewer.ClickDownArrowbutton(1, 2);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                PageLoadWait.WaitForAllViewportsToLoad(60);
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //The 5 series are displayed and one window is empty.
                //MR-Series 2, MR-Series 3 and MR-Series 5 are linked with each other.
                //MR-Series 4 and MR-Series 6 are unlinked.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status36 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());


                if (status36 &&
                    viewer.NonEmptyViewPorts().Count == 5 &&
                    viewer.EmptyViewPorts().Count == 1)
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



                //Step-37
                //Change reference viewer by selecting MR-Series 6.

                viewer.SeriesViewer_2X2().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //IWebElement referenceViewer = BasePage.Driver.FindElement(By.CssSelector("div[id='viewerImgDiv'] img[id$='_SeriesViewer_2_2_viewerImg'][class$='activeSeriesViewer']"));

                if (viewer.SeriesViewer_2X2().GetAttribute("class").Contains("activeSeriesViewer") &&
                    viewer.SeriesViewer_2X2().GetCssValue("border-top-color").Equals(rgbavalue))
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

                //Step-38
                //Scroll through the selected series.

                for (int i = 0; i < 3; i++)
                {
                    viewer.ClickDownArrowbutton(2, 2);
                }

                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //As the series is scrolled, the image number of the other series are not updated because MR-Series 6 is not linked.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status38 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status38)
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


                //Step-39
                //Select the empty window as a reference viewer.

                viewer.SeriesViewer_2X3().Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //IWebElement referenceViewer1 = BasePage.Driver.FindElement(By.CssSelector("div[id='viewerImgDiv'] img[id$='_SeriesViewer_2_3_viewerImg'][class$='activeSeriesViewer']"));
                //if (referenceViewer1 != null)

                if (viewer.SeriesViewer_2X3().GetAttribute("class").Contains("activeSeriesViewer") &&
                 viewer.SeriesViewer_2X3().GetCssValue("border-top-color").Equals(rgbavalue))
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


                //step-40:Scroll up and down.

                //Can't Scroll up/down (up/down triangle is removed for Empty viewport)
                //So verifying Up arrow downarrow not displayed for emptyport
                IWebElement DownArrow = viewer.DownArrowBtn(2, 3);
                IWebElement UpArrow = viewer.UpArrowBtn(2, 3);

                if (DownArrow.Displayed == false && UpArrow.Displayed == false)
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

                //step-41:
                //Select Linked Scrolling button and then Link Selected

                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkSelected);
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);

                //A window with six black square windows is displayed.
                //Square 3, 5 and Square 6 show empty window. Square 1, 2, and 4 show link icon.

                if (viewer.LinkSelectTableCheckBox(1, 1).GetAttribute("class").Contains("SelectedLinkSeries") &&
                    viewer.LinkSelectTableCheckBox(1, 2).GetAttribute("class").Contains("SelectedLinkSeries") &&
                    !viewer.LinkSelectTableCheckBox(1, 3).GetAttribute("class").Contains("SelectedLinkSeries") &&
                    viewer.LinkSelectTableCheckBox(2, 1).GetAttribute("class").Contains("SelectedLinkSeries") &&
                    !viewer.LinkSelectTableCheckBox(2, 2).GetAttribute("class").Contains("SelectedLinkSeries") &&
                    !viewer.LinkSelectTableCheckBox(2, 3).GetAttribute("class").Contains("SelectedLinkSeries"))
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

                //Step-42
                //Select one of the previous unlined Series and select green check mark.
                //(example - MR-Series 4 - window 3)

                viewer.SelectLinkedCheckBox(1, 3);
                viewer.LinkedScrollingCheckBtn().Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                //The Series is linked with the reference window.  The link icon is displayed on the series.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status42 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status42 && viewer.LinkScrollingStatusImageList().Count == 4)
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

                //step-43:
                //Select MR-Series 3 as a reference viewer.

                viewer.SeriesViewer_1X2().Click();
                //IWebElement referenceViewer2 = BasePage.Driver.FindElement(By.CssSelector("div[id='viewerImgDiv'] img[id$='_SeriesViewer_1_3_viewerImg'][class$='activeSeriesViewer']"));
                //if (referenceViewer2 != null)


                if (viewer.SeriesViewer_1X2().GetAttribute("class").Contains("activeSeriesViewer") &&
                  viewer.SeriesViewer_1X2().GetCssValue("border-top-color").Equals(rgbavalue))
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


                //step-44:
                //Select a position near to the top of the vertical scrollbar in the series MR-Series 3.
                //Example - Series 3 Image 11 and Image 12.

                source = viewer.ViewportScrollHandle(1, 2);
                destination = viewer.ViewportScrollBar(1, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 10 / 48).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(25);
                PageLoadWait.WaitForFrameLoad(25);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 10 / 48 + 2).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForPageLoad(25);
                PageLoadWait.WaitForFrameLoad(25);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                Thread.Sleep(5000);
                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 10 / 48 + 2).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForPageLoad(25);
                PageLoadWait.WaitForFrameLoad(25);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //In the base viewers- MR-Series 2 & MR-Series 4 the image 11 is displayed.
                //MR-Series 5, the image 11 and 12 are displayed.
                //MR-Series 6, does not change since it is not linked.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status44 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                //Serier-2 image-13 not available
                //Series-4 inage-9 not available

                if (status44 &&
                     viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("11") &&
                     viewer.SeriesViewer_1X3().GetAttribute("imagenum").Equals("10") &&
                     viewer.SeriesViewer_2X1().GetAttribute("imagenum").Equals("11"))
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

                //Step-45
                //Select Link All

                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkAll);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForPageLoad(25);
                PageLoadWait.WaitForFrameLoad(25);
                PageLoadWait.WaitForAllViewportsToLoad(30);


                //All series are numerically linked to the reference viewer MR-Series 3.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status45 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status45 && viewer.LinkScrollingStatusImageList().Count == 5)
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

                //Step-46
                //Scroll to the image 15, image 16 in the series MR-Series 3 by using up/down triangle.
                //(example SliceLocation - -84.605206381294)

                for (int i = 0; i < 4; i++)
                {
                    viewer.ClickDownArrowbutton(1, 2);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //In the base viewers- MR-Series 2, MR-Series 4, the image 15 is displayed.
                //MR-Series 5, the image 15, 16 are displayed.
                //MR-Series 6, the image 15 is displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status46 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                //Serier-2 image-13 not available
                //Series-4 inage-9 not available

                if (status46 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("14") &&
                    viewer.SeriesViewer_1X3().GetAttribute("imagenum").Equals("14") &&
                    viewer.SeriesViewer_2X1().GetAttribute("imagenum").Equals("15") &&
                    viewer.SeriesViewer_2X2().GetAttribute("imagenum").Equals("15"))
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


                //Step-47
                //Change the layout of the selected viewport to 1x2.Scroll through the selected series.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x2);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                for (int i = 0; i < 3; i++)
                {
                    viewer.ClickDownArrowbutton(1, 2);
                }
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //As the series is scrolled, the image number of the base viewer is updated
                // to match the number of the image of the top left corner window on the reference viewer.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status47 = studies.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (status47 &&
                   viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(), "src", '&', "layoutFormat").Equals("1x2"))
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

                //step-48
                //Change the layout of the selected viewport to 2x2.Scroll through the selected series.
                viewer.switchToUserHomeFrame();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                for (int i = 0; i < 3; i++)
                {
                    viewer.ClickUpArrowbutton(1, 2);
                }
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //As the series is scrolled, the image number of the base viewer is updated
                // to match the number of the image of the top left corner window on the reference viewer.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status48 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status48 &&
                     viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(), "src", '&', "layoutFormat").Equals("2x2"))
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


                //step-49
                //Change the layout of the selected viewport to 3x3.Scroll through the selected series.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout3x3);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                viewer.switchToUserHomeFrame();


                for (int i = 0; i < 3; i++)
                {
                    viewer.ClickDownArrowbutton(1, 2);
                    PageLoadWait.WaitForPageLoad(15);
                    PageLoadWait.WaitForFrameLoad(15);
                }
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //As the series is scrolled, the image number of the base viewer is updated
                // to match the number of the image of the top left corner window on the reference viewer.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status49 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status49 &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(), "src", '&', "layoutFormat").Equals("3x3"))
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


                //step-50
                //Change the layout of the selected viewport to 4x4.Scroll through the selected series.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout4x4);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                viewer.switchToUserHomeFrame();

                for (int i = 0; i < 3; i++)
                {
                    viewer.ClickUpArrowbutton(1, 2);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //As the series is scrolled, the image number of the base viewer is updated
                // to match the number of the image of the top left corner window on the reference viewer.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status50 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status50 &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(), "src", '&', "layoutFormat").Equals("4x4"))
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

                //Step-51
                //Change the layout back to 1x1.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x1);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //The image is displayed in the 1x1 layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status51 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status51 &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(), "src", '&', "layoutFormat").Equals("1x1"))
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



                //step-52:Select Unlink to unlink all

                viewer.SelectToolInToolBar(IEnum.ViewerTools.Unlink);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //All Series are unlinked.  Link icon is no longer displayed on any series.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status52 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status52)
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


                //step-53
                //Scroll through a selected series.

                for (int i = 0; i < 3; i++)
                {
                    viewer.ClickDownArrowbutton(1, 2);
                }

                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                //Other series are not updated.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status53 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                //--it will fail because of ICA-12531
                if (status53 &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("18"))
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Last Step-54
                login.Logout();
                ExecutedSteps++;

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
        /// This Test Case is Verification of "Localizer and Linked Series"
        /// </summary>

        public TestCaseResult Test_27990(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;
            String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
            try
            {

                //Step-1
                //Precondition completed

                //Enable Link All in service tool 
                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                Taskbar bar = new Taskbar();
                bar.Hide();

                st.LaunchServiceTool();
                st.NavigateToTab("Linked Scrolling");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);

                if (wpfobject.IsCheckBoxSelected("CB_LinkAll") == false)
                {
                    wpfobject.SelectCheckBox("CB_LinkAll");
                    Logger.Instance.InfoLog("Link All check box is selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Link All check box is already selected");
                }

                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("6");
                wpfobject.WaitTillLoad();
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();

                bar.Show();

                ExecutedSteps++;

                //Step-2

                //Login as system Administrator. And open the Study. Change 2x3 viewer.

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Enable Link All
                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();


                if (domain.LinkAllCheckbox().Selected == false)
                {
                    domain.LinkAllCheckbox().Click();
                    Logger.Instance.InfoLog("Link scrolling Link All Check box is selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Link scrolling Link All Check box is already selected");
                }
                domain.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                Studies study = (Studies)login.Navigate("Studies");

                //Accession: U-ID179490
                study.SearchStudy(AccessionNo: AccessionID, Datasource: EA_91);
                study.SelectStudy("Accession", AccessionID);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                if (viewer.Thumbnails().Count == 5 &&
                    viewer.SeriesViewPorts().Count == 6)
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



                //Step-3
                //Change all layouts to 1x1. Series 6 is selected. Select Localizer icon.

                viewer.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                viewer.UpArrowBtn(1, 1).Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x1);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                viewer.SeriesViewer_1X2().Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                viewer.UpArrowBtn(1, 2).Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x1);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                viewer.SeriesViewer_1X3().Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                viewer.UpArrowBtn(1, 3).Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x1);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                viewer.SeriesViewer_2X1().Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                viewer.UpArrowBtn(2, 1).Click();
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x1);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                viewer.SeriesViewer_2X2().Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                viewer.UpArrowBtn(2, 2).Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x1);
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);


                viewer.SeriesViewer_1X2().Click();
                PageLoadWait.WaitForPageLoad(25);
                PageLoadWait.WaitForFrameLoad(25);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);

                viewer.SeriesViewer_2X2().Click();
                PageLoadWait.WaitForPageLoad(25);
                PageLoadWait.WaitForFrameLoad(25);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);

                //Localizer lines are displayed in all of the series.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status3 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status3 &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "ToggleLocalizerOn").Equals("true") &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn").Equals("true") &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_1X3(), "src", '&', "ToggleLocalizerOn").Equals("true") &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_2X1(), "src", '&', "ToggleLocalizerOn").Equals("true"))
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

                //Step-4
                //Select MR-Series 3 as a reference viewer.
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                viewer.SeriesViewer_1X2().Click();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //The enclosed orange window is displayed around the window.

                if (viewer.SeriesViewer_1X2().GetAttribute("class").Contains("activeSeriesViewer") &&
                    viewer.SeriesViewer_1X2().GetCssValue("border-top-color").Equals(rgbavalue))
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

                //Step-5
                //Scroll through the series.

                viewer.ClickDownArrowbutton(1, 2);

                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(60);

                //As the series is scrolled, the image number is updated to match the location of 
                //the current image on the base viewer. Note that Series 2 does not update.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status5 = study.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (status5 && viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("2"))
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

                //Step-6
                //Select Link All.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkAll);
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //viewer.DownArrowBtn(1, 2).Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //All series are numerically linked to the reference viewer MR-Series 3.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status6 = study.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (status6 && viewer.LinkScrollingStatusImageList().Count == 5)
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

                //Step-7
                //Use the up/down triangle, continue scrolling to the image 19.



                for (int i = 0; i < 17; i++)
                {
                    viewer.ClickDownArrowbutton(1, 2);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                }
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //In the base viewer MR-Series 2, MR-Series 4, MR-Series 5 & MR-Series 6, the image 19 is displayed.
                //Localizer line shows 19 in series 4, 5, and 6.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status7 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                //Series-2 1x1 Image-13 not available.
                //Series-4 1x3 Image-9 not available.

                if (status7 &&
                    //viewer.SeriesViewer_2X1().GetAttribute("imagenum").Equals("19") &&
                    //viewer.SeriesViewer_2X2().GetAttribute("imagenum").Equals("19") &&
                    viewer.SeriesViewer_1X3().GetAttribute("src").Contains("ToggleLocalizerOn=true") &&
                    viewer.SeriesViewer_2X1().GetAttribute("src").Contains("ToggleLocalizerOn=true") &&
                    viewer.SeriesViewer_2X2().GetAttribute("src").Contains("ToggleLocalizerOn=true"))
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


                //Step-8
                //Use the up triangle, continue scrolling to the image 18.

                viewer.ClickUpArrowbutton(1, 2);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //In the base viewer MR-Series 2, MR-Series 4, MR-Series 5 & MR-Series 6, the image 18 is displayed.
                //Localizer line shows 18 in series 4, 5, and 6.


                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status8 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                //Series-2 1x1 Image-13 not available.
                //Series-4 1x3 Image-9 not available.

                if (status8 &&
                    viewer.SeriesViewer_1X3().GetAttribute("src").Contains("ToggleLocalizerOn=true") &&
                    viewer.SeriesViewer_2X1().GetAttribute("src").Contains("ToggleLocalizerOn=true") &&
                    viewer.SeriesViewer_2X2().GetAttribute("src").Contains("ToggleLocalizerOn=true"))
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

                //Step-9
                //Drag the vertical scrollbar position to the image 9 in MR-Series 3

                IWebElement source = viewer.ViewportScrollHandle(1, 2);
                IWebElement destination = viewer.ViewportScrollBar(1, 2);

                int w = destination.Size.Width;
                int h = destination.Size.Height;

                Actions action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 9 / 48).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                Thread.Sleep(5000);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 9 / 48).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //In the base viewer MR-Series 2, MR-Series 4, MR-Series 5 & MR-Series 6, the image 9 is displayed.
                //Localizer line shows 9 in series 4, 5, and 6.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status9 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                //Series-2 1x1 Image-13 not available.
                //Series-4 1x3 Image-9 not available.

                if (status9 &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_1X3(), "src", '&', "ToggleLocalizerOn").Equals("true") &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_2X1(), "src", '&', "ToggleLocalizerOn").Equals("true") &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_2X2(), "src", '&', "ToggleLocalizerOn").Equals("true"))
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


                //Step-10
                //Drag the vertical scrollbar position to the image 42 in MR-Series 3

                source = viewer.ViewportScrollHandle(1, 2);
                destination = viewer.ViewportScrollBar(1, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 38 / 48).Release().Build().Perform();
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 38 / 48 - 2).Release().Build().Perform();
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //In the base viewer MR-Series 2, MR-Series 4, MR-Series 5 
                //& MR-Series 6, the out of range linked indicator is displayed.   
                //Localizer line shows 42 in series 4, 5, and 6.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status10 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status10 &&
                    //viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("42") &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_1X3(), "src", '&', "ToggleLocalizerOn").Equals("true") &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_2X1(), "src", '&', "ToggleLocalizerOn").Equals("true") &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_2X2(), "src", '&', "ToggleLocalizerOn").Equals("true"))
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

                //Step-11
                //Drag the vertical scrollbar position to the end of the MR-Series 3

                source = viewer.ViewportScrollHandle(1, 2);
                destination = viewer.ViewportScrollBar(1, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //In the base viewer MR-Series 2, 4, 5, 6, the out of range linked indicator is displayed.
                //Localizer line shows 48 in series 4, 5, and 6.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status11 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status11 &&
                     viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("48") &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_1X3(), "src", '&', "ToggleLocalizerOn").Equals("true") &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_2X1(), "src", '&', "ToggleLocalizerOn").Equals("true") &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_2X2(), "src", '&', "ToggleLocalizerOn").Equals("true"))
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

                //Step-12
                //Select MR-Series 6 as a reference viewer.
                viewer.SeriesViewer_2X2().Click();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //The enclosed orange window is displayed around the MR-Series 6's window

                if (viewer.SeriesViewer_2X2().GetAttribute("class").Contains("activeSeriesViewer") &&
                    viewer.SeriesViewer_2X2().GetCssValue("border-top-color").Equals(rgbavalue))
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

                //Step-13
                //Drag the vertical scrollbar position to the top of the MR-Series 6

                source = viewer.ViewportScrollHandle(2, 2);
                destination = viewer.ViewportScrollBar(2, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, 1).Release().Build().Perform();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                action.ClickAndHold(source).MoveToElement(destination, w / 2, 1).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);


                //In the base viewer MR-Series 2, the image 1 is displayed. MR-Series 3, the image 1 is displayed.
                //MR-Series 4, the image 1 is displayed. MR-Series 5, the image 1 is displayed.
                //Localizer line shows 1 in all series.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status13 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status13 &&
                    viewer.SeriesViewer_1X1().GetAttribute("src").Contains("ToggleLocalizerOn=true") &&
                    viewer.SeriesViewer_1X2().GetAttribute("src").Contains("ToggleLocalizerOn=true") &&
                    viewer.SeriesViewer_1X3().GetAttribute("src").Contains("ToggleLocalizerOn=true") &&
                    viewer.SeriesViewer_2X1().GetAttribute("src").Contains("ToggleLocalizerOn=true"))
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


                //Step-14--***
                //Use the mouse's middle wheel, scroll to the image 19 in the series MR-Series 6.

                //source = viewer.ViewportScrollHandle(2, 2);
                //destination = viewer.ViewportScrollBar(2, 2);
                //w = destination.Size.Width;
                //h = destination.Size.Height;
                //action = new Actions(BasePage.Driver);
                //action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 19 / 19).Release().Build().Perform();
                //PageLoadWait.WaitForPageLoad(15);

                //result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                //bool status14 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                //In the base viewer MR-Series 2, the image 19 is displayed.
                //MR-Series 3, the image 19 is displayed. MR-Series 4, the image 19 is displayed. 
                //MR-Series 5, the image 19 is displayed. Localizer line shows 19 in all series.

                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-15
                //Select Unlink to unlink all.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Unlink);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //All Series are unlinked. Link icon is not displayed on any series.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status15 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status15 && viewer.LinkScrollingStatusImageList().Count == 0)
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

                //Step-16
                //Toggle off localizer line

                viewer.SelectToolInToolBar(IEnum.ViewerTools.LocalizerLine);
                PageLoadWait.WaitForPageLoad(25);
                PageLoadWait.WaitForFrameLoad(25);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //Localizer lines are disappeared in all of the series.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status16 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status16 &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_1X1(), "src", '&', "ToggleLocalizerOn").Equals("false") &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(), "src", '&', "ToggleLocalizerOn").Equals("false") &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_1X3(), "src", '&', "ToggleLocalizerOn").Equals("false") &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_2X1(), "src", '&', "ToggleLocalizerOn").Equals("false") &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_2X2(), "src", '&', "ToggleLocalizerOn").Equals("false"))
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
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //Step-17
                //[ICA-10683] 
                //1. From Studies tab load any multi-series MR (at least more than 3 series)study in ICA viewer
                //2. Link any two series in viewer
                //3. Scroll in linked series 1 a few times then scroll linked series 2 back to image first and scroll down

                viewer.CloseStudy();
                //Accession: U-ID179490
                study.SearchStudy(AccessionNo: AccessionID, Datasource: EA_91);
                study.SelectStudy("Accession", AccessionID);
                viewer = StudyViewer.LaunchStudy();

                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkSelected);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewer.SelectLinkedCheckBox(1, 1);
                viewer.SelectLinkedCheckBox(1, 2);
                viewer.LinkedScrollingCheckBtn().Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                viewer.switchToUserHomeFrame();

                //Scroll in linked series 1 a few times
                for (int i = 0; i < 10; i++)
                {
                    viewer.ClickDownArrowbutton(1, 1);
                    PageLoadWait.WaitForPageLoad(15);
                    PageLoadWait.WaitForFrameLoad(15);
                    viewer.switchToUserHomeFrame();
                }

                PageLoadWait.WaitForFrameLoad(10);
                viewer.switchToUserHomeFrame();

                //scroll linked series 2 back to image first
                for (int i = 0; i < 21; i++)
                {
                    viewer.ClickUpArrowbutton(1, 2);
                    PageLoadWait.WaitForPageLoad(15);
                    PageLoadWait.WaitForFrameLoad(15);
                    viewer.switchToUserHomeFrame();
                }
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                result.steps[++ExecutedSteps].SetPath(testid + "_17_1_up", ExecutedSteps + 1);
                bool status17_1 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                //scroll down

                for (int i = 0; i < 15; i++)
                {
                    viewer.ClickDownArrowbutton(1, 2);
                }
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                result.steps[ExecutedSteps].SetPath(testid + "_17_2_down", ExecutedSteps + 1);
                bool status17_2 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status17_1 && status17_2 &&
                    viewer.SeriesViewer_1X2().GetAttribute("class").Contains("activeSeriesViewer") &&
                    viewer.SeriesViewer_1X2().GetCssValue("border-top-color").Equals(rgbavalue))
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

                //Step-18
                //[ICA-10683] 
                //4. Select any other series which is not linked
                //5. Without clicking/selecting on linked viewport, slide the scroll handle

                viewer.SeriesViewer_2X1().Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                for (int i = 0; i < 15; i++)
                {
                    viewer.ClickDownArrowbutton(2, 1);
                }
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status18 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status18 &&
                    viewer.SeriesViewer_2X1().GetAttribute("class").Contains("activeSeriesViewer") &&
                    viewer.SeriesViewer_2X1().GetCssValue("border-top-color").Equals(rgbavalue))
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

                //Step--19
                //Logout  
                login.Logout();
                ExecutedSteps++;

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
        /// This Test Case is Verification of "Offset Linked Series"
        /// </summary>

        public TestCaseResult Test_27991(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables

            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);

            try
            {
                String username = Config.adminUserName;
                String password = Config.adminPassword;
                String AccessionID = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");

                Actions action = new Actions(BasePage.Driver);
                //step-1: initial setup
                //Link All in service tool 
                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                Taskbar bar = new Taskbar();
                bar.Hide();

                st.LaunchServiceTool();
                st.NavigateToTab("Linked Scrolling");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);

                if (wpfobject.IsCheckBoxSelected("CB_LinkAll") == false)
                {
                    wpfobject.SelectCheckBox("CB_LinkAll");
                    Logger.Instance.InfoLog("Link All check box is selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Link All check box is already selected");
                }

                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("6");
                wpfobject.WaitTillLoad();
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();

                bar.Show();
                Thread.Sleep(5000);

                ExecutedSteps++;


                //Step-2
                //Login as system Administrator. And open the Study. Change 2x3 viewer.
                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Enable Link All
                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();


                if (domain.LinkAllCheckbox().Selected == false)
                {
                    domain.LinkAllCheckbox().Click();
                    Logger.Instance.InfoLog("Link scrolling Link All Check box is selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Link scrolling Link All Check box is already selected");
                }
                domain.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Launch study
                Studies studies = (Studies)login.Navigate("Studies");
                //U-ID179490
                studies.SearchStudy(AccessionNo: AccessionID, Datasource: EA_91);
                studies.SelectStudy("Accession", AccessionID);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForAllViewportsToLoad(30);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer2x3);
                PageLoadWait.WaitForAllViewportsToLoad(30);


                if (viewer.Thumbnails().Count == 5 &&
                   viewer.SeriesViewPorts().Count == 6)
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


                //step-3: With 6 series are being displayed and all with 1x1 layout.
                //Move all the scroll bar positions to the top of the window.
                //MR-Series 2 is selected as reference viewer.

                foreach (IWebElement viewport in viewer.SeriesViewPorts())
                {
                    viewport.Click();
                    viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout1x1);
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                }

                IWebElement source = viewer.ViewportScrollHandle(1, 1);
                IWebElement destination = viewer.ViewportScrollBar(1, 1);
                int w = destination.Size.Width;
                int h = destination.Size.Height;
                action = new Actions(BasePage.Driver);
                action.ClickAndHold(source).MoveToElement(destination, w / 2, 1).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                source = viewer.ViewportScrollHandle(1, 2);
                destination = viewer.ViewportScrollBar(1, 2);
                w = destination.Size.Width;
                action = new Actions(BasePage.Driver);
                action.ClickAndHold(source).MoveToElement(destination, w / 2, 1).Release().Build().Perform();
                Thread.Sleep(1000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                source = viewer.ViewportScrollHandle(1, 3);
                destination = viewer.ViewportScrollBar(1, 3);
                w = destination.Size.Width;
                action = new Actions(BasePage.Driver);
                action.ClickAndHold(source).MoveToElement(destination, w / 2, 1).Release().Build().Perform();
                Thread.Sleep(1000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                source = viewer.ViewportScrollHandle(2, 1);
                destination = viewer.ViewportScrollBar(2, 1);
                w = destination.Size.Width;
                action = new Actions(BasePage.Driver);
                action.ClickAndHold(source).MoveToElement(destination, w / 2, 1).Release().Build().Perform();
                Thread.Sleep(1000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                source = viewer.ViewportScrollHandle(2, 2);
                destination = viewer.ViewportScrollBar(2, 2);
                w = destination.Size.Width;
                action = new Actions(BasePage.Driver);
                action.ClickAndHold(source).MoveToElement(destination, w / 2, 1).Release().Build().Perform();
                Thread.Sleep(1000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //select MR series-2
                viewer.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //Precondition
                if (viewer.SeriesViewer_1X1().GetAttribute("class").Contains("activeSeriesViewer") &&
                   viewer.SeriesViewer_1X1().GetCssValue("border-top-color").Equals(rgbavalue))
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

                //Step-4: 
                //Select Linked Scrolling button and select Link All Offset option.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkAllOffset);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //A numerical link icon is displayed on the right top corner of all series (except the empty series)

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status4 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status4 && viewer.LinkScrollingStatusImageList().Count == 5)
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

                //step-5:Select the empty window as a reference viewer.
                if (viewer.EmptyViewPorts().Count != 0)
                {
                    viewer.SeriesViewer_2X3().Click();
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                }
                else
                {
                    throw new Exception("Empty viewport not found");
                }

                //The enclosed orange window is displayed around the empty window

                if (viewer.EmptyViewPorts().Count == 1 &&
                    viewer.SeriesViewer_2X3().GetAttribute("class").Contains("activeSeriesViewer") &&
                   viewer.SeriesViewer_2X3().GetCssValue("border-top-color").Equals(rgbavalue) &&
                    viewer.SeriesViewer_2X3().GetAttribute("id").Equals(viewer.EmptyViewPorts()[0].GetAttribute("id")))
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


                //step-6:Scroll up and down.

                IWebElement UpArrow = viewer.UpArrowBtn(2, 3);
                IWebElement DownArrow = viewer.DownArrowBtn(2, 3);

                // Both up/down Button should not display, So verifying IsDisplayed property 
                //Nothing happens.  The image number of the other series are not updated.

                if (!UpArrow.Displayed && !DownArrow.Displayed)
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


                //Step-7:
                //Select the MR-Series 6 window as a reference viewer.

                viewer.SeriesViewer_2X2().Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //The enclosed orange window is displayed around the MR-Series 6 window.

                if (viewer.SeriesViewer_2X2().GetAttribute("class").Contains("activeSeriesViewer") &&
                    viewer.SeriesViewer_2X2().GetCssValue("border-top-color").Equals(rgbavalue))
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

                //step-8:
                //Scroll through the series MR-Series 6 one by one by using down triangle.

                for (int i = 0; i < 3; i++)
                {
                    viewer.ClickDownArrowbutton(2, 2);
                }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //As the series is scrolled, the image number of the base viewer is updated to match 
                //the number of the image of the reference viewer.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status8 = studies.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());
                if (status8 &&
                    viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("4") &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("4") &&
                    viewer.SeriesViewer_1X3().GetAttribute("imagenum").Equals("4") &&
                    viewer.SeriesViewer_2X1().GetAttribute("imagenum").Equals("4") &&
                    viewer.SeriesViewer_2X2().GetAttribute("imagenum").Equals("4"))
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
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Step-9:
                //Select the MR-Series 2 window as a reference viewer.

                viewer.SeriesViewer_1X1().Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //The enclosed orange window is displayed around the MR-Series 2 window.

                if (viewer.SeriesViewer_1X1().GetAttribute("class").Contains("activeSeriesViewer") &&
                    viewer.SeriesViewer_1X1().GetCssValue("border-top-color").Equals(rgbavalue))
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

                //step-10:
                //Drag the vertical scrollbar position to the end of the MR-Series 2

                source = viewer.ViewportScrollHandle(1, 1);
                destination = viewer.ViewportScrollBar(1, 1);
                w = destination.Size.Width;
                h = destination.Size.Height;
                action = new Actions(BasePage.Driver);
                action.ClickAndHold(source).MoveToElement(destination, w / 2, h - 5).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(25);
                PageLoadWait.WaitForFrameLoad(25);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                action.ClickAndHold(source).MoveToElement(destination, w / 2, h).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(25);
                PageLoadWait.WaitForFrameLoad(25);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                Thread.Sleep(5000);
                action.ClickAndHold(source).MoveToElement(destination, w / 2, h).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(25);
                PageLoadWait.WaitForFrameLoad(25);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //In the base viewers -MR-Series 3, the image 23 is displayed.
                //MR-Series 4, MR-Series 5 & MR-Series 6, the out of range linked indicator is displayed on top of the image 19.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status10 = studies.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status10 && viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("23"))
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

                //step-11:Select Unlink to unlink all

                viewer.SelectToolInToolBar(IEnum.ViewerTools.Unlink);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //All Series are unlinked.  Link icon is not displayed on any series.

                if (viewer.LinkScrollingStatusImageList().Count == 0)
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

                //step-12:
                //In the MR-Series 2 select image 5

                source = viewer.ViewportScrollHandle(1, 1);
                destination = viewer.ViewportScrollBar(1, 1);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);
                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 5 / 23).Release().Build().Perform();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 5 / 23).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Equals("internet explorer"))
                //{
                //    viewer.ClickUpArrowbutton(1, 1);
                //    PageLoadWait.WaitForPageLoad(15);
                //    PageLoadWait.WaitForFrameLoad(15);
                //    PageLoadWait.WaitForAllViewportsToLoad(20);
                //}

                //Image 5 of the MR-Series 2 is displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status12 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X1());

                if (status12 && viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("5"))
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

                //step-13:In MR-Series 4 select image 10

                source = viewer.ViewportScrollHandle(1, 3);
                destination = viewer.ViewportScrollBar(1, 3);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);
                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 9 / 19).Release().Build().Perform();
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Image 10 of the MR-Series 4 is displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status13 = studies.CompareImage(result.steps[ExecutedSteps], viewer.SeriesViewer_1X3());

                //Image-9 not available in series-4,so imagenum should be 9 not 10

                int loop = 0;
                while (loop < 2)
                {
                    if (Int32.Parse(viewer.SeriesViewer_1X3().GetAttribute("imagenum")) == 9)
                        break;
                    else if (Int32.Parse(viewer.SeriesViewer_1X3().GetAttribute("imagenum")) < 9)
                        viewer.ClickDownArrowbutton(1, 3);
                    else
                        viewer.ClickUpArrowbutton(1, 3);

                    loop++;
                }

                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                if (status13 && viewer.SeriesViewer_1X3().GetAttribute("imagenum").Equals("9"))
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

                //Step-14
                //Select Linked Scrolling button and select Link Selected Offset option.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkSelectedOffset);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //A window with six black square windows is displayed. 

                if (viewer.LinkSelectTableCheckBoxList().Count == 6)
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

                //Step-15
                //Select square 1 and square 3 and then green check mark.

                viewer.SelectLinkedCheckBox(1, 1);
                viewer.SelectLinkedCheckBox(1, 3);
                viewer.LinkedScrollingCheckBtn().Click();
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //The MR-Series 2 and the MR-Series 4 are now linked offset by 5 images.

                if (viewer.LinkScrollingStatusImageList().Count == 2 &&
                    viewer.LinkScrollingStatusImage(1, 1).Displayed &&
                    viewer.LinkScrollingStatusImage(1, 3).Displayed)
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

                //Step-16
                //Drag the vertical scrollbar position to the top of the MR-Series 2

                source = viewer.ViewportScrollHandle(1, 1);
                destination = viewer.ViewportScrollBar(1, 1);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);
                action.ClickAndHold(source).MoveToElement(destination, w / 2, 1).Release().Build().Perform();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                action.ClickAndHold(source).MoveToElement(destination, w / 2, 1).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(30);


                //In the base viewers - MR-Series 3, no change.MR-Series 4, the image 6 is displayed.
                //MR-Series 5, no change.   MR-Series 6, no change.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status16 = studies.CompareImage(result.steps[ExecutedSteps], viewer.ViewerContainer());

                if (status16 && viewer.SeriesViewer_1X3().GetAttribute("imagenum").Equals("6"))
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

                //step-17:Select Unlink to unlink all
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Unlink);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                if (viewer.LinkScrollingStatusImageList().Count == 0)
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

                //Step-18
                //Logout
                login.Logout();
                ExecutedSteps++;

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
        /// This Test Case is Verification of "Anatomic Linking with Offset"
        /// </summary>

        public TestCaseResult Test_27992(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;


            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;

            String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
            String[] AccessionNumbers = AccessionNoList.Split(':');

            String StudyDateList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Date");
            String[] StudyDate = StudyDateList.Split('=');

            try
            {

                //Step-1
                //Precondition completed
                ExecutedSteps++;


                //Step-2
                //Precondition completed
                //Load a study with multiple series - Brunschweiler, Rene ID#141496 & its prior study 254682 located at \\optra
                //Coronal view - MR Series 2 in study 254682 and MR Series 4 in study 141496 are used.
                //Set Inter VNT greater than 22 degrees (example - 25) by configuring 
                //the Merge iConnect Service Tool-> Link scrolling tab-> set Inter Volume Normal Tolerance (Inter VNT)
                //Note:Numerical linking is used if Inter VNT < or Equal 22 degrees).
                //The viewer should be set for 2 series.

                //**##--
                //string WebConfigPath = @"C:\\WebAccess\\WebAccess\\Web.config";
                //login.SetWebConfigValue(WebConfigPath, "LinkedScrolling.Tolerances.InterVolumeNormal", "25.0");
                //Thread.Sleep(2000);
                //login.RestartIISUsingexe();


                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                Taskbar bar = new Taskbar();
                bar.Hide();

                st.LaunchServiceTool();
                st.NavigateToTab("Linked Scrolling");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                wpfobject.WaitTillLoad();
                if (wpfobject.IsCheckBoxSelected("CB_LinkAll") == false)
                {
                    wpfobject.SelectCheckBox("CB_LinkAll");
                    Logger.Instance.InfoLog("Link All check box is selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Link All check box is already selected");
                }

                wpfobject.WaitTillLoad();
                //**Have to change script
                //wpfobject.SetSpinner("interVolumeNormalNumericUpDown", "25.0");
                st.SetSpinnerValueFromTab(25.0, "AutoSelectTextBox", 0, "1");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("6");
                wpfobject.WaitTillLoad();

                ExecutedSteps++;


                //Step-3
                //**##--
                //In order to load a study with its priors, the Merge EMPI must be set to None 
                //by doing the followings-The Merge iConnect Service Tool-> Enable Features tab-> MPI-> 
                //Select None and then Apply button. IISRESET.

                st.NavigateToTab("Enable Features");
                wpfobject.GetTabWpf(1).SelectTabPage(3);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickRadioButtonById("RB_PatientDemographicQueryNone");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();


                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();

                bar.Show();

                ExecutedSteps++;


                //Step-4
                //Login and open the study
                //AccessionNumbers[0]= U-ID323791 

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                //Enable Link All
                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();

                if (domain.LinkAllCheckbox().Selected == false)
                {
                    domain.LinkAllCheckbox().Click();
                    Logger.Instance.InfoLog("Link scrolling Link All Check box is selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Link scrolling Link All Check box is already selected");
                }
                domain.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                Studies study = (Studies)login.Navigate("Studies");
                //AccessionNumbers[0] : U-ID323791
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_91);
                study.SelectStudy("Accession", AccessionNumbers[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();

                PageLoadWait.WaitForFrameLoad(30);
                viewer.NavigateToHistoryPanel();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //StudyDate[0] :30-May-2006 8:33:32 AM
                viewer.OpenPriors(new string[] { "Study Date" }, new string[] { StudyDate[0] });
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);

                //add validation
                if (viewer.studyPanel(1).Displayed && viewer.studyPanel(2).Displayed)
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


                //Step-5
                //Load MR-Series 2 of the study ID 254682 into the left viewer.

                viewer.SeriesViewer_1X1(1).Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //The MR-Series 2 is loaded successfully into the left viewer and 1x1 layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status5 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel(1));

                if (status5 && viewer.SeriesViewPorts().Count.Equals(1) &&
                   viewer.Thumbnails(1).Count == 5 &&
                   viewer.studyPanel(1).GetAttribute("class").Contains("Active") &&
                   viewer.studyPanel(1).GetCssValue("border-top-color").Equals(rgbavalue))
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

                //Step-6
                //Load MR-Series 4 of the study ID 141496 into the right viewer.

                viewer.SeriesViewer_2X1(2).Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //The MR-Series 4 is loaded successfully into the right viewer and 1x1 layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status6 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel(2));

                if (status6 && viewer.SeriesViewPorts(2).Count.Equals(1) &&
                   viewer.Thumbnails(2).Count == 5 &&
                   viewer.studyPanel(2).GetAttribute("class").Contains("Active") &&
                   viewer.studyPanel(2).GetCssValue("border-top-color").Equals(rgbavalue))
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

                //Step-7
                //Select MR-Series 2 of the study ID 254682 (left viewer).

                viewer.SeriesViewer_1X1(1).Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //The enclosed orange window is displayed around the left viewer.

                if (viewer.studyPanel(1).GetAttribute("class").Contains("Active") &&
                   viewer.studyPanel(1).GetCssValue("border-top-color").Equals(rgbavalue))
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

                //Step-8
                //In the MR-Series 2 select image 9  (example SliceLocation - -55.624817473853)

                for (int i = 0; i < 8; i++)
                {
                    viewer.ClickDownArrowbutton(1, 1, 1);

                }
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //Image 9 of the MR-Series 2 is displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status8 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel(1));

                if (status8 &&
                    viewer.SeriesViewer_1X1(1).GetAttribute("imagenum").Equals("9"))
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

                //Step-9
                //In MR-Series 4 select image 10 (example SliceLocation- -66.972604698546)

                for (int i = 0; i < 9; i++)
                {
                    viewer.ClickDownArrowbutton(1, 1, 2);
                }
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //Image 10 of the MR-Series 4 is displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status9 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel(2));

                //Series-4 1x3 Image-9 not available.
                //Image 11 will displaying the screenshot

                if (status9 &&
                    viewer.SeriesViewer_1X1(2).GetAttribute("imagenum").Equals("10"))
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

                //Step-10
                //Select Linked Scrolling button and select Link Selected Offset option.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkSelectedOffset);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //Two windows with a black square are displayed.
                if (viewer.LinkSelectTableCheckBoxList(1).Count == 1 &&
                    viewer.LinkSelectTableCheckBox(1, 1, 1).Displayed &&
                    viewer.LinkSelectTableCheckBoxList(2).Count == 1 &&
                    viewer.LinkSelectTableCheckBox(1, 1, 2).Displayed)
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

                //Step-11
                //Select square 1 and square 2 and then green check mark.

                viewer.SelectLinkedCheckBox(1, 1, 1);
                viewer.SelectLinkedCheckBox(1, 1, 2);
                viewer.LinkedScrollingCheckBtn(2).Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //The reference series and the base series are now linked offset by ~11.35 slice location.
                //(the difference between the image 9 from the reference series and the image 10 from the base series)

                if (viewer.LinkScrollingStatusImageList().Count == 2 &&
                    viewer.LinkScrollingStatusImage(1, 1, 1).Displayed &&
                    viewer.LinkScrollingStatusImage(1, 1, 2).Displayed)
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


                //Step-12
                //Drag the vertical scrollbar position to the bottom of the reference series.
                //(SliceLocation - -19.624816845243)

                IWebElement source = viewer.ViewportScrollHandle(1, 1, 1);
                IWebElement destination = viewer.ViewportScrollBar(1, 1, 1);

                int w = destination.Size.Width;
                int h = destination.Size.Height;

                Actions action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h).Release().Build().Perform();
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                action.ClickAndHold(source).MoveToElement(destination, w / 2, h).Release().Build().Perform();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //Image 19 is displayed in the base viewer.



                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status12 = study.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (status12 &&
                    viewer.SeriesViewer_1X1(2).GetAttribute("imagenum").Equals("18"))
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

                //Step-13
                //Select MR-Series 4

                viewer.SeriesViewer_1X1(2).Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //MR-Series 4 becomes a reference viewer (with orange window).
                //MR-Series 2 becomes a base viewer.

                if (viewer.studyPanel(2).GetAttribute("class").Contains("Active") &&
                    viewer.studyPanel(2).GetCssValue("border-top-color").Equals(rgbavalue) &&
                    viewer.Thumbnails(2)[2].GetCssValue("border-top-color").Equals(rgbavalue) &&
                    viewer.SeriesViewer_1X1(2).GetAttribute("class").Contains("activeSeriesViewer") &&
                    viewer.SeriesViewer_1X1(2).GetCssValue("border-top-color").Equals(rgbavalue))
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

                //Step-14 -17 
                // Mouse middle wheel operation
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-18
                //Select Unlink to unlink all

                viewer.SelectToolInToolBar(IEnum.ViewerTools.Unlink);
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);
                //All Series are unlinked.  Link icon is not displayed on any series.
                if (viewer.LinkScrollingStatusImageList().Count == 0 &&
                    viewer.LinkScrollingStatusImage(1, 1, 1).Displayed == false &&
                    viewer.LinkScrollingStatusImage(1, 1, 2).Displayed == false)
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

                //Step-19
                //Scroll up/down one of the series.

                for (int i = 0; i < 4; i++)
                {
                    viewer.ClickUpArrowbutton(1, 1, 1);
                }
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //The image is not updated on the other series.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status19 = study.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                //Series-2 1x1 Image-13 not available.
                //Series-4 1x3 Image-9 not available.

                if (status19 && viewer.SeriesViewer_1X1(1).GetAttribute("imagenum").Equals("15") &&
                    viewer.SeriesViewer_1X1(2).GetAttribute("imagenum").Equals("18"))
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
                study.CloseStudy();

                //Step-20
                //Enable the Merge EMPI by doing the followings- The Merge iConnect Service Tool-> Enable Features tab-> MPI-> Select Merge EMPI-> Attribute based search and then Apply button.  IISRESET.

                //Have to do (if Patient tab works)
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-21
                //Go to the Domain Management tab, select SuperAdminGroup and select Edit button.

                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();

                //Edit Domain page is displayed.

                if (domain.GetElement("cssselector", "a[id$='EditDomainControl_HyperLink1']").Text.Equals("Domain Management") &&
                    domain.GetElement("cssselector", "span[id$='EditDomainControl_Label1']").Text.Equals("Edit Domain"))
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

                //Step-22
                //Unselect/uncheck box Enable Linked Scrolling Link All   Select Save button

                if (domain.LinkAllCheckbox().Selected)
                    domain.LinkAllCheckbox().Click();

                domain.ClickSaveDomain();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                //The changes are saved successfully and Domain Management page is displayed.
                ExecutedSteps++;

                //Step-23 -27
                //Patient Tab

                for (int i = 0; i < 5; i++)
                    result.steps[++ExecutedSteps].status = "Not Automated";


                //Patients patient = (Patients)login.Navigate("Patients");
                //ExecutedSteps++;
                //study.SearchStudy("last Name", "John");
                //ExecutedSteps++;
                //study.SelectStudy("Study ID", "1111");
                //ExecutedSteps++;
                //StudyViewer.LaunchStudy();
                //ExecutedSteps++;

                //Step-28
                //Select Studies tab.
                login.Navigate("Studies");
                ExecutedSteps++;

                //Step-29
                //Load a study with multiple series- Brunschweiler, Rene ID#141496 located at \\optra.
                //AccessionNumbers[1]=U-ID179490
                study.SearchStudy(AccessionNo: AccessionNumbers[1], Datasource: EA_91);
                study.SelectStudy("Accession", AccessionNumbers[1]);
                StudyViewer.LaunchStudy();
                PageLoadWait.WaitForAllViewportsToLoad(30);


                //The study is displayed in the Study Viewer.

                if (viewer.Thumbnails().Count == 5 && viewer.SeriesViewPorts().Count == 4)
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

                //Step-30
                //Load MR-Series 2 into the viewer.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.SeriesViewer1x1);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                action = new Actions(BasePage.Driver);

                action.DoubleClick(viewer.Thumbnails()[0]).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);


                //The MR-Series 2 is loaded successfully into the viewer with 1 series and 1x1 layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status30 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status30 && viewer.SeriesViewPorts().Count == 1)
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

                //Step-31
                //Hover the mouse cursor to the Linked Scrolling button from the menu.

                IWebElement linkselected = viewer.GetReviewTool("Link Selected");
                viewer.JSMouseHover(linkselected);

                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //A drop down menu is displayed. Unlink, Link Selected, Link Selected Offset

                IList<IWebElement> dropdown5 = BasePage.Driver.FindElements(By.CssSelector("li[title='Link Selected'] ul>li"));
                IList<String> title = new List<String>();

                bool flag31 = true;
                foreach (IWebElement dropdowntool in dropdown5)
                {
                    title.Add(dropdowntool.GetAttribute("title"));
                    if (!(dropdowntool.Enabled && dropdowntool.Displayed))
                    {
                        flag31 = false;
                        break;
                    }
                }
                if (flag31 && dropdown5.Count == 3 && title.SequenceEqual(link_3))
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

                //Logout  -Step-32
                login.Logout();
                ExecutedSteps++;

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
        /// This Test Case is Verification of "Hosted Integration Mode"
        /// </summary>

        public TestCaseResult Test_27993(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;

            String LastName = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
            try
            {

                //Step-1
                //Precondition--
                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                Taskbar bar = new Taskbar();
                bar.Hide();

                st.LaunchServiceTool();
                st.NavigateToTab("Linked Scrolling");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                wpfobject.WaitTillLoad();
                if (wpfobject.IsCheckBoxSelected("CB_LinkAll") == true)
                {
                    wpfobject.UnSelectCheckBox("CB_LinkAll");
                    Logger.Instance.InfoLog("Link All check box is un-selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Link All check box is already un-selected");
                }

                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("6");
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();

                bar.Show();

                ExecutedSteps++;

                //Step-2
                //##** 
                //Copy the TestEHR.exe application from the TestTools folder to the test server system.
                //Ensure that the ByPass mode is enable in the ..WebAccess\IntegratorAuthenticationSTS\Web.config by replacing from
                //"IntegratorAuthenticator"to"ByPassAuthenticator"

                ExecutedSteps++;

                //Step-3
                //Run the TestEHR.exe application. Address box - http -//localhost/WebAccess.
                //Security ID box - Administrator-Administrator, Select Use default browser, Show Selector as True
                //Show Selector Search as True, Show Report as True, Select Patient for Selector Centricity
                //Auto End Session as True, Enter b for Patient Last Name, Select Load button


                //Precondition--
                bar.Hide();

                st.LaunchServiceTool();
                st.NavigateToTab("Linked Scrolling");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                wpfobject.WaitTillLoad();
                //**Have to change spinner id
                //wpfobject.SetSpinner("interVolumeNormalNumericUpDown", "25.0");
                st.SetSpinnerValueFromTab(25.0, "AutoSelectTextBox", 0, "1");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("6");
                wpfobject.WaitTillLoad();

                st.NavigateToTab("Integrator");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);
                wpfobject.WaitTillLoad();

                wpfobject.SelectFromComboBox("ComboBox_OnMultipleStudies", "1", 1);
                wpfobject.WaitTillLoad();

                wpfobject.SelectCheckBox("CB_AllowShowSelectorSearch");
                wpfobject.WaitTillLoad();

                wpfobject.SelectCheckBox("CB_AllowShowSelector");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();

                wpfobject.ClickButton("6");
                wpfobject.WaitTillLoad();

                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();

                bar.Show();


                //Run the TestEHR.exe application. Address box: http://localhost/WebAccess. 
                //Security ID box: Administrator-Administrator 
                //Show Selector=True Show Selector Search=True Show Report=True 
                //Select Patient for Selector Centricity Auto 
                //End Session=True Enter b for Patient Last Name Select Load button


                ehr.LaunchEHR();
                ehr.SetCommonParameters(SecurityID: "Administrator-Administrator", autoendsession: "True");
                ehr.SetSelectorOptions(showSelector: "True", selectorsearch: "True", showReport: "True", selectoroption: "Patient");
                ehr.SetSearchKeys_Study("b", "Last_Name");

                String TestEHRURL = ehr.clickCmdLine("ImageLoad");

                BasePage.Driver.Navigate().GoToUrl(TestEHRURL);
                Thread.Sleep(10000);
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(25);
                PageLoadWait.WaitForPageLoad(25);
                PageLoadWait.WaitForFrameLoad(25);

                login.SwitchToDefault();
                BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");
                PageLoadWait.WaitForPageLoad(25);
                Thread.Sleep(5000);
                Thread.Sleep(5000);

                //A patient list is generated

                if (login.GetElement("id", "listControlDiv").Displayed)
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

                //Step-4
                //Select Brunschweiler, Rene and View button

                login.SetText("id", "ctl00_m_studySearchControl_m_searchInputPatientLastName", LastName);
                login.Click("id", "ctl00_m_studySearchControl_m_searchButton");
                Thread.Sleep(10000);

                PageLoadWait.WaitForPageLoad(30);


                login.Click("id", "ctl00_ctl05_parentGrid_check_0_3");
                PageLoadWait.WaitForPageLoad(15);

                Thread.Sleep(4000);
                login.Click("id", "ctl00_StudyListControlContent_m_viewButton");
                Thread.Sleep(10000);
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForPageLoad(50);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                StudyViewer viewer = new StudyViewer();

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status4 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status4 && viewer.SeriesViewPorts().Count == 4 &&
                    viewer.Thumbnails().Count == 5)
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

                //Step-5
                //Hover the mouse cursor to the Linked Scrolling button from the menu.

                IWebElement linkselected = viewer.GetReviewTool("Link Selected");
                viewer.JSMouseHover(linkselected);

                PageLoadWait.WaitForPageLoad(15);


                //A drop down menu is displayed. Unlink, Link Selected, Link Selected Offset
                login.SwitchToDefault();
                BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");

                IList<IWebElement> dropdown3 = BasePage.Driver.FindElements(By.CssSelector("li[title='Link Selected'] ul>li"));
                IList<String> title = new List<String>();
                bool flag5 = true;
                foreach (IWebElement dropdowntool in dropdown3)
                {
                    title.Add(dropdowntool.GetAttribute("title"));
                    if (!(dropdowntool.Enabled && dropdowntool.Displayed))
                    {
                        flag5 = false;
                        break;
                    }
                }
                if (flag5 && dropdown3.Count == 3 && title.SequenceEqual(link_3))
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

                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //Step-6 
                //Close the browser
                login.CloseBrowser();
                BasePage.Driver.Quit();
                ExecutedSteps++;

                //Step-7
                //Run Merge iConnect Access Service Tool. Select Linked Scrolling tab.

                bar.Hide();

                st.LaunchServiceTool();
                st.NavigateToTab("Linked Scrolling");
                wpfobject.WaitTillLoad();

                //Linked Scrolling tab is displayed

                ExecutedSteps++;

                //Step-8
                //Select Modify button, select Integrator"Link All"Enabled and then select Apply button. IISRESET.

                wpfobject.ClickButton("Modify", 1);
                wpfobject.WaitTillLoad();
                if (wpfobject.IsCheckBoxSelected("CB_LinkAll") == false)
                {
                    wpfobject.SelectCheckBox("CB_LinkAll");
                    Logger.Instance.InfoLog("Link All check box is selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Link All check box is already selected");
                }

                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("6");
                wpfobject.WaitTillLoad();
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();

                bar.Show();

                //The changes are saved successfully.

                ExecutedSteps++;

                //Step-9
                //Run the TestEHR.exe application.
                //Address box-http-//localhost/WebAccess. Security ID box - Administrator-Administrator
                //Select Use default browser, Show Selector as True, Show Selector Search as True,
                //Show Report as True, Select Patient for Selector Centricity, Auto End Session as True,
                //Enter b for Patient Last Name, Select Load button

                //**## have to do
                login.InvokeBrowser(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName);

                ehr.LaunchEHR();
                ehr.SetCommonParameters(SecurityID: "Administrator-Administrator", autoendsession: "True");
                ehr.SetSelectorOptions(showSelector: "True", selectorsearch: "True", showReport: "True", selectoroption: "Patient");
                ehr.SetSearchKeys_Study("b", "Last_Name");

                TestEHRURL = ehr.clickCmdLine("ImageLoad");

                BasePage.Driver.Navigate().GoToUrl(TestEHRURL);
                Thread.Sleep(10000);
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(25);
                PageLoadWait.WaitForPageLoad(25);
                PageLoadWait.WaitForFrameLoad(25);

                login.SwitchToDefault();
                BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");
                PageLoadWait.WaitForPageLoad(25);
                Thread.Sleep(3000);
                Thread.Sleep(5000);

                //A patient list is generated

                if (login.GetElement("id", "listControlDiv").Displayed)
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

                //Step-10
                //Select Brunschweiler, Rene and View button

                login.SetText("id", "ctl00_m_studySearchControl_m_searchInputPatientLastName", LastName);
                login.Click("id", "ctl00_m_studySearchControl_m_searchButton");
                Thread.Sleep(3000);

                PageLoadWait.WaitForPageLoad(15);
                login.Click("id", "ctl00_ctl05_parentGrid_check_0_3");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(15);

                login.Click("id", "ctl00_StudyListControlContent_m_viewButton");
                Thread.Sleep(10000);
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForPageLoad(50);
                PageLoadWait.WaitForAllViewportsToLoad(30);


                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status10 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status10 && viewer.SeriesViewPorts().Count == 4 &&
                    viewer.Thumbnails().Count == 5)
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

                //Step-11
                //Hover the mouse cursor to the Linked Scrolling button from the menu.

                linkselected = viewer.GetReviewTool("Link Selected");
                viewer.JSMouseHover(linkselected);

                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                login.SwitchToDefault();
                BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");

                //A drop down menu is displayed. Link All, Link All Offset, Unlink, Link Selected, Link Selected Offset

                IList<IWebElement> dropdown5 = BasePage.Driver.FindElements(By.CssSelector("li[title='Link Selected'] ul>li"));
                title.Clear();
                bool flag11 = true;
                foreach (IWebElement dropdowntool in dropdown5)
                {
                    title.Add(dropdowntool.GetAttribute("title"));
                    if (!(dropdowntool.Enabled && dropdowntool.Displayed))
                    {
                        flag11 = false;
                        break;
                    }
                }
                if (flag11 && dropdown5.Count == 5 && title.SequenceEqual(link_5))
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

                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //Step-12 
                //Close the browser
                login.CloseBrowser();
                BasePage.Driver.Quit();
                ExecutedSteps++;

                //Step-13
                //Test Data -  AUTO-5
                //Note: In the following section, the Linked Scrolling is disabled by running 
                //Merge iConnect Access Service Tool. Run Merge iConnect Access Service Tool. 
                //Select Linked Scrolling tab.

                bar.Hide();

                st.LaunchServiceTool();
                st.NavigateToTab("Linked Scrolling");
                wpfobject.WaitTillLoad();

                ExecutedSteps++;

                //Linked Scrolling tab is displayed

                //Step-14
                //Select Modify btn, UNSELECT Integrator"Link All"Enabled & then select Apply btn. IISRESET
                wpfobject.ClickButton("Modify", 1);

                if (wpfobject.IsCheckBoxSelected("CB_LinkAll") == true)
                {
                    wpfobject.UnSelectCheckBox("CB_LinkAll");
                    Logger.Instance.InfoLog("Link All check box is un-selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Link All check box is already un-selected");
                }

                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("6");
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();

                bar.Show();

                ExecutedSteps++;

                //The changes are saved successfully.

                //Step-15
                //Run the TestEHR.exe application.
                //Address box-http-//localhost/WebAccess. Security ID box - Administrator-Administrator
                //Select Use default browser, Show Selector as True, Show Selector Search as True,
                //Show Report as True, Select Patient for Selector Centricity, Auto End Session as True,
                //Enter b for Patient Last Name, Select Load button

                //**## have to add testehr code.

                login.InvokeBrowser(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName);

                ehr.LaunchEHR();
                ehr.SetCommonParameters(SecurityID: "Administrator-Administrator", autoendsession: "True");
                ehr.SetSelectorOptions(showSelector: "True", selectorsearch: "True", showReport: "True", selectoroption: "Patient");
                ehr.SetSearchKeys_Study("b", "Last_Name");

                TestEHRURL = ehr.clickCmdLine("ImageLoad");

                BasePage.Driver.Navigate().GoToUrl(TestEHRURL);
                Thread.Sleep(10000);
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(25);
                PageLoadWait.WaitForPageLoad(25);
                PageLoadWait.WaitForFrameLoad(25);

                login.SwitchToDefault();
                BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");
                PageLoadWait.WaitForPageLoad(25);
                Thread.Sleep(3000);
                Thread.Sleep(5000);

                //A patient list is generated

                if (login.GetElement("id", "listControlDiv").Displayed)
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

                //Step-16
                //Select Brunschweiler, Rene and View button

                login.SetText("id", "ctl00_m_studySearchControl_m_searchInputPatientLastName", LastName);
                login.Click("id", "ctl00_m_studySearchControl_m_searchButton");
                Thread.Sleep(3000);

                PageLoadWait.WaitForPageLoad(15);
                login.Click("id", "ctl00_ctl05_parentGrid_check_0_3");
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(15);

                login.Click("id", "ctl00_StudyListControlContent_m_viewButton");
                Thread.Sleep(10000);
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForPageLoad(50);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status16 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());

                if (status16 && viewer.SeriesViewPorts().Count == 4 &&
                    viewer.Thumbnails().Count == 5)
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

                //Step-17
                //Hover the mouse cursor to the Linked Scrolling button from the menu.

                linkselected = viewer.GetReviewTool("Link Selected");
                viewer.JSMouseHover(linkselected);

                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                login.SwitchToDefault();
                BasePage.Driver.SwitchTo().Frame("IntegratorHomeFrame");

                //A drop down menu is displayed. Unlink, Link Selected, Link Selected Offset

                IList<IWebElement> dropdown_3 = BasePage.Driver.FindElements(By.CssSelector("li[title='Link Selected'] ul>li"));
                title.Clear();
                bool flag17 = true;
                foreach (IWebElement dropdowntool in dropdown_3)
                {
                    title.Add(dropdowntool.GetAttribute("title"));
                    if (!(dropdowntool.Enabled && dropdowntool.Displayed))
                    {
                        flag17 = false;
                        break;
                    }
                }

                if (flag17 && dropdown_3.Count == 3 && title.SequenceEqual(link_3))
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

                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //Step-18 
                //Close the browser
                login.CloseBrowser();
                BasePage.Driver.Quit();
                ExecutedSteps++;

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

                //Close the browser
                login.CloseBrowser();

                //Return Result
                return result;
            }
        }


        /// <summary> 
        /// This Test Case is Verification of "Linking Series with multiple priors -Anatomic linking with priors"
        /// </summary>

        public TestCaseResult Test_27994(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;

            String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
            String[] AccessionNumbers = AccessionNoList.Split(':');

            String StudyDateList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Date");
            String[] StudyDate = StudyDateList.Split('=');

            try
            {
                //Step-1
                // Precondition

                ExecutedSteps++;

                //Step-2
                //Load a study with multiple priors - B, E Patient ID#556677 located at \\optra.
                //The default Linked Scrolling Configuration is used. Refer to Linked Scrolling- DataSet excel tab for the Image SliceLocation.
                //1. Ensure to select Link All Enabled in the Merge iConnect Access Service Tool ->
                //Linked Scrolling tab and in the Domain Management tab.
                //2 Ensure the viewer and series layout are added to the menu.
                //Anatomic linking with priors For anatomical linking, all of the linked series need to be approximately in the same plain.
                //For example axial images can only be anatomically linked with other axial images.

                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                Taskbar bar = new Taskbar();
                bar.Hide();

                st.LaunchServiceTool();
                st.NavigateToTab("Linked Scrolling");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);

                if (wpfobject.IsCheckBoxSelected("CB_LinkAll") == false)
                {
                    wpfobject.SelectCheckBox("CB_LinkAll");
                    Logger.Instance.InfoLog("Link All check box is selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Link All check box is already selected");
                }

                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("6");
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();
                bar.Show();
                Thread.Sleep(3000);

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();

                //Enable Link All check box
                if (domain.LinkAllCheckbox().Selected == false)
                {
                    domain.LinkAllCheckbox().Click();
                    Logger.Instance.InfoLog("Link scrolling Link All Check box is selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Link scrolling Link All Check box is already selected");
                }

                if (domain.LinkAllCheckbox().Selected == true)
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

                domain.ClickSaveDomain();


                //Step-3
                //Load study B, E with accession 1211824 (study date 12-Jan-2005) into the viewer.
                //AccessionNumbers[0]=1211824

                Studies study = (Studies)login.Navigate("Studies");
                //AccessionNumbers[0] :1211824
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_91);
                study.SelectStudy("Accession", AccessionNumbers[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForAllViewportsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);
                PageLoadWait.WaitForThumbnailsToLoad(40);

                //The study is loaded successfully into the viewer with 4 series and 2x2 layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status3 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel(1));

                if (status3 && viewer.SeriesViewPorts().Count == 4 &&
                    viewer.Thumbnails().Count == 4)
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

                //Step-4
                //Select History tab and select a prior with study date 09-Mar-2005 into the second viewer panel.

                PageLoadWait.WaitForFrameLoad(30);
                viewer.NavigateToHistoryPanel();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                ////StudyDate[0] :09-Mar-2005 11:46:49 AM
                viewer.OpenPriors(new string[] { "Study Date" }, new string[] { StudyDate[0] });

                //viewer.DoubleClick(viewer.Study(2));
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                viewer.SeriesViewer_1X2(2).Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //The prior is loaded successfully into the second viewer panel with 2 series and 2x2 layout.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status4 = study.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel(2));

                if (status4 && viewer.SeriesViewPorts(2).Count == 4 &&
                    viewer.Thumbnails(2).Count == 2)
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

                //Step-5
                //Select one of the series as a reference viewer (example - accession #1211824 - Series 2 PET NAC 2D)

                viewer.SeriesViewer_2X2(1).Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                viewer.UpArrowBtn(2, 2, 1).Click();//clicked up arrow for getting image number
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //The enclosed orange window is displayed around the Series 2 Image 1

                if (viewer.studyPanel(1).GetAttribute("class").Contains("Active") &&
                   viewer.studyPanel(1).GetCssValue("border-top-color").Equals(rgbavalue) &&
                   viewer.SeriesViewer_2X2(1).GetAttribute("class").Contains("activeSeriesViewer") &&
                   viewer.SeriesViewer_2X2(1).GetCssValue("border-top-color").Equals(rgbavalue))
                //viewer.SeriesViewer_2X2(1).GetAttribute("imagenum").Equals("1"))
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

                //Step-6
                //Hover the mouse cursor to the Linked Scrolling button from the menu.

                IWebElement linkselected = viewer.GetReviewTool("Link Selected");
                viewer.JSMouseHover(linkselected);

                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //A drop down menu is displayed. Link All, Link All Offset, Unlink, Link Selected, Link Selected Offset

                IList<IWebElement> dropdown5 = BasePage.Driver.FindElements(By.CssSelector("li[title='Link Selected'] ul>li"));
                bool flag5 = true;
                IList<String> title = new List<String>();
                foreach (IWebElement dropdowntool in dropdown5)
                {
                    title.Add(dropdowntool.GetAttribute("title"));
                    if (!(dropdowntool.Enabled && dropdowntool.Displayed))
                    {
                        flag5 = false;
                        break;
                    }
                }

                if (flag5 && dropdown5.Count == 5 && title.SequenceEqual(link_5))
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
                //Click reset to remove mousehover
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Step-7

                //Select Link Selected from the list

                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkSelected);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //Two windows with four black square windows are displayed.
                //(Black square represents for the series in the window and window represents panel viewer)

                if (viewer.LinkSelectTable(1).Displayed &&
                    viewer.LinkSelectTable(2).Displayed &&
                    viewer.LinkSelectTableCheckBoxList(1).Count == 4 &&
                    viewer.LinkSelectTableCheckBoxList(2).Count == 4 &&
                     viewer.LinkSelectTableCheckBox(1, 1, 1).Displayed &&
                     viewer.LinkSelectTableCheckBox(1, 2, 1).Displayed &&
                     viewer.LinkSelectTableCheckBox(2, 1, 1).Displayed &&
                     viewer.LinkSelectTableCheckBox(2, 2, 1).Displayed &&
                     viewer.LinkSelectTableCheckBox(1, 1, 2).Displayed &&
                     viewer.LinkSelectTableCheckBox(1, 2, 2).Displayed &&
                     viewer.LinkSelectTableCheckBox(2, 1, 2).Displayed &&
                     viewer.LinkSelectTableCheckBox(2, 2, 2).Displayed)
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

                //Step-8
                //Select square 4 of the reference viewer (example - accession #1211824 - Series 2 PET NAC 2D)
                //and square 2 (example - accession #1700795 - Series 2 PET NAC 2D) and then green check mark.

                PageLoadWait.WaitForAllViewportsToLoad(30);
                viewer.SelectLinkedCheckBox(2, 2, 1);
                viewer.SelectLinkedCheckBox(1, 2, 2);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.LinkedScrollingCheckBtn(1)));
                //viewer.LinkedScrollingCheckBtn(1).Click();
                viewer.ClickElement(viewer.LinkedScrollingCheckBtn(1));
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);


                //Two series are linked.An anatomical linking icon is displayed on the right top corner of the image.
                //At this point the Series 2 with accession # 1700795 is referred as a Base viewer

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status8 = study.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (status8 && viewer.LinkScrollingStatusImageList().Count == 2 &&
                    viewer.LinkScrollingStatusImage(2, 2, 1).Displayed &&
                    viewer.LinkScrollingStatusImage(1, 2, 2).Displayed &&
                     viewer.studyPanel(1).GetAttribute("class").Contains("Active") &&
                     viewer.studyPanel(1).GetCssValue("border-top-color").Equals(rgbavalue) &&
                     viewer.SeriesViewer_2X2(1).GetAttribute("class").Contains("activeSeriesViewer") &&
                     viewer.SeriesViewer_2X2(1).GetCssValue("border-top-color").Equals(rgbavalue))
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

                //Step-9
                //Scroll to the image 4 in the reference Series 2 (right viewer) by using down triangle.

                for (int i = 0; i < 3; i++)
                {
                    viewer.ClickDownArrowbutton(1, 2, 2);
                }
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //In the base viewer Series 2 (left viewer), the image 4 is displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status9 = study.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (status9 && viewer.SeriesViewer_2X2(1).GetAttribute("imagenum").Equals("4"))
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

                //Step-10
                //Scroll to the image 34 in the reference Series 2 (right viewer) by using down triangle.

                for (int i = 0; i < 30; i++)
                {
                    viewer.ClickDownArrowbutton(1, 2, 2);
                }
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //In the base viewer Series 2 (left viewer), the image 34 is displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status10 = study.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (status10 && viewer.SeriesViewer_2X2(1).GetAttribute("imagenum").Equals("34"))
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

                //Step-11
                //Continue scrolling to the image 56 in the reference Series 2 by using the mouse's middle wheel.

                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-12
                //Select a middle position on the vertical scrollbar in the reference Series 2
                //Example - Image 176.

                IWebElement source = viewer.ViewportScrollHandle(1, 2, 2);
                IWebElement destination = viewer.ViewportScrollBar(1, 2, 2);

                int w = destination.Size.Width;
                int h = destination.Size.Height;

                Actions action = new Actions(BasePage.Driver);
                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 166 / 311).Release().Build().Perform();

                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                int loop = 0;
                while (loop < 7)
                {
                    if (Int32.Parse(viewer.SeriesViewer_1X2(2).GetAttribute("imagenum")) == 176)
                        break;
                    else if (Int32.Parse(viewer.SeriesViewer_1X2(2).GetAttribute("imagenum")) < 176)
                        viewer.ClickDownArrowbutton(1, 2, 2);
                    else
                        viewer.ClickUpArrowbutton(1, 2, 2);

                    loop++;
                }
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //In the base viewer Series 2 (left viewer), the image 176 is displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status12 = study.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (status12 && viewer.SeriesViewer_2X2(1).GetAttribute("imagenum").Equals("176"))
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

                //Step-13
                //Change the layout of the reference viewport to 2x2.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Layout 2x2 is displayed in the reference viewport Series 2.
                //Note-The link icon is displayed in the right top corner of the window 
                //& is not displayed on the individual images.

                if (viewer.LinkScrollingStatusImageList().Count == 2 &&
                    viewer.GetInnerAttribute(viewer.SeriesViewer_1X2(2), "src", '&', "layoutFormat").Equals("2x2"))
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

                //Step-14
                //Use the mouse's middle wheel, scroll to the image 229, Image 230, Image 231 
                //and Image 232 in the reference Series 2.

                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step=15
                //Hover the mouse cursor to the Linked Scrolling button from the menu.

                linkselected = viewer.GetReviewTool("Link Selected");
                viewer.JSMouseHover(linkselected);

                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //A drop down menu is displayed. Link All, Link All Offset, Unlink, Link Selected, Link Selected Offset

                IList<IWebElement> dropdown14 = BasePage.Driver.FindElements(By.CssSelector("li[title='Link Selected'] ul>li"));
                bool flag14 = true;
                title.Clear();

                foreach (IWebElement dropdowntool in dropdown14)
                {
                    title.Add(dropdowntool.GetAttribute("title"));
                    if (!(dropdowntool.Enabled && dropdowntool.Displayed))
                    {
                        flag14 = false;
                        break;
                    }
                }
                if (flag14 && dropdown14.Count == 5 && title.SequenceEqual(link_5))
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

                //Click reset to remove mousehover
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //Step-16
                //Select Link Selected from the list

                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkSelected);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Two windows with four black square windows are displayed.
                //(Square 4 in the reference and Square 2 in the base viewer are currently linked)

                if (viewer.LinkSelectTable(1).Displayed &&
                    viewer.LinkSelectTable(2).Displayed &&
                    viewer.LinkSelectTableCheckBoxList(1).Count == 4 &&
                    viewer.LinkSelectTableCheckBoxList(2).Count == 4 &&
                    viewer.LinkSelectTableCheckBox(2, 2, 1).GetAttribute("class").Contains("SelectedLinkSeries") &&
                    viewer.LinkSelectTableCheckBox(1, 2, 2).GetAttribute("class").Contains("SelectedLinkSeries"))
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

                //Step-17
                //Select square 1 of the base viewer (example-accession #1700795 -Series 1 PET AC 2D)
                //and then green check mark.

                viewer.SelectLinkedCheckBox(1, 1, 2);
                viewer.LinkedScrollingCheckBtn(1).Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //3 series are linked. An anatomical linking icon is displayed on the right top corner of the image.
                //At this point the two series-Series 1 & Series 2 with accession # 1700795 are referred as a Base viewer.

                if (viewer.LinkScrollingStatusImageList().Count == 3 &&
                   viewer.LinkScrollingStatusImage(2, 2, 1).Displayed &&
                   viewer.LinkScrollingStatusImage(1, 1, 2).Displayed &&
                   viewer.LinkScrollingStatusImage(1, 2, 2).Displayed &&
                    viewer.studyPanel(1).GetAttribute("class").Contains("Active") &&
                    viewer.studyPanel(1).GetCssValue("border-top-color").Equals(rgbavalue) &&
                    viewer.SeriesViewer_2X2(1).GetAttribute("class").Contains("activeSeriesViewer") &&
                    viewer.SeriesViewer_2X2(1).GetCssValue("border-top-color").Equals(rgbavalue))
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

                //Step-18
                //Select a middle position on the vertical scrollbar in the reference Series 2.
                //Example - Image 155, 156, 157, 158.

                source = viewer.ViewportScrollHandle(1, 2, 2);
                destination = viewer.ViewportScrollBar(1, 2, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 148 / 311).Release().Build().Perform();
                Thread.Sleep(4000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                loop = 0;
                while (loop < 3)
                {
                    if (Int32.Parse(viewer.SeriesViewer_1X2(2).GetAttribute("imagenum")) == 155)
                        break;
                    else if (Int32.Parse(viewer.SeriesViewer_1X2(2).GetAttribute("imagenum")) < 155)
                        viewer.ClickDownArrowbutton(1, 2, 2);
                    else
                        viewer.ClickUpArrowbutton(1, 2, 2);

                    loop++;
                }
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //In the base viewer Series 1 and Series 2 (left viewer), the image 155 are displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status18 = study.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (status18 &&
                    viewer.SeriesViewer_1X1(2).GetAttribute("imagenum").Equals("155") &&
                    viewer.SeriesViewer_2X2(1).GetAttribute("imagenum").Equals("155"))
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

                //Step-19
                //Select History tab and select a prior with study date 14-Apr-2005 into the third viewer panel.

                viewer.NavigateToHistoryPanel();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //viewer.DoubleClick(viewer.Study(1));
                //StudyDate[1] :14-Apr-2005 1:52:10 PM
                viewer.OpenPriors(new string[] { "Study Date" }, new string[] { StudyDate[1] });
                Thread.Sleep(10000);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);

                //The prior is loaded successfully into the third viewer panel with 4 series and 2x2 layout.

                if (viewer.studyPanel(3).Displayed &&
                    viewer.SeriesViewPorts(3).Count == 4 &&
                    viewer.Thumbnails(3).Count == 4)
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

                //Step-20
                //Continue scrolling to the image 172, 173, 174 and 175 in the reference Series 2 
                //by using the mouse's middle wheel.

                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-21
                //Move to the image 1, 2, 3 and 4 in the reference Series 2 by dragging the mouse cursor.

                source = viewer.ViewportScrollHandle(1, 2, 2);
                destination = viewer.ViewportScrollBar(1, 2, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, 1).Release().Build().Perform();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);

                //In the base viewer Series 1 and Series 2 (left viewer), the image 1 are displayed.
                //There are no changes in the third panel viewer.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status21 = study.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (status21 &&
                    viewer.SeriesViewer_1X1(2).GetAttribute("imagenum").Equals("1") &&
                    viewer.SeriesViewer_2X2(1).GetAttribute("imagenum").Equals("1"))
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

                //Step-22
                //Hover the mouse cursor to the Linked Scrolling button from the menu.

                linkselected = viewer.GetReviewTool("Link Selected");
                viewer.JSMouseHover(linkselected);

                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //A drop down menu is displayed. Link All, Link All Offset, Unlink, Link Selected, Link Selected Offset

                IList<IWebElement> dropdown22 = BasePage.Driver.FindElements(By.CssSelector("li[title='Link Selected'] ul>li"));
                bool flag22 = true;
                title.Clear();

                foreach (IWebElement dropdowntool in dropdown22)
                {
                    title.Add(dropdowntool.GetAttribute("title"));
                    if (!(dropdowntool.Enabled && dropdowntool.Displayed))
                    {
                        flag22 = false;
                        break;
                    }
                }
                if (flag22 && dropdown22.Count == 5 && title.SequenceEqual(link_5))
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
                //Click reset to remove mousehover
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //Step-23
                //Select Link Selected from the list

                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkSelected);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Three windows with four black square windows are displayed.
                //(Square 4 in the reference, Square 1 & 2 in the base viewer are currently linked)

                if (viewer.LinkSelectTable(1).Displayed &&
                    viewer.LinkSelectTable(2).Displayed &&
                    viewer.LinkSelectTable(3).Displayed &&
                    viewer.LinkSelectTableCheckBoxList(1).Count == 4 &&
                    viewer.LinkSelectTableCheckBoxList(2).Count == 4 &&
                    viewer.LinkSelectTableCheckBoxList(3).Count == 4 &&
                    viewer.LinkSelectTableCheckBox(2, 2, 1).GetAttribute("class").Contains("SelectedLinkSeries") &&
                    viewer.LinkSelectTableCheckBox(1, 1, 2).GetAttribute("class").Contains("SelectedLinkSeries") &&
                    viewer.LinkSelectTableCheckBox(1, 2, 2).GetAttribute("class").Contains("SelectedLinkSeries"))
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

                //Step-24
                //Select square 4 of the base viewer (example - accession #1742901 - Series 2 PET NAC 2D) 
                //and then green check mark.

                viewer.SelectLinkedCheckBox(2, 2, 3);

                viewer.ClickElement(viewer.LinkedScrollingCheckBtn(1));
                //viewer.LinkedScrollingCheckBtn(1).Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                viewer.switchToUserHomeFrame();

                //4 series are linked. An anatomical linking icon is displayed on the right top corner of the image.
                //At this point the three series - Series 1 and Series 2 with accession # 1700795 
                //and Series 2 with accession # 1742901 are referred as a Base viewer

                if (viewer.LinkScrollingStatusImageList().Count == 4 &&
                   viewer.LinkScrollingStatusImage(2, 2, 1).Displayed &&
                   viewer.LinkScrollingStatusImage(1, 1, 2).Displayed &&
                   viewer.LinkScrollingStatusImage(1, 2, 2).Displayed &&
                   viewer.LinkScrollingStatusImage(2, 2, 3).Displayed)
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

                //Step-25

                //Select a 1/4 position (from the bottom) on the vertical scrollbar in the reference Series 2.
                //Example - Image 238, 239, 240, 241.

                source = viewer.ViewportScrollHandle(1, 2, 2);
                destination = viewer.ViewportScrollBar(1, 2, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 223 / 311).Release().Build().Perform();
                Thread.Sleep(3000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                loop = 0;
                while (loop < 5)
                {
                    if (Int32.Parse(viewer.SeriesViewer_1X2(2).GetAttribute("imagenum")) == 238)
                        break;
                    else if (Int32.Parse(viewer.SeriesViewer_1X2(2).GetAttribute("imagenum")) < 238)
                        viewer.ClickDownArrowbutton(1, 2, 2);
                    else
                        viewer.ClickUpArrowbutton(1, 2, 2);

                    loop++;
                }
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //In the base viewer Series 1, Series 2 and Series 2 (2 left viewers), the image 238 are displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status25 = study.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (status25 &&
                    viewer.SeriesViewer_2X2(1).GetAttribute("imagenum").Equals("238") &&
                    viewer.SeriesViewer_1X1(2).GetAttribute("imagenum").Equals("238") &&
                    viewer.SeriesViewer_1X2(2).GetAttribute("imagenum").Equals("238") &&
                    viewer.SeriesViewer_2X2(3).GetAttribute("imagenum").Equals("238"))
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

                //Step-26
                //Hover the mouse cursor to the Linked Scrolling button from the menu.

                linkselected = viewer.GetReviewTool("Link Selected");
                viewer.JSMouseHover(linkselected);

                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //A drop down menu is displayed. Link All, Link All Offset, Unlink, Link Selected, Link Selected Offset

                IList<IWebElement> dropdown26 = BasePage.Driver.FindElements(By.CssSelector("li[title='Link Selected'] ul>li"));
                bool flag26 = true;
                title.Clear();

                foreach (IWebElement dropdowntool in dropdown26)
                {
                    title.Add(dropdowntool.GetAttribute("title"));
                    if (!(dropdowntool.Enabled && dropdowntool.Displayed))
                    {
                        flag26 = false;
                        break;
                    }
                }

                if (flag26 && dropdown26.Count == 5 && title.SequenceEqual(link_5))
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
                //Click reset to remove mousehover
                //viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                viewer.switchToUserHomeFrame();

                //Step-27
                //Select Link Selected from the list

                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkSelected);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //Three windows with four black square windows are displayed.
                //(Square 4 in the reference, Square 1 & Square 2 and Square 4 in the base viewer are currently linked)

                if (viewer.LinkSelectTable(1).Displayed &&
                    viewer.LinkSelectTable(2).Displayed &&
                    viewer.LinkSelectTable(3).Displayed &&
                    viewer.LinkSelectTableCheckBoxList(1).Count == 4 &&
                    viewer.LinkSelectTableCheckBoxList(2).Count == 4 &&
                    viewer.LinkSelectTableCheckBoxList(3).Count == 4 &&
                    viewer.LinkSelectTableCheckBox(2, 2, 1).GetAttribute("class").Contains("SelectedLinkSeries") &&
                    viewer.LinkSelectTableCheckBox(1, 1, 2).GetAttribute("class").Contains("SelectedLinkSeries") &&
                    viewer.LinkSelectTableCheckBox(1, 2, 2).GetAttribute("class").Contains("SelectedLinkSeries") &&
                    viewer.LinkSelectTableCheckBox(2, 2, 3).GetAttribute("class").Contains("SelectedLinkSeries"))
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

                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                study.CloseStudy();

                //Logout  -Step-28
                login.Logout();
                ExecutedSteps++;

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
        /// This Test Case is Verification of "Linking Series with multiple priors - Numerical linking with priors"
        /// </summary>

        public TestCaseResult Test_27995(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;

            String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
            String[] AccessionNumbers = AccessionNoList.Split(':');

            String StudyDateList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Date");
            String[] StudyDate = StudyDateList.Split('=');

            try
            {

                //Precondition
                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                Taskbar bar = new Taskbar();
                bar.Hide();

                st.LaunchServiceTool();
                st.NavigateToTab("Linked Scrolling");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);

                if (wpfobject.IsCheckBoxSelected("CB_LinkAll") == false)
                {
                    wpfobject.SelectCheckBox("CB_LinkAll");
                    Logger.Instance.InfoLog("Link All check box is selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Link All check box is already selected");
                }

                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("6");
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();
                bar.Show();


                //Step-1

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();

                //Enable Link All check box
                if (domain.LinkAllCheckbox().Selected == false)
                {
                    domain.LinkAllCheckbox().Click();
                    Logger.Instance.InfoLog("Link scrolling Link All Check box is selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Link scrolling Link All Check box is already selected");
                }
                domain.ClickSaveDomain();

                ExecutedSteps++;


                //Step-2

                ExecutedSteps++;

                //Step-3
                //Load study B, E with accession 1211824 (study date 12-Jan-2005) into the viewer.
                //Select a prior with study date 09-Mar-2005 into the 2-nd viewer panel & study date 14-Apr-2005 into the 3-rd viewer panel.
                //Link the three study panel.(study panel 1: check box-4 (Accession:1211824), study panel 2: check box 1 and 2 (Accession:1700975),study panel 3: check box 4 (Accession:1742901)) and Click Green check mark.

                Studies study = (Studies)login.Navigate("Studies");
                //AccessionNumbers[0] :1211824
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_91);
                study.SelectStudy("Accession", AccessionNumbers[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForAllViewportsToLoad(30);
                viewer.NavigateToHistoryPanel();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //viewer.DoubleClick(viewer.Study(2));
                //StudyDate[0] :09-Mar-2005 11:46:49 AM
                viewer.OpenPriors(new string[] { "Study Date" }, new string[] { StudyDate[0] });

                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                viewer.NavigateToHistoryPanel();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //viewer.DoubleClick(viewer.Study(1));
                //StudyDate[1] :14-Apr-2005 1:52:10 PM
                viewer.OpenPriors(new string[] { "Study Date" }, new string[] { StudyDate[1] });

                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                viewer.SeriesViewer_1X2(2).Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkSelected);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                viewer.SelectLinkedCheckBox(2, 2, 1);
                viewer.SelectLinkedCheckBox(1, 1, 2);
                viewer.SelectLinkedCheckBox(1, 2, 2);
                viewer.SelectLinkedCheckBox(2, 2, 3);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.LinkedScrollingCheckBtn(1)));
                //viewer.LinkedScrollingCheckBtn(1).Click();
                viewer.ClickElement(viewer.LinkedScrollingCheckBtn(1));
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Click down arrow one time

                viewer.ClickDownArrowbutton(1, 2, 2);

                //Three studies are loaded successfully into the viewer with 4 series and 2x2 layout. Four series are numerical linked.

                if (viewer.LinkScrollingStatusImageList().Count == 4 &&
                  viewer.LinkScrollingStatusImage(2, 2, 1).Displayed &&
                  viewer.LinkScrollingStatusImage(1, 1, 2).Displayed &&
                  viewer.LinkScrollingStatusImage(1, 2, 2).Displayed &&
                  viewer.LinkScrollingStatusImage(2, 2, 3).Displayed)
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

                //Step-4

                //Select square 1 of the preference viewer (example - accession #1211824 - Series 1) and then green check mark.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkSelected);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                viewer.SelectLinkedCheckBox(1, 1, 1);
                viewer.LinkedScrollingCheckBtn(2).Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //Five series are numerical linked. A numerical linking icon is displayed on the right top corner of the image.
                //At this point - Series 1 with the accession # 1211824 is numerical link with out of range but the rest of the series are numerical link.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status4 = study.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (status4 && viewer.LinkScrollingStatusImageList().Count == 5 &&
                  viewer.LinkScrollingStatusImage(1, 1, 1).GetAttribute("src").Contains("LinkNumericFar") &&
                  viewer.LinkScrollingStatusImage(1, 1, 1).Displayed &&
                  viewer.LinkScrollingStatusImage(2, 2, 1).Displayed &&
                  viewer.LinkScrollingStatusImage(1, 1, 2).Displayed &&
                  viewer.LinkScrollingStatusImage(1, 2, 2).Displayed &&
                  viewer.LinkScrollingStatusImage(2, 2, 3).Displayed)
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


                //Step-5
                //Hover the mouse cursor to the Linked Scrolling button from the menu

                IWebElement linkselected = viewer.GetReviewTool("Link Selected");
                viewer.JSMouseHover(linkselected);

                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //A drop down menu is displayed. Link All, Link All Offset, Unlink, Link Selected, Link Selected Offset

                IList<IWebElement> dropdown5 = BasePage.Driver.FindElements(By.CssSelector("li[title='Link Selected'] ul>li"));
                IList<String> title = new List<String>();

                bool flag5 = true;
                foreach (IWebElement dropdowntool in dropdown5)
                {
                    title.Add(dropdowntool.GetAttribute("title"));

                    if (!(dropdowntool.Enabled && dropdowntool.Displayed))
                    {
                        flag5 = false;
                        break;
                    }
                }

                if (flag5 && dropdown5.Count == 5 && title.SequenceEqual(link_5))
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
                //Click reset to remove mousehover
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Step-6
                //Select Unlink from the list
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Unlink);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //All the series are unlinked.
                if (viewer.LinkScrollingStatusImageList().Count == 0)
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

                //Step-7
                //Select Link All from the list

                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkAll);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //All the series are numerical linked.  Note that Series 1 with accession # 1211824 
                //and Series 1 with accession # 1742901 are numerical link with out of range.

                if (viewer.LinkScrollingStatusImageList().Count == 10 &&
                   viewer.LinkScrollingStatusImage(1, 1, 1).GetAttribute("src").Contains("LinkNumericFar") &&
                   viewer.LinkScrollingStatusImage(1, 1, 3).GetAttribute("src").Contains("LinkNumericFar"))
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

                //Step-8

                //Select a 1/4 position (from the top) on the vertical scrollbar in the reference Series 2.
                //Example- Image 69, 70, 71, 72.

                IWebElement source = viewer.ViewportScrollHandle(1, 2, 2);
                IWebElement destination = viewer.ViewportScrollBar(1, 2, 2);

                int w = destination.Size.Width;
                int h = destination.Size.Height;

                Actions action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 70 / 311).Release().Build().Perform();
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                int loop = 0;
                while (loop < 3)
                {
                    if (Int32.Parse(viewer.SeriesViewer_1X2(2).GetAttribute("imagenum")) == 69)
                        break;
                    else if (Int32.Parse(viewer.SeriesViewer_1X2(2).GetAttribute("imagenum")) < 69)
                        viewer.ClickDownArrowbutton(1, 2, 2);
                    else
                        viewer.ClickUpArrowbutton(1, 2, 2);

                    loop++;
                }
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);


                //All the base viewer Series are displayed with image 69 except Series 1 with the accession # 1211824 
                //and Series 1 with accession # 1742901 are unchanged (out of range).

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status8 = study.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (status8 && viewer.LinkScrollingStatusImageList().Count == 10 &&
                  viewer.LinkScrollingStatusImage(1, 1, 1).GetAttribute("src").Contains("LinkNumericFar") &&
                  viewer.LinkScrollingStatusImage(1, 1, 3).GetAttribute("src").Contains("LinkNumericFar") &&
                  viewer.SeriesViewer_1X2(1).GetAttribute("imagenum").Equals("69") &&
                  viewer.SeriesViewer_2X1(1).GetAttribute("imagenum").Equals("69") &&
                  viewer.SeriesViewer_2X2(1).GetAttribute("imagenum").Equals("69") &&
                  viewer.SeriesViewer_1X1(2).GetAttribute("imagenum").Equals("69") &&
                  viewer.SeriesViewer_1X2(2).GetAttribute("imagenum").Equals("69") &&
                  viewer.SeriesViewer_1X2(3).GetAttribute("imagenum").Equals("69") &&
                  viewer.SeriesViewer_2X1(3).GetAttribute("imagenum").Equals("69") &&
                  viewer.SeriesViewer_2X2(3).GetAttribute("imagenum").Equals("69"))
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

                //Step-9
                //Select another reference series (example - Series 1 with accession # 1700795)

                viewer.SeriesViewer_1X1(2).Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //The enclosed orange window is displayed around the Series 1 with accession # 1700795.

                if (viewer.SeriesViewer_1X1(2).GetAttribute("class").Contains("activeSeriesViewer") &&
                  viewer.SeriesViewer_1X1(2).GetCssValue("border-top-color").Equals(rgbavalue))
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

                //Step-10

                //Select a end position point (bottom) on the vertical scrollbar in the reference Series 1.
                //Example - Image 311.

                source = viewer.ViewportScrollHandle(1, 1, 2);
                destination = viewer.ViewportScrollBar(1, 1, 2);

                w = destination.Size.Width;
                h = destination.Size.Height;

                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h).Release().Build().Perform();
                Thread.Sleep(5000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(5000);
                action.ClickAndHold(source).MoveToElement(destination, w / 2, h).Release().Build().Perform();
                Thread.Sleep(5000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                Thread.Sleep(5000);

                PageLoadWait.WaitForAllViewportsToLoad(30, 1);
                PageLoadWait.WaitForAllViewportsToLoad(30, 2);
                PageLoadWait.WaitForAllViewportsToLoad(30, 3);

                PageLoadWait.WaitForAttributeInViewport(1, 2, "imagenum", "311", studyPanelIndex: 3);

                //All the base viewer Series are displayed with image 311 
                //except Series 1 with the accession # 1211824 and Series 1 with accession # 1742901 are unchanged (out of range).

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status10 = study.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (status10 && viewer.LinkScrollingStatusImageList().Count == 10 &&
                  viewer.LinkScrollingStatusImage(1, 1, 1).GetAttribute("src").Contains("LinkNumericFar") &&
                  viewer.LinkScrollingStatusImage(1, 1, 3).GetAttribute("src").Contains("LinkNumericFar") &&
                  viewer.SeriesViewer_1X2(1).GetAttribute("imagenum").Equals("311") &&
                  viewer.SeriesViewer_2X1(1).GetAttribute("imagenum").Equals("311") &&
                  viewer.SeriesViewer_2X2(1).GetAttribute("imagenum").Equals("311") &&
                  viewer.SeriesViewer_1X1(2).GetAttribute("imagenum").Equals("311") &&
                  viewer.SeriesViewer_1X2(2).GetAttribute("imagenum").Equals("311") && //*dt
                  viewer.SeriesViewer_1X2(3).GetAttribute("imagenum").Equals("311") &&
                  viewer.SeriesViewer_2X1(3).GetAttribute("imagenum").Equals("311") &&
                  viewer.SeriesViewer_2X2(3).GetAttribute("imagenum").Equals("311"))
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

                //Step-11
                //Select Unlink from the list

                viewer.SelectToolInToolBar(IEnum.ViewerTools.Unlink);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //All the series are unlinked.

                if (viewer.LinkScrollingStatusImageList().Count == 0)
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


                //Logout  -Step-12
                login.Logout();
                ExecutedSteps++;

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
                //login.Logout();

                //Return Result
                return result;
            }
        }


        /// <summary> 
        /// This Test Case is Verification of "Offset Linked Series with priors"
        /// </summary>

        public TestCaseResult Test_27996(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;

            //Set up Validation Steps
            result.SetTestStepDescription(teststeps);
            String username = Config.adminUserName;
            String password = Config.adminPassword;

            String AccessionNoList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
            String[] AccessionNumbers = AccessionNoList.Split(':');

            String StudyDateList = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Date");
            String[] StudyDate = StudyDateList.Split('=');

            try
            {
                //Precondition--

                //Link All in service tool 
                ServiceTool st = new ServiceTool();
                WpfObjects wpfobject = new WpfObjects();
                Taskbar bar = new Taskbar();
                bar.Hide();

                st.LaunchServiceTool();
                st.NavigateToTab("Linked Scrolling");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);

                if (wpfobject.IsCheckBoxSelected("CB_LinkAll") == false)
                {
                    wpfobject.SelectCheckBox("CB_LinkAll");
                    Logger.Instance.InfoLog("Link All check box is selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Link All check box is already selected");
                }

                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("6");
                wpfobject.WaitTillLoad();
                st.RestartService();
                wpfobject.WaitTillLoad();
                st.CloseServiceTool();
                wpfobject.WaitTillLoad();

                bar.Show();

                //Step-1
                //Complete steps in Initial Setups test case.

                login.DriverGoTo(login.url);
                login.LoginIConnect(username, password);

                DomainManagement domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();

                //Enable Link All check box
                if (domain.LinkAllCheckbox().Selected == false)
                {
                    domain.LinkAllCheckbox().Click();
                    Logger.Instance.InfoLog("Link scrolling Link All Check box is selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Link scrolling Link All Check box is already selected");
                }
                domain.ClickSaveDomain();
                ExecutedSteps++;

                //Step-2
                //Load study B, E with accession 1211824 (study date 12-Jan-2005) into the viewer.
                //Select a prior with study date 09-Mar-2005 into the 2-nd viewer panel & study date 14-Apr-2005 into the 3-rd viewer panel

                Studies study = (Studies)login.Navigate("Studies");
                //AccessionNumbers[0]=1211824
                study.SearchStudy(AccessionNo: AccessionNumbers[0], Datasource: EA_91);
                study.SelectStudy("Accession", AccessionNumbers[0]);
                StudyViewer viewer = StudyViewer.LaunchStudy();
                PageLoadWait.WaitForAllViewportsToLoad(30);
                viewer.NavigateToHistoryPanel();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //viewer.DoubleClick(viewer.Study(2));
                //StudyDate[0] :09-Mar-2005 11:46:49 AM
                viewer.OpenPriors(new string[] { "Study Date" }, new string[] { StudyDate[0] });
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                viewer.NavigateToHistoryPanel();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                //viewer.DoubleClick(viewer.Study(1));
                //StudyDate[1] :14-Apr-2005 1:52:10 PM
                viewer.OpenPriors(new string[] { "Study Date" }, new string[] { StudyDate[1] });
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.SeriesViewer_1X2(2)));
                viewer.SeriesViewer_1X2(2).Click();
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.ImageLayout2x2);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //Three studies are loaded successfully into the viewer with 4 series.
                if (viewer.studyPanel(1).Displayed &&
                    viewer.studyPanel(2).Displayed &&
                    viewer.studyPanel(3).Displayed &&
                    viewer.SeriesViewPorts(1).Count == 4 &&
                    viewer.SeriesViewPorts(2).Count == 4 &&
                    viewer.SeriesViewPorts(3).Count == 4)
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

                //Step-3
                //All series images are set back to Image 1.
                //Select Series 2 Image 78 with accession # 1211824.
                //Select Series 1 Image 68 with accession # 1700795.
                //Select Series 1 Image 58 with accession # 1742901 --> Reference viewer.
                //Hover the mouse cursor to the Linked Scrolling button from the menu.

                //Select Series 2 Image 78 with accession # 1211824.
                IWebElement source = viewer.ViewportScrollHandle(1, 2, 1);
                IWebElement destination = viewer.ViewportScrollBar(1, 2, 1);
                int w = destination.Size.Width;
                int h = destination.Size.Height;
                Actions action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 78 / 311).Release().Build().Perform();
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                int loop = 0;
                while (loop < 3)
                {
                    if (Int32.Parse(viewer.SeriesViewer_1X2(1).GetAttribute("imagenum")) == 78)
                        break;
                    else if (Int32.Parse(viewer.SeriesViewer_1X2(1).GetAttribute("imagenum")) < 78)
                        viewer.ClickDownArrowbutton(1, 2, 1);
                    else
                        viewer.ClickUpArrowbutton(1, 2, 1);

                    loop++;
                }

                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                //Select Series 1 Image 68 with accession # 1700795.

                source = viewer.ViewportScrollHandle(1, 1, 2);
                destination = viewer.ViewportScrollBar(1, 1, 2);
                w = destination.Size.Width;
                h = destination.Size.Height;
                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 68 / 311).Release().Build().Perform();
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                loop = 0;
                while (loop < 3)
                {
                    if (Int32.Parse(viewer.SeriesViewer_1X1(2).GetAttribute("imagenum")) == 68)
                        break;
                    else if (Int32.Parse(viewer.SeriesViewer_1X1(2).GetAttribute("imagenum")) < 68)
                        viewer.ClickDownArrowbutton(1, 1, 2);
                    else
                        viewer.ClickUpArrowbutton(1, 1, 2);

                    loop++;
                }
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                //Select Series 1 Image 58 with accession # 1742901

                source = viewer.ViewportScrollHandle(2, 1, 3);
                destination = viewer.ViewportScrollBar(2, 1, 3);
                w = destination.Size.Width;
                h = destination.Size.Height;
                action = new Actions(BasePage.Driver);

                action.ClickAndHold(source).MoveToElement(destination, w / 2, h * 59 / 311).Release().Build().Perform();
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                loop = 0;
                while (loop < 3)
                {
                    if (Int32.Parse(viewer.SeriesViewer_2X1(3).GetAttribute("imagenum")) == 58)
                        break;
                    else if (Int32.Parse(viewer.SeriesViewer_2X1(3).GetAttribute("imagenum")) < 58)
                        viewer.ClickDownArrowbutton(2, 1, 3);
                    else
                        viewer.ClickUpArrowbutton(2, 1, 3);

                    loop++;
                }
                Thread.Sleep(2000);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);

                viewer.SeriesViewer_2X1(3).Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);


                //Hover the mouse cursor to the Linked Scrolling button from the menu.

                IWebElement linkselected = viewer.GetReviewTool("Link Selected");
                viewer.JSMouseHover(linkselected);

                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);


                //A drop down menu is displayed. Link All, Link All Offset, Unlink, Link Selected, Link Selected Offset

                IList<IWebElement> dropdown_5 = BasePage.Driver.FindElements(By.CssSelector("li[title='Link Selected'] ul>li"));
                IList<String> title = new List<String>();

                bool flag3 = true;
                foreach (IWebElement dropdowntool in dropdown_5)
                {
                    title.Add(dropdowntool.GetAttribute("title"));

                    if (!(dropdowntool.Enabled && dropdowntool.Displayed))
                    {
                        flag3 = false;
                        break;
                    }
                }
                if (flag3 && dropdown_5.Count == 5 && title.SequenceEqual(link_5) &&
                    viewer.SeriesViewer_1X2(1).GetAttribute("imagenum").Equals("78") &&
                    viewer.SeriesViewer_1X1(2).GetAttribute("imagenum").Equals("68") &&
                    viewer.SeriesViewer_2X1(3).GetAttribute("imagenum").Equals("58") &&
                    viewer.SeriesViewer_2X1(3).GetAttribute("class").Contains("activeSeriesViewer") &&
                    viewer.SeriesViewer_2X1(3).GetCssValue("border-top-color").Equals(rgbavalue))
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
                //Click reset to remove mousehover
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);

                //Step-4
                //Select Link Selected Offset from the list

                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkSelectedOffset);
                PageLoadWait.WaitForPageLoad(5);
                PageLoadWait.WaitForFrameLoad(5);

                //Three windows with four black square windows are displayed.

                if (viewer.LinkSelectTable(1).Displayed &&
                    viewer.LinkSelectTable(2).Displayed &&
                    viewer.LinkSelectTable(3).Displayed &&
                    viewer.LinkSelectTableCheckBoxList(1).Count == 4 &&
                    viewer.LinkSelectTableCheckBoxList(2).Count == 4 &&
                    viewer.LinkSelectTableCheckBoxList(3).Count == 4)
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

                //Step-5
                //Select window 2 (Series 2 Image 78), window 1 (Series 1 Image 68) and
                //window 3(Series 1 Image 58 --> reference viewer) and then green check mark

                viewer.SelectLinkedCheckBox(1, 2, 1);
                viewer.SelectLinkedCheckBox(1, 1, 2);
                viewer.SelectLinkedCheckBox(2, 1, 3);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(viewer.LinkedScrollingCheckBtn(3)));
                //viewer.LinkedScrollingCheckBtn(3).Click();
                viewer.ClickElement(viewer.LinkedScrollingCheckBtn(3));
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //Three series are linked with offset.

                if (viewer.LinkScrollingStatusImageList().Count == 3 &&
                    viewer.LinkScrollingStatusImage(1, 2, 1).Displayed &&
                    viewer.LinkScrollingStatusImage(1, 1, 2).Displayed &&
                    viewer.LinkScrollingStatusImage(2, 1, 3).Displayed)
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

                //Step-6
                //Scroll down to the image 68 in the reference Series 1 (accession # 1742901) by using down triangle.

                for (int i = 0; i < 10; i++)
                {
                    viewer.ClickDownArrowbutton(2, 1, 3);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    PageLoadWait.WaitForAllViewportsToLoad(20);
                }

                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                //In the base viewer Series 1 (accession # 1700795), the image 78 is displayed.
                //In the base viewer Series 2 (accession # 1211824), the image 88 is displayed.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status6 = study.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (status6 && viewer.SeriesViewer_1X1(2).GetAttribute("imagenum").Equals("78") &&
                               viewer.SeriesViewer_1X2(1).GetAttribute("imagenum").Equals("88"))
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


                //Step-7
                //Scroll down to the image 220 in the reference Series 1 (accession # 1742901) by dragging the mouse cursor.

                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-8
                //Hover the mouse cursor to the Linked Scrolling button from the menu.

                linkselected = viewer.GetReviewTool("Link Selected Offset");
                viewer.JSMouseHover(linkselected);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //A drop down menu is displayed. Link All, Link All Offset, Unlink, Link Selected, Link Selected Offset

                IList<IWebElement> dropdown8 = BasePage.Driver.FindElements(By.CssSelector("li[title='Link Selected Offset'] ul>li"));
                title.Clear();
                bool flag8 = true;
                foreach (IWebElement dropdowntool in dropdown8)
                {
                    title.Add(dropdowntool.GetAttribute("title"));
                    if (!(dropdowntool.Enabled && dropdowntool.Displayed))
                    {
                        flag8 = false;
                        break;
                    }
                }

                if (flag8 && dropdown8.Count == 5 && title.SequenceEqual(link_5))
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

                //Click reset to remove mousehover
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //Step-9
                //Select Link All Offset from the list

                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkAllOffset);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //All the series are numerical linked with offset
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status9 = study.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (status9 && viewer.LinkScrollingStatusImageList().Count == 10)
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

                //Step-10
                //Scroll down to the image 250 in the reference Series 1 (accession # 1742901) by dragging the mouse cursor.
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-11
                //Select Unlink from the list
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Unlink);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);

                //All the series are unlinked.
                if (viewer.LinkScrollingStatusImageList().Count == 0)
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


                //Logout  -Step-12
                login.Logout();
                ExecutedSteps++;

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
                //login.Logout();

                //Return Result
                return result;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_119547(String testid, String teststeps, int stepcount)
        {

            //Decalre Variables

            TestCaseResult result;
            Studies studies;
            DomainManagement domain;
            StudyViewer viewer = new StudyViewer();
            WpfObjects wpfobject = new WpfObjects();
            ServiceTool servicetool = new ServiceTool();
            Random randomnumber = new Random();
            String searchname = "Test" + randomnumber.Next(100, 10000);
            String lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "LastName");
            String patientid = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientID");
            //Set Test step description  
            result = new TestCaseResult(stepcount);

            int ExecutedSteps = -1;
            result.SetTestStepDescription(teststeps);
            try
            {
                // Pre-condition
                servicetool.LaunchServiceTool();
                servicetool.NavigateToTab("Linked Scrolling");
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Modify", 1);

                if (wpfobject.IsCheckBoxSelected("CB_LinkAll") == false)
                {
                    wpfobject.SelectCheckBox("CB_LinkAll");
                    Logger.Instance.InfoLog("Link All check box is selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Link All check box is already selected");
                }
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("Apply", 1);
                wpfobject.WaitTillLoad();
                wpfobject.ClickButton("6");
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                wpfobject.WaitTillLoad();
                servicetool.RestartService();
                servicetool.CloseServiceTool();
                wpfobject.WaitTillLoad();

                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);

                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SelectDomain("SuperAdminGroup");
                domain.ClickEditDomain();

                //Enable Link All check box
                if (domain.LinkAllCheckbox().Selected == false)
                {
                    domain.LinkAllCheckbox().Click();
                    Logger.Instance.InfoLog("Link scrolling Link All Check box is selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("Link scrolling Link All Check box is already selected");
                }
                domain.ClickSaveDomain();

                //Step-1
                // 1. Load a study Amaya Sharon.
                String Full_Name = "AMAYA, SHARON ";
                String Last_Name = "amaya";
                String First_Name = "sharon";

                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy(LastName: Last_Name, FirstName: First_Name, Datasource: EA_131);
                studies.SelectStudy("Patient Name", Full_Name);
                viewer = studies.LaunchStudy();

                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);

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

                // Step 2
                // Load series 1794 and 1795 in viewport1 and viewport2.
                viewer.SeriesViewer_1X1().Click();
                var action4 = new Actions(BasePage.Driver);
                action4.DoubleClick(viewer.ThumbnailCaptions()[4]).Build().Perform();
                PageLoadWait.WaitForAllViewportsToLoad(30);

                viewer.SeriesViewer_1X2().Click();
                action4.DoubleClick(viewer.ThumbnailCaptions()[5]).Build().Perform();
                PageLoadWait.WaitForAllViewportsToLoad(30);

                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status2 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

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



                // STep 3
                // Hover the mouse cursor to the Linked Scrolling button from the menu.
                //Hover the mouse cursor to the Linked Scrolling button from the menu.

                IWebElement linkselected = viewer.GetReviewTool("Link Selected");
                viewer.JSMouseHover(linkselected);

                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                //A drop down menu is displayed. Link All, Link All Offset, Unlink, Link Selected, Link Selected Offset

                IList<IWebElement> dropdown_5 = BasePage.Driver.FindElements(By.CssSelector("li[title='Link Selected'] ul>li"));
                IList<String> title = new List<String>();

                bool flag3 = true;
                foreach (IWebElement dropdowntool in dropdown_5)
                {
                    title.Add(dropdowntool.GetAttribute("title"));

                    if (!(dropdowntool.Enabled && dropdowntool.Displayed))
                    {
                        flag3 = false;
                        break;
                    }
                }
                if (flag3 && dropdown_5.Count == 5 && title.SequenceEqual(link_5))
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

                //Select Reset for remove mouse hover.
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Pan);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.Reset);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                // Step 4
                //Select Link All.

                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkAll);
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);
                PageLoadWait.WaitForPageLoad(25);
                PageLoadWait.WaitForFrameLoad(25);
                PageLoadWait.WaitForAllViewportsToLoad(30);
                viewer.SelectToolInToolBar(IEnum.ViewerTools.LinkSelected);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);

                if (viewer.LinkScrollingStatusImageList().Count == 4 &&

                 viewer.LinkScrollingStatusImage(1, 1).Displayed &&
                 viewer.LinkScrollingStatusImage(1, 2).Displayed &&
                 viewer.LinkScrollingStatusImage(2, 1).Displayed &&

                 viewer.LinkScrollingStatusImage(2, 2).Displayed)
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

                // Step 5
                // 5. Linke both viewports and scrolled to index 10.
                viewer.SelectLinkedCheckBox(2, 1);
                viewer.SelectLinkedCheckBox(2, 2);
                viewer.LinkedScrollingCheckBtn().Click();
                PageLoadWait.WaitForPageLoad(15);
                PageLoadWait.WaitForFrameLoad(15);

                for (int i = 0; i < 10; i++)
                {
                    viewer.ClickDownArrowbutton(1, 2);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    PageLoadWait.WaitForAllViewportsToLoad(20);
                }

                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                if (viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("3") &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("11") &&
                    viewer.SeriesViewer_2X1().GetAttribute("imagenum").Equals("11") &&
                    viewer.SeriesViewer_2X2().GetAttribute("imagenum").Equals("3"))
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


                // Step 6
                // Double click viewport1 to maximize in 1x1. // perform sameaction in viewport 3(because no of image 3 in viewport 1)
                action4.DoubleClick(viewer.SeriesViewer_2X1()).Build().Perform();
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(30);
                PageLoadWait.WaitForFrameLoad(30);
                PageLoadWait.WaitForThumbnailsToLoad(60);
                PageLoadWait.WaitForAllViewportsToLoad(60);

                if (viewer.SeriesViewPorts().Count == 1 &&
                    viewer.SeriesViewer_1X1().Displayed == true &&
                    viewer.SeriesViewer_1X2().Displayed == false &&
                    viewer.SeriesViewer_2X1().Displayed == false &&
                    viewer.SeriesViewer_2X2().Displayed == false)
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

                // Step 7
                // Applied WL and scrolled to index 30.
                viewer.SelectToolInToolBar(IEnum.ViewerTools.WindowLevel);
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement viewport1 = viewer.SeriesViewer_1X1();
                int h = viewport1.Size.Height;
                int w = viewport1.Size.Width;

                viewer.DragandDropImage(viewport1, w / 4, h / 4, 3 * w / 4, 3 * h / 4);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                for (int i = 0; i < 19; i++)
                {
                    viewer.ClickDownArrowbutton(1, 1);
                    PageLoadWait.WaitForPageLoad(20);
                    PageLoadWait.WaitForFrameLoad(20);
                    PageLoadWait.WaitForAllViewportsToLoad(20);
                }

                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                PageLoadWait.WaitForAllViewportsToLoad(20);

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status7 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());


                if (viewer.SeriesViewer_2X1().GetAttribute("imagenum").Equals("30") &&
                    status7)
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

                // Step 8
                // Double click to go back to 2x2 view.
                action4.DoubleClick(viewer.SeriesViewer_1X1()).Build().Perform();

                if (viewer.SeriesViewPorts().Count == 4 &&
                   viewer.SeriesViewer_1X1().Displayed == true &&
                   viewer.SeriesViewer_1X2().Displayed == true &&
                   viewer.SeriesViewer_2X1().Displayed == true &&
                   viewer.SeriesViewer_2X2().Displayed == true)
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


                // Step 9
                // Check both linked view ports show image # 30 and scroll handle position is the same.

                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps + 1);
                bool status9 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.StudyPanelContainer());

                if (viewer.SeriesViewer_1X1().GetAttribute("imagenum").Equals("3") &&
                    viewer.SeriesViewer_1X2().GetAttribute("imagenum").Equals("30") &&
                    viewer.SeriesViewer_2X1().GetAttribute("imagenum").Equals("30") &&
                    viewer.SeriesViewer_2X2().GetAttribute("imagenum").Equals("3") &&
                   status9)
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

                //Report Result                
                login.Logout();
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Results
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
