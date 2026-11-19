using System;
using System.Linq;
using CC.Core.Core.Domain;
using CC.Core.Core.DomainTools;
using CC.Core.Core.Services;
using CC.Core.Core.ValidationServices;
using CC.Core.DataValidation;
using MF.Core.Domain;

namespace MF.Core.Rules
{
    // A payment may be deleted only if
    //  - it carries a batch id, which links it to the sessions it bought or cleared.
    //    Payments entered before the id was stored have a blank one; deleting such a
    //    payment would leave all of its sessions behind.
    //  - no trainer has been paid for any of those sessions.
    public class NoTrainerHasBeenPaidForPaymentSessions : IRule
    {
        private readonly ISystemClock _systemClock;
        private readonly IRepository _repository;

        public NoTrainerHasBeenPaidForPaymentSessions(ISystemClock systemClock, IRepository repository)
        {
            _systemClock = systemClock;
            _repository = repository;
        }

        public ValidationReport Execute<ENTITY>(ENTITY entity) where ENTITY : Entity
        {
            var payment = entity as Payment;
            var result = new ValidationReport { Success = true, entity = payment };
            if (payment.PaymentBatchId == Guid.Empty)
            {
                result.Success = false;
                result.AddErrorInfo(new ErrorInfo("Rule", CoreLocalizationKeys.PAYMENT_PREDATES_DELETING_PAYMENTS.ToString()));
                return result;
            }
            var batch = payment.PaymentBatchId.ToString();
            var paidOut = payment.Client.Sessions.Count(x => x.PurchaseBatchNumber == batch && x.TrainerPaid);
            if (paidOut > 0)
            {
                result.Success = false;
                result.AddErrorInfo(new ErrorInfo("Rule", CoreLocalizationKeys.PAYMENT_CONTAINS_SESSIONS_THAT_HAVE_BEEN_PAID_OUT.ToFormat(paidOut)));
            }
            return result;
        }
    }
}
