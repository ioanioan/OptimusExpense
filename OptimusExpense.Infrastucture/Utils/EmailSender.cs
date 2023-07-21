using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using OptimusExpense.Infrastucture.Exception;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace OptimusExpense.Infrastucture.Utils
{
    public class EmailSender : IEmailSender
    {

        // Our private configuration variables
        private string host;
        private int port;
        private bool enableSSL;
        private string userName;
        private string password;
        IWebHostEnvironment _webHostEnvirnoment;
        // Get our parameterized configuration
        public EmailSender(string host, int port, bool enableSSL, string userName, string password, IWebHostEnvironment webHostEnvirnoment)
        {
            this.host = host;
            this.port = port;
            this.enableSSL = enableSSL;
            this.userName = userName;
            this.password = password;
            _webHostEnvirnoment = webHostEnvirnoment;
        }

        // Use our configuration to send the email by using SmtpClient
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (email == null|| email=="")
            {
                throw new HttpResponseException { Value="Nu ati setat adresa de mail destinatar!" };
            }
            var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential { UserName = userName, Password = password },
                EnableSsl = enableSSL,
                UseDefaultCredentials = false,
               DeliveryMethod = SmtpDeliveryMethod.Network

            };
            
            MailMessage mmEMail = new MailMessage();
            mmEMail.From = new MailAddress(userName);
            mmEMail.Subject = subject;
            mmEMail.Body = htmlMessage;
            mmEMail.IsBodyHtml = true;
            mmEMail.Bcc.Add("silviu.pantea@setrio.ro");
           mmEMail.Bcc.Add("ioan.garbacea@setrio.ro");

            String[] address = email.Split(';');        
            foreach (String l in address)
            {
                // lista de e-mail-uri predefinita
                MailAddress mm = new MailAddress(l);
                mmEMail.To.Add(mm); 
            }
            client.SendCompleted += Client_SendCompleted;
          
         
            client.SendMailAsync(mmEMail);
            return Task.FromResult(0);
        }

        private void Client_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

            if (e.Error != null)
            {
                var filePath = _webHostEnvirnoment.WebRootPath + "\\" + "log_" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt";

                File.AppendAllText(filePath,DateTime.Now.ToString("HH:mm:ss")+": "+ e.Error.Message+Environment.NewLine);
            }
        }
    }
}

