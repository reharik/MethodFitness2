using CC.Core.Core.Domain;
using CC.Core.Core.DomainTools;
using CC.Core.Core.Services;
using CC.Core.Core.ValidationServices;

namespace MF.Core.Rules
{
    public class NoTrainerHasBeenPaidForPaymentSessions : IRule
    {
        private readonly ISystemClock _systemClock;
        private readonly IRepository _repository;

        public NoTrainerHasBeenPaidForPaymentSessions(ISystemClock systemClock, IRepository repository)
        {
            _systemClock = systemClock;
            _repository = repository;
        }

        public ValidationReport Execute<ENTITY>(ENTITY trainer) where ENTITY : Entity
        {
            var result = new ValidationReport { Success = true };
            var sessions = client.Sessions.Where(x => x.PurchaseBatchNumber == payment.PaymentBatchId.ToString() && x.TrainerPaid);
            if(sessions.Count() > 0)
           {
               result.Success = false;
               result.AddErrorInfo(new ErrorInfo("Rule", CoreLocalizationKeys.PAYMENT_CONTAINS_SESSIONS_THAT_HAVE_BEEN_PAID_OUT.ToFormat(sessions.Count())));
           }
            return result;
        }
    }
}