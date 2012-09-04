using AutoMapper;
using MethodFitness.Core.Domain;
using MethodFitness.Web.Areas.Billing.Controllers;
using MethodFitness.Web.Areas.Schedule.Controllers;
using MethodFitness.Web.Controllers;

namespace MethodFitness.Web.Config
{
    public class AutoMapperBootstrapper
    {
        public virtual void BootstrapAutoMapper()
        {
            Mapper.CreateMap<Client, ClientViewModel>();
            Mapper.CreateMap<Payment, PaymentViewModel>();
            Mapper.CreateMap<Appointment, AppointmentViewModel>();
        }

        public static void Bootstrap()
        {
            new AutoMapperBootstrapper().BootstrapAutoMapper();
        }
    }
}