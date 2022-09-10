using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CC.Core.Core.CoreViewModelAndDTOs;
using CC.Core.Core.DomainTools;
using CC.Core.Core.Enumerations;
using CC.Core.Core.Html;
using CC.Core.Core.Html.Grid;
using CC.Core.Core.Html.Menu;
using CC.Core.Core.Services;
using CC.Core.Core.ValidationServices;
using CC.Core.DataValidation;
using CC.Core.Utilities;
using MF.Core.CoreViewModelAndDTOs;
using MF.Core.Domain;
using MF.Core.Enumerations;
using MF.Core.Services;
using MF.Web.Areas.Schedule.Grids;
using MF.Web.Areas.Schedule.Models.BulkAction;
using MF.Web.Config;
using MF.Web.Controllers;
using StructureMap;
using System.Net.Mail;

namespace MF.Web.Areas.Billing.Controllers
{
    public class TrainerSessionVerificationController : MFController
    {
        private readonly IEntityListGrid<TrainerSessionDto> _grid;
        private readonly IDynamicExpressionQuery _dynamicExpressionQuery;
        private readonly ISaveEntityService _saveEntityService;
        private readonly IRepository _repository;
        private readonly ISessionContext _sessionContext;
        private readonly IEmailService _emailService;

        public TrainerSessionVerificationController(
            IDynamicExpressionQuery dynamicExpressionQuery,
            ISaveEntityService saveEntityService,
            IRepository repository,
            ISessionContext sessionContext,
            IEmailService emailService)
        {
            _grid = ObjectFactory.Container.GetInstance<IEntityListGrid<TrainerSessionDto>>("SessionVerification");

            _dynamicExpressionQuery = dynamicExpressionQuery;
            _saveEntityService = saveEntityService;
            _repository = repository;
            _sessionContext = sessionContext;
            _emailService = emailService;
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
            if (input.EntityIds == null || !input.EntityIds.Any())
            {
                return new CustomJsonResult(new Notification{Success = false,Message = WebLocalizationKeys.NO_SESSIONS_TO_VERIFY.ToString()});
            }
            var user = (User) input.User;
            user.SessionVerification(input.EntityIds);
            var validationManager = _saveEntityService.ProcessSave(user);
            var notification = validationManager.Finish();
            if (notification.Success)
            {
                var emailDto = new EmailDTO
                {
                    Body = WebLocalizationKeys.TRAINER_SESSIONS_VERIFIED.ToFormat(user.FullNameFNF),
                    Subject = WebLocalizationKeys.TRAINER_SESSIONS_VERIFIED.ToFormat(user.FullNameFNF),
                    From = new MailAddress(Site.Config.AdminEmail),
                    To = new MailAddress(Site.Config.AdminEmail)
                };

                _emailService.SendEmail(emailDto);
            }
            return new CustomJsonResult(notification);
        }

        public JsonResult AlertAdminEmail(TrainersPaymentListViewModel input)
        {
            var notification = new Notification{Success = true,Message = WebLocalizationKeys.EMAIL_SENT_SUCCESSFULLY.ToString()};
            var emailDto = new EmailDTO
                {
                    Body = input.Body,
                    Subject = input.Subject,
                    From = new MailAddress(input.From),
                    To = new MailAddress(input.To),
                    ReplyTo = new MailAddress(input.From)
                };

            var exception = _emailService.SendEmail(emailDto);
            if (exception.IsNotEmpty())
            {
                notification.Success = false;
                notification.Message = "";
                notification.Errors= new List<ErrorInfo> {new ErrorInfo("", exception)};
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
