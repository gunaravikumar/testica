using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Selenium.Scripts.Reusable.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using Selenium.Scripts.Pages.iConnect;
using System.Runtime.Serialization;
using Word;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Globalization;

namespace Selenium.Scripts.Pages.iConnect
{
    public class BluRingViewer : BasePage
    {
        //Selected Study
        public static String SelectedStudy = "tr[aria-selected]";
        public static String div_studySearchResult = "div.ui-paging-info";

        //Bluring Button
        //public static String btn_bluringviewer = "input[id$='m_viewStudyButton']";
        public static String btn_bluringviewer = "input[id$='m_universalViewStudyButton']";
        public static String btn_bluringviewer_integrator = "input[name$= 'm_universalViewButton']";
        public static string btn_bluringviewer_ConferenceFolder = "input[id='UniversalViewStudyButton']";

        //UI Element properties - Viewport        
        public static String div_viewport = "div.viewerContainer:nth-of-type(1) .viewportDiv";
        public static String div_allViewportes = "div.compositeViewerComponent div[class*='viewerContainer'] div[class='viewerContainerComponent shown']";
        public static String div_allLinkScrollLoadingInProgress = "div.linkedScrollingParentDiv mat-progress-spinner[mode='determinate']";
        public static String div_viewport_Outer = "div [bluringdroppable]";
        public static String div_viewportNo(int viewportNo = 1) { return "div.viewerContainer:nth-of-type(" + viewportNo + ") .viewportDiv"; }
        public static String div_compositeViewerComponent = "div.compositeViewerComponent>div";
        public static String div_Panel(int panel = 1) { return "div.studyPanelsContainer blu-ring-study-panel-control:nth-of-type(" + panel + ") "; }

        public static String div_LayoutIcon = (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")) ? "div[title='Layout']" : "div.toolbarLayoutIconContainer";
        public static String div_ToolbarLayoutWrapper = "div.toolbarLayoutWrapper";
        public static String div_ToolbarLayoutDisabled = "toolbarLayout-disabled";
        public static String div_LayoutGridWrapper = "div.dimensionWrapper";
        public static String div_ViewportPanels = "div[class='viewerContainer ng-star-inserted']";
        public static String div_ConnectionTool = "div[title='Network Connection']";
        public static String div_HelpIcon = "div[title='Help']";
        public static String li_AboutIcon = div_HelpIcon + "~div>div[class='toolDropDownMenu']>ul>li";
        public static String span_Demographics = "span[class='demoDetailContents']";		

		//UI Elements - Stack Slider
		public static String div_StackSlider = ".mat-slider-thumb-label-text";
        public static String div_FullStackSlider = "div.mat-slider-track-wrapper";
        public static String div_StackSliderLabel = "div.labelComponent";
        public static String div_StackSliderBox = "div.mat-slider-thumb";
        public static String div_StackSliderMax = "div.sliderControlComponent div.labelComponent.fontDefault_m:nth-of-type(2)";







        //UI Elements - About Splash Screen
        public static String div_AboutScreen = "div.popDialog.showDialog";
        public static String div_AboutScreenVersion = "div.dialogBodyWrapper div:nth-of-type(1)>span";
        public static String div_AboutScreenUDInumber = "div.dialogBodyWrapper div:nth-of-type(2)>span";
        public static String div_AboutScreenFooter = "div.dialogFooterWrapper";
        public static String div_AboutScreenlogo = "div.logoDiv";
        public static String div_AboutScreenCloseButton = "div.closeButton";
        public static String p_IBMAboutScreenVersion = "p.aboutIBMSubTitle:nth-of-type(1)";
        public static String p_IBMAboutScreenUDInumber = "p.aboutUdi";
        public static String div_Manufacturername = "div.manufacturerImage > img:nth-child(1)";
        public static String div_Datemanufacturer = "div.manufacturerImage > img:nth-child(3)";
        public static String div_Cataloguenumbersymbol = "div.refPosition";
        public static String div_EuropeanCommunity = " td.ecPosition > img";
        public static String div_AustralianSponsor = "#ctl00_AboutControl_addressAustralia";
        public static String div_CEmark = "td.ceImage > img";
        public static String div_Consultinstructions = "div.ConsultManual > img";
        public static String div_Rxonly = "div.rx > img";
        public static String div_addressEmergo = "#ctl00_AboutControl_addressEmergo";
        public static String div_manufactureraddress = "div.manufacturerImage";
        public static String div_ManufacturernameEV = "div.manufacturerImage > img:nth-child(1)";
        public static String div_DatemanufacturerEV = "div.manufacturerImage > img:nth-child(3)";
        public static String div_CataloguenumbersymbolEV = "td.ceImage > img";
        public static String div_EuropeanCommunityEV = " td.ecPosition > img";
        public static String div_AustralianSponsorEV = "#AboutControl_addressAustralia";
        public static String div_CEmarkEV = "td.ceImage > img";
        public static String div_ConsultinstructionsEV = "div.ConsultManual > img";
        public static String div_RxonlyEV = "div.rx > img";
        public static String div_addressEmergoEV = "#AboutControl_addressEmergo";
        public static String div_manufactureraddressEV = "div.manufacturerImage";
        public static String div_IBMheaderEV = "div.headerAbout > img";
        public static String div_CopyRight = "#AboutControl_copyright";
        public static String div_CopyRightUV = "div.footerCopyright";
        public static String div_ManufacturernameUV = "div.manufacturerImage > img:nth-child(1)";
        public static String div_DatemanufacturerUV = "div.manufacturerImage > img:nth-child(3)";
        public static String div_CataloguenumbersymbolUV = "td.ceImage > img";
        public static String div_EuropeanCommunityUV = " td.ecPosition > img";
        public static String div_AustralianSponsorUV = "div.addressAustralia";
        public static String div_CEmarkUV = "td.ceImage > img";
        public static String div_ConsultinstructionsUV = "div.ConsultManual > img";
        public static String div_RxonlyUV = "div.rx > img";
        public static String div_addressEmergoUV = "div.emergoAddress";
        public static String div_manufactureraddressUV = "div.manufacturerImage";
        public static String div_IBMheaderUV = "div.headerAbout > img";
        public static String div_EV = "div[id = 'reviewToolbar']>ul[class='dropmenu'] li[itag = 'About'][title = 'About iConnect® Access']>a";

        //UI Elements - Thumbnails
        public static String div_thumbnails = "div.thumbnailControlContainer div.thumbnailImage";
        //public static String div_thumbnailloadingstatus = "blu-ring-study-panel-control.studyPanelControl div.thumbnailStatus";
        public static String div_thumbnailloadingstatus = "div.thumbnailStatus";
        public static String div_thumbnailcontainer = "div.thumbnailControlContainer";
        public static String div_viewportToolboxComponent = "div[class*='viewportToolboxComponent']";
        public static String div_allThumbnailsViewports = "div.thumbnailBar";
        public static String div_studyPanelThumbnailImages = "div.studyPanelThumbnailImageComponent .thumbnailInnerDiv";
        public static String div_ThumbnailOuter = ".thumbnailOuterDiv";
        public static String div_ThumbnailNavNext(int panel = 1) { return "blu-ring-study-panel-control:nth-child(" + panel + ") div.thumbnailNavNext > div > div"; }

        //UI Elements - Viewer Tool
        //public static String div_globalstackicon = "div.globalStackMode-toolbar";
        public static String div_viewToolPOPUp = "div.gridContainer";
        public static String div_toolsExpansionIcon = "div[class*='expansionIcon']";
        public static String div_toolsSetContainer = "div[class*='toolsetContainer']";
        public static String div_activetoolsContainer = "div[class^='activeToolContainer'] div[class^='toolWrapper']";
        public static String div_expandedtoolsContainer = "div.expandedToolsContainer";
        public static String div_toolWrapper = "div.toolWrapper";
        public static String input_calibrationtextbox = "input[type='text']";
        public static String div_closeStudy = "div[class$='exitIcon']";
        public static String div_closeStudyIconControl = "div.toolIconControl.exitIcon-disabled";
        public static String div_closeStudy_integrator = "div.exitIconBoxWrapper>div";
        public static String div_HelpTool = "div[class='toolIconBoxWrapper'][title='Help']";
        public static String div_Help = "div.toolDropDownMenu div.ng-tns-c4-4";
        public static String div_ShowHideTool = "div.globalToolbarPanel div[title = 'Show / Hide Operations']";
        public static String div_ShowHideToolName = div_ShowHideTool + " .toolIconLabel";
        public static String div_ShowHideDropdown = "div.globalToolbarPanel div.toolDropDownMenu ul li";
        public static String div_Toolbox = "div.viewportToolboxComponent";
        public static String div_toolboxContainer = "div[class*='toolboxContainer']";
        public static String div_viewportToolbox = "blu-ring-viewport-toolbox";
        public static String div_toolGrid = "div.gridTile";
        public static String fillerTool = "blu-ring-toolbox-filler-tool";
        public string ToolBoxOpened = "blu-ring-viewport-toolbox-container div[class='toolboxContainer ng-trigger ng-trigger-toolboxFadeout toolboxOpen']";

		//UI Elements  - Study Panel
		public static String div_studypanel = "div.studyPanelsContainer blu-ring-study-panel-control";
        public static String div_globalstackicon(int panel = 1) { return "blu-ring-study-panel-control:nth-child(" + panel + ") div[title='Global Stack'][class*='toolWrapper']"; }
        public static String div_studypaneldate = "div[class*='panel']>div:nth-of-type(1) div:nth-child(1) span:nth-child(1)";//"div[class*='panel']>div:nth-of-type(1)>span.content:nth-child(1)";
        public static String div_closestudypanel = "div.closeButton";
        public static String div_StudyPanel = "div.studyPanelsContainer blu-ring-study-panel-control.studyPanelControl";
        //public static String div_studypaneltime = "div[class*='panel']>div:nth-of-type(1) span.content:nth-child(2)";
        //public static String div_studypaneldescription = "div[class*='panel']>div:nth-of-type(2)>span.content:nth-child(1)";
        public static String div_studypaneltime = "div[class*='panel'] div.aligninfoToLeft>div:nth-of-type(1)>span.content:nth-child(1)";
        public static String div_studypaneldescription = "div[class*='panel'] div.aligninfoToLeft>div:nth-of-type(2)>span.content:nth-child(1)";
        public static String div_separator = "blu-ring-separator-tool";
        public static String div_globalStack = "div[title='Global Stack']";
        //UI Elements  - CINE Tool 
        /*public static String div_CINE_PlayBtn = "div.playButton";
        public static String div_CINE_PauseBtn = "div.pauseButton";
        public static String div_CINE_NextImageBtn = "div.nextImageButton";
        public static String div_CINE_PreviousImageBtn = "div.previousImageButton";*/
        public static String div_CINE_PlayBtn = "button.playButton";
        public static String div_CINE_PauseBtn = "button.pauseButton";
        public static String div_CINE_NextImageBtn = "button.nextImageButton";
        public static String div_CINE_PreviousImageBtn = "button.previousImageButton";
        public static String div_viewerMenuButton = "button.viewerMenuButton";
        public static String div_viewerMenuCloseButton = "button[class*='viewerMenuCloseButton']";
        public static String div_cineToolboxComponent = "div.cineToolboxComponent";
        public static String button_ExamMode = "button.examModeButton";
        public static String div_MoreButton = "div.moreButtonWrapper";
        //public static String div_CINE_FPSControlButton = "div.FPSControlButton";
        public static String div_CINE_FPS = "div.fpsControlButton";
        public static String div_CINE_FPSControlButton = "span.fpsValue";
        //public static String div_CINE_FPSSliderHandler = "div.mat-slider-thumb";
        public static String div_CINE_FPSSliderHandler = "div.viewerMenu div.mat-slider-thumb:not([style])";
        public static string div_CINE_FPSSlider = "mat-slider[aria-valuemax='60']";
        public static String div_CINE_PlayAllBtn = "div[title*='Group Play'] div";
        public static String div_CINE_PauseAllBtn = "div[title*='Group Pause'] div";
        public static String div_CINE_PlayNextSeriesBtn = "div[class*='nextSeries']";
        public static String div_CINE_PlayPrevSeriesBtn = "div[class*='previousSeries']";
        public static String div_PlayPrevSeriesVerify = "div[class='ClickToolComponent'] div[class*='toolWrapper tool-container']";
        public static String div_PlayBtnVerify = "div[class='toggleSelectToolComponent'] div[class*='toolWrapper tool']";
        //Exam List UI Elements   
        public static String div_relatedStudy = "div.ps-content blu-ring-related-study";
        public static String div_RSmodality = "div[class*='RSmodality']";

        //Thumbnail
        public static String div_ThumbnailSeriesValue = "div[class='thumbnailOuterDiv thumbnailImageSelected'] > div > div > div[class*='thumbnailCaption']";
        public static String div_ActiveThumbnail = "div[class='thumbnailOuterDiv thumbnailImageSelected']";
        public static String div_ScrollImgCount = "/html/body/div/blu-ring-root/div/blu-ring-study-viewer/form/div/div[4]/blu-ring-study-panel-container/div/blu-ring-study-panel-control/div/div[3]/blu-ring-composite-viewer/div/div[1]/blu-ring-viewer-host-component/div/blu-ring-series-viewer/div/blu-ring-slider-control/div/mat-slider/div/div[3]/div[3]/span";

        //GlobalStack
        public static String div_GlobaliconVerify = "div[class='ClickToolComponent'] div[class*='toolWrapper toolIcon']";
        public static String div_GlobalStackIconActive = "div[class*='toolWrapper toolIconControl']";

        //Warning Message
        public static String warning_msg = "div[class^='patientHistoryPartialResultFirstStudyFailed warningMessageText']";

        //Select List
        public static String id_List = "[id^='ctl00_ctl05_parentGrid_check']";
        /// <summary>
        /// This includes thumbnail icon and report icon
        /// </summary>
        public static String div_priorsBlock = "div.ps-content blu-ring-related-study";
        public string ExamListTitleBar = ".patientHistoryExamListTitle";
        public string ExamListTitleText = ".patientHistoryExamListTitle .content";
        public string HistoryTitleText = ".patientHistorySubTitle .content";
        public static String div_priors = "div.listContainer div.relatedStudyDiv>div[class^='relatedStudyContainer']";
        public static String div_priorsThumbnail = "div.listContainer div.relatedStudyDiv>div[class*='thumbnailIcon']";
        public static String div_ContainerPriors = "div.listContainer";
        public static String div_priorDate = "div[class*='date']";
        public static String div_resultsList = "div.listSubTitleBarContainer";
        public static String div_examListPanelDate = "div.RSdate.fontTitle_m";
        public static string examTimeInExamList = "div.RStime";
        public static string AccessionNumberInExamList = "div[class*='RSaccession']";
        public static String div_studyPanelThumbnailImageComponent = "div.studyPanelThumbnailImageComponent";
        public static String ActiveThumbnailAtstudyPanel = "div.studyPanelThumbnailImageComponent .thumbnailOuterDiv .thumbnailImageSelected";
        public static String div_priorsreportIcon = "div.listContainer div.relatedStudyDiv>div[class*='report']";
        public static String div_priorTime = "div[class*='time']";
        public static String div_ExamList = "div.relatedStudyDiv";
        public static String div_ContainerList = ".listOperationsContainer";
        public static String div_ExamList_thumbnails = "div.relatedStudyComponent div.thumbnailContainer:not([class*='hideDiv']) div.thumbnailImage";
        public static String div_Scrollbar_ExamList_thumbnails = "div.thumbnailContainer .ps--active-y .ps__thumb-y";
        public static String div_priorModality = "div[class*='modality']";
        public static String div_priorSite = "div[class*='site']";
        public static string ExamListStudyDescription = ".RSdescription";
        public static String select_priormodality = "select#modalitySelect";
        public static String select_priorsite = "select#siteSelect";
        public static String select_priorsort = "select#sortBy";
        public static String div_priortitlelist = "div[class*='SubTitleBarContainer'] span";
        public static String div_closeExamList = "div[class$='HistoryPanelDiv'] div.closeButton";
        public static String div_launchExamList = "div.globalToolbarPanel div[title='Patient History']";
        public static String div_site = ".relatedStudiesListOperationsComponent div:nth-of-type(2) .select_join";
        public static String select_Site = "select#siteSelect";
        public static String select_Siteoption = "select#siteSelect option:nth-of-type(2)";
        public static String select_Modality = "select#modalitySelect";
        public static String select_sortby = "select#sortBy";
        public static String div_SelectSort = "#mat-select-1 > div:nth-child(1) > div:nth-child(2)";
        public static String div_SortPopUp = "div[class^='mat-select-content']"; //opacity=1
        public static String div_SortItems = "mat-option";
        public static String div_ExamListthumbnailview = "div.relatedStudythumbnailContainerComponent";
        public static String div_Examlistdefaultselectedthumbnail = ".relatedStudyThumbnailImageComponent .thumbnailOuterDiv.thumbnailImageSelected";
        public static String div_examlistThumbnailVerticalScrollbar = "div.ps__thumb-y";
        public static String div_Studythumbnail = "div[class^='thumbnails']";
        public static String div_relatedStudyPanel = "blu-ring-related-study";
        public static String div_examListThumbnailContainer = "div[class^='thumbnailContainer']";
        public static string ThumbnailPreviewScrollBar = "div.relatedStudyComponent div.thumbnailContainer:not([class*='hideDiv']) .ps__thumb-y:not([style='top: 0px; height: 0px;'])";
        public static String div_thumbnailContainerExamList = "div[class^='ps-container thumbnails']";
        public static String div_examListThumbnailImageComponent = "div.relatedStudyThumbnailImageComponent";
        public static String div_examListPanel = "div.patientHistoryPanelComponent";
        public static String HistoryPanel_div = "div.patientHistoryPanelContainer";
        public static String div_examListThumbnailImages = "div.relatedStudyThumbnailImageComponent .thumbnailInnerDiv";
        public static String div_ExamListContainer = "div.relatedStudyComponent div.thumbnailContainer";
        public static String div_relatedStudyComponent = "div.relatedStudyComponent";
        public static String MultiplePatientErrorContent = "div[class*='patientHistoryMultiplePatient warningMessageText']>span";
        public IList<IWebElement> StudyPanelList => Driver.FindElements(By.CssSelector(".studyPanelControl"));
        public static string viewportContainer = ".compositeViewerContainer";
        public static string examCardDate = "div.RSDateTime.ng-star-inserted";
        public static String examCardStudyCountText = "div.content";

        //UI Elements - Reports
        public static string report_icon = ".reportDocIconContainer";
        public static String div_Reports = "div.mat-tab-label";
        public static String ReportContainer_div = "div.relatedStudyReportViewer"; //for SR
        public static String PDFContainer_div = "div.page div.canvasWrapper";
        public static String SRReport_iframe = "iframe#reportIframe";
        public static String Pdf_iframe = "iframe[type*='application/pdf']";
        public static String PdfViewer_iframe = "iframe[src*='PdfWebViewer.html']";//iframe[src*='PdfWebViewer.html']
        public static String pdfreport_continer = "div[id*='_ViewerContent_TextDisplay_text_Display_Div'] iframe"; //object //for SR
        public static String ReportTabList_div = "div.relatedStudyReportViewer div.mat-tab-label.mat-ripple";
        public static String CurrentReportBar = "div.relatedStudyReportViewer mat-ink-bar";
        public static String AUReport_div = "div#Audio_Display_Div audio";
        public static String PDFToolsBtn = "button#secondaryToolbarToggle";
        public static String PDFDownladBtn = "div#secondaryToolbarButtonContainer button[id*='Download']";
        public static String div_activeReportIcon = ".reportDocIconContainerActive";
        public static String reportContainerID = "ViewerContainer_Content";
        public static String AvailableReports = " div.relatedStudyDiv div[class*='reportDocIconContainer']>div[class='reportDocIcon']";
        public static String MergeportReport_iframe = "iframe#printIframe";
        public static String DisabledReporticon = "div[class='reportDocIcon reportDocIconDeActivated']";
        public static String EnabledReportIcons = "div.relatedStudyDiv>div[class*=onMouseHover][class*=reportDocIcon]";
        public static String PrintIcon = "div.printIcon";

        //UI Element - Others
        public static String div_mergeLogo = "div.brandPanelComponent";
        public static String userSettings_Icon = "div[title='User Settings']";
        public static String div_usercontoltools = ".globalSettingPanel div ul:nth-of-type";
        public static string div_userControlToolsList = "div[class*='globalSettingPanel'] ul li";
        public static String div_PatientAge = "div.patientDemoDetailWrapper.fontDefault_m span:nth-of-type(2)>span.demoDetailContents b";
        public static String div_PatientGender = "div.patientDemoDetailWrapper.fontDefault_m span:nth-of-type(3)>span.demoDetailContents";
        public static String div_PatientID = "div.patientDemoDetailWrapper span:nth-of-type(4)>span.demoDetailContents";
        public static String span_PatientDOB = "span#dobContent";
        public static String p_PatientName = "p.patientName";
        public static String div_thumbnailCaption = "div[class^='thumbnailCaption']";
        public static String div_imageFrameNumber = "div[class^='imageFrameNumber']";
        public static String div_thumbnailPercentImagesViewed = "div[class^='thumbnailPercentImagesViewed']";
		public static String div_thumbnailModality = "div[class^='modality']";
        public static String div_ViewportBorder = "div[class*='activeViewportContainer']";
        public static String div_Draggable = "div[class^='thumbnailInnerDiv']";
        /*public static String div_ThumbnailPreviousArrowButtonDisabled = "div[class = 'prevButton']";
        public static String div_ThumbnailNextArrowButtonDisabled = "div[class = 'nextButton']";
        public static String div_ThumbnailPreviousArrowButtonEnabled = "div[class='prevButton buttonEnabled']";
        public static String div_ThumbnailNextArrowButtonEnabled = "div[class='nextButton buttonEnabled']";*/
        public static String div_ThumbnailPreviousArrowButtonDisabled = "div[class = 'prevButtonBackground']";
        public static String div_ThumbnailNextArrowButtonDisabled = "div[class = 'nextButtonBackground']";
        public static String div_ThumbnailPreviousArrowButtonEnabled = "div[class='prevButtonBackground buttonEnabled']";
        public static String div_ThumbnailNextArrowButtonEnabled = "div[class='nextButtonBackground buttonEnabled']";
        public static String div_ThumbnailPreviousArrowButton = "div[class = 'prevButton']";
        public static String div_ThumbnailNextArrowButton = "div[class = 'nextButton']";
        public static String div_compositeViewer = "div.compositeViewerContainer";
        public static String div_LicenseWarningandMedium = "div.preReleaseText.fontPreRelease_m";
        public static String div_Warningsymbol = "div.preReleaseIconWrapper.preReleaseImage";
        public static String div_LicenseiconLarge = "div.preReleaseText.fontPreRelease_l";
        public static String div_Licenseiconsmall = "div.preReleaseText.fontPreRelease_s";
        public static String div_prereleaseWarningicon = "div.preReleaseMain.ng-tns-c0-0";
        public static String span_PatientDetailsFont = "span.demoDetailLabels";
        public static String span_PatientDetailsLabel = "span.demoDetailContents";
        public static String div_clinicalRelease = "div[class^='preReleaseText']";
        public static String div_examlistSubTitle = "div.patientHistoryTitleBar";
        public static String div_globalTitleBar = "div.menuPanelsContainer";
        public static String div_patientPanel = "div.patientPanel";
        public static String div_globalPrimaryTools = "div.globalToolbarPanel";
        public static String div_globalSecondaryTools = "div.userControlsPanel";
        public static String div_divider = "div.menuPanelDivider_large";
        public static String div_tooldivider = "div.toolDivier";
        public static String div_thumbnailIcon = "div.thumbnailIcon";
        public static String div_ActiveExamPanel = "div[class$=relatedStudyContainerActive]";
        public static String div_thumbnailpreviewIconActiveStudy = "div.thumbnailIconContainer.thumbnailIconContainerActive";
        public static String div_activeExamPanel = "div.relatedStudyContainer.relatedStudyContainerActive";
        public static String p_PatientNamesmall = "p.patientName.fontName_s";
        public static String p_PatientNamemedium = "p.patientName.fontName_m";
        public static String p_PatientNamelarge = "p.patientName.fontName_l";
        public static String div_showhideIconLabel = "div.globalToolbarPanel div[title='Show/Hide Operations'] .toolIconTextWrapper";
        public static String div_ExamsIconLabel = "div.globalToolbarPanel div[title='Patient History'] .toolIconTextWrapper";
        public static String div_ExamsIconButton = "div[class^='topMenu'] div[class^='toolIconWrapperNLMedium'] div[class='toolIconNLMedium patientIconWrapper']";
        public static String li_ViewerLayoutDropdown = "li.stackedDropdownItem>div";
        public static String div_ViewerLayoutDropdown = "div.toolbarLayoutWrapper td.dimensionColumn";
        public static String div_PatientInfoContainer = "div[class*='panelInfoContainer']";
        public static String div_thumbnailImageInViewer = "div.relatedStudyThumbnailImageComponent .thumbnailOuterDiv.thumbnailImageInViewer";
        public static String div_PRStatusIndicator = "blu-ring-save-part10-instance-indicator>div[class='statusIndicator']";
        public static String td_LayoutGridCells = "td[class='dimensionColumn ng-star-inserted box-focus-border']";
        public static String div_NetworkConnectionIcon = "div.toolIconBoxWrapper[title='Network Connection']";
        public static String div_NetworkConnectionDetails = "div.networkConnectionDetails";
        public static String div_NetworkConnectionDialogTitle = "div.dialogTitle";
        public static String div_NetworkDropdownDialogConnection = "div.toolDropDownMenu div.dialogConnection";
        public static String div_NetworkDropdownDialogDetails = "div.toolDropDownMenu div.dialogDetails";
        public static String div_NetworkDropdownDialogFooter = "div.toolDropDownMenu div.dialogFooter";
        public static String div_StudyViewerTitleBar = "div.studyViewerTitleBarContainer";
        public static String div_Usercontrolpanel = "div.userControlsPanel";
        public static String div_NetworkConnection = ".ng-tns-c2-2";
        public static String div_BluRingCollaboration = "div.userControlsPanel blu-ring-collaboration";
        public static String div_BluRingNetworkConnection = "div.userControlsPanel blu-ring-network-connection";
        public static String div_BluRingShowHide = "blu-ring-show-hide";
        public static String div_BluRingHelp = "blu-ring-help";
        public static String div_BluRingRelatedStudiesMultiSelect = "blu-ring-related-studies-list-operations bluring-multi-select";
        public static String cdk_OverlayContainer = ".cdk-overlay-container";
        public static String div_BluRingRelatedStudiesSingleSelect = "blu-ring-related-studies-list-operations bluring-single-select";
		public static String div_StudyPanelTitleComponent = ".studyPanelTitleComponent";
        public static String div_3DViewDropdown = "div.smartviewSelector";
        public static String div_LocalizerSwitch = "blu-ring-localizer-lines .toolIconControl";
        public static String div_LocalizerLinesIcon = "div.toolIconNLMedium.localizerLinesIcon";
		public static String div_LinkedScrollingsIcon = "div.toolIconNLMedium.linkedScrollingIcon";
		public static String span_3DOptionsListBox = "span.mat-option-text";
        public static String div_StudypanelMoreButton = ".toolbarItemTool .viewerMenuButton";
        public static String div_overlayPanel = "div.cdk-overlay-pane";
        public static String div_overlay = "#cdk-overlay-";

        //Email Study - Controls
        public static String div_toolbarEmailStudyWrapper = ".e4-toolbar";
        public static String div_emailContainer = "div[class^='emailStudyDetailsContainer']";
        public static String div_emailstudy = "div[class*='e4-toolbar']";
        public static String div_emailWindow = "div[class^='emailStudy'] div.dialogConatiner";
        public static String input_emailName = "input[name='name']";
        public static String input_email = "input[name='email']";
        public static String input_confirmemail = "input[placeholder='Confirm Email']";
        public static String input_Notes = "textarea[name='reason']";
        public static String div_pinWindow = "div.pinCodeDialogDimmerDiv";
        public static String div_sendEmail = ".dialogButton:nth-child(1) button:nth-child(1)";
        public static String div_cancelEmail = ".dialogButton:nth-child(2) button:nth-child(1)";
        public static String div_emailErrorMessage = ".errorDialog label:nth-child(1)";
        public static String label_emailAttachedStudies = ".dialogAttachedStudiesCount label:nth-child(1)";
        public static String div_closePinDialog = ".dialogHeader span:nth-child(1)";
        public static String label_emailPinCode = "#lblPincode";
        public static String label_emailPinCodeInfo = "#PinCodeInfo_Label";
        public static String div_emailTitle = "div[title='Email Study']";
        public static String label_emailModalityDropdown = "div[class*='dialogModalityFilterBox']";
        public static String div_emailStudyList = "div.emailrelatedstudylist";
        public static String div_emailSelectAll = "input[name='chkboxSelectAll']";
        //public static String div_emailRelatedStudyList = "div.emailStudyDetails div[class*='ps--active-y']";
        public static String div_emailRelatedStudyList = "div.emailStudyDetails div.ps-content";
        public static String span_modalityDropdownCurrentValue = "div[class*='dialogModalityFilterBox'] span>span[class*='ng-star-inserted']";
        public static String input_priorStudiesCheckboxes = "blu-ring-email-study-prior-details input[class ='ng-untouched ng-pristine ng-valid']";
        public static String input_primaryStudyCheckbox = "input[class ='ng-untouched ng-pristine']";
        public static String div_emailStudyDate = "div.emailStudyDetails div[class*='RSdate']";
        public static String div_emailStudyTime = "div.emailStudyDetails div[class*='RStime']";
        public static String div_emailAcession = "div[class*='RSaccession']";
        public static String div_emailModality = "div.emailStudyDetails div[class*='RSmodality']";
        public static String div_emailDescription = "div.emailStudyDetails div[class*='RSdescription']";
        public static String div_modalityFilter = "div.emailStudyDetails div.mat-input-infix.mat-form-field-infix";       
        public static String div_emailRelativeStudiesModalities = "blu-ring-email-study-prior-details div[class*='RSmodality']";
        public static String input_priorsCheckbox = "input[name='chkboxStudyList']";
        public static String StudypanelEmailStudy = "div.toolWrapper.tool-container-column[title='Email Study']";

        //Filters and Sorts - Exam List
        public static String div_multiSelect_Modality = "div.select_join";
        public static String div_modalityFilterPopup = "div[class^='mat-select-content']";
        public static string ModalityFliterPopUpWithScrollBar = ".ng-trigger-transformPanel";
        public static String div_modalityFilterPopup_Title = "div[class^='mat-select-content'] div.md-custom-div";        
        public static String modality_options_text = "span.mat-option-text";
        public static String modality_options_ele = "mat-option[class^='mat-option']";
        public static String modality_checkbox = "mat-pseudo-checkbox";
        public static string Modality_Clear_All = ".md-select-anchor";

        public static String div_StudySort = "div.singleSelect mat-select";
        public static String div_StudySortPopup = "div[class*='mat-select-panel']";
        public static String div_StudySortOption = "div[class*='mat-select-panel']";
        public static String div_StudySortOptionValues = "div[class*='mat-select-panel'] .mat-option-text";
        public static String div_MaxStudyPanelPopup = "div[id*='cdk-overlay'] mat-dialog-container";
        public static String div_MaxStudyPanelErrorMessage ="p#messageContent";       


        // Prior study viewport 
        public static String div_emailPrior = "body > div > blu-ring-root > div > blu-ring-study-viewer > form > div > div.studyPanelsContainer > blu-ring-study-panel-container > div > blu-ring-study-panel-control:nth-child(2) > div > div.studyViewerHeaderContainer > div.studyViewerTitleBarContainer > div.toolbarWrapper > blu-ring-study-panel-toolbar > div > div:nth-child(6) > div > blu-ring-click-tool > div > blu-ring-single-click-tool > div";
        public static String div_layoutPrior = "body > div > blu-ring-root > div > blu-ring-study-viewer > form > div > div.studyPanelsContainer > blu-ring-study-panel-container > div > blu-ring-study-panel-control:nth-child(2) > div > div.studyViewerHeaderContainer > div.studyViewerTitleBarContainer > div.toolbarWrapper > blu-ring-study-panel-toolbar > div > div:nth-child(5) > div > blu-ring-layout-selector-tool > div";
        public static String div_datePrior = "blu-ring-study-panel-control:nth-child(2) div div.studyViewerHeaderContainer div.studyViewerTitleBarContainer div.studyPanelTitleBarWrapper blu-ring-study-panel-title div div div.primaryInfoContainer.fontTitle_m span:nth-child(1)";
        public static String div_timePrior = "blu-ring-study-panel-control:nth-child(2) div div.studyViewerHeaderContainer div.studyViewerTitleBarContainer div.studyPanelTitleBarWrapper blu-ring-study-panel-title div div div.primaryInfoContainer.fontTitle_m span.content.fontDefault_m";
        public static String div_Aboutbox = "table[style='width:600px']";
        public static String div_AboutboxUV = "table[style='width:550px']";

        //Global stack 
        public static String div_Exammodebutton = "div[class='menuPanelContainer'] [class='examModeButton']";
        public static String div_Exammodebuttonclick = "div[class='menuPanelContainer'] [class='examModeButton examModeActive']";

        #region NotToUse
        public IWebElement PatinetName() { return BasePage.Driver.FindElement(By.CssSelector("p[class*='patientName']")); }
        public IWebElement DOB() { return BasePage.Driver.FindElement(By.CssSelector("#dobContent")); }
        public IWebElement PatinetID() { return BasePage.Driver.FindElement(By.CssSelector("#dobContent")); }
        public IWebElement studyPanel(int studyPanelIndex = 1) { return BasePage.Driver.FindElement(By.CssSelector("blu-ring-study-panel-control.studyPanelControl:nth-of-type(" + studyPanelIndex + ")")); }
        public IList<IWebElement> AllstudyPanel() { return BasePage.Driver.FindElements(By.CssSelector("blu-ring-study-panel-control.studyPanelControl")); }
        public IWebElement SettingButton() { return BasePage.Driver.FindElement(By.CssSelector(userSettings_Icon)); }
        public IWebElement SettingPanel() { return BasePage.Driver.FindElement(By.CssSelector(".globalSettingPanel")); }
        public IList<IWebElement> usersettingList() { return BasePage.Driver.FindElements(By.CssSelector(".content.uiSizes span:nth-child(2)")); }
        public IList<IWebElement> AllViewPorts() { return Driver.FindElements(By.CssSelector(div_allViewportes)); }
        public IWebElement ViewPortContainer() { return BasePage.Driver.FindElement(By.CssSelector(".compositeViewerContainer")); }
        public IWebElement ThumbnailandViewPortContainer() { return BasePage.Driver.FindElement(By.CssSelector(".studyPanelControlComponent")); }
        public IWebElement ExamIcon() { return BasePage.Driver.FindElement(By.CssSelector("div[title = 'Patient History']")); }
        //public IWebElement ExamTextLable() { return BasePage.Driver.FindElement(By.CssSelector("div.globalToolbarPanel div[title = 'Patient History'] .toolIconLabel")); }
        //public IWebElement ShowHideToolName() { return BasePage.Driver.FindElement(By.CssSelector(div_ShowHideToolName)); }
        public IWebElement ExitBluringViewerButton() { return BasePage.Driver.FindElement(By.CssSelector("div[title = 'Exit']")); }
        public IWebElement ExitButtonLable() { return BasePage.Driver.FindElement(By.CssSelector("div[title = 'Exit'] .toolIconLabel")); }
        public IWebElement MergeLogo() { return BasePage.Driver.FindElement(By.CssSelector("div.logoWrapper")); }
        public IWebElement MergeLogoText() { return BasePage.Driver.FindElement(By.CssSelector("div.logoTextWrapper")); }
        public IList<IWebElement> ThumbnailPercentImagesViewed() { return BasePage.Driver.FindElements(By.CssSelector("div[class*='thumbnailPercentImagesViewed']")); }
        public IList<IWebElement> thumbnailCaption() { return BasePage.Driver.FindElements(By.CssSelector("div[class*='thumbnailCaption']")); }
        public IList<IWebElement> ThumbnailImageFrameNumber() { return BasePage.Driver.FindElements(By.CssSelector("div[class*='imageFrameNumber']")); }
        public IWebElement ExamListLable() { return BasePage.Driver.FindElement(By.CssSelector(".patientHistoryTitleBar .patientHistoryExamListTitle")); }
        public IList<IWebElement> RecentStudyAllDates() { return BasePage.Driver.FindElements(By.CssSelector("div[class*='RSdate']")); }
        public IList<IWebElement> RecentStudyAllTimes() { return BasePage.Driver.FindElements(By.CssSelector("div[class*='RStime']")); }
        public IList<IWebElement> RecentStudyAllModality() { return BasePage.Driver.FindElements(By.CssSelector("div[class*='RSmodality']")); }
        public IList<IWebElement> RecentStudyAllDescription() { return BasePage.Driver.FindElements(By.CssSelector("div[class*='RSdescription']")); }
        public IList<IWebElement> RecentStudyAllAccession() { return BasePage.Driver.FindElements(By.CssSelector("div[class*='RSaccession']")); }
        public IList<IWebElement> PatientDemoDetailList() { return BasePage.Driver.FindElements(By.CssSelector("div [class*='patientDemoDetailWrapper'] span")); }
        public IWebElement thumbnailpreviewIconActiveStudy() { return BasePage.Driver.FindElement(By.CssSelector(".thumbnailIconContainer.thumbnailIconContainerActive")); }
        public IList<IWebElement> RecentStudythumbnailpreviewIcons() { return BasePage.Driver.FindElements(By.CssSelector("div[class='thumbnailIconContainer onMouseHover']")); }
        public IList<IWebElement> AllFilterInExamPanel() { return BasePage.Driver.FindElements(By.CssSelector(modality_options_text)); }
        public IList<IWebElement> RecentStudyActiveThumbnailCaption() { return BasePage.Driver.FindElements(By.CssSelector("div[class='thumbnailContainer'] .thumbnailCaption")); }
        public IList<IWebElement> RecentStudyActiveThumbnailModalityText() { return BasePage.Driver.FindElements(By.CssSelector("div[class='thumbnailContainer'] .thumbnailCaption")); }
        public IList<IWebElement> RecentStudyActiveThumbnailImageFrameNumber() { return BasePage.Driver.FindElements(By.CssSelector("div.relatedStudythumbnailContainerComponent .imageFrameNumber")); }
        public IWebElement ActiveRecentStudyThumbnailPreviewContainer() { return BasePage.Driver.FindElement(By.CssSelector("div[class='thumbnailContainer']")); }
        public IList<IWebElement> AllStudyDateAtStudyPanel() { return BasePage.Driver.FindElements(By.CssSelector("div[class*='primaryInfoContainer'] [class='content']")); }
        public IList<IWebElement> AllStudyTimeAtStudyPanel() { return BasePage.Driver.FindElements(By.CssSelector("div[class*='primaryInfoContainer'] span:nth-child(2)")); }
        public IList<IWebElement> AllStudyInfoAtStudyPanel() { return BasePage.Driver.FindElements(By.CssSelector("div[class*='secondaryInfoContainer'] .content")); }
        public IWebElement CloseExamPanel() { return BasePage.Driver.FindElement(By.CssSelector(".patientHistoryPanelDiv .closeButton")); }
        public IWebElement ExamPanel() { return BasePage.Driver.FindElement(By.CssSelector(".patientHistoryPanelContainer")); }
        public IWebElement AuthenticationErrorMsg() { return BasePage.Driver.FindElement(By.CssSelector("span#m_title")); }
        public static IWebElement ActiveExamPanel() { return Driver.FindElement(By.CssSelector("div[class$=relatedStudyContainerActive]")); }
        public static IWebElement StudyPanelThumbnailContainer() { return Driver.FindElement(By.CssSelector("div.thumbnailControlContainer")); }
        public static IWebElement ExamListThumbnailContainer() { return Driver.FindElement(By.CssSelector("div.thumbnailContainer")); }
        //public IList<IWebElement> Panel() { return Driver.FindElements(By.CssSelector("div[class='compositeViewerComponent']")); }
        public IList<IWebElement> ThumbnailLoadedIndicator(int StudyPanel)
        {
            IList<IWebElement> indicator = StudyPanels()[StudyPanel].FindElements(By.CssSelector("div.studyPanelThumbnailImageComponent"));
            return indicator.Where<IWebElement>(element => element.Displayed).ToList<IWebElement>();
        }
        public IList<IWebElement> RelatedStudyPanels() { return BasePage.Driver.FindElements(By.CssSelector("blu-ring-related-study")); }
        public IList<IWebElement> ThumbnailIndicator(int StudyPanel) { return StudyPanels()[StudyPanel].FindElements(By.CssSelector("div.studyPanelThumbnailImageComponent")); }
        public IList<IWebElement> StudyPanels() { return BasePage.Driver.FindElements(By.CssSelector("blu-ring-study-panel-control[class^=studyPanelControl]")); }
        public IList<IWebElement> GlobalToolbarPanel() { return Driver.FindElements(By.CssSelector(".toolIconLabel.fontSmall_m")); }
        public IList<IWebElement> DemoDetailLabels() { return Driver.FindElements(By.CssSelector(".demoDetailLabels")); }
        public IList<IWebElement> OperationListContainer() { return Driver.FindElements(By.CssSelector(".options>div")); }
        #endregion NotToUse

        //Constructor
        public BluRingViewer() { }

        //Instance members
        private String activeviewport = null;
        public String Activeviewport
        {
            get
            {
                if (activeviewport == null)
                {
                    return div_viewport;
                }
                else
                {
                    return activeviewport;
                }
            }
        }

        /// <summary>
        /// This method will the active view port
        /// </summary>
        /// <param name="viewport">should start from 0</param>
        /// <param name="panel">should start from 1</param>
        public string SetViewPort(int viewportnumber, int panelnumber)
        {
            //Set the Study panel
            String studypanel = div_studypanel + ":nth-of-type(" + panelnumber + ")";
            viewportnumber++;
            //Set the viewport			
            String studyviewport = "div.viewerContainer:nth-of-type(" + viewportnumber + ") .viewportDiv";
            this.activeviewport = studypanel + " " + studyviewport;

            return this.activeviewport;
        }

        /// <summary>
        /// This method will the active view port
        /// </summary>
        /// <param name="viewport">should start from 0</param>
        /// <param name="panel">should start from 1</param>
        public string SetViewPort1(int panelnumber = 1, int viewportnumber = 1)
        {
            //Set the Study panel
            String studypanel = div_studypanel + ":nth-of-type(" + panelnumber + ")";
            //Set the viewport			
            String studyviewport = "div.viewerContainer:nth-of-type(" + viewportnumber + ") .viewportDiv";
            this.activeviewport = studypanel + " " + studyviewport;

            return this.activeviewport;
        }

        /// <summary>
        /// This method will launch the BluRing viewer
        /// </summary>
        /// <returns></returns>
        public static BluRingViewer LaunchBluRingViewer(String tabname = "Studies", String fieldname = "", String value = "",
            String mode = "StandAlone", bool showselector = false, string url = null, int ThumbnailTimeout = 360, bool isRefreshForIntegrator=true)
        {
            var js = (IJavaScriptExecutor)Driver;
            if (mode.Equals("StandAlone"))
            {
                PageLoadWait.WaitForFrameLoad(30);
                if (tabname.Equals("Studies"))
                {
                    if (SBrowserName.ToLower().Equals("internet explorer"))
                    {
                        js.ExecuteScript("arguments[0].click()", Driver.FindElement(By.CssSelector(btn_bluringviewer)));
                    }
                    else
                    {
                        var button = BasePage.Driver.FindElement(By.CssSelector(btn_bluringviewer));
                        button.Click();
                    }
                }
                else if (tabname.ToLower().Equals("conference"))
                {
                    if (SBrowserName.ToLower().Equals("internet explorer"))
                    {
                        js.ExecuteScript("arguments[0].click()", Driver.FindElement(By.CssSelector(ConferenceFolders.btnUniversalviewer)));
                    }
                    else
                    {
                        var button = BasePage.Driver.FindElement(By.CssSelector(ConferenceFolders.btnUniversalviewer));
                        button.Click();
                    }
                }
                else
                {
                    new BluRingViewer().SelectStudy1(fieldname, value);
                    var button = BasePage.Driver.FindElement(By.CssSelector(btn_bluringviewer));
                    button.Click();
                }

                //wait for page to load
                PageLoadWait.WaitForPageLoad(60);
                PageLoadWait.WaitForFrameLoad(60);
            }
            else if (mode.Equals("ConferenceFolders"))
            {
                var button = BasePage.FindElementByCss(BluRingViewer.btn_bluringviewer_ConferenceFolder);
                button.Click();
                PageLoadWait.WaitForFrameLoad(60);
                //viewstudy = Driver.FindElement(By.CssSelector("input#ViewStudyButton")); 
            }
            else
            {
                PageLoadWait.WaitForPageLoad(5);
                if (showselector)
                {
                    if (SBrowserName.ToLower().Equals("internet explorer") || SBrowserName.ToLower().Equals("firefox"))
                        js.ExecuteScript("arguments[0].click()", Driver.FindElement(By.CssSelector(btn_bluringviewer_integrator)));
                    else
                        Driver.FindElement(By.CssSelector(btn_bluringviewer_integrator)).Click();
                    //BasePage.Driver.FindElement(By.CssSelector(btn_bluringviewer_integrator)).Click();
                }
                else
                {
                    bool Status = false;
                    int timeout = 0;

                    BasePage.Driver.Navigate().GoToUrl(url);
                    if(isRefreshForIntegrator == true)
                    {

                        while (!Status)
                        {
                            PageLoadWait.WaitForPageLoad(20);
                            timeout++;
                            try
                            {
                                Thread.Sleep(5000);
                                if (new StudyViewer().AuthenticationErrorMsg().Text.ToLower().Contains("there is another session open"))
                                {
                                    Driver.Navigate().Refresh();
                                }
                            }
                            catch (Exception) { Status = true; }
                            if (timeout >= 3)
                            {
                                break;
                            }
                        }
                    }
                    else { PageLoadWait.WaitForPageLoad(20); Thread.Sleep(5); }
                }
                PatientsStudy.NavigateToIntegratorFrame();
            }
            //wait for viewport to load
            WaitforViewports();

            //wait for thumbnails to load
            WaitforThumbnails(ThumbnailTimeout);

			//Wait for priors to Load
			WaitForPriorsToLoad();

            //Wait for linked scrolling to load
            WaitforLinkedScrolling();

            //Select Theme
            if (Config.Theme.ToLower().Equals("grey"))
            {   
                Driver.FindElement(By.CssSelector(userSettings_Icon)).Click();
                BasePage.wait.Until(ExpectedConditions.
                    ElementExists(By.CssSelector("blu-ring-global-settings div[class*='toolIconControl isToolActive']")));
                BasePage.FindElementByCss(".globalSettingPanel div ul:nth-of-type(2) li:nth-of-type(3)").Click();
                Thread.Sleep(2000);
            }

            return new BluRingViewer();
        }


       

        /// <summary>
        /// This method is to get the Tool's css property
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        public static String GetToolCss(BluRingTools tool)
        {
            String toolname = GetToolName(tool);
            return GetToolCss(toolname);
        }

        /// <summary>
        /// This method is to get the Tool's css property in first study panel
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        public static String GetToolCss(String tool)
        {

            return ("div[class*='tool-container']" + "[title^='" + tool + "']" + ">div[class^='toolIconContainer']");
        }

        /// <summary>
        /// This method is to get the Tool's css property
        /// </summary>
        /// <param name="panelnumber">should start from 1</param>
        /// <param name="seriesNumber">series number should start from 1</param>
        /// <param name="tool"></param>
        /// <returns></returns>
        private static String GetToolCss(int panelNum, int seriesNumber, String tool)
        {
            return (div_studypanel + ":nth-of-type(" + panelNum + ") " + "div.compositeViewerComponent div.viewerContainer:nth-of-type(" + seriesNumber + ")" + " " + GetToolCss(tool));
        }

        /// <summary>
        /// This method will get the Tool Tip Text in String
        /// </summary>
        /// <param name="tool"></param>
        /// <returns></returns>
        public static String GetToolName(BluRingTools tool)
        {
            String toolname = tool.ToString();
            String toolname1 = "";

            if (toolname.Contains("_"))
            {
                var toolnames = toolname.Split('_');
                int counter = 0;
                foreach (String t in toolnames)
                {
                    if (counter == 0)
                    { toolname1 = t; counter++; continue; }
                    toolname1 = toolname1 + " " + t;
                }

                toolname1 = toolname1.Replace('0', '/');
                return toolname1;
            }
            else
            {
                toolname1 = toolname1.Replace('0', '/');
                return toolname;
            }

        }

        /// <summary>
        /// This method is to open the viewer tool popup
        /// </summary>
        public void OpenViewerToolsPOPUp(bool js = true)
        {
            var viewport = BasePage.Driver.FindElement(By.CssSelector(this.Activeviewport));
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("edge"))
            {
                if (js)
                {
                    ContextClickUsingJS(viewport);
                }
                else
                {
                    new TestCompleteAction().ContextClick(viewport).Perform();
                }
            }
            else
            {
                Thread.Sleep(3000);
                new Actions(BasePage.Driver).ContextClick(viewport).Build().Perform();
                Thread.Sleep(3000);
            }
            Thread.Sleep(Config.minTimeout);
        }

