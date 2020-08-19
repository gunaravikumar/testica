using System;
using System.Collections.Generic;
using System.Linq;
using EAGetMail;
using HtmlAgilityPack;
using System.IO;
using System.Net.Mail;
using System.Threading;
using S22.Imap;
using System.Text.RegularExpressions;

namespace Selenium.Scripts.Reusable.Generic
{
    class EmailUtils
    {
        //Fields
        
        public EmailUtils()
        {            
            ServerAddress = Config.IMAPServer;            
            InboxPath = Config.InboxPath;
            SSLConnection = Config.SSLConnection;
            IMAPport = Convert.ToInt32(Config.IMAPport);

            try {
                if (InboxPath != null && InboxPath != "" && !Directory.Exists(InboxPath))
                {
                    Directory.CreateDirectory(InboxPath);
                }
            }
            catch (Exception err)
            {
                Logger.Instance.ErrorLog("Error in Creating Inbox Path. Error: " + err.Message);
            }
        }

        public string EmailId { get; set; }
        public string Password { get; set; }
        public string ServerAddress { get; set; }
        public bool SSLConnection { get; set; }
        public string InboxPath { get; set; }
        public int IMAPport { get; set; }

        //OBSOLETE - Trial Version DLL (EAGetMail.dll)
        ///// <summary>
        ///// This method is to save the Received mail which matches the filter criteria
        ///// </summary>
        ///// <param name="IsNewMail"> True - unread mail, false - read mail</param>
        ///// <param name="SubjectContains">Filter for Subject</param>
        ///// <param name="SenderContains">Filter for Sender</param>
        ///// <param name="MessageSaveFormat">binary - Saved as .msg format, text - Saved as .eml format</param>
        ///// <param name="DeleteSavedMail">True - Delete mail from mailbox after saved</param>
        ///// <returns></returns>
        //public void ReceiveMail(bool IsNewMail = true, String SubjectContains = "", String SenderContains = "", String MessageSaveFormat = "binary", bool DeleteSavedMail = false)
        //{
        //    MailClient oClient = new MailClient("TryIt");

        //    MailServer oServer = new MailServer(ServerAddress,
        //        EmailId, Password, SSLConnection,
        //        ServerAuthType.AuthLogin, ServerProtocol.Imap4);
        //    try
        //    {
        //        oClient.Connect(oServer);

        //        oClient.GetMailInfosParam.Reset();

        //        if (IsNewMail)
        //        {
        //            //Get only new email:
        //            oClient.GetMailInfosParam.GetMailInfosOptions = GetMailInfosOptionType.NewOnly;
        //        }
        //        else {
        //            //Get only Read email:
        //            oClient.GetMailInfosParam.GetMailInfosOptions = GetMailInfosOptionType.ReadOnly;
        //        }

        //        oClient.GetMailInfosParam.GetMailInfosOptions |= GetMailInfosOptionType.DateRange;
        //        oClient.GetMailInfosParam.GetMailInfosOptions |= GetMailInfosOptionType.OrderByDateTime;

        //        oClient.GetMailInfosParam.SubjectContains = SubjectContains;
        //        oClient.GetMailInfosParam.SenderContains = SenderContains;
        //        oClient.GetMailInfosParam.DateRange.SINCE = System.DateTime.Now.AddDays(-2);
        //        oClient.GetMailInfosParam.DateRange.BEFORE = System.DateTime.Now.AddDays(1);

        //        MailInfo[] infos = oClient.GetMailInfos();
        //        int count = infos.Length;
        //        for (int i = 0; i < count; i++)
        //        {
        //            MailInfo info = infos[i];
        //            Mail oMail = oClient.GetMail(info);
        //            if (MessageSaveFormat.Contains("text"))
        //            {
        //                //Save mail to local file
        //                oMail.SaveAs(String.Format(InboxPath + "\\{0}.eml", i), true);
        //            }
        //            else {
        //                //Save mail to Outlook MSG file (*.msg file)
        //                oMail.SaveAsOMSG(String.Format(InboxPath + "\\{0}.msg", i), true, false);
        //            }
        //            if (!info.Read)
        //            {
        //                oClient.MarkAsRead(info, true); // mark saved email as read 
        //            }

