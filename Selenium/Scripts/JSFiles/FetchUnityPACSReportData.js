/* This Java Script function will retrive all the UnityPACS Report data */
function getUnityPACSReportData()
{

console.log("getUnityPACSReportData--JSScript Execution started");
var ReportData = {};

var userHomeFrame = document.querySelector('iframe#UserHomeFrame')
var userHomeDocument = (userHomeFrame.contentDocument) ? userHomeFrame.contentDocument : userHomeFrame.contentWindow.document;

var outerFrame = userHomeDocument.querySelector('iframe#reportIframe')
var outerDocument = (outerFrame.contentDocument) ? outerFrame.contentDocument : outerFrame.contentWindow.document

var innerFrame = outerDocument.querySelector('iframe#textDisplay')
var innerDocument = (innerFrame.contentDocument) ? innerFrame.contentDocument : innerFrame.contentWindow.document;
var pdftext = innerDocument.querySelectorAll('span.awspan.awtext2')
var valueslist = []
for(var i = 0; i < pdftext.length; i++){
valueslist.push(pdftext[i].textContent)
}
var pdfcontent = valueslist.join(' ')

//Get the patient name
var PatientName = pdfcontent.split('Patient  Name:   ')[1].split(' Exam')[0];
ReportData.Patient = PatientName;

//Get MRN of the Patient
var MRN = pdfcontent.split('Patient  I.D.#:   ')[1].split(' Birth')[0];
ReportData.MRN = MRN;

//Get Patient DOB
var DOB = pdfcontent.split('Birth  date:   ')[1].split(' Age:   ')[0]
ReportData.DOB = DOB;

//Get Patient Gender
var Gender = pdfcontent.split('Sex:   ')[1].split(' Referring  Doctor:')[0]
ReportData.Gender = Gender;

console.log("getCardioReportData--JSScript Execution ended");
return ReportData;

};