using System.Configuration;
using System.Web.Script.Serialization;
using CC.Core.Services;

namespace MethodFitness.Core.Domain
{
    public class SiteConfiguration : SiteConfigurationBase
    {
        public virtual string TrainerClientRateDefault { get; set; }
        public virtual string LastDayOfPayWeek { get; set; }
        public virtual string AdminEmail { get; set; }
        public virtual string SMTPUN { get; set; }
        public virtual string SMTPPW { get; set; }
    }

    public static class SiteConfig
    {
        public static SiteConfiguration Settings()
        {
            var appSetting = ConfigurationSettings.AppSettings["MethodFitness.SiteConfiguration"];
            var jss = new JavaScriptSerializer();
            var siteConfiguration = jss.Deserialize<SiteConfiguration>(appSetting);
            return siteConfiguration;
        }
    }

    public class InjectableSiteConfig : IInjectableSiteConfig
    {
        public SiteConfigurationBase Settings()
        {
            return SiteConfig.Settings();
        }
    }
}