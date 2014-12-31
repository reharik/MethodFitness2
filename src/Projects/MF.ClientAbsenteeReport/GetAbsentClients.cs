using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using CC.Core.Core.DomainTools;
using CC.Core.Utilities;
using MF.Core.Domain;
using MF.Core.Services;
using System.Linq;

namespace MF.ClientAbsenteeReport
{
    public interface IGetDroppedClients
    {
        IEnumerable<DroppedClientDto> GetClients();
        string CreateEmail(IEnumerable<DroppedClientDto> clients);
        void SendEmail(string email, string subject);
        void UpdateClients(IEnumerable<DroppedClientDto> clients);
        IEnumerable<DroppedClientDto> GetWeeklyClients();
        string CreateWeeklyEmail(IEnumerable<DroppedClientDto> clients);
    }

    public class GetDroppedClients : IGetDroppedClients
    {
        private readonly IRepository _repository;
        private readonly IEmailService _emailService;

        public GetDroppedClients(IRepository repository,
            IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        public IEnumerable<DroppedClientDto> GetClients()
         {
             var sql = @"SELECT  c.EntityId, 
		c.FirstName,
		c.LastName,
		c.Email,
		c.MobilePhone,
		MAX(a.date) LastDate,
				u.FirstName as TrainerFirstName,
				u.LastName as TrainerLastName
FROM    client c
        INNER JOIN appointment_client ac
            ON c.entityid = ac.clientid
        INNER JOIN appointment a 
            ON ac.appointmentid = a.entityid
		left join [user] u on a.trainerid = u.entityId
        left JOIN ClientStatus cs 
			ON c.ClientStatusId = cs.EntityId
GROUP   BY c.entityid ,c.entityId, 
		c.firstName,
		c.lastName,
		c.email,
		c.mobilephone,
        a.Completed,
		cs.AdminAlerted,
				u.FirstName,
				u.LastName
having max(a.date) < DATEADD(DAY, DATEDIFF(DAY, 0, GETDATE()), -10) 
and max(a.date) > DATEADD(DAY, DATEDIFF(DAY, 0, GETDATE()), -90)
and a.Completed = 1
and (cs.AdminAlerted is null OR cs.AdminAlerted = 0)
order by u.lastname, u.FirstName ";
             var droppedClientDtos = _repository.CreateSQLQuery<DroppedClientDto>(sql, new List<object>());
            return droppedClientDtos;
         }

        public IEnumerable<DroppedClientDto> GetWeeklyClients()
        {
            var sql = @"SELECT  c.EntityId, 
		c.FirstName,
		c.LastName,
		c.Email,
		c.MobilePhone,
		MAX(a.date) LastDate,
				u.FirstName as TrainerFirstName,
				u.LastName as TrainerLastName
FROM    client c
        INNER JOIN appointment_client ac
            ON c.entityid = ac.clientid
        INNER JOIN appointment a 
            ON ac.appointmentid = a.entityid
        left join [user] u on a.trainerid = u.entityId
		left JOIN ClientStatus cs 
			ON c.ClientStatusId = cs.EntityId
GROUP   BY c.entityid ,c.entityId, 
		c.firstName,
		c.lastName,
		c.email,
		c.mobilephone,
        a.Completed,
		cs.AdminAlerted,
				u.FirstName,
				u.LastName
having max(a.date) < DATEADD(DAY, DATEDIFF(DAY, 0, GETDATE()), -10) 
and max(a.date) > DATEADD(DAY, DATEDIFF(DAY, 0, GETDATE()), -17)
and a.Completed = 1
order by u.lastname, u.FirstName ";
            var droppedClientDtos = _repository.CreateSQLQuery<DroppedClientDto>(sql, new List<object>());
            return droppedClientDtos;
        }

        public string CreateEmail(IEnumerable<DroppedClientDto> clients)
        {
            var grouped = clients.GroupBy(c => c.TrainerLastName + ", " + c.TrainerFirstName, c => c,
                              (key, g) => new { Trainer = key, Clients = g.ToList() });
            var email = new StringBuilder("<b>Daily absentee report for {0} </b>".ToFormat(DateTime.Now.AddDays(-10)));
            
            email.Append("<br />");
            grouped.ForEachItem(g =>
            {
                email.Append("<br />");
                email.Append("<b>");
                email.Append(g.Trainer);
                email.Append("</b>");
                email.Append("<br />");
                g.Clients.OrderBy(x => x.LastDate).ForEachItem(x =>
                {
                    email.Append("<br />");
                    email.Append(x.FirstName);
                    email.Append(" ");
                    email.Append(x.LastName);
                    email.Append("'s Last Appointment was on ");
                    email.Append(x.LastDate.ToShortDateString());
                    email.Append(". Their Phone number is ");
                    email.Append(x.MobilePhone);
                    email.Append(". Their Email is ");
                    email.Append("<a href='mailto:");
                    email.Append(x.Email);
                    email.Append("'>");
                    email.Append(x.Email);
                    email.Append("</a>");
                    email.Append("<br />");
                });
            });
            return email.ToString();
        }

        public string CreateWeeklyEmail(IEnumerable<DroppedClientDto> clients)
        {
            var grouped = clients.GroupBy(c => c.TrainerLastName + ", " + c.TrainerFirstName, c => c,
                              (key, g) => new { Trainer = key, Clients = g.ToList() });

            var email = new StringBuilder("<b>Weekly absentee report for {0} - {1} </b>".ToFormat(DateTime.Now.AddDays(-17), DateTime.Now.AddDays(-10)));
            email.Append("<br />");
            grouped.ForEachItem(g =>
                {
                    email.Append("<br />");
                    email.Append("<b>");
                    email.Append(g.Trainer);
                    email.Append("</b>");
                    email.Append("<br />");
                    g.Clients.OrderBy(x => x.LastDate).ForEachItem(x =>
                        {
                            email.Append("<br />");
                            email.Append(x.FirstName);
                            email.Append(" ");
                            email.Append(x.LastName);
                            email.Append("'s Last Appointment was on ");
                            email.Append(x.LastDate.ToShortDateString());
                            email.Append(". Their Phone number is ");
                            email.Append(x.MobilePhone);
                            email.Append(". Their Email is ");
                            email.Append("<a href='mailto:");
                            email.Append(x.Email);
                            email.Append("'>");
                            email.Append(x.Email);
                            email.Append("</a>");
                            email.Append("<br />");
                        });
                });

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

        public void UpdateClients(IEnumerable<DroppedClientDto> clients)
        {
            clients.ForEachItem(x =>
                {
                    var client = _repository.Find<Client>(x.EntityId);
                    if(client.ClientStatus==null){client.ClientStatus=new ClientStatus();}
                    client.ClientStatus.AdminAlerted = true;
                    _repository.Save(client);
                });
            _repository.Commit();
        }
    }
    public class DroppedClientDto
    {
        public int EntityId { get; set; }
        public string TrainerFirstName { get; set; }
        public string TrainerLastName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public DateTime LastDate { get; set; }
    }
}