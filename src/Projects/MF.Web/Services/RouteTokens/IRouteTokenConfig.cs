using System.Collections.Generic;
using CC.Core.Core.Enumerations;
using MF.Core.Enumerations;
using MF.Web.Areas.Billing.Controllers;
using MF.Web.Areas.Reporting.Controllers;
using MF.Web.Areas.Schedule.Controllers;
using MF.Web.Controllers;

namespace MF.Web.Services.RouteTokens
{
    public interface IRouteTokenConfig
    {
        IList<RouteToken> Build(bool withoutPermissions = false);
    }
    public class ScheduleRouteTokenList : IRouteTokenConfig
    {
        private readonly IRouteTokenBuilder _builder;

        public ScheduleRouteTokenList(IRouteTokenBuilder routeTokenBuilder)
        {
            _builder = routeTokenBuilder;
        }

        public IList<RouteToken> Build(bool withoutPermissions = false)
        {
            _builder.WithoutPermissions(withoutPermissions);
            _builder.Url<OrthogonalController>(x => x.MainMenu()).ViewId("scheduleMenu").End();
            _builder.UrlForForm<AppointmentCalendarController>(x => x.AppointmentCalendar(null),AreaName.Schedule).RouteToken("calendar").ViewName("CalendarView").IsChild(false).End();

            _builder.UrlForList<ClientListController>(x => x.ItemList(null)).ViewName("ClientGridView").End();
            _builder.UrlForForm<ClientController>(x => x.AddUpdate(null)).ViewName("ClientFormView").End();

            _builder.UrlForList<TrainerListController>(x => x.ItemList(null)).ViewName("TrainerGridView").End();
            _builder.UrlForForm<TrainerController>(x => x.AddUpdate(null)).ViewName("TrainerFormView").End();

            _builder.UrlForList<PaymentListController>(x => x.ItemList(null), AreaName.Billing).ViewName("PaymentListView").End();
            _builder.UrlForForm<PaymentController>(x => x.AddUpdate(null), AreaName.Billing).ViewName("PaymentFormView").End();
            _builder.UrlForDisplay<PaymentController>(x => x.Display(null), AreaName.Billing).End();

            _builder.UrlForList<PayTrainerListController>(x => x.ItemList(null), AreaName.Billing).ViewName("PayTrainerGridView").IsChild().End();

            _builder.UrlForList<TrainerSessionViewController>(x => x.ItemList(null), AreaName.Billing).ViewName("TrainerSessionView").End();
            _builder.UrlForList<TrainerSessionVerificationListController>(x => x.ItemList(null), AreaName.Billing).DisplayToken("VerifiedTrainerSessions").NoMultiSelectGridView().End();
            _builder.UrlForList<VerifiedTrainerSessionsController>(x => x.ItemList(null), AreaName.Billing).NoMultiSelectGridView().End();
            _builder.UrlForList<TrainerSessionVerificationController>(x => x.ItemList(null), AreaName.Billing).ViewName("TrainerSessionVerificationView").End();
            _builder.UrlForDisplay<TrainerSessionVerificationController>(x => x.Display(null), AreaName.Billing).End();
            //_builder.UrlForForm<PayTrainerController>(x => x.AddUpdate(null), AreaName.Billing).End();

            _builder.UrlForList<TrainerPaymentListController>(x => x.ItemList(null), AreaName.Billing).ViewName("TrainerPaymentListGridView").IsChild().End();
            _builder.UrlForForm<TrainerPaymentController>(x => x.Display(null), AreaName.Billing).End();

            _builder.UrlForForm<DailyPaymentsController>(x => x.Display(null), AreaName.Reporting).ViewName("DailyPaymentsView").End();
            _builder.UrlForForm<TrainerMetricController>(x => x.Display(null), AreaName.Reporting).ViewName("TrainerMetricView").End();
            _builder.UrlForForm<ActivityController>(x => x.Display(null), AreaName.Reporting).ViewName("ActivityView").End();

            _builder.UrlForList<LocationListController>(x => x.ItemList(null)).End();
            _builder.UrlForForm<LocationController>(x => x.AddUpdate(null)).End();

            _builder.UrlForForm<BaseSessionRateController>(x => x.AddUpdate(null), AreaName.Billing).NoBubbleUp().End();

            return _builder.Items;
        }
    }
}
