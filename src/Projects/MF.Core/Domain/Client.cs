using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CC.Core.Core.Domain;
using CC.Core.Core.Enumerations;
using CC.Core.Core.Localization;
using MF.Core.Domain.Tools.CustomAttributes;
using MF.Core.Enumerations;

namespace MF.Core.Domain
{
    public class Client : DomainEntity
    {
        [Required]
        public virtual string FirstName { get; set; }
        [Required]
        public virtual string LastName { get; set; }
        [Required]
        public virtual string Email { get; set; }
        [Required]
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
        [ValueOf(typeof(Source))]
        public virtual string Source { get; set; }
        [TextArea]
        public virtual string SourceOther { get; set; }
        [Required]
        public virtual DateTime StartDate { get; set; }
        public virtual SessionRates SessionRates { get; set; }
        public virtual ClientStatus ClientStatus { get; set; }

        public virtual bool Archived { get; set; }
        public virtual string FullNameLNF
        {
            get { return LastName + ", " + FirstName; }
        }
        public virtual string FullNameFNF
        {
            get { return FirstName + " " + LastName; }
        }

        #region Collections
        private IList<Session> _sessions = new List<Session>();
        public virtual IEnumerable<Session> Sessions { get { return _sessions; } }
        public virtual void RemoveSession(Session session)
        {
            _sessions.Remove(session);
        }
        public virtual void AddSession(Session session)
        {
            _sessions.Add(session);
        }
        private IList<Payment> _payments = new List<Payment>();
        private ClientStatus _clientStatus;
        public virtual IEnumerable<Payment> Payments { get { return _payments; } }

        public virtual void RemovePayment(Payment payment)
        {
            _payments.Remove(payment);
        }
        public virtual void AddPayment(Payment payment)
        {
            if (_payments.Contains(payment)) return;
            _payments.Add(payment);
        }

        #endregion

        public override void UpdateSelf(Entity entityBase)
        {
            var self = (Client) entityBase;
            FirstName = self.FirstName;
            LastName = self.LastName;
            Email = self.Email;
            Address1 = self.Address1;
            Address2 = self.Address2;
            City = self.City;
            State = self.State;
            ZipCode = self.ZipCode;
            MobilePhone = self.MobilePhone;
            SecondaryPhone = self.SecondaryPhone;
            Notes = self.Notes;
            ImageUrl = self.ImageUrl;
            Source = self.Source;
            StartDate = self.StartDate;
        }
    }
}   
