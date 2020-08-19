using System;
using System.Globalization;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Reusable.Generic;
using System.IO;
using OpenQA.Selenium;
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
using System.Xml.Serialization;
using OpenQA.Selenium.Remote;
using System.Text.RegularExpressions;

namespace Selenium.Scripts.Tests
{
    class ConferenceListBR
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public MpacLogin mpaclogin { get; set; }
        public HPHomePage hphomepage { get; set; }
        public HPLogin hplogin { get; set; }
        public ServiceTool servicetool { get; set; }
        public WpfObjects wpfobject { get; set;}


        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        /// <param name="classname"></param>
        public ConferenceListBR(String classname)
        {
            login = new Login();
            login.DriverGoTo(login.url);
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ei = new ExamImporter();
            mpaclogin = new MpacLogin();
            wpfobject = new WpfObjects();
        }

        /// <summary>
        /// Launching conference study in Universal viewer
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_161306(String testid, String teststeps, int stepcount)
        {         

            //Declare and initialize variables              
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedsteps = -1;
            Random random = new Random();
            int limit = Math.Abs((int)DateTime.Now.Ticks);
            limit = Int32.Parse(limit.ToString().Substring(0, 4));
            String description = "Desc" + random.Next(1, limit);

            //DomainC, RoleC and User
            String domainA = String.Empty;
            String domainadminA = String.Empty;
            String passwordA = String.Empty;
            String roleA = "RoleA" + BasePage.GetUniqueRole();
            String user = "UserA" + BasePage.GetUniqueUserId();            

            //Folders
            String topfoldername1 = "Top1" + new System.DateTime().Millisecond + random.Next(100, 10000);
            String subfoldername11 = "Sub11" + new System.DateTime().Millisecond + random.Next(100, 10000);
            String foldermanager = String.Empty;
            String description1 = "Desc1" + new System.DateTime().Millisecond + random.Next(100, 10000);
            String notes1 = "Notes1" + new System.DateTime().Millisecond + random.Next(100, 10000);
            String folderpath1 = topfoldername1;          
            String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");           
            String[] arraccession = accession.Split(':');            
            folderpath1 = topfoldername1 + "/" + subfoldername11;

            try
            {
                //Precondition
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domain = login.Navigate<DomainManagement>();
                var domainattr = domain.CreateDomainAttr();
                domainA = domainattr[DomainManagement.DomainAttr.DomainName];
                domainadminA = domainattr[DomainManagement.DomainAttr.UserID];
                passwordA = domainattr[DomainManagement.DomainAttr.Password];
                domain.CreateDomain(domainattr, isconferenceneeded: true);

                //Create Role role1
                var rolemgmt = login.Navigate<RoleManagement>();
                rolemgmt.CreateRole(domainA, roleA, "Conference");

                //Create user           
                var usermgmt = login.Navigate<UserManagement>();
                usermgmt.CreateUser(user, domainA, roleA);

                //Set UV as default viewer
                login.LoginIConnect(user, user);
                var userpref = login.OpenUserPreferences();
                userpref.SwitchToUserPrefFrame();
                userpref.SetRadioButton(userpref.BluringViewerRadioBtn());
                userpref.CloseUserPreferences();
                login.Logout();
                login.LoginIConnect(domainadminA, passwordA);
                userpref = login.OpenUserPreferences();
                userpref.SwitchToUserPrefFrame();
                userpref.SetRadioButton(userpref.BluringViewerRadioBtn());
                userpref.CloseUserPreferences();

                //Step-1        
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);                
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(domainA);
                domain.SelectDomain(domainA);
                domain.ClickEditDomain();
                if (domain.ConferenceListsCB().Selected == false)
                {
                    domain.ConferenceListsCB().Click();
                    Logger.Instance.InfoLog("ConferenceLists Check Box Selected successfully");
                }
                else
                {
                    Logger.Instance.InfoLog("ConferenceLists Check Box already selected");
                }
                domain.ClickSaveNewDomain();
                executedsteps++;

                //Step-2   
                login.Logout();
                login.LoginIConnect(domainadminA, passwordA);
                if (login.IsTabPresent("Conference Folders"))
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-3
                rolemgmt = login.Navigate<RoleManagement>();               
                rolemgmt.SearchRole(roleA);
                rolemgmt.SelectRole(roleA);
                rolemgmt.ClickEditRole();
                if (!rolemgmt.ConferenceUserCB().Selected)
                {
                    rolemgmt.ConferenceUserCB().Click();
                }
                executedsteps++;


                //Step-4 - Check Conference Tab
                login.Logout();
                login.LoginIConnect(user, user);
                if (login.IsTabPresent("Conference Folders"))
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-5 & 6 - Create Folders
                login.Logout();
                login.LoginIConnect(domainadminA, passwordA);
                var conference = login.Navigate<ConferenceFolders>();
                conference.CreateToplevelFolder(topfoldername1);                
                conference.CreateSubFolder(topfoldername1, subfoldername11);
                executedsteps++;
                executedsteps++;

                //Step-7
                var studies = login.Navigate<Studies>();
                studies.SearchStudy(AccessionNo: arraccession[0], Datasource: studies.GetHostName(Config.EA1));       
                executedsteps++;

                //Step-8
                var isUniversalbtn = BasePage.FindElementByCss(BluRingViewer.btn_bluringviewer).Displayed;
                var isEnterprisebtn = BasePage.FindElementByCss("input#m_enterpriseViewStudyButton").Displayed;
                if(isUniversalbtn && isEnterprisebtn)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-9
                studies.SelectStudy("Accession", arraccession[0]);                
                var evviewer = studies.LaunchStudy();
                executedsteps++;

                //Step-10, 11
                evviewer.SelectToolInToolBar(IEnum.ViewerTools.AddToConferenceFolder);
                evviewer.AddStudyToStudyFolder(folderpath1);
                studies.CloseStudy();
                executedsteps++;
                executedsteps++;

                //Step-12
                conference = login.Navigate<ConferenceFolders>();
                conference.ExpandAndSelectFolder(folderpath1);
                executedsteps++;

                //Step-13
                conference.SelectStudy("Accession", arraccession[0]);
                isUniversalbtn = BasePage.FindElementByCss(ConferenceFolders.btnUniversalviewer).Displayed;
                isEnterprisebtn = BasePage.FindElementByCss(ConferenceFolders.btnEnterpriseviewer).Displayed;
                if (isUniversalbtn && isEnterprisebtn)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }
                