        /// <summary>
        /// This method will select the mentioned viewer tool.
        /// </summary>
        /// <param name="toolname"></param>
        public Boolean SelectViewerTool(BluRingTools tool = BluRingTools.Pan, int panel = 1, int viewport = 1, bool isOpenToolsPOPup = true, bool isLocalization = false, string ToolName = null)
        {
            if (isOpenToolsPOPup)
                this.OpenViewerToolsPOPUp();

            //Set the tool property
            Thread.Sleep(500);
            String toolname = "";
            if (isLocalization && ToolName != null)
            {
                toolname = ToolName;
            }
            else
            {
                toolname = GetToolName(tool);
            }
            String tooltitlecss = "div[class*='tool-container']" + "[title='" + toolname + "']";
            //String tooltitlecss = "div[class*='tool-container']" + "[title='" + toolname + "']" + ">span.tool-caption";
            // String toolcaption = BasePage.Driver.FindElement(By.CssSelector(tooltitlecss)).GetAttribute("innerHTML");
            String csstool = GetToolCss(panel, viewport, toolname);
            IWebElement element = BasePage.Driver.FindElement(By.CssSelector(csstool));
            if(element != null)
            {
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("edge"))
                {
                    new TestCompleteAction().MoveToElement(element).Click().Perform();
                }
                else
                {
                    Thread.Sleep(1000);
                        new Actions(BasePage.Driver).Click(element).Build().Perform();
                    Thread.Sleep(4000);
                }
                Thread.Sleep(Config.ms_minTimeout);
                return true;
            }
            return false;

            //Validate tool caption or name
          //String expToolName = this.GetAttributeOfType<DefaultValueAttribute, Enum>(tool, "Value").Value.ToLower();
         //   return toolcaption.ToLower().Trim().Equals((expToolName)) ? true : false;
        }

