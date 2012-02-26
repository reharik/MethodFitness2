using System;
using System.Collections.Generic;
using Castle.Components.Validator;
using MethodFitness.Core.Domain.Tools.CustomAttributes;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Localization;
using MethodFitness.Core.Services;
using System.Linq;
using Rhino.Security;

namespace MethodFitness.Core.Domain
{
    public class  User : DomainEntity, IUser
    {
        public virtual string UserId { get; set; }
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
        [TextArea]
        public virtual string Notes { get; set; }
        public virtual DateTime? BirthDate { get; set; }
        public virtual string ImageUrl { get; set; }
        [ValueOf(typeof(Status))]
        public virtual string Status { get; set; }
        public virtual string Color { get; set; }
        
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
        }
        public virtual void AddClient(Client client)
        {
            if (_clients.Contains(client)) return;
            _clients.Add(client);
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
        #endregion

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
        [ValidateSqlDateTime]
        public virtual string Salt { get; set; }
        public virtual bool CanLogin { get; set; }
        [ValidateSqlDateTime]
        public virtual DateTime? LastVisitDate { get; set; }
        public virtual Guid ByPassToken { get; set; }

        #region Collections
        #endregion
    }
}