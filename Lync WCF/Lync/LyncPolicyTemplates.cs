using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lync_WCF
{
    public class LyncPolicyTemplate
    {
        public string PolicyTemplateName { get; set; }
        public string VoicePolicy { get; set; }
        public string VoiceRoutingPolicy { get; set; }
        public string ConferencingPolicy { get; set; }
        public string PresencePolicy { get; set; }
        public string DialPlan { get; set; }
        public string LocationPolicy { get; set; }
        public string ClientPolicy { get; set; }
        public string ClientVersionPolicy { get; set; }
        public string ArchivingPolicy { get; set; }
        public string ExchangeArchivingPolicy { get; set; }
        public string MobilityPolicy { get; set; }
        public string PersistentChatPolicy { get; set; }
        public string UserServicesPolicy { get; set; }
        public string CallViaWorkPolicy { get; set; }
        public string ThirdPartyVideoSystemPolicy { get; set; }
        public string HostedVoicemailPolicy { get; set; }  
    }
}