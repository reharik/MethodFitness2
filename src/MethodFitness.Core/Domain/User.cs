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
    public class  User : DomainEntity, IUser
    {
        public virtual string UserId { get; set; }
        [ValidateNonEmpty]
        public virtual string FirstName { get; set; }
        public virtual string MiddleInitial { get; set; }
        [ValidateNonEmpty]
        public virtual string LastName { get; set; }
        [ValidateSqlDateTime]
        public virtual DateTime? BirthDate { get; set; }

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
        public virtual UserType UserType { get; set; }
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