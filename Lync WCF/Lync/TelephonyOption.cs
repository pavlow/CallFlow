using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lync_WCF
{
    public enum TelephonyOption
    {
        PcToPcOnly = 0,
        RemoteCallControl = 1,
        EnterpriseVoice = 2,
        AudioVideoDisabled = 3,
        RemoteCallControlOnly = 4
    }
}