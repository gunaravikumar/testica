using System;
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
using Selenium.Scripts.Pages.MPAC;
using Selenium.Scripts.Pages.Putty;
using System.Globalization;
using Selenium.Scripts.Pages.iConnect;

namespace Selenium.Scripts.Pages.iConnect
{
    public class UserPreferences : BasePage
    {
        // RadioButton Selectors
        public static String LocalizerOn = "#ViewingProtocolsControl_LocalizerLineRadioButtons_0";
        public static String LocalizerOff = "#ViewingProtocolsControl_LocalizerLineRadioButtons_1";

        //Label
        public IWebElement UserPreferenceName() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#PreferencesTitle"))); return Driver.FindElement(By.CssSelector("#PreferencesTitle")); }
        //Radiobutton
        public IWebElement JPEGRadioBtn() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#NonTransientImageFormatRadioButtonList_0"))); return BasePage.Driver.FindElement(By.CssSelector("#NonTransientImageFormatRadioButtonList_0")); }
        public IWebElement PNGRadioBtn() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#NonTransientImageFormatRadioButtonList_1"))); return BasePage.Driver.FindElement(By.CssSelector("#NonTransientImageFormatRadioButtonList_1")); }
        public IWebElement HTML5RadioBtn() { return BasePage.Driver.FindElement(By.CssSelector("#DefaultViewerSettingRadioButtonList_0")); }
        public By hTML4RadioBtn() { return By.CssSelector("#DefaultViewerSettingRadioButtonList_1"); }
        public IWebElement HTML4RadioBtn() { return BasePage.Driver.FindElement(hTML4RadioBtn()); }
        public By bluringViewerRadioBtn() { return By.CssSelector("#DefaultViewerSettingRadioButtonList_0"); }
        public IWebElement BluringViewerRadioBtn() { return BasePage.Driver.FindElement(bluringViewerRadioBtn()); }
        public IWebElement ThumbnailSplittingAutoRadioBtn() { return BasePage.Driver.FindElement(By.CssSelector("#ViewingProtocolsControl_ThumbSplitRadioButtons_0")); }
        public IWebElement ThumbnailSplittingSeriesRadioBtn() { return BasePage.Driver.FindElement(By.CssSelector("#ViewingProtocolsControl_ThumbSplitRadioButtons_1")); }
        public IWebElement ThumbnailSplittingImageRadioBtn() { return BasePage.Driver.FindElement(By.CssSelector("#ViewingProtocolsControl_ThumbSplitRadioButtons_2")); }
        public IWebElement ViewingScopeSeriesRadioBtn() { return BasePage.Driver.FindElement(By.CssSelector("#ViewingProtocolsControl_ScopeRadioButtons_0")); }
        public IWebElement ViewingScopeImageRadioBtn() { return BasePage.Driver.FindElement(By.CssSelector("#ViewingProtocolsControl_ScopeRadioButtons_1")); }
        public IWebElement GrantYesEmailNotificationBtn() { wait.Until(ExpectedConditions.ElementIsVisible(By.Id("SharedStudyForUserNotificationRadioButtonList_0"))); return BasePage.Driver.FindElement(By.Id("SharedStudyForUserNotificationRadioButtonList_0")); }
        public IWebElement GrantNoEmailNotificationBtn() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#SharedStudyForUserNotificationRadioButtonList_1"))); return BasePage.Driver.FindElement(By.CssSelector("input#SharedStudyForUserNotificationRadioButtonList_1")); }
        public IWebElement EmailFormatHTML() { wait.Until(ExpectedConditions.ElementIsVisible(By.Id("EmailFormatRadioButtonList_0"))); return BasePage.Driver.FindElement(By.Id("EmailFormatRadioButtonList_0")); }
        public IWebElement EmailFormatText() { wait.Until(ExpectedConditions.ElementIsVisible(By.Id("EmailFormatRadioButtonList_1"))); return BasePage.Driver.FindElement(By.Id("EmailFormatRadioButtonList_1")); }
        public IWebElement DefaultStartPageStudies() { wait.Until(ExpectedConditions.ElementIsVisible(By.Id("DefaultPageRadioButtonList_0"))); return BasePage.Driver.FindElement(By.Id("DefaultPageRadioButtonList_0")); }
        public IWebElement DownloadStudiesAsZipFiles() { wait.Until(ExpectedConditions.ElementIsVisible(By.Id("DownloadRadioButtonList_0"))); return BasePage.Driver.FindElement(By.Id("DownloadRadioButtonList_0")); }
        public IWebElement AutoCINE_ON() { wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ViewingProtocolsControl_AutoStartCineRadioButtons_0"))); return BasePage.Driver.FindElement(By.Id("ViewingProtocolsControl_AutoStartCineRadioButtons_0")); }
        public IWebElement AutoCINE_OFF() { wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ViewingProtocolsControl_AutoStartCineRadioButtons_1"))); return BasePage.Driver.FindElement(By.Id("ViewingProtocolsControl_AutoStartCineRadioButtons_1")); }
        public IWebElement ExamMode(string mode) { return Driver.FindElement(By.CssSelector("input[id$='_ExamModeRadioButtons_" + mode + "']")); }
        public SelectElement DefaultUploaderList() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("select#defaultUploaderList"))); return new SelectElement(Driver.FindElement(By.CssSelector("select#defaultUploaderList"))); }
        public IWebElement Localizer_ON() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(LocalizerOn))); return BasePage.Driver.FindElement(By.CssSelector(LocalizerOn)); }
        public IWebElement Localizer_OFF() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(LocalizerOff))); return BasePage.Driver.FindElement(By.CssSelector(LocalizerOff)); }
        public IWebElement ExamMode_ON() { wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ViewingProtocolsControl_ExamModeRadioButtons_0"))); return BasePage.Driver.FindElement(By.Id("ViewingProtocolsControl_ExamModeRadioButtons_0")); }
        public IWebElement ExamMode_OFF() { wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ViewingProtocolsControl_ExamModeRadioButtons_1"))); return BasePage.Driver.FindElement(By.Id("ViewingProtocolsControl_ExamModeRadioButtons_1")); }


        //Checkboxes
        public IWebElement LoadStudyInFullScreenChkBox() { return BasePage.Driver.FindElement(By.CssSelector("#LoadStudyInFullScreenCB")); }
        public IWebElement CardioOrderChkBox() { return BasePage.Driver.FindElement(By.CssSelector("#ViewingProtocolsControl_CardioOrderCheckBox")); }
        public IWebElement PatientRecordLiveSearch() { return BasePage.Driver.FindElement(By.Id("PRLiveSearchCB")); }
        public IWebElement PatientRecordLiveSearchChkBox() { return BasePage.Driver.FindElement(By.CssSelector("#PRLiveSearchCB")); }
        public IWebElement EnableConnectionTestTool() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#ConnTestToolCB"))); return BasePage.Driver.FindElement(By.CssSelector("input#ConnTestToolCB")); }
        public IWebElement JavaEIChkBox() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#defaultToJavaExamImporterCB"))); return BasePage.Driver.FindElement(By.CssSelector("defaultToJavaExamImporterCB")); }
        
        //Buttons
        public IWebElement SavePreferenceBtn() { return BasePage.Driver.FindElement(By.CssSelector("input#SavePreferenceUpdateButton")); }
        public IWebElement CancelPreferenceBtn() { return BasePage.Driver.FindElement(By.CssSelector("#CancelPreferenceUpdateButton")); }
        public IWebElement ResultLable() { return BasePage.Driver.FindElement(By.CssSelector("#ResultLabel")); }
        public IWebElement CloseBtn() { return BasePage.Driver.FindElement(By.CssSelector("#CloseResultButton")); }
        public IWebElement AddModifyBtn() { return BasePage.Driver.FindElement(By.CssSelector("#ViewingProtocolsControl_SaveAliasButton")); }
        public IWebElement RemoveBtn() { return BasePage.Driver.FindElement(By.CssSelector("#ViewingProtocolsControl_RemoveAliasButton")); }
        public IWebElement ExternalAppCommitBtn() { return BasePage.Driver.FindElement(By.CssSelector("input#m_prefAppUserPassBtn")); }

        //Dropdownlist
        public SelectElement ModalityDropDown() { return new SelectElement(Driver.FindElement(By.CssSelector("#ViewingProtocolsControl_DropDownListModalities"))); }
        public SelectElement LayoutDropDown() { return new SelectElement(Driver.FindElement(By.CssSelector("#ViewingProtocolsControl_DropDownListLayout"))); }
        public SelectElement PresetsDropDown() { return new SelectElement(Driver.FindElement(By.CssSelector("#ViewingProtocolsControl_DropDownListAlias"))); }
        public By By_DefaultUploaderDropdown() { return By.CssSelector("select[id$='defaultUploaderList']"); }
        public SelectElement DefaultUploaderDropdown() { return new SelectElement(PageLoadWait.WaitForElement(By_DefaultUploaderDropdown(), WaitTypes.Visible)); }
        public SelectElement ExternalApplicationDropdown() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("select#m_prefAppDropDownList"))); return new SelectElement(Driver.FindElement(By.CssSelector("select#m_prefAppDropDownList"))); }
        //Textboxes
        public IWebElement PresetNameTextBox() { return BasePage.Driver.FindElement(By.CssSelector("#ViewingProtocolsControl_AliasTextBox")); }
        public IWebElement WidthTextBox() { return BasePage.Driver.FindElement(By.CssSelector("#ViewingProtocolsControl_WidthTextBox")); }
        public IWebElement LevelTextBox() { return BasePage.Driver.FindElement(By.CssSelector("#ViewingProtocolsControl_LevelTextBox")); }
        public IWebElement CineDefaultFrameRate() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#DefaultFrameRateTextBox"))); return BasePage.Driver.FindElement(By.CssSelector("input#DefaultFrameRateTextBox")); }
        public IWebElement CineMaxMemory() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#MaxMemoryTextBox"))); return BasePage.Driver.FindElement(By.CssSelector("input#MaxMemoryTextBox")) ; }
        public IWebElement CineFrameDelay() { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#MaxMemoryTextBox"))); return BasePage.Driver.FindElement(By.CssSelector("#CineSingleFrameDelayTimeInSecondsTextBox")); }
        public IWebElement ExternalAppUserIdTextBox() { return BasePage.Driver.FindElement(By.CssSelector("input#UserIDTextBox")); }
        public IWebElement ExternalAppPasswordTextBox() { return BasePage.Driver.FindElement(By.CssSelector("input#PasswordTextBox")); }
        //CineSingleFrameDelayTimeInSecondsTextBox
        //All in one tool 
        public SelectElement AllinOneLMB() { return new SelectElement(Driver.FindElement(By.CssSelector("div[id='AllInOneSelectionDiv'] select[id='dpLeftButtonFunctions']"))); }
        public SelectElement AllinOneMMB() { return new SelectElement(Driver.FindElement(By.CssSelector("div[id='AllInOneSelectionDiv'] select[id='dpMiddleButtonFunctions']"))); }
        public SelectElement AllinOneRMB() { return new SelectElement(Driver.FindElement(By.CssSelector("div[id='AllInOneSelectionDiv'] select[id='dpRightButtonFunctions']"))); }
        

        public void SwitchToToolBarUserPrefFrame()
        {
            SwitchToDefault();
            SwitchTo("index", "0");
            SwitchTo("id", "m_UserprefFrame");
        }

        /// <summary>
        ///     This function Saves and Closes the User Preferences open through Tool Bar option
        /// </summary>
        /// <param>
        ///     <name></name>
        /// </param>
        public void SaveToolBarUserPreferences()
        {
            BasePage.Driver.SwitchTo().DefaultContent();
            BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_UserprefFrame");            
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#SavePreferenceUpdateButton")));
            SavePreferenceBtn().Click();
            PageLoadWait.WaitForFrameLoad(20);
            //wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#CloseResultButton")));

            SwitchToToolBarUserPrefFrame();
            CloseBtn().Click();

            SwitchToDefault();
            SwitchTo("index", "0");
        }
        /// <summary>
        ///     This function Saves and Closes the User Preferences
        /// </summary>
        public void ClickSaveToolBarUserPreferences()
        {
            SwitchToToolBarUserPrefFrame();

            //BasePage.Driver.SwitchTo().DefaultContent();
            //BasePage.Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_UserprefFrame");
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#SavePreferenceUpdateButton")));
            SavePreferenceBtn().Click();
            //PageLoadWait.WaitForFrameLoad(20);
            //wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#CloseResultButton")));
            SwitchToToolBarUserPrefFrame();
        }

        public void AddPresetAtUserLevel(string modality, string preset, string width, string level, string layout = "auto")
        {
            OpenUserPreferences();
            SwitchToToolBarUserPrefFrame();
            SwitchToDefault();
            SwitchTo("index", "0");
            SwitchTo("id", "m_preferenceFrame");
            //Driver.SwitchTo().Frame("m_preferenceFrame");
            Thread.Sleep(2000);

            var selectModality = new SelectElement(GetElement("id", "ViewingProtocolsControl_DropDownListModalities"));
            Thread.Sleep(1000);
            var selectlayout = new SelectElement(GetElement("id", "ViewingProtocolsControl_DropDownListLayout"));
            Thread.Sleep(1000);
            IWebElement presetName = GetElement("id", "ViewingProtocolsControl_AliasTextBox");
            IWebElement widthField = GetElement("id", "ViewingProtocolsControl_WidthTextBox");
            IWebElement levelField = GetElement("id", "ViewingProtocolsControl_LevelTextBox");
            IWebElement savePreset = GetElement("id", "ViewingProtocolsControl_SaveAliasButton");

            Thread.Sleep(1000);
            selectModality.SelectByText(modality);
            selectlayout.SelectByText(layout);
            presetName.Clear();
            presetName.SendKeys(preset);
            Thread.Sleep(1000);
            widthField.Clear();
            widthField.SendKeys(width);
            levelField.Clear();
            levelField.SendKeys(level);
            savePreset.Click();
            savePreset.Click();

            Thread.Sleep(2000);

            CloseUserPreferences();
        }

        public void AddPresetAtToolbar(string modality, string preset, string width, string level, string layout = "auto")
        {
            SwitchToToolBarUserPrefFrame();
            ModalityDropDown().SelectByText(modality);
            PresetNameTextBox().Clear();
            PresetNameTextBox().SendKeys(preset);
            WidthTextBox().Clear();
            WidthTextBox().SendKeys(width);
            LevelTextBox().Clear();
            LevelTextBox().SendKeys(level);
            AddModifyBtn().Click();
        }

        public bool VerifyPresetsInUserPreference(string modality, string layout, string preset, bool value = true)
        {
            bool IsPresetPresent = false;
            OpenUserPreferences();
            //Thread.Sleep(5000);
            //PageLoadWait.WaitForFrameLoad(20);
            //Driver.SwitchTo().Frame("m_preferenceFrame");                        
            SwitchToDefault();
            SwitchTo("index", "0");
            SwitchTo("id", "m_preferenceFrame");
            ModalityDropDown().SelectByText(modality);
            LayoutDropDown().SelectByText(layout);
            IList<IWebElement> options = Driver.FindElement(By.CssSelector("select[id='ViewingProtocolsControl_DropDownListAlias']")).FindElements(By.TagName("option"));
            if (options.Count > 0)
            {
                foreach (IWebElement option in options)
                {
                    if (value)
                    {
                        if (option.Text.Equals(preset))
                        {
                            IsPresetPresent = true;
                        }
                    }
                    else if (options.Count > 0 && !value)
                    {
                        if (option.Text.Equals(preset))
                        {
                            IsPresetPresent = false;
                        }
                    }

                }
            }
            else if (!(options.Count > 0) && !value)
            {
                IsPresetPresent = true;
            }
            else
            {
                IsPresetPresent = false;
            }
            Click("id", "CancelPreferenceUpdateButton");
            Thread.Sleep(3000);

            return IsPresetPresent;
        }

        public void ModifyPresetsInToolBarUserPref(string modality, string layout, string preset, string width, string level, bool isStudiesTab=true)
        {
            if (isStudiesTab)
            {
                SwitchToToolBarUserPrefFrame();
            }
            else
            {
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("m_preferenceFrame");
            }
            SelectFromList("id", "ViewingProtocolsControl_DropDownListModalities", modality, 1);
            SelectFromList("id", "ViewingProtocolsControl_DropDownListLayout", layout, 1);
            SelectFromList("id", "ViewingProtocolsControl_DropDownListAlias", preset, 1);
            ClearText("id", "ViewingProtocolsControl_WidthTextBox");
            SetText("id", "ViewingProtocolsControl_WidthTextBox", width);
            ClearText("id", "ViewingProtocolsControl_LevelTextBox");
            SetText("id", "ViewingProtocolsControl_LevelTextBox", level);
            Click("id", "ViewingProtocolsControl_SaveAliasButton");
        }

        public void SwitchToUserPrefFrame()
        {
            SwitchToDefault();
            SwitchTo("index", "0");
            SwitchTo("id", "m_preferenceFrame");
        }

        public void SetThumbnailSplittingAtUserLevel(string modality, string thumbnailSplitting = "auto")
        {
            OpenUserPreferences();
            BasePage.Driver.SwitchTo().DefaultContent();
            BasePage.Driver.SwitchTo().Frame(0);
            SwitchTo("id", "m_preferenceFrame");
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ViewingProtocolsControl_DropDownListModalities")));
           
            ModalityDropDown().SelectByText(modality);

            switch (thumbnailSplitting.ToLower())
            {
                case "auto":
                    ThumbnailSplittingAutoRadioBtn().Click();
                    break;
                case "series":
                    ThumbnailSplittingSeriesRadioBtn().Click();
                    break;
                case "image":
                    ThumbnailSplittingImageRadioBtn().Click();
                    break;
            }
            CloseUserPreferences();
        }

        public void SetThumbnailSplitting(string option)
        {
            try { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ViewingProtocolsControl_DropDownListModalities"))); }
            catch (NoSuchElementException e) { }

            switch (option.ToLower())
            {
                case "auto":
                    ThumbnailSplittingAutoRadioBtn().Click();
                    break;
                case "series":
                    ThumbnailSplittingSeriesRadioBtn().Click();
                    break;
                case "image":
                    ThumbnailSplittingImageRadioBtn().Click();
                    break;
            }

            Logger.Instance.InfoLog(option + " is set");
        }

        /// <summary>
        /// This method is used to Enable/Disable the Localizer based on Modality
        /// </summary>
        /// <param name="ModalityList"></param>
        /// <param name="Enable"></param> To enable pass "true" and to disable pass "False"
        public void SetLocalizerByModality(String[] modalityList, bool enable = true)
        {
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ViewingProtocolsControl_DropDownListModalities")));
            foreach (String Modality in modalityList)
            {
                ModalityDropDown().SelectByText(Modality);
                if (enable)
                {
                    Localizer_ON().Click();
                    Logger.Instance.InfoLog("The localizer turned ON for " + Modality + " modality");
                }
                else
                {
                    Localizer_OFF().Click();
                    Logger.Instance.InfoLog("The localizer turned OFF for " + Modality + " modality");
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// This method is used to Enable/Disable the Localizer based on Modality
        /// </summary>
        /// <param name="ModalityList"></param>
        /// <param name="Enable"></param> To enable pass "true" and to disable pass "False"
        public void SetLocalizerByModality(String modality, bool enable = true)
        {            
            ModalityDropDown().SelectByText(modality);
            if (enable)
            {
                Localizer_ON().Click();
                Logger.Instance.InfoLog("The localizer turned ON for " + modality + " modality");
            }
            else
            {
                Localizer_OFF().Click();
                Logger.Instance.InfoLog("The localizer turned OFF for " + modality + " modality");
            }
        }
    }
}
