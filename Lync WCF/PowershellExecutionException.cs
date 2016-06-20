using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lync_WCF
{
    public class PowershellExecutionException : Exception
    {
        private string _Script;

        private List<string> _ErrorList;
        public PowershellExecutionException(string Script, Exception InnerEx, List<string> ErrorList)
            : base(string.Format("Unable to run the \"{0}\" PowerShell script. Error: {1}", Script, InnerEx.Message), InnerEx)
        {
            _Script = Script;
            _ErrorList = ErrorList;
        }
        public string Script
        {
            get { return _Script; }
        }
        public Exception Exception
        {
            get { return InnerException; }
        }
        public List<string> ErrorList
        {
            get
            {
                if (_ErrorList == null)
                    _ErrorList = new List<string>();
                return _ErrorList;
            }
        }
    }
}