﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Alpinely.TownCrier;
using CC.Core;
using MethodFitness.Core.Domain;
using StructureMap;

namespace MethodFitness.Core.Services
{
    public interface IEmailService
    {
        string SendEmail(EmailDTO input);
        string SendEmail(EmailDTO input, IEnumerable<MailAddress> addresses);
    }

    public class EmailService : IEmailService
    {
        private readonly IContainer _container;

        public EmailService(IContainer container)
        {
            _container = container;
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
                }

                message.From = input.From;
                message.To.AddRange(addresses);
                if(input.ReplyTo != null){message.ReplyToList.Add(input.ReplyTo);}
                
                var smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.Credentials = new System.Net.NetworkCredential("methodfit@gmail.com", "methgoo69");
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
                var mergedEmailFactory = _container.GetInstance<IMergedEmailFactory>();
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