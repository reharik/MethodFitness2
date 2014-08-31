using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using Alpinely.TownCrier;
using CC.Core;
using CC.Utility;
using MF.Core.Domain;
using StructureMap;

namespace MF.Core.Services
{
    public interface IEmailService
    {
        string SendEmail(EmailDTO input);
        string SendEmail(EmailDTO input, IEnumerable<MailAddress> addresses);
    }

    public class EmailService : IEmailService
    {
        public EmailService()
        {
        }

        public string SendEmail(EmailDTO input)
        {
           return SendEmail(input, input.Addresses ?? new[] { input.To });
        }
        public string SendEmail(EmailDTO input, IEnumerable<MailAddress> addresses )
        {
            try
            {
                var message = new MailMessage();
                if (input.TokenValues != null && input.TokenValues.Any())
                {
                    message = ProcessEmailTemplate(input);
                }
                else
                {
                    message.Body = input.Body;
                    message.Subject = input.Subject;
                }

                message.From = input.From;
                message.To.AddRange(addresses);
                if(input.ReplyTo != null){message.ReplyToList.Add(input.ReplyTo);}

                var smtpClient = new SmtpClient(Site.Config.SMTPServer, 587);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Credentials = new System.Net.NetworkCredential(Site.Config.SMTPUN, Site.Config.SMTPPW);
                smtpClient.EnableSsl = true;
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return string.Empty;
        }

        private MailMessage ProcessEmailTemplate(EmailDTO input)
        {
                var mergedEmailFactory = new MergedEmailFactory(new TemplateParser());
                MailMessage message = mergedEmailFactory
                    .WithTokenValues(input.TokenValues)
                    .WithSubject(input.Subject)
                    .WithHtmlBody(input.Body)
                    .Create();
                return message;
        }
    }

    public class EmailDTO
    {
        public IDictionary<string, string> TokenValues { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public MailAddress From { get; set; }
        public MailAddress To { get; set; }
        public IEnumerable<MailAddress> Addresses { get; set; }
        public MailAddress ReplyTo { get; set; }
    }
}