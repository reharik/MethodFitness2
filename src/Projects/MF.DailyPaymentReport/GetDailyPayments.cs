using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using CC.Core.Core.DomainTools;
using CC.Core.Utilities;
using MF.Core.Domain;
using MF.Core.Services;
using System.Linq;

namespace MF.DailyPaymentReport
{
    public interface IGetDailyPayments
    {
        IEnumerable<DailyPaymentDto> GetPayments();
        string CreateEmail(IEnumerable<DailyPaymentDto> clients);
        void SendEmail(string email, string subject);
    }

    public class GetDailyPayments : IGetDailyPayments
    {
        private readonly IRepository _repository;
        private readonly IEmailService _emailService;

        public GetDailyPayments(IRepository repository,
            IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        public IEnumerable<DailyPaymentDto> GetPayments()
         {
             var sql = @"SELECT p.CreatedDate,
		t.FirstName + ' ' + t.LastName AS Trainer,
		c.FirstName + ' ' + c.LastName AS Client, 
		p.PaymentTotal,
		p.EntityId,
		CONVERT (date, GETDATE()) as StartDate
FROM Payment as p INNER JOIN [User] AS t ON p.CreatedById = t.EntityId 
				INNER JOIN Client as c ON p.ClientId = c.EntityId
WHERE CONVERT (date, p.CreatedDate) = CONVERT (date, GETDATE())";
             var droppedClientDtos = _repository.CreateSQLQuery<DailyPaymentDto>(sql, new List<object>());
            return droppedClientDtos;
         }

        public string CreateEmail(IEnumerable<DailyPaymentDto> payments)
        {
            var email = new StringBuilder("<b>Daily Payment Report for {0} </b>".ToFormat(DateTime.Now));

            email.Append("<br />");
            email.Append("<table><tr><th><b>Trainer</b></th><th><b>Client</b></th><th><b>Total</b></th></tr>");
            payments.ForEachItem(p =>
            {
                email.Append("<tr><td>");
                email.Append(p.Trainer);
                email.Append("</td><td>");
                email.Append(p.Client);
                email.Append("</td><td>");
                email.Append(p.PaymentTotal);
                email.Append("</td></tr>");
            });
            email.Append("<tr><td col=\"2\">Total</td><td>");
            email.Append(payments.Sum(x => x.PaymentTotal));
            email.Append("</td></tr></table>");
            return email.ToString();
        }

        public void SendEmail(string email, string subject)
        {
            var emailDto = new EmailDTO
            {
                Body = email,
                Subject = subject,
                From = new MailAddress(Site.Config.EmailReportAddress),
                To = new MailAddress(Site.Config.AdminEmail)
            };
            _emailService.SendEmail(emailDto);
        }
    }
    public class DailyPaymentDto
    {
        public int EntityId { get; set; }
        public string Trainer { get; set; }
        public string Client { get; set; }
        public int PaymentTotal { get; set; }
    }
}