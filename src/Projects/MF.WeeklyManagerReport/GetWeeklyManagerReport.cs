using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using CC.Core.Core.DomainTools;
using CC.Core.Utilities;
using MF.Core.Domain;
using MF.Core.Services;
using System.Linq;

namespace MF.WeeklyManagerReport
{
    public interface IGetWeeklyManagerReport
    {
        IEnumerable<WeeklyManagerDto> GetPayments();
        string CreateEmail(IEnumerable<WeeklyManagerDto> clients);
        void SendEmail(string email, string subject);
    }

    public class GetWeeklyManagerReport : IGetWeeklyManagerReport
    {
        private readonly IRepository _repository;
        private readonly IEmailService _emailService;

        public GetWeeklyManagerReport(IRepository repository,
            IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        public IEnumerable<WeeklyManagerDto> GetPayments()
        {
            var sql = @"EXEC	[dbo].[ManagerReport]
		@StartDate = '9/1/2016',
		@EndDate = '10/1/2016',
		@ManagerId = 3";

            var droppedClientDtos = _repository.CreateSQLQuery<WeeklyManagerDto>(sql, new List<object>());
            return droppedClientDtos;
        }

        public string CreateEmail(IEnumerable<WeeklyManagerDto> items)
        {
            var email = new StringBuilder("<b>Weekly Manager Report for {0} </b>".ToFormat(DateTime.Now));

            email.Append("<br />");
            email.Append("<table><tr><th><b>Date</b></th><th><b>Start</b></th><th><b>Type</b></th>");
            email.Append("<th><b>Client</b></th><th><b>Trainer</b></th></th></tr>");
            items.ForEachItem(p =>
            {
                email.Append("<tr><td>");
                email.Append(p.Date.ToShortDateString());
                email.Append("</td><td>");
                email.Append(p.StartTime.ToShortTimeString());
                email.Append("</td><td>");
                email.Append(p.AppointmentType);
                email.Append("</td><td>");
                email.Append(p.ClientName);
                email.Append("</td><td>");
                email.Append(p.TrainerName);
            });
            email.Append("</td></tr></table>");
            email.Append("<br />");
            email.Append("<table><tr><th><b>Hours</b></th><th><b>HalfHours</b></th><th><b>Pairs</b></th>");
            email.Append("<th><b>Total Hours</b></th><th><b>Trainer</b></th>");
            email.Append("<th><b>Total Revenue</b></th><th><b>Mgr Revenue</b></th>");
            email.Append("<th><b>Sans Mgr</b></th><th><b>Mgr Percent</b></th><th><b>Mgr Payment</b></th></tr>");
            var item = items.First();
            email.Append("<tr><td>");
            email.Append(item.Hour);
            email.Append("</td><td>");
            email.Append(item.HalfHour);
            email.Append("</td><td>");
            email.Append(item.Pair);
            email.Append("</td><td>");
            email.Append(item.Total);
            email.Append("</td><td>");
            email.Append(item.TrainerName);
            email.Append("</td><td>");
            email.Append(item.TotalRevenue);
            email.Append("</td><td>");
            email.Append(item.MgrRevenue);
            email.Append("</td><td>");
            email.Append(item.TotalMinusMgr);
            email.Append("</td><td>");
            email.Append(item.MgrPercent);
            email.Append("</td><td>");
            email.Append(item.MgrPayment);
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
    public class WeeklyManagerDto
    {
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public string AppointmentType { get; set; }
        public decimal Cost { get; set; }
        public string ClientName { get; set; }
        public string TrainerName { get; set; }
        public int TotalRevenue { get; set; }
        public int MgrRevenue { get; set; }
        public int TotalMinusMgr { get; set; }
        public decimal MgrPercent { get; set; }
        public decimal MgrPayment{ get; set; }
        public int Hour { get; set; }
        public int HalfHour { get; set; }
        public int Pair { get; set; }
        public int Total { get; set; }
    }
}