using System;
using CC.Core.Html.Grid;

namespace MethodFitness.Core.CoreViewModelAndDTOs
{
    public class TrainerSessionDto : IGridEnabledClass
    {
        public int EntityId { get; set; }
        public int TrainerId { get; set; }
        public string FirstlName { get; set; }
        public string LastName { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string Type { get; set; }
        public int TrainerPercentage { get; set; }
        public bool InArrears { get; set; }
        public bool TrainerVerified { get; set; }
        public double PricePerSession { get; set; }
        public double TrainerPay { get { return TrainerPercentage * .01 * PricePerSession; }}
        public string FullName { get { return FirstlName + " " + LastName; } }

    }
}