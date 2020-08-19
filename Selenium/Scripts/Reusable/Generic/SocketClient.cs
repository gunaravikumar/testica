using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;
using System.Net.Sockets;

namespace Selenium.Scripts.Reusable.Generic
{
    public class SocketClient
    {
        public const int UNKNOWN = 0000;
        public const int INITIALIZE = 1000;
        public const int ACKNOWLGEMENT = 1001;
        public const int CLARC_STOP = 1002;
        public const int CLARC_START = 1003;
        public const int CLARC_RESTART = 1004;
        public const int EMAGEON_STOP = 1005;
        public const int EMAGEON_START = 1006;
        public const int EMAGEON_RESTART = 1007;
        public const int HAREGISTRY_STOP = 1008;
        public const int HAREGISTRY_START = 1009;
        public const int HAREGISTRY_RESTART = 1010;
        public const int DB2_CONNECT = 1011;
        public const int CHECK_IMG_AVAILABLE = 1012;
        public const int GET_EA_SERVER_VERSION = 1013;
        public const int GET_ROUTING_STATUS = 1014;
        public const int GET_HL7ORDER_STATUS = 1015;
        public const int EA_CLEAN_DB_STORED_FILES = 1016;
        public const int EA_VERIFY_PACS_STUDY_NOTIFICATION = 1017;
        public const int EA_VERIFY_IE_MOVE_PACS_NOTIFICATION = 1018;
        public const int EA_VERIFY_IE_STORE_PACS_NOTIFICATION = 1019;
        public const int EA_VERIFY_IE_UPDATE_PACS_NOTIFICATION = 1020;
        public const int EA_VERIFY_IE_DELETE_PACS_NOTIFICATION = 1021;
        public const int GET_HL7ORDER_STATUS_FROM_ARCHIVE = 1022;
        public const int EA_GET_STORE_IMAGE_STATUS = 1023;
        public const int GET_HL7ORDER_MULTICASTER_STATUS = 1024;
        public const int CLEAN_CARDIO_DATABASE = 1025;
        public const int GET_MERGE_HL7ORDER_MULTICASTER_STATUS = 1026;
        public const int GET_VERICIS_STORE_NOTIFICATION = 1027;
        public const int ENABLE_EA_DELETOR_PLUGIN = 1028;
        public const int DISABLE_EA_DELETOR_PLUGIN = 1029;
        public const int GET_VERICIS_UPDATE_NOTIFICATION = 1030;
        public const int GET_VERICIS_DELETE_NOTIFICATION = 1031;
        public const int GET_VERICIS_DELETOR_PLUGIN_STATUS = 1032;
        public const int GET_VERICIS_DELETOR_PLUGIN_NOTIFICAION = 1033;
        public const int GET_VERICIS_MERGE_NOTIFICAION_STATUS = 1034;
        public const int GET_IMAGE_SHARING_STORE_NOTIFICAION_STATUS = 1035;
        public const int GET_IMAGE_SHARING_DELETE_NOTIFICAION_STATUS = 1036;
        public const int GET_IMAGE_SHARING_RECONCILIATION_NOTIFICAION_STATUS = 1037;
        public const int GET_IMAGE_SHARING_NO_ORDER_FOUND_RECONCILIATION_STATUS = 1038;
        public const int SET_MULTIPLE_ORDER_PROPERTY = 1039;
        public const int GET_IMAGE_SHARING_ROUTING_STARTED_STATUS = 1040;
        public const int GET_IMAGE_SHARING_ROUTING_COMPLETED_STATUS = 1041;
        public const int GET_IMAGE_SHARING_ROUTING_FAILED_STATUS = 1042;
        public const int GET_IMAGE_SHARING_SENT_NO_ORDER_FOUND_RECONCILIATION_STATUS = 1043;
        public const int GET_IMAGE_SHARING_MULTIPLE_ORDER_FOUND_RECONCILIATION_STATUS = 1044;
        public const int GET_IMAGE_SHARING_MULTIPLE_ORDER_RECONCILIATION_STATUS = 1045;
        public const int SET_MULTIPLE_ORDER_PROPERTY_TO_DEFAULT = 1046;


