using System;
using System.Collections.Generic;
using Castle.Components.Validator;
using MethodFitness.Core.Domain.Tools.CustomAttributes;
using MethodFitness.Core.Enumerations;
using MethodFitness.Core.Services;
using System.Linq;
using Rhino.Security;

namespace MethodFitness.Core.Domain
{
    public class  User : Entity, IUser
    {
        public virtual string UserId { get; set; }
        [ValidateNonEmpty]
        public virtual string FirstName { get; set; }
        public virtual string MiddleInitial { get; set; }
        [ValidateNonEmpty]
        public virtual string LastName { get; set; }
        public virtual string Title { get; set; }
        [ValidateSqlDateTime]
        public virtual DateTime? BirthDate { get; set; }
        public virtual string StartPage { get; set; }
        public virtual bool SystemSupport { get; set; }
        public virtual bool Registering { get; set; }

        public virtual string FullNameLNF
        {
            get { return LastName + ", " + FirstName; }
        }
        public virtual string FullNameFNF
        {
            get { return FirstName + " " + LastName; }
        }
       
        #region Collections
        private IList<UserLoginInfo> _userLoginInfos = new List<UserLoginInfo>();
        public virtual void EmptyUserLoginInfos() { _userLoginInfos.Clear(); }
        public virtual IEnumerable<UserLoginInfo> UserLoginInfos { get { return _userLoginInfos; } }
        public virtual void RemoveUserLoginInfo(UserLoginInfo UserLoginInfo)
        {
            _userLoginInfos.Remove(UserLoginInfo);
        }
        public virtual void AddUserLoginInfo(UserLoginInfo UserLoginInfo)
        {
            if (_userLoginInfos.Contains(UserLoginInfo)) return;
            _userLoginInfos.Add(UserLoginInfo);
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
        public virtual bool PasswordExpires { get; set; }
        //IsActive might be used to switch on/off users and use them for 
        // testing or contracting.  Different from Archived on Entity base-class
        public virtual bool IsActive { get; set; }
        //basic user type, used for user listings to identify a usertype (not permissions)
        public virtual UserType UserType { get; set; }
        [ValidateSqlDateTime]
        public virtual DateTime? PasswordExpireDate { get; set; }
        public virtual string Salt { get; set; }
        public virtual bool CanLogin { get; set; }
        [ValidateSqlDateTime]
        public virtual DateTime? LastVisitDate { get; set; }
        [ValidateSqlDateTime]
        public virtual DateTime? CanLoginFrom { get; set; }
        [ValidateSqlDateTime]
        public virtual DateTime? CanLoginTo { get; set; }
        public virtual Guid ByPassToken { get; set; }

        #region Collections
        private IList<UserSubscription> _userSubscriptions = new List<UserSubscription>();
        public virtual void UpdateUserSubscriptionCollection(IUpdateCollectionService service, IEnumerable<UserSubscription> newItems)
        {
            service.Update(UserSubscriptions, newItems, AddUserSubscription, RemoveUserSubscription);
        }
        public virtual void EmptyUserSubscriptions() { _userSubscriptions.Clear(); }
        public virtual IEnumerable<UserSubscription> UserSubscriptions { get { return _userSubscriptions; } }
        public virtual void RemoveUserSubscription(UserSubscription userSubscription)
        {
            _userSubscriptions.Remove(userSubscription);
        }
        public virtual void AddUserSubscription(UserSubscription userSubscription)
        {
            if (_userSubscriptions.Contains(userSubscription)) return;
            _userSubscriptions.Add(userSubscription);
        }
        #endregion

        public virtual UserSubscription GetLatestSubscription()
        {
            return UserSubscriptions.OrderBy(x => x.CreateDate).FirstOrDefault();
        }
    }

    public class UserSubscription : Entity
    {
        public virtual DateTime? BeginDate { get; set; }
        public virtual DateTime? ExpirationDate { get; set; }
        public virtual bool Approved { get; set; }
        public virtual string AuthorizationCode { get; set; }
        public virtual string CardNumber { get; set; }
        public virtual string TransactionId { get; set; }
        public virtual double AmountOfSale { get; set; }
        //current employer/hospital/company ... migrated from legacy Portfolios
        public virtual string Institution { get; set; }

    }
}