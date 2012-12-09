using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.DomainTools;
using CC.Core.Html;
using CC.Core.Html.Grid;
using CC.Core.Services;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Schedule.Grids;
using MethodFitness.Web.Config;
using MethodFitness.Web.Controllers;
using NHibernate.Linq;

namespace MethodFitness.Web.Areas.Billing.Controllers
{
    using System.Net.Mail;

    using Castle.Core.Smtp;

    using xVal.ServerSide;

    public class TrainerSessionVerificationController : MFController
    {
        private readonly IEntityListGrid<SessionVerificationDto> _grid;
        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;
        private readonly IRepository _repository;
        private readonly ISessionContext _sessionContext;

       


        public TrainerSessionVerificationController(IEntityListGrid<SessionVerificationDto> grid,
            IDynamicExpressionQuery dynamicExpressionQuery,
            IRepository repository,
            ISessionContext sessionContext)
        {
            _grid = grid;
            _dynamicExpressionQuery = dynamicExpressionQuery;
            _repository = repository;
            _sessionContext = sessionContext;
        }

        public ActionResult ItemList(ViewModel input)
        {
            var url = UrlContext.GetUrlForAction<TrainerSessionVerificationController>(x => x.TrainerSessions(null), AreaName.Billing) + "?ParentId=" + input.EntityId;
            var user = (User)input.User;
            var model = new TrainersPaymentListViewModel()
            {
                gridDef = _grid.GetGridDefinition(url, user),
                _Title = user.FullNameFNF + "'s " + WebLocalizationKeys.PAYMENT_AMOUNT,
                TrainersName = user.FullNameFNF,
                EntityId = user.EntityId,
                From = user.Email,
                To = SiteConfig.Settings().AdminEmail,
                Subject = WebLocalizationKeys.PROBLEM_WITH_SESSIONS_ALERT.ToString(),
                Body = WebLocalizationKeys.PROBLEM_WITH_SESSIONS_ALERT_BODY.ToString(),
                AcceptSessionsUrl = UrlContext.GetUrlForAction<TrainerSessionVerificationController>(x => AcceptSessions(null), AreaName.Billing),
                AlertAdminEmailUrl = UrlContext.GetUrlForAction<TrainerSessionVerificationController>(x=>AlertAdminEmail(null),AreaName.Billing)
            };
            return new CustomJsonResult { Data = model };
        }

        public JsonResult AcceptSessions(ViewModel viewModel)
        {
            throw new NotImplementedException();
        }

        public JsonResult AlertAdminEmail(TrainersPaymentListViewModel input)
        {
            var notification = new Notification{Success = true,Message = WebLocalizationKeys.EMAIL_SENT_SUCCESSFULLY.ToString()};
            try
            {
                // pull this shit out and put it in a service fool.
                var message = new MailMessage(new MailAddress(input.From), new MailAddress(input.To))
                                  {
                                      Subject = input.Subject,
                                      Body = input.Body
                                  };
                var smtpClient = new SmtpClient("mail.methodfitness.com", 25);
                smtpClient.Send(message);
            }
            catch (Exception exception)
            {
                notification.Success = false;
                notification.Errors= new List<ErrorInfo>();
                notification.Errors.Add(new ErrorInfo("",exception.Message));
            }
            return new CustomJsonResult { Data = notification };
        }

        public JsonResult TrainerSessions(TrainerPaymentGridItemsRequestModel input)
        {
            var user = ((User) input.User);
            var sessions = user.Sessions.Where(x => !x.TrainerPaid).OrderBy(x=>x.InArrears).ThenBy(x=>x.Client.LastName).ThenBy(x=>x.Appointment.Date);
            var endDate = input.endDate.HasValue ? input.endDate : DateTime.Now;
            var items = _dynamicExpressionQuery.PerformQuery(sessions,input.filters, x=>x.Appointment.Date<=endDate);
            var sessionPaymentDtos = items.Select(x => new SessionVerificationDto
                                                           {
                                                               AppointmentDate = x.Appointment.Date,
                                                               EntityId = x.EntityId,
                                                               FullName = x.Client.FullNameLNF,
                                                               PricePerSession = x.Cost,
                                                               Type = x.AppointmentType,
                                                               TrainerPercentage =
                                                                   x.Trainer.TrainerClientRates.FirstOrDefault(
                                                                       y => y.Client == x.Client)!=null?x.Trainer.TrainerClientRates.FirstOrDefault(
                                                                       y => y.Client == x.Client).Percent:x.Trainer.ClientRateDefault,
                                                               TrainerPay =
                                                                   x.Trainer.TrainerClientRates.FirstOrDefault(
                                                                       y => y.Client == x.Client)!=null?x.Trainer.TrainerClientRates.FirstOrDefault(
                                                                       y => y.Client == x.Client).Percent * .01 * x.Cost : x.Trainer.ClientRateDefault * .01 * x.Cost
                                                           });


            var gridItemsViewModel = _grid.GetGridItemsViewModel(input.PageSortFilter, sessionPaymentDtos,user);
            return new CustomJsonResult { Data = gridItemsViewModel };
        }

        public JsonResult Display(ViewModel input)
        {
            throw new NotImplementedException();
        }
    }

    public class SessionVerificationDto : IGridEnabledClass
    {
        public int EntityId { get; set; }
        public string FullName { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string Type { get; set; }
        public double PricePerSession { get; set; }
        public int TrainerPercentage { get; set; }
        public double TrainerPay { get; set; }
    }
}
