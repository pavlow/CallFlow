using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lync_WCF
{
    [Serializable]
    public enum PolicyType
    {
        Archiving,
        Client,
        ClientVersion,
        Conferencing,
        ExternalAccess,
        HostedVoiceMail,
        Location,
        Pin,
        Presence,
        Voice,
        DialPlan,
        UserExperience
    }
}