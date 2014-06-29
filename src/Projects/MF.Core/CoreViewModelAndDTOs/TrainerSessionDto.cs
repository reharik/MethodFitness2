using System;
using CC.Core.Html.Grid;

namespace MF.Core.CoreViewModelAndDTOs
{
    public class TrainerSessionDto : IGridEnabledClass
    {
        public virtual int EntityId { get; set; }
        public virtual int TrainerId { get; set; }
        public virtual int TrainerSessionVerificationId { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual DateTime? AppointmentDate { get; set; }
        public virtual string Type { get; set; }
        public virtual int TrainerPercentage { get; set; }
        public virtual bool InArrears { get; set; }
        public virtual bool TrainerVerified { get; set; }
        public virtual bool TrainerPaid { get; set; }
        public virtual double PricePerSession { get; set; }
        public virtual double TrainerPay { get { return TrainerPercentage * .01 * PricePerSession; }}
        public virtual string FullName { get { return FirstName + " " + LastName; } }

    }
}