        //Pacs Related Variables
        public const int RESTART_AMICAS_SERVICE = 7001;
        public const int STOP_AMICAS_SERVICE = 7003;
        public const int START_AMICAS_SERVICE = 7004;
        public const int CLEAN_PACS_STUDIES = 7005;
        public const int DELETE_PACS_STUDIES = 7006;

        //ICA Related Variables
        public const int CLEAN_ICA_DATABASE = 8001;

        //MPI Related Variables
        public const int DICOM_QUERY_RETRIEVE = 1047;
        public const int PROXY_QUERY_RETRIEVE = 1048;
        public const int DICOM_QUERY_RULE_RETRIEVE = 1049;

        //Morpher Related Variables
        public const int DICOM_DEFAULT_MORPHER_PROPERTIES_CHANGES = 1050;
        public const int CHECK_QUERY_MORPHER = 1051;
        public const int INPUT_MORPHER_PROPERTIES_CHANGE = 1052;
        public const int MORPHER_CHECK_PRIOR_VALUES = 1053;
        public const int MORPHER_ORDER_LOGGING = 1054;
        public const int VERIFY_INJECT_TAG_DCM_DUMP = 1055;
        public const int VERIFY_TAG_EXISTS_DCM_DUMP = 1056;
        public const int VERIFY_TAG_LENGTH_DCM_DUMP = 1057;
        public const int REMOVE_DEFAULT_EMAGEON_PROPERTIES = 1058;
        public const int REMOVE_GIVEN_EMAGEON_PROPERTIES = 1059;
        public const int ADD_EMAGEON_PROPERTIES = 1060;
        public const int CHECK_BOSTON_MORPHER_FIRED_STATUS = 1061;
        public const int GET_REMOTE_AE_CONTEXT_NUMBER = 1062;
        public const int DB_RUN_PERMISSION_SCRIPT = 1063;
        public const int CREATE_FILE_CONTENTS = 1064;
        public const int EXECUTE_HL7_SEND = 1065;
        public const int GET_MWL_TAG_MORPHER = 1066;
        public const int VERIFY_MWL_TAG_MORPHER_FIRED = 1067;
        public const int STUB_STUDY_OVERWRITE_ISSUE_FIRED = 1068;
        public const int GET_EA_PLATFORM_VERSION = 1069;

        public const int VERIFY_LOG_MESSAGE_PRESENT_IN_ARCHIVE = 1070;
        public const int VERIFY_FILE_PRESENT_IN_LOCATION = 1071;

        //Install Plugins
        public const int INSTALL_MPI = 1072;
        public const int INSTALL_EATH = 1073;
        public const int GET_PROPERTY_VALUE_FROM_FILE = 1074;
        public const int INSTALL_IMAGE_UPLOADER = 1075;
        public const int INSTALL_ALPR_PLUGIN = 1107;

        //PrefetcherFail
        public const int REMOVE_PIXEL_MODIFY_PHOTOMETRIC_TAG_FROM_IMAGE = 1076;

        public const int COPY_FILES = 1077;
        public const int REPLACE_CONTENT_IN_FILE = 1078;

        public const int VERIFY_LOG_MESSAGE_PRESENT_IN_CLARCLOG = 1079;
        public const int REMOVE_FILES = 1080;
        public const int SEARCH_MULlINES_ARCHIVE = 1081;
        public const int SEARCH_EVENT_FILES = 1082;
        public const int GETPROPERTYVALUE = 1083;
        public const int CLEAR_ARCHIVELOG = 1084;
        public const int GET_PROCESSID = 1085;
        public const int SEARCH_FILE_CONTENT = 1086;
        public const int VERIFY_FINDSCU_QUERY = 1087;
        public const int RESTART_CATALINA_SERVICES = 1088;
        public const int FILEPATH_QUERY = 1089;
        public const int MODIFY_DICOM_ATTRIBUTE = 1090;
        public const int GET_EASERVER_DATETIME = 1091;
        public const int GET_REMOTEAPPLICATION_ENTITY_VALUE = 1092;
        public const int EXECUTE_COMMAND = 1093;
        public const int SET_EASERVER_DATETIME = 1094;
        public const int CLEAR_CLARCSERVERLOG = 1095;
        public const int EMG_MON_STOP = 1096;
        public const int EMG_MON_START = 1097;
        public const int MOVE_STUDY_TO_LTA = 1098;
        public const int GET_EXTERNAL_ARCHIVE_CONTEXT_IN_PROXY = 1099;
        public const int MOVE_STUDY_EXTERNAL_EA = 1100;
        public const int REPLACE_STRING_IN_EMAGEON_PROPERTIES = 1101;
        public const int CARDIO_RESTART = 2001;

