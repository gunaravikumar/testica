/*-----------------------------------------------------------------------------------------------------------------------
Function: getXYCoordinates
Purpose:
  To get X,Y coordinates of the element to move to another element.
Parameters:
  objDestinationTable: Destination table object
  objSourceTable: Source table object
Return Value:
  returns array of XY coordinates
Remarks:
  <Enter any useful remarks about the function here>.
Modifications:
  <mm/dd/yyyy>, <first name, last name>: <Brief description of the modification(s)>.
-----------------------------------------------------------------------------------------------------------------------*/
function getXYCoordinates(objDestinationTable, objSourceTable) {
  try {
    var arrCoordinates = [];
    //var intToX = (objDestinationTable.ScreenLeft + (objSourceTable.Width * 7 / 10)) - (objDestinationTable.ScreenLeft + objDestinationTable.Width / 2)
    var intToX = (objDestinationTable.ScreenLeft + (objDestinationTable.Width * 1 / 25)) - (objSourceTable.ScreenLeft + objSourceTable.Width / 2)
    var intToY = (objDestinationTable.ScreenTop + (objDestinationTable.Height - 50)) - (objSourceTable.ScreenTop + objSourceTable.Height / 2)
    arrCoordinates.push(intToX);
    arrCoordinates.push(intToY);
    return arrCoordinates;
  }
  catch(ex) {
    throw Logger.Error("Error while getting XY coordinates", ex);
  }
}