        //            //Delete method just mark the email as deleted
        //            if (!info.Deleted && DeleteSavedMail)
        //            {
        //                oClient.Delete(info);   //mark Saved email as deleted
        //            }
        //        }

        //        oClient.Quit();   // Quit method purge the emails from server exactly.
        //    }
        //    catch (Exception ep)
        //    {
        //        Logger.Instance.ErrorLog(String.Format("Error in Receiving Mail. Error Info: {0}", ep.Message));
        //    }

        //    oClient.Close();
        //}


        //OBSOLETE - Trial Version DLL (EAGetMail.dll)
        /// <summary>
        /// This function is to parse the email content into individual parts
        /// </summary>
        /// <param name="msgFile">saved email(.msg) file location</param>
        /// <returns></returns>
        //public Dictionary<string, string> ParseMSG(string msgFile)
        //{
        //    Mail oMail = new Mail("TryIt");
        //    oMail.LoadOMSG(msgFile);

        //    Dictionary<string, string> MessageContent = new Dictionary<string, string>();

        //    // Add some elements to the dictionary. There are no 
        //    MessageContent.Add("From", oMail.From.ToString());

        //    // Parse Mail From, Sender
        //    Console.WriteLine("From: {0}", oMail.From.ToString());

        //    // Parse Mail To, Recipient
        //    EAGetMail.MailAddress[] addrs = oMail.To;
        //    String ToAddress = String.Empty;
        //    for (int i = 0; i < addrs.Length; i++)
        //    {
        //        if (i == 0)
        //            ToAddress += addrs[i].ToString();
        //        else
        //            ToAddress = ToAddress + ";" + addrs[i].ToString();
        //    }
        //    MessageContent.Add("To", ToAddress);

        //    // Parse Mail CC
        //    addrs = oMail.Cc;
        //    String CcAddress = String.Empty;
        //    for (int i = 0; i < addrs.Length; i++)
        //    {
        //        if (i == 0)
        //            CcAddress += addrs[i].ToString();
        //        else
        //            CcAddress = CcAddress + ";" + addrs[i].ToString();
        //    }
        //    MessageContent.Add("CC", CcAddress);

        //    // Parse Mail Subject
        //    Console.WriteLine("Subject: {0}", oMail.Subject);
        //    MessageContent.Add("Subject", oMail.Subject);

        //    // Parse Mail Text/Plain body
        //    Console.WriteLine("TextBody: {0}", oMail.TextBody);
        //    MessageContent.Add("TextBody", oMail.TextBody);

        //    // Parse Mail Html Body
        //    Console.WriteLine("HtmlBody: {0}", oMail.HtmlBody);
        //    MessageContent.Add("HtmlBody", oMail.HtmlBody);

        //    // Parse Mail ReplyTo
        //    MessageContent.Add("ReplyTo", oMail.ReplyTo.ToString());

        //    // Parse Attachments
        //    EAGetMail.Attachment[] atts = oMail.Attachments;
        //    String attachments = String.Empty;
        //    for (int i = 0; i < atts.Length; i++)
        //    {
        //        if (i == 0)
        //            attachments += atts[i].Name;
        //        else
        //            attachments = attachments + ";" + atts[i].Name;
        //    }
        //    MessageContent.Add("Attachment", attachments);

        //    return MessageContent;
        //}

        /// <summary>
        /// This function is to parse the email content into individual parts
        /// </summary>
        /// <param name="msgFile">saved email(.msg) file location</param>
        /// <returns></returns>
        public List<string> ParseLinks(String FilePath)
        {
            HtmlDocument doc = new HtmlDocument();
            HashSet<string> list = new HashSet<string>();

            doc.Load(FilePath);
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes(@"//a[@href]"))
            {
                // Get the value of the HREF attribute
                string hrefValue = link.GetAttributeValue("href", string.Empty);
                string parsedLink = System.Net.WebUtility.HtmlDecode(hrefValue);        //Decode html character references '&amp' into real character '&'
                list.Add(parsedLink);
            }
            return list.ToList();
        }

