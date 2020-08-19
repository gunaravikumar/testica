using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Selenium.Scripts.Reusable.Generic
{
    class PauseTimer
    {

        System.Timers.Timer mytimer = new System.Timers.Timer();
        System.Threading.ManualResetEvent oSignalEvent = new System.Threading.ManualResetEvent(false);
        public void PauseExecution(int timeoutmin)
        {
            int timeout = timeoutmin * 60000;
            mytimer.Interval = timeout;
            mytimer.Enabled = true;
            mytimer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            mytimer.Start();
            Logger.Instance.InfoLog("PauseExecution started");
            oSignalEvent.WaitOne();
            Logger.Instance.InfoLog("PauseExecution Stoped");
        }

        public void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            mytimer.Stop();
            Logger.Instance.InfoLog("timer_Elapsed called");
            oSignalEvent.Set();
        }
    }
}
