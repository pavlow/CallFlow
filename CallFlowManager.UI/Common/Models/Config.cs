namespace CallFlowManager.UI.Models
{
    using System;

    // Api license
    [Serializable] //// for BinaryFormatter
    public class Config
    {
        public string Email { get; set; }
        public string InstanceApp { get; set; }
        public string LicenseKey { get; set; }
        public string SoftwareTitle { get; set; }
        public DateTime FirstRun { get; set; }
        public bool IsActive { get; set; }
    }
}