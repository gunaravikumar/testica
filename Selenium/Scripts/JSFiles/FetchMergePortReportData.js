/*
This Java Script function will retrive all the Report data
Currently Written for SR Report which is sent by Merge Port
*/
function getMergePortHL7ReportData(farme_selector, obj_selector, priorCount)
{

console.log("getReportData--JSScript Execution started");
//var reportdata=[];
if (priorCount == undefined) priorCount = 0;
//var  reportData = {"Table":'',"InnerText":''}
var reportData = "";
var iframe  = document.getElementById("UserHomeFrame");
var innerdoc = (iframe.contentDocument) ? iframe.contentDocument : iframe.contentWindow.document;
var reportdom_1  = innerdoc.querySelectorAll(farme_selector)[priorCount].contentDocument;
var reportdom = reportdom_1.querySelector(obj_selector).contentDocument;

//Get Report Table 
var reportTable = reportdom.querySelector("form[name='pageElements'] tr>td>table");
//reportData.Table = reportTable;

//Get Conetent  of the report
var reportData = reportTable.querySelector("span:not([class])").innerText;
console.log("getReportData--JSScript Execution ended");

return reportData;
};
