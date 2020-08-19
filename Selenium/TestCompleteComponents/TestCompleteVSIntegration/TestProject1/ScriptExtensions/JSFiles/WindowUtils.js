/*-----------------------------------------------------------------------------------------------------------------------
This library contains the below listed class and functions.
  - getChildObject(objectParentElement, enumChildElement): Returns the child element object under the given parent object
  - getChildObjects(objectParentElement, enumChildElement): Returns all the children element objects under the given parent object
  - isChildAvailable(objectParentElement, enumChildElement): Return the visibility status (true/false) of the child element
-----------------------------------------------------------------------------------------------------------------------*/

/*-----------------------------------------------------------------------------------------------------------------------
Function: getChildObject
Purpose:
  To return the child element object under the given parent object
-----------------------------------------------------------------------------------------------------------------------*/
function getChildObject(objectParentElement, enumChildElement, intTimeout){
if (intTimeout == undefined) intTimeout = 30000;	
  return objectParentElement.FindChildEx(enumChildElement.property, enumChildElement.value, enumChildElement.depth, true, intTimeout);
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: getChildObjects
Purpose:
  To return all the children element objects under the given parent object
-----------------------------------------------------------------------------------------------------------------------*/
function getChildObjects(objectParentElement, enumChildElement){
  if(isChildAvailable(objectParentElement, enumChildElement))
    return objectParentElement.FindAllChildren(enumChildElement.property, enumChildElement.value, enumChildElement.depth, true).toArray();
  else
    return null;
}

/*-----------------------------------------------------------------------------------------------------------------------
Function: isChildAvailable
Purpose:
  To return the visibility status (true/false) of the child element
-----------------------------------------------------------------------------------------------------------------------*/
function isChildAvailable(objectParentElement, enumChildElement){
  return objectParentElement.FindChildEx(enumChildElement.property, enumChildElement.value, enumChildElement.depth, true, 30000).Visible;
}