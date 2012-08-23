using System.Configuration;

namespace MethodFitness.Core.Domain
{
    public class SessionRates:DomainEntity
    {
        public SessionRates()
        {
        }

        public SessionRates(bool newRates)
        {
            FullHour = double.Parse(ConfigurationSettings.AppSettings["Client.SessionRates.FullHour"]);
            HalfHour = double.Parse(ConfigurationSettings.AppSettings["Client.SessionRates.HalfHour"]);
            FullHourTenPack = double.Parse(ConfigurationSettings.AppSettings["Client.SessionRates.FullHourTenPack"]);
            HalfHourTenPack = double.Parse(ConfigurationSettings.AppSettings["Client.SessionRates.HalfHourTenPack"]);
            Pair = double.Parse(ConfigurationSettings.AppSettings["Client.SessionRates.Pair"]);
            PairTenPack = double.Parse(ConfigurationSettings.AppSettings["Client.SessionRates.PairTenPack"]);
        }

        public virtual double ResetFullHourRate()
        { FullHour = double.Parse(ConfigurationSettings.AppSettings["Client.SessionRates.FullHour"]); return FullHour; }
        public virtual double ResetHalfHourRate()
        { HalfHour = double.Parse(ConfigurationSettings.AppSettings["Client.SessionRates.HalfHour"]); return HalfHour; }
        public virtual double ResetFullHourTenPackRate()
        { FullHourTenPack = double.Parse(ConfigurationSettings.AppSettings["Client.SessionRates.FullHourTenPack"]); return FullHourTenPack; }
        public virtual double ResetHalfHourTenPackRate()
        { HalfHourTenPack = double.Parse(ConfigurationSettings.AppSettings["Client.SessionRates.HalfHourTenPack"]); return HalfHourTenPack; }
        public virtual double ResetPairRate()
        { Pair = double.Parse(ConfigurationSettings.AppSettings["Client.SessionRates.Pair"]); return Pair; }
        public virtual double ResetPairTenPackRate()
        { PairTenPack = double.Parse(ConfigurationSettings.AppSettings["Client.SessionRates.PairTenPack"]); return PairTenPack; }

        
        
        public virtual double FullHour { get; set; }
        public virtual double HalfHour { get; set; }
        public virtual double FullHourTenPack { get; set; }
        public virtual double HalfHourTenPack { get; set; }
        public virtual double Pair { get; set; }
        public virtual double PairTenPack { get; set; }
    }
}