using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Selenium.Scripts.Pages;

namespace Selenium.Scripts.Reusable.Generic  
{
    public class NetStat
    {

        /// <summary>
        /// Port class which has the details of port nuber, protocol state and other info
        /// </summary>
        public class Port
        {
            public string name
            {
                get
                {
                    return string.Format("{0} ({1} port {2})", this.process_name, this.protocol, this.port_number);
                }
                set { }
            }
            public string port_number { get; set; }
            public string process_name { get; set; }
            public string protocol { get; set; }
            public string state { get; set; }
            public string pid { get; set; }
        }

        /// <summary>
        /// Gets the list of Active Ports
        /// </summary>
        /// <returns></returns>
        public List<Port> GetNetStatPorts()
        {
            var Ports = new List<Port>();

            try
            {
                using (Process p = new Process())
                {

                    ProcessStartInfo ps = new ProcessStartInfo();
                    ps.Arguments = "-a -n -o";
                    ps.FileName = "netstat.exe";
                    ps.UseShellExecute = false;
                    ps.WindowStyle = ProcessWindowStyle.Hidden;
                    ps.RedirectStandardInput = true;
                    ps.RedirectStandardOutput = true;
                    ps.RedirectStandardError = true;

                    p.StartInfo = ps;
                    p.Start();

                    StreamReader stdOutput = p.StandardOutput;
                    StreamReader stdError = p.StandardError;

                    string content = stdOutput.ReadToEnd() + stdError.ReadToEnd();
                    string exitStatus = p.ExitCode.ToString();

                    if (exitStatus != "0")
                    {
                        // Command Errored. Handle Here If Need Be
                    }

                    //Get The Rows
                    string[] rows = Regex.Split(content, "\r\n");
                    foreach (string row in rows)
                    {
                        //Split it baby
                        string[] tokens = Regex.Split(row, "\\s+");
                        if (tokens.Length > 4 && (tokens[1].Equals("UDP") || tokens[1].Equals("TCP")))
                        {
                            string localAddress = Regex.Replace(tokens[2], @"\[(.*?)\]", "1.1.1.1");
                            Ports.Add(new Port
                            {
                                protocol = localAddress.Contains("1.1.1.1") ? String.Format("{0}v6", tokens[1]) : String.Format("{0}v4", tokens[1]),
                                port_number = localAddress.Split(':')[1],
                                process_name = tokens[1] == "UDP" ? LookupProcess(Convert.ToInt16(tokens[4])) : LookupProcess(Convert.ToInt16(tokens[5])),
                                state = tokens[4],
                                pid = tokens[5]
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Ports;
        }

        /// <summary>
        /// Returns the Process Name for a Process ID
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static string LookupProcess(int pid)
        {
            string procName;
            try { procName = Process.GetProcessById(pid).ProcessName; }
            catch (Exception) { procName = "-"; }
            return procName;
        }

    }



}
