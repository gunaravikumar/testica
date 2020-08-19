using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Reusable.Generic;
using Keys = OpenQA.Selenium.Keys;
using OpenQA.Selenium.Support.UI;



namespace Selenium.Scripts.Pages.iConnect
{
    class ConferenceFolders : BasePage
    {
        #region Constructor
        public ConferenceFolders() { }
        #endregion Constructor

        #region Webelements
        public By By_ManageTopLevelFolders() { return By.CssSelector("input[id='FolderManagementButton']"); }
        public IWebElement ManageTopLevelFolders() { return BasePage.Driver.FindElement(By_ManageTopLevelFolders()); }
        public By By_FolderConfigurationdiv() { return By.CssSelector("form[name='form1'] div [id='ConferenceFolderManagementDialogDiv']"); }
        public IWebElement FolderConfigurationdiv() { return BasePage.Driver.FindElement(By_FolderConfigurationdiv()); }
        public IWebElement AddFolderBtn() { return BasePage.Driver.FindElement(By.CssSelector("div[id='divFolderManagement'] input[id='ConferenceFolderManagementControl_AddFolderButton']")); }
        public IWebElement InputFolder() { return BasePage.Driver.FindElement(By.CssSelector("#newFolder")); }
        public By By_SaveFolderBtn() { return By.CssSelector("#ConferenceFolderManagementControl_Save"); }
        public IWebElement SaveFolderBtn() { return BasePage.Driver.FindElement(By_SaveFolderBtn()); }
        public By By_ManageFolderOkBtn() { return By.CssSelector("#OkButton"); }
        public IWebElement ManageFolderOkBtn() { return BasePage.Driver.FindElement(By_ManageFolderOkBtn()); }
        public IWebElement DeleteFolderManagerBtn() { return BasePage.Driver.FindElement(By.CssSelector("#ConferenceFolderManagementControl_DeleteFolderManagerButton")); }
        public IWebElement FolderManagerOKBtn() { return BasePage.Driver.FindElement(By.CssSelector("#ConferenceFolderManagementControl_OkButton")); }
        public IWebElement FolderManagerTextBox() { return BasePage.Driver.FindElement(By.CssSelector("input[id='ConferenceFolderManagementControl_SearchFolderManagerTextBox']")); }
        public IList<IWebElement> GetToplevelFolders() { return BasePage.Driver.FindElements(By.CssSelector("div[id='divtopLevelFolder'] div")); }
        public IWebElement FolderManager_Suggestion()
        {
            return BasePage.Driver.FindElement(By.CssSelector("div#AutoCompleteDiv .folderListHover"));
        }
        public IWebElement FolderManager(String name) { return BasePage.Driver.FindElement(By.CssSelector("div[id='divFolderManagers']>div[id='" + name + "']")); }
        public By By_CreateFolderButton() { return By.CssSelector("input#CreateFolderButton"); }
        public IWebElement CreateFolderButton() { return BasePage.Driver.FindElement(By_CreateFolderButton()); }
        new public IWebElement ActiveTopFolder() { return base.ActiveTopFolder(); }
        public IWebElement ActiveTopFolder_Expander() { return BasePage.Driver.FindElement(By.CssSelector("ul[class='ui-fancytree fancytree-container fancytree-plain fancytree-ext-edit'] li>span[class*='active']>span[class*='expander']")); }
        public By By_EditDesc_Button() { return By.CssSelector("a#EditFolderDescriptionButton"); }
        public IWebElement EditDesc_Button() { return BasePage.Driver.FindElement(By_EditDesc_Button()); }
        public IWebElement EditDesc_TextArea() { return BasePage.Driver.FindElement(By.CssSelector("textarea#EditFolderDescriptionTextBox")); }
        public IWebElement EditDesc_OKbutton() { return BasePage.Driver.FindElement(By.CssSelector("a#SubmitFolderDescriptionButton")); }
        public IWebElement EditDesc_CANCELbutton() { return BasePage.Driver.FindElement(By.CssSelector("a#CancelEditFolderDescriptionButton")); }
        public By By_EditDesc_Emptydesc() { return By.CssSelector("span#EmptyFolderDescriptionText"); }
        public IWebElement EditDesc_Emptydesc() { return BasePage.Driver.FindElement(By_EditDesc_Emptydesc()); }
        public IWebElement EditDesc_CurrentDesc() { return BasePage.Driver.FindElement(By.CssSelector("div#FolderDescriptionText")); }
        public IWebElement ActiveLastsib() { return BasePage.Driver.FindElement(By.CssSelector("li[class*='lastsib'] span[class*='active']")); }
        public By By_ErrorMessageLabel() { return By.CssSelector("#ConferenceFolderManagementControl_m_errorMessageLabel"); }
        public IWebElement ErrorMessageLabel() { return BasePage.Driver.FindElement(By_ErrorMessageLabel()); }
        public IWebElement TopLevelFolderCancelButton() { return BasePage.Driver.FindElement(By.CssSelector("#ConferenceFolderManagementControl_Cancel")); }

        public By By_ActiveFolder() { return By.CssSelector("span[class*='active']"); }
        public IWebElement ActiveFolder() { return BasePage.Driver.FindElement(By_ActiveFolder()); }
        public By By_FolderPathDiv() { return By.CssSelector("div#FolderPathDiv"); }
        public IWebElement FolderPathDiv() { return BasePage.Driver.FindElement(By_FolderPathDiv()); }

        public IList<IWebElement> AllTopLevelfolders()
        {
            return BasePage.Driver.FindElements(By.CssSelector("div#treeDiv>ul>li>span>span.fancytree-title"));
        }

        public IList<IWebElement> VisibleTopAndSubLevelfolders()
        {
            return BasePage.Driver.FindElements(By.CssSelector("li>span>span.fancytree-title"));
        }

