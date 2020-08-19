/*-----------------------------------------------------------------------------------------------------------------------
This library contains the below listed class and functions.
  - Info(strMessage): Logs the given message in Info level
  - Success(strMessage): Logs the given message in Success level
  - Error(strMessage, objException): Logs the given message in Error level
  - Warning(strMessage): Logs the given message in Warning level
  - captureScreenshot(): Captures the screenshot and saves it as image
  - RGB(intRed, intGreen, intBlue): returns the RGB color code value
-----------------------------------------------------------------------------------------------------------------------*/

var LogAttr;
var CurrentResultScreenshotFolderPath;
var pmNormal = 300;

/*-----------------------------------------------------------------------------------------------------------------------
Function: LogAttr
Purpose:
  To return the current value of LogAttr
-----------------------------------------------------------------------------------------------------------------------*/
function LogAttr(){
  return LogAttr;
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: CurrentResultScreenshotFolderPath
Purpose:
  To return the current result screenshot folder absolute path
-----------------------------------------------------------------------------------------------------------------------*/
function CurrentResultScreenshotFolderPath(){
  return CurrentResultScreenshotFolderPath;
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: Picture
Purpose:
  To Log the given picture object
Parameter:
  objPicture: The picture object
  strPictMessage: The string message to be logged
-----------------------------------------------------------------------------------------------------------------------*/
function Picture(objPicture, strPictMessage){
  Log.picture(objPicture, strPictMessage, "Screenshot of the object for reference");
}


/*-----------------------------------------------------------------------------------------------------------------------
Function: setCurrentResultScreenshotFolderPath
Purpose:
  To set the value for CurrentResultScreenshotFolderPath
Parameter:
  strFolderPath: current result screenshot folder absolute path
-----------------------------------------------------------------------------------------------------------------------*/
function setCurrentResultScreenshotFolderPath(strFolderPath){
  CurrentResultScreenshotFolderPath = strFolderPath;
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: Info
Purpose:
  To Log the given message in Info level
Parameter:
  strMessage: string message to be logged
-----------------------------------------------------------------------------------------------------------------------*/
function Info(strMessage){
  Log.Message("Info : " + strMessage);
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: Success
Purpose:
  To Log the given message in Success level
Parameter:
  strMessage: string message to be logged
-----------------------------------------------------------------------------------------------------------------------*/
function Success(strMessage){
  LogAttr = Log.CreateNewAttributes();
  LogAttr.FontColor = RGB(0, 0, 0);
  LogAttr.BackColor = RGB(204, 255, 153);
  Log.Message("Success : " + strMessage, "", pmNormal, LogAttr);
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: Error
Purpose:
  To Log the given message in Error level
Parameters:
  strMessage: string message
  objException: exception object
-----------------------------------------------------------------------------------------------------------------------*/
function Error(strMessage, objException, objTestMethodException){
  LogAttr = Log.CreateNewAttributes();
  LogAttr.FontColor = RGB(0, 0, 0);
  LogAttr.BackColor = RGB(255, 117, 117);


  if(objException.ErrorExists){
    Log.Error("Exception caught : " + strMessage, "", pmNormal, LogAttr);

    if (objTestMethodException != undefined)
      objException.StepLog = objTestMethodException.StepLog;
    
    return objException;
  }
  else{
    var enumReturn = getReturnEnum();
    Log.Error("Exception caught : " + strMessage, "", pmNormal, LogAttr);
    Log.Error("\t\tDescription : " + objException.description, "", pmNormal, LogAttr);
    Log.Error("\t\tMessage : " + objException.message, "", pmNormal, LogAttr);
    enumReturn.ErrorExists = true;
    enumReturn.Description = strMessage + " : " + objException.description;
    enumReturn.Message = objException.message;
    enumReturn.Screenshot = captureScreenshot();

    if (objTestMethodException != undefined)
      enumReturn.StepLog = objTestMethodException.StepLog;
    
    return enumReturn;
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: Warning
Purpose:
  To Log the given message in Warning level
Parameter:
  strMessage: string message to be logged
-----------------------------------------------------------------------------------------------------------------------*/
function Warning(strMessage){
  LogAttr = Log.CreateNewAttributes();
  LogAttr.FontColor = RGB(0, 0, 0);
  LogAttr.BackColor = RGB(255, 255, 153);
  Log.Warning("Warning : " + strMessage, "", pmNormal, LogAttr);
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: Warning
Purpose:
  To return a new enumerator object
Return Value:
  enumReturn: enumerator object
-----------------------------------------------------------------------------------------------------------------------*/
function getReturnEnum(){
  var enumReturn = {
    PassCount: 0,
    TestDataCount: 0,
    Duration: "",
    ErrorExists: false,
    Description: "",
    Message: "",
    Screenshot: "",
    StepLog: []
  };
  return enumReturn;
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: captureScreenshot
Purpose:
  To capture the screenshot and save it as image
Return Value:
  Realtive path of the saved image
-----------------------------------------------------------------------------------------------------------------------*/
function captureScreenshot(){
  try{
    var strImageFileName = aqConvert.DateTimeToFormatStr(aqDateTime.Now(), "%B_%d_%Y_%H_%M_%S") + ".jpg"
    var strImagePath = CurrentResultScreenshotFolderPath + "\\"+ strImageFileName;
    Sys.Desktop.Picture().SaveToFile(strImagePath);
    return "Screenshot\\" + strImageFileName;
  }
  catch(ex)
  { Error("Error while capturing screenshot ", ex); }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: RGB
Purpose:
  To return the RGB color code value
Return Value:
  RGB color code
-----------------------------------------------------------------------------------------------------------------------*/
function RGB(intRed, intGreen, intBlue){   
  return intRed | (intGreen << 8) | (intBlue << 16);
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: logScreenshot
Purpose:
  To add screenshot for the given steps number in the custom report
Return Value:
  enumLogger: enumerator holding the logger return object
-----------------------------------------------------------------------------------------------------------------------*/
function logScreenshot(enumLogger, intStepNumber, strLogMessage, strImageFilePath) {
  try {
    var enumStepLog = {
      Screenshot: "",
      StepNumber: 0,
      Message: ""
    };
    
    if (strImageFilePath == undefined) {
      enumStepLog.Screenshot = captureScreenshot();
    } else {
      enumStepLog.Screenshot = strImageFilePath;
    }
    enumStepLog.StepNumber = intStepNumber;
    if (strLogMessage != undefined) enumStepLog.Message = strLogMessage;
    enumLogger.StepLog.push(enumStepLog);
    return enumLogger;
  }
  catch (ex) {
    Error("Error while adding screenshot to custom report", ex);
  }
}