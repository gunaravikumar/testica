using System;
using System.IO;

namespace WindowsFormsApplication1
{
    internal sealed class Logger
    {

        #region Private Members

        private static Logger _mLogger;
        private string _mFilePath;

        #endregion Private Members

        #region Constructor

        /// <summary>
        ///     This is the constructor method which appends the log file name with the timestamp to make it unique
        /// </summary>
        private Logger()
        {
        }

        #endregion Constructor

        #region Static Members

        public static string LogFilePath = string.Empty;

        public static Logger Instance
        {
            get { return _mLogger ?? (_mLogger = new Logger()); }
        }

        #endregion Static Members

        #region Public Methids

        public void Initialize(string folderPath)
        {
            try
            {
                string datetimeString = string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
                string fileName = "log-" + datetimeString + ".log";
                _mFilePath = folderPath.TrimEnd('\\') + "\\" + fileName;
                LogFilePath = _mFilePath;
            }
            catch (Exception ex)
            {
                Instance.ErrorLog("Exception encountered due to : " + ex);
            }
        }

        /// <summary>
        ///     This is the method which adds an ERROR log to the log file with the time
        /// </summary>
        /// <param name="debugMsg">The message that needs to be logged as ERROR in the file</param>
        public void ErrorLog(string debugMsg)
        {
            Log("ERROR", debugMsg);
        }

        /// <summary>
        ///     This is the method which adds a INFORMATION log to the log file with the time
        /// </summary>
        /// <param name="debugMsg">The message that needs to be logged as INFO in the file</param>
        public void InfoLog(string debugMsg)
        {
            Log("INFO", debugMsg);
        }

        /// <summary>
        ///     This is the method which adds a WARNING log to the log file with the time
        /// </summary>
        /// <param name="debugMsg">The message that needs to be logged as WARN in the file</param>
        public void WarnLog(string debugMsg)
        {
            Log("WARNING", debugMsg);
        }

        #endregion Public Methids

        #region Private Methods

        private void Log(string logType, string message)
        {
            try
            {
                using (var sw = new StreamWriter(_mFilePath, true))
                {
                    sw.WriteLine(logType + "  :   " + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) +
                                 " :  " + message);
                    Console.WriteLine(logType + "  :   " + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) +
                                " :  " + message);
                }
            }
            catch (Exception e) { }
        }

        #endregion Private Methods

    }
}