                //Step-14
                var uvviewer = BluRingViewer.LaunchBluRingViewer(tabname:"conference");
                var step14 = result.steps[++executedsteps];
                step14.SetPath(testid, executedsteps);
                var isImageCorrect14 = uvviewer.CompareImage(step14, BasePage.FindElementByCss(uvviewer.Activeviewport));
                if(isImageCorrect14)
                {
                    result.steps[executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-15
                uvviewer.CloseBluRingViewer();
                executedsteps++;

                //Step-16                
                conference.SelectStudy1("Accession", arraccession[0], dblclick: true);
                PageLoadWait.WaitForFrameLoad(5);
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitForPriorsToLoad();
                var step16 = result.steps[++executedsteps];
                step16.SetPath(testid, executedsteps);
                var isImageCorrect16 = uvviewer.CompareImage(step16, BasePage.FindElementByCss(uvviewer.Activeviewport));
                if (isImageCorrect16)
                {
                    result.steps[executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-17
                uvviewer.CloseBluRingViewer();                
                executedsteps++;

                //Step-18
                conference.ArchiveConferenceFolder(folderpath1);
                var isfoldrepresent18_1 = conference.ExpandAndSelectFolder(folderpath1) == null? true : false;
                conference.NavigateToArchiveMode();
                var isfoldrepresent18_2 = conference.ExpandAndSelectFolder(folderpath1)!= null ? true : false;
                if(isfoldrepresent18_1 && isfoldrepresent18_2)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-19
                var study = conference.GetMatchingRow("Accession", arraccession[0]);
                if(study!=null)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-20
                conference.SelectStudy("Accession", arraccession[0]);
                isUniversalbtn = BasePage.FindElementByCss(ConferenceFolders.btnUniversalviewer).Displayed;
                isEnterprisebtn = BasePage.FindElementByCss(ConferenceFolders.btnEnterpriseviewer).Displayed;
                if (isUniversalbtn && isEnterprisebtn)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-21
                uvviewer = BluRingViewer.LaunchBluRingViewer(tabname:"conference");
                var step21 = result.steps[++executedsteps];
                step21.SetPath(testid, executedsteps);
                var isImageCorrect21 = uvviewer.CompareImage(step21, BasePage.FindElementByCss(uvviewer.Activeviewport));
                if (isImageCorrect21)
                {
                    result.steps[executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-22
                uvviewer.CloseBluRingViewer();
                conference.SelectStudy1("Accession", arraccession[0], dblclick: true);
                PageLoadWait.WaitForFrameLoad(5);
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitForPriorsToLoad();
                var step22 = result.steps[++executedsteps];
                step22.SetPath(testid, executedsteps);
                var isImageCorrect22 = uvviewer.CompareImage(step22, BasePage.FindElementByCss(uvviewer.Activeviewport));
                uvviewer.CloseBluRingViewer();
                if (isImageCorrect22)
                {
                    result.steps[executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-23-Undo Archive
                PageLoadWait.WaitForFrameLoad(5);
                conference.UndoArchiveFolder(folderpath1);
                bool isFolderRemoved = false;
                try { conference.ExpandAndSelectFolder(folderpath1).Text.Equals(subfoldername11); isFolderRemoved = false; }
                catch (Exception) { isFolderRemoved = true; }
                conference.NavigateToActiveMode();
                bool isFolderRestored = conference.ExpandAndSelectFolder(folderpath1).Text.Equals(subfoldername11);
                if (isFolderRemoved && isFolderRestored)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }


                //Step-24
                login.Logout();
                login.LoginIConnect(user, user);
                login.Navigate<ConferenceFolders>();
                conference.ExpandAndSelectFolder(folderpath1);
                conference.SelectStudy1("Accession", arraccession[0], dblclick: true);
                PageLoadWait.WaitForFrameLoad(5);
                BluRingViewer.WaitforViewports();
                BluRingViewer.WaitforThumbnails();
                BluRingViewer.WaitForPriorsToLoad();
                var step24 = result.steps[++executedsteps];
                step24.SetPath(testid, executedsteps);
                var isImageCorrect24 = uvviewer.CompareImage(step24, BasePage.FindElementByCss(uvviewer.Activeviewport));
                uvviewer.CloseBluRingViewer();
                if (isImageCorrect24)
                {
                    result.steps[executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Logout   
                login.Logout();

                //Return Result
                result.FinalResult(executedsteps);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedsteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary>
        /// Launching XDS studies from Conference folder in Universal Viewer
        /// </summary>
        /// <param name="testid"></param>
        /// <param name="teststeps"></param>
        /// <param name="stepcount"></param>
        /// <returns></returns>
        public TestCaseResult Test_164674(String testid, String teststeps, int stepcount)
        {

            //Declare and initialize variables              
            TestCaseResult result = new TestCaseResult(stepcount);
            result.SetTestStepDescription(teststeps);
            int executedsteps = -1;
            Random random = new Random();
            int limit = Math.Abs((int)DateTime.Now.Ticks);
            limit = Int32.Parse(limit.ToString().Substring(0, 4));
            String description = "Desc" + random.Next(1, limit);            

            //Folders
            String topfoldername1 = "Top1" + new System.DateTime().Millisecond + random.Next(100, 10000);
            String subfoldername11 = "Sub11" + new System.DateTime().Millisecond + random.Next(100, 10000);
            String foldermanager = String.Empty;
            String description1 = "Desc1" + new System.DateTime().Millisecond + random.Next(100, 10000);
            String notes1 = "Notes1" + new System.DateTime().Millisecond + random.Next(100, 10000);
            String folderpath1 = topfoldername1;
            String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
            String studydescription = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Description");
            String datasource = Config.xds2;
            String ipid  = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "IPID");
            String lastname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "PatientName");
            String domainname = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "DomainName");
            String role = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "RoleName");
            String[] arraccession = accession.Split(':');            
            folderpath1 = topfoldername1 + "/" + subfoldername11;

            try
            {
                
                //Step-1  
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                var domaintab = login.Navigate<DomainManagement>();
                domaintab.SearchDomain(domainname);
                domaintab.SelectDomain(domainname);
                domaintab.ClickEditDomain();
                if (!domaintab.ConferenceListsCB().Selected)
                    domaintab.SetCheckbox(domaintab.ConferenceListsCB());
                domaintab.ClickSaveEditDomain();
                var conference = login.Navigate<ConferenceFolders>();
                conference.CreateToplevelFolder(topfoldername1, domain:"SuperAdminGroup");
                conference.CreateSubFolder(topfoldername1, subfoldername11);
                executedsteps++;

                //Step-2 
                var studies = login.Navigate<Studies>();
                studies.SearchStudy(LastName:lastname, Datasource:datasource);
                studies.ChooseColumns(new string[] {"Issuer of PID"});
                studies.SelectStudy("Issuer of PID", ipid);
                var evviewer = studies.LaunchStudy();
                executedsteps++;

                //Step-3                
                evviewer.SelectToolInToolBar(IEnum.ViewerTools.AddToConferenceFolder);
                evviewer.AddStudyToStudyFolder(folderpath1);
                studies.CloseStudy();                
                executedsteps++;

                //Step-4
                conference = login.Navigate<ConferenceFolders>();
                var study = conference.GetMatchingRow("Accession", arraccession[0]);
                if(study!=null)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-5
                conference.SelectStudy("Accession", arraccession[0]);
                var isUniversalbtn = BasePage.FindElementByCss(ConferenceFolders.btnUniversalviewer).Enabled;
                var isEnterprisebtn = BasePage.FindElementByCss(ConferenceFolders.btnEnterpriseviewer).Enabled;
                if (isUniversalbtn && isEnterprisebtn)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-6
                BasePage.FindElementByCss(ConferenceFolders.btnUniversalviewer).Click();
                PageLoadWait.WaitForFrameLoad(5);
                PageLoadWait.WaitForThumbnailsToLoad(5);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                var step6 = result.steps[++executedsteps];
                step6.SetPath(testid, executedsteps);
                var isImageCorrect6 = evviewer.CompareImage(step6, evviewer.ViewerContainer());
                if (isImageCorrect6)
                {
                    result.steps[executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-7
                evviewer.CloseStudy();
                studies.SelectStudy1("Accession", arraccession[0], dblclick: true);
                PageLoadWait.WaitForFrameLoad(5);
                PageLoadWait.WaitForThumbnailsToLoad(5);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                var step7 = result.steps[++executedsteps];
                step7.SetPath(testid, executedsteps);
                var isImageCorrect7 = evviewer.CompareImage(step7, evviewer.ViewerContainer());
                if (isImageCorrect7)
                {
                    result.steps[executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-8
                evviewer.CloseStudy();
                executedsteps++;

                //Step-9, 10
                studies = login.Navigate<Studies>();
                studies.SearchStudy(LastName: lastname, Datasource: datasource);
                studies.ChooseColumns(new string[] { "Issuer of PID" });
                studies.SelectStudy("Issuer of PID", ipid);
                evviewer = studies.LaunchStudy();
                evviewer.NavigateToHistoryPanel();
                studies.OpenPriors(new string[] { "Study Description" }, new string[] { studydescription });
                executedsteps++;
                executedsteps++;

                //Step-11
                if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer"))
                    evviewer.studyPanel(2).Click();
                else
                    login.GetElement(BasePage.SelectorType.CssSelector,
                        "#m_studyPanels_m_studyPanel_2_PatientBannerDiv > div").Click();              
               
                PageLoadWait.WaitForFrameLoad(5);
                evviewer.SelectToolInToolBar(IEnum.ViewerTools.AddToConferenceFolder);
                evviewer.AddStudyToStudyFolder(folderpath1);
                executedsteps++;

                //Step-12
                evviewer.CloseStudy();
                conference = login.Navigate<ConferenceFolders>();
                study = conference.GetMatchingRow("Description", studydescription);
                if(study!=null)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-13
                conference.SelectStudy("Description", studydescription);
                isUniversalbtn = BasePage.FindElementByCss(ConferenceFolders.btnUniversalviewer).Enabled;
                isEnterprisebtn = BasePage.FindElementByCss(ConferenceFolders.btnEnterpriseviewer).Enabled;
                if (isUniversalbtn && isEnterprisebtn)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-14
                studies.SelectStudy1("Description", studydescription, dblclick: true);
                PageLoadWait.WaitForFrameLoad(5);
                PageLoadWait.WaitForThumbnailsToLoad(5);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                var step13 = result.steps[++executedsteps];
                step13.SetPath(testid, executedsteps);
                var isImageCorrect13 = evviewer.CompareImage(step13, evviewer.ViewerContainer());
                if (isImageCorrect13)
                {
                    result.steps[executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-15
                evviewer.CloseStudy();
                conference.ArchiveConferenceFolder(folderpath1);
                var isfoldrenotpresent14_1 = conference.ExpandAndSelectFolder(folderpath1) == null ? true : false;
                conference.NavigateToArchiveMode();
                var isfoldrepresent14_2 = conference.ExpandAndSelectFolder(folderpath1) != null ? true : false;
                if (isfoldrenotpresent14_1 && isfoldrepresent14_2)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-16
                studies.SelectStudy1("Description", studydescription, dblclick: true);
                PageLoadWait.WaitForFrameLoad(5);
                PageLoadWait.WaitForThumbnailsToLoad(5);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                var step15 = result.steps[++executedsteps];
                step15.SetPath(testid, executedsteps);
                var isImageCorrect15 = evviewer.CompareImage(step15, evviewer.ViewerContainer());
                if (isImageCorrect15)
                {
                    result.steps[executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-17
                evviewer.CloseStudy();
                conference.SelectStudy("Description", studydescription);
                isUniversalbtn = BasePage.FindElementByCss(ConferenceFolders.btnUniversalviewer).Enabled;
                isEnterprisebtn = BasePage.FindElementByCss(ConferenceFolders.btnEnterpriseviewer).Enabled;
                if (isUniversalbtn && isEnterprisebtn)
                {
                    result.steps[++executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[++executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }

                //Step-18
                BasePage.FindElementByCss(ConferenceFolders.btnUniversalviewer).Click();
                PageLoadWait.WaitForFrameLoad(5);
                PageLoadWait.WaitForThumbnailsToLoad(5);
                PageLoadWait.WaitForAllViewportsToLoad(10);
                var step17 = result.steps[++executedsteps];
                step17.SetPath(testid, executedsteps);
                var isImageCorrect17 = evviewer.CompareImage(step17, evviewer.ViewerContainer());
                if (isImageCorrect17)
                {
                    result.steps[executedsteps].status = "Pass";
                    Logger.Instance.InfoLog("-->Test Step Passed--" + result.steps[executedsteps].description);
                }
                else
                {
                    result.steps[executedsteps].status = "Fail";
                    Logger.Instance.ErrorLog("-->Test Step Failed--" + result.steps[executedsteps].description);
                    result.steps[executedsteps].SetLogs();
                }


                //Logout   
                evviewer.CloseStudy();
                login.Logout();

                //Return Result
                result.FinalResult(executedsteps);
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedsteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

    }
}
