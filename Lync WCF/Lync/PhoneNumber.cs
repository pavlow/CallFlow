using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lync_WCF
{
    public class PhoneNumber
    {
        public string Number;
        public PoolType Pool;
        public string AccountName;
    }

    public enum PoolType
    {
        Customer,
        General
    }

    public class PhoneNumberType
    {
        public string NumberType;
    }
}