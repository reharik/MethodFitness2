using AutoMapper;
using MF.Core.Domain;
using MF.Web.Areas.Billing.Controllers;
using MF.Web.Areas.Schedule.Controllers;
using MF.Web.Controllers;

namespace MF.Web.Config
{
    public class AutoMapperBootstrapper
    {
        public virtual void BootstrapAutoMapper()
        {
            Mapper.CreateMap<Client, ClientViewModel>();
            Mapper.CreateMap<Payment, PaymentViewModel>();
            Mapper.CreateMap<Appointment, AppointmentViewModel>();
            Mapper.CreateMap<User, TrainerViewModel>();
            Mapper.CreateMap<Location, LocationViewModel>();
        }

        public static void Bootstrap()
        {
            new AutoMapperBootstrapper().BootstrapAutoMapper();
        }
    }
}