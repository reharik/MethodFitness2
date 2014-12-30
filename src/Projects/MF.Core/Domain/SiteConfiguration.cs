using System.Configuration;
using CC.Core.Core.Services;
using System;

namespace MF.Core.Domain
{
    public class SiteConfiguration : SiteConfigurationBase
    {
        public override void Initialize()
        {
 	        base.Initialize();
             TrainerClientRateDefault = ConfigurationSettings.AppSettings["TrainerClientRateDefault"];
             LastDayOfPayWeek = ConfigurationSettings.AppSettings["LastDayOfPayWeek"];
             AdminEmail = ConfigurationSettings.AppSettings["AdminEmail"];
             SMTPServer = ConfigurationSettings.AppSettings["SMTPServer"];
             SMTPUN = ConfigurationSettings.AppSettings["SMTPUN"];
             SMTPPW = ConfigurationSettings.AppSettings["SMTPPW"];
             EmailReportAddress = ConfigurationSettings.AppSettings["EmailReportAddress"];
        }

        public virtual string TrainerClientRateDefault { get; set; }
        public virtual string LastDayOfPayWeek { get; set; }
        public virtual string AdminEmail { get; set; }
        public virtual string SMTPServer { get; set; }
        public virtual string SMTPUN { get; set; }
        public virtual string SMTPPW { get; set; }
        public virtual string EmailReportAddress { get; set; }
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