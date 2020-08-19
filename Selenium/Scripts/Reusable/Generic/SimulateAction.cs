using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;
using Selenium.Scripts.Pages;
using Selenium.Scripts.Pages.iConnect;

namespace Selenium.Scripts.Reusable.Generic
{
    /// <summary>
    /// This helper class will create Synthetic events by firing java script
    /// This is based on Simulate jquery
    /// https://github.com/j-ulrich/jquery-simulate-ext
    /// </summary>
    public class SimulateAction
    {
        //Fields
        IWebDriver driver = null;
        IJavaScriptExecutor jsexecutor = null;

        /// <summary>
        /// Constructor -- Loading JS Files   
        /// </summary>
        /// <param name="driver">BasePage.Driver</param>
        public SimulateAction(IWebDriver driver)
        {
            this.driver = driver;
            jsexecutor = (IJavaScriptExecutor)this.driver;
            String simulateextjs1 = File.ReadAllText("OtherFiles\\Simulate\\bililiteRange.js");
            String simulateextjs2 = File.ReadAllText("OtherFiles\\Simulate\\jquery.simulate.js");
            String simulateextjs3 = File.ReadAllText("OtherFiles\\Simulate\\jquery.simulate.ext.js");
            String simulateextjs4 = File.ReadAllText("OtherFiles\\Simulate\\jquery.simulate.drag-n-drop.js");
            String simulateextjs5 = File.ReadAllText("OtherFiles\\Simulate\\jquery.simulate.key-sequence.js");
            String simulateextjs6 = File.ReadAllText("OtherFiles\\Simulate\\jquery.simulate.key-combo.js");

            ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(simulateextjs1);
            ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(simulateextjs2);
            ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(simulateextjs3);
            ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(simulateextjs4);
            ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(simulateextjs5);
            ((IJavaScriptExecutor)BasePage.Driver).ExecuteScript(simulateextjs6);
        }

        /// <summary>
        /// This is to simulate Click action
        /// </summary>
        /// <param name="cssSelector"></param>
        public void Click(String cssSelector)
        {
            String script = "jQuery(" + "\"" + cssSelector + "\"" + ").simulate(\"click\", {bubbles:true, cancelable:true})";
            jsexecutor.ExecuteScript(script);
        }

        /// <summary>
        /// Perform Ctrl Click
        /// </summary>
        /// <param name="cssSelector"></param>
        public void CtrlClick(String cssSelector)
        {
            String script = "jQuery(" + "\"" + cssSelector + "\"" + ").simulate(\"click\", {ctrlKey:true})";
            jsexecutor.ExecuteScript(script);
        }

        /// <summary>
        /// Simulate Mouse Right click
        /// <Test>Tested working on HTML5 only(BluRinViewer)</Test>
        /// </summary>
        /// <param name="cssSelector"></param>
        public void ContextClick(String cssSelector)
        {
            String script = "jQuery(" + "\"" + cssSelector + "\"" + ").simulate(\"contextmenu\", {bubbles:true, cancelable:true})";
            jsexecutor.ExecuteScript(script);
            Thread.Sleep(2000);

        }        

        /// <summary>
        /// This method will perform Drag and drop action
        /// Tested working on Draging and Dropping Tools in Domain/Role
        /// </summary>
        /// <param name="cssSelector"></param>
        public void DrangAndDrop(String sourcecssSelector, String targetcssSelector)
        {
            String dragscript = "jQuery(" + "\"" + sourcecssSelector + "\"" + ").simulate(\"drag\");";
            String dropscript = "jQuery(" + "\"" + targetcssSelector + "\"" + ").simulate(\"drop\");";
            jsexecutor.ExecuteScript(dragscript);
            jsexecutor.ExecuteScript(dropscript);
            Thread.Sleep(4000);
        }

        /// <summary>
        /// This method will perform Double click on specified element       
        /// </summary>
        /// <param name="cssSelector"></param>
        public void DoubleClick(String cssSelector)
        {
            String script = "jQuery(" + "\"" + cssSelector + "\"" + ").simulate(\"dblclick\");";            
            jsexecutor.ExecuteScript(script);            
            Thread.Sleep(4000);
        }
    }
}  