        public String DescriptionText() { return BasePage.Driver.FindElement(By.CssSelector("div#FolderDescriptionText")).Text; }
        public IWebElement TopLevelEditBox() { return BasePage.Driver.FindElement(By.CssSelector(".fancytree-edit-input")); }
        public By By_Btn_ArchiveFolder() { return By.CssSelector("input#ArchiveFolderButton");}
        public IWebElement Btn_ArchiveFolder() { return BasePage.Driver.FindElement(By_Btn_ArchiveFolder()); }
        public IWebElement Div_Confirmation() { return BasePage.Driver.FindElement(By.CssSelector("div#ConfirmationDiv")); }
        public By Div_ConfirmationBy() { return By.CssSelector("div#ConfirmationDiv"); }
        public By By_Btn_OKConfirmationDiv(int locale = 0)
        {
            if (locale == 0) return By.CssSelector("div#ConfirmationDiv input[value='OK']");
            else return By.CssSelector("div#ConfirmationDiv input[value='" + ReadDataFromResourceFile(Localization.GlobalResourceFilePath, "data", "Button_OK") + "']");
        }
        public IWebElement Btn_OKConfirmationDiv(int locale = 0)
        {
            if (locale == 0) return BasePage.Driver.FindElement(By_Btn_OKConfirmationDiv());
            else return BasePage.Driver.FindElement(By_Btn_OKConfirmationDiv(1));
        }
        public By By_Btn_CancelConfirmationDiv(int locale = 0)
        {
            if (locale == 0) return By.CssSelector("div#ConfirmationDiv input[value='Cancel']");
            else return By.CssSelector("div#ConfirmationDiv input[value='" + ReadDataFromResourceFile(Localization.GlobalResourceFilePath, "data", "Button_Cancel") + "']");
        }
        public IWebElement Btn_Cancel_ConfirmationDiv(int locale = 0)
        {
            if (locale == 0) return BasePage.Driver.FindElement(By_Btn_CancelConfirmationDiv());
            else return BasePage.Driver.FindElement(By_Btn_CancelConfirmationDiv(1));
        }
        public By By_Btn_ArchiveMode() { return By.CssSelector("input#ArchiveModeButton"); }
        public IWebElement Btn_ArchiveMode() { return BasePage.Driver.FindElement(By_Btn_ArchiveMode()); }
        public By By_Btn_ActiveMode() { return By.CssSelector("input#ActiveModeButton"); }
        public IWebElement Btn_ActiveMode() { return BasePage.Driver.FindElement(By_Btn_ActiveMode()); }
        public By By_Btn_UndoArchiveFolder() { return By.CssSelector("input[name='UnarchiveFolderButton']"); }
        public IWebElement Btn_UndoArchiveFolder() { return BasePage.Driver.FindElement(By_Btn_UndoArchiveFolder()); }
        public IWebElement Btn_DeleteFolder() { return BasePage.Driver.FindElement(By.CssSelector("input#DeleteFolderButton")); }
        public By Btn_DeleteFolderBy() { return By.CssSelector("input#DeleteFolderButton"); }
        public By Btn_DeleteStudyBy() { return By.CssSelector("input[name='DeleteStudiesButton']"); }
        public By By_Btn_EditStudyNotes() { return By.CssSelector("input#EditStudyNotesButton"); }
        public IWebElement Btn_EditStudyNotes() { return BasePage.Driver.FindElement(By_Btn_EditStudyNotes()); }
        public IWebElement TxtArea_StudyNotes() { return BasePage.Driver.FindElement(By.CssSelector("textarea#TextAreaStudyNotes")); }
        public IWebElement Btn_OkStudyNotes() { return BasePage.Driver.FindElement(By.CssSelector("input#SubmitStudyNotesButton")); }
        public By Div_ManageTopLevelFolder() { return By.CssSelector("div#divFolderManagement"); }
        public By By_Btn_StudyNotes_HistoryPanel() { return By.CssSelector("#m_patientHistory_StudyNotesButton"); }
        public IWebElement Btn_StudyNotes_HistoryPanel() { return BasePage.Driver.FindElement(By_Btn_StudyNotes_HistoryPanel()); }
        public IWebElement Div_StudyNotes_HistoryPanel() { return BasePage.Driver.FindElement(By.CssSelector("#m_patientHistory_m_StudyNotesViewer_studyNotesContent")); }
        public By By_FolderManagerList() { return By.CssSelector("#divFolderManagers>div"); }
        public IList<IWebElement> AllSubfolders() { return BasePage.Driver.FindElements(By.CssSelector("#treeDiv>ul>li>ul>li>span>span.fancytree-title")); }
        //public By By_FolderConfigurationdiv() { return By.CssSelector("form[name='form1'] div [id='ConferenceFolderManagementDialogDiv']"); }
        public By By_FolderManager(String name) { return By.CssSelector("div[id='divFolderManagers']>div[id='" + name + "']"); }
        public IList<IWebElement> AutoCompleteDiv() { return BasePage.Driver.FindElements(By.CssSelector("#AutoCompleteDiv div")); }
        public By By_LeftPane() { return By.CssSelector("td div#ConferenceFolderBrowserControlDiv"); }
        public IWebElement FolderBrowserSection() { return BasePage.Driver.FindElement(By_LeftPane()); }
        public By By_MidPane() { return By.CssSelector("table[style*='collapse'] td[style*='width:65%']"); }
        public IWebElement ConferenceStudieSection() { return BasePage.Driver.FindElement(By_MidPane()); }
        public By By_RightPane() { return By.CssSelector("table[style*='collapse'] td#ColumnStudyNotes"); }
        public IWebElement CommentsSection() { return BasePage.Driver.FindElement(By_RightPane()); }
        public By By_ColumnHeaders() { return By.CssSelector("tr[role='rowheader']"); }
        public By By_StudyGrid() { return By.CssSelector("table#gridTableConferenceStudyRecords"); }
        public By By_ChooseColumns() { return By.CssSelector("td[title='Launch Column Chooser']"); }
        public By By_Reset() { return By.CssSelector("td[title*='Reset']"); }       
        public By By_ViewStudyBtn() { return By.CssSelector("input#ViewStudyButton"); }
        public String RecordSummaryText() { return BasePage.Driver.FindElement(By.CssSelector("td#gridPagerDiv_right div.ui-paging-info")).Text; }
        public By By_StudyNoteTime() { return By.CssSelector("span#LabelStudyNoteTimeStamp"); }
        public IWebElement DeleteStudyBtn() { return BasePage.Driver.FindElement(Btn_DeleteStudyBy()); }
        public String DeleteStudies_Caption() { return Div_Confirmation().FindElement(By.CssSelector("div>span#ctl00_ConfirmationCaption")).Text; }
        public IWebElement Btn_CancelStudyNotes() { return BasePage.Driver.FindElement(By.CssSelector("input#CancelEditStudyNotesButton")); }
        public IWebElement CancelFolderBtn() { return BasePage.Driver.FindElement(By.CssSelector("input#ConferenceFolderManagementControl_Cancel")); }
        public IWebElement FolderSaveDialog() { return BasePage.Driver.FindElement(By.CssSelector("span#folderSaveConfigMsgLabel")); }
        public IList<IWebElement> FolderManagerListed() { return BasePage.Driver.FindElements(By_FolderManagerList()); }
        public String DeleteFManager_Caption() { return BasePage.Driver.FindElement(By.CssSelector("span#ConferenceFolderManagementControl_DeleteTitleLabel")).GetAttribute("innerHTML"); }
        public IWebElement FManager_CancelBtn() { return BasePage.Driver.FindElement(By.CssSelector("input#ConferenceFolderManagementControl_CancelButton")); }
        public String FolderManager_Name(){return BasePage.Driver.FindElement(By.CssSelector("div[id='divFolderManagers']>div")).GetAttribute("innerHTML");}
        public By By_TopLevelEditBox() { return By.CssSelector(".fancytree-edit-input"); }
        public static String btnUniversalviewer = "input#UniversalViewStudyButton";
        public static String btnEnterpriseviewer = "input#EnterpriseViewStudyButton";