        /// <summary>
        /// This method will select the mentioned viewer tool.
        /// </summary>
        /// <param name="toolname"></param>
        public Boolean SelectViewerToolByName(String toolName, int panel = 1, int viewport = 1)
        {
            String csstool = GetToolCss(panel, viewport, toolName);
            IWebElement element = BasePage.Driver.FindElement(By.CssSelector(csstool));
            if (element != null)
            {
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("edge"))
                {
                    new TestCompleteAction().Click(element).Perform();
                }
                else
                {
                    new Actions(BasePage.Driver).Click(element).Build().Perform();
                }
                Thread.Sleep(Config.ms_minTimeout);
                return true;
            }
            return false;
        }

        /// <summary>
        /// This method is to open the stacked tool popup
        /// Needs to be used when the outer tool name is dynamic
        /// </summary>
        /// <param name="outertool"></param>      
        public IWebElement OpenStackedTool(BluRingTools outertool, bool isOpenToolsPOPup = true, bool Contextclick = true, int panel = 1, int viewport = 1, bool js=true)
        {
            //Open viewer tools popup
            if (isOpenToolsPOPup)
                this.OpenViewerToolsPOPUp();

            //Find and Right click outer tool    
            if (!SBrowserName.ToLower().Contains("edge"))
            {
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(div_studypanel + ":nth-of-type(" + panel + ") " + "div.compositeViewerComponent div.viewerContainer:nth-of-type(" + viewport + ") " + div_toolboxContainer)));
            }
            var toolsconatiner = BasePage.Driver.FindElements(By.CssSelector(div_studypanel + ":nth-of-type(" + panel + ") " + "div.compositeViewerComponent div.viewerContainer:nth-of-type(" + viewport + ") " + div_toolsSetContainer));            
            IWebElement outer_tool = null;
            IWebElement tool_container = null;
            foreach (IWebElement toolcontainer in toolsconatiner)
            {
                outer_tool = toolcontainer.FindElement(By.CssSelector(div_activetoolsContainer));
                if (outer_tool.GetAttribute("title").Equals(GetToolName(outertool)))
                {
                    tool_container = toolcontainer;
                    //Right on outer tool   
                    if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("edge"))
                    {

                        if (Contextclick)
                        {
                            if (js)
                            {
                                ContextClickUsingJS(outer_tool);
                            }
                            else
                            {
                                new TestCompleteAction().ContextClick(outer_tool).Perform();
                            }
                        }
                        else
                        {
                            new TestCompleteAction().MoveToElement(outer_tool).ClickAndHold().Perform();
                        }
                    }
                    else
                    {
                        if (Contextclick)
                            new Actions(BasePage.Driver).ContextClick(outer_tool).Build().Perform();
                        else
                            new Actions(BasePage.Driver).ClickAndHold(outer_tool).Build().Perform();
                    }
                    outer_tool = toolcontainer.FindElement(By.CssSelector(div_activetoolsContainer));   //refreshing element
                    break;
                }
            }
            if (outer_tool == null) { throw new Exception("Outer Toool Not Found" + GetToolName(outertool)); }

            //Thread.Sleep(Config.minTimeout);
            wait.Until(d =>
            {
                Logger.Instance.InfoLog("Attribute state: " + tool_container.FindElement(By.CssSelector(div_expandedtoolsContainer)).GetAttribute("class").Contains("initalStackState"));
                return !(tool_container.FindElement(By.CssSelector(div_expandedtoolsContainer)).GetAttribute("class").Contains("initalStackState"));
            });

            //Ensure stack tools are opened
            IWebElement expandedtoolsContainer = tool_container.FindElement(By.CssSelector(div_expandedtoolsContainer));
            var timeout = new SystemClock();
            var synch = new DefaultWait<IWebElement>(expandedtoolsContainer, timeout);
            synch.Timeout = new TimeSpan(0, 0, 10);
            synch.Until<Boolean>(element => element.GetAttribute("style").ToLower().Replace(" ", "").Contains("opacity:1"));
            if (expandedtoolsContainer.GetAttribute("style").ToLower().Replace(" ", "").Contains("opacity:1"))
            {
                return expandedtoolsContainer;
            }
            return null;
        }

        /// <summary>
        /// This method is to select the innner tool
        /// Needs to be used when the outer tool name is dynamic
        /// </summary>
        /// <param name="outertool"></param>
        /// <param name="innnertool"></param>
        public Boolean SelectInnerViewerTool(BluRingTools innnertool, BluRingTools outertool, bool isOpenToolsPOPup = true, bool contextClick = true, int panel = 1, int viewport = 1)
        {
            IWebElement expandedtoolsContainer = this.OpenStackedTool(outertool, isOpenToolsPOPup, contextClick, panel, viewport);

            //Select inner tool
            if (expandedtoolsContainer != null)
            {              
                IWebElement ele = expandedtoolsContainer.FindElement(By.CssSelector(GetToolCss(innnertool)));
                if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("edge"))
                {
                    new TestCompleteAction().Click(ele).Perform();
                }
                else
                {
                    new Actions(BasePage.Driver).Click(ele).Build().Perform();
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Selects and Applies Horizontal Plumb Line measurement on the images
        /// </summary>
        public void ApplyTool_HorizontalPlumbLine(int dragStartX = 0, int dragStartY = 0)
        {
            if (dragStartX == 0 && dragStartY == 0)
            {
                var attributes = GetElementAttributes(this.Activeviewport);
                dragStartX = attributes["width"] / 2;
                dragStartY = attributes["height"] / 2;
                new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dragStartX, dragStartY).Build().Perform();
                Thread.Sleep(2000);
                new Actions(BasePage.Driver).Click().Build().Perform();
            }
            else
            {
                new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dragStartX, dragStartY).Build().Perform();
                Thread.Sleep(2000);
                new Actions(BasePage.Driver).Click().Build().Perform();
            }
        }


        /// <summary>
        /// Selects and Applies Vertical Plumb Line measurement on the images
        /// </summary>
        public void ApplyTool_VerticalPlumbLine(int dragStartX = 0, int dragStartY = 0)
        {
            if (dragStartX == 0 && dragStartY == 0)
            {
                var attributes = GetElementAttributes(this.Activeviewport);
                dragStartX = attributes["width"] / 2;
                dragStartY = attributes["height"] / 2;
                new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dragStartX, dragStartY).Build().Perform();
                Thread.Sleep(2000);
                new Actions(BasePage.Driver).Click().Build().Perform();
            }
            else
            {
                new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dragStartX, dragStartY).Build().Perform();
                Thread.Sleep(2000);
                new Actions(BasePage.Driver).Click().Build().Perform();

            }
        }

        /// <summary>
        /// Ths helper method will simulate user drag drop operation on given view port
        /// </summary>
        /// <param name="dragX"></param>
        /// <param name="dragY"></param>
        /// <param name="dropX"></param>
        /// <param name="dropY"></param>
        /// <param name="viewportCSS"></param>
        public void PlumbLineMove(int dragX, int dragY, int dropX, int dropY, bool isReleaseDrag = true)
        {
            if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("chrome")) ||
            (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
            {
                //Drag and Drop in X and Y direction
                new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dragX, dragY).Release().Build().Perform();
                Thread.Sleep(2000);
                new Actions(BasePage.Driver).ClickAndHold().Build().Perform();
                Thread.Sleep(2000);
                new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dropX, dropY).Build().Perform();
                Thread.Sleep(2000);

                if (isReleaseDrag)
                    new Actions(BasePage.Driver).Release().Build().Perform();
            }
            else
            {
                // Drag and Drop using TestComplete Actions
                var actions = new TestCompleteAction();
                actions.MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dragX, dragY);
                //actions.ClickAndHold();
                actions.Click();
                actions.ClickAndHold();
                Thread.Sleep(2000);
                actions.MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dropX, dropY);
                Thread.Sleep(2000);

                if (isReleaseDrag)
                    actions.Release();

                actions.Perform();

            }
        }

        /// <summary>
        /// This method is used to delete annotation
        /// </summary>
        /// <returns></returns>
        public void ToDeletAnnotation(IWebElement element, int locationx = 0, int locationy = 0)
        {
            if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("chrome")) ||
            (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
            {
                new Actions(BasePage.Driver).MoveToElement(element, locationx, locationy).Build().Perform();
                Thread.Sleep(2000);
                new Actions(BasePage.Driver).ContextClick().Build().Perform();
            }
            else
            {
                new TestCompleteAction().MoveToElement(element, locationx, locationy).Perform();
                Thread.Sleep(2000);
                new TestCompleteAction().ContextClick().Perform();
            }

        }
        /// <summary>
        /// Wait till all Thumbnails in viewport completed loading
        /// </summary>
        public static void WaitforThumbnails(int timeout = 180)
        {
            //Wait Obejcts
            var thumbnailwait_toappear = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 30));
            thumbnailwait_toappear.IgnoreExceptionTypes(new Type[] { (new StaleElementReferenceException().GetType()) });
            var thumbnailwait_disappear = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, timeout));
            thumbnailwait_disappear.IgnoreExceptionTypes(new Type[] { (new StaleElementReferenceException().GetType()) });

            //Wait Till Thumbnail Loading Text Appears
            try
            {
                thumbnailwait_toappear.Until<Boolean>((d) =>
                      {
                          Logger.Instance.InfoLog("Inside Block Wait Till Thumbnail Loading Text Appears");
                          var thumbnails = d.FindElements(By.CssSelector(div_thumbnailloadingstatus));
                          bool isThumbnailLoading = false;
                          foreach (IWebElement thumbnail in thumbnails)
                          {
                              if (thumbnail.GetAttribute("innerHTML").ToLower().Contains("loading"))
                              {
                                  Logger.Instance.InfoLog("Thumbnails are Loading..");
                                  isThumbnailLoading = true;
                                  return true;
                              }
                              else
                              {
                                  continue;
                              }
                          }
                          if (isThumbnailLoading) { return true; } else { return false; }
                      });
            }
            catch (Exception) { Logger.Instance.InfoLog("In Catch ..Waiting for Thumbnail Loadin.."); }


			//Wait Till Thumbnails Loading Text disappears
			try
			{
				thumbnailwait_disappear.Until<Boolean>((d) =>
				{
					Logger.Instance.InfoLog("Inside Block Wait Till Thumbnail Loading Text DisAppears");
					var thumbnails = d.FindElements(By.CssSelector(div_thumbnailloadingstatus));
					bool isThumbnailLoading = false;
					foreach (IWebElement thumbnail in thumbnails)
					{
						if (thumbnail.GetAttribute("innerHTML").ToLower().Contains("loading"))
						{
							Logger.Instance.InfoLog("Thumbnails are still loading");
							isThumbnailLoading = true;
							return false;
						}
						else
						{
							continue;
						}
					}
					if (isThumbnailLoading) { return false; } else { return true; }
				});
			}
			catch (Exception) { Logger.Instance.InfoLog("In Catch ..Waiting for Thumbnail Loadin.. to disapperar"); }
		}

        /// <summary>
        /// Wait for a all viewporst to load completly in different study panels.
        /// </summary>
        public static void WaitforViewports(int timeout = 180)
        {
            if(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("edge"))
            {
                try
                {
                    BasePage.Driver.SwitchTo().Window(BasePage.Driver.CurrentWindowHandle);
                    PageLoadWait.WaitForFrameLoad(10);
                }
                catch (Exception ex)
                {

                }
            }
            
            WebDriverWait wait1 = new WebDriverWait(BasePage.Driver, TimeSpan.FromSeconds(timeout));
            wait1.Until<Boolean>((d) =>
            {
                var viewports = BasePage.Driver.FindElements(By.CssSelector(div_allViewportes));
                foreach (IWebElement viewport in viewports)
                {
                    if (!viewport.GetAttribute("style").Contains("display: none"))
                        return true;
                    else
                        return false;
                }
                return false;
            });
        }

        /// <summary>
        /// Wait for linked scrolling to load in all viewports
        /// </summary>
        public static void WaitforLinkedScrolling(int timeout = 400)
        {
            try
            {
                WebDriverWait wait1 = new WebDriverWait(BasePage.Driver, TimeSpan.FromSeconds(timeout));
                wait1.Until<Boolean>((d) =>
                {
                    var linkScrollLoading = BasePage.Driver.FindElements(By.CssSelector(div_allLinkScrollLoadingInProgress));
                    if (linkScrollLoading.Count > 0)
                        return false;
                    else
                        return true;
                });
            }
            catch (Exception e)
            {
                Logger.Instance.InfoLog("Exception while waiting for linked scrolling icon to load - " + e);
            }

        }

        /// <summary>
        /// This method will wait till priors loads in the Exam List
        /// </summary>
        public static void WaitForPriorsToLoad()
        {
            //Wait till Loading symbol to appear
            IWebElement loadingdiv = null;
            int timeout = 0;
            do
            {
                try
                {
                    Thread.Sleep(5000);
                    timeout++;
                    if ((loadingdiv = BasePage.Driver.FindElement(By.CssSelector("div[class*='relatedStudyStatus']>div"))) != null)
                    {
                        Logger.Instance.InfoLog("Priors Loading symbol appeared");
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Priors Loading Symbol not present, it coule be loaded already");

                    }
                }
                catch (Exception) { Logger.Instance.InfoLog("Priors Loading Symbol not present, it could be loaded alrady"); }
            } while (loadingdiv == null && timeout < 1);

            //Wait till Loading symbol to disappears
            loadingdiv = null;
            timeout = 0;
            do
            {
                try
                {
                    Thread.Sleep(10000);
                    timeout++;
                    if ((loadingdiv = BasePage.Driver.FindElement(By.CssSelector("div[class*='relatedStudyStatus']>div"))) != null)
                    {
                        Logger.Instance.InfoLog("Priors Loading div still present");

                    }
                    else
                    {
                        Logger.Instance.InfoLog("Priors Loading Symbol not present");

                    }
                }
                catch (Exception) { loadingdiv = null; }
            } while (loadingdiv != null && timeout < 20);


            //Wait till Exam load div shows up studies
            BasePage.wait.Until<Boolean>(d =>
            {
                if (d.FindElement(By.CssSelector(div_ContainerPriors)).GetAttribute("style").Contains("height:"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });

            Thread.Sleep(5000);
        }

        /// <summary>
        /// Ths helper method will simulate user drag drop operation on given view port
        /// </summary>
        /// <param name="dragX"></param>
        /// <param name="dragY"></param>
        /// <param name="dropX"></param>
        /// <param name="dropY"></param>
        /// <param name="viewportCSS"></param>
        public void PerformTool(int dragX, int dragY, int dropX, int dropY, bool isReleaseDrag = true, bool clickOnly = false, bool isEdit = false, bool isInteractivetool = false)
        {
            if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("chrome")) ||
            (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
            {
                //Drag and Drop in X and Y direction
                new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dragX, dragY).Build().Perform();
                Thread.Sleep(2000);
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("chrome")) && !isInteractivetool)
                    new Actions(BasePage.Driver).Click().Build().Perform();
                new Actions(BasePage.Driver).ClickAndHold().Build().Perform();
                Thread.Sleep(2000);
                new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dropX, dropY).Build().Perform();
                Thread.Sleep(2000);

                if (isReleaseDrag)
                    new Actions(BasePage.Driver).Release().Build().Perform();

                if (isEdit)
                    new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), 1, 1).Build().Perform();
            }
            else
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("edge"))
            {
                var actions = new TestCompleteAction();
                actions.MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dragX, dragY).Perform();
                Thread.Sleep(2000);
                actions.ClickAndHold().Perform();
                Thread.Sleep(2000);
                actions.MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dropX, dropY).Perform();
                Thread.Sleep(2000);

                if (isReleaseDrag)
                    actions.Release().Perform();
            }
            else
            {
                // Drag and Drop using TestComplete Actions
                var actions = new TestCompleteAction();
                actions.MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dragX, dragY);
                if (clickOnly == true)
                    actions.Click();
                else
                {
                    actions.Click();
                    actions.ClickAndHold();
                }

                Thread.Sleep(2000);
                actions.MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dropX, dropY);
                Thread.Sleep(2000);

                if (isReleaseDrag)
                    actions.Release();

                if (isEdit)
                    actions.MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), 0, 0);

                actions.Perform();


            }
        }

        /// <summary>
        /// Returns the attribute of the WebElement
        /// </summary>
        /// <param name="cssSelector"></param>
        /// <returns></returns>
        public IDictionary<String, int> GetElementAttributes(String cssSelector)
        {
            var attributes = new Dictionary<String, int>();
            var element = BasePage.Driver.FindElement(By.CssSelector(cssSelector));
            attributes.Add("xcoordinate", element.Location.X);
            attributes.Add("ycoordinate", element.Location.Y);
            attributes.Add("width", element.Size.Width);
            attributes.Add("height", element.Size.Height);

            return attributes;
        }

        /// <summary>
        /// Selects and Applies pan on the images
        /// Testedin Chrome-58
        /// </summary>
        public void ApplyTool_Pan(int dragStartX = 0, int dragStartY = 0, int dropX = 0, int dropY = 0)
        {
            if (dragStartX == 0 && dragStartY == 0 && dropX == 0 && dropY == 0)
            {
                var attributes = GetElementAttributes(this.Activeviewport);
                dragStartX = attributes["width"] / 2;
                dragStartY = attributes["height"] / 2;
                dropX = attributes["width"] / 4;
                dropY = attributes["height"] / 4;
            }

            this.PerformTool(dragStartX, dragStartY, dropX, dropY,isInteractivetool: true);
            Thread.Sleep(Config.ms_minTimeout);
        }

        public void ApplyTool_Magnifier(bool IsIncrementalZoom = true, int dragStartX = 0, int dragStartY = 0, int dropX = 0, int dropY = 0)
        {
            if (dragStartX == 0 && dragStartY == 0 && dropX == 0 && dropY == 0)
            {
                var attributes = GetElementAttributes(this.Activeviewport);
                dragStartX = attributes["width"] / 2;
                dragStartY = attributes["height"] / 2;
                dropX = attributes["width"] / 4;
                dropY = attributes["height"] / 4;
            }
            IWebElement ele = Driver.FindElement(By.CssSelector(Activeviewport));

            if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("chrome")) ||
            (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
            {
                if (!IsIncrementalZoom)
                {
                    //Drag and Drop in X and Y direction
                    new Actions(BasePage.Driver).MoveToElement(ele, dragStartX, dragStartY).Build().Perform();
                    Thread.Sleep(2000);
                    new Actions(BasePage.Driver).ClickAndHold().Build().Perform();
                    Thread.Sleep(2000);
                }
                new Actions(BasePage.Driver).MoveToElement(ele, dropX, dropY).Build().Perform();
                Thread.Sleep(2000);
            }
            else if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox")) ||
                    (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("edge")))
            {
                TestCompleteAction action = new TestCompleteAction();
                if (!IsIncrementalZoom)
                {
                    action.MoveToElement(ele, dragStartX, dragStartY);
                    Thread.Sleep(2000);
                    action.Click();
                    action.ClickAndHold();
                    Thread.Sleep(2000);
                }
                action.MoveToElement(ele, dropX, dropY).Perform();
                Thread.Sleep(2000);
            }

            Thread.Sleep(Config.ms_minTimeout);
        }

        /// <summary>
        /// This is to Exit the Maginifier tool
        /// </summary>
        public void ExitTool_Magnifier()
        {
            
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("edge"))
            {
                new TestCompleteAction().Release().Perform();
            }
            else
            {
                new Actions(BasePage.Driver).Release().Build().Perform();
            }
            
            Thread.Sleep(Config.ms_minTimeout);
        }

        /// <summary>
        /// Flip the image in Horizontal
        /// </summary>
        public void ApplyTool_FlipHorizontal()
        {
            this.SelectViewerTool(BluRingTools.Flip_Horizontal);
            Thread.Sleep(Config.ms_minTimeout);
        }

        /// <summary>
        /// Flip Image vertical
        /// </summary>
        public void ApplyTool_FlipVertical()
        {
            this.SelectInnerViewerTool(BluRingTools.Flip_Vertical, BluRingTools.Flip_Horizontal);
            Thread.Sleep(Config.ms_minTimeout);
        }

        /// <summary>
        /// Rotate image clockwise
        /// </summary>
        public Boolean ApplyTool_RotateClockwise()
        {
            //var toolnamecheck = this.SelectInnerViewerTool(BluRingTools.Rotate_Clockwise, BluRingTools.Flip_Horizontal);
            var toolnamecheck = this.SelectViewerTool(BluRingTools.Rotate_Clockwise);
            Thread.Sleep(Config.ms_minTimeout);
            return toolnamecheck;
        }

        /// <summary>
        /// Rotate Counter Clockwise
        /// </summary>
        public void ApplyTool_RotateCClockwise()
        {
            //this.SelectInnerViewerTool(BluRingTools.Rotate_Counterclockwise, BluRingTools.Flip_Horizontal);
            this.SelectInnerViewerTool(BluRingTools.Rotate_Counterclockwise, BluRingTools.Rotate_Clockwise);
            Thread.Sleep(Config.ms_minTimeout);
        }

        /// <summary>
        /// Apply window width
        /// </summary>
        public void ApplyTool_WindowWidth(int dragStartX = 0, int dragStartY = 0, int dropX = 0, int dropY = 0)
        {
            if (dragStartX == 0 && dragStartY == 0 && dropX == 0 && dropY == 0)
            {
                var attributes = GetElementAttributes(this.Activeviewport);
                dragStartX = attributes["width"] / 2;
                dragStartY = attributes["height"] / 2;
                dropX = (attributes["width"] / 2) + 10;
                dropY = (attributes["height"] / 2) + 10;
            }
            this.PerformTool(dragStartX, dragStartY, dropX, dropY, true, isInteractivetool: true);
            Thread.Sleep(Config.ms_minTimeout);

        }

        /// <summary>
        /// Line measurement tool
        /// </summary>
        public void ApplyTool_LineMeasurement(int dragStartX = 0, int dragStartY = 0, int dropX = 0, int dropY = 0)
        {
            if (dragStartX == 0 && dragStartY == 0 && dropX == 0 && dropY == 0)
            {
                var attributes = GetElementAttributes(this.Activeviewport);
                dragStartX = attributes["width"] / 2;
                dragStartY = attributes["height"] / 2;
                dropX = attributes["width"] / 4;
                dropY = attributes["height"] / 4;
            }
            this.PerformTool(dragStartX, dragStartY, dropX, dropY, clickOnly: true);
            Thread.Sleep(Config.ms_minTimeout);
        }

        /// <summary>
        /// Calibration tool
        /// </summary>
        public void ApplyTool_Calibration(int number, int dragStartX = 0, int dragStartY = 0, int dropX = 0, int dropY = 0, bool acceptCal = true)
        {
            if (dragStartX == 0 && dragStartY == 0 && dropX == 0 && dropY == 0)
            {
                var attributes = GetElementAttributes(this.Activeviewport);
                dragStartX = attributes["width"] / 2;
                dragStartY = attributes["height"] / 2;
                dropX = attributes["width"] / 4;
                dropY = attributes["height"] / 4;
            }

            this.PerformTool(dragStartX, dragStartY, dropX, dropY);
            Thread.Sleep(Config.ms_minTimeout);

            //Enter the Pixel to mm calibration
            BasePage.wait.Until<Boolean>(d => d.FindElement(By.CssSelector("input[type='text']")) != null);
            BasePage.Driver.FindElement(By.CssSelector(this.Activeviewport)).FindElement(By.CssSelector("input[type='text']")).SendKeys(number.ToString());
            if (acceptCal)
                BasePage.Driver.FindElement(By.CssSelector(this.Activeviewport)).FindElement(By.CssSelector("input[type='text']")).SendKeys(Keys.Enter);
            else
                BasePage.Driver.FindElement(By.CssSelector(this.Activeviewport)).FindElement(By.CssSelector("input[type='text']")).SendKeys(Keys.Escape);
        }

        /// <summary>
        /// Apply Calibration with alphabets
        /// </summary>
        public void ApplyTool_Calibration(String text, int dragStartX = 0, int dragStartY = 0, int dropX = 0, int dropY = 0)
        {
            if (dragStartX == 0 && dragStartY == 0 && dropX == 0 && dropY == 0)
            {
                var attributes = GetElementAttributes(this.Activeviewport);
                dragStartX = attributes["width"] / 2;
                dragStartY = attributes["height"] / 2;
                dropX = attributes["width"] / 4;
                dropY = attributes["height"] / 4;
            }

            this.PerformTool(dragStartX, dragStartY, dropX, dropY);
            Thread.Sleep(Config.ms_minTimeout);

            BasePage.wait.Until<Boolean>(d => d.FindElement(By.CssSelector("input[type='text']")) != null);
            BasePage.Driver.FindElement(By.CssSelector(this.Activeviewport)).FindElement(By.CssSelector("input[type='text']")).SendKeys(text);
            BasePage.Driver.FindElement(By.CssSelector(this.Activeviewport)).FindElement(By.CssSelector("input[type='text']")).SendKeys(Keys.Enter);

        }

        /// <summary>
        /// Cobb Angle tool
        /// </summary>
        public void ApplyTool_CobbAngle(int dragStartX = 0, int dragStartY = 0, int dropX = 0, int dropY = 0)
        {
            if (dragStartX == 0 && dragStartY == 0 && dropX == 0 && dropY == 0)
            {
                var attributes = GetElementAttributes(this.Activeviewport);
                dragStartX = attributes["width"] / 2;
                dragStartY = attributes["height"] / 2;
                dropX = attributes["width"] / 4;
                dropY = attributes["height"] / 4;
            }

            this.PerformTool(dragStartX, dragStartY, dropX, dropY);
            this.PerformTool(dragStartX / 2, dragStartY / 2, dropX / 2, ((dragStartY / 2) + 20));
            Thread.Sleep(Config.ms_minTimeout);

        }

        /// <summary>
        /// Free Draw Tool
        /// </summary>
        public void ApplyTool_FreeDraw(int dragStartX = 0, int dragStartY = 0, int dropX = 0, int dropY = 0)
        {
            if (dragStartX == 0 && dragStartY == 0 && dropX == 0 && dropY == 0)
            {
                var attributes = GetElementAttributes(this.Activeviewport);
                dragStartX = attributes["width"] / 2;
                dragStartY = attributes["height"] / 2;
                dropX = attributes["width"] / 4;
                dropY = attributes["height"] / 4;
            }

            this.PerformTool(dragStartX, dragStartY, dropX, dropY);
            Thread.Sleep(Config.ms_minTimeout);

        }

        /// <summary>
        /// Draw Ellipse
        /// </summary>
        public void ApplyTool_DrawEllipse(int dragStartX = 0, int dragStartY = 0, int dropX = 0, int dropY = 0)
        {
            if (dragStartX == 0 && dragStartY == 0 && dropX == 0 && dropY == 0)
            {
                var attributes = GetElementAttributes(this.Activeviewport);
                dragStartX = attributes["width"] / 2;
                dragStartY = attributes["height"] / 2;
                dropX = attributes["width"] / 4;
                dropY = attributes["height"] / 4;
            }

            this.PerformTool(dragStartX, dragStartY, dropX, dropY);
            Thread.Sleep(Config.ms_minTimeout);
        }

        /// <summary>
        /// Draw Rectangle tool
        /// </summary>
        public void ApplyTool_DrawRectangle(int dragStartX = 0, int dragStartY = 0, int dropX = 0, int dropY = 0, bool isClick = true)
        {
            if (dragStartX == 0 && dragStartY == 0 && dropX == 0 && dropY == 0)
            {
                var attributes = GetElementAttributes(this.Activeviewport);
                dragStartX = attributes["width"] / 2;
                dragStartY = attributes["height"] / 2;
                dropX = attributes["width"] / 4;
                dropY = attributes["height"] / 4;
            }

            if (isClick)
            {
                this.PerformTool(dragStartX, dragStartY, dropX, dropY);
            }
            else
            {
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("chrome")) ||
                (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
                {
                    new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dragStartX, dragStartY).Build().Perform();
                    Thread.Sleep(2000);
                    new Actions(BasePage.Driver).Click().Build().Perform();
                    Thread.Sleep(2000);
                    new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dropX, dropY).Build().Perform();
                    Thread.Sleep(2000);
                    new Actions(BasePage.Driver).Click().Build().Perform();

                }
                else
                {
                    var action = new TestCompleteAction();
                    action.MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dragStartX, dragStartY);
                    Thread.Sleep(2000);
                    action.Click();
                    Thread.Sleep(2000);
                    action.MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dropX, dropY).Click().Perform();
                    Thread.Sleep(2000);
                    ScrollIntoView(GetElement("cssselector", div_closeStudy));
                }

            }
            Thread.Sleep(Config.ms_minTimeout);
        }

        /// <summary>
        /// Angle Measurement
        /// </summary>
        public void ApplyTool_AngleMeasurement(int dragStartX = 0, int dragStartY = 0, int dropX = 0, int dropY = 0)
        {
            if (dragStartX == 0 && dragStartY == 0 && dropX == 0 && dropY == 0)
            {
                var attributes = GetElementAttributes(this.Activeviewport);
                dragStartX = attributes["width"] / 2;
                dragStartY = attributes["height"] / 2;
                dropX = attributes["width"] / 4;
                dropY = attributes["height"] / 4;
            }

            this.PerformTool(dragStartX, dragStartY, dropX, dropY);
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("edge"))
            {
                new TestCompleteAction().MoveToElement(BasePage.Driver.FindElement(By.CssSelector(this.Activeviewport)), dragStartX, dropY).Perform();
                Thread.Sleep(2000);
                new TestCompleteAction().Click().Perform();
            }
            else if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("firefox"))
            {
                new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(this.Activeviewport)), dragStartX, dropY).Build().Perform();
                Thread.Sleep(2000);
                new Actions(BasePage.Driver).Click().Build().Perform();
            }
            else
            {
                new TestCompleteAction().MoveToElement(BasePage.Driver.FindElement(By.CssSelector(this.Activeviewport)), dragStartX, dropY).Perform();
                Thread.Sleep(2000);
                new TestCompleteAction().Click().Perform();
            }
            Thread.Sleep(Config.ms_minTimeout);
        }

        /// <summary>
        /// Closing the Bluring viewer
        /// </summary>		
        public void CloseBluRingViewer()
        {
            PageLoadWait.WaitForFrameLoad(1);
            if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("chrome")))
            {
                this.ClickElement(BasePage.Driver.FindElement(By.CssSelector(div_closeStudy)));
            }
            else
            {
                BasePage.Driver.FindElement(By.CssSelector(div_closeStudy)).Click();
                PageLoadWait.WaitForPageLoad(10);
            }

        }

        /// <summary>
        /// Open the Help Tool
        /// </summary>
        /// <returns></returns>
        public OnlineHelp OpenOnlineHelp()
        {
            if (BasePage.SBrowserName.ToLower().Contains("explorer") || BasePage.SBrowserName.ToLower().Contains("edge"))
            
            {
                this.ClickElement(BasePage.Driver.FindElement(By.CssSelector(div_HelpTool)));
                this.ClickElement(BasePage.Driver.FindElement(By.CssSelector(div_Help)));               
            }
            else
            {
                IWebElement ele = this.GetElement(BasePage.SelectorType.CssSelector, div_HelpTool);
                new Actions(BasePage.Driver).Click(ele).Build().Perform(); Thread.Sleep(1000);
                IWebElement ele1 = this.GetElement(BasePage.SelectorType.CssSelector, div_Help);
                new Actions(BasePage.Driver).Click(ele1).Build().Perform(); Thread.Sleep(1000);
            }
            return new OnlineHelp();
        }

        /// <summary>
        /// Pixel Value Measurement
        /// </summary>
        public void ApplyTool_PixelValue(int dragStartX = 0, int dragStartY = 0, int dropX = 0, int dropY = 0)
        {
            var attributes = GetElementAttributes(this.Activeviewport);
            dragStartX = attributes["width"] / 2;
            dragStartY = attributes["height"] / 2;
            dropX = (Int32)(attributes["width"] / 1.5);
            dropY = (Int32)(attributes["height"] / 1.5);
            this.PerformTool(dragStartX, dragStartY, dropX, dropY, isReleaseDrag: false);
            Thread.Sleep(Config.ms_minTimeout);
        }

        /// <summary>
        /// Open priors by sequence
        /// </summary>
        public void OpenPriors(int sequence = 0, String eventType = "click", string StudyDate = null, string StudyTime = null, String accession = null)
        {
            Thread.Sleep(2000);
            var priors = BasePage.Driver.FindElements(By.CssSelector(div_priors));

            if (accession != null)
            {
                foreach (IWebElement prior in priors)
                {
                    var accession_ele = prior.FindElement(By.CssSelector(AccessionNumberInExamList));
                    if (this.GetAccession(accession_ele).Equals(accession))
                    {
                        this.ClickElement(prior);
                        break;
                    }

                }
            }

            else if (StudyDate == null)
            {
                if (eventType.Equals("click"))
                {
                    if (SBrowserName.ToLower().Equals("internet explorer") || SBrowserName.ToLower().Contains("edge"))
                    {
                        var js = (IJavaScriptExecutor)Driver;
                        js.ExecuteScript("arguments[0].click()", priors[sequence]);
                    }
                    else
                    {
                        priors[sequence].Click();
                    }
                }
                else
                    this.DoubleClick(priors[sequence]);
            }

            else
            {
                IList<IWebElement> PriorStudyDate = Driver.FindElements(By.CssSelector(div_examListPanelDate));

                foreach (IWebElement Date in PriorStudyDate)
                {
                    if (StudyTime == null)
                    {
                        if (Date.GetAttribute("innerHTML").ToLower().Equals(StudyDate.ToLower()))
                        {
                            ClickElement(Date);
                            break;
                        }
                    }
                    else
                    {
                        string Time = Date.FindElement(By.XPath("..")).FindElement(By.CssSelector(div_priorTime)).GetAttribute("innerHTML").ToLower();
                        if (Date.GetAttribute("innerHTML").ToLower().Equals(StudyDate.ToLower()) && Time.Contains(StudyTime.ToLower()))
                        {
                            ClickElement(Date);
                            break;
                        }

                    }
                }

            }
            WaitforViewports();
            WaitforThumbnails();
            WaitforLinkedScrolling();
        }

        /// <summary>
        /// Open priors by sequence
        /// </summary>
        public int CheckPriorsCount()
        {
            var priors = BasePage.Driver.FindElements(By.CssSelector(div_priors));
            return priors.Count();
        }

        /// <summary>
        /// This Method is used to verify the border of stackedtool border
        /// </summary>
        /// <param name="Toolscss"></param>
        /// <returns></returns>
        public bool VerifyStackedToolBorder()
        {
            bool IsBorderAvailable = false;
            string background = BasePage.Driver.FindElement(By.CssSelector(div_expandedtoolsContainer)).GetCssValue("background-color");
            IsBorderAvailable = background.Equals("rgba(42, 42, 42, 1)");
            if (SBrowserName.ToLower().Equals("firefox"))
                IsBorderAvailable = background.Equals("rgba(42, 42, 42, 0.2)");
            if (SBrowserName.ToLower().Contains("edge"))
                IsBorderAvailable = background.Equals("rgba(42, 42, 42, 0.2)");
            return IsBorderAvailable;
        }

        /// <summary>
        /// Filter Studies in Exam list either by modality
        /// </summary>
        /// <param name="filtertype"></param>
        public void FilterPriors(String filtervalue, String filtertype = "modality")
        {

          //Click open the list
          this.OpenModalityFilter();

          //Select Modality
          this.SelectModalityValue(filtervalue);

          //Close the list      
          this.CloseModalityFilter();

        }

        /// <summary>
        /// Opens the modlaity filter
        /// </summary>
        public void OpenModalityFilter()
        {
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("chrome"))
            {
              var multiselect = this.GetElement(BasePage.SelectorType.CssSelector, div_multiSelect_Modality);
              multiselect.Click();
            }
            else
            {
                var arrow = BasePage.Driver.FindElement(By.CssSelector("#mat-select-0 > div:nth-child(1) > div"));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", arrow);
            }
            Thread.Sleep(3000);
            BasePage.wait.Until<Boolean>(d => d.FindElement(By.CssSelector(div_modalityFilterPopup)) != null);
        }

        /// <summary>
        /// This method will validate the priors filtered based on Modality filter
        /// </summary>
        /// <param name="modalities_filtered"></param>
        /// <returns></returns>
        public Boolean ValidateModalityFiltered(IList<String> modalities_filtered)
        {
            Boolean isModalityFilteredCorrctly = false;
            IList<String> priors_modality = new List<String>();
            IList<String>  priors_modality_distinct = new List<String>();
            var priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));            
            modalities_filtered.Remove("All");

            foreach (IWebElement prior in priors)
            {
                String priormodality = prior.FindElement(By.CssSelector(BluRingViewer.div_priorModality)).GetAttribute("innerHTML").Trim();
                var examListModalities = priormodality.Split(',').ToList<String>();
                examListModalities.Remove("PR");
                examListModalities.Remove("KO");

                //Check each modality in exam list is in dropdown filter except PR
                if (!examListModalities.All<String>(modality =>
                {   
                    if (modalities_filtered.Contains(modality))
                    {
                        Logger.Instance.ErrorLog("Exam Modality--"+modality+" found Modality Filter Drop Down");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.ErrorLog("Exam Modality--"+modality+" Not found Modality Filter Drop Down");
                        return false;
                    }
                }))                
                {
                    return false;
                }                

                //Get all exam list modality
                foreach (var mod in examListModalities) { priors_modality.Add(mod); }

            }

            //Check each modality in modality filter dropdown is present in exam list card
            priors_modality.Remove("PR");
            priors_modality_distinct = priors_modality.Distinct<String>().ToList();
            isModalityFilteredCorrctly = modalities_filtered.All<String>(mod =>
            {
                if(priors_modality_distinct.Contains(mod))
                {
                    Logger.Instance.ErrorLog("Dropdown Modality--" + mod + " found in exam card modality");
                    return true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Dropdown Modality--" + mod + " NOt found in exam card modality");
                    return false;
                }               

            });

            return isModalityFilteredCorrctly;
        }


        /// <summary>
        /// This method is to query if a Modality is selected in Modality Filter dropdownbox
        /// </summary>
        /// <param name="modality"> Modality to Query</param>
        public bool IsModalitySelected(String modality)
        {
            var options = BasePage.Driver.FindElements(By.CssSelector(modality_options_ele));

            foreach (var option in options)
            {
                if (option.FindElement(By.CssSelector(BluRingViewer.modality_options_text)).GetAttribute("innerHTML").Equals(modality))
                {
                    return option.GetAttribute("aria-selected").Equals("true");
                }
            }

            return false;
        }


        /// <summary>
        /// This method is to Select or unselect modality filter
        /// </summary>
        /// <param name="modality"> Modality to Select</param>
        /// <param name="unselect">flag to either Select or UnSelect</param>
        public void SelectModalityValue(String modality, Boolean unselect = false)
        {
            var options = BasePage.Driver.FindElements(By.CssSelector(modality_options_ele));
            var select_unselect_flagtext = unselect ? "true" : "false";

            foreach(var option in options)
            {
                if (option.GetAttribute("aria-selected").ToLower().Equals(select_unselect_flagtext))
                {
                    if (option.FindElement(By.CssSelector(BluRingViewer.modality_options_text)).GetAttribute("innerHTML").Equals(modality))
                    {
                        var checkbox = option.FindElement(By.CssSelector(modality_checkbox));
                        checkbox.Click();
                    }
                }
            }

            Thread.Sleep(2000);

        }

        /// <summary>
        /// UnSelect all Modalities in the filter
        /// </summary>
        /// <param name="modalities">List of modalities to unselect</param>
        public void UnSelectAllModalities(String[] modalities)
        {
            foreach (var modality in modalities)
            {
                this.SelectModalityValue(modality, unselect: true);
            }
        }

        /// <summary>
        ///  Close Modality Filter
        /// </summary>
        public void CloseModalityFilter()
        {

            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("edge"))
            {
                new TestCompleteAction().Click(this.GetElement(BasePage.SelectorType.CssSelector, Activeviewport)).Perform();
            }
            else 
            {
                new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab).Build().Perform();
            }            
            
            //Synch up
            BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(div_modalityFilterPopup)));
        }

        /// <summary>
        ///  Close Exam Filter
        /// </summary>
        public void CloseSortByFilter()
        {

            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("edge"))
            {
                new TestCompleteAction().Click(this.GetElement(BasePage.SelectorType.CssSelector, Activeviewport)).Perform();
            }
            else
            {              
             new Actions(BasePage.Driver).SendKeys(OpenQA.Selenium.Keys.Tab).Build().Perform();  
             Thread.Sleep(3000);
            }

            BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(div_SelectSort)));
        }

        /// <summary>
        /// Sort Studies in Exam list either by modality or instituition
        /// </summary>
        /// <param name="filtertype"></param>
        public void SortPriors(String value)
        {
            //Open sort drop down
            this.OpenSortDorpdown();

            //Select Value
            this.SelectValue_SortDropdown(value);
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("edge"))
            {
                new TestCompleteAction().Click(this.GetElement(BasePage.SelectorType.CssSelector, Activeviewport)).Perform();
            }
            else
            {
                new Actions(BasePage.Driver).Click(this.GetElement(BasePage.SelectorType.CssSelector, Activeviewport)).Build().Perform();
            }
            //Synch up
            Thread.Sleep(2000);

        }

        /// <summary>
        /// Open the Exam List sort drop down
        /// </summary>
        public void OpenSortDorpdown()
        {
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("chrome"))
            {
                var dropdown = this.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_SelectSort);
                dropdown.Click();
            }
            else
            {
                var arrow = BasePage.Driver.FindElement(By.CssSelector("#mat-select-1 > div:nth-child(1) > div"));
                ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("arguments[0].click()", arrow);
            }
            Thread.Sleep(3000);
            BasePage.wait.Until<Boolean>(d => d.FindElement(By.CssSelector(div_SortPopUp)) != null);

            //Synch up
            BasePage.wait.Until(ExpectedConditions.ElementExists(By.CssSelector(BluRingViewer.div_SortPopUp)));
        }

        /// <summary>
        /// This method will select the value from Sort Dropdown  
        /// </summary>
        /// <param name="value"></param>
        public void SelectValue_SortDropdown(String value)
        {
            var popup = this.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_SortPopUp);
            var items = popup.FindElements(By.CssSelector(BluRingViewer.div_SortItems));
            int iterate = -1;
            bool itemfound = false;

            foreach(var item in items)
            {   
                if (item.FindElement(By.CssSelector("span")).GetAttribute("innerHTML").Equals(value))
                {
                    item.Click();
                    itemfound = true;
                }
                iterate++;
            }

            if (itemfound == false) { throw new Exception("Item Not Found Excpection"); }       
            
        }

        /// <summary>
        /// This method will open the exam list panel
        /// </summary>
        public Boolean OpenExamList()
        {
            var element = this.GetElement(BasePage.SelectorType.CssSelector, div_launchExamList);
            element.Click();
            Boolean isPriorDisplayed = false;
            try
            {
                BasePage.wait.Until<Boolean>(d =>
            {
                var priors = d.FindElements(By.CssSelector(BluRingViewer.div_priors));
                foreach (IWebElement prior in priors)
                {
                    if (prior.Displayed) { isPriorDisplayed = true; }
                    else { isPriorDisplayed = false; break; }
                }
                if (isPriorDisplayed)
                    return true;
                else
                    return false;
            });
            }
            catch (Exception) { }


            Thread.Sleep(4000);
            return isPriorDisplayed;
        }

        /// <summary>
        /// This method will close the exam list section
        /// </summary>
        public Boolean CloseExamList()
        {
            var element = this.GetElement(BasePage.SelectorType.CssSelector, div_closeExamList);
            element.Click();
            Boolean isPriorNotDisplayed = false;

            BasePage.wait.Until<Boolean>(d =>
            {
                var priors = d.FindElements(By.CssSelector(BluRingViewer.div_priors));
                foreach (IWebElement prior in priors)
                {
                    if (!prior.Displayed) { isPriorNotDisplayed = true; }
                    else { isPriorNotDisplayed = false; break; }
                }
                if (isPriorNotDisplayed)
                    return true;
                else
                    return false;
            });

            Thread.Sleep(4000);
            return isPriorNotDisplayed;


        }

        /// <summary>
        /// This method will get the count of Study panel (Number of priors opened including the main study)
        /// </summary>
        /// <returns></returns>
        public int GetStudyPanelCount()
        {
            int panelcount = 0;
            panelcount = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studypanel)).Count;
            return panelcount;
        }

        /// <summary>
        /// Line measurement tool by clicking on 2 points
        /// </summary>
        public void Draw_LineMeasurement(int dragStartX = 0, int dragStartY = 0, int dropX = 0, int dropY = 0)
        {
            if (dragStartX == 0 && dragStartY == 0 && dropX == 0 && dropY == 0)
            {
                var attributes = GetElementAttributes(this.Activeviewport);
                dragStartX = attributes["width"] / 2;
                dragStartY = attributes["height"] / 2;
                dropX = attributes["width"] / 4;
                dropY = attributes["height"] / 4;
            }

            if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox")) ||
                    (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("edge")))
            {
                // Draw a line by clicking Firefox using TestComplete actions
                var action = new TestCompleteAction();
                action.MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dragStartX, dragStartY);
                Thread.Sleep(2000);
                action.Click();
                Thread.Sleep(2000);
                action.MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dropX, dropY).Click().Perform();
                Thread.Sleep(2000);
                ScrollIntoView(GetElement("cssselector", div_closeStudy));
            }
            else
            {
                //Draw line by clicking on 2 coordinates
                new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dragStartX, dragStartY).Click().Build().Perform();
                Thread.Sleep(2000);
                new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dropX, dropY).Click().Build().Perform();
                Thread.Sleep(2000);

            }
            Thread.Sleep(Config.ms_minTimeout);
        }

        /// <summary>
        /// This method for closing the Study Panel
        /// </summary>
        /// <param name="panel">should start from 1</param>
        public void CloseStudypanel(int panelNum)
        {
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer") || ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("edge"))
            {
                this.ClickElement(BasePage.Driver.FindElement(By.CssSelector(div_studypanel + ":nth-of-type(" + panelNum + ") " + div_closestudypanel)));
            }
            else
            {
                BasePage.Driver.FindElement(By.CssSelector(div_studypanel + ":nth-of-type(" + panelNum + ") " + div_closestudypanel)).Click();
                PageLoadWait.WaitForPageLoad(10);
            }

            Thread.Sleep(5000);
        }

        /// <summary>
        /// Delete Measurement
        /// </summary>
        public void ApplyTool_DeleteAnnotation(int dragStartX = 0, int dragStartY = 0)
        {
            var attributes = GetElementAttributes(this.Activeviewport);
            dragStartX = attributes["width"] / 3;
            dragStartY = attributes["height"] / 3;

            //Left click on the measurement
            if(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("edge"))
            {
                new TestCompleteAction().MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dragStartX, dragStartY).Click().Perform();
            }
            else
            {
                new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dragStartX, dragStartY).Click().Build().Perform();
            }
            
            Thread.Sleep(2000);
            Thread.Sleep(Config.ms_minTimeout);
        }

        /// <summary>
        /// This method will give the Css of the Toolbox
        /// </summary>
        /// <param name="panelnumber">should start from 1</param>
        public String GetToolBoxCss(int panelnumber = 1, int viewport = 1)
        {
            //Set the Study panel and viewport
            String studypanel = div_studypanel + ":nth-of-type(" + panelnumber + ") " + "div.compositeViewerComponent div.viewerContainer:nth-of-type(" + viewport + ")";           
            return studypanel + " " + div_toolboxContainer;
        }

        /// <summary>
        /// This method will return 'true' if the given element is visible in UI. otherwise 'false'
        /// </summary>
        /// <param name="by"></param>
        public Boolean IsElementVisibleInUI(By by)
        {
            try
            {
                IWebElement ele = BasePage.Driver.FindElement(by);
                if (ele.Size.Height > 0 && ele.Size.Width > 0)
                {
                    return true;
                }
                return false;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        /// <summary>
        /// This Method will return the Css of the viewport in studypanel
        /// </summary>
        /// <param name="panelnumber"></param> // Should start with 1
        /// <param name="viewportnumber"></param> // Should start with 0
        /// <returns></returns>
        public String GetViewportCss(int panelnumber, int viewportnumber)
        {
            String studypanel = div_studypanel + ":nth-of-type(" + panelnumber + ")";
            viewportnumber++;
            String viewport = "div.viewerContainer:nth-of-type(" + viewportnumber + ") .viewportDiv";
            return studypanel + " " + viewport;
        }

        /// <summary>
        /// This Method will return the Total of the viewport in studypanel
        /// </summary>
        /// <param name="panelnumber"></param> // Should start with 1
        /// <returns></returns>
        public int GetViewPortCount(int panelnumber)
        {
            String studypanel = div_studypanel + ":nth-of-type(" + panelnumber + ")";
            //String viewport = "div.viewerContainer .viewportDiv";
            return Driver.FindElements(By.CssSelector(studypanel + " " + div_allViewportes)).Count;
        }

        /// <summary>
		/// Edit the drawn annotations
		/// </summary>
		public void ApplyTool_EditAnnotation(int dragStartX = 0, int dragStartY = 0, int dropX = 0, int dropY = 0)
        {

            if (dragStartX == 0 && dragStartY == 0 && dropX == 0 && dropY == 0)
            {
                var attributes = GetElementAttributes(this.Activeviewport);
                dragStartX = attributes["width"] / 3;
                dragStartY = attributes["height"] / 3;
                dropX = attributes["width"] / 6;
                dropY = attributes["height"] / 6;
            }

            Thread.Sleep(500);
            this.PerformTool(dragStartX, dragStartY, dropX, dropY);
            Thread.Sleep(Config.ms_minTimeout);
        }

        /// <summary>  
        /// This Funtion is used to select the User Settings  
        /// </summary>  
        /// <param name="ul"></param> // Where 1 = font, 2 = Language, 3 - font color
        /// <param name="li"></param> // Where 1 = UI - LARGE/LANG - ENGLISH/DEFAULT, 2 = UI - MEDIUM/LANG - FRANÇAIS/GREEN , 3 - UI - SMALL//ORANGE
        public void SelectUserSetting(int ul, int li)
        {
            String ClassName = GetElementAttribute("cssselector", userSettings_Icon, "class");
            if (!(ClassName.Contains("isActive")))
            {
                Driver.FindElement(By.CssSelector(userSettings_Icon)).Click();
                Thread.Sleep(Config.ms_minTimeout);
            }
            GetElement("cssselector", div_usercontoltools + "(" + ul + ") li:nth-of-type(" + li + ")").Click();
            Thread.Sleep(Config.ms_minTimeout);
        }

        /// <summary>  
        /// This Funtion returns the selected options in the user settings  
        /// </summary>  
        /// <param name="ul"></param> // Where 1 = font, 2 = Language, 3 - font color
        /// <param name="li"></param> // Where 1 = UI - LARGE/LANG - ENGLISH/DEFAULT, 2 = UI - MEDIUM/LANG - FRANÇAIS/GREEN , 3 - UI - SMALL//ORANGE  
        /// <returns></returns>  
        public String GetSelectedUserSetting(int ul, int li)
        {
            String ClassName = GetElementAttribute("cssselector", userSettings_Icon, "class");
            if (!(ClassName.Contains("isActive")))
            {
                Driver.FindElement(By.CssSelector(userSettings_Icon)).Click();
                Thread.Sleep(Config.ms_minTimeout);
            }
            String Option = GetText("cssselector", div_usercontoltools + "(" + ul + ") li:nth-of-type(" + li + ")");
            Driver.FindElement(By.CssSelector(userSettings_Icon)).Click();
            Thread.Sleep(Config.ms_minTimeout);
            return Option;
        }

        /// <summary>
        /// This Method is used to Calculate the age of patient with the Given DOB
        /// </summary>
        /// <returns>Age for the given DOB</returns>
        public static int CalculateAge(String DOB)
        {
            String[] DOB_Split = DOB.Split('/');
            int year = Int32.Parse(DOB_Split[2]);
            int Age = DateTime.Now.Year - year;
            return Age;
        }

        /// <summary>
        /// This Method is used to verify the color of the font/label
        /// </summary>
        /// <returns>Age for the given DOB</returns>
        public bool CheckColorOfFonts_Labels(String reuse, String Color)
        {
            IList<IWebElement> Element = BasePage.Driver.FindElements(By.CssSelector(reuse));

            foreach (IWebElement ele in Element)
            {
                if (!ele.GetCssValue("color").Equals(Color))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// This method Click on the Given specified View Port
        /// </summary>
        /// <param name="viewport">should start from 0</param>
        /// <param name="panel">should start from 1</param>
        public IWebElement ClickOnViewPort(int panelnumber = 1, int viewportnumber = 1)
        {
            //Set view port
            Driver.FindElement(By.CssSelector(SetViewPort1(panelnumber, viewportnumber))).Click();
            return Driver.FindElement(By.CssSelector(SetViewPort1(panelnumber, viewportnumber)));
        }

        /// <summary>
        /// This method Will verfiy the Specified view port is active and it border colour.
        /// </summary>
        /// <param name="viewport">should start from 0</param>
        /// <param name="panel">should start from 1</param>
        public bool VerifyViewPortIsActive(int panelnumber = 0, int viewportnumber = 0, IWebElement ViewPortwebObject = null)
        {
            bool status = false;
            if (panelnumber != 0 && viewportnumber != 0)
            {
                SetViewPort1(panelnumber, viewportnumber);
                Thread.Sleep(2000); //get the Color and boarder loaded
                //bool color = (BasePage.Driver.FindElement(By.CssSelector(this.activeviewport)).FindElement(By.XPath("..")).GetCssValue("border").Equals("1px solid #5AAAFF") ||
                //    BasePage.Driver.FindElement(By.CssSelector(this.activeviewport)).FindElement(By.XPath("..")).GetCssValue("border").Equals("1px solid rgb(90, 170, 255)"));

                bool color = (BasePage.Driver.FindElement(By.CssSelector(this.activeviewport)).FindElement(By.XPath("..")).GetCssValue("border").Equals("1px solid #5AAAFF") ||
                    BasePage.Driver.FindElement(By.CssSelector(this.activeviewport)).FindElement(By.XPath("..")).GetCssValue("border").Equals("1px solid rgb(90, 170, 255)") ||

                    (BasePage.Driver.FindElement(By.CssSelector(this.activeviewport)).FindElement(By.XPath("..")).GetCssValue("border-top-color").Equals("rgba(90, 170, 255, 1)") &&
                    BasePage.Driver.FindElement(By.CssSelector(this.activeviewport)).FindElement(By.XPath("..")).GetCssValue("border-bottom-color").Equals("rgba(90, 170, 255, 1)") &&
                    BasePage.Driver.FindElement(By.CssSelector(this.activeviewport)).FindElement(By.XPath("..")).GetCssValue("border-left-color").Equals("rgba(90, 170, 255, 1)") &&
                    BasePage.Driver.FindElement(By.CssSelector(this.activeviewport)).FindElement(By.XPath("..")).GetCssValue("border-right-color").Equals("rgba(90, 170, 255, 1)")) ||

                    (BasePage.Driver.FindElement(By.CssSelector(this.activeviewport)).FindElement(By.XPath("..")).GetCssValue("border-top-color").Equals("rgb(90, 170, 255)") &&
                    BasePage.Driver.FindElement(By.CssSelector(this.activeviewport)).FindElement(By.XPath("..")).GetCssValue("border-bottom-color").Equals("rgb(90, 170, 255)") &&
                    BasePage.Driver.FindElement(By.CssSelector(this.activeviewport)).FindElement(By.XPath("..")).GetCssValue("border-left-color").Equals("rgb(90, 170, 255)") &&
                    BasePage.Driver.FindElement(By.CssSelector(this.activeviewport)).FindElement(By.XPath("..")).GetCssValue("border-right-color").Equals("rgb(90, 170, 255)")) ||

                    (BasePage.Driver.FindElement(By.CssSelector(this.activeviewport)).FindElement(By.XPath("..")).GetCssValue("border-top-color").Equals("#5AAAFF") &&
                    BasePage.Driver.FindElement(By.CssSelector(this.activeviewport)).FindElement(By.XPath("..")).GetCssValue("border-bottom-color").Equals("#5AAAFF") &&
                    BasePage.Driver.FindElement(By.CssSelector(this.activeviewport)).FindElement(By.XPath("..")).GetCssValue("border-left-color").Equals("#5AAAFF") &&
                    BasePage.Driver.FindElement(By.CssSelector(this.activeviewport)).FindElement(By.XPath("..")).GetCssValue("border-right-color").Equals("#5AAAFF"))

                    );

                string activeAttribute = BasePage.Driver.FindElement(By.CssSelector(this.activeviewport)).GetAttribute("class");
                if (color && activeAttribute.Contains("activeViewportDiv"))
                {
                    Logger.Instance.InfoLog(" Viewport " + viewportnumber + " at study Panel " + panelnumber + " is selected and blue highlight border ");
                    status = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Viewport " + viewportnumber + " at study Panel " + panelnumber + " is not selected and not has blue highlight border ");
                    status = false;
                }
            }
            else if (ViewPortwebObject != null)
            {
                status = (
                    
                    ViewPortwebObject.FindElement(By.XPath("..")).GetCssValue("border").Equals("1px solid rgb(90, 170, 255)") ||

                    (ViewPortwebObject.FindElement(By.XPath("..")).GetCssValue("border-top-color").Equals("rgba(90, 170, 255, 1)") &&
                    ViewPortwebObject.FindElement(By.XPath("..")).GetCssValue("border-bottom-color").Equals("rgba(90, 170, 255, 1)") &&
                    ViewPortwebObject.FindElement(By.XPath("..")).GetCssValue("border-left-color").Equals("rgba(90, 170, 255, 1)") &&
                    ViewPortwebObject.FindElement(By.XPath("..")).GetCssValue("border-right-color").Equals("rgba(90, 170, 255, 1)")) ||

                    (ViewPortwebObject.FindElement(By.XPath("..")).GetCssValue("border-top-color").Equals("rgb(90, 170, 255)") &&
                    ViewPortwebObject.FindElement(By.XPath("..")).GetCssValue("border-bottom-color").Equals("rgb(90, 170, 255)") &&
                    ViewPortwebObject.FindElement(By.XPath("..")).GetCssValue("border-left-color").Equals("rgb(90, 170, 255)") &&
                    ViewPortwebObject.FindElement(By.XPath("..")).GetCssValue("border-right-color").Equals("rgb(90, 170, 255)")) ||

                    (ViewPortwebObject.FindElement(By.XPath("..")).GetCssValue("border-top-color").Equals("#5AAAFF") &&
                    ViewPortwebObject.FindElement(By.XPath("..")).GetCssValue("border-bottom-color").Equals("#5AAAFF") &&
                    ViewPortwebObject.FindElement(By.XPath("..")).GetCssValue("border-left-color").Equals("#5AAAFF") &&
                    ViewPortwebObject.FindElement(By.XPath("..")).GetCssValue("border-right-color").Equals("#5AAAFF"))

                    );
            }
            return status;
        }

        /// <summary>
        /// This method Will verfiy the Specified view port is active and it border colour.
        /// </summary>
        public static int TotalStudyPanel()
        {
            IList<IWebElement> StudyPanel = BasePage.Driver.FindElements(By.CssSelector(".studyPanelControl"));
            return StudyPanel.Count;
        }

        /// <summary>
        /// This method used to take count of thumbnails in the Study Panel
        /// </summary>
        /// <returns>no of thumbnails in Study Panel</returns>
        public static int NumberOfThumbnailsInStudyPanel(int studyPanelNumber = 1)
        {
            IList<IWebElement> thumbnailslist = Driver.FindElements(By.CssSelector(div_studypanel + ":nth-of-type(" + studyPanelNumber + ") " + div_studyPanelThumbnailImageComponent));
            int count = thumbnailslist.Count;
            Logger.Instance.InfoLog("Total number of thumbnails found at Study Panel-"+ studyPanelNumber + "  is- " + count);
            return count;
        }

        /// <summary>
        /// This method used to take count of thumbnails in the Exam list for opened Thumbnail preview.
        /// </summary>
        /// <returns>no of thumbnails in Study Panel</returns>
        public static int NumberOfThumbnailsInExamList()
        {
            IList<IWebElement> thumbnailslist = Driver.FindElements(By.CssSelector(div_ExamList_thumbnails));
            int count = thumbnailslist.Count;
            //Logger.Instance.InfoLog("Total number of thumbnails found at thumbnail preview Opened: " + count);
            return count;
        }

        /// <summary>
        /// This method used to click on the thumbnails in the Study Panel
        /// </summary>
        /// <returns>no of thumbnails in Study Panel</returns>
        public void ClickOnThumbnailsInStudyPanel(int studyPanelNumber = 1, int thumbnail = 1, bool doubleclick = false, bool isTestcompleteAction = false)
        {
            TestCompleteAction Testcompleteaction = new TestCompleteAction();
            IList<IWebElement> thumbnailslist = Driver.FindElements(By.CssSelector(div_studypanel + ":nth-of-type(" + studyPanelNumber + ") " + div_studyPanelThumbnailImageComponent));
            if (doubleclick == false)
            {
                if (isTestcompleteAction == true)
                    Testcompleteaction.Click(thumbnailslist[thumbnail - 1]);
                else
                    thumbnailslist.ToArray()[thumbnail - 1].Click();
            }
            else if ((Config.BrowserType == "firefox"))
            {
                if (isTestcompleteAction == true)
                    Testcompleteaction.DoubleClick(thumbnailslist[thumbnail - 1]);
                else
                {
                    Actions builder = new Actions(Driver);
                    builder.DoubleClick(thumbnailslist.ToArray()[thumbnail - 1]).Build().Perform();
                }
            }
            else
            {
                if (isTestcompleteAction == true)
                    Testcompleteaction.DoubleClick(thumbnailslist[thumbnail - 1]);
            else
                this.DoubleClick(thumbnailslist.ToArray()[thumbnail - 1]);
            }
            Testcompleteaction.Perform();
        }

        /// <summary>
        /// This method used to verfiy that on the thumbnails in the Study Panel is active
        /// </summary>
        /// <returns>no of thumbnails in Study Panel</returns>
        public static bool VerifyThumbnailsInStudyPanelIsActive(int studyPanelNumber = 1, int thumbnail = 1)
        {
            bool status = false;
            if (thumbnail > NumberOfThumbnailsInStudyPanel(studyPanelNumber))
            {
                Logger.Instance.ErrorLog("Error Occured : Total number of thumbnail is less than the given thumbnail count " + thumbnail + " at the study panel " + studyPanelNumber);
            }
            else
            {
                Thread.Sleep(2000);
                IList<IWebElement> thumbnailslist = Driver.FindElements(By.CssSelector(div_studypanel + ":nth-of-type(" + studyPanelNumber + ") " + div_studyPanelThumbnailImageComponent + ">div"));
                //bool color1 = thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border").Equals("1px solid rgb(90, 170, 255)");

                bool color1 = thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border").Equals("1px solid rgb(90, 170, 255)") ||

                    (thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-top-color").Equals("rgba(90, 170, 255, 1)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-bottom-color").Equals("rgba(90, 170, 255, 1)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-left-color").Equals("rgba(90, 170, 255, 1)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-right-color").Equals("rgba(90, 170, 255, 1)")) ||

                    (thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-top-color").Equals("rgb(90, 170, 255)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-bottom-color").Equals("rgb(90, 170, 255)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-left-color").Equals("rgb(90, 170, 255)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-right-color").Equals("rgb(90, 170, 255)")) ||

                    (thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-top-color").Equals("#5AAAFF") &&
                    thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-bottom-color").Equals("#5AAAFF") &&
                    thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-left-color").Equals("#5AAAFF") &&
                    thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-right-color").Equals("#5AAAFF"));


                bool color2 = thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("background-color").Contains("rgb(90, 170, 255)");
                bool color3 = thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("background-color").Contains("rgba(90, 170, 255, 1)");
                string activeAttribute = thumbnailslist.ToArray()[thumbnail - 1].GetAttribute("class");
                if ((color1 || color2 || color3) && activeAttribute.Contains("thumbnailImageSelected"))
                {
                    Logger.Instance.InfoLog("Thumbnail " + thumbnail + " at study Panel " + studyPanelNumber + " is selected and blue highlight border ");
                    status = true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Thumbnail " + thumbnail + " at study Panel " + studyPanelNumber + " is not selected and not has blue highlight border ");
                    status = false;
                }
            }

            return status;
        }

        /// <summary>
        /// This method used to verfiy that on the thumbnails in the Exam List is active
        /// </summary>
        /// <returns>Return bool </returns>
        public static bool VerifyThumbnailsInExamList(int thumbnail = 1, string type = "Active")
        {
            bool status = false; bool color = false; bool attribute;
            if (thumbnail > NumberOfThumbnailsInExamList())
            {
                Logger.Instance.ErrorLog("Error Occured : Total number of thumbnail is less than the given thumbnail count " + thumbnail + " at the Exam list panel");
            }
            else
            {
                Thread.Sleep(2000);
                IList<IWebElement> thumbnailslist = Driver.FindElements(By.CssSelector(div_ExamList_thumbnails));
                if (type.ToLower() == "active")
                {
                    //color = thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border").Equals("1px solid rgb(90, 170, 255)");

                    color =
                        thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border").Equals("1px solid rgb(90, 170, 255)") ||

                    (thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-top-color").Equals("rgba(90, 170, 255, 1)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-bottom-color").Equals("rgba(90, 170, 255, 1)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-left-color").Equals("rgba(90, 170, 255, 1)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-right-color").Equals("rgba(90, 170, 255, 1)")) ||

                    (thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-top-color").Equals("rgb(90, 170, 255)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-bottom-color").Equals("rgb(90, 170, 255)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-left-color").Equals("rgb(90, 170, 255)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-right-color").Equals("rgb(90, 170, 255)")) ||

                    (thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-top-color").Equals("#5AAAFF") &&
                    thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-bottom-color").Equals("#5AAAFF") &&
                    thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-left-color").Equals("#5AAAFF") &&
                    thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-right-color").Equals("#5AAAFF"));

                    attribute = thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).GetAttribute("class").Contains("thumbnailImageSelected");
                }
				else if (type.ToLower() == "no border")
				{
					//color = thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border").Equals("1px solid rgb(255, 255, 255)");

					color =
						thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border").Equals("1px solid rgb(0, 0, 0)") ||

					(thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-top-color").Equals("rgba(0, 0, 0, 1)") &&
					thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-bottom-color").Equals("rgba(0, 0, 0, 1)") &&
					thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-left-color").Equals("rgba(0, 0, 0, 1)") &&
					thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-right-color").Equals("rgba(0, 0, 0, 1)")) ||

					(thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-top-color").Equals("rgb(0, 0, 0)") &&
					thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-bottom-color").Equals("rgb(0, 0, 0)") &&
					thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-left-color").Equals("rgb(0, 0, 0)") &&
					thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-right-color").Equals("rgb(0, 0, 0)"));

					attribute = true;
				}
                else
                {
                    //color = thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border").Equals("1px solid rgb(255, 255, 255)");

                    color =
                        thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border").Equals("1px solid rgb(255, 255, 255)") ||

                    (thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-top-color").Equals("rgba(255, 255, 255, 1)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-bottom-color").Equals("rgba(255, 255, 255, 1)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-left-color").Equals("rgba(255, 255, 255, 1)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-right-color").Equals("rgba(255, 255, 255, 1)")) ||

                    (thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-top-color").Equals("rgb(255, 255, 255)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-bottom-color").Equals("rgb(255, 255, 255)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-left-color").Equals("rgb(255, 255, 255)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-right-color").Equals("rgb(255, 255, 255)")) ||

                    (thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-top-color").Equals("#ffffff") &&
                    thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-bottom-color").Equals("#ffffff") &&
                    thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-left-color").Equals("#ffffff") &&
                    thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetCssValue("border-right-color").Equals("#ffffff"));

                    attribute = thumbnailslist.ToArray()[thumbnail - 1].FindElement(By.XPath("..")).FindElement(By.XPath("..")).GetAttribute("class").Contains("thumbnailImageInViewer");
                }

                if (color && attribute)
                    status = true;
                else
                    status = false;
            }

            return status;
        }

        /// <summary>
        /// This method used to verfiy that on the thumbnails in the Study Panel is white border
        /// </summary>
        /// <returns>no of thumbnails in Study Panel</returns>
        public static bool VerifyThumbnailsInStudyPanelIsVisible(int studyPanelNumber = 1, int thumbnail = 1)
        {
            bool status = false;
            Thread.Sleep(2000);
            IList<IWebElement> thumbnailslist = Driver.FindElements(By.CssSelector(div_studypanel + ":nth-of-type(" + studyPanelNumber + ") " + div_studyPanelThumbnailImageComponent + ">div"));
            //bool color = thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border").Equals("1px solid rgb(255, 255, 255)");

            bool color =
                    thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border").Equals("1px solid rgb(255, 255, 255)") ||

                    (thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-top-color").Equals("rgba(255, 255, 255, 1)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-bottom-color").Equals("rgba(255, 255, 255, 1)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-left-color").Equals("rgba(255, 255, 255, 1)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-right-color").Equals("rgba(255, 255, 255, 1)")) ||

                    (thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-top-color").Equals("rgb(255, 255, 255)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-bottom-color").Equals("rgb(255, 255, 255)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-left-color").Equals("rgb(255, 255, 255)") &&
                    thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-right-color").Equals("rgb(255, 255, 255)")) ||

                    (thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-top-color").Equals("#FFFFFF") &&
                    thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-bottom-color").Equals("#FFFFFF") &&
                    thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-left-color").Equals("#FFFFFF") &&
                    thumbnailslist.ToArray()[thumbnail - 1].GetCssValue("border-right-color").Equals("#FFFFFF"));

            if (color)
            {
                Logger.Instance.InfoLog("Thumbnail " + thumbnail + " at study Panel " + studyPanelNumber + " is visible as it contain the white border ");
                status = true;
            }
            else
            {
                Logger.Instance.ErrorLog("Thumbnail " + thumbnail + " at study Panel " + studyPanelNumber + " is not visible as it not contain the white border ");
                status = false;
            }

            return status;
        }

        /// <summary>
        /// Verify Priors Highlighted In ExamList
        /// </summary>
        public bool VerifyPriorsHighlightedInExamList(int sequence = 0, string AccessionNumber = null, string StudyDate = null, string StudyTime = null)
        {
            var priors = BasePage.Driver.FindElements(By.CssSelector(div_priors));
            if (sequence != 0)
            {
                Reusable.Generic.Logger.Instance.InfoLog("VerifyPriorsHighlightedInExamList: priors[sequence].GetCssValue('border - color') = " + priors[sequence].GetCssValue("border-color"));
                //if (priors[sequence].GetCssValue("border-color").Equals("rgb(90, 170, 255)"))

                if (
                    priors[sequence].GetCssValue("border-color").Equals("rgb(90, 170, 255)") ||

                    (priors[sequence].GetCssValue("border-top-color").Equals("rgba(90, 170, 255, 1)") &&
                    priors[sequence].GetCssValue("border-bottom-color").Equals("rgba(90, 170, 255, 1)") &&
                    priors[sequence].GetCssValue("border-left-color").Equals("rgba(90, 170, 255, 1)") &&
                    priors[sequence].GetCssValue("border-right-color").Equals("rgba(90, 170, 255, 1)")) ||

                    (priors[sequence].GetCssValue("border-top-color").Equals("rgb(90, 170, 255)") &&
                    priors[sequence].GetCssValue("border-bottom-color").Equals("rgb(90, 170, 255)") &&
                    priors[sequence].GetCssValue("border-left-color").Equals("rgb(90, 170, 255)") &&
                    priors[sequence].GetCssValue("border-right-color").Equals("rgb(90, 170, 255)")) ||

                    (priors[sequence].GetCssValue("border-top-color").Equals("#5AAAFF") &&
                    priors[sequence].GetCssValue("border-bottom-color").Equals("#5AAAFF") &&
                    priors[sequence].GetCssValue("border-left-color").Equals("#5AAAFF") &&
                    priors[sequence].GetCssValue("border-right-color").Equals("#5AAAFF"))
                    )
                return true;
            }
            else
            {
                //The Exam List has the primary and the opened prior studies highlighted (blue border around the study info rectangle).
                IList<IWebElement> ActiveExamInExamList = Driver.FindElements(By.CssSelector("div[class$=relatedStudyContainerActive]"));
                if (AccessionNumber != null)
                {
                    foreach (IWebElement activeexam in ActiveExamInExamList)
                        if (GetAccession(activeexam.FindElement(By.CssSelector(AccessionNumberInExamList))) == AccessionNumber)
                        {
                            Reusable.Generic.Logger.Instance.InfoLog("VerifyPriorsHighlightedInExamList: activeexam.GetCssValue('border - color') = " + activeexam.GetCssValue("border-color"));
                            //if (activeexam.GetCssValue("border-color").Equals("rgb(90, 170, 255)"))    
                            if (
                                activeexam.GetCssValue("border-color").Equals("rgb(90, 170, 255)") ||

                                (activeexam.GetCssValue("border-top-color").Equals("rgba(90, 170, 255, 1)") &&
                                activeexam.GetCssValue("border-bottom-color").Equals("rgba(90, 170, 255, 1)") &&
                                activeexam.GetCssValue("border-left-color").Equals("rgba(90, 170, 255, 1)") &&
                                activeexam.GetCssValue("border-right-color").Equals("rgba(90, 170, 255, 1)")) ||

                                (activeexam.GetCssValue("border-top-color").Equals("rgb(90, 170, 255)") &&
                                activeexam.GetCssValue("border-bottom-color").Equals("rgb(90, 170, 255)") &&
                                activeexam.GetCssValue("border-left-color").Equals("rgb(90, 170, 255)") &&
                                activeexam.GetCssValue("border-right-color").Equals("rgb(90, 170, 255)")) ||

                                (activeexam.GetCssValue("border-top-color").Equals("#5AAAFF)") &&
                                activeexam.GetCssValue("border-bottom-color").Equals("#5AAAFF") &&
                                activeexam.GetCssValue("border-left-color").Equals("#5AAAFF)") &&
                                activeexam.GetCssValue("border-right-color").Equals("#5AAAFF"))
                                )
                                return true;
                        }
                }
                else if (StudyTime == null)
                {
                    foreach (IWebElement activeexam in ActiveExamInExamList)
                        if (activeexam.FindElement(By.CssSelector(div_examListPanelDate)).GetAttribute("innerHTML") == StudyDate)
                        {
                            Reusable.Generic.Logger.Instance.InfoLog("VerifyPriorsHighlightedInExamList: activeexam.GetCssValue('border - color') = " + activeexam.GetCssValue("border-color"));
                            //if (activeexam.GetCssValue("border-color").Equals("rgb(90, 170, 255)"))
                            if (
                                activeexam.GetCssValue("border-color").Equals("rgb(90, 170, 255)") ||

                                (activeexam.GetCssValue("border-top-color").Equals("rgba(90, 170, 255, 1)") &&
                                activeexam.GetCssValue("border-bottom-color").Equals("rgba(90, 170, 255, 1)") &&
                                activeexam.GetCssValue("border-left-color").Equals("rgba(90, 170, 255, 1)") &&
                                activeexam.GetCssValue("border-right-color").Equals("rgba(90, 170, 255, 1)")) ||

                                (activeexam.GetCssValue("border-top-color").Equals("rgb(90, 170, 255)") &&
                                activeexam.GetCssValue("border-bottom-color").Equals("rgb(90, 170, 255)") &&
                                activeexam.GetCssValue("border-left-color").Equals("rgb(90, 170, 255)") &&
                                activeexam.GetCssValue("border-right-color").Equals("rgb(90, 170, 255)")) ||

                                (activeexam.GetCssValue("border-top-color").Equals("#5AAAFF") &&
                                activeexam.GetCssValue("border-bottom-color").Equals("#5AAAFF") &&
                                activeexam.GetCssValue("border-left-color").Equals("#5AAAFF") &&
                                activeexam.GetCssValue("border-right-color").Equals("#5AAAFF"))
                                )
                                return true;
                        }
                }
                else if (StudyTime != null && StudyDate != null)
                {
                    foreach (IWebElement activeexam in ActiveExamInExamList)
                        if (activeexam.FindElement(By.CssSelector(div_examListPanelDate)).GetAttribute("innerHTML") == StudyDate)
                            if (activeexam.FindElement(By.CssSelector(examTimeInExamList)).GetAttribute("innerHTML").Contains(StudyTime))
                            {
                                Reusable.Generic.Logger.Instance.InfoLog("VerifyPriorsHighlightedInExamList: activeexam.GetCssValue('border - color') = " + activeexam.GetCssValue("border-color"));
                                //if (activeexam.GetCssValue("border-color").Equals("rgb(90, 170, 255)"))
                                if (
                                activeexam.GetCssValue("border-color").Equals("rgb(90, 170, 255)") ||

                                (activeexam.GetCssValue("border-top-color").Equals("rgba(90, 170, 255, 1)") &&
                                activeexam.GetCssValue("border-bottom-color").Equals("rgba(90, 170, 255, 1)") &&
                                activeexam.GetCssValue("border-left-color").Equals("rgba(90, 170, 255, 1)") &&
                                activeexam.GetCssValue("border-right-color").Equals("rgba(90, 170, 255, 1)")) ||

                                (activeexam.GetCssValue("border-top-color").Equals("rgb(90, 170, 255)") &&
                                activeexam.GetCssValue("border-bottom-color").Equals("rgb(90, 170, 255)") &&
                                activeexam.GetCssValue("border-left-color").Equals("rgb(90, 170, 255)") &&
                                activeexam.GetCssValue("border-right-color").Equals("rgb(90, 170, 255)")) ||

                                (activeexam.GetCssValue("border-top-color").Equals("#5AAAFF") &&
                                activeexam.GetCssValue("border-bottom-color").Equals("#5AAAFF") &&
                                activeexam.GetCssValue("border-left-color").Equals("#5AAAFF") &&
                                activeexam.GetCssValue("border-right-color").Equals("#5AAAFF"))
                                )
                                    return true;
                            }
                }
                else
                    Logger.Instance.ErrorLog("Invaild search parameter is given");

            }

            return false;
        }
        
        /// <summary>
        /// This method used to drag and drop thumbnails
        /// </summary>
        /// <returns>Thumbnail caption texts displayed on thumbnail</returns>
        public void DropAndDropThumbnails(int thumbnailnumber, int viewport, int studyPanelNumber, bool ExamList = false, bool UseDragDrop = false)
        {
            TestCompleteAction action = new TestCompleteAction();
            Thread.Sleep(3000);
            IList<IWebElement> thumbnailslist;
            IWebElement viewportObject = BasePage.Driver.FindElement(By.CssSelector(SetViewPort1(studyPanelNumber, viewport)));

            if (ExamList == true)
                thumbnailslist = Driver.FindElements(By.CssSelector(div_ExamList_thumbnails));
            else
                thumbnailslist = Driver.FindElements(By.CssSelector(div_studypanel + ":nth-of-type(" + studyPanelNumber + ") " + div_studyPanelThumbnailImageComponent + ">div"));

            if (!UseDragDrop)
            {
                action.MoveToElement(thumbnailslist[thumbnailnumber - 1]).ClickAndHold(thumbnailslist[thumbnailnumber - 1]);
                action.MoveToElement(viewportObject);
                action.Release(viewportObject).Perform(); 
            }
            else
            {
                action.DragAndDrop(thumbnailslist[thumbnailnumber - 1], viewportObject, "false").Perform();
            }
            Thread.Sleep(10000);
        }

        /// <summary>
        /// This method used to drag and drop thumbnails for foreign series
        /// </summary>
        /// <param name="sourceStudyPanelNumber">starts with 0</param>
        /// <param name="sourceThumbnailNumber">starts with 0</param>
        /// <param name="destinationStudyPanelNumber">starts with 0</param>
        /// <param name="destinationViewport">starts with 0</param>
        /// <param name="UseDragDrop">Mechanism used for drag and drop</param>
        public void DropAndDropForeignThumbnails(int sourceStudyPanelNumber, int sourceThumbnailNumber, int destinationStudyPanelNumber, int destinationViewport, bool UseDragDrop = true)
        {
            TestCompleteAction action = new TestCompleteAction();
            Thread.Sleep(3000);
            IList<IWebElement> thumbnailslist;
            IWebElement viewportObject = BasePage.Driver.FindElement(By.CssSelector(SetViewPort1(destinationStudyPanelNumber, destinationViewport)));

            thumbnailslist = Driver.FindElements(By.CssSelector(div_studypanel + ":nth-of-type(" + sourceStudyPanelNumber + ") " + div_thumbnails));
            
            if (UseDragDrop)
            {
                action.DragAndDrop(thumbnailslist[sourceThumbnailNumber - 1], viewportObject, "false").Perform();
            }
            else
            {
                action.MoveToElement(thumbnailslist[sourceThumbnailNumber - 1]).ClickAndHold(thumbnailslist[sourceThumbnailNumber - 1]);
                action.MoveToElement(viewportObject);
                action.Release(viewportObject).Perform();
            }
            Thread.Sleep(10000);
        }

        /// <summary>
        /// This method used to verify Thumbnail caption displayed as overlay on thumbnail image
        /// </summary>
        /// <returns>Thumbnail caption texts displayed on thumbnail</returns>
        public static bool CheckThumbnailCaption(IWebElement element)
        {
            int imageNum = 0;
            IList<IWebElement> Captions = element.FindElements(By.CssSelector(div_thumbnailCaption));
            IList<IWebElement> ImageCount = element.FindElements(By.CssSelector(div_imageFrameNumber));
            for (int i = 0; i < Captions.Count(); i++)
            {
                if (i == 0)
                {
                    String s = Captions.ElementAt(i).Text;
                    String[] Splitnum = s.Split('-');
                    imageNum = Int32.Parse(Splitnum[1]);
                }
                if (!(Captions.ElementAt(i)).Text.Equals("S1- " + imageNum) && !(ImageCount.ElementAt(i).Text.Equals(imageNum)))
                {
                    Logger.Instance.InfoLog("Thumbnail number" + (i + 1) + "is not having the correct caption");
                    Logger.Instance.InfoLog("Thumbnail number" + (i + 1) + "is not having the correct Image count");
                    return false;
                }
                Logger.Instance.InfoLog("Image number displayed on Thumbnail is" + imageNum);
                imageNum++;
            }
            return true;
        }

        /// <summary>
        /// Return Series number of he thumbnail specified in the argument
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public int GetSeriesNumber(IWebElement element)
        {
            String Caption = element.FindElement(By.CssSelector(div_thumbnailCaption)).GetAttribute("innerHTML");
            String[] split = Caption.Split('-');
            return Int32.Parse(Regex.Replace(split[0], "[^0-9]+", string.Empty));
        }

        /// <summary>
        /// Returns Image number of the thumbnail specified in the argument
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public int GetImageNumber(IWebElement element)
        {
            String Caption = element.FindElement(By.CssSelector(div_thumbnailCaption)).GetAttribute("innerHTML");
            String[] split = Caption.Split('-');
            return Int32.Parse(split[1]);
        }

        /// <summary>
        /// This method is to open show / hide drop down
        /// </summary>
        public void OpenShowHideDropdown()
        {
            IWebElement showhide = GetElement(BasePage.SelectorType.CssSelector, div_ShowHideTool);
            this.ClickElement(showhide);
            Thread.Sleep(1000);
        }

        /// <summary>
        /// This method is to verify the Show / Hide drop down values
        /// </summary>
        public bool Verify_ShowHideDropdown_Values(string[] Values = null , bool isOpenShowHide = true, string Value = null)
        {
            if (isOpenShowHide)
                this.OpenShowHideDropdown();

            IList<IWebElement> dropdown = BasePage.Driver.FindElements(By.CssSelector(div_ShowHideDropdown));
            Thread.Sleep(500);
            int count = dropdown.Count - 1;
            while (count >= 0)
            {
                if (Value != null)
                {
                    if (dropdown[count].GetAttribute("innerHTML").Trim().ToLower().Equals(Value.ToLower()))
                    {
                        break;
                    }

                }
                else
                {
                    if (!dropdown[count].GetAttribute("innerHTML").Trim().ToLower().Equals(Values[count].ToLower()))
                    {
                        return false;
                    }
                }
                count--;
            }
            return true;
        }



        /// <summary>
        /// This method is to select specific value from Show / Hide drop down
        /// </summary>
        public bool SelectShowHideValue(string Value, bool isOpenShowHide = true)
        {
            if (isOpenShowHide)
                this.OpenShowHideDropdown();

            IList<IWebElement> dropdown = BasePage.Driver.FindElements(By.CssSelector(div_ShowHideDropdown));
            Thread.Sleep(500);
            int count = dropdown.Count - 1;
            while (count >= 0)
            {
                if (dropdown[count].GetAttribute("innerHTML").Trim().ToLower().Equals(Value.ToLower()))
                {
                    this.ClickElement(dropdown[count]);
                    Thread.Sleep(2000);
                    return true;
                }
                count--;
            }
            return false;
        }

        /// <summary>
        /// To get the Font size of the element passed as argumnet
        /// </summary>
        public string[] getFontSizeofElements(IList<IWebElement> ElementListToFind)
        {
            IList<string> font = new List<string>();

            foreach (IWebElement element in ElementListToFind)
            {
                font.Add(element.GetCssValue("font-size"));
            }
            return font.ToArray();
        }

        /// <summary>
        /// This Method is to click on the User setting option 
        /// </summary>
        public IWebElement ClickOnUSerSettings()
        {
            this.ClickElement(SettingButton());
            Thread.Sleep(2000);
            if (!SettingPanel().Displayed)
                Logger.Instance.ErrorLog("User setting panel is not displayed");

            Logger.Instance.InfoLog("User setting panel is displayed");

            return SettingPanel();
        }

        /// <summary>
        /// This Method is to click on the User setting option and verify given user setting is selected and is displayed
        /// </summary>
        public bool UserSettings(string ActionType, string UserSettingValue)
        {
            bool result = false;
            // Open Seeting panel if not opened
            if (!SettingPanel().Displayed)
                ClickOnUSerSettings();

            //Get all the User settings options 
            IList<IWebElement> usersettingList = BasePage.Driver.FindElements(By.CssSelector("div[class*='globalSettingPanel'] ul li"));


            foreach (IWebElement usersetting in usersettingList)
                if (usersetting.Text.Replace(" ", "").Replace("✔", "").Replace("\r", "").Replace("\n", "").ToLower() == UserSettingValue.Replace(" ", "").ToLower())
                {
                    // If action is to select , select the given User settings and return true
                    if (ActionType.ToLower() == "select")
                    {
                        this.ClickElement(usersetting);
                        Logger.Instance.InfoLog("User setting panel " + UserSettingValue + " is selected");
                        Thread.Sleep(2000);
                        result = true;
                        break;
                    }

                    // If action is to select , select the given User settings and return true
                    if (ActionType.ToLower() == "displayed")
                    {
                        Logger.Instance.InfoLog("User setting panel " + UserSettingValue + " is displayed");
                        result = true;
                        break;
                    }

                    // If the Action is to 'Verfiy' , then verify whether the  given user settings is selected and return true
                    if (ActionType.ToLower() == "checked")
                        if (usersetting.Text.Contains("✔"))
                        {
                            Logger.Instance.InfoLog("User setting panel " + UserSettingValue + " is Checked");
                            result = true;
                            ClickOnUSerSettings();
                            break;
                        }
                }

            return result;
        }

        // <summary>
        // This method navigate to the url generated in Test-EHR even "session already exists" message is displayed
        // </summary>
        // <param name = "URL" ></ param >
        public BluRingViewer NavigateToBluringIntegratorURL(String URL)
        {
            bool Status = false;
            int timeout = 0;
            //DriverGoTo(URL);
            //Driver.Navigate().GoToUrl(URL);
			new Login().DriverGoTo(URL);

            while (!Status)
            {
                PageLoadWait.WaitForPageLoad(20);
                timeout++;
                try
                {
                    Thread.Sleep(5000);
                    if (new BluRingViewer().AuthenticationErrorMsg().Text.ToLower().Contains("there is another session open"))
                    {
                        //Driver.Navigate().Refresh();
						((IJavaScriptExecutor)BasePage.Driver).ExecuteScript("location.reload()");
                    }
                }
                catch (Exception) { Status = true; }
                if (timeout > 3)
                {
                    break;
                }
            }
            return new BluRingViewer();
        }

        /// <summary>
        /// This Method is used to click the Thumbnail icon in the exam list
        /// </summary>
        /// <param name="DateAndTime"></param>
        public void ClickExamListThumbnailIcon(String DateAndTime)
        {
            IList<IWebElement> relatedStudy = Driver.FindElements(By.CssSelector(div_priorsBlock));
            foreach (IWebElement ele in relatedStudy)
            {
                if (ele.GetAttribute("innerHTML").Contains(DateAndTime) || ele.Text.Contains(DateAndTime))
                {
                    this.ClickExamListThumbnailIcon(ele);
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prior">Prior WebElement on which thumnailiCon to be clicked</param>
        public void ClickExamListThumbnailIcon(IWebElement prior)
        {
            //ScrollIntoView(prior);
            IWebElement icon = prior.FindElement(By.CssSelector(div_priorsThumbnail));
            ClickElement(icon);
            WaitforThumbnails();
        }

        /// <summary>
        /// This Method is used to get the css of the Exam list thumbnail container
        /// </summary>
        /// <param name="Prior"></param> Prior shoudl start from 1
        /// <returns></returns>
        public String GetExamListThumbnailContainerCss(int Prior)
        {
            String css = div_relatedStudy + ":nth-of-type(" + Prior + ")" + " .thumbnailContainer";
            return css;
        }

        /// <summary>
        /// This Method is used to get the captions of the thumbnail in the study panel
        /// </summary>
        /// <returns></returns>
        public String[] GetStudyPanelThumbnailCaption()
        {
            IList<IWebElement> ActiveThumbnail = BasePage.Driver.FindElements(By.CssSelector(div_thumbnailcontainer + " " + div_thumbnailCaption));

            String[] allText = new String[ActiveThumbnail.Count];
            int i = 0;
            foreach (IWebElement ele in ActiveThumbnail)
            {
                allText[i++] = ele.Text;
            }
            return allText;
        }

        /// <summary>  
        /// This Method is used to return the css of the individual thumbnail in the study panel  
        /// </summary>  
        /// <param name="thumbnailNumber"></param> should starts from 1  
        /// <param name="Studypanel"></param>  should starts from 1
        /// <returns></returns>  
        public String GetStudyPanelThumbnailCss(int thumbnailNumber, int Studypanel = 1)
        {
            int number = thumbnailNumber + 1;
            String css = div_studypanel + ":nth-of-type(" + Studypanel + ") " + div_Studythumbnail + " div:nth-of-type(" + number + ") .thumbnailOuterDiv";
            return css;
        }

        /// <summary>  
        /// This Method is used to get the List of the Thumbnail in the exam list when Panel number is passed  
        /// </summary>  
        /// /// <param name="Panel"></param>  should starts from 1
        /// <returns></returns>  
        public IList<IWebElement> ExamListThumbnailIndicator(int Panel)
        {
            IWebElement ele = Driver.FindElements(By.CssSelector(div_relatedStudyPanel))[Panel];
            if (!ele.FindElement(By.CssSelector("div[class^='thumbnailContainer']")).Displayed)
            {
                ele.FindElement(By.CssSelector("div[class^='thumbnailIcon']")).Click();
                PageLoadWait.WaitForThumbnailsToLoad(100);
            }
            return ele.FindElements(By.CssSelector("div.relatedStudyThumbnailImageComponent"));
        }

        /// <summary>  
        /// This Method is used to return the css of the individual thumbnail in the Exam List  
        /// </summary>  
        /// <param name="Prior"></param>  should start from 1
        /// <param name="thumbnailNumber"></param>  should start from 1
        /// <returns></returns>  
        public String GetExamListThumbnailCss(int Prior, int thumbnailNumber)
        {
            String css = GetExamListThumbnailContainerCss(Prior) + " blu-ring-related-study-thumbnail-image:nth-of-type(" + thumbnailNumber + ") .thumbnailOuterDiv";
            return css;
        }

        /// <summary>
        /// This method is to check if Exam List Section is opened and Displayed.
        /// </summary>
        /// <returns></returns>
        public Boolean IsExamListVisible()
        {
            var priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
            var priorcount = priors.Count;
            bool isPriorDisplayed = false;
            foreach (IWebElement prior in priors)
            {
                if (prior.Displayed) { isPriorDisplayed = true; }
                else { isPriorDisplayed = false; break; }
            }
            return isPriorDisplayed;
        }

        /// <summary>
        /// This method is to open the Thumbnail conatiner from prior
        /// </summary>
        /// <param name="prior">Prior on which Thumbnail needs to be opened</param>
        public void ClickThumailIconPrior(IWebElement prior)
        {
            //Yet to implement
        }

        /// <summary>
        /// This Method is used to return the element of the All thumbnail in the exam list
        /// </summary>
        /// <param name="priornumber"></param> should start form 1
        /// <returns></returns>
        public IWebElement GetExamListThumbnailContainer(int priornumber)
        {
            IWebElement ele = GetElement("cssselector", div_relatedStudy + ":nth-of-type(" + priornumber + ")" + " .thumbnailContainer");
            return ele;
        }

        ///<summary>
        ///This Method is used to get a from Examlist ThumbNail
        ///</summary>
        ///<returns> Get the all series Number(S1) in Thumbnail </returns>
        public String[] GetExamListThumbnailCaption()
        {
            IList<IWebElement> ActiveThumbnail = BasePage.Driver.FindElements(By.CssSelector(div_ExamListthumbnailview + " " + div_thumbnailCaption));
            String[] allText = new String[ActiveThumbnail.Count];
            int i = 0;
            foreach (IWebElement ele in ActiveThumbnail)
            {
                allText[i++] = ele.Text;
            }
            return allText;
        }

		/// <summary>
		/// This method is used to open the Thumbnail conatiner
		/// </summary>
		/// <param name="prior">Prior on which Thumbnail needs to be opened and starts from 0</param>
		public bool OpenExamListThumbnailPreview(int prior = 0, string studyDate = null, string studyTime = null, string accession = null)
        {
            IList<IWebElement> relatedStudyList = Driver.FindElements(By.CssSelector(div_relatedStudyPanel));
            bool status = false;
            if (prior >= 0 && studyDate == null && accession == null)
            {
                IWebElement relatedStudy = Driver.FindElements(By.CssSelector(div_relatedStudyPanel))[prior];
                IWebElement thumbnailContainer = relatedStudy.FindElement(By.CssSelector(div_examListThumbnailContainer));
                if (!thumbnailContainer.Displayed)
                {
                    ClickElement(relatedStudy.FindElement(By.CssSelector(div_priorsThumbnail)));
                    PageLoadWait.WaitForPageLoad(40);
                    WaitforThumbnails();
                    Logger.Instance.InfoLog("Clicked on ExamList Thumbnail Preview Icon");
                    status = true;
                }
                else
                    Logger.Instance.InfoLog("Unable to find the specified Thumbnail.");
            }
            else if (studyTime == null && studyDate != null)
            {
                foreach (IWebElement PriorStudy in relatedStudyList)
                {
                    string date = PriorStudy.FindElement(By.CssSelector(div_examListPanelDate)).GetAttribute("innerHTML");
                    if (date == studyDate)
                    {
                        ClickElement(PriorStudy.FindElement(By.CssSelector(div_priorsThumbnail)));
                        break;
                    }
                }
            }
            else if (studyTime != null)
            {
                foreach (IWebElement PriorStudy in relatedStudyList)
                {
                    string date = PriorStudy.FindElement(By.CssSelector(div_examListPanelDate)).GetAttribute("innerHTML");
                    string time = PriorStudy.FindElement(By.CssSelector(examTimeInExamList)).GetAttribute("innerHTML");
                    if (date == studyDate && time == studyTime)
                    {
                        ClickElement(PriorStudy.FindElement(By.CssSelector(div_priorsThumbnail)));
                        break;
                    }
                }
            }
			else if (accession != null)
			{
				foreach (IWebElement PriorStudy in relatedStudyList)
				{
					var accession_ele = PriorStudy.FindElement(By.CssSelector(AccessionNumberInExamList));
					if (this.GetAccession(accession_ele).Equals(accession))
					{
						ClickElement(PriorStudy.FindElement(By.CssSelector(div_priorsThumbnail)));
						break;
					}

				}
			}
			IWebElement thumbnailPreviewOpened = Driver.FindElement(By.CssSelector(div_ExamList_thumbnails));
            if (thumbnailPreviewOpened.Displayed)
                status = true;

            return status;
        }

        /// <summary>
        /// This method is used to close the Thumbnail conatiner
        /// </summary>
        /// <param name="prior">Prior on which Thumbnail needs to be Closed and starts from 0</param>
        public void CloseExamListThumbnailPreviewWindow(int prior)
        {
            IWebElement relatedStudy = Driver.FindElements(By.CssSelector(div_relatedStudyPanel))[prior];
            IWebElement thumbnailContainer = relatedStudy.FindElement(By.CssSelector(div_examListThumbnailContainer));
            if (thumbnailContainer.Displayed)
            {
                ClickElement(relatedStudy.FindElement(By.CssSelector(div_priorsThumbnail)));
                PageLoadWait.WaitForPageLoad(40);
                WaitforThumbnails();
                Logger.Instance.InfoLog("Clicked on ExamList Thumbnail Preview Icon");
            }
        }

        /// <summary>
        /// This Method will verify the % viewed in thumbnail
        /// </summary>
        /// <param name="element"></param> // Thumnail element
        /// <param name="totalImages"></param> // total images of Series / total frames of an image
        /// <param name="viewedImages"></param> // viewed images / frames
        /// <returns></returns>
        public bool VerifyThumbnailPercentImagesViewed(IWebElement element, int totalImages, int viewedImages)
        {
            Thread.Sleep(3000);
            Decimal percentViewed1 = Decimal.Divide(viewedImages, totalImages) * 100;
            int percentViewed = (int)Math.Floor(percentViewed1);
            String percentViewedinUI = element.GetAttribute("innerHTML").Trim();
            if (percentViewedinUI.Equals(percentViewed.ToString() + "%"))
            {
                Logger.Instance.InfoLog("Percent Viewed - " + percentViewed + " and " + percentViewedinUI + " Matched");
                return true;
            }
            Logger.Instance.InfoLog("Percent Viewed - " + percentViewed + " and " + percentViewedinUI + " Not Matched");
            return false;
        }

        /// <summary>
        /// This Method will verify the % viewed in list of thumbnails
        /// </summary>
        /// <param name="percentViewedElements"></param> // Thumnails list
        /// <param name="totalImages"></param> // list total images of Series / total frames of an image
        /// <param name="viewedImages"></param> // list of viewed images / frames
        /// <returns></returns>
        public bool VerifyThumbnailPercentImagesViewed(IList<IWebElement> percentViewedElements, IList<int> totalImages, IList<int> viewedImages, int NumberOfThumbnails = 0)
        {
            Thread.Sleep(3000);
            if (NumberOfThumbnails == 0)
            {
                NumberOfThumbnails = percentViewedElements.Count();
            }
            int index = 0;
            if (percentViewedElements.Count() > 0)
            {
                while (NumberOfThumbnails != 0)
                {
                    Decimal percentViewed1 = Decimal.Divide(viewedImages[index], totalImages[index]) * 100;
                    int percentViewed = (int)Math.Floor(percentViewed1);
                    //   percentViewed = Math.Round(percentViewed, 2);
                    String percentViewedinUI = percentViewedElements.ElementAt(index).GetAttribute("innerHTML").Trim();
                    if (!percentViewedinUI.Equals(percentViewed.ToString() + "%"))
                    {
                        Logger.Instance.InfoLog("Percent Viewed - " + percentViewed + " and " + percentViewedinUI + " Not Matched");
                        return false;
                    }
                    index++;
                    NumberOfThumbnails--;
                }
                Logger.Instance.InfoLog("Percent Viewed Matched");
                return true;
            }
            else
                Logger.Instance.InfoLog("Percent Viewed Element is not available");
            return false;            
        }

        /// <summary>
        /// This Method will verify the number of images in series or number of frames in an Image
        /// </summary>
        /// <param name="FrameElements"></param> // Thumnails list
        /// <param name="ImagesCount"></param> // Total images / frames list		
        /// <returns></returns>
        public bool VerifyThumbnailFrameNumber(IList<IWebElement> FrameElements, IList<String> ImagesCount, int NumberOfThumbnails = 0)
        {
            Thread.Sleep(3000);
            if (NumberOfThumbnails == 0)
            {
                NumberOfThumbnails = FrameElements.Count();
            }
            int index = 0;
            while (NumberOfThumbnails != 0)
            {
                if (!(FrameElements.ElementAt(index).GetAttribute("innerHTML").Trim() == ImagesCount[index]))
                {
                    Logger.Instance.InfoLog("Image/Frame count is not matched");
                    return false;
                }
                index++;
                NumberOfThumbnails--;
            }
            Logger.Instance.InfoLog("Image/Frame count is matched");
            return true;

        }

        /// <summary>
        /// This method is to get the color of the background or text inside a cardio report(PDF)
        /// </summary>
        /// <param name="text"></param>
        /// <param name="reporttype">PDF/SR</param>
        /// <returns></returns>
        public String GetColorInReport(int text = 0, string reporttype = "PDF")
        {
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            String script = null;
            if (reporttype == "PDF") //Acc :REP12311 Cardio study
            {
                this.NavigateToReportFrame(reporttype: "PDF");
                if (text != 0)
                {
                    if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("chrome")))
                    {
                        script = "function bgcolor(){ var x =document.querySelectorAll(\"html svg g[clip - path]\")[7].querySelector('svg path').getAttribute('fill'); return x;}return bgcolor()";
                    }
                    else
                    {   //clippath7
                        script = "function bgcolor(){ var x = document.documentElement.querySelectorAll(\'svg g[clip-path=\"url(#clippath4)\"] path\')[0].getAttribute('fill'); return x;}return bgcolor()";
                    }
                }
                else
                {
                    if (!(((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("chrome")))
                    {
                        script = "function textcolor(){ var x = document.querySelectorAll(\'html svg g[clip-path=\"url(#clippath7)\"] text>tspan\')[0].getAttribute('fill'); return x;}return textcolor()";
                    }
                    else
                    {  //clippath7
                        script = "function textcolor(){ var x = document.documentElement.querySelectorAll(\'svg g[clip-path=\"url(#clippath5)\"] text>tspan\')[0].getAttribute('fill'); return x;}return textcolor()";
                    }
                }
            }
            else if (reporttype == "OT")
            {
                this.NavigateToReportFrame(reporttype: "SR");
                if (text != 0)
                {
                    //script = "function bgcolor(){ var x = document.documentElement.querySelectorAll(\'svg g[clip-path=\"url(#clippath7)\"] path\')[0].getAttribute('fill'); return x;}return bgcolor()";
                    script = "function bgcolor(){ var x = document.documentElement.querySelectorAll(\'svg g[clip-path=\"url(#clippath7)\"] path\')[0].getAttribute('fill'); return x;}return bgcolor()";
                }
                else
                {
                    script = "function textcolor(){ var x = document.documentElement.querySelector('iframe').contentDocument.querySelector('div#screenLockDiv2 tr>td>table').getAttribute('style') ; return x;}return textcolor()";
                    //script = "function textcolor(){ var x = document.documentElement.querySelector('object').contentDocument.querySelector('div#screenLockDiv2 tr>td>table').getAttribute('style') ; return x;}return textcolor()";
                }
            }
            else //Common for all SR reports
            {
                this.NavigateToReportFrame(reporttype: "SR");
                if (text != 0)
                {
                    script = "function bgcolor(){ var x = document.documentElement.querySelector('iframe').contentDocument.querySelector('body').getAttribute('bgcolor'); return x;}return bgcolor();";
                    // script = "function bgcolor(){ var x = document.documentElement.querySelector('object').contentDocument.querySelector('body').getAttribute('bgcolor'); return x;}return bgcolor();";
                }
                else
                {
                    script = "function textcolor(){ var x = document.documentElement.querySelector('iframe').contentDocument.querySelector('body.radiologist_report').getAttribute('text'); return x;}return textcolor();";
                    //script = "function textcolor(){ var x = document.documentElement.querySelector('object').contentDocument.querySelector('body.radiologist_report').getAttribute('text'); return x;}return textcolor();";
                }
            }
            var Color = ((IJavaScriptExecutor)Driver).ExecuteScript(script);
            Logger.Instance.InfoLog("Color fetched is: " + Color.ToString());
            return Color.ToString();
        }

        /// <summary>
        /// Switch to Report Frame
        /// </summary>
        /// <param name="reporttype">PDF/SR</param>
        public void NavigateToReportFrame(int nthframe = 0, string reporttype = "SR", bool Guest = false)
        {
            if (!Guest)
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            else
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("GuestHomeFrame");
            if (reporttype == "SR")
            {
                var iframes = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.SRReport_iframe));
                BasePage.Driver.SwitchTo().Frame(iframes[nthframe]);
            }
            else if(reporttype=="MergeportReport")
            {
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                Logger.Instance.InfoLog("Switched to UserHomeFrame");
                var iframes = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.SRReport_iframe));
                BasePage.Driver.SwitchTo().Frame(iframes[nthframe]);
                Logger.Instance.InfoLog("Switched to first inner frame: "+BluRingViewer.SRReport_iframe);
                var iframe1 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.MergeportReport_iframe));
                BasePage.Driver.SwitchTo().Frame(iframe1[nthframe]);
                Logger.Instance.InfoLog("Switched to second inner frame: "+ BluRingViewer.MergeportReport_iframe);                
            }
            else
            {
                //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                if (!Guest)
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                else
                    BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("GuestHomeFrame");
                Logger.Instance.InfoLog("Switched to UserHomeFrame");
                var iframes = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.SRReport_iframe));
                BasePage.Driver.SwitchTo().Frame(iframes[nthframe]);
                var iframe1 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.Pdf_iframe));
                BasePage.Driver.SwitchTo().Frame(iframe1[nthframe]);
                Logger.Instance.InfoLog("Switched to first inner frame");
                //var iframe2 = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.PdfViewer_iframe));
                //BasePage.Driver.SwitchTo().Frame(iframe2[nthframe]);
                //Logger.Instance.InfoLog("Switched to second inner frame");
            }
        }

        /// <summary>
        /// This method is to open a specific report from ExamList section 
        /// And Navigates to specific frame
        /// </summary>
        /// <param name="prior"></param>
        public void OpenReport_BR(int priorcount, string reporttype = "SR", int frameIndex = -1, string accession = null, bool Guest = false)
        {
            WebDriverWait wait1 = new WebDriverWait(Driver, new TimeSpan(0, 0, 120));
            int count = 0, framecount = -1 ;            
            if (frameIndex == -1)           
               frameIndex = priorcount; 
            
            PageLoadWait.WaitForFrameLoad(5);
            IList<IWebElement> reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon));              
            IList<string> ACCPriors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors)).
                Select(pr => GetAccession(pr.FindElement(By.CssSelector(BluRingViewer.AccessionNumberInExamList)))).ToArray();

            //Open Report
            if (accession != null)
            {
                foreach (string acc in ACCPriors)
                {                    
                    if (acc.Equals(accession))
                    {
                        Thread.Sleep(2000);
                        framecount++;
                        break;
                    }
                    count++;
                }
                this.ClickElement(reportIcon[count]);
            }
            else
            {
                this.ClickElement(reportIcon[frameIndex]);
            }           
            
            if (reporttype == "SR")
            {
                //Navigate to To Report Frame
                if (accession != null)
                {
                    this.NavigateToReportFrame(framecount);
                }
                else
                {
                    this.NavigateToReportFrame(Guest: Guest);
                }
                //Synch Up
                var report_conatiners = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.pdfreport_continer));
                if (report_conatiners.Count > 1)
                {
                    wait1.Until<Boolean>(d => report_conatiners[priorcount].Displayed == true);
                }
                else
                {
                    Thread.Sleep(5000);
                }
            }
            else
            {               
                if (accession != null)
                {
                    this.NavigateToReportFrame(count - 1,"PDF");
                }
                else
                {
                    if (ACCPriors.Count == 1)
                    frameIndex = frameIndex + 1;
                    this.NavigateToReportFrame(frameIndex-1,"PDF");
                }

                var report_conatiners = BasePage.Driver.
                    FindElements(By.CssSelector(BluRingViewer.PDFContainer_div));
                if (report_conatiners.Count > 1)
                {
                    wait1.Until<Boolean>(d => report_conatiners[priorcount].Displayed == true);
                }
                else
                {
                    wait1.Until<Boolean>(d => report_conatiners[0].Displayed == true);
                }
            }

            Logger.Instance.InfoLog("Report is opened..");
            Thread.Sleep(1000);
        }

        /// <summary>
        /// This method is to close a specific report from ExamList section 
        /// </summary>
        /// <param name="prior"></param>
        public void CloseReport_BR(int prior, bool Guest = false)
        {
            if (!Guest)
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            else
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("GuestHomeFrame");
            IList<IWebElement> reportIcon = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priorsreportIcon));
            reportIcon[prior].Click();
            Logger.Instance.InfoLog("Report is closed..");
            Thread.Sleep(2000);
        }

        /// <summary>
        ///  This methos will get the entire report in a Dictionary
        /// </summary>
        /// <param name="priorcount">Prior count, should start from zero</param>
        /// <param name="reportType">Report Type</param>
        /// <returns></returns>
        public Dictionary<String, String> FetchReportData_BR(int priorcount, String reportType = "PDF")
        {
            IList<Object> reportdata = null;
            Dictionary<String, String> report = new Dictionary<String, String>();
            String script = System.IO.File.ReadAllText("Scripts\\JSFiles\\FetchReportData.js");
                                                   //Pdf_iframe
            var parameters = "\"" + BluRingViewer.SRReport_iframe + "\"" + "," +
                "\"" + BluRingViewer.pdfreport_continer + "\"" + ", " + priorcount;

            script = script + "return getReportData(" + parameters + ");";
            BasePage.Driver.SwitchTo().DefaultContent();
            reportdata = (IList<Object>)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);

            foreach (Object data in reportdata)
            {
                var keyvalue = ((String)data).Replace("\n", "").Split('=');
                report.Add(keyvalue[0], keyvalue[1]);
            }
            return report;
        }

        /// <summary>
        /// This method will select the report based on prior and report index
        /// </summary>
        /// <param name="priorcount">Start rom zero</param>
        /// <param name="reportindex">Starts from zero</param>
        public void SelectReport_BR(int priorcount, int reportindex, string reporttype = "SR", bool Guest = false)
        {
            //BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            if (!Guest)
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            else
                BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("GuestHomeFrame");
            var reportContainer = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.ReportContainer_div))[priorcount];
            IList<IWebElement> reportlist = reportContainer.FindElements(By.CssSelector(BluRingViewer.ReportTabList_div));
            reportlist[reportindex].Click();
            if (reporttype == "SR")
            {
                this.NavigateToReportFrame(priorcount);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(pdfreport_continer)));
                var report_conatiners = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.pdfreport_continer));
                if (report_conatiners.Count > 1)
                {
                    BasePage.wait.Until<Boolean>(d => report_conatiners[priorcount].Displayed == true);
                }
                else
                {
                    BasePage.wait.Until<Boolean>(d => report_conatiners[0].Displayed == true);
                }
            }
            else if (reporttype == "AU")
            {
                this.NavigateToReportFrame(priorcount);
                wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(AUReport_div)));
                var report_conatiners = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.AUReport_div));
                if (report_conatiners.Count > 1)
                {
                    BasePage.wait.Until<Boolean>(d => report_conatiners[priorcount].Displayed == true);
                }
                else
                {
                    BasePage.wait.Until<Boolean>(d => report_conatiners[0].Displayed == true);
                }
            }
            else
            {
                this.NavigateToReportFrame(reporttype: "PDF", Guest: Guest);
                var report_conatiners = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.PDFContainer_div));
                if (report_conatiners.Count > 1)
                {
                    BasePage.wait.Until<Boolean>(d => report_conatiners[priorcount].Displayed == true);
                }
                else
                {
                    BasePage.wait.Until<Boolean>(d => report_conatiners[0].Displayed == true);
                }
            }
            Thread.Sleep(2000);
        }

        /// <summary>  
        /// This Method is used to get the List of the Thumbnail in the Study panel when Panel number is passed  
        /// </summary>  
        /// /// <param name="Panel"></param>  should starts from 1
        /// <returns></returns>
        public IList<IWebElement> StudyPanelThumbnailIndicator(int StudyPanel)
        {
            IList<IWebElement> StudyPanels = BasePage.Driver.FindElements(By.CssSelector(div_StudyPanel));
            return StudyPanels[StudyPanel].FindElements(By.CssSelector(div_studyPanelThumbnailImageComponent));
        }

        /// <summary>
        /// This method is to get the current page number of PDF report
        /// </summary>
        /// <returns></returns>
        public string GetCurrentPageNumber()
        {
            NavigateToReportFrame(reporttype: "PDF");
            //string script = "document.querySelector('input#pageNumber').value;";
            //var PageNo = ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);            
            IWebElement pg = BasePage.Driver.FindElement(By.CssSelector("input#pageNumber"));
            var PageNo = pg.GetAttribute("value");
            Logger.Instance.InfoLog("Current PageNo' : " + PageNo.ToString());
            return PageNo.ToString();
        }

        /// <summary>
        /// This method is to download PDF report from Tools menu
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="filepath"></param>
        /// <param name="filetype">pdf</param>
        /// <returns></returns>
        public bool DownloadPDF_BR(string filename, string filepath, string filetype = "pdf")
        {
            //Deleting existing file
            new List<string>(Directory.GetFiles(filepath)).ForEach(file1 =>
            {
                if (file1.IndexOf(filename, StringComparison.OrdinalIgnoreCase) >= 0)
                    File.Delete(file1);
            });
            NavigateToReportFrame(reporttype: "PDF");
            IWebElement ToolsIcon = BasePage.Driver.FindElement(By.CssSelector(PDFToolsBtn));
            ToolsIcon.Click();
            Logger.Instance.InfoLog("Tools button is clicked..");
            BasePage.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(PDFDownladBtn)));
            IWebElement DwnldBtn = BasePage.Driver.FindElement(By.CssSelector(PDFDownladBtn));
            DwnldBtn.Click();
            Logger.Instance.InfoLog("Download button is clicked..");
            PageLoadWait.WaitForFileDownload(filepath, filename, filetype, 2);
            bool FileExists = CheckFile(filename, filepath, filetype);
            Logger.Instance.InfoLog("FileExists status is: " + FileExists);
            return FileExists;
        }

        /// <summary>
		/// This method is to open the viewer tool popup in a specified coordinates.
		/// </summary>
		/// <param name="clickX"></param>  
		/// <param name="clickY"></param> 
		public void OpenViewerToolsPOPUp(int clickX, int clickY)
        {
            var viewport = BasePage.Driver.FindElement(By.CssSelector(this.Activeviewport));
            if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("chrome")) ||
                (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
            {
                new Actions(BasePage.Driver).MoveToElement(viewport, clickX, clickY).Build().Perform();
                new Actions(BasePage.Driver).ContextClick().Build().Perform();
            }
            else if (SBrowserName.ToLower().Contains("edge"))
            {

                IJavaScriptExecutor executor = (IJavaScriptExecutor)Driver;
                string javaScript = "var evt = document.createEvent('MouseEvents');"
                    + "var RIGHT_CLICK_BUTTON_CODE = 2;"
                    + "evt.initMouseEvent('contextmenu', true, true, window, 1, 0, 0, arguments[1], arguments[2], false, false, false, false, RIGHT_CLICK_BUTTON_CODE, null);"
                    + "arguments[0].dispatchEvent(evt)";
                executor.ExecuteScript(javaScript, viewport, clickX, clickY);
            }
            else
            {
                var actions = new TestCompleteAction();
                actions.MoveToElement(viewport, clickX, clickY);
                actions.ContextClick().Perform();
            }
            Thread.Sleep(2000);
        }

        /// <summary>
        /// This Study will email a study from a new BluRing viewer
        /// </summary>
        /// <returns></returns>
        public String EmailStudy_BR(String emailaddr = null, String name = null, String notes = null, bool DeleteEmail = true)
        {
            String pinnumber = String.Empty;
            emailaddr = String.IsNullOrEmpty(emailaddr) ? Config.emailid : emailaddr;
            name = String.IsNullOrEmpty(name) ? "Testing" : name;
            notes = String.IsNullOrEmpty(notes) ? "Testing" : notes;

            //Delete all email notification
            if(DeleteEmail == true)
                Pop3EmailUtil.DeleteAllMails(Config.emailid, Config.Email_Password);

            //Email Study
            this.ClickElement(BasePage.Driver.FindElement(By.CssSelector(div_emailstudy)));
            this.WaitTillEmailWindowAppears();
            this.SendKeys(BasePage.Driver.FindElement(By.CssSelector(input_emailName)), name);
            this.SendKeys(BasePage.Driver.FindElement(By.CssSelector(input_email)), emailaddr);
            //this.SendKeys(BasePage.Driver.FindElement(By.CssSelector(input_confirmemail)), emailaddr);
            this.SendKeys(BasePage.Driver.FindElement(By.CssSelector(input_Notes)), notes);
            IWebElement sendButton = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.div_sendEmail));

            ClickElement(sendButton);
            BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(div_pinWindow)));
            
            //Fetch Pin
            var pinwindow = BasePage.Driver.FindElement(By.CssSelector(div_pinWindow));
            pinnumber = pinwindow.FindElement(By.CssSelector(".dialogFooter label")).GetAttribute("innerHTML");
            this.ClickElement(pinwindow.FindElement(By.CssSelector("span")));
            BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(div_pinWindow)));
            //var cancel_button = BasePage.Driver.FindElements(By.CssSelector("button[type='submit']")).ToList<IWebElement>().Find(element =>
            //element.GetAttribute("innerHTML").Equals("Cancel"));
            //cancel_button.Click();
            //this.WaitTillEmailWindowDisAppears();

            return pinnumber;
        }

        /// <summary>
        /// This method will wait till the email pop occurs.
        /// </summary>
        public void WaitTillEmailWindowAppears(bool isPriorStudies = false)
        {
            BasePage.wait.Until<Boolean>(d =>
            {
                if (d.FindElement(By.CssSelector(div_emailWindow)).Displayed)
                {
                    if (isPriorStudies)
                    {
                        BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(BluRingViewer.div_emailSelectAll)));
                        BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(BluRingViewer.div_emailRelatedStudyList)));
                        Thread.Sleep(5000);
                    }
                    return true;
                }

                else
                {
                    return false;
                }

            });

        }

        /// <summary>
        /// This method will wait till the email pop dis appears
        /// </summary>
        public void WaitTillEmailWindowDisAppears()
        {
            BasePage.wait.Until<Boolean>(d =>
            {
                if (d.FindElements(By.CssSelector(div_emailWindow)).Count == 0)
                {
                    return true;
                }

                else
                {
                    return false;
                }

            });
        }

        /// <summary>
        /// This method is to verify the color with help of element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public bool VerifyBordorColor(IWebElement element, string color)
        {
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("firefox") ||
                ((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("edge"))
            {
                string[] array = color.Split('(');
                string[] rgb = array[1].Split(',');
                string colorCode = "rgb(" + rgb[0] + "," + rgb[1] + "," + rgb[2] + ")";

                if (element.GetCssValue("border-top-color").Equals(colorCode) && element.GetCssValue("border-bottom-color").Equals(colorCode) &&
                        element.GetCssValue("border-right-color").Equals(colorCode) && element.GetCssValue("border-left-color").Equals(colorCode))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (element.GetCssValue("border-top-color").Equals(color) && element.GetCssValue("border-bottom-color").Equals(color) &&
                        element.GetCssValue("border-right-color").Equals(color) && element.GetCssValue("border-left-color").Equals(color))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>  
        /// Pixel Value Measurement - To Hold on the Image and measure Pixel Value.  
        /// </summary>  
        public void ApplyTool_PixelValueByClickAndHold(int dragStartX = 0, int dragStartY = 0)
        {
            if (dragStartX == 0 && dragStartY == 0)
            {
                var attributes = GetElementAttributes(this.Activeviewport);
                dragStartX = attributes["width"] / 2;
                dragStartY = attributes["height"] / 2;
            }
            if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("chrome")) ||
            (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
            {
                new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dragStartX, dragStartY).Build().Perform();
                Thread.Sleep(2000);
                new Actions(BasePage.Driver).ClickAndHold().Build().Perform();
            }
            else
            {
                var actions = new TestCompleteAction();
                actions.MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dragStartX, dragStartY).ClickAndHold().Perform();
                Thread.Sleep(2000);
                ScrollIntoView(GetElement("cssselector", div_closeStudy));
            }
        }

        /// <summary>
        /// This method will check if all the accession present in input array is present in the exam list
        /// </summary>
        /// <param name="accessionnumber"></param>
        /// <returns></returns>
        public bool CheckAccession_ExamList(String[] accessionnumber)
        {
            bool isAllAccessionPresent = false;
            var exams = BasePage.Driver.FindElements(By.CssSelector(AccessionNumberInExamList));

            var acclist = exams.Select<IWebElement, String>(element =>
            {
                return this.GetAccession(element);
            }).ToArray<String>();

            //Compate arrays
            isAllAccessionPresent = (accessionnumber.Length == acclist.Length &&
                accessionnumber.Intersect(acclist).Count() == accessionnumber.Length);

            //print logs for debugging
            foreach (var acc in acclist) { Logger.Instance.InfoLog(acc + "\n"); }
            foreach (var acc in accessionnumber) { Logger.Instance.InfoLog(acc + "\n"); }

            return isAllAccessionPresent;
        }

        /// <summary>
        /// This method gets the accession number
        /// </summary>
        /// <param name="acession_ele">This parameter is prior Webelement</param>
        /// <returns></returns>
        public String GetAccession(IWebElement acession_ele)
        {
            String accession = String.Empty;

            var innertext = acession_ele.GetAttribute("innerHTML");
            accession = innertext.Replace("<label>Acc:</label>", "");
            return accession.Trim().Replace(" ", "");

        }
		/// <summary>
		/// This method gets the prior number using accession 
		/// </summary>
		/// <param name="accession">This parameter is accession number</param>
		/// <returns>int</returns>
		public int GetPriorNumber(string accession)
		{
			int priorNo = 1;
			IList<IWebElement> relatedStudyList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_relatedStudyPanel));
			foreach (IWebElement PriorStudy in relatedStudyList)
			{
				var accession_ele = PriorStudy.FindElement(By.CssSelector(BluRingViewer.AccessionNumberInExamList));
				if (GetAccession(accession_ele).Equals(accession))
				{
					return priorNo;
				}
				priorNo++;
			}
			return -1;
		}

		/// <summary>  
		/// This Method is used to Apply Draw Roi  
		/// </summary>  
		/// <param name="dragStartX"></param>  
		/// <param name="dragStartY"></param>  
		/// <param name="dropX"></param>  
		/// <param name="dropY"></param>  
		public void ApplyTool_DrawRoi(int dragStartX = 0, int dragStartY = 0, int dropX = 0, int dropY = 0, bool isClick = true)
        {
            if (dragStartX == 0 && dragStartY == 0 && dropX == 0 && dropY == 0)
            {
                var attributes = GetElementAttributes(this.Activeviewport);
                dragStartX = attributes["width"] / 2;
                dragStartY = attributes["height"] / 2;
                dropX = attributes["width"] / 4;
                dropY = attributes["height"] / 4;
            }
            if (isClick)
            {
                this.PerformTool(dragStartX, dragStartY, dropX, dropY);
            }
            else
            {
                if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("chrome")) ||
                (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
                {
                    new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dragStartX, dragStartY).Build().Perform();
                    Thread.Sleep(2000);
                    new Actions(BasePage.Driver).Click().Build().Perform();
                    Thread.Sleep(2000);
                    new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dropX, dropY).Build().Perform();
                    Thread.Sleep(2000);
                    new Actions(BasePage.Driver).Click().Build().Perform();
                    Thread.Sleep(2000);
                }
                else
                {
                    var action = new TestCompleteAction();
                    action.MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dragStartX, dragStartY);
                    Thread.Sleep(2000);
                    action.Click();
                    Thread.Sleep(2000);
                    action.MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), dropX, dropY).Click().Perform();
                    Thread.Sleep(2000);
                    ScrollIntoView(GetElement("cssselector", div_closeStudy));
                }
            }
            Thread.Sleep(Config.ms_minTimeout);
        }

        /// <summary>
        /// This Method is used to apply Add Text tool
        /// </summary>
        /// <param name="text"></param>
        public void ApplyTool_AddText(String text, int clickX = 0, int clickY = 0)
        {

            if (clickX == 0 && clickY == 0)
            {
                var attributes = GetElementAttributes(this.Activeviewport);
                clickX = attributes["width"] / 4;
                clickY = attributes["height"] / 4;
            }
            if ((((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("chrome")) ||
                (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("explorer")))
            {
				new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), clickX, clickY).Build().Perform();
                Thread.Sleep(2000);
                new Actions(BasePage.Driver).Click().Build().Perform();
                Thread.Sleep(2000);
            }
            else
            {
                //var action = new TestCompleteAction();
				new TestCompleteAction().MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), clickX, clickY).Click().Perform();
				new TestCompleteAction().MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), clickX, clickY).Click().Perform();

				Thread.Sleep(2000);
                
            }
            BasePage.wait.Until<Boolean>(d => d.FindElement(By.CssSelector("input[type='text']")) != null);
            BasePage.Driver.FindElement(By.CssSelector(this.Activeviewport)).FindElement(By.CssSelector("input[type='text']")).SendKeys(text);
            BasePage.Driver.FindElement(By.CssSelector(this.Activeviewport)).FindElement(By.CssSelector("input[type='text']")).SendKeys(Keys.Enter);
        }

        /// <summary>
        /// This method will get the list of tools available in toolbox
        /// </summary>
        /// <returns></returns>
        public IList<String> GetToolsInViewer()
        {
            IList<String> tools = new List<String>();
            tools = Driver.FindElements(By.CssSelector(div_Toolbox + " " + div_toolWrapper)).Select<IWebElement, String>
                        (tool => tool.GetAttribute("title")).ToList();
            return tools;
        }

        /// <summary>
        /// This method will get the Groups in use from ToolBox
        /// </summary>
        /// <returns></returns>
        public IList<IWebElement> GetGroupsInToolBox(int viewportNumber = 1, int panelNumber = 1)
        {
             return Driver.FindElements(By.CssSelector(div_studypanel + ":nth-of-type(" + panelNumber + 
                    ") div.viewerContainer:nth-of-type(" + viewportNumber + ") " + div_toolGrid));
        }

        /// <summary>
        /// This method returns the list of Tool names in the BluRing viewer as mentioned in Management Page
        /// </summary>
        /// <returns></returns>
        public static IList<String> GetViewerToolsNameAsInMgmtPage(IList<String> toolsInViewer)
        {
            Dictionary<String, String> toolsName = new Dictionary<String, String>();
            toolsName.Add("Line Measurement", "Annotation OrthoLine");
            toolsName.Add("Draw Ellipse", "Roi Circle");
            toolsName.Add("Draw Rectangle", "Roi Rectangle");
            toolsName.Add("Angle Measurement", "Measure Angle");
            toolsName.Add("Interactive Window Width/Level", "Window Level");
            toolsName.Add("Distance Calibration", "Calibration");
            toolsName.Add("Invert Greyscale Image", "Invert");
            toolsName.Add("Draw ROI", "Roi Draw");
            toolsName.Add("Save Annotated Series", "Save Series");
            toolsName.Add("AllInOne", "All in One");
            toolsName.Add("Pixel Value", "Get Pixel Value");

            IList<String> viewerTools = new List<String>();

            foreach (var tool in toolsInViewer)
            {
                var modifiedTool = tool;
                var changeRequired = false;
                while ((toolsName.Keys).Any(vt =>
                {
                    if ((modifiedTool.Split(',').Any(t => t.Equals(vt))))
                    {
                        modifiedTool = modifiedTool.Replace(vt, toolsName[vt]);
                        changeRequired = true;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                })) ;
                if (changeRequired)
                {
                    viewerTools.Add(modifiedTool);
                }
                else
                {
                    viewerTools.Add(tool);
                }
            }
            return viewerTools;
        }

        /// <summary>
        /// This method will get the Tools name list based on GridNumber in the Bluering Viewer ToolsBox Pop-up
        /// </summary>
        /// <returns></returns>
        public IList<String> GetToolsInToolBoxByGrid(int gridnumber = 0)
        {
            IList<IWebElement> GridList = Driver.FindElements(By.CssSelector(div_toolGrid));
            IList<String> toolsInGrid = new List<String>();
            IList<string> resultList = new List<string>();
            foreach (var Grid in GridList)
            {
                IWebElement gridelemnet = Grid;
                if (gridnumber != 0)
                {
                    gridelemnet = GridList[gridnumber - 1];
                }

                String tools = "";
                try
                {
                    var stackedTool = gridelemnet.FindElement(By.CssSelector(BluRingViewer.div_activetoolsContainer));
                    tools = stackedTool.GetAttribute("title").ToString();
                    var toolList = gridelemnet.FindElement(By.CssSelector(BluRingViewer.div_expandedtoolsContainer));
                    resultList = toolList.FindElements(By.CssSelector(BluRingViewer.div_toolWrapper)).Select<IWebElement, String>
                    (tool => tool.GetAttribute("title")).ToList();
                    tools = tools + "," + String.Join(",", resultList);
                }
                catch (NoSuchElementException)
                {
                    resultList = gridelemnet.FindElements(By.CssSelector(BluRingViewer.div_toolWrapper)).Select<IWebElement, String>
                    (tool => tool.GetAttribute("title")).ToList();
                    tools = String.Join(",", resultList);
                }

                toolsInGrid.Add(tools);

                if (gridnumber != 0)
                    break;

            }
            return toolsInGrid;
        }

        /// <summary>
        /// This method will verify the configured tools from ToolBox with the given set of tools. Return true if both matches and false if not.
        /// </summary>
        /// <param name="expectedTools"></param>  // Stacked tools should be separated by ","
        /// <returns></returns>
        public bool VerifyConfiguredTools(String[] expectedTools, int panelNumber = 1, int viewportNumber = 1)
        {
            int i = 0;
            bool isMatched = true;
            var groupsInUse = GetGroupsInToolBox(viewportNumber, panelNumber);
            if (groupsInUse.Count == 0)
            {
                Logger.Instance.ErrorLog("No Tools are available");
                return false;
            }
            foreach (IWebElement ele in groupsInUse)
            {
                IList<String> tools = ele.FindElements(By.CssSelector(div_toolWrapper)).Select<IWebElement, String>
                        (tool => tool.GetAttribute("title")).ToList();
                int toolsCount = tools.Count;
				if(toolsCount == 0)
                {
                    //  continue;
                    tools.Add("");
                }
                if(toolsCount > 1)
                {
                    String str = tools.ElementAt(toolsCount - 1);
                    tools.RemoveAt(toolsCount - 1);
                    tools.Insert(0, str);
                }
                String[] toolsInGroup = expectedTools[i].Split(',');
                if (tools.Count() == toolsInGroup.Count())
                {
                    int j = 0;
                    foreach (String tool in tools)
                    {
                        if (SBrowserName.ToLower().Equals("firefox"))  // Firefox Specific: In Edit Domain management page, tool is not dropped at the end of column. It dropped in between the column. So added separate login for firefox
                        {
                            if (!toolsInGroup.Contains(tool))
                            {
                                isMatched = false;
                                Logger.Instance.ErrorLog(tool + " is not available in coulmn " + j);
                                break;
                            }
                            Logger.Instance.ErrorLog(tool + " is available in column " + j);
                        }
                        else
                        {
                            if (!tool.Equals(toolsInGroup[j]))
                            {
                                isMatched = false;
                                Logger.Instance.ErrorLog("Tool Mismatched   : Expected Tool - " + toolsInGroup[j] + "   Actual Tool - " + tool);
                                break;
                            }
                            Logger.Instance.InfoLog("Tool Matched    : Expected Tool - " + toolsInGroup[j] + "   Actual Tool - " + tool);
                        }                        
                        j++;
                    }
                }
                else
                {
                    isMatched = false;
                    Logger.Instance.ErrorLog("Tools count Mismatched");
                    break;
                }
                i++;
            }
            return isMatched;
        }

        /// <summary>
        /// Method created to Save PR for Enterprise Viewer (Series/Annotated Images)
        /// </summary>
        /// <param name="prTool">Provide PR related tool as input (Save Series/Annotated Images, etc)</param>
        /// <param name="outerTool">Provide outer tool. Outer tool will change on selection</param>
        public bool SavePresentationState(BluRingTools prTool, BluRingTools outerTool = BluRingTools.Add_Text, int panel = 1, int viewport = 1, bool isOpenToolsPOPup = true)
        {
            int count1 = 0;
            int count2 = 0;
            IList<IWebElement> Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
            count1 = Thumbnail_list.Count;
            if (prTool == BluRingTools.Save_Series || prTool == BluRingTools.Save_Annotated_Image)
            {
                Thread.Sleep(3000);
                SelectInnerViewerTool(prTool, outerTool, isOpenToolsPOPup, true, panel, viewport);
                //Sync Up
                try
                {
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(div_PRStatusIndicator)));
                    wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(div_PRStatusIndicator)));
                }
                catch (Exception ex) { }
                WaitforThumbnails();
                Thumbnail_list = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_thumbnails));
                count2 = Thumbnail_list.Count;
                if (count1 < count2)
                {
                    Logger.Instance.InfoLog("Thumbnails count before:" + count1 + "Thumbanils count after:" + count2);
                    return true;
                }
                else
                {
                    Logger.Instance.ErrorLog("Thumbnails count before:" + count1 + "Thumbanils count after:" + count2);
                    return false;
                }
            }
            else
            {
                Logger.Instance.ErrorLog("Save Presentation State method not invoked since invalid tool provided as input");
                return false;
            }
        }

        #region CardioCINE
        /// <summary>
        /// This Method is used to play CINE in particular view port
        /// </summary>
        /// <param name="viewPort"></param> 
        /// <param name="panelNumber"></param>         
        public bool PlayCINE(int viewPort = 1, int panelNumber = 1)
        {
            OpenCineToolBar(viewPort, panelNumber);
            /*String PlayButton = div_studypanel + ":nth-of-type(" + panelNumber + ")"
                                + " div.viewerContainer:nth-of-type(" + viewPort + ") "
                                + div_CINE_PlayBtn;
            String PauseButton = div_studypanel + ":nth-of-type(" + panelNumber + ")"
                                + " div.viewerContainer:nth-of-type(" + viewPort + ") "
                                + div_CINE_PauseBtn;*/
            String PlayButton = div_CINE_PlayBtn;
            String PauseButton = div_CINE_PauseBtn;
            ClickElement(SetViewPort(viewPort - 1, panelNumber));
            WaitforViewports();
            if (GetElement(SelectorType.CssSelector, PlayButton).Enabled)
                ClickElement(Driver.FindElement(By.CssSelector(PlayButton)));
            Thread.Sleep(5000);
            bool Status = wait.Until(ExpectedConditions.ElementExists(By.CssSelector(PauseButton))).Enabled;
            CloseCineToolBar();
            return Status;
        }

        /// <summary>
        /// This Method is used to play CINE in particular view port
        /// </summary>
        /// <param name="viewPort"></param> 
        /// <param name="panelNumber"></param>  
        public bool PauseCINE(int viewPort = 1, int panelNumber = 1)
        {
            /*String PlayButton = div_studypanel + ":nth-of-type(" + panelNumber + ")"
                                + " div.viewerContainer:nth-of-type(" + viewPort + ") "
                                + div_CINE_PlayBtn;
            String PauseButton = div_studypanel + ":nth-of-type(" + panelNumber + ")"
                                + " div.viewerContainer:nth-of-type(" + viewPort + ") "
                                + div_CINE_PauseBtn;*/
            OpenCineToolBar(viewPort, panelNumber);
            String PlayButton = div_CINE_PlayBtn;
            String PauseButton = div_CINE_PauseBtn;
            ClickElement(GetElement(SelectorType.CssSelector, SetViewPort(viewPort - 1, panelNumber)));
            WaitforViewports();
            if (GetElement(SelectorType.CssSelector, PauseButton).Enabled)
                ClickElement(Driver.FindElement(By.CssSelector(PauseButton)));
            Thread.Sleep(5000);
            bool Status = wait.Until(ExpectedConditions.ElementExists(By.CssSelector(PlayButton))).Enabled;
            CloseCineToolBar();
            return Status;
        }

		/// <summary>
		/// This Method is used to play CINE in particular view port
		/// </summary>
		/// <param name="viewPort"></param> 
		/// <param name="panelNumber"></param>  
		public bool VerifyCINEPlayorPauseEnabled(string button, int viewPort = 1, int panelNumber = 1)
		{
            /*String PlayButton = div_studypanel + ":nth-of-type(" + panelNumber + ")"
								+ " div.viewerContainer:nth-of-type(" + viewPort + ") "
								+ div_CINE_PlayBtn;
			String PauseButton = div_studypanel + ":nth-of-type(" + panelNumber + ")"
								+ " div.viewerContainer:nth-of-type(" + viewPort + ") "
								+ div_CINE_PauseBtn;*/
            OpenCineToolBar(viewPort, panelNumber);
            String PlayButton = div_CINE_PlayBtn;
            String PauseButton = div_CINE_PauseBtn;
            if (button == "play")
			{
				return FindElementByCss(PlayButton).Enabled;
			}
			else
			{
				return FindElementByCss(PauseButton).Enabled;
			}
			

		}

		/// <summary>
		/// This Method is used to click on 'Play Previoues Image Series' cine button.
		/// </summary>
		/// <param name="viewportNumber">should start from 1</param>
		/// <param name="panelNumber">should start from 1</param>
		public void ClickPreviousSeriesCINE(int viewportNumber = 1, int panelNumber = 1)
        {
            /*var previousimageseries = Driver.FindElement(By.CssSelector(div_studypanel + ":nth-of-type(" + panelNumber + ")"
                + " div.viewerContainer:nth-of-type(" + viewportNumber + ") " + div_CINE_PreviousImageBtn));

            ClickElement(previousimageseries);*/
            OpenCineToolBar(viewportNumber, panelNumber);
            Click("cssselector", div_CINE_PreviousImageBtn);
            Thread.Sleep(5000);
            CloseCineToolBar();
            Logger.Instance.InfoLog("Cine Play Previoues Image Series button clicked successfully.");
        }

        /// <summary>
        /// This Method is used to click on 'Play Next Image Series' cine button.
        /// </summary>
        /// <param name="viewportNumber">should start from 1</param>
        /// <param name="panelNumber">should start from 1</param>
        public void ClickNextSeriesCINE(int viewportNumber = 1, int panelNumber = 1)
        {
            /*IWebElement StudyPanel = BasePage.Driver.FindElements(By.CssSelector(div_StudyPanel))[panelNumber - 1];
            IWebElement vport = StudyPanel.FindElements(By.CssSelector(div_compositeViewerComponent))[viewportNumber - 1].FindElement(By.CssSelector(div_cineToolboxComponent));
            IWebElement nextimageseries = vport.FindElement(By.CssSelector(div_CINE_NextImageBtn));
            ClickElement(nextimageseries);*/
            OpenCineToolBar(viewportNumber, panelNumber);
            Click("cssselector", div_CINE_NextImageBtn);
            Thread.Sleep(5000);
            CloseCineToolBar();
            Logger.Instance.InfoLog("Cine Play Next Image Series button clicked successfully.");
        }

        /// <summary>
        /// This Method is used to verify that CINE is playing or not
        /// </summary>
        /// <param name="viewportNumber"></param> Should start with 1
        /// <param name="panelNumber"></param> Should start with 1
        /// <returns></returns> if CineStatus is true then CINE is Playing, if CineStatus is false then CINE is not Playing
        public Boolean IsCINEPlaying(int viewportNumber = 1, int panelNumber = 1)
        {
            /*String FPSvalue = GetElement("cssselector", div_StudyPanel + ":nth-of-type(" + panelNumber + ") div.viewerContainer:nth-of-type("
                + viewportNumber + ") " + div_CINE_FPSControlButton).GetAttribute("innerHTML").Trim();
            Logger.Instance.InfoLog("The FPS value of the CINE is " + FPSvalue);*/
            OpenCineToolBar(viewportNumber, panelNumber);
            bool IsCinePlaying = false;
            String FPSvalue = GetElement("cssselector", div_CINE_FPSControlButton).GetAttribute("innerHTML").Trim();
            Logger.Instance.InfoLog("The FPS value of the CINE is " + FPSvalue);
            //if (!FPSvalue.Equals("0 FPS"))
            if (!FPSvalue.Equals("0"))
            {
                Logger.Instance.InfoLog("CINE is Playing");
                IsCinePlaying = true;
            }
            CloseCineToolBar();
            return IsCinePlaying;
        }

        /// <summary>
        /// This Method is used to get the FPS value
        /// </summary>
        /// <param name="viewportNumber"></param> Should start with 1
        /// <param name="panelNumber"></param> Should start with 1
        /// <returns></returns> 
        public string GetFPSValue(int viewportNumber = 1, int panelNumber = 1)
        {
            OpenCineToolBar(viewportNumber, panelNumber);
            string fps = null;
            /*String Locator = div_StudyPanel + ":nth-of-type(" + panelNumber + ")" 
                                + " div.viewerContainer:nth-of-type(" + viewportNumber + ")"
                                + " .FPSControlButton";*/

            //fps = GetElement(SelectorType.CssSelector, Locator).GetAttribute("innerText").Trim();
            fps = GetElement(SelectorType.CssSelector, div_CINE_FPSControlButton).GetAttribute("innerText").Trim()+ " FPS";
            CloseCineToolBar();
            return fps;
        }

        /// <summary>
        /// This Method is used to set the FPS of Cardio CINE
        /// </summary>
        /// <param name="value"></param> The value to set the FPS
        /// <param name="viewportNumber"></param> should starts from 1
        /// <param name="PanelNumber"></param> should starts from 1
        public int SetFPSValue(String value, int viewportNumber = 1, int PanelNumber = 1, bool testcomplete = false)
        {
            /*IWebElement FPSButton = null;
            if (!IsElementVisibleInUI(By.CssSelector(div_StudyPanel + ":nth-of-type(" + PanelNumber + ") div.viewerContainer:nth-of-type("
                + viewportNumber + ") " + div_CINE_FPSSlider)))
            {
                FPSButton = GetElement("cssselector", div_StudyPanel + ":nth-of-type(" + PanelNumber + ") " +
                    "div.viewerContainer:nth-of-type(" + viewportNumber + ") " + div_CINE_FPSControlButton);
                VerifyCardioCINEToolbarOnMouseHover(viewportNumber, PanelNumber);
                Thread.Sleep(10000);
                ClickElement(FPSButton);
                Thread.Sleep(10000);
            }
            PageLoadWait.WaitForElementToDisplay(GetElement("cssselector", div_StudyPanel + ":nth-of-type(" + PanelNumber +
                ") div.viewerContainer:nth-of-type(" + viewportNumber + ") " + div_CINE_FPSSliderHandler));
            IWebElement sliderPointer = GetElement("cssselector", div_StudyPanel + ":nth-of-type(" + PanelNumber +
                ") div.viewerContainer:nth-of-type(" + viewportNumber + ") " + div_CINE_FPSSliderHandler);
            IWebElement slider = GetElement("cssselector", div_StudyPanel + ":nth-of-type(" + PanelNumber +
                ") div.viewerContainer:nth-of-type(" + viewportNumber + ") " + div_CINE_FPSSlider);
            int slidervalue = Convert.ToInt32(slider.Text);
            Logger.Instance.InfoLog("The FPS value is " + slidervalue);
            for (int i=1; slidervalue > 1 && i<=90;i++)
            {
                Actions action = new Actions(Driver);
                action.ClickAndHold(sliderPointer).MoveByOffset(0, 1).Release().Build();
                action.Perform();
                Thread.Sleep(2000);
                slidervalue = Convert.ToInt32(slider.Text);
                Logger.Instance.InfoLog("Loop count for reducing Slider Value is = "+i);
            }
            slidervalue = Convert.ToInt32(slider.Text);
            for (int i=1; slidervalue < Convert.ToInt32(value) && i<=90;i++)
            {

                Actions action = new Actions(Driver);
                action.ClickAndHold(sliderPointer).MoveByOffset(0, -10).Release().Build();
                action.Perform();
                Thread.Sleep(2000);
                slidervalue = Convert.ToInt32(slider.Text);
                Logger.Instance.InfoLog("Loop count for Setting Slider value is = " + i);
            }
            Logger.Instance.InfoLog("The FPS is set as " + slidervalue);
            return slidervalue;*/
            OpenCineToolBar(viewportNumber, PanelNumber);
            /*TestCompleteAction action = new TestCompleteAction();
            action.Click(GetElement("cssselector", div_CINE_FPSControlButton)).Perform();*/
            if (testcomplete)
            {
                TestCompleteAction action = new TestCompleteAction();
                action.Click(GetElement("cssselector", div_CINE_FPSControlButton)).Perform();
            }
            else
            {
                if (SBrowserName.ToLower().Contains("firefox"))
                {
                    ClickElement(GetElement("cssselector", div_CINE_FPSControlButton));
                }
                else
                {
                    GetElement("cssselector", div_CINE_FPSControlButton).Click();
                }
            }
            IWebElement sliderPointer = GetElement("cssselector", div_CINE_FPSSliderHandler);
            IWebElement slider = GetElement("cssselector", div_CINE_FPSSlider);
            Thread.Sleep(1000);
            int slidervalue = 0;
            if (testcomplete)
            {
                TestCompleteAction action = new TestCompleteAction();
                action.SetFPS(sliderPointer, slider, value);
            }
            else
            {
                Logger.Instance.InfoLog("The FPS Text is " + slider.Text.Trim());
                slidervalue = Convert.ToInt32(slider.Text.Trim());
                Logger.Instance.InfoLog("The FPS value is " + slidervalue);
                if (slidervalue < Convert.ToInt32(value))
                {
                    for (int i = 1; slidervalue < Convert.ToInt32(value) && i <= 90; i++)
                    {

                        Actions action = new Actions(Driver);
                        action.ClickAndHold(sliderPointer).MoveByOffset(0, -2).Release().Build();
                        action.Perform();
                        Thread.Sleep(1000);
                        slidervalue = Convert.ToInt32(slider.Text.Trim());
                        Logger.Instance.InfoLog("Loop count for Setting Slider value is = " + i);
                    }
                }
                else if (slidervalue > Convert.ToInt32(value))
                {
                    for (int i = 1; slidervalue > Convert.ToInt32(value) && i <= 90; i++)
                    {
                        Actions action = new Actions(Driver);
                        action.ClickAndHold(sliderPointer).MoveByOffset(0, 2).Release().Build();
                        action.Perform();
                        Thread.Sleep(1000);
                        slidervalue = Convert.ToInt32(slider.Text.Trim());
                        Logger.Instance.InfoLog("Loop count for reducing Slider Value is = " + i);
                    }
                }
            }
            slidervalue = Convert.ToInt32(slider.Text.Trim());            
            Logger.Instance.InfoLog("The FPS is set as " + slidervalue);
            return slidervalue;
        }

        /// <summary>
        /// This Method is used to click on 'Play Previoues Image Series' cine button.
        /// </summary>
        /// <param name="viewportNumber">should start from 1</param>
        /// <param name="panelNumber">should start from 1</param>
        /// <param name= "action">action should be either "PlayAll" or "PauseAll">
        public void ClickPlayAllOrPauseAll(string action, int viewportNumber = 1, int panelNumber = 1)
        {
            //IWebElement playorpauseall;
            /*String playorpauseall = div_studypanel + ":nth-of-type(" + panelNumber + ") "; // + " div.viewerContainer:nth-of-type(" + viewportNumber + ") ";
            String waitButton = div_studypanel + ":nth-of-type(" + panelNumber + ") "; // + " div.viewerContainer:nth-of-type(" + viewportNumber + ") ";*/
            ClosePanelMoreButtonToolBar(panelNumber);
            string viewerMenuButton = div_StudyPanel + ":nth-of-type(" + panelNumber + ") " + div_MoreButton + " " + div_viewerMenuButton;
            String playorpauseall = div_studypanel + ":nth-of-type(" + panelNumber + ") ";
            bool ClickviewerMenuButton = false;
            //String waitButton = string.Empty;
            //ClickOnViewPort(panelNumber, viewportNumber);
            if (action == "PlayAll")
            {
                playorpauseall = playorpauseall + div_CINE_PlayAllBtn;
                if(Driver.FindElements(By.CssSelector(playorpauseall)).Count == 0)
                {
                    playorpauseall = div_CINE_PlayAllBtn;
                    ClickviewerMenuButton = true;
                }
				//waitButton = waitButton + div_CINE_PauseAllBtn;

            }
            else
            {
                playorpauseall = playorpauseall + div_CINE_PauseAllBtn;
                if (Driver.FindElements(By.CssSelector(playorpauseall)).Count == 0)
                {
                    playorpauseall = div_CINE_PauseAllBtn;
                    ClickviewerMenuButton = true;
                }
                //waitButton = waitButton + div_CINE_PlayAllBtn;
            }

            var js = (IJavaScriptExecutor)Driver;
            if(ClickviewerMenuButton)
            {
                OpenPanelMoreButtonToolBar(panelNumber);
            }
            /*if(Driver.FindElements(By.CssSelector(viewerMenuButton)).Count > 0)
            {
                if (Driver.FindElement(By.CssSelector(viewerMenuButton)).Displayed)
                {
                    OpenPanelMoreButtonToolBar(panelNumber);
                }
            }*/
            js.ExecuteScript("arguments[0].click()", Driver.FindElement(By.CssSelector(playorpauseall)));
            Logger.Instance.InfoLog("Cine " + action + " button clicked successfully.");
            //wait.Until(ExpectedConditions.ElementExists(By.CssSelector(waitButton)));
        }

        #endregion CardioCINE

        /// <summary>
        /// Method created to verify Cardiocine playbar displayed when we do mouse hover on viewport bottom. 
        /// </summary>
        /// <param name="viewport">Provide viewport number</param>
        /// <param name="studypanel">Provide studypanel number</param>
        public bool VerifyCardioCINEToolbarOnMouseHover(int viewport = 1, int panel = 1, int waittime=30)
        {
            string locator = div_StudyPanel + ":nth-of-type(" + panel + ") " + div_compositeViewerComponent + ":nth-of-type(" + viewport + ") " + div_cineToolboxComponent;
            if (GetElement(SelectorType.CssSelector, locator).Displayed)
            {
                return true;
            }
            else
            {
                string cls = Driver.FindElement(By.CssSelector(locator)).GetAttribute("class");
                string sty = Driver.FindElement(By.CssSelector(locator)).GetAttribute("style");
                IJavaScriptExecutor executor = (IJavaScriptExecutor)Driver;
                string script1 = "document.querySelector('" + locator + "').setAttribute('class', '" + cls + " cineToolboxComponentVisible')";
                string script2 = "document.querySelector('" + locator + "').setAttribute('style', '" + sty.Replace("opacity: 0", "opacity: 1") + "')";
                PageLoadWait.WaitForFrameLoad(20);
                executor.ExecuteScript(script1);
                executor.ExecuteScript(script2);
                PageLoadWait.WaitForElementToDisplay(GetElement(SelectorType.CssSelector, locator), waittime);
                bool hover = GetElement(SelectorType.CssSelector, locator).Displayed;
                return hover;
            }

        }

        /// <summary>
        /// OpenCineToolBar
        /// </summary>
        public void OpenCineToolBar(int viewport = 1, int panel = 1)
        {
            CloseCineToolBar();
            string viewerMenuButton = div_StudyPanel + ":nth-of-type(" + panel + ") " + div_compositeViewerComponent + ":nth-of-type(" + viewport + ") " + div_cineToolboxComponent+ " "+ div_viewerMenuButton;
            if (SBrowserName.ToLower().Equals("internet explorer"))
            {
                ClickElement(GetElement("cssselector", viewerMenuButton));
            }
            else
            {
                Click("cssselector", viewerMenuButton);
            }
            Thread.Sleep(3000);
        }

        /// <summary>
        /// CloseCineToolBar
        /// </summary>
        public void CloseCineToolBar()
        {
            if (SBrowserName.ToLower().Equals("internet explorer"))
            {
                if (Driver.FindElements(By.CssSelector(div_cineToolboxComponent + " " + div_viewerMenuCloseButton)).Count != 0)
                {
                    ClickElement(GetElement("cssselector", div_cineToolboxComponent + " " + div_viewerMenuCloseButton));
                }
            }
            else
            {
                if (Driver.FindElements(By.CssSelector(div_viewerMenuCloseButton)).Count != 0)
                {
                    Click("cssselector", div_viewerMenuCloseButton);
                }
            }
            Thread.Sleep(5000);
        }

        /// <summary>
        /// OpenPanelMoreButtonToolBar
        /// </summary>
        public void OpenPanelMoreButtonToolBar(int panel = 1)
        {
            ClosePanelMoreButtonToolBar(panel);
            string viewerMenuButton = div_StudyPanel + ":nth-of-type(" + panel + ") " + div_MoreButton+ " "+ div_viewerMenuButton;
            if (SBrowserName.ToLower().Equals("internet explorer"))
            {
                ClickElement(GetElement("cssselector", viewerMenuButton));
            }
            else
            {
                Click("cssselector", viewerMenuButton);
            }
            Thread.Sleep(3000);
        }

        /// <summary>
        /// ClosePanelViewerMenuButton
        /// </summary>
        public void ClosePanelMoreButtonToolBar(int panel = 1)
        {
            string viewerMenuCloseButton = div_StudyPanel + ":nth-of-type(" + panel + ") " + div_MoreButton + " " + div_viewerMenuCloseButton;
            if (SBrowserName.ToLower().Equals("internet explorer"))
            {
                if (Driver.FindElements(By.CssSelector(viewerMenuCloseButton)).Count != 0)
                {
                    ClickElement(GetElement("cssselector", viewerMenuCloseButton));
                }
            }
            else
            {
                if (Driver.FindElements(By.CssSelector(viewerMenuCloseButton)).Count != 0)
                {
                    Click("cssselector", viewerMenuCloseButton);
                }
            }
            Thread.Sleep(5000);
        }

        /// <summary>
        /// Sets series in the selected view port 
        /// </summary>
        /// <param name="viewportNumber">should start from 0</param>
        /// <param name="panelNumber">should start from 1</param>
        public void SetSeriesInViewport(int viewportNumber = 0, int panelNumber = 1)
        {            
            GetElement(SelectorType.CssSelector, SetViewPort(viewportNumber, panelNumber)).Click();            
            WaitforViewports();
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("edge"))
            {
                new TestCompleteAction().DoubleClick(Driver.FindElements(By.CssSelector(div_thumbnails))[viewportNumber]).Perform();
            }
            else
            {
                new Actions(Driver).DoubleClick(Driver.FindElements(By.CssSelector(div_thumbnails))[viewportNumber]).Build().Perform();
            }
            
            WaitforViewports();
            Thread.Sleep(5000);
        }

        /// <summary>
        /// This method returns set of FPS value for particular time range. 
        /// </summary>
        /// <param name="viewportNumber">should start from 0</param>
        /// <param name="panelNumber">should start from 1</param>
        public IList<int> GetFPSValueInList(int viewport = 1, int panel = 1, int waittime = 60)
        {
            Stopwatch stopwatch = new Stopwatch();
            TimeSpan timeout = new TimeSpan(0, 0, waittime);
            IList<int> FPSValue = new List<int>();
            stopwatch.Start();
            while (!(stopwatch.Elapsed >= new TimeSpan(0, 0, 30)))
            {
                //Wait for 30 seconds, for view port to start study
            }
            stopwatch.Stop();
            stopwatch.Reset();
            stopwatch.Start();
            /*while (!(stopwatch.Elapsed >= timeout))
            {
                FPSValue.Add(Convert.ToInt32(GetFPSValue(viewport, panel).Split(' ')[0]));
            }*/
            OpenCineToolBar(viewport, panel);
            while (!(stopwatch.Elapsed >= timeout))
            {
                FPSValue.Add(Convert.ToInt32(GetElement(SelectorType.CssSelector, div_CINE_FPSControlButton).GetAttribute("innerText").Trim()));
                Thread.Sleep(2000);
            }
            CloseCineToolBar();
            stopwatch.Stop();
            stopwatch.Reset();
            return FPSValue;
        }

        /// <summary>
        /// Method for changing viewer Layout
        /// </summary>
        /// <param name="Layout">Provide the layout as per the availability in Grid UI</param>
        /// <param name="panel">Provide panel number. Starts with 1</param>
        /// <param name="viewport">Provide viewport number only for sync up. Provide last viewport in grid that has image present for sync up for it to load. For eg. Out of 36 viewports in panel 1 only 15 have images, then put 15 as number for syncup to check if image has loaded or not. Viewport starts with 1</param>
        public bool ChangeViewerLayout(string Layout="1x1", int panel = 1, int viewport = 1)
        {
            PageLoadWait.WaitForFrameLoad(10);
            //IWebElement LayoutIcon = PageLoadWait.WaitForElement(By.CssSelector(div_Panel(panel) + div_LayoutIcon), WaitTypes.Visible);
            //PageLoadWait.WaitForElement(By.CssSelector(div_Panel(panel) + div_LayoutIcon), WaitTypes.Clickable);
            //LayoutIcon.Click();
            IWebElement LayoutIcon = Driver.FindElement(By.CssSelector(div_Panel(panel) + div_LayoutIcon));
            if (SBrowserName.ToLower().Equals("internet explorer"))
            {
				//LayoutIcon.Click();
				//this.ClickElement(LayoutIcon);
				new TestCompleteAction().MoveToElement(LayoutIcon).Click().Perform();

			}
            else if((SBrowserName.ToLower().Contains("edge")) || (SBrowserName.ToLower().Contains("chrome")))
            {
                ClickElement(LayoutIcon);
            }
            else
            {
				this.ClickElement(LayoutIcon);
				//new TestCompleteAction().MoveToElement(LayoutIcon).Click().Perform();
			}
            IWebElement Wrapper = null;
            if (SBrowserName.ToLower().Contains("edge"))
            {
                Wrapper = Driver.FindElement(By.CssSelector(div_Panel(panel) + div_LayoutGridWrapper));
            }
            else
            {
                Wrapper = PageLoadWait.WaitForElement(By.CssSelector(div_Panel(panel) + div_LayoutGridWrapper), WaitTypes.Visible);
            }
            IList<IWebElement> TDList = Wrapper.FindElements(By.TagName("td"));
            bool flag = false;
            foreach (IWebElement TD in TDList)
            {
                if(TD.GetAttribute("id").Equals(Layout))
                {
                    if (SBrowserName.ToLower().Contains("edge"))
                    {
                        ClickElement(TD);
                    }
                    else
                    {
                        // TD.Click();
                        wait.Until(ExpectedConditions.ElementToBeClickable(TD));
                        var js = (IJavaScriptExecutor)Driver; js.ExecuteScript("arguments[0].click()", TD);
                    }
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                Logger.Instance.ErrorLog("Invalid Layout type provided as argument. Please check the argument");
                //Dismiss Dialog Popup
                LayoutIcon.Click();
            }
            else
            {
                //Sync up
                PageLoadWait.WaitForElement(By.CssSelector(div_Panel(panel) + div_LayoutGridWrapper), WaitTypes.Invisible);
                int number1 = Convert.ToInt32(Layout.Split('x')[0]);
                int number2 = Convert.ToInt32(Layout.Split('x')[1]);
                wait.Until(d =>
                {
                    if (Driver.FindElements(By.CssSelector(div_Panel(panel) + div_ViewportPanels)).Count == (number1 * number2))
                    {
                        Logger.Instance.InfoLog("Layout load completed");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Layout load waiting");
                        return false;
                    }
                });
                PageLoadWait.WaitForBluRingViewportToLoad(panel: panel, viewport: viewport);
                //Sync up for Layout icon to be re-enabled
                wait.Until(d =>
                {
                    if (!d.FindElement(By.CssSelector(div_Panel(panel) + div_ToolbarLayoutWrapper)).GetAttribute("class").Contains(div_ToolbarLayoutDisabled))
                    {
                        Logger.Instance.InfoLog("Layout icon enabled");
                        return true;
                    }
                    else
                    {
                        Logger.Instance.InfoLog("Layout icon disabled");
                        return false;
                    }
                });
            }
            return flag;
        }

        /// <summary>
        /// Method for changing viewer Layout
        /// </summary>
        public bool verfiyLayoutHighlighted( int row, int column )
        {
            bool status = true;
            IWebElement gridLayout = BasePage.FindElementByCss(BluRingViewer.div_LayoutGridWrapper);
            
            IList<IWebElement> TDList = gridLayout.FindElements(By.TagName("td"));
            foreach (IWebElement Grid in TDList)
            {
                //  TD.GetAttribute("id").Equals(Layout))
                int CurrentGridRowValue = Convert.ToInt32(Grid.GetAttribute("id").Split('x')[0]);
                int CurrentGridcolumnValue = Convert.ToInt32(Grid.GetAttribute("id").Split('x')[1]);
                if (CurrentGridRowValue <= row && CurrentGridcolumnValue <= column)
                {
                    if (!(Grid.GetCssValue("border-bottom-color") == "rgba(90, 170, 255, 1)") && Grid.GetCssValue("border-bottom-color") == Grid.GetCssValue("border-right-color") && Grid.GetCssValue("border-bottom-color") == Grid.GetCssValue("border-top-color"))
                    {
                        status = false;
                        break;
                    }
                }
                else
                {
                    if (!(Grid.GetCssValue("border-bottom-color") == "rgba(255, 255, 255, 1)") && Grid.GetCssValue("border-bottom-color") == Grid.GetCssValue("border-right-color") && Grid.GetCssValue("border-bottom-color") == Grid.GetCssValue("border-top-color"))
                    {
                        status = false;
                        break;
                    }
                }
            }


            return status;

        }

        public bool HouseOverToLayout(int row, int column)
        {
            bool status = false;
            IWebElement gridLayout = BasePage.FindElementByCss(BluRingViewer.div_LayoutGridWrapper);
            IList<IWebElement> TDList = gridLayout.FindElements(By.TagName("td"));
            foreach (IWebElement Grid in TDList)
            {
                if (Grid.GetAttribute("id").Equals(row + "x" + column))
                {
                    Actions action = new Actions(BasePage.Driver);
                    action.MoveToElement(Grid).Perform();
                    status = true;
                    break;
                }
            }
            if(status== false)
            {
                Logger.Instance.ErrorLog("Unable to find the Specified layout" + row +"x"+column);
            }

            return status;

          

        }



        /// <summary>
        /// This method is for validating the background color of the particular element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public bool verifyBackgroundColor(IWebElement element, String color)
        {
            if (!((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.Contains("firefox"))
            {
                if (element.GetCssValue("background-color").Equals(color))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                string[] array = color.Split('(');
                string[] rgb = array[1].Split(',');
                string colorCode = "rgb(" + rgb[0] + "," + rgb[1] + "," + rgb[2] + ")";
                if (element.GetCssValue("background-color").Equals(colorCode))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// This method checks if the exam list is sorted by study date
        /// </summary>
        /// <param name="sortedAscending">default false</param>
        /// <returns></returns>
        public bool IsStudiesSortedByDate(bool sortedAscending = false)
        {
            bool isSorted = false;
            IList<DateTime> priorsdate = new List<DateTime>();
            var priors = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));

            foreach (IWebElement prior in priors)
            {
                String date = prior.FindElement(By.CssSelector(BluRingViewer.div_priorDate)).GetAttribute("innerHTML") +
                    " " + prior.FindElement(By.CssSelector(BluRingViewer.div_priorTime)).GetAttribute("innerHTML");
                priorsdate.Add(DateTime.ParseExact(date, "dd-MMM-yyyy h:mm:ss tt", CultureInfo.InvariantCulture));
            }

            if (sortedAscending)
            {
                var sortedList = priorsdate.OrderBy(date => date.Date).
                    OrderBy(date => date.Month).OrderBy(date => date.Year).ToList<DateTime>();
                isSorted = sortedList.SequenceEqual(priorsdate);
            }
            else
            {
                var sortedList = priorsdate.OrderByDescending(date => date.Date).
                    OrderByDescending(date => date.Month).OrderByDescending(date => date.Year).ToList<DateTime>();
                isSorted = sortedList.SequenceEqual(priorsdate);
            }

            return isSorted;
        }

        /// <summary>
        /// Apply Zoom
        /// </summary>
        public void ApplyTool_Zoom(int dragStartX = 0, int dragStartY = 0, int dropX = 0, int dropY = 0)
        {
            if (dragStartX == 0 && dragStartY == 0 && dropX == 0 && dropY == 0)
            {
                var attributes = GetElementAttributes(this.Activeviewport);
                dragStartX = attributes["width"] / 2;
                dragStartY = attributes["height"] / 2;
                dropX = attributes["width"] / 4;
                dropY = attributes["height"] / 4;
            }

            this.PerformTool(dragStartX, dragStartY, dropX, dropY, isInteractivetool: true);
            Thread.Sleep(Config.ms_minTimeout);
        }

        /// <summary>
        ///  This methos will get the entire merge port report content and table in a Dictionary
        /// </summary>
        /// <param name="priorcount">Prior count, should start from zero</param>
        /// <param name="reportType">Report Type</param>
        /// <returns></returns>
        public Dictionary<String, String> FetchMergePortReportData_BR(int priorcount, String reportType = "PDF")
        {
            String reportdata = null;
            Dictionary<String, String> report = new Dictionary<String, String>();
            String script = System.IO.File.ReadAllText("Scripts\\JSFiles\\FetchMergePortReportData.js");
            string parameters;
            if (string.Equals(reportType, "MergeportReport"))
            {
                parameters = "\"" + BluRingViewer.SRReport_iframe + "\"" + "," +
                    "\"" + BluRingViewer.MergeportReport_iframe + "\"" + "," + priorcount;
            }
            else
            {
                parameters = "\"" + BluRingViewer.Pdf_iframe + "\"" + "," +
                    "\"" + BluRingViewer.pdfreport_continer + "\"" + ", " + priorcount;
            }

            script = script + "return getMergePortHL7ReportData(" + parameters + ");";
            BasePage.Driver.SwitchTo().DefaultContent();
            reportdata = (String)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);

            //report.Add("Table", (IWebElement)reportdata["Table"]);
            report.Add("ReportData", reportdata);
            return report;
        }

        /// <summary>
        /// This method returns the patient details in the BluRing viewer
        /// Dictionary Keys : "LastName", "FirstName", "PatientID"
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, String> PatientDetailsInViewer()
        {
            PageLoadWait.WaitForPageLoad(10);
            Dictionary<String, String> details = new Dictionary<string, string>();
            String patientID = GetText("cssselector", BluRingViewer.div_PatientID);
            String patientName = GetText("cssselector", BluRingViewer.p_PatientName).Replace(" ", String.Empty);

            details.Add("LastName", patientName.Split(',')[0]);
            details.Add("FirstName", patientName.Split(',')[1]);
            details.Add("PatientID", patientID);
            return details;
        }

        /// <summary>
        /// This method used to wait till reaching thumbnail percentage as 100% in BluRing Viewer.
        /// </summary>
        public bool WaitForThumbnailPercentageTo100(int numofthumbnails)
        {
            bool is100percent = false;
            IList<IWebElement> PercentViewedList = BasePage.Driver.FindElements(By.CssSelector(BluRingViewer.div_studyPanelThumbnailImageComponent + " " + BluRingViewer.div_thumbnailPercentImagesViewed));
            for (int i = 0; i < numofthumbnails; i++)
            {
                String thumbnailpercentage = PercentViewedList[i].GetAttribute("innerHTML");
                for (int j = 0; !thumbnailpercentage.Equals("100%") && j < 500; j++)
                {
                    thumbnailpercentage = PercentViewedList[i].GetAttribute("innerHTML");
                    if (!thumbnailpercentage.Equals("100%"))
                    {
                        Thread.Sleep(2000);
                    }
                    else
                    {
                        is100percent = true;
                        Logger.Instance.InfoLog("Thumbnail" + i + " reaches 100% ");
                        break;
                    }
                }
            }
            return is100percent;
        }

        /// <summary>
        /// Opens Help window and switches driver to it
        /// </summary>
        /// <returns>Parent window and Print preview window handle</returns>
        public string[] OpenHelpandSwitchtoIT(int viewer = 1)
        {
            string[] result = new string[2];
            try
            {
                int timeout = 0;
                string ParentWindowID = Driver.CurrentWindowHandle;
                //Adding Parent window handle
                result[0] = ParentWindowID;
                while (Driver.WindowHandles.Count == 1 && timeout < 5)
                {
                    if (viewer == 1)
                    {
                        PageLoadWait.WaitForElement(By.CssSelector(div_HelpIcon), WaitTypes.Visible).Click();
                        PageLoadWait.WaitForElement(By.CssSelector(li_AboutIcon), WaitTypes.Visible).Click();
                    }
                    else
                    {
                        BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
                        HelpIcon().Click();
                        wait.Until(ExpectedConditions.ElementExists(By_HelpContentsIcon));
                        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click()", HelpContentsIcon());
                    }
                    PageLoadWait.WaitForPageLoad(3);
                    timeout = timeout + 1;
                    Driver.SwitchTo().DefaultContent();
                    Driver.SwitchTo().Frame(0);
                }
                if (Driver.WindowHandles.Count > 1)
                {
                    string previewWindowId = Driver.WindowHandles[0].Equals(ParentWindowID, StringComparison.InvariantCultureIgnoreCase) ? Driver.WindowHandles[1] : Driver.WindowHandles[0];
                    Driver.SwitchTo().Window(previewWindowId);
                    result[1] = previewWindowId;
                }
                else
                {
                    Logger.Instance.ErrorLog("Could not open New window for Help ");
                }

            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error encountered in OpenHelpandSwitchtoIT due to :" + ex.Message);
                return null;
            }
            return result;
        }

        /// <summary>
        /// This method returns the study details in the viewer
        /// Dictionary Keys : "Patient Name", "Patinet Name"
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, String> StudyPatientDetailsInUniversalViewer()
        {
            PageLoadWait.WaitForPageLoad(10);
            Dictionary<String, String> details = new Dictionary<string, string>();
            details.Add("Patinet Name", BasePage.FindElementByCss(p_PatientName).Text);
            details.Add("PatientID", BasePage.FindElementByCss(div_PatientID).Text);
            return details;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="UIElements"></param>
        /// <returns></returns>
        public Boolean ValidateLocalizationStudyTitle(String[] dateTime, String[] UIElements)
        {
            String jsonFile = File.ReadAllText(Localization.ToolbarConfigSettingsJsonFile);
            var jObj = Newtonsoft.Json.Linq.JObject.Parse(jsonFile);

            String emailStudyTitle = jObj["StudyPanelControlComponent"]["ToolbarConfiguration"]["emailStudyTool"]["Tooltip"].ToString();
            String layoutTitle = jObj["StudyPanelControlComponent"]["ToolbarConfiguration"]["changeRowColumnLayoutTool"]["Tooltip"].ToString();

            var emailStudyText = Driver.FindElement(By.CssSelector(UIElements[0])).GetAttribute("title");
            var layoutText = Driver.FindElement(By.CssSelector(UIElements[1])).GetAttribute("title");
            var dateText = Driver.FindElement(By.CssSelector(UIElements[2])).Text;
            var timeText = Driver.FindElement(By.CssSelector(UIElements[3])).Text;

            if (emailStudyTitle.Equals(emailStudyText) && layoutTitle.Equals(layoutText) && dateText.Equals(dateTime[0]) && timeText.Equals(dateTime[1]))
            {
                return true;
            }
            else
            {
                string errMessage = "Study Panel Title - Expected result: ";
                if (!emailStudyTitle.Equals(emailStudyText)) Logger.Instance.ErrorLog(errMessage + emailStudyTitle + ", Actual result: " + emailStudyText);
                if (!layoutTitle.Equals(layoutText)) Logger.Instance.ErrorLog(errMessage + layoutTitle + ", Actual result: " + layoutText);
                if (!dateText.Equals(dateTime[0])) Logger.Instance.ErrorLog(errMessage + dateTime[0] + ", Actual result: " + dateText);
                if (!timeText.Equals(dateTime[1])) Logger.Instance.ErrorLog(errMessage + dateTime[1] + ", Actual result: " + timeText);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Boolean ValidateLocalizationStudyToolbox()
        {
            OpenViewerToolsPOPUp();
            IList<String> toolsInToolbox = GetToolsInToolBoxByGrid();
            IList<bool> valueMatched = new List<bool>();

            string[] col1 = toolsInToolbox[0].Split(',');
            string windowLevel = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.WindowLevel.Tooltip");
            valueMatched.Add(col1[0].Equals(windowLevel));
            string autoWL = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.AutoWindowLevel.Tooltip");
            valueMatched.Add(col1[1].Equals(autoWL));
            string invert = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.Invert.Tooltip");
            valueMatched.Add(col1[2].Equals(invert));
            string[] col2 = toolsInToolbox[1].Split(',');
            string zoom = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.Zoom.Tooltip");
            valueMatched.Add(col2[0].Equals(zoom));
            string magnifierX2 = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.Magnifier2.Tooltip");
            valueMatched.Add(col2[1].Equals(magnifierX2));
            string pan = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.Pan.Tooltip");
            valueMatched.Add(toolsInToolbox[2].Equals(pan));
            string[] col4 = toolsInToolbox[3].Split(',');
            string lineMeasurement = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.LineMeasurement.Tooltip");
            valueMatched.Add(col4[0].Equals(lineMeasurement));
            string calibration = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.Calibration.Tooltip");
            valueMatched.Add(col4[1].Equals(calibration));
            string scroll = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.ScrollTool.Tooltip");
            valueMatched.Add(toolsInToolbox[4].Equals(scroll));
            string[] col6 = toolsInToolbox[5].Split(',');
            string angleMeasurement = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.AngleMeasurement.Tooltip");
            valueMatched.Add(col6[0].Equals(angleMeasurement));
            string cobbAngle = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.CobbAngle.Tooltip");
            valueMatched.Add(col6[1].Equals(cobbAngle));
            string[] col7 = toolsInToolbox[6].Split(',');
            string ellipse = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.Ellipse.Tooltip");
            valueMatched.Add(col7[0].Equals(ellipse));
            string rectangle = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.Rectangle.Tooltip");
            valueMatched.Add(col7[1].Equals(rectangle));
            string drawROI = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.DrawROI.Tooltip");
            valueMatched.Add(col7[2].Equals(drawROI));
            string[] col8 = toolsInToolbox[7].Split(',');
            string rotateClockwise = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.RotateClockwise.Tooltip");
            valueMatched.Add(col8[0].Equals(rotateClockwise));
            string rotateCounterclockwise = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.RotateCounterclockwise.Tooltip");
            valueMatched.Add(col8[1].Equals(rotateCounterclockwise));
            string[] col9 = toolsInToolbox[8].Split(',');
            string flipHorizontal = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.FlipHorizontal.Tooltip");
            valueMatched.Add(col9[0].Equals(flipHorizontal));
            string flipVertical = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.FlipVertical.Tooltip");
            valueMatched.Add(col9[1].Equals(flipVertical));
            string saveSeries = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.SaveSeries.Tooltip");
            valueMatched.Add(col9[2].Equals(saveSeries));
            string seriesScope = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.SeriesScope.Tooltip");
            valueMatched.Add(col9[3].Equals(seriesScope));
            string saveAnnotatedImage = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.SaveAnnotatedImage.Tooltip");
            valueMatched.Add(col9[4].Equals(saveAnnotatedImage));
            string[] col10 = toolsInToolbox[9].Split(',');
            string pixelValue = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.PixelValue.Tooltip");
            valueMatched.Add(col10[0].Equals(pixelValue));
            string imageScope = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.ImageScope.Tooltip");
            valueMatched.Add(col10[1].Equals(imageScope));
            string[] col11 = toolsInToolbox[10].Split(',');
            string addText = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.AddText.Tooltip");
            valueMatched.Add(col11[0].Equals(addText));
            string freeDraw = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.FreeDraw.Tooltip");
            valueMatched.Add(col11[1].Equals(freeDraw));
            string removeAllAnnotation = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.RemoveAllAnnotation.Tooltip");
            valueMatched.Add(col11[2].Equals(removeAllAnnotation));
            string reset = ReadDataFromJsonFile(Localization.LocaleViewportToolbarJsonFile, "ViewportTools.Reset.Tooltip");
            valueMatched.Add(toolsInToolbox[11].Equals(reset));

            if (!valueMatched.Contains(false))
            {
                return true;
            }
            else
            {
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col1[0] + ", Actual result: " + windowLevel);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col1[1] + ", Actual result: " + autoWL);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col1[2] + ", Actual result: " + invert);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col2[0] + ", Actual result: " + zoom);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col2[1] + ", Actual result: " + magnifierX2);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + toolsInToolbox[2] + ", Actual result: " + pan);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col4[0] + ", Actual result: " + lineMeasurement);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col4[1] + ", Actual result: " + calibration);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + toolsInToolbox[4] + ", Actual result: " + scroll);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col6[0] + ", Actual result: " + angleMeasurement);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col6[1] + ", Actual result: " + cobbAngle);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col7[0] + ", Actual result: " + ellipse);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col7[1] + ", Actual result: " + rectangle);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col7[2] + ", Actual result: " + drawROI);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col8[0] + ", Actual result: " + rotateClockwise);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col8[1] + ", Actual result: " + rotateCounterclockwise);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col9[0] + ", Actual result: " + flipHorizontal);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col9[1] + ", Actual result: " + flipVertical);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col9[2] + ", Actual result: " + seriesScope);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col9[3] + ", Actual result: " + saveSeries);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col9[4] + ", Actual result: " + saveAnnotatedImage);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col10[0] + ", Actual result: " + pixelValue);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col10[1] + ", Actual result: " + imageScope);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col11[0] + ", Actual result: " + addText);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col11[1] + ", Actual result: " + freeDraw);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + col11[2] + ", Actual result: " + removeAllAnnotation);
                Logger.Instance.ErrorLog("Study Toolbox - Expected result: " + toolsInToolbox[11] + ", Actual result: " + reset);
                return false;
            }
        }

        /// <summary>
        /// This methis will check if foreign exam present in Exam Card
        /// Also check the too tip.
        /// </summary>
        /// <param name="accession"></param>
        /// <returns></returns>
        public Boolean IsForeignExamAlert(String accession)
        {
            var isAlertPresent = false;
            var isToolTipPresent = false;
            IWebElement hpPrior = null;
            var priors = BasePage.Driver.FindElements(By.CssSelector(div_priors));           

            if (accession != null)
            {
                foreach (IWebElement prior in priors)
                {
                    var accession_ele = prior.FindElement(By.CssSelector(AccessionNumberInExamList));
                    if (this.GetAccession(accession_ele).Equals(accession))
                    {
                        hpPrior = prior;
                        break;
                    }

                }
            }

            isAlertPresent = !hpPrior.FindElement(By.CssSelector("div.warningStatusIcon")).GetAttribute("class").Contains("invisible");
            isToolTipPresent = hpPrior.FindElement(By.CssSelector("div.warningStatusIcon")).GetAttribute("title").Equals
                ("Foreign Exam. This study may not belong to the same patient.");
            Logger.Instance.InfoLog("Value of IsAlertPresent is" + isAlertPresent);
            Logger.Instance.InfoLog("Value of isToolTipPresent is" + isToolTipPresent);

            return (isAlertPresent && isToolTipPresent);
        }

        /// <summary>
        /// This method will scroll through the Exam List and check if all priors are Displayed
        /// </summary>
        /// <returns></returns>
        public Boolean IsAllPriorsDisplayed()
        {
            int priorCount = 0;
            bool isPriorDisplayed = false;
            int totalPriorCount = BasePage.FindElementsByCss(BluRingViewer.div_priors).Count;

            while (priorCount <= totalPriorCount-1)
            {
                var priors = BasePage.FindElementsByCss(BluRingViewer.div_priors);
                var container = BasePage.FindElementByCss(BluRingViewer.div_ContainerPriors);               
                if (this.IsInBrowserViewport(priors[priorCount]) == false)
                {
                    new TestCompleteAction().MouseScroll(container, "down", "3").Perform();
                    new TestCompleteAction().MouseScroll(container, "down", "1").Perform();
                    if (this.IsInBrowserViewport(priors[priorCount]) == true)
                    {
                        isPriorDisplayed = true;
                        priorCount++;
                        continue;
                    }
                    else
                    {
                        isPriorDisplayed = false;
                        Logger.Instance.ErrorLog((priorCount + 1) + "th Prior Not Displayed");
                        break;
                    }
                }
                else
                {
                    isPriorDisplayed = true;
                    priorCount++;
                    continue;
                }
            }

            return isPriorDisplayed;
        }

        /// <summary>
        /// This method will return if PlayAll/PauseAll button is displayed on viewport
        /// </summary>
        /// <returns></returns>
        public bool VerifyPlayAllOrPauseAll(string action, int viewportNumber = 1, int panelNumber = 1)
        {
            //IWebElement playorpauseall;
            VerifyCardioCINEToolbarOnMouseHover(viewportNumber, panelNumber);
            String playorpauseall = div_studypanel + ":nth-of-type(" + panelNumber + ")" + " div.viewerContainer:nth-of-type(" + viewportNumber + ") ";
            String waitButton = div_studypanel + ":nth-of-type(" + panelNumber + ")" + " div.viewerContainer:nth-of-type(" + viewportNumber + ") ";
            if (action == "PlayAll")
            {
                playorpauseall = playorpauseall + div_CINE_PlayAllBtn;
                waitButton = waitButton + div_CINE_PauseAllBtn;
            }
            else
            {
                playorpauseall = playorpauseall + div_CINE_PauseAllBtn;
                waitButton = waitButton + div_CINE_PlayAllBtn;
            }
            if (Driver.FindElement(By.CssSelector(playorpauseall)).Displayed)
            {
                Logger.Instance.InfoLog(action + " is displayed successfully");
                return true;
            }
            {
                Logger.Instance.InfoLog(action + " failed to display");
                return false;
            }
        }

        /// <summary>
        ///  This methos will get the entire cardio report data in a Dictionary
        /// </summary>
        /// <param name="priorcount">Prior count, should start from zero</param>
        /// <param name="reportType">Report Type</param>
        /// <returns></returns>
        public Dictionary<String, String> FetchCardioReportData_BR(int priorcount, String reportType = "PDF")
        {
            Dictionary<String, String> report = new Dictionary<String, String>();
            String script = System.IO.File.ReadAllText("Scripts\\JSFiles\\FetchCardioReportData.js");

            script = script + "return getCardioReportData()";
            BasePage.Driver.SwitchTo().DefaultContent();
            dynamic reportdata = ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);

            if (reportdata != null)
            {
                foreach (var item in reportdata)
                {
                    report.Add(item.Key, item.Value);
                }
            }
            return report;
        }

        /// <summary>
        ///  This methos will get the entire UnityPACS report data in a Dictionary
        /// </summary>
        /// <param name="priorcount">Prior count, should start from zero</param>
        /// <param name="reportType">Report Type</param>
        /// <returns></returns>
        public Dictionary<String, String> FetchUnityPACSReportData_BR(int priorcount, String reportType = "PDF")
        {
            Dictionary<String, String> report = new Dictionary<String, String>();
            String script = System.IO.File.ReadAllText("Scripts\\JSFiles\\FetchUnityPACSReportData.js");

            script = script + "return getUnityPACSReportData()";
            BasePage.Driver.SwitchTo().DefaultContent();
            dynamic reportdata = ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(script);

            if (reportdata != null)
            {
                foreach (var item in reportdata)
                {
                    report.Add(item.Key, item.Value);
                }
            }
            return report;
        }

        /// <summary>
        /// This method is to ensure the required elements are present in the Email study window
        /// </summary>
        /// <param name="isPriorsExists"></param>  		
        public Boolean ValidateEmailStudyDialogue(bool isPriorsExists = false)
        {
            bool isAllElementsAvailable = false;
            if (IsElementVisible(By.CssSelector(input_email)) && IsElementVisible(By.CssSelector(input_emailName)) &&
                IsElementVisible(By.CssSelector(input_Notes)) && IsElementVisible(By.CssSelector(label_emailAttachedStudies)) &&
                IsElementVisible(By.CssSelector(label_emailModalityDropdown)) && IsElementVisible(By.CssSelector(div_emailStudyList)) &&
                IsElementVisible(By.CssSelector(div_sendEmail)) && IsElementVisible(By.CssSelector(div_cancelEmail)))
            {
                isAllElementsAvailable = true;
            }
            if (isPriorsExists)
            {
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(BluRingViewer.div_emailSelectAll)));
                BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(BluRingViewer.div_emailRelatedStudyList)));
                Thread.Sleep(5000);
                if (!(IsElementVisible(By.CssSelector(div_emailSelectAll)) && IsElementVisible(By.CssSelector(div_emailRelatedStudyList))))
                {
                    isAllElementsAvailable = false;
                }
            }
            return isAllElementsAvailable;
        }


        /// <summary>
        /// This method is used to fill the required imformation in Email Study window and sent it.
        /// </summary>
        /// <param name="emailAddress"></param>  	
        /// <param name="name"></param>  	
        /// <param name="reason"></param>  	
        /// <param name="selectAll"></param>  	
        /// <param name="isOpenEmailWindow"></param>  	
        public void EmailStudy(string emailAddress = "", string name = "", string reason = "", bool selectAll = false, bool isOpenEmailWindow = true)
        {
            //Open Email Study window
            if (isOpenEmailWindow)
            {
                this.ClickElement(BasePage.Driver.FindElement(By.CssSelector(div_emailstudy)));
                this.WaitTillEmailWindowAppears(selectAll);
            }

            IWebElement emailEle = Driver.FindElement(By.CssSelector(input_email));
            IWebElement nameEle = Driver.FindElement(By.CssSelector(input_emailName));
            IWebElement reasonEle = Driver.FindElement(By.CssSelector(input_Notes));

            //Clear fields
            emailEle.Clear();
            nameEle.Clear();
            reasonEle.Clear();

            //Enter Email Address, Name and reason
            this.SendKeys(emailEle, emailAddress);
            this.SendKeys(nameEle, name);
            this.SendKeys(reasonEle, reason);
            if (selectAll)
            {
                ClickElement(Driver.FindElement(By.CssSelector(div_emailSelectAll)));
            }

            // Click on Send button
            ClickElement(Driver.FindElement(By.CssSelector(div_sendEmail)));
            Thread.Sleep(3000);
        }

        /// <summary>
        /// This method is used to return the Pin number
        /// </summary>
        public String FetchPin_BR()
        {
            BasePage.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(div_pinWindow)));
            var pinwindow = BasePage.Driver.FindElement(By.CssSelector(div_pinWindow));
            String pinnumber = pinwindow.FindElement(By.CssSelector(".dialogFooter label")).GetAttribute("innerHTML");
            this.ClickElement(pinwindow.FindElement(By.CssSelector("span")));
            BasePage.wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.CssSelector(div_pinWindow)));
            return pinnumber;
        }


        /// <summary>
        /// Open the About Splash Screen in BluRing Viewer.
        /// </summary>
        public void OpenAboutSplashScreen()
        {
            if (SBrowserName.ToLower().Contains("edge"))
            {
                ClickElement(GetElement("cssselector", div_HelpIcon));
                Thread.Sleep(2000);
                ClickElement(GetElement("cssselector", li_AboutIcon + ":nth-child(2)"));
            }
            else
            {
                this.GetElement("cssselector", div_HelpIcon).Click();
                Thread.Sleep(2000);
                this.GetElement("cssselector", li_AboutIcon + ":nth-child(2)").Click();
            }
        }

        /// <summary>
        /// This method is used to get to the count of attached studies in email window.
        /// </summary>
        public int getAttachedStudiesCount()
        {
            String text = BasePage.Driver.FindElement(By.CssSelector(BluRingViewer.label_emailAttachedStudies)).Text;
            return Int32.Parse(text.Split('(')[1].Split(')')[0].Trim());
        }

        // <summary>
        /// This method gets the accession number from Email Study window
        /// </summary>
        /// <param name="ele">This parameter is prior Webelement</param>
        /// <returns></returns>
        public String GetAccessionFromEmailStudy(IWebElement ele)
        {
            String accession = String.Empty;
            var innertext = ele.GetAttribute("innerHTML");
            accession = innertext.Replace("<label>ACC#:</label>", "");
            return accession.Trim();
        }                

        /// <summary>
        /// This method return the Exam List tool tip for a exam card
        /// </summary>
        /// <param name="accession"></param>
        /// <returns></returns>
        public String GetExamListToolTip(String accession)
        {
            var priors = BasePage.Driver.FindElements(By.CssSelector(div_priors));
            IWebElement prior = null;
            foreach (IWebElement prior1 in priors)
            {
                var accession_ele = prior1.FindElement(By.CssSelector(AccessionNumberInExamList));
                if (this.GetAccession(accession_ele).Equals(accession))
                {
                    prior = prior1;
                    break;
                }
            }
            new TestCompleteAction().MoveToElement(prior).Perform();
            var tooltip = prior.GetAttribute("title");
            return tooltip;

        }

        /// <summary>
        /// This method is to retrieve all tool names of all groups in the toolbox
        /// </summary>
        /// <returns></returns>
        public IList<string> GetAllToolNamesinViewPort()
        {
            IList<String> totaltools = GetToolsInToolBoxByGrid();
            IList<String> totaltools1 = new List<string>();
            for (int i = 0; i <= totaltools.Count - 1; i++)
            {
                if (totaltools[i].Contains(','))
                {
                    for (int j = 0; j <= totaltools[i].Split(',').Length - 1; j++)
                    {
                        totaltools1.Add(totaltools[i].Split(',')[j]);
                    }
                }
                else
                {
                    totaltools1.Add(totaltools[i]);
                }
            }
            return totaltools1;
        }

        /// <summary>
        /// This method is to retriev all the accessions of the enabled report icons
        /// </summary>
        /// <returns></returns>
        public IList<string> GetMappingAccofEnabledReports()
        {
            BasePage.Driver.SwitchTo().DefaultContent().SwitchTo().Frame("UserHomeFrame");
            IList<string> List = new List<string>();
            IList<IWebElement> myparentList = new List<IWebElement>();
            int i = 0;
            IList<IWebElement> ActiveExamInExamList = Driver.FindElements(By.CssSelector(BluRingViewer.div_priors));
            IList<IWebElement> myList = BasePage.Driver.FindElements(By.CssSelector(EnabledReportIcons));

            foreach (IWebElement ele in myList)
            {
                myparentList.Add((IWebElement)((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(
                                                   "return arguments[0].parentNode;", ele));
                List.Add(GetAccession(myparentList[i].FindElement(By.CssSelector(BluRingViewer.AccessionNumberInExamList))));
                if (i <= myList.Count)
                {
                    i++;
                }
                else { break; }
            }
            Logger.Instance.InfoLog("Retrieved the list of accessions which has reports:"+List.ToString());
            return List;
        }

        public int GetSliderValue(int studyPanelNum = 1, int viewportNum = 1)
        {
            String studypanel = div_studypanel + ":nth-of-type(" + studyPanelNum + ")";
            String studyviewport = "div.viewerContainer:nth-of-type(" + viewportNum + ") ";            
            IWebElement ele = BasePage.Driver.FindElement(By.CssSelector(studypanel + " " + studyviewport + " " + BluRingViewer.div_StackSlider));
            return Int32.Parse(ele.GetAttribute("innerHTML"));
        }

        public int GetSliderMaxValue(int studyPanelNum = 1, int viewportNum = 1)
		{
			String studypanel = div_studypanel + ":nth-of-type(" + studyPanelNum + ")";
			String studyviewport = "div.viewerContainer:nth-of-type(" + viewportNum + ") ";
            IWebElement ele = BasePage.Driver.FindElement(By.CssSelector(studypanel + " " + studyviewport + " " + BluRingViewer.div_StackSliderMax));
			return Int32.Parse(ele.GetAttribute("innerHTML"));
		}

		/// <summary>
		/// Joint Lines Measurement
		/// </summary>
		public void ApplyTool_JointLines(int dragStartX = 0, int dragStartY = 0, int dropX = 0, int dropY = 0, int dropEndX = 0, int dropEndY = 0)
        {
            this.ApplyTool_AngleMeasurement(dragStartX, dragStartY, dropX, dropY);
        }

        /// <summary>
        /// This method is used to delete the existing annotation by mouse hover and right clicking
        /// </summary>
        /// <param name="Xcoordinate"></param>
        /// <param name="Ycoordinate"></param>
        public void DeleteAnnotation(int Xcoordinate = 0, int Ycoordinate = 0)
        {
            if (BasePage.SBrowserName.Contains("firefox"))
            {
                var actions = new TestCompleteAction();
                actions.MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), Xcoordinate, Ycoordinate).Release();
                Thread.Sleep(1000);
                actions.ContextClick().Perform();
            }
            else
            {
                var action = new Actions(BasePage.Driver);
                action.MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), Xcoordinate, Ycoordinate).Release().Build().Perform();
                Thread.Sleep(1000);
                action.ContextClick().Build().Perform();
            }
            Logger.Instance.InfoLog("The existing annotation is deleted");
        }

        /// <summary>
        /// TransischialMeasurement
        /// </summary>
        public void ApplyTool_TransischialMeasurement(int dragStartX = 0, int dragStartY = 0, int dropX = 0, int dropY = 0, int middragStartX = 0, int middragStartY = 0, int middropX = 0, int middropY = 0)
        {
            this.PerformTool(dragStartX, dragStartY, dropX, dropY);
            if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("firefox"))
            {
                var actions = new TestCompleteAction();
                actions.MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), middragStartX, middragStartY).Click();
                Thread.Sleep(2000);
                actions.MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), middropX, middropY).Click();
            }
            else
            {
                new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), middragStartX, middragStartY).Build().Perform();
                Thread.Sleep(2000);
                new Actions(BasePage.Driver).Click().Build().Perform();
                Thread.Sleep(2000);
                new Actions(BasePage.Driver).MoveToElement(BasePage.Driver.FindElement(By.CssSelector(Activeviewport)), middropX, middropY).Build().Perform();
                Thread.Sleep(2000);
                new Actions(BasePage.Driver).Click().Build().Perform();
            }
            Thread.Sleep(Config.ms_minTimeout);
        }

        // <summary>
        /// This method is used to verify localizer line is ON or OFF
        /// </summary>
        /// <returns></returns>
        public Boolean IsLocalizerON()
        {
            var className = GetElement(BasePage.SelectorType.CssSelector, div_LocalizerSwitch).GetAttribute("class");
            var status = className.Contains("isToolActive") ? true : false;
            Logger.Instance.InfoLog("The state of localizer line is " + status);
            return status;            
        }

        /// <summary>
        /// This method is used to turn ON / OFF Localizer inside the viewer
        /// </summary>
        /// <param name="turnOnLocalizer"></param>to enable localizer pass "true", pass "false" to disabled
        public void SetLocalizer(bool turnOnLocalizer = true)
        {
            var localizerElement = GetElement(BasePage.SelectorType.CssSelector, div_LocalizerLinesIcon);
            if (turnOnLocalizer)
            {
                if (!IsLocalizerON())
                {
                    ClickElement(localizerElement);
                    Thread.Sleep(2000);
                    Logger.Instance.InfoLog("The Localizer Icon is clicked and localizer is ON");
                }
                else               
                    Logger.Instance.InfoLog("The Localizer is already ON");
            }
            else
            {
                if (!IsLocalizerON())                
                    Logger.Instance.InfoLog("The Localizer is already turned OFF");                
                else
                {
                    ClickElement(localizerElement);
                    Thread.Sleep(2000);
                    Logger.Instance.InfoLog("The Localizer  is OFF");
                }
            }
        }

        /// <summary>
        /// This method is used to click Email Study Icon in respective studypanel
        /// </summary>
        /// <param name="studyPanel"></param> should start from 1
        /// <param name="overlay"></param> should start from 0
        public void clickEmailStudyIcon(int studyPanel, int overlay = 0)
        {
            this.ClickElement(this.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_studypanel + ":nth-of-type(" + studyPanel + ")  " + BluRingViewer.div_StudypanelMoreButton));
            BasePage.wait.Until<bool>(e => e.FindElement(By.CssSelector(BluRingViewer.div_overlayPanel)).Displayed);
            this.ClickElement(this.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_overlay + overlay + " " + BluRingViewer.StudypanelEmailStudy));
            this.WaitTillEmailWindowAppears();
            Logger.Instance.InfoLog("The EmailStudy icon from overlay panel is clicked successfully in studypanel " + studyPanel);
        }


        /// <summary>
        /// This Method is used to wait for the thumbnails to load
        /// </summary>
        /// <param name="timeout"></param>
        public void waitForThumbnailstoLoad(int timeout = 180)
        {
            //Wait Obejcts
            var thumbnailwait_toappear = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, 30));
            thumbnailwait_toappear.IgnoreExceptionTypes(new Type[] { (new StaleElementReferenceException().GetType()) });
            var thumbnailwait_disappear = new WebDriverWait(BasePage.Driver, new TimeSpan(0, 0, timeout));
            thumbnailwait_disappear.IgnoreExceptionTypes(new Type[] { (new StaleElementReferenceException().GetType()) });

            //Wait Till Thumbnail Loading Text Appears
            try
            {
                thumbnailwait_toappear.Until<Boolean>((d) =>
                {
                    Logger.Instance.InfoLog("Inside Block Wait Till Thumbnail Loading Text Appears");
                    var thumbnails = d.FindElements(By.CssSelector(div_thumbnailloadingstatus));
                    bool isThumbnailLoading = false;
                    foreach (IWebElement thumbnail in thumbnails)
                    {
                        if (thumbnail.GetAttribute("innerHTML").ToLower().Contains("loading"))
                        {
                            Logger.Instance.InfoLog("Thumbnails are Loading..");
                            isThumbnailLoading = true;
                            return true;
    }
                        else
                        {
                            continue;
}
                    }
                    if (isThumbnailLoading) { return true; } else { return false; }
                });
            }
            catch (Exception) { Logger.Instance.InfoLog("In Catch ..Waiting for Thumbnail Loadin.."); }


            //Wait Till Thumbnails Loading Text disappears
            try
            {
                thumbnailwait_disappear.Until<Boolean>((d) =>
                {
                    Logger.Instance.InfoLog("Inside Block Wait Till Thumbnail Loading Text DisAppears");
                    var thumbnails = d.FindElements(By.CssSelector(div_thumbnailloadingstatus));
                    if (thumbnails.Count != 0)
                        return false;
                    else
                        return true;
                });
            }
            catch (Exception) { Logger.Instance.InfoLog("In Catch ..Waiting for Thumbnail Loadin.. to disapperar"); }
        }

        /// <summary>
        /// This method is used to click Global Stack Icon in respective studypanel
        /// </summary>
        /// <param name="studyPanel"></param> should start from 1

        public void clickglobalstackIcon(int studyPanel)
        {
            this.ClickElement(this.GetElement(BasePage.SelectorType.CssSelector, BluRingViewer.div_globalstackicon(studyPanel)));
            Logger.Instance.InfoLog("The globalstackicon is clicked successfully in studypanel " + studyPanel);
        }


        /// <summary>
        /// This method is used to click Exam Mode button in respective studypanel
        /// </summary>
        /// <param name="studyPanel"></param> should start from 1
        public void OpenExammode(int viewport = 1, int panel = 1)
        {
            string viewerMenuButton = div_StudyPanel + ":nth-of-type(" + panel + ") div.viewerContainer:nth-of-type(" + viewport + ") " + div_viewerMenuButton;
            if (SBrowserName.ToLower().Equals("internet explorer"))
            {
                ClickElement(GetElement("cssselector", viewerMenuButton));
            }
            else
            {
                Click("cssselector", viewerMenuButton);

            }
            Thread.Sleep(3000);
            Click("cssselector", div_Exammodebutton);
        }

    }
}

