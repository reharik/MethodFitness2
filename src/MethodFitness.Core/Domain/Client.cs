using System;
using Castle.Components.Validator;
using MethodFitness.Core.Domain.Tools.CustomAttributes;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Localization;

namespace MethodFitness.Core.Domain
{
    public class Client:DomainEntity
    {
        [ValidateNonEmpty]
        public virtual string FirstName { get; set; }
        public virtual string MiddleInitial { get; set; }
        [ValidateNonEmpty]
        public virtual string LastName { get; set; }
        [ValidateNonEmpty]
        public virtual string Email { get; set; }
        [ValidateNonEmpty]
        public virtual string MobilePhone { get; set; }
        public virtual string SecondaryPhone { get; set; }
        public virtual string Address1 { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string City { get; set; }
        [ValueOf(typeof(State))]
        public virtual string State { get; set; }
        public virtual string ZipCode { get; set; }
        [TextArea]
        public virtual string Notes { get; set; }
        public virtual DateTime? BirthDate { get; set; }
        public virtual string ImageUrl { get; set; }
        [ValueOf(typeof(Status))]
        public virtual string Status { get; set; }
        [ValueOf(typeof(Source))]
        public virtual string Source { get; set; }
        public virtual DateTime StartDate { get; set; }

        public virtual string FullNameLNF
        {
            get { return LastName + ", " + FirstName; }
        }
        public virtual string FullNameFNF
        {
            get { return FirstName + " " + LastName; }
        }
    }
}