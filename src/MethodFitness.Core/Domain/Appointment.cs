using System;
using Castle.Components.Validator;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Domain.Tools.CustomAttributes;

namespace MethodFitness.Web.Areas.Schedule.Controllers
{
    public class Appointment:DomainEntity
    {
        [ValidateNonEmpty]
        public virtual DateTime? ScheduledDate { get; set; }
        [ValidateNonEmpty]
        public virtual DateTime? ScheduledEndTime { get; set; }
        [ValidateNonEmpty]
        public virtual DateTime? ScheduledStartTime { get; set; }
        [ValidateNonEmpty]
        public virtual Location Location { get; set; }
        [ValidateNonEmpty]
        public virtual User Trainer { get; set; }
        [ValidateNonEmpty]
        public virtual string Client { get; set; }
        [TextArea]
        public virtual string Notes { get; set; }
    }
}