        public const String IIS_APPGATE_RESTART = "2019";

        /**/
        static TcpClient clientSocket = new TcpClient();

        static string HostName { get; set; }

        static int Port { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostName"></param> ip address of the host to get connected
        /// <param name="port"></param>port number used to get connected
        /// <param name="message"></param>Message sent the socket server
        /// <returns></returns>
        public static string Send(string hostName, int port, string message)
        {
            string receivedData = string.Empty;

            try
            {
                if (IsConnected() == false || IsConnectionChangeRequired(hostName, port) == true)
                {
                    HostName = hostName;
                    Port = port;
                    if (Connect() == false)
                    {
                        throw new NullReferenceException("Failed to connect the socket");
                    }

                    if (IsInitialized() == false)
                    {
                        return null;
                    }
                }

                receivedData = Send(message);
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Unable to get the response " + ex.ToString());
            }

            return receivedData;
        }

        public static string SendInitialize(string hostName, int port)
        {
            return Send(hostName, port, string.Format("{0}", INITIALIZE));
        }


        public static void Close()
        {
            if (IsConnected())
            {
                clientSocket.Close();
            }
        }

        static bool IsConnectionChangeRequired(string hostName, int port)
        {
            if (string.Compare(HostName, hostName, true) != 0)
            {
                return true;
            }

            if (Port != port)
            {
                return true;
            }

            return false;
        }

        static bool IsConnected()
        {
            try
            {
                if (clientSocket == null)
                {
                    throw new NullReferenceException("Invalid socket object.");
                }

                return clientSocket.Connected;
            }
            catch (Exception)
            {
            }

            return false;
        }

        static bool Connect()
        {
            try
            {
                if (IsConnected())
                {
                    return true;
                }
                else
                {
                    clientSocket = new TcpClient();
                }
                clientSocket.Connect(HostName, Port);
                if (clientSocket.Connected)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new NullReferenceException("Invalid socket object." + ex.ToString());

            }

            return false;
        }

        static string Send(string message)
        {
            try
            {
                if (clientSocket == null)
                {
                    throw new NullReferenceException("Invalid client socket object");
                }

                System.Net.Sockets.NetworkStream serverStream = clientSocket.GetStream();

                byte[] outStream = System.Text.Encoding.ASCII.GetBytes("" + message + "");
                serverStream.Write(outStream, 0, outStream.Length);
                string dataFromClient = null;

                byte[] inStream = new byte[70025];
                if (serverStream.CanRead)
                {
                    serverStream.Read(inStream, 0, (int)clientSocket.ReceiveBufferSize);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(inStream);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("\0"));

                    Logger.Instance.InfoLog("Data Received : " + dataFromClient);
                }

                return dataFromClient;
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Unable to get the response " + ex.ToString());
            }

            return null;
        }

        static string Initialize()
        {
            return Send(string.Format("{0}", INITIALIZE));
        }

        static bool IsInitialized()
        {
            string receivedData = Initialize();
            if (System.Convert.ToDecimal(receivedData) == 1001)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Method to restart IIS & AppGate Services using Socket Client
        /// </summary>
        /// <returns></returns>
        public bool RestartIISService3D()
        {
            bool status = false;
            try
            {
                String output = Send(Config.IConnectIP, 7777, IIS_APPGATE_RESTART);
                if (output == "1001")
                {
                    Logger.Instance.InfoLog("IIS and AppGate Manager Services Restarted successfully");
                    status = true;
                }
                else
                    Logger.Instance.ErrorLog("Restarting IIS and AppGate Manager Services failed");
            }
            catch (Exception e)
            {
                Logger.Instance.ErrorLog("Failed while restarting IIS and 3D AppGate Services due to exception : " + e.StackTrace);
            }
            return status;
        }
    }
}