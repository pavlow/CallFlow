using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lync_WCF
{
    public class LyncPsFactory : PsFactory
    {
        private static readonly string[] PsModules = new string[] { "Lync", "LyncOnline", "ActiveDirectory" };
        private static readonly string ScriptFolder = AppDomain.CurrentDomain.BaseDirectory + "Scripts\\";
        public LyncPsFactory()
            : base(PsModules) //, ScriptFolder - also referenced in PsFactory.cs
        {

        }
    }
}