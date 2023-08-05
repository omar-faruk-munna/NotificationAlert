using HtmlAgilityPack;
using LogLog4Net;
using Microsoft.Extensions.Configuration;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace Email
{
    public class EmailRepository : IEmailRepository
    {
        private readonly ILoggerRepository _error;
        private readonly IConfiguration _configuration;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpEmail;
        private readonly string _smtpMasking;
        private readonly string _smtpPass;
        private readonly bool _smtpUseDefaultCredentials;
        private readonly bool _smtpEnableSsl;

        public EmailRepository(ILoggerRepository error, IConfiguration configuration)
        {
            this._error = error;
            this._configuration = configuration;
            this._smtpServer = _configuration.GetValue<string>("SmtpCredentials:Server");
            this._smtpPort = _configuration.GetValue<int>("SmtpCredentials:Port");
            this._smtpEmail = _configuration.GetValue<string>("SmtpCredentials:Email");
            this._smtpMasking = _configuration.GetValue<string>("SmtpCredentials:Masking");
            this._smtpPass = _configuration.GetValue<string>("SmtpCredentials:Password");
            this._smtpUseDefaultCredentials = _configuration.GetValue<bool>("SmtpCredentials:UseDefaultCredentials");
            this._smtpEnableSsl = _configuration.GetValue<bool>("SmtpCredentials:EnableSsl");
        }

        public EmailResponse SendEmail(string toEmail, string emailSubject, string emailContent)
        {
            EmailResponse resp = new EmailResponse();

            try
            {
                string line1 = null;
                string clobString = emailContent.ToString();
                HtmlDocument body = new HtmlDocument();
                body.LoadHtml(clobString);
                HtmlNode bodyNode = body.DocumentNode.SelectSingleNode("//body");
                List<HtmlNode> nodesToRemove = new List<HtmlNode>();

                if (bodyNode != null)
                {
                    nodesToRemove.AddRange(bodyNode.ChildNodes.Where(des => des.Name != "table"));
                }

                foreach (HtmlNode node in nodesToRemove)
                {
                    node.Remove();
                }

                if (bodyNode != null)
                {
                    string s = bodyNode.InnerText;
                    s = s.Trim();
                    line1 = s.Split(new[] { '\r', '\n' }).FirstOrDefault();
                }

                using MailMessage mailMessage = new MailMessage();
                using SmtpClient client = new SmtpClient(_smtpServer, _smtpPort);
                client.UseDefaultCredentials = _smtpUseDefaultCredentials;
                client.EnableSsl = _smtpEnableSsl;
                client.Credentials = new NetworkCredential(_smtpEmail, _smtpPass);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                mailMessage.To.Add(toEmail);
                mailMessage.Subject = emailSubject;
                mailMessage.Body = clobString;
                mailMessage.IsBodyHtml = true;
                mailMessage.From = new MailAddress(_smtpEmail, _smtpMasking);
                client.Send(mailMessage);
                resp.IsSuccess = 1;
                resp.EmailResponseTime = DateTime.Now.ToString();
                resp.EmailProiderName = _smtpEmail;

                return resp;
            }
            catch (Exception e)
            {
                this._error.LogError("Email", "SendEmail", e.Message);
                resp.IsSuccess = 0;
                resp.EmailResponseTime = DateTime.Now.ToString();
                resp.EmailProiderName = _smtpEmail;
                resp.ReasonForFail = e.Message;

                return resp;
            }
        }
    }
}
