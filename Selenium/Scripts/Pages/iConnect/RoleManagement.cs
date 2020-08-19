using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using Selenium.Scripts.Reusable.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using System.Configuration;
using System.Collections.ObjectModel;

namespace Selenium.Scripts.Pages.iConnect
{
    class RoleManagement : BasePage
    {
        public RoleManagement() { }

        public IWebElement NewRoleLabel() { wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl00_MasterContentPlaceHolder_Label1"))); return Driver.FindElement(By.Id("ctl00_MasterContentPlaceHolder_Label1")); }
        public IWebElement SearchRoleBtn() { return Driver.FindElement(By.Id("m_roleSearchControl_m_searchButton")); }
        public IWebElement NewRoleBtn() { return Driver.FindElement(By.Id("NewRoleButton")); }
        public IWebElement EditRoleBtn() { return Driver.FindElement(By.Id("EditRoleButton")); }
        public IWebElement DeleteRoleBtn() { return Driver.FindElement(By.Id("DeleteRoleButton")); }
        public IWebElement AlertText() { wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl00_AlertText"))); return Driver.FindElement(By.Id("ctl00_AlertText")); }
        public IWebElement CloseAlertBox() { wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl00_CloseAlertButton"))); return Driver.FindElement(By.Id("ctl00_CloseAlertButton")); }
        public IWebElement LdapDataMapButtonRoleBtn() { return Driver.FindElement(By.Id("LdapDataMapButton")); }
        public IWebElement ConfirmRoleDeletion() { return Driver.FindElement(By.CssSelector("input[id$='ConfirmButton']")); }
        public IWebElement CloseBtn() { return Driver.FindElement(By.CssSelector("input[id$='_CloseButton']")); }
        public IWebElement RoleAccessFiltersTextBox() { BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_PrefValue']"))); return BasePage.Driver.FindElement(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_PrefValue']")); }
        public IWebElement AddAccessFilters() { return BasePage.Driver.FindElement(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_AddButton']")); }
        public IWebElement RoleSelfStudyFilter() { return BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_SelfStudiesCheckBox")); }
        public IWebElement RoleAccessFiltersLastName() { BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_LastNameText']"))); return BasePage.Driver.FindElement(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_LastNameText']")); }
        public IWebElement RoleAccessFiltersFirstName() { BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_FirstNameText']"))); return BasePage.Driver.FindElement(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_FirstNameText']")); }
        public IWebElement RoleAccessFiltersMiddleName() { BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_MiddleNameText']"))); return BasePage.Driver.FindElement(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_MiddleNameText']")); }
        public IWebElement AccessFiltersPrefix() { BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_PrefixNameText']"))); return BasePage.Driver.FindElement(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_PrefixNameText']")); }
        public IWebElement RoleAccessFiltersSuffix() { BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_SuffixNameText']"))); return BasePage.Driver.FindElement(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_SuffixNameText']")); }
        public SelectElement AccessFiltersInformation() { return new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select[id$='_RoleAccessFilter_FilterDropDownList']"))); }
        public SelectElement SelectedFilterCriteria() { return new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select[id$='_RoleAccessFilter_SelectedFilterCriteriaListBox']"))); }
        public SelectElement ModalityFilter() { BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("select[id$='_RoleAccessFilter_ModalityListBox']"))); return new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select[id$='_RoleAccessFilter_ModalityListBox']"))); }
        public IWebElement DateLastBtn() { BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_DateRadioLast']"))); return BasePage.Driver.FindElement(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_DateRadioLast']")); }
        public IWebElement DateFromTo() { BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_DateRadioFromTo']"))); return BasePage.Driver.FindElement(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_DateRadioFromTo']")); }
        public IWebElement DateAll() { BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_DateRadioAll']"))); return BasePage.Driver.FindElement(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_DateRadioAll']")); }
        public IWebElement DateLastText() { BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_DateLastText']"))); return BasePage.Driver.FindElement(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_DateLastText']")); }
        public SelectElement DateDropDown() { BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("select[id$='RolePreferenceConfig_RoleAccessFilter_DateDropDownList']"))); return new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select[id$='RolePreferenceConfig_RoleAccessFilter_DateDropDownList']"))); }
        public IWebElement RoleDateFrom() { BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("roleDateFrom"))); return BasePage.Driver.FindElement(By.Id("roleDateFrom")); }
        public IWebElement RoleDateTo() { BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.Id("roleDateTo"))); return BasePage.Driver.FindElement(By.Id("roleDateTo")); }
        public By RoleDatePickerFrom() { return By.Id("MasterPageCalendarFrom_mainheading"); }
        public By RoleDatePickerTo() { return By.Id("MasterPageCalendarTo_mainheading"); }
        public By RoleAccessFilter_AutoCompleteDiv() { return By.CssSelector("div[id$='RolePreferenceConfig_RoleAccessFilter_AutoCompleteDiv']"); }
        public By RoleAccessFilter_AutoCompleteNameDiv() { return By.CssSelector("div[id$='RolePreferenceConfig_RoleAccessFilter_AutoCompleteNameDiv']"); }
        public SelectElement RoleAccessFilterTxtBoxDropDown() { BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("select[id$='RolePreferenceConfig_RoleAccessFilter_AutoCompleteListBox']"))); return new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select[id$='RolePreferenceConfig_RoleAccessFilter_AutoCompleteListBox']"))); }
        public SelectElement RoleAccessFilterLastNameTxtBoxDropDown() { BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("select[id$='RolePreferenceConfig_RoleAccessFilter_AutoCompleteNameListBox']"))); return new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select[id$='RolePreferenceConfig_RoleAccessFilter_AutoCompleteNameListBox']"))); }
        // public IWebElement RoleAccessFilterRemoveBtn() { return BasePage.Driver.FindElement(By.Id("ctl00_MasterContentPlaceHolder_NewRolePreferenceConfig_RoleAccessFilter_RemoveButton")); }
        public IWebElement RoleAccessFilterRemoveBtn() { return BasePage.Driver.FindElement(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_RemoveButton']")); }
        public SelectElement DomainDropDown() { return new SelectElement(Driver.FindElement(By.CssSelector("#m_listResultsControl_m_resultsSelectorControl_m_selectorList"))); }
        public IWebElement Domainnamedropdown() { return Driver.FindElement(By.CssSelector("select[id$='DomainDropDown_NameDropDownList']")); }
        public SelectElement DomainNameDropDown() { return new SelectElement(Domainnamedropdown()); }
        public SelectElement ShowRolesFromDomainDropDown() { return new SelectElement(Driver.FindElement(By.CssSelector("select[id$='m_resultsSelectorControl_m_selectorList']"))); }
        public IWebElement RoleNameTb() { return Driver.FindElement(By.CssSelector("input#m_roleSearchControl_m_input1")); }

        public IWebElement RoleNameTxt() { return Driver.FindElement(By.CssSelector("input[id$='_RoleAccessFilter_Name']")); }
        public IWebElement RoleDescriptionTxt() { return Driver.FindElement(By.CssSelector("input[id$='_RoleAccessFilter_Description']")); }
        public IWebElement SaveBtn() { return Driver.FindElement(By.CssSelector("input[id$='_SaveButton']")); }
        public IWebElement GrantAccessRadioBtn() { return Driver.FindElement(By.CssSelector("[id$='_RoleAccessFilter_GrantAccessRadioButtonList_2']")); }
        public IWebElement DataSourceListed(string ds) { return Driver.FindElement(By.CssSelector("[id$='dataSourcePathId_" + ds + "']")); }
        public IWebElement DataSourceAddBtn() { return Driver.FindElement(By.CssSelector("[id$='_RoleAccessFilter_RoleDataSourceListControl_Button_Add']")); }
        public IWebElement CloseButton() { return Driver.FindElement(By.CssSelector("div[id = 'ContainerDiv'] [id$= '_CloseButton']")); }
        public IWebElement Btn_Remove() { return Driver.FindElement(By.CssSelector("input[id$='RoleDataSourceListControl_Button_Remove']")); }
        public IWebElement Btn_Add() { return BasePage.Driver.FindElement(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_RoleDataSourceListControl_Button_Add']")); }
        public IWebElement RoleTable() { return Driver.FindElement(By.CssSelector("table[id='m_roleListControl_m_dataListGrid']")); }
        public IWebElement RoleHeading() { return Driver.FindElement(By.CssSelector("#Container_Heading>span")); }
        public By RoleTableColumn() { return By.CssSelector("tr[style]"); }
        public IWebElement RoleCount() { return Driver.FindElement(By.Id("m_listResultsControl_m_resultState")); }
        public IList<IWebElement> RoleList() { return Driver.FindElements(By.CssSelector("table[id='m_roleListControl_m_dataListGrid']>tbody>tr")); }
        public IList<IWebElement> RoleDetails() { return Driver.FindElements(By.CssSelector("table[id='m_roleListControl_m_dataListGrid']>tbody>tr>td>span")); }
        public IList<IWebElement> RoleDetailsortBy() { return Driver.FindElements(By.CssSelector("table[id='m_roleListControl_m_dataListGrid']>tbody>tr>th>span>img")); }
        public IList<IWebElement> RoleSort() { return Driver.FindElements(By.CssSelector("table[id='m_roleListControl_m_dataListGrid']>tbody>tr>th>span")); }
        public IList<IWebElement> ColumnHeadings() { return Driver.FindElements(By.CssSelector("table[id='m_roleListControl_m_dataListGrid']>tbody>tr>th")); }
        public IList<IWebElement> Labels() { return Driver.FindElements(By.CssSelector("span[id^='ctl00_MasterContentPlaceHolder']")); }
        public IWebElement Btn_ConnectApplication() { return BasePage.Driver.FindElement(By.CssSelector("input[id$='RoleAccessFilter_ConnectApplication']")); }
        public IWebElement Btn_DisconnectApplication() { return BasePage.Driver.FindElement(By.CssSelector("input[id$='RoleAccessFilter_DisconnectApplication']")); }
        //Checkboxes 
        public IWebElement ShowAllRoles() { return Driver.FindElement(By.Id("m_roleSearchControl_reset")); }
        public IWebElement ReceiveExamCB() { return Driver.FindElement(By.CssSelector("[id$='_EnableReceiveExamsCB']")); }
        public IWebElement ArchiveToPacsCB() { return Driver.FindElement(By.CssSelector("[id$='_EnableArchiveToPacsCB']")); }
        public IWebElement AllowUserToSaveStudyLayout() { BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame"); return Driver.FindElement(By.CssSelector("[id$='_AllowSaveStudyListLayoutCB']")); }
        public IWebElement UserDomainSettingsSearchFields() { BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame"); return Driver.FindElement(By.CssSelector("input[id$='ssclUseDomainCB']")); }
        public By By_ConferenceUserCB() { return By.CssSelector("[id$='_ConferenceUserCB']"); }
        public IWebElement ConferenceUserCB() { return Driver.FindElement(By_ConferenceUserCB()); }
        public IWebElement UseDomainSetting() { BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame"); return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_ViewingProcotocolsConfigUseDomainLayoutCheckbox")); }
        public By allowUniversalViewer() { return By.CssSelector("[id$='_EnableUniversalViewerCB']"); }
        public IWebElement AllowUniversalViewer() { return Driver.FindElement(allowUniversalViewer()); }
        public By defaultUniversalViewer() { return By.CssSelector("[id$='RoleAccessFilter_DefaultViewerRadioButtonList_0']"); }
        public IWebElement DefaultUniversalViewer() { return Driver.FindElement(defaultUniversalViewer()); }
        public By defaultEnterpriseViewer() { return By.CssSelector("[id$='RoleAccessFilter_DefaultViewerRadioButtonList_1']"); }
        public IWebElement DefaultEnterpriseViewer() { return Driver.FindElement(defaultEnterpriseViewer()); }
        //Dropdown--
        //Drop down List
        public SelectElement LayoutDropDown() { return new SelectElement(Driver.FindElement(By.CssSelector("select[id$='DropDownListLayout']"))); }
        public SelectElement ModalityDropDown() { return new SelectElement(Driver.FindElement(By.CssSelector("select[id$='DropDownListModalities']"))); }

        //(New/Edit) Role page
        //Check box -UseDomainSetting 
        public IWebElement StudySearchFieldUseDomainSetting_CB() { return Driver.FindElement(By.CssSelector("[id$='m_sslConfigControl_ssclUseDomainCB']")); }
        public IWebElement iPadStudyListUseDomainSetting_CB() { return Driver.FindElement(By.CssSelector("[id$='m_ipadStudyListLayoutConfig_isclUseDomainCB']")); }
        public IWebElement ToolbarConfigurationUseDomainSetting_CB() { return Driver.FindElement(By.CssSelector("[id$='RoleToolbarConfig_UseDomainToolbarCheckbox']")); }
        public IWebElement PatientHistoryLayoutUseDomainSetting_CB() { return Driver.FindElement(By.CssSelector("[id$='m_studyGrid_1_StudyGridConfigUseDomainLayoutCheckbox']")); }
        public IWebElement StudyListlayoutUseDomainSetting_CB() { return Driver.FindElement(By.CssSelector("[id$='m_studyGrid_2_StudyGridConfigUseDomainLayoutCheckbox']")); }
        public IWebElement DefaultSettingPerModalityUseDomainSetting_CB() { return Driver.FindElement(By.CssSelector("[id$='ViewingProcotocolsConfigUseDomainLayoutCheckbox']")); }
        public IWebElement AutoCINEON_RB() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_m_viewingProtocols_AutoStartCineRadioButtons_0")); }
        public IWebElement AutoCINEOFF_RB() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_m_viewingProtocols_AutoStartCineRadioButtons_1")); }
        public IWebElement SetcardioDefaults() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_m_viewingProtocols_SetCardioDefaultsButton")); }
        public IWebElement CnfrmButton() { return Driver.FindElement(By.CssSelector("#ctl00_ConfirmButton")); }
        public IWebElement SetcardioDefaults_Warning() { return Driver.FindElement(By.CssSelector("#ctl00_ConfirmationText")); }
        public IWebElement GrantAccessRadioBtn_Disabled() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_GrantAccessRadioButtonList_0")); }
        public IWebElement GrantAccessRadioBtn_Anyone() { return Driver.FindElement(By.CssSelector("[id$='_RoleAccessFilter_GrantAccessRadioButtonList_2']")); }
        public IWebElement GrantAccessRadioBtn_Grouponly() { return Driver.FindElement(By.CssSelector("[id$='_RoleAccessFilter_GrantAccessRadioButtonList_1']")); }
        public IWebElement StudySearchFieldsDiv() { return Driver.FindElement(By.CssSelector("div[id='ssclMainDiv']")); }
        public IWebElement iPadStudyListFieldsDiv() { return Driver.FindElement(By.CssSelector("div[id='isclMainDiv']")); }
        public IWebElement ExamModeON() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_m_viewingProtocols_ExamModeRadioButtons_0")); }
        public IWebElement ExamModeOFF() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_m_viewingProtocols_ExamModeRadioButtons_1")); }
        public IWebElement CardioOrder() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_m_viewingProtocols_CardioOrderCheckBox")); }
        public IWebElement ExamMode(string mode) { return Driver.FindElement(By.CssSelector("input[id$='_ExamModeRadioButtons_" + mode + "']")); }

        //NewRole Page Heading
        public IWebElement RoleManagemantTitle() { return Driver.FindElement(By.CssSelector("div[id='Container_Heading'] a")); }
        public IWebElement SubHeading() { return Driver.FindElement(By.CssSelector("div[id='Container_Heading'] span")); }
       

        //Labels
        public IWebElement DomainInformation() { return Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_DomainDropDown_TitleLabel")); }
        public IWebElement RoleInformationNewRole() { return Driver.FindElement(By.CssSelector("[id$='_NewRolePreferenceConfig_RoleAccessFilter_RoleInfoTitleLabel']")); }
        public IWebElement RoleInformationEditRole() { return Driver.FindElement(By.CssSelector("[id$='_EditRolePreferenceConfig_RoleAccessFilter_RoleInfoTitleLabel")); }
        public IWebElement DataSources() { return Driver.FindElement(By.CssSelector("[id$='_RoleAccessFilter_DataSourceTitleLabel']")); }
        public IWebElement ExternalApplications() { return Driver.FindElement(By.CssSelector("[id$='_RoleAccessFilter_ApplicationTitleLabel")); }
        public IWebElement AccessFilter() { return Driver.FindElement(By.CssSelector("[id$='_RoleAccessFilter_AccessFilterTitleLabel")); }
        public IWebElement ToolBarConfig() { return Driver.FindElement(By.CssSelector("[id$='_RoleToolbarConfig_DomainToolbarConfig")); }
        public IWebElement PatientHistoryLayout() { return Driver.FindElement(By.CssSelector("span[id$='_m_studyGrid_1_StudyGridConfigLabel']")); }
        public IWebElement StudyListLayout() { return Driver.FindElement(By.CssSelector("span[id$='_m_studyGrid_2_StudyGridConfigLabel']")); }

        //Toolbar Configuration
        public IWebElement UseDomainSetting_toolbar() { return Driver.FindElement(By.CssSelector("input[id$='UseDomainToolbarCheckbox']")); }

        //Default Settings per modality
        public IWebElement UseDomainSetting_modality() { return Driver.FindElement(By.CssSelector("input[id$='ViewingProcotocolsConfigUseDomainLayoutCheckbox']")); }

        //Dropdown
        public SelectElement DomainSelector() { return new SelectElement(Driver.FindElement(By.CssSelector("#m_listResultsControl_m_resultsSelectorControl_m_selectorList"))); }

        // Access Filter
        public SelectElement AccessFilterBox() { return new SelectElement(Driver.FindElement(By.CssSelector("[id$='RoleAccessFilter_SelectedFilterCriteriaListBox']"))); }
        public IWebElement RemoveButton() { return Driver.FindElement(By.Id("ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_RemoveButton")); }
        public IWebElement UseAllDataSource() {return Driver.FindElement(By.CssSelector("input[id$='RolePreferenceConfig_RoleAccessFilter_UseAllDataSourcesCB']")); }
        public IList<IWebElement> List_ConnectDatasource() { return Driver.FindElements(By.CssSelector("div[id$='RolePreferenceConfig_RoleAccessFilter_RoleDataSourceListControl_ListDiv'] div.hierarchyList div[class$='collapsed'] div[id*='dataSourcePathId']")); }
        public IList<IWebElement> List_ConnectedDatasource() { return Driver.FindElements(By.CssSelector("div[id$='RolePreferenceConfig_RoleAccessFilter_RoleDataSourceListControl_selectedListDIV'] span")); }
        public IList<IWebElement> List_FilterDatasource() { return Driver.FindElements(By.CssSelector("div[id$='RolePreferenceConfig_RoleAccessFilter_FilterDataSourceListControl'] div[id^='dataSourcePathId_']")); }
        public IWebElement AccessFilterElement(int elementno) { return Driver.FindElement(By.XPath("//*[@id='ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_SelectedFilterCriteriaListBox']/option[" + elementno + "]")); }
        public IWebElement MeaningfulUse() { return Driver.FindElement(By.CssSelector("input[id$='_EnableMUCB']")); }

        //Error Message
        public IWebElement ErrorMessage() { return Driver.FindElement(By.CssSelector("[id$='_ErrorMessage']")); }
        public IWebElement SelfStudiesFilterCB() { return Driver.FindElement(By.CssSelector("[id$='SelfStudiesCheckBox']")); }

        // Viewer Image radio buttons
        public IWebElement ViewerImage() { return Driver.FindElement(By.CssSelector("span[title = 'Select View Scope'] input[value = 'Image']")); }
        public IWebElement ViewerSeries() { return Driver.FindElement(By.CssSelector("span[title = 'Select View Scope'] input[value = 'Series']")); }

        public static string checkbox_enable3DView = "input[id$=ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_Enable3DViewCB]";

        /// <summary>
        ///     This function Creates a role
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="roleName">The value to be inputted in the Role name field</param>
        /// <param name="RoleCred"></param>
        public void CreateRole(string domainName, string roleName, int RoleCred = 0, int ConfUsr = 0, int DataTransfer = 0, int GrantAccess = 1)
        {
            PageLoadWait.WaitForFrameLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
            //Driver.SwitchTo().DefaultContent();
            //Driver.SwitchTo().Frame("UserHomeFrame");
            SelectDomainfromDropDown(domainName);
            ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_NewRolePreferenceConfig_RoleAccessFilter_Name");
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_NewRolePreferenceConfig_RoleAccessFilter_Name", roleName);
            Thread.Sleep(2000);
            ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_NewRolePreferenceConfig_RoleAccessFilter_Name");
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_NewRolePreferenceConfig_RoleAccessFilter_Name", roleName);
            //ClearText("cssselector", "#ctl00_MasterContentPlaceHolder_NewRolePreferenceConfig_RoleAccessFilter_Description");
            SetText("cssselector", "#ctl00_MasterContentPlaceHolder_NewRolePreferenceConfig_RoleAccessFilter_Description", roleName + " Description");
            Thread.Sleep(2000);

            PageLoadWait.WaitForPageLoad(20);
            if (RoleCred == 0)
            {
                UnCheckCheckbox("cssselector", "#ctl00_MasterContentPlaceHolder_NewRolePreferenceConfig_RoleAccessFilter_EnableReceiveExamsCB");
                UnCheckCheckbox("cssselector", "#ctl00_MasterContentPlaceHolder_NewRolePreferenceConfig_RoleAccessFilter_EnableArchiveToPacsCB");
            }
            else if (RoleCred == 1)
            {
                try
                {
                    PageLoadWait.WaitForElement(By.CssSelector("[id$='_EnableReceiveExamsCB']"), WaitTypes.Visible);
                    SetCheckbox("cssselector", "[id$='_EnableReceiveExamsCB']", 1);
                }
                catch (Exception e)
                {
                    Logger.Instance.ErrorLog("Exception while enabling Receive exam using jscript" + e);
                    Logger.Instance.InfoLog("Trying out using selenium");
                    PageLoadWait.WaitForPageLoad(20);
                    SetCheckbox("cssselector", "[id$='_EnableReceiveExamsCB']");
                }
                UnCheckCheckbox("cssselector", "#ctl00_MasterContentPlaceHolder_NewRolePreferenceConfig_RoleAccessFilter_EnableArchiveToPacsCB");
            }
            else if (RoleCred == 2)
            {
                UnCheckCheckbox("cssselector", "#ctl00_MasterContentPlaceHolder_NewRolePreferenceConfig_RoleAccessFilter_EnableReceiveExamsCB");
                SetCheckbox("cssselector", "#ctl00_MasterContentPlaceHolder_NewRolePreferenceConfig_RoleAccessFilter_EnableArchiveToPacsCB");
            }
            else if (RoleCred == 3)
            {
                SetCheckbox("cssselector", "#ctl00_MasterContentPlaceHolder_NewRolePreferenceConfig_RoleAccessFilter_EnableReceiveExamsCB");
                SetCheckbox("cssselector", "#ctl00_MasterContentPlaceHolder_NewRolePreferenceConfig_RoleAccessFilter_EnableArchiveToPacsCB");
            }

            if (ConfUsr != 0)
            {
                SetCheckbox(Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_NewRolePreferenceConfig_RoleAccessFilter_ConferenceUserCB")));
            }
            if (DataTransfer != 0)
            {
                SetCheckbox(Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_NewRolePreferenceConfig_RoleAccessFilter_AllowTransferCB")));
            }
            if(GrantAccess==1)
            {
                //Enable Grant Access
                SetRadioButton("cssselector", "#ctl00_MasterContentPlaceHolder_NewRolePreferenceConfig_RoleAccessFilter_GrantAccessRadioButtonList_2");
            }

            //Set Viewer Type
            SetViewerTypeInNewRole();

            Click("cssselector", "#ctl00_MasterContentPlaceHolder_SaveButton");
            Click("cssselector", "#ctl00_MasterContentPlaceHolder_SaveButton");
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
        }

        /// <summary>
        /// This will create a role with any privilage for a specicfic domain
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="roleName"></param>
        /// <param name="roletype">If mutiple role type is needed then concatinate multiple roles with "="</param>
        /// <param name="filterdatasource">The data sources need to be removed has to be concatinated with "="</param>
        public void CreateRole(String domainName, String roleName, String roletype, String datasourcelist = null, bool domainadmin = false)
        {
            PageLoadWait.WaitForFrameLoad(10);
            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#NewRoleButton")));
            }
            catch (Exception)
            {
                new Login().Navigate("Role Management");
                PageLoadWait.WaitForFrameLoad(10);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#NewRoleButton")));
            }
            ClickButton("#NewRoleButton");
            PageLoadWait.WaitForFrameLoad(30);

            if (!domainadmin)
            {
                //Select Domain
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("select[id*='_DomainDropDown_']")));
                this.SelectFromList(BasePage.Driver.FindElement(By.CssSelector("select[id*='_DomainDropDown_']")), domainName);
                String domain = "select[id*='_DomainDropDown_']>option[value='" + domainName + "']";
                BasePage.Driver.FindElement(By.CssSelector(domain)).Click();
                //SelectDomainfromDropDown(domainName);
            }

            //Enter Role name and Description  
            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("select[id$='ChooseCopyRoleDropDownList']")));
            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='_NewRolePreferenceConfig_RoleAccessFilter_Name']"))).Click();
            int counter = 0;
            while (!BasePage.Driver.FindElement(By.CssSelector("[id$='_NewRolePreferenceConfig_RoleAccessFilter_Name']")).Text.Equals(roleName) && counter++ <= 3)
            {
                //SetText("cssselector", "[id$='_NewRolePreferenceConfig_RoleAccessFilter_Name']", roleName);
                RoleNameTxt().Clear();
                RoleNameTxt().SendKeys(roleName);
                Thread.Sleep(3000);
            }
            counter = 0;
            while (!BasePage.Driver.FindElement(By.CssSelector("[id$='_NewRolePreferenceConfig_RoleAccessFilter_Description']")).Text.Equals(roleName) && counter++ <= 3)
            {
                //SetText("cssselector", "[id$='_NewRolePreferenceConfig_RoleAccessFilter_Description']", roleName);
                RoleDescriptionTxt().Clear();
                RoleDescriptionTxt().SendKeys(roleName);
                Thread.Sleep(3000);
            }

            //Set previlage
            foreach (String role in roletype.Split('='))
            {
                this.SetPrivelage(role);
            }

            //Set Viewer Type            
            if (!this.getiCAVersion().Contains("6.5"))
            {
                SetViewerTypeInNewRole();
            }

            //Connect DataSource
            if (!String.IsNullOrEmpty(datasourcelist))
            {
                this.ConnectDataSource(datasourcelist);
            }

            //Save the Transaction
            try
            {
                if (!GrantAccessRadioBtn_Anyone().Selected)
                {
                    GrantAccessRadioBtn_Anyone().Click();
                }
            }
            catch (Exception) { }
            Click("cssselector", "[id$='_MasterContentPlaceHolder_SaveButton']", true);
            PageLoadWait.WaitForFrameToBeVisible(15);
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(20);
            PageLoadWait.WaitForLoadingMessage(10);
        }

        /// <summary>
        /// This method will set the access as per the Role Typ
        /// </summary>
        /// <param name="roletype"></param>
        public void SetPrivelage(String roletype)
        {
            String receiveexam = "input[id$='_EnableReceiveExamsCB']";
            String archiveeexam = "input[id$='_EnableArchiveToPacsCB']";
            String emailstudy = "input[id$='AllowEmailCB']";

            if (String.IsNullOrEmpty(roletype)) { return; }

            if (roletype.ToLower().Equals("physician"))
            {
                SetCheckbox("cssselector", receiveexam);
                UnCheckCheckbox("cssselector", archiveeexam);
            }
            else if (roletype.ToLower().Equals("archivist"))
            {
                UnCheckCheckbox("cssselector", receiveexam);
                SetCheckbox("cssselector", archiveeexam);
            }
            else if (roletype.ToLower().Equals("both"))
            {
                SetCheckbox("cssselector", receiveexam);
                SetCheckbox("cssselector", archiveeexam);
            }
            else if (roletype.ToLower().Equals("conference"))
            {
                this.SetCheckbox("cssselector", "input[id$='ConferenceUserCB']");
                this.SetCheckbox("cssselector", "input[id$='ConferenceUserCB']");
            }
            else if(roletype.ToLower().Contains("email"))
            {
                this.SetCheckbox("cssselector", emailstudy);
            }
            else
            {
                UnCheckCheckbox("cssselector", receiveexam);
                UnCheckCheckbox("cssselector", archiveeexam);
            }
        }

        /// <summary>
        /// This function selects the given role in the role management tab when logged in as Admin
        /// </summary>
        /// <param name="domain"></param>
        public void SelectRole(String role)
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div.row tr td>span[title='" + role + "']")));
            IList<IWebElement> d = Driver.FindElements(By.CssSelector("div.row tr td>span[title='" + role + "']"));
            foreach (IWebElement elm in d)
            {
                String s = elm.Text;
                if (s.Equals(role))
                {
                    // elm.Click();
                    ClickElement(elm);
                    Logger.Instance.InfoLog(role + "role is selected");
                    break;
                }
             
            }

            PageLoadWait.WaitForFrameLoad(20);
        }

