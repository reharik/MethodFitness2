using System.Collections.Generic;

namespace MethodFitness.Web.Areas.Billing.Controllers
{
    public class PaymentDetailsDto
    {
        public double amount { get; set; }
        public IEnumerable<PaymentSessionDetailsDto> items { get; set; }
    }

    public class PaymentSessionDetailsDto
    {
        public long id { get; set; }
        public double amount { get; set; }
    }

}