using System;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Selenium.Scripts.Reusable.Generic;
using TestStack.White;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using TestStack.White.UIItems.Finders;
using Selenium.Scripts.Pages.HoldingPen;


namespace Selenium.Scripts.Pages.Putty
{
    class Putty
    {
        //Fields
        string user;
        string host;
        string pwd;
        string rootPwd;
        string puttypath;
        int waitTime;
        int maxrecursionloop;

        public Putty()
        {
            user = Config.puttyuser;
            host = Config.HoldingPenIP;
            pwd = Config.puttypassword;
            rootPwd = Config.rootPwd;
            waitTime = 0;
            puttypath = Config.puttypath;
            maxrecursionloop = 0;
        }

        /// <summary>
        /// This method is to stop and retstart the emageon standalone service.
        /// </summary>
        public void RestartService()
        {
            //Launch Putty and login as Emageon user
            var startinfo = new ProcessStartInfo();
            startinfo.FileName = puttypath;
            startinfo.Arguments = string.Format("{0}@{1} -pw {2}", user, host, pwd);
            var process = new Process
            {
                StartInfo = startinfo
            };

            try
            {
                process.Start();
                WpfObjects wpf = new WpfObjects();
                WpfObjects._application = Application.Attach(process);
                wpf.GetMainWindow(host + " - PuTTY");

                //Check user looged in else throw exception
                this.CopyPuttyTextToClipboard();
                String logintext = System.Windows.Forms.Clipboard.GetText();
                int counterloginwait = 0;
                while (true)
                {
                    if ((logintext.Contains(user + "@")) && (!logintext.Contains("root@")) && (!logintext.Contains("> su")))
                    {
                        break;
                    }
                    else
                    {
                        Thread.Sleep(5000);
                        counterloginwait++;
                        this.CopyPuttyTextToClipboard();
                        logintext = System.Windows.Forms.Clipboard.GetText();
                    }
                    if (counterloginwait > 6)
                    {
                        break;
                    }
                }
                if ((logintext.Contains(user + "@")) && (!logintext.Contains("root@")) && (!logintext.Contains("> su")))
                {
                    this.SendKeysThruKeyBoard("su");
                }
                else
                {
                    Logger.Instance.ErrorLog("Error in logging in as user - " + user + " || Putty Content : " + logintext);
                    throw new Exception("Not able to Login Putty");
                }

                //Check if password is prompted
                this.CopyPuttyTextToClipboard();
                string passtext = System.Windows.Forms.Clipboard.GetText();
                int counterpass = 0;
                while (true)
                {
                    if (passtext.Contains("> su") && !passtext.Contains("root@") && passtext.Contains("Password:"))
                    {
                        break;
                    }
                    else
                    {
                        Thread.Sleep(5000);
                        counterpass++;
                        this.CopyPuttyTextToClipboard();
                        passtext = System.Windows.Forms.Clipboard.GetText();
                    }
                    if (counterpass > 6)
                    {
                        break;
                    }
                }
                if (passtext.Contains("> su") && !passtext.Contains("root@") && passtext.Contains("Password:"))
                {
                    this.SendKeysThruKeyBoard(rootPwd);
                }
                else
                {
                    Logger.Instance.ErrorLog("Error in switching user to root||Error in prompting for Password || Putty Content : " + passtext);
                    throw new Exception("Not able to Login Root user");
                }
                this.CopyPuttyTextToClipboard();
                String roottext = System.Windows.Forms.Clipboard.GetText();

                //Stop the service
                String requiredEndText = "";
                if (roottext.Contains("root"))
                {
                    requiredEndText = roottext.Substring(roottext.IndexOf("[root"), roottext.IndexOf("]#", roottext.IndexOf("[root")) - roottext.IndexOf("[root") + 3);
                    if (Config.HoldingPenVersion.StartsWith("11.2")) { this.SendKeysThruKeyBoard("rcemageon_standalone stop"); }
                    else { this.SendKeysThruKeyBoard("sh /etc/init.d/emageon_standalone stop"); }
                    //("sh RestartService.sh");
                }
                else
                {
                    Logger.Instance.ErrorLog("Error in switching user to root || Putty Content : " + requiredEndText);
                    return;
                }

                //Wait till the service is completely stopped
                String stopservicetext;// = System.Windows.Forms.Clipboard.GetText();
                Boolean StopCondition;

                do
                {
                    this.CopyPuttyTextToClipboard();
                    stopservicetext = System.Windows.Forms.Clipboard.GetText();
                    Thread.Sleep(5000);

                    //Condition for Holding pen 11.2 EA version
                    if (Config.HoldingPenVersion.StartsWith("11.2")) { StopCondition = stopservicetext.EndsWith("home]#\r\n"); }
                    else { StopCondition = stopservicetext.EndsWith("done\r\n" + requiredEndText + "\n"); }

                } while (!StopCondition && waitTime++ < 60);

                    if (waitTime >= 60)
                {
                    Logger.Instance.ErrorLog("Error in stopping the service emageon_standalone || Putty content : " + stopservicetext);
                    throw new Exception("Not able to Stop Service");
                }

                //Start Service and wait till it started completely
                    if (Config.HoldingPenVersion.StartsWith("11.2")) { this.SendKeysThruKeyBoard("rcemageon_standalone start"); }
                else { this.SendKeysThruKeyBoard("sh /etc/init.d/emageon_standalone start"); }
                waitTime = 0;
                String strtservicetext;
                Boolean StartCondition;
                
                do
                {
                    this.CopyPuttyTextToClipboard();
                    strtservicetext = System.Windows.Forms.Clipboard.GetText();
                    Thread.Sleep(5000);

                    //Condition for Holding pen 11.2 EA version
                    if (Config.HoldingPenVersion.StartsWith("11.2")) { StartCondition = strtservicetext.EndsWith("home]#\r\n"); }
                    else { StartCondition = strtservicetext.EndsWith("done\r\n" + requiredEndText + "\n"); }

                } while (!StartCondition && waitTime++ < 60);

                if (waitTime >= 60)
                {
                    Logger.Instance.ErrorLog("Error in starting the service emageon_standalone || Putty content : " + strtservicetext);
                    throw new Exception("Service Started");
                }
                Logger.Instance.InfoLog("INFO : Restarted the service emageon_standalone successfully");
                this.SendKeysThruKeyBoard("exit");
                if (!process.HasExited) { this.SendKeysThruKeyBoard("logout"); }                
                bool systemup = this.WaitTillHoldingPenUp();

                if ((!systemup) && (this.maxrecursionloop == 0))
                {
                    this.RestartOracleService();
                    this.maxrecursionloop++;
                    this.RestartService();                    
                }
            }

            catch (Exception e)
            {
                Logger.Instance.InfoLog("Exception in restarting service" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                if (!process.HasExited)
                    process.Kill();
                throw new Exception("Exception in restarting service in Putty", e);
            }
            finally
            {
                if (!process.HasExited)
                    process.Kill();
                BasePage.KillProcess("putty");
            }
        }

        /// <summary>
        /// This method is to copy the text in putty into clipboard
        /// </summary>
        public void CopyPuttyTextToClipboard()
        {
            System.Windows.Forms.Clipboard.Clear();
            TestStack.White.UIItems.WindowStripControls.MenuBar menu = WpfObjects._mainWindow.Get<TestStack.White.UIItems.WindowStripControls.MenuBar>(SearchCriteria.ByAutomationId("SystemMenuBar"));
            menu.Click();
            Thread.Sleep(500);
            SendKeysThruKeyBoard("o", false);
        }

        /// <summary>
        /// This method is to send keys to Putty terminal
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="pressEnter"></param>
        public void SendKeysThruKeyBoard(string keys, bool pressEnter = true)
        {
            try
            {
                //White.Core.InputDevices.AttachedKeyboard
                TestStack.White.InputDevices.AttachedKeyboard keyboard = WpfObjects._mainWindow.Keyboard;
                Thread.Sleep(1000);
                keyboard.Enter(keys);
                Thread.Sleep(1000);
                Logger.Instance.InfoLog("String keyed in in putty--" + keys);

                if (pressEnter)
                {
                    keyboard.PressSpecialKey(TestStack.White.WindowsAPI.KeyboardInput.SpecialKeys.RETURN);
                    Thread.Sleep(3000);
                    Logger.Instance.InfoLog("Enter key pressed in putty");
                }
            }
            catch (Exception e) { Logger.Instance.InfoLog("Exception while entering value in putty" + e); }
        }

        /// <summary>
        /// This method is to wait till Hoding pen is up after restarting the service
        /// </summary>
        public bool WaitTillHoldingPenUp()
        {
            int counter = 0;
            while (true)
            {

                try
                {
                    counter++;
                    Thread.Sleep(60000);
                    if (counter > 10)
                    {
                        return false;
                    }

                    //Launch Holding pen
                    BasePage.Driver.Navigate().GoToUrl("https://" + Config.HoldingPenIP + "/webadmin");             
                    if (((RemoteWebDriver)BasePage.Driver).Capabilities.BrowserName.ToLower().Contains("explorer"))
                    {
                        try
                        {
                            PageLoadWait.WaitForHPPageLoad(20);
                            Thread.Sleep(10000);
                            if (BasePage.Driver.Title.ToLower().Contains("certificate error"))
                            {
                               BasePage.Driver.Navigate().GoToUrl("javascript:document.getElementById('overridelink').click()");
                            }
                        }

                        catch (Exception e)
                        {
                            Logger.Instance.InfoLog("No Warning message thrown in IE");
                        }
                    }

                    //Check if element is found
                    if (BasePage.Driver.FindElement(By.CssSelector("div.imgTopRight table>tbody td>input[type='text']")).Displayed)
                    {
                        new HPLogin().LoginHPen(Config.hpUserName, Config.hpPassword);
                        if ((BasePage.Driver.PageSource).Contains("java.lang.NullPointerException"))
                        {  
                            new HPLogin().LogoutHPen();
                            Logger.Instance.InfoLog("Issue with Oracle Startup, removing table from backup mode");
                            return false;
                        }
                        else if (BasePage.Driver.FindElement(By.CssSelector("body> table:nth-child(1) > tbody > tr:nth-child(3) > td > table > tbody > tr > td:nth-child(2) > a")).Displayed)
                        {
                            new HPLogin().LogoutHPen();
                            Logger.Instance.InfoLog("Holding pen is up after restarting services");
                            return true;                            
                        }
                        else
                        {
                            new HPLogin().LogoutHPen();
                            Logger.Instance.InfoLog("Waiting for Holding up to be up after restarting services");
                        }
                                                                 
                    }
                }
                catch (Exception e)
                {
                    Logger.Instance.InfoLog("Waiting for Holding up to be up after restarting services");
                }
            }



        }

        public void RestartOracleService()
        {
            //Login into putty as root users
            Logger.Instance.InfoLog("Logging into putty and sqlplus to remove table from backup mode");
            this.LoginPutty();

            this.CopyPuttyTextToClipboard();
            String roottext = System.Windows.Forms.Clipboard.GetText();

            //swicth to oracle  user
            String requiredEndText = "";
            if (roottext.Contains("root"))
            {
                this.SendKeysThruKeyBoard("su oracle");
            }
            else
            {
                Logger.Instance.ErrorLog("Error in switching user to root || Putty Content : " + requiredEndText);
                return;
            }

            //Wait till User is switched
            this.CopyPuttyTextToClipboard();
            roottext = System.Windows.Forms.Clipboard.GetText();
            requiredEndText = roottext.Substring(roottext.IndexOf("[oracle"), roottext.IndexOf("]$", roottext.IndexOf("[oracle")) - roottext.IndexOf("[oracle") + 3);
            waitTime = 0;
            while (!roottext.Contains(requiredEndText) && waitTime++ < 100)
            {
                this.CopyPuttyTextToClipboard();
                roottext = System.Windows.Forms.Clipboard.GetText();                
            }
            if (waitTime >= 100) { throw new Exception("Not switched to Oracle user"); }


            //enter into sqlplus
            this.SendKeysThruKeyBoard("sqlplus '/ as sysdba'");
            this.CopyPuttyTextToClipboard();
            roottext = System.Windows.Forms.Clipboard.GetText();
            waitTime = 0;
            while (!roottext.Contains("SQL>") && waitTime++ < 100)
            {
                this.CopyPuttyTextToClipboard();
                roottext = System.Windows.Forms.Clipboard.GetText();
            }
            if (waitTime >= 100) { throw new Exception("Not switched to Oracle user"); }

            this.SendKeysThruKeyBoard("set lines 150;");
            this.SendKeysThruKeyBoard("select distinct 'alter tablespace ' || t.name || ' end backup;' from v$backup b, v$tablespace t, v$datafile d where b.file# = d.file# and d.ts# = t.ts# and b.status = 'ACTIVE';");

            waitTime = 0;
            this.CopyPuttyTextToClipboard();
            roottext = System.Windows.Forms.Clipboard.GetText();
            while (!roottext.EndsWith("SQL>\r\n") && waitTime++ < 100)
            {
                this.CopyPuttyTextToClipboard();
                roottext = System.Windows.Forms.Clipboard.GetText();
            }

            if (roottext.Contains("no rows selected"))
            { BasePage.KillProcess("putty"); return; }
            else
            {
                //String sql = roottext.Substring((roottext.IndexOf("\nalter tablespace")), (roottext.IndexOf("SQL>", roottext.IndexOf("\nalter tablespace"))-3));
                String sql = roottext.Substring((roottext.IndexOf("\nalter tablespace")), (roottext.IndexOf("SQL>", roottext.IndexOf("\nalter tablespace")) - roottext.IndexOf("\nalter tablespace") - 4));
                Logger.Instance.InfoLog(sql);
                this.SendKeysThruKeyBoard(sql.Substring(1));

                //wait till sql is executed
                Thread.Sleep(2);
                waitTime = 0;
                this.CopyPuttyTextToClipboard();
                roottext = System.Windows.Forms.Clipboard.GetText();
                while (!roottext.EndsWith("SQL>\r\n") && waitTime++ < 5)
                {
                    Thread.Sleep(2);
                    this.CopyPuttyTextToClipboard();
                    roottext = System.Windows.Forms.Clipboard.GetText();
                }
            }
    
            //Close Putty
            BasePage.KillProcess("putty");
        }

        public void LoginPutty()
        {

            //Launch Putty and login as Emageon user
            var startinfo = new ProcessStartInfo();
            startinfo.FileName = puttypath;
            startinfo.Arguments = string.Format("{0}@{1} -pw {2}", user, host, pwd);
            var process = new Process
            {
                StartInfo = startinfo
            };

                process.Start();
                WpfObjects wpf = new WpfObjects();
                WpfObjects._application = Application.Attach(process);
                wpf.GetMainWindow(host + " - PuTTY");

                //Check user looged in else throw exception
                this.CopyPuttyTextToClipboard();
                String logintext = System.Windows.Forms.Clipboard.GetText();
                int counterloginwait = 0;
                while (true)
                {
                    if ((logintext.Contains(user + "@")) && (!logintext.Contains("root@")) && (!logintext.Contains("> su")))
                    {
                        break;
                    }
                    else
                    {
                        Thread.Sleep(5000);
                        counterloginwait++;
                        this.CopyPuttyTextToClipboard();
                        logintext = System.Windows.Forms.Clipboard.GetText();
                    }
                    if (counterloginwait > 6)
                    {
                        break;
                    }
                }

                if ((logintext.Contains(user + "@")) && (!logintext.Contains("root@")) && (!logintext.Contains("> su")))
                {
                    this.SendKeysThruKeyBoard("su");
                }
                else
                {
                    Logger.Instance.ErrorLog("Error in logging in as user - " + user + " || Putty Content : " + logintext);
                    throw new Exception("Not able to Login Putty");
                }

                //Check if password is prompted
                this.CopyPuttyTextToClipboard();
                string passtext = System.Windows.Forms.Clipboard.GetText();
                int counterpass = 0;
                while (true)
                {
                    if (passtext.Contains("> su") && !passtext.Contains("root@") && passtext.Contains("Password:"))
                    {
                        break;
                    }
                    else
                    {
                        Thread.Sleep(5000);
                        counterpass++;
                        this.CopyPuttyTextToClipboard();
                        passtext = System.Windows.Forms.Clipboard.GetText();
                    }
                    if (counterpass > 6)
                    {
                        break;
                    }
                }
                if (passtext.Contains("> su") && !passtext.Contains("root@") && passtext.Contains("Password:"))
                {
                    this.SendKeysThruKeyBoard(rootPwd);
                }
                else
                {
                    Logger.Instance.ErrorLog("Error in switching user to root||Error in prompting for Password || Putty Content : " + passtext);
                    throw new Exception("Not able to Login Root user");
                }

                this.CopyPuttyTextToClipboard();
                String roottext = System.Windows.Forms.Clipboard.GetText();                
                String requiredEndText = "";
                if (roottext.Contains("root"))
                {
                    Logger.Instance.InfoLog("Root User Logged in");
                }
                else
                {
                    Logger.Instance.ErrorLog("Error in switching user to root || Putty Content : " + requiredEndText);
                    return;
                }            
        }

        public void EA_Cleanup()
        {
            //Launch Putty and login as Emageon user
            var startinfo = new ProcessStartInfo();
            startinfo.FileName = puttypath;
            startinfo.Arguments = string.Format("{0}@{1} -pw {2}", user, host, pwd);
            var process = new Process
            {
                StartInfo = startinfo
            };

            try
            {
                process.Start();
                WpfObjects wpf = new WpfObjects();
                WpfObjects._application = Application.Attach(process);
                wpf.GetMainWindow(host + " - PuTTY");

                //Check user looged in else throw exception
                this.CopyPuttyTextToClipboard();
                String logintext = System.Windows.Forms.Clipboard.GetText();
                int counterloginwait = 0;
                while (true)
                {
                    if ((logintext.Contains(user + "@")) && (!logintext.Contains("root@")) && (!logintext.Contains("> su")))
                    {
                        break;
                    }
                    else
                    {
                        Thread.Sleep(5000);
                        counterloginwait++;
                        this.CopyPuttyTextToClipboard();
                        logintext = System.Windows.Forms.Clipboard.GetText();
                    }
                    if (counterloginwait > 6)
                    {
                        break;
                    }
                }
                if ((logintext.Contains(user + "@")) && (!logintext.Contains("root@")) && (!logintext.Contains("> su")))
                {
                    this.SendKeysThruKeyBoard("su");
                }
                else
                {
                    Logger.Instance.ErrorLog("Error in logging in as user - " + user + " || Putty Content : " + logintext);
                    throw new Exception("Not able to Login Putty");
                }

                //Check if password is prompted
                this.CopyPuttyTextToClipboard();
                string passtext = System.Windows.Forms.Clipboard.GetText();
                int counterpass = 0;
                while (true)
                {
                    if (passtext.Contains("> su") && !passtext.Contains("root@") && passtext.Contains("Password:"))
                    {
                        break;
                    }
                    else
                    {
                        Thread.Sleep(5000);
                        counterpass++;
                        this.CopyPuttyTextToClipboard();
                        passtext = System.Windows.Forms.Clipboard.GetText();
                    }
                    if (counterpass > 6)
                    {
                        break;
                    }
                }
                if (passtext.Contains("> su") && !passtext.Contains("root@") && passtext.Contains("Password:"))
                {
                    this.SendKeysThruKeyBoard(rootPwd);
                }
                else
                {
                    Logger.Instance.ErrorLog("Error in switching user to root||Error in prompting for Password || Putty Content : " + passtext);
                    throw new Exception("Not able to Login Root user");
                }
                this.CopyPuttyTextToClipboard();
                String roottext = System.Windows.Forms.Clipboard.GetText();


                //cleanup
                
                String requiredEndText = "";
                if (roottext.Contains("root"))
                {
                    requiredEndText = roottext.Substring(roottext.IndexOf("[root"), roottext.IndexOf("]#", roottext.IndexOf("[root")) - roottext.IndexOf("[root") + 3);
                    this.SendKeysThruKeyBoard("cd /usr/bin");
                }
                else
                {
                    Logger.Instance.ErrorLog("Error in switching user to root || Putty Content : " + requiredEndText);
                    return;
                }

                this.CopyPuttyTextToClipboard();
                String roottext1 = System.Windows.Forms.Clipboard.GetText();

                String requiredEndText1 = "";
                if (roottext1.Contains("bin"))
                {
                    requiredEndText1 = roottext1.Substring(roottext.IndexOf("[root"), roottext1.IndexOf("]#", roottext1.IndexOf("bin]")) - roottext1.IndexOf("[root") + 3);
                    this.SendKeysThruKeyBoard("./cleanup.pl");
                }
                else
                {
                    Logger.Instance.ErrorLog("Error in switching  to bin || Putty Content : " + requiredEndText1);
                    return;
                }
                //String requiredEndText2 = "";
                 //int count = 0;
                while (true)
                {
                    Thread.Sleep(20000);
                    this.CopyPuttyTextToClipboard();
                    String roottext2 = System.Windows.Forms.Clipboard.GetText();
                    //requiredEndText2 = roottext2.Substring(roottext2.IndexOf("EA Version"), roottext2.IndexOf("]#", roottext2.IndexOf("EA Version")) - roottext2.IndexOf("[root") + 3);
                    if (roottext2.Contains("EA Version"))
                    {
                        break;
                    }
                }

            }

            catch (Exception e)
            {
                Logger.Instance.InfoLog("Exception in cleaning up EA" + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine + e.InnerException);
                if (!process.HasExited)
                    process.Kill();
                throw new Exception("Exception in cleaning up EA in Putty", e);
            }
            finally
            {
                if (!process.HasExited)
                    process.Kill();
                BasePage.KillProcess("putty");
            }
        }
    }
}