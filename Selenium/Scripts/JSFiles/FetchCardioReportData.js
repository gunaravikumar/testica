/* This Java Script function will retrive all the Cardio Report data */
function getCardioReportData()
{

console.log("getCardioReportData--JSScript Execution started");
var ReportData = {};

var userHomeFrame = document.querySelector('iframe#UserHomeFrame')
var userHomeDocument = (userHomeFrame.contentDocument) ? userHomeFrame.contentDocument : userHomeFrame.contentWindow.document;

var outerFrame = userHomeDocument.querySelector('iframe#reportIframe')
var outerDocument = (outerFrame.contentDocument) ? outerFrame.contentDocument : outerFrame.contentWindow.document

var innerFrame = outerDocument.querySelector('iframe#pdfIframe')
var innerDocument = (innerFrame.contentDocument) ? innerFrame.contentDocument : innerFrame.contentWindow.document;
var pdfSvg = innerDocument.querySelectorAll('svg')
var pdfContent = pdfSvg[0].textContent

//Get the patient name
var patientLableExcept = pdfContent.split('Patient:')[1];
var betweenMRN = patientLableExcept.split('MRN:')[0];
var PatientName = betweenMRN.split('Study')[0].split('').join(' ');
ReportData.Patient = PatientName;

//Get MRN of the Patient
var MRN = patientLableExcept.split('Location')[0].split('MRN:')[1].split('').join(' ');
ReportData.MRN = MRN;

//Get Patient DOB
var betweenReferring = patientLableExcept.split('REFERRING')[0];
var ageDOB = betweenReferring.split('DOB:')[1].split('Gender:')[0].split('').join(' ');
var DOB = ageDOB.split('(')[0];
ReportData.DOB = DOB;

//Get Patient Gender
var Gender = betweenReferring.split('DOB:')[1].split('Gender:')[1];
ReportData.Gender = Gender;

console.log("getCardioReportData--JSScript Execution ended");
return ReportData;

};