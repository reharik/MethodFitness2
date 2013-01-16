using System.Configuration;
using System.Web.Script.Serialization;
using CC.Core.Services;

namespace MethodFitness.Core.Domain
{
    using System;

    public class SiteConfiguration : SiteConfigurationBase
    {
        public override void Initialize()
        {
 	        base.Initialize();
             TrainerClientRateDefault = ConfigurationSettings.AppSettings["TrainerClientRateDefault"];
             LastDayOfPayWeek = ConfigurationSettings.AppSettings["LastDayOfPayWeek"];
             AdminEmail = ConfigurationSettings.AppSettings["AdminEmail"];
        }

        public virtual string TrainerClientRateDefault { get; set; }
        public virtual string LastDayOfPayWeek { get; set; }
        public virtual string AdminEmail { get; set; }
    }

    public class Site
    {
        private static SiteConfiguration _config;
        public static SiteConfiguration Config
        {
            get
            {
                if (_config.CreatedDate.AddHours(2) <= DateTime.Now)
                {
                    _config.Initialize();
                }
                return _config;
            }
        }

        static Site()
        {
            _config = new SiteConfiguration();
            _config.Initialize();
        }
    }

    public class InjectableSiteConfig : IInjectableSiteConfig
    {
        public SiteConfigurationBase Settings()
        {
            return Site.Config;
        }
    }
}