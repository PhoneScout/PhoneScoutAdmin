using DotNetEnv;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PhoneScoutAdmin.Commands
{
    public class EmailSending
    {
        public static async Task SendEmail(string mailAddressTo, string subject, string body)
        {
            Env.Load();

            string EmailAddress = Environment.GetEnvironmentVariable("EMAIL");
            string EmailPassword = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");

            MessageBox.Show(EmailAddress ?? "EMAIL IS NULL");

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress(EmailAddress);
            mail.To.Add(mailAddressTo);
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = body;

            /*System.Net.Mail.Attachment attachment;
            attachment = new System.Net.Mail.Attachment("");
            mail.Attachments.Add(attachment);*/

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(EmailAddress, EmailPassword);


            SmtpServer.EnableSsl = true;

            await SmtpServer.SendMailAsync(mail);

        }
    }
}
