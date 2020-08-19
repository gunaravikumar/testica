/*
This Java Script function will retrive all the Report data
Currently Writeen for PDF Report can be exanded
*/
function getReportData(farme_selector, obj_selector, priorcount)
{

console.log("getReportData--JSScript Execution started");
var reportdata=[];
var iframe  = document.getElementById("UserHomeFrame");
var innerdoc = (iframe.contentDocument) ? iframe.contentDocument : iframe.contentWindow.document;
var reportdom_1  = innerdoc.querySelectorAll(farme_selector)[priorcount].contentWindow.document;
var reportdom = reportdom_1.querySelector(obj_selector).contentWindow.document;
var patient  = reportdom.getElementById("patient");
var exam  = reportdom.getElementById("exam");
var report  = reportdom.getElementById("report1");
var prows = patient.querySelectorAll("table tbody tr");
var examrows = exam.querySelectorAll("table tbody tr");
var reportrows = report.querySelectorAll("table tbody tr");
var report_title = report.querySelector("div#report_text1>h3").textContent;
var report_descs = report.querySelectorAll("p");

//Get all patient info
for (var iterate=0;iterate<prows.length;iterate++) 
{    
    var columnname = prows[iterate].querySelectorAll("td")[0].textContent;	
	var columnvalue = prows[iterate].querySelectorAll("td")[1].textContent;	    
	var data = columnname+"="+columnvalue;
	reportdata.push(data);  
}

//Get all Study info
for (var iterate=0;iterate<examrows.length;iterate++) 
{   
    var columnname = examrows[iterate].querySelectorAll("td")[0].textContent;
	var columnvalue = examrows[iterate].querySelectorAll("td")[1].textContent;
	var data = columnname+"="+columnvalue;
	reportdata.push(data); 
}

//Get report info
for (var iterate=0;iterate<reportrows.length;iterate++) 
{   
    var columnname = reportrows[iterate].querySelectorAll("td")[0].textContent;
	var columnvalue = reportrows[iterate].querySelectorAll("td")[1].textContent;
	var data = columnname+"="+columnvalue;
	reportdata.push(data); 
}

//Get report title
var title  = "Report Title"+"="+report_title;
reportdata.push(title);

//Get report description
for (var iterate=0;iterate<report_descs.length;iterate++) 
{   
    var description = ""; 
    description  = description + report_descs[iterate].textContent;
	description.replace("<br>", "\n");
	description.replace("<bbr>", "\n");
}
reportdata.push("Description"+"="+description);


console.log("getReportData--JSScript Execution ended");
return reportdata;

};