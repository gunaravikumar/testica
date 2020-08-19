//USEUNIT GenericUtils
//

function getBrowser() {
    //return Sys.FindChildEx(["ObjectType", "ObjectIdentifier","FullName"], ["Browser", "chrome"], 5, 10000);
	var counter = 0;
	do {
		Sys.Refresh();
		Delay(2000);
		 var page = Sys.FindChildEx(["ObjectType", "ObjectIdentifier"], ["Page", "*web*ccess*"], 30, 10000);
		 Delay(1000);
		 //if(page == null || page.Exists == false)
		   // Log.Error("Browser not found attempt"+counter);
		 //else
		   // Log.Error("Browser found attempt" +counter);
		 counter++;
		} while(page.Exists == false && counter< 10)
		return page;
   
}


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

        var intMouseX = Sys.Desktop.MouseX;
        var intMousey = Sys.Desktop.MouseY;
        var objOnScreen = Sys.ObjectFromPoint(intMouseX, intMousey);
        var intControlPoint = objOnScreen.ScreenToWindow(intMouseX, intMousey);
        objOnScreen.Click(intControlPoint.X, intControlPoint.Y);
        }

        catch (ex) {
            Log.Error("Error while performing click on the last known mouse coordinates", ex);
        } finally {
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
    function click(strXpath) {
        try {

            var objICAViewer = getBrowser();
            if (objICAViewer == null || !objICAViewer.Exists) {
                throw new Error("Unable to get the Browser object");
            }
            var objElement = objICAViewer.FindChildByXPath(strXpath, true);
            if (objElement == null || !objElement.Exists) {
                throw new Error("given xpath related element does not exists");
            }
            objElement.Click();
        } catch (ex) {
            Log.Error("Error while performing click on the element", ex);
        } finally {
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
            var intMouseX = Sys.Desktop.MouseX;
            var intMousey = Sys.Desktop.MouseY;
            var objOnScreen = Sys.ObjectFromPoint(intMouseX, intMousey);
            var intControlPoint = objOnScreen.ScreenToWindow(intMouseX, intMousey);
            objOnScreen.ClickR(intControlPoint.X, intControlPoint.Y);
        } catch (ex) {
            Log.Error("Error while performing click on the last known mouse coordinates", ex);
        } finally {
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
    function contextClick(strXpath) {
        try {
            var objICAViewer = getBrowser();
            if (objICAViewer == null || !objICAViewer.Exists) {
                throw new Error("ICAViewer does not exists");
            }
            var objElement = objICAViewer.FindChildByXPath(strXpath, true);
            if (objElement == null || !objElement.Exists) {
                throw new Error("given xpath related element does not exists");
            }
            objElement.ClickR();
        } catch (ex) {
            Log.Error("Error while performing click on the element", ex);
        } finally {
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
            var intMouseX = Sys.Desktop.MouseX;
            var intMousey = Sys.Desktop.MouseY;
            var objOnScreen = Sys.ObjectFromPoint(intMouseX, intMousey);
            var intControlPoint = objOnScreen.ScreenToWindow(intMouseX, intMousey);
            objOnScreen.DblClick(intControlPoint.X, intControlPoint.Y);
        } catch (ex) {
            Log.Error("Error while performing click on the last known mouse coordinates", ex);
        } finally {
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
    function DoubleClick(strXpath) {
        try {
            var objICAViewer = getBrowser();
            if (objICAViewer == null || !objICAViewer.Exists) {
                throw new Error("Unable to get the Browser object");
            }
            var objElement = objICAViewer.FindChildByXPath(strXpath, true);
            if (objElement == null || !objElement.Exists) {
                throw new Error("given xpath related element does not exists");
            }
            objElement.DblClick();
        } catch (ex) {
            Log.Error("Error while performing double click on the element", ex);
        } finally {
            SavingLogToDisk("DoubleClick" + getCurrentDateTime());
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
    function DragAndDrop(strImageXpath, strDestinationTableXpath, scrollIntoView) {
        try {
            Log.Message("enetered to routine");
            if (aqString.Find(strImageXpath, "//") == -1 || aqString.Find(strDestinationTableXpath, "//") == -1) {
                throw new Error("given xpath is Invalid syntax");
            }
            // var objICAViewer = Sys.Browser("firefox").Page("*/webaccess/Default.ashx");
            var objICAViewer = getBrowser();
            if (!objICAViewer.Exists) {
                throw new Error("browser does not exists");
            }
            //objICAViewer.Activate();
            var objIconToMove = objICAViewer.FindChildByXPath(strImageXpath, true);
            if (objIconToMove == null || !objIconToMove.Exists) {
                throw new Error("given source xpath does not exists");
            }
            var objDestinationTable = objICAViewer.FindChildByXPath(strDestinationTableXpath, true);
            if (objDestinationTable == null) {
                throw new Error("given destination xpath does not exists");
            }
            if (scrollIntoView === undefined || scrollIntoView == "true") {
                objDestinationTable.scrollIntoView();
            }
            objDestinationTable.WindowToScreen(objDestinationTable.Height, objDestinationTable.Width);
            var intStartY = objIconToMove.Height / 2
            var intStartX = objIconToMove.Width / 2;
            var arrCoordinates = getXYCoordinates(objDestinationTable, objIconToMove);
            objIconToMove.Drag(intStartX, intStartY, arrCoordinates[0], arrCoordinates[1]);
        } catch (ex) {
            Log.Error("Error while performing drag an element to another element", ex);
        } finally {
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
    function KeyDown(strString) {
        try {
            var objICAViewer = getBrowser();
            if (objICAViewer == null || !objICAViewer.Exists) {
                throw new Error("ICAViewer does not exists");
            }
            objICAViewer.Click();
            Sys.Desktop.KeyDown(strString);
        } catch (ex) {
            Log.Error("Error while sending a modifier key up message to the browser", ex);
        } finally {
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
    function KeyUp(strString) {
        try {
            var objICAViewer = getBrowser();
            if (objICAViewer == null || !objICAViewer.Exists) {
                throw new Error("ICAViewer does not exists");
            }
            objICAViewer.Click();
            Sys.Desktop.KeyUp(strString);
        } catch (ex) {
            Log.Error("Error while sending a modifier key up message to the browser", ex);
        } finally {
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
    function KeyDownElement(strXpath, strString) {
        try {
            var objICAViewer = getBrowser();
            if (objICAViewer == null || !objICAViewer.Exists) {
                throw new Error("ICAViewer does not exists");
            }
            var objElement = objICAViewer.FindChildByXPath(strXpath, true);
            if (objElement == null || !objElement.Exists) {
                throw new Error("given xpath does not exists");
            }
            objElement.Click();
            //    aqUtils.Delay(1000);
            //    strDirection = "Up"
            //    objElement.MouseWheel(strDirection.toLowerCase().indexOf("down") != -1 ? -1 : 1 );
            Sys.Desktop.KeyDown(strString);
        } catch (ex) {
            Log.Error("Error while sending a modifier up down message to the specified element in the browser", ex);
        } finally {
            SavingLogToDisk("KeyDownElement" + getCurrentDateTime());
        }
    }

    /*-----------------------------------------------------------------------------------------------------------------------
    Function: KeyDownElement
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
    function KeyUpElement(strXpath, strString) {
        try {
            var objICAViewer = getBrowser();
            if (objICAViewer == null || !objICAViewer.Exists) {
                throw new Error("ICAViewer does not exists");
            }
            var objElement = objICAViewer.FindChildByXPath(strXpath, true);
            if (objElement == null || !objElement.Exists) {
                throw new Error("given xpath does not exists");
            }
            objElement.Click();
            Sys.Desktop.KeyUp(strString);
        } catch (ex) {
            Log.Error("Error while sending a modifier up down message to the specified element in the browser", ex);
        } finally {
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
            var intMouseX = Sys.Desktop.MouseX;
            var intMousey = Sys.Desktop.MouseY;
            LLPlayer.MouseDown(MK_LBUTTON, intMouseX, intMousey, 10);
        } catch (ex) {
            Log.Error("Error while performing click on the last known mouse coordinates", ex);
        } finally {
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
            var intMouseX = Sys.Desktop.MouseX;
            var intMousey = Sys.Desktop.MouseY;
            LLPlayer.MouseUp(MK_LBUTTON, intMouseX, intMousey, 10);
        } catch (ex) {
            Log.Error("Error while performing click on the last known mouse coordinates", ex);
        } finally {
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
    function clickAndHold(strXpath) {
        try {
            var objICAViewer = getBrowser();
            if (objICAViewer == null || !objICAViewer.Exists) {
                throw new Error("ICAViewer does not exists");
            }
            var objElement = objICAViewer.FindChildByXPath(strXpath, true);
            if (objElement == null || !objElement.Exists) {
                throw new Error("given xpath related element does not exists");
            }
            objElement.WindowToScreen(objElement.Height, objElement.Width);
            var intStartX = (objElement.ScreenLeft + (objElement.Width / 2));
            var intStartY = (objElement.ScreenTop + (objElement.Height / 2));
            LLPlayer.MouseDown(MK_LBUTTON, intStartX, intStartY, 10);
        } catch (ex) {
            Log.Error("Error while performing click on the last known mouse coordinates", ex);
        } finally {
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
    function release(strXpath) {
        try {
            var objICAViewer = getBrowser();
            if (objICAViewer == null || !objICAViewer.Exists) {
                throw new Error("ICAViewer does not exists");
            }
            var objElement = objICAViewer.FindChildByXPath(strXpath, true);
            if (objElement == null || !objElement.Exists) {
                throw new Error("given xpath related element does not exists");
            }
            objElement.WindowToScreen(objElement.Height, objElement.Width);
            var intStartX = (objElement.ScreenLeft + (objElement.Width / 2));
            var intStartY = (objElement.ScreenTop + (objElement.Height / 2));
            LLPlayer.MouseUp(MK_LBUTTON, intStartX, intStartY, 10);
        } catch (ex) {
            Log.Error("Error while performing click on the last known mouse coordinates", ex);
        } finally {
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
    function MoveByOffset(intOffsetx, intOffsety) {
        try {
            var intMouseX = Sys.Desktop.MouseX;
            var intMousey = Sys.Desktop.MouseY;
            intOffsetx = aqConvert.VarToInt(intOffsetx);
            intOffsety = aqConvert.VarToInt(intOffsety);
            LLPlayer.MouseMove(intMouseX + intOffsetx, intMousey + intOffsety, 10);
        } catch (ex) {
            Log.Error("Error while performing click on the last known mouse coordinates", ex);
        } finally {
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
    function move(strXpath) {
        try {
			Log.Error("Method Move");
            var objICAViewer = getBrowser();
            if (objICAViewer == null || !objICAViewer.Exists) {
                throw new Error("ICAViewer does not exists");
            }
            var objElement = objICAViewer.FindChildByXPath(strXpath, true);
            if (objElement == null || !objElement.Exists) {
                throw new Error("given xpath does not exists");
            }
            objElement.WindowToScreen(objElement.Height, objElement.Width);
            var intStartX = (objElement.ScreenLeft + (objElement.Width / 2));
            var intStartY = (objElement.ScreenTop + (objElement.Height / 2));
            LLPlayer.MouseMove(intStartX, intStartY, 50);
        } catch (ex) {
            Log.Error("Error while performing click on the last known mouse coordinates", ex);
        } finally {
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
    function moveToElement(strXpath, intOffsetx, intOffsety, isScrolltoView) {
        try {
            var objICAViewer = getBrowser();
            if (objICAViewer == null || !objICAViewer.Exists) {
                throw new Error("ICAViewer does not exists");
            }
            Log.Message(objICAViewer.Exists);
            var objElement = objICAViewer.FindChildByXPath(strXpath, true);
            if (objElement == null || !objElement.Exists) {
                aqUtils.Delay(3000);
                var objElement = objICAViewer.FindChildByXPath(strXpath, true);
                if (objElement == null || !objElement.Exists) {
                    throw new Error("given xpath does not exists");
                }
            }
            objElement.WindowToScreen(objElement.Height, objElement.Width);			
            if (isScrolltoView != "false") {
                objElement.scrollIntoView();
            }
            intOffsetx = aqConvert.VarToInt(intOffsetx);
            intOffsety = aqConvert.VarToInt(intOffsety);
            Log.Message(intOffsetx);
            Log.Message(intOffsety);
            var intStartX = objElement.ScreenLeft;
            var intStartY = objElement.ScreenTop;
            LLPlayer.MouseMove(intStartX + intOffsetx, intStartY + intOffsety, 10);
            
        } catch (ex) {
            Log.Error("Error while performing click on the last known mouse coordinates", ex);
        } finally {
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
    function DragAndDropByOffset(strSourceXpath, intOffsetx, intOffsety) {
        try {
            var objICAViewer = getBrowser();
            if (objICAViewer == null || !objICAViewer.Exists) {
                throw new Error("ICAViewer does not exists");
            }
            //objICAViewer.Activate();
            var objIconToMove = objICAViewer.FindChildByXPath(strSourceXpath, true);
            if (objIconToMove == null || !objIconToMove.Exists) {
                throw new Error("given source xpath does not exists");
            }
            objIconToMove.scrollIntoView();
            intOffsetx = aqConvert.VarToInt(intOffsetx);
            intOffsety = aqConvert.VarToInt(intOffsety);
            var intStartY = objIconToMove.Height / 2
            var intStartX = objIconToMove.Width / 2;
            objIconToMove.Drag(intStartX, intStartY, intStartX + intOffsetx, intStartY + intStartX);
        } catch (ex) {
            Log.Error("Error while performing drag an element to another element", ex);
        } finally {
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
    function SendKeys(strXpath, strKeysToSend) {
        try {
            var objICAViewer = getBrowser();
            if (objICAViewer == null || !objICAViewer.Exists) {
                throw new Error("ICAViewer does not exists");
            }
            var objElementTextbox = objICAViewer.FindChildByXPath(strXpath, true);
            if (objElementTextbox == null || !objElementTextbox.Exists) {
                throw new Error("given xpath does not exists");
            }
            objElementTextbox.Click();
            objElementTextbox.Keys(strKeysToSend + "[Enter]");
        } catch (ex) {
            Log.Error("Error while sending sequence of keystrokes to the element", ex);
        } finally {
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
    function MouseScroll(strXpath, strDirection, intNoofTimes) {
        try {
            var objICAViewer = getBrowser();
            if (objICAViewer == null || !objICAViewer.Exists) {
                throw new Error("ICAViewer does not exists");
            }
            var objElement = objICAViewer.FindChildByXPath(strXpath, true);
            if (objElement == null || !objElement.Exists) {
                throw new Error("given xpath does not exists");
            }
            //objElement.Click();
            intTotalCount = aqConvert.VarToInt(intNoofTimes);
            for (var intCount = 0; intCount < intTotalCount; intCount++) {
                aqUtils.Delay(1000);
                objElement.MouseWheel(strDirection.toLowerCase().indexOf("down") != -1 ? -1 : 1);
            }
        } catch (ex) {
            Log.Error("Error while scrolling mouse wheel on specified element in the browser", ex);
        } finally {
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
    function SendKeysToBrowser(strKeysToSend) {
        try {
            var objICAViewer = getBrowser();
            if (objICAViewer == null || !objICAViewer.Exists) {
                throw new Error("ICAViewer does not exists");
            }
            //objICAViewer.Click();                  
            //objICAViewer.Keys(strKeysToSend + "[Enter]");
            Sys.Desktop.Keys(strKeysToSend + "[Enter]");
        } catch (ex) {
            Log.Error("Error while sending a modifier key up message to the browser", ex);
        } finally {
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
    function SavingLogToDisk(strResultFolderName) {
        var Path;
        // Obtains the path to the current project suite’s file
        Path = ProjectSuite.Path;
        Path = Path + "\\TestCompleteLog";
        // Creates a new folder
        aqFileSystem.CreateFolder(Path);
        Log.SaveResultsAs(Path + "\\\\" + strResultFolderName + ".mht", lsMHT);
    }

    /*-----------------------------------------------------------------------------------------------------------------------
    Function: getCurrentDateTime
    Purpose:
      To return the current datetime
    Return Value:
      datetime object
    -----------------------------------------------------------------------------------------------------------------------*/
    function getCurrentDateTime() {
        var strTime = aqDateTime.Time();
        var strString = aqConvert.DateTimeToFormatStr(strTime, "%M:%S")
        var arrDate = strString.split(":");
        var strValue = arrDate[1];
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
    function loginAndOpenCardioViewer(userName, password) {
        try {

            var browserName = "iexplore"; //Only IE works for cardio
            //Default Values
            if (userName == "" || userName == undefined) {
                userName = "PRODUCTS\\ctx-qa";
            }
            if (password == "" || password == undefined) {
                password = "Pa$$word";
            }
            //Login     
            var vericisBrowser = Sys.Browser(browserName);
            var loginPopUp = vericisBrowser.FindChildEx(["FullName", "Name", "Visible"], ["*" + browserName + "*vericis*", "Login", "True"], 50, true, 30000);
            if (loginPopUp.Exists) {
                var userNameTxtBx = loginPopUp.FindChildEx(["ObjectIdentifier", "ObjectType"], ["UserName", "TextBox"], 50, true, 10000);
                var passwordTxtBx = loginPopUp.FindChildEx(["ObjectIdentifier", "ObjectType"], ["Password", "TextBox"], 50, true, 10000);
                if (userNameTxtBx.Exists && passwordTxtBx.Exists) {
                    userNameTxtBx.SetText(userName);
                    userNameTxtBx.Click();
                    aqUtils.Delay(1000);
                    passwordTxtBx.SetText(password);
                    var okBtn = loginPopUp.FindChildEx(["ObjectIdentifier", "ObjectType"], ["OK", "Button"], 50, true, 10000);
                    okBtn.Click();
                } else
                    throw new Error("UserName Password fields not availalbe");
            } else
                throw new Error("Login pop-up not availalbe");
            //Install the workstation viewer Add on  
            var addOnInstallBtn = vericisBrowser.FindChildEx(["ObjectIdentifier", "FullName"], ["Install", "*" + browserName + "*Notification*Install"], 50, true, 20000);
            if (addOnInstallBtn.Exists) {
                addOnInstallBtn.Click();
                var confirmationPopup = vericisBrowser.FindChildEx(["ObjectType", "Message"], ["Confirm", "*install this software*"], 50, true, 20000);
                var installBtn = vericisBrowser.FindChildEx(["WndClass", "WndCaption", "VisibleOnScreen"], ["Button", "&Install", true], 50, true, 20000);
                installBtn.Click();
            }
            //Check the Study is Opened  
            vericisBrowser.Refresh();
            var thumbnailImg = vericisBrowser.FindChildEx(["ObjectIdentifier", "ObjectType", "VisibleOnScreen"], ["jpg", "Image", true], 50, true, 30000);
            if (!thumbnailImg.Exists)
                throw new Error("Study not openend poperly in Cardio Viewer");
            try {
                var Tab = Sys.Browser(browserName).FindChildEx(["ObjectType", "ObjectIdentifier"], ["TabButton", "*Cardio Web Client*"], 50, true, 10000);
                Tab.Close();
            } catch (ex) {

            }
        } catch (ex) {
            Log.Error("Error while executing function loginAndOpenCardioViewer - ", ex);
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
    function loginAndOpenRadsuiteViewer(userName, password) {
        try {

            if (userName == undefined) {
                userName = "";
            }
            if (password == undefined) {
                password = "";
            }
            //Wait for Java Process & Handle Popups
            Sys.WaitProcess("jp2launcher", 30000);
            allowJavaRunPermissonDialog(45);
            Sys.WaitProcess("jp2launcher", 20000);
            handleErrorOccuredJavaDialog(30);
            //Login
            var javaProcess = Sys.Process("jp2launcher");
            if (userName != "") {
                var loginWindow = javaProcess.FindChildEx(["AWTComponentAccessibleName", "JavaFullClassName"], ["Merge Radsuite", "javax.swing.JFrame"], 50, true, 10000);
                if (loginWindow.Exists) {
                    var userNameTxtBx = javaProcess.FindChildEx(["JavaClassName", "Name"], ["JTextField", "*JTextField*"], 50, true, 10000);
                    var passwordTxtBx = javaProcess.FindChildEx(["JavaClassName", "Name"], ["JPasswordField", "*JPasswordField*"], 50, true, 10000);
                    if (userNameTxtBx.Exists && passwordTxtBx.Exists) {
                        userNameTxtBx.Keys(userName);
                        passwordTxtBx.Keys(password);
                        aqUtils.Delay(1000);
                        var okBtn = javaProcess.FindChildEx(["JavaClassName", "AWTComponentAccessibleName", "VisibleOnScreen"], ["JButton", "OK", true], 50, true, 10000);
                        if (okBtn.Exists)
                            okBtn.Click();
                    } else
                        throw new Error("UserName Password fields not availalbe");
                } else
                    throw new Error("Login pop-up not availalbe");
            }
            //Handle warnings
            handleErrorOccuredJavaDialog(10);
            //Check the Study is visible   
            var counterI = 0;
            do {
                imagePanel = Sys.Process("jp2launcher").FindChildEx(["JavaClassName", "FullName", "VisibleOnScreen"], ["UVSwingPanel", "*Radsuite*", true], 50, true, 1000);
            }
            while (!imagePanel.Exists && counterI++ < 25)
            if (!imagePanel.Exists)
                throw new Error("Study Image not opened");

        } catch (ex) {
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
    function loginAndOpenMPACSViewer(userName, password, patientIdentifier) {
        try {

            //Default Values
            if (userName == undefined) {
                userName = "";
            }
            if (password == undefined) {
                password = "";
            }
            if (patientIdentifier == undefined || patientIdentifier == "") {
                patientIdentifier = "|";
            }


            //wait for Viewer Installation
            allowJavaRunPermissonDialog(30);
            var counterI = 0;
            var studyOpenened = false;
            do {
                //Wait for aviewer
                Sys.WaitProcess("aViewer", 10000);
                allowJavaRunPermissonDialog(5);
                try {
                    var imageAWTObject = Sys.Process("aViewer").FindChildEx(["FullName", "JavaClassName", "VisibleOnScreen"], ["*JFrame*" + patientIdentifier + "*AWTObject*", "HeavyweightRenderWindow", true], 50, true, 5000);
                    if (imageAWTObject.Exists)
                        studyOpenened = true;
                } catch (ex) {}
            }
            while (!studyOpenened && counterI++ < 10)

            //Check if the Study is visible
            imageAWTObject = Sys.Process("aViewer").FindChildEx(["FullName", "JavaClassName", "VisibleOnScreen"], ["*JFrame*" + patientIdentifier + "*AWTObject*", "HeavyweightRenderWindow", true], 50, true, 15000);
            if (!imageAWTObject.Exists)
                throw new Error("Study Image not opened Properly");

        } catch (ex) {
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
    function handleErrorOccuredJavaDialog(waitSeconds) {
        try {
            var counterI = 0;
            do {
                okOption = Sys.Process("jp2launcher").FindChildEx(["AWTComponentName", "AWTComponentAccessibleName", "VisibleOnScreen"], ["OptionPane.button", "OK", true], 50, true, 1000);
            }
            while (!okOption.Exists && counterI++ < waitSeconds)
            if (okOption.Exists)
                okOption.Click();
        } catch (ex) {
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
    function allowJavaRunPermissonDialog(waitSeconds) {

        try {
            var counterI = 0;
            do {
                var runBtn = Sys.Process("jp2launcher").FindChildEx(["JavaClassName", "AWTComponentAccessibleName", "VisibleOnScreen"], ["JButton", "Run", true], 50, true, 1000);
            }
            while (!runBtn.Exists && counterI++ < waitSeconds)
            if (runBtn.Exists) {
                if (runBtn.Enabled == false) {
                    var acceptRiskChkBx = Sys.Process("jp2launcher").FindChildEx(["JavaClassName", "AWTComponentAccessibleName", "VisibleOnScreen"], ["JCheckBox", "*accept the risk*", true], 50, true, 1000);
                    acceptRiskChkBx.Click();
                }
                if (runBtn.Enabled)
                    runBtn.Click();
            }
        } catch (ex) {
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
    function waitForPacsInstallation(maxSecondsToWait) {
        var counterI = 0;
        try {
            skipIdentification = false;
            do {
                var mpacsForm = Sys.FindChildEx(["ObjectType", "ObjectIdentifier"], ["Form", "Merge PACS*"], 50, true, 5000);
                if (mpacsForm == undefined) {
                    aqUtils.Delay(45000, "Wait for PACS install if required");
                    skipIdentification = true;
                } else {
                    var viewerInstalledLbl = mpacsForm.FindChildEx(["AWTComponentAccessibleName", "JavaClassName"], ["Viewer installed", "JLabel"], 50, true, 5000);
                    var ReinstallBtn = mpacsForm.FindChildEx(["AWTComponentAccessibleName", "JavaClassName"], ["Reinstall", "InstallerButton"], 50, true, 5000);
                }
            }
            while (!viewerInstalledLbl.Exists && counterI++ < aqConvert.VarToInt(maxSecondsToWait / 10) && !ReinstallBtn.Exists && !skipIdentification);
            if (!viewerInstalledLbl.Exists && !ReinstallBtn.Exists && !skipIdentification)
                throw new Error("PACS Viewer not installed successfully. Waited for " + maxSecondsToWait + " seconds");
        } catch (ex) {
            Log.Error("Error while executing function waitForPacsInstallation - ", ex);
        }
    }
	/*
	Function: Click Save
	*/
	function clickSavePopup() 
	{			
		try
		{
				var flag=0;
				try
				{
			    var objNotification = Sys.Browser("iexplore").FindChildEx(["WndClass"],["Frame Notification Bar"],2,true,5000);					var objSave = objNotification.FindChildEx(["ObjectIdentifier"],["Save"],10,true,5000);
				if (objNotification == null || !objNotification.Exists) 
				{
					throw new Error("Unable to get the Browser object");
				}
				if (objSave == null || !objSave.Exists) 
				{
					throw new Error("given xpath related element does not exists");
				}
				objSave.Click();
				aqUtils.Delay(5000);
				var objClose = objNotification.FindChildEx(["ObjectIdentifier"],["Close"],10,true,5000);
				if (objClose == null || !objClose.Exists) 
				{
					throw new Error("given xpath related element does not exists");
				}
				objClose.Click();
				flag=1;
				}
				catch(e)
				{
				}
				
				if(flag ==0)
				{
				var objviewdownload = Sys.Browser("iexplore").FindChildEx("WndClass","#32770",3,true,3000);
				var objlist = objviewdownload.FindChildEx(["Frameworkid","ClassName","LocalizedControlType","ObjectIdentifier"],["Win32","DirectUIHWND","pane","View_and_track_your_downloads"],3,true);
				var objSaveBtn = objlist.FindChildEx(["LocalizedControlType","ObjectIdentifier"],["split button","Save"],3,true);
				objSaveBtn.Click();
				aqUtils.Delay(5000);
				var objclearbtn = objviewdownload.FindChildEx(["LocalizedControlType","ObjectIdentifier"],["button","Clear_list"],3,true);
				objclearbtn.Click();
				aqUtils.Delay(3000);
				var objclosebtn = objviewdownload.FindChildEx(["LocalizedControlType","ObjectIdentifier"],["button","Close"],3,true);
				objclosebtn.Click();
				aqUtils.Delay(3000);
				}
		}
		catch(e)
		{
			Log.Error("Error while performing click on the element", ex);
		} 
		finally 
		{
			SavingLogToDisk("click" + getCurrentDateTime());
		}	
	}
	
	/*
	Function: PerformDraganddrop
	*/
    function PerformDraganddrop(strXpath, intOnsetx, intOnsety, intOffsetx, intOffsety, isScrolltoView) {
        try {
            var objICAViewer = getBrowser();
            if (objICAViewer == null || !objICAViewer.Exists) {
                throw new Error("ICAViewer does not exists");
            }
            Log.Message(objICAViewer.Exists);
            var objElement = objICAViewer.FindChildByXPath(strXpath, true);
            if (objElement == null || !objElement.Exists) {
                aqUtils.Delay(3000);
                var objElement = objICAViewer.FindChildByXPath(strXpath, true);
                if (objElement == null || !objElement.Exists) {
                    throw new Error("given xpath does not exists");
                }
            }
            objElement.WindowToScreen(objElement.Height, objElement.Width);			
            if (isScrolltoView != "false") {
                objElement.scrollIntoView();
            }
			intOnsetx = aqConvert.VarToInt(intOnsetx);
            intOnsety = aqConvert.VarToInt(intOnsety);
            intOffsetx = aqConvert.VarToInt(intOffsetx);
            intOffsety = aqConvert.VarToInt(intOffsety);
            Log.Message(intOffsetx);
            Log.Message(intOffsety);
            var intStartX = objElement.ScreenLeft;
            var intStartY = objElement.ScreenTop;
			LLPlayer.MouseMove(intStartX + intOnsetx, intStartY + intOnsety, 500);
			var intMouseX = Sys.Desktop.MouseX;
            var intMousey = Sys.Desktop.MouseY;
			LLPlayer.MouseDown(MK_LBUTTON, intMouseX, intMousey, 500);
            LLPlayer.MouseMove(intStartX + intOffsetx, intStartY + intOffsety, 500);
			var intMouseX1 = Sys.Desktop.MouseX;
            var intMousey1 = Sys.Desktop.MouseY;
			LLPlayer.MouseUp(MK_LBUTTON, intMouseX1, intMousey1, 500);
        } catch (ex) {
            Log.Error("Error while performing click on the last known mouse coordinates", ex);
        } finally {
            SavingLogToDisk("moveToElement" + getCurrentDateTime());
        }
    }
	
	/*
	Function: MoveAndClick
	*/
	function MoveAndClick(strXpath, intOffsetx, intOffsety, isScrolltoView) {
        try {
            var objICAViewer = getBrowser();
            if (objICAViewer == null || !objICAViewer.Exists) {
                throw new Error("ICAViewer does not exists");
            }
            Log.Message(objICAViewer.Exists);
            var objElement = objICAViewer.FindChildByXPath(strXpath, true);
            if (objElement == null || !objElement.Exists) {
                aqUtils.Delay(3000);
                var objElement = objICAViewer.FindChildByXPath(strXpath, true);
                if (objElement == null || !objElement.Exists) {
                    throw new Error("given xpath does not exists");
                }
            }
            objElement.WindowToScreen(objElement.Height, objElement.Width);			
            if (isScrolltoView != "false") {
                objElement.scrollIntoView();
            }
            intOffsetx = aqConvert.VarToInt(intOffsetx);
            intOffsety = aqConvert.VarToInt(intOffsety);
            Log.Message(intOffsetx);
            Log.Message(intOffsety);
			var intStartX = objElement.ScreenLeft;
            var intStartY = objElement.ScreenTop;
			LLPlayer.MouseMove(intStartX + intOffsetx, intStartY + intOffsety, 500);
			var intMouseX = Sys.Desktop.MouseX;
            var intMousey = Sys.Desktop.MouseY;
			LLPlayer.MouseDown(MK_LBUTTON, intMouseX, intMousey, 100);
			LLPlayer.MouseUp(MK_LBUTTON, intMouseX, intMousey, 100);
        } catch (ex) {
            Log.Error("Error while performing click on the last known mouse coordinates", ex);
        } finally {
            SavingLogToDisk("moveToElement" + getCurrentDateTime());
        }
    }
	/*
	Function: MoveClickAndHold
	*/
	function MoveClickAndHold(strXpath, intOffsetx, intOffsety, isScrolltoView) {
        try {
            var objICAViewer = getBrowser();
            if (objICAViewer == null || !objICAViewer.Exists) {
                throw new Error("ICAViewer does not exists");
            }
            Log.Message(objICAViewer.Exists);
            var objElement = objICAViewer.FindChildByXPath(strXpath, true);
            if (objElement == null || !objElement.Exists) {
                aqUtils.Delay(3000);
                var objElement = objICAViewer.FindChildByXPath(strXpath, true);
                if (objElement == null || !objElement.Exists) {
                    throw new Error("given xpath does not exists");
                }
            }
            objElement.WindowToScreen(objElement.Height, objElement.Width);			
            if (isScrolltoView != "false") {
                objElement.scrollIntoView();
            }
            intOffsetx = aqConvert.VarToInt(intOffsetx);
            intOffsety = aqConvert.VarToInt(intOffsety);
            Log.Message(intOffsetx);
            Log.Message(intOffsety);
			var intStartX = objElement.ScreenLeft;
            var intStartY = objElement.ScreenTop;
			LLPlayer.MouseMove(intStartX + intOffsetx, intStartY + intOffsety, 500);
			var intMouseX = Sys.Desktop.MouseX;
            var intMousey = Sys.Desktop.MouseY;
			LLPlayer.MouseDown(MK_LBUTTON, intMouseX, intMousey, 100);
			aqUtils.Delay(5000);
			LLPlayer.MouseUp(MK_LBUTTON, intMouseX, intMousey, 100);
        } catch (ex) {
            Log.Error("Error while performing click on the last known mouse coordinates", ex);
        } finally {
            SavingLogToDisk("moveToElement" + getCurrentDateTime());
        }
    }
	/*
	Function: SetFPS
	*/
	function SetFPS(xpathSliderPointer, xpathSlider, intExpectedValue)
	{
		aqUtils.Delay(1000);
		var objICAViewer = getBrowser();
		if (objICAViewer == null || !objICAViewer.Exists)
		{
			throw new Error("Unable to get the Browser object");
		}
		var  sliderPointer = objICAViewer.FindChildByXPath(xpathSliderPointer, true);
		var slider = objICAViewer.FindChildByXPath(xpathSlider, true);
		var slidervalue = aqConvert.VarToInt(slider.contentText);
		Log.Message("The FPS value is " + slidervalue);
		sliderPointer.Click();
		if(slidervalue>intExpectedValue)
		{
			for (var  i = 1; slidervalue > aqConvert.VarToInt(intExpectedValue) && i <= 90; i++)
			{
				var intMouseX = Sys.Desktop.MouseX;
				var intMousey = Sys.Desktop.MouseY;
				var intMouseToX = intMouseX;
				var intMouseToY = intMousey + 2;              
				LLPlayer.MouseDown(MK_LBUTTON, intMouseX, intMousey, 10);
				LLPlayer.MouseUp(MK_LBUTTON, intMouseToX, intMouseToY, 10);                            
				slidervalue = aqConvert.VarToInt(slider.contentText);
				Log.Message("Loop count for reducing Slider Value is = " + i);
			}
		}
		else if(slidervalue<intExpectedValue)
		{
			for (var i = 1; slidervalue < aqConvert.VarToInt(intExpectedValue) && i <= 90; i++)
			{
				var intMouseX = Sys.Desktop.MouseX;
				var intMousey = Sys.Desktop.MouseY;
				var intMouseToX = intMouseX;
				var intMouseToY = intMousey - 2;
				LLPlayer.MouseDown(MK_LBUTTON, intMouseX, intMousey, 10);
				LLPlayer.MouseUp(MK_LBUTTON, intMouseToX, intMouseToY, 10);
				slidervalue = aqConvert.VarToInt(slider.contentText);
				Log.Message("Loop count for increasing Slider Value is = " + i);
			}
		}
		Log.Message("The FPS is set as " + slidervalue);
	}