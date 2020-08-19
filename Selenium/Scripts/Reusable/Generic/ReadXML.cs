using System;
using System.Xml;
using System.Collections.Generic;

namespace Selenium.Scripts.Reusable.Generic
{   
    public class ReadXML
    {  
       /// <This funcion will read the data from XMl file>
        /// 
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <param name="node"></param>
        /// <returns></returns>
       public static Dictionary<string, string> ReadDataXML(string xmlPath, string node)
        {
            try
            {
                Dictionary<string, string> values = new Dictionary<string, string>();
                var doc = new XmlDocument();
                doc.Load(xmlPath);
                var nodevalues = doc.SelectNodes(node);

                foreach (XmlNode member in nodevalues)
                {
                    foreach (var child in member.ChildNodes)
                    {
                        var element = (XmlElement)child;
                        string nodeName = element.Name;
                        string value = element.InnerXml;
                        values.Add(nodeName, value);
                    }
                }
                return values;                    
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step ReadDataXML due to : " + ex);
                return null;
            }
        }
       
       /// <summary>
        /// This method is to update an existing xml file
        /// </summary>
        /// <param name="xmlpath">Path of xml file to update</param>
        /// <param name="keyvalue">node and value list to be updated</param>
       public static void UpdateXML(String xmlpath, Dictionary<String, String> nodevalues, String filterParamter = "", String filtervalue = "")
       {
           XmlDocument doc = new XmlDocument();
           doc.Load(xmlpath);

           foreach (String node in nodevalues.Keys)
           {
               XmlNodeList elements = doc.GetElementsByTagName(node);
               foreach (XmlNode xnode in elements)
               {

                   if (String.IsNullOrEmpty(filterParamter))
                   {
                       xnode.InnerXml = nodevalues[node];
                   }
                   else
                   {
                       if (xnode.Attributes[filterParamter].Value.Equals(filtervalue))
                       { 
                           xnode.InnerXml = nodevalues[node]; 
                       }
                   }
               }
           }
           doc.Save(xmlpath);
       }                
                   
       public static String ReadAttribute(String xmlpath, string node, String attribute)
        {  
            try
            {
                //Load XMl
                var doc = new XmlDocument();
                doc.Load(xmlpath);
                XmlNodeList elemList = doc.GetElementsByTagName(node);
                string attrVal = null;
                for (int i = 0; i < elemList.Count; i++)
                {
                    attrVal = elemList[i].Attributes[attribute].Value;
                }
                return attrVal;
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Exception in step ReadDataXML due to : " + ex);
                return null;
            }

        }
    }
}