using System.Collections.Generic;

namespace MethodFitness.Web.Areas.Billing.Controllers
{
    public class PaymentDetailsDto
    {
        public int id { get; set; }
        public double trainerPay { get; set; }
        public bool _checked { get; set; }
    }
}