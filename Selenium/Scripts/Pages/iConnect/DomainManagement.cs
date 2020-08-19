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
    class DomainManagement : BasePage
    {

        public DomainManagement() { }

        public IWebElement DomainNameTxtBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_DomainInfo_Name")); }
        public IWebElement DomainDescriptionTxtBox() { return Driver.FindElement(By.CssSelector("[id$='_DomainInfo_Description'")); }
        public IWebElement UserIDTxtBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_UserID")); }
        public IWebElement UserFirstNameTxtBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_FirstName")); }
        public IWebElement UserLastNameTxtBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_LastName")); }
        public IWebElement UserPasswordTxtBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_Password")); }
        public IWebElement UserConfirmPasswordTxtBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword")); }
        public IWebElement RoleNameTxtBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Name")); }
        public IWebElement RoleDescriptionTxtBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Description")); }
        public IWebElement SearchButton() { return Driver.FindElement(By.CssSelector("#m_domainSearchControl_m_searchButton")); }
        public IWebElement CloseButton() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_CloseButton")); }
        public IWebElement EditDomainCloseBtn() { return Driver.FindElement(By.CssSelector("[id$='DomainControl_CloseButton']")); }
        public IWebElement SaveButton() { return Driver.FindElement(By.CssSelector("input[id$='_SaveButton")); }
        public IWebElement CloseAlertButton() { return Driver.FindElement(By.CssSelector("#CloseButton")); }
        public IWebElement ReceivingInstTxtBox() { return Driver.FindElement(By.CssSelector("[id$='DomainInfo_ReceivingInstitution']")); }
        public IList<IWebElement> PageandNext() { return Driver.FindElements(By.CssSelector("span[id$='m_domainListControl_m_dataListGridPager']>span span[style*='text-decoration: underline;']")); }
        public IWebElement AutoCineOFF() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_m_viewingProtocolsControl_AutoStartCineRadioButtons_1")); }
        public IWebElement WebUploaderConsentCheckbox() { return PageLoadWait.WaitForElement(By.CssSelector("input[id$='_DisplayHIPAAComplianceCB']"), WaitTypes.Visible); }
        public IWebElement AutoCineON() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_m_viewingProtocolsControl_AutoStartCineRadioButtons_0")); }
        public IWebElement ExamModeON() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_m_viewingProtocolsControl_ExamModeRadioButtons_0")); }
        public IWebElement ExamModeON_DA() { return Driver.FindElement(By.CssSelector("#EditDomainControl_DomainInfo_m_viewingProtocolsControl_ExamModeRadioButtons_0")); }
        public IWebElement ExamModeOFF() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_m_viewingProtocolsControl_ExamModeRadioButtons_1")); }
        public IWebElement ExamModeOFF_DA() { return Driver.FindElement(By.CssSelector("#EditDomainControl_DomainInfo_m_viewingProtocolsControl_ExamModeRadioButtons_1")); }
        public IWebElement EditDomainButton() { return Driver.FindElement(By.CssSelector("#EditDomainButton")); }
        public IWebElement NewDomainButton() { return Driver.FindElement(By.CssSelector("#NewDomainButton")); }
        public IWebElement SaveDomainButtoninEditPage() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditDomainControl_SaveButton")); }
        public IWebElement ShowAllDomainCB() { return Driver.FindElement(By.CssSelector("#m_domainSearchControl_reset")); }
        public IWebElement EditDomainDescription() { return Driver.FindElement(By.CssSelector("[id$='EditDomainControl_DomainInfo_Description']")); }
        public IList<IWebElement> DomainDetails() { return Driver.FindElements(By.CssSelector("table[id='m_domainListControl_m_dataListGrid']>tbody>tr>td>span")); }
        public IList<IWebElement> DomainList() { return Driver.FindElements(By.CssSelector("table[id='m_domainListControl_m_dataListGrid']>tbody>tr")); }
        public IWebElement ResultcountLabel() { return Driver.FindElement(By.CssSelector("#m_listResultsControl_m_resultState")); }
        public IWebElement DoaminNameTb() { return Driver.FindElement(By.CssSelector("input#m_domainSearchControl_m_input1")); }
        public IWebElement EditDomainInstituitionNameTxtBox() { return Driver.FindElement(By.CssSelector("ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_InstitutionTextBox")); }
        public IList<IWebElement> ContactsAddedTb() { return Driver.FindElements(By.CssSelector("select[id$='EditDomainControl_DomainContacts_ContactsListBox']>option")); }
        public IList<IWebElement> DomainDetailsortBy() { return Driver.FindElements(By.CssSelector("table[id='m_domainListControl_m_dataListGrid']>tbody>tr>th>span>img")); }
        public IList<IWebElement> ColumnHeadings() { return Driver.FindElements(By.CssSelector("table[id='m_domainListControl_m_dataListGrid']>tbody>tr>th")); }
        public IWebElement DomainPwdTxtBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_UserInfo_Password")); }

        //Checkboxes
        public IWebElement EnablePrintCheckBox() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_PrintEnabledCB")); }
        public IWebElement OverlayCheckbox() { return Driver.FindElement(By.CssSelector("input[id$='_ThumbnailCaptionOverlayCB']")); }
        public IWebElement LinkAllCheckbox() { return Driver.FindElement(By.CssSelector("input[id$='_DomainInfo_LinkAllEnabledCB']")); }
        public IWebElement SaveStudyLayout() { return Driver.FindElement(By.CssSelector("input[id$='_AllowSaveStudyListLayoutEnabledCB']")); }  
        public IWebElement PatientIDCheckbox() { return Driver.FindElement(By.CssSelector("[id$='DomainInfo_QueryRelatedStudyParametersCBList_0']")); }
        public IWebElement PatientFullnameCheckbox() { return Driver.FindElement(By.CssSelector("[id$='DomainInfo_QueryRelatedStudyParametersCBList_1']")); }
        public IWebElement PatientLastnameCheckbox() { return Driver.FindElement(By.CssSelector("[id$='DomainInfo_QueryRelatedStudyParametersCBList_2']")); }
        public IWebElement PatientDOBCheckBox() { return Driver.FindElement(By.CssSelector("[id$='DomainInfo_QueryRelatedStudyParametersCBList_3']")); }
        public IWebElement IPIDCheckBox() { return Driver.FindElement(By.CssSelector("[id$='DomainInfo_QueryRelatedStudyParametersCBList_4']")); }
        public IWebElement consentwebuploader() { return Driver.FindElement(By.Id("ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_DisplayHIPAAComplianceCB")); }


        public By By_ConferenceListsCB() { return By.CssSelector("[id$='_EnableConferenceListsCB']"); }
        public IWebElement ConferenceListsCB() { return Driver.FindElement(By_ConferenceListsCB()); }
        public IWebElement ReportviewCB() { return Driver.FindElement(By.CssSelector("[id$='ReportViewingEnabledCB']")); }
        public IWebElement AttachmentUploadCB() { return Driver.FindElement(By.CssSelector("[id$='_AttachmentAllowUploadEnabledCB']")); }
        public IWebElement EmailStudyCB() { return Driver.FindElement(By.CssSelector("[id$='_EnableEmailStudyCB']")); }
        public By By_GSPScb() { return By.CssSelector("[id$='_DomainInfo_SavingGSPSCB']"); }
        public IWebElement SaveGspsCB() { return Driver.FindElement(By_GSPScb()); }
        public IWebElement MeaningfulUse() { return Driver.FindElement(By.CssSelector("input[id$='_EnableMUCB']")); }
		public IWebElement PercentageViewed() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_ShowThumbnailPercentImagesViewed")); }

        //by ravsoft
        public IWebElement EnableImagesharing() { return Driver.FindElement(By.XPath("//input[@id='ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_EnableImageSharingCB']")); }
        public IWebElement TxtReceivingInstutionName() { return Driver.FindElement(By.Id(Locators.ID.ReceivingInstitutionTextbox)); }
        public IWebElement PatientIdDomainDropBox() { return Driver.FindElement(By.Id(Locators.ID.eleEditDomainPrimaryDropDown)); }
        public IList<IWebElement> PatientDropDownOptions() { return PatientIdDomainDropBox().FindElements(By.TagName("option")); }
        public IWebElement PatientIdDomainDropBoxNewDomainPage() { return Driver.FindElement(By.Id("ctl00_MasterContentPlaceHolder_m_editDomainPidDomainControl_m_primaryDropDown")); }
        public IList<IWebElement> PatientDropDownOptionsNewDomainPage() { return PatientIdDomainDropBoxNewDomainPage().FindElements(By.TagName("option")); }
        public IWebElement ReceiveInst() { return Driver.FindElement(By.Id("ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_ReceivingInstitution")); }
        // Viewer Image radio buttons
        public IWebElement ViewerImage() { return Driver.FindElement(By.CssSelector("span[title = 'Select View Scope'] input[value = 'Image']")); }
        public IWebElement ViewerSeries() { return Driver.FindElement(By.CssSelector("span[title = 'Select View Scope'] input[value = 'Series']")); }

        //Lables 
        public IWebElement PageHeaderLabel() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditDomainControl_Label1")); }
        public IWebElement PwdRequirementIcon() { return Driver.FindElement(By.CssSelector("#PwdRequirementIcon")); }
        public IWebElement XIcon() { return Driver.FindElement(By.XPath("html/body/div[1]/div[1]/a/span")); }
        public IWebElement IntegratedUserSharingLabel() { return Driver.FindElement(By.CssSelector("[id$='EditDomainControl_DefaultRoleLabel']")); }
        public IWebElement IntegratedUserSharingDropdown() { return Driver.FindElement(By.CssSelector("select[id$='EditDomainControl_m_DefaultRoleInputControl_m_selectorList']")); }
        public IWebElement DomainNameLbl() { return Driver.FindElement(By.CssSelector("#m_domainSearchControl_m_input1Label")); }

        //New Domain Labels
        public IWebElement DomainManagementHeaderLabel() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_HyperLink1")); }
        public IWebElement DomainManagementHeaderLabelEditDomain() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditDomainControl_HyperLink1")); }
        public IWebElement NewDomainHeaderLabel() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_Label1")); }
        public IWebElement EnterDomaininfoLabel() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_InfoLabel")); }
        public IWebElement EnterDomaininfoLabelEditDomain() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditDomainControl_InfoLabel")); }
        public IWebElement DomaininfoLabel() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_DomainInfo_TitleLabel")); }
        public IWebElement DomaininfoLabelEditDomain() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_TitleLabel")); }
        public IWebElement InstitutionLabel() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_DataInfo_InstitutionTitleLabel")); }
        public IWebElement InstitutionLabelED() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_InstitutionTitleLabel")); }
        public IWebElement IpidLabel() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_DataInfo_IssuerTitleLabel")); }
        public IWebElement DataSourceLabel() { return Driver.FindElement(By.CssSelector("span[id$='_DataInfo_DataSourceTitleLabel']")); }
        public IWebElement StudySearchLabel() { return Driver.FindElement(By.CssSelector("span[id$='_m_sslConfigControl_StudySearchLayoutLabel']")); }
        public IWebElement IpadstudyLabel() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_m_ipadStudyListLayoutConfig1_IpadStudylistLayoutLabel")); }
        public IWebElement IpadstudyLabelED() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditDomainControl_m_ipadStudyListLayoutConfig_IpadStudylistLayoutLabel")); }
        public IWebElement DefaultModalitySettingLabel() { return Driver.FindElement(By.CssSelector("span[id$='_DomainInfo_m_viewingProtocolsControl_DefaultModalitySettingsLabel']")); }
        public IWebElement ContactLabel() { return Driver.FindElement(By.CssSelector("[id$='_ContactListLabel']")); }
        public IWebElement ArchiveNominationLabel() { return Driver.FindElement(By.CssSelector("span[id$='_NominateReasons_ReasonListLabel']")); }
        public IWebElement ToolbarConfigLabel() { return Driver.FindElement(By.CssSelector("span[id$='_DomainToolbarConfig']")); }
        public IWebElement ExternalApplicationsLabel() { return Driver.FindElement(By.CssSelector("[id$='_RoleAccessFilter_ApplicationTitleLabel")); }
        public IWebElement AccessFilterLabel() { return Driver.FindElement(By.CssSelector("[id$='_RoleAccessFilter_AccessFilterTitleLabel']")); }
        public IWebElement PatientHistoryLayoutLabel() { return Driver.FindElement(By.CssSelector("span[id$='_PatientHistoryLabel']")); }
        public IWebElement StudyListLayoutLabel() { return Driver.FindElement(By.CssSelector("span[id$='_StudyListLabel']")); }
        public IWebElement DomainAdminUserinfoLabel() { return Driver.FindElement(By.CssSelector("span[id$='_UserInfo_TitleLabel']")); }
        public IWebElement DomainAdminRoleinfoLabel() { return Driver.FindElement(By.CssSelector("span[id$='_RoleAccessFilter_RoleInfoTitleLabel']")); }

        //Presets
        public IWebElement PresetInvalidMsg() { return Driver.FindElement(By.CssSelector("[id$='_m_viewingProtocolsControl_ValidationErrorLabel']")); }
        public IWebElement PresetName() { return Driver.FindElement(By.CssSelector("input[id$='_AliasTextBox']")); }
        public IWebElement PresetsWidthField() { return Driver.FindElement(By.CssSelector("input[id$='_WidthTextBox']")); }
        public IWebElement PresetLevelField() { return Driver.FindElement(By.CssSelector("input[id$='_LevelTextBox']")); }
        public IWebElement savePreset() { return Driver.FindElement(By.CssSelector("input[id$='_SaveAliasButton']")); }
        public IList<IWebElement> PresetDropdown() { return BasePage.Driver.FindElements(By.CssSelector("select[id$='ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_m_viewingProtocolsControl_DropDownListAlias'] option")); }
        public SelectElement PresetSelect() { return new SelectElement(Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_m_viewingProtocolsControl_DropDownListAlias"))); }

        //Drop down List
        public SelectElement LayoutDropDown() { return new SelectElement(Driver.FindElement(By.CssSelector("select[id$='DropDownListLayout']"))); }
        public SelectElement ModalityDropDown() { return new SelectElement(Driver.FindElement(By.CssSelector("select[id$='DropDownListModalities']"))); }
        public By By_DefaultUploaderDropdown() { return By.CssSelector("select[id$='DefaultUploaderList']"); }
        public SelectElement DefaultUploaderDropdown() { return new SelectElement(PageLoadWait.WaitForElement(By_DefaultUploaderDropdown(), WaitTypes.Visible)); }
        public SelectElement DatasourceConnectedDropDown() { return new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select[id$='_DataSourceConnectedListBox']"))); }
        public IWebElement DefaultRoleDropDown() { return Driver.FindElement(By.CssSelector("select[id$='DefaultRoleInputControl_m_selectorList']")); }
        public SelectElement EditDomainDefaultRole() { return new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select[id$='_EditDomainControl_m_DefaultRoleInputControl_m_selectorList']"))); }
        public SelectElement CultureDropDownList() { return new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select[id$='_CultureDropDownList']"))); }

        //Connected DS List
        public IList<IWebElement> ConnectedDataSourceListBox() { return BasePage.Driver.FindElements(By.CssSelector("select[id$='DataSourceConnectedListBox']>option")); }


        //Button objects
        public SelectElement HiddenSearchField() { return new SelectElement(Driver.FindElement(By.CssSelector("select[id$='ssclHiddenSearchFieldsLB']"))); }
        public SelectElement VisibleSearchField() { return new SelectElement(Driver.FindElement(By.CssSelector("select[id$='ssclVisibleSearchFieldsLB']"))); }
        public IWebElement ShowBtn() { return Driver.FindElement(By.CssSelector("input[id$='ssclAddButton']")); }
        public IWebElement HideBtn() { return Driver.FindElement(By.CssSelector("input[id$='ssclRemoveButton']")); }

        public IWebElement GrantAccessValidDaysTxtBox() { return Driver.FindElement(By.CssSelector("input[id$='GrantAccessValidDaysTB']")); }
        public IWebElement EmailStudyValidDaysTxtBox() { return Driver.FindElement(By.CssSelector("input[id$='EmailedStudyValidDaysTB']")); }
        public IWebElement UploadStudyValidDaysTxtBox() { return Driver.FindElement(By.CssSelector("input[id$='UploadStudyValidDaysTB']")); }
        public IWebElement InstituitionAddButton() { return Driver.FindElement(By.CssSelector("ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_IncludeInstitutionButton")); }

        public static string btn_revertToDefault = "input[id$= 'ToolboxConfiguration_RevertToDefault']";
        public static string checkbox_enable3DView = "input[id$=ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_Enable3DViewCB]";

        public IWebElement UserMgmtLabel(string mode)
        {
            if (mode.Equals("New"))
            {
                return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditDomainControl_HyperLink1"));
            }
            else
            {
                return Driver.FindElement(By.CssSelector("#Container_Heading>a"));
            }
        }

        //ErrorMessage
        public String ErrorMessage() { return Driver.FindElement(By.CssSelector("span#ctl00_MasterContentPlaceHolder_EditDomainControl_ErrorMessage")).Text; }

        //CardioCine
        public IWebElement ExamMode(string mode) { return Driver.FindElement(By.CssSelector("input[id$='_ExamModeRadioButtons_"+mode+"']")); }

        //Internationalization
        public By By_DomainMgmtErrorMsg() { return By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage"); }
        public IWebElement DomainMgmtErrorMsg() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_ErrorMessage")); }
        public SelectElement RoleAccessFilterSelectList() { return new SelectElement(Driver.FindElement(By.CssSelector("[id$='RoleAccessFilter_FilterDropDownList']"))); }
        public SelectElement ModalityListBox() { return new SelectElement(Driver.FindElement(By.CssSelector("[id$='RoleAccessFilter_ModalityListBox']"))); }
        public IWebElement AddAccessFilterButton() { return Driver.FindElement(By.CssSelector("[id$='RoleAccessFilter_AddButton']")); }

        public IWebElement DomainNameEditLabel() { return Driver.FindElement(By.CssSelector("[id$='DomainInfo_NameInfoLabel']")); }
        public IWebElement ReceivingInstitutionLabel() { return Driver.FindElement(By.CssSelector("[id$='DomainInfo_ReceivingInstitutionLabel']")); }
        public IWebElement ConfirmationTextLabel() { return Driver.FindElement(By.CssSelector("#ctl00_ConfirmationText")); }
        public IWebElement AlertTextLabel() { return Driver.FindElement(By.CssSelector("#ctl00_ConfirmationText")); }
        public By domain_UniversalViewer() { return By.CssSelector("[id$='_DomainInfo_EnableUniversalViewerCB']"); }
        public IWebElement Domain_UniversalViewer() { return Driver.FindElement(domain_UniversalViewer()); }
        public IWebElement Role_UniversalViewer() { return Driver.FindElement(By.CssSelector("[id$='_RoleAccessFilter_EnableUniversalViewerCB']")); }
        public By defaultUniversalViewer() { return By.CssSelector("[id$='_DefaultViewerRadioButtonList_0']"); }
        public IWebElement DefaultUniversalViewer() { return Driver.FindElement(defaultUniversalViewer()); }
        public By defaultEnterpriseViewer() { return By.CssSelector("[id$='_DefaultViewerRadioButtonList_1']"); }
        public IWebElement DefaultEnterpriseViewer() { return Driver.FindElement(defaultEnterpriseViewer()); }
        public IWebElement Enable3DViewCheckbox() { return Driver.FindElement(By.CssSelector("[id$='_DomainInfo_Enable3DViewCB']")); }
        /// <summary>
        /// This function selects the given domain in the domain management tab when logged in as Admin
        /// </summary>
        /// <param name="domain"></param>
        public void SelectDomain(String domain)
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row tr td>span[title='" + domain + "']")));
            IList<IWebElement> d = Driver.FindElements(By.CssSelector("div.row tr td>span[title='" + domain + "']"));
            foreach (IWebElement elm in d)
            {
                String s = elm.Text;
                if (s.Equals(domain))
                {
                    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", elm);
                    Logger.Instance.InfoLog(domain + "domain is selected");
                    break;
                }
            }
            //PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
        }

        /// <summary>
        /// This Function Unchecks the "Use system settings" Checkbox in "Archive nomination reasons section" in
        /// edit domain management tab. When logged in as Admin
        /// </summary>
        /// <param name="TextBox"></param>
        /// <param name="ListBox"></param>
        /// <param name="AddBtn"></param>
        /// <param name="RemoveBtn"></param>
        public void UnChecKUseSystemSetting(out IWebElement TextBox, out IWebElement ListBox, out IWebElement AddBtn, out IWebElement RemoveBtn)
        {
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#NominateReasonsInputControl_Div")));
            Driver.FindElement(By.CssSelector("[id$='_UseSystemNominationReasonsCB']")).Click();
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[id$='_NominateReasons_nominateDiv']")));
            Logger.Instance.InfoLog("Use System settings Checkbox is Unchecked");
            TextBox = Driver.FindElement(By.CssSelector("[id$='_nominateTextBox']"));
            ListBox = Driver.FindElement(By.CssSelector("[id$='_nominateListBox']"));
            AddBtn = Driver.FindElement(By.CssSelector("[id$='_NominateReasons_AddButton']"));
            RemoveBtn = Driver.FindElement(By.CssSelector("[id$='_RemoveButton']"));
        }

        /// <summary>
        /// New reasons will be added in archive nominations field in Edit domain
        /// </summary>
        /// <param name="Reasons">This Reasons represents a set of values that would be added in nominations</param>
        public void AddArchiveNominationReasons(String[] Reasons)
        {
            if (Driver.FindElement(By.CssSelector("#NominateReasonsInputControl_Div")).Displayed == true)
            {
                IWebElement TextBox, ListBox, AddBtn, RemoveBtn;
                this.UnChecKUseSystemSetting(out TextBox, out ListBox, out AddBtn, out RemoveBtn);
                foreach (String reason in Reasons)
                {
                    TextBox.SendKeys(reason);
                    AddBtn.Click();
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#NominateReasonsInputControl_Div")));
                }
                PageLoadWait.WaitForPageLoad(30);
                this.ClickSaveEditDomain();
                Logger.Instance.InfoLog("New Archive nomination reasons are added");
            }
            else
            {
                Logger.Instance.ErrorLog("Archive Nomination Reasons field is not found");
            }
        }

        /// <summary>
        /// Given Reasons are removed from the Archive Nomination reasons
        /// </summary>
        /// <param name="Reasons">This Reasons represents a set of values that would be added in nominations</param>
        public void RemoveArchiveNominationReasons(String[] Reasons)
        {
            if (Driver.FindElement(By.CssSelector("#NominateReasonsInputControl_Div")).Displayed == true)
            {
                IWebElement TextBox, ListBox, AddBtn, RemoveBtn;
                TextBox = Driver.FindElement(By.CssSelector("[id$='_nominateTextBox']"));
                ListBox = Driver.FindElement(By.CssSelector("[id$='_nominateListBox']"));
                AddBtn = Driver.FindElement(By.CssSelector("[id$='_NominateReasons_AddButton']"));
                RemoveBtn = Driver.FindElement(By.CssSelector("[id$='_RemoveButton']"));
                foreach (String reason in Reasons)
                {
                    this.SelectFromList(ListBox, reason, 1);
                    RemoveBtn.Click();
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#NominateReasonsInputControl_Div")));
                }
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='_EditDomainControl_SaveButton']")));
                Driver.FindElement(By.CssSelector("[id$='_EditDomainControl_SaveButton']")).Click();
                PageLoadWait.WaitForFrameLoad(30);
                Logger.Instance.InfoLog("Given Archive nomination reasons are removed");
            }
            else
            {
                Logger.Instance.ErrorLog("Archive Nomination Reasons field is not found");
            }
        }

        /// <summary>
        /// This function clicks the save button in Edit domain 
        /// </summary>
        public void ClickSaveEditDomain()
        {
            PageLoadWait.WaitForFrameLoad(20);
            if (Driver.FindElement(By.CssSelector("[id$='EditDomainControl_SaveButton']")).Enabled == true)
            {
                this.ClickElement(Driver.FindElement(By.CssSelector("[id$='EditDomainControl_SaveButton']")));
                PageLoadWait.WaitForPageLoad(10);
                Thread.Sleep(3000);
                try
                {
                    if (BasePage.Driver.FindElement(By.CssSelector("div[id='ModalDialogDiv'] input[name='CloseButton']")).Displayed)
                    {
                        this.ClickElement(BasePage.Driver.FindElement(By.CssSelector("div[id='ModalDialogDiv'] input[name='CloseButton']")));
                    }
                }
                catch (Exception) { }
                PageLoadWait.WaitForFrameLoad(30);
                Logger.Instance.InfoLog("Save button in Edit Domain is clicked");
                Driver.SwitchTo().DefaultContent();
                PageLoadWait.WaitForFrameLoad(20);
            }
            else
            {
                Logger.Instance.ErrorLog("Save Button is not found in Edit Domain");
            }
        }


        /// <summary>
        ///     This function clicks on the Edit Domain button
        /// </summary>
        public void ClickEditDomain()
        {
            try
            {
                int timeout = 0;
                Boolean flag = true;
                while (flag && timeout < 4)
                {
                    SwitchToDefault();
                    SwitchTo("index", "0");
                    SwitchTo("index", "1");
                    SwitchTo("index", "0");
                    if (SBrowserName.ToLower().Equals("internet explorer") || SBrowserName.ToLower().Contains("edge"))
                    {
                        Click("id", "EditDomainButton", true);
                    }
                    else
                    {
                        Click("id", "EditDomainButton");
                    }
                    Thread.Sleep(3000);
                    if (SBrowserName.ToLower().Contains("edge"))
                    {
                        BasePage.Driver.SwitchTo().Window(BasePage.Driver.CurrentWindowHandle);
                    }
                    SwitchToDefault();
                    SwitchTo("index", "0");

                    if (GetElement("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_Label1") !=
                        null)
                    {
                        flag = false;
                    }
                    Thread.Sleep(1000);
                    timeout = timeout + 1;
                }

                Thread.Sleep(5000);
                SwitchToDefault();
                Thread.Sleep(2000);
                SwitchTo("index", "0");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception encountered in ClickEditDomain due to " + ex.Message);
            }
        }

        /// <summary>
        /// Check if domain exists
        /// </summary>
        /// <param>Domain Name to verify if this domain exists</param>
        /// <param name="domainName"></param>
        public Boolean DomainExists(string domainName)
        {
            try
            {
                ReadOnlyCollection<IWebElement> elements =
                    Driver.FindElements(
                        By.XPath("//table[@id='m_domainListControl_m_dataListGrid']/tbody/tr"));

                for (int i = 1; i < elements.Count + 1; i++)
                {
                    if (
                        elements[i - 1].FindElement(By.XPath("//tr[" + i + "]/td[1]/span"))
                                       .GetAttribute("title")
                                       .Equals(domainName))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception while verifying existing domaim names for : " + domainName +
                                         " for exception : " + ex.Message);
            }

            return false;
        }

        /// <summary>
        /// This is to click New Domain Botton
        /// </summary>
        public void ClickNewDomainBtn()
        {
            PageLoadWait.WaitForFrameLoad(15);
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            //Click("cssselector", "#NewDomainButton");
            ClickButton("#NewDomainButton");
            PageLoadWait.WaitForFrameLoad(30);
            PageLoadWait.WaitForFrameLoad(15);
        }

        public void MakeAllFieldsVisibleStudySearchFieldsInNewDomain()
        {
            try
            {
                string[] valueFromHidden = GetValuesfromDropDown("id", "ctl00_MasterContentPlaceHolder_m_sslConfigControl_ssclHiddenSearchFieldsLB");
                string[] valueFromVisible = GetValuesfromDropDown("id", "ctl00_MasterContentPlaceHolder_m_sslConfigControl_ssclVisibleSearchFieldsLB");

                bool valueSelected = false;
                int i = 0;
                while (valueSelected != true && i < 10)
                {
                    int k = 0;
                    string[] valueFromHiddennew = GetValuesfromDropDown("id", "ctl00_MasterContentPlaceHolder_m_sslConfigControl_ssclHiddenSearchFieldsLB");
                    while (k < valueFromHiddennew.Length)
                    {
                        int n = 0;
                        if (valueFromVisible.Any(t => t.Equals(valueFromHiddennew[k], StringComparison.CurrentCultureIgnoreCase)))
                        {
                            valueSelected = true;
                        }

                        if (valueSelected != true)
                        {
                            for (int j = 0; j < valueFromHidden.Length; j++)
                            {
                                if (valueFromHidden[j].Equals(valueFromHiddennew[k], StringComparison.CurrentCultureIgnoreCase))
                                {
                                    n = j;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Logger.Instance.InfoLog(@"Option :" + valueFromHiddennew + @" already selected");
                        }

                        if (n < valueFromHidden.Length)
                        {
                            SelectFromList("id", "ctl00_MasterContentPlaceHolder_m_sslConfigControl_ssclHiddenSearchFieldsLB", valueFromHidden[n]);
                        }
                        else
                        {
                            Logger.Instance.InfoLog(@"Option :" + valueFromHidden[n] + "not found in the select list");
                        }

                        k = k + 1;
                    }
                    ClearText("id", "ctl00_MasterContentPlaceHolder_m_sslConfigControl_ssclAddButton");
                    ClearText("id", "ctl00_MasterContentPlaceHolder_m_sslConfigControl_ssclAddButton");
                    string[] valuefromConnectednew = GetValuesfromDropDown("id", "ctl00_MasterContentPlaceHolder_m_sslConfigControl_ssclVisibleSearchFieldsLB");
                    int m = valueFromVisible.Count() + valueFromHiddennew.Count();
                    int l = valuefromConnectednew.Count();
                    if (m == l)
                    {
                        valueSelected = true;
                    }
                    else
                    {
                        valueSelected = false;
                    }
                    i++;
                }
                Thread.Sleep(2000);

                Logger.Instance.InfoLog("Serach Field Addition successful");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog(ex.Message);
            }
        }

        /// <summary>
        /// Add all tools to Toolbar
        /// </summary>
        public void AddAllToolsToToolBar()
        {
            try
            {
                var action1 = new Actions(Driver);
                ReadOnlyCollection<IWebElement> totalColumn = Driver.FindElements(By.XPath("//div[@id='toolbarItemsConfig']/div"));
                int i = totalColumn.Count;

                ReadOnlyCollection<IWebElement> elements = Driver.FindElements(By.XPath("//ul[@id='available']/li"));
                try
                {
                    foreach (IWebElement item in elements)
                    {
                        IWebElement targetElement = GetElement("id", Equals(item, elements[0]) ? "newList" : i.ToString(CultureInfo.InvariantCulture));
                        action1.DragAndDrop(item, targetElement).Build().Perform();
                    }
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Exception while searching for last header for adding tools. Error: " +
                                             e.Message);
                }

                Logger.Instance.InfoLog("AddAllToolsToToolBar successful");
            }
            catch (Exception er)
            {
                Logger.Instance.ErrorLog("Exception in method AddAllToolsToToolBar due to  " + er);
            }
        }

        /// <summary>
        /// This is to create new domain
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="roleName"></param>
        public void CreateDomain(string domainName, string roleName, String[] datasources = null)
        {

            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            bool DomainFlag = DomainExists(domainName);
            if (!DomainFlag)
            {
                ClickNewDomainBtn();
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                PageLoadWait.WaitForFrameLoad(10);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Name", domainName);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Description", domainName);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_ReceivingInstitution", domainName + "Inst");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_UserID", domainName.Replace(" ", "_"));
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_LastName", domainName.Replace(" ", "_") + "LastName");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Password", domainName);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_FirstName", domainName.Replace(" ", "_") + "FirstName");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword", domainName);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Name", roleName);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Description", roleName);

                //Set Viewer Type
                SetViewerTypeInNewDomain();

                if (datasources == null)
                    ConnectAllDataSources();
                else
                {
                    foreach (string ds in datasources)
                        ConnectDataSource(ds);
                }
                MakeAllFieldsVisibleStudySearchFieldsDomainMgmt();
                AddAllToolsToToolBar();
            }
            else
            {
                Logger.Instance.ErrorLog("Domain Name already exists. hence not creating new Domain");
            }
        }

        /// <summary>
        /// This method will create a domain with input array of Strings
        /// </summary>
        /// <param name="domainattr"></param>
        public void CreateDomain(Dictionary<Object, String> domainattr, String[] datasources = null, Boolean isconferenceneeded = false, Boolean isgrantaccessneeded = false, Boolean isimagesharingneeded = false, Boolean isemailstudy = false, Boolean AddAllStudySearchFields = false)
        {

            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            bool DomainFlag = DomainExists(domainattr[DomainAttr.DomainName]);
            WebDriverWait wait2;
            wait2 = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 5));

            if (!DomainFlag)
            {
                ClickNewDomainBtn();
                PageLoadWait.WaitForHPPageLoad(10);
                BasePage.Driver.SwitchTo().DefaultContent();
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("iframe[id='UserHomeFrame']")));
                BasePage.Driver.SwitchTo().Frame(Driver.FindElement(By.CssSelector("iframe[id='UserHomeFrame']")));
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Name", domainattr[DomainAttr.DomainName]);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Description", domainattr[DomainAttr.DomainDescription]);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_ReceivingInstitution", domainattr[DomainAttr.InstitutionName]);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_UserID", domainattr[DomainAttr.UserID]);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_LastName", domainattr[DomainAttr.LastName]);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_FirstName", domainattr[DomainAttr.FirstName]);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Email", domainattr[DomainAttr.EmailAddress]);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Password", domainattr[DomainAttr.Password]);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword", domainattr[DomainAttr.Password]);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Name", domainattr[DomainAttr.RoleName]);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Description", domainattr[DomainAttr.RoleDescription]);
                if (isconferenceneeded) { this.SetCheckbox("cssselector", "input[id$='EnableConferenceListsCB']"); }
                if (isgrantaccessneeded) { this.SetCheckbox("cssselector", "[id$='_GrantAccessEnabledCB']"); }
                if (isimagesharingneeded) { this.SetCheckbox("cssselector", "[id$='_EnableImageSharingCB']"); }
                if (isemailstudy) { this.SetCheckbox("cssselector", "input[id$='EnableEmailStudyCB']"); }

                //Set Viewer Type
                SetViewerTypeInNewDomain();

                //Add all Data Sources
                if (datasources == null)
                {
                    SelectElement select = new SelectElement(BasePage.Driver.FindElement(By.CssSelector("[id*='DataSourceDisconnectedListBox']")));
                    foreach (IWebElement item in select.Options)
                    {
                        this.ConnectDataSourcesConsolidatedInNewDomain(item.Text);
                    }

                    try
                    {
                        wait2.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogDiv")));
                        this.ClickButton("#ctl00_ConfirmButton");
                    }
                    catch (Exception) { }

                    if (AddAllStudySearchFields)
                    {
                        ModifyStudySearchFields();
                    }
                    PageLoadWait.WaitForFrameLoad(10);

                    //Save Transaction
                    this.ClickButton("#ctl00_MasterContentPlaceHolder_SaveButton");
                    try
                    {

                        wait2.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogDiv")));
                        this.ClickButton("#ctl00_CloseAlertButton");
                    }
                    catch (Exception) { }
                    PageLoadWait.WaitForPageLoad(10);
                }

                //Select Data Sources Required
                else
                {
                    foreach (String datasource in datasources)
                    {
                        PageLoadWait.WaitForFrameLoad(10);
                        this.ConnectDataSourcesConsolidatedInNewDomain(datasource);
                    }

                    if (AddAllStudySearchFields)
                    {
                        ModifyStudySearchFields();
                    }
                    PageLoadWait.WaitForFrameLoad(10);

                    //Save Transaction      
                    PageLoadWait.WaitForFrameLoad(10);
                    this.ClickButton("#ctl00_MasterContentPlaceHolder_SaveButton");
                    try
                    {
                        wait2.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogDiv")));
                        this.ClickButton("#ctl00_CloseAlertButton");
                    }
                    catch (Exception) { }
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                }

            }
            else
            {
                Logger.Instance.ErrorLog("Domain Name already exists. hence not creating new Domain");
                throw new Exception("Domain Name Already Present");
            }
            PageLoadWait.WaitForLoadingMessage(10);
        }

        /// <summary>
        /// This method will create the domain attributes required to create domain at run time
        /// </summary>
        /// <returns></returns>
        public Dictionary<Object, String> CreateDomainAttr()
        {
            Dictionary<Object, String> domainattr = new Dictionary<Object, string>();
            int limit = Math.Abs((int)DateTime.Now.Ticks);
            limit = Int32.Parse(limit.ToString().Substring(0, 4));
            Random random = new Random();

            domainattr.Add(DomainAttr.DomainName, BasePage.GetUniqueDomainID());
            domainattr.Add(DomainAttr.DomainDescription, "Desc" + random.Next(1, limit));
            domainattr.Add(DomainAttr.InstitutionName, "Inst" + new DateTime().Second + random.Next(1, limit));
            domainattr.Add(DomainAttr.UserID, BasePage.GetUniqueUserId("Admin"));
            domainattr.Add(DomainAttr.LastName, "Last" + new DateTime().Second + random.Next(1, limit));
            domainattr.Add(DomainAttr.FirstName, "First" + new DateTime().Second + random.Next(1, limit));
            domainattr.Add(DomainAttr.EmailAddress, Config.emailid);
            domainattr.Add(DomainAttr.Password, "Pass" + new DateTime().Second + random.Next(1, limit));
            domainattr.Add(DomainAttr.RoleName, BasePage.GetUniqueRole("DomainAdmin"));
            domainattr.Add(DomainAttr.RoleDescription, "DomainAdmin" + new DateTime().Second + random.Next(1, limit));
            foreach (KeyValuePair<Object, String> keyvalue in domainattr)
            {
                Logger.Instance.InfoLog("The Domain Attributes are--" + keyvalue.Key + "---" + keyvalue.Value);
            }

            return domainattr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datasourceName"></param>
        public void ConnectDataSourcesConsolidated(string datasourceName)
        {
            bool valueSelected = false;
            string[] disconnectedAllOptions = GetValuesfromDropDown("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_DataSourceDisconnectedListBox");

            int index = 0;

            while (index < disconnectedAllOptions.Length)
            {
                if (disconnectedAllOptions[index] == datasourceName)
                {
                    if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Equals("internet explorer") &&
                            ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("10"))
                    {
                        IList<IWebElement> datasourceOptions = BasePage.Driver.FindElements(By.CssSelector("select[id$='DataSourceDisconnectedListBox'] option"));
                        foreach (IWebElement option in datasourceOptions)
                        {
                            if (option.Text.ToUpper().Equals(datasourceName))
                            {
                                if (SBrowserName.ToLower().Equals("internet explorer") && BrowserVersion.ToLower().Equals("8"))
                                {
                                    this.CtrlKeyDown();
                                    option.Click();
                                    this.CtrlKeyUp();
                                }
                                else { CtrClick(option); }

                            }
                        }
                    }
                    else
                    {
                        SelectFromList("id", "ctl00_MasterContentPlaceHolder_DataInfo_DataSourceDisconnectedListBox", datasourceName);
                    }
                    Click("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_ConnectDataSource");
                    Click("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_ConnectDataSource");
                }

                string[] connectedAllOptions = GetValuesfromDropDown("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_DataSourceConnectedListBox");
                while (index < connectedAllOptions.Length)
                {
                    if (connectedAllOptions[index] == datasourceName)
                    {
                        valueSelected = true;
                        break;
                    }
                }

                if (valueSelected)
                {
                    break;
                }
            }
        }

        /// <summary>
        ///     This function makes the specified data sources connected to a domain
        /// </summary>
        public void ConnectDataSources()
        {
            try
            {
                string[] valueFromDisconnected = GetValuesfromDropDown("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_DataSourceDisconnectedListBox");
                string[] valuefromConnected = GetValuesfromDropDown("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_DataSourceConnectedListBox");

                bool valueSelected = false;

                int i = 0;

                while (valueSelected != true && i < 10)
                {
                    int k = 0;
                    string[] valueFromDisconnectednew = GetValuesfromDropDown("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_DataSourceDisconnectedListBox");

                    while (k < valueFromDisconnectednew.Length)
                    {
                        int n = 0;
                        if (
                            valuefromConnected.Any(
                                t => t.Equals(valueFromDisconnectednew[k], StringComparison.CurrentCultureIgnoreCase)))
                        {
                            valueSelected = true;
                        }

                        if (valueSelected != true)
                        {
                            for (int j = 0; j < valueFromDisconnected.Length; j++)
                            {
                                if (valueFromDisconnected[j].Equals(valueFromDisconnectednew[k],
                                                                    StringComparison.CurrentCultureIgnoreCase))
                                {
                                    n = j;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Logger.Instance.InfoLog(@"Option :" + valueFromDisconnectednew + @" already selected");
                        }

                        if (n < valueFromDisconnected.Length)
                        {
                            SelectFromMultipleList("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_DataSourceDisconnectedListBox",
                                                            valueFromDisconnected[n]);
                        }
                        else
                        {
                            Logger.Instance.InfoLog(@"Option :" + valueFromDisconnected[n] +
                                                    "not found in the select list");
                        }

                        k = k + 1;
                    }
                    Click("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_ConnectDataSource");
                    Click("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_ConnectDataSource");

                    string[] valuefromConnectednew = GetValuesfromDropDown("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_DataSourceConnectedListBox");

                    int m = valuefromConnected.Count() + valueFromDisconnectednew.Count();
                    int l = valuefromConnectednew.Count();
                    if (m == l)
                    {
                        valueSelected = true;
                    }
                    else
                    {
                        valueSelected = false;
                    }
                    i++;
                }
                Thread.Sleep(2000);
                Logger.Instance.InfoLog("ConnectDataSources successful");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in method ConnectDataSources due to  " + ex);
            }
        }

        /// <summary>
        /// Connect All data sources while Editing domain
        /// </summary>
        public bool ConnectAllDatasourcesEditDomain()
        {
            bool valueSelected = false;
            PageLoadWait.WaitForElement(By.Id("ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_DataSourceDisconnectedListBox"), BasePage.WaitTypes.Visible);
            IWebElement DisconnectedListBox = Driver.FindElement(By.Id("ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_DataSourceDisconnectedListBox"));
            PageLoadWait.WaitForElement(By.Id("ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_DataSourceConnectedListBox"), BasePage.WaitTypes.Visible);
            IWebElement ConnectedListBox = Driver.FindElement(By.Id("ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_DataSourceConnectedListBox"));
            //Get Count of Connected List box to compare if they got added
            SelectElement ConnectedSelect = new SelectElement(ConnectedListBox);
            int count = ConnectedSelect.Options.Count;
            SelectElement DisconnectedSelect = new SelectElement(DisconnectedListBox);
            if (DisconnectedSelect.Options.Count >= 1)
            {
                for (int i = 0; i < DisconnectedSelect.Options.Count; i++)
                {
                    DisconnectedSelect.SelectByIndex(i);
                }
                PageLoadWait.WaitForElement(By.Id("ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_ConnectDataSource"), BasePage.WaitTypes.Visible);
                Driver.FindElement(By.Id("ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_ConnectDataSource")).Click();
                PageLoadWait.WaitForPageLoad(10);
                IWebElement popup = GetElement("id", "ctl00_ConfirmButton");
                if (popup.Displayed)
                {
                    popup.Click();
                }
            }
            ConnectedSelect = new SelectElement(ConnectedListBox);
            DisconnectedSelect = new SelectElement(DisconnectedListBox);
            if (ConnectedSelect.Options.Count > DisconnectedSelect.Options.Count)
                valueSelected = true;
            return valueSelected;
        }

        /// <summary>
        ///     This function makes the specified data sources connected to a New domain
        /// </summary>
        public void ConnectDataSourcesInNewDomain()
        {
            try
            {
                string[] valueFromDisconnected = GetValuesfromDropDown("id", "ctl00_MasterContentPlaceHolder_DataInfo_DataSourceDisconnectedListBox");
                string[] valuefromConnected = GetValuesfromDropDown("id", "ctl00_MasterContentPlaceHolder_DataInfo_DataSourceConnectedListBox");

                bool valueSelected = false;
                int i = 0;

                while (valueSelected != true && i < 10)
                {
                    int k = 0;
                    string[] valueFromDisconnectednew = GetValuesfromDropDown("id", "ctl00_MasterContentPlaceHolder_DataInfo_DataSourceDisconnectedListBox");
                    while (k < valueFromDisconnectednew.Length)
                    {
                        int n = 0;
                        if (
                            valuefromConnected.Any(
                                t => t.Equals(valueFromDisconnectednew[k], StringComparison.CurrentCultureIgnoreCase)))
                        {
                            valueSelected = true;
                        }

                        if (valueSelected != true)
                        {
                            for (int j = 0; j < valueFromDisconnected.Length; j++)
                            {
                                if (valueFromDisconnected[j].Equals(valueFromDisconnectednew[k],
                                                                    StringComparison.CurrentCultureIgnoreCase))
                                {
                                    n = j;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Logger.Instance.InfoLog(@"Option :" + valueFromDisconnectednew + @" already selected");
                        }

                        if (n < valueFromDisconnected.Length)
                        {
                            SelectFromList("id", "ctl00_MasterContentPlaceHolder_DataInfo_DataSourceDisconnectedListBox", valueFromDisconnected[n]);
                        }
                        else
                        {
                            Logger.Instance.InfoLog(@"Option :" + valueFromDisconnected[n] +
                                                    "not found in the select list");
                        }
                        k = k + 1;
                    }
                    Click("id", "ctl00_MasterContentPlaceHolder_DataInfo_DataSourceConnectedListBox");
                    Click("id", "ctl00_MasterContentPlaceHolder_DataInfo_ConnectDataSource");

                    IWebElement popup = GetElement("id", "ctl00_ConfirmButton");
                    if (popup.Displayed)
                    {
                        popup.Click();
                        Thread.Sleep(2000);
                    }

                    string[] valuefromConnectednew = GetValuesfromDropDown("id", "ctl00_MasterContentPlaceHolder_DataInfo_DataSourceConnectedListBox");

                    int l = valuefromConnected.Count() + valueFromDisconnectednew.Count();
                    int m = valuefromConnectednew.Count();
                    if (l == m)
                    {
                        valueSelected = true;
                    }
                    else
                    {
                        valueSelected = false;
                    }
                    i++;
                }

                Thread.Sleep(2000);

                //ClickSaveDomain();
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in method ConnectDataSourcesInNewDomain due to  " + ex);
            }
        }

        /// <summary>
        /// This method is to move elements to the Available item section
        /// </summary>
        /// <param name="tools"></param>
        public new void MoveToolsToAvailableSection(IWebElement[] tools, bool isSave = true)
        {
            base.MoveToolsToAvailableSection(tools, isSave);
        }

        /// <summary>
        /// This method is to check if tool is available in Available item section
        /// </summary>
        /// <param name="tools"></param>
        /// <returns></returns>
        public new Boolean CheckToolsInAvailbleSection(String[] tooltitles)
        {
            return base.CheckToolsInAvailbleSection(tooltitles);
        }

        /// <summary>
        /// Enum for Domain Attributes
        /// </summary>
        public enum DomainAttr { DomainName, DomainDescription, InstitutionName, UserID, LastName, FirstName, EmailAddress, Password, RoleName, RoleDescription };

        /// <summary>
        /// This method is to close the domain management screen
        /// </summary>
        public void CloseDomainManagement()
        {
            PageLoadWait.WaitForFrameLoad(5);
            BasePage.Driver.FindElement(By.CssSelector("input[name$='CloseButton'][name*='MasterContent']")).Click();
            PageLoadWait.WaitForHPPageLoad(10);
        }

        /// <summary>
        /// To set the checkboxes for a domain
        /// </summary>
        /// <param name="field"></param>
        /// <param name="set"></param>
        public void SetCheckBoxInEditDomain(String field, int set)
        {
            PageLoadWait.WaitForFrameLoad(40);
            try { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id$='Domain_Content']"))); }
            catch (NoSuchElementException e) { }
            String FieldName = field.ToLower();
            string Checkbox = "";
            switch (FieldName)
            {
                case "modality":
                    Checkbox = "[id$='_ToolbarConfiguration1_UseDefaultToolbarCheckbox']";
                    break;
                case "allow":
                    Checkbox = "[id$='_AllowSuppressLoginMessageCB']";
                    break;
                case "login":
                    Checkbox = "[id$='_UseSystemSettingsForLoginMessageCB']";
                    break;
                case "emergency":
                    Checkbox = "[id$='_EmergencyAccessEnabledCB']";
                    break;
                case "grant":
                    Checkbox = "[id$='_GrantAccessEnabledCB']";
                    break;
                case "emailstudy":
                    Checkbox = "[id$='_EnableEmailStudyCB']";
                    break;
                case "datatransfer":
                    Checkbox = "[id$='_DataTransferEnabledCB']";
                    break;
                case "savegsps":
                    Checkbox = "[id$='_DomainInfo_SavingGSPSCB']";
                    break;
                case "datadownload":
                    Checkbox = "[id$='_DataDownloadEnabledCB']";
                    break;
                case "reportview":
                    Checkbox = "[id$='ReportViewingEnabledCB']";
                    break;
                case "requisitionreport":
                    Checkbox = "[id$='_RequisitionEnabledCB']";
                    break;
                case "attachment":
                    Checkbox = "[id$='_AttachmentEnabledCB']";
                    break;
                case "attachmentupload":
                    Checkbox = "[id$='_AttachmentAllowUploadEnabledCB']";
                    break;
                case "print":
                    Checkbox = "[id$='_PrintEnabledCB']";
                    break;

                case "disable":
                    Checkbox = "[id$='_ToolbarDisableCheckBox']";
                    break;
                // to set the PDF Report Checkbox in Domain
                case "pdfreport":
                    Checkbox = "[id$='_EnablePdfReportCB']";
                    break;

                case "patientid":
                    Checkbox = "[id$='DomainInfo_QueryRelatedStudyParametersCBList_0']";
                    break;

                case "patientfullname":
                    Checkbox = "[id$='DomainInfo_QueryRelatedStudyParametersCBList_1']";
                    break;

                case "patientlastname":
                    Checkbox = "[id$='DomainInfo_QueryRelatedStudyParametersCBList_2']";
                    break;

                case "patientdob":
                    Checkbox = "[id$='DomainInfo_QueryRelatedStudyParametersCBList_3']";
                    break;

                case "patientipid":
                    Checkbox = "[id$='DomainInfo_QueryRelatedStudyParametersCBList_4']";
                    break;
                case "defaultjavaei":
                    Checkbox = "[id$='DefaultToJavaEICB']";
                    break;
                case "breifcase":
                    Checkbox = "[id$='BriefcaseDisplayCB']";
                    break;

                case "imagesharing":
                    Checkbox = "[id$='_EnableImageSharingCB']";
                    break;

                case "conferencelists":
                    Checkbox = "[id$='_EnableConferenceListsCB']";
                    break;

                case "autoinherit":
                    Checkbox = "[id$='_AutoInheritGroupRolesCB']";
                    break;

                case "domainimagesharing":
                    Checkbox = "[id$='_AllowDomainAdminControlImageSharingCB']";
                    break;

                case "invite":
                    Checkbox = "[id$='_InviteUnregisteredUserCB']";
                    break;

                case "patientnamesearch":
                    Checkbox = "[id$='_PatientNameSearchCB']";
                    break;

                case "universalviewer":                    
                    Checkbox = "[id$='_EnableUniversalViewerCB'][id*='Domain']";
                    break;

                case "3dview":
                    Checkbox = checkbox_enable3DView;
                    break;


                //Under DomainAdmin Role
                case "allowdownload":
                    Checkbox = "[id$='_AllowDownloadCB']";
                    break;
                case "allowtransfer":
                    Checkbox = "[id$='_AllowTransferCB']";
                    break;
                case "allowemail":
                    Checkbox = "[id$='_AllowEmailCB']";
                    break;
                case "PDFreport":
                    Checkbox = "[id$='_RoleAccessFilter_EnablePdfReportCB']";
                    break;
                case "receiveexam":
                    Checkbox = "[id$='_EnableReceiveExamsCB']";
                    break;
                case "archive":
                    Checkbox = "[id$='_EnableArchiveToPacsCB']";
                    break;            


            }

            //To Check/Uncheck the given check box
            if (set == 0)
            {
                if (!Driver.FindElement(By.CssSelector(Checkbox)).Selected)
                    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", Driver.FindElement(By.CssSelector(Checkbox)));
                Logger.Instance.InfoLog(Checkbox + " is set");
            }
            else
            {
                if (Driver.FindElement(By.CssSelector(Checkbox)).Selected)
                {
                    ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", Driver.FindElement(By.CssSelector(Checkbox)));
                }
            }

        }

        /// <summary>
        /// To set the checkboxes for a domain and verify if its selected
        /// </summary>
        /// <param name="field"></param>
        /// <param name="set"></param>
        public bool VerifyCheckBoxInEditDomain(String field)
        {
            PageLoadWait.WaitForFrameLoad(40);
            try { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id$='Domain_Content']"))); }
            catch (NoSuchElementException e) { }
            String FieldName = field.ToLower();
            string Checkbox = "";
            switch (FieldName)
            {
                case "modality":
                    Checkbox = "[id$='_ToolbarConfiguration1_UseDefaultToolbarCheckbox']";
                    break;
                case "allow":
                    Checkbox = "[id$='_AllowSuppressLoginMessageCB']";
                    break;
                case "login":
                    Checkbox = "[id$='_UseSystemSettingsForLoginMessageCB']";
                    break;
                case "emergency":
                    Checkbox = "[id$='_EmergencyAccessEnabledCB']";
                    break;
                case "grant":
                    Checkbox = "[id$='_GrantAccessEnabledCB']";
                    break;
                case "emailstudy":
                    Checkbox = "[id$='_EnableEmailStudyCB']";
                    break;
                case "datatransfer":
                    Checkbox = "[id$='_DataTransferEnabledCB']";
                    break;
                case "savegsps":
                    Checkbox = "[id$='_DomainInfo_SavingGSPSCB']";
                    break;
                case "datadownload":
                    Checkbox = "[id$='_DataDownloadEnabledCB']";
                    break;
                case "reportview":
                    Checkbox = "[id$='ReportViewingEnabledCB']";
                    break;
                case "requisitionreport":
                    Checkbox = "[id$='_RequisitionEnabledCB']";
                    break;
                case "attachment":
                    Checkbox = "[id$='_AttachmentEnabledCB']";
                    break;
                case "attachmentupload":
                    Checkbox = "[id$='_AttachmentAllowUploadEnabledCB']";
                    break;
                case "print":
                    Checkbox = "[id$='_PrintEnabledCB']";
                    break;

                case "disable":
                    Checkbox = "[id$='_ToolbarDisableCheckBox']";
                    break;
                // to set the PDF Report Checkbox in Domain
                case "pdfreport":
                    Checkbox = "[id$='_EnablePdfReportCB']";
                    break;

                case "patientid":
                    Checkbox = "[id$='DomainInfo_QueryRelatedStudyParametersCBList_0']";
                    break;

                case "patientfullname":
                    Checkbox = "[id$='DomainInfo_QueryRelatedStudyParametersCBList_1']";
                    break;

                case "patientlastname":
                    Checkbox = "[id$='DomainInfo_QueryRelatedStudyParametersCBList_2']";
                    break;

                case "patientnamesearch":
                    Checkbox = "[id$='_PatientNameSearchCB']";
                    break;

                case "patientdob":
                    Checkbox = "[id$='DomainInfo_QueryRelatedStudyParametersCBList_3']";
                    break;

                case "patientipid":
                    Checkbox = "[id$='DomainInfo_QueryRelatedStudyParametersCBList_4']";
                    break;
                case "defaultjavaei":
                    Checkbox = "[id$='DefaultToJavaEICB']";
                    break;
                case "breifcase":
                    Checkbox = "[id$='BriefcaseDisplayCB']";
                    break;

                case "imagesharing":
                    Checkbox = "[id$='_EnableImageSharingCB']";
                    break;

                case "conferencelists":
                    Checkbox = "[id$='_EnableConferenceListsCB']";
                    break;

                case "grantaccess":
                    Checkbox = "[id$='_GrantAccessEnabledCB']";
                    break;

                case "autoinherit":
                    Checkbox = "[id$='_AutoInheritGroupRolesCB']";
                    break;

                case "domainimagesharing":
                    Checkbox = "[id$='_AllowDomainAdminControlImageSharingCB']";
                    break;

                //Under DomainAdmin Role
                case "allowdownload":
                    Checkbox = "[id$='_AllowDownloadCB']";
                    break;
                case "allowtransfer":
                    Checkbox = "[id$='_AllowTransferCB']";
                    break;
                case "allowemail":
                    Checkbox = "[id$='_AllowEmailCB']";
                    break;
                case "PDFreport":
                    Checkbox = "[id$='_RoleAccessFilter_EnablePdfReportCB']";
                    break;
                case "receiveexam":
                    Checkbox = "[id$='_EnableReceiveExamsCB']";
                    break;
                case "archive":
                    Checkbox = "[id$='_EnableArchiveToPacsCB']";
                    break;



            }

            //To Check/Uncheck the given check box
            bool flag = false;

            if (!Driver.FindElement(By.CssSelector(Checkbox)).Selected)
            {

                flag = false;
            }
            else
            {

                flag = true;
            }
            return flag;

            //return flag;
        }

        /// <summary>
        /// This is to create new domain
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="roleName"></param>
        public void CreateDomain(string domainName, string roleName, string ds, string modality = null, bool domainFlag = false)
        {

            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            bool DomainFlag = false;
            if (domainFlag == false)
            {
                DomainFlag = DomainExists(domainName);
            }
            else
            {
                DomainFlag = false;
            } 

            if (!DomainFlag)
            {
                ClickNewDomainBtn();

                //ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Name");
                //SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Name", domainName);
                DomainNameTxtBox().SendKeys(domainName);

                ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Description");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Description", domainName);

                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_ReceivingInstitution",
                                         domainName + "Inst");


                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_UserID",
                                         domainName);


                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_LastName",
                                         domainName.Replace(" ", "_") + "LastName");

                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_FirstName",
                                         domainName.Replace(" ", "_") + "FirstName");

                //SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Password", domainName);
                DomainPwdTxtBox().SendKeys(domainName);

                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword",
                                         domainName);

                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Name", roleName);

                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Description",
                                         roleName);
                PageLoadWait.WaitForPageLoad(20);

                //Set Viewer Type
                SetViewerTypeInNewDomain();

                ConnectDataSourcesConsolidatedInNewDomain(ds);
                PageLoadWait.WaitForPageLoad(30);
                if (modality != null)
                {
                    RoleAccessFilterForModality(modality);
                }

            }
            else
            {
                Logger.Instance.ErrorLog("Domain Name already exists. hence not creating new Domain");
            }


        }

        /// <summary>
        /// This function will connect a Datasource in new domain
        /// </summary>
        /// <param name="datasourceName"></param>
        public void ConnectDataSourcesConsolidatedInNewDomain(string datasourceName)
        {
            bool valueSelected = false;
            PageLoadWait.WaitForPageLoad(30);
            string[] disconnectedAllOptions = GetValuesfromDropDown("id", "ctl00_MasterContentPlaceHolder_DataInfo_DataSourceDisconnectedListBox");
            PageLoadWait.WaitForPageLoad(30);
            int index = 0;

            while (index < disconnectedAllOptions.Length)
            {
                if (disconnectedAllOptions[index] == datasourceName)
                {
                    if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Equals("internet explorer") &&
                            ((RemoteWebDriver)BasePage.Driver).Capabilities.Version.Equals("10"))
                    {
                        ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"select[id$='DataSourceDisconnectedListBox']>option[value=" + datasourceName + "]\").selected=true");
                    }
                    else
                    {
                        SelectFromList("id", "ctl00_MasterContentPlaceHolder_DataInfo_DataSourceDisconnectedListBox", datasourceName);
                    }

                    Click("id", "ctl00_MasterContentPlaceHolder_DataInfo_ConnectDataSource");
                    Click("id", "ctl00_MasterContentPlaceHolder_DataInfo_ConnectDataSource");
                }

                string[] connectedAllOptions = GetValuesfromDropDown("id", "ctl00_MasterContentPlaceHolder_DataInfo_DataSourceConnectedListBox");
                int connectedIndex = 0;
                while (connectedIndex < connectedAllOptions.Length)
                {
                    if (connectedAllOptions[connectedIndex++] == datasourceName)
                    {
                        valueSelected = true;
                        break;
                    }
                }

                if (valueSelected)
                {
                    break;
                }
                index++;
            }
            PageLoadWait.WaitForPageLoad(20);


        }

        /// <summary>
        /// This method closes the Edit domain dialog
        /// </summary>
        public void ClickCloseEditDomain()
        {
            IWebElement close = null;
            String cssselector = "input#CloseButton";
            try
            {
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(cssselector)));
            }
            catch (Exception)
            {
                cssselector = "div[id$='_RegisterButtonsDiv'] [id$='_CloseButton']";
            }

            PageLoadWait.WaitForFrameLoad(10);
            BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(cssselector)));
            if ((Driver.FindElement(By.CssSelector(cssselector)).Displayed))
            {
                close = Driver.FindElement(By.CssSelector(cssselector));
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer"))
                    ClickElement(close);
                else
                    close.Click();

            }
            PageLoadWait.WaitForFrameLoad(20);
            Driver.SwitchTo().DefaultContent();
        }

        /// <summary>
        /// This is to reset the LoginMessage
        /// </summary>
        public void ResetLoginMessage()
        {
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#EditDomain_Content>div")));
            PageLoadWait.WaitForFrameLoad(20);
            //Driver.FindElement(By.CssSelector("[id$='_ResetWarningCheck']")).Click();
            ClickButton("[id$='_ResetWarningCheck']");
            Logger.Instance.InfoLog("Reset button is clicked");

        }

        /// <summary>
        /// This is to modify the URL in Login Message Address Box inDomainManagement
        /// </summary>
        /// <param name="URL"></param>
        public void ModifyLoginURLinDomain(String URL)
        {
            PageLoadWait.WaitForFrameLoad(20);
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            IWebElement urlbox = Driver.FindElement(By.CssSelector("input[id$='LoginMessageAddressTB']"));
            urlbox.Clear();
            urlbox.SendKeys(URL);

        }

        /// <summary>
        /// This is to check if Java Exam Importer is selected in DomainManagement
        /// </summary>
        public bool CheckJavaExamImporter()
        {
            PageLoadWait.WaitForFrameLoad(20);
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            IWebElement javachkbox = Driver.FindElement(By.Id(Locators.ID.DomainMgmtJavaCheckbox));
            return javachkbox.Selected;
        }

        /// <summary>
        /// This function clicks the save button in New domain 
        /// </summary>
        public void ClickSaveNewDomain()
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            if (Driver.FindElement(By.CssSelector("[id$='_SaveButton']")).Enabled == true)
            {
                //Save Domain                
                var js = (IJavaScriptExecutor)Driver;
                if (SBrowserName.ToLower().Equals("internet explorer") || SBrowserName.ToLower().Contains("edge"))
                {
                    Thread.Sleep(3000);
                    js.ExecuteScript("arguments[0].click()", Driver.FindElement(By.CssSelector("[id$='_SaveButton']")));
                    Thread.Sleep(5000);
                }
                else
                {
                    Driver.FindElement(By.CssSelector("[id$='_SaveButton']")).Click();
                }

                //Syncup points
                PageLoadWait.WaitForFrameLoad(30);
                //Enable Image sharing option enabled then have to click Close btn
                try
                {
                    if (BasePage.Driver.FindElement(By.CssSelector("input[id='ctl00_CloseAlertButton']")).Displayed)
                    {
                        this.ClickElement(BasePage.Driver.FindElement(By.CssSelector("input[id='ctl00_CloseAlertButton']")));
                    }
                }
                catch (Exception) { }

                Logger.Instance.InfoLog("Save button in New Domain is clicked");
                //Driver.SwitchTo().DefaultContent();
                //Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(10);
            }
            else
            {
                Logger.Instance.ErrorLog("Save Button is not found in New Domain");
            }
        }

        /// <summary>
        /// This is to create new domain
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="roleName"></param>
        public void CreateDomain(string domainName, string roleName, string ds, string username, string password, bool domainFlag = false)
        {

            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            bool DomainFlag = false;
            if (domainFlag == false)
            {
                DomainFlag = DomainExists(domainName);
            }
            else
            {
                DomainFlag = false;
            }   

            if (!DomainFlag)
            {
                ClickNewDomainBtn();

                ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Name");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Name", domainName);

                ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Description");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Description", domainName);

                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_ReceivingInstitution",
                                         domainName + "Inst");


                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_UserID",
                                         username);


                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_LastName",
                                         domainName.Replace(" ", "_") + "LastName");

                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_FirstName",
                                         domainName.Replace(" ", "_") + "FirstName");

                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Password", password);

                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword",
                                         password);


                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Name", roleName);

                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Description",
                                         roleName);
                PageLoadWait.WaitForPageLoad(20);

                //Set Viewer Type
                SetViewerTypeInNewDomain();

                ConnectDataSourcesConsolidatedInNewDomain(ds);

                PageLoadWait.WaitForPageLoad(30);


            }
            else
            {
                Logger.Instance.ErrorLog("Domain Name already exists. hence not creating new Domain");
            }


        }

        /// <summary>
        ///     This function clicks on the Save Domain button
        /// </summary>
        public void ClickSaveDomain()
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            if (Driver.FindElement(By.CssSelector("[id$='_SaveButton']")).Enabled == true)
            {
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("document.querySelector(\"[id$='_SaveButton']\").click()");
                try
                {
                    WebDriverWait wait2 = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 10));
                    wait2.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogDiv")));
                    this.ClickButton("#ctl00_CloseAlertButton");
                }
                catch (Exception) { }
                PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                Logger.Instance.InfoLog("Save button in New Domain is clicked");
				WebDriverWait wait3 = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 120));
				wait3.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[id$='_SaveButton']")));
			}
            else
            {
                Logger.Instance.ErrorLog("Save Button is not found in New Domain");
            }
        }

        /// <summary>
        /// This is to create new domain
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="roleName"></param>
        public void CreateDomain(string domainName, string domaindescription, string Institution, string userID, string datasource,
            string lastname, string firstname, string adminPassword, string roleName, string roledescription, int alldatasource = 0, string[] datasources = null, string checkbox = null)
        {
            ClickNewDomainBtn();

            ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Name");
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Name", domainName);
            Logger.Instance.InfoLog("Domain Name --" + domainName + "is entered.");

            ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Description");
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Description", domaindescription);
            Logger.Instance.InfoLog("Domain Description --" + domaindescription + "is entered.");

            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_ReceivingInstitution", Institution);
            Logger.Instance.InfoLog("Receiving Institution --" + Institution + "is entered.");

            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_UserID", userID);
            Logger.Instance.InfoLog("User ID --" + userID + "is entered.");

            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_LastName", lastname);
            Logger.Instance.InfoLog("Last name --" + lastname + "is entered.");

            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_FirstName", firstname);
            Logger.Instance.InfoLog("First name --" + firstname + "is entered.");

            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Password", adminPassword);
            Logger.Instance.InfoLog("Password --" + adminPassword + "is entered.");

            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword", adminPassword);
            Logger.Instance.InfoLog("Confirm Password --" + adminPassword + "is entered.");

            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Name", roleName);
            Logger.Instance.InfoLog("Role Name --" + roleName + "is entered.");

            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Description", roledescription);
            Logger.Instance.InfoLog("Role Description --" + roledescription + "is entered.");

            if (checkbox != null)
            {
                string[] Checkbox = checkbox.Split(',');
                foreach (string check in Checkbox)
                {
                    string[] box = check.Split('=');
                    SetCheckBoxInEditDomain(box[0], Convert.ToInt32(box[1]));
                    Logger.Instance.InfoLog(box[0] + " is selected");
                }
            }

            //Set Viewer Type
            SetViewerTypeInNewDomain();

            //Add all Data Sources
            if (datasources == null)
            {
                SelectElement select = new SelectElement(BasePage.Driver.FindElement(By.CssSelector("[id*='DataSourceDisconnectedListBox']")));
                foreach (IWebElement item in select.Options)
                {
                    this.ConnectDataSourcesConsolidatedInNewDomain(item.Text);
                }


                try
                {
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogDiv")));
                    this.ClickButton("#ctl00_ConfirmButton");
                }
                catch (Exception) { }

                //Save Transaction
                this.ClickButton("#ctl00_MasterContentPlaceHolder_SaveButton");                               
                Thread.Sleep(3000);
                if (Driver.FindElements(By.CssSelector("#DialogDiv")).Count != 0)
                {
                    this.ClickButton("#ctl00_CloseAlertButton");
                }
                PageLoadWait.WaitForPageLoad(10);
            }
            if (alldatasource != 0 && datasources != null)
            {
                foreach (string ds in datasources)
                {
                    PageLoadWait.WaitForPageLoad(20);
                    ConnectDataSourcesConsolidatedInNewDomain(ds);
                    Logger.Instance.InfoLog("DataSource --" + ds + "is Connected.");
                    PageLoadWait.WaitForPageLoad(30);
                }

                try
                {
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogDiv")));
                    this.ClickButton("#ctl00_ConfirmButton");
                }
                catch (Exception) { }

                //Save Transaction
                this.ClickButton("#ctl00_MasterContentPlaceHolder_SaveButton");
                try
                {
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogDiv")));
                    this.ClickButton("#ctl00_CloseAlertButton");
                }
                catch (Exception) { }

                PageLoadWait.WaitForPageLoad(10);
            }
            if (datasource != null)
            {
                PageLoadWait.WaitForPageLoad(20);
                ConnectDataSourcesConsolidatedInNewDomain(datasource);
                Logger.Instance.InfoLog("DataSource --" + datasource + "is Connected.");
                PageLoadWait.WaitForPageLoad(30);

                try
                {
                    BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogDiv")));
                    this.ClickButton("#ctl00_ConfirmButton");
                }
                catch (Exception) { }

                //Save Transaction
                this.ClickButton("#ctl00_MasterContentPlaceHolder_SaveButton");
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogDiv")));
                this.ClickButton("#ctl00_CloseAlertButton");
                PageLoadWait.WaitForPageLoad(10);

            }

        }

        /// <summary>
        /// This function searches the given domain
        /// </summary>
        /// <param name="domain"></param>
        public Boolean SearchDomain(String domain)
        {
            try
            {
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#m_domainSearchControl_m_input1")));
                BasePage.Driver.FindElement(By.CssSelector("input#m_domainSearchControl_m_input1")).Clear();
                BasePage.Driver.FindElement(By.CssSelector("input#m_domainSearchControl_m_input1")).SendKeys(domain);
                BasePage.Driver.FindElement(By.CssSelector("input#m_domainSearchControl_m_searchButton")).Click();
                //PageLoadWait.WaitForPageLoad(20);
                PageLoadWait.WaitForFrameLoad(20);
                String result = Driver.FindElement(By.CssSelector("div.row tr td>span")).GetAttribute("innerHTML");
                if (result.Equals(domain) == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception while verifying existing domaim names for : " + domain +
                                         " for exception : " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// This function returns password criteria text
        /// </summary>
        public String PasswordCriteriaText()
        {
            BasePage.Driver.Manage().Timeouts().ImplicitWait = (TimeSpan.FromSeconds(1));
            string str1, str2, str3, str4, str5;
            if (SBrowserName.ToLower().Equals("internet explorer") && (BrowserVersion.ToLower().Equals("8")))
            {
                str1 = BasePage.Driver.FindElements(By.CssSelector("#PwdRequirementDlg>ul>li"))[0].Text;
                str2 = BasePage.Driver.FindElements(By.CssSelector("#PwdRequirementDlg>ul>li"))[1].FindElements(By.CssSelector("ul>li"))[0].Text;
                str3 = BasePage.Driver.FindElements(By.CssSelector("#PwdRequirementDlg>ul>li"))[1].FindElements(By.CssSelector("ul>li"))[1].Text;
                str4 = BasePage.Driver.FindElements(By.CssSelector("#PwdRequirementDlg>ul>li"))[1].FindElements(By.CssSelector("ul>li"))[2].Text;
                str5 = BasePage.Driver.FindElements(By.CssSelector("#PwdRequirementDlg>ul>li"))[1].FindElements(By.CssSelector("ul>li"))[3].Text;
            }
            else
            {
                str1 = BasePage.Driver.FindElement(By.CssSelector("#PwdRequirementDlg>ul>li:nth-child(1)")).Text;
                str2 = BasePage.Driver.FindElement(By.CssSelector("#PwdRequirementDlg>ul>li:nth-child(2)>ul>li:nth-child(1)")).Text;
                str3 = BasePage.Driver.FindElement(By.CssSelector("#PwdRequirementDlg>ul>li:nth-child(2)>ul>li:nth-child(2)")).Text;
                str4 = BasePage.Driver.FindElement(By.CssSelector("#PwdRequirementDlg>ul>li:nth-child(2)>ul>li:nth-child(3)")).Text;
                str5 = BasePage.Driver.FindElement(By.CssSelector("#PwdRequirementDlg>ul>li:nth-child(2)>ul>li:nth-child(4)")).Text;
            }


            return str1 + " " + str2 + " " + str3 + " " + str4 + " " + str5;
        }

        /// <summary>
        /// This function searches the given domain
        /// </summary>
        /// <param name="domain"></param>
        public Boolean IsDomainExist(String domain)
        {

            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#m_domainSearchControl_m_input1")));
            BasePage.Driver.FindElement(By.CssSelector("input#m_domainSearchControl_m_input1")).Clear();
            BasePage.Driver.FindElement(By.CssSelector("input#m_domainSearchControl_m_input1")).SendKeys(domain);
            if (SBrowserName.ToLower().Equals("internet explorer"))
            {
                ClickElement(BasePage.Driver.FindElement(By.CssSelector("input#m_domainSearchControl_m_searchButton")));
            }
            else
            {
                BasePage.Driver.FindElement(By.CssSelector("input#m_domainSearchControl_m_searchButton")).Click();
            }
            //PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            PageLoadWait.WaitForLoadingMessage1(10);
            try
            {
                String result = Driver.FindElement(By.CssSelector("div.row tr td>span")).GetAttribute("innerHTML");
                if (result.Equals(domain) == true)
                {
                    return true;
                }
            }
            catch (Exception e)
            {


                return false;

            }
            return false;
        }

        /// <summary>
        /// This is to create new domain
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="roleName"></param>
        public void CreateDomain(string domainName, string roleName, int hasPass = 0, string password = "")
        {

            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            bool DomainFlag = DomainExists(domainName);
            if (!DomainFlag)
            {
                ClickNewDomainBtn();
                BasePage.Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                //}
                //else {
                //    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                //}               
                //ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Name");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Name", domainName);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Description", domainName);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_ReceivingInstitution", domainName + "Inst");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_UserID", domainName.Replace(" ", "_"));
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_LastName", domainName.Replace(" ", "_") + "LastName");
                ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_FirstName");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_FirstName", domainName.Replace(" ", "_") + "FirstName");
                if (hasPass != 0)
                {
                    SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Password", password);
                    SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword", password);
                }
                else
                {
                    SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Password", domainName);
                    SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword", domainName);
                }

                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Name", roleName);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Description", roleName);

                //Set Viewer Type
                SetViewerTypeInNewDomain();

                ConnectDataSourcesInNewDomain();
                SaveDomain();
                Thread.Sleep(5000);
                //ClickSaveDomain();
            }
            else
            {
                Logger.Instance.ErrorLog("Domain Name already exists. hence not creating new Domain");
            }
        }
        public void EnableImageSharingICAEditDomain()
        {
            try
            {
                SetCheckbox("id",
                                             "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_EnableImageSharingCB");

                Thread.Sleep(1500);

                ClearText("id",
                                           "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_UploadStudyValidDaysTB");
                SetText("id",
                                         "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_UploadStudyValidDaysTB",
                                         "30");

                ClearText("id",
                                           "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_ArchivedStudyValidDaysTB");
                SetText("id",
                                         "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_ArchivedStudyValidDaysTB",
                                         "30");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step EnableImageSharingICAEditDomain due to : " + ex);
            }
        }

        /// <summary>
        ///     This function connects the specified data sources for both add/Edit domain
        /// </summary>
        public void ConnectDataSource(string datasourceName)
        {
            bool valueSelected = false;
            BasePage.Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            string[] disconnectedAllOptions = GetValuesfromDropDown("cssselector", "select[id$='DataSourceDisconnectedListBox']");
            int index = 0;

            while (index < disconnectedAllOptions.Length)
            {
                if (disconnectedAllOptions[index] == datasourceName)
                {
                    SelectFromList("cssselector", "select[id$='DataSourceDisconnectedListBox']", datasourceName);
                    Click("cssselector", "input[id$='_DataInfo_ConnectDataSource']");
                    PageLoadWait.WaitForPageLoad(20);

                    try
                    {
                        //--adding holding pen datasource popup confirmation
                        IWebElement confirmButton = Driver.FindElement(By.Id("ctl00_ConfirmButton"));
                        if (confirmButton.Displayed)
                        {
                            confirmButton.Click();
                            Logger.Instance.InfoLog("The Selected data source has holding pen & the Pop-up clicked successfully");
                        }
                    }
                    catch (NoSuchElementException e)
                    {
                        Logger.Instance.InfoLog("The Selected data source does not have holding pen");
                    }
                    break;
                }
                index++;
            }
            index = 0;

            string[] connectedAllOptions = GetValuesfromDropDown("cssselector", "select[id$='DataSourceConnectedListBox']");
            while (index < connectedAllOptions.Length)
            {
                if (connectedAllOptions[index] == datasourceName)
                {
                    Logger.Instance.InfoLog("Data source :" + datasourceName + " is connected successfully");
                    valueSelected = true;
                    break;
                }

                index++;
            }

            if (valueSelected == false)
            {
                Logger.Instance.ErrorLog("Data source: " + datasourceName + " is not connected - verified failed");
            }
        }



        /// <summary>
        ///     This function disconnects the specified data sources for both add/Edit domain
        /// </summary>
        public void DisConnectDataSource(string datasourceName)
        {
            bool valueSelected = false;
            string[] connectedAllOptions = GetValuesfromDropDown("cssselector", "select[id$='DataSourceConnectedListBox']");
            int index = 0;

            while (index < connectedAllOptions.Length)
            {
                if (connectedAllOptions[index] == datasourceName)
                {
                    SelectFromList("cssselector", "select[id$='DataSourceConnectedListBox']", datasourceName);
                    Click("cssselector", "input[id$='_DataInfo_DisconnectDataSource']");

                    PageLoadWait.WaitForPageLoad(20);
                    try
                    {
                        //--adding holding pen datasource popup confirmation
                        IWebElement confirmButton = Driver.FindElement(By.Id("ctl00_ConfirmButton"));
                        if (confirmButton.Displayed)
                        {
                            confirmButton.Click();
                            Logger.Instance.InfoLog("The Selected data source has holding pen & the Pop-up clicked successfully");
                        }
                    }
                    catch (NoSuchElementException e)
                    {
                        Logger.Instance.InfoLog("The Selected data source does not have holding pen");
                    }
                    break;
                }
                index++;
            }
            index = 0;
            PageLoadWait.WaitForPageLoad(20);
            string[] DisconnectedAllOptions = GetValuesfromDropDown("cssselector", "select[id$='DataSourceDisconnectedListBox']");
            while (index < DisconnectedAllOptions.Length)
            {
                if (DisconnectedAllOptions[index] == datasourceName)
                {
                    Logger.Instance.InfoLog("Data source :" + datasourceName + " is  disconnected successfully");
                    valueSelected = true;
                    break;
                }

                index++;
            }

            if (valueSelected == false)
            {
                Logger.Instance.ErrorLog("Data source: " + datasourceName + " is not disconnected - verified failed");
            }
        }


        /// <summary>
        ///     This function disconnects the all the data sources for both Add/Edit domain
        /// </summary>
        public void DisConnectAllDataSources()
        {
            PageLoadWait.WaitForPageLoad(20);
            string[] connectedAllOptions = GetValuesfromDropDown("cssselector", "select[id$='DataSourceConnectedListBox']");

            foreach (string s in connectedAllOptions)
            {
                SelectFromList("cssselector", "select[id$='DataSourceConnectedListBox']", s);
                Click("cssselector", "input[id$='_DataInfo_DisconnectDataSource']");

                PageLoadWait.WaitForPageLoad(20);
                //--adding holding pen datasource popup confirmation
                try
                {
                    IWebElement confirmButton = Driver.FindElement(By.Id("ctl00_ConfirmButton"));
                    if (confirmButton.Displayed)
                    {
                        confirmButton.Click();
                        Logger.Instance.InfoLog("The Selected data source has holding pen & the Pop-up clicked successfully");
                    }
                }
                catch (NoSuchElementException e)
                {
                    Logger.Instance.InfoLog("The Selected data source does not have holding pen");
                }
            }
            //--verification--
            PageLoadWait.WaitForPageLoad(20);
            try
            {
                IWebElement FirstOption = Driver.FindElement(By.CssSelector("select[id$='DataSourceConnectedListBox']>option"));
                if (!FirstOption.Displayed)
                    Logger.Instance.InfoLog("All Data sources are disconnected successfully");

                else
                    Logger.Instance.ErrorLog(" Error on disconnecting all data source- verified failed");
            }
            catch (NoSuchElementException e)
            {
                Logger.Instance.InfoLog(" All Data sources are disconnected successfully ");
            }
        }

        /// <summary>
        ///     This function connects the all the data sources for both Add/Edit domain
        /// </summary>
        public void ConnectAllDataSources()
        {

            PageLoadWait.WaitForPageLoad(20);
            string[] DisConnectedAllOptions = GetValuesfromDropDown("cssselector", "select[id$='DataSourceDisconnectedListBox']");

            foreach (string s in DisConnectedAllOptions)
            {
                SelectFromList("cssselector", "select[id$='DataSourceDisconnectedListBox']", s);
                Click("cssselector", "input[id$='_DataInfo_ConnectDataSource']");

                PageLoadWait.WaitForPageLoad(20);
                //--adding holding pen datasource popup confirmation
                try
                {
                    IWebElement confirmButton = Driver.FindElement(By.Id("ctl00_ConfirmButton"));
                    if (confirmButton.Displayed)
                    {
                        confirmButton.Click();
                        Logger.Instance.InfoLog("The Selected data source has holding pen & the Pop-up clicked successfully");
                    }
                }
                catch (NoSuchElementException e)
                {
                    Logger.Instance.InfoLog("The Selected data source does not have holding pen");
                }
            }
            //--verification--
            PageLoadWait.WaitForPageLoad(20);
            try
            {
                if (BasePage.Driver.FindElements(By.CssSelector
                    ("select[id$='DataSourceDisconnectedListBox']>option")).Count==0)
                    Logger.Instance.InfoLog(" All Data sources are connected successfully");

                else
                    Logger.Instance.ErrorLog(" Error on connecting all data source- verified failed");
            }
            catch (NoSuchElementException e)
            {
                Logger.Instance.InfoLog(" All Data sources are connected successfully");
            }
        }


        /// <summary>
        /// This function clicks on the Save Domain button
        /// </summary>
        public void SaveDomain()
        {
            try
            {
                IWebElement element = GetElement("id", "ctl00_MasterContentPlaceHolder_SaveButton");

                int timeout = 0;
                Boolean flag = true;
                while (flag && timeout < 5)
                {
                    element.Click();
                    Thread.Sleep(3000);

                    SwitchToDefault();
                    SwitchTo("index", "0");
                    Click("id", "ctl00_CloseAlertButton");

                    if (GetElement("id", "TabText0") == null)
                    {
                        element.Click();
                        Thread.Sleep(1000);
                        timeout = timeout + 1;
                    }
                    else
                    {
                        flag = false;
                        Logger.Instance.InfoLog("Domain saved succesfully");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception encountered in ClickSaveDomain due to " + ex.Message);
            }
        }

        public void AddPreset(string modality, string preset, string width, string level, string layout = "auto")
        {
            Thread.Sleep(1000);
            var select = new SelectElement(GetElement("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_m_viewingProtocolsControl_DropDownListModalities"));
            Thread.Sleep(1000);
            var selectlayout =
                new SelectElement(GetElement("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_m_viewingProtocolsControl_DropDownListLayout"));
            Thread.Sleep(1000);
            IWebElement presetName = GetElement("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_m_viewingProtocolsControl_AliasTextBox");
            IWebElement widthField = GetElement("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_m_viewingProtocolsControl_WidthTextBox");
            IWebElement levelField = GetElement("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_m_viewingProtocolsControl_LevelTextBox");

            IWebElement savePreset = GetElement("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_m_viewingProtocolsControl_SaveAliasButton");
            Thread.Sleep(1000);
            select.SelectByText(modality);
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
        }
        /// <summary>
        /// This helper method hide/show the options in Study search fields
        /// </summary>
        /// <param name="type">Show/Hide</param>
        /// <param name="Options">Visible text of options</param>
        public void ModifyStudySearchFields(String type = "show", string[] Options = null)
        {
            //Sync-up
            PageLoadWait.WaitForPageLoad(10);
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("window.scroll(800, 800)");

            if (type.ToLower().Equals("show"))
            {
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector("select[id$='ssclHiddenSearchFieldsLB']")));
                if (Options != null)
                {
                    foreach (string option in Options)
                    {
                        try { HiddenSearchField().SelectByText(option); }
                        catch (Exception) { }
                    }
                }
                else
                {
                    IList<IWebElement> options = HiddenSearchField().Options;
                    foreach (IWebElement option in options)
                    {
                        option.Click();

                    }
                }
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                wait.Until(ExpectedConditions.ElementToBeClickable(ShowBtn()));
                ShowBtn().Click();
            }
            else
            {
                wait.Until(ExpectedConditions.ElementExists(By.CssSelector("select[id$='ssclVisibleSearchFieldsLB']")));
                if (Options != null)
                {
                    foreach (string option in Options)
                    {
                        try { VisibleSearchField().SelectByText(option); }
                        catch (Exception) { }
                    }
                }
                else
                {
                    IList<IWebElement> options = VisibleSearchField().Options;
                    foreach (IWebElement option in options)
                    {
                        option.Click();
                    }
                }
                Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                wait.Until(ExpectedConditions.ElementToBeClickable(HideBtn()));
                HideBtn().Click();
            }

        }


        /// <summary>
        /// This methis will reorder fields in the Study search fields dropdown
        /// </summary>
        /// <param name="elementindex">element needs to be moded, zero based index</param>
        /// <param name="move">Position - need to be moved up or down</param>
        /// <param name="timestomove">number of times to move</param>
        public String[] ReorderStudySearchFields(int elementindex, int timestomove, String movetype = "UP")
        {
            //Select Elements
            this.VisibleSearchField().DeselectAll();
            this.VisibleSearchField().SelectByIndex(elementindex);

            //Move up or Down
            if (movetype.ToLower().Equals("up"))
            {

                for (int iterate = 0; iterate <= timestomove; iterate++)
                {
                    this.ClickElement(this.Up());
                }
            }
            else
            {
                for (int iterate = 0; iterate <= timestomove - 1; iterate++)
                {
                    this.ClickElement(this.Down());
                }
            }

            IList<IWebElement> searchquerelements = this.VisibleSearchField().Options;
            String[] searchfields = searchquerelements.Select((element) => element.Text).ToArray();
            return searchfields;
        }


        /// <summary>
        ///     This function makes the all study search fields to visible for both Add/Edit domain
        /// </summary>

        public void VisibleAllStudySearchField()
        {

            PageLoadWait.WaitForPageLoad(20);
            string[] HiddenSearchFieldAllOptions = GetValuesfromDropDown("cssselector", "select[id$='HiddenSearchFieldsLB']");

            foreach (string s in HiddenSearchFieldAllOptions)
            {
                SelectFromList("cssselector", "select[id$='HiddenSearchFieldsLB']", s);
                Click("cssselector", "input[id$='ConfigControl_ssclAddButton']");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(5);
            }
        }


        /// <summary>
        ///     This function makes the all study search fields to hide for both Add/Edit domain
        /// </summary>

        public void HideAllStudySearchField()
        {

            PageLoadWait.WaitForPageLoad(20);
            string[] VisibleSearchFieldAllOptions = GetValuesfromDropDown("cssselector", "select[id$='VisibleSearchFieldsLB']");

            foreach (string s in VisibleSearchFieldAllOptions)
            {
                SelectFromList("cssselector", "select[id$='VisibleSearchFieldsLB']", s);
                Click("cssselector", "input[id$='ConfigControl_ssclRemoveButton']");
                PageLoadWait.WaitForPageLoad(10);
                PageLoadWait.WaitForFrameLoad(5);
            }
        }

        /// <summary>
        ///     This function makes the specified study search field to visible for both add/Edit domain
        /// </summary>
        public void VisibleStudySearchField(string FieldName)
        {
            string[] HiddenAllOptions = GetValuesfromDropDown("cssselector", "select[id$='HiddenSearchFieldsLB']");
            int index = 0;
            String temp1 = Regex.Replace(FieldName, @"\s+", "");

            while (index < HiddenAllOptions.Length)
            {
                if (HiddenAllOptions[index].ToLower().Contains(temp1.ToLower()))
                {
                    SelectFromList("cssselector", "select[id$='HiddenSearchFieldsLB']", FieldName, 1);
                    Click("cssselector", "input[id$='ConfigControl_ssclAddButton']");
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(5);
                    break;
                }
                index++;
            }
        }

        /// <summary>
        ///     This function makes the specified study search field to hide for both add/Edit domain
        /// </summary>

        public void HideStudySearchField(string FieldName)
        {
            string[] VisibleAllOptions = GetValuesfromDropDown("cssselector", "select[id$='VisibleSearchFieldsLB']");
            int index = 0;
            String temp1 = Regex.Replace(FieldName, @"\s+", "");

            while (index < VisibleAllOptions.Length)
            {
                if (VisibleAllOptions[index].ToLower().Contains(temp1.ToLower()))
                {
                    SelectFromList("cssselector", "select[id$='VisibleSearchFieldsLB']", FieldName, 1);
                    Click("cssselector", "input[id$='ConfigControl_ssclRemoveButton']");
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(5);
                    break;
                }
                index++;
            }
        }

        public void AddPresetForDomain(string modality, string preset, string width, string level, string layout = "auto", string user = "admin")
        {
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            PageLoadWait.WaitForFrameLoad(10);

            IWebElement presetName = GetElement("cssselector", "input[id$='_AliasTextBox");
            IWebElement widthField = GetElement("cssselector", "input[id$='_WidthTextBox");
            IWebElement levelField = GetElement("cssselector", "input[id$='_LevelTextBox");
            IWebElement savePreset = GetElement("cssselector", "input[id$='_SaveAliasButton");

            if (user.Equals("domain"))
            {
                new SelectElement(Driver.FindElement(By.CssSelector("#EditDomainControl_DomainInfo_m_viewingProtocolsControl_DropDownListModalities"))).SelectByText(modality);
                new SelectElement(Driver.FindElement(By.CssSelector("#EditDomainControl_DomainInfo_m_viewingProtocolsControl_DropDownListLayout"))).SelectByText(layout);
            }
            else
            {
                new SelectElement(GetElement("cssselector", "select[id$='_DropDownListModalities']")).SelectByText(modality);
                new SelectElement(GetElement("cssselector", "select[id$='_DropDownListLayout")).SelectByText(layout);
            }
            presetName.Clear();
            presetName.SendKeys(preset);
            widthField.Clear();
            widthField.SendKeys(width);

            levelField.Clear();
            levelField.SendKeys(level);

            savePreset.Click();
            savePreset.Click();
        }

        public bool VerifyPresetsInDomain(string modality, string layout, string preset, bool value = true)
        {
            bool IsPresetPresent = false;
            ModalityDropDown().SelectByText(modality);
            LayoutDropDown().SelectByText(layout);
            IList<IWebElement> options = Driver.FindElement(By.CssSelector("[id$='DomainInfo_m_viewingProtocolsControl_DropDownListAlias']")).FindElements(By.TagName("option"));
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
            return IsPresetPresent;
        }

        public void RemoveAllPresets(string modality)
        {
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            PageLoadWait.WaitForFrameLoad(10);
            var select = new SelectElement(GetElement("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_m_viewingProtocolsControl_DropDownListModalities"));
            var presetList = new SelectElement(GetElement("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_m_viewingProtocolsControl_DropDownListAlias"));
            IWebElement delPreset = GetElement("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_m_viewingProtocolsControl_RemoveAliasButton");

            select.SelectByText(modality);
            int count = presetList.Options.Count + 1;

            for (int i = 0; i < count; i++)
            {
                delPreset.Click();
            }
        }

        public void RemovePreset(string modality, string preset)
        {
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            PageLoadWait.WaitForFrameLoad(10);
            var select = new SelectElement(GetElement("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_m_viewingProtocolsControl_DropDownListModalities"));
            var presetList = new SelectElement(GetElement("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_m_viewingProtocolsControl_DropDownListAlias"));
            IWebElement delPreset = GetElement("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_m_viewingProtocolsControl_RemoveAliasButton");

            select.SelectByText(modality);
            int count = presetList.Options.Count + 1;
            presetList.SelectByText(preset);
            delPreset.Click();

        }

        /// <summary>
        /// This is to create new domain with setting most of the required checkboxes as initial setup
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="roleName"></param>
        public void CreateDomain(string domainName, string roleName, String[] DS, String[] datasources = null, int check = 0)
        {

            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            bool DomainFlag = DomainExists(domainName);
            if (!DomainFlag)
            {
                ClickNewDomainBtn();

                //Setting  the checkboxes            

                foreach (String ds in DS)
                {
                    SetCheckBoxInEditDomain(ds, check);
                }

                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Name", domainName);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_Description", domainName);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_DomainInfo_ReceivingInstitution", domainName + "Inst");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_UserID", domainName.Replace(" ", "_"));
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_LastName", domainName.Replace(" ", "_") + "LastName");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_FirstName", domainName.Replace(" ", "_") + "FirstName");
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_Password", domainName);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_UserInfo_ComparisonPassword", domainName);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Name", roleName);
                SetText("cssselector", "#ctl00_MasterContentPlaceHolder_RoleAccessFilter_Description", roleName);

                VisibleAllStudySearchField();

                //Set Viewer Type
                SetViewerTypeInNewDomain();

                //Add all Data Sources
                if (datasources == null)
                {
                    SelectElement select = new SelectElement(BasePage.Driver.FindElement(By.CssSelector("[id*='DataSourceDisconnectedListBox']")));
                    foreach (IWebElement item in select.Options)
                    {
                        this.ConnectDataSourcesConsolidatedInNewDomain(item.Text);
                    }


                    try
                    {
                        BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogDiv")));
                        this.ClickButton("#ctl00_ConfirmButton");
                    }
                    catch (Exception) { }

                    //Save Transaction


                    this.ClickButton("#ctl00_MasterContentPlaceHolder_SaveButton");
                    try
                    {
                        BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogDiv")));
                        this.ClickButton("#ctl00_CloseAlertButton");
                    }
                    catch (Exception)
                    { }
                    PageLoadWait.WaitForPageLoad(10);
                }

                //Select Data Sources Required
                else
                {
                    foreach (String datasource in datasources)
                    {
                        this.ConnectDataSourcesConsolidatedInNewDomain(datasource);
                    }

                    //Save Transaction                    
                    this.ClickButton("#ctl00_MasterContentPlaceHolder_SaveButton");
                    try
                    {
                        BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#DialogDiv")));
                        this.ClickButton("#ctl00_CloseAlertButton");
                    }
                    catch (Exception) { }
                    PageLoadWait.WaitForFrameToBeVisible(15);
                    PageLoadWait.WaitForPageLoad(10);
                }
            }
            else
            {
                Logger.Instance.ErrorLog("Domain Name already exists. hence not creating new Domain");
            }
        }

        public void CreateConfUsrDomain(String domain)
        {
            CreateDomain(domain, domain, 0);
            SearchDomain(domain);
            SelectDomain(domain);
            ClickEditDomain();
            PageLoadWait.WaitForFrameLoad(5);
            BasePage.Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            SetCheckBoxInEditDomain("conferencelists", 0);
            PageLoadWait.WaitForFrameLoad(5);
            ClickSaveEditDomain();
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(20);
        }

        public void SetConfListFeatureForDomain(string domain)
        {
            SearchDomain(domain);
            SelectDomain(domain);
            ClickEditDomain();
            PageLoadWait.WaitForFrameLoad(5);
            BasePage.Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            SetCheckBoxInEditDomain("conferencelists", 0);
            PageLoadWait.WaitForFrameLoad(5);
            ClickSaveEditDomain();
            //PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(20);
        }

        /// <summary>
        /// This method will Set the domain attributes for debuggin purpose
        /// </summary>
        /// <returns></returns>
        public Dictionary<Object, String> SetDomainAttr(String domainname, String description, string institutionname,
        String userid, String lastname, String firstname, String emailaddress,
        String password, String rolename, String roledescription)
        {
            Dictionary<Object, String> domainattr = new Dictionary<Object, string>();

            domainattr.Add(DomainAttr.DomainName, domainname);
            domainattr.Add(DomainAttr.DomainDescription, description);
            domainattr.Add(DomainAttr.InstitutionName, institutionname);
            domainattr.Add(DomainAttr.UserID, userid);
            domainattr.Add(DomainAttr.LastName, lastname);
            domainattr.Add(DomainAttr.FirstName, firstname);
            domainattr.Add(DomainAttr.EmailAddress, Config.emailid);
            domainattr.Add(DomainAttr.Password, password);
            domainattr.Add(DomainAttr.RoleName, rolename);
            domainattr.Add(DomainAttr.RoleDescription, roledescription);
            foreach (KeyValuePair<Object, String> keyvalue in domainattr)
            {
                Logger.Instance.InfoLog("The Domain Attributes are--" + keyvalue.Key + "---" + keyvalue.Value);
            }

            return domainattr;
        }

        public void ClickDeleteDomainBtn()
        {
            PageLoadWait.WaitForFrameLoad(15);
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            Click("id", "DeleteDomainButton");
            PageLoadWait.WaitForFrameLoad(30);
            PageLoadWait.WaitForFrameLoad(15);
            Logger.Instance.InfoLog("Domain Deleted successfully");

        }

        ///summary
        ///--- after clicking delete domain, if domain has user then click on OK button to close the alert
        ///summary
        public void ClickCloseAlertButton()
        {

            Driver.SwitchTo().DefaultContent();
            SwitchTo("id", "UserHomeFrame");
            BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_CloseAlertButton")));
            //Driver.FindElement(By.CssSelector("#ctl00_CloseAlertButton")).Click(); 
            Click("id", "ctl00_CloseAlertButton");
            PageLoadWait.WaitForFrameLoad(30);
            PageLoadWait.WaitForFrameLoad(15);
            Logger.Instance.InfoLog("Alert closed successfully");

        }

        public void ConfirmDeleteDomain()
        {
            Driver.SwitchTo().DefaultContent();
            SwitchTo("id", "UserHomeFrame");
            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='ConfirmButton']")));
            BasePage.Driver.FindElement(By.CssSelector("[id$='ConfirmButton']")).Click();
            BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector("[id$='ConfirmButton']")));
            PageLoadWait.WaitForFrameLoad(30);
            PageLoadWait.WaitForFrameLoad(15);
            Logger.Instance.InfoLog("Domain Deleted successfully");
        }
        /// <summary>
        ///     This function makes the specified data sources connected to a Edit domain
        /// </summary>
        public void DisConnectDataSourcesConsolidated_EditDomain(string datasourceName)
        {
            bool valueSelected = false;
            string[] connectedAllOptions = GetValuesfromDropDown("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_DataSourceConnectedListBox");

            int index = 0;

            while (index < connectedAllOptions.Length)
            {
                if (connectedAllOptions[index] == datasourceName)
                {
                    SelectFromList("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_DataSourceConnectedListBox", datasourceName);
                    Click("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_DisconnectDataSource");

                    Thread.Sleep(2000);

                    //--adding holding pen datasource popup confirmation
                    IWebElement confirmButton = Driver.FindElement(By.Id("ctl00_ConfirmButton"));
                    if (confirmButton.Displayed)
                    { confirmButton.Click(); }

                }
                index++;
            }
            index = 0;

            string[] disconnectedAllOptions = GetValuesfromDropDown("id", "ctl00_MasterContentPlaceHolder_EditDomainControl_DataInfo_DataSourceDisconnectedListBox");
            while (index < connectedAllOptions.Length)
            {
                if (disconnectedAllOptions[index] == datasourceName)
                {
                    Logger.Instance.InfoLog("Data sources disconnected successfully");
                    valueSelected = true;
                    break;
                }

                index++;
            }

            if (valueSelected == false)
            {
                Logger.Instance.ErrorLog("Data sources not connected - verified failed");
            }

        }


        /// <summary>
        /// This function verify the datasources of given domain in Domain management page
        /// </summary>
        /// <param name="domain"></param>
        public bool verifyDomainDatasources(String domainName, String domainDataSource)
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            //wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row tr td>span[title='" + domainName + "']")));
            IList<IWebElement> domains = Driver.FindElements(By.XPath("//*[@id='m_domainListControl_m_dataListGrid']/tbody/tr[*]/td[1]/span"));
            int i = 2;

            foreach (IWebElement ele in domains)
            {
                if (ele.Text.Equals(domainName))
                {
                    string xpathDomainDatasource = "//*[@id='m_domainListControl_m_dataListGrid']/tbody/tr[" + i + "]/td[3]/span";
                    IWebElement datasource = Driver.FindElement(By.XPath(xpathDomainDatasource));
                    String ds = datasource.Text;
                    String[] Datasources = ds.Split(',');
                    if (Datasources.Contains(domainDataSource))
                    {
                        Logger.Instance.InfoLog("Data sources of Domain " + domainName + " is " + domainDataSource + " verified successfully");
                        return true;
                    }

                    else
                    {
                        Logger.Instance.ErrorLog("Data sources of Domain " + domainName + " is not " + domainDataSource + " verified failed");
                        return false;
                    }
                }

                i++;
            }
            return false;
        }

        public void AddInsitutionInEditPage(String Domain, String InstitutionName)
        {
            SearchDomain(Domain);
            SelectDomain(Domain);
            EditDomainButton().Click();
            Driver.SwitchTo().DefaultContent();
            PageLoadWait.WaitForFrameLoad(10);
            PageLoadWait.WaitForPageLoad(10);
            EditDomainInstituitionNameTxtBox().SendKeys(InstitutionName);
            InstituitionAddButton().Click();
        }
        /// <summary>
        /// This function will return all domain name in the Domain management page
        /// </summary>
        public string[] DomainNameList()
        {
            List<string> s = new List<string>();


            try
            {
                SwitchToDefault();
                SwitchTo("index", "0");
                SwitchTo("index", "1");
                SwitchTo("index", "0");
                ReadOnlyCollection<IWebElement> elements =
                   Driver.FindElements(
                       By.XPath("//table[@id='m_domainListControl_m_dataListGrid']/tbody/tr"));

                for (int i = 1; i < elements.Count; i++)
                {
                    s.Add(elements[i].FindElement(By.XPath("//tr[" + (i + 1) + "]/td[1]/span")).GetAttribute("title"));

                }

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception encountered in DomainNameList due to " + ex.Message);
            }
            return s.ToArray();
        }

        public void AddContactEditDomain(string Name, string Number)
        {

            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("[id$='EditDomainControl_DomainContacts_AddContactButton']")));
            IWebElement addbtn = BasePage.Driver.FindElement(By.CssSelector("[id$='EditDomainControl_DomainContacts_AddContactButton']"));
            IWebElement ContactNameTb = BasePage.Driver.FindElement(By.CssSelector("[id$='_DomainContacts_ContactNameTextBox']"));
            IWebElement ContactNoTb = BasePage.Driver.FindElement(By.CssSelector("[id$='EditDomainControl_DomainContacts_ContactNumberTextBox']"));
            ContactNameTb.Click();
            ContactNameTb.Clear();
            ContactNameTb.SendKeys(Name);
            ContactNoTb.Clear();
            ContactNoTb.SendKeys(Number);
            addbtn.Click();
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(30);
        }

        public void RoleAccessFilterForModality(String modality)
        {
            RoleAccessFilterSelectList().SelectByValue("Modality");
            ModalityListBox().SelectByText(modality);
            AddAccessFilterButton().Click();
        }

        /// <summary>
        /// This helper method will setup the combination of study query parameters
        /// </summary>
        /// <param name="domainname"></param>
        /// <param name=""></param>
        public void SetupStudyQueryParameters(String domainname, QueryParamters[] paramters)
        {   
            this.SearchDomain(domainname);
            this.SelectDomain(domainname);
            this.ClickEditDomain();
            this.ScrollIntoView(this.PatientFullnameCheckbox());

            //Check all required query parameters
            foreach (var field in paramters)
            {
                if (field == QueryParamters.FullName)                    
                    this.SetCheckbox(this.PatientFullnameCheckbox(), true);

                else if (field == QueryParamters.LastName)
                    this.SetCheckbox(this.PatientLastnameCheckbox(), true);

                else if (field == QueryParamters.PatientID)
                    this.SetCheckbox(this.PatientIDCheckbox(), true);

                else if (field == QueryParamters.IPID)
                    this.SetCheckbox(this.IPIDCheckBox(), true);

                else if(field == QueryParamters.PatientDOB)
                        this.SetCheckbox(this.PatientDOBCheckBox(), true);            
            }

            //UnCheck parameters not required            
            if (!(paramters.Contains(QueryParamters.FullName)))
                this.UnCheckCheckbox(this.PatientFullnameCheckbox(), isJSClick: true);

            if (!(paramters.Contains(QueryParamters.LastName)))
                this.UnCheckCheckbox(this.PatientLastnameCheckbox(), isJSClick: true);

            if (!(paramters.Contains(QueryParamters.PatientID)))
                this.UnCheckCheckbox(this.PatientIDCheckbox(), isJSClick:true);

            if (!(paramters.Contains(QueryParamters.IPID)))
                this.UnCheckCheckbox(this.IPIDCheckBox(), isJSClick:true);

            if (!(paramters.Contains(QueryParamters.PatientDOB)))
                this.UnCheckCheckbox(this.PatientDOBCheckBox(), isJSClick:true);         

            this.ClickSaveEditDomain();            
        }

        public void SetViewerTypeInNewDomain(string ViewerType = "universal")
        {
            try
            {
                if (!Domain_UniversalViewer().Selected)
                {
                    ClickElement(Domain_UniversalViewer());
                }
                else
                {
                    Logger.Instance.InfoLog(" Enable Universal viewer is already selected in domain page");
                }
            }
            catch { Logger.Instance.InfoLog("Error while trying to enable Universal viewer in domainpage"); }
            try
            {
                if (!Role_UniversalViewer().Selected)
                {
                    ClickElement(Role_UniversalViewer());
                }
                else
                {
                    Logger.Instance.InfoLog(" Enable Universal viewer is already selected in role management");
                }
            }
            catch
            {
                Logger.Instance.InfoLog("Error while trying to enable Universal viewer in rolemanagement");
            }
            if (string.Equals(ViewerType, "universal", StringComparison.OrdinalIgnoreCase))
            {
                ClickElement(DefaultUniversalViewer());
            }
            else
            {
                ClickElement(DefaultEnterpriseViewer());
            }
        }
        public void Check_Imgsharing_Enable(bool flag = true, bool webconsent = true)
        {
            Login login = new Login();
            try
            {
                //String username = Config.adminUserName;
                //String password = Config.adminPassword;
                //String DomainName = Config.adminGroupName;
                //login.LoginIConnect(username, password);
                //login.Navigate("DomainManagement");
                //Actions action = new Actions(BasePage.Driver);
                //bool DMExisits = this.DomainExists(DomainName);
                //if (DMExisits)
                //{
                //    this.SelectDomain(DomainName);
                //}
                //IList<IWebElement> d = Driver.FindElements(By.CssSelector("div.row tr td>span[title='" + DomainName + "']"));
                //IWebElement DomainElement_RightClick = null;

                //foreach (IWebElement elm in d)
                //{
                //    if (elm.Text.Equals(DomainName))
                //    {
                //        DomainElement_RightClick = elm;
                //        break;
                //    }
                //}

                //action.ContextClick(DomainElement_RightClick).Build().Perform();

                //BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("#GlobalGridContextMenuDiv div")));
                //IList<IWebElement> DomainOptions = BasePage.Driver.FindElements(By.CssSelector("#GlobalGridContextMenuDiv div"));
                //int counter = 0;
                //String[] options1 = new String[DomainOptions.Count];
                //foreach (IWebElement option in DomainOptions)
                //{
                //    options1[counter] = option.Text;
                //    counter++;
                //}

                //action.MoveToElement(DomainOptions[1]).Click().Build().Perform();
                Thread.Sleep(10000);
                SwitchToDefault();
                SwitchToFrameUsingElement("id", "UserHomeFrame");
                //check enable image sharing check box
                IWebElement enableImagesharing = this.EnableImagesharing();
                if (enableImagesharing.Displayed)
                {
                    if (flag == true)
                    {
                        if (enableImagesharing.Selected == false)
                        {
                            enableImagesharing.Click();
                            IWebElement consentwebuploader = Driver.FindElement(By.Id("ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_DisplayHIPAAComplianceCB"));
                            if (webconsent == true)
                            {
                                if (consentwebuploader.Displayed == true && consentwebuploader.Selected == false)
                                {
                                    consentwebuploader.Click();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (enableImagesharing.Enabled == true)
                        {
                            enableImagesharing.Click();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error raise d in Check_Imgsharing_Enable " + e);
            }
        }
        /// <summary>
        /// 
        ///Select Enable 3D View checkBox
        /// </summary>
        public void Enable3DView()
        {
            if (!Enable3DViewCheckbox().Selected)
            {
                ClickElement(Enable3DViewCheckbox());
            }
        }

        /// <summary>
        /// To set the modality thumbnail splitting
        /// </summary>
        /// <param name="option"></param>
        public void SetThumbnailSpiltting(String option)
        {
            PageLoadWait.WaitForFrameLoad(40);
            try { wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[id$='Domain_Content']"))); }
            catch (NoSuchElementException e) { }
            String FieldName = option.ToLower();
            string radioButton = "";
            switch (FieldName)
            {
                case "auto":
                    radioButton = "#ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_m_viewingProtocolsControl_ThumbSplitRadioButtons_0";
                    break;
                case "series":
                    radioButton = "#ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_m_viewingProtocolsControl_ThumbSplitRadioButtons_1";
                    break;
                case "image":
                    radioButton = "#ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_m_viewingProtocolsControl_ThumbSplitRadioButtons_2";
                    break;
            }

            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", Driver.FindElement(By.CssSelector(radioButton)));
            Logger.Instance.InfoLog(radioButton + " is set");
        }

        /// <summary>
        /// This method is used to Enable/Disable the Localizer based on Modality
        /// </summary>
        /// <param name="ModalityList"></param>
        /// <param name="Enable"></param> To enable pass "true" and to disable pass "False"
        public void SetLocalizerByModality(String[] modalityList, bool enable = true)
        {
            foreach (String modality in modalityList)
            {
                SetLocalizerByModality(modality, enable);
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
            PageLoadWait.WaitForPageLoad(10);
            if (enable)
                SelectRadioBtn("LocalizerLineRadioButtons", "On");
            else
                SelectRadioBtn("LocalizerLineRadioButtons", "Off");
            Thread.Sleep(500);

        }

    }
}
