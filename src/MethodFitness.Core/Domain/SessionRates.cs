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

        }

        public virtual double FullHour { get; set; }
        public virtual double HalfHour { get; set; }
        public virtual double FullHourTenPack { get; set; }
        public virtual double HalfHourTenPack { get; set; }
        public virtual double Pair { get; set; }
    }
}