        /// <summary>
        /// This function is to search email from the particular IMAP folder in mailbox
        /// </summary>
        /// <param name="folders">IMAP folder list</param>
        /// <param name="name">Folder name in the given mailbox</param>
        /// <returns></returns>
        public Imap4Folder SearchFolder(Imap4Folder[] folders, string name)
        {
            int count = folders.Length;
            for (int i = 0; i < count; i++)
            {
                Imap4Folder folder = folders[i];
                Console.WriteLine(folder.FullPath);
                // Folder was found.
                if (String.Compare(folder.Name, name) == 0)
                    return folder;

                folder = SearchFolder(folder.SubFolders, name);
                if (folder != null)
                    return folder;
            }
            // No folder found
            return null;
        }

        /// <summary>
        /// This function is to get the total mail count in the inbox
        /// </summary>
        /// <param name="IsNewMail">True - get mail count of unread messages</param>
        /// <returns></returns>
        public int GetTotalMailCount(bool IsNewMail = false)
        {
            MailClient oClient = new MailClient("TryIt");
            MailServer oServer = new MailServer(ServerAddress,
                    EmailId, Password, SSLConnection,
                    ServerAuthType.AuthLogin, ServerProtocol.Imap4);
            try
            {
                int TotalMail = -1;
                oClient.Connect(oServer);
                oClient.GetMailInfosParam.Reset();

                if (IsNewMail)
                {
                    //Get only new email:
                    oClient.GetMailInfosParam.GetMailInfosOptions = GetMailInfosOptionType.NewOnly;
                }
                else {
                    //Get only Read email:
                    oClient.GetMailInfosParam.GetMailInfosOptions = GetMailInfosOptionType.ReadOnly;
                }

                TotalMail = oClient.GetMailCount();
                return TotalMail;
            }
            catch (Exception ep)
            {
                Logger.Instance.ErrorLog(String.Format("Email Related Error: {0}", ep.Message));
                return -1;
            }
            finally
            {
                if (oClient != null && oClient.Connected) {
                    oClient.Logout();
                    oClient.Close();
                }
            }
        }

        /// <summary>
        /// This function is to wait until a new mail arrived in the mailbox
        /// </summary>
        /// <param name="WaitTime">wait time in milliseconds</param>
        /// <returns></returns>
        public bool IsNewMailArrived(int WaitTime = 5000)
        {
            MailClient oClient = new MailClient("TryIt");
            bool IsNewMail = false;
            MailServer oServer = new MailServer(ServerAddress,
                EmailId, Password, SSLConnection,
                ServerAuthType.AuthLogin, ServerProtocol.Imap4);
            try
            {
                oClient.Connect(oServer);
                oClient.GetMailInfosParam.Reset();
                oClient.GetMailInfosParam.GetMailInfosOptions = GetMailInfosOptionType.NewOnly;
                IsNewMail = oClient.WaitNewEmail(WaitTime);
                //oClient.Logout();
                return IsNewMail;
            }
            catch (Exception ep)
            {
                Logger.Instance.ErrorLog(String.Format("Email Related Error: {0}", ep.Message));
                return IsNewMail;
            }
            finally
            {
                if (oClient != null && oClient.Connected)
                {
                    oClient.Close();
                }
            }
        }

        //OBSOLETE - Trial Version DLL (EAGetMail.dll)
        /// <summary>
        /// This function is to Mark all mail as Read
        /// </summary>
        //public void MarkAllMailAsRead()
        //{
        //    MailClient oClient = new MailClient("TryIt");
        //    MailServer oServer = new MailServer(ServerAddress,
        //            EmailId, Password, SSLConnection,
        //            ServerAuthType.AuthLogin, ServerProtocol.Imap4);
        //    try
        //    {
        //        oClient.Connect(oServer);
        //        oClient.GetMailInfosParam.Reset();

