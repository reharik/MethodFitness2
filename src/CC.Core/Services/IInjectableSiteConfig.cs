namespace CC.Core.Services
{
    using System;
    using System.Configuration;

    public interface IInjectableSiteConfig
    {
        SiteConfigurationBase Settings();
    }

   
    public class SiteConfigurationBase 
    {
        public virtual void Initialize()
        {
            BuildNumber = ConfigurationSettings.AppSettings["BuildNumber"];
            Debug = ConfigurationSettings.AppSettings["Debug"].IsNotEmpty() ? bool.Parse(ConfigurationSettings.AppSettings["Debug"]) : false;
            Name = ConfigurationSettings.AppSettings["Name"];
            Host = ConfigurationSettings.AppSettings["Host"];
            LanguageDefault = ConfigurationSettings.AppSettings["LanguageDefault"];
            ScriptsPath = ConfigurationSettings.AppSettings["ScriptsPath"];
            CssPath = ConfigurationSettings.AppSettings["CssPath"];
            ImagesPath = ConfigurationSettings.AppSettings["ImagesPath"];
            WebSiteRoot = ConfigurationSettings.AppSettings["WebSiteRoot"];
            jsApplicationName = ConfigurationSettings.AppSettings["jsApplicationName"];
            SMTPServer = ConfigurationSettings.AppSettings["SMTPServer"];
            SMTPUserName = ConfigurationSettings.AppSettings["SMTPUserName"];
            SMTPPassword = ConfigurationSettings.AppSettings["SMTPPassword"];
            SMTPPort = ConfigurationSettings.AppSettings["SMTPPort"].IsNotEmpty() ? Int32.Parse(ConfigurationSettings.AppSettings["SMTPPort"]) : 0;
            CreatedDate = DateTime.Now;
        }

        public virtual DateTime CreatedDate { get; set; }
        public virtual string BuildNumber { get; set; }
        public virtual bool Debug { get; set; }
        public virtual string Name { get; set; }
        public virtual string Host { get; set; }
        public virtual string LanguageDefault { get; set; }
        public virtual string ScriptsPath { get; set; }
        public virtual string CssPath { get; set; }
        public virtual string ImagesPath { get; set; }
        public virtual string WebSiteRoot { get; set; }
        public virtual string jsApplicationName { get; set; }
        public virtual string SMTPServer { get; set; }
        public virtual string SMTPUserName { get; set; }
        public virtual string SMTPPassword { get; set; }
        public virtual int SMTPPort { get; set; }
    }
}