        public void SelectDomainfromDropDown(string domainName)
        {
            IList<IWebElement> domains = new List<IWebElement>();
            try
            {
                PageLoadWait.WaitForFrameLoad(20);
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("select#m_listResultsControl_m_resultsSelectorControl_m_selectorList")));
                domains = Driver.FindElements(By.CssSelector("select#m_listResultsControl_m_resultsSelectorControl_m_selectorList>option[value='" + domainName + "']"));
            }
            catch (Exception)
            {
                Driver.SwitchTo().DefaultContent();
                Driver.SwitchTo().Frame("UserHomeFrame");
                wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("select#ctl00_MasterContentPlaceHolder_DomainDropDown_NameDropDownList")));
                domains = Driver.FindElements(By.CssSelector("select#ctl00_MasterContentPlaceHolder_DomainDropDown_NameDropDownList>option[value='" + domainName + "']"));
            }
            foreach (IWebElement elm in domains)
            {
                String s = elm.Text;
                if (s.Equals(domainName))
                {
                    elm.Click();
                    Logger.Instance.InfoLog(domainName + "domain is selected");
                    break;
                }
            }

            Logger.Instance.InfoLog("Domain " + domainName + " successfully selected in User Manangement");
        }

        /// <summary>
        ///     This function selects the specified RoleName from the grid
        /// </summary>
        /// <param name="roleName">The Role name to be selected</param>
        public void EditRoleByName(string roleName)
        {
            Click("cssselector", "span[title='" + roleName + "']");
            PageLoadWait.WaitForPageLoad(2);
            Click("id", "EditRoleButton");
            PageLoadWait.WaitForPageLoad(2);
        }

        /// <summary>
        ///     This function clicks on the Edit Role button
        /// </summary>
        public void ClickEditRole()
        {
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer"))           
                this.ClickElement(BasePage.Driver.FindElement(By.Id("EditRoleButton")));
            else
                Click("id", "EditRoleButton");
            PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
            PageLoadWait.WaitForFrameLoad(5);
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id$='RoleAccessFilter_Name']")));
            //wait.Until(ExpectedConditions.ElementExists(By.CssSelector("select[id$='_ChooseCopyRoleDropDownList']>option")));
            //wait.Until(ExpectedConditions.ElementExists(By.CssSelector("div[id$='_RoleDataSourceListControl']>div")));
        }

        /// <summary>
        /// This function clicks the save button in Edit role 
        /// </summary>
        public void ClickSaveEditRole()
        {
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            if (Driver.FindElement(By.CssSelector("[id$='_SaveButton']")).Enabled == true)
            {
                try
                {   if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer"))
                    Driver.FindElement(By.CssSelector("[id$='_SaveButton']")).Click();
                    else
                    ClickElement(Driver.FindElement(By.CssSelector("[id$='_SaveButton']")));
                }
                catch (Exception) 
                {
                    Click("cssselector", "[id$='_SaveButton']"); 
                }                
                
                PageLoadWait.WaitForFrameToBeVisible(15);
                PageLoadWait.WaitForPageLoad(10);
                Logger.Instance.InfoLog("Save button in Edit role is clicked");
                Driver.SwitchTo().DefaultContent();
                BasePage.wait.Until<Boolean>(driver =>
                {
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                    try
                    {
                        if (driver.FindElement(By.CssSelector("input[id$='_SaveButton']")) != null)
                            return false;
                        else
                            return true;
                    }
                    catch(Exception)
                    {
                        return true;
                    }

                });
                PageLoadWait.WaitForFrameLoad(5);
                BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector("input[id='NewRoleButton']")));

            }
            else
            {
                Logger.Instance.ErrorLog("Save Button is not found in Edit role");
            }
        }

        public Boolean RoleExists(string roleName, string domain = null)
        {
            SearchRole(roleName, domain);
            try
            {
                IList<IWebElement> elements =
                    Driver.FindElements(
                        By.CssSelector("table[id='m_roleListControl_m_dataListGrid']>tbody>tr"));

                for (int i = 1; i < elements.Count + 1; i++)
                {
                    if (
                        elements[i - 1].FindElement(By.XPath("//tr[" + i + "]/td[1]/span"))
                                       .GetAttribute("title")
                                       .Equals(roleName))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Logger.Instance.InfoLog("Role does not exist");
                return false;
            }
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
        /// This method is to close the Role management screen
        /// </summary>
        public void CloseRoleManagement()
        {
            PageLoadWait.WaitForFrameLoad(5);
            BasePage.Driver.FindElement(By.CssSelector("input[name$='CloseButton'][name*='MasterContent']")).Click();
            PageLoadWait.WaitForHPPageLoad(10);
        }

        /// <summary>
        /// This method is to move elements to the Available item section
        /// </summary>
        /// <param name="tools"></param>
        public new void MoveToolsToAvailableSection(IWebElement[] tools)
        {
            base.MoveToolsToAvailableSection(tools);
        }

        /// <summary>
        /// It sets the Checkbox in either new/edit role
        /// </summary>
        /// <param name="field"></param>
        /// <param name="set"></param>
        public void SetCheckboxInEditRole(String field, int set)
        {
            //wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#ContainerDiv")));
            String FieldName = field.ToLower();
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame");
            switch (FieldName)
            {
                case "toolbar":
                    string tool = "[id$='_UseDomainToolbarCheckbox']";
                    if (set == 0)
                    {
                        this.SetCheckbox("CssSelector", tool);
                    }
                    else
                    {
                        this.UnCheckCheckbox("CssSelector", tool);
                    }
                    break;

                case "layout":
                    string layout = "[id$='UseDomainLayoutCheckbox']";
                    if (set == 0)
                    {
                        this.SetCheckbox("CssSelector", layout);
                    }
                    else
                    {
                        this.UnCheckCheckbox("CssSelector", layout);
                    }
                    break;

                case "emergency":
                    string emergency = "[id$='_AllowEmergencyAccessCB']";
                    if (set == 0)
                    {
                        this.SetCheckbox("CssSelector", emergency);
                    }
                    else
                    {
                        this.UnCheckCheckbox("CssSelector", emergency);
                    }
                    break;

                case "download":
                    string Download = "[id$='_AllowDownloadCB']";
                    if (set == 0)
                    {
                        this.SetCheckbox("CssSelector", Download);
                    }
                    else
                    {
                        this.UnCheckCheckbox("CssSelector", Download);
                    }
                    break;

                case "transfer":
                    string Transfer = "[id$='_AllowTransferCB']";
                    if (set == 0)
                    {
                        this.SetCheckbox("CssSelector", Transfer);
                    }
                    else
                    {
                        this.UnCheckCheckbox("CssSelector", Transfer);
                    }
                    break;

                case "pdfreport":
                    string pdfreport = "[id$='_EnablePdfReportCB']";
                    if (set == 0)
                    {
                        this.SetCheckbox("CssSelector", pdfreport);
                    }
                    else
                    {
                        this.UnCheckCheckbox("CssSelector", pdfreport);
                    }
                    break;
                case "receiveexam":
                    string exam = "[id$='_EnableReceiveExamsCB']";
                    if (set == 0)
                    {
                        this.SetCheckbox("CssSelector", exam);
                    }
                    else
                    {
                        this.UnCheckCheckbox("CssSelector", exam);
                    }
                    break;
                case "archive":
                    string archive = "[id$='_EnableArchiveToPacsCB']";
                    if (set == 0)
                    {
                        this.SetCheckbox("CssSelector", archive);
                    }
                    else
                    {
                        this.UnCheckCheckbox("CssSelector", archive);
                    }
                    break;
                case "email":
                    string email = "[id$='_RoleAccessFilter_AllowEmailCB']";
                    if (set == 0)
                    {
                        this.SetCheckbox("CssSelector", email);
                    }
                    else
                    {
                        this.UnCheckCheckbox("CssSelector", email);
                    }
                    break;

                case "conferenceuser":
                    string conference = "[id$='_ConferenceUserCB']";
                    if (set == 0)
                    {
                        this.SetCheckbox("CssSelector", conference);
                    }
                    else
                    {
                        this.UnCheckCheckbox("CssSelector", conference);
                    }
                    break;

                case "viewingprotocol":
                    string protocol = "[id$='ViewingProcotocolsConfigUseDomainLayoutCheckbox']";
                    if (set == 0)
                    {
                        this.SetCheckbox("CssSelector", protocol);
                    }
                    else
                    {
                        this.UnCheckCheckbox("CssSelector", protocol);
                    }
                    break;

                case "studylist":
                    string studylist = "[id$='m_studyGrid_2_StudyGridConfigUseDomainLayoutCheckbox']";
                    if (set == 0)
                    {
                        this.SetCheckbox("CssSelector", studylist);
                    }
                    else
                    {
                        this.UnCheckCheckbox("CssSelector", studylist);
                    }
                    break;

                    case "universalviewer":
                    string viewer = "[id$='EnableUniversalViewerCB'][id*='Role']";
                    if (set == 0)
                    {
                        this.SetCheckbox("CssSelector", viewer);
                    }
                    else
                    {
                        this.UnCheckCheckbox("CssSelector", viewer);
                    }
                    break;

                case "3dview":
                    string _3dviewer = "[id$='_Enable3DViewCB'][id*='Role']";
                    if (set == 0)
                    {
                        this.SetCheckbox("CssSelector", _3dviewer);
                    }
                    else
                    {
                        this.UnCheckCheckbox("CssSelector", _3dviewer);
                    }
                    break;

                case "invitetoupload":
                    string invitetoupload = "[id$='_RoleAccessFilter_AllowInviteToUploadCB']";
                    if (set == 0)
                    {
                        this.SetCheckbox("CssSelector", invitetoupload);
                    }
                    else
                    {
                        this.UnCheckCheckbox("CssSelector", invitetoupload);
                    }
                    break;
                    //**Still can include for other checkboxes also**

            }
        }

        /// <summary>
        /// Setting modality for a Role
        /// </summary>
        /// <param name="Filter"></param>
        public void RoleFilter_Modality(String Filter)
        {
            BasePage.Driver.SwitchTo().DefaultContent();
            BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
            IWebElement Modality = BasePage.Driver.FindElement(By.CssSelector("select[id$='_RoleAccessFilter_FilterDropDownList']"));
            SelectFromList(Modality, "Modality", 1);
            IWebElement Modalitylist = BasePage.Driver.FindElement(By.CssSelector("select[id$='_RoleAccessFilter_ModalityListBox']"));
            Modalitylist.Click();
            SelectFromList(Modality, "Modality", 1);
            IWebElement CR = Modalitylist.FindElement(By.CssSelector("option[value='" + Filter + "']"));
            CR.Click();

            IWebElement addbtn = BasePage.Driver.FindElement(By.CssSelector("input[id$='_RoleAccessFilter_AddButton']"));
            addbtn.Click();
        }

        /// <summary>
        /// Setting Study date filter in Role
        /// </summary>
        /// <param name="lastDate"></param>
        /// <param name="date"></param>
        public void AddStudyDateinRoleFilter(string lastDate, string date)
        {
            PageLoadWait.WaitForElement(By.Id(Locators.ID.DomainMgmtFilter), BasePage.WaitTypes.Visible);
            SelectFromList("id", Locators.ID.DomainMgmtFilter, "Study Date");
            SetText("id", "ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_DateLastText", lastDate);
            SelectFromList("id", "ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_DateDropDownList", date);
            Click("id", "ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_AddButton");
            Logger.Instance.InfoLog("Study Date added in Role Filter successfully");

        }

        /// <summary>
        /// Setting Referring Physician filter in Role
        /// </summary>
        /// <param name="refPhysicianName"></param>
        public void AddFilterinRole(string FilterName, string FilterValue)
        {
            switch (FilterName)
            {
                case "Referring Physician":
                    {
                        PageLoadWait.WaitForElement(By.Id(Locators.ID.DomainMgmtFilter), BasePage.WaitTypes.Visible);
                        SelectFromList("id", Locators.ID.DomainMgmtFilter, "Referring Physician");
                        SetText("id", Locators.ID.DomainMgmtReferPhysFilterText, FilterValue);
                        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_AutoCompleteNameListBox")));
                        DoubleClick(GetElement("id", "ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_AutoCompleteNameListBox"));
                        Click("id", Locators.ID.DomainMgmtFilterAddButton,true);
                        Logger.Instance.InfoLog("Referring Physician added in Role Filter successfully");
                        break;
                    }

                case "Accession Number":
                    {
                        PageLoadWait.WaitForElement(By.Id(Locators.ID.DomainMgmtFilter), BasePage.WaitTypes.Visible);
                        SelectFromList("id", Locators.ID.DomainMgmtFilter, "Accession Number");
                        SetText("id", Locators.ID.DomainMgmtFilterText, FilterValue);
                        PageLoadWait.WaitForPageLoad(3);
                        Click("id", Locators.ID.DomainMgmtFilterAddButton, true);
                        PageLoadWait.WaitForPageLoad(3);
                        Logger.Instance.InfoLog("Accession Number added in Role Filter successfully");
                        break;
                    }
                case "Patient ID":
                    {
                        PageLoadWait.WaitForElement(By.Id(Locators.ID.DomainMgmtFilter), BasePage.WaitTypes.Visible);
                        SelectFromList("id", Locators.ID.DomainMgmtFilter, "Patient ID");
                        SetText("id", Locators.ID.DomainMgmtFilterText, FilterValue);
                        Click("id", Locators.ID.DomainMgmtFilterAddButton,true);
                        PageLoadWait.WaitForPageLoad(3);
                        Logger.Instance.InfoLog("Patient ID added in Role Filter successfully");
                        break;
                    }
                case "Patient Name":
                    {
                        PageLoadWait.WaitForElement(By.Id(Locators.ID.DomainMgmtFilter), BasePage.WaitTypes.Visible);
                        SelectFromList("id", Locators.ID.DomainMgmtFilter, "Patient Name");
                        SetText("id", Locators.ID.DomainMgmtReferPhysFilterText, FilterValue);
                        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_AutoCompleteNameListBox")));
                        DoubleClick(GetElement("id", "ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_AutoCompleteNameListBox"));
                        Click("id", Locators.ID.DomainMgmtFilterAddButton,true);
                        Logger.Instance.InfoLog("Patient Name added in Role Filter successfully");
                        break;
                    }

                case "IPID":
                    {
                        PageLoadWait.WaitForElement(By.Id(Locators.ID.DomainMgmtFilter), BasePage.WaitTypes.Visible);
                        SelectFromList("id", Locators.ID.DomainMgmtFilter, "Issuer of Patient ID");
                        SetText("id", Locators.ID.DomainMgmtFilterText, FilterValue);
                        Click("id", Locators.ID.DomainMgmtFilterAddButton,true);
                        PageLoadWait.WaitForPageLoad(3);
                        Logger.Instance.InfoLog("Issuer of Patient ID added in Role Filter successfully");
                        break;
                    }

                case "Reading Physician":
                    {
                        String[] ReadingPhyvalues = null;
                        PageLoadWait.WaitForElement(By.Id(Locators.ID.DomainMgmtFilter), BasePage.WaitTypes.Visible);
                        SelectFromList("id", Locators.ID.DomainMgmtFilter, "Reading Physician");
                        if (FilterValue.Contains(":"))
                        {
                            ReadingPhyvalues = FilterValue.Split(':');
                            SetText("id", Locators.ID.DomainMgmtReferPhysFilterText, ReadingPhyvalues[0]);
                            SetText("id", "ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_LastNameText", ReadingPhyvalues[1]);
                        }
                        else
                            SetText("id", Locators.ID.DomainMgmtReferPhysFilterText, FilterValue);
                        wait.Until(ExpectedConditions.ElementIsVisible(By.Id("ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_AutoCompleteNameListBox")));
                        DoubleClick(GetElement("id", "ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_AutoCompleteNameListBox"));

                        Click("id", Locators.ID.DomainMgmtFilterAddButton,true);
                        Logger.Instance.InfoLog("Referring Physician added in Role Filter successfully");
                        break;
                    }

                default:
                    break;
            }
        }

        public void RemoveAccessFilter()
        {
            PageLoadWait.WaitForElement(By.Id(Locators.ID.DomainMgmtFilter), BasePage.WaitTypes.Visible);
            SelectElement eles = AccessFilterBox();
            int total = eles.Options.Count;
            for (int i = 0; i < total; i++)
            {
                eles.SelectByIndex(0);
                RemoveButton().Click();
            }
        }

        /// <summary>
        ///     This function clicks on the Edit Role button
        /// </summary>
        public void ClickButtonInRole(String button = "edit")
        {
            switch (button)
            {
                case "edit":
                    Click("id", "EditRoleButton");
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchToDefault();
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchTo("index", "0");
                    break;

                case "new":
                    Click("id", "NewRoleButton");
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchToDefault();
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchTo("index", "0");
                    break;

                case "delete":
                    Click("id", "DeleteRoleButton");
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchToDefault();
                    PageLoadWait.WaitForPageLoad(10);
                    SwitchTo("index", "0");
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#ctl00_ConfirmButton")));
                    Click("id", "ctl00_ConfirmButton");
                    PageLoadWait.WaitForPageLoad(10);
                    PageLoadWait.WaitForFrameLoad(10);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// This function clicks the save button in Edit role 
        /// </summary>
        public void ClickSaveRole()
        {
            Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            if (Driver.FindElement(By.CssSelector("[id$='_SaveButton']")).Enabled == true)
            {
                Driver.FindElement(By.CssSelector("[id$='_SaveButton']")).Click();
                PageLoadWait.WaitForFrameLoad(30);
                Logger.Instance.InfoLog("Save button in Edit role is clicked");
                Driver.SwitchTo().DefaultContent();
                PageLoadWait.WaitForFrameLoad(15);
                //Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            }
            else
            {
                Logger.Instance.ErrorLog("Save Button is not found in Edit role");
            }
        }

        /// <summary>
        /// This function searches the given role
        /// </summary>
        /// <param name="domain"></param>
        public void SearchRole(String RoleName, String Domainname = null)
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame("UserHomeFrame").SwitchTo().Frame("TabContent").SwitchTo().Frame("TabContent");
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input#m_roleSearchControl_m_input1")));
            if (Domainname != null)
            {
                DomainSelector().SelectByText(Domainname);
                PageLoadWait.WaitForFrameLoad(20);
            }
            BasePage.Driver.FindElement(By.CssSelector("input#m_roleSearchControl_m_input1")).Clear();
            BasePage.Driver.FindElement(By.CssSelector("input#m_roleSearchControl_m_input1")).SendKeys(RoleName);
            BasePage.Driver.FindElement(By.CssSelector("input#m_roleSearchControl_m_searchButton")).Click();

            //PageLoadWait.WaitForPageLoad(20);
            PageLoadWait.WaitForFrameLoad(20);
        }

        /// <summary>
        /// This function Clicks New Role Btn
        /// </summary>
        public void ClickNewRoleBtn()
        {
            try
            {
                SwitchToDefault();
                SwitchTo("index", "0");
                SwitchTo("index", "1");
                SwitchTo("index", "0");
                Click("id", "NewRoleButton");
                Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in Step ClickClearBtn due to " + ex.Message);
            }
        }

        /// <summary>
        /// This will add the given datasource for the role in Edit Role Page
        /// </summary>
        /// <param name="ds"></param>
        public void AddDatasourceToRole(string ds)
        {

            PageLoadWait.WaitForFrameLoad(10);
            UnCheckCheckbox("cssselector", "[id$='_RoleAccessFilter_UseAllDataSourcesCB']");
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id*='dataSourcePathId_']")));
            PageLoadWait.WaitForFrameLoad(10);
            if (DataSourceListed(ds).Displayed)
            {
                DataSourceListed(ds).Click();
                DataSourceAddBtn().Click();

            }
            else
            {
                throw new Exception("Data source " + ds + "is not listed");
            }

        }

        public void SwitchToRoleMgmtFrame()
        {
            SwitchTo("index", "0");
            SwitchTo("index", "1");
            SwitchTo("index", "0");
        }
        /// <summary>
        /// This method is to click the close button in role management page
        /// </summary>
        public void ClickCloseButton()
        {
            PageLoadWait.WaitForFrameLoad(10);
            this.CloseButton().Click();
            PageLoadWait.WaitForPageLoad(15);
            PageLoadWait.WaitForFrameLoad(10);
        }

        public void AddPresetForRole(string modality, string preset, string width, string level, string layout = "auto")
        {
            var select = new SelectElement(GetElement("cssselector", "#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_m_viewingProtocols_DropDownListModalities"));
            var selectlayout = new SelectElement(GetElement("cssselector", "#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_m_viewingProtocols_DropDownListLayout"));
            IWebElement presetName = GetElement("cssselector", "#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_m_viewingProtocols_AliasTextBox");
            IWebElement widthField = GetElement("cssselector", "#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_m_viewingProtocols_WidthTextBox");
            IWebElement levelField = GetElement("cssselector", "#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_m_viewingProtocols_LevelTextBox");
            IWebElement savePreset = GetElement("cssselector", "#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_m_viewingProtocols_SaveAliasButton");
            IWebElement webElement = GetElement("cssselector", "#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_m_viewingProtocols_DropDownListAlias");

            select.SelectByText(modality);
            selectlayout.SelectByText(layout);

            if (webElement != null && webElement.Displayed && webElement.Enabled)
            {
                presetName.Clear();
                presetName.SendKeys(preset);
                Thread.Sleep(1000);
                widthField.Clear();
                widthField.SendKeys(width);

                levelField.Clear();
                levelField.SendKeys(level);

                savePreset.Click();
                savePreset.Click();
            }
        }

        public bool VerifyPresetsInRole(string modality, string layout, string preset, bool value = true)
        {
            bool IsPresetPresent = false;
            SwitchToDefault();
            SwitchTo("index", "0");
            UnCheckCheckbox("cssselector", "ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_ViewingProcotocolsConfigUseDomainLayoutCheckbox");
            SelectFromList("cssselector", "#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_m_viewingProtocols_DropDownListModalities", modality, 1);
            SelectFromList("cssselector", "#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_m_viewingProtocols_DropDownListLayout", layout, 1);
            IList<IWebElement> options = Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_viewingprotocolsConfig_m_viewingProtocols_DropDownListAlias")).FindElements(By.TagName("option"));
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

        public void CreateConfListRole(string DomainA, string roleA1)
        {
            ClickNewRoleBtn();
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(10);
            CreateRole(DomainA, roleA1, ConfUsr: 1, GrantAccess: 0);
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(20);
        }

        /// <summary>
        /// To Connect DataSources
        /// </summary>
        /// <param name="datasources">This paramters is for list of datasources separted by delimeter "=" thats needs Connected</param>
        public void ConnectDataSource(String datasources)
        {

            //Expand list if there is a RDM Data Source
            this.UnCheckCheckbox(UseAllDataSource());
            foreach (IWebElement datasource in this.List_ConnectDatasource())
            {
                String datasourcename = datasource.GetAttribute("innerHTML");
                if (datasourcename.Equals(Config.rdm))
                {
                    String cssselector = "#dataSourceItem_" + Config.rdm + ">div.dataSourceListHeader>div>span";
                    BasePage.Driver.FindElements(By.CssSelector(cssselector))[1].Click();
                    break;
                }
            }


            //Select DataSource
            var listdatasource = datasources.Split('=').ToList<String>();
            foreach (IWebElement datasource in this.List_ConnectDatasource())
            {

                String datasourceid = datasource.GetAttribute("id");
                String datasourcename = datasource.GetAttribute("innerHTML");
                if (
                    (listdatasource.Any(datasourcename1 =>
                    {
                    if (datasourceid.Contains(Config.rdm) && datasourcename1.Contains(Config.rdm))
                    {
                      return datasourceid.Contains(datasourcename1);
                    }
                    else if (datasourceid.Contains(Config.rdm) && !datasourcename1.Contains(Config.rdm))
                    {
                        return false;
                    }
                    else 
                    {  
                       return datasourcename.Equals(datasourcename1);
                    }
                     })) &&
                     (datasource.Displayed))
                {               
                    //Select and add data source
                    datasource.Click();
                    this.Btn_Add().Click();

                    //Synch up
                    // BasePage.wait.Until<Boolean>(d => d.FindElements(
                    // By.CssSelector("div[id$='_NewRolePreferenceConfig_RoleAccessFilter_FilterDataSourceListControl']>div div[id^='dataSourcePathId_']"))
                    // .Any(element => element.GetAttribute("innerHTML").Contains(datasourcename)));
                    Thread.Sleep(2000);
                }
            }
        }

        /// <summary>
        /// This is to Create role with enabling checkboxes,datasources,specific grantaccess 
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="roleName"></param>
        /// <param name="roletype"></param>
        /// <param name="datasourcelist"></param>
        public void CreateRole(String domainName, String roleName, String roledescription, String[] checkboxes, Boolean isGrantAccessAnyone = false, Boolean isGrantAccessGrouponly = false, String datasourcelist = null)
        {
            PageLoadWait.WaitForFrameLoad(10);
            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#NewRoleButton")));
            }
            catch (Exception)
            {
                new Login().Navigate("Role Management");
                PageLoadWait.WaitForFrameLoad(10);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#NewRoleButton")));
            }
            ClickButton("#NewRoleButton");
            PageLoadWait.WaitForFrameLoad(30);

            //Select Domain
            BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("select[id*='_DomainDropDown_']")));
            this.SelectFromList(BasePage.Driver.FindElement(By.CssSelector("select[id*='_DomainDropDown_']")), domainName);
            String domain = "select[id*='_DomainDropDown_']>option[value='" + domainName + "']";
            BasePage.Driver.FindElement(By.CssSelector(domain)).Click();

            //Enter Role name and Description  
            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("select[id$='ChooseCopyRoleDropDownList']")));
            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='_NewRolePreferenceConfig_RoleAccessFilter_Name']"))).Click();
            int counter = 0;
            while (!BasePage.Driver.FindElement(By.CssSelector("[id$='_NewRolePreferenceConfig_RoleAccessFilter_Name']")).Text.Equals(roleName) && counter++ <= 3)
            {
                SetText("cssselector", "[id$='_NewRolePreferenceConfig_RoleAccessFilter_Name']", roleName);
                Thread.Sleep(1000);
            }
            counter = 0;
            while (!BasePage.Driver.FindElement(By.CssSelector("[id$='_NewRolePreferenceConfig_RoleAccessFilter_Description']")).Text.Equals(roledescription) && counter++ <= 3)
            {
                SetText("cssselector", "[id$='_NewRolePreferenceConfig_RoleAccessFilter_Description']", roledescription);
                Thread.Sleep(1000);
            }

            //Set Viewer Type
            SetViewerTypeInNewRole();

            //Connect DataSource
            if (!String.IsNullOrEmpty(datasourcelist))
            {
                this.ConnectDataSource(datasourcelist);
            }

            //Set Checkboxes
            foreach (String features in checkboxes)
            {
                string[] feature = features.Split('=');
                if (feature.Length == 1)
                {
                    SetCheckboxInEditRole(features, 0);
                }
                else
                {
                    SetCheckboxInEditRole(feature[0], Convert.ToInt32(feature[1]));
                }
            }

            //Grant Access
            if (isGrantAccessAnyone)
            {
                if (!GrantAccessRadioBtn_Anyone().Selected)
                {
                    GrantAccessRadioBtn_Anyone().Click();
                }
            }
            else if (isGrantAccessGrouponly)
            {
                if (!GrantAccessRadioBtn_Grouponly().Selected)
                {
                    GrantAccessRadioBtn_Grouponly().Click();
                }
            }
            else
            {
                try    //Radio button not available in certain scenarios
                {
                    if (!GrantAccessRadioBtn_Disabled().Selected)
                    {
                        GrantAccessRadioBtn_Disabled().Click();
                    }
                }
                catch (Exception) { }
            }
            //Save the Transaction      
            if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer")))
                Click("cssselector", "[id$='_MasterContentPlaceHolder_SaveButton']", true);
            else
                Click("cssselector", "[id$='_MasterContentPlaceHolder_SaveButton']");
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(20);
            PageLoadWait.WaitForLoadingMessage(10);
        }

        public void RemoveDataSource(string Datasource=null)
        {
            this.UnCheckCheckbox(BasePage.Driver.FindElement(By.CssSelector("#ctl00_MasterContentPlaceHolder_NewRolePreferenceConfig_RoleAccessFilter_UseAllDataSourcesCB")));
            IList<IWebElement> ConnectedDatasource = List_ConnectedDatasource();
            if (string.IsNullOrWhiteSpace(Datasource))
            {
                foreach (IWebElement datasource in ConnectedDatasource)
                {
                    datasource.Click();
                }
                Btn_Remove().Click();
            }
            else
            {
                string[] Datasources = Datasource.Split('=');
                foreach (string source in Datasources)
                {
                    foreach (IWebElement datasource in ConnectedDatasource)
                    {
                        if (string.Equals(datasource.Text, source, StringComparison.OrdinalIgnoreCase))
                        {
                            datasource.Click();
                            break;
                        }
                    }
                }
                Btn_Remove().Click();
            }
        }

        /// <summary>
        ///     This function connects the all the data sources for both Add/Edit domain
        /// </summary>
        public void ConnectAllDataSources(int i = 0)
        {
            PageLoadWait.WaitForPageLoad(20);
            if (i == 0)
            {
                SetCheckbox("cssselector", "[id$='_RoleAccessFilter_UseAllDataSourcesCB']");
            }
            else
            {
                UnCheckCheckbox("cssselector", "[id$='_RoleAccessFilter_UseAllDataSourcesCB']");
            }
            wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id*='dataSourcePathId_']")));
            PageLoadWait.WaitForFrameLoad(10);
        }

        /// <summary>
        /// Setting modality for a Role
        /// </summary>
        /// <param name="Filter"></param>
        public void RoleFilter_RefPhysician(String LName, String FName)
        {
            BasePage.Driver.SwitchTo().DefaultContent();
            BasePage.Driver.SwitchTo().Frame("UserHomeFrame");
            IWebElement RefPhysician = BasePage.Driver.FindElement(By.CssSelector("select[id$='_RoleAccessFilter_FilterDropDownList']"));            
            IWebElement LastName  = Driver.FindElement(By.CssSelector("input[id$='_RoleAccessFilter_LastNameText'"));
            IWebElement FirstName = Driver.FindElement(By.CssSelector("input[id$='_RoleAccessFilter_FirstNameText'"));
            IWebElement AddBtn = BasePage.Driver.FindElement(By.CssSelector("input[id$='_RoleAccessFilter_AddButton']"));

            SelectFromList(RefPhysician, "Referring Physician", 1);
            LastName.SendKeys(LName);
            FirstName.SendKeys(FName);
            AddBtn.Click();
            UnCheckCheckbox("cssselector", "[id$='_RoleAccessFilter_SelfStudiesCheckBox']");
        }

        /// <summary>
        /// Setting viewer type in role
        /// </summary>
        public void SetViewerTypeInNewRole(string ViewerType = "universal")
        {
            if (!AllowUniversalViewer().Selected)
            {
                ClickElement(AllowUniversalViewer());
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

        /// <summary>
        /// To Connect External Applications
        /// </summary>
        /// <param name="appNames">This paramters is for list of external applications separted by delimeter "=" thats needs Connected</param>
        public void ConnectExternalApplications(String appNames = null)
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame(0);
            if (string.IsNullOrWhiteSpace(appNames))
            {
                foreach (IWebElement externalApp in List_ConnectExternalApplications())
                {
                    externalApp.Click();
                }
                Thread.Sleep(2000);
                Btn_ConnectApplication().Click();
            }
            else
            {
                //Select External application
                var listAppName = appNames.Split('=').ToList<String>();
                foreach (IWebElement app in List_ConnectExternalApplications())
                {
                    String appName = app.GetAttribute("innerHTML");
                    if (
                        (listAppName.Any(appname1 =>
                        {
                            if (appname1.Contains(appName))
                            {
                                return appName.Equals(appname1);
                            }
                            else
                                return false;
                        })) &&
                         (app.Displayed))
                    {
                        //Select and add data source
                        app.Click();
                        Thread.Sleep(1000);
                        Btn_ConnectApplication().Click();
                        Thread.Sleep(2000);
                    }
                }
            }
        }

        /// <summary>
        /// This is to Create role by copying from another role
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="copyFromRole"></param>
        /// <param name="roleName"></param>
        /// <param name="roletype"></param>        
        public void CreateRoleByCopy(String domainName, String copyFromRole, String roleName, String roledescription)
        {
            PageLoadWait.WaitForFrameLoad(10);
            try
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#NewRoleButton")));
            }
            catch (Exception)
            {
                new Login().Navigate("RoleManagement");
                PageLoadWait.WaitForFrameLoad(10);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("#NewRoleButton")));
            }
            ClickButton("#NewRoleButton");
            PageLoadWait.WaitForFrameLoad(30);

            //Select Domain
            BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("select[id*='_DomainDropDown_']")));
            this.SelectFromList(BasePage.Driver.FindElement(By.CssSelector("select[id*='_DomainDropDown_']")), domainName);
            String domain = "select[id*='_DomainDropDown_']>option[value='" + domainName + "']";
            BasePage.Driver.FindElement(By.CssSelector(domain)).Click();

            //Select the role to copy from  
            try
            {
                BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("select[id$='ChooseCopyRoleDropDownList']")));
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("select[id$='ChooseCopyRoleDropDownList']")));
                new SelectElement(BasePage.Driver.FindElement(By.CssSelector("select[id$='ChooseCopyRoleDropDownList']"))).SelectByText(copyFromRole);
            }
            catch (Exception) { }

            //Enter Role name and Description 
            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("[id$='_NewRolePreferenceConfig_RoleAccessFilter_Name']"))).Click();
            int counter = 0;
            while (!BasePage.Driver.FindElement(By.CssSelector("[id$='_NewRolePreferenceConfig_RoleAccessFilter_Name']")).Text.Equals(roleName) && counter++ <= 3)
            {
                SetText("cssselector", "[id$='_NewRolePreferenceConfig_RoleAccessFilter_Name']", roleName);
                Thread.Sleep(1000);
            }
            counter = 0;
            while (!BasePage.Driver.FindElement(By.CssSelector("[id$='_NewRolePreferenceConfig_RoleAccessFilter_Description']")).Text.Equals(roledescription) && counter++ <= 3)
            {
                SetText("cssselector", "[id$='_NewRolePreferenceConfig_RoleAccessFilter_Description']", roledescription);
                Thread.Sleep(1000);
            }

            //Save the Transaction      
            Click("cssselector", "[id$='_MasterContentPlaceHolder_SaveButton']");
            PageLoadWait.WaitForFrameToBeVisible(15);
            PageLoadWait.WaitForPageLoad(10);
            PageLoadWait.WaitForFrameLoad(20);
            PageLoadWait.WaitForLoadingMessage(10);
        }

        /// <summary>
        /// To Disconnect External Applications
        /// </summary>
        /// <param name="appNames">This paramters is for list of external applications separted by delimeter "=" thats needs Connected</param>
        public void DisconnectExternalApplications(string appNames = null)
        {
            Driver.SwitchTo().DefaultContent();
            Driver.SwitchTo().Frame(0);
            IList<IWebElement> connectedApps = List_ConnectedExternalApplications();
            if (string.IsNullOrWhiteSpace(appNames))
            {
                foreach (IWebElement externalApp in connectedApps)
                {
                    externalApp.Click();
                }
                Thread.Sleep(2000);
                Btn_DisconnectApplication().Click();
            }
            else
            {

                string[] listAppName = appNames.Split('=');
                //Select connected External application               
                foreach (IWebElement app in connectedApps)
                {
                    String appName = app.GetAttribute("innerHTML");
                    if (
                        (listAppName.Any(appname1 =>
                        {
                            if (appname1.Contains(appName))
                            {
                                return appName.Equals(appname1);
                            }
                            else
                                return false;
                        })) &&
                         (app.Displayed))
                    {
                        //Select and add data source
                        app.Click();
                        Thread.Sleep(1000);
                        Btn_DisconnectApplication().Click();
                        Thread.Sleep(2000);
                    }
                }
            }
        }

        /// <summary>
        /// This method is used to Enable/Disable the Localizer based on Modality
        /// </summary>
        /// <param name="ModalityList"></param>
        /// <param name="Enable"></param> To enable pass "true" and to disable pass "False"
        public void SetLocalizerByModality(String[] modalityList, bool enable = true)
        {
            if (UseDomainSetting_modality().Selected)
                UseDomainSetting_modality().Click();
            foreach (String modality in modalityList)
            {
                ModalityDropDown().SelectByText(modality);
                if (enable)
                    SelectRadioBtn("LocalizerLineRadioButtons", "On");
                else
                    SelectRadioBtn("LocalizerLineRadioButtons", "Off");
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// This method is used to Enable/Disable the Localizer based on Modality
        /// </summary>
        /// <param name="ModalityList"></param>
        /// <param name="Enable"></param> To enable pass "true" and to disable pass "False"
        public void SetLocalizerByModality(String modality, bool enable = true)
        {
            if (UseDomainSetting_modality().Selected)
                UseDomainSetting_modality().Click();
            ModalityDropDown().SelectByText(modality);
            if (enable)
                SelectRadioBtn("LocalizerLineRadioButtons", "On");
            else
                SelectRadioBtn("LocalizerLineRadioButtons", "Off");
            Thread.Sleep(500);
        }
    }
}
