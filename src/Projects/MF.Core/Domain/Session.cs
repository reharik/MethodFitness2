using System;
using System.ComponentModel.DataAnnotations;
using CC.Core.Localization;
using MF.Core.Enumerations;

namespace MF.Core.Domain
{
    public class Session:DomainEntity
    {
//        [ValidateSqlDateTime]
        public virtual DateTime? Date { get; set; }
        [Required]
        public virtual double Cost { get; set; }
        [Required]
        [ValueOf(typeof(AppointmentType))]
        public virtual string AppointmentType { get; set; }
        [Required]
        public virtual Client Client { get; set; }
        public virtual Appointment Appointment { get; set; }
        public virtual User Trainer { get; set; }
        public virtual bool SessionUsed { get; set; }
        public virtual bool TrainerPaid { get; set; }
        public virtual bool TrainerVerified { get; set; }
        public virtual string PurchaseBatchNumber { get; set; }
        public virtual int TrainerCheckNumber { get; set; }
        public virtual bool InArrears { get; set; }
    }
}