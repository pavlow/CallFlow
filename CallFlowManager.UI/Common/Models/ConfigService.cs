using System;
using CallFlowManager.UI.Common;


namespace CallFlowManager.UI.Models
{
    [Serializable] //// for BinaryFormatter
    public sealed class ConfigService : PropertyChangedBase
    {
        static ConfigService _instance;

        [NonSerialized]
        private string _email;
        
        [NonSerialized]
        private string _licenseKey;
        
        [NonSerialized]
        private string _softwareTitle;

        
        public static ConfigService Instance
        {
            get { return _instance ?? (_instance = new ConfigService()); }

        }

        private ConfigService()
        {
            Config = new Config();
            Config.FirstRun = DateTime.Now;
            Config.Email = String.Empty;
            Config.InstanceApp = "CallFlow";
            Config.LicenseKey = String.Empty;
            Config.SoftwareTitle = String.Empty;
        }

        static ConfigService() { }

        public Config Config { get; set; }
    }
}