        //        //Get only new email:
        //        oClient.GetMailInfosParam.GetMailInfosOptions = GetMailInfosOptionType.NewOnly;

        //        MailInfo[] infos = oClient.GetMailInfos();
        //        int count = infos.Length;
        //        for (int i = 0; i < count; i++)
        //        {
        //            MailInfo info = infos[i];
        //            Mail oMail = oClient.GetMail(info);
        //            if (!info.Read)
        //            {
        //                oClient.MarkAsRead(info, true); // mark saved email as read 
        //            }
        //        }
        //    }
        //    catch (Exception ep)
        //    {
        //        Logger.Instance.ErrorLog(String.Format("Email Related Error: {0}", ep.Message));
        //    }
        //    finally
        //    {
        //        if (oClient != null && oClient.Connected)
        //        {
        //            oClient.Logout();
        //            oClient.Close();
        //        }
        //    }
        //}

        /// <summary>
        /// This function is to delete mail from inbox
        /// </summary>
        public void DeleteMail(bool IsNewMail = false)
        {
            MailClient oClient = new MailClient("TryIt");
            MailServer oServer = new MailServer(ServerAddress,
                    EmailId, Password, SSLConnection,
                    ServerAuthType.AuthLogin, ServerProtocol.Imap4);
            try
            {
                oClient.Connect(oServer);
                oClient.GetMailInfosParam.Reset();

                if (IsNewMail)
                {
                    //Get only new email:
                    oClient.GetMailInfosParam.GetMailInfosOptions = GetMailInfosOptionType.NewOnly;
                }
                else {
                    //Get only Read email:
                    oClient.GetMailInfosParam.GetMailInfosOptions = GetMailInfosOptionType.ReadOnly;
                }

                MailInfo[] infos = oClient.GetMailInfos();
                int count = infos.Length;
                for (int i = 0; i < count; i++)
                {
                    MailInfo info = infos[i];
                    Mail oMail = oClient.GetMail(info);
                    //Delete method just mark the email as deleted
                    if (!info.Deleted)
                    {
                        oClient.Delete(info);   //mark Saved email as deleted
                    }
                }

                oClient.Quit();   // Quit method purge the emails from server exactly.
                oClient.Logout();
            }
            catch (Exception ep)
            {
                Logger.Instance.ErrorLog(String.Format("Email Related Error: {0}", ep.Message));
            }
            finally
            {
                if (oClient != null && oClient.Connected)
                {
                    oClient.Logout();
                    oClient.Close();
                }
            }
        }

