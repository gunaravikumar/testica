//USEUNIT GenericUtils
//USEUNIT ICAActionsXpath

/*-----------------------------------------------------------------------------------------------------------------------
Function: clickOnLastMouseCoOrdinates
Purpose:
  To perform click on the last mouse known coordinates
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function clickOnLastMouseCoOrdinates() {
  try {
    ICAActionsXpath.clickOnLastMouseCoOrdinates();
  }
  catch(ex) {
    Log.Error("Error while performing click on the last known mouse coordinates", ex)
  }
  finally {
    SavingLogToDisk("clickOnLastMouseCoOrdinates" + getCurrentDateTime());
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: click
Purpose:
  To perform click on the given element
Parameters:
  strXpath: Xpath of an element
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function click() {
  try {
    if (BuiltIn.ParamCount() < 10)
      throw new Error("No parameter passed to the Click function");
      
    var strXpath = BuiltIn.ParamStr(10);
    Log.Message("XPath -- " + strXpath);
    if (strXpath == null || strXpath == "" || aqString.Find(strXpath, "//") == -1)
      throw new Error("Invalid XPath string");

    ICAActionsXpath.click(strXpath);
    Log.Message("Given XPath Clicked successfully"); 
  }
  catch(ex) {
    Log.Error("Error while performing click on the given xpath", ex)
  }
  finally {
    SavingLogToDisk("click" + getCurrentDateTime());
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: contextClickOnLastMouseCoOrdinates
Purpose:
  To perform right click on the last mouse known coordinates
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function contextClickOnLastMouseCoOrdinates() {
  try {
    ICAActionsXpath.contextClickOnLastMouseCoOrdinates();
  }
  catch(ex) {
    Log.Error("Error while performing right click on Last Mouse CoOrdinates", ex)
  }
  finally {
    SavingLogToDisk("contextClickOnLastMouseCoOrdinates" + getCurrentDateTime());
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: contextClick
Purpose:
  To perform right click on the element 
Parameters:
  strXpath: Xpath of an element
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function contextClick() {
  try {
   if (BuiltIn.ParamCount() < 10)
      throw new Error("No parameter passed to the contextClick function");
      
    var strXpath = BuiltIn.ParamStr(10);
    Log.Message("XPath -- " + strXpath);
    if (strXpath == null || strXpath == "" || aqString.Find(strXpath, "//") == -1)
      throw new Error("Invalid XPath string");
        
    ICAActionsXpath.contextClick(strXpath);
  }
  catch(ex) {
    Log.Error("Error while performing right click on the element", ex);
  }
  finally {
    SavingLogToDisk("contextClick" + getCurrentDateTime());
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: DoubleClickOnLastMouseCoOrdinates
Purpose:
  To perform double click on the last mouse known coordinates
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function DoubleClickOnLastMouseCoOrdinates() {
  try {
    ICAActionsXpath.DoubleClickOnLastMouseCoOrdinates();
  }
  catch(ex) {
    Log.Error("Error while performing double click on the last known mouse coordinates", ex);
  }
  finally {
    SavingLogToDisk("DoubleClickOnLastMouseCoOrdinates" + getCurrentDateTime());
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: DoubleClick
Purpose:
  To perform double click on the element
Parameters:
  strXpath: Xpath of an element
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function DoubleClick() {
  try {
    if (BuiltIn.ParamCount() < 10)
      throw new Error("No parameter passed to the DoubleClick function");
      
    var strXpath = BuiltIn.ParamStr(10);
    Log.Message("XPath -- " + strXpath);
    if (strXpath == null || strXpath == "" || aqString.Find(strXpath, "//") == -1)
      throw new Error("Invalid XPath string");
     
    ICAActionsXpath.DoubleClick(strXpath);  
  }
  catch(ex) {
    Log.Error("Error while performing double click on the element", ex);
  }
  finally {
    SavingLogToDisk("DoubleClick"  + getCurrentDateTime());
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: DragAndDrop
Purpose:
  To perform drag and drop from one element to another element
Parameters:
  strImageXpath: Xpath of source element
  strDestinationTableXpath: Xpath of destination element
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function DragAndDrop() {
  try {
	  if (BuiltIn.ParamCount() < 10)
      throw new Error("No parameter passed to the DragAndDrop function");
      
    var strImageXpath = BuiltIn.ParamStr(10);
    if (strImageXpath == null || strImageXpath == "" || aqString.Find(strImageXpath, "//") == -1)
      throw new Error("Invalid Source XPath string");

    var strDestinationTableXpath = BuiltIn.ParamStr(11);
    if (strDestinationTableXpath == null || strDestinationTableXpath == "" || aqString.Find(strDestinationTableXpath, "//") == -1)
      throw new Error("Invalid Destination XPath string");
    
    ICAActionsXpath.DragAndDrop(strImageXpath, strDestinationTableXpath);
  }
  catch(ex) {
    Log.Error("Error while performing drag an element to another element", ex);
  }
  finally {
    SavingLogToDisk("DragAndDrop" + getCurrentDateTime());
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: KeyDown
Purpose:
  To Sends a modifier key down message to the browser
Parameters:
  strString: value of the key to press
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function KeyDown() {
  try {
    if (BuiltIn.ParamCount() < 10)
      throw new Error("No parameter passed to the KeyDown function");
      
    var strKey = BuiltIn.ParamStr(10);
    if (strKey == null || strKey == "")
      throw new Error("Invalid Key to press - " + strKey);

    ICAActionsXpath.KeyDown(strKey);
  }
  catch(ex) {
    Log.Error("Error while sending a modifier key up message to the browser", ex);
  }
  finally {
    SavingLogToDisk("KeyDown" + getCurrentDateTime());
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: KeyUp
Purpose:
  To Sends a modifier key up message to the browser.
Parameters:
  strString: value of the key to press
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function KeyUp() {
  try {
    if (BuiltIn.ParamCount() < 10)
      throw new Error("No parameter passed to the KeyUp function");
      
    var strKey = BuiltIn.ParamStr(10);
    if (strKey == null || strKey == "")
      throw new Error("Invalid Key to release " + strKey);

    ICAActionsXpath.KeyUp(strKey);
  }
  catch(ex) {
    Log.Error("Error while sending a modifier key to release message to the browser", ex);
  }
  finally {
    SavingLogToDisk("KeyUp" + getCurrentDateTime());
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: KeyDownElement
Purpose:
  To Sends a modifier key down message to the specified element in the browser.
Parameters:
  strXpath: Xpath of the element
  strString: value of the key to press
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function KeyDownElement() {
  try {
    if (BuiltIn.ParamCount() < 10)
      throw new Error("No parameter passed to the KeyDownElement function");
      
    var strXpath = BuiltIn.ParamStr(10);
    if (strXpath == null || strXpath == "" || aqString.Find(strXpath, "//") == -1)
      throw new Error("Invalid XPath string -- " + strXpath);
     
    var strKey = BuiltIn.ParamStr(11);
    if (strKey == null || strKey == "")
      throw new Error("Invalid Key to press -- " + strKey);
    
    ICAActionsXpath.KeyDownElement(strXpath, strKey);
  }
  catch(ex) {
    Log.Error("Error while sending a modifier up down message to the specified element in the browser", ex);
  }
  finally {
    SavingLogToDisk("KeyDownElement" + getCurrentDateTime());
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: KeyUpElement
Purpose:
  To Sends a modifier key up message to the specified element in the browser.
Parameters:
  strXpath: Xpath of the element
  strString: value of the key to press
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function KeyUpElement() {
  try {
    if (BuiltIn.ParamCount() < 10)
      throw new Error("No parameter passed to the KeyUpElement function");
      
    var strXpath = BuiltIn.ParamStr(10);
    if (strXpath == null || strXpath == "" || aqString.Find(strXpath, "//") == -1)
      throw new Error("Invalid XPath string");
     
    var strKey = BuiltIn.ParamStr(11);
    if (strKey == null || strKey == "")
      throw new Error("Invalid Key to release -- " + strKey);
    
    ICAActionsXpath.KeyUpElement(strXpath, strKey);
  }
  catch(ex) {
    Log.Error("Error while sending a modifier up down message to the specified element in the browser", ex);
  }
  finally {
    SavingLogToDisk("KeyUpElement" + getCurrentDateTime());
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: clickAndHoldLastMouseCoOrdinates
Purpose:
  To perform click and hold on the last mouse known coordinates
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function clickAndHoldLastMouseCoOrdinates() {
  try {
    ICAActionsXpath.clickAndHoldLastMouseCoOrdinates();
  }
  catch(ex) {
    Log.Error("Error while performing click on the last known mouse coordinates", ex);
  }
  finally {
    SavingLogToDisk("clickAndHoldLastMouseCoOrdinates" + getCurrentDateTime());
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: releaseLastMouseCoOrdinates
Purpose:
  To perform release of the mouse on the last mouse known coordinates
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function releaseLastMouseCoOrdinates() {
  try {
    ICAActionsXpath.releaseLastMouseCoOrdinates();
  }
  catch(ex) {
    Log.Error("Error while performing click on the last known mouse coordinates", ex);
  }
  finally {
    SavingLogToDisk("releaseLastMouseCoOrdinates" + getCurrentDateTime());
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: clickAndHold
Purpose:
  To perform click and hold on the particular element
Parameters:
  strXpath: Xpath of an element
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function clickAndHold() {
  try {
    if (BuiltIn.ParamCount() < 10)
      throw new Error("No parameter passed to the clickAndHold function");
      
    var strXpath = BuiltIn.ParamStr(10);
    if (strXpath == null || strXpath == "" || aqString.Find(strXpath, "//") == -1)
      throw new Error("Invalid XPath string");
    
    ICAActionsXpath.clickAndHold(strXpath);
  }
  catch(ex) {
    Log.Error("Error while performing click on the last known mouse coordinates", ex);
  }
  finally {
    SavingLogToDisk("clickAndHold" + getCurrentDateTime());
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: release
Purpose:
  To perform release of the mouse on the particular element
Parameters:
  strXpath: Xpath of an element
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function release() {
  try {
    if (BuiltIn.ParamCount() < 10)
      throw new Error("No parameter passed to the release function");
      
    var strXpath = BuiltIn.ParamStr(10);
    if (strXpath == null || strXpath == "" || aqString.Find(strXpath, "//") == -1)
      throw new Error("Invalid XPath string");
    
    ICAActionsXpath.clickAndHold(strXpath);
  }
  catch(ex) {
    Log.Error("Error while releasing click on the last known mouse coordinates", ex);
  }
  finally {
    SavingLogToDisk("release" + getCurrentDateTime());
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: MoveByOffset
Purpose:
  To Moves the mouse to the specified offset of the last known mouse coordinates.
Parameters:
  intOffsetx: X-coordinates
  intOffsety: y-coordinates
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function MoveByOffset() {
  try {
    if (BuiltIn.ParamCount() < 10)
      throw new Error("No parameter passed to the MoveByOffset function");
      
    var strXOffset = BuiltIn.ParamStr(10);
    if (strXOffset == null || strXOffset == "")
      throw new Error("Invalid X-Offset position " + strXOffset);
     
    var strYOffset = BuiltIn.ParamStr(11);
    if (strYOffset == null || strYOffset == "")
      throw new Error("Invalid Y-Offset position " + strYOffset);
    
    ICAActionsXpath.MoveByOffset(aqConvert.VarToInt(strXOffset), aqConvert.VarToInt(strYOffset));
  }
  catch(ex) {
    Log.Error("Error while moving mouse to given offset postion", ex);
  }
  finally {
    SavingLogToDisk("release" + getCurrentDateTime());
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: move
Purpose:
  To Moves the mouse to the specified element.
Parameters:
  strXpath: Xpath of an element
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function move() {
  try {
    if (BuiltIn.ParamCount() < 10)
      throw new Error("No parameter passed to the move function");
      
    var strXpath = BuiltIn.ParamStr(10);
    if (strXpath == null || strXpath == "" || aqString.Find(strXpath, "//") == -1)
      throw new Error("Invalid XPath string");
    
    ICAActionsXpath.move(strXpath);
  }
  catch(ex) {
    Log.Error("Error while performing click on the last known mouse coordinates", ex);
  }
  finally {
    SavingLogToDisk("move" + getCurrentDateTime());
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: moveToElement
Purpose:
  To Moves the mouse to the specified offset of the top-left corner of the specified element
  intOffsetx: X-coordinates
  intOffsety: y-coordinates
  isScrolltoView : bool value to scroll the element to view
Parameters:
  strXpath: Xpath of an element
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function moveToElement() {
  try {
    if (BuiltIn.ParamCount() < 10)
      throw new Error("No parameter passed to the moveToElement function");

    var strXpath = BuiltIn.ParamStr(10);
    if (strXpath == null || strXpath == "" || aqString.Find(strXpath, "//") == -1)
      throw new Error("Invalid XPath string");
    
    var strXOffset = BuiltIn.ParamStr(11);
    if (strXOffset == null || strXOffset == "")
      throw new Error("Invalid X-Offset position " + strXOffset);
     
    var strYOffset = BuiltIn.ParamStr(12);
    if (strYOffset == null || strYOffset == "")
      throw new Error("Invalid Y-Offset position " + strYOffset);
    
    var strIsScroll = BuiltIn.ParamStr(12);
    if (strIsScroll == null || strIsScroll == "")
      throw new Error("Invalid scroll parameter " + strIsScroll);
      
    ICAActionsXpath.moveToElement(strXpath, aqConvert.VarToInt(strXOffset), aqConvert.VarToInt(strYOffset), aqConvert.VarToBool(strIsScroll));
  }
  catch(ex) {
    Log.Error("Error while performing click on the last known mouse coordinates", ex);
  }
  finally {
    SavingLogToDisk("moveToElement" + getCurrentDateTime());
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: DragAndDropByOffset
Purpose:
  To Performs a drag-and-drop operation on one element to a specified offset
Parameters:
  strSourceXpath: Xpath of an element
  intOffsetx: X-coordinates
  intOffsety: y-coordinates
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function DragAndDropByOffset() {
  try {
    if (BuiltIn.ParamCount() < 10)
      throw new Error("No parameter passed to the DragAndDropByOffset function");

    var strXpath = BuiltIn.ParamStr(10);
    if (strXpath == null || strXpath == "" || aqString.Find(strXpath, "//") == -1)
      throw new Error("Invalid XPath string");
    
    var strXOffset = BuiltIn.ParamStr(11);
    if (strXOffset == null || strXOffset == "")
      throw new Error("Invalid X-Offset position " + strXOffset);
     
    var strYOffset = BuiltIn.ParamStr(12);
    if (strYOffset == null || strYOffset == "")
      throw new Error("Invalid Y-Offset position " + strYOffset);
      
    ICAActionsXpath.DragAndDropByOffset(strXpath, aqConvert.VarToInt(strXOffset), aqConvert.VarToInt(strYOffset));
  }
  catch(ex) {
    Log.Error("Error while performing drag an element to another element", ex);
  }
  finally {
    SavingLogToDisk("DragAndDropByOffset" + getCurrentDateTime());
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: SendKeys
Purpose:
  To Sends a sequence of keystrokes to the specified element in the browser.
Parameters:
  strXpath: Xpath of an element
  strKeysToSend: sequence of keystrokes
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function SendKeys() {
  try {
    if (BuiltIn.ParamCount() < 10)
      throw new Error("No parameter passed to the SendKeys function");

    var strXpath = BuiltIn.ParamStr(10);
    if (strXpath == null || strXpath == "" || aqString.Find(strXpath, "//") == -1)
      throw new Error("Invalid XPath string");
    
    var strKey = BuiltIn.ParamStr(11);
    if (strKey == null || strKey == "")
      throw new Error("Invalid Key to send -- " + strKey);
      
    ICAActionsXpath.SendKeys(strXpath, strKey);
  }
  catch(ex) {
    Log.Error("Error while sending sequence of keystrokes to the element", ex);
  }
  finally {
    SavingLogToDisk("SendKeys" + getCurrentDateTime());
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: MouseScroll
Purpose:
  To Scroll the mouse towards the given direction
Parameters:
  strXpath: Xpath of an element
  strDirection: direction to scroll
  intNoofTimes: count to scroll the mouse  wheel
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function MouseScroll() {
  try {
    if (BuiltIn.ParamCount() < 10)
      throw new Error("No parameter passed to the MouseScroll function");

    var strXpath = BuiltIn.ParamStr(10);
    if (strXpath == null || strXpath == "" || aqString.Find(strXpath, "//") == -1)
      throw new Error("Invalid XPath string");
    
    var strDirection = BuiltIn.ParamStr(11);
    if (strDirection == null || strDirection == "")
      throw new Error("Invalid Direction -- " + strDirection);
      
    var strTimes = BuiltIn.ParamStr(12);
    if (strTimes == null || strTimes == "")
      throw new Error("Invalid Number of scroll moves -- " + strTimes);
      
    ICAActionsXpath.MouseScroll(strXpath, strDirection, aqConvert.VarToInt(strTimes));
  }
  catch(ex) {
    Log.Error("Error while scrolling mouse wheel on specified element in the browser", ex);
  }
  finally {
    SavingLogToDisk("MouseScroll" + getCurrentDateTime());
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: SendKeysToBrowser
Purpose:
  To Sends a sequence of keystrokes to the browser.
Parameters:
  strKeysToSend: sequence of keystrokes
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function SendKeysToBrowser() {
  try {
   if (BuiltIn.ParamCount() < 10)
      throw new Error("No parameter passed to the SendKeysToBrowser function");

    var strXpath = BuiltIn.ParamStr(10);
    if (strXpath == null || strXpath == "" || aqString.Find(strXpath, "//") == -1)
      throw new Error("Invalid XPath string");
    
    var strKey = BuiltIn.ParamStr(11);
    if (strKey == null || strKey == "")
      throw new Error("Invalid Key to send -- " + strKey);
      
    ICAActionsXpath.SendKeysToBrowser(strKey);
  }
  catch(ex) {
    Log.Error("Error while sending a modifier key up message to the browser", ex);
  }
  finally {
    SavingLogToDisk("SendKeysToBrowser" + getCurrentDateTime());
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: SavingLogToDisk
Purpose:
  To save the log to the project location path with the given name
Parameters:
  strResultFolderName: name of the log file to save
Return Value:
  <Enter the return value here.>
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function SavingLogToDisk()
{
  try {
    if (BuiltIn.ParamCount() < 10)
      throw new Error("No parameter passed to the SavingLogToDisk function");

    var strFolderName = BuiltIn.ParamStr(10);
    if (strFolderName == null || strFolderName == "")
      throw new Error("Invalid Log Result Folder Name -- " + strFolderName);
      
    ICAActionsXpath.SavingLogToDisk(strFolderName);
  }
    catch(err) {
      Log.Error("Error while saving log to disk ", err);
  }
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: getCurrentDateTime
Purpose:
  To return the current datetime
Return Value:
  datetime object
-----------------------------------------------------------------------------------------------------------------------*/
function getCurrentDateTime() {
  var strValue = ICAActionsXpath.getCurrentDateTime();
  return strValue;
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: loginAndOpenCardioViewer()
Purpose:
  Function to login and make sure that the study is opened in cardio viewer
Parameters:
    
Return Value:
  
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/yy/dddd>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function loginAndOpenCardioViewer()
{
  try{
    var userName = "";
    var password = "";
    if (BuiltIn.ParamCount() > 10)
    {
        userName = BuiltIn.ParamStr(10);
        password = BuiltIn.ParamStr(11);
    }

    if (userName == "" || userName==undefined)
    {
      userName = "PRODUCTS\\ctx-qa";
    } 
    if (password == "" || password==undefined)
    {
      password = "Pa$$word";
    }   
      ICAActionsXpath.loginAndOpenCardioViewer(userName, password);
    
  }
  catch(ex)
  {
   Log.Error("Error while executing function loginAndOpenCardioViewer - " , ex);
  }  
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: loginAndOpenRadsuiteViewer()
Purpose:
  Function to login and make sure that the study is opened in RadSuite viewer
Parameters:
    
Return Value:
  
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/yy/dddd>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function loginAndOpenRadsuiteViewer()
{
  try{
    var userName = "";
    var password = "";
    if (BuiltIn.ParamCount() > 10)
    {
        userName = BuiltIn.ParamStr(10);
        password = BuiltIn.ParamStr(11);
    }
   
    ICAActionsXpath.loginAndOpenRadsuiteViewer(userName, password);
    
  }
  catch(ex)
  {
   Log.Error("Error while executing function loginAndOpenRadsuiteViewer - ", ex);
  }  
}


/*-----------------------------------------------------------------------------------------------------------------------
Function: loginAndOpenMPACSViewer()
Purpose:
  Function to login and make sure that the study is opened in MPACS viewer
Parameters:
    
Return Value:
  
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/yy/dddd>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function loginAndOpenMPACSViewer()
{
try{
    var userName = "";
    var password = "";
    var pid = "";
    if (BuiltIn.ParamCount() > 10)
    {
        userName = BuiltIn.ParamStr(10);
        password = BuiltIn.ParamStr(11);
        pid = BuiltIn.ParamStr(12);
    }
   
    if (pid == undefined || pid=="")
    {
      pid = "|";
    }
    
    ICAActionsXpath.loginAndOpenMPACSViewer(userName, password, pid);

  }
  catch(ex)
  {
   Log.Error("Error while executing function loginAndOpenMPACSViewer - ", ex);
  }  
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: handleErrorOccuredJavaDialog()
Purpose:
  Function to handle error occured java dialog
Parameters:    
Return Value:  
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/yy/dddd>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function handleErrorOccuredJavaDialog()
{
try
  {       
   if (BuiltIn.ParamCount() < 10)
      throw new Error("No parameter passed to the handleErrorOccuredJavaDialog function");

    var strWaitTime = BuiltIn.ParamStr(10);
    if (strWaitTime == null || strWaitTime == "")
      throw new Error("Invalid wait time for java dialog -- " + strWaitTime);
      
    ICAActionsXpath.handleErrorOccuredJavaDialog(aqConvert.VarToInt(strWaitTime));
  }
  catch(ex)
  {
   Log.Error("Error while executing function handleErrorOccuredJavaDialog - ", ex);
  }  
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: allowJavaRunPermissonDialog()
Purpose:
  Function to handle Java Dialog Run Permisson 
Parameters:    
Return Value:  
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/yy/dddd>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function allowJavaRunPermissonDialog()
{
try
  {
	  if (BuiltIn.ParamCount() < 10)
      throw new Error("No parameter passed to the allowJavaRunPermissonDialog function");

    var strWaitTime = BuiltIn.ParamStr(10);
    if (strWaitTime == null || strWaitTime == "")
      throw new Error("Invalid wait time for java dialog -- " + strWaitTime);
      
    ICAActionsXpath.allowJavaRunPermissonDialog(aqConvert.VarToInt(strWaitTime));
  }
  catch(ex)
  {
   Log.Error("Error while executing function allowJavaRunPermissonDialog - ", ex);
  }  
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: waitForPacsInstallation()
Purpose:
  Function to handle Java Dialog Run Permisson 
Parameters:    
Return Value:  
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/yy/dddd>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function waitForPacsInstallation()
{ 
  try
  {
	  if (BuiltIn.ParamCount() < 10)
      throw new Error("No parameter passed to the waitForPacsInstallation function");

    var strWaitTime = BuiltIn.ParamStr(10);
    if (strWaitTime == null || strWaitTime == "")
      throw new Error("Invalid wait time for PACS installation -- " + strWaitTime);
      
    ICAActionsXpath.waitForPacsInstallation(aqConvert.VarToInt(strWaitTime));   	
  }
  catch(ex)
  {
   Log.Error("Error while executing function waitForPacsInstallation - ", ex);
  }  
}
/*-----------------------------------------------------------------------------------------------------------------------
Function: PerformDraganddrop
-----------------------------------------------------------------------------------------------------------------------*/
function PerformDraganddrop() {
    try {
        ICAActionsXpath.PerformDraganddrop();
    }
    catch (ex) {
        Log.Error("Error while performing click on the last known mouse coordinates", ex);
    }
    finally {
        SavingLogToDisk("clickAndHoldLastMouseCoOrdinates" + getCurrentDateTime());
    }
}
/*-----------------------------------------------------------------------------------------------------------------------
Function: MoveAndClick
-----------------------------------------------------------------------------------------------------------------------*/
function MoveAndClick() {
    try {
        ICAActionsXpath.MoveAndClick();
    }
    catch (ex) {
        Log.Error("Error while performing click on the last known mouse coordinates", ex);
    }
    finally {
        SavingLogToDisk("clickAndHoldLastMouseCoOrdinates" + getCurrentDateTime());
    }
}
/*-----------------------------------------------------------------------------------------------------------------------
Function: clickSavePopup
-----------------------------------------------------------------------------------------------------------------------*/
function clickSavePopup() {
    try {
        ICAActionsXpath.clickSavePopup();
    }
    catch (ex) {
        Log.Error("Error while performing click on the last known mouse coordinates", ex);
    }
    finally {
        SavingLogToDisk("clickAndHoldLastMouseCoOrdinates" + getCurrentDateTime());
    }
}
/*-----------------------------------------------------------------------------------------------------------------------
Function: SetFPS
-----------------------------------------------------------------------------------------------------------------------*/
function SetFPS() {
    try
	{
		if (BuiltIn.ParamCount() < 10)
			throw new Error("No parameter passed to the SetFPS function");
		var xpathSliderPointer = BuiltIn.ParamStr(10);
		if (xpathSliderPointer == null || xpathSliderPointer == "" || aqString.Find(xpathSliderPointer, "//") == -1)
			throw new Error("Invalid Source XPath string");
		var xpathSlider = BuiltIn.ParamStr(11);
		if (xpathSlider == null || xpathSlider == "" || aqString.Find(xpathSlider, "//") == -1)
			throw new Error("Invalid Destination XPath string");
		var intExpectedValue = BuiltIn.ParamStr(12);
		if (intExpectedValue == null || intExpectedValue == "" || aqString.Find(intExpectedValue, "//") == -1)
			throw new Error("Invalid Destination XPath string");
		ICAActionsXpath.SetFPS(xpathSliderPointer, xpathSlider, intExpectedValue);
    }
    catch (ex) {
        Log.Error("Error while performing SetFPS", ex);
    }
    finally {
        SavingLogToDisk("SetFPS" + getCurrentDateTime());
    }
}