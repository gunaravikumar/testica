using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Selenium.Scripts.Pages;

namespace Selenium.Scripts.Reusable.Generic
{
    class VideoCaptureUtil
    {
        public static void StartVideoCapture(string VideoLoc)
        {
            string command = string.Empty;
            try
            {
                //Killprocess("vlc");
                BasePage.KillProcess("vlc");
                if (File.Exists(VideoLoc))
                {
                    File.Delete(VideoLoc);
                }

                command = "\"--screen-mouse-image="+ Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())) + Path.DirectorySeparatorChar + "OtherFiles\\mouse.png\" screen:// --one-instance -I dummy --dummy-quiet --extraintf rc --rc-host localhost:8082 --rc-quiet --screen-follow-mouse --no-video :screen-fps=15 :screen-caching=300 --sout \"#transcode{vcodec=h264,vb=800,fps=5,scale=1,acodec=none}:duplicate{dst=std{access=file,mux=mp4,dst=" + VideoLoc + "}}\"";
                Logger.Instance.InfoLog(command);
                ExecuteCommand(command);
                Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Problem in executing command " + command + " due to " + ex.ToString());
            }
        }

        public static void StopVideoCapture()
        {
            string command = string.Empty;
            try
            {

                command = "--one-instance vlc://quit";
                Logger.Instance.InfoLog(command);
                ExecuteCommand(command);
                Thread.Sleep(5000);
                BasePage.KillProcess("vlc");
                //Killprocess("vlc");
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Problem in executing command " + command + " due to " + ex.ToString());
            }
        }


        private static void ExecuteCommand(string command)
        {
            try
            {

                Process p = new Process();
                p.StartInfo.FileName = "\"C:\\Program Files (x86)\\VideoLAN\\VLC\\vlc.exe\"";
                p.StartInfo.Arguments = command;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Problem in executing command " + command + " due to " + ex.ToString());
            }
        }

        private static void Killprocess(string name)
        {
            try
            {

                foreach (Process proc in Process.GetProcessesByName(name))
                {
                    Logger.Instance.InfoLog(name);
                    proc.Kill();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Problem in killing process " + name + " due to " + ex.ToString());
            }
        }

    }
}