        /// <summary>
        /// This method is get the mail content using IMAP Client
        /// </summary>
        /// <param name="from"> Mail sender</param>
        /// <param name="subject">Filter for Subject</param>
        /// <param name="MarkAsRead">True - to mark mail as read, false - to unread</param>   
        /// <param name="maxWaitTime">Maximum waiting time in minutes</param> 
        /// <returns></returns>
        public Dictionary<string, string> GetMailUsingIMAP(string from, string subject, bool MarkAsRead = true, int maxWaitTime = 4)
        {
            if (String.IsNullOrWhiteSpace(from)) //Default value
                from = Config.SystemEmail;

            Dictionary<string, string> MessageContent = new Dictionary<string, string>();
            try
            {
                ImapClient Client = new ImapClient(ServerAddress, IMAPport, EmailId, Password, AuthMethod.Login, SSLConnection);
                if (!Client.Authed)
                {
                   throw new Exception(EmailId + " is not authenticated with the server " + ServerAddress);
                }
                else
                    Logger.Instance.InfoLog("Email Authentication success for id : " + EmailId);

                int counterI = 0;
                IEnumerable<uint> uids;
                do
                {
                    Thread.Sleep(30000);
                    // Find all Unread messages in the mailbox that were sent from "<from>" with the subject containing "<subject>"
                    uids = Client.Search(SearchCondition.Unseen().And(SearchCondition.From(from)).And(SearchCondition.Subject(subject)));
                    counterI++;
                }
                while (uids.Count() == 0 && (counterI <= (maxWaitTime*2)));

                Logger.Instance.InfoLog("Total unread mails : " + uids.Count());

                if (uids.Count() > 0)
                {
                    // Fetch the latest message and parse its mail content
                    MailMessage msg = Client.GetMessage(uids.Last(), MarkAsRead);

                    // Get From Address from the mail content
                    Logger.Instance.InfoLog("New unread email found for search conditon. Sender: " + from + " , Subject: " + subject);
                    MessageContent.Add("From", msg.From.Address.ToString());
                    Logger.Instance.InfoLog("From: " + msg.From.Address.ToString());

                    // Get all To Address from the mail content
                    String ToAddress = String.Empty;
                    for (int i = 0; i < msg.To.Count; i++)
                    {
                        if (i == 0)
                            ToAddress += msg.To.ElementAt(i).Address.ToString();
                        else
                            ToAddress = ToAddress + ";" + msg.To.ElementAt(i).Address.ToString();
                    }
                    Logger.Instance.InfoLog("To: " + ToAddress);
                    MessageContent.Add("To", ToAddress);

                    // Get all CC Address from the mail content
                    String CcAddress = String.Empty;
                    for (int i = 0; i < msg.CC.Count; i++)
                    {
                        if (i == 0)
                            CcAddress += msg.CC.ElementAt(i).Address.ToString();
                        else
                            CcAddress = CcAddress + ";" + msg.CC.ElementAt(i).Address.ToString();
                    }
                    Logger.Instance.InfoLog("CC: " + CcAddress);
                    MessageContent.Add("CC", CcAddress);

                    // Get Mail Subject from the mail content
                    Logger.Instance.InfoLog("Subject: " + msg.Subject);
                    MessageContent.Add("Subject", msg.Subject);

                    // Get Mail Text/Plain body from the mail content
                    if (msg.IsBodyHtml)
                    {
                        // Parse Mail Html Body
                        Logger.Instance.InfoLog("HtmlBody: " + msg.Body);
                        MessageContent.Add("HtmlBody", msg.Body);
                    }
                    else {
                        Logger.Instance.InfoLog("TextBody: " + msg.Body);
                        MessageContent.Add("TextBody", msg.Body);
                    }
                    MessageContent.Add("Body", msg.Body);

                    // Get all ReplyTo Address from the mail content
                    String ReplyToList = String.Empty;
                    for (int i = 0; i < msg.ReplyToList.Count; i++)
                    {
                        if (i == 0)
                            ReplyToList += msg.ReplyToList.ElementAt(i).Address.ToString();
                        else
                            ReplyToList = ReplyToList + ";" + msg.ReplyToList.ElementAt(i).Address.ToString();
                    }
                    MessageContent.Add("ReplyTo", ReplyToList);
                    Logger.Instance.InfoLog("ReplyToList: " + ReplyToList);

                    // Get all Attachments name from the mail content
                    String attachments = String.Empty;
                    for (int i = 0; i < msg.Attachments.Count; i++)
                    {
                        if (i == 0)
                            attachments += msg.Attachments.ElementAt(i).Name;
                        else
                            attachments = attachments + ";" + msg.Attachments.ElementAt(i).Name;
                    }
                    MessageContent.Add("Attachment", attachments);
                    Logger.Instance.InfoLog("Attachment: " + attachments);
                }
                else
                    Logger.Instance.WarnLog("No Email for given search condition. Waited for '" + maxWaitTime + "' mins");
                
                // Free up any resources associated with this instance.
                Client.Dispose();

                return MessageContent;
            }
            catch (Exception ex)
            {
                Logger.Instance.InfoLog("Email read failed due to problem in accessing Mail Server because of " + ex.ToString());
                return MessageContent;
            }
        }

