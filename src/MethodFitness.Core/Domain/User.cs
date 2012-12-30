using System;
using System.Collections.Generic;
using CC.Core;
using CC.Core.Domain;
using CC.Core.Enumerations;
using CC.Core.Localization;
using CC.Security;
using Castle.Components.Validator;
using MethodFitness.Core.Domain.Tools.CustomAttributes;
using System.Linq;
using MethodFitness.Web.Areas.Billing.Controllers;

namespace MethodFitness.Core.Domain
{
    public class  User : DomainEntity, IUser, IPersistableObject
    {
        [ValidateNonEmpty]
        public virtual string FirstName { get; set; }
        [ValidateNonEmpty]
        public virtual string LastName { get; set; }
        [ValidateNonEmpty]
        public virtual string Email { get; set; }
        [ValidateNonEmpty]
        public virtual string PhoneMobile { get; set; }
        public virtual string SecondaryPhone { get; set; }
        public virtual string Address1 { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string City { get; set; }
        [ValueOf(typeof(State))]
        public virtual string State { get; set; }
        public virtual string ZipCode { get; set; }
        [Tools.CustomAttributes.TextArea]
        public virtual string Notes { get; set; }
        public virtual DateTime? BirthDate { get; set; }
        public virtual string ImageUrl { get; set; }
        public virtual string Color { get; set; }
        public virtual int ClientRateDefault { get; set; }
        
        public virtual UserLoginInfo UserLoginInfo { get; set; }
        public virtual string FullNameLNF
        {
            get { return LastName + ", " + FirstName; }
        }
        public virtual string FullNameFNF
        {
            get { return FirstName + " " + LastName; }
        }
       
        #region Collections
        private IList<UserRole> _userRoles = new List<UserRole>();
        public virtual void EmptyUserRoles() { _userRoles.Clear(); }
        public virtual IEnumerable<UserRole> UserRoles { get { return _userRoles; } }
        public virtual void RemoveUserRole(UserRole userRole)
        {
            _userRoles.Remove(userRole);
        }
        public virtual void AddUserRole(UserRole userRole)
        {
            if (_userRoles.Contains(userRole)) return;
            _userRoles.Add(userRole);
        }

        private IList<Client> _clients = new List<Client>();
        public virtual void EmptyClients() { _clients.Clear(); }
        public virtual IEnumerable<Client> Clients { get { return _clients; } }
        public virtual void RemoveClient(Client client)
        {
            _clients.Remove(client);
            _trainerClientRates.Remove(_trainerClientRates.FirstOrDefault(x => x.Client == client));
        }
        public virtual void AddClient(Client client, int clientRate)
        {
            if (_clients.Contains(client))
            {
                _trainerClientRates.FirstOrDefault(x => x.Client == client).Percent = clientRate;
            }
            else
            {
                _clients.Add(client);
                AddTrainerClientRate(new TrainerClientRate { Trainer = this, Client = client, Percent = clientRate });
            }
        }

        private IList<TrainerClientRate> _trainerClientRates = new List<TrainerClientRate>();
        public virtual void EmptyTrainerClientRates() { _trainerClientRates.Clear(); }
        public virtual IEnumerable<TrainerClientRate> TrainerClientRates { get { return _trainerClientRates; } }
        public virtual void RemoveTrainerClientRate(TrainerClientRate trainerClientRate)
        {
            _trainerClientRates.Remove(trainerClientRate);
        }
        public virtual void AddTrainerClientRate(TrainerClientRate trainerClientRate)
        {
            if (_trainerClientRates.Contains(trainerClientRate)) return;
            _trainerClientRates.Add(trainerClientRate);
        }

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

        private IList<Appointment> _appointments = new List<Appointment>();
        public virtual void EmptyAppointments() { _appointments.Clear(); }
        public virtual IEnumerable<Appointment> Appointments { get { return _appointments; } }
        public virtual void RemoveAppointment(Appointment appointment)
        {
            _appointments.Remove(appointment);
        }
        public virtual void AddAppointment(Appointment appointment)
        {
            if (_appointments.Contains(appointment)) return;
            _appointments.Add(appointment);
        }

        private IList<TrainerPayment> _trainerPayments = new List<TrainerPayment>();
        public virtual void EmptyTrainerPayments() { _trainerPayments.Clear(); }
        public virtual IEnumerable<TrainerPayment> TrainerPayments { get { return _trainerPayments; } }


        public virtual void RemoveTrainerPayment(TrainerPayment trainerPayment)
        {
            _trainerPayments.Remove(trainerPayment);
        }
        public virtual void AddTrainerPayment(TrainerPayment trainerPayment)
        {
            if (_trainerPayments.Contains(trainerPayment)) return;
            _trainerPayments.Add(trainerPayment);
        }

        private IList<TrainerSessionVerification> _trainerSessionVerifications = new List<TrainerSessionVerification>();
        public virtual IEnumerable<TrainerSessionVerification> TrainerSessionVerifications { get { return _trainerSessionVerifications; } }
        public virtual void RemoveTrainerSessionVerification(TrainerSessionVerification trainerSessionVerification)
        {
            _trainerSessionVerifications.Remove(trainerSessionVerification);
        }
        public virtual void AddTrainerSessionVerification(TrainerSessionVerification trainerSessionVerification)
        {
            if (_trainerSessionVerifications.Contains(trainerSessionVerification)) return;
            _trainerSessionVerifications.Add(trainerSessionVerification);
        }
        #endregion

        public virtual TrainerPayment PayTrainer(IEnumerable<PaymentDetailsDto> items, double amount)
        {
            if (items == null || !items.Any()) return null;
            var trainerPayment = new TrainerPayment
            {
                Trainer = this,
                Total = amount
            };
            items.ForEachItem(x =>
            {
                var session = Sessions.First(y => y.EntityId == x.id);
                session.TrainerPaid = true;
                trainerPayment.AddTrainerPaymentSessionItem(new TrainerPaymentSessionItem
                {
                    Appointment = session.Appointment,
                    AppointmentCost = session.Cost,
                    Client = session.Client,
                    TrainerPay = x.trainerPay
                });
            });
            AddTrainerPayment(trainerPayment);
            return trainerPayment;
        }

        public virtual SecurityInfo SecurityInfo
        {
            get { return new SecurityInfo(FullNameLNF, EntityId); }
        }
    }

    public class UserLoginInfo : DomainEntity
    {
        [ValidateNonEmpty]
        public virtual string LoginName { get; set; }
        [ValidateNonEmpty]
        public virtual string Password { get; set; }
        public virtual string Salt { get; set; }
        [ValidateSqlDateTime]
        public virtual DateTime? LastVisitDate { get; set; }
        public virtual Guid ByPassToken { get; set; }

        #region Collections
        #endregion
    }
}