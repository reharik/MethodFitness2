using System.ComponentModel.DataAnnotations;
using CC.Core.Domain;

namespace MF.Core.Domain
{
    public class BaseSessionRate : DomainEntity, IPersistableObject
    {
        [Required]
        public virtual double FullHour { get; set; }
        [Required]
        public virtual double HalfHour { get; set; }
        [Required]
        public virtual double FullHourTenPack { get; set; }
        [Required]
        public virtual double HalfHourTenPack { get; set; }
        [Required]
        public virtual double Pair { get; set; }
        [Required]
        public virtual double PairTenPack { get; set; }
    }
}