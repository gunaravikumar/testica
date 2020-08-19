using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Selenium.Scripts.Pages
{
    public static class Locators
    {
        public const string Uploader_nominate_button_id = "m_nominateStudyButton";
        
        public static class ID
        {
            public const string DomainMgmtJavaCheckbox = "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_DefaultToJavaEICB";
            public const string UserPrefJavaCheckbox = "defaultToJavaExamImporterCB";
            public const string StudyListTable = "gridTableStudyList";
            public const string UserPreferenceOptionstable1 = "options_menu_0";
            public const string UserPreferenceOptionstable2 = "options_menu_1";
            public const string UserPreferenceDiv = "#PreferencesDiv.dialogRounded";
            public const string UserPreferenceFrame = "m_preferenceFrame";
            public const string CancelUserPreferenceButton = "CancelPreferenceUpdateButton";
            public const string HelpOptionstable1 = "help_menu_0";
            public const string HelpOptionstable2 = "help_menu_1";
            public const string ShowAllDomainsCheckbox = "m_domainSearchControl_reset";
            public const string ReceivingInstitutionTextbox = "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_ReceivingInstitution";
            public const string DomainSelector_RoleMgmt = "m_listResultsControl_m_resultsSelectorControl_m_selectorList";
            public const string NewUserButton = "NewUserButton";
            public const string NewUserDialogLabel = "m_newUserDialog_NewUserLabel";
            public const string UserWindowRoleMgmt = "userListParentDiv";
            public const string GroupWindowRoleMgmt = "hierarchyList_0";
            public const string CreateGroupDialogLabel = "m_groupInfoDialog_CreateGroupTitleLabel";
            public const string UserSearchBoxUserMgmt = "GroupListControl_m_filterInput";
            public const string UserSearchButtonUserMgmt = "GroupListControl_Button_Search";
            public const string UserListUserMgmt = "0_hierarchyUserList_itemList";
            public const string NewSubGroupUserMgmt = "NewSubgroupButton";
            public const string LicenseTable = "m_licenseUsageListControl_m_dataListGrid";
            public const string EmailIDUserMgmt = "ctl00_MasterContentPlaceHolder_UserInfo_Email";
            public const string UserSaveUserMgmt = "ctl00_MasterContentPlaceHolder_SaveButton";
            public const string EmergencySearchRadio = "m_studySearchControl_m_emergencySearchRadio";
            public const string DomainMgmtFilter = "ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_FilterDropDownList";
            public const string DomainMgmtFilterText = "ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_PrefValue";
            public const string DomainMgmtFilterAddButton = "ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_AddButton";
            public const string StudyNameDivViewer = "m_studyPanels_m_studyPanel_1_patientInfoDiv";
            public const string ModalityListBox = "ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_ModalityListBox";
            public const string DomainMgmtReferPhysFilterText = "ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_RoleAccessFilter_LastNameText";
            public const string SavePresetButton = "m_studySearchControl_m_saveSearchButton";
            public const string PresetTextbox = "m_searchPresetNameTextBox";
            public const string PresetSaveButton = "SaveSearchButton";
            public const string PresetDropdown = "m_studySearchControl_SearchPresetsDropDownList";
            public const string GroupByDropdown = "m_studyGrid_m_groupByDropDownList";
            public const string ElementAddbutton = "ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_sslConfigControl_ssclAddButton";
            public const string SearchFieldSelector = "ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_sslConfigControl_ssclHiddenSearchFieldsLB";
            public const string Allviewports = "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewerDiv";
            public const string viewer = "m_studyPanels_m_studyPanel_1_studyViewerContainer";
			public const string UserPrefImageFormatJPG = "NonTransientImageFormatRadioButtonList_0";
            public const string UserPrefImageFormatPNG = "NonTransientImageFormatRadioButtonList_1";
            public const string UserPrefStartPageRole = "DefaultPageRadioButtonList_3";
            public const string UserPrefSaveButton = "SavePreferenceUpdateButton";
            public const string LossyCompressedDiv = "m_studyPanels_m_studyPanel_1_compressionDiv";
            public const string RoleSearchTextBox = "m_roleSearchControl_m_input1";
            public const string LossyCompressedTextPrint = "PrintCompressionText";
            
            //********Series Viewer**************
            public const string SeriesViewer1_1x1 = "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg";
            public const string SeriesViewer2_2x3 = "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_2_viewerImg";
            public const string SeriesViewer3_2x3 = "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_3_viewerImg";
            public const string SeriesViewer4_2x3 = "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_1_viewerImg";
            public const string SeriesViewer5_2x3 = "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_2_viewerImg";
            public const string SeriesViewer6_2x3 = "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_2_3_viewerImg";
            public const string SeriesViewer1_2X2 = "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_viewerImg";
            public const string ScrollNext1_1X1 = "m_studyPanels_m_studyPanel_1_ctl03_SeriesViewer_1_1_m_scrollNextImageButton";
            //***********************************
            public const string ThumbnailDivMain = "m_studyPanels_m_studyPanel_1_thumbnailContent";
            public const string WarningMsgContent = "messageContent";
            public const string eleEditDomainPrimaryDropDown = "ctl00_MasterContentPlaceHolder_EditDomainControl_m_editDomainPidDomainControl_m_primaryDropDown";
            public const string PIDPrimaryDropDownNewDomain = "EditDomainControl_m_editDomainPidDomainControl_m_primaryDropDown";
            public const string chkIncludeLocalRelatedStudy = "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_IncludeLocalRelatedStudiesCB";
            public const string MPISearchBox = "m_studySearchControl_m_searchInputEnterprisePatientID";
            public const string ModalitySearchBox = "m_studySearchControl_m_searchInputModality";
            public const string ReferingPhysicianTxtBox = "m_studySearchControl_m_searchInputReferringPhysicianName";

            public const string RadBtnPresetNamed = "m_savePresetRadio";
            public const string txtBoxPresetName = "m_searchPresetNameTextBox";
            public const string eleIPIDDropDownStudyPage = "m_studySearchControl_m_ipidSelectorControl_m_pidDomainSelector";
            public const string eleIPIDSearchBoxStudyPage = "m_studySearchControl_m_ipidSelectorControl_m_ipidTextBox";
            public const string chkUseDomainSettingRolePage = "ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_editDomainPidDomainControl_pidUseDomainCB";
            public const string txtPatientIdTextBox = "m_studySearchControl_m_searchInputPatientID";

            public const string btnTransfer = "ctl00_StudyTransferControl_TransferButton";
            public const string SecondViewport = "studyPanelDiv_2";
            public const string ThirdViewport = "studyPanelDiv_3";
            public const string ReclOtherPIdTextBox = "m_ReconciliationControl_TextboxOtherPID_Searched";
            public const string ReclPIdTextBox = "m_ReconciliationControl_TextboxPID_Searched";
            public const string MaintainanceEventID = "m_messageDetail_EventIDDetail";
            public const string MaintainanceMasterPI = "m_messageDetail_EnterprisePatientIDTextbox";
            public const string PatientAccNoField = "m_studySearchControl_m_searchInputAccession";
            public const string NhsNo = "m_studySearchControl_m_inputEnterprisePatientIDLabel";
            public const string studySearchUseDomainSettingRolePage = "ctl00_MasterContentPlaceHolder_EditRolePreferenceConfig_m_sslConfigControl_ssclUseDomainCB";
            public const string ErrMsg = "ctl00_LoginMasterContentPlaceHolder_ErrorMessage";
            public const string SearchTable = "customSearchTable";
        }

        public static class Xpath
        {
            public const string UserPreferenceOptionsImg = "//img[@alt='Options']";
            public const string ChooseColumnsDiv = "//*[@id='gridPagerDivStudyList_left']/table/tbody/tr/td[1]/div";
            public const string ResetColumnsDiv = "//*[@id='gridPagerDivStudyList_left']/table/tbody/tr/td[3]/div";
            public const string MRThumbnailDiv = "//*[@id='ModalityDivMR']/div";
            public const string Result = "//div[@class='tile unselectable']//div[@class='left top'][starts-with(text(),'Result')]";
            public const string preset_xpath = "//div[@class='presets']//div[@class='dropdown Regular_cursor']//button[normalize-space(.)='string']";
            public const string CollapseMenuPreset = "//div[@class='toolbarDropdownMenu']//button[.='Preset']";
            public const string CollapseMenuSubVolumes = "//div[@class='toolbarDropdownMenu']//button[.='Sub Volumes']";
            public const string CollapseMenuRenderType = "//div[@class='toolbarDropdownMenu']//button[.='Render Type']";
            public const string CollapseMenuThickness = "//div[@class='toolbarDropdownMenu']//div[.='Thickness']";
            public const string CollapseMenuResult = "//div[@class='toolbarDropdownMenu']//button[.='Result']";
            public const string CollapseMenuToggle_3D_MPR = "//div[@class='toolbarDropdownMenu']//button[.='Toggle 3D/MPR']";
            public const string CollapseMenuFlip = "//div[@class='toolbarDropdownMenu']//div[.='Flip']";
            public const string RadioContainer = "//div[@class='radiocontainer']//mat-radio-button";
            public const string RadioLabelContent = "//div[@class='mat-radio-label-content']";
            public const string helpcontentframeset = "//frame[@src='whskin_frmset01.htm'][@frameborder='1']";
            public const string SmartBox_Visibility = "//*[@id='mat-select-2'and@class='mat-select ng-tns-c6-10 mat-select-disabled']";

        }

        public static class CssSelector
        {
            public const string UserPreferenceOptionsSelector = "table#options_menu_0>tbody>tr>td>a";
            public const string UserPreferenceImageSelector = "img[src^='Images/options']";
            public const string HelpSelector = "img[src^='Images/help']";
            public const string iCAHelpSelector = "table#help_menu_1>tbody>tr>td>a";
            public const string ToolbarUseDomainCheckboxRoleMgmt = "[id$='_RoleToolbarConfig_UseDomainToolbarCheckbox']";
            public const string StudyListUseDomainRoleMgmt = "[id$='_studyGrid_2_StudyGridConfigUseDomainLayoutCheckbox']";
            public const string EmergencyAccessCheckboxDomain = "[id$='_EmergencyAccessEnabledCB']";
            public const string AllowEmergencyAccessCheckboxRole = "[id$='_AllowEmergencyAccessCB']";
            public const string StudyListColumnTable = "#gview_gridTableStudyList>div.ui-state-default.ui-jqgrid-hdiv>div>table>thead>tr";
            public const string StudyListOKButton = "div.ui-dialog-buttonset>button";
            public const string ICATabCSS = "div.TabText";
            public const string AllowStudyListSaveCheckboxRole = "[id$='_RoleAccessFilter_AllowSaveStudyListLayoutCB']";
            public const string GroupByPlusMinusHeading = "span.ui-icon-circlesmall-minus";
            public const string DataSourceList = "div.msMenuContainer";
            public const string AboutICAMergeLogo = "#HelpAboutDiv>div.whiteRounded>div:nth-child(1)>img";
            public const string AboutICALogo = "#HelpAboutDiv>div.whiteRounded>div:nth-child(3)>img";
            public const string FromCalendarPrevButton = "#DateRangeSelectorCalendarFrom_mainheading>input[type='button']:nth-child(1)";
            public const string FromCalendarNextButton = "#DateRangeSelectorCalendarFrom_mainheading>input[type='button']:nth-child(4)";
            public const string FromCalendarMonthSelect = "#DateRangeSelectorCalendarFrom_mainheading>select:nth-child(2)";
            public const string FromCalendarYearSelect = "#DateRangeSelectorCalendarFrom_mainheading>select:nth-child(3)";
            public const string AllinOneTool = "img[title='All in One Tool']";
            public const string SearchPageViewText = "#gridPagerDivStudyList_right>div";
            public const string ReviewToolbarDomainMgmt = "[id$='_ToolbarConfiguration1_DrpListReviewAndModalities']";
            public const string EnableAttachmentDomainMgmt = "[id$='_AttachmentEnabledCB']";
            public const string AllowUploadAttachmentDomainMgmt = "[id$='_AttachmentAllowUploadEnabledCB']";
            public const string UserPrefStartPageRole = "input[value='Role']";
            public const string ViewportImgLocation = "div[class='tile unselectable'][style*='left: 3px']";
            public const string DownloadToolBox = "div.saveimagetolocaldialog i[class^='xclose']";
            public const string SculptToolBox = "div.sculptdialog i[class^='xclose']";
            public const string SelectionToolBox = "div.tissueselectiondialog i[class^='xclose']";
            public const string CloseSelectedToolBox = "i[class^='xclose']";
            public const string UserNamefield = "input[id$='_LoginMasterContentPlaceHolder_Username']";
            //Z3DBluRing
            public const string ViewerButton3D = "div.smartviewSelector div[class='mat-select-arrow']";
            public const string Layoutlist = "mat-option.mat-option span";
            public const string DropDown3DBox = "div[class^='mat-select-content']";
            public const string ControlImage = "div[class='tile unselectable']";
            public const string OverLayPane = "[class^='mat-dialog-container']";
            public const string CheckBox = "input[type='checkbox']";
            public const string SettingsButton = "div[title='User Settings']";
            public const string SettingsValues = "div[class='container ng-star-inserted']";
            public const string CheckBoxDiv = "div[class^='mat-checkbox-inner-container']";
            public const string SliderThumb = "div.mat-slider-thumb";
            public const string Centercontent = "div.contentcenter";
            public const string ConfirmButton = "span[class='mat-button-wrapper']";
            public const string Canvas = "canvas[class='unselectable']";
            public const string GridTile = "div.gridTile";
            public const string ToolSetContainer = "div[class*='toolsetContainer']";
            public const string ToolWrapper = "div[class^='toolWrapper']";
            public const string ToolBoxComponent = "div.viewportToolboxComponent";
            public const string Warning = "div[class='cdk-overlay-pane']";
            public const string Warningmsg = "div[class='messageBoxContentComponent']";
            public const string ActiveToolContainer = "div[class^='activeToolContainerComponent']";
            public const string CanvasImage = "img[class='imageCanvas unselectable']";
            public const string AnnotationLeftTop = "div[class^='left top']";
            public const string AnnotationRightTop = "div[class^='right top']";
            public const string AnnotationCentreTop = "div[class^='center top']";
            public const string AnnotationLeftMiddle = "div[class^='left middle']";
            public const string AnnotationRightMiddle = "div[class^='right middle']";
            public const string ViewerContainer = "div.compositeViewerContainer";
            public const string Viewport = "[class='fill unselectable']";
            public const string Crosshairvisibility = "[title='Toggle crosshair visibility']";
            public const string Thickness = "div.numericSlider";
            public const string ProgressBar = "div.progress";
            public const string Viewport2D = "div.seriesViewerPanelContainer";
            public const string ToolbarDialog = "div[class*='dialog'] mat-toolbar";
            public const string CenterBottomAnnotationValue = "div[class^='center bottom']";
            public const string ThreshholdProgressBar = ".slidercenter.fontDefault_m mat-slider";
            public const string RadiousProgressBar = "[Class='slidercenter'] mat-slider";
            public const string tissueSelectionVolume = "#volume";
            public const string LoadingIcon = "div.loadingspinner";
            public const string ExitIcon = "[title='Exit']";
            public const string SelectedLayout = ".smartviewSelector mat-select";
            public const string DownloadImgJPGPNG = "div[class='mat-radio-label-content']";
            public const string ThreeDsetting = "div[class*='globalSettingPanel']>div>ul:nth-child(2)>li";
            public const string DivThumbslider = "div[class='container ng-star-inserted']";
            public const string Thumbslider = "div[class='mat-tab-body-wrapper'] div[class='mat-slider-thumb";
            public const string Thumbslidervalue = "div[class='contentcenter fontDefault_m']";
            public const string PNGDisabled = "mat-radio-button[class*='fontDefault_m mat-radio-disabled']";
            public const string SixupviewCont = "div[class^='compositeViewerComponent3D']";
            public const string BusyCursor = "div[class^='showStatusIndicator']";
            public const string ThumbNailList = "div.thumbnailControlContainer div.thumbnailImage";
            public const string GridTilecontains = "div[class^='gridTile']";

            //3dTools
            public const string NgStarInserted = "div[class='ng-star-inserted']";
            public const string CompositeViewer = "div[class^='compositeViewerComponent']";
            public const string ViewContainer = "div[class='viewerContainer ng-star-inserted']";
            public const string SerieViewComponent = "div.seriesViewerComponent";
            public const string ToolsDialogbutton = "span.mat-button-wrapper";
            public const string dialogclose = "[class ^='xclose']";
            public const string dialogRadioBtn = "div.mat-radio-label-content";
            public const string ctrlpointdropdown = "div[class^='context-menu-content']";
            public const string ctrlpointoptions = "button[class^='menu-item']";
            public const string dropdownforsubvol = "div[class='toolbarComponent fontDefault_m'] div[class='leftPanel'] div[class='dropdownSelector']";
            public const string SelectedItem = "div[class='selectedItem']";
            public const string SaveImage = "div[class='mat-menu-content ng-trigger ng-trigger-fadeInItems']>table>tbody>tr[class*='menu']>td[class*='headerMenuPanel']>div[class*='menuPanelContainer'] div[class='menuPanelItem ng-star-inserted']>button[class='saveImageButton']";
            public const string thumnail = "div[class='studyPanelThumbnailContainerComponent";

            //2d
            public const string MenuContainer = "div.menuContainer";
            public const string StudyViewTitleBar = "div.studyViewerTitleBarContainer";
            public const string ThumbnailBar = "div.thumbnailBar";
            public const string PatientHistoryPanel = "div.patientHistoryPanelComponent";
            public const string saveimgdialg = "div.saveimagetolocaldialog";
            public const string saveimgtxt = ".ng-pristine.ng-valid.ng-touched";
            public const string saveimgradio = "div.mat-radio-label-content";
            public const string saveimgsavebtn = ".mat-raised-button.fontDefault_m:nth-child(1) span";
            public const string presetdrbdwn = "button.dropbtn";
            public const string selectPreset = "div[class='presets']  div[class='dropdown Regular_cursor']";
            public const string presetdrbdwnlist = "div[class ^='ng-tns-c32'] a";
            public const string select_result_dp = "div[class='rclassesultlink'] div[class='dropdown Regular_cursor']";
            public const string select_Preset_submenu = "div[='dropdown-content-inner ng-trigger ng-trigger-slideInOut']>div";
            public const string CenterTopPane = "div[class='annotations unselectable'] div[class='center top fontSmall_m']";
            public const string LeftTopPane = "div[class='annotations unselectable']>div[class='left top fontSmall_m']";
            public const string sThickness = "div[class='numericupdown'] span";
            public const string AfterPreset = "div[class='toolbarDropdownMenu']";
            public const string ThumbNailNextButton = "div.thumbnailNavNext";
            public const string ViewPort3Dbtn = "button.topright.threed.ng-star-inserted";
            public const string PopwindowwarnOk = "button[class='mat-raised-button']>span>p";
            public const string UserSetting = "div[class='toolIconBoxWrapper'][title='User Settings']>div[class='toolIconWrapperNL']";
            public const string UserSettingDP = "div[class='toolDropDownMenuBoxWrapper globalSettingPanel ng-trigger ng-trigger-menuState']>div[class='toolDropDownMenu'] ul li";

            public const string RenderDropDownValues = "a[class*='ng-star-inserted']";
            public const string TissueSelectionDialog = "div[class='tissueselectiondialog blu-ring-dialog']";
            public const string NumericThickness = "div[class='numericupdown'] span";
            public const string StudyForm = "form[name='StudyMainForm']";
            public const string options = "button[class^='btn collapsedMenu']>img";
            public const string FlipCheckbox = "div[class='checkbox']>i";
            public const string Navigation_Increment = "div[class='wrapper']>div:nth-child(2) div[class='numericupdown']>span";
            public const string toolbarDropdownMenuList = "[class*='mat-menu-item']";
            public const string Mouseoverlay = "button[class='btn collapsedMenu ng-star-inserted']>img";
            public const string TBDMainMenu = "div[role='menu']";
            public const string OptionCell = "div[class^='flextable-cell']";
            public const string CalciumScoringDialog = "div.calciumscoringdialog";
            public const string toolbar = "div.toolbar";
            public const string radiolabel = "label.mat-radio-label";
            public const string radiobutton = "input[type= 'radio']";
            public const string layoutvalue = "div[class='mat-select-value']";
            public const string dropdownSelector = "div[class*='dropdownSelector' ][title= '1']";
            public const string activeviewport = "div[class*='activeViewportContainer']";
            public const string loadvolumecomponent = "div[class='loadvolumeComponent'] div[class='progress']";
            public const string tabledata = "div[class*='flextable-data']";
            public const string tablerow = "div[class*='flextable-row']";
            public const string toolbarvalues = "span[id='1']";
            public const string copytoclipboard = "button[title*='(Ctrl-C)']";
            public const string ThumbnailImageCount = ".imageFrameNumber";
            public const string STudyTableList = "div[id='gridDivStudyList'] table[id='gridTableStudyList']";
            public const string StudyExamlist = "div[class*='relatedStudyContainer relatedStudyContainerActive']";
            // Viewport specific Locators
            public const string SecondPanelFirstViewport = "div.studyPanelsContainer blu-ring-study-panel-control:nth-of-type(2) div.viewerContainer:nth-of-type(1) .viewportDiv";
            public const string FirstPanelSecondViewport = "div.studyPanelsContainer blu-ring-study-panel-control:nth-of-type(1) div.viewerContainer:nth-of-type(2) .viewportDiv";
            public const string FirstPanelForthViewport = "div.studyPanelsContainer blu-ring-study-panel-control:nth-of-type(1) div.viewerContainer:nth-of-type(4) .viewportDiv";
            public const string FirstPanelFirstViewport = "div.studyPanelsContainer blu-ring-study-panel-control:nth-of-type(1) div.viewerContainer:nth-of-type(1) .viewportDiv";
            public const string ViewmodeDropdown = "div[class^='smartviewSelector font'] [id^='mat-select']";
            public const string ViewmodeDropdownText = "div.smartviewSelector span>span";
            public const string activetool = "div[class='toolIconControl isToolActive']";
            public const string dropdownactive = "div[class*='toolDropDownMenuBoxWrapper']";
            public const string labelcontent = "div.mat-tab-label-content";
            public const string leftcontent = "div[class*='contentleft']";
            public const string matlist = "mat-select[role='listbox']";
            public const string Undo = "button[title='Undo Segmentation']";
            public const string Redo = "button[title='Redo Segmentation']";
            public const string SaveImageExam = "button[title='Save image and annotations to the exam']";
            public const string SubVolumes = "div[title = 'Available volumes']>div[class='selectorContent']";
            public const string SelectPreset = "div[title='Apply window level preset']>div[class='selectorContent']";
            public const string SmartViewEnabled = "div[class='smartviewSelector fontDefault_m ng-star-inserted']>mat-select";
			public const string fps = "div[class^='fps']";
            public const string SmartViewSelectedValue = "span[class='mat-select-value-text ng-tns-c6-10 ng-star-inserted']";
            public const string SelectedTab = "div[class*='TabSelected']";
            public const string Close2ndpanel = "div.studyPanelsContainer blu-ring-study-panel-control:nth-of-type(2) div[class = 'closeButton']";
            public const string bluringstudypanel = "div.studyPanelContainerComponent blu-ring-study-panel-control";
            public const string FirstpanelThumbnailcount = "div.studyPanelContainerComponent blu-ring-study-panel-control:nth-of-type(1) div.thumbnailControlContainer div.thumbnailImage";
            public const string SecondpanelThumbnailcount = "div.studyPanelContainerComponent blu-ring-study-panel-control:nth-of-type(2) div.thumbnailControlContainer div.thumbnailImage";
            public const string wholepanel = "div.studyPanelContainerComponent blu-ring-study-panel-control div[class^='compositeViewerComponent3D']";
            public const string StudyPanel = "div.studyPanelsContainer blu-ring-study-panel-container blu-ring-study-panel-control";
            public const string toolBarDropDownList = "div.toolbarDropdownMenu ";
            //for new design menubutton starts here 
            public const string IMenubutton = "div[class='viewerMenuComponent'] div[class='viewerMenuButton']";
            public const string IMenutable = "div[class='mat-menu-content ng-trigger ng-trigger-fadeInItems']  tr[class*='menu']:not([class*='menuSpacerRow ng-star-inserted']) td";
            public const string IMenuThickness= "div[class='mat-menu-content ng-trigger ng-trigger-fadeInItems'] tr[class*='menuItem mat-menu-item'] div[class='menuItemSelection']  div>span";
            public const string thumbclick = "div[class='studyPanelThumbnailContainerComponent']";
            public const string IMenuSubTable = "div[class='mat-menu-content ng-trigger ng-trigger-fadeInItems'] div[class='viewerMenu']";
            public const string IFlipEnable = "div[class='toggleValue toggleValueEnabled']";
            public const string IFlipUncheck = "div[class^='toggleValue']";
            public const string IToggleMPR= "div[class='mat-menu-content ng-trigger ng-trigger-fadeInItems'] tr[class*='menuItem mat-menu-item'] td[class='menuItemLabel fontTitle_m']";
            public const string MenuClose = "div[Class$='viewerMenuCloseButton']";
            //for new design menubutton ends here 
            public const string menutable = "table[class='viewerMenu matMenuContent']";
            public const string menuitem = "td[class*='menuItem']";
            public const string menuitemvalue = "span[class*='menuItemValue']";
            public const string menubutton = "div[class*='viewerMenuButton']";
            public const string subMenulayout = "div.viewerMenu";
            public const string UndoRedoSavepanel = "div[class^='menuPanelItem'] [class$='Button']";
            public const string SubVolumebutton = "td[class^='menuItemLabel']";
            public const string SubVolumeAllImagecount = "div.viewerMenu div div[class^='menuControl ']";
            public const string Flipcheckbox = "div.menuItemSelection";
            public const string GetCulture = "select[id$='Culture']";
            public const string loginbtn = "input[name$='LoginButton']";
            public const string passwordbtn = "input[name$='PasswordButton']";
            public const string unamelable = "span[id$='UsernameLabel']";
            public const string pwdlable = "span[id$='PasswordLabel']";
            public const string IFlipcheckElement = "div[class='menuItemSelection']>div[class='toggleInput']>div[class='toggleValue toggleValueEnabled']";
            public const string IFlipUncheckElement = "table[class='viewerMenu matMenuContent'] td div[class^='toggleValue']";
            public const string Toggle3DButton = "tr[title='Toggle between 3D and MPR views'] td:nth-of-type(1)";
            public const string minbarframe = "frame[id='minibar_navpane']";
            public const string navpaneframe = "frame[id='navpane']";
            public const string chapterlink = "a[id^='B_']";
            public const string topicframe = "frame[id='topic']";
            public const string viewingmodecontent = "p[class='FM_3Level']";
        }

        public static class Name
        {
            public const string DomainMgmtJavaCheckbox = "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_DefaultToJavaEICB";
            public const string OptionTable = "tablescore";
        }

        public static class PartialLinktext
        {
            public const string DomainMgmtJavaCheckbox = "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_DefaultToJavaEICB";
        }

        public static class Linktext
        {
            public const string DomainMgmtJavaCheckbox = "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_DefaultToJavaEICB";
        }

        public static class Classname
        {
            public const string DomainMgmtJavaCheckbox = "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_DefaultToJavaEICB";
            public const string ThumbnailBar = "thumbnailBar";
            public const string Studypanel = "compositeViewerContainer";
            public const string ToolBarDropDown = "toolbarDropdownMenu";
            public const string collapsedmenu = "btn collapsedMenu ng-star-inserted";
        }

        public static class Tagname
        {
            public const string DomainMgmtJavaCheckbox = "ctl00_MasterContentPlaceHolder_EditDomainControl_DomainInfo_DefaultToJavaEICB";
            public const string tablerowcalciumscore = "blu-ring-table-row-calcium-score";
        }
        
    }
}
