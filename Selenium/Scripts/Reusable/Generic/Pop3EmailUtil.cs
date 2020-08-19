using OpenPop.Mime;
using OpenPop.Pop3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Selenium.Scripts.Reusable.Generic
{
    public class Pop3EmailUtil
    {
        public static string GetMail(String userEmail, string userPassword, string appUrl, string subject)
        {
            Logger.Instance.InfoLog("Method :GetMail.");
            Logger.Instance.InfoLog(" User " + userEmail + " is trying to connect gmail for " + subject + " of " + appUrl);
            List<MailMessage> mailMessage = new List<MailMessage>();
            try
            {
                Pop3Client pop3Client = new Pop3Client();
                {
                    List<string> Uids = new List<string>();
                    pop3Client.Connect(Config.POPMailHostname, Config.POPMailPort, Config.POPMailUseSSL);
                    if (!pop3Client.Connected)
                    {
                        pop3Client.Disconnect();
                        Thread.Sleep(5000);

                        pop3Client.Connect(Config.POPMailHostname, Config.POPMailPort, Config.POPMailUseSSL);
                    }
                    pop3Client.Authenticate(userEmail, userPassword);
                    int messageCount = pop3Client.GetMessageCount();
                    for (int i = 0; i < messageCount; i++)
                    {
                        OpenPop.Mime.Message message = pop3Client.GetMessage(i + 1);
                        Uids.Add(pop3Client.GetMessageUid(i + 1));
                        if (Convert.ToString(message.Headers.Subject).Trim() == subject.Trim())
                        //if (Common.Compare(message.Headers.Subject, subject) == 0)
                        {
                            mailMessage.Add(message.ToMailMessage());
                            Logger.Instance.InfoLog("Mail content is  " + mailMessage[0].Body);
                        }
                    }
                    pop3Client.Reset();
                    pop3Client.Disconnect();
                    //pop3Client.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.InfoLog("Email read failed due to problem in accessing Mail Server because of " + ex.ToString());
            }
            if (mailMessage.Count > 0)
                return mailMessage[0].Body;
            else
                return "No mail";
        }

        public static int DeleteAllMails(string userEmail, string userPassword)
        {
            Logger.Instance.InfoLog("Method :DeleteAllMails.");
            Logger.Instance.InfoLog(" User " + userEmail + " is trying to clear mail from mail server");
            int messageCount = 0;
            try
            {
                Pop3Client pop3Client = new Pop3Client();
                {
                    List<string> Uids = new List<string>();
                    pop3Client.Connect(Config.POPMailHostname, Config.POPMailPort, Config.POPMailUseSSL);
                    if (!pop3Client.Connected)
                    {
                        pop3Client.Disconnect();
                        Thread.Sleep(5000);

                        pop3Client.Connect(Config.POPMailHostname, Config.POPMailPort, Config.POPMailUseSSL);
                    }
                    pop3Client.Authenticate(userEmail, userPassword);
                    messageCount = pop3Client.GetMessageCount();
                    Logger.Instance.InfoLog("Get all mail for deleting.");
                    for (int messageItem = messageCount; messageItem > 0; messageItem--)
                    {
                        pop3Client.DeleteMessage(messageItem);

                    }
                    pop3Client.Disconnect();
                    // pop3Client.Dispose();

                }
            }
            catch (Exception ex)
            {
                Logger.Instance.ErrorLog("Error in DeleteAllMails" + ex);
                Logger.Instance.InfoLog("Either Deleted all messages or No Mails received :" + userEmail + "  account");
            }
            return messageCount;
        }

        public static string GetMailSubject(string userEmail, string userPassword, string appUrl, string subject)
        {
            Logger.Instance.InfoLog("Method :GetMail.");
            Logger.Instance.InfoLog(" User " + userEmail + " is trying to connect gmail for " + subject + " of " + appUrl);
            List<MailMessage> mailMessage = new List<MailMessage>();
            try
            {
                Pop3Client pop3Client = new Pop3Client();
                {
                    List<string> Uids = new List<string>();
                    pop3Client.Connect(Config.POPMailHostname, Config.POPMailPort, Config.POPMailUseSSL);
                    if (!pop3Client.Connected)
                    {
                        pop3Client.Disconnect();

                        pop3Client.Connect(Config.POPMailHostname, Config.POPMailPort, Config.POPMailUseSSL);
                    }
                    pop3Client.Authenticate(userEmail, userPassword);
                    int messageCount = pop3Client.GetMessageCount();
                    for (int i = 0; i < messageCount; i++)
                    {
                        OpenPop.Mime.Message message = pop3Client.GetMessage(i + 1);
                        Uids.Add(pop3Client.GetMessageUid(i + 1));
                        if (Convert.ToString(message.Headers.Subject) == subject)
                        {
                            mailMessage.Add(message.ToMailMessage());
                            Logger.Instance.InfoLog("Mail subject is  " + mailMessage[0].Subject);
                        }
                    }
                    pop3Client.Reset();
                }
            }
            catch
            {
                Logger.Instance.InfoLog("Email read failed due to problem in accessing Mail Server.");
            }
            if (mailMessage.Count > 0)
                return mailMessage[0].Subject;
            else
                return "No mail";
        }

        public static string DownloadAttachmentFromMail(string userEmail, string userPassword, string appUrl, string subject)
        {
            Logger.Instance.InfoLog("Method :GetMail.");
            Logger.Instance.InfoLog(" User " + userEmail + " is trying to connect gmail for " + subject + " of " + appUrl);
            List<MailMessage> mailMessage = new List<MailMessage>();
            try
            {
                Pop3Client pop3Client = new Pop3Client();
                {
                    List<string> Uids = new List<string>();
                    pop3Client.Connect(Config.POPMailHostname, Config.POPMailPort, Config.POPMailUseSSL);
                    if (!pop3Client.Connected)
                    {
                        pop3Client.Disconnect();

                        pop3Client.Connect(Config.POPMailHostname, Config.POPMailPort, Config.POPMailUseSSL);
                    }
                    pop3Client.Authenticate(userEmail, userPassword);
                    int messageCount = pop3Client.GetMessageCount();
                    for (int i = 0; i < messageCount; i++)
                    {
                        OpenPop.Mime.Message message = pop3Client.GetMessage(i + 1);
                        Uids.Add(pop3Client.GetMessageUid(i + 1));
                        if (Convert.ToString(message.Headers.Subject).Contains(subject))
                        {
                            Logger.Instance.InfoLog(userEmail + " received the Mail with Subject " + subject);
                            List<MessagePart> attachments = message.FindAllAttachments();
                            foreach (var attachment in message.FindAllAttachments())
                            {
                                if (attachment.FileName.Equals("AccountingReport.csv"))
                                {
                                    mailMessage.Add(message.ToMailMessage());
                                    Logger.Instance.InfoLog("Mail has Accouting Report attachment");
                                    attachment.Save(new FileInfo(Path.Combine(Config.FileDownloadLocation, attachment.FileName)));
                                    Logger.Instance.InfoLog("Attachment is downloaded to the location: " + Config.FileDownloadLocation);
                                }
                            }
                            pop3Client.Reset();
                        }
                    }
                }
            }
            catch
            {
                Logger.Instance.InfoLog("Email read failed due to problem in accessing Mail Server.");
            }
            if (mailMessage.Count > 0)
                return mailMessage[0].Body;
            else
                return "No mail";
        }

        public static string DownloadAttachmentFromMailHMail(String userEmail, string userPassword, string appUrl, string subject)
        {
            Logger.Instance.InfoLog("Method :GetMail.");
            Logger.Instance.InfoLog(" User " + userEmail + " is trying to connect gmail for " + subject + " of " + appUrl);
            List<MailMessage> mailMessage = new List<MailMessage>();
            try
            {
                Pop3Client pop3Client = new Pop3Client();
                {
                    List<string> Uids = new List<string>();
                    pop3Client.Connect(Config.POPMailHostname, Config.POPMailPort, Config.POPMailUseSSL);
                    if (!pop3Client.Connected)
                    {
                        pop3Client.Disconnect();

                        pop3Client.Connect(Config.POPMailHostname, Config.POPMailPort, Config.POPMailUseSSL);
                    }
                    pop3Client.Authenticate(userEmail, userPassword);
                    int messageCount = pop3Client.GetMessageCount();
                    for (int i = 0; i < messageCount; i++)
                    {
                        OpenPop.Mime.Message message = pop3Client.GetMessage(i + 1);
                        Uids.Add(pop3Client.GetMessageUid(i + 1));
                        if (Convert.ToString(message.Headers.Subject).Contains(subject))
                        {
                            Logger.Instance.InfoLog(userEmail + " received the Mail with Subject " + subject);
                            AttachmentCollection coll = message.ToMailMessage().Attachments;
                            MessagePart attachments = message.MessagePart;
                            List<MessagePart> depth = attachments.MessageParts;

                            foreach (var indepth in depth)
                            {

                                if (indepth.FileName.Equals("AccountingReport.csv"))
                                {
                                    mailMessage.Add(message.ToMailMessage());
                                    Logger.Instance.InfoLog("Mail has Accouting Report attachment");
                                    indepth.Save(new FileInfo(Path.Combine(Config.FileDownloadLocation, indepth.FileName)));
                                    Logger.Instance.InfoLog("Attachment is downloaded to the location: " + Config.FileDownloadLocation);
                                }
                            }

                            pop3Client.Reset();
                        }
                    }
                }
            }
            catch
            {
                Logger.Instance.InfoLog("Email read failed due to problem in accessing Mail Server.");
            }
            if (mailMessage.Count > 0)
                return mailMessage[0].Body;
            else
                return "No mail";
        }

        public static string DownloadLogoFromMail(string userEmail, string userPassword, string appUrl, string subject)
        {
            Logger.Instance.InfoLog("Method :GetMail.");
            Logger.Instance.InfoLog(" User " + userEmail + " is trying to connect gmail for " + subject + " of " + appUrl);
            List<MailMessage> mailMessage = new List<MailMessage>();
            try
            {
                Pop3Client pop3Client = new Pop3Client();
                {
                    List<string> Uids = new List<string>();
                    pop3Client.Connect(Config.POPMailHostname, Config.POPMailPort, Config.POPMailUseSSL);
                    if (!pop3Client.Connected)
                    {
                        pop3Client.Disconnect();

                        pop3Client.Connect(Config.POPMailHostname, Config.POPMailPort, Config.POPMailUseSSL);
                    }
                    pop3Client.Authenticate(userEmail, userPassword);
                    int messageCount = pop3Client.GetMessageCount();
                    for (int i = 0; i < messageCount; i++)
                    {
                        OpenPop.Mime.Message message = pop3Client.GetMessage(i + 1);
                        Uids.Add(pop3Client.GetMessageUid(i + 1));

                        if (Convert.ToString(message.Headers.Subject).Contains(subject))
                        {
                            Logger.Instance.InfoLog(userEmail + " received the Mail with Subject " + subject);
                            var type = message.FindFirstHtmlVersion();
                            var rawString = type.GetBodyAsText();
                            var url = Regex.Split(rawString, @"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                            string pattern = ".png";
                            string[] substrings = Regex.Split(rawString, pattern);
                            foreach (string match in substrings)
                            {
                                string[] URl = match.Split('\"');
                                return URl[1];
                            }

                            pop3Client.Reset();
                            break;
                        }
                    }
                }
            }
            catch
            {
                Logger.Instance.InfoLog("Email read failed due to problem in accessing Mail Server.");
            }
            if (mailMessage.Count > 0)
                return mailMessage[0].Body;
            else
                return "No mail";
        }

        public static string DownloadJpegAttachmentFromMailHMail(string userEmail, string userPassword, string appUrl, string subject)
        {
            Logger.Instance.InfoLog("Method :GetMail.");
            Logger.Instance.InfoLog(" User " + userEmail + " is trying to connect gmail for " + subject + " of " + appUrl);
            List<MailMessage> mailMessage = new List<MailMessage>();
            try
            {
                Pop3Client pop3Client = new Pop3Client();
                {
                    List<string> Uids = new List<string>();
                    pop3Client.Connect(Config.POPMailHostname, Config.POPMailPort, Config.POPMailUseSSL);
                    if (!pop3Client.Connected)
                    {
                        pop3Client.Disconnect();

                        pop3Client.Connect(Config.POPMailHostname, Config.POPMailPort, Config.POPMailUseSSL);
                    }
                    pop3Client.Authenticate(userEmail, userPassword);
                    int messageCount = pop3Client.GetMessageCount();
                    for (int i = 0; i < messageCount; i++)
                    {
                        OpenPop.Mime.Message message = pop3Client.GetMessage(i + 1);
                        Uids.Add(pop3Client.GetMessageUid(i + 1));
                        if (Convert.ToString(message.Headers.Subject).Contains(subject))
                        {
                            Logger.Instance.InfoLog(userEmail + " received the Mail with Subject " + subject);
                            AttachmentCollection coll = message.ToMailMessage().Attachments;
                            MessagePart attachments = message.MessagePart;
                            List<MessagePart> depth = attachments.MessageParts;

                            List<MessagePart> part = depth[1].MessageParts;
                            part[1].Save(new FileInfo("D:\\logo.jpg"));
                            foreach (var indepth in depth)
                            {

                                if (indepth.FileName.Equals("AccountingReport.csv"))
                                {
                                    mailMessage.Add(message.ToMailMessage());
                                    Logger.Instance.InfoLog("Mail has Accouting Report attachment");
                                    indepth.Save(new FileInfo(Path.Combine(Config.FileDownloadLocation, indepth.FileName)));
                                    Logger.Instance.InfoLog("Attachment is downloaded to the location: " + Config.FileDownloadLocation);
                                }
                            }

                            pop3Client.Reset();
                        }
                    }
                }
            }
            catch
            {
                Logger.Instance.InfoLog("Email read failed due to problem in accessing Mail Server.");
            }
            if (mailMessage.Count > 0)
                return mailMessage[0].Body;
            else
                return "No mail";
        }

        public static string GetEmailedStudyLink(string user, string password, string appUrl, string subject)
        {

            if (Config.POP3_Enable == "true")
            {
                bool status = false;
                bool matchSrc = false;
                string mailLink = string.Empty;
                for (int i = 0; i <= 30; i++)
                {
                    mailLink = GetMail(user, password, appUrl, subject);
                    if (mailLink == "" || mailLink == null || mailLink == "No mail")
                    {
                        Thread.Sleep(10000);
                        Logger.Instance.InfoLog("iterating " + i);
                    }
                    else
                    {
                        status = true;
                        break;
                    }
                }
                if (!status)
                {
                    throw new Exception("Registration Link not yet received");
                }

                mailLink = mailLink.Replace("=3D", "=");
                List<Uri> links = new List<Uri>();
                string regexImgSrc = @"<a.*href=[""'](?<url>[^""^']+[.]*)[""'].*>*</a>";
                MatchCollection matchesImgSrc = Regex.Matches(mailLink, regexImgSrc, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                foreach (Match m in matchesImgSrc)
                {
                    matchSrc = true;
                    string href = m.Groups[1].Value;
                    links.Add(new Uri(href));
                }
                if (matchSrc == false)
                {
                    mailLink = GetMail(user, password, appUrl, subject);
                    mailLink = mailLink.Replace("=3D", "=");
                    matchesImgSrc = Regex.Matches(mailLink, regexImgSrc, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    foreach (Match m in matchesImgSrc)
                    {
                        matchSrc = true;
                        string href = m.Groups[1].Value;
                        links.Add(new Uri(href));
                    }
                }
                Logger.Instance.InfoLog("Link: " + links[0].AbsoluteUri);
                return links[0].AbsoluteUri;
            }
            else
                return null;

        }
    }
}
