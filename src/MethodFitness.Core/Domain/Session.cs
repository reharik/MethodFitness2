using System;
using CC.Core.Domain;
using CC.Core.Localization;
using Castle.Components.Validator;
using MethodFitness.Core.Domain.Tools.CustomAttributes;
using MethodFitness.Core.Enumerations;

namespace MethodFitness.Core.Domain
{
    public class Session:DomainEntity
    {
        [ValidateSqlDateTime]
        public virtual DateTime? Date { get; set; }
        [ValidateNonEmpty]
        public virtual double Cost { get; set; }
        [ValidateNonEmpty]
        [ValueOf(typeof(AppointmentType))]
        public virtual string AppointmentType { get; set; }
        [ValidateNonEmpty]
        public virtual Client Client { get; set; }
        public virtual Appointment Appointment { get; set; }
        public virtual User Trainer { get; set; }
        public virtual bool SessionUsed { get; set; }
        public virtual bool TrainerPaid { get; set; }
        public virtual string PurchaseBatchNumber { get; set; }
        public virtual int TrainerCheckNumber { get; set; }
        public virtual bool InArrears { get; set; }
    }
}