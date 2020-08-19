using System.Collections.Generic;
using System.Linq;
using TestStack.White.UIItems.Finders;
using Application = TestStack.White.Application;
using Button = TestStack.White.UIItems.Button;
using TestStack.White.UIItems.WindowItems;
using System.Windows.Automation;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace WindowsFormsApplication1
{
    class HandleWerFault
    {
        static void Main(string[] args)
        {
            String logfilepath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + "\\LogFolder";
            Directory.CreateDirectory(logfilepath);
            Logger.Instance.Initialize(logfilepath);
            Logger.Instance.InfoLog("Log folder locations is : " + logfilepath);
            while (1 > 0)
            {
                Thread.Sleep(180000);
                Logger.Instance.InfoLog("Started execution of handling wefault process");
                foreach (Process p in Process.GetProcessesByName("WerFault"))
                {
                    try
                    {
                        Application myApp = Application.Attach("WerFault");
                        List<Window> myWindows = myApp.GetWindows();
                        foreach (Window mywindow in myWindows)
                        {
                            if (mywindow.Name.Contains("Internet Explorer"))
                            {
                                Logger.Instance.InfoLog("WerFault for IE Crash process found");
                                Button closebutton = mywindow.Get<Button>(SearchCriteria.ByControlType(ControlType.Button).AndByText("Close the program"));
                                if (closebutton.Visible)
                                {
                                    closebutton.Click();
                                    Logger.Instance.InfoLog("WerFault for IE Crash process closed successfully");
                                    break;
                                }
                                else
                                    Logger.Instance.ErrorLog("IE Crash unable to close");
                            }
                            else if (mywindow.Name.Equals("Command line server for the IE driver"))
                            {
                                Logger.Instance.InfoLog("WerFault for IE Driver Crash process found");
                                Button minimizebutton = mywindow.Get<Button>(SearchCriteria.ByControlType(ControlType.Button).AndByText("Minimize"));
                                if (minimizebutton.Visible || minimizebutton.Enabled)
                                {
                                    try
                                    {
                                        minimizebutton.Click();
                                        Logger.Instance.InfoLog("WerFault process for IE Driver Crash minimized successfully");
                                    }
                                    catch (Exception et)
                                    {
                                        Logger.Instance.InfoLog("WerFault process for IE Driver Crash already minimized");
                                    }
                                }
                                else
                                    Logger.Instance.ErrorLog("IEDriver Crash unable to minimize");
                            }
                            else
                            {
                                Logger.Instance.ErrorLog("Unknown Window found, with Window name : " + mywindow.Name);
                                try
                                {
                                    Button minimizebutton = mywindow.Get<Button>(SearchCriteria.ByControlType(ControlType.Button).AndByText("Minimize"));
                                    if (minimizebutton.Visible)
                                    {
                                        try
                                        {
                                            minimizebutton.Click();
                                            Logger.Instance.InfoLog("Unknown Window minimized successfully");
                                        }
                                        catch (Exception et)
                                        {
                                            Logger.Instance.InfoLog("Unknown Window already minimized");
                                        }
                                    }
                                    else
                                        Logger.Instance.ErrorLog("Unknown Window unable to minimize");
                                }
                                catch (Exception e)
                                {
                                    Logger.Instance.ErrorLog("Unable to minimize Unknown Window , Exception at : " + e.StackTrace);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.ErrorLog("Exception while handling WerFault Error Reporting window due to : " + e.StackTrace);
                    }
                }
            }
        }
    }
}
