using System.Collections.Generic;
using MethodFitness.Core.Enumerations;
using MethodFitness.Web.Areas.Billing.Controllers;
using MethodFitness.Web.Areas.Schedule.Controllers;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Services.ViewOptions
{
    public interface IViewOptionConfig
    {
        IList<ViewOption> Build(bool withoutPermissions = false);
    }
    public class ScheduleViewOptionList : IViewOptionConfig
    {
        private readonly IViewOptionBuilder _builder;

        public ScheduleViewOptionList(IViewOptionBuilder viewOptionBuilder)
        {
            _builder = viewOptionBuilder;
        }

        public IList<ViewOption> Build(bool withoutPermissions = false)
        {
            _builder.WithoutPermissions(withoutPermissions);
            _builder.Url<OrthogonalController>(x => x.MainMenu()).ViewId("scheduleMenu").End();
            _builder.UrlForForm<AppointmentCalendarController>(x => x.AppointmentCalendar(null),AreaName.Schedule).RouteToken("calendar",AreaName.Schedule).ViewName("CalendarView").IsChild(false).End();

            _builder.UrlForList<ClientListController>(x => x.ItemList(null)).End();
            _builder.UrlForForm<ClientController>(x => x.AddUpdate(null)).ViewName("ClientFormView").End();

            _builder.UrlForList<TrainerListController>(x => x.ItemList(null)).End();
            _builder.UrlForForm<TrainerController>(x => x.AddUpdate(null)).ViewName("TrainerFormView").End();

            _builder.UrlForList<PaymentListController>(x => x.ItemList(null), AreaName.Billing).ViewName("PaymentListView").End();
            _builder.UrlForForm<PaymentController>(x => x.AddUpdate(null),AreaName.Billing).ViewName("PaymentFormView").End();

            return _builder.Items;
        }
    }
}