        /// <summary>
        /// This function is to get the total mail count in the inbox
        /// </summary>
        /// <param name="MailboxFolder">True - get mail count of unread messages</param>
        /// <returns></returns>
        public int GetTotalMailCount(string MailboxFolder = "INBOX")
        {
            ImapClient Client = new ImapClient(ServerAddress, IMAPport, EmailId, Password, AuthMethod.Login, SSLConnection);
            if (Client.Authed)
            {
                int TotalMail = Client.GetMailboxInfo(MailboxFolder).Messages;
                
                // Free up any resources associated with this instance.
                Client.Dispose();

                return TotalMail;
            }
            else
            {
                Logger.Instance.WarnLog(EmailId + " is not authenticated with the server " + ServerAddress);
                return -1;
            }
        }

        /// <summary>
        /// This function is to get the total mail count in the inbox
        /// </summary>
        /// <param name="WaitTime">True 
        /// <param name="MailboxFolder">True - get mail count of unread messages</param>
        /// <returns></returns>
        public bool IsNewMailArrived(int WaitTime = 5000, string MailboxFolder = "INBOX")
        {
            int TotalMailBefore = -1, TotalMailAfter = -1;
            ImapClient Client = new ImapClient(ServerAddress, IMAPport, EmailId, Password, AuthMethod.Login, SSLConnection);
            if (Client.Authed)
            {
                TotalMailBefore = Client.GetMailboxInfo(MailboxFolder).Messages;
                Thread.Sleep(WaitTime);
                TotalMailAfter = Client.GetMailboxInfo(MailboxFolder).Messages;

                // Free up any resources associated with this instance.
                Client.Dispose();

                if (TotalMailAfter <= TotalMailBefore)
                    return false;
                else
                    return true;
            }
            else
            {
                Logger.Instance.WarnLog(EmailId + " is not authenticated with the server " + ServerAddress);
                return false;
            }
        }

        /// <summary>
        /// This function is to get the total mail count in the inbox
        /// </summary>
        /// <param name="MailboxFolder">True - get mail count of unread messages</param>
        /// <returns></returns>
        public int GetUnreadMailCount(string MailboxFolder = "INBOX")
        {
            ImapClient Client = new ImapClient(ServerAddress, IMAPport, EmailId, Password, AuthMethod.Login, SSLConnection);
            if (Client.Authed)
            {
                int TotalMail = Client.GetMailboxInfo(MailboxFolder).Unread;

                // Free up any resources associated with this instance.
                Client.Dispose();

                return TotalMail;
            }
            else
            {
                Logger.Instance.WarnLog(EmailId + " is not authenticated with the server " + ServerAddress);
                return -1;
            }
        }

        /// <summary>
        /// This function is to Mark all mail as Read
        /// </summary>
        public void MarkAllMailAsRead(string MailboxFolder = "INBOX")
        {
            ImapClient Client = new ImapClient(ServerAddress, IMAPport, EmailId, Password, AuthMethod.Login, SSLConnection);
            if (Client.Authed)
            {

                // Find all Unread messages in the mailbox that were sent from "<from>" with the subject containing "<subject>"
                IEnumerable<uint> uids = Client.Search(SearchCondition.Unseen(), MailboxFolder);
                Logger.Instance.InfoLog("MarkAllMailAsRead(): Total unread mails - " + uids.Count());
                foreach (var uid in uids)
                {
                    MailMessage msg = Client.GetMessage(uid,seen: true);
                    //Client.SetMessageFlags(uid, MailboxFolder, MessageFlag.Seen);
                    Logger.Instance.InfoLog("Mail '" + msg.Subject + "' is marked as read");
                }
                
                // Free up any resources associated with this instance.
                Client.Dispose();

            }
            else
            {
                Logger.Instance.WarnLog(EmailId + " is not authenticated with the server " + ServerAddress);
            }
        }

