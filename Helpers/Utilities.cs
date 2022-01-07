using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Specialized;
using System.Net.Mail;

namespace UCC.Iknow.Notifications
{
    public class Utilities
    {
        private static Utilities _Current;

        public static Utilities Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new Utilities();
                }

                return _Current;
            }
        }

        public static void SendEmail(SPWeb spWeb, string from, string to, string subject, string htmlBody)
        {
            try
            {
                var messageHeaders = new StringDictionary();
                messageHeaders.Add("to", to);
                messageHeaders.Add("from", from);
                messageHeaders.Add("subject", subject);
                messageHeaders.Add("content-type", "text/html");
                bool result = SPUtility.SendEmail(spWeb, messageHeaders, htmlBody);
            }
            catch (Exception ex)
            {
                //Log Exception
                SPLogger.LogError(ex.ToString());
            }
        }

        public static void SendEmail(string smtpHost, string from, string to, string subject, AlternateView htmlView)
        {
            try
            {
                //Create the mail message
                MailMessage mail = new MailMessage();
                //Set the email addresses
                mail.From = new MailAddress(from);
                mail.To.Add(to);
                //Set the subject
                mail.Subject = subject;
                //Add the view
                mail.AlternateViews.Add(htmlView);
                //specify the mail server address
                SmtpClient smtp = new SmtpClient(smtpHost);
                //send the message
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                //Log Exception
                SPLogger.LogError(ex.ToString());
            }
        }

    }
}
