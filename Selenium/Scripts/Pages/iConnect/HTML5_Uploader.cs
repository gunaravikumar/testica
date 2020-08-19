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
    class HTML5_Uploader : BasePage
    {
        #region Constructor
        public HTML5_Uploader() { }
        #endregion Constructor

        #region Webelements
        public By By_RegisteredUserRadioBtn() { return By.CssSelector("input[id$='_RegisteredUser']"); }
        public By By_GuestUserRadioBtn() { return By.CssSelector("input[id$='_GuestUser']"); }
        public By By_UserNameTxtBox() { return By.CssSelector("input[id$='_Username']"); }
        public By By_PasswordTxtBox() { return By.CssSelector("input[id$='_Password']"); }
        public By By_EmailTxtBox() { return By.CssSelector("input[id$='_Email']"); }
        public By By_PhoneTxtBox() { return By.CssSelector("input[id$='_PhoneNumber']"); }
        public By By_SignInBtn() { return By.CssSelector("input[id$='_LoginButton']"); }
        public IWebElement HTML5LoginErrorMsgLabel() { return Driver.FindElement(By.CssSelector("span[id='ctl00_MainContentPlaceHolder_ErrorMessage']")); }
        public By By_AddFolderBtn() { return By.CssSelector("div[id$='addFolderToJob_1']"); }
        public By By_AddFileBtn() { return By.CssSelector("div[id$='addFilesToJob_1']"); }
        public By By_DownloadEIOptional() { return By.CssSelector("span[id='DownloadExamImporterOptional']"); }
        public By By_DownloadEIHelp() { return By.CssSelector("div[id='ctl00_MainContentPlaceHolder_divToolTip']"); }
        public By By_DownloadEIHelpToolTip() { return By.CssSelector("#ctl00_MainContentPlaceHolder_divToolTip > div"); }
        public By By_HippaComplianceLabel() { return By.CssSelector("span[id$='_HipaaComplianceLabel']"); }
        public By By_HipaaComplianceDescription() { return By.CssSelector("div[id$='_HipaaComplianceDescription']"); }
        public By By_HippaAgreeChkBox() { return By.CssSelector("input[id$='_AgreementCB']"); }
        public By By_AttachmentSelectChkBox() { return By.CssSelector("input[id$='cb_1033']"); }
        public By By_HippaContinueBtn() { return By.CssSelector("input[id$='_ContinueButton']"); }
        public By By_UsernameDisplayed() { return By.CssSelector("span[id$='_UserName']"); }
        public By By_NoDICOMMsgDisplayed() { return By.CssSelector("div[id$='messageDiv']"); }
        public By By_SignOutBtn() { return By.CssSelector("a[title='Log Out']"); }
        public By By_UploadFilesBtn() { return By.CssSelector("span[id$='_UploadFilesLabel']"); }
        public By By_UploadFolderBtn() { return By.CssSelector("span[id$='_UploadFolderLabel']"); }
        public By By_DragFilesDiv() { return By.CssSelector("div[id$='divUploadFilesArea']"); }
        public By By_JobErrorMsg() { return By.CssSelector("div[id='divJobErrorMessageContainer']"); }
        public By By_CancelJobBtn() { return By.CssSelector("div[id$='cancelJob_1']"); }
        public By By_nonDICOMYesBtn() { return By.CssSelector("div[id$='yesButton']"); }
        public IWebElement RegisteredUserRadioBtn() { return PageLoadWait.WaitForElement(By_RegisteredUserRadioBtn(), WaitTypes.Visible); }
        public IWebElement GuestUserRadioBtn() { return PageLoadWait.WaitForElement(By_GuestUserRadioBtn(), WaitTypes.Visible); }
        public IWebElement UserNameTxtBox() { return PageLoadWait.WaitForElement(By_UserNameTxtBox(), WaitTypes.Visible); }
        public IWebElement PasswordTxtBox() { return PageLoadWait.WaitForElement(By_PasswordTxtBox(), WaitTypes.Visible); }
        public IWebElement EmailTxtBox() { return PageLoadWait.WaitForElement(By_EmailTxtBox(), WaitTypes.Visible); }
        public IWebElement PhoneTxtBox() { return PageLoadWait.WaitForElement(By_PhoneTxtBox(), WaitTypes.Visible); }
        public IWebElement SignInBtn() { return PageLoadWait.WaitForElement(By_SignInBtn(), WaitTypes.Visible); }
        public IWebElement DomainSelectorPopup() { return PageLoadWait.WaitForElement(By.CssSelector("div[id$='_ImageSharingDomains']"), WaitTypes.Visible); }
        public SelectElement DomainSelectorDropdown() { return new SelectElement(PageLoadWait.WaitForElement(By.CssSelector("select[id$='_SelectDomainName']"), WaitTypes.Visible)); }
        public IWebElement DomainSelectorGoBtn() { return PageLoadWait.WaitForElement(By.CssSelector("input[id$='_GoButton']"), WaitTypes.Visible); }
        public IWebElement AddFolderBtn() { return PageLoadWait.WaitForElement(By_AddFolderBtn(), WaitTypes.Visible); }
        public IWebElement AddFileBtn() { return PageLoadWait.WaitForElement(By_AddFileBtn(), WaitTypes.Visible); }
        public IWebElement DownloadEIOptional() { return PageLoadWait.WaitForElement(By_DownloadEIOptional(), WaitTypes.Visible); }
        public IWebElement DownloadEIHelp() { return PageLoadWait.WaitForElement(By_DownloadEIHelp(), WaitTypes.Visible); }
        public IWebElement DownloadEIHelpToolTip() { return PageLoadWait.WaitForElement(By_DownloadEIHelpToolTip(), WaitTypes.Visible); }
        public IWebElement HippaComplianceLabel() { return PageLoadWait.WaitForElement(By_HippaComplianceLabel(), WaitTypes.Visible); }
        public IWebElement HipaaComplianceDescription() { return PageLoadWait.WaitForElement(By_HipaaComplianceDescription(), WaitTypes.Visible); }
        public IWebElement HippaAgreeChkBox() { return PageLoadWait.WaitForElement(By_HippaAgreeChkBox(), WaitTypes.Visible); }
        public IWebElement AttachmentSelectChkBox() { return PageLoadWait.WaitForElement(By_AttachmentSelectChkBox(), WaitTypes.Visible); }
        public IWebElement HippaContinueBtn() { return PageLoadWait.WaitForElement(By_HippaContinueBtn(), WaitTypes.Visible); }
        public IWebElement WelcomeMessage() { return PageLoadWait.WaitForElement(By.CssSelector("div[id$='_LogOutDiv']>a"), WaitTypes.Visible); }
        public IWebElement UsernameDisplayed() { return PageLoadWait.WaitForElement(By_UsernameDisplayed(), WaitTypes.Visible); }
        public IWebElement NoDICOMMsgDisplayed() { return PageLoadWait.WaitForElement(By_NoDICOMMsgDisplayed(), WaitTypes.Visible); }
        public IWebElement SignOutBtn() { return PageLoadWait.WaitForElement(By_SignOutBtn(), WaitTypes.Visible); }
        public IWebElement UploadFilesBtn() { return PageLoadWait.WaitForElement(By_UploadFilesBtn(), WaitTypes.Visible); }
        public IWebElement UploadFolderBtn()
        {
            //if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
            //{
            //    return PageLoadWait.WaitForElement(By_UploadFilesBtn(), WaitTypes.Visible);
            //}
            //else
            //{
            //    return PageLoadWait.WaitForElement(By_UploadFolderBtn(), WaitTypes.Visible); 
            //}
            return PageLoadWait.WaitForElement(By_UploadFolderBtn(), WaitTypes.Visible);
        }
        public IWebElement DragFilesDiv() { return PageLoadWait.WaitForElement(By_DragFilesDiv(), WaitTypes.Visible); }
        public IWebElement JobErrorMsg() { return PageLoadWait.WaitForElement(By_JobErrorMsg(), WaitTypes.Visible); }
        public IWebElement CancelJobBtn() { return PageLoadWait.WaitForElement(By_CancelJobBtn(), WaitTypes.Visible); }
        public IWebElement nonDICOMYesBtn() { return PageLoadWait.WaitForElement(By_nonDICOMYesBtn(), WaitTypes.Visible); }
        public By By_UploadJobContainer(int jobID = 1) { return By.CssSelector("div[id='jobContainer_" + jobID + "']"); }
        public IWebElement UploadJobContainer(int jobID = 1) { return PageLoadWait.WaitForElement(By_UploadJobContainer(jobID), WaitTypes.Visible,300); }
        public IList<IWebElement> UploadJobContainerList() { return Driver.FindElements(By.CssSelector("div[id^='jobContainer_']")); }
        public By By_PatientInfoContainer() { return By.CssSelector("div[id='divPatientInfo']"); }
        public IWebElement PatientInfoContainer() { return PageLoadWait.WaitForElement(By_PatientInfoContainer(), WaitTypes.Visible); }
        public IList<IWebElement> IndividualPatientInfoContainer() { return PageLoadWait.WaitForElement(By_PatientInfoContainer(), WaitTypes.Visible).FindElements(By.CssSelector("#patientInfoContainer")); }
        public By By_UploadJobProgressBar(int jobID) { return By.CssSelector("div[id='progressBar_" + jobID + "']"); }
        public IWebElement UploadJobProgressBar(int jobID = 1) { return PageLoadWait.WaitForElement(By_UploadJobProgressBar(jobID), WaitTypes.Visible); }
        public By By_AddFiles(int jobID) { return By.CssSelector("span[id='addFileSpan_" + jobID + "']"); }
        public IWebElement AddFiles(int jobID = 1) { return PageLoadWait.WaitForElement(By_AddFiles(jobID), WaitTypes.Visible); }
        public By By_AddFolder(int jobID) { return By.CssSelector("span[id='addFolderSpan_" + jobID + "']"); }
        public IWebElement AddFolder(int jobID = 1) { return PageLoadWait.WaitForElement(By_AddFolder(jobID), WaitTypes.Visible); }
        public By By_DragDropMessage(int jobID) { return By.CssSelector("span[id='dragDropMessage_" + jobID + "']"); }
        public IWebElement DragDropMessage(int jobID = 1) { return PageLoadWait.WaitForElement(By_DragDropMessage(jobID), WaitTypes.Visible); }
        public By By_DeleteJobButton(int jobID = 1) { return By.CssSelector("div[id='deleteJob_" + jobID + "']"); }
        public IWebElement DeleteJobButton(int jobID = 1) { return PageLoadWait.WaitForElement(By_DeleteJobButton(jobID), WaitTypes.Visible); }
        public By By_ShareJobButton(int jobID = 1) { return By.CssSelector("div[id='shareJob_" + jobID + "']"); }
        public IWebElement ShareJobButton(int jobID = 1) { return PageLoadWait.WaitForElement(By_ShareJobButton(jobID), WaitTypes.Visible); }
        public By By_DeleteButtons() { return By.CssSelector("div#divPatientInfo div.cardBgColor span.DeleteButton"); }
        public IList<IWebElement> DeleteButtons() { return Driver.FindElements(By_DeleteButtons()); }
        public By By_PatientNameSpan() { return By.CssSelector("span[id^='span_patientName']"); }
        public IList<IWebElement> PatientNameSpan() { return Driver.FindElements(By_PatientNameSpan()); }
        public By By_DOBSpan() { return By.CssSelector("span[id^='span_DobValue']"); }
        public IList<IWebElement> DOBSpan() { return Driver.FindElements(By_DOBSpan()); }
        public By By_AgeSpan() { return By.CssSelector("span[id^='span_Age']"); }
        public By By_YrsSpan() { return By.CssSelector("span[id^='span_YearLabel']"); }
        public IList<IWebElement> AgeSpan() { return Driver.FindElements(By_AgeSpan()); }
        public IList<IWebElement> YearsSpan() { return Driver.FindElements(By_YrsSpan()); }
        public By By_GenderSpan() { return By.CssSelector("span[id^='span_Gender']"); }
        public IList<IWebElement> GenderSpan() { return Driver.FindElements(By_GenderSpan()); }
        public By By_MRNSpan() { return By.CssSelector("span[id^='span_MrnValue']"); }
        public IList<IWebElement> MRNSpan() { return Driver.FindElements(By_MRNSpan()); }
        public By By_IPIDSpan() { return By.CssSelector("span[id^='span_IpIdValue']"); }
        public IList<IWebElement> IPIDSpan() { return Driver.FindElements(By_IPIDSpan()); }
        public By By_StudyDescSpan() { return By.CssSelector("span[id^='span_StudyDescValue']"); }
        public IList<IWebElement> StudyDescSpan() { return Driver.FindElements(By_StudyDescSpan()); }
        public By By_ModalitySpan() { return By.CssSelector("span[id^='span_ModalityValue']"); }
        public IList<IWebElement> ModalitySpan() { return Driver.FindElements(By_ModalitySpan()); }
        public By By_DateSpan() { return By.CssSelector("span[id^='span_DateValue']"); }
        public IList<IWebElement> DateSpan() { return Driver.FindElements(By_DateSpan()); }
        public By By_SeriesSpan() { return By.CssSelector("span[id^='span_SeriesValue']"); }
        public IList<IWebElement> SeriesSpan() { return Driver.FindElements(By_SeriesSpan()); }
        public By By_ImagesSpan() { return By.CssSelector("span[id^='span_ImageValue']"); }
        public IList<IWebElement> ImagesSpan() { return Driver.FindElements(By_ImagesSpan()); }
        public By By_AccessionSpan() { return By.CssSelector("span[id^='span_StudyAccValue']"); }
        public IList<IWebElement> AccessionSpan() { return Driver.FindElements(By_AccessionSpan()); }
        public By By_InstitutionSpan() { return By.CssSelector("span[id^='span_InstitutioValue']"); }
        public IList<IWebElement> InstitutionSpan() { return Driver.FindElements(By_InstitutionSpan()); }
        public By By_RefPhysSpan() { return By.CssSelector("span[id^='span_PhysicianValue']"); }
        public IList<IWebElement> RefPhysSpan() { return Driver.FindElements(By_RefPhysSpan()); }
        public By By_StudyPhoneTxtBox() { return By.CssSelector("input[id^='txt_ContactNo_']"); }
        public IList<IWebElement> StudyPhoneTxtBox() { return Driver.FindElements(By_StudyPhoneTxtBox()); }
        public By By_StudyEmailTxtBox() { return By.CssSelector("input[id^='txt_EmailId_']"); }
        public IList<IWebElement> StudyEmailTxtBox() { return Driver.FindElements(By_StudyEmailTxtBox()); }
        public By By_AttachmentButtons() { return By.CssSelector("div[id^='attachmentDiv_']"); }
        public IList<IWebElement> AttachmentButtons() { return Driver.FindElements(By_AttachmentButtons()); }
        public By By_DestinationDropdown() { return By.CssSelector("select[id$='destinationDropDownList']"); }
        public SelectElement DestinationDropdown() { return new SelectElement(PageLoadWait.WaitForElement(By_DestinationDropdown(), WaitTypes.Visible)); }
        public By By_PriorityDropdown() { return By.CssSelector("select[id$='priorityDropDownList']"); }
        public SelectElement PriorityDropdown() { return new SelectElement(PageLoadWait.WaitForElement(By_PriorityDropdown(), WaitTypes.Visible)); }
        public By By_CommentTextBox() { return By.CssSelector("textarea[id$='textAreaComments']"); }
        public IWebElement CommentTextBox() { return PageLoadWait.WaitForElement(By_CommentTextBox(), WaitTypes.Visible); }
        public By By_ShareBtn() { return By.CssSelector("div[id$='shareButton']"); }
        public IWebElement ShareBtn() { return PageLoadWait.WaitForElement(By_ShareBtn(), WaitTypes.Visible); }
        public By By_CancelShareBtn() { return By.CssSelector("div[id$='cancelShareButton']"); }
        public IWebElement CancelBtn() { return PageLoadWait.WaitForElement(By_CancelBtn(), WaitTypes.Visible); }
        public By By_CancelBtn() { return By.CssSelector("div[id$='cancelButton']"); }
        public IWebElement CancelShareBtn() { return PageLoadWait.WaitForElement(By_CancelShareBtn(), WaitTypes.Visible); }
        public By By_StudyRows() { return By.CssSelector("div[class='ShareJobElements dynamicDiv']"); }
        public IList<IWebElement> StudyRows() { return Driver.FindElements(By_StudyRows()); }
        public IList<IWebElement> StudyDetailsonSharePage(int rowNumber = 0) { return StudyRows()[rowNumber].FindElements(By.CssSelector("div[class='numberText']")); }
        public IWebElement PatientNameonSharePage(int rowNumber = 0) { return StudyRows()[rowNumber].FindElement(By.CssSelector("div[class='patientName']")); }
        public By By_ModalDialogDiv() { return By.CssSelector("div[id='ModalDialogDiv']"); }
        public IWebElement ModalDialogDiv() { return PageLoadWait.WaitForElement(By_ModalDialogDiv(), WaitTypes.Visible); }
        public By By_ModalMessageDiv() { return By.CssSelector("div[id='messageDiv']"); }
        public IWebElement ModalMessageDiv() { return PageLoadWait.WaitForElement(By_ModalMessageDiv(), WaitTypes.Visible); }
        public By By_YesBtn() { return By.CssSelector("div[id='yesButton']"); }
        public IWebElement YesBtn() { return PageLoadWait.WaitForElement(By_YesBtn(), WaitTypes.Visible); }
        public By By_NoBtn() { return By.CssSelector("div[id='noButton']"); }
        public IWebElement NoBtn() { return PageLoadWait.WaitForElement(By_NoBtn(), WaitTypes.Visible); }
        public IWebElement ModalOkBtn() { return PageLoadWait.WaitForElement(By.CssSelector("span[id='okButton']"), WaitTypes.Visible); }
        public IWebElement ModalCancelBtn() { return PageLoadWait.WaitForElement(By.CssSelector("span[id='cancelButton']"), WaitTypes.Visible); }

        //New Patient Elements
        public By By_DeleteJobBtn(int jobID) { return By.CssSelector("div[id='deleteNewPatientJob_" + jobID + "']"); }
        public By By_SaveBtn(int jobID) { return By.CssSelector("div[id='saveNewPatientJob_" + jobID + "']"); }
        public IWebElement DeleteJobBtn(int jobID = 1) { return PageLoadWait.WaitForElement(By_DeleteJobBtn(jobID), WaitTypes.Visible); }
        public IWebElement SaveBtn(int jobID = 1) { return PageLoadWait.WaitForElement(By_SaveBtn(jobID), WaitTypes.Visible); }
        public IWebElement FamilynameTxtBox(int jobID = 1) { return PageLoadWait.WaitForElement(By.CssSelector("#txtBoxFamilyName_" + jobID), WaitTypes.Visible); }
        public IWebElement FirstnameTxtBox(int jobID = 1) { return PageLoadWait.WaitForElement(By.CssSelector("#txtBoxFirstName_" + jobID), WaitTypes.Visible); }
        public IWebElement BirthdateTxtBox(int jobID = 1) { return PageLoadWait.WaitForElement(By.CssSelector("#txtBoxDOB_" + jobID), WaitTypes.Visible); }
        public SelectElement GenderListBox(int jobID = 1) { return new SelectElement(PageLoadWait.WaitForElement(By.CssSelector("#dropDownGender_" + jobID), WaitTypes.Visible)); }
        public IWebElement MRNTxtBox(int jobID = 1) { return PageLoadWait.WaitForElement(By.CssSelector("#txtBoxMRN_" + jobID), WaitTypes.Visible); }
        public IWebElement DescriptionTxtBox(int jobID = 1) { return PageLoadWait.WaitForElement(By.CssSelector("#txtBoxDescription_" + jobID), WaitTypes.Visible); }
        public IWebElement InstitutionTxtBox(int jobID = 1) { return PageLoadWait.WaitForElement(By.CssSelector("#txtBoxInstitution_" + jobID), WaitTypes.Visible); }
        public IWebElement RefPhysTxtBox(int jobID = 1) { return PageLoadWait.WaitForElement(By.CssSelector("#txtBoxRefPhy_" + jobID), WaitTypes.Visible); }
        public IWebElement NewPatientPhoneTxtBox(int jobID = 1) { return PageLoadWait.WaitForElement(By.CssSelector("#txtBoxPhoneNumber_" + jobID), WaitTypes.Visible); }
        public IWebElement NewPatientEmailTxtBox(int jobID = 1) { return PageLoadWait.WaitForElement(By.CssSelector("#txtBoxEmail_" + jobID), WaitTypes.Visible); }
        public By By_MultiDomainDropdown() { return By.CssSelector("select[id$='SelectDomainName']"); }
        public SelectElement MultiDomainDropdown() { return new SelectElement(PageLoadWait.WaitForElement(By_MultiDomainDropdown(), WaitTypes.Visible)); }
        public By By_ChooseDomainDisplayed() { return By.CssSelector("#ctl00_NonRegisterUserControl_Label1"); }
        public IWebElement ChooseDomainDisplayed() { return PageLoadWait.WaitForElement(By_ChooseDomainDisplayed(), WaitTypes.Visible); }
        public By By_GoBtnDisplayed() { return By.CssSelector("input[id$='GoButton']"); }
        public IWebElement GoBtnDisplayed() { return PageLoadWait.WaitForElement(By_GoBtnDisplayed(), WaitTypes.Visible); }
        //
        public By By_CancelJobBtn(int jobID) { return By.CssSelector("div[id='cancelJob_" + jobID + "']"); }
        public IWebElement CancelJobBtn(int jobID = 1) { return PageLoadWait.WaitForElement(By_CancelJobBtn(jobID), WaitTypes.Visible); }
        //Attachment Elements
        public IWebElement AttachmentFilesContainer() { return PageLoadWait.WaitForElement(By.CssSelector("div[id='divFilesContainer']"), WaitTypes.Visible); }
        public IList<IWebElement> AttachmentCheckboxes() { return AttachmentFilesContainer().FindElements(By.TagName("input")); }
        public IList<IWebElement> AttachmentLabels() { return AttachmentFilesContainer().FindElements(By.TagName("label")); }
        public By By_PatientDropdown() { return By.CssSelector("select[id$='patientSelectList']"); }
        public SelectElement PatientDropdown() { return new SelectElement(PageLoadWait.WaitForElement(By_PatientDropdown(), WaitTypes.Visible)); }
        public By By_StudyDropdown() { return By.CssSelector("select[id$='studySelectList']"); }
        public SelectElement StudyDropdown() { return new SelectElement(PageLoadWait.WaitForElement(By_StudyDropdown(), WaitTypes.Visible)); }
        public IWebElement AttachmentCancelBtn() { return PageLoadWait.WaitForElement(By.CssSelector("div[id='btnCancelNonDicom']"), WaitTypes.Visible); }
        public IWebElement AttachmentSubmitBtn() { return PageLoadWait.WaitForElement(By.CssSelector("div[id='btnAttachNonDicom']"), WaitTypes.Visible); }

        //New Patient Demographics - URL launch

        public By By_newPatientNameSpan() { return By.CssSelector("span[id='newPatientName']"); }
        public IList<IWebElement> newPatientNameSpan() { return Driver.FindElements(By_newPatientNameSpan()); }
        public By By_newPatientDobValueSpan() { return By.CssSelector("span[id='newPatientDobValue']"); }
        public IList<IWebElement> newPatientDobValueSpan() { return Driver.FindElements(By_newPatientDobValueSpan()); }
        public By By_newPatientMrnValueSpan() { return By.CssSelector("span[id='newPatientMrnValue']"); }
        public IList<IWebElement> newPatientMrnValueSpan() { return Driver.FindElements(By_newPatientMrnValueSpan()); }
        public By By_newPatientIpIdValueSpan() { return By.CssSelector("span[id='newPatientIpIdValue']"); }
        public IList<IWebElement> newPatientIpIdValueSpan() { return Driver.FindElements(By_newPatientIpIdValueSpan()); }
        public By By_newPatientGenderLbSpan() { return By.CssSelector("span[id='newPatientGenderLb']"); }
        public IList<IWebElement> newPatientGenderLbSpan() { return Driver.FindElements(By_newPatientGenderLbSpan()); }
        public By By_newPatientDemographicsLabelSpan() { return By.CssSelector("span[id='newPatientDemographicsLabel']"); }
        public IList<IWebElement> newPatientDemographicsLabelSpan() { return Driver.FindElements(By_newPatientDemographicsLabelSpan()); }
        public By By_newPatientDemographicsContainer() { return By.CssSelector("div[id='newPatientDemographicsDiv']"); }
        public IWebElement newPatientDemographicsContainer() { return PageLoadWait.WaitForElement(By_newPatientDemographicsContainer(), WaitTypes.Visible); }

        //by ravsoft starts here
        public By By_LogOutHTML5Btn() { return By.CssSelector("a[title='Log Out']"); }
        public IWebElement LogOutBtn() { return PageLoadWait.WaitForElement(By_LogOutHTML5Btn(), WaitTypes.Visible); }
        public By By_Activitylog() { return By.CssSelector("By.ClassName('activity_log')"); }
        public IList<IWebElement> Activitylog() { return Driver.FindElements(By_Activitylog()); }

        public By By_AddedAttachment() { return By.CssSelector("span[id^= 'div_AttachedFiles_']"); }
        public IList<IWebElement> AddedAttachment() { return Driver.FindElements(By_AddedAttachment()); }
        public By By_webEmailAddress() { return By.Id("ctl00_UserName"); }
        public IWebElement webEmailaddress() { return PageLoadWait.WaitForElement(By_webEmailAddress(), WaitTypes.Visible); }
        public By By_webIAggremnt() { return By.Id("ctl00_MainContentPlaceHolder_AgreementCB"); }
        public IWebElement webIAggrement() { return PageLoadWait.WaitForElement(By_webIAggremnt(), WaitTypes.Visible); }
        public By By_continueBt() { return By.Id("ctl00_MainContentPlaceHolder_ContinueButton"); }
        public IWebElement webContinueBtn() { return PageLoadWait.WaitForElement(By_continueBt(), WaitTypes.Visible); }
        public By By_upload_Files() { return By.Id("ctl00_MainContentPlaceHolder_UploadFilesLabel"); }
        public IWebElement webUploadFiles() { return PageLoadWait.WaitForElement(By_upload_Files(), WaitTypes.Visible); }
        public By By_ProgressBar_Cont() { return By.Id("progressBarContainer"); }
        public IWebElement ProgressBarCont() { return PageLoadWait.WaitForElement(By_ProgressBar_Cont(), WaitTypes.Visible); }
        public By By_JobCreated() { return By.Id("jobLabel_1"); }
        public IWebElement webJobcreated() { return PageLoadWait.WaitForElement(By_JobCreated(), WaitTypes.Visible); }
        public By By_Job2Created() { return By.Id("jobLabel_2"); }
        public IWebElement webJob2created() { return PageLoadWait.WaitForElement(By_Job2Created(), WaitTypes.Visible); }
        public By By_Upload_PhoneNo() { return By.XPath(".//input[contains(@id,'txt_ContactNo')]"); }
        public IList<IWebElement> PhoneNoUpload() { return Driver.FindElements(By_Upload_PhoneNo()); }
        public By By_Upload_Emailtxt() { return By.XPath(".//input[contains(@id,'txt_EmailId')]"); }
        public IList<IWebElement> EmailtxtUpload() { return Driver.FindElements(By_Upload_Emailtxt()); }
        public By By_div_shareJob() { return By.Id("divShareJob"); }
        public IWebElement Desdiv() { return PageLoadWait.WaitForElement(By_div_shareJob(), WaitTypes.Visible); }
        public By By_span_AddFolder(int jobid = 1) { return By.XPath("//span[@id='addFolderSpan_" + jobid + "']"); }
        public IWebElement AddFolderSpan() { return PageLoadWait.WaitForElement(By_span_AddFolder(), WaitTypes.Visible); }
        public By By_PatientCont() { return By.Id("patientInfoContainer"); }
        public IList<IWebElement> Patientcont() { return Driver.FindElements(By_PatientCont()); }
        public By By_Crossdelete() { return By.Id("span_DeleteStudy_"); }
        public IList<IWebElement> Crossdelete() { return Driver.FindElements(By_Crossdelete()); }
        public By By_CancelShareBT() { return By.XPath("//div[@id='divButtons']//following::div[@id='cancelShareButton']"); }
        public IWebElement CancelShareBT() { return Driver.FindElement(By_CancelShareBT()); }
        public By By_CancelOption() { return By.Id("cancelJob_1"); }
        public IWebElement CancelOption() { return PageLoadWait.WaitForElement(By_CancelOption(), WaitTypes.Visible); }
        public IWebElement EmailTxtBox(int jobID = 1) { return PageLoadWait.WaitForElement(By.CssSelector("#txtBoxPhoneNumber_" + jobID), WaitTypes.Visible); }
        public IWebElement PhoneTxtBox(int jobID = 1) { return PageLoadWait.WaitForElement(By.CssSelector("#txtBoxEmail_" + jobID), WaitTypes.Visible); }
        public By By_ErrMsg() { return By.Id("divJobErrorMessageContainer"); }
        public IWebElement ErrMsg() { return PageLoadWait.WaitForElement(By_ErrMsg(), WaitTypes.Visible); }
        public By By_GuestUserEmail() { return By.Id("ctl00_MainContentPlaceHolder_Email"); }
        public IWebElement GuestUserEmail() { return PageLoadWait.WaitForElement(By_GuestUserEmail(), WaitTypes.Visible); }
        public By By_GuestUserPh() { return By.Id("ctl00_MainContentPlaceHolder_PhoneNumber"); }
        public IWebElement GuestUserPh() { return PageLoadWait.WaitForElement(By_GuestUserPh(), WaitTypes.Visible); }
        public By By_GuestUserLoginBtn() { return By.Id("ctl00_MainContentPlaceHolder_LoginButton"); }
        public IWebElement GuestUserLoginBtn() { return PageLoadWait.WaitForElement(By_GuestUserLoginBtn(), WaitTypes.Visible); }
        public By By_JobPage() { return By.Id("labelJobNumber"); }
        public IWebElement JobPage() { return PageLoadWait.WaitForElement(By_JobPage(), WaitTypes.Visible); }

        public By By_NewPatientDemographic() { return By.Id("newPatientDemographicsLabel"); }
        public IWebElement NewPatientDemographic() { return PageLoadWait.WaitForElement(By_NewPatientDemographic(), WaitTypes.Visible); }
        public By By_NewPatientMRN() { return By.Id("newPatientMrnValue"); }
        public IWebElement NewPatientMRN() { return PageLoadWait.WaitForElement(By_NewPatientMRN(), WaitTypes.Visible); }
        public By By_NewPatientIPID() { return By.Id("newPatientIpIdValue"); }
        public IWebElement NewPatientIPID() { return PageLoadWait.WaitForElement(By_NewPatientIPID(), WaitTypes.Visible); }
        public By By_NewPatientName() { return By.Id("newPatientName"); }
        public IWebElement NewPatientName() { return PageLoadWait.WaitForElement(By_NewPatientName(), WaitTypes.Visible); }
        public By By_ListedAttachment() { return (By.XPath("//div[contains(@id, 'pan_AttachedFiles_')]")); }
        public IWebElement ListedAttachment() { return PageLoadWait.WaitForElement(By_ListedAttachment(), WaitTypes.Visible); }
        public By By_ProgreassBarCancel(int Jobid = 1) { return By.XPath("//div[@id='cancelJob_" + Jobid + "']"); }
        public IWebElement ProgressbarCancel(int Jobid = 1) { return PageLoadWait.WaitForElement(By_ProgreassBarCancel(Jobid), WaitTypes.Visible); }

        public By By_AttachYesButton() { return By.Id("yesButton"); }
        public IWebElement AttachYesButton() { return PageLoadWait.WaitForElement(By_AttachYesButton(), WaitTypes.Visible); }
        public By By_AddAttachment() { return By.Id("addFileSpan_1"); }
        public IWebElement AddAttachment() { return PageLoadWait.WaitForElement(By_AddAttachment(), WaitTypes.Visible); }
        public By By_AddFile() { return By.Id("addFileSpan_1"); }
        public IWebElement AddFile() { return PageLoadWait.WaitForElement(By_AddAttachment(), WaitTypes.Visible); }
        public By By_UnsupportedBrowserEIDownloader() { return By.Id("spn_DownloadExamImporter"); }
        public IWebElement UnsupportedBrowserEIDownloader() { return PageLoadWait.WaitForElement(By_UnsupportedBrowserEIDownloader(), WaitTypes.Visible); }
        public By By_UnsupportedBrowserWebUploader() { return By.Id("spn_LaunchJavaUploader"); }
        public IWebElement UnsupportedBrowserWebUploader() { return PageLoadWait.WaitForElement(By_UnsupportedBrowserWebUploader(), WaitTypes.Visible); }
        public By By_UnexpectedEAError() { return By.ClassName("normalred"); }
        public IWebElement UnexpectedEAError() { return PageLoadWait.WaitForElement(By_UnexpectedEAError(), WaitTypes.Visible); }
        public By By_OKButton() { return By.Id("okButton"); }
        public IWebElement OKButton() { return PageLoadWait.WaitForElement(By_OKButton(), WaitTypes.Visible); }
        public By By_CancelButton() { return By.Id("cancelButton"); }
        public IWebElement CancelButtn() { return PageLoadWait.WaitForElement(By_CancelButton(), WaitTypes.Visible); }
        public By By_Unsupportedbrowser() { return By.Id("span_browserNotSupported"); }
        public IWebElement Unsupportedbrowser() { return PageLoadWait.WaitForElement(By_Unsupportedbrowser(), WaitTypes.Visible); }
        public By By_MaincontentLoginBtn() { return By.ClassName("ctl00$MainContentPlaceHolder$LoginButton"); }
        public IWebElement MaincontentLoginBtn() { return PageLoadWait.WaitForElement(By_MaincontentLoginBtn(), WaitTypes.Visible); }
        public By By_SelectDomaininIE9() { return By.Id("sel_SelectDomainName"); }
        public SelectElement SelectDomaininIE9() { return new SelectElement(PageLoadWait.WaitForElement(By_SelectDomaininIE9(), WaitTypes.Visible)); }
        public By By_GotoBtninIE9() { return By.Id("btn_GoButton"); }
        public IWebElement GotoBtninIE9() { return PageLoadWait.WaitForElement(By_GotoBtninIE9(), WaitTypes.Visible); }
        public By By_PromptBox() { return By.Id("messageDiv"); }
        public IWebElement PromptBox() { return PageLoadWait.WaitForElement(By_PromptBox(), WaitTypes.Visible); }
        public By By_TxtBoxDesc() { return By.Id("txtBoxDescription_1"); }
        public IWebElement TxtBoxDesc() { return PageLoadWait.WaitForElement(By_TxtBoxDesc(), WaitTypes.Visible); }
        public By By_TxtBoxFname() { return By.Id("txtBoxFirstName_1"); }
        public IWebElement TxtBoxFname() { return PageLoadWait.WaitForElement(By_TxtBoxFname(), WaitTypes.Visible); }
        public By By_SavePatient() { return By.Id("saveNewPatientJob_1"); }
        public IWebElement SavePatient() { return PageLoadWait.WaitForElement(By_SavePatient(), WaitTypes.Visible); }
        public By By_AttachementContainer() { return By.ClassName("attachmentsContainer"); }
        public IWebElement AttachementContainer() { return PageLoadWait.WaitForElement(By_AttachementContainer(), WaitTypes.Visible); }
        public By By_AttachFile() { return By.CssSelector("span[id^='span_AttachFilesBtn_']"); }
        public IWebElement AttachFile() { return PageLoadWait.WaitForElement(By_AttachFile(), WaitTypes.Visible); }
        public By By_fileattachment() { return By.CssSelector("span[id^='span_AttachFilesBtn_']"); }
        public IList<IWebElement> fileattachment() { return Driver.FindElements(By_fileattachment()); }
        public By By_PatientDemographic() { return By.Id("divPatientDemographic"); }
        public IList<IWebElement> PatientDemographic() { return Driver.FindElements(By_PatientDemographic()); }
        public By By_EnterMail() { return By.CssSelector("input[id^='txt_EmailId_']"); }
        public IWebElement EnterMail() { return PageLoadWait.WaitForElement(By_EnterMail(), WaitTypes.Visible); }
        public By By_EnterPH() { return By.CssSelector("input[id^='txt_ContactNo_']"); }
        public IWebElement EnterPH() { return PageLoadWait.WaitForElement(By_EnterPH(), WaitTypes.Visible); }
        public By By_SelectCheckbox() { return By.CssSelector("input[id^='cb_']"); }
        public IWebElement SelectCheckbox() { return PageLoadWait.WaitForElement(By_SelectCheckbox(), WaitTypes.Visible); }
        public By By_Attachemntcontainer() { return By.Id("divFilesContainer"); }
        public IWebElement Attachemntcontainer() { return PageLoadWait.WaitForElement(By_Attachemntcontainer(), WaitTypes.Visible); }
        public By By_AttachNonDicomBtn() { return By.Id("btnAttachNonDicom"); }
        public IWebElement AttachNonDicomBtn() { return PageLoadWait.WaitForElement(By_AttachNonDicomBtn(), WaitTypes.Visible); }
        public By By_Job1() { return By.Id("job_1"); }
        public IWebElement Job1() { return PageLoadWait.WaitForElement(By_Job1(), WaitTypes.Visible); }
        public By By_Emailtxt() { return By.Id("txtEmail"); }
        public IWebElement ICCAemailtxt() { return PageLoadWait.WaitForElement(By_Emailtxt(), WaitTypes.Visible); }
        public By By_Passworttxt() { return By.Id("txtPassword"); }
        public IWebElement ICCaPasstxt() { return PageLoadWait.WaitForElement(By_Passworttxt(), WaitTypes.Visible); }
        public By By_LoginBt() { return By.Id("loginButton"); }
        public IWebElement ICCALoginBT() { return PageLoadWait.WaitForElement(By_LoginBt(), WaitTypes.Visible); }
        public By By_Image_sharing() { return By.XPath("//div[contains(text(),'Image Sharing')]"); }
        public IWebElement ICCAImageSharing() { return PageLoadWait.WaitForElement(By_Image_sharing(), WaitTypes.Visible); }
        public By By_ImgPatientid() { return By.Id("PatientID"); }
        public IWebElement ICCAImgPatientid() { return PageLoadWait.WaitForElement(By_ImgPatientid(), WaitTypes.Visible); }
        public By By_ICCAUploadBtn() { return By.Id("UploadBtn"); }
        public IWebElement ICCAUploadBtn() { return PageLoadWait.WaitForElement(By_ICCAUploadBtn(), WaitTypes.Visible); }
        public By By_ICCAUploaderCloseBtn() { return By.CssSelector("div[id='dialogTitleDiv']>div>input"); }
        public IWebElement ICCAUploaderCloseBtn() { return PageLoadWait.WaitForElement(By_ICCAUploaderCloseBtn(), WaitTypes.Visible); }
        public By By_ImageStudylist() { return By.Id("list"); }
        public IWebElement ICCAImgStudyLis() { return PageLoadWait.WaitForElement(By_ImageStudylist(), WaitTypes.Visible); }
        public By By_AddFilesBt(int jobID = 1) { return By.XPath("//span[@id='addFileSpan_" + jobID + "']"); }
        public IWebElement AddFilesbt(int jobID = 1) { return PageLoadWait.WaitForElement(By_AddFilesBt(jobID), WaitTypes.Visible); }
        public By By_ClearButton() { return By.XPath("//div[@id='spContainer']//following::input[@id='clearBtn']"); }
        public IWebElement ClearBut() { return PageLoadWait.WaitForElement(By_ClearButton(), WaitTypes.Visible); }
        public By By_Qryservices() { return By.XPath("//div[@id='queryServiceMessage']"); }
        public IWebElement Qryservices() { return PageLoadWait.WaitForElement(By_Qryservices(), WaitTypes.Visible); }
        public By By_Uploadfilebutton() { return By.CssSelector("input[id='divUploadFiles']"); }
        public IWebElement Uploadfilebutton() { return PageLoadWait.WaitForElement(By_Uploadfilebutton(), WaitTypes.Visible); }
        public By By_EaArchivetable() { return By.Id("tabrow"); }
        public IWebElement EaArchivetable() { return PageLoadWait.WaitForElement(By_EaArchivetable(), WaitTypes.Visible); }
        ////
        public By By_EACloudUID() { return By.CssSelector("input[name='cloudUID']"); }
        public IWebElement EACloudUID() { return PageLoadWait.WaitForElement(By_EACloudUID(), WaitTypes.Visible); }
        public By By_EaPatientID() { return By.CssSelector("input[name='PatientID']"); }
        public IWebElement EaPatientID() { return PageLoadWait.WaitForElement(By_EaPatientID(), WaitTypes.Visible); }
        public By By_EaAccessionNo() { return By.CssSelector("input[name='accessionNumber']"); }
        public IWebElement EaAccessionNo() { return PageLoadWait.WaitForElement(By_EaAccessionNo(), WaitTypes.Visible); }
        public By By_EaIssuerID() { return By.CssSelector("input[name='issuerOfPatientID']"); }
        public IWebElement EaIssuerID() { return PageLoadWait.WaitForElement(By_EaIssuerID(), WaitTypes.Visible); }
        public By By_EaDeptName() { return By.CssSelector("input[name='departmentName']"); }
        public IWebElement EaDeptName() { return PageLoadWait.WaitForElement(By_EaDeptName(), WaitTypes.Visible); }
        public By By_EaStudyInstanceID() { return By.CssSelector("input[name='studyInstanceUID']"); }
        public IWebElement EaStudyInstanceID() { return PageLoadWait.WaitForElement(By_EaStudyInstanceID(), WaitTypes.Visible); }
        public By By_EaSeriesInstanceID() { return By.CssSelector("input[name='seriesInstanceUID']"); }
        public IWebElement EaSeriesInstanceID() { return PageLoadWait.WaitForElement(By_EaSeriesInstanceID(), WaitTypes.Visible); }
        public By By_EaInstitutionName() { return By.CssSelector("input[name='institutionName']"); }
        public IWebElement EaInstitutionName() { return PageLoadWait.WaitForElement(By_EaSeriesInstanceID(), WaitTypes.Visible); }
        public By By_EaSOPId() { return By.CssSelector("input[name='sopInstanceUID']"); }
        public IWebElement EaSOPId() { return PageLoadWait.WaitForElement(By_EaSOPId(), WaitTypes.Visible); }

        //
     //   public By By_icacrossbt() { return By.XPath(".//div[@id='reviewToolbar']//following::li[@id='71']/a/img"); }
        public By By_icacrossbt() { return By.XPath(".//div[@id='reviewToolbar']//following::li[@itag='closebrowsertab']/a/img"); }
     
        public IWebElement icacrossbt() { return PageLoadWait.WaitForElement(By_icacrossbt(), WaitTypes.Visible); }


        public By By_Activitylist() { return By.CssSelector("img[alt='Activity List']"); }
        public IWebElement ICCAActivitylist() { return PageLoadWait.WaitForElement(By_Activitylist(), WaitTypes.Visible); }
        public By By_ImageSharing() { return By.ClassName("module_add_feature_alt"); }
        public IWebElement ICCAImagesharingTab() { return PageLoadWait.WaitForElement(By_ImageSharing(), WaitTypes.Visible); }
        public By By_ViewButton() { return (By.Id("bViewBtn")); }
        public IWebElement ICCAViewButton() { return PageLoadWait.WaitForElement(By_ViewButton(), WaitTypes.Visible); }
        public By By_EASubmitButton() { return By.Id("submitbutton"); }
        public IWebElement EASubmitButton() { return PageLoadWait.WaitForElement(By_EASubmitButton(), WaitTypes.Visible); }

        public By By_ICCASearchBtn() { return By.Id("searchBtn"); }
        public IWebElement ICCASearchBtn() { return PageLoadWait.WaitForElement(By_ICCASearchBtn(), WaitTypes.Visible); }
        //   public By By_ICCAPatientCheckbox() { return By.ClassName("cbox"); }
        // public IWebElement ICCAPatientCheckbox() { return PageLoadWait.WaitForElement(By_ICCAPatientCheckbox(), WaitTypes.Visible); }
        public By By_ICCAPatientCheckbox() { return By.XPath("//div[@id='jqgh_list_cb']/input[@id='cb_list']"); }
        public IWebElement ICCAPatientCheckbox() { return PageLoadWait.WaitForElement(By_ICCAPatientCheckbox(), WaitTypes.Visible); }
        public By By_StudyinICAViewer() { return By.CssSelector("#m_studyPanels_m_studyPanel_1_PatientBannerControl_patientBannerInfoDiv"); }
        public IWebElement StudyBanner() { return PageLoadWait.WaitForElement(By_StudyinICAViewer(), WaitTypes.Visible); }
        public By By_ICCAActivityLog() { return By.ClassName("activity_log"); }
        public IList<IWebElement> ICCAActivityLog() { return Driver.FindElements(By_ICCAActivityLog()); }
        public By By_RefreshBtn() { return By.CssSelector("#RefreshBtn"); }
        public IWebElement RefreshBtn() { return PageLoadWait.WaitForElement(By_RefreshBtn(), WaitTypes.Visible); }

        //by ravsoft ends here

        //
        #endregion Webelements

        #region ReusableComponents_HelperMethods

        /// <summary>
        /// Login to HTML5 Uploader either as registered or guest user
        /// </summary>
        /// <param name="username">Provide Username</param>
        /// <param name="password">Provide Password</param>
        /// <param name="email">Provide email address</param>
        /// <param name="phonenumber">Provide Phone Number</param>
        /// <param name="HippaComplianceSelection">optional paramter if Hippa compliance checkbox needs to selected as part of Login</param>
        public void Login_HTML5Uploader(string username = "", string password = "", string email = "", string phonenumber = "", bool HippaComplianceSelection = true, string DomainName = "SuperAdminGroup")
        {
            try
            {
                //Login as registered user
                if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
                {
                    RegisteredUserRadioBtn().Click();
                    UserNameTxtBox().Clear();
                    UserNameTxtBox().SendKeys(username);
                    PasswordTxtBox().Clear();
                    PasswordTxtBox().SendKeys(password);
                    SignInBtn().Click();
                    try
                    {
                        if (DomainSelectorPopup().Displayed)
                        {
                            DomainSelectorDropdown().SelectByText(DomainName);
                            DomainSelectorGoBtn().Click();
                        }
                    }
                    catch (Exception ex) { }
                    Driver.SwitchTo().DefaultContent(); Driver.SwitchTo().Frame(0);
                    Logger.Instance.InfoLog("Successfully Signed into HTML5 Uploader using registered user");
                    //if (HippaComplianceLabel().Displayed)
                    //{
                    //    Logger.Instance.InfoLog("Successfully Signed into HTML5 Uploader using registered user");
                    //}
                }
                //Login as guest user
                else if (!String.IsNullOrEmpty(email) && !String.IsNullOrEmpty(phonenumber))
                {
                    GuestUserRadioBtn().Click();
                    EmailTxtBox().Clear();
                    EmailTxtBox().SendKeys(email);
                    PhoneTxtBox().Clear();
                    PhoneTxtBox().SendKeys(phonenumber);
                    SignInBtn().Click();
                    try
                    {
                        if (DomainSelectorPopup().Displayed)
                        {
                            DomainSelectorDropdown().SelectByText(DomainName);
                            DomainSelectorGoBtn().Click();
                        }
                    }
                    catch (Exception ex) { }
                    Driver.SwitchTo().DefaultContent(); Driver.SwitchTo().Frame(0);
                    Logger.Instance.InfoLog("Successfully Signed into HTML5 Uploader using using email and phone number");
                    //if (HippaComplianceLabel().Displayed)
                    //{
                    //    Logger.Instance.InfoLog("Successfully Signed into HTML5 Uploader using email and phone number");
                    //}
                }
                else
                {
                    Logger.Instance.ErrorLog("Username & Password or Email & Phonenumber were not provided for logging in");
                }
                // If Hippa Compliance selection to be done
                if (HippaComplianceSelection)
                {
                    if (!HippaAgreeChkBox().Selected)
                    {
                        HippaAgreeChkBox().Click();
                    }
                    PageLoadWait.WaitForElement(By_HippaContinueBtn(), WaitTypes.Clickable);
                    HippaContinueBtn().Click();
                    Logger.Instance.InfoLog("Hippa Compliance checkbox selected and continued");
                    //if (UploadFilesBtn().Displayed)
                    //{
                    //    Logger.Instance.InfoLog("Hippa Compliance checkbox selected and continued");
                    //}
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Unable to login to HTML5 Uploader due to: " + ex);
                throw ex;
            }
        }

        /// <summary>
        /// Logout of HTML5 Uploader 
        /// </summary>
        public void Logout_HTML5Uploader()
        {
            string username = UsernameDisplayed().Text;
            SignOutBtn().Click();
            Logger.Instance.InfoLog("Successfully logged out of HTML5 Uploader as user: " + username);
            PageLoadWait.WaitForFrameLoad(10);
        }

        /// <summary>
        /// This method only selects the attachment checkbox while attaching non-dicom files to studies
        /// </summary>
        /// <param name="attachmentfilename"></param>
        public void SelectAttachmentForPatient(string attachmentfilename)
        {
            int count = 0;
            //Finding location of attachment
            foreach (IWebElement label in AttachmentLabels())
            {
                if (label.Text == attachmentfilename)
                {
                    break;
                }
                count++;
            }
            //Clicking checkbox
            if (!AttachmentCheckboxes()[count].Selected)
            {
                AttachmentCheckboxes()[count].Click();
            }
            Logger.Instance.InfoLog("Successfully selected checkbox with label: " + attachmentfilename);
        }

        //by ravsoft starts here 

        /// <summary>
        ///to check the uploader files in temp folder
        /// </summary>
        public string CheckTempFolder(string sopvalues = "")
        {
            string bFileName = "";
            try
            {
                bool lflag = false;
                string splittedfilename = "";
                string[] files = Directory.GetFiles(Config.HTML5UploaderAcceptedPath, "*.*", SearchOption.AllDirectories);
                foreach (string filelist in files)
                {
                    if (filelist != "")
                    {
                        string[] splitfilename = filelist.Split('\\');
                        int lsplitlen = splitfilename.Length;
                        splittedfilename = splitfilename[lsplitlen - 1];
                        if (sopvalues != "")
                        {
                            if (sopvalues.Equals(splittedfilename))
                            {
                                lflag = true;
                                break;

                            }
                        }
                    }
                }


                if (sopvalues != "")
                {
                    if (lflag == true)
                    {
                        return splittedfilename;
                    }
                }
                else
                {
                    return splittedfilename;
                }


            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Unable to login to HTML5 Uploader due to: " + ex);
                throw ex;
            }
            return bFileName;
        }
        /// <summary>
        ///delete the temp folder before upload the files  
        /// </summary>
        public void DeleteTempfiles(string sfilepath)
        {
            try
            {
                DirectoryInfo tempfiles = new DirectoryInfo(sfilepath);
                foreach (DirectoryInfo dir in tempfiles.GetDirectories())
                    if (Directory.Exists(dir.FullName))
                        Directory.Delete(dir.FullName, true);
                    else
                        throw new SystemException("Directory you want to delete is not exist");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Unable to delete the temp folder " + ex);
            }
        }

        public string FilesfromFolder(string UploadFilePath)
        {


            string splittedfilename = "";
           // string[] files = null;
            string[] files = Directory.GetFiles(UploadFilePath, "*.*", SearchOption.AllDirectories);
            //if (UploadFilePath.IndexOf("Windows") >1)
            //{
            //    files = Directory.GetFiles( UploadFilePath, "*.*", SearchOption.AllDirectories);
            //}
            //else
            //{
            //    files = Directory.GetFiles(Config.TestDataPath + UploadFilePath, "*.*", SearchOption.AllDirectories);
            //}
                 

            foreach (string filelist in files)
            {
                if (filelist != "")
                {
                    string[] splitfilename = filelist.Split('\\');
                    int lsplitlen = splitfilename.Length;
                    splittedfilename = splitfilename[lsplitlen - 1];
                    break;
                }
            }

            return splittedfilename;
        }
        //public void DeleteDataICCA()
        //{
        //    Login login = new Login();
        //    string ICCAURL = "https://" + Config.ICCAHOSTNAME + ".merge.com";
        //    BasePage.Driver.Quit();
        //    BasePage.Driver = null;
        //    //   login.InvokeBrowser(Config.BrowserType);
        //    login.InvokeBrowser("chrome");
        //    Driver.Navigate().GoToUrl(ICCAURL);
        //    PageLoadWait.WaitForPageLoad(40);
        //    IWebElement EnterEmailiD = ICCAemailtxt();
        //    EnterEmailiD.SendKeys(Config.ICCAUser);
        //    IWebElement Enterpassword = ICCaPasstxt();
        //    Enterpassword.SendKeys(Config.ICCAPAssword);
        //    IWebElement ClickLoginButton = ICCALoginBT();
        //    ClickLoginButton.Click();

        //    Thread.Sleep(10000);
        //    IWebElement imagesharing = ICCAImageSharing();
        //    imagesharing.Click();
        //    PageLoadWait.WaitForPageLoad(5000);
        //    try
        //    {
        //        IWebElement norecords1 = Qryservices();



        //    }

        //    catch
        //    {
        //        IWebElement checkbox32 = ICCAPatientCheckbox();

        //        if (checkbox32.Displayed == true)
        //        {
        //            checkbox32.Click();
        //            Thread.Sleep(20000);
        //            if (checkbox32.Selected == false)
        //            {
        //                checkbox32.Click();
        //            }
        //        }


        //        //select the delete key
        //        IWebElement idelete = Driver.FindElement(By.Id("jqlistbuttonRemoveAll"));
        //        if (idelete.Displayed == true)
        //        {
        //            idelete.Click();
        //            Thread.Sleep(10000);
        //            IWebElement iconfirmwindow = Driver.FindElement(By.Id("DialogDiv"));
        //            if (iconfirmwindow.Displayed == true)
        //            {
        //                IWebElement iConfirmBt = Driver.FindElement(By.Id("btnSubmit"));
        //                if (iConfirmBt.Displayed == true)
        //                {
        //                    iConfirmBt.Click();
        //                    Thread.Sleep(10000);
        //                    IWebElement iCloseStudy = Driver.FindElement(By.Id("DialogDiv"));
        //                    if (iCloseStudy.Displayed == true)
        //                    {
        //                        IWebElement iCloseBt = Driver.FindElement(By.Id("btnClose"));
        //                        iCloseBt.Click();

        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        //by ravsoft ends here
        #endregion ReusableComponents_HelperMethods

    }
}
