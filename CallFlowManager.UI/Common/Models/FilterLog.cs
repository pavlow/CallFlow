using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CallFlowManager.UI.Common;


namespace CallFlowManager.UI.Models
{
    public class FilterLog : PropertyChangedBase
    
    {
        private bool _isTrace;
        private bool _isDebug;
        private bool _isInfo;
        private bool _isWarn;
        private bool _isError;
        private bool _isFatal;
        private bool _isPs;


        public bool IsTrace
        {
            get { return _isTrace; }
            set
            {
                _isTrace = value;
                OnPropertyChanged("IsTrace");
            }
        }

        public bool IsDebug
        {
            get { return _isDebug; }
            set
            {
                _isDebug = value;
                OnPropertyChanged("IsDebug");
            }
        }

        public bool IsInfo
        {
            get { return _isInfo; }
            set
            {
                _isInfo = value;
                OnPropertyChanged("IsInfo");
            }
        }

        public bool IsWarn
        {
            get { return _isWarn; }
            set
            {
                _isWarn = value;
                OnPropertyChanged("IsWarn");
            }
        }

        public bool IsError
        {
            get { return _isError; }
            set
            {
                _isError = value;
                OnPropertyChanged("IsError");
            }
        }

        public bool IsFatal
        {
            get { return _isFatal; }
            set
            {
                _isFatal = value;
                OnPropertyChanged("IsFatal");
            }
        }

        public bool IsPs
        {
            get { return _isPs; }
            set
            {
                _isPs = value;
                OnPropertyChanged("IsPs");
            }
        }
    }
}
