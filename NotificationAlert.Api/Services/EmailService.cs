using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using NotificationAlert.Api.Repositories;
using Oracle.ManagedDataAccess.Types;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace NotificationAlert.Api.Services
{
    public class EmailService : IEmailRepository
    {
        #region Fields
        private readonly IConfiguration _configuration;
        private readonly ILogRepository _errorLog;
        private readonly string _emailHeadFormat;

        #endregion

        public EmailService(IConfiguration configuration, ILogRepository errorLog)
        {
            this._emailHeadFormat = "Sent Time\tProduct Id\tAccount Number\tEmail\tEmail Text\tStatus";
            this._configuration = configuration;
            this._errorLog = errorLog;

        }

        public async Task SendEmailAsync(OracleClob clob, string toEmail, string accountNo)
        {
            try
            {
                if (_configuration.GetSection("API_ExecutionMode").GetSection("MODE").Value.ToUpper() == "UAT")
                {
                    toEmail = _configuration.GetSection("API_ExecutionMode").GetSection("EMAIL").Value;
                }

                string line1 = null;
                string clobString = clob.ToString();
                HtmlDocument body = new HtmlDocument();
                body.LoadHtml(clobString);
                HtmlNode bodyNode = body.DocumentNode.SelectSingleNode("//body");
                List<HtmlNode> nodesToRemove = new List<HtmlNode>();

                if (bodyNode != null)
                {
                    nodesToRemove.AddRange(bodyNode.ChildNodes.Where(des => des.Name != "table"));
                }

                foreach (HtmlNode node in nodesToRemove) node.Remove();

                if (bodyNode != null)
                {
                    string s = bodyNode.InnerText;
                    s = s.Trim();
                    line1 = s.Split(new[] { '\r', '\n' }).FirstOrDefault();
                }

                try
                {
                    string host = _configuration.GetSection("SMTP_Credentials").GetSection("Server").Value;
                    int port = Convert.ToInt32(_configuration.GetSection("SMTP_Credentials").GetSection("Port").Value);
                    string userName = _configuration.GetSection("SMTP_Credentials").GetSection("Email").Value;
                    string password = _configuration.GetSection("SMTP_Credentials").GetSection("Password").Value;
                    using MailMessage mailMessage = new MailMessage();
                    using SmtpClient client = new SmtpClient(host, port)
                    {
                        EnableSsl = true,
                        Credentials = new NetworkCredential(userName, password)
                    };
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    mailMessage.To.Add(toEmail);
                    mailMessage.Subject = line1;
                    mailMessage.Body = clobString;
                    mailMessage.IsBodyHtml = true;
                    client.Send(mailMessage);
                    string emailSuccessDir = _configuration.GetValue<string>("EmailSuccessDir");
                    _errorLog.LogEmailSuccessTsv(emailSuccessDir, _emailHeadFormat, toEmail, clobString, accountNo, "Success");
                    await Task.Delay(10);
                }
                catch (Exception e)
                {
                    string emailFailDir = _configuration.GetValue<string>("EmailFailDir");
                    _errorLog.LogEmailFailTsv(emailFailDir, _emailHeadFormat, toEmail, "content", accountNo, "Failed");
                    Log.Error(e.Message);
                    _errorLog.LogError(e.Message).Wait();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                _errorLog.LogError(e.Message).Wait();
            }
        }
        public async Task SendEmailAsync(string clob, string toEmail, string accountNo, string msg_fnc_nm, string content)
        {
            try
            {
                if (_configuration.GetSection("API_ExecutionMode").GetSection("MODE").Value.ToUpper() == "UAT")
                {
                    toEmail = _configuration.GetSection("API_ExecutionMode").GetSection("EMAIL").Value;
                }

                string line1 = null;
                string clobString = clob.ToString();
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

                try
                {
                    using MailMessage mailMessage = new MailMessage();
                    using SmtpClient client = new SmtpClient(_configuration.GetSection("SMTP_Credentials").GetSection("Server").Value, Convert.ToInt32(_configuration.GetSection("SMTP_Credentials").GetSection("Port").Value));

                    try
                    {
                        client.UseDefaultCredentials = Convert.ToBoolean(_configuration.GetSection("SMTP_Credentials").GetSection("UseDefaultCredentials").Value);
                        client.EnableSsl = Convert.ToBoolean(_configuration.GetSection("SMTP_Credentials").GetSection("EnableSsl").Value);
                        client.Credentials = new NetworkCredential(_configuration.GetSection("SMTP_Credentials").GetSection("Email").Value, _configuration.GetSection("SMTP_Credentials").GetSection("Password").Value);
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        mailMessage.To.Add(toEmail);
                        mailMessage.Subject = msg_fnc_nm;
                        mailMessage.Body = clobString;
                        mailMessage.IsBodyHtml = true;
                        mailMessage.From = new MailAddress(_configuration.GetSection("SMTP_Credentials").GetSection("Email").Value, _configuration.GetSection("SMTP_Credentials").GetSection("Masking").Value);
                        client.Send(mailMessage);
                        string emailSuccessDir = _configuration.GetValue<string>("EmailSuccessDir");
                        _errorLog.LogEmailSuccessTsv(emailSuccessDir, _emailHeadFormat, toEmail, content, accountNo, "Success");
                        await Task.Delay(10);
                    }
                    catch (Exception)
                    {
                        string emailFailDir = _configuration.GetValue<string>("EmailFailDir");
                        _errorLog.LogEmailFailTsv(emailFailDir, _emailHeadFormat, toEmail, content, accountNo, "Failed");
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                    _errorLog.LogError(e.Message).Wait();
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                _errorLog.LogError(e.Message).Wait();
            }
        }
    }
}
