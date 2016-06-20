using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lync_WCF
{
    public class Policies
    {
        private string[] _names = new string[0];

        public Policies()
        {
        }

        public Policies(PolicyType type, string[] names)
        {
            Type = type;
            Names = names;
        }

        public PolicyType Type { get; set; }

        public string[] Names
        {
            get
            {
                return _names;
            }
            set
            {
                if (value != null)
                {
                    _names = value;
                }
            }
        }
    }
}