        /// <summary>
        /// This function is delete all mail in the mailbox
        /// </summary>
        public void DeleteAllMails(string MailboxFolder = "INBOX")
        {
            ImapClient Client = new ImapClient(ServerAddress, IMAPport, EmailId, Password, AuthMethod.Login, SSLConnection);
            if (Client.Authed)
            {

                // Find all messages in the mailbox and set Deleted flag to it.
                IEnumerable<uint> uids = Client.Search(SearchCondition.All(), MailboxFolder);
                foreach (var uid in uids)
                {
                    Client.SetMessageFlags(uid, MailboxFolder, MessageFlag.Deleted);
                }

                //Permanently removes all messages that have the \Deleted flag set from the specified mailbox.
                Client.Expunge(MailboxFolder);
                Logger.Instance.InfoLog("Permanently deleted " + uids.Count() + " messages from the " + MailboxFolder + " folder");

                // Free up any resources associated with this instance.
                Client.Dispose();

            }
            else
            {
                Logger.Instance.WarnLog(EmailId + " is not authenticated with the server " + ServerAddress);
            }
        }

        /// <summary>
        /// This function is to send mail via SMTP server
        /// </summary>
        /// <param name="ToList"></param>
        /// <param name="Subject"></param>
        /// <param name="Body"></param>
        /// <param name="From"></param>
        public static void SendSMTPEmail(String ToList, String Subject, String Body, String From = "admin@merge.com")
        {
            //"$Imgdrvpath\\blat.exe -install $smtphost admin@merge.com 1 25";
            MailMessage mail = new MailMessage(From, ToList);
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = Config.SMTPServerIP;
            mail.Subject = Subject;
            mail.Body = Body;
            client.Send(mail);
        }

        /// <summary>
        /// This function is to send mail via SMTP server
        /// </summary>
        /// <param name="ToList"></param>
        /// <param name="Subject"></param>
        /// <param name="Body"></param>
        /// <param name="From"></param>
        public static void SendSMTPEmail(String ToList, String Subject, String Body, String From = "admin@merge.com", String HostIP = "")
        {

            try
            {
                //"$Imgdrvpath\\blat.exe -install $smtphost admin@merge.com 1 25";
                MailMessage mail = new MailMessage(From, ToList);
                SmtpClient client = new SmtpClient();
                client.Port = 25;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Host = (!HostIP.Equals("") ? HostIP : Config.SMTPServerIP);
                mail.Subject = Subject;
                mail.Body = Body;
                client.Send(mail);
            }
            catch (Exception e) {
                Logger.Instance.InfoLog(e.Message + Environment.NewLine +
e.StackTrace); }
        }

        /// <summary>
        /// This function is to get the shared study email link
        /// </summary>
        /// <param name="MessageContent">Email dictionary object retrieved using GetMailFromIMAP()</param>        
        public string GetEmailedStudyLink(Dictionary<string, string> MessageContent)
        {          
            if (MessageContent.Count > 0)
            {      
                string mailLink = MessageContent["Body"];           
                string returnLink = "";
                mailLink = mailLink.Replace("=3D", "=");
                //List<string> links = new List<string>();
                string regexImgSrc = @"<a.*href=[""'](?<url>[^""^']+[.]*)[""'].*>*</a>";
                MatchCollection matchesImgSrc = Regex.Matches(mailLink, regexImgSrc, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                Logger.Instance.InfoLog("Links extracted from email body:");
                foreach (Match m in matchesImgSrc)
                {
                    string href = m.Groups[1].Value;           
                    //links.Add(new Uri(href).AbsoluteUri);
                    Logger.Instance.InfoLog(new Uri(href).AbsoluteUri);
                    if (!new Uri(href).AbsoluteUri.ToLower().Contains("mailto"))
                        returnLink = new Uri(href).AbsoluteUri;
                }
                Logger.Instance.InfoLog("Link returned : " + returnLink);
                return returnLink;

            }
            else
            {
                Logger.Instance.ErrorLog("No content to extract Link");
                return null;                
            }

        }

    }
}
