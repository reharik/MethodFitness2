using System;
using CC.Core.Html.Grid;

namespace MethodFitness.Core.CoreViewModelAndDTOs
{
    public class SessionPaymentDto : IGridEnabledClass
    {
        public int EntityId { get; set; }
        public string FullName { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string Type { get; set; }
        public double PricePerSession { get; set; }
        public int TrainerPercentage { get; set; }
        public double TrainerPay { get; set; }
        public bool InArrears { get; set; }
        public bool TrainerVerified { get; set; }
    }
}