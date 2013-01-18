using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CC.Core;
using CC.Core.CoreViewModelAndDTOs;
using CC.Core.DomainTools;
using CC.Core.Html;
using CC.Core.Html.Grid;
using CC.Core.Services;
using MethodFitness.Core.CoreViewModelAndDTOs;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Services;
using MethodFitness.Web.Areas.Portfolio.Models.BulkAction;
using MethodFitness.Web.Areas.Schedule.Grids;
using MethodFitness.Web.Config;
using MethodFitness.Web.Controllers;
using NHibernate.Linq;
using StructureMap;

namespace MethodFitness.Web.Areas.Billing.Controllers
{
    using System.Net.Mail;

    using Castle.Core.Smtp;

    using xVal.ServerSide;

    public class TrainerSessionVerificationController : MFController
    {
        private readonly IEntityListGrid<TrainerSessionDto> _grid;
        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;
        private readonly ISaveEntityService _saveEntityService;
        private readonly IRepository _repository;
        private readonly ISessionContext _sessionContext;

        public TrainerSessionVerificationController(
            IDynamicExpressionQuery dynamicExpressionQuery,
            ISaveEntityService saveEntityService,
            IRepository repository,
            ISessionContext sessionContext)
        {
            _grid = ObjectFactory.Container.GetInstance<IEntityListGrid<TrainerSessionDto>>("SessionVerification");

            _dynamicExpressionQuery = dynamicExpressionQuery;
            _saveEntityService = saveEntityService;
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
                To = Site.Config.AdminEmail,
                Subject = WebLocalizationKeys.PROBLEM_WITH_SESSIONS_ALERT.ToString(),
                Body = WebLocalizationKeys.PROBLEM_WITH_SESSIONS_ALERT_BODY.ToString(),
                AcceptSessionsUrl = UrlContext.GetUrlForAction<TrainerSessionVerificationController>(x => AcceptSessions(null), AreaName.Billing),
                AlertAdminEmailUrl = UrlContext.GetUrlForAction<TrainerSessionVerificationController>(x=>AlertAdminEmail(null),AreaName.Billing)
            };
            return new CustomJsonResult(model);
        }

        public JsonResult AcceptSessions(BulkActionViewModel input)
        {
            var user = (User) input.User;
            var verification = new TrainerSessionVerification {Trainer = user};
            //get this from the view then add it to the verification
            var total = 0d;
            // call tolist because were gonna itterate
            user.TrainerClientRates.ToList();
            user.Sessions.Where(x=> input.EntityIds.Contains(x.EntityId)).ToList();
            input.EntityIds.ForEachItem(x =>
                {
                    var session = user.Sessions.FirstOrDefault(y => y.EntityId == x);
                    if (session != null)
                    {
                        session.TrainerVerified = true;
                        verification.AddTrainerApprovedSessionItem(session);
                        var trainerClientRate = user.TrainerClientRates.FirstOrDefault(tcr => tcr.EntityId == session.Client.EntityId);
                        int percent = trainerClientRate != null ? trainerClientRate.Percent:user.ClientRateDefault;
                        verification.Total += session.Cost * (percent*.01);
                    }
                });
            user.AddTrainerSessionVerification(verification);
            var validationManager = _saveEntityService.ProcessSave(user);
            var notification = validationManager.Finish();
            return new CustomJsonResult(notification);
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

//                var smtpClient = new SmtpClient(SiteConfig.Settings().SMTPServer);
//                smtpClient.Credentials = new System.Net.NetworkCredential(SiteConfig.Settings().AdminEmail, SiteConfig.Settings().SMTPPW);
                var smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.Credentials = new System.Net.NetworkCredential("reharik@gmail.com", "m124m124");
                smtpClient.EnableSsl = true;
                
//                var smtpClient = new SmtpClient("mail.methodfitness.com", 25);
                smtpClient.Send(message);
            }
            catch (Exception exception)
            {
                notification.Success = false;
                notification.Message = "";
                notification.Errors= new List<ErrorInfo>();
                notification.Errors.Add(new ErrorInfo("",exception.Message));
            }
            return new CustomJsonResult(notification);
        }

        public JsonResult TrainerSessions(TrainerPaymentGridItemsRequestModel input)
        {
            var endDate = DateTime.Now;
            if (endDate.DayOfWeek != DayOfWeek.Sunday)
            {
                int diff = endDate.DayOfWeek - DayOfWeek.Sunday;
                if (diff < 0)
                {
                    diff += 7;
                }

                endDate = endDate.AddDays(-1 * diff).Date;
            }
            var trainerSessionDtos = _repository.Query<TrainerSessionDto>(
                    x => x.TrainerId == input.User.EntityId && x.AppointmentDate <= endDate && !x.TrainerVerified);

            var items = _dynamicExpressionQuery.PerformQuery(trainerSessionDtos, input.filters);
            var gridItemsViewModel = _grid.GetGridItemsViewModel(input.PageSortFilter, items, input.User);
            return new CustomJsonResult(gridItemsViewModel);
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
        public bool InArrears { get; set; }
    }
}