        #endregion Webelements

        #region ReusableComponents_HelperMethods
        /// <summary>
        /// To Verify top level folder Exist or not
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="folderName"></param>
        public bool IsTopLevelFolderExist(string domain, string folderName)
        {
            new SelectElement(BasePage.Driver.FindElement(By.CssSelector("#m_resultsSelectorControl_m_selectorList"))).SelectByText(domain);
            try
            {
                SwitchtoConferenceFrame();
                //if (!IsElementPresent(By.CssSelector("div[id='treeDiv'] ul li:nth-child(1)")))
                if (!IsElementPresent(By.CssSelector("div[id='treeDiv'] ul li")))
                {
                    return false;
                }
                else
                {
                    //-- have to remove commented code-- IE-8 changes-
                    //int folderCount = Driver.FindElements(By.XPath("//div[@id='treeDiv']/ul/li")).Count;
                    //for (int i = 1; i < folderCount + 1; i++)
                    //{
                    //    if (Driver.FindElement(By.XPath("//div[@id='treeDiv']/ul/li[" + i + "]/span/span[2]")).Text.Equals(folderName))
                    //    {
                    //        return true;
                    //    }
                    //}
                    IList<IWebElement> topFolder = Driver.FindElements(By.CssSelector("div[id='treeDiv'] ul li"));
                    foreach(IWebElement ele in topFolder)
                    {
                        if (ele.FindElement(By.CssSelector("span>span.fancytree-title")).Text.Equals(folderName))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// To Verify Sub level1 folder Exist or not under Top Level folder
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="TopFolder"></param>
        /// <param name="SubFolder1"></param>
        public bool IsSubLevel2FolderExist(string Domain, string TopFolder, string SubFolder2)
        {
            try
            {
                SwitchtoConferenceFrame();
                if (IsTopLevelFolderExist(Domain, TopFolder))
                {
                    ClickTopLevelFolderExpander(TopFolder);
                    if (IsElementPresent(By.XPath(GetXathOfSubLevel2Folder(TopFolder, SubFolder2))))
                    {
                        return true;
                    }
                }
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// To return the XPath of Top Level Folder Expander
        /// </summary>
        /// <param name="FolderName"></param>        
        public string GetXPathOfTopLevelFolderExpander(string FolderName)
        {
            string xpath = null;
            //if (IsElementPresent(By.CssSelector("div[id='treeDiv'] ul li:nth-child(1)")))
            if (IsElementPresent(By.CssSelector("div[id='treeDiv'] ul li")))
            {
                int folderCount = Driver.FindElements(By.XPath("//div[@id='treeDiv']/ul/li")).Count;
                for (int i = 1; i < folderCount + 1; i++)
                {
                    if (Driver.FindElement(By.XPath("//div[@id='treeDiv']/ul/li[" + i + "]/span/span[2]")).Text.Equals(FolderName))
                    {
                        xpath = "//div[@id='treeDiv']/ul/li[" + i + "]/span/span[1]";
                    }
                }
            }
            return xpath;
        }

        /// <summary>
        /// To Click Top Level Folder Expander
        /// </summary>
        /// <param name="FolderName"></param> 
        public void ClickTopLevelFolderExpander(string FolderName)
        {
            Driver.FindElement(By.XPath(GetXPathOfTopLevelFolderExpander(FolderName))).Click();
        }

        /// <summary>
        /// To Get the Xpath of Top level folder
        /// </summary>        
        /// <param name="folderName"></param>
        public string GetXPathOfTopLevelFolder(string folderName)
        {
            string xpath = null;
            //if (IsElementPresent(By.CssSelector("div[id='treeDiv'] ul li:nth-child(1)")))
            if (IsElementPresent(By.CssSelector("div[id='treeDiv'] ul li")))
            {
                int folderCount = Driver.FindElements(By.XPath("//div[@id='treeDiv']/ul/li")).Count;
                for (int i = 1; i < folderCount + 1; i++)
                {
                    if (Driver.FindElement(By.XPath("//div[@id='treeDiv']/ul/li[" + i + "]/span/span[2]")).Text.Equals(folderName))
                    {
                        xpath = "//div[@id='treeDiv']/ul/li[" + i + "]/span/span[2]";
                    }
                }
            }
            return xpath;
        }

        /// <summary>
        /// To Create sub level1 folder
        /// </summary>
        /// <param name="toplevelfolder"></param>
        /// <param name="level1folderName"></param>
        public void CreateSubLevel2Folder(string TopLevelFolder, string Level2FolderName, string Domain = null, string Description = null)
        {
            SelectFolder(TopLevelFolder, 1);
            CreateFolderButton().Click();
            Driver.FindElement(By.CssSelector(".fancytree-edit-input")).SendKeys(Level2FolderName);
            Driver.FindElement(By.CssSelector(".fancytree-edit-input")).SendKeys(Keys.Enter);
            if (Description != null)
            {
                if (IsSubLevel2FolderExist(Domain, TopLevelFolder, Level2FolderName))
                {
                    SelectFolder(TopLevelFolder, 2);
                    EditDescription(Level2FolderName, Description);
                }
            }
        }

        /// <summary>
        /// To edit top level folder
        /// </summary>
        /// <param name="folderName"></param>  
        /// <param name="editString"></param> 
        public void EditTopLevelFolder(string folderName, string editString)
        {
            SelectFolder(folderName, 1);
            Thread.Sleep(3000);
            SelectFolder(folderName, 1);
            Driver.FindElement(By.CssSelector(".fancytree-edit-input")).SendKeys(editString);
            Driver.FindElement(By.CssSelector(".fancytree-edit-input")).SendKeys(Keys.Enter);
        }

        /// <summary>
        /// To Create top level folder with or without FolderManager
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="foldermanager"></param>
        public Boolean CreateToplevelFolder(string folderName, string foldermanager = null, string domain = null, string description = null)
        {
            try
            {
                if (domain != null)
                {
                    new SelectElement(BasePage.Driver.FindElement(By.CssSelector("#m_resultsSelectorControl_m_selectorList"))).SelectByText(domain);
                }
                SwitchtoConferenceFrame();
                wait.Until(ExpectedConditions.ElementToBeClickable(ManageTopLevelFolders()));
                ManageTopLevelFolders().Click();
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector("form[name='form1'] div [id='ConferenceFolderManagementDialogDiv']")));
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div[id='divFolderManagement'] input[id='ConferenceFolderManagementControl_AddFolderButton']")));
                SwitchtoConferenceFrame();
                AddFolderBtn().Click();
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#newFolder")));
                //Thread.Sleep(3000);
                InputFolder().Click();
                InputFolder().SendKeys(folderName);
                new Actions(BasePage.Driver).SendKeys(Keys.Enter).Perform();
                BasePage.wait.Until<Boolean>(d => {
                var folders = d.FindElements(By.CssSelector("div#divFolderManagement div#divtopLevelFolder>div"));
                if (folders.Any(folder => folder.GetAttribute("innerHTML").Contains(folderName)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
                });
                if (foldermanager != null)
                {
                    AddFolderManager(folderName, foldermanager);
                }
                PageLoadWait.WaitForFrameLoad(20);
                SwitchtoConferenceFrame();
                SaveFolderBtn().Click();
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("form[name='form1'] div [id='ConferenceFolderManagementDialogDiv']")));
                SwitchtoConferenceFrame();
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div[id='ConferenceFolderConfigurationMsgDialogDiv']")));
                ManageFolderOkBtn().Click();
                PageLoadWait.WaitForFrameLoad(20);

                if (description != null)
                {
                    this.ExpandAndSelectFolder(folderName);
                    EditDescription(description);
                    Logger.Instance.InfoLog(description + "is Added as description to" + folderName);
                }

                IList<IWebElement> TopLevelFolders = AllTopLevelfolders();
                foreach (IWebElement Topfolder in TopLevelFolders)
                {
                    if (Topfolder.Text.Equals(folderName))
                    {
                        Logger.Instance.InfoLog(folderName + "is Created as TopLevel Folder");
                        return true;
                    }

                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in Top level folder: " + ex);
                Logger.Instance.ErrorLog("Exception in CreateToplevelFolder: " + ex);
                throw ex;
            }
        }

        /// <summary>
        /// This emthod is to Select Top level folder in Manage Top level folder
        /// </summary>
        /// <param name="folder"></param>
        public void SelectTopFolderInManageTopLevelFolder(IWebElement folder)
        {
            String cssselector = "div#" + folder.GetAttribute("id");
            String jsscript = "document.querySelector(" + "\"" + cssselector + "\"" + ").click()";
            ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(jsscript);
        }

        /// <summary>
        /// To add Folder manager to a Toplevel folder
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="user">If Mutiple folder manager should be added, then contactinate this value with "-"</param>
        public Boolean AddFolderManager(string folderName, string user,bool isLDAPuser=false,string LDAPusername=null)
        {
            PageLoadWait.WaitForFrameLoad(20);
            IList<IWebElement> folders = GetToplevelFolders();
            if (folders.Count == 0) { throw new Exception("Top Folder Not Found"); }
            bool isTopFolderFound = false;            
            foreach (IWebElement folder in folders)
            {
                if (folder.GetAttribute("innerHTML").Contains(folderName))
                {
                    folder.Click();
                    isTopFolderFound = true;
                    break;
                }
            }

            if (!isTopFolderFound) { throw new Exception("Top Folder Not Found"); }

            if (isLDAPuser)
            {
                FolderManagerTextBox().Click();
                //FolderManagerTextBox().SendKeys(user);
                new Actions(BasePage.Driver).Click(FolderManagerTextBox()).SendKeys(user).Build().Perform();
                SwitchtoConferenceFrame();
                try
                {
                    PageLoadWait.WaitForFrameLoad(30);
                    PageLoadWait.WaitForSuggestionToLoad();
                    wait.Until(ExpectedConditions.ElementToBeClickable(FolderManager_Suggestion()));
                    FolderManager_Suggestion().Click();
                    if (FolderManager_Name().Equals(LDAPusername))
                    {
                        wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div[id='divFolderManagers']>div")));
                        Driver.FindElement(By.CssSelector("div[id='divFolderManagers']>div")).Click();
                        Logger.Instance.InfoLog("Selected FolderManager" + LDAPusername);
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog(" FolderManager" + LDAPusername + "not found");
                        return false;
                    }
                }

                catch (Exception)
                {
                    Logger.Instance.InfoLog(" FolderManager" + user + "not found");
                    return false;
                }
            }
            else
            {
                //Add Folder Manager
                IList<Boolean> isuseradded = new List<Boolean>();
                foreach (String username in user.Split('-'))
                {
                    FolderManagerTextBox().Click();
                    //FolderManagerTextBox().SendKeys(user);
                    new Actions(BasePage.Driver).Click(FolderManagerTextBox()).SendKeys(username).Build().Perform();
                    SwitchtoConferenceFrame();
                    try
                    {
                        PageLoadWait.WaitForFrameLoad(30);
                        PageLoadWait.WaitForSuggestionToLoad();
                        wait.Until(ExpectedConditions.ElementToBeClickable(FolderManager_Suggestion()));
                        FolderManager_Suggestion().Click();
                        if (FolderManager(user).Displayed)
                        {
                            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div[id='divFolderManagers']>div")));
                            FolderManager(user).Click();
                            Logger.Instance.InfoLog("Selected FolderManager" + user);
                            isuseradded.Add(true);
                        }
                        else
                        {
                            Logger.Instance.InfoLog(" FolderManager" + user + "not found");
                            isuseradded.Add(false);
                        }
                    }

                    catch (Exception)
                    {
                        Logger.Instance.InfoLog(" FolderManager" + user + "not found");
                        isuseradded.Add(false);
                    }                    
                }
                return isuseradded.Contains(false) ? false : true;
            }            
        }

        /// <summary>
        /// Switching to the inner frame (Tabcontent)
        /// </summary>
        public void SwitchtoConferenceFrame()
        {
            PageLoadWait.WaitForFrameLoad(10);            
            PageLoadWait.WaitForPageLoad(15);
        }

        /// <summary>
        /// This function will Create SubFolder
        /// </summary>
        /// <param name="folderpath">folderpath should be in format Parent/Sub/sub</param>
        /// <param name="FolderName">Name of the Folder</param>
        /// <param name="Domain_Name">Domain Name</param>
        /// <param name="Description">Description of Folder</param>
        public Boolean CreateSubFolder(String folderpath, String FolderName, String Domain_Name = null, String Description = null)
        {

            //Select Folder
            ExpandAndSelectFolder(folderpath, Domain_Name);

            IWebElement Syncup = PageLoadWait.WaitForElement(By.CssSelector("#treeDiv>ul"), WaitTypes.Visible, 30);
            //Create new folder
            CreateFolderButton().Click();
            List<IWebElement> inputbox = Syncup.FindElements(By.TagName("input")).ToList();
            Driver.FindElement(By.CssSelector(".fancytree-edit-input")).SendKeys(FolderName);
            Driver.FindElement(By.CssSelector(".fancytree-edit-input")).SendKeys(Keys.Enter);

            if (Description != null)
            {
                EditDescription(Description);
                Logger.Instance.InfoLog("Subfolder is created and description is added");
            }
            //Adding sync up
            foreach (IWebElement item in inputbox)
            {
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(".fancytree-edit-input")));
            }
            //New dimmer sync up
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("#ConferenceStudyListDimmerDiv")));
            if (ActiveFolder().Text.Equals(FolderName))
            {

                Logger.Instance.InfoLog("Subfolder is created");
                return true;
            }
            else
            {

                Logger.Instance.InfoLog("Subfolder is not created");
                return false;
            }
        }

        /// <summary>
        /// This function will Expand And Select Folder given folder
        /// </summary>
        /// <param name="folderpath">Top_1/Level_2/Level_3</param>
        /// <param name="DomainName">Domain_Name</param>         
        public new IWebElement ExpandAndSelectFolder(String folderpath, String DomainName = null)
        {
            return base.ExpandAndSelectFolder(folderpath, DomainName);
        }

        /// <summary>
        /// To Edit/Delete the description of any folder
        /// </summary>
        /// <param name="Description"></param>
        /// <param name="action">OK/Cancel</param>
        /// <param name="del">1 to delete</param>
        /// <param name="subfolder"></param>
        public void EditDescription(string Description, string action = "OK", int del = 0)
        {

            SwitchtoConferenceFrame();
            //EditDesc_Button().Click();
            this.ClickElement(EditDesc_Button());
            //wait.Until(ExpectedConditions.ElementToBeClickable(EditDesc_TextArea()));
            BasePage.wait.Until<Boolean>(d =>
            {
                if (EditDesc_TextArea().GetAttribute("style").Contains("display: inline-block"))
                { return true; }
                else { return false; }
            });
            EditDesc_TextArea().Click();
            EditDesc_TextArea().Clear();
            try { EditDesc_TextArea().SendKeys(Description); }
            catch (ElementNotVisibleException)
            {
                this.ClickElement(EditDesc_Button());
                EditDesc_TextArea().Click();
                EditDesc_TextArea().Clear();
                EditDesc_TextArea().SendKeys(Description);
            }
            if (!(action.Equals("OK")))
            {
                EditDesc_CANCELbutton().Click();
            }
            else
            {
                EditDesc_OKbutton().Click();
            }
            wait.Until(ExpectedConditions.ElementToBeClickable(EditDesc_CurrentDesc()));
            Logger.Instance.InfoLog("New description modified as " + Description);
            if (del != 0)
            {
                EditDesc_Button().Click();
                EditDesc_TextArea().Click();
                EditDesc_TextArea().Clear();
                EditDesc_OKbutton().Click();
                Logger.Instance.InfoLog("Deleted the description");
            }

        }

        /// <summary>
        /// This function will delete the description of the Folder.
        /// before calling this function, ExpandAndSelect should perform
        /// </summary>
        public void DeleteDescription()
        {
            SwitchtoConferenceFrame();
            wait.Until(ExpectedConditions.ElementToBeClickable(EditDesc_TextArea()));
            EditDesc_TextArea().Click();
            EditDesc_TextArea().Clear();
            EditDesc_OKbutton().Click();
            wait.Until(ExpectedConditions.ElementToBeClickable(EditDesc_CurrentDesc()));
            Logger.Instance.InfoLog("Description deleted (Cleared) Successfully ");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TopFolder"></param>
        /// <param name="SubLevel2Folder"></param>
        /// <returns></returns>
        public string GetXathOfSubLevel2Folder(string TopFolder, string SubLevel2Folder)
        {
            string xpath = null;
            int folderCount = Driver.FindElements(By.XPath("//div[@id='treeDiv']/ul/li")).Count;
            for (int i = 1; i < folderCount + 1; i++)
            {
                if (Driver.FindElement(By.XPath("//div[@id='treeDiv']/ul/li[" + i + "]/span/span[2]")).Text.Equals(TopFolder))
                {
                    /* try
                     {
                         PageLoadWait.WaitForFrameLoad(30);
                         wait.Until(ExpectedConditions.ElementToBeClickable(ActiveTopFolder_Expander()));
                         ActiveTopFolder_Expander().Click();
                     }
                     catch(Exception e)
                     {
                         wait.Until(ExpectedConditions.ElementToBeClickable(ActiveTopFolder_Expander()));
                         ActiveTopFolder_Expander().Click();
                     }*/
                    ClickActiveExpander();
                    //((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.queryselector(\"ul[class='ui-fancytree fancytree-container fancytree-plain fancytree-ext-edit'] li>span[class*='active']>span[class*='expander']\").click()");
                    int SubFolderCount = Driver.FindElements(By.XPath("//div[@id='treeDiv']/ul/li[" + i + "]/ul/li")).Count;
                    for (int j = 1; j < SubFolderCount + 1; j++)
                    {
                        if (Driver.FindElement(By.XPath("//div[@id='treeDiv']/ul/li[" + i + "]/ul/li[" + j + "]/span/span[2]")).Text.Equals(SubLevel2Folder))
                        {
                            xpath = "//div[@id='treeDiv']/ul/li[" + i + "]/ul/li[" + j + "]/span/span[2]";
                        }
                        //break;
                    }
                }
            }
            return xpath;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClickActiveExpander()
        {
            String script = @"(function(){
                            var span  = document.querySelector('#treeDiv > ul > li > span > span.fancytree-expander');
                            span.click();
                            })();";
            //ul[class='ui-fancytree fancytree-container fancytree-plain fancytree-ext-edit'] li>span[class*='active']>span[class*='expander']
            //#treeDiv > ul > li:nth-child(1) > span > span.fancytree-expander

            ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);
        }

