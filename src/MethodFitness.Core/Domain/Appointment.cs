using System;
using MethodFitness.Core.Domain;
using MethodFitness.Core.Domain.Tools.CustomAttributes;

namespace MethodFitness.Web.Areas.Schedule.Controllers
{
    public class Appointment:DomainEntity
    {
        public virtual DateTime? ScheduledDate { get; set; }
        public virtual DateTime? ScheduledEndTime { get; set; }
        public virtual DateTime? ScheduledStartTime { get; set; }
        public virtual Location Location { get; set; }
        public virtual User Trainer { get; set; }
        public virtual string Client { get; set; }
        [TextArea]
        public virtual string Notes { get; set; }
    }
}