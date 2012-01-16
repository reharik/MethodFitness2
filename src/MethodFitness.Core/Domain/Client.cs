﻿using System;
using System.Collections.Generic;
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

        #region Collections
        private IList<Session> _sessions = new List<Session>();
        public virtual void EmptySessions() { _sessions.Clear(); }
        public virtual IEnumerable<Session> Sessions { get { return _sessions; } }
        public virtual void RemoveSession(Session session)
        {
            _sessions.Remove(session);
        }
        public virtual void AddSession(Session session)
        {
            if (_sessions.Contains(session)) return;
            _sessions.Add(session);
        }

        #endregion

        public override void UpdateSelf(Entity entity)
        {
            var self = (Client) entity;
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
            Status = self.State;
            ImageUrl = self.ImageUrl;
            Source = self.Source;
            StartDate = self.StartDate;
        }
    }
}   