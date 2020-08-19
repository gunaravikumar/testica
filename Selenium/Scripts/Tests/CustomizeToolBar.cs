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
using Selenium.Scripts.Pages.iConnect;

namespace Selenium.Scripts.Tests
{
    class CustomizeToolBar
    {
        public Login login { get; set; }
        public string filepath { get; set; }
        public ExamImporter ei { get; set; }
        public MpacLogin mpaclogin {get; set;}
        public HPHomePage hphomepage { get; set; }
        public HPLogin hplogin { get; set;}

        /// <summary>
        /// Constructor - Test Suite
        /// </summary>
        public CustomizeToolBar(String classname)
        {                              
            login = new Login();
            login.DriverGoTo(login.url);
            hplogin = new HPLogin();
            hphomepage = new HPHomePage();
            filepath = Config.TestSuitePath + Path.DirectorySeparatorChar + classname + ".xls";
            ei = new ExamImporter();
            mpaclogin = new MpacLogin();
        }

        /// <summary> 
        /// Initial Setup
        /// </summary>
        public TestCaseResult Test_27881(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables  
            TestCaseResult result = null;
            int executedSteps = -1;


            try
            {
                //Setup Test Step Description
                result = new TestCaseResult(stepcount);

                //Set up Validation Steps
                result.SetTestStepDescription(teststeps);

                /* This is initial setup Test - Automated in Environment Setup itself*/
                /* Hence marking this case as Automated */

                for (int i = 0; i < stepcount; i++)
                {
                    result.steps[++executedSteps].status = "Pass";
                }

                //Report Result
                result.FinalResult(executedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                return result;
            }
            catch (Exception e)
            {
                //Log Exception
                Logger.Instance.ErrorLog(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);

                //Report Result
                result.FinalResult(e, executedSteps);
                Logger.Instance.ErrorLog("Overall Test status--" + result.status);

                //Logout
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary> 
        /// Domain Toolbar
        /// </summary>
        public TestCaseResult Test_27883(String testid, String teststeps, int stepcount)
        {

            #region TestCaseSummary
            /*
             * 
                Step 1 to 9 - Login as Administrator - Change Review Toolbar Configuration in Edit Domain page for Test Domain.
                And re-verify by logging as Test Domain Admin in domain edit, role edit and in study viewer.

                Step 10 to 11 - Make Review toolbar changes in Domain Edit as Test Domain Admin and verify it in Study viewer.

                Step 12 to 14 - Uncheck use domain settings in Role Management and make changes in Role level for review tools.And validate as as user
                of that Role.

                Step 15 to 16 - At Role level Remove all tools and validate the same as user of that Role.

                Step-17 - 19 -  Login as Administrator from Machine-2 and update modality toolbar settings in Test Domian and verify the same in the current machine(User1 logged in).

                Step-20 to 22 - From machine-2 login as Administrator, update review toolbar in Role Management. In machine-1 login is Regular user 
                and validate review toolbar settings. And verify modality toolbar is as per domain settings.

                Step-23 to 25 - From machine-2 update review toolbar and modality toolbar in Role Management. In Machine-1 login as regular user and validate the same.
            *
            */
            #endregion TestCaseSummary

            //Declare and initialize variables            
            TestCaseResult result;            
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;
            String regularuserrole = BasePage.GetUniqueRole("Regular");
            String user1 = BasePage.GetUniqueUserId("User1");
            String accession = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionID");
            String accessionlist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
            String modalitylist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
            String[] arraccession = accessionlist.Split(':');
            String[] arrmodality = modalitylist.Split(':');           


            try
            {
                //Intial Setup - Step-1-2
                result.SetTestStepDescription(teststeps);           
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                DomainManagement domain = login.Navigate<DomainManagement>();
                var domainattr = domain.CreateDomainAttr();
                String testdomain = domainattr[DomainManagement.DomainAttr.DomainName];
                String domainadmin = domainattr[DomainManagement.DomainAttr.UserID];
                domain.CreateDomain(domainattr, isconferenceneeded: false);         
                RoleManagement rolemgmt = (RoleManagement)login.Navigate("RoleManagement");
                rolemgmt.CreateRole(domainattr[DomainManagement.DomainAttr.DomainName], regularuserrole, "AnyRole");
                UserManagement usermgmt  = (UserManagement)login.Navigate("UserManagement");
                usermgmt.CreateUser(user1, testdomain, regularuserrole, 1, Config.emailid, 1, user1);
                ExecutedSteps++;
                ExecutedSteps++;

                //Step-3 -- Login as Administrator and Go to DomainManagement
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                domain = (DomainManagement)login.Navigate("DomainManagement");
                domain.SearchDomain(testdomain);
                domain.SelectDomain(testdomain);
                domain.ClickEditDomain();
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("window.scrollBy(0,1850)");
                bool isToolbarEnabled = domain.CheckStateToolBarSection();
                bool isToolBarType = domain.ToolbarTypeDropDown().SelectedOption.GetAttribute("innerHTML").Equals("Review Toolbar");
                int toolscount = domain.GetToolsInUse().Count;
                if(isToolBarType && isToolbarEnabled && toolscount==71)
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


                //Step-4 and 5 -- Drag Some tools to Available Items and save
                PageLoadWait.WaitForFrameLoad(10);
                IWebElement tool1 = BasePage.Driver.FindElement(By.CssSelector("#toolbarItemsConfig>div[id='1']>div[class='groupItems']>ul>li:nth-of-type(1)>a>img"));
                String tooltitle1 = tool1.GetAttribute("title");
                IWebElement tool2 = BasePage.Driver.FindElement(By.CssSelector("#toolbarItemsConfig>div[id='2']>div[class='groupItems']>ul>li:nth-of-type(1)>a>img"));
                String tooltitle2 = tool2.GetAttribute("title");
                domain.MoveToolsToAvailableSection(new IWebElement[] {tool1, tool2});                
                login.Navigate("DomainManagement");
                domain.SearchDomain(testdomain);
                domain.SelectDomain(domainattr[DomainManagement.DomainAttr.DomainName]);
                domain.ClickEditDomain();                
                IList<String> alltoolsinuse_superadmin1 = domain.GetToolsInUse();
                ExecutedSteps++;
                ExecutedSteps++;

                //Step-6 -- Logout as Administrator and Login as Domain Admin
                login.Logout();
                login.LoginIConnect(domainattr[DomainManagement.DomainAttr.UserID], domainattr[DomainManagement.DomainAttr.Password]);
                ExecutedSteps++;

                //Step-7 -- Navigate to Domain Management and check the Tools in available section
                domain = (DomainManagement)login.Navigate("DomainManagement");
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("window.scrollBy(0,1850)");
                IList<String> alltoolsinuse = domain.GetReviewToolsInUse(true);
                IList<String> alltoolsinuse1 = domain.GetToolsInUse();
                bool isToolInAvailableSection = domain.CheckToolsInAvailbleSection(new String[] { tooltitle1, tooltitle2 });
                if (isToolInAvailableSection && (domain.CompareList(alltoolsinuse_superadmin1, alltoolsinuse1)) && (domain.CompareList(alltoolsinuse1, alltoolsinuse_superadmin1)))
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
                             

                //Step-8 -- Navigate to RoleManagement and perform the same above validations.
                rolemgmt = (RoleManagement)login.Navigate("RoleManagement");                
                rolemgmt.SearchRole(domainattr[DomainManagement.DomainAttr.RoleName]);
                rolemgmt.SelectRole(domainattr[DomainManagement.DomainAttr.RoleName]);
                rolemgmt.ClickEditRole();
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("window.scrollBy(0,1850)");
                //check available section
                bool itemsavailable = rolemgmt.CheckToolsInAvailbleSection(new String[] { tooltitle1, tooltitle2 });                
                IList<String> toolsinrolemgmt1 = rolemgmt.GetToolsInUse();
                bool checkboxselected = false;
                try
                {
                    if (BasePage.Driver.FindElement(By.CssSelector("input[id$='_UseDomainToolbarCheckbox']")).Selected)
                    {
                        checkboxselected = true;
                    }
                }
                catch (Exception e) { }
                bool isDisablesectionavailable = false;
                try
                {
                    if (BasePage.Driver.FindElement(By.CssSelector("#disabledItemsList")).Displayed)
                        isDisablesectionavailable = true;
                }
                catch(Exception e) {}
                if (itemsavailable && checkboxselected && !isDisablesectionavailable && (rolemgmt.CompareList(toolsinrolemgmt1, alltoolsinuse1)) && (rolemgmt.CompareList(alltoolsinuse1, toolsinrolemgmt1)))
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
                rolemgmt.CloseRoleManagement();

                //step-9 - Navigate to Study tab and view the study and perfrom validation
                //validate review toolbar in study viewer
                Studies studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy("Accession", accession);
                studies.LaunchStudy();
                IList<String> toolsinviewer = studies.GetReviewToolsFromviewer(true);                
                bool reviewtools = alltoolsinuse.All(tool => toolsinviewer.Contains(tool));
                bool reviewtools2 = toolsinviewer.All(tool =>
                {
                    if (tool.Equals("Close")||tool.Equals("Help"))
                    {
                        return true;
                    }
                    else
                    {
                        if (alltoolsinuse.Contains(tool)) { return true; } else { return false;}
                    }

                });
                //validate no modality toolbar is present
                int modalitytools = BasePage.Driver.FindElements(By.CssSelector("#StudyToolbar>div>ul>li")).Count;
                if (reviewtools && (modalitytools == 1) && reviewtools2)
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

               //Step-10 -- Navigate to Domain Management and make changes in Review Toolbar
               domain = (DomainManagement)login.Navigate("DomainManagement");
               ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("window.scrollBy(0,1850)");
               IWebElement tool3 = BasePage.Driver.FindElement(By.CssSelector("#toolbarItemsConfig>div[id='1']>div[class='groupItems']>ul>li:nth-of-type(1)>a>img"));
               String tooltitle3 = tool3.GetAttribute("title");
               IWebElement tool4 = BasePage.Driver.FindElement(By.CssSelector("#toolbarItemsConfig>div[id='2']>div[class='groupItems']>ul>li:nth-of-type(1)>a>img"));
               String tooltitle4 = tool4.GetAttribute("title");
               domain.MoveToolsToAvailableSection(new IWebElement[] { tool3, tool4 });
               login.Navigate("DomainManagement");
               ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("window.scrollBy(0,1850)");
               IList<String> reviwetoolsinuse_step10=domain.GetReviewToolsInUse(true); 
               ExecutedSteps++;
  
                //Step-11 -- Navigate  to studies tab and perfrom same validation
                studies = (Studies)login.Navigate("Studies");
                studies.SearchStudy("Accession", accession);
                studies.SelectStudy("Accession", accession);
                studies.LaunchStudy();
                toolsinviewer = studies.GetReviewToolsFromviewer(true);
                reviewtools = reviwetoolsinuse_step10.All(tool => toolsinviewer.Contains(tool));
                reviewtools2 = toolsinviewer.All(tool =>
                {
                    if (tool.Equals("Close") || tool.Equals("Help"))
                    {
                        return true;
                    }
                    else
                    {
                        if (reviwetoolsinuse_step10.Contains(tool)) { return true; } else { return false; }
                    }

                });
                modalitytools = BasePage.Driver.FindElements(By.CssSelector("#StudyToolbar>div>ul>li")).Count;
                if (reviewtools && (modalitytools == 1) && reviewtools2)
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

               //Step-12 -- Navigate to Rolemanagement and select role and click edit
               rolemgmt = (RoleManagement)login.Navigate("RoleManagement");
               rolemgmt.SearchRole(regularuserrole);
               rolemgmt.SelectRole(regularuserrole);
               rolemgmt.ClickEditRole();
               ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("window.scrollBy(0,1850)");
               ExecutedSteps++;

                //Step-13 -- Uncheck use domain settings and modify review toolbar
               BasePage.Driver.FindElement(By.CssSelector("[id$='_UseDomainToolbarCheckbox']")).Click();
               PageLoadWait.WaitForFrameLoad(10);
               tool1 = BasePage.Driver.FindElement(By.CssSelector("#toolbarItemsConfig>div[id='1']>div[class='groupItems']>ul>li:nth-of-type(1)>a>img"));
               tooltitle1 = tool1.GetAttribute("title");
               tool2 = BasePage.Driver.FindElement(By.CssSelector("#toolbarItemsConfig>div[id='2']>div[class='groupItems']>ul>li:nth-of-type(1)>a>img"));
               tooltitle2 = tool2.GetAttribute("title");
               rolemgmt.MoveToolsToAvailableSection(new IWebElement[] { tool1, tool2 });
               rolemgmt.SelectRole(regularuserrole);
               rolemgmt.ClickEditRole();
               ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("window.scrollBy(0,1850)");
               IList<String> reviewtoolsinuse_step13 = rolemgmt.GetReviewToolsInUse(true);
               ExecutedSteps++;

               //Step-14 -- Logout as domain admin and login as regular user
               login.Logout();
               login.LoginIConnect(user1, user1);               
               studies.SearchStudy("Accession", accession);
               studies.SelectStudy("Accession", accession);
               studies.LaunchStudy();               
               toolsinviewer = studies.GetReviewToolsFromviewer();
               bool isreviewtools14 = reviewtoolsinuse_step13.All(tool => toolsinviewer.Contains(tool));
               reviewtools2 = toolsinviewer.All(tool =>
                {
                    if (tool.Equals("Close") || tool.Equals("Help"))
                    {
                        return true;
                    }
                    else
                    {
                        if (reviewtoolsinuse_step13.Contains(tool)) { return true; } else { return false; }
                    }

                });
               modalitytools = BasePage.Driver.FindElements(By.CssSelector("#StudyToolbar>div>ul>li")).Count;
               if ((modalitytools == 1) && isreviewtools14 && reviewtools2)
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

               //stepcount-15 -- Logout and Login as Domain Admin
               login.Logout();
               login.LoginIConnect(domainattr[DomainManagement.DomainAttr.UserID], domainattr[DomainManagement.DomainAttr.Password]);
               rolemgmt = (RoleManagement)login.Navigate("RoleManagement");
               rolemgmt.SelectRole(regularuserrole);
               rolemgmt.ClickEditRole();
               ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("window.scrollBy(0,1850)");
               IList<IWebElement> tools = rolemgmt.GetReviewToolsElementsInUse();
               rolemgmt.MoveToolsToAvailableSection(tools.ToArray());
               rolemgmt.SelectRole(regularuserrole);
               rolemgmt.ClickEditRole();
               tools = rolemgmt.GetReviewToolsElementsInUse();
               rolemgmt.MoveToolsToAvailableSection(tools.ToArray());
               ExecutedSteps++;

               //Step-16 -- Logout and Login as regular user and verify the study viewer               
               login.Logout();
               login.LoginIConnect(user1, user1);               
               studies.SearchStudy("Accession", accession);
               studies.SelectStudy("Accession", accession);
               studies.LaunchStudy(toolscount: 2);
               toolsinviewer = studies.GetReviewToolsFromviewer();
               bool isreviewtollsynch = ((toolsinviewer.Count == 2) && (toolsinviewer.Contains("Close")) && (toolsinviewer.Contains("Help")))? true:false; 
               modalitytools = BasePage.Driver.FindElements(By.CssSelector("#StudyToolbar>div>ul>li")).Count;                
               if((modalitytools==1) && isreviewtollsynch)
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

                //Sep-17 - Login in a Administrator; Configure Modality tool bar. (Machine-2)
                //login.Logout();
                BasePage.MultiDriver.Add(BasePage.Driver);
                BasePage.MultiDriver.Add(login.InvokeBrowser(Config.BrowserType));
                login.SetDriver(BasePage.MultiDriver[1]);
                login.DriverGoTo(login.url);
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.Navigate<DomainManagement>();
                domain.SearchDomain(testdomain);
                domain.SelectDomain(testdomain);
                domain.ClickEditDomain();
                var mtools17 = new String[] { IEnum.ViewerTools.AllinOneTool.ToString(), IEnum.ViewerTools.DrawEllipse.ToString() }.ToList();
                domain.ConfigureModalityToolbar(arrmodality[0], new String[] { IEnum.ViewerTools.AllinOneTool.ToString(), IEnum.ViewerTools.DrawEllipse.ToString()}, false);
                domain.ClickSaveDomain();
                ExecutedSteps++;

                //Step-18 - Validate above change for Specific Modality (CR Modality) (MAchine-1)
                //login.Logout();
                //login.LoginIConnect(user1, user1);
                login.SetDriver(BasePage.MultiDriver[0]);
                studies.CloseStudy();                                
                studies.SearchStudy("Accession", arraccession[0]);
                studies.SelectStudy("Accession", arraccession[0]);
                studies.LaunchStudy(toolscount: 2);
                toolsinviewer = studies.GetReviewToolsFromviewer();
                bool isreviewtollsynch18 = ((toolsinviewer.Count == 2) && (toolsinviewer.Contains("Close")) && (toolsinviewer.Contains("Help"))) ? true : false;
                IList<IWebElement> modality_tools = BasePage.Driver.FindElements(By.CssSelector("#StudyToolbar>div>ul>li>a>img"));
                var mtools18 = modality_tools.Select<IWebElement, String>(tool => tool.GetAttribute("title").Replace(" ", "")).ToList();
                if ((modality_tools.Count== 3) && isreviewtollsynch18 && mtools17.All(tool=> mtools18.Contains(tool.Replace(" ", ""))))
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

                //Step-19 - Load a different modality (CT Modality)                
                studies.SearchStudy("Accession", arraccession[1]);
                studies.SelectStudy("Accession", arraccession[1]);
                studies.LaunchStudy(toolscount: 2);
                toolsinviewer = studies.GetReviewToolsFromviewer();
                bool isreviewtollsynch19 = ((toolsinviewer.Count == 2) && (toolsinviewer.Contains("Close")) && (toolsinviewer.Contains("Help"))) ? true : false;
                modalitytools = BasePage.Driver.FindElements(By.CssSelector("#StudyToolbar>div>ul>li>a>img")).Count;                
                if ((modalitytools == 1) && isreviewtollsynch19)
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

                //Step 20-Relogin as Administrator and Go to Role Management
                //login.Logout();
                // login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.SetDriver(BasePage.MultiDriver[1]);
                login.Navigate<RoleManagement>();
                rolemgmt.SelectDomainfromDropDown(testdomain);
                rolemgmt.SearchRole(regularuserrole);
                rolemgmt.SelectRole(regularuserrole);
                rolemgmt.ClickEditRole();
                ExecutedSteps++;

                //Step-21 - Update Review Toolbar configuration
                IList<String> reviewtools21 = new List<String>();
                reviewtools21.Add(IEnum.ViewerTools.AllinOneTool.ToString());
                reviewtools21.Add(IEnum.ViewerTools.DrawRectangle.ToString());
                rolemgmt.MoveToolsToToolbarSection(reviewtools21.ToArray());
                rolemgmt.ClickSaveEditRole();
                ExecutedSteps++;

                //Step-22 - Relogin as User1 and Load CR Modality Study
                //login.Logout();
                //login.LoginIConnect(user1, user1);
                login.SetDriver(BasePage.MultiDriver[0]);               
                studies.SearchStudy("Accession", arraccession[0]);
                studies.SelectStudy("Accession", arraccession[0]);
                studies.LaunchStudy(toolscount: 2);
                toolsinviewer = studies.GetReviewToolsFromviewer();
                bool isReviewToolsPresent = reviewtools21.All(tool => toolsinviewer.Contains(tool.Replace(" ", "")));
                IList<IWebElement> modality_tools22 = BasePage.Driver.FindElements(By.CssSelector("#StudyToolbar>div>ul>li>a>img"));
                var mtools22 = modality_tools22.Select<IWebElement, String>(tool => tool.GetAttribute("title").Replace(" ", "")).ToList();
                bool isModalityToolsPresent = mtools17.All(tool => mtools22.Contains(tool.Replace(" ", "")));
                if(isModalityToolsPresent && isReviewToolsPresent)
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

                //Step-23 - Navigate back to RoleManagement
                // login.Logout();
                //login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.SetDriver(BasePage.MultiDriver[1]);
                login.Navigate<RoleManagement>();
                rolemgmt.SearchRole(regularuserrole);
                rolemgmt.SelectRole(regularuserrole);
                rolemgmt.ClickEditRole();
                ExecutedSteps++;

                //Step-24 - Update Modality Tool Bar Settings(**** need to make this method for Role management as well)
                var mtools24 = new String[] { IEnum.ViewerTools.AllinOneTool.ToString(), IEnum.ViewerTools.DrawEllipse.ToString() }.ToList();
                rolemgmt.ConfigureModalityToolbar(arrmodality[2], new String[] { IEnum.ViewerTools.AllinOneTool.ToString(), IEnum.ViewerTools.DrawEllipse.ToString() }, remove_existing_tools:false, isRolemanagementScreen:true);
                rolemgmt.ClickSaveEditRole();
                ExecutedSteps++;

                //Step-25 - Validate Modality and Review Toolbar
                //login.Logout();
                //login.LoginIConnect(user1, user1);
                login.SetDriver(BasePage.MultiDriver[0]);                
                studies.SearchStudy("Accession", arraccession[2]);
                studies.SelectStudy("Accession", arraccession[2]);
                studies.LaunchStudy(toolscount:2);
                toolsinviewer = studies.GetReviewToolsFromviewer();                
                bool isReviewToolsPresent25 = reviewtools21.All(tool => toolsinviewer.Contains(tool.Replace(" ", "")));
                IList<IWebElement> modality_tools25 = BasePage.Driver.FindElements(By.CssSelector("#StudyToolbar>div>ul>li>a>img"));
                var mtools25 = modality_tools25.Select<IWebElement, String>(tool => tool.GetAttribute("title").Replace(" ", "")).ToList();
                bool isModalityToolsPresent25 = mtools24.All(tool => mtools25.Contains(tool));
                if(isModalityToolsPresent25 && isReviewToolsPresent25)
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

                //Logout
                login.ResetDriver();
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
                login.ResetDriver();
                login.Logout();

                //Return Result
                return result;
            }

        }

        /// <summary> 
        /// Modality Toolbar
        /// </summary>
        public TestCaseResult Test_27882(String testid, String teststeps, int stepcount)
        {
            //Declare and initialize variables            
            TestCaseResult result;
            result = new TestCaseResult(stepcount);
            int ExecutedSteps = -1;            
            String user1 = BasePage.GetUniqueUserId("User1");
            String testdomain = String.Empty;
            String domainname = String.Empty;
            String rolename = String.Empty;
            String username = String.Empty;
            String password = String.Empty;
            String domainadmin = String.Empty;

            String regularuserrole = BasePage.GetUniqueRole("Regular");            
            String accessionlist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "AccessionIDList");
            String modalitylist = (String)ReadExcel.GetTestData(filepath, "TestData", testid, "Modality");
            String[] arraccession = accessionlist.Split(':');
            String[] arrmodality = modalitylist.Split(':');
            String cssToolsSection = "div#toolbarItemsConfig";
            domainname = "SuperAdminGroup";
            rolename = "SuperRole";
            username = Config.adminUserName;
            password = Config.adminPassword;

            #region TestSummary
            /**
             * Step-1, and 2 - Precondition
             * Step-3 and 4  - Validate the default state of Modality toolbar for different modalities in SuperAdminGroup - DomainEdit Screen
             * Step-5 to 10  - Add tools in Modality Default Toolbar section(SuperAdmin Domain) and validate the same by opening different studies. 
             * Step-11 to 28 - Uncheck Modality default toolbar and update modality tools and validate the same.
             * (For CR, CT and MR modalities-SuperAdminGroup -  Domain Level)
             * Step-29 to 36 - Uncheck 'Use Domain Settings', and update modality toolbar for each modality in SuperRole and validate the same.
             * Precondition -  A regular user role and user created in SuperAdmin domain. Setup width presets at Domain level(SuperAdmin)
             * Step-37 to 42 -
             * a. Login as Regular user and validate W/L presets are based on domain settings
             * b. Lofin as Regular user and validate modality tools as per Domain setttings.
             * c. Update W/L presets at user preference level and validte the same by launching studies.
             * */
            #endregion Test Summary

            try
            {  

                //Intial Setup - Step-1-2
                result.SetTestStepDescription(teststeps);
                ExecutedSteps++;
                ExecutedSteps++;
                
                //Create Regular user role and user               
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                RoleManagement rolemgmt = (RoleManagement)login.Navigate("RoleManagement");
                rolemgmt.CreateRole(domainname, regularuserrole, "AnyRole");
                UserManagement usermgmt = (UserManagement)login.Navigate("UserManagement");
                usermgmt.CreateUser(user1, testdomain, regularuserrole, 1, Config.emailid, 1, user1);               

                //Step-3 - Select Toolbar Type - CR                 
                DomainManagement domain = login.Navigate<DomainManagement>();
                domain.SearchDomain(domainname);
                domain.SelectDomain(domainname);
                domain.ClickEditDomain();
                domain.ToolbarTypeDropDown().SelectByText(arrmodality[0]);
                BasePage.wait.Until<Boolean>(d => d.FindElement(By.CssSelector(cssToolsSection)).GetAttribute("class").Contains("disabled"));
                var toolsinuser3 = domain.GetToolsInUse();
                var availabletools3 = domain.GetAvailableTools();
                var disabledtools3 = domain.GetDisabledTools();
                var isEnabled3 = domain.CheckStateToolBarSection();
                var isModalityDefault3 = domain.GetState_ToolsUseModalityDefaultToolBar();
                if (toolsinuser3.Count == 0 && availabletools3.Count>0 && disabledtools3.Count == 0 && isEnabled3==false && isModalityDefault3==true)
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

                //Step-4 above validation for different modality
                domain.ToolbarTypeDropDown().SelectByText(arrmodality[1]);
                BasePage.wait.Until<Boolean>(d => d.FindElement(By.CssSelector(cssToolsSection)).GetAttribute("class").Contains("disabled"));
                var toolsinuser4 = domain.GetToolsInUse();
                var availabletools4 = domain.GetAvailableTools();
                var disabledtools4 = domain.GetDisabledTools();
                var isEnabled4 = domain.CheckStateToolBarSection();
                var isModalityDefault4 = domain.GetState_ToolsUseModalityDefaultToolBar();
                if (toolsinuser4.Count == 0 && availabletools4.Count > 0 && disabledtools4.Count == 0 && isEnabled4 == false && isModalityDefault4 == true)
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

                //Step-5 - Select Toolbar as Modality Default toolbar
                domain.ToolbarTypeDropDown().SelectByText("Modality Default Toolbar");
                Thread.Sleep(1000);
                var toolsinuse5 = domain.GetToolsInUse();
                var availabletools5 = domain.GetAvailableTools();
                var disabledtools5 = domain.GetDisabledTools();
                var isEnabled5 = domain.CheckStateToolBarSection();                
                if (toolsinuse5.Count == 0 && availabletools5.Count > 0 && disabledtools5.Count == 0 && isEnabled5 == true)
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

                //Step-6 - Move Tools from Available to New Tools
                IWebElement tool1 = BasePage.Driver.FindElement(By.CssSelector("div#availableItems>div[id$='ItemsList']>ul>li[id='1']>a>img"));
                String tooltitle1 = tool1.GetAttribute("title");
                IWebElement tool2 = BasePage.Driver.FindElement(By.CssSelector("div#availableItems>div[id$='ItemsList']>ul>li[id='2']>a>img"));
                String tooltitle2 = tool2.GetAttribute("title");
                var toolslist6 = (new String[] { tooltitle1, tooltitle2 }).ToList();
                domain.AddToolsToModalityToolbar(new string[] {tooltitle1, tooltitle2}, "Modality Default Toolbar");
                domain.ClickSaveEditDomain();
                ExecutedSteps++;

                //Step7- Validate the same in Role Management
                var role = login.Navigate<RoleManagement>();
                role.SelectDomainfromDropDown(domainname);
                role.SearchRole(rolename);
                role.SelectRole(rolename);
                role.ClickEditRole();
                role.ToolbarTypeDropDown().SelectByText("Modality Default Toolbar");
                Thread.Sleep(1000);
                var toolsinuse7 = role.GetToolsInUse();
                var availabletools7 = role.GetAvailableTools();
                var disabledtools7 = role.GetDisabledTools();
                var isEnabled7 = role.CheckStateToolBarSection();
                if (availabletools7.Count>0 && isEnabled7==false && disabledtools7.Count==0
                    && toolsinuse7.All(tool=> toolslist6.Contains(tool)))
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
                role.ClickSaveEditRole();

                //Step-8  - Open studies of diffrent modality and validate
                login.Logout();
                login.LoginIConnect(username, password);
                var studies = login.Navigate<Studies>();                
                IList<String> mtools;
                ExecutedSteps++;
                foreach (String accession in arraccession)
                {
                    studies.SearchStudy("Accession", accession);
                    studies.SelectStudy("Accession", accession);
                    studies.LaunchStudy();
                    mtools = studies.GetModalityTools();
                    studies.CloseStudy();
                    if (toolslist6.All(tool=> mtools.Contains(tool)))
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

                //Step-9-- Set Role level settings and add tools
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.Navigate<RoleManagement>();
                role.SelectDomainfromDropDown(domainname);
                role.SearchRole(rolename);
                role.SelectRole(rolename);
                role.ClickEditRole();
                role.ToolbarTypeDropDown().SelectByText("Modality Default Toolbar");
                role.Check_ToolsUseDomainSettings(false);
                Thread.Sleep(1000);
                IWebElement tool3 = BasePage.Driver.FindElement(By.CssSelector("div#availableItems>div[id$='ItemsList']>ul>li[id='4']>a>img"));
                String tooltitle3 = tool3.GetAttribute("title");
                IWebElement tool4 = BasePage.Driver.FindElement(By.CssSelector("div#availableItems>div[id$='ItemsList']>ul>li[id='5']>a>img"));
                String tooltitle4 = tool4.GetAttribute("title");
                var toolslist9 = (new String[] { tooltitle3, tooltitle4 }).ToList();
                role.AddToolsToModalityToolbar(new string[] { tooltitle3, tooltitle4 }, "Modality Default Toolbar", isRoleManagement:true);
                role.ClickSaveEditRole();
                ExecutedSteps++;

                //Step-10 validate above steps in study viewer
                login.Logout();
                login.LoginIConnect(username, password);
                login.Navigate<Studies>();
                IList<String> mtools10;
                ExecutedSteps++;
                foreach (String accession in arraccession)
                {
                    studies.SearchStudy("Accession", accession);
                    studies.SelectStudy("Accession", accession);
                    studies.LaunchStudy();
                    mtools10 = studies.GetModalityTools();
                    studies.CloseStudy();
                    if (toolslist6.All(tool => mtools10.Contains(tool)) && toolslist9.All(tool => mtools10.Contains(tool)))
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

                //Step-11 - Navigate to Domain - Select Modality CR
                login.Logout();
                login.LoginIConnect(Config.adminUserName, Config.adminPassword);
                login.Navigate<DomainManagement>();
                domain.SearchDomain(domainname);
                domain.SelectDomain(domainname);
                domain.ClickEditDomain();
                domain.ToolbarTypeDropDown().SelectByText(arrmodality[0]);
                domain.Check_ToolsUseModalityDefaultSettings(false);
                Thread.Sleep(1000);
                var toolsinuse11 = domain.GetToolsInUse();
                var availabletools11 = domain.GetAvailableTools();
                var disabledtools11 = domain.GetDisabledTools();
                var isEnabled11 = domain.CheckStateToolBarSection();
                if(toolsinuse11.Count==0 && disabledtools11.Count==0 && isEnabled11==true &&
                    domain.GetState_ToolsUseModalityDefaultToolBar()==false)
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

                //Step-12 - Drag some tools to new sections
                var toolsinavailable_before = BasePage.Driver.FindElements(By.CssSelector("div#availableItems>div.groupItems>ul>li>a>img")).Count;
                String[] mtools12 = new String[] {"Pan", "Zoom"};
                domain.MoveToolsToToolbarSection(mtools12);               
                var toolsinuse = domain.GetToolsInUse();
                var toolsinavailable_after = BasePage.Driver.FindElements(By.CssSelector("div#availableItems>div.groupItems>ul>li>a>img")).Count;
                if (toolsinuse.All(tool => mtools12.Contains(tool)) &&
                    (toolsinuse.Count==toolsinavailable_before- toolsinavailable_after))
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

                //Step-13 - Not Automated
                result.steps[++ExecutedSteps].status = "Not Automated";

                //Step-14 - Move a Tool from New to Available Section
                var pantool = BasePage.Driver.FindElements(By.CssSelector("div#toolbarItemsConfig>div div.groupItems ul li>a>img"))
                    .TakeWhile(element => element.GetAttribute("title").Equals("Pan")).ToArray<IWebElement>();
                domain.MoveToolsToAvailableSection(pantool, false);
                if(domain.GetAvailableTools().Contains("Pan"))
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

                //Step-15- Move the tool back to New section                
                domain.AddToolsToToolbarByName(new string[] {"Pan"});
                if (!domain.GetAvailableTools().Contains("Pan"))
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

                //Step-16 - Move tools to diabled
                var mtools16 = new String[] {"Edit Annotations", "Cine"};
                var disabledtools_before = domain.GetDisabledTools();
                domain.MoveToolsToDisabled(mtools16);
                var disabledtools_after = domain.GetDisabledTools();
                if(disabledtools_before.Count==0 && disabledtools_after.Count==2 
                    && disabledtools_after.All(tool=> mtools16.Contains(tool)))
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
                
                //Step-17 Configure MR modality (Domain Level)                
                String[] mtools17 = new String[] {"Pan", "Zoom"};           
                domain.ConfigureModalityToolbar(arrmodality[1], mtools17, false, false);
                var toolsinuse17 = domain.GetToolsInUse();
                if(mtools17.All(tool=>toolsinuse17.Contains(tool)))
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


                //Step-18 - Configure CT modality (Domain Level)
                String[] mtools18 = new String[] { "Pan", "Zoom" };                
                domain.ConfigureModalityToolbar(arrmodality[2], mtools18, false, false);
                var toolsinuse18 = domain.GetToolsInUse();
                if (mtools18.All(tool => toolsinuse18.Contains(tool)))
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

                //Step-19 - Save the Domain Management page
                domain.ClickSaveDomain();
                ExecutedSteps++;

                //Step-20 - Validate above in Role Management
                login.Navigate<RoleManagement>();
                role.SelectDomainfromDropDown(domainname);
                role.SearchRole(rolename);
                role.SelectRole(rolename);
                role.ClickEditRole();
                domain.ToolbarTypeDropDown().SelectByText(arrmodality[0]);
                var CR20 = role.GetState_ToolsUseDomainSettings();
                var CRtools20 = role.GetToolsInUse();
                var CRdiabledtools20 = role.GetDisabledTools();

                role.ToolbarTypeDropDown().SelectByText(arrmodality[1]);
                var MR20 = role.GetState_ToolsUseDomainSettings();
                var MRtools20 = role.GetToolsInUse();

                role.ToolbarTypeDropDown().SelectByText(arrmodality[2]);
                var CT20 = role.GetState_ToolsUseDomainSettings();
                var CTtools20 = role.GetToolsInUse(); 
                               
                if (CRtools20.All(tool => mtools12.Contains(tool)) && MRtools20.All(tool => mtools17.Contains(tool))
                    && CTtools20.All(tool => mtools18.Contains(tool)) && CRdiabledtools20.All(tool=> mtools16.Contains(tool))
                    && CRtools20.Count==2 && MRtools20.Count==2 && CTtools20.Count==2 && CR20 && MR20 && CT20)
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

                //Step-21-Redundant step as in subsequents same validation is being done.
                role.ClickSaveEditRole();
                ExecutedSteps++;

                //Step-22 - Load CR Modality
                login.Navigate<Studies>();
                studies.SearchStudy("Accession", arraccession[0]);
                studies.SelectStudy("Accession", arraccession[0]);
                var viewer = studies.LaunchStudy();
                viewer.isToolbartypeModality = true;
                var crmtools22 = viewer.GetModalityTools();                
                if(mtools12.All(tool=> crmtools22.Contains(tool)) && crmtools22.Count>0)
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

                //Step-23 - Apply tool
                viewer.ApplyPan(viewer.SeriesViewer_1X1());
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                var isImageCompare23 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                viewer.CloseStudy();
                if (isImageCompare23)
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


                //Step-24- Load MR Modality study
                //login.Navigate<Studies>();
                studies.SearchStudy("Accession", arraccession[1]);
                studies.SelectStudy("Accession", arraccession[1]);
                viewer = studies.LaunchStudy();
                viewer.isToolbartypeModality = true;
                var mrmtools22 = viewer.GetModalityTools();                
                if (mtools17.All(tool => mrmtools22.Contains(tool)) && mrmtools22.Count > 0)
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

                //Step-25 - Apply MR Tool
                viewer.ApplyZoom(viewer.SeriesViewer_1X1());
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                var isImageCompare24 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                viewer.CloseStudy();
                if(isImageCompare24)
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

                //Step-26-Load CT Study
                //login.Navigate<Studies>();
                studies.SearchStudy("Accession", arraccession[2]);
                studies.SelectStudy("Accession", arraccession[2]);
                viewer = studies.LaunchStudy();
                viewer.isToolbartypeModality = true;
                var ctmtools22 = viewer.GetModalityTools();                
                if (mtools18.All(tool => ctmtools22.Contains(tool)) && ctmtools22.Count > 0)
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

                //Step-27 - Apply Tool
                viewer.ApplyZoom(viewer.SeriesViewer_1X1());
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                var isImageCompare25 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());                
                if (isImageCompare25)
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

                //Step-28 - Apply and Remove Full ScreenTool
                ExecutedSteps++;
                viewer.isToolbartypeModality = false;
                viewer.SelectToolInToolBar("FullScreen");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 1);
                var isImageCompare28_1 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                viewer.SelectToolInToolBar("FullScreen");
                result.steps[ExecutedSteps].SetPath(testid, ExecutedSteps, 2);
                var isImageCompare28_2 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                viewer.CloseStudy();
                if(isImageCompare28_1 && isImageCompare28_2)
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

                //Step-29 - Edit Role management screen
                login.Navigate<RoleManagement>();
                role.SelectDomainfromDropDown(domainname);
                role.SearchRole(rolename);
                role.SelectRole(rolename);
                role.ClickEditRole();
                ExecutedSteps++;

                //Step-30 - Choose CR Modality and validate
                role.ToolbarTypeDropDown().SelectByText(arrmodality[0]);
                var toolsinuse30 = role.GetToolsInUse();
                var disabledtools30 = role.GetDisabledTools();                
                if ((toolsinuse30.All(tool=> mtools12.Contains(tool))) && (disabledtools30.All(tool30=> mtools16.Contains(tool30)))
                    && (toolsinuse30.Count>0)  && (disabledtools30.Count>0))
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

                //Step-31 - Apply Role Level settings and update toobar -- CR
                role.Check_ToolsUseDomainSettings(false);
                role.Check_ToolsUseModalityDefaultSettings(false);
                String[] crmtools31 = new String[] { IEnum.ViewerTools.CalibrationTool.ToString(), IEnum.ViewerTools.CobbAngle.ToString() };
                role.MoveToolsToToolbarSection(crmtools31);
                role.MoveToolsToDisabled(new String[] {"Pan"});
                var mtools31 = role.GetToolsInUse();
                role.ClickSaveEditRole();
                ExecutedSteps++;

                //Step-32 - Do above settings for MR and CT
                role.SelectDomainfromDropDown(domainname);
                role.SearchRole(rolename);
                role.SelectRole(rolename);
                role.ClickEditRole();                

                //MR Modality
                role.ToolbarTypeDropDown().SelectByText(arrmodality[1]);
                role.Check_ToolsUseDomainSettings(false);
                role.Check_ToolsUseModalityDefaultSettings(false);
                String[] mtools32_1 = new String[] {IEnum.ViewerTools.CalibrationTool.ToString(), IEnum.ViewerTools.CobbAngle.ToString()};
                role.MoveToolsToToolbarSection(mtools32_1);
                role.MoveToolsToDisabled(new String[] {"Pan"});
                var mrtools32_1 = role.GetToolsInUse();

                //CT Modality
                role.ToolbarTypeDropDown().SelectByText(arrmodality[2]);
                role.Check_ToolsUseDomainSettings(false);
                role.Check_ToolsUseModalityDefaultSettings(false);
                String[] mtools32_2 = new String[] {IEnum.ViewerTools.CalibrationTool.ToString(), IEnum.ViewerTools.CobbAngle.ToString()};
                role.MoveToolsToToolbarSection(mtools32_2);
                role.MoveToolsToDisabled(new String[] {"Pan"});
                var mrtools32_2 = role.GetToolsInUse();
                ExecutedSteps++;

                //Step-33 - Redundant Step
                role.ClickSaveEditRole();
                ExecutedSteps++;

                //Step-34 - Validate Role level settings for CR Modality
                login.Navigate<Studies>();
                studies.SearchStudy("Accession", arraccession[0]);
                studies.SelectStudy("Accession", arraccession[0]);
                viewer = studies.LaunchStudy();
                var crmtools34 = viewer.GetModalityTools();
                if ((mtools31.All(tool => crmtools34.Contains(tool))) && (crmtools34.Count > 0))
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


                //Step-35 - Validate Role level settings for MR Modality
                //login.Navigate<Studies>();
                studies.SearchStudy("Accession", arraccession[1]);
                studies.SelectStudy("Accession", arraccession[1]);
                viewer = studies.LaunchStudy();
                var mrmtools35 = viewer.GetModalityTools().
                Select<String, String>(tool=> tool.Replace(" ", "")).ToList();
                if ((mtools32_1.All(tool => mrmtools35.Contains(tool.Replace(" ", "")))) && (mrmtools35.Count > 0))
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


                //Step-36 - Validate Role level settings for CT Modality
                //login.Navigate<Studies>();
                studies.SearchStudy("Accession", arraccession[1]);
                studies.SelectStudy("Accession", arraccession[1]);
                viewer = studies.LaunchStudy();
                IList<String> CTmtools36 = viewer.GetModalityTools().
                Select<String, String>(tool => tool.Replace(" ", "")).ToList<String>();
                if ((mtools32_2.All(tool => CTmtools36.Contains(tool.Replace(" ", "")))) && (CTmtools36.Count > 0))
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

                //Step-37 - Setup Width/Level Presets
                login.Navigate<DomainManagement>();
                domain.SearchDomain(domainname);
                domain.SelectDomain(domainname);
                domain.ClickEditDomain();
                domain.RemoveAllPresets(arrmodality[0]);
                domain.AddPreset(arrmodality[0], "CRTest", "1000", "1000");
                domain.RemoveAllPresets(arrmodality[1]);
                domain.AddPreset(arrmodality[1], "MRTest", "500", "500");
                domain.ClickSaveEditDomain();
                ExecutedSteps++;

                //Step-38 - Login as regular user
                login.Logout();
                login.LoginIConnect(user1, user1);
                ExecutedSteps++;

                //Step-39 - Redundant Step
                ExecutedSteps++;

                //Step-40 - Load Study CR Modality and apply preset
                //login.Navigate<Studies>();
                studies.SearchStudy("Accession", arraccession[0]);
                studies.SelectStudy("Accession", arraccession[0]);
                viewer = studies.LaunchStudy();
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul li[title*='CRTest']\").click()");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                var isImageCompare40 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                var mtools40 = studies.GetModalityTools();
                if(isImageCompare40 && mtools40[0].Contains("CRTest"))
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
                studies.CloseStudy();

                //Step-41 - Load Study MR Modality and apply preset                
                studies.SearchStudy("Accession", arraccession[1]);
                studies.SelectStudy("Accession", arraccession[1]);
                viewer = studies.LaunchStudy();
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div ul li[title*='MRTest']\").click()");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                var isImageCompare41 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                var mtools41 = studies.GetModalityTools();
                if (isImageCompare41 && mtools41[0].Contains("MRTest"))
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
                studies.CloseStudy();

                //Step-42 - Modify Presets at UserPreference
                var userpref = studies.OpenUserPreferences();
                userpref.ModifyPresetsInToolBarUserPref(arrmodality[0],"auto", "CRTest", "100", "100", isStudiesTab:false);
                userpref.AddPresetAtUserLevel(arrmodality[0], "CRTest2", "200", "200");
                //login.Navigate<Studies>();
                studies.SearchStudy("Accession", arraccession[0]);
                studies.SelectStudy("Accession", arraccession[0]);
                viewer = studies.LaunchStudy();
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"#StudyToolbar div>ul>li>ul>li:nth-of-type(2)\").click()");
                result.steps[++ExecutedSteps].SetPath(testid, ExecutedSteps);
                var isImageCompare42 = viewer.CompareImage(result.steps[ExecutedSteps], viewer.studyPanel());
                if(isImageCompare42)
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

                //Report Result
                result.FinalResult(ExecutedSteps);
                Logger.Instance.InfoLog("Overall Test status--" + result.status);

                //Return Result
                login.Logout();
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

    }
}
