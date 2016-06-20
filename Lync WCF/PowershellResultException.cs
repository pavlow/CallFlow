using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lync_WCF
{
    public class PowershellResultException : Exception
    {
        public PowershellResultException(string message) : base(message)
        {

        }
    }
}