        /// <summary>
        /// Selecting a folder
        /// </summary>
        /// <param name="TopLevelFolder"></param>
        /// <param name="Level"></param>
        /// <param name="SubLevel2Folder"></param>
        /// <param name="SubLevel3Folder"></param>
        public void SelectFolder(string TopLevelFolder, int Level, string SubLevel2Folder = null, string SubLevel3Folder = null)
        {
            if (Level == 1)
            {
                SwitchtoConferenceFrame();
                Driver.FindElement(By.XPath(GetXPathOfTopLevelFolder(TopLevelFolder))).Click();
                Thread.Sleep(2000);
            }
            else if (Level == 2)
            {
                SwitchtoConferenceFrame();
                Driver.FindElement(By.XPath(GetXPathOfTopLevelFolder(TopLevelFolder))).Click();
                Driver.FindElement(By.XPath(GetXathOfSubLevel2Folder(TopLevelFolder, SubLevel2Folder))).Click();
                Thread.Sleep(2000);
            }
            else if (Level == 3)
            {
                SwitchtoConferenceFrame();
                Driver.FindElement(By.XPath(GetXPathOfTopLevelFolder(TopLevelFolder))).Click();
                Driver.FindElement(By.XPath(GetXathOfSubLevel2Folder(TopLevelFolder, SubLevel2Folder))).Click();
                /*Need to include for subfolder level3*/
                Thread.Sleep(2000);
            }
            else
            {
                //Logger
            }
        }

        /// <summary>
        /// To modify the folder name
        /// </summary>
        /// <param name="Folderpath">Parent/Sub1/Sub2</param>
        /// <param name="Foldername">Specific folder name</param>
        /// <param name="NewName">New Folder name</param>
        /// <param name="DomainName"></param>
        public void RenameFolder(String Folderpath, string NewName, string DomainName = null)
        {
            String browser = ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower();
            ExpandAndSelectFolder(Folderpath, DomainName);
            ActiveFolder().Click();
            PageLoadWait.WaitForFolderPathToChange(ActiveFolder().Text);

            if (browser.Equals("chrome") || browser.Equals("internet explorer"))
            {
                ActiveFolder().Click();
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector(".fancytree-edit-input")));
            }

            Driver.FindElement(By.CssSelector(".fancytree-edit-input")).Clear();
            Driver.FindElement(By.CssSelector(".fancytree-edit-input")).SendKeys(NewName);
            Driver.FindElement(By.CssSelector(".fancytree-edit-input")).SendKeys(Keys.Enter);
            PageLoadWait.WaitForFolderPathToChange(NewName);

            Logger.Instance.InfoLog("Foldername is modified as" + NewName);


        }

        /// <summary>
        /// To check if method exists in Subfolder2
        /// </summary>
        /// <param name="TopFolder"></param>
        /// <param name="SubFolder1"></param>
        /// <returns></returns>
        public bool IsSubLevel2FolderExist(string TopFolder, string SubFolder1)
        {
            try
            {
                SwitchtoConferenceFrame();
                if (IsElementPresent(By.XPath(GetXPathOfTopLevelFolder(TopFolder))))
                {
                    //ClickTopLevelFolderExpander(TopFolder);                    
                    if (IsElementPresent(By.XPath(GetXPathOfSubLevel2Folder(TopFolder, SubFolder1))))
                    {
                        return true;
                    }
                }
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// Gets the XPath of Subfloder-2
        /// </summary>
        /// <param name="TopFolder"></param>
        /// <param name="SubLevel2Folder"></param>
        /// <returns></returns>
        public string GetXPathOfSubLevel2Folder(string TopFolder, string SubLevel2Folder)
        {
            string xpath = null;
            int folderCount = Driver.FindElements(By.XPath("//div[@id='treeDiv']/ul/li")).Count;
            for (int i = 1; i < folderCount + 1; i++)
            {
                if (Driver.FindElement(By.XPath("//div[@id='treeDiv']/ul/li[" + i + "]/span/span[2]")).Text.Equals(TopFolder))
                {
                    int SubFolderCount = Driver.FindElements(By.XPath("//div[@id='treeDiv']/ul/li[" + i + "]/ul/li")).Count;
                    for (int j = 1; j < SubFolderCount + 1; j++)
                    {
                        if (Driver.FindElement(By.XPath("//div[@id='treeDiv']/ul/li[" + i + "]/ul/li[" + j + "]/span/span[2]")).Text.Equals(SubLevel2Folder))
                        {
                            xpath = "//div[@id='treeDiv']/ul/li[" + i + "]/ul/li[" + j + "]/span/span[2]";
                        }
                        //break;
                    }
                }
            }
            return xpath;
        }

        /// <summary>
        /// Gets the Xpath of Subfolder level-3
        /// </summary>
        /// <param name="TopFolder"></param>
        /// <param name="SubLevel3Folder"></param>
        /// <returns></returns>
        public string GetXPathOfSubLevel3Folder(string TopFolder, string SubLevel3Folder)
        {
            string xpath = null;
            int folderCount = Driver.FindElements(By.XPath("//div[@id='treeDiv']/ul/li")).Count;
            for (int i = 1; i < folderCount + 1; i++)
            {
                int SubLevel2FolderCount = Driver.FindElements(By.XPath("//div[@id='treeDiv']/ul/li[" + i + "]/ul/li")).Count;
                for (int j = 1; j < SubLevel2FolderCount + 1; j++)
                {
                    int SubLevel3FolderCount = Driver.FindElements(By.XPath("//div[@id='treeDiv']/ul/li[" + i + "]/ul/li[" + j + "]/ul/li")).Count;
                    for (int k = 1; k < SubLevel3FolderCount + 1; k++)
                    {
                        if (Driver.FindElement(By.XPath("//div[@id='treeDiv']/ul/li[" + i + "]/ul/li[" + j + "]/ul/li[" + k + "]/span/span[2]")).Text.Equals(SubLevel3Folder))
                        {
                            xpath = "//div[@id='treeDiv']/ul/li[" + i + "]/ul/li[" + j + "]/ul/li[" + k + "]/span/span[2]";
                        }
                        //break;
                    }
                }
            }
            return xpath;
        }

        /// <summary>
        /// EXpands and Selects Sube level-2 Folder.
        /// </summary>
        /// <param name="Level"></param>
        /// <param name="TopFolder"></param>
        /// <param name="Level2Folder"></param>
        public void ClickSubLevelFolderExpander(int Level, string TopFolder, string Level2Folder)
        {
            if (Level == 2)
            {
                Driver.FindElement(By.XPath(GetXPathOfLevel2FolderExpander(TopFolder, Level2Folder))).Click();
            }
        }

        /// <summary>
        /// Gets XPATH of Level2 FOlder expander
        /// </summary>
        /// <param name="TopFolder"></param>
        /// <param name="SubLevel2Folder"></param>
        /// <returns></returns>
        public string GetXPathOfLevel2FolderExpander(string TopFolder, string SubLevel2Folder)
        {
            string xpath = null;
            int folderCount = Driver.FindElements(By.XPath("//div[@id='treeDiv']/ul/li")).Count;
            for (int i = 1; i < folderCount + 1; i++)
            {
                if (Driver.FindElement(By.XPath("//div[@id='treeDiv']/ul/li[" + i + "]/span/span[2]")).Text.Equals(TopFolder))
                {
                    int SubFolderCount = Driver.FindElements(By.XPath("//div[@id='treeDiv']/ul/li[" + i + "]/ul/li")).Count;
                    for (int j = 1; j < SubFolderCount + 1; j++)
                    {
                        if (Driver.FindElement(By.XPath("//div[@id='treeDiv']/ul/li[" + i + "]/ul/li[" + j + "]/span/span[2]")).Text.Equals(SubLevel2Folder))
                        {
                            xpath = "//div[@id='treeDiv']/ul/li[" + i + "]/ul/li[" + j + "]/span/span[1]";
                        }
                        //break;
                    }
                }
            }
            return xpath;
        }

        /// <summary>
        /// To Remove a FolderManager from TopLevel folder
        /// </summary>
        public void RemoveFolderManager(string folderName, string user,string LDAPuser="")
        {
            SwitchtoConferenceFrame();
            IList<IWebElement> folders = GetToplevelFolders();
            foreach (IWebElement folder in folders)
            {
                if (folder.Text.Equals(folderName))
                {
                    folder.Click();
                    break;
                }
            }
            if (LDAPuser == "")
            {
                wait.Until(ExpectedConditions.ElementToBeClickable(FolderManager(user)));
                FolderManager(user).Click();
            }
            else
            {
                foreach(IWebElement name in FolderManagerListed())
                {
                    if (FolderManager_Name().Equals(LDAPuser))
                    {
                        name.Click();
                        break;
                    }
                }
            }
           
            DeleteFolderManagerBtn().Click();
            wait.Until(ExpectedConditions.ElementToBeClickable(FolderManagerOKBtn()));
            FolderManagerOKBtn().Click();

        }

        /// <summary>
        /// To save TopLevel folder in ManageFolder.
        /// </summary>
        public void SaveTopLevelfolder()
        {
            PageLoadWait.WaitForFrameLoad(20);
            SwitchtoConferenceFrame();
            SaveFolderBtn().Click();
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("form[name='form1'] div [id='ConferenceFolderManagementDialogDiv']")));
            SwitchtoConferenceFrame();
            PageLoadWait.WaitForLoadingIndicatorToAppear_Conference();
            PageLoadWait.WaitForLoadingIndicatorToDisAppear_Conference();
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div[id='ConferenceFolderConfigurationMsgDialogDiv']")));
            ManageFolderOkBtn().Click();
            PageLoadWait.WaitForFrameLoad(20);

        }

        /// <summary>
        /// This method will Archive the selected conference Folder
        /// </summary>
        /// <param name="folderpath">Complte Path of Folder starting from TopFolder</param>
        public void ArchiveConferenceFolder(String folderpath, String DomainName = null, int locale = 0)
        {
            //Select Folder
            this.ExpandAndSelectFolder(folderpath, DomainName);
            PageLoadWait.WaitForLoadingDivToAppear_Conference(10);
            PageLoadWait.WaitForLoadingDivToDisAppear_Conference(10);
            //Archive and synchup
            this.ClickElement(this.Btn_ArchiveFolder());
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            if (locale == 0)
            {
                BasePage.wait.Until<Boolean>(driver =>
                {
                    if (this.Div_Confirmation().FindElements(By.CssSelector("div#ConfirmationDiv>div"))[0].FindElement(By.CssSelector("span")).GetAttribute("innerHTML").ToLower().Equals("archive folder"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            }
            else
            {
                String ArchiveFolder = ReadDataFromResourceFile(Localization.ConferenceStudyList, "data", "ConfirmationBox_ArchiveFolder");
                BasePage.wait.Until<Boolean>(driver =>
                {
                    if (this.Div_Confirmation().FindElements(By.CssSelector("div#ConfirmationDiv>div"))[0].FindElement(By.CssSelector("span")).GetAttribute("innerHTML").Equals(ArchiveFolder))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            }
            try{this.Btn_OKConfirmationDiv().Click();}
            catch (Exception){Driver.FindElement(By.CssSelector("#ctl00_ConfirmButton")).Click();}            
            BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(Div_ConfirmationBy()));
            this.SwitchtoConferenceFrame();
            BasePage.wait.Until<Boolean>(driver => driver.FindElements(By.CssSelector("table#gridTableConferenceStudyRecords tr")).Count == 1);
        }

        /// <summary>
        ///  Navigate to Active Tab in Conference Tab
        /// </summary>
        public void NavigateToActiveMode()
        {
            if (!this.Btn_ActiveMode().GetAttribute("style").Contains("opacity: 1"))
                //this.ClickElement(this.Btn_ActiveMode());
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"input#ActiveModeButton\").click();");
            BasePage.wait.Until<Boolean>(d => this.Btn_ActiveMode().GetAttribute("style").Contains("opacity: 1"));
        }

        /// <summary>
        /// Navigate to Archive Tab in Conference Tab
        /// </summary>
        public void NavigateToArchiveMode()
        {
            if (!this.Btn_ArchiveMode().GetAttribute("style").Contains("opacity: 1"))
                // this.ClickElement(this.Btn_ArchiveFolder());
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"input#ArchiveModeButton\").click();");
            BasePage.wait.Until<Boolean>(d => this.Btn_ArchiveMode().GetAttribute("style").Contains("opacity: 1"));

        }

        /// <summary>
        /// This method will undo Archive folder
        /// </summary>
        /// <param name="folderpath"></param>
        public void UndoArchiveFolder(String folderpath, int locale = 0)
        {
            //Navigate to Archive Tab
            this.NavigateToArchiveMode();
            this.ExpandAndSelectFolder(folderpath);
            this.Btn_UndoArchiveFolder().Click();

            //Syncup
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            if (locale == 0)
            {
                BasePage.wait.Until<Boolean>(driver =>
                {
                    if (this.Div_Confirmation().FindElements(By.CssSelector("div#ConfirmationDiv>div"))[0].FindElement(By.CssSelector("span")).GetAttribute("innerHTML").ToLower().Equals("restore archived folder"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            }
            else
            {
                String restore = ReadDataFromResourceFile(Localization.ConferenceStudyList, "data", "ConfirmationBox_UndoArchiveFolder");
                BasePage.wait.Until<Boolean>(driver =>
                {
                    if (this.Div_Confirmation().FindElements(By.CssSelector("div#ConfirmationDiv>div"))[0].FindElement(By.CssSelector("span")).GetAttribute("innerHTML").Equals(restore))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            }

            try
            {
                this.Btn_OKConfirmationDiv().Click();
            }
            catch (Exception)
            {
                Driver.FindElement(By.CssSelector("#ctl00_ConfirmButton")).Click();
            }
            this.SwitchtoConferenceFrame();
            BasePage.wait.Until<Boolean>(driver => driver.FindElements(By.CssSelector("table#gridTableConferenceStudyRecords tr")).Count == 1);

        }

        /// <summary>
        /// To Delete folder as per the given path
        /// </summary>
        /// <param name="folderpath"></param>
        public void DeleteFolder(String folderpath, int locale = 0)
        {
            IWebElement folder = this.ExpandAndSelectFolder(folderpath);
            BasePage.wait.Until<IWebElement>(d =>
            {
                if (d.FindElement(this.Btn_DeleteFolderBy()).Enabled)
                {
                    return d.FindElement(this.Btn_DeleteFolderBy());
                }
                else
                {
                    return null;
                }
            }).Click();

            //Syncup
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            if (locale == 0)
            {
                BasePage.wait.Until<Boolean>(driver =>
                {
                    if (this.Div_Confirmation().FindElements(By.CssSelector("div#ConfirmationDiv>div"))[0].FindElement(By.CssSelector("span")).GetAttribute("innerHTML").ToLower().Equals("delete folder"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            }
            else
            {
                String delete = ReadDataFromResourceFile(Localization.ConferenceStudyList, "data", "ConfirmationBox_DeleteFolder");
                BasePage.wait.Until<Boolean>(driver =>
                {
                    if (this.Div_Confirmation().FindElements(By.CssSelector("div#ConfirmationDiv>div"))[0].FindElement(By.CssSelector("span")).GetAttribute("innerHTML").Equals(delete))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            }
            try
            {
                this.Btn_OKConfirmationDiv().Click();
            }
            catch (Exception)
            {
                Driver.FindElement(By.CssSelector("#ctl00_ConfirmButton")).Click();
            }
            this.SwitchtoConferenceFrame();
            PageLoadWait.WaitForPageLoad(10);
        }

        /// <summary>
        /// Delete Coneference Study from Conference Tab
        /// As pre-requisite expand and select the study folder
        /// </summary>
        /// <param name="searchparameter"></param>
        /// <param name="value"></param>
        public void DeleteStudy(String searchparameter, String value = null, Boolean ctrlclick = false, String[] values = null, int locale = 0)
        {
            var rows = BasePage.GetSearchResults();
            int rowcount = rows.Count;
            if (!ctrlclick)
            {
                this.SelectStudy(searchparameter, value);
            }
            else
            {
                foreach (String columnvalue in values)
                {
                    this.SelectStudy1(searchparameter, columnvalue, ctrlclick);
                }
            }
            BasePage.wait.Until<IWebElement>(d =>
            {
                if (d.FindElement(this.Btn_DeleteStudyBy()).Enabled)
                {
                    return d.FindElement(this.Btn_DeleteStudyBy());
                }
                else
                {
                    return null;
                }

            }).Click();

            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            BasePage.wait.Until<Boolean>(d => this.Div_Confirmation().FindElement(By.CssSelector("div>span"))
            .GetAttribute("innerHTML").ToLower().Contains("delete studies"));
            try
            {
                if (locale == 0)
                    this.Btn_OKConfirmationDiv().Click();
                else
                    this.Btn_OKConfirmationDiv(1).Click();
            }
            catch (Exception) { Driver.FindElement(By.CssSelector("#ctl00_ConfirmButton")).Click(); }
            
            this.SwitchtoConferenceFrame();

            //Syncup
            try
            {
                BasePage.wait.Until<Boolean>(d =>
                {
                    if (BasePage.GetSearchResults().Count == rowcount - 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                });
            }
            catch(Exception){}

        }

        /// <summary>
        /// To update Study Notes
        /// Use this method after Selecting Study
        /// </summary>
        /// <param name="notes"></param>
        public void ModifyConferenceStudyNotes(String notes, String mode = "Add")
        {
            this.Btn_EditStudyNotes().Click();
            BasePage.wait.Until<Boolean>(d =>
            {
                if (!this.TxtArea_StudyNotes().GetAttribute("style").ToLower().Contains("readonly"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

            if (mode.ToLower().Equals("add"))
            {
                this.TxtArea_StudyNotes().Clear();
                this.TxtArea_StudyNotes().SendKeys(notes);
            }
            else if (mode.ToLower().Equals("append"))
            {
                String existingnotes = this.GetConferenceStudyNotes();
                this.TxtArea_StudyNotes().Clear();
                this.TxtArea_StudyNotes().SendKeys(existingnotes + notes);
            }
            else if (mode.ToLower().Equals("remove"))
            {
                this.TxtArea_StudyNotes().Clear();
            }

            this.Btn_OkStudyNotes().Click();
        }

        /// <summary>
        /// To return the value of ConferenceStudy Notes
        /// </summary>
        /// <returns></returns>
        public String GetConferenceStudyNotes()
        {
            String Notes;
            String script = @"function value(){
                            var Text  = document.querySelector('textarea#TextAreaStudyNotes').value;                            
                            return Text;
                            }return value();";
            Notes = ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script).ToString();

            return Notes;

        }

        /// <summary>
        /// Opens the Dialog Manage Top Level Folder
        /// </summary>
        public void OpenManageTopLevelFolder()
        {
            this.ManageTopLevelFolders().Click();
            BasePage.wait.Until(ExpectedConditions.ElementIsVisible(this.Div_ManageTopLevelFolder()));
        }

        /// <summary>
        /// This helper method will close the Manage Top Level folder dialog box when opened
        /// </summary>
        public void CloseManageTopLevelFolder()
        {
            if (!BasePage.Driver.FindElement(By.CssSelector("div#folderManagementPointerDialog")).GetAttribute("style").Contains("display: none"))
            {
                BasePage.Driver.FindElement(By.CssSelector("input#ConferenceFolderManagementControl_Cancel")).Click();
                BasePage.wait.Until<Boolean>(d => d.FindElement(By.CssSelector("div#folderManagementPointerDialog")).GetAttribute("style").Contains("display: none"));
            }
        }

        /// <summary>
        /// This Method will return Conference Study notes/comments from conference tab when a study is selected. Ensure the study is selected before calling this function
        /// </summary>
        /// <returns>string</returns>
        public string GetStudyNotesFromTextArea()
        {
            var js = Driver as IJavaScriptExecutor;
            string notes = (string)js.ExecuteScript("return jQuery('#TextAreaStudyNotes').val()");
            return notes;
        }

        public void SelectTopFolderInConfigDiv(string TopFolder)
        {
            IList<IWebElement> TopFolders1 = GetToplevelFolders();
            foreach (IWebElement folder in TopFolders1)
            {
                if (folder.Text.Equals(TopFolder))
                {
                    folder.Click();
                    break;
                }
            }
        }

        /// <summary>
        /// This is to Add top folder in TopFolder dialog
        /// </summary>
        public void AddTopFolder(String Foldername)
        {
            wait.Until(ExpectedConditions.ElementExists(By_FolderConfigurationdiv()));
            wait.Until(ExpectedConditions.ElementToBeClickable(AddFolderBtn()));
            SwitchtoConferenceFrame();
            AddFolderBtn().Click();
            wait.Until(ExpectedConditions.ElementToBeClickable(InputFolder()));
            InputFolder().Click();
            InputFolder().SendKeys(Foldername);
            new Actions(BasePage.Driver).SendKeys(Keys.Enter).Perform();
            BasePage.wait.Until<Boolean>(d =>
            {
                var folders = d.FindElements(By.CssSelector("div#divFolderManagement div#divtopLevelFolder>div"));
                if (folders.Any(folder => folder.GetAttribute("innerHTML").Contains(Foldername)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

        }

        #endregion ReusableComponents_HelperMethods

    }
}
