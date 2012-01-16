using System;
using Castle.Components.Validator;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Localization;
using MethodFitness.Web.Areas.Schedule.Controllers;

namespace MethodFitness.Core.Domain
{
    public class Session:DomainEntity
    {
        [ValidateNonEmpty]
        public virtual DateTime Date { get; set; }
        [ValidateNonEmpty]
        public virtual double Cost { get; set; }
        [ValidateNonEmpty]
        [ValueOf(typeof(AppointmentLength))]
        public virtual string Length { get; set; }
        [ValidateNonEmpty]
        public virtual Client Client { get; set; }
        public virtual Appointment Appointment { get; set; }
        public virtual User Trainer { get; set; }
        public virtual bool SessionCompleted { get; set; }
        public virtual bool TrainerPaid { get; set; }
        public virtual string PurchaseBatchNumber { get; set; }
        public virtual int TrainerCheckNumber { get; set; }
        public virtual bool InArrears { get